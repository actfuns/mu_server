using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000114 RID: 276
	[ProtoContract]
	public class ActivityIconStateData : IProtoBuffData
	{
		// Token: 0x0600042F RID: 1071 RVA: 0x0003EB10 File Offset: 0x0003CD10
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

		// Token: 0x06000430 RID: 1072 RVA: 0x0003EBEC File Offset: 0x0003CDEC
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

		// Token: 0x040005D7 RID: 1495
		[ProtoMember(1)]
		public ushort[] arrIconState;
	}
}
