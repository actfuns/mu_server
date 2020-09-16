using System;
using Tmsk.Contract;

namespace Server.Data
{
	
	public class SpriteHitedInnerData : IProtoBuffDataEx, IProtoBuffData
	{
		
		public SpriteHitedInnerData()
		{
		}

		
		public SpriteHitedInnerData(int enemy, int enemyX, int enemyY)
		{
			this.enemy = enemy;
			this.enemyX = enemyX;
			this.enemyY = enemyY;
		}

		
		public int getBytesSize()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.enemy, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.enemyX, true, 2, true, 0);
			return total + ProtoUtil.GetIntSize(this.enemyY, true, 3, true, 0);
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
					this.enemy = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.enemyX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.enemyY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
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
			total += ProtoUtil.GetIntSize(this.enemy, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.enemyX, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.enemyY, true, 3, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.enemy, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.enemyX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.enemyY, true, 0);
			return data;
		}

		
		public int enemy;

		
		public int enemyX;

		
		public int enemyY;
	}
}
