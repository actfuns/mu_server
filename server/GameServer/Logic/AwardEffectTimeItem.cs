using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	
	public class AwardEffectTimeItem
	{
		
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

		
		public int GoodsCnt()
		{
			return (this.GoodsDataList != null) ? this.GoodsDataList.Count : 0;
		}

		
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

		
		private List<GoodsData> GoodsDataList = null;

		
		private List<AwardEffectTimeItem.TimeDetail> GoodsTimeList = null;

		
		public enum EffectTimeType
		{
			
			ETT_Unknown,
			
			ETT_LastMinutesFromNow,
			
			ETT_AbsoluteLastTime
		}

		
		public class TimeDetail
		{
			
			public AwardEffectTimeItem.EffectTimeType Type = AwardEffectTimeItem.EffectTimeType.ETT_Unknown;

			
			public int LastMinutes = 0;

			
			public string AbsoluteStartTime = "1900-01-01 12:00:00";

			
			public string AbsoluteEndTime = "1900-01-01 12:00:00";
		}
	}
}
