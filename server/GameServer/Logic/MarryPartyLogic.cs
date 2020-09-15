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
	// Token: 0x02000531 RID: 1329
	public class MarryPartyLogic
	{
		// Token: 0x06001945 RID: 6469 RVA: 0x00189FB4 File Offset: 0x001881B4
		public static MarryPartyLogic getInstance()
		{
			return MarryPartyLogic.Instance;
		}

		// Token: 0x06001946 RID: 6470 RVA: 0x00189FCC File Offset: 0x001881CC
		public void LoadMarryPartyConfig()
		{
			lock (this.MarryPartyConfigList)
			{
				this.MarryPartyConfigList.Clear();
				string fileName = "Config/WeddingFeasttAward.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					MarryPartyConfigData data = new MarryPartyConfigData();
					data.PartyID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					data.PartyType = (int)Global.GetSafeAttributeLong(xmlItem, "Type");
					data.PartyCost = (int)Global.GetSafeAttributeLong(xmlItem, "ConductBindJinBi");
					data.PartyMaxJoinCount = (int)Global.GetSafeAttributeLong(xmlItem, "SumNum");
					data.PlayerMaxJoinCount = (int)Global.GetSafeAttributeLong(xmlItem, "UseNum");
					data.JoinCost = (int)Global.GetSafeAttributeLong(xmlItem, "BindJinBi");
					data.RewardExp = (int)Global.GetSafeAttributeLong(xmlItem, "EXPAward");
					data.RewardXingHun = (int)Global.GetSafeAttributeLong(xmlItem, "XingHunAward");
					data.RewardShengWang = (int)Global.GetSafeAttributeLong(xmlItem, "ShengWangAward");
					data.GoodWillRatio = (int)Global.GetSafeAttributeLong(xmlItem, "GoodWillRatio");
					string strGoodsAward = Global.GetSafeAttributeStr(xmlItem, "GoodsAward");
					string[] fields = strGoodsAward.Split(new char[]
					{
						','
					});
					if (fields.Length == 7)
					{
						data.RewardItem = new AwardsItemData
						{
							Occupation = 0,
							RoleSex = 0,
							GoodsID = Convert.ToInt32(fields[0]),
							GoodsNum = Convert.ToInt32(fields[1]),
							Binding = Convert.ToInt32(fields[2]),
							Level = Convert.ToInt32(fields[3]),
							AppendLev = Convert.ToInt32(fields[4]),
							IsHaveLuckyProp = Convert.ToInt32(fields[5]),
							ExcellencePorpValue = Convert.ToInt32(fields[6]),
							EndTime = "1900-01-01 12:00:00"
						};
					}
					this.MarryPartyConfigList.Add(data.PartyType, data);
				}
			}
			string npcDataString = GameManager.systemParamsList.GetParamValueByName("HunYanNPC");
			string[] npcAttrString = npcDataString.Split(new char[]
			{
				','
			});
			if (npcAttrString.Length >= 5)
			{
				int.TryParse(npcAttrString[0], out this.MarryPartyNPCConfig.MapCode);
				int.TryParse(npcAttrString[1], out this.MarryPartyNPCConfig.NpcID);
				int.TryParse(npcAttrString[2], out this.MarryPartyNPCConfig.NpcX);
				int.TryParse(npcAttrString[3], out this.MarryPartyNPCConfig.NpcY);
				int.TryParse(npcAttrString[4], out this.MarryPartyNPCConfig.NpcDir);
			}
			this.MarryPartyPlayerMaxJoinCount = (int)GameManager.systemParamsList.GetParamValueIntByName("HunYanUseMaxNum", -1);
			this.MarryPartyJoinListResetTime = TimeUtil.NowDateTime().DayOfYear;
			this.MarryPartyQueryList();
		}

		// Token: 0x06001947 RID: 6471 RVA: 0x0018A344 File Offset: 0x00188544
		private MarryPartyConfigData GetMarryPartyConfigData(int type)
		{
			MarryPartyConfigData data = null;
			lock (this.MarryPartyConfigList)
			{
				this.MarryPartyConfigList.TryGetValue(type, out data);
			}
			return data;
		}

		// Token: 0x06001948 RID: 6472 RVA: 0x0018A3A4 File Offset: 0x001885A4
		public bool MarryPartyQueryList()
		{
			byte[] byteData = null;
			bool result;
			if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10187, string.Format("{0}", GameManager.ServerLineID), out byteData, 0))
			{
				this.m_MarryPartyDataCache.MarryPartyList = new Dictionary<int, MarryPartyData>();
				result = false;
			}
			else if (byteData == null || byteData.Length <= 6)
			{
				this.m_MarryPartyDataCache.MarryPartyList = new Dictionary<int, MarryPartyData>();
				result = false;
			}
			else
			{
				int length = BitConverter.ToInt32(byteData, 0);
				this.m_MarryPartyDataCache.MarryPartyList = DataHelper.BytesToObject<Dictionary<int, MarryPartyData>>(byteData, 6, length - 2);
				result = true;
			}
			return result;
		}

		// Token: 0x06001949 RID: 6473 RVA: 0x0018A454 File Offset: 0x00188654
		public bool MarryPartyJoinListClear(GameClient client, bool clearAll)
		{
			bool result;
			if (null == client.ClientData.MyMarryPartyJoinList)
			{
				result = false;
			}
			else
			{
				client.ClientData.MyMarryPartyJoinList.Clear();
				int writeDB = clearAll ? 2 : 1;
				if (clearAll)
				{
					int dayID = TimeUtil.NowDateTime().DayOfYear;
					if (this.MarryPartyJoinListResetTime == dayID)
					{
						writeDB = 0;
					}
					else
					{
						this.MarryPartyJoinListResetTime = dayID;
					}
				}
				byte[] byteData = null;
				if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10191, string.Format("{0}:{1}", client.ClientData.RoleID, writeDB), out byteData, client.ServerId))
				{
					result = false;
				}
				else if (byteData == null || byteData.Length <= 6)
				{
					result = false;
				}
				else
				{
					this.SendMarryPartyJoinList(client);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600194A RID: 6474 RVA: 0x0018A554 File Offset: 0x00188754
		public MarryPartyResult MarryPartyCreate(GameClient client, int partyType, long startTime)
		{
			MarryPartyResult result2;
			if (!MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result2 = MarryPartyResult.NotOpen;
			}
			else
			{
				MarryPartyConfigData configData = this.GetMarryPartyConfigData(partyType);
				if (null == configData)
				{
					result2 = MarryPartyResult.InvalidParam;
				}
				else
				{
					MarriageData marryData = client.ClientData.MyMarriageData;
					if (marryData.nSpouseID < 0 || marryData.byMarrytype < 0)
					{
						result2 = MarryPartyResult.NotMarry;
					}
					else
					{
						int husbandRoleID;
						string husbandName;
						int wifeRoleID;
						string wifeName;
						if (1 == marryData.byMarrytype)
						{
							husbandRoleID = client.ClientData.RoleID;
							husbandName = client.ClientData.RoleName;
							wifeRoleID = marryData.nSpouseID;
							wifeName = MarryLogic.GetRoleName(marryData.nSpouseID);
						}
						else
						{
							husbandRoleID = marryData.nSpouseID;
							husbandName = MarryLogic.GetRoleName(marryData.nSpouseID);
							wifeRoleID = client.ClientData.RoleID;
							wifeName = client.ClientData.RoleName;
						}
						MarryPartyData partyData = this.m_MarryPartyDataCache.AddParty(client.ClientData.RoleID, partyType, startTime, husbandRoleID, wifeRoleID, husbandName, wifeName);
						if (partyData == null)
						{
							result2 = MarryPartyResult.AlreadyRequest;
						}
						else
						{
							MarryPartyResult result = MarryPartyResult.Success;
							byte[] byteData = null;
							TCPProcessCmdResults dbResult = Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10188, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
							{
								client.ClientData.RoleID,
								partyType,
								startTime,
								husbandRoleID,
								wifeRoleID,
								husbandName,
								wifeName
							}), out byteData, client.ServerId);
							if (TCPProcessCmdResults.RESULT_FAILED == dbResult || byteData == null || byteData.Length <= 6)
							{
								result = MarryPartyResult.AlreadyRequest;
							}
							if (result == MarryPartyResult.Success)
							{
								if (configData.PartyCost > Global.GetTotalBindTongQianAndTongQianVal(client))
								{
									result = MarryPartyResult.NotEnoughMoney;
								}
								if (configData.PartyCost > 0)
								{
									if (!Global.SubBindTongQianAndTongQian(client, configData.PartyCost, "举办婚宴"))
									{
										result = MarryPartyResult.NotEnoughMoney;
									}
								}
							}
							if (result != MarryPartyResult.Success)
							{
								if (dbResult != TCPProcessCmdResults.RESULT_FAILED)
								{
									try
									{
										Global.SendAndRecvData<string>(10189, string.Format("{0}", client.ClientData.RoleID), client.ServerId, 0);
									}
									catch (Exception)
									{
									}
								}
								this.m_MarryPartyDataCache.RemoveParty(client.ClientData.RoleID);
								result2 = result;
							}
							else
							{
								int length = BitConverter.ToInt32(byteData, 0);
								MarryPartyData dbPartyData = DataHelper.BytesToObject<MarryPartyData>(byteData, 6, length - 2);
								this.m_MarryPartyDataCache.SetPartyTime(partyData, dbPartyData.StartTime);
								this.SendMarryPartyList(client, partyData, -1);
								result2 = result;
							}
						}
					}
				}
			}
			return result2;
		}

		// Token: 0x0600194B RID: 6475 RVA: 0x0018A844 File Offset: 0x00188A44
		public MarryPartyResult MarryPartyCancel(GameClient client)
		{
			MarryPartyResult result;
			if (!MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = MarryPartyResult.NotOpen;
			}
			else
			{
				MarryPartyData partyData = this.m_MarryPartyDataCache.GetParty(client.ClientData.MyMarriageData.nSpouseID);
				if (partyData != null)
				{
					result = MarryPartyResult.NotOriginator;
				}
				else
				{
					result = this.MarryPartyRemove(client.ClientData.RoleID, false, client);
				}
			}
			return result;
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x0018A8A0 File Offset: 0x00188AA0
		public MarryPartyResult MarryPartyRemove(int roleID, bool forceRemove, GameClient client)
		{
			MarryPartyData partyData = this.m_MarryPartyDataCache.GetParty(roleID);
			MarryPartyResult result;
			if (partyData == null)
			{
				result = MarryPartyResult.PartyNotFound;
			}
			else if (!forceRemove && partyData.StartTime <= TimeUtil.NOW())
			{
				result = MarryPartyResult.AlreadyStart;
			}
			else
			{
				MarryPartyConfigData configData = this.GetMarryPartyConfigData(partyData.PartyType);
				if (null == configData)
				{
					result = MarryPartyResult.InvalidParam;
				}
				else if (!this.MarryPartyRemoveInternal(roleID, forceRemove, client, partyData))
				{
					result = MarryPartyResult.PartyNotFound;
				}
				else
				{
					if (!forceRemove)
					{
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, configData.PartyCost, "婚宴退款", false);
						this.SendMarryPartyList(client, new MarryPartyData(), -1);
					}
					else if (partyData.StartTime > TimeUtil.NOW())
					{
						GoodsData goodData = Global.GetNewGoodsData(50014, 1);
						goodData.GCount = configData.PartyCost / 10000;
						Global.UseMailGivePlayerAward3(roleID, new List<GoodsData>
						{
							goodData
						}, GLang.GetLang(491, new object[0]), GLang.GetLang(492, new object[0]), 0, 0, 0);
					}
					result = MarryPartyResult.Success;
				}
			}
			return result;
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x0018A9F8 File Offset: 0x00188BF8
		private bool MarryPartyRemoveInternal(int roleID, bool forceRemove, GameClient self, MarryPartyData partyData = null)
		{
			bool result;
			if (!this.m_MarryPartyDataCache.RemoveParty(roleID))
			{
				result = false;
			}
			else
			{
				byte[] byteData = null;
				TCPProcessCmdResults dbResult = Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10189, string.Format("{0}", roleID), out byteData, self.ServerId);
				if (dbResult == TCPProcessCmdResults.RESULT_FAILED)
				{
					if (!forceRemove)
					{
						this.m_MarryPartyDataCache.RemovePartyCancel(partyData);
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x0018AA80 File Offset: 0x00188C80
		public MarryPartyResult MarryPartyJoin(GameClient client, int roleID)
		{
			MarryPartyResult result;
			if (!MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = MarryPartyResult.NotOpen;
			}
			else
			{
				MarryPartyData partyData = this.m_MarryPartyDataCache.GetParty(roleID);
				if (partyData == null)
				{
					result = MarryPartyResult.PartyNotFound;
				}
				else if (partyData.StartTime > TimeUtil.NOW())
				{
					result = MarryPartyResult.NotStart;
				}
				else
				{
					MarryPartyConfigData configData = this.GetMarryPartyConfigData(partyData.PartyType);
					if (null == configData)
					{
						result = MarryPartyResult.PartyNotFound;
					}
					else if (configData.JoinCost > Global.GetTotalBindTongQianAndTongQianVal(client))
					{
						result = MarryPartyResult.NotEnoughMoney;
					}
					else
					{
						Dictionary<int, int> joinList = client.ClientData.MyMarryPartyJoinList;
						int targetPartyJoinCount = 0;
						int allPartyJoinCount = 0;
						bool remove = false;
						lock (joinList)
						{
							joinList.TryGetValue(roleID, out targetPartyJoinCount);
							foreach (KeyValuePair<int, int> kv in client.ClientData.MyMarryPartyJoinList)
							{
								allPartyJoinCount += kv.Value;
							}
							if (allPartyJoinCount >= this.MarryPartyPlayerMaxJoinCount)
							{
								return MarryPartyResult.ZeroPlayerJoinCount;
							}
							if (targetPartyJoinCount >= configData.PlayerMaxJoinCount)
							{
								return MarryPartyResult.ZeroPlayerJoinCount;
							}
							if (!this.m_MarryPartyDataCache.IncPartyJoin(roleID, configData.PartyMaxJoinCount, out remove))
							{
								return MarryPartyResult.ZeroPartyJoinCount;
							}
							targetPartyJoinCount++;
							byte[] byteData = null;
							TCPProcessCmdResults dbResult = Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10190, string.Format("{0}:{1}:{2}", roleID, client.ClientData.RoleID, targetPartyJoinCount), out byteData, client.ServerId);
							if (TCPProcessCmdResults.RESULT_FAILED == dbResult || byteData == null || byteData.Length <= 6)
							{
								this.m_MarryPartyDataCache.IncPartyJoinCancel(roleID);
								return MarryPartyResult.ZeroPartyJoinCount;
							}
							joinList[roleID] = targetPartyJoinCount;
						}
						if (configData.JoinCost > 0)
						{
							if (!Global.SubBindTongQianAndTongQian(client, configData.JoinCost, "參予婚宴"))
							{
								return MarryPartyResult.NotEnoughMoney;
							}
						}
						if (configData.RewardExp > 0)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, (long)configData.RewardExp, false, true, false, "none");
						}
						if (configData.RewardShengWang > 0)
						{
							GameManager.ClientMgr.ModifyShengWangValue(client, configData.RewardShengWang, "婚宴奖励", false, true);
						}
						if (configData.RewardXingHun > 0)
						{
							GameManager.ClientMgr.ModifyStarSoulValue(client, configData.RewardXingHun, "婚宴奖励", false, true);
						}
						if (remove)
						{
							this.MarryPartyRemoveInternal(roleID, true, client, null);
							GoodsData goodData = Global.GetNewGoodsData(configData.RewardItem.GoodsID, configData.RewardItem.Binding);
							goodData.GCount = configData.JoinCost * configData.PartyMaxJoinCount / configData.GoodWillRatio / 2;
							List<GoodsData> goodList = new List<GoodsData>();
							goodList.Add(goodData);
							string sMsg = GLang.GetLang(493, new object[0]);
							Global.UseMailGivePlayerAward3(roleID, goodList, GLang.GetLang(494, new object[0]), sMsg, 0, 0, 0);
							int spouseID = (roleID == partyData.HusbandRoleID) ? partyData.WifeRoleID : partyData.HusbandRoleID;
							Global.UseMailGivePlayerAward3(spouseID, goodList, GLang.GetLang(494, new object[0]), sMsg, 0, 0, 0);
						}
						this.SendMarryPartyJoinList(client);
						this.SendMarryPartyList(client, partyData, -1);
						result = MarryPartyResult.Success;
					}
				}
			}
			return result;
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x0018AE74 File Offset: 0x00189074
		public void MarryPartyPeriodicUpdate(long ticks)
		{
			if (ticks >= this.NextUpdateTime)
			{
				this.NextUpdateTime = ticks + 10000L;
				bool showNPC = this.m_MarryPartyDataCache.HasPartyStarted(ticks);
				if (showNPC != this.MarryPartyNPCShow)
				{
					this.MarryPartyNPCShow = showNPC;
					if (showNPC)
					{
						GameMap gameMap = GameManager.MapMgr.DictMaps[this.MarryPartyNPCConfig.MapCode];
						NPC npc = NPCGeneralManager.GetNPCFromConfig(this.MarryPartyNPCConfig.MapCode, this.MarryPartyNPCConfig.NpcID, this.MarryPartyNPCConfig.NpcX, this.MarryPartyNPCConfig.NpcY, this.MarryPartyNPCConfig.NpcDir);
						if (null != npc)
						{
							if (NPCGeneralManager.AddNpcToMap(npc))
							{
								this.MarryPartyNpc = npc;
							}
							else
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("add marry party npc failure, MapCode={0}, NpcID={1}", this.MarryPartyNPCConfig.MapCode, this.MarryPartyNPCConfig.NpcID), null, true);
							}
						}
					}
					else if (null != this.MarryPartyNpc)
					{
						NPCGeneralManager.RemoveMapNpc(this.MarryPartyNPCConfig.MapCode, this.MarryPartyNPCConfig.NpcID);
						this.MarryPartyNpc = null;
					}
				}
			}
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x0018AFC0 File Offset: 0x001891C0
		public void SendMarryPartyList(GameClient client, MarryPartyData partyData, int roleID = -1)
		{
			if (partyData != null || roleID > 0)
			{
				if (partyData == null)
				{
					partyData = this.m_MarryPartyDataCache.GetParty(roleID);
					if (partyData == null)
					{
						int SpouseID = MarryLogic.GetSpouseID(roleID);
						partyData = this.m_MarryPartyDataCache.GetParty(SpouseID);
						if (partyData == null)
						{
							partyData = new MarryPartyData();
						}
					}
				}
				client.sendCmd<Dictionary<int, MarryPartyData>>(880, new Dictionary<int, MarryPartyData>
				{
					{
						roleID,
						partyData
					}
				}, false);
			}
			else
			{
				client.sendCmd(this.m_MarryPartyDataCache.GetPartyList(TCPOutPacketPool.getInstance(), 880), true);
			}
		}

		// Token: 0x06001951 RID: 6481 RVA: 0x0018B074 File Offset: 0x00189274
		public void SendMarryPartyJoinList(GameClient client)
		{
			if (null != client.ClientData.MyMarryPartyJoinList)
			{
				client.sendCmd<Dictionary<int, int>>(884, client.ClientData.MyMarryPartyJoinList, false);
			}
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x0018B0B4 File Offset: 0x001892B4
		public TCPProcessCmdResults ProcessMarryPartyQuery(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (null == client)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (client.ClientSocket.IsKuaFuLogin)
				{
					client.sendCmd<Dictionary<int, MarryPartyData>>(880, new Dictionary<int, MarryPartyData>(), false);
				}
				else
				{
					this.SendMarryPartyList(client, null, roleID);
				}
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x0018B24C File Offset: 0x0018944C
		public TCPProcessCmdResults ProcessMarryPartyCreate(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int partyType = Convert.ToInt32(fields[1]);
				long startTime = Convert.ToInt64(fields[2]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (client.ClientSocket.IsKuaFuLogin)
				{
					client.sendCmd(nID, string.Format("{0}:{1}", -12, roleID), false);
					return TCPProcessCmdResults.RESULT_OK;
				}
				MarryPartyResult result = this.MarryPartyCreate(client, partyType, startTime);
				string strcmd = string.Format("{0}:{1}", (int)result, roleID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessMarryPartyCreate", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x0018B430 File Offset: 0x00189630
		public TCPProcessCmdResults ProcessMarryPartyCancel(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (client.ClientSocket.IsKuaFuLogin)
				{
					client.sendCmd(nID, string.Format("{0}:{1}", -12, roleID), false);
					return TCPProcessCmdResults.RESULT_OK;
				}
				MarryPartyResult result = this.MarryPartyCancel(client);
				string strcmd = string.Format("{0}:{1}", (int)result, roleID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessMarryPartyCancel", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x0018B5FC File Offset: 0x001897FC
		public TCPProcessCmdResults ProcessMarryPartyJoin(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int holderRoleID = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				MarryPartyResult result = this.MarryPartyJoin(client, holderRoleID);
				string strcmd = string.Format("{0}:{1}", (int)result, roleID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessJoinQingGongYanCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x0018B77C File Offset: 0x0018997C
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			this.m_MarryPartyDataCache.OnChangeName(roleId, oldName, newName);
		}

		// Token: 0x04002365 RID: 9061
		private MarryPartyDataCache m_MarryPartyDataCache = new MarryPartyDataCache();

		// Token: 0x04002366 RID: 9062
		private Dictionary<int, MarryPartyConfigData> MarryPartyConfigList = new Dictionary<int, MarryPartyConfigData>();

		// Token: 0x04002367 RID: 9063
		private MarryPartyNPCData MarryPartyNPCConfig = new MarryPartyNPCData();

		// Token: 0x04002368 RID: 9064
		private int MarryPartyPlayerMaxJoinCount;

		// Token: 0x04002369 RID: 9065
		private int MarryPartyJoinListResetTime = 0;

		// Token: 0x0400236A RID: 9066
		private bool MarryPartyNPCShow = false;

		// Token: 0x0400236B RID: 9067
		private NPC MarryPartyNpc = null;

		// Token: 0x0400236C RID: 9068
		private static MarryPartyLogic Instance = new MarryPartyLogic();

		// Token: 0x0400236D RID: 9069
		private long NextUpdateTime = 0L;
	}
}
