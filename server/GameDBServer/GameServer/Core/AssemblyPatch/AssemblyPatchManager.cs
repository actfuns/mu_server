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
	
	public class AssemblyPatchManager : IManager, IEventListener
	{
		
		public static AssemblyPatchManager getInstance()
		{
			return AssemblyPatchManager.instance;
		}

		
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

		
		public bool IfNeedMonMsg()
		{
			return this.patchCfgDict.ContainsKey(EventTypes.BeforeProcessMsg);
		}

		
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

		
		private static AssemblyPatchManager instance = new AssemblyPatchManager();

		
		private Dictionary<EventTypes, List<MethodConfig>> patchCfgDict = new Dictionary<EventTypes, List<MethodConfig>>();

		
		private bool[] CmdRegisteredFlags = new bool[32768];

		
		private Dictionary<string, AssemblyLoader> patchDict = new Dictionary<string, AssemblyLoader>();
	}
}
