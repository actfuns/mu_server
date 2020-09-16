using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Tools;
using Server.Data;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class WarnManager : IManager
	{
		
		public static WarnManager getInstance()
		{
			return WarnManager.instance;
		}

		
		public bool initialize()
		{
			WarnManager.initWarnInfo();
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("WarnManager.WarnCloseClient()", new EventHandler(WarnManager.WarnCloseClient)), 5000, 5000);
			return true;
		}

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public static void initWarnInfo()
		{
			string fileName = Global.IsolateResPath("Config/JingGao.xml");
			XElement xml = CheckHelper.LoadXml(fileName, true);
			if (null != xml)
			{
				try
				{
					WarnManager._warnInfoList.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							WarnInfo config = new WarnInfo();
							config.ID = Convert.ToInt32(Global.GetSafeAttributeLong(xmlItem, "ID"));
							config.Desc = Global.GetSafeAttributeStr(xmlItem, "Description");
							config.TimeSec = Convert.ToInt32(Global.GetSafeAttributeLong(xmlItem, "Time"));
							config.Operate = Convert.ToInt32(Global.GetSafeAttributeLong(xmlItem, "Operate"));
							WarnManager._warnInfoList.Add(config.Operate, config);
						}
					}
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载[{0}]时出错!!!", fileName), null, true);
				}
			}
		}

		
		private static WarnInfo GetWarnInfo(int warnType)
		{
			WarnInfo result;
			if (WarnManager._warnInfoList.ContainsKey(warnType))
			{
				result = WarnManager._warnInfoList[warnType];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public static void WarnProcess(string userID, int warnType)
		{
			WarnInfo info = WarnManager.GetWarnInfo(warnType);
			if (info != null)
			{
				TMSKSocket socket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
				if (socket != null)
				{
					GameClient client = GameManager.ClientMgr.FindClient(socket);
					if (null != client)
					{
						client.sendCmd<WarnInfo>(1004, info, false);
						if (info.Operate != 1)
						{
							WarnManager.AddTaskToHashSet(client, info.TimeSec);
						}
					}
				}
			}
		}

		
		private static void AddTaskToHashSet(GameClient client, int time)
		{
			lock (WarnManager._lock)
			{
				if (!WarnManager._clientList.ContainsKey(client))
				{
					WarnManager._clientList.Add(client, TimeUtil.NowDateTime().AddSeconds((double)time));
					if (WarnManager._clientList.Count >= 3000)
					{
						WarnManager.WarnCloseClient(null, null);
					}
				}
			}
		}

		
		public static void WarnCloseClient(object sender, EventArgs e)
		{
			try
			{
				Dictionary<GameClient, DateTime> dic = new Dictionary<GameClient, DateTime>();
				List<GameClient> list = new List<GameClient>();
				lock (WarnManager._lock)
				{
					foreach (KeyValuePair<GameClient, DateTime> c in WarnManager._clientList)
					{
						DateTime endTime = c.Value;
						if (TimeUtil.NowDateTime() >= endTime)
						{
							list.Add(c.Key);
						}
						else
						{
							dic.Add(c.Key, c.Value);
						}
					}
					WarnManager._clientList.Clear();
					WarnManager._clientList = dic;
				}
				foreach (GameClient client in list)
				{
					Global.ForceCloseClient(client, "warn踢人", true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, ex.Message, null, true);
			}
		}

		
		private static WarnManager instance = new WarnManager();

		
		private static Dictionary<int, WarnInfo> _warnInfoList = new Dictionary<int, WarnInfo>();

		
		private static object _lock = new object();

		
		private static Dictionary<GameClient, DateTime> _clientList = new Dictionary<GameClient, DateTime>();
	}
}
