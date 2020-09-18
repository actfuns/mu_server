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
    
    public class FluorescentGemManager
    {
        
        public static FluorescentGemManager getInstance()
        {
            return FluorescentGemManager.instance;
        }

        
        public TCPProcessCmdResults ProcessResetBagDataCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
        {
            tcpOutPacket = null;
            client.sendCmd<bool>(nID, false);
            return TCPProcessCmdResults.RESULT_OK;
        }

        
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

        
        public void ParsePosAndType(int slot, out int pos, out int type)
        {
            pos = slot / 100;
            type = slot % 100;
        }

        
        public int GenerateBagIndex(int pos, int type)
        {
            return pos * 100 + type;
        }

        
        private static FluorescentGemManager instance = new FluorescentGemManager();
    }
}
