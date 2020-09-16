using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ShengXiaoGuessHistory
	{
		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public string RoleName = "";

		
		[ProtoMember(3)]
		public int GuessKey = 0;

		
		[ProtoMember(4)]
		public int Mortgage = 0;

		
		[ProtoMember(5)]
		public int ResultKey = 0;

		
		[ProtoMember(6)]
		public int GainNum = 0;

		
		[ProtoMember(7)]
		public int LeftMortgage = 0;

		
		[ProtoMember(8)]
		public string GuessTime = "";
	}
}
