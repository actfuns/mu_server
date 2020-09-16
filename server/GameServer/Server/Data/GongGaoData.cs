using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GongGaoData
	{
		
		[ProtoMember(1)]
		public int nHaveGongGao = 0;

		
		[ProtoMember(2)]
		public int nLianXuLoginReward = 0;

		
		[ProtoMember(3)]
		public int nLeiJiLoginReward = 0;

		
		[ProtoMember(4)]
		public string strGongGaoInfo = "";
	}
}
