using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SetSecondPassword
	{
		
		[ProtoMember(1)]
		public int RoleID;

		
		[ProtoMember(2)]
		public string OldSecPwd;

		
		[ProtoMember(3)]
		public string NewSecPwd;
	}
}
