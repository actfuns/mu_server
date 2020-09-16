using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class JieriCZQGData
	{
		
		[ProtoMember(1)]
		public int ID;

		
		[ProtoMember(2)]
		public int PurchaseNum;
	}
}
