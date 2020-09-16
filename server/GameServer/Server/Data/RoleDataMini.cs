using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class RoleDataMini
	{
		
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
		public int PKMode = 0;

		
		[ProtoMember(8)]
		public int PKValue = 0;

		
		[ProtoMember(9)]
		public int MapCode = 0;

		
		[ProtoMember(10)]
		public int PosX = 0;

		
		[ProtoMember(11)]
		public int PosY = 0;

		
		[ProtoMember(12)]
		public int RoleDirection = 0;

		
		[ProtoMember(13)]
		public int LifeV = 0;

		
		[ProtoMember(14)]
		public int MaxLifeV = 0;

		
		[ProtoMember(15)]
		public int MagicV = 0;

		
		[ProtoMember(16)]
		public int MaxMagicV = 0;

		
		[ProtoMember(17)]
		public int BodyCode;

		
		[ProtoMember(18)]
		public int WeaponCode;

		
		[ProtoMember(19)]
		public string OtherName;

		
		[ProtoMember(20)]
		public int TeamID;

		
		[ProtoMember(21)]
		public int TeamLeaderRoleID = 0;

		
		[ProtoMember(22)]
		public int PKPoint = 0;

		
		[ProtoMember(23)]
		public long StartPurpleNameTicks = 0L;

		
		[ProtoMember(24)]
		public long BattleNameStart = 0L;

		
		[ProtoMember(25)]
		public int BattleNameIndex = 0;

		
		[ProtoMember(26)]
		public int ZoneID = 0;

		
		[ProtoMember(27)]
		public string BHName = "";

		
		[ProtoMember(28)]
		public int BHVerify = 0;

		
		[ProtoMember(29)]
		public int BHZhiWu = 0;

		
		[ProtoMember(30)]
		public long FSHuDunStart = 0L;

		
		[ProtoMember(31)]
		public int BattleWhichSide = -1;

		
		[ProtoMember(32)]
		public int IsVIP = 0;

		
		[ProtoMember(33)]
		public long DSHideStart = 0L;

		
		[ProtoMember(34)]
		public List<int> RoleCommonUseIntPamams = new List<int>();

		
		[ProtoMember(35)]
		public int FSHuDunSeconds = 0;

		
		[ProtoMember(36)]
		public long ZhongDuStart = 0L;

		
		[ProtoMember(37)]
		public int ZhongDuSeconds = 0;

		
		[ProtoMember(38)]
		public int JieriChengHao = 0;

		
		[ProtoMember(39)]
		public long DongJieStart = 0L;

		
		[ProtoMember(40)]
		public int DongJieSeconds = 0;

		
		[ProtoMember(41)]
		public List<GoodsData> GoodsDataList;

		
		[ProtoMember(42)]
		public int ChangeLifeLev;

		
		[ProtoMember(43)]
		public int ChangeLifeCount = 0;

		
		[ProtoMember(44)]
		public string StallName;

		
		[ProtoMember(45)]
		public List<BufferDataMini> BufferMiniInfo;

		
		[ProtoMember(46)]
		public WingData MyWingData = null;

		
		[ProtoMember(47)]
		public int VIPLevel = 0;

		
		[ProtoMember(48)]
		public int GMAuth = 0;

		
		[ProtoMember(49)]
		public long SettingBitFlags;

		
		[ProtoMember(50)]
		public int SpouseId;

		
		[ProtoMember(51)]
		public List<int> OccupationList;

		
		[ProtoMember(52)]
		public int JunTuanId;

		
		[ProtoMember(53)]
		public string JunTuanName;

		
		[ProtoMember(54)]
		public int JunTuanZhiWu;

		
		[ProtoMember(55)]
		public int LingDi;

		
		[ProtoMember(56)]
		public RoleHuiJiData HuiJiData;

		
		[ProtoMember(57)]
		public SkillEquipData ShenShiEquipData;

		
		[ProtoMember(58)]
		public List<int> PassiveEffectList;

		
		[ProtoMember(59)]
		public JueXingShiData JueXingData;

		
		[ProtoMember(60)]
		public int CompType;

		
		[ProtoMember(61)]
		public byte CompZhiWu;

		
		[ProtoMember(62)]
		public GoodsData EquipMount;

		
		[ProtoMember(63)]
		public int SubOccupation;

		
		[ProtoMember(64)]
		public int CurrentArmorV;

		
		[ProtoMember(65)]
		public int MaxArmorV;

		
		[ProtoMember(66)]
		public ZuoQiMainData ZuoQiMainData;

		
		[ProtoMember(67)]
		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData;

		
		[ProtoMember(68)]
		public int RebornCount = 0;

		
		[ProtoMember(69)]
		public int RebornLevel = 0;

		
		[ProtoMember(70)]
		public int RebornEquipShow = 0;

		
		[ProtoMember(71)]
		public List<GoodsData> RebornGoodsDataList;

		
		[ProtoMember(72)]
		public RebornStampData RebornYinJi;

		
		[ProtoMember(73)]
		public int RebornModelShow = 0;

		
		[ProtoMember(250)]
		public int UserPTID;

		
		[ProtoMember(251)]
		public string WorldRoleID;

		
		[ProtoMember(252)]
		public string Channel;
	}
}
