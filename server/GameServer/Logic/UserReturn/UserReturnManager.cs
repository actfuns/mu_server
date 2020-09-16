using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.UserReturn
{
    
    public class UserReturnManager : ICmdProcessorEx, ICmdProcessor
    {
        
        public static UserReturnManager getInstance()
        {
            return UserReturnManager.instance;
        }

        
        public bool initialize()
        {
            return this.initConfigInfo();
        }

        
        public bool startup()
        {
            TCPCmdDispatcher.getInstance().registerProcessorEx(900, 1, 1, UserReturnManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(901, 2, 2, UserReturnManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(902, 3, 3, UserReturnManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
            TCPCmdDispatcher.getInstance().registerProcessorEx(903, 1, 1, UserReturnManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
            return true;
        }

        
        public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                switch (nID)
                {
                    case 900:
                        return this.ProCmdReturnData(client, nID, bytes, cmdParams);
                    case 901:
                        return this.ProCmdReturnCheck(client, nID, bytes, cmdParams);
                    case 902:
                        return this.ProCmdReturnAward(client, nID, bytes, cmdParams);
                    case 903:
                        return this.ProCmdReturnXml(client, nID, bytes, cmdParams);
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteException(ex.ToString());
                client.sendCmd(1373, -11003, false);
            }
            return true;
        }

        
        public bool ProCmdReturnData(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
                {
                    return false;
                }
                UserReturnData result = this.GetUserReturnData(client);
                client.sendCmd<UserReturnData>(nID, result, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        
        public bool ProCmdReturnCheck(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
                {
                    return false;
                }
                string code = cmdParams[1];
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        
        public bool ProCmdReturnAward(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 3))
                {
                    return false;
                }
                int awardType = Convert.ToInt32(cmdParams[0]);
                int awardID = Convert.ToInt32(cmdParams[1]);
                int awardCount = Convert.ToInt32(cmdParams[2]);
                string result = "{0}:{1}:{2}";
                string awardData = "0";
                EReturnAwardState state = this.ReturnAward(client, awardType, awardID, awardCount);
                if (state == EReturnAwardState.Succ)
                {
                    UserReturnData myData = this.GetUserReturnData(client);
                    if (myData.AwardDic.ContainsKey(awardType))
                    {
                        awardData = string.Join<int>("*", myData.AwardDic[awardType]);
                    }
                }
                result = string.Format(result, (int)state, awardType, awardData);
                client.sendCmd(nID, result, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        
        public bool ProCmdReturnXml(GameClient client, int nID, byte[] bytes, string[] cmdParams)
        {
            try
            {
                if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
                {
                    return false;
                }
                client.sendCmd<UserReturnXmlData>(nID, this._xmlData, false);
                return true;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
            }
            return false;
        }

        
        private UserReturnData GetUserReturnData(GameClient client)
        {
            UserReturnData result;
            lock (this.Mutex)
            {
                UserReturnData myData = client.ClientData.ReturnData;
                if (myData != null && (myData.ActivityIsOpen != this._returnActivityInfo.IsOpen || myData.ActivityDay != this._returnActivityInfo.ActivityDay))
                {
                    this.initUserReturnData(client);
                    myData = client.ClientData.ReturnData;
                }
                result = myData;
            }
            return result;
        }

        
        private EReturnAwardState ReturnAward(GameClient client, int awardType, int awardID, int awardCount)
        {
            EReturnAwardState result;
            lock (this.Mutex)
            {
                if (!this._returnActivityInfo.IsOpen || TimeUtil.NowDateTime() >= this._returnActivityInfo.TimeEnd)
                {
                    result = EReturnAwardState.ENoOpen;
                }
                else
                {
                    UserReturnData myData = this.GetUserReturnData(client);
                    if (myData.ReturnState != 2)
                    {
                        result = EReturnAwardState.ENoReturn;
                    }
                    else
                    {
                        switch (awardType)
                        {
                            case 1:
                                result = EReturnAwardState.EFail;
                                break;
                            case 2:
                                result = this.AwardReturn(client, myData, awardID);
                                break;
                            case 3:
                                result = this.AwardCheck(client, myData, awardID);
                                break;
                            case 4:
                                result = this.AwardShop(client, myData, awardID, awardCount);
                                break;
                            case 5:
                                result = this.AwardChongZhi(client, myData, awardID);
                                break;
                            default:
                                result = EReturnAwardState.EFail;
                                break;
                        }
                    }
                }
            }
            return result;
        }

        
        private EReturnAwardState AwardReturn(GameClient client, UserReturnData myData, int awardID)
        {
            EReturnAwardState result2;
            lock (this.Mutex)
            {
                bool isNew = false;
                int awardType = 2;
                ReturnAwardInfo awardInfo = null;
                if (!myData.AwardDic.ContainsKey(awardType))
                {
                    isNew = true;
                    IOrderedEnumerable<ReturnAwardInfo> tReturn = from info in this._returnAwardDic.Values
                                                                  orderby info.Vip
                                                                  select info;
                    if (tReturn.Any<ReturnAwardInfo>())
                    {
                        awardInfo = tReturn.First<ReturnAwardInfo>();
                    }
                }
                else
                {
                    int oldID = myData.AwardDic[awardType][0];
                    IOrderedEnumerable<ReturnAwardInfo> tReturn = from info in this._returnAwardDic.Values
                                                                  where info.ID > oldID
                                                                  orderby info.Vip
                                                                  select info;
                    if (tReturn.Any<ReturnAwardInfo>())
                    {
                        awardInfo = tReturn.First<ReturnAwardInfo>();
                    }
                }
                if (awardInfo == null || awardInfo.ID != awardID)
                {
                    result2 = EReturnAwardState.EFail;
                }
                else if (client.ClientData.VipLevel < awardInfo.Vip)
                {
                    result2 = EReturnAwardState.EVip;
                }
                else
                {
                    List<GoodsData> awardList = new List<GoodsData>();
                    if (awardInfo.DefaultGoodsList != null)
                    {
                        awardList.AddRange(awardInfo.DefaultGoodsList);
                    }
                    List<GoodsData> proGoods = GoodsHelper.GetAwardPro(client, awardInfo.ProGoodsList);
                    if (proGoods != null)
                    {
                        awardList.AddRange(proGoods);
                    }
                    if (!Global.CanAddGoodsDataList(client, awardList))
                    {
                        result2 = EReturnAwardState.ENoBag;
                    }
                    else
                    {
                        string award = awardInfo.ID.ToString();
                        bool result = this.DBUserReturnAwardUpdate(client, awardType, award);
                        if (result)
                        {
                            if (isNew)
                            {
                                myData.AwardDic.Add(awardType, new int[]
                                {
                                    awardInfo.ID
                                });
                            }
                            else
                            {
                                myData.AwardDic[awardType] = new int[]
                                {
                                    awardInfo.ID
                                };
                            }
                            for (int i = 0; i < awardList.Count; i++)
                            {
                                Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardList[i].GoodsID, awardList[i].GCount, awardList[i].Quality, "", awardList[i].Forge_level, awardList[i].Binding, 0, "", true, 1, "回归奖励", "1900-01-01 12:00:00", awardList[i].AddPropIndex, awardList[i].BornIndex, awardList[i].Lucky, awardList[i].Strong, awardList[i].ExcellenceInfo, awardList[i].AppendPropLev, 0, null, null, 0, true);
                            }
                            this.CheckActivityTip(client);
                            result2 = EReturnAwardState.Succ;
                        }
                        else
                        {
                            result2 = EReturnAwardState.EFail;
                        }
                    }
                }
            }
            return result2;
        }

        
        private EReturnAwardState AwardCheck(GameClient client, UserReturnData myData, int awardID)
        {
            EReturnAwardState result2;
            lock (this.Mutex)
            {
                int awardType = 3;
                ReturnCheckAwardInfo awardInfo = null;
                if (!myData.AwardDic.ContainsKey(awardType))
                {
                    Dictionary<int, int[]> awardDic = myData.AwardDic;
                    int key = awardType;
                    int[] value = new int[1];
                    awardDic[key] = value;
                }
                int oldID = myData.AwardDic[awardType][0];
                IOrderedEnumerable<ReturnCheckAwardInfo> tReturn = from info in this._returnCheckAwardDic.Values
                                                                   where myData.Level >= info.LevelMin && myData.Level <= info.LevelMax && info.ID > oldID
                                                                   orderby info.Day
                                                                   select info;
                if (tReturn.Any<ReturnCheckAwardInfo>())
                {
                    awardInfo = tReturn.First<ReturnCheckAwardInfo>();
                }
                if (awardInfo == null || awardInfo.ID != awardID)
                {
                    result2 = EReturnAwardState.EFail;
                }
                else
                {
                    int spanDay = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(myData.TimeReturn) + 1;
                    if (awardInfo.Day > spanDay)
                    {
                        result2 = EReturnAwardState.EFail;
                    }
                    else
                    {
                        List<GoodsData> awardList = new List<GoodsData>();
                        if (awardInfo.DefaultGoodsList != null)
                        {
                            awardList.AddRange(awardInfo.DefaultGoodsList);
                        }
                        List<GoodsData> proGoods = GoodsHelper.GetAwardPro(client, awardInfo.ProGoodsList);
                        if (proGoods != null)
                        {
                            awardList.AddRange(proGoods);
                        }
                        if (!Global.CanAddGoodsDataList(client, awardList))
                        {
                            result2 = EReturnAwardState.ENoBag;
                        }
                        else
                        {
                            string award = awardInfo.ID.ToString();
                            bool result = this.DBUserReturnAwardUpdate(client, awardType, award);
                            if (result)
                            {
                                myData.AwardDic[awardType][0] = awardInfo.ID;
                                for (int i = 0; i < awardList.Count; i++)
                                {
                                    Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardList[i].GoodsID, awardList[i].GCount, awardList[i].Quality, "", awardList[i].Forge_level, awardList[i].Binding, 0, "", true, 1, "召回签到", "1900-01-01 12:00:00", awardList[i].AddPropIndex, awardList[i].BornIndex, awardList[i].Lucky, awardList[i].Strong, awardList[i].ExcellenceInfo, awardList[i].AppendPropLev, 0, null, null, 0, true);
                                }
                                this.CheckActivityTip(client);
                                result2 = EReturnAwardState.Succ;
                            }
                            else
                            {
                                result2 = EReturnAwardState.EFail;
                            }
                        }
                    }
                }
            }
            return result2;
        }

        
        private EReturnAwardState AwardShop(GameClient client, UserReturnData myData, int awardID, int awardCount)
        {
            EReturnAwardState result2;
            lock (this.Mutex)
            {
                int awardType = 4;
                bool isNew = true;
                int oldCount = 0;
                int[] oldArr = null;
                if (myData.AwardDic.ContainsKey(awardType))
                {
                    isNew = false;
                    oldArr = myData.AwardDic[awardType];
                    for (int i = 0; i < oldArr.Length; i++)
                    {
                        if (oldArr[i++] == awardID)
                        {
                            oldCount = oldArr[i];
                            break;
                        }
                    }
                }
                ReturnShopAwardInfo awardInfo = this.GetReturnShopAwardInfo(awardID);
                if (awardInfo == null)
                {
                    result2 = EReturnAwardState.EFail;
                }
                else if (oldCount + awardCount > awardInfo.LimitCount)
                {
                    result2 = EReturnAwardState.EShopMax;
                }
                else
                {
                    int priceNeed = awardInfo.NewPrice * awardCount;
                    if (priceNeed > client.ClientData.UserMoney)
                    {
                        result2 = EReturnAwardState.ENoMoney;
                    }
                    else if (!Global.CanAddGoodsNum(client, awardCount))
                    {
                        result2 = EReturnAwardState.ENoBag;
                    }
                    else if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, priceNeed, "召回商店", true, false, false, DaiBiSySType.None))
                    {
                        result2 = EReturnAwardState.ENoMoney;
                    }
                    else
                    {
                        List<int> list = new List<int>();
                        if (isNew)
                        {
                            list.Add(awardID);
                            list.Add(awardCount);
                        }
                        else
                        {
                            bool isAdd = false;
                            for (int i = 0; i < oldArr.Length; i++)
                            {
                                int id = oldArr[i++];
                                int count = oldArr[i];
                                if (id == awardID)
                                {
                                    isAdd = true;
                                    count += awardCount;
                                }
                                list.Add(id);
                                list.Add(count);
                            }
                            if (!isAdd)
                            {
                                list.Add(awardID);
                                list.Add(awardCount);
                            }
                        }
                        string award = string.Join<int>("*", list.ToArray<int>());
                        bool result = this.DBUserReturnAwardUpdate(client, awardType, award);
                        if (result)
                        {
                            if (isNew)
                            {
                                myData.AwardDic.Add(awardType, list.ToArray<int>());
                            }
                            else
                            {
                                myData.AwardDic[awardType] = list.ToArray<int>();
                            }
                            Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardInfo.Goods.GoodsID, awardInfo.Goods.GCount, awardInfo.Goods.Quality, "", awardInfo.Goods.Forge_level, awardInfo.Goods.Binding, 0, "", true, awardCount, "召回商店", "1900-01-01 12:00:00", awardInfo.Goods.AddPropIndex, awardInfo.Goods.BornIndex, awardInfo.Goods.Lucky, awardInfo.Goods.Strong, awardInfo.Goods.ExcellenceInfo, awardInfo.Goods.AppendPropLev, 0, null, null, 0, true);
                            result2 = EReturnAwardState.Succ;
                        }
                        else
                        {
                            result2 = EReturnAwardState.EFail;
                        }
                    }
                }
            }
            return result2;
        }

        
        private EReturnAwardState AwardChongZhi(GameClient client, UserReturnData myData, int awardID)
        {
            bool flag = false;
            EReturnAwardState result2;
            lock (this.Mutex)
            {
                int awardType = 5;
                ReturnChonZhiAwardInfo awardInfo = null;
                if (!myData.AwardDic.ContainsKey(awardType))
                {
                    Dictionary<int, int[]> awardDic = myData.AwardDic;
                    int key = awardType;
                    int[] value = new int[1];
                    awardDic[key] = value;
                }
                int oldID = myData.AwardDic[awardType][0];
                IEnumerable<ReturnChonZhiAwardInfo> tReturn = from info in this._recallChongZhiAwardDict.Values
                                                              where myData.LeiJiChongZhi >= info.MinYuanBao && info.ID > oldID
                                                              select info;
                if (tReturn.Any<ReturnChonZhiAwardInfo>())
                {
                    awardInfo = tReturn.First<ReturnChonZhiAwardInfo>();
                }
                if (awardInfo == null || awardInfo.ID != awardID)
                {
                    result2 = EReturnAwardState.EFail;
                }
                else
                {
                    List<GoodsData> awardList = new List<GoodsData>();
                    if (awardInfo.DefaultGoodsList != null)
                    {
                        awardList.AddRange(awardInfo.DefaultGoodsList);
                    }
                    List<GoodsData> proGoods = GoodsHelper.GetAwardPro(client, awardInfo.ProGoodsList);
                    if (proGoods != null)
                    {
                        awardList.AddRange(proGoods);
                    }
                    if (!Global.CanAddGoodsDataList(client, awardList))
                    {
                        result2 = EReturnAwardState.ENoBag;
                    }
                    else
                    {
                        string award = awardInfo.ID.ToString();
                        bool result = this.DBUserReturnAwardUpdate(client, awardType, award);
                        if (result)
                        {
                            myData.AwardDic[awardType][0] = awardInfo.ID;
                            for (int i = 0; i < awardList.Count; i++)
                            {
                                Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardList[i].GoodsID, awardList[i].GCount, awardList[i].Quality, "", awardList[i].Forge_level, awardList[i].Binding, 0, "", true, 1, "召回累充", "1900-01-01 12:00:00", awardList[i].AddPropIndex, awardList[i].BornIndex, awardList[i].Lucky, awardList[i].Strong, awardList[i].ExcellenceInfo, awardList[i].AppendPropLev, 0, null, null, 0, true);
                            }
                            this.CheckActivityTip(client);
                            result2 = EReturnAwardState.Succ;
                        }
                        else
                        {
                            result2 = EReturnAwardState.EFail;
                        }
                    }
                }
            }
            return result2;
        }

        
        public bool initConfigInfo()
        {
            lock (this.Mutex)
            {
                this.InitXmlData();
                this.LoadReturnActivityInfo();
                this.LoadReturnAwardInfo();
                this.LoadReturnCheckAwardInfo();
                this.LoadReturnShopAwardInfo();
                this.LoadReturnChongZhiGiftInfo();
            }
            return true;
        }

        
        private void InitXmlData()
        {
            try
            {
                if (this._xmlData.XmlList == null)
                {
                    this._xmlData.XmlList = new List<string>();
                }
                if (this._xmlData.XmlNameList == null)
                {
                    this._xmlData.XmlNameList = new List<string>();
                }
                this._xmlData.XmlList.Clear();
                this._xmlData.XmlNameList.Clear();
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.Fatal, "初始化老玩家数据时出现异常!!!", ex, true);
            }
        }

        
        private void LoadReturnActivityInfo()
        {
            string fileName = Global.IsolateResPath("Config/PlayerRecall/HuoDongZhaoHui.xml");
            XElement xml = CheckHelper.LoadXml(fileName, true);
            if (null != xml)
            {
                try
                {
                    this._xmlData.XmlNameList.Add("HuoDongZhaoHui.xml");
                    this._xmlData.XmlList.Add(File.ReadAllText(fileName));
                    this._returnActivityInfo = new ReturnActivityInfo();
                    IEnumerable<XElement> xmlItems = xml.Elements();
                    foreach (XElement xmlItem in xmlItems)
                    {
                        if (xmlItem != null)
                        {
                            this._returnActivityInfo.ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
                            this._returnActivityInfo.ActivityID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "HuoDongID", "0"));
                            this._returnActivityInfo.TimeBegin = DateTime.Parse(Global.GetDefAttributeStr(xmlItem, "BeginTime", "1970-01-01 00:00:00"));
                            this._returnActivityInfo.TimeEnd = DateTime.Parse(Global.GetDefAttributeStr(xmlItem, "FinishTime", "1970-01-01 00:00:00"));
                            this._returnActivityInfo.TimeBeginNoLogin = DateTime.Parse(Global.GetDefAttributeStr(xmlItem, "NotLoggedInBegin", "1970-01-01 00:00:00"));
                            this._returnActivityInfo.TimeEndNoLogin = DateTime.Parse(Global.GetDefAttributeStr(xmlItem, "NotLoggedInFinish", "1970-01-01 00:00:00"));
                            string levelStr = Global.GetDefAttributeStr(xmlItem, "Level", "0,0");
                            string[] levelArr = levelStr.Split(new char[]
                            {
                                ','
                            });
                            this._returnActivityInfo.Level = int.Parse(levelArr[0]) * 100 + int.Parse(levelArr[1]);
                            this._returnActivityInfo.Vip = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "VIP", "4"));
                            if (TimeUtil.NowDateTime() >= this._returnActivityInfo.TimeBegin && TimeUtil.NowDateTime() < this._returnActivityInfo.TimeEnd)
                            {
                                this._returnActivityInfo.IsOpen = true;
                                this._returnActivityInfo.ActivityDay = this._returnActivityInfo.TimeBegin.ToString("yyyy-MM-dd");
                                this._returnActivityInfo.TimeBeginStr = this._returnActivityInfo.TimeBegin.ToString("yyyy-MM-dd HH:mm:ss");
                                this._returnActivityInfo.TimeEndStr = this._returnActivityInfo.TimeEnd.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            this.DBReturnIsOpen();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogTypes.Fatal, "加载IsolateRes/Config/PlayerRecall/HuoDongZhaoHui.xml时出现异常!!!", ex, true);
                }
            }
        }

        
        public bool IsUserReturnOpen()
        {
            return this._returnActivityInfo.IsOpen;
        }

        
        private void LoadReturnAwardInfo()
        {
            string fileName = Global.IsolateResPath("Config/PlayerRecall/OldLogin.xml");
            XElement xml = CheckHelper.LoadXml(fileName, true);
            if (null != xml)
            {
                try
                {
                    this._xmlData.XmlNameList.Add("OldLogin.xml");
                    this._xmlData.XmlList.Add(File.ReadAllText(fileName));
                    this._returnAwardDic = new Dictionary<int, ReturnAwardInfo>();
                    IEnumerable<XElement> xmlItems = xml.Elements();
                    foreach (XElement xmlItem in xmlItems)
                    {
                        ReturnAwardInfo info = new ReturnAwardInfo();
                        info.ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
                        info.Vip = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MinVip", "0"));
                        string goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID1");
                        if (!string.IsNullOrEmpty(goods))
                        {
                            string[] fields = goods.Split(new char[]
                            {
                                '|'
                            });
                            if (fields.Length > 0)
                            {
                                info.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
                            }
                        }
                        goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID2");
                        if (!string.IsNullOrEmpty(goods))
                        {
                            string[] fields = goods.Split(new char[]
                            {
                                '|'
                            });
                            if (fields.Length > 0)
                            {
                                info.ProGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
                            }
                        }
                        this._returnAwardDic.Add(info.ID, info);
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogTypes.Fatal, "加载IsolateRes/Config/PlayerRecall/OldLogin.xml时出现异常!!!", ex, true);
                }
            }
        }

        
        private void LoadReturnCheckAwardInfo()
        {
            string fileName = Global.IsolateResPath("Config/PlayerRecall/OldHuoDongLoginNumGift.xml");
            XElement xml = CheckHelper.LoadXml(fileName, true);
            if (null != xml)
            {
                try
                {
                    this._xmlData.XmlNameList.Add("OldHuoDongLoginNumGift.xml");
                    this._xmlData.XmlList.Add(File.ReadAllText(fileName));
                    this._returnCheckAwardDic = new Dictionary<int, ReturnCheckAwardInfo>();
                    IEnumerable<XElement> xmlItems = xml.Elements();
                    foreach (XElement xmlItem in xmlItems)
                    {
                        ReturnCheckAwardInfo info = new ReturnCheckAwardInfo();
                        info.ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
                        string levelStr = Global.GetDefAttributeStr(xmlItem, "Level", "");
                        if (!string.IsNullOrEmpty(levelStr))
                        {
                            string[] levelArr = levelStr.Split(new char[]
                            {
                                '|'
                            });
                            string[] levelArr2 = levelArr[0].Split(new char[]
                            {
                                ','
                            });
                            info.LevelMin = Convert.ToInt32(levelArr2[0]) * 100 + Convert.ToInt32(levelArr2[1]);
                            levelArr2 = levelArr[1].Split(new char[]
                            {
                                ','
                            });
                            info.LevelMax = Convert.ToInt32(levelArr2[0]) * 100 + Convert.ToInt32(levelArr2[1]);
                            info.Day = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "TimeOl", "0"));
                            string goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID1");
                            if (!string.IsNullOrEmpty(goods))
                            {
                                string[] fields = goods.Split(new char[]
                                {
                                    '|'
                                });
                                if (fields.Length > 0)
                                {
                                    info.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
                                }
                            }
                            goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID2");
                            if (!string.IsNullOrEmpty(goods))
                            {
                                string[] fields = goods.Split(new char[]
                                {
                                    '|'
                                });
                                if (fields.Length > 0)
                                {
                                    info.ProGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
                                }
                            }
                            this._returnCheckAwardDic.Add(info.ID, info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogTypes.Fatal, "加载IsolateRes/Config/PlayerRecall/OldHuoDongLoginNumGift.xml时出错!!!文件不存在", null, true);
                }
            }
        }

        
        private void LoadReturnShopAwardInfo()
        {
            string fileName = Global.IsolateResPath("Config/PlayerRecall/OldStore.xml");
            XElement xml = CheckHelper.LoadXml(fileName, true);
            if (null != xml)
            {
                try
                {
                    this._xmlData.XmlNameList.Add("OldStore.xml");
                    this._xmlData.XmlList.Add(File.ReadAllText(fileName));
                    this._returnShopAwardDic = new Dictionary<int, ReturnShopAwardInfo>();
                    IEnumerable<XElement> xmlItems = xml.Elements();
                    foreach (XElement xmlItem in xmlItems)
                    {
                        ReturnShopAwardInfo info = new ReturnShopAwardInfo();
                        info.ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
                        info.OldPrice = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "OrigPrice", "0"));
                        info.NewPrice = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Price", "0"));
                        info.LimitCount = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SinglePurchase", "0"));
                        string goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID");
                        if (!string.IsNullOrEmpty(goods))
                        {
                            info.Goods = GoodsHelper.ParseGoodsData(goods, fileName);
                        }
                        this._returnShopAwardDic.Add(info.ID, info);
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogTypes.Fatal, "加载IsolateRes/Config/PlayerRecall/OldHuoDongLoginNumGift.xml时出错!!!文件不存在", null, true);
                }
            }
        }

        
        public ReturnShopAwardInfo GetReturnShopAwardInfo(int id)
        {
            ReturnShopAwardInfo result;
            if (this._returnShopAwardDic.ContainsKey(id))
            {
                result = this._returnShopAwardDic[id];
            }
            else
            {
                result = null;
            }
            return result;
        }

        
        private void LoadReturnChongZhiGiftInfo()
        {
            string fileName = Global.IsolateResPath("Config/PlayerRecall/OldHuoDongchongzhiGift.xml");
            XElement xml = CheckHelper.LoadXml(fileName, true);
            if (null != xml)
            {
                try
                {
                    this._xmlData.XmlNameList.Add("OldHuoDongchongzhiGift.xml");
                    this._xmlData.XmlList.Add(File.ReadAllText(fileName));
                    this._recallChongZhiAwardDict = new Dictionary<int, ReturnChonZhiAwardInfo>();
                    IEnumerable<XElement> xmlItems = xml.Elements();
                    foreach (XElement xmlItem in xmlItems)
                    {
                        if (xmlItem != null)
                        {
                            ReturnChonZhiAwardInfo info = new ReturnChonZhiAwardInfo
                            {
                                ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
                                MinYuanBao = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MinYuanBao", "0"))
                            };
                            string goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID1");
                            if (!string.IsNullOrEmpty(goods))
                            {
                                string[] fields = goods.Split(new char[]
                                {
                                    '|'
                                });
                                if (fields.Length > 0)
                                {
                                    info.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
                                }
                            }
                            goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID2");
                            if (!string.IsNullOrEmpty(goods))
                            {
                                string[] fields = goods.Split(new char[]
                                {
                                    '|'
                                });
                                if (fields.Length > 0)
                                {
                                    info.ProGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
                                }
                            }
                            this._recallChongZhiAwardDict.Add(info.ID, info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(LogTypes.Fatal, "加载IsolateRes/Config/PlayerRecall/OldHuoDongchongzhiGift.xml时出错!!!文件不存在", ex, true);
                }
            }
        }

        
        public void initUserReturnData(GameClient client)
        {
            lock (this.Mutex)
            {
                UserReturnData oldData = client.ClientData.ReturnData;
                UserReturnData newData = new UserReturnData();
                if (this._returnActivityInfo.IsOpen)
                {
                    newData.ActivityIsOpen = this._returnActivityInfo.IsOpen;
                    newData.ActivityID = this._returnActivityInfo.ActivityID;
                    newData.ActivityDay = this._returnActivityInfo.ActivityDay;
                    newData.TimeBegin = this._returnActivityInfo.TimeBegin;
                    newData.TimeEnd = this._returnActivityInfo.TimeEnd;
                    newData.TimeAward = this._returnActivityInfo.TimeEnd;
                    int myPlatform = (int)GameCoreInterface.getinstance().GetPlatformType();
                    newData.MyCode = string.Format("{0}#{1}#{2}", StringUtil.IDToCode(myPlatform), StringUtil.IDToCode(client.ClientData.ZoneID), StringUtil.IDToCode(client.ClientData.RoleID));
                    this.initReturnData(client, newData, oldData);
                    newData.AwardDic = this.DBUserReturnAwardList(client);
                }
                client.ClientData.ReturnData = newData;
                this.CheckActivityTip(client);
            }
        }

        
        private void initReturnData(GameClient client, UserReturnData newData, UserReturnData oldData)
        {
            lock (this.Mutex)
            {
                string dbData = string.Format("{0}:{1}:{2}", client.strUserID, client.ClientData.ZoneID, TimeUtil.NowDateTime().ToString("yyyy-MM-dd"));
                Global.sendToDB<int, string>(13107, dbData, 0);
                ReturnData data = this.DBUserReturnDataGet(client);
                if (data != null)
                {
                    int platform = (int)GameCoreInterface.getinstance().GetPlatformType();
                    newData.RecallCode = string.Format("{0}#{1}#{2}", StringUtil.IDToCode(platform), StringUtil.IDToCode(data.PZoneID), StringUtil.IDToCode(data.PRoleID));
                    newData.RecallZoneID = data.PZoneID;
                    newData.RecallRoleID = data.PRoleID;
                    newData.Level = data.Level;
                    newData.Vip = data.Vip;
                    newData.TimeReturn = data.LogTime;
                    if (data.Level % 100 == 0)
                    {
                        newData.ZhuanSheng = data.Level / 100 - 1;
                        newData.DengJi = 100;
                    }
                    else
                    {
                        newData.ZhuanSheng = data.Level / 100;
                        newData.DengJi = data.Level % 100;
                    }
                    newData.LeiJiChongZhi = data.LeiJiChongZhi;
                    switch (data.StateCheck)
                    {
                        case 0:
                            newData.ReturnState = -7;
                            break;
                        case 1:
                            newData.ReturnState = 2;
                            break;
                    }
                    if ((oldData == null && newData != null && newData.ReturnState == 2) || (oldData != null && oldData.ActivityDay != newData.ActivityDay && newData.ReturnState == 2))
                    {
                        string broadcastMsg = StringUtil.substitute(GLang.GetLang(555, new object[0]), new object[]
                        {
                            client.ClientData.RoleName
                        });
                        Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.HintMsg, broadcastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
                    }
                }
            }
        }

        
        private void ReturnDicAdd(List<ReturnData> list)
        {
            foreach (ReturnData data in list)
            {
                this._returnDic[data.CRoleID] = data;
            }
        }

        
        public void DBReturnIsOpen()
        {
            int vipLevel = Global.Clamp(this._returnActivityInfo.Vip, 0, Data.VIPLevAwardAndExpInfoList.Count - 1);
            ReturnActivity data = new ReturnActivity
            {
                IsOpen = this._returnActivityInfo.IsOpen,
                NotLoggedInBegin = this._returnActivityInfo.TimeBeginNoLogin,
                NotLoggedInFinish = this._returnActivityInfo.TimeEndNoLogin,
                Level = this._returnActivityInfo.Level,
                VIPNeedExp = ((vipLevel == 0) ? 0 : Data.VIPLevAwardAndExpInfoList[vipLevel].NeedExp),
                ActivityID = this._returnActivityInfo.ActivityID,
                ActivityDay = this._returnActivityInfo.ActivityDay
            };
            Global.sendToDB<int, ReturnActivity>(13100, data, 0);
        }

        
        public ReturnData DBUserReturnDataGet(GameClient client)
        {
            string cmd2db = string.Format("{0}:{1}", client.strUserID, client.ClientData.ZoneID);
            ReturnData result = Global.sendToDB<ReturnData, string>(13101, cmd2db, client.ServerId);
            this.ReturnDicAdd(new List<ReturnData>
            {
                result
            });
            return result;
        }

        
        public List<ReturnData> DBUserReturnDataList(GameClient client)
        {
            string cmd2db = string.Format("{0}:{1}:{2}:{3}", new object[]
            {
                this._returnActivityInfo.ActivityDay,
                this._returnActivityInfo.ActivityID,
                client.ClientData.ZoneID,
                client.ClientData.RoleID
            });
            List<ReturnData> result = Global.sendToDB<List<ReturnData>, string>(13104, cmd2db, client.ServerId);
            this.ReturnDicAdd(result);
            return result;
        }

        
        public bool DBUserReturnDataUpdate(GameClient client, ReturnData data)
        {
            return Global.sendToDB<bool, ReturnData>(13102, data, client.ServerId);
        }

        
        public bool DBUserReturnDataDel(GameClient client, ReturnData data)
        {
            return Global.sendToDB<bool, ReturnData>(13103, data, client.ServerId);
        }

        
        public Dictionary<int, int[]> DBUserReturnAwardList(GameClient client)
        {
            string cmd2db = string.Format("{0}:{1}:{2}", this._returnActivityInfo.ActivityDay, this._returnActivityInfo.ActivityID, client.strUserID);
            return Global.sendToDB<Dictionary<int, int[]>, string>(13105, cmd2db, client.ServerId);
        }

        
        public bool DBUserReturnAwardUpdate(GameClient client, int awardType, string award)
        {
            string cmd2db = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
            {
                this._returnActivityInfo.ActivityDay,
                this._returnActivityInfo.ActivityID,
                client.ClientData.ZoneID,
                client.strUserID,
                awardType,
                award
            });
            return Global.sendToDB<bool, string>(13106, cmd2db, client.ServerId);
        }

        
        public void CheckUserReturnOpenState(long ticks)
        {
            if (ticks - this._lastTicks >= 10000L)
            {
                this._lastTicks = ticks;
                this.UpdateUserReturnState();
            }
        }

        
        public void UpdateUserReturnState()
        {
            DateTime nowTime = TimeUtil.NowDateTime();
            if (!this._returnActivityInfo.IsOpen && nowTime >= this._returnActivityInfo.TimeBegin && nowTime < this._returnActivityInfo.TimeEnd)
            {
                this._returnActivityInfo.IsOpen = true;
                this._returnActivityInfo.ActivityDay = this._returnActivityInfo.TimeBegin.ToString("yyyy-MM-dd");
                this._returnActivityInfo.TimeBeginStr = this._returnActivityInfo.TimeBegin.ToString("yyyy-MM-dd HH:mm:ss");
                this._returnActivityInfo.TimeEndStr = this._returnActivityInfo.TimeEnd.ToString("yyyy-MM-dd HH:mm:ss");
                GameManager.ClientMgr.NotifyAllActivityState(3, 1, this._returnActivityInfo.TimeBegin.ToString("yyyyMMddHHmmss"), this._returnActivityInfo.TimeEnd.ToString("yyyyMMddHHmmss"), this._returnActivityInfo.ActivityID);
                this.DBReturnIsOpen();
            }
            if (this._returnActivityInfo.IsOpen && (nowTime > this._returnActivityInfo.TimeEnd || nowTime < this._returnActivityInfo.TimeBegin))
            {
                this.DBReturnIsOpen();
                this._returnActivityInfo.IsOpen = false;
                this._returnActivityInfo.ActivityDay = "";
                this._returnActivityInfo.TimeBeginStr = "";
                this._returnActivityInfo.TimeEndStr = "";
                GameManager.ClientMgr.NotifyAllActivityState(3, 0, "", "", 0);
            }
        }

        
        private DateTime getUserReturnBeginTime()
        {
            string dayBeginStr = GameManager.GameConfigMgr.GetGameConfigItemStr("userbegintime", "");
            DateTime result;
            if (dayBeginStr == "")
            {
                result = DateTime.MinValue;
            }
            else
            {
                DateTime dateTime;
                DateTime.TryParse(dayBeginStr, out dateTime);
                result = dateTime;
            }
            return result;
        }

        
        public void VipChange(GameClient client)
        {
            this.CheckActivityTip(client);
        }

        
        public void CheckActivityTip(GameClient client)
        {
            UserReturnData myData = client.ClientData.ReturnData;
            if (myData == null || myData.ReturnState <= 0)
            {
                string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
                {
                    3,
                    0,
                    "",
                    "",
                    0
                });
                client.sendCmd(770, strcmd, false);
            }
            else
            {
                lock (this.Mutex)
                {
                    bool isTipReturn = false;
                    bool isTipRecall = false;
                    bool isTipChongZhi = false;
                    if (myData != null)
                    {
                        int oldID = 0;
                        if (myData.ReturnState == 2)
                        {
                            int awardType = 2;
                            ReturnAwardInfo returnInfo = null;
                            if (myData.AwardDic.ContainsKey(awardType) && myData.AwardDic[awardType].Length > 0)
                            {
                                oldID = myData.AwardDic[awardType][0];
                            }
                            IOrderedEnumerable<ReturnAwardInfo> tReturn = from info in this._returnAwardDic.Values
                                                                          where info.ID > oldID
                                                                          orderby info.Vip
                                                                          select info;
                            if (tReturn.Any<ReturnAwardInfo>())
                            {
                                returnInfo = tReturn.First<ReturnAwardInfo>();
                            }
                            if (returnInfo != null && client.ClientData.VipLevel >= returnInfo.Vip)
                            {
                                isTipReturn = true;
                                client._IconStateMgr.AddFlushIconState(14102, true);
                            }
                            else
                            {
                                client._IconStateMgr.AddFlushIconState(14102, false);
                            }
                            oldID = 0;
                            awardType = 3;
                            ReturnCheckAwardInfo checkInfo = null;
                            if (myData.AwardDic.ContainsKey(awardType) && myData.AwardDic[awardType].Length > 0)
                            {
                                oldID = myData.AwardDic[awardType][0];
                            }
                            IOrderedEnumerable<ReturnCheckAwardInfo> tCheck = from info in this._returnCheckAwardDic.Values
                                                                              where myData.Level >= info.LevelMin && myData.Level <= info.LevelMax && info.ID > oldID
                                                                              orderby info.Day
                                                                              select info;
                            if (tCheck.Any<ReturnCheckAwardInfo>())
                            {
                                checkInfo = tCheck.First<ReturnCheckAwardInfo>();
                            }
                            int spanDay = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(myData.TimeReturn) + 1;
                            if (checkInfo != null && checkInfo.Day <= spanDay)
                            {
                                isTipReturn = true;
                                client._IconStateMgr.AddFlushIconState(14103, true);
                            }
                            else
                            {
                                client._IconStateMgr.AddFlushIconState(14103, false);
                            }
                            oldID = 0;
                            awardType = 5;
                            ReturnChonZhiAwardInfo chongZhiInfo = null;
                            if (myData.AwardDic.ContainsKey(awardType) && myData.AwardDic[awardType].Length > 0)
                            {
                                oldID = myData.AwardDic[awardType][0];
                            }
                            IEnumerable<ReturnChonZhiAwardInfo> tChong = from info in this._recallChongZhiAwardDict.Values
                                                                         where myData.LeiJiChongZhi >= info.MinYuanBao && info.ID > oldID
                                                                         select info;
                            if (tChong.Any<ReturnChonZhiAwardInfo>())
                            {
                                chongZhiInfo = tChong.First<ReturnChonZhiAwardInfo>();
                            }
                            if (chongZhiInfo != null)
                            {
                                isTipChongZhi = true;
                                client._IconStateMgr.AddFlushIconState(14115, true);
                            }
                            else
                            {
                                client._IconStateMgr.AddFlushIconState(14115, false);
                            }
                        }
                        if (isTipReturn || isTipRecall || isTipChongZhi)
                        {
                            client._IconStateMgr.AddFlushIconState(14100, true);
                        }
                        else
                        {
                            client._IconStateMgr.AddFlushIconState(14100, false);
                        }
                        client._IconStateMgr.SendIconStateToClient(client);
                    }
                }
            }
        }

        
        private const int AWARD_DAY_COUNT = 6;

        
        private const int CHECK_WAIT_HOUR = 1;

        
        private static UserReturnManager instance = new UserReturnManager();

        
        private object Mutex = new object();

        
        public UserReturnXmlData _xmlData = new UserReturnXmlData();

        
        public ReturnActivityInfo _returnActivityInfo = new ReturnActivityInfo();

        
        public Dictionary<int, ReturnAwardInfo> _returnAwardDic = new Dictionary<int, ReturnAwardInfo>();

        
        public Dictionary<int, ReturnCheckAwardInfo> _returnCheckAwardDic = new Dictionary<int, ReturnCheckAwardInfo>();

        
        public Dictionary<int, ReturnShopAwardInfo> _returnShopAwardDic = new Dictionary<int, ReturnShopAwardInfo>();

        
        public Dictionary<int, ReturnChonZhiAwardInfo> _recallChongZhiAwardDict = new Dictionary<int, ReturnChonZhiAwardInfo>();

        
        private Dictionary<int, ReturnData> _returnDic = new Dictionary<int, ReturnData>();

        
        private long _lastTicks = 0L;
    }
}
