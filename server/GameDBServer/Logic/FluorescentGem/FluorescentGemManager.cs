using System;
using System.Text;
using System.Threading;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.FluorescentGem
{
    // Token: 0x0200012D RID: 301
    public class FluorescentGemManager
    {
        // Token: 0x060004F6 RID: 1270 RVA: 0x00028FC8 File Offset: 0x000271C8
        public static FluorescentGemManager getInstance()
        {
            return FluorescentGemManager.instance;
        }

        // Token: 0x060004F7 RID: 1271 RVA: 0x00028FE0 File Offset: 0x000271E0
        public TCPProcessCmdResults ProcessResetBagDataCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            client.sendCmd<bool>(nID, false);
            return TCPProcessCmdResults.RESULT_OK;
        }

        // Token: 0x060004F8 RID: 1272 RVA: 0x00029000 File Offset: 0x00027200
        public TCPProcessCmdResults ProcessModifyFluorescentPointCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;
            bool bRet = false;
            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
                client.sendCmd<bool>(nID, bRet);
                return TCPProcessCmdResults.RESULT_OK;
            }
            try
            {
                string[] fields = cmdData.Split(new char[]
                {
                    ':'
                });
                if (fields.Length != 2)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
                    client.sendCmd<bool>(nID, bRet);
                    return TCPProcessCmdResults.RESULT_OK;
                }
                int nRoleID = Convert.ToInt32(fields[0]);
                int nPointChg = Convert.ToInt32(fields[1]);
                DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
                if (null == dbRoleInfo)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, nRoleID), null, true);
                    client.sendCmd<bool>(nID, bRet);
                    return TCPProcessCmdResults.RESULT_OK;
                }
                lock (dbRoleInfo)
                {
                    dbRoleInfo.FluorescentPoint += nPointChg;
                    bRet = FluorescentGemDBOperate.UpdateFluorescentPoint(dbMgr, nRoleID, dbRoleInfo.FluorescentPoint);
                }
                client.sendCmd<bool>(nID, bRet);
                return TCPProcessCmdResults.RESULT_OK;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "", false, false);
            }
            client.sendCmd<bool>(nID, bRet);
            return TCPProcessCmdResults.RESULT_OK;
        }

        // Token: 0x060004F9 RID: 1273 RVA: 0x000291D4 File Offset: 0x000273D4
        public TCPProcessCmdResults ProcessUpdateFluorescentPointCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            string cmdData = null;
            bool bRet = false;
            try
            {
                cmdData = new UTF8Encoding().GetString(data, 0, count);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
                client.sendCmd<bool>(nID, bRet);
                return TCPProcessCmdResults.RESULT_OK;
            }
            try
            {
                string[] fields = cmdData.Split(new char[]
                {
                    ':'
                });
                if (fields.Length != 2)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
                    client.sendCmd<bool>(nID, bRet);
                    return TCPProcessCmdResults.RESULT_OK;
                }
                int nRoleID = Convert.ToInt32(fields[0]);
                int nPoint = Convert.ToInt32(fields[1]);
                DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
                if (null == dbRoleInfo)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, nRoleID), null, true);
                    client.sendCmd<bool>(nID, bRet);
                    return TCPProcessCmdResults.RESULT_OK;
                }
                bRet = FluorescentGemDBOperate.UpdateFluorescentPoint(dbMgr, nRoleID, nPoint);
                lock (dbRoleInfo)
                {
                    dbRoleInfo.FluorescentPoint = nPoint;
                }
                client.sendCmd<bool>(nID, bRet);
                return TCPProcessCmdResults.RESULT_OK;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "", false, false);
            }
            client.sendCmd<bool>(nID, bRet);
            return TCPProcessCmdResults.RESULT_OK;
        }

        // Token: 0x060004FA RID: 1274 RVA: 0x00029378 File Offset: 0x00027578
        public TCPProcessCmdResults ProcessEquipGemCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            bool bRet = false;
            FluorescentGemSaveDBData gemData = null;
            try
            {
                gemData = DataHelper.BytesToObject<FluorescentGemSaveDBData>(data, 0, count);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
                client.sendCmd<bool>(nID, bRet);
                return TCPProcessCmdResults.RESULT_OK;
            }
            try
            {
                DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref gemData._RoleID);
                if (null == dbRoleInfo)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, gemData._RoleID), null, true);
                    client.sendCmd<bool>(nID, bRet);
                    return TCPProcessCmdResults.RESULT_OK;
                }
                bRet = FluorescentGemDBOperate.EquipFluorescentGem(dbMgr, gemData);
                if (bRet)
                {
                    lock (dbRoleInfo)
                    {
                        GoodsData goodsData = new GoodsData();
                        goodsData.GoodsID = gemData._GoodsID;
                        goodsData.GCount = 1;
                        goodsData.Binding = gemData._Bind;
                        goodsData.Site = 7001;
                        goodsData.BagIndex = this.GenerateBagIndex(gemData._Position, gemData._GemType);
                        dbRoleInfo.FluorescentGemData.GemEquipList.Add(goodsData);
                    }
                }
                client.sendCmd<bool>(nID, bRet);
                return TCPProcessCmdResults.RESULT_OK;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "", false, false);
            }
            client.sendCmd<bool>(nID, bRet);
            return TCPProcessCmdResults.RESULT_OK;
        }

        // Token: 0x060004FB RID: 1275 RVA: 0x0002953C File Offset: 0x0002773C
        public TCPProcessCmdResults ProcessUnEquipGemCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            bool bRet = false;
            FluorescentGemSaveDBData gemData = null;
            try
            {
                gemData = DataHelper.BytesToObject<FluorescentGemSaveDBData>(data, 0, count);
            }
            catch (Exception)
            {
                LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
                client.sendCmd<bool>(nID, bRet);
                return TCPProcessCmdResults.RESULT_OK;
            }
            try
            {
                DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref gemData._RoleID);
                if (null == dbRoleInfo)
                {
                    LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, gemData._RoleID), null, true);
                    client.sendCmd<bool>(nID, bRet);
                    return TCPProcessCmdResults.RESULT_OK;
                }
                bRet = FluorescentGemDBOperate.UnEquipFluorescentGem(dbMgr, gemData);
                if (bRet)
                {
                    lock (dbRoleInfo)
                    {
                        int slot = this.GenerateBagIndex(gemData._Position, gemData._GemType);
                        dbRoleInfo.FluorescentGemData.GemEquipList.RemoveAll((GoodsData _g) => _g.BagIndex == slot);
                    }
                }
                client.sendCmd<bool>(nID, bRet);
                return TCPProcessCmdResults.RESULT_OK;
            }
            catch (Exception ex)
            {
                DataHelper.WriteFormatExceptionLog(ex, "", false, false);
            }
            client.sendCmd<bool>(nID, bRet);
            return TCPProcessCmdResults.RESULT_OK;
        }

        // Token: 0x060004FC RID: 1276 RVA: 0x000296B4 File Offset: 0x000278B4
        public void ParsePosAndType(int slot, out int pos, out int type)
        {
            pos = slot / 100;
            type = slot % 100;
        }

        // Token: 0x060004FD RID: 1277 RVA: 0x000296C4 File Offset: 0x000278C4
        public int GenerateBagIndex(int pos, int type)
        {
            return pos * 100 + type;
        }

        // Token: 0x040007C6 RID: 1990
        private static FluorescentGemManager instance = new FluorescentGemManager();
    }
}
