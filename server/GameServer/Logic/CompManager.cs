using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	
	public class CompManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		
		public static CompManager getInstance()
		{
			return CompManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("CompManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.CompConfigDict.Clear();
					fileName = "Config/Comp.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompConfig item = new CompConfig();
						item.ID = (int)Global.GetSafeAttributeLong(node, "CompID");
						item.CompName = Global.GetSafeAttributeStr(node, "CompName");
						item.MapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						item.BossID = (int)Global.GetSafeAttributeLong(node, "MonstersID");
						item.MaxPlayer = (int)Global.GetSafeAttributeLong(node, "MaxPlayer");
						item.MoBai = (int)Global.GetSafeAttributeLong(node, "MoBai");
						string JiaoTuanBirth = Global.GetSafeAttributeStr(node, "JiaoTuanBirth");
						if (!string.IsNullOrEmpty(JiaoTuanBirth))
						{
							string[] BirthFields = JiaoTuanBirth.Split(new char[]
							{
								'|'
							});
							if (BirthFields.Length == 3)
							{
								item.BirthPosX[0] = Global.SafeConvertToInt32(BirthFields[0]);
								item.BirthPosY[0] = Global.SafeConvertToInt32(BirthFields[1]);
								item.BirthRadius[0] = Global.SafeConvertToInt32(BirthFields[2]);
							}
						}
						string MengJunBirth = Global.GetSafeAttributeStr(node, "MengJunBirth");
						if (!string.IsNullOrEmpty(MengJunBirth))
						{
							string[] BirthFields = MengJunBirth.Split(new char[]
							{
								'|'
							});
							if (BirthFields.Length == 3)
							{
								item.BirthPosX[1] = Global.SafeConvertToInt32(BirthFields[0]);
								item.BirthPosY[1] = Global.SafeConvertToInt32(BirthFields[1]);
								item.BirthRadius[1] = Global.SafeConvertToInt32(BirthFields[2]);
							}
						}
						string XieHuiBirth = Global.GetSafeAttributeStr(node, "XieHuiBirth");
						if (!string.IsNullOrEmpty(XieHuiBirth))
						{
							string[] BirthFields = XieHuiBirth.Split(new char[]
							{
								'|'
							});
							if (BirthFields.Length == 3)
							{
								item.BirthPosX[2] = Global.SafeConvertToInt32(BirthFields[0]);
								item.BirthPosY[2] = Global.SafeConvertToInt32(BirthFields[1]);
								item.BirthRadius[2] = Global.SafeConvertToInt32(BirthFields[2]);
							}
						}
						this.RuntimeData.CompConfigDict[item.ID] = item;
					}
					this.RuntimeData.CompResourcesConfigDict.Clear();
					fileName = "Config/CompResources.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompResourcesConfig item2 = new CompResourcesConfig();
						item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.MapCodeID = (int)Global.GetSafeAttributeLong(node, "MapID");
						item2.MonsterID = (int)Global.GetSafeAttributeLong(node, "MonstersID");
						string[] strFields = Global.GetSafeAttributeStr(node, "Site").Split(new char[]
						{
							'|'
						});
						if (strFields.Length == 2)
						{
							item2.PosX = Global.SafeConvertToInt32(strFields[0]);
							item2.PosY = Global.SafeConvertToInt32(strFields[1]);
						}
						item2.GrowTime = (int)Global.GetSafeAttributeLong(node, "GrowTime");
						item2.CollectTime = (int)Global.GetSafeAttributeLong(node, "CollectTime");
						item2.AutoCollectTime = (int)Global.GetSafeAttributeLong(node, "AutoCollectTime");
						strFields = Global.GetSafeAttributeStr(node, "RefreshTime").Split(new char[]
						{
							'-'
						});
						if (strFields.Length == 2)
						{
							TimeSpan.TryParse(strFields[0], out item2.RefreshTimeBegin);
							TimeSpan.TryParse(strFields[1], out item2.RefreshTimeEnd);
						}
						item2.BoomValue = (int)Global.GetSafeAttributeLong(node, "CompNum");
						item2.CompDonate = (int)Global.GetSafeAttributeLong(node, "CompHonor");
						item2.JunXian = (int)Global.GetSafeAttributeLong(node, "CompFeast");
						this.RuntimeData.CompResourcesConfigDict[item2.ID] = item2;
					}
					this.RuntimeData.CompSolderSiteConfigList.Clear();
					fileName = "Config/CompSolderSite.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompSolderSiteConfig item3 = new CompSolderSiteConfig();
						item3.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						string[] strFields = Global.GetSafeAttributeStr(node, "Site").Split(new char[]
						{
							'|'
						});
						if (strFields.Length == 2)
						{
							item3.PosX = Global.SafeConvertToInt32(strFields[0]);
							item3.PosY = Global.SafeConvertToInt32(strFields[1]);
						}
						strFields = Global.GetSafeAttributeStr(node, "RefreshTime").Split(new char[]
						{
							'-'
						});
						if (strFields.Length == 2)
						{
							TimeSpan.TryParse(strFields[0], out item3.RefreshTimeBegin);
							TimeSpan.TryParse(strFields[1], out item3.RefreshTimeEnd);
						}
						this.RuntimeData.CompSolderSiteConfigList.Add(item3);
					}
					this.RuntimeData.CompSolderConfigDict.Clear();
					fileName = "Config/CompSolder.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompSolderConfig item4 = new CompSolderConfig();
						item4.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item4.CompID = (int)Global.GetSafeAttributeLong(node, "CompID");
						item4.Rank = (int)Global.GetSafeAttributeLong(node, "Level");
						item4.MonsterID = (int)Global.GetSafeAttributeLong(node, "MonstersID");
						item4.AlarmTime = (int)Global.GetSafeAttributeLong(node, "AlarmTime");
						this.RuntimeData.CompSolderConfigDict[new KeyValuePair<int, int>(item4.CompID, item4.Rank)] = item4;
					}
					this.RuntimeData.CompNoticeConfigDict.Clear();
					fileName = "Config/CompNotice.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompNoticeConfig item5 = new CompNoticeConfig();
						item5.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item5.Goal = (int)Global.GetSafeAttributeLong(node, "Goal");
						item5.CDTime = (int)Global.GetSafeAttributeLong(node, "CDTime");
						item5.Range = (int)Global.GetSafeAttributeLong(node, "Range");
						item5.OriginalMapOpen = Global.String2IntArray(Global.GetSafeAttributeStr(node, "OriginalMapOpen"), '|');
						this.RuntimeData.CompNoticeConfigDict[item5.ID] = item5;
					}
					this.RuntimeData.CompLevelConfigList.Clear();
					fileName = "Config/CompLevel.xml";
					fullPathFileName = Global.GameResPath(fileName);
					xml = XElement.Load(fullPathFileName);
					nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						CompLevelConfig item6 = new CompLevelConfig();
						item6.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item6.CompID = (int)Global.GetSafeAttributeLong(node, "CompID");
						item6.Level = (int)Global.GetSafeAttributeLong(node, "Level");
						item6.Enemy = (int)Global.GetSafeAttributeLong(node, "Enemy");
						item6.TalkCD = (int)Global.GetSafeAttributeLong(node, "TalkCD");
						item6.CraftSelfBuffID = (int)Global.GetSafeAttributeLong(node, "CraftSelfBuffID");
						item6.CraftBuffID = (int)Global.GetSafeAttributeLong(node, "CraftBuffID");
						this.RuntimeData.CompLevelConfigList.Add(item6);
					}
					ConfigParser.ParseAwardsItemList(GameManager.systemParamsList.GetParamValueByName("CompRecommend"), ref this.RuntimeData.CompRecommend, '|', ',');
					this.RuntimeData.CompRecommendRatio = GameManager.systemParamsList.GetParamValueDoubleByName("CompRecommendRatio", 0.0);
					this.RuntimeData.CompReplaceAmerce = GameManager.systemParamsList.GetParamValueDoubleByName("CompReplaceAmerce", 0.0);
					this.RuntimeData.CompReplaceNeed = (int)GameManager.systemParamsList.GetParamValueIntByName("CompReplaceNeed", -1);
					this.RuntimeData.CompSolderCD = GameManager.systemParamsList.GetParamValueIntArrayByName("CompSolderCD", ',');
					this.RuntimeData.CompBossCompNum = GameManager.systemParamsList.GetParamValueIntArrayByName("CompBossCompNum", ',');
					this.RuntimeData.CompBossCompHonor = GameManager.systemParamsList.GetParamValueIntArrayByName("CompBossCompHonor", ',');
					this.RuntimeData.CompBossRealive = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CompBossRealive", ',');
					this.RuntimeData.CompEnemy = GameManager.systemParamsList.GetParamValueIntArrayByName("CompEnemy", ',');
					this.RuntimeData.CompEnemyHurtNum = GameManager.systemParamsList.GetParamValueDoubleByName("CompEnemyHurtNum", 0.0);
					List<string> ShopList = GameManager.systemParamsList.GetParamValueStringListByName("CompShop", '|');
					foreach (string item7 in ShopList)
					{
						string[] strFields = item7.Split(new char[]
						{
							','
						});
						int boom = Global.SafeConvertToInt32(strFields[0]);
						int num = Global.SafeConvertToInt32(strFields[1]);
						this.RuntimeData.CompShop.Add(new Tuple<int, int>(boom, num));
					}
					this.RuntimeData.CompShopDuiHuanType = GameManager.systemParamsList.GetParamValueIntArrayByName("CraftStore", ',');
					this.RuntimeData.MaxDailyTaskNumDict.Clear();
					int[] MaxDailyTaskNumList = GameManager.systemParamsList.GetParamValueIntArrayByName("CompTaskNum", ',');
					if (null != MaxDailyTaskNumList)
					{
						for (int loop = 0; loop < MaxDailyTaskNumList.Length; loop++)
						{
							this.RuntimeData.MaxDailyTaskNumDict[100 + loop] = MaxDailyTaskNumList[loop];
						}
					}
					this.RuntimeData.CompTaskBeginDict.Clear();
					List<string> CompTaskBeginList = GameManager.systemParamsList.GetParamValueStringListByName("CompTaskBegin", '|');
					foreach (string item7 in CompTaskBeginList)
					{
						string[] beginFields = item7.Split(new char[]
						{
							','
						});
						if (beginFields.Length == 4)
						{
							int taskClass = Global.SafeConvertToInt32(beginFields[0]);
							List<int> beginList = null;
							if (!this.RuntimeData.CompTaskBeginDict.TryGetValue(taskClass, out beginList))
							{
								beginList = new List<int>();
								this.RuntimeData.CompTaskBeginDict[taskClass] = beginList;
							}
							beginList.Add(Global.SafeConvertToInt32(beginFields[1]));
							beginList.Add(Global.SafeConvertToInt32(beginFields[2]));
							beginList.Add(Global.SafeConvertToInt32(beginFields[3]));
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

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1125, 1, 1, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1126, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1127, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1128, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1129, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1130, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1132, 1, 5, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1136, 1, 1, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1137, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10002, 48, CompManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 48, CompManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, CompManager.getInstance());
			GlobalEventSource.getInstance().registerListener(14, CompManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, CompManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10002, 48, CompManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 48, CompManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, CompManager.getInstance());
			GlobalEventSource.getInstance().removeListener(14, CompManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, CompManager.getInstance());
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

		
		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.Comp, hint);
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened(client, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1125:
					return this.ProcessCompDataCmd(client, nID, bytes, cmdParams);
				case 1126:
					return this.ProcessCompJoinCmd(client, nID, bytes, cmdParams);
				case 1127:
					return this.ProcessCompRankInfoCmd(client, nID, bytes, cmdParams);
				case 1128:
					return this.ProcessCompSetBulletinCmd(client, nID, bytes, cmdParams);
				case 1129:
					return this.ProcessCompSetEnemyCmd(client, nID, bytes, cmdParams);
				case 1130:
					return this.ProcessCompZhiWuCmd(client, nID, bytes, cmdParams);
				case 1132:
					return this.ProcessCompEnterCmd(client, nID, bytes, cmdParams);
				case 1136:
					return this.ProcessGetCompAdmireDataCmd(client, nID, bytes, cmdParams);
				case 1137:
					return this.ProcessCompAdmireCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 11)
			{
				MonsterDeadEventObject e = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(e.getAttacker(), e.getMonster());
			}
			if (eventType == 14)
			{
				PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
				if (null != playerInitGameEventObject)
				{
					this.OnInitGame(playerInitGameEventObject.getPlayer());
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
				}
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num != 30)
			{
				if (num == 10002)
				{
					CaiJiEventObject e = eventObject as CaiJiEventObject;
					if (null != e)
					{
						GameClient client = e.Source as GameClient;
						Monster monster = e.Target as Monster;
						this.OnCaiJiFinish(client, monster);
						eventObject.Handled = true;
						eventObject.Result = true;
					}
				}
			}
			else
			{
				OnCreateMonsterEventObject e2 = eventObject as OnCreateMonsterEventObject;
				if (null != e2)
				{
					CompSolderSiteConfig siteConfig = e2.Monster.Tag as CompSolderSiteConfig;
					if (null != siteConfig)
					{
						e2.Monster.Camp = siteConfig.SolderConfig.CompID;
						e2.Result = true;
						e2.Handled = true;
					}
				}
			}
		}

		
		public bool ProcessCompDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				Dictionary<int, KFCompData> tempCompDataDict = null;
				Dictionary<int, List<KFCompRankInfo>> tempCompRankJXLDict = null;
				lock (this.RuntimeData.Mutex)
				{
					tempCompDataDict = this.CompSyncDataCache.CompDataDict.V;
					tempCompRankJXLDict = this.CompSyncDataCache.CompRankJunXianLastDict.V;
				}
				CompData myCompData = new CompData();
				if (client.ClientData.CompType > 0)
				{
					tempCompDataDict.TryGetValue(client.ClientData.CompType, out myCompData.kfCompData);
					myCompData.kfCompData = (KFCompData)myCompData.kfCompData.Clone();
					myCompData.kfCompData.StrongholdDict = null;
					myCompData.kfCompData.compBattleBaseData = CompBattleManager.getInstance().GetCompBattleBaseData(client.ClientData.CompType);
					CompBattleGameStates state = CompBattleGameStates.None;
					CompBattleManager.getInstance().CheckCondition(client, ref state);
					myCompData.kfCompData.CompBattleStates = (int)state;
					KFCompRoleData myCompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
					if (myCompRoleData != null && null != myCompData.kfCompData)
					{
						myCompData.kfCompData.SelfJunXian = myCompRoleData.JunXian;
						myCompData.kfCompData.CompTypeBattle = myCompRoleData.CompTypeBattle;
						if (myCompRoleData.CompType > 0 && myCompRoleData.CompType != client.ClientData.CompType)
						{
							client.ClientData.CompType = myCompRoleData.CompType;
							GameManager.ClientMgr.SetCompType(client, client.ClientData.CompType);
							return false;
						}
						int BattleJiFen = GameManager.ClientMgr.GetCompBattleJiFenValue(client);
						if (BattleJiFen != myCompRoleData.BattleJiFen)
						{
							int modScore = myCompRoleData.BattleJiFen - BattleJiFen;
							GameManager.ClientMgr.ModifyCompBattleJiFenValue(client, modScore, "势力战KF", true, true, false);
						}
						int MineJiFen = GameManager.ClientMgr.GetCompMineJiFenValue(client);
						if (MineJiFen != myCompRoleData.MineJiFen)
						{
							int modScore = myCompRoleData.MineJiFen - MineJiFen;
							GameManager.ClientMgr.ModifyCompMineJiFenValue(client, modScore, "势力矿洞KF", true, true, false);
						}
					}
					else if (myCompRoleData == null && myCompData.kfCompData != null && client.ClientData.CompType > 0)
					{
						int result = TianTiClient.getInstance().Comp_JoinComp_Repair(client.ClientData.RoleID, client.ClientData.ZoneID, client.ClientData.RoleName, client.ClientData.CompType, GameManager.ClientMgr.GetCompBattleJiFenValue(client));
						if (result >= 0)
						{
							myCompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
							if (null != myCompRoleData)
							{
								myCompData.kfCompData.SelfJunXian = myCompRoleData.JunXian;
								myCompData.kfCompData.CompTypeBattle = myCompRoleData.CompTypeBattle;
							}
						}
					}
				}
				for (int loop = 1; loop <= 3; loop++)
				{
					KFCompData tempCompData = null;
					tempCompDataDict.TryGetValue(loop, out tempCompData);
					if (null != tempCompData)
					{
						myCompData.YestdBoomValueList.Add(tempCompData.YestdBoomValue);
					}
					else
					{
						myCompData.YestdBoomValueList.Add(0);
					}
				}
				myCompData.SelectData.RecommendCompList = this.ComputerRecommendCompList(myCompData.YestdBoomValueList);
				for (int compLoop = 1; compLoop <= 3; compLoop++)
				{
					List<KFCompRankInfo> rankInfo = null;
					if (tempCompRankJXLDict.TryGetValue(compLoop, out rankInfo) && rankInfo != null && rankInfo.Count != 0)
					{
						KFCompRoleData roleData = TianTiClient.getInstance().Comp_GetCompRoleData(rankInfo[0].Key);
						if (roleData != null && roleData.CompType != compLoop)
						{
							myCompData.SelectData.DaLingZhuNameList.Add("");
						}
						else
						{
							myCompData.SelectData.DaLingZhuNameList.Add(rankInfo[0].Param1);
						}
					}
					else
					{
						myCompData.SelectData.DaLingZhuNameList.Add("");
					}
				}
				client.sendCmd<CompData>(nID, myCompData, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool ProcessCompJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int toCompType = Global.SafeConvertToInt32(cmdParams[1]);
				if (toCompType < 1 || toCompType > 3)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}", result, toCompType), false);
					return true;
				}
				if (!this.CheckMap(client))
				{
					result = -21;
					client.sendCmd(nID, string.Format("{0}:{1}", result, toCompType), false);
					return true;
				}
				if (toCompType == client.ClientData.CompType)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}", result, toCompType), false);
					return true;
				}
				List<int> YestdBoomValueList = this.GetYestdBoomValueList();
				if (this.ComputerRecommendCompList(YestdBoomValueList).Contains(toCompType))
				{
					if (!Global.CanAddGoodsNum(client, this.RuntimeData.CompRecommend.Items.Count))
					{
						result = -100;
						client.sendCmd(nID, string.Format("{0}:{1}", result, toCompType), false);
						return true;
					}
				}
				bool changeCompType = false;
				if (client.ClientData.CompType > 0)
				{
					changeCompType = true;
					CompBattleGameStates state = CompBattleGameStates.None;
					CompBattleManager.getInstance().CheckCondition(client, ref state);
					if (state != CompBattleGameStates.None)
					{
						result = -12;
						client.sendCmd(nID, string.Format("{0}:{1}", result, toCompType), false);
						return true;
					}
					CompMineManager.getInstance().CheckCondition(client, ref state);
					if (state != CompBattleGameStates.None)
					{
						result = -12;
						client.sendCmd(nID, string.Format("{0}:{1}", result, toCompType), false);
						return true;
					}
					if (client.ClientData.UserMoney < this.RuntimeData.CompReplaceNeed)
					{
						result = -10;
						client.sendCmd(nID, string.Format("{0}:{1}", result, toCompType), false);
						return true;
					}
				}
				result = TianTiClient.getInstance().Comp_JoinComp(client.ClientData.RoleID, client.ClientData.ZoneID, client.ClientData.RoleName, toCompType);
				if (result < 0)
				{
					client.sendCmd(nID, string.Format("{0}:{1}", result, toCompType), false);
					return true;
				}
				client.ClientData.CompType = toCompType;
				GameManager.ClientMgr.SetCompType(client, toCompType);
				if (changeCompType)
				{
					GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.RuntimeData.CompReplaceNeed, "势力争霸切换势力", true, true, false, DaiBiSySType.None);
					int donate = GameManager.ClientMgr.GetCompDonateValue(client);
					int subvalue = (int)((double)donate * this.RuntimeData.CompReplaceAmerce) - donate;
					GameManager.ClientMgr.ModifyCompDonateValue(client, subvalue, "势力争霸切换势力", true, true, false);
				}
				if (this.ComputerRecommendCompList(YestdBoomValueList).Contains(toCompType))
				{
					foreach (AwardsItemData item in this.RuntimeData.CompRecommend.Items)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "势力争霸加入势力", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
					}
				}
				this.HandleCompTaskSomething(client, false);
				Global.SaveRoleParamsStringToDB(client, "49", "", true);
				client.sendCmd(nID, string.Format("{0}:{1}", result, toCompType), false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool ProcessCompRankInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int rankType = Global.SafeConvertToInt32(cmdParams[1]);
				if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
				{
					return true;
				}
				Dictionary<int, List<KFCompRankInfo>> tempCompRankJXDict = null;
				Dictionary<int, List<KFCompRankInfo>> tempCompRankJXLDict = null;
				lock (this.RuntimeData.Mutex)
				{
					tempCompRankJXDict = this.CompSyncDataCache.CompRankJunXianDict.V;
					tempCompRankJXLDict = this.CompSyncDataCache.CompRankJunXianLastDict.V;
				}
				KFCompRoleData compRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (null == compRoleData)
				{
					return true;
				}
				List<KFCompRankInfo> compRankInfo = null;
				if (rankType == 1)
				{
					tempCompRankJXDict.TryGetValue(client.ClientData.CompType, out compRankInfo);
				}
				else if (rankType == 2)
				{
					tempCompRankJXLDict.TryGetValue(client.ClientData.CompType, out compRankInfo);
				}
				if (null == compRankInfo)
				{
					compRankInfo = new List<KFCompRankInfo>();
				}
				List<KFCompRankInfo> rankInfo2Client = new List<KFCompRankInfo>(compRankInfo);
				if (null != rankInfo2Client)
				{
					if (rankType == 1)
					{
						rankInfo2Client.Add(new KFCompRankInfo
						{
							Key = client.ClientData.RoleID,
							Value = compRoleData.JunXian,
							Param1 = Global.FormatRoleNameWithZoneId2(client)
						});
					}
					else if (rankType == 2)
					{
						rankInfo2Client.Add(new KFCompRankInfo
						{
							Key = client.ClientData.RoleID,
							Value = compRoleData.JunXianLast,
							Param1 = Global.FormatRoleNameWithZoneId2(client)
						});
					}
				}
				client.sendCmd<List<KFCompRankInfo>>(nID, rankInfo2Client, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool ProcessCompSetBulletinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				string bulletin = cmdParams[1];
				if (client.ClientData.CompZhiWu != 1)
				{
					result = -12;
					client.sendCmd<int>(nID, result, false);
					return true;
				}
				result = NameServerNamager.CheckInvalidCharacters(bulletin, false);
				if (result < 0)
				{
					client.sendCmd<int>(nID, result, false);
					return true;
				}
				result = TianTiClient.getInstance().Comp_SetBulletin(client.ClientData.CompType, bulletin);
				if (result < 0)
				{
					client.sendCmd<int>(nID, result, false);
					return true;
				}
				KFCompData myCompData = null;
				lock (this.RuntimeData.Mutex)
				{
					this.CompSyncDataCache.CompDataDict.V.TryGetValue(client.ClientData.CompType, out myCompData);
				}
				if (null != myCompData)
				{
					myCompData.Bulletin = bulletin;
				}
				client.sendCmd<int>(nID, result, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool ProcessCompSetEnemyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int enemyCompType = Global.SafeConvertToInt32(cmdParams[1]);
				if (enemyCompType < 1 || enemyCompType > 3)
				{
					return true;
				}
				if (client.ClientData.CompType <= 0 || client.ClientData.CompZhiWu <= 0 || enemyCompType == client.ClientData.CompType)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}", result, enemyCompType), false);
					return true;
				}
				List<CompLevelConfig> tempCompLevelConfigList = null;
				lock (this.RuntimeData.Mutex)
				{
					tempCompLevelConfigList = this.RuntimeData.CompLevelConfigList;
				}
				CompLevelConfig levelConfig = tempCompLevelConfigList.Find((CompLevelConfig x) => x.CompID == client.ClientData.CompType && x.Level == (int)client.ClientData.CompZhiWu);
				if (null == levelConfig)
				{
					result = -3;
					client.sendCmd(nID, string.Format("{0}:{1}", result, enemyCompType), false);
					return true;
				}
				if (0 == levelConfig.Enemy)
				{
					result = -12;
					client.sendCmd(nID, string.Format("{0}:{1}", result, enemyCompType), false);
					return true;
				}
				TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 3, enemyCompType, 0);
				client.sendCmd(nID, string.Format("{0}:{1}", result, enemyCompType), false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool ProcessCompZhiWuCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int compType = Global.SafeConvertToInt32(cmdParams[1]);
				if (compType < 1 || compType > 3)
				{
					return true;
				}
				List<KFCompRankInfo> rankInfoList = new List<KFCompRankInfo>();
				lock (this.RuntimeData.Mutex)
				{
					this.CompSyncDataCache.CompRankJunXianLastDict.V.TryGetValue(compType, out rankInfoList);
				}
				CompZhiWuData myZhiWuData = new CompZhiWuData();
				if (null != rankInfoList)
				{
					List<KFCompRankInfo> rankInfoList2Client = new List<KFCompRankInfo>(rankInfoList);
					if (rankInfoList2Client.Count > 5)
					{
						rankInfoList2Client = rankInfoList2Client.GetRange(0, 5);
					}
					foreach (KFCompRankInfo item in rankInfoList2Client)
					{
						KFCompRoleData roleData = TianTiClient.getInstance().Comp_GetCompRoleData(item.Key);
						if (roleData == null || roleData.CompType != compType)
						{
							myZhiWuData.CompRoleData.Add(new KFCompRoleData());
						}
						else
						{
							myZhiWuData.CompRoleData.Add(roleData);
						}
					}
				}
				client.sendCmd<CompZhiWuData>(nID, myZhiWuData, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool ProcessCompEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int toMapCode = Global.SafeConvertToInt32(cmdParams[0]);
				int toPosX = 0;
				int toPosY = 0;
				int teleportId = 0;
				int toBoss = 0;
				if (cmdParams.Length >= 2)
				{
					toPosX = Global.SafeConvertToInt32(cmdParams[1]);
				}
				if (cmdParams.Length >= 3)
				{
					toPosY = Global.SafeConvertToInt32(cmdParams[2]);
				}
				if (cmdParams.Length >= 4)
				{
					teleportId = Global.SafeConvertToInt32(cmdParams[3]);
				}
				if (cmdParams.Length >= 5)
				{
					toBoss = Global.SafeConvertToInt32(cmdParams[4]);
				}
				Dictionary<int, CompMapData> tempCompMapDataDict = null;
				List<int> tempServerLineList = null;
				Dictionary<int, CompConfig> tempCompConfigDict = null;
				lock (this.RuntimeData.Mutex)
				{
					tempServerLineList = this.CompSyncDataCache.ServerLineList;
					tempCompConfigDict = this.RuntimeData.CompConfigDict;
					tempCompMapDataDict = this.CompSyncDataCache.CompMapDataDict;
				}
				CompConfig toCompConfig = null;
				int toCompType = 0;
				bool validCompMap = false;
				bool currentCompMap = false;
				foreach (CompConfig item in tempCompConfigDict.Values)
				{
					if (item.MapCode == toMapCode)
					{
						toCompType = item.ID;
						validCompMap = true;
						toCompConfig = item;
					}
					if (item.MapCode == client.ClientData.MapCode)
					{
						currentCompMap = true;
					}
				}
				if (!validCompMap)
				{
					result = -12;
					client.sendCmd<int>(nID, result, false);
					return true;
				}
				if (client.ClientData.CompType <= 0)
				{
					result = -12;
				}
				else if (!Global.CanEnterMap(client, toMapCode) || toMapCode == client.ClientData.MapCode)
				{
					result = -12;
				}
				else if (!currentCompMap && !this.CheckMap(client) && !KuaFuMapManager.getInstance().IsKuaFuMap(client.ClientData.MapCode))
				{
					result = -21;
				}
				else
				{
					if (toPosX > 0 && toPosY > 0)
					{
						if (Global.InObs(ObjectTypes.OT_CLIENT, toMapCode, toPosX, toPosY, 0, 0))
						{
							result = -12;
							goto IL_48A;
						}
					}
					int fromMapCode = client.ClientData.MapCode;
					if (teleportId > 0)
					{
						GameMap fromGameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(fromMapCode, out fromGameMap))
						{
							result = -3;
							goto IL_48A;
						}
						MapTeleport mapTeleport = null;
						if (!fromGameMap.MapTeleportDict.TryGetValue(teleportId, out mapTeleport) || mapTeleport.ToMapID != toMapCode)
						{
							result = -12;
							goto IL_48A;
						}
						if (Global.GetTwoPointDistance(client.CurrentPos, new Point((double)mapTeleport.X, (double)mapTeleport.Y)) > 800.0)
						{
							result = -301;
							goto IL_48A;
						}
					}
					CompMapData compMapData = null;
					if (tempCompMapDataDict == null || !tempCompMapDataDict.TryGetValue(toMapCode, out compMapData) || compMapData.roleNum >= toCompConfig.MaxPlayer)
					{
						result = -22;
					}
					else if (tempServerLineList == null || tempServerLineList.Count < 3)
					{
						result = -11003;
					}
					else
					{
						int kuaFuServerId = tempServerLineList[toCompType - 1];
						KuaFuServerInfo kfserverInfo = null;
						if (!KuaFuManager.getInstance().TryGetValue(kuaFuServerId, out kfserverInfo))
						{
							result = -11000;
						}
						else
						{
							bool flag2 = 0 == 0;
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
							if (null != clientKuaFuServerLoginData)
							{
								clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
								clientKuaFuServerLoginData.GameId = (long)toMapCode;
								clientKuaFuServerLoginData.EndTicks = 0L;
								clientKuaFuServerLoginData.ServerId = client.ServerId;
								clientKuaFuServerLoginData.ServerIp = kfserverInfo.Ip;
								clientKuaFuServerLoginData.ServerPort = kfserverInfo.Port;
								if (toCompType == 1)
								{
									clientKuaFuServerLoginData.GameType = 27;
								}
								else if (toCompType == 2)
								{
									clientKuaFuServerLoginData.GameType = 28;
								}
								else if (toCompType == 3)
								{
									clientKuaFuServerLoginData.GameType = 29;
								}
							}
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							Global.SaveRoleParamsIntListToDB(client, new List<int>(new int[]
							{
								fromMapCode,
								teleportId,
								toBoss,
								toPosX,
								toPosY
							}), "EnterKuaFuMapFlag", true);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
					}
				}
				IL_48A:
				client.sendCmd<int>(nID, result, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public bool ProcessGetCompAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompScene scene = client.SceneObject as CompScene;
				if (null == scene)
				{
					return true;
				}
				RoleData4Selector OwnerRoleData = null;
				this.OwnerRoleDataDict.TryGetValue(scene.m_nMapCode, out OwnerRoleData);
				int roleID = Convert.ToInt32(cmdParams[0]);
				client.sendCmd<CompDaLingZhuShowData>(nID, new CompDaLingZhuShowData
				{
					AdmireCount = Global.GetCompAdmireCount(client),
					RoleData4Selector = OwnerRoleData
				}, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessCompAdmireCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompScene scene = client.SceneObject as CompScene;
				if (null == scene)
				{
					return true;
				}
				RoleData4Selector OwnerRoleData = null;
				this.OwnerRoleDataDict.TryGetValue(scene.m_nMapCode, out OwnerRoleData);
				int roleID = Convert.ToInt32(cmdParams[0]);
				int type = Convert.ToInt32(cmdParams[1]);
				MoBaiData MoBaiConfig = null;
				string strcmd;
				if (!Data.MoBaiDataInfoList.TryGetValue(7, out MoBaiConfig))
				{
					strcmd = string.Format("{0}", -2);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (client.ClientData.ChangeLifeCount < MoBaiConfig.MinZhuanSheng || (client.ClientData.ChangeLifeCount == MoBaiConfig.MinZhuanSheng && client.ClientData.Level < MoBaiConfig.MinLevel))
				{
					strcmd = string.Format("{0}", -2);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				int nRealyNum = MoBaiConfig.AdrationMaxLimit;
				int AdmireCount = Global.GetCompAdmireCount(client);
				if (OwnerRoleData != null && client.ClientData.RoleID == OwnerRoleData.RoleID)
				{
					nRealyNum += MoBaiConfig.ExtraNumber;
				}
				int nVIPLev = client.ClientData.VipLevel;
				int[] nArrayVIPAdded = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPMoBaiNum", ',');
				if (nVIPLev > VIPEumValue.VIPENUMVALUE_MAXLEVEL || nArrayVIPAdded.Length < 1)
				{
					strcmd = string.Format("{0}", -2);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				nRealyNum += nArrayVIPAdded[nVIPLev];
				if (AdmireCount >= nRealyNum)
				{
					strcmd = string.Format("{0}", -3);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				double nRate;
				if (client.ClientData.ChangeLifeCount == 0)
				{
					nRate = 1.0;
				}
				else
				{
					nRate = Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
				}
				if (type == 1)
				{
					if (!Global.SubBindTongQianAndTongQian(client, MoBaiConfig.NeedJinBi, "膜拜势力争霸大领主"))
					{
						strcmd = string.Format("{0}", -4);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
					long nExp = (long)(nRate * (double)MoBaiConfig.JinBiExpAward);
					if (nExp > 0L)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, nExp, true, true, false, "none");
					}
					if (MoBaiConfig.JinBiZhanGongAward > 0)
					{
						GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.JinBiZhanGongAward, AddBangGongTypes.CompMoBai, 0);
					}
					if (MoBaiConfig.LingJingAwardByJinBi > 0)
					{
						GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByJinBi, "膜拜势力争霸大领主", true, true, false);
					}
					if (MoBaiConfig.ShenLiJingHuaByJinBi > 0)
					{
						GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, MoBaiConfig.ShenLiJingHuaByJinBi, "膜拜势力争霸大领主", true, true);
					}
				}
				else if (type == 2)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, MoBaiConfig.NeedZuanShi, "膜拜势力争霸大领主", true, true, false, DaiBiSySType.None))
					{
						strcmd = string.Format("{0}", -5);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
					int nExp2 = (int)(nRate * (double)MoBaiConfig.ZuanShiExpAward);
					if (nExp2 > 0)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, (long)nExp2, true, true, false, "none");
					}
					if (MoBaiConfig.ZuanShiZhanGongAward > 0)
					{
						GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref MoBaiConfig.ZuanShiZhanGongAward, AddBangGongTypes.CompMoBai, 0);
					}
					if (MoBaiConfig.LingJingAwardByZuanShi > 0)
					{
						GameManager.ClientMgr.ModifyMUMoHeValue(client, MoBaiConfig.LingJingAwardByZuanShi, "膜拜势力争霸大领主", true, true, false);
					}
					if (MoBaiConfig.ShenLiJingHuaByZuanShi > 0)
					{
						GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, MoBaiConfig.ShenLiJingHuaByZuanShi, "膜拜势力争霸大领主", true, true);
					}
				}
				Global.ProcessIncreaseCompAdmireCount(client);
				strcmd = string.Format("{0}", 1);
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				string valueString = Global.GetRoleParamsFromDBByRoleID(roleId, "10203", 0);
				int compType = Global.SafeConvertToInt32(valueString);
				if (compType > 0)
				{
					TianTiClient.getInstance().Comp_ChangeName(roleId, newName);
				}
			}
		}

		
		public void OnStartPlayGame(GameClient client)
		{
			this.UpdateMapBuffer(client);
			List<int> enterFlags = Global.GetRoleParamsIntListFromDB(client, "EnterKuaFuMapFlag");
			if (enterFlags != null && enterFlags.Count >= 5)
			{
				Global.SaveRoleParamsIntListToDB(client, new List<int>(new int[]
				{
					enterFlags[0],
					0,
					0,
					0,
					0
				}), "EnterKuaFuMapFlag", true);
			}
		}

		
		private void UpdateMapBuffer(GameClient client)
		{
			int CompEnemy = 0;
			CompMapClientContextData contextData = client.SceneContextData2 as CompMapClientContextData;
			if (contextData != null && client.ClientData.CompType > 0 && client.ClientData.CompType == contextData.BattleWhichSide)
			{
				KFCompData myCompData = null;
				lock (this.RuntimeData.Mutex)
				{
					this.CompSyncDataCache.CompDataDict.V.TryGetValue(client.ClientData.CompType, out myCompData);
					CompEnemy = ((myCompData != null) ? myCompData.EnemyCompType : 0);
				}
			}
			if (CompEnemy < 1 || CompEnemy > 3)
			{
				double[] array = new double[1];
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.CompEnemy, actionParams, 1, false);
			}
			else
			{
				double[] actionParams = new double[]
				{
					(double)this.RuntimeData.CompEnemy[CompEnemy - 1]
				};
				Global.UpdateBufferData(client, BufferItemTypes.CompEnemy, actionParams, 1, false);
			}
		}

		
		public int FilterCompEnemyInjure(GameClient client, GameClient enemy, int injure)
		{
			CompMapClientContextData contextData = client.SceneContextData2 as CompMapClientContextData;
			int result;
			if (contextData == null || client.ClientData.CompType <= 0 || client.ClientData.CompType != contextData.BattleWhichSide)
			{
				result = injure;
			}
			else
			{
				KFCompData myCompData = null;
				lock (this.RuntimeData.Mutex)
				{
					this.CompSyncDataCache.CompDataDict.V.TryGetValue(client.ClientData.CompType, out myCompData);
				}
				if (myCompData == null || myCompData.EnemyCompType == 0 || enemy.ClientData.CompType != myCompData.EnemyCompType)
				{
					result = injure;
				}
				else
				{
					result = (int)((double)injure * (1.0 + this.RuntimeData.CompEnemyHurtNum));
				}
			}
			return result;
		}

		
		private int GetZhiWuByRankJunXianLast(int compType, int rid)
		{
			List<KFCompRankInfo> rankInfoList = new List<KFCompRankInfo>();
			lock (this.RuntimeData.Mutex)
			{
				this.CompSyncDataCache.CompRankJunXianLastDict.V.TryGetValue(compType, out rankInfoList);
			}
			int result;
			if (rankInfoList == null || rankInfoList.Count == 0)
			{
				result = 0;
			}
			else
			{
				int zhiwuIdx = rankInfoList.FindIndex((KFCompRankInfo x) => x.Key == rid) + 1;
				result = ((zhiwuIdx > 5) ? 0 : zhiwuIdx);
			}
			return result;
		}

		
		public void UpdateBuff4GameClient(GameClient client, BufferItemTypes bufferItem, int bufferGoodsID, bool add)
		{
			EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(bufferGoodsID);
			if (null != item)
			{
				if (add)
				{
					double[] actionParams = new double[]
					{
						(double)bufferGoodsID
					};
					Global.UpdateBufferData(client, bufferItem, actionParams, 1, true);
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.BufferByGoodsProps,
						bufferGoodsID,
						item.ExtProps
					});
				}
				else
				{
					Global.RemoveBufferData(client, (int)bufferItem);
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.BufferByGoodsProps,
						bufferGoodsID,
						PropsCacheManager.ConstExtProps
					});
				}
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.RecalcProps,
					DelayExecProcIds.NotifyRefreshProps
				});
			}
		}

		
		private void UpdateChengHaoBuffer(GameClient client)
		{
			if (client.ClientData.CompType > 0)
			{
				List<KFCompRankInfo> RankBossDamageList = null;
				lock (this.RuntimeData.Mutex)
				{
					RankBossDamageList = this.CompSyncDataCache.CompRankBossDamageList.V;
				}
				int CompZhiWu = this.GetZhiWuByRankJunXianLast(client.ClientData.CompType, client.ClientData.RoleID);
				this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_1, 2000817, false);
				this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_2, 2000818, false);
				this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_3, 2000819, false);
				if (client.ClientData.CompType > 0 && RankBossDamageList.Count >= 3)
				{
					if (RankBossDamageList[0].Value == client.ClientData.RoleID)
					{
						this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_1, 2000817, true);
					}
					if (RankBossDamageList[1].Value == client.ClientData.RoleID)
					{
						this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_2, 2000818, true);
					}
					if (RankBossDamageList[2].Value == client.ClientData.RoleID)
					{
						this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_3, 2000819, true);
					}
				}
				if (client.ClientData.CompType > 0 && CompZhiWu != (int)client.ClientData.CompZhiWu)
				{
					client.ClientData.CompZhiWu = (byte)CompZhiWu;
					client.sendCmd(1135, string.Format("{0}:{1}", client.ClientData.RoleID, CompZhiWu), false);
				}
				Global.RemoveBufferData(client, 9003);
				Global.RemoveBufferData(client, 9004);
				Global.RemoveBufferData(client, 9005);
				Global.RemoveBufferData(client, 9006);
				Global.RemoveBufferData(client, 9007);
				Global.RemoveBufferData(client, 9008);
				Global.RemoveBufferData(client, 9009);
				Global.RemoveBufferData(client, 9010);
				Global.RemoveBufferData(client, 9011);
				if (CompZhiWu > 0 && client.ClientData.CompType > 0)
				{
					double[] bufferParams = new double[]
					{
						1.0
					};
					if (client.ClientData.CompType == 1)
					{
						if (CompZhiWu == 1)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_1_1, bufferParams, 1, false);
						}
						if (CompZhiWu == 2)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_1_2, bufferParams, 1, false);
						}
						if (CompZhiWu == 3)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_1_3, bufferParams, 1, false);
						}
					}
					if (client.ClientData.CompType == 2)
					{
						if (CompZhiWu == 1)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_2_1, bufferParams, 1, false);
						}
						if (CompZhiWu == 2)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_2_2, bufferParams, 1, false);
						}
						if (CompZhiWu == 3)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_2_3, bufferParams, 1, false);
						}
					}
					if (client.ClientData.CompType == 3)
					{
						if (CompZhiWu == 1)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_3_1, bufferParams, 1, false);
						}
						if (CompZhiWu == 2)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_3_2, bufferParams, 1, false);
						}
						if (CompZhiWu == 3)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_3_3, bufferParams, 1, false);
						}
					}
				}
			}
		}

		
		public bool NoticeCoolDown(CompScene scene, int noticeID)
		{
			lock (this.RuntimeData.Mutex)
			{
				CoolDownItem coolDownItem = null;
				if (!scene.CompNoticeCoolDownDict.TryGetValue(noticeID, out coolDownItem))
				{
					return true;
				}
				long ticks = TimeUtil.NOW();
				if (ticks > coolDownItem.StartTicks + coolDownItem.CDTicks)
				{
					return true;
				}
			}
			return false;
		}

		
		public void AddNoticeCoolDown(CompScene scene, int noticeID)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompNoticeConfig noticeConfig = null;
				if (this.RuntimeData.CompNoticeConfigDict.TryGetValue(noticeID, out noticeConfig))
				{
					CoolDownItem coolDownItem = null;
					scene.CompNoticeCoolDownDict.TryGetValue(noticeID, out coolDownItem);
					long nowTicks = TimeUtil.NOW();
					long cdTicks = (long)(noticeConfig.CDTime * 1000);
					if (null == coolDownItem)
					{
						coolDownItem = new CoolDownItem
						{
							ID = noticeID,
							StartTicks = nowTicks,
							CDTicks = cdTicks
						};
						scene.CompNoticeCoolDownDict[noticeID] = coolDownItem;
					}
					else if (nowTicks + cdTicks > coolDownItem.StartTicks + coolDownItem.CDTicks)
					{
						coolDownItem.StartTicks = nowTicks;
						coolDownItem.CDTicks = cdTicks;
					}
				}
			}
		}

		
		public void CompChat(GameClient client, string text)
		{
			long nowTicks = TimeUtil.NOW();
			List<CompLevelConfig> tempCompLevelConfigList = null;
			lock (this.RuntimeData.Mutex)
			{
				tempCompLevelConfigList = this.RuntimeData.CompLevelConfigList;
			}
			CompLevelConfig levelConfig = tempCompLevelConfigList.Find((CompLevelConfig x) => x.CompID == client.ClientData.CompType && x.Level == (int)client.ClientData.CompZhiWu);
			if (null != levelConfig)
			{
				int talkCD = Math.Max(levelConfig.TalkCD * 1000, 3000);
				if (talkCD > 0 && nowTicks - client.ClientData.LastCompChatTicks < (long)talkCD)
				{
					long secs = ((long)talkCD - (nowTicks - client.ClientData.LastCompChatTicks)) / 1000L + 1L;
					GameManager.ClientMgr.NotifyHintMsg(client, string.Format(GLang.GetLang(4003, new object[0]), secs));
				}
				else
				{
					client.ClientData.LastCompChatTicks = nowTicks;
					KFCompChat chat = new KFCompChat(client.ClientData.ZoneID, client.ClientData.RoleName, text, client.ClientData.CompType);
					lock (this.RuntimeData.Mutex)
					{
						this.RuntimeData.CompChatList.Add(chat);
					}
					this.BroadcastCompChatMsg(chat);
				}
			}
		}

		
		public void BroadcastCompChatMsg(KFCompChat kfChat)
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				if (client != null && client.ClientData.CompType == kfChat.CompType)
				{
					client.sendCmd(157, kfChat.Text, false);
				}
			}
		}

		
		public void OnChatListData(byte[] data)
		{
			if (null != data)
			{
				List<KFCompChat> chatList = DataHelper.BytesToObject<List<KFCompChat>>(data, 0, data.Length);
				if (null != chatList)
				{
					foreach (KFCompChat kfChat in chatList)
					{
						this.BroadcastCompChatMsg(kfChat);
					}
				}
			}
		}

		
		public void CompNotice(CompScene scene, KFCompNotice notice)
		{
			if (this.NoticeCoolDown(scene, notice.NoticeID))
			{
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.CompNoticeList.Add(notice);
				}
				this.BroadcastCompNoticeMsg(notice);
				this.AddNoticeCoolDown(scene, notice.NoticeID);
			}
		}

		
		public void OnNoticeListData(byte[] data)
		{
			if (null != data)
			{
				List<KFCompNotice> noticeList = DataHelper.BytesToObject<List<KFCompNotice>>(data, 0, data.Length);
				if (null != noticeList)
				{
					foreach (KFCompNotice kfNotice in noticeList)
					{
						this.BroadcastCompNoticeMsg(kfNotice);
					}
				}
			}
		}

		
		public void BroadcastCompNoticeMsg(KFCompNotice kfNotice)
		{
			CompNoticeConfig noticeConfig = null;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.CompNoticeConfigDict.TryGetValue(kfNotice.NoticeID, out noticeConfig))
				{
					return;
				}
			}
			if (!GameManager.IsKuaFuServer)
			{
				bool OriginalBroadCast = false;
				if (noticeConfig.OriginalMapOpen != null && noticeConfig.OriginalMapOpen.Contains(1))
				{
					OriginalBroadCast = true;
				}
				if (!OriginalBroadCast)
				{
					return;
				}
			}
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				if (noticeConfig.Range > 0)
				{
					client.sendCmd<KFCompNotice>(1131, kfNotice, false);
				}
				else if ((kfNotice.CompType > 0 && client.ClientData.CompType == kfNotice.CompType) || kfNotice.CompType <= 0)
				{
					client.sendCmd<KFCompNotice>(1131, kfNotice, false);
				}
			}
		}

		
		public int GetDayDuiHuanNum(GameClient client, int DuiHuanNum)
		{
			int result;
			if (client.ClientData.CompType <= 0)
			{
				result = DuiHuanNum;
			}
			else
			{
				List<int> YestdBoomValueList = this.GetYestdBoomValueList();
				int myBoomVal = YestdBoomValueList[client.ClientData.CompType - 1];
				int maxTimes = 1;
				foreach (Tuple<int, int> item in this.RuntimeData.CompShop)
				{
					if (myBoomVal >= item.Item1 && item.Item2 > maxTimes)
					{
						maxTimes = item.Item2;
					}
				}
				result = maxTimes * DuiHuanNum;
			}
			return result;
		}

		
		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			CompScene scene;
			if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
			{
				CompResourcesConfig monsterItem = null;
				lock (this.RuntimeData.Mutex)
				{
					monsterItem = (monster.Tag as CompResourcesConfig);
					if (monsterItem == null)
					{
						return;
					}
					monster.Tag = null;
					DateTime now = TimeUtil.NowDateTime();
					if (now.TimeOfDay >= monsterItem.RefreshTimeBegin && now.TimeOfDay <= monsterItem.RefreshTimeEnd)
					{
						scene.ResourceGrowUpNum--;
						monsterItem.ResourceState = 1;
						this.AddDelayCreateMonster(scene, TimeUtil.NOW(), monsterItem);
					}
					else
					{
						scene.ResourceNum--;
						scene.ResourceGrowUpNum--;
						monsterItem.ResourceState = 0;
					}
				}
				if (monsterItem.CompDonate > 0)
				{
					GameManager.ClientMgr.ModifyCompDonateValue(client, monsterItem.CompDonate, "采集", true, true, false);
				}
				if (monsterItem.JunXian > 0)
				{
					TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 1, client.ClientData.RoleID, monsterItem.JunXian);
					string broadMsg = string.Format(GLang.GetLang(4017, new object[0]), monsterItem.JunXian);
					GameManager.ClientMgr.NotifyImportantMsg(client, broadMsg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				}
				if (monsterItem.BoomValue > 0)
				{
					TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 0, monsterItem.BoomValue, 0);
					TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 6, monsterItem.BoomValue, 0);
					string broadMsg = string.Format(GLang.GetLang(4018, new object[0]), monsterItem.BoomValue);
					GameManager.ClientMgr.NotifyImportantMsg(client, broadMsg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					if (scene.CompSceneInfo.ID != client.ClientData.CompType)
					{
						TianTiClient.getInstance().Comp_CompOpt(scene.CompSceneInfo.ID, 2, client.ClientData.CompType, monsterItem.BoomValue);
						KFCompNotice noticeInfo = new KFCompNotice
						{
							NoticeID = 2,
							CompType = scene.CompSceneInfo.ID,
							Param1 = Global.FormatRoleNameWithZoneId2(client),
							toMapCode = scene.m_nMapCode,
							toPosX = client.ClientData.PosX,
							toPosY = client.ClientData.PosY
						};
						this.CompNotice(scene, noticeInfo);
					}
				}
				scene.SaveCompSceneDBInfo();
			}
		}

		
		public void OnProcessBossRealive(Monster monster)
		{
			if (401 == monster.MonsterType)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompScene scene;
					if (this.SceneDict.TryGetValue(monster.MonsterZoneNode.MapCode, out scene))
					{
						foreach (CompMapClientContextData item in scene.ClientContextDataDict.Values)
						{
							item.TotalScore = 0L;
							item.InjureBossDeltaDict.Remove(monster.MonsterInfo.ExtensionID);
						}
						MonsterZone monsterZone = GameManager.MonsterZoneMgr.GetDynamicMonsterZone(monster.MonsterZoneNode.MapCode);
						if (monsterZone != null && this.RuntimeData.CompBossRealive != null && this.RuntimeData.CompBossRealive.Length == 2)
						{
							Monster seedMonster = GameManager.MonsterZoneMgr.GetDynamicMonsterSeed(monster.MonsterInfo.ExtensionID);
							if (seedMonster != null && monster.LastDeadTicks > 0L)
							{
								MonsterStaticInfo cloneMonsterInfo = seedMonster.MonsterInfo.Clone();
								double curMaxLifeFactor = monster.MonsterInfo.VLifeMax / cloneMonsterInfo.VLifeMax;
								double tempNumForTest = curMaxLifeFactor;
								long aliveTicks = monster.LastDeadTicks - monster.GetMonsterBirthTick();
								long bossDeadTicks = (long)this.RuntimeData.CompBossRealive[0] * 10000000L;
								if (aliveTicks > bossDeadTicks)
								{
									curMaxLifeFactor = Math.Max(curMaxLifeFactor * 0.9, 1.0);
								}
								else
								{
									curMaxLifeFactor += (double)(bossDeadTicks - aliveTicks) / 10000000.0 * this.RuntimeData.CompBossRealive[1];
								}
								string logForTest = string.Format("势力Boss刷新 势力ID:{0} Before血量:{1} Before系数:{2} 存活时间:{3}s After系数:{4} After血量:{5}", new object[]
								{
									scene.CompSceneInfo.ID,
									monster.MonsterInfo.VLifeMax,
									tempNumForTest,
									aliveTicks / 10000000L,
									curMaxLifeFactor,
									monster.MonsterInfo.VLifeMax * curMaxLifeFactor
								});
								LogManager.WriteLog(LogTypes.Analysis, logForTest, null, true);
								cloneMonsterInfo.VLifeMax *= curMaxLifeFactor;
								monster.MonsterInfo = cloneMonsterInfo;
								scene.BossMaxLifeFactor = curMaxLifeFactor;
								scene.SaveCompSceneDBInfo();
							}
							else if (null != seedMonster)
							{
								MonsterStaticInfo cloneMonsterInfo = seedMonster.MonsterInfo.Clone();
								cloneMonsterInfo.VLifeMax *= scene.BossMaxLifeFactor;
								monster.MonsterInfo = cloneMonsterInfo;
								string logForTest = string.Format("势力Boss刷新 势力ID:{0} 血量:{1} 系数:{2}", scene.CompSceneInfo.ID, monster.MonsterInfo.VLifeMax, scene.BossMaxLifeFactor);
								LogManager.WriteLog(LogTypes.Analysis, logForTest, null, true);
							}
						}
						scene.ScoreData.Score1 = 0L;
						scene.ScoreData.Score2 = 0L;
						scene.ScoreData.Score3 = 0L;
						scene.ScoreData.BossMaxLifeV = (long)monster.MonsterInfo.VLifeMax;
						List<object> objsList = GameManager.ClientMgr.GetMapClients(scene.m_nMapCode);
						if (objsList != null && objsList.Count != 0)
						{
							for (int i = 0; i < objsList.Count; i++)
							{
								GameClient c = objsList[i] as GameClient;
								if (c != null)
								{
									this.NotifyScoreInfo(c, true, true);
								}
							}
						}
					}
				}
			}
		}

		
		public void OnKillRole(GameClient client, GameClient other)
		{
			CompScene scene;
			if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
			{
				lock (this.RuntimeData.Mutex)
				{
					if (other.ClientData.CompType == scene.CompSceneInfo.ID)
					{
						KFCompNotice noticeInfo = new KFCompNotice
						{
							NoticeID = 3,
							CompType = scene.CompSceneInfo.ID,
							Param1 = Global.FormatRoleNameWithZoneId2(client),
							Param2 = Global.FormatRoleNameWithZoneId2(other),
							toMapCode = scene.m_nMapCode,
							toPosX = client.ClientData.PosX,
							toPosY = client.ClientData.PosY
						};
						this.CompNotice(scene, noticeInfo);
					}
					if (other.ClientData.CompZhiWu == 1)
					{
						if (other.ClientData.CompType == scene.CompSceneInfo.ID)
						{
							KFCompNotice noticeInfo = new KFCompNotice
							{
								NoticeID = 4,
								CompType = scene.CompSceneInfo.ID,
								Param1 = Global.FormatRoleNameWithZoneId2(client),
								Param2 = Global.FormatRoleNameWithZoneId2(other),
								toMapCode = scene.m_nMapCode,
								toPosX = client.ClientData.PosX,
								toPosY = client.ClientData.PosY
							};
							this.CompNotice(scene, noticeInfo);
						}
						KFCompNotice noticeInfo2 = new KFCompNotice
						{
							NoticeID = 6,
							CompType = client.ClientData.CompType,
							Param1 = Global.FormatRoleNameWithZoneId2(client),
							Param2 = Global.FormatRoleNameWithZoneId2(other),
							toMapCode = scene.m_nMapCode,
							toPosX = client.ClientData.PosX,
							toPosY = client.ClientData.PosY
						};
						this.CompNotice(scene, noticeInfo2);
					}
				}
			}
		}

		
		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			CompScene scene;
			if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
			{
				lock (this.RuntimeData.Mutex)
				{
					if (401 == monster.MonsterType && monster.MonsterInfo.ExtensionID == scene.CompSceneInfo.BossID)
					{
						int boomDiv = this.RuntimeData.CompBossCompNum[0];
						int boomMax = this.RuntimeData.CompBossCompNum[1];
						int donateDiv = this.RuntimeData.CompBossCompHonor[0];
						int junxianDiv = this.RuntimeData.CompBossCompHonor[1];
						int donateMax = this.RuntimeData.CompBossCompHonor[2];
						int junxianMax = this.RuntimeData.CompBossCompHonor[3];
						List<Tuple<int, long>> RoleInjureBossList = new List<Tuple<int, long>>();
						foreach (CompMapClientContextData item in scene.ClientContextDataDict.Values)
						{
							long damage = 0L;
							if (item.InjureBossDeltaDict.TryGetValue(monster.MonsterInfo.ExtensionID, out damage) && damage > 0L)
							{
								RoleInjureBossList.Add(new Tuple<int, long>(item.RoleId, damage));
								long boomAdd = Math.Min(damage / (long)boomDiv, (long)boomMax);
								long donateAdd = Math.Min(damage / (long)donateDiv, (long)donateMax);
								long junxianAdd = Math.Min(damage / (long)junxianDiv, (long)junxianMax);
								if (boomAdd > 0L)
								{
									TianTiClient.getInstance().Comp_CompOpt(item.BattleWhichSide, 0, (int)boomAdd, 0);
									TianTiClient.getInstance().Comp_CompOpt(item.BattleWhichSide, 5, (int)boomAdd, 0);
									if (scene.CompSceneInfo.ID != item.BattleWhichSide)
									{
										TianTiClient.getInstance().Comp_CompOpt(scene.CompSceneInfo.ID, 2, item.BattleWhichSide, (int)boomAdd);
									}
								}
								GameClient attackerClient = GameManager.ClientMgr.FindClient(item.RoleId);
								if (null != attackerClient)
								{
									if (donateAdd > 0L)
									{
										GameManager.ClientMgr.ModifyCompDonateValue(attackerClient, (int)donateAdd, "Boss", true, true, false);
									}
									if (junxianAdd > 0L)
									{
										TianTiClient.getInstance().Comp_CompOpt(attackerClient.ClientData.CompType, 1, item.RoleId, (int)junxianAdd);
										string broadMsg = string.Format(GLang.GetLang(4017, new object[0]), junxianAdd);
										GameManager.ClientMgr.NotifyImportantMsg(attackerClient, broadMsg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									}
								}
							}
						}
						TianTiClient.getInstance().Comp_CompOpt(scene.CompSceneInfo.ID, 4, client.ClientData.CompType, 0);
						RoleInjureBossList.Sort(delegate(Tuple<int, long> left, Tuple<int, long> righit)
						{
							int result;
							if (left.Item2 < righit.Item2)
							{
								result = 1;
							}
							else if (left.Item2 > righit.Item2)
							{
								result = -1;
							}
							else
							{
								result = 0;
							}
							return result;
						});
						if (RoleInjureBossList.Count != 0)
						{
							TianTiClient.getInstance().Comp_CompOpt(scene.CompSceneInfo.ID, 7, RoleInjureBossList[0].Item1, 0);
						}
						KFCompNotice noticeInfo = new KFCompNotice
						{
							NoticeID = 7,
							CompType = scene.CompSceneInfo.ID,
							Param1 = scene.CompSceneInfo.CompName,
							toMapCode = scene.m_nMapCode,
							toPosX = client.ClientData.PosX,
							toPosY = client.ClientData.PosY
						};
						this.CompNotice(scene, noticeInfo);
					}
					CompSolderSiteConfig siteConfig = monster.Tag as CompSolderSiteConfig;
					if (null != siteConfig)
					{
						monster.Tag = null;
						siteConfig.MonsterState = 0;
						scene.SolderNum--;
						scene.SaveCompSceneDBInfo();
					}
				}
			}
		}

		
		public CompMapClientContextData GetBossTopDamageClientContext(Monster monster)
		{
			CompMapClientContextData clientContext = null;
			CompScene scene;
			CompMapClientContextData result;
			if (!this.SceneDict.TryGetValue(monster.MonsterZoneNode.MapCode, out scene))
			{
				result = clientContext;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					List<Tuple<int, long>> RoleInjureBossList = new List<Tuple<int, long>>();
					foreach (CompMapClientContextData item in scene.ClientContextDataDict.Values)
					{
						long damage = 0L;
						if (item.InjureBossDeltaDict.TryGetValue(monster.MonsterInfo.ExtensionID, out damage) && damage > 0L)
						{
							RoleInjureBossList.Add(new Tuple<int, long>(item.RoleId, damage));
						}
					}
					RoleInjureBossList.Sort(delegate(Tuple<int, long> left, Tuple<int, long> righit)
					{
						int result2;
						if (left.Item2 < righit.Item2)
						{
							result2 = 1;
						}
						else if (left.Item2 > righit.Item2)
						{
							result2 = -1;
						}
						else
						{
							result2 = 0;
						}
						return result2;
					});
					if (RoleInjureBossList.Count != 0)
					{
						scene.ClientContextDataDict.TryGetValue(RoleInjureBossList[0].Item1, out clientContext);
					}
				}
				result = clientContext;
			}
			return result;
		}

		
		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			CompResourcesConfig tag = (monster != null) ? (monster.Tag as CompResourcesConfig) : null;
			int result;
			if (tag == null)
			{
				result = -200;
			}
			else if (client.ClientData.CompType <= 0)
			{
				result = -12;
			}
			else
			{
				bool IfResourceGrowUp = false;
				long nowMs = TimeUtil.NOW();
				if (nowMs >= monster.GetMonsterBirthTick() / 10000L + (long)(tag.GrowTime * 1000))
				{
					IfResourceGrowUp = true;
				}
				if (tag.ResourceState == 2)
				{
					IfResourceGrowUp = true;
				}
				if (!IfResourceGrowUp)
				{
					result = -2007;
				}
				else
				{
					result = tag.CollectTime;
				}
			}
			return result;
		}

		
		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (401 == monster.MonsterType)
			{
				CompMapClientContextData contextData = client.SceneContextData2 as CompMapClientContextData;
				if (null != contextData)
				{
					CompScene scene;
					if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
					{
						lock (this.RuntimeData.Mutex)
						{
							long InjureBossDelta = 0L;
							contextData.InjureBossDeltaDict.TryGetValue(monster.MonsterInfo.ExtensionID, out InjureBossDelta);
							InjureBossDelta += injure;
							contextData.InjureBossDeltaDict[monster.MonsterInfo.ExtensionID] = InjureBossDelta;
							contextData.TotalScore += injure;
							if (client.ClientData.CompType == 1)
							{
								scene.ScoreData.Score1 += injure;
							}
							else if (client.ClientData.CompType == 2)
							{
								scene.ScoreData.Score2 += injure;
							}
							else if (client.ClientData.CompType == 3)
							{
								scene.ScoreData.Score3 += injure;
							}
						}
						List<object> objsList = GameManager.ClientMgr.GetMapClients(scene.m_nMapCode);
						if (objsList != null && objsList.Count != 0)
						{
							for (int i = 0; i < objsList.Count; i++)
							{
								GameClient c = objsList[i] as GameClient;
								if (c != null)
								{
									c.sendCmd<CompBattleScoreData>(1133, scene.ScoreData, false);
								}
							}
						}
						this.NotifyScoreInfo(client, true, true);
						if (scene.CompSceneInfo.ID != client.ClientData.CompType)
						{
							KFCompNotice noticeInfo = new KFCompNotice
							{
								NoticeID = 5,
								CompType = scene.CompSceneInfo.ID,
								Param1 = Global.FormatRoleNameWithZoneId2(client),
								toMapCode = scene.m_nMapCode,
								toPosX = client.ClientData.PosX,
								toPosY = client.ClientData.PosY
							};
							this.CompNotice(scene, noticeInfo);
						}
					}
				}
			}
		}

		
		public void NotifyScoreInfo(GameClient client, bool sideScore = true, bool selfScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompScene scene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
				{
					if (sideScore)
					{
						client.sendCmd<CompBattleScoreData>(1133, scene.ScoreData, false);
					}
					if (selfScore)
					{
						CompMapClientContextData clientContextData = client.SceneContextData2 as CompMapClientContextData;
						if (null != clientContextData)
						{
							client.sendCmd<long>(1134, clientContextData.TotalScore, false);
						}
					}
				}
			}
		}

		
		public void HandleCompTaskSomething(GameClient client, bool login = false)
		{
			if (client.ClientData.CompType > 0)
			{
				List<TaskData> AbandonTaskList = new List<TaskData>();
				foreach (TaskData task in client.ClientData.TaskDataList)
				{
					SystemXmlItem systemTask = null;
					if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(task.DoingTaskID, out systemTask))
					{
						int taskClass = systemTask.GetIntValue("TaskClass", -1);
						if (taskClass >= 100 && taskClass <= 150)
						{
							int taskCompType = systemTask.GetIntValue("CompID", -1);
							if (taskCompType > 0 && client.ClientData.CompType != taskCompType)
							{
								AbandonTaskList.Add(task);
							}
							string taskDateTime = new DateTime(task.AddDateTime * 10000L).ToString("yyyy-MM-dd");
							DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, taskClass);
							if (dailyTaskData == null || string.Compare(dailyTaskData.RecTime, taskDateTime) > 0)
							{
								AbandonTaskList.Add(task);
							}
						}
					}
				}
				foreach (TaskData task in AbandonTaskList)
				{
					bool b = Global.CancelTask(client, task.DbID, task.DoingTaskID);
					if (!login && b)
					{
						client.sendCmd(154, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.RoleID,
							task.DbID,
							task.DoingTaskID,
							0
						}), false);
					}
				}
				Dictionary<int, List<int>> paoHuanTaskIDDict = new Dictionary<int, List<int>>();
				List<int> taskIdList = new List<int>();
				foreach (int key in GameManager.SystemTasksMgr.SystemXmlItemDict.Keys)
				{
					SystemXmlItem systemTask = GameManager.SystemTasksMgr.SystemXmlItemDict[key];
					int taskID = systemTask.GetIntValue("ID", -1);
					if (-1 != taskID)
					{
						int taskClass = systemTask.GetIntValue("TaskClass", -1);
						if (taskClass >= 100 && taskClass <= 150)
						{
							if (Global.CanTaskPaoHuanTask(client, taskClass))
							{
								List<int> paoHuanTaskIDList = null;
								if (!paoHuanTaskIDDict.TryGetValue(taskClass, out paoHuanTaskIDList))
								{
									paoHuanTaskIDList = new List<int>();
									paoHuanTaskIDDict[taskClass] = paoHuanTaskIDList;
								}
								paoHuanTaskIDList.Add(taskID);
							}
						}
					}
				}
				foreach (KeyValuePair<int, List<int>> kvp in paoHuanTaskIDDict)
				{
					DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, kvp.Key);
					if (null != dailyTaskData)
					{
						List<int> beginList = null;
						lock (this.RuntimeData.Mutex)
						{
							this.RuntimeData.CompTaskBeginDict.TryGetValue(kvp.Key, out beginList);
						}
						if (beginList != null && beginList.Count >= 3)
						{
							int taskID = beginList[client.ClientData.CompType - 1] + dailyTaskData.RecNum;
							if (kvp.Value.Contains(taskID))
							{
								TCPOutPacket tcpOutPacketTemp = null;
								TCPProcessCmdResults result = Global.TakeNewTask(TCPManager.getInstance(), client.ClientSocket, TCPManager.getInstance().tcpClientPool, TCPManager.getInstance().tcpRandKey, TCPManager.getInstance().TcpOutPacketPool, 125, client, client.ClientData.RoleID, taskID, -1, out tcpOutPacketTemp);
								if (!login && result == TCPProcessCmdResults.RESULT_DATA && null != tcpOutPacketTemp)
								{
									client.sendCmd(tcpOutPacketTemp, true);
								}
							}
						}
					}
				}
			}
		}

		
		private bool CheckMap(GameClient client)
		{
			SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return sceneType == SceneUIClasses.Normal;
		}

		
		private List<int> ComputerRecommendCompList(List<int> YestdBoomValueList)
		{
			List<int> validCompList = new List<int>();
			List<int> result;
			if (YestdBoomValueList == null || YestdBoomValueList.Count == 0)
			{
				result = validCompList;
			}
			else
			{
				int MaxBoomValue = YestdBoomValueList.Max();
				if (0 == MaxBoomValue)
				{
					result = validCompList;
				}
				else
				{
					for (int compLoop = 1; compLoop <= 3; compLoop++)
					{
						if ((double)YestdBoomValueList[compLoop - 1] < (double)MaxBoomValue * this.RuntimeData.CompRecommendRatio)
						{
							validCompList.Add(compLoop);
						}
					}
					result = validCompList;
				}
			}
			return result;
		}

		
		public bool IfInMyselfCompMap(GameClient client)
		{
			CompScene scene = client.SceneObject as CompScene;
			bool result;
			if (null == scene)
			{
				result = false;
			}
			else
			{
				SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				result = (sceneType == SceneUIClasses.Comp && scene.CompSceneInfo.ID == client.ClientData.CompType);
			}
			return result;
		}

		
		public bool IfTopBoomCompType(GameClient client, int compType, bool self = true)
		{
			bool result;
			if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
			{
				result = false;
			}
			else if (compType < 1 || compType > 3)
			{
				result = false;
			}
			else
			{
				List<int> YestdBoomValueList = this.GetYestdBoomValueList();
				int BoomValue = YestdBoomValueList[compType - 1];
				for (int loop = 0; loop < YestdBoomValueList.Count; loop++)
				{
					if (self || loop + 1 != client.ClientData.CompType)
					{
						if (BoomValue < YestdBoomValueList[loop])
						{
							return false;
						}
					}
				}
				result = true;
			}
			return result;
		}

		
		public int GetTaskClassNum()
		{
			int count;
			lock (this.RuntimeData.Mutex)
			{
				count = this.RuntimeData.MaxDailyTaskNumDict.Count;
			}
			return count;
		}

		
		public int GetMaxDailyTaskNum(int taskClass)
		{
			int TaskNum = 0;
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.MaxDailyTaskNumDict.TryGetValue(taskClass, out TaskNum);
			}
			return TaskNum;
		}

		
		public bool ClientRelive(GameClient client)
		{
			int toPosX;
			int toPosY;
			int side = this.GetBirthPoint(client.SceneObject as CompScene, client, out toPosX, out toPosY);
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

		
		public int GetBirthPoint(CompScene sceneInfo, GameClient client, out int posX, out int posY)
		{
			int side = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				CompConfig compConfig = (sceneInfo != null) ? sceneInfo.CompSceneInfo : null;
				if (null != compConfig)
				{
					Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, compConfig.BirthPosX[side - 1], compConfig.BirthPosY[side - 1], compConfig.BirthRadius[side - 1]);
					posX = (int)newPos.X;
					posY = (int)newPos.Y;
					return side;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		
		public bool OnInitGameKuaFu(GameClient client)
		{
			client.ClientData.CompType = GameManager.ClientMgr.GetCompType(client);
			bool result;
			if (client.ClientData.CompType <= 0)
			{
				result = false;
			}
			else
			{
				CompConfig myCompConfig = null;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.CompConfigDict.TryGetValue(client.ClientData.CompType, out myCompConfig))
					{
						return false;
					}
				}
				KuaFuServerLoginData kuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
				client.ClientData.MapCode = (int)kuaFuServerLoginData.GameId;
				client.ClientData.BattleWhichSide = client.ClientData.CompType;
				CompScene scene;
				if (!this.SceneDict.TryGetValue(client.ClientData.MapCode, out scene))
				{
					result = false;
				}
				else
				{
					bool toBirthPoint = false;
					List<int> enterFlags = Global.GetRoleParamsIntListFromDB(client, "EnterKuaFuMapFlag");
					if (enterFlags != null && enterFlags.Count >= 5)
					{
						int fromMapCode = enterFlags[0];
						int fromTeleport = enterFlags[1];
						int targetBossId = enterFlags[2];
						int toPosX = enterFlags[3];
						int toPosY = enterFlags[4];
						if (myCompConfig.MapCode == client.ClientData.MapCode)
						{
							if (toPosX > 0 && toPosY > 0)
							{
								Point pos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, toPosX, toPosY, 60);
								client.ClientData.PosX = (int)pos.X;
								client.ClientData.PosY = (int)pos.Y;
							}
							else if (targetBossId > 0)
							{
								int bossX;
								int bossY;
								int radis;
								if (GameManager.MonsterZoneMgr.GetMonsterBirthPoint(client.ClientData.MapCode, targetBossId, out bossX, out bossY, out radis))
								{
									radis = 1;
									Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, bossX, bossY, radis);
									client.ClientData.PosX = (int)newPos.X;
									client.ClientData.PosY = (int)newPos.Y;
								}
							}
							else
							{
								toBirthPoint = true;
							}
						}
						else
						{
							toBirthPoint = true;
						}
					}
					if (toBirthPoint)
					{
						int _posx = 0;
						int _posy = 0;
						if (this.GetBirthPoint(scene, client, out _posx, out _posy) <= 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("找不到出生点mapcode={0},side={1}", client.ClientData.MapCode, client.ClientData.BattleWhichSide), null, true);
							return false;
						}
						client.ClientData.PosX = _posx;
						client.ClientData.PosY = _posy;
					}
					lock (this.RuntimeData.Mutex)
					{
						CompMapClientContextData clientContextData;
						if (!scene.ClientContextDataDict.TryGetValue(client.ClientData.RoleID, out clientContextData))
						{
							clientContextData = new CompMapClientContextData();
							scene.ClientContextDataDict[client.ClientData.RoleID] = clientContextData;
						}
						clientContextData.RoleId = client.ClientData.RoleID;
						clientContextData.ServerId = client.ServerId;
						clientContextData.BattleWhichSide = client.ClientData.BattleWhichSide;
						clientContextData.RoleName = client.ClientData.RoleName;
						clientContextData.Occupation = client.ClientData.Occupation;
						clientContextData.RoleSex = client.ClientData.RoleSex;
						clientContextData.ZoneID = client.ClientData.ZoneID;
						client.SceneContextData2 = clientContextData;
						client.SceneObject = scene;
					}
					result = true;
				}
			}
			return result;
		}

		
		public void OnInitGameAhead(GameClient client)
		{
			client.ClientData.CompType = GameManager.ClientMgr.GetCompType(client);
			client.ClientData.CompZhiWu = (byte)this.GetZhiWuByRankJunXianLast(client.ClientData.CompType, client.ClientData.RoleID);
		}

		
		public void OnInitGame(GameClient client)
		{
			this.HandleCompTaskSomething(client, true);
			this.UpdateChengHaoBuffer(client);
		}

		
		private void TimerProc(object sender, EventArgs e)
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					CompSyncData SyncData = TianTiClient.getInstance().Comp_SyncData(this.CompSyncDataCache.CompDataDict.Age, this.CompSyncDataCache.CompRankJunXianDict.Age, this.CompSyncDataCache.CompRankJunXianLastDict.Age, this.CompSyncDataCache.CompRankBossDamageList.Age, this.CompSyncDataCache.CompRankBattleJiFenDict.Age, this.CompSyncDataCache.CompRankMineJiFenDict.Age);
					if (null == SyncData)
					{
						return;
					}
					this.CompSyncDataCache.ServerLineList = SyncData.ServerLineList;
					if (null != SyncData.BytesCompMapDataDict)
					{
						this.CompSyncDataCache.CompMapDataDict = DataHelper2.BytesToObject<Dictionary<int, CompMapData>>(SyncData.BytesCompMapDataDict, 0, SyncData.BytesCompMapDataDict.Length);
					}
					if (this.CompSyncDataCache.CompRankBattleJiFenDict.Age != SyncData.CompRankBattleJiFenDict.Age)
					{
						this.CompSyncDataCache.CompBattleJoinRoleNum = SyncData.CompBattleJoinRoleNum;
						if (null == SyncData.BytesCompRankBattleJiFenDict)
						{
							this.CompSyncDataCache.CompRankBattleJiFenDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();
							this.CompSyncDataCache.CompRankBattleJiFenDict.Age = SyncData.CompRankBattleJiFenDict.Age;
						}
						else
						{
							this.CompSyncDataCache.CompRankBattleJiFenDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(SyncData.BytesCompRankBattleJiFenDict, 0, SyncData.BytesCompRankBattleJiFenDict.Length);
						}
					}
					if (this.CompSyncDataCache.CompRankMineJiFenDict.Age != SyncData.CompRankMineJiFenDict.Age)
					{
						this.CompSyncDataCache.CompMineJoinRoleNum = SyncData.CompMineJoinRoleNum;
						if (null == SyncData.BytesCompRankMineJiFenDict)
						{
							this.CompSyncDataCache.CompRankMineJiFenDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();
							this.CompSyncDataCache.CompRankMineJiFenDict.Age = SyncData.CompRankMineJiFenDict.Age;
						}
						else
						{
							this.CompSyncDataCache.CompRankMineJiFenDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(SyncData.BytesCompRankMineJiFenDict, 0, SyncData.BytesCompRankMineJiFenDict.Length);
						}
					}
					if (this.CompSyncDataCache.CompDataDict.Age != SyncData.CompDataDict.Age && null != SyncData.BytesCompDataDict)
					{
						Dictionary<int, KFCompData> OldCompDataDict = new Dictionary<int, KFCompData>(this.CompSyncDataCache.CompDataDict.V);
						this.CompSyncDataCache.CompDataDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, KFCompData>>>(SyncData.BytesCompDataDict, 0, SyncData.BytesCompDataDict.Length);
						bool enemyChg = false;
						foreach (KFCompData item in OldCompDataDict.Values)
						{
							int oldEnemy = item.EnemyCompType;
							KFCompData compData;
							this.CompSyncDataCache.CompDataDict.V.TryGetValue(item.CompType, out compData);
							if (compData != null && compData.EnemyCompType != oldEnemy)
							{
								enemyChg = true;
							}
						}
						if (enemyChg)
						{
							int count = GameManager.ClientMgr.GetMaxClientCount();
							for (int i = 0; i < count; i++)
							{
								GameClient client = GameManager.ClientMgr.FindClientByNid(i);
								if (null != client)
								{
									this.UpdateMapBuffer(client);
								}
							}
						}
					}
					if (this.CompSyncDataCache.CompRankJunXianDict.Age != SyncData.CompRankJunXianDict.Age && null != SyncData.BytesCompRankJunXianDict)
					{
						this.CompSyncDataCache.CompRankJunXianDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(SyncData.BytesCompRankJunXianDict, 0, SyncData.BytesCompRankJunXianDict.Length);
					}
					if (this.CompSyncDataCache.CompRankJunXianLastDict.Age != SyncData.CompRankJunXianLastDict.Age && null != SyncData.BytesCompRankJunXianLastDict)
					{
						this.CompSyncDataCache.CompRankJunXianLastDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(SyncData.BytesCompRankJunXianLastDict, 0, SyncData.BytesCompRankJunXianLastDict.Length);
						int count = GameManager.ClientMgr.GetMaxClientCount();
						for (int i = 0; i < count; i++)
						{
							GameClient client = GameManager.ClientMgr.FindClientByNid(i);
							if (null != client)
							{
								this.UpdateChengHaoBuffer(client);
							}
						}
						if (!GameManager.IsKuaFuServer)
						{
							for (int loop = 1; loop <= 3; loop++)
							{
								List<KFCompRankInfo> rankInfoList = null;
								if (this.CompSyncDataCache.CompRankJunXianLastDict.V.TryGetValue(loop, out rankInfoList) && null != rankInfoList)
								{
									List<KFCompRankInfo> rankInfoList2Client = new List<KFCompRankInfo>(rankInfoList);
									if (rankInfoList2Client.Count > 5)
									{
										rankInfoList2Client = rankInfoList2Client.GetRange(0, 5);
									}
									foreach (KFCompRankInfo item2 in rankInfoList2Client)
									{
										RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, item2.Key), 0);
										if (dbRd != null && dbRd.RoleID > 0)
										{
											RoleData4Selector RoleData4Selector = Global.RoleDataEx2RoleData4Selector(dbRd);
											RoleData4Selector.CompType = loop;
											RoleData4Selector.CompZhiWu = (byte)this.GetZhiWuByRankJunXianLast(loop, item2.Key);
											TianTiClient.getInstance().Comp_SetRoleData4Selector(dbRd.RoleID, DataHelper.ObjectToBytes<RoleData4Selector>(RoleData4Selector));
										}
									}
								}
							}
						}
						else
						{
							this.OnRefreshAllCompNpc(0);
						}
					}
					if (this.CompSyncDataCache.CompRankBossDamageList.Age != SyncData.CompRankBossDamageList.Age && null != SyncData.BytesCompRankBossDamageList)
					{
						List<KFCompRankInfo> OldBossKillerList = new List<KFCompRankInfo>(this.CompSyncDataCache.CompRankBossDamageList.V);
						this.CompSyncDataCache.CompRankBossDamageList = DataHelper2.BytesToObject<KuaFuData<List<KFCompRankInfo>>>(SyncData.BytesCompRankBossDamageList, 0, SyncData.BytesCompRankBossDamageList.Length);
						for (int loop = 0; loop < this.CompSyncDataCache.CompRankBossDamageList.V.Count; loop++)
						{
							int oldBossDamageTop = (OldBossKillerList.Count > loop) ? OldBossKillerList[loop].Value : 0;
							int newBossDamageTop = this.CompSyncDataCache.CompRankBossDamageList.V[loop].Value;
							if (oldBossDamageTop != newBossDamageTop)
							{
								if (oldBossDamageTop > 0)
								{
									GameClient client = GameManager.ClientMgr.FindClient(oldBossDamageTop);
									if (null != client)
									{
										this.UpdateChengHaoBuffer(client);
									}
								}
								if (newBossDamageTop > 0)
								{
									GameClient client = GameManager.ClientMgr.FindClient(newBossDamageTop);
									if (null != client)
									{
										this.UpdateChengHaoBuffer(client);
									}
								}
							}
						}
					}
				}
				CompBattleManager.getInstance().UpdateCompBattleBaseData(this.CompSyncDataCache.CompDataDict.V);
				CompMineManager.getInstance().UpdateCompMineResourceData(this.CompSyncDataCache.CompDataDict.V);
				List<KFCompChat> chatList = null;
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.CompChatList.Count > 0)
					{
						chatList = new List<KFCompChat>(this.RuntimeData.CompChatList);
						this.RuntimeData.CompChatList.Clear();
					}
				}
				if (null != chatList)
				{
					TianTiClient.getInstance().Comp_CompChat(chatList);
				}
				List<KFCompNotice> noticeList = null;
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.CompNoticeList.Count > 0)
					{
						noticeList = new List<KFCompNotice>(this.RuntimeData.CompNoticeList);
						this.RuntimeData.CompNoticeList.Clear();
					}
				}
				if (null != noticeList)
				{
					TianTiClient.getInstance().Comp_BroadCastCompNotice(noticeList);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public void AddDelayCreateMonster(CompScene scene, long ticks, object monster)
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

		
		public void CheckCreateDynamicMonster(CompScene scene, long nowMs)
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
							if (obj is CompResourcesConfig)
							{
								CompResourcesConfig item = obj as CompResourcesConfig;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, item.MonsterID, -1, 1, item.PosX / 100, item.PosY / 100, 0, 0, SceneUIClasses.Comp, item, null);
							}
							else if (obj is CompSolderSiteConfig)
							{
								CompSolderSiteConfig item2 = obj as CompSolderSiteConfig;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, item2.SolderConfig.MonsterID, -1, 1, item2.PosX / 100, item2.PosY / 100, 0, 0, SceneUIClasses.Comp, item2, null);
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

		
		private List<int> GetYestdBoomValueList()
		{
			Dictionary<int, KFCompData> tempCompDataDict = null;
			lock (this.RuntimeData.Mutex)
			{
				tempCompDataDict = this.CompSyncDataCache.CompDataDict.V;
			}
			List<int> YestdBoomValueList = new List<int>();
			for (int loop = 1; loop <= 3; loop++)
			{
				KFCompData tempCompData = null;
				tempCompDataDict.TryGetValue(loop, out tempCompData);
				if (null != tempCompData)
				{
					YestdBoomValueList.Add(tempCompData.YestdBoomValue);
				}
				else
				{
					YestdBoomValueList.Add(0);
				}
			}
			return YestdBoomValueList;
		}

		
		private int GetCompBoomRank(int compType)
		{
			List<int> YestdBoomValueList = this.GetYestdBoomValueList();
			int tarBoomVal = YestdBoomValueList[compType - 1];
			int upperNum = 0;
			foreach (int boomval in YestdBoomValueList)
			{
				if (boomval > tarBoomVal)
				{
					upperNum++;
				}
			}
			return upperNum + 1;
		}

		
		private CompSolderConfig GetCompSolderConfigByCompType(int compType)
		{
			Dictionary<KeyValuePair<int, int>, CompSolderConfig> tempCompSolderConfigDict = null;
			lock (this.RuntimeData.Mutex)
			{
				tempCompSolderConfigDict = this.RuntimeData.CompSolderConfigDict;
			}
			int rank = this.GetCompBoomRank(compType);
			CompSolderConfig solderConfig = null;
			tempCompSolderConfigDict.TryGetValue(new KeyValuePair<int, int>(compType, rank), out solderConfig);
			return solderConfig;
		}

		
		private void InitCreateDynamicMonster(CompScene scene, DateTime now)
		{
			long nowMs = now.Ticks / 10000L;
			Dictionary<int, CompResourcesConfig> tempCompResourcesConfigDict = null;
			List<CompSolderSiteConfig> tempCompSolderSiteConfigList = null;
			lock (this.RuntimeData.Mutex)
			{
				tempCompResourcesConfigDict = this.RuntimeData.CompResourcesConfigDict;
				tempCompSolderSiteConfigList = this.RuntimeData.CompSolderSiteConfigList;
			}
			foreach (CompSolderSiteConfig item in tempCompSolderSiteConfigList)
			{
				scene.CompSolderSiteConfigList.Add(item.Clone() as CompSolderSiteConfig);
			}
			if (scene.ResourceNum > 0)
			{
				int createNum = 0;
				int createGrowUpNum = 0;
				foreach (CompResourcesConfig res in tempCompResourcesConfigDict.Values)
				{
					if (res.MapCodeID == scene.m_nMapCode)
					{
						res.ResourceState = ((createGrowUpNum < scene.ResourceGrowUpNum) ? 2 : 1);
						this.AddDelayCreateMonster(scene, nowMs, res);
						if (2 == res.ResourceState)
						{
							createGrowUpNum++;
						}
						if (++createNum >= scene.ResourceNum)
						{
							break;
						}
					}
				}
			}
		}

		
		public void CheckCreateResource(CompScene scene, DateTime now)
		{
			long nowMs = now.Ticks / 10000L;
			Dictionary<int, CompResourcesConfig> tempCompResourcesConfigDict = null;
			lock (this.RuntimeData.Mutex)
			{
				tempCompResourcesConfigDict = this.RuntimeData.CompResourcesConfigDict;
			}
			bool needSave = false;
			foreach (CompResourcesConfig res in tempCompResourcesConfigDict.Values)
			{
				if (res.MapCodeID == scene.m_nMapCode)
				{
					if (!(now.TimeOfDay < res.RefreshTimeBegin) && !(now.TimeOfDay > res.RefreshTimeEnd))
					{
						if (res.ResourceState == 0)
						{
							res.ResourceState = 1;
							this.AddDelayCreateMonster(scene, nowMs, res);
							scene.ResourceNum++;
							needSave = true;
						}
					}
				}
			}
			List<object> objList = GameManager.MonsterMgr.GetObjectsByMap(scene.m_nMapCode);
			if (null != objList)
			{
				foreach (object item in objList)
				{
					Monster monster = item as Monster;
					if (monster.MonsterType == 1601 && monster.Alive)
					{
						CompResourcesConfig tagInfo = monster.Tag as CompResourcesConfig;
						if (tagInfo != null && tagInfo.ResourceState != 0)
						{
							if (tagInfo.ResourceState == 1 && nowMs >= monster.GetMonsterBirthTick() / 10000L + (long)(tagInfo.GrowTime * 1000))
							{
								tagInfo.ResourceState = 2;
								monster.ResetMonsterBirthTick();
								scene.ResourceGrowUpNum++;
								needSave = true;
							}
							else if (tagInfo.ResourceState == 2 && nowMs >= monster.GetMonsterBirthTick() / 10000L + (long)(tagInfo.AutoCollectTime * 1000))
							{
								scene.ResourceGrowUpNum--;
								needSave = true;
								scene.ResourceNum--;
								needSave = true;
								monster.Tag = null;
								tagInfo.ResourceState = 0;
								GameManager.MonsterMgr.DeadMonsterImmediately(monster);
								if (tagInfo.BoomValue > 0)
								{
									TianTiClient.getInstance().Comp_CompOpt(scene.CompSceneInfo.ID, 0, tagInfo.BoomValue, 0);
									TianTiClient.getInstance().Comp_CompOpt(scene.CompSceneInfo.ID, 6, tagInfo.BoomValue, 0);
								}
							}
						}
					}
				}
			}
			if (needSave)
			{
				scene.SaveCompSceneDBInfo();
			}
		}

		
		public void CheckCreateSolder(CompScene scene, DateTime now)
		{
			long nowMs = now.Ticks / 10000L;
			CompSolderSiteConfig siteConfig = scene.CompSolderSiteConfigList.First<CompSolderSiteConfig>();
			if (now.TimeOfDay < siteConfig.RefreshTimeBegin || now.TimeOfDay > siteConfig.RefreshTimeEnd)
			{
				List<object> objList = GameManager.MonsterMgr.GetObjectsByMap(scene.m_nMapCode);
				if (null != objList)
				{
					foreach (object item in objList)
					{
						Monster monster = item as Monster;
						CompSolderSiteConfig monsterSiteConfig = monster.Tag as CompSolderSiteConfig;
						if (monsterSiteConfig != null && monster.Alive)
						{
							monster.Tag = null;
							monsterSiteConfig.MonsterState = 0;
							GameManager.MonsterMgr.DeadMonsterImmediately(monster);
						}
					}
				}
			}
			else
			{
				List<CompSolderSiteConfig> InitSolderList = scene.CompSolderSiteConfigList.FindAll((CompSolderSiteConfig x) => x.MonsterState == 1);
				if (scene.SolderNum > InitSolderList.Count)
				{
					int createSolderNum = 0;
					foreach (CompSolderSiteConfig solderSite in scene.CompSolderSiteConfigList)
					{
						if (solderSite.MonsterState != 1)
						{
							solderSite.MonsterState = 1;
							solderSite.SolderConfig = this.GetCompSolderConfigByCompType(scene.CompSceneInfo.ID);
							this.AddDelayCreateMonster(scene, nowMs, solderSite);
							if (++createSolderNum >= scene.SolderNum - InitSolderList.Count)
							{
								break;
							}
						}
					}
				}
				int rank = this.GetCompBoomRank(scene.CompSceneInfo.ID);
				int SolderCDSecond = this.RuntimeData.CompSolderCD[rank - 1];
				DateTime RefreshDateTm = new DateTime(scene.SolderRefreshTimeMS * 10000L);
				if (Global.GetOffsetDay(RefreshDateTm) != Global.GetOffsetDay(now))
				{
					RefreshDateTm = now.Date + siteConfig.RefreshTimeBegin;
				}
				if (nowMs >= scene.SolderRefreshTimeMS)
				{
					int refreshNum = (now - RefreshDateTm).Seconds / SolderCDSecond + 1;
					scene.SolderRefreshTimeMS = RefreshDateTm.Ticks / 10000L + (long)(refreshNum * SolderCDSecond * 1000);
					List<CompSolderSiteConfig> randomSolderSiteList = new List<CompSolderSiteConfig>();
					foreach (CompSolderSiteConfig item2 in scene.CompSolderSiteConfigList)
					{
						if (item2.MonsterState == 0)
						{
							randomSolderSiteList.Add(item2);
						}
					}
					if (randomSolderSiteList.Count != 0)
					{
						List<CompSolderSiteConfig> refreshSolderSiteList = Global.RandomSortList<CompSolderSiteConfig>(randomSolderSiteList);
						refreshSolderSiteList = refreshSolderSiteList.GetRange(0, refreshNum);
						foreach (CompSolderSiteConfig solderSite in refreshSolderSiteList)
						{
							solderSite.MonsterState = 1;
							solderSite.SolderConfig = this.GetCompSolderConfigByCompType(scene.CompSceneInfo.ID);
							this.AddDelayCreateMonster(scene, nowMs, solderSite);
							scene.SolderNum++;
						}
					}
					scene.SaveCompSceneDBInfo();
				}
			}
		}

		
		public void CheckSolderWarning(CompScene scene, DateTime now)
		{
			long nowMs = now.Ticks / 10000L;
			List<object> objList = GameManager.MonsterMgr.GetObjectsByMap(scene.m_nMapCode);
			if (null != objList)
			{
				foreach (object item in objList)
				{
					Monster monster = item as Monster;
					CompSolderSiteConfig siteConfig = monster.Tag as CompSolderSiteConfig;
					if (monster.LockFocusTime != 0L && siteConfig != null && monster.Alive)
					{
						if (nowMs - monster.LockFocusTime >= (long)(siteConfig.SolderConfig.AlarmTime * 1000))
						{
							GameClient lockObject = GameManager.ClientMgr.FindClient(monster.LockObject);
							if (null != lockObject)
							{
								KFCompNotice noticeInfo = new KFCompNotice
								{
									NoticeID = 1,
									CompType = scene.CompSceneInfo.ID,
									Param1 = Global.FormatRoleNameWithZoneId2(lockObject),
									toMapCode = scene.m_nMapCode,
									toPosX = lockObject.ClientData.PosX,
									toPosY = lockObject.ClientData.PosY
								};
								this.CompNotice(scene, noticeInfo);
							}
						}
					}
				}
			}
		}

		
		public void CheckMapRoleNum(CompScene scene, DateTime now)
		{
			int roleNum = GameManager.ClientMgr.GetMapClientsCount(scene.m_nMapCode);
			TianTiClient.getInstance().Comp_UpdateMapRoleNum(scene.m_nMapCode, roleNum);
		}

		
		public void TimerProc_fuBenWorker()
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				long nowMs = now.Ticks / 10000L;
				if (this.RuntimeData.NextHeartBeatTicks <= nowMs)
				{
					this.RuntimeData.NextHeartBeatTicks = nowMs + 1020L;
					List<int> ServerLineList = null;
					lock (this.RuntimeData.Mutex)
					{
						ServerLineList = this.CompSyncDataCache.ServerLineList;
					}
					if (ServerLineList.Exists((int x) => x == GameManager.ServerId))
					{
						Dictionary<int, CompConfig> tempCompConfigDict = null;
						lock (this.RuntimeData.Mutex)
						{
							tempCompConfigDict = this.RuntimeData.CompConfigDict;
						}
						for (int compLoop = 1; compLoop <= 3; compLoop++)
						{
							if (ServerLineList[compLoop - 1] == GameManager.ServerId)
							{
								CompConfig compConfig = null;
								if (tempCompConfigDict.TryGetValue(compLoop, out compConfig))
								{
									CompScene sceneData = null;
									if (!this.SceneDict.TryGetValue(compConfig.MapCode, out sceneData))
									{
										sceneData = new CompScene();
										sceneData.m_nMapCode = compConfig.MapCode;
										sceneData.CompSceneInfo = compConfig;
										this.SceneDict[compConfig.MapCode] = sceneData;
										sceneData.LoadCompSceneDBInfo();
										this.InitCreateDynamicMonster(sceneData, now);
									}
								}
							}
						}
						foreach (CompScene scene in this.SceneDict.Values)
						{
							lock (this.RuntimeData.Mutex)
							{
								this.CheckCreateDynamicMonster(scene, nowMs);
								this.CheckCreateResource(scene, now);
								this.CheckCreateSolder(scene, now);
								this.CheckSolderWarning(scene, now);
								this.CheckMapRoleNum(scene, now);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public void RestoreCompNpc(CompConfig compConfig)
		{
			NPC npc = NPCGeneralManager.FindNPC(compConfig.MapCode, compConfig.MoBai);
			if (null != npc)
			{
				npc.ShowNpc = true;
				GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CompDaLingZhu_1 + compConfig.ID - 1, false);
			}
		}

		
		public void ReplaceCompNpc(CompConfig compConfig, RoleData4Selector OwnerRoleData)
		{
			if (null == OwnerRoleData)
			{
				this.RestoreCompNpc(compConfig);
			}
			else
			{
				NPC npc = NPCGeneralManager.FindNPC(compConfig.MapCode, compConfig.MoBai);
				if (null != npc)
				{
					npc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CompDaLingZhu_1 + compConfig.ID - 1, false);
					FakeRoleManager.ProcessNewFakeRole(OwnerRoleData, npc.MapCode, FakeRoleTypes.CompDaLingZhu_1 + compConfig.ID - 1, 4, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, compConfig.MoBai);
				}
			}
		}

		
		public void OnRefreshAllCompNpc(int rid = 0)
		{
			lock (this.RuntimeData.Mutex)
			{
				Dictionary<int, CompConfig> tempCompConfigDict = this.RuntimeData.CompConfigDict;
				Dictionary<int, List<KFCompRankInfo>> tempCompRankJXLDict = this.CompSyncDataCache.CompRankJunXianLastDict.V;
				for (int compLoop = 1; compLoop <= 3; compLoop++)
				{
					CompConfig compConfig = null;
					if (tempCompConfigDict.TryGetValue(compLoop, out compConfig))
					{
						RoleData4Selector OwnerRoleData = null;
						List<KFCompRankInfo> rankInfo = null;
						if (tempCompRankJXLDict.TryGetValue(compLoop, out rankInfo) && rankInfo != null && rankInfo.Count != 0)
						{
							if (rid > 0 && rankInfo[0].Key != rid)
							{
								goto IL_10D;
							}
							int dlzRoleID = rankInfo[0].Key;
							KFCompRoleData myCompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(dlzRoleID);
							if (myCompRoleData != null && null != myCompRoleData.RoleData4Selector)
							{
								OwnerRoleData = DataHelper.BytesToObject<RoleData4Selector>(myCompRoleData.RoleData4Selector, 0, myCompRoleData.RoleData4Selector.Length);
							}
						}
						this.OwnerRoleDataDict[compConfig.MapCode] = OwnerRoleData;
						this.ReplaceCompNpc(compConfig, OwnerRoleData);
					}
					IL_10D:;
				}
			}
		}

		
		public int GetCompShopDuiHuanType(CompShopDHTypeIndex idx)
		{
			int result;
			lock (this.RuntimeData.Mutex)
			{
				result = this.RuntimeData.CompShopDuiHuanType[(int)idx];
			}
			return result;
		}

		
		public bool CheckCanAddJunXian(long LastStartTimeTicks)
		{
			DateTime deadline = TimeUtil.NowDateTime().AddMinutes(5.0);
			DateTime jointm = new DateTime(LastStartTimeTicks * 10000L);
			DateTime weekDead = TimeUtil.GetWeekStartTime(deadline);
			DateTime weekJoin = TimeUtil.GetWeekStartTime(jointm);
			return weekDead.DayOfYear == weekJoin.DayOfYear;
		}

		
		public const SceneUIClasses ManagerType = SceneUIClasses.Comp;

		
		public CompRuntimeData RuntimeData = new CompRuntimeData();

		
		public CompSyncData CompSyncDataCache = new CompSyncData();

		
		public ConcurrentDictionary<int, CompScene> SceneDict = new ConcurrentDictionary<int, CompScene>();

		
		public Dictionary<int, RoleData4Selector> OwnerRoleDataDict = new Dictionary<int, RoleData4Selector>();

		
		private static CompManager instance = new CompManager();
	}
}
