using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000182 RID: 386
	[ProtoContract]
	public class SCFindMonster : IProtoBuffData
	{
		// Token: 0x060004B5 RID: 1205 RVA: 0x000416B6 File Offset: 0x0003F8B6
		public SCFindMonster()
		{
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x000416E0 File Offset: 0x0003F8E0
		public SCFindMonster(int roleID, int x, int y, int num)
		{
			this.RoleID = roleID;
			this.X = x;
			this.Y = y;
			this.Num = num;
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x00041730 File Offset: 0x0003F930
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
					this.X = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.Y = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.Num = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x000417E8 File Offset: 0x0003F9E8
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.RoleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.X, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.Y, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.Num, true, 4, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.X, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.Y, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.Num, true, 0);
			return data;
		}

		// Token: 0x0400088D RID: 2189
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x0400088E RID: 2190
		[ProtoMember(2)]
		public int X = 0;

		// Token: 0x0400088F RID: 2191
		[ProtoMember(3)]
		public int Y = 0;

		// Token: 0x04000890 RID: 2192
		[ProtoMember(4)]
		public int Num = 0;
	}
}
