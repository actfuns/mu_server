using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;

namespace GameServer.Logic.YueKa
{
	// Token: 0x02000820 RID: 2080
	internal class YueKaAward
	{
		// Token: 0x06003ADB RID: 15067 RVA: 0x0031F4A8 File Offset: 0x0031D6A8
		public void Init(XElement xml)
		{
			this.Day = (int)Global.GetSafeAttributeLong(xml, "Day");
			this.BindZuanShi = (int)Global.GetSafeAttributeLong(xml, "BandZuanShiAward");
			this._InitGoods(this.AllGoodsList, Global.GetSafeAttributeStr(xml, "GoodsOne"));
			this._InitGoods(this.OccGoodsList, Global.GetSafeAttributeStr(xml, "GoodsTwo"));
		}

		// Token: 0x06003ADC RID: 15068 RVA: 0x0031F50C File Offset: 0x0031D70C
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

		// Token: 0x06003ADD RID: 15069 RVA: 0x0031F5EC File Offset: 0x0031D7EC
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

		// Token: 0x06003ADE RID: 15070 RVA: 0x0031F6C8 File Offset: 0x0031D8C8
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

		// Token: 0x040044ED RID: 17645
		public int Day = 0;

		// Token: 0x040044EE RID: 17646
		public int BindZuanShi = 0;

		// Token: 0x040044EF RID: 17647
		public List<Tuple<int, int, int, int, int, int, int>> AllGoodsList = new List<Tuple<int, int, int, int, int, int, int>>();

		// Token: 0x040044F0 RID: 17648
		public List<Tuple<int, int, int, int, int, int, int>> OccGoodsList = new List<Tuple<int, int, int, int, int, int, int>>();
	}
}
