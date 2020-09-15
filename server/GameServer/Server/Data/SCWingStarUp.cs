using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000187 RID: 391
	[ProtoContract]
	public class SCWingStarUp : IProtoBuffData
	{
		// Token: 0x060004C9 RID: 1225 RVA: 0x000423FA File Offset: 0x000405FA
		public SCWingStarUp()
		{
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x00042424 File Offset: 0x00040624
		public SCWingStarUp(int state, int roleID, int nNextStarLevel, int nNextStarExp)
		{
			this.RoleID = roleID;
			this.NextStarLevel = nNextStarLevel;
			this.NextStarExp = nNextStarExp;
			this.State = state;
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x00042474 File Offset: 0x00040674
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
					this.RoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.NextStarLevel = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.NextStarExp = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.State = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x0004252C File Offset: 0x0004072C
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.RoleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.NextStarLevel, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.NextStarExp, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.State, true, 4, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.NextStarLevel, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.NextStarExp, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.State, true, 0);
			return data;
		}

		// Token: 0x040008AD RID: 2221
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040008AE RID: 2222
		[ProtoMember(2)]
		public int NextStarLevel = 0;

		// Token: 0x040008AF RID: 2223
		[ProtoMember(3)]
		public int NextStarExp = 0;

		// Token: 0x040008B0 RID: 2224
		[ProtoMember(4)]
		public int State = 0;
	}
}
