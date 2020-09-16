using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class HolyItemPartData
	{
		
		[ProtoMember(1)]
		public sbyte m_sSuit = 0;

		
		[ProtoMember(2)]
		public int m_nSlice = 0;

		
		[ProtoMember(3)]
		public int m_nFailCount = 0;
	}
}
