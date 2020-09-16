using System;
using ProtoBuf;

namespace CheckSysValueDll
{
	
	[ProtoContract]
	public class SeachData
	{
		
		[ProtoMember(1)]
		public string AttName;

		
		[ProtoMember(2)]
		public string SeachVal;
	}
}
