using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Tmsk.Contract
{
	
	[ProtoContract]
	public class HongBaoListQueryData
	{
		
		[ProtoMember(1)]
		public int BhId;

		
		[ProtoMember(2)]
		public int Success;

		
		[ProtoMember(3)]
		public List<HongBaoSendData> List;

		
		[ProtoMember(4)]
		public string KeyStr;
	}
}
