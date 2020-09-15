using System;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000597 RID: 1431
	public class SpriteHitedInnerData : IProtoBuffDataEx, IProtoBuffData
	{
		// Token: 0x06001A24 RID: 6692 RVA: 0x001934B2 File Offset: 0x001916B2
		public SpriteHitedInnerData()
		{
		}

		// Token: 0x06001A25 RID: 6693 RVA: 0x001934BD File Offset: 0x001916BD
		public SpriteHitedInnerData(int enemy, int enemyX, int enemyY)
		{
			this.enemy = enemy;
			this.enemyX = enemyX;
			this.enemyY = enemyY;
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x001934E0 File Offset: 0x001916E0
		public int getBytesSize()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.enemy, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.enemyX, true, 2, true, 0);
			return total + ProtoUtil.GetIntSize(this.enemyY, true, 3, true, 0);
		}

		// Token: 0x06001A27 RID: 6695 RVA: 0x0019352C File Offset: 0x0019172C
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

		// Token: 0x06001A28 RID: 6696 RVA: 0x001935C8 File Offset: 0x001917C8
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

		// Token: 0x0400284D RID: 10317
		public int enemy;

		// Token: 0x0400284E RID: 10318
		public int enemyX;

		// Token: 0x0400284F RID: 10319
		public int enemyY;
	}
}
