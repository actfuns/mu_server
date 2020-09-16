using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpriteNotifyOtherMoveData : IProtoBuffData
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

		
		[ProtoMember(1)]
		public int roleID = 0;

		
		[ProtoMember(2)]
		public int mapCode = 0;

		
		[ProtoMember(3)]
		public int action = 0;

		
		[ProtoMember(4)]
		public int toX = 0;

		
		[ProtoMember(5)]
		public int toY = 0;

		
		[ProtoMember(6)]
		public int extAction = 0;

		
		[ProtoMember(7)]
		public int fromX = 0;

		
		[ProtoMember(8)]
		public int fromY = 0;

		
		[ProtoMember(9)]
		public long startMoveTicks = 0L;

		
		[ProtoMember(10)]
		public string pathString = "";

		
		[ProtoMember(11)]
		public double moveCost = 0.0;
	}
}
