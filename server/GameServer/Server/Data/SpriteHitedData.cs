using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpriteHitedData : IProtoBuffData
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
					this.roleId = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.enemy = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.enemyX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.enemyY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.magicCode = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
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
			total += ProtoUtil.GetIntSize(this.roleId, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.enemy, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.enemyX, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.enemyY, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.magicCode, true, 5, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.roleId, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.enemy, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.enemyX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.enemyY, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.magicCode, true, 0);
			return data;
		}

		
		[ProtoMember(1)]
		public int roleId;

		
		[ProtoMember(2)]
		public int enemy;

		
		[ProtoMember(3)]
		public int enemyX;

		
		[ProtoMember(4)]
		public int enemyY;

		
		[ProtoMember(5)]
		public int magicCode;
	}
}
