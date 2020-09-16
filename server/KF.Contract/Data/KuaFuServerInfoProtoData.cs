using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	
	[ProtoContract]
	public class KuaFuServerInfoProtoData
	{
		
		[ProtoMember(1)]
		public int ServerId;

		
		[ProtoMember(2)]
		public string Ip;

		
		[ProtoMember(3)]
		public int Port;

		
		[ProtoMember(4)]
		public int DbPort;

		
		[ProtoMember(5)]
		public int LogDbPort;

		
		[ProtoMember(6)]
		public int Flags;
	}
}
