using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PushMessageData
	{
		
		[ProtoMember(1)]
		public string UserID = "";

		
		[ProtoMember(2)]
		public string PushID = "";

		
		[ProtoMember(3)]
		public string LastLoginTime = "";
	}
}
