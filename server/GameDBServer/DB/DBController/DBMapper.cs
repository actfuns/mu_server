using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameDBServer.DB.DBController
{
	// Token: 0x020000E2 RID: 226
	internal class DBMapper
	{
		// Token: 0x060001E4 RID: 484 RVA: 0x0000A534 File Offset: 0x00008734
		public DBMapper(Type type)
		{
			MemberInfo[] members = type.GetMembers();
			foreach (MemberInfo member in members)
			{
				if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
				{
					object[] attributes = member.GetCustomAttributes(typeof(DBMappingAttribute), false);
					if (null != attributes)
					{
						DBMappingAttribute[] mappingAttrs = (DBMappingAttribute[])attributes;
						foreach (DBMappingAttribute mappingAttr in mappingAttrs)
						{
							if (mappingAttr.ColumnName != null && !"".Equals(mappingAttr.ColumnName))
							{
								this.memberMappings.Add(mappingAttr.ColumnName, member);
							}
						}
					}
				}
			}
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000A634 File Offset: 0x00008834
		public MemberInfo getMemberInfo(string columnName)
		{
			MemberInfo member = null;
			this.memberMappings.TryGetValue(columnName, out member);
			return member;
		}

		// Token: 0x0400061D RID: 1565
		private Dictionary<string, MemberInfo> memberMappings = new Dictionary<string, MemberInfo>();
	}
}
