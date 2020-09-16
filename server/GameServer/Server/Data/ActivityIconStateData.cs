using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class ActivityIconStateData : IProtoBuffData
	{
		
		public int fromBytes(byte[] data, int offset, int count)
		{
			int pos = offset;
			int mycount = 0;
			int arrCount = 0;
			while (mycount < count)
			{
				int fieldnumber = -1;
				int wt = -1;
				ProtoUtil.GetTag(data, ref pos, ref fieldnumber, ref wt, ref mycount);
				int num = fieldnumber;
				if (num != 1)
				{
					throw new ArgumentException("error!!!");
				}
				arrCount++;
				ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
			}
			if (arrCount > 0)
			{
				this.arrIconState = new ushort[arrCount];
				pos = offset;
				mycount = 0;
				arrCount = 0;
				while (mycount < count)
				{
					int fieldnumber = -1;
					int wt = -1;
					ProtoUtil.GetTag(data, ref pos, ref fieldnumber, ref wt, ref mycount);
					int num = fieldnumber;
					if (num != 1)
					{
						throw new ArgumentException("error!!!");
					}
					this.arrIconState[arrCount++] = (ushort)ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
				}
			}
			return pos;
		}

		
		public byte[] toBytes()
		{
			int total = 0;
			if (this.arrIconState != null && this.arrIconState.Length > 0)
			{
				for (int i = 0; i < this.arrIconState.Length; i++)
				{
					total += ProtoUtil.GetIntSize((int)this.arrIconState[i], true, 1, true, 0);
				}
			}
			byte[] data = new byte[total];
			int offset = 0;
			if (this.arrIconState != null && this.arrIconState.Length > 0)
			{
				for (int i = 0; i < this.arrIconState.Length; i++)
				{
					ProtoUtil.IntMemberToBytes(data, 1, ref offset, (int)this.arrIconState[i], true, 0);
				}
			}
			return data;
		}

		
		[ProtoMember(1)]
		public ushort[] arrIconState;
	}
}
