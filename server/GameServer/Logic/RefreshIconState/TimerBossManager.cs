using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic.RefreshIconState
{
	
	public class TimerBossManager
	{
		
		private TimerBossManager()
		{
		}

		
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

		
		private Dictionary<int, TimerBossData> m_WorldBossDict = new Dictionary<int, TimerBossData>();

		
		private Dictionary<int, TimerBossData> m_HuangJinBossDict = new Dictionary<int, TimerBossData>();

		
		private Dictionary<int, int> m_LivedInMapBoss = new Dictionary<int, int>();

		
		private static TimerBossManager instance = null;

		
		private static object Mutex = new object();
	}
}
