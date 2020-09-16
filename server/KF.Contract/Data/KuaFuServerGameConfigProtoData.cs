using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	
	[ProtoContract]
	public class KuaFuServerGameConfigProtoData
	{
		
		[ProtoMember(1)]
		public int ServerId;

		
		[ProtoMember(2)]
		public int GameType;

		
		[ProtoMember(3)]
		public int Capacity;
	}
}
