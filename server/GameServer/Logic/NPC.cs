using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Interface;

namespace GameServer.Logic
{
	// Token: 0x0200076C RID: 1900
	public class NPC : IObject
	{
		// Token: 0x17000396 RID: 918
		// (get) Token: 0x060030D2 RID: 12498 RVA: 0x002B58CC File Offset: 0x002B3ACC
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_NPC;
			}
		}

		// Token: 0x060030D3 RID: 12499 RVA: 0x002B58E0 File Offset: 0x002B3AE0
		public int GetObjectID()
		{
			return this.NpcID;
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x060030D4 RID: 12500 RVA: 0x002B58F8 File Offset: 0x002B3AF8
		// (set) Token: 0x060030D5 RID: 12501 RVA: 0x002B590F File Offset: 0x002B3B0F
		public long LastLifeMagicTick { get; set; }

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x060030D6 RID: 12502 RVA: 0x002B5918 File Offset: 0x002B3B18
		// (set) Token: 0x060030D7 RID: 12503 RVA: 0x002B5930 File Offset: 0x002B3B30
		public Point CurrentGrid
		{
			get
			{
				return this.GridPoint;
			}
			set
			{
				this.GridPoint = value;
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x060030D8 RID: 12504 RVA: 0x002B593C File Offset: 0x002B3B3C
		// (set) Token: 0x060030D9 RID: 12505 RVA: 0x002B5954 File Offset: 0x002B3B54
		public Point CurrentPos
		{
			get
			{
				return this._CurrentPos;
			}
			set
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
				this.GridPoint = new Point((double)((int)(value.X / (double)gameMap.MapGridWidth)), (double)((int)(value.Y / (double)gameMap.MapGridHeight)));
				this._CurrentPos = value;
			}
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x060030DA RID: 12506 RVA: 0x002B59AC File Offset: 0x002B3BAC
		public int CurrentMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x060030DB RID: 12507 RVA: 0x002B59C4 File Offset: 0x002B3BC4
		public int CurrentCopyMapID
		{
			get
			{
				return this.CopyMapID;
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x060030DC RID: 12508 RVA: 0x002B59DC File Offset: 0x002B3BDC
		// (set) Token: 0x060030DD RID: 12509 RVA: 0x002B59F3 File Offset: 0x002B3BF3
		public Dircetions CurrentDir { get; set; }

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x060030DE RID: 12510 RVA: 0x002B59FC File Offset: 0x002B3BFC
		// (set) Token: 0x060030DF RID: 12511 RVA: 0x002B5A13 File Offset: 0x002B3C13
		public List<int> PassiveEffectList { get; set; }

		// Token: 0x060030E0 RID: 12512 RVA: 0x002B5A1C File Offset: 0x002B3C1C
		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			return default(T);
		}

		// Token: 0x04003D63 RID: 15715
		public int NpcID;

		// Token: 0x04003D64 RID: 15716
		public int MapCode = -1;

		// Token: 0x04003D65 RID: 15717
		public Point GridPoint;

		// Token: 0x04003D66 RID: 15718
		public int CopyMapID = -1;

		// Token: 0x04003D67 RID: 15719
		public byte[] RoleBufferData = null;

		// Token: 0x04003D68 RID: 15720
		private Point _CurrentPos = new Point(0.0, 0.0);

		// Token: 0x04003D69 RID: 15721
		public bool ShowNpc = true;
	}
}
