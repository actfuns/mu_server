using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020005A4 RID: 1444
	[ProtoContract]
	public class TaskData
	{
		// Token: 0x06001A4B RID: 6731 RVA: 0x00194D5C File Offset: 0x00192F5C
		public void IncDoingTaskVal(int index)
		{
			switch (index)
			{
			case 1:
				this.DoingTaskVal1++;
				break;
			case 2:
				this.DoingTaskVal2++;
				break;
			}
		}

		// Token: 0x06001A4C RID: 6732 RVA: 0x00194DA0 File Offset: 0x00192FA0
		public int GetDoingTaskVal(int index)
		{
			int result;
			switch (index)
			{
			case 1:
				result = this.DoingTaskVal1;
				break;
			case 2:
				result = this.DoingTaskVal2;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		// Token: 0x06001A4D RID: 6733 RVA: 0x00194DDC File Offset: 0x00192FDC
		public void SetDoingTaskVal(int index, int val)
		{
			if (index == 1)
			{
				this.DoingTaskVal1 = val;
			}
			else
			{
				this.DoingTaskVal2 = val;
			}
		}

		// Token: 0x040028B1 RID: 10417
		[ProtoMember(1)]
		public int DbID;

		// Token: 0x040028B2 RID: 10418
		[ProtoMember(2)]
		public int DoingTaskID;

		// Token: 0x040028B3 RID: 10419
		[ProtoMember(3)]
		public int DoingTaskVal1;

		// Token: 0x040028B4 RID: 10420
		[ProtoMember(4)]
		public int DoingTaskVal2;

		// Token: 0x040028B5 RID: 10421
		[ProtoMember(5)]
		public int DoingTaskFocus;

		// Token: 0x040028B6 RID: 10422
		[ProtoMember(6)]
		public long AddDateTime;

		// Token: 0x040028B7 RID: 10423
		[ProtoMember(7)]
		public TaskAwardsData TaskAwards = null;

		// Token: 0x040028B8 RID: 10424
		[ProtoMember(8)]
		public int DoneCount = 0;

		// Token: 0x040028B9 RID: 10425
		[ProtoMember(9)]
		public int StarLevel;

		// Token: 0x040028BA RID: 10426
		[ProtoMember(10)]
		public int RefreshCount;

		// Token: 0x040028BB RID: 10427
		[ProtoMember(11)]
		public long ChengJiuVal;
	}
}
