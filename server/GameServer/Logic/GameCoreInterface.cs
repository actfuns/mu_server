using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class GameCoreInterface : ICoreInterface
	{
		
		public static GameCoreInterface getinstance()
		{
			return GameCoreInterface.CoreInterface;
		}

		
		public int GetNewFuBenSeqId()
		{
			int nSeqID = -1;
			try
			{
				string[] dbFields = Global.ExecuteDBCmd(10049, string.Format("{0}", 0), 0);
				if (dbFields != null && dbFields.Length >= 2)
				{
					nSeqID = Global.SafeConvertToInt32(dbFields[1]);
				}
			}
			catch (Exception ex)
			{
				nSeqID = -1;
			}
			return nSeqID;
		}

		
		public int GetLocalServerId()
		{
			return GameManager.ServerId;
		}

		
		public ISceneEventSource GetEventSourceInterface()
		{
			return GlobalEventSource4Scene.getInstance();
		}

		
		public string GetGameConfigStr(string name, string defVal)
		{
			return GameManager.GameConfigMgr.GetGameConfigItemStr(name, defVal);
		}

		
		public PlatformTypes GetPlatformType()
		{
			return GameManager.PlatformType;
		}

		
		public void SetRuntimeVariable(string name, string val)
		{
			if (null != name)
			{
				lock (this.RuntimeVariableDict)
				{
					this.RuntimeVariableDict[name] = val;
				}
			}
		}

		
		public string GetRuntimeVariable(string name, string defVal)
		{
			string result;
			if (null == name)
			{
				result = defVal;
			}
			else
			{
				lock (this.RuntimeVariableDict)
				{
					string val;
					if (this.RuntimeVariableDict.TryGetValue(name, out val))
					{
						return val;
					}
				}
				result = defVal;
			}
			return result;
		}

		
		public int GetRuntimeVariable(string name, int defVal)
		{
			int result;
			if (null == name)
			{
				result = defVal;
			}
			else
			{
				lock (this.RuntimeVariableDict)
				{
					string val;
					if (this.RuntimeVariableDict.TryGetValue(name, out val))
					{
						int ret;
						if (int.TryParse(val, out ret))
						{
							return ret;
						}
					}
				}
				result = defVal;
			}
			return result;
		}

		
		public string GetLocalAddressIPs()
		{
			return Global.GetLocalAddressIPs();
		}

		
		public int GetMapClientCount(int mapCode)
		{
			return GameManager.ClientMgr.GetMapClientsCount(mapCode);
		}

		
		private static GameCoreInterface CoreInterface = new GameCoreInterface();

		
		private Dictionary<string, string> RuntimeVariableDict = new Dictionary<string, string>();
	}
}
