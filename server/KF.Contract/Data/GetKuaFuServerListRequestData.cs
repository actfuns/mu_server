using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	
	[ProtoContract]
	public class GetKuaFuServerListRequestData
	{
		
		[ProtoMember(1)]
		public int GameType;

		
		[ProtoMember(2)]
		public int ServerListAge;

		
		[ProtoMember(3)]
		public int ServerGameConfigAge;

		
		[ProtoMember(4)]
		public int GameConfigAge;
	}
}
