using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020005C9 RID: 1481
	public class BiaoCheItem : IObject
	{
		// Token: 0x06001B1D RID: 6941 RVA: 0x0019BAC8 File Offset: 0x00199CC8
		public BiaoCheData GetBiaoCheData()
		{
			return this._MyBiaoCheData;
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06001B1E RID: 6942 RVA: 0x0019BAE0 File Offset: 0x00199CE0
		// (set) Token: 0x06001B1F RID: 6943 RVA: 0x0019BB34 File Offset: 0x00199D34
		public int OwnerRoleID
		{
			get
			{
				int ownerRoleID;
				lock (this.Mutex)
				{
					ownerRoleID = this._MyBiaoCheData.OwnerRoleID;
				}
				return ownerRoleID;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.OwnerRoleID = value;
				}
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06001B20 RID: 6944 RVA: 0x0019BB88 File Offset: 0x00199D88
		// (set) Token: 0x06001B21 RID: 6945 RVA: 0x0019BBDC File Offset: 0x00199DDC
		public int BiaoCheID
		{
			get
			{
				int biaoCheID;
				lock (this.Mutex)
				{
					biaoCheID = this._MyBiaoCheData.BiaoCheID;
				}
				return biaoCheID;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.BiaoCheID = value;
				}
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06001B22 RID: 6946 RVA: 0x0019BC30 File Offset: 0x00199E30
		// (set) Token: 0x06001B23 RID: 6947 RVA: 0x0019BC84 File Offset: 0x00199E84
		public string BiaoCheName
		{
			get
			{
				string biaoCheName;
				lock (this.Mutex)
				{
					biaoCheName = this._MyBiaoCheData.BiaoCheName;
				}
				return biaoCheName;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.BiaoCheName = value;
				}
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06001B24 RID: 6948 RVA: 0x0019BCD8 File Offset: 0x00199ED8
		// (set) Token: 0x06001B25 RID: 6949 RVA: 0x0019BD2C File Offset: 0x00199F2C
		public int YaBiaoID
		{
			get
			{
				int yaBiaoID;
				lock (this.Mutex)
				{
					yaBiaoID = this._MyBiaoCheData.YaBiaoID;
				}
				return yaBiaoID;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.YaBiaoID = value;
				}
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06001B26 RID: 6950 RVA: 0x0019BD80 File Offset: 0x00199F80
		// (set) Token: 0x06001B27 RID: 6951 RVA: 0x0019BDD4 File Offset: 0x00199FD4
		public int MapCode
		{
			get
			{
				int mapCode;
				lock (this.Mutex)
				{
					mapCode = this._MyBiaoCheData.MapCode;
				}
				return mapCode;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.MapCode = value;
				}
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06001B28 RID: 6952 RVA: 0x0019BE28 File Offset: 0x0019A028
		// (set) Token: 0x06001B29 RID: 6953 RVA: 0x0019BE7C File Offset: 0x0019A07C
		public int PosX
		{
			get
			{
				int posX;
				lock (this.Mutex)
				{
					posX = this._MyBiaoCheData.PosX;
				}
				return posX;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.PosX = value;
				}
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06001B2A RID: 6954 RVA: 0x0019BED0 File Offset: 0x0019A0D0
		// (set) Token: 0x06001B2B RID: 6955 RVA: 0x0019BF24 File Offset: 0x0019A124
		public int PosY
		{
			get
			{
				int posY;
				lock (this.Mutex)
				{
					posY = this._MyBiaoCheData.PosY;
				}
				return posY;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.PosY = value;
				}
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06001B2C RID: 6956 RVA: 0x0019BF78 File Offset: 0x0019A178
		// (set) Token: 0x06001B2D RID: 6957 RVA: 0x0019BFCC File Offset: 0x0019A1CC
		public int Direction
		{
			get
			{
				int direction;
				lock (this.Mutex)
				{
					direction = this._MyBiaoCheData.Direction;
				}
				return direction;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.Direction = value;
				}
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06001B2E RID: 6958 RVA: 0x0019C020 File Offset: 0x0019A220
		// (set) Token: 0x06001B2F RID: 6959 RVA: 0x0019C070 File Offset: 0x0019A270
		public int LifeV
		{
			get
			{
				int lifeV;
				lock (this.Mutex)
				{
					lifeV = this._MyBiaoCheData.LifeV;
				}
				return lifeV;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.LifeV = value;
				}
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06001B30 RID: 6960 RVA: 0x0019C0C0 File Offset: 0x0019A2C0
		// (set) Token: 0x06001B31 RID: 6961 RVA: 0x0019C110 File Offset: 0x0019A310
		public int CutLifeV
		{
			get
			{
				int cutLifeV;
				lock (this.Mutex)
				{
					cutLifeV = this._MyBiaoCheData.CutLifeV;
				}
				return cutLifeV;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.CutLifeV = value;
				}
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06001B32 RID: 6962 RVA: 0x0019C160 File Offset: 0x0019A360
		// (set) Token: 0x06001B33 RID: 6963 RVA: 0x0019C1B0 File Offset: 0x0019A3B0
		public long StartTime
		{
			get
			{
				long startTime;
				lock (this.Mutex)
				{
					startTime = this._MyBiaoCheData.StartTime;
				}
				return startTime;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.StartTime = value;
				}
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06001B34 RID: 6964 RVA: 0x0019C200 File Offset: 0x0019A400
		// (set) Token: 0x06001B35 RID: 6965 RVA: 0x0019C250 File Offset: 0x0019A450
		public int BodyCode
		{
			get
			{
				int bodyCode;
				lock (this.Mutex)
				{
					bodyCode = this._MyBiaoCheData.BodyCode;
				}
				return bodyCode;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.BodyCode = value;
				}
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06001B36 RID: 6966 RVA: 0x0019C2A0 File Offset: 0x0019A4A0
		// (set) Token: 0x06001B37 RID: 6967 RVA: 0x0019C2F0 File Offset: 0x0019A4F0
		public int PicCode
		{
			get
			{
				int picCode;
				lock (this.Mutex)
				{
					picCode = this._MyBiaoCheData.PicCode;
				}
				return picCode;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.PicCode = value;
				}
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06001B38 RID: 6968 RVA: 0x0019C340 File Offset: 0x0019A540
		// (set) Token: 0x06001B39 RID: 6969 RVA: 0x0019C394 File Offset: 0x0019A594
		public string OwnerRoleName
		{
			get
			{
				string ownerRoleName;
				lock (this.Mutex)
				{
					ownerRoleName = this._MyBiaoCheData.OwnerRoleName;
				}
				return ownerRoleName;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyBiaoCheData.OwnerRoleName = value;
				}
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06001B3A RID: 6970 RVA: 0x0019C3E8 File Offset: 0x0019A5E8
		// (set) Token: 0x06001B3B RID: 6971 RVA: 0x0019C434 File Offset: 0x0019A634
		public long ReportPosTicks
		{
			get
			{
				long reportPosTicks;
				lock (this.Mutex)
				{
					reportPosTicks = this._ReportPosTicks;
				}
				return reportPosTicks;
			}
			set
			{
				lock (this.Mutex)
				{
					this._ReportPosTicks = value;
				}
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06001B3C RID: 6972 RVA: 0x0019C480 File Offset: 0x0019A680
		// (set) Token: 0x06001B3D RID: 6973 RVA: 0x0019C4CC File Offset: 0x0019A6CC
		public int CurrentAction
		{
			get
			{
				int currentAction;
				lock (this.Mutex)
				{
					currentAction = this._CurrentAction;
				}
				return currentAction;
			}
			set
			{
				lock (this.Mutex)
				{
					if (this._CurrentAction != value)
					{
						this._CurrentAction = value;
					}
				}
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06001B3E RID: 6974 RVA: 0x0019C528 File Offset: 0x0019A728
		// (set) Token: 0x06001B3F RID: 6975 RVA: 0x0019C570 File Offset: 0x0019A770
		public int CopyMapID
		{
			get
			{
				int copyMapID;
				lock (this)
				{
					copyMapID = this._CopyMapID;
				}
				return copyMapID;
			}
			set
			{
				lock (this)
				{
					this._CopyMapID = value;
				}
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06001B40 RID: 6976 RVA: 0x0019C5B8 File Offset: 0x0019A7B8
		// (set) Token: 0x06001B41 RID: 6977 RVA: 0x0019C604 File Offset: 0x0019A804
		public int CurrentLifeV
		{
			get
			{
				int currentLifeV;
				lock (this.Mutex)
				{
					currentLifeV = this._CurrentLifeV;
				}
				return currentLifeV;
			}
			set
			{
				lock (this.Mutex)
				{
					this._CurrentLifeV = value;
				}
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06001B42 RID: 6978 RVA: 0x0019C650 File Offset: 0x0019A850
		// (set) Token: 0x06001B43 RID: 6979 RVA: 0x0019C698 File Offset: 0x0019A898
		public int AttackedRoleID
		{
			get
			{
				int attackedRoleID;
				lock (this)
				{
					attackedRoleID = this._AttackedRoleID;
				}
				return attackedRoleID;
			}
			set
			{
				lock (this)
				{
					long ticks = TimeUtil.NOW();
					if (this._AttackedRoleID == value)
					{
						this._LastAttackedTick = ticks;
					}
					else if (ticks - this._LastAttackedTick >= 10000L)
					{
						this._LastAttackedTick = ticks;
						this._AttackedRoleID = value;
					}
				}
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06001B44 RID: 6980 RVA: 0x0019C720 File Offset: 0x0019A920
		// (set) Token: 0x06001B45 RID: 6981 RVA: 0x0019C768 File Offset: 0x0019A968
		public int CurrentGridX
		{
			get
			{
				int currentGridX;
				lock (this)
				{
					currentGridX = this._CurrentGridX;
				}
				return currentGridX;
			}
			set
			{
				lock (this)
				{
					this._CurrentGridX = value;
				}
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06001B46 RID: 6982 RVA: 0x0019C7B0 File Offset: 0x0019A9B0
		// (set) Token: 0x06001B47 RID: 6983 RVA: 0x0019C7F8 File Offset: 0x0019A9F8
		public int CurrentGridY
		{
			get
			{
				int currentGridY;
				lock (this)
				{
					currentGridY = this._CurrentGridY;
				}
				return currentGridY;
			}
			set
			{
				lock (this)
				{
					this._CurrentGridY = value;
				}
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06001B48 RID: 6984 RVA: 0x0019C840 File Offset: 0x0019AA40
		// (set) Token: 0x06001B49 RID: 6985 RVA: 0x0019C888 File Offset: 0x0019AA88
		public Dictionary<string, bool> CurrentObjsDict
		{
			get
			{
				Dictionary<string, bool> currentObjsDict;
				lock (this)
				{
					currentObjsDict = this._CurrentObjsDict;
				}
				return currentObjsDict;
			}
			set
			{
				lock (this)
				{
					this._CurrentObjsDict = value;
				}
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06001B4A RID: 6986 RVA: 0x0019C8D0 File Offset: 0x0019AAD0
		// (set) Token: 0x06001B4B RID: 6987 RVA: 0x0019C918 File Offset: 0x0019AB18
		public Dictionary<string, bool> CurrentGridsDict
		{
			get
			{
				Dictionary<string, bool> currentGridsDict;
				lock (this)
				{
					currentGridsDict = this._CurrentGridsDict;
				}
				return currentGridsDict;
			}
			set
			{
				lock (this)
				{
					this._CurrentGridsDict = value;
				}
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06001B4C RID: 6988 RVA: 0x0019C960 File Offset: 0x0019AB60
		// (set) Token: 0x06001B4D RID: 6989 RVA: 0x0019C9A8 File Offset: 0x0019ABA8
		public bool HandledDead
		{
			get
			{
				bool handledDead;
				lock (this)
				{
					handledDead = this._HandledDead;
				}
				return handledDead;
			}
			set
			{
				lock (this)
				{
					this._HandledDead = value;
				}
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06001B4E RID: 6990 RVA: 0x0019C9F0 File Offset: 0x0019ABF0
		// (set) Token: 0x06001B4F RID: 6991 RVA: 0x0019CA3C File Offset: 0x0019AC3C
		public long BiaoCheDeadTicks
		{
			get
			{
				long biaoCheDeadTicks;
				lock (this.Mutex)
				{
					biaoCheDeadTicks = this._BiaoCheDeadTicks;
				}
				return biaoCheDeadTicks;
			}
			set
			{
				lock (this.Mutex)
				{
					this._BiaoCheDeadTicks = value;
				}
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06001B50 RID: 6992 RVA: 0x0019CA88 File Offset: 0x0019AC88
		// (set) Token: 0x06001B51 RID: 6993 RVA: 0x0019CAD4 File Offset: 0x0019ACD4
		public int DestNPC
		{
			get
			{
				int destNPC;
				lock (this.Mutex)
				{
					destNPC = this._DestNPC;
				}
				return destNPC;
			}
			set
			{
				lock (this.Mutex)
				{
					this._DestNPC = value;
				}
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06001B52 RID: 6994 RVA: 0x0019CB20 File Offset: 0x0019AD20
		// (set) Token: 0x06001B53 RID: 6995 RVA: 0x0019CB6C File Offset: 0x0019AD6C
		public int MinLevel
		{
			get
			{
				int minLevel;
				lock (this.Mutex)
				{
					minLevel = this._MinLevel;
				}
				return minLevel;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MinLevel = value;
				}
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06001B54 RID: 6996 RVA: 0x0019CBB8 File Offset: 0x0019ADB8
		// (set) Token: 0x06001B55 RID: 6997 RVA: 0x0019CC04 File Offset: 0x0019AE04
		public int MaxLevel
		{
			get
			{
				int maxLevel;
				lock (this.Mutex)
				{
					maxLevel = this._MaxLevel;
				}
				return maxLevel;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MaxLevel = value;
				}
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06001B56 RID: 6998 RVA: 0x0019CC50 File Offset: 0x0019AE50
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_BIAOCHE;
			}
		}

		// Token: 0x06001B57 RID: 6999 RVA: 0x0019CC64 File Offset: 0x0019AE64
		public int GetObjectID()
		{
			return this.BiaoCheID;
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06001B58 RID: 7000 RVA: 0x0019CC7C File Offset: 0x0019AE7C
		// (set) Token: 0x06001B59 RID: 7001 RVA: 0x0019CC94 File Offset: 0x0019AE94
		public long LastLifeMagicTick
		{
			get
			{
				return this._LastLifeMagicTick;
			}
			set
			{
				this._LastLifeMagicTick = value;
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06001B5A RID: 7002 RVA: 0x0019CCA0 File Offset: 0x0019AEA0
		// (set) Token: 0x06001B5B RID: 7003 RVA: 0x0019CCEC File Offset: 0x0019AEEC
		public Point CurrentGrid
		{
			get
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
				return new Point((double)(this.PosX / gameMap.MapGridWidth), (double)(this.PosY / gameMap.MapGridHeight));
			}
			set
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
				this.PosX = (int)(value.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2));
				this.PosY = (int)(value.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06001B5C RID: 7004 RVA: 0x0019CD54 File Offset: 0x0019AF54
		// (set) Token: 0x06001B5D RID: 7005 RVA: 0x0019CD79 File Offset: 0x0019AF79
		public Point CurrentPos
		{
			get
			{
				return new Point((double)this.PosX, (double)this.PosY);
			}
			set
			{
				this.PosX = (int)value.X;
				this.PosY = (int)value.Y;
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06001B5E RID: 7006 RVA: 0x0019CD9C File Offset: 0x0019AF9C
		public int CurrentMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06001B5F RID: 7007 RVA: 0x0019CDB4 File Offset: 0x0019AFB4
		public int CurrentCopyMapID
		{
			get
			{
				return this.CopyMapID;
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06001B60 RID: 7008 RVA: 0x0019CDCC File Offset: 0x0019AFCC
		// (set) Token: 0x06001B61 RID: 7009 RVA: 0x0019CDE4 File Offset: 0x0019AFE4
		public Dircetions CurrentDir
		{
			get
			{
				return (Dircetions)this.Direction;
			}
			set
			{
				this.Direction = (int)value;
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06001B62 RID: 7010 RVA: 0x0019CDF0 File Offset: 0x0019AFF0
		// (set) Token: 0x06001B63 RID: 7011 RVA: 0x0019CE07 File Offset: 0x0019B007
		public List<int> PassiveEffectList { get; set; }

		// Token: 0x06001B64 RID: 7012 RVA: 0x0019CE10 File Offset: 0x0019B010
		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			return default(T);
		}

		// Token: 0x040029CE RID: 10702
		private object Mutex = new object();

		// Token: 0x040029CF RID: 10703
		private BiaoCheData _MyBiaoCheData = new BiaoCheData();

		// Token: 0x040029D0 RID: 10704
		private long _ReportPosTicks = 0L;

		// Token: 0x040029D1 RID: 10705
		private int _CurrentAction = 0;

		// Token: 0x040029D2 RID: 10706
		private int _CopyMapID = -1;

		// Token: 0x040029D3 RID: 10707
		private int _CurrentLifeV;

		// Token: 0x040029D4 RID: 10708
		private int _AttackedRoleID;

		// Token: 0x040029D5 RID: 10709
		private long _LastAttackedTick = 0L;

		// Token: 0x040029D6 RID: 10710
		private int _CurrentGridX = -1;

		// Token: 0x040029D7 RID: 10711
		private int _CurrentGridY = -1;

		// Token: 0x040029D8 RID: 10712
		private Dictionary<string, bool> _CurrentObjsDict = null;

		// Token: 0x040029D9 RID: 10713
		private Dictionary<string, bool> _CurrentGridsDict = null;

		// Token: 0x040029DA RID: 10714
		public bool _HandledDead = false;

		// Token: 0x040029DB RID: 10715
		private long _BiaoCheDeadTicks = 0L;

		// Token: 0x040029DC RID: 10716
		private int _DestNPC = 0;

		// Token: 0x040029DD RID: 10717
		private int _MinLevel = 0;

		// Token: 0x040029DE RID: 10718
		private int _MaxLevel = 0;

		// Token: 0x040029DF RID: 10719
		private long _LastLifeMagicTick = TimeUtil.NOW();
	}
}
