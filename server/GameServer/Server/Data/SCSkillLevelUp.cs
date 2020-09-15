using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000186 RID: 390
	[ProtoContract]
	public class SCSkillLevelUp : IProtoBuffData
	{
		// Token: 0x060004C5 RID: 1221 RVA: 0x000421C8 File Offset: 0x000403C8
		public SCSkillLevelUp()
		{
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x000421F8 File Offset: 0x000403F8
		public SCSkillLevelUp(int state, int roleID, int skillID, int skillLevel, int SkillUsedNum)
		{
			this.State = state;
			this.RoleID = roleID;
			this.SkillID = skillID;
			this.SkillLevel = skillLevel;
			this.SkillUsedNum = SkillUsedNum;
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x00042258 File Offset: 0x00040458
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
					this.SkillID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.SkillLevel = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.SkillUsedNum = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x00042328 File Offset: 0x00040528
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.State, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.RoleID, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.SkillID, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.SkillLevel, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.SkillUsedNum, true, 5, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.State, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.SkillID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.SkillLevel, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.SkillUsedNum, true, 0);
			return data;
		}

		// Token: 0x040008A8 RID: 2216
		[ProtoMember(1)]
		public int State = 0;

		// Token: 0x040008A9 RID: 2217
		[ProtoMember(2)]
		public int RoleID = 0;

		// Token: 0x040008AA RID: 2218
		[ProtoMember(3)]
		public int SkillID = 0;

		// Token: 0x040008AB RID: 2219
		[ProtoMember(4)]
		public int SkillLevel = 0;

		// Token: 0x040008AC RID: 2220
		[ProtoMember(5)]
		public int SkillUsedNum = 0;
	}
}
