using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x0200017F RID: 383
	[ProtoContract]
	public class SCAutoFight : IProtoBuffData
	{
		// Token: 0x060004A9 RID: 1193 RVA: 0x0004113A File Offset: 0x0003F33A
		public SCAutoFight()
		{
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00041164 File Offset: 0x0003F364
		public SCAutoFight(int state, int roleID, int fightType, int extTag1)
		{
			this.State = state;
			this.RoleID = roleID;
			this.FightType = fightType;
			this.Tag = extTag1;
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x000411B4 File Offset: 0x0003F3B4
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
					this.State = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.RoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.FightType = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.Tag = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x0004126C File Offset: 0x0003F46C
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.State, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.RoleID, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.FightType, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.Tag, true, 4, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.State, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.FightType, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.Tag, true, 0);
			return data;
		}

		// Token: 0x04000881 RID: 2177
		[ProtoMember(1)]
		public int State = 0;

		// Token: 0x04000882 RID: 2178
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x04000883 RID: 2179
		[ProtoMember(3)]
		public int FightType = 0;

		// Token: 0x04000884 RID: 2180
		[ProtoMember(4)]
		public int Tag = 0;
	}
}
