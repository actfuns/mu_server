using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BufferData
	{
		
		[ProtoMember(1)]
		public int BufferID = 0;

		
		[ProtoMember(2)]
		public long StartTime = 0L;

		
		[ProtoMember(3)]
		public int BufferSecs = 0;

		
		[ProtoMember(4)]
		public long BufferVal = 0L;

		
		[ProtoMember(5)]
		public int BufferType = 0;
	}
}
