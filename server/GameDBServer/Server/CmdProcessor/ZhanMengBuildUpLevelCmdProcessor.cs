using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001FE RID: 510
	public class ZhanMengBuildUpLevelCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A8A RID: 2698 RVA: 0x00064CE4 File Offset: 0x00062EE4
		private ZhanMengBuildUpLevelCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(601, this);
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x00064D00 File Offset: 0x00062F00
		public static ZhanMengBuildUpLevelCmdProcessor getInstance()
		{
			return ZhanMengBuildUpLevelCmdProcessor.instance;
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x00064D18 File Offset: 0x00062F18
		private bool CheckHaveUpGradeItem(string strReqItem, DBManager dbMgr, int nBangHuiID, int nRoleID, int nToLevel)
		{
			BangHuiBagData dataBangHuiBag = DBQuery.QueryBangHuiBagDataByID(dbMgr, nBangHuiID);
			string[] arrReqItems = strReqItem.Split(new char[]
			{
				'|'
			});
			int[] arrItemNums = new int[5];
			for (int i = 0; i < arrItemNums.Length; i++)
			{
				arrItemNums[i] = 0;
			}
			for (int i = 0; i < arrReqItems.Length; i++)
			{
				string[] arrItemInfo = arrReqItems[i].Split(new char[]
				{
					','
				});
				if (2 == arrItemInfo.Length)
				{
					arrItemNums[i] = int.Parse(arrItemInfo[1]);
				}
			}
			bool result;
			if (dataBangHuiBag.Goods1Num < arrItemNums[0])
			{
				result = false;
			}
			else if (dataBangHuiBag.Goods2Num < arrItemNums[1])
			{
				result = false;
			}
			else if (dataBangHuiBag.Goods3Num < arrItemNums[2])
			{
				result = false;
			}
			else if (dataBangHuiBag.Goods4Num < arrItemNums[3])
			{
				result = false;
			}
			else if (dataBangHuiBag.Goods5Num < arrItemNums[4])
			{
				result = false;
			}
			else
			{
				DBWriter.UpdateBangHuiQiLevel(dbMgr, nBangHuiID, nToLevel, arrItemNums[0], arrItemNums[1], arrItemNums[2], arrItemNums[3], arrItemNums[4], 0);
				result = true;
			}
			return result;
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x00064E58 File Offset: 0x00063058
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd(30767, "0");
				return;
			}
			string[] fields = cmdData.Split(new char[]
			{
				':'
			});
			if (fields.Length != 7)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int roleID = Convert.ToInt32(fields[0]);
				int bhid = Convert.ToInt32(fields[1]);
				int buildType = Convert.ToInt32(fields[2]);
				int levelupCost = Convert.ToInt32(fields[3]);
				int toLevel = Convert.ToInt32(fields[4]);
				int initCoin = Convert.ToInt32(fields[5]);
				string strReqGoods = fields[6];
				DBManager dbMgr = DBManager.getInstance();
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					client.sendCmd(30767, "0");
				}
				else
				{
					BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
					if (null == bangHuiDetailData)
					{
						string strcmd = string.Format("{0}", -1000);
						client.sendCmd(nID, strcmd);
					}
					else if (roleID != bangHuiDetailData.BZRoleID)
					{
						string strcmd = string.Format("{0}", -9368);
						client.sendCmd(nID, strcmd);
					}
					else
					{
						int currentLevel = bangHuiDetailData.QiLevel;
						if (buildType == 1)
						{
							if (bangHuiDetailData.JiTan < bangHuiDetailData.QiLevel || bangHuiDetailData.JunXie < bangHuiDetailData.QiLevel || bangHuiDetailData.GuangHuan < bangHuiDetailData.QiLevel)
							{
								string strcmd = string.Format("{0}", -1005);
								client.sendCmd(nID, strcmd);
								return;
							}
							currentLevel = bangHuiDetailData.QiLevel;
						}
						else if (buildType == 2)
						{
							if (bangHuiDetailData.JiTan >= bangHuiDetailData.QiLevel)
							{
								string strcmd = string.Format("{0}", -1005);
								client.sendCmd(nID, strcmd);
								return;
							}
							currentLevel = bangHuiDetailData.JiTan;
						}
						else if (buildType == 3)
						{
							if (bangHuiDetailData.JunXie >= bangHuiDetailData.QiLevel)
							{
								string strcmd = string.Format("{0}", -1005);
								client.sendCmd(nID, strcmd);
								return;
							}
							currentLevel = bangHuiDetailData.JunXie;
						}
						else if (buildType == 4)
						{
							if (bangHuiDetailData.GuangHuan >= bangHuiDetailData.QiLevel)
							{
								string strcmd = string.Format("{0}", -1005);
								client.sendCmd(nID, strcmd);
								return;
							}
							currentLevel = bangHuiDetailData.GuangHuan;
						}
						if (currentLevel + 1 != toLevel)
						{
							string strcmd = string.Format("{0}", -1005);
							client.sendCmd(nID, strcmd);
						}
						else if (bangHuiDetailData.TotalMoney < levelupCost + initCoin)
						{
							string strcmd = string.Format("{0}", -1120);
							client.sendCmd(nID, strcmd);
						}
						else if (bangHuiDetailData.TotalMoney < levelupCost)
						{
							string strcmd = string.Format("{0}", -1110);
							client.sendCmd(nID, strcmd);
						}
						else if (!this.CheckHaveUpGradeItem(strReqGoods, dbMgr, bhid, roleID, bangHuiDetailData.QiLevel))
						{
							string strcmd = string.Format("{0}", -1210);
							client.sendCmd(nID, strcmd);
						}
						else
						{
							string strcmd;
							string fieldName;
							if (buildType == 1)
							{
								fieldName = "qilevel";
							}
							else if (buildType == 2)
							{
								fieldName = "jitan";
							}
							else if (buildType == 3)
							{
								fieldName = "junxie";
							}
							else
							{
								if (buildType != 4)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("ZhanMengBuildUpLevelCmdProcessor::processCmd Param Error: buildType={0}", buildType), null, true);
									strcmd = string.Format("{0}", -1310);
									client.sendCmd(nID, strcmd);
									return;
								}
								fieldName = "guanghuan";
							}
							DBWriter.UpdateZhanMengBuildLevel(dbMgr, bhid, toLevel, levelupCost, fieldName);
							if (buildType == 1)
							{
								GameDBManager.BangHuiJunQiMgr.UpdateBangHuiQiLevel(bhid, toLevel);
							}
							strcmd = string.Format("{0}", 0);
							client.sendCmd(nID, strcmd);
						}
					}
				}
			}
		}

		// Token: 0x04000C6D RID: 3181
		private static ZhanMengBuildUpLevelCmdProcessor instance = new ZhanMengBuildUpLevelCmdProcessor();
	}
}
