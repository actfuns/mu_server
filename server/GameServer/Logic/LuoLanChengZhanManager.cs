using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class LuoLanChengZhanManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx
	{
		
		public static LuoLanChengZhanManager getInstance()
		{
			return LuoLanChengZhanManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(700, 2, 2, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(701, 1, 1, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(702, 2, 2, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(706, 1, 1, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(708, 1, 1, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(709, 1, 1, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1342, 2, 2, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(23, 10000, LuoLanChengZhanManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(24, 10000, LuoLanChengZhanManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(23, 10000, LuoLanChengZhanManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(24, 10000, LuoLanChengZhanManager.getInstance());
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
			case 700:
				return this.ProcessChengZhanJingJiaCmd(client, nID, bytes, cmdParams);
			case 701:
				return this.ProcessGetChengZhanDailyAwardsCmd(client, nID, bytes, cmdParams);
			case 702:
				return this.ProcessLuoLanChengZhanCmd(client, nID, bytes, cmdParams);
			case 703:
			case 704:
			case 705:
			case 707:
				break;
			case 706:
				return this.ProcessGetLuoLanChengZhuInfoCmd(client, nID, bytes, cmdParams);
			case 708:
				return this.ProcessLuoLanChengZhanRequestInfoListCmd(client, nID, bytes, cmdParams);
			case 709:
				return this.ProcessQueryZhanMengZiJinCmd(client, nID, bytes, cmdParams);
			default:
				if (nID == 1342)
				{
					return this.ProcessGetLuoLanKingLooks(client, nID, bytes, cmdParams);
				}
				break;
			}
			return true;
		}

		
		public void processEvent(EventObject eventObject)
		{
			switch (eventObject.getEventType())
			{
			}
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 22:
			{
				PreInstallJunQiEventObject e = eventObject as PreInstallJunQiEventObject;
				if (null != e)
				{
					this.OnPreInstallJunQi(e.Player, e.NPCID);
					eventObject.Handled = true;
				}
				break;
			}
			case 23:
			{
				PreBangHuiAddMemberEventObject e2 = eventObject as PreBangHuiAddMemberEventObject;
				if (null != e2)
				{
					eventObject.Handled = this.OnPreBangHuiAddMember(e2);
				}
				break;
			}
			case 24:
			{
				PreBangHuiRemoveMemberEventObject e3 = eventObject as PreBangHuiRemoveMemberEventObject;
				if (null != e3)
				{
					eventObject.Handled = this.OnPreBangHuiRemoveMember(e3);
				}
				break;
			}
			}
		}

		
		public bool InitConfig()
		{
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.SiegeWarfareEveryDayAwardsDict.Clear();
					fileName = "Config/SiegeWarfareEveryDayAward.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						SiegeWarfareEveryDayAwardsItem item = new SiegeWarfareEveryDayAwardsItem();
						item.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item.ZhiWu = (int)Global.GetSafeAttributeLong(node, "Status");
						item.DayZhanGong = (int)Global.GetSafeAttributeLong(node, "DayZhanGong");
						item.DayExp = Global.GetSafeAttributeLong(node, "DayExp");
						item.DayGoods.AddNoRepeat(Global.GetSafeAttributeStr(node, "DayGoods"));
						if (!this.RuntimeData.SiegeWarfareEveryDayAwardsDict.ContainsKey(item.ZhiWu))
						{
							this.RuntimeData.SiegeWarfareEveryDayAwardsDict.Add(item.ZhiWu, item);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.MapBirthPointListDict.Clear();
					fileName = "Config/SiegeWarfareBirthPoint.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						MapBirthPoint item2 = new MapBirthPoint();
						item2.ID = (int)Global.GetSafeAttributeLong(node, "ID");
						item2.Type = (int)Global.GetSafeAttributeLong(node, "Type");
						item2.MapCode = (int)Global.GetSafeAttributeLong(node, "MapCode");
						item2.BirthPosX = (int)Global.GetSafeAttributeLong(node, "BirthPosX");
						item2.BirthPosY = (int)Global.GetSafeAttributeLong(node, "BirthPosY");
						item2.BirthRangeX = (int)Global.GetSafeAttributeLong(node, "BirthRangeX");
						item2.BirthRangeY = (int)Global.GetSafeAttributeLong(node, "BirthRangeY");
						List<MapBirthPoint> list;
						if (!this.RuntimeData.MapBirthPointListDict.TryGetValue(item2.Type, out list))
						{
							list = new List<MapBirthPoint>();
							this.RuntimeData.MapBirthPointListDict.Add(item2.Type, list);
						}
						list.Add(item2);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					this.RuntimeData.QiZhiBuffOwnerDataList.Clear();
					this.RuntimeData.QiZhiBuffDisableParamsDict.Clear();
					this.RuntimeData.QiZhiBuffEnableParamsDict.Clear();
					fileName = "Config/QiZuoConfig.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement node in nodes)
					{
						QiZhiConfig item3 = new QiZhiConfig();
						item3.NPCID = (int)Global.GetSafeAttributeLong(node, "NPCID");
						item3.BufferID = (int)Global.GetSafeAttributeLong(node, "BufferID");
						item3.PosX = (int)Global.GetSafeAttributeLong(node, "PosX");
						item3.PosY = (int)Global.GetSafeAttributeLong(node, "PosY");
						List<int> useAuthority = Global.StringToIntList(Global.GetSafeAttributeStr(node, "UseAuthority"), ',');
						foreach (int zhiwu in useAuthority)
						{
							item3.UseAuthority.Add(zhiwu);
						}
						this.RuntimeData.NPCID2QiZhiConfigDict[item3.NPCID] = item3;
						this.RuntimeData.QiZhiBuffOwnerDataList.Add(new LuoLanChengZhanQiZhiBuffOwnerData
						{
							NPCID = item3.NPCID,
							OwnerBHName = ""
						});
						this.RuntimeData.QiZhiBuffDisableParamsDict[item3.BufferID] = new double[]
						{
							0.0,
							(double)item3.BufferID
						};
						this.RuntimeData.QiZhiBuffEnableParamsDict[item3.BufferID] = new double[]
						{
							0.0,
							(double)item3.BufferID
						};
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.MapCode = 0;
					this.RuntimeData.MapCode_LongTa = 0;
					QiZhiConfig qiZhiConfig;
					if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(this.RuntimeData.SuperQiZhiNpcId, out qiZhiConfig))
					{
						this.RuntimeData.SuperQiZhiOwnerBirthPosX = qiZhiConfig.PosX;
						this.RuntimeData.SuperQiZhiOwnerBirthPosY = qiZhiConfig.PosY;
					}
					fileName = "Config/SiegeWarfare.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = xml.Elements();
					using (IEnumerator<XElement> enumerator = nodes.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							XElement node = enumerator.Current;
							this.RuntimeData.MapCode = (int)Global.GetSafeAttributeLong(node, "MapCode1");
							this.RuntimeData.MapCode_LongTa = (int)Global.GetSafeAttributeLong(node, "MapCode2");
							this.RuntimeData.GongNengOpenDaysFromKaiFu = (int)Global.GetSafeAttributeLong(node, "KaiFuDay");
							this.RuntimeData.ApplyZhangMengZiJin = (long)((int)Global.GetSafeAttributeLong(node, "ApplyZhangMengZiJin"));
							this.RuntimeData.BidZhangMengZiJin = (long)((int)Global.GetSafeAttributeLong(node, "BidZhangMengZiJin"));
							this.RuntimeData.MaxZhanMengNum = (int)Global.GetSafeAttributeLong(node, "MaxZhanMengNum");
							this.RuntimeData.WeekPoints = Global.String2IntArray(Global.GetSafeAttributeStr(node, "WeekPoints"), '|');
							this.RuntimeData.TimePoints = DateTime.Parse(Global.GetSafeAttributeStr(node, "TimePoints"));
							this.RuntimeData.EnrollTime = Global.GetSafeAttributeLong(node, "EnrollTime");
							this.RuntimeData.MinZhuanSheng = (int)Global.GetSafeAttributeLong(node, "MinZhuanSheng");
							this.RuntimeData.MinLevel = (int)Global.GetSafeAttributeLong(node, "MinLevel");
							this.RuntimeData.MinRequestNum = (int)Global.GetSafeAttributeLong(node, "MinRequestNum");
							this.RuntimeData.MaxEnterNum = (int)Global.GetSafeAttributeLong(node, "MaxEnterNum");
							this.RuntimeData.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(node, "WaitingEnterSecs");
							this.RuntimeData.PrepareSecs = (int)Global.GetSafeAttributeLong(node, "PrepareSecs");
							this.RuntimeData.FightingSecs = (int)Global.GetSafeAttributeLong(node, "FightingSecs");
							this.RuntimeData.ClearRolesSecs = (int)Global.GetSafeAttributeLong(node, "ClearRolesSecs");
							this.RuntimeData.ExpAward = Global.GetSafeAttributeLong(node, "ExpAward");
							this.RuntimeData.ZhanGongAward = (int)Global.GetSafeAttributeLong(node, "ZhanGongAward");
							this.RuntimeData.ZiJin = (int)Global.GetSafeAttributeLong(node, "ZiJin");
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
				try
				{
					fileName = "Config/SiegeWarfareExp.xml";
					this._LevelAwardsMgr.LoadFromXMlFile(fileName, "", "ID", 0);
					this.ParseWeekDaysTimes();
					this.InitLuoLanChengZhuInfo();
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
			}
			return true;
		}

		
		public void LoadDataFromDB()
		{
			int luoLanChengZhuRoleID = 0;
			lock (this.RuntimeData.Mutex)
			{
				BangHuiLingDiItemData lingDiItem = JunQiManager.GetItemByLingDiID(7);
				if (null != lingDiItem)
				{
					this.WangZuBHid = lingDiItem.BHID;
					this.WangZuBHName = (lingDiItem.BHName = this.UpdateWangZuBHNameFromDBServer(lingDiItem.BHID));
					if (lingDiItem.WarRequest != this.RuntimeData.WarRequestStr)
					{
						this.RuntimeData.WarRequstDict = this.GetWarRequstMap(lingDiItem.WarRequest);
						this.RuntimeData.WarRequestStr = lingDiItem.WarRequest;
					}
					BangHuiDetailData bangHuiDetailData = this.GetBangHuiDetailDataAuto(lingDiItem.BHID, -1);
					if (null != bangHuiDetailData)
					{
						luoLanChengZhuRoleID = bangHuiDetailData.BZRoleID;
					}
				}
				else
				{
					this.WangZuBHid = 0;
					this.WangZuBHName = "";
					this.RuntimeData.WarRequstDict = new Dictionary<int, LuoLanChengZhanRequestInfo>();
					this.RuntimeData.WarRequestStr = null;
				}
				this.RuntimeData.LongTaOwnerData.OwnerBHid = this.WangZuBHid;
				this.RuntimeData.LongTaOwnerData.OwnerBHName = this.WangZuBHName;
				this.RuntimeData.LuoLanChengZhuBHID = this.WangZuBHid;
				this.RuntimeData.LuoLanChengZhuBHName = this.WangZuBHName;
				this.ResetBHID2SiteDict();
			}
			this.ReShowLuolanKing(luoLanChengZhuRoleID);
		}

		
		private LuoLanChengZhuInfo GetLuoLanChengZhuInfo(GameClient client)
		{
			int roleID = 0;
			if (null != client)
			{
				roleID = client.ClientData.RoleID;
			}
			LuoLanChengZhuInfo luoLanChengZhuInfo = new LuoLanChengZhuInfo();
			BangHuiLingDiItemData lingDiItem = JunQiManager.GetItemByLingDiID(7);
			LuoLanChengZhuInfo result;
			if (lingDiItem == null || lingDiItem.BHID <= 0)
			{
				result = luoLanChengZhuInfo;
			}
			else
			{
				BangHuiDetailData bangHuiDetailData = this.GetBangHuiDetailDataAuto(lingDiItem.BHID, roleID);
				if (null != bangHuiDetailData)
				{
					luoLanChengZhuInfo.BHID = bangHuiDetailData.BHID;
					luoLanChengZhuInfo.BHName = bangHuiDetailData.BHName;
					luoLanChengZhuInfo.ZoneID = bangHuiDetailData.ZoneID;
					if (null != bangHuiDetailData.MgrItemList)
					{
						foreach (BangHuiMgrItemData item in bangHuiDetailData.MgrItemList)
						{
							if (item.BHZhiwu == 1)
							{
								RoleDataEx rd = this.KingRoleData;
								if (rd != null && rd.RoleID == item.RoleID)
								{
									RoleData4Selector sel = Global.RoleDataEx2RoleData4Selector(rd);
									luoLanChengZhuInfo.RoleInfoList.Add(sel);
									luoLanChengZhuInfo.ZhiWuList.Add(item.BHZhiwu);
								}
							}
						}
					}
				}
				result = luoLanChengZhuInfo;
			}
			return result;
		}

		
		public BangHuiDetailData GetBangHuiDetailDataAuto(int bhid, int roleID = -1)
		{
			BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(roleID, bhid, 0);
			if (null != bangHuiDetailData)
			{
				if (roleID <= 0 && bangHuiDetailData.BZRoleID > 0)
				{
					bangHuiDetailData = Global.GetBangHuiDetailData(bangHuiDetailData.BZRoleID, bhid, 0);
				}
			}
			return bangHuiDetailData;
		}

		
		public void ParseWeekDaysTimes()
		{
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.WeekPoints != null && this.RuntimeData.WeekPoints.Length > 0)
				{
					this.WangChengZhanWeekDaysByConfig = true;
				}
				string wangChengZhanFightingDayTimes_str = string.Format("{0}-{1}", this.RuntimeData.TimePoints.ToString("HH:mm"), this.RuntimeData.TimePoints.AddSeconds((double)this.RuntimeData.FightingSecs).ToString("HH:mm"));
				this.WangChengZhanFightingDayTimes = Global.ParseDateTimeRangeStr(wangChengZhanFightingDayTimes_str);
				this.RuntimeData.NoRequestTimeEnd = this.RuntimeData.TimePoints.AddSeconds((double)this.RuntimeData.FightingSecs).TimeOfDay;
				this.RuntimeData.NoRequestTimeStart = this.RuntimeData.TimePoints.AddSeconds((double)(-(double)this.RuntimeData.EnrollTime)).TimeOfDay;
				this.MaxTakingHuangGongSecs = (int)GameManager.systemParamsList.GetParamValueIntByName("LuoLanHoldTime", -1);
				this.MaxTakingHuangGongSecs *= 1000;
			}
		}

		
		private void InitLuoLanChengZhuInfo()
		{
			this.LoadDataFromDB();
			HuodongCachingMgr.UpdateHeFuWCKingBHID(this.GetWangZuBHid());
			this.NotifyAllWangChengMapInfoData();
			FashionManager.getInstance().UpdateLuoLanChengZhuFasion(this.WangZuBHid);
			BangHuiLingDiItemData lingdiItemData = JunQiManager.GetItemByLingDiID(7);
			if (null != lingdiItemData)
			{
			}
		}

		
		public void BangHuiLingDiItemsDictFromDBServer()
		{
			if (!this.IsInWangChengFightingTime(TimeUtil.NowDateTime()))
			{
				this.InitLuoLanChengZhuInfo();
			}
		}

		
		public string UpdateWangZuBHNameFromDBServer(int bhid)
		{
			BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(bhid, 0);
			string result;
			if (null == bangHuiMiniData)
			{
				result = GLang.GetLang(6, new object[0]);
			}
			else
			{
				result = bangHuiMiniData.BHName;
			}
			return result;
		}

		
		public int GetWangZuBHid()
		{
			return this.WangZuBHid;
		}

		
		public string GetWangZuBHName()
		{
			return this.WangZuBHName;
		}

		
		private bool IsDayOfWeek(int weekDayID)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (null == this.RuntimeData.WeekPoints)
				{
					return false;
				}
				for (int i = 0; i < this.RuntimeData.WeekPoints.Length; i++)
				{
					if (this.RuntimeData.WeekPoints[i] == weekDayID)
					{
						return true;
					}
				}
			}
			return false;
		}

		
		public bool IsInWangChengFightingTime(DateTime now)
		{
			bool result;
			lock (this.RuntimeData.Mutex)
			{
				int weekDayID = (int)now.DayOfWeek;
				if (!this.IsDayOfWeek(weekDayID))
				{
					result = false;
				}
				else
				{
					int endMinute = 0;
					result = Global.JugeDateTimeInTimeRange(now, this.WangChengZhanFightingDayTimes, out endMinute, false);
				}
			}
			return result;
		}

		
		public void GMStartHuoDongNow()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.WeekPoints[0] = (int)TimeUtil.NowDateTime().DayOfWeek;
					this.RuntimeData.TimePoints = TimeUtil.NowDateTime();
					this.ParseWeekDaysTimes();
				}
			}
			catch (Exception ex)
			{
			}
		}

		
		public void GMSetLuoLanChengZhu(int newBHid)
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					this.LastTheOnlyOneBangHui = newBHid;
					this.RuntimeData.LongTaOwnerData.OwnerBHid = newBHid;
					this.RuntimeData.LongTaOwnerData.OwnerBHName = this.UpdateWangZuBHNameFromDBServer(newBHid);
					this.WangZuBHid = this.RuntimeData.LongTaOwnerData.OwnerBHid;
					this.WangZuBHName = this.RuntimeData.LongTaOwnerData.OwnerBHName;
					this.HandleHuangChengResultEx(true);
					this.NotifyAllWangChengMapInfoData();
				}
			}
			catch (Exception ex)
			{
			}
		}

		
		public bool IsWangChengZhanOver()
		{
			return !this.WaitingHuangChengResult;
		}

		
		public bool IsInBattling()
		{
			return WangChengZhanStates.None != this.WangChengZhanState;
		}

		
		private void NotifyAllLuoLanChengZhanJingJiaResult()
		{
			lock (this.RuntimeData.Mutex)
			{
				bool canRequest = this.CanRequest();
				if (this.RuntimeData.CanRequestState != canRequest)
				{
					this.RuntimeData.CanRequestState = canRequest;
					if (!canRequest)
					{
						string broadCastMsg = GLang.GetLang(40, new object[0]);
						List<LuoLanChengZhanRequestInfoEx> list = this.GetWarRequestInfoList();
						list = list.FindAll((LuoLanChengZhanRequestInfoEx x) => x.BHID > 0);
						for (int i = 0; i < list.Count; i++)
						{
							broadCastMsg += string.Format(GLang.GetLang(41, new object[0]), this.GetBHName(list[i].BHID));
							if (i < list.Count - 1)
							{
								broadCastMsg += GLang.GetLang(42, new object[0]);
							}
						}
						if (list.Count > 0)
						{
							Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, broadCastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
						}
					}
				}
			}
		}

		
		public void ProcessWangChengZhanResult()
		{
			try
			{
				if (Global.GetBangHuiFightingLineID() == GameManager.ServerLineID)
				{
					Global.UpdateLuoLanChengZhanWeekDays(false);
					DateTime now = TimeUtil.NowDateTime();
					if (WangChengZhanStates.None == this.WangChengZhanState)
					{
						if (this.IsInWangChengFightingTime(now))
						{
							this._MapEventMgr.ClearAllMapEvents();
							this.WangChengZhanState = WangChengZhanStates.Fighting;
							this.BangHuiTakeHuangGongTicks = now.Ticks;
							this.RuntimeData.FightEndTime = now.AddSeconds((double)this.RuntimeData.FightingSecs);
							this.WaitingHuangChengResult = true;
							this.RuntimeData.SuperQiZhiOwnerBhid = 0;
							this.NotifyAllWangChengMapInfoData();
							Global.BroadcastHuangChengBattleStart();
						}
						else
						{
							this.ClearMapClients(false);
							this.NotifyAllLuoLanChengZhanJingJiaResult();
						}
					}
					else
					{
						this.UpdateQiZhiBuffParams(now);
						if (this.IsInWangChengFightingTime(now))
						{
							bool ret = this.TryGenerateNewHuangChengBangHui();
							if (ret)
							{
								this.HandleHuangChengResultEx(false);
							}
							else
							{
								this.ProcessTimeAddRoleExp();
							}
						}
						else
						{
							this.ClearMapClients(true);
							this.WangChengZhanState = WangChengZhanStates.None;
							this.WaitingHuangChengResult = false;
							this.TryGenerateNewHuangChengBangHui();
							this.HandleHuangChengResultEx(true);
							JunQiManager.ProcessDelAllJunQiByMapCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.RuntimeData.MapCode);
							this.NotifyAllWangChengMapInfoData();
							this.GiveLuoLanChengZhanAwards();
							this.ResetRequestInfo();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		private void UpdateQiZhiBuffParams(DateTime now)
		{
			lock (this.RuntimeData.Mutex)
			{
				foreach (int key in this.RuntimeData.QiZhiBuffEnableParamsDict.Keys)
				{
					this.RuntimeData.QiZhiBuffEnableParamsDict[key][0] = (double)((int)(this.RuntimeData.FightEndTime - now).TotalSeconds);
				}
			}
		}

		
		private void GiveLuoLanChengZhanAwards()
		{
			LuoLanChengZhanResultInfo resultInfoSuccess = new LuoLanChengZhanResultInfo();
			LuoLanChengZhanResultInfo resultInfoFaild = new LuoLanChengZhanResultInfo();
			resultInfoSuccess.BHID = (resultInfoFaild.BHID = this.WangZuBHid);
			resultInfoSuccess.BHName = (resultInfoFaild.BHName = this.WangZuBHName);
			resultInfoSuccess.ExpAward = this.RuntimeData.ExpAward;
			resultInfoSuccess.ZhanGongAward = this.RuntimeData.ZhanGongAward;
			resultInfoSuccess.ZhanMengZiJin = this.RuntimeData.ZiJin;
			resultInfoFaild.ExpAward = this.RuntimeData.ExpAward / 2L;
			resultInfoFaild.ZhanGongAward = this.RuntimeData.ZhanGongAward / 2;
			resultInfoFaild.ZhanMengZiJin = this.RuntimeData.ZiJin / 2;
			GameClient client = GameManager.ClientMgr.GetFirstClient();
			lock (this.RuntimeData.Mutex)
			{
				foreach (LuoLanChengZhanRequestInfo item in this.RuntimeData.WarRequstDict.Values)
				{
					int bhid = item.BHID;
					int zhanMengZiJin;
					if (item.BHID == this.WangZuBHid)
					{
						zhanMengZiJin = resultInfoSuccess.ZhanMengZiJin;
					}
					else
					{
						zhanMengZiJin = resultInfoFaild.ZhanMengZiJin;
					}
					BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(item.BHID, 0);
					if (!GameManager.ClientMgr.AddBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bhid, zhanMengZiJin))
					{
						LogManager.WriteLog(LogTypes.SQL, string.Format("罗兰城战奖励战盟资金失败,bhid={0}, bidMoney={1}", bhid, zhanMengZiJin), null, true);
					}
				}
			}
			List<object> objsList = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode);
			if (null == objsList)
			{
				objsList = new List<object>();
			}
			List<object> objsList2 = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode_LongTa);
			objsList.AddRange(objsList2);
			if (objsList != null && objsList.Count > 0)
			{
				byte[] bytes0 = DataHelper.ObjectToBytes<LuoLanChengZhanResultInfo>(resultInfoSuccess);
				TCPOutPacket tcpOutPacket0 = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, bytes0, 0, bytes0.Length, 707);
				byte[] bytes = DataHelper.ObjectToBytes<LuoLanChengZhanResultInfo>(resultInfoFaild);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, bytes, 0, bytes.Length, 707);
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						if (c.ClientData.Faction == this.WangZuBHid)
						{
							GameManager.ClientMgr.ProcessRoleExperience(c, resultInfoSuccess.ExpAward, true, true, false, "none");
							int bangGong = resultInfoSuccess.ZhanGongAward;
							GameManager.ClientMgr.AddBangGong(c, ref bangGong, AddBangGongTypes.BG_ChengZhan, 0);
							c.sendCmd(tcpOutPacket0, false);
						}
						else
						{
							GameManager.ClientMgr.ProcessRoleExperience(c, resultInfoFaild.ExpAward, true, true, false, "none");
							int bangGong = resultInfoFaild.ZhanGongAward;
							GameManager.ClientMgr.AddBangGong(c, ref bangGong, AddBangGongTypes.BG_ChengZhan, 0);
							c.sendCmd(tcpOutPacket, false);
						}
					}
				}
				Global.PushBackTcpOutPacket(tcpOutPacket0);
				Global.PushBackTcpOutPacket(tcpOutPacket);
			}
		}

		
		public void ResetRequestInfo()
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.WarRequstDict = new Dictionary<int, LuoLanChengZhanRequestInfo>();
				this.RuntimeData.WarRequestStr = this.GeWarRequstString(this.RuntimeData.WarRequstDict);
				BangHuiLingDiItemData lingDiItem = JunQiManager.GetItemByLingDiID(7);
				if (null != lingDiItem)
				{
					lingDiItem.WarRequest = this.RuntimeData.WarRequestStr;
					this.SetCityWarRequestToDBServer(lingDiItem.LingDiID, lingDiItem.WarRequest);
				}
				this.ResetBHID2SiteDict();
				this.RuntimeData.LongTaBHRoleCountList.Clear();
				for (int i = 0; i < this.RuntimeData.QiZhiBuffOwnerDataList.Count; i++)
				{
					this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHID = 0;
					this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHName = "";
				}
			}
		}

		
		public bool TryGenerateNewHuangChengBangHui()
		{
			List<string> logList = new List<string>();
			int newBHid = this.GetTheOnlyOneBangHui(logList);
			if (!this.WaitingHuangChengResult)
			{
				foreach (string log in logList)
				{
					LogManager.WriteLog(LogTypes.Error, log, null, true);
				}
			}
			this.NotifyLongTaRoleDataList();
			this.NotifyLongTaOwnerData();
			lock (this.RuntimeData.Mutex)
			{
				if (newBHid <= 0 || newBHid == this.RuntimeData.LongTaOwnerData.OwnerBHid)
				{
					this.LastTheOnlyOneBangHui = -1;
					return false;
				}
				if (this.LastTheOnlyOneBangHui != newBHid)
				{
					this.LastTheOnlyOneBangHui = newBHid;
					this.BangHuiTakeHuangGongTicks = TimeUtil.NOW();
					return false;
				}
				if (this.LastTheOnlyOneBangHui > 0)
				{
					long ticks = TimeUtil.NOW();
					if (ticks - this.BangHuiTakeHuangGongTicks > (long)this.MaxTakingHuangGongSecs)
					{
						this.RuntimeData.LongTaOwnerData.OwnerBHid = this.LastTheOnlyOneBangHui;
						this.RuntimeData.LongTaOwnerData.OwnerBHName = this.UpdateWangZuBHNameFromDBServer(newBHid);
						if (this.WaitingHuangChengResult)
						{
							foreach (string log in logList)
							{
								LogManager.WriteLog(LogTypes.Error, log, null, true);
							}
						}
						return true;
					}
				}
			}
			return false;
		}

		
		public int GetTheOnlyOneBangHui(List<string> logList)
		{
			List<GameClient> lsClients = new List<GameClient>();
			List<GameClient> allClients = GameManager.ClientMgr.GetMapGameClients(this.RuntimeData.MapCode_LongTa);
			int newBHid = -1;
			int result;
			if (null == allClients)
			{
				result = newBHid;
			}
			else
			{
				int mapCode = this.RuntimeData.MapCode_LongTa;
				foreach (GameClient client in allClients)
				{
					bool valid = false;
                    if (client.ClientData.CurrentLifeV > 0)
					{
						if (!client.ClientData.WaitingNotifyChangeMap && !client.ClientData.WaitingForChangeMap)
						{
							if (client.ClientData.MapCode == mapCode && Global.IsPosReachable(mapCode, client.ClientData.PosX, client.ClientData.PosY))
							{
								valid = true;
								lsClients.Add(client);
								string reason = string.Format("龙塔地图有效玩家#bhid={8},client={6}({7}),mapCode:{0},lifev={9},clientMapCode{1}:,WaitingNotifyChangeMap:{2},WaitingForChangeMap:{3},PosX:{4},PosY{5}", new object[]
								{
									mapCode,
									client.ClientData.MapCode,
									client.ClientData.WaitingNotifyChangeMap,
									client.ClientData.WaitingForChangeMap,
									client.ClientData.PosX,
									client.ClientData.PosY,
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									client.ClientData.Faction,
									client.ClientData.CurrentLifeV
								});
								logList.Add(reason);
							}
						}
					}
					if (!valid)
					{
						string reason = string.Format("龙塔地图无效玩家#bhid={8},client={6}({7}),mapCode:{0},lifev={9},clientMapCode{1}:,WaitingNotifyChangeMap:{2},WaitingForChangeMap:{3},PosX:{4},PosY{5}", new object[]
						{
							mapCode,
							client.ClientData.MapCode,
							client.ClientData.WaitingNotifyChangeMap,
							client.ClientData.WaitingForChangeMap,
							client.ClientData.PosX,
							client.ClientData.PosY,
							client.ClientData.RoleID,
							client.ClientData.RoleName,
							client.ClientData.Faction,
							client.ClientData.CurrentLifeV
						});
						logList.Add(reason);
					}
				}
				lock (this.RuntimeData.Mutex)
				{
					List<LuoLanChengZhanRoleCountData> list = new List<LuoLanChengZhanRoleCountData>(this.RuntimeData.MaxZhanMengNum);
					for (int i = 0; i < lsClients.Count; i++)
					{
						GameClient client = lsClients[i];
						int bhid = client.ClientData.Faction;
						if (bhid > 0)
						{
							LuoLanChengZhanRoleCountData data = list.Find((LuoLanChengZhanRoleCountData x) => x.BHID == bhid);
							if (null == data)
							{
								list.Add(new LuoLanChengZhanRoleCountData
								{
									BHID = bhid,
									RoleCount = 1
								});
							}
							else
							{
								data.RoleCount++;
							}
						}
					}
					this.RuntimeData.LongTaBHRoleCountList = list;
					if (list.Count == 1)
					{
						newBHid = list[0].BHID;
					}
				}
				result = newBHid;
			}
			return result;
		}

		
		private List<LuoLanChengZhanRequestInfoEx> GetWarRequestInfoList()
		{
			List<LuoLanChengZhanRequestInfoEx> list = new List<LuoLanChengZhanRequestInfoEx>();
			lock (this.RuntimeData.Mutex)
			{
				foreach (LuoLanChengZhanRequestInfo item in this.RuntimeData.WarRequstDict.Values)
				{
					list.Add(new LuoLanChengZhanRequestInfoEx
					{
						Site = item.Site,
						BHID = item.BHID,
						BHName = this.GetBHName(item.BHID),
						BidMoney = item.BidMoney
					});
				}
			}
			return list;
		}

		
		public void NotifyAllWangChengMapInfoData()
		{
			WangChengMapInfoData wangChengMapInfoData = this.FormatWangChengMapInfoData();
			GameManager.ClientMgr.NotifyAllWangChengMapInfoData(wangChengMapInfoData);
		}

		
		public void NotifyLongTaRoleDataList()
		{
			byte[] bytes;
			lock (this.RuntimeData.Mutex)
			{
				bytes = DataHelper.ObjectToBytes<List<LuoLanChengZhanRoleCountData>>(this.RuntimeData.LongTaBHRoleCountList);
			}
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, bytes, 0, bytes.Length, 703);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode, -1, false);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode_LongTa, -1, true);
		}

		
		public void NotifyLongTaOwnerData()
		{
			byte[] bytes;
			lock (this.RuntimeData.Mutex)
			{
				bytes = DataHelper.ObjectToBytes<LuoLanChengZhanLongTaOwnerData>(this.RuntimeData.LongTaOwnerData);
			}
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, bytes, 0, bytes.Length, 705);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode, -1, false);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode_LongTa, -1, true);
		}

		
		public void UpdateQiZhiBangHui(int npcExtentionID, int bhid, string bhName)
		{
			int oldBHID = 0;
			int bufferID = 0;
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < this.RuntimeData.QiZhiBuffOwnerDataList.Count; i++)
				{
					if (this.RuntimeData.QiZhiBuffOwnerDataList[i].NPCID == npcExtentionID)
					{
						oldBHID = this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHID;
						this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHID = bhid;
						this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHName = bhName;
						break;
					}
				}
				QiZhiConfig qiZhiConfig;
				if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out qiZhiConfig))
				{
					bufferID = qiZhiConfig.BufferID;
				}
			}
			if (bhid != oldBHID)
			{
				if (npcExtentionID == this.RuntimeData.SuperQiZhiNpcId)
				{
					this.RuntimeData.SuperQiZhiOwnerBhid = bhid;
				}
				try
				{
					List<object> objsList = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode);
					List<object> objsList2 = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode_LongTa);
					objsList.AddRange(objsList2);
					EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(bufferID);
					if (null != item)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							GameClient c = objsList[i] as GameClient;
							if (c != null)
							{
								bool add = false;
								if (c.ClientData.Faction == oldBHID)
								{
									add = false;
								}
								else if (c.ClientData.Faction == bhid)
								{
									add = true;
								}
								this.UpdateQiZhiBuff4GameClient(c, item, bufferID, add);
							}
						}
					}
					this.NotifyQiZhiBuffOwnerDataList();
				}
				catch (Exception ex)
				{
					LogManager.WriteException("旗帜状态变化,设置旗帜Buff时发生异常:" + ex.ToString());
				}
			}
		}

		
		private void UpdateQiZhiBuff4GameClient(GameClient client, EquipPropItem item, int bufferID, bool add)
		{
			try
			{
				if (add)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.BufferByGoodsProps,
						bufferID,
						item.ExtProps
					});
					Global.UpdateBufferData(client, (BufferItemTypes)bufferID, this.RuntimeData.QiZhiBuffEnableParamsDict[bufferID], 1, true);
				}
				else
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.BufferByGoodsProps,
						bufferID,
						PropsCacheManager.ConstExtProps
					});
					Global.UpdateBufferData(client, (BufferItemTypes)bufferID, this.RuntimeData.QiZhiBuffDisableParamsDict[bufferID], 1, true);
				}
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		
		public void NotifyQiZhiBuffOwnerDataList()
		{
			byte[] bytes;
			lock (this.RuntimeData.Mutex)
			{
				bytes = DataHelper.ObjectToBytes<List<LuoLanChengZhanQiZhiBuffOwnerData>>(this.RuntimeData.QiZhiBuffOwnerDataList);
			}
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, bytes, 0, bytes.Length, 704);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode, -1, false);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode_LongTa, -1, true);
		}

		
		private void HandleHuangChengResultEx(bool isBattleOver = false)
		{
			if (isBattleOver)
			{
				BangHuiDetailData oldBangHuiDetailData = (this.WangZuBHid > 0) ? this.GetBangHuiDetailDataAuto(this.WangZuBHid, -1) : null;
				BangHuiDetailData newBangHuiDetailData = (this.RuntimeData.LongTaOwnerData.OwnerBHid > 0) ? this.GetBangHuiDetailDataAuto(this.RuntimeData.LongTaOwnerData.OwnerBHid, -1) : null;
				EventLogManager.AddLuoLanChengZhanEvent(oldBangHuiDetailData, newBangHuiDetailData);
				this.WangZuBHid = this.RuntimeData.LongTaOwnerData.OwnerBHid;
				this.WangZuBHName = this.RuntimeData.LongTaOwnerData.OwnerBHName;
				if (this.WangZuBHid <= 0)
				{
					JunQiManager.HandleLuoLanChengZhanResult(7, this.RuntimeData.MapCode, 0, "", true, false);
					JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
					Global.BroadcastWangChengFailedHint();
					this.ClearDbKingNpc();
					this.InitLuoLanChengZhuInfo();
					return;
				}
				JunQiManager.HandleLuoLanChengZhanResult(7, this.RuntimeData.MapCode, this.WangZuBHid, this.WangZuBHName, true, false);
				JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
				this.ClearDbKingNpc();
				this.InitLuoLanChengZhuInfo();
				HeFuLuoLanActivity hefuActivity = HuodongCachingMgr.GetHeFuLuoLanActivity();
				if (hefuActivity != null && hefuActivity.InActivityTime())
				{
					string strHefuLuolanGuildid = GameManager.GameConfigMgr.GetGameConfigItemStr("hefu_luolan_guildid", "");
					if (strHefuLuolanGuildid.Split(new char[]
					{
						'|'
					}).Length < 2)
					{
						if (strHefuLuolanGuildid.Length > 0)
						{
							strHefuLuolanGuildid += "|";
						}
						int luoLanChengZhuRoleID = 0;
						BangHuiDetailData bangHuiDetailData = this.GetBangHuiDetailDataAuto(this.WangZuBHid, -1);
						if (null != bangHuiDetailData)
						{
							luoLanChengZhuRoleID = bangHuiDetailData.BZRoleID;
						}
						strHefuLuolanGuildid = strHefuLuolanGuildid + this.WangZuBHid.ToString() + "," + luoLanChengZhuRoleID.ToString();
						Global.UpdateDBGameConfigg("hefu_luolan_guildid", strHefuLuolanGuildid);
					}
				}
			}
			if (this.LastTheOnlyOneBangHui > 0)
			{
				Global.BroadcastHuangChengOkHintEx(this.RuntimeData.LongTaOwnerData.OwnerBHName, isBattleOver);
			}
		}

		
		public void NotifyClientWangChengMapInfoData(GameClient client)
		{
			WangChengMapInfoData wangChengMapInfoData = this.GetWangChengMapInfoData(client);
			GameManager.ClientMgr.NotifyWangChengMapInfoData(client, wangChengMapInfoData);
		}

		
		public WangChengMapInfoData GetWangChengMapInfoData(GameClient client)
		{
			return this.FormatWangChengMapInfoData();
		}

		
		public WangChengMapInfoData FormatWangChengMapInfoData()
		{
			string nextBattleTime = GLang.GetLang(43, new object[0]);
			long endTime = 0L;
			if (WangChengZhanStates.None == this.WangChengZhanState)
			{
				nextBattleTime = this.GetNextCityBattleTime();
			}
			else
			{
				endTime = this.GetBattleEndMs();
			}
			return new WangChengMapInfoData
			{
				FightingEndTime = endTime,
				FightingState = (this.WaitingHuangChengResult ? 1 : 0),
				NextBattleTime = nextBattleTime,
				WangZuBHName = this.WangZuBHName,
				WangZuBHid = this.WangZuBHid
			};
		}

		
		public bool ProcessChengZhanJingJiaCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int bidSite = Global.SafeConvertToInt32(cmdParams[0]);
				int bidMoney = Global.SafeConvertToInt32(cmdParams[1]);
				int bhid = client.ClientData.Faction;
				if (bidSite < 1 || bidSite > this.RuntimeData.MaxZhanMengNum)
				{
					result = -18;
				}
				else if (!this.CanRequest())
				{
					result = -2001;
				}
				else if (bhid <= 0 || client.ClientData.BHZhiWu != 1)
				{
					result = -1002;
				}
				else
				{
					int oldBHID = -1;
					int oldBidMoney = 0;
					lock (this.RuntimeData.Mutex)
					{
						if (this.WangZuBHid == bhid)
						{
							result = -5;
							goto IL_34C;
						}
						BangHuiLingDiItemData lingDiItem = JunQiManager.GetItemByLingDiID(7);
						if (null != lingDiItem)
						{
							if (lingDiItem.WarRequest != this.RuntimeData.WarRequestStr)
							{
								this.RuntimeData.WarRequstDict = this.GetWarRequstMap(lingDiItem.WarRequest);
								this.RuntimeData.WarRequestStr = lingDiItem.WarRequest;
							}
						}
						else
						{
							this.RuntimeData.WarRequstDict = new Dictionary<int, LuoLanChengZhanRequestInfo>();
							this.RuntimeData.WarRequestStr = null;
						}
						int oldSite;
						if (this.RuntimeData.BHID2SiteDict.TryGetValue(bhid, out oldSite) && oldSite != bidSite)
						{
							result = -1004;
							goto IL_34C;
						}
						LuoLanChengZhanRequestInfo requestInfo;
						if (!this.RuntimeData.WarRequstDict.TryGetValue(bidSite, out requestInfo))
						{
							requestInfo = new LuoLanChengZhanRequestInfo();
							requestInfo.Site = bidSite;
							this.RuntimeData.WarRequstDict.Add(bidSite, requestInfo);
						}
						else
						{
							oldBHID = requestInfo.BHID;
							oldBidMoney = requestInfo.BidMoney;
						}
						if ((long)bidMoney < (long)oldBidMoney + this.RuntimeData.BidZhangMengZiJin)
						{
							result = -4;
							goto IL_34C;
						}
						int bhZoneID = 0;
						int subBidMoney;
						if (oldBHID == bhid)
						{
							subBidMoney = bidMoney - oldBidMoney;
						}
						else
						{
							subBidMoney = bidMoney;
						}
						if (!GameManager.ClientMgr.SubBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, subBidMoney, out bhZoneID))
						{
							result = -9;
							goto IL_34C;
						}
						requestInfo.BHID = bhid;
						requestInfo.BidMoney = bidMoney;
						this.RuntimeData.WarRequestStr = this.GeWarRequstString(this.RuntimeData.WarRequstDict);
						lingDiItem.WarRequest = this.RuntimeData.WarRequestStr;
						this.SetCityWarRequestToDBServer(lingDiItem.LingDiID, lingDiItem.WarRequest);
						this.ResetBHID2SiteDict();
					}
					if (oldBHID != bhid && oldBHID > 0 && oldBidMoney > 0)
					{
						if (!GameManager.ClientMgr.AddBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, oldBHID, oldBidMoney))
						{
							LogManager.WriteLog(LogTypes.SQL, string.Format("返还罗兰城战竞价资金失败,bhid={0}, bidMoney={1}", oldBHID, oldBidMoney), null, true);
						}
					}
					GameManager.ClientMgr.NotifyAllLuoLanChengZhanRequestInfoList(this.GetWarRequestInfoList());
				}
				IL_34C:
				client.sendCmd(nID, string.Format("{0}", result), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessLuoLanChengZhanCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Global.SafeConvertToInt32(cmdParams[0]);
				int operation = Global.SafeConvertToInt32(cmdParams[1]);
				int bhid = client.ClientData.Faction;
				int uniolLevel = Global.GetUnionLevel(client, false);
				if (uniolLevel < Global.GetUnionLevel(this.RuntimeData.MinZhuanSheng, this.RuntimeData.MinLevel, false))
				{
					result = -19;
				}
				else if (this.WangChengZhanState != WangChengZhanStates.Fighting)
				{
					result = -2001;
				}
				else if (bhid <= 0)
				{
					result = -1000;
				}
				else
				{
					bool canEnter = false;
					lock (this.RuntimeData.Mutex)
					{
						if (bhid == this.WangZuBHid)
						{
							canEnter = true;
						}
						else
						{
							foreach (LuoLanChengZhanRequestInfo item in this.RuntimeData.WarRequstDict.Values)
							{
								if (item.BHID == bhid)
								{
									canEnter = true;
									break;
								}
							}
						}
					}
					int toMapCode;
					int toPosX;
					int toPosY;
					if (!canEnter)
					{
						result = -1003;
					}
					else if (!this.GetZhanMengBirthPoint(client, this.RuntimeData.MapCode, out toMapCode, out toPosX, out toPosY))
					{
						result = -3;
					}
					else
					{
						GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, toPosX, toPosY, -1, 0);
					}
				}
				client.sendCmd(nID, string.Format("{0}", result), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessGetChengZhanDailyAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 1;
				int roleID = Convert.ToInt32(cmdParams[0]);
				int bhid = client.ClientData.Faction;
				int lingDiID = 7;
				if (bhid <= 0 || client.ClientData.Faction != bhid)
				{
					result = -12;
				}
				else if (7 != lingDiID)
				{
					result = -13;
				}
				else
				{
					BangHuiLingDiItemData lingdiItemData = JunQiManager.GetItemByLingDiID(lingDiID);
					SiegeWarfareEveryDayAwardsItem awardsItem;
					if (lingdiItemData.BHID != bhid)
					{
						result = -12;
					}
					else if (this.IsInBattling())
					{
						result = -2002;
					}
					else if (!this.RuntimeData.SiegeWarfareEveryDayAwardsDict.TryGetValue(client.ClientData.BHZhiWu, out awardsItem))
					{
						result = -1005;
					}
					else
					{
						int lastDayID = Global.GetRoleParamsInt32FromDB(client, "SiegeWarfareEveryDayAwardDayID");
						if (lastDayID == Global.GetOffsetDayNow())
						{
							result = -200;
						}
						else
						{
							List<GoodsData> goodsDataList = Global.ConvertToGoodsDataList(awardsItem.DayGoods.Items, -1);
							if (Global.CanAddGoodsDataList(client, goodsDataList))
							{
								for (int i = 0; i < goodsDataList.Count; i++)
								{
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, "", goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "罗兰城战胜利战盟每日奖励", "1900-01-01 12:00:00", 0, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, 0, goodsDataList[i].ExcellenceInfo, goodsDataList[i].AppendPropLev, 0, null, null, 0, true);
									GoodsData goodsData = goodsDataList[i];
									GameManager.logDBCmdMgr.AddDBLogInfo(goodsData.Id, Global.ModifyGoodsLogName(goodsData), "罗兰城战胜利战盟每日奖励", Global.GetMapName(client.ClientData.MapCode), client.ClientData.RoleName, "增加", goodsData.GCount, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
								}
							}
							else
							{
								result = -100;
							}
							long exp = awardsItem.DayExp;
							int zhanGong = awardsItem.DayZhanGong;
							if (result >= 0)
							{
								Global.SaveRoleParamsInt32ValueToDB(client, "SiegeWarfareEveryDayAwardDayID", Global.GetOffsetDayNow(), true);
								if (exp > 0L)
								{
									GameManager.ClientMgr.ProcessRoleExperience(client, exp, true, true, false, "none");
									long newExp = client.ClientData.Experience;
									GameManager.SystemServerEvents.AddEvent(string.Format("角色根据领地特权领取经验, roleID={0}({1}), exp={2}, newExp={3}, bhid={4}", new object[]
									{
										client.ClientData.RoleID,
										client.ClientData.RoleName,
										exp,
										exp,
										bhid
									}), EventLevels.Record);
								}
								if (zhanGong > 0)
								{
									if (GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref zhanGong, AddBangGongTypes.BG_ChengZhan, 0))
									{
										if (0 != zhanGong)
										{
											GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战功", "罗兰城战每日奖励", "系统", client.ClientData.RoleName, "增加", zhanGong, client.ClientData.ZoneID, client.strUserID, client.ClientData.BangGong, client.ServerId, null);
										}
									}
								}
							}
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}", result), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessGetLuoLanChengZhuInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				LuoLanChengZhuInfo luoLanChengZhuInfo = this.GetLuoLanChengZhuInfo(client);
				if (client.ClientData.Faction == luoLanChengZhuInfo.BHID && luoLanChengZhuInfo.BHID > 0)
				{
					int lastDayID = Global.GetRoleParamsInt32FromDB(client, "SiegeWarfareEveryDayAwardDayID");
					if (lastDayID != Global.GetOffsetDayNow())
					{
						luoLanChengZhuInfo.isGetReward = false;
					}
				}
				client.sendCmd<LuoLanChengZhuInfo>(nID, luoLanChengZhuInfo, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessLuoLanChengZhanRequestInfoListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				GameManager.ClientMgr.NotifyLuoLanChengZhanRequestInfoList(client, this.GetWarRequestInfoList());
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessQueryZhanMengZiJinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				GameManager.ClientMgr.NotifyBangHuiZiJinChanged(client, client.ClientData.Faction);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessGetLuoLanKingLooks(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int lookWho = Convert.ToInt32(cmdParams[1]);
				RoleDataEx rd = this.KingRoleData;
				if (rd != null && rd.RoleID == lookWho)
				{
					RoleData4Selector sel = Global.RoleDataEx2RoleData4Selector(rd);
					client.sendCmd<RoleData4Selector>(nID, sel, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public Dictionary<int, LuoLanChengZhanRequestInfo> GetWarRequstMap(string warReqString)
		{
			Dictionary<int, LuoLanChengZhanRequestInfo> warRequstMap = null;
			try
			{
				byte[] bytes = Convert.FromBase64String(warReqString);
				warRequstMap = DataHelper.BytesToObject<Dictionary<int, LuoLanChengZhanRequestInfo>>(bytes, 0, bytes.Length);
			}
			catch (Exception ex)
			{
			}
			if (null == warRequstMap)
			{
				warRequstMap = new Dictionary<int, LuoLanChengZhanRequestInfo>();
			}
			return warRequstMap;
		}

		
		public string GeWarRequstString(Dictionary<int, LuoLanChengZhanRequestInfo> warRequstMap)
		{
			string nowWarRequest = "";
			try
			{
				byte[] bytes = DataHelper.ObjectToBytes<Dictionary<int, LuoLanChengZhanRequestInfo>>(warRequstMap);
				return Convert.ToBase64String(bytes);
			}
			catch (Exception ex)
			{
			}
			return nowWarRequest;
		}

		
		public int SetCityWarRequestToDBServer(int lingDiID, string nowWarRequest)
		{
			int retCode = -200;
			string strcmd = string.Format("{0}:{1}", lingDiID, nowWarRequest);
			string[] fields = Global.ExecuteDBCmd(10098, strcmd, 0);
			int result;
			if (fields == null || fields.Length != 5)
			{
				result = retCode;
			}
			else
			{
				retCode = Global.SafeConvertToInt32(fields[0]);
				JunQiManager.NotifySyncBangHuiLingDiItemsDict();
				result = retCode;
			}
			return result;
		}

		
		public bool CanRequest()
		{
			DateTime now = TimeUtil.NowDateTime();
			bool result;
			if ((now - Global.GetKaiFuTime()).TotalDays < (double)this.RuntimeData.GongNengOpenDaysFromKaiFu)
			{
				result = false;
			}
			else
			{
				if (this.IsDayOfWeek((int)now.DayOfWeek))
				{
					TimeSpan time = now.TimeOfDay;
					if (time >= this.RuntimeData.NoRequestTimeStart && time <= this.RuntimeData.NoRequestTimeEnd)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		
		private void ResetBHID2SiteDict()
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.BHID2SiteDict.Clear();
				if (this.WangZuBHid > 0)
				{
					this.RuntimeData.BHID2SiteDict[this.WangZuBHid] = 0;
				}
				foreach (LuoLanChengZhanRequestInfo item in this.RuntimeData.WarRequstDict.Values)
				{
					this.RuntimeData.BHID2SiteDict[item.BHID] = item.Site;
				}
			}
		}

		
		public bool IsExistCityWarToday()
		{
			bool result;
			if (!this.IsDayOfWeek((int)TimeUtil.NowDateTime().DayOfWeek))
			{
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.WarRequstDict.Count == 0)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		
		public long GetBattleEndMs()
		{
			DateTime now = TimeUtil.NowDateTime();
			int hour = now.Hour;
			int minute = now.Minute;
			int nowMinite = hour * 60 + minute;
			int endMinute = 0;
			Global.JugeDateTimeInTimeRange(TimeUtil.NowDateTime(), this.WangChengZhanFightingDayTimes, out endMinute, true);
			return now.AddMinutes((double)Math.Max(0, endMinute - nowMinite)).Ticks / 10000L;
		}

		
		public string GetNextCityBattleTime()
		{
			string unKown = GLang.GetLang(6, new object[0]);
			string result;
			if (this.WangChengZhanFightingDayTimes != null && this.WangChengZhanFightingDayTimes.Length > 0)
			{
				result = this.RuntimeData.WangChengZhanFightingDateTime.ToString("yyyy-MM-dd ") + string.Format("{0:00}:{1:00}", this.WangChengZhanFightingDayTimes[0].FromHour, this.WangChengZhanFightingDayTimes[0].FromMinute);
			}
			else
			{
				result = unKown;
			}
			return result;
		}

		
		public string GetCityBattleTimeAndBangHuiListString()
		{
			string result;
			if (this.WangChengZhanFightingDayTimes == null || this.WangChengZhanFightingDayTimes.Length <= 0)
			{
				result = "";
			}
			else
			{
				string timeBangHuiString = "";
				lock (this.RuntimeData.Mutex)
				{
					timeBangHuiString = timeBangHuiString + this.RuntimeData.WangChengZhanFightingDateTime.ToString("yyyy-MM-dd ") + string.Format("{0:00}:{1:00}", this.WangChengZhanFightingDayTimes[0].FromHour, this.WangChengZhanFightingDayTimes[0].FromMinute);
					timeBangHuiString += "|";
					foreach (LuoLanChengZhanRequestInfo req in this.RuntimeData.WarRequstDict.Values)
					{
						timeBangHuiString += string.Format(" {0}", this.GetBHName(req.BHID));
					}
				}
				result = timeBangHuiString;
			}
			return result;
		}

		
		private string GetBHName(int bangHuiID)
		{
			BangHuiMiniData bhData = Global.GetBangHuiMiniData(bangHuiID, 0);
			string result;
			if (null != bhData)
			{
				result = bhData.BHName;
			}
			else
			{
				result = GLang.GetLang(6, new object[0]);
			}
			return result;
		}

		
		private void ProcessTimeAddRoleExp()
		{
			long ticks = TimeUtil.NOW();
			if (ticks - this.LastAddBangZhanAwardsTicks >= 10000L)
			{
				this.LastAddBangZhanAwardsTicks = ticks;
				this.NotifyQiZhiBuffOwnerDataList();
				List<object> objsList = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode);
				if (null != objsList)
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient c = objsList[i] as GameClient;
						if (c != null)
						{
							this._LevelAwardsMgr.ProcessBangZhanAwards(c);
						}
					}
				}
				objsList = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode_LongTa);
				if (null != objsList)
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient c = objsList[i] as GameClient;
						if (c != null)
						{
							this._LevelAwardsMgr.ProcessBangZhanAwards(c);
						}
					}
				}
			}
		}

		
		public bool GetZhanMengBirthPoint(GameClient client, int toMapCode, out int mapCode, out int posX, out int posY)
		{
			mapCode = GameManager.MainMapCode;
			posX = -1;
			posY = -1;
			int bhid = client.ClientData.Faction;
			lock (this.RuntimeData.Mutex)
			{
				int site;
				if (!this.RuntimeData.BHID2SiteDict.TryGetValue(bhid, out site))
				{
					return true;
				}
				int round = 0;
				if (toMapCode == this.RuntimeData.MapCode_LongTa)
				{
					Point pt;
					for (;;)
					{
						pt = Global.GetRandomPoint(ObjectTypes.OT_CLIENT, this.RuntimeData.MapCode_LongTa);
						if (!Global.InObs(ObjectTypes.OT_CLIENT, this.RuntimeData.MapCode_LongTa, (int)pt.X, (int)pt.Y, 0, 0))
						{
							break;
						}
						if (round++ >= 1000)
						{
							goto IL_EE;
						}
					}
					mapCode = this.RuntimeData.MapCode_LongTa;
					posX = (int)pt.X;
					posY = (int)pt.Y;
					return true;
				}
				IL_EE:
				round = 0;
				if (client.ClientData.Faction == this.RuntimeData.SuperQiZhiOwnerBhid && toMapCode == this.RuntimeData.MapCode)
				{
					for (;;)
					{
						mapCode = toMapCode;
						posX = Global.GetRandomNumber(this.RuntimeData.SuperQiZhiOwnerBirthPosX - 400, this.RuntimeData.SuperQiZhiOwnerBirthPosX + 400);
						posY = Global.GetRandomNumber(this.RuntimeData.SuperQiZhiOwnerBirthPosY - 400, this.RuntimeData.SuperQiZhiOwnerBirthPosY + 400);
						if (!Global.InObs(ObjectTypes.OT_CLIENT, toMapCode, posX, posY, 0, 0))
						{
							break;
						}
						if (round++ >= 100)
						{
							goto IL_1B1;
						}
					}
					return true;
				}
				IL_1B1:
				List<MapBirthPoint> list;
				if (!this.RuntimeData.MapBirthPointListDict.TryGetValue(site, out list) || list.Count == 0)
				{
					return true;
				}
				round = 0;
				for (;;)
				{
					int rnd = Global.GetRandomNumber(0, list.Count);
					MapBirthPoint mapBirthPoint = list[rnd];
					mapCode = mapBirthPoint.MapCode;
					posX = mapBirthPoint.BirthPosX + Global.GetRandomNumber(-mapBirthPoint.BirthRangeX, mapBirthPoint.BirthRangeX);
					posY = mapBirthPoint.BirthPosY + Global.GetRandomNumber(-mapBirthPoint.BirthRangeY, mapBirthPoint.BirthRangeY);
					if (!Global.InObs(ObjectTypes.OT_CLIENT, mapCode, posX, posY, 0, 0))
					{
						break;
					}
					if (round++ >= 1000)
					{
						goto Block_10;
					}
				}
				return true;
				Block_10:;
			}
			return true;
		}

		
		public bool ClientRelive(GameClient client)
		{
			int mapCode = client.ClientData.MapCode;
			if (mapCode == this.RuntimeData.MapCode || mapCode == this.RuntimeData.MapCode_LongTa)
			{
				int toMapCode;
				int toPosX;
				int toPosY;
				if (this.GetZhanMengBirthPoint(client, this.RuntimeData.MapCode, out toMapCode, out toPosX, out toPosY))
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					client.ClientData.MoveAndActionNum = 0;
					GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, toPosX, toPosY, -1);
					if (toMapCode != client.ClientData.MapCode)
					{
						GameManager.ClientMgr.NotifyMySelfRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, -1);
						GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, toPosX, toPosY, -1, 1);
					}
					else
					{
						Global.ClientRealive(client, toPosX, toPosY, -1);
					}
					return true;
				}
			}
			return false;
		}

		
		public bool ClientInitGame(GameClient client)
		{
			int mapCode = client.ClientData.MapCode;
			if (mapCode == this.RuntimeData.MapCode || mapCode == this.RuntimeData.MapCode_LongTa)
			{
				int toMapCode;
				int toPosX;
				int toPosY;
				if (this.WangChengZhanState != WangChengZhanStates.Fighting)
				{
					client.ClientData.MapCode = GameManager.MainMapCode;
					client.ClientData.PosX = -1;
					client.ClientData.PosY = -1;
				}
				else if (this.GetZhanMengBirthPoint(client, this.RuntimeData.MapCode, out toMapCode, out toPosX, out toPosY))
				{
					client.ClientData.MapCode = toMapCode;
					client.ClientData.PosX = toPosX;
					client.ClientData.PosY = toPosY;
				}
			}
			return true;
		}

		
		public bool ClientChangeMap(GameClient client, ref int toNewMapCode, ref int toNewPosX, ref int toNewPosY)
		{
			if (toNewMapCode == this.RuntimeData.MapCode || toNewMapCode == this.RuntimeData.MapCode_LongTa)
			{
				if (this.WangChengZhanState != WangChengZhanStates.Fighting)
				{
					toNewMapCode = GameManager.MainMapCode;
					toNewPosX = -1;
					toNewPosY = -1;
				}
				else if (client.ClientData.MapCode != this.RuntimeData.MapCode_LongTa)
				{
					int toMapCode;
					int toPosX;
					int toPosY;
					if (this.GetZhanMengBirthPoint(client, toNewMapCode, out toMapCode, out toPosX, out toPosY))
					{
						toNewMapCode = toMapCode;
						toNewPosX = toPosX;
						toNewPosY = toPosY;
					}
				}
			}
			return true;
		}

		
		public bool OnPreInstallJunQi(GameClient client, int npcID)
		{
			bool result;
			if (!this.IsInBattling())
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(44, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else if (client.ClientData.Faction <= 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(45, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else
			{
				int oldBHID = 0;
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < this.RuntimeData.QiZhiBuffOwnerDataList.Count; i++)
					{
						if (this.RuntimeData.QiZhiBuffOwnerDataList[i].NPCID == npcID)
						{
							oldBHID = this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHID;
							break;
						}
					}
				}
				if (oldBHID > 0)
				{
					result = false;
				}
				else if (!JunQiManager.CanInstallJunQiNow(client.ClientData.MapCode, npcID - 2130706432, client.ClientData.Faction))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(46, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		
		public void ClearMapClients(bool resetTimeOnly = false)
		{
			if (resetTimeOnly)
			{
				this.RuntimeData.LastClearMapTicks = TimeUtil.NOW();
			}
			else
			{
				long nowTicks = TimeUtil.NOW();
				if (nowTicks - this.RuntimeData.LastClearMapTicks > 60000L)
				{
					this.RuntimeData.LastClearMapTicks = nowTicks;
					List<object> objsList = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode);
					if (objsList != null && objsList.Count > 0)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							GameClient c = objsList[i] as GameClient;
							if (c != null)
							{
								GameManager.ClientMgr.NotifyChangMap2NormalMap(c);
							}
						}
					}
					objsList = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode_LongTa);
					if (objsList != null && objsList.Count > 0)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							GameClient c = objsList[i] as GameClient;
							if (c != null)
							{
								GameManager.ClientMgr.NotifyChangMap2NormalMap(c);
							}
						}
					}
				}
			}
		}

		
		public void OnInstallJunQi(GameClient client, int npcID)
		{
			int bhZoneID = 0;
			if (this.RuntimeData.InstallJunQiNeedMoney > 0)
			{
				if (!GameManager.ClientMgr.SubBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.RuntimeData.InstallJunQiNeedMoney, out bhZoneID))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(47, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 27);
					return;
				}
			}
			string junQiName = JunQiManager.GetJunQiNameByBHID(client.ClientData.Faction);
			int junQiLevel = JunQiManager.GetJunQiLevelByBHID(client.ClientData.Faction);
			bool installed = JunQiManager.ProcessNewJunQi(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.MapCode, client.ClientData.Faction, bhZoneID, client.ClientData.BHName, npcID - 2130706432, junQiName, junQiLevel, SceneUIClasses.LuoLanChengZhan);
			if (installed)
			{
				this.UpdateQiZhiBangHui(npcID - 2130706432, client.ClientData.Faction, client.ClientData.BHName);
				Global.BroadcastBangHuiMsg(-1, client.ClientData.Faction, StringUtil.substitute(GLang.GetLang(48, new object[0]), new object[]
				{
					Global.FormatRoleName(client, client.ClientData.RoleName),
					Global.GetServerLineName2(),
					Global.GetMapName(client.ClientData.MapCode)
				}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlySysHint);
			}
		}

		
		public void OnProcessJunQiDead(int npcID, int bhid)
		{
			this.UpdateQiZhiBangHui(npcID, 0, "");
		}

		
		private void ResetQiZhiBuff(GameClient client)
		{
			int toMapCode = client.ClientData.MapCode;
			List<int> bufferIDList = new List<int>();
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < this.RuntimeData.QiZhiBuffOwnerDataList.Count; i++)
				{
					QiZhiConfig qiZhiConfig;
					if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(this.RuntimeData.QiZhiBuffOwnerDataList[i].NPCID, out qiZhiConfig))
					{
						int bufferID = qiZhiConfig.BufferID;
						EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(bufferID);
						if (null != item)
						{
							bool add = false;
							if (toMapCode == this.RuntimeData.MapCode || toMapCode == this.RuntimeData.MapCode_LongTa)
							{
								if (this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHID == client.ClientData.Faction)
								{
									add = true;
								}
							}
							this.UpdateQiZhiBuff4GameClient(client, item, bufferID, add);
						}
					}
				}
			}
		}

		
		public void OnStartPlayGame(GameClient client)
		{
			this.ResetQiZhiBuff(client);
			if (client.ClientData.MapCode == this.RuntimeData.MapCode)
			{
				this._MapEventMgr.PlayMapEvents(client);
			}
			this.BroadcastLuoLanChengZhuLoginHint(client);
			if (client.ClientData.Faction == this.RuntimeData.LuoLanChengZhuBHID && client.ClientData.Faction > 0)
			{
				if (client.ClientData.BHZhiWu == 1)
				{
					Global.UpdateBufferData(client, BufferItemTypes.LuoLanChengZhu_Title, new double[]
					{
						1.0
					}, 1, true);
				}
				else
				{
					Global.UpdateBufferData(client, BufferItemTypes.LuoLanGuiZu_Title, new double[]
					{
						1.0
					}, 1, true);
				}
			}
		}

		
		public void UpdateChengHaoBuff(GameClient client)
		{
			if (client.ClientData.Faction == this.RuntimeData.LuoLanChengZhuBHID && client.ClientData.Faction > 0)
			{
				if (client.ClientData.BHZhiWu == 1)
				{
					Global.UpdateBufferData(client, BufferItemTypes.LuoLanChengZhu_Title, new double[]
					{
						1.0
					}, 1, true);
				}
				else
				{
					Global.UpdateBufferData(client, BufferItemTypes.LuoLanGuiZu_Title, new double[]
					{
						1.0
					}, 1, true);
				}
			}
			else
			{
				BufferItemTypes bufferItemType = BufferItemTypes.LuoLanChengZhu_Title;
				double[] actionParams = new double[1];
				Global.UpdateBufferData(client, bufferItemType, actionParams, 1, true);
				BufferItemTypes bufferItemType2 = BufferItemTypes.LuoLanGuiZu_Title;
				actionParams = new double[1];
				Global.UpdateBufferData(client, bufferItemType2, actionParams, 1, true);
			}
		}

		
		public void BroadcastLuoLanChengZhuLoginHint(GameClient client)
		{
			long nowTicks = TimeUtil.NOW();
			if (!GameManager.IsKuaFuServer && nowTicks >= Data.NextBroadCastTickDict[2])
			{
				Data.NextBroadCastTickDict[2] = nowTicks + Data.LuoLanKingGongGaoCD * 1000L;
				if (this.RuntimeData.LuoLanChengZhuClient != client && client.ClientData.Faction == this.RuntimeData.LuoLanChengZhuBHID && client.ClientData.BHZhiWu == 1)
				{
					if (nowTicks > this.RuntimeData.LuoLanChengZhuLastLoginTicks + 60000L)
					{
						this.RuntimeData.LuoLanChengZhuLastLoginTicks = nowTicks;
						this.RuntimeData.LuoLanChengZhuClient = client;
						string broadCastMsg = StringUtil.substitute(GLang.GetLang(49, new object[0]), new object[]
						{
							Global.FormatRoleName(client, client.ClientData.RoleName)
						});
						Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.Bulletin, broadCastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
					}
				}
			}
		}

		
		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			bool isQiZuo = false;
			bool installJunQi = false;
			lock (this.RuntimeData.Mutex)
			{
				foreach (QiZhiConfig item in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
				{
					if (item.NPCID == npcExtentionID)
					{
						if (Math.Abs(client.ClientData.PosX - item.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - item.PosY) <= 1000)
						{
							installJunQi = true;
						}
						isQiZuo = true;
						break;
					}
				}
			}
			if (installJunQi)
			{
				Global.InstallJunQi(client, npcID, SceneUIClasses.LuoLanChengZhan);
			}
			return isQiZuo;
		}

		
		public void AddGuangMuEvent(int guangMuID, int show)
		{
			this._MapEventMgr.AddGuangMuEvent(guangMuID, show);
		}

		
		public bool OnPreBangHuiAddMember(PreBangHuiAddMemberEventObject e)
		{
			bool result;
			if (!this.IsInBattling())
			{
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.BHID2SiteDict.ContainsKey(e.BHID))
					{
						e.Result = false;
					}
				}
				if (!e.Result)
				{
					GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(50, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		
		public bool OnPreBangHuiRemoveMember(PreBangHuiRemoveMemberEventObject e)
		{
			bool result;
			if (!this.IsInBattling())
			{
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.BHID2SiteDict.ContainsKey(e.BHID))
					{
						e.Result = false;
					}
				}
				if (!e.Result)
				{
					GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(51, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		
		
		
		private RoleDataEx KingRoleData
		{
			get
			{
				RoleDataEx kingRoleData;
				lock (this.kingRoleDataMutex)
				{
					kingRoleData = this._kingRoleData;
				}
				return kingRoleData;
			}
			set
			{
				lock (this.kingRoleDataMutex)
				{
					this._kingRoleData = value;
				}
			}
		}

		
		public void ReShowLuolanKing(int roleID = 0)
		{
			if (roleID <= 0)
			{
				roleID = LuoLanChengZhanManager.getInstance().GetLuoLanChengZhuRoleID();
			}
			if (roleID <= 0)
			{
				this.RestoreLuolanKingNpc();
			}
			else
			{
				this.ReplaceLuolanKingNpc(roleID);
			}
		}

		
		public void ClearDbKingNpc()
		{
			this.KingRoleData = null;
			Global.sendToDB<bool, string>(13232, string.Format("{0}", 2), 0);
		}

		
		public void ReplaceLuolanKingNpc(int roleId)
		{
			RoleDataEx rd = this.KingRoleData;
			this.KingRoleData = null;
			if (rd == null || rd.RoleID != roleId)
			{
				rd = Global.sendToDB<RoleDataEx, KingRoleGetData>(13230, new KingRoleGetData
				{
					KingType = 2
				}, 0);
				if (rd == null || rd.RoleID != roleId)
				{
					RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, roleId), 0);
					if (dbRd == null || dbRd.RoleID <= 0)
					{
						return;
					}
					rd = dbRd;
					if (!Global.sendToDB<bool, KingRolePutData>(13231, new KingRolePutData
					{
						KingType = 2,
						RoleDataEx = rd
					}, 0))
					{
					}
				}
			}
			if (rd != null && rd.RoleID > 0)
			{
				this.KingRoleData = rd;
				NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 131);
				if (null != npc)
				{
					npc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang2, false);
					FakeRoleManager.ProcessNewFakeRole(new SafeClientData
					{
						RoleData = rd
					}, npc.MapCode, FakeRoleTypes.DiaoXiang2, 4, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, 131);
				}
			}
		}

		
		public void RestoreLuolanKingNpc()
		{
			NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 131);
			if (null != npc)
			{
				npc.ShowNpc = true;
				GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang2, false);
			}
		}

		
		public int GetLuoLanChengZhuRoleID()
		{
			int luoLanChengZhuRoleID = 0;
			lock (this.RuntimeData.Mutex)
			{
				BangHuiLingDiItemData lingDiItem = JunQiManager.GetItemByLingDiID(7);
				if (null != lingDiItem)
				{
					BangHuiDetailData bangHuiDetailData = this.GetBangHuiDetailDataAuto(lingDiItem.BHID, -1);
					if (null != bangHuiDetailData)
					{
						luoLanChengZhuRoleID = bangHuiDetailData.BZRoleID;
					}
				}
			}
			return luoLanChengZhuRoleID;
		}

		
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				RoleDataEx rd = this.KingRoleData;
				if (rd != null && rd.RoleID == roleId)
				{
					rd.RoleName = newName;
					if (!Global.sendToDB<bool, KingRolePutData>(13231, new KingRolePutData
					{
						KingType = 2,
						RoleDataEx = rd
					}, 0))
					{
					}
					this.KingRoleData = null;
					this.ReShowLuolanKing(0);
				}
			}
		}

		
		public const SceneUIClasses ManagerType = SceneUIClasses.LuoLanChengZhan;

		
		private static LuoLanChengZhanManager instance = new LuoLanChengZhanManager();

		
		public LuoLanChengZhanData RuntimeData = new LuoLanChengZhanData();

		
		public LevelAwardsMgr _LevelAwardsMgr = new LevelAwardsMgr();

		
		private MapEventMgr _MapEventMgr = new MapEventMgr();

		
		private bool WaitingHuangChengResult = false;

		
		private long BangHuiTakeHuangGongTicks = TimeUtil.NOW();

		
		private string WangZuBHName = "";

		
		private int WangZuBHid = -1;

		
		public object ApplyWangChengWarMutex = new object();

		
		public int MaxTakingHuangGongSecs = 5000;

		
		public bool WangChengZhanWeekDaysByConfig = false;

		
		public DateTimeRange[] WangChengZhanFightingDayTimes = null;

		
		public WangChengZhanStates WangChengZhanState = WangChengZhanStates.None;

		
		private int LastTheOnlyOneBangHui = -1;

		
		private long LastAddBangZhanAwardsTicks = 0L;

		
		private object kingRoleDataMutex = new object();

		
		private RoleDataEx _kingRoleData = null;
	}
}
