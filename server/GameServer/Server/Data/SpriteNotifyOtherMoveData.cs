using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x0200059F RID: 1439
	[ProtoContract]
	public class SpriteNotifyOtherMoveData : IProtoBuffData
	{
		// Token: 0x06001A41 RID: 6721 RVA: 0x00194560 File Offset: 0x00192760
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
					this.action = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.toX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.toY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.extAction = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.fromX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 8:
					this.fromY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 9:
					this.startMoveTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 10:
					this.pathString = ProtoUtil.StringMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 11:
					this.moveCost = ProtoUtil.DoubleMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x06001A42 RID: 6722 RVA: 0x001946C8 File Offset: 0x001928C8
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.mapCode, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.action, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.toX, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.toY, true, 5, true, 0);
			total += ProtoUtil.GetIntSize(this.extAction, true, 6, true, 0);
			total += ProtoUtil.GetIntSize(this.fromX, true, 7, true, 0);
			total += ProtoUtil.GetIntSize(this.fromY, true, 8, true, 0);
			total += ProtoUtil.GetLongSize(this.startMoveTicks, true, 9, true, 0L);
			total += ProtoUtil.GetStringSize(this.pathString, true, 10);
			total += ProtoUtil.GetDoubleSize(this.moveCost, true, 11, true, 0.0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.mapCode, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.action, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.toX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.toY, true, 0);
			ProtoUtil.IntMemberToBytes(data, 6, ref offset, this.extAction, true, 0);
			ProtoUtil.IntMemberToBytes(data, 7, ref offset, this.fromX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 8, ref offset, this.fromY, true, 0);
			ProtoUtil.LongMemberToBytes(data, 9, ref offset, this.startMoveTicks, true, 0L);
			ProtoUtil.StringMemberToBytes(data, 10, ref offset, this.pathString);
			ProtoUtil.DoubleMemberToBytes(data, 11, ref offset, this.moveCost, true, 0.0);
			return data;
		}

		// Token: 0x0400287C RID: 10364
		[ProtoMember(1)]
		public int roleID = 0;

		// Token: 0x0400287D RID: 10365
		[ProtoMember(2)]
		public int mapCode = 0;

		// Token: 0x0400287E RID: 10366
		[ProtoMember(3)]
		public int action = 0;

		// Token: 0x0400287F RID: 10367
		[ProtoMember(4)]
		public int toX = 0;

		// Token: 0x04002880 RID: 10368
		[ProtoMember(5)]
		public int toY = 0;

		// Token: 0x04002881 RID: 10369
		[ProtoMember(6)]
		public int extAction = 0;

		// Token: 0x04002882 RID: 10370
		[ProtoMember(7)]
		public int fromX = 0;

		// Token: 0x04002883 RID: 10371
		[ProtoMember(8)]
		public int fromY = 0;

		// Token: 0x04002884 RID: 10372
		[ProtoMember(9)]
		public long startMoveTicks = 0L;

		// Token: 0x04002885 RID: 10373
		[ProtoMember(10)]
		public string pathString = "";

		// Token: 0x04002886 RID: 10374
		[ProtoMember(11)]
		public double moveCost = 0.0;
	}
}
