using System;
using System.Collections.Generic;
using GameServer.Logic.Olympics;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000587 RID: 1415
	[ProtoContract]
	public class RoleData
	{
		// Token: 0x04002637 RID: 9783
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x04002638 RID: 9784
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x04002639 RID: 9785
		[ProtoMember(3)]
		public int RoleSex = 0;

		// Token: 0x0400263A RID: 9786
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x0400263B RID: 9787
		[ProtoMember(5)]
		public int Level = 1;

		// Token: 0x0400263C RID: 9788
		[ProtoMember(6)]
		public int Faction = 0;

		// Token: 0x0400263D RID: 9789
		[ProtoMember(7)]
		public int Money1 = 0;

		// Token: 0x0400263E RID: 9790
		[ProtoMember(8)]
		public int Money2 = 0;

		// Token: 0x0400263F RID: 9791
		[ProtoMember(9)]
		public long Experience = 0L;

		// Token: 0x04002640 RID: 9792
		[ProtoMember(10)]
		public int PKMode = 0;

		// Token: 0x04002641 RID: 9793
		[ProtoMember(11)]
		public int PKValue = 0;

		// Token: 0x04002642 RID: 9794
		[ProtoMember(12)]
		public int MapCode = 0;

		// Token: 0x04002643 RID: 9795
		[ProtoMember(13)]
		public int PosX = 0;

		// Token: 0x04002644 RID: 9796
		[ProtoMember(14)]
		public int PosY = 0;

		// Token: 0x04002645 RID: 9797
		[ProtoMember(15)]
		public int RoleDirection = 0;

		// Token: 0x04002646 RID: 9798
		[ProtoMember(16)]
		public int LifeV = 0;

		// Token: 0x04002647 RID: 9799
		[ProtoMember(17)]
		public int MaxLifeV = 0;

		// Token: 0x04002648 RID: 9800
		[ProtoMember(18)]
		public int MagicV = 0;

		// Token: 0x04002649 RID: 9801
		[ProtoMember(19)]
		public int MaxMagicV = 0;

		// Token: 0x0400264A RID: 9802
		[ProtoMember(20)]
		public int RolePic = 0;

		// Token: 0x0400264B RID: 9803
		[ProtoMember(21)]
		public int BagNum = 0;

		// Token: 0x0400264C RID: 9804
		[ProtoMember(22)]
		public List<TaskData> TaskDataList;

		// Token: 0x0400264D RID: 9805
		[ProtoMember(23)]
		public List<GoodsData> GoodsDataList;

		// Token: 0x0400264E RID: 9806
		[ProtoMember(24)]
		public int BodyCode;

		// Token: 0x0400264F RID: 9807
		[ProtoMember(25)]
		public int WeaponCode;

		// Token: 0x04002650 RID: 9808
		[ProtoMember(26)]
		public List<SkillData> SkillDataList;

		// Token: 0x04002651 RID: 9809
		[ProtoMember(27)]
		public string OtherName;

		// Token: 0x04002652 RID: 9810
		[ProtoMember(28)]
		public List<NPCTaskState> NPCTaskStateList;

		// Token: 0x04002653 RID: 9811
		[ProtoMember(29)]
		public string MainQuickBarKeys = "";

		// Token: 0x04002654 RID: 9812
		[ProtoMember(30)]
		public string OtherQuickBarKeys = "";

		// Token: 0x04002655 RID: 9813
		[ProtoMember(31)]
		public int LoginNum = 0;

		// Token: 0x04002656 RID: 9814
		[ProtoMember(32)]
		public int UserMoney = 0;

		// Token: 0x04002657 RID: 9815
		[ProtoMember(33)]
		public string StallName;

		// Token: 0x04002658 RID: 9816
		[ProtoMember(34)]
		public int TeamID;

		// Token: 0x04002659 RID: 9817
		[ProtoMember(35)]
		public int LeftFightSeconds = 0;

		// Token: 0x0400265A RID: 9818
		[ProtoMember(36)]
		public int TotalHorseCount = 0;

		// Token: 0x0400265B RID: 9819
		[ProtoMember(37)]
		public int HorseDbID = -1;

		// Token: 0x0400265C RID: 9820
		[ProtoMember(38)]
		public int TotalPetCount = 0;

		// Token: 0x0400265D RID: 9821
		[ProtoMember(39)]
		public int PetDbID = -1;

		// Token: 0x0400265E RID: 9822
		[ProtoMember(40)]
		public int InterPower = 0;

		// Token: 0x0400265F RID: 9823
		[ProtoMember(41)]
		public int TeamLeaderRoleID = 0;

		// Token: 0x04002660 RID: 9824
		[ProtoMember(42)]
		public int YinLiang = 0;

		// Token: 0x04002661 RID: 9825
		[ProtoMember(43)]
		public int JingMaiBodyLevel = 0;

		// Token: 0x04002662 RID: 9826
		[ProtoMember(44)]
		public int JingMaiXueWeiNum = 0;

		// Token: 0x04002663 RID: 9827
		[ProtoMember(45)]
		public int LastHorseID = 0;

		// Token: 0x04002664 RID: 9828
		[ProtoMember(46)]
		public int DefaultSkillID = -1;

		// Token: 0x04002665 RID: 9829
		[ProtoMember(47)]
		public int AutoLifeV = 0;

		// Token: 0x04002666 RID: 9830
		[ProtoMember(48)]
		public int AutoMagicV = 0;

		// Token: 0x04002667 RID: 9831
		[ProtoMember(49)]
		public List<BufferData> BufferDataList = null;

		// Token: 0x04002668 RID: 9832
		[ProtoMember(50)]
		public List<DailyTaskData> MyDailyTaskDataList = null;

		// Token: 0x04002669 RID: 9833
		[ProtoMember(51)]
		public int JingMaiOkNum = 0;

		// Token: 0x0400266A RID: 9834
		[ProtoMember(52)]
		public DailyJingMaiData MyDailyJingMaiData = null;

		// Token: 0x0400266B RID: 9835
		[ProtoMember(53)]
		public int NumSkillID = 0;

		// Token: 0x0400266C RID: 9836
		[ProtoMember(54)]
		public PortableBagData MyPortableBagData = null;

		// Token: 0x0400266D RID: 9837
		[ProtoMember(55)]
		public int NewStep = 0;

		// Token: 0x0400266E RID: 9838
		[ProtoMember(56)]
		public long StepTime = 0L;

		// Token: 0x0400266F RID: 9839
		[ProtoMember(57)]
		public int BigAwardID = 0;

		// Token: 0x04002670 RID: 9840
		[ProtoMember(58)]
		public int SongLiID = 0;

		// Token: 0x04002671 RID: 9841
		[ProtoMember(59)]
		public List<FuBenData> FuBenDataList = null;

		// Token: 0x04002672 RID: 9842
		[ProtoMember(60)]
		public int TotalLearnedSkillLevelCount = 0;

		// Token: 0x04002673 RID: 9843
		[ProtoMember(61)]
		public int CompletedMainTaskID = 0;

		// Token: 0x04002674 RID: 9844
		[ProtoMember(62)]
		public int PKPoint = 0;

		// Token: 0x04002675 RID: 9845
		[ProtoMember(63)]
		public int LianZhan = 0;

		// Token: 0x04002676 RID: 9846
		[ProtoMember(64)]
		public long StartPurpleNameTicks = 0L;

		// Token: 0x04002677 RID: 9847
		[ProtoMember(65)]
		public YaBiaoData MyYaBiaoData = null;

		// Token: 0x04002678 RID: 9848
		[ProtoMember(66)]
		public long BattleNameStart = 0L;

		// Token: 0x04002679 RID: 9849
		[ProtoMember(67)]
		public int BattleNameIndex = 0;

		// Token: 0x0400267A RID: 9850
		[ProtoMember(68)]
		public int CZTaskID = 0;

		// Token: 0x0400267B RID: 9851
		[ProtoMember(69)]
		public int HeroIndex = 0;

		// Token: 0x0400267C RID: 9852
		[ProtoMember(70)]
		public int AllQualityIndex = 0;

		// Token: 0x0400267D RID: 9853
		[ProtoMember(71)]
		public int AllForgeLevelIndex = 0;

		// Token: 0x0400267E RID: 9854
		[ProtoMember(72)]
		public int AllJewelLevelIndex = 0;

		// Token: 0x0400267F RID: 9855
		[ProtoMember(73)]
		public int HalfYinLiangPeriod = 0;

		// Token: 0x04002680 RID: 9856
		[ProtoMember(74)]
		public int ZoneID = 0;

		// Token: 0x04002681 RID: 9857
		[ProtoMember(75)]
		public string BHName = "";

		// Token: 0x04002682 RID: 9858
		[ProtoMember(76)]
		public int BHVerify = 0;

		// Token: 0x04002683 RID: 9859
		[ProtoMember(77)]
		public int BHZhiWu = 0;

		// Token: 0x04002684 RID: 9860
		[ProtoMember(78)]
		public int BangGong = 0;

		// Token: 0x04002685 RID: 9861
		[ProtoMember(79)]
		public Dictionary<int, BangHuiLingDiItemData> BangHuiLingDiItemsDict = null;

		// Token: 0x04002686 RID: 9862
		[ProtoMember(80)]
		public int HuangDiRoleID = 0;

		// Token: 0x04002687 RID: 9863
		[ProtoMember(81)]
		public int HuangHou = 0;

		// Token: 0x04002688 RID: 9864
		[ProtoMember(82)]
		public Dictionary<int, int> PaiHangPosDict = null;

		// Token: 0x04002689 RID: 9865
		[ProtoMember(83)]
		public int AutoFightingProtect = 0;

		// Token: 0x0400268A RID: 9866
		[ProtoMember(84)]
		public long FSHuDunStart = 0L;

		// Token: 0x0400268B RID: 9867
		[ProtoMember(85)]
		public int BattleWhichSide = -1;

		// Token: 0x0400268C RID: 9868
		[ProtoMember(86)]
		public int LastMailID = 0;

		// Token: 0x0400268D RID: 9869
		[ProtoMember(87)]
		public int IsVIP = 0;

		// Token: 0x0400268E RID: 9870
		[ProtoMember(88)]
		public long OnceAwardFlag = 0L;

		// Token: 0x0400268F RID: 9871
		[ProtoMember(89)]
		public int Gold = 0;

		// Token: 0x04002690 RID: 9872
		[ProtoMember(90)]
		public long DSHideStart = 0L;

		// Token: 0x04002691 RID: 9873
		[ProtoMember(91)]
		public List<int> RoleCommonUseIntPamams = new List<int>();

		// Token: 0x04002692 RID: 9874
		[ProtoMember(92)]
		public int FSHuDunSeconds = 0;

		// Token: 0x04002693 RID: 9875
		[ProtoMember(93)]
		public long ZhongDuStart = 0L;

		// Token: 0x04002694 RID: 9876
		[ProtoMember(94)]
		public int ZhongDuSeconds = 0;

		// Token: 0x04002695 RID: 9877
		[ProtoMember(95)]
		public string KaiFuStartDay = "";

		// Token: 0x04002696 RID: 9878
		[ProtoMember(96)]
		public string RegTime = "";

		// Token: 0x04002697 RID: 9879
		[ProtoMember(97)]
		public string JieriStartDay = "";

		// Token: 0x04002698 RID: 9880
		[ProtoMember(98)]
		public int JieriDaysNum = 0;

		// Token: 0x04002699 RID: 9881
		[ProtoMember(99)]
		public string HefuStartDay = "";

		// Token: 0x0400269A RID: 9882
		[ProtoMember(100)]
		public int JieriChengHao = 0;

		// Token: 0x0400269B RID: 9883
		[ProtoMember(101)]
		public string BuChangStartDay = "";

		// Token: 0x0400269C RID: 9884
		[ProtoMember(102)]
		public long DongJieStart = 0L;

		// Token: 0x0400269D RID: 9885
		[ProtoMember(103)]
		public int DongJieSeconds = 0;

		// Token: 0x0400269E RID: 9886
		[ProtoMember(104)]
		public string YueduDazhunpanStartDay = "";

		// Token: 0x0400269F RID: 9887
		[ProtoMember(105)]
		public int YueduDazhunpanStartDayNum = 0;

		// Token: 0x040026A0 RID: 9888
		[ProtoMember(106)]
		public int RoleStrength = 0;

		// Token: 0x040026A1 RID: 9889
		[ProtoMember(107)]
		public int RoleIntelligence = 0;

		// Token: 0x040026A2 RID: 9890
		[ProtoMember(108)]
		public int RoleDexterity = 0;

		// Token: 0x040026A3 RID: 9891
		[ProtoMember(109)]
		public int RoleConstitution = 0;

		// Token: 0x040026A4 RID: 9892
		[ProtoMember(110)]
		public int ChangeLifeCount = 0;

		// Token: 0x040026A5 RID: 9893
		[ProtoMember(111)]
		public int TotalPropPoint = 0;

		// Token: 0x040026A6 RID: 9894
		[ProtoMember(112)]
		public int IsFlashPlayer = 0;

		// Token: 0x040026A7 RID: 9895
		[ProtoMember(113)]
		public int AdmiredCount = 0;

		// Token: 0x040026A8 RID: 9896
		[ProtoMember(114)]
		public int CombatForce = 0;

		// Token: 0x040026A9 RID: 9897
		[ProtoMember(115)]
		public int AdorationCount = 0;

		// Token: 0x040026AA RID: 9898
		[ProtoMember(116)]
		public int DayOnlineSecond = 0;

		// Token: 0x040026AB RID: 9899
		[ProtoMember(117)]
		public int SeriesLoginNum = 0;

		// Token: 0x040026AC RID: 9900
		[ProtoMember(118)]
		public int AutoAssignPropertyPoint = 0;

		// Token: 0x040026AD RID: 9901
		[ProtoMember(119)]
		public int OnLineTotalTime = 0;

		// Token: 0x040026AE RID: 9902
		[ProtoMember(120)]
		public int AllZhuoYueNum = 0;

		// Token: 0x040026AF RID: 9903
		[ProtoMember(121)]
		public int VIPLevel = 0;

		// Token: 0x040026B0 RID: 9904
		[ProtoMember(122)]
		public int OpenGridTime = 0;

		// Token: 0x040026B1 RID: 9905
		[ProtoMember(123)]
		public int OpenPortableGridTime = 0;

		// Token: 0x040026B2 RID: 9906
		[ProtoMember(124)]
		public WingData MyWingData = null;

		// Token: 0x040026B3 RID: 9907
		[ProtoMember(125)]
		public Dictionary<int, int> PictureJudgeReferInfo = null;

		// Token: 0x040026B4 RID: 9908
		[ProtoMember(126)]
		public int StarSoulValue = 0;

		// Token: 0x040026B5 RID: 9909
		[ProtoMember(127)]
		public long StoreYinLiang = 0L;

		// Token: 0x040026B6 RID: 9910
		[ProtoMember(128)]
		public long StoreMoney = 0L;

		// Token: 0x040026B7 RID: 9911
		[ProtoMember(129)]
		public string UserReturnTimeBegin = "";

		// Token: 0x040026B8 RID: 9912
		[ProtoMember(130)]
		public string UserReturnTimeEnd = "";

		// Token: 0x040026B9 RID: 9913
		[ProtoMember(131)]
		public TalentData MyTalentData = null;

		// Token: 0x040026BA RID: 9914
		[ProtoMember(132)]
		public int TianTiRongYao;

		// Token: 0x040026BB RID: 9915
		[ProtoMember(133)]
		public FluorescentGemData FluorescentGemData = null;

		// Token: 0x040026BC RID: 9916
		[ProtoMember(134)]
		public int GMAuth = 0;

		// Token: 0x040026BD RID: 9917
		[ProtoMember(135)]
		public SoulStoneData SoulStoneData = null;

		// Token: 0x040026BE RID: 9918
		[ProtoMember(136)]
		public long SettingBitFlags;

		// Token: 0x040026BF RID: 9919
		[ProtoMember(137)]
		public int SpouseId;

		// Token: 0x040026C0 RID: 9920
		[ProtoMember(138)]
		public List<ActivityData> ActivityList = null;

		// Token: 0x040026C1 RID: 9921
		[ProtoMember(139)]
		public List<int> OccupationList;

		// Token: 0x040026C2 RID: 9922
		[ProtoMember(140)]
		public int JunTuanId;

		// Token: 0x040026C3 RID: 9923
		[ProtoMember(141)]
		public string JunTuanName;

		// Token: 0x040026C4 RID: 9924
		[ProtoMember(142)]
		public int JunTuanZhiWu;

		// Token: 0x040026C5 RID: 9925
		[ProtoMember(143)]
		public int LingDi;

		// Token: 0x040026C6 RID: 9926
		[ProtoMember(144)]
		public Dictionary<int, ShenJiFuWenData> ShenJiDict = null;

		// Token: 0x040026C7 RID: 9927
		[ProtoMember(145)]
		public RoleHuiJiData HuiJiData;

		// Token: 0x040026C8 RID: 9928
		[ProtoMember(146)]
		public List<FuWenTabData> FuWenTabList;

		// Token: 0x040026C9 RID: 9929
		[ProtoMember(147)]
		public int HideGM;

		// Token: 0x040026CA RID: 9930
		[ProtoMember(148)]
		public JueXingShiData JueXingData;

		// Token: 0x040026CB RID: 9931
		[ProtoMember(149, IsRequired = true)]
		public LongCollection MoneyData;

		// Token: 0x040026CC RID: 9932
		[ProtoMember(150)]
		public int CompType;

		// Token: 0x040026CD RID: 9933
		[ProtoMember(151)]
		public byte CompZhiWu;

		// Token: 0x040026CE RID: 9934
		[ProtoMember(152)]
		public List<GoodsData> MountStoreList;

		// Token: 0x040026CF RID: 9935
		[ProtoMember(153)]
		public List<GoodsData> MountEquipList;

		// Token: 0x040026D0 RID: 9936
		[ProtoMember(154)]
		public List<GoodsLimitData> GoodsLimitDataList;

		// Token: 0x040026D1 RID: 9937
		[ProtoMember(155, IsRequired = true)]
		public SystemOpenData OpenData;

		// Token: 0x040026D2 RID: 9938
		[ProtoMember(156)]
		public int ThemeState;

		// Token: 0x040026D3 RID: 9939
		[ProtoMember(157)]
		public int SubOccupation;

		// Token: 0x040026D4 RID: 9940
		[ProtoMember(158, IsRequired = true)]
		public RoleArmorData ArmorData;

		// Token: 0x040026D5 RID: 9941
		[ProtoMember(159)]
		public int CurrentArmorV;

		// Token: 0x040026D6 RID: 9942
		[ProtoMember(160)]
		public int MaxArmorV;

		// Token: 0x040026D7 RID: 9943
		[ProtoMember(161)]
		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData;

		// Token: 0x040026D8 RID: 9944
		[ProtoMember(162)]
		public RoleBianShenData BianShenData;

		// Token: 0x040026D9 RID: 9945
		[ProtoMember(163)]
		public int RebornCombatForce = 0;

		// Token: 0x040026DA RID: 9946
		[ProtoMember(164)]
		public int RebornCount = 0;

		// Token: 0x040026DB RID: 9947
		[ProtoMember(165)]
		public int RebornLevel = 0;

		// Token: 0x040026DC RID: 9948
		[ProtoMember(166)]
		public long RebornExperience = 0L;

		// Token: 0x040026DD RID: 9949
		[ProtoMember(167)]
		public List<GoodsData> RebornGoodsDataList;

		// Token: 0x040026DE RID: 9950
		[ProtoMember(168)]
		public List<GoodsData> RebornGoodsStoreList;

		// Token: 0x040026DF RID: 9951
		[ProtoMember(169)]
		public int RebornBagNum = 0;

		// Token: 0x040026E0 RID: 9952
		[ProtoMember(170)]
		public RebornPortableBagData RebornGirdData = null;

		// Token: 0x040026E1 RID: 9953
		[ProtoMember(171)]
		public int OpenRebornGridTime = 0;

		// Token: 0x040026E2 RID: 9954
		[ProtoMember(172)]
		public int OpenRebornPortableGridTime = 0;

		// Token: 0x040026E3 RID: 9955
		[ProtoMember(173)]
		public int RebornShowEquip = 0;

		// Token: 0x040026E4 RID: 9956
		[ProtoMember(174)]
		public RebornStampData RebornYinJi = null;

		// Token: 0x040026E5 RID: 9957
		[ProtoMember(175)]
		public int RebornShowModel = 0;

		// Token: 0x040026E6 RID: 9958
		[ProtoMember(176)]
		public int ZhanDuiID;

		// Token: 0x040026E7 RID: 9959
		[ProtoMember(177)]
		public int ZhanDuiZhiWu;

		// Token: 0x040026E8 RID: 9960
		[ProtoMember(178)]
		public List<GoodsData> HolyGoodsDataList;

		// Token: 0x040026E9 RID: 9961
		[ProtoMember(179)]
		public Dictionary<int, RebornEquipData> RebornEquipHoleData = null;

		// Token: 0x040026EA RID: 9962
		[ProtoMember(180)]
		public Dictionary<int, MazingerStoreData> MazingerStoreDataInfo = null;

		// Token: 0x040026EB RID: 9963
		[ProtoMember(250)]
		public int UserPTID;

		// Token: 0x040026EC RID: 9964
		[ProtoMember(251)]
		public string WorldRoleID;

		// Token: 0x040026ED RID: 9965
		[ProtoMember(252)]
		public string Channel;
	}
}
