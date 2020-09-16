using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.Spread
{
	
	[ProtoContract]
	public class SpreadData
	{
		
		[ProtoMember(1)]
		public bool IsOpen = false;

		
		[ProtoMember(2)]
		public string SpreadCode = "";

		
		[ProtoMember(3)]
		public string VerifyCode = "";

		
		[ProtoMember(4)]
		public int CountRole = 0;

		
		[ProtoMember(5)]
		public int CountVip = 0;

		
		[ProtoMember(6)]
		public int CountLevel = 0;

		
		[ProtoMember(7)]
		public int State = 0;

		
		[ProtoMember(8)]
		public Dictionary<int, string> AwardDic = new Dictionary<int, string>();

		
		[ProtoMember(9)]
		public Dictionary<int, int> AwardCountDic = new Dictionary<int, int>();
	}
}
