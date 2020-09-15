using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameServer.Core.Executor;
using Server.Data;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000249 RID: 585
	internal class FileBanLogic
	{
		// Token: 0x0600080B RID: 2059 RVA: 0x0007A628 File Offset: 0x00078828
		private static void LoadBanFile()
		{
			long currTicks = TimeUtil.NOW();
			if (currTicks - FileBanLogic.m_UpdateTicks >= 10000L)
			{
				FileBanLogic.m_UpdateTicks = currTicks;
				IEnumerable<string> files = from file in Directory.EnumerateFiles(DataHelper.CurrentDirectory, "Ban*.txt", SearchOption.AllDirectories)
				select file;
				foreach (string file2 in files)
				{
					FileStream fs = null;
					try
					{
						fs = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.None);
					}
					catch
					{
						fs = null;
					}
					if (null != fs)
					{
						StreamReader sr = new StreamReader(fs, Encoding.Default);
						string strLine;
						while (null != (strLine = sr.ReadLine()))
						{
							string[] userid = strLine.Split(new char[]
							{
								','
							});
							if (userid.Length > 0)
							{
								FileBanLogic.m_BanList.Add(userid[0]);
							}
						}
						sr.Close();
						fs.Close();
						FileInfo fi = new FileInfo(file2);
						if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
						{
							fi.Attributes = FileAttributes.Normal;
						}
						File.Delete(file2);
					}
				}
			}
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0007A7EC File Offset: 0x000789EC
		public static void Tick()
		{
			FileBanLogic.LoadBanFile();
			if (null != FileBanLogic.m_BanList)
			{
				if (FileBanLogic.m_IsNeedClear > 0)
				{
					FileBanLogic.m_BanList.Clear();
					FileBanLogic.m_IsNeedClear = 0;
				}
				bool bCrashForce = GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("CrashUnityForce");
				int i = 20;
				while (i > 0 && FileBanLogic.m_BanList.Count > 0)
				{
					i--;
					string userID = FileBanLogic.m_BanList[FileBanLogic.m_BanList.Count - 1];
					FileBanLogic.m_BanList.RemoveAt(FileBanLogic.m_BanList.Count - 1);
					BanManager.BanUserID2Memory(userID);
					TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
					if (null != clientSocket)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(clientSocket);
						if (null != gameClient)
						{
							RoleData roleData = new RoleData
							{
								RoleID = -70
							};
							gameClient.sendCmd<RoleData>(104, roleData, false);
							if (bCrashForce)
							{
								FileBanLogic.SendMagicCrashMsg(gameClient, MagicCrashUnityType.CrashTimeOut);
							}
							LogManager.WriteLog(LogTypes.FileBan, string.Format("FileBanLogic ban2 userID={0} roleID={1}", userID, gameClient.ClientData.RoleID), null, true);
						}
						else
						{
							Global.ForceCloseSocket(clientSocket, "被禁止登陆", true);
							LogManager.WriteLog(LogTypes.FileBan, string.Format("FileBanLogic ForceCloseSocket userID={0}", userID), null, true);
						}
					}
				}
			}
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x0007A965 File Offset: 0x00078B65
		public static void ClearBanList()
		{
			FileBanLogic.m_IsNeedClear = 1;
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x0007A970 File Offset: 0x00078B70
		public static void BroadCastDetectHook()
		{
			int index = 0;
			GameClient client;
			while ((client = GameManager.ClientMgr.GetNextClient(ref index, true)) != null)
			{
				if (client != null)
				{
					FileBanLogic.SendMagicCrashMsg(client, MagicCrashUnityType.DetectHook);
				}
			}
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0007A9B4 File Offset: 0x00078BB4
		public static void BroadCastDetectHook(List<string> uidList)
		{
			if (uidList != null)
			{
				foreach (string uid in uidList)
				{
					TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(uid);
					if (null != clientSocket)
					{
						GameClient client = GameManager.ClientMgr.FindClient(clientSocket);
						if (client != null)
						{
							FileBanLogic.SendMagicCrashMsg(client, MagicCrashUnityType.DetectHook);
						}
					}
				}
			}
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x0007AA4C File Offset: 0x00078C4C
		private static void SendMagicCrashMsg(GameClient client, MagicCrashUnityType crashType)
		{
			if (client != null)
			{
				int timeOutSec = Global.GetRandomNumber(5, 15);
				string cmd = string.Format("{0}:{1}", (int)crashType, timeOutSec);
				client.sendCmd(14010, cmd, false);
			}
		}

		// Token: 0x04000DE9 RID: 3561
		private static List<string> m_BanList = new List<string>();

		// Token: 0x04000DEA RID: 3562
		private static long m_UpdateTicks = 0L;

		// Token: 0x04000DEB RID: 3563
		private static int m_IsNeedClear = 0;
	}
}
