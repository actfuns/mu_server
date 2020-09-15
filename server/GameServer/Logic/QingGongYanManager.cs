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

namespace GameServer.Logic
{
	// Token: 0x02000543 RID: 1347
	public class QingGongYanManager
	{
		// Token: 0x060019A9 RID: 6569 RVA: 0x0018F4E0 File Offset: 0x0018D6E0
		public void LoadQingGongYanConfig()
		{
			lock (this._QingGongYanMutex)
			{
				this.QingGongYanDict.Clear();
				string fileName = "Config/GleeFeastAward.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null != xml)
				{
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						QingGongYanInfo InfoData = new QingGongYanInfo();
						InfoData.Index = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						InfoData.NpcID = (int)Global.GetSafeAttributeLong(xmlItem, "NPCID");
						InfoData.MapCode = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");
						InfoData.X = (int)Global.GetSafeAttributeLong(xmlItem, "X");
						InfoData.Y = (int)Global.GetSafeAttributeLong(xmlItem, "Y");
						InfoData.Direction = (int)Global.GetSafeAttributeLong(xmlItem, "Direction");
						string[] strBanTime = Global.GetSafeAttributeStr(xmlItem, "ProhibitedTime").Split(new char[]
						{
							'|'
						});
						for (int i = 0; i < strBanTime.Length; i++)
						{
							InfoData.ProhibitedTimeList.Add(strBanTime[i]);
						}
						InfoData.BeginTime = Global.GetSafeAttributeStr(xmlItem, "BeginTime");
						InfoData.OverTime = Global.GetSafeAttributeStr(xmlItem, "OverTime");
						InfoData.FunctionID = (int)Global.GetSafeAttributeLong(xmlItem, "FunctionID");
						InfoData.HoldBindJinBi = (int)Global.GetSafeAttributeLong(xmlItem, "ConductBindJinBi");
						InfoData.TotalNum = (int)Global.GetSafeAttributeLong(xmlItem, "SumNum");
						InfoData.SingleNum = (int)Global.GetSafeAttributeLong(xmlItem, "UseNum");
						InfoData.JoinBindJinBi = (int)Global.GetSafeAttributeLong(xmlItem, "BindJinBi");
						InfoData.ExpAward = (int)Global.GetSafeAttributeLong(xmlItem, "EXPAward");
						InfoData.XingHunAward = (int)Global.GetSafeAttributeLong(xmlItem, "XingHunAward");
						InfoData.ZhanGongAward = (int)Global.GetSafeAttributeLong(xmlItem, "ZhanGongAward");
						InfoData.ZuanShiCoe = (int)Global.GetSafeAttributeLong(xmlItem, "ZuanShiRatio");
						this.QingGongYanDict[InfoData.Index] = InfoData;
					}
				}
			}
		}

		// Token: 0x060019AA RID: 6570 RVA: 0x0018F780 File Offset: 0x0018D980
		private QingGongYanInfo GetQingGongYanConfig(int index)
		{
			QingGongYanInfo InfoData = null;
			lock (this._QingGongYanMutex)
			{
				if (this.QingGongYanDict.ContainsKey(index))
				{
					InfoData = this.QingGongYanDict[index];
				}
			}
			return InfoData;
		}

