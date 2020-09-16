using System;
using System.Collections.Generic;
using ProtoBuf;

namespace KF.Contract.Data
{
	
	[ProtoContract]
	public class GetKuaFuServerListResponseData
	{
		
		[ProtoMember(1)]
		public int GameType;

		
		[ProtoMember(2)]
		public int ServerListAge;

		
		[ProtoMember(3)]
		public int ServerGameConfigAge;

		
		[ProtoMember(4)]
		public int GameConfigAge;

		
		[ProtoMember(5)]
		public List<KuaFuServerInfoProtoData> ServerList;

		
		[ProtoMember(6)]
		public List<KuaFuServerGameConfigProtoData> ServerGameConfigList;
	}
}
