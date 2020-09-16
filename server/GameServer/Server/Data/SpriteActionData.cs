using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpriteActionData : IProtoBuffData
	{
		
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

		
		[ProtoMember(1)]
		public int roleID = 0;

		
		[ProtoMember(2)]
		public int mapCode = 0;

		
		[ProtoMember(3)]
		public int direction = 0;

		
		[ProtoMember(4)]
		public int action = 0;

		
		[ProtoMember(5)]
		public int toX = 0;

		
		[ProtoMember(6)]
		public int toY = 0;

		
		[ProtoMember(7)]
		public int targetX = 0;

		
		[ProtoMember(8)]
		public int targetY = 0;

		
		[ProtoMember(9)]
		public int yAngle = 0;

		
		[ProtoMember(10)]
		public int moveToX = 0;

		
		[ProtoMember(11)]
		public int moveToY = 0;

		
		[ProtoMember(12)]
		public long clientTicks;
	}
}