		// Token: 0x060019AB RID: 6571 RVA: 0x0018F7F0 File Offset: 0x0018D9F0
		public QingGongYanResult HoldQingGongYan(GameClient client, int index, int onlyCheck = 0)
		{
			QingGongYanResult result;
			if (!Global.IsKingCityLeader(client))
			{
				result = QingGongYanResult.NotKing;
			}
			else
			{
				QingGongYanInfo InfoData = this.GetQingGongYanConfig(index);
				if (null == InfoData)
				{
					result = QingGongYanResult.ErrorParam;
				}
				else if (InfoData.IfBanTime(TimeUtil.NowDateTime()))
				{
					result = QingGongYanResult.OutTime;
				}
				else
				{
					int DBStartDay = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_startday", 0);
					int currDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
					if (DBStartDay == currDay && TimeUtil.NowDateTime() <= DateTime.Parse(InfoData.OverTime))
					{
						result = QingGongYanResult.RepeatHold;
					}
					else
					{
						int startDay;
						if (TimeUtil.NowDateTime() < DateTime.Parse(InfoData.BeginTime))
						{
							startDay = currDay;
						}
						else
						{
							startDay = currDay + 1;
						}
						if (startDay == DBStartDay)
						{
							result = QingGongYanResult.RepeatHold;
						}
						else
						{
							if (InfoData.HoldBindJinBi > 0)
							{
								if (InfoData.HoldBindJinBi > Global.GetTotalBindTongQianAndTongQianVal(client))
								{
									return QingGongYanResult.MoneyNotEnough;
								}
							}
							if (onlyCheck > 0)
							{
								result = QingGongYanResult.CheckSuccess;
							}
							else
							{
								if (InfoData.HoldBindJinBi > 0)
								{
									if (!Global.SubBindTongQianAndTongQian(client, InfoData.HoldBindJinBi, "举办庆功宴"))
									{
										return QingGongYanResult.MoneyNotEnough;
									}
								}
								Global.UpdateDBGameConfigg("qinggongyan_roleid", client.ClientData.RoleID.ToString());
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_roleid", client.ClientData.RoleID.ToString());
								BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(client.ClientData.Faction, 0);
								if (null != bangHuiMiniData)
								{
									Global.UpdateDBGameConfigg("qinggongyan_guildname", bangHuiMiniData.BHName);
									GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_guildname", bangHuiMiniData.BHName);
								}
								else
								{
									Global.UpdateDBGameConfigg("qinggongyan_guildname", "");
									GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_guildname", "");
								}
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_guildname", client.ClientData.RoleName);
								Global.UpdateDBGameConfigg("qinggongyan_startday", startDay.ToString());
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_startday", startDay.ToString());
								Global.UpdateDBGameConfigg("qinggongyan_grade", index.ToString());
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_grade", index.ToString());
								Global.UpdateDBGameConfigg("qinggongyan_joincount", "0");
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joincount", "0");
								Global.UpdateDBGameConfigg("qinggongyan_joinmoney", "0");
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joinmoney", "0");
								Global.UpdateDBGameConfigg("qinggongyan_jubanmoney", InfoData.HoldBindJinBi.ToString());
								GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_jubanmoney", InfoData.HoldBindJinBi.ToString());
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "举办庆功宴", startDay.ToString(), "", client.ClientData.RoleName, "", index, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
								EventLogManager.AddRoleEvent(client, OpTypes.Hold, OpTags.QingGongYan, LogRecordType.OffsetDayId, new object[]
								{
									startDay
								});
								result = QingGongYanResult.Success;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060019AC RID: 6572 RVA: 0x0018FB6C File Offset: 0x0018DD6C
		private bool IfNeedOpenQingGongYan()
		{
			QingGongYanInfo InfoData = this.GetInfoData();
			bool result;
			if (null == InfoData)
			{
				result = false;
			}
			else
			{
				int DBStartDay = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_startday", 0);
				int currDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				result = (DBStartDay == currDay && !(TimeUtil.NowDateTime() < DateTime.Parse(InfoData.BeginTime)) && !(TimeUtil.NowDateTime() > DateTime.Parse(InfoData.OverTime)) && !this.QingGongYanOpenFlag);
			}
			return result;
		}

		// Token: 0x060019AD RID: 6573 RVA: 0x0018FC18 File Offset: 0x0018DE18
		private bool IfNeedCloseQingGongYan()
		{
			QingGongYanInfo InfoData = this.GetInfoData();
			bool result;
			if (null == InfoData)
			{
				result = false;
			}
			else
			{
				int DBStartDay = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_startday", 0);
				int currDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				result = (DBStartDay == currDay && !(TimeUtil.NowDateTime() <= DateTime.Parse(InfoData.OverTime)) && this.QingGongYanOpenFlag);
			}
			return result;
		}

		// Token: 0x060019AE RID: 6574 RVA: 0x0018FCA4 File Offset: 0x0018DEA4
		public void CheckQingGongYan(long ticks)
		{
			if (ticks - this.lastProcessTicks >= 10000L)
			{
				this.lastProcessTicks = ticks;
				if (this.IfNeedOpenQingGongYan())
				{
					this.OpenQingGongYan();
				}
				if (this.IfNeedCloseQingGongYan())
				{
					this.CloseQingGongYan();
				}
			}
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x0018FD00 File Offset: 0x0018DF00
		private void OpenQingGongYan()
		{
			this.QingGongYanOpenFlag = true;
			QingGongYanInfo InfoData = this.GetInfoData();
			if (null != InfoData)
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[InfoData.MapCode];
				NPC npc = NPCGeneralManager.GetNPCFromConfig(InfoData.MapCode, InfoData.NpcID, InfoData.X, InfoData.Y, InfoData.Direction);
				if (null != npc)
				{
					if (NPCGeneralManager.AddNpcToMap(npc))
					{
						GameManager.ClientMgr.BroadcastServerCmd(733, "1", false);
						this.QingGongYanNpc = npc;
						string guildName = GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_guildname", "");
						string broadCastMsg = StringUtil.substitute(GLang.GetLang(524, new object[0]), new object[]
						{
							guildName
						});
						Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, broadCastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.SysHintAndChatBox, 0, 0, 100, 100);
					}
					else
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("OpenQingGongYan, AddNpcToMap Faild !InfoData.MapCode={0}, InfoData.NpcID={1}", InfoData.MapCode, InfoData.NpcID), null, true);
					}
				}
			}
		}

		// Token: 0x060019B0 RID: 6576 RVA: 0x0018FE24 File Offset: 0x0018E024
		private QingGongYanInfo GetInfoData()
		{
			int DBGrade = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_grade", 0);
			QingGongYanInfo result;
			if (DBGrade <= 0)
			{
				result = null;
			}
			else
			{
				result = this.GetQingGongYanConfig(DBGrade);
			}
			return result;
		}

		// Token: 0x060019B1 RID: 6577 RVA: 0x0018FE5C File Offset: 0x0018E05C
		private void CloseQingGongYan()
		{
			if (null != this.QingGongYanNpc)
			{
				NPCGeneralManager.RemoveMapNpc(this.QingGongYanNpc.MapCode, this.QingGongYanNpc.NpcID);
				this.QingGongYanNpc = null;
				GameManager.ClientMgr.BroadcastServerCmd(733, "0", false);
			}
			this.QingGongYanOpenFlag = false;
			QingGongYanInfo InfoData = this.GetInfoData();
			if (null != InfoData)
			{
				if (InfoData.ZuanShiCoe > 0)
				{
					int JoinMoney = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_joinmoney", 0);
					int ZuanShiAward = JoinMoney / InfoData.ZuanShiCoe;
					int DBRoleID = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_roleid", 0);
					if (DBRoleID > 0)
					{
						string sContent = string.Format(GLang.GetLang(525, new object[0]), new object[]
						{
							TimeUtil.NowDateTime().Year,
							TimeUtil.NowDateTime().Month,
							TimeUtil.NowDateTime().Day,
							DateTime.Parse(InfoData.BeginTime).Hour,
							DateTime.Parse(InfoData.BeginTime).Minute,
							ZuanShiAward
						});
						Global.UseMailGivePlayerAward3(DBRoleID, null, GLang.GetLang(526, new object[0]), sContent, ZuanShiAward, 0, 0);
						Global.UpdateDBGameConfigg("qinggongyan_roleid", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_roleid", "");
						Global.UpdateDBGameConfigg("qinggongyan_guildname", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_guildname", "");
						Global.UpdateDBGameConfigg("qinggongyan_startday", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_startday", "");
						Global.UpdateDBGameConfigg("qinggongyan_grade", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_grade", "");
						Global.UpdateDBGameConfigg("qinggongyan_joincount", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joincount", "");
						Global.UpdateDBGameConfigg("qinggongyan_joinmoney", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joinmoney", "");
						Global.UpdateDBGameConfigg("qinggongyan_jubanmoney", "");
						GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_jubanmoney", "");
						string broadCastMsg = StringUtil.substitute(GLang.GetLang(527, new object[0]), new object[0]);
						Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, broadCastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.SysHintAndChatBox, 0, 0, 100, 100);
					}
				}
			}
		}

		// Token: 0x060019B2 RID: 6578 RVA: 0x00190120 File Offset: 0x0018E320
		public QingGongYanResult JoinQingGongYan(GameClient client)
		{
			QingGongYanResult result;
			if (null == this.QingGongYanNpc)
			{
				result = QingGongYanResult.OutTime;
			}
			else
			{
				QingGongYanInfo InfoData = this.GetInfoData();
				if (null == InfoData)
				{
					result = QingGongYanResult.OutTime;
				}
				else
				{
					int JoinCount = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_joincount", 0);
					if (JoinCount > 0)
					{
						if (JoinCount >= InfoData.TotalNum)
						{
							return QingGongYanResult.TotalNotEnough;
						}
					}
					if (InfoData.JoinBindJinBi > 0)
					{
						if (InfoData.JoinBindJinBi > Global.GetTotalBindTongQianAndTongQianVal(client))
						{
							return QingGongYanResult.MoneyNotEnough;
						}
					}
					string QingGongYanJoinFlag = Global.GetRoleParamByName(client, "QingGongYanJoinFlag");
					int currDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
					int lastJoinDay = 0;
					int joinCount = 0;
					if (null != QingGongYanJoinFlag)
					{
						string[] fields = QingGongYanJoinFlag.Split(new char[]
						{
							','
						});
						if (2 == fields.Length)
						{
							lastJoinDay = Convert.ToInt32(fields[0]);
							joinCount = Convert.ToInt32(fields[1]);
						}
					}
					if (currDay != lastJoinDay)
					{
						joinCount = 0;
					}
					if (InfoData.SingleNum > 0)
					{
						if (joinCount >= InfoData.SingleNum)
						{
							return QingGongYanResult.CountNotEnough;
						}
					}
					if (InfoData.JoinBindJinBi > 0)
					{
						if (!Global.SubBindTongQianAndTongQian(client, InfoData.JoinBindJinBi, "参加庆功宴"))
						{
							return QingGongYanResult.MoneyNotEnough;
						}
					}
					string roleParam = currDay.ToString() + "," + (joinCount + 1).ToString();
					Global.UpdateRoleParamByName(client, "QingGongYanJoinFlag", roleParam, true);
					Global.UpdateDBGameConfigg("qinggongyan_joincount", (JoinCount + 1).ToString());
					GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joincount", (JoinCount + 1).ToString());
					int JoinMoney = GameManager.GameConfigMgr.GetGameConfigItemInt("qinggongyan_joinmoney", 0);
					Global.UpdateDBGameConfigg("qinggongyan_joinmoney", (JoinMoney + InfoData.JoinBindJinBi).ToString());
					GameManager.GameConfigMgr.SetGameConfigItem("qinggongyan_joinmoney", (JoinMoney + InfoData.JoinBindJinBi).ToString());
					if (InfoData.ExpAward > 0)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, (long)InfoData.ExpAward, true, true, false, "none");
					}
					if (InfoData.XingHunAward > 0)
					{
						GameManager.ClientMgr.ModifyStarSoulValue(client, InfoData.XingHunAward, "庆功宴", true, true);
					}
					if (InfoData.ZhanGongAward > 0)
					{
						int nZhanGong = InfoData.ZhanGongAward;
						if (GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref nZhanGong, AddBangGongTypes.BG_QGY, 0))
						{
							if (0 != nZhanGong)
							{
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战功", "罗兰宴会领取", "系统", client.ClientData.RoleName, "增加", nZhanGong, client.ClientData.ZoneID, client.strUserID, client.ClientData.BangGong, client.ServerId, null);
							}
						}
						GameManager.SystemServerEvents.AddEvent(string.Format("角色获取帮贡, roleID={0}({1}), BangGong={2}, newBangGong={3}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.RoleName,
							client.ClientData.BangGong,
							nZhanGong
						}), EventLevels.Record);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "参加庆功宴", "", "", client.ClientData.RoleName, "", 1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
					EventLogManager.AddRoleEvent(client, OpTypes.Join, OpTags.QingGongYan, LogRecordType.OffsetDayId, new object[]
					{
						currDay
					});
					result = QingGongYanResult.Success;
				}
			}
			return result;
		}

		// Token: 0x060019B3 RID: 6579 RVA: 0x00190540 File Offset: 0x0018E740
		public TCPProcessCmdResults ProcessHoldQingGongYanCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int Index = Convert.ToInt32(fields[1]);
				int OnlyCheck = Convert.ToInt32(fields[2]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				QingGongYanResult result = this.HoldQingGongYan(client, Index, OnlyCheck);
				string strcmd;
				if (result != QingGongYanResult.Success)
				{
					strcmd = string.Format("{0}:{1}:{2}", (int)result, roleID, OnlyCheck);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}:{1}:{2}", 0, roleID, OnlyCheck);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessHoldQingGongYanCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x060019B4 RID: 6580 RVA: 0x00190734 File Offset: 0x0018E934
		public TCPProcessCmdResults ProcessQueryQingGongYanCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 1)
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
				int DBGrade = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_grade", "0"));
				int TotalCount = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_joincount", "0"));
				int JoinMoney = Convert.ToInt32(GameManager.GameConfigMgr.GetGameConfigItemStr("qinggongyan_joinmoney", "0"));
				string QingGongYanJoinFlag = Global.GetRoleParamByName(client, "QingGongYanJoinFlag");
				int currDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				int lastJoinDay = 0;
				int joinCount = 0;
				if (null != QingGongYanJoinFlag)
				{
					string[] strTemp = QingGongYanJoinFlag.Split(new char[]
					{
						','
					});
					if (2 == strTemp.Length)
					{
						lastJoinDay = Convert.ToInt32(strTemp[0]);
						joinCount = Convert.ToInt32(strTemp[1]);
					}
				}
				if (currDay != lastJoinDay)
				{
					joinCount = 0;
				}
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					roleID,
					DBGrade,
					joinCount,
					TotalCount,
					JoinMoney
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessHoldQingGongYanCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x001909C4 File Offset: 0x0018EBC4
		public TCPProcessCmdResults ProcessJoinQingGongYanCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 1)
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
				QingGongYanResult result = this.JoinQingGongYan(client);
				string strcmd;
				if (result != QingGongYanResult.Success)
				{
					strcmd = string.Format("{0}:{1}", (int)result, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}:{1}", 0, roleID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessJoinQingGongYanCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x00190B90 File Offset: 0x0018ED90
		public TCPProcessCmdResults ProcessCMD_SPR_QueryQingGongYanOpenCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				string cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				int result = (this.QingGongYanNpc == null) ? 0 : 1;
				string strcmd = string.Format("{0}", result);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessJoinQingGongYanCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x04002418 RID: 9240
		private object _QingGongYanMutex = new object();

		// Token: 0x04002419 RID: 9241
		private Dictionary<int, QingGongYanInfo> QingGongYanDict = new Dictionary<int, QingGongYanInfo>();

		// Token: 0x0400241A RID: 9242
		private bool QingGongYanOpenFlag = false;

		// Token: 0x0400241B RID: 9243
		private NPC QingGongYanNpc = null;

		// Token: 0x0400241C RID: 9244
		private long lastProcessTicks = 0L;
	}
}
