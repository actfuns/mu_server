using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiMatchScoreData
	{
		
		[ProtoMember(1)]
		public int PlayerNum1;

		
		[ProtoMember(2)]
		public int PlayerNum2;

		
		[ProtoMember(3)]
		public string LT_BHName;

		
		[ProtoMember(4)]
		public int QiZhi1;

		
		[ProtoMember(5)]
		public int QiZhi2;

		
		[ProtoMember(6)]
		public int Score1;

		
		[ProtoMember(7)]
		public int Score2;
	}
}
