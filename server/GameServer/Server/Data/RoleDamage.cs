using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000586 RID: 1414
	[ProtoContract]
	public class RoleDamage : IComparable<RoleDamage>
	{
		// Token: 0x06001A07 RID: 6663 RVA: 0x00191CB7 File Offset: 0x0018FEB7
		public RoleDamage()
		{
		}

		// Token: 0x06001A08 RID: 6664 RVA: 0x00191CC4 File Offset: 0x0018FEC4
		public RoleDamage(int roleID, long damage, string roleName = null, params int[] param)
		{
			this.RoleID = roleID;
			this.Damage = damage;
			this.RoleName = roleName;
			if (param != null && param.Length > 0)
			{
				this.FlagList = param.ToList<int>();
			}
		}

		// Token: 0x06001A09 RID: 6665 RVA: 0x00191D14 File Offset: 0x0018FF14
		public int CompareTo(RoleDamage right)
		{
			long ret = this.Damage - right.Damage;
			int result;
			if (ret > 0L)
			{
				result = 1;
			}
			else if (ret == 0L)
			{
				result = 0;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x04002633 RID: 9779
		[ProtoMember(1)]
		public int RoleID;

		// Token: 0x04002634 RID: 9780
		[ProtoMember(2)]
		public long Damage;

		// Token: 0x04002635 RID: 9781
		[ProtoMember(3)]
		public string RoleName;

		// Token: 0x04002636 RID: 9782
		[ProtoMember(4)]
		public List<int> FlagList;
	}
}
