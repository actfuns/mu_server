using System;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleDamage : IComparable<RoleDamage>
	{
		
		public RoleDamage()
		{
		}

		
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

		
		[ProtoMember(1)]
		public int RoleID;

		
		[ProtoMember(2)]
		public long Damage;

		
		[ProtoMember(3)]
		public string RoleName;

		
		[ProtoMember(4)]
		public List<int> FlagList;
	}
}
