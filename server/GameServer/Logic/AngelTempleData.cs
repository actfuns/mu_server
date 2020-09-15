using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000603 RID: 1539
	public class AngelTempleData
	{
		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06001E32 RID: 7730 RVA: 0x001ACCA8 File Offset: 0x001AAEA8
		// (set) Token: 0x06001E33 RID: 7731 RVA: 0x001ACCBF File Offset: 0x001AAEBF
		public int MapCode { get; set; }

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06001E34 RID: 7732 RVA: 0x001ACCC8 File Offset: 0x001AAEC8
		// (set) Token: 0x06001E35 RID: 7733 RVA: 0x001ACCDF File Offset: 0x001AAEDF
		public int MinChangeLifeNum { get; set; }

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06001E36 RID: 7734 RVA: 0x001ACCE8 File Offset: 0x001AAEE8
		// (set) Token: 0x06001E37 RID: 7735 RVA: 0x001ACCFF File Offset: 0x001AAEFF
		public int MinLevel { get; set; }

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06001E38 RID: 7736 RVA: 0x001ACD08 File Offset: 0x001AAF08
		// (set) Token: 0x06001E39 RID: 7737 RVA: 0x001ACD1F File Offset: 0x001AAF1F
		public List<string> BeginTime { get; set; }

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06001E3A RID: 7738 RVA: 0x001ACD28 File Offset: 0x001AAF28
		// (set) Token: 0x06001E3B RID: 7739 RVA: 0x001ACD3F File Offset: 0x001AAF3F
		public int PrepareTime { get; set; }

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06001E3C RID: 7740 RVA: 0x001ACD48 File Offset: 0x001AAF48
		// (set) Token: 0x06001E3D RID: 7741 RVA: 0x001ACD5F File Offset: 0x001AAF5F
		public int DurationTime { get; set; }

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06001E3E RID: 7742 RVA: 0x001ACD68 File Offset: 0x001AAF68
		// (set) Token: 0x06001E3F RID: 7743 RVA: 0x001ACD7F File Offset: 0x001AAF7F
		public int LeaveTime { get; set; }

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06001E40 RID: 7744 RVA: 0x001ACD88 File Offset: 0x001AAF88
		// (set) Token: 0x06001E41 RID: 7745 RVA: 0x001ACD9F File Offset: 0x001AAF9F
		public int MinPlayerNum { get; set; }

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06001E42 RID: 7746 RVA: 0x001ACDA8 File Offset: 0x001AAFA8
		// (set) Token: 0x06001E43 RID: 7747 RVA: 0x001ACDBF File Offset: 0x001AAFBF
		public int MaxPlayerNum { get; set; }

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06001E44 RID: 7748 RVA: 0x001ACDC8 File Offset: 0x001AAFC8
		// (set) Token: 0x06001E45 RID: 7749 RVA: 0x001ACDDF File Offset: 0x001AAFDF
		public int BossID { get; set; }

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06001E46 RID: 7750 RVA: 0x001ACDE8 File Offset: 0x001AAFE8
		// (set) Token: 0x06001E47 RID: 7751 RVA: 0x001ACDFF File Offset: 0x001AAFFF
		public int BossPosX { get; set; }

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06001E48 RID: 7752 RVA: 0x001ACE08 File Offset: 0x001AB008
		// (set) Token: 0x06001E49 RID: 7753 RVA: 0x001ACE1F File Offset: 0x001AB01F
		public int BossPosY { get; set; }
	}
}
