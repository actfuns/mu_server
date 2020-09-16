using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using Server.Data;

namespace GameServer.Logic
{
	
	public class FakeRoleItem : IObject
	{
		
		public FakeRoleData GetFakeRoleData()
		{
			return this._MyFakeRoleData;
		}

		
		
		
		public int FakeRoleID
		{
			get
			{
				int fakeRoleID;
				lock (this.Mutex)
				{
					fakeRoleID = this._MyFakeRoleData.FakeRoleID;
				}
				return fakeRoleID;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyFakeRoleData.FakeRoleID = value;
				}
			}
		}

		
		
		
		public int FakeRoleType
		{
			get
			{
				int fakeRoleType;
				lock (this.Mutex)
				{
					fakeRoleType = this._MyFakeRoleData.FakeRoleType;
				}
				return fakeRoleType;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyFakeRoleData.FakeRoleType = value;
				}
			}
		}

		
		
		
		public int ToExtensionID
		{
			get
			{
				int toExtensionID;
				lock (this.Mutex)
				{
					toExtensionID = this._MyFakeRoleData.ToExtensionID;
				}
				return toExtensionID;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyFakeRoleData.ToExtensionID = value;
				}
			}
		}

		
		
		
		public RoleDataMini MyRoleDataMini
		{
			get
			{
				RoleDataMini myRoleDataMini;
				lock (this.Mutex)
				{
					myRoleDataMini = this._MyFakeRoleData.MyRoleDataMini;
				}
				return myRoleDataMini;
			}
			set
			{
				lock (this.Mutex)
				{
					this._MyFakeRoleData.MyRoleDataMini = value;
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
				int lifeV;
				lock (this)
				{
					lifeV = this._MyFakeRoleData.MyRoleDataMini.LifeV;
				}
				return lifeV;
			}
			set
			{
				lock (this)
				{
					this._MyFakeRoleData.MyRoleDataMini.LifeV = value;
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

		
		
		
		public long FakeRoleDeadTicks
		{
			get
			{
				long fakeRoleDeadTicks;
				lock (this.Mutex)
				{
					fakeRoleDeadTicks = this._FakeRoleDeadTicks;
				}
				return fakeRoleDeadTicks;
			}
			set
			{
				lock (this.Mutex)
				{
					this._FakeRoleDeadTicks = value;
				}
			}
		}

		
		
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_FAKEROLE;
			}
		}

		
		public int GetObjectID()
		{
			return this.FakeRoleID;
		}

		
		
		
		public long LastLifeMagicTick { get; set; }

		
		
		
		public Point CurrentGrid
		{
			get
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MyRoleDataMini.MapCode];
				return new Point((double)(this.MyRoleDataMini.PosX / gameMap.MapGridWidth), (double)(this.MyRoleDataMini.PosY / gameMap.MapGridHeight));
			}
			set
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MyRoleDataMini.MapCode];
				this.MyRoleDataMini.PosX = (int)(value.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2));
				this.MyRoleDataMini.PosY = (int)(value.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
			}
		}

		
		
		
		public Point CurrentPos
		{
			get
			{
				return new Point((double)this.MyRoleDataMini.PosX, (double)this.MyRoleDataMini.PosY);
			}
			set
			{
				this.MyRoleDataMini.PosX = (int)value.X;
				this.MyRoleDataMini.PosY = (int)value.Y;
			}
		}

		
		
		public int CurrentMapCode
		{
			get
			{
				return this.MyRoleDataMini.MapCode;
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
				return (Dircetions)this.MyRoleDataMini.RoleDirection;
			}
			set
			{
				this.MyRoleDataMini.RoleDirection = (int)value;
			}
		}

		
		
		
		public List<int> PassiveEffectList { get; set; }

		
		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			return default(T);
		}

		
		private bool CheckAttackerListEfficiency()
		{
			bool result;
			if (TimeUtil.NOW() - this._LastLogAttackerTicks > 30000L)
			{
				lock (this._AttackerDict)
				{
					this._AttackerDict.Clear();
					this._AttackerTicksDict.Clear();
				}
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		
		public void AddAttacker(int roleID, int injured)
		{
			lock (this._AttackerDict)
			{
				this._LastLogAttackerTicks = TimeUtil.NOW();
				int oldInjured = 0;
				this._AttackerDict.TryGetValue(roleID, out oldInjured);
				this._AttackerDict[roleID] = oldInjured + injured;
				this._AttackerTicksDict[roleID] = this._LastLogAttackerTicks;
			}
		}

		
		public void RemoveAttacker(int roleID)
		{
			lock (this._AttackerDict)
			{
				this._AttackerDict.Remove(roleID);
				this._AttackerTicksDict.Remove(roleID);
			}
		}

		
		public bool IsAttackedBy(int roleID)
		{
			this.CheckAttackerListEfficiency();
			lock (this._AttackerDict)
			{
				if (this._AttackerDict.ContainsKey(roleID))
				{
					return true;
				}
			}
			return false;
		}

		
		public int GetAttackerFromList()
		{
			this.CheckAttackerListEfficiency();
			int attacker = -1;
			int maxInjured = 0;
			long nowTicks = TimeUtil.NOW();
			lock (this._AttackerDict)
			{
				foreach (int key in this._AttackerDict.Keys)
				{
					long lastAttackTicks = 0L;
					this._AttackerTicksDict.TryGetValue(key, out lastAttackTicks);
					if (nowTicks - lastAttackTicks < 30000L)
					{
						int injured = this._AttackerDict[key];
						if (injured > maxInjured)
						{
							maxInjured = injured;
							attacker = key;
						}
					}
				}
			}
			return attacker;
		}

		
		private object Mutex = new object();

		
		private FakeRoleData _MyFakeRoleData = new FakeRoleData();

		
		private int _CopyMapID = -1;

		
		private int _AttackedRoleID;

		
		private long _LastAttackedTick = 0L;

		
		private int _CurrentGridX = -1;

		
		private int _CurrentGridY = -1;

		
		private Dictionary<string, bool> _CurrentObjsDict = null;

		
		private Dictionary<string, bool> _CurrentGridsDict = null;

		
		public bool _HandledDead = false;

		
		private long _FakeRoleDeadTicks = 0L;

		
		private long _LastLogAttackerTicks = 0L;

		
		private Dictionary<int, int> _AttackerDict = new Dictionary<int, int>();

		
		private Dictionary<int, long> _AttackerTicksDict = new Dictionary<int, long>();
	}
}
