using System;
using System.Collections.Generic;
using ProtoBuf;

namespace CheckSysValueDll
{
	
	[ProtoContract]
	public class CheckValueResultItem
	{
		
		[ProtoMember(1)]
		public string TypeName;

		
		[ProtoMember(2)]
		public object StrValue;

		
		[ProtoMember(3)]
		public List<string> Childs;
	}
}
