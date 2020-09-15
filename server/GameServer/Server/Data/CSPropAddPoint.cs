using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000175 RID: 373
	[ProtoContract]
	public class CSPropAddPoint : IProtoBuffData
	{
		// Token: 0x06000498 RID: 1176 RVA: 0x00040BDC File Offset: 0x0003EDDC
		public CSPropAddPoint()
		{
			this.RoleID = 0;
			this.Strength = 0;
			this.Intelligence = 0;
			this.Dexterity = 0;
			this.Constitution = 0;
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x00040C38 File Offset: 0x0003EE38
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
					this.Strength = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.Intelligence = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.Dexterity = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.Constitution = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00040D08 File Offset: 0x0003EF08
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.RoleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.Strength, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.Intelligence, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.Dexterity, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.Constitution, true, 5, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.Strength, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.Intelligence, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.Dexterity, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.Constitution, true, 0);
			return data;
		}

		// Token: 0x0400084B RID: 2123
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x0400084C RID: 2124
		[ProtoMember(2)]
		public int Strength = 0;

		// Token: 0x0400084D RID: 2125
		[ProtoMember(3)]
		public int Intelligence = 0;

		// Token: 0x0400084E RID: 2126
		[ProtoMember(4)]
		public int Dexterity = 0;

		// Token: 0x0400084F RID: 2127
		[ProtoMember(5)]
		public int Constitution = 0;
	}
}
