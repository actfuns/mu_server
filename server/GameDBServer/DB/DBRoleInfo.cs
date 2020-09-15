using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Data;
using GameDBServer.Data.Tarot;
using GameDBServer.Logic;
using GameDBServer.Logic.FluorescentGem;
using GameDBServer.Logic.Rank;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
	// Token: 0x020000EC RID: 236
	public class DBRoleInfo
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060002BE RID: 702 RVA: 0x00016C90 File Offset: 0x00014E90
		// (set) Token: 0x060002BF RID: 703 RVA: 0x00016CA7 File Offset: 0x00014EA7
		public int RoleID { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060002C0 RID: 704 RVA: 0x00016CB0 File Offset: 0x00014EB0
		// (set) Token: 0x060002C1 RID: 705 RVA: 0x00016CC7 File Offset: 0x00014EC7
		public string UserID { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060002C2 RID: 706 RVA: 0x00016CD0 File Offset: 0x00014ED0
		// (set) Token: 0x060002C3 RID: 707 RVA: 0x00016CE7 File Offset: 0x00014EE7
		public string RoleName { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x00016CF0 File Offset: 0x00014EF0
		// (set) Token: 0x060002C5 RID: 709 RVA: 0x00016D07 File Offset: 0x00014F07
		public int RoleSex { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x00016D10 File Offset: 0x00014F10
		// (set) Token: 0x060002C7 RID: 711 RVA: 0x00016D27 File Offset: 0x00014F27
		public int Occupation { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x00016D30 File Offset: 0x00014F30
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x00016D47 File Offset: 0x00014F47
		public int Level { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060002CA RID: 714 RVA: 0x00016D50 File Offset: 0x00014F50
		// (set) Token: 0x060002CB RID: 715 RVA: 0x00016D67 File Offset: 0x00014F67
		public int RolePic { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060002CC RID: 716 RVA: 0x00016D70 File Offset: 0x00014F70
		// (set) Token: 0x060002CD RID: 717 RVA: 0x00016D87 File Offset: 0x00014F87
		public int Faction { get; set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060002CE RID: 718 RVA: 0x00016D90 File Offset: 0x00014F90
		// (set) Token: 0x060002CF RID: 719 RVA: 0x00016DA7 File Offset: 0x00014FA7
		public int Money1 { get; set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x00016DB0 File Offset: 0x00014FB0
		// (set) Token: 0x060002D1 RID: 721 RVA: 0x00016DC7 File Offset: 0x00014FC7
		public int Money2 { get; set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x00016DD0 File Offset: 0x00014FD0
		// (set) Token: 0x060002D3 RID: 723 RVA: 0x00016DE7 File Offset: 0x00014FE7
		public long Experience { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x00016DF0 File Offset: 0x00014FF0
		// (set) Token: 0x060002D5 RID: 725 RVA: 0x00016E07 File Offset: 0x00015007
		public int PKMode { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x00016E10 File Offset: 0x00015010
		// (set) Token: 0x060002D7 RID: 727 RVA: 0x00016E27 File Offset: 0x00015027
		public int PKValue { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x00016E30 File Offset: 0x00015030
		// (set) Token: 0x060002D9 RID: 729 RVA: 0x00016E47 File Offset: 0x00015047
		public string Position { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060002DA RID: 730 RVA: 0x00016E50 File Offset: 0x00015050
		// (set) Token: 0x060002DB RID: 731 RVA: 0x00016E67 File Offset: 0x00015067
		public string RegTime { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060002DC RID: 732 RVA: 0x00016E70 File Offset: 0x00015070
		// (set) Token: 0x060002DD RID: 733 RVA: 0x00016E87 File Offset: 0x00015087
		public long LastTime { get; set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060002DE RID: 734 RVA: 0x00016E90 File Offset: 0x00015090
		// (set) Token: 0x060002DF RID: 735 RVA: 0x00016EA7 File Offset: 0x000150A7
		public int BagNum { get; set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x00016EB0 File Offset: 0x000150B0
		// (set) Token: 0x060002E1 RID: 737 RVA: 0x00016EC7 File Offset: 0x000150C7
		public int RebornBagNum { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x00016ED0 File Offset: 0x000150D0
		// (set) Token: 0x060002E3 RID: 739 RVA: 0x00016EE7 File Offset: 0x000150E7
		public string OtherName { get; set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x00016EF0 File Offset: 0x000150F0
		// (set) Token: 0x060002E5 RID: 741 RVA: 0x00016F07 File Offset: 0x00015107
		public string MainQuickBarKeys { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060002E6 RID: 742 RVA: 0x00016F10 File Offset: 0x00015110
		// (set) Token: 0x060002E7 RID: 743 RVA: 0x00016F27 File Offset: 0x00015127
		public string OtherQuickBarKeys { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060002E8 RID: 744 RVA: 0x00016F30 File Offset: 0x00015130
		// (set) Token: 0x060002E9 RID: 745 RVA: 0x00016F47 File Offset: 0x00015147
		public int LoginNum { get; set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060002EA RID: 746 RVA: 0x00016F50 File Offset: 0x00015150
		// (set) Token: 0x060002EB RID: 747 RVA: 0x00016F67 File Offset: 0x00015167
		public int LeftFightSeconds { get; set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060002EC RID: 748 RVA: 0x00016F70 File Offset: 0x00015170
		// (set) Token: 0x060002ED RID: 749 RVA: 0x00016F87 File Offset: 0x00015187
		public int ServerLineID { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060002EE RID: 750 RVA: 0x00016F90 File Offset: 0x00015190
		// (set) Token: 0x060002EF RID: 751 RVA: 0x00016FA7 File Offset: 0x000151A7
		public int HorseDbID { get; set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x00016FB0 File Offset: 0x000151B0
		// (set) Token: 0x060002F1 RID: 753 RVA: 0x00016FC7 File Offset: 0x000151C7
		public int PetDbID { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060002F2 RID: 754 RVA: 0x00016FD0 File Offset: 0x000151D0
		// (set) Token: 0x060002F3 RID: 755 RVA: 0x00016FE7 File Offset: 0x000151E7
		public int InterPower { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060002F4 RID: 756 RVA: 0x00016FF0 File Offset: 0x000151F0
		// (set) Token: 0x060002F5 RID: 757 RVA: 0x00017007 File Offset: 0x00015207
		public int TotalOnlineSecs { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060002F6 RID: 758 RVA: 0x00017010 File Offset: 0x00015210
		// (set) Token: 0x060002F7 RID: 759 RVA: 0x00017027 File Offset: 0x00015227
		public int AntiAddictionSecs { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060002F8 RID: 760 RVA: 0x00017030 File Offset: 0x00015230
		// (set) Token: 0x060002F9 RID: 761 RVA: 0x00017047 File Offset: 0x00015247
		public long LogOffTime { get; set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060002FA RID: 762 RVA: 0x00017050 File Offset: 0x00015250
		// (set) Token: 0x060002FB RID: 763 RVA: 0x00017067 File Offset: 0x00015267
		public long BiGuanTime { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060002FC RID: 764 RVA: 0x00017070 File Offset: 0x00015270
		// (set) Token: 0x060002FD RID: 765 RVA: 0x00017087 File Offset: 0x00015287
		public int YinLiang { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060002FE RID: 766 RVA: 0x00017090 File Offset: 0x00015290
		// (set) Token: 0x060002FF RID: 767 RVA: 0x000170A7 File Offset: 0x000152A7
		public int TotalJingMaiExp { get; set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000300 RID: 768 RVA: 0x000170B0 File Offset: 0x000152B0
		// (set) Token: 0x06000301 RID: 769 RVA: 0x000170C7 File Offset: 0x000152C7
		public int JingMaiExpNum { get; set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000302 RID: 770 RVA: 0x000170D0 File Offset: 0x000152D0
		// (set) Token: 0x06000303 RID: 771 RVA: 0x000170E7 File Offset: 0x000152E7
		public int LastHorseID { get; set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000304 RID: 772 RVA: 0x000170F0 File Offset: 0x000152F0
		// (set) Token: 0x06000305 RID: 773 RVA: 0x00017107 File Offset: 0x00015307
		public int DefaultSkillID { get; set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000306 RID: 774 RVA: 0x00017110 File Offset: 0x00015310
		// (set) Token: 0x06000307 RID: 775 RVA: 0x00017127 File Offset: 0x00015327
		public int AutoLifeV { get; set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000308 RID: 776 RVA: 0x00017130 File Offset: 0x00015330
		// (set) Token: 0x06000309 RID: 777 RVA: 0x00017147 File Offset: 0x00015347
		public int AutoMagicV { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600030A RID: 778 RVA: 0x00017150 File Offset: 0x00015350
		// (set) Token: 0x0600030B RID: 779 RVA: 0x00017167 File Offset: 0x00015367
		public int NumSkillID { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600030C RID: 780 RVA: 0x00017170 File Offset: 0x00015370
		// (set) Token: 0x0600030D RID: 781 RVA: 0x00017187 File Offset: 0x00015387
		public int MainTaskID { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600030E RID: 782 RVA: 0x00017190 File Offset: 0x00015390
		// (set) Token: 0x0600030F RID: 783 RVA: 0x000171A7 File Offset: 0x000153A7
		public int PKPoint { get; set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000310 RID: 784 RVA: 0x000171B0 File Offset: 0x000153B0
		// (set) Token: 0x06000311 RID: 785 RVA: 0x000171C7 File Offset: 0x000153C7
		public int LianZhan { get; set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000312 RID: 786 RVA: 0x000171D0 File Offset: 0x000153D0
		// (set) Token: 0x06000313 RID: 787 RVA: 0x000171E7 File Offset: 0x000153E7
		public int KillBoss { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000314 RID: 788 RVA: 0x000171F0 File Offset: 0x000153F0
		// (set) Token: 0x06000315 RID: 789 RVA: 0x00017207 File Offset: 0x00015407
		public long BattleNameStart { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000316 RID: 790 RVA: 0x00017210 File Offset: 0x00015410
		// (set) Token: 0x06000317 RID: 791 RVA: 0x00017227 File Offset: 0x00015427
		public int BattleNameIndex { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000318 RID: 792 RVA: 0x00017230 File Offset: 0x00015430
		// (set) Token: 0x06000319 RID: 793 RVA: 0x00017247 File Offset: 0x00015447
		public int CZTaskID { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600031A RID: 794 RVA: 0x00017250 File Offset: 0x00015450
		// (set) Token: 0x0600031B RID: 795 RVA: 0x00017267 File Offset: 0x00015467
		public int BattleNum { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600031C RID: 796 RVA: 0x00017270 File Offset: 0x00015470
		// (set) Token: 0x0600031D RID: 797 RVA: 0x00017287 File Offset: 0x00015487
		public int HeroIndex { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600031E RID: 798 RVA: 0x00017290 File Offset: 0x00015490
		// (set) Token: 0x0600031F RID: 799 RVA: 0x000172A7 File Offset: 0x000154A7
		public int LoginDayID { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000320 RID: 800 RVA: 0x000172B0 File Offset: 0x000154B0
		// (set) Token: 0x06000321 RID: 801 RVA: 0x000172C7 File Offset: 0x000154C7
		public int LoginDayNum { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000322 RID: 802 RVA: 0x000172D0 File Offset: 0x000154D0
		// (set) Token: 0x06000323 RID: 803 RVA: 0x000172E7 File Offset: 0x000154E7
		public int ZoneID { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000324 RID: 804 RVA: 0x000172F0 File Offset: 0x000154F0
		// (set) Token: 0x06000325 RID: 805 RVA: 0x00017307 File Offset: 0x00015507
		public string BHName { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000326 RID: 806 RVA: 0x00017310 File Offset: 0x00015510
		// (set) Token: 0x06000327 RID: 807 RVA: 0x00017327 File Offset: 0x00015527
		public int BHVerify { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000328 RID: 808 RVA: 0x00017330 File Offset: 0x00015530
		// (set) Token: 0x06000329 RID: 809 RVA: 0x00017347 File Offset: 0x00015547
		public int BHZhiWu { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600032A RID: 810 RVA: 0x00017350 File Offset: 0x00015550
		// (set) Token: 0x0600032B RID: 811 RVA: 0x00017367 File Offset: 0x00015567
		public int BGDayID1 { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x0600032C RID: 812 RVA: 0x00017370 File Offset: 0x00015570
		// (set) Token: 0x0600032D RID: 813 RVA: 0x00017387 File Offset: 0x00015587
		public int BGMoney { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600032E RID: 814 RVA: 0x00017390 File Offset: 0x00015590
		// (set) Token: 0x0600032F RID: 815 RVA: 0x000173A7 File Offset: 0x000155A7
		public int BGDayID2 { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000330 RID: 816 RVA: 0x000173B0 File Offset: 0x000155B0
		// (set) Token: 0x06000331 RID: 817 RVA: 0x000173C7 File Offset: 0x000155C7
		public int BGGoods { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000332 RID: 818 RVA: 0x000173D0 File Offset: 0x000155D0
		// (set) Token: 0x06000333 RID: 819 RVA: 0x000173E7 File Offset: 0x000155E7
		public int BangGong { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000334 RID: 820 RVA: 0x000173F0 File Offset: 0x000155F0
		// (set) Token: 0x06000335 RID: 821 RVA: 0x00017407 File Offset: 0x00015607
		public int HuangHou { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000336 RID: 822 RVA: 0x00017410 File Offset: 0x00015610
		// (set) Token: 0x06000337 RID: 823 RVA: 0x00017427 File Offset: 0x00015627
		public int JieBiaoDayID { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000338 RID: 824 RVA: 0x00017430 File Offset: 0x00015630
		// (set) Token: 0x06000339 RID: 825 RVA: 0x00017447 File Offset: 0x00015647
		public int JieBiaoDayNum { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600033A RID: 826 RVA: 0x00017450 File Offset: 0x00015650
		// (set) Token: 0x0600033B RID: 827 RVA: 0x00017467 File Offset: 0x00015667
		public string UserName { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600033C RID: 828 RVA: 0x00017470 File Offset: 0x00015670
		// (set) Token: 0x0600033D RID: 829 RVA: 0x00017487 File Offset: 0x00015687
		public int LastMailID { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600033E RID: 830 RVA: 0x00017490 File Offset: 0x00015690
		// (set) Token: 0x0600033F RID: 831 RVA: 0x000174A7 File Offset: 0x000156A7
		public long OnceAwardFlag { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000340 RID: 832 RVA: 0x000174B0 File Offset: 0x000156B0
		// (set) Token: 0x06000341 RID: 833 RVA: 0x000174C7 File Offset: 0x000156C7
		public int Gold { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000342 RID: 834 RVA: 0x000174D0 File Offset: 0x000156D0
		// (set) Token: 0x06000343 RID: 835 RVA: 0x000174E7 File Offset: 0x000156E7
		public int BanChat { get; set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000344 RID: 836 RVA: 0x000174F0 File Offset: 0x000156F0
		// (set) Token: 0x06000345 RID: 837 RVA: 0x00017507 File Offset: 0x00015707
		public int BanLogin { get; set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000346 RID: 838 RVA: 0x00017510 File Offset: 0x00015710
		// (set) Token: 0x06000347 RID: 839 RVA: 0x00017527 File Offset: 0x00015727
		public int IsFlashPlayer { get; set; }

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000348 RID: 840 RVA: 0x00017530 File Offset: 0x00015730
		// (set) Token: 0x06000349 RID: 841 RVA: 0x00017547 File Offset: 0x00015747
		public int ChangeLifeCount { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600034A RID: 842 RVA: 0x00017550 File Offset: 0x00015750
		// (set) Token: 0x0600034B RID: 843 RVA: 0x00017567 File Offset: 0x00015767
		public int AdmiredCount { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600034C RID: 844 RVA: 0x00017570 File Offset: 0x00015770
		// (set) Token: 0x0600034D RID: 845 RVA: 0x00017587 File Offset: 0x00015787
		public int CombatForce { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600034E RID: 846 RVA: 0x00017590 File Offset: 0x00015790
		// (set) Token: 0x0600034F RID: 847 RVA: 0x000175A7 File Offset: 0x000157A7
		public int AutoAssignPropertyPoint { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000350 RID: 848 RVA: 0x000175B0 File Offset: 0x000157B0
		// (set) Token: 0x06000351 RID: 849 RVA: 0x000175C7 File Offset: 0x000157C7
		public string PushMsgID { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000352 RID: 850 RVA: 0x000175D0 File Offset: 0x000157D0
		// (set) Token: 0x06000353 RID: 851 RVA: 0x000175E7 File Offset: 0x000157E7
		public int VipAwardFlag { get; set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000354 RID: 852 RVA: 0x000175F0 File Offset: 0x000157F0
		// (set) Token: 0x06000355 RID: 853 RVA: 0x00017607 File Offset: 0x00015807
		public int VIPLevel { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000356 RID: 854 RVA: 0x00017610 File Offset: 0x00015810
		// (set) Token: 0x06000357 RID: 855 RVA: 0x00017627 File Offset: 0x00015827
		public long store_yinliang { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000358 RID: 856 RVA: 0x00017630 File Offset: 0x00015830
		// (set) Token: 0x06000359 RID: 857 RVA: 0x00017647 File Offset: 0x00015847
		public long store_money { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600035A RID: 858 RVA: 0x00017650 File Offset: 0x00015850
		// (set) Token: 0x0600035B RID: 859 RVA: 0x00017667 File Offset: 0x00015867
		public int MagicSwordParam { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600035C RID: 860 RVA: 0x00017670 File Offset: 0x00015870
		// (set) Token: 0x0600035D RID: 861 RVA: 0x00017688 File Offset: 0x00015888
		public UserRankValueCache RankValue
		{
			get
			{
				return this.rankValue;
			}
			set
			{
				this.rankValue = value;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600035E RID: 862 RVA: 0x00017694 File Offset: 0x00015894
		// (set) Token: 0x0600035F RID: 863 RVA: 0x000176AB File Offset: 0x000158AB
		public long UpdateDBPositionTicks { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000360 RID: 864 RVA: 0x000176B4 File Offset: 0x000158B4
		// (set) Token: 0x06000361 RID: 865 RVA: 0x000176CB File Offset: 0x000158CB
		public long UpdateDBTimeTicks { get; set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000362 RID: 866 RVA: 0x000176D4 File Offset: 0x000158D4
		// (set) Token: 0x06000363 RID: 867 RVA: 0x000176EB File Offset: 0x000158EB
		public long UpdateDBInterPowerTimeTicks { get; set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000364 RID: 868 RVA: 0x000176F4 File Offset: 0x000158F4
		// (set) Token: 0x06000365 RID: 869 RVA: 0x0001770B File Offset: 0x0001590B
		public List<OldTaskData> OldTasks { get; set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000366 RID: 870 RVA: 0x00017714 File Offset: 0x00015914
		// (set) Token: 0x06000367 RID: 871 RVA: 0x0001772B File Offset: 0x0001592B
		public List<TaskData> DoingTaskList { get; set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000368 RID: 872 RVA: 0x00017734 File Offset: 0x00015934
		// (set) Token: 0x06000369 RID: 873 RVA: 0x0001774B File Offset: 0x0001594B
		public List<GoodsData> GoodsDataList { get; set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x0600036A RID: 874 RVA: 0x00017754 File Offset: 0x00015954
		// (set) Token: 0x0600036B RID: 875 RVA: 0x0001776B File Offset: 0x0001596B
		public List<GoodsData> RebornGoodsDataList { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600036C RID: 876 RVA: 0x00017774 File Offset: 0x00015974
		// (set) Token: 0x0600036D RID: 877 RVA: 0x0001778B File Offset: 0x0001598B
		public List<GoodsLimitData> GoodsLimitDataList { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600036E RID: 878 RVA: 0x00017794 File Offset: 0x00015994
		// (set) Token: 0x0600036F RID: 879 RVA: 0x000177AB File Offset: 0x000159AB
		public List<FriendData> FriendDataList { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000370 RID: 880 RVA: 0x000177B4 File Offset: 0x000159B4
		// (set) Token: 0x06000371 RID: 881 RVA: 0x000177CB File Offset: 0x000159CB
		public List<HorseData> HorsesDataList { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000372 RID: 882 RVA: 0x000177D4 File Offset: 0x000159D4
		// (set) Token: 0x06000373 RID: 883 RVA: 0x000177EB File Offset: 0x000159EB
		public List<PetData> PetsDataList { get; set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000374 RID: 884 RVA: 0x000177F4 File Offset: 0x000159F4
		// (set) Token: 0x06000375 RID: 885 RVA: 0x0001780B File Offset: 0x00015A0B
		public long LastDJPointDataTikcs { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000376 RID: 886 RVA: 0x00017814 File Offset: 0x00015A14
		// (set) Token: 0x06000377 RID: 887 RVA: 0x0001782B File Offset: 0x00015A2B
		public DJPointData RoleDJPointData { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000378 RID: 888 RVA: 0x00017834 File Offset: 0x00015A34
		// (set) Token: 0x06000379 RID: 889 RVA: 0x0001784B File Offset: 0x00015A4B
		public List<JingMaiData> JingMaiDataList { get; set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600037A RID: 890 RVA: 0x00017854 File Offset: 0x00015A54
		// (set) Token: 0x0600037B RID: 891 RVA: 0x0001786B File Offset: 0x00015A6B
		public List<SkillData> SkillDataList { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600037C RID: 892 RVA: 0x00017874 File Offset: 0x00015A74
		// (set) Token: 0x0600037D RID: 893 RVA: 0x0001788B File Offset: 0x00015A8B
		public List<BufferData> BufferDataList { get; set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600037E RID: 894 RVA: 0x00017894 File Offset: 0x00015A94
		// (set) Token: 0x0600037F RID: 895 RVA: 0x000178AB File Offset: 0x00015AAB
		public List<DailyTaskData> MyDailyTaskDataList { get; set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000380 RID: 896 RVA: 0x000178B4 File Offset: 0x00015AB4
		// (set) Token: 0x06000381 RID: 897 RVA: 0x000178CB File Offset: 0x00015ACB
		public DailyJingMaiData MyDailyJingMaiData { get; set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000382 RID: 898 RVA: 0x000178D4 File Offset: 0x00015AD4
		// (set) Token: 0x06000383 RID: 899 RVA: 0x000178EB File Offset: 0x00015AEB
		public PortableBagData MyPortableBagData { get; set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000384 RID: 900 RVA: 0x000178F4 File Offset: 0x00015AF4
		// (set) Token: 0x06000385 RID: 901 RVA: 0x0001790B File Offset: 0x00015B0B
		public RebornPortableBagData RebornGirdData { get; set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000386 RID: 902 RVA: 0x00017914 File Offset: 0x00015B14
		// (set) Token: 0x06000387 RID: 903 RVA: 0x0001792B File Offset: 0x00015B2B
		public int RebornShowEquip { get; set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000388 RID: 904 RVA: 0x00017934 File Offset: 0x00015B34
		// (set) Token: 0x06000389 RID: 905 RVA: 0x0001794B File Offset: 0x00015B4B
		public int RebornShowModel { get; set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600038A RID: 906 RVA: 0x00017954 File Offset: 0x00015B54
		// (set) Token: 0x0600038B RID: 907 RVA: 0x0001796B File Offset: 0x00015B6B
		public bool ExistsMyHuodongData { get; set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600038C RID: 908 RVA: 0x00017974 File Offset: 0x00015B74
		// (set) Token: 0x0600038D RID: 909 RVA: 0x0001798B File Offset: 0x00015B8B
		public HuodongData MyHuodongData { get; set; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600038E RID: 910 RVA: 0x00017994 File Offset: 0x00015B94
		// (set) Token: 0x0600038F RID: 911 RVA: 0x000179AB File Offset: 0x00015BAB
		public List<FuBenData> FuBenDataList { get; set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000390 RID: 912 RVA: 0x000179B4 File Offset: 0x00015BB4
		// (set) Token: 0x06000391 RID: 913 RVA: 0x000179CB File Offset: 0x00015BCB
		public MarriageData MyMarriageData { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000392 RID: 914 RVA: 0x000179D4 File Offset: 0x00015BD4
		// (set) Token: 0x06000393 RID: 915 RVA: 0x000179EB File Offset: 0x00015BEB
		public Dictionary<int, int> MyMarryPartyJoinList { get; set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000394 RID: 916 RVA: 0x000179F4 File Offset: 0x00015BF4
		// (set) Token: 0x06000395 RID: 917 RVA: 0x00017A0B File Offset: 0x00015C0B
		public Dictionary<sbyte, HolyItemData> MyHolyItemDataDic { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000396 RID: 918 RVA: 0x00017A14 File Offset: 0x00015C14
		// (set) Token: 0x06000397 RID: 919 RVA: 0x00017A2B File Offset: 0x00015C2B
		public RoleDailyData MyRoleDailyData { get; set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000398 RID: 920 RVA: 0x00017A34 File Offset: 0x00015C34
		// (set) Token: 0x06000399 RID: 921 RVA: 0x00017A4B File Offset: 0x00015C4B
		public YaBiaoData MyYaBiaoData { get; set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600039A RID: 922 RVA: 0x00017A54 File Offset: 0x00015C54
		// (set) Token: 0x0600039B RID: 923 RVA: 0x00017A6C File Offset: 0x00015C6C
		public long LastReferenceTicks
		{
			get
			{
				return this._LastReferenceTicks;
			}
			set
			{
				this._LastReferenceTicks = value;
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600039C RID: 924 RVA: 0x00017A78 File Offset: 0x00015C78
		// (set) Token: 0x0600039D RID: 925 RVA: 0x00017A8F File Offset: 0x00015C8F
		public Dictionary<int, int> PaiHangPosDict { get; set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x0600039E RID: 926 RVA: 0x00017A98 File Offset: 0x00015C98
		// (set) Token: 0x0600039F RID: 927 RVA: 0x00017AAF File Offset: 0x00015CAF
		public List<VipDailyData> VipDailyDataList { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060003A0 RID: 928 RVA: 0x00017AB8 File Offset: 0x00015CB8
		// (set) Token: 0x060003A1 RID: 929 RVA: 0x00017ACF File Offset: 0x00015CCF
		public YangGongBKDailyJiFenData YangGongBKDailyJiFen { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060003A2 RID: 930 RVA: 0x00017AD8 File Offset: 0x00015CD8
		// (set) Token: 0x060003A3 RID: 931 RVA: 0x00017AEF File Offset: 0x00015CEF
		public WingData MyWingData { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060003A4 RID: 932 RVA: 0x00017AF8 File Offset: 0x00015CF8
		// (set) Token: 0x060003A5 RID: 933 RVA: 0x00017B0F File Offset: 0x00015D0F
		public Dictionary<int, int> PictureJudgeReferInfo { get; set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x00017B18 File Offset: 0x00015D18
		// (set) Token: 0x060003A7 RID: 935 RVA: 0x00017B2F File Offset: 0x00015D2F
		public Dictionary<int, int> StarConstellationInfo { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00017B38 File Offset: 0x00015D38
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x00017B4F File Offset: 0x00015D4F
		public Dictionary<int, LingYuData> LingYuDict { get; set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060003AA RID: 938 RVA: 0x00017B58 File Offset: 0x00015D58
		// (set) Token: 0x060003AB RID: 939 RVA: 0x00017B6F File Offset: 0x00015D6F
		public GuardStatueDetail MyGuardStatueDetail { get; set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060003AC RID: 940 RVA: 0x00017B78 File Offset: 0x00015D78
		// (set) Token: 0x060003AD RID: 941 RVA: 0x00017B8F File Offset: 0x00015D8F
		public TalentData MyTalentData { get; set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060003AE RID: 942 RVA: 0x00017B98 File Offset: 0x00015D98
		// (set) Token: 0x060003AF RID: 943 RVA: 0x00017BAF File Offset: 0x00015DAF
		public string LastIP { get; set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060003B0 RID: 944 RVA: 0x00017BB8 File Offset: 0x00015DB8
		// (set) Token: 0x060003B1 RID: 945 RVA: 0x00017BCF File Offset: 0x00015DCF
		public List<int> GroupMailRecordList { get; set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060003B2 RID: 946 RVA: 0x00017BD8 File Offset: 0x00015DD8
		// (set) Token: 0x060003B3 RID: 947 RVA: 0x00017BEF File Offset: 0x00015DEF
		public MerlinGrowthSaveDBData MerlinData { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x00017BF8 File Offset: 0x00015DF8
		// (set) Token: 0x060003B5 RID: 949 RVA: 0x00017C0F File Offset: 0x00015E0F
		public FluorescentGemData FluorescentGemData { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x00017C18 File Offset: 0x00015E18
		// (set) Token: 0x060003B7 RID: 951 RVA: 0x00017C2F File Offset: 0x00015E2F
		public int FluorescentPoint { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x00017C38 File Offset: 0x00015E38
		// (set) Token: 0x060003B9 RID: 953 RVA: 0x00017C4F File Offset: 0x00015E4F
		public List<BuildingData> BuildingDataList { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060003BA RID: 954 RVA: 0x00017C58 File Offset: 0x00015E58
		// (set) Token: 0x060003BB RID: 955 RVA: 0x00017C6F File Offset: 0x00015E6F
		public Dictionary<int, OrnamentData> OrnamentDataDict { get; set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060003BC RID: 956 RVA: 0x00017C78 File Offset: 0x00015E78
		// (set) Token: 0x060003BD RID: 957 RVA: 0x00017C8F File Offset: 0x00015E8F
		public Dictionary<int, Dictionary<int, SevenDayItemData>> SevenDayActDict { get; set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060003BE RID: 958 RVA: 0x00017C98 File Offset: 0x00015E98
		// (set) Token: 0x060003BF RID: 959 RVA: 0x00017CAF File Offset: 0x00015EAF
		public long BanTradeToTicks { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060003C0 RID: 960 RVA: 0x00017CB8 File Offset: 0x00015EB8
		// (set) Token: 0x060003C1 RID: 961 RVA: 0x00017CCF File Offset: 0x00015ECF
		public Dictionary<int, SpecActInfoDB> SpecActInfoDict { get; set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x00017CD8 File Offset: 0x00015ED8
		// (set) Token: 0x060003C3 RID: 963 RVA: 0x00017CEF File Offset: 0x00015EEF
		public Dictionary<int, EverydayActInfoDB> EverydayActInfoDict { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060003C4 RID: 964 RVA: 0x00017CF8 File Offset: 0x00015EF8
		// (set) Token: 0x060003C5 RID: 965 RVA: 0x00017D0F File Offset: 0x00015F0F
		public Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> SpecPriorityActInfoDict { get; set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060003C6 RID: 966 RVA: 0x00017D18 File Offset: 0x00015F18
		// (set) Token: 0x060003C7 RID: 967 RVA: 0x00017D2F File Offset: 0x00015F2F
		public AlchemyDataDB AlchemyInfo { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060003C8 RID: 968 RVA: 0x00017D38 File Offset: 0x00015F38
		// (set) Token: 0x060003C9 RID: 969 RVA: 0x00017D4F File Offset: 0x00015F4F
		public Dictionary<int, ShenJiFuWenData> ShenJiDict { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060003CA RID: 970 RVA: 0x00017D58 File Offset: 0x00015F58
		// (set) Token: 0x060003CB RID: 971 RVA: 0x00017D6F File Offset: 0x00015F6F
		public TarotSystemData TarotData { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060003CC RID: 972 RVA: 0x00017D78 File Offset: 0x00015F78
		// (set) Token: 0x060003CD RID: 973 RVA: 0x00017D8F File Offset: 0x00015F8F
		public List<FuWenTabData> FuWenTabList { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060003CE RID: 974 RVA: 0x00017D98 File Offset: 0x00015F98
		// (set) Token: 0x060003CF RID: 975 RVA: 0x00017DAF File Offset: 0x00015FAF
		public List<TaoZhuangData> JueXingTaoZhuangList { get; set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060003D0 RID: 976 RVA: 0x00017DB8 File Offset: 0x00015FB8
		// (set) Token: 0x060003D1 RID: 977 RVA: 0x00017DCF File Offset: 0x00015FCF
		public List<MountData> MountList { get; set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060003D2 RID: 978 RVA: 0x00017DD8 File Offset: 0x00015FD8
		// (set) Token: 0x060003D3 RID: 979 RVA: 0x00017DEF File Offset: 0x00015FEF
		public RebornStampData RebornYinJi { get; set; }

		// Token: 0x060003D4 RID: 980 RVA: 0x00017DF8 File Offset: 0x00015FF8
		public static void DBTableRow2RoleInfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd, int index)
		{
			dbRoleInfo.RoleID = Convert.ToInt32(cmd.Table.Rows[index]["rid"]);
			dbRoleInfo.UserID = cmd.Table.Rows[index]["userid"].ToString();
			dbRoleInfo.RoleName = cmd.Table.Rows[index]["rname"].ToString();
			dbRoleInfo.RoleSex = Convert.ToInt32(cmd.Table.Rows[index]["sex"]);
			dbRoleInfo.Occupation = Convert.ToInt32(cmd.Table.Rows[index]["occupation"]);
			dbRoleInfo.Level = Convert.ToInt32(cmd.Table.Rows[index]["level"]);
			dbRoleInfo.RolePic = Convert.ToInt32(cmd.Table.Rows[index]["pic"]);
			dbRoleInfo.Faction = Convert.ToInt32(cmd.Table.Rows[index]["faction"]);
			dbRoleInfo.Money1 = Convert.ToInt32(cmd.Table.Rows[index]["money1"]);
			dbRoleInfo.Money2 = Convert.ToInt32(cmd.Table.Rows[index]["money2"]);
			dbRoleInfo.Experience = Convert.ToInt64(cmd.Table.Rows[index]["experience"]);
			dbRoleInfo.PKMode = Convert.ToInt32(cmd.Table.Rows[index]["pkmode"]);
			dbRoleInfo.PKValue = Convert.ToInt32(cmd.Table.Rows[index]["pkvalue"]);
			dbRoleInfo.Position = cmd.Table.Rows[index]["position"].ToString();
			dbRoleInfo.RegTime = cmd.Table.Rows[index]["regtime"].ToString();
			dbRoleInfo.LastTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["lasttime"].ToString());
			dbRoleInfo.BagNum = Convert.ToInt32(cmd.Table.Rows[index]["bagnum"]);
			dbRoleInfo.OtherName = cmd.Table.Rows[index]["othername"].ToString();
			dbRoleInfo.MainQuickBarKeys = cmd.Table.Rows[index]["main_quick_keys"].ToString();
			dbRoleInfo.OtherQuickBarKeys = cmd.Table.Rows[index]["other_quick_keys"].ToString();
			dbRoleInfo.LoginNum = Convert.ToInt32(cmd.Table.Rows[index]["loginnum"].ToString());
			dbRoleInfo.LeftFightSeconds = Convert.ToInt32(cmd.Table.Rows[index]["leftfightsecs"].ToString());
			dbRoleInfo.HorseDbID = Convert.ToInt32(cmd.Table.Rows[index]["horseid"].ToString());
			dbRoleInfo.PetDbID = Convert.ToInt32(cmd.Table.Rows[index]["petid"].ToString());
			dbRoleInfo.InterPower = Convert.ToInt32(cmd.Table.Rows[index]["interpower"].ToString());
			dbRoleInfo.TotalOnlineSecs = Convert.ToInt32(cmd.Table.Rows[index]["totalonlinesecs"].ToString());
			dbRoleInfo.AntiAddictionSecs = Convert.ToInt32(cmd.Table.Rows[index]["antiaddictionsecs"].ToString());
			dbRoleInfo.LogOffTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["logofftime"].ToString());
			dbRoleInfo.BiGuanTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["biguantime"].ToString());
			dbRoleInfo.YinLiang = Convert.ToInt32(cmd.Table.Rows[index]["yinliang"].ToString());
			dbRoleInfo.TotalJingMaiExp = Convert.ToInt32(cmd.Table.Rows[index]["total_jingmai_exp"].ToString());
			dbRoleInfo.JingMaiExpNum = Convert.ToInt32(cmd.Table.Rows[index]["jingmai_exp_num"].ToString());
			dbRoleInfo.LastHorseID = Convert.ToInt32(cmd.Table.Rows[index]["lasthorseid"].ToString());
			dbRoleInfo.DefaultSkillID = Convert.ToInt32(cmd.Table.Rows[index]["skillid"].ToString());
			dbRoleInfo.AutoLifeV = Convert.ToInt32(cmd.Table.Rows[index]["autolife"].ToString());
			dbRoleInfo.AutoMagicV = Convert.ToInt32(cmd.Table.Rows[index]["automagic"].ToString());
			dbRoleInfo.NumSkillID = Convert.ToInt32(cmd.Table.Rows[index]["numskillid"].ToString());
			dbRoleInfo.MainTaskID = Convert.ToInt32(cmd.Table.Rows[index]["maintaskid"].ToString());
			dbRoleInfo.PKPoint = Convert.ToInt32(cmd.Table.Rows[index]["pkpoint"].ToString());
			dbRoleInfo.LianZhan = Convert.ToInt32(cmd.Table.Rows[index]["lianzhan"].ToString());
			dbRoleInfo.KillBoss = Convert.ToInt32(cmd.Table.Rows[index]["killboss"].ToString());
			dbRoleInfo.BattleNameStart = Convert.ToInt64(cmd.Table.Rows[index]["battlenamestart"].ToString());
			dbRoleInfo.BattleNameIndex = Convert.ToInt32(cmd.Table.Rows[index]["battlenameindex"].ToString());
			dbRoleInfo.CZTaskID = Convert.ToInt32(cmd.Table.Rows[index]["cztaskid"].ToString());
			dbRoleInfo.BattleNum = Convert.ToInt32(cmd.Table.Rows[index]["battlenum"].ToString());
			dbRoleInfo.HeroIndex = Convert.ToInt32(cmd.Table.Rows[index]["heroindex"].ToString());
			dbRoleInfo.LoginDayID = Convert.ToInt32(cmd.Table.Rows[index]["logindayid"].ToString());
			dbRoleInfo.LoginDayNum = Convert.ToInt32(cmd.Table.Rows[index]["logindaynum"].ToString());
			dbRoleInfo.ZoneID = Convert.ToInt32(cmd.Table.Rows[index]["zoneid"].ToString());
			dbRoleInfo.BHName = cmd.Table.Rows[index]["bhname"].ToString();
			dbRoleInfo.BHVerify = Convert.ToInt32(cmd.Table.Rows[index]["bhverify"].ToString());
			dbRoleInfo.BHZhiWu = Convert.ToInt32(cmd.Table.Rows[index]["bhzhiwu"].ToString());
			dbRoleInfo.BGDayID1 = Convert.ToInt32(cmd.Table.Rows[index]["bgdayid1"].ToString());
			dbRoleInfo.BGMoney = Convert.ToInt32(cmd.Table.Rows[index]["bgmoney"].ToString());
			dbRoleInfo.BGDayID2 = Convert.ToInt32(cmd.Table.Rows[index]["bgdayid2"].ToString());
			dbRoleInfo.BGGoods = Convert.ToInt32(cmd.Table.Rows[index]["bggoods"].ToString());
			dbRoleInfo.BangGong = Convert.ToInt32(cmd.Table.Rows[index]["banggong"].ToString());
			dbRoleInfo.HuangHou = Convert.ToInt32(cmd.Table.Rows[index]["huanghou"].ToString());
			dbRoleInfo.JieBiaoDayID = Convert.ToInt32(cmd.Table.Rows[index]["jiebiaodayid"].ToString());
			dbRoleInfo.JieBiaoDayNum = Convert.ToInt32(cmd.Table.Rows[index]["jiebiaonum"].ToString());
			dbRoleInfo.UserName = cmd.Table.Rows[index]["username"].ToString();
			dbRoleInfo.LastMailID = Convert.ToInt32(cmd.Table.Rows[index]["lastmailid"].ToString());
			dbRoleInfo.OnceAwardFlag = Convert.ToInt64(cmd.Table.Rows[index]["onceawardflag"].ToString());
			dbRoleInfo.Gold = Convert.ToInt32(cmd.Table.Rows[index]["money2"].ToString());
			dbRoleInfo.BanChat = Convert.ToInt32(cmd.Table.Rows[index]["banchat"].ToString());
			dbRoleInfo.BanLogin = Convert.ToInt32(cmd.Table.Rows[index]["banlogin"].ToString());
			dbRoleInfo.IsFlashPlayer = Convert.ToInt32(cmd.Table.Rows[index]["isflashplayer"].ToString());
			dbRoleInfo.ChangeLifeCount = Convert.ToInt32(cmd.Table.Rows[index]["changelifecount"].ToString());
			dbRoleInfo.AdmiredCount = Convert.ToInt32(cmd.Table.Rows[index]["admiredcount"].ToString());
			dbRoleInfo.CombatForce = Convert.ToInt32(cmd.Table.Rows[index]["combatforce"].ToString());
			dbRoleInfo.AutoAssignPropertyPoint = Convert.ToInt32(cmd.Table.Rows[index]["autoassignpropertypoint"].ToString());
			dbRoleInfo.store_yinliang = Convert.ToInt64(cmd.Table.Rows[index]["store_yinliang"]);
			dbRoleInfo.store_money = Convert.ToInt64(cmd.Table.Rows[index]["store_money"]);
			dbRoleInfo.MagicSwordParam = Convert.ToInt32(cmd.Table.Rows[index]["magic_sword_param"]);
			dbRoleInfo.FluorescentPoint = Convert.ToInt32(cmd.Table.Rows[index]["fluorescent_point"]);
			dbRoleInfo.BanTradeToTicks = Convert.ToInt64(cmd.Table.Rows[index]["ban_trade_to_ticks"].ToString());
			dbRoleInfo.JunTuanZhiWu = Convert.ToInt32(cmd.Table.Rows[index]["juntuanzhiwu"]);
			dbRoleInfo.HuiJiData.huiji = Convert.ToInt32(cmd.Table.Rows[index]["huiji"]);
			dbRoleInfo.HuiJiData.Exp = Convert.ToInt32(cmd.Table.Rows[index]["huijiexp"]);
			dbRoleInfo.ArmorData.Armor = Convert.ToInt32(cmd.Table.Rows[index]["armor"]);
			dbRoleInfo.ArmorData.Exp = Convert.ToInt32(cmd.Table.Rows[index]["armorexp"]);
			dbRoleInfo.BianShenData.BianShen = Convert.ToInt32(cmd.Table.Rows[index]["bianshen"]);
			dbRoleInfo.BianShenData.Exp = Convert.ToInt32(cmd.Table.Rows[index]["bianshenexp"]);
			dbRoleInfo.RebornBagNum = Convert.ToInt32(cmd.Table.Rows[index]["reborn_bagnum"]);
			dbRoleInfo.RebornShowEquip = Convert.ToInt32(cmd.Table.Rows[index]["reborn_isshow"]);
			dbRoleInfo.RebornShowModel = Convert.ToInt32(cmd.Table.Rows[index]["reborn_isshow_model"]);
			dbRoleInfo.ZhanDuiID = Convert.ToInt32(cmd.Table.Rows[index]["zhanduiid"]);
			dbRoleInfo.ZhanDuiZhiWu = Convert.ToInt32(cmd.Table.Rows[index]["zhanduizhiwu"]);
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00018C84 File Offset: 0x00016E84
		public static void DBTableRow2RoleInfo_Params(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd, bool normalOnly)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<string, RoleParamsData> dict = dbRoleInfo.RoleParamsDict;
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					RoleParamsData roleParamsData = new RoleParamsData
					{
						ParamName = cmd.Table.Rows[i]["pname"].ToString(),
						ParamValue = cmd.Table.Rows[i]["pvalue"].ToString()
					};
					roleParamsData.ParamType = RoleParamNameInfo.GetRoleParamType(roleParamsData.ParamName, roleParamsData.ParamValue);
					if (roleParamsData.ParamType.Type <= 0 || !normalOnly)
					{
						dict[roleParamsData.ParamName] = roleParamsData;
					}
				}
			}
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x00018D7C File Offset: 0x00016F7C
		public static void DBTableRow2RoleInfo_ParamsEx(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<string, RoleParamsData> dict = dbRoleInfo.RoleParamsDict;
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int idx = Convert.ToInt32(cmd.Table.Rows[i]["idx"].ToString());
					int columnCount = cmd.Table.Rows[i].ItemArray.Length;
					for (int columnIndex = 2; columnIndex < columnCount; columnIndex++)
					{
						RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType(idx, columnIndex - 2);
						if (null != roleParamType)
						{
							RoleParamsData roleParamsData = new RoleParamsData
							{
								ParamName = roleParamType.ParamName,
								ParamValue = cmd.Table.Rows[i][columnIndex].ToString(),
								ParamType = roleParamType
							};
							dict[roleParamsData.ParamName] = roleParamsData;
						}
					}
				}
			}
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00018EA4 File Offset: 0x000170A4
		public static void InitFromRoleParams(DBRoleInfo dbRoleInfo)
		{
			string str = Global.GetRoleParamByName(dbRoleInfo, "20017");
			if (!string.IsNullOrEmpty(str))
			{
				string[] ids = str.Split(new char[]
				{
					'$'
				});
				foreach (string s in ids)
				{
					int occu;
					if (int.TryParse(s, out occu) && !dbRoleInfo.OccupationList.Contains(occu))
					{
						dbRoleInfo.OccupationList.Add(occu);
					}
				}
			}
			if (!dbRoleInfo.OccupationList.Contains(dbRoleInfo.Occupation))
			{
				dbRoleInfo.OccupationList.Insert(0, dbRoleInfo.Occupation);
			}
			str = Global.GetRoleParamByName(dbRoleInfo, "10213");
			if (!string.IsNullOrEmpty(str))
			{
				dbRoleInfo.SubOccupation = Global.SafeConvertToInt32(str, 10);
			}
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00018F8C File Offset: 0x0001718C
		public static void DBTableRow2RoleInfo_OldTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				List<OldTaskData> oldTasks = new List<OldTaskData>(cmd.Table.Rows.Count);
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					oldTasks.Add(new OldTaskData
					{
						TaskID = Convert.ToInt32(cmd.Table.Rows[i]["taskid"].ToString()),
						DoCount = Convert.ToInt32(cmd.Table.Rows[i]["count"].ToString())
					});
				}
				dbRoleInfo.OldTasks = oldTasks;
			}
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00019060 File Offset: 0x00017260
		public static void DBTableRow2RoleInfo_DoingTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.DoingTaskList = new List<TaskData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.DoingTaskList.Add(new TaskData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["id"].ToString()),
						DoingTaskID = Convert.ToInt32(cmd.Table.Rows[i]["taskid"].ToString()),
						DoingTaskVal1 = Convert.ToInt32(cmd.Table.Rows[i]["value1"].ToString()),
						DoingTaskVal2 = Convert.ToInt32(cmd.Table.Rows[i]["value2"].ToString()),
						DoingTaskFocus = Convert.ToInt32(cmd.Table.Rows[i]["focus"].ToString()),
						AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
						StarLevel = Convert.ToInt32(cmd.Table.Rows[i]["starlevel"].ToString())
					});
				}
			}
		}

		// Token: 0x060003DA RID: 986 RVA: 0x00019200 File Offset: 0x00017400
		public static void DBTableRow2RoleInfo_Goods(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.GoodsDataList = new List<GoodsData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					GoodsData goodsData = new GoodsData
					{
						Id = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						GoodsID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString()),
						Using = Convert.ToInt32(cmd.Table.Rows[i]["isusing"].ToString()),
						Forge_level = Convert.ToInt32(cmd.Table.Rows[i]["forge_level"].ToString()),
						Starttime = cmd.Table.Rows[i]["starttime"].ToString(),
						Endtime = cmd.Table.Rows[i]["endtime"].ToString(),
						Site = Convert.ToInt32(cmd.Table.Rows[i]["site"].ToString()),
						Quality = Convert.ToInt32(cmd.Table.Rows[i]["quality"].ToString()),
						Props = cmd.Table.Rows[i]["Props"].ToString(),
						GCount = Convert.ToInt32(cmd.Table.Rows[i]["gcount"].ToString()),
						Binding = Convert.ToInt32(cmd.Table.Rows[i]["binding"].ToString()),
						Jewellist = cmd.Table.Rows[i]["jewellist"].ToString(),
						BagIndex = Convert.ToInt32(cmd.Table.Rows[i]["bagindex"].ToString()),
						SaleMoney1 = Convert.ToInt32(cmd.Table.Rows[i]["salemoney1"].ToString()),
						SaleYuanBao = Convert.ToInt32(cmd.Table.Rows[i]["saleyuanbao"].ToString()),
						SaleYinPiao = Convert.ToInt32(cmd.Table.Rows[i]["saleyinpiao"].ToString()),
						AddPropIndex = Convert.ToInt32(cmd.Table.Rows[i]["addpropindex"].ToString()),
						BornIndex = Convert.ToInt32(cmd.Table.Rows[i]["bornindex"].ToString()),
						Lucky = Convert.ToInt32(cmd.Table.Rows[i]["lucky"].ToString()),
						Strong = Convert.ToInt32(cmd.Table.Rows[i]["strong"].ToString()),
						ExcellenceInfo = Convert.ToInt32(cmd.Table.Rows[i]["excellenceinfo"].ToString()),
						AppendPropLev = Convert.ToInt32(cmd.Table.Rows[i]["appendproplev"].ToString()),
						ChangeLifeLevForEquip = Convert.ToInt32(cmd.Table.Rows[i]["equipchangelife"].ToString()),
						JuHunID = Convert.ToInt32(cmd.Table.Rows[i]["juhun"].ToString())
					};
					string washpropsStr = cmd.Table.Rows[i]["washprops"].ToString();
					if (!string.IsNullOrEmpty(washpropsStr))
					{
						try
						{
							byte[] props = Convert.FromBase64String(washpropsStr);
							goodsData.WashProps = DataHelper.BytesToObject<List<int>>(props, 0, props.Length);
						}
						catch
						{
						}
					}
					string ehinfopropsStr = cmd.Table.Rows[i]["ehinfo"].ToString();
					if (!string.IsNullOrEmpty(ehinfopropsStr))
					{
						try
						{
							byte[] props = Convert.FromBase64String(ehinfopropsStr);
							goodsData.ElementhrtsProps = DataHelper.BytesToObject<List<int>>(props, 0, props.Length);
						}
						catch
						{
						}
					}
					dbRoleInfo.GoodsDataList.Add(goodsData);
				}
			}
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00019748 File Offset: 0x00017948
		public static void DBTableRow2RoleInfo_GoodsLimit(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.GoodsLimitDataList = new List<GoodsLimitData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.GoodsLimitDataList.Add(new GoodsLimitData
					{
						GoodsID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString()),
						DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
						UsedNum = Convert.ToInt32(cmd.Table.Rows[i]["usednum"].ToString())
					});
				}
			}
		}

		// Token: 0x060003DC RID: 988 RVA: 0x0001983C File Offset: 0x00017A3C
		public static void DBTableRow2RoleInfo_Friends(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.FriendDataList = new List<FriendData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.FriendDataList.Add(new FriendData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						OtherRoleID = Convert.ToInt32(cmd.Table.Rows[i]["otherid"].ToString()),
						FriendType = Convert.ToInt32(cmd.Table.Rows[i]["friendType"].ToString())
					});
				}
			}
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00019930 File Offset: 0x00017B30
		public static void DBTableRow2RoleInfo_Horses(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.HorsesDataList = new List<HorseData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.HorsesDataList.Add(new HorseData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						HorseID = Convert.ToInt32(cmd.Table.Rows[i]["horseid"].ToString()),
						BodyID = Convert.ToInt32(cmd.Table.Rows[i]["bodyid"].ToString()),
						PropsNum = cmd.Table.Rows[i]["propsNum"].ToString(),
						PropsVal = cmd.Table.Rows[i]["PropsVal"].ToString(),
						AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
						JinJieFailedNum = Convert.ToInt32(cmd.Table.Rows[i]["failednum"].ToString()),
						JinJieTempTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["temptime"].ToString()),
						JinJieTempNum = Convert.ToInt32(cmd.Table.Rows[i]["tempnum"].ToString()),
						JinJieFailedDayID = Convert.ToInt32(cmd.Table.Rows[i]["faileddayid"].ToString())
					});
				}
			}
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00019B48 File Offset: 0x00017D48
		public static void DBTableRow2RoleInfo_Pets(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.PetsDataList = new List<PetData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.PetsDataList.Add(new PetData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						PetID = Convert.ToInt32(cmd.Table.Rows[i]["petid"].ToString()),
						PetName = cmd.Table.Rows[i]["petname"].ToString(),
						PetType = Convert.ToInt32(cmd.Table.Rows[i]["pettype"].ToString()),
						FeedNum = Convert.ToInt32(cmd.Table.Rows[i]["feednum"].ToString()),
						ReAliveNum = Convert.ToInt32(cmd.Table.Rows[i]["realivenum"].ToString()),
						AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
						PetProps = cmd.Table.Rows[i]["props"].ToString(),
						Level = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString())
					});
				}
			}
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00019D34 File Offset: 0x00017F34
		public static void DBTableRow2RoleInfo_JingMais(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.JingMaiDataList = new List<JingMaiData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.JingMaiDataList.Add(new JingMaiData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						JingMaiID = Convert.ToInt32(cmd.Table.Rows[i]["jmid"].ToString()),
						JingMaiLevel = Convert.ToInt32(cmd.Table.Rows[i]["jmlevel"].ToString()),
						JingMaiBodyLevel = Convert.ToInt32(cmd.Table.Rows[i]["bodylevel"].ToString())
					});
				}
			}
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00019E54 File Offset: 0x00018054
		public static void DBTableRow2RoleInfo_Skills(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.SkillDataList = new List<SkillData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.SkillDataList.Add(new SkillData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						SkillID = Convert.ToInt32(cmd.Table.Rows[i]["skillid"].ToString()),
						SkillLevel = Convert.ToInt32(cmd.Table.Rows[i]["skilllevel"].ToString()),
						UsedNum = Convert.ToInt32(cmd.Table.Rows[i]["usednum"].ToString())
					});
				}
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x00019F74 File Offset: 0x00018174
		public static void DBTableRow2RoleInfo_Buffers(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.BufferDataList = new List<BufferData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.BufferDataList.Add(new BufferData
					{
						BufferID = Convert.ToInt32(cmd.Table.Rows[i]["bufferid"].ToString()),
						StartTime = Convert.ToInt64(cmd.Table.Rows[i]["starttime"].ToString()),
						BufferSecs = Convert.ToInt32(cmd.Table.Rows[i]["buffersecs"].ToString()),
						BufferVal = Convert.ToInt64(cmd.Table.Rows[i]["bufferval"].ToString()),
						BufferType = 0
					});
				}
			}
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x0001A09C File Offset: 0x0001829C
		public static void DBTableRow2RoleInfo_DailyTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (null == dbRoleInfo.MyDailyTaskDataList)
			{
				dbRoleInfo.MyDailyTaskDataList = new List<DailyTaskData>();
			}
			for (int i = 0; i < cmd.Table.Rows.Count; i++)
			{
				DailyTaskData dailyTaskData = new DailyTaskData
				{
					HuanID = Convert.ToInt32(cmd.Table.Rows[i]["huanid"].ToString()),
					RecTime = cmd.Table.Rows[i]["rectime"].ToString(),
					RecNum = Convert.ToInt32(cmd.Table.Rows[i]["recnum"].ToString()),
					TaskClass = Convert.ToInt32(cmd.Table.Rows[i]["taskClass"].ToString()),
					ExtDayID = Convert.ToInt32(cmd.Table.Rows[i]["extdayid"].ToString()),
					ExtNum = Convert.ToInt32(cmd.Table.Rows[i]["extnum"].ToString())
				};
				dbRoleInfo.MyDailyTaskDataList.Add(dailyTaskData);
			}
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x0001A204 File Offset: 0x00018404
		public static void DBTableRow2RoleInfo_DailyJingMai(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyDailyJingMaiData = new DailyJingMaiData
				{
					JmTime = cmd.Table.Rows[0]["jmtime"].ToString(),
					JmNum = Convert.ToInt32(cmd.Table.Rows[0]["jmnum"].ToString())
				};
			}
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x0001A290 File Offset: 0x00018490
		public static void DBTableRow2RoleInfo_PortableBag(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MyPortableBagData = new PortableBagData
			{
				GoodsUsedGridNum = 0
			};
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyPortableBagData.ExtGridNum = Convert.ToInt32(cmd.Table.Rows[0]["extgridnum"].ToString());
			}
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x0001A300 File Offset: 0x00018500
		public static void DBTableRow2RoleInfo_RebornPortableBag(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.RebornGirdData = new RebornPortableBagData
			{
				GoodsUsedGridNum = 0
			};
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.RebornGirdData.ExtGridNum = Convert.ToInt32(cmd.Table.Rows[0]["extgridnum"].ToString());
			}
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x0001A370 File Offset: 0x00018570
		public static void DBTableRow2RoleInfo_HuodongData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.ExistsMyHuodongData = false;
			dbRoleInfo.MyHuodongData = new HuodongData();
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.ExistsMyHuodongData = true;
				dbRoleInfo.MyHuodongData.LastWeekID = cmd.Table.Rows[0]["loginweekid"].ToString();
				dbRoleInfo.MyHuodongData.LastDayID = cmd.Table.Rows[0]["logindayid"].ToString();
				dbRoleInfo.MyHuodongData.LoginNum = Convert.ToInt32(cmd.Table.Rows[0]["loginnum"].ToString());
				dbRoleInfo.MyHuodongData.NewStep = Convert.ToInt32(cmd.Table.Rows[0]["newstep"].ToString());
				dbRoleInfo.MyHuodongData.StepTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["steptime"].ToString());
				dbRoleInfo.MyHuodongData.LastMTime = Convert.ToInt32(cmd.Table.Rows[0]["lastmtime"].ToString());
				dbRoleInfo.MyHuodongData.CurMID = cmd.Table.Rows[0]["curmid"].ToString();
				dbRoleInfo.MyHuodongData.CurMTime = Convert.ToInt32(cmd.Table.Rows[0]["curmtime"].ToString());
				dbRoleInfo.MyHuodongData.SongLiID = Convert.ToInt32(cmd.Table.Rows[0]["songliid"].ToString());
				dbRoleInfo.MyHuodongData.LoginGiftState = Convert.ToInt32(cmd.Table.Rows[0]["logingiftstate"].ToString());
				dbRoleInfo.MyHuodongData.OnlineGiftState = Convert.ToInt32(cmd.Table.Rows[0]["onlinegiftstate"].ToString());
				dbRoleInfo.MyHuodongData.LastLimitTimeHuoDongID = Convert.ToInt32(cmd.Table.Rows[0]["lastlimittimehuodongid"].ToString());
				dbRoleInfo.MyHuodongData.LastLimitTimeDayID = Convert.ToInt32(cmd.Table.Rows[0]["lastlimittimedayid"].ToString());
				dbRoleInfo.MyHuodongData.LimitTimeLoginNum = Convert.ToInt32(cmd.Table.Rows[0]["limittimeloginnum"].ToString());
				dbRoleInfo.MyHuodongData.LimitTimeGiftState = Convert.ToInt32(cmd.Table.Rows[0]["limittimegiftstate"].ToString());
				dbRoleInfo.MyHuodongData.EveryDayOnLineAwardStep = Convert.ToInt32(cmd.Table.Rows[0]["everydayonlineawardstep"].ToString());
				dbRoleInfo.MyHuodongData.GetEveryDayOnLineAwardDayID = Convert.ToInt32(cmd.Table.Rows[0]["geteverydayonlineawarddayid"].ToString());
				dbRoleInfo.MyHuodongData.SeriesLoginGetAwardStep = Convert.ToInt32(cmd.Table.Rows[0]["serieslogingetawardstep"].ToString());
				dbRoleInfo.MyHuodongData.SeriesLoginAwardDayID = Convert.ToInt32(cmd.Table.Rows[0]["seriesloginawarddayid"].ToString());
				dbRoleInfo.MyHuodongData.SeriesLoginAwardGoodsID = cmd.Table.Rows[0]["seriesloginawardgoodsid"].ToString();
				dbRoleInfo.MyHuodongData.EveryDayOnLineAwardGoodsID = cmd.Table.Rows[0]["everydayonlineawardgoodsid"].ToString();
			}
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x0001A790 File Offset: 0x00018990
		public static void DBTableRow2RoleInfo_FuBenData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.FuBenDataList = new List<FuBenData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.FuBenDataList.Add(new FuBenData
					{
						FuBenID = Convert.ToInt32(cmd.Table.Rows[i]["fubenid"].ToString()),
						DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
						EnterNum = Convert.ToInt32(cmd.Table.Rows[i]["enternum"].ToString()),
						QuickPassTimer = Convert.ToInt32(cmd.Table.Rows[i]["quickpasstimer"].ToString()),
						FinishNum = Convert.ToInt32(cmd.Table.Rows[i]["finishnum"].ToString())
					});
				}
			}
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x0001A8DC File Offset: 0x00018ADC
		public static void DBTableRow2RoleInfo_HolyItemData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MyHolyItemDataDic = new Dictionary<sbyte, HolyItemData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					HolyItemData data = null;
					sbyte sShengwuType = Convert.ToSByte(cmd.Table.Rows[i]["shengwu_type"].ToString());
					sbyte sPartSlot = Convert.ToSByte(cmd.Table.Rows[i]["part_slot"].ToString());
					bool bFind = dbRoleInfo.MyHolyItemDataDic.TryGetValue(sShengwuType, out data);
					if (!bFind)
					{
						data = new HolyItemData();
					}
					data.m_sType = sShengwuType;
					HolyItemPartData partdata = null;
					if (!data.m_PartArray.TryGetValue(sPartSlot, out partdata))
					{
						partdata = new HolyItemPartData();
						data.m_PartArray.Add(sPartSlot, partdata);
					}
					partdata.m_sSuit = Convert.ToSByte(cmd.Table.Rows[i]["part_suit"].ToString());
					partdata.m_nSlice = Convert.ToInt32(cmd.Table.Rows[i]["part_slice"].ToString());
					if (!bFind)
					{
						dbRoleInfo.MyHolyItemDataDic.Add(sShengwuType, data);
					}
				}
			}
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x0001AA50 File Offset: 0x00018C50
		public static void DBTableRow2RoleInfo_TarotData(MySQLConnection connection, DBRoleInfo dbRoleInfo, int roleId)
		{
			dbRoleInfo.TarotData = new TarotSystemData();
			string str = string.Format("SELECT tarotinfo,kingbuff FROM t_tarot where roleid={0}", roleId);
			GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", str), EventLevels.Important);
			try
			{
				MySQLCommand command = new MySQLCommand(str, connection);
				MySQLDataReader reader = command.ExecuteReaderEx();
				while (reader.Read())
				{
					string str2 = Encoding.UTF8.GetString(reader["tarotinfo"] as byte[]);
					string str3 = Encoding.UTF8.GetString(reader["kingbuff"] as byte[]);
					string[] strArray = str2.Split(new char[]
					{
						';'
					}, StringSplitOptions.RemoveEmptyEntries);
					foreach (string str4 in strArray)
					{
						TarotCardData item = new TarotCardData(str4);
						dbRoleInfo.TarotData.TarotCardDatas.Add(item);
					}
					dbRoleInfo.TarotData.KingData = new TarotKingData(str3);
				}
				command.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0001AB90 File Offset: 0x00018D90
		public static void DBTableRow2RoleInfo_MarriageData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyMarriageData = new MarriageData
				{
					nSpouseID = Convert.ToInt32(cmd.Table.Rows[0]["spouseid"].ToString()),
					byMarrytype = Convert.ToSByte(cmd.Table.Rows[0]["marrytype"].ToString()),
					nRingID = Convert.ToInt32(cmd.Table.Rows[0]["ringid"].ToString()),
					nGoodwillexp = Convert.ToInt32(cmd.Table.Rows[0]["goodwillexp"].ToString()),
					byGoodwillstar = Convert.ToSByte(cmd.Table.Rows[0]["goodwillstar"].ToString()),
					byGoodwilllevel = Convert.ToSByte(cmd.Table.Rows[0]["goodwilllevel"].ToString()),
					nGivenrose = Convert.ToInt32(cmd.Table.Rows[0]["givenrose"].ToString()),
					strLovemessage = cmd.Table.Rows[0]["lovemessage"].ToString(),
					byAutoReject = Convert.ToSByte(cmd.Table.Rows[0]["autoreject"].ToString()),
					ChangTime = cmd.Table.Rows[0]["changtime"].ToString()
				};
			}
			else
			{
				dbRoleInfo.MyMarriageData = new MarriageData();
			}
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x0001AD80 File Offset: 0x00018F80
		public static void DBTableRow2RoleInfo_MarryPartyJoinList(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MyMarryPartyJoinList = new Dictionary<int, int>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.MyMarryPartyJoinList.Add(Convert.ToInt32(cmd.Table.Rows[i]["partyroleid"].ToString()), Convert.ToInt32(cmd.Table.Rows[i]["joincount"].ToString()));
				}
			}
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x0001AE30 File Offset: 0x00019030
		public static void DBTableRow2RoleInfo_DailyData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyRoleDailyData = new RoleDailyData
				{
					ExpDayID = Convert.ToInt32(cmd.Table.Rows[0]["expdayid"].ToString()),
					TodayExp = Convert.ToInt32(cmd.Table.Rows[0]["todayexp"].ToString()),
					LingLiDayID = Convert.ToInt32(cmd.Table.Rows[0]["linglidayid"].ToString()),
					TodayLingLi = Convert.ToInt32(cmd.Table.Rows[0]["todaylingli"].ToString()),
					KillBossDayID = Convert.ToInt32(cmd.Table.Rows[0]["killbossdayid"].ToString()),
					TodayKillBoss = Convert.ToInt32(cmd.Table.Rows[0]["todaykillboss"].ToString()),
					FuBenDayID = Convert.ToInt32(cmd.Table.Rows[0]["fubendayid"].ToString()),
					TodayFuBenNum = Convert.ToInt32(cmd.Table.Rows[0]["todayfubennum"].ToString()),
					WuXingDayID = Convert.ToInt32(cmd.Table.Rows[0]["wuxingdayid"].ToString()),
					WuXingNum = Convert.ToInt32(cmd.Table.Rows[0]["wuxingnum"].ToString()),
					RebornExpDayID = Convert.ToInt32(cmd.Table.Rows[0]["reborndayid"].ToString()),
					RebornExpMonster = Convert.ToInt32(cmd.Table.Rows[0]["rebornexpmonster"].ToString()),
					RebornExpSale = Convert.ToInt32(cmd.Table.Rows[0]["rebornexpsale"].ToString())
				};
			}
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x0001B09C File Offset: 0x0001929C
		public static void DBTableRow2RoleInfo_YaBiaoData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyYaBiaoData = new YaBiaoData
				{
					YaBiaoID = Convert.ToInt32(cmd.Table.Rows[0]["yabiaoid"].ToString()),
					StartTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["starttime"].ToString()),
					State = Convert.ToInt32(cmd.Table.Rows[0]["state"].ToString()),
					LineID = Convert.ToInt32(cmd.Table.Rows[0]["lineid"].ToString()),
					TouBao = Convert.ToInt32(cmd.Table.Rows[0]["toubao"].ToString()),
					YaBiaoDayID = Convert.ToInt32(cmd.Table.Rows[0]["yabiaodayid"].ToString()),
					YaBiaoNum = Convert.ToInt32(cmd.Table.Rows[0]["yabiaonum"].ToString()),
					TakeGoods = Convert.ToInt32(cmd.Table.Rows[0]["takegoods"].ToString())
				};
			}
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x0001B230 File Offset: 0x00019430
		public static void DBTableRow2RoleInfo_VipDailyData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.VipDailyDataList = new List<VipDailyData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.VipDailyDataList.Add(new VipDailyData
					{
						PriorityType = Convert.ToInt32(cmd.Table.Rows[i]["prioritytype"].ToString()),
						DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
						UsedTimes = Convert.ToInt32(cmd.Table.Rows[i]["usedtimes"].ToString())
					});
				}
			}
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x0001B324 File Offset: 0x00019524
		public static void DBTableRow2RoleInfo_YangGongBKDailyJiFenData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.YangGongBKDailyJiFen = new YangGongBKDailyJiFenData
				{
					JiFen = Convert.ToInt32(cmd.Table.Rows[0]["jifen"].ToString()),
					DayID = Convert.ToInt32(cmd.Table.Rows[0]["dayid"].ToString()),
					AwardHistory = Convert.ToInt64(cmd.Table.Rows[0]["awardhistory"].ToString())
				};
			}
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x0001B3E0 File Offset: 0x000195E0
		public static void DBTableRow2RoleInfo_Wings(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyWingData = new WingData
				{
					DbID = Convert.ToInt32(cmd.Table.Rows[0]["Id"].ToString()),
					WingID = Convert.ToInt32(cmd.Table.Rows[0]["wingid"].ToString()),
					ForgeLevel = Convert.ToInt32(cmd.Table.Rows[0]["forgeLevel"].ToString()),
					AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["addtime"].ToString()),
					JinJieFailedNum = Convert.ToInt32(cmd.Table.Rows[0]["failednum"].ToString()),
					Using = Convert.ToInt32(cmd.Table.Rows[0]["equiped"].ToString()),
					StarExp = Convert.ToInt32(cmd.Table.Rows[0]["starexp"].ToString()),
					ZhuLingNum = Convert.ToInt32(cmd.Table.Rows[0]["zhulingnum"].ToString()),
					ZhuHunNum = Convert.ToInt32(cmd.Table.Rows[0]["zhuhunnum"].ToString())
				};
			}
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0001B5A0 File Offset: 0x000197A0
		public static void DBTableRow2RoleInfo_picturejudgeinfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<int, int> Tmpdict = new Dictionary<int, int>(cmd.Table.Rows.Count);
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int nID = Convert.ToInt32(cmd.Table.Rows[i]["picturejudgeid"].ToString());
					int nNum = Convert.ToInt32(cmd.Table.Rows[i]["refercount"].ToString());
					Tmpdict[nID] = nNum;
				}
				dbRoleInfo.PictureJudgeReferInfo = Tmpdict;
			}
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0001B66C File Offset: 0x0001986C
		public static void DBTableRow2RoleInfo_starconstellationinfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<int, int> Tmpdict = new Dictionary<int, int>(cmd.Table.Rows.Count);
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int nStarsiteid = Convert.ToInt32(cmd.Table.Rows[i]["starsiteid"].ToString());
					int nStarslotid = Convert.ToInt32(cmd.Table.Rows[i]["starslotid"].ToString());
					Tmpdict[nStarsiteid] = nStarslotid;
				}
				dbRoleInfo.StarConstellationInfo = Tmpdict;
			}
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x0001B738 File Offset: 0x00019938
		public static void DBTableRow2RoleInfo_LingYuInfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.LingYuDict = new Dictionary<int, LingYuData>();
			for (int i = 0; i < cmd.Table.Rows.Count; i++)
			{
				LingYuData lyData = new LingYuData();
				lyData.Type = Convert.ToInt32(cmd.Table.Rows[i]["type"].ToString());
				lyData.Level = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString());
				lyData.Suit = Convert.ToInt32(cmd.Table.Rows[i]["suit"].ToString());
				dbRoleInfo.LingYuDict[lyData.Type] = lyData;
			}
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0001B814 File Offset: 0x00019A14
		public static void DBTableRow2RoleInfo_GuardStatue(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				if (dbRoleInfo.MyGuardStatueDetail == null)
				{
					dbRoleInfo.MyGuardStatueDetail = new GuardStatueDetail();
					dbRoleInfo.MyGuardStatueDetail.IsActived = true;
				}
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.Level = Convert.ToInt32(cmd.Table.Rows[0]["level"].ToString());
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.Suit = Convert.ToInt32(cmd.Table.Rows[0]["suit"].ToString());
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.HasGuardPoint = Convert.ToInt32(cmd.Table.Rows[0]["total_guard_point"].ToString());
				dbRoleInfo.MyGuardStatueDetail.ActiveSoulSlot = Convert.ToInt32(cmd.Table.Rows[0]["slot_cnt"].ToString());
				dbRoleInfo.MyGuardStatueDetail.LastdayRecoverPoint = Convert.ToInt32(cmd.Table.Rows[0]["lastday_recover_point"].ToString());
				dbRoleInfo.MyGuardStatueDetail.LastdayRecoverOffset = Convert.ToInt32(cmd.Table.Rows[0]["lastday_recover_offset"].ToString());
			}
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0001B99C File Offset: 0x00019B9C
		public static void DBTableRow2RoleInfo_GuardSoul(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				if (dbRoleInfo.MyGuardStatueDetail == null)
				{
					dbRoleInfo.MyGuardStatueDetail = new GuardStatueDetail();
					dbRoleInfo.MyGuardStatueDetail.IsActived = true;
				}
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.GuardSoulList.Clear();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					GuardSoulData data = new GuardSoulData();
					data.Type = Convert.ToInt32(cmd.Table.Rows[i]["soul_type"].ToString());
					data.EquipSlot = Convert.ToInt32(cmd.Table.Rows[i]["equip_slot"].ToString());
					dbRoleInfo.MyGuardStatueDetail.GuardStatue.GuardSoulList.Add(data);
				}
			}
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x0001BACC File Offset: 0x00019CCC
		public static int QueryRoleID_ByRolename(MySQLConnection conn, string strRoleName)
		{
			List<Tuple<int, string>> idList = DBRoleInfo.QueryRoleIdList_ByRolename_IgnoreDbCmp(conn, strRoleName);
			int roleId = -1;
			if (idList != null)
			{
				Tuple<int, string> tuple = idList.Find((Tuple<int, string> _t) => _t.Item2 == strRoleName);
				roleId = ((tuple != null) ? tuple.Item1 : -1);
			}
			return roleId;
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0001BB38 File Offset: 0x00019D38
		public static List<Tuple<int, string>> QueryRoleIdList_ByRolename_IgnoreDbCmp(MySQLConnection conn, string rolename)
		{
			List<Tuple<int, string>> resultList = new List<Tuple<int, string>>();
			string sql = string.Format("SELECT rid,rname FROM t_roles where rname='{0}'", rolename);
			GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
			MySQLCommand cmd = new MySQLCommand(sql, conn);
			MySQLDataReader reader = cmd.ExecuteReaderEx();
			while (reader.Read())
			{
				int oneRoleId = Convert.ToInt32(reader["rid"].ToString());
				string oneRolename = reader["rname"].ToString();
				resultList.Add(new Tuple<int, string>(oneRoleId, oneRolename));
			}
			cmd.Dispose();
			return resultList;
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x0001BBD8 File Offset: 0x00019DD8
		public static void DBTableRow2RoleInfo_GMailInfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.GroupMailRecordList = new List<int>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.GroupMailRecordList.Add(Convert.ToInt32(cmd.Table.Rows[i]["gmailid"].ToString()));
				}
			}
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0001BC60 File Offset: 0x00019E60
		public static void DBTableRow2RoleInfo_TalentBase(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyTalentData = new TalentData
				{
					IsOpen = true,
					TotalCount = Convert.ToInt32(cmd.Table.Rows[0]["tatalCount"].ToString()),
					Exp = Convert.ToInt64(cmd.Table.Rows[0]["exp"].ToString())
				};
			}
			else
			{
				dbRoleInfo.MyTalentData = new TalentData();
			}
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x0001BD08 File Offset: 0x00019F08
		public static void DBTableRow2RoleInfo_TalentEffects(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			Dictionary<int, int> countList = new Dictionary<int, int>();
			countList.Add(1, 0);
			countList.Add(2, 0);
			countList.Add(3, 0);
			dbRoleInfo.MyTalentData.EffectList = new List<TalentEffectItem>();
			if (dbRoleInfo.MyTalentData != null && cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int type = Convert.ToInt32(cmd.Table.Rows[i]["talentType"].ToString());
					int id = Convert.ToInt32(cmd.Table.Rows[i]["effectID"].ToString());
					int level = Convert.ToInt32(cmd.Table.Rows[i]["effectLevel"].ToString());
					TalentEffectItem item = new TalentEffectItem
					{
						ID = id,
						Level = level,
						TalentType = type
					};
					dbRoleInfo.MyTalentData.EffectList.Add(item);
					Dictionary<int, int> dictionary;
					int key;
					(dictionary = countList)[key = type] = dictionary[key] + level;
				}
			}
			dbRoleInfo.MyTalentData.CountList = countList;
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0001BE78 File Offset: 0x0001A078
		public static void DBTableRow2RoleInfo_TianTiData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			RoleTianTiData roleTianTiData = new RoleTianTiData
			{
				RoleId = dbRoleInfo.RoleID
			};
			dbRoleInfo.TianTiData = roleTianTiData;
			if (cmd.Table.Rows.Count > 0)
			{
				roleTianTiData.DuanWeiId = Convert.ToInt32(cmd.Table.Rows[0]["duanweiid"].ToString());
				roleTianTiData.DuanWeiJiFen = Convert.ToInt32(cmd.Table.Rows[0]["duanweijifen"].ToString());
				roleTianTiData.DuanWeiRank = Convert.ToInt32(cmd.Table.Rows[0]["duanweirank"].ToString());
				roleTianTiData.LianSheng = Convert.ToInt32(cmd.Table.Rows[0]["liansheng"].ToString());
				roleTianTiData.FightCount = Convert.ToInt32(cmd.Table.Rows[0]["fightcount"].ToString());
				roleTianTiData.SuccessCount = Convert.ToInt32(cmd.Table.Rows[0]["successcount"].ToString());
				roleTianTiData.TodayFightCount = Convert.ToInt32(cmd.Table.Rows[0]["todayfightcount"].ToString());
				roleTianTiData.LastFightDayId = Convert.ToInt32(cmd.Table.Rows[0]["lastfightdayid"].ToString());
				roleTianTiData.MonthDuanWeiRank = Convert.ToInt32(cmd.Table.Rows[0]["monthduanweirank"].ToString());
				DateTime.TryParse(cmd.Table.Rows[0]["fetchmonthawarddate"].ToString(), out roleTianTiData.FetchMonthDuanWeiRankAwardsTime);
				roleTianTiData.RongYao = Convert.ToInt32(cmd.Table.Rows[0]["rongyao"].ToString());
			}
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x0001C09C File Offset: 0x0001A29C
		public static void DBTableRow2RoleInfo_MerlinData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (null == dbRoleInfo.MerlinData)
			{
				dbRoleInfo.MerlinData = new MerlinGrowthSaveDBData();
				for (int i = 0; i < 4; i++)
				{
					dbRoleInfo.MerlinData._ActiveAttr[i] = 0.0;
					dbRoleInfo.MerlinData._UnActiveAttr[i] = 0.0;
				}
			}
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MerlinData._RoleID = Global.SafeConvertToInt32(cmd.Table.Rows[0]["roleID"].ToString(), 10);
				dbRoleInfo.MerlinData._Occupation = Global.SafeConvertToInt32(cmd.Table.Rows[0]["occupation"].ToString(), 10);
				dbRoleInfo.MerlinData._Level = Global.SafeConvertToInt32(cmd.Table.Rows[0]["level"].ToString(), 10);
				dbRoleInfo.MerlinData._LevelUpFailNum = Global.SafeConvertToInt32(cmd.Table.Rows[0]["level_up_fail_num"].ToString(), 10);
				dbRoleInfo.MerlinData._StarNum = Global.SafeConvertToInt32(cmd.Table.Rows[0]["starNum"].ToString(), 10);
				dbRoleInfo.MerlinData._StarExp = Global.SafeConvertToInt32(cmd.Table.Rows[0]["starExp"].ToString(), 10);
				dbRoleInfo.MerlinData._LuckyPoint = Global.SafeConvertToInt32(cmd.Table.Rows[0]["luckyPoint"].ToString(), 10);
				dbRoleInfo.MerlinData._ToTicks = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["toTicks"].ToString());
				dbRoleInfo.MerlinData._AddTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["addTime"].ToString());
				dbRoleInfo.MerlinData._ActiveAttr[0] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activeFrozen"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._ActiveAttr[1] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activePalsy"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._ActiveAttr[2] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activeSpeedDown"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._ActiveAttr[3] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activeBlow"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[0] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActiveFrozen"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[1] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActivePalsy"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[2] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActiveSpeedDown"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[3] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActiveBlow"].ToString(), 10) / 100);
			}
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x0001C4D8 File Offset: 0x0001A6D8
		public static void DBTableRow2RoleInfo_FluorescentGemData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (null == dbRoleInfo.FluorescentGemData)
			{
				dbRoleInfo.FluorescentGemData = new FluorescentGemData();
			}
			dbRoleInfo.FluorescentGemData.GemBagList.Clear();
			dbRoleInfo.FluorescentGemData.GemEquipList.Clear();
			HashSet<int> repeatedSet = new HashSet<int>();
			FluorescentGemSaveDBData data = new FluorescentGemSaveDBData();
			for (int i = 0; i < cmd.Table.Rows.Count; i++)
			{
				ulong id = Convert.ToUInt64(cmd.Table.Rows[i]["id"].ToString());
				data._RoleID = Global.SafeConvertToInt32(cmd.Table.Rows[i]["roleid"].ToString(), 10);
				data._GoodsID = Global.SafeConvertToInt32(cmd.Table.Rows[i]["goodsid"].ToString(), 10);
				data._Position = Global.SafeConvertToInt32(cmd.Table.Rows[i]["position"].ToString(), 10);
				data._GemType = Global.SafeConvertToInt32(cmd.Table.Rows[i]["type"].ToString(), 10);
				data._Bind = Global.SafeConvertToInt32(cmd.Table.Rows[i]["bind"].ToString(), 10);
				int slot = FluorescentGemManager.getInstance().GenerateBagIndex(data._Position, data._GemType);
				if (!repeatedSet.Contains(slot))
				{
					GoodsData tmpGoods = new GoodsData();
					tmpGoods.GoodsID = data._GoodsID;
					tmpGoods.GCount = 1;
					tmpGoods.Binding = data._Bind;
					tmpGoods.Site = 7001;
					repeatedSet.Add(slot);
					tmpGoods.BagIndex = slot;
					dbRoleInfo.FluorescentGemData.GemEquipList.Add(tmpGoods);
				}
				else
				{
					FluorescentGemDBOperate.ForceUnEquipFluorescentGem(DBManager.getInstance(), id);
					LogManager.WriteLog(LogTypes.Error, string.Format("荧光宝石装备栏位置重复，强制删除，rid={0}, goodsid={1}, pos={2}, type={3}, bind={4}", new object[]
					{
						data._RoleID,
						data._GoodsID,
						data._Position,
						data._GemType,
						data._Bind
					}), null, true);
				}
			}
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x0001C760 File Offset: 0x0001A960
		public static void DBTableRow2RoleInfo_BuildingData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.BuildingDataList = new List<BuildingData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					BuildingData myBuild = new BuildingData();
					myBuild.BuildId = Convert.ToInt32(cmd.Table.Rows[i]["buildid"].ToString());
					myBuild.TaskID_1 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_1"].ToString());
					myBuild.TaskID_2 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_2"].ToString());
					myBuild.TaskID_3 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_3"].ToString());
					myBuild.TaskID_4 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_4"].ToString());
					myBuild.BuildLev = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString());
					myBuild.BuildExp = Convert.ToInt32(cmd.Table.Rows[i]["exp"].ToString());
					myBuild.BuildTime = cmd.Table.Rows[i]["developtime"].ToString();
					dbRoleInfo.BuildingDataList.Add(myBuild);
				}
			}
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x0001C928 File Offset: 0x0001AB28
		public static void DBTableRow2RoleInfo_OrnamentData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.OrnamentDataDict = new Dictionary<int, OrnamentData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					OrnamentData myOrnament = new OrnamentData();
					myOrnament.ID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString());
					myOrnament.Param1 = Convert.ToInt32(cmd.Table.Rows[i]["param1"].ToString());
					myOrnament.Param2 = Convert.ToInt32(cmd.Table.Rows[i]["param2"].ToString());
					dbRoleInfo.OrnamentDataDict[myOrnament.ID] = myOrnament;
				}
			}
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x0001CA24 File Offset: 0x0001AC24
		public static void DBTableRow2RoleInfo_SevenDayActData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.SevenDayActDict = new Dictionary<int, Dictionary<int, SevenDayItemData>>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					SevenDayItemData itemData = new SevenDayItemData();
					itemData.AwardFlag = Convert.ToInt32(cmd.Table.Rows[i]["award_flag"].ToString());
					itemData.Params1 = Convert.ToInt32(cmd.Table.Rows[i]["param1"].ToString());
					itemData.Params2 = Convert.ToInt32(cmd.Table.Rows[i]["param2"].ToString());
					int roleid = Convert.ToInt32(cmd.Table.Rows[i]["roleid"].ToString());
					int actType = Convert.ToInt32(cmd.Table.Rows[i]["act_type"].ToString());
					int id = Convert.ToInt32(cmd.Table.Rows[i]["id"].ToString());
					Dictionary<int, SevenDayItemData> itemDict = null;
					if (!dbRoleInfo.SevenDayActDict.TryGetValue(actType, out itemDict))
					{
						itemDict = new Dictionary<int, SevenDayItemData>();
						dbRoleInfo.SevenDayActDict[actType] = itemDict;
					}
					itemDict[id] = itemData;
				}
			}
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x0001CBBC File Offset: 0x0001ADBC
		public static void DBTableRow2RoleInfo_SpecialActivityData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.SpecActInfoDict = new Dictionary<int, SpecActInfoDB>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					SpecActInfoDB itemData = new SpecActInfoDB();
					itemData.GroupID = Convert.ToInt32(cmd.Table.Rows[i]["groupid"].ToString());
					itemData.ActID = Convert.ToInt32(cmd.Table.Rows[i]["actid"].ToString());
					itemData.PurNum = Convert.ToInt32(cmd.Table.Rows[i]["purchaseNum"].ToString());
					itemData.CountNum = Convert.ToInt32(cmd.Table.Rows[i]["countNum"].ToString());
					itemData.Active = Convert.ToInt16(cmd.Table.Rows[i]["active"].ToString());
					dbRoleInfo.SpecActInfoDict[itemData.ActID] = itemData;
				}
			}
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0001CD0C File Offset: 0x0001AF0C
		public static void DBTableRow2RoleInfo_SpecialPriorityActivityData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.SpecPriorityActInfoDict = new Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					SpecPriorityActInfoDB itemData = new SpecPriorityActInfoDB();
					itemData.TeQuanID = Convert.ToInt32(cmd.Table.Rows[i]["tequanid"].ToString());
					itemData.ActID = Convert.ToInt32(cmd.Table.Rows[i]["actid"].ToString());
					itemData.PurNum = Convert.ToInt32(cmd.Table.Rows[i]["purchaseNum"].ToString());
					itemData.CountNum = Convert.ToInt32(cmd.Table.Rows[i]["countNum"].ToString());
					KeyValuePair<int, int> kvpKey = new KeyValuePair<int, int>(itemData.TeQuanID, itemData.ActID);
					dbRoleInfo.SpecPriorityActInfoDict[kvpKey] = itemData;
				}
			}
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x0001CE40 File Offset: 0x0001B040
		public static void DBTableRow2RoleInfo_EverydayActivityData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.EverydayActInfoDict = new Dictionary<int, EverydayActInfoDB>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					EverydayActInfoDB itemData = new EverydayActInfoDB();
					itemData.GroupID = Convert.ToInt32(cmd.Table.Rows[i]["groupid"].ToString());
					itemData.ActID = Convert.ToInt32(cmd.Table.Rows[i]["actid"].ToString());
					itemData.PurNum = Convert.ToInt32(cmd.Table.Rows[i]["purchaseNum"].ToString());
					itemData.CountNum = Convert.ToInt32(cmd.Table.Rows[i]["countNum"].ToString());
					itemData.ActiveDay = (int)Convert.ToInt16(cmd.Table.Rows[i]["activeDay"].ToString());
					dbRoleInfo.EverydayActInfoDict[itemData.ActID] = itemData;
				}
			}
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x0001CF90 File Offset: 0x0001B190
		public static void DBTableRow2RoleInfo_AlchemyData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.AlchemyInfo = new AlchemyDataDB();
				dbRoleInfo.AlchemyInfo.RoleID = dbRoleInfo.RoleID;
				dbRoleInfo.AlchemyInfo.BaseData.Element = Convert.ToInt32(cmd.Table.Rows[0]["element"].ToString());
				dbRoleInfo.AlchemyInfo.ElementDayID = Convert.ToInt32(cmd.Table.Rows[0]["dayid"].ToString());
				dbRoleInfo.AlchemyInfo.rollbackType = cmd.Table.Rows[0]["rollback"].ToString();
				string alchemyValue = cmd.Table.Rows[0]["value"].ToString();
				if (!string.IsNullOrEmpty(alchemyValue))
				{
					string[] alchemyFields = alchemyValue.Split(new char[]
					{
						'|'
					});
					foreach (string item in alchemyFields)
					{
						string[] kvpFields = item.Split(new char[]
						{
							','
						});
						if (kvpFields.Length == 2)
						{
							dbRoleInfo.AlchemyInfo.BaseData.AlchemyValue[Global.SafeConvertToInt32(kvpFields[0], 10)] = Global.SafeConvertToInt32(kvpFields[1], 10);
						}
					}
				}
				string todayCost = cmd.Table.Rows[0]["todaycost"].ToString();
				if (!string.IsNullOrEmpty(todayCost))
				{
					string[] costFields = todayCost.Split(new char[]
					{
						'|'
					});
					foreach (string item in costFields)
					{
						string[] kvpFields = item.Split(new char[]
						{
							','
						});
						if (kvpFields.Length == 2)
						{
							dbRoleInfo.AlchemyInfo.BaseData.ToDayCost[Global.SafeConvertToInt32(kvpFields[0], 10)] = Global.SafeConvertToInt32(kvpFields[1], 10);
						}
					}
				}
				string histCost = cmd.Table.Rows[0]["histcost"].ToString();
				if (!string.IsNullOrEmpty(histCost))
				{
					string[] costFields = histCost.Split(new char[]
					{
						'|'
					});
					foreach (string item in costFields)
					{
						string[] kvpFields = item.Split(new char[]
						{
							','
						});
						if (kvpFields.Length == 2)
						{
							dbRoleInfo.AlchemyInfo.HistCost[Global.SafeConvertToInt32(kvpFields[0], 10)] = Global.SafeConvertToInt32(kvpFields[1], 10);
						}
					}
				}
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0001D2A8 File Offset: 0x0001B4A8
		public static void DBTableRow2RoleInfo_ShenJiData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.ShenJiDict = new Dictionary<int, ShenJiFuWenData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					ShenJiFuWenData itemData = new ShenJiFuWenData();
					itemData.ShenJiID = Convert.ToInt32(cmd.Table.Rows[i]["sjID"].ToString());
					itemData.Level = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString());
					dbRoleInfo.ShenJiDict[itemData.ShenJiID] = itemData;
				}
			}
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x0001D3A4 File Offset: 0x0001B5A4
		public static void DBTableRow2RoleInfo_FuWenData(MySQLConnection connection, DBRoleInfo dbRoleInfo, int rid)
		{
			dbRoleInfo.FuWenTabList = new List<FuWenTabData>();
			string str = string.Format("SELECT * FROM t_fuwen where rid={0}", rid);
			try
			{
				MySQLCommand command = new MySQLCommand(str, connection);
				MySQLDataReader reader = command.ExecuteReaderEx();
				while (reader.Read())
				{
					string fuwenEquip = Encoding.UTF8.GetString(reader["fuwenequip"] as byte[]);
					string shenShiEquip = reader["shenshiactive"].ToString();
					List<FuWenTabData> fuWenTabList = dbRoleInfo.FuWenTabList;
					FuWenTabData fuWenTabData = new FuWenTabData();
					fuWenTabData.TabID = Convert.ToInt32(reader["tabid"].ToString());
					fuWenTabData.Name = reader["name"].ToString();
					FuWenTabData fuWenTabData2 = fuWenTabData;
					List<int> fuWenEquipList;
					if (!(fuwenEquip == ""))
					{
						fuWenEquipList = Array.ConvertAll<string, int>(fuwenEquip.Split(new char[]
						{
							','
						}), (string x) => Convert.ToInt32(x)).ToList<int>();
					}
					else
					{
						fuWenEquipList = new List<int>();
					}
					fuWenTabData2.FuWenEquipList = fuWenEquipList;
					FuWenTabData fuWenTabData3 = fuWenTabData;
					List<int> shenShiActiveList;
					if (!(shenShiEquip == ""))
					{
						shenShiActiveList = Array.ConvertAll<string, int>(shenShiEquip.Split(new char[]
						{
							','
						}), (string x) => Convert.ToInt32(x)).ToList<int>();
					}
					else
					{
						shenShiActiveList = new List<int>();
					}
					fuWenTabData3.ShenShiActiveList = shenShiActiveList;
					fuWenTabData.SkillEquip = Convert.ToInt32(reader["skillequip"].ToString());
					fuWenTabData.OwnerID = Convert.ToInt32(reader["rid"].ToString());
					fuWenTabList.Add(fuWenTabData);
				}
				command.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x0001D5B0 File Offset: 0x0001B7B0
		public static void DBTableRow2RoleInfo_JueXingData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.JueXingTaoZhuangList = new List<TaoZhuangData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					List<TaoZhuangData> jueXingTaoZhuangList = dbRoleInfo.JueXingTaoZhuangList;
					TaoZhuangData taoZhuangData = new TaoZhuangData();
					taoZhuangData.ID = Convert.ToInt32(cmd.Table.Rows[i]["suitid"].ToString());
					taoZhuangData.ActiviteList = Array.ConvertAll<string, int>(cmd.Table.Rows[i]["activite"].ToString().Split(new char[]
					{
						','
					}), (string x) => Convert.ToInt32(x)).ToList<int>();
					jueXingTaoZhuangList.Add(taoZhuangData);
				}
			}
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x0001D6B0 File Offset: 0x0001B8B0
		public static void DBTableRow2RoleInfo_ZuoQiData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MountList = new List<MountData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.MountList.Add(new MountData
					{
						GoodsID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString()),
						IsNew = (cmd.Table.Rows[i]["isnew"].ToString() == "1")
					});
				}
			}
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x0001D77C File Offset: 0x0001B97C
		public static void DBTableRow2RoleInfo_JingLingYuanSuJueXingData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				JingLingYuanSuJueXingData data = new JingLingYuanSuJueXingData();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					data.ActiveType = Convert.ToInt32(cmd.Table.Rows[i]["activetype"].ToString());
					string str = cmd.Table.Rows[i]["activeids"].ToString();
					if (str.Length == 0)
					{
						return;
					}
					string[] arr = str.Split(new char[]
					{
						','
					});
					data.ActiveIDs = new int[arr.Length];
					for (int j = 0; j < arr.Length; j++)
					{
						int.TryParse(arr[j], out data.ActiveIDs[j]);
					}
				}
				dbRoleInfo.JingLingYuanSuJueXingData = data;
			}
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x0001D898 File Offset: 0x0001BA98
		public bool Query(MySQLConnection conn, int roleID, bool bUseIsdel = true, int tempRoleID = 0)
		{
			LogManager.WriteLog(LogTypes.Info, string.Format("从数据库加载角色数据: {0}", roleID), null, true);
			MySQLSelectCommand cmd;
			if (bUseIsdel)
			{
				string[] fields = new string[]
				{
					"rid",
					"userid",
					"rname",
					"sex",
					"occupation",
					"level",
					"pic",
					"faction",
					"money1",
					"money2",
					"experience",
					"pkmode",
					"pkvalue",
					"position",
					"regtime",
					"lasttime",
					"bagnum",
					"othername",
					"main_quick_keys",
					"other_quick_keys",
					"loginnum",
					"leftfightsecs",
					"horseid",
					"petid",
					"interpower",
					"totalonlinesecs",
					"antiaddictionsecs",
					"logofftime",
					"biguantime",
					"yinliang",
					"total_jingmai_exp",
					"jingmai_exp_num",
					"lasthorseid",
					"skillid",
					"autolife",
					"automagic",
					"numskillid",
					"maintaskid",
					"pkpoint",
					"lianzhan",
					"killboss",
					"battlenamestart",
					"battlenameindex",
					"cztaskid",
					"battlenum",
					"heroindex",
					"logindayid",
					"logindaynum",
					"zoneid",
					"bhname",
					"bhverify",
					"bhzhiwu",
					"bgdayid1",
					"bgmoney",
					"bgdayid2",
					"bggoods",
					"banggong",
					"huanghou",
					"jiebiaodayid",
					"jiebiaonum",
					"username",
					"lastmailid",
					"onceawardflag",
					"banchat",
					"banlogin",
					"isflashplayer",
					"changelifecount",
					"admiredcount",
					"combatforce",
					"autoassignpropertypoint",
					"store_yinliang",
					"store_money",
					"magic_sword_param",
					"fluorescent_point",
					"ban_trade_to_ticks",
					"juntuanzhiwu",
					"huiji",
					"huijiexp",
					"armor",
					"armorexp",
					"bianshen",
					"bianshenexp",
					"reborn_bagnum",
					"reborn_isshow",
					"reborn_isshow_model",
					"zhanduiid",
					"zhanduizhiwu"
				};
				string[] tables = new string[]
				{
					"t_roles"
				};
				object[,] array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "isdel";
				array[1, 1] = "=";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields, tables, array, null, null);
			}
			else
			{
				string[] fields2 = new string[]
				{
					"rid",
					"userid",
					"rname",
					"sex",
					"occupation",
					"level",
					"pic",
					"faction",
					"money1",
					"money2",
					"experience",
					"pkmode",
					"pkvalue",
					"position",
					"regtime",
					"lasttime",
					"bagnum",
					"othername",
					"main_quick_keys",
					"other_quick_keys",
					"loginnum",
					"leftfightsecs",
					"horseid",
					"petid",
					"interpower",
					"totalonlinesecs",
					"antiaddictionsecs",
					"logofftime",
					"biguantime",
					"yinliang",
					"total_jingmai_exp",
					"jingmai_exp_num",
					"lasthorseid",
					"skillid",
					"autolife",
					"automagic",
					"numskillid",
					"maintaskid",
					"pkpoint",
					"lianzhan",
					"killboss",
					"battlenamestart",
					"battlenameindex",
					"cztaskid",
					"battlenum",
					"heroindex",
					"logindayid",
					"logindaynum",
					"zoneid",
					"bhname",
					"bhverify",
					"bhzhiwu",
					"bgdayid1",
					"bgmoney",
					"bgdayid2",
					"bggoods",
					"banggong",
					"huanghou",
					"jiebiaodayid",
					"jiebiaonum",
					"username",
					"lastmailid",
					"onceawardflag",
					"banchat",
					"banlogin",
					"isflashplayer",
					"changelifecount",
					"admiredcount",
					"combatforce",
					"autoassignpropertypoint",
					"store_yinliang",
					"store_money",
					"magic_sword_param",
					"fluorescent_point",
					"ban_trade_to_ticks",
					"juntuanzhiwu",
					"huiji",
					"huijiexp",
					"armor",
					"armorexp",
					"bianshen",
					"bianshenexp",
					"reborn_bagnum",
					"reborn_isshow",
					"reborn_isshow_model",
					"zhanduiid",
					"zhanduizhiwu"
				};
				string[] tables2 = new string[]
				{
					"t_roles"
				};
				object[,] array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields2, tables2, array, null, null);
			}
			bool result;
			if (cmd.Table.Rows.Count <= 0)
			{
				result = false;
			}
			else
			{
				this.PTID = GameDBManager.PTID;
				this.WorldRoleID = string.Format("{0}@{1}", roleID, this.PTID);
				DBRoleInfo.DBTableRow2RoleInfo(this, cmd, 0);
				object[,] array;
				if (GameDBManager.Flag_Splite_RoleParams_Table == 0)
				{
					string[] fields3 = new string[]
					{
						"pname",
						"pvalue"
					};
					string[] tables3 = new string[]
					{
						"t_roleparams"
					};
					array = new object[1, 3];
					array[0, 0] = "rid";
					array[0, 1] = "=";
					array[0, 2] = roleID;
					cmd = new MySQLSelectCommand(conn, fields3, tables3, array, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_Params(this, cmd, true);
				}
				else
				{
					string[] fields4 = new string[]
					{
						"pname",
						"pvalue"
					};
					string[] tables4 = new string[]
					{
						"t_roleparams_2"
					};
					array = new object[1, 3];
					array[0, 0] = "rid";
					array[0, 1] = "=";
					array[0, 2] = roleID;
					cmd = new MySQLSelectCommand(conn, fields4, tables4, array, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_Params(this, cmd, false);
					string[] fields5 = new string[]
					{
						"*"
					};
					string[] tables5 = new string[]
					{
						"t_roleparams_long"
					};
					array = new object[1, 3];
					array[0, 0] = "rid";
					array[0, 1] = "=";
					array[0, 2] = roleID;
					cmd = new MySQLSelectCommand(conn, fields5, tables5, array, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_ParamsEx(this, cmd);
					string[] fields6 = new string[]
					{
						"*"
					};
					string[] tables6 = new string[]
					{
						"t_roleparams_char"
					};
					array = new object[1, 3];
					array[0, 0] = "rid";
					array[0, 1] = "=";
					array[0, 2] = roleID;
					cmd = new MySQLSelectCommand(conn, fields6, tables6, array, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_ParamsEx(this, cmd);
				}
				DBRoleInfo.InitFromRoleParams(this);
				string[] fields7 = new string[]
				{
					"rid",
					"taskid",
					"count"
				};
				string[] tables7 = new string[]
				{
					"t_taskslog"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields7, tables7, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_OldTasks(this, cmd);
				string[] fields8 = new string[]
				{
					"Id",
					"rid",
					"taskid",
					"focus",
					"value1",
					"value2",
					"addtime",
					"starlevel"
				};
				string[] tables8 = new string[]
				{
					"t_tasks"
				};
				array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "isdel";
				array[1, 1] = "=";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields8, tables8, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DoingTasks(this, cmd);
				string[] fields9 = new string[]
				{
					"id",
					"goodsid",
					"isusing",
					"forge_level",
					"starttime",
					"endtime",
					"site",
					"quality",
					"Props",
					"gcount",
					"binding",
					"jewellist",
					"bagindex",
					"salemoney1",
					"saleyuanbao",
					"saleyinpiao",
					"addpropindex",
					"bornindex",
					"lucky",
					"strong",
					"excellenceinfo",
					"appendproplev",
					"equipchangelife",
					"ehinfo",
					"washprops",
					"juhun"
				};
				string[] tables9 = new string[]
				{
					"t_goods"
				};
				array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "gcount";
				array[1, 1] = ">";
				array[1, 2] = 0;
				object[,] whereParamFields = array;
				string[,] whereNoparamFields = null;
				string[,] array2 = new string[1, 2];
				array2[0, 0] = "id";
				array2[0, 1] = "asc";
				cmd = new MySQLSelectCommand(conn, fields9, tables9, whereParamFields, whereNoparamFields, array2);
				DBRoleInfo.DBTableRow2RoleInfo_Goods(this, cmd);
				string[] fields10 = new string[]
				{
					"goodsid",
					"dayid",
					"usednum"
				};
				string[] tables10 = new string[]
				{
					"t_goodslimit"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields10, tables10, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GoodsLimit(this, cmd);
				string[] fields11 = new string[]
				{
					"Id",
					"otherid",
					"friendType"
				};
				string[] tables11 = new string[]
				{
					"t_friends"
				};
				array = new object[1, 3];
				array[0, 0] = "myid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields11, tables11, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Friends(this, cmd);
				string[] fields12 = new string[]
				{
					"Id",
					"horseid",
					"bodyid",
					"propsNum",
					"PropsVal",
					"addtime",
					"failednum",
					"temptime",
					"tempnum",
					"faileddayid"
				};
				string[] tables12 = new string[]
				{
					"t_horses"
				};
				array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "isdel";
				array[1, 1] = "=";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields12, tables12, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Horses(this, cmd);
				string[] fields13 = new string[]
				{
					"Id",
					"petid",
					"petname",
					"pettype",
					"feednum",
					"realivenum",
					"addtime",
					"props",
					"level"
				};
				string[] tables13 = new string[]
				{
					"t_pets"
				};
				array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "isdel";
				array[1, 1] = "=";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields13, tables13, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Pets(this, cmd);
				string[] fields14 = new string[]
				{
					"Id",
					"jmid",
					"jmlevel",
					"bodylevel"
				};
				string[] tables14 = new string[]
				{
					"t_jingmai"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields14, tables14, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_JingMais(this, cmd);
				string[] fields15 = new string[]
				{
					"Id",
					"skillid",
					"skilllevel",
					"usednum"
				};
				string[] tables15 = new string[]
				{
					"t_skills"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields15, tables15, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Skills(this, cmd);
				string[] fields16 = new string[]
				{
					"bufferid",
					"starttime",
					"buffersecs",
					"bufferval"
				};
				string[] tables16 = new string[]
				{
					"t_buffer"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields16, tables16, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Buffers(this, cmd);
				string[] fields17 = new string[]
				{
					"huanid",
					"rectime",
					"recnum",
					"taskClass",
					"extdayid",
					"extnum"
				};
				string[] tables17 = new string[]
				{
					"t_dailytasks"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields17, tables17, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DailyTasks(this, cmd);
				string[] fields18 = new string[]
				{
					"jmtime",
					"jmnum"
				};
				string[] tables18 = new string[]
				{
					"t_dailyjingmai"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields18, tables18, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DailyJingMai(this, cmd);
				string[] fields19 = new string[]
				{
					"extgridnum"
				};
				string[] tables19 = new string[]
				{
					"t_ptbag"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields19, tables19, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_PortableBag(this, cmd);
				string[] fields20 = new string[]
				{
					"extgridnum"
				};
				string[] tables20 = new string[]
				{
					"t_reborn_storage"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields20, tables20, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_RebornPortableBag(this, cmd);
				string[] fields21 = new string[]
				{
					"loginweekid",
					"logindayid",
					"loginnum",
					"newstep",
					"steptime",
					"lastmtime",
					"curmid",
					"curmtime",
					"songliid",
					"logingiftstate",
					"onlinegiftstate",
					"lastlimittimehuodongid",
					"lastlimittimedayid",
					"limittimeloginnum",
					"limittimegiftstate",
					"everydayonlineawardstep",
					"geteverydayonlineawarddayid",
					"serieslogingetawardstep",
					"seriesloginawarddayid",
					"seriesloginawardgoodsid",
					"everydayonlineawardgoodsid"
				};
				string[] tables21 = new string[]
				{
					"t_huodong"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields21, tables21, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_HuodongData(this, cmd);
				string[] fields22 = new string[]
				{
					"fubenid",
					"dayid",
					"enternum",
					"quickpasstimer",
					"finishnum"
				};
				string[] tables22 = new string[]
				{
					"t_fuben"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields22, tables22, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_FuBenData(this, cmd);
				string[] fields23 = new string[]
				{
					"spouseid",
					"marrytype",
					"ringid",
					"goodwillexp",
					"goodwillstar",
					"goodwilllevel",
					"givenrose",
					"lovemessage",
					"autoreject",
					"changtime"
				};
				string[] tables23 = new string[]
				{
					"t_marry"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields23, tables23, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_MarriageData(this, cmd);
				string[] fields24 = new string[]
				{
					"partyroleid",
					"joincount"
				};
				string[] tables24 = new string[]
				{
					"t_marryparty_join"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields24, tables24, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_MarryPartyJoinList(this, cmd);
				string[] fields25 = new string[]
				{
					"*"
				};
				string[] tables25 = new string[]
				{
					"t_holyitem"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields25, tables25, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_HolyItemData(this, cmd);
				string[] fields26 = new string[]
				{
					"*"
				};
				string[] tables26 = new string[]
				{
					"t_dailydata"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields26, tables26, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DailyData(this, cmd);
				string[] fields27 = new string[]
				{
					"yabiaoid",
					"starttime",
					"state",
					"lineid",
					"toubao",
					"yabiaodayid",
					"yabiaonum",
					"takegoods"
				};
				string[] tables27 = new string[]
				{
					"t_yabiao"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields27, tables27, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_YaBiaoData(this, cmd);
				string[] fields28 = new string[]
				{
					"rid",
					"prioritytype",
					"dayid",
					"usedtimes"
				};
				string[] tables28 = new string[]
				{
					"t_vipdailydata"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields28, tables28, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_VipDailyData(this, cmd);
				string[] fields29 = new string[]
				{
					"rid",
					"jifen",
					"dayid",
					"awardhistory"
				};
				string[] tables29 = new string[]
				{
					"t_yangguangbkdailydata"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields29, tables29, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_YangGongBKDailyJiFenData(this, cmd);
				string[] fields30 = new string[]
				{
					"Id",
					"wingid",
					"forgeLevel",
					"addtime",
					"failednum",
					"equiped",
					"starexp",
					"zhulingnum",
					"zhuhunnum"
				};
				string[] tables30 = new string[]
				{
					"t_wings"
				};
				array = new object[2, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "isdel";
				array[1, 1] = "=";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields30, tables30, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Wings(this, cmd);
				string[] fields31 = new string[]
				{
					"Id",
					"roleid",
					"picturejudgeid",
					"refercount"
				};
				string[] tables31 = new string[]
				{
					"t_picturejudgeinfo"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields31, tables31, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_picturejudgeinfo(this, cmd);
				string[] fields32 = new string[]
				{
					"Id",
					"roleid",
					"starsiteid",
					"starslotid"
				};
				string[] tables32 = new string[]
				{
					"t_starconstellationinfo"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields32, tables32, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_starconstellationinfo(this, cmd);
				string[] fields33 = new string[]
				{
					"roleid",
					"type",
					"level",
					"suit"
				};
				string[] tables33 = new string[]
				{
					"t_lingyu"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields33, tables33, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_LingYuInfo(this, cmd);
				string[] fields34 = new string[]
				{
					"roleid",
					"gmailid"
				};
				string[] tables34 = new string[]
				{
					"t_rolegmail_record"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields34, tables34, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GMailInfo(this, cmd);
				string[] fields35 = new string[]
				{
					"roleid",
					"slot_cnt",
					"level",
					"suit",
					"total_guard_point",
					"lastday_recover_point",
					"lastday_recover_offset"
				};
				string[] tables35 = new string[]
				{
					"t_guard_statue"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields35, tables35, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GuardStatue(this, cmd);
				string[] fields36 = new string[]
				{
					"roleid",
					"soul_type",
					"equip_slot"
				};
				string[] tables36 = new string[]
				{
					"t_guard_soul"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields36, tables36, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GuardSoul(this, cmd);
				string[] fields37 = new string[]
				{
					"tatalCount",
					"exp"
				};
				string[] tables37 = new string[]
				{
					"t_talent"
				};
				array = new object[1, 3];
				array[0, 0] = "roleID";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields37, tables37, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_TalentBase(this, cmd);
				string[] fields38 = new string[]
				{
					"talentType",
					"effectID",
					"effectLevel"
				};
				string[] tables38 = new string[]
				{
					"t_talent_effect"
				};
				array = new object[2, 3];
				array[0, 0] = "roleID";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				array[1, 0] = "effectLevel";
				array[1, 1] = ">";
				array[1, 2] = 0;
				cmd = new MySQLSelectCommand(conn, fields38, tables38, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_TalentEffects(this, cmd);
				string[] fields39 = new string[]
				{
					"duanweiid",
					"duanweijifen",
					"duanweirank",
					"liansheng",
					"fightcount",
					"successcount",
					"todayfightcount",
					"lastfightdayid",
					"monthduanweirank",
					"fetchmonthawarddate",
					"rongyao"
				};
				string[] tables39 = new string[]
				{
					"t_kf_tianti_role"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields39, tables39, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_TianTiData(this, cmd);
				string[] fields40 = new string[]
				{
					"roleID",
					"occupation",
					"level",
					"level_up_fail_num",
					"starNum",
					"starExp",
					"luckyPoint",
					"toTicks",
					"addTime",
					"activeFrozen",
					"activePalsy",
					"activeSpeedDown",
					"activeBlow",
					"unActiveFrozen",
					"unActivePalsy",
					"unActiveSpeedDown",
					"unActiveBlow"
				};
				string[] tables40 = new string[]
				{
					"t_merlin_magic_book"
				};
				array = new object[1, 3];
				array[0, 0] = "roleID";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields40, tables40, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_MerlinData(this, cmd);
				string[] fields41 = new string[]
				{
					"id",
					"roleid",
					"goodsid",
					"position",
					"type",
					"bind"
				};
				string[] tables41 = new string[]
				{
					"t_fluorescent_gem_equip"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields41, tables41, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_FluorescentGemData(this, cmd);
				string[] fields42 = new string[]
				{
					"buildid",
					"taskid_1",
					"taskid_2",
					"taskid_3",
					"taskid_4",
					"level",
					"exp",
					"developtime"
				};
				string[] tables42 = new string[]
				{
					"t_building"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields42, tables42, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_BuildingData(this, cmd);
				string[] fields43 = new string[]
				{
					"goodsid",
					"param1",
					"param2"
				};
				string[] tables43 = new string[]
				{
					"t_ornament"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields43, tables43, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_OrnamentData(this, cmd);
				string[] fields44 = new string[]
				{
					"roleid",
					"act_type",
					"id",
					"award_flag",
					"param1",
					"param2"
				};
				string[] tables44 = new string[]
				{
					"t_seven_day_act"
				};
				array = new object[1, 3];
				array[0, 0] = "roleid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields44, tables44, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_SevenDayActData(this, cmd);
				string[] fields45 = new string[]
				{
					"rid",
					"groupid",
					"actid",
					"purchaseNum",
					"countNum",
					"active"
				};
				string[] tables45 = new string[]
				{
					"t_special_activity"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields45, tables45, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_SpecialActivityData(this, cmd);
				string[] fields46 = new string[]
				{
					"rid",
					"tequanid",
					"actid",
					"purchaseNum",
					"countNum"
				};
				string[] tables46 = new string[]
				{
					"t_special_priority_activity"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields46, tables46, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_SpecialPriorityActivityData(this, cmd);
				string[] fields47 = new string[]
				{
					"rid",
					"sjID",
					"level"
				};
				string[] tables47 = new string[]
				{
					"t_shenjifuwen"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields47, tables47, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_ShenJiData(this, cmd);
				DBRoleInfo.DBTableRow2RoleInfo_TarotData(conn, this, roleID);
				string[] fields48 = new string[]
				{
					"rid",
					"groupid",
					"actid",
					"purchaseNum",
					"countNum",
					"activeDay"
				};
				string[] tables48 = new string[]
				{
					"t_everyday_activity"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields48, tables48, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_EverydayActivityData(this, cmd);
				DBRoleInfo.DBTableRow2RoleInfo_FuWenData(conn, this, roleID);
				string[] fields49 = new string[]
				{
					"rid",
					"element",
					"dayid",
					"value",
					"todaycost",
					"histcost",
					"rollback"
				};
				string[] tables49 = new string[]
				{
					"t_alchemy"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields49, tables49, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_AlchemyData(this, cmd);
				string[] fields50 = new string[]
				{
					"rid",
					"suitid",
					"activite"
				};
				string[] tables50 = new string[]
				{
					"t_juexing"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields50, tables50, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_JueXingData(this, cmd);
				string[] fields51 = new string[]
				{
					"rid",
					"goodsid",
					"isnew"
				};
				string[] tables51 = new string[]
				{
					"t_zuoqi"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields51, tables51, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_ZuoQiData(this, cmd);
				string[] fields52 = new string[]
				{
					"rid",
					"activetype",
					"activeids"
				};
				string[] tables52 = new string[]
				{
					"t_juexing_jlys"
				};
				array = new object[1, 3];
				array[0, 0] = "rid";
				array[0, 1] = "=";
				array[0, 2] = roleID;
				cmd = new MySQLSelectCommand(conn, fields52, tables52, array, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_JingLingYuanSuJueXingData(this, cmd);
				this.RankValue.Init(roleID);
				result = true;
			}
			return result;
		}

		// Token: 0x04000632 RID: 1586
		public int PTID;

		// Token: 0x04000633 RID: 1587
		public string WorldRoleID;

		// Token: 0x04000634 RID: 1588
		public string Channel;

		// Token: 0x04000635 RID: 1589
		public int SubOccupation;

		// Token: 0x04000636 RID: 1590
		public int ZhanDuiID;

		// Token: 0x04000637 RID: 1591
		public int ZhanDuiZhiWu;

		// Token: 0x04000638 RID: 1592
		public int JunTuanZhiWu;

		// Token: 0x04000639 RID: 1593
		public RoleHuiJiData HuiJiData = new RoleHuiJiData();

		// Token: 0x0400063A RID: 1594
		public RoleBianShenData BianShenData = new RoleBianShenData();

		// Token: 0x0400063B RID: 1595
		public RoleArmorData ArmorData = new RoleArmorData();

		// Token: 0x0400063C RID: 1596
		public List<int> OccupationList = new List<int>();

		// Token: 0x0400063D RID: 1597
		public RoleCustomData roleCustomData;

		// Token: 0x0400063E RID: 1598
		private UserRankValueCache rankValue = new UserRankValueCache();

		// Token: 0x0400063F RID: 1599
		public Dictionary<string, RoleParamsData> RoleParamsDict = new Dictionary<string, RoleParamsData>();

		// Token: 0x04000640 RID: 1600
		private long _LastReferenceTicks = DateTime.Now.Ticks / 10000L;

		// Token: 0x04000641 RID: 1601
		public RoleTianTiData TianTiData;

		// Token: 0x04000642 RID: 1602
		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData;
	}
}
