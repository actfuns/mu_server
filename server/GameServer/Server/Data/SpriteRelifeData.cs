using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpriteRelifeData : IProtoBuffData
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
					this.x = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.y = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.direction = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.lifeV = ProtoUtil.DoubleMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.magicV = ProtoUtil.DoubleMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.force = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
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
			total += ProtoUtil.GetIntSize(this.x, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.y, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.direction, true, 4, true, 0);
			total += ProtoUtil.GetDoubleSize(this.lifeV, true, 5, true, 0.0);
			total += ProtoUtil.GetDoubleSize(this.magicV, true, 6, true, 0.0);
			total += ProtoUtil.GetIntSize(this.force, true, 7, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.x, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.y, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.direction, true, 0);
			ProtoUtil.DoubleMemberToBytes(data, 5, ref offset, this.lifeV, true, 0.0);
			ProtoUtil.DoubleMemberToBytes(data, 6, ref offset, this.magicV, true, 0.0);
			ProtoUtil.IntMemberToBytes(data, 7, ref offset, this.force, true, 0);
			return data;
		}

		
		[ProtoMember(1)]
		public int roleID;

		
		[ProtoMember(2)]
		public int x;

		
		[ProtoMember(3)]
		public int y;

		
		[ProtoMember(4)]
		public int direction;

		
		[ProtoMember(5)]
		public double lifeV;

		
		[ProtoMember(6)]
		public double magicV;

		
		[ProtoMember(7)]
		public int force;
	}
}
