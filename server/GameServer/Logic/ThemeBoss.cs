using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class ThemeBoss : Activity, IManager, IEventListener, ICmdProcessorEx, ICmdProcessor
	{
		
		public static ThemeBoss getInstance()
		{
			return ThemeBoss.instance;
		}

		
		public bool initialize()
		{
			this.InitConfig();
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(910, 1, 1, ThemeBoss.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(10, ThemeBoss.getInstance());
			GlobalEventSource.getInstance().registerListener(11, ThemeBoss.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, ThemeBoss.getInstance());
			GlobalEventSource.getInstance().removeListener(11, ThemeBoss.getInstance());
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
			return nID != 910 || this.ProcessThemeBossStateCmd(client, nID, bytes, cmdParams);
		}

		
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 11)
			{
				MonsterDeadEventObject e = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(e.getAttacker(), e.getMonster());
			}
		}

		
		public bool InitConfig()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(ThemeDataConst.ThemeActivityBoss);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				Dictionary<int, ThemeBossConfig> bossConfigDict = new Dictionary<int, ThemeBossConfig>();
				IEnumerable<XElement> nodes = xml.Elements();
				foreach (XElement xmlItem in nodes)
				{
					ThemeBossConfig config = new ThemeBossConfig();
					string[] maxLevel = Global.GetDefAttributeStr(xmlItem, "MaxLevel", "0").Split(new char[]
					{
						'|'
					});
					if (maxLevel.Length >= 2)
					{
						config.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						config.MonstersID = (int)Global.GetSafeAttributeLong(xmlItem, "MonstersID");
						config.MapCode = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");
						config.PosX = (int)Global.GetSafeAttributeLong(xmlItem, "X");
						config.PosY = (int)Global.GetSafeAttributeLong(xmlItem, "Y");
						config.Radius = (int)Global.GetSafeAttributeLong(xmlItem, "Radius");
						config.Num = (int)Global.GetSafeAttributeLong(xmlItem, "Num");
						config.MaxUnionLevel = Global.GetUnionLevel2(Global.SafeConvertToInt32(maxLevel[0]), Global.SafeConvertToInt32(maxLevel[1]));
						if (!ConfigParser.ParserTimeRangeList(config.TimePoints, xmlItem.Attribute("TimePoints").Value.ToString(), true, '|', '-'))
						{
							throw new Exception(string.Format("读取{0}时间配置(TimePoints)出错", fileName));
						}
						for (int i = 0; i < config.TimePoints.Count; i++)
						{
							TimeSpan ts = new TimeSpan(config.TimePoints[i].Hours, config.TimePoints[i].Minutes, config.TimePoints[i].Seconds);
							config.SecondsOfDay.Add(ts.TotalSeconds);
						}
						bossConfigDict[config.ID] = config;
					}
				}
				List<int> goodsList = new List<int>();
				string goodsStr = GameManager.systemParamsList.GetParamValueByName("ThemeActivityBOSSGoods");
				if (!string.IsNullOrEmpty(goodsStr))
				{
					goodsList = Array.ConvertAll<string, int>(goodsStr.Split(new char[]
					{
						','
					}), (string _x) => Global.SafeConvertToInt32(_x)).ToList<int>();
				}
				lock (ThemeBoss.Mutex)
				{
					this.ThemeBossConfigDict = bossConfigDict;
					this.BroadGoodsIDList = goodsList;
				}
				this.ActivityType = 155;
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				return false;
			}
			return true;
		}

		
		public bool ProcessThemeBossStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int actId = 0;
				int bossState = 0;
				if (this.SceneDict.Count != 0)
				{
					ThemeBossScene scene = this.SceneDict.FirstOrDefault<KeyValuePair<int, ThemeBossScene>>().Value;
					if (scene.State == BattleStates.StartFight)
					{
						if (scene.AliveBossNum > 0)
						{
							bossState = 1;
						}
						else
						{
							bossState = 2;
						}
					}
					else
					{
						bossState = 0;
					}
					actId = scene.BossConfigInfo.ID;
				}
				client.sendCmd(nID, string.Format("{0}:{1}", actId, bossState), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		
		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			if (ThemeBoss.getInstance().IsThemeBoss(monster))
			{
				ThemeBossScene scene = null;
				if (this.SceneDict.TryGetValue(monster.CurrentMapCode, out scene))
				{
					int ownerRoleID = monster.GetAttackerFromList();
					if (ownerRoleID >= 0 && ownerRoleID != client.ClientData.RoleID)
					{
						GameClient findClient = GameManager.ClientMgr.FindClient(ownerRoleID);
						if (null != findClient)
						{
							client = findClient;
						}
					}
					string broadMsg = string.Format(GLang.GetLang(4016, new object[0]), Global.FormatRoleNameWithZoneId2(client));
					Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, broadMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
					scene.AliveBossNum--;
					if (scene.AliveBossNum <= 0)
					{
						scene.State = BattleStates.EndFight;
					}
				}
			}
		}

		
		private void GenerateThemeBossScene()
		{
			int curDayID = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
			if (curDayID != this.SceneDayID)
			{
				this.SceneDict.Clear();
				string strcmd = StringUtil.substitute("{0}:{1}:{2}", new object[]
				{
					0,
					5,
					100
				});
				PaiHangData paiHangData = Global.sendToDB<PaiHangData, string>(269, strcmd, 0);
				if (null != paiHangData)
				{
					long NumOne = 0L;
					long NumTwo = 0L;
					List<PaiHangItemData> PaiHangList = paiHangData.PaiHangList;
					if (null != PaiHangList)
					{
						int DivCalNum = Math.Min(PaiHangList.Count, 100);
						for (int i = 0; i < DivCalNum; i++)
						{
							PaiHangItemData phData = PaiHangList[i];
							NumOne += (long)phData.Val1;
							NumTwo += (long)phData.Val2;
						}
						int TransNum = Global.GetUnionLevel2((int)NumTwo, (int)NumOne) / DivCalNum;
						NumOne = (long)((TransNum - 1) / 100);
						NumTwo = (long)((TransNum - 1) % 100 + 1);
					}
					int curAvgLev = Global.GetUnionLevel2((int)NumTwo, (int)NumOne);
					foreach (ThemeBossConfig config in this.ThemeBossConfigDict.Values)
					{
						if (curAvgLev <= config.MaxUnionLevel)
						{
							ThemeBossScene scene = null;
							if (!this.SceneDict.TryGetValue(config.MapCode, out scene))
							{
								scene = new ThemeBossScene();
								scene.MapCode = config.MapCode;
								scene.BossConfigInfo = config;
								scene.State = BattleStates.NoBattle;
								this.SceneDict[config.MapCode] = scene;
							}
							if (scene.BossConfigInfo.MaxUnionLevel > config.MaxUnionLevel)
							{
								scene.BossConfigInfo = config;
							}
						}
					}
					this.SceneDayID = curDayID;
				}
			}
		}

		
		private bool GetStartEndTime(int sceneId, out long StartTick, out long EndTick)
		{
			StartTick = 0L;
			EndTick = 0L;
			ThemeBossScene sceneItem = null;
			DateTime now = TimeUtil.NowDateTime();
			lock (ThemeBoss.Mutex)
			{
				if (!this.SceneDict.TryGetValue(sceneId, out sceneItem))
				{
					return false;
				}
			}
			lock (ThemeBoss.Mutex)
			{
				for (int i = 0; i < sceneItem.BossConfigInfo.TimePoints.Count - 1; i += 2)
				{
					if (now.TimeOfDay.TotalSeconds >= sceneItem.BossConfigInfo.SecondsOfDay[i] - 180.0 && now.TimeOfDay.TotalSeconds <= sceneItem.BossConfigInfo.SecondsOfDay[i + 1])
					{
						StartTick = now.Date.AddSeconds(sceneItem.BossConfigInfo.SecondsOfDay[i]).Ticks / 10000L;
						EndTick = now.Date.AddSeconds(sceneItem.BossConfigInfo.SecondsOfDay[i + 1]).Ticks / 10000L;
						return true;
					}
				}
			}
			return false;
		}

		
		public bool JudgeCanTriggerActivity(ThemeBossScene scene, DateTime now)
		{
			lock (ThemeBoss.Mutex)
			{
				for (int i = 0; i < scene.BossConfigInfo.TimePoints.Count - 1; i += 2)
				{
					if (now.TimeOfDay.TotalSeconds >= scene.BossConfigInfo.SecondsOfDay[i] - 180.0 && now.TimeOfDay.TotalSeconds <= scene.BossConfigInfo.SecondsOfDay[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		
		public bool IsThemeBossGoods(int goodsID)
		{
			bool result;
			lock (ThemeBoss.Mutex)
			{
				result = this.BroadGoodsIDList.Contains(goodsID);
			}
			return result;
		}

		
		public bool IsThemeBossScene(int mapCode)
		{
			bool result;
			lock (ThemeBoss.Mutex)
			{
				result = this.SceneDict.ContainsKey(mapCode);
			}
			return result;
		}

		
		public bool IsThemeBoss(Monster monster)
		{
			bool result;
			if (401 != monster.MonsterType)
			{
				result = false;
			}
			else
			{
				ThemeBossScene scene = null;
				result = (this.SceneDict.TryGetValue(monster.CurrentMapCode, out scene) && scene.BossConfigInfo.MonstersID == monster.MonsterInfo.ExtensionID);
			}
			return result;
		}

		
		public void TimerProc()
		{
			if (!GameManager.IsKuaFuServer)
			{
				if (155 == this.ActivityType && this.InActivityTime())
				{
					DateTime now = TimeUtil.NowDateTime();
					long nowTicks = now.Ticks / 10000L;
					lock (ThemeBoss.Mutex)
					{
						if (Math.Abs(nowTicks - ThemeBoss.LastHeartBeatTicks) < 1000L)
						{
							return;
						}
						ThemeBoss.LastHeartBeatTicks = nowTicks;
						this.GenerateThemeBossScene();
					}
					foreach (KeyValuePair<int, ThemeBossScene> scenes in this.SceneDict)
					{
						lock (ThemeBoss.Mutex)
						{
							switch (scenes.Value.State)
							{
							case BattleStates.NoBattle:
								if (this.JudgeCanTriggerActivity(scenes.Value, now))
								{
									if (this.GetStartEndTime(scenes.Key, out scenes.Value.StartTick, out scenes.Value.EndTick))
									{
										scenes.Value.State = BattleStates.WaitingFight;
									}
								}
								break;
							case BattleStates.WaitingFight:
								if (nowTicks >= scenes.Value.StartTick)
								{
									ThemeBossConfig BossConfigInfo = scenes.Value.BossConfigInfo;
									Monster bossSeed = GameManager.MonsterZoneMgr.AddDynamicMonsters(scenes.Value.MapCode, BossConfigInfo.MonstersID, -1, BossConfigInfo.Num, BossConfigInfo.PosX / 100, BossConfigInfo.PosY / 100, BossConfigInfo.Radius, 0, SceneUIClasses.Normal, null, null);
									if (null != bossSeed)
									{
										scenes.Value.State = BattleStates.StartFight;
										scenes.Value.AliveBossNum = BossConfigInfo.Num;
										Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, GLang.GetLang(4013, new object[0]), true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
									}
								}
								break;
							case BattleStates.StartFight:
								if (!scenes.Value.BroadCast4014 && (scenes.Value.EndTick - nowTicks) / 1000L <= 180L)
								{
									Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, GLang.GetLang(4014, new object[0]), true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
									scenes.Value.BroadCast4014 = true;
								}
								if (nowTicks >= scenes.Value.EndTick)
								{
									scenes.Value.State = BattleStates.EndFight;
									List<object> monsterList = GameManager.MonsterMgr.GetObjectsByMap(scenes.Value.MapCode);
									foreach (object item in monsterList)
									{
										Monster monster = item as Monster;
										if (monster != null && monster.MonsterInfo.ExtensionID == scenes.Value.BossConfigInfo.MonstersID)
										{
											GameManager.MonsterMgr.DeadMonsterImmediately(monster);
										}
									}
								}
								break;
							case BattleStates.EndFight:
								scenes.Value.State = BattleStates.NoBattle;
								scenes.Value.BroadCast4014 = false;
								scenes.Value.AliveBossNum = 0;
								break;
							}
						}
					}
				}
			}
		}

		
		public const int MaxActiveConditionDataNum = 100;

		
		private static object Mutex = new object();

		
		private static long LastHeartBeatTicks = 0L;

		
		private Dictionary<int, ThemeBossConfig> ThemeBossConfigDict = new Dictionary<int, ThemeBossConfig>();

		
		private List<int> BroadGoodsIDList = new List<int>();

		
		public ConcurrentDictionary<int, ThemeBossScene> SceneDict = new ConcurrentDictionary<int, ThemeBossScene>();

		
		public int SceneDayID;

		
		private static ThemeBoss instance = new ThemeBoss();
	}
}
