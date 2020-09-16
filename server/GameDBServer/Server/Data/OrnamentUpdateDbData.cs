using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class OrnamentUpdateDbData
	{
		
		[ProtoMember(1)]
		public int RoleId;

		
		[ProtoMember(2)]
		public OrnamentData Data;
	}
}
