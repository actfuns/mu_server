using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	
	public class BuChangManager
	{
		
		public static void ResetBuChangItemDict()
		{
			GameManager.SystemBuChang.ReloadLoadFromXMlFile();
			BuChangManager.InitBuChangDict();
		}

		
		private static void InitBuChangDict()
		{
			lock (BuChangManager._BuChangItemDict)
			{
				BuChangManager._BuChangItemDict.Clear();
			}
			foreach (SystemXmlItem systemBuChangItem in GameManager.SystemBuChang.SystemXmlItemDict.Values)
			{
				BuChangItem buChangItem = new BuChangItem
				{
					MinLevel = systemBuChangItem.GetIntValue("MinLevel", -1),
					MinZhuanSheng = systemBuChangItem.GetIntValue("MinZhuanSheng", -1),
					MaxLevel = systemBuChangItem.GetIntValue("MaxLevel", -1),
					MaxZhuanSheng = systemBuChangItem.GetIntValue("MaxZhuanSheng", -1),
					Experience = Math.Max(0L, (long)systemBuChangItem.GetDoubleValue("AwardExp")),
					MoJing = Math.Max(0, systemBuChangItem.GetIntValue("MoJing", -1)),
					GoodsDataList = BuChangManager.ParseGoodsDataList(systemBuChangItem.GetStringValue("Goods"))
				};
				int minUnionLevel = Global.GetUnionLevel(buChangItem.MinZhuanSheng, buChangItem.MinLevel, false);
				int maxUnionLevel = Global.GetUnionLevel(buChangItem.MaxZhuanSheng, buChangItem.MaxLevel, false);
				lock (BuChangManager._BuChangItemDict)
				{
					BuChangManager._BuChangItemDict[new RangeKey(minUnionLevel, maxUnionLevel, null)] = buChangItem;
				}
			}
		}

		
		public static BuChangItem GetBuChangItem(int unionLevel)
		{
			BuChangItem buChangItem = null;
			lock (BuChangManager._BuChangItemDict)
			{
				if (BuChangManager._BuChangItemDict.TryGetValue(unionLevel, out buChangItem))
				{
					return buChangItem;
				}
			}
			BuChangManager.InitBuChangDict();
			lock (BuChangManager._BuChangItemDict)
			{
				if (BuChangManager._BuChangItemDict.TryGetValue(unionLevel, out buChangItem))
				{
					return buChangItem;
				}
			}
			return buChangItem;
		}

		
		public static long GetBuChangExp(GameClient client)
		{
			BuChangItem buChangItem = BuChangManager.GetBuChangItem(Global.GetUnionLevel(client, false));
			long result;
			if (null == buChangItem)
			{
				result = 0L;
			}
			else
			{
				result = buChangItem.Experience;
			}
			return result;
		}

		
		public static int GetBuChangBindYuanBao(GameClient client)
		{
			BuChangItem buChangItem = BuChangManager.GetBuChangItem(Global.GetUnionLevel(client, false));
			int result;
			if (null == buChangItem)
			{
				result = 0;
			}
			else
			{
				result = buChangItem.MoJing;
			}
			return result;
		}

		
		public static List<GoodsData> GetBuChangGoodsDataList(GameClient client)
		{
			BuChangItem buChangItem = BuChangManager.GetBuChangItem(Global.GetUnionLevel(client, false));
			List<GoodsData> result;
			if (null == buChangItem)
			{
				result = null;
			}
			else
			{
				result = buChangItem.GoodsDataList;
			}
			return result;
		}

		
		private static List<GoodsData> ParseGoodsDataList(string goodsIDs)
		{
			List<GoodsData> goodsDataList = new List<GoodsData>();
			List<GoodsData> result;
			if (string.IsNullOrEmpty(goodsIDs))
			{
				result = goodsDataList;
			}
			else
			{
				string[] fields = goodsIDs.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < fields.Length; i++)
				{
					string[] sa = fields[i].Split(new char[]
					{
						','
					});
					if (sa.Length == 7)
					{
						int[] goodsFields = Global.StringArray2IntArray(sa);
						GoodsData goodsData = Global.GetNewGoodsData(goodsFields[0], goodsFields[1], 0, goodsFields[3], goodsFields[2], 0, goodsFields[5], 0, goodsFields[6], goodsFields[4], 0);
						goodsDataList.Add(goodsData);
					}
				}
				result = goodsDataList;
			}
			return result;
		}

		
		public static bool CanGiveBuChang()
		{
			try
			{
				string AwardStartDate = Global.GetTimeByBuChang(0, 0, 0, 0);
				string AwardEndDate = Global.GetTimeByBuChang(1, 23, 59, 59);
				DateTime startAward = DateTime.Parse(AwardStartDate);
				DateTime endAward = DateTime.Parse(AwardEndDate);
				if (TimeUtil.NowDateTime() >= startAward && TimeUtil.NowDateTime() <= endAward)
				{
					return true;
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		
		public static bool HasEnoughBagSpaceForAwardGoods(GameClient client, BuChangItem buChangItem)
		{
			int needSpace = buChangItem.GoodsDataList.Count;
			return needSpace <= 0 || Global.CanAddGoodsDataList(client, buChangItem.GoodsDataList);
		}

		
		public static bool CheckGiveBuChang(GameClient client)
		{
			bool result;
			if (!BuChangManager.CanGiveBuChang())
			{
				result = false;
			}
			else
			{
				BuChangItem buChangItem = BuChangManager.GetBuChangItem(Global.GetUnionLevel(client, false));
				if (null == buChangItem)
				{
					result = false;
				}
				else
				{
					DateTime buChangDateTime = Global.GetBuChangStartDay();
					int buChangFlag = Global.GetRoleParamsInt32FromDB(client, "BuChangFlag");
					result = (buChangDateTime.DayOfYear != buChangFlag);
				}
			}
			return result;
		}

		
		public static void GiveBuChang(GameClient client)
		{
			if (!BuChangManager.CanGiveBuChang())
			{
				GameManager.LuaMgr.Error(client, GLang.GetLang(24, new object[0]), 0);
			}
			else
			{
				BuChangItem buChangItem = BuChangManager.GetBuChangItem(Global.GetUnionLevel(client, false));
				if (null == buChangItem)
				{
					GameManager.LuaMgr.Error(client, GLang.GetLang(25, new object[0]), 0);
				}
				else if (!BuChangManager.HasEnoughBagSpaceForAwardGoods(client, buChangItem))
				{
					GameManager.LuaMgr.Error(client, GLang.GetLang(26, new object[0]), 0);
				}
				else
				{
					DateTime buChangDateTime = Global.GetBuChangStartDay();
					int buChangFlag = Global.GetRoleParamsInt32FromDB(client, "BuChangFlag");
					if (buChangDateTime.DayOfYear == buChangFlag)
					{
						GameManager.LuaMgr.Error(client, GLang.GetLang(27, new object[0]), 0);
					}
					else
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "BuChangFlag", buChangDateTime.DayOfYear, true);
						for (int i = 0; i < buChangItem.GoodsDataList.Count; i++)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, buChangItem.GoodsDataList[i].GoodsID, buChangItem.GoodsDataList[i].GCount, buChangItem.GoodsDataList[i].Quality, "", buChangItem.GoodsDataList[i].Forge_level, buChangItem.GoodsDataList[i].Binding, 0, "", true, 1, "系统补偿物品", "1900-01-01 12:00:00", buChangItem.GoodsDataList[i].AddPropIndex, buChangItem.GoodsDataList[i].BornIndex, buChangItem.GoodsDataList[i].Lucky, buChangItem.GoodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
						}
						if (buChangItem.MoJing > 0)
						{
							GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, buChangItem.MoJing, "系统补偿", false, true, false);
						}
						if (buChangItem.Experience > 0L)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, buChangItem.Experience, false, true, false, "none");
						}
						client._IconStateMgr.CheckBuChangState(client);
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		
		private static Dictionary<RangeKey, BuChangItem> _BuChangItemDict = new Dictionary<RangeKey, BuChangItem>(new RangeKey(0, 0, null));
	}
}
