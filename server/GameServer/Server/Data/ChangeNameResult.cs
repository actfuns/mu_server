using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ChangeNameResult
	{
		
		[ProtoMember(1)]
		public int ErrCode;

		
		[ProtoMember(2)]
		public int ZoneId;

		
		[ProtoMember(3)]
		public string NewName;

		
		[ProtoMember(4)]
		public ChangeNameInfo NameInfo = new ChangeNameInfo();
	}
}
