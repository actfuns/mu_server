using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;

namespace GameServer.Logic.YueKa
{
	
	internal class YueKaAward
	{
		
		public void Init(XElement xml)
		{
			this.Day = (int)Global.GetSafeAttributeLong(xml, "Day");
			this.BindZuanShi = (int)Global.GetSafeAttributeLong(xml, "BandZuanShiAward");
			this._InitGoods(this.AllGoodsList, Global.GetSafeAttributeStr(xml, "GoodsOne"));
			this._InitGoods(this.OccGoodsList, Global.GetSafeAttributeStr(xml, "GoodsTwo"));
		}

		
		public List<GoodsData> GetGoodsByOcc(int occ)
		{
			List<GoodsData> goodsDataList = new List<GoodsData>();
			foreach (Tuple<int, int, int, int, int, int, int> detail in this.AllGoodsList)
			{
				GoodsData data = this._ParseGoodsFromDetail(detail);
				goodsDataList.Add(data);
			}
			foreach (Tuple<int, int, int, int, int, int, int> detail in this.OccGoodsList)
			{
				if (Global.IsRoleOccupationMatchGoods(occ, detail.Item1))
				{
					GoodsData data = this._ParseGoodsFromDetail(detail);
					goodsDataList.Add(data);
				}
			}
			return goodsDataList;
		}

		
		private GoodsData _ParseGoodsFromDetail(Tuple<int, int, int, int, int, int, int> detail)
		{
			return new GoodsData
			{
				Id = -1,
				GoodsID = detail.Item1,
				Using = 0,
				Forge_level = detail.Item4,
				Starttime = "1900-01-01 12:00:00",
				Endtime = "1900-01-01 12:00:00",
				Site = 0,
				Quality = 0,
				Props = "",
				GCount = detail.Item2,
				Binding = detail.Item3,
				Jewellist = "",
				BagIndex = 0,
				AddPropIndex = 0,
				BornIndex = 0,
				Lucky = detail.Item6,
				Strong = 0,
				ExcellenceInfo = detail.Item7,
				AppendPropLev = detail.Item5,
				ChangeLifeLevForEquip = 0
			};
		}

		
		private void _InitGoods(List<Tuple<int, int, int, int, int, int, int>> lst, string goods)
		{
			if (!string.IsNullOrEmpty(goods))
			{
				string[] fields = goods.Split(new char[]
				{
					'|'
				});
				foreach (string field in fields)
				{
					string[] details = field.Split(new char[]
					{
						','
					});
					if (details.Length == 7)
					{
						int goodsID = Convert.ToInt32(details[0]);
						int goodsCnt = Convert.ToInt32(details[1]);
						int goodsBind = Convert.ToInt32(details[2]);
						int goodsForge = Convert.ToInt32(details[3]);
						int goodsAppend = Convert.ToInt32(details[4]);
						int goodsLucky = Convert.ToInt32(details[5]);
						int goodsExcellence = Convert.ToInt32(details[6]);
						lst.Add(new Tuple<int, int, int, int, int, int, int>(goodsID, goodsCnt, goodsBind, goodsForge, goodsAppend, goodsLucky, goodsExcellence));
					}
				}
			}
		}

		
		public int Day = 0;

		
		public int BindZuanShi = 0;

		
		public List<Tuple<int, int, int, int, int, int, int>> AllGoodsList = new List<Tuple<int, int, int, int, int, int, int>>();

		
		public List<Tuple<int, int, int, int, int, int, int>> OccGoodsList = new List<Tuple<int, int, int, int, int, int, int>>();
	}
}
