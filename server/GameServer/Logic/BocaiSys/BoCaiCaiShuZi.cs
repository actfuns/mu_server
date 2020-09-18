using System;
using System.Collections.Generic;
using System.Linq;
using AutoCSer.Net.TcpServer;
using GameServer.Core.Executor;
using KF.TcpCall;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
    
    public class BoCaiCaiShuZi
    {
        
        private BoCaiCaiShuZi()
        {
        }

        
        public static BoCaiCaiShuZi GetInstance()
        {
            return BoCaiCaiShuZi.instance;
        }

        
        private bool GetOpenLotteryData(bool init = false)
        {
            try
            {
                ReturnValue<OpenLottery> msgData = TcpCall.KFBoCaiManager.GetOpenLottery(this.BoCaiType);
                if (!msgData.IsReturn)
                {
                    return false;
                }
                OpenLottery OpenData = msgData.Value;
                return this.SetOpenLotteryData(OpenData, init, false);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
            return false;
        }

        
        private void GetRank()
        {
            try
            {
                List<OpenLottery> openHistory = BoCaiManager.getInstance().GetNewOpenLottery10(this.BoCaiType);
                if (null != openHistory)
                {
                    ReturnValue<List<KFBoCaoHistoryData>> msgData = TcpCall.KFBoCaiManager.GetWinHistory(this.BoCaiType);
                    if (!msgData.IsReturn)
                    {
                        LogManager.WriteLog(LogTypes.Error, "[ljl_caidaxiao_猜数字]猜数字获取排行 失败", null, true);
                    }
                    else
                    {
                        List<KFBoCaoHistoryData> History = msgData.Value;
                        lock (this.mutex)
                        {
                            this.RankResult = true;
                            this.OpenHistory = openHistory;
                            this.WinHistory.Clear();
                            if (null != History)
                            {
                                using (List<KFBoCaoHistoryData>.Enumerator enumerator = History.GetEnumerator())
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        KFBoCaoHistoryData item = enumerator.Current;
                                        OpenLottery data = this.OpenHistory.Find((OpenLottery x) => x.DataPeriods == item.DataPeriods);
                                        if (data != null && !string.IsNullOrEmpty(data.strWinNum))
                                        {
                                            item.OpenData = data.strWinNum;
                                            this.WinHistory.Add(item);
                                        }
                                    }
                                }
                                List<long> DataPeriodsList = new List<long>();
                                List<long> DataPeriodsList2 = new List<long>();
                                foreach (OpenLottery open in this.OpenHistory)
                                {
                                    DataPeriodsList.Add(open.DataPeriods);
                                }
                                DataPeriodsList2 = this.BuyItemHistoryDict.Keys.ToList<long>();
                                using (List<long>.Enumerator enumerator3 = DataPeriodsList2.GetEnumerator())
                                {
                                    while (enumerator3.MoveNext())
                                    {
                                        long Periods = enumerator3.Current;
                                        if (DataPeriodsList.Find((long x) => x == Periods) < 1L)
                                        {
                                            this.BuyItemHistoryDict.Remove(Periods);
                                        }
                                    }
                                }
                                foreach (long Periods2 in DataPeriodsList)
                                {
                                    if (!this.BuyItemHistoryDict.ContainsKey(Periods2))
                                    {
                                        List<BuyBoCai2SDB> ItemList;
                                        if (BoCaiManager.getInstance().GetBuyList2DB(this.BoCaiType, Periods2, out ItemList, 1))
                                        {
                                            List<RoleBuyHistory> roleBuyList = new List<RoleBuyHistory>();
                                            using (List<BuyBoCai2SDB>.Enumerator enumerator4 = ItemList.GetEnumerator())
                                            {
                                                while (enumerator4.MoveNext())
                                                {
                                                    BuyBoCai2SDB dbdata = enumerator4.Current;
                                                    RoleBuyHistory roledata = roleBuyList.Find((RoleBuyHistory x) => x.RoleID == dbdata.m_RoleID);
                                                    if (null == roledata)
                                                    {
                                                        roledata = new RoleBuyHistory();
                                                        roledata.RoleID = dbdata.m_RoleID;
                                                        roledata.BuyItemList = new List<BoCaiBuyItem>();
                                                        roleBuyList.Add(roledata);
                                                    }
                                                    roledata.BuyItemList.Add(new BoCaiBuyItem
                                                    {
                                                        BuyNum = dbdata.BuyNum,
                                                        strBuyValue = dbdata.strBuyValue,
                                                        DataPeriods = Periods2
                                                    });
                                                }
                                            }
                                            this.BuyItemHistoryDict.Add(Periods2, roleBuyList);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
        }

        
        public bool LoadBuyList(long DataPeriods)
        {
            try
            {
                if (!this.IsStart)
                {
                    return true;
                }
                List<BuyBoCai2SDB> ItemList = new List<BuyBoCai2SDB>();
                if (DataPeriods > 0L && !BoCaiManager.getInstance().GetBuyList2DB(this.BoCaiType, DataPeriods, out ItemList, 1))
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_caidaxiao_猜数字]获取购买记录失败 BoCaiType={0},DataPeriods={1}", this.BoCaiType, DataPeriods), null, true);
                    return false;
                }
                lock (this.mutex)
                {
                    using (List<BuyBoCai2SDB>.Enumerator enumerator = ItemList.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            BuyBoCai2SDB item = enumerator.Current;
                            PlayerBuyBoCaiData playerBoCai = this.BoCaiBaseList.Find((PlayerBuyBoCaiData x) => x.RoleID == item.m_RoleID);
                            if (null == playerBoCai)
                            {
                                playerBoCai = new PlayerBuyBoCaiData();
                                playerBoCai.BuyItemList = new List<BoCaiBuyItem>();
                                playerBoCai.ZoneID = item.ZoneID;
                                playerBoCai.RoleID = item.m_RoleID;
                                playerBoCai.ServerId = item.ServerId;
                                playerBoCai.RoleName = item.m_RoleName;
                                playerBoCai.strUserID = item.strUserID;
                                this.BoCaiBaseList.Add(playerBoCai);
                            }
                            playerBoCai.BuyItemList.Add(new BoCaiBuyItem
                            {
                                BuyNum = item.BuyNum,
                                strBuyValue = item.strBuyValue,
                                DataPeriods = item.DataPeriods
                            });
                        }
                    }
                    LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_caidaxiao_猜数字]加载购买数据true ,DataPeriods = {0}", DataPeriods), null, true);
                    this.IsStart = false;
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
            LogManager.WriteLog(LogTypes.Error, "[ljl_caidaxiao_猜数字]猜数字获取排行 失败", null, true);
            return false;
        }

        
        private bool GetStageData()
        {
            try
            {
                ReturnValue<KFStageData> msgData = TcpCall.KFBoCaiManager.GetKFStageData(this.BoCaiType);
                if (!msgData.IsReturn)
                {
                    return false;
                }
                KFStageData data = msgData.Value;
                return this.SetStageData(data, false);
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
            return false;
        }

        
        private void startNewGame(OpenLottery OpenData = null, bool init = false)
        {
            try
            {
                lock (this.mutex)
                {
                    this.BoCaiBaseList.Clear();
                    this.ServerOpenData.XiaoHaoDaiBi = -1;
                    this.ServerData = new CenterServerCaiShuZi();
                    this.GetRank();
                    if (null != OpenData)
                    {
                        this.SetOpenLotteryData(OpenData, true, false);
                        this.GetStageData();
                    }
                    else
                    {
                        if (this.GetStageData())
                        {
                            this.StartServerStage = (BoCaiStageEnum)this.StageData.Stage;
                        }
                        else
                        {
                            this.StartServerStage = BoCaiStageEnum.Stage_Err;
                        }
                        if (this.StageData.isOpenDay && this.StageData.Stage >= 2)
                        {
                            if (!this.GetOpenLotteryData(true))
                            {
                                LogManager.WriteLog(LogTypes.Error, "[ljl_caidaxiao_猜数字]本期开奖数据 失败", null, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
        }

        
        public void Init()
        {
            try
            {
                lock (this.mutex)
                {
                    this.IsStart = true;
                    this.StageData.isOpen = false;
                    this.StageData.isOpenDay = false;
                    GetOpenList DBData;
                    if (BoCaiManager.getInstance().GetOpenList2DB(this.BoCaiType, out DBData) && DBData != null && null != DBData.ItemList)
                    {
                        this.StartServerOpenData = new OpenLottery();
                        this.StartServerOpenData.DataPeriods = 0L;
                        if (DBData.ItemList.Count < 1)
                        {
                            this.StartServerOpenData.IsAward = true;
                            this.StartServerOpenData.DataPeriods = DBData.MaxDataPeriods;
                        }
                        foreach (OpenLottery item in DBData.ItemList)
                        {
                            if (item.DataPeriods > this.StartServerOpenData.DataPeriods)
                            {
                                this.StartServerOpenData = item;
                            }
                        }
                    }
                    this.startNewGame(null, true);
                    this.ServerData.UpOldOpenTime = TimeUtil.NowDateTime();
                    BoCaiManager.getInstance().OldtterySet(this.BoCaiType, this.ServerOpenData.DataPeriods);
                    if (!this.StageData.isOpen)
                    {
                        LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_caidaxiao_猜数字]博彩猜数字 暂未开启活动 OpenTime={0}", this.StageData.OpenTime), null, true);
                    }
                }
                this.OpenLotterySetWin();
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
        }

        
        private void CheckStageData(DateTime _time, bool isReload)
        {
            try
            {
                lock (this.mutex)
                {
                    bool isCheck = false;
                    double last = (_time - this.ServerData.GetStageDataTime).TotalSeconds;
                    if (isReload)
                    {
                        isCheck = true;
                    }
                    else if (this.StageData.isOpenDay)
                    {
                        if (last > (double)this.UpStageTime)
                        {
                            isCheck = true;
                        }
                        else if (this.StageData.Stage == 1 && last > 5.0)
                        {
                            isCheck = true;
                        }
                        else if (this.StageData.Stage == 4 && last > 5.0)
                        {
                            isCheck = true;
                        }
                        else if (this.StageData.Stage >= 1 && this.StageData.LastOpenTime >= 0L && (last + 1.0) * 1000.0 > (double)this.StageData.LastOpenTime)
                        {
                            isCheck = true;
                        }
                        else if (this.StageData.LastOpenTime < 0L && last > 5.0)
                        {
                            isCheck = true;
                        }
                        else if (_time.Day != this.ServerData.GetStageDataTime.Day && _time.AddSeconds(-5.0).Day != this.ServerData.GetStageDataTime.Day)
                        {
                            isCheck = true;
                        }
                    }
                    else if (this.ServerData.GetStageDataTime.Day != _time.Day)
                    {
                        isCheck = true;
                    }
                    else if (this.StageData.OpenTime >= 0L && (this.ServerData.GetStageDataTime.AddSeconds((double)this.StageData.OpenTime) - _time).TotalSeconds < 1.0)
                    {
                        isCheck = true;
                    }
                    else if (this.StageData.OpenTime < 0L && last > 30.0)
                    {
                        isCheck = true;
                    }
                    if (isCheck)
                    {
                        this.GetStageData();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
        }

        
        public void BigTimeUpData(bool reload = false)
        {
            try
            {
                DateTime _time = TimeUtil.NowDateTime();
                this.CheckStageData(_time, reload);
                if (this.StageData.isOpenDay)
                {
                    if (this.StageData.Stage < 5 && this.StageData.Stage >= 2 && this.ServerOpenData.DataPeriods < 1L)
                    {
                        this.GetOpenLotteryData(false);
                    }
                    else if (this.StageData.Stage >= 2 && this.StageData.Stage < 4 && (_time - this.ServerData.UpBalanceTime).TotalSeconds > (double)this.UpAllBalanceTime)
                    {
                        this.GetOpenLotteryData(false);
                    }
                    else if (_time.Day != this.ServerData.UpBalanceTime.Day && _time.AddSeconds(-5.0).Day != this.ServerData.UpBalanceTime.Day)
                    {
                        this.GetOpenLotteryData(false);
                    }
                    this.OpenLotterySetWin();
                }
                if (!this.RankResult && _time.Second % 5 == 0)
                {
                    this.GetRank();
                }
                if (this.StageData.Stage <= 2 && (_time - this.ServerData.UpOldOpenTime).Hours > 1)
                {
                    this.ServerData.UpOldOpenTime = TimeUtil.NowDateTime();
                    BoCaiManager.getInstance().OldtterySet(this.BoCaiType, this.ServerOpenData.DataPeriods);
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
        }

        
        private void OpenLotterySetWin()
        {
            try
            {
                if (this.StageData.Stage == 5 && !this.ServerData.IsAward)
                {
                    if (string.IsNullOrEmpty(this.ServerOpenData.strWinNum) || string.IsNullOrEmpty(this.ServerOpenData.WinInfo))
                    {
                        this.GetOpenLotteryData(false);
                    }
                    if (this.FirstDataPeriods == this.ServerOpenData.DataPeriods && this.FirstDataPeriods > 0L)
                    {
                        if (null != this.StartServerOpenData)
                        {
                            if (this.StartServerOpenData.DataPeriods == this.FirstDataPeriods && this.StartServerOpenData.IsAward)
                            {
                                return;
                            }
                        }
                        else if (this.StartServerStage > BoCaiStageEnum.Stage_Open)
                        {
                            return;
                        }
                    }
                    if (this.ServerOpenData.DataPeriods >= 1L && this.ServerOpenData.XiaoHaoDaiBi >= 1 && !string.IsNullOrEmpty(this.ServerOpenData.strWinNum) && !string.IsNullOrEmpty(this.ServerOpenData.WinInfo))
                    {
                        LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_caidaxiao_猜数字]猜数字 开奖 GetOpenLottery su DataPeriods={0}", this.ServerOpenData.DataPeriods), null, true);
                        List<BuyBoCai2SDB> BuyList = new List<BuyBoCai2SDB>();
                        lock (this.mutex)
                        {
                            foreach (PlayerBuyBoCaiData PlayerBuyData in this.BoCaiBaseList)
                            {
                                BuyBoCai2SDB buyItem = new BuyBoCai2SDB
                                {
                                    ZoneID = PlayerBuyData.ZoneID,
                                    m_RoleID = PlayerBuyData.RoleID,
                                    ServerId = PlayerBuyData.ServerId,
                                    strUserID = PlayerBuyData.strUserID,
                                    m_RoleName = PlayerBuyData.RoleName,
                                    DataPeriods = this.ServerOpenData.DataPeriods,
                                    BocaiType = this.BoCaiType,
                                    IsSend = false,
                                    IsWin = false
                                };
                                foreach (BoCaiBuyItem item in PlayerBuyData.BuyItemList)
                                {
                                    if (item.DataPeriods == this.ServerOpenData.DataPeriods)
                                    {
                                        BuyBoCai2SDB temp = new BuyBoCai2SDB();
                                        GlobalNew.Copy<BuyBoCai2SDB>(buyItem, ref temp);
                                        temp.BuyNum = item.BuyNum;
                                        temp.strBuyValue = item.strBuyValue;
                                        BuyList.Add(temp);
                                    }
                                }
                            }
                        }
                        this.ServerOpenData.IsAward = true;
                        this.ServerData.IsAward = true;
                        foreach (BuyBoCai2SDB buyItem in BuyList)
                        {
                            if (!BoCaiManager.getInstance().SendWinItem(this.ServerOpenData, buyItem))
                            {
                                this.ServerOpenData.IsAward = false;
                            }
                        }
                        if (this.ServerOpenData.IsAward)
                        {
                            Global.Send2DB<OpenLottery>(2084, this.ServerOpenData, 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
        }

        
        public bool SetStageData(KFStageData data, bool isKF = true)
        {
            try
            {
                bool isOpen = false;
                bool isActivity = false;
                bool isChangeStage = false;
                lock (this.mutex)
                {
                    if (null == data)
                    {
                        return false;
                    }
                    if (data.Stage != this.StageData.Stage)
                    {
                        isOpen = (this.StageData.isOpenDay && data.Stage == 5);
                        isChangeStage = true;
                    }
                    isActivity = (this.StageData.isOpen != data.isOpen);
                    this.StageData = data;
                    this.ServerData.GetStageDataTime = TimeUtil.NowDateTime();
                }
                if (isOpen)
                {
                    this.OpenLotterySetWin();
                    this.GetRank();
                }
                if (isChangeStage && (this.StageData.Stage == 5 || this.StageData.Stage == 3 || this.StageData.Stage == 2))
                {
                    if (this.ServerOpenData.DataPeriods < 0L || (this.StageData.Stage == 2 && !string.IsNullOrEmpty(this.ServerOpenData.strWinNum)))
                    {
                        this.GetOpenLotteryData(false);
                    }
                    this.UpdateBoCai();
                }
                if (isActivity)
                {
                    this.PriorityActivity(null);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
            return false;
        }

        
        public bool SetOpenLotteryData(OpenLottery OpenData, bool init = false, bool isKF = false)
        {
            try
            {
                lock (this.mutex)
                {
                    if (null != OpenData)
                    {
                        if (this.FirstDataPeriods < 1L)
                        {
                            this.FirstDataPeriods = OpenData.DataPeriods;
                        }
                        if (OpenData.DataPeriods < 1L || OpenData.XiaoHaoDaiBi < 1)
                        {
                            if (this.StageData.Stage > 1)
                            {
                                LogManager.WriteLog(LogTypes.Error, string.Format("[ljl_caidaxiao_猜数字] DataPeriods = {0},XiaoHaoDaiBi={1} ", OpenData.DataPeriods, OpenData.XiaoHaoDaiBi), null, true);
                            }
                            return false;
                        }
                        if (this.ServerOpenData.DataPeriods < 1L)
                        {
                            if (!this.LoadBuyList(OpenData.DataPeriods))
                            {
                                return false;
                            }
                        }
                        if (this.ServerOpenData.DataPeriods < OpenData.DataPeriods && this.ServerOpenData.DataPeriods > 1L && !init)
                        {
                            this.startNewGame(OpenData, false);
                            return true;
                        }
                        if (this.ServerOpenData.DataPeriods < OpenData.DataPeriods)
                        {
                        }
                        this.ServerOpenData = OpenData;
                        this.ServerData.UpBalanceTime = TimeUtil.NowDateTime();
                        if (this.FirstDataPeriods == OpenData.DataPeriods && this.StartServerStage > BoCaiStageEnum.Stage_Open)
                        {
                            if (null != this.StartServerOpenData)
                            {
                                if (this.StartServerOpenData.DataPeriods != this.FirstDataPeriods || !this.StartServerOpenData.IsAward)
                                {
                                    Global.Send2DB<OpenLottery>(2084, OpenData, 0);
                                }
                            }
                        }
                        else
                        {
                            Global.Send2DB<OpenLottery>(2084, OpenData, 0);
                        }
                        if (isKF && this.StageData.Stage == 2)
                        {
                            this.UpdateBoCai();
                        }
                        return true;
                    }
                    else
                    {
                        LogManager.WriteLog(LogTypes.Error, "[ljl_caidaxiao_猜数字] 猜数字 TcpCall.KFBoCaiManager.GetOpenLottery = null", null, true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
            return false;
        }

        
        public bool IsCanBuy()
        {
            bool result;
            lock (this.mutex)
            {
                result = (this.StageData.isOpenDay && this.StageData.Stage == 2 && this.ServerOpenData.DataPeriods > 1L);
            }
            return result;
        }

        
        public BuyBoCai2SDB BuyBocai(GameClient client, int buyNum, string BuyVal, ref int allNum)
        {
            BuyBoCai2SDB DbData = null;
            try
            {
                lock (this.mutex)
                {
                    PlayerBuyBoCaiData playerBoCai = this.BoCaiBaseList.Find((PlayerBuyBoCaiData x) => x.RoleID == client.ClientData.RoleID);
                    if (null == playerBoCai)
                    {
                        playerBoCai = new PlayerBuyBoCaiData();
                        playerBoCai.RoleID = client.ClientData.RoleID;
                        playerBoCai.RoleName = client.ClientData.RoleName;
                        playerBoCai.ZoneID = client.ClientData.ZoneID;
                        playerBoCai.strUserID = client.strUserID;
                        playerBoCai.ServerId = client.ServerId;
                        playerBoCai.BuyItemList = new List<BoCaiBuyItem>();
                        BoCaiBuyItem item = new BoCaiBuyItem
                        {
                            BuyNum = buyNum,
                            strBuyValue = BuyVal,
                            DataPeriods = this.ServerOpenData.DataPeriods
                        };
                        playerBoCai.BuyItemList.Add(item);
                        this.BoCaiBaseList.Add(playerBoCai);
                    }
                    else
                    {
                        BoCaiBuyItem item = playerBoCai.BuyItemList.Find((BoCaiBuyItem x) => x.strBuyValue.Equals(BuyVal));
                        if (null == item)
                        {
                            item = new BoCaiBuyItem
                            {
                                BuyNum = buyNum,
                                strBuyValue = BuyVal,
                                DataPeriods = this.ServerOpenData.DataPeriods
                            };
                            playerBoCai.BuyItemList.Add(item);
                        }
                        else
                        {
                            item.BuyNum += buyNum;
                            allNum = item.BuyNum;
                        }
                    }
                    DbData = new BuyBoCai2SDB();
                    DbData.m_RoleID = playerBoCai.RoleID;
                    DbData.m_RoleName = playerBoCai.RoleName;
                    DbData.ZoneID = playerBoCai.ZoneID;
                    DbData.strUserID = playerBoCai.strUserID;
                    DbData.ServerId = playerBoCai.ServerId;
                    DbData.BuyNum = buyNum;
                    DbData.strBuyValue = BuyVal;
                    DbData.BocaiType = this.BoCaiType;
                    DbData.DataPeriods = this.ServerOpenData.DataPeriods;
                    DbData.IsSend = false;
                    DbData.IsWin = false;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
            return DbData;
        }

        
        public void OpenGetBoCai(int roleid, ref GetBoCaiResult mgsData)
        {
            try
            {
                lock (this.mutex)
                {
                    if (null == this.OpenHistory.Find((OpenLottery x) => x.DataPeriods == this.ServerOpenData.DataPeriods))
                    {
                        this.CopyBuyList(out mgsData.ItemList, roleid);
                    }
                    this.CopyBuyHistoryList(ref mgsData.ItemList, roleid);
                    mgsData.NowPeriods = this.ServerOpenData.DataPeriods;
                    mgsData.IsOpen = (this.StageData.Stage > 1);
                    mgsData.Value1 = this.ServerOpenData.AllBalance.ToString();
                    mgsData.Stage = this.StageData.Stage;
                    mgsData.OpenHistory = new List<BoCaiOpenHistory>();
                    if (this.StageData.isOpenDay)
                    {
                        mgsData.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddMilliseconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
                    }
                    else
                    {
                        mgsData.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddSeconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
                    }
                    BoCaiHelper.CopyHistoryData(this.WinHistory, out mgsData.WinLotteryRoleList);
                    if (null != this.OpenHistory)
                    {
                        foreach (OpenLottery item in this.OpenHistory)
                        {
                            BoCaiOpenHistory data = new BoCaiOpenHistory();
                            data.DataPeriods = item.DataPeriods;
                            data.OpenValue = item.strWinNum;
                            mgsData.OpenHistory.Add(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
        }

        
        public void UpdateBoCai()
        {
            try
            {
                lock (this.mutex)
                {
                    BoCaiUpdate data = new BoCaiUpdate();
                    data.BocaiType = this.BoCaiType;
                    data.Value1 = this.ServerOpenData.AllBalance.ToString();
                    data.DataPeriods = this.ServerOpenData.DataPeriods;
                    data.Stage = this.StageData.Stage;
                    if (this.StageData.isOpenDay)
                    {
                        data.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddMilliseconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
                    }
                    else
                    {
                        data.OpenTime = TimeUtil.GetDiffTimeSeconds(this.ServerData.GetStageDataTime.AddSeconds((double)this.StageData.OpenTime), TimeUtil.NowDateTime(), true);
                    }
                    FunctionSendManager.GetInstance().SendMsg<BoCaiUpdate>(FunctionType.CaiShuZi, 2084, data);
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
        }

        
        public void CopyBuyList(out List<BoCaiBuyItem> itemList, int roleID)
        {
            itemList = new List<BoCaiBuyItem>();
            try
            {
                lock (this.mutex)
                {
                    PlayerBuyBoCaiData playerBoCai = this.BoCaiBaseList.Find((PlayerBuyBoCaiData x) => x.RoleID == roleID);
                    if (null != playerBoCai)
                    {
                        foreach (BoCaiBuyItem item in playerBoCai.BuyItemList)
                        {
                            BoCaiBuyItem temp = new BoCaiBuyItem
                            {
                                BuyNum = item.BuyNum,
                                strBuyValue = item.strBuyValue,
                                DataPeriods = item.DataPeriods
                            };
                            itemList.Add(temp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
        }

        
        public void CopyBuyHistoryList(ref List<BoCaiBuyItem> itemList, int roleID)
        {
            try
            {
                lock (this.mutex)
                {
                    foreach (List<RoleBuyHistory> item in this.BuyItemHistoryDict.Values)
                    {
                        RoleBuyHistory hostory = item.Find((RoleBuyHistory x) => x.RoleID == roleID);
                        if (null != hostory)
                        {
                            foreach (BoCaiBuyItem buyitem in hostory.BuyItemList)
                            {
                                itemList.Add(new BoCaiBuyItem
                                {
                                    BuyNum = buyitem.BuyNum,
                                    strBuyValue = buyitem.strBuyValue,
                                    DataPeriods = buyitem.DataPeriods
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
        }

        
        public int GetXiaoHaoDaiBi()
        {
            int xiaoHaoDaiBi;
            lock (this.mutex)
            {
                xiaoHaoDaiBi = this.ServerOpenData.XiaoHaoDaiBi;
            }
            return xiaoHaoDaiBi;
        }

        
        public long GetDataPeriods()
        {
            long dataPeriods;
            lock (this.mutex)
            {
                dataPeriods = this.ServerOpenData.DataPeriods;
            }
            return dataPeriods;
        }

        
        public void PriorityActivity(GameClient client = null)
        {
            try
            {
                int Activity = 0;
                lock (this.mutex)
                {
                    Activity = Convert.ToInt32(this.StageData.isOpen);
                }
                if (null == client)
                {
                    GameManager.ClientMgr.NotifyAllActivityState(18, Activity, "", "", 0);
                }
                else
                {
                    string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
                    {
                        18,
                        Activity,
                        "",
                        0,
                        0
                    });
                    client.sendCmd(770, strcmd, false);
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_caidaxiao_猜数字]{0}", ex.ToString()), null, true);
            }
        }

        
        private static BoCaiCaiShuZi instance = new BoCaiCaiShuZi();

        
        private bool IsStart;

        
        private bool RankResult = false;

        
        private int UpStageTime = 1800;

        
        private int UpAllBalanceTime = 660;

        
        private object mutex = new object();

        
        private int BoCaiType = 2;

        
        private KFStageData StageData = new KFStageData();

        
        private OpenLottery ServerOpenData = new OpenLottery();

        
        private long FirstDataPeriods = 0L;

        
        private OpenLottery StartServerOpenData = null;

        
        private BoCaiStageEnum StartServerStage = BoCaiStageEnum.Stage_Err;

        
        private CenterServerCaiShuZi ServerData = new CenterServerCaiShuZi();

        
        private List<OpenLottery> OpenHistory = new List<OpenLottery>();

        
        private List<KFBoCaoHistoryData> WinHistory = new List<KFBoCaoHistoryData>();

        
        private List<PlayerBuyBoCaiData> BoCaiBaseList = new List<PlayerBuyBoCaiData>();

        
        private Dictionary<long, List<RoleBuyHistory>> BuyItemHistoryDict = new Dictionary<long, List<RoleBuyHistory>>();
    }
}
