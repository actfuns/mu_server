using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020005DB RID: 1499
	public class BroadcastInfoMgr
	{
		// Token: 0x06001BE1 RID: 7137 RVA: 0x001A286C File Offset: 0x001A0A6C
		public static void LoadBroadcastInfoItemList()
		{
			XElement xml = null;
			string fileName = "Config/BroadcastInfos.xml";
			try
			{
				xml = XElement.Load(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
			}
			List<BroadcastInfoItem> broadcastInfoItemList = new List<BroadcastInfoItem>();
			IEnumerable<XElement> nodes = xml.Elements("Infos").Elements<XElement>();
			foreach (XElement node in nodes)
			{
				SystemXmlItem systemXmlItem = new SystemXmlItem
				{
					XMLNode = node
				};
				BroadcastInfoMgr.ParseXmlItem(systemXmlItem, broadcastInfoItemList);
			}
			BroadcastInfoMgr.BroadcastInfoItemList = broadcastInfoItemList;
		}

		// Token: 0x06001BE2 RID: 7138 RVA: 0x001A2960 File Offset: 0x001A0B60
		private static void ParseXmlItem(SystemXmlItem systemXmlItem, List<BroadcastInfoItem> broadcastInfoItemList)
		{
			int id = systemXmlItem.GetIntValue("ID", -1);
			int infoClass = systemXmlItem.GetIntValue("InfoClass", -1);
			int hintErrID = systemXmlItem.GetIntValue("HintErrID", -1);
			int timeType = systemXmlItem.GetIntValue("TimeType", -1);
			int kaiFuStartDay = systemXmlItem.GetIntValue("StartDay", -1);
			int kaiFuShowType = systemXmlItem.GetIntValue("ShowType", -1);
			string weekDays = systemXmlItem.GetStringValue("WeekDays");
			string times = systemXmlItem.GetStringValue("Times");
			string text = systemXmlItem.GetStringValue("Text");
			string onlineNotice = systemXmlItem.GetStringValue("OnlineNotice");
			int minZhuanSheng = systemXmlItem.GetIntValue("MinZhuanSheng", -1);
			int minLevel = systemXmlItem.GetIntValue("MinLevel", -1);
			if (string.IsNullOrEmpty(times))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析广播配置表中的时间项失败, ID={0}", id), null, true);
			}
			else
			{
				BroadcastTimeItem[] broadcastTimeItemArray = BroadcastInfoMgr.ParseBroadcastTimeItems(times);
				if (null == broadcastTimeItemArray)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析广播配置表中的时间项为数组时失败, ID={0}", id), null, true);
				}
				else if (string.IsNullOrEmpty(text))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析广播配置表中的时间项失败, ID={0}", id), null, true);
				}
				else
				{
					DateTimeRange[] onlineNoticeTimeRanges = Global.ParseDateTimeRangeStr(onlineNotice);
					BroadcastInfoItem broadcastInfoItem = new BroadcastInfoItem
					{
						ID = id,
						InfoClass = infoClass,
						HintErrID = hintErrID,
						TimeType = timeType,
						KaiFuStartDay = kaiFuStartDay,
						KaiFuShowType = kaiFuShowType,
						WeekDays = weekDays,
						Times = broadcastTimeItemArray,
						OnlineNoticeTimeRanges = onlineNoticeTimeRanges,
						Text = text.Replace(":", ""),
						MinZhuanSheng = minZhuanSheng,
						MinLevel = minLevel
					};
					broadcastInfoItemList.Add(broadcastInfoItem);
				}
			}
		}

		// Token: 0x06001BE3 RID: 7139 RVA: 0x001A2B38 File Offset: 0x001A0D38
		private static BroadcastTimeItem[] ParseBroadcastTimeItems(string times)
		{
			BroadcastTimeItem[] result;
			if (string.IsNullOrEmpty(times))
			{
				result = null;
			}
			else
			{
				string[] fields = times.Split(new char[]
				{
					'|'
				});
				if (fields.Length <= 0)
				{
					result = null;
				}
				else
				{
					BroadcastTimeItem[] broadcastTimeItemArray = new BroadcastTimeItem[fields.Length];
					for (int i = 0; i < fields.Length; i++)
					{
						string str = fields[i].Trim();
						if (string.IsNullOrEmpty(str))
						{
							return null;
						}
						string[] fields2 = str.Split(new char[]
						{
							':'
						});
						if (fields2 == null || fields2.Length != 2)
						{
							return null;
						}
						broadcastTimeItemArray[i] = new BroadcastTimeItem
						{
							Hour = Global.SafeConvertToInt32(fields2[0]),
							Minute = Global.SafeConvertToInt32(fields2[1])
						};
					}
					result = broadcastTimeItemArray;
				}
			}
			return result;
		}

		// Token: 0x06001BE4 RID: 7140 RVA: 0x001A2C34 File Offset: 0x001A0E34
		private static bool CanBroadcast(BroadcastInfoItem broadcastInfoItem, BroadcastTimeItem lastBroadcastTimeItem, int weekDayID, int hour, int minute)
		{
			bool result;
			if (null == broadcastInfoItem.Times)
			{
				result = false;
			}
			else
			{
				if (!string.IsNullOrEmpty(broadcastInfoItem.WeekDays))
				{
					if (-1 == broadcastInfoItem.WeekDays.IndexOf(weekDayID.ToString()))
					{
						return false;
					}
				}
				if (broadcastInfoItem.KaiFuStartDay > 0)
				{
					DateTime jugeDateTime = Global.GetKaiFuTime();
					if (2 == broadcastInfoItem.TimeType)
					{
						jugeDateTime = Global.GetHefuStartDay();
					}
					else if (3 == broadcastInfoItem.TimeType)
					{
						jugeDateTime = Global.GetJieriStartDay();
					}
					DateTime todayTime = TimeUtil.NowDateTime();
					int currday = Global.GetOffsetDay(todayTime);
					int jugeday = Global.GetOffsetDay(jugeDateTime);
					if (currday - jugeday < broadcastInfoItem.KaiFuStartDay)
					{
						return false;
					}
					if (broadcastInfoItem.KaiFuShowType > 0)
					{
						if (currday - jugeday >= broadcastInfoItem.KaiFuStartDay + broadcastInfoItem.KaiFuShowType)
						{
							return false;
						}
					}
				}
				int lastTime = lastBroadcastTimeItem.Hour * 60 + lastBroadcastTimeItem.Minute;
				int nowTime = hour * 60 + minute;
				for (int i = 0; i < broadcastInfoItem.Times.Length; i++)
				{
					int itemTime = broadcastInfoItem.Times[i].Hour * 60 + broadcastInfoItem.Times[i].Minute;
					if (itemTime > lastTime)
					{
						if (nowTime >= itemTime)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06001BE5 RID: 7141 RVA: 0x001A2DDC File Offset: 0x001A0FDC
		public static void ProcessBroadcastInfos()
		{
			DateTime now = TimeUtil.NowDateTime();
			int weekDayID = (int)now.DayOfWeek;
			int day = now.DayOfYear;
			int hour = now.Hour;
			int minute = now.Minute;
			if (day != BroadcastInfoMgr.LastBroadcastDay)
			{
				BroadcastInfoMgr.LastBroadcastDay = day;
				BroadcastInfoMgr.LastBroadcastTimeItem.Hour = hour;
				BroadcastInfoMgr.LastBroadcastTimeItem.Minute = minute;
			}
			else if (hour != BroadcastInfoMgr.LastBroadcastTimeItem.Hour || minute != BroadcastInfoMgr.LastBroadcastTimeItem.Minute)
			{
				List<BroadcastInfoItem> broadcastInfoItemList = BroadcastInfoMgr.BroadcastInfoItemList;
				if (broadcastInfoItemList == null || broadcastInfoItemList.Count <= 0)
				{
					BroadcastInfoMgr.LastBroadcastDay = day;
					BroadcastInfoMgr.LastBroadcastTimeItem.Hour = hour;
					BroadcastInfoMgr.LastBroadcastTimeItem.Minute = minute;
				}
				else
				{
					for (int i = 0; i < broadcastInfoItemList.Count; i++)
					{
						if (BroadcastInfoMgr.CanBroadcast(broadcastInfoItemList[i], BroadcastInfoMgr.LastBroadcastTimeItem, weekDayID, hour, minute))
						{
							if (broadcastInfoItemList[i].InfoClass <= 1)
							{
								Global.BroadcastRoleActionMsg(null, (broadcastInfoItemList[i].InfoClass == 0) ? RoleActionsMsgTypes.Bulletin : RoleActionsMsgTypes.HintMsg, broadcastInfoItemList[i].Text, false, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, broadcastInfoItemList[i].MinZhuanSheng, broadcastInfoItemList[i].MinLevel, 100, 100);
							}
							else if (3 == broadcastInfoItemList[i].InfoClass)
							{
								GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, broadcastInfoItemList[i].Text, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Math.Max(broadcastInfoItemList[i].HintErrID, 0), broadcastInfoItemList[i].MinZhuanSheng, broadcastInfoItemList[i].MinLevel, 100, 100);
							}
						}
					}
					BroadcastInfoMgr.LastBroadcastDay = day;
					BroadcastInfoMgr.LastBroadcastTimeItem.Hour = hour;
					BroadcastInfoMgr.LastBroadcastTimeItem.Minute = minute;
				}
			}
		}

		// Token: 0x06001BE6 RID: 7142 RVA: 0x001A3004 File Offset: 0x001A1204
		public static void LoginBroadcastInfos(GameClient client)
		{
			DateTime now = TimeUtil.NowDateTime();
			List<BroadcastInfoItem> broadcastInfoItemList = BroadcastInfoMgr.BroadcastInfoItemList;
			if (broadcastInfoItemList != null && broadcastInfoItemList.Count > 0)
			{
				DateTime todayTime = now;
				int weekDayID = (int)now.DayOfWeek;
				for (int i = 0; i < broadcastInfoItemList.Count; i++)
				{
					if (broadcastInfoItemList[i].InfoClass == 3)
					{
						if (null != broadcastInfoItemList[i].OnlineNoticeTimeRanges)
						{
							if (Global.GetUnionLevel(client, false) >= Global.GetUnionLevel(broadcastInfoItemList[i].MinZhuanSheng, broadcastInfoItemList[i].MinLevel, false))
							{
								int endMinute = 0;
								if (Global.JugeDateTimeInTimeRange(now, broadcastInfoItemList[i].OnlineNoticeTimeRanges, out endMinute, true))
								{
									if (!string.IsNullOrEmpty(broadcastInfoItemList[i].WeekDays))
									{
										if (-1 == broadcastInfoItemList[i].WeekDays.IndexOf(weekDayID.ToString()))
										{
											goto IL_246;
										}
									}
									if (broadcastInfoItemList[i].KaiFuStartDay > 0)
									{
										DateTime jugeDateTime = Global.GetKaiFuTime();
										if (2 == broadcastInfoItemList[i].TimeType)
										{
											jugeDateTime = Global.GetHefuStartDay();
										}
										else if (3 == broadcastInfoItemList[i].TimeType)
										{
											jugeDateTime = Global.GetJieriStartDay();
										}
										int currday = Global.GetOffsetDay(todayTime);
										int jugeday = Global.GetOffsetDay(jugeDateTime);
										if (currday - jugeday < broadcastInfoItemList[i].KaiFuStartDay)
										{
											goto IL_246;
										}
										if (broadcastInfoItemList[i].KaiFuShowType > 0)
										{
											if (currday - jugeday >= broadcastInfoItemList[i].KaiFuStartDay + broadcastInfoItemList[i].KaiFuShowType)
											{
												goto IL_246;
											}
										}
									}
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, broadcastInfoItemList[i].Text, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Math.Max(broadcastInfoItemList[i].HintErrID, 0));
								}
							}
						}
					}
					IL_246:;
				}
			}
		}

		// Token: 0x04002A22 RID: 10786
		private static List<BroadcastInfoItem> BroadcastInfoItemList = null;

		// Token: 0x04002A23 RID: 10787
		private static int LastBroadcastDay = TimeUtil.NowDateTime().DayOfYear;

		// Token: 0x04002A24 RID: 10788
		private static BroadcastTimeItem LastBroadcastTimeItem = new BroadcastTimeItem
		{
			Hour = TimeUtil.NowDateTime().Hour,
			Minute = TimeUtil.NowDateTime().Minute
		};
	}
}
