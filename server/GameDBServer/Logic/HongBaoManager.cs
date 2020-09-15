using System;
using System.Collections.Generic;
using System.Threading;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameDBServer.Logic
{
	// Token: 0x0200016D RID: 365
	public class HongBaoManager : SingletonTemplate<HongBaoManager>, IManager, ICmdProcessor
	{
		// Token: 0x06000654 RID: 1620 RVA: 0x000397E8 File Offset: 0x000379E8
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x000397FC File Offset: 0x000379FC
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1430, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1431, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1432, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1433, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1434, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1435, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1436, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1437, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1428, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1438, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1439, SingletonTemplate<HongBaoManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(1440, SingletonTemplate<HongBaoManager>.Instance());
			if (null == this.WorkThread)
			{
				this.WorkThread = new Thread(new ThreadStart(this.ThreadStart));
				this.WorkThread.IsBackground = true;
				this.WorkThread.Start();
			}
			return true;
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00039950 File Offset: 0x00037B50
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00039964 File Offset: 0x00037B64
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x00039978 File Offset: 0x00037B78
		private void ThreadStart()
		{
			for (;;)
			{
				Thread.Sleep(2000);
			}
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x00039998 File Offset: 0x00037B98
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			switch (nID)
			{
			case 1428:
				this.GetJieRiHongBaoBangAwards(client, nID, cmdParams, count);
				break;
			case 1430:
				this.GetZhanMengHongBaoRankList(client, nID, cmdParams, count);
				break;
			case 1431:
				this.GetJieRiHongBaoRankList(client, nID, cmdParams, count);
				break;
			case 1432:
				this.UpdateZhanMengHongBao(client, nID, cmdParams, count);
				break;
			case 1433:
				this.RecvZhanMengHongBao(client, nID, cmdParams, count);
				break;
			case 1434:
				this.GetZhanMengHongBaoList(client, nID, cmdParams, count);
				break;
			case 1435:
				this.UpdateJieRiHongBao(client, nID, cmdParams, count);
				break;
			case 1436:
				this.RecvJieRiHongBao(client, nID, cmdParams, count);
				break;
			case 1437:
				this.GetJieRiHongBaoList(client, nID, cmdParams, count);
				break;
			case 1438:
				this.GetZhanMengHongBaoLogList(client, nID, cmdParams, count);
				break;
			case 1439:
				this.GetZhanMengHongBaoRecvList(client, nID, cmdParams, count);
				break;
			case 1440:
				this.GetJieRiHongBaoCount(client, nID, cmdParams, count);
				break;
			}
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00039AAC File Offset: 0x00037CAC
		private void GetZhanMengHongBaoRankList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<HongBaoRankItemData> list = new List<HongBaoRankItemData>();
			try
			{
				ZhanMengHongBaoRankListQueryData args = DataHelper.BytesToObject<ZhanMengHongBaoRankListQueryData>(cmdParams, 0, count);
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText;
					if (args.Type == 1)
					{
						cmdText = string.Format("SELECT `senderid`,`sendername`,MAX(`sendtime`),SUM(`zuanshi`) FROM `t_hongbao_send` WHERE `bhid`={0} and sendtime>='{1}' GROUP BY `senderid` ORDER BY SUM(`zuanshi`) DESC,MAX(`sendtime`) ASC limit {2};", args.Bhid, args.StartTime.ToString("yyyy-MM-dd"), args.Count);
					}
					else
					{
						cmdText = string.Format("SELECT `rid`,`rname`,MAX(`recvtime`),SUM(`zuanshi`) FROM `t_hongbao_recv` WHERE `bhid`={0} AND recvtime>='{1}' GROUP BY `rid` ORDER BY SUM(`zuanshi`) DESC,MAX(`recvtime`) ASC limit {2};", args.Bhid, args.StartTime.ToString("yyyy-MM-dd"), args.Count);
					}
					int ranking = 1;
					MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
					while (reader.Read())
					{
						HongBaoRankItemData data = new HongBaoRankItemData();
						data.roleId = Global.SafeConvertToInt32(reader[0].ToString(), 10);
						if (data.roleId > 0)
						{
							data.roleName = reader[1].ToString();
							data.daimondNum = Global.SafeConvertToInt32(reader[3].ToString(), 10);
							data.rankID = ranking++;
							list.Add(data);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<List<HongBaoRankItemData>>(nID, list);
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x00039C5C File Offset: 0x00037E5C
		private void GetZhanMengHongBaoLogList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<HongBaoItemData> list = new List<HongBaoItemData>();
			try
			{
				List<string> args = DataHelper.BytesToObject<List<string>>(cmdParams, 0, count);
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText;
					if (args[0] == "0")
					{
						cmdText = string.Format("SELECT hongbaoid FROM t_hongbao_recv WHERE bhid={0} AND rid={1} order by hongbaoid desc limit {2}", args[1], args[2], args[3]);
						List<int> hongBaoIdList = new List<int>();
						MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
						while (reader.Read())
						{
							HongBaoItemData data = new HongBaoItemData();
							hongBaoIdList.Add(Global.SafeConvertToInt32(reader[0].ToString(), 10));
						}
						reader.Close();
						if (hongBaoIdList.Count > 0)
						{
							cmdText = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE id IN ({0}) order by id desc;", string.Join<int>(",", hongBaoIdList));
						}
						else
						{
							cmdText = "";
						}
					}
					else if (args[0] == "1")
					{
						cmdText = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE bhid={0} AND senderid={1} order by id desc limit {2};", args[1], args[2], args[3]);
					}
					else
					{
						cmdText = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE bhid={0} order by id desc limit {2};", args[1], args[2], args[3]);
					}
					if (!string.IsNullOrEmpty(cmdText))
					{
						MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
						while (reader.Read())
						{
							list.Add(new HongBaoItemData
							{
								hongBaoID = Global.SafeConvertToInt32(reader[0].ToString(), 10),
								sender = reader[3].ToString(),
								beginTime = Global.SafeConvertToDateTime(reader[4].ToString(), DateTime.MinValue),
								endTime = Global.SafeConvertToDateTime(reader[5].ToString(), DateTime.MinValue),
								diamondSumCount = Global.SafeConvertToInt32(reader[7].ToString(), 10),
								type = Global.SafeConvertToInt32(reader[9].ToString(), 10),
								hongBaoStatus = Global.SafeConvertToInt32(reader[12].ToString(), 10)
							});
						}
						reader.Close();
						foreach (HongBaoItemData data in list)
						{
							cmdText = string.Format("SELECT `zuanshi` FROM t_hongbao_recv WHERE hongbaoid={0} and rid={1};", data.hongBaoID, args[2]);
							MySQLDataReader mySQLDataReader;
							reader = (mySQLDataReader = conn.ExecuteReader(cmdText, new MySQLParameter[0]));
							try
							{
								if (reader.Read())
								{
									data.diamondCount = Global.SafeConvertToInt32(reader[0].ToString(), 10);
								}
							}
							finally
							{
								if (mySQLDataReader != null)
								{
									((IDisposable)mySQLDataReader).Dispose();
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<List<HongBaoItemData>>(nID, list);
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0003A004 File Offset: 0x00038204
		private void GetZhanMengHongBaoRecvList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			HongBaoSendData data = new HongBaoSendData();
			try
			{
				int hongBaoId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE id={0};", hongBaoId);
					MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
					while (reader.Read())
					{
						data.hongBaoID = Global.SafeConvertToInt32(reader[0].ToString(), 10);
						data.bhid = Global.SafeConvertToInt32(reader[1].ToString(), 10);
						data.senderID = Global.SafeConvertToInt32(reader[2].ToString(), 10);
						data.sender = reader[3].ToString();
						data.sendTime = Global.SafeConvertToDateTime(reader[4].ToString(), DateTime.MinValue);
						data.endTime = Global.SafeConvertToDateTime(reader[5].ToString(), DateTime.MinValue);
						data.message = reader[6].ToString();
						data.sumDiamondNum = Global.SafeConvertToInt32(reader[7].ToString(), 10);
						data.sumCount = Global.SafeConvertToInt32(reader[8].ToString(), 10);
						data.type = Global.SafeConvertToInt32(reader[9].ToString(), 10);
						data.leftZuanShi = Global.SafeConvertToInt32(reader[10].ToString(), 10);
						data.leftCount = Global.SafeConvertToInt32(reader[11].ToString(), 10);
						data.hongBaoStatus = Global.SafeConvertToInt32(reader[12].ToString(), 10);
					}
					reader.Close();
					data.RecvList = new List<HongBaoRecvData>();
					cmdText = string.Format("SELECT `bhid`,`rid`,`rname`,`recvtime`,`zuanshi` FROM t_hongbao_recv WHERE hongbaoid={0};", hongBaoId);
					MySQLDataReader mySQLDataReader;
					reader = (mySQLDataReader = conn.ExecuteReader(cmdText, new MySQLParameter[0]));
					try
					{
						while (reader.Read())
						{
							HongBaoRecvData recvData = new HongBaoRecvData();
							recvData.HongBaoID = Global.SafeConvertToInt32(reader[0].ToString(), 10);
							recvData.RoleId = Global.SafeConvertToInt32(reader[1].ToString(), 10);
							recvData.RoleName = reader[2].ToString();
							recvData.RecvTime = Global.SafeConvertToDateTime(reader[3].ToString(), DateTime.MinValue);
							recvData.ZuanShi = Global.SafeConvertToInt32(reader[4].ToString(), 10);
							data.RecvList.Add(recvData);
						}
					}
					finally
					{
						if (mySQLDataReader != null)
						{
							((IDisposable)mySQLDataReader).Dispose();
						}
					}
					reader.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<HongBaoSendData>(nID, data);
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0003A338 File Offset: 0x00038538
		private void GetJieRiHongBaoRankList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<JieriHongBaoKingItemData> list = new List<JieriHongBaoKingItemData>();
			try
			{
				List<string> args = DataHelper.BytesToObject<List<string>>(cmdParams, 0, count);
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText = string.Format("SELECT `rid`,`count`,`getawardtimes`,`lasttime`,`rname` FROM `t_hongbao_jieri_recv` WHERE `keystr`='{0}' ORDER BY `count` DESC,`lasttime` ASC,rid ASC limit {1};", args[0], args[1]);
					using (MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]))
					{
						int ranking = 1;
						while (reader.Read())
						{
							list.Add(new JieriHongBaoKingItemData
							{
								RoleID = Global.SafeConvertToInt32(reader[0].ToString(), 10),
								TotalRecv = Global.SafeConvertToInt32(reader[1].ToString(), 10),
								GetAwardTimes = Global.SafeConvertToInt32(reader[2].ToString(), 10),
								Rolename = reader[4].ToString(),
								Rank = ranking++
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<List<JieriHongBaoKingItemData>>(nID, list);
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x0003A4C0 File Offset: 0x000386C0
		private void UpdateJieRiHongBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			HongBaoSendData data = DataHelper.BytesToObject<HongBaoSendData>(cmdParams, 0, count);
			if (data != null)
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText;
					if (data.hongBaoID == 0)
					{
						cmdText = string.Format("insert into `t_hongbao_jieri_send` (`keystr`, `senderid`, `sendtime`, `endtime`, `msg`, `zuanshi`, `type`, `leftzuanshi`, `state`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", new object[]
						{
							data.sender,
							data.senderID,
							data.sendTime.ToString("yyyy-MM-dd HH:mm:ss"),
							data.endTime.ToString("yyyy-MM-dd HH:mm:ss"),
							data.message,
							data.sumDiamondNum,
							data.type,
							data.leftZuanShi,
							0
						});
					}
					else
					{
						cmdText = string.Format("replace into `t_hongbao_jieri_send` (`id`,`keystr`, `senderid`, `sendtime`, `endtime`, `msg`, `zuanshi`, `type`, `leftzuanshi`, `state`) values({9},'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');", new object[]
						{
							data.sender,
							data.senderID,
							data.sendTime.ToString("yyyy-MM-dd HH:mm:ss"),
							data.endTime.ToString("yyyy-MM-dd HH:mm:ss"),
							data.message,
							data.sumDiamondNum,
							data.type,
							data.leftZuanShi,
							data.hongBaoStatus,
							data.hongBaoID
						});
					}
					ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
					if (ret >= 0 && data.hongBaoID == 0)
					{
						cmdText = "SELECT LAST_INSERT_ID();";
						ret = conn.GetSingleInt(cmdText, 0, new MySQLParameter[0]);
					}
				}
			}
			client.sendCmd<int>(nID, ret);
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0003A6D0 File Offset: 0x000388D0
		private void UpdateZhanMengHongBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			HongBaoSendData data = DataHelper.BytesToObject<HongBaoSendData>(cmdParams, 0, count);
			if (data != null)
			{
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText;
					if (data.hongBaoID == 0)
					{
						cmdText = string.Format("insert into `t_hongbao_send` (`bhid`, `senderid`, `sendername`, `sendtime`, `endtime`, `msg`, `zuanshi`, `count`, `type`, `leftzuanshi`, `leftcount`, `state`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','0');", new object[]
						{
							data.bhid,
							data.senderID,
							data.sender,
							data.sendTime.ToString("yyyy-MM-dd HH:mm:ss"),
							data.endTime.ToString("yyyy-MM-dd HH:mm:ss"),
							data.message,
							data.sumDiamondNum,
							data.sumCount,
							data.type,
							data.leftZuanShi,
							data.leftCount
						});
					}
					else
					{
						cmdText = string.Format("replace into `t_hongbao_send` (`bhid`, `senderid`, `sendername`, `sendtime`, `endtime`, `msg`, `zuanshi`, `count`, `type`, `leftzuanshi`, `leftcount`, `state`,`id`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',{12});", new object[]
						{
							data.bhid,
							data.senderID,
							data.sender,
							data.sendTime.ToString("yyyy-MM-dd HH:mm:ss"),
							data.endTime.ToString("yyyy-MM-dd HH:mm:ss"),
							data.message,
							data.sumDiamondNum,
							data.sumCount,
							data.type,
							data.leftZuanShi,
							data.leftCount,
							data.hongBaoStatus,
							data.hongBaoID
						});
					}
					ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
					if (ret >= 0 && data.hongBaoID == 0)
					{
						cmdText = "SELECT LAST_INSERT_ID();";
						ret = conn.GetSingleInt(cmdText, 0, new MySQLParameter[0]);
					}
				}
			}
			client.sendCmd<int>(nID, ret);
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x0003A934 File Offset: 0x00038B34
		private void RecvJieRiHongBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			JieriHongBaoKingItemData itemData = null;
			try
			{
				List<string> args = DataHelper.BytesToObject<List<string>>(cmdParams, 0, count);
				if (args != null)
				{
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("INSERT INTO `t_hongbao_jieri_recv` (`keystr`, `rid`, `count`, `getawardtimes`, `lasttime`, `rname`, `zuanshi`) VALUES('{0}',{1},{2},{3},'{4}','{5}','{6}') ON DUPLICATE KEY UPDATE `count`=`count`+{2},rname='{5}',`zuanshi`=`zuanshi`+{6},`lasttime`='{4}';", new object[]
						{
							args[0],
							args[1],
							args[2],
							0,
							args[3],
							args[4],
							args[5]
						});
						if (string.IsNullOrEmpty(args[3]) || conn.ExecuteSql(cmdText, new MySQLParameter[0]) >= 0)
						{
							cmdText = string.Format("SELECT `rid`,`count`,`getawardtimes`,`lasttime`,`rname` FROM `t_hongbao_jieri_recv` WHERE `keystr`='{0}' AND `rid`={1};", args[0], args[1]);
							using (MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]))
							{
								int ranking = 1;
								if (reader.Read())
								{
									itemData = new JieriHongBaoKingItemData
									{
										RoleID = Global.SafeConvertToInt32(reader[0].ToString(), 10),
										TotalRecv = Global.SafeConvertToInt32(reader[1].ToString(), 10),
										GetAwardTimes = Global.SafeConvertToInt32(reader[2].ToString(), 10),
										Rolename = reader[4].ToString(),
										Rank = ranking++
									};
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<JieriHongBaoKingItemData>(nID, itemData);
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x0003AB4C File Offset: 0x00038D4C
		private void GetJieRiHongBaoCount(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			try
			{
				List<string> args = DataHelper.BytesToObject<List<string>>(cmdParams, 0, count);
				if (args != null)
				{
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("SELECT `count` FROM `t_hongbao_jieri_recv` WHERE `keystr`='{0}' AND `rid`={1};", args[0], args[1]);
						ret = conn.GetSingleInt(cmdText, 0, new MySQLParameter[0]);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, ret);
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x0003ABF8 File Offset: 0x00038DF8
		private void GetJieRiHongBaoBangAwards(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			try
			{
				List<string> args = DataHelper.BytesToObject<List<string>>(cmdParams, 0, count);
				if (args != null)
				{
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("update t_hongbao_jieri_recv set getawardtimes={2} where `keystr`='{0}' and `rid`={1} and getawardtimes='0'", args[0], args[1], args[2]);
						ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				ret = -1;
			}
			client.sendCmd<int>(nID, ret);
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x0003ACAC File Offset: 0x00038EAC
		private void RecvZhanMengHongBao(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int ret = 0;
			try
			{
				HongBaoRecvData args = DataHelper.BytesToObject<HongBaoRecvData>(cmdParams, 0, count);
				if (args != null)
				{
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("replace into `t_hongbao_recv` (`hongbaoid`, `rid`, `bhid`, `zuanshi`, `recvtime`, `rname`) values('{0}','{1}','{2}','{3}','{4}','{5}');", new object[]
						{
							args.HongBaoID,
							args.RoleId,
							args.BhId,
							args.ZuanShi,
							args.RecvTime,
							args.RoleName
						});
						ret = conn.ExecuteSql(cmdText, new MySQLParameter[0]);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<int>(nID, ret);
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x0003ADA8 File Offset: 0x00038FA8
		private void GetZhanMengHongBaoList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			HongBaoListQueryData queryData = null;
			try
			{
				List<HongBaoSendData> list = new List<HongBaoSendData>();
				queryData = DataHelper.BytesToObject<HongBaoListQueryData>(cmdParams, 0, count);
				if (null == queryData)
				{
					queryData = new HongBaoListQueryData();
				}
				using (MyDbConnection3 conn = new MyDbConnection3(false))
				{
					string cmdText;
					if (queryData.BhId != 0)
					{
						cmdText = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE `state`=0 AND `bhid`={0};", queryData.BhId);
					}
					else
					{
						cmdText = string.Format("SELECT `id`,`bhid`,`senderid`,`sendername`,`sendtime`,`endtime`,`msg`,`zuanshi`,`count`,`type`,`leftzuanshi`,`leftcount`,`state` FROM `t_hongbao_send` WHERE `state`=0;", new object[0]);
					}
					MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
					while (reader.Read())
					{
						list.Add(new HongBaoSendData
						{
							hongBaoID = Global.SafeConvertToInt32(reader[0].ToString(), 10),
							bhid = Global.SafeConvertToInt32(reader[1].ToString(), 10),
							senderID = Global.SafeConvertToInt32(reader[2].ToString(), 10),
							sender = reader[3].ToString(),
							sendTime = Global.SafeConvertToDateTime(reader[4].ToString(), DateTime.MinValue),
							endTime = Global.SafeConvertToDateTime(reader[5].ToString(), DateTime.MinValue),
							message = reader[6].ToString(),
							sumDiamondNum = Global.SafeConvertToInt32(reader[7].ToString(), 10),
							sumCount = Global.SafeConvertToInt32(reader[8].ToString(), 10),
							type = Global.SafeConvertToInt32(reader[9].ToString(), 10),
							leftZuanShi = Global.SafeConvertToInt32(reader[10].ToString(), 10),
							leftCount = Global.SafeConvertToInt32(reader[11].ToString(), 10),
							hongBaoStatus = Global.SafeConvertToInt32(reader[12].ToString(), 10)
						});
					}
					reader.Close();
					foreach (HongBaoSendData hongbao in list)
					{
						hongbao.RecvList = new List<HongBaoRecvData>();
						cmdText = string.Format("SELECT `bhid`,`rid`,`rname`,`recvtime`,`zuanshi` FROM t_hongbao_recv WHERE hongbaoid={0};", hongbao.hongBaoID);
						MySQLDataReader mySQLDataReader;
						reader = (mySQLDataReader = conn.ExecuteReader(cmdText, new MySQLParameter[0]));
						try
						{
							while (reader.Read())
							{
								HongBaoRecvData recvData = new HongBaoRecvData();
								recvData.HongBaoID = Global.SafeConvertToInt32(reader[0].ToString(), 10);
								recvData.RoleId = Global.SafeConvertToInt32(reader[1].ToString(), 10);
								recvData.RoleName = reader[2].ToString();
								recvData.RecvTime = Global.SafeConvertToDateTime(reader[3].ToString(), DateTime.MinValue);
								recvData.ZuanShi = Global.SafeConvertToInt32(reader[4].ToString(), 10);
								hongbao.RecvList.Add(recvData);
							}
						}
						finally
						{
							if (mySQLDataReader != null)
							{
								((IDisposable)mySQLDataReader).Dispose();
							}
						}
					}
				}
				queryData.List = list;
				queryData.Success = 1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<HongBaoListQueryData>(nID, queryData);
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x0003B19C File Offset: 0x0003939C
		private void GetJieRiHongBaoList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			HongBaoListQueryData queryData = null;
			try
			{
				List<HongBaoSendData> list = new List<HongBaoSendData>();
				queryData = DataHelper.BytesToObject<HongBaoListQueryData>(cmdParams, 0, count);
				if (null != queryData)
				{
					queryData.List = list;
					using (MyDbConnection3 conn = new MyDbConnection3(false))
					{
						string cmdText = string.Format("SELECT `id`,`senderid`,`keystr`,`sendtime`,`endtime`,`msg`,`zuanshi`,`type`,`leftzuanshi`,`state` FROM `t_hongbao_jieri_send`  WHERE keystr='{0}';", queryData.KeyStr);
						MySQLDataReader reader = conn.ExecuteReader(cmdText, new MySQLParameter[0]);
						while (reader.Read())
						{
							list.Add(new HongBaoSendData
							{
								hongBaoID = Global.SafeConvertToInt32(reader[0].ToString(), 10),
								senderID = Global.SafeConvertToInt32(reader[1].ToString(), 10),
								sender = reader[2].ToString(),
								sendTime = Global.SafeConvertToDateTime(reader[3].ToString(), DateTime.MinValue),
								endTime = Global.SafeConvertToDateTime(reader[4].ToString(), DateTime.MinValue),
								message = reader[5].ToString(),
								sumDiamondNum = Global.SafeConvertToInt32(reader[6].ToString(), 10),
								type = Global.SafeConvertToInt32(reader[7].ToString(), 10),
								leftZuanShi = Global.SafeConvertToInt32(reader[8].ToString(), 10),
								hongBaoStatus = Global.SafeConvertToInt32(reader[9].ToString(), 10)
							});
						}
					}
					queryData.Success = 1;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<HongBaoListQueryData>(nID, queryData);
		}

		// Token: 0x04000895 RID: 2197
		private const int ALLY_LOG_COUNT_MAX = 20;

		// Token: 0x04000896 RID: 2198
		private const string timefm = "yyyy-MM-dd HH:mm:ss";

		// Token: 0x04000897 RID: 2199
		private object Mutex = new object();

		// Token: 0x04000898 RID: 2200
		private Thread WorkThread;
	}
}
