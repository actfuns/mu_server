using System;
using System.Text;
using System.Threading;
using GameDBServer.Data.Tarot;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Tarot
{
	// Token: 0x0200017B RID: 379
	public class TarotManager
	{
		// Token: 0x060006AD RID: 1709 RVA: 0x0003CFBC File Offset: 0x0003B1BC
		public static TCPProcessCmdResults ProcessUpdateTarotDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 20100);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				string dataInfo = fields[1];
				string kingBuffInfo = fields[2];
				string dbTarotInfo = string.Empty;
				string dbKingInfo = string.Empty;
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null != dbRoleInfo)
				{
					bool flag = false;
					try
					{
						DBRoleInfo obj;
						Monitor.Enter(obj = dbRoleInfo, ref flag);
						TarotCardData tarotData = new TarotCardData(dataInfo);
						if (tarotData.GoodId > 0)
						{
							TarotCardData oldData = dbRoleInfo.TarotData.TarotCardDatas.Find((TarotCardData x) => x.GoodId == tarotData.GoodId);
							if (oldData == null)
							{
								dbRoleInfo.TarotData.TarotCardDatas.Add(tarotData);
							}
							else
							{
								TarotCardData targetData = dbRoleInfo.TarotData.TarotCardDatas.Find((TarotCardData x) => x.Postion == tarotData.Postion);
								if (targetData != null)
								{
									targetData.Postion = 0;
								}
								oldData.Level = tarotData.Level;
								oldData.Postion = tarotData.Postion;
								oldData.TarotMoney = tarotData.TarotMoney;
							}
						}
						foreach (TarotCardData info in dbRoleInfo.TarotData.TarotCardDatas)
						{
							dbTarotInfo += info.GetDataStrInfo();
						}
						if (kingBuffInfo != "-1")
						{
							TarotKingData kingBuffData = new TarotKingData(kingBuffInfo);
							dbRoleInfo.TarotData.KingData = kingBuffData;
						}
					}
					finally
					{
						if (flag)
						{
							DBRoleInfo obj;
							Monitor.Exit(obj);
						}
					}
				}
				string strcmd = DBWriter.UpdateTarotData(dbMgr, roleID, dbTarotInfo, dbRoleInfo.TarotData.KingData.GetDataStrInfo()) ? "1" : "0";
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
