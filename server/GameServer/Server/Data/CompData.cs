using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace Server.Data
{
	
	[ProtoContract]
	public class CompData
	{
		
		[ProtoMember(1)]
		public KFCompData kfCompData = new KFCompData();

		
		[ProtoMember(2)]
		public List<int> YestdBoomValueList = new List<int>();

		
		[ProtoMember(3)]
		public CompSelectData SelectData = new CompSelectData();
	}
}
