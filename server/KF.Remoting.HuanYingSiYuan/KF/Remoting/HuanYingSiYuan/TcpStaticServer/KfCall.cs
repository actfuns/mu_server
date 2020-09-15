using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using AutoCSer;
using AutoCSer.BinarySerialize;
using AutoCSer.IOS;
using AutoCSer.Json;
using AutoCSer.Log;
using AutoCSer.Metadata;
using AutoCSer.Net;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.TcpCall;
using Server.Data;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.HuanYingSiYuan.TcpStaticServer
{
    // Token: 0x0200008F RID: 143
    public class KfCall : AutoCSer.Net.TcpInternalServer.Server
    {
        // Token: 0x06000787 RID: 1927 RVA: 0x0006442C File Offset: 0x0006262C
        public static KeyValue<string, int>[] _identityCommandNames_()
        {
            KeyValue<string, int>[] names = new KeyValue<string, int>[23];
            names[0].Set("KF.TcpCall.EscapeBattle_K(Server.Data.EscapeBattleSyncData)SyncZhengBaData", 0);
            names[1].Set("KF.TcpCall.EscapeBattle_K(AutoCSer.Net.TcpInternalServer.ServerSocketSender,int,int,int,out Server.Data.EscapeBattleFuBenData)ZhengBaKuaFuLogin", 1);
            names[2].Set("KF.TcpCall.EscapeBattle_K(int,System.Collections.Generic.List<int>)GameResult", 2);
            names[3].Set("KF.TcpCall.EscapeBattle_K(int,out int,out int,out string[],out int[])ZhengBaRequestEnter", 3);
            names[4].Set("KF.TcpCall.KFBoCaiManager(Tmsk.Contract.KuaFuData.KFBoCaiShopDB,int)BoCaiBuyItem", 4);
            names[5].Set("KF.TcpCall.KFBoCaiManager(Tmsk.Contract.KuaFuData.KFBuyBocaiData)BuyBoCai", 5);
            names[6].Set("KF.TcpCall.KFBoCaiManager(int)GetKFStageData", 6);
            names[7].Set("KF.TcpCall.KFBoCaiManager(int)GetOpenLottery", 7);
            names[8].Set("KF.TcpCall.KFBoCaiManager(int,long,bool)GetOpenLottery", 8);
            names[9].Set("KF.TcpCall.KFBoCaiManager(int)GetWinHistory", 9);
            names[10].Set("KF.TcpCall.KFBoCaiManager(int,string,int,long)IsCanBuy", 10);
            names[11].Set("KF.TcpCall.KFServiceBase(AutoCSer.Net.TcpInternalServer.ServerSocketSender,string[])ExecuteCommand", 11);
            names[12].Set("KF.TcpCall.KFServiceBase(AutoCSer.Net.TcpInternalServer.ServerSocketSender,KF.Contract.KuaFuClientContext)InitializeClient", 12);
            names[13].Set("KF.TcpCall.KFServiceBase(AutoCSer.Net.TcpInternalServer.ServerSocketSender,System.Func<AutoCSer.Net.TcpServer.ReturnValue<KF.Remoting.KFCallMsg>,bool>)KeepGetMessage", 13);
            names[14].Set("KF.TcpCall.KFServiceBase(AutoCSer.Net.TcpInternalServer.ServerSocketSender,int,System.Collections.Generic.Dictionary<int,int>)UpdateKuaFuMapClientCount", 14);
            names[15].Set("KF.TcpCall.ZhanDuiZhengBa_K(KF.Contract.Data.ZhanDuiZhengBaSyncData)SyncZhengBaData", 15);
            names[16].Set("KF.TcpCall.ZhanDuiZhengBa_K(AutoCSer.Net.TcpInternalServer.ServerSocketSender,int,int,int,out KF.Contract.Data.ZhanDuiZhengBaFuBenData)ZhengBaKuaFuLogin", 16);
            names[17].Set("KF.TcpCall.ZhanDuiZhengBa_K(int,int)ZhengBaPkResult", 17);
            names[18].Set("KF.TcpCall.ZhanDuiZhengBa_K(int,out int,out int,out string[],out int[])ZhengBaRequestEnter", 18);
            names[19].Set("KF.TcpCall.EscapeBattle_K(int,int,int)ZhanDuiJoin", 19);
            names[20].Set("KF.TcpCall.EscapeBattle_K(int,int)GameState", 20);
            names[21].Set("KF.TcpCall.EscapeBattle_K(int)GetZhanDuiState", 21);
            names[22].Set("KF.TcpCall.TestS2KFCommunication(int,bool)SendData", 22);
            return names;
        }

        // Token: 0x06000788 RID: 1928 RVA: 0x00064618 File Offset: 0x00062818
        public KfCall(AutoCSer.Net.TcpInternalServer.ServerAttribute attribute = null, Func<Socket, bool> verify = null, Action<SubArray<byte>> onCustomData = null, ILog log = null) 
            : base(attribute, verify, onCustomData, log, true)
        {
            base.setCommandData(23);
            base.setCommand(0);
            base.setCommand(1);
            base.setCommand(2);
            base.setCommand(3);
            base.setCommand(4);
            base.setCommand(5);
            base.setCommand(6);
            base.setCommand(7);
            base.setCommand(8);
            base.setCommand(9);
            base.setCommand(10);
            base.setCommand(11);
            base.setVerifyCommand(12);
            base.setCommand(13);
            base.setCommand(14);
            base.setCommand(15);
            base.setCommand(16);
            base.setCommand(17);
            base.setCommand(18);
            base.setCommand(19);
            base.setCommand(20);
            base.setCommand(21);
            base.setCommand(22);
            if (attribute.IsAutoServer)
            {
                this.Start();
            }
        }

        // Token: 0x06000789 RID: 1929 RVA: 0x00064734 File Offset: 0x00062934
        public override void DoCommand(int index, AutoCSer.Net.TcpInternalServer.ServerSocketSender sender, ref SubArray<byte> data)
        {
            long startTicks = TimeUtil.NOW();
            switch (index)
            {
                case 128:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p1 inputParameter = default(KfCall._p1);
                            if (sender.DeSerialize<KfCall._p1>(ref data, ref inputParameter, false))
                            {
                                (ServerCall<KfCall._s0>.Pop() ?? new KfCall._s0()).Set(sender, ServerTaskType.Queue, ref inputParameter);
                                CmdMonitor.RecordCmdDetail(0, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(0, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 129:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p3 inputParameter2 = default(KfCall._p3);
                            if (sender.DeSerialize<KfCall._p3>(ref data, ref inputParameter2, false))
                            {
                                (ServerCall<KfCall._s1>.Pop() ?? new KfCall._s1()).Set(sender, ServerTaskType.Queue, ref inputParameter2);
                                CmdMonitor.RecordCmdDetail(1, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(1, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 130:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p5 inputParameter3 = default(KfCall._p5);
                            if (sender.DeSerialize<KfCall._p5>(ref data, ref inputParameter3, false))
                            {
                                (ServerCall<KfCall._s2>.Pop() ?? new KfCall._s2()).Set(sender, ServerTaskType.Queue, ref inputParameter3);
                                CmdMonitor.RecordCmdDetail(2, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(2, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 131:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p7 inputParameter4 = default(KfCall._p7);
                            if (sender.DeSerialize<KfCall._p7>(ref data, ref inputParameter4, false))
                            {
                                (ServerCall<KfCall._s3>.Pop() ?? new KfCall._s3()).Set(sender, ServerTaskType.Queue, ref inputParameter4);
                                CmdMonitor.RecordCmdDetail(3, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(3, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 132:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p9 inputParameter5 = default(KfCall._p9);
                            if (sender.DeSerialize<KfCall._p9>(ref data, ref inputParameter5, false))
                            {
                                (ServerCall<KfCall._s4>.Pop() ?? new KfCall._s4()).Set(sender, ServerTaskType.Queue, ref inputParameter5);
                                CmdMonitor.RecordCmdDetail(4, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(4, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 133:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p11 inputParameter6 = default(KfCall._p11);
                            if (sender.DeSerialize<KfCall._p11>(ref data, ref inputParameter6, false))
                            {
                                (ServerCall<KfCall._s5>.Pop() ?? new KfCall._s5()).Set(sender, ServerTaskType.Queue, ref inputParameter6);
                                CmdMonitor.RecordCmdDetail(5, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(5, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 134:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p12 inputParameter7 = default(KfCall._p12);
                            if (sender.DeSerialize<KfCall._p12>(ref data, ref inputParameter7, true))
                            {
                                (ServerCall<KfCall._s6>.Pop() ?? new KfCall._s6()).Set(sender, ServerTaskType.Queue, ref inputParameter7);
                                CmdMonitor.RecordCmdDetail(6, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(6, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 135:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p12 inputParameter7 = default(KfCall._p12);
                            if (sender.DeSerialize<KfCall._p12>(ref data, ref inputParameter7, true))
                            {
                                (ServerCall<KfCall._s7>.Pop() ?? new KfCall._s7()).Set(sender, ServerTaskType.Queue, ref inputParameter7);
                                CmdMonitor.RecordCmdDetail(7, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(7, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 136:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p15 inputParameter8 = default(KfCall._p15);
                            if (sender.DeSerialize<KfCall._p15>(ref data, ref inputParameter8, true))
                            {
                                (ServerCall<KfCall._s8>.Pop() ?? new KfCall._s8()).Set(sender, ServerTaskType.Queue, ref inputParameter8);
                                CmdMonitor.RecordCmdDetail(8, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(8, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 137:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p12 inputParameter7 = default(KfCall._p12);
                            if (sender.DeSerialize<KfCall._p12>(ref data, ref inputParameter7, true))
                            {
                                (ServerCall<KfCall._s9>.Pop() ?? new KfCall._s9()).Set(sender, ServerTaskType.Queue, ref inputParameter7);
                                CmdMonitor.RecordCmdDetail(9, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(9, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 138:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p18 inputParameter9 = default(KfCall._p18);
                            if (sender.DeSerialize<KfCall._p18>(ref data, ref inputParameter9, true))
                            {
                                (ServerCall<KfCall._s10>.Pop() ?? new KfCall._s10()).Set(sender, ServerTaskType.Queue, ref inputParameter9);
                                CmdMonitor.RecordCmdDetail(10, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(10, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 139:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p19 inputParameter10 = default(KfCall._p19);
                            if (sender.DeSerialize<KfCall._p19>(ref data, ref inputParameter10, false))
                            {
                                (ServerCall<KfCall._s11>.Pop() ?? new KfCall._s11()).Set(sender, ServerTaskType.Queue, ref inputParameter10);
                                CmdMonitor.RecordCmdDetail(11, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(11, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 140:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p20 inputParameter11 = default(KfCall._p20);
                            if (sender.DeSerialize<KfCall._p20>(ref data, ref inputParameter11, false))
                            {
                                KfCall._p10 _outputParameter_ = default(KfCall._p10);
                                bool Return = KFServiceBase.TcpStaticServer._M13(sender, inputParameter11.p0);
                                if (Return)
                                {
                                    sender.SetVerifyMethod();
                                }
                                _outputParameter_.Return = Return;
                                sender.Push<KfCall._p10>(KfCall._c13, ref _outputParameter_);
                                CmdMonitor.RecordCmdDetail(12, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(12, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 141:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p21 outputParameter = default(KfCall._p21);
                            KFServiceBase.TcpStaticServer._M14(sender, sender.GetCallback<KfCall._p21, KFCallMsg>(KfCall._c14, ref outputParameter));
                            CmdMonitor.RecordCmdDetail(13, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                            break;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(13, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 142:
                    try
                    {
                        KfCall._p22 inputParameter12 = default(KfCall._p22);
                        if (sender.DeSerialize<KfCall._p22>(ref data, ref inputParameter12, false))
                        {
                            (ServerCall<KfCall._s14>.Pop() ?? new KfCall._s14()).Set(sender, ServerTaskType.Queue, ref inputParameter12);
                            CmdMonitor.RecordCmdDetail(14, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                            break;
                        }
                    }
                    catch (Exception error)
                    {
                        sender.AddLog(error);
                    }
                    CmdMonitor.RecordCmdDetail(14, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                    break;
                case 143:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p23 inputParameter13 = default(KfCall._p23);
                            if (sender.DeSerialize<KfCall._p23>(ref data, ref inputParameter13, false))
                            {
                                (ServerCall<KfCall._s15>.Pop() ?? new KfCall._s15()).Set(sender, ServerTaskType.Queue, ref inputParameter13);
                                CmdMonitor.RecordCmdDetail(15, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(15, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 144:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p25 inputParameter14 = default(KfCall._p25);
                            if (sender.DeSerialize<KfCall._p25>(ref data, ref inputParameter14, false))
                            {
                                (ServerCall<KfCall._s16>.Pop() ?? new KfCall._s16()).Set(sender, ServerTaskType.Queue, ref inputParameter14);
                                CmdMonitor.RecordCmdDetail(16, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(16, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 145:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p27 inputParameter15 = default(KfCall._p27);
                            if (sender.DeSerialize<KfCall._p27>(ref data, ref inputParameter15, true))
                            {
                                (ServerCall<KfCall._s17>.Pop() ?? new KfCall._s17()).Set(sender, ServerTaskType.Queue, ref inputParameter15);
                                CmdMonitor.RecordCmdDetail(17, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(17, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 146:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p7 inputParameter4 = default(KfCall._p7);
                            if (sender.DeSerialize<KfCall._p7>(ref data, ref inputParameter4, false))
                            {
                                (ServerCall<KfCall._s18>.Pop() ?? new KfCall._s18()).Set(sender, ServerTaskType.Queue, ref inputParameter4);
                                CmdMonitor.RecordCmdDetail(18, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(18, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 147:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p29 inputParameter16 = default(KfCall._p29);
                            if (sender.DeSerialize<KfCall._p29>(ref data, ref inputParameter16, true))
                            {
                                (ServerCall<KfCall._s19>.Pop() ?? new KfCall._s19()).Set(sender, ServerTaskType.Queue, ref inputParameter16);
                                CmdMonitor.RecordCmdDetail(19, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(19, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 148:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p27 inputParameter15 = default(KfCall._p27);
                            if (sender.DeSerialize<KfCall._p27>(ref data, ref inputParameter15, true))
                            {
                                (ServerCall<KfCall._s20>.Pop() ?? new KfCall._s20()).Set(sender, ServerTaskType.Queue, ref inputParameter15);
                                CmdMonitor.RecordCmdDetail(20, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(20, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 149:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p12 inputParameter7 = default(KfCall._p12);
                            if (sender.DeSerialize<KfCall._p12>(ref data, ref inputParameter7, true))
                            {
                                (ServerCall<KfCall._s21>.Pop() ?? new KfCall._s21()).Set(sender, ServerTaskType.Queue, ref inputParameter7);
                                CmdMonitor.RecordCmdDetail(21, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(21, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
                case 150:
                    {
                        ReturnType returnType = ReturnType.Unknown;
                        try
                        {
                            KfCall._p30 inputParameter17 = default(KfCall._p30);
                            if (sender.DeSerialize<KfCall._p30>(ref data, ref inputParameter17, true))
                            {
                                (ServerCall<KfCall._s22>.Pop() ?? new KfCall._s22()).Set(sender, ServerTaskType.Queue, ref inputParameter17);
                                CmdMonitor.RecordCmdDetail(22, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_OK);
                                break;
                            }
                            returnType = ReturnType.ServerDeSerializeError;
                        }
                        catch (Exception error)
                        {
                            returnType = ReturnType.ServerException;
                            sender.AddLog(error);
                        }
                        sender.Push(returnType);
                        CmdMonitor.RecordCmdDetail(22, TimeUtil.NOW() - startTicks, (long)data.Count, TCPProcessCmdResults.RESULT_FAILED);
                        break;
                    }
            }
        }

        // Token: 0x0600078A RID: 1930 RVA: 0x00065664 File Offset: 0x00063864
        static KfCall()
        {
            Type[] array = new Type[7];
            array[0] = typeof(KfCall._p12);
            array[1] = typeof(KfCall._p15);
            array[2] = typeof(KfCall._p18);
            array[3] = typeof(KfCall._p27);
            array[4] = typeof(KfCall._p29);
            array[5] = typeof(KfCall._p30);
            Type[] simpleDeSerializeTypes = array;
            array = new Type[4];
            array[0] = typeof(KfCall._p6);
            array[1] = typeof(KfCall._p10);
            array[2] = typeof(KfCall._p31);
            Type[] simpleSerializeTypes = array;
            array = new Type[12];
            array[0] = typeof(KfCall._p1);
            array[1] = typeof(KfCall._p3);
            array[2] = typeof(KfCall._p5);
            array[3] = typeof(KfCall._p7);
            array[4] = typeof(KfCall._p9);
            array[5] = typeof(KfCall._p11);
            array[6] = typeof(KfCall._p19);
            array[7] = typeof(KfCall._p20);
            array[8] = typeof(KfCall._p22);
            array[9] = typeof(KfCall._p23);
            array[10] = typeof(KfCall._p25);
            Type[] deSerializeTypes = array;
            array = new Type[12];
            array[0] = typeof(KfCall._p2);
            array[1] = typeof(KfCall._p4);
            array[2] = typeof(KfCall._p8);
            array[3] = typeof(KfCall._p13);
            array[4] = typeof(KfCall._p14);
            array[5] = typeof(KfCall._p16);
            array[6] = typeof(KfCall._p17);
            array[7] = typeof(KfCall._p21);
            array[8] = typeof(KfCall._p24);
            array[9] = typeof(KfCall._p26);
            array[10] = typeof(KfCall._p28);
            Type[] serializeTypes = array;
            array = new Type[1];
            Type[] jsonDeSerializeTypes = array;
            array = new Type[1];
            CommandBase.CompileSerialize(simpleDeSerializeTypes, simpleSerializeTypes, deSerializeTypes, serializeTypes, jsonDeSerializeTypes, array);
        }

        // Token: 0x04000410 RID: 1040
        private static readonly OutputInfo _c1 = new OutputInfo
        {
            OutputParameterIndex = 2,
            IsBuildOutputThread = true
        };

        // Token: 0x04000411 RID: 1041
        private static readonly OutputInfo _c2 = new OutputInfo
        {
            OutputParameterIndex = 4,
            IsBuildOutputThread = true
        };

        // Token: 0x04000412 RID: 1042
        private static readonly OutputInfo _c3 = new OutputInfo
        {
            OutputParameterIndex = 6,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        // Token: 0x04000413 RID: 1043
        private static readonly OutputInfo _c4 = new OutputInfo
        {
            OutputParameterIndex = 8,
            IsBuildOutputThread = true
        };

        // Token: 0x04000414 RID: 1044
        private static readonly OutputInfo _c5 = new OutputInfo
        {
            OutputParameterIndex = 10,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        // Token: 0x04000415 RID: 1045
        private static readonly OutputInfo _c6 = new OutputInfo
        {
            OutputParameterIndex = 10,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        // Token: 0x04000416 RID: 1046
        private static readonly OutputInfo _c7 = new OutputInfo
        {
            OutputParameterIndex = 13,
            IsBuildOutputThread = true
        };

        // Token: 0x04000417 RID: 1047
        private static readonly OutputInfo _c8 = new OutputInfo
        {
            OutputParameterIndex = 14,
            IsBuildOutputThread = true
        };

        // Token: 0x04000418 RID: 1048
        private static readonly OutputInfo _c9 = new OutputInfo
        {
            OutputParameterIndex = 16,
            IsBuildOutputThread = true
        };

        // Token: 0x04000419 RID: 1049
        private static readonly OutputInfo _c10 = new OutputInfo
        {
            OutputParameterIndex = 17,
            IsBuildOutputThread = true
        };

        // Token: 0x0400041A RID: 1050
        private static readonly OutputInfo _c11 = new OutputInfo
        {
            OutputParameterIndex = 10,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        // Token: 0x0400041B RID: 1051
        private static readonly OutputInfo _c12 = new OutputInfo
        {
            OutputParameterIndex = 6,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        // Token: 0x0400041C RID: 1052
        private static readonly OutputInfo _c13 = new OutputInfo
        {
            OutputParameterIndex = 10,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        // Token: 0x0400041D RID: 1053
        private static readonly OutputInfo _c14 = new OutputInfo
        {
            OutputParameterIndex = 21,
            IsKeepCallback = 1,
            IsBuildOutputThread = true
        };

        // Token: 0x0400041E RID: 1054
        private static readonly OutputInfo _c15 = new OutputInfo
        {
            OutputParameterIndex = 0,
            IsClientSendOnly = 1,
            IsBuildOutputThread = true
        };

        // Token: 0x0400041F RID: 1055
        private static readonly OutputInfo _c16 = new OutputInfo
        {
            OutputParameterIndex = 24,
            IsBuildOutputThread = true
        };

        // Token: 0x04000420 RID: 1056
        private static readonly OutputInfo _c17 = new OutputInfo
        {
            OutputParameterIndex = 26,
            IsBuildOutputThread = true
        };

        // Token: 0x04000421 RID: 1057
        private static readonly OutputInfo _c18 = new OutputInfo
        {
            OutputParameterIndex = 28,
            IsBuildOutputThread = true
        };

        // Token: 0x04000422 RID: 1058
        private static readonly OutputInfo _c19 = new OutputInfo
        {
            OutputParameterIndex = 8,
            IsBuildOutputThread = true
        };

        // Token: 0x04000423 RID: 1059
        private static readonly OutputInfo _c20 = new OutputInfo
        {
            OutputParameterIndex = 6,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        // Token: 0x04000424 RID: 1060
        private static readonly OutputInfo _c21 = new OutputInfo
        {
            OutputParameterIndex = 6,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        // Token: 0x04000425 RID: 1061
        private static readonly OutputInfo _c22 = new OutputInfo
        {
            OutputParameterIndex = 6,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        // Token: 0x04000426 RID: 1062
        private static readonly OutputInfo _c23 = new OutputInfo
        {
            OutputParameterIndex = 31,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        // Token: 0x02000090 RID: 144
        private sealed class _s0 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s0, KfCall._p1>
        {
            // Token: 0x0600078B RID: 1931 RVA: 0x00065B7C File Offset: 0x00063D7C
            private void get(ref ReturnValue<KfCall._p2> value)
            {
                try
                {
                    EscapeBattleSyncData Return = EscapeBattle_K.TcpStaticServer._M1(this.inputParameter.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x0600078C RID: 1932 RVA: 0x00065BE0 File Offset: 0x00063DE0
            public override void Call()
            {
                ReturnValue<KfCall._p2> value = default(ReturnValue<KfCall._p2>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p2>(this.CommandIndex, KfCall._c1, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x02000091 RID: 145
        private sealed class _s1 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s1, KfCall._p3>
        {
            // Token: 0x0600078E RID: 1934 RVA: 0x00065C3C File Offset: 0x00063E3C
            private void get(ref ReturnValue<KfCall._p4> value)
            {
                try
                {
                    int Return = EscapeBattle_K.TcpStaticServer._M2(this.Sender, this.inputParameter.p1, this.inputParameter.p2, this.inputParameter.p3, out value.Value.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x0600078F RID: 1935 RVA: 0x00065CC4 File Offset: 0x00063EC4
            public override void Call()
            {
                ReturnValue<KfCall._p4> value = default(ReturnValue<KfCall._p4>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p4>(this.CommandIndex, KfCall._c2, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x02000092 RID: 146
        private sealed class _s2 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s2, KfCall._p5>
        {
            // Token: 0x06000791 RID: 1937 RVA: 0x00065D20 File Offset: 0x00063F20
            private void get(ref ReturnValue<KfCall._p6> value)
            {
                try
                {
                    int Return = EscapeBattle_K.TcpStaticServer._M3(this.inputParameter.p1, this.inputParameter.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x06000792 RID: 1938 RVA: 0x00065D8C File Offset: 0x00063F8C
            public override void Call()
            {
                ReturnValue<KfCall._p6> value = default(ReturnValue<KfCall._p6>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p6>(this.CommandIndex, KfCall._c3, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x02000093 RID: 147
        private sealed class _s3 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s3, KfCall._p7>
        {
            // Token: 0x06000794 RID: 1940 RVA: 0x00065DE8 File Offset: 0x00063FE8
            private void get(ref ReturnValue<KfCall._p8> value)
            {
                try
                {
                    int Return = EscapeBattle_K.TcpStaticServer._M4(this.inputParameter.p0, out value.Value.p0, out value.Value.p1, out value.Value.p3, out value.Value.p2);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x06000795 RID: 1941 RVA: 0x00065E78 File Offset: 0x00064078
            public override void Call()
            {
                ReturnValue<KfCall._p8> value = default(ReturnValue<KfCall._p8>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p8>(this.CommandIndex, KfCall._c4, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x02000094 RID: 148
        private sealed class _s4 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s4, KfCall._p9>
        {
            // Token: 0x06000797 RID: 1943 RVA: 0x00065ED4 File Offset: 0x000640D4
            private void get(ref ReturnValue<KfCall._p10> value)
            {
                try
                {
                    bool Return = KFBoCaiManager.TcpStaticServer._M5(this.inputParameter.p0, this.inputParameter.p1);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x06000798 RID: 1944 RVA: 0x00065F40 File Offset: 0x00064140
            public override void Call()
            {
                ReturnValue<KfCall._p10> value = default(ReturnValue<KfCall._p10>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p10>(this.CommandIndex, KfCall._c5, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x02000095 RID: 149
        private sealed class _s5 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s5, KfCall._p11>
        {
            // Token: 0x0600079A RID: 1946 RVA: 0x00065F9C File Offset: 0x0006419C
            private void get(ref ReturnValue<KfCall._p10> value)
            {
                try
                {
                    bool Return = KFBoCaiManager.TcpStaticServer._M6(this.inputParameter.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x0600079B RID: 1947 RVA: 0x00066000 File Offset: 0x00064200
            public override void Call()
            {
                ReturnValue<KfCall._p10> value = default(ReturnValue<KfCall._p10>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p10>(this.CommandIndex, KfCall._c6, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x02000096 RID: 150
        private sealed class _s6 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s6, KfCall._p12>
        {
            // Token: 0x0600079D RID: 1949 RVA: 0x0006605C File Offset: 0x0006425C
            private void get(ref ReturnValue<KfCall._p13> value)
            {
                try
                {
                    KFStageData Return = KFBoCaiManager.TcpStaticServer._M7(this.inputParameter.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x0600079E RID: 1950 RVA: 0x000660C0 File Offset: 0x000642C0
            public override void Call()
            {
                ReturnValue<KfCall._p13> value = default(ReturnValue<KfCall._p13>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p13>(this.CommandIndex, KfCall._c7, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x02000097 RID: 151
        private sealed class _s7 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s7, KfCall._p12>
        {
            // Token: 0x060007A0 RID: 1952 RVA: 0x0006611C File Offset: 0x0006431C
            private void get(ref ReturnValue<KfCall._p14> value)
            {
                try
                {
                    OpenLottery Return = KFBoCaiManager.TcpStaticServer._M8(this.inputParameter.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007A1 RID: 1953 RVA: 0x00066180 File Offset: 0x00064380
            public override void Call()
            {
                ReturnValue<KfCall._p14> value = default(ReturnValue<KfCall._p14>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p14>(this.CommandIndex, KfCall._c8, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x02000098 RID: 152
        private sealed class _s8 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s8, KfCall._p15>
        {
            // Token: 0x060007A3 RID: 1955 RVA: 0x000661DC File Offset: 0x000643DC
            private void get(ref ReturnValue<KfCall._p16> value)
            {
                try
                {
                    List<OpenLottery> Return = KFBoCaiManager.TcpStaticServer._M9(this.inputParameter.p1, this.inputParameter.p2, this.inputParameter.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007A4 RID: 1956 RVA: 0x00066254 File Offset: 0x00064454
            public override void Call()
            {
                ReturnValue<KfCall._p16> value = default(ReturnValue<KfCall._p16>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p16>(this.CommandIndex, KfCall._c9, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x02000099 RID: 153
        private sealed class _s9 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s9, KfCall._p12>
        {
            // Token: 0x060007A6 RID: 1958 RVA: 0x000662B0 File Offset: 0x000644B0
            private void get(ref ReturnValue<KfCall._p17> value)
            {
                try
                {
                    List<KFBoCaoHistoryData> Return = KFBoCaiManager.TcpStaticServer._M10(this.inputParameter.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007A7 RID: 1959 RVA: 0x00066314 File Offset: 0x00064514
            public override void Call()
            {
                ReturnValue<KfCall._p17> value = default(ReturnValue<KfCall._p17>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p17>(this.CommandIndex, KfCall._c10, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x0200009A RID: 154
        private sealed class _s10 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s10, KfCall._p18>
        {
            // Token: 0x060007A9 RID: 1961 RVA: 0x00066370 File Offset: 0x00064570
            private void get(ref ReturnValue<KfCall._p10> value)
            {
                try
                {
                    bool Return = KFBoCaiManager.TcpStaticServer._M11(this.inputParameter.p0, this.inputParameter.p3, this.inputParameter.p1, this.inputParameter.p2);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007AA RID: 1962 RVA: 0x000663F4 File Offset: 0x000645F4
            public override void Call()
            {
                ReturnValue<KfCall._p10> value = default(ReturnValue<KfCall._p10>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p10>(this.CommandIndex, KfCall._c11, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x0200009B RID: 155
        private sealed class _s11 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s11, KfCall._p19>
        {
            // Token: 0x060007AC RID: 1964 RVA: 0x00066450 File Offset: 0x00064650
            private void get(ref ReturnValue<KfCall._p6> value)
            {
                try
                {
                    int Return = KFServiceBase.TcpStaticServer._M12(this.Sender, this.inputParameter.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007AD RID: 1965 RVA: 0x000664B8 File Offset: 0x000646B8
            public override void Call()
            {
                ReturnValue<KfCall._p6> value = default(ReturnValue<KfCall._p6>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p6>(this.CommandIndex, KfCall._c12, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x0200009C RID: 156
        private sealed class _s14 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s14, KfCall._p22>
        {
            // Token: 0x060007AF RID: 1967 RVA: 0x00066514 File Offset: 0x00064714
            private void get(ref ReturnValue value)
            {
                try
                {
                    KFServiceBase.TcpStaticServer._M15(this.Sender, this.inputParameter.p1, this.inputParameter.p0);
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007B0 RID: 1968 RVA: 0x0006657C File Offset: 0x0006477C
            public override void Call()
            {
                ReturnValue value = default(ReturnValue);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x0200009D RID: 157
        private sealed class _s15 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s15, KfCall._p23>
        {
            // Token: 0x060007B2 RID: 1970 RVA: 0x000665C0 File Offset: 0x000647C0
            private void get(ref ReturnValue<KfCall._p24> value)
            {
                try
                {
                    ZhanDuiZhengBaSyncData Return = ZhanDuiZhengBa_K.TcpStaticServer._M16(this.inputParameter.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007B3 RID: 1971 RVA: 0x00066624 File Offset: 0x00064824
            public override void Call()
            {
                ReturnValue<KfCall._p24> value = default(ReturnValue<KfCall._p24>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p24>(this.CommandIndex, KfCall._c16, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x0200009E RID: 158
        private sealed class _s16 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s16, KfCall._p25>
        {
            // Token: 0x060007B5 RID: 1973 RVA: 0x00066680 File Offset: 0x00064880
            private void get(ref ReturnValue<KfCall._p26> value)
            {
                try
                {
                    int Return = ZhanDuiZhengBa_K.TcpStaticServer._M17(this.Sender, this.inputParameter.p1, this.inputParameter.p2, this.inputParameter.p3, out value.Value.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007B6 RID: 1974 RVA: 0x00066708 File Offset: 0x00064908
            public override void Call()
            {
                ReturnValue<KfCall._p26> value = default(ReturnValue<KfCall._p26>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p26>(this.CommandIndex, KfCall._c17, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x0200009F RID: 159
        private sealed class _s17 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s17, KfCall._p27>
        {
            // Token: 0x060007B8 RID: 1976 RVA: 0x00066764 File Offset: 0x00064964
            private void get(ref ReturnValue<KfCall._p28> value)
            {
                try
                {
                    List<ZhanDuiZhengBaNtfPkResultData> Return = ZhanDuiZhengBa_K.TcpStaticServer._M18(this.inputParameter.p0, this.inputParameter.p1);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007B9 RID: 1977 RVA: 0x000667D0 File Offset: 0x000649D0
            public override void Call()
            {
                ReturnValue<KfCall._p28> value = default(ReturnValue<KfCall._p28>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p28>(this.CommandIndex, KfCall._c18, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x020000A0 RID: 160
        private sealed class _s18 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s18, KfCall._p7>
        {
            // Token: 0x060007BB RID: 1979 RVA: 0x0006682C File Offset: 0x00064A2C
            private void get(ref ReturnValue<KfCall._p8> value)
            {
                try
                {
                    int Return = ZhanDuiZhengBa_K.TcpStaticServer._M19(this.inputParameter.p0, out value.Value.p0, out value.Value.p1, out value.Value.p3, out value.Value.p2);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007BC RID: 1980 RVA: 0x000668BC File Offset: 0x00064ABC
            public override void Call()
            {
                ReturnValue<KfCall._p8> value = default(ReturnValue<KfCall._p8>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p8>(this.CommandIndex, KfCall._c19, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x020000A1 RID: 161
        private sealed class _s19 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s19, KfCall._p29>
        {
            // Token: 0x060007BE RID: 1982 RVA: 0x00066918 File Offset: 0x00064B18
            private void get(ref ReturnValue<KfCall._p6> value)
            {
                try
                {
                    int Return = EscapeBattle_K.TcpStaticServer._M20(this.inputParameter.p0, this.inputParameter.p1, this.inputParameter.p2);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007BF RID: 1983 RVA: 0x00066990 File Offset: 0x00064B90
            public override void Call()
            {
                ReturnValue<KfCall._p6> value = default(ReturnValue<KfCall._p6>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p6>(this.CommandIndex, KfCall._c20, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x020000A2 RID: 162
        private sealed class _s20 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s20, KfCall._p27>
        {
            // Token: 0x060007C1 RID: 1985 RVA: 0x000669EC File Offset: 0x00064BEC
            private void get(ref ReturnValue<KfCall._p6> value)
            {
                try
                {
                    int Return = EscapeBattle_K.TcpStaticServer._M21(this.inputParameter.p0, this.inputParameter.p1);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007C2 RID: 1986 RVA: 0x00066A58 File Offset: 0x00064C58
            public override void Call()
            {
                ReturnValue<KfCall._p6> value = default(ReturnValue<KfCall._p6>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p6>(this.CommandIndex, KfCall._c21, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x020000A3 RID: 163
        private sealed class _s21 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s21, KfCall._p12>
        {
            // Token: 0x060007C4 RID: 1988 RVA: 0x00066AB4 File Offset: 0x00064CB4
            private void get(ref ReturnValue<KfCall._p6> value)
            {
                try
                {
                    int Return = EscapeBattle_K.TcpStaticServer._M22(this.inputParameter.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007C5 RID: 1989 RVA: 0x00066B18 File Offset: 0x00064D18
            public override void Call()
            {
                ReturnValue<KfCall._p6> value = default(ReturnValue<KfCall._p6>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p6>(this.CommandIndex, KfCall._c22, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x020000A4 RID: 164
        private sealed class _s22 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s22, KfCall._p30>
        {
            // Token: 0x060007C7 RID: 1991 RVA: 0x00066B74 File Offset: 0x00064D74
            private void get(ref ReturnValue<KfCall._p31> value)
            {
                try
                {
                    string Return = TestS2KFCommunication.TcpStaticServer._M23(this.inputParameter.p1, this.inputParameter.p0);
                    value.Value.Return = Return;
                    value.Type = ReturnType.Success;
                }
                catch (Exception error)
                {
                    value.Type = ReturnType.ServerException;
                    this.Sender.AddLog(error);
                }
            }

            // Token: 0x060007C8 RID: 1992 RVA: 0x00066BE0 File Offset: 0x00064DE0
            public override void Call()
            {
                ReturnValue<KfCall._p31> value = default(ReturnValue<KfCall._p31>);
                if (this.Sender.IsSocket)
                {
                    this.get(ref value);
                    this.Sender.Push<KfCall._p31>(this.CommandIndex, KfCall._c23, ref value);
                }
                base.push(this);
            }
        }

        // Token: 0x020000A5 RID: 165
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p1
        {
            // Token: 0x04000427 RID: 1063
            public EscapeBattleSyncData p0;
        }

        // Token: 0x020000A6 RID: 166
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p2 : IReturnParameter<EscapeBattleSyncData>
        {
            // Token: 0x17000037 RID: 55
            // (get) Token: 0x060007CA RID: 1994 RVA: 0x00066C3C File Offset: 0x00064E3C
            // (set) Token: 0x060007CB RID: 1995 RVA: 0x00066C54 File Offset: 0x00064E54
            [Preserve(Conditional = true)]
            public EscapeBattleSyncData Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x04000428 RID: 1064
            [AutoCSer.Json.IgnoreMember]
            public EscapeBattleSyncData Ret;
        }

        // Token: 0x020000A7 RID: 167
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p3
        {
            // Token: 0x04000429 RID: 1065
            public EscapeBattleFuBenData p0;

            // Token: 0x0400042A RID: 1066
            public int p1;

            // Token: 0x0400042B RID: 1067
            public int p2;

            // Token: 0x0400042C RID: 1068
            public int p3;
        }

        // Token: 0x020000A8 RID: 168
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p4 : IReturnParameter<int>
        {
            // Token: 0x17000038 RID: 56
            // (get) Token: 0x060007CC RID: 1996 RVA: 0x00066C60 File Offset: 0x00064E60
            // (set) Token: 0x060007CD RID: 1997 RVA: 0x00066C78 File Offset: 0x00064E78
            [Preserve(Conditional = true)]
            public int Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x0400042D RID: 1069
            public EscapeBattleFuBenData p0;

            // Token: 0x0400042E RID: 1070
            [AutoCSer.Json.IgnoreMember]
            public int Ret;
        }

        // Token: 0x020000A9 RID: 169
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p5
        {
            // Token: 0x0400042F RID: 1071
            public List<int> p0;

            // Token: 0x04000430 RID: 1072
            public int p1;
        }

        // Token: 0x020000AA RID: 170
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p6 : IReturnParameter<int>
        {
            // Token: 0x17000039 RID: 57
            // (get) Token: 0x060007CE RID: 1998 RVA: 0x00066C84 File Offset: 0x00064E84
            // (set) Token: 0x060007CF RID: 1999 RVA: 0x00066C9C File Offset: 0x00064E9C
            [Preserve(Conditional = true)]
            public int Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x04000431 RID: 1073
            [AutoCSer.Json.IgnoreMember]
            public int Ret;
        }

        // Token: 0x020000AB RID: 171
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p7
        {
            // Token: 0x04000432 RID: 1074
            public int p0;

            // Token: 0x04000433 RID: 1075
            public int p1;

            // Token: 0x04000434 RID: 1076
            public int p2;

            // Token: 0x04000435 RID: 1077
            public int[] p3;

            // Token: 0x04000436 RID: 1078
            public string[] p4;
        }

        // Token: 0x020000AC RID: 172
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p8 : IReturnParameter<int>
        {
            // Token: 0x1700003A RID: 58
            // (get) Token: 0x060007D0 RID: 2000 RVA: 0x00066CA8 File Offset: 0x00064EA8
            // (set) Token: 0x060007D1 RID: 2001 RVA: 0x00066CC0 File Offset: 0x00064EC0
            [Preserve(Conditional = true)]
            public int Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x04000437 RID: 1079
            public int p0;

            // Token: 0x04000438 RID: 1080
            public int p1;

            // Token: 0x04000439 RID: 1081
            public int[] p2;

            // Token: 0x0400043A RID: 1082
            public string[] p3;

            // Token: 0x0400043B RID: 1083
            [AutoCSer.Json.IgnoreMember]
            public int Ret;
        }

        // Token: 0x020000AD RID: 173
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p9
        {
            // Token: 0x0400043C RID: 1084
            public KFBoCaiShopDB p0;

            // Token: 0x0400043D RID: 1085
            public int p1;
        }

        // Token: 0x020000AE RID: 174
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p10 : IReturnParameter<bool>
        {
            // Token: 0x1700003B RID: 59
            // (get) Token: 0x060007D2 RID: 2002 RVA: 0x00066CCC File Offset: 0x00064ECC
            // (set) Token: 0x060007D3 RID: 2003 RVA: 0x00066CE4 File Offset: 0x00064EE4
            [Preserve(Conditional = true)]
            public bool Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x0400043E RID: 1086
            [AutoCSer.Json.IgnoreMember]
            public bool Ret;
        }

        // Token: 0x020000AF RID: 175
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p11
        {
            // Token: 0x0400043F RID: 1087
            public KFBuyBocaiData p0;
        }

        // Token: 0x020000B0 RID: 176
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p12
        {
            // Token: 0x04000440 RID: 1088
            public int p0;
        }

        // Token: 0x020000B1 RID: 177
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p13 : IReturnParameter<KFStageData>
        {
            // Token: 0x1700003C RID: 60
            // (get) Token: 0x060007D4 RID: 2004 RVA: 0x00066CF0 File Offset: 0x00064EF0
            // (set) Token: 0x060007D5 RID: 2005 RVA: 0x00066D08 File Offset: 0x00064F08
            [Preserve(Conditional = true)]
            public KFStageData Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x04000441 RID: 1089
            [AutoCSer.Json.IgnoreMember]
            public KFStageData Ret;
        }

        // Token: 0x020000B2 RID: 178
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p14 : IReturnParameter<OpenLottery>
        {
            // Token: 0x1700003D RID: 61
            // (get) Token: 0x060007D6 RID: 2006 RVA: 0x00066D14 File Offset: 0x00064F14
            // (set) Token: 0x060007D7 RID: 2007 RVA: 0x00066D2C File Offset: 0x00064F2C
            [Preserve(Conditional = true)]
            public OpenLottery Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x04000442 RID: 1090
            [AutoCSer.Json.IgnoreMember]
            public OpenLottery Ret;
        }

        // Token: 0x020000B3 RID: 179
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p15
        {
            // Token: 0x04000443 RID: 1091
            public bool p0;

            // Token: 0x04000444 RID: 1092
            public int p1;

            // Token: 0x04000445 RID: 1093
            public long p2;
        }

        // Token: 0x020000B4 RID: 180
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p16 : IReturnParameter<List<OpenLottery>>
        {
            // Token: 0x1700003E RID: 62
            // (get) Token: 0x060007D8 RID: 2008 RVA: 0x00066D38 File Offset: 0x00064F38
            // (set) Token: 0x060007D9 RID: 2009 RVA: 0x00066D50 File Offset: 0x00064F50
            [Preserve(Conditional = true)]
            public List<OpenLottery> Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x04000446 RID: 1094
            [AutoCSer.Json.IgnoreMember]
            public List<OpenLottery> Ret;
        }

        // Token: 0x020000B5 RID: 181
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p17 : IReturnParameter<List<KFBoCaoHistoryData>>
        {
            // Token: 0x1700003F RID: 63
            // (get) Token: 0x060007DA RID: 2010 RVA: 0x00066D5C File Offset: 0x00064F5C
            // (set) Token: 0x060007DB RID: 2011 RVA: 0x00066D74 File Offset: 0x00064F74
            [Preserve(Conditional = true)]
            public List<KFBoCaoHistoryData> Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x04000447 RID: 1095
            [AutoCSer.Json.IgnoreMember]
            public List<KFBoCaoHistoryData> Ret;
        }

        // Token: 0x020000B6 RID: 182
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p18
        {
            // Token: 0x04000448 RID: 1096
            public int p0;

            // Token: 0x04000449 RID: 1097
            public int p1;

            // Token: 0x0400044A RID: 1098
            public long p2;

            // Token: 0x0400044B RID: 1099
            public string p3;
        }

        // Token: 0x020000B7 RID: 183
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p19
        {
            // Token: 0x0400044C RID: 1100
            public string[] p0;
        }

        // Token: 0x020000B8 RID: 184
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p20
        {
            // Token: 0x0400044D RID: 1101
            public KuaFuClientContext p0;
        }

        // Token: 0x020000B9 RID: 185
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p21 : IReturnParameter<KFCallMsg>
        {
            // Token: 0x17000040 RID: 64
            // (get) Token: 0x060007DC RID: 2012 RVA: 0x00066D80 File Offset: 0x00064F80
            // (set) Token: 0x060007DD RID: 2013 RVA: 0x00066D98 File Offset: 0x00064F98
            [Preserve(Conditional = true)]
            public KFCallMsg Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x0400044E RID: 1102
            [AutoCSer.Json.IgnoreMember]
            public KFCallMsg Ret;
        }

        // Token: 0x020000BA RID: 186
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p22
        {
            // Token: 0x0400044F RID: 1103
            public Dictionary<int, int> p0;

            // Token: 0x04000450 RID: 1104
            public int p1;
        }

        // Token: 0x020000BB RID: 187
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p23
        {
            // Token: 0x04000451 RID: 1105
            public ZhanDuiZhengBaSyncData p0;
        }

        // Token: 0x020000BC RID: 188
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p24 : IReturnParameter<ZhanDuiZhengBaSyncData>
        {
            // Token: 0x17000041 RID: 65
            // (get) Token: 0x060007DE RID: 2014 RVA: 0x00066DA4 File Offset: 0x00064FA4
            // (set) Token: 0x060007DF RID: 2015 RVA: 0x00066DBC File Offset: 0x00064FBC
            [Preserve(Conditional = true)]
            public ZhanDuiZhengBaSyncData Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x04000452 RID: 1106
            [AutoCSer.Json.IgnoreMember]
            public ZhanDuiZhengBaSyncData Ret;
        }

        // Token: 0x020000BD RID: 189
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p25
        {
            // Token: 0x04000453 RID: 1107
            public ZhanDuiZhengBaFuBenData p0;

            // Token: 0x04000454 RID: 1108
            public int p1;

            // Token: 0x04000455 RID: 1109
            public int p2;

            // Token: 0x04000456 RID: 1110
            public int p3;
        }

        // Token: 0x020000BE RID: 190
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p26 : IReturnParameter<int>
        {
            // Token: 0x17000042 RID: 66
            // (get) Token: 0x060007E0 RID: 2016 RVA: 0x00066DC8 File Offset: 0x00064FC8
            // (set) Token: 0x060007E1 RID: 2017 RVA: 0x00066DE0 File Offset: 0x00064FE0
            [Preserve(Conditional = true)]
            public int Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x04000457 RID: 1111
            public ZhanDuiZhengBaFuBenData p0;

            // Token: 0x04000458 RID: 1112
            [AutoCSer.Json.IgnoreMember]
            public int Ret;
        }

        // Token: 0x020000BF RID: 191
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p27
        {
            // Token: 0x04000459 RID: 1113
            public int p0;

            // Token: 0x0400045A RID: 1114
            public int p1;
        }

        // Token: 0x020000C0 RID: 192
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p28 : IReturnParameter<List<ZhanDuiZhengBaNtfPkResultData>>
        {
            // Token: 0x17000043 RID: 67
            // (get) Token: 0x060007E2 RID: 2018 RVA: 0x00066DEC File Offset: 0x00064FEC
            // (set) Token: 0x060007E3 RID: 2019 RVA: 0x00066E04 File Offset: 0x00065004
            [Preserve(Conditional = true)]
            public List<ZhanDuiZhengBaNtfPkResultData> Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x0400045B RID: 1115
            [AutoCSer.Json.IgnoreMember]
            public List<ZhanDuiZhengBaNtfPkResultData> Ret;
        }

        // Token: 0x020000C1 RID: 193
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p29
        {
            // Token: 0x0400045C RID: 1116
            public int p0;

            // Token: 0x0400045D RID: 1117
            public int p1;

            // Token: 0x0400045E RID: 1118
            public int p2;
        }

        // Token: 0x020000C2 RID: 194
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p30
        {
            // Token: 0x0400045F RID: 1119
            public bool p0;

            // Token: 0x04000460 RID: 1120
            public int p1;
        }

        // Token: 0x020000C3 RID: 195
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p31 : IReturnParameter<string>
        {
            // Token: 0x17000044 RID: 68
            // (get) Token: 0x060007E4 RID: 2020 RVA: 0x00066E10 File Offset: 0x00065010
            // (set) Token: 0x060007E5 RID: 2021 RVA: 0x00066E28 File Offset: 0x00065028
            [Preserve(Conditional = true)]
            public string Return
            {
                get
                {
                    return this.Ret;
                }
                set
                {
                    this.Ret = value;
                }
            }

            // Token: 0x04000461 RID: 1121
            [AutoCSer.Json.IgnoreMember]
            public string Ret;
        }
    }
}
