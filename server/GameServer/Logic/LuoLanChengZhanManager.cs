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
	// Token: 0x0200051B RID: 1307
	public class LuoLanChengZhanManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx
	{
		// Token: 0x0600187D RID: 6269 RVA: 0x0017F050 File Offset: 0x0017D250
		public static LuoLanChengZhanManager getInstance()
		{
			return LuoLanChengZhanManager.instance;
		}

		// Token: 0x0600187E RID: 6270 RVA: 0x0017F068 File Offset: 0x0017D268
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x0600187F RID: 6271 RVA: 0x0017F08C File Offset: 0x0017D28C
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

		// Token: 0x06001880 RID: 6272 RVA: 0x0017F178 File Offset: 0x0017D378
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(23, 10000, LuoLanChengZhanManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(24, 10000, LuoLanChengZhanManager.getInstance());
			return true;
		}

		// Token: 0x06001881 RID: 6273 RVA: 0x0017F1BC File Offset: 0x0017D3BC
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06001882 RID: 6274 RVA: 0x0017F1D0 File Offset: 0x0017D3D0
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06001883 RID: 6275 RVA: 0x0017F1E4 File Offset: 0x0017D3E4
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

		// Token: 0x06001884 RID: 6276 RVA: 0x0017F29C File Offset: 0x0017D49C
		public void processEvent(EventObject eventObject)
		{
			switch (eventObject.getEventType())
			{
			}
		}

		// Token: 0x06001885 RID: 6277 RVA: 0x0017F2D0 File Offset: 0x0017D4D0
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

		// Token: 0x06001886 RID: 6278 RVA: 0x0017F37C File Offset: 0x0017D57C
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

		// Token: 0x06001887 RID: 6279 RVA: 0x0017FCD4 File Offset: 0x0017DED4
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

		// Token: 0x06001888 RID: 6280 RVA: 0x0017FE68 File Offset: 0x0017E068
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

		// Token: 0x06001889 RID: 6281 RVA: 0x0017FFD0 File Offset: 0x0017E1D0
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

		// Token: 0x0600188A RID: 6282 RVA: 0x00180020 File Offset: 0x0017E220
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

		// Token: 0x0600188B RID: 6283 RVA: 0x00180188 File Offset: 0x0017E388
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

		// Token: 0x0600188C RID: 6284 RVA: 0x001801D4 File Offset: 0x0017E3D4
		public void BangHuiLingDiItemsDictFromDBServer()
		{
			if (!this.IsInWangChengFightingTime(TimeUtil.NowDateTime()))
			{
				this.InitLuoLanChengZhuInfo();
			}
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x001801FC File Offset: 0x0017E3FC
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

		// Token: 0x0600188E RID: 6286 RVA: 0x00180238 File Offset: 0x0017E438
		public int GetWangZuBHid()
		{
			return this.WangZuBHid;
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x00180250 File Offset: 0x0017E450
		public string GetWangZuBHName()
		{
			return this.WangZuBHName;
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x00180268 File Offset: 0x0017E468
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

		// Token: 0x06001891 RID: 6289 RVA: 0x00180314 File Offset: 0x0017E514
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

		// Token: 0x06001892 RID: 6290 RVA: 0x00180390 File Offset: 0x0017E590
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

		// Token: 0x06001893 RID: 6291 RVA: 0x00180424 File Offset: 0x0017E624
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

		// Token: 0x06001894 RID: 6292 RVA: 0x001804F0 File Offset: 0x0017E6F0
		public bool IsWangChengZhanOver()
		{
			return !this.WaitingHuangChengResult;
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x0018050C File Offset: 0x0017E70C
		public bool IsInBattling()
		{
			return WangChengZhanStates.None != this.WangChengZhanState;
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x00180550 File Offset: 0x0017E750
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

		// Token: 0x06001897 RID: 6295 RVA: 0x001806B4 File Offset: 0x0017E8B4
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

		// Token: 0x06001898 RID: 6296 RVA: 0x00180854 File Offset: 0x0017EA54
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

		// Token: 0x06001899 RID: 6297 RVA: 0x00180920 File Offset: 0x0017EB20
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

		// Token: 0x0600189A RID: 6298 RVA: 0x00180CC4 File Offset: 0x0017EEC4
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

		// Token: 0x0600189B RID: 6299 RVA: 0x00180DE0 File Offset: 0x0017EFE0
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

		// Token: 0x0600189C RID: 6300 RVA: 0x00181020 File Offset: 0x0017F220
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

		// Token: 0x0600189D RID: 6301 RVA: 0x00181488 File Offset: 0x0017F688
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

		// Token: 0x0600189E RID: 6302 RVA: 0x00181578 File Offset: 0x0017F778
		public void NotifyAllWangChengMapInfoData()
		{
			WangChengMapInfoData wangChengMapInfoData = this.FormatWangChengMapInfoData();
			GameManager.ClientMgr.NotifyAllWangChengMapInfoData(wangChengMapInfoData);
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x0018159C File Offset: 0x0017F79C
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

		// Token: 0x060018A0 RID: 6304 RVA: 0x00181648 File Offset: 0x0017F848
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

		// Token: 0x060018A1 RID: 6305 RVA: 0x001816F4 File Offset: 0x0017F8F4
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

		// Token: 0x060018A2 RID: 6306 RVA: 0x00181948 File Offset: 0x0017FB48
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

		// Token: 0x060018A3 RID: 6307 RVA: 0x00181A48 File Offset: 0x0017FC48
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

		// Token: 0x060018A4 RID: 6308 RVA: 0x00181AF4 File Offset: 0x0017FCF4
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

		// Token: 0x060018A5 RID: 6309 RVA: 0x00181D10 File Offset: 0x0017FF10
		public void NotifyClientWangChengMapInfoData(GameClient client)
		{
			WangChengMapInfoData wangChengMapInfoData = this.GetWangChengMapInfoData(client);
			GameManager.ClientMgr.NotifyWangChengMapInfoData(client, wangChengMapInfoData);
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x00181D34 File Offset: 0x0017FF34
		public WangChengMapInfoData GetWangChengMapInfoData(GameClient client)
		{
			return this.FormatWangChengMapInfoData();
		}

		// Token: 0x060018A7 RID: 6311 RVA: 0x00181D4C File Offset: 0x0017FF4C
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

		// Token: 0x060018A8 RID: 6312 RVA: 0x00181DDC File Offset: 0x0017FFDC
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

		// Token: 0x060018A9 RID: 6313 RVA: 0x001821AC File Offset: 0x001803AC
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

		// Token: 0x060018AA RID: 6314 RVA: 0x001823E8 File Offset: 0x001805E8
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

		// Token: 0x060018AB RID: 6315 RVA: 0x00182820 File Offset: 0x00180A20
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

		// Token: 0x060018AC RID: 6316 RVA: 0x001828D0 File Offset: 0x00180AD0
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

		// Token: 0x060018AD RID: 6317 RVA: 0x00182928 File Offset: 0x00180B28
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

		// Token: 0x060018AE RID: 6318 RVA: 0x00182984 File Offset: 0x00180B84
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

		// Token: 0x060018AF RID: 6319 RVA: 0x00182A08 File Offset: 0x00180C08
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

		// Token: 0x060018B0 RID: 6320 RVA: 0x00182A60 File Offset: 0x00180C60
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

		// Token: 0x060018B1 RID: 6321 RVA: 0x00182AA4 File Offset: 0x00180CA4
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

		// Token: 0x060018B2 RID: 6322 RVA: 0x00182B04 File Offset: 0x00180D04
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

		// Token: 0x060018B3 RID: 6323 RVA: 0x00182BA4 File Offset: 0x00180DA4
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

		// Token: 0x060018B4 RID: 6324 RVA: 0x00182C94 File Offset: 0x00180E94
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

		// Token: 0x060018B5 RID: 6325 RVA: 0x00182D24 File Offset: 0x00180F24
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

		// Token: 0x060018B6 RID: 6326 RVA: 0x00182D90 File Offset: 0x00180F90
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

		// Token: 0x060018B7 RID: 6327 RVA: 0x00182E1C File Offset: 0x0018101C
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

		// Token: 0x060018B8 RID: 6328 RVA: 0x00182F60 File Offset: 0x00181160
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

		// Token: 0x060018B9 RID: 6329 RVA: 0x00182F98 File Offset: 0x00181198
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

		// Token: 0x060018BA RID: 6330 RVA: 0x001830A4 File Offset: 0x001812A4
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

		// Token: 0x060018BB RID: 6331 RVA: 0x0018336C File Offset: 0x0018156C
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

		// Token: 0x060018BC RID: 6332 RVA: 0x001834D8 File Offset: 0x001816D8
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

		// Token: 0x060018BD RID: 6333 RVA: 0x001835B0 File Offset: 0x001817B0
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

		// Token: 0x060018BE RID: 6334 RVA: 0x0018365C File Offset: 0x0018185C
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

		// Token: 0x060018BF RID: 6335 RVA: 0x00183830 File Offset: 0x00181A30
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

		// Token: 0x060018C0 RID: 6336 RVA: 0x0018397C File Offset: 0x00181B7C
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

		// Token: 0x060018C1 RID: 6337 RVA: 0x00183B25 File Offset: 0x00181D25
		public void OnProcessJunQiDead(int npcID, int bhid)
		{
			this.UpdateQiZhiBangHui(npcID, 0, "");
		}

		// Token: 0x060018C2 RID: 6338 RVA: 0x00183B38 File Offset: 0x00181D38
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

		// Token: 0x060018C3 RID: 6339 RVA: 0x00183C98 File Offset: 0x00181E98
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

		// Token: 0x060018C4 RID: 6340 RVA: 0x00183D78 File Offset: 0x00181F78
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

		// Token: 0x060018C5 RID: 6341 RVA: 0x00183E48 File Offset: 0x00182048
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

		// Token: 0x060018C6 RID: 6342 RVA: 0x00183F54 File Offset: 0x00182154
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

		// Token: 0x060018C7 RID: 6343 RVA: 0x00184074 File Offset: 0x00182274
		public void AddGuangMuEvent(int guangMuID, int show)
		{
			this._MapEventMgr.AddGuangMuEvent(guangMuID, show);
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x00184088 File Offset: 0x00182288
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

		// Token: 0x060018C9 RID: 6345 RVA: 0x00184140 File Offset: 0x00182340
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

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060018CA RID: 6346 RVA: 0x001841F8 File Offset: 0x001823F8
		// (set) Token: 0x060018CB RID: 6347 RVA: 0x00184244 File Offset: 0x00182444
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

		// Token: 0x060018CC RID: 6348 RVA: 0x00184290 File Offset: 0x00182490
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

		// Token: 0x060018CD RID: 6349 RVA: 0x001842CE File Offset: 0x001824CE
		public void ClearDbKingNpc()
		{
			this.KingRoleData = null;
			Global.sendToDB<bool, string>(13232, string.Format("{0}", 2), 0);
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x001842F8 File Offset: 0x001824F8
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

		// Token: 0x060018CF RID: 6351 RVA: 0x00184488 File Offset: 0x00182688
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

		// Token: 0x060018D0 RID: 6352 RVA: 0x001844E0 File Offset: 0x001826E0
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

		// Token: 0x060018D1 RID: 6353 RVA: 0x0018456C File Offset: 0x0018276C
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

		// Token: 0x040022BA RID: 8890
		public const SceneUIClasses ManagerType = SceneUIClasses.LuoLanChengZhan;

		// Token: 0x040022BB RID: 8891
		private static LuoLanChengZhanManager instance = new LuoLanChengZhanManager();

		// Token: 0x040022BC RID: 8892
		public LuoLanChengZhanData RuntimeData = new LuoLanChengZhanData();

		// Token: 0x040022BD RID: 8893
		public LevelAwardsMgr _LevelAwardsMgr = new LevelAwardsMgr();

		// Token: 0x040022BE RID: 8894
		private MapEventMgr _MapEventMgr = new MapEventMgr();

		// Token: 0x040022BF RID: 8895
		private bool WaitingHuangChengResult = false;

		// Token: 0x040022C0 RID: 8896
		private long BangHuiTakeHuangGongTicks = TimeUtil.NOW();

		// Token: 0x040022C1 RID: 8897
		private string WangZuBHName = "";

		// Token: 0x040022C2 RID: 8898
		private int WangZuBHid = -1;

		// Token: 0x040022C3 RID: 8899
		public object ApplyWangChengWarMutex = new object();

		// Token: 0x040022C4 RID: 8900
		public int MaxTakingHuangGongSecs = 5000;

		// Token: 0x040022C5 RID: 8901
		public bool WangChengZhanWeekDaysByConfig = false;

		// Token: 0x040022C6 RID: 8902
		public DateTimeRange[] WangChengZhanFightingDayTimes = null;

		// Token: 0x040022C7 RID: 8903
		public WangChengZhanStates WangChengZhanState = WangChengZhanStates.None;

		// Token: 0x040022C8 RID: 8904
		private int LastTheOnlyOneBangHui = -1;

		// Token: 0x040022C9 RID: 8905
		private long LastAddBangZhanAwardsTicks = 0L;

		// Token: 0x040022CA RID: 8906
		private object kingRoleDataMutex = new object();

		// Token: 0x040022CB RID: 8907
		private RoleDataEx _kingRoleData = null;
	}
}
