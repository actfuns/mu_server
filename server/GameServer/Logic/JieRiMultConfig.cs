using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class JieRiMultConfig
	{
		
		public double GetMult()
		{
			double result;
			if (this.Effective == 0)
			{
				result = 0.0;
			}
			else
			{
				JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null == activity)
				{
					result = 0.0;
				}
				else if (!activity.InActivityTime())
				{
					result = 0.0;
				}
				else if (!this.InActivityTime())
				{
					result = 0.0;
				}
				else if (this.Multiplying < 1.0)
				{
					result = 0.0;
				}
				else
				{
					result = this.Multiplying;
				}
			}
			return result;
		}

		
		public bool InActivityTime()
		{
			JieriActivityConfig config = HuodongCachingMgr.GetJieriActivityConfig();
			bool result;
			if (null == config)
			{
				result = false;
			}
			else if (!config.InList(41))
			{
				result = false;
			}
			else
			{
				JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null == activity)
				{
					result = false;
				}
				else if (!activity.InActivityTime())
				{
					result = false;
				}
				else
				{
					DateTime startTime = DateTime.Parse(this.StartDate);
					DateTime endTime = DateTime.Parse(this.EndDate);
					result = (TimeUtil.NowDateTime() >= startTime && TimeUtil.NowDateTime() <= endTime);
				}
			}
			return result;
		}

		
		public int index;

		
		public int type;

		
		public double Multiplying;

		
		public int Effective;

		
		public string StartDate;

		
		public string EndDate;
	}
}
