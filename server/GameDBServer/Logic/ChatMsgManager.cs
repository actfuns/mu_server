using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.DB;
using GameServer.Core.AssemblyPatch;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x020001A9 RID: 425
	public class ChatMsgManager
	{
		// Token: 0x06000903 RID: 2307 RVA: 0x00053FD8 File Offset: 0x000521D8
		private static Queue<string> GetChatMsgQueue(int serverLineID)
		{
			Queue<string> msgQueue = null;
			lock (ChatMsgManager.ChatMsgDict)
			{
				if (!ChatMsgManager.ChatMsgDict.TryGetValue(serverLineID, out msgQueue))
				{
					msgQueue = new Queue<string>();
					ChatMsgManager.ChatMsgDict[serverLineID] = msgQueue;
				}
			}
			return msgQueue;
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x00054050 File Offset: 0x00052250
		public static void AddGMCmdChatMsg(int serverLineID, string gmCmd)
		{
			string chatMsg = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				0,
				"",
				0,
				"",
				0,
				gmCmd,
				0,
				0,
				serverLineID
			});
			List<LineItem> itemList = LineManager.GetLineItemList();
			if (null != itemList)
			{
				for (int i = 0; i < itemList.Count; i++)
				{
					if (itemList[i].LineID != serverLineID)
					{
						if (itemList[i].LineID < 9000 || itemList[i].LineID == GameDBManager.ZoneID)
						{
							ChatMsgManager.AddChatMsg(itemList[i].LineID, chatMsg);
						}
					}
				}
			}
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x0005414C File Offset: 0x0005234C
		public static void AddGMCmdChatMsgToOneClient(string gmCmd)
		{
			string chatMsg = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				0,
				"",
				0,
				"",
				0,
				gmCmd,
				0,
				0,
				-1
			});
			List<LineItem> itemList = LineManager.GetLineItemList();
			if (null != itemList)
			{
				for (int i = 0; i < itemList.Count; i++)
				{
					if (itemList[i].LineID < 9000 || itemList[i].LineID == GameDBManager.ZoneID)
					{
						ChatMsgManager.AddChatMsg(itemList[i].LineID, chatMsg);
						break;
					}
				}
			}
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x00054250 File Offset: 0x00052450
		public static void AddChatMsg(int serverLineID, string chatMsg)
		{
			LogManager.WriteLog(LogTypes.SQL, string.Format("AddChatMsg:LineID={0},Msg={1}", serverLineID, chatMsg), null, true);
			Queue<string> msgQueue = ChatMsgManager.GetChatMsgQueue(serverLineID);
			lock (msgQueue)
			{
				if (msgQueue.Count > 30000)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("线路{0}的转发消息太多，被丢弃，一共丢弃{1}条，请检查GameServer是否正常", serverLineID, msgQueue.Count), null, true);
					List<string> cmdList = msgQueue.ToList<string>();
					msgQueue.Clear();
					Dictionary<string, int> cmdAnalysis = new Dictionary<string, int>();
					foreach (string cmd in cmdList)
					{
						string szKey = string.Empty;
						try
						{
							szKey = cmd.Split(new char[]
							{
								':'
							})[5].Split(new char[]
							{
								' '
							})[0];
						}
						catch
						{
						}
						if (!string.IsNullOrEmpty(szKey))
						{
							if (cmdAnalysis.ContainsKey(szKey))
							{
								Dictionary<string, int> dictionary;
								string key;
								(dictionary = cmdAnalysis)[key = szKey] = dictionary[key] + 1;
							}
							else
							{
								cmdAnalysis[szKey] = 1;
							}
							if (szKey.StartsWith("-buyyueka") || szKey.StartsWith("-updateyb") || szKey.StartsWith("-updateBindgold") || szKey.StartsWith("-config"))
							{
								msgQueue.Enqueue(cmd);
							}
						}
					}
					if (msgQueue.Count<string>() >= 15000)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("线路{0}丢失重要命令{1}条", serverLineID, msgQueue.Count<string>()), null, true);
						msgQueue.Clear();
					}
					List<KeyValuePair<string, int>> cmdAnaList = cmdAnalysis.ToList<KeyValuePair<string, int>>();
					cmdAnaList.Sort((KeyValuePair<string, int> _left, KeyValuePair<string, int> _right) => _right.Value - _left.Value);
					StringBuilder sb = new StringBuilder();
					sb.Append("转发消息统计,").AppendFormat("共有{0}类消息:    ", cmdAnaList.Count<KeyValuePair<string, int>>()).AppendLine();
					for (int i = 0; i < cmdAnaList.Count<KeyValuePair<string, int>>(); i++)
					{
						string _cmd = cmdAnaList[i].Key;
						int _cnt = cmdAnaList[i].Value;
						if (_cnt <= 10)
						{
							break;
						}
						sb.AppendFormat("   cmd={0}, cnt={1}", _cmd, _cnt).AppendLine();
					}
					LogManager.WriteLog(LogTypes.Error, string.Format("线路{0}的转发消息太多，丢弃日志分析如下{1}", serverLineID, sb.ToString()), null, true);
				}
				msgQueue.Enqueue(chatMsg);
			}
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x00054590 File Offset: 0x00052790
		public static TCPOutPacket GetWaitingChatMsg(TCPOutPacketPool pool, int cmdID, int serverLineID)
		{
			List<string> msgList = new List<string>();
			Queue<string> msgQueue = ChatMsgManager.GetChatMsgQueue(serverLineID);
			lock (msgQueue)
			{
				while (msgQueue.Count > 0 && msgList.Count < 250)
				{
					msgList.Add(msgQueue.Dequeue());
				}
			}
			return DataHelper.ObjectToTCPOutPacket<List<string>>(msgList, pool, cmdID);
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x00054624 File Offset: 0x00052824
		public static void ScanGMMsgToGameServer(DBManager dbMgr)
		{
			try
			{
				long nowTicks = DateTime.Now.Ticks / 10000L;
				if (nowTicks - ChatMsgManager.LastScanInputGMMsgTicks >= 10000L)
				{
					ChatMsgManager.LastScanInputGMMsgTicks = nowTicks;
					List<string> msgList = new List<string>();
					DBQuery.ScanGMMsgFromTable(dbMgr, msgList);
					bool reloadConfig = false;
					bool reloadGameserverLineList = false;
					bool reloadGMail = false;
					for (int i = 0; i < msgList.Count; i++)
					{
						string msg = msgList[i].Replace(":", "：");
						if (msg.IndexOf("-config ") >= 0)
						{
							reloadConfig = true;
							string[] fields = msg.Trim().Split(new char[]
							{
								' '
							});
							if (fields.Count<string>() == 3)
							{
								string paramName = fields[1];
								string paramValue = fields[2];
								DBWriter.UpdateGameConfig(dbMgr, paramName, paramValue);
							}
						}
						else if (msg == "-reload kuafu")
						{
							reloadGameserverLineList = true;
						}
						else if (msg == "-reloadall")
						{
							try
							{
								AssemblyPatchManager.getInstance().InitConfig();
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
							}
						}
						if (msg.IndexOf("-resetgmail") >= 0)
						{
							reloadGMail = true;
						}
						if (msg.IndexOf("-outrank") >= 0)
						{
							GameDBManager.RankCacheMgr.PrintfRankData();
						}
						ChatMsgManager.AddGMCmdChatMsg(-1, msg);
					}
					if (reloadConfig)
					{
						GameDBManager.GameConfigMgr.LoadGameConfigFromDB(dbMgr);
					}
					if (reloadGameserverLineList)
					{
						LineManager.LoadConfig();
					}
					if (reloadGMail)
					{
						GroupMailManager.ResetData();
					}
				}
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("扫描GM命令表时发生了错误", new object[0]), null, true);
			}
		}

		// Token: 0x040009AD RID: 2477
		private static Dictionary<int, Queue<string>> ChatMsgDict = new Dictionary<int, Queue<string>>();

		// Token: 0x040009AE RID: 2478
		private static long LastScanInputGMMsgTicks = DateTime.Now.Ticks / 10000L;
	}
}
