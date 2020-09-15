using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Interface;
using GameServer.Logic.NewBufferExt;
using GameServer.Logic.RefreshIconState;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
    // Token: 0x020006C6 RID: 1734
    public class GameClient : IObject
    {
        // Token: 0x060020F8 RID: 8440 RVA: 0x001C3EF0 File Offset: 0x001C20F0
        public GameClient()
        {
            int[] allyTip = new int[2];
            this.AllyTip = allyTip;
            this.CodeRevision = 0;
            this.MainExeVer = 0;
            this.ResVer = 0;
            this.KuaFuContextData = null;
            this.SceneContextData = null;
            this.SceneContextData2 = null;
            this.CheckCheatData = new CheckCheat();
            this.InterestingData = new InterestingData();
            this._RoleBuffer = new SpriteBuffer();
            this._RoleOnceBuffer = new SpriteOnceBuffer();
            this._RoleMagicHelper = new SpriteMagicHelper();
            this._RoleMultipliedBuffer = new SpriteMultipliedBuffer();
            this._AllThingsMultipliedBuffer = new SpriteMultipliedBuffer();
            this._LastLifeMagicTick = TimeUtil.NOW();
            this._LastCheckGMailTick = TimeUtil.NOW();
            this._OldGridPoint = new Point(-1.0, -1.0);
            this._OldAreaLuaIDList = null;
            this.InSafeRegion = true;
            this.lastAutoGetthingsTicks = 0L;
            this.BackgroundHandling = 0;
            this.Current9GridMutex = new object();
            this.LastRefresh9GridObjectsTicks = new long[5];
            this.CurrentSlotTicks = 2000L;
            this.ClientEffectHideFlag1 = 0;
            this.DelayStartPlayGameMsgQueue = new Queue<TCPOutPacket>();
            this.SumDamageForCopyTeam = 0L;
            this.bufferPropsManager = new BufferPropsModule();
            this.passiveSkillModule = new PassiveSkillModule();
            this.MyMagicsManyTimeDmageQueue = new MagicsManyTimeDmageQueue();
            this.propsCacheModule = new PropsCacheModule();
            this.delayExecModule = new DelayExecModule();
            this.buffManager = new BuffManager();
            this.TimedActionMgr = new TimedActionManager();
            this.OneSecsTimerEventObject = new TimerEventObject();
            this.MyBufferExtManager = new BufferExtManager();
        }

        // Token: 0x1700022F RID: 559
        // (get) Token: 0x060020F9 RID: 8441 RVA: 0x001C40B0 File Offset: 0x001C22B0
        // (set) Token: 0x060020FA RID: 8442 RVA: 0x001C40C7 File Offset: 0x001C22C7
        public TMSKSocket ClientSocket { get; set; }

        // Token: 0x17000230 RID: 560
        // (get) Token: 0x060020FB RID: 8443 RVA: 0x001C40D0 File Offset: 0x001C22D0
        // (set) Token: 0x060020FC RID: 8444 RVA: 0x001C40E7 File Offset: 0x001C22E7
        public string strUserID { get; set; }

        // Token: 0x17000231 RID: 561
        // (get) Token: 0x060020FD RID: 8445 RVA: 0x001C40F0 File Offset: 0x001C22F0
        // (set) Token: 0x060020FE RID: 8446 RVA: 0x001C4107 File Offset: 0x001C2307
        public string deviceID { get; set; }

        // Token: 0x17000232 RID: 562
        // (get) Token: 0x060020FF RID: 8447 RVA: 0x001C4110 File Offset: 0x001C2310
        // (set) Token: 0x06002100 RID: 8448 RVA: 0x001C4128 File Offset: 0x001C2328
        public SafeClientData ClientData
        {
            get
            {
                return this._ClientData;
            }
            set
            {
                this._ClientData = value;
                if (null != this._ClientData)
                {
                    this._ClientData.PropsCacheManager = new PropsCacheManager(this);
                    this.bufferPropsManager.Init(this.ClientData.PropsCacheManager);
                    this._ClientData.ChangePosHandler += this.ChangeGrid;
                    this._ClientData.PurePropsCacheManager = new PropsCacheManager(this);
                    this._ClientData.PctPropsCacheManager = new PropsCacheManager(this);
                }
            }
        }

        // Token: 0x17000233 RID: 563
        // (get) Token: 0x06002101 RID: 8449 RVA: 0x001C41B0 File Offset: 0x001C23B0
        public UsingEquipManager UsingEquipMgr
        {
            get
            {
                return this._UsingEquipMgr;
            }
        }

        // Token: 0x06002102 RID: 8450 RVA: 0x001C41C8 File Offset: 0x001C23C8
        public bool ClientLogOutOnce()
        {
            bool result;
            lock (this)
            {
                if (!this._ClientLogOut)
                {
                    this._ClientLogOut = true;
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        // Token: 0x17000234 RID: 564
        // (get) Token: 0x06002103 RID: 8451 RVA: 0x001C4220 File Offset: 0x001C2420
        public int ServerId
        {
            get
            {
                return this.ClientSocket.ServerId;
            }
        }

        // Token: 0x17000235 RID: 565
        // (get) Token: 0x06002104 RID: 8452 RVA: 0x001C4240 File Offset: 0x001C2440
        public ObjectTypes ObjectType
        {
            get
            {
                return ObjectTypes.OT_CLIENT;
            }
        }

        // Token: 0x17000236 RID: 566
        // (get) Token: 0x06002105 RID: 8453 RVA: 0x001C4254 File Offset: 0x001C2454
        // (set) Token: 0x06002106 RID: 8454 RVA: 0x001C426C File Offset: 0x001C246C
        public SpriteBuffer RoleBuffer
        {
            get
            {
                return this._RoleBuffer;
            }
            set
            {
                this._RoleBuffer = value;
            }
        }

        // Token: 0x17000237 RID: 567
        // (get) Token: 0x06002107 RID: 8455 RVA: 0x001C4278 File Offset: 0x001C2478
        // (set) Token: 0x06002108 RID: 8456 RVA: 0x001C4290 File Offset: 0x001C2490
        public SpriteOnceBuffer RoleOnceBuffer
        {
            get
            {
                return this._RoleOnceBuffer;
            }
            set
            {
                this._RoleOnceBuffer = value;
            }
        }

        // Token: 0x17000238 RID: 568
        // (get) Token: 0x06002109 RID: 8457 RVA: 0x001C429C File Offset: 0x001C249C
        // (set) Token: 0x0600210A RID: 8458 RVA: 0x001C42B4 File Offset: 0x001C24B4
        public SpriteMagicHelper RoleMagicHelper
        {
            get
            {
                return this._RoleMagicHelper;
            }
            set
            {
                this._RoleMagicHelper = value;
            }
        }

        // Token: 0x17000239 RID: 569
        // (get) Token: 0x0600210B RID: 8459 RVA: 0x001C42C0 File Offset: 0x001C24C0
        // (set) Token: 0x0600210C RID: 8460 RVA: 0x001C42D8 File Offset: 0x001C24D8
        public SpriteMultipliedBuffer RoleMultipliedBuffer
        {
            get
            {
                return this._RoleMultipliedBuffer;
            }
            set
            {
                this._RoleMultipliedBuffer = value;
            }
        }

        // Token: 0x1700023A RID: 570
        // (get) Token: 0x0600210D RID: 8461 RVA: 0x001C42E4 File Offset: 0x001C24E4
        // (set) Token: 0x0600210E RID: 8462 RVA: 0x001C42FC File Offset: 0x001C24FC
        public SpriteMultipliedBuffer AllThingsMultipliedBuffer
        {
            get
            {
                return this._AllThingsMultipliedBuffer;
            }
            set
            {
                this._AllThingsMultipliedBuffer = value;
            }
        }

        // Token: 0x0600210F RID: 8463 RVA: 0x001C4308 File Offset: 0x001C2508
        public int GetObjectID()
        {
            return this.ClientData.RoleID;
        }

        // Token: 0x1700023B RID: 571
        // (get) Token: 0x06002110 RID: 8464 RVA: 0x001C4328 File Offset: 0x001C2528
        // (set) Token: 0x06002111 RID: 8465 RVA: 0x001C4340 File Offset: 0x001C2540
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

        // Token: 0x1700023C RID: 572
        // (get) Token: 0x06002112 RID: 8466 RVA: 0x001C434C File Offset: 0x001C254C
        // (set) Token: 0x06002113 RID: 8467 RVA: 0x001C4364 File Offset: 0x001C2564
        public long LastCheckGMailTick
        {
            get
            {
                return this._LastCheckGMailTick;
            }
            set
            {
                this._LastCheckGMailTick = value;
            }
        }

        // Token: 0x1700023D RID: 573
        // (get) Token: 0x06002114 RID: 8468 RVA: 0x001C4370 File Offset: 0x001C2570
        // (set) Token: 0x06002115 RID: 8469 RVA: 0x001C4410 File Offset: 0x001C2610
        public Point CurrentGrid
        {
            get
            {
                GameMap gameMap = null;
                Point result;
                if (!GameManager.MapMgr.DictMaps.TryGetValue(this.ClientData.MapCode, out gameMap))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("GameClient CurrentGrid Error MapCode={0}", this.ClientData.MapCode), null, true);
                    result = new Point(0.0, 0.0);
                }
                else
                {
                    result = new Point((double)(this.ClientData.PosX / gameMap.MapGridWidth), (double)(this.ClientData.PosY / gameMap.MapGridHeight));
                }
                return result;
            }
            set
            {
                GameMap gameMap = GameManager.MapMgr.DictMaps[this.ClientData.MapCode];
                this.ClientData.PosX = (int)(value.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2));
                this.ClientData.PosY = (int)(value.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
            }
        }

        // Token: 0x1700023E RID: 574
        // (get) Token: 0x06002116 RID: 8470 RVA: 0x001C4488 File Offset: 0x001C2688
        // (set) Token: 0x06002117 RID: 8471 RVA: 0x001C44B7 File Offset: 0x001C26B7
        public Point CurrentPos
        {
            get
            {
                return new Point((double)this.ClientData.PosX, (double)this.ClientData.PosY);
            }
            set
            {
                this.ClientData.PosX = (int)value.X;
                this.ClientData.PosY = (int)value.Y;
            }
        }

        // Token: 0x1700023F RID: 575
        // (get) Token: 0x06002118 RID: 8472 RVA: 0x001C44E4 File Offset: 0x001C26E4
        public int CurrentMapCode
        {
            get
            {
                return this.ClientData.MapCode;
            }
        }

        // Token: 0x17000240 RID: 576
        // (get) Token: 0x06002119 RID: 8473 RVA: 0x001C4504 File Offset: 0x001C2704
        public int CurrentCopyMapID
        {
            get
            {
                return this.ClientData.CopyMapID;
            }
        }

        // Token: 0x17000241 RID: 577
        // (get) Token: 0x0600211A RID: 8474 RVA: 0x001C4524 File Offset: 0x001C2724
        // (set) Token: 0x0600211B RID: 8475 RVA: 0x001C4541 File Offset: 0x001C2741
        public Dircetions CurrentDir
        {
            get
            {
                return (Dircetions)this.ClientData.RoleDirection;
            }
            set
            {
                this.ClientData.RoleDirection = (int)value;
            }
        }

        // Token: 0x17000242 RID: 578
        // (get) Token: 0x0600211C RID: 8476 RVA: 0x001C4554 File Offset: 0x001C2754
        // (set) Token: 0x0600211D RID: 8477 RVA: 0x001C4571 File Offset: 0x001C2771
        public List<int> PassiveEffectList
        {
            get
            {
                return this.ClientData.PassiveEffectList;
            }
            set
            {
                this.ClientData.PassiveEffectList = value;
            }
        }

        // Token: 0x0600211E RID: 8478 RVA: 0x001C4580 File Offset: 0x001C2780
        public void ClearChangeGrid()
        {
            if (this._OldAreaLuaIDList != null)
            {
                this._OldAreaLuaIDList.Clear();
            }
        }

        // Token: 0x0600211F RID: 8479 RVA: 0x001C45A8 File Offset: 0x001C27A8
        public void ChangeGrid()
        {
            if (this._ClientData.MapCode >= 0)
            {
                GameMap gameMap = GameManager.MapMgr.DictMaps[this._ClientData.MapCode];
                int newGridX = this._ClientData.PosX / gameMap.MapGridWidth;
                int newGridY = this._ClientData.PosY / gameMap.MapGridHeight;
                if (this._OldGridPoint.X != (double)newGridX || this._OldGridPoint.Y != (double)newGridY)
                {
                    this._OldGridPoint = new Point((double)newGridX, (double)newGridY);
                    this.ProcessAreaLua(gameMap, this._OldGridPoint);
                    if (!gameMap.InSafeRegionList(this.CurrentGrid))
                    {
                        this.InSafeRegion = false;
                        this.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
                        {
                            DelayExecProcIds.ProcessClickGoodsPack
                        });
                    }
                    else
                    {
                        this.InSafeRegion = true;
                    }
                }
            }
        }

        // Token: 0x06002120 RID: 8480 RVA: 0x001C4698 File Offset: 0x001C2898
        public void AutoGetThingsOnAutoFight(long ticks)
        {
            if (ticks - this.lastAutoGetthingsTicks >= 20000L)
            {
                this.lastAutoGetthingsTicks = ticks;
                if (this.ClientData.AutoFighting && this.ClientData.AutoFightGetThings != 0)
                {
                    GameMap gameMap = GameManager.MapMgr.DictMaps[this._ClientData.MapCode];
                    if (!gameMap.InSafeRegionList(this.CurrentGrid))
                    {
                        GameManager.GoodsPackMgr.ProcessClickGoodsPackWhenMovingToOtherGrid(this, 5);
                    }
                }
            }
        }

        // Token: 0x06002121 RID: 8481 RVA: 0x001C4728 File Offset: 0x001C2928
        private void ProcessAreaLua(GameMap gameMap, Point p)
        {
            if (gameMap != null)
            {
                Point GridPoint = new Point(p.X, p.Y);
                List<int> areaLuaIDList = gameMap.GetAreaLuaIDListByPoint(GridPoint);
                List<int> enteLuaIDList = new List<int>();
                List<int> leaveLuaIDList = new List<int>();
                if (areaLuaIDList != null)
                {
                    if (this._OldAreaLuaIDList != null)
                    {
                        foreach (int luaID in areaLuaIDList)
                        {
                            if (!this._OldAreaLuaIDList.Contains(luaID))
                            {
                                enteLuaIDList.Add(luaID);
                            }
                        }
                    }
                    else
                    {
                        enteLuaIDList = areaLuaIDList;
                    }
                }
                if (this._OldAreaLuaIDList != null)
                {
                    if (areaLuaIDList != null)
                    {
                        foreach (int luaID in this._OldAreaLuaIDList)
                        {
                            if (!areaLuaIDList.Contains(luaID))
                            {
                                leaveLuaIDList.Add(luaID);
                            }
                        }
                    }
                    else
                    {
                        leaveLuaIDList = this._OldAreaLuaIDList;
                    }
                }
                if (leaveLuaIDList != null && leaveLuaIDList.Count > 0)
                {
                }
                if (enteLuaIDList != null && enteLuaIDList.Count > 0)
                {
                    this.RunAreaLuaFile(gameMap, RunAreaLuaType.AreaLuaIDList, enteLuaIDList, "enterArea", 0);
                }
                this._OldAreaLuaIDList = areaLuaIDList;
            }
        }

        // Token: 0x06002122 RID: 8482 RVA: 0x001C48CC File Offset: 0x001C2ACC
        public void RunAreaLuaFile(GameMap gameMap, int areaLuaID, string functionName)
        {
            GAreaLua areaLua = gameMap.GetAreaLuaByID(areaLuaID);
            if (null != areaLua)
            {
                string fileName = areaLua.LuaScriptFileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    ProcessAreaScripts.ProcessScripts(this, fileName, functionName, areaLuaID);
                }
            }
        }

        // Token: 0x06002123 RID: 8483 RVA: 0x001C4988 File Offset: 0x001C2B88
        public void RunAreaLuaFile(GameMap gameMap, RunAreaLuaType runAreaLuaType, List<int> areaLuaIDList, string functionName, int taskId = 0)
        {
            List<GAreaLua> GAreaLuaList = null;
            if (RunAreaLuaType.SelfPoint == runAreaLuaType)
            {
                int newGridX = this._ClientData.PosX / gameMap.MapGridWidth;
                int newGridY = this._ClientData.PosY / gameMap.MapGridHeight;
                GAreaLuaList = gameMap.GetAreaLuaListByPoint(new Point((double)newGridX, (double)newGridY));
            }
            else if (RunAreaLuaType.AreaLuaIDList == runAreaLuaType)
            {
                if (areaLuaIDList == null || areaLuaIDList.Count == 0)
                {
                    return;
                }
                foreach (int areaLuaID in areaLuaIDList)
                {
                    GAreaLua areaLua = gameMap.GetAreaLuaByID(areaLuaID);
                    if (areaLua != null)
                    {
                        if (GAreaLuaList == null)
                        {
                            GAreaLuaList = new List<GAreaLua>();
                        }
                        GAreaLuaList.Add(areaLua);
                    }
                }
            }
            if (GAreaLuaList != null)
            {
                using (List<GAreaLua>.Enumerator enumerator2 = GAreaLuaList.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        GAreaLua areaLuaEv = enumerator2.Current;
                        bool isTrigger = false;
                        if (areaLuaEv.AddtionType != AddtionType.NowTrigger)
                        {
                            switch (areaLuaEv.AddtionType)
                            {
                                case AddtionType.AccessTask:
                                    {
                                        TaskData taskData;
                                        lock (this.ClientData.TaskDataList)
                                        {
                                            taskData = this.ClientData.TaskDataList.Find((TaskData x) => x.DoingTaskID == areaLuaEv.TaskId);
                                            if (taskData == null)
                                            {
                                                break;
                                            }
                                        }
                                        SystemXmlItem systemTask = null;
                                        if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(areaLuaEv.TaskId, out systemTask))
                                        {
                                            if (taskData.DoingTaskVal1 < systemTask.GetIntValue(string.Format("TargetNum1", new object[0]), -1) || taskData.DoingTaskVal2 < systemTask.GetIntValue(string.Format("TargetNum2", new object[0]), -1))
                                            {
                                                isTrigger = true;
                                            }
                                        }
                                        break;
                                    }
                                case AddtionType.FinishTask:
                                    {
                                        TaskData taskData;
                                        lock (this.ClientData.TaskDataList)
                                        {
                                            taskData = this.ClientData.TaskDataList.Find((TaskData x) => x.DoingTaskID == areaLuaEv.TaskId);
                                            if (taskData == null)
                                            {
                                                break;
                                            }
                                        }
                                        if (Global.JugeTaskComplete(this, areaLuaEv.TaskId, taskData.DoingTaskVal1, taskData.DoingTaskVal2))
                                        {
                                            isTrigger = true;
                                        }
                                        break;
                                    }
                                case AddtionType.BackTask:
                                    if (taskId != 0)
                                    {
                                        if (areaLuaEv.TaskId == taskId)
                                        {
                                            isTrigger = true;
                                        }
                                    }
                                    break;
                                case AddtionType.NewMainTask:
                                    if (functionName == "takeNewMainTask" && taskId == areaLuaEv.TaskId)
                                    {
                                        isTrigger = true;
                                    }
                                    break;
                            }
                            if (!isTrigger)
                            {
                                continue;
                            }
                        }
                        foreach (KeyValuePair<AreaEventType, List<int>> areaEvent in areaLuaEv.AreaEventDict)
                        {
                            if (areaEvent.Key == AreaEventType.FinishTask)
                            {
                                int eventTaskId = areaEvent.Value[0];
                                TaskData taskData = this.ClientData.TaskDataList.Find((TaskData x) => x.DoingTaskID == eventTaskId);
                                SystemXmlItem systemTask = null;
                                if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(eventTaskId, out systemTask))
                                {
                                    if (taskData.DoingTaskVal1 < systemTask.GetIntValue("TargetNum1", -1))
                                    {
                                        taskData.DoingTaskVal1 = systemTask.GetIntValue("TargetNum1", -1);
                                    }
                                    if (taskData.DoingTaskVal2 < systemTask.GetIntValue("TargetNum2", -1))
                                    {
                                        taskData.DoingTaskVal2 = systemTask.GetIntValue("TargetNum2", -1);
                                    }
                                    GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
                                    {
                                        this.ClientData.RoleID,
                                        taskData.DoingTaskID,
                                        taskData.DbID,
                                        taskData.DoingTaskFocus,
                                        taskData.DoingTaskVal1,
                                        taskData.DoingTaskVal2
                                    }), null, this.ServerId);
                                    GameManager.ClientMgr.NotifyUpdateTask(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this, taskData.DbID, taskData.DoingTaskID, taskData.DoingTaskVal1, taskData.DoingTaskVal2, taskData.DoingTaskFocus, taskData.ChengJiuVal);
                                    int destNPC = systemTask.GetIntValue("DestNPC", -1);
                                    if (-1 != destNPC)
                                    {
                                        int state = Global.ComputeNPCTaskState(this, this.ClientData.TaskDataList, destNPC, 0);
                                        GameManager.ClientMgr.NotifyUpdateNPCTaskSate(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this, destNPC + 2130706432, state);
                                    }
                                    ProcessTask.CheckAutoCompleteTask(this);
                                }
                            }
                            else if (areaEvent.Key == AreaEventType.CallMonsters && functionName == "enterArea")
                            {
                                int monsterID = areaEvent.Value[0];
                                Global.SystemKillSummonMonster(this, MonsterTypes.AreaCallMonster);
                                GameManager.LuaMgr.CallMonstersForGameClient(this, monsterID, 1, 0, 1002, 1);
                            }
                            else if (areaEvent.Key == AreaEventType.RemoveMonsters && functionName == "enterArea")
                            {
                                int monsterid = areaEvent.Value[0];
                                Global.SystemKillSummonMonster(this, monsterid);
                            }
                            else
                            {
                                string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
                                {
                                    (int)areaEvent.Key,
                                    areaEvent.Value[0],
                                    areaLuaEv.CenterPoint.X,
                                    areaLuaEv.CenterPoint.Y
                                });
                                this.sendCmd(3000, strCmd, false);
                            }
                        }
                        string fileName = areaLuaEv.LuaScriptFileName;
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            ProcessAreaScripts.ProcessScripts(this, fileName, functionName, areaLuaEv.ID);
                        }
                    }
                }
            }
        }

        // Token: 0x06002124 RID: 8484 RVA: 0x001C5174 File Offset: 0x001C3374
        public void ClearVisibleObjects(bool recalcMonsterVisibleNum)
        {
            lock (this.ClientData.VisibleGrid9Objects)
            {
                if (recalcMonsterVisibleNum)
                {
                    List<object> keysList = this.ClientData.VisibleGrid9Objects.Keys.ToList<object>();
                    for (int i = 0; i < keysList.Count; i++)
                    {
                        object key = keysList[i];
                        if (key is Monster)
                        {
                            if ((key as Monster).CurrentCopyMapID == this.ClientData.CopyMapID)
                            {
                                (key as Monster).VisibleClientsNum--;
                            }
                        }
                    }
                }
                this.ClientData.VisibleGrid9Objects.Clear();
                this.ClientData.VisibleMeGrid9GameClients.Clear();
            }
        }

        // Token: 0x06002125 RID: 8485 RVA: 0x001C5274 File Offset: 0x001C3474
        public void SendCmdAfterStartPlayGame<T>(int cmdId, T cmdData)
        {
            lock (this.DelayStartPlayGameMsgQueue)
            {
                TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<T>(cmdData, TCPOutPacketPool.getInstance(), cmdId);
                this.DelayStartPlayGameMsgQueue.Enqueue(tcpOutPacket);
            }
        }

        // Token: 0x06002126 RID: 8486 RVA: 0x001C52D4 File Offset: 0x001C34D4
        public void SendCmdAfterStartPlayGame(int cmdId, string cmdData)
        {
            lock (this.DelayStartPlayGameMsgQueue)
            {
                TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), cmdData, cmdId);
                this.DelayStartPlayGameMsgQueue.Enqueue(tcpOutPacket);
            }
        }

        // Token: 0x06002127 RID: 8487 RVA: 0x001C5338 File Offset: 0x001C3538
        public void SendCmdAfterStartPlayGame(int cmdId, byte[] cmdData)
        {
            lock (this.DelayStartPlayGameMsgQueue)
            {
                TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), cmdData, cmdId);
                this.DelayStartPlayGameMsgQueue.Enqueue(tcpOutPacket);
            }
        }

        // Token: 0x06002128 RID: 8488 RVA: 0x001C539C File Offset: 0x001C359C
        public void SendCmdOnStartPlayGame()
        {
            lock (this.DelayStartPlayGameMsgQueue)
            {
                while (this.DelayStartPlayGameMsgQueue.Count > 0)
                {
                    TCPOutPacket tcpOutPacket = this.DelayStartPlayGameMsgQueue.Dequeue();
                    TCPManager.getInstance().MySocketListener.SendData(this.ClientSocket, tcpOutPacket, true);
                }
            }
        }

        // Token: 0x06002129 RID: 8489 RVA: 0x001C541C File Offset: 0x001C361C
        public void sendCmd(int cmdId, string cmdData, bool waitEnterScene = false)
        {
            if (waitEnterScene && this.ClientData.FirstPlayStart)
            {
                this.SendCmdAfterStartPlayGame(cmdId, cmdData);
            }
            else
            {
                TCPManager.getInstance().MySocketListener.SendData(this.ClientSocket, TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), cmdData, cmdId), true);
            }
        }

        // Token: 0x0600212A RID: 8490 RVA: 0x001C5478 File Offset: 0x001C3678
        public void sendCmd(int cmdId, byte[] cmdData, bool waitEnterScene = false)
        {
            if (waitEnterScene && this.ClientData.FirstPlayStart)
            {
                this.SendCmdAfterStartPlayGame(cmdId, cmdData);
            }
            else
            {
                TCPManager.getInstance().MySocketListener.SendData(this.ClientSocket, TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), cmdData, cmdId), true);
            }
        }

        // Token: 0x0600212B RID: 8491 RVA: 0x001C54D4 File Offset: 0x001C36D4
        public void sendOthersCmd(int cmdId, string cmdData)
        {
            List<object> objsList = Global.GetAll9Clients(this);
            if (null != objsList)
            {
                GameManager.ClientMgr.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList, cmdData, cmdId);
            }
        }

        // Token: 0x0600212C RID: 8492 RVA: 0x001C551C File Offset: 0x001C371C
        public void sendOthersCmd(int cmdId, byte[] cmdData)
        {
            List<object> objsList = Global.GetAll9Clients(this);
            if (null != objsList)
            {
                GameManager.ClientMgr.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList, cmdData, cmdId);
            }
        }

        // Token: 0x0600212D RID: 8493 RVA: 0x001C5564 File Offset: 0x001C3764
        public void sendCmd<T>(int cmdId, T cmdData, bool waitEnterScene = false)
        {
            if (waitEnterScene && this.ClientData.FirstPlayStart)
            {
                this.SendCmdAfterStartPlayGame<T>(cmdId, cmdData);
            }
            else
            {
                TCPManager.getInstance().MySocketListener.SendData(this.ClientSocket, DataHelper.ObjectToTCPOutPacket<T>(cmdData, TCPOutPacketPool.getInstance(), cmdId), true);
            }
        }

        // Token: 0x0600212E RID: 8494 RVA: 0x001C55BD File Offset: 0x001C37BD
        public void sendCmd(TCPOutPacket cmdData, bool pushBack = true)
        {
            TCPManager.getInstance().MySocketListener.SendData(this.ClientSocket, cmdData, pushBack);
        }

        // Token: 0x0600212F RID: 8495 RVA: 0x001C55D8 File Offset: 0x001C37D8
        public void PushVersion(string MainExeVer = "", string ResVer = "")
        {
            this.sendCmd(673, string.Format("{0}:{1}:{2}", this.CodeRevision, MainExeVer, ResVer), false);
        }

        // Token: 0x06002130 RID: 8496 RVA: 0x001C5600 File Offset: 0x001C3800
        public void ExecuteEnterMap(int mapCode)
        {
            if (this._ClientData.MapCode >= 0 && this._ClientData.CopyMapID >= 0)
            {
                MapTypes mapType = Global.GetMapType(mapCode);
                if (mapType >= MapTypes.NormalCopy && mapType <= MapTypes.MarriageCopy)
                {
                    CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(this._ClientData.CopyMapID);
                    if (null != copyMap)
                    {
                        lock (copyMap.EventQueue)
                        {
                            foreach (MapAIEvent e in copyMap.EventQueue)
                            {
                                int guangMuID = e.GuangMuID;
                                int show = e.Show;
                                this.sendCmd(667, string.Format("{0}:{1}", guangMuID, show), false);
                            }
                        }
                        if (!copyMap.ExecEnterMapLuaFile)
                        {
                            copyMap.ExecEnterMapLuaFile = true;
                            GameMap gameMap = GameManager.MapMgr.DictMaps[this._ClientData.MapCode];
                            if (!string.IsNullOrEmpty(gameMap.EnterMapLuaFile))
                            {
                                Global.ExcuteLuaFunction(this, gameMap.EnterMapLuaFile, "comeOn", null, null);
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x06002131 RID: 8497 RVA: 0x001C579C File Offset: 0x001C399C
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

        // Token: 0x040036AD RID: 13997
        public string deviceModel;

        // Token: 0x040036AE RID: 13998
        public string deviceOSVersion;

        // Token: 0x040036AF RID: 13999
        public string IsReconnect;

        // Token: 0x040036B0 RID: 14000
        public string deviceIdfa;

        // Token: 0x040036B1 RID: 14001
        public bool IsYueYu = false;

        // Token: 0x040036B2 RID: 14002
        public string strUserName;

        // Token: 0x040036B3 RID: 14003
        private SafeClientData _ClientData = null;

        // Token: 0x040036B4 RID: 14004
        private UsingEquipManager _UsingEquipMgr = new UsingEquipManager();

        // Token: 0x040036B5 RID: 14005
        public IconStateManager _IconStateMgr = new IconStateManager();

        // Token: 0x040036B6 RID: 14006
        public bool LogoutState = false;

        // Token: 0x040036B7 RID: 14007
        private bool _ClientLogOut = false;

        // Token: 0x040036B8 RID: 14008
        public long KuaFuSwitchServerTicks;

        // Token: 0x040036B9 RID: 14009
        public int[] AllyTip;

        // Token: 0x040036BA RID: 14010
        public int CodeRevision;

        // Token: 0x040036BB RID: 14011
        public int MainExeVer;

        // Token: 0x040036BC RID: 14012
        public int ResVer;

        // Token: 0x040036BD RID: 14013
        public object KuaFuContextData;

        // Token: 0x040036BE RID: 14014
        public object SceneContextData;

        // Token: 0x040036BF RID: 14015
        public object SceneContextData2;

        // Token: 0x040036C0 RID: 14016
        public int SceneType;

        // Token: 0x040036C1 RID: 14017
        public object SceneObject;

        // Token: 0x040036C2 RID: 14018
        public object SceneInfoObject;

        // Token: 0x040036C3 RID: 14019
        public long SceneGameId;

        // Token: 0x040036C4 RID: 14020
        public long SceneAge;

        // Token: 0x040036C5 RID: 14021
        public CheckCheat CheckCheatData;

        // Token: 0x040036C6 RID: 14022
        public InterestingData InterestingData;

        // Token: 0x040036C7 RID: 14023
        private SpriteBuffer _RoleBuffer;

        // Token: 0x040036C8 RID: 14024
        private SpriteOnceBuffer _RoleOnceBuffer;

        // Token: 0x040036C9 RID: 14025
        private SpriteMagicHelper _RoleMagicHelper;

        // Token: 0x040036CA RID: 14026
        private SpriteMultipliedBuffer _RoleMultipliedBuffer;

        // Token: 0x040036CB RID: 14027
        private SpriteMultipliedBuffer _AllThingsMultipliedBuffer;

        // Token: 0x040036CC RID: 14028
        private long _LastLifeMagicTick;

        // Token: 0x040036CD RID: 14029
        private long _LastCheckGMailTick;

        // Token: 0x040036CE RID: 14030
        private Point _OldGridPoint;

        // Token: 0x040036CF RID: 14031
        private List<int> _OldAreaLuaIDList;

        // Token: 0x040036D0 RID: 14032
        public bool InSafeRegion;

        // Token: 0x040036D1 RID: 14033
        private long lastAutoGetthingsTicks;

        // Token: 0x040036D2 RID: 14034
        public int BackgroundHandling;

        // Token: 0x040036D3 RID: 14035
        public object Current9GridMutex;

        // Token: 0x040036D4 RID: 14036
        public long[] LastRefresh9GridObjectsTicks;

        // Token: 0x040036D5 RID: 14037
        public long CurrentSlotTicks;

        // Token: 0x040036D6 RID: 14038
        public int ClientEffectHideFlag1;

        // Token: 0x040036D7 RID: 14039
        private Queue<TCPOutPacket> DelayStartPlayGameMsgQueue;

        // Token: 0x040036D8 RID: 14040
        public long SumDamageForCopyTeam;

        // Token: 0x040036D9 RID: 14041
        public BufferPropsModule bufferPropsManager;

        // Token: 0x040036DA RID: 14042
        public PassiveSkillModule passiveSkillModule;

        // Token: 0x040036DB RID: 14043
        public MagicsManyTimeDmageQueue MyMagicsManyTimeDmageQueue;

        // Token: 0x040036DC RID: 14044
        public PropsCacheModule propsCacheModule;

        // Token: 0x040036DD RID: 14045
        public DelayExecModule delayExecModule;

        // Token: 0x040036DE RID: 14046
        public BuffManager buffManager;

        // Token: 0x040036DF RID: 14047
        public TimedActionManager TimedActionMgr;

        // Token: 0x040036E0 RID: 14048
        public ExtData extData;

        // Token: 0x040036E1 RID: 14049
        public TimerEventObject OneSecsTimerEventObject;

        // Token: 0x040036E2 RID: 14050
        public BufferExtManager MyBufferExtManager;
    }
}
