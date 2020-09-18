using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using Server.Data;

namespace GameServer.Logic
{
	
	public class BiaoCheItem : IObject
	{
		
		public BiaoCheData GetBiaoCheData()
		{
			return this._MyBiaoCheData;
		}

		
		
		
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

		
		
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_BIAOCHE;
			}
		}

		
		public int GetObjectID()
		{
			return this.BiaoCheID;
		}

		
		
		
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

		
		
		public int CurrentMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		
		
		public int CurrentCopyMapID
		{
			get
			{
				return this.CopyMapID;
			}
		}

		
		
		
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

		
		
		
		public List<int> PassiveEffectList { get; set; }

		
		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			return default(T);
		}

		
		private object Mutex = new object();

		
		private BiaoCheData _MyBiaoCheData = new BiaoCheData();

		
		private long _ReportPosTicks = 0L;

		
		private int _CurrentAction = 0;

		
		private int _CopyMapID = -1;

		
		private int _CurrentLifeV;

		
		private int _AttackedRoleID;

		
		private long _LastAttackedTick = 0L;

		
		private int _CurrentGridX = -1;

		
		private int _CurrentGridY = -1;

		
		private Dictionary<string, bool> _CurrentObjsDict = null;

		
		private Dictionary<string, bool> _CurrentGridsDict = null;

		
		public bool _HandledDead = false;

		
		private long _BiaoCheDeadTicks = 0L;

		
		private int _DestNPC = 0;

		
		private int _MinLevel = 0;

		
		private int _MaxLevel = 0;

		
		private long _LastLifeMagicTick = TimeUtil.NOW();
	}
}
