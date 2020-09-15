using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000183 RID: 387
	[ProtoContract]
	public class SCMapChange : IProtoBuffData
	{
		// Token: 0x060004B9 RID: 1209 RVA: 0x00041896 File Offset: 0x0003FA96
		public SCMapChange()
		{
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x000418D4 File Offset: 0x0003FAD4
		public SCMapChange(int roleID, int teleportID, int newMapCode, int toNewMapX, int toNewMapY, int toNewDiection, int state)
		{
			this.RoleID = roleID;
			this.TeleportID = teleportID;
			this.NewMapCode = newMapCode;
			this.ToNewMapX = toNewMapX;
			this.ToNewMapY = toNewMapY;
			this.ToNewDiection = toNewDiection;
			this.State = state;
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x00041950 File Offset: 0x0003FB50
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
					this.TeleportID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.NewMapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.ToNewMapX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.ToNewMapY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.ToNewDiection = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.State = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x00041A50 File Offset: 0x0003FC50
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.RoleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.TeleportID, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.NewMapCode, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.ToNewMapX, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.ToNewMapY, true, 5, true, 0);
			total += ProtoUtil.GetIntSize(this.ToNewDiection, true, 6, true, 0);
			total += ProtoUtil.GetIntSize(this.State, true, 7, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.TeleportID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.NewMapCode, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.ToNewMapX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.ToNewMapY, true, 0);
			ProtoUtil.IntMemberToBytes(data, 6, ref offset, this.ToNewDiection, true, 0);
			ProtoUtil.IntMemberToBytes(data, 7, ref offset, this.State, true, 0);
			return data;
		}

		// Token: 0x04000891 RID: 2193
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04000892 RID: 2194
		[ProtoMember(2)]
		public int TeleportID = 0;

		// Token: 0x04000893 RID: 2195
		[ProtoMember(3)]
		public int NewMapCode = 0;

		// Token: 0x04000894 RID: 2196
		[ProtoMember(4)]
		public int ToNewMapX = 0;

		// Token: 0x04000895 RID: 2197
		[ProtoMember(5)]
		public int ToNewMapY = 0;

		// Token: 0x04000896 RID: 2198
		[ProtoMember(6)]
		public int ToNewDiection = 0;

		// Token: 0x04000897 RID: 2199
		[ProtoMember(7)]
		public int State = 0;
	}
}
