using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x02000709 RID: 1801
	public class JieRiMultConfig
	{
		// Token: 0x06002B44 RID: 11076 RVA: 0x002679FC File Offset: 0x00265BFC
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

		// Token: 0x06002B45 RID: 11077 RVA: 0x00267AA0 File Offset: 0x00265CA0
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

		// Token: 0x04003A35 RID: 14901
		public int index;

		// Token: 0x04003A36 RID: 14902
		public int type;

		// Token: 0x04003A37 RID: 14903
		public double Multiplying;

		// Token: 0x04003A38 RID: 14904
		public int Effective;

		// Token: 0x04003A39 RID: 14905
		public string StartDate;

		// Token: 0x04003A3A RID: 14906
		public string EndDate;
	}
}
