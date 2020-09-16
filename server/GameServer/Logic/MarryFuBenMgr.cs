using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	internal class MarryFuBenMgr : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		
		public static MarryFuBenMgr getInstance()
		{
			return MarryFuBenMgr.instance;
		}

		
		public bool initialize()
		{
			MarriageOtherLogic.getInstance().init();
			this.ManAndWifeBossXmlItems.LoadFromXMlFile("Config/ManAndWifeBoss.xml", "", "MonsterID", 0);
			TCPCmdDispatcher.getInstance().registerProcessorEx(870, 1, 2, MarryFuBenMgr.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(18, MarryFuBenMgr.getInstance());
			GlobalEventSource.getInstance().registerListener(12, MarryFuBenMgr.getInstance());
			return true;
		}

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			GlobalEventSource.getInstance().removeListener(18, MarryFuBenMgr.getInstance());
			GlobalEventSource.getInstance().removeListener(12, MarryFuBenMgr.getInstance());
			if (null != this.MarriageInstanceDic)
			{
				lock (this.MarriageInstanceDic)
				{
					this.MarriageInstanceDic.Clear();
				}
			}
			MarriageOtherLogic.getInstance().destroy();
			return true;
		}

		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 18)
			{
				Monster monster = (eventObject as MonsterBlooadChangedEventObject).getMonster();
				GameClient client = (eventObject as MonsterBlooadChangedEventObject).getGameClient();
				if (monster != null && null != client)
				{
					if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0 && MapTypes.MarriageCopy == Global.GetMapType(client.ClientData.MapCode) && MapTypes.MarriageCopy == Global.GetMapType(monster.CurrentMapCode))
					{
						SystemXmlItem XMLItem = null;
						if (this.ManAndWifeBossXmlItems.SystemXmlItemDict.TryGetValue(monster.MonsterInfo.ExtensionID, out XMLItem) && null != XMLItem)
						{
							if (XMLItem.GetIntValue("Need", -1) != (int)client.ClientData.MyMarriageData.byMarrytype)
							{
								BufferData bufferData = Global.GetMonsterBufferDataByID(monster, XMLItem.GetIntValue("GoodsID", -1));
								if (bufferData == null || Global.IsBufferDataOver(bufferData, 0L))
								{
									double[] newActionParams = new double[]
									{
										15.0,
										1.0
									};
									EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(2000808);
									if (null != item)
									{
										newActionParams[1] = item.ExtProps[24];
									}
									Global.UpdateMonsterBufferData(monster, BufferItemTypes.MU_MARRIAGE_SUBDAMAGEPERCENTTIMER, newActionParams);
									string text = string.Format(GLang.GetLang(484, new object[0]), client.ClientData.RoleName, monster.MonsterInfo.VSName);
									GameManager.ClientMgr.BroadSpecialHintText(monster.CurrentMapCode, monster.CurrentCopyMapID, text);
								}
							}
						}
					}
				}
			}
			else if (eventObject.getEventType() == 12)
			{
				GameClient client = (eventObject as PlayerLogoutEventObject).getPlayer();
				this.ClientExitRoom(client);
			}
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			if (nID == 870)
			{
				int nSelect = 0;
				try
				{
					nSelect = Global.SafeConvertToInt32(cmdParams[0]);
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, "ProcessMarryFuben", false, false);
				}
				int[] iRet = null;
				if (!MarryLogic.IsVersionSystemOpenOfMarriage())
				{
					iRet = new int[]
					{
						6
					};
					client.sendCmd<int[]>(nID, iRet, false);
				}
				else
				{
					if (1 == nSelect)
					{
						iRet = new int[]
						{
							(int)this.GetMarriageInstanceState(client, null)
						};
					}
					else if (2 == nSelect)
					{
						iRet = new int[]
						{
							(int)this.ClientEnterRoom(client)
						};
					}
					else if (3 == nSelect)
					{
						iRet = new int[]
						{
							(int)this.ClientExitRoom(client)
						};
					}
					else if (4 == nSelect)
					{
						iRet = new int[]
						{
							(int)this.ClientReady(client, Global.SafeConvertToInt32(cmdParams[1]))
						};
					}
					else if (5 == nSelect)
					{
						iRet = new int[]
						{
							(int)this.ClientExitReady(client)
						};
					}
					client.sendCmd<int[]>(nID, iRet, false);
				}
			}
			return true;
		}

		
		private MarriageInstance GetMarriageInstance(GameClient client)
		{
			MarriageInstance result;
			if (null == client)
			{
				result = null;
			}
			else
			{
				lock (this.MarriageInstanceDic)
				{
					MarriageInstance FubenInstance = null;
					this.MarriageInstanceDic.TryGetValue(client.ClientData.RoleID, out FubenInstance);
					if (null == FubenInstance)
					{
						this.MarriageInstanceDic.TryGetValue(client.ClientData.MyMarriageData.nSpouseID, out FubenInstance);
					}
					result = FubenInstance;
				}
			}
			return result;
		}

		
		private MarryFubenResult GetMarriageInstanceState(GameClient client, MarriageInstance FubenInstance = null)
		{
			MarryFubenResult result;
			if (null == client)
			{
				result = MarryFubenResult.Error;
			}
			else
			{
				int[] RetArry = new int[6];
				if (null == FubenInstance)
				{
					FubenInstance = this.GetMarriageInstance(client);
				}
				if (null != FubenInstance)
				{
					RetArry = new int[]
					{
						FubenInstance.nHusband_ID,
						FubenInstance.nHusband_state,
						FubenInstance.nWife_ID,
						FubenInstance.nWife_state,
						FubenInstance.nHusband_FuBenID,
						FubenInstance.nWife_FuBenID
					};
					client.sendCmd<int[]>(870, RetArry, false);
					result = MarryFubenResult.Success;
				}
				else
				{
					int[] array = new int[6];
					array[0] = -1;
					array[2] = -1;
					RetArry = array;
					client.sendCmd<int[]>(870, RetArry, false);
					result = MarryFubenResult.Success;
				}
			}
			return result;
		}

		
		public MarryFubenResult ClientEnterRoom(GameClient client)
		{
			MarryFubenResult result;
			if (!client.ClientData.IsMainOccupation)
			{
				result = MarryFubenResult.Error_Denied_For_Minor_Occupation;
			}
			else if (-1 == client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryFubenResult.NotMarriaged;
			}
			else if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
			{
				result = MarryFubenResult.InFuben;
			}
			else
			{
				SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				if (sceneType == SceneUIClasses.ShuiJingHuanJing || sceneType == SceneUIClasses.GuZhanChang)
				{
					result = MarryFubenResult.InFuben;
				}
				else
				{
					GameClient Spouseclient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
					MarriageInstance FubenInstance = this.GetMarriageInstance(client);
					lock (this.MarriageInstanceDic)
					{
						if (null == FubenInstance)
						{
							FubenInstance = new MarriageInstance();
							FubenInstance.nCreateRole_ID = client.ClientData.RoleID;
							if (1 == client.ClientData.MyMarriageData.byMarrytype)
							{
								FubenInstance.nHusband_ID = client.ClientData.RoleID;
								FubenInstance.nHusband_state = 0;
							}
							else if (2 == client.ClientData.MyMarriageData.byMarrytype)
							{
								FubenInstance.nWife_ID = client.ClientData.RoleID;
								FubenInstance.nWife_state = 0;
							}
							this.MarriageInstanceDic.Add(FubenInstance.nCreateRole_ID, FubenInstance);
							this.GetMarriageInstanceState(client, FubenInstance);
							this.GetMarriageInstanceState(Spouseclient, FubenInstance);
							result = MarryFubenResult.Success;
						}
						else
						{
							if (1 == client.ClientData.MyMarriageData.byMarrytype)
							{
								FubenInstance.nHusband_ID = client.ClientData.RoleID;
								FubenInstance.nHusband_FuBenID = 0;
								FubenInstance.nHusband_state = 0;
							}
							else if (2 == client.ClientData.MyMarriageData.byMarrytype)
							{
								FubenInstance.nWife_ID = client.ClientData.RoleID;
								FubenInstance.nWife_FuBenID = 0;
								FubenInstance.nWife_state = 0;
							}
							this.GetMarriageInstanceState(client, FubenInstance);
							this.GetMarriageInstanceState(Spouseclient, FubenInstance);
							result = MarryFubenResult.Success;
						}
					}
				}
			}
			return result;
		}

		
		public MarryFubenResult ClientExitRoom(GameClient client)
		{
			MarryFubenResult result;
			if (-1 == client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryFubenResult.NotMarriaged;
			}
			else
			{
				GameClient Spouseclient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
				MarriageInstance FubenInstance = this.GetMarriageInstance(client);
				if (null != FubenInstance)
				{
					if (1 == client.ClientData.MyMarriageData.byMarrytype)
					{
						FubenInstance.nHusband_ID = -1;
						FubenInstance.nHusband_state = -1;
						FubenInstance.nHusband_FuBenID = 0;
					}
					else
					{
						FubenInstance.nWife_ID = -1;
						FubenInstance.nWife_state = -1;
						FubenInstance.nWife_FuBenID = 0;
					}
					if (-1 == FubenInstance.nHusband_ID && -1 == FubenInstance.nWife_ID)
					{
						this.RemoveMarriageInstance(FubenInstance, false);
						FubenInstance = null;
					}
					this.GetMarriageInstanceState(client, FubenInstance);
					this.GetMarriageInstanceState(Spouseclient, FubenInstance);
					result = MarryFubenResult.Success;
				}
				else
				{
					result = MarryFubenResult.Error;
				}
			}
			return result;
		}

		
		public MarryFubenResult ClientReady(GameClient client, int FuBenID)
		{
			MarryFubenResult result;
			if (!client.ClientData.IsMainOccupation)
			{
				result = MarryFubenResult.Error_Denied_For_Minor_Occupation;
			}
			else if (-1 == client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryFubenResult.NotMarriaged;
			}
			else if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
			{
				result = MarryFubenResult.InFuben;
			}
			else
			{
				SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				if (sceneType == SceneUIClasses.ShuiJingHuanJing || sceneType == SceneUIClasses.GuZhanChang)
				{
					result = MarryFubenResult.InFuben;
				}
				else
				{
					GameClient Spouseclient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
					MarriageInstance FubenInstance = this.GetMarriageInstance(client);
					if (null == FubenInstance)
					{
						result = MarryFubenResult.Error;
					}
					else if (1 == FubenInstance.nHusband_state && 1 == FubenInstance.nWife_state && FubenInstance.nHusband_FuBenID == FubenInstance.nWife_FuBenID)
					{
						result = MarryFubenResult.IsReaday;
					}
					else
					{
						if (1 == client.ClientData.MyMarriageData.byMarrytype)
						{
							FubenInstance.nHusband_state = 1;
							FubenInstance.nHusband_FuBenID = FuBenID;
						}
						else if (2 == client.ClientData.MyMarriageData.byMarrytype)
						{
							FubenInstance.nWife_state = 1;
							FubenInstance.nWife_FuBenID = FuBenID;
						}
						this.GetMarriageInstanceState(client, FubenInstance);
						this.GetMarriageInstanceState(Spouseclient, FubenInstance);
						result = MarryFubenResult.Success;
					}
				}
			}
			return result;
		}

		
		public MarryFubenResult ClientExitReady(GameClient client)
		{
			MarryFubenResult result;
			if (-1 == client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryFubenResult.NotMarriaged;
			}
			else
			{
				GameClient Spouseclient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
				MarriageInstance FubenInstance = this.GetMarriageInstance(client);
				if (null != FubenInstance)
				{
					if (1 == FubenInstance.nHusband_state && 1 == FubenInstance.nWife_state && FubenInstance.nHusband_FuBenID == FubenInstance.nWife_FuBenID)
					{
						result = MarryFubenResult.IsReaday;
					}
					else
					{
						if (1 == client.ClientData.MyMarriageData.byMarrytype)
						{
							FubenInstance.nHusband_state = 0;
							FubenInstance.nHusband_FuBenID = 0;
						}
						else
						{
							FubenInstance.nWife_state = 0;
							FubenInstance.nWife_FuBenID = 0;
						}
						this.GetMarriageInstanceState(client, FubenInstance);
						this.GetMarriageInstanceState(Spouseclient, FubenInstance);
						result = MarryFubenResult.Success;
					}
				}
				else
				{
					result = MarryFubenResult.Error;
				}
			}
			return result;
		}

		
		public void StartInstance(GameClient client)
		{
			this.ClientExitRoom(client);
		}

		
		private void RemoveMarriageInstance(MarriageInstance FubenInstance, bool bNeedsendtoclient = false)
		{
			lock (this.MarriageInstanceDic)
			{
				if (bNeedsendtoclient)
				{
					GameClient Husbandclient = GameManager.ClientMgr.FindClient(FubenInstance.nHusband_ID);
					this.GetMarriageInstanceState(Husbandclient, FubenInstance);
					GameClient Wifeclient = GameManager.ClientMgr.FindClient(FubenInstance.nWife_ID);
					this.GetMarriageInstanceState(Wifeclient, FubenInstance);
				}
				this.MarriageInstanceDic.Remove(FubenInstance.nCreateRole_ID);
			}
		}

		
		public static bool UpdateMarriageData2DB(GameClient client)
		{
			return MarryFuBenMgr.UpdateMarriageData2DB(client.ClientData.RoleID, client.ClientData.MyMarriageData, client);
		}

		
		public static bool UpdateMarriageData2DB(int nRoleID, MarriageData updateMarriageData, GameClient self)
		{
			byte[] dataBytes = DataHelper.ObjectToBytes<MarriageData>(updateMarriageData);
			byte[] byRoleID = BitConverter.GetBytes(nRoleID);
			byte[] sendBytes = new byte[dataBytes.Length + 4];
			Array.Copy(byRoleID, sendBytes, 4);
			Array.Copy(dataBytes, 0, sendBytes, 4, dataBytes.Length);
			return Global.sendToDB<bool, byte[]>(10185, sendBytes, self.ServerId);
		}

		
		public bool CanEnterSceneEX(GameClient client)
		{
			MarriageInstance FubenInstance = this.GetMarriageInstance(client);
			GameClient clienth = GameManager.ClientMgr.FindClient(FubenInstance.nHusband_ID);
			bool result;
			if (clienth == null)
			{
				result = false;
			}
			else
			{
				GameClient clientw = GameManager.ClientMgr.FindClient(FubenInstance.nWife_ID);
				if (clientw == null)
				{
					result = false;
				}
				else if (1 != FubenInstance.nHusband_state || 1 != FubenInstance.nWife_state)
				{
					this.RemoveMarriageInstance(FubenInstance, true);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		
		public MarriageInstance GetMarriageInstanceEX(GameClient client)
		{
			return this.GetMarriageInstance(client);
		}

		
		private static MarryFuBenMgr instance = new MarryFuBenMgr();

		
		private Dictionary<int, MarriageInstance> MarriageInstanceDic = new Dictionary<int, MarriageInstance>();

		
		private SystemXmlItem MarriageFubenXmlItem = null;

		
		private SystemXmlItems ManAndWifeBossXmlItems = new SystemXmlItems();
	}
}
