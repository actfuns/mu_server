using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ServerDBInfo
	{
		
		[ProtoMember(1)]
		public string strIP;

		
		[ProtoMember(2)]
		public int Port;

		
		[ProtoMember(3)]
		public string dbName;

		
		[ProtoMember(4)]
		public string userName;

		
		[ProtoMember(5)]
		public string uPassword;

		
		[ProtoMember(6)]
		public int maxConns;

		
		[ProtoMember(7)]
		public string InternalIP;

		
		[ProtoMember(8)]
		public string ChargeKey;

		
		[ProtoMember(9)]
		public string ServerKey;

		
		[ProtoMember(64)]
		public string DbNames;

		
		[ProtoMember(65)]
		public int CodePage;
	}
}
