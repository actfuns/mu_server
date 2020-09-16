using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BangHuiDetailData
	{
		
		[ProtoMember(1)]
		public int BHID = 0;

		
		[ProtoMember(2)]
		public string BHName = "";

		
		[ProtoMember(3)]
		public int ZoneID = 0;

		
		[ProtoMember(4)]
		public int BZRoleID = 0;

		
		[ProtoMember(5)]
		public string BZRoleName = "";

		
		[ProtoMember(6)]
		public int BZOccupation = 0;

		
		[ProtoMember(7)]
		public int TotalNum = 0;

		
		[ProtoMember(8)]
		public int TotalLevel = 0;

		
		[ProtoMember(9)]
		public string BHBulletin = "";

		
		[ProtoMember(10)]
		public string BuildTime = "";

		
		[ProtoMember(11)]
		public string QiName = "";

		
		[ProtoMember(12)]
		public int QiLevel = 0;

		
		[ProtoMember(13)]
		public List<BangHuiMgrItemData> MgrItemList = null;

		
		[ProtoMember(14)]
		public int IsVerify = 0;

		
		[ProtoMember(15)]
		public int TotalMoney = 0;

		
		[ProtoMember(16)]
		public int TodayZhanGongForGold = 0;

		
		[ProtoMember(17)]
		public int TodayZhanGongForDiamond = 0;

		
		[ProtoMember(18)]
		public int JiTan = 0;

		
		[ProtoMember(19)]
		public int JunXie = 0;

		
		[ProtoMember(20)]
		public int GuangHuan = 0;

		
		[ProtoMember(21)]
		public int CanModNameTimes = 0;

		
		[ProtoMember(22)]
		public long TotalCombatForce;
	}
}
