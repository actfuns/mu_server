using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000157 RID: 343
	[ProtoContract]
	public class LoadAlreadyData : IProtoBuffData
	{
		// Token: 0x06000470 RID: 1136 RVA: 0x0003FB98 File Offset: 0x0003DD98
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
					this.RoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.MapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.StartMoveTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.CurrentX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.CurrentY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.CurrentDirection = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.Action = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 8:
					this.ToX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 9:
					this.ToY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 10:
					this.MoveCost = ProtoUtil.DoubleMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 11:
					this.ExtAction = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 12:
					this.PathString = ProtoUtil.StringMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 13:
					this.CurrentPathIndex = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x0003FD34 File Offset: 0x0003DF34
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.RoleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.MapCode, true, 2, true, 0);
			total += ProtoUtil.GetLongSize(this.StartMoveTicks, true, 3, true, 0L);
			total += ProtoUtil.GetIntSize(this.CurrentX, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.CurrentY, true, 5, true, 0);
			total += ProtoUtil.GetIntSize(this.CurrentDirection, true, 6, true, 0);
			total += ProtoUtil.GetIntSize(this.Action, true, 7, true, 0);
			total += ProtoUtil.GetIntSize(this.ToX, true, 8, true, 0);
			total += ProtoUtil.GetIntSize(this.ToY, true, 9, true, 0);
			total += ProtoUtil.GetDoubleSize(this.MoveCost, true, 10, true, 0.0);
			total += ProtoUtil.GetIntSize(this.ExtAction, true, 11, true, 0);
			total += ProtoUtil.GetStringSize(this.PathString, true, 12);
			total += ProtoUtil.GetIntSize(this.CurrentPathIndex, true, 13, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.MapCode, true, 0);
			ProtoUtil.LongMemberToBytes(data, 3, ref offset, this.StartMoveTicks, true, 0L);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.CurrentX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.CurrentY, true, 0);
			ProtoUtil.IntMemberToBytes(data, 6, ref offset, this.CurrentDirection, true, 0);
			ProtoUtil.IntMemberToBytes(data, 7, ref offset, this.Action, true, 0);
			ProtoUtil.IntMemberToBytes(data, 8, ref offset, this.ToX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 9, ref offset, this.ToY, true, 0);
			ProtoUtil.DoubleMemberToBytes(data, 10, ref offset, this.MoveCost, true, 0.0);
			ProtoUtil.IntMemberToBytes(data, 11, ref offset, this.ExtAction, true, 0);
			ProtoUtil.StringMemberToBytes(data, 12, ref offset, this.PathString);
			ProtoUtil.IntMemberToBytes(data, 13, ref offset, this.CurrentPathIndex, true, 0);
			return data;
		}

		// Token: 0x04000793 RID: 1939
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000794 RID: 1940
		[ProtoMember(2)]
		public int MapCode = 0;

		// Token: 0x04000795 RID: 1941
		[ProtoMember(3)]
		public long StartMoveTicks = 0L;

		// Token: 0x04000796 RID: 1942
		[ProtoMember(4)]
		public int CurrentX = 0;

		// Token: 0x04000797 RID: 1943
		[ProtoMember(5)]
		public int CurrentY = 0;

		// Token: 0x04000798 RID: 1944
		[ProtoMember(6)]
		public int CurrentDirection = 0;

		// Token: 0x04000799 RID: 1945
		[ProtoMember(7)]
		public int Action = 0;

		// Token: 0x0400079A RID: 1946
		[ProtoMember(8)]
		public int ToX = 0;

		// Token: 0x0400079B RID: 1947
		[ProtoMember(9)]
		public int ToY = 0;

		// Token: 0x0400079C RID: 1948
		[ProtoMember(10)]
		public double MoveCost = 1.0;

		// Token: 0x0400079D RID: 1949
		[ProtoMember(11)]
		public int ExtAction = 0;

		// Token: 0x0400079E RID: 1950
		[ProtoMember(12)]
		public string PathString = "";

		// Token: 0x0400079F RID: 1951
		[ProtoMember(13)]
		public int CurrentPathIndex = 0;
	}
}
