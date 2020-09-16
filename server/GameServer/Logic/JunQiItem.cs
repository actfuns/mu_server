using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class JunQiItem : IObject
	{
		
		public JunQiData GetJunQiData()
		{
			return this._MyJunQiData;
		}

		
		
		
		public int JunQiID
		{
			get
			{
				int junQiID;
				lock (this.Mutex)
				{
					junQiID = this._MyJunQiData.JunQiID;
				}
				return junQiID;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.JunQiID = value;
				}
			}
		}

		
		
		
		public string QiName
		{
			get
			{
				string qiName;
				lock (this.Mutex)
				{
					qiName = this._MyJunQiData.QiName;
				}
				return qiName;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.QiName = value;
				}
			}
		}

		
		
		
		public int JunQiLevel
		{
			get
			{
				int junQiLevel;
				lock (this.Mutex)
				{
					junQiLevel = this._MyJunQiData.JunQiLevel;
				}
				return junQiLevel;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.JunQiLevel = value;
				}
			}
		}

		
		
		
		public int ZoneID
		{
			get
			{
				int zoneID;
				lock (this.Mutex)
				{
					zoneID = this._MyJunQiData.ZoneID;
				}
				return zoneID;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.ZoneID = value;
				}
			}
		}

		
		
		
		public int BHID
		{
			get
			{
				int bhid;
				lock (this.Mutex)
				{
					bhid = this._MyJunQiData.BHID;
				}
				return bhid;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.BHID = value;
				}
			}
		}

		
		
		
		public string BHName
		{
			get
			{
				string bhname;
				lock (this.Mutex)
				{
					bhname = this._MyJunQiData.BHName;
				}
				return bhname;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.BHName = value;
				}
			}
		}

		
		
		
		public int QiZuoNPC
		{
			get
			{
				int qiZuoNPC;
				lock (this.Mutex)
				{
					qiZuoNPC = this._MyJunQiData.QiZuoNPC;
				}
				return qiZuoNPC;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.QiZuoNPC = value;
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
					mapCode = this._MyJunQiData.MapCode;
				}
				return mapCode;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.MapCode = value;
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
					posX = this._MyJunQiData.PosX;
				}
				return posX;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.PosX = value;
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
					posY = this._MyJunQiData.PosY;
				}
				return posY;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.PosY = value;
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
					direction = this._MyJunQiData.Direction;
				}
				return direction;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.Direction = value;
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
					lifeV = this._MyJunQiData.LifeV;
				}
				return lifeV;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.LifeV = value;
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
					cutLifeV = this._MyJunQiData.CutLifeV;
				}
				return cutLifeV;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.CutLifeV = value;
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
					startTime = this._MyJunQiData.StartTime;
				}
				return startTime;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.StartTime = value;
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
					bodyCode = this._MyJunQiData.BodyCode;
				}
				return bodyCode;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.BodyCode = value;
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
					picCode = this._MyJunQiData.PicCode;
				}
				return picCode;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyJunQiData.PicCode = value;
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

		
		
		
		public long JunQiDeadTicks
		{
			get
			{
				long junQiDeadTicks;
				lock (this.Mutex)
				{
					junQiDeadTicks = this._JunQiDeadTicks;
				}
				return junQiDeadTicks;
			}
			set
			{
				lock (this.Mutex)
				{
					this._JunQiDeadTicks = value;
				}
			}
		}

		
		
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_JUNQI;
			}
		}

		
		public int GetObjectID()
		{
			return this.JunQiID;
		}

		
		
		
		public long LastLifeMagicTick { get; set; }

		
		
		
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

		
		private bool CheckAttackerListEfficiency()
		{
			bool result;
			if (TimeUtil.NOW() - this._LastLogAttackerTicks > 30000L)
			{
				lock (this._AttackerLogDict)
				{
					this._AttackerLogDict.Clear();
				}
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		
		public void AddAttacker(int roleID, int injured, Monster DSPet = null)
		{
			lock (this._AttackerLogDict)
			{
				this._LastLogAttackerTicks = TimeUtil.NOW();
				JunQiAttackerLog attacker = null;
				if (!this._AttackerLogDict.TryGetValue(roleID, out attacker))
				{
					attacker = new JunQiAttackerLog();
					attacker.RoleId = roleID;
					this._AttackerLogDict[roleID] = attacker;
				}
				attacker.LastAttackMs = this._LastLogAttackerTicks;
				if (null == DSPet)
				{
					attacker.TotalInjured += (long)injured;
				}
				else
				{
					attacker.TotalInjuredByPet += (long)injured;
				}
			}
		}

		
		public void RemoveAttacker(int roleID)
		{
			lock (this._AttackerLogDict)
			{
				this._AttackerLogDict.Remove(roleID);
			}
		}

		
		public bool IsAttackedBy(int roleID)
		{
			this.CheckAttackerListEfficiency();
			lock (this._AttackerLogDict)
			{
				if (this._AttackerLogDict.ContainsKey(roleID))
				{
					return true;
				}
			}
			return false;
		}

		
		public int GetAttackerFromList()
		{
			this.CheckAttackerListEfficiency();
			int attackerRid = -1;
			long maxInjured = 0L;
			long nowTicks = TimeUtil.NOW();
			lock (this._AttackerLogDict)
			{
				foreach (JunQiAttackerLog attacker in this._AttackerLogDict.Values)
				{
					if (nowTicks - attacker.LastAttackMs < 30000L)
					{
						if (attacker.TotalInjured > maxInjured || attacker.TotalInjuredByPet > maxInjured)
						{
							maxInjured = ((attacker.TotalInjured > attacker.TotalInjuredByPet) ? attacker.TotalInjured : attacker.TotalInjuredByPet);
							attackerRid = attacker.RoleId;
						}
					}
				}
			}
			return attackerRid;
		}

		
		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			return default(T);
		}

		
		private object Mutex = new object();

		
		private JunQiData _MyJunQiData = new JunQiData();

		
		public SceneUIClasses ManagerType = SceneUIClasses.Normal;

		
		private int _CopyMapID = -1;

		
		private int _CurrentLifeV;

		
		private int _AttackedRoleID;

		
		private long _LastAttackedTick = 0L;

		
		private int _CurrentGridX = -1;

		
		private int _CurrentGridY = -1;

		
		private Dictionary<string, bool> _CurrentObjsDict = null;

		
		private Dictionary<string, bool> _CurrentGridsDict = null;

		
		public bool _HandledDead = false;

		
		private long _JunQiDeadTicks = 0L;

		
		private long _LastLogAttackerTicks = 0L;

		
		private Dictionary<int, JunQiAttackerLog> _AttackerLogDict = new Dictionary<int, JunQiAttackerLog>();
	}
}
