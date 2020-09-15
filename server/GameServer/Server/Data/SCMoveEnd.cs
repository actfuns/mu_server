using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000185 RID: 389
	[ProtoContract]
	public class SCMoveEnd : IProtoBuffData
	{
		// Token: 0x060004C1 RID: 1217 RVA: 0x00041EA0 File Offset: 0x000400A0
		public SCMoveEnd()
		{
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x00041EF0 File Offset: 0x000400F0
		public SCMoveEnd(int roleID, int mapCode, int action, int toNewMapX, int toNewMapY, int toNewDiection, int tryRun, long clientTicks = 0L)
		{
			this.RoleID = roleID;
			this.Action = action;
			this.MapCode = mapCode;
			this.ToMapX = toNewMapX;
			this.ToMapY = toNewMapY;
			this.ToDiection = toNewDiection;
			this.TryRun = tryRun;
			this.clientTicks = clientTicks;
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x00041F7C File Offset: 0x0004017C
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
					this.Action = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.MapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.ToMapX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.ToMapY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.ToDiection = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.TryRun = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 8:
					this.clientTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				}
			}
			return pos;
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x00042088 File Offset: 0x00040288
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.RoleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.Action, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.MapCode, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.ToMapX, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.ToMapY, true, 5, true, 0);
			total += ProtoUtil.GetIntSize(this.ToDiection, true, 6, true, 0);
			total += ProtoUtil.GetIntSize(this.TryRun, true, 7, true, 0);
			total += ProtoUtil.GetLongSize(this.clientTicks, true, 8, true, 0L);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.Action, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.MapCode, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.ToMapX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.ToMapY, true, 0);
			ProtoUtil.IntMemberToBytes(data, 6, ref offset, this.ToDiection, true, 0);
			ProtoUtil.IntMemberToBytes(data, 7, ref offset, this.TryRun, true, 0);
			ProtoUtil.LongMemberToBytes(data, 8, ref offset, this.clientTicks, true, 0L);
			return data;
		}

		// Token: 0x040008A0 RID: 2208
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040008A1 RID: 2209
		[ProtoMember(2)]
		public int Action = 0;

		// Token: 0x040008A2 RID: 2210
		[ProtoMember(3)]
		public int MapCode = 0;

		// Token: 0x040008A3 RID: 2211
		[ProtoMember(4)]
		public int ToMapX = 0;

		// Token: 0x040008A4 RID: 2212
		[ProtoMember(5)]
		public int ToMapY = 0;

		// Token: 0x040008A5 RID: 2213
		[ProtoMember(6)]
		public int ToDiection = 0;

		// Token: 0x040008A6 RID: 2214
		[ProtoMember(7)]
		public int TryRun = 0;

		// Token: 0x040008A7 RID: 2215
		[ProtoMember(8)]
		public long clientTicks = 0L;
	}
}
