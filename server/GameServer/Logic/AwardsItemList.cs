using System;
using System.Collections.Generic;
using System.Text;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000119 RID: 281
	public class AwardsItemList
	{
		// Token: 0x17000018 RID: 24
		
		public List<AwardsItemData> Items
		{
			get
			{
				return this.list;
			}
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x0003EE20 File Offset: 0x0003D020
		public AwardsItemData ParseItem(string awardsString)
		{
			if (!string.IsNullOrEmpty(awardsString))
			{
				string[] strFields = awardsString.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < strFields.Length; i++)
				{
					string[] strGoods = strFields[i].Split(new char[]
					{
						','
					});
					if (strGoods.Length == 7)
					{
						return new AwardsItemData
						{
							GoodsID = Global.SafeConvertToInt32(strGoods[0]),
							GoodsNum = Global.SafeConvertToInt32(strGoods[1]),
							Binding = Global.SafeConvertToInt32(strGoods[2]),
							Level = Global.SafeConvertToInt32(strGoods[3]),
							AppendLev = Global.SafeConvertToInt32(strGoods[4]),
							IsHaveLuckyProp = Global.SafeConvertToInt32(strGoods[5]),
							ExcellencePorpValue = Global.SafeConvertToInt32(strGoods[6]),
							EndTime = "1900-01-01 12:00:00"
						};
					}
				}
			}
			return null;
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x0003EF1C File Offset: 0x0003D11C
		public bool ItemEqual(AwardsItemData item0, AwardsItemData item1)
		{
			return item0.GoodsID == item1.GoodsID && item0.Binding == item1.Binding && item0.Level == item1.Level && item0.AppendLev == item1.AppendLev && item0.IsHaveLuckyProp == item1.IsHaveLuckyProp && item0.ExcellencePorpValue == item1.ExcellencePorpValue && item0.Occupation == item1.Occupation && item0.EndTime == item1.EndTime;
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x0003EFB4 File Offset: 0x0003D1B4
		public void Add(string awardsString)
		{
			if (!string.IsNullOrEmpty(awardsString))
			{
				string[] strFields = awardsString.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < strFields.Length; i++)
				{
					AwardsItemData itemData = this.ParseItem(strFields[i]);
					if (null != itemData)
					{
						this.list.Add(itemData);
					}
				}
			}
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x0003F01C File Offset: 0x0003D21C
		public void AddNoRepeat(string awardsString)
		{
			if (!string.IsNullOrEmpty(awardsString))
			{
				string[] strFields = awardsString.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < strFields.Length; i++)
				{
					AwardsItemData itemData = this.ParseItem(strFields[i]);
					if (null != itemData)
					{
						int j;
						for (j = 0; j < this.list.Count; j++)
						{
							AwardsItemData item = this.list[j];
							if (this.ItemEqual(itemData, item))
							{
								item.GoodsNum += itemData.GoodsNum;
								break;
							}
						}
						if (j == this.list.Count)
						{
							this.list.Add(itemData);
						}
					}
				}
			}
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x0003F100 File Offset: 0x0003D300
		public override string ToString()
		{
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < this.list.Count; i++)
			{
				AwardsItemData item = this.list[i];
				result.AppendFormat("{0},{1},{2},{3},{4},{5},{6}|", new object[]
				{
					item.GoodsID,
					item.GoodsNum,
					item.Binding,
					item.Level,
					item.AppendLev,
					item.IsHaveLuckyProp,
					item.ExcellencePorpValue
				});
			}
			return result.ToString().TrimEnd(new char[]
			{
				'|'
			});
		}

		// Token: 0x04000602 RID: 1538
		private List<AwardsItemData> list = new List<AwardsItemData>();
	}
}
