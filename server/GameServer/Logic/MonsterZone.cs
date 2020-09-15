using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.MoRi;
using GameServer.Logic.RefreshIconState;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
    // Token: 0x02000764 RID: 1892
    public class MonsterZone
    {
        // Token: 0x17000383 RID: 899
        // (get) Token: 0x06003042 RID: 12354 RVA: 0x002AF460 File Offset: 0x002AD660
        // (set) Token: 0x06003043 RID: 12355 RVA: 0x002AF477 File Offset: 0x002AD677
        public int MapCode { get; set; }

        // Token: 0x17000384 RID: 900
        // (get) Token: 0x06003044 RID: 12356 RVA: 0x002AF480 File Offset: 0x002AD680
        // (set) Token: 0x06003045 RID: 12357 RVA: 0x002AF497 File Offset: 0x002AD697
        public int ID { get; set; }

        // Token: 0x17000385 RID: 901
        // (get) Token: 0x06003046 RID: 12358 RVA: 0x002AF4A0 File Offset: 0x002AD6A0
        // (set) Token: 0x06003047 RID: 12359 RVA: 0x002AF4B7 File Offset: 0x002AD6B7
        public int Code { get; set; }

        // Token: 0x17000386 RID: 902
        // (get) Token: 0x06003048 RID: 12360 RVA: 0x002AF4C0 File Offset: 0x002AD6C0
        // (set) Token: 0x06003049 RID: 12361 RVA: 0x002AF4D7 File Offset: 0x002AD6D7
        public int ToX { get; set; }

        // Token: 0x17000387 RID: 903
        // (get) Token: 0x0600304A RID: 12362 RVA: 0x002AF4E0 File Offset: 0x002AD6E0
        // (set) Token: 0x0600304B RID: 12363 RVA: 0x002AF4F7 File Offset: 0x002AD6F7
        public int ToY { get; set; }

        // Token: 0x17000388 RID: 904
        // (get) Token: 0x0600304C RID: 12364 RVA: 0x002AF500 File Offset: 0x002AD700
        // (set) Token: 0x0600304D RID: 12365 RVA: 0x002AF517 File Offset: 0x002AD717
        public int Radius { get; set; }

        // Token: 0x17000389 RID: 905
        // (get) Token: 0x0600304E RID: 12366 RVA: 0x002AF520 File Offset: 0x002AD720
        // (set) Token: 0x0600304F RID: 12367 RVA: 0x002AF537 File Offset: 0x002AD737
        public int TotalNum { get; set; }

        // Token: 0x1700038A RID: 906
        // (get) Token: 0x06003050 RID: 12368 RVA: 0x002AF540 File Offset: 0x002AD740
        // (set) Token: 0x06003051 RID: 12369 RVA: 0x002AF557 File Offset: 0x002AD757
        public int Timeslot { get; set; }

        // Token: 0x1700038B RID: 907
        // (get) Token: 0x06003052 RID: 12370 RVA: 0x002AF560 File Offset: 0x002AD760
        // (set) Token: 0x06003053 RID: 12371 RVA: 0x002AF577 File Offset: 0x002AD777
        public int PursuitRadius { get; set; }

        // Token: 0x1700038C RID: 908
        // (get) Token: 0x06003054 RID: 12372 RVA: 0x002AF580 File Offset: 0x002AD780
        // (set) Token: 0x06003055 RID: 12373 RVA: 0x002AF597 File Offset: 0x002AD797
        public int BirthType { get; set; }

        // Token: 0x1700038D RID: 909
        // (get) Token: 0x06003056 RID: 12374 RVA: 0x002AF5A0 File Offset: 0x002AD7A0
        // (set) Token: 0x06003057 RID: 12375 RVA: 0x002AF5B7 File Offset: 0x002AD7B7
        public int ConfigBirthType { get; set; }

        // Token: 0x1700038E RID: 910
        // (get) Token: 0x06003058 RID: 12376 RVA: 0x002AF5C0 File Offset: 0x002AD7C0
        // (set) Token: 0x06003059 RID: 12377 RVA: 0x002AF5D7 File Offset: 0x002AD7D7
        public int SpawnMonstersAfterKaiFuDays { get; set; }

        // Token: 0x1700038F RID: 911
        // (get) Token: 0x0600305A RID: 12378 RVA: 0x002AF5E0 File Offset: 0x002AD7E0
        // (set) Token: 0x0600305B RID: 12379 RVA: 0x002AF5F7 File Offset: 0x002AD7F7
        public int SpawnMonstersDays { get; set; }

        // Token: 0x17000390 RID: 912
        // (get) Token: 0x0600305C RID: 12380 RVA: 0x002AF600 File Offset: 0x002AD800
        // (set) Token: 0x0600305D RID: 12381 RVA: 0x002AF617 File Offset: 0x002AD817
        public List<BirthTimeForDayOfWeek> SpawnMonstersDayOfWeek { get; set; }

        // Token: 0x17000391 RID: 913
        // (get) Token: 0x0600305E RID: 12382 RVA: 0x002AF620 File Offset: 0x002AD820
        // (set) Token: 0x0600305F RID: 12383 RVA: 0x002AF637 File Offset: 0x002AD837
        public List<BirthTimePoint> BirthTimePointList { get; set; }

        // Token: 0x17000392 RID: 914
        // (get) Token: 0x06003060 RID: 12384 RVA: 0x002AF640 File Offset: 0x002AD840
        // (set) Token: 0x06003061 RID: 12385 RVA: 0x002AF657 File Offset: 0x002AD857
        public int BirthRate { get; set; }

        // Token: 0x06003062 RID: 12386 RVA: 0x002AF660 File Offset: 0x002AD860
        public MonsterStaticInfo GetMonsterInfo()
        {
            return this.MonsterInfo;
        }

        // Token: 0x06003063 RID: 12387 RVA: 0x002AF678 File Offset: 0x002AD878
        private void LoadMonster(Monster monster, MonsterZone monsterZone, MonsterStaticInfo monsterInfo, int monsterType, int roleID, string name, double life, double mana, Point coordinate, double direction, double moveSpeed, int attackRange)
        {
            monster.Name = name;
            monster.MonsterZoneNode = monsterZone;
            monster.MonsterInfo = monsterInfo;
            monster.RoleID = roleID;
            monster.VLife = life;
            monster.VMana = mana;
            monster.AttackRange = attackRange;
            monster.FirstCoordinate = coordinate;
            monster.Coordinate = coordinate;
            monster.Direction = direction;
            monster.MoveSpeed = moveSpeed;
            monster.MonsterType = monsterType;
            if (monster.MonsterInfo.ExtProps != null)
            {
                Array.Copy(monster.MonsterInfo.ExtProps, monster.DynamicData.ExtProps, 177);
            }
            monster.CoordinateChanged += this.UpdateMonsterEvent;
            monster.NextSeekEnemyTicks = (long)(3000 + Global.GetRandomNumber(0, 2000));
        }

        // Token: 0x06003064 RID: 12388 RVA: 0x002AF754 File Offset: 0x002AD954
        private Monster CopyMonster(Monster oldMonster)
        {
            Monster monster = oldMonster.Clone();
            monster.CoordinateChanged += this.UpdateMonsterEvent;
            monster.MoveToComplete += new MoveToEventHandler(this.MoveToComplete);
            return monster;
        }

        // Token: 0x06003065 RID: 12389 RVA: 0x002AF794 File Offset: 0x002AD994
        private void DestroyMonster(Monster monster)
        {
            if (monster.OwnerClient != null)
            {
                monster.OwnerClient.ClientData.SummonMonstersList.Remove(monster);
                monster.OwnerClient = null;
            }
            if (monster.OwnerMonster != null)
            {
                monster.OwnerMonster.CallMonster = null;
                monster.OwnerMonster = null;
            }
            monster.CoordinateChanged -= this.UpdateMonsterEvent;
            monster.MoveToComplete -= new MoveToEventHandler(this.MoveToComplete);
            GameManager.MapGridMgr.DictGrids[this.MapCode].RemoveObject(monster);
            bool ret = this.MonsterList.Remove(monster);
            GameManager.MonsterMgr.RemoveMonster(monster);
            GameManager.MonsterIDMgr.PushBack((long)monster.RoleID);
            if (ret)
            {
                Monster.DecMonsterCount();
            }
        }

        // Token: 0x06003066 RID: 12390 RVA: 0x002AF870 File Offset: 0x002ADA70
        private Monster InitMonster(XElement monsterXml, double maxLifeV, double maxMagicV, XElement xmlFrameConfig, double moveSpeed, bool attachEvent = true)
        {
            GameMap gameMap = GameManager.MapMgr.DictMaps[this.MapCode];
            Monster monster = new Monster();
            int roleID = (int)GameManager.MonsterIDMgr.GetNewID(this.MapCode);
            monster.UniqueID = Global.GetUniqueID();
            this.LoadMonster(monster, this, this.MonsterInfo, (int)Global.GetSafeAttributeLong(monsterXml, "MonsterType"), roleID, string.Format("Role_{0}", roleID), maxLifeV, maxMagicV, Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MapCode, this.ToX, this.ToY, this.Radius, 0, true), (double)Global.GetRandomNumber(0, 8), moveSpeed, (int)Global.GetSafeAttributeLong(monsterXml, "AttackRange"));
            if (attachEvent)
            {
                monster.MoveToComplete += new MoveToEventHandler(this.MoveToComplete);
            }
            return monster;
        }

        // Token: 0x06003067 RID: 12391 RVA: 0x002AF940 File Offset: 0x002ADB40
        private bool CanRealiveByRate()
        {
            bool result;
            if (this.BirthRate >= 10000)
            {
                result = true;
            }
            else
            {
                int randNum = Global.GetRandomNumber(1, 10001);
                result = (randNum <= this.BirthRate);
            }
            return result;
        }

        // Token: 0x06003068 RID: 12392 RVA: 0x002AF988 File Offset: 0x002ADB88
        public void LoadStaticMonsterInfo_2()
        {
            MonsterStaticInfo monster = MonsterStaticInfoMgr.GetInfo(this.Code);
            if (null == monster)
            {
                this.LoadStaticMonsterInfo();
            }
            else
            {
                this.MonsterInfo = monster;
            }
        }

        // Token: 0x06003069 RID: 12393 RVA: 0x002AF9C4 File Offset: 0x002ADBC4
        public void LoadStaticMonsterInfo()
        {
            string fileName = string.Format("Config/Monsters.xml", new object[0]);
            XElement xml = GameManager.MonsterZoneMgr.AllMonstersXml;
            if (xml == null)
            {
                throw new Exception(string.Format("加载系统怪物配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
            }
            XElement monsterXml = Global.GetSafeXElement(xml, "Monster", "ID", this.Code.ToString());
            fileName = string.Format("GuaiWu/{0}.xml", Global.GetSafeAttributeStr(monsterXml, "ResName"));
            string defaultFileName = string.Format("GuaiWu/ceshi_guaiwu.unity3d.xml", new object[0]);
            try
            {
                xml = null;
                string fileFullName = Global.ResPath(fileName);
                if (File.Exists(fileFullName))
                {
                    xml = XElement.Load(fileFullName);
                }
            }
            catch (Exception)
            {
                xml = null;
            }
            if (null == xml)
            {
                fileName = defaultFileName;
                xml = null;
                string fileFullName = Global.ResPath(fileName);
                if (File.Exists(fileFullName))
                {
                    xml = XElement.Load(fileFullName);
                }
                if (null == xml)
                {
                    throw new Exception(string.Format("加载指定怪物的衣服代号:{0}, 失败。没有找到相关XML配置文件!", fileName));
                }
            }
            XElement xmlFrameConfig = Global.GetSafeXElement(xml, "FrameConfig");
            XElement xmlSpeedConfig = Global.GetSafeXElement(xml, "SpeedConfig");
            int[] speedTickList = Global.StringArray2IntArray(Global.GetSafeAttributeStr(xmlSpeedConfig, "Tick").Split(new char[]
            {
                ','
            }));
            double moveSpeed = Global.GetSafeAttributeDouble(xmlSpeedConfig, "UnitSpeed") / 100.0;
            double monsterSpeed = Global.GetSafeAttributeDouble(monsterXml, "MonsterSpeed");
            moveSpeed *= monsterSpeed;
            double maxLifeV = (double)Global.GetSafeAttributeLong(monsterXml, "MaxLife");
            double maxMagicV = (double)Global.GetSafeAttributeLong(monsterXml, "MaxMagic");
            if (maxLifeV <= 0.0)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("怪物部署的时，怪物的数据配置错误，生命值不能小于等于0: MonsterID={0}, MonsterName={1}", (int)Global.GetSafeAttributeLong(monsterXml, "ID"), Global.GetSafeAttributeStr(monsterXml, "SName")), null, true);
            }
            else
            {
                this.InitMonsterStaticInfo(monsterXml, maxLifeV, maxMagicV, xmlFrameConfig, moveSpeed, speedTickList);
            }
        }

        // Token: 0x0600306A RID: 12394 RVA: 0x002AFBCC File Offset: 0x002ADDCC
        public void LoadMonsters()
        {
            string fileName = string.Format("Config/Monsters.xml", new object[0]);
            XElement xml = GameManager.MonsterZoneMgr.AllMonstersXml;
            if (xml == null)
            {
                throw new Exception(string.Format("加载系统怪物配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
            }
            XElement monsterXml = Global.GetSafeXElement(xml, "Monster", "ID", this.Code.ToString());
            MonsterNameManager.AddMonsterName(this.Code, Global.GetSafeAttributeStr(monsterXml, "SName"));
            fileName = string.Format("GuaiWu/{0}.xml", Global.GetSafeAttributeStr(monsterXml, "ResName"));
            string defaultFileName = string.Format("GuaiWu/ceshi_guaiwu.unity3d.xml", new object[0]);
            try
            {
                xml = XElement.Load(Global.ResPath(fileName));
            }
            catch (Exception)
            {
                xml = null;
            }
            if (null == xml)
            {
                fileName = defaultFileName;
                xml = XElement.Load(Global.ResPath(fileName));
                if (null == xml)
                {
                    throw new Exception(string.Format("加载指定怪物的衣服代号:{0}, 失败。没有找到相关XML配置文件!", fileName));
                }
            }
            XElement xmlFrameConfig = Global.GetSafeXElement(xml, "FrameConfig");
            XElement xmlSpeedConfig = Global.GetSafeXElement(xml, "SpeedConfig");
            double moveSpeed = Global.GetSafeAttributeDouble(xmlSpeedConfig, "UnitSpeed") / 100.0;
            double monsterSpeed = Global.GetSafeAttributeDouble(monsterXml, "MonsterSpeed");
            moveSpeed *= monsterSpeed;
            double maxLifeV = (double)Global.GetSafeAttributeLong(monsterXml, "MaxLife");
            double maxMagicV = (double)Global.GetSafeAttributeLong(monsterXml, "MaxMagic");
            if (maxLifeV <= 0.0)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("怪物部署的时，怪物的数据配置错误，生命值不能小于等于0: MonsterID={0}, MonsterName={1}", (int)Global.GetSafeAttributeLong(monsterXml, "ID"), Global.GetSafeAttributeStr(monsterXml, "SName")), null, true);
            }
            else if (!this.IsFuBenMap)
            {
                for (int i = 0; i < this.TotalNum; i++)
                {
                    Monster monster = this.InitMonster(monsterXml, maxLifeV, maxMagicV, xmlFrameConfig, moveSpeed, true);
                    if (MonsterTypes.None == this.MonsterType)
                    {
                        this.MonsterType = (MonsterTypes)monster.MonsterType;
                    }
                    this.MonsterList.Add(monster);
                    GameManager.MonsterMgr.AddMonster(monster);
                    if (401 == monster.MonsterType)
                    {
                        MonsterBossManager.AddBoss(monster);
                    }
                }
            }
            else
            {
                Monster monster = this.InitMonster(monsterXml, maxLifeV, maxMagicV, xmlFrameConfig, moveSpeed, true);
                if (MonsterTypes.None == this.MonsterType)
                {
                    this.MonsterType = (MonsterTypes)monster.MonsterType;
                }
                this.SeedMonster = monster;
            }
        }

        // Token: 0x0600306B RID: 12395 RVA: 0x002AFE64 File Offset: 0x002AE064
        public Monster LoadDynamicMonsterSeed()
        {
            string fileName = string.Format("Config/Monsters.xml", new object[0]);
            XElement xml = GameManager.MonsterZoneMgr.AllMonstersXml;
            if (xml == null)
            {
                throw new Exception(string.Format("加载系统怪物配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
            }
            XElement monsterXml = Global.GetSafeXElement(xml, "Monster", "ID", this.Code.ToString());
            MonsterNameManager.AddMonsterName(this.Code, Global.GetSafeAttributeStr(monsterXml, "SName"));
            fileName = string.Format("GuaiWu/{0}.xml", Global.GetSafeAttributeStr(monsterXml, "ResName"));
            string defaultFileName = string.Format("GuaiWu/ceshi_guaiwu.unity3d.xml", new object[0]);
            try
            {
                xml = null;
                string fileFullName = Global.ResPath(fileName);
                if (File.Exists(fileFullName))
                {
                    xml = XElement.Load(fileFullName);
                }
            }
            catch (Exception)
            {
                xml = null;
            }
            if (null == xml)
            {
                fileName = defaultFileName;
                xml = null;
                string fileFullName = Global.ResPath(fileName);
                if (File.Exists(fileFullName))
                {
                    xml = XElement.Load(fileFullName);
                }
                if (null == xml)
                {
                    throw new Exception(string.Format("加载指定怪物的衣服代号:{0}, 失败。没有找到相关XML配置文件!", fileName));
                }
            }
            XElement xmlFrameConfig = Global.GetSafeXElement(xml, "FrameConfig");
            XElement xmlSpeedConfig = Global.GetSafeXElement(xml, "SpeedConfig");
            double moveSpeed = Global.GetSafeAttributeDouble(xmlSpeedConfig, "UnitSpeed") / 100.0;
            double monsterSpeed = Global.GetSafeAttributeDouble(monsterXml, "MonsterSpeed");
            moveSpeed *= monsterSpeed;
            double maxLifeV = (double)Global.GetSafeAttributeLong(monsterXml, "MaxLife");
            double maxMagicV = (double)Global.GetSafeAttributeLong(monsterXml, "MaxMagic");
            Monster result;
            if (maxLifeV <= 0.0)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("怪物部署的时，怪物的数据配置错误，生命值不能小于等于0: MonsterID={0}, MonsterName={1}", (int)Global.GetSafeAttributeLong(monsterXml, "ID"), Global.GetSafeAttributeStr(monsterXml, "SName")), null, true);
                result = null;
            }
            else
            {
                result = this.InitMonster(monsterXml, maxLifeV, maxMagicV, xmlFrameConfig, moveSpeed, false);
            }
            return result;
        }

        // Token: 0x0600306C RID: 12396 RVA: 0x002B0060 File Offset: 0x002AE260
        private void MoveToComplete(object sender)
        {
            (sender as Monster).DestPoint = new Point(-1.0, -1.0);
            (sender as Monster).Action = GActions.Stand;
            Global.RemoveStoryboard((sender as Monster).Name);
        }

        // Token: 0x0600306D RID: 12397 RVA: 0x002B00B0 File Offset: 0x002AE2B0
        private void UpdateMonsterEvent(Monster monster)
        {
            if (!monster.FirstStoryMove)
            {
                GameManager.MapGridMgr.DictGrids[this.MapCode].MoveObject((int)monster.SafeOldCoordinate.X, (int)monster.SafeOldCoordinate.Y, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, monster);
            }
            else
            {
                monster.FirstStoryMove = false;
            }
        }

        // Token: 0x0600306E RID: 12398 RVA: 0x002B0130 File Offset: 0x002AE330
        public string GetNextBirthTimePoint()
        {
            string result;
            if (this.BirthType == 7 && this.SpawnMonstersDayOfWeek != null)
            {
                DateTime nowTime = TimeUtil.NowDateTime();
                int nextIndex = -1;
                if (this.LastBirthTimePointIndex >= 0)
                {
                    nextIndex = (this.LastBirthTimePointIndex + 1) % this.SpawnMonstersDayOfWeek.Count;
                }
                else
                {
                    DateTime now = TimeUtil.NowDateTime();
                    int time = (int)((int)now.DayOfWeek * 1440 + now.Hour * 60 + now.Minute);
                    for (int i = 0; i < this.SpawnMonstersDayOfWeek.Count; i++)
                    {
                        int time2 = this.SpawnMonstersDayOfWeek[i].BirthDayOfWeek * 1440 + this.SpawnMonstersDayOfWeek[i].BirthTime.BirthHour * 60 + this.SpawnMonstersDayOfWeek[i].BirthTime.BirthMinute;
                        if (time <= time2)
                        {
                            nextIndex = i;
                            break;
                        }
                    }
                    if (nextIndex < 0)
                    {
                        nextIndex = 0;
                    }
                }
                int dayOfWeek = this.SpawnMonstersDayOfWeek[nextIndex].BirthDayOfWeek;
                int birthHour = this.SpawnMonstersDayOfWeek[nextIndex].BirthTime.BirthHour;
                int birthMinite = this.SpawnMonstersDayOfWeek[nextIndex].BirthTime.BirthMinute;
                result = string.Format("{0}${1}${2}", birthHour, birthMinite, dayOfWeek);
            }
            else if (null == this.BirthTimePointList)
            {
                result = "";
            }
            else
            {
                int lastBirthTimePointIndex = this.LastBirthTimePointIndex;
                int nextIndex = 0;
                if (lastBirthTimePointIndex >= 0)
                {
                    nextIndex = (lastBirthTimePointIndex + 1) % this.BirthTimePointList.Count;
                }
                else
                {
                    DateTime now = TimeUtil.NowDateTime();
                    int time2 = now.Hour * 60 + now.Minute;
                    for (int i = 0; i < this.BirthTimePointList.Count; i++)
                    {
                        int time = this.BirthTimePointList[i].BirthHour * 60 + this.BirthTimePointList[i].BirthMinute;
                        if (time2 <= time)
                        {
                            nextIndex = i;
                            break;
                        }
                    }
                }
                result = string.Format("{0}${1}", this.BirthTimePointList[nextIndex].BirthHour, this.BirthTimePointList[nextIndex].BirthMinute);
            }
            return result;
        }

        // Token: 0x0600306F RID: 12399 RVA: 0x002B03C8 File Offset: 0x002AE5C8
        private bool CanBirthOnTimePoint(DateTime now, BirthTimePoint birthTimePoint)
        {
            if (now.DayOfYear == this.LastBirthDayID)
            {
                if (null != this.LastBirthTimePoint)
                {
                    if (this.LastBirthTimePoint.BirthHour == birthTimePoint.BirthHour && this.LastBirthTimePoint.BirthMinute == birthTimePoint.BirthMinute)
                    {
                        return false;
                    }
                }
            }
            bool result;
            if (now.Hour != birthTimePoint.BirthHour)
            {
                result = false;
            }
            else
            {
                while (this.LastReloadMonstersDateTime < now)
                {
                    if (this.LastReloadMonstersDateTime.Minute == birthTimePoint.BirthMinute)
                    {
                        return true;
                    }
                    this.LastReloadMonstersDateTime = this.LastReloadMonstersDateTime.AddMinutes(1.0);
                }
                result = false;
            }
            return result;
        }

        // Token: 0x06003070 RID: 12400 RVA: 0x002B04A0 File Offset: 0x002AE6A0
        private bool CanBirthOnTimePointForWeekOfDay(DateTime now, BirthTimePoint birthTimePoint)
        {
            if (now.DayOfYear == this.LastBirthDayID)
            {
                if (null != this.LastBirthTimePoint)
                {
                    if (this.LastBirthTimePoint.BirthHour == birthTimePoint.BirthHour && this.LastBirthTimePoint.BirthMinute == birthTimePoint.BirthMinute)
                    {
                        return false;
                    }
                }
            }
            bool result;
            if (now.Hour != birthTimePoint.BirthHour)
            {
                result = false;
            }
            else
            {
                int minMinute = birthTimePoint.BirthMinute;
                int maxMinute = birthTimePoint.BirthMinute + 1;
                result = (now.Minute >= minMinute && now.Minute <= maxMinute);
            }
            return result;
        }

        // Token: 0x06003071 RID: 12401 RVA: 0x002B0554 File Offset: 0x002AE754
        public void ReloadMonsters(SocketListener sl, TCPOutPacketPool pool)
        {
            if (!this.IsFuBenMap)
            {
                DateTime now = TimeUtil.NowDateTime();
                if (Global.CanMapInLimitTimes(this.MapCode, now))
                {
                    if (!this.CanTodayReloadMonsters() || !this.CanTodayReloadMonstersForDayOfWeek())
                    {
                        if (!this.HasSystemKilledAllOfThisZone)
                        {
                            this.SystemKillAllMonstersOfThisZone();
                            this.HasSystemKilledAllOfThisZone = true;
                        }
                    }
                    else
                    {
                        this.HasSystemKilledAllOfThisZone = false;
                        if (this.Code > 0 && this.ConfigBirthType == 6)
                        {
                            try
                            {
                                this.LoadStaticMonsterInfo();
                            }
                            catch (Exception ex)
                            {
                                LogManager.WriteLog(LogTypes.Error, "reload jieri boss monster failed", ex, true);
                            }
                        }
                        if (0 == this.BirthType)
                        {
                            long ticks = now.Ticks;
                            if (this.LastReloadTicks <= 0L || ticks - this.LastReloadTicks >= 10000000L)
                            {
                                this.LastReloadTicks = ticks;
                                this.MonsterRealive(sl, pool, -1, 65535);
                            }
                        }
                        else if (1 == this.BirthType)
                        {
                            if (null != this.BirthTimePointList)
                            {
                                int nextIndex = 0;
                                if (this.LastBirthTimePointIndex >= 0)
                                {
                                    nextIndex = (this.LastBirthTimePointIndex + 1) % this.BirthTimePointList.Count;
                                }
                                else
                                {
                                    for (int i = 0; i < this.BirthTimePointList.Count; i++)
                                    {
                                        if (this.CanBirthOnTimePoint(now, this.BirthTimePointList[i]))
                                        {
                                            nextIndex = i;
                                            break;
                                        }
                                    }
                                }
                                BirthTimePoint birthTimePoint = this.BirthTimePointList[nextIndex];
                                if (this.CanBirthOnTimePoint(now, birthTimePoint))
                                {
                                    this.LastBirthTimePointIndex = nextIndex;
                                    this.LastBirthTimePoint = birthTimePoint;
                                    this.LastBirthDayID = TimeUtil.NowDateTime().DayOfYear;
                                    this.MonsterRealive(sl, pool, -1, 65535);
                                }
                            }
                        }
                        else if (7 == this.BirthType)
                        {
                            if (this.SpawnMonstersDayOfWeek == null)
                            {
                                return;
                            }
                            DayOfWeek nDayOfWeek = TimeUtil.NowDateTime().DayOfWeek;
                            for (int i = 0; i < this.SpawnMonstersDayOfWeek.Count; i++)
                            {
                                int nDay = this.SpawnMonstersDayOfWeek[i].BirthDayOfWeek;
                                if (nDay == (int)nDayOfWeek)
                                {
                                    BirthTimePoint time = this.SpawnMonstersDayOfWeek[i].BirthTime;
                                    if (this.CanBirthOnTimePoint(now, time))
                                    {
                                        this.LastBirthTimePointIndex = i;
                                        this.LastBirthTimePoint = time;
                                        this.LastBirthDayID = TimeUtil.NowDateTime().DayOfYear;
                                        this.MonsterRealive(sl, pool, -1, 65535);
                                    }
                                }
                            }
                        }
                        this.LastReloadMonstersDateTime = now;
                    }
                }
            }
        }

        // Token: 0x06003072 RID: 12402 RVA: 0x002B0868 File Offset: 0x002AEA68
        public bool CanTodayReloadMonsters()
        {
            bool result;
            if (this.SpawnMonstersAfterKaiFuDays <= 0 && this.SpawnMonstersDays <= 0)
            {
                result = true;
            }
            else
            {
                DateTime kaifuTime = Global.GetKaiFuTime();
                if (this.ConfigBirthType == 5)
                {
                    HeFuActivityConfig config = HuodongCachingMgr.GetHeFuActivityConfing();
                    if (null == config)
                    {
                        return false;
                    }
                    if (!config.InList(26))
                    {
                        return false;
                    }
                    kaifuTime = Global.GetHefuStartDay();
                }
                else if (this.ConfigBirthType == 6)
                {
                    JieriActivityConfig config2 = HuodongCachingMgr.GetJieriActivityConfig();
                    if (null == config2)
                    {
                        return false;
                    }
                    if (!config2.InList(17))
                    {
                        return false;
                    }
                    kaifuTime = Global.GetJieriStartDay();
                }
                DateTime now = TimeUtil.NowDateTime();
                int days2Kaifu = Global.GetDaysSpanNum(now, kaifuTime, true) + 1;
                if (this.SpawnMonstersAfterKaiFuDays <= 0 || days2Kaifu >= this.SpawnMonstersAfterKaiFuDays)
                {
                    if (this.SpawnMonstersDays <= 0 || days2Kaifu < this.SpawnMonstersDays + this.SpawnMonstersAfterKaiFuDays)
                    {
                        return true;
                    }
                }
                result = false;
            }
            return result;
        }

        // Token: 0x06003073 RID: 12403 RVA: 0x002B0998 File Offset: 0x002AEB98
        public bool CanTodayReloadMonstersForDayOfWeek()
        {
            bool result;
            if (this.SpawnMonstersDayOfWeek == null)
            {
                result = true;
            }
            else if (this.ConfigBirthType != 7)
            {
                result = true;
            }
            else
            {
                DayOfWeek nDayOfWeek = TimeUtil.NowDateTime().DayOfWeek;
                for (int i = 0; i < this.SpawnMonstersDayOfWeek.Count; i++)
                {
                    int nDay = this.SpawnMonstersDayOfWeek[i].BirthDayOfWeek;
                    if (nDay == (int)nDayOfWeek)
                    {
                        return true;
                    }
                }
                result = false;
            }
            return result;
        }

        // Token: 0x06003074 RID: 12404 RVA: 0x002B0A2C File Offset: 0x002AEC2C
        public void SystemKillAllMonstersOfThisZone()
        {
            for (int i = 0; i < this.MonsterList.Count; i++)
            {
                if (null != this.MonsterList[i])
                {
                    if (this.MonsterList[i].Alive)
                    {
                        Global.SystemKillMonster(this.MonsterList[i]);
                    }
                }
            }
        }

        // Token: 0x06003075 RID: 12405 RVA: 0x002B0A9B File Offset: 0x002AEC9B
        private void RepositionMonster(Monster monster, int toX, int toY)
        {
            GameManager.MapGridMgr.DictGrids[this.MapCode].MoveObject(-1, -1, toX, toY, monster);
        }

        // Token: 0x06003076 RID: 12406 RVA: 0x002B0AC0 File Offset: 0x002AECC0
        private void MonsterRealive(SocketListener sl, TCPOutPacketPool pool, int copyMapID = -1, int birthCount = 65535)
        {
            SceneUIClasses sceneType = Global.GetMapSceneType(this.MapCode);
            if (SceneUIClasses.ThemeMoYu != sceneType || MoYuLongXue.InActivityTime())
            {
                int haveBirthCount = 0;
                for (int i = 0; i < this.MonsterList.Count; i++)
                {
                    if (null != this.MonsterList[i])
                    {
                        if (haveBirthCount >= birthCount)
                        {
                            break;
                        }
                        if (-1 != copyMapID)
                        {
                            if (this.MonsterList[i].CopyMapID != copyMapID)
                            {
                                goto IL_48F;
                            }
                        }
                        if (!this.MonsterList[i].Alive)
                        {
                            if ((this.BirthType == 0 || 2 == this.BirthType) && this.Timeslot > 0)
                            {
                                long monsterRealiveTimeslot = (long)this.Timeslot * 1000L * 10000L;
                                if (TimeUtil.NOW() * 10000L - this.MonsterList[i].LastDeadTicks < monsterRealiveTimeslot)
                                {
                                    goto IL_48F;
                                }
                                if (this.BirthType == 0 && this.BirthTimePointList != null && this.BirthTimePointList.Count == 2)
                                {
                                    DateTime now = TimeUtil.NowDateTime();
                                    int nowtime = now.Hour * 60 + now.Minute;
                                    int begintm = this.BirthTimePointList[0].BirthHour * 60 + this.BirthTimePointList[0].BirthMinute;
                                    int endtm = this.BirthTimePointList[1].BirthHour * 60 + this.BirthTimePointList[1].BirthMinute;
                                    if (nowtime < begintm || endtm < nowtime)
                                    {
                                        goto IL_48F;
                                    }
                                }
                            }
                            if (this.CanRealiveByRate())
                            {
                                Point pt = this.MonsterList[i].Realive();
                                this.RepositionMonster(this.MonsterList[i], (int)pt.X, (int)pt.Y);
                                List<object> listObjs = Global.GetAll9Clients(this.MonsterList[i]);
                                GameManager.ClientMgr.NotifyMonsterRealive(sl, pool, this.MonsterList[i], this.MapCode, this.MonsterList[i].CopyMapID, this.MonsterList[i].RoleID, (int)this.MonsterList[i].Coordinate.X, (int)this.MonsterList[i].Coordinate.Y, (int)this.MonsterList[i].Direction, listObjs);
                                haveBirthCount++;
                                if (401 == this.MonsterList[i].MonsterType)
                                {
                                    if (this.BirthType == 0 && this.Timeslot >= 1800)
                                    {
                                        GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, StringUtil.substitute(GLang.GetLang(503, new object[0]), new object[]
                                        {
                                            this.MonsterList[i].MonsterInfo.VSName,
                                            Global.GetMapName(this.MonsterList[i].CurrentMapCode)
                                        }), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0, 0, 0, 100, 100);
                                    }
                                    if (SceneUIClasses.ThemeMoYu == sceneType)
                                    {
                                        MoYuLongXue.ProcessAddMonster(this.MonsterList[i]);
                                    }
                                }
                                if (Global.IsGongGaoReliveMonster(this.MonsterList[i].MonsterInfo.ExtensionID))
                                {
                                    GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, StringUtil.substitute(GLang.GetLang(504, new object[0]), new object[]
                                    {
                                        Global.GetMapName(this.MonsterList[i].CurrentMapCode),
                                        this.MonsterList[i].MonsterInfo.VSName
                                    }), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0, 0, 0, 100, 100);
                                }
                                if (this.BirthType == 1 || this.BirthType == 7)
                                {
                                    TimerBossManager.getInstance().AddBoss(this.BirthType, this.MonsterList[i].MonsterInfo.ExtensionID);
                                }
                            }
                        }
                    }
                    IL_48F:;
                }
            }
        }

        // Token: 0x06003077 RID: 12407 RVA: 0x002B0F78 File Offset: 0x002AF178
        public void LoadCopyMapMonsters(int copyMapID)
        {
            if (this.IsFuBenMap)
            {
                if (null != this.SeedMonster)
                {
                    for (int i = 0; i < this.TotalNum; i++)
                    {
                        Monster monster = this.CopyMonster(this.SeedMonster);
                        int roleID = (int)GameManager.MonsterIDMgr.GetNewID(this.MapCode);
                        monster.RoleID = roleID;
                        monster.UniqueID = Global.GetUniqueID();
                        monster.Name = string.Format("Role_{0}", roleID);
                        monster.CopyMapID = copyMapID;
                        monster.FirstCoordinate = Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MapCode, this.ToX, this.ToY, this.Radius, 0, true);
                        if (Global.InOnlyObsByXY(ObjectTypes.OT_MONSTER, monster.CurrentMapCode, (int)monster.FirstCoordinate.X, (int)monster.FirstCoordinate.Y))
                        {
                            Debug.WriteLine("abc");
                        }
                        monster.Direction = (double)Global.GetRandomNumber(0, 8);
                        this.MonsterList.Add(monster);
                        GameManager.MonsterMgr.AddMonster(monster);
                        monster.Start();
                        monster.Coordinate = monster.FirstCoordinate;
                    }
                }
            }
        }

        // Token: 0x06003078 RID: 12408 RVA: 0x002B10C0 File Offset: 0x002AF2C0
        public void ReloadCopyMapMonsters(SocketListener sl, TCPOutPacketPool pool, int copyMapID)
        {
            if (this.IsFuBenMap)
            {
                if (2 == this.BirthType)
                {
                    this.MonsterRealive(sl, pool, copyMapID, 65535);
                }
            }
        }

        // Token: 0x06003079 RID: 12409 RVA: 0x002B1114 File Offset: 0x002AF314
        public void ClearCopyMapMonsters(int copyMapID)
        {
            if (this.IsFuBenMap)
            {
                List<Monster> monsterList = new List<Monster>();
                bool bExistNull = false;
                for (int i = 0; i < this.MonsterList.Count; i++)
                {
                    if (null == this.MonsterList[i])
                    {
                        bExistNull = true;
                    }
                    else if (this.MonsterList[i].CopyMapID == copyMapID)
                    {
                        monsterList.Add(this.MonsterList[i]);
                    }
                }
                for (int i = 0; i < monsterList.Count; i++)
                {
                    this.DestroyMonster(monsterList[i]);
                }
                if (bExistNull)
                {
                    this.MonsterList.RemoveAll((Monster x) => null == x);
                }
            }
        }

        // Token: 0x0600307A RID: 12410 RVA: 0x002B1214 File Offset: 0x002AF414
        public void DestroyDeadMonsters(bool onlyFuBen = true)
        {
            if (this.IsFuBenMap || !onlyFuBen)
            {
                if (this.BirthType != 2)
                {
                    long ticks = TimeUtil.NOW() * 10000L;
                    long monsterDestroyTimeslot = 300000000L;
                    if (ticks - this.LastDestroyTicks >= monsterDestroyTimeslot)
                    {
                        this.LastDestroyTicks = ticks;
                        List<Monster> monsterList = new List<Monster>();
                        bool bExistNull = false;
                        for (int i = 0; i < this.MonsterList.Count; i++)
                        {
                            if (null == this.MonsterList[i])
                            {
                                bExistNull = true;
                            }
                            else if (!this.MonsterList[i].Alive)
                            {
                                monsterList.Add(this.MonsterList[i]);
                            }
                        }
                        for (int i = 0; i < monsterList.Count; i++)
                        {
                            this.DestroyMonster(monsterList[i]);
                        }
                        if (bExistNull)
                        {
                            this.MonsterList.RemoveAll((Monster x) => null == x);
                            LogManager.WriteLog(LogTypes.Error, string.Format("DestroyDeadMonsters MonsterList Exist Null!!!", new object[0]), null, true);
                        }
                    }
                }
            }
        }

        // Token: 0x0600307B RID: 12411 RVA: 0x002B1380 File Offset: 0x002AF580
        public void DestroyDeadDynamicMonsters()
        {
            if (this.IsDynamicZone())
            {
                this.DestroyDeadMonsters(false);
            }
        }

        // Token: 0x0600307C RID: 12412 RVA: 0x002B13A8 File Offset: 0x002AF5A8
        public bool IsDynamicZone()
        {
            return 3 == this.BirthType;
        }

        // Token: 0x0600307D RID: 12413 RVA: 0x002B13C4 File Offset: 0x002AF5C4
        public Monster LoadDynamicRobot(MonsterZoneQueueItem monsterZoneQueueItem)
        {
            Monster monster = monsterZoneQueueItem.seedMonster;
            monster.Tag = monsterZoneQueueItem.Tag;
            monster.ManagerType = monsterZoneQueueItem.ManagerType;
            monster.CopyMapID = monsterZoneQueueItem.CopyMapID;
            monster.MonsterZoneNode = this;
            monster.CoordinateChanged += this.UpdateMonsterEvent;
            monster.MoveToComplete += new MoveToEventHandler(this.MoveToComplete);
            monster.DynamicMonster = true;
            monster.DynamicPursuitRadius = monsterZoneQueueItem.PursuitRadius;
            monster.FirstCoordinate = Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MapCode, monsterZoneQueueItem.ToX, monsterZoneQueueItem.ToY, monsterZoneQueueItem.Radius, 0, true);
            monster.Direction = (double)Global.GetRandomNumber(0, 8);
            this.MonsterList.Add(monster);
            GameManager.MonsterMgr.AddMonster(monster);
            monster.Start();
            monster.Coordinate = monster.FirstCoordinate;
            JingJiChangManager.getInstance().onRobotBron(monster as Robot);
            return monster;
        }

        // Token: 0x0600307E RID: 12414 RVA: 0x002B14B8 File Offset: 0x002AF6B8
        public void LoadDynamicMonsters(MonsterZoneQueueItem monsterZoneQueueItem)
        {
            if (this.IsDynamicZone())
            {
                if (monsterZoneQueueItem != null && null != monsterZoneQueueItem.seedMonster)
                {
                    for (int i = 0; i < monsterZoneQueueItem.BirthCount; i++)
                    {
                        Monster monster = this.CopyMonster(monsterZoneQueueItem.seedMonster);
                        if (monster.OwnerClient != null)
                        {
                            monster.OwnerClient.ClientData.SummonMonstersList.Add(monster);
                        }
                        if (monster.OwnerMonster != null)
                        {
                            monster.OwnerMonster.CallMonster = monster;
                        }
                        int roleID = (int)GameManager.MonsterIDMgr.GetNewID(this.MapCode);
                        monster.RoleID = roleID;
                        monster.UniqueID = Global.GetUniqueID();
                        monster.Tag = monsterZoneQueueItem.Tag;
                        monster.ManagerType = monsterZoneQueueItem.ManagerType;
                        monster.Name = string.Format("Role_{0}", roleID);
                        monster.CopyMapID = monsterZoneQueueItem.CopyMapID;
                        monster.MonsterZoneNode = this;
                        monster.CoordinateChanged += this.UpdateMonsterEvent;
                        monster.MoveToComplete += new MoveToEventHandler(this.MoveToComplete);
                        monster.DynamicMonster = true;
                        monster.DynamicPursuitRadius = monsterZoneQueueItem.PursuitRadius;
                        monster.Flags.Copy(monsterZoneQueueItem.Flags);
                        if (monster.OwnerClient != null)
                        {
                            monster.FirstCoordinate = new Point((double)monster.OwnerClient.ClientData.PosX, (double)monster.OwnerClient.ClientData.PosY);
                        }
                        if (monster.OwnerMonster != null)
                        {
                            monster.FirstCoordinate = new Point(monster.OwnerMonster.CurrentPos.X, monster.OwnerMonster.CurrentPos.Y);
                        }
                        else if (monster.ManagerType == SceneUIClasses.EMoLaiXiCopy)
                        {
                            monster.FirstCoordinate = new Point((double)monsterZoneQueueItem.ToX, (double)monsterZoneQueueItem.ToY);
                            monster.Step = 0;
                            monster.PatrolPath = (monster.Tag as List<int[]>);
                        }
                        else if (monster.MonsterType == 1501)
                        {
                            Point pos = new Point((double)monsterZoneQueueItem.ToX, (double)monsterZoneQueueItem.ToY);
                            monster.FirstCoordinate = pos;
                            monster.Step = 0;
                            monster.PatrolPath = Data.Goldcopyscenedata.m_MonsterPatorlPathList;
                        }
                        else if (monster.MonsterType == 1502)
                        {
                            monster.FirstCoordinate = Global.GridToPixel(this.MapCode, new Point((double)monsterZoneQueueItem.ToX, (double)monsterZoneQueueItem.ToY));
                            monster.Step = 0;
                        }
                        else if (monster.MonsterType == 2001)
                        {
                            if (Global.InOnlyObs(ObjectTypes.OT_MONSTER, this.MapCode, monsterZoneQueueItem.ToX, monsterZoneQueueItem.ToY))
                            {
                                monster.FirstCoordinate = Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MapCode, monsterZoneQueueItem.ToX, monsterZoneQueueItem.ToY, monsterZoneQueueItem.Radius, 0, true);
                            }
                            else
                            {
                                monster.FirstCoordinate = Global.GridToPixel(this.MapCode, new Point((double)monsterZoneQueueItem.ToX, (double)monsterZoneQueueItem.ToY));
                            }
                        }
                        else
                        {
                            monster.FirstCoordinate = Global.GetMapPointByGridXY(ObjectTypes.OT_MONSTER, this.MapCode, monsterZoneQueueItem.ToX, monsterZoneQueueItem.ToY, monsterZoneQueueItem.Radius, 0, true);
                        }
                        monster.Direction = (double)Global.GetRandomNumber(0, 8);
                        if (monsterZoneQueueItem.MyMonsterZone.MapCode == GameManager.AngelTempleMgr.m_AngelTempleData.MapCode)
                        {
                            GameManager.AngelTempleMgr.OnLoadDynamicMonsters(monster);
                        }
                        else if (monsterZoneQueueItem.MyMonsterZone.MapCode == SingletonTemplate<MoRiJudgeManager>.Instance().MapCode)
                        {
                            SingletonTemplate<MoRiJudgeManager>.Instance().OnLoadDynamicMonsters(monster);
                        }
                        else if (LingDiCaiJiManager.getInstance().GetLingDiType(monsterZoneQueueItem.MyMonsterZone.MapCode) != 2)
                        {
                            LingDiCaiJiManager.getInstance().OnLoadDynamicMonsters(monsterZoneQueueItem.MyMonsterZone.MapCode, monster);
                        }
                        if (Global.IsStoryCopyMapScene(monsterZoneQueueItem.MyMonsterZone.MapCode))
                        {
                            CopyMap mapInfo = GameManager.CopyMapMgr.FindCopyMap(monster.CopyMapID);
                            if (mapInfo == null)
                            {
                                break;
                            }
                            SystemXmlItem systemFuBenItem = null;
                            if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(mapInfo.FubenMapID, out systemFuBenItem) && systemFuBenItem != null)
                            {
                                int nBossID = systemFuBenItem.GetIntValue("BossID", -1);
                                if (nBossID == monster.MonsterInfo.ExtensionID)
                                {
                                    Global.NotifyClientStoryCopyMapInfo(monster.CopyMapID, 2);
                                }
                            }
                        }
                        this.MonsterList.Add(monster);
                        GameManager.MonsterMgr.AddMonster(monster);
                        GlobalEventSource4Scene.getInstance().fireEvent(new OnCreateMonsterEventObject(monster), (int)monster.ManagerType);
                        monster.Start();
                        monster.Coordinate = monster.FirstCoordinate;
                        if (1001 == monster.MonsterType)
                        {
                            GameManager.ClientMgr.NotifyOthersMyDeco(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, 160, 2, -1, (int)monster.Coordinate.X, (int)monster.Coordinate.Y, 0, -1, -1, 0, 0, null);
                        }
                    }
                }
            }
        }

        // Token: 0x0600307F RID: 12415 RVA: 0x002B1A58 File Offset: 0x002AFC58
        public string GetMonstersInfoString()
        {
            int aliveCount = 0;
            int deadCount = 0;
            foreach (Monster monster in this.MonsterList)
            {
                if (null != monster)
                {
                    if (monster.Alive)
                    {
                        aliveCount++;
                    }
                    else
                    {
                        deadCount++;
                    }
                }
            }
            string result;
            if (aliveCount + deadCount > 0)
            {
                result = string.Format("地图{0}, {1}副本, aliveCount={2}, deadCount={3}, total={4}", new object[]
                {
                    this.MapCode,
                    this.IsFuBenMap ? "是" : "不是",
                    aliveCount,
                    deadCount,
                    aliveCount + deadCount
                });
            }
            else
            {
                result = "";
            }
            return result;
        }

        // Token: 0x06003080 RID: 12416 RVA: 0x002B1B58 File Offset: 0x002AFD58
        public void ReloadNormalMapMonsters(SocketListener sl, TCPOutPacketPool pool, int birthCount)
        {
            if (!this.IsFuBenMap)
            {
                if (2 == this.BirthType)
                {
                    this.MonsterRealive(sl, pool, -1, birthCount);
                }
            }
        }

        // Token: 0x06003081 RID: 12417 RVA: 0x002B1B91 File Offset: 0x002AFD91
        public void OnReallyDied(Monster deadMonster)
        {
        }

        // Token: 0x06003082 RID: 12418 RVA: 0x002B1B94 File Offset: 0x002AFD94
        private void InitMonsterStaticInfo(XElement monsterXml, double maxLifeV, double maxMagicV, XElement xmlFrameConfig, double moveSpeed, int[] speedTickList)
        {
            double[] extProps = ConfigParser.ParserExtPropsFromAttrubite(monsterXml, MonsterZone.MonsterExtPropsConfigList);
            this.SetStaticInfo4Monster(Global.GetSafeAttributeStr(monsterXml, "SName"), (int)Global.GetSafeAttributeLong(monsterXml, "ID"), maxLifeV, maxMagicV, (int)Global.GetSafeAttributeLong(monsterXml, "Level"), (int)Global.GetSafeAttributeLong(monsterXml, "Experience"), 0, Global.StringArray2IntArray(Global.GetSafeAttributeStr(xmlFrameConfig, "EachActionFrameRange").Split(new char[]
            {
                ','
            })), Global.StringArray2IntArray(Global.GetSafeAttributeStr(xmlFrameConfig, "EachActionEffectiveFrame").Split(new char[]
            {
                ','
            })), (int)Global.GetSafeAttributeLong(monsterXml, "AttackRange"), (int)Global.GetSafeAttributeLong(monsterXml, "SeedRange"), (int)Global.GetSafeAttributeLong(monsterXml, "Code"), -1, speedTickList, 0, 0, (int)Global.GetSafeAttributeLong(monsterXml, "MinAttackPercent"), (int)Global.GetSafeAttributeLong(monsterXml, "MaxAttackPercent"), (int)Global.GetSafeAttributeLong(monsterXml, "DefensePercent"), (int)Global.GetSafeAttributeLong(monsterXml, "MDefensePercent"), Global.GetSafeAttributeDouble(monsterXml, "HitV"), Global.GetSafeAttributeDouble(monsterXml, "Dodge"), Global.GetSafeAttributeDouble(monsterXml, "RecoverLifeV"), Global.GetSafeAttributeDouble(monsterXml, "RecoverMagicV"), Global.GetSafeAttributeDouble(monsterXml, "DamageThornPercent"), Global.GetSafeAttributeDouble(monsterXml, "DamageThorn"), Global.GetSafeAttributeDouble(monsterXml, "SubAttackInjurePercent"), Global.GetSafeAttributeDouble(monsterXml, "SubAttackInjure"), Global.GetSafeAttributeDouble(monsterXml, "IgnoreDefensePercent"), Global.GetSafeAttributeDouble(monsterXml, "IgnoreDefenseRate"), Global.GetSafeAttributeDouble(monsterXml, "Lucky"), Global.GetSafeAttributeDouble(monsterXml, "FatalAttack"), Global.GetSafeAttributeDouble(monsterXml, "DoubleAttack"), (int)Global.GetSafeAttributeLong(monsterXml, "FallID"), (int)Global.GetSafeAttributeLong(monsterXml, "MonsterType"), (int)Global.GetSafeAttributeLong(monsterXml, "PersonalJiFen"), (int)Global.GetSafeAttributeLong(monsterXml, "CampJiFen"), (int)Global.GetSafeAttributeLong(monsterXml, "EMoJiFen"), (int)Global.GetSafeAttributeLong(monsterXml, "XueSeJiFen"), (int)Global.GetSafeAttributeLong(monsterXml, "Belong"), Global.String2IntArray(Global.GetSafeAttributeStr(monsterXml, "SkillIDs"), ','), (int)Global.GetSafeAttributeLong(monsterXml, "AttackType"), (int)Global.GetSafeAttributeLong(monsterXml, "Camp"), (int)Global.GetSafeAttributeLong(monsterXml, "AIID"), (int)Global.GetSafeAttributeLong(monsterXml, "ZhuanSheng"), (int)Global.GetSafeAttributeLong(monsterXml, "LangHunJiFen"), (int)Global.GetSafeAttributeLong(monsterXml, "RebornExp"), extProps);
        }

        // Token: 0x06003083 RID: 12419 RVA: 0x002B1DD4 File Offset: 0x002AFFD4
        private void SetStaticInfo4Monster(string sname, int extensionID, double life, double mana, int level, int experience, int money, int[] frameRange, int[] effectiveFrame, int attackRange, int seekRange, int equipmentBody, int equipmentWeapon, int[] speedTickList, int toOccupation, int toRoleLevel, int minAttack, int maxAttack, int defense, int magicDefense, double hitV, double dodge, double recoverLifeV, double recoverMagicV, double DamageThornPercent, double DamageThorn, double SubAttackInjurePercent, double SubAttackInjure, double IgnoreDefensePercent, double IgnoreDefenseRate, double Lucky, double FatalAttack, double DoubleAttack, int fallGoodsPackID, int monsterType, int battlePersonalJiFen, int battleZhenYingJiFen, int nDaimonSquareJiFen, int nBloodCastJiFen, int fallBelongTo, int[] skillIDs, int attackType, int camp, int AIID, int nChangeLifeCount, int nWolfScore, int rebornExp, double[] extProps)
        {
            this.MonsterInfo.SpriteSpeedTickList = speedTickList;
            this.MonsterInfo.VSName = sname;
            this.MonsterInfo.ExtensionID = extensionID;
            this.MonsterInfo.VLifeMax = life;
            this.MonsterInfo.VManaMax = mana;
            this.MonsterInfo.VLevel = level;
            this.MonsterInfo.VExperience = experience;
            this.MonsterInfo.VMoney = money;
            this.MonsterInfo.EachActionFrameRange = frameRange;
            this.MonsterInfo.EffectiveFrame = effectiveFrame;
            this.MonsterInfo.SeekRange = seekRange;
            this.MonsterInfo.EquipmentBody = equipmentBody;
            this.MonsterInfo.EquipmentWeapon = equipmentWeapon;
            this.MonsterInfo.ToOccupation = toOccupation;
            this.MonsterInfo.MinAttack = minAttack;
            this.MonsterInfo.MaxAttack = maxAttack;
            this.MonsterInfo.Defense = defense;
            this.MonsterInfo.MDefense = magicDefense;
            this.MonsterInfo.HitV = hitV;
            this.MonsterInfo.Dodge = dodge;
            this.MonsterInfo.RecoverLifeV = recoverLifeV;
            this.MonsterInfo.RecoverMagicV = recoverMagicV;
            this.MonsterInfo.MonsterDamageThornPercent = DamageThornPercent;
            this.MonsterInfo.MonsterDamageThorn = DamageThorn;
            this.MonsterInfo.MonsterSubAttackInjurePercent = SubAttackInjurePercent;
            this.MonsterInfo.MonsterSubAttackInjure = SubAttackInjure;
            this.MonsterInfo.MonsterIgnoreDefensePercent = IgnoreDefensePercent;
            this.MonsterInfo.MonsterIgnoreDefenseRate = IgnoreDefenseRate;
            this.MonsterInfo.MonsterLucky = Lucky;
            this.MonsterInfo.MonsterFatalAttack = FatalAttack;
            this.MonsterInfo.MonsterDoubleAttack = DoubleAttack;
            this.MonsterInfo.FallGoodsPackID = fallGoodsPackID;
            this.MonsterInfo.BattlePersonalJiFen = Global.GMax(0, battlePersonalJiFen);
            this.MonsterInfo.BattleZhenYingJiFen = Global.GMax(0, battleZhenYingJiFen);
            this.MonsterInfo.DaimonSquareJiFen = Global.GMax(0, nDaimonSquareJiFen);
            this.MonsterInfo.BloodCastJiFen = Global.GMax(0, nBloodCastJiFen);
            this.MonsterInfo.WolfScore = Global.GMax(0, nWolfScore);
            this.MonsterInfo.FallBelongTo = Global.GMax(0, fallBelongTo);
            this.MonsterInfo.SkillIDs = skillIDs;
            this.MonsterInfo.AttackType = attackType;
            this.MonsterInfo.Camp = camp;
            this.MonsterInfo.AIID = AIID;
            this.MonsterInfo.ChangeLifeCount = ((nChangeLifeCount < 0) ? 0 : nChangeLifeCount);
            this.MonsterInfo.RebornExp = rebornExp;
            this.MonsterInfo.ExtProps = extProps;
            MonsterStaticInfoMgr.SetInfo(extensionID, this.MonsterInfo);
        }

        // Token: 0x04003D21 RID: 15649
        private bool HasSystemKilledAllOfThisZone = false;

        // Token: 0x04003D22 RID: 15650
        public bool IsFuBenMap = false;

        // Token: 0x04003D23 RID: 15651
        public MonsterTypes MonsterType = MonsterTypes.None;

        // Token: 0x04003D24 RID: 15652
        private long LastReloadTicks = 0L;

        // Token: 0x04003D25 RID: 15653
        private long LastDestroyTicks = 0L;

        // Token: 0x04003D26 RID: 15654
        private int LastBirthDayID = -1;

        // Token: 0x04003D27 RID: 15655
        private BirthTimePoint LastBirthTimePoint = null;

        // Token: 0x04003D28 RID: 15656
        private int LastBirthTimePointIndex = -1;

        // Token: 0x04003D29 RID: 15657
        private DateTime LastReloadMonstersDateTime = DateTime.MaxValue;

        // Token: 0x04003D2A RID: 15658
        private MonsterStaticInfo MonsterInfo = new MonsterStaticInfo();

        // Token: 0x04003D2B RID: 15659
        private List<Monster> MonsterList = new List<Monster>(100);

        // Token: 0x04003D2C RID: 15660
        private Monster SeedMonster = null;

        // Token: 0x04003D2D RID: 15661
        private static List<KeyValuePair<int, string>> MonsterExtPropsConfigList = new List<KeyValuePair<int, string>>
        {
            new KeyValuePair<int, string>(24, "SubAttackInjurePercent"),
            new KeyValuePair<int, string>(25, "SubAttackInjure"),
            new KeyValuePair<int, string>(122, "HolyAttack"),
            new KeyValuePair<int, string>(123, "HolyDefense"),
            new KeyValuePair<int, string>(126, "HolyWeakPercent"),
            new KeyValuePair<int, string>(129, "ShadowAttack"),
            new KeyValuePair<int, string>(130, "ShadowDefense"),
            new KeyValuePair<int, string>(133, "ShadowWeakPercent"),
            new KeyValuePair<int, string>(136, "NatureAttack"),
            new KeyValuePair<int, string>(137, "NatureDefense"),
            new KeyValuePair<int, string>(140, "NatureWeakPercent"),
            new KeyValuePair<int, string>(143, "ChaosAttack"),
            new KeyValuePair<int, string>(144, "ChaosDefense"),
            new KeyValuePair<int, string>(147, "ChaosWeakPercent"),
            new KeyValuePair<int, string>(150, "IncubusAttack"),
            new KeyValuePair<int, string>(151, "IncubusDefense"),
            new KeyValuePair<int, string>(154, "IncubusWeakPercent")
        };
    }
}
