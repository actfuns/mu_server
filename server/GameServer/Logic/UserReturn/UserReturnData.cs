using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.UserReturn
{
	
	[ProtoContract]
	public class UserReturnData
	{
		
		[ProtoMember(1)]
		public bool ActivityIsOpen = false;

		
		[ProtoMember(2)]
		public int ActivityID = 0;

		
		[ProtoMember(3)]
		public string ActivityDay = "";

		
		[ProtoMember(4)]
		public DateTime TimeBegin = DateTime.MinValue;

		
		[ProtoMember(5)]
		public DateTime TimeEnd = DateTime.MinValue;

		
		[ProtoMember(6)]
		public DateTime TimeAward = DateTime.MinValue;

		
		[ProtoMember(7)]
		public string RecallCode = "0";

		
		[ProtoMember(8)]
		public int RecallZoneID = 0;

		
		[ProtoMember(9)]
		public int RecallRoleID = 0;

		
		[ProtoMember(10)]
		public int Level = 0;

		
		[ProtoMember(11)]
		public int Vip = 0;

		
		[ProtoMember(12)]
		public DateTime TimeReturn = DateTime.MinValue;

		
		[ProtoMember(13)]
		public int ReturnState = 0;

		
		[ProtoMember(14)]
		public Dictionary<int, int[]> AwardDic = new Dictionary<int, int[]>();

		
		[ProtoMember(15)]
		public DateTime TimeWait = DateTime.MinValue;

		
		[ProtoMember(16)]
		public int ZhuanSheng = 0;

		
		[ProtoMember(17)]
		public int DengJi = 0;

		
		[ProtoMember(18)]
		public string MyCode = "";

		
		[ProtoMember(19)]
		public int LeiJiChongZhi = 0;
	}
}
