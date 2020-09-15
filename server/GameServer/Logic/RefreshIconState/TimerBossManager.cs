using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic.RefreshIconState
{
	// Token: 0x02000786 RID: 1926
	public class TimerBossManager
	{
		// Token: 0x060031CF RID: 12751 RVA: 0x002C8F97 File Offset: 0x002C7197
		private TimerBossManager()
		{
		}

		// Token: 0x060031D0 RID: 12752 RVA: 0x002C8FC4 File Offset: 0x002C71C4
		private void LoadWorldBossInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/Activity/BossInfo.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> WorldBossXEle = xmlFile.Elements("Boss");
					foreach (XElement xmlItem in WorldBossXEle)
					{
						if (null != xmlItem)
						{
							SystemXmlItem systemXmlItem = new SystemXmlItem
							{
								XMLNode = xmlItem
							};
							TimerBossData tmpInfo = new TimerBossData();
							tmpInfo.nRoleID = systemXmlItem.GetIntValue("ID", -1);
							int[] arrLevel = systemXmlItem.GetIntArrayValue("Level", ',');
							if (arrLevel == null || arrLevel.Length != 2)
							{
								throw new Exception(string.Format("启动时加载xml文件: {0} 失败 Level格式错误", string.Format("Config/Activity/BossInfo.xml", new object[0])));
							}
							tmpInfo.nReqLevel = arrLevel[1];
							tmpInfo.nReqChangeLiveCount = arrLevel[0];
							this.m_WorldBossDict.Add(tmpInfo.nRoleID, tmpInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Activity/BossInfo.xml", new object[0])));
			}
		}

		// Token: 0x060031D1 RID: 12753 RVA: 0x002C915C File Offset: 0x002C735C
		private void LoadHuangJinBossInfo()
		{
			try
			{
				XElement xmlFile = Global.GetGameResXml(string.Format("Config/HuangJin.xml", new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> WorldBossXEle = xmlFile.Elements("Boss");
					foreach (XElement xmlItem in WorldBossXEle)
					{
						if (null != xmlItem)
						{
							SystemXmlItem systemXmlItem = new SystemXmlItem
							{
								XMLNode = xmlItem
							};
							TimerBossData tmpInfo = new TimerBossData();
							tmpInfo.nRoleID = systemXmlItem.GetIntValue("ID", -1);
							int[] arrLevel = systemXmlItem.GetIntArrayValue("Level", ',');
							if (arrLevel == null || arrLevel.Length != 2)
							{
								throw new Exception(string.Format("启动时加载xml文件: {0} 失败 Level格式错误", string.Format("Config/HuangJin.xml", new object[0])));
							}
							tmpInfo.nReqLevel = arrLevel[1];
							tmpInfo.nReqChangeLiveCount = arrLevel[0];
							this.m_HuangJinBossDict.Add(tmpInfo.nRoleID, tmpInfo);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/HuangJin.xml", new object[0])));
			}
		}

		// Token: 0x060031D2 RID: 12754 RVA: 0x002C92F4 File Offset: 0x002C74F4
		public static TimerBossManager getInstance()
		{
			if (null == TimerBossManager.instance)
			{
				lock (TimerBossManager.Mutex)
				{
					if (null == TimerBossManager.instance)
					{
						TimerBossManager inst = new TimerBossManager();
						inst.LoadWorldBossInfo();
						inst.LoadHuangJinBossInfo();
						TimerBossManager.instance = inst;
					}
				}
			}
			return TimerBossManager.instance;
		}

		// Token: 0x060031D3 RID: 12755 RVA: 0x002C9380 File Offset: 0x002C7580
		public void AddBoss(int nBirthType, int nRoleID)
		{
			lock (this.m_LivedInMapBoss)
			{
				this.m_LivedInMapBoss[nRoleID] = nBirthType;
			}
			if (nBirthType == 1)
			{
				this.RefreshWorldBoss();
			}
			else if (nBirthType == 7)
			{
				this.RefreshHuangJinBoss();
			}
		}

		// Token: 0x060031D4 RID: 12756 RVA: 0x002C9400 File Offset: 0x002C7600
		public void RemoveBoss(int nRoleID)
		{
			int nBirthType = 0;
			lock (this.m_LivedInMapBoss)
			{
				if (!this.m_LivedInMapBoss.TryGetValue(nRoleID, out nBirthType))
				{
					return;
				}
				this.m_LivedInMapBoss.Remove(nRoleID);
			}
			if (nBirthType == 1)
			{
				this.RefreshWorldBoss();
			}
			else if (nBirthType == 7)
			{
				this.RefreshHuangJinBoss();
			}
		}

		// Token: 0x060031D5 RID: 12757 RVA: 0x002C9494 File Offset: 0x002C7694
		public bool HaveWorldBoss(GameClient client)
		{
			lock (this.m_LivedInMapBoss)
			{
				foreach (KeyValuePair<int, int> kvp in this.m_LivedInMapBoss)
				{
					if (kvp.Value == 1)
					{
						TimerBossData bossData = null;
						if (this.m_WorldBossDict.TryGetValue(kvp.Key, out bossData))
						{
							if (null != bossData)
							{
								if ((client.ClientData.ChangeLifeCount == bossData.nReqChangeLiveCount && client.ClientData.Level >= bossData.nReqLevel) || client.ClientData.ChangeLifeCount > bossData.nReqChangeLiveCount)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060031D6 RID: 12758 RVA: 0x002C95BC File Offset: 0x002C77BC
		public bool HaveHuangJinBoss(GameClient client)
		{
			lock (this.m_LivedInMapBoss)
			{
				foreach (KeyValuePair<int, int> kvp in this.m_LivedInMapBoss)
				{
					if (kvp.Value == 7)
					{
						TimerBossData bossData = null;
						if (this.m_HuangJinBossDict.TryGetValue(kvp.Key, out bossData))
						{
							if (null != bossData)
							{
								if ((client.ClientData.ChangeLifeCount == bossData.nReqChangeLiveCount && client.ClientData.Level >= bossData.nReqLevel) || client.ClientData.ChangeLifeCount > bossData.nReqChangeLiveCount)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060031D7 RID: 12759 RVA: 0x002C96E4 File Offset: 0x002C78E4
		public void RefreshHuangJinBoss()
		{
			int count = GameManager.ClientMgr.GetMaxClientCount();
			for (int i = 0; i < count; i++)
			{
				GameClient client = GameManager.ClientMgr.FindClientByNid(i);
				if (null != client)
				{
					client._IconStateMgr.CheckHuangJinBoss(client);
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		// Token: 0x060031D8 RID: 12760 RVA: 0x002C9740 File Offset: 0x002C7940
		public void RefreshWorldBoss()
		{
			int count = GameManager.ClientMgr.GetMaxClientCount();
			for (int i = 0; i < count; i++)
			{
				GameClient client = GameManager.ClientMgr.FindClientByNid(i);
				if (null != client)
				{
					client._IconStateMgr.CheckShiJieBoss(client);
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		// Token: 0x04003E5C RID: 15964
		private Dictionary<int, TimerBossData> m_WorldBossDict = new Dictionary<int, TimerBossData>();

		// Token: 0x04003E5D RID: 15965
		private Dictionary<int, TimerBossData> m_HuangJinBossDict = new Dictionary<int, TimerBossData>();

		// Token: 0x04003E5E RID: 15966
		private Dictionary<int, int> m_LivedInMapBoss = new Dictionary<int, int>();

		// Token: 0x04003E5F RID: 15967
		private static TimerBossManager instance = null;

		// Token: 0x04003E60 RID: 15968
		private static object Mutex = new object();
	}
}
