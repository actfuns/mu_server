using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SCMoveEnd : IProtoBuffData
	{
		
		public SCMoveEnd()
		{
		}

		
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

		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int Action = 0;

		
		[ProtoMember(3)]
		public int MapCode = 0;

		
		[ProtoMember(4)]
		public int ToMapX = 0;

		
		[ProtoMember(5)]
		public int ToMapY = 0;

		
		[ProtoMember(6)]
		public int ToDiection = 0;

		
		[ProtoMember(7)]
		public int TryRun = 0;

		
		[ProtoMember(8)]
		public long clientTicks = 0L;
	}
}
