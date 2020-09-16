using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpecPriorityActInfo
	{
		
		[ProtoMember(1)]
		public int TeQuanID = 0;

		
		[ProtoMember(2)]
		public int ActID = 0;

		
		[ProtoMember(3)]
		public int LeftPurNum = 0;

		
		[ProtoMember(4)]
		public int State = 0;

		
		[ProtoMember(5)]
		public int ShowNum = 0;

		
		[ProtoMember(6)]
		public int ShowNum2 = 0;
	}
}
