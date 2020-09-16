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
using Server.Tools.Pattern;
using Tmsk.Tools.Tools;

namespace GameServer.Logic.TuJian
{
	
	public class GuardStatueManager : SingletonTemplate<GuardStatueManager>
	{
		
		private GuardStatueManager()
		{
		}

		
		public void LoadConfig()
		{
			if (!this.loadGuardSoul() || !this.loadGuardPoint() || !this.loadGuardLevelUp() || !this.loadGuardSuitUp())
			{
			}
		}

		
		private bool loadGuardSoul()
		{
			try
			{
				XElement xml = XElement.Load(Global.GameResPath("Config/TuJianShouHuType.xml"));
				if (xml == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("{0} 不存在!", Global.GameResPath("Config/TuJianShouHuType.xml")), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (xmlItem != null)
					{
						GuardSoul soul = new GuardSoul();
						soul.TypeID = (int)Global.GetSafeAttributeLong(xmlItem, "Type");
						soul.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
						soul.GoodsID = (int)Global.GetSafeAttributeLong(xmlItem, "GoodsID");
						this.guardSoulDict.Add(soul.TypeID, soul);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("{0} 读取出错!", Global.GameResPath("Config/TuJianShouHuType.xml")), ex, true);
				return false;
			}
			return true;
		}

		
		private bool loadGuardPoint()
		{
			try
			{
				XElement xml = XElement.Load(Global.GameResPath("Config/JingPoShouHu.xml"));
				if (xml == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("{0} 不存在!", Global.GameResPath("Config/JingPoShouHu.xml")), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (xmlItem != null)
					{
						GuardPoint point = new GuardPoint();
						point.ItemID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						point.TypeID = (int)Global.GetSafeAttributeLong(xmlItem, "Type");
						point.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
						point.GoodsID = (int)Global.GetSafeAttributeLong(xmlItem, "GoodsID");
						point.Point = (int)Global.GetSafeAttributeLong(xmlItem, "ShouHuAward");
						this.guardPointDict.Add(point.ItemID, point);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("{0} 读取出错!", Global.GameResPath("Config/JingPoShouHu.xml")), ex, true);
				return false;
			}
			return true;
		}

		
		private bool loadGuardLevelUp()
		{
			try
			{
				XElement xml = XElement.Load(Global.GameResPath("Config/ShouHuLevelUp.xml"));
				if (xml == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("{0} 不存在!", Global.GameResPath("Config/ShouHuLevelUp.xml")), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (xmlItem != null)
					{
						GuardLevelUp levelUp = new GuardLevelUp();
						levelUp.Level = (int)Global.GetSafeAttributeLong(xmlItem, "Level");
						levelUp.NeedGuardPoint = (int)Global.GetSafeAttributeLong(xmlItem, "NeedShouHu");
						this.guardLevelUpDict.Add(levelUp.Level, levelUp);
						this.GuardStatueMaxLevel = Math.Max(this.GuardStatueMaxLevel, levelUp.Level);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("{0} 读取出错!", Global.GameResPath("Config/ShouHuLevelUp.xml")), ex, true);
				return false;
			}
			return true;
		}

		
		private bool loadGuardSuitUp()
		{
			try
			{
				XElement xml = XElement.Load(Global.GameResPath("Config/ShouHuSuitUp.xml"));
				if (xml == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("{0} 不存在!", Global.GameResPath("Config/ShouHuSuitUp.xml")), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (xmlItem != null)
					{
						GuardSuitUp suitUp = new GuardSuitUp();
						suitUp.Suit = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						string szGoods = Global.GetSafeAttributeStr(xmlItem, "NeedGoods");
						suitUp.GoodsCost = ConfigHelper.ParserIntArrayList(szGoods, true, '|', ',');
						this.guardSuitUpDict.Add(suitUp.Suit, suitUp);
						this.GuardStatueMaxSuit = Math.Max(this.GuardStatueMaxSuit, suitUp.Suit);
					}
				}
				this.GuardStatueMaxSuit = (int)((sbyte)Global.GMin(this.GuardStatueMaxSuit, (int)GameManager.systemParamsList.GetParamValueIntByName("ShouHuShenMax", 0)));
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("{0} 读取出错!", Global.GameResPath("Config/ShouHuSuitUp.xml")), ex, true);
				return false;
			}
			return true;
		}

		
		public void OnTaskComplete(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				if (client != null && !client.ClientData.MyGuardStatueDetail.IsActived)
				{
					this.CheckGuardStatueOpenInfo(client);
				}
			}
		}

		
		public void OnLogin(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				if (client != null)
				{
					this.CheckGuardStatueOpenInfo(client);
					this.UpdateGuardStatueProps(client);
				}
			}
		}

		
		public void OnActiveTuJian(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				if (client != null)
				{
					this.CheckGuardStatueOpenInfo(client);
				}
			}
		}

		
		private void CheckGuardStatueOpenInfo(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				if (client != null)
				{
					if (!client.ClientData.MyGuardStatueDetail.IsActived)
					{
						if (GlobalNew.IsGongNengOpened(client, GongNengIDs.GuardStatue, false))
						{
							client.ClientData.MyGuardStatueDetail.IsActived = true;
							client.ClientData.MyGuardStatueDetail.GuardStatue.Level = 0;
							client.ClientData.MyGuardStatueDetail.GuardStatue.Suit = 1;
							client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint = 0;
							client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Clear();
							client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint = 0;
							client.ClientData.MyGuardStatueDetail.LastdayRecoverOffset = Global.GetOffsetDay(TimeUtil.NowDateTime());
							client.ClientData.MyGuardStatueDetail.ActiveSoulSlot = 0;
							if (!this._UpdateGuardStatue2DB(client))
							{
								client.ClientData.MyGuardStatueDetail.IsActived = false;
							}
						}
					}
					if (client.ClientData.MyGuardStatueDetail.IsActived)
					{
						using (HashSet<int>.Enumerator enumerator = client.ClientData.ActivedTuJianType.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								int type = enumerator.Current;
								if (!client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Exists((GuardSoulData soul) => soul.Type == type))
								{
									GuardSoulData soulData = new GuardSoulData
									{
										Type = type,
										EquipSlot = -1
									};
									if (this._UpdateGuardSoul2DB(client.ClientData.RoleID, soulData.Type, soulData.EquipSlot, client.ServerId))
									{
										client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Add(soulData);
									}
								}
							}
						}
						int newGuardSoulCnt = client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Count;
						int newSoltCnt = this.GetSlotCntBySoulCnt(newGuardSoulCnt);
						if (client.ClientData.MyGuardStatueDetail.ActiveSoulSlot != newSoltCnt)
						{
							int oldSlotCnt = client.ClientData.MyGuardStatueDetail.ActiveSoulSlot;
							client.ClientData.MyGuardStatueDetail.ActiveSoulSlot = newSoltCnt;
							if (!this._UpdateGuardStatue2DB(client))
							{
								client.ClientData.MyGuardStatueDetail.ActiveSoulSlot = oldSlotCnt;
							}
						}
					}
				}
			}
		}

		
		private void UpdateGuardStatueProps(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				if (client != null && client.ClientData.MyGuardStatueDetail.IsActived)
				{
					EquipPropItem props = new EquipPropItem();
					foreach (GuardSoulData soul in client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList)
					{
						if (soul.EquipSlot != -1)
						{
							GuardSoul gs = null;
							if (this.guardSoulDict.TryGetValue(soul.Type, out gs))
							{
								EquipPropItem tmp = GameManager.EquipPropsMgr.FindEquipPropItem(gs.GoodsID);
								if (tmp != null)
								{
									int level = client.ClientData.MyGuardStatueDetail.GuardStatue.Level;
									int suit = client.ClientData.MyGuardStatueDetail.GuardStatue.Suit;
									for (int i = 0; i < tmp.ExtProps.Length; i++)
									{
										props.ExtProps[i] += tmp.ExtProps[i] * (1.0 + (double)level * this.LevelFactor + (double)(suit - 1) * this.SuitFactor);
									}
								}
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.GuardStatue,
						props.ExtProps
					});
				}
			}
		}

		
		private bool _UpdateGuardStatue2DB(GameClient client)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = false;
			}
			else if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = false;
			}
			else
			{
				int roleid = client.ClientData.RoleID;
				int slotCnt = client.ClientData.MyGuardStatueDetail.ActiveSoulSlot;
				int level = client.ClientData.MyGuardStatueDetail.GuardStatue.Level;
				int suit = client.ClientData.MyGuardStatueDetail.GuardStatue.Suit;
				int totalGuardPoint = client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint;
				int lastdayRecoverPoint = client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint;
				int lastdayRecoverOffset = client.ClientData.MyGuardStatueDetail.LastdayRecoverOffset;
				string cmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
				{
					client.ClientData.RoleID,
					slotCnt,
					level,
					suit,
					totalGuardPoint,
					lastdayRecoverPoint,
					lastdayRecoverOffset
				});
				string[] dbRsp = Global.ExecuteDBCmd(13210, cmd, client.ServerId);
				if (dbRsp != null && dbRsp.Length != 1 && Convert.ToInt32(dbRsp[0]) < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色守护雕像失败，roleid={0}, slot={1}, level={2}, suit={3}, totalGuardPoint={4}, lastdayRecoverPoint={5}, lastdayRecoverOffset={6}", new object[]
					{
						roleid,
						slotCnt,
						level,
						suit,
						totalGuardPoint,
						lastdayRecoverPoint,
						lastdayRecoverOffset
					}), null, true);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		
		private bool _UpdateGuardSoul2DB(int roleid, int soulType, int equipSlot, int serverId)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = false;
			}
			else
			{
				string cmdText = string.Format("{0}:{1}:{2}", roleid, soulType, equipSlot);
				string[] dbRsp = Global.ExecuteDBCmd(13211, cmdText, serverId);
				if (dbRsp != null && dbRsp.Length != 1 && Convert.ToInt32(dbRsp[0]) < 0)
				{
					LogManager.WriteLog(LogTypes.Error, "更新角色守护之灵信息失败，" + cmdText, null, true);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		
		public TCPProcessCmdResults ProcRoleQueryGuardPointRecover(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int todayHasRecover = 0;
				int todayMaxRecover = 0;
				GuardStatueErrorCode ec = this.QueryGuardPointRecoverInfo(client, out todayHasRecover, out todayMaxRecover);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}:{2}", (int)ec, todayHasRecover, todayMaxRecover), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		private GuardStatueErrorCode QueryGuardPointRecoverInfo(GameClient client, out int todayHasRecover, out int todayMaxRecover)
		{
			todayHasRecover = 0;
			todayMaxRecover = 0;
			GuardStatueErrorCode result;
			if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else
			{
				int guardSoulCnt = client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Count;
				int nowOffsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				if (client.ClientData.MyGuardStatueDetail.LastdayRecoverOffset != nowOffsetDay)
				{
					client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint = 0;
					client.ClientData.MyGuardStatueDetail.LastdayRecoverOffset = nowOffsetDay;
					if (!this._UpdateGuardStatue2DB(client))
					{
					}
				}
				todayHasRecover = client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint;
				todayMaxRecover = this.GetDayMaxCanRecoverPointBySoulCnt(guardSoulCnt);
				result = GuardStatueErrorCode.Success;
			}
			return result;
		}

		
		public TCPProcessCmdResults ProcRoleGuardPointRecover(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length <= 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				Dictionary<int, int> itemDict = new Dictionary<int, int>();
				for (int i = 1; i < fields.Length; i++)
				{
					string[] szItem = fields[i].Split(new char[]
					{
						','
					});
					if (szItem.Length == 2)
					{
						int itemID = Convert.ToInt32(szItem[0]);
						int itemCnt = Convert.ToInt32(szItem[1]);
						if (itemDict.ContainsKey(itemID))
						{
							Dictionary<int, int> dictionary;
							int key;
							(dictionary = itemDict)[key = itemID] = dictionary[key] + itemCnt;
						}
						else
						{
							itemDict.Add(itemID, itemCnt);
						}
					}
				}
				GuardStatueErrorCode ec = this.RecoverGuardPoint(client, itemDict);
				int todayHasRecover = 0;
				int todayMaxRecover = 0;
				this.QueryGuardPointRecoverInfo(client, out todayHasRecover, out todayMaxRecover);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}:{2}", (int)ec, todayHasRecover, todayMaxRecover), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		private GuardStatueErrorCode RecoverGuardPoint(GameClient client, Dictionary<int, int> itemDict)
		{
			GuardStatueErrorCode result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else if (client == null || itemDict == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else
			{
				List<Tuple<int, int>> costGoodsList = new List<Tuple<int, int>>();
				int totalPoint = 0;
				int todayHasRecoverPoint = 0;
				int todayMaxRecoverPoint = 0;
				this.QueryGuardPointRecoverInfo(client, out todayHasRecoverPoint, out todayMaxRecoverPoint);
				foreach (KeyValuePair<int, int> kvp in itemDict)
				{
					int itemID = kvp.Key;
					int itemCnt = kvp.Value;
					if (client.ClientData.ActivedTuJianItem.Contains(itemID))
					{
						GuardPoint gp = null;
						if (this.guardPointDict.TryGetValue(itemID, out gp))
						{
							if (client.ClientData.ActivedTuJianType.Contains(gp.TypeID))
							{
								int canAddPoint = todayMaxRecoverPoint - todayHasRecoverPoint - totalPoint;
								if (canAddPoint <= 0)
								{
									break;
								}
								int realCanRecoverCnt = canAddPoint / gp.Point + ((canAddPoint % gp.Point != 0) ? 1 : 0);
								realCanRecoverCnt = Math.Max(0, Math.Min(realCanRecoverCnt, itemCnt));
								if (Global.GetTotalGoodsCountByID(client, gp.GoodsID) >= realCanRecoverCnt)
								{
									costGoodsList.Add(new Tuple<int, int>(gp.GoodsID, realCanRecoverCnt));
									totalPoint += gp.Point * realCanRecoverCnt;
									if (totalPoint >= todayMaxRecoverPoint - todayHasRecoverPoint)
									{
										break;
									}
								}
							}
						}
					}
				}
				if (costGoodsList.Count == 0 || totalPoint <= 0)
				{
					result = GuardStatueErrorCode.MaterialNotEnough;
				}
				else
				{
					if (totalPoint > todayMaxRecoverPoint - todayHasRecoverPoint)
					{
					}
					foreach (Tuple<int, int> tuple in costGoodsList)
					{
						int goodsID = tuple.Item1;
						int goodsCnt = tuple.Item2;
						bool usedBinding_just_placeholder = false;
						bool usedTimeLimited_just_placeholder = false;
						GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsID, goodsCnt, false, out usedBinding_just_placeholder, out usedTimeLimited_just_placeholder, false);
					}
					GuardStatueData data = client.ClientData.MyGuardStatueDetail.GuardStatue;
					int validPoint = Math.Min(totalPoint, todayMaxRecoverPoint - todayHasRecoverPoint);
					data.HasGuardPoint += validPoint;
					client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint += validPoint;
					if (!this._UpdateGuardStatue2DB(client))
					{
						data.HasGuardPoint -= validPoint;
						client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint -= validPoint;
						result = GuardStatueErrorCode.DBFailed;
					}
					else
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TotalGuardPoint, data.HasGuardPoint);
						EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.GuardPoint, (long)validPoint, (long)data.HasGuardPoint, "回收守护点");
						result = GuardStatueErrorCode.Success;
					}
				}
			}
			return result;
		}

		
		public TCPProcessCmdResults ProcRoleQueryGuardStatue(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GuardStatueData>(client.ClientData.MyGuardStatueDetail.GuardStatue, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public TCPProcessCmdResults ProcRoleGuardStatueLevelUp(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				GuardStatueErrorCode err = this.HandleLevelUp(client);
				int level = client.ClientData.MyGuardStatueDetail.GuardStatue.Level;
				int hasGuardPoint = client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint;
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}:{2}", (int)err, level, hasGuardPoint), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public GuardStatueErrorCode HandleLevelUp(GameClient client)
		{
			GuardStatueErrorCode result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else
			{
				GuardStatueData data = client.ClientData.MyGuardStatueDetail.GuardStatue;
				if (data.Level >= this.GuardStatueMaxLevel)
				{
					result = GuardStatueErrorCode.LevelIsFull;
				}
				else if (data.Level > 0 && data.Level % 10 == 0 && (data.Level + 10) / 10 != data.Suit)
				{
					result = GuardStatueErrorCode.NeedSuitUp;
				}
				else
				{
					GuardLevelUp levelUp = null;
					if (!this.guardLevelUpDict.TryGetValue(data.Level + 1, out levelUp))
					{
						result = GuardStatueErrorCode.ConfigError;
					}
					else if (levelUp.NeedGuardPoint > data.HasGuardPoint)
					{
						result = GuardStatueErrorCode.GuardPointNotEnough;
					}
					else
					{
						data.HasGuardPoint -= levelUp.NeedGuardPoint;
						data.Level++;
						if (!this._UpdateGuardStatue2DB(client))
						{
							data.HasGuardPoint += levelUp.NeedGuardPoint;
							data.Level--;
							result = GuardStatueErrorCode.DBFailed;
						}
						else
						{
							GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TotalGuardPoint, data.HasGuardPoint);
							EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.GuardPoint, (long)(-(long)levelUp.NeedGuardPoint), (long)data.HasGuardPoint, "升级守护雕像");
							this.UpdateGuardStatueProps(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							if (client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							result = GuardStatueErrorCode.Success;
						}
					}
				}
			}
			return result;
		}

		
		public TCPProcessCmdResults ProcRoleGuardStatueSuitUp(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				GuardStatueErrorCode err = this.HandleSuitUp(client);
				int suit = client.ClientData.MyGuardStatueDetail.GuardStatue.Suit;
				int hasGuardPoint = client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint;
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}:{2}", (int)err, suit, hasGuardPoint), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		private GuardStatueErrorCode HandleSuitUp(GameClient client)
		{
			GuardStatueErrorCode result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else
			{
				GuardStatueData data = client.ClientData.MyGuardStatueDetail.GuardStatue;
				string strCostList = "";
				int oldSuit = data.Suit;
				if (data.Suit >= this.GuardStatueMaxSuit)
				{
					result = GuardStatueErrorCode.SuitIsFull;
				}
				else if (data.Level == 0 || (data.Level + 10) / 10 == data.Suit)
				{
					result = GuardStatueErrorCode.NeedLevelUp;
				}
				else
				{
					GuardSuitUp suitUp = null;
					if (!this.guardSuitUpDict.TryGetValue(data.Suit + 1, out suitUp))
					{
						result = GuardStatueErrorCode.ConfigError;
					}
					else
					{
						for (int i = 0; i < suitUp.GoodsCost.Count; i++)
						{
							int goodsId = suitUp.GoodsCost[i][0];
							int costCount = suitUp.GoodsCost[i][1];
							int haveGoodsCnt = Global.GetTotalGoodsCountByID(client, goodsId);
							if (haveGoodsCnt < costCount)
							{
								return GuardStatueErrorCode.MaterialNotEnough;
							}
						}
						data.Suit++;
						if (!this._UpdateGuardStatue2DB(client))
						{
							data.Suit--;
							result = GuardStatueErrorCode.DBFailed;
						}
						else
						{
							bool bUsedBinding_just_placeholder = false;
							bool bUsedTimeLimited_just_placeholder = false;
							for (int i = 0; i < suitUp.GoodsCost.Count; i++)
							{
								int goodsId = suitUp.GoodsCost[i][0];
								int costCount = suitUp.GoodsCost[i][1];
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsId, costCount, false, out bUsedBinding_just_placeholder, out bUsedTimeLimited_just_placeholder, false))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("守护雕像进阶时，消耗{1}个GoodsID={0}的物品失败，但是已设置为升阶成功", goodsId, costCount), null, true);
								}
								GoodsData goodsDataCost = new GoodsData
								{
									GoodsID = goodsId,
									GCount = costCount
								};
								strCostList += EventLogManager.NewGoodsDataPropString(goodsDataCost);
							}
							GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TotalGuardPoint, data.HasGuardPoint);
							this.UpdateGuardStatueProps(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							if (client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							EventLogManager.AddGuardStatueSuitEvent(client, oldSuit, data.Suit, data.Level, data.Level, strCostList);
							result = GuardStatueErrorCode.Success;
						}
					}
				}
			}
			return result;
		}

		
		public TCPProcessCmdResults ProcRoleModGuardSoulEquip(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int slot = Convert.ToInt32(fields[1]);
				int soulType = Convert.ToInt32(fields[2]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				this.HandleModGuardSoulEquip(client, slot, soulType);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GuardStatueData>(client.ClientData.MyGuardStatueDetail.GuardStatue, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		private void HandleModGuardSoulEquip(GameClient client, int slot, int guardSoulType)
		{
			if (client != null && client.ClientData.MyGuardStatueDetail.IsActived)
			{
				int slotCnt = client.ClientData.MyGuardStatueDetail.ActiveSoulSlot;
				if (slot >= 0 && slot < slotCnt)
				{
					GuardSoulData oldSoulData = client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Find((GuardSoulData soul) => soul.EquipSlot == slot);
					GuardSoulData newSoulData = null;
					if (guardSoulType != -1)
					{
						newSoulData = client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Find((GuardSoulData soul) => soul.Type == guardSoulType);
					}
					if ((oldSoulData != null || newSoulData != null) && oldSoulData != newSoulData)
					{
						if (newSoulData == null || newSoulData.EquipSlot == -1)
						{
							if (oldSoulData != null && this._UpdateGuardSoul2DB(client.ClientData.RoleID, oldSoulData.Type, -1, client.ServerId))
							{
								oldSoulData.EquipSlot = -1;
							}
							if (oldSoulData == null || oldSoulData.EquipSlot == -1)
							{
								if (newSoulData != null && this._UpdateGuardSoul2DB(client.ClientData.RoleID, newSoulData.Type, slot, client.ServerId))
								{
									newSoulData.EquipSlot = slot;
								}
							}
							this.UpdateGuardStatueProps(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
					}
				}
			}
		}

		
		private int GetSlotCntBySoulCnt(int cnt)
		{
			int slot = 0;
			foreach (Tuple<int, int> t in this.maxActiveSlotCntList)
			{
				if (cnt >= t.Item1)
				{
					slot = Math.Max(slot, t.Item2);
				}
			}
			return slot;
		}

		
		private int GetDayMaxCanRecoverPointBySoulCnt(int cnt)
		{
			int maxPoint = 0;
			foreach (Tuple<int, int> t in this.dayMaxCanRecoverPointList)
			{
				if (cnt >= t.Item1)
				{
					maxPoint = Math.Max(maxPoint, t.Item2);
				}
			}
			return maxPoint;
		}

		
		public void InitRecoverPoint_BySysParam(string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				this.dayMaxCanRecoverPointList.Clear();
				string[] szDetail = str.Split(new char[]
				{
					'|'
				});
				foreach (string s in szDetail)
				{
					string[] szSlot = s.Split(new char[]
					{
						','
					});
					if (szSlot.Length == 2)
					{
						int soulCnt = Convert.ToInt32(szSlot[0]);
						int maxPoint = Convert.ToInt32(szSlot[1]);
						this.dayMaxCanRecoverPointList.Add(new Tuple<int, int>(soulCnt, maxPoint));
					}
				}
			}
		}

		
		public void InitSoulSlot_BySysParam(string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				this.maxActiveSlotCntList.Clear();
				string[] szDetail = str.Split(new char[]
				{
					'|'
				});
				foreach (string s in szDetail)
				{
					string[] szSlot = s.Split(new char[]
					{
						','
					});
					if (szSlot.Length == 2)
					{
						int slotCnt = Convert.ToInt32(szSlot[0]);
						int needSoulCnt = Convert.ToInt32(szSlot[1]);
						this.maxActiveSlotCntList.Add(new Tuple<int, int>(needSoulCnt, slotCnt));
					}
				}
			}
		}

		
		public void GM_HandleModGuardSoulEquip(GameClient client, int slot, int guardSoulType)
		{
			this.HandleModGuardSoulEquip(client, slot, guardSoulType);
		}

		
		public string GM_QueryGuardPoint(GameClient client)
		{
			int todayHasRecover = 0;
			int todayCanRecover = 0;
			this.QueryGuardPointRecoverInfo(client, out todayHasRecover, out todayCanRecover);
			int totalHas = (client != null) ? client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint : 0;
			return string.Format("今日已回收[{0}], 今日最大可回收[{1}]，总共有守护点[{2}]", todayHasRecover, todayCanRecover, totalHas);
		}

		
		public void GM_GuardPointRecover(GameClient client, Dictionary<int, int> itemDict)
		{
			this.RecoverGuardPoint(client, itemDict);
		}

		
		public void GM_HandleLevelUp(GameClient client)
		{
			this.HandleLevelUp(client);
		}

		
		public void GM_HandleSuitlUp(GameClient client)
		{
			this.HandleSuitUp(client);
		}

		
		public string GM_QueryGuardStatue(GameClient client)
		{
			string result;
			if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = "守护雕像未激活";
			}
			else
			{
				string tip = string.Format("守护雕像已激活, 等级={0}, 品阶={1}, 激活的守护之灵装备栏={2}, 共有守护之灵={3}， ", new object[]
				{
					client.ClientData.MyGuardStatueDetail.GuardStatue.Level,
					client.ClientData.MyGuardStatueDetail.GuardStatue.Suit,
					this.GetSlotCntBySoulCnt(client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Count),
					client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Count
				});
				tip += this.GM_QueryGuardPoint(client);
				foreach (GuardSoulData soul in client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList)
				{
					tip += string.Format(", 【type={0}, slot={1}】", soul.Type, soul.EquipSlot);
				}
				result = tip;
			}
			return result;
		}

		
		public string GM_ModGuardPoint(GameClient client, int newVal)
		{
			string result;
			if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = "守护雕像未激活";
			}
			else
			{
				client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint = newVal;
				if (!this._UpdateGuardStatue2DB(client))
				{
					result = "设置守护点失败";
				}
				else
				{
					GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TotalGuardPoint, client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint);
					EventLogManager.AddMoneyEvent(client, OpTypes.Set, OpTags.None, MoneyTypes.GuardPoint, 0L, (long)newVal, "GM设置");
					result = "设置守护点成功";
				}
			}
			return result;
		}

		
		public void AddGuardPoint(GameClient client, int point, string strFrom)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot6))
			{
				if (client != null && client.ClientData.MyGuardStatueDetail.IsActived)
				{
					client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint += point;
					if (!this._UpdateGuardStatue2DB(client))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("AddGuardPoint failed, roleid={0}, rolename={1}, addpoint={2}", client.ClientData.RoleID, client.ClientData.RoleName, point), null, true);
					}
					else
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TotalGuardPoint, client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint);
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "守护点", strFrom, "系统", client.ClientData.RoleName, "修改", point, client.ClientData.ZoneID, client.strUserID, client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint, client.ServerId, null);
						EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.GuardPoint, (long)point, (long)client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint, strFrom);
					}
				}
			}
		}

		
		private Dictionary<int, GuardSoul> guardSoulDict = new Dictionary<int, GuardSoul>();

		
		private Dictionary<int, GuardPoint> guardPointDict = new Dictionary<int, GuardPoint>();

		
		private Dictionary<int, GuardLevelUp> guardLevelUpDict = new Dictionary<int, GuardLevelUp>();

		
		private Dictionary<int, GuardSuitUp> guardSuitUpDict = new Dictionary<int, GuardSuitUp>();

		
		private List<Tuple<int, int>> dayMaxCanRecoverPointList = new List<Tuple<int, int>>();

		
		private List<Tuple<int, int>> maxActiveSlotCntList = new List<Tuple<int, int>>();

		
		private int GuardStatueMaxLevel = 0;

		
		private int GuardStatueMaxSuit = 0;

		
		public double LevelFactor = 1.0;

		
		public double SuitFactor = 1.0;
	}
}
