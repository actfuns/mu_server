using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	internal class JieRiFanBeiItemData
	{
		
		[ProtoMember(1)]
		public int Type;

		
		[ProtoMember(2)]
		public int IsOpen;

		
		[ProtoMember(3)]
		public string ExtArg1;

		
		[ProtoMember(4)]
		public string ExtArg2;
	}
}
