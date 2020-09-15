using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006F6 RID: 1782
	public class AwardEffectTimeItem
	{
		// Token: 0x06002AED RID: 10989 RVA: 0x00264378 File Offset: 0x00262578
		public void Init(string goodsList, string timeList, string note)
		{
			if (!string.IsNullOrEmpty(goodsList) && !string.IsNullOrEmpty(timeList))
			{
				string[] szGoods = goodsList.Split(new char[]
				{
					'|'
				});
				string[] szTime = timeList.Split(new char[]
				{
					'|'
				});
				if (szGoods.Length == szTime.Length)
				{
					this.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(szGoods, note);
					this.GoodsTimeList = HuodongCachingMgr.ParseGoodsTimeList(szTime, note);
				}
			}
		}

		// Token: 0x06002AEE RID: 10990 RVA: 0x002643F4 File Offset: 0x002625F4
		public int GoodsCnt()
		{
			return (this.GoodsDataList != null) ? this.GoodsDataList.Count : 0;
		}

		// Token: 0x06002AEF RID: 10991 RVA: 0x00264420 File Offset: 0x00262620
		public AwardItem ToAwardItem()
		{
			AwardItem result = new AwardItem();
			if (this.GoodsDataList != null)
			{
				for (int i = 0; i < this.GoodsDataList.Count; i++)
				{
					GoodsData goods = this.GoodsDataList[i];
					bool bSetOk = false;
					if (this.GoodsTimeList != null && this.GoodsTimeList.Count > i)
					{
						AwardEffectTimeItem.TimeDetail time = this.GoodsTimeList[i];
						if (time.Type == AwardEffectTimeItem.EffectTimeType.ETT_LastMinutesFromNow)
						{
							DateTime now = TimeUtil.NowDateTime();
							goods.Starttime = now.ToString("yyyy-MM-dd HH:mm:ss");
							goods.Endtime = now.AddMinutes((double)time.LastMinutes).ToString("yyyy-MM-dd HH:mm:ss");
							bSetOk = true;
						}
						else if (time.Type == AwardEffectTimeItem.EffectTimeType.ETT_AbsoluteLastTime)
						{
							goods.Starttime = time.AbsoluteStartTime;
							goods.Endtime = time.AbsoluteEndTime;
							bSetOk = true;
						}
					}
					if (!bSetOk)
					{
						goods.Starttime = "1900-01-01 12:00:00";
						goods.Endtime = "1900-01-01 12:00:00";
					}
					result.GoodsDataList.Add(goods);
				}
			}
			return result;
		}

		// Token: 0x04003A03 RID: 14851
		private List<GoodsData> GoodsDataList = null;

		// Token: 0x04003A04 RID: 14852
		private List<AwardEffectTimeItem.TimeDetail> GoodsTimeList = null;

		// Token: 0x020006F7 RID: 1783
		public enum EffectTimeType
		{
			// Token: 0x04003A06 RID: 14854
			ETT_Unknown,
			// Token: 0x04003A07 RID: 14855
			ETT_LastMinutesFromNow,
			// Token: 0x04003A08 RID: 14856
			ETT_AbsoluteLastTime
		}

		// Token: 0x020006F8 RID: 1784
		public class TimeDetail
		{
			// Token: 0x04003A09 RID: 14857
			public AwardEffectTimeItem.EffectTimeType Type = AwardEffectTimeItem.EffectTimeType.ETT_Unknown;

			// Token: 0x04003A0A RID: 14858
			public int LastMinutes = 0;

			// Token: 0x04003A0B RID: 14859
			public string AbsoluteStartTime = "1900-01-01 12:00:00";

			// Token: 0x04003A0C RID: 14860
			public string AbsoluteEndTime = "1900-01-01 12:00:00";
		}
	}
}
