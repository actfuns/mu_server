using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TaskData
	{
		
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

		
		[ProtoMember(1)]
		public int DbID;

		
		[ProtoMember(2)]
		public int DoingTaskID;

		
		[ProtoMember(3)]
		public int DoingTaskVal1;

		
		[ProtoMember(4)]
		public int DoingTaskVal2;

		
		[ProtoMember(5)]
		public int DoingTaskFocus;

		
		[ProtoMember(6)]
		public long AddDateTime;

		
		[ProtoMember(7)]
		public TaskAwardsData TaskAwards = null;

		
		[ProtoMember(8)]
		public int DoneCount = 0;

		
		[ProtoMember(9)]
		public int StarLevel;

		
		[ProtoMember(10)]
		public int RefreshCount;

		
		[ProtoMember(11)]
		public long ChengJiuVal;
	}
}
