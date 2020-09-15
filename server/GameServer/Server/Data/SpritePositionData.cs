using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x020005A0 RID: 1440
	[ProtoContract]
	public class SpritePositionData : IProtoBuffData
	{
		// Token: 0x06001A43 RID: 6723 RVA: 0x00194888 File Offset: 0x00192A88
		public int fromBytes(byte[] data, int offset, int count)
		{
			int pos = offset;
			int mycount = 0;
			while (mycount < count)
			{
				int fieldnumber = -1;
				int wt = -1;
				ProtoUtil.GetTag(data, ref pos, ref fieldnumber, ref wt, ref mycount);
				switch (fieldnumber)
				{
				case 1:
					this.roleID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.mapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.toX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.toY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.currentPosTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x06001A44 RID: 6724 RVA: 0x00194958 File Offset: 0x00192B58
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.mapCode, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.toX, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.toY, true, 4, true, 0);
			total += ProtoUtil.GetLongSize(this.currentPosTicks, true, 5, true, 0L);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.mapCode, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.toX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.toY, true, 0);
			ProtoUtil.LongMemberToBytes(data, 5, ref offset, this.currentPosTicks, true, 0L);
			return data;
		}

		// Token: 0x04002887 RID: 10375
		[ProtoMember(1)]
		public int roleID = 0;

		// Token: 0x04002888 RID: 10376
		[ProtoMember(2)]
		public int mapCode = 0;

		// Token: 0x04002889 RID: 10377
		[ProtoMember(3)]
		public int toX = 0;

		// Token: 0x0400288A RID: 10378
		[ProtoMember(4)]
		public int toY = 0;

		// Token: 0x0400288B RID: 10379
		[ProtoMember(5)]
		public long currentPosTicks = 0L;
	}
}
