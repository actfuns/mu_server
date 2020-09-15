using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020004CA RID: 1226
	public class GameCoreInterface : ICoreInterface
	{
		// Token: 0x060016A4 RID: 5796 RVA: 0x001616E8 File Offset: 0x0015F8E8
		public static GameCoreInterface getinstance()
		{
			return GameCoreInterface.CoreInterface;
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x00161700 File Offset: 0x0015F900
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

		// Token: 0x060016A6 RID: 5798 RVA: 0x0016176C File Offset: 0x0015F96C
		public int GetLocalServerId()
		{
			return GameManager.ServerId;
		}

		// Token: 0x060016A7 RID: 5799 RVA: 0x00161784 File Offset: 0x0015F984
		public ISceneEventSource GetEventSourceInterface()
		{
			return GlobalEventSource4Scene.getInstance();
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x0016179C File Offset: 0x0015F99C
		public string GetGameConfigStr(string name, string defVal)
		{
			return GameManager.GameConfigMgr.GetGameConfigItemStr(name, defVal);
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x001617BC File Offset: 0x0015F9BC
		public PlatformTypes GetPlatformType()
		{
			return GameManager.PlatformType;
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x001617D4 File Offset: 0x0015F9D4
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

		// Token: 0x060016AB RID: 5803 RVA: 0x00161838 File Offset: 0x0015FA38
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

		// Token: 0x060016AC RID: 5804 RVA: 0x001618B0 File Offset: 0x0015FAB0
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

		// Token: 0x060016AD RID: 5805 RVA: 0x00161940 File Offset: 0x0015FB40
		public string GetLocalAddressIPs()
		{
			return Global.GetLocalAddressIPs();
		}

		// Token: 0x060016AE RID: 5806 RVA: 0x00161958 File Offset: 0x0015FB58
		public int GetMapClientCount(int mapCode)
		{
			return GameManager.ClientMgr.GetMapClientsCount(mapCode);
		}

		// Token: 0x0400206F RID: 8303
		private static GameCoreInterface CoreInterface = new GameCoreInterface();

		// Token: 0x04002070 RID: 8304
		private Dictionary<string, string> RuntimeVariableDict = new Dictionary<string, string>();
	}
}
