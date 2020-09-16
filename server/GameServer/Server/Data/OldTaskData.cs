using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class OldTaskData
	{
		
		[ProtoMember(1)]
		public int TaskID;

		
		[ProtoMember(2)]
		public int DoCount;
	}
}
