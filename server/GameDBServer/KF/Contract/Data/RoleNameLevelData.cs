using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	
	[ProtoContract]
	public class RoleNameLevelData
	{
		
		public RoleNameLevelData()
		{
		}

		
		public RoleNameLevelData(int zhuanSheng, int level, string roleName, bool zhiWu, int occupation)
		{
			this.ZhuanSheng = zhuanSheng;
			this.Level = level;
			this.RoleName = roleName;
			this.ZhiWu = zhiWu;
			this.Occupation = occupation;
		}

		
		[ProtoMember(1)]
		public int ZhuanSheng;

		
		[ProtoMember(2)]
		public int Level;

		
		[ProtoMember(3)]
		public string RoleName;

		
		[ProtoMember(4)]
		public bool ZhiWu;

		
		[ProtoMember(5)]
		public int Occupation;
	}
}
