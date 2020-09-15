using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000596 RID: 1430
	[ProtoContract]
	public class SpriteHitedData : IProtoBuffData
	{
		// Token: 0x06001A21 RID: 6689 RVA: 0x00193308 File Offset: 0x00191508
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

		// Token: 0x06001A22 RID: 6690 RVA: 0x001933D8 File Offset: 0x001915D8
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

		// Token: 0x04002848 RID: 10312
		[ProtoMember(1)]
		public int roleId;

		// Token: 0x04002849 RID: 10313
		[ProtoMember(2)]
		public int enemy;

		// Token: 0x0400284A RID: 10314
		[ProtoMember(3)]
		public int enemyX;

		// Token: 0x0400284B RID: 10315
		[ProtoMember(4)]
		public int enemyY;

		// Token: 0x0400284C RID: 10316
		[ProtoMember(5)]
		public int magicCode;
	}
}
