using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020006C0 RID: 1728
	public class FakeRoleItem : IObject
	{
		// Token: 0x0600207D RID: 8317 RVA: 0x001BF698 File Offset: 0x001BD898
		public FakeRoleData GetFakeRoleData()
		{
			return this._MyFakeRoleData;
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x0600207E RID: 8318 RVA: 0x001BF6B0 File Offset: 0x001BD8B0
		// (set) Token: 0x0600207F RID: 8319 RVA: 0x001BF704 File Offset: 0x001BD904
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

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06002080 RID: 8320 RVA: 0x001BF758 File Offset: 0x001BD958
		// (set) Token: 0x06002081 RID: 8321 RVA: 0x001BF7AC File Offset: 0x001BD9AC
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

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06002082 RID: 8322 RVA: 0x001BF800 File Offset: 0x001BDA00
		// (set) Token: 0x06002083 RID: 8323 RVA: 0x001BF854 File Offset: 0x001BDA54
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

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06002084 RID: 8324 RVA: 0x001BF8A8 File Offset: 0x001BDAA8
		// (set) Token: 0x06002085 RID: 8325 RVA: 0x001BF8FC File Offset: 0x001BDAFC
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

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06002086 RID: 8326 RVA: 0x001BF950 File Offset: 0x001BDB50
		// (set) Token: 0x06002087 RID: 8327 RVA: 0x001BF998 File Offset: 0x001BDB98
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

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06002088 RID: 8328 RVA: 0x001BF9E0 File Offset: 0x001BDBE0
		// (set) Token: 0x06002089 RID: 8329 RVA: 0x001BFA30 File Offset: 0x001BDC30
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

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x0600208A RID: 8330 RVA: 0x001BFA80 File Offset: 0x001BDC80
		// (set) Token: 0x0600208B RID: 8331 RVA: 0x001BFAC8 File Offset: 0x001BDCC8
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

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x0600208C RID: 8332 RVA: 0x001BFB50 File Offset: 0x001BDD50
		// (set) Token: 0x0600208D RID: 8333 RVA: 0x001BFB98 File Offset: 0x001BDD98
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

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x0600208E RID: 8334 RVA: 0x001BFBE0 File Offset: 0x001BDDE0
		// (set) Token: 0x0600208F RID: 8335 RVA: 0x001BFC28 File Offset: 0x001BDE28
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

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06002090 RID: 8336 RVA: 0x001BFC70 File Offset: 0x001BDE70
		// (set) Token: 0x06002091 RID: 8337 RVA: 0x001BFCB8 File Offset: 0x001BDEB8
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

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06002092 RID: 8338 RVA: 0x001BFD00 File Offset: 0x001BDF00
		// (set) Token: 0x06002093 RID: 8339 RVA: 0x001BFD48 File Offset: 0x001BDF48
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

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06002094 RID: 8340 RVA: 0x001BFD90 File Offset: 0x001BDF90
		// (set) Token: 0x06002095 RID: 8341 RVA: 0x001BFDD8 File Offset: 0x001BDFD8
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

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06002096 RID: 8342 RVA: 0x001BFE20 File Offset: 0x001BE020
		// (set) Token: 0x06002097 RID: 8343 RVA: 0x001BFE6C File Offset: 0x001BE06C
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

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06002098 RID: 8344 RVA: 0x001BFEB8 File Offset: 0x001BE0B8
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_FAKEROLE;
			}
		}

		// Token: 0x06002099 RID: 8345 RVA: 0x001BFECC File Offset: 0x001BE0CC
		public int GetObjectID()
		{
			return this.FakeRoleID;
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x0600209A RID: 8346 RVA: 0x001BFEE4 File Offset: 0x001BE0E4
		// (set) Token: 0x0600209B RID: 8347 RVA: 0x001BFEFB File Offset: 0x001BE0FB
		public long LastLifeMagicTick { get; set; }

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x0600209C RID: 8348 RVA: 0x001BFF04 File Offset: 0x001BE104
		// (set) Token: 0x0600209D RID: 8349 RVA: 0x001BFF5C File Offset: 0x001BE15C
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

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x0600209E RID: 8350 RVA: 0x001BFFD0 File Offset: 0x001BE1D0
		// (set) Token: 0x0600209F RID: 8351 RVA: 0x001BFFFF File Offset: 0x001BE1FF
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

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x060020A0 RID: 8352 RVA: 0x001C0028 File Offset: 0x001BE228
		public int CurrentMapCode
		{
			get
			{
				return this.MyRoleDataMini.MapCode;
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x060020A1 RID: 8353 RVA: 0x001C0048 File Offset: 0x001BE248
		public int CurrentCopyMapID
		{
			get
			{
				return this.CopyMapID;
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x060020A2 RID: 8354 RVA: 0x001C0060 File Offset: 0x001BE260
		// (set) Token: 0x060020A3 RID: 8355 RVA: 0x001C007D File Offset: 0x001BE27D
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

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x060020A4 RID: 8356 RVA: 0x001C008C File Offset: 0x001BE28C
		// (set) Token: 0x060020A5 RID: 8357 RVA: 0x001C00A3 File Offset: 0x001BE2A3
		public List<int> PassiveEffectList { get; set; }

		// Token: 0x060020A6 RID: 8358 RVA: 0x001C00AC File Offset: 0x001BE2AC
		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			return default(T);
		}

		// Token: 0x060020A7 RID: 8359 RVA: 0x001C00C8 File Offset: 0x001BE2C8
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

		// Token: 0x060020A8 RID: 8360 RVA: 0x001C014C File Offset: 0x001BE34C
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

		// Token: 0x060020A9 RID: 8361 RVA: 0x001C01D0 File Offset: 0x001BE3D0
		public void RemoveAttacker(int roleID)
		{
			lock (this._AttackerDict)
			{
				this._AttackerDict.Remove(roleID);
				this._AttackerTicksDict.Remove(roleID);
			}
		}

		// Token: 0x060020AA RID: 8362 RVA: 0x001C0230 File Offset: 0x001BE430
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

		// Token: 0x060020AB RID: 8363 RVA: 0x001C029C File Offset: 0x001BE49C
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

		// Token: 0x0400367C RID: 13948
		private object Mutex = new object();

		// Token: 0x0400367D RID: 13949
		private FakeRoleData _MyFakeRoleData = new FakeRoleData();

		// Token: 0x0400367E RID: 13950
		private int _CopyMapID = -1;

		// Token: 0x0400367F RID: 13951
		private int _AttackedRoleID;

		// Token: 0x04003680 RID: 13952
		private long _LastAttackedTick = 0L;

		// Token: 0x04003681 RID: 13953
		private int _CurrentGridX = -1;

		// Token: 0x04003682 RID: 13954
		private int _CurrentGridY = -1;

		// Token: 0x04003683 RID: 13955
		private Dictionary<string, bool> _CurrentObjsDict = null;

		// Token: 0x04003684 RID: 13956
		private Dictionary<string, bool> _CurrentGridsDict = null;

		// Token: 0x04003685 RID: 13957
		public bool _HandledDead = false;

		// Token: 0x04003686 RID: 13958
		private long _FakeRoleDeadTicks = 0L;

		// Token: 0x04003687 RID: 13959
		private long _LastLogAttackerTicks = 0L;

		// Token: 0x04003688 RID: 13960
		private Dictionary<int, int> _AttackerDict = new Dictionary<int, int>();

		// Token: 0x04003689 RID: 13961
		private Dictionary<int, long> _AttackerTicksDict = new Dictionary<int, long>();
	}
}
