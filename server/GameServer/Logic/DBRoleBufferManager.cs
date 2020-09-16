using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic.NewBufferExt;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
    
    public class DBRoleBufferManager
    {
        
        public static void ProcessLifeVAndMagicVReserve(SocketListener sl, TCPOutPacketPool pool, GameClient client)
        {
            if (client.ClientData.CurrentLifeV > 0)
            {
                RoleRelifeLog relifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "血瓶蓝瓶");
                bool doRelife = false;
                if (client.ClientData.CurrentLifeV < client.ClientData.LifeV)
                {
                    if (Global.CanMapUseBuffer(client.ClientData.MapCode, 4))
                    {
                        BufferData bufferData = Global.GetBufferDataByID(client, 4);
                        if (null != bufferData)
                        {
                            if (bufferData.BufferVal > 0L)
                            {
                                doRelife = true;
                                relifeLog.hpModify = true;
                                relifeLog.oldHp = client.ClientData.CurrentLifeV;
                                int needLifeV = client.ClientData.LifeV - client.ClientData.CurrentLifeV;
                                int lifeRecoverNum = Global.GMax(0, (int)GameManager.systemParamsList.GetParamValueIntByName("LifeRecoverNum", -1));
                                lifeRecoverNum = (int)((double)lifeRecoverNum * (1.0 + RoleAlgorithm.GetLifeRecoverAddPercentV(client)));
                                needLifeV = Global.GMin(needLifeV, lifeRecoverNum);
                                needLifeV = Global.GMin(needLifeV, (int)bufferData.BufferVal);
                                bufferData.BufferVal -= (long)Global.GMin((int)bufferData.BufferVal, lifeRecoverNum);
                                needLifeV += client.ClientData.CurrentLifeV;
                                client.ClientData.CurrentLifeV = Global.GMin(client.ClientData.LifeV, needLifeV);
                                relifeLog.newHp = client.ClientData.CurrentLifeV;
                                GameManager.ClientMgr.NotifyBufferData(client, bufferData);
                            }
                        }
                    }
                }
                if (client.ClientData.CurrentMagicV < client.ClientData.MagicV)
                {
                    if (Global.CanMapUseBuffer(client.ClientData.MapCode, 5))
                    {
                        BufferData bufferData = Global.GetBufferDataByID(client, 5);
                        if (null != bufferData)
                        {
                            if (bufferData.BufferVal > 0L)
                            {
                                doRelife = true;
                                relifeLog.mpModify = true;
                                relifeLog.oldMp = client.ClientData.CurrentMagicV;
                                int needMagicV = client.ClientData.MagicV - client.ClientData.CurrentMagicV;
                                int magicRecoverNum = Global.GMax(0, (int)GameManager.systemParamsList.GetParamValueIntByName("MagicRecoverNum", -1));
                                magicRecoverNum = (int)((double)magicRecoverNum * (1.0 + RoleAlgorithm.GetMagicRecoverAddPercentV(client)));
                                needMagicV = Global.GMin(needMagicV, magicRecoverNum);
                                needMagicV = (int)Global.GMin((long)needMagicV, bufferData.BufferVal);
                                bufferData.BufferVal -= (long)((int)Global.GMin(bufferData.BufferVal, (long)magicRecoverNum));
                                needMagicV += client.ClientData.CurrentMagicV;
                                client.ClientData.CurrentMagicV = Global.GMin(client.ClientData.MagicV, needMagicV);
                                relifeLog.newMp = client.ClientData.CurrentMagicV;
                                GameManager.ClientMgr.NotifyBufferData(client, bufferData);
                            }
                        }
                    }
                }
                SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
                if (doRelife)
                {
                    List<object> listObjs = Global.GetAll9Clients(client);
                    GameManager.ClientMgr.NotifyOthersRelife(sl, pool, client, client.ClientData.MapCode, client.ClientData.CopyMapID, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, (double)client.ClientData.CurrentLifeV, (double)client.ClientData.CurrentMagicV, 120, listObjs, 0);
                }
            }
        }

        
        public static int ProcessHuZhaoSubLifeV(GameClient client, int subLifeV)
        {
            if (client.ClientData.CurrentLifeV > 0)
            {
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 97))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 97);
                    if (null != bufferData)
                    {
                        if (bufferData.BufferVal > 0L)
                        {
                            int needLifeV = client.ClientData.LifeV - client.ClientData.CurrentLifeV;
                            HuZhaoBufferItem huZhaoBufferItem = client.MyBufferExtManager.FindBufferItem(97) as HuZhaoBufferItem;
                            if (huZhaoBufferItem != null)
                            {
                                needLifeV = Global.GMin(needLifeV, huZhaoBufferItem.InjuredV);
                                needLifeV = Global.GMin(needLifeV, (int)bufferData.BufferVal);
                                bufferData.BufferVal -= (long)Global.GMin((int)bufferData.BufferVal, huZhaoBufferItem.InjuredV);
                                subLifeV = Global.GMin(needLifeV, subLifeV);
                                GameManager.ClientMgr.NotifyBufferData(client, bufferData);
                            }
                        }
                        else
                        {
                            Global.RemoveBufferData(client, 97);
                            client.MyBufferExtManager.RemoveBufferItem(97);
                        }
                    }
                }
            }
            return subLifeV;
        }

        
        public static double ProcessHuZhaoRecoverPercent(GameClient client)
        {
            double percent = 0.0;
            if (client.ClientData.CurrentLifeV > 0)
            {
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 97))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 97);
                    if (null != bufferData)
                    {
                        if (bufferData.BufferVal > 0L)
                        {
                            HuZhaoBufferItem huZhaoBufferItem = client.MyBufferExtManager.FindBufferItem(97) as HuZhaoBufferItem;
                            if (huZhaoBufferItem != null)
                            {
                                percent = huZhaoBufferItem.RecoverLifePercent;
                            }
                        }
                        else
                        {
                            Global.RemoveBufferData(client, 97);
                            client.MyBufferExtManager.RemoveBufferItem(97);
                        }
                    }
                }
            }
            return percent;
        }

        
        public static int ProcessWuDiHuZhaoNoInjured(GameClient client, int subLifeV)
        {
            if (client.ClientData.CurrentLifeV > 0)
            {
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 98))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 98);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            subLifeV = 0;
                        }
                        else
                        {
                            Global.RemoveBufferData(client, 98);
                        }
                    }
                }
            }
            return subLifeV;
        }

        
        public static void ProcessTimeAddLifeMagic(GameClient client)
        {
            if (client.ClientData.CurrentLifeV > 0)
            {
                double lifeV = 0.0;
                double magicV = 0.0;
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 27))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 27);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            int actionGoodsID = (int)bufferData.BufferVal;
                            List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(actionGoodsID);
                            if (magicActionItemList != null && magicActionItemList[0].MagicActionID == MagicActionIDs.DB_TIME_LIFE_MAGIC)
                            {
                                if (nowTicks - client.ClientData.StartAddLifeMagicTicks >= (long)((int)(magicActionItemList[0].MagicActionParams[3] * 1000.0)))
                                {
                                    client.ClientData.StartAddLifeMagicTicks = nowTicks;
                                    lifeV = magicActionItemList[0].MagicActionParams[0];
                                    lifeV = (double)((int)(lifeV * (1.0 + RoleAlgorithm.GetLifeRecoverAddPercentV(client))));
                                    magicV = magicActionItemList[0].MagicActionParams[1];
                                    magicV = (double)((int)(magicV * (1.0 + RoleAlgorithm.GetMagicRecoverAddPercentV(client))));
                                }
                            }
                        }
                    }
                }
                RoleRelifeLog relifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "加血加蓝buff" + 27);
                if (lifeV > 0.0)
                {
                    relifeLog.hpModify = true;
                    relifeLog.oldHp = client.ClientData.CurrentLifeV;
                    int needLifeV = client.ClientData.LifeV - client.ClientData.CurrentLifeV;
                    needLifeV = Global.GMin(needLifeV, (int)lifeV);
                    lifeV = (double)needLifeV;
                    needLifeV += client.ClientData.CurrentLifeV;
                    client.ClientData.CurrentLifeV = Global.GMin(client.ClientData.LifeV, needLifeV);
                    relifeLog.newHp = client.ClientData.CurrentLifeV;
                }
                if (magicV > 0.0)
                {
                    relifeLog.mpModify = true;
                    relifeLog.oldMp = client.ClientData.CurrentMagicV;
                    int needMagicV = client.ClientData.MagicV - client.ClientData.CurrentMagicV;
                    needMagicV = Global.GMin(needMagicV, (int)magicV);
                    magicV = (double)needMagicV;
                    needMagicV += client.ClientData.CurrentMagicV;
                    client.ClientData.CurrentMagicV = Global.GMin(client.ClientData.MagicV, needMagicV);
                    relifeLog.newMp = client.ClientData.CurrentMagicV;
                }
                SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
                if (lifeV > 0.0 || magicV > 0.0)
                {
                    List<object> listObjs = Global.GetAll9Clients(client);
                    GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, client.ClientData.CopyMapID, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, (double)client.ClientData.CurrentLifeV, (double)client.ClientData.CurrentMagicV, 120, listObjs, 0);
                }
            }
        }

        
        public static void ProcessTimeAddLifeNoShow(GameClient client)
        {
            if (client.ClientData.CurrentLifeV > 0)
            {
                double lifeV = 0.0;
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 37))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 37);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            int actionGoodsID = (int)bufferData.BufferVal;
                            List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(actionGoodsID);
                            if (magicActionItemList != null && magicActionItemList[0].MagicActionID == MagicActionIDs.DB_TIME_LIFE_NOSHOW)
                            {
                                if (nowTicks - client.ClientData.StartAddLifeNoShowTicks >= (long)((int)(magicActionItemList[0].MagicActionParams[2] * 1000.0)))
                                {
                                    double addrate = 0.0;
                                    client.ClientData.StartAddLifeNoShowTicks = nowTicks;
                                    lifeV = magicActionItemList[0].MagicActionParams[0];
                                    addrate += RoleAlgorithm.GetLifeRecoverAddPercentV(client);
                                    if (1000L == bufferData.BufferVal || 1001L == bufferData.BufferVal || 1002L == bufferData.BufferVal || 1100L == bufferData.BufferVal || 1101L == bufferData.BufferVal || 1102L == bufferData.BufferVal)
                                    {
                                        addrate += client.ClientData.PropsCacheManager.GetExtProp(87);
                                    }
                                    lifeV = (double)((int)(lifeV * (1.0 + addrate)));
                                }
                            }
                        }
                    }
                }
                if (lifeV > 0.0)
                {
                    RoleRelifeLog relifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "加血buff" + 37);
                    relifeLog.hpModify = true;
                    relifeLog.oldHp = client.ClientData.CurrentLifeV;
                    int needLifeV = client.ClientData.LifeV - client.ClientData.CurrentLifeV;
                    needLifeV = Global.GMin(needLifeV, (int)lifeV);
                    lifeV = (double)needLifeV;
                    needLifeV += client.ClientData.CurrentLifeV;
                    client.ClientData.CurrentLifeV = Global.GMin(client.ClientData.LifeV, needLifeV);
                    relifeLog.newHp = client.ClientData.CurrentLifeV;
                    SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
                }
                if (lifeV > 0.0)
                {
                    List<object> listObjs = Global.GetAll9Clients(client);
                    GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, client.ClientData.CopyMapID, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, (double)client.ClientData.CurrentLifeV, (double)client.ClientData.CurrentMagicV, 120, listObjs, 0);
                }
            }
        }

        
        public static void ProcessTimeAddMagicNoShow(GameClient client)
        {
            if (client.ClientData.CurrentLifeV > 0)
            {
                double magicV = 0.0;
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 38))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 38);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            int actionGoodsID = (int)bufferData.BufferVal;
                            List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(actionGoodsID);
                            if (magicActionItemList != null && magicActionItemList[0].MagicActionID == MagicActionIDs.DB_TIME_MAGIC_NOSHOW)
                            {
                                if (nowTicks - client.ClientData.StartAddMaigcNoShowTicks >= (long)((int)(magicActionItemList[0].MagicActionParams[2] * 1000.0)))
                                {
                                    client.ClientData.StartAddMaigcNoShowTicks = nowTicks;
                                    magicV = magicActionItemList[0].MagicActionParams[0];
                                    magicV = (double)((int)(magicV * (1.0 + RoleAlgorithm.GetMagicRecoverAddPercentV(client))));
                                }
                            }
                        }
                    }
                }
                if (magicV > 0.0)
                {
                    RoleRelifeLog relifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "加蓝buff" + 38);
                    relifeLog.mpModify = true;
                    relifeLog.oldMp = client.ClientData.CurrentMagicV;
                    int needMagicV = client.ClientData.MagicV - client.ClientData.CurrentMagicV;
                    needMagicV = Global.GMin(needMagicV, (int)magicV);
                    magicV = (double)needMagicV;
                    needMagicV += client.ClientData.CurrentMagicV;
                    client.ClientData.CurrentMagicV = Global.GMin(client.ClientData.MagicV, needMagicV);
                    relifeLog.newMp = client.ClientData.CurrentMagicV;
                    SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
                }
                if (magicV > 0.0)
                {
                    List<object> listObjs = Global.GetAll9Clients(client);
                    GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, client.ClientData.CopyMapID, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, (double)client.ClientData.CurrentLifeV, (double)client.ClientData.CurrentMagicV, 120, listObjs, 0);
                }
            }
        }

        
        public static void ProcessDSTimeAddLifeNoShow(GameClient client)
        {
            if (client.ClientData.CurrentLifeV > 0)
            {
                double lifeV = 0.0;
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 40))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 40);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            int timeSlotSecs = (int)(bufferData.BufferVal >> 32 & (long)0xffff_ffffUL);
                            int addLiefV = (int)(bufferData.BufferVal & (long)0xffff_ffffUL);
                            if (nowTicks - client.ClientData.DSStartDSAddLifeNoShowTicks >= (long)(timeSlotSecs * 1000))
                            {
                                client.ClientData.DSStartDSAddLifeNoShowTicks = nowTicks;
                                lifeV = (double)addLiefV;
                                lifeV = (double)((int)(lifeV * (1.0 + RoleAlgorithm.GetLifeRecoverAddPercentV(client))));
                            }
                        }
                    }
                }
                if (lifeV > 0.0)
                {
                    RoleRelifeLog relifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "加血buff" + 40);
                    relifeLog.hpModify = true;
                    relifeLog.oldHp = client.ClientData.CurrentLifeV;
                    int needLifeV = client.ClientData.LifeV - client.ClientData.CurrentLifeV;
                    needLifeV = Global.GMin(needLifeV, (int)lifeV);
                    lifeV = (double)needLifeV;
                    needLifeV += client.ClientData.CurrentLifeV;
                    client.ClientData.CurrentLifeV = Global.GMin(client.ClientData.LifeV, needLifeV);
                    relifeLog.newHp = client.ClientData.CurrentLifeV;
                    SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
                }
                if (lifeV > 0.0)
                {
                    List<object> listObjs = Global.GetAll9Clients(client);
                    GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, client.ClientData.CopyMapID, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, (double)client.ClientData.CurrentLifeV, (double)client.ClientData.CurrentMagicV, 120, listObjs, 0);
                }
            }
        }

        
        public static void ProcessLingLiVReserve(SocketListener sl, TCPOutPacketPool pool, GameClient client)
        {
            if (Global.CanMapUseBuffer(client.ClientData.MapCode, 10))
            {
                if (client.ClientData.InterPower < 30000)
                {
                    int needLingLiV = 30000 - client.ClientData.InterPower;
                    if (needLingLiV > 0)
                    {
                        BufferData bufferData = Global.GetBufferDataByID(client, 10);
                        if (null != bufferData)
                        {
                            if (bufferData.BufferVal > 0L)
                            {
                                int lingLiV = (int)Global.GMin((long)needLingLiV, bufferData.BufferVal);
                                bufferData.BufferVal -= (long)lingLiV;
                                client.ClientData.InterPower += lingLiV;
                                GameManager.ClientMgr.NotifyBufferData(client, bufferData);
                                GameManager.ClientMgr.NotifyUpdateInterPowerCmd(sl, pool, client, 0);
                            }
                        }
                    }
                }
            }
        }

        
        public static void ProcessLingLiVReserve2(SocketListener sl, TCPOutPacketPool pool, GameClient client, BufferData bufferData)
        {
            if (Global.CanMapUseBuffer(client.ClientData.MapCode, 10))
            {
                if (client.ClientData.InterPower < 30000)
                {
                    int needLingLiV = 30000 - client.ClientData.InterPower;
                    if (needLingLiV > 0)
                    {
                        if (null != bufferData)
                        {
                            if (bufferData.BufferVal > 0L)
                            {
                                int lingLiV = (int)Global.GMin((long)needLingLiV, bufferData.BufferVal);
                                bufferData.BufferVal -= (long)lingLiV;
                                client.ClientData.InterPower += lingLiV;
                                GameManager.ClientMgr.NotifyUpdateInterPowerCmd(sl, pool, client, 0);
                            }
                        }
                    }
                }
            }
        }

        
        public static double ProcessDblLingLi(GameClient client)
        {
            double ret = 1.0;
            double result;
            if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 3))
            {
                result = ret;
            }
            else
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 3);
                if (null == bufferData)
                {
                    result = ret;
                }
                else
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        ret = 2.0;
                    }
                    result = ret;
                }
            }
            return result;
        }

        
        public static void RefreshTimePropBuffer(GameClient client, BufferItemTypes bufferItemType)
        {
            BufferData bufferData = Global.GetBufferDataFromDict(client, (int)bufferItemType);
            if (null == bufferData)
            {
                bufferData = Global.GetBufferDataByID(client, (int)bufferItemType);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        Global.AddBufferDataIntoDict(client, bufferData.BufferID, bufferData);
                        client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
                        {
                            DelayExecProcIds.NotifyRefreshProps
                        });
                    }
                }
            }
            else
            {
                long nowTicks = TimeUtil.NOW();
                if (nowTicks - bufferData.StartTime >= (long)bufferData.BufferSecs * 1000L)
                {
                    Global.AddBufferDataIntoDict(client, bufferData.BufferID, null);
                    client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
                    {
                        DelayExecProcIds.NotifyRefreshProps
                    });
                }
            }
        }

        
        public static int GetTimeAddProp(GameClient client, BufferItemTypes bufferItemType)
        {
            int nRet = 0;
            BufferData bufferData = Global.GetBufferDataByID(client, (int)bufferItemType);
            int result;
            if (null == bufferData)
            {
                result = nRet;
            }
            else
            {
                long nowTicks = TimeUtil.NOW();
                if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                {
                    if (bufferData.BufferID == 52 || bufferData.BufferID == 53 || bufferData.BufferID == 54 || bufferData.BufferID == 55)
                    {
                        int actionGoodsID = (int)bufferData.BufferVal;
                        if (actionGoodsID <= 0)
                        {
                            return nRet;
                        }
                        List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(actionGoodsID);
                        int nValue = (int)magicActionItemList[0].MagicActionParams[0];
                        if (nValue > -1)
                        {
                            nRet = nValue;
                        }
                    }
                }
                result = nRet;
            }
            return result;
        }

        
        public static int GetBuffAddProp(GameClient client, BufferItemTypes bufferItemType)
        {
            int nRet = 0;
            BufferData bufferData = Global.GetBufferDataByID(client, (int)bufferItemType);
            int result;
            if (null == bufferData)
            {
                result = nRet;
            }
            else
            {
                if (bufferData.BufferID == 52 || bufferData.BufferID == 53 || bufferData.BufferID == 54 || bufferData.BufferID == 55)
                {
                    int actionGoodsID = (int)bufferData.BufferVal;
                    if (actionGoodsID <= 0)
                    {
                        return nRet;
                    }
                    List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(actionGoodsID);
                    int nValue = (int)magicActionItemList[0].MagicActionParams[0];
                    if (nValue > -1)
                    {
                        nRet = nValue;
                    }
                }
                result = nRet;
            }
            return result;
        }

        
        public static double ProcessTimeAddProp(GameClient client, BufferItemTypes bufferItemType)
        {
            double ret = 0.0;
            double result;
            if (!Global.CanMapUseBuffer(client.ClientData.MapCode, (int)bufferItemType))
            {
                result = ret;
            }
            else
            {
                BufferData bufferData = Global.GetBufferDataByID(client, (int)bufferItemType);
                if (null == bufferData)
                {
                    result = ret;
                }
                else
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        int actionGoodsID = (int)bufferData.BufferVal;
                        List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(actionGoodsID);
                        ret = magicActionItemList[0].MagicActionParams[0];
                    }
                    else if (bufferData.BufferID == 52 || bufferData.BufferID == 53 || bufferData.BufferID == 54 || bufferData.BufferID == 55)
                    {
                        long bufferVal = 0L;
                        lock (bufferData)
                        {
                            nowTicks = TimeUtil.NOW();
                            if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                            {
                                return ret;
                            }
                            bufferVal = bufferData.BufferVal;
                            bufferData.BufferVal = 0L;
                            if (bufferVal <= 0L)
                            {
                                return ret;
                            }
                            Global.RemoveBufferData(client, (int)bufferItemType);
                        }
                        int actionGoodsID = (int)bufferVal;
                        if (actionGoodsID <= 0)
                        {
                            return ret;
                        }
                        client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
                        {
                            DelayExecProcIds.RecalcProps,
                            DelayExecProcIds.NotifyRefreshProps
                        });
                    }
                    result = ret;
                }
            }
            return result;
        }

        
        public static double ProcessTempBufferProp(GameClient client, ExtPropIndexes expPropIndex)
        {
            return 0.0;
        }

        
        public static double ProcessAddTempAttack(GameClient client)
        {
            double ret = 0.0;
            double result;
            if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 6))
            {
                result = ret;
            }
            else
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 6);
                if (null == bufferData)
                {
                    result = ret;
                }
                else
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        ret = 0.1;
                    }
                    result = ret;
                }
            }
            return result;
        }

        
        public static void AddAttackBuffer(GameClient client)
        {
            if (null == client.ClientData.AddTempAttackBufferData)
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 6);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        client.ClientData.AddTempAttackBufferData = bufferData;
                        GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
                    }
                }
            }
        }

        
        public static void RemoveAttackBuffer(GameClient client)
        {
            BufferData bufferData = client.ClientData.AddTempAttackBufferData;
            if (null != bufferData)
            {
                long nowTicks = TimeUtil.NOW();
                if (nowTicks - bufferData.StartTime >= (long)bufferData.BufferSecs * 1000L)
                {
                    client.ClientData.AddTempAttackBufferData = null;
                    GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
                }
            }
        }

        
        public static double ProcessAddTempDefense(GameClient client)
        {
            double ret = 0.0;
            double result;
            if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 7))
            {
                result = ret;
            }
            else
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 7);
                if (null == bufferData)
                {
                    result = ret;
                }
                else
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        ret = 0.1;
                    }
                    result = ret;
                }
            }
            return result;
        }

        
        public static void AddDefenseBuffer(GameClient client)
        {
            if (null == client.ClientData.AddTempDefenseBufferData)
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 7);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        client.ClientData.AddTempDefenseBufferData = bufferData;
                        GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
                    }
                }
            }
        }

        
        public static void RemoveDefenseBuffer(GameClient client)
        {
            BufferData bufferData = client.ClientData.AddTempDefenseBufferData;
            if (null != bufferData)
            {
                long nowTicks = TimeUtil.NOW();
                if (nowTicks - bufferData.StartTime >= (long)bufferData.BufferSecs * 1000L)
                {
                    client.ClientData.AddTempDefenseBufferData = null;
                    GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
                }
            }
        }

        
        public static double ProcessUpLifeLimit(GameClient client)
        {
            double ret = 0.0;
            BufferData bufferData = Global.GetBufferDataByID(client, 8);
            double result;
            if (null == bufferData)
            {
                result = ret;
            }
            else
            {
                long nowTicks = TimeUtil.NOW();
                if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                {
                    ret = 0.1;
                }
                result = ret;
            }
            return result;
        }

        
        public static void AddUpLifeLimitStatus(GameClient client)
        {
            if (null == client.ClientData.UpLifeLimitBufferData)
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 8);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        client.ClientData.UpLifeLimitBufferData = bufferData;
                        GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
                    }
                }
            }
        }

        
        public static void RemoveUpLifeLimitStatus(GameClient client)
        {
            BufferData bufferData = client.ClientData.UpLifeLimitBufferData;
            if (null != bufferData)
            {
                long nowTicks = TimeUtil.NOW();
                if (nowTicks - bufferData.StartTime >= (long)bufferData.BufferSecs * 1000L)
                {
                    client.ClientData.UpLifeLimitBufferData = null;
                    GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
                }
            }
        }

        
        public static double ProcessRebornMultiExperience(GameClient client)
        {
            double ret = 1.0;
            if (Global.CanMapUseBuffer(client.ClientData.MapCode, 123))
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 123);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        ret = (double)((int)(bufferData.BufferVal & (long)0xffff_ffffUL));
                    }
                }
            }
            return ret;
        }

        
        public static double ProcessDblAndThreeExperience(GameClient client)
        {
            double ret = 1.0;
            if (Global.CanMapUseBuffer(client.ClientData.MapCode, 36))
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 36);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        ret = 5.0;
                    }
                }
            }
            if (ret < 5.0)
            {
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 18))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 18);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            ret = 3.0;
                        }
                    }
                }
            }
            if (ret < 3.0)
            {
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 1))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 1);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            ret = 2.0;
                        }
                    }
                }
            }
            if (ret <= 1.0)
            {
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 46))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 46);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            ret = (double)((int)(bufferData.BufferVal & (long)0xffff_ffffUL));
                        }
                    }
                }
            }
            return ret;
        }

        
        public static double ProcessDblAndThreeMoney(GameClient client)
        {
            double ret = 1.0;
            if (Global.CanMapUseBuffer(client.ClientData.MapCode, 19))
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 19);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        ret = 3.0;
                    }
                }
            }
            if (ret < 3.0)
            {
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 2))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 2);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            ret = 2.0;
                        }
                    }
                }
            }
            return ret;
        }

        
        public static double ProcessAutoGiveExperience(GameClient client)
        {
            double ret = 0.0;
            int actionGoodsID = 0;
            int leftSecs = 0;
            if (Global.CanMapUseBuffer(client.ClientData.MapCode, 21))
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 21);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        actionGoodsID = (int)bufferData.BufferVal;
                        List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(actionGoodsID);
                        if (magicActionItemList != null && magicActionItemList[0].MagicActionID == MagicActionIDs.DB_ADD_EXP)
                        {
                            if (nowTicks - client.ClientData.StartAddExpTicks >= (long)((int)(magicActionItemList[0].MagicActionParams[2] * 1000.0)))
                            {
                                client.ClientData.StartAddExpTicks = nowTicks;
                                ret = magicActionItemList[0].MagicActionParams[0];
                            }
                        }
                        leftSecs = (int)(((long)bufferData.BufferSecs * 1000L - (nowTicks - bufferData.StartTime)) / 1000L);
                    }
                }
            }
            if (ret > 0.0)
            {
                GameManager.ClientMgr.ProcessRoleExperience(client, (long)((int)ret), true, false, false, "none");
                if (actionGoodsID > 0)
                {
                    string msgText = string.Format(GLang.GetLang(107, new object[0]), Global.GetGoodsNameByID(actionGoodsID), ret, leftSecs / 60);
                    GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
                }
            }
            return ret;
        }

        
        public static void ProcessWaWaGiveExperience(GameClient client, Monster monster)
        {
            if (monster.MonsterInfo.VLevel >= client.ClientData.Level)
            {
                double ret = 0.0;
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 28))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 28);
                    if (null != bufferData)
                    {
                        if (bufferData.BufferVal > 0L)
                        {
                            bufferData.BufferVal -= 1L;
                            int actionGoodsID = bufferData.BufferSecs;
                            bool notifyBufferData = false;
                            List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(actionGoodsID);
                            if (magicActionItemList != null && magicActionItemList[0].MagicActionID == MagicActionIDs.DB_ADD_WAWA_EXP)
                            {
                                int monsterNumToAddExp = (int)magicActionItemList[0].MagicActionParams[0];
                                if (monsterNumToAddExp > 0)
                                {
                                    if (0L == bufferData.BufferVal % (long)monsterNumToAddExp)
                                    {
                                        ret = (double)client.ClientData.Level * (magicActionItemList[0].MagicActionParams[2] + (double)Global.GetRandomNumber(0, (int)magicActionItemList[0].MagicActionParams[3]));
                                        GameManager.ClientMgr.NotifyBufferData(client, bufferData);
                                        notifyBufferData = true;
                                    }
                                }
                            }
                            if (!notifyBufferData)
                            {
                                if (bufferData.BufferVal <= 0L)
                                {
                                    GameManager.ClientMgr.NotifyBufferData(client, bufferData);
                                }
                            }
                        }
                    }
                }
                if (ret > 0.0)
                {
                    double dblExperience = 1.0;
                    if (SpecailTimeManager.JugeIsDoulbeExperienceAndLingli())
                    {
                        dblExperience += 1.0;
                    }
                    ret = (double)((int)(ret * dblExperience));
                    GameManager.ClientMgr.ProcessRoleExperience(client, (long)((int)ret), true, false, false, "none");
                    Global.NotifySelfWaWaExp(client, (int)ret);
                }
            }
        }

        
        public static long ProcessZhuFuGiveExperience(GameClient client)
        {
            long ret = 0L;
            if (Global.CanMapUseBuffer(client.ClientData.MapCode, 29))
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 29);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        ret = ((long)bufferData.BufferSecs * 1000L - (nowTicks - bufferData.StartTime)) / 1000L;
                    }
                }
            }
            return ret;
        }

        
        public static long ProcessErGuoTouGiveExperience(GameClient client, long subTicks, out double multiExpNum)
        {
            multiExpNum = 0.0;
            long ret = 0L;
            if (Global.CanMapUseBuffer(client.ClientData.MapCode, 48))
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 48);
                if (null != bufferData)
                {
                    if (bufferData.BufferSecs > 0)
                    {
                        multiExpNum = (double)(bufferData.BufferVal & (long)0xffff_ffffUL) - 1.0;
                        bufferData.BufferSecs = Math.Max(0, bufferData.BufferSecs - (int)(subTicks / 1000L));
                        ret = (long)bufferData.BufferSecs;
                        GameManager.ClientMgr.NotifyBufferData(client, bufferData);
                    }
                }
            }
            return ret;
        }

        
        public static double ProcessDblSkillUp(GameClient client)
        {
            double ret = 1.0;
            double result;
            if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 17))
            {
                result = ret;
            }
            else
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 17);
                if (null == bufferData)
                {
                    result = ret;
                }
                else
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        ret = 2.0;
                    }
                    result = ret;
                }
            }
            return result;
        }

        
        public static int ProcessAntiBoss(GameClient client, Monster monster, int injuredVal)
        {
            int result;
            if (monster.MonsterType != 401 && monster.MonsterType != 301)
            {
                result = injuredVal;
            }
            else if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 11))
            {
                result = injuredVal;
            }
            else
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 11);
                if (null == bufferData)
                {
                    result = injuredVal;
                }
                else
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime >= (long)bufferData.BufferSecs * 1000L)
                    {
                        result = injuredVal;
                    }
                    else
                    {
                        result = injuredVal * (int)bufferData.BufferVal;
                    }
                }
            }
            return result;
        }

        
        public static int ProcessAntiRole(GameClient client, GameClient otherClient, int injuredVal)
        {
            int result;
            if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 12))
            {
                result = injuredVal;
            }
            else
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 12);
                if (null == bufferData)
                {
                    result = injuredVal;
                }
                else
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime >= (long)bufferData.BufferSecs * 1000L)
                    {
                        result = injuredVal;
                    }
                    else
                    {
                        result = injuredVal + (int)((double)bufferData.BufferVal / 100.0 * (double)injuredVal);
                    }
                }
            }
            return result;
        }

        
        public static double ProcessMonthVIP(GameClient client)
        {
            double ret = 0.0;
            double result;
            if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 13))
            {
                result = ret;
            }
            else
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 13);
                if (null == bufferData)
                {
                    result = ret;
                }
                else
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        ret = 1.0;
                    }
                    result = ret;
                }
            }
            return result;
        }

        
        public static bool ProcessAutoFightingProtect(GameClient client)
        {
            bool ret = false;
            if (Global.CanMapUseBuffer(client.ClientData.MapCode, 20))
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 20);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        ret = true;
                    }
                }
            }
            return ret;
        }

        
        public static bool ProcessFallTianSheng(GameClient client)
        {
            bool ret = false;
            if (Global.CanMapUseBuffer(client.ClientData.MapCode, 30))
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 30);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        int randNum = Global.GetRandomNumber(0, 101);
                        if ((long)randNum <= bufferData.BufferVal)
                        {
                            ret = true;
                        }
                    }
                }
            }
            return ret;
        }

        
        public static void ProcessGuMu(GameClient client, long elapseTicks)
        {
            BufferData bufferData = Global.GetBufferDataByID(client, 34);
            if (bufferData != null)
            {
                if (bufferData.StartTime == (long)TimeUtil.NowDateTime().DayOfYear && bufferData.BufferVal > 0L)
                {
                    bufferData.BufferVal = Math.Max(0L, bufferData.BufferVal - elapseTicks / 1000L);
                }
                else if (bufferData.BufferSecs > 0)
                {
                    bufferData.BufferSecs = (int)Math.Max(0L, (long)bufferData.BufferSecs - elapseTicks / 1000L);
                }
                GameManager.ClientMgr.NotifyBufferData(client, bufferData);
            }
            if (bufferData == null || Global.IsBufferDataOver(bufferData, 0L))
            {
                if (bufferData != null)
                {
                }
                int todayID = TimeUtil.NowDateTime().DayOfYear;
                int lastGiveDayID = Global.GetRoleParamsInt32FromDB(client, "GuMuAwardDayID");
                if (todayID == lastGiveDayID)
                {
                    if (client.CheckCheatData.LastNotifyLeaveGuMuTick == 0L)
                    {
                        client.CheckCheatData.LastNotifyLeaveGuMuTick = TimeUtil.NOW() * 10000L;
                    }
                    else if (TimeUtil.NOW() * 10000L - client.CheckCheatData.LastNotifyLeaveGuMuTick > 600000000L)
                    {
                        Global.ForceCloseClient(client, "超时未离开古墓地图", true);
                    }
                    GameManager.LuaMgr.GotoMap(client, GameManager.MainMapCode, -1, -1, -1);
                }
                else
                {
                    if (client.CheckCheatData.LastNotifyLeaveGuMuTick > 0L)
                    {
                        client.CheckCheatData.LastNotifyLeaveGuMuTick = 0L;
                    }
                    Global.GiveGuMuTimeLimitAward(client);
                }
            }
        }

        
        public static void ProcessMingJieBuffer(GameClient client, long elapseTicks)
        {
            BufferData bufferData = Global.GetBufferDataByID(client, 35);
            if (bufferData != null)
            {
                bufferData.BufferVal -= elapseTicks / 1000L;
                GameManager.ClientMgr.NotifyBufferData(client, bufferData);
            }
            if (bufferData == null || bufferData.BufferVal <= -6L)
            {
                if (bufferData != null)
                {
                    Global.RemoveBufferData(client, 35);
                }
                GameManager.LuaMgr.GotoMap(client, GameManager.MainMapCode, -1, -1, -1);
            }
        }

        
        public static double ProcessTimeAddPkKingAttackProp(GameClient client, ExtPropIndexes attackType)
        {
            double ret = 0.0;
            double result;
            if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 39))
            {
                result = ret;
            }
            else
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 39);
                if (null == bufferData)
                {
                    result = ret;
                }
                else
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        int actionGoodsID = (int)bufferData.BufferVal;
                        EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(actionGoodsID);
                        if (null != item)
                        {
                            ret += item.ExtProps[(int)attackType];
                        }
                    }
                    result = ret;
                }
            }
            return result;
        }

        
        public static double ProcessTimeAddPkKingExpProp(GameClient client)
        {
            double ret = 0.0;
            double result;
            if (!Global.CanMapUseBuffer(client.ClientData.MapCode, 39))
            {
                result = ret;
            }
            else
            {
                BufferData bufferData = Global.GetBufferDataByID(client, 39);
                if (null == bufferData)
                {
                    result = ret;
                }
                else
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        int actionGoodsID = (int)bufferData.BufferVal;
                        List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(actionGoodsID);
                        ret = magicActionItemList[0].MagicActionParams[3];
                    }
                    ret -= 1.0;
                    ret = Global.GMax(0.0, ret);
                    result = ret;
                }
            }
            return result;
        }

        
        public static double ProcessTimeAddJunQiProp(GameClient client, ExtPropIndexes attackType)
        {
            return 0.0;
        }

        
        public static void ProcessDSTimeSubLifeNoShow(GameClient client)
        {
            if (client.ClientData.CurrentLifeV > 0)
            {
                double lifeV = 0.0;
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, 42))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, 42);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            int timeSlotSecs = (int)(bufferData.BufferVal >> 32 & (long)0xffff_ffffUL);
                            int SubLiefV = (int)(bufferData.BufferVal & (long)0xffff_ffffUL);
                            if (nowTicks - client.ClientData.DSStartDSSubLifeNoShowTicks >= (long)(timeSlotSecs * 1000))
                            {
                                client.ClientData.DSStartDSSubLifeNoShowTicks = nowTicks;
                                lifeV = (double)SubLiefV;
                            }
                        }
                        else
                        {
                            Global.RemoveBufferData(client, 42);
                        }
                    }
                }
                if (lifeV > 0.0)
                {
                    GameClient enemyClient = GameManager.ClientMgr.FindClient(client.ClientData.FangDuRoleID);
                    if (null != enemyClient)
                    {
                        int nOcc = Global.CalcOriginalOccupationID(enemyClient);
                        GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, enemyClient, client, 0, (int)lifeV, 1.0, nOcc, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, true, 1.0, 0, 0, 0, 0.0);
                        if (client.ClientData.CurrentLifeV <= 0)
                        {
                            Global.RemoveBufferData(client, 42);
                        }
                    }
                }
            }
        }

        
        private static void ProcessTimeSubLifeNoShow(GameClient client, int id)
        {
            if (client.ClientData.CurrentLifeV > 0)
            {
                double lifeV = 0.0;
                DelayInjuredBufferItem delayInjuredBufferItem = null;
                if (Global.CanMapUseBuffer(client.ClientData.MapCode, id))
                {
                    BufferData bufferData = Global.GetBufferDataByID(client, id);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            delayInjuredBufferItem = (client.MyBufferExtManager.FindBufferItem(id) as DelayInjuredBufferItem);
                            if (null != delayInjuredBufferItem)
                            {
                                if (nowTicks - delayInjuredBufferItem.StartSubLifeNoShowTicks >= (long)(delayInjuredBufferItem.TimeSlotSecs * 1000))
                                {
                                    delayInjuredBufferItem.StartSubLifeNoShowTicks = nowTicks;
                                    lifeV = (double)delayInjuredBufferItem.SubLifeV;
                                }
                            }
                        }
                        else
                        {
                            Global.RemoveBufferData(client, id);
                            client.MyBufferExtManager.RemoveBufferItem(id);
                        }
                    }
                }
                if (lifeV > 0.0 && null != delayInjuredBufferItem)
                {
                    GameClient enemyClient = GameManager.ClientMgr.FindClient(delayInjuredBufferItem.ObjectID);
                    if (null != enemyClient)
                    {
                        int nOcc = Global.CalcOriginalOccupationID(enemyClient);
                        GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, enemyClient, client, 0, (int)lifeV, 1.0, nOcc, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, true, 1.0, 0, 0, 0, 0.0);
                        if (client.ClientData.CurrentLifeV <= 0)
                        {
                            Global.RemoveBufferData(client, id);
                            client.MyBufferExtManager.RemoveBufferItem(id);
                        }
                    }
                    else
                    {
                        Global.RemoveBufferData(client, id);
                        client.MyBufferExtManager.RemoveBufferItem(id);
                    }
                }
            }
        }

        
        public static void ProcessAllTimeSubLifeNoShow(GameClient client)
        {
            for (int id = 93; id <= 96; id++)
            {
                DBRoleBufferManager.ProcessTimeSubLifeNoShow(client, id);
            }
        }

        
        public static double ProcessSpecialAttackValueBuff(GameClient client, int BufferTypes)
        {
            double dValue = 0.0;
            double result;
            if (!Global.CanMapUseBuffer(client.ClientData.MapCode, BufferTypes))
            {
                result = dValue;
            }
            else
            {
                BufferData bufferData = Global.GetBufferDataByID(client, BufferTypes);
                if (null == bufferData)
                {
                    result = dValue;
                }
                else
                {
                    int nMagicID;
                    switch (BufferTypes)
                    {
                        case 64:
                            nMagicID = 202;
                            break;
                        case 65:
                            nMagicID = 203;
                            break;
                        case 66:
                            nMagicID = 204;
                            break;
                        default:
                            return dValue;
                    }
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        int actionGoodsID = (int)bufferData.BufferVal;
                        List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(actionGoodsID);
                        if (magicActionItemList != null && magicActionItemList[0].MagicActionID == (MagicActionIDs)nMagicID)
                        {
                            dValue = magicActionItemList[0].MagicActionParams[0];
                        }
                    }
                    result = dValue;
                }
            }
            return result;
        }
    }
}
