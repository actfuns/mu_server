using System;

namespace GameServer.Logic
{
	// Token: 0x020004DB RID: 1243
	public class GetMapcodeOnlineNumManager
	{
		// Token: 0x0600171A RID: 5914 RVA: 0x0016A4B2 File Offset: 0x001686B2
		public static void LoadCountMapID()
		{
			GetMapcodeOnlineNumManager.arrCountMapcode = GameManager.systemParamsList.GetParamValueIntArrayByName("CountOnlineMapID", ',');
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x0016A4CC File Offset: 0x001686CC
		public static int IsCountMapID(int nMapID)
		{
			for (int i = 0; i < GetMapcodeOnlineNumManager.arrCountMapcode.Length; i++)
			{
				if (GetMapcodeOnlineNumManager.arrCountMapcode[i] == nMapID)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x0016A50C File Offset: 0x0016870C
		public static string CountMapIDOnlineNum()
		{
			if (null == GetMapcodeOnlineNumManager.arrCountMapcode)
			{
				GetMapcodeOnlineNumManager.LoadCountMapID();
			}
			string result;
			if (null == GetMapcodeOnlineNumManager.arrCountMapcode)
			{
				result = "";
			}
			else
			{
				string strOnlineInfo = "";
				for (int i = 0; i < GetMapcodeOnlineNumManager.arrCountMapcode.Length; i++)
				{
					if (0 != i)
					{
						strOnlineInfo += "|";
					}
					strOnlineInfo += string.Format("{0},{1}", GetMapcodeOnlineNumManager.arrCountMapcode[i], GameManager.ClientMgr.GetMapClientsCount(GetMapcodeOnlineNumManager.arrCountMapcode[i]));
				}
				result = strOnlineInfo;
			}
			return result;
		}

		// Token: 0x04002103 RID: 8451
		private static int[] arrCountMapcode = null;
	}
}
