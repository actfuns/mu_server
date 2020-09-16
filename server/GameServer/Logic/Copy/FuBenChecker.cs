using System;
using GameServer.Logic.WanMota;

namespace GameServer.Logic.Copy
{
	
	public static class FuBenChecker
	{
		
		public static bool HasFinishedPreTask(GameClient client, SystemXmlItem fubenItem)
		{
			bool result;
			if (client == null || fubenItem == null)
			{
				result = false;
			}
			else
			{
				int copyTab = fubenItem.GetIntValue("TabID", -1);
				int needTask = GlobalNew.GetFuBenTabNeedTask(copyTab);
				result = (needTask <= client.ClientData.MainTaskID);
			}
			return result;
		}

		
		public static bool HasPassedPreCopy(GameClient client, SystemXmlItem fubenItem)
		{
			bool result;
			if (client == null || fubenItem == null)
			{
				result = false;
			}
			else
			{
				int nUpCopyID = fubenItem.GetIntValue("UpCopyID", -1);
				int nFinishNumber = fubenItem.GetIntValue("FinishNumber", -1);
				if (nUpCopyID > 0 && nFinishNumber > 0)
				{
					if (!Global.FuBenPassed(client, nUpCopyID))
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		
		public static bool IsInCopyLevelLimit(GameClient client, SystemXmlItem fubenItem)
		{
			bool result;
			if (client == null || fubenItem == null)
			{
				result = false;
			}
			else
			{
				int minLevel = fubenItem.GetIntValue("MinLevel", -1);
				int maxLevel = fubenItem.GetIntValue("MaxLevel", -1);
				maxLevel = ((maxLevel <= 0) ? 1000 : maxLevel);
				int nMinZhuanSheng = fubenItem.GetIntValue("MinZhuanSheng", -1);
				int nMaxZhuanSheng = fubenItem.GetIntValue("MaxZhuanSheng", -1);
				nMaxZhuanSheng = ((nMaxZhuanSheng <= 0) ? 1000 : nMaxZhuanSheng);
				minLevel = Global.GetUnionLevel(nMinZhuanSheng, minLevel, false);
				maxLevel = Global.GetUnionLevel(nMaxZhuanSheng, maxLevel, true);
				int unionLevel = Global.GetUnionLevel(client.ClientData.ChangeLifeCount, client.ClientData.Level, false);
				result = (unionLevel >= minLevel && unionLevel <= maxLevel);
			}
			return result;
		}

		
		public static bool IsInCopyTimesLimit(GameClient client, SystemXmlItem fubenItem)
		{
			bool result;
			if (client == null || fubenItem == null)
			{
				result = false;
			}
			else
			{
				int copyId = fubenItem.GetIntValue("ID", -1);
				if (WanMotaCopySceneManager.IsWanMoTaMapCode(copyId))
				{
					result = true;
				}
				else
				{
					int maxEnterNum = fubenItem.GetIntValue("EnterNumber", -1);
					int maxFinishNum = fubenItem.GetIntValue("FinishNumber", -1);
					int hadFinishNum;
					int hadEnterNum = Global.GetFuBenEnterNum(Global.GetFuBenData(client, copyId), out hadFinishNum);
					if (maxEnterNum <= 0 && maxFinishNum <= 0)
					{
						result = true;
					}
					else
					{
						int[] nAddNum;
						if (Global.IsInExperienceCopyScene(copyId))
						{
							nAddNum = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJinYanFuBenNum", ',');
						}
						else if (copyId == 5100)
						{
							nAddNum = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJinBiFuBenNum", ',');
						}
						else
						{
							nAddNum = null;
						}
						int extAddNum = 0;
						int nVipLev = client.ClientData.VipLevel;
						if (nVipLev > 0 && nVipLev <= VIPEumValue.VIPENUMVALUE_MAXLEVEL && nAddNum != null && nAddNum.Length > nVipLev)
						{
							extAddNum = nAddNum[nVipLev];
						}
						result = ((maxEnterNum <= 0 || hadEnterNum < maxEnterNum + extAddNum) && (maxFinishNum <= 0 || hadFinishNum < maxFinishNum + extAddNum));
					}
				}
			}
			return result;
		}
	}
}
