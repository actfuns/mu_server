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
    
    public class Monster : IObject
    {
        
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

        
        
        
        public MonsterZone MonsterZoneNode { get; set; }

        
        
        
        public MonsterStaticInfo MonsterInfo { get; set; }

        
        
        public ObjectTypes ObjectType
        {
            get
            {
                return ObjectTypes.OT_MONSTER;
            }
        }

        
        public int GetObjectID()
        {
            return this.RoleID;
        }

        
        
        
        public long LastLifeMagicTick { get; set; }

        
        
        
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

        
        
        public int CurrentMapCode
        {
            get
            {
                return this.MonsterZoneNode.MapCode;
            }
        }

        
        
        
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

        
        
        
        public long LastInObsJugeTicks { get; set; }

        
        
        
        public long LastSeekEnemyTicks { get; set; }

        
        
        
        public long NextSeekEnemyTicks { get; set; }

        
        
        
        public long LastLockEnemyTicks { get; set; }

        
        
        
        public long LockFocusTime { get; set; }

        
        
        
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

        
        
        
        public string Name { get; set; }

        
        
        
        public int RoleID { get; set; }

        
        
        
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

        
        
        
        public int MonsterType { get; set; }

        
        
        
        public int WhoKillMeID { get; set; }

        
        
        
        public string WhoKillMeName { get; set; }

        
        public void AddLife(long life)
        {
            if (this._VLife > 0.0 && this.Alive)
            {
                this._VLife = Math.Min(this.MonsterInfo.VLifeMax, this._VLife + (double)life);
            }
        }

        
        
        
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

        
        // (add) Token: 0x06002D53 RID: 11603 RVA: 0x002840A4 File Offset: 0x002822A4
        // (remove) Token: 0x06002D54 RID: 11604 RVA: 0x002840E0 File Offset: 0x002822E0
        public event MoveToEventHandler MoveToComplete;

        
        
        public long LastDeadTicks
        {
            get
            {
                return this._LastDeadTicks;
            }
        }

        
        
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

        
        public int GetMovingNeedTick()
        {
            double moveSpeed = this.MoveSpeed;
            if (moveSpeed < 0.05)
            {
                moveSpeed = 0.05;
            }
            return this.IsMonsterSpeedDown() ? Convert.ToInt32((double)Global.MovingNeedTicksPerGrid / moveSpeed) : Global.MovingNeedTicksPerGrid;
        }

        
        // (add) Token: 0x06002D58 RID: 11608 RVA: 0x002841C4 File Offset: 0x002823C4
        // (remove) Token: 0x06002D59 RID: 11609 RVA: 0x00284200 File Offset: 0x00282400
        public event CoordinateEventHandler CoordinateChanged;

        
        
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

        
        
        
        public Point FirstCoordinate { get; set; }

        
        public Point getFirstGrid()
        {
            if (default(Point).Equals(this.firstGrid))
            {
                GameMap gameMap = GameManager.MapMgr.DictMaps[this.MonsterZoneNode.MapCode];
                this.firstGrid = new Point((double)((int)(this.FirstCoordinate.X / (double)gameMap.MapGridWidth)), (double)((int)(this.FirstCoordinate.Y / (double)gameMap.MapGridHeight)));
            }
            return this.firstGrid;
        }

        
        
        
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

        
        
        public double SafeDirection
        {
            get
            {
                return Thread.VolatileRead(ref this._SafeDirection);
            }
        }

        
        
        
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

        
        private static void ChangeDirectionProperty2(Monster obj, double oldValue, double newValue)
        {
            lock (obj)
            {
                obj._SafeDirection = newValue;
            }
            obj.ChangeAction(false);
        }

        
        
        
        public int PetLockObjectPriority { get; set; }

        
        
        
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

        
        
        
        public int AttackRange { get; set; }

        
        
        public long LastActionTick
        {
            get
            {
                return this._LastActionTick;
            }
        }

        
        
        public long LastAttackActionTick
        {
            get
            {
                return this._LastAttackActionTick;
            }
        }

        
        
        
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

        
        
        
        public Point Destination { get; set; }

        
        
        
        public Point EnemyTarget { get; set; }

        
        
        
        public int FrameCounter { get; set; }

        
        
        
        public int StartFrame { get; set; }

        
        
        
        public int EndFrame { get; set; }

        
        // (add) Token: 0x06002D7D RID: 11645 RVA: 0x002848D4 File Offset: 0x00282AD4
        // (remove) Token: 0x06002D7E RID: 11646 RVA: 0x00284910 File Offset: 0x00282B10
        public event SpriteChangeActionEventHandler SpriteChangeAction;

        
        private void BeginHeart()
        {
            this.LastMonsterLivingSlotTicks = TimeUtil.NOW();
            this.LastMonsterLivingTicks = this.LastMonsterLivingSlotTicks;
            GlobalEventSource.getInstance().fireEvent(new MonsterBirthOnEventObject(this));
        }

        
        public void Start()
        {
            this.Alive = true;
            this.BeginHeart();
        }

        
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
                        lock (this)
                        {
                            this._SafeAction = this._Action;
                        }
                        this.ChangeAction(false);
                        this.FrameCounter = this.StartFrame;
                    }
                    else if (this.FrameCounter >= this.EndFrame && this._Action == GActions.PreAttack)
                    {
                        this._Action = this.OldAction;
                        lock (this)
                        {
                            this._SafeAction = this._Action;
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

        
        private void RefreshThread(double timeSpan, int startFrame, int endFrame)
        {
            this._HeartInterval = (long)timeSpan;
            this.StartFrame = startFrame;
            this.EndFrame = endFrame;
        }

        
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

        
        protected void OnReallyDied()
        {
            this.MonsterZoneNode.OnReallyDied(this);
        }

        
        
        
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

        
        
        public MagicCoolDownMgr MyMagicCoolDownMgr
        {
            get
            {
                return this._MagicCoolDownMgr;
            }
        }

        
        
        
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
                    this.DynSkillIDsList.Sort(delegate (DynSkillItem left, DynSkillItem right)
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

        
        private void ClearDynSkill()
        {
            lock (this.CurrentSkillIDIndexLock)
            {
                this._ToExecSkillID = -1;
                this.DynSkillIDsList.Clear();
                this._CurrentSkillIDIndex = 0;
            }
        }

        
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

        
        
        
        public long AddToDeadQueueTicks { get; set; }

        
        public static void IncMonsterCount()
        {
            lock (Monster.CountLock)
            {
                Monster.TotalMonsterCount++;
            }
        }

        
        public static void DecMonsterCount()
        {
            lock (Monster.CountLock)
            {
                Monster.TotalMonsterCount--;
            }
        }

        
        public static int GetMonsterCount()
        {
            int count = 0;
            lock (Monster.CountLock)
            {
                count = Monster.TotalMonsterCount;
            }
            return count;
        }

        
        public long GetMonsterLivingTicks()
        {
            return TimeUtil.NOW() - this.LastMonsterLivingTicks;
        }

        
        public long GetMonsterBirthTick()
        {
            return this.LastMonsterLivingTicks * 10000L;
        }

        
        public void ResetMonsterBirthTick()
        {
            this.LastMonsterLivingSlotTicks = TimeUtil.NOW();
            this.LastMonsterLivingTicks = TimeUtil.NOW();
        }

        
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

        
        public void ClearBossAI()
        {
            lock (this.TriggerMutex)
            {
                this.TriggerNumDict.Clear();
                this.TriggerCDDict.Clear();
            }
        }

        
        
        
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

        
        public DynamicData DynamicData = new DynamicData();

        
        public bool Alive = false;

        
        public MonsterFlags Flags = new MonsterFlags();

        
        public bool AllwaySearchEnemy = false;

        
        private List<VisibleItem> _VisibleItemList = null;

        
        private long _LastLogAttackerTicks = 0L;

        
        private Dictionary<int, MonsterAttackerLog> _AttackerLogDict = new Dictionary<int, MonsterAttackerLog>();

        
        public long LastExecTimerTicks = 0L;

        
        private double _MoveSpeed = 1.0;

        
        private Point _DestPoint = new Point(-1.0, -1.0);

        
        private int _CopyMapID = -1;

        
        public bool _HandledDead = false;

        
        public int VisibleClientsNum = 0;

        
        public long UniqueID;

        
        public object Tag;

        
        public SceneUIClasses ManagerType = SceneUIClasses.Normal;

        
        public string MonsterName;

        
        public bool IsAttackRole = true;

        
        public bool IsAutoSearchRoad = false;

        
        private double _VLife = 0.0;

        
        private double _VMana = 0.0;

        
        public int Camp;

        
        public GameClient OwnerClient = null;

        
        public Monster OwnerMonster = null;

        
        public Monster CallMonster = null;

        
        public int _PetAiControlType = 2;

        
        public Dictionary<int, BufferData> BufferDataDict = null;

        
        public long DSStartDSAddLifeNoShowTicks = 0L;

        
        public long DSStartDSSubLifeNoShowTicks = 0L;

        
        public int FangDuRoleID = 0;

        
        public long DongJieStart = 0L;

        
        public int DongJieSeconds = 0;

        
        public long XuanYunStart = 0L;

        
        public int XuanYunSeconds = 0;

        
        public long DingShenStart = 0L;

        
        public int DingShenSeconds = 0;

        
        public long SpeedDownStart = 0L;

        
        public int SpeedDownSeconds = 0;

        
        private long _LastDeadTicks = 0L;

        
        private Point _SafeCoordinate;

        
        private Point _SafeOldCoordinate = new Point(0.0, 0.0);

        
        private bool _FirstStoryMove = false;

        
        public int SubMapCode = -1;

        
        private Point firstGrid = default(Point);

        
        public bool isReturn = false;

        
        private Point _Coordinate = new Point(0.0, 0.0);

        
        private double _SafeDirection = 0.0;

        
        private double _Direction = 0.0;

        
        private int _LockObject = -1;

        
        private long _LastActionTick = 0L;

        
        private long _LastAttackActionTick = 0L;

        
        private GActions OldAction = GActions.Attack;

        
        public GActions _Action;

        
        private GActions _SafeAction;

        
        public long _HeartInterval = 400L;

        
        private bool isDeath = false;

        
        private int deathDelay = 0;

        
        private int _CurrentMagic = -1;

        
        private int _CurrentMagicLevel = 1;

        
        private int _SurvivalTime = 0;

        
        private long _SurvivalTick = 0L;

        
        private int _MagicFinish = 0;

        
        private MagicCoolDownMgr _MagicCoolDownMgr = new MagicCoolDownMgr();

        
        private long _MaxAttackTimeSlot = 2000L;

        
        public object CurrentSkillIDIndexLock = new object();

        
        public int _CurrentSkillIDIndex = 0;

        
        public int _ToExecSkillID = -1;

        
        private List<DynSkillItem> DynSkillIDsList = new List<DynSkillItem>();

        
        public bool DynamicMonster = false;

        
        public int DynamicPursuitRadius = 0;

        
        private static object CountLock = new object();

        
        private static int TotalMonsterCount = 0;

        
        public MonsterBuffer TempPropsBuffer = new MonsterBuffer();

        
        public int Step;

        
        public long MoveTime;

        
        public List<int[]> PatrolPath;

        
        private long LastMonsterLivingSlotTicks = TimeUtil.NOW();

        
        private long LastMonsterLivingTicks = TimeUtil.NOW();

        
        public object TriggerMutex = new object();

        
        private Dictionary<int, int> TriggerNumDict = new Dictionary<int, int>();

        
        private Dictionary<int, long> TriggerCDDict = new Dictionary<int, long>();

        
        public SpriteExtensionProps ExtensionProps = new SpriteExtensionProps();

        
        public MagicsManyTimeDmageQueue MyMagicsManyTimeDmageQueue = new MagicsManyTimeDmageQueue();

        
        public BufferExtManager MyBufferExtManager = new BufferExtManager();

        
        public TimedActionManager TimedActionMgr = new TimedActionManager();

        
        public object CaiJiStateLock = new object();

        
        private bool _IsCollected = false;
    }
}
