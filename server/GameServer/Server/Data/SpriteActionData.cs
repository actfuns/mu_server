using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000592 RID: 1426
	[ProtoContract]
	public class SpriteActionData : IProtoBuffData
	{
		// Token: 0x06001A17 RID: 6679 RVA: 0x001928E4 File Offset: 0x00190AE4
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
					this.direction = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.action = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.toX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.toY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.targetX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 8:
					this.targetY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 9:
					this.yAngle = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 10:
					this.moveToX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 11:
					this.moveToY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 12:
					this.clientTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				}
			}
			return pos;
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x00192A58 File Offset: 0x00190C58
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.mapCode, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.direction, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.action, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.toX, true, 5, true, 0);
			total += ProtoUtil.GetIntSize(this.toY, true, 6, true, 0);
			total += ProtoUtil.GetIntSize(this.targetX, true, 7, true, 0);
			total += ProtoUtil.GetIntSize(this.targetY, true, 8, true, 0);
			total += ProtoUtil.GetIntSize(this.yAngle, true, 9, true, 0);
			total += ProtoUtil.GetIntSize(this.moveToX, true, 10, true, 0);
			total += ProtoUtil.GetIntSize(this.moveToY, true, 11, true, 0);
			total += ProtoUtil.GetLongSize(this.clientTicks, true, 12, true, 0L);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.mapCode, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.direction, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.action, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.toX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 6, ref offset, this.toY, true, 0);
			ProtoUtil.IntMemberToBytes(data, 7, ref offset, this.targetX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 8, ref offset, this.targetY, true, 0);
			ProtoUtil.IntMemberToBytes(data, 9, ref offset, this.yAngle, true, 0);
			ProtoUtil.IntMemberToBytes(data, 10, ref offset, this.moveToX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 11, ref offset, this.moveToY, true, 0);
			ProtoUtil.LongMemberToBytes(data, 12, ref offset, this.clientTicks, true, 0L);
			return data;
		}

		// Token: 0x04002823 RID: 10275
		[ProtoMember(1)]
		public int roleID = 0;

		// Token: 0x04002824 RID: 10276
		[ProtoMember(2)]
		public int mapCode = 0;

		// Token: 0x04002825 RID: 10277
		[ProtoMember(3)]
		public int direction = 0;

		// Token: 0x04002826 RID: 10278
		[ProtoMember(4)]
		public int action = 0;

		// Token: 0x04002827 RID: 10279
		[ProtoMember(5)]
		public int toX = 0;

		// Token: 0x04002828 RID: 10280
		[ProtoMember(6)]
		public int toY = 0;

		// Token: 0x04002829 RID: 10281
		[ProtoMember(7)]
		public int targetX = 0;

		// Token: 0x0400282A RID: 10282
		[ProtoMember(8)]
		public int targetY = 0;

		// Token: 0x0400282B RID: 10283
		[ProtoMember(9)]
		public int yAngle = 0;

		// Token: 0x0400282C RID: 10284
		[ProtoMember(10)]
		public int moveToX = 0;

		// Token: 0x0400282D RID: 10285
		[ProtoMember(11)]
		public int moveToY = 0;

		// Token: 0x0400282E RID: 10286
		[ProtoMember(12)]
		public long clientTicks;
	}
}
