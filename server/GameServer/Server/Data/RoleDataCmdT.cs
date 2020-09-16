using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleDataCmdT<T>
	{
		
		public RoleDataCmdT()
		{
		}

		
		public RoleDataCmdT(int roleId, T v)
		{
			this.RoleID = roleId;
			this.Value = v;
		}

		
		[ProtoMember(1)]
		public int RoleID;

		
		[ProtoMember(2, IsRequired = true)]
		public T Value;
	}
}
