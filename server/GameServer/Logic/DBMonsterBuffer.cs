using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic.NewBufferExt;
using Server.Data;

namespace GameServer.Logic
{
    
    public class DBMonsterBuffer
    {
        
        public static void ProcessDSTimeAddLifeNoShow(Monster monster)
        {
            if (monster.VLife > 0.0)
            {
                double lifeV = 0.0;
                BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 40);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        int timeSlotSecs = (int)((ulong)bufferData.BufferVal >> 32 & 0xffff_ffffUL);
                        int addLiefV = (int)(bufferData.BufferVal & (long)0xffff_ffffUL);
                        if (nowTicks - monster.DSStartDSAddLifeNoShowTicks >= (long)(timeSlotSecs * 1000))
                        {
                            monster.DSStartDSAddLifeNoShowTicks = nowTicks;
                            lifeV = (double)addLiefV;
                        }
                    }
                    else
                    {
                        Global.RemoveMonsterBufferData(monster, 40);
                    }
                }
                if (monster.VLife < monster.MonsterInfo.VLifeMax && lifeV > 0.0)
                {
                    lifeV += monster.VLife;
                    monster.VLife = Global.GMin(monster.MonsterInfo.VLifeMax, lifeV);
                    List<object> listObjs = Global.GetAll9Clients(monster);
                    GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.SafeDirection, monster.VLife, monster.VMana, 120, listObjs, 0);
                }
            }
        }

        
        public static int ProcessHuZhaoSubLifeV(Monster monster, int subLifeV)
        {
            if (monster.VLife > 0.0)
            {
                if (Global.CanMapUseBuffer(monster.CurrentMapCode, 97))
                {
                    BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 97);
                    if (null != bufferData)
                    {
                        if (bufferData.BufferVal > 0L)
                        {
                            long needLifeV = (long)(monster.MonsterInfo.VLifeMax - monster.VLife);
                            HuZhaoBufferItem huZhaoBufferItem = monster.MyBufferExtManager.FindBufferItem(97) as HuZhaoBufferItem;
                            if (huZhaoBufferItem != null)
                            {
                                needLifeV = Global.GMin(needLifeV, (long)huZhaoBufferItem.InjuredV);
                                needLifeV = (long)((int)Global.GMin(needLifeV, (long)((int)bufferData.BufferVal)));
                                bufferData.BufferVal -= (long)Global.GMin((int)bufferData.BufferVal, huZhaoBufferItem.InjuredV);
                                subLifeV = (int)Global.GMin(needLifeV, (long)subLifeV);
                            }
                        }
                        else
                        {
                            Global.RemoveMonsterBufferData(monster, 97);
                            monster.MyBufferExtManager.RemoveBufferItem(97);
                            bufferData.BufferSecs = 0;
                            bufferData.StartTime = 0L;
                            GameManager.ClientMgr.NotifyOtherBufferData(monster, bufferData);
                        }
                    }
                }
            }
            return subLifeV;
        }

        
        public static double ProcessHuZhaoRecoverPercent(Monster monster)
        {
            double percent = 0.0;
            if (monster.VLife > 0.0)
            {
                if (Global.CanMapUseBuffer(monster.CurrentMapCode, 97))
                {
                    BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 97);
                    if (null != bufferData)
                    {
                        if (bufferData.BufferVal > 0L)
                        {
                            HuZhaoBufferItem huZhaoBufferItem = monster.MyBufferExtManager.FindBufferItem(97) as HuZhaoBufferItem;
                            if (huZhaoBufferItem != null)
                            {
                                percent = huZhaoBufferItem.RecoverLifePercent;
                            }
                        }
                        else
                        {
                            Global.RemoveMonsterBufferData(monster, 97);
                            monster.MyBufferExtManager.RemoveBufferItem(97);
                        }
                    }
                }
            }
            return percent;
        }

        
        public static int ProcessWuDiHuZhaoNoInjured(Monster monster, int subLifeV)
        {
            if (monster.VLife > 0.0)
            {
                if (Global.CanMapUseBuffer(monster.CurrentMapCode, 98))
                {
                    BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 98);
                    if (null != bufferData)
                    {
                        long nowTicks = TimeUtil.NOW();
                        if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                        {
                            subLifeV = 0;
                        }
                        else
                        {
                            Global.RemoveMonsterBufferData(monster, 98);
                        }
                    }
                }
            }
            return subLifeV;
        }

        
        public static int ProcessMarriageFubenInjured(Monster monster, int subLifeV)
        {
            if (monster.VLife > 0.0 && subLifeV > 0)
            {
                if (Global.CanMapUseBuffer(monster.CurrentMapCode, 2000808))
                {
                    BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 2000808);
                    if (null != bufferData)
                    {
                        subLifeV = (int)((double)subLifeV * ((double)bufferData.BufferVal / 100.0));
                    }
                }
            }
            return subLifeV;
        }

        
        public static void ProcessDSTimeSubLifeNoShow(Monster monster)
        {
            if (monster.VLife > 0.0)
            {
                double lifeV = 0.0;
                BufferData bufferData = Global.GetMonsterBufferDataByID(monster, 42);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        int timeSlotSecs = (int)(bufferData.BufferVal >> 32 & (long)0xffff_ffffUL);
                        int SubLiefV = (int)(bufferData.BufferVal & (long)0xffff_ffffUL);
                        if (nowTicks - monster.DSStartDSSubLifeNoShowTicks >= (long)(timeSlotSecs * 1000))
                        {
                            monster.DSStartDSSubLifeNoShowTicks = nowTicks;
                            lifeV = (double)SubLiefV;
                        }
                    }
                    else
                    {
                        Global.RemoveMonsterBufferData(monster, 42);
                    }
                }
                if (lifeV > 0.0)
                {
                    GameClient enemyClient = GameManager.ClientMgr.FindClient(monster.FangDuRoleID);
                    if (null != enemyClient)
                    {
                        int nOcc = Global.CalcOriginalOccupationID(enemyClient);
                        GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, enemyClient, monster, 0, (int)lifeV, 1.0, nOcc, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
                        if (monster.VLife <= 0.0)
                        {
                            Global.RemoveMonsterBufferData(monster, 42);
                        }
                    }
                }
            }
        }

        
        private static void ProcessTimeSubLifeNoShow(Monster monster, int id)
        {
            if (monster.VLife > 0.0)
            {
                double lifeV = 0.0;
                DelayInjuredBufferItem delayInjuredBufferItem = null;
                BufferData bufferData = Global.GetMonsterBufferDataByID(monster, id);
                if (null != bufferData)
                {
                    long nowTicks = TimeUtil.NOW();
                    if (nowTicks - bufferData.StartTime < (long)bufferData.BufferSecs * 1000L)
                    {
                        delayInjuredBufferItem = (monster.MyBufferExtManager.FindBufferItem(id) as DelayInjuredBufferItem);
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
                        Global.RemoveMonsterBufferData(monster, id);
                        monster.MyBufferExtManager.RemoveBufferItem(id);
                    }
                }
                if (lifeV > 0.0 && null != delayInjuredBufferItem)
                {
                    GameClient enemyClient = GameManager.ClientMgr.FindClient(delayInjuredBufferItem.ObjectID);
                    if (null != enemyClient)
                    {
                        int nOcc = Global.CalcOriginalOccupationID(enemyClient);
                        GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, enemyClient, monster, 0, (int)lifeV, 1.0, nOcc, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
                        if (monster.VLife <= 0.0)
                        {
                            Global.RemoveMonsterBufferData(monster, id);
                            monster.MyBufferExtManager.RemoveBufferItem(id);
                        }
                    }
                    else
                    {
                        Global.RemoveMonsterBufferData(monster, id);
                        monster.MyBufferExtManager.RemoveBufferItem(id);
                    }
                }
            }
        }

        
        public static void ProcessAllTimeSubLifeNoShow(Monster monster)
        {
            for (int id = 93; id <= 96; id++)
            {
                DBMonsterBuffer.ProcessTimeSubLifeNoShow(monster, id);
            }
        }
    }
}
