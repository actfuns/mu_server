using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpritePositionData : IProtoBuffData
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

		
		[ProtoMember(1)]
		public int roleID = 0;

		
		[ProtoMember(2)]
		public int mapCode = 0;

		
		[ProtoMember(3)]
		public int toX = 0;

		
		[ProtoMember(4)]
		public int toY = 0;

		
		[ProtoMember(5)]
		public long currentPosTicks = 0L;
	}
}
