using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Interface;

namespace GameServer.Logic
{
	// Token: 0x02000622 RID: 1570
	public class Decoration : IObject
	{
		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06002006 RID: 8198 RVA: 0x001BB500 File Offset: 0x001B9700
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_DECO;
			}
		}

		// Token: 0x06002007 RID: 8199 RVA: 0x001BB514 File Offset: 0x001B9714
		public int GetObjectID()
		{
			return this.AutoID;
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06002008 RID: 8200 RVA: 0x001BB52C File Offset: 0x001B972C
		// (set) Token: 0x06002009 RID: 8201 RVA: 0x001BB543 File Offset: 0x001B9743
		public long LastLifeMagicTick { get; set; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x0600200A RID: 8202 RVA: 0x001BB54C File Offset: 0x001B974C
		// (set) Token: 0x0600200B RID: 8203 RVA: 0x001BB5A4 File Offset: 0x001B97A4
		public Point CurrentGrid
		{
			get
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
				return new Point((double)((int)(this.Pos.X / (double)gameMap.MapGridWidth)), (double)((int)(this.Pos.Y / (double)gameMap.MapGridHeight)));
			}
			set
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
				this.Pos = new Point((double)((int)(value.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2))), (double)((int)(value.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2))));
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x0600200C RID: 8204 RVA: 0x001BB60C File Offset: 0x001B980C
		// (set) Token: 0x0600200D RID: 8205 RVA: 0x001BB624 File Offset: 0x001B9824
		public Point CurrentPos
		{
			get
			{
				return this.Pos;
			}
			set
			{
				this.Pos = value;
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x0600200E RID: 8206 RVA: 0x001BB630 File Offset: 0x001B9830
		public int CurrentMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x0600200F RID: 8207 RVA: 0x001BB648 File Offset: 0x001B9848
		public int CurrentCopyMapID
		{
			get
			{
				return this.CopyMapID;
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06002010 RID: 8208 RVA: 0x001BB660 File Offset: 0x001B9860
		// (set) Token: 0x06002011 RID: 8209 RVA: 0x001BB677 File Offset: 0x001B9877
		public Dircetions CurrentDir { get; set; }

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06002012 RID: 8210 RVA: 0x001BB680 File Offset: 0x001B9880
		// (set) Token: 0x06002013 RID: 8211 RVA: 0x001BB697 File Offset: 0x001B9897
		public List<int> PassiveEffectList { get; set; }

		// Token: 0x06002014 RID: 8212 RVA: 0x001BB6A0 File Offset: 0x001B98A0
		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			return default(T);
		}

		// Token: 0x04002CC7 RID: 11463
		public int AutoID;

		// Token: 0x04002CC8 RID: 11464
		public int DecoID;

		// Token: 0x04002CC9 RID: 11465
		public int MapCode = -1;

		// Token: 0x04002CCA RID: 11466
		public Point Pos;

		// Token: 0x04002CCB RID: 11467
		public int CopyMapID = -1;

		// Token: 0x04002CCC RID: 11468
		public long StartTicks = 0L;

		// Token: 0x04002CCD RID: 11469
		public int MaxLiveTicks = 0;

		// Token: 0x04002CCE RID: 11470
		public int AlphaTicks = 0;
	}
}
