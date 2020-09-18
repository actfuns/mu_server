using System;
using ProtoBuf;

namespace GameServer.Logic.BocaiSys
{
	
	[ProtoContract]
	public class BuyBoCai2SDB
	{
		
		[ProtoMember(1)]
		public int m_RoleID;

		
		[ProtoMember(2)]
		public string m_RoleName;

		
		[ProtoMember(3)]
		public int ZoneID;

		
		[ProtoMember(4)]
		public string strUserID;

		
		[ProtoMember(5)]
		public int ServerId;

		
		[ProtoMember(6)]
		public int BuyNum;

		
		[ProtoMember(7)]
		public string strBuyValue;

		
		[ProtoMember(8)]
		public bool IsSend;

		
		[ProtoMember(9)]
		public bool IsWin;

		
		[ProtoMember(10)]
		public int BocaiType;

		
		[ProtoMember(11)]
		public long DataPeriods;
	}
}
