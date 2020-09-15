using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000598 RID: 1432
	[ProtoContract]
	public class SpriteHitedDataEx : IProtoBuffData
	{
		// Token: 0x06001A29 RID: 6697 RVA: 0x00193652 File Offset: 0x00191852
		public void AddEnemy(int enemy, int enemyX = 0, int enemyY = 0)
		{
			this.enemys.Add(new SpriteHitedInnerData(enemy, enemyX, enemyY));
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x0019366C File Offset: 0x0019186C
		public int getMySize()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.roleId, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.magicCode, true, 2, true, 0);
			return total + ProtoUtil.GetListBytesSize<SpriteHitedInnerData>(this.enemys, true, 3);
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x001936B8 File Offset: 0x001918B8
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
					this.magicCode = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					ProtoUtil.ListMemberFromBytes<SpriteHitedInnerData>(this.enemys, data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x00193754 File Offset: 0x00191954
		public byte[] toBytes()
		{
			int total = this.getMySize();
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.roleId, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.magicCode, true, 0);
			ProtoUtil.ListToBytes<SpriteHitedInnerData>(this.enemys, 3, ref offset, data);
			return data;
		}

		// Token: 0x04002850 RID: 10320
		[ProtoMember(1)]
		public int roleId;

		// Token: 0x04002851 RID: 10321
		[ProtoMember(2)]
		public int magicCode;

		// Token: 0x04002852 RID: 10322
		[ProtoMember(3)]
		public List<SpriteHitedInnerData> enemys = new List<SpriteHitedInnerData>();
	}
}
