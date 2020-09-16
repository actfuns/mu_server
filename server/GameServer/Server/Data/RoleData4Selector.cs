using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleData4Selector
	{
		
		public RoleData4Selector Clone()
		{
			return base.MemberwiseClone() as RoleData4Selector;
		}

		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public string RoleName = "";

		
		[ProtoMember(3)]
		public int RoleSex = 0;

		
		[ProtoMember(4)]
		public int Occupation = 0;

		
		[ProtoMember(5)]
		public int Level = 1;

		
		[ProtoMember(6)]
		public int Faction = 0;

		
		[ProtoMember(7)]
		public string OtherName = "";

		
		[ProtoMember(8)]
		public List<GoodsData> GoodsDataList = null;

		
		[ProtoMember(9)]
		public WingData MyWingData = null;

		
		[ProtoMember(10)]
		public int CombatForce = 0;

		
		[ProtoMember(11)]
		public int AdmiredCount = 0;

		
		[ProtoMember(12)]
		public int FashionWingsID = 0;

		
		[ProtoMember(13)]
		public long SettingBitFlags;

		
		[ProtoMember(14)]
		public int ZoneId;

		
		[ProtoMember(15)]
		public List<int> OccupationList;

		
		[ProtoMember(16)]
		public int JunTuanId;

		
		[ProtoMember(17)]
		public string JunTuanName;

		
		[ProtoMember(18)]
		public int JunTuanZhiWu;

		
		[ProtoMember(19)]
		public int LingDi;

		
		[ProtoMember(20)]
		public int BuffFashionID;

		
		[ProtoMember(21)]
		public RoleHuiJiData HuiJiData;

		
		[ProtoMember(22)]
		public int CompType;

		
		[ProtoMember(23)]
		public byte CompZhiWu;

		
		[ProtoMember(24)]
		public int SubOccupation;
	}
}
