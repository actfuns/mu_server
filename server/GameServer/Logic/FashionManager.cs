using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020004E7 RID: 1255
	public class FashionManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		// Token: 0x0600173B RID: 5947 RVA: 0x0016BABC File Offset: 0x00169CBC
		public static FashionManager getInstance()
		{
			if (FashionManager.instance.State == 0)
			{
				FashionManager.instance.initialize();
			}
			return FashionManager.instance;
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x0016BAF4 File Offset: 0x00169CF4
		public bool initialize()
		{
			bool result;
			if (!this.InitConfig())
			{
				this.State = -1;
				result = false;
			}
			else
			{
				this.State = 1;
				result = true;
			}
			return result;
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x0016BB24 File Offset: 0x00169D24
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(710, 4, 4, FashionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1610, 2, 2, FashionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1611, 2, 2, FashionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1841, 3, 3, FashionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(41, FashionManager.getInstance());
			return true;
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x0016BBAC File Offset: 0x00169DAC
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(41, FashionManager.getInstance());
			return true;
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x0016BBD4 File Offset: 0x00169DD4
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x0016BBE8 File Offset: 0x00169DE8
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x0016BBFC File Offset: 0x00169DFC
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (nID != 710)
			{
				switch (nID)
				{
				case 1610:
					result = this.ProcessFashionBagForgeLevUpCmd(client, nID, bytes, cmdParams);
					break;
				case 1611:
					result = this.ProcessFashionBagActiveCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = (nID != 1841 || this.ProcessModifyBuffFashionCmd(client, nID, bytes, cmdParams));
					break;
				}
			}
			else
			{
				result = this.ProcessModifyFashionCmd(client, nID, bytes, cmdParams);
			}
			return result;
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x0016BC70 File Offset: 0x00169E70
		public void processEvent(EventObject eventObject)
		{
			int nID = eventObject.getEventType();
			int num = nID;
			if (num == 41)
			{
				GameClient client = (eventObject as PlayerLoginGameEventObject).getPlayer();
				this.InitFashion(client);
			}
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x0016BCC0 File Offset: 0x00169EC0
		public bool InitConfig()
		{
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.FashionTabDict.Clear();
					fileName = "Config/FashionTab.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						FashionTabData item = new FashionTabData();
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item.Name = Global.GetSafeAttributeStr(node, "Name");
						item.Categoriy = (int)Global.GetSafeAttributeLong(node, "Categoriy");
						this.RuntimeData.FashionTabDict.Add(item.ID, item);
					}
					Data.ClearMiniBufferDataIds();
					this.RuntimeData.SpecialTitleDict.Clear();
					fileName = "Config/SpecialTitle.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						int id = (int)Global.GetSafeAttributeLong(node, "ID");
						int bufferID = (int)Global.GetSafeAttributeLong(node, "BuffID");
						this.RuntimeData.SpecialTitleDict.Add(id, bufferID);
						Data.AddMiniBufferDataIds(new int[]
						{
							bufferID
						});
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.FashingDict.Clear();
					fileName = "Config/Fashion.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						FashionData item2 = new FashionData();
						item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.TabID = (int)Global.GetSafeAttributeLong(node, "Tab");
						item2.Name = Global.GetSafeAttributeStr(node, "Name");
						item2.GoodsID = (int)Global.GetSafeAttributeLong(node, "Goods");
						item2.Type = (int)Global.GetSafeAttributeLong(node, "Type");
						item2.Time = (int)Global.GetSafeAttributeLong(node, "Time");
						item2.RandomType = Global.SafeConvertToInt32(Global.GetDefAttributeStr(node, "Random", "-1"));
						item2.EndTime = DateTime.MinValue;
						string strEndTm = Global.GetSafeAttributeStr(node, "Term");
						if (!string.IsNullOrEmpty(strEndTm) && 0 != string.Compare(strEndTm, "-1"))
						{
							DateTime.TryParse(strEndTm, out item2.EndTime);
						}
						this.RuntimeData.FashingDict.Add(item2.ID, item2);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
				string[] configFileNames = new string[]
				{
					"Config/ShiZhuangLevelup.xml",
					"Config/FashionWings.xml",
					"Config/HorseFashion.xml",
					"Config/JiaoYinShiZhuangShengJi.xml",
					"Config/WuQiShiZhuangShengJi.xml",
					"Config/TransfigurationFashion.xml"
				};
				int[] fashionTabs = new int[]
				{
					3,
					1,
					4,
					5,
					6,
					7
				};
				try
				{
					this.RuntimeData.FashionBagDict.Clear();
					for (int i = 0; i < configFileNames.Length; i++)
					{
						int fashionTab = fashionTabs[i];
						fileName = configFileNames[i];
						string fullPathFileName = Global.GameResPath(fileName);
						XElement xml = XElement.Load(fullPathFileName);
						IEnumerable<XElement> nodes = xml.Elements();
						foreach (XElement node in nodes)
						{
							FashionBagData item3 = new FashionBagData();
							item3.FasionTab = fashionTab;
							item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
							item3.GoodsID = (int)Global.GetSafeAttributeLong(node, "GoodsID");
							item3.ForgeLev = (int)Global.GetSafeAttributeLong(node, "level");
							item3.LimitTime = (int)Global.GetSafeAttributeLong(node, "Time");
							string TempValueString = Global.GetSafeAttributeStr(node, "NeedGoods");
							string[] ValueFileds = TempValueString.Split(new char[]
							{
								','
							});
							if (ValueFileds.Length == 2)
							{
								item3.NeedGoodsID = Global.SafeConvertToInt32(ValueFileds[0]);
								item3.NeedGoodsCount = Global.SafeConvertToInt32(ValueFileds[1]);
							}
							TempValueString = Global.GetSafeAttributeStr(node, "ProPerty");
							ValueFileds = TempValueString.Split(new char[]
							{
								'|'
							});
							foreach (string value in ValueFileds)
							{
								string[] KvpFileds = value.Split(new char[]
								{
									','
								});
								if (KvpFileds.Length == 2)
								{
									ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
									if (index != ExtPropIndexes.Max)
									{
										item3.ExtProps[(int)index] = Global.SafeConvertToDouble(KvpFileds[1]);
									}
								}
							}
							TempValueString = ConfigHelper.GetElementAttributeValue(node, "AttackSkill", "");
							if (!string.IsNullOrEmpty(TempValueString))
							{
								item3.AttackSkill = Global.StringToIntList(TempValueString, ',');
							}
							TempValueString = ConfigHelper.GetElementAttributeValue(node, "MagicSkill", "");
							if (!string.IsNullOrEmpty(TempValueString))
							{
								item3.MagicSkill = Global.StringToIntList(TempValueString, ',');
							}
							item3.BianShenEffect = (int)ConfigHelper.GetElementAttributeValueLong(node, "Effect", 0L);
							item3.BianShenDuration = (int)ConfigHelper.GetElementAttributeValueLong(node, "Duration", 0L);
							KeyValuePair<int, int> key = new KeyValuePair<int, int>(item3.GoodsID, item3.ForgeLev);
							if (this.RuntimeData.FashionBagDict.ContainsKey(key))
							{
								LogManager.WriteLog(LogTypes.Fatal, string.Format("道具ID重复或者已经配置为其它类型的时装,xml={0}", node), null, true);
							}
							this.RuntimeData.FashionBagDict[key] = item3;
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x0016C470 File Offset: 0x0016A670
		public bool ProcessModifyFashionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				int tabID = Convert.ToInt32(cmdParams[1]);
				int fashionID = Convert.ToInt32(cmdParams[2]);
				FashionModeTypes mode = (FashionModeTypes)Convert.ToInt32(cmdParams[3]);
				int result = this.ModifyFashion(client, tabID, fashionID, mode);
				client.sendCmd(nID, string.Format("{0}", result), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x0016C508 File Offset: 0x0016A708
		public bool ProcessModifyBuffFashionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				int buffID = Convert.ToInt32(cmdParams[1]);
				FashionModeTypes mode = (FashionModeTypes)Convert.ToInt32(cmdParams[2]);
				int result = this.ModifyBuffFashion(client, buffID, mode);
				client.sendCmd(nID, string.Format("{0}", result), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x0016C594 File Offset: 0x0016A794
		public void InitLuoLanChengZhuFashion(GameClient client)
		{
			if (client.ClientSocket.IsKuaFuLogin)
			{
				if (client.ClientData.Faction <= 0 || client.ClientData.BHZhiWu != 1)
				{
					this.DelLuoLanZhiYi(client);
					return;
				}
				Dictionary<int, BangHuiLingDiItemData> lingdiItemDataS = JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer(client.ClientSocket.ServerId);
				BangHuiLingDiItemData lingdiItemData = null;
				int lingDiID = 7;
				if (lingdiItemDataS == null || !lingdiItemDataS.TryGetValue(lingDiID, out lingdiItemData))
				{
					this.DelLuoLanZhiYi(client);
					return;
				}
				if (lingdiItemData == null || client.ClientData.Faction != lingdiItemData.BHID)
				{
					this.DelLuoLanZhiYi(client);
					return;
				}
			}
			else
			{
				int lingDiID = 7;
				BangHuiLingDiItemData lingdiItemData = JunQiManager.GetItemByLingDiID(lingDiID);
				if (lingdiItemData == null || lingdiItemData.BHID <= 0)
				{
					this.DelLuoLanZhiYi(client);
					return;
				}
				if (client.ClientData.Faction != lingdiItemData.BHID || client.ClientData.BHZhiWu != 1)
				{
					this.DelLuoLanZhiYi(client);
					return;
				}
			}
			this.GetFashionByMagic(client, 1, true);
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x0016C6B5 File Offset: 0x0016A8B5
		public void DelLuoLanZhiYi(GameClient gameclient)
		{
			this.DelFashionByMagic(gameclient, 1);
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x0016C6C4 File Offset: 0x0016A8C4
		public void DelFashionByMagic(GameClient client, int nFashionID)
		{
			if (client != null)
			{
				FashionData fashionData = null;
				if (!this.RuntimeData.FashingDict.TryGetValue(nFashionID, out fashionData))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("Fashion配置文件中，配置的时装物品不存在, ID={0}", nFashionID), null, true);
				}
				else
				{
					GoodsData goodsData = FashionManager.GetFashionDataByGoodsID(client, fashionData.GoodsID);
					if (goodsData != null)
					{
						string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
						{
							client.ClientData.RoleID,
							4,
							goodsData.Id,
							goodsData.GoodsID,
							0,
							goodsData.Site,
							goodsData.GCount,
							goodsData.BagIndex,
							""
						});
						Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
					}
				}
			}
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x0016C7D8 File Offset: 0x0016A9D8
		public void GetFashionByMagic(GameClient client, int nFashionID, bool isAddTime = true)
		{
			FashionData fashionData = null;
			if (!this.RuntimeData.FashingDict.TryGetValue(nFashionID, out fashionData))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("Fashion配置文件中，配置的时装物品不存在, ID={0}", nFashionID), null, true);
			}
			else
			{
				int nGoodsID = fashionData.GoodsID;
				SystemXmlItem systemSZGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodsID, out systemSZGoods))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("Fashion配置文件中，配置的时装物品不存在, GoodsID={0}", nGoodsID), null, true);
				}
				else
				{
					DateTime oldTime = DateTime.MinValue;
					GoodsData oldGoods = FashionManager.GetFashionDataByGoodsID(client, nGoodsID);
					int nGCount = systemSZGoods.GetIntValue("GridNum", -1);
					string strStartTime;
					string strEndTime;
					if (fashionData.Time > 0)
					{
						if (oldGoods != null)
						{
							if (DateTime.TryParse(oldGoods.Endtime, out oldTime))
							{
								oldGoods.Endtime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
							}
							Global.DestroyGoods(client, oldGoods);
						}
						strStartTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
						if (oldTime > DateTime.MinValue && isAddTime)
						{
							strEndTime = oldTime.AddSeconds((double)fashionData.Time).ToString("yyyy-MM-dd HH:mm:ss");
						}
						else
						{
							strEndTime = TimeUtil.NowDateTime().AddSeconds((double)fashionData.Time).ToString("yyyy-MM-dd HH:mm:ss");
						}
					}
					else if (fashionData.EndTime != DateTime.MinValue)
					{
						strStartTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
						strEndTime = fashionData.EndTime.ToString("yyyy-MM-dd HH:mm:ss");
					}
					else
					{
						strStartTime = "1900-01-01 12:00:00";
						strEndTime = "1900-01-01 12:00:00";
					}
					if (oldGoods == null || fashionData.Time > 0)
					{
						GoodsData goodsData = new GoodsData
						{
							GoodsID = nGoodsID,
							GCount = 1,
							Binding = 1,
							Starttime = strStartTime,
							Endtime = strEndTime,
							Site = 6000
						};
						goodsData.Id = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, nGoodsID, nGCount, 0, "", 0, 0, 6000, "", true, 1, "使用指定道具后获取", true, strEndTime, 0, 0, 0, 0, 0, 0, 0, true, null, null, strStartTime, 0, true);
						string resList = EventLogManager.NewGoodsDataPropString(goodsData);
						EventLogManager.AddTitleEvent(client, 1, fashionData.Time, resList);
						FashionManager.NotifyFashionList(client);
					}
				}
			}
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x0016CA74 File Offset: 0x0016AC74
		public void GetFashionByMagic(GameClient client, int nFashionID, string endTime)
		{
			FashionData fashionData = null;
			if (!this.RuntimeData.FashingDict.TryGetValue(nFashionID, out fashionData))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("Fashion配置文件中，配置的时装物品不存在, ID={0}", nFashionID), null, true);
			}
			else
			{
				int nGoodsID = fashionData.GoodsID;
				SystemXmlItem systemSZGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodsID, out systemSZGoods))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("Fashion配置文件中，配置的时装物品不存在, GoodsID={0}", nGoodsID), null, true);
				}
				else
				{
					DateTime oldTime = DateTime.MinValue;
					GoodsData oldGoods = FashionManager.GetFashionDataByGoodsID(client, nGoodsID);
					int nGCount = systemSZGoods.GetIntValue("GridNum", -1);
					if (oldGoods == null || oldGoods.Endtime != endTime)
					{
						if (oldGoods != null)
						{
							oldGoods.Endtime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
							Global.DestroyGoods(client, oldGoods);
						}
						string strStartTime = "1900-01-01 12:00:00";
						Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, nGoodsID, nGCount, 0, "", 0, 0, 6000, "", true, 1, "使用指定道具后获取", true, endTime, 0, 0, 0, 0, 0, 0, 0, true, null, null, strStartTime, 0, true);
						FashionManager.NotifyFashionList(client);
					}
				}
			}
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x0016CBBC File Offset: 0x0016ADBC
		public static void NotifyFashionList(GameClient client)
		{
			byte[] bytesData = DataHelper.ObjectToBytes<List<GoodsData>>(client.ClientData.FashionGoodsDataList);
			GameManager.ClientMgr.SendToClient(client, bytesData, 946);
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x0016CBF0 File Offset: 0x0016ADF0
		public bool FashionCanAdd(GameClient client, int nFashionID)
		{
			FashionData fashionData = null;
			bool result;
			if (!this.RuntimeData.FashingDict.TryGetValue(nFashionID, out fashionData))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("Fashion配置文件中，配置的时装物品不存在, ID={0}", nFashionID), null, true);
				result = false;
			}
			else
			{
				if (fashionData.Time <= 0)
				{
					int nGoodsID = fashionData.GoodsID;
					GoodsData oldGoods = FashionManager.GetFashionDataByGoodsID(client, nGoodsID);
					if (oldGoods != null)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x0016CC6C File Offset: 0x0016AE6C
		public int ModifyFashion(GameClient client, int tabID, int fashionID, FashionModeTypes mode)
		{
			int result = 0;
			if (mode <= FashionModeTypes.None || mode >= FashionModeTypes.Max)
			{
				result = -5;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					FashionData fashionData;
					if (!this.RuntimeData.FashingDict.TryGetValue(fashionID, out fashionData))
					{
						result = -20;
					}
					else if (mode == FashionModeTypes.Load)
					{
						result = this.ValidateFashion(client, fashionData.Type, fashionData.GoodsID);
						if (result >= 0)
						{
							result = this.LoadFashion(client, fashionData);
						}
					}
					else if (mode == FashionModeTypes.Unload)
					{
						if (this.RuntimeData.FashionTabDict.ContainsKey(tabID))
						{
							result = this.UnloadFashion(client, fashionData, false);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600174E RID: 5966 RVA: 0x0016CD78 File Offset: 0x0016AF78
		public int ModifyBuffFashion(GameClient client, int buffID, FashionModeTypes mode)
		{
			int result = 0;
			if (mode <= FashionModeTypes.None || mode >= FashionModeTypes.Max)
			{
				result = -5;
			}
			else
			{
				BufferData buffData = null;
				foreach (BufferData item in client.ClientData.BufferDataList)
				{
					if (item.BufferID == buffID)
					{
						buffData = item;
						break;
					}
				}
				if (null == buffData)
				{
					return -20;
				}
				int usingFashionID = Global.GetRoleParamsInt32FromDB(client, "10163");
				if (mode == FashionModeTypes.Load)
				{
					if (usingFashionID != buffData.BufferID)
					{
						this.ModifyBuffFashionTitleID(client, buffData.BufferID, true, true);
					}
					return 0;
				}
				if (mode == FashionModeTypes.Unload)
				{
					if (usingFashionID != buffData.BufferID)
					{
						return 0;
					}
					this.ModifyBuffFashionTitleID(client, 0, true, true);
					return 0;
				}
			}
			return result;
		}

		// Token: 0x0600174F RID: 5967 RVA: 0x0016CE9C File Offset: 0x0016B09C
		public int ValidateFashion(GameClient client, int fashionType, int GoodsID)
		{
			int result;
			if (fashionType == 1)
			{
				if (client.ClientData.Faction <= 0 || client.ClientData.BHZhiWu != 1)
				{
					result = -3001;
				}
				else
				{
					int lingDiID = 7;
					BangHuiLingDiItemData lingdiItemData = null;
					if (client.ClientSocket.IsKuaFuLogin)
					{
						Dictionary<int, BangHuiLingDiItemData> itemDatas = JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer(client.ServerId);
						if (itemDatas != null)
						{
							itemDatas.TryGetValue(lingDiID, out lingdiItemData);
						}
					}
					else
					{
						lingdiItemData = JunQiManager.GetItemByLingDiID(lingDiID);
					}
					if (lingdiItemData == null || lingdiItemData.BHID <= 0)
					{
						result = -3001;
					}
					else if (client.ClientData.Faction != lingdiItemData.BHID || client.ClientData.BHZhiWu != 1)
					{
						result = -3001;
					}
					else
					{
						result = 0;
					}
				}
			}
			else if (fashionType == 2)
			{
				GoodsData goodsData = FashionManager.GetFashionDataByGoodsID(client, GoodsID);
				if (goodsData != null)
				{
					result = 0;
				}
				else
				{
					result = -12;
				}
			}
			else if (fashionType == 3)
			{
				if (client.ClientData.MyMarriageData.byMarrytype > 0)
				{
					result = 0;
				}
				else
				{
					result = -3002;
				}
			}
			else if (fashionType == 4)
			{
				if (client.ClientData.Faction <= 0 || client.ClientData.BHZhiWu != 1)
				{
					result = -3003;
				}
				else if ((long)client.ClientData.Faction != BangHuiMatchManager.getInstance().RuntimeData.ChengHaoBHid_Gold)
				{
					result = -3003;
				}
				else
				{
					result = 0;
				}
			}
			else if (fashionType == 5)
			{
				if (client.ClientData.Faction <= 0)
				{
					result = -3003;
				}
				else if ((long)client.ClientData.Faction != KuaFuLueDuoManager.getInstance().RuntimeData.ChengHaoBHid)
				{
					result = -3003;
				}
				else
				{
					result = 0;
				}
			}
			else if (fashionType == 6)
			{
				if (client.ClientData.Faction <= 0)
				{
					result = -3003;
				}
				else if ((long)client.ClientData.Faction != KuaFuLueDuoManager.getInstance().RuntimeData.ChengHaoBHid)
				{
					result = -3003;
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				result = -3;
			}
			return result;
		}

		// Token: 0x06001750 RID: 5968 RVA: 0x0016D148 File Offset: 0x0016B348
		public void InitFashion(GameClient client)
		{
			this.InitLuoLanChengZhuFashion(client);
			int usingFashionID = this.GetFashionWingsID(client);
			if (usingFashionID > 0)
			{
				FashionData fashionData = null;
				if (this.RuntimeData.FashingDict.TryGetValue(usingFashionID, out fashionData))
				{
					if (this.ValidateFashion(client, fashionData.Type, fashionData.GoodsID) >= 0)
					{
						this.LoadFashion(client, fashionData);
					}
					else
					{
						this.UnloadFashion(client, fashionData, false);
					}
				}
			}
			usingFashionID = this.GetFashionTitleID(client);
			if (usingFashionID > 0)
			{
				FashionData fashionData = null;
				if (this.RuntimeData.FashingDict.TryGetValue(usingFashionID, out fashionData))
				{
					if (this.ValidateFashion(client, fashionData.Type, fashionData.GoodsID) >= 0)
					{
						this.LoadFashion(client, fashionData);
					}
					else
					{
						this.UnloadFashion(client, fashionData, false);
					}
				}
			}
			this.InitFashionBag(client);
			this.RefreshTitleFashionProps(client);
		}

		// Token: 0x06001751 RID: 5969 RVA: 0x0016D23C File Offset: 0x0016B43C
		private void RefreshTitleFashionProps(GameClient client)
		{
			bool propsChanged = false;
			if (null != client.ClientData.FashionGoodsDataList)
			{
				List<GoodsData> fashionGoodsDataList;
				lock (client.ClientData.FashionGoodsDataList)
				{
					fashionGoodsDataList = new List<GoodsData>(client.ClientData.FashionGoodsDataList);
				}
				lock (this.RuntimeData.Mutex)
				{
					foreach (GoodsData goodsData in fashionGoodsDataList)
					{
						foreach (FashionData fashionData in this.RuntimeData.FashingDict.Values)
						{
							if (fashionData.GoodsID == goodsData.GoodsID && fashionData.TabID == 2)
							{
								EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(fashionData.GoodsID);
								if (null != item)
								{
									client.ClientData.PropsCacheManager.SetExtProps(new object[]
									{
										PropsSystemTypes.FashionByGoodsProps,
										fashionData.TabID,
										fashionData.ID,
										item.ExtProps
									});
									propsChanged = true;
								}
							}
						}
						foreach (FashionBagData fashionBagData in this.RuntimeData.FashionBagDict.Values)
						{
							int nCategories = Global.GetGoodsCatetoriy(goodsData.GoodsID);
							if (fashionBagData.GoodsID == goodsData.GoodsID && goodsData.Forge_level == fashionBagData.ForgeLev && (GoodsUtil.GetGoodsTypeInfo(nCategories).FashionGoods || nCategories == 8))
							{
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.FashionByGoodsProps,
									3,
									fashionBagData.GoodsID,
									fashionBagData.ExtProps
								});
								propsChanged = true;
							}
						}
					}
				}
			}
			if (propsChanged)
			{
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
			}
		}

		// Token: 0x06001752 RID: 5970 RVA: 0x0016D5A4 File Offset: 0x0016B7A4
		public void InitFashionBag(GameClient client)
		{
			GoodsData goodsData = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, 24);
			if (goodsData != null && goodsData.Site != 6000)
			{
				if (!Global.CanAddGoods(client, goodsData.GoodsID, 1, goodsData.Binding, "1900-01-01 12:00:00", true, false))
				{
					if (Global.UseMailGivePlayerAward(client, goodsData, GLang.GetLang(377, new object[0]), GLang.GetLang(378, new object[0]), 1.0))
					{
						Global.DestroyGoods(client, goodsData);
					}
				}
				else
				{
					string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
					{
						client.ClientData.RoleID,
						2,
						goodsData.Id,
						goodsData.GoodsID,
						0,
						goodsData.Site,
						goodsData.GCount,
						goodsData.BagIndex,
						""
					});
					Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
				}
			}
		}

		// Token: 0x06001753 RID: 5971 RVA: 0x0016D6DC File Offset: 0x0016B8DC
		public bool FashionBagCanActive(GameClient client, GoodsData goodsData)
		{
			int nCategories = Global.GetGoodsCatetoriy(goodsData.GoodsID);
			bool result;
			if (!GoodsUtil.GetGoodsTypeInfo(nCategories).FashionGoods && nCategories != 8)
			{
				result = false;
			}
			else if (client.ClientData.FashionGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				List<GoodsData> fashionGoodsDataList;
				lock (client.ClientData.FashionGoodsDataList)
				{
					fashionGoodsDataList = new List<GoodsData>(client.ClientData.FashionGoodsDataList);
				}
				foreach (GoodsData item in fashionGoodsDataList)
				{
					if (item.GoodsID == goodsData.GoodsID && !Global.IsTimeLimitGoods(item))
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x0016D7F4 File Offset: 0x0016B9F4
		private bool ProcessFashionBagActiveCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				int GoodsDbID = Convert.ToInt32(cmdParams[1]);
				GoodsData goodsData = Global.GetGoodsByDbID(client, GoodsDbID);
				if (null == goodsData)
				{
					result = -1;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, GoodsDbID), false);
					return true;
				}
				if (!this.FashionBagCanActive(client, goodsData))
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, GoodsDbID), false);
					return true;
				}
				FashionBagData FashionDataCurrentLev = null;
				lock (this.RuntimeData.Mutex)
				{
					KeyValuePair<int, int> key = new KeyValuePair<int, int>(goodsData.GoodsID, goodsData.Forge_level);
					if (!this.RuntimeData.FashionBagDict.TryGetValue(key, out FashionDataCurrentLev))
					{
						result = -23;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, GoodsDbID), false);
						return true;
					}
				}
				if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData, 1, false, true))
				{
					result = -6;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, GoodsDbID), false);
					return true;
				}
				DateTime oldTime = DateTime.MinValue;
				GoodsData oldGoods = FashionManager.GetFashionDataByGoodsID(client, goodsData.GoodsID);
				int oldGoodsUsing = 0;
				int oldGoodsForgeLev = 0;
				if (null != oldGoods)
				{
					oldGoodsUsing = oldGoods.Using;
					oldGoodsForgeLev = oldGoods.Forge_level;
				}
				string strStartTime;
				string strEndTime;
				if (FashionDataCurrentLev.LimitTime > 0)
				{
					if (oldGoods != null)
					{
						if (DateTime.TryParse(oldGoods.Endtime, out oldTime))
						{
							oldGoods.Endtime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
						}
						Global.DestroyGoods(client, oldGoods);
					}
					strStartTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
					if (oldTime > DateTime.MinValue)
					{
						strEndTime = oldTime.AddSeconds((double)FashionDataCurrentLev.LimitTime).ToString("yyyy-MM-dd HH:mm:ss");
					}
					else
					{
						strEndTime = TimeUtil.NowDateTime().AddSeconds((double)FashionDataCurrentLev.LimitTime).ToString("yyyy-MM-dd HH:mm:ss");
					}
				}
				else
				{
					strStartTime = "1900-01-01 12:00:00";
					strEndTime = "1900-01-01 12:00:00";
				}
				int NewGoodsDBID;
				if (oldGoods != null)
				{
					NewGoodsDBID = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, 1, 0, "", oldGoodsForgeLev, goodsData.Binding, 6000, "", true, 1, "时装激活", true, strEndTime, 0, 0, 0, 0, 0, 0, 0, true, null, null, strStartTime, 0, true);
					if (oldGoodsUsing > 0)
					{
						string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
						{
							client.ClientData.RoleID,
							1,
							NewGoodsDBID,
							goodsData.GoodsID,
							1,
							6000,
							1,
							0,
							""
						});
						Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
					}
				}
				else
				{
					NewGoodsDBID = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, 1, 0, "", 0, goodsData.Binding, 6000, "", true, 1, "时装激活", true, strEndTime, 0, 0, 0, 0, 0, 0, 0, true, null, null, strStartTime, 0, true);
				}
				FashionManager.NotifyFashionList(client);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, roleID, NewGoodsDBID), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001755 RID: 5973 RVA: 0x0016DC94 File Offset: 0x0016BE94
		private bool ProcessFashionBagForgeLevUpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				int GoodsDbID = Convert.ToInt32(cmdParams[1]);
				GoodsData goodsData = FashionManager.GetFashionGoodsDataByDbID(client, GoodsDbID);
				if (null == goodsData)
				{
					result = -1;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						roleID,
						GoodsDbID,
						0
					}), false);
					return true;
				}
				if (Global.IsTimeLimitGoods(goodsData))
				{
					result = -5;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						roleID,
						GoodsDbID,
						0
					}), false);
					return true;
				}
				FashionBagData FashionDataCurrentLev = null;
				FashionBagData FashionDataNextLev = null;
				lock (this.RuntimeData.Mutex)
				{
					KeyValuePair<int, int> key = new KeyValuePair<int, int>(goodsData.GoodsID, goodsData.Forge_level);
					if (!this.RuntimeData.FashionBagDict.TryGetValue(key, out FashionDataCurrentLev))
					{
						result = -23;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							GoodsDbID,
							0
						}), false);
						return true;
					}
					key = new KeyValuePair<int, int>(goodsData.GoodsID, goodsData.Forge_level + 1);
					if (!this.RuntimeData.FashionBagDict.TryGetValue(key, out FashionDataNextLev))
					{
						result = -23;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							GoodsDbID,
							0
						}), false);
						return true;
					}
				}
				int nBindGoodNum = Global.GetTotalBindGoodsCountByID(client, FashionDataNextLev.NeedGoodsID);
				int nNotBindGoodNum = Global.GetTotalNotBindGoodsCountByID(client, FashionDataNextLev.NeedGoodsID);
				if (FashionDataNextLev.NeedGoodsCount > nBindGoodNum + nNotBindGoodNum)
				{
					result = -6;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						roleID,
						GoodsDbID,
						0
					}), false);
					return true;
				}
				int nSubNum = FashionDataNextLev.NeedGoodsCount;
				int nSum;
				if (FashionDataNextLev.NeedGoodsCount > nBindGoodNum)
				{
					nSum = nBindGoodNum;
					nSubNum = FashionDataNextLev.NeedGoodsCount - nBindGoodNum;
				}
				else
				{
					nSum = FashionDataNextLev.NeedGoodsCount;
					nSubNum = 0;
				}
				bool usedBinding = false;
				bool usedTimeLimited = false;
				if (nSum > 0)
				{
					if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, FashionDataNextLev.NeedGoodsID, nSum, false, out usedBinding, out usedTimeLimited, false))
					{
						result = -6;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							GoodsDbID,
							0
						}), false);
						return true;
					}
				}
				if (nSubNum > 0)
				{
					if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, FashionDataNextLev.NeedGoodsID, nSubNum, false, out usedBinding, out usedTimeLimited, false))
					{
						result = -6;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							GoodsDbID,
							0
						}), false);
						return true;
					}
				}
				goodsData.Forge_level++;
				string[] dbFields = null;
				string strDbCmd = Global.FormatUpdateDBGoodsStr(new object[]
				{
					client.ClientData.RoleID,
					goodsData.Id,
					"*",
					goodsData.Forge_level,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					goodsData.Binding,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*"
				});
				TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strDbCmd, out dbFields, client.ServerId);
				if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
				{
					result = -15;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						roleID,
						GoodsDbID,
						0
					}), false);
					return true;
				}
				if (dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
				{
					result = -15;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						roleID,
						GoodsDbID,
						0
					}), false);
					return true;
				}
				this.RefreshTitleFashionProps(client);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					result,
					roleID,
					GoodsDbID,
					goodsData.Forge_level
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001756 RID: 5974 RVA: 0x0016E38C File Offset: 0x0016C58C
		private int LoadFashion(GameClient client, FashionData fashionData)
		{
			EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(fashionData.GoodsID);
			int result;
			if (null == item)
			{
				result = -3;
			}
			else if (fashionData.TabID == 1)
			{
				int usingFashionID = this.GetFashionWingsID(client);
				if (usingFashionID != fashionData.ID)
				{
					this.ModifyFashionWingsID(client, fashionData.ID, false, true);
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.FashionByGoodsProps,
					fashionData.TabID,
					0,
					item.ExtProps
				});
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				result = 0;
			}
			else if (fashionData.TabID == 2)
			{
				int usingFashionID = this.GetFashionTitleID(client);
				if (usingFashionID != fashionData.ID)
				{
					this.ModifyFashionTitleID(client, fashionData.ID, false, true);
				}
				result = 0;
			}
			else
			{
				result = -3;
			}
			return result;
		}

		// Token: 0x06001757 RID: 5975 RVA: 0x0016E4AC File Offset: 0x0016C6AC
		private int UnloadFashion(GameClient client, FashionData fashionData, bool bIsRemove)
		{
			int usingFashionID = 0;
			if (fashionData.TabID == 2)
			{
				usingFashionID = this.GetFashionTitleID(client);
			}
			else if (fashionData.TabID == 1)
			{
				usingFashionID = this.GetFashionWingsID(client);
			}
			int result;
			if (usingFashionID != fashionData.ID)
			{
				result = 0;
			}
			else
			{
				int nULID = 0;
				if (bIsRemove)
				{
					nULID = -1;
				}
				if (fashionData.TabID == 1)
				{
					this.ModifyFashionWingsID(client, nULID, false, true);
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.FashionByGoodsProps,
						fashionData.TabID,
						0,
						PropsCacheManager.ConstExtProps
					});
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				}
				else if (fashionData.TabID == 2)
				{
					this.ModifyFashionTitleID(client, nULID, false, true);
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x0016E5BC File Offset: 0x0016C7BC
		public bool FashionActiveByMagic(GameClient client, double[] cmdParams)
		{
			try
			{
				int randomType = Convert.ToInt32(cmdParams[0]);
				int addType = 38;
				int addPoint = 0;
				if (cmdParams.Length < 2 || cmdParams.Length > 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("称号道具使用参数异常！！！", new object[0]), null, true);
					return false;
				}
				if (cmdParams.Length > 2)
				{
					addType = Convert.ToInt32(cmdParams[1]);
					addPoint = Convert.ToInt32(cmdParams[2]);
				}
				else
				{
					addPoint = Convert.ToInt32(cmdParams[1]);
				}
				List<FashionData> fashionList = new List<FashionData>();
				Dictionary<int, FashionData> fashinDict = new Dictionary<int, FashionData>();
				lock (this.RuntimeData.Mutex)
				{
					fashinDict = new Dictionary<int, FashionData>(this.RuntimeData.FashingDict);
				}
				foreach (FashionData fashionData in fashinDict.Values)
				{
					if (fashionData.RandomType == randomType && FashionManager.GetFashionDataByGoodsID(client, fashionData.GoodsID) == null)
					{
						fashionList.Add(fashionData);
					}
				}
				if (fashionList.Count > 0)
				{
					int random = Global.GetRandomNumber(0, fashionList.Count);
					FashionData fashionAdd = fashionList[random];
					this.GetFashionByMagic(client, fashionList[random].ID, true);
				}
				else
				{
					int num = addType;
					if (num <= 31)
					{
						switch (num)
						{
						case 23:
							GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, addPoint, "道具脚本", true, false);
							break;
						case 24:
							break;
						case 25:
							GameManager.ClientMgr.ModifyMUMoHeValue(client, addPoint, "道具脚本", false, true, false);
							break;
						default:
							if (num == 31)
							{
								GameManager.FluorescentGemMgr.AddFluorescentPoint(client, addPoint, "道具脚本", true);
							}
							break;
						}
					}
					else if (num != 34)
					{
						if (num == 38)
						{
							GameManager.ClientMgr.ModifyOrnamentCharmPointValue(client, addPoint, "道具脚本", true, true, false);
						}
					}
					else
					{
						GameManager.ClientMgr.ModifyLangHunFenMoValue(client, addPoint, "道具脚本", true, true);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001759 RID: 5977 RVA: 0x0016E86C File Offset: 0x0016CA6C
		public int GetFashionWingsID(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "FashionWingsID", "2020-12-12 12:12:12");
		}

		// Token: 0x0600175A RID: 5978 RVA: 0x0016E890 File Offset: 0x0016CA90
		public int GetFashionTitleID(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "FashionTitleID", "2020-12-12 12:12:12");
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x0016E8B4 File Offset: 0x0016CAB4
		public void ModifyFashionWingsID(GameClient client, int nID, bool writeToDB = false, bool notifyClient = true)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "FashionWingsID", nID, true, "2020-12-12 12:12:12");
			if (notifyClient)
			{
				GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.FashionWingsID, nID);
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 26, nID);
				client.sendOthersCmd(427, strcmd);
			}
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x0016E924 File Offset: 0x0016CB24
		public void ModifyFashionTitleID(GameClient client, int nID, bool writeToDB = false, bool notifyClient = true)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "FashionTitleID", nID, true, "2020-12-12 12:12:12");
			if (notifyClient)
			{
				GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.FashionTitleID, nID);
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 30, nID);
				client.sendOthersCmd(427, strcmd);
			}
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x0016E994 File Offset: 0x0016CB94
		public void ModifyBuffFashionTitleID(GameClient client, int nID, bool writeToDB = true, bool notifyClient = true)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "10163", nID, writeToDB);
			if (notifyClient)
			{
				GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.BuffFashionID, nID);
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 40, nID);
				client.sendOthersCmd(427, strcmd);
			}
		}

		// Token: 0x0600175E RID: 5982 RVA: 0x0016EA00 File Offset: 0x0016CC00
		public int GetBufferIDBySpecialTitleID(int titleID)
		{
			lock (this.RuntimeData.Mutex)
			{
				int bufferID;
				if (this.RuntimeData.SpecialTitleDict.TryGetValue(titleID, out bufferID))
				{
					return bufferID;
				}
			}
			return 0;
		}

		// Token: 0x0600175F RID: 5983 RVA: 0x0016EA74 File Offset: 0x0016CC74
		public void UpdateLuoLanChengZhuFasion(int bhid)
		{
			int roleID = -1;
			int fashionID = 0;
			BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(roleID, bhid, 0);
			lock (this.RuntimeData.Mutex)
			{
				foreach (FashionData item in this.RuntimeData.FashingDict.Values)
				{
					if (item.Type == 1)
					{
						fashionID = item.ID;
						break;
					}
				}
			}
			if (bangHuiDetailData != null && fashionID > 0)
			{
				int oldBHId = 0;
				int newBHId = 0;
				GameClient oldClient = GameManager.ClientMgr.FindClient(this.RuntimeData.LuoLanChengZhuRoleID);
				if (oldClient != null && bangHuiDetailData.BZRoleID != oldClient.ClientData.RoleID)
				{
					this.DelLuoLanZhiYi(oldClient);
					oldBHId = oldClient.ClientData.Faction;
				}
				GameClient newClient = GameManager.ClientMgr.FindClient(bangHuiDetailData.BZRoleID);
				if (null != newClient)
				{
					this.GetFashionByMagic(newClient, 1, false);
					newBHId = newClient.ClientData.Faction;
				}
				this.RuntimeData.LuoLanChengZhuRoleID = bangHuiDetailData.BZRoleID;
				if (oldBHId != newBHId)
				{
					int index = 0;
					GameClient gc;
					while ((gc = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
					{
						if (gc.ClientData.Faction == newBHId && newBHId != 0)
						{
							if (gc.ClientData.RoleID == this.RuntimeData.LuoLanChengZhuRoleID)
							{
								Global.UpdateBufferData(gc, BufferItemTypes.LuoLanChengZhu_Title, new double[]
								{
									1.0
								}, 1, false);
							}
							else
							{
								Global.UpdateBufferData(gc, BufferItemTypes.LuoLanGuiZu_Title, new double[]
								{
									1.0
								}, 1, false);
							}
						}
						else
						{
							GameClient client = gc;
							BufferItemTypes bufferItemType = BufferItemTypes.LuoLanChengZhu_Title;
							double[] actionParams = new double[1];
							Global.UpdateBufferData(client, bufferItemType, actionParams, 1, false);
							GameClient client2 = gc;
							BufferItemTypes bufferItemType2 = BufferItemTypes.LuoLanGuiZu_Title;
							actionParams = new double[1];
							Global.UpdateBufferData(client2, bufferItemType2, actionParams, 1, false);
						}
					}
				}
			}
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x0016ECFC File Offset: 0x0016CEFC
		public FashionBagData GetFashionBagData(GameClient client, GoodsData goodsData)
		{
			KeyValuePair<int, int> key = new KeyValuePair<int, int>(goodsData.GoodsID, goodsData.Forge_level);
			lock (this.RuntimeData.Mutex)
			{
				FashionBagData data;
				if (this.RuntimeData.FashionBagDict.TryGetValue(key, out data))
				{
					return data;
				}
			}
			return null;
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x0016ED88 File Offset: 0x0016CF88
		public static GoodsData GetFashionGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.FashionGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.FashionGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.FashionGoodsDataList.Count; i++)
					{
						if (client.ClientData.FashionGoodsDataList[i].Id == id)
						{
							return client.ClientData.FashionGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x0016EE50 File Offset: 0x0016D050
		public static GoodsData GetFashionDataByGoodsID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.FashionGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.FashionGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.FashionGoodsDataList.Count; i++)
					{
						if (client.ClientData.FashionGoodsDataList[i].GoodsID == id)
						{
							return client.ClientData.FashionGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06001763 RID: 5987 RVA: 0x0016EF18 File Offset: 0x0016D118
		public void AddFashionGoodsData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 6000)
			{
				if (null == client.ClientData.FashionGoodsDataList)
				{
					client.ClientData.FashionGoodsDataList = new List<GoodsData>();
				}
				lock (client.ClientData.FashionGoodsDataList)
				{
					client.ClientData.FashionGoodsDataList.Add(goodsData);
				}
				this.RefreshTitleFashionProps(client);
			}
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x0016EFB8 File Offset: 0x0016D1B8
		public GoodsData AddFashionGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
		{
			GoodsData gd = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = "1900-01-01 12:00:00",
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = binding,
				Jewellist = jewelList,
				BagIndex = 0,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife
			};
			this.AddFashionGoodsData(client, gd);
			return gd;
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x0016F080 File Offset: 0x0016D280
		public void RemoveFashionGoodsData(GameClient client, GoodsData goodsData)
		{
			if (null != client.ClientData.FashionGoodsDataList)
			{
				if (null != goodsData)
				{
					FashionData fashionData = null;
					foreach (FashionData item in FashionManager.getInstance().RuntimeData.FashingDict.Values)
					{
						if (item.GoodsID == goodsData.GoodsID)
						{
							fashionData = item;
							break;
						}
					}
					if (fashionData != null)
					{
						this.UnloadFashion(client, fashionData, true);
					}
					lock (client.ClientData.FashionGoodsDataList)
					{
						if (null != fashionData)
						{
							EquipPropItem item2 = GameManager.EquipPropsMgr.FindEquipPropItem(fashionData.GoodsID);
							if (null != item2)
							{
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.FashionByGoodsProps,
									fashionData.TabID,
									fashionData.ID,
									PropsCacheManager.ConstExtProps
								});
							}
						}
						int nCategories = Global.GetGoodsCatetoriy(goodsData.GoodsID);
						if (GoodsUtil.GetGoodsTypeInfo(nCategories).FashionGoods || nCategories == 8)
						{
							client.ClientData.PropsCacheManager.SetExtProps(new object[]
							{
								PropsSystemTypes.FashionByGoodsProps,
								3,
								goodsData.GoodsID,
								PropsCacheManager.ConstExtProps
							});
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						}
						client.ClientData.FashionGoodsDataList.Remove(goodsData);
						string resList = EventLogManager.NewGoodsDataPropString(goodsData);
						EventLogManager.AddTitleEvent(client, 0, 0, resList);
					}
					this.RefreshTitleFashionProps(client);
				}
			}
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x0016F2D0 File Offset: 0x0016D4D0
		public static int GetFashionGoodsDataCount(GameClient client)
		{
			int result;
			if (null == client.ClientData.FashionGoodsDataList)
			{
				result = 0;
			}
			else
			{
				result = client.ClientData.FashionGoodsDataList.Count;
			}
			return result;
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x0016F30C File Offset: 0x0016D50C
		public static TCPProcessCmdResults ProcessGetFashionList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (1 != fields.Length)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				byte[] bytesData = null;
				List<GoodsData> fashionGoodsDataList = client.ClientData.FashionGoodsDataList;
				if (null != fashionGoodsDataList)
				{
					lock (fashionGoodsDataList)
					{
						if (fashionGoodsDataList.Count > 100)
						{
							CompressdGoodsDataList list = new CompressdGoodsDataList(client.ClientData.FashionGoodsDataList);
							bytesData = DataHelper.ObjectToBytes<CompressdGoodsDataList>(list);
						}
					}
				}
				if (null == bytesData)
				{
					bytesData = DataHelper.ObjectToBytes<List<GoodsData>>(fashionGoodsDataList);
				}
				GameManager.ClientMgr.SendToClient(client, bytesData, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetElementHrtList", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x0016F524 File Offset: 0x0016D724
		public static TCPProcessCmdResults ProcessGetBuffFashionList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (1 != fields.Length)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				byte[] bytesData = DataHelper.ObjectToBytes<List<BufferData>>(client.ClientData.BufferDataList);
				GameManager.ClientMgr.SendToClient(client, bytesData, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetElementHrtList", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x04002141 RID: 8513
		private int State = 0;

		// Token: 0x04002142 RID: 8514
		private static FashionManager instance = new FashionManager();

		// Token: 0x04002143 RID: 8515
		public FashionNamagerData RuntimeData = new FashionNamagerData();
	}
}
