using System;
using ProtoBuf;

namespace Tmsk.Contract
{
	
	[ProtoContract]
	public class HongBaoRecvData
	{
		
		[ProtoMember(1)]
		public int HongBaoID;

		
		[ProtoMember(2)]
		public int RoleId;

		
		[ProtoMember(3)]
		public string RoleName;

		
		[ProtoMember(4)]
		public int ZuanShi;

		
		[ProtoMember(5)]
		public DateTime RecvTime;

		
		[ProtoMember(6)]
		public int ZuiJia;

		
		[ProtoMember(7)]
		public int BhId;
	}
}
