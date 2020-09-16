using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class JieriXmlData : ICompressed
	{
		
		[ProtoMember(1)]
		public List<string> XmlList = null;

		
		[ProtoMember(2)]
		public int Version;
	}
}
