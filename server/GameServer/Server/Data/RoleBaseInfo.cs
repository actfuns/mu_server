using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000016 RID: 22
	[ProtoContract]
	public class RoleBaseInfo
	{
		// Token: 0x04000083 RID: 131
		[ProtoMember(1)]
		public string RoleName;

		// Token: 0x04000084 RID: 132
		[ProtoMember(2)]
		public string UserID;

		// Token: 0x04000085 RID: 133
		[ProtoMember(3)]
		public string Position;

		// Token: 0x04000086 RID: 134
		[ProtoMember(4)]
		public string Feeling;

		// Token: 0x04000087 RID: 135
		[ProtoMember(5)]
		public int RoleID;

		// Token: 0x04000088 RID: 136
		[ProtoMember(6)]
		public int Level;

		// Token: 0x04000089 RID: 137
		[ProtoMember(7)]
		public int CombatForce;

		// Token: 0x0400008A RID: 138
		[ProtoMember(8)]
		public int Admiredcount;

		// Token: 0x0400008B RID: 139
		[ProtoMember(9)]
		public long LoginTime;

		// Token: 0x0400008C RID: 140
		[ProtoMember(10)]
		public long LogoutTime;

		// Token: 0x0400008D RID: 141
		[ProtoMember(11)]
		public short ZoneID;

		// Token: 0x0400008E RID: 142
		[ProtoMember(12)]
		public byte ChangeLifeCount;

		// Token: 0x0400008F RID: 143
		[ProtoMember(13)]
		public byte Occupation;

		// Token: 0x04000090 RID: 144
		[ProtoMember(14)]
		public byte Sex;

		// Token: 0x04000091 RID: 145
		[ProtoMember(15)]
		public int RealMoney;
	}
}
