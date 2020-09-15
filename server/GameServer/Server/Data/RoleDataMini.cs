using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x0200058A RID: 1418
	[ProtoContract]
	public class RoleDataMini
	{
		// Token: 0x040027B3 RID: 10163
		[ProtoMember(1)]
		public int RoleID = 0;

		// Token: 0x040027B4 RID: 10164
		[ProtoMember(2)]
		public string RoleName = "";

		// Token: 0x040027B5 RID: 10165
		[ProtoMember(3)]
		public int RoleSex = 0;

		// Token: 0x040027B6 RID: 10166
		[ProtoMember(4)]
		public int Occupation = 0;

		// Token: 0x040027B7 RID: 10167
		[ProtoMember(5)]
		public int Level = 1;

		// Token: 0x040027B8 RID: 10168
		[ProtoMember(6)]
		public int Faction = 0;

		// Token: 0x040027B9 RID: 10169
		[ProtoMember(7)]
		public int PKMode = 0;

		// Token: 0x040027BA RID: 10170
		[ProtoMember(8)]
		public int PKValue = 0;

		// Token: 0x040027BB RID: 10171
		[ProtoMember(9)]
		public int MapCode = 0;

		// Token: 0x040027BC RID: 10172
		[ProtoMember(10)]
		public int PosX = 0;

		// Token: 0x040027BD RID: 10173
		[ProtoMember(11)]
		public int PosY = 0;

		// Token: 0x040027BE RID: 10174
		[ProtoMember(12)]
		public int RoleDirection = 0;

		// Token: 0x040027BF RID: 10175
		[ProtoMember(13)]
		public int LifeV = 0;

		// Token: 0x040027C0 RID: 10176
		[ProtoMember(14)]
		public int MaxLifeV = 0;

		// Token: 0x040027C1 RID: 10177
		[ProtoMember(15)]
		public int MagicV = 0;

		// Token: 0x040027C2 RID: 10178
		[ProtoMember(16)]
		public int MaxMagicV = 0;

		// Token: 0x040027C3 RID: 10179
		[ProtoMember(17)]
		public int BodyCode;

		// Token: 0x040027C4 RID: 10180
		[ProtoMember(18)]
		public int WeaponCode;

		// Token: 0x040027C5 RID: 10181
		[ProtoMember(19)]
		public string OtherName;

		// Token: 0x040027C6 RID: 10182
		[ProtoMember(20)]
		public int TeamID;

		// Token: 0x040027C7 RID: 10183
		[ProtoMember(21)]
		public int TeamLeaderRoleID = 0;

		// Token: 0x040027C8 RID: 10184
		[ProtoMember(22)]
		public int PKPoint = 0;

		// Token: 0x040027C9 RID: 10185
		[ProtoMember(23)]
		public long StartPurpleNameTicks = 0L;

		// Token: 0x040027CA RID: 10186
		[ProtoMember(24)]
		public long BattleNameStart = 0L;

		// Token: 0x040027CB RID: 10187
		[ProtoMember(25)]
		public int BattleNameIndex = 0;

		// Token: 0x040027CC RID: 10188
		[ProtoMember(26)]
		public int ZoneID = 0;

		// Token: 0x040027CD RID: 10189
		[ProtoMember(27)]
		public string BHName = "";

		// Token: 0x040027CE RID: 10190
		[ProtoMember(28)]
		public int BHVerify = 0;

		// Token: 0x040027CF RID: 10191
		[ProtoMember(29)]
		public int BHZhiWu = 0;

		// Token: 0x040027D0 RID: 10192
		[ProtoMember(30)]
		public long FSHuDunStart = 0L;

		// Token: 0x040027D1 RID: 10193
		[ProtoMember(31)]
		public int BattleWhichSide = -1;

		// Token: 0x040027D2 RID: 10194
		[ProtoMember(32)]
		public int IsVIP = 0;

		// Token: 0x040027D3 RID: 10195
		[ProtoMember(33)]
		public long DSHideStart = 0L;

		// Token: 0x040027D4 RID: 10196
		[ProtoMember(34)]
		public List<int> RoleCommonUseIntPamams = new List<int>();

		// Token: 0x040027D5 RID: 10197
		[ProtoMember(35)]
		public int FSHuDunSeconds = 0;

		// Token: 0x040027D6 RID: 10198
		[ProtoMember(36)]
		public long ZhongDuStart = 0L;

		// Token: 0x040027D7 RID: 10199
		[ProtoMember(37)]
		public int ZhongDuSeconds = 0;

		// Token: 0x040027D8 RID: 10200
		[ProtoMember(38)]
		public int JieriChengHao = 0;

		// Token: 0x040027D9 RID: 10201
		[ProtoMember(39)]
		public long DongJieStart = 0L;

		// Token: 0x040027DA RID: 10202
		[ProtoMember(40)]
		public int DongJieSeconds = 0;

		// Token: 0x040027DB RID: 10203
		[ProtoMember(41)]
		public List<GoodsData> GoodsDataList;

		// Token: 0x040027DC RID: 10204
		[ProtoMember(42)]
		public int ChangeLifeLev;

		// Token: 0x040027DD RID: 10205
		[ProtoMember(43)]
		public int ChangeLifeCount = 0;

		// Token: 0x040027DE RID: 10206
		[ProtoMember(44)]
		public string StallName;

		// Token: 0x040027DF RID: 10207
		[ProtoMember(45)]
		public List<BufferDataMini> BufferMiniInfo;

		// Token: 0x040027E0 RID: 10208
		[ProtoMember(46)]
		public WingData MyWingData = null;

		// Token: 0x040027E1 RID: 10209
		[ProtoMember(47)]
		public int VIPLevel = 0;

		// Token: 0x040027E2 RID: 10210
		[ProtoMember(48)]
		public int GMAuth = 0;

		// Token: 0x040027E3 RID: 10211
		[ProtoMember(49)]
		public long SettingBitFlags;

		// Token: 0x040027E4 RID: 10212
		[ProtoMember(50)]
		public int SpouseId;

		// Token: 0x040027E5 RID: 10213
		[ProtoMember(51)]
		public List<int> OccupationList;

		// Token: 0x040027E6 RID: 10214
		[ProtoMember(52)]
		public int JunTuanId;

		// Token: 0x040027E7 RID: 10215
		[ProtoMember(53)]
		public string JunTuanName;

		// Token: 0x040027E8 RID: 10216
		[ProtoMember(54)]
		public int JunTuanZhiWu;

		// Token: 0x040027E9 RID: 10217
		[ProtoMember(55)]
		public int LingDi;

		// Token: 0x040027EA RID: 10218
		[ProtoMember(56)]
		public RoleHuiJiData HuiJiData;

		// Token: 0x040027EB RID: 10219
		[ProtoMember(57)]
		public SkillEquipData ShenShiEquipData;

		// Token: 0x040027EC RID: 10220
		[ProtoMember(58)]
		public List<int> PassiveEffectList;

		// Token: 0x040027ED RID: 10221
		[ProtoMember(59)]
		public JueXingShiData JueXingData;

		// Token: 0x040027EE RID: 10222
		[ProtoMember(60)]
		public int CompType;

		// Token: 0x040027EF RID: 10223
		[ProtoMember(61)]
		public byte CompZhiWu;

		// Token: 0x040027F0 RID: 10224
		[ProtoMember(62)]
		public GoodsData EquipMount;

		// Token: 0x040027F1 RID: 10225
		[ProtoMember(63)]
		public int SubOccupation;

		// Token: 0x040027F2 RID: 10226
		[ProtoMember(64)]
		public int CurrentArmorV;

		// Token: 0x040027F3 RID: 10227
		[ProtoMember(65)]
		public int MaxArmorV;

		// Token: 0x040027F4 RID: 10228
		[ProtoMember(66)]
		public ZuoQiMainData ZuoQiMainData;

		// Token: 0x040027F5 RID: 10229
		[ProtoMember(67)]
		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData;

		// Token: 0x040027F6 RID: 10230
		[ProtoMember(68)]
		public int RebornCount = 0;

		// Token: 0x040027F7 RID: 10231
		[ProtoMember(69)]
		public int RebornLevel = 0;

		// Token: 0x040027F8 RID: 10232
		[ProtoMember(70)]
		public int RebornEquipShow = 0;

		// Token: 0x040027F9 RID: 10233
		[ProtoMember(71)]
		public List<GoodsData> RebornGoodsDataList;

		// Token: 0x040027FA RID: 10234
		[ProtoMember(72)]
		public RebornStampData RebornYinJi;

		// Token: 0x040027FB RID: 10235
		[ProtoMember(73)]
		public int RebornModelShow = 0;

		// Token: 0x040027FC RID: 10236
		[ProtoMember(250)]
		public int UserPTID;

		// Token: 0x040027FD RID: 10237
		[ProtoMember(251)]
		public string WorldRoleID;

		// Token: 0x040027FE RID: 10238
		[ProtoMember(252)]
		public string Channel;
	}
}
