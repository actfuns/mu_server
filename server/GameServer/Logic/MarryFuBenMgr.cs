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
	// Token: 0x02000523 RID: 1315
	internal class MarryFuBenMgr : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		// Token: 0x060018EE RID: 6382 RVA: 0x00185300 File Offset: 0x00183500
		public static MarryFuBenMgr getInstance()
		{
			return MarryFuBenMgr.instance;
		}

		// Token: 0x060018EF RID: 6383 RVA: 0x00185318 File Offset: 0x00183518
		public bool initialize()
		{
			MarriageOtherLogic.getInstance().init();
			this.ManAndWifeBossXmlItems.LoadFromXMlFile("Config/ManAndWifeBoss.xml", "", "MonsterID", 0);
			TCPCmdDispatcher.getInstance().registerProcessorEx(870, 1, 2, MarryFuBenMgr.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(18, MarryFuBenMgr.getInstance());
			GlobalEventSource.getInstance().registerListener(12, MarryFuBenMgr.getInstance());
			return true;
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x00185390 File Offset: 0x00183590
		public bool startup()
		{
			return true;
		}

		// Token: 0x060018F1 RID: 6385 RVA: 0x001853A4 File Offset: 0x001835A4
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060018F2 RID: 6386 RVA: 0x001853B8 File Offset: 0x001835B8
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

		// Token: 0x060018F3 RID: 6387 RVA: 0x0018544C File Offset: 0x0018364C
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

		// Token: 0x060018F4 RID: 6388 RVA: 0x00185650 File Offset: 0x00183850
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x060018F5 RID: 6389 RVA: 0x00185664 File Offset: 0x00183864
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

		// Token: 0x060018F6 RID: 6390 RVA: 0x001857BC File Offset: 0x001839BC
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

		// Token: 0x060018F7 RID: 6391 RVA: 0x0018585C File Offset: 0x00183A5C
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

		// Token: 0x060018F8 RID: 6392 RVA: 0x0018592C File Offset: 0x00183B2C
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

		// Token: 0x060018F9 RID: 6393 RVA: 0x00185B9C File Offset: 0x00183D9C
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

		// Token: 0x060018FA RID: 6394 RVA: 0x00185C8C File Offset: 0x00183E8C
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

		// Token: 0x060018FB RID: 6395 RVA: 0x00185E10 File Offset: 0x00184010
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

		// Token: 0x060018FC RID: 6396 RVA: 0x00185EF4 File Offset: 0x001840F4
		public void StartInstance(GameClient client)
		{
			this.ClientExitRoom(client);
		}

		// Token: 0x060018FD RID: 6397 RVA: 0x00185F00 File Offset: 0x00184100
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

		// Token: 0x060018FE RID: 6398 RVA: 0x00185F9C File Offset: 0x0018419C
		public static bool UpdateMarriageData2DB(GameClient client)
		{
			return MarryFuBenMgr.UpdateMarriageData2DB(client.ClientData.RoleID, client.ClientData.MyMarriageData, client);
		}

		// Token: 0x060018FF RID: 6399 RVA: 0x00185FCC File Offset: 0x001841CC
		public static bool UpdateMarriageData2DB(int nRoleID, MarriageData updateMarriageData, GameClient self)
		{
			byte[] dataBytes = DataHelper.ObjectToBytes<MarriageData>(updateMarriageData);
			byte[] byRoleID = BitConverter.GetBytes(nRoleID);
			byte[] sendBytes = new byte[dataBytes.Length + 4];
			Array.Copy(byRoleID, sendBytes, 4);
			Array.Copy(dataBytes, 0, sendBytes, 4, dataBytes.Length);
			return Global.sendToDB<bool, byte[]>(10185, sendBytes, self.ServerId);
		}

		// Token: 0x06001900 RID: 6400 RVA: 0x00186020 File Offset: 0x00184220
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

		// Token: 0x06001901 RID: 6401 RVA: 0x001860B0 File Offset: 0x001842B0
		public MarriageInstance GetMarriageInstanceEX(GameClient client)
		{
			return this.GetMarriageInstance(client);
		}

		// Token: 0x040022FE RID: 8958
		private static MarryFuBenMgr instance = new MarryFuBenMgr();

		// Token: 0x040022FF RID: 8959
		private Dictionary<int, MarriageInstance> MarriageInstanceDic = new Dictionary<int, MarriageInstance>();

		// Token: 0x04002300 RID: 8960
		private SystemXmlItem MarriageFubenXmlItem = null;

		// Token: 0x04002301 RID: 8961
		private SystemXmlItems ManAndWifeBossXmlItems = new SystemXmlItems();
	}
}
