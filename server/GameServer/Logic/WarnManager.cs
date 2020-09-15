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
	// Token: 0x020007B6 RID: 1974
	public class WarnManager : IManager
	{
		// Token: 0x060033EB RID: 13291 RVA: 0x002DF534 File Offset: 0x002DD734
		public static WarnManager getInstance()
		{
			return WarnManager.instance;
		}

		// Token: 0x060033EC RID: 13292 RVA: 0x002DF54C File Offset: 0x002DD74C
		public bool initialize()
		{
			WarnManager.initWarnInfo();
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("WarnManager.WarnCloseClient()", new EventHandler(WarnManager.WarnCloseClient)), 5000, 5000);
			return true;
		}

		// Token: 0x060033ED RID: 13293 RVA: 0x002DF590 File Offset: 0x002DD790
		public bool startup()
		{
			return true;
		}

		// Token: 0x060033EE RID: 13294 RVA: 0x002DF5A4 File Offset: 0x002DD7A4
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060033EF RID: 13295 RVA: 0x002DF5B8 File Offset: 0x002DD7B8
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060033F0 RID: 13296 RVA: 0x002DF5CC File Offset: 0x002DD7CC
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

		// Token: 0x060033F1 RID: 13297 RVA: 0x002DF710 File Offset: 0x002DD910
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

		// Token: 0x060033F2 RID: 13298 RVA: 0x002DF744 File Offset: 0x002DD944
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

		// Token: 0x060033F3 RID: 13299 RVA: 0x002DF7C0 File Offset: 0x002DD9C0
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

		// Token: 0x060033F4 RID: 13300 RVA: 0x002DF854 File Offset: 0x002DDA54
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

		// Token: 0x04003F6C RID: 16236
		private static WarnManager instance = new WarnManager();

		// Token: 0x04003F6D RID: 16237
		private static Dictionary<int, WarnInfo> _warnInfoList = new Dictionary<int, WarnInfo>();

		// Token: 0x04003F6E RID: 16238
		private static object _lock = new object();

		// Token: 0x04003F6F RID: 16239
		private static Dictionary<GameClient, DateTime> _clientList = new Dictionary<GameClient, DateTime>();
	}
}
