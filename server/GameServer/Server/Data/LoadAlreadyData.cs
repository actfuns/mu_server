using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class LoadAlreadyData : IProtoBuffData
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

		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int MapCode = 0;

		
		[ProtoMember(3)]
		public long StartMoveTicks = 0L;

		
		[ProtoMember(4)]
		public int CurrentX = 0;

		
		[ProtoMember(5)]
		public int CurrentY = 0;

		
		[ProtoMember(6)]
		public int CurrentDirection = 0;

		
		[ProtoMember(7)]
		public int Action = 0;

		
		[ProtoMember(8)]
		public int ToX = 0;

		
		[ProtoMember(9)]
		public int ToY = 0;

		
		[ProtoMember(10)]
		public double MoveCost = 1.0;

		
		[ProtoMember(11)]
		public int ExtAction = 0;

		
		[ProtoMember(12)]
		public string PathString = "";

		
		[ProtoMember(13)]
		public int CurrentPathIndex = 0;
	}
}
