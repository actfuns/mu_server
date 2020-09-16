using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.Marriage.CoupleWish
{
    
    public class CoupleWishManager : SingletonTemplate<CoupleWishManager>, IManager, ICmdProcessorEx, ICmdProcessor
    {
        
        public bool initialize()
        {
            bool result;
            if (!this._Config.Load(Global.GameResPath(CoupleWishConsts.RankAwardCfgFile), Global.GameResPath(CoupleWishConsts.WishTypeCfgFile), Global.GameResPath(CoupleWishConsts.YanHuiCfgFile)))
            {
                result = false;
            }
            else
            {
                this.StatueMgr.SetWishConfig(this._Config);
                if (!this.StatueMgr.LoadConfig())
                {
                    result = false;
                }
                else
                {
                    foreach (CoupleWishRankAwardConfig awardItem in this._Config.RankAwardCfgList)
                    {
                        List<GoodsData> goods1List = GoodsHelper.ParseGoodsDataList(((string)awardItem.GoodsOneTag).Split(new char[]
                        {
                            '|'
                        }), CoupleWishConsts.RankAwardCfgFile);
                        List<GoodsData> goods2List = GoodsHelper.ParseGoodsDataList(((string)awardItem.GoodsTwoTag).Split(new char[]
                        {
                            '|'
                        }), CoupleWishConsts.RankAwardCfgFile);
                        awardItem.GoodsOneTag = goods1List;
                        awardItem.GoodsTwoTag = goods2List;
                    }
                    int[] nDayMax = GameManager.systemParamsList.GetParamValueIntArrayByName("WishEffectAwardMax", ',');
                    this.WishEffectDayMaxAward[CoupleWishManager.EWishEffectAwardType.BangJin] = nDayMax[0];
                    this.WishEffectDayMaxAward[CoupleWishManager.EWishEffectAwardType.BangZuan] = nDayMax[1];
                    this.WishEffectDayMaxAward[CoupleWishManager.EWishEffectAwardType.Exp] = nDayMax[2];
                    int[] nCanGetAwardMap = GameManager.systemParamsList.GetParamValueIntArrayByName("WishEffectAwardMap", ',');
                    if (nCanGetAwardMap != null)
                    {
                        foreach (int i in nCanGetAwardMap)
                        {
                            if (!this.CanEffectAwardMap.Contains(i))
                            {
                                this.CanEffectAwardMap.Add(i);
                            }
                        }
                    }
                    ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("CoupleWishManager.TimerProc", new EventHandler(this.TimerProc)), 20000, 10000);
                    result = true;
                }
            }
            return result;
        }

        
        public bool startup()
        {
            TCPCmdDispatcher.getInstance().registerProcessorEx(1390, 1, 1, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1391, 1, 1, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerStreamProcessorEx(1392, SingletonTemplate<CoupleWishManager>.Instance());
            TCPCmdDispatcher.getInstance().registerProcessorEx(1394, 1, 1, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1395, 3, 3, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1396, 1, 1, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(1397, 2, 2, SingletonTemplate<CoupleWishManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
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

        
        public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            if (nID == 1390)
            {
                this.HandleGetMainDataCommand(client, nID, bytes, cmdParams);
            }
            else if (nID == 1391)
            {
                this.HandleGetWishRecordCommand(client, nID, bytes, cmdParams);
            }
            else if (nID == 1392)
            {
                this.HandleWishOtherRoleCommand(client, nID, bytes, cmdParams);
            }
            else if (nID == 1395)
            {
                this.HandleAdmireStatueCommand(client, nID, bytes, cmdParams);
            }
            else if (nID == 1394)
            {
                this.HandleGetAdmireDataCommand(client, nID, bytes, cmdParams);
            }
            else if (nID == 1396)
            {
                this.HandleGetPartyDataCommand(client, nID, bytes, cmdParams);
            }
            else if (nID == 1397)
            {
                this.HandleJoinPartyCommand(client, nID, bytes, cmdParams);
            }
            return true;
        }

        
        public bool processCmd(GameClient client, string[] cmdParams)
        {
            return true;
        }

        
        private void HandleJoinPartyCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            int toCoupleId = Convert.ToInt32(cmdParams[1]);
            lock (this.Mutex)
            {
                client.sendCmd(nID, this.StatueMgr.HandleJoinParty(client, toCoupleId));
            }
        }

        
        private void HandleGetPartyDataCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            lock (this.Mutex)
            {
                CoupleWishYanHuiData data = this.StatueMgr.HandleQueryParty(client);
                client.sendCmd<CoupleWishYanHuiData>(nID, data, false);
            }
        }

        
        private void HandleGetAdmireDataCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            lock (this.Mutex)
            {
                CoupleWishTop1AdmireData data = this.StatueMgr.HandleQueryAdmireData(client);
                client.sendCmd<CoupleWishTop1AdmireData>(nID, data, false);
            }
        }

        
        private void HandleAdmireStatueCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            int toCoupleId = Convert.ToInt32(cmdParams[1]);
            int admireType = Convert.ToInt32(cmdParams[2]);
            if (admireType == 1 || admireType == 2)
            {
                lock (this.Mutex)
                {
                    client.sendCmd(nID, this.StatueMgr.HandleAdmireStatue(client, toCoupleId, admireType));
                }
            }
        }

        
        private void HandleWishOtherRoleCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            CoupleWishWishReqData cliReq = DataHelper.BytesToObject<CoupleWishWishReqData>(bytes, 0, bytes.Length);
            DateTime now = TimeUtil.NowDateTime();
            int wishWeek;
            if (client.ClientSocket.IsKuaFuLogin)
            {
                client.sendCmd(nID, -12);
            }
            else if (cliReq.CostType != 1 && cliReq.CostType != 2)
            {
                client.sendCmd(nID, -18);
            }
            else if (!this._Config.IsInWishTime(now, out wishWeek))
            {
                client.sendCmd(nID, -31);
            }
            else
            {
                CoupleWishTypeConfig wishCfg = this._Config.WishTypeCfgList.Find((CoupleWishTypeConfig _w) => _w.WishType == cliReq.WishType);
                if (wishCfg == null)
                {
                    client.sendCmd(nID, -3);
                }
                else if (cliReq.CostType == 1 && wishCfg.CostGoodsId > 0 && wishCfg.CostGoodsNum > 0 && Global.GetTotalGoodsCountByID(client, wishCfg.CostGoodsId) < wishCfg.CostGoodsNum)
                {
                    client.sendCmd(nID, -6, false);
                }
                else if (cliReq.CostType == 2 && wishCfg.CostZuanShi > 0 && client.ClientData.UserMoney < wishCfg.CostZuanShi)
                {
                    client.sendCmd(nID, -10, false);
                }
                else
                {
                    if (!string.IsNullOrEmpty(cliReq.WishTxt))
                    {
                        if (wishCfg.CanHaveWishTxt != 1)
                        {
                            client.sendCmd(nID, -25, false);
                            return;
                        }
                        if (cliReq.WishTxt.Length > CoupleWishConsts.MaxWishTxtLen)
                        {
                            client.sendCmd(nID, -26, false);
                            return;
                        }
                    }
                    CoupleWishWishRoleReq centerReq = new CoupleWishWishRoleReq();
                    centerReq.From.RoleId = client.ClientData.RoleID;
                    centerReq.From.ZoneId = client.ClientData.ZoneID;
                    centerReq.From.RoleName = client.ClientData.RoleName;
                    centerReq.WishType = cliReq.WishType;
                    centerReq.WishTxt = cliReq.WishTxt;
                    RoleData4Selector toManSelector = null;
                    RoleData4Selector toWifeSelector = null;
                    CoupleWishCoupleDataK rankCoupleData = null;
                    if (cliReq.IsWishRankRole)
                    {
                        centerReq.IsWishRank = true;
                        lock (this.Mutex)
                        {
                            int coupleIdx;
                            if (!this.SyncData.ThisWeek.CoupleIdex.TryGetValue(cliReq.ToRankCoupleId, out coupleIdx))
                            {
                                client.sendCmd(nID, -12, false);
                                return;
                            }
                            rankCoupleData = this.SyncData.ThisWeek.RankList[coupleIdx];
                            if (rankCoupleData == null || rankCoupleData.DbCoupleId != cliReq.ToRankCoupleId || rankCoupleData.Rank > CoupleWishConsts.MaxRankNum * 2)
                            {
                                client.sendCmd(nID, -12, false);
                                return;
                            }
                            centerReq.ToCoupleId = cliReq.ToRankCoupleId;
                            toManSelector = Global.sendToDB<RoleData4Selector, int>(10232, rankCoupleData.Man.RoleId, client.ServerId);
                            toWifeSelector = Global.sendToDB<RoleData4Selector, int>(10232, rankCoupleData.Wife.RoleId, client.ServerId);
                            if (toManSelector == null || toWifeSelector == null || toManSelector.RoleID <= 0 || toWifeSelector.RoleID <= 0)
                            {
                                toWifeSelector = (toManSelector = null);
                            }
                        }
                    }
                    else
                    {
                        int toRoleId = -1;
                        if (!string.IsNullOrEmpty(cliReq.ToLocalRoleName))
                        {
                            toRoleId = RoleName2IDs.FindRoleIDByName(cliReq.ToLocalRoleName, true);
                        }
                        if (toRoleId <= 0)
                        {
                            client.sendCmd(nID, -28, false);
                            return;
                        }
                        if (toRoleId == client.ClientData.RoleID)
                        {
                            client.sendCmd(nID, -27, false);
                            return;
                        }
                        int nSpouseId = MarryLogic.GetSpouseID(toRoleId);
                        if (nSpouseId <= 0)
                        {
                            client.sendCmd(nID, -29, false);
                            return;
                        }
                        toManSelector = Global.sendToDB<RoleData4Selector, int>(10232, toRoleId, client.ServerId);
                        toWifeSelector = Global.sendToDB<RoleData4Selector, int>(10232, nSpouseId, client.ServerId);
                        if (toManSelector == null || toWifeSelector == null)
                        {
                            client.sendCmd(nID, -15, false);
                            return;
                        }
                        if (!MarryLogic.SameSexMarry(false))
                        {
                            if (toManSelector.RoleSex == toWifeSelector.RoleSex)
                            {
                                client.sendCmd(nID, -15, false);
                                return;
                            }
                            if (toManSelector.RoleSex == 1)
                            {
                                DataHelper2.Swap<RoleData4Selector>(ref toManSelector, ref toWifeSelector);
                            }
                        }
                    }
                    if (toManSelector != null && toWifeSelector != null)
                    {
                        centerReq.ToMan.RoleId = toManSelector.RoleID;
                        centerReq.ToMan.ZoneId = toManSelector.ZoneId;
                        centerReq.ToMan.RoleName = toManSelector.RoleName;
                        centerReq.ToManSelector = DataHelper.ObjectToBytes<RoleData4Selector>(toManSelector);
                        centerReq.ToWife.RoleId = toWifeSelector.RoleID;
                        centerReq.ToWife.ZoneId = toWifeSelector.ZoneId;
                        centerReq.ToWife.RoleName = toWifeSelector.RoleName;
                        centerReq.ToWifeSelector = DataHelper.ObjectToBytes<RoleData4Selector>(toWifeSelector);
                    }
                    int ec = TianTiClient.getInstance().CoupleWishWishRole(centerReq);
                    if (ec < 0)
                    {
                        client.sendCmd(nID, ec);
                    }
                    else
                    {
                        if (cliReq.CostType == 1 && wishCfg.CostGoodsId > 0 && wishCfg.CostGoodsNum > 0)
                        {
                            bool oneUseBind = false;
                            bool oneUseTimeLimit = false;
                            Global.UseGoodsBindOrNot(client, wishCfg.CostGoodsId, wishCfg.CostGoodsNum, true, out oneUseBind, out oneUseTimeLimit);
                        }
                        if (cliReq.CostType == 2 && wishCfg.CostZuanShi > 0)
                        {
                            GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, wishCfg.CostZuanShi, "情侣祝福", true, true, false, DaiBiSySType.None);
                        }
                        if (wishCfg.IsHaveEffect == 1)
                        {
                            CoupleWishNtfWishEffectData effectData = new CoupleWishNtfWishEffectData();
                            effectData.From = centerReq.From;
                            effectData.WishType = cliReq.WishType;
                            effectData.WishTxt = cliReq.WishTxt;
                            effectData.To = new List<KuaFuRoleMiniData>();
                            if (cliReq.IsWishRankRole)
                            {
                                effectData.To.Add(rankCoupleData.Man);
                                effectData.To.Add(rankCoupleData.Wife);
                            }
                            else if (centerReq.ToMan.RoleName == cliReq.ToLocalRoleName)
                            {
                                effectData.To.Add(centerReq.ToMan);
                            }
                            else
                            {
                                effectData.To.Add(centerReq.ToWife);
                            }
                            lock (this.Mutex)
                            {
                                this.HandleWishEffect(effectData);
                            }
                        }
                        client.sendCmd(nID, 1);
                    }
                }
            }
        }

        
        private void HandleGetWishRecordCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            List<CoupleWishWishRecordData> records = TianTiClient.getInstance().CoupleWishGetWishRecord(client.ClientData.RoleID);
            if (records != null)
            {
                records.Reverse();
            }
            client.sendCmd<List<CoupleWishWishRecordData>>(nID, records, false);
        }

        
        private void HandleGetMainDataCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            if (this.IsGongNengOpened(client))
            {
                DateTime now = TimeUtil.NowDateTime();
                CoupleWishMainData mainData = new CoupleWishMainData();
                lock (this.Mutex)
                {
                    mainData.RankList = new List<CoupleWishCoupleData>(this.ThisWeekTopNList);
                    mainData.CanGetAwardId = this.CheckGiveAward(client);
                    int idx;
                    if (this.SyncData.ThisWeek.RoleIndex.TryGetValue(client.ClientData.RoleID, out idx))
                    {
                        CoupleWishCoupleDataK coupleDataK = this.SyncData.ThisWeek.RankList[idx];
                        if (coupleDataK.Man.RoleId == client.ClientData.RoleID || coupleDataK.Wife.RoleId == client.ClientData.RoleID)
                        {
                            mainData.MyCoupleRank = coupleDataK.Rank;
                            mainData.MyCoupleBeWishNum = coupleDataK.BeWishedNum;
                        }
                    }
                }
                mainData.MyCoupleManSelector = Global.sendToDB<RoleData4Selector, int>(10232, client.ClientData.RoleID, client.ServerId);
                if (MarryLogic.IsMarried(client.ClientData.RoleID))
                {
                    mainData.MyCoupleWifeSelector = Global.sendToDB<RoleData4Selector, int>(10232, client.ClientData.MyMarriageData.nSpouseID, client.ServerId);
                }
                if (client.ClientData.RoleSex == 1)
                {
                    DataHelper2.Swap<RoleData4Selector>(ref mainData.MyCoupleManSelector, ref mainData.MyCoupleWifeSelector);
                }
                client.sendCmd<CoupleWishMainData>(nID, mainData, false);
            }
        }

        
        private int CheckGiveAward(GameClient client)
        {
            int result;
            if (client == null)
            {
                result = 0;
            }
            else
            {
                DateTime now = TimeUtil.NowDateTime();
                int awardWeek;
                if (!this._Config.IsInAwardTime(now, out awardWeek))
                {
                    result = 0;
                }
                else
                {
                    lock (this.Mutex)
                    {
                        string szAwardFlag = Global.GetRoleParamByName(client, "29");
                        string[] fields = string.IsNullOrEmpty(szAwardFlag) ? null : szAwardFlag.Split(new char[]
                        {
                            ','
                        });
                        int idx;
                        if (fields != null && fields.Length == 2 && Convert.ToInt32(fields[0]) == awardWeek)
                        {
                            result = 0;
                        }
                        else if (awardWeek != this.SyncData.LastWeek.Week)
                        {
                            result = 0;
                        }
                        else if (!this.SyncData.LastWeek.RoleIndex.TryGetValue(client.ClientData.RoleID, out idx))
                        {
                            result = 0;
                        }
                        else
                        {
                            CoupleWishCoupleDataK coupleData = this.SyncData.LastWeek.RankList[idx];
                            if (coupleData == null)
                            {
                                result = 0;
                            }
                            else if (coupleData.Man.RoleId != client.ClientData.RoleID && coupleData.Wife.RoleId != client.ClientData.RoleID)
                            {
                                result = 0;
                            }
                            else
                            {
                                CoupleWishRankAwardConfig wishAward = this._Config.RankAwardCfgList.Find((CoupleWishRankAwardConfig _r) => coupleData.Rank >= _r.StartRank && (_r.EndRank <= 0 || coupleData.Rank <= _r.EndRank));
                                if (wishAward == null)
                                {
                                    result = 0;
                                }
                                else
                                {
                                    List<GoodsData> goodsList = new List<GoodsData>();
                                    goodsList.AddRange(wishAward.GoodsOneTag as List<GoodsData>);
                                    goodsList.AddRange((wishAward.GoodsTwoTag as List<GoodsData>).FindAll((GoodsData _g) => Global.IsCanGiveRewardByOccupation(client, _g.GoodsID)));
                                    if (Global.CanAddGoodsDataList(client, goodsList))
                                    {
                                        foreach (GoodsData goodsData in goodsList)
                                        {
                                            Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "情侣排行榜", false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
                                        }
                                    }
                                    else
                                    {
                                        Global.UseMailGivePlayerAward3(client.ClientData.RoleID, goodsList, GLang.GetLang(479, new object[0]), string.Format(GLang.GetLang(480, new object[0]), coupleData.Rank), 0, 0, 0);
                                    }
                                    Global.SaveRoleParamsStringToDB(client, "29", string.Format("{0},{1}", awardWeek, wishAward.Id), true);
                                    this.CheckTipsIconState(client);
                                    result = wishAward.Id;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        
        public void CheckTipsIconState(GameClient client)
        {
            if (client != null && this.IsGongNengOpened(client))
            {
                bool bCanGetAward = false;
                lock (this.Mutex)
                {
                    int awardWeek = 0;
                    if (this._Config.IsInAwardTime(TimeUtil.NowDateTime(), out awardWeek))
                    {
                        string szAwardFlag = Global.GetRoleParamByName(client, "29");
                        string[] fields = string.IsNullOrEmpty(szAwardFlag) ? null : szAwardFlag.Split(new char[]
                        {
                            ','
                        });
                        if (fields == null || fields.Length != 2 || (Convert.ToInt32(fields[0]) != awardWeek && this.SyncData.LastWeek.Week == awardWeek))
                        {
                            int idx;
                            if (this.SyncData.LastWeek.Week == awardWeek && this.SyncData.LastWeek.RoleIndex.TryGetValue(client.ClientData.RoleID, out idx))
                            {
                                int rank = this.SyncData.LastWeek.RankList[idx].Rank;
                                bCanGetAward = this._Config.RankAwardCfgList.Exists((CoupleWishRankAwardConfig _a) => rank >= _a.StartRank && (_a.EndRank <= 0 || rank <= _a.EndRank));
                            }
                        }
                    }
                }
                if (client._IconStateMgr.AddFlushIconState(15012, bCanGetAward))
                {
                    client._IconStateMgr.SendIconStateToClient(client);
                }
            }
        }

        
        public bool IsGongNengOpened(GameClient client)
        {
            return client != null;
        }

        
        private void SaveGetNextEffectAward(GameClient client, int day, CoupleWishManager.EWishEffectAwardType awardType, int get)
        {
            string szEffectAward = Global.GetRoleParamByName(client, "34");
            string[] awardFields = (!string.IsNullOrEmpty(szEffectAward)) ? szEffectAward.Split(new char[]
            {
                ','
            }) : null;
            int[] newFlag = new int[5];
            newFlag[0] = (int)awardType;
            newFlag[1] = day;
            if (awardFields != null && awardFields.Length == 5 && Convert.ToInt32(awardFields[1]) == day)
            {
                for (int i = 0; i < 3; i++)
                {
                    newFlag[2 + i] = Convert.ToInt32(awardFields[2 + i]);
                }
            }
            newFlag[(int)(2 + awardType)] = (int)Math.Min(2147483647L, (long)(newFlag[(int)(2 + awardType)] + get));
            Global.SaveRoleParamsStringToDB(client, "34", string.Format("{0},{1},{2},{3},{4}", new object[]
            {
                newFlag[0],
                newFlag[1],
                newFlag[2],
                newFlag[3],
                newFlag[4]
            }), true);
        }

        
        private bool GetNextCanEffectAward(GameClient client, int day, out CoupleWishManager.EWishEffectAwardType awardType, out int canGetMax)
        {
            awardType = CoupleWishManager.EWishEffectAwardType.None;
            canGetMax = 0;
            string szEffectAward = Global.GetRoleParamByName(client, "34");
            string[] awardFields = (!string.IsNullOrEmpty(szEffectAward)) ? szEffectAward.Split(new char[]
            {
                ','
            }) : null;
            bool result;
            if (awardFields == null || awardFields.Length != 5)
            {
                awardType = CoupleWishManager.EWishEffectAwardType.BangJin;
                canGetMax = this.WishEffectDayMaxAward[awardType];
                result = true;
            }
            else if (Convert.ToInt32(awardFields[1]) != day)
            {
                awardType = (EWishEffectAwardType)((Convert.ToInt32(awardFields[0]) + (int)EWishEffectAwardType.BangZuan) % (int)EWishEffectAwardType.Max);
                canGetMax = this.WishEffectDayMaxAward[awardType];
                result = true;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    int nextType = (Convert.ToInt32(awardFields[0]) + i + 1) % 3;
                    int nextHadGet = Convert.ToInt32(awardFields[2 + nextType]);
                    if (this.WishEffectDayMaxAward[(CoupleWishManager.EWishEffectAwardType)nextType] > nextHadGet)
                    {
                        awardType = (CoupleWishManager.EWishEffectAwardType)nextType;
                        canGetMax = this.WishEffectDayMaxAward[awardType] - nextHadGet;
                        return true;
                    }
                }
                result = false;
            }
            return result;
        }

        
        private void HandleWishEffect(CoupleWishNtfWishEffectData effectData)
        {
            if (effectData != null)
            {
                int dayFlag = TimeUtil.MakeYearMonthDay(TimeUtil.NowDateTime());
                int index = 0;
                GameClient client;
                while ((client = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
                {
                    if (!client.ClientData.FirstPlayStart)
                    {
                        if (this.CanEffectAwardMap.Contains(client.ClientData.MapCode))
                        {
                            GameMap gameMap = null;
                            if (GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
                            {
                                if (gameMap.InSafeRegionList(client.CurrentGrid))
                                {
                                    effectData.GetBinJinBi = 0;
                                    effectData.GetBindZuanShi = 0;
                                    effectData.GetExp = 0;
                                    CoupleWishManager.EWishEffectAwardType awardType;
                                    int canGetMax;
                                    if (!this.GetNextCanEffectAward(client, dayFlag, out awardType, out canGetMax))
                                    {
                                        client.sendCmd<CoupleWishNtfWishEffectData>(1393, effectData, false);
                                    }
                                    else
                                    {
                                        int calcKey = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
                                        int realGet;
                                        if (awardType == CoupleWishManager.EWishEffectAwardType.BangJin)
                                        {
                                            effectData.GetBinJinBi = Math.Max(0, Math.Min(400 * calcKey, canGetMax));
                                            realGet = effectData.GetBinJinBi;
                                        }
                                        else if (awardType == CoupleWishManager.EWishEffectAwardType.BangZuan)
                                        {
                                            effectData.GetBindZuanShi = Math.Max(0, Math.Min((int)(0.08 * (double)calcKey), canGetMax));
                                            realGet = effectData.GetBindZuanShi;
                                        }
                                        else
                                        {
                                            if (awardType != CoupleWishManager.EWishEffectAwardType.Exp)
                                            {
                                                continue;
                                            }
                                            effectData.GetExp = Math.Max(0, Math.Min(4000 * calcKey, canGetMax));
                                            realGet = effectData.GetExp;
                                        }
                                        if (effectData.GetBinJinBi > 0)
                                        {
                                            GameManager.ClientMgr.AddMoney1(client, effectData.GetBinJinBi, "情侣祝福特效", true);
                                            string tip = string.Format(GLang.GetLang(481, new object[0]), effectData.From.RoleName, realGet);
                                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
                                        }
                                        if (effectData.GetBindZuanShi > 0)
                                        {
                                            GameManager.ClientMgr.AddUserGold(client, effectData.GetBindZuanShi, "情侣祝福特效");
                                            string tip = string.Format(GLang.GetLang(482, new object[0]), effectData.From.RoleName, realGet);
                                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
                                        }
                                        if (effectData.GetExp > 0)
                                        {
                                            GameManager.ClientMgr.ProcessRoleExperience(client, (long)effectData.GetExp, false, true, false, "none");
                                            GameManager.ClientMgr.NotifyAddExpMsg(client, (long)effectData.GetExp);
                                            string tip = string.Format(GLang.GetLang(483, new object[0]), effectData.From.RoleName, realGet);
                                            GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
                                        }
                                        client.sendCmd<CoupleWishNtfWishEffectData>(1393, effectData, false);
                                        this.SaveGetNextEffectAward(client, dayFlag, awardType, realGet);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        
        public bool PreClearDivorceData(int man, int wife)
        {
            return TianTiClient.getInstance().CoupleWishPreDivorce(man, wife) >= 0;
        }

        
        private void TimerProc(object sender, EventArgs e)
        {
            CoupleWishSyncData _syncData = TianTiClient.getInstance().CoupleWishSyncCenterData(this.SyncData.ThisWeek.ModifyTime, this.SyncData.LastWeek.ModifyTime, this.SyncData.Statue.ModifyTime);
            if (_syncData != null)
            {
                lock (this.Mutex)
                {
                    if (_syncData.ThisWeek.ModifyTime != this.SyncData.ThisWeek.ModifyTime)
                    {
                        this.SyncData.ThisWeek = _syncData.ThisWeek;
                        this.ThisWeekTopNList.Clear();
                        foreach (CoupleWishCoupleDataK syncCouple in this.SyncData.ThisWeek.RankList)
                        {
                            if (syncCouple.Rank > CoupleWishConsts.MaxRankNum)
                            {
                                break;
                            }
                            CoupleWishCoupleData couple = new CoupleWishCoupleData();
                            couple.DbCoupleId = syncCouple.DbCoupleId;
                            couple.Man = syncCouple.Man;
                            if (syncCouple.ManSelector != null)
                            {
                                couple.ManSelector = DataHelper.BytesToObject<RoleData4Selector>(syncCouple.ManSelector, 0, syncCouple.ManSelector.Length);
                            }
                            couple.Wife = syncCouple.Wife;
                            if (syncCouple.WifeSelector != null)
                            {
                                couple.WifeSelector = DataHelper.BytesToObject<RoleData4Selector>(syncCouple.WifeSelector, 0, syncCouple.WifeSelector.Length);
                            }
                            couple.BeWishedNum = syncCouple.BeWishedNum;
                            couple.Rank = syncCouple.Rank;
                            this.ThisWeekTopNList.Add(couple);
                        }
                    }
                    if (_syncData.LastWeek.ModifyTime != this.SyncData.LastWeek.ModifyTime)
                    {
                        this.SyncData.LastWeek = _syncData.LastWeek;
                    }
                    if (_syncData.Statue.ModifyTime != this.SyncData.Statue.ModifyTime)
                    {
                        this.SyncData.Statue = _syncData.Statue;
                        this.StatueMgr.SetDiaoXiang(this.SyncData.Statue);
                    }
                }
            }
        }

        
        private object Mutex = new object();

        
        private CoupleWishSyncData SyncData = new CoupleWishSyncData();

        
        private List<CoupleWishCoupleData> ThisWeekTopNList = new List<CoupleWishCoupleData>();

        
        public readonly CoupleWishConfig _Config = new CoupleWishConfig();

        
        private CoupleWishStatueManager StatueMgr = new CoupleWishStatueManager();

        
        private Dictionary<CoupleWishManager.EWishEffectAwardType, int> WishEffectDayMaxAward = new Dictionary<CoupleWishManager.EWishEffectAwardType, int>
        {
            {
                CoupleWishManager.EWishEffectAwardType.BangJin,
                60000
            },
            {
                CoupleWishManager.EWishEffectAwardType.BangZuan,
                10000
            },
            {
                CoupleWishManager.EWishEffectAwardType.Exp,
                1000000
            }
        };

        
        private HashSet<int> CanEffectAwardMap = new HashSet<int>();

        
        private enum EWishEffectAwardType
        {
            
            BangJin,
            
            BangZuan,
            
            Exp,
            
            Max,
            
            None = 99
        }
    }
}
