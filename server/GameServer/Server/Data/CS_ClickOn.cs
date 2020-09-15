using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000172 RID: 370
	[ProtoContract]
	public class CS_ClickOn : IProtoBuffData
	{
		// Token: 0x06000490 RID: 1168 RVA: 0x00040798 File Offset: 0x0003E998
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
					this.RoleId = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.MapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.NpcId = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.ExtId = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x00040850 File Offset: 0x0003EA50
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.RoleId, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.MapCode, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.NpcId, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.ExtId, true, 4, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.RoleId, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.MapCode, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.NpcId, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.ExtId, true, 0);
			return data;
		}

		// Token: 0x04000840 RID: 2112
		[ProtoMember(1)]
		public int RoleId = 0;

		// Token: 0x04000841 RID: 2113
		[ProtoMember(2)]
		public int MapCode = 0;

		// Token: 0x04000842 RID: 2114
		[ProtoMember(3)]
		public int NpcId = 0;

		// Token: 0x04000843 RID: 2115
		[ProtoMember(4)]
		public int ExtId = 0;
	}
}
