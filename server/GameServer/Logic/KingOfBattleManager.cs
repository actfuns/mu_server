using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace GameServer.Logic
{
	
	public class KingOfBattleManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		
		public static KingOfBattleManager getInstance()
		{
			return KingOfBattleManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KingOfBattleManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1180, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1181, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1183, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1188, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1182, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1190, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1191, 3, 3, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1192, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10001, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10002, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(33, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(27, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(29, 39, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(31, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, KingOfBattleManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10002, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(33, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(27, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(29, 39, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(31, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, KingOfBattleManager.getInstance());
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1180:
				return this.ProcessKingOfBattleJoinCmd(client, nID, bytes, cmdParams);
			case 1181:
				return this.ProcessKingOfBattleEnterCmd(client, nID, bytes, cmdParams);
			case 1182:
				return this.ProcessGetKingOfBattleAwardInfoCmd(client, nID, bytes, cmdParams);
			case 1183:
				return this.ProcessGetKingOfBattleStateCmd(client, nID, bytes, cmdParams);
			case 1188:
				return this.ProcessGetKingOfBattleAwardCmd(client, nID, bytes, cmdParams);
			case 1190:
				return this.ProcessGetKingOfBattleMallDataCmd(client, nID, bytes, cmdParams);
			case 1191:
				return this.ProcessKingOfBattleMallBuyCmd(client, nID, bytes, cmdParams);
			case 1192:
				return this.ProcessKingOfBattleMallRefreshCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 31)
			{
				ClientRegionEventObject e = eventObject as ClientRegionEventObject;
				if (null != e)
				{
					if (e.EventType == 1 && e.Flag == 1)
					{
						this.SubmitCrystalBuff(e.Client, e.AreaLuaID);
					}
				}
			}
			if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEvent = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEvent)
				{
					if (playerDeadEvent.Type == PlayerDeadEventTypes.ByRole)
					{
						this.OnKillRole(playerDeadEvent.getAttackerRole(), playerDeadEvent.getPlayer());
					}
					GameClient clientDead = playerDeadEvent.getPlayer();
					if (null != clientDead)
					{
						KingOfBattleScene scene;
						if (this.SceneDict.TryGetValue(clientDead.ClientData.FuBenSeqID, out scene))
						{
							this.RemoveBattleSceneBuffForRole(scene, clientDead);
						}
					}
				}
			}
			if (eventType == 11)
			{
				MonsterDeadEventObject e2 = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(e2.getAttacker(), e2.getMonster());
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			switch (num)
			{
			case 27:
			{
				ProcessClickOnNpcEventObject e = eventObject as ProcessClickOnNpcEventObject;
				if (null != e)
				{
					if (null != e.Npc)
					{
						int npcId = e.Npc.NpcID;
					}
					if (this.OnSpriteClickOnNpc(e.Client, e.NpcId, e.ExtensionID))
					{
						e.Result = false;
						e.Handled = true;
					}
				}
				break;
			}
			case 28:
			case 31:
			case 32:
				break;
			case 29:
			{
				OnClientChangeMapEventObject e2 = eventObject as OnClientChangeMapEventObject;
				if (null != e2)
				{
					e2.Result = this.ClientChangeMap(e2.Client, e2.TeleportID, ref e2.ToMapCode, ref e2.ToPosX, ref e2.ToPosY);
					e2.Handled = true;
				}
				break;
			}
			case 30:
			{
				OnCreateMonsterEventObject e3 = eventObject as OnCreateMonsterEventObject;
				if (null != e3)
				{
					KingOfBattleQiZhiConfig qiZhiConfig = e3.Monster.Tag as KingOfBattleQiZhiConfig;
					if (null != qiZhiConfig)
					{
						e3.Monster.Camp = qiZhiConfig.BattleWhichSide;
						e3.Result = true;
						e3.Handled = true;
					}
					KingOfBattleDynamicMonsterItem tagInfo = e3.Monster.Tag as KingOfBattleDynamicMonsterItem;
					if (null != tagInfo)
					{
						if (tagInfo.MonsterType == 1)
						{
							e3.Monster.Camp = e3.Monster.MonsterInfo.Camp;
							e3.Result = true;
							e3.Handled = true;
						}
						else if (tagInfo.MonsterType == 2 || tagInfo.MonsterType == 3)
						{
							if (1301 == e3.Monster.MonsterType)
							{
								e3.Monster.Camp = 1;
							}
							else if (1302 == e3.Monster.MonsterType)
							{
								e3.Monster.Camp = 2;
							}
							else if (1303 == e3.Monster.MonsterType)
							{
								e3.Monster.Camp = 3;
							}
							e3.Result = true;
							e3.Handled = true;
						}
					}
				}
				break;
			}
			case 33:
			{
				PreMonsterInjureEventObject obj = eventObject as PreMonsterInjureEventObject;
				if (obj != null && obj.SceneType == 39)
				{
					Monster injureMonster = obj.Monster;
					if (injureMonster != null)
					{
						if (injureMonster.MonsterInfo.ExtensionID == this.RuntimeData.BattleQiZhiMonsterID1 || injureMonster.MonsterInfo.ExtensionID == this.RuntimeData.BattleQiZhiMonsterID2)
						{
							obj.Injure = this.RuntimeData.KingOfBattleDamageJunQi;
							eventObject.Handled = true;
							eventObject.Result = true;
						}
						KingOfBattleDynamicMonsterItem tagInfo = injureMonster.Tag as KingOfBattleDynamicMonsterItem;
						if (tagInfo != null)
						{
							if (tagInfo.MonsterType == 1)
							{
								obj.Injure = this.RuntimeData.KingOfBattleDamageCenter;
								eventObject.Handled = true;
								eventObject.Result = true;
							}
							else if (tagInfo.MonsterType == 2 || tagInfo.MonsterType == 3)
							{
								obj.Injure = this.RuntimeData.KingOfBattleDamageTower;
								eventObject.Handled = true;
								eventObject.Result = true;
							}
						}
					}
				}
				break;
			}
			default:
				switch (num)
				{
				case 10001:
				{
					KuaFuNotifyEnterGameEvent e4 = eventObject as KuaFuNotifyEnterGameEvent;
					if (null != e4)
					{
						KuaFuServerLoginData kuaFuServerLoginData = e4.Arg as KuaFuServerLoginData;
						if (null != kuaFuServerLoginData)
						{
							lock (this.RuntimeData.Mutex)
							{
								this.RuntimeData.RoleIdKuaFuLoginDataDict[kuaFuServerLoginData.RoleId] = kuaFuServerLoginData;
								LogManager.WriteLog(LogTypes.Error, string.Format("通知角色ID={0}拥有进入王者战场资格,跨服GameID={1}", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
							}
						}
						eventObject.Handled = true;
					}
					break;
				}
				case 10002:
				{
					CaiJiEventObject e5 = eventObject as CaiJiEventObject;
					if (null != e5)
					{
						GameClient client = e5.Source as GameClient;
						Monster monster = e5.Target as Monster;
						this.OnCaiJiFinish(client, monster);
						eventObject.Handled = true;
						eventObject.Result = true;
					}
					break;
				}
				}
				break;
			}
		}

		
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.BattleCrystalMonsterDict.Clear();
					fileName = "Config/KingOfBattleCrystalMonster.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						BattleCrystalMonsterItem item = new BattleCrystalMonsterItem();
						item.Id = (int)Global.GetSafeAttributeLong(node, "ID");
						item.MonsterID = (int)Global.GetSafeAttributeLong(node, "MonsterID");
						item.GatherTime = (int)Global.GetSafeAttributeLong(node, "GatherTime");
						item.BattleJiFen = (int)Global.GetSafeAttributeLong(node, "BattleJiFen");
						item.PosX = (int)Global.GetSafeAttributeLong(node, "X");
						item.PosY = (int)Global.GetSafeAttributeLong(node, "Y");
						item.FuHuoTime = (int)Global.GetSafeAttributeLong(node, "FuHuoTime") * 1000;
						item.BuffGoodsID = (int)Global.GetSafeAttributeLong(node, "GoodsID");
						item.BuffTime = (int)Global.GetSafeAttributeLong(node, "Time");
						this.RuntimeData.BattleCrystalMonsterDict[item.Id] = item;
					}
					this.RuntimeData.MapBirthPointDict.Clear();
					fileName = "Config/KingOfBattleRebirth.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						YongZheZhanChangBirthPoint item2 = new YongZheZhanChangBirthPoint();
						item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item2.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						item2.BirthRadius = (int)Global.GetSafeAttributeLong(node, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[item2.ID] = item2;
					}
					this.RuntimeData.SceneDataDict.Clear();
					this.RuntimeData.LevelRangeSceneIdDict.Clear();
					fileName = "Config/KingOfBattle.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						KingOfBattleSceneInfo sceneItem = new KingOfBattleSceneInfo();
						int id = (int)Global.GetSafeAttributeLong(node, "Group");
						int mapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						sceneItem.Id = id;
						sceneItem.MapCode = mapCode;
						sceneItem.MinLevel = (int)Global.GetSafeAttributeLong(node, "MinLevel");
						sceneItem.MaxLevel = (int)Global.GetSafeAttributeLong(node, "MaxLevel");
						sceneItem.MinZhuanSheng = (int)Global.GetSafeAttributeLong(node, "MinZhuanSheng");
						sceneItem.MaxZhuanSheng = (int)Global.GetSafeAttributeLong(node, "MaxZhuanSheng");
						sceneItem.PrepareSecs = (int)Global.GetSafeAttributeLong(node, "PrepareSecs");
						sceneItem.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(node, "WaitingEnterSecs");
						sceneItem.FightingSecs = (int)Global.GetSafeAttributeLong(node, "FightingSecs");
						sceneItem.ClearRolesSecs = (int)Global.GetSafeAttributeLong(node, "ClearRolesSecs");
						ConfigParser.ParseStrInt2(Global.GetSafeAttributeStr(node, "ApplyTime"), ref sceneItem.SignUpStartSecs, ref sceneItem.SignUpEndSecs, ',');
						sceneItem.SignUpStartSecs += sceneItem.SignUpEndSecs;
						if (!ConfigParser.ParserTimeRangeListWithDay(sceneItem.TimePoints, Global.GetSafeAttributeStr(node, "TimePoints"), true, '|', '-', ','))
						{
							success = false;
							LogManager.WriteLog(LogTypes.Fatal, string.Format("读取{0}时间配置(TimePoints)出错", fileName), null, true);
						}
						for (int i = 0; i < sceneItem.TimePoints.Count; i++)
						{
							TimeSpan ts = new TimeSpan(sceneItem.TimePoints[i].Hours, sceneItem.TimePoints[i].Minutes, sceneItem.TimePoints[i].Seconds);
							sceneItem.SecondsOfDay.Add(ts.TotalSeconds);
						}
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
						{
							success = false;
							LogManager.WriteLog(LogTypes.Fatal, string.Format("地图配置中缺少{0}所需的地图:{1}", fileName, mapCode), null, true);
						}
						RangeKey range = new RangeKey(Global.GetUnionLevel(sceneItem.MinZhuanSheng, sceneItem.MinLevel, false), Global.GetUnionLevel(sceneItem.MaxZhuanSheng, sceneItem.MaxLevel, false), null);
						this.RuntimeData.LevelRangeSceneIdDict[range] = sceneItem;
						this.RuntimeData.SceneDataDict[id] = sceneItem;
					}
					fileName = "Config/KingOfBattleAward.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						int id = (int)Global.GetSafeAttributeLong(node, "MapCode");
						KingOfBattleSceneInfo sceneItem;
						if (this.RuntimeData.SceneDataDict.TryGetValue(id, out sceneItem))
						{
							sceneItem.Exp = Global.GetSafeAttributeLong(node, "Exp");
							sceneItem.BandJinBi = (int)Global.GetSafeAttributeLong(node, "BandJinBi");
							sceneItem.AwardMinLevel = (int)Global.GetSafeAttributeLong(node, "MinLevel");
							sceneItem.AwardMaxLevel = (int)Global.GetSafeAttributeLong(node, "MaxLevel");
							sceneItem.AwardMinZhuanSheng = (int)Global.GetSafeAttributeLong(node, "MinZhuanSheng");
							sceneItem.AwardMaxZhuanSheng = (int)Global.GetSafeAttributeLong(node, "MaxZhuanSheng");
							ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "WinGoods"), ref sceneItem.WinAwardsItemList, '|', ',');
							ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(node, "LoseGoods"), ref sceneItem.LoseAwardsItemList, '|', ',');
						}
					}
					this.RuntimeData.SceneDynMonsterDict.Clear();
					fileName = "Config/KingOfBattleMonster.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					HashSet<int> RebornBirthMonsterSet = new HashSet<int>();
					foreach (XElement node in nodes)
					{
						KingOfBattleDynamicMonsterItem item3 = new KingOfBattleDynamicMonsterItem();
						item3.Id = (int)Global.GetSafeAttributeLong(node, "ID");
						item3.MapCode = (int)Global.GetSafeAttributeLong(node, "CodeID");
						item3.MonsterID = (int)Global.GetSafeAttributeLong(node, "MonsterID");
						item3.MonsterType = (int)Global.GetSafeAttributeLong(node, "MonsterType");
						item3.RebornID = (int)Global.GetSafeAttributeLong(node, "RebornID");
						item3.PosX = (int)Global.GetSafeAttributeLong(node, "X");
						item3.PosY = (int)Global.GetSafeAttributeLong(node, "Y");
						item3.DelayBirthMs = (int)Global.GetSafeAttributeLong(node, "Time") * 1000;
						item3.PursuitRadius = (int)Global.GetSafeAttributeLong(node, "PursuitRadius");
						item3.BuffTime = (int)Global.GetSafeAttributeLong(node, "BuffTime");
						string[] JiFenListFileds = Global.GetSafeAttributeStr(node, "JiFen").Split(new char[]
						{
							'|'
						});
						if (JiFenListFileds.Length == 2)
						{
							item3.JiFenDamage = Global.SafeConvertToInt32(JiFenListFileds[0]);
							item3.JiFenKill = Global.SafeConvertToInt32(JiFenListFileds[1]);
						}
						string[] BuffListFileds = Global.GetSafeAttributeStr(node, "Buff").Split(new char[]
						{
							'|'
						});
						for (int j = 0; j < BuffListFileds.Length; j++)
						{
							string[] BuffFiled = BuffListFileds[j].Split(new char[]
							{
								','
							});
							if (BuffFiled.Length == 2)
							{
								KingOfBattleRandomBuff buff = new KingOfBattleRandomBuff();
								buff.GoodsID = Global.SafeConvertToInt32(BuffFiled[0]);
								buff.Pct = Global.SafeConvertToDouble(BuffFiled[1]);
								item3.RandomBuffList.Add(buff);
							}
						}
						List<KingOfBattleDynamicMonsterItem> itemList = null;
						if (!this.RuntimeData.SceneDynMonsterDict.TryGetValue(item3.MapCode, out itemList))
						{
							itemList = new List<KingOfBattleDynamicMonsterItem>();
							this.RuntimeData.SceneDynMonsterDict[item3.MapCode] = itemList;
						}
						this.RuntimeData.DynMonsterDict[item3.Id] = item3;
						if (item3.RebornID != -1)
						{
							RebornBirthMonsterSet.Add(item3.RebornID);
						}
						itemList.Add(item3);
					}
					foreach (KeyValuePair<int, KingOfBattleDynamicMonsterItem> kvp in this.RuntimeData.DynMonsterDict)
					{
						if (RebornBirthMonsterSet.Contains(kvp.Value.Id))
						{
							kvp.Value.RebornBirth = true;
						}
					}
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					fileName = "Config/KingOfBattleQiZuo.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						KingOfBattleQiZhiConfig item4 = new KingOfBattleQiZhiConfig();
						item4.NPCID = (int)Global.GetSafeAttributeLong(node, "NPCID");
						item4.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item4.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						item4.QiZhiMonsterID = (int)Global.GetSafeAttributeLong(node, "Monster");
						this.RuntimeData.NPCID2QiZhiConfigDict[item4.NPCID] = item4;
					}
					this.RuntimeData.KingOfBattleStoreDict.Clear();
					this.RuntimeData.KingOfBattleStoreList.Clear();
					fileName = "Config/KingOfBattleStore.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						KingOfBattleStoreConfig item5 = new KingOfBattleStoreConfig();
						item5.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item5.SaleData = Global.ParseGoodsFromStr_7(Global.GetSafeAttributeStr(node, "GoodsID").Split(new char[]
						{
							','
						}), 0);
						item5.JiFen = (int)Global.GetSafeAttributeLong(node, "WangZheJiFen");
						item5.SinglePurchase = (int)Global.GetSafeAttributeLong(node, "SinglePurchase");
						item5.BeginNum = (int)Global.GetSafeAttributeLong(node, "BeginNum");
						item5.EndNum = (int)Global.GetSafeAttributeLong(node, "EndNum");
						item5.RandNumMinus = item5.EndNum - item5.BeginNum + 1;
						this.RuntimeData.KingOfBattleStoreDict[item5.ID] = item5;
						this.RuntimeData.KingOfBattleStoreList.Add(item5);
					}
					int[] intArray = GameManager.systemParamsList.GetParamValueIntArrayByName("KingOfBattleAttackBuild", ',');
					if (intArray.Length == 3)
					{
						this.RuntimeData.KingOfBattleDamageJunQi = intArray[0];
						this.RuntimeData.KingOfBattleDamageTower = intArray[1];
						this.RuntimeData.KingOfBattleDamageCenter = intArray[2];
					}
					this.RuntimeData.KingOfBattleDie = (int)GameManager.systemParamsList.GetParamValueIntByName("KingOfBattleDie", -1);
					intArray = GameManager.systemParamsList.GetParamValueIntArrayByName("KingOfBattleUltraKill", ',');
					if (intArray.Length == 4)
					{
						this.RuntimeData.KingOfBattleUltraKillParam1 = intArray[0];
						this.RuntimeData.KingOfBattleUltraKillParam2 = intArray[1];
						this.RuntimeData.KingOfBattleUltraKillParam3 = intArray[2];
						this.RuntimeData.KingOfBattleUltraKillParam4 = intArray[3];
					}
					intArray = GameManager.systemParamsList.GetParamValueIntArrayByName("KingOfBattleShutDown", ',');
					if (intArray.Length == 4)
					{
						this.RuntimeData.KingOfBattleShutDownParam1 = intArray[0];
						this.RuntimeData.KingOfBattleShutDownParam2 = intArray[1];
						this.RuntimeData.KingOfBattleShutDownParam3 = intArray[2];
						this.RuntimeData.KingOfBattleShutDownParam4 = intArray[3];
					}
					this.RuntimeData.KingOfBattleLowestJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("KingOfBattleLowestJiFen", -1);
					intArray = GameManager.systemParamsList.GetParamValueIntArrayByName("KingOfBattleStore", ',');
					if (intArray.Length == 3)
					{
						this.RuntimeData.KingOfBattleStoreRefreshTm = intArray[0];
						this.RuntimeData.KingOfBattleStoreRefreshNum = intArray[1];
						this.RuntimeData.KingOfBattleStoreRefreshCost = intArray[2];
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

		
		private void TimerProc(object sender, EventArgs e)
		{
			bool notifyPrepareGame = false;
			bool notifyEnterGame = false;
			DateTime now = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				bool bInActiveTime = false;
				KingOfBattleSceneInfo sceneItem = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<KingOfBattleSceneInfo>();
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
					{
						double secs = sceneItem.SecondsOfDay[i] - now.TimeOfDay.TotalSeconds;
						bInActiveTime = true;
						if (!this.RuntimeData.PrepareGame)
						{
							if (secs > 0.0 && secs < (double)(sceneItem.SignUpEndSecs / 2))
							{
								LogManager.WriteLog(LogTypes.Error, "报名截止5分钟时间过半,通知跨服中心开始分配所有报名玩家的活动场次", null, true);
								this.RuntimeData.PrepareGame = true;
								notifyPrepareGame = true;
								break;
							}
						}
						else if (secs < 0.0)
						{
							LogManager.WriteLog(LogTypes.Error, "报名截止状态结束,可以通知已分配到场次的玩家进入游戏了", null, true);
							notifyEnterGame = true;
							this.RuntimeData.PrepareGame = false;
							break;
						}
					}
				}
				if (!bInActiveTime)
				{
					if (this.RuntimeData.RoleIdKuaFuLoginDataDict.Count > 0)
					{
						this.RuntimeData.RoleIdKuaFuLoginDataDict.Clear();
					}
					if (this.RuntimeData.RoleId2JoinGroup.Count > 0)
					{
						this.RuntimeData.RoleId2JoinGroup.Clear();
					}
				}
			}
			if (notifyPrepareGame)
			{
				LogManager.WriteLog(LogTypes.Error, "通知跨服中心开始分配所有报名玩家的活动场次", null, true);
				string cmd = string.Format("{0} {1} {2}", "GameState", 2, 15);
				YongZheZhanChangClient.getInstance().ExecuteCommand(cmd);
			}
			if (notifyEnterGame)
			{
				lock (this.RuntimeData.Mutex)
				{
					foreach (KuaFuServerLoginData kuaFuServerLoginData in this.RuntimeData.RoleIdKuaFuLoginDataDict.Values)
					{
						this.RuntimeData.NotifyRoleEnterDict.Add(kuaFuServerLoginData.RoleId, kuaFuServerLoginData);
					}
				}
			}
			List<KuaFuServerLoginData> list = null;
			lock (this.RuntimeData.Mutex)
			{
				int count = this.RuntimeData.NotifyRoleEnterDict.Count;
				if (count > 0)
				{
					list = new List<KuaFuServerLoginData>();
					KuaFuServerLoginData kuaFuServerLoginData = this.RuntimeData.NotifyRoleEnterDict.First<KeyValuePair<int, KuaFuServerLoginData>>().Value;
					foreach (KeyValuePair<int, KuaFuServerLoginData> kv in this.RuntimeData.NotifyRoleEnterDict)
					{
						if (kv.Key % 15 == kuaFuServerLoginData.RoleId % 15)
						{
							list.Add(kv.Value);
						}
					}
					foreach (KuaFuServerLoginData data in list)
					{
						this.RuntimeData.NotifyRoleEnterDict.Remove(data.RoleId);
					}
				}
			}
			if (null != list)
			{
				foreach (KuaFuServerLoginData kuaFuServerLoginData in list)
				{
					GameClient client = GameManager.ClientMgr.FindClient(kuaFuServerLoginData.RoleId);
					if (null != client)
					{
						client.sendCmd<int>(1181, 1, false);
					}
				}
			}
		}

		
		public bool ProcessKingOfBattleJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				if (this.IsGongNengOpened(client, false))
				{
					KingOfBattleSceneInfo sceneItem = null;
					KingOfBattleGameStates state = KingOfBattleGameStates.None;
					if (!this.CheckMap(client))
					{
						result = -21;
					}
					else
					{
						result = this.CheckCondition(client, ref sceneItem, ref state);
					}
					if (state != KingOfBattleGameStates.SignUp)
					{
						result = -2001;
					}
					else if (this.RuntimeData.RoleId2JoinGroup.ContainsKey(client.ClientData.RoleID))
					{
						result = -12;
					}
					if (result >= 0)
					{
						int gropuId = sceneItem.Id;
						result = YongZheZhanChangClient.getInstance().YongZheZhanChangSignUp(client.strUserID, client.ClientData.RoleID, client.ClientData.ZoneID, 15, gropuId, client.ClientData.CombatForce);
						if (result > 0)
						{
							this.RuntimeData.RoleId2JoinGroup[client.ClientData.RoleID] = gropuId;
							client.ClientData.SignUpGameType = 15;
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

		
		private bool CheckMap(GameClient client)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return sceneType == SceneUIClasses.Normal;
		}

		
		private int CheckCondition(GameClient client, ref KingOfBattleSceneInfo sceneItem, ref KingOfBattleGameStates state)
		{
			int result = 0;
			sceneItem = null;
			if (!this.IsGongNengOpened(client, true))
			{
				result = -13;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.LevelRangeSceneIdDict.TryGetValue(new RangeKey(Global.GetUnionLevel(client, false)), out sceneItem))
					{
						return -12;
					}
				}
				result = -2001;
				DateTime now = TimeUtil.NowDateTime();
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
					{
						if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
						{
							if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs)
							{
								state = KingOfBattleGameStates.None;
								result = -2001;
							}
							else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpEndSecs)
							{
								state = KingOfBattleGameStates.SignUp;
								result = 1;
							}
							else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i])
							{
								state = KingOfBattleGameStates.Wait;
								result = 1;
							}
							else if (now.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
							{
								state = KingOfBattleGameStates.Start;
								result = 1;
							}
							else
							{
								state = KingOfBattleGameStates.None;
								result = -2001;
							}
							break;
						}
					}
				}
			}
			return result;
		}

		
		private TimeSpan GetStartTime(int sceneId)
		{
			KingOfBattleSceneInfo sceneItem = null;
			TimeSpan startTime = TimeSpan.MinValue;
			DateTime now = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.SceneDataDict.TryGetValue(sceneId, out sceneItem))
				{
					goto IL_153;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && now.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs && now.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
					{
						startTime = TimeSpan.FromSeconds(sceneItem.SecondsOfDay[i]);
						break;
					}
				}
			}
			IL_153:
			if (startTime < TimeSpan.Zero)
			{
				startTime = now.TimeOfDay;
			}
			return startTime;
		}

		
		public bool ProcessGetKingOfBattleAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int err = 1;
				if (!this.IsGongNengOpened(client, false))
				{
					return false;
				}
				string awardsInfo = Global.GetRoleParamByName(client, "32");
				if (!string.IsNullOrEmpty(awardsInfo))
				{
					int lastGroupId = 0;
					int score = 0;
					int success = 0;
					string mvpInfo = Global.GetRoleParamByName(client, "43");
					int mvprid = 0;
					List<string> mvpParamList = Global.StringToList(mvpInfo, '&');
					if (mvpParamList.Count != 4 && !string.IsNullOrEmpty(mvpInfo))
					{
						byte[] strBytes = Convert.FromBase64String(mvpInfo);
						mvpInfo = new UTF8Encoding().GetString(strBytes);
						mvpParamList = Global.StringToList(mvpInfo, '&');
					}
					ConfigParser.ParseStrInt3(awardsInfo, ref lastGroupId, ref success, ref score, ',');
					List<int> awardsParamList = Global.StringToIntList(awardsInfo, ',');
					lastGroupId = awardsParamList[0];
					bool clear = true;
					if (awardsParamList.Count >= 5 && lastGroupId > 0)
					{
						success = awardsParamList[1];
						score = awardsParamList[2];
						int sideScore = awardsParamList[3];
						int sideScore2 = awardsParamList[4];
						if (mvpParamList.Count >= 4)
						{
							mvprid = Global.SafeConvertToInt32(mvpParamList[0]);
							string mvpname = mvpParamList[1];
							int mvpocc = Global.SafeConvertToInt32(mvpParamList[2]);
							int mvpsex = Global.SafeConvertToInt32(mvpParamList[3]);
						}
						KingOfBattleSceneInfo lastSceneItem = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(lastGroupId, out lastSceneItem))
						{
							err = this.GiveRoleAwards(client, success, score, lastSceneItem);
							if (err < 0)
							{
								clear = false;
							}
							if (client.ClientData.RoleID == mvprid)
							{
								GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_KingOfBattleMVP, new int[0]));
							}
						}
					}
					if (clear)
					{
						Global.SaveRoleParamsStringToDB(client, "32", this.RuntimeData.RoleParamsAwardsDefaultString, true);
						Global.SaveRoleParamsStringToDB(client, "43", "", true);
					}
					client.sendCmd<int>(nID, err, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		
		public bool ProcessGetKingOfBattleAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					return false;
				}
				string awardsInfo = Global.GetRoleParamByName(client, "32");
				if (!string.IsNullOrEmpty(awardsInfo))
				{
					int lastGroupId = 0;
					int score = 0;
					int success = 0;
					string mvpInfo = Global.GetRoleParamByName(client, "43");
					int mvprid = 0;
					string mvpname = "";
					int mvpocc = 0;
					int mvpsex = 0;
					List<string> mvpParamList = Global.StringToList(mvpInfo, '&');
					if (mvpParamList.Count != 4 && !string.IsNullOrEmpty(mvpInfo))
					{
						byte[] strBytes = Convert.FromBase64String(mvpInfo);
						mvpInfo = new UTF8Encoding().GetString(strBytes);
						mvpParamList = Global.StringToList(mvpInfo, '&');
					}
					ConfigParser.ParseStrInt3(awardsInfo, ref lastGroupId, ref success, ref score, ',');
					List<int> awardsParamList = Global.StringToIntList(awardsInfo, ',');
					lastGroupId = awardsParamList[0];
					bool clear = true;
					if (awardsParamList.Count >= 5 && lastGroupId > 0)
					{
						success = awardsParamList[1];
						score = awardsParamList[2];
						int sideScore = awardsParamList[3];
						int sideScore2 = awardsParamList[4];
						if (mvpParamList.Count >= 4)
						{
							mvprid = Global.SafeConvertToInt32(mvpParamList[0]);
							mvpname = mvpParamList[1];
							mvpocc = Global.SafeConvertToInt32(mvpParamList[2]);
							mvpsex = Global.SafeConvertToInt32(mvpParamList[3]);
						}
						KingOfBattleSceneInfo lastSceneItem = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(lastGroupId, out lastSceneItem))
						{
							if (score >= this.RuntimeData.KingOfBattleLowestJiFen)
							{
								clear = false;
							}
							this.NtfCanGetAward(client, success, score, lastSceneItem, sideScore, sideScore2, mvprid, mvpname, mvpocc, mvpsex);
						}
					}
					if (clear)
					{
						Global.SaveRoleParamsStringToDB(client, "32", this.RuntimeData.RoleParamsAwardsDefaultString, true);
						Global.SaveRoleParamsStringToDB(client, "43", "", true);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		
		public bool ProcessGetKingOfBattleStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string awardsInfo = Global.GetRoleParamByName(client, "32");
				if (!string.IsNullOrEmpty(awardsInfo))
				{
					int lastGroupId = 0;
					int score = 0;
					int success = 0;
					ConfigParser.ParseStrInt3(awardsInfo, ref lastGroupId, ref success, ref score, ',');
					if (lastGroupId > 0)
					{
						KingOfBattleSceneInfo lastSceneItem = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(lastGroupId, out lastSceneItem))
						{
							client.sendCmd<int>(nID, 4, false);
							return true;
						}
					}
				}
				KingOfBattleSceneInfo sceneItem = null;
				KingOfBattleGameStates timeState = KingOfBattleGameStates.None;
				int result = 0;
				int groupId = 0;
				this.RuntimeData.RoleId2JoinGroup.TryGetValue(client.ClientData.RoleID, out groupId);
				this.CheckCondition(client, ref sceneItem, ref timeState);
				if (groupId > 0)
				{
					if (timeState >= KingOfBattleGameStates.SignUp && timeState <= KingOfBattleGameStates.Wait)
					{
						int state = YongZheZhanChangClient.getInstance().GetKuaFuRoleState(client.ClientData.RoleID);
						if (state >= 1)
						{
							result = 2;
						}
						else
						{
							result = 5;
						}
					}
					else if (timeState == KingOfBattleGameStates.Start)
					{
						if (this.RuntimeData.RoleIdKuaFuLoginDataDict.ContainsKey(client.ClientData.RoleID))
						{
							result = 3;
						}
					}
				}
				else if (timeState == KingOfBattleGameStates.SignUp)
				{
					result = 1;
				}
				else if (timeState == KingOfBattleGameStates.Wait || timeState == KingOfBattleGameStates.Start)
				{
					result = 5;
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

		
		public bool ProcessKingOfBattleEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, result, false);
					return true;
				}
				KingOfBattleSceneInfo sceneItem = null;
				KingOfBattleGameStates state = KingOfBattleGameStates.None;
				if (!this.CheckMap(client))
				{
					result = -21;
				}
				else
				{
					result = this.CheckCondition(client, ref sceneItem, ref state);
				}
				if (state == KingOfBattleGameStates.Start)
				{
					KuaFuServerLoginData kuaFuServerLoginData = null;
					lock (this.RuntimeData.Mutex)
					{
						if (this.RuntimeData.RoleIdKuaFuLoginDataDict.TryGetValue(client.ClientData.RoleID, out kuaFuServerLoginData))
						{
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
							if (null != clientKuaFuServerLoginData)
							{
								clientKuaFuServerLoginData.RoleId = kuaFuServerLoginData.RoleId;
								clientKuaFuServerLoginData.GameId = kuaFuServerLoginData.GameId;
								clientKuaFuServerLoginData.GameType = kuaFuServerLoginData.GameType;
								clientKuaFuServerLoginData.EndTicks = kuaFuServerLoginData.EndTicks;
								clientKuaFuServerLoginData.ServerId = kuaFuServerLoginData.ServerId;
								clientKuaFuServerLoginData.ServerIp = kuaFuServerLoginData.ServerIp;
								clientKuaFuServerLoginData.ServerPort = kuaFuServerLoginData.ServerPort;
								clientKuaFuServerLoginData.FuBenSeqId = kuaFuServerLoginData.FuBenSeqId;
							}
						}
						else
						{
							result = -11000;
						}
					}
					if (result >= 0)
					{
						result = YongZheZhanChangClient.getInstance().ChangeRoleState(client.ClientData.RoleID, KuaFuRoleStates.EnterGame, false);
						if (result >= 0)
						{
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
						}
					}
				}
				else
				{
					result = -2001;
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

		
		public void RefreshKingOfBattleStoreData(KingOfBattleStoreData KOBattleStoreData, bool SetRefreshTm = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (SetRefreshTm)
				{
					KOBattleStoreData.LastRefTime = TimeUtil.NowDateTime();
				}
				KOBattleStoreData.SaleList.Clear();
				List<KingOfBattleStoreConfig> KOBStoreList = this.RuntimeData.KingOfBattleStoreList;
				int PercentZero = KOBStoreList[0].BeginNum;
				int PercentOne = KOBStoreList[KOBStoreList.Count - 1].EndNum;
				for (int Num = 0; Num < this.RuntimeData.KingOfBattleStoreRefreshNum; Num++)
				{
					int rate = Global.GetRandomNumber(PercentZero, PercentOne);
					for (int i = 0; i < KOBStoreList.Count; i++)
					{
						if (KOBStoreList[i].RandSkip)
						{
							rate += KOBStoreList[i].RandNumMinus;
						}
						if (!KOBStoreList[i].RandSkip && rate >= KOBStoreList[i].BeginNum && rate <= KOBStoreList[i].EndNum)
						{
							KOBStoreList[i].RandSkip = true;
							PercentOne -= KOBStoreList[i].RandNumMinus;
							KingOfBattleStoreSaleData SaleData = new KingOfBattleStoreSaleData();
							SaleData.ID = KOBStoreList[i].ID;
							KOBattleStoreData.SaleList.Add(SaleData);
						}
					}
				}
				for (int i = 0; i < KOBStoreList.Count; i++)
				{
					KOBStoreList[i].RandSkip = false;
				}
			}
		}

		
		public KingOfBattleStoreData GetClientKingOfBattleStoreData(GameClient client)
		{
			KingOfBattleStoreData kobattleStoreData;
			if (null != client.ClientData.KOBattleStoreData)
			{
				kobattleStoreData = client.ClientData.KOBattleStoreData;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					client.ClientData.KOBattleStoreData = new KingOfBattleStoreData();
					client.ClientData.KOBattleStoreData.LastRefTime = Global.GetRoleParamsDateTimeFromDB(client, "10149");
					client.ClientData.KOBattleStoreData.SaleList = new List<KingOfBattleStoreSaleData>();
					List<ushort> StoreSaleDataList = Global.GetRoleParamsUshortListFromDB(client, "33");
					for (int index = 0; index < StoreSaleDataList.Count - 1; index += 2)
					{
						KingOfBattleStoreSaleData SaleData = new KingOfBattleStoreSaleData();
						SaleData.ID = (int)StoreSaleDataList[index];
						SaleData.Purchase = (int)StoreSaleDataList[index + 1];
						client.ClientData.KOBattleStoreData.SaleList.Add(SaleData);
					}
				}
				kobattleStoreData = client.ClientData.KOBattleStoreData;
			}
			return kobattleStoreData;
		}

		
		public void SaveKingOfBattleStoreData(GameClient client)
		{
			if (null != client.ClientData.KOBattleStoreData)
			{
				lock (this.RuntimeData.Mutex)
				{
					KingOfBattleStoreData KOBattleStoreData = client.ClientData.KOBattleStoreData;
					Global.SaveRoleParamsDateTimeToDB(client, "10149", KOBattleStoreData.LastRefTime, true);
					List<ushort> StoreSaleDataList = new List<ushort>();
					foreach (KingOfBattleStoreSaleData item in KOBattleStoreData.SaleList)
					{
						StoreSaleDataList.Add((ushort)item.ID);
						StoreSaleDataList.Add((ushort)item.Purchase);
					}
					Global.SaveRoleParamsUshortListToDB(client, StoreSaleDataList, "33", true);
				}
			}
		}

		
		public bool ProcessGetKingOfBattleMallDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					return false;
				}
				KingOfBattleStoreData KOBattleStoreData = this.GetClientKingOfBattleStoreData(client);
				if ((TimeUtil.NowDateTime() - KOBattleStoreData.LastRefTime).TotalSeconds >= (double)(this.RuntimeData.KingOfBattleStoreRefreshTm * 3600))
				{
					this.RefreshKingOfBattleStoreData(KOBattleStoreData, true);
					this.SaveKingOfBattleStoreData(client);
				}
				client.sendCmd<KingOfBattleStoreData>(nID, KOBattleStoreData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessKingOfBattleMallBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					return false;
				}
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int storeID = Global.SafeConvertToInt32(cmdParams[1]);
				int countNum = Global.SafeConvertToInt32(cmdParams[2]);
				KingOfBattleStoreConfig KOBattleStoreConfig = null;
				string strcmd;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.KingOfBattleStoreDict.TryGetValue(storeID, out KOBattleStoreConfig))
					{
						result = 4;
						strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
				}
				KingOfBattleStoreData KOBattleStoreData = this.GetClientKingOfBattleStoreData(client);
				KingOfBattleStoreSaleData SaleData = null;
				foreach (KingOfBattleStoreSaleData item in KOBattleStoreData.SaleList)
				{
					if (item.ID == storeID)
					{
						SaleData = item;
						break;
					}
				}
				if (null == SaleData)
				{
					result = 3;
					strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (KOBattleStoreConfig.SinglePurchase - SaleData.Purchase < countNum)
				{
					result = 6;
					strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (!Global.CanAddGoods(client, KOBattleStoreConfig.SaleData.GoodsID, KOBattleStoreConfig.SaleData.GCount * countNum, KOBattleStoreConfig.SaleData.Binding, "1900-01-01 12:00:00", true, false))
				{
					result = 6;
					strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				int curKingOfBattlePoint = GameManager.ClientMgr.GetKingOfBattlePointValue(client);
				if (curKingOfBattlePoint < KOBattleStoreConfig.JiFen * countNum)
				{
					result = 1;
					strcmd = string.Format("{0}:{1}:{2}", result, storeID, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				GameManager.ClientMgr.ModifyKingOfBattlePointValue(client, -KOBattleStoreConfig.JiFen * countNum, "王者战场商店", true, true);
				GoodsData goodsData = KOBattleStoreConfig.SaleData;
				Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount * countNum, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, string.Format("王者战场商店", new object[0]), false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
				SaleData.Purchase += countNum;
				this.SaveKingOfBattleStoreData(client);
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

		
		public bool ProcessKingOfBattleMallRefreshCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					return false;
				}
				int result = 0;
				string strcmd;
				if (client.ClientData.UserMoney < this.RuntimeData.KingOfBattleStoreRefreshCost)
				{
					result = 7;
					strcmd = string.Format("{0}", result);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.RuntimeData.KingOfBattleStoreRefreshCost, "王者战场商店刷新", true, true, false, DaiBiSySType.None))
				{
					result = 7;
					strcmd = string.Format("{0}", result);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				KingOfBattleStoreData KOBattleStoreData = this.GetClientKingOfBattleStoreData(client);
				this.RefreshKingOfBattleStoreData(KOBattleStoreData, false);
				this.SaveKingOfBattleStoreData(client);
				client.sendCmd<KingOfBattleStoreData>(1190, KOBattleStoreData, false);
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

		
		public void OnStartPlayGame(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				KingOfBattleScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					client.sendCmd<List<int>>(1189, scene.SceneOpenTeleportList, false);
				}
			}
		}

		
		public bool OnInitGame(GameClient client)
		{
			KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			YongZheZhanChangFuBenData fuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenItemData.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenData))
				{
					fuBenData = null;
				}
				else if (fuBenData.State >= GameFuBenState.End)
				{
					return false;
				}
			}
			if (null == fuBenData)
			{
				YongZheZhanChangFuBenData newFuBenData = YongZheZhanChangClient.getInstance().GetKuaFuFuBenData((int)kuaFuServerLoginData.GameId);
				if (newFuBenData == null || newFuBenData.State == GameFuBenState.End)
				{
					LogManager.WriteLog(LogTypes.Error, ("获取不到有效的副本数据," + newFuBenData == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.FuBenItemData.TryGetValue((int)kuaFuServerLoginData.GameId, out fuBenData))
					{
						fuBenData = newFuBenData;
						fuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.FuBenItemData[fuBenData.GameId] = fuBenData;
					}
				}
			}
			KuaFuFuBenRoleData kuaFuFuBenRoleData;
			bool result;
			if (!fuBenData.RoleDict.TryGetValue(client.ClientData.RoleID, out kuaFuFuBenRoleData))
			{
				result = false;
			}
			else
			{
				client.ClientData.BattleWhichSide = kuaFuFuBenRoleData.Side;
				int posX;
				int posY;
				int side = this.GetBirthPoint(client, out posX, out posY);
				if (side <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, "无法获取有效的阵营和出生点,进入跨服失败,side=" + side, null, true);
					result = false;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						kuaFuServerLoginData.FuBenSeqId = fuBenData.SequenceId;
						KingOfBattleSceneInfo sceneInfo;
						if (!this.RuntimeData.SceneDataDict.TryGetValue(fuBenData.GroupIndex, out sceneInfo))
						{
							return false;
						}
						client.ClientData.MapCode = sceneInfo.MapCode;
					}
					client.ClientData.PosX = posX;
					client.ClientData.PosY = posY;
					client.ClientData.FuBenSeqID = kuaFuServerLoginData.FuBenSeqId;
					result = true;
				}
			}
			return result;
		}

		
		public bool ClientRelive(GameClient client)
		{
			int toPosX;
			int toPosY;
			int side = this.GetBirthPoint(client, out toPosX, out toPosY);
			bool result;
			if (side <= 0)
			{
				result = false;
			}
			else
			{
				client.ClientData.CurrentLifeV = client.ClientData.LifeV;
				client.ClientData.CurrentMagicV = client.ClientData.MagicV;
				client.ClientData.MoveAndActionNum = 0;
				GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, toPosX, toPosY, -1);
				Global.ClientRealive(client, toPosX, toPosY, -1);
				result = true;
			}
			return result;
		}

		
		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int side = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				YongZheZhanChangBirthPoint birthPoint = null;
				if (this.RuntimeData.MapBirthPointDict.TryGetValue(side, out birthPoint))
				{
					posX = birthPoint.PosX;
					posY = birthPoint.PosY;
					return side;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7) && !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("KingOfBattle") && GlobalNew.IsGongNengOpened(client, GongNengIDs.KingOfBattle, hint);
		}

		
		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			BattleCrystalMonsterItem tag = (monster != null) ? (monster.Tag as BattleCrystalMonsterItem) : null;
			int result;
			if (tag == null)
			{
				result = -200;
			}
			else
			{
				result = tag.GatherTime;
			}
			return result;
		}

		
		private string BuildSceneBuffKey(GameClient client, int bufferGoodsID)
		{
			return string.Format("{0}_{1}", client.ClientData.RoleID, bufferGoodsID);
		}

		
		private void UpdateBuff4GameClient(GameClient client, int bufferGoodsID, object tagInfo, bool add)
		{
			try
			{
				BattleCrystalMonsterItem CrystalItem = tagInfo as BattleCrystalMonsterItem;
				KingOfBattleDynamicMonsterItem MonsterItem = tagInfo as KingOfBattleDynamicMonsterItem;
				if (CrystalItem != null || null != MonsterItem)
				{
					int BuffTime = 0;
					BufferItemTypes buffItemType = BufferItemTypes.None;
					if (null != CrystalItem)
					{
						BuffTime = CrystalItem.BuffTime;
						buffItemType = BufferItemTypes.KingOfBattleCrystal;
					}
					if (null != MonsterItem)
					{
						BuffTime = MonsterItem.BuffTime;
						buffItemType = (BufferItemTypes)bufferGoodsID;
					}
					KingOfBattleScene scene;
					if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
					{
						EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(bufferGoodsID);
						if (null != item)
						{
							if (add)
							{
								double[] actionParams = new double[]
								{
									(double)BuffTime,
									(double)bufferGoodsID
								};
								Global.UpdateBufferData(client, buffItemType, actionParams, 1, true);
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.BufferByGoodsProps,
									bufferGoodsID,
									item.ExtProps
								});
								string Key = this.BuildSceneBuffKey(client, bufferGoodsID);
								scene.SceneBuffDict[Key] = new KingOfBattleSceneBuff
								{
									RoleID = client.ClientData.RoleID,
									BuffID = bufferGoodsID,
									EndTicks = TimeUtil.NOW() + (long)(BuffTime * 1000),
									tagInfo = tagInfo
								};
								if (buffItemType == BufferItemTypes.KingOfBattleCrystal)
								{
									client.SceneContextData = tagInfo;
								}
							}
							else
							{
								Global.RemoveBufferData(client, (int)buffItemType);
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.BufferByGoodsProps,
									bufferGoodsID,
									PropsCacheManager.ConstExtProps
								});
								string Key = this.BuildSceneBuffKey(client, bufferGoodsID);
								scene.SceneBuffDict.Remove(Key);
								if (buffItemType == BufferItemTypes.KingOfBattleCrystal)
								{
									client.SceneContextData = null;
								}
							}
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public void TryAddBossKillRandomBuff(GameClient client, KingOfBattleDynamicMonsterItem tagInfo)
		{
			int GoodsID = -1;
			if (tagInfo.RandomBuffList.Count != 0)
			{
				double rateEnd = 0.0;
				double rate = (double)Global.GetRandomNumber(1, 101) / 100.0;
				for (int i = 0; i < tagInfo.RandomBuffList.Count; i++)
				{
					rateEnd += tagInfo.RandomBuffList[i].Pct;
					if (rate <= rateEnd)
					{
						GoodsID = tagInfo.RandomBuffList[i].GoodsID;
						break;
					}
				}
				this.UpdateBuff4GameClient(client, GoodsID, tagInfo, true);
			}
		}

		
		public void InstallJunQi(KingOfBattleScene scene, GameClient client, KingOfBattleQiZhiConfig item)
		{
			CopyMap copyMap = scene.CopyMap;
			GameMap gameMap = GameManager.MapMgr.GetGameMap(scene.m_nMapCode);
			if (copyMap != null && null != gameMap)
			{
				item.Alive = true;
				item.BattleWhichSide = client.ClientData.BattleWhichSide;
				int BattleQiZhiMonsterID = 0;
				if (client.ClientData.BattleWhichSide == 1)
				{
					BattleQiZhiMonsterID = this.RuntimeData.BattleQiZhiMonsterID1;
				}
				else if (client.ClientData.BattleWhichSide == 2)
				{
					BattleQiZhiMonsterID = this.RuntimeData.BattleQiZhiMonsterID2;
				}
				GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, BattleQiZhiMonsterID, copyMap.CopyMapID, 1, item.PosX / gameMap.MapGridWidth, item.PosY / gameMap.MapGridHeight, 0, 0, SceneUIClasses.KingOfBattle, item, null);
			}
		}

		
		public void CalculateTeleportGateState(KingOfBattleScene scene)
		{
			int OpenGateSide = -1;
			foreach (KingOfBattleQiZhiConfig qizhi in scene.NPCID2QiZhiConfigDict.Values)
			{
				if (OpenGateSide == -1 && qizhi.Alive)
				{
					OpenGateSide = qizhi.BattleWhichSide;
				}
				if (!qizhi.Alive || qizhi.BattleWhichSide != OpenGateSide)
				{
					OpenGateSide = -1;
					break;
				}
			}
			scene.SceneOpenTeleportList.Clear();
			if (-1 != OpenGateSide)
			{
				scene.SceneOpenTeleportList.Add(OpenGateSide + 10);
			}
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<int>>(1189, scene.SceneOpenTeleportList, scene.CopyMap);
		}

		
		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			KingOfBattleQiZhiConfig item = null;
			bool isQiZuo = false;
			bool installJunQi = false;
			KingOfBattleScene scene = client.SceneObject as KingOfBattleScene;
			bool result;
			if (null == scene)
			{
				result = isQiZuo;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (scene.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out item))
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
						this.InstallJunQi(scene, client, item);
						this.CalculateTeleportGateState(scene);
					}
				}
				result = isQiZuo;
			}
			return result;
		}

		
		private void InitScene(KingOfBattleScene scene, GameClient client)
		{
			foreach (KingOfBattleQiZhiConfig item in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
			{
				scene.NPCID2QiZhiConfigDict.Add(item.NPCID, item.Clone() as KingOfBattleQiZhiConfig);
			}
		}

		
		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.KingOfBattle)
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
					int gameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime now = TimeUtil.NowDateTime();
					lock (this.RuntimeData.Mutex)
					{
						KingOfBattleScene scene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqId, out scene))
						{
							KingOfBattleSceneInfo sceneInfo = null;
							YongZheZhanChangFuBenData fuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out fuBenData))
							{
								LogManager.WriteLog(LogTypes.Error, "王者战场没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
							}
							if (!this.RuntimeData.SceneDataDict.TryGetValue(fuBenData.GroupIndex, out sceneInfo))
							{
								LogManager.WriteLog(LogTypes.Error, "王者战场没有为副本找到对应的档位数据,ID:" + fuBenData.GroupIndex, null, true);
							}
							scene = new KingOfBattleScene();
							scene.CopyMap = copyMap;
							scene.CleanAllInfo();
							scene.GameId = gameId;
							scene.m_nMapCode = mapCode;
							scene.CopyMapId = copyMap.CopyMapID;
							scene.FuBenSeqId = fuBenSeqId;
							scene.m_nPlarerCount = 1;
							scene.SceneInfo = sceneInfo;
							scene.MapGridWidth = gameMap.MapGridWidth;
							scene.MapGridHeight = gameMap.MapGridHeight;
							DateTime startTime = now.Date.Add(this.GetStartTime(sceneInfo.Id));
							scene.StartTimeTicks = startTime.Ticks / 10000L;
							this.InitScene(scene, client);
							scene.GameStatisticalData.GameId = gameId;
							this.SceneDict[fuBenSeqId] = scene;
						}
						else
						{
							scene.m_nPlarerCount++;
						}
						KingOfBattleClientContextData clientContextData;
						if (!scene.ClientContextDataDict.TryGetValue(roleId, out clientContextData))
						{
							clientContextData = new KingOfBattleClientContextData
							{
								RoleId = roleId,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide,
								RoleName = client.ClientData.RoleName,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								ZoneID = client.ClientData.ZoneID
							};
							scene.ClientContextDataDict[roleId] = clientContextData;
						}
						else
						{
							clientContextData.KillNum = 0;
						}
						client.SceneObject = scene;
						client.SceneGameId = (long)scene.GameId;
						client.SceneContextData2 = clientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(scene.SceneInfo.TotalSecs * 1000));
					}
					YongZheZhanChangClient.getInstance().GameFuBenRoleChangeState(roleId, 5, 0, 0);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == SceneUIClasses.KingOfBattle)
			{
				lock (this.RuntimeData.Mutex)
				{
					KingOfBattleScene KingOfBattleScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out KingOfBattleScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			lock (this.RuntimeData.Mutex)
			{
				KingOfBattleScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
					{
						BattleCrystalMonsterItem monsterItem = monster.Tag as BattleCrystalMonsterItem;
						if (monsterItem != null)
						{
							BattleCrystalMonsterItem crystalItem = client.SceneContextData as BattleCrystalMonsterItem;
							if (null != crystalItem)
							{
								this.AddDelayCreateMonster(scene, TimeUtil.NOW() + (long)crystalItem.FuHuoTime, crystalItem);
							}
							this.UpdateBuff4GameClient(client, monsterItem.BuffGoodsID, monsterItem, true);
						}
					}
				}
			}
		}

		
		public bool ClientChangeMap(GameClient client, int teleportID, ref int toNewMapCode, ref int toNewPosX, ref int toNewPosY)
		{
			KingOfBattleScene scene = client.SceneObject as KingOfBattleScene;
			bool result;
			if (null == scene)
			{
				result = false;
			}
			else
			{
				int OpenGateSide = teleportID % 10;
				result = (client.ClientData.BattleWhichSide == OpenGateSide && scene.SceneOpenTeleportList.Contains(teleportID));
			}
			return result;
		}

		
		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			if (client != null && (monster.MonsterInfo.ExtensionID == this.RuntimeData.BattleQiZhiMonsterID1 || monster.MonsterInfo.ExtensionID == this.RuntimeData.BattleQiZhiMonsterID2))
			{
				KingOfBattleScene scene = client.SceneObject as KingOfBattleScene;
				KingOfBattleQiZhiConfig qizhiConfig = monster.Tag as KingOfBattleQiZhiConfig;
				if (scene != null && null != qizhiConfig)
				{
					lock (this.RuntimeData.Mutex)
					{
						qizhiConfig.DeadTicks = TimeUtil.NOW();
						qizhiConfig.Alive = false;
						qizhiConfig.BattleWhichSide = client.ClientData.BattleWhichSide;
						this.CalculateTeleportGateState(scene);
					}
				}
			}
			KingOfBattleDynamicMonsterItem monsterConfig = monster.Tag as KingOfBattleDynamicMonsterItem;
			if (monsterConfig != null && (monsterConfig.MonsterType == 3 || monsterConfig.MonsterType == 2))
			{
				KingOfBattleScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					CopyMap copyMap = scene.CopyMap;
					string msgText = string.Format(GLang.GetLang(397, new object[0]), Global.FormatRoleName4(client));
					if (client.ClientData.BattleWhichSide == 1 && !scene.GuangMuNotify1)
					{
						scene.GuangMuNotify1 = true;
						GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, client.ClientData.BattleWhichSide, 0);
						GameManager.ClientMgr.BroadSpecialCopyMapMsg(copyMap, msgText, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
					}
					else if (client.ClientData.BattleWhichSide == 2 && !scene.GuangMuNotify2)
					{
						scene.GuangMuNotify2 = true;
						GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, client.ClientData.BattleWhichSide, 0);
						GameManager.ClientMgr.BroadSpecialCopyMapMsg(copyMap, msgText, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
					}
					msgText = string.Format(GLang.GetLang(398, new object[0]), Global.FormatRoleName4(client));
					GameManager.ClientMgr.BroadSpecialCopyMapMsg(copyMap, msgText, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
				}
			}
			if (monsterConfig != null && monsterConfig.MonsterType == 1)
			{
				KingOfBattleScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					this.ProcessEnd(scene, client.ClientData.BattleWhichSide, TimeUtil.NOW());
				}
			}
			if (monsterConfig != null && monsterConfig.MonsterType == 4)
			{
				KingOfBattleScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					string msgText = string.Format(GLang.GetLang(399, new object[0]), Global.FormatRoleName4(client));
					GameManager.ClientMgr.BroadSpecialCopyMapMsg(scene.CopyMap, msgText, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
				}
			}
		}

		
		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (monster.MonsterType == 401 || monster.MonsterType == 1301 || monster.MonsterType == 1302 || monster.MonsterType == 1303 || monster.MonsterType == 2000 || monster.MonsterType == 2001)
			{
				KingOfBattleClientContextData contextData = client.SceneContextData2 as KingOfBattleClientContextData;
				if (null != contextData)
				{
					KingOfBattleDynamicMonsterItem tagInfo = monster.Tag as KingOfBattleDynamicMonsterItem;
					if (null != tagInfo)
					{
						KingOfBattleScene scene = null;
						int addScore = 0;
						if (monster.HandledDead && monster.WhoKillMeID == client.ClientData.RoleID)
						{
							addScore += tagInfo.JiFenKill;
						}
						double jiFenInjure = this.RuntimeData.KingBattleBossAttackPercent * monster.MonsterInfo.VLifeMax;
						lock (this.RuntimeData.Mutex)
						{
							if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
							{
								if (scene.m_eStatus != GameSceneStatuses.STATUS_BEGIN)
								{
									return;
								}
								double InjureBossDelta = 0.0;
								contextData.InjureBossDeltaDict.TryGetValue(monster.MonsterInfo.ExtensionID, out InjureBossDelta);
								InjureBossDelta += (double)injure;
								if (InjureBossDelta >= jiFenInjure && jiFenInjure > 0.0)
								{
									int calcRate = (int)(InjureBossDelta / jiFenInjure);
									InjureBossDelta -= jiFenInjure * (double)calcRate;
									addScore += tagInfo.JiFenDamage * calcRate;
								}
								contextData.InjureBossDeltaDict[monster.MonsterInfo.ExtensionID] = InjureBossDelta;
								contextData.TotalScore += addScore;
								if (monster.HandledDead)
								{
									KingOfBattleDynamicMonsterItem RebornItem = null;
									if (tagInfo.RebornID != -1 && this.RuntimeData.DynMonsterDict.TryGetValue(tagInfo.RebornID, out RebornItem))
									{
										long ticks = TimeUtil.NOW();
										this.AddDelayCreateMonster(scene, ticks + (long)RebornItem.DelayBirthMs, RebornItem);
									}
									this.TryAddBossKillRandomBuff(client, tagInfo);
								}
								if (client.ClientData.BattleWhichSide == 1)
								{
									scene.ScoreData.Score1 += addScore;
								}
								else if (client.ClientData.BattleWhichSide == 2)
								{
									scene.ScoreData.Score2 += addScore;
								}
								scene.GameStatisticalData.BossScore += addScore;
							}
						}
						if (addScore > 0 && scene != null)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<KingOfBattleScoreData>(1184, scene.ScoreData, scene.CopyMap);
							this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
						}
					}
				}
			}
		}

		
		private void ProcessEnd(KingOfBattleScene scene, int successSide, long nowTicks)
		{
			if (successSide != 0)
			{
				List<KingOfBattleClientContextData> winSidePlayerList = new List<KingOfBattleClientContextData>();
				foreach (KingOfBattleClientContextData item in scene.ClientContextDataDict.Values)
				{
					if (item.BattleWhichSide == successSide)
					{
						winSidePlayerList.Add(item);
					}
				}
				winSidePlayerList.Sort(delegate(KingOfBattleClientContextData left, KingOfBattleClientContextData right)
				{
					int result;
					if (left.TotalScore > right.TotalScore)
					{
						result = -1;
					}
					else if (left.TotalScore < right.TotalScore)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
					return result;
				});
				if (winSidePlayerList.Count != 0)
				{
					scene.ClientContextMVP = winSidePlayerList[0];
				}
			}
			this.CompleteScene(scene, successSide);
			scene.m_eStatus = GameSceneStatuses.STATUS_END;
			scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
			scene.StateTimeData.GameType = 15;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		
		public void TimerProc()
		{
			long nowTicks = TimeUtil.NOW();
			if (nowTicks >= KingOfBattleManager.NextHeartBeatTicks)
			{
				KingOfBattleManager.NextHeartBeatTicks = nowTicks + 1020L;
				foreach (KingOfBattleScene scene in this.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int nID = scene.FuBenSeqId;
						int nCopyID = scene.CopyMapId;
						int nMapCodeID = scene.m_nMapCode;
						if (nID >= 0 && nCopyID >= 0 && nMapCodeID >= 0)
						{
							CopyMap copyMap = scene.CopyMap;
							DateTime now = TimeUtil.NowDateTime();
							long ticks = TimeUtil.NOW();
							if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE || scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								this.CheckCreateDynamicMonster(scene, ticks);
							}
							if (scene.m_eStatus == GameSceneStatuses.STATUS_NULL)
							{
								if (ticks >= scene.StartTimeTicks)
								{
									scene.m_lPrepareTime = scene.StartTimeTicks;
									scene.m_lBeginTime = scene.m_lPrepareTime + (long)(scene.SceneInfo.PrepareSecs * 1000);
									scene.m_eStatus = GameSceneStatuses.STATUS_PREPARE;
									scene.StateTimeData.GameType = 15;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									this.InitCreateDynamicMonster(scene);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_PREPARE)
							{
								if (ticks >= scene.m_lBeginTime)
								{
									scene.m_eStatus = GameSceneStatuses.STATUS_BEGIN;
									scene.m_lEndTime = scene.m_lBeginTime + (long)(scene.SceneInfo.FightingSecs * 1000);
									scene.StateTimeData.GameType = 15;
									scene.StateTimeData.State = (int)scene.m_eStatus;
									scene.StateTimeData.EndTicks = scene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
									for (int guangMuId = 3; guangMuId <= 8; guangMuId++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, guangMuId, 0);
									}
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								if (ticks >= scene.m_lEndTime)
								{
									int successSide = 0;
									if (scene.ScoreData.Score1 > scene.ScoreData.Score2)
									{
										successSide = 1;
									}
									else if (scene.ScoreData.Score2 > scene.ScoreData.Score1)
									{
										successSide = 2;
									}
									this.ProcessEnd(scene, successSide, ticks);
								}
								else
								{
									this.CheckSceneBufferTime(scene, ticks);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_END)
							{
								GameManager.CopyMapMgr.KillAllMonster(scene.CopyMap);
								scene.m_eStatus = GameSceneStatuses.STATUS_AWARD;
								YongZheZhanChangClient.getInstance().GameFuBenChangeState(scene.GameId, GameFuBenState.End, now);
								this.GiveAwards(scene);
								YongZheZhanChangFuBenData fuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out fuBenData))
								{
									fuBenData.State = GameFuBenState.End;
									LogManager.WriteLog(LogTypes.Error, string.Format("王者战场跨服副本GameID={0},战斗结束", fuBenData.GameId), null, true);
								}
							}
							else if (scene.m_eStatus == GameSceneStatuses.STATUS_AWARD)
							{
								if (ticks >= scene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(scene.m_lLeaveTime);
									scene.m_eStatus = GameSceneStatuses.STATUS_CLEAR;
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
										DataHelper.WriteExceptionLogEx(ex, "王者战场系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		
		private void AddDelayCreateMonster(KingOfBattleScene scene, long ticks, object monster)
		{
			lock (this.RuntimeData.Mutex)
			{
				List<object> list = null;
				if (!scene.CreateMonsterQueue.TryGetValue(ticks, out list))
				{
					list = new List<object>();
					scene.CreateMonsterQueue.Add(ticks, list);
				}
				list.Add(monster);
			}
		}

		
		private void InitCreateDynamicMonster(KingOfBattleScene scene)
		{
			lock (this.RuntimeData.Mutex)
			{
				foreach (KingOfBattleQiZhiConfig item in scene.NPCID2QiZhiConfigDict.Values)
				{
					item.Alive = true;
					if (item.QiZhiMonsterID == this.RuntimeData.BattleQiZhiMonsterID1)
					{
						item.BattleWhichSide = 1;
					}
					else if (item.QiZhiMonsterID == this.RuntimeData.BattleQiZhiMonsterID2)
					{
						item.BattleWhichSide = 2;
					}
					GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, item.QiZhiMonsterID, scene.CopyMapId, 1, item.PosX / scene.MapGridWidth, item.PosY / scene.MapGridHeight, 0, 0, SceneUIClasses.KingOfBattle, item, null);
				}
				foreach (BattleCrystalMonsterItem crystal in this.RuntimeData.BattleCrystalMonsterDict.Values)
				{
					this.AddDelayCreateMonster(scene, scene.m_lPrepareTime, crystal);
				}
				List<KingOfBattleDynamicMonsterItem> dynMonsterList = null;
				if (this.RuntimeData.SceneDynMonsterDict.TryGetValue(scene.m_nMapCode, out dynMonsterList))
				{
					foreach (KingOfBattleDynamicMonsterItem item2 in dynMonsterList)
					{
						if (!item2.RebornBirth)
						{
							this.AddDelayCreateMonster(scene, scene.m_lPrepareTime + (long)item2.DelayBirthMs, item2);
						}
					}
				}
			}
		}

		
		public void CheckCreateDynamicMonster(KingOfBattleScene scene, long nowMs)
		{
			lock (this.RuntimeData.Mutex)
			{
				while (scene.CreateMonsterQueue.Count > 0)
				{
					KeyValuePair<long, List<object>> pair = scene.CreateMonsterQueue.First<KeyValuePair<long, List<object>>>();
					if (nowMs < pair.Key)
					{
						break;
					}
					try
					{
						foreach (object obj in pair.Value)
						{
							if (obj is KingOfBattleDynamicMonsterItem)
							{
								KingOfBattleDynamicMonsterItem item = obj as KingOfBattleDynamicMonsterItem;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, item.MonsterID, scene.CopyMapId, 1, item.PosX / scene.MapGridWidth, item.PosY / scene.MapGridHeight, 0, item.PursuitRadius, SceneUIClasses.KingOfBattle, item, null);
								if (item.MonsterType == 4)
								{
									string msgText = string.Format(GLang.GetLang(400, new object[0]), Global.GetMonsterNameByID(item.MonsterID));
									GameManager.ClientMgr.BroadSpecialCopyMapMsg(scene.CopyMap, msgText, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
								}
							}
							else if (obj is BattleCrystalMonsterItem)
							{
								BattleCrystalMonsterItem crystal = obj as BattleCrystalMonsterItem;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, crystal.MonsterID, scene.CopyMap.CopyMapID, 1, crystal.PosX / scene.MapGridWidth, crystal.PosY / scene.MapGridHeight, 0, 0, SceneUIClasses.KingOfBattle, crystal, null);
							}
						}
					}
					finally
					{
						scene.CreateMonsterQueue.RemoveAt(0);
					}
				}
			}
		}

		
		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true, bool selfScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				KingOfBattleScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<KingOfBattleScoreData>(1184, scene.ScoreData, false);
					}
					if (selfScore)
					{
						KingOfBattleClientContextData clientContextData = client.SceneContextData2 as KingOfBattleClientContextData;
						if (null != clientContextData)
						{
							client.sendCmd<int>(1185, clientContextData.TotalScore, false);
						}
					}
				}
			}
		}

		
		public void CompleteScene(KingOfBattleScene scene, int successSide)
		{
			scene.SuccessSide = successSide;
		}

		
		public void RemoveBattleSceneBuffForRole(KingOfBattleScene scene, GameClient client)
		{
			List<KingOfBattleSceneBuff> sceneBuffDeleteList = new List<KingOfBattleSceneBuff>();
			lock (this.RuntimeData.Mutex)
			{
				if (scene.SceneBuffDict.Count != 0)
				{
					foreach (KingOfBattleSceneBuff contextData in scene.SceneBuffDict.Values)
					{
						if (contextData.RoleID == client.ClientData.RoleID)
						{
							sceneBuffDeleteList.Add(contextData);
						}
					}
					if (sceneBuffDeleteList.Count != 0)
					{
						foreach (KingOfBattleSceneBuff contextData in sceneBuffDeleteList)
						{
							if (contextData.RoleID != 0)
							{
								this.UpdateBuff4GameClient(client, contextData.BuffID, contextData.tagInfo, false);
							}
							BattleCrystalMonsterItem CrystalItem = contextData.tagInfo as BattleCrystalMonsterItem;
							if (null != CrystalItem)
							{
								this.AddDelayCreateMonster(scene, TimeUtil.NOW() + (long)CrystalItem.FuHuoTime, contextData.tagInfo);
							}
						}
					}
				}
			}
		}

		
		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				KingOfBattleScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					if (scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
					{
						int addScore = 0;
						int addScoreDie = this.RuntimeData.KingOfBattleDie;
						KingOfBattleClientContextData clientLianShaContextData = client.SceneContextData2 as KingOfBattleClientContextData;
						KingOfBattleClientContextData otherLianShaContextData = other.SceneContextData2 as KingOfBattleClientContextData;
						HuanYingSiYuanLianSha huanYingSiYuanLianSha = null;
						HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = null;
						HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
						huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
						huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
						huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
						huanYingSiYuanAddScore.ByLianShaNum = 1;
						huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
						huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
						scene.GameStatisticalData.KillScore += this.RuntimeData.KingOfBattleUltraKillParam1;
						if (null != clientLianShaContextData)
						{
							clientLianShaContextData.KillNum++;
							int lianShaScore = this.RuntimeData.KingOfBattleUltraKillParam1 + clientLianShaContextData.KillNum * this.RuntimeData.KingOfBattleUltraKillParam2;
							lianShaScore = Math.Min(this.RuntimeData.KingOfBattleUltraKillParam4, Math.Max(this.RuntimeData.KingOfBattleUltraKillParam3, lianShaScore));
							huanYingSiYuanAddScore.ByLianShaNum = 1;
							huanYingSiYuanLianSha = new HuanYingSiYuanLianSha();
							huanYingSiYuanLianSha.Name = huanYingSiYuanAddScore.Name;
							huanYingSiYuanLianSha.ZoneID = huanYingSiYuanAddScore.ZoneID;
							huanYingSiYuanLianSha.Occupation = huanYingSiYuanAddScore.Occupation;
							huanYingSiYuanLianSha.LianShaType = Math.Min(clientLianShaContextData.KillNum, 30) / 5;
							huanYingSiYuanLianSha.ExtScore = lianShaScore;
							huanYingSiYuanLianSha.Side = huanYingSiYuanAddScore.Side;
							addScore += lianShaScore;
							scene.GameStatisticalData.LianShaScore += lianShaScore;
							if (clientLianShaContextData.KillNum % 5 != 0)
							{
								huanYingSiYuanLianSha = null;
							}
						}
						if (null != otherLianShaContextData)
						{
							int overScore = this.RuntimeData.KingOfBattleShutDownParam1 + otherLianShaContextData.KillNum * this.RuntimeData.KingOfBattleShutDownParam2;
							overScore = Math.Min(this.RuntimeData.KingOfBattleShutDownParam4, Math.Max(this.RuntimeData.KingOfBattleShutDownParam3, overScore));
							addScore += overScore;
							scene.GameStatisticalData.ZhongJieScore += overScore;
							if (otherLianShaContextData.KillNum >= 10)
							{
								huanYingSiYuanLianshaOver = new HuanYingSiYuanLianshaOver();
								huanYingSiYuanLianshaOver.KillerName = huanYingSiYuanAddScore.Name;
								huanYingSiYuanLianshaOver.KillerZoneID = huanYingSiYuanAddScore.ZoneID;
								huanYingSiYuanLianshaOver.KillerOccupation = client.ClientData.Occupation;
								huanYingSiYuanLianshaOver.KillerSide = huanYingSiYuanAddScore.Side;
								huanYingSiYuanLianshaOver.KilledName = Global.FormatRoleName4(other);
								huanYingSiYuanLianshaOver.KilledZoneID = other.ClientData.ZoneID;
								huanYingSiYuanLianshaOver.KilledOccupation = other.ClientData.Occupation;
								huanYingSiYuanLianshaOver.KilledSide = other.ClientData.BattleWhichSide;
								huanYingSiYuanLianshaOver.ExtScore = overScore;
							}
							otherLianShaContextData.KillNum = 0;
							otherLianShaContextData.TotalScore += addScoreDie;
							scene.GameStatisticalData.KillScore += addScoreDie;
						}
						huanYingSiYuanAddScore.Score = addScore;
						if (client.ClientData.BattleWhichSide == 1)
						{
							scene.ScoreData.Score1 += addScore;
							scene.ScoreData.Score2 += addScoreDie;
						}
						else
						{
							scene.ScoreData.Score2 += addScore;
							scene.ScoreData.Score1 += addScoreDie;
						}
						if (null != clientLianShaContextData)
						{
							clientLianShaContextData.TotalScore += addScore;
						}
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<KingOfBattleScoreData>(1184, scene.ScoreData, scene.CopyMap);
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(1186, huanYingSiYuanLianSha, scene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(1187, huanYingSiYuanLianshaOver, scene.CopyMap);
						}
						this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
						this.NotifyTimeStateInfoAndScoreInfo(other, false, false, true);
					}
				}
			}
		}

		
		public void SubmitCrystalBuff(GameClient client, int areaLuaID)
		{
			if (areaLuaID == client.ClientData.BattleWhichSide)
			{
				BattleCrystalMonsterItem crystalItem = client.SceneContextData as BattleCrystalMonsterItem;
				if (null != crystalItem)
				{
					lock (this.RuntimeData.Mutex)
					{
						KingOfBattleScene scene;
						if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
						{
							KingOfBattleClientContextData contextData = client.SceneContextData2 as KingOfBattleClientContextData;
							if (contextData != null && scene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
							{
								int addScore = crystalItem.BattleJiFen;
								contextData.TotalScore += addScore;
								scene.GameStatisticalData.CaiJiScore += addScore;
								if (client.ClientData.BattleWhichSide == 1)
								{
									scene.ScoreData.Score1 += addScore;
								}
								else if (client.ClientData.BattleWhichSide == 2)
								{
									scene.ScoreData.Score2 += addScore;
								}
								if (addScore > 0)
								{
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<KingOfBattleScoreData>(1184, scene.ScoreData, scene.CopyMap);
									this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
								}
							}
							this.UpdateBuff4GameClient(client, crystalItem.BuffGoodsID, crystalItem, false);
							this.AddDelayCreateMonster(scene, TimeUtil.NOW() + (long)crystalItem.FuHuoTime, crystalItem);
						}
					}
				}
			}
		}

		
		public void GiveAwards(KingOfBattleScene scene)
		{
			try
			{
				Dictionary<int, int[]> bhDict = new Dictionary<int, int[]>();
				KingOfBattleStatisticalData gameResultData = scene.GameStatisticalData;
				foreach (KingOfBattleClientContextData contextData in scene.ClientContextDataDict.Values)
				{
					gameResultData.AllRoleCount++;
					int success;
					if (contextData.BattleWhichSide == scene.SuccessSide)
					{
						success = 1;
						gameResultData.WinRoleCount++;
					}
					else
					{
						success = 0;
						gameResultData.LoseRoleCount++;
					}
					GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleId);
					string awardsInfo = string.Format("{0},{1},{2},{3},{4}", new object[]
					{
						scene.SceneInfo.Id,
						success,
						contextData.TotalScore,
						scene.ScoreData.Score1,
						scene.ScoreData.Score2
					});
					string mvpInfo = string.Format("{0}&{1}&{2}&{3}", new object[]
					{
						scene.ClientContextMVP.RoleId,
						scene.ClientContextMVP.RoleName,
						scene.ClientContextMVP.Occupation,
						scene.ClientContextMVP.RoleSex
					});
					byte[] bytes = new UTF8Encoding().GetBytes(mvpInfo);
					mvpInfo = Convert.ToBase64String(bytes);
					if (client != null)
					{
						int bhid = client.ClientData.Faction;
						int junTuanId = client.ClientData.JunTuanId;
						if (bhid > 0 && junTuanId > 0)
						{
							int[] kv;
							if (!bhDict.TryGetValue(bhid, out kv))
							{
								int[] array = new int[2];
								array[0] = junTuanId;
								kv = array;
								bhDict[bhid] = kv;
							}
							kv[1]++;
						}
						int score = contextData.TotalScore;
						contextData.TotalScore = 0;
						if (score >= this.RuntimeData.KingOfBattleLowestJiFen)
						{
							Global.SaveRoleParamsStringToDB(client, "32", awardsInfo, true);
							Global.SaveRoleParamsStringToDB(client, "43", mvpInfo, true);
						}
						else
						{
							Global.SaveRoleParamsStringToDB(client, "32", this.RuntimeData.RoleParamsAwardsDefaultString, true);
							Global.SaveRoleParamsStringToDB(client, "43", "", true);
						}
						this.NtfCanGetAward(client, success, score, scene.SceneInfo, scene.ScoreData.Score1, scene.ScoreData.Score2, scene.ClientContextMVP.RoleId, scene.ClientContextMVP.RoleName, scene.ClientContextMVP.Occupation, scene.ClientContextMVP.RoleSex);
					}
					else if (contextData.TotalScore >= this.RuntimeData.KingOfBattleLowestJiFen)
					{
						Global.UpdateRoleParamByNameOffline(contextData.RoleId, "32", awardsInfo, contextData.ServerId);
						Global.UpdateRoleParamByNameOffline(contextData.RoleId, "43", mvpInfo, contextData.ServerId);
					}
					else
					{
						Global.UpdateRoleParamByNameOffline(contextData.RoleId, "32", this.RuntimeData.RoleParamsAwardsDefaultString, contextData.ServerId);
						Global.UpdateRoleParamByNameOffline(contextData.RoleId, "43", "", contextData.ServerId);
					}
				}
				foreach (KeyValuePair<int, int[]> kv2 in bhDict)
				{
					JunTuanManager.getInstance().AddJunTuanTaskValue(kv2.Key, kv2.Value[0], (int)Global.GetMapSceneType(scene.m_nMapCode), kv2.Value[1]);
				}
				YongZheZhanChangClient.getInstance().PushGameResultData(gameResultData);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "王者战场系统清场调度异常");
			}
		}

		
		private void NtfCanGetAward(GameClient client, int success, int score, KingOfBattleSceneInfo sceneInfo, int sideScore1, int sideScore2, int mvprid, string mvpname, int mvpocc, int mvpsex)
		{
			long addExp = 0L;
			int addBindJinBi = 0;
			List<AwardsItemData> awardsItemDataList = null;
			if (score >= this.RuntimeData.KingOfBattleLowestJiFen)
			{
				addExp = (long)((double)sceneInfo.Exp * (0.2 + Math.Min(0.8, Math.Pow((double)score, 0.5) / 100.0)));
				addBindJinBi = (int)((double)sceneInfo.BandJinBi * Math.Min(100.0, Math.Pow((double)score, 0.5)));
				if (success > 0)
				{
					awardsItemDataList = sceneInfo.WinAwardsItemList.Items;
				}
				else
				{
					addExp = (long)((double)addExp * 0.8);
					addBindJinBi = (int)((double)addBindJinBi * 0.8);
					awardsItemDataList = sceneInfo.LoseAwardsItemList.Items;
				}
				addExp -= addExp % 10000L;
				addBindJinBi -= addBindJinBi % 10000;
			}
			client.sendCmd<KingOfBattleAwardsData>(1182, new KingOfBattleAwardsData
			{
				Exp = addExp,
				BindJinBi = addBindJinBi,
				Success = success,
				AwardsItemDataList = awardsItemDataList,
				SideScore1 = sideScore1,
				SideScore2 = sideScore2,
				SelfScore = score,
				MvpRoleName = mvpname,
				MvpOccupation = mvpocc,
				MvpRoleSex = mvpsex
			}, false);
		}

		
		private int GiveRoleAwards(GameClient client, int success, int score, KingOfBattleSceneInfo sceneInfo)
		{
			long addExp = 0L;
			int addBindJinBi = 0;
			List<AwardsItemData> awardsItemDataList = null;
			if (score >= this.RuntimeData.KingOfBattleLowestJiFen)
			{
				addExp = (long)((double)sceneInfo.Exp * (0.2 + Math.Min(0.8, Math.Pow((double)score, 0.5) / 100.0)));
				addBindJinBi = (int)((double)sceneInfo.BandJinBi * Math.Min(100.0, Math.Pow((double)score, 0.5)));
				if (success > 0)
				{
					awardsItemDataList = sceneInfo.WinAwardsItemList.Items;
				}
				else
				{
					addExp = (long)((double)addExp * 0.8);
					addBindJinBi = (int)((double)addBindJinBi * 0.8);
					awardsItemDataList = sceneInfo.LoseAwardsItemList.Items;
				}
				addExp -= addExp % 10000L;
				addBindJinBi -= addBindJinBi % 10000;
			}
			int result;
			if (awardsItemDataList != null && !Global.CanAddGoodsNum(client, awardsItemDataList.Count))
			{
				result = -100;
			}
			else
			{
				if (addExp > 0L)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, addExp, true, true, false, "none");
				}
				if (addBindJinBi > 0)
				{
					GameManager.ClientMgr.AddMoney1(client, addBindJinBi, "王者战场奖励", true);
				}
				if (awardsItemDataList != null)
				{
					foreach (AwardsItemData item in awardsItemDataList)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "王者战场奖励", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
					}
				}
				if (score >= this.RuntimeData.KingOfBattleLowestJiFen && success > 0)
				{
					GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_KingOfBattle, new int[0]));
				}
				result = 1;
			}
			return result;
		}

		
		public void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				KingOfBattleScene scene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out scene))
				{
					scene.m_nPlarerCount--;
					this.RemoveBattleSceneBuffForRole(scene, client);
				}
			}
		}

		
		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		
		private void CheckSceneBufferTime(KingOfBattleScene kingOfBattleScene, long nowTicks)
		{
			List<KingOfBattleSceneBuff> sceneBuffDeleteList = new List<KingOfBattleSceneBuff>();
			lock (this.RuntimeData.Mutex)
			{
				if (kingOfBattleScene.m_eStatus == GameSceneStatuses.STATUS_BEGIN)
				{
					if (kingOfBattleScene.SceneBuffDict.Count != 0)
					{
						foreach (KingOfBattleSceneBuff contextData in kingOfBattleScene.SceneBuffDict.Values)
						{
							if (contextData.EndTicks < nowTicks)
							{
								sceneBuffDeleteList.Add(contextData);
							}
						}
						if (sceneBuffDeleteList.Count != 0)
						{
							foreach (KingOfBattleSceneBuff contextData in sceneBuffDeleteList)
							{
								if (contextData.RoleID != 0)
								{
									GameClient client = GameManager.ClientMgr.FindClient(contextData.RoleID);
									if (null != client)
									{
										this.UpdateBuff4GameClient(client, contextData.BuffID, contextData.tagInfo, false);
									}
								}
								BattleCrystalMonsterItem CrystalItem = contextData.tagInfo as BattleCrystalMonsterItem;
								if (null != CrystalItem)
								{
									this.AddDelayCreateMonster(kingOfBattleScene, TimeUtil.NOW() + (long)CrystalItem.FuHuoTime, contextData.tagInfo);
								}
							}
						}
					}
				}
			}
		}

		
		public const SceneUIClasses ManagerType = SceneUIClasses.KingOfBattle;

		
		private static KingOfBattleManager instance = new KingOfBattleManager();

		
		public KingOfBattleData RuntimeData = new KingOfBattleData();

		
		public ConcurrentDictionary<int, KingOfBattleScene> SceneDict = new ConcurrentDictionary<int, KingOfBattleScene>();

		
		private static long NextHeartBeatTicks = 0L;
	}
}
