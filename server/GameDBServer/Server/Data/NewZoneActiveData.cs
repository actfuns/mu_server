using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class NewZoneActiveData
	{
		
		[ProtoMember(1)]
		public int YuanBao;

		
		[ProtoMember(2)]
		public int ActiveId;

		
		[ProtoMember(3)]
		public int GetState;

		
		[ProtoMember(4)]
		public List<PaiHangItemData> Ranklist;
	}
}
