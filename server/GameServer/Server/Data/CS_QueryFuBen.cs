using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000173 RID: 371
	[ProtoContract]
	public class CS_QueryFuBen : IProtoBuffData
	{
		// Token: 0x06000492 RID: 1170 RVA: 0x00040900 File Offset: 0x0003EB00
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
					this.MapId = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.FuBenId = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!" + fieldnumber);
				}
			}
			return pos;
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x000409A8 File Offset: 0x0003EBA8
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.RoleId, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.MapId, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.FuBenId, true, 3, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.RoleId, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.MapId, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.FuBenId, true, 0);
			return data;
		}

		// Token: 0x04000844 RID: 2116
		[ProtoMember(1)]
		public int RoleId = 0;

		// Token: 0x04000845 RID: 2117
		[ProtoMember(2)]
		public int MapId = 0;

		// Token: 0x04000846 RID: 2118
		[ProtoMember(3)]
		public int FuBenId = 0;
	}
}
