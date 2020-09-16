using System;
using System.Collections.Generic;
using GameServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameServer.Tools
{
	
	public class GoodsHelper
	{
		
		public static List<GoodsData> ParseGoodsDataList(string[] fields, string fileName)
		{
			int attrCount = 7;
			List<GoodsData> goodsDataList = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] sa = fields[i].Split(new char[]
				{
					','
				});
				if (sa.Length != attrCount)
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("解析{0}文件中的奖励项时失败, 物品配置项个数错误", fileName), null, true);
				}
				else
				{
					int[] goodsFields = Global.StringArray2IntArray(sa);
					GoodsData goodsData = Global.GetNewGoodsData(goodsFields[0], goodsFields[1], 0, goodsFields[3], goodsFields[2], 0, goodsFields[5], 0, goodsFields[6], goodsFields[4], 0);
					goodsDataList.Add(goodsData);
				}
			}
			return goodsDataList;
		}

		
		public static List<GoodsData> GetAwardPro(GameClient client, List<GoodsData> proGoodsList)
		{
			List<GoodsData> result;
			if (proGoodsList == null || proGoodsList.Count <= 0)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				foreach (GoodsData data in proGoodsList)
				{
					if (Global.IsCanGiveRewardByOccupation(client, data.GoodsID))
					{
						list.Add(data);
					}
				}
				result = list;
			}
			return result;
		}

		
		public static GoodsData ParseGoodsData(string fields, string fileName)
		{
			int attrCount = 7;
			string[] sa = fields.Split(new char[]
			{
				','
			});
			GoodsData result;
			if (sa.Length != attrCount)
			{
				LogManager.WriteLog(LogTypes.Warning, string.Format("解析{0}文件中的奖励项时失败, 物品配置项个数错误", fileName), null, true);
				result = null;
			}
			else
			{
				int[] goodsFields = Global.StringArray2IntArray(sa);
				result = Global.GetNewGoodsData(goodsFields[0], goodsFields[1], 0, goodsFields[3], goodsFields[2], 0, goodsFields[5], 0, goodsFields[6], goodsFields[4], 0);
			}
			return result;
		}
	}
}
