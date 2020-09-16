using System;
using System.Collections.Generic;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class GoodsBaoGuoCachingMgr
	{
		
		public static List<GoodsData> FindGoodsBaoGuoByID(int baoguoID)
		{
			List<GoodsData> goodsDataList = null;
			GoodsBaoGuoCachingMgr._GoodsBaoGuoDict.TryGetValue(baoguoID, out goodsDataList);
			return goodsDataList;
		}

		
		public static int LoadGoodsBaoGuoDict()
		{
			try
			{
				Dictionary<int, List<GoodsData>> goodsBaoGuoDict = new Dictionary<int, List<GoodsData>>();
				foreach (SystemXmlItem systemGoodsBaoGuoItem in GameManager.systemGoodsBaoGuoMgr.SystemXmlItemDict.Values)
				{
					int baoguoID = systemGoodsBaoGuoItem.GetIntValue("ID", -1);
					string goodsIDs = systemGoodsBaoGuoItem.GetStringValue("Item");
					if (string.IsNullOrEmpty(goodsIDs))
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("加载物品包时, 读取物品列表错误, BaoguoID={0}", baoguoID), null, true);
					}
					else
					{
						string[] goodsIDFields = goodsIDs.Split(new char[]
						{
							'|'
						});
						if (goodsIDFields == null || goodsIDFields.Length <= 0)
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("加载物品包时, 物品列表格式错误, BaoguoID={0}, List={1}", baoguoID, goodsIDFields), null, true);
						}
						else
						{
							List<GoodsData> goodsDataList = new List<GoodsData>();
							for (int i = 0; i < goodsIDFields.Length; i++)
							{
								string[] goodsPropFields = goodsIDFields[i].Trim().Split(new char[]
								{
									','
								});
								if (goodsPropFields == null || goodsPropFields.Length != 7)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("加载物品包时, 物品列表格中的物品配置项错误, BaoguoID={0}, GoodsItem={1}", baoguoID, goodsPropFields), null, true);
								}
								else
								{
									GoodsData goodsData = new GoodsData
									{
										Id = i,
										GoodsID = Global.SafeConvertToInt32(goodsPropFields[0]),
										Using = 0,
										Forge_level = Global.SafeConvertToInt32(goodsPropFields[3]),
										Starttime = "1900-01-01 12:00:00",
										Endtime = "1900-01-01 12:00:00",
										Site = 0,
										Quality = 0,
										Props = "",
										GCount = Global.SafeConvertToInt32(goodsPropFields[1]),
										Binding = Global.SafeConvertToInt32(goodsPropFields[2]),
										Jewellist = "",
										BagIndex = 0,
										AddPropIndex = 0,
										BornIndex = 0,
										Lucky = Global.SafeConvertToInt32(goodsPropFields[5]),
										Strong = 0,
										ExcellenceInfo = Global.SafeConvertToInt32(goodsPropFields[6]),
										AppendPropLev = Global.SafeConvertToInt32(goodsPropFields[4])
									};
									goodsDataList.Add(goodsData);
								}
							}
							goodsBaoGuoDict[baoguoID] = goodsDataList;
						}
					}
				}
				GoodsBaoGuoCachingMgr._GoodsBaoGuoDict = goodsBaoGuoDict;
				return 0;
			}
			catch (Exception)
			{
			}
			return -1;
		}

		
		private static Dictionary<int, List<GoodsData>> _GoodsBaoGuoDict = null;
	}
}
