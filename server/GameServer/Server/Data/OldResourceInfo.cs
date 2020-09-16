using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class OldResourceInfo
	{
		
		[ProtoMember(1)]
		public int type = 1;

		
		[ProtoMember(2)]
		public int exp = 0;

		
		[ProtoMember(3)]
		public int bandmoney = 0;

		
		[ProtoMember(4)]
		public int mojing = 0;

		
		[ProtoMember(5)]
		public int chengjiu = 0;

		
		[ProtoMember(6)]
		public int shengwang = 0;

		
		[ProtoMember(7)]
		public int zhangong = 0;

		
		[ProtoMember(8)]
		public int leftCount = 0;

		
		[ProtoMember(9)]
		public int roleId;

		
		[ProtoMember(10)]
		public int bandDiamond = 0;

		
		[ProtoMember(11)]
		public int xinghun = 0;

		
		[ProtoMember(12)]
		public int yuanSuFenMo = 0;

		
		[ProtoMember(13)]
		public int HasGetOffsetDay;
	}
}
