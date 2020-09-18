using System;
using ProtoBuf;

namespace GameDBServer.Logic.UserReturn
{
	
	[ProtoContract]
	public class ReturnData
	{
		
		[ProtoMember(1)]
		public int DBID = 0;

		
		[ProtoMember(2)]
		public int ActivityID = 0;

		
		[ProtoMember(3)]
		public string ActivityDay = "";

		
		[ProtoMember(4)]
		public int PZoneID = 0;

		
		[ProtoMember(5)]
		public int PRoleID = 0;

		
		[ProtoMember(6)]
		public int CZoneID = 0;

		
		[ProtoMember(7)]
		public int CRoleID = 0;

		
		[ProtoMember(8)]
		public int Vip = 0;

		
		[ProtoMember(9)]
		public int Level = 0;

		
		[ProtoMember(10)]
		public DateTime LogTime = DateTime.MinValue;

		
		[ProtoMember(11)]
		public int StateCheck = 0;

		
		[ProtoMember(12)]
		public int StateLog = 0;

		
		[ProtoMember(13)]
		public int LeiJiChongZhi = 0;
	}
}
