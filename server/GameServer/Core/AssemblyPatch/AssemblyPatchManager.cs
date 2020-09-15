using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic;
using GameServer.Server;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Core.AssemblyPatch
{
	// Token: 0x020000C4 RID: 196
	public class AssemblyPatchManager : IManager, IEventListener
	{
		// Token: 0x06000362 RID: 866 RVA: 0x0003B948 File Offset: 0x00039B48
		public static AssemblyPatchManager getInstance()
		{
			return AssemblyPatchManager.instance;
		}

		// Token: 0x06000363 RID: 867 RVA: 0x0003B960 File Offset: 0x00039B60
		public bool initialize()
		{
			bool result;
			if (!this.InitConfig())
			{
				result = true;
			}
			else
			{
				GlobalEventSource.getInstance().removeListener(14, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(41, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(12, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(38, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(39, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(40, AssemblyPatchManager.getInstance());
				List<EventTypes> list = this.patchCfgDict.Keys.ToList<EventTypes>();
				foreach (EventTypes item in list)
				{
					GlobalEventSource.getInstance().registerListener((int)item, AssemblyPatchManager.getInstance());
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0003BA58 File Offset: 0x00039C58
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x0003BA6C File Offset: 0x00039C6C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000366 RID: 870 RVA: 0x0003BA80 File Offset: 0x00039C80
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000367 RID: 871 RVA: 0x0003BA94 File Offset: 0x00039C94
		private bool CheckVersion(string cur, string max)
		{
			bool result;
			if (string.IsNullOrEmpty(cur))
			{
				result = false;
			}
			else if (string.IsNullOrEmpty(max))
			{
				result = true;
			}
			else
			{
				string[] x = cur.Split(new char[]
				{
					'.'
				});
				string[] y = max.Split(new char[]
				{
					'.'
				});
				result = (x.Length == y.Length && x.Length == 4 && (!(x[0] != y[0]) && !(x[1] != y[1])) && !(x[2] != y[2]) && x[3].CompareTo(y[3]) <= 0);
			}
			return result;
		}

		// Token: 0x06000368 RID: 872 RVA: 0x0003BB5C File Offset: 0x00039D5C
		public bool InitConfig()
		{
			string fileName = "AssemblyPatch.xml";
			XElement xml = ConfigHelper.Load(fileName);
			bool result;
			if (null == xml)
			{
				result = false;
			}
			else
			{
				try
				{
					Dictionary<EventTypes, List<MethodConfig>> tmpPatchCfgDict = new Dictionary<EventTypes, List<MethodConfig>>();
					bool[] flags = new bool[32768];
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						string maxVer = Global.GetDefAttributeStr(xmlItem, "VersionID", "99.9.9.999999");
						string curVer = Program.GetVersionStr();
						if (!this.CheckVersion(curVer, maxVer))
						{
							Program.DeleteFile("AssemblyPatch.xml");
							Program.DeleteFile("AssemblyPatch.dll");
							Program.DeleteFile("DotNetDetour.dll");
							Program.DeleteFile("dlls\\AssemblyPatch.dll");
							Program.DeleteFile("dlls\\DotNetDetour.dll");
							return true;
						}
						MethodConfig config = new MethodConfig();
						Enum.TryParse<EventTypes>(Global.GetDefAttributeStr(xmlItem, "Type", ""), true, out config.eventType);
						config.assemblyName = Global.GetDefAttributeStr(xmlItem, "AssemblyName", "");
						config.fullClassName = Global.GetDefAttributeStr(xmlItem, "fullClassName", "");
						config.methodName = Global.GetDefAttributeStr(xmlItem, "methodName", "");
						config.methodParams = Global.GetDefAttributeStr(xmlItem, "methodParams", "").Split(new char[]
						{
							','
						});
						config.cmdID = (int)Global.GetSafeAttributeLong(xmlItem, "CmdID");
						if (this.CheckMethod(config))
						{
							flags[config.cmdID] = true;
							if (tmpPatchCfgDict.ContainsKey(config.eventType))
							{
								tmpPatchCfgDict[config.eventType].Add(config);
							}
							else
							{
								List<MethodConfig> methodList = new List<MethodConfig>();
								methodList.Add(config);
								tmpPatchCfgDict.Add(config.eventType, methodList);
							}
						}
					}
					this.patchCfgDict = tmpPatchCfgDict;
					this.CmdRegisteredFlags = flags;
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, "加载AssemblyPatch时文件出错", ex, true);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000369 RID: 873 RVA: 0x0003BDDC File Offset: 0x00039FDC
		private bool CheckMethod(MethodConfig cfg)
		{
			bool result;
			try
			{
				AssemblyLoader loader = this.GetAssemblyLoader(cfg.assemblyName);
				if (null == loader)
				{
					loader = new AssemblyLoader();
					if (!loader.LoadAssembly(cfg.assemblyName))
					{
						return false;
					}
					this.AddAssemblyLoader(cfg.assemblyName, loader);
				}
				result = loader.LoadMethod(cfg.fullClassName, cfg.methodName);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("PatchMgr::CheckMethod Error, AssemblyName={0}, fullClassName={1}, methodName={2}", cfg.assemblyName, cfg.fullClassName, cfg.methodName), ex, false);
				result = false;
			}
			return result;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x0003BE80 File Offset: 0x0003A080
		private AssemblyLoader GetAssemblyLoader(string AssemblyName)
		{
			AssemblyLoader result;
			if (!this.patchDict.ContainsKey(AssemblyName))
			{
				result = null;
			}
			else
			{
				result = this.patchDict[AssemblyName];
			}
			return result;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x0003BEB4 File Offset: 0x0003A0B4
		private void AddAssemblyLoader(string AssemblyName, AssemblyLoader loader)
		{
			try
			{
				this.patchDict.Add(AssemblyName, loader);
			}
			catch
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("PatchMgr::AddLoader Error, AssemblyName={0}", AssemblyName), null, true);
			}
		}

		// Token: 0x0600036C RID: 876 RVA: 0x0003BF00 File Offset: 0x0003A100
		public void processEvent(EventObject eventObject)
		{
			List<MethodConfig> methodList = null;
			if (this.patchCfgDict.TryGetValue((EventTypes)eventObject.getEventType(), out methodList))
			{
				if (methodList != null && methodList.Count > 0)
				{
					GameClient client = null;
					if (eventObject.getEventType() == 14)
					{
						PlayerInitGameEventObject eventObj = (PlayerInitGameEventObject)eventObject;
						client = eventObj.getPlayer();
					}
					else if (eventObject.getEventType() == 41)
					{
						PlayerLoginGameEventObject eventObj2 = (PlayerLoginGameEventObject)eventObject;
						client = eventObj2.getPlayer();
					}
					else if (eventObject.getEventType() == 12)
					{
						PlayerLogoutEventObject eventObj3 = (PlayerLogoutEventObject)eventObject;
						client = eventObj3.getPlayer();
					}
					else if (eventObject.getEventType() == 38)
					{
						PlayerOnlineEventObject eventObj4 = (PlayerOnlineEventObject)eventObject;
						client = eventObj4.getPlayer();
					}
					foreach (MethodConfig item in methodList)
					{
						AssemblyLoader loader = this.GetAssemblyLoader(item.assemblyName);
						if (null != loader)
						{
							object[] paramarray;
							if (null != client)
							{
								int index = 1;
								paramarray = new object[item.methodParams.Length + index];
								paramarray[0] = client;
								for (int i = 0; i < item.methodParams.Length; i++)
								{
									paramarray[i + index] = item.methodParams[i];
								}
							}
							else
							{
								paramarray = new object[item.methodParams.Length];
								for (int i = 0; i < item.methodParams.Length; i++)
								{
									paramarray[i] = item.methodParams[i];
								}
							}
							loader.Invoke(item.fullClassName, item.methodName, paramarray);
						}
					}
				}
			}
		}

		// Token: 0x0600036D RID: 877 RVA: 0x0003C108 File Offset: 0x0003A308
		public bool IfNeedMonMsg()
		{
			return this.patchCfgDict.ContainsKey(EventTypes.BeforeProcessMsg);
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0003C128 File Offset: 0x0003A328
		public TCPProcessCmdResults ProcessMsg(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				if (!this.CmdRegisteredFlags[nID])
				{
					return TCPProcessCmdResults.RESUTL_CONTINUE;
				}
				List<MethodConfig> methodList = null;
				if (!this.patchCfgDict.TryGetValue(EventTypes.BeforeProcessMsg, out methodList))
				{
					return TCPProcessCmdResults.RESUTL_CONTINUE;
				}
				if (methodList == null || methodList.Count <= 0)
				{
					return TCPProcessCmdResults.RESUTL_CONTINUE;
				}
				foreach (MethodConfig item in methodList)
				{
					if (item.cmdID == nID)
					{
						AssemblyLoader loader = this.GetAssemblyLoader(item.assemblyName);
						if (null != loader)
						{
							int index = 8;
							object[] paramarray = new object[item.methodParams.Length + index];
							paramarray[0] = tcpMgr;
							paramarray[1] = socket;
							paramarray[2] = tcpClientPool;
							paramarray[3] = tcpRandKey;
							paramarray[4] = pool;
							paramarray[5] = nID;
							paramarray[6] = data;
							paramarray[7] = count;
							for (int i = 0; i < item.methodParams.Length; i++)
							{
								paramarray[i + index] = item.methodParams[i];
							}
							TcpResult result = (TcpResult)loader.Invoke(item.fullClassName, item.methodName, paramarray);
							tcpOutPacket = result.outPacket;
							return result.cmdResult;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("AssemblyPatchMgr::ProcessMsg Error nID={0}", nID), null, true);
			}
			return TCPProcessCmdResults.RESUTL_CONTINUE;
		}

		// Token: 0x040004C9 RID: 1225
		private static AssemblyPatchManager instance = new AssemblyPatchManager();

		// Token: 0x040004CA RID: 1226
		private Dictionary<EventTypes, List<MethodConfig>> patchCfgDict = new Dictionary<EventTypes, List<MethodConfig>>();

		// Token: 0x040004CB RID: 1227
		private bool[] CmdRegisteredFlags = new bool[32768];

		// Token: 0x040004CC RID: 1228
		private Dictionary<string, AssemblyLoader> patchDict = new Dictionary<string, AssemblyLoader>();
	}
}
