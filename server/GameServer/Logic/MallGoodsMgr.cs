using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class MallGoodsMgr
	{
		
		public static void InitMallGoodsPriceDict()
		{
			foreach (SystemXmlItem systemXmlItem in GameManager.systemMallMgr.SystemXmlItemDict.Values)
			{
				int goodsID = systemXmlItem.GetIntValue("GoodsID", -1);
				int price = systemXmlItem.GetIntValue("Price", -1);
				string property = systemXmlItem.GetStringValue("Property");
				if (string.IsNullOrEmpty(property))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("加载商城出售列表时, 物品配置属性错误，忽略。{0}", property), null, true);
				}
				else
				{
					string[] fields2 = property.Split(new char[]
					{
						','
					});
					if (4 != fields2.Length)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("加载加载商城出售列表时出售列表时, 物品配置项个数错误，忽略。{0}", systemXmlItem.GetIntValue("ID", -1)), null, true);
					}
					else
					{
						MallGoodsCacheItem mallGoodsCacheItem = new MallGoodsCacheItem
						{
							Price = price,
							Forge_level = Math.Max(0, Global.SafeConvertToInt32(fields2[0])),
							AppendPropLev = Math.Max(0, Global.SafeConvertToInt32(fields2[1])),
							Lucky = Math.Max(0, Global.SafeConvertToInt32(fields2[2])),
							ExcellenceInfo = Math.Max(0, Global.SafeConvertToInt32(fields2[3]))
						};
						MallGoodsMgr.MallGoodsCacheDict[goodsID] = mallGoodsCacheItem;
					}
				}
			}
			foreach (SystemXmlItem systemXmlItem in GameManager.systemQiZhenGeGoodsMgr.SystemXmlItemDict.Values)
			{
				int goodsID = systemXmlItem.GetIntValue("GoodsID", -1);
				int price = systemXmlItem.GetIntValue("Price", -1);
				string property = systemXmlItem.GetStringValue("Property");
				if (string.IsNullOrEmpty(property))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("加载商城出售列表时, 物品配置属性错误，忽略。{0}", property), null, true);
				}
				else
				{
					string[] fields2 = property.Split(new char[]
					{
						','
					});
					if (4 != fields2.Length)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("加载加载商城出售列表时出售列表时, 物品配置项个数错误，忽略。{0}", systemXmlItem.GetIntValue("ID", -1)), null, true);
					}
					else
					{
						MallGoodsCacheItem mallGoodsCacheItem = new MallGoodsCacheItem
						{
							Price = price,
							Forge_level = Math.Max(0, Global.SafeConvertToInt32(fields2[0])),
							AppendPropLev = Math.Max(0, Global.SafeConvertToInt32(fields2[1])),
							Lucky = Math.Max(0, Global.SafeConvertToInt32(fields2[2])),
							ExcellenceInfo = Math.Max(0, Global.SafeConvertToInt32(fields2[3]))
						};
						MallGoodsMgr.MallGoodsCacheDict[goodsID] = mallGoodsCacheItem;
					}
				}
			}
		}

		
		public static MallGoodsCacheItem GetMallGoodsCacheItem(int goodsID)
		{
			MallGoodsCacheItem mallGoodsCacheItem = null;
			MallGoodsCacheItem result;
			if (!MallGoodsMgr.MallGoodsCacheDict.TryGetValue(goodsID, out mallGoodsCacheItem))
			{
				result = null;
			}
			else
			{
				result = mallGoodsCacheItem;
			}
			return result;
		}

		
		private static Dictionary<int, MallGoodsCacheItem> MallGoodsCacheDict = new Dictionary<int, MallGoodsCacheItem>();
	}
}
