using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
    
    public class YaoSaiBossManager : IManager, ICmdProcessorEx, ICmdProcessor
    {
        
        public static YaoSaiBossManager getInstance()
        {
            return YaoSaiBossManager.instance;
        }

        
        public bool initialize()
        {
            this.LoadConfig();
            return true;
        }

        
        public bool startup()
        {
            TCPCmdDispatcher.getInstance().registerProcessorEx(1851, 2, 2, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1852, 2, 2, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1853, 1, 1, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1854, 2, 2, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1855, 3, 3, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1857, 2, 2, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1858, 1, 1, YaoSaiBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            return true;
        }

        
        public bool showdown()
        {
            return true;
        }

        
        public bool destroy()
        {
            return true;
        }

        
        public bool processCmd(GameClient client, string[] cmdParams)
        {
            return false;
        }

        
        public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            bool result;
            if (client.ClientSocket.IsKuaFuLogin)
            {
                result = true;
            }
            else
            {
                switch (nID)
                {
                    case 1851:
                        return this.ProcessGetBossMiniInfoCmd(client, nID, bytes, cmdParams);
                    case 1852:
                        return this.ProcessZhaoHuanBossCmd(client, nID, bytes, cmdParams);
                    case 1853:
                        return this.ProcessTaoFaBossCmd(client, nID, bytes, cmdParams);
                    case 1854:
                        return this.ProcessGetBossFightInFoCmd(client, nID, bytes, cmdParams);
                    case 1855:
                        return this.ProcessBossFightExcuteCmd(client, nID, bytes, cmdParams);
                    case 1857:
                        return this.ProcessGetBossFightLogCmd(client, nID, bytes, cmdParams);
                    case 1858:
                        return this.ProcessGiveBossKillAwardCmd(client, nID, bytes, cmdParams);
                }
                result = true;
            }
            return result;
        }

        
        public bool ProcessGetBossMiniInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
                {
                    return false;
                }
                int roleID = Global.SafeConvertToInt32(cmdParams[0]);
                int otherRoleId = Global.SafeConvertToInt32(cmdParams[1]);
                YaoSaiBossData bossData = this.GetRoleBossData(otherRoleId);
                int zhanDouCount = this.GetZhanDouCount(roleID, otherRoleId);
                YaoSaiBossMainData data = new YaoSaiBossMainData
                {
                    BossInfo = bossData,
                    HaveZhaoHuanCount = Global.GetRoleParamsInt32FromDB(client, "10176"),
                    TaoFaCount = Global.GetRoleParamsInt32FromDB(client, "10179"),
                    ZhaoHuanBossID = Global.GetRoleParamsInt32FromDB(client, "10177"),
                    OtherID = otherRoleId,
                    FightCount = zhanDouCount
                };
                client.sendCmd<YaoSaiBossMainData>(nID, data, false);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
            }
            return false;
        }

        
        public bool ProcessZhaoHuanBossCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
                {
                    return false;
                }
                int roleID = Global.SafeConvertToInt32(cmdParams[0]);
                int type = Global.SafeConvertToInt32(cmdParams[1]);
                int zhaoHuanType = 0;
                DateTime now = TimeUtil.NowDateTime();
                int result = 0;
                int bossID = 0;
                int zhaoHuanCount = Global.GetRoleParamsInt32FromDB(client, "10176");
                if (type < 0 || type > 3)
                {
                    result = 12;
                }
                else
                {
                    if (type > 1)
                    {
                        zhaoHuanType = type;
                    }
                    else
                    {
                        zhaoHuanType = ((zhaoHuanCount >= YaoSaiBossManager.PuTongZhaoHuanCount) ? 1 : 0);
                    }
                    YaoSaiBossData bossData = this.GetRoleBossData(roleID);
                    if (null != bossData)
                    {
                        if (bossData.LifeV == 0.0 || bossData.DeadTime < now)
                        {
                            result = 2;
                        }
                        else
                        {
                            result = 3;
                        }
                    }
                    else
                    {
                        int taoFaCount = Global.GetRoleParamsInt32FromDB(client, "10179");
                        if (taoFaCount >= YaoSaiBossManager.TaoFaCount)
                        {
                            result = 11;
                        }
                        else
                        {
                            int random = Global.GetRandomNumber(1, 100000);
                            foreach (KeyValuePair<int, PetBossItem> item in this.PetBossXmlDict)
                            {
                                switch (zhaoHuanType)
                                {
                                    case 0:
                                        if (random >= item.Value.FreeStartValue && random <= item.Value.FreeEndValue)
                                        {
                                            bossID = item.Key;
                                        }
                                        break;
                                    case 1:
                                        if (random >= item.Value.ZuanShiStartValue && random <= item.Value.ZuanShiEndValue)
                                        {
                                            bossID = item.Key;
                                        }
                                        break;
                                    case 2:
                                    case 3:
                                        if (item.Value.Star == 5 && random <= item.Value.FreeEndValue)
                                        {
                                            bossID = item.Key;
                                        }
                                        break;
                                }
                                if (bossID > 0)
                                {
                                    break;
                                }
                            }
                            if (bossID < 1)
                            {
                                result = 6;
                            }
                            else
                            {
                                switch (zhaoHuanType)
                                {
                                    case 0:
                                        if (zhaoHuanCount >= YaoSaiBossManager.PuTongZhaoHuanCount)
                                        {
                                            result = 4;
                                        }
                                        else
                                        {
                                            zhaoHuanCount++;
                                            Global.SaveRoleParamsInt32ValueToDB(client, "10176", zhaoHuanCount, true);
                                        }
                                        break;
                                    case 1:
                                        if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, YaoSaiBossManager.ZuanShiZhaoHuanCost, "要塞Boss召唤", true, true, false, DaiBiSySType.JingLingYaoSaiPuTongZhaoHuan))
                                        {
                                            result = 5;
                                        }
                                        break;
                                    case 2:
                                        {
                                            bool usedBinding;
                                            bool usedTimeLimited;
                                            if (Global.UseGoodsBindOrNot(client, YaoSaiBossManager.FiveStartNeedGoods, YaoSaiBossManager.FiveStartNeedNums, true, out usedBinding, out usedTimeLimited) < 1)
                                            {
                                                result = 13;
                                            }
                                            break;
                                        }
                                    case 3:
                                        {
                                            bool usedBinding;
                                            bool usedTimeLimited;
                                            if (Global.UseGoodsBindOrNot(client, YaoSaiBossManager.FiveStartNeedGoods, YaoSaiBossManager.FiveStartNeedNums, true, out usedBinding, out usedTimeLimited) < 1)
                                            {
                                                if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, YaoSaiBossManager.FiveStartNeedZuan, "要塞Boss五星召唤", true, true, false, DaiBiSySType.JingLingYaoSaiZhaoHuanBoss))
                                                {
                                                    result = 5;
                                                }
                                            }
                                            break;
                                        }
                                }
                            }
                        }
                    }
                }
                if (result == 0)
                {
                    Global.SaveRoleParamsInt32ValueToDB(client, "10177", bossID, true);
                }
                string data = string.Concat(new object[]
                {
                    result,
                    ":",
                    bossID,
                    ":",
                    zhaoHuanCount
                });
                client.sendCmd(nID, data, false);
                GameManager.logDBCmdMgr.AddDBLogInfo(-1, "要塞boss召唤", "类型=" + zhaoHuanType, client.ClientData.RoleName, "系统", "修改", -1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
            }
            return false;
        }

        
        public bool ProcessTaoFaBossCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
                {
                    return false;
                }
                int roleID = Global.SafeConvertToInt32(cmdParams[0]);
                int bossID = Global.GetRoleParamsInt32FromDB(client, "10177");
                int taoFaCount = Global.GetRoleParamsInt32FromDB(client, "10179");
                DateTime now = TimeUtil.NowDateTime();
                int result = 0;
                PetBossItem bossItem = null;
                double life = 0.0;
                YaoSaiBossData bossData = this.GetRoleBossData(roleID);
                if (null != bossData)
                {
                    if (bossData.LifeV == 0.0 || bossData.DeadTime < now)
                    {
                        result = 2;
                    }
                    else
                    {
                        result = 3;
                    }
                }
                else if (taoFaCount >= YaoSaiBossManager.TaoFaCount)
                {
                    result = 11;
                }
                else if (!this.PetBossXmlDict.TryGetValue(bossID, out bossItem))
                {
                    result = 6;
                }
                else
                {
                    Monster bossMonster = GameManager.MonsterZoneMgr.GetMonsterByMonsterID(bossItem.MonsterID);
                    if (null == bossMonster)
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 讨伐boss失败 不存在配置的boss MonsterID bossID={0}", bossID), null, true);
                        result = 6;
                    }
                    else
                    {
                        life = bossMonster.VLife;
                    }
                }
                if (result != 0)
                {
                    client.sendCmd<int>(nID, result, false);
                    return true;
                }
                Global.SaveRoleParamsInt32ValueToDB(client, "10177", 0, true);
                bossData = new YaoSaiBossData
                {
                    BossID = bossID,
                    LifeV = life,
                    DeadTime = now.AddSeconds((double)bossItem.Time),
                    OwnerID = roleID
                };
                client.sendCmd<int>(nID, result, false);
                Global.SaveRoleParamsInt32ValueToDB(client, "10179", taoFaCount + 1, true);
                this.SaveAndBroadcastUpdateYaoSaiBoss(client.ClientData.RoleID, bossData, true);
                GameManager.logDBCmdMgr.AddDBLogInfo(-1, "要塞boss讨伐", "bossID=" + bossID, client.ClientData.RoleName, "系统", "修改", -1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
            }
            return false;
        }

        
        public bool ProcessGetBossFightInFoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
                {
                    return false;
                }
                int roleID = Global.SafeConvertToInt32(cmdParams[0]);
                int otherID = Global.SafeConvertToInt32(cmdParams[1]);
                YaoSaiBossFightData data = this.GetBossFightData(client, otherID);
                client.sendCmd<YaoSaiBossFightData>(nID, data, false);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
            }
            return false;
        }

        
        public bool ProcessBossFightExcuteCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
                {
                    return false;
                }
                int roleID = Global.SafeConvertToInt32(cmdParams[0]);
                int otherID = Global.SafeConvertToInt32(cmdParams[1]);
                YaoSaiBossFightData data = this.GetBossFightData(client, otherID);
                DateTime now = TimeUtil.NowDateTime();
                double injure = 0.0;
                bool needNotifyAward = true;
                int result;
                if (data.BossMiniInfo == null || data.BossMiniInfo.LifeV <= 0.0 || now >= data.BossMiniInfo.DeadTime)
                {
                    result = 7;
                }
                else if (!this.CanFight(client, cmdParams[2]))
                {
                    result = 8;
                }
                else if (roleID != otherID && data.HaveFightTime >= YaoSaiBossManager.XieZhuFightCount)
                {
                    result = 9;
                }
                else if (data.ZuanShiFightCost > client.ClientData.UserMoney)
                {
                    result = 5;
                }
                else
                {
                    injure = this.GetInjure(client, data.BossMiniInfo.BossID, cmdParams[2]);
                    if (data.BossMiniInfo.LifeV < injure)
                    {
                        injure = data.BossMiniInfo.LifeV;
                    }
                    result = this.ExcuteInjure(client, data.BossMiniInfo, injure);
                    if (result == 0)
                    {
                        if (data.ZuanShiFightCost > 0)
                        {
                            GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, data.ZuanShiFightCost, "要塞boss战斗消耗_" + data.BossMiniInfo.BossID, true, true, false, DaiBiSySType.None);
                        }
                        this.SaveAndBroadcastUpdateYaoSaiBoss(client.ClientData.RoleID, data.BossMiniInfo, true);
                        data.HaveFightTime++;
                        Global.SaveRoleParamsStringToDB(client, "37", cmdParams[2], true);
                        if (roleID != otherID)
                        {
                            int lingJiangCount = Global.GetRoleParamsInt32FromDB(client, "10178");
                            if (lingJiangCount >= YaoSaiBossManager.XieZhuAwardCount)
                            {
                                needNotifyAward = false;
                                goto IL_244;
                            }
                            lingJiangCount++;
                            Global.SaveRoleParamsInt32ValueToDB(client, "10178", lingJiangCount, true);
                        }
                        this.GiveFightAward(client, otherID, injure);
                    }
                }
                IL_244:
                YaoSaiBossFightResultData resultData = new YaoSaiBossFightResultData
                {
                    Result = result,
                    FightLife = Convert.ToInt32(injure),
                    BossInfo = this.GetRoleBossData(otherID),
                    NeedNotifyAward = needNotifyAward
                };
                client.sendCmd<YaoSaiBossFightResultData>(nID, resultData, false);
                if (result == 0)
                {
                    string invite = (roleID == otherID) ? "自己" : "协助";
                    string dead = (data.BossMiniInfo.LifeV > 0.0) ? "存活" : "击杀";
                    GameManager.logDBCmdMgr.AddDBLogInfo(-1, "要塞boss战斗", "bossID=" + data.BossMiniInfo.BossID, client.ClientData.RoleName, invite, dead, Convert.ToInt32(injure), client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
            }
            return false;
        }

        
        public bool ProcessGetBossFightLogCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
                {
                    return false;
                }
                int roleID = Global.SafeConvertToInt32(cmdParams[0]);
                int otherID = Global.SafeConvertToInt32(cmdParams[1]);
                List<YaoSaiBossFightLog> fightLogList = this.GetFightLogList(roleID, otherID);
                if (fightLogList != null && fightLogList.Count > 0)
                {
                    fightLogList.Sort((YaoSaiBossFightLog x, YaoSaiBossFightLog y) => y.FightLife - x.FightLife);
                    fightLogList = fightLogList.Take(25).ToList<YaoSaiBossFightLog>();
                }
                YaoSaiBossFightLogInfo data = new YaoSaiBossFightLogInfo
                {
                    OtherRid = otherID,
                    BossFightLogList = fightLogList
                };
                client.sendCmd<YaoSaiBossFightLogInfo>(nID, data, false);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取主页面boss信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
            }
            return false;
        }

        
        public bool ProcessGiveBossKillAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
                {
                    return false;
                }
                int roleID = Global.SafeConvertToInt32(cmdParams[0]);
                int result = 0;
                DateTime now = TimeUtil.NowDateTime();
                YaoSaiBossData bossData = null;
                PetBossItem bossItem = null;
                bossData = this.GetRoleBossData(roleID);
                if (null == bossData)
                {
                    result = 7;
                }
                else if (bossData.LifeV != 0.0 && bossData.DeadTime > now)
                {
                    result = 3;
                }
                else if (!this.PetBossXmlDict.TryGetValue(bossData.BossID, out bossItem))
                {
                    result = 6;
                }
                else
                {
                    double lifeV = GameManager.MonsterZoneMgr.GetMonsterByMonsterID(bossItem.MonsterID).VLife;
                    double injure = lifeV - bossData.LifeV;
                    if (injure * 10.0 < lifeV)
                    {
                        injure = lifeV / 10.0;
                    }
                    string[] goodList = bossItem.KillAward.Split(new char[]
                    {
                        '|'
                    });
                    string[] goodList2 = bossItem.KillExtraAward.Split(new char[]
                    {
                        '|'
                    });
                    int multy = 0;
                    JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
                    if (null != activity)
                    {
                        JieRiMultConfig config = activity.GetConfig(14);
                        if (null != config)
                        {
                            multy += (int)config.GetMult();
                        }
                    }
                    SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
                    if (null != spAct)
                    {
                        multy += (int)spAct.GetMult(SpecPActivityBuffType.SPABT_YaoSaiBoss);
                    }
                    multy = Math.Max(1, multy);
                    List<GoodsData> awardList = new List<GoodsData>();
                    for (int i = 0; i < goodList.Length; i++)
                    {
                        if (!(goodList[i] == ""))
                        {
                            string[] goodItem = goodList[i].Split(new char[]
                            {
                                ','
                            });
                            if (goodItem.Length == 7)
                            {
                                GoodsData goodsData = new GoodsData
                                {
                                    Id = -1,
                                    GoodsID = Convert.ToInt32(goodItem[0]),
                                    Using = 0,
                                    Forge_level = Convert.ToInt32(goodItem[3]),
                                    Starttime = "1900-01-01 12:00:00",
                                    Endtime = "1900-01-01 12:00:00",
                                    Site = 0,
                                    GCount = Convert.ToInt32(Math.Floor((double)Convert.ToInt32(goodItem[1]) * injure / lifeV)) * multy,
                                    Binding = Convert.ToInt32(goodItem[2]),
                                    BagIndex = 0,
                                    Lucky = Convert.ToInt32(goodItem[5]),
                                    ExcellenceInfo = Convert.ToInt32(goodItem[6]),
                                    AppendPropLev = Convert.ToInt32(goodItem[4])
                                };
                                SystemXmlItem systemGoods = null;
                                if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
                                {
                                    string strinfo = string.Format("系统中不存在{0}", goodsData.GoodsID);
                                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
                                }
                                else
                                {
                                    awardList.Add(goodsData);
                                }
                            }
                        }
                    }
                    if (bossData.LifeV <= 0.0)
                    {
                        for (int i = 0; i < goodList2.Length; i++)
                        {
                            if (!(goodList2[i] == ""))
                            {
                                string[] goodItem = goodList2[i].Split(new char[]
                                {
                                    ','
                                });
                                if (goodItem.Length == 7)
                                {
                                    GoodsData goodsData = new GoodsData
                                    {
                                        Id = -1,
                                        GoodsID = Convert.ToInt32(goodItem[0]),
                                        Using = 0,
                                        Forge_level = Convert.ToInt32(goodItem[3]),
                                        Starttime = "1900-01-01 12:00:00",
                                        Endtime = "1900-01-01 12:00:00",
                                        Site = 0,
                                        GCount = Convert.ToInt32(Math.Floor((double)Convert.ToInt32(goodItem[1]) * injure / lifeV)) * multy,
                                        Binding = Convert.ToInt32(goodItem[2]),
                                        BagIndex = 0,
                                        Lucky = Convert.ToInt32(goodItem[5]),
                                        ExcellenceInfo = Convert.ToInt32(goodItem[6]),
                                        AppendPropLev = Convert.ToInt32(goodItem[4])
                                    };
                                    SystemXmlItem systemGoods = null;
                                    if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
                                    {
                                        string strinfo = string.Format("系统中不存在{0}", goodsData.GoodsID);
                                        GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
                                    }
                                    else
                                    {
                                        awardList.Add(goodsData);
                                    }
                                }
                            }
                        }
                    }
                    int count = 0;
                    foreach (GoodsData item in awardList)
                    {
                        if (item.GCount > 0)
                        {
                            count += (int)Math.Ceiling((double)item.GCount / (double)Global.GetGoodsGridNumByID(item.GoodsID));
                        }
                    }
                    if (!Global.CanAddGoodsNum(client, count))
                    {
                        result = 10;
                    }
                    else
                    {
                        foreach (GoodsData item in awardList)
                        {
                            if (item.GCount >= 1)
                            {
                                SystemXmlItem systemXmlGood = null;
                                GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemXmlGood);
                                string goodsName = systemXmlGood.GetStringValue("Title");
                                LogManager.WriteLog(LogTypes.SQL, string.Format("要塞boss击杀奖励{0} {1}", client.ClientData.RoleID, goodsName), null, true);
                                Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GCount, item.Quality, "", item.Forge_level, item.Binding, item.Site, "", true, 1, "要塞boss击杀奖励", "1900-01-01 12:00:00", 0, 0, item.Lucky, 0, item.ExcellenceInfo, item.AppendPropLev, 0, null, null, 0, true);
                            }
                        }
                        lock (this.RunTimeData.Mutex)
                        {
                            this.RunTimeData.RoleBossCacheDict.Remove(roleID);
                            this.RunTimeData.BossZhanDouLogDict.Remove(roleID);
                        }
                        GameManager.DBCmdMgr.AddDBCmd(20310, string.Format("{0}", bossData.OwnerID), null, client.ServerId);
                    }
                }
                client.sendCmd<int>(nID, result, false);
                if (result == 0)
                {
                    GameManager.logDBCmdMgr.AddDBLogInfo(-1, "要塞boss击杀结果=" + bossData.LifeV, string.Concat(new object[]
                    {
                        "角色ID=",
                        bossData.OwnerID,
                        "bossID=",
                        bossData.BossID
                    }), client.ClientData.RoleName, "系统", "修改", -1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 领取击杀boss奖励信息错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
            }
            return false;
        }

        
        public YaoSaiBossData GetRoleBossData(int rid)
        {
            YaoSaiBossData bossData = null;
            YaoSaiBossData result;
            try
            {
                lock (this.RunTimeData.Mutex)
                {
                    this.RunTimeData.RoleBossCacheDict.TryGetValue(rid, out bossData);
                }
                result = bossData;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取角色boss信息错误 roleid={0} ex:{1}", rid, ex.Message), null, true);
                result = null;
            }
            return result;
        }

        
        public int GetZhanDouCount(int rid, int otherid)
        {
            try
            {
                YaoSaiBossData bossData = this.GetRoleBossData(otherid);
                if (null == bossData)
                {
                    return 0;
                }
                lock (this.RunTimeData.Mutex)
                {
                    Dictionary<int, List<YaoSaiBossFightLog>> fightLogDic = null;
                    if (!this.RunTimeData.BossZhanDouLogDict.TryGetValue(otherid, out fightLogDic))
                    {
                        return 0;
                    }
                    List<YaoSaiBossFightLog> fightLogList = null;
                    if (!fightLogDic.TryGetValue(rid, out fightLogList))
                    {
                        return 0;
                    }
                    return fightLogList.Count;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取boss战斗次数出错 rid={0},otherid={2},ex={3}", rid, otherid, ex.Message), null, true);
            }
            return 0;
        }

        
        public int ExcuteInjure(GameClient client, YaoSaiBossData bossData, double injure)
        {
            try
            {
                if (bossData.LifeV < injure)
                {
                    injure = bossData.LifeV;
                }
                bossData.LifeV -= injure;
                int inviteType = 0;
                if (client.ClientData.RoleID != bossData.OwnerID)
                {
                    RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, bossData.OwnerID), 0);
                    if (dbRd.Faction > 0)
                    {
                        List<BangHuiMemberData> BHList = Global.sendToDB<List<BangHuiMemberData>, string>(299, string.Format("{0}:{1}", bossData.OwnerID, dbRd.Faction), 0);
                        bool flag;
                        if (BHList != null)
                        {
                            flag = (BHList.Find((BangHuiMemberData x) => x.RoleID == client.ClientData.RoleID) == null);
                        }
                        else
                        {
                            flag = true;
                        }
                        if (!flag)
                        {
                            inviteType = 2;
                            goto IL_105;
                        }
                    }
                    inviteType = 1;
                }
                IL_105:
                lock (this.RunTimeData.Mutex)
                {
                    Dictionary<int, List<YaoSaiBossFightLog>> fightLogDic = null;
                    List<YaoSaiBossFightLog> fightLogList = null;
                    if (!this.RunTimeData.BossZhanDouLogDict.TryGetValue(bossData.OwnerID, out fightLogDic))
                    {
                        fightLogDic = new Dictionary<int, List<YaoSaiBossFightLog>>();
                        this.RunTimeData.BossZhanDouLogDict[bossData.OwnerID] = fightLogDic;
                    }
                    if (!fightLogDic.TryGetValue(client.ClientData.RoleID, out fightLogList))
                    {
                        fightLogList = new List<YaoSaiBossFightLog>();
                        fightLogDic[client.ClientData.RoleID] = fightLogList;
                    }
                    fightLogList.Add(new YaoSaiBossFightLog
                    {
                        OtherRid = client.ClientData.RoleID,
                        OtherRname = client.ClientData.RoleName,
                        InviteType = inviteType,
                        FightLife = Convert.ToInt32(injure)
                    });
                }
                GameManager.DBCmdMgr.AddDBCmd(20309, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
                {
                    bossData.OwnerID,
                    client.ClientData.RoleID,
                    client.ClientData.RoleName,
                    inviteType,
                    injure
                }), null, client.ServerId);
                return 0;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 执行boss战斗信息出错 rid={0},ex={3}", client.ClientData.RoleID, ex.Message), null, true);
            }
            return -1;
        }

        
        public YaoSaiBossFightData GetBossFightData(GameClient client, int otherID)
        {
            try
            {
                int roleID = client.ClientData.RoleID;
                int zhanDouCount = this.GetZhanDouCount(roleID, otherID);
                int zuanShiCost = 0;
                if (roleID == otherID)
                {
                    int index = zhanDouCount - YaoSaiBossManager.ZhaoHuanFightCount;
                    if (index >= 0)
                    {
                        if (index >= YaoSaiBossManager.EWaiFightCost.Count)
                        {
                            index = YaoSaiBossManager.EWaiFightCost.Count - 1;
                        }
                        zuanShiCost = YaoSaiBossManager.EWaiFightCost[index];
                    }
                }
                string jingLingZhenRong = Global.GetRoleParamByName(client, "37");
                if (string.IsNullOrEmpty(jingLingZhenRong))
                {
                    jingLingZhenRong = "0|0|0";
                }
                var realJingLing = jingLingZhenRong.Split(new char[] { '|' });
                int i;
                for (i = 0; i < realJingLing.Length; i++)
                {
                    if (null == client.ClientData.PaiZhuDamonGoodsDataList.Find((GoodsData x) => x.Site == 10000 && x.Id == Convert.ToInt32(realJingLing[i])))
                    {
                        realJingLing[i] = "0";
                    }
                    jingLingZhenRong = string.Join("|", realJingLing);
                }
                return new YaoSaiBossFightData
                {
                    BossMiniInfo = this.GetRoleBossData(otherID),
                    JingLingZhenRong = jingLingZhenRong,
                    HaveFightTime = zhanDouCount,
                    ZuanShiFightCost = zuanShiCost
                };
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取boss战斗信息出错 rid={0},otherid={2},ex={3}", client.ClientData.RoleID, otherID, ex.Message), null, true);
            }
            return null;
        }

        
        public List<YaoSaiBossFightLog> GetFightLogList(int rid, int otherid)
        {
            List<YaoSaiBossFightLog> fightLogList = new List<YaoSaiBossFightLog>();
            try
            {
                lock (this.RunTimeData.Mutex)
                {
                    Dictionary<int, List<YaoSaiBossFightLog>> fightLogDict = null;
                    if (!this.RunTimeData.BossZhanDouLogDict.TryGetValue(otherid, out fightLogDict))
                    {
                        return fightLogList;
                    }
                    foreach (KeyValuePair<int, List<YaoSaiBossFightLog>> pair in fightLogDict)
                    {
                        fightLogList.AddRange(pair.Value);
                    }
                }
                return fightLogList;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取战斗记录列表失败。rid={0},otherid={1},ex={2}", rid, otherid, ex.Message), null, true);
            }
            return fightLogList;
        }

        
        public bool CanFight(GameClient client, string jingLing)
        {
            bool ret = false;
            bool result;
            if (string.IsNullOrEmpty(jingLing))
            {
                result = false;
            }
            else
            {
                string[] jingLingArray = jingLing.Split(new char[]
                {
                    '|'
                });
                if (jingLingArray.Length != 3)
                {
                    result = false;
                }
                else
                {
                    List<string> tmp = new List<string>();
                    string[] array = jingLingArray;
                    for (int i = 0; i < array.Length; i++)
                    {
                        string item = array[i];
                        if (!(item == "0"))
                        {
                            if (tmp.Contains(item))
                            {
                                return false;
                            }
                            tmp.Add(item);
                            if (null == client.ClientData.PaiZhuDamonGoodsDataList.Find((GoodsData x) => x.Site == 10000 && x.Id == Convert.ToInt32(item)))
                            {
                                return false;
                            }
                            ret = true;
                        }
                    }
                    result = ret;
                }
            }
            return result;
        }

        
        public double GetInjure(GameClient client, int bossID, string jingLing)
        {
            string[] zhenRong = jingLing.Split(new char[]
            {
                '|'
            });
            List<GoodsData> zhenRongList = new List<GoodsData>();
            string[] array = zhenRong;
            for (int j = 0; j < array.Length; j++)
            {
                string item = array[j];
                if (!(item == "0"))
                {
                    GoodsData one = client.ClientData.PaiZhuDamonGoodsDataList.Find((GoodsData x) => x.Id == Convert.ToInt32(item));
                    zhenRongList.Add(one);
                }
            }
            PetBossItem bossItem = null;
            double result;
            if (!this.PetBossXmlDict.TryGetValue(bossID, out bossItem))
            {
                result = 0.0;
            }
            else
            {
                int minInjure = 0;
                int maxInjure = 0;
                foreach (GoodsData item2 in zhenRongList)
                {
                    int levelBeiLv = 1 + (1 + item2.Forge_level) / bossItem.PetLevelStep;
                    int levelStartVal = levelBeiLv * bossItem.PetLevelStepNum[0];
                    int levelEndVal = levelBeiLv * bossItem.PetLevelStepNum[1];
                    int exceBeiLv = 1 + Global.GetEquipExcellencePropNum(item2) / bossItem.ExcellentStep;
                    int exceStartVal = exceBeiLv * bossItem.ExcellentStepNum[0];
                    int exceEndVal = exceBeiLv * bossItem.ExcellentStepNum[1];
                    minInjure += levelStartVal + exceStartVal;
                    maxInjure += levelEndVal + exceEndVal;
                }
                double injure = (double)Global.GetRandomNumber(minInjure, maxInjure + 1);
                int zhenRongRate = 1;
                bool suitRate = true;
                for (int i = 0; i < bossItem.PetSuit.Count; i++)
                {
                    int index;
                    for (index = 0; index < zhenRongList.Count; index++)
                    {
                        if (bossItem.PetSuit[i] == zhenRongList[index].GoodsID)
                        {
                            break;
                        }
                    }
                    if (index < zhenRongList.Count)
                    {
                        zhenRongList.RemoveAt(index);
                        zhenRongRate += bossItem.PetRate[i];
                    }
                    else
                    {
                        suitRate = false;
                    }
                }
                if (suitRate)
                {
                    zhenRongRate += bossItem.SuitRate;
                }
                injure *= (double)zhenRongRate;
                result = injure;
            }
            return result;
        }

        
        public void OnLogin(GameClient client, bool isNewDay)
        {
            if (isNewDay)
            {
                Global.SaveRoleParamsInt32ValueToDB(client, "10176", 0, true);
                Global.SaveRoleParamsInt32ValueToDB(client, "10178", 0, true);
                Global.SaveRoleParamsInt32ValueToDB(client, "10179", 0, true);
            }
        }

        
        public void SaveAndBroadcastUpdateYaoSaiBoss(int roleID, YaoSaiBossData bossData, bool writeDB = true)
        {
            try
            {
                GameClient client = GameManager.ClientMgr.FindClient(roleID);
                if (writeDB)
                {
                    lock (this.RunTimeData.Mutex)
                    {
                        this.RunTimeData.RoleBossCacheDict[bossData.OwnerID] = bossData;
                    }
                }
                GameClient owner = GameManager.ClientMgr.FindClient(bossData.OwnerID);
                if (null != owner)
                {
                    owner.sendCmd<YaoSaiBossData>(1856, bossData, false);
                }
                if (writeDB)
                {
                    GameManager.DBCmdMgr.AddDBCmd(20307, string.Format("{0}:{1}:{2}:{3}", new object[]
                    {
                        bossData.OwnerID,
                        bossData.BossID,
                        bossData.LifeV,
                        bossData.DeadTime.ToString().Replace(':', '$')
                    }), null, GameCoreInterface.getinstance().GetLocalServerId());
                }
                List<FriendData> friendsList = Global.sendToDB<List<FriendData>, string>(142, string.Format("{0}", bossData.OwnerID), GameCoreInterface.getinstance().GetLocalServerId());
                foreach (FriendData friend in friendsList)
                {
                    if (friend.OnlineState > 0 && friend.FriendType == 0)
                    {
                        GameClient gc = GameManager.ClientMgr.FindClient(friend.OtherRoleID);
                        if (null != gc)
                        {
                            if (Global.GetRoleParamsInt64FromDB(gc, "10184") != 0L)
                            {
                                gc.sendCmd<YaoSaiBossData>(1856, bossData, false);
                            }
                        }
                    }
                }
                RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, bossData.OwnerID), 0);
                if (dbRd.Faction > 0)
                {
                    List<BangHuiMemberData> BHList = Global.sendToDB<List<BangHuiMemberData>, string>(299, string.Format("{0}:{1}", bossData.OwnerID, dbRd.Faction), 0);
                    foreach (BangHuiMemberData bhMember in BHList)
                    {
                        if (bhMember.OnlineState > 0)
                        {
                            GameClient gc = GameManager.ClientMgr.FindClient(bhMember.RoleID);
                            if (null != gc)
                            {
                                if (Global.GetRoleParamsInt64FromDB(gc, "10184") != 0L)
                                {
                                    gc.sendCmd<YaoSaiBossData>(1856, bossData, false);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 广播更新boss信息错误。ex:{0}", ex.Message), null, true);
            }
        }

        
        public void GiveFightAward(GameClient client, int otherid, double injure)
        {
            try
            {
                YaoSaiBossData bossData = this.GetRoleBossData(otherid);
                PetBossItem bossItem = null;
                if (this.PetBossXmlDict.TryGetValue(bossData.BossID, out bossItem))
                {
                    int multy = 0;
                    JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
                    if (null != activity)
                    {
                        JieRiMultConfig config = activity.GetConfig(14);
                        if (null != config)
                        {
                            multy += (int)config.GetMult();
                        }
                    }
                    SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
                    if (null != spAct)
                    {
                        multy += (int)spAct.GetMult(SpecPActivityBuffType.SPABT_YaoSaiBoss);
                    }
                    multy = Math.Max(1, multy);
                    string[] goodList = bossItem.FightAward.Split(new char[]
                    {
                        '|'
                    });
                    List<GoodsData> awardList = new List<GoodsData>();
                    for (int i = 0; i < goodList.Length; i++)
                    {
                        if (!(goodList[i] == ""))
                        {
                            string[] goodItem = goodList[i].Split(new char[]
                            {
                                ','
                            });
                            if (goodItem.Length == 7)
                            {
                                GoodsData goodsData = new GoodsData
                                {
                                    Id = -1,
                                    GoodsID = Convert.ToInt32(goodItem[0]),
                                    Using = 0,
                                    Forge_level = Convert.ToInt32(goodItem[3]),
                                    Starttime = "1900-01-01 12:00:00",
                                    Endtime = "1900-01-01 12:00:00",
                                    Site = 0,
                                    GCount = Convert.ToInt32(Convert.ToInt32(goodItem[1])) * multy,
                                    Binding = Convert.ToInt32(goodItem[2]),
                                    BagIndex = 0,
                                    Lucky = Convert.ToInt32(goodItem[5]),
                                    ExcellenceInfo = Convert.ToInt32(goodItem[6]),
                                    AppendPropLev = Convert.ToInt32(goodItem[4])
                                };
                                SystemXmlItem systemGoods = null;
                                if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
                                {
                                    string strinfo = string.Format("系统中不存在{0}", goodsData.GoodsID);
                                    GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
                                }
                                else
                                {
                                    awardList.Add(goodsData);
                                }
                            }
                        }
                    }
                    int count = 0;
                    foreach (GoodsData item in awardList)
                    {
                        if (item.GCount > 0)
                        {
                            count += (int)Math.Ceiling((double)item.GCount / (double)Global.GetGoodsGridNumByID(item.GoodsID));
                        }
                    }
                    if (!Global.CanAddGoodsNum(client, count))
                    {
                        Global.UseMailGivePlayerAward2(client, awardList, GLang.GetLang(2622, new object[0]), GLang.GetLang(2622, new object[0]), 0, 0, 0);
                    }
                    else
                    {
                        foreach (GoodsData item in awardList)
                        {
                            SystemXmlItem systemXmlGood = null;
                            GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemXmlGood);
                            string goodsName = systemXmlGood.GetStringValue("Title");
                            LogManager.WriteLog(LogTypes.SQL, string.Format("要塞Boss战斗奖励{0} {1}", client.ClientData.RoleID, goodsName), null, true);
                            Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GCount, item.Quality, "", item.Forge_level, item.Binding, item.Site, "", true, 1, "要塞Boss战斗奖励", "1900-01-01 12:00:00", 0, 0, item.Lucky, 0, item.ExcellenceInfo, item.AppendPropLev, 0, null, null, 0, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 发放战斗奖励失败。ex:{0}", ex.Message), null, true);
            }
        }

        
        public int GetRoleBossState(int rid, int otherid)
        {
            try
            {
                DateTime now = TimeUtil.NowDateTime();
                if (Global.SafeConvertToInt64(Global.GetRoleParamsFromDBByRoleID(rid, "10184", 0)) == 0L)
                {
                    return 0;
                }
                lock (this.RunTimeData.Mutex)
                {
                    YaoSaiBossData bossData = null;
                    if (!this.RunTimeData.RoleBossCacheDict.TryGetValue(rid, out bossData))
                    {
                        return 1;
                    }
                    if (bossData.LifeV <= 0.0 || now > bossData.DeadTime)
                    {
                        return 1;
                    }
                    Dictionary<int, List<YaoSaiBossFightLog>> fightLog = null;
                    if (!this.RunTimeData.BossZhanDouLogDict.TryGetValue(rid, out fightLog))
                    {
                        return 2;
                    }
                    List<YaoSaiBossFightLog> fightLogList = null;
                    if (!fightLog.TryGetValue(otherid, out fightLogList))
                    {
                        return 2;
                    }
                    return (fightLogList.Count < YaoSaiBossManager.XieZhuFightCount) ? 2 : 3;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取显示boss状态错误。rid={0},otherid={1},ex={2}", rid, otherid, ex.Message), null, true);
            }
            return 0;
        }

        
        public void LoadRunTimeDataFromDB()
        {
            try
            {
                DateTime now = TimeUtil.NowDateTime();
                Dictionary<int, YaoSaiBossData> roleBossData = Global.sendToDB<Dictionary<int, YaoSaiBossData>, int>(20306, 0, GameCoreInterface.getinstance().GetLocalServerId());
                lock (this.RunTimeData.Mutex)
                {
                    if (null != roleBossData)
                    {
                        this.RunTimeData.RoleBossCacheDict = roleBossData;
                    }
                }
                Dictionary<int, List<YaoSaiBossFightLog>> bossFightLogDict = Global.sendToDB<Dictionary<int, List<YaoSaiBossFightLog>>, int>(20308, 0, GameCoreInterface.getinstance().GetLocalServerId());
                Dictionary<int, Dictionary<int, List<YaoSaiBossFightLog>>> bossZhanDouLogDict = new Dictionary<int, Dictionary<int, List<YaoSaiBossFightLog>>>();
                foreach (KeyValuePair<int, List<YaoSaiBossFightLog>> item in bossFightLogDict)
                {
                    Dictionary<int, List<YaoSaiBossFightLog>> bossZhaoDouItemLog = new Dictionary<int, List<YaoSaiBossFightLog>>();
                    foreach (YaoSaiBossFightLog fightitem in item.Value)
                    {
                        List<YaoSaiBossFightLog> fightList = null;
                        if (!bossZhaoDouItemLog.TryGetValue(fightitem.OtherRid, out fightList))
                        {
                            fightList = new List<YaoSaiBossFightLog>();
                            bossZhaoDouItemLog[fightitem.OtherRid] = fightList;
                        }
                        fightList.Add(fightitem);
                    }
                    bossZhanDouLogDict[item.Key] = bossZhaoDouItemLog;
                }
                lock (this.RunTimeData.Mutex)
                {
                    this.RunTimeData.BossZhanDouLogDict = bossZhanDouLogDict;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 获取数据库数据失败。ex:{0}", ex.Message), null, true);
            }
        }

        
        public void LoadConfig()
        {
            this.LoadSystemParams();
            this.LoadPetBossXml();
            this.LoadRunTimeDataFromDB();
        }

        
        public void LoadSystemParams()
        {
            try
            {
                string[] ManorBossCall = GameManager.systemParamsList.GetParamValueByName("ManorBossCall").Split(new char[]
                {
                    ','
                });
                if (ManorBossCall.Length < 3)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 配置表配置出错 ManorBossCall", new object[0]), null, true);
                }
                else
                {
                    YaoSaiBossManager.PuTongZhaoHuanCount = Global.SafeConvertToInt32(ManorBossCall[0]);
                    YaoSaiBossManager.ZuanShiZhaoHuanCost = Global.SafeConvertToInt32(ManorBossCall[1]);
                    YaoSaiBossManager.TaoFaCount = Global.SafeConvertToInt32(ManorBossCall[2]);
                    string[] ManorBossFight = GameManager.systemParamsList.GetParamValueByName("ManorBossFight").Split(new char[]
                    {
                        ','
                    });
                    if (ManorBossFight.Length < 4)
                    {
                        LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 配置表配置出错 ManorBossCall", new object[0]), null, true);
                    }
                    else
                    {
                        YaoSaiBossManager.ZhaoHuanFightCount = Global.SafeConvertToInt32(ManorBossFight[0]);
                        YaoSaiBossManager.XieZhuFightCount = Global.SafeConvertToInt32(ManorBossFight[1]);
                        YaoSaiBossManager.XieZhuAwardCount = Global.SafeConvertToInt32(ManorBossFight[2]);
                        for (int i = 3; i < ManorBossFight.Length; i++)
                        {
                            YaoSaiBossManager.EWaiFightCost.Add(Global.SafeConvertToInt32(ManorBossFight[i]));
                        }
                        string[] ManorSuperBoss = GameManager.systemParamsList.GetParamValueByName("ManorSuperBoss").Split(new char[]
                        {
                            ',',
                            '|'
                        });
                        if (ManorSuperBoss.Length < 3)
                        {
                            LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 配置表配置出错 ManorSuperBoss", new object[0]), null, true);
                        }
                        else
                        {
                            YaoSaiBossManager.FiveStartNeedGoods = Global.SafeConvertToInt32(ManorSuperBoss[0]);
                            YaoSaiBossManager.FiveStartNeedNums = Global.SafeConvertToInt32(ManorSuperBoss[1]);
                            YaoSaiBossManager.FiveStartNeedZuan = Global.SafeConvertToInt32(ManorSuperBoss[2]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", "SystemParms.xml", ex.Message), ex, true);
            }
        }

        
        public void LoadPetBossXml()
        {
            string fileName = "";
            try
            {
                fileName = Global.GameResPath("Config\\PetBoss.xml");
                XElement xml = CheckHelper.LoadXml(fileName, true);
                if (null != xml)
                {
                    IEnumerable<XElement> nodes = xml.Elements();
                    this.PetBossXmlDict.Clear();
                    foreach (XElement xmlItem in nodes)
                    {
                        int id = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
                        string[] freeRate = Global.GetDefAttributeStr(xmlItem, "FreeRate", "").Split(new char[]
                        {
                            ','
                        });
                        string[] zuanShiRate = Global.GetDefAttributeStr(xmlItem, "ZuanRate", "").Split(new char[]
                        {
                            ','
                        });
                        if (freeRate.Length < 2)
                        {
                            freeRate = new string[]
                            {
                                "0",
                                "0"
                            };
                        }
                        if (zuanShiRate.Length < 2)
                        {
                            zuanShiRate = new string[]
                            {
                                "0",
                                "0"
                            };
                        }
                        string[] petLevelStepNumXml = Global.GetDefAttributeStr(xmlItem, "PetLevelStepNum", "").Split(new char[]
                        {
                            ','
                        });
                        int[] petLevelStepNum;
                        if (petLevelStepNumXml.Length != 2)
                        {
                            int[] array = new int[2];
                            petLevelStepNum = array;
                        }
                        else
                        {
                            petLevelStepNum = new int[]
                            {
                                Global.SafeConvertToInt32(petLevelStepNumXml[0]),
                                Global.SafeConvertToInt32(petLevelStepNumXml[1])
                            };
                        }
                        string[] excellentStepNumXml = Global.GetDefAttributeStr(xmlItem, "ExcellentStepNum", "").Split(new char[]
                        {
                            ','
                        });
                        int[] excellentStepNum;
                        if (excellentStepNumXml.Length != 2)
                        {
                            int[] array = new int[2];
                            excellentStepNum = array;
                        }
                        else
                        {
                            excellentStepNum = new int[]
                            {
                                Global.SafeConvertToInt32(excellentStepNumXml[0]),
                                Global.SafeConvertToInt32(excellentStepNumXml[1])
                            };
                        }
                        string[] petSuitXml = Global.GetDefAttributeStr(xmlItem, "PetSuit", "").Split(new char[]
                        {
                            ','
                        });
                        List<int> petSuit = new List<int>();
                        foreach (string item in petSuitXml)
                        {
                            petSuit.Add(Global.SafeConvertToInt32(item));
                        }
                        string[] petRateXml = Global.GetDefAttributeStr(xmlItem, "PetRate", "").Split(new char[]
                        {
                            ','
                        });
                        List<int> petRate = new List<int>();
                        foreach (string item in petRateXml)
                        {
                            petRate.Add(Global.SafeConvertToInt32(item));
                        }
                        for (int i = petSuit.Count; i < petRate.Count; i++)
                        {
                            petRate.Add(1);
                        }
                        this.PetBossXmlDict[id] = new PetBossItem
                        {
                            ID = id,
                            MonsterID = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "MonsterID", "0")),
                            Star = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "Star", "0")),
                            FreeStartValue = Global.SafeConvertToInt32(freeRate[0]),
                            FreeEndValue = Global.SafeConvertToInt32(freeRate[1]),
                            ZuanShiStartValue = Global.SafeConvertToInt32(zuanShiRate[0]),
                            ZuanShiEndValue = Global.SafeConvertToInt32(zuanShiRate[1]),
                            Time = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "Time", "0")),
                            FightAward = Global.GetDefAttributeStr(xmlItem, "FightAward", ""),
                            KillAward = Global.GetDefAttributeStr(xmlItem, "KillAward", ""),
                            KillExtraAward = Global.GetDefAttributeStr(xmlItem, "KillExtraAward", ""),
                            PetLevelStep = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "PetLevelStep", "1")),
                            PetLevelStepNum = petLevelStepNum,
                            ExcellentStep = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "ExcellentStep", "1")),
                            ExcellentStepNum = excellentStepNum,
                            PetSuit = petSuit,
                            PetRate = petRate,
                            SuitRate = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "SuitRate", "1"))
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", fileName, ex.Message), ex, true);
            }
        }

        
        public void YaoSaiBossTimer_Work()
        {
            try
            {
                long nowMs = TimeUtil.NOW();
                lock (this.RunTimeData.Mutex)
                {
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("YaoSaiBoss :: 定时器错误 ：ex:{0}", ex.Message), null, true);
            }
        }

        
        public YaoSaiBossRunTimeData RunTimeData = new YaoSaiBossRunTimeData();

        
        public static int PuTongZhaoHuanCount = 0;

        
        public static int ZuanShiZhaoHuanCost = 0;

        
        public static int TaoFaCount = 0;

        
        public static int ZhaoHuanFightCount = 0;

        
        public static int XieZhuFightCount = 0;

        
        public static int XieZhuAwardCount = 0;

        
        public static int FiveStartNeedGoods = 0;

        
        public static int FiveStartNeedNums = 0;

        
        public static int FiveStartNeedZuan = 0;

        
        public static List<int> EWaiFightCost = new List<int>();

        
        public Dictionary<int, PetBossItem> PetBossXmlDict = new Dictionary<int, PetBossItem>();

        
        private static YaoSaiBossManager instance = new YaoSaiBossManager();
    }
}
