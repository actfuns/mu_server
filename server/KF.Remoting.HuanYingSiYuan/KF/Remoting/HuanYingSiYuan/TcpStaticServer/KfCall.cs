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
    
    public class KfCall : AutoCSer.Net.TcpInternalServer.Server
    {
        
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

        
        private static readonly OutputInfo _c1 = new OutputInfo
        {
            OutputParameterIndex = 2,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c2 = new OutputInfo
        {
            OutputParameterIndex = 4,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c3 = new OutputInfo
        {
            OutputParameterIndex = 6,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c4 = new OutputInfo
        {
            OutputParameterIndex = 8,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c5 = new OutputInfo
        {
            OutputParameterIndex = 10,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c6 = new OutputInfo
        {
            OutputParameterIndex = 10,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c7 = new OutputInfo
        {
            OutputParameterIndex = 13,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c8 = new OutputInfo
        {
            OutputParameterIndex = 14,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c9 = new OutputInfo
        {
            OutputParameterIndex = 16,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c10 = new OutputInfo
        {
            OutputParameterIndex = 17,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c11 = new OutputInfo
        {
            OutputParameterIndex = 10,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c12 = new OutputInfo
        {
            OutputParameterIndex = 6,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c13 = new OutputInfo
        {
            OutputParameterIndex = 10,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c14 = new OutputInfo
        {
            OutputParameterIndex = 21,
            IsKeepCallback = 1,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c15 = new OutputInfo
        {
            OutputParameterIndex = 0,
            IsClientSendOnly = 1,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c16 = new OutputInfo
        {
            OutputParameterIndex = 24,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c17 = new OutputInfo
        {
            OutputParameterIndex = 26,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c18 = new OutputInfo
        {
            OutputParameterIndex = 28,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c19 = new OutputInfo
        {
            OutputParameterIndex = 8,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c20 = new OutputInfo
        {
            OutputParameterIndex = 6,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c21 = new OutputInfo
        {
            OutputParameterIndex = 6,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c22 = new OutputInfo
        {
            OutputParameterIndex = 6,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        
        private static readonly OutputInfo _c23 = new OutputInfo
        {
            OutputParameterIndex = 31,
            IsSimpleSerializeOutputParamter = true,
            IsBuildOutputThread = true
        };

        
        private sealed class _s0 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s0, KfCall._p1>
        {
            
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

        
        private sealed class _s1 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s1, KfCall._p3>
        {
            
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

        
        private sealed class _s2 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s2, KfCall._p5>
        {
            
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

        
        private sealed class _s3 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s3, KfCall._p7>
        {
            
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

        
        private sealed class _s4 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s4, KfCall._p9>
        {
            
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

        
        private sealed class _s5 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s5, KfCall._p11>
        {
            
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

        
        private sealed class _s6 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s6, KfCall._p12>
        {
            
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

        
        private sealed class _s7 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s7, KfCall._p12>
        {
            
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

        
        private sealed class _s8 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s8, KfCall._p15>
        {
            
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

        
        private sealed class _s9 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s9, KfCall._p12>
        {
            
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

        
        private sealed class _s10 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s10, KfCall._p18>
        {
            
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

        
        private sealed class _s11 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s11, KfCall._p19>
        {
            
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

        
        private sealed class _s14 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s14, KfCall._p22>
        {
            
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

        
        private sealed class _s15 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s15, KfCall._p23>
        {
            
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

        
        private sealed class _s16 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s16, KfCall._p25>
        {
            
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

        
        private sealed class _s17 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s17, KfCall._p27>
        {
            
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

        
        private sealed class _s18 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s18, KfCall._p7>
        {
            
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

        
        private sealed class _s19 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s19, KfCall._p29>
        {
            
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

        
        private sealed class _s20 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s20, KfCall._p27>
        {
            
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

        
        private sealed class _s21 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s21, KfCall._p12>
        {
            
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

        
        private sealed class _s22 : AutoCSer.Net.TcpStaticServer.ServerCall<KfCall._s22, KfCall._p30>
        {
            
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

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p1
        {
            
            public EscapeBattleSyncData p0;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p2 : IReturnParameter<EscapeBattleSyncData>
        {
            
            
            
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

            
            [AutoCSer.Json.IgnoreMember]
            public EscapeBattleSyncData Ret;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p3
        {
            
            public EscapeBattleFuBenData p0;

            
            public int p1;

            
            public int p2;

            
            public int p3;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p4 : IReturnParameter<int>
        {
            
            
            
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

            
            public EscapeBattleFuBenData p0;

            
            [AutoCSer.Json.IgnoreMember]
            public int Ret;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p5
        {
            
            public List<int> p0;

            
            public int p1;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p6 : IReturnParameter<int>
        {
            
            
            
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

            
            [AutoCSer.Json.IgnoreMember]
            public int Ret;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p7
        {
            
            public int p0;

            
            public int p1;

            
            public int p2;

            
            public int[] p3;

            
            public string[] p4;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p8 : IReturnParameter<int>
        {
            
            
            
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

            
            public int p0;

            
            public int p1;

            
            public int[] p2;

            
            public string[] p3;

            
            [AutoCSer.Json.IgnoreMember]
            public int Ret;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p9
        {
            
            public KFBoCaiShopDB p0;

            
            public int p1;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p10 : IReturnParameter<bool>
        {
            
            
            
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

            
            [AutoCSer.Json.IgnoreMember]
            public bool Ret;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p11
        {
            
            public KFBuyBocaiData p0;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p12
        {
            
            public int p0;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p13 : IReturnParameter<KFStageData>
        {
            
            
            
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

            
            [AutoCSer.Json.IgnoreMember]
            public KFStageData Ret;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p14 : IReturnParameter<OpenLottery>
        {
            
            
            
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

            
            [AutoCSer.Json.IgnoreMember]
            public OpenLottery Ret;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p15
        {
            
            public bool p0;

            
            public int p1;

            
            public long p2;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p16 : IReturnParameter<List<OpenLottery>>
        {
            
            
            
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

            
            [AutoCSer.Json.IgnoreMember]
            public List<OpenLottery> Ret;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p17 : IReturnParameter<List<KFBoCaoHistoryData>>
        {
            
            
            
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

            
            [AutoCSer.Json.IgnoreMember]
            public List<KFBoCaoHistoryData> Ret;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p18
        {
            
            public int p0;

            
            public int p1;

            
            public long p2;

            
            public string p3;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p19
        {
            
            public string[] p0;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p20
        {
            
            public KuaFuClientContext p0;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p21 : IReturnParameter<KFCallMsg>
        {
            
            
            
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

            
            [AutoCSer.Json.IgnoreMember]
            public KFCallMsg Ret;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p22
        {
            
            public Dictionary<int, int> p0;

            
            public int p1;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p23
        {
            
            public ZhanDuiZhengBaSyncData p0;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p24 : IReturnParameter<ZhanDuiZhengBaSyncData>
        {
            
            
            
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

            
            [AutoCSer.Json.IgnoreMember]
            public ZhanDuiZhengBaSyncData Ret;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p25
        {
            
            public ZhanDuiZhengBaFuBenData p0;

            
            public int p1;

            
            public int p2;

            
            public int p3;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p26 : IReturnParameter<int>
        {
            
            
            
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

            
            public ZhanDuiZhengBaFuBenData p0;

            
            [AutoCSer.Json.IgnoreMember]
            public int Ret;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p27
        {
            
            public int p0;

            
            public int p1;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p28 : IReturnParameter<List<ZhanDuiZhengBaNtfPkResultData>>
        {
            
            
            
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

            
            [AutoCSer.Json.IgnoreMember]
            public List<ZhanDuiZhengBaNtfPkResultData> Ret;
        }

        
        [BoxSerialize]
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p29
        {
            
            public int p0;

            
            public int p1;

            
            public int p2;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p30
        {
            
            public bool p0;

            
            public int p1;
        }

        
        [AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
        [BoxSerialize]
        [StructLayout(LayoutKind.Auto)]
        internal struct _p31 : IReturnParameter<string>
        {
            
            
            
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

            
            [AutoCSer.Json.IgnoreMember]
            public string Ret;
        }
    }
}
