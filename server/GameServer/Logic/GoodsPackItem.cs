using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Interface;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006E6 RID: 1766
	public class GoodsPackItem : IObject
	{
		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06002A30 RID: 10800 RVA: 0x0025AAA0 File Offset: 0x00258CA0
		// (set) Token: 0x06002A31 RID: 10801 RVA: 0x0025AAB7 File Offset: 0x00258CB7
		public int AutoID { get; set; }

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06002A32 RID: 10802 RVA: 0x0025AAC0 File Offset: 0x00258CC0
		// (set) Token: 0x06002A33 RID: 10803 RVA: 0x0025AAD7 File Offset: 0x00258CD7
		public int GoodsPackID { get; set; }

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06002A34 RID: 10804 RVA: 0x0025AAE0 File Offset: 0x00258CE0
		// (set) Token: 0x06002A35 RID: 10805 RVA: 0x0025AAF7 File Offset: 0x00258CF7
		public int OwnerRoleID { get; set; }

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06002A36 RID: 10806 RVA: 0x0025AB00 File Offset: 0x00258D00
		// (set) Token: 0x06002A37 RID: 10807 RVA: 0x0025AB17 File Offset: 0x00258D17
		public string OwnerRoleName { get; set; }

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06002A38 RID: 10808 RVA: 0x0025AB20 File Offset: 0x00258D20
		// (set) Token: 0x06002A39 RID: 10809 RVA: 0x0025AB37 File Offset: 0x00258D37
		public int GoodsPackType { get; set; }

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06002A3A RID: 10810 RVA: 0x0025AB40 File Offset: 0x00258D40
		// (set) Token: 0x06002A3B RID: 10811 RVA: 0x0025AB57 File Offset: 0x00258D57
		public long ProduceTicks { get; set; }

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06002A3C RID: 10812 RVA: 0x0025AB60 File Offset: 0x00258D60
		// (set) Token: 0x06002A3D RID: 10813 RVA: 0x0025AB77 File Offset: 0x00258D77
		public int LockedRoleID { get; set; }

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06002A3E RID: 10814 RVA: 0x0025AB80 File Offset: 0x00258D80
		public Dictionary<int, bool> GoodsIDDict
		{
			get
			{
				return this._GoodsIDDict;
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06002A3F RID: 10815 RVA: 0x0025AB98 File Offset: 0x00258D98
		public Dictionary<int, int> GoodsIDToRolesDict
		{
			get
			{
				return this._GoodsIDToRolesDict;
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06002A40 RID: 10816 RVA: 0x0025ABB0 File Offset: 0x00258DB0
		// (set) Token: 0x06002A41 RID: 10817 RVA: 0x0025ABC7 File Offset: 0x00258DC7
		public long OpenPackTicks { get; set; }

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06002A42 RID: 10818 RVA: 0x0025ABD0 File Offset: 0x00258DD0
		public Dictionary<int, long> RolesTicksDict
		{
			get
			{
				return this._RolesTicksDict;
			}
		}

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06002A43 RID: 10819 RVA: 0x0025ABE8 File Offset: 0x00258DE8
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_GOODSPACK;
			}
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06002A44 RID: 10820 RVA: 0x0025ABFC File Offset: 0x00258DFC
		// (set) Token: 0x06002A45 RID: 10821 RVA: 0x0025AC13 File Offset: 0x00258E13
		public long AutoOpenPackTicks { get; set; }

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06002A46 RID: 10822 RVA: 0x0025AC1C File Offset: 0x00258E1C
		// (set) Token: 0x06002A47 RID: 10823 RVA: 0x0025AC33 File Offset: 0x00258E33
		public int OnlyID { get; set; }

		// Token: 0x06002A48 RID: 10824 RVA: 0x0025AC3C File Offset: 0x00258E3C
		public int GetObjectID()
		{
			return this.AutoID;
		}

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06002A49 RID: 10825 RVA: 0x0025AC54 File Offset: 0x00258E54
		// (set) Token: 0x06002A4A RID: 10826 RVA: 0x0025AC6B File Offset: 0x00258E6B
		public long LastLifeMagicTick { get; set; }

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06002A4B RID: 10827 RVA: 0x0025AC74 File Offset: 0x00258E74
		// (set) Token: 0x06002A4C RID: 10828 RVA: 0x0025ACCC File Offset: 0x00258ECC
		public Point CurrentGrid
		{
			get
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
				return new Point((double)((int)(this.FallPoint.X / (double)gameMap.MapGridWidth)), (double)((int)(this.FallPoint.Y / (double)gameMap.MapGridHeight)));
			}
			set
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
				this.FallPoint = new Point((double)((int)(value.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2))), (double)((int)(value.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2))));
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06002A4D RID: 10829 RVA: 0x0025AD34 File Offset: 0x00258F34
		// (set) Token: 0x06002A4E RID: 10830 RVA: 0x0025AD4C File Offset: 0x00258F4C
		public Point CurrentPos
		{
			get
			{
				return this.FallPoint;
			}
			set
			{
				this.FallPoint = value;
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06002A4F RID: 10831 RVA: 0x0025AD58 File Offset: 0x00258F58
		public int CurrentMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06002A50 RID: 10832 RVA: 0x0025AD70 File Offset: 0x00258F70
		public int CurrentCopyMapID
		{
			get
			{
				return this.CopyMapID;
			}
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06002A51 RID: 10833 RVA: 0x0025AD88 File Offset: 0x00258F88
		// (set) Token: 0x06002A52 RID: 10834 RVA: 0x0025AD9F File Offset: 0x00258F9F
		public Dircetions CurrentDir { get; set; }

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06002A53 RID: 10835 RVA: 0x0025ADA8 File Offset: 0x00258FA8
		// (set) Token: 0x06002A54 RID: 10836 RVA: 0x0025ADBF File Offset: 0x00258FBF
		public List<int> PassiveEffectList { get; set; }

		// Token: 0x06002A55 RID: 10837 RVA: 0x0025ADC8 File Offset: 0x00258FC8
		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			return default(T);
		}

		// Token: 0x04003999 RID: 14745
		public List<int> TeamRoleIDs = null;

		// Token: 0x0400399A RID: 14746
		public List<long> TeamRoleDamages;

		// Token: 0x0400399B RID: 14747
		public int TeamID = -1;

		// Token: 0x0400399C RID: 14748
		private Dictionary<int, bool> _GoodsIDDict = new Dictionary<int, bool>();

		// Token: 0x0400399D RID: 14749
		private Dictionary<int, int> _GoodsIDToRolesDict = new Dictionary<int, int>();

		// Token: 0x0400399E RID: 14750
		private Dictionary<int, long> _RolesTicksDict = new Dictionary<int, long>();

		// Token: 0x0400399F RID: 14751
		public List<GoodsData> GoodsDataList = null;

		// Token: 0x040039A0 RID: 14752
		public int MapCode = -1;

		// Token: 0x040039A1 RID: 14753
		public Point FallPoint;

		// Token: 0x040039A2 RID: 14754
		public int CopyMapID = -1;

		// Token: 0x040039A3 RID: 14755
		public string KilledMonsterName = "";

		// Token: 0x040039A4 RID: 14756
		public int BelongTo = -1;

		// Token: 0x040039A5 RID: 14757
		public int FallLevel = 0;

		// Token: 0x040039A6 RID: 14758
		public int PickRoleID = -1;

		// Token: 0x040039A7 RID: 14759
		public bool CanPickUp = true;
	}
}
