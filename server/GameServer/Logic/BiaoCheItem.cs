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
		
		public int CurrentMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		// Token: 0x170000B3 RID: 179
		
		public int CurrentCopyMapID
		{
			get
			{
				return this.CopyMapID;
			}
		}

		// Token: 0x170000B4 RID: 180
		
		
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
