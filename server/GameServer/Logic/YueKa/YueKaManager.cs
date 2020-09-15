using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic.YueKa
{
	// Token: 0x02000823 RID: 2083
	public class YueKaManager
	{
		// Token: 0x06003AE6 RID: 15078 RVA: 0x0031FA10 File Offset: 0x0031DC10
		public static void LoadConfig()
		{
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(YueKaManager.YUE_KA_GOODS_FILE));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(YueKaManager.YUE_KA_GOODS_FILE));
			if (xml == null)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件不存在", YueKaManager.YUE_KA_GOODS_FILE), null, true);
			}
			else
			{
				try
				{
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							YueKaAward award = new YueKaAward();
							award.Init(xmlItem);
							YueKaManager.AllGoodsDict[award.Day] = award;
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时异常{1}", YueKaManager.YUE_KA_GOODS_FILE, ex), null, true);
				}
			}
		}

		// Token: 0x06003AE7 RID: 15079 RVA: 0x0031FB1C File Offset: 0x0031DD1C
		public static void HandleUserBuyYueKa(string userID, int roleID)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
			{
				GameClient client = GameManager.ClientMgr.FindClient(roleID);
				LogManager.WriteLog(LogTypes.Error, string.Format("HandleUserBuyYueKa, userid={0}, roleid={1}", userID, roleID), null, true);
				if (null != client)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("HandleUserBuyYueKa, 玩家在线, 在线的userid={0},  roleid={1}", userID, client.ClientData.RoleID), null, true);
					Global.ProcessVipLevelUp(client);
					lock (client.ClientData.YKDetail)
					{
						if (client.ClientData.YKDetail.HasYueKa == 0)
						{
							DateTime nowDate = TimeUtil.NowDateTime();
							client.ClientData.YKDetail.HasYueKa = 1;
							client.ClientData.YKDetail.BegOffsetDay = Global.GetOffsetDay(nowDate);
							client.ClientData.YKDetail.EndOffsetDay = client.ClientData.YKDetail.BegOffsetDay + YueKaManager.DAYS_PER_YUE_KA;
							client.ClientData.YKDetail.CurOffsetDay = Global.GetOffsetDay(nowDate);
							client.ClientData.YKDetail.AwardInfo = "0";
						}
						else
						{
							client.ClientData.YKDetail.EndOffsetDay += YueKaManager.DAYS_PER_YUE_KA;
						}
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.YueKaRemainDay, client.ClientData.YKDetail.RemainDayOfYueKa());
						YueKaManager._UpdateYKDetail2DB(client, client.ClientData.YKDetail);
						if (client._IconStateMgr.CheckFuLiYueKaFanLi(client))
						{
							client._IconStateMgr.SendIconStateToClient(client);
						}
					}
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("玩家购买了月卡，但是处理的时候找不到在线角色, UserID={0}, last roldid={1}, 转交给db处理", userID, roleID), null, true);
					int beginOffsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
					string strcmd = string.Format("{0}:{1}:{2}", roleID, beginOffsetDay, "YueKaInfo");
					string[] dbFields = Global.ExecuteDBCmd(10181, strcmd, 0);
					if (dbFields == null || dbFields.Length != 1 || dbFields[0] != "0")
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("玩家购买了月卡，但是处理的时候找不到在线角色, UserID={0}, last roldid={1}, 转交给db处理时失败了", userID, roleID), null, true);
					}
				}
			}
		}

		// Token: 0x06003AE8 RID: 15080 RVA: 0x0031FD98 File Offset: 0x0031DF98
		public static TCPProcessCmdResults ProcessGetYueKaData(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (1 != fields.Length)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				YueKaData ykData = null;
				lock (client.ClientData.YKDetail)
				{
					ykData = client.ClientData.YKDetail.ToYueKaData();
				}
				GameManager.ClientMgr.SendToClient(client, DataHelper.ObjectToBytes<YueKaData>(ykData), nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetYueKaData", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06003AE9 RID: 15081 RVA: 0x0031FF6C File Offset: 0x0031E16C
		public static TCPProcessCmdResults ProcessGetYueKaAward(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (2 != fields.Length)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int day = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				YueKaError err = YueKaManager._GetYueKaAward(client, day);
				string cmd = string.Format("{0}:{1}:{2}", roleID, (int)err, day);
				GameManager.ClientMgr.SendToClient(client, cmd, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetYueKaData", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06003AEA RID: 15082 RVA: 0x003200F8 File Offset: 0x0031E2F8
		private static YueKaError _GetYueKaAward(GameClient client, int day)
		{
			YueKaError result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
			{
				result = YueKaError.YK_CannotAward_HasNotYueKa;
			}
			else if (day <= 0 || day > YueKaManager.DAYS_PER_YUE_KA)
			{
				result = YueKaError.YK_CannotAward_ParamInvalid;
			}
			else
			{
				lock (client.ClientData.YKDetail)
				{
					if (client.ClientData.YKDetail.HasYueKa == 0)
					{
						return YueKaError.YK_CannotAward_HasNotYueKa;
					}
					if (day < client.ClientData.YKDetail.CurDayOfPerYueKa())
					{
						return YueKaError.YK_CannotAward_DayHasPassed;
					}
					if (day > client.ClientData.YKDetail.CurDayOfPerYueKa())
					{
						return YueKaError.YK_CannotAward_TimeNotReach;
					}
					string awardInfo = client.ClientData.YKDetail.AwardInfo;
					if (awardInfo.Length < day || awardInfo[day - 1] == '1')
					{
						return YueKaError.YK_CannotAward_AlreadyAward;
					}
					YueKaAward awardData = null;
					YueKaManager.AllGoodsDict.TryGetValue(day, out awardData);
					if (awardData == null)
					{
						return YueKaError.YK_CannotAward_ConfigError;
					}
					List<GoodsData> goodsDataList = awardData.GetGoodsByOcc(Global.CalcOriginalOccupationID(client));
					if (goodsDataList != null && goodsDataList.Count > 0)
					{
						if (!Global.CanAddGoodsNum(client, goodsDataList.Count))
						{
							return YueKaError.YK_CannotAward_BagNotEnough;
						}
						foreach (GoodsData goodsData in goodsDataList)
						{
							goodsData.Id = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, string.Format("第{0}天月卡返利", awardData.Day), false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
						}
						client.ClientData.AddAwardRecord(RoleAwardMsg.YueKaoAward, goodsDataList, false);
					}
					GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, awardData.BindZuanShi, string.Format("第{0}天月卡返利", awardData.Day));
					client.ClientData.AddAwardRecord(RoleAwardMsg.YueKaoAward, MoneyTypes.BindYuanBao, awardData.BindZuanShi);
					GameManager.ClientMgr.NotifyGetAwardMsg(client, RoleAwardMsg.YueKaoAward, "");
					client.ClientData.YKDetail.AwardInfo = awardInfo.Substring(0, day - 1) + "1";
					YueKaManager._UpdateYKDetail2DB(client, client.ClientData.YKDetail);
					if (client._IconStateMgr.CheckFuLiYueKaFanLi(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
				result = YueKaError.YK_Success;
			}
			return result;
		}

		// Token: 0x06003AEB RID: 15083 RVA: 0x00320474 File Offset: 0x0031E674
		private static void _SendAward2Player(GameClient client, YueKaAward award)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
			{
				List<GoodsData> goodsDataList = award.GetGoodsByOcc(Global.CalcOriginalOccupationID(client));
				if (!Global.CanAddGoodsNum(client, goodsDataList.Count))
				{
					foreach (GoodsData item in goodsDataList)
					{
						Global.UseMailGivePlayerAward(client, item, GLang.GetLang(576, new object[0]), string.Format(GLang.GetLang(577, new object[0]), award.Day), 1.0);
					}
				}
				else
				{
					foreach (GoodsData goodsData in goodsDataList)
					{
						goodsData.Id = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, string.Format("第{0}天月卡返利", award.Day), false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
					}
				}
				GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, award.BindZuanShi, string.Format("第{0}天月卡返利", award.Day));
			}
		}

		// Token: 0x06003AEC RID: 15084 RVA: 0x0032065C File Offset: 0x0031E85C
		public static void CheckValid(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
			{
				if (client != null)
				{
					lock (client.ClientData.YKDetail)
					{
						if (client.ClientData.YKDetail.HasYueKa != 0)
						{
							int todayOffset = Global.GetOffsetDay(TimeUtil.NowDateTime());
							if (todayOffset >= client.ClientData.YKDetail.EndOffsetDay)
							{
								client.ClientData.YKDetail.HasYueKa = 0;
							}
							else
							{
								int curBegOffsetDay = client.ClientData.YKDetail.CurOffsetDay - client.ClientData.YKDetail.AwardInfo.Length + 1;
								if (todayOffset >= curBegOffsetDay + YueKaManager.DAYS_PER_YUE_KA)
								{
									client.ClientData.YKDetail.CurOffsetDay = todayOffset;
									client.ClientData.YKDetail.AwardInfo = "";
									for (int i = curBegOffsetDay + YueKaManager.DAYS_PER_YUE_KA; i <= todayOffset; i++)
									{
										YueKaDetail ykdetail2 = client.ClientData.YKDetail;
										ykdetail2.AwardInfo += "0";
									}
								}
								else
								{
									for (int i = client.ClientData.YKDetail.CurOffsetDay + 1; i <= todayOffset; i++)
									{
										YueKaDetail ykdetail3 = client.ClientData.YKDetail;
										ykdetail3.AwardInfo += "0";
									}
									client.ClientData.YKDetail.CurOffsetDay = todayOffset;
								}
							}
							YueKaManager._UpdateYKDetail2DB(client, client.ClientData.YKDetail);
						}
					}
				}
			}
		}

		// Token: 0x06003AED RID: 15085 RVA: 0x00320854 File Offset: 0x0031EA54
		private static void _UpdateYKDetail2DB(GameClient client, YueKaDetail YKDetail)
		{
			string value = client.ClientData.YKDetail.SerializeToString();
			Global.SaveRoleParamsStringToDB(client, "YueKaInfo", value, true);
		}

		// Token: 0x06003AEE RID: 15086 RVA: 0x00320884 File Offset: 0x0031EA84
		public static void UpdateNewDay(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot4Dot1))
			{
				if (client != null)
				{
					YueKaManager.CheckValid(client);
					lock (client.ClientData.YKDetail)
					{
						if (client._IconStateMgr.CheckFuLiYueKaFanLi(client))
						{
							client._IconStateMgr.SendIconStateToClient(client);
						}
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.YueKaRemainDay, client.ClientData.YKDetail.RemainDayOfYueKa());
					}
				}
			}
		}

		// Token: 0x04004500 RID: 17664
		public static int DAYS_PER_YUE_KA = 30;

		// Token: 0x04004501 RID: 17665
		public static readonly int YUE_KA_MONEY_ID_IN_CHARGE_FILE = 10000;

		// Token: 0x04004502 RID: 17666
		private static readonly string YUE_KA_GOODS_FILE = "Config/Activity/Card.xml";

		// Token: 0x04004503 RID: 17667
		private static Dictionary<int, YueKaAward> AllGoodsDict = new Dictionary<int, YueKaAward>();
	}
}
