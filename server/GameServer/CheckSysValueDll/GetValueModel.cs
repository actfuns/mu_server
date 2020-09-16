using System;
using System.Collections.Generic;
using ProtoBuf;

namespace CheckSysValueDll
{
	
	[ProtoContract]
	public class GetValueModel
	{
		
		[ProtoMember(1)]
		public string TypeName;

		
		[ProtoMember(2)]
		public string SeachName;

		
		[ProtoMember(3)]
		public List<SeachData> SeachDataList;
	}
}
