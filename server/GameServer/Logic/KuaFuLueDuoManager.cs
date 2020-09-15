using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Ornament;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000222 RID: 546
	public class KuaFuLueDuoManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		// Token: 0x0600073C RID: 1852 RVA: 0x0006B024 File Offset: 0x00069224
		public static KuaFuLueDuoManager getInstance()
		{
			return KuaFuLueDuoManager.instance;
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x0006B03C File Offset: 0x0006923C
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x0006B060 File Offset: 0x00069260
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KuaFuLueDuoManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x0006B0A0 File Offset: 0x000692A0
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1245, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1246, 2, 2, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1247, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1248, 2, 2, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1250, 3, 3, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1251, 2, 2, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1252, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1254, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1255, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1257, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1258, 3, 3, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1259, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(23, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(24, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(25, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(26, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10003, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10002, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(33, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(27, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(14, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, KuaFuLueDuoManager.getInstance());
			return true;
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x0006B2E4 File Offset: 0x000694E4
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(23, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(24, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(25, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(26, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10003, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10002, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(33, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(27, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(14, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(28, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, KuaFuLueDuoManager.getInstance());
			return true;
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x0006B408 File Offset: 0x00069608
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x0006B41C File Offset: 0x0006961C
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x0006B430 File Offset: 0x00069630
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			if (nID != 1252)
			{
				if (!this.IsGongNengOpenedEnter(client, false))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return true;
				}
			}
			switch (nID)
			{
			case 1245:
				return this.ProcessGetKuaFuLueDuoMainInfoCmd(client, nID, bytes, cmdParams);
			case 1246:
				return this.ProcessGetKuaFuLueDuoRankInfoCmd(client, nID, bytes, cmdParams);
			case 1247:
				return this.ProcessGetKuaFuLueDuoAnalysisDataCmd(client, nID, bytes, cmdParams);
			case 1248:
				return this.ProcessKuaFuLueDuoBuyEnterNumCmd(client, nID, bytes, cmdParams);
			case 1250:
				return this.ProcessKuaFuLueDuoJoinCmd(client, nID, bytes, cmdParams);
			case 1251:
				return this.ProcessKuaFuLueDuoEnterCmd(client, nID, bytes, cmdParams);
			case 1252:
				return this.ProcessGetKuaFuLueDuoStateCmd(client, nID, bytes, cmdParams);
			case 1254:
				return this.ProcessGetKuaFuLueDuoAwardInfoCmd(client, nID, bytes, cmdParams);
			case 1255:
				return this.ProcessGetKuaFuLueDuoAwardCmd(client, nID, bytes, cmdParams);
			case 1257:
				return this.ProcessGetKuaFuLueDuoMallDataCmd(client, nID, bytes, cmdParams);
			case 1258:
				return this.ProcessKuaFuLueDuoMallBuyCmd(client, nID, bytes, cmdParams);
			case 1259:
				return this.ProcessKuaFuLueDuoMallRefreshCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x0006B5A0 File Offset: 0x000697A0
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 28)
			{
				OnStartPlayGameEventObject e = eventObject as OnStartPlayGameEventObject;
				this.OnStartPlayGame(e.Client);
			}
			else if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEvent = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEvent)
				{
					if (playerDeadEvent.Type == PlayerDeadEventTypes.ByRole)
					{
						this.OnKillRole(playerDeadEvent.getAttackerRole(), playerDeadEvent.getPlayer());
					}
				}
			}
			else if (eventType == 14)
			{
				PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
				if (null != playerInitGameEventObject)
				{
					this.OnInitGame(playerInitGameEventObject.getPlayer());
				}
			}
			else if (eventType == 11)
			{
				MonsterDeadEventObject e2 = eventObject as MonsterDeadEventObject;
				this.OnProcessJunQiDead(e2.getAttacker(), e2.getMonster());
			}
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x0006B68C File Offset: 0x0006988C
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			switch (num)
			{
			case 23:
			{
				PreBangHuiAddMemberEventObject e = eventObject as PreBangHuiAddMemberEventObject;
				if (null != e)
				{
					eventObject.Handled = this.OnPreBangHuiAddMember(e);
				}
				break;
			}
			case 24:
			{
				PreBangHuiRemoveMemberEventObject e2 = eventObject as PreBangHuiRemoveMemberEventObject;
				if (null != e2)
				{
					eventObject.Handled = this.OnPreBangHuiRemoveMember(e2);
				}
				break;
			}
			case 25:
				break;
			case 26:
			{
				PostBangHuiChangeEventObject e3 = eventObject as PostBangHuiChangeEventObject;
				if (e3 != null && null != e3.Player)
				{
					this.UpdateChengHaoBuffer(e3.Player, true);
				}
				break;
			}
			case 27:
			{
				ProcessClickOnNpcEventObject e4 = eventObject as ProcessClickOnNpcEventObject;
				if (null != e4)
				{
					if (null != e4.Npc)
					{
						int npcId = e4.Npc.NpcID;
					}
					if (this.OnSpriteClickOnNpc(e4.Client, e4.NpcId, e4.ExtensionID))
					{
						e4.Result = false;
						e4.Handled = true;
					}
				}
				break;
			}
			case 28:
			case 29:
			case 31:
			case 32:
				break;
			case 30:
			{
				OnCreateMonsterEventObject e5 = eventObject as OnCreateMonsterEventObject;
				if (null != e5)
				{
					QiZhiConfig qiZhiConfig = e5.Monster.Tag as QiZhiConfig;
					if (null != qiZhiConfig)
					{
						e5.Monster.Camp = qiZhiConfig.BattleWhichSide;
						e5.Result = true;
						e5.Handled = true;
					}
				}
				break;
			}
			case 33:
			{
				PreMonsterInjureEventObject obj = eventObject as PreMonsterInjureEventObject;
				if (obj != null && obj.SceneType == 47)
				{
					Monster injureMonster = obj.Monster;
					if (injureMonster != null)
					{
						QiZhiConfig item = injureMonster.Tag as QiZhiConfig;
						if (item != null)
						{
							obj.Injure = item.Injure;
							eventObject.Handled = true;
							eventObject.Result = true;
						}
					}
				}
				break;
			}
			default:
				switch (num)
				{
				case 10002:
				{
					CaiJiEventObject e6 = eventObject as CaiJiEventObject;
					if (null != e6)
					{
						GameClient client = e6.Source as GameClient;
						Monster monster = e6.Target as Monster;
						this.OnCaiJiFinish(client, monster);
						eventObject.Handled = true;
						eventObject.Result = true;
					}
					break;
				}
				case 10003:
				{
					GetCaiJiTimeEventObject e7 = eventObject as GetCaiJiTimeEventObject;
					if (null != e7)
					{
						GameClient client = e7.Source as GameClient;
						Monster monster = e7.Target as Monster;
						if (client != null && null != monster)
						{
							e7.GatherTime = this.GetCaiJiMonsterTime(client, monster);
							eventObject.Handled = true;
							eventObject.Result = true;
						}
					}
					break;
				}
				}
				break;
			}
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x0006B97C File Offset: 0x00069B7C
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					if (!this.RuntimeData.CommonConfigData.Load(Global.GameResPath("Config\\CrusadeWar.xml"), Global.GameResPath("Config\\CrusadeGroup.xml"), GameManager.PlatformType))
					{
						LogManager.WriteLog(LogTypes.Error, "跨服掠夺 活动和分组配置 InitConfig failed!", null, true);
					}
					this.LoadCollectXml();
					this.RuntimeData.MapBirthPointDict.Clear();
					fileName = "Config\\CrusadeBirthPoint.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						MapBirthPoint item = new MapBirthPoint();
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item.BirthPosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item.BirthPosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						item.BirthRangeX = (item.BirthRangeY = (int)Global.GetSafeAttributeLong(node, "BirthRadius"));
						this.RuntimeData.MapBirthPointDict[item.ID] = item;
					}
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					fileName = "Config\\CrusadeQiZhi.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						QiZhiConfig item2 = new QiZhiConfig();
						item2.NPCID = (int)Global.GetSafeAttributeLong(node, "QiZuoID");
						item2.MonsterId = (int)Global.GetSafeAttributeLong(node, "JunQiID");
						item2.PosX = ConfigHelper.String2IntArray(Global.GetSafeAttributeStr(node, "QiZuoSite"), '|')[0];
						item2.PosY = ConfigHelper.String2IntArray(Global.GetSafeAttributeStr(node, "QiZuoSite"), '|')[1];
						item2.Injure = (int)Global.GetSafeAttributeLong(node, "Hurt");
						item2.RebirthRadius = (int)Global.GetSafeAttributeLong(node, "RebirthRadius");
						this.RuntimeData.NPCID2QiZhiConfigDict[item2.NPCID] = item2;
					}
					this.RuntimeData.KingOfBattleStoreDict.Clear();
					this.RuntimeData.KingOfBattleStoreList.Clear();
					fileName = "Config\\CrusadeStore.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						KuaFuLueDuoStoreConfig item3 = new KuaFuLueDuoStoreConfig();
						item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item3.Type = (int)Global.GetSafeAttributeLong(node, "Type");
						item3.SaleData = Global.ParseGoodsFromStr_7(Global.GetSafeAttributeStr(node, "GoodsID").Split(new char[]
						{
							','
						}), 0);
						item3.ZuanShi = (int)Global.GetSafeAttributeLong(node, "ZuanShiNum");
						item3.JueXingNum = (int)Global.GetSafeAttributeLong(node, "JueXingNum");
						item3.SinglePurchase = (int)Global.GetSafeAttributeLong(node, "SinglePurchase");
						item3.BeginNum = (int)Global.GetSafeAttributeLong(node, "BeginNum");
						item3.EndNum = (int)Global.GetSafeAttributeLong(node, "EndNum");
						item3.RandNumMinus = item3.EndNum - item3.BeginNum + 1;
						this.RuntimeData.KingOfBattleStoreDict[item3.ID] = item3;
						this.RuntimeData.KingOfBattleStoreList.Add(item3);
						if (item3.Type == 2)
						{
							this.RuntimeData.BeginNum = Math.Min(this.RuntimeData.BeginNum, item3.BeginNum);
							this.RuntimeData.EndNum = Math.Max(this.RuntimeData.EndNum, item3.EndNum);
						}
					}
					this.RuntimeData.CrusadeOrePercent = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CrusadeOrePercent", ',');
					this.RuntimeData.CrusadeUltraKill = GameManager.systemParamsList.GetParamValueIntArrayByName("CrusadeUltraKill", ',');
					this.RuntimeData.CrusadeShutDown = GameManager.systemParamsList.GetParamValueIntArrayByName("CrusadeShutDown", ',');
					this.RuntimeData.CrusadeAwardAttacker = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CrusadeAwardAttacker", ',');
					this.RuntimeData.CrusadeAwardDefender = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CrusadeAwardDefender", ',');
					this.RuntimeData.CrusadeSeason = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeSeason", 13);
					this.RuntimeData.CrusadeOre = GameManager.systemParamsList.GetParamValueIntArrayByName("CrusadeOre", ',');
					this.RuntimeData.CrusadeMinApply = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeMinApply", 10000);
					this.RuntimeData.CrusadeApplyCD = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeApplyCD", 300);
					this.RuntimeData.CrusadeEnterTime = GameManager.systemParamsList.GetParamValueIntArrayByName("CrusadeEnterTime", ',');
					this.RuntimeData.CrusadeEnterPrice = GameManager.systemParamsList.GetParamValueIntArrayByName("CrusadeEnterPrice", ',');
					this.RuntimeData.CrusadePerfect = GameManager.systemParamsList.GetParamValueDoubleByName("CrusadePerfect", 0.0);
					this.RuntimeData.CrusadeStoreCD = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeStoreCD", 86400);
					this.RuntimeData.CrusadeStorePrice = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeStorePrice", 0);
					this.RuntimeData.CrusadeStoreRandomNum = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeStoreRandomNum", 8);
					this.RuntimeData.ZhanMengZiJin = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhanMengZiJin", ',');
					this.RuntimeData.HideRankList.Clear();
					List<string> hideranks = GameManager.systemParamsList.GetParamValueStringListByName("KuaFuLueDuoHideRankList", '|');
					if (null != hideranks)
					{
						foreach (string s in hideranks)
						{
							List<int> ilist = ConfigHelper.String2IntList(s, ',');
							if (ilist.Count > 1 && ilist[0] == (int)GameManager.PlatformType)
							{
								for (int i = 1; i < ilist.Count; i++)
								{
									this.RuntimeData.HideRankList.Add(ilist[i]);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x0006C164 File Offset: 0x0006A364
		public void LoadCollectXml()
		{
			string fileName = "";
			try
			{
				Dictionary<int, KuaFuLueDuoMonsterItem> dict = new Dictionary<int, KuaFuLueDuoMonsterItem>();
				fileName = Global.GameResPath("Config\\CrusadeCrystalMonster.xml");
				XElement xml = ConfigHelper.Load(fileName);
				if (null != xml)
				{
					foreach (XElement xmlItem in xml.Elements())
					{
						int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
						int monsterId = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MonsterID", "0"));
						dict[id] = new KuaFuLueDuoMonsterItem
						{
							ID = id,
							MonsterID = monsterId,
							Type = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Type", "0")),
							Name = Global.GetDefAttributeStr(xmlItem, "Name", ""),
							GatherTime = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GatherTime", "0")),
							FuHuoTime = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "FuHuoTime", "0")),
							ZiYuan = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Ore", "0")),
							JiFen = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "JiFen", "0")),
							X = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "X", "0")),
							Y = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Y", "0"))
						};
					}
				}
				this.RuntimeData.CollectMonsterDict = dict;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", fileName, ex.Message), ex, true);
			}
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x0006C37C File Offset: 0x0006A57C
		public void RefreshKuaFuLueDuoStoreData(KuaFuLueDuoStoreData KuaFuLueDuoStoreData, bool SetRefreshTm = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (SetRefreshTm)
				{
					KuaFuLueDuoStoreData.LastRefTime = TimeUtil.NowDateTime();
				}
				KuaFuLueDuoStoreData.SaleList.Clear();
				List<KuaFuLueDuoStoreConfig> KOBStoreList = this.RuntimeData.KingOfBattleStoreList;
				int PercentZero = this.RuntimeData.BeginNum;
				int PercentOne = this.RuntimeData.EndNum;
				for (int Num = 0; Num < this.RuntimeData.CrusadeStoreRandomNum; Num++)
				{
					int rate = Global.GetRandomNumber(PercentZero, PercentOne);
					for (int i = 0; i < KOBStoreList.Count; i++)
					{
						KuaFuLueDuoStoreConfig item = KOBStoreList[i];
						if (item.Type == 1)
						{
							if (!item.RandSkip)
							{
								item.RandSkip = true;
								KuaFuLueDuoStoreSaleData SaleData = new KuaFuLueDuoStoreSaleData();
								SaleData.ID = item.ID;
								KuaFuLueDuoStoreData.SaleList.Add(SaleData);
							}
						}
						else if (item.Type == 2)
						{
							if (item.RandSkip)
							{
								rate += item.RandNumMinus;
							}
							else if (rate >= item.BeginNum && rate <= item.EndNum)
							{
								item.RandSkip = true;
								PercentOne -= item.RandNumMinus;
								KuaFuLueDuoStoreSaleData SaleData = new KuaFuLueDuoStoreSaleData();
								SaleData.ID = item.ID;
								KuaFuLueDuoStoreData.SaleList.Add(SaleData);
							}
						}
					}
				}
				for (int i = 0; i < KOBStoreList.Count; i++)
				{
					KOBStoreList[i].RandSkip = false;
				}
			}
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x0006C580 File Offset: 0x0006A780
		public KuaFuLueDuoStoreData GetClientKuaFuLueDuoStoreData(GameClient client)
		{
			KuaFuLueDuoStoreData kuaFuLueDuoStoreData;
			if (null != client.ClientData.KuaFuLueDuoStoreData)
			{
				kuaFuLueDuoStoreData = client.ClientData.KuaFuLueDuoStoreData;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					client.ClientData.KuaFuLueDuoStoreData = new KuaFuLueDuoStoreData();
					client.ClientData.KuaFuLueDuoStoreData.LastRefTime = Global.GetRoleParamsDateTimeFromDB(client, "10202");
					client.ClientData.KuaFuLueDuoStoreData.SaleList = new List<KuaFuLueDuoStoreSaleData>();
					List<ushort> StoreSaleDataList = Global.GetRoleParamsUshortListFromDB(client, "46");
					for (int index = 0; index < StoreSaleDataList.Count - 1; index += 2)
					{
						KuaFuLueDuoStoreSaleData SaleData = new KuaFuLueDuoStoreSaleData();
						SaleData.ID = (int)StoreSaleDataList[index];
						SaleData.Purchase = (int)StoreSaleDataList[index + 1];
						client.ClientData.KuaFuLueDuoStoreData.SaleList.Add(SaleData);
					}
				}
				kuaFuLueDuoStoreData = client.ClientData.KuaFuLueDuoStoreData;
			}
			return kuaFuLueDuoStoreData;
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x0006C6A8 File Offset: 0x0006A8A8
		public void SaveKuaFuLueDuoStoreData(GameClient client)
		{
			if (null != client.ClientData.KuaFuLueDuoStoreData)
			{
				lock (this.RuntimeData.Mutex)
				{
					KuaFuLueDuoStoreData KuaFuLueDuoStoreData = client.ClientData.KuaFuLueDuoStoreData;
					Global.SaveRoleParamsDateTimeToDB(client, "10202", KuaFuLueDuoStoreData.LastRefTime, true);
					List<ushort> StoreSaleDataList = new List<ushort>();
					foreach (KuaFuLueDuoStoreSaleData item in KuaFuLueDuoStoreData.SaleList)
					{
						StoreSaleDataList.Add((ushort)item.ID);
						StoreSaleDataList.Add((ushort)item.Purchase);
					}
					Global.SaveRoleParamsUshortListToDB(client, StoreSaleDataList, "46", true);
				}
			}
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x0006C7A8 File Offset: 0x0006A9A8
		public bool ProcessGetKuaFuLueDuoMallDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpenedEnter(client, false))
				{
					return false;
				}
				KuaFuLueDuoStoreData KuaFuLueDuoStoreData = this.GetClientKuaFuLueDuoStoreData(client);
				if ((TimeUtil.NowDateTime() - KuaFuLueDuoStoreData.LastRefTime).TotalSeconds >= (double)this.RuntimeData.CrusadeStoreCD)
				{
					this.RefreshKuaFuLueDuoStoreData(KuaFuLueDuoStoreData, true);
					this.SaveKuaFuLueDuoStoreData(client);
				}
				client.sendCmd<KuaFuLueDuoStoreData>(nID, KuaFuLueDuoStoreData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x0006C850 File Offset: 0x0006AA50
		public bool ProcessKuaFuLueDuoMallBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpenedEnter(client, false))
				{
					return false;
				}
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int storeID = Global.SafeConvertToInt32(cmdParams[1]);
				int countNum = Global.SafeConvertToInt32(cmdParams[2]);
				KuaFuLueDuoStoreConfig KOBattleStoreConfig = null;
				string strcmd;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.KingOfBattleStoreDict.TryGetValue(storeID, out KOBattleStoreConfig))
					{
						result = -20;
						strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
				}
				KuaFuLueDuoStoreData KuaFuLueDuoStoreData = this.GetClientKuaFuLueDuoStoreData(client);
				KuaFuLueDuoStoreSaleData SaleData = null;
				foreach (KuaFuLueDuoStoreSaleData item in KuaFuLueDuoStoreData.SaleList)
				{
					if (item.ID == storeID)
					{
						SaleData = item;
						break;
					}
				}
				if (null == SaleData)
				{
					result = -20;
					strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (KOBattleStoreConfig.SinglePurchase - SaleData.Purchase < countNum)
				{
					result = -36;
					strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (!Global.CanAddGoods(client, KOBattleStoreConfig.SaleData.GoodsID, KOBattleStoreConfig.SaleData.GCount * countNum, KOBattleStoreConfig.SaleData.Binding, "1900-01-01 12:00:00", true, false))
				{
					result = -100;
					strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (KOBattleStoreConfig.JueXingNum > 0)
				{
					int curKingOfBattlePoint = client.ClientData.JueXingPoint;
					if (curKingOfBattlePoint < KOBattleStoreConfig.JueXingNum * countNum)
					{
						result = -45;
						strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
				}
				else if (KOBattleStoreConfig.ZuanShi > 0)
				{
					if (client.ClientData.UserMoney < KOBattleStoreConfig.ZuanShi * countNum)
					{
						result = -10;
						strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
				}
				if (KOBattleStoreConfig.JueXingNum > 0)
				{
					if (!GameManager.ClientMgr.ModifyJueXingValue(client, -KOBattleStoreConfig.JueXingNum * countNum, "觉醒商城购买", false))
					{
						result = -45;
						strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
				}
				else if (KOBattleStoreConfig.ZuanShi > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(client, KOBattleStoreConfig.ZuanShi * countNum, "觉醒商城购买", true, true, true, true, DaiBiSySType.None))
					{
						result = -10;
						strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
				}
				GoodsData goodsData = KOBattleStoreConfig.SaleData;
				Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount * countNum, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, string.Format("觉醒商城", new object[0]), false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
				SaleData.Purchase += countNum;
				this.SaveKuaFuLueDuoStoreData(client);
				strcmd = string.Format("{0}:{1}:{2}", result, storeID, SaleData.Purchase);
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x0006CD8C File Offset: 0x0006AF8C
		public bool ProcessKuaFuLueDuoMallRefreshCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpenedEnter(client, false))
				{
					return false;
				}
				int result = 0;
				string strcmd;
				if (client.ClientData.UserMoney < this.RuntimeData.CrusadeStorePrice)
				{
					result = 7;
					strcmd = string.Format("{0}", result);
					client.sendCmd<int>(nID, result, false);
					return true;
				}
				if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.RuntimeData.CrusadeStorePrice, "觉醒商城刷新", true, true, false, DaiBiSySType.None))
				{
					result = 7;
					strcmd = string.Format("{0}", result);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				KuaFuLueDuoStoreData KuaFuLueDuoStoreData = this.GetClientKuaFuLueDuoStoreData(client);
				this.RefreshKuaFuLueDuoStoreData(KuaFuLueDuoStoreData, false);
				this.SaveKuaFuLueDuoStoreData(client);
				client.sendCmd<KuaFuLueDuoStoreData>(1257, KuaFuLueDuoStoreData, false);
				strcmd = string.Format("{0}", result);
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x0006CEEC File Offset: 0x0006B0EC
		public bool ProcessGetKuaFuLueDuoMainInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int bhid = client.ClientData.Faction;
				KuaFuLueDuoMainInfo mainInfo = DataHelper.BytesToObject<KuaFuLueDuoMainInfo>(bytes, 0, bytes.Length);
				if (null == mainInfo)
				{
					mainInfo = new KuaFuLueDuoMainInfo();
				}
				lock (this.RuntimeData.Mutex)
				{
					if (mainInfo.StateListAge != this.SyncDataCache.StateAge)
					{
						if (this.RuntimeData.JingJiaDict.TryGetValue(bhid, out mainInfo.JingJiaData))
						{
						}
						mainInfo.StateListAge = this.SyncDataCache.StateAge;
						if (null != this.SyncDataCache.StateList)
						{
							mainInfo.StateList = this.SyncDataCache.StateList.Values.ToList<KuaFuLueDuoServerJingJiaState>();
							foreach (KuaFuLueDuoServerJingJiaState sData in mainInfo.StateList)
							{
								if (null != sData.JingJiaList)
								{
									foreach (KuaFuLueDuoBangHuiJingJiaData item in sData.JingJiaList)
									{
										item.ZiJin = 0;
									}
								}
							}
						}
					}
					if (mainInfo.ServerListAge != this.SyncDataCache.ServerInfoDictAge)
					{
						mainInfo.ServerListAge = this.SyncDataCache.ServerInfoDictAge;
						mainInfo.ServerList = this.SyncDataCache.ServerInfoDict.Values.ToList<KuaFuLueDuoServerInfo>();
					}
				}
				client.sendCmd<KuaFuLueDuoMainInfo>(nID, mainInfo, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x0006D148 File Offset: 0x0006B348
		private int GetKuaFuLueDuoRankNum(GameClient client, int ranktype)
		{
			int rankNum = 0;
			switch (ranktype)
			{
			case 0:
			{
				int key = client.ServerId;
				KuaFuLueDuoServerInfo info;
				if (this.SyncDataCache.ServerInfoDict.TryGetValue(key, out info))
				{
					rankNum = ((info.ZhengFuList == null) ? 0 : info.ZhengFuList.Count);
				}
				break;
			}
			case 2:
			{
				int key = client.ClientData.Faction;
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.CacheBh2LueDuoDict.TryGetValue(key, out rankNum))
					{
						return rankNum;
					}
				}
				KuaFuLueDuoBHData info2 = HuanYingSiYuanClient.getInstance().GetBHDataByBhid_KuaFuLueDuo(key);
				if (null != info2)
				{
					rankNum = info2.sum_ziyuan;
					if (rankNum >= 0)
					{
						lock (this.RuntimeData.Mutex)
						{
							this.RuntimeData.CacheBh2LueDuoDict[key] = rankNum;
						}
					}
				}
				break;
			}
			case 4:
			{
				int key = client.ClientData.RoleID;
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.CacheRole2KillDict.TryGetValue(key, out rankNum))
					{
						return rankNum;
					}
				}
				byte[] bytes = HuanYingSiYuanClient.getInstance().GetRoleData_KuaFuLueDuo((long)key);
				if (null != bytes)
				{
					rankNum = DataHelper.BytesToObject<int>(bytes, 0, bytes.Length);
					if (rankNum >= 0)
					{
						lock (this.RuntimeData.Mutex)
						{
							this.RuntimeData.CacheRole2KillDict[key] = rankNum;
						}
					}
				}
				break;
			}
			}
			return rankNum;
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x0006D3B8 File Offset: 0x0006B5B8
		public bool ProcessGetKuaFuLueDuoRankInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuLueDuoRankListCmdData request = DataHelper.BytesToObject<KuaFuLueDuoRankListCmdData>(bytes, 0, bytes.Length);
				if (request == null)
				{
					request = new KuaFuLueDuoRankListCmdData();
				}
				int type = request.RankType;
				KuaFuLueDuoRankListCmdData cmdData = new KuaFuLueDuoRankListCmdData
				{
					RankType = type
				};
				int selfValue = this.GetKuaFuLueDuoRankNum(client, type);
				cmdData.SelfData = new KuaFuLueDuoRankInfo
				{
					Value = selfValue
				};
				lock (this.RuntimeData.Mutex)
				{
					KuaFuLueDuoRankListData data = this.SyncDataCache.KuaFuLueDuoRankInfoDict;
					if (data != null && !this.RuntimeData.HideRankList.Contains(type))
					{
						List<KuaFuLueDuoRankInfo> rankInfoList = new List<KuaFuLueDuoRankInfo>();
						if (data.ListDict != null && data.ListDict.TryGetValue(type, out rankInfoList))
						{
							cmdData.ListRankList = rankInfoList;
						}
						KuaFuLueDuoRankInfo last;
						if (data.LastInfoDict != null && data.LastInfoDict.TryGetValue(type, out last))
						{
							cmdData.LastData = last;
						}
					}
				}
				client.sendCmd<KuaFuLueDuoRankListCmdData>(nID, cmdData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x0006D54C File Offset: 0x0006B74C
		public bool ProcessGetKuaFuLueDuoAnalysisDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x0006D590 File Offset: 0x0006B790
		private int UpdateClientEnterNum(GameClient client)
		{
			int dayID = TimeUtil.GetOffsetDay2(TimeUtil.NowDateTime());
			if (this.SyncDataCache.GameState == 3)
			{
				if (client.ClientData.KuaFuLueDuoEnterNumDayID < dayID)
				{
					GameManager.ClientMgr.ModifyKuaFuLueDuoBuyNumAndDayID(client, 0, dayID, "重置次数");
					int addNum = Math.Min(this.RuntimeData.CrusadeEnterTime[1], client.ClientData.KuaFuLueDuoEnterNum + this.RuntimeData.CrusadeEnterTime[0]) - client.ClientData.KuaFuLueDuoEnterNum;
					if (addNum > 0)
					{
						GameManager.ClientMgr.ModifyKuaFuLueDuoEnterNum(client, addNum, "系统每轮补充", false);
					}
				}
			}
			return dayID;
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x0006D64C File Offset: 0x0006B84C
		public bool ProcessKuaFuLueDuoBuyEnterNumCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = -18;
				int count = Global.SafeConvertToInt32(cmdParams[1]);
				if (this.SyncDataCache.GameState != 3)
				{
					result = -2001;
				}
				else
				{
					int dayID = this.UpdateClientEnterNum(client);
					result = client.ClientData.KuaFuLueDuoEnterNum;
					if (result + count > this.RuntimeData.CrusadeEnterTime[1])
					{
						result = -36;
					}
					else
					{
						while (count > 0)
						{
							int buyNum = Global.Clamp(client.ClientData.KuaFuLueDuoEnterNumBuyNum, 0, this.RuntimeData.CrusadeEnterPrice.Length - 1);
							int price = this.RuntimeData.CrusadeEnterPrice[buyNum];
							lock (client.ClientData.UserMoneyMutex)
							{
								if (client.ClientData.UserMoney < price)
								{
									result = -10;
									break;
								}
								if (!GameManager.ClientMgr.SubUserMoney(client, price, "购买跨服掠夺进入次数", true, true, true, true, DaiBiSySType.None))
								{
									result = -10;
									break;
								}
							}
							GameManager.ClientMgr.ModifyKuaFuLueDuoEnterNum(client, 1, "购买跨服掠夺进入次数", false);
							GameManager.ClientMgr.ModifyKuaFuLueDuoBuyNumAndDayID(client, client.ClientData.KuaFuLueDuoEnterNumBuyNum + 1, dayID, "购买跨服掠夺进入次数");
							result = client.ClientData.KuaFuLueDuoEnterNum;
							count--;
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x0006D82C File Offset: 0x0006BA2C
		public bool ProcessKuaFuLueDuoJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int serverId = Global.SafeConvertToInt32(cmdParams[1]);
				int ziJin = Global.SafeConvertToInt32(cmdParams[2]);
				int bhid = client.ClientData.Faction;
				int rid = client.ClientData.RoleID;
				if (!this.IsGongNengOpenedJingJia(client, false))
				{
					result = -400;
				}
				else if (bhid <= 0 || client.ClientData.BHZhiWu != 1)
				{
					result = -1002;
				}
				else if (this.SyncDataCache.GameState != 1)
				{
					result = -2001;
				}
				else
				{
					BangHuiDetailData bangHuiMiniData = Global.GetBangHuiDetailData(rid, bhid, 0);
					if (null == bangHuiMiniData)
					{
						result = -1001;
					}
					else if (serverId <= 0 || serverId == GameManager.ServerId)
					{
						result = -18;
					}
					else if (bangHuiMiniData.ZoneID == serverId)
					{
						result = -18;
					}
					else
					{
						KuaFuLueDuoConfig sceneItem = null;
						KuaFuLueDuoGameStates state = KuaFuLueDuoGameStates.None;
						int signUpRound = 0;
						result = this.CheckCondition(client, ref sceneItem, ref state, ref signUpRound);
						if (state != KuaFuLueDuoGameStates.SignUp)
						{
							result = -2001;
						}
						if (result >= 0)
						{
							KuaFuLueDuoBangHuiJingJiaData jingJiaData;
							lock (this.RuntimeData.Mutex)
							{
								if (!this.RuntimeData.JingJiaDict.TryGetValue(bhid, out jingJiaData))
								{
									jingJiaData = new KuaFuLueDuoBangHuiJingJiaData
									{
										BhId = bhid,
										ZoneId = bangHuiMiniData.ZoneID,
										BhName = bangHuiMiniData.BHName,
										ServerId = serverId
									};
									this.RuntimeData.JingJiaDict[bhid] = jingJiaData;
								}
								if (jingJiaData.ServerId > 0 && jingJiaData.ServerId != serverId && jingJiaData.ZiJin > 0)
								{
									result = -1004;
									goto IL_30F;
								}
								if (ziJin < jingJiaData.ZiJin + this.RuntimeData.CrusadeMinApply)
								{
									result = -1043;
									goto IL_30F;
								}
								if (bangHuiMiniData.TotalMoney < ziJin - jingJiaData.ZiJin + this.RuntimeData.ZhanMengZiJin[0])
								{
									result = -1041;
									goto IL_30F;
								}
							}
							KuaFuLueDuoJingJiaResult ret = HuanYingSiYuanClient.getInstance().JingJia_KuaFuLueDuo(bhid, bangHuiMiniData.ZoneID, bangHuiMiniData.BHName, ziJin, serverId, jingJiaData.ZiJin);
							result = ret.Result;
							jingJiaData.ZiJin = ret.ZiJin;
							if (ret.Result >= 0)
							{
								int cost = ziJin - ret.ZiJin;
								int bhZoneId;
								if (!GameManager.ClientMgr.SubBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cost, out bhZoneId))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("帮会{0}资金扣除{1}失败", bhid, cost), null, true);
								}
							}
						}
					}
				}
				IL_30F:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x0006DBB0 File Offset: 0x0006BDB0
		public bool ProcessKuaFuLueDuoEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int serverID = Global.SafeConvertToInt32(cmdParams[1]);
				int bhid = client.ClientData.Faction;
				if (!this.IsGongNengOpenedEnter(client, true))
				{
					result = -13;
				}
				else if (client.ClientSocket.IsKuaFuLogin)
				{
					result = -21;
				}
				else
				{
					if (serverID == 0)
					{
						serverID = GameManager.ServerId;
					}
					if (bhid <= 0 && serverID != GameManager.ServerId)
					{
						result = -1000;
					}
					else if (this.SyncDataCache.GameState != 3)
					{
						result = -2001;
					}
					else
					{
						KuaFuLueDuoConfig sceneItem = null;
						KuaFuLueDuoGameStates state = KuaFuLueDuoGameStates.None;
						int signUpRound = 0;
						if (!this.CheckMap(client))
						{
							result = -21;
						}
						else
						{
							result = this.CheckCondition(client, ref sceneItem, ref state, ref signUpRound);
							if (result >= 0)
							{
								lock (this.RuntimeData.Mutex)
								{
									FightInfo sfi = null;
									if (this.SyncDataCache.ServerZiYuanDict != null && this.SyncDataCache.ServerZiYuanDict.TryGetValue(serverID, out sfi))
									{
										int leftZiYuan = sfi.ZiYuan;
										if (leftZiYuan < 0)
										{
											result = -1044;
											goto IL_367;
										}
									}
									FightInfo bhfi;
									if (serverID == client.ServerId)
									{
										if (sfi != null && sfi.RoleNum >= sceneItem.DefenderMaxNum)
										{
											result = -1045;
											goto IL_367;
										}
									}
									else if (this.SyncDataCache.BhZiYuanDict.TryGetValue(bhid, out bhfi) && bhfi.RoleNum >= sceneItem.AttackerMaxNum)
									{
										result = -1045;
										goto IL_367;
									}
								}
								KuaFuServerInfo kfserverInfo = null;
								KuaFuLueDuoFuBenData fubenData = HuanYingSiYuanClient.getInstance().GetFuBenDataByServerId_KuaFuLueDuo(serverID);
								if (fubenData == null || !KuaFuManager.getInstance().TryGetValue(fubenData.ServerId, out kfserverInfo))
								{
									result = -11000;
								}
								else if (fubenData.BhDataList.Count == 0)
								{
									result = -1046;
								}
								else if (fubenData.State >= 3 || fubenData.LeftZiYuan <= 0)
								{
									result = -1044;
								}
								else
								{
									if (!VideoLogic.getInstance().IsGuanZhanGM(client))
									{
										if (fubenData.DestServerId != GameManager.ServerId && !fubenData.BhDataList.Contains(bhid))
										{
											result = -4008;
											goto IL_367;
										}
										if (client.ClientData.KuaFuLueDuoEnterNum <= 0)
										{
											result = -16;
											goto IL_367;
										}
									}
									KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
									if (null != clientKuaFuServerLoginData)
									{
										clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
										clientKuaFuServerLoginData.GameId = fubenData.GameId;
										clientKuaFuServerLoginData.GameType = 25;
										clientKuaFuServerLoginData.EndTicks = 0L;
										clientKuaFuServerLoginData.ServerId = client.ServerId;
										clientKuaFuServerLoginData.ServerIp = kfserverInfo.Ip;
										clientKuaFuServerLoginData.ServerPort = kfserverInfo.Port;
									}
									GlobalNew.RecordSwitchKuaFuServerLog(client);
									client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
								}
							}
						}
					}
				}
				IL_367:
				client.sendCmd<int>(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x0006DF8C File Offset: 0x0006C18C
		public bool ProcessGetKuaFuLueDuoStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int bhid = client.ClientData.Faction;
				KuaFuLueDuoStateData stateData = DataHelper.BytesToObject<KuaFuLueDuoStateData>(bytes, 0, bytes.Length);
				if (null == stateData)
				{
					stateData = new KuaFuLueDuoStateData();
				}
				stateData.GameState = this.SyncDataCache.GameState;
				this.UpdateClientEnterNum(client);
				lock (this.RuntimeData.Mutex)
				{
					if (this.SyncDataCache.ServerInfoDict.ContainsKey(GameManager.ServerId))
					{
						stateData.ServerID = GameManager.ServerId;
						if (this.SyncDataCache.GameState >= 1 && this.SyncDataCache.GameState <= 4)
						{
							KuaFuLueDuoServerJingJiaState jjData;
							if (this.SyncDataCache.StateList != null && this.SyncDataCache.StateList.TryGetValue(GameManager.ServerId, out jjData))
							{
								if (jjData.JingJiaList != null && jjData.JingJiaList.Count > 0)
								{
									stateData.AttackerList = new List<BangHuiMiniData>();
									foreach (KuaFuLueDuoBangHuiJingJiaData bh in jjData.JingJiaList)
									{
										stateData.AttackerList.Add(new BangHuiMiniData
										{
											BHID = bh.BhId,
											BHName = bh.BhName,
											ZoneID = bh.ZoneId
										});
									}
								}
							}
							if (this.SyncDataCache.ServerZiYuanDict != null)
							{
								FightInfo fi;
								if (this.SyncDataCache.ServerZiYuanDict.TryGetValue(GameManager.ServerId, out fi))
								{
									stateData.ZiYuan = fi.ZiYuan;
								}
							}
							if (bhid > 0)
							{
								KuaFuLueDuoBangHuiJingJiaData bjData;
								if (this.RuntimeData.JingJiaDict.TryGetValue(bhid, out bjData))
								{
									stateData.EnemyServerID = bjData.ServerId;
								}
								if (this.SyncDataCache.ServerZiYuanDict != null)
								{
									FightInfo fi;
									if (this.SyncDataCache.ServerZiYuanDict.TryGetValue(stateData.EnemyServerID, out fi))
									{
										stateData.EnemyZiYuan = fi.ZiYuan;
									}
								}
								if (this.SyncDataCache.BhZiYuanDict != null)
								{
									FightInfo fi;
									if (this.SyncDataCache.BhZiYuanDict.TryGetValue(bhid, out fi))
									{
										stateData.LueDuoZiYuan = fi.ZiYuan;
									}
								}
							}
						}
					}
				}
				KuaFuLueDuoConfig sceneInfo = this.RuntimeData.CommonConfigData.GetKuaFuLueDuoConfig(0);
				stateData.AwardsDataList = this.GetClientAwardsDataList(client, sceneInfo);
				client.sendCmd<KuaFuLueDuoStateData>(nID, stateData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x0006E2FC File Offset: 0x0006C4FC
		public bool ProcessGetKuaFuLueDuoAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuLueDuoConfig lastSceneItem = this.RuntimeData.CommonConfigData.GetKuaFuLueDuoConfig(0);
				this.NtfCanGetAward(client, lastSceneItem);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x0006E35C File Offset: 0x0006C55C
		public bool ProcessGetKuaFuLueDuoAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuLueDuoConfig lastSceneItem = this.RuntimeData.CommonConfigData.GetKuaFuLueDuoConfig(0);
				int err = this.GiveRoleAwards(client, lastSceneItem);
				client.sendCmd<int>(nID, err, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x0006E3C8 File Offset: 0x0006C5C8
		private bool CheckMap(GameClient client)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return sceneType == SceneUIClasses.Normal;
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x0006E3F9 File Offset: 0x0006C5F9
		private void OnInitGame(GameClient client)
		{
			this.UpdateChengHaoBuffer(client, false);
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x0006E408 File Offset: 0x0006C608
		private KuaFuLueDuoFuBenData GetFuBenDataByGameId(long gameId)
		{
			KuaFuLueDuoFuBenData fuBenData = null;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData))
				{
					fuBenData = null;
				}
			}
			if (null == fuBenData)
			{
				KuaFuLueDuoFuBenData newFuBenData = HuanYingSiYuanClient.getInstance().GetFuBenDataByGameId_KuaFuLueDuo(gameId);
				if (newFuBenData == null)
				{
					LogManager.WriteLog(LogTypes.Error, ("获取不到有效的副本数据," + newFuBenData == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
					return null;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData))
					{
						fuBenData = newFuBenData;
						fuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.FuBenItemData[fuBenData.GameId] = fuBenData;
					}
				}
			}
			return fuBenData;
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x0006E554 File Offset: 0x0006C754
		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			KuaFuLueDuoFuBenData fuBenData = this.GetFuBenDataByGameId(kuaFuServerLoginData.GameId);
			bool result;
			if (fuBenData == null || fuBenData.ServerId != GameManager.ServerId)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x0006E5BC File Offset: 0x0006C7BC
		public bool OnInitGameKuaFu(GameClient client)
		{
			int bhid = client.ClientData.Faction;
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			KuaFuLueDuoFuBenData fuBenData = this.GetFuBenDataByGameId(kuaFuServerLoginData.GameId);
			bool result;
			if (null == fuBenData)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("角色({0})进入跨服活动({1})失败,未获取到活动副本({2})信息", client.ClientData.RoleID, SceneUIClasses.KuaFuLueDuo, kuaFuServerLoginData.GameId), null, true);
				client.ClientData.PushMessageID = GLang.GetLang(2000, new object[0]);
				result = false;
			}
			else if (fuBenData.State >= 3)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("角色({0})进入跨服活动({1})失败,活动副本({2})已结束", client.ClientData.RoleID, SceneUIClasses.KuaFuLueDuo, kuaFuServerLoginData.GameId), null, true);
				client.ClientData.PushMessageID = GLang.GetLang(2000, new object[0]);
				result = false;
			}
			else
			{
				int side = 0;
				if (client.ServerId == fuBenData.DestServerId)
				{
					side = 1;
					client.ClientData.BirthSide = 1;
				}
				else
				{
					int index = fuBenData.ServerIdList.IndexOf(client.ServerId);
					if (index < 0)
					{
						if (!VideoLogic.getInstance().IsGuanZhanGM(client))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("角色({0})进入跨服活动({1})失败,角色服务器ID({2})无效", client.ClientData.RoleID, SceneUIClasses.KuaFuLueDuo, client.ServerId), null, true);
							return false;
						}
						client.ClientData.HideGM = 1;
					}
					else
					{
						side = index + 1;
					}
					index = fuBenData.BhDataList.IndexOf(client.ClientData.Faction);
					if (index < 0)
					{
						if (!VideoLogic.getInstance().IsGuanZhanGM(client))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("角色({0})进入跨服活动({1})失败,角色战盟ID({2})无效", client.ClientData.RoleID, SceneUIClasses.KuaFuLueDuo, client.ClientData.Faction), null, true);
							return false;
						}
						client.ClientData.HideGM = 1;
					}
					else
					{
						client.ClientData.BirthSide = index + 1 + 1;
					}
				}
				KuaFuLueDuoConfig sceneInfo;
				lock (this.RuntimeData.Mutex)
				{
					kuaFuServerLoginData.FuBenSeqId = fuBenData.SequenceId;
					sceneInfo = this.RuntimeData.CommonConfigData.KuaFuLueDuoConfigDict.Values.FirstOrDefault<KuaFuLueDuoConfig>();
					if (null == sceneInfo)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("角色({0})进入跨服活动({1})失败,配置无效", client.ClientData.RoleID, SceneUIClasses.KuaFuLueDuo), null, true);
						return false;
					}
					client.SceneInfoObject = sceneInfo;
					client.ClientData.MapCode = sceneInfo.MapCode;
					client.ClientData.BattleWhichSide = side;
				}
				int toMapCode;
				int toPosX;
				int toPosY;
				if (!this.GetZhanMengBirthPoint(sceneInfo, client, client.ClientData.MapCode, out toMapCode, out toPosX, out toPosY, false))
				{
					LogManager.WriteLog(LogTypes.Error, "无法获取有效的阵营和出生点,进入跨服失败,side=" + client.ClientData.BattleWhichSide, null, true);
					result = false;
				}
				else
				{
					client.ClientData.PosX = toPosX;
					client.ClientData.PosY = toPosY;
					client.ClientData.FuBenSeqID = kuaFuServerLoginData.FuBenSeqId;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x0006E974 File Offset: 0x0006CB74
		public bool GetZhanMengBirthPoint(KuaFuLueDuoConfig sceneInfo, GameClient client, int toMapCode, out int mapCode, out int posX, out int posY, bool isLogin = false)
		{
			mapCode = sceneInfo.MapCode;
			posX = 0;
			posY = 0;
			int side = client.ClientData.BirthSide;
			bool result;
			if (client.ClientData.HideGM > 0)
			{
				if (VideoLogic.getInstance().GetGuanZhanPos(toMapCode, ref posX, ref posY))
				{
					mapCode = toMapCode;
				}
				result = true;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					KuaFuLueDuoScene scene = client.SceneObject as KuaFuLueDuoScene;
					if (null != scene)
					{
						QiZhiConfig item = scene.QiZhiItem;
						if (item != null && item.Alive && item.BattleWhichSide == client.ClientData.BattleWhichSide)
						{
							Point BirthPoint = Global.GetMapPointByGridXY(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, item.PosX / scene.MapGridWidth, item.PosY / scene.MapGridHeight, item.RebirthRadius / scene.MapGridWidth, 0, false);
							posX = (int)BirthPoint.X;
							posY = (int)BirthPoint.Y;
							return true;
						}
					}
					MapBirthPoint birthPoint = null;
					if (!this.RuntimeData.MapBirthPointDict.TryGetValue(side, out birthPoint))
					{
						return false;
					}
					posX = birthPoint.BirthPosX;
					posY = birthPoint.BirthPosY;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x0006EB0C File Offset: 0x0006CD0C
		public void HandleNtfEnterEvent(KuaFuLueDuoNtfEnterData data)
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				if (this.IsGongNengOpenedEnter(client, false) && this.CheckMap(client))
				{
					if (client != null && data.BhIdList.Contains(client.ClientData.Faction))
					{
						client.sendCmd<int>(1256, 1, false);
					}
				}
			}
			LogManager.WriteLog(LogTypes.Error, string.Format("通知战盟ID={0}拥有进入跨服掠夺资格", string.Join<int>(",", data.BhIdList)), null, true);
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x0006EBD8 File Offset: 0x0006CDD8
		private void TimerProc(object sender, EventArgs e)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RequestSyncData.ServerID = GameManager.ServerId;
				this.RequestSyncData.StateAge = this.SyncDataCache.StateAge;
				this.RequestSyncData.ServerInfoDictAge = this.SyncDataCache.ServerInfoDictAge;
				this.RequestSyncData.KuaFuLueDuoRankInfoDict.Age = this.SyncDataCache.KuaFuLueDuoRankInfoDict.Age;
				this.RequestSyncData.FuBenStateAge = this.SyncDataCache.FuBenStateAge;
				if (this.RuntimeData.UpdateZiYuanData)
				{
					this.RequestSyncData.FuBenStateAge = 1L;
					this.RequestSyncData.BhZiYuanDict = new Dictionary<int, FightInfo>(this.RuntimeData.BhZiYuanDict);
					this.RequestSyncData.ServerZiYuanDict = new Dictionary<int, FightInfo>(this.RuntimeData.ServerZiYuanDict);
					this.RuntimeData.BhZiYuanDict.Clear();
					this.RuntimeData.ServerZiYuanDict.Clear();
					this.RuntimeData.UpdateZiYuanData = false;
				}
				KuaFuLueDuoSyncData SyncData = HuanYingSiYuanClient.getInstance().SyncData_KuaFuLueDuo(this.RequestSyncData);
				if (null != SyncData)
				{
					if (this.SyncDataCache.SeasonID != SyncData.SeasonID)
					{
						this.SyncDataCache.SeasonID = SyncData.SeasonID;
						this.RuntimeData.JingJiaDict.Clear();
					}
					this.SyncDataCache.LastSeasonID = SyncData.LastSeasonID;
					if (SyncData.GameState < 0)
					{
						this.SyncDataCache.GameState = (this.SyncDataCache.ServerGameState = SyncData.ServerGameState);
					}
					else
					{
						if (SyncData.GroupID != this.SyncDataCache.GroupID)
						{
						}
						if (SyncData.ServerInfoDictAge != this.RequestSyncData.ServerInfoDictAge)
						{
							this.SyncDataCache.ServerInfoDictAge = SyncData.ServerInfoDictAge;
							this.SyncDataCache.ServerInfoDict = SyncData.ServerInfoDict;
						}
						if (SyncData.FuBenStateAge != this.RequestSyncData.FuBenStateAge)
						{
							this.SyncDataCache.FuBenStateAge = SyncData.FuBenStateAge;
							this.SyncDataCache.ServerZiYuanDict = SyncData.ServerZiYuanDict;
							this.SyncDataCache.BhZiYuanDict = SyncData.BhZiYuanDict;
						}
						this.SyncDataCache.GameState = SyncData.GameState;
						if (SyncData.StateAge != this.RequestSyncData.StateAge)
						{
							this.SyncDataCache.StateAge = SyncData.StateAge;
							this.SyncDataCache.SignUpRound = SyncData.SignUpRound;
							this.SyncDataCache.StateList = SyncData.StateList;
							HashSet<int> newList = new HashSet<int>();
							Dictionary<int, KuaFuLueDuoBangHuiJingJiaData> dict = new Dictionary<int, KuaFuLueDuoBangHuiJingJiaData>();
							if (null != this.SyncDataCache.StateList)
							{
								foreach (KuaFuLueDuoServerJingJiaState jjData in this.SyncDataCache.StateList.Values)
								{
									if (null != jjData.JingJiaList)
									{
										foreach (KuaFuLueDuoBangHuiJingJiaData data in jjData.JingJiaList)
										{
											KuaFuLueDuoBangHuiJingJiaData data2;
											if (this.RuntimeData.JingJiaDict.TryGetValue(data.BhId, out data2))
											{
												data2.ZiJin = data.ZiJin;
												data2.ServerId = data.ServerId;
												data2.Age = this.SyncDataCache.StateAge;
											}
											else
											{
												data.Age = this.SyncDataCache.StateAge;
												this.RuntimeData.JingJiaDict[data.BhId] = data;
											}
										}
									}
								}
							}
							List<int> removeList = new List<int>();
							foreach (KuaFuLueDuoBangHuiJingJiaData data2 in this.RuntimeData.JingJiaDict.Values)
							{
								if (data2.Age < this.SyncDataCache.StateAge)
								{
									if (data2.ServerId > 0 && data2.ZiJin > 0)
									{
										int bhid = data2.BhId;
										int zhanMengZiJin = data2.ZiJin;
										int serverID = data2.ServerId;
										data2.ServerId = 0;
										data2.ZiJin = 0;
										BangHuiDetailData detailData = Global.GetBangHuiDetailData(-1, bhid, 0);
										if (null != detailData)
										{
											if (!GameManager.ClientMgr.AddBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, null, bhid, zhanMengZiJin))
											{
												LogManager.WriteLog(LogTypes.SQL, string.Format("跨服掠夺返还战盟竞价资金失败,bhid={0}, bidMoney={1}", bhid, zhanMengZiJin), null, true);
											}
											else
											{
												Global.UseMailGivePlayerAward3(detailData.BZRoleID, null, GLang.GetLang(3000, new object[0]), string.Format(GLang.GetLang(3001, new object[0]), serverID), 0, 0, 0);
												removeList.Add(bhid);
											}
											GameClient bz = GameManager.ClientMgr.FindClient(detailData.BZRoleID);
											if (null != bz)
											{
												bz.sendCmd<int>(1260, 1, false);
											}
										}
									}
									else
									{
										removeList.Add(data2.BhId);
									}
								}
							}
							foreach (int bhid in removeList)
							{
								this.RuntimeData.JingJiaDict.Remove(bhid);
							}
						}
						if (this.SyncDataCache.KuaFuLueDuoRankInfoDict.Age != SyncData.KuaFuLueDuoRankInfoDict.Age)
						{
							this.RuntimeData.CacheRole2KillDict.Clear();
							this.RuntimeData.CacheBh2LueDuoDict.Clear();
							this.SyncDataCache.KuaFuLueDuoRankInfoDict = SyncData.KuaFuLueDuoRankInfoDict;
							this.RefreshKuaFuLueDuoChampionBH();
						}
					}
				}
			}
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x0006F2F0 File Offset: 0x0006D4F0
		public bool IsGongNengOpenedEnter(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.KuaFuLueDuoEnter, hint);
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x0006F30C File Offset: 0x0006D50C
		private bool IsGongNengOpenedJingJia(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.KuaFuLueDuoJingJia, hint);
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x0006F328 File Offset: 0x0006D528
		public List<KuaFuLueDuoAwardsData> GetClientAwardsDataList(GameClient client, KuaFuLueDuoConfig sceneInfo)
		{
			List<KuaFuLueDuoAwardsData> awardsDataList = new List<KuaFuLueDuoAwardsData>();
			List<int> args = Global.GetRoleParamsIntListFromDBOffline(client, "47");
			if (this.ValidateAwardsInfo(args))
			{
				awardsDataList.Add(new KuaFuLueDuoAwardsData
				{
					type = 1,
					ZiYuan = args[1],
					JiFen = args[2],
					Exp = (long)args[3] * sceneInfo.Exp,
					BindJinBi = args[4],
					JueXing = args[5]
				});
			}
			List<int> args2 = Global.GetRoleParamsIntListFromDBOffline(client, "48");
			if (this.ValidateAwardsInfo(args2))
			{
				awardsDataList.Add(new KuaFuLueDuoAwardsData
				{
					type = 2,
					ZiYuan = args2[1],
					JiFen = args2[2],
					Exp = (long)args2[3] * sceneInfo.Exp,
					BindJinBi = args2[4],
					JueXing = args2[5]
				});
			}
			return awardsDataList;
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x0006F43C File Offset: 0x0006D63C
		private void NtfCanGetAward(GameClient client, KuaFuLueDuoConfig sceneInfo)
		{
			List<KuaFuLueDuoAwardsData> awardsDataList = this.GetClientAwardsDataList(client, sceneInfo);
			client.sendCmd<List<KuaFuLueDuoAwardsData>>(1254, awardsDataList, false);
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x0006F464 File Offset: 0x0006D664
		private bool ValidateAwardsInfo(List<int> args)
		{
			if (args != null && args.Count >= 8 && args[3] > 0)
			{
				if (args[6] == this.SyncDataCache.SeasonID || (args[6] == this.SyncDataCache.LastSeasonID && this.SyncDataCache.GameState == 0))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x0006F4E4 File Offset: 0x0006D6E4
		private int GiveRoleAwards(GameClient client, KuaFuLueDuoConfig sceneInfo)
		{
			List<KuaFuLueDuoAwardsData> awardsDataList = new List<KuaFuLueDuoAwardsData>();
			List<int> args = Global.GetRoleParamsIntListFromDBOffline(client, "47");
			if (this.ValidateAwardsInfo(args))
			{
				awardsDataList.Add(new KuaFuLueDuoAwardsData
				{
					Exp = (long)args[3] * sceneInfo.Exp,
					BindJinBi = args[4],
					JueXing = args[5]
				});
			}
			List<int> args2 = Global.GetRoleParamsIntListFromDBOffline(client, "48");
			if (this.ValidateAwardsInfo(args2))
			{
				awardsDataList.Add(new KuaFuLueDuoAwardsData
				{
					Exp = (long)args2[3] * sceneInfo.Exp,
					BindJinBi = args2[4],
					JueXing = args2[5]
				});
			}
			foreach (KuaFuLueDuoAwardsData data in awardsDataList)
			{
				if (data.Exp > 0L)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, data.Exp, true, true, false, "领取跨服掠夺奖励");
				}
				if (data.BindJinBi > 0)
				{
					GameManager.ClientMgr.AddMoney1(client, data.BindJinBi, "领取跨服掠夺奖励", true);
				}
				if (data.JueXing > 0)
				{
					GameManager.ClientMgr.ModifyJueXingValue(client, data.JueXing, "领取跨服掠夺奖励", false);
				}
			}
			if (this.ValidateAwardsInfo(args))
			{
				args[3] = 0;
				Global.SaveRoleParamsIntListToDBOffline(client.ClientData.RoleID, args, "47", client.ServerId);
			}
			if (this.ValidateAwardsInfo(args2))
			{
				args2[3] = 0;
				Global.SaveRoleParamsIntListToDBOffline(client.ClientData.RoleID, args2, "48", client.ServerId);
			}
			return 1;
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x0006F6F8 File Offset: 0x0006D8F8
		public void GiveAwards(KuaFuLueDuoScene scene)
		{
			try
			{
				foreach (KuaFuLueDuoClientContextData contextData in scene.ClientContextDataDict.Values)
				{
					if (0 != contextData.BattleWhichSide)
					{
						int jiFen = contextData.TotalScore;
						int totalJiFen = contextData.BangHuiContextData.TotalScore + 1;
						if (contextData.BattleWhichSide == 1)
						{
							int ziyuan = scene.LeftZiYuan;
							contextData.AwardJueXing = (int)Math.Min((double)ziyuan * this.RuntimeData.CrusadeAwardDefender[0] * (double)jiFen / (double)totalJiFen, this.RuntimeData.CrusadeAwardDefender[1]);
						}
						else
						{
							int ziyuan = contextData.BangHuiContextData.ZiYuan;
							contextData.AwardJueXing = (int)Math.Min((double)ziyuan * this.RuntimeData.CrusadeAwardAttacker[0] * (double)jiFen / (double)totalJiFen, this.RuntimeData.CrusadeAwardAttacker[1]);
						}
						contextData.BangHuiContextData.TotalAwardJueXing += contextData.AwardJueXing;
						contextData.BangHuiContextData.TotalRoleNum++;
					}
				}
				foreach (KuaFuLueDuoBangHuiContextData contextData2 in scene.BangHuiContextDataDict.Values)
				{
					contextData2.TotalRoleNum = Math.Max(contextData2.TotalRoleNum, 1);
					if (contextData2.ServerId == scene.ThisFuBenData.DestServerId)
					{
						contextData2.ZiYuan = scene.LeftZiYuan;
					}
					contextData2.BaoDi = contextData2.ZiYuan - contextData2.TotalAwardJueXing;
				}
				foreach (KuaFuLueDuoClientContextData contextData in scene.ClientContextDataDict.Values)
				{
					if (0 != contextData.BattleWhichSide)
					{
						List<int> args = new List<int>();
						string paramName;
						if (contextData.BattleWhichSide == 1)
						{
							contextData.AwardJueXing += (int)Math.Max((double)(contextData.BangHuiContextData.BaoDi / contextData.BangHuiContextData.TotalRoleNum), this.RuntimeData.CrusadeAwardDefender[2]);
							contextData.AwardJueXing = (int)Math.Min((double)contextData.AwardJueXing, this.RuntimeData.CrusadeAwardDefender[3]) * scene.SceneInfo.JueXingNum;
							paramName = "47";
							args.Add(1);
							args.Add(scene.LeftZiYuan);
							args.Add(contextData.TotalScore);
							args.Add(contextData.AwardExpLevel);
							args.Add(contextData.AwardBindJinBi);
							args.Add(contextData.AwardJueXing);
							args.Add(this.SyncDataCache.SeasonID);
							args.Add(contextData.KillNum);
						}
						else
						{
							contextData.AwardJueXing += (int)Math.Max((double)(contextData.BangHuiContextData.BaoDi / contextData.BangHuiContextData.TotalRoleNum), this.RuntimeData.CrusadeAwardAttacker[2]);
							contextData.AwardJueXing = (int)Math.Min((double)contextData.AwardJueXing, this.RuntimeData.CrusadeAwardAttacker[3]) * scene.SceneInfo.JueXingNum;
							paramName = "48";
							args.Add(2);
							args.Add(contextData.ZiYuan);
							args.Add(contextData.TotalScore);
							args.Add(contextData.AwardExpLevel);
							args.Add(contextData.AwardBindJinBi);
							args.Add(contextData.AwardJueXing);
							args.Add(this.SyncDataCache.SeasonID);
							args.Add(contextData.KillNum);
						}
						GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleId);
						Global.SaveRoleParamsIntListToDBOffline(contextData.RoleId, args, paramName, contextData.ServerId);
						if (client != null && client.SceneObject == scene)
						{
							this.NtfCanGetAward(client, scene.SceneInfo);
						}
					}
				}
				this.PushGameResultData(scene);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "跨服掠夺系统清场调度异常");
			}
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x0006FBD0 File Offset: 0x0006DDD0
		public void PushGameResultData(KuaFuLueDuoScene scene)
		{
			KuaFuLueDuoFuBenData fuBenData;
			if (this.RuntimeData.FuBenItemData.TryGetValue((long)scene.GameId, out fuBenData))
			{
				foreach (KuaFuLueDuoClientContextData contextData in scene.ClientContextDataDict.Values)
				{
					if (0 != contextData.BattleWhichSide)
					{
						KuaFuLueDuoRoleData roleData = new KuaFuLueDuoRoleData();
						roleData.rid = contextData.RoleId;
						roleData.rname = contextData.RoleName;
						roleData.zoneid = contextData.ZoneID;
						roleData.kill = contextData.TotalKill;
						if (roleData.kill > 0)
						{
							scene.GameStatisticalData.roleStatisticalData.Add(roleData);
						}
					}
				}
				HuanYingSiYuanClient.getInstance().GameFuBenComplete_KuaFuLueDuo(scene.GameStatisticalData);
			}
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x0006FCCC File Offset: 0x0006DECC
		private void UpdateChengHaoBuffer(GameClient client, bool notify = true)
		{
			if (this.RuntimeData.ChengHaoBHid > 0L && (long)client.ClientData.Faction == this.RuntimeData.ChengHaoBHid)
			{
				double[] bufferParams = new double[]
				{
					1.0
				};
				bool flag = 0 == 0;
				Global.UpdateBufferData(client, BufferItemTypes.KuaFuLueDuo_1_2, bufferParams, 1, notify);
			}
			else
			{
				double[] array = new double[1];
				double[] bufferParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.KuaFuLueDuo_1_2, bufferParams, 1, notify);
			}
			if (this.RuntimeData.ChengHaoBHid_Week > 0L && (long)client.ClientData.Faction == this.RuntimeData.ChengHaoBHid_Week)
			{
				double[] bufferParams = new double[]
				{
					1.0
				};
				Global.UpdateBufferData(client, BufferItemTypes.KuaFuLueDuo_2_1, bufferParams, 1, notify);
			}
			else
			{
				double[] array = new double[1];
				double[] bufferParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.KuaFuLueDuo_2_1, bufferParams, 1, notify);
			}
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x0006FDD0 File Offset: 0x0006DFD0
		public bool RefreshKuaFuLueDuoChampionBH()
		{
			bool chgChampion = false;
			int newChengHaoBHid_Week = 0;
			int newChengHaoBHid = 0;
			if (null != this.SyncDataCache.KuaFuLueDuoRankInfoDict)
			{
				List<KuaFuLueDuoRankInfo> list;
				if (this.SyncDataCache.KuaFuLueDuoRankInfoDict.ListDict != null && this.SyncDataCache.KuaFuLueDuoRankInfoDict.ListDict.TryGetValue(2, out list))
				{
					if (list != null && list.Count > 0)
					{
						newChengHaoBHid = list[0].Key;
					}
				}
				KuaFuLueDuoRankInfo last;
				if (this.SyncDataCache.KuaFuLueDuoRankInfoDict.LastInfoDict != null && this.SyncDataCache.KuaFuLueDuoRankInfoDict.LastInfoDict.TryGetValue(2, out last))
				{
					if (null != last)
					{
						newChengHaoBHid_Week = last.Key;
					}
				}
			}
			if (this.RuntimeData.ChengHaoBHid_Week != (long)newChengHaoBHid_Week)
			{
				chgChampion = true;
			}
			this.RuntimeData.ChengHaoBHid_Week = (long)newChengHaoBHid_Week;
			if (this.RuntimeData.ChengHaoBHid != (long)newChengHaoBHid)
			{
				chgChampion = true;
			}
			this.RuntimeData.ChengHaoBHid = (long)newChengHaoBHid;
			if (chgChampion)
			{
				int count = GameManager.ClientMgr.GetMaxClientCount();
				for (int i = 0; i < count; i++)
				{
					GameClient client = GameManager.ClientMgr.FindClientByNid(i);
					if (null != client)
					{
						this.UpdateChengHaoBuffer(client, true);
					}
				}
			}
			return chgChampion;
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x0006FF50 File Offset: 0x0006E150
		private bool RefuseChangeBangHui(int bhid)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (this.SyncDataCache.GameState >= 3 && this.SyncDataCache.GameState < 4)
				{
					KuaFuLueDuoBangHuiJingJiaData jjData;
					if (this.RuntimeData.JingJiaDict.TryGetValue(bhid, out jjData) && jjData.ServerId > 0 && jjData.ZiJin > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x00070008 File Offset: 0x0006E208
		public bool PreRemoveBangHui(GameClient client)
		{
			bool result;
			if (this.RefuseChangeBangHui(client.ClientData.Faction))
			{
				GameManager.ClientMgr.NotifyImportantMsg(client, GLang.GetLang(3002, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x00070058 File Offset: 0x0006E258
		public bool OnPreBangHuiRemoveMember(PreBangHuiRemoveMemberEventObject e)
		{
			bool result;
			if (this.RefuseChangeBangHui(e.BHID))
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(3002, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x000700B0 File Offset: 0x0006E2B0
		public bool OnPreBangHuiAddMember(PreBangHuiAddMemberEventObject e)
		{
			bool result;
			if (this.RefuseChangeBangHui(e.BHID))
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(3002, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x00070108 File Offset: 0x0006E308
		private void InitScene(KuaFuLueDuoScene scene, GameClient client)
		{
			int small = 0;
			int big = 0;
			foreach (KuaFuLueDuoMonsterItem item in this.RuntimeData.CollectMonsterDict.Values)
			{
				if (item.Type == 1)
				{
					small = item.ZiYuan;
				}
				if (item.Type == 2)
				{
					big = item.ZiYuan;
				}
				scene.CollectMonsterXml.Add(item.ID, item.Clone() as KuaFuLueDuoMonsterItem);
			}
			int smallZiYuan = (int)Math.Min(this.RuntimeData.CrusadeOrePercent[0] * (double)scene.TotalZiYuan, this.RuntimeData.CrusadeOrePercent[1]);
			if (small > 0)
			{
				scene.SmallZiYuanCount = (smallZiYuan - 1) / small + 1;
			}
			if (big > 0)
			{
				scene.BigZiYuanCount = (scene.TotalZiYuan - scene.SmallZiYuanCount * small - 1) / big + 1;
			}
			foreach (QiZhiConfig item2 in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
			{
				scene.QiZhiItem = (item2.Clone() as QiZhiConfig);
			}
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x0007028C File Offset: 0x0006E48C
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.KuaFuLueDuo)
			{
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
				{
					result = false;
				}
				else
				{
					int fuBenSeqId = copyMap.FuBenSeqID;
					int mapCode = copyMap.MapCode;
					int roleId = client.ClientData.RoleID;
					int bhid = client.ClientData.Faction;
					long gameId = Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime now = TimeUtil.NowDateTime();
					KuaFuLueDuoConfig sceneInfo = this.RuntimeData.CommonConfigData.KuaFuLueDuoConfigDict.Values.FirstOrDefault<KuaFuLueDuoConfig>();
					lock (this.RuntimeData.Mutex)
					{
						KuaFuLueDuoScene scene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqId, out scene))
						{
							KuaFuLueDuoFuBenData fuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData))
							{
								LogManager.WriteLog(LogTypes.Error, "跨服掠夺没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
							}
							scene = new KuaFuLueDuoScene();
							scene.GameId = (int)gameId;
							scene.FuBenSeqId = fuBenSeqId;
							scene.ThisFuBenData = fuBenData;
							scene.SceneInfo = sceneInfo;
							scene.MapGridWidth = gameMap.MapGridWidth;
							scene.MapGridHeight = gameMap.MapGridHeight;
							DateTime startTime = now.Date.Add(this.GetStartTime(sceneInfo.ID));
							scene.StartTimeTicks = startTime.Ticks / 10000L;
							scene.GameStatisticalData.GameId = gameId;
							scene.TotalZiYuan = fuBenData.LeftZiYuan;
							scene.LeftZiYuan = fuBenData.LeftZiYuan;
							scene.BangHuiContextDataDict[0] = new KuaFuLueDuoBangHuiContextData();
							for (int i = 0; i < fuBenData.BhDataList.Count; i++)
							{
								int b = fuBenData.BhDataList[i];
								scene.BangHuiContextDataDict[b] = new KuaFuLueDuoBangHuiContextData
								{
									BhId = b
								};
							}
							this.SceneDict[fuBenSeqId] = scene;
							this.InitScene(scene, client);
						}
						scene.CopyMap = copyMap;
						KuaFuLueDuoClientContextData clientContextData;
						if (!scene.ClientContextDataDict.TryGetValue(roleId, out clientContextData))
						{
							clientContextData = new KuaFuLueDuoClientContextData
							{
								RoleId = roleId,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide,
								RoleName = client.ClientData.RoleName,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								ZoneID = client.ClientData.ZoneID,
								AwardExpLevel = (int)Global.GetExpMultiByZhuanShengExpXiShu(client, 1L),
								AwardBindJinBi = sceneInfo.BandJinBi
							};
							scene.ClientContextDataDict[roleId] = clientContextData;
						}
						KuaFuLueDuoBangHuiContextData bangHuiContextData;
						if (scene.BangHuiContextDataDict.TryGetValue(bhid, out bangHuiContextData))
						{
							clientContextData.BangHuiContextData = bangHuiContextData;
							if (bangHuiContextData.ZoneID == 0)
							{
								bangHuiContextData.ServerId = client.ServerId;
								if (bhid > 0)
								{
									BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(bhid, client.ServerId);
									bangHuiContextData.BhName = bangHuiMiniData.BHName;
									bangHuiContextData.ZoneID = bangHuiMiniData.ZoneID;
								}
								else
								{
									bangHuiContextData.ZoneID = client.ServerId;
								}
							}
						}
						else if (scene.BangHuiContextDataDict.TryGetValue(0, out bangHuiContextData))
						{
							clientContextData.BangHuiContextData = bangHuiContextData;
							bangHuiContextData.ServerId = client.ServerId;
						}
						clientContextData.Kill = 0;
						client.SceneObject = scene;
						client.SceneGameId = (long)scene.GameId;
						client.SceneContextData2 = clientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(scene.SceneInfo.TotalSecs * 1000));
					}
					GameManager.ClientMgr.ModifyKuaFuLueDuoEnterNum(client, -1, "进入跨服掠夺战场", false);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x000706F0 File Offset: 0x0006E8F0
		private int CheckCondition(GameClient client, ref KuaFuLueDuoConfig sceneItem, ref KuaFuLueDuoGameStates state, ref int signUpRound)
		{
			int result = 0;
			sceneItem = null;
			state = KuaFuLueDuoGameStates.None;
			lock (this.RuntimeData.Mutex)
			{
				sceneItem = this.RuntimeData.CommonConfigData.GetKuaFuLueDuoConfig(0);
				if (null == sceneItem)
				{
					return -4007;
				}
			}
			TimeSpan nowTs = TimeUtil.GetTimeOfWeekNow2();
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < sceneItem.ApplyTimePoints.Count - 1; i++)
				{
					TimeSpan ts = nowTs - sceneItem.ApplyTimePoints[i];
					TimeSpan ts2 = nowTs - sceneItem.ApplyTimePoints[i + 1];
					if (ts.TotalSeconds >= 0.0 && ts2.TotalSeconds < 0.0)
					{
						signUpRound = i + 1;
						state = KuaFuLueDuoGameStates.SignUp;
						break;
					}
				}
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					TimeSpan ts = nowTs - sceneItem.TimePoints[i];
					if (ts.TotalSeconds >= 0.0 && ts.TotalSeconds < (double)sceneItem.GameSecs)
					{
						state = KuaFuLueDuoGameStates.Start;
						break;
					}
				}
			}
			return result;
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x000708D8 File Offset: 0x0006EAD8
		private TimeSpan GetStartTime(int sceneId)
		{
			KuaFuLueDuoConfig sceneItem = null;
			TimeSpan nowTs = TimeUtil.GetTimeOfWeekNow2();
			DateTime weekStartTime = TimeUtil.GetWeekStartTimeNow();
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.CommonConfigData.KuaFuLueDuoConfigDict.TryGetValue(sceneId, out sceneItem))
				{
					for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
					{
						TimeSpan ts = sceneItem.TimePoints[i];
						if (nowTs >= ts)
						{
							return weekStartTime.Add(ts).TimeOfDay;
						}
					}
				}
			}
			return TimeUtil.NowDateTime().TimeOfDay;
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x000709C4 File Offset: 0x0006EBC4
		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			int result;
			if (client.ClientData.BattleWhichSide == 1)
			{
				result = -201;
			}
			else
			{
				KuaFuLueDuoMonsterItem item = monster.Tag as KuaFuLueDuoMonsterItem;
				if (null != item)
				{
					result = item.GatherTime;
				}
				else
				{
					result = -4;
				}
			}
			return result;
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00070A14 File Offset: 0x0006EC14
		public void GMCaiJi(GameClient client, int ziYuan)
		{
			int addScore = ziYuan * 2;
			KuaFuLueDuoScene scene;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					return;
				}
				if (scene.m_eStatus != GameSceneStatuses.STATUS_BEGIN)
				{
					return;
				}
				KuaFuLueDuoClientContextData contextData = client.SceneContextData2 as KuaFuLueDuoClientContextData;
				if (null != contextData)
				{
					contextData.TotalScore += addScore;
					contextData.ZiYuan += ziYuan;
					contextData.BangHuiContextData.TotalScore += addScore;
					contextData.BangHuiContextData.ZiYuan += ziYuan;
					scene.LeftZiYuan = Math.Max(0, scene.LeftZiYuan - ziYuan);
				}
				this.SceneInfoChangeRole(scene, client, 0);
			}
			if (addScore > 0)
			{
				this.NotifyTimeStateInfoAndScoreInfo(scene, false, true);
			}
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x00070B34 File Offset: 0x0006ED34
		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			int addScore = 0;
			KuaFuLueDuoScene scene;
			lock (this.RuntimeData.Mutex)
			{
				KuaFuLueDuoMonsterItem monsterItem = monster.Tag as KuaFuLueDuoMonsterItem;
				if (monsterItem == null)
				{
					return;
				}
				if (!this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					return;
				}
				if (scene.m_eStatus != GameSceneStatuses.STATUS_BEGIN)
				{
					return;
				}
				KuaFuLueDuoClientContextData contextData = client.SceneContextData2 as KuaFuLueDuoClientContextData;
				if (null != contextData)
				{
					addScore = monsterItem.JiFen;
					int ziYuan = monsterItem.ZiYuan;
					contextData.TotalScore += addScore;
					contextData.ZiYuan += ziYuan;
					contextData.BangHuiContextData.TotalScore += addScore;
					contextData.BangHuiContextData.ZiYuan += ziYuan;
					scene.LeftZiYuan = Math.Max(0, scene.LeftZiYuan - ziYuan);
					monsterItem.Alive = false;
					monsterItem.FuHuoTicks = TimeUtil.NOW() + (long)(monsterItem.FuHuoTime * 1000);
					monster.Tag = null;
				}
				this.SceneInfoChangeRole(scene, client, 0);
			}
			if (addScore > 0)
			{
				this.NotifyTimeStateInfoAndScoreInfo(scene, false, true);
			}
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x00070CBC File Offset: 0x0006EEBC
		public void InstallJunQi(KuaFuLueDuoScene scene, CopyMap copyMap, GameClient client, QiZhiConfig item)
		{
			GameMap gameMap = GameManager.MapMgr.GetGameMap(scene.SceneInfo.MapCode);
			if (copyMap != null && null != gameMap)
			{
				item.Alive = true;
				item.BattleWhichSide = client.ClientData.BattleWhichSide;
				GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, item.MonsterId, copyMap.CopyMapID, 1, item.PosX / gameMap.MapGridWidth, item.PosY / gameMap.MapGridHeight, 0, 0, SceneUIClasses.KuaFuLueDuo, item, null);
			}
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x00070D50 File Offset: 0x0006EF50
		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			bool isQiZuo = false;
			bool installJunQi = false;
			KuaFuLueDuoScene scene = client.SceneObject as KuaFuLueDuoScene;
			bool result;
			if (null == scene)
			{
				result = isQiZuo;
			}
			else
			{
				CopyMap copyMap = scene.CopyMap;
				lock (this.RuntimeData.Mutex)
				{
					QiZhiConfig item = scene.QiZhiItem;
					if (item != null && item.NPCID == npcExtentionID)
					{
						isQiZuo = true;
						if (item.Alive)
						{
							return isQiZuo;
						}
						if (client.ClientData.BattleWhichSide != item.BattleWhichSide && Math.Abs(TimeUtil.NOW() - item.DeadTicks) < 3000L)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(12, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (Math.Abs(client.ClientData.PosX - item.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - item.PosY) <= 1000)
						{
							installJunQi = true;
						}
					}
					if (installJunQi)
					{
						this.InstallJunQi(scene, copyMap, client, item);
					}
				}
				result = isQiZuo;
			}
			return result;
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x00070F00 File Offset: 0x0006F100
		public void OnProcessJunQiDead(GameClient client, Monster monster)
		{
			QiZhiConfig qizhiConfig = monster.Tag as QiZhiConfig;
			if (null != qizhiConfig)
			{
				lock (this.RuntimeData.Mutex)
				{
					qizhiConfig.KillerBhid = (long)client.ClientData.Faction;
					qizhiConfig.InstallBhName = "";
					qizhiConfig.InstallBhid = 0L;
					qizhiConfig.DeadTicks = TimeUtil.NOW();
					qizhiConfig.Alive = false;
				}
			}
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x00070F98 File Offset: 0x0006F198
		public bool ClientRelive(GameClient client)
		{
			bool result;
			if (!GameManager.ClientMgr.ModifyKuaFuLueDuoEnterNum(client, -1, "跨服掠夺复活", false))
			{
				KuaFuManager.getInstance().GotoLastMap(client);
				result = true;
			}
			else
			{
				int mapCode = client.ClientData.MapCode;
				KuaFuLueDuoConfig sceneInfo = client.SceneInfoObject as KuaFuLueDuoConfig;
				if (null != sceneInfo)
				{
					int toMapCode;
					int toPosX;
					int toPosY;
					if (this.GetZhanMengBirthPoint(sceneInfo, client, toMapCode = sceneInfo.MapCode, out toMapCode, out toPosX, out toPosY, false))
					{
						client.ClientData.CurrentLifeV = client.ClientData.LifeV;
						client.ClientData.CurrentMagicV = client.ClientData.MagicV;
						client.ClientData.MoveAndActionNum = 0;
						if (toMapCode != client.ClientData.MapCode)
						{
							GameManager.ClientMgr.NotifyMySelfRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, -1);
							client.ClientData.KuaFuChangeMapCode = toMapCode;
							GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, toPosX, toPosY, -1, 1);
						}
						else
						{
							Global.ClientRealive(client, toPosX, toPosY, -1);
						}
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x00071104 File Offset: 0x0006F304
		private void ProcessEnd(KuaFuLueDuoScene scene, long nowTicks)
		{
			Dictionary<int, int> lueDuoDict = new Dictionary<int, int>();
			foreach (KuaFuLueDuoBangHuiContextData item in scene.BangHuiContextDataDict.Values)
			{
				if (item.ServerId != scene.ThisFuBenData.DestServerId)
				{
					int ziYuan;
					lueDuoDict.TryGetValue(item.ServerId, out ziYuan);
					lueDuoDict[item.ServerId] = ziYuan + item.ZiYuan;
					scene.GameStatisticalData.LueDuoResultList.Add(new KuaFuLueDuoLueDuoResultData
					{
						bhid = item.BhId,
						bhname = item.BhName,
						zoneid = item.ZoneID,
						ziyuan = item.ZiYuan
					});
				}
			}
			foreach (KeyValuePair<int, int> kv in lueDuoDict)
			{
				if ((double)kv.Value >= (double)scene.TotalZiYuan * this.RuntimeData.CrusadePerfect)
				{
					scene.GameStatisticalData.SuccessServerID = kv.Key;
					scene.GameStatisticalData.Percent = kv.Value * 100 / scene.TotalZiYuan;
					break;
				}
			}
			scene.GameStatisticalData.DestServerID = scene.ThisFuBenData.DestServerId;
			scene.GameStatisticalData.LeftZiYuan = scene.LeftZiYuan;
			scene.m_eStatus = GameSceneStatuses.STATUS_END;
			scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
			scene.StateTimeData.GameType = 25;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x00071318 File Offset: 0x0006F518
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= KuaFuLueDuoManager.NextHeartBeatTicks)
			{
				KuaFuLueDuoManager.NextHeartBeatTicks = nowTicks + 1020L;
				List<KuaFuLueDuoScene> removeList = new List<KuaFuLueDuoScene>();
				lock (this.RuntimeData.Mutex)
				{
					foreach (KuaFuLueDuoScene scene in this.SceneDict.Values)
					{
						int nID = scene.FuBenSeqId;
						if (nID >= 0)
						{
							if (TimeUtil.NOW() - scene.StartTimeTicks > 86400000L)
							{
								removeList.Add(scene);
							}
							DateTime now = TimeUtil.NowDateTime();
							long ticks = TimeUtil.NOW();
							CopyMap copyMap = scene.CopyMap;
							if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
							{
								if (ticks >= scene.StartTimeTicks)
								{
									scene.m_lPrepareTime = scene.StartTimeTicks;
									scene.m_lBeginTime = scene.m_lPrepareTime + (long)(scene.SceneInfo.PrepareSecs * 1000);
									scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
									scene.StateTimeData.GameType = 25;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, copyMap);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								if (ticks >= scene.m_lBeginTime)
								{
									scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.m_lEndTime = scene.m_lBeginTime + (long)(scene.SceneInfo.FightingSecs * 1000);
									scene.StateTimeData.GameType = 25;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, copyMap);
									for (int guangMuId = 1; guangMuId <= 4; guangMuId++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, guangMuId, 0);
									}
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= scene.m_lEndTime || scene.LeftZiYuan <= 0)
								{
									this.ProcessEnd(scene, ticks);
								}
								else
								{
									this.CheckSceneScoreTime(scene, ticks);
									this.CheckCreateDynamicMonster(scene, ticks);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
								this.GiveAwards(scene);
								GameManager.CopyMapMgr.KillAllMonster(copyMap);
								KuaFuLueDuoFuBenData fuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue((long)scene.GameId, out fuBenData))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("跨服掠夺跨服副本GameID={0},战斗结束", fuBenData.GameId), null, true);
									this.RuntimeData.FuBenItemData.Remove((long)scene.GameId);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (ticks >= scene.m_lLeaveTime)
								{
									scene.ThisFuBenData.State = 3;
									copyMap.SetRemoveTicks(scene.m_lLeaveTime);
									scene.m_eStatus = GameSceneStatuses.STATUS_CLEAR;
									removeList.Add(scene);
									try
									{
										List<GameClient> objsList = copyMap.GetClientsList();
										if (objsList != null && objsList.Count > 0)
										{
											for (int i = 0; i < objsList.Count; i++)
											{
												GameClient c = objsList[i];
												if (c != null)
												{
													KuaFuManager.getInstance().GotoLastMap(c);
												}
											}
										}
									}
									catch (Exception ex)
									{
										DataHelper.WriteExceptionLogEx(ex, "跨服掠夺系统清场调度异常");
									}
								}
							}
						}
					}
				}
				if (removeList.Count > 0)
				{
					lock (this.RuntimeData.Mutex)
					{
						foreach (KuaFuLueDuoScene scene in removeList)
						{
							KuaFuLueDuoScene item;
							this.SceneDict.TryRemove(scene.FuBenSeqId, out item);
						}
					}
				}
			}
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x00071864 File Offset: 0x0006FA64
		public void CheckCreateDynamicMonster(KuaFuLueDuoScene scene, long nowMs)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
				{
					foreach (KuaFuLueDuoMonsterItem monsterInfo in scene.CollectMonsterXml.Values)
					{
						try
						{
							if (!monsterInfo.Alive && monsterInfo.FuHuoTicks <= nowMs)
							{
								if (monsterInfo.Type == 1 && scene.SmallZiYuanCount > 0)
								{
									scene.SmallZiYuanCount--;
								}
								else
								{
									if (monsterInfo.Type != 2 || scene.BigZiYuanCount <= 0)
									{
										continue;
									}
									scene.BigZiYuanCount--;
								}
								monsterInfo.Alive = true;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.CopyMap.MapCode, monsterInfo.MonsterID, scene.CopyMap.CopyMapID, 1, monsterInfo.X / scene.MapGridWidth, monsterInfo.Y / scene.MapGridHeight, 0, 0, SceneUIClasses.KuaFuLueDuo, monsterInfo, null);
							}
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
					}
				}
			}
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x00071A30 File Offset: 0x0006FC30
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				KuaFuLueDuoScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					if (sideScore)
					{
						KuaFuLueDuoScoreData data = new KuaFuLueDuoScoreData();
						data.LeftZiYuan = scene.LeftZiYuan;
						KuaFuLueDuoClientContextData contextData = client.SceneContextData2 as KuaFuLueDuoClientContextData;
						if (null != contextData)
						{
							data.LueDuoZiYuan = contextData.BangHuiContextData.ZiYuan;
							data.SelfScore = contextData.TotalScore;
							data.LeftNum = contextData.LeftNum;
							client.sendCmd<KuaFuLueDuoScoreData>(1253, data, false);
						}
					}
				}
			}
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x00071B30 File Offset: 0x0006FD30
		public void NotifyTimeStateInfoAndScoreInfo(KuaFuLueDuoScene scene, bool timeState = true, bool sideScore = true)
		{
			List<GameClient> list = scene.CopyMap.GetClientsList();
			if (list != null && list.Count > 0)
			{
				KuaFuLueDuoScoreData data = new KuaFuLueDuoScoreData();
				data.LeftZiYuan = scene.LeftZiYuan;
				foreach (GameClient client in list)
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					KuaFuLueDuoClientContextData contextData = client.SceneContextData2 as KuaFuLueDuoClientContextData;
					if (null != contextData)
					{
						data.LueDuoZiYuan = contextData.BangHuiContextData.ZiYuan;
						data.SelfScore = contextData.TotalScore;
						data.LeftNum = contextData.LeftNum;
						client.sendCmd<KuaFuLueDuoScoreData>(1253, data, false);
					}
				}
			}
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x00071C30 File Offset: 0x0006FE30
		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				KuaFuLueDuoScene scene = client.SceneObject as KuaFuLueDuoScene;
				if (scene != null && scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
				{
					int addScore = 0;
					KuaFuLueDuoClientContextData clientLianShaContextData = client.SceneContextData2 as KuaFuLueDuoClientContextData;
					KuaFuLueDuoClientContextData otherLianShaContextData = other.SceneContextData2 as KuaFuLueDuoClientContextData;
					if (clientLianShaContextData != null && otherLianShaContextData != null)
					{
						clientLianShaContextData.KillNum++;
						clientLianShaContextData.Kill++;
						clientLianShaContextData.TotalKill++;
						int s = this.RuntimeData.CrusadeUltraKill[0] + this.RuntimeData.CrusadeUltraKill[1] * clientLianShaContextData.KillNum;
						addScore += Global.Clamp(s, this.RuntimeData.CrusadeUltraKill[2], this.RuntimeData.CrusadeUltraKill[3]);
						if (otherLianShaContextData.KillNum >= 2)
						{
							s = this.RuntimeData.CrusadeShutDown[0] + this.RuntimeData.CrusadeShutDown[1] * otherLianShaContextData.KillNum;
							addScore += Global.Clamp(s, this.RuntimeData.CrusadeShutDown[2], this.RuntimeData.CrusadeShutDown[3]);
						}
						otherLianShaContextData.KillNum = 0;
						clientLianShaContextData.TotalScore += addScore;
						clientLianShaContextData.BangHuiContextData.TotalScore += addScore;
					}
				}
			}
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x00071DE0 File Offset: 0x0006FFE0
		public void LeaveFuBen(GameClient client)
		{
			KuaFuLueDuoScene scene = client.SceneObject as KuaFuLueDuoScene;
			if (null != scene)
			{
				this.SceneInfoChangeRole(scene, client, -1);
			}
			KuaFuLueDuoClientContextData contextData = client.SceneContextData2 as KuaFuLueDuoClientContextData;
			if (contextData.Kill > 0)
			{
				if (contextData.BattleWhichSide == 1)
				{
					GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_KuaFuLueDuo_Defender, new int[]
					{
						contextData.Kill
					}));
				}
				else
				{
					GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_KuaFuLueDuo_Attacker, new int[]
					{
						contextData.Kill
					}));
				}
				contextData.Kill = 0;
			}
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x00071E91 File Offset: 0x00070091
		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x00071E9C File Offset: 0x0007009C
		public void OnStartPlayGame(GameClient client)
		{
			KuaFuLueDuoScene scene = client.SceneObject as KuaFuLueDuoScene;
			if (null != scene)
			{
				this.NotifyTimeStateInfoAndScoreInfo(client, true, true);
				this.SceneInfoChangeRole(scene, client, 0);
			}
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x00071ED4 File Offset: 0x000700D4
		private void SceneInfoChangeRole(KuaFuLueDuoScene scene, GameClient client, int addNum = 0)
		{
			int bhid = client.ClientData.Faction;
			int serverId = scene.ThisFuBenData.DestServerId;
			if (scene != null && scene.CopyMap != null)
			{
				FightInfo sfi = new FightInfo
				{
					RoleNum = addNum
				};
				FightInfo bhfi = new FightInfo
				{
					RoleNum = addNum
				};
				List<GameClient> list = scene.CopyMap.GetClientsList();
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.UpdateZiYuanData = true;
					this.RuntimeData.ServerZiYuanDict[serverId] = sfi;
					this.RuntimeData.BhZiYuanDict[bhid] = bhfi;
					sfi.ZiYuan = scene.LeftZiYuan;
					foreach (GameClient c in list)
					{
						if (c.ServerId == serverId)
						{
							sfi.RoleNum++;
						}
						else if (bhid == c.ClientData.Faction)
						{
							bhfi.RoleNum++;
						}
					}
					foreach (KuaFuLueDuoBangHuiContextData data in scene.BangHuiContextDataDict.Values)
					{
						if (data.BhId == bhid)
						{
							bhfi.ZiYuan = data.ZiYuan;
							break;
						}
					}
				}
			}
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x000720EC File Offset: 0x000702EC
		private void CheckSceneScoreTime(KuaFuLueDuoScene scene, long nowTicks)
		{
			lock (this.RuntimeData.Mutex)
			{
				bool NotifyScoreData = true;
				if (NotifyScoreData)
				{
					this.NotifyTimeStateInfoAndScoreInfo(scene, true, true);
				}
			}
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x0007214C File Offset: 0x0007034C
		private int CalMVPScore(KuaFuLueDuoScene scene, int factor)
		{
			int beginSecs = (int)(TimeUtil.NOW() - scene.m_lBeginTime) / 1000;
			return (int)((1.0 + (double)beginSecs / 60.0 * 0.075) * (double)factor);
		}

		// Token: 0x04000CB1 RID: 3249
		public const SceneUIClasses ManagerType = SceneUIClasses.KuaFuLueDuo;

		// Token: 0x04000CB2 RID: 3250
		private static KuaFuLueDuoManager instance = new KuaFuLueDuoManager();

		// Token: 0x04000CB3 RID: 3251
		public KuaFuLueDuoData RuntimeData = new KuaFuLueDuoData();

		// Token: 0x04000CB4 RID: 3252
		public KuaFuLueDuoSyncData SyncDataCache = new KuaFuLueDuoSyncData();

		// Token: 0x04000CB5 RID: 3253
		public KuaFuLueDuoSyncData RequestSyncData = new KuaFuLueDuoSyncData();

		// Token: 0x04000CB6 RID: 3254
		private RoleDataEx OwnerRoleData = null;

		// Token: 0x04000CB7 RID: 3255
		public ConcurrentDictionary<int, KuaFuLueDuoScene> SceneDict = new ConcurrentDictionary<int, KuaFuLueDuoScene>();

		// Token: 0x04000CB8 RID: 3256
		private static long NextHeartBeatTicks = 0L;
	}
}
