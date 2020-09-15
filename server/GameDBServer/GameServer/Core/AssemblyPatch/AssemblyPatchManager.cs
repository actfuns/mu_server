using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameDBServer.Core.GameEvent;
using GameDBServer.Logic;
using GameDBServer.Server;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Core.AssemblyPatch
{
	// Token: 0x02000008 RID: 8
	public class AssemblyPatchManager : IManager, IEventListener
	{
		// Token: 0x06000013 RID: 19 RVA: 0x0000241C File Offset: 0x0000061C
		public static AssemblyPatchManager getInstance()
		{
			return AssemblyPatchManager.instance;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002434 File Offset: 0x00000634
		public bool initialize()
		{
			bool result;
			if (!this.InitConfig())
			{
				result = true;
			}
			else
			{
				GlobalEventSource.getInstance().removeListener(0, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(1, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(2, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(3, AssemblyPatchManager.getInstance());
				GlobalEventSource.getInstance().removeListener(4, AssemblyPatchManager.getInstance());
				List<EventTypes> list = this.patchCfgDict.Keys.ToList<EventTypes>();
				foreach (EventTypes item in list)
				{
					GlobalEventSource.getInstance().registerListener((int)item, AssemblyPatchManager.getInstance());
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002514 File Offset: 0x00000714
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002528 File Offset: 0x00000728
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000253C File Offset: 0x0000073C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002550 File Offset: 0x00000750
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
						MethodConfig config = new MethodConfig();
						Enum.TryParse<EventTypes>(ConfigHelper.GetElementAttributeValue(xmlItem, "Type", ""), true, out config.eventType);
						config.assemblyName = ConfigHelper.GetElementAttributeValue(xmlItem, "AssemblyName", "");
						config.fullClassName = ConfigHelper.GetElementAttributeValue(xmlItem, "fullClassName", "");
						config.methodName = ConfigHelper.GetElementAttributeValue(xmlItem, "methodName", "");
						config.methodParams = ConfigHelper.GetElementAttributeValue(xmlItem, "methodParams", "").Split(new char[]
						{
							','
						});
						config.cmdID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "CmdID", 0L);
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
					LogManager.WriteLog(LogTypes.Exception, "加载AssemblyPatch时文件出错\r\n" + ex.ToString(), null, true);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002768 File Offset: 0x00000968
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
				LogManager.WriteLog(LogTypes.Error, string.Format("PatchMgr::CheckMethod Error, AssemblyName={0}, fullClassName={1}, methodName={2}\r\n", cfg.assemblyName, cfg.fullClassName, cfg.methodName) + ex.ToString(), null, true);
				result = false;
			}
			return result;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002818 File Offset: 0x00000A18
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

		// Token: 0x0600001B RID: 27 RVA: 0x0000284C File Offset: 0x00000A4C
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

		// Token: 0x0600001C RID: 28 RVA: 0x00002898 File Offset: 0x00000A98
		public void processEvent(EventObject eventObject)
		{
			List<MethodConfig> methodList = null;
			if (this.patchCfgDict.TryGetValue((EventTypes)eventObject.getEventType(), out methodList))
			{
				if (methodList != null && methodList.Count > 0)
				{
					foreach (MethodConfig item in methodList)
					{
						AssemblyLoader loader = this.GetAssemblyLoader(item.assemblyName);
						if (null != loader)
						{
							GameServerClient client = null;
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

		// Token: 0x0600001D RID: 29 RVA: 0x00002A00 File Offset: 0x00000C00
		public bool IfNeedMonMsg()
		{
			return this.patchCfgDict.ContainsKey(EventTypes.BeforeProcessMsg);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002A20 File Offset: 0x00000C20
		public TCPProcessCmdResults ProcessMsg(GameServerClient client, int nID, byte[] data, int count)
		{
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
							paramarray[0] = client;
							paramarray[1] = nID;
							paramarray[2] = data;
							paramarray[3] = count;
							for (int i = 0; i < item.methodParams.Length; i++)
							{
								paramarray[i + index] = item.methodParams[i];
							}
							TcpResult result = (TcpResult)loader.Invoke(item.fullClassName, item.methodName, paramarray);
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

		// Token: 0x0400000E RID: 14
		private static AssemblyPatchManager instance = new AssemblyPatchManager();

		// Token: 0x0400000F RID: 15
		private Dictionary<EventTypes, List<MethodConfig>> patchCfgDict = new Dictionary<EventTypes, List<MethodConfig>>();

		// Token: 0x04000010 RID: 16
		private bool[] CmdRegisteredFlags = new bool[32768];

		// Token: 0x04000011 RID: 17
		private Dictionary<string, AssemblyLoader> patchDict = new Dictionary<string, AssemblyLoader>();
	}
}
