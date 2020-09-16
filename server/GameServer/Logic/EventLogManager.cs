using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Logic.Goods;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class EventLogManager
	{
		
		
		private static long LogId
		{
			get
			{
				return Interlocked.Increment(ref EventLogManager._LogId);
			}
		}

		
		static EventLogManager()
		{
			EventLogManager.Init();
		}

		
		private static void Init()
		{
			for (int i = 0; i < 82; i++)
			{
				EventLogManager.SystemRoleEvents[i] = new ServerEvents
				{
					EventRootPath = "RoleEvents",
					EventPreFileName = ((RoleEvent)i).ToString()
				};
				EventLogManager.SystemRoleEvents[i].EventLevel = GameManager.SystemServerEvents.EventLevel;
			}
		}

		
		public static void WriteAllEvents()
		{
			for (int i = 0; i < 82; i++)
			{
				while (EventLogManager.SystemRoleEvents[i].WriteEvent())
				{
				}
			}
		}

		
		public static void AddMoneyEvent(GameClient client, OpTypes optType, OpTags optTag, MoneyTypes moneyType, long addValue, long curValue = -1L, string msg = "none")
		{
			try
			{
				if (GameManager.FlagEnableMoneyEventLog)
				{
					if (optType != OpTypes.AddOrSub || addValue != 0L)
					{
						if (curValue == -1L)
						{
							if (moneyType <= MoneyTypes.YuanBao)
							{
								if (moneyType != MoneyTypes.TongQian)
								{
									if (moneyType != MoneyTypes.YinLiang)
									{
										if (moneyType == MoneyTypes.YuanBao)
										{
											curValue = (long)client.ClientData.UserMoney;
										}
									}
									else
									{
										curValue = (long)client.ClientData.YinLiang;
									}
								}
								else
								{
									curValue = (long)client.ClientData.Money1;
								}
							}
							else if (moneyType <= MoneyTypes.BangGong)
							{
								if (moneyType != MoneyTypes.BindYuanBao)
								{
									if (moneyType == MoneyTypes.BangGong)
									{
										curValue = (long)client.ClientData.BangGong;
									}
								}
								else
								{
									curValue = (long)client.ClientData.Money2;
								}
							}
							else if (moneyType != MoneyTypes.XingHun)
							{
								if (moneyType == MoneyTypes.Exp)
								{
									curValue = client.ClientData.Experience;
								}
							}
							else
							{
								curValue = (long)client.ClientData.StarSoul;
							}
						}
						EventLogManager.AddMoneyEvent(client.ServerId, client.ClientData.ZoneID, client.strUserID, (long)client.ClientData.RoleID, optType, optTag, moneyType, addValue, curValue, msg);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public static void AddMoneyEvent(int serverId, int zoneId, string userId, long roleId, OpTypes optType, OpTags optTag, MoneyTypes moneyType, long addValue, long curValue, string msg)
		{
			try
			{
				if (GameManager.FlagEnableMoneyEventLog)
				{
					if (LogFilterConfig.LogMoneyTypeLog((int)moneyType))
					{
						if (zoneId == 0)
						{
							zoneId = CacheManager.GetZoneIdByRoleId(roleId, serverId);
						}
						string eventMsg = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}", new object[]
						{
							1,
							serverId,
							zoneId,
							userId,
							roleId,
							(int)moneyType,
							(int)optType,
							(int)optTag,
							addValue,
							curValue,
							msg
						});
						EventLogManager.SystemRoleEvents[73].AddEvent(eventMsg, EventLevels.Important);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public static void AddGoodsEvent(GameClient client, OpTypes optType, OpTags optTag, int goodsId, long dbId, int addValue, int curValue, string msg)
		{
			try
			{
				if (addValue != 0)
				{
					if (GameManager.FlagEnableGoodsEventLog)
					{
						if (LogFilterConfig.LogGoodsIdLog(goodsId))
						{
							int serverId = client.ServerId;
							string userID = client.strUserID;
							long roleId = (long)client.ClientData.RoleID;
							int zoneId = client.ClientData.ZoneID;
							string eventMsg = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}", new object[]
							{
								2,
								serverId,
								zoneId,
								userID,
								roleId,
								(int)optType,
								(int)optTag,
								goodsId,
								dbId,
								addValue,
								curValue,
								msg
							});
							EventLogManager.SystemRoleEvents[74].AddEvent(eventMsg, EventLevels.Important);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public static void AddGoodsEvent(RoleDataEx client, OpTypes optType, OpTags optTag, int goodsId, long dbId, int addValue, int curValue, string msg)
		{
			try
			{
				if (addValue != 0)
				{
					if (GameManager.FlagEnableGoodsEventLog)
					{
						if (LogFilterConfig.LogGoodsIdLog(goodsId))
						{
							int serverId = 0;
							string userID = client.userMiniData.UserId;
							long roleId = (long)client.RoleID;
							int zoneId = client.ZoneID;
							string eventMsg = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}", new object[]
							{
								2,
								serverId,
								zoneId,
								userID,
								roleId,
								(int)optType,
								(int)optTag,
								goodsId,
								dbId,
								addValue,
								curValue,
								msg
							});
							EventLogManager.SystemRoleEvents[74].AddEvent(eventMsg, EventLevels.Important);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public static void AddRoleLoginEvent(string userID, string isadult, string ip)
		{
			EventLogManager.SystemRoleEvents[0].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				userID,
				isadult,
				ip
			});
		}

		
		public static void AddRoleInitGameEvent(GameClient client, string hid)
		{
			string userID = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
			string ip = Global.GetSocketRemoteEndPoint(client.ClientSocket, false).Replace(":", ".");
			EventLogManager.SystemRoleEvents[1].AddImporEvent(new object[]
			{
				RoleEvent.InitGame.ToString(),
				GameManager.ServerId,
				userID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				client.ClientData.ChangeLifeCount,
				client.ClientData.Level,
				client.ClientData.MapCode,
				ip,
				hid
			});
		}

		
		public static void AddRoleLogoutEvent(GameClient client)
		{
			string userID = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
			string ip = Global.GetSocketRemoteEndPoint(client.ClientSocket, false).Replace(":", ".");
			string loginTime = client.ClientData.LastLoginTime.ToString("yyyy-MM-dd HH:mm:ss");
			EventLogManager.SystemRoleEvents[1].AddImporEvent(new object[]
			{
				RoleEvent.Logout.ToString(),
				GameManager.ServerId,
				userID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				client.ClientData.ChangeLifeCount,
				client.ClientData.Level,
				client.ClientData.MapCode,
				ip,
				loginTime
			});
		}

		
		public static void AddCreateRoleEvent(string userID, int zoneID, string strCmdResult, string ip, string hid)
		{
			try
			{
				string[] strFields = strCmdResult.Split(new char[]
				{
					':'
				});
				string strRst = strFields[0];
				strFields = strFields[1].Split(new char[]
				{
					'$'
				});
				int roleID = Global.SafeConvertToInt32(strFields[0]);
				int roleSex = Global.SafeConvertToInt32(strFields[1]);
				int roleOcc = Global.SafeConvertToInt32(strFields[2]);
				string roleName = strFields[3];
				if (!("1" != strRst))
				{
					EventLogManager.SystemRoleEvents[3].AddImporEvent(new object[]
					{
						GameManager.ServerId,
						userID,
						zoneID,
						roleID,
						roleName,
						roleSex,
						roleOcc,
						ip,
						hid
					});
				}
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("AddCreateRoleEvent Exception {0} {1}", userID, strCmdResult), null, true);
			}
		}

		
		public static void AddRemoveRoleEvent(string userID, int zoneID, int roleID, string ip)
		{
			EventLogManager.SystemRoleEvents[4].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				userID,
				zoneID,
				roleID,
				ip
			});
		}

		
		public static void AddRoleDeathEvent(GameClient client, int mapCode, ObjectTypes killerType, int killerId, string strDropList)
		{
			EventLogManager.SystemRoleEvents[6].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				mapCode,
				(int)killerType,
				killerId,
				strDropList
			});
		}

		
		public static void AddResourceEvent(GameClient client, MoneyTypes type, long surplus, long currValue, string reason)
		{
			EventLogManager.AddResourceEvent(client.strUserID, client.ClientData.ZoneID, client.ClientData.RoleID, type, surplus, currValue, reason);
		}

		
		public static void AddResourceEvent(string userID, int zoneID, int roleID, MoneyTypes type, long surplus, long currValue, string reason)
		{
			if (MoneyTypes.YuanBao == type)
			{
				EventLogManager.SystemRoleEvents[9].AddImporEvent(new object[]
				{
					GameManager.ServerId,
					userID,
					zoneID,
					roleID,
					surplus,
					currValue,
					reason
				});
			}
			else if (MoneyTypes.YinLiang == type)
			{
				EventLogManager.SystemRoleEvents[10].AddImporEvent(new object[]
				{
					GameManager.ServerId,
					userID,
					zoneID,
					roleID,
					surplus,
					currValue,
					reason
				});
			}
			else if (MoneyTypes.TongQian == type)
			{
				EventLogManager.SystemRoleEvents[11].AddImporEvent(new object[]
				{
					GameManager.ServerId,
					userID,
					zoneID,
					roleID,
					surplus,
					currValue,
					reason
				});
			}
			else if (MoneyTypes.BindYuanBao == type)
			{
				EventLogManager.SystemRoleEvents[12].AddImporEvent(new object[]
				{
					GameManager.ServerId,
					userID,
					zoneID,
					roleID,
					surplus,
					currValue,
					reason
				});
			}
			else if (MoneyTypes.ShengWang == type)
			{
				EventLogManager.SystemRoleEvents[13].AddImporEvent(new object[]
				{
					GameManager.ServerId,
					userID,
					zoneID,
					roleID,
					surplus,
					currValue,
					reason
				});
			}
			else
			{
				EventLogManager.SystemRoleEvents[8].AddImporEvent(new object[]
				{
					GameManager.ServerId,
					userID,
					zoneID,
					roleID,
					(int)type,
					surplus,
					currValue,
					reason
				});
			}
		}

		
		public static void AddLangHunLingYuEvent(int GameId, int cityID, int oldZoneID, int oldBHID, int oldLev, int newZoneID, int newBHID, int newLev)
		{
			EventLogManager.SystemRoleEvents[45].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				GameId,
				cityID,
				oldZoneID,
				oldBHID,
				oldLev,
				newZoneID,
				newBHID,
				newLev
			});
		}

		
		public static void AddLuoLanChengZhanEvent(BangHuiDetailData oldBangHuiDetailData, BangHuiDetailData newBangHuiDetailData)
		{
			EventLogManager.SystemRoleEvents[39].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				(oldBangHuiDetailData == null) ? 0 : oldBangHuiDetailData.ZoneID,
				(oldBangHuiDetailData == null) ? 0 : oldBangHuiDetailData.BHID,
				(oldBangHuiDetailData == null) ? 0 : oldBangHuiDetailData.QiLevel,
				(oldBangHuiDetailData == null) ? 0L : oldBangHuiDetailData.TotalCombatForce,
				(newBangHuiDetailData == null) ? 0 : newBangHuiDetailData.ZoneID,
				(newBangHuiDetailData == null) ? 0 : newBangHuiDetailData.BHID,
				(newBangHuiDetailData == null) ? 0 : newBangHuiDetailData.QiLevel,
				(newBangHuiDetailData == null) ? 0L : newBangHuiDetailData.TotalCombatForce
			});
		}

		
		public static void AddBangHuiBuildUpEvent(GameClient client, int bhid, int type, int tolevel, string resList)
		{
			EventLogManager.SystemRoleEvents[35].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				bhid,
				type,
				tolevel,
				resList
			});
		}

		
		public static void AddBangHuiCreateEvent(GameClient client, int bhid)
		{
			EventLogManager.SystemRoleEvents[36].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				bhid
			});
		}

		
		public static void AddBangHuiDestroyEvent(GameClient client, int bhid)
		{
			EventLogManager.SystemRoleEvents[37].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				bhid
			});
		}

		
		public static void AddBangHuiQuitEvent(GameClient client, int bhid)
		{
			EventLogManager.SystemRoleEvents[38].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				bhid
			});
		}

		
		public static void AddJunTuanCreateEvent(GameClient client, int juntuanID, int bhid, string strCostList)
		{
			EventLogManager.SystemRoleEvents[40].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				juntuanID,
				bhid,
				strCostList
			});
		}

		
		public static void AddJunTuanDestroyEvent(GameClient client, int juntuanID, int bhid)
		{
			EventLogManager.SystemRoleEvents[42].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				juntuanID,
				bhid
			});
		}

		
		public static void AddJunTuanZhiWuEvent(GameClient client, int optZhiWu, int otherRoleID, int zhiWu)
		{
			EventLogManager.SystemRoleEvents[41].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				client.ClientData.JunTuanId,
				client.ClientData.Faction,
				optZhiWu,
				otherRoleID,
				zhiWu
			});
		}

		
		public static void AddKarenBattleEvent(int lingDiType, RoleData4Selector oldLeader, int newZoneID, int newJunTuanId, int newRoleID)
		{
			EventLogManager.SystemRoleEvents[43].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				lingDiType,
				(oldLeader == null) ? 0 : oldLeader.ZoneId,
				(oldLeader == null) ? 0 : oldLeader.JunTuanId,
				(oldLeader == null) ? 0 : oldLeader.RoleID,
				newZoneID,
				newJunTuanId,
				newRoleID
			});
		}

		
		public static void AddKarenBattleEnterEvent(int lingDiType, GameClient client)
		{
			EventLogManager.SystemRoleEvents[44].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				lingDiType,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				client.ClientData.JunTuanId,
				client.ClientData.Faction,
				client.ClientData.CombatForce,
				Global.GetUnionLevel2(client)
			});
		}

		
		public static void AddRoleUpgradeEvent(GameClient client, long addExp, long oldExp, int oldLevel, string reason)
		{
			EventLogManager.SystemRoleEvents[5].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				addExp,
				oldExp,
				client.ClientData.Experience,
				oldLevel,
				Global.GetUnionLevel2(client),
				reason
			});
		}

		
		public static void AddChangeLifeEvent(GameClient client, int oldNum, int newNum, string resList)
		{
			EventLogManager.SystemRoleEvents[19].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				oldNum,
				newNum,
				resList
			});
		}

		
		public static void AddRoleRebornUpgradeEvent(GameClient client, long addExp, long oldExp, int oldLevel, string reason)
		{
			EventLogManager.SystemRoleEvents[7].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				addExp,
				oldExp,
				client.ClientData.Experience,
				oldLevel,
				Global.GetUnionLevel2(client.ClientData.RebornCount, client.ClientData.RebornLevel),
				reason
			});
		}

		
		public static void AddRebornEvent(GameClient client, int oldNum, int newNum, string resList)
		{
			EventLogManager.SystemRoleEvents[53].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				oldNum,
				newNum,
				resList
			});
		}

		
		public static void AddMazingerStoreEvent(GameClient client, int oldNum, int newNum, int oldExp, int newExp, string resList)
		{
			EventLogManager.SystemRoleEvents[54].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				oldNum,
				newNum,
				oldExp,
				newExp,
				resList
			});
		}

		
		public static void AddPurchaseEvent(GameClient client, int strFrom, int purchaseID, string castResList, string strResList)
		{
			EventLogManager.SystemRoleEvents[17].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				strFrom,
				purchaseID,
				castResList,
				strResList
			});
		}

		
		public static void AddTitleEvent(GameClient client, int operateType, int time, string resList)
		{
			EventLogManager.SystemRoleEvents[18].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				operateType,
				time,
				resList
			});
		}

		
		public static void AddFallGoodsEvent(int MapCode, int OwnerRoleID, string KilledName, string resList)
		{
			EventLogManager.SystemRoleEvents[34].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				MapCode,
				OwnerRoleID,
				KilledName,
				resList
			});
		}

		
		public static void AddRingStarSuitEvent(GameClient client, int RingID, int OldLev, int NewLev, int OldStar, int NewStar, string strCostList)
		{
			EventLogManager.SystemRoleEvents[30].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				RingID,
				OldLev,
				NewLev,
				OldStar,
				NewStar,
				strCostList
			});
		}

		
		public static void AddRingBuyEvent(GameClient client, int UpMode, int RingID, int OldLev, int NewLev, int OldStar, int NewStar, string strCostList)
		{
			EventLogManager.SystemRoleEvents[31].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				UpMode,
				RingID,
				OldLev,
				NewLev,
				OldStar,
				NewStar,
				strCostList
			});
		}

		
		public static void AddGuardStatueSuitEvent(GameClient client, int OldSuit, int NewSuit, int OldLev, int NewLev, string strCostList)
		{
			EventLogManager.SystemRoleEvents[29].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				OldSuit,
				NewSuit,
				OldLev,
				NewLev,
				strCostList
			});
		}

		
		public static void AddWingZhuLingEvent(GameClient client, int OldLev, int NewLev, string strCostList)
		{
			EventLogManager.SystemRoleEvents[26].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				OldLev,
				NewLev,
				EventLogManager.SystemRoleEvents
			});
		}

		
		public static void AddWingZhuHunEvent(GameClient client, int OldLev, int NewLev, string strCostList)
		{
			EventLogManager.SystemRoleEvents[27].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				OldLev,
				NewLev,
				EventLogManager.SystemRoleEvents
			});
		}

		
		public static void AddMerlinBookStarEvent(GameClient client, int UpStarMode, int AddExp, int OldStar, int NewLev, int NewStar, int NewStarExp, string strCostList)
		{
			EventLogManager.SystemRoleEvents[32].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				UpStarMode,
				AddExp,
				OldStar,
				NewLev,
				NewStar,
				NewStarExp,
				strCostList
			});
		}

		
		public static void AddMerlinBookLevEvent(GameClient client, int nUpMode, int OldFailedNum, int NewFailedNum, int OldLev, int NewLev, int OldStar, int NewStar, int OldStarExp, int NewStarExp, string strCostList)
		{
			EventLogManager.SystemRoleEvents[33].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				nUpMode,
				OldFailedNum,
				NewFailedNum,
				OldLev,
				NewLev,
				OldStar,
				NewStar,
				OldStarExp,
				NewStarExp,
				strCostList
			});
		}

		
		public static void AddWingStarEvent(GameClient client, int UpStarMode, int AddExp, int OldStarLevel, int WingID, int NewStarLevel, int NewStarExp, string strCostList)
		{
			EventLogManager.SystemRoleEvents[14].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				UpStarMode,
				AddExp,
				OldStarLevel,
				WingID,
				NewStarLevel,
				NewStarExp,
				strCostList
			});
		}

		
		public static void AddWingUpgradeEvent(GameClient client, int nUpWingMode, int OldJinJieFailedNum, int NewJinJieFailedNum, int OldWingID, int NewWingID, int OldStarLevel, int NewStarLevel, int OldStarExp, int NewStarExp, string strCostList)
		{
			EventLogManager.SystemRoleEvents[15].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				nUpWingMode,
				OldJinJieFailedNum,
				NewJinJieFailedNum,
				OldWingID,
				NewWingID,
				OldStarLevel,
				NewStarLevel,
				OldStarExp,
				NewStarExp,
				strCostList
			});
		}

		
		public static void AddLingYuLevelEvent(GameClient client, int UpLevMode, int OldLevel, int CurrentSuit, int NewLevel, string strCostList)
		{
			EventLogManager.SystemRoleEvents[24].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				UpLevMode,
				OldLevel,
				CurrentSuit,
				NewLevel,
				strCostList
			});
		}

		
		public static void AddLingYuSuitEvent(GameClient client, int UpMode, int OldSuit, int NewSuit, int OldLev, int NewLev, string strCostList)
		{
			EventLogManager.SystemRoleEvents[25].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				UpMode,
				OldSuit,
				NewSuit,
				OldLev,
				NewLev,
				strCostList
			});
		}

		
		public static void AddChengJiuAwardEvent(GameClient client, int chengJiuID, string strResList)
		{
			EventLogManager.SystemRoleEvents[28].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				chengJiuID,
				strResList
			});
		}

		
		public static void AddAchievementRuneEvent(GameClient client, int RuneID, string AddProps, string strCostList)
		{
			EventLogManager.SystemRoleEvents[46].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				RuneID,
				AddProps,
				strCostList
			});
		}

		
		public static void AddBossDiedEvent(GameClient client, int monsterID, int mapCode, long birthTick, long deathTick, string drop)
		{
			EventLogManager.SystemRoleEvents[16].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				monsterID,
				mapCode,
				birthTick,
				deathTick,
				drop
			});
		}

		
		public static void AddChangeOccupationEvent(GameClient client, int targetOccupation)
		{
			string stringOccList = "";
			foreach (int occ in client.ClientData.OccupationList)
			{
				stringOccList += string.Format("{0}:", occ);
			}
			if (!string.IsNullOrEmpty(stringOccList))
			{
				stringOccList = stringOccList.Substring(0, stringOccList.Length - 1);
			}
			EventLogManager.SystemRoleEvents[47].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				client.ClientData.Occupation,
				targetOccupation,
				stringOccList
			});
		}

		
		public static void AddRankingEvent(PaiHangTypes type, int ranking, string strText)
		{
			EventLogManager.SystemRoleEvents[20].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				(int)type,
				ranking,
				strText
			});
		}

		
		public static void AddJieriCZSongEvent(GameClient client, int awardId, string goods)
		{
			EventLogManager.SystemRoleEvents[21].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				awardId,
				goods
			});
		}

		
		public static void AddJieRiMeiRiLeiJiEvent(GameClient client, int awardId, string goods)
		{
			EventLogManager.SystemRoleEvents[22].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				awardId,
				goods
			});
		}

		
		public static void AddJieriLeiJiCZEvent(GameClient client, int awardId, string goods)
		{
			EventLogManager.SystemRoleEvents[23].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				awardId,
				goods
			});
		}

		
		public static void AddGameEvent(LogRecordType logRecordType, params object[] args)
		{
			try
			{
				if (GameManager.FlagEnableGameEventLog)
				{
					int serverId = GameManager.ServerId;
					string eventMsg = string.Format("{0}\t{1}", (int)logRecordType, serverId);
					if (logRecordType == LogRecordType.Json)
					{
						if (args[0].GetType() == typeof(Hashtable))
						{
							eventMsg = eventMsg + "\t" + MUJson.jsonEncode(args[0]);
						}
						else
						{
							Hashtable table = new Hashtable();
							for (int i = 0; i < args.Length - 1; i += 2)
							{
								table.Add(args[i], args[1]);
							}
							eventMsg = eventMsg + "\t" + MUJson.jsonEncode(table);
						}
					}
					else
					{
						foreach (object arg in args)
						{
							eventMsg = eventMsg + "\t" + arg.ToString();
						}
					}
					EventLogManager.SystemRoleEvents[75].AddEvent(eventMsg, EventLevels.Important);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public static void AddRoleEvent(GameClient client, OpTypes optType, OpTags optTag, LogRecordType logRecordType, params object[] args)
		{
			try
			{
				if (GameManager.FlagEnableOperatorEventLog)
				{
					string eventMsg = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", new object[]
					{
						(int)logRecordType,
						client.ServerId,
						client.ClientData.ZoneID,
						client.strUserID,
						client.ClientData.RoleID,
						(int)optType,
						(int)optTag
					});
					foreach (object arg in args)
					{
						eventMsg = eventMsg + "\t" + arg;
					}
					EventLogManager.SystemRoleEvents[76].AddEvent(eventMsg, EventLevels.Important);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public static void AddRoleSkillEvent(GameClient client, SkillLogTypes optType, LogRecordType logRecordType, params object[] args)
		{
			try
			{
				if (GameManager.FlagEnableRoleSkillLog)
				{
					string eventMsg = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", new object[]
					{
						(int)logRecordType,
						client.ServerId,
						client.strUserID,
						client.ClientData.RoleID,
						(int)optType
					});
					foreach (object arg in args)
					{
						eventMsg = eventMsg + "\t" + arg;
					}
					EventLogManager.SystemRoleEvents[77].AddEvent(eventMsg, EventLevels.Important);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public static void AddPetSkillEvent(GameClient client, LogRecordType logRecordType, EPetSkillLog logType, params object[] args)
		{
			try
			{
				if (GameManager.FlagEnablePetSkillLog)
				{
					string eventMsg = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", new object[]
					{
						(int)logRecordType,
						client.ServerId,
						client.ClientData.ZoneID,
						client.strUserID,
						client.ClientData.RoleID,
						(int)logType
					});
					foreach (object arg in args)
					{
						eventMsg = eventMsg + "\t" + arg;
					}
					EventLogManager.SystemRoleEvents[78].AddEvent(eventMsg, EventLevels.Important);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public static void AddUnionPalaceEvent(GameClient client, LogRecordType logRecordType, params object[] args)
		{
			try
			{
				if (GameManager.FlagEnableUnionPalaceLog)
				{
					string eventMsg = string.Format("{0}\t{1}\t{2}\t{3}", new object[]
					{
						(int)logRecordType,
						client.ServerId,
						client.strUserID,
						client.ClientData.RoleID
					});
					foreach (object arg in args)
					{
						eventMsg = eventMsg + "\t" + arg;
					}
					EventLogManager.SystemRoleEvents[79].AddEvent(eventMsg, EventLevels.Important);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
		}

		
		public static string CreateAwardListStr(ResLogType type, long addData, long oldData, long clientData, ref bool isBegin)
		{
			string result;
			if (isBegin)
			{
				isBegin = false;
				result = EventLogManager.NewResPropString(type, new object[]
				{
					addData,
					oldData,
					clientData
				});
			}
			else
			{
				result = EventLogManager.AddResPropString(type, new object[]
				{
					addData,
					oldData,
					clientData
				});
			}
			return result;
		}

		
		public static string NewResPropString(ResLogType type, params object[] list)
		{
			string strProp = string.Format("{0}", (int)type);
			for (int i = 0; i < list.Length; i++)
			{
				strProp += string.Format(":{0}", list[i]);
			}
			return strProp;
		}

		
		public static string AddResPropString(ResLogType type, params object[] list)
		{
			return "@" + EventLogManager.NewResPropString(type, list);
		}

		
		public static string AddGoodsDataPropString(GoodsData goodsData)
		{
			return "@" + EventLogManager.NewGoodsDataPropString(goodsData);
		}

		
		public static string AddGoodsDataPropString(List<GoodsData> goodsDataList)
		{
			string result = EventLogManager.MakeGoodsDataPropString(goodsDataList);
			return string.IsNullOrEmpty(result) ? "" : ("@" + result);
		}

		
		public static string NewGoodsDataPropString(GoodsData goodsData)
		{
			string result;
			if (null == goodsData)
			{
				result = "";
			}
			else
			{
				result = string.Format("{0}:{1}_{2}_{3}_{4}_{5}_{6}", new object[]
				{
					0,
					goodsData.Id,
					goodsData.GoodsID,
					goodsData.Site,
					goodsData.GCount,
					goodsData.Binding,
					goodsData.ExcellenceInfo
				});
			}
			return result;
		}

		
		public static string MakeGoodsDataPropString(List<GoodsData> goodsDataList)
		{
			string strGoodsList = "";
			if (goodsDataList != null)
			{
				foreach (GoodsData item in goodsDataList)
				{
					strGoodsList += EventLogManager.NewGoodsDataPropString(item);
					strGoodsList += "@";
				}
				if (strGoodsList.Length > 0)
				{
					strGoodsList = strGoodsList.Remove(strGoodsList.Length - 1);
				}
			}
			return strGoodsList;
		}

		
		public static string AddResPropString(string res, ResLogType type, params object[] list)
		{
			string result;
			if (string.IsNullOrEmpty(res))
			{
				result = EventLogManager.NewResPropString(type, list);
			}
			else
			{
				result = res + "@" + EventLogManager.AddResPropString(type, list);
			}
			return result;
		}

		
		public static string AddGoodsDataPropString(string res, List<GoodsData> goodsDataList)
		{
			string result = EventLogManager.MakeGoodsDataPropString(goodsDataList);
			string result2;
			if (string.IsNullOrEmpty(res))
			{
				result2 = result;
			}
			else
			{
				result2 = res + "@" + result;
			}
			return result2;
		}

		
		public static string AddGoodsDataPropString(string res, GoodsData goodsData)
		{
			string result;
			if (null == goodsData)
			{
				result = res;
			}
			else
			{
				result = res + string.Format("@{0}:{1}_{2}_{3}_{4}_{5}_{6}", new object[]
				{
					0,
					goodsData.Id,
					goodsData.GoodsID,
					goodsData.Site,
					goodsData.GCount,
					goodsData.Binding,
					goodsData.ExcellenceInfo
				});
			}
			return result;
		}

		
		public static void AddHuiJiEvent(GameClient client, int type, int mode, int AddExp, int OldLevel, int OldStar, int NewLevel, int NewStar, int NewExp, string strCostList)
		{
			EventLogManager.SystemRoleEvents[48].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				type,
				mode,
				AddExp,
				OldLevel,
				OldStar,
				NewLevel,
				NewStar,
				NewExp,
				strCostList
			});
		}

		
		public static void AddRoleMeditateEvent(GameClient client, long meditateTime, int totalMeditateCnt, string goodStr)
		{
			EventLogManager.SystemRoleEvents[49].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				meditateTime,
				totalMeditateCnt,
				goodStr
			});
		}

		
		public static void AddRoleQiFuEvent(GameClient client, string format, params object[] args)
		{
			try
			{
				EventLogManager.SystemRoleEvents[50].AddImporEvent(new object[]
				{
					GameManager.PlatformType,
					client.ClientData.ZoneID,
					client.ClientData.RoleID,
					string.Format(format, args)
				});
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		
		public static void AddArmorEvent(GameClient client, int type, int mode, int AddExp, int OldLevel, int OldStar, int NewLevel, int NewStar, int NewExp, string strCostList)
		{
			EventLogManager.SystemRoleEvents[51].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				type,
				mode,
				AddExp,
				OldLevel,
				OldStar,
				NewLevel,
				NewStar,
				NewExp,
				strCostList
			});
		}

		
		public static void AddBianShenEvent(GameClient client, int type, int mode, int AddExp, int OldLevel, int NewLevel, int NewExp, string strCostList)
		{
			EventLogManager.SystemRoleEvents[52].AddImporEvent(new object[]
			{
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				type,
				mode,
				AddExp,
				OldLevel,
				NewLevel,
				NewExp,
				strCostList
			});
		}

		
		public static void AddCreateZhanDuiEvent(GameClient client, long teamID, string teamName, string costStr)
		{
			EventLogManager.SystemRoleEvents[55].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				teamID,
				teamName,
				costStr
			});
		}

		
		public static void AddAttendZhanDuiEvent(GameClient client, long teamID, string teamName, int leaderRid, string teamRoles)
		{
			EventLogManager.SystemRoleEvents[56].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				teamID,
				teamName,
				leaderRid,
				teamRoles
			});
		}

		
		public static void QuitZhanDuiEvent(GameClient client, long teamID, string teamName, int leaderRid, string teamRoles)
		{
			EventLogManager.SystemRoleEvents[57].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				teamID,
				teamName,
				leaderRid,
				teamRoles
			});
		}

		
		public static void DeleteZhanDuiEvent(GameClient client, long teamID, string teamName, int leaderRid, string teamRoles)
		{
			EventLogManager.SystemRoleEvents[58].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				teamID,
				teamName,
				leaderRid,
				teamRoles
			});
		}

		
		public static void AddKF5V5FreeBattleEvent(int rid, int roleserverid, int winorfalse, long gameid, int side, int online, int oldTodayAttendCnt, int todayAttendCnt, int oldMonthAttendCnt, int monthAttendCnt, int oldKillCnt, int killCnt, int oldLinasheng, int liansheng, int oldjingji, int jingji, int oldwincnt, int wincnt, string awardStr)
		{
			EventLogManager.SystemRoleEvents[68].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				rid,
				roleserverid,
				winorfalse,
				gameid,
				side,
				online,
				oldTodayAttendCnt,
				todayAttendCnt,
				oldMonthAttendCnt,
				monthAttendCnt,
				oldKillCnt,
				killCnt,
				oldLinasheng,
				liansheng,
				oldjingji,
				jingji,
				oldwincnt,
				wincnt,
				awardStr
			});
		}

		
		public static void AddMonthChengJiuAwardEvent(int rid, int serverid, int type, int oldval, int newval, int oldawarID, int awardid, string awardStr)
		{
			EventLogManager.SystemRoleEvents[69].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				rid,
				serverid,
				type,
				awardid,
				oldval,
				newval,
				awardStr
			});
		}

		
		public static void ChangeZhanDuiLeaderEvent(GameClient client, long teamID, string teamName, int oldLeaderRid, int newLeaderRid, string teamRoles)
		{
			EventLogManager.SystemRoleEvents[59].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				teamID,
				teamName,
				oldLeaderRid,
				newLeaderRid,
				teamRoles
			});
		}

		
		public static void AddKF5v5SendMailEvent(int NextRound, int myRound, int oldSendMailFlag, int mailFlag, string teamName, int n64QiangID, long teamID, string subject, string content, string roles, int oppsite64QiangID, long oppsiteTeamID, string oppsiteTeamName)
		{
			EventLogManager.SystemRoleEvents[70].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				NextRound,
				myRound,
				teamName,
				n64QiangID,
				teamID,
				subject,
				content,
				roles,
				oppsite64QiangID,
				oppsiteTeamID,
				oppsiteTeamName
			});
		}

		
		public static void AddRoleYaZhuEvent(GameClient client, long teamid, string teamName, int n64QiangID, int round, string costStr)
		{
			EventLogManager.SystemRoleEvents[60].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				teamid,
				teamName,
				n64QiangID,
				round,
				costStr
			});
		}

		
		public static void AddRoleYaZhuResultEvent(string userID, int zoneID, int rid, long teamid, string teamName, int n64QiangID, int yaZhuRound, int n64QiangRound, int awawrdFlag, string awardStr)
		{
			EventLogManager.SystemRoleEvents[61].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				userID,
				zoneID,
				rid,
				teamid,
				teamName,
				n64QiangID,
				yaZhuRound,
				n64QiangRound,
				awawrdFlag,
				awardStr
			});
		}

		
		public static void Add5v5PiPeiSuccessEvent(GameClient client, int kfServerID, string toIP, int toPort, long gameID)
		{
			EventLogManager.SystemRoleEvents[62].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				kfServerID,
				toIP,
				toPort,
				gameID
			});
		}

		
		public static void Add5v5Enter64QiangEvent(GameClient client, int kfServerID, string toIP, int toPort, long gameID, long teamid, int n64QiangID, int nRound, int ncurrentRound, int yazhuRound)
		{
			EventLogManager.SystemRoleEvents[63].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				kfServerID,
				toIP,
				toPort,
				gameID,
				teamid,
				n64QiangID,
				nRound,
				ncurrentRound,
				yazhuRound
			});
		}

		
		public static void Add5v5DailyAwardEvent(GameClient client, int rank, int attendcnt, int killcnt, int wincnt, int oldFlag, int awardFlag, string awardStr)
		{
			EventLogManager.SystemRoleEvents[64].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				rank,
				attendcnt,
				killcnt,
				wincnt,
				oldFlag,
				awardFlag,
				awardStr
			});
		}

		
		public static void Add5v5MonthAwardEvent(string usrID, int zoneid, int rid, int rank, int attendcnt, int killcnt, int wincnt, int awardFlag, string awardStr)
		{
			EventLogManager.SystemRoleEvents[65].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				usrID,
				zoneid,
				rid,
				rank,
				attendcnt,
				killcnt,
				wincnt,
				awardStr
			});
		}

		
		public static void Add5v5Notify64QiangEvent(GameClient client, long teamid, int n64QinagID, int round, int nCurrentRoud, int nYaZhuRound)
		{
			EventLogManager.SystemRoleEvents[66].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				teamid,
				n64QinagID,
				round,
				nCurrentRoud,
				nYaZhuRound
			});
		}

		
		public static void Add5v5JinJi64QiangAwardEvent(string usrID, int zoneID, int rid, long teamid, int n64QinagID, int round, string awardStr)
		{
			EventLogManager.SystemRoleEvents[67].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				usrID,
				zoneID,
				rid,
				teamid,
				n64QinagID,
				round,
				awardStr
			});
		}

		
		public static void AddMakeOldPlayerEvent(GameClient client)
		{
			EventLogManager.SystemRoleEvents[71].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				Global.GetUnionLevel(client, false),
				client.ClientData.VipLevel
			});
		}

		
		public static void AddGetOldPlayerAwardEvent(GameClient client, int type, int param, int oldLevel, string strCostList)
		{
			EventLogManager.SystemRoleEvents[72].AddImporEvent(new object[]
			{
				TimeUtil.NowDateTime(),
				GameManager.ServerId,
				client.strUserID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID,
				type,
				param,
				oldLevel,
				strCostList
			});
		}

		
		private const string NA = "-1";

		
		private static long _LogId;

		
		public static ServerEvents[] SystemRoleEvents = new ServerEvents[82];
	}
}
