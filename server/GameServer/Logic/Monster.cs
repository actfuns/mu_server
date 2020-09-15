using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.BossAI;
using GameServer.Logic.ExtensionProps;
using GameServer.Logic.NewBufferExt;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000735 RID: 1845
	public class Monster : IObject
	{
		// Token: 0x06002D09 RID: 11529 RVA: 0x00282A08 File Offset: 0x00280C08
		public Monster Clone()
		{
			Monster monster = new Monster();
			monster.Name = this.Name;
			monster.MonsterZoneNode = this.MonsterZoneNode;
			monster.MonsterInfo = this.MonsterInfo;
			if (null == monster.MonsterInfo)
			{
				monster.MonsterInfo = this.MonsterZoneNode.GetMonsterInfo();
			}
			if (monster.MonsterInfo != null && monster.MonsterInfo.ExtProps != null)
			{
				Array.Copy(monster.MonsterInfo.ExtProps, monster.DynamicData.ExtProps, 177);
			}
			monster.Camp = monster.MonsterInfo.Camp;
			monster.RoleID = this.RoleID;
			monster.VLife = this.VLife;
			monster.VMana = this.VMana;
			monster.AttackRange = this.AttackRange;
			monster.Direction = this.Direction;
			monster.MoveSpeed = this.MoveSpeed;
			monster.MonsterType = this.MonsterType;
			monster.NextSeekEnemyTicks = this.NextSeekEnemyTicks;
			monster.OwnerClient = this.OwnerClient;
			monster.OwnerMonster = this.OwnerMonster;
			monster.CurrentMagicLevel = this.CurrentMagicLevel;
			monster.SurvivalTime = this.SurvivalTime;
			monster.SurvivalTick = this.SurvivalTick;
			Monster.IncMonsterCount();
			return monster;
		}

		// Token: 0x06002D0A RID: 11530 RVA: 0x00282B6C File Offset: 0x00280D6C
		public MonsterData GetMonsterData()
		{
			MonsterData _MonsterData = new MonsterData();
			_MonsterData.RoleID = this.RoleID;
			if (null != this.OwnerClient)
			{
				_MonsterData.RoleName = string.Format(GLang.GetLang(674, new object[0]), Global.FormatRoleName4(this.OwnerClient), this.MonsterInfo.VSName);
			}
			else if (!string.IsNullOrEmpty(this.MonsterName))
			{
				_MonsterData.RoleName = this.MonsterName;
			}
			else
			{
				_MonsterData.RoleName = this.MonsterInfo.VSName;
			}
			_MonsterData.ExtensionID = this.MonsterInfo.ExtensionID;
			_MonsterData.Level = this.MonsterInfo.VLevel;
			_MonsterData.Experience = this.MonsterInfo.VExperience;
			_MonsterData.MaxLifeV = this.MonsterInfo.VLifeMax;
			_MonsterData.MaxMagicV = this.MonsterInfo.VManaMax;
			_MonsterData.EquipmentBody = this.MonsterInfo.EquipmentBody;
			_MonsterData.MonsterType = this.MonsterType;
			_MonsterData.BattleWitchSide = this.Camp;
			CompResourcesConfig tagInfo = this.Tag as CompResourcesConfig;
			if (tagInfo != null && tagInfo.ResourceState == 1)
			{
				_MonsterData.BirthTicks = this.LastMonsterLivingTicks;
			}
			if (null != this.OwnerClient)
			{
				_MonsterData.MasterRoleID = this.OwnerClient.ClientData.RoleID;
			}
			_MonsterData.PosX = (int)this.SafeCoordinate.X;
			_MonsterData.PosY = (int)this.SafeCoordinate.Y;
			_MonsterData.RoleDirection = (int)this.SafeDirection;
			_MonsterData.LifeV = this.VLife;
			_MonsterData.MagicV = this.VMana;
			_MonsterData.AiControlType = (ushort)this.PetAiControlType;
			_MonsterData.MonsterLevel = this.MonsterInfo.VLevel;
			BufferData bufferData = Global.GetMonsterBufferDataByID(this, 42);
			if (null != bufferData)
			{
				_MonsterData.ZhongDuStart = bufferData.StartTime;
				_MonsterData.ZhongDuSeconds = bufferData.BufferSecs;
			}
			else
			{
				_MonsterData.ZhongDuStart = 0L;
				_MonsterData.ZhongDuSeconds = 0;
			}
			if (this.IsMonsterDongJie())
			{
				_MonsterData.FaintStart = this.DongJieStart;
				_MonsterData.FaintSeconds = this.DongJieSeconds;
			}
			return _MonsterData;
		}

		// Token: 0x06002D0B RID: 11531 RVA: 0x00282DB8 File Offset: 0x00280FB8
		public Point Realive()
		{
			this.UniqueID = Global.GetUniqueID();
			if (401 == this.MonsterType)
			{
				CompManager.getInstance().OnProcessBossRealive(this);
			}
			this.TimedActionMgr.RemoveItem(0);
			this._LastDeadTicks = 0L;
			this.HandledDead = false;
			this.VLife = this.MonsterInfo.VLifeMax;
			this.VMana = this.MonsterInfo.VManaMax;
			this.Action = GActions.Stand;
			this.DongJieStart = 0L;
			this.DongJieSeconds = 0;
			this.TempPropsBuffer.Init();
			this.WhoKillMeID = 0;
			this.WhoKillMeName = "";
			this.IsCollected = false;
			this.isDeath = false;
			this.deathDelay = 0;
			lock (this._AttackerLogDict)
			{
				this._AttackerLogDict.Clear();
			}
			this.Start();
			Point toPoint;
			if (501 == this.MonsterType || 501 == this.MonsterType)
			{
				toPoint = Global.GetRandomPoint(ObjectTypes.OT_MONSTER, this.MonsterZoneNode.MapCode);
			}
			else
			{
				toPoint = Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MonsterZoneNode.MapCode, this.MonsterZoneNode.ToX, this.MonsterZoneNode.ToY, this.MonsterZoneNode.Radius, 0, true);
			}
			this.Coordinate = toPoint;
			this.Direction = (double)Global.GetRandomNumber(0, 8);
			GameManager.SystemServerEvents.AddEvent(string.Format("怪物复活, roleID={0}", this.RoleID), EventLevels.Debug);
			return toPoint;
		}

		// Token: 0x06002D0C RID: 11532 RVA: 0x00282F7C File Offset: 0x0028117C
		public void OnDead()
		{
			this.MyMagicsManyTimeDmageQueue.Clear();
			this._CurrentMagic = -1;
			this._MagicFinish = 0;
			this.ClearDynSkill();
			Global.RemoveMonsterBufferData(this, 42);
			this.DestPoint = new Point(-1.0, -1.0);
			Global.RemoveStoryboard(this.Name);
			GameManager.MapGridMgr.DictGrids[this.MonsterZoneNode.MapCode].RemoveObject(this);
			this.Action = GActions.Death;
			this._LastDeadTicks = TimeUtil.NOW() * 10000L;
			this.ClearBossAI();
			this.Alive = false;
			this.OnReallyDied();
		}

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06002D0D RID: 11533 RVA: 0x00283030 File Offset: 0x00281230
		// (set) Token: 0x06002D0E RID: 11534 RVA: 0x00283047 File Offset: 0x00281247
		public MonsterZone MonsterZoneNode { get; set; }

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06002D0F RID: 11535 RVA: 0x00283050 File Offset: 0x00281250
		// (set) Token: 0x06002D10 RID: 11536 RVA: 0x00283067 File Offset: 0x00281267
		public MonsterStaticInfo MonsterInfo { get; set; }

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06002D11 RID: 11537 RVA: 0x00283070 File Offset: 0x00281270
		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_MONSTER;
			}
		}

		// Token: 0x06002D12 RID: 11538 RVA: 0x00283084 File Offset: 0x00281284
		public int GetObjectID()
		{
			return this.RoleID;
		}

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06002D13 RID: 11539 RVA: 0x0028309C File Offset: 0x0028129C
		// (set) Token: 0x06002D14 RID: 11540 RVA: 0x002830B3 File Offset: 0x002812B3
		public long LastLifeMagicTick { get; set; }

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06002D15 RID: 11541 RVA: 0x002830BC File Offset: 0x002812BC
		// (set) Token: 0x06002D16 RID: 11542 RVA: 0x00283120 File Offset: 0x00281320
		public Point CurrentGrid
		{
			get
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MonsterZoneNode.MapCode];
				return new Point((double)((int)(this.Coordinate.X / (double)gameMap.MapGridWidth)), (double)((int)(this.Coordinate.Y / (double)gameMap.MapGridHeight)));
			}
			set
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MonsterZoneNode.MapCode];
				this.Coordinate = new Point(value.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), value.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
			}
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06002D17 RID: 11543 RVA: 0x00283188 File Offset: 0x00281388
		// (set) Token: 0x06002D18 RID: 11544 RVA: 0x002831A0 File Offset: 0x002813A0
		public Point CurrentPos
		{
			get
			{
				return this.Coordinate;
			}
			set
			{
				this.Coordinate = value;
			}
		}

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06002D19 RID: 11545 RVA: 0x002831AC File Offset: 0x002813AC
		public int CurrentMapCode
		{
			get
			{
				return this.MonsterZoneNode.MapCode;
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06002D1A RID: 11546 RVA: 0x002831CC File Offset: 0x002813CC
		// (set) Token: 0x06002D1B RID: 11547 RVA: 0x002831E4 File Offset: 0x002813E4
		public int CurrentCopyMapID
		{
			get
			{
				return this.CopyMapID;
			}
			set
			{
				this.CopyMapID = value;
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06002D1C RID: 11548 RVA: 0x002831F0 File Offset: 0x002813F0
		// (set) Token: 0x06002D1D RID: 11549 RVA: 0x00283209 File Offset: 0x00281409
		public Dircetions CurrentDir
		{
			get
			{
				return (Dircetions)this.Direction;
			}
			set
			{
				this.Direction = (double)value;
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06002D1E RID: 11550 RVA: 0x00283218 File Offset: 0x00281418
		// (set) Token: 0x06002D1F RID: 11551 RVA: 0x0028322F File Offset: 0x0028142F
		public List<int> PassiveEffectList
		{
			get
			{
				return new List<int>();
			}
			set
			{
			}
		}

		// Token: 0x06002D20 RID: 11552 RVA: 0x00283234 File Offset: 0x00281434
		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			T result;
			if (type != ExtComponentTypes.ManyTimeDamageQueue)
			{
				result = default(T);
			}
			else
			{
				result = (this.MyMagicsManyTimeDmageQueue as T);
			}
			return result;
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06002D21 RID: 11553 RVA: 0x0028326C File Offset: 0x0028146C
		// (set) Token: 0x06002D22 RID: 11554 RVA: 0x002832B4 File Offset: 0x002814B4
		public List<VisibleItem> VisibleItemList
		{
			get
			{
				List<VisibleItem> visibleItemList;
				lock (this)
				{
					visibleItemList = this._VisibleItemList;
				}
				return visibleItemList;
			}
			set
			{
				lock (this)
				{
					this._VisibleItemList = value;
				}
			}
		}

		// Token: 0x06002D23 RID: 11555 RVA: 0x002832FC File Offset: 0x002814FC
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

		// Token: 0x06002D24 RID: 11556 RVA: 0x00283374 File Offset: 0x00281574
		public void AddAttacker(GameClient client, int injured, Monster DSPet = null)
		{
			if (client != null)
			{
				int roleID = client.ClientData.RoleID;
				lock (this._AttackerLogDict)
				{
					this._LastLogAttackerTicks = TimeUtil.NOW();
					MonsterAttackerLog attacker = null;
					if (!this._AttackerLogDict.TryGetValue(roleID, out attacker))
					{
						attacker = new MonsterAttackerLog();
						attacker.RoleId = roleID;
						attacker.RoleName = client.ClientData.RoleName;
						attacker.Occupation = Global.CalcOriginalOccupationID(client);
						attacker.FirstAttackMs = this._LastLogAttackerTicks;
						attacker.FirstAttack_MaxAttckV = RoleAlgorithm.GetMaxAttackV(client);
						attacker.FirstAttack_MaxMAttackV = RoleAlgorithm.GetMaxMagicAttackV(client);
						attacker.ZhanLi = (long)client.ClientData.CombatForce;
						attacker.VipExp = client.ClientData.VipExp;
						this._AttackerLogDict[roleID] = attacker;
					}
					attacker.MaxAttackV = Math.Max(attacker.MaxAttackV, RoleAlgorithm.GetMaxAttackV(client));
					attacker.MaxMAttackV = Math.Max(attacker.MaxMAttackV, RoleAlgorithm.GetMaxMagicAttackV(client));
					attacker.LastAttackMs = this._LastLogAttackerTicks;
					if (null == DSPet)
					{
						attacker.TotalInjured += (long)injured;
						attacker.InjureTimes++;
					}
					else
					{
						attacker.TotalInjuredByPet += (long)injured;
						attacker.InjureTimesByPet += 1L;
					}
				}
			}
		}

		// Token: 0x06002D25 RID: 11557 RVA: 0x0028350C File Offset: 0x0028170C
		public void RemoveAttacker(int roleID)
		{
			lock (this._AttackerLogDict)
			{
				this._AttackerLogDict.Remove(roleID);
			}
		}

		// Token: 0x06002D26 RID: 11558 RVA: 0x00283560 File Offset: 0x00281760
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

		// Token: 0x06002D27 RID: 11559 RVA: 0x002835CC File Offset: 0x002817CC
		public int GetAttackerFromList()
		{
			this.CheckAttackerListEfficiency();
			int attackerRid = -1;
			long maxInjured = 0L;
			long nowTicks = TimeUtil.NOW();
			lock (this._AttackerLogDict)
			{
				foreach (MonsterAttackerLog attacker in this._AttackerLogDict.Values)
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

		// Token: 0x06002D28 RID: 11560 RVA: 0x002836DC File Offset: 0x002818DC
		public List<long> GetAttackerDamageList(List<int> ridList)
		{
			List<long> result = new List<long>();
			if (null != ridList)
			{
				lock (this._AttackerLogDict)
				{
					foreach (int rid in ridList)
					{
						long damage = 0L;
						MonsterAttackerLog attacker;
						if (this._AttackerLogDict.TryGetValue(rid, out attacker))
						{
							damage = ((attacker.TotalInjured > attacker.TotalInjuredByPet) ? attacker.TotalInjured : attacker.TotalInjuredByPet);
						}
						result.Add(damage);
					}
				}
			}
			return result;
		}

		// Token: 0x06002D29 RID: 11561 RVA: 0x002837C8 File Offset: 0x002819C8
		public long GetAttackerDamage(int rid)
		{
			long damage = 0L;
			lock (this._AttackerLogDict)
			{
				MonsterAttackerLog attacker;
				if (this._AttackerLogDict.TryGetValue(rid, out attacker))
				{
					damage = ((attacker.TotalInjured > attacker.TotalInjuredByPet) ? attacker.TotalInjured : attacker.TotalInjuredByPet);
				}
			}
			return damage;
		}

		// Token: 0x06002D2A RID: 11562 RVA: 0x00283850 File Offset: 0x00281A50
		public List<int> GetAttackerList()
		{
			List<int> attackerList = new List<int>();
			long nowTicks = TimeUtil.NOW();
			lock (this._AttackerLogDict)
			{
				foreach (MonsterAttackerLog attacker in this._AttackerLogDict.Values)
				{
					if (nowTicks - attacker.LastAttackMs < 15000L)
					{
						attackerList.Add(attacker.RoleId);
					}
				}
			}
			return attackerList;
		}

		// Token: 0x06002D2B RID: 11563 RVA: 0x0028391C File Offset: 0x00281B1C
		public string BuildAttackerLog()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("怪物伤害日志: MonsterId={0}, MonsterName={1},", this.MonsterInfo.ExtensionID, Global.GetMonsterNameByID(this.MonsterInfo.ExtensionID));
			lock (this._AttackerLogDict)
			{
				sb.AppendFormat("共有{0}个攻击者: ", this._AttackerLogDict.Count<KeyValuePair<int, MonsterAttackerLog>>());
				sb.AppendLine();
				foreach (MonsterAttackerLog attacker in this._AttackerLogDict.Values)
				{
					sb.Append("     ");
					sb.AppendFormat("roleid={0}, ", attacker.RoleId);
					sb.AppendFormat("rolename={0}, ", attacker.RoleName);
					sb.AppendFormat("职业={0}, ", Global.GetOccupationStr(attacker.Occupation));
					sb.AppendFormat("总计伤害={0}, ", attacker.TotalInjured);
					sb.AppendFormat("总计伤害Pet={0},", attacker.TotalInjuredByPet);
					long totalAttackSec = attacker.LastAttackMs - attacker.FirstAttackMs;
					sb.AppendFormat("共用时={0}ms, ", totalAttackSec);
					sb.AppendFormat("伤害次数={0}, ", attacker.InjureTimes);
					sb.AppendFormat("伤害次数Pet={0},", attacker.InjureTimesByPet);
					sb.AppendFormat("首次伤害物攻={0}, ", attacker.FirstAttack_MaxAttckV);
					sb.AppendFormat("首次伤害魔攻={0}, ", attacker.FirstAttack_MaxMAttackV);
					sb.AppendFormat("最大物攻={0}, ", attacker.MaxAttackV);
					sb.AppendFormat("最大魔攻={0}, ", attacker.MaxMAttackV);
					sb.AppendFormat("ZhanLi={0}, ", attacker.ZhanLi);
					sb.AppendFormat("VipExp={0}, ", attacker.VipExp);
					double maxAttack = Math.Max(attacker.MaxMAttackV, attacker.MaxAttackV);
					if (maxAttack > 0.0)
					{
						sb.AppendFormat("攻击系数={0}, ", (double)attacker.TotalInjured * 1.0 / (double)attacker.InjureTimes / maxAttack);
					}
					else
					{
						sb.AppendFormat("攻击系数={0}[最大伤害无效]", "无效");
					}
					if (totalAttackSec > 0L)
					{
						sb.AppendFormat("攻速系数={0}", (double)attacker.InjureTimes * 1.0 / (double)totalAttackSec);
					}
					else
					{
						sb.AppendFormat("攻速系数={0}[总攻击时间无效]", "无效");
					}
					sb.AppendLine();
				}
			}
			return sb.ToString();
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06002D2C RID: 11564 RVA: 0x00283C38 File Offset: 0x00281E38
		// (set) Token: 0x06002D2D RID: 11565 RVA: 0x00283C4F File Offset: 0x00281E4F
		public long LastInObsJugeTicks { get; set; }

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06002D2E RID: 11566 RVA: 0x00283C58 File Offset: 0x00281E58
		// (set) Token: 0x06002D2F RID: 11567 RVA: 0x00283C6F File Offset: 0x00281E6F
		public long LastSeekEnemyTicks { get; set; }

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06002D30 RID: 11568 RVA: 0x00283C78 File Offset: 0x00281E78
		// (set) Token: 0x06002D31 RID: 11569 RVA: 0x00283C8F File Offset: 0x00281E8F
		public long NextSeekEnemyTicks { get; set; }

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06002D32 RID: 11570 RVA: 0x00283C98 File Offset: 0x00281E98
		// (set) Token: 0x06002D33 RID: 11571 RVA: 0x00283CAF File Offset: 0x00281EAF
		public long LastLockEnemyTicks { get; set; }

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06002D34 RID: 11572 RVA: 0x00283CB8 File Offset: 0x00281EB8
		// (set) Token: 0x06002D35 RID: 11573 RVA: 0x00283CCF File Offset: 0x00281ECF
		public long LockFocusTime { get; set; }

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06002D36 RID: 11574 RVA: 0x00283CD8 File Offset: 0x00281ED8
		// (set) Token: 0x06002D37 RID: 11575 RVA: 0x00283CF0 File Offset: 0x00281EF0
		public double MoveSpeed
		{
			get
			{
				return this._MoveSpeed;
			}
			set
			{
				this._MoveSpeed = value;
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06002D38 RID: 11576 RVA: 0x00283CFC File Offset: 0x00281EFC
		// (set) Token: 0x06002D39 RID: 11577 RVA: 0x00283D44 File Offset: 0x00281F44
		public Point DestPoint
		{
			get
			{
				Point destPoint;
				lock (this)
				{
					destPoint = this._DestPoint;
				}
				return destPoint;
			}
			set
			{
				lock (this)
				{
					this._DestPoint = value;
				}
			}
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06002D3A RID: 11578 RVA: 0x00283D8C File Offset: 0x00281F8C
		// (set) Token: 0x06002D3B RID: 11579 RVA: 0x00283DA4 File Offset: 0x00281FA4
		public int CopyMapID
		{
			get
			{
				return this._CopyMapID;
			}
			set
			{
				this._CopyMapID = value;
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06002D3C RID: 11580 RVA: 0x00283DB0 File Offset: 0x00281FB0
		// (set) Token: 0x06002D3D RID: 11581 RVA: 0x00283DC8 File Offset: 0x00281FC8
		public bool HandledDead
		{
			get
			{
				return this._HandledDead;
			}
			set
			{
				this._HandledDead = value;
			}
		}

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06002D3E RID: 11582 RVA: 0x00283DD4 File Offset: 0x00281FD4
		// (set) Token: 0x06002D3F RID: 11583 RVA: 0x00283DEB File Offset: 0x00281FEB
		public string Name { get; set; }

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06002D40 RID: 11584 RVA: 0x00283DF4 File Offset: 0x00281FF4
		// (set) Token: 0x06002D41 RID: 11585 RVA: 0x00283E0B File Offset: 0x0028200B
		public int RoleID { get; set; }

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06002D42 RID: 11586 RVA: 0x00283E14 File Offset: 0x00282014
		// (set) Token: 0x06002D43 RID: 11587 RVA: 0x00283E31 File Offset: 0x00282031
		public double VLife
		{
			get
			{
				return Thread.VolatileRead(ref this._VLife);
			}
			set
			{
				Thread.VolatileWrite(ref this._VLife, value);
			}
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06002D44 RID: 11588 RVA: 0x00283E44 File Offset: 0x00282044
		// (set) Token: 0x06002D45 RID: 11589 RVA: 0x00283E61 File Offset: 0x00282061
		public double VMana
		{
			get
			{
				return Thread.VolatileRead(ref this._VMana);
			}
			set
			{
				Thread.VolatileWrite(ref this._VMana, value);
			}
		}

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06002D46 RID: 11590 RVA: 0x00283E74 File Offset: 0x00282074
		// (set) Token: 0x06002D47 RID: 11591 RVA: 0x00283E8B File Offset: 0x0028208B
		public int MonsterType { get; set; }

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06002D48 RID: 11592 RVA: 0x00283E94 File Offset: 0x00282094
		// (set) Token: 0x06002D49 RID: 11593 RVA: 0x00283EAB File Offset: 0x002820AB
		public int WhoKillMeID { get; set; }

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06002D4A RID: 11594 RVA: 0x00283EB4 File Offset: 0x002820B4
		// (set) Token: 0x06002D4B RID: 11595 RVA: 0x00283ECB File Offset: 0x002820CB
		public string WhoKillMeName { get; set; }

		// Token: 0x06002D4C RID: 11596 RVA: 0x00283ED4 File Offset: 0x002820D4
		public void AddLife(long life)
		{
			if (this._VLife > 0.0 && this.Alive)
			{
				this._VLife = Math.Min(this.MonsterInfo.VLifeMax, this._VLife + (double)life);
			}
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06002D4D RID: 11597 RVA: 0x00283F28 File Offset: 0x00282128
		// (set) Token: 0x06002D4E RID: 11598 RVA: 0x00283F40 File Offset: 0x00282140
		public int PetAiControlType
		{
			get
			{
				return this._PetAiControlType;
			}
			set
			{
				int oldType = this._PetAiControlType;
				this._PetAiControlType = value;
				if (oldType != this._PetAiControlType)
				{
					this._LockObject = -1;
				}
			}
		}

		// Token: 0x06002D4F RID: 11599 RVA: 0x00283F74 File Offset: 0x00282174
		public bool IsMonsterDongJie()
		{
			bool result;
			if (this.DongJieStart <= 0L)
			{
				result = false;
			}
			else
			{
				long ticks = TimeUtil.NOW();
				result = (ticks < this.DongJieStart + (long)(this.DongJieSeconds * 1000));
			}
			return result;
		}

		// Token: 0x06002D50 RID: 11600 RVA: 0x00283FC0 File Offset: 0x002821C0
		public bool IsMonsterXuanYun()
		{
			bool result;
			if (this.XuanYunStart <= 0L)
			{
				result = false;
			}
			else
			{
				long ticks = TimeUtil.NOW();
				result = (ticks < this.XuanYunStart + (long)(this.XuanYunSeconds * 1000));
			}
			return result;
		}

		// Token: 0x06002D51 RID: 11601 RVA: 0x0028400C File Offset: 0x0028220C
		public bool IsMonsterDingShen()
		{
			bool result;
			if (this.DingShenStart <= 0L)
			{
				result = false;
			}
			else
			{
				long ticks = TimeUtil.NOW();
				result = (ticks < this.DingShenStart + (long)(this.DingShenSeconds * 1000));
			}
			return result;
		}

		// Token: 0x06002D52 RID: 11602 RVA: 0x00284058 File Offset: 0x00282258
		public bool IsMonsterSpeedDown()
		{
			bool result;
			if (this.SpeedDownStart <= 0L)
			{
				result = false;
			}
			else
			{
				long ticks = TimeUtil.NOW();
				result = (ticks < this.SpeedDownStart + (long)(this.SpeedDownSeconds * 1000));
			}
			return result;
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06002D53 RID: 11603 RVA: 0x002840A4 File Offset: 0x002822A4
		// (remove) Token: 0x06002D54 RID: 11604 RVA: 0x002840E0 File Offset: 0x002822E0
		public event MoveToEventHandler MoveToComplete;

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06002D55 RID: 11605 RVA: 0x0028411C File Offset: 0x0028231C
		public long LastDeadTicks
		{
			get
			{
				return this._LastDeadTicks;
			}
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06002D56 RID: 11606 RVA: 0x00284134 File Offset: 0x00282334
		public bool IsMoving
		{
			get
			{
				bool result;
				if (GActions.Walk != this._Action)
				{
					result = false;
				}
				else
				{
					long nowTicks = TimeUtil.NOW();
					result = (nowTicks - this._LastActionTick < (long)this.GetMovingNeedTick());
				}
				return result;
			}
		}

		// Token: 0x06002D57 RID: 11607 RVA: 0x00284170 File Offset: 0x00282370
		public int GetMovingNeedTick()
		{
			double moveSpeed = this.MoveSpeed;
			if (moveSpeed < 0.05)
			{
				moveSpeed = 0.05;
			}
			return this.IsMonsterSpeedDown() ? Convert.ToInt32((double)Global.MovingNeedTicksPerGrid / moveSpeed) : Global.MovingNeedTicksPerGrid;
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06002D58 RID: 11608 RVA: 0x002841C4 File Offset: 0x002823C4
		// (remove) Token: 0x06002D59 RID: 11609 RVA: 0x00284200 File Offset: 0x00282400
		public event CoordinateEventHandler CoordinateChanged;

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06002D5A RID: 11610 RVA: 0x0028423C File Offset: 0x0028243C
		public Point SafeCoordinate
		{
			get
			{
				Point safeCoordinate;
				lock (this)
				{
					safeCoordinate = this._SafeCoordinate;
				}
				return safeCoordinate;
			}
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06002D5B RID: 11611 RVA: 0x00284284 File Offset: 0x00282484
		public Point SafeOldCoordinate
		{
			get
			{
				Point safeOldCoordinate;
				lock (this)
				{
					safeOldCoordinate = this._SafeOldCoordinate;
				}
				return safeOldCoordinate;
			}
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06002D5C RID: 11612 RVA: 0x002842CC File Offset: 0x002824CC
		// (set) Token: 0x06002D5D RID: 11613 RVA: 0x002842E4 File Offset: 0x002824E4
		public bool FirstStoryMove
		{
			get
			{
				return this._FirstStoryMove;
			}
			set
			{
				this._FirstStoryMove = value;
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06002D5E RID: 11614 RVA: 0x002842F0 File Offset: 0x002824F0
		// (set) Token: 0x06002D5F RID: 11615 RVA: 0x00284307 File Offset: 0x00282507
		public Point FirstCoordinate { get; set; }

		// Token: 0x06002D60 RID: 11616 RVA: 0x00284310 File Offset: 0x00282510
		public Point getFirstGrid()
		{
			if (default(Point).Equals(this.firstGrid))
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.MonsterZoneNode.MapCode];
				this.firstGrid = new Point((double)((int)(this.FirstCoordinate.X / (double)gameMap.MapGridWidth)), (double)((int)(this.FirstCoordinate.Y / (double)gameMap.MapGridHeight)));
			}
			return this.firstGrid;
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06002D61 RID: 11617 RVA: 0x002843A0 File Offset: 0x002825A0
		// (set) Token: 0x06002D62 RID: 11618 RVA: 0x002843B8 File Offset: 0x002825B8
		public Point Coordinate
		{
			get
			{
				return this._Coordinate;
			}
			set
			{
				lock (this)
				{
					this._SafeOldCoordinate = value;
				}
				Point oldCoordinate = this._Coordinate;
				this._Coordinate = new Point(value.X, value.Y);
				Monster.ChangeCoordinateProperty2(this, oldCoordinate, this._Coordinate);
			}
		}

		// Token: 0x06002D63 RID: 11619 RVA: 0x00284430 File Offset: 0x00282630
		private static void ChangeCoordinateProperty2(Monster obj, Point oldValue, Point newValue)
		{
			lock (obj)
			{
				obj._SafeOldCoordinate = oldValue;
				obj._SafeCoordinate = newValue;
			}
			if (obj.CoordinateChanged != null)
			{
				obj.CoordinateChanged(obj);
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06002D64 RID: 11620 RVA: 0x002844A0 File Offset: 0x002826A0
		public double SafeDirection
		{
			get
			{
				return Thread.VolatileRead(ref this._SafeDirection);
			}
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06002D65 RID: 11621 RVA: 0x002844C0 File Offset: 0x002826C0
		// (set) Token: 0x06002D66 RID: 11622 RVA: 0x002844D8 File Offset: 0x002826D8
		public double Direction
		{
			get
			{
				return this._Direction;
			}
			set
			{
				lock (this)
				{
					this._SafeDirection = value;
				}
				double oldDirection = this._Direction;
				this._Direction = value;
				if (this.Action == GActions.Attack || this.Action == GActions.Magic || this.Action == GActions.Bow)
				{
					if (this.FrameCounter < this.EndFrame)
					{
						return;
					}
				}
				Monster.ChangeDirectionProperty2(this, oldDirection, this._Direction);
			}
		}

		// Token: 0x06002D67 RID: 11623 RVA: 0x0028457C File Offset: 0x0028277C
		private static void ChangeDirectionProperty2(Monster obj, double oldValue, double newValue)
		{
			lock (obj)
			{
				obj._SafeDirection = newValue;
			}
			obj.ChangeAction(false);
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06002D68 RID: 11624 RVA: 0x002845CC File Offset: 0x002827CC
		// (set) Token: 0x06002D69 RID: 11625 RVA: 0x002845E3 File Offset: 0x002827E3
		public int PetLockObjectPriority { get; set; }

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06002D6A RID: 11626 RVA: 0x002845EC File Offset: 0x002827EC
		// (set) Token: 0x06002D6B RID: 11627 RVA: 0x00284634 File Offset: 0x00282834
		public int LockObject
		{
			get
			{
				int lockObject;
				lock (this)
				{
					lockObject = this._LockObject;
				}
				return lockObject;
			}
			set
			{
				lock (this)
				{
					this._LockObject = value;
				}
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06002D6C RID: 11628 RVA: 0x0028467C File Offset: 0x0028287C
		// (set) Token: 0x06002D6D RID: 11629 RVA: 0x00284693 File Offset: 0x00282893
		public int AttackRange { get; set; }

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06002D6E RID: 11630 RVA: 0x0028469C File Offset: 0x0028289C
		public long LastActionTick
		{
			get
			{
				return this._LastActionTick;
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06002D6F RID: 11631 RVA: 0x002846B4 File Offset: 0x002828B4
		public long LastAttackActionTick
		{
			get
			{
				return this._LastAttackActionTick;
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06002D70 RID: 11632 RVA: 0x002846CC File Offset: 0x002828CC
		// (set) Token: 0x06002D71 RID: 11633 RVA: 0x002846E4 File Offset: 0x002828E4
		public GActions Action
		{
			get
			{
				return this._Action;
			}
			set
			{
				if (this._Action != value)
				{
					if (value == GActions.Attack || value == GActions.Magic || value == GActions.Bow)
					{
						if (this._Action == GActions.PreAttack)
						{
							return;
						}
					}
					this._Action = value;
					if (this._Action == GActions.Run)
					{
						this._Action = GActions.Walk;
					}
					lock (this)
					{
						this._SafeAction = this._Action;
					}
					this._LastActionTick = TimeUtil.NOW();
					if (value == GActions.Attack || value == GActions.Magic || value == GActions.Bow)
					{
						if (this.FrameCounter < this.EndFrame)
						{
							return;
						}
					}
					if (this.CurrentMagic <= 0)
					{
						this.ChangeAction(true);
					}
				}
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06002D72 RID: 11634 RVA: 0x002847EC File Offset: 0x002829EC
		public GActions SafeAction
		{
			get
			{
				GActions safeAction;
				lock (this)
				{
					safeAction = this._SafeAction;
				}
				return safeAction;
			}
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06002D73 RID: 11635 RVA: 0x00284834 File Offset: 0x00282A34
		// (set) Token: 0x06002D74 RID: 11636 RVA: 0x0028484B File Offset: 0x00282A4B
		public Point Destination { get; set; }

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06002D75 RID: 11637 RVA: 0x00284854 File Offset: 0x00282A54
		// (set) Token: 0x06002D76 RID: 11638 RVA: 0x0028486B File Offset: 0x00282A6B
		public Point EnemyTarget { get; set; }

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06002D77 RID: 11639 RVA: 0x00284874 File Offset: 0x00282A74
		// (set) Token: 0x06002D78 RID: 11640 RVA: 0x0028488B File Offset: 0x00282A8B
		public int FrameCounter { get; set; }

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06002D79 RID: 11641 RVA: 0x00284894 File Offset: 0x00282A94
		// (set) Token: 0x06002D7A RID: 11642 RVA: 0x002848AB File Offset: 0x00282AAB
		public int StartFrame { get; set; }

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06002D7B RID: 11643 RVA: 0x002848B4 File Offset: 0x00282AB4
		// (set) Token: 0x06002D7C RID: 11644 RVA: 0x002848CB File Offset: 0x00282ACB
		public int EndFrame { get; set; }

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06002D7D RID: 11645 RVA: 0x002848D4 File Offset: 0x00282AD4
		// (remove) Token: 0x06002D7E RID: 11646 RVA: 0x00284910 File Offset: 0x00282B10
		public event SpriteChangeActionEventHandler SpriteChangeAction;

		// Token: 0x06002D7F RID: 11647 RVA: 0x0028494C File Offset: 0x00282B4C
		private void BeginHeart()
		{
			this.LastMonsterLivingSlotTicks = TimeUtil.NOW();
			this.LastMonsterLivingTicks = this.LastMonsterLivingSlotTicks;
			GlobalEventSource.getInstance().fireEvent(new MonsterBirthOnEventObject(this));
		}

		// Token: 0x06002D80 RID: 11648 RVA: 0x00284977 File Offset: 0x00282B77
		public void Start()
		{
			this.Alive = true;
			this.BeginHeart();
		}

		// Token: 0x06002D81 RID: 11649 RVA: 0x00284988 File Offset: 0x00282B88
		public void onSurvivalTick()
		{
			if (0 != this.SurvivalTime)
			{
				if (TimeUtil.NOW() >= this.SurvivalTick)
				{
					if (1001 != this.MonsterType)
					{
						Global.SystemKillMonster(this);
					}
					else
					{
						GameManager.MonsterMgr.DeadMonsterImmediately(this);
					}
				}
			}
		}

		// Token: 0x06002D82 RID: 11650 RVA: 0x002849E8 File Offset: 0x00282BE8
		public virtual void Timer_Tick(object sender, EventArgs e)
		{
			if (this.isDeath)
			{
				this.deathDelay++;
				if (this.deathDelay >= 10)
				{
				}
			}
			else
			{
				if (this.IsMonsterDongJie())
				{
					this.Action = GActions.Stand;
				}
				long nowTicks = TimeUtil.NOW();
				if (nowTicks - this.LastMonsterLivingSlotTicks >= 60000L)
				{
					this.LastMonsterLivingSlotTicks = nowTicks;
					GlobalEventSource.getInstance().fireEvent(new MonsterLivingTimeEventObject(this));
				}
				if (GActions.Walk == this.Action || GActions.Run == this.Action)
				{
					if (nowTicks - this._LastActionTick >= (long)this.GetMovingNeedTick())
					{
						if (null != this.MoveToComplete)
						{
							this.MoveToComplete(this);
						}
					}
				}
				else
				{
					double newDirection = this.ChangeDirectionValue();
					int action = Global.GetActionIndex(this.Action);
					int frameNumber = this.MonsterInfo.EachActionFrameRange[action];
					if (GActions.PreAttack == this.Action)
					{
						action = Global.GetActionIndex(GActions.Stand);
						frameNumber = this.MonsterInfo.EachActionFrameRange[action];
					}
					int EffectiveFrameCounter;
					if (this.Action == GActions.Death)
					{
						EffectiveFrameCounter = (int)(newDirection * (double)frameNumber) + (frameNumber - 1);
					}
					else
					{
						EffectiveFrameCounter = (int)(newDirection * (double)frameNumber) + this.MonsterInfo.EffectiveFrame[action];
					}
					if (this.FrameCounter == EffectiveFrameCounter)
					{
						this.DoAction();
					}
					if (this.FrameCounter >= this.EndFrame && (this._Action == GActions.Attack || this._Action == GActions.Magic || this._Action == GActions.Bow))
					{
						SystemXmlItem systemMagic = null;
						lock (this.CurrentSkillIDIndexLock)
						{
							this._ToExecSkillID = -1;
							if (this._CurrentMagic > 0)
							{
								if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(this._CurrentMagic, out systemMagic))
								{
									int nextMagicID = systemMagic.GetIntValue("NextMagicID", -1);
									if (nextMagicID > 0)
									{
										this._ToExecSkillID = nextMagicID;
									}
								}
							}
							this._CurrentSkillIDIndex++;
						}
						this.OldAction = this._Action;
						this._Action = GActions.Stand;
						this._MaxAttackTimeSlot = 2000L;
						if (null == systemMagic)
						{
							GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(this._CurrentMagic, out systemMagic);
						}
						if (null != systemMagic)
						{
							int attackInterval = systemMagic.GetIntValue("AttackInterval", -1);
							if (attackInterval > 0)
							{
								this._MaxAttackTimeSlot = (long)attackInterval;
							}
						}
						this._LastAttackActionTick = TimeUtil.NOW();
						bool flag2 = false;
						try
						{
							Monster obj = this;
							Monitor.Enter(this, ref flag2);
							this._SafeAction = this._Action;
						}
						finally
						{
							if (flag2)
							{
								Monster obj;
								Monitor.Exit(obj);
							}
						}
						this.ChangeAction(false);
						this.FrameCounter = this.StartFrame;
					}
					else if (this.FrameCounter >= this.EndFrame && this._Action == GActions.PreAttack)
					{
						this._Action = this.OldAction;
						bool flag3 = false;
						try
						{
							Monster obj = this;
							Monitor.Enter(this, ref flag3);
							this._SafeAction = this._Action;
						}
						finally
						{
							if (flag3)
							{
								Monster obj;
								Monitor.Exit(obj);
							}
						}
						this.ChangeAction(false);
						this.FrameCounter = this.StartFrame;
					}
					else
					{
						this.FrameCounter = ((this.FrameCounter >= this.EndFrame) ? this.StartFrame : (this.FrameCounter + 1));
					}
				}
			}
		}

		// Token: 0x06002D83 RID: 11651 RVA: 0x00284E04 File Offset: 0x00283004
		private double ChangeDirectionValue()
		{
			double result;
			if (1601 == this.MonsterType)
			{
				result = 0.0;
			}
			else
			{
				result = this.Direction;
			}
			return result;
		}

		// Token: 0x06002D84 RID: 11652 RVA: 0x00284E40 File Offset: 0x00283040
		private void ChangeAction(bool resetCounter)
		{
			int i = Global.GetActionIndex(this.Action);
			int frameNumber = this.MonsterInfo.EachActionFrameRange[i];
			if (frameNumber <= 0)
			{
			}
			int newDirection = (int)this.ChangeDirectionValue();
			if (this.Action == GActions.Death)
			{
			}
			if (1601 == this.MonsterType)
			{
				newDirection = 0;
			}
			int actionTick = Global.GetActionTick(this.Action, this.MonsterInfo.SpriteSpeedTickList);
			if (actionTick <= 0)
			{
				actionTick = 100;
			}
			if (GActions.Death == this.Action)
			{
				actionTick *= 2;
			}
			if (GActions.PreAttack == this.Action)
			{
				int preAttackIndex = Global.GetActionIndex(GActions.Stand);
				frameNumber = this.MonsterInfo.EachActionFrameRange[preAttackIndex];
				actionTick = Global.GetActionTick(GActions.Stand, this.MonsterInfo.SpriteSpeedTickList);
			}
			switch (this.Action)
			{
			case GActions.Stand:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.Walk:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.Run:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.Attack:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.Injured:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.Magic:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.Bow:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.Death:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.HorseStand:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.HorseRun:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.HorseDead:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.Sit:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			case GActions.PreAttack:
				this.RefreshThread((double)actionTick, newDirection * frameNumber, (newDirection + 1) * frameNumber - 1);
				break;
			}
			if (resetCounter)
			{
				this.FrameCounter = this.StartFrame;
			}
			else if (this.FrameCounter < this.StartFrame || this.StartFrame >= this.EndFrame)
			{
				this.FrameCounter = this.StartFrame;
			}
		}

		// Token: 0x06002D85 RID: 11653 RVA: 0x00285115 File Offset: 0x00283315
		private void RefreshThread(double timeSpan, int startFrame, int endFrame)
		{
			this._HeartInterval = (long)timeSpan;
			this.StartFrame = startFrame;
			this.EndFrame = endFrame;
		}

		// Token: 0x06002D86 RID: 11654 RVA: 0x00285130 File Offset: 0x00283330
		private void DoAction()
		{
			GActions action = this.Action;
			if (action != GActions.Attack)
			{
				if (action == GActions.Death)
				{
					this.isDeath = true;
				}
			}
			else if (this.LockObject == -1)
			{
				if (null != this.SpriteChangeAction)
				{
					this.SpriteChangeAction(this, new SpriteChangeActionEventArgs
					{
						Action = 0
					});
				}
			}
			else
			{
				SystemXmlItem systemMagic = null;
				if (this.CurrentMagic > 0)
				{
					if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(this.CurrentMagic, out systemMagic))
					{
						return;
					}
					if (systemMagic.GetIntValue("InjureType", -1) == 1)
					{
						return;
					}
					if (this.MagicFinish == -2)
					{
						return;
					}
					if (this.MyMagicsManyTimeDmageQueue.GetManyTimeDmageQueueItemNumEx() > 0)
					{
						return;
					}
				}
				Global.DoInjure(this, this._LockObject, this.EnemyTarget);
			}
		}

		// Token: 0x06002D87 RID: 11655 RVA: 0x00285237 File Offset: 0x00283437
		protected void OnReallyDied()
		{
			this.MonsterZoneNode.OnReallyDied(this);
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06002D88 RID: 11656 RVA: 0x00285248 File Offset: 0x00283448
		// (set) Token: 0x06002D89 RID: 11657 RVA: 0x00285260 File Offset: 0x00283460
		public int CurrentMagic
		{
			get
			{
				return this._CurrentMagic;
			}
			set
			{
				this._CurrentMagic = value;
			}
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06002D8A RID: 11658 RVA: 0x0028526C File Offset: 0x0028346C
		// (set) Token: 0x06002D8B RID: 11659 RVA: 0x00285284 File Offset: 0x00283484
		public int CurrentMagicLevel
		{
			get
			{
				return this._CurrentMagicLevel;
			}
			set
			{
				this._CurrentMagicLevel = value;
			}
		}

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06002D8C RID: 11660 RVA: 0x00285290 File Offset: 0x00283490
		// (set) Token: 0x06002D8D RID: 11661 RVA: 0x002852A8 File Offset: 0x002834A8
		public int SurvivalTime
		{
			get
			{
				return this._SurvivalTime;
			}
			set
			{
				this._SurvivalTime = value;
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06002D8E RID: 11662 RVA: 0x002852B4 File Offset: 0x002834B4
		// (set) Token: 0x06002D8F RID: 11663 RVA: 0x002852CC File Offset: 0x002834CC
		public long SurvivalTick
		{
			get
			{
				return this._SurvivalTick;
			}
			set
			{
				this._SurvivalTick = value;
			}
		}

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06002D90 RID: 11664 RVA: 0x002852D8 File Offset: 0x002834D8
		// (set) Token: 0x06002D91 RID: 11665 RVA: 0x002852F0 File Offset: 0x002834F0
		public int MagicFinish
		{
			get
			{
				return this._MagicFinish;
			}
			set
			{
				this._MagicFinish = value;
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06002D92 RID: 11666 RVA: 0x002852FC File Offset: 0x002834FC
		public MagicCoolDownMgr MyMagicCoolDownMgr
		{
			get
			{
				return this._MagicCoolDownMgr;
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06002D93 RID: 11667 RVA: 0x00285314 File Offset: 0x00283514
		// (set) Token: 0x06002D94 RID: 11668 RVA: 0x0028532C File Offset: 0x0028352C
		public long MaxAttackTimeSlot
		{
			get
			{
				return this._MaxAttackTimeSlot;
			}
			set
			{
				this._MaxAttackTimeSlot = value;
			}
		}

		// Token: 0x06002D95 RID: 11669 RVA: 0x00285380 File Offset: 0x00283580
		public void AddDynSkillID(int skillID, int priority)
		{
			lock (this.CurrentSkillIDIndexLock)
			{
				bool found = false;
				for (int i = 0; i < this.DynSkillIDsList.Count; i++)
				{
					if (this.DynSkillIDsList[i].SkillID == skillID)
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					this.DynSkillIDsList.Add(new DynSkillItem
					{
						SkillID = skillID,
						Priority = priority
					});
					this.DynSkillIDsList.Sort(delegate(DynSkillItem left, DynSkillItem right)
					{
						int result;
						if (left.Priority < right.Priority)
						{
							result = 1;
						}
						else if (left.Priority == right.Priority)
						{
							result = 0;
						}
						else
						{
							result = -1;
						}
						return result;
					});
				}
			}
		}

		// Token: 0x06002D96 RID: 11670 RVA: 0x00285460 File Offset: 0x00283660
		public void RemoveDynSkill(int skillID)
		{
			lock (this.CurrentSkillIDIndexLock)
			{
				for (int i = 0; i < this.DynSkillIDsList.Count; i++)
				{
					if (this.DynSkillIDsList[i].SkillID == skillID)
					{
						this.DynSkillIDsList.RemoveAt(i);
						break;
					}
				}
			}
		}

		// Token: 0x06002D97 RID: 11671 RVA: 0x002854EC File Offset: 0x002836EC
		private void ClearDynSkill()
		{
			lock (this.CurrentSkillIDIndexLock)
			{
				this._ToExecSkillID = -1;
				this.DynSkillIDsList.Clear();
				this._CurrentSkillIDIndex = 0;
			}
		}

		// Token: 0x06002D98 RID: 11672 RVA: 0x0028554C File Offset: 0x0028374C
		public int GetAutoUseSkillID()
		{
			int skillID = -1;
			lock (this.CurrentSkillIDIndexLock)
			{
				if (this._ToExecSkillID > 0)
				{
					skillID = this._ToExecSkillID;
					return skillID;
				}
				if (this.DynSkillIDsList.Count > 0)
				{
					if (this._CurrentSkillIDIndex >= this.DynSkillIDsList.Count)
					{
						this._CurrentSkillIDIndex = 0;
					}
					for (int i = this._CurrentSkillIDIndex; i < this.DynSkillIDsList.Count; i++)
					{
						if (this.MyMagicCoolDownMgr.SkillCoolDown(this.DynSkillIDsList[i].SkillID))
						{
							if (this.SkillNeedMagicVOk(this, this.DynSkillIDsList[i].SkillID))
							{
								skillID = this.DynSkillIDsList[i].SkillID;
								break;
							}
						}
					}
				}
				if (skillID <= 0)
				{
					if (null != this.MonsterInfo.SkillIDs)
					{
						if (this._CurrentSkillIDIndex >= this.MonsterInfo.SkillIDs.Length)
						{
							this._CurrentSkillIDIndex = 0;
						}
						for (int i = this._CurrentSkillIDIndex; i < this.MonsterInfo.SkillIDs.Length; i++)
						{
							if (this.MyMagicCoolDownMgr.SkillCoolDown(this.MonsterInfo.SkillIDs[i]))
							{
								if (this.SkillNeedMagicVOk(this, this.MonsterInfo.SkillIDs[i]))
								{
									skillID = this.MonsterInfo.SkillIDs[i];
									break;
								}
							}
						}
					}
				}
			}
			return skillID;
		}

		// Token: 0x06002D99 RID: 11673 RVA: 0x00285744 File Offset: 0x00283944
		protected bool SkillNeedMagicVOk(Monster monster, int skillID)
		{
			int usedMagicV = Global.GetNeedMagicV(monster, skillID, 1);
			if (usedMagicV > 0)
			{
				int nMax = (int)monster.MonsterInfo.VManaMax;
				int nNeed = (int)((double)nMax * ((double)usedMagicV / 100.0));
				nNeed = Global.GMax(0, nNeed);
				if (monster.VMana - (double)nNeed < 0.0)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06002D9A RID: 11674 RVA: 0x002857B8 File Offset: 0x002839B8
		// (set) Token: 0x06002D9B RID: 11675 RVA: 0x002857CF File Offset: 0x002839CF
		public long AddToDeadQueueTicks { get; set; }

		// Token: 0x06002D9C RID: 11676 RVA: 0x002857D8 File Offset: 0x002839D8
		public static void IncMonsterCount()
		{
			lock (Monster.CountLock)
			{
				Monster.TotalMonsterCount++;
			}
		}

		// Token: 0x06002D9D RID: 11677 RVA: 0x00285828 File Offset: 0x00283A28
		public static void DecMonsterCount()
		{
			lock (Monster.CountLock)
			{
				Monster.TotalMonsterCount--;
			}
		}

		// Token: 0x06002D9E RID: 11678 RVA: 0x00285878 File Offset: 0x00283A78
		public static int GetMonsterCount()
		{
			int count = 0;
			lock (Monster.CountLock)
			{
				count = Monster.TotalMonsterCount;
			}
			return count;
		}

		// Token: 0x06002D9F RID: 11679 RVA: 0x002858CC File Offset: 0x00283ACC
		public long GetMonsterLivingTicks()
		{
			return TimeUtil.NOW() - this.LastMonsterLivingTicks;
		}

		// Token: 0x06002DA0 RID: 11680 RVA: 0x002858EC File Offset: 0x00283AEC
		public long GetMonsterBirthTick()
		{
			return this.LastMonsterLivingTicks * 10000L;
		}

		// Token: 0x06002DA1 RID: 11681 RVA: 0x0028590B File Offset: 0x00283B0B
		public void ResetMonsterBirthTick()
		{
			this.LastMonsterLivingSlotTicks = TimeUtil.NOW();
			this.LastMonsterLivingTicks = TimeUtil.NOW();
		}

		// Token: 0x06002DA2 RID: 11682 RVA: 0x00285924 File Offset: 0x00283B24
		public bool CanExecBossAI(BossAIItem bossAIItem)
		{
			bool result;
			if (bossAIItem.TriggerNum <= 0 && bossAIItem.TriggerCD <= 0)
			{
				result = true;
			}
			else
			{
				int num = 0;
				long lastTicks = 0L;
				lock (this.TriggerMutex)
				{
					if (bossAIItem.TriggerNum > 0)
					{
						this.TriggerNumDict.TryGetValue(bossAIItem.ID, out num);
						if (num >= bossAIItem.TriggerNum)
						{
							return false;
						}
					}
					if (bossAIItem.TriggerCD > 0)
					{
						this.TriggerCDDict.TryGetValue(bossAIItem.ID, out lastTicks);
						if (TimeUtil.NOW() - lastTicks < (long)(bossAIItem.TriggerCD * 1000))
						{
							return false;
						}
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06002DA3 RID: 11683 RVA: 0x00285A24 File Offset: 0x00283C24
		public bool RecBossAI(BossAIItem bossAIItem)
		{
			int num = 0;
			lock (this.TriggerMutex)
			{
				if (bossAIItem.TriggerNum > 0)
				{
					if (!this.TriggerNumDict.TryGetValue(bossAIItem.ID, out num))
					{
						num = 0;
					}
					num++;
					this.TriggerNumDict[bossAIItem.ID] = num;
				}
				if (bossAIItem.TriggerCD > 0)
				{
					this.TriggerCDDict[bossAIItem.ID] = TimeUtil.NOW();
				}
			}
			return true;
		}

		// Token: 0x06002DA4 RID: 11684 RVA: 0x00285AEC File Offset: 0x00283CEC
		public void ClearBossAI()
		{
			lock (this.TriggerMutex)
			{
				this.TriggerNumDict.Clear();
				this.TriggerCDDict.Clear();
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06002DA5 RID: 11685 RVA: 0x00285B4C File Offset: 0x00283D4C
		// (set) Token: 0x06002DA6 RID: 11686 RVA: 0x00285B64 File Offset: 0x00283D64
		public bool IsCollected
		{
			get
			{
				return this._IsCollected;
			}
			set
			{
				this._IsCollected = value;
			}
		}

		// Token: 0x04003B81 RID: 15233
		public DynamicData DynamicData = new DynamicData();

		// Token: 0x04003B82 RID: 15234
		public bool Alive = false;

		// Token: 0x04003B83 RID: 15235
		public MonsterFlags Flags = new MonsterFlags();

		// Token: 0x04003B84 RID: 15236
		public bool AllwaySearchEnemy = false;

		// Token: 0x04003B85 RID: 15237
		private List<VisibleItem> _VisibleItemList = null;

		// Token: 0x04003B86 RID: 15238
		private long _LastLogAttackerTicks = 0L;

		// Token: 0x04003B87 RID: 15239
		private Dictionary<int, MonsterAttackerLog> _AttackerLogDict = new Dictionary<int, MonsterAttackerLog>();

		// Token: 0x04003B88 RID: 15240
		public long LastExecTimerTicks = 0L;

		// Token: 0x04003B89 RID: 15241
		private double _MoveSpeed = 1.0;

		// Token: 0x04003B8A RID: 15242
		private Point _DestPoint = new Point(-1.0, -1.0);

		// Token: 0x04003B8B RID: 15243
		private int _CopyMapID = -1;

		// Token: 0x04003B8C RID: 15244
		public bool _HandledDead = false;

		// Token: 0x04003B8D RID: 15245
		public int VisibleClientsNum = 0;

		// Token: 0x04003B8E RID: 15246
		public long UniqueID;

		// Token: 0x04003B8F RID: 15247
		public object Tag;

		// Token: 0x04003B90 RID: 15248
		public SceneUIClasses ManagerType = SceneUIClasses.Normal;

		// Token: 0x04003B91 RID: 15249
		public string MonsterName;

		// Token: 0x04003B92 RID: 15250
		public bool IsAttackRole = true;

		// Token: 0x04003B93 RID: 15251
		public bool IsAutoSearchRoad = false;

		// Token: 0x04003B94 RID: 15252
		private double _VLife = 0.0;

		// Token: 0x04003B95 RID: 15253
		private double _VMana = 0.0;

		// Token: 0x04003B96 RID: 15254
		public int Camp;

		// Token: 0x04003B97 RID: 15255
		public GameClient OwnerClient = null;

		// Token: 0x04003B98 RID: 15256
		public Monster OwnerMonster = null;

		// Token: 0x04003B99 RID: 15257
		public Monster CallMonster = null;

		// Token: 0x04003B9A RID: 15258
		public int _PetAiControlType = 2;

		// Token: 0x04003B9B RID: 15259
		public Dictionary<int, BufferData> BufferDataDict = null;

		// Token: 0x04003B9C RID: 15260
		public long DSStartDSAddLifeNoShowTicks = 0L;

		// Token: 0x04003B9D RID: 15261
		public long DSStartDSSubLifeNoShowTicks = 0L;

		// Token: 0x04003B9E RID: 15262
		public int FangDuRoleID = 0;

		// Token: 0x04003B9F RID: 15263
		public long DongJieStart = 0L;

		// Token: 0x04003BA0 RID: 15264
		public int DongJieSeconds = 0;

		// Token: 0x04003BA1 RID: 15265
		public long XuanYunStart = 0L;

		// Token: 0x04003BA2 RID: 15266
		public int XuanYunSeconds = 0;

		// Token: 0x04003BA3 RID: 15267
		public long DingShenStart = 0L;

		// Token: 0x04003BA4 RID: 15268
		public int DingShenSeconds = 0;

		// Token: 0x04003BA5 RID: 15269
		public long SpeedDownStart = 0L;

		// Token: 0x04003BA6 RID: 15270
		public int SpeedDownSeconds = 0;

		// Token: 0x04003BA8 RID: 15272
		private long _LastDeadTicks = 0L;

		// Token: 0x04003BAA RID: 15274
		private Point _SafeCoordinate;

		// Token: 0x04003BAB RID: 15275
		private Point _SafeOldCoordinate = new Point(0.0, 0.0);

		// Token: 0x04003BAC RID: 15276
		private bool _FirstStoryMove = false;

		// Token: 0x04003BAD RID: 15277
		public int SubMapCode = -1;

		// Token: 0x04003BAE RID: 15278
		private Point firstGrid = default(Point);

		// Token: 0x04003BAF RID: 15279
		public bool isReturn = false;

		// Token: 0x04003BB0 RID: 15280
		private Point _Coordinate = new Point(0.0, 0.0);

		// Token: 0x04003BB1 RID: 15281
		private double _SafeDirection = 0.0;

		// Token: 0x04003BB2 RID: 15282
		private double _Direction = 0.0;

		// Token: 0x04003BB3 RID: 15283
		private int _LockObject = -1;

		// Token: 0x04003BB4 RID: 15284
		private long _LastActionTick = 0L;

		// Token: 0x04003BB5 RID: 15285
		private long _LastAttackActionTick = 0L;

		// Token: 0x04003BB6 RID: 15286
		private GActions OldAction = GActions.Attack;

		// Token: 0x04003BB7 RID: 15287
		public GActions _Action;

		// Token: 0x04003BB8 RID: 15288
		private GActions _SafeAction;

		// Token: 0x04003BB9 RID: 15289
		public long _HeartInterval = 400L;

		// Token: 0x04003BBB RID: 15291
		private bool isDeath = false;

		// Token: 0x04003BBC RID: 15292
		private int deathDelay = 0;

		// Token: 0x04003BBD RID: 15293
		private int _CurrentMagic = -1;

		// Token: 0x04003BBE RID: 15294
		private int _CurrentMagicLevel = 1;

		// Token: 0x04003BBF RID: 15295
		private int _SurvivalTime = 0;

		// Token: 0x04003BC0 RID: 15296
		private long _SurvivalTick = 0L;

		// Token: 0x04003BC1 RID: 15297
		private int _MagicFinish = 0;

		// Token: 0x04003BC2 RID: 15298
		private MagicCoolDownMgr _MagicCoolDownMgr = new MagicCoolDownMgr();

		// Token: 0x04003BC3 RID: 15299
		private long _MaxAttackTimeSlot = 2000L;

		// Token: 0x04003BC4 RID: 15300
		public object CurrentSkillIDIndexLock = new object();

		// Token: 0x04003BC5 RID: 15301
		public int _CurrentSkillIDIndex = 0;

		// Token: 0x04003BC6 RID: 15302
		public int _ToExecSkillID = -1;

		// Token: 0x04003BC7 RID: 15303
		private List<DynSkillItem> DynSkillIDsList = new List<DynSkillItem>();

		// Token: 0x04003BC8 RID: 15304
		public bool DynamicMonster = false;

		// Token: 0x04003BC9 RID: 15305
		public int DynamicPursuitRadius = 0;

		// Token: 0x04003BCA RID: 15306
		private static object CountLock = new object();

		// Token: 0x04003BCB RID: 15307
		private static int TotalMonsterCount = 0;

		// Token: 0x04003BCC RID: 15308
		public MonsterBuffer TempPropsBuffer = new MonsterBuffer();

		// Token: 0x04003BCD RID: 15309
		public int Step;

		// Token: 0x04003BCE RID: 15310
		public long MoveTime;

		// Token: 0x04003BCF RID: 15311
		public List<int[]> PatrolPath;

		// Token: 0x04003BD0 RID: 15312
		private long LastMonsterLivingSlotTicks = TimeUtil.NOW();

		// Token: 0x04003BD1 RID: 15313
		private long LastMonsterLivingTicks = TimeUtil.NOW();

		// Token: 0x04003BD2 RID: 15314
		public object TriggerMutex = new object();

		// Token: 0x04003BD3 RID: 15315
		private Dictionary<int, int> TriggerNumDict = new Dictionary<int, int>();

		// Token: 0x04003BD4 RID: 15316
		private Dictionary<int, long> TriggerCDDict = new Dictionary<int, long>();

		// Token: 0x04003BD5 RID: 15317
		public SpriteExtensionProps ExtensionProps = new SpriteExtensionProps();

		// Token: 0x04003BD6 RID: 15318
		public MagicsManyTimeDmageQueue MyMagicsManyTimeDmageQueue = new MagicsManyTimeDmageQueue();

		// Token: 0x04003BD7 RID: 15319
		public BufferExtManager MyBufferExtManager = new BufferExtManager();

		// Token: 0x04003BD8 RID: 15320
		public TimedActionManager TimedActionMgr = new TimedActionManager();

		// Token: 0x04003BD9 RID: 15321
		public object CaiJiStateLock = new object();

		// Token: 0x04003BDA RID: 15322
		private bool _IsCollected = false;
	}
}
