using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameDBServer.Server;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class LineManager
	{
		
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

		
		private static object Mutex = new object();

		
		private static Dictionary<int, LineItem> _LinesDict = new Dictionary<int, LineItem>();

		
		private static List<LineItem> _LinesList = new List<LineItem>();
	}
}
