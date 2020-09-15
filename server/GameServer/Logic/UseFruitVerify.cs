using System;

namespace GameServer.Logic
{
	// Token: 0x020007A4 RID: 1956
	internal class UseFruitVerify
	{
		// Token: 0x0600330B RID: 13067 RVA: 0x002D44A8 File Offset: 0x002D26A8
		public static int GetFruitAddPropLimit(GameClient client, string strPropName)
		{
			ChangeLifeAddPointInfo tmpChangeAddPointInfo = Data.ChangeLifeAddPointInfoList[client.ClientData.ChangeLifeCount];
			int result;
			if ("Strength" == strPropName)
			{
				result = tmpChangeAddPointInfo.nStrLimit;
			}
			else if ("Dexterity" == strPropName)
			{
				result = tmpChangeAddPointInfo.nDexLimit;
			}
			else if ("Intelligence" == strPropName)
			{
				result = tmpChangeAddPointInfo.nIntLimit;
			}
			else if ("Constitution" == strPropName)
			{
				result = tmpChangeAddPointInfo.nConLimit;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x0600330C RID: 13068 RVA: 0x002D4544 File Offset: 0x002D2744
		public static int AddValueVerify(GameClient client, int nOld, int nPropLimit, int nAddValue)
		{
			if (nOld < nPropLimit)
			{
				if (nOld + nAddValue > nPropLimit)
				{
					nAddValue = nPropLimit - nOld;
				}
			}
			else
			{
				nAddValue = 0;
			}
			return nAddValue;
		}
	}
}
