using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class KarenNotifyMsg
	{
		
		[ProtoMember(1)]
		public int index;

		
		[ProtoMember(2)]
		public int LegionID;

		
		[ProtoMember(3)]
		public string param1 = "";

		
		[ProtoMember(4)]
		public string param2 = "";
	}
}
