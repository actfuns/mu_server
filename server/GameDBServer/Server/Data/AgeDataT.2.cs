using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	[Serializable]
	public class AgeDataT
	{
		
		[ProtoMember(1)]
		public long Age;

		
		[ProtoMember(2)]
		public byte[] V;
	}
}
