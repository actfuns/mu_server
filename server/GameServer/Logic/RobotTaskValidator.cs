using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using ComponentAce.Compression.Libs.zlib;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using ProtoBuf;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200024C RID: 588
	internal class RobotTaskValidator
	{
		// Token: 0x06000821 RID: 2081 RVA: 0x0007B0D8 File Offset: 0x000792D8
		private RobotTaskValidator()
		{
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x0007B2A8 File Offset: 0x000794A8
		public static RobotTaskValidator getInstance()
		{
			return RobotTaskValidator.instance;
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000823 RID: 2083 RVA: 0x0007B2C0 File Offset: 0x000794C0
		// (set) Token: 0x06000824 RID: 2084 RVA: 0x0007B2D8 File Offset: 0x000794D8
		public bool UseWorkThread
		{
			get
			{
				return this._useWorkThread;
			}
			set
			{
				this._useWorkThread = value;
			}
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x0007B2E4 File Offset: 0x000794E4
		public bool Initialize(bool client, int seed, int randomCount, string pubKey)
		{
			try
			{
				if (null == this.BackgroundThread)
				{
					this.BackgroundThread = new Thread(new ThreadStart(this.TimerProc));
					this.BackgroundThread.IsBackground = true;
					this.BackgroundThread.Start();
				}
				if (!client)
				{
					this.m_TaskListVerifySeed = Environment.TickCount;
					this.m_TaskListRSA = new RSACryptoServiceProvider(1024);
					this.m_TaskListRSA.PersistKeyInCsp = false;
					this.m_TaskListRSAPubKey = this.m_TaskListRSA.ToXmlString(false);
					ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("RobotTaskValidator.FlushTaskListToFile()", new EventHandler(this.FlushTaskListToFile)), 1800000, 1800000);
					return this.LoadRobotTaskData();
				}
				this.m_TaskListVerifySeed = seed;
				this.m_TaskListVerifyRandomCount = randomCount;
				this.m_TaskListRSA = new RSACryptoServiceProvider();
				this.m_TaskListRSA.PersistKeyInCsp = false;
				this.m_TaskListRSA.FromXmlString(pubKey);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(string.Format("rsa create failure: {0}", ex.ToString()));
				return false;
			}
			return true;
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x0007B418 File Offset: 0x00079618
		private uint GenMagicRandom(uint seed, int loop)
		{
			uint w = seed;
			uint z = 362436069U;
			for (int i = 0; i <= loop; i++)
			{
				z = 36969U * (z & 65535U) + (z >> 16);
				w = 18000U * (w & 65535U) + (w >> 16);
			}
			return (z << 16) + w;
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x0007B478 File Offset: 0x00079678
		private void AddTaskToHashSet(string taskId)
		{
			using (this.m_Mutex.Enter(4))
			{
				if (this.m_AllTaskHashSet.Add(taskId))
				{
					if (this.m_AllTaskHashSet.Count >= 3000)
					{
						this.FlushTaskListToFile(null, null);
					}
				}
			}
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x0007B4F0 File Offset: 0x000796F0
		public void FlushTaskListToFile(object sender, EventArgs e)
		{
			try
			{
				StringBuilder sb = new StringBuilder("-------------------------\r\n");
				using (this.m_Mutex.Enter(5))
				{
					try
					{
						foreach (string taskId in this.m_AllTaskHashSet)
						{
							sb.AppendLine(taskId);
						}
					}
					finally
					{
						this.m_AllTaskHashSet.Clear();
					}
				}
				LogManager.WriteLog(LogTypes.task, sb.ToString(), null, true);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0007B5DC File Offset: 0x000797DC
		public void SendTaskListKey(GameClient client)
		{
			if (!client.CheckCheatData.RobotTaskCheckInitialed)
			{
				if (!client.ClientSocket.session.IsGM)
				{
					if (this.BanIsOpen())
					{
						this.UpdateTaskListTimeout(client);
						string data = string.Format("{0}:{1}:{2}", this.m_TaskListVerifySeed, this.m_TaskListVerifyRandomCount, this.m_TaskListRSAPubKey);
						client.sendCmd(30000, data, false);
						client.sendCmd<RobotTaskValidator.TaskCheckList>(699, this.m_TaskCheckList, false);
						client.CheckCheatData.RobotTaskCheckInitialed = true;
					}
				}
			}
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0007B684 File Offset: 0x00079884
		public bool BanIsOpen()
		{
			return this._BanIsOpen;
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x0007B69C File Offset: 0x0007989C
		public bool YueYuIsOpen()
		{
			using (this.m_Mutex.Enter(6))
			{
				if (this._yyOpenPlatform != null && this._yyOpenPlatform.ContainsKey(this._platformType) && this._yyOpenPlatform[this._platformType] > 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x0007B720 File Offset: 0x00079920
		public bool IsYueYu(GameClient client, string appStr)
		{
			using (this.m_Mutex.Enter(7))
			{
				if (!client.IsYueYu || !this.BanIsOpen() || !this.YueYuIsOpen())
				{
					return false;
				}
				if (this._yueyuAppDic == null || this._yueyuAppDic.Count <= 0 || this._yueyuAppDic.ContainsKey("null"))
				{
					return false;
				}
				if (this._yueyuAppDic.ContainsKey("*") || this._yueyuAppDic.ContainsKey(appStr))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x0007B7EC File Offset: 0x000799EC
		public bool BanYueYu(GameClient client, out bool isLog, string clientData = "0", int jailbreak = 1, int autoStart = 0, string taskStr = "*", int banCount = 0)
		{
			isLog = false;
			switch (this._banReasonDic[BanReasonType.YueYu])
			{
			case 1:
				isLog = true;
				return this.BanLog(client, BanType.BanLog, clientData, jailbreak, autoStart, taskStr, banCount, false);
			case 2:
				return this.BanKick(client, BanType.BanKick, clientData, jailbreak, autoStart, taskStr, banCount, true);
			case 3:
				return this.BanClose(client, BanType.BanClose, clientData, jailbreak, autoStart, taskStr, banCount, true);
			case 6:
				return this.BanRate(client, BanType.BanRate, clientData, jailbreak, autoStart, taskStr, banCount, true);
			}
			return false;
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x0007B88C File Offset: 0x00079A8C
		public bool LoadRobotTaskData()
		{
			this._platformType = GameCoreInterface.getinstance().GetPlatformType();
			this._BanIsOpen = false;
			string fileName = "";
			using (this.m_Mutex.Enter(8))
			{
				try
				{
					fileName = Global.GameResPath("Config/TaskCheckList.xml");
					if (File.Exists(fileName))
					{
						this.m_TaskCheckList.TaskCheckListData = File.ReadAllBytes(fileName);
					}
					fileName = Global.GameResPath("Config/AssInfo.xml");
					XElement xmlFile = CheckHelper.LoadXml(fileName, false);
					if (null == xmlFile)
					{
						return false;
					}
					IEnumerable<XElement> nodes = xmlFile.Elements();
					this.m_RobotBuildList.Clear();
					this.m_RobotCPUList.Clear();
					this.m_RobotPhoneList.Clear();
					foreach (XElement node in nodes)
					{
						string id = Global.GetSafeAttributeStr(node, "ID");
						id = id.ToLower();
						if (node.Name.LocalName == "RobotBuild")
						{
							if (!this.m_RobotBuildList.ContainsKey(id))
							{
								this.m_RobotBuildList.Add(id, 0);
							}
						}
						else if (node.Name.LocalName == "RobotCPU")
						{
							if (!this.m_RobotCPUList.ContainsKey(id))
							{
								this.m_RobotCPUList.Add(id, 0);
							}
						}
						else if (node.Name.LocalName == "RobotPhone")
						{
							if (!this.m_RobotPhoneList.ContainsKey(id))
							{
								this.m_RobotPhoneList.Add(id, 0);
							}
						}
						else if (node.Name.LocalName == "RobotSign")
						{
							if (!this.m_RobotSignList.ContainsKey(id))
							{
								this.m_RobotSignList.Add(id, 0);
							}
						}
						else if (node.Name.LocalName == "YueYuDev")
						{
							this.m_YueYuDevList[id] = 0;
						}
						else if (node.Name.LocalName == "YueYuOSVer")
						{
							this.m_YueYuOSVerList[id] = 0;
						}
						else if (string.Compare(node.Name.LocalName, "NoYueYuDev", true) == 0)
						{
							string os = Global.GetSafeAttributeStr(node, "OS").ToLower();
							List<string> list;
							if (!this.m_NewYueYuDevOSVerListDict.TryGetValue(id, out list))
							{
								list = new List<string>();
								this.m_NewYueYuDevOSVerListDict[id] = list;
							}
							list.Add(os);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
			}
			using (this.m_Mutex.Enter(9))
			{
				try
				{
					fileName = Global.GameResPath("Config/AssList.xml");
					XElement xmlFile = CheckHelper.LoadXml(fileName, false);
					if (null == xmlFile)
					{
						return false;
					}
					IEnumerable<XElement> nodes = xmlFile.Elements();
					this.m_RobotTaskList.Clear();
					foreach (XElement node in nodes)
					{
						string id = Global.GetSafeAttributeStr(node, "ID");
						int level = int.Parse(Global.GetSafeAttributeStr(node, "Level"));
						int banType = int.Parse(Global.GetSafeAttributeStr(node, "BanType"));
						if (!this.m_RobotTaskList.ContainsKey(id))
						{
							BanInfo info = new BanInfo();
							info.BanID = id;
							info.Level = level;
							info.BanType = (BanType)banType;
							this.m_RobotTaskList.Add(id, info);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
					return false;
				}
			}
			using (this.m_Mutex.Enter(10))
			{
				try
				{
					fileName = Global.GameResPath("Config/AssConfig.xml");
					XElement xmlFile = CheckHelper.LoadXml(fileName, false);
					if (null == xmlFile)
					{
						return false;
					}
					this._useWorkThread = (ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "UseWorkThread", "Value", 0L) > 0L);
					int[] platformState = ConfigHelper.GetElementAttributeValueIntArray(xmlFile, "Param", "Name", "BanOpenPlayform", "Value", null);
					this._banOpenPlatform.Clear();
					if (platformState != null && platformState.Length > 1)
					{
						for (int p = 0; p < platformState.Length; p++)
						{
							PlatformTypes t = (PlatformTypes)platformState[p++];
							int state = platformState[p];
							if (!this._banOpenPlatform.ContainsKey(t))
							{
								this._banOpenPlatform.Add(t, state);
							}
						}
					}
					int[] yyPlatform = ConfigHelper.GetElementAttributeValueIntArray(xmlFile, "Param", "Name", "YueYuPlayform", "Value", null);
					this._yyOpenPlatform.Clear();
					if (yyPlatform != null && yyPlatform.Length > 1)
					{
						for (int p = 0; p < yyPlatform.Length; p++)
						{
							PlatformTypes t = (PlatformTypes)yyPlatform[p++];
							int state = yyPlatform[p];
							if (!this._yyOpenPlatform.ContainsKey(t))
							{
								this._yyOpenPlatform.Add(t, state);
							}
						}
					}
					this._jiaoBenIsOpen = (ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "JiaoBenIsOpen", "Value", 0L) > 0L);
					this._vmIsOpen = (ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "VmIsOpen", "Value", 0L) > 0L);
					this._vmSignGameOpenSeven = ConfigHelper.GetElementAttributeValueIntArray(xmlFile, "Param", "Name", "VmSignGameOpenSeveb", "Value", new int[]
					{
						0,
						7,
						210
					});
					this._yueYuGameOpenSeveb = ConfigHelper.GetElementAttributeValueIntArray(xmlFile, "Param", "Name", "YueYuGameOpenSeveb", "Value", new int[]
					{
						0,
						7,
						210
					});
					int[] mapcodes = ConfigHelper.GetElementAttributeValueIntArray(xmlFile, "Param", "Name", "MapCode", "Value", new int[0]);
					foreach (int i in mapcodes)
					{
						this.VMBanMapCodes.Add(i);
					}
					this._timeOutCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "TimeOutCount", "Value", 5L);
					this._timeOutMinute = ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "TimeOutMinute", "Value", 5L) * 60L * 1000L;
					this._banCheckMaxCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "BanCheckMaxCount", "Value", 10L);
					this._kickWarnMaxCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "KickWarnMaxCount", "Value", 2L);
					this._banLevelLimit = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "BanLevelLimit", "Value", 401L);
					this._banVipLimit = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "BanVipLimit", "Value", 13L);
					XElement xml = xmlFile;
					string name = "Param";
					string attrName = "Name";
					string attrValue = "RobotBanMinutes";
					string attribute = "Value";
					int[] defArr = new int[1];
					this.m_BanMiuteList = ConfigHelper.GetElementAttributeValueIntArray(xml, name, attrName, attrValue, attribute, defArr);
					this.m_BanMinutes = this.m_BanMiuteList[0];
					this._logCountDic.Clear();
					int[] logArr = ConfigHelper.GetElementAttributeValueIntArray(xmlFile, "Param", "Name", "BanLogCountLimit", "Value", null);
					if (logArr != null && logArr.Length > 1)
					{
						for (int j = 0; j < logArr.Length; j++)
						{
							this._logCountDic.Add(logArr[j], logArr[++j]);
						}
					}
					this._banReasonDic.Clear();
					int[] banReasonArr = ConfigHelper.GetElementAttributeValueIntArray(xmlFile, "Param", "Name", "BanReason", "Value", null);
					if (banReasonArr != null && banReasonArr.Length > 1)
					{
						for (int j = 0; j < banReasonArr.Length; j++)
						{
							this._banReasonDic.Add((BanReasonType)banReasonArr[j], banReasonArr[++j]);
						}
					}
					string[] cancelList = ConfigHelper.GetElementAttributeValueStrArray(xmlFile, "Param", "Name", "CancelApp", "Value", null);
					this._cancelAppDic.Clear();
					if (cancelList != null && cancelList.Length > 0)
					{
						foreach (string s in cancelList)
						{
							this._cancelAppDic.Add(s, 0);
						}
					}
					string[] yueyuList = ConfigHelper.GetElementAttributeValueStrArray(xmlFile, "Param", "Name", "YueYuApp", "Value", null);
					this._yueyuAppDic.Clear();
					if (yueyuList != null && yueyuList.Length > 0)
					{
						foreach (string s in yueyuList)
						{
							this._yueyuAppDic.Add(s, 0);
						}
					}
					this._specialAppList = ConfigHelper.GetElementAttributeValueStrArray(xmlFile, "Param", "Name", "SpecialApp", "Value", new string[0]);
					this._appCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "AppCount", "Value", 15L);
					this._multiCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "MultiCount", "Value", 2L);
					this._decryptCount = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "DecryptCount", "Value", 5L);
					this._banRateNum = (ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "BanRateNum", "Value", 0L) > 0L);
					this._canLogIp = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "Param", "Name", "CanLogIp", "Value", 0L);
					this._appPlatformCount = ConfigHelper.GetElementAttributeValueIntArray(xmlFile, "Param", "Name", "AppPlayformCount", "Value", new int[]
					{
						0,
						1,
						5
					});
					this.m_DropRateDownMonsterTypeHashSet.Clear();
					int[] dropRateDownMonsterTypeArray = ConfigHelper.GetElementAttributeValueIntArray(xmlFile, "Param", "Name", "DropRateDownMonsterType", "Value", null);
					if (dropRateDownMonsterTypeArray != null && dropRateDownMonsterTypeArray.Length > 0)
					{
						foreach (int t2 in dropRateDownMonsterTypeArray)
						{
							this.m_DropRateDownMonsterTypeHashSet.TryAdd(t2, 0);
						}
					}
					this._DropRateDownMapDict.Clear();
					string jiaoBenDropRateDown = ConfigHelper.GetElementAttributeValue(xmlFile, "Param", "Name", "DropRateDownMap", "Value", "");
					string[] jiaoBenDropRateDown_List = jiaoBenDropRateDown.Split(new char[]
					{
						'|'
					});
					for (int j = 0; j < jiaoBenDropRateDown_List.Length; j++)
					{
						string[] dropRateData = jiaoBenDropRateDown_List[j].Split(new char[]
						{
							','
						});
						if (dropRateData.Length == 2)
						{
							int sceneID = Convert.ToInt32(dropRateData[0]);
							if (sceneID > 0)
							{
								double dropRate = Convert.ToDouble(dropRateData[1]);
								this._DropRateDownMapDict.TryAdd(sceneID, dropRate);
							}
						}
					}
					bool isPlatform = false;
					bool isBan = false;
					if (this._banOpenPlatform != null && this._banOpenPlatform.ContainsKey(this._platformType) && this._banOpenPlatform[this._platformType] > 0)
					{
						isPlatform = true;
					}
					if (this._jiaoBenIsOpen || this._vmIsOpen)
					{
						isBan = true;
					}
					this._BanIsOpen = (isBan && isPlatform);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return true;
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x0007C65C File Offset: 0x0007A85C
		public void UpdateTaskListTimeout(GameClient client)
		{
			if (this.BanIsOpen())
			{
				client.CheckCheatData.NextTaskListTimeout = TimeUtil.NOW() + this._timeOutMinute;
			}
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x0007C690 File Offset: 0x0007A890
		public bool KickTimeout(GameClient client)
		{
			bool result;
			if (!this.BanIsOpen())
			{
				result = false;
			}
			else
			{
				if (client.CheckCheatData.NextTaskListTimeout > 0L)
				{
					long nowTicks = TimeUtil.NOW();
					if (nowTicks > client.CheckCheatData.NextTaskListTimeout)
					{
						if (client.ClientSocket.session.TimeOutCount >= this._timeOutCount)
						{
							using (this.m_Mutex.Enter(11))
							{
								switch (this._banReasonDic[BanReasonType.TimeOut])
								{
								case 81:
									this.BanKick(client, BanType.BanKickTimeOut, client.CheckCheatData.RobotTaskListData, 0, 0, client.ClientSocket.session.TimeOutCount.ToString(), -1, false);
									break;
								case 82:
									this.BanClose(client, BanType.BanCloseTimeOut, client.CheckCheatData.RobotTaskListData, 0, 0, client.ClientSocket.session.TimeOutCount.ToString(), -1, false);
									break;
								case 83:
									this.BanRate(client, BanType.BanRateTimeOut, client.CheckCheatData.RobotTaskListData, 0, 0, client.ClientSocket.session.TimeOutCount.ToString(), -1, false);
									break;
								}
								this.UpdateTaskListTimeout(client);
								return false;
							}
						}
						int cmdID = client.ClientSocket.session.CmdID;
						long cmdTime = client.ClientSocket.session.CmdTime;
						if (cmdID > 0 && cmdTime + this._timeOutMinute > nowTicks)
						{
							if (this._platformType == PlatformTypes.APP || this._platformType == PlatformTypes.YueYu)
							{
								if (string.IsNullOrEmpty(client.deviceID))
								{
									switch (this._banReasonDic[BanReasonType.TimeOutDeviceNull])
									{
									case 41:
										this.BanKick(client, BanType.BanKickDeviceNull, client.CheckCheatData.RobotTaskListData, 0, 0, "", -1, false);
										break;
									case 42:
										this.BanClose(client, BanType.BanCloseDeviceNull, client.CheckCheatData.RobotTaskListData, 0, 0, "", -1, false);
										break;
									case 43:
										this.BanRate(client, BanType.BanRateDeviceNull, client.CheckCheatData.RobotTaskListData, 0, 0, "", -1, false);
										break;
									}
								}
							}
							client.ClientSocket.session.TimeOutCountAdd();
							this.BanLog(client, BanType.BanLogTimeOut, client.CheckCheatData.RobotTaskListData, 0, 0, client.ClientSocket.session.TimeOutCount.ToString(), -1, true);
							this.UpdateTaskListTimeout(client);
							return false;
						}
					}
					if (client.CheckCheatData.RobotDetectedKickTime > 0L && nowTicks > client.CheckCheatData.RobotDetectedKickTime)
					{
						Global.ForceCloseClient(client, client.CheckCheatData.RobotDetectedReason, true);
						this.RobotDataReset(client);
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x0007C9C0 File Offset: 0x0007ABC0
		public TCPProcessCmdResults ProcessTaskList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				if (!this.BanIsOpen())
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (client != null)
				{
					if (this._useWorkThread)
					{
						byte[] copyData = new byte[count];
						Array.Copy(data, copyData, count);
						this.AddRobotDataItem(nID, client, copyData);
					}
					else
					{
						this.ValidateTaskList(data, client);
						this.UpdateTaskListTimeout(client);
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_OK;
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x0007CA70 File Offset: 0x0007AC70
		private void AddRobotDataItem(int cmdId, GameClient client, byte[] data)
		{
			RobotDataItem item = new RobotDataItem
			{
				CmdId = cmdId,
				Client = client,
				Data = data
			};
			lock (this.RobotDataItemQueue)
			{
				this.RobotDataItemQueue.Enqueue(item);
			}
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x0007CAE4 File Offset: 0x0007ACE4
		public void TimerProc()
		{
			for (;;)
			{
				try
				{
					Thread.Sleep(60000);
					RobotDataItem[] itemArray = null;
					lock (this.RobotDataItemQueue)
					{
						if (this.RobotDataItemQueue.Count <= 0)
						{
							continue;
						}
						itemArray = this.RobotDataItemQueue.ToArray();
						this.RobotDataItemQueue.Clear();
					}
					if (itemArray != null && itemArray.Length > 0)
					{
						foreach (RobotDataItem item in itemArray)
						{
							try
							{
								if (item != null && item.Client != null && item.Data != null)
								{
									this.ValidateTaskList(item.Data, item.Client);
									this.UpdateTaskListTimeout(item.Client);
								}
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
							}
						}
					}
				}
				catch (Exception ex)
				{
				}
			}
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x0007CC50 File Offset: 0x0007AE50
		public int BanYueYu(GameClient client, string devInfo)
		{
			int result;
			if (this._platformType != PlatformTypes.APP)
			{
				result = 0;
			}
			else if (string.IsNullOrEmpty(client.deviceModel) || string.IsNullOrEmpty(client.deviceOSVersion))
			{
				result = 0;
			}
			else
			{
				using (this.m_Mutex.Enter(16))
				{
					try
					{
						if (this._yueYuGameOpenSeveb[0] == 0)
						{
							return 0;
						}
						bool forceDo = false;
						if (client.IsYueYu)
						{
							foreach (KeyValuePair<string, int> validateData in this.m_YueYuDevList)
							{
								string dev = client.deviceModel.ToLower();
								if (dev == validateData.Key)
								{
									foreach (string osVer in this.m_YueYuOSVerList.Keys)
									{
										string os = client.deviceOSVersion.ToLower();
										if (os.StartsWith(osVer))
										{
											forceDo = true;
											break;
										}
									}
									break;
								}
							}
						}
						else
						{
							string dev = client.deviceModel.ToLower();
							List<string> list;
							if (this.m_NewYueYuDevOSVerListDict.TryGetValue(dev, out list))
							{
								string osVer = client.deviceOSVersion.ToLower();
								foreach (string os in list)
								{
									if (os == osVer)
									{
										forceDo = true;
										break;
									}
								}
							}
						}
						if (forceDo)
						{
							if (this.VMBanMapCodes.Contains(client.ClientData.MapCode) || Global.GetKaiFuTime().AddDays((double)this._yueYuGameOpenSeveb[1]) > DateTime.Now)
							{
								switch (this._yueYuGameOpenSeveb[2])
								{
								case 1:
									this.BanLog(client, BanType.BanLog, devInfo, 0, 0, "", -1, false);
									break;
								case 2:
									this.BanKick(client, BanType.BanKick, devInfo, 0, 0, "", -1, false);
									break;
								case 3:
									this.BanClose(client, BanType.BanClose, devInfo, 0, 0, "", -1, false);
									break;
								case 6:
									this.BanRate(client, BanType.BanRate, devInfo, 0, 0, "", -1, false);
									break;
								}
							}
							return 0;
						}
					}
					catch
					{
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0007CFC4 File Offset: 0x0007B1C4
		public bool ValidateTaskList(byte[] data, GameClient client)
		{
			bool result;
			if (client.CheckCheatData.BanCheckMaxCount >= this._banCheckMaxCount)
			{
				result = true;
			}
			else
			{
				bool isLog = false;
				string clientData = "";
				List<byte> rawData = new List<byte>();
				try
				{
					using (this.m_Mutex.Enter(12))
					{
						List<byte> listData = new List<byte>(data);
						int i = 0;
						while (i < data.Length)
						{
							byte len = listData[i];
							byte[] section = new byte[(int)len];
							listData.CopyTo(i + 1, section, 0, (int)len);
							i++;
							i += (int)len;
							byte[] decryptData = this.m_TaskListRSA.Decrypt(section, false);
							if (rawData == null)
							{
								clientData += new UTF8Encoding().GetString(decryptData, 0, decryptData.Length);
							}
							else
							{
								for (int j = 0; j < decryptData.Length; j++)
								{
									rawData.Add(decryptData[j]);
								}
							}
						}
						if (rawData != null)
						{
							this.m_Mutex.SetTag(13);
							byte[] uncompressData;
							using (MemoryStream ms = new MemoryStream())
							{
								using (ZOutputStream outZStream = new ZOutputStream(ms))
								{
									outZStream.Write(rawData.ToArray(), 0, rawData.Count);
									outZStream.Flush();
								}
								uncompressData = ms.ToArray();
							}
							clientData = new UTF8Encoding().GetString(uncompressData, 0, uncompressData.Length);
						}
					}
				}
				catch (Exception e)
				{
					if (clientData == null)
					{
						clientData = "";
					}
					if (client.ClientSocket.session.DecryptCount >= this._decryptCount)
					{
						using (this.m_Mutex.Enter(14))
						{
							switch (this._banReasonDic[BanReasonType.Decrypt])
							{
							case 91:
								this.BanKick(client, BanType.BanKickDecrypt, clientData, 0, 0, client.ClientSocket.session.DecryptCount.ToString(), -1, false);
								break;
							case 92:
								this.BanClose(client, BanType.BanCloseDecrypt, clientData, 0, 0, client.ClientSocket.session.DecryptCount.ToString(), -1, false);
								break;
							case 93:
								this.BanRate(client, BanType.BanRateDecrypt, clientData, 0, 0, client.ClientSocket.session.DecryptCount.ToString(), -1, false);
								break;
							}
							return false;
						}
					}
					client.ClientSocket.session.DecryptCountAdd();
					this.BanLog(client, BanType.BanLogDecrypt, clientData, 0, 0, client.ClientSocket.session.DecryptCount.ToString(), -1, true);
					return false;
				}
				string[] fields = clientData.Split(new char[]
				{
					':'
				});
				if (fields.Length < 5)
				{
					result = this.BanLog(client, BanType.BanLogInvalid, clientData, 0, 0, "data_length", -1, false);
				}
				else
				{
					int clientTick = 0;
					int clientRandomValue = 0;
					int jailbreak = 0;
					int autoStart = 0;
					try
					{
						clientTick = Convert.ToInt32(fields[1]);
						clientRandomValue = Convert.ToInt32(fields[2]);
						jailbreak = Convert.ToInt32(fields[3]);
						autoStart = Convert.ToInt32(fields[4]);
					}
					catch (Exception e)
					{
						return this.BanLog(client, BanType.BanLogInvalid, clientData, jailbreak, autoStart, "data_type", -1, false);
					}
					int randomCount = clientTick % this.m_TaskListVerifyRandomCount;
					int randomValue = (int)this.GenMagicRandom((uint)this.m_TaskListVerifySeed, Math.Abs(randomCount));
					if (randomValue != clientRandomValue)
					{
						result = this.BanLog(client, BanType.BanLogInvalid, clientData, jailbreak, autoStart, "random", -1, false);
					}
					else
					{
						client.CheckCheatData.RobotTaskListData = clientData;
						string[] taskList = fields[0].Split(new char[]
						{
							'*'
						});
						int appLen = 0;
						using (this.m_Mutex.Enter(15))
						{
							int banCount = Global.GetRoleParamsInt32FromDB(client, "BanRobotCount");
							if (this._jiaoBenIsOpen)
							{
								appLen = taskList.Length;
								if (appLen == 1 && fields[0].Trim() == "")
								{
									appLen = 0;
								}
								this.m_Mutex.SetTag(16);
								if ((this._platformType == PlatformTypes.APP || this._platformType == PlatformTypes.YueYu) && !string.IsNullOrEmpty(clientData) && clientData.IndexOf("MODEL|") > 0 && clientData.IndexOf("MANUFACTURER|") > 0 && clientData.IndexOf("PRODUCT|") > 0)
								{
									switch (this._banReasonDic[BanReasonType.AppVM])
									{
									case 20:
										isLog = true;
										this.BanLog(client, BanType.BanLogAppVM, clientData, jailbreak, autoStart, taskList.Length.ToString(), banCount, false);
										break;
									case 21:
										this.BanKick(client, BanType.BanKickAppVM, clientData, jailbreak, autoStart, taskList.Length.ToString(), banCount, false);
										break;
									case 22:
										this.BanClose(client, BanType.BanCloseAppVM, clientData, jailbreak, autoStart, taskList.Length.ToString(), banCount, false);
										break;
									case 23:
										this.BanRate(client, BanType.BanRateAppVM, clientData, jailbreak, autoStart, taskList.Length.ToString(), banCount, false);
										break;
									}
								}
								if (appLen <= this._appCount)
								{
									this.m_Mutex.SetTag(17);
									switch (this._banReasonDic[BanReasonType.AppCount])
									{
									case 60:
										isLog = true;
										this.BanLog(client, BanType.BanLogAppCount, clientData, jailbreak, autoStart, appLen.ToString(), banCount, false);
										break;
									case 61:
										return this.BanKick(client, BanType.BanKickAppCount, clientData, jailbreak, autoStart, appLen.ToString(), banCount, false);
									case 62:
										return this.BanClose(client, BanType.BanCloseAppCount, clientData, jailbreak, autoStart, appLen.ToString(), banCount, false);
									case 63:
										return this.BanRate(client, BanType.BanRateAppCount, clientData, jailbreak, autoStart, appLen.ToString(), banCount, false);
									}
								}
								if (this._platformType == PlatformTypes.APP && this._appPlatformCount != null && this._appPlatformCount.Length == 3 && this._appPlatformCount[0] > 0 && appLen >= this._appPlatformCount[1] && appLen <= this._appPlatformCount[2])
								{
									this.m_Mutex.SetTag(18);
									switch (this._banReasonDic[BanReasonType.AppPlatformCount])
									{
									case 30:
										isLog = true;
										this.BanLog(client, BanType.BanLogAppPlatformCount, clientData, jailbreak, autoStart, appLen.ToString(), banCount, false);
										break;
									case 31:
										return this.BanKick(client, BanType.BanKickAppPlatformCount, clientData, jailbreak, autoStart, appLen.ToString(), banCount, false);
									case 32:
										return this.BanClose(client, BanType.BanCloseAppPlatformCount, clientData, jailbreak, autoStart, appLen.ToString(), banCount, false);
									case 33:
										return this.BanRate(client, BanType.BanRateAppPlatformCount, clientData, jailbreak, autoStart, appLen.ToString(), banCount, false);
									}
								}
								Dictionary<string, int> taskDic = new Dictionary<string, int>();
								for (int j = 0; j < appLen; j++)
								{
									this.AddTaskToHashSet(taskList[j]);
									string taskStr = this.GetTaskName(taskList[j]);
									if (!this._cancelAppDic.ContainsKey(taskStr))
									{
										if (taskDic.ContainsKey(taskStr))
										{
											Dictionary<string, int> dictionary;
											string key;
											(dictionary = taskDic)[key = taskStr] = dictionary[key] + 1;
										}
										else
										{
											taskDic.Add(taskStr, 1);
										}
									}
								}
								this.m_Mutex.SetTag(19);
								int maxCount = 0;
								StringBuilder sb = new StringBuilder();
								foreach (KeyValuePair<string, int> t in taskDic)
								{
									if (t.Value >= this._multiCount)
									{
										sb.Append(t.Key).Append("*").Append(t.Value).Append("|");
										if (maxCount < t.Value)
										{
											maxCount = t.Value;
										}
									}
								}
								if (maxCount >= this._multiCount)
								{
									this.m_Mutex.SetTag(20);
									switch (this._banReasonDic[BanReasonType.MultiApp])
									{
									case 70:
										isLog = true;
										this.BanLog(client, BanType.BanLogMulti, clientData, jailbreak, autoStart, sb.ToString(), banCount, false);
										break;
									case 71:
										return this.BanKick(client, BanType.BanKickMulti, clientData, jailbreak, autoStart, sb.ToString(), banCount, false);
									case 72:
										return this.BanClose(client, BanType.BanCloseMulti, clientData, jailbreak, autoStart, sb.ToString(), banCount, false);
									case 73:
										return this.BanRate(client, BanType.BanRateMulti, clientData, jailbreak, autoStart, sb.ToString(), banCount, false);
									}
								}
								foreach (string s in this._specialAppList)
								{
                                    if (taskDic.ContainsKey(s) && taskDic[s] >= this._multiCount)
									{
										switch (this._banReasonDic[BanReasonType.SpecialApp])
										{
										case 50:
											isLog = true;
											this.BanLog(client, BanType.BanLogSpecialApp, clientData, jailbreak, autoStart, s, banCount, false);
											break;
										case 51:
											return this.BanKick(client, BanType.BanKickSpecialApp, clientData, jailbreak, autoStart, s, banCount, false);
										case 52:
											return this.BanClose(client, BanType.BanCloseSpecialApp, clientData, jailbreak, autoStart, s, banCount, false);
										case 53:
											return this.BanRate(client, BanType.BanRateSpecialApp, clientData, jailbreak, autoStart, s, banCount, false);
										}
									}
								}
								this.m_Mutex.SetTag(21);
								for (int i = 0; i < appLen; i++)
								{
									string taskStr = this.GetTaskName(taskList[i]);
									if (this.IsYueYu(client, taskStr))
									{
										bool yyLog = false;
										this.BanYueYu(client, out yyLog, clientData, jailbreak, autoStart, taskStr, banCount);
									}
									if (this.m_RobotTaskList.ContainsKey(taskStr))
									{
										BanInfo banInfo = this.m_RobotTaskList[taskStr];
										int roleLevel = client.ClientData.Level + client.ClientData.ChangeLifeCount * 100;
										if (roleLevel < banInfo.Level)
										{
											switch (banInfo.BanType)
											{
											case BanType.Old:
												return this.KickWarn(client, banInfo.BanType, clientData, jailbreak, autoStart, taskList[i], banCount, false);
											case BanType.BanLog:
												isLog = true;
												this.BanLog(client, banInfo.BanType, clientData, jailbreak, autoStart, taskList[i], banCount, false);
												break;
											case BanType.BanKick:
											case BanType.BanKickBreak:
												return this.BanKick(client, banInfo.BanType, clientData, jailbreak, autoStart, taskList[i], banCount, true);
											case BanType.BanClose:
											case BanType.BanCloseBreak:
												return this.BanClose(client, banInfo.BanType, clientData, jailbreak, autoStart, taskList[i], banCount, true);
											case BanType.BanRate:
												return this.BanRate(client, banInfo.BanType, clientData, jailbreak, autoStart, taskList[i], banCount, true);
											}
										}
										else
										{
											isLog = true;
											this.BanLog(client, BanType.BanLogBig, clientData, jailbreak, autoStart, taskList[i], -1, false);
										}
									}
									else
									{
										try
										{
											if (!client.CheckCheatData.DropRateDown && this._banRateNum)
											{
												string s = taskList[i];
												char[] arr = s.ToCharArray();
												for (int a = 0; a < arr.Length; a++)
												{
													int t2 = 0;
													if (int.TryParse(arr[a].ToString(), out t2))
													{
														long aa = 0L;
														long.TryParse(taskList[i], NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out aa);
														if (aa != 0L)
														{
															this.BanRate(client, BanType.BanRateNum, clientData, jailbreak, autoStart, taskList[i], banCount, false);
															break;
														}
													}
												}
											}
										}
										catch (Exception e)
										{
											LogManager.WriteException("反外挂：数值进程检查异常");
										}
									}
								}
							}
							if (this._vmIsOpen && fields.Length > 5 && fields[5].Length != 0)
							{
								bool isVirtual = false;
								string taskID = "";
								string[] infoList = fields[5].Split(new char[]
								{
									'*'
								});
								if (infoList.Length < 4)
								{
									return this.BanLog(client, BanType.BanLogInvalid, clientData, jailbreak, autoStart, "vm_info", -1, false);
								}
								try
								{
									bool forceDo = false;
									foreach (KeyValuePair<string, int> validateData in this.m_RobotSignList)
									{
										if (fields[5].Contains(validateData.Key))
										{
											forceDo = true;
											taskID = validateData.Key;
											break;
										}
									}
									this.m_Mutex.SetTag(22);
									if (forceDo)
									{
										int banType = this._banReasonDic[BanReasonType.VMSign];
										if (this._vmSignGameOpenSeven[0] > 0 && (this.VMBanMapCodes.Contains(client.ClientData.MapCode) || Global.GetKaiFuTime().AddDays((double)this._vmSignGameOpenSeven[1]) > DateTime.Now))
										{
											banType = this._vmSignGameOpenSeven[2];
										}
										switch (banType)
										{
										case 120:
											isLog = true;
											this.BanLog(client, BanType.VmLogSign, clientData, jailbreak, autoStart, taskID, -1, false);
											break;
										case 121:
											return this.BanKick(client, BanType.VmKickSign, clientData, jailbreak, autoStart, taskID, -1, false);
										case 122:
											return this.BanClose(client, BanType.VmCloseSign, clientData, jailbreak, autoStart, taskID, -1, false);
										case 123:
											return this.BanRate(client, BanType.VmRateSign, clientData, jailbreak, autoStart, taskID, -1, false);
										}
										return true;
									}
								}
								catch
								{
									isLog = true;
									this.BanLog(client, BanType.BanLogInvalid, clientData, jailbreak, autoStart, "vm_sign", -1, false);
								}
								bool isX86 = false;
								bool isArm = false;
								for (int i = 0; i < 3; i++)
								{
									Dictionary<string, int> validateList = null;
									bool isWhiteList = true;
									bool logOnly = false;
									infoList[i] = infoList[i].ToLower();
									switch (i)
									{
									case 0:
										validateList = this.m_RobotBuildList;
										isWhiteList = false;
										if (infoList[i].Contains("abi:x86"))
										{
											isX86 = true;
										}
										break;
									case 1:
										validateList = this.m_RobotCPUList;
										if (infoList[i].Contains("arm") || infoList[i].Contains("aarch"))
										{
											isArm = true;
										}
										break;
									case 2:
										validateList = this.m_RobotPhoneList;
										logOnly = true;
										break;
									}
									if (!logOnly)
									{
										if (isArm && isX86)
										{
											taskID = "isArm&isX86";
											isVirtual = true;
											break;
										}
										this.m_Mutex.SetTag(23);
										if (isWhiteList)
										{
											isVirtual = (validateList.Count != 0);
											foreach (KeyValuePair<string, int> validateData in validateList)
											{
												if (infoList[i].Contains(validateData.Key))
												{
													isVirtual = false;
													break;
												}
											}
										}
										else
										{
											isVirtual = false;
											foreach (KeyValuePair<string, int> validateData in validateList)
											{
												if (infoList[i].Contains(validateData.Key))
												{
													isVirtual = true;
													taskID = validateData.Key;
													break;
												}
											}
										}
										if (isVirtual)
										{
											break;
										}
									}
								}
								if (isVirtual)
								{
									switch (this._banReasonDic[BanReasonType.VM])
									{
									case 110:
										isLog = true;
										this.BanLog(client, BanType.VmLog, clientData, jailbreak, autoStart, taskID, -1, false);
										break;
									case 111:
										this.BanKick(client, BanType.VmKick, clientData, jailbreak, autoStart, taskID, -1, false);
										break;
									case 112:
										this.BanClose(client, BanType.VmClose, clientData, jailbreak, autoStart, taskID, -1, false);
										break;
									case 113:
										this.BanRate(client, BanType.VmRate, clientData, jailbreak, autoStart, taskID, -1, false);
										break;
									}
								}
							}
						}
						if (!isLog)
						{
							this.BanLog(client, BanType.BanLogNormal, clientData, jailbreak, autoStart, "", -1, false);
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x0007E2A4 File Offset: 0x0007C4A4
		public bool KickWarn(GameClient client, BanType banType, string clientData, int jailbreak = 0, int autoStart = 0, string taskID = "", int banCount = -1, bool forceKick = false)
		{
			client.CheckCheatData.KickWarnMaxCount++;
			this.BanLog(client, banType, clientData, jailbreak, autoStart, taskID, banCount, false);
			if (this.m_BanMinutes > 0 && this._jiaoBenIsOpen)
			{
				if (client.CheckCheatData.KickWarnMaxCount >= this._kickWarnMaxCount)
				{
					int banMin = this.BanRoleByCount(client, true);
					client.CheckCheatData.RobotDetectedReason = banType.ToString();
					client.CheckCheatData.RobotDetectedKickTime = TimeUtil.NOW() + 1000L;
					return false;
				}
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(32, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
			}
			return true;
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x0007E38C File Offset: 0x0007C58C
		private int BanRoleByCount(GameClient client, bool addBanCount)
		{
			int banMinute = this.m_BanMinutes;
			using (this.m_Mutex.Enter(1))
			{
				if (addBanCount && !client.CheckCheatData.KickState)
				{
					client.CheckCheatData.KickState = true;
					int banCount = Global.GetRoleParamsInt32FromDB(client, "BanRobotCount");
					Global.SaveRoleParamsInt32ValueToDB(client, "BanRobotCount", banCount + 1, false);
					banCount = Global.Clamp(banCount, 0, this.m_BanMiuteList.Length - 1);
					banMinute = this.m_BanMiuteList[banCount];
				}
			}
			if (banMinute > 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(33, new object[0]), new object[]
				{
					banMinute
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
				BanManager.BanRoleName(Global.FormatRoleName(client, client.ClientData.RoleName), banMinute, 2);
			}
			else if (banMinute < 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(34, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
				GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 2, 1), null, client.ServerId);
			}
			return banMinute;
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x0007E528 File Offset: 0x0007C728
		private bool BanLog(GameClient client, BanType banType, string clientData, int jailbreak = 0, int autoStart = 0, string taskID = "", int banCount = -1, bool isForce = false)
		{
			bool result;
			if (!this.BanIsOpen())
			{
				result = false;
			}
			else
			{
				client.CheckCheatData.BanCheckMaxCount++;
				using (this.m_Mutex.Enter(2))
				{
					if (this._logCountDic.ContainsKey((int)banType) && !isForce)
					{
						if (!client.CheckCheatData.LogCountDic.ContainsKey((int)banType))
						{
							client.CheckCheatData.LogCountDic.Add((int)banType, 0);
						}
						if (client.CheckCheatData.LogCountDic[(int)banType] >= this._logCountDic[(int)banType])
						{
							return false;
						}
						Dictionary<int, int> logCountDic;
						(logCountDic = client.CheckCheatData.LogCountDic)[(int)banType] = logCountDic[(int)banType] + 1;
					}
				}
				string userID = client.strUserID;
				int level = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
				int vip = client.ClientData.VipLevel;
				string ip = this.GetIp(client);
				LogManager.WriteLog(LogTypes.Robot, string.Format("Reason={3} BanCount={8} userID={9} level={10} vip={11} RoleID={0} RoleName={1} Exp={2} DeviceID={12} JailBreak={4} AutoStart={5} TaskID={7} Server={13} Data={6}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.RoleName,
					ip,
					banType.ToString(),
					jailbreak,
					autoStart,
					clientData,
					taskID,
					banCount,
					userID,
					level,
					vip,
					client.deviceID,
					GameManager.ServerId
				}), null, true);
				result = true;
			}
			return result;
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x0007E720 File Offset: 0x0007C920
		private bool BanKick(GameClient client, BanType banType, string clientData = "", int jailbreak = 0, int autoStart = 0, string taskID = "", int banCount = -1, bool isForce = false)
		{
			bool result;
			if (!this.BanIsOpen() || this.m_BanMinutes <= 0)
			{
				result = true;
			}
			else
			{
				bool isBig = false;
				int level = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
				int vip = client.ClientData.VipLevel;
				if (!isForce && (level >= this._banLevelLimit || vip >= this._banVipLimit))
				{
					isBig = true;
				}
				if (banCount < 0)
				{
					banCount = Global.GetRoleParamsInt32FromDB(client, "BanRobotCount");
				}
				if (isBig)
				{
					client.CheckCheatData.BanCheckMaxCount++;
					taskID = banType.ToString() + "|" + taskID;
					this.BanLog(client, BanType.BanLogBig, clientData, jailbreak, autoStart, taskID, banCount, false);
					result = false;
				}
				else
				{
					this.BanLog(client, banType, clientData, jailbreak, autoStart, taskID, banCount, true);
					int banMinute = this.m_BanMinutes;
					using (this.m_Mutex.Enter(3))
					{
						if (!client.CheckCheatData.KickState)
						{
							client.CheckCheatData.KickState = true;
							Global.SaveRoleParamsInt32ValueToDB(client, "BanRobotCount", banCount + 1, false);
							int banIndex = Global.Clamp(banCount, 0, this.m_BanMiuteList.Length - 1);
							banMinute = this.m_BanMiuteList[banIndex];
							this.BanDBLog(client, banType, taskID, banCount + 1);
						}
					}
					if (banMinute > 0)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(33, new object[0]), new object[]
						{
							banMinute
						}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
						BanManager.BanRoleName(Global.FormatRoleName(client, client.ClientData.RoleName), banMinute, 2);
					}
					else if (banMinute < 0)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(34, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
						GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 2, 1), null, client.ServerId);
					}
					client.CheckCheatData.RobotDetectedReason = banType.ToString();
					client.CheckCheatData.RobotDetectedKickTime = TimeUtil.NOW() + 1000L;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x0007E9E0 File Offset: 0x0007CBE0
		private bool BanClose(GameClient client, BanType banType, string clientData = "", int jailbreak = 0, int autoStart = 0, string taskID = "", int banCount = -1, bool isForce = false)
		{
			bool result;
			if (!this.BanIsOpen())
			{
				result = true;
			}
			else
			{
				bool isBig = false;
				int level = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
				int vip = client.ClientData.VipLevel;
				if (!isForce && (level >= this._banLevelLimit || vip >= this._banVipLimit))
				{
					isBig = true;
				}
				if (banCount < 0)
				{
					banCount = Global.GetRoleParamsInt32FromDB(client, "BanRobotCount");
				}
				if (isBig)
				{
					client.CheckCheatData.BanCheckMaxCount++;
					taskID = banType.ToString() + "|" + taskID;
					this.BanLog(client, BanType.BanLogBig, clientData, jailbreak, autoStart, taskID, banCount, false);
					result = false;
				}
				else
				{
					this.BanLog(client, banType, clientData, jailbreak, autoStart, taskID, banCount, true);
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(34, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
					GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 2, 1), null, client.ServerId);
					Global.SaveRoleParamsInt32ValueToDB(client, "BanRobotCount", banCount + 1, false);
					this.BanDBLog(client, banType, taskID, banCount + 1);
					client.CheckCheatData.RobotDetectedReason = banType.ToString();
					client.CheckCheatData.RobotDetectedKickTime = TimeUtil.NOW() + 1000L;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x0007EB90 File Offset: 0x0007CD90
		private bool BanRate(GameClient client, BanType banType, string clientData = "", int jailbreak = 0, int autoStart = 0, string taskID = "", int banCount = -1, bool isForce = false)
		{
			bool result;
			if (client.CheckCheatData.BanCheckMaxCount >= this._banCheckMaxCount)
			{
				result = true;
			}
			else
			{
				client.CheckCheatData.BanCheckMaxCount++;
				if (!this.BanIsOpen() || client.CheckCheatData.DropRateDown)
				{
					result = true;
				}
				else
				{
					bool isBig = false;
					int level = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
					int vip = client.ClientData.VipLevel;
					if (!isForce && (level >= this._banLevelLimit || vip >= this._banVipLimit))
					{
						isBig = true;
					}
					if (banCount < 0)
					{
						banCount = Global.GetRoleParamsInt32FromDB(client, "BanRobotCount");
					}
					if (isBig)
					{
						taskID = banType.ToString() + "|" + taskID;
						this.BanLog(client, BanType.BanLogBig, clientData, jailbreak, autoStart, taskID, banCount, false);
						result = false;
					}
					else
					{
						this.BanLog(client, banType, clientData, jailbreak, autoStart, taskID, banCount, true);
						client.CheckCheatData.DropRateDown = true;
						this.BanDBLog(client, banType, taskID, banCount + 1);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x0007ECD4 File Offset: 0x0007CED4
		private void BanDBLog(GameClient client, BanType banType, string banID, int banCount)
		{
			string dbcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				client.ClientData.ZoneID,
				client.strUserID,
				client.ClientData.RoleID,
				(int)banType,
				banID,
				banCount,
				client.deviceID
			});
			Global.ExecuteDBCmd(13112, dbcmd, client.ServerId);
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x0007ED58 File Offset: 0x0007CF58
		public void RobotDataReset(GameClient client)
		{
			if (client != null)
			{
				client.CheckCheatData.RobotTaskListData = "";
				client.CheckCheatData.BanCheckMaxCount = 0;
				client.CheckCheatData.KickWarnMaxCount = 0;
				client.CheckCheatData.DropRateDown = false;
				client.CheckCheatData.KickState = false;
				client.CheckCheatData.RobotDetectedKickTime = 0L;
				client.CheckCheatData.RobotDetectedReason = "";
				client.CheckCheatData.NextTaskListTimeout = 0L;
				client.CheckCheatData.LogCountDic = new Dictionary<int, int>();
			}
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x0007EDF0 File Offset: 0x0007CFF0
		public double GetRobotSceneDropRate(GameClient client, int mapID, double dropRate, int monsterType)
		{
			double result;
			if (null == client)
			{
				result = dropRate;
			}
			else if (!this.BanIsOpen())
			{
				result = dropRate;
			}
			else
			{
				if (this.m_DropRateDownMonsterTypeHashSet.ContainsKey(monsterType))
				{
					double rate;
					if (client.CheckCheatData.DropRateDown && this._DropRateDownMapDict.TryGetValue(mapID, out rate))
					{
						dropRate *= rate;
					}
				}
				result = dropRate;
			}
			return result;
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0007EE64 File Offset: 0x0007D064
		public string GetIp(GameClient client)
		{
			string ip = "0";
			switch (this._canLogIp)
			{
			case 1:
				ip = Global.GetIPAddress(client.ClientSocket);
				break;
			case 2:
			{
				string ipStr = Global.GetIPAddress(client.ClientSocket);
				ip = IpHelper.IpToInt(ipStr).ToString();
				break;
			}
			}
			return ip;
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0007EEC4 File Offset: 0x0007D0C4
		private string GetTaskName(string task)
		{
			string taskStr = task;
			int index = taskStr.LastIndexOf('-');
			if (index > 0)
			{
				taskStr = taskStr.Substring(0, index);
			}
			return taskStr;
		}

		// Token: 0x04000DF3 RID: 3571
		private const int BAN_REASON = 2;

		// Token: 0x04000DF4 RID: 3572
		private const long KICK_DELAY_TIME = 1000L;

		// Token: 0x04000DF5 RID: 3573
		private static RobotTaskValidator instance = new RobotTaskValidator();

		// Token: 0x04000DF6 RID: 3574
		private MyTagLock m_Mutex = new MyTagLock(true);

		// Token: 0x04000DF7 RID: 3575
		private int m_TaskListVerifySeed;

		// Token: 0x04000DF8 RID: 3576
		private int m_TaskListVerifyRandomCount = 50;

		// Token: 0x04000DF9 RID: 3577
		private RSACryptoServiceProvider m_TaskListRSA;

		// Token: 0x04000DFA RID: 3578
		private string m_TaskListRSAPubKey;

		// Token: 0x04000DFB RID: 3579
		private RobotTaskValidator.TaskCheckList m_TaskCheckList = new RobotTaskValidator.TaskCheckList();

		// Token: 0x04000DFC RID: 3580
		private PlatformTypes _platformType = PlatformTypes.Tmsk;

		// Token: 0x04000DFD RID: 3581
		private bool _useWorkThread = true;

		// Token: 0x04000DFE RID: 3582
		private int _banCheckMaxCount = 5;

		// Token: 0x04000DFF RID: 3583
		private int _kickWarnMaxCount = 2;

		// Token: 0x04000E00 RID: 3584
		private int _banLevelLimit = 401;

		// Token: 0x04000E01 RID: 3585
		private int _banVipLimit = 6;

		// Token: 0x04000E02 RID: 3586
		private int _canLogIp = 0;

		// Token: 0x04000E03 RID: 3587
		private int m_BanMinutes = 0;

		// Token: 0x04000E04 RID: 3588
		private int[] m_BanMiuteList;

		// Token: 0x04000E05 RID: 3589
		private HashSet<string> m_AllTaskHashSet = new HashSet<string>();

		// Token: 0x04000E06 RID: 3590
		private Dictionary<int, int> _logCountDic = new Dictionary<int, int>();

		// Token: 0x04000E07 RID: 3591
		private bool _jiaoBenIsOpen = false;

		// Token: 0x04000E08 RID: 3592
		private bool _vmIsOpen = false;

		// Token: 0x04000E09 RID: 3593
		private int _timeOutCount = 5;

		// Token: 0x04000E0A RID: 3594
		private long _timeOutMinute = 5L;

		// Token: 0x04000E0B RID: 3595
		private Dictionary<string, int> _cancelAppDic = new Dictionary<string, int>();

		// Token: 0x04000E0C RID: 3596
		private string[] _specialAppList = null;

		// Token: 0x04000E0D RID: 3597
		private Dictionary<PlatformTypes, int> _yyOpenPlatform = new Dictionary<PlatformTypes, int>();

		// Token: 0x04000E0E RID: 3598
		private Dictionary<string, int> _yueyuAppDic = new Dictionary<string, int>();

		// Token: 0x04000E0F RID: 3599
		private int _appCount = 10;

		// Token: 0x04000E10 RID: 3600
		private int _multiCount = 3;

		// Token: 0x04000E11 RID: 3601
		private int _decryptCount = 5;

		// Token: 0x04000E12 RID: 3602
		private bool _banRateNum = false;

		// Token: 0x04000E13 RID: 3603
		private int[] _appPlatformCount = new int[]
		{
			0,
			1,
			5
		};

		// Token: 0x04000E14 RID: 3604
		private Dictionary<BanReasonType, int> _banReasonDic = new Dictionary<BanReasonType, int>();

		// Token: 0x04000E15 RID: 3605
		private Dictionary<string, BanInfo> m_RobotTaskList = new Dictionary<string, BanInfo>();

		// Token: 0x04000E16 RID: 3606
		private Dictionary<string, int> m_RobotBuildList = new Dictionary<string, int>();

		// Token: 0x04000E17 RID: 3607
		private Dictionary<string, int> m_RobotCPUList = new Dictionary<string, int>();

		// Token: 0x04000E18 RID: 3608
		private Dictionary<string, int> m_RobotPhoneList = new Dictionary<string, int>();

		// Token: 0x04000E19 RID: 3609
		private Dictionary<string, int> m_RobotSignList = new Dictionary<string, int>();

		// Token: 0x04000E1A RID: 3610
		private Dictionary<string, int> m_YueYuDevList = new Dictionary<string, int>();

		// Token: 0x04000E1B RID: 3611
		private Dictionary<string, int> m_YueYuOSVerList = new Dictionary<string, int>();

		// Token: 0x04000E1C RID: 3612
		private Dictionary<string, List<string>> m_NewYueYuDevOSVerListDict = new Dictionary<string, List<string>>();

		// Token: 0x04000E1D RID: 3613
		private int[] _vmSignGameOpenSeven = new int[]
		{
			0,
			7,
			120
		};

		// Token: 0x04000E1E RID: 3614
		private int[] _yueYuGameOpenSeveb = new int[]
		{
			0,
			7,
			120
		};

		// Token: 0x04000E1F RID: 3615
		private HashSet<int> VMBanMapCodes = new HashSet<int>();

		// Token: 0x04000E20 RID: 3616
		private Thread BackgroundThread;

		// Token: 0x04000E21 RID: 3617
		public ConcurrentDictionary<int, double> _DropRateDownMapDict = new ConcurrentDictionary<int, double>();

		// Token: 0x04000E22 RID: 3618
		public ConcurrentDictionary<int, int> m_DropRateDownMonsterTypeHashSet = new ConcurrentDictionary<int, int>();

		// Token: 0x04000E23 RID: 3619
		private Dictionary<PlatformTypes, int> _banOpenPlatform = new Dictionary<PlatformTypes, int>();

		// Token: 0x04000E24 RID: 3620
		private bool _BanIsOpen = false;

		// Token: 0x04000E25 RID: 3621
		private Queue<RobotDataItem> RobotDataItemQueue = new Queue<RobotDataItem>();

		// Token: 0x0200024D RID: 589
		[ProtoContract]
		public class TaskCheckList : ICompressed
		{
			// Token: 0x04000E26 RID: 3622
			[ProtoMember(1)]
			public byte[] TaskCheckListData;
		}
	}
}
