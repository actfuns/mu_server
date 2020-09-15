using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000778 RID: 1912
	public class PopupWinMgr
	{
		// Token: 0x06003108 RID: 12552 RVA: 0x002B70F4 File Offset: 0x002B52F4
		public static void LoadPopupWinItemList()
		{
			XElement xml = null;
			string fileName = "Config/PopupWin.xml";
			try
			{
				xml = XElement.Load(Global.GameResPath(fileName));
				if (null == xml)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
			}
			List<PopupWinItem> popupWinItemList = new List<PopupWinItem>();
			IEnumerable<XElement> nodes = xml.Elements();
			foreach (XElement node in nodes)
			{
				SystemXmlItem systemXmlItem = new SystemXmlItem
				{
					XMLNode = node
				};
				PopupWinMgr.ParseXmlItem(systemXmlItem, popupWinItemList);
			}
			PopupWinMgr.PopupWinItemList = popupWinItemList;
		}

		// Token: 0x06003109 RID: 12553 RVA: 0x002B71D8 File Offset: 0x002B53D8
		private static void ParseXmlItem(SystemXmlItem systemXmlItem, List<PopupWinItem> popupWinItemList)
		{
			int id = systemXmlItem.GetIntValue("ID", -1);
			int hintFileID = systemXmlItem.GetIntValue("HintFileID", -1);
			string times = systemXmlItem.GetStringValue("Times");
			if (!string.IsNullOrEmpty(times))
			{
				PopupWinTimeItem[] popupWinTimeItemArray = PopupWinMgr.ParsePopupWinTimeItems(times);
				if (null == popupWinTimeItemArray)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析弹窗配置表中的时间项为数组时失败, ID={0}", id), null, true);
				}
				else
				{
					PopupWinItem popupWinItem = new PopupWinItem
					{
						ID = id,
						HintFileID = hintFileID,
						Times = popupWinTimeItemArray
					};
					popupWinItemList.Add(popupWinItem);
				}
			}
		}

		// Token: 0x0600310A RID: 12554 RVA: 0x002B727C File Offset: 0x002B547C
		private static PopupWinTimeItem[] ParsePopupWinTimeItems(string times)
		{
			string[] fields = times.Split(new char[]
			{
				'|'
			});
			PopupWinTimeItem[] result;
			if (fields.Length <= 0)
			{
				result = null;
			}
			else
			{
				PopupWinTimeItem[] popupWinTimeItemArray = new PopupWinTimeItem[fields.Length];
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
					popupWinTimeItemArray[i] = new PopupWinTimeItem
					{
						Hour = Global.SafeConvertToInt32(fields2[0]),
						Minute = Global.SafeConvertToInt32(fields2[1])
					};
				}
				result = popupWinTimeItemArray;
			}
			return result;
		}

		// Token: 0x0600310B RID: 12555 RVA: 0x002B7360 File Offset: 0x002B5560
		private static bool CanPopupWin(PopupWinItem popupWinItem, PopupWinTimeItem lastPopupWinTimeItem, int hour, int minute)
		{
			bool result;
			if (null == popupWinItem.Times)
			{
				result = false;
			}
			else
			{
				int time2 = lastPopupWinTimeItem.Hour * 60 + lastPopupWinTimeItem.Minute;
				int time3 = hour * 60 + minute;
				for (int i = 0; i < popupWinItem.Times.Length; i++)
				{
					int time4 = popupWinItem.Times[i].Hour * 60 + popupWinItem.Times[i].Minute;
					if (time4 > time2)
					{
						if (time3 >= time4)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600310C RID: 12556 RVA: 0x002B73FC File Offset: 0x002B55FC
		public static void ProcessPopupWins()
		{
			DateTime now = TimeUtil.NowDateTime();
			int day = now.DayOfYear;
			int hour = now.Hour;
			int minute = now.Minute;
			if (day != PopupWinMgr.LastPopupWinDay)
			{
				PopupWinMgr.LastPopupWinDay = day;
				PopupWinMgr.LastPopupWinTimeItem.Hour = hour;
				PopupWinMgr.LastPopupWinTimeItem.Minute = minute;
			}
			else if (hour != PopupWinMgr.LastPopupWinTimeItem.Hour || minute != PopupWinMgr.LastPopupWinTimeItem.Minute)
			{
				List<PopupWinItem> popupWinItemList = PopupWinMgr.PopupWinItemList;
				if (popupWinItemList == null || popupWinItemList.Count <= 0)
				{
					PopupWinMgr.LastPopupWinDay = day;
					PopupWinMgr.LastPopupWinTimeItem.Hour = hour;
					PopupWinMgr.LastPopupWinTimeItem.Minute = minute;
				}
				else
				{
					for (int i = 0; i < popupWinItemList.Count; i++)
					{
						if (PopupWinMgr.CanPopupWin(popupWinItemList[i], PopupWinMgr.LastPopupWinTimeItem, hour, minute))
						{
							string strcmd = string.Format("{0}", popupWinItemList[i].HintFileID);
							GameManager.ClientMgr.NotifyAllPopupWinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, strcmd);
						}
					}
					PopupWinMgr.LastPopupWinDay = day;
					PopupWinMgr.LastPopupWinTimeItem.Hour = hour;
					PopupWinMgr.LastPopupWinTimeItem.Minute = minute;
				}
			}
		}

		// Token: 0x0600310D RID: 12557 RVA: 0x002B7564 File Offset: 0x002B5764
		public static void ProcessClientPopupWins(GameClient client)
		{
			List<PopupWinItem> popupWinItemList = PopupWinMgr.PopupWinItemList;
			if (popupWinItemList != null && popupWinItemList.Count > 0)
			{
				if (popupWinItemList[0].Times.Length > 0)
				{
					string strcmd = string.Format("{0}", popupWinItemList[0].HintFileID);
					GameManager.ClientMgr.NotifyPopupWinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strcmd);
				}
			}
		}

		// Token: 0x04003D8D RID: 15757
		private static List<PopupWinItem> PopupWinItemList = null;

		// Token: 0x04003D8E RID: 15758
		private static int LastPopupWinDay = TimeUtil.NowDateTime().DayOfYear;

		// Token: 0x04003D8F RID: 15759
		private static PopupWinTimeItem LastPopupWinTimeItem = new PopupWinTimeItem
		{
			Hour = TimeUtil.NowDateTime().Hour,
			Minute = TimeUtil.NowDateTime().Minute
		};
	}
}
