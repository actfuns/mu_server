using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameDBServer.Server;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x020001CF RID: 463
	public class LineManager
	{
		// Token: 0x060009BB RID: 2491 RVA: 0x0005D6FC File Offset: 0x0005B8FC
		public static void LoadConfig()
		{
			bool success = false;
			Dictionary<int, LineItem> linesDict = new Dictionary<int, LineItem>();
			try
			{
				XElement xml = XElement.Load("GameServer.xml");
				IEnumerable<XElement> mapItems = xml.Element("GameServer").Elements();
				foreach (XElement mapItem in mapItems)
				{
					LineItem lineItem = new LineItem
					{
						LineID = (int)Global.GetSafeAttributeLong(mapItem, "ID"),
						GameServerIP = Global.GetSafeAttributeStr(mapItem, "ip"),
						GameServerPort = (int)Global.GetSafeAttributeLong(mapItem, "port"),
						OnlineCount = 0
					};
					linesDict[lineItem.LineID] = lineItem;
					success = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				success = false;
			}
			if (success)
			{
				lock (LineManager.Mutex)
				{
					foreach (LineItem item in linesDict.Values)
					{
						if (!LineManager._LinesDict.ContainsKey(item.LineID))
						{
							LineManager._LinesDict.Add(item.LineID, item);
						}
					}
					LineManager._LinesList = LineManager._LinesDict.Values.ToList<LineItem>();
				}
			}
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x0005D8D8 File Offset: 0x0005BAD8
		public static void UpdateLineHeart(GameServerClient client, int lineID, int onlineNum, string strMapOnlineNum = "")
		{
			lock (LineManager.Mutex)
			{
				LineItem lineItem = null;
				if (LineManager._LinesDict.TryGetValue(lineID, out lineItem))
				{
					lineItem.OnlineCount = onlineNum;
					lineItem.OnlineTicks = DateTime.Now.Ticks / 10000L;
					lineItem.MapOnlineNum = strMapOnlineNum;
					lineItem.ServerClient = client;
					client.LineId = lineID;
				}
				else if (lineID >= 9000)
				{
					lineItem = new LineItem();
					LineManager._LinesDict[lineID] = lineItem;
					lineItem.LineID = lineID;
					lineItem.OnlineCount = onlineNum;
					lineItem.OnlineTicks = DateTime.Now.Ticks / 10000L;
					lineItem.MapOnlineNum = strMapOnlineNum;
					lineItem.ServerClient = client;
					client.LineId = lineID;
					if (!LineManager._LinesList.Contains(lineItem))
					{
						LineManager._LinesList.Add(lineItem);
					}
				}
			}
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x0005D9F4 File Offset: 0x0005BBF4
		public static GameServerClient GetGameServerClient(int lineId)
		{
			lock (LineManager.Mutex)
			{
				LineItem lineItem = null;
				if (LineManager._LinesDict.TryGetValue(lineId, out lineItem))
				{
					return lineItem.ServerClient;
				}
			}
			return null;
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x0005DA64 File Offset: 0x0005BC64
		public static int GetLineHeartState(int lineID)
		{
			long ticks = DateTime.Now.Ticks / 10000L;
			int state = 0;
			lock (LineManager.Mutex)
			{
				LineItem lineItem = null;
				if (LineManager._LinesDict.TryGetValue(lineID, out lineItem))
				{
					if (ticks - lineItem.OnlineTicks < 60000L)
					{
						state = 1;
					}
				}
			}
			return state;
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x0005DB04 File Offset: 0x0005BD04
		public static List<LineItem> GetLineItemList()
		{
			List<LineItem> lineItemList = new List<LineItem>();
			long ticks = DateTime.Now.Ticks / 10000L;
			lock (LineManager.Mutex)
			{
				for (int i = 0; i < LineManager._LinesList.Count; i++)
				{
					if (ticks - LineManager._LinesList[i].OnlineTicks < 180000L)
					{
						lineItemList.Add(LineManager._LinesList[i]);
					}
				}
			}
			return lineItemList;
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x0005DBC4 File Offset: 0x0005BDC4
		public static int GetTotalOnlineNum()
		{
			int totalNum = 0;
			long ticks = DateTime.Now.Ticks / 10000L;
			lock (LineManager.Mutex)
			{
				for (int i = 0; i < LineManager._LinesList.Count; i++)
				{
					if (ticks - LineManager._LinesList[i].OnlineTicks < 60000L)
					{
						totalNum += LineManager._LinesList[i].OnlineCount;
					}
				}
			}
			return totalNum;
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x0005DC80 File Offset: 0x0005BE80
		public static string GetMapOnlineNum()
		{
			string strMapOnlineInfo = "";
			long ticks = DateTime.Now.Ticks / 10000L;
			lock (LineManager.Mutex)
			{
				for (int i = 0; i < LineManager._LinesList.Count; i++)
				{
					if (ticks - LineManager._LinesList[i].OnlineTicks < 60000L)
					{
						return LineManager._LinesList[i].MapOnlineNum;
					}
				}
			}
			return strMapOnlineInfo;
		}

		// Token: 0x04000BEC RID: 3052
		private static object Mutex = new object();

		// Token: 0x04000BED RID: 3053
		private static Dictionary<int, LineItem> _LinesDict = new Dictionary<int, LineItem>();

		// Token: 0x04000BEE RID: 3054
		private static List<LineItem> _LinesList = new List<LineItem>();
	}
}
