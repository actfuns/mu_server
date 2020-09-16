using System;

namespace GameServer.Logic
{
	
	internal class UseFruitVerify
	{
		
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
