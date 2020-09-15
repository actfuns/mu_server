using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x0200059B RID: 1435
	[ProtoContract]
	public class SpriteLifeChangeData2 : IProtoBuffData
	{
		// Token: 0x06001A34 RID: 6708 RVA: 0x00193E0C File Offset: 0x0019200C
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
					this.lifeV = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					goto IL_72;
				case 4:
					this.currentLifeV = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					goto IL_72;
				}
				continue;
				IL_72:
				throw new ArgumentException("error!!!");
			}
			return pos;
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x00193EAC File Offset: 0x001920AC
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.lifeV, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.currentLifeV, true, 4, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.lifeV, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.currentLifeV, true, 0);
			return data;
		}

		// Token: 0x04002869 RID: 10345
		[ProtoMember(1)]
		public int roleID;

		// Token: 0x0400286A RID: 10346
		[ProtoMember(2)]
		public int lifeV;

		// Token: 0x0400286B RID: 10347
		[ProtoMember(4)]
		public int currentLifeV;
	}
}
