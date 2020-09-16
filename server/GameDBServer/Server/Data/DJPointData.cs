using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class DJPointData
	{
		
		[ProtoMember(1)]
		public int DbID = 0;

		
		[ProtoMember(2)]
		public int RoleID = 0;

		
		[ProtoMember(3)]
		public int DJPoint = 0;

		
		[ProtoMember(4)]
		public int Total = 0;

		
		[ProtoMember(5)]
		public int Wincnt = 0;

		
		[ProtoMember(6)]
		public int Yestoday = 0;

		
		[ProtoMember(7)]
		public int Lastweek = 0;

		
		[ProtoMember(8)]
		public int Lastmonth = 0;

		
		[ProtoMember(9)]
		public int Dayupdown = 0;

		
		[ProtoMember(10)]
		public int Weekupdown = 0;

		
		[ProtoMember(11)]
		public int Monthupdown = 0;

		
		[ProtoMember(12)]
		public DJRoleInfoData djRoleInfoData = null;
	}
}
