using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000739 RID: 1849
	public class JunQiItem : IObject
	{
		// Token: 0x06002E0A RID: 11786 RVA: 0x002865C8 File Offset: 0x002847C8
		public JunQiData GetJunQiData()
		{
			return this._MyJunQiData;
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06002E0B RID: 11787 RVA: 0x002865E0 File Offset: 0x002847E0
		// (set) Token: 0x06002E0C RID: 11788 RVA: 0x00286634 File Offset: 0x00284834
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

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06002E0D RID: 11789 RVA: 0x00286688 File Offset: 0x00284888
		// (set) Token: 0x06002E0E RID: 11790 RVA: 0x002866DC File Offset: 0x002848DC
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

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06002E0F RID: 11791 RVA: 0x00286730 File Offset: 0x00284930
		// (set) Token: 0x06002E10 RID: 11792 RVA: 0x00286784 File Offset: 0x00284984
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

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06002E11 RID: 11793 RVA: 0x002867D8 File Offset: 0x002849D8
		// (set) Token: 0x06002E12 RID: 11794 RVA: 0x0028682C File Offset: 0x00284A2C
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

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06002E13 RID: 11795 RVA: 0x00286880 File Offset: 0x00284A80
		// (set) Token: 0x06002E14 RID: 11796 RVA: 0x002868D4 File Offset: 0x00284AD4
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

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06002E15 RID: 11797 RVA: 0x00286928 File Offset: 0x00284B28
		// (set) Token: 0x06002E16 RID: 11798 RVA: 0x0028697C File Offset: 0x00284B7C
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

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06002E17 RID: 11799 RVA: 0x002869D0 File Offset: 0x00284BD0
		// (set) Token: 0x06002E18 RID: 11800 RVA: 0x00286A24 File Offset: 0x00284C24
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

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06002E19 RID: 11801 RVA: 0x00286A78 File Offset: 0x00284C78
		// (set) Token: 0x06002E1A RID: 11802 RVA: 0x00286ACC File Offset: 0x00284CCC
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

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06002E1B RID: 11803 RVA: 0x00286B20 File Offset: 0x00284D20
		// (set) Token: 0x06002E1C RID: 11804 RVA: 0x00286B74 File Offset: 0x00284D74
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

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06002E1D RID: 11805 RVA: 0x00286BC8 File Offset: 0x00284DC8
		// (set) Token: 0x06002E1E RID: 11806 RVA: 0x00286C1C File Offset: 0x00284E1C
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

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06002E1F RID: 11807 RVA: 0x00286C70 File Offset: 0x00284E70
		// (set) Token: 0x06002E20 RID: 11808 RVA: 0x00286CC4 File Offset: 0x00284EC4
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

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06002E21 RID: 11809 RVA: 0x00286D18 File Offset: 0x00284F18
		// (set) Token: 0x06002E22 RID: 11810 RVA: 0x00286D6C File Offset: 0x00284F6C
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

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06002E23 RID: 11811 RVA: 0x00286DC0 File Offset: 0x00284FC0
		// (set) Token: 0x06002E24 RID: 11812 RVA: 0x00286E14 File Offset: 0x00285014
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

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06002E25 RID: 11813 RVA: 0x00286E68 File Offset: 0x00285068
		// (set) Token: 0x06002E26 RID: 11814 RVA: 0x00286EBC File Offset: 0x002850BC
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

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06002E27 RID: 11815 RVA: 0x00286F10 File Offset: 0x00285110
		// (set) Token: 0x06002E28 RID: 11816 RVA: 0x00286F64 File Offset: 0x00285164
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

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06002E29 RID: 11817 RVA: 0x00286FB8 File Offset: 0x002851B8
		// (set) Token: 0x06002E2A RID: 11818 RVA: 0x0028700C File Offset: 0x0028520C
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

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06002E2B RID: 11819 RVA: 0x00287060 File Offset: 0x00285260
		// (set) Token: 0x06002E2C RID: 11820 RVA: 0x002870A8 File Offset: 0x002852A8
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

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06002E2D RID: 11821 RVA: 0x002870F0 File Offset: 0x002852F0
		// (set) Token: 0x06002E2E RID: 11822 RVA: 0x0028713C File Offset: 0x0028533C
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

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06002E2F RID: 11823 RVA: 0x00287188 File Offset: 0x00285388
		// (set) Token: 0x06002E30 RID: 11824 RVA: 0x002871D0 File Offset: 0x002853D0
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

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06002E31 RID: 11825 RVA: 0x00287258 File Offset: 0x00285458
		// (set) Token: 0x06002E32 RID: 11826 RVA: 0x002872A0 File Offset: 0x002854A0
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

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06002E33 RID: 11827 RVA: 0x002872E8 File Offset: 0x002854E8
		// (set) Token: 0x06002E34 RID: 11828 RVA: 0x00287330 File Offset: 0x00285530
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

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06002E35 RID: 11829 RVA: 0x00287378 File Offset: 0x00285578
		// (set) Token: 0x06002E36 RID: 11830 RVA: 0x002873C0 File Offset: 0x002855C0
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

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06002E37 RID: 11831 RVA: 0x00287408 File Offset: 0x00285608
		// (set) Token: 0x06002E38 RID: 11832 RVA: 0x00287450 File Offset: 0x00285650
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

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06002E39 RID: 11833 RVA: 0x00287498 File Offset: 0x00285698
		// (set) Token: 0x06002E3A RID: 11834 RVA: 0x002874E0 File Offset: 0x002856E0
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

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06002E3B RID: 11835 RVA: 0x00287528 File Offset: 0x00285728
		// (set) Token: 0x06002E3C RID: 11836 RVA: 0x00287574 File Offset: 0x00285774
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

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06002E3D RID: 11837 RVA: 0x002875C0 File Offset: 0x002857C0
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_JUNQI;
			}
		}

		// Token: 0x06002E3E RID: 11838 RVA: 0x002875D4 File Offset: 0x002857D4
		public int GetObjectID()
		{
			return this.JunQiID;
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06002E3F RID: 11839 RVA: 0x002875EC File Offset: 0x002857EC
		// (set) Token: 0x06002E40 RID: 11840 RVA: 0x00287603 File Offset: 0x00285803
		public long LastLifeMagicTick { get; set; }

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06002E41 RID: 11841 RVA: 0x0028760C File Offset: 0x0028580C
		// (set) Token: 0x06002E42 RID: 11842 RVA: 0x00287658 File Offset: 0x00285858
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

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06002E43 RID: 11843 RVA: 0x002876C0 File Offset: 0x002858C0
		// (set) Token: 0x06002E44 RID: 11844 RVA: 0x002876E5 File Offset: 0x002858E5
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

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06002E45 RID: 11845 RVA: 0x00287708 File Offset: 0x00285908
		public int CurrentMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06002E46 RID: 11846 RVA: 0x00287720 File Offset: 0x00285920
		public int CurrentCopyMapID
		{
			get
			{
				return this.CopyMapID;
			}
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06002E47 RID: 11847 RVA: 0x00287738 File Offset: 0x00285938
		// (set) Token: 0x06002E48 RID: 11848 RVA: 0x00287750 File Offset: 0x00285950
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

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06002E49 RID: 11849 RVA: 0x0028775C File Offset: 0x0028595C
		// (set) Token: 0x06002E4A RID: 11850 RVA: 0x00287773 File Offset: 0x00285973
		public List<int> PassiveEffectList { get; set; }

		// Token: 0x06002E4B RID: 11851 RVA: 0x0028777C File Offset: 0x0028597C
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

		// Token: 0x06002E4C RID: 11852 RVA: 0x002877F4 File Offset: 0x002859F4
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

		// Token: 0x06002E4D RID: 11853 RVA: 0x002878B0 File Offset: 0x00285AB0
		public void RemoveAttacker(int roleID)
		{
			lock (this._AttackerLogDict)
			{
				this._AttackerLogDict.Remove(roleID);
			}
		}

		// Token: 0x06002E4E RID: 11854 RVA: 0x00287904 File Offset: 0x00285B04
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

		// Token: 0x06002E4F RID: 11855 RVA: 0x00287970 File Offset: 0x00285B70
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

		// Token: 0x06002E50 RID: 11856 RVA: 0x00287A80 File Offset: 0x00285C80
		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			return default(T);
		}

		// Token: 0x04003C22 RID: 15394
		private object Mutex = new object();

		// Token: 0x04003C23 RID: 15395
		private JunQiData _MyJunQiData = new JunQiData();

		// Token: 0x04003C24 RID: 15396
		public SceneUIClasses ManagerType = SceneUIClasses.Normal;

		// Token: 0x04003C25 RID: 15397
		private int _CopyMapID = -1;

		// Token: 0x04003C26 RID: 15398
		private int _CurrentLifeV;

		// Token: 0x04003C27 RID: 15399
		private int _AttackedRoleID;

		// Token: 0x04003C28 RID: 15400
		private long _LastAttackedTick = 0L;

		// Token: 0x04003C29 RID: 15401
		private int _CurrentGridX = -1;

		// Token: 0x04003C2A RID: 15402
		private int _CurrentGridY = -1;

		// Token: 0x04003C2B RID: 15403
		private Dictionary<string, bool> _CurrentObjsDict = null;

		// Token: 0x04003C2C RID: 15404
		private Dictionary<string, bool> _CurrentGridsDict = null;

		// Token: 0x04003C2D RID: 15405
		public bool _HandledDead = false;

		// Token: 0x04003C2E RID: 15406
		private long _JunQiDeadTicks = 0L;

		// Token: 0x04003C2F RID: 15407
		private long _LastLogAttackerTicks = 0L;

		// Token: 0x04003C30 RID: 15408
		private Dictionary<int, JunQiAttackerLog> _AttackerLogDict = new Dictionary<int, JunQiAttackerLog>();
	}
}
