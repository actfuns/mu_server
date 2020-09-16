using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MallSaleData
	{
		
		[ProtoMember(1)]
		public string MallXmlString = "";

		
		[ProtoMember(2)]
		public string MallTabXmlString = "";

		
		[ProtoMember(3)]
		public string QiangGouXmlString = "";
	}
}
