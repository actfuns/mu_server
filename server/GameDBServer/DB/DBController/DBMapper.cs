using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameDBServer.DB.DBController
{
	
	internal class DBMapper
	{
		
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

		
		public MemberInfo getMemberInfo(string columnName)
		{
			MemberInfo member = null;
			this.memberMappings.TryGetValue(columnName, out member);
			return member;
		}

		
		private Dictionary<string, MemberInfo> memberMappings = new Dictionary<string, MemberInfo>();
	}
}
