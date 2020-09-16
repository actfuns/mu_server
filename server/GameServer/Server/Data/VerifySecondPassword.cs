using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class VerifySecondPassword
	{
		
		[ProtoMember(1)]
		public string UserID;

		
		[ProtoMember(2)]
		public string SecPwd;
	}
}
