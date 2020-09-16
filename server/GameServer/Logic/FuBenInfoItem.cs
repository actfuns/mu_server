using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class FuBenInfoItem
	{
		
		
		
		public long StartTicks
		{
			get
			{
				long startTicks;
				lock (this)
				{
					startTicks = this._StartTicks;
				}
				return startTicks;
			}
			set
			{
				lock (this)
				{
					this._StartTicks = value;
				}
			}
		}

		
		
		
		public long EndTicks
		{
			get
			{
				long endTicks;
				lock (this)
				{
					endTicks = this._EndTicks;
				}
				return endTicks;
			}
			set
			{
				lock (this)
				{
					this._EndTicks = value;
				}
			}
		}

		
		
		
		public int nDieCount
		{
			get
			{
				int nDieCount;
				lock (this)
				{
					nDieCount = this._nDieCount;
				}
				return nDieCount;
			}
			set
			{
				lock (this)
				{
					this._nDieCount = value;
				}
			}
		}

		
		public int FuBenSeqID = 0;

		
		private long _StartTicks = 0L;

		
		public long _EndTicks = 0L;

		
		public int GoodsBinding = 0;

		
		public int FuBenID = 0;

		
		public int _nDieCount = 0;

		
		public int nDayOfYear = TimeUtil.NowDateTime().DayOfYear;

		
		public double AwardRate = 1.0;
	}
}
