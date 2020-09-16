using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ZhengBaPkLogData
	{
		
		[ProtoMember(1)]
		public int Day;

		
		[ProtoMember(2)]
		public int RoleID1;

		
		[ProtoMember(3)]
		public int ZoneID1;

		
		[ProtoMember(4)]
		public string RoleName1;

		
		[ProtoMember(5)]
		public int RoleID2;

		
		[ProtoMember(6)]
		public int ZoneID2;

		
		[ProtoMember(7)]
		public string RoleName2;

		
		[ProtoMember(8)]
		public int PkResult;

		
		[ProtoMember(9)]
		public bool UpGrade;

		
		[ProtoMember(10)]
		public int Month;

		
		[ProtoMember(11)]
		public bool IsMirror1;

		
		[ProtoMember(12)]
		public bool IsMirror2;

		
		[ProtoMember(13)]
		public DateTime StartTime;

		
		[ProtoMember(14)]
		public DateTime EndTime;
	}
}
