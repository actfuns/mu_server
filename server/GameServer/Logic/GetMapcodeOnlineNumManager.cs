using System;

namespace GameServer.Logic
{
	
	public class GetMapcodeOnlineNumManager
	{
		
		public static void LoadCountMapID()
		{
			GetMapcodeOnlineNumManager.arrCountMapcode = GameManager.systemParamsList.GetParamValueIntArrayByName("CountOnlineMapID", ',');
		}

		
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

		
		private static int[] arrCountMapcode = null;
	}
}
