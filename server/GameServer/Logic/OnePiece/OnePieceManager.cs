using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.OnePiece
{
	
	public class OnePieceManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static OnePieceManager getInstance()
		{
			return OnePieceManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1600, 1, 1, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1601, 1, 1, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1605, 1, 1, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1602, 1, 1, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1604, 1, 1, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1606, 2, 2, OnePieceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.OnePieceTreasure, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(506, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot9))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(507, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1600:
					return this.ProcessOnePieceGetInfoCmd(client, nID, bytes, cmdParams);
				case 1601:
					return this.ProcessOnePieceRollCmd(client, nID, bytes, cmdParams);
				case 1602:
					return this.ProcessOnePieceTriggerEventCmd(client, nID, bytes, cmdParams);
				case 1604:
					return this.ProcessOnePieceMoveCmd(client, nID, bytes, cmdParams);
				case 1605:
					return this.ProcessOnePieceRollMiracleCmd(client, nID, bytes, cmdParams);
				case 1606:
					return this.ProcessOnePieceDiceBuyCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		
		public void GetOnePieceTreasureData(GameClient client, OnePieceTreasureData myTreasureData)
		{
			int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
			DateTime now = TimeUtil.NowDateTime();
			int rollnum = 0;
			int rollnumMiracle = 0;
			int posid = this.GenPosID(1, 0);
			int eventid = 0;
			string strFlag = "TreasureData";
			string OnePieceTreasureDataFlag = Global.GetRoleParamByName(client, strFlag);
			if (null != OnePieceTreasureDataFlag)
			{
				string[] fields = OnePieceTreasureDataFlag.Split(new char[]
				{
					','
				});
				if (5 == fields.Length)
				{
					int lastday = Convert.ToInt32(fields[0]);
					rollnum = Convert.ToInt32(fields[1]);
					rollnumMiracle = Convert.ToInt32(fields[2]);
					posid = Convert.ToInt32(fields[3]);
					eventid = Convert.ToInt32(fields[4]);
				}
			}
			myTreasureData.PosID = posid;
			myTreasureData.EventID = eventid;
			myTreasureData.RollNumNormal = rollnum;
			myTreasureData.RollNumMiracle = rollnumMiracle;
			string resetTime = now.ToString("yyyy-MM-dd");
			DateTime resetDateTm;
			if (DateTime.TryParse(resetTime, out resetDateTm))
			{
				int spanday = (int)(DayOfWeek.Monday - now.DayOfWeek);
				spanday = ((spanday <= 0) ? (7 + spanday) : spanday);
				resetDateTm = resetDateTm.AddDays((double)spanday);
				myTreasureData.ResetPosTicks = TimeUtil.TimeDiff(resetDateTm.Ticks, now.Ticks);
			}
		}

		
		public void JudgeResetOnePieceTreasureData(GameClient client)
		{
			if (client.ClientData.OnePieceMoveLeft == 0)
			{
				int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
				DateTime now = TimeUtil.NowDateTime();
				string strFlag = "TreasureData";
				string OnePieceTreasureDataFlag = Global.GetRoleParamByName(client, strFlag);
				int lastday = 0;
				if (null != OnePieceTreasureDataFlag)
				{
					string[] fields = OnePieceTreasureDataFlag.Split(new char[]
					{
						','
					});
					if (5 == fields.Length)
					{
						lastday = Convert.ToInt32(fields[0]);
					}
				}
				OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, myOnePieceData);
				int daydis = now.DayOfWeek - DayOfWeek.Monday;
				daydis = ((daydis >= 0) ? (daydis + 1) : 7);
				if (lastday != 0 && currday - lastday >= daydis)
				{
					myOnePieceData.PosID = this.ResetRolePos(client);
					myOnePieceData.EventID = 0;
				}
				if (currday != lastday && currday > lastday)
				{
					this.HandleDicePassDay(client, currday, lastday, myOnePieceData);
					lastday = currday;
				}
				string result = string.Format("{0},{1},{2},{3},{4}", new object[]
				{
					lastday,
					myOnePieceData.RollNumNormal,
					myOnePieceData.RollNumMiracle,
					myOnePieceData.PosID,
					myOnePieceData.EventID
				});
				Global.SaveRoleParamsStringToDB(client, strFlag, result, true);
			}
		}

		
		public void HandleDicePassDay(GameClient client, int currday, int lastday, OnePieceTreasureData myOnePieceData)
		{
			int passday = 1;
			if (lastday != 0)
			{
				passday = currday - lastday;
			}
			if (passday > 0)
			{
				this.ModifyOnePieceDice(client, myOnePieceData, 0, this.SystemParamsTreasureFreeNum * passday);
				this.ModifyOnePieceDice(client, myOnePieceData, 1, this.SystemParamsTreasureMiracleNum * passday);
			}
		}

		
		public int ResetRolePos(GameClient client)
		{
			int posid = this.GenPosID(1, 0);
			this.TryGiveOnePieceBoxListAward(client);
			string strcmd = string.Format("{0}:{1}", 13, posid);
			client.sendCmd(1604, strcmd, false);
			return posid;
		}

		
		public void ModifyOnePieceTreasureData(GameClient client, OnePieceTreasureData myOnePieceData)
		{
			int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
			DateTime now = TimeUtil.NowDateTime();
			string strFlag = "TreasureData";
			string OnePieceTreasureDataFlag = Global.GetRoleParamByName(client, strFlag);
			int lastday = currday;
			if (null != OnePieceTreasureDataFlag)
			{
				string[] fields = OnePieceTreasureDataFlag.Split(new char[]
				{
					','
				});
				if (5 == fields.Length)
				{
					lastday = Convert.ToInt32(fields[0]);
				}
			}
			string result = string.Format("{0},{1},{2},{3},{4}", new object[]
			{
				lastday,
				myOnePieceData.RollNumNormal,
				myOnePieceData.RollNumMiracle,
				myOnePieceData.PosID,
				myOnePieceData.EventID
			});
			Global.SaveRoleParamsStringToDB(client, strFlag, result, true);
		}

		
		public int GenPosID(int floor, int cell)
		{
			return 1000 * floor + cell;
		}

		
		public int GetFloorByPosID(int posid)
		{
			return posid / 1000;
		}

		
		public int FilterPosIDChangeFloor(GameClient client, int posid)
		{
			int result;
			if (client.ClientData.OnePieceMoveDir == 0)
			{
				result = posid;
			}
			else if (this.IfHaveOnePieceBoxListAward(client))
			{
				result = posid;
			}
			else
			{
				bool foward = client.ClientData.OnePieceMoveDir > 0;
				if (posid % 1000 != 0 && posid % 1000 != 30)
				{
					result = posid;
				}
				else
				{
					int FilterPosID;
					if (foward)
					{
						FilterPosID = (this.GetFloorByPosID(posid) + 1) * 1000;
					}
					else
					{
						FilterPosID = posid - 1;
					}
					OnePieceTreasureMapConfig myTreasureMapConfig = null;
					if (!this.TreasureMapConfig.TryGetValue(FilterPosID, out myTreasureMapConfig))
					{
						FilterPosID = this.GenPosID(1, 0);
					}
					result = FilterPosID;
				}
			}
			return result;
		}

		
		public int GetNextPosIDForEvent(int posid, bool foward = true)
		{
			int nextPosID;
			if (foward)
			{
				nextPosID = posid + 1;
			}
			else
			{
				nextPosID = posid - 1;
			}
			OnePieceTreasureMapConfig myTreasureMapConfig = null;
			int result;
			if (this.TreasureMapConfig.TryGetValue(nextPosID, out myTreasureMapConfig) && posid % 1000 != 30 && nextPosID % 1000 != 0)
			{
				result = nextPosID;
			}
			else
			{
				if (foward)
				{
					nextPosID = (this.GetFloorByPosID(posid) + 1) * 1000 + 1;
					if (!this.TreasureMapConfig.TryGetValue(nextPosID, out myTreasureMapConfig))
					{
						nextPosID = this.GenPosID(1, 1);
					}
				}
				else
				{
					nextPosID = (this.GetFloorByPosID(posid) - 1) * 1000 + 30;
					if (!this.TreasureMapConfig.TryGetValue(nextPosID, out myTreasureMapConfig))
					{
						nextPosID = this.GenPosID(1, 0);
					}
				}
				result = nextPosID;
			}
			return result;
		}

		
		public int RollMoveNum()
		{
			int move = 0;
			int result;
			if (this.OnePiece_FakeRollNum_GM != 0)
			{
				move = this.OnePiece_FakeRollNum_GM;
				result = move;
			}
			else
			{
				double rate = (double)Global.GetRandomNumber(1, 101) / 100.0;
				double rateend = 0.0;
				for (int i = 0; i < this.SystemParamsTreasureDice.Count; i++)
				{
					rateend += this.SystemParamsTreasureDice[i];
					if (rate <= rateend)
					{
						move = i + 1;
						break;
					}
				}
				result = move;
			}
			return result;
		}

		
		public int RandomTreasureEvent(List<OnePieceRandomEvent> LisRandomEvent)
		{
			int EventID = 0;
			int result;
			if (LisRandomEvent == null || LisRandomEvent.Count == 0)
			{
				result = EventID;
			}
			else
			{
				double rate = (double)Global.GetRandomNumber(1, 101) / 100.0;
				double rateend = 0.0;
				for (int i = 0; i < LisRandomEvent.Count; i++)
				{
					rateend += LisRandomEvent[i].Rate;
					if (rate <= rateend)
					{
						EventID = LisRandomEvent[i].EventID;
						break;
					}
				}
				result = EventID;
			}
			return result;
		}

		
		public void SyncOnePieceEvent(GameClient client, int EventID, int EventValue = 0, int ErrCode = 0, List<int> BoxIDList = null)
		{
			OnePieceTreasureEvent myTreasureEvent = new OnePieceTreasureEvent
			{
				EventID = EventID,
				EventValue = EventValue,
				BoxIDList = BoxIDList,
				ErrCode = ErrCode
			};
			byte[] bytesData = DataHelper.ObjectToBytes<OnePieceTreasureEvent>(myTreasureEvent);
			GameManager.ClientMgr.SendToClient(client, bytesData, 1603);
		}

		
		public void ModifyOnePieceDice(GameClient client, OnePieceTreasureData myOnePieceData, int diceType, int num)
		{
			bool RollNumReachMax = false;
			int oldNum;
			int newNum;
			if (diceType == 0)
			{
				oldNum = myOnePieceData.RollNumNormal;
				myOnePieceData.RollNumNormal += num;
				if (myOnePieceData.RollNumNormal > 99)
				{
					myOnePieceData.RollNumNormal = 99;
					RollNumReachMax = true;
				}
				newNum = myOnePieceData.RollNumNormal;
			}
			else
			{
				if (diceType != 1)
				{
					return;
				}
				oldNum = myOnePieceData.RollNumMiracle;
				myOnePieceData.RollNumMiracle += num;
				if (myOnePieceData.RollNumMiracle > 99)
				{
					myOnePieceData.RollNumMiracle = 99;
					RollNumReachMax = true;
				}
				newNum = myOnePieceData.RollNumMiracle;
			}
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				0,
				diceType,
				newNum,
				oldNum
			});
			client.sendCmd(1607, strcmd, false);
			if (RollNumReachMax)
			{
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					14,
					diceType,
					newNum,
					oldNum
				});
				client.sendCmd(1607, strcmd, false);
			}
		}

		
		public int GiveCopyMapGift(GameClient client, int fuBenID)
		{
			OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
			this.GetOnePieceTreasureData(client, myOnePieceData);
			int result;
			if (client.ClientData.OnePieceTempEventID == 0)
			{
				result = 0;
			}
			else
			{
				OnePieceTreasureEventConfig myTreasureEventConfig = null;
				if (!this.TreasureEventConfig.TryGetValue(client.ClientData.OnePieceTempEventID, out myTreasureEventConfig))
				{
					result = 0;
				}
				else if (myTreasureEventConfig.FuBenID != fuBenID)
				{
					result = 0;
				}
				else
				{
					int EventID = client.ClientData.OnePieceTempEventID;
					this.GiveOnePieceEventAward(client, myOnePieceData, myTreasureEventConfig);
					client.ClientData.OnePieceTempEventID = 0;
					result = EventID;
				}
			}
			return result;
		}

		
		public OnePieceTreasureErrorCode GiveOnePieceEventAward(GameClient client, OnePieceTreasureData myOnePieceData, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			OnePieceTreasureErrorCode ret = OnePieceTreasureErrorCode.OnePiece_Success;
			List<GoodsData> goodsDataList = Global.ConvertToGoodsDataList(myTreasureEventConfig.GoodsList.Items, -1);
			if (!Global.CanAddGoodsDataList(client, goodsDataList))
			{
				if (myTreasureEventConfig.Type == TreasureEventType.ETET_Excharge)
				{
					return OnePieceTreasureErrorCode.OnePiece_ErrorBagNotEnough;
				}
				foreach (GoodsData item in goodsDataList)
				{
					Global.UseMailGivePlayerAward(client, item, GLang.GetLang(508, new object[0]), GLang.GetLang(508, new object[0]), 1.0);
				}
				ret = OnePieceTreasureErrorCode.OnePiece_ErrorCheckMail;
			}
			else
			{
				for (int i = 0; i < goodsDataList.Count; i++)
				{
					GoodsData goodsData = goodsDataList[i];
					if (null != goodsData)
					{
						goodsData.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "获得藏宝秘境奖励", goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, 0, 0, 0, null, null, 0, true);
					}
				}
			}
			if (myTreasureEventConfig.NewValue.Type != MoneyTypes.None)
			{
				int type = (int)myTreasureEventConfig.NewValue.Type;
				if (type <= 8)
				{
					if (type != 1)
					{
						if (type == 8)
						{
							GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myTreasureEventConfig.NewValue.Num, "获得藏宝秘境奖励", false);
						}
					}
					else
					{
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myTreasureEventConfig.NewValue.Num, "获得藏宝秘境奖励", false);
					}
				}
				else if (type != 40)
				{
					if (type != 50)
					{
						switch (type)
						{
						case 110:
							GameManager.ClientMgr.ModifyTreasureJiFenValue(client, myTreasureEventConfig.NewValue.Num, "获得藏宝秘境奖励", true);
							break;
						case 111:
							GameManager.ClientMgr.ModifyTreasureXueZuanValue(client, myTreasureEventConfig.NewValue.Num, true, true);
							break;
						}
					}
					else
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myTreasureEventConfig.NewValue.Num, "获得藏宝秘境奖励");
					}
				}
				else
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myTreasureEventConfig.NewValue.Num, "获得藏宝秘境奖励", ActivityTypes.None, "");
				}
			}
			if (myTreasureEventConfig.NewDiec > 0)
			{
				this.ModifyOnePieceDice(client, myOnePieceData, 0, myTreasureEventConfig.NewDiec);
			}
			if (myTreasureEventConfig.NewSuperDiec > 0)
			{
				this.ModifyOnePieceDice(client, myOnePieceData, 1, myTreasureEventConfig.NewSuperDiec);
			}
			return ret;
		}

		
		public OnePieceTreasureErrorCode TriggerEventAward(GameClient client, OnePieceTreasureData myOnePieceData, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			OnePieceTreasureErrorCode ret = this.GiveOnePieceEventAward(client, myOnePieceData, myTreasureEventConfig);
			OnePieceTreasureErrorCode result;
			if (ret != OnePieceTreasureErrorCode.OnePiece_Success && ret != OnePieceTreasureErrorCode.OnePiece_ErrorCheckMail)
			{
				result = ret;
			}
			else
			{
				this.SyncOnePieceEvent(client, myTreasureEventConfig.ID, 0, (int)ret, null);
				result = OnePieceTreasureErrorCode.OnePiece_Success;
			}
			return result;
		}

		
		public OnePieceTreasureErrorCode TriggerEventExcharge(GameClient client, OnePieceTreasureData myOnePieceData, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			int i = 0;
			while (i < myTreasureEventConfig.NeedGoods.Count)
			{
				SystemXmlItem needGoods = null;
				OnePieceTreasureErrorCode result;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(myTreasureEventConfig.NeedGoods[i]._NeedGoodsID, out needGoods))
				{
					result = OnePieceTreasureErrorCode.OnePiece_ErrorNeedGoodsID;
				}
				else if (myTreasureEventConfig.NeedGoods[i]._NeedGoodsCount <= 0)
				{
					result = OnePieceTreasureErrorCode.OnePiece_ErrorNeedGoodsCount;
				}
				else
				{
					int nTotalGoodsCount = Global.GetTotalGoodsCountByID(client, myTreasureEventConfig.NeedGoods[i]._NeedGoodsID);
					if (nTotalGoodsCount >= myTreasureEventConfig.NeedGoods[i]._NeedGoodsCount)
					{
						i++;
						continue;
					}
					result = OnePieceTreasureErrorCode.OnePiece_ErrorGoodsNotEnough;
				}
				return result;
			}
			if (0 > Global.IsRoleHasEnoughMoney(client, myTreasureEventConfig.NeedValue.Num, (int)myTreasureEventConfig.NeedValue.Type))
			{
				return OnePieceTreasureErrorCode.OnePiece_ErrorNeedMoneyNotEnough;
			}
			OnePieceTreasureErrorCode ret = this.GiveOnePieceEventAward(client, myOnePieceData, myTreasureEventConfig);
			if (OnePieceTreasureErrorCode.OnePiece_Success != ret)
			{
				return ret;
			}
			for (i = 0; i < myTreasureEventConfig.NeedGoods.Count; i++)
			{
				bool usedBinding = false;
				bool usedTimeLimited = false;
				GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myTreasureEventConfig.NeedGoods[i]._NeedGoodsID, myTreasureEventConfig.NeedGoods[i]._NeedGoodsCount, false, out usedBinding, out usedTimeLimited, false);
			}
			Global.SubRoleMoneyForGoods(client, myTreasureEventConfig.NeedValue.Num, (int)myTreasureEventConfig.NeedValue.Type, "藏宝秘境");
			return OnePieceTreasureErrorCode.OnePiece_Success;
		}

		
		public OnePieceTreasureErrorCode TriggerEventMove(GameClient client, OnePieceTreasureData myOnePieceData, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			OnePieceTreasureErrorCode result;
			if (myTreasureEventConfig.MoveRange == null || myTreasureEventConfig.MoveRange.Count == 0)
			{
				result = OnePieceTreasureErrorCode.OnePiece_ErrorMoveRange;
			}
			else
			{
				int randIndex = Global.GetRandomNumber(0, myTreasureEventConfig.MoveRange.Count);
				int move = myTreasureEventConfig.MoveRange[randIndex];
				client.ClientData.OnePieceMoveLeft = move;
				client.ClientData.OnePieceMoveDir = move;
				this.SyncOnePieceEvent(client, myTreasureEventConfig.ID, move, 0, null);
				result = OnePieceTreasureErrorCode.OnePiece_Success;
			}
			return result;
		}

		
		public OnePieceTreasureErrorCode TriggerEventCombat(GameClient client, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			SystemXmlItem systemFuBenItem = null;
			OnePieceTreasureErrorCode result;
			if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(myTreasureEventConfig.FuBenID, out systemFuBenItem))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(509, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = OnePieceTreasureErrorCode.OnePiece_DBFailed;
			}
			else
			{
				int toMapCode = systemFuBenItem.GetIntValue("MapCode", -1);
				string[] dbFields = Global.ExecuteDBCmd(10049, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
				if (dbFields == null || dbFields.Length < 2)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(510, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = OnePieceTreasureErrorCode.OnePiece_DBFailed;
				}
				else
				{
					int fuBenSeqID = Global.SafeConvertToInt32(dbFields[1]);
					Global.UpdateFuBenData(client, myTreasureEventConfig.FuBenID, 1, 0);
					GameMap gameMap = null;
					if (!GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(511, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = OnePieceTreasureErrorCode.OnePiece_DBFailed;
					}
					else
					{
						client.ClientData.FuBenSeqID = fuBenSeqID;
						client.ClientData.FuBenID = myTreasureEventConfig.FuBenID;
						FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, client.ClientData.FuBenSeqID, 0, myTreasureEventConfig.FuBenID);
						GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, -1, -1, -1, 0);
						client.ClientData.OnePieceTempEventID = myTreasureEventConfig.ID;
						result = OnePieceTreasureErrorCode.OnePiece_Success;
					}
				}
			}
			return result;
		}

		
		public int GiveOnePieceBoxAward(GameClient client, OnePieceTreasureBoxConfig myBoxConfig)
		{
			int ret = 0;
			if (myBoxConfig.Type == TeasureBoxType.ETBT_Goods)
			{
				List<GoodsData> goodsDataList = Global.ConvertToGoodsDataList(myBoxConfig.Goods.Items, -1);
				if (!Global.CanAddGoodsDataList(client, goodsDataList))
				{
					foreach (GoodsData item in goodsDataList)
					{
						Global.UseMailGivePlayerAward(client, item, GLang.GetLang(508, new object[0]), GLang.GetLang(508, new object[0]), 1.0);
					}
					ret = 16;
				}
				else
				{
					for (int i = 0; i < goodsDataList.Count; i++)
					{
						GoodsData goodsData = goodsDataList[i];
						if (null != goodsData)
						{
							goodsData.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "获得藏宝秘境奖励", goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, 0, 0, 0, null, null, 0, true);
						}
					}
				}
			}
			else if (myBoxConfig.Type == TeasureBoxType.ETBT_BaoZangJiFen)
			{
				GameManager.ClientMgr.ModifyTreasureJiFenValue(client, myBoxConfig.Num, "获得藏宝秘境奖励", true);
			}
			else if (myBoxConfig.Type == TeasureBoxType.ETBT_BaoZangXueZuan)
			{
				GameManager.ClientMgr.ModifyTreasureXueZuanValue(client, myBoxConfig.Num, true, true);
			}
			return ret;
		}

		
		public int TryGiveOnePieceBoxListAward(GameClient client)
		{
			List<int> OnePieceBoxIDList = client.ClientData.OnePieceBoxIDList;
			int result;
			if (OnePieceBoxIDList == null)
			{
				result = 4;
			}
			else
			{
				int ret = 0;
				for (int i = 0; i < OnePieceBoxIDList.Count; i++)
				{
					int BoxID = OnePieceBoxIDList[i] / 1000;
					int BoxConfigID = OnePieceBoxIDList[i] % 1000;
					List<OnePieceTreasureBoxConfig> myBoxConfigList = null;
					if (this.TreasureBoxConfig.TryGetValue(BoxID, out myBoxConfigList))
					{
						if (BoxConfigID > 0 && BoxConfigID <= myBoxConfigList.Count)
						{
							ret = this.GiveOnePieceBoxAward(client, myBoxConfigList[BoxConfigID - 1]);
						}
					}
				}
				client.ClientData.OnePieceBoxIDList = null;
				result = ret;
			}
			return result;
		}

		
		public OnePieceTreasureErrorCode TriggerEventTreasureBox(GameClient client, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			List<int> BoxIDList = new List<int>();
			for (int i = 0; i < myTreasureEventConfig.BoxList.Count; i++)
			{
				int OpenNum = myTreasureEventConfig.BoxList[i].OpenNum;
				List<OnePieceTreasureBoxConfig> myBoxConfig = null;
				if (this.TreasureBoxConfig.TryGetValue(myTreasureEventConfig.BoxList[i].BoxID, out myBoxConfig))
				{
					for (int loop = 0; loop < OpenNum; loop++)
					{
						int RandRangeMin = myBoxConfig[0].BeginNum;
						int RandRangeMax = myBoxConfig[myBoxConfig.Count - 1].EndNum + 1;
						int randnum = Global.GetRandomNumber(RandRangeMin, RandRangeMax);
						for (int index = 0; index < myBoxConfig.Count; index++)
						{
							if (randnum <= myBoxConfig[index].EndNum)
							{
								int BoxID = myTreasureEventConfig.BoxList[i].BoxID * 1000 + myBoxConfig[index].ID;
								BoxIDList.Add(BoxID);
								break;
							}
						}
					}
				}
			}
			client.ClientData.OnePieceBoxIDList = BoxIDList;
			this.SyncOnePieceEvent(client, myTreasureEventConfig.ID, 0, 0, BoxIDList);
			return OnePieceTreasureErrorCode.OnePiece_Success;
		}

		
		public OnePieceTreasureErrorCode TriggerEvent(GameClient client, OnePieceTreasureData myOnePieceData, OnePieceTreasureEventConfig myTreasureEventConfig)
		{
			OnePieceTreasureErrorCode result;
			switch (myTreasureEventConfig.Type)
			{
			case TreasureEventType.ETET_Award:
				result = this.TriggerEventAward(client, myOnePieceData, myTreasureEventConfig);
				break;
			case TreasureEventType.ETET_Excharge:
				result = this.TriggerEventExcharge(client, myOnePieceData, myTreasureEventConfig);
				break;
			case TreasureEventType.ETET_Move:
				result = this.TriggerEventMove(client, myOnePieceData, myTreasureEventConfig);
				break;
			case TreasureEventType.ETET_Combat:
				result = this.TriggerEventCombat(client, myTreasureEventConfig);
				break;
			case TreasureEventType.ETET_TreasureBox:
				result = this.TriggerEventTreasureBox(client, myTreasureEventConfig);
				break;
			default:
				result = OnePieceTreasureErrorCode.OnePiece_ErrorNotHaveEvent;
				break;
			}
			return result;
		}

		
		public bool OnePieceMoveTrigger(GameClient client, ref OnePieceTreasureData myOnePieceData, OnePieceTreasureMapConfig myTreasureMapConfig, TriggerType Trigger)
		{
			bool result;
			if (Trigger != myTreasureMapConfig.Trigger)
			{
				result = false;
			}
			else
			{
				if (myTreasureMapConfig.Score > 0 && Trigger == TriggerType.ETT_Stay)
				{
					GameManager.ClientMgr.ModifyTreasureJiFenValue(client, myTreasureMapConfig.Score, "获得藏宝秘境奖励", true);
				}
				int EventID = this.RandomTreasureEvent(myTreasureMapConfig.LisRandomEvent);
				OnePieceTreasureEventConfig myTreasureEventConfig = null;
				if (!this.TreasureEventConfig.TryGetValue(EventID, out myTreasureEventConfig))
				{
					result = false;
				}
				else
				{
					if (myTreasureEventConfig.Type == TreasureEventType.ETET_Combat || myTreasureEventConfig.Type == TreasureEventType.ETET_Excharge)
					{
						myOnePieceData.EventID = EventID;
						this.SyncOnePieceEvent(client, myTreasureEventConfig.ID, 0, 0, null);
					}
					else
					{
						this.TriggerEvent(client, myOnePieceData, myTreasureEventConfig);
					}
					result = true;
				}
			}
			return result;
		}

		
		public void HandleRoleLogout(GameClient client)
		{
			if (this.IfCanContinueMove(client))
			{
				OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, myOnePieceData);
				for (int i = 0; i < this.SystemParamsTreasureDice.Count + 1; i++)
				{
					this.TryGiveOnePieceBoxListAward(client);
					this.HandleOnePieceTreasureMove(client, client.ClientData.OnePieceMoveLeft, myOnePieceData);
					if (!this.IfCanContinueMove(client))
					{
						break;
					}
				}
				this.ModifyOnePieceTreasureData(client, myOnePieceData);
			}
		}

		
		public int CalculateMoveCellToNextEvent(GameClient client, int MoveCellNum, OnePieceTreasureData myOnePieceData)
		{
			int result;
			if (MoveCellNum == 0)
			{
				result = myOnePieceData.PosID;
			}
			else
			{
				OnePieceTreasureMapConfig myTreasureMapConfig = null;
				int posid = myOnePieceData.PosID;
				for (int i = 0; i < Math.Abs(MoveCellNum); i++)
				{
					if (MoveCellNum > 0)
					{
						posid = this.GetNextPosIDForEvent(posid, true);
					}
					else
					{
						posid = this.GetNextPosIDForEvent(posid, false);
					}
					if (!this.TreasureMapConfig.TryGetValue(posid, out myTreasureMapConfig))
					{
						break;
					}
					if (myTreasureMapConfig.Trigger == TriggerType.ETT_Pass)
					{
						return posid;
					}
				}
				result = posid;
			}
			return result;
		}

		
		public void HandleOnePieceTreasureMove(GameClient client, int MoveCellNum, OnePieceTreasureData myOnePieceData)
		{
			OnePieceTreasureMapConfig myTreasureMapConfig = null;
			int posid = myOnePieceData.PosID;
			for (int i = 0; i < Math.Abs(MoveCellNum); i++)
			{
				if (MoveCellNum > 0)
				{
					posid = this.GetNextPosIDForEvent(myOnePieceData.PosID, true);
				}
				else
				{
					posid = this.GetNextPosIDForEvent(myOnePieceData.PosID, false);
				}
				if (!this.TreasureMapConfig.TryGetValue(posid, out myTreasureMapConfig))
				{
					break;
				}
				myOnePieceData.PosID = posid;
				if (MoveCellNum > 0)
				{
					client.ClientData.OnePieceMoveLeft--;
				}
				else
				{
					client.ClientData.OnePieceMoveLeft++;
				}
				if (myTreasureMapConfig.Trigger == TriggerType.ETT_Pass)
				{
					break;
				}
			}
			if (myTreasureMapConfig != null && MoveCellNum != 0)
			{
				this.OnePieceMoveTrigger(client, ref myOnePieceData, myTreasureMapConfig, TriggerType.ETT_Pass);
			}
			if (myTreasureMapConfig != null && client.ClientData.OnePieceMoveLeft == 0 && MoveCellNum != 0)
			{
				this.OnePieceMoveTrigger(client, ref myOnePieceData, myTreasureMapConfig, TriggerType.ETT_Stay);
			}
			myOnePieceData.PosID = this.FilterPosIDChangeFloor(client, myOnePieceData.PosID);
		}

		
		public void GM_SetDice(GameClient client, int diceType, int newNum)
		{
			OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
			this.GetOnePieceTreasureData(client, myOnePieceData);
			if (diceType == 0)
			{
				this.ModifyOnePieceDice(client, myOnePieceData, diceType, newNum - myOnePieceData.RollNumNormal);
			}
			else
			{
				if (diceType != 1)
				{
					return;
				}
				this.ModifyOnePieceDice(client, myOnePieceData, diceType, newNum - myOnePieceData.RollNumMiracle);
			}
			this.ModifyOnePieceTreasureData(client, myOnePieceData);
		}

		
		public void GM_SetPosID(GameClient client, int posid)
		{
			OnePieceTreasureMapConfig myTreasureMapConfig = null;
			if (this.TreasureMapConfig.TryGetValue(posid, out myTreasureMapConfig))
			{
				OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, myOnePieceData);
				myOnePieceData.PosID = posid;
				this.ModifyOnePieceTreasureData(client, myOnePieceData);
				byte[] bytesData = DataHelper.ObjectToBytes<OnePieceTreasureData>(myOnePieceData);
				GameManager.ClientMgr.SendToClient(client, bytesData, 1600);
			}
		}

		
		public void GM_PrintTreasureData(GameClient client)
		{
			OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
			OnePieceManager.getInstance().GetOnePieceTreasureData(client, myOnePieceData);
			string strinfo = string.Format("藏宝秘境 位置PosID[{0}] MoveLeft[{1}] RollNumNormal[{2}] RollNumMiracle[{3}] JiFen[{4}] XueZuan[{5}]", new object[]
			{
				myOnePieceData.PosID,
				client.ClientData.OnePieceMoveLeft,
				myOnePieceData.RollNumNormal,
				myOnePieceData.RollNumMiracle,
				GameManager.ClientMgr.GetTreasureJiFen(client),
				GameManager.ClientMgr.GetTreasureXueZuan(client)
			});
			GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
		}

		
		public void UpdateOnePieceTreasureLogDB(GameClient client, OnePieceTreasureLogType LogType, int addValue = 1)
		{
			EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.Building, LogRecordType.IntValue, new object[]
			{
				LogType,
				addValue
			});
		}

		
		public bool ProcessOnePieceGetInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				this.HandleRoleLogout(client);
				this.JudgeResetOnePieceTreasureData(client);
				OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, myOnePieceData);
				byte[] bytesData = DataHelper.ObjectToBytes<OnePieceTreasureData>(myOnePieceData);
				GameManager.ClientMgr.SendToClient(client, bytesData, nID);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessOnePieceRollMiracleCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				string strcmd;
				if (this.IfCanContinueMove(client))
				{
					result = 5;
					strcmd = string.Format("{0}:{1}", result, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				int MoveNumRoll = Global.SafeConvertToInt32(cmdParams[0]);
				if (MoveNumRoll <= 0 || MoveNumRoll > this.SystemParamsTreasureDice.Count)
				{
					result = 3;
					strcmd = string.Format("{0}:{1}", result, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				this.JudgeResetOnePieceTreasureData(client);
				OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, myOnePieceData);
				if (myOnePieceData.RollNumMiracle < 1)
				{
					result = 15;
					strcmd = string.Format("{0}:{1}", result, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				myOnePieceData.EventID = 0;
				client.ClientData.OnePieceMoveLeft = MoveNumRoll;
				client.ClientData.OnePieceMoveDir = MoveNumRoll;
				int DestPosID = this.CalculateMoveCellToNextEvent(client, MoveNumRoll, myOnePieceData);
				myOnePieceData.RollNumMiracle--;
				this.ModifyOnePieceTreasureData(client, myOnePieceData);
				strcmd = string.Format("{0}:{1}", result, MoveNumRoll);
				client.sendCmd(nID, strcmd, false);
				strcmd = string.Format("{0}:{1}", result, DestPosID);
				client.sendCmd(1604, strcmd, false);
				this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_Role, 1);
				this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_MoveNum, MoveNumRoll);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessOnePieceRollCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				string strcmd;
				if (this.IfCanContinueMove(client))
				{
					result = 5;
					strcmd = string.Format("{0}:{1}", result, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				this.JudgeResetOnePieceTreasureData(client);
				OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, myOnePieceData);
				if (myOnePieceData.RollNumNormal < 1)
				{
					result = 15;
					strcmd = string.Format("{0}:{1}", result, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				myOnePieceData.EventID = 0;
				int MoveNumRoll = this.RollMoveNum();
				client.ClientData.OnePieceMoveLeft = MoveNumRoll;
				client.ClientData.OnePieceMoveDir = MoveNumRoll;
				int DestPosID = this.CalculateMoveCellToNextEvent(client, MoveNumRoll, myOnePieceData);
				myOnePieceData.RollNumNormal--;
				this.ModifyOnePieceTreasureData(client, myOnePieceData);
				strcmd = string.Format("{0}:{1}", result, MoveNumRoll);
				client.sendCmd(nID, strcmd, false);
				strcmd = string.Format("{0}:{1}", result, DestPosID);
				client.sendCmd(1604, strcmd, false);
				this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_Role, 1);
				this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_MoveNum, MoveNumRoll);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessOnePieceDiceBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string strcmd = "";
				int result = 0;
				int diceType = Global.SafeConvertToInt32(cmdParams[0]);
				int diceBuyNum = Global.SafeConvertToInt32(cmdParams[1]);
				if (diceBuyNum <= 0 || diceBuyNum > 99)
				{
					result = 14;
					strcmd = string.Format("{0}:{1}:{2}", result, diceType, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, myOnePieceData);
				int UserMoneyCost;
				if (diceType == 0)
				{
					if (myOnePieceData.RollNumNormal + diceBuyNum > 99)
					{
						result = 14;
						strcmd = string.Format("{0}:{1}:{2}", result, diceType, 0);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
					UserMoneyCost = diceBuyNum * this.SystemParamsTreasurePrice;
				}
				else
				{
					if (diceType != 1)
					{
						result = 3;
						strcmd = string.Format("{0}:{1}:{2}", result, diceType, 0);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
					if (myOnePieceData.RollNumMiracle + diceBuyNum > 99)
					{
						result = 14;
						strcmd = string.Format("{0}:{1}:{2}", result, diceType, 0);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
					UserMoneyCost = diceBuyNum * this.SystemParamsTreasureSuperPrice;
				}
				if (client.ClientData.UserMoney < UserMoneyCost)
				{
					result = 1;
					strcmd = string.Format("{0}:{1}", result, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				if (UserMoneyCost > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, UserMoneyCost, "藏宝秘境买骰子", true, true, false, DaiBiSySType.None))
					{
						result = 1;
						strcmd = string.Format("{0}:{1}", result, 0);
						client.sendCmd(nID, strcmd, false);
						return true;
					}
				}
				if (diceType == 0)
				{
					this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_BuyDice, diceBuyNum);
					myOnePieceData.RollNumNormal += diceBuyNum;
					strcmd = string.Format("{0}:{1}:{2}", result, diceType, myOnePieceData.RollNumNormal);
				}
				else if (diceType == 1)
				{
					this.UpdateOnePieceTreasureLogDB(client, OnePieceTreasureLogType.TreasureLog_BuySuperDice, diceBuyNum);
					myOnePieceData.RollNumMiracle += diceBuyNum;
					strcmd = string.Format("{0}:{1}:{2}", result, diceType, myOnePieceData.RollNumMiracle);
				}
				this.ModifyOnePieceTreasureData(client, myOnePieceData);
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool IfHaveOnePieceBoxListAward(GameClient client)
		{
			return client.ClientData.OnePieceBoxIDList != null;
		}

		
		public bool IfCanContinueMove(GameClient client)
		{
			return client.ClientData.OnePieceMoveLeft != 0 || this.IfHaveOnePieceBoxListAward(client);
		}

		
		public bool ProcessOnePieceMoveCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				string strcmd;
				if (!this.IfCanContinueMove(client))
				{
					result = 12;
					strcmd = string.Format("{0}:{1}", result, 0);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, myOnePieceData);
				if (this.IfHaveOnePieceBoxListAward(client))
				{
					result = this.TryGiveOnePieceBoxListAward(client);
					myOnePieceData.PosID = this.FilterPosIDChangeFloor(client, myOnePieceData.PosID);
				}
				else
				{
					this.HandleOnePieceTreasureMove(client, client.ClientData.OnePieceMoveLeft, myOnePieceData);
				}
				int DestPosID = myOnePieceData.PosID;
				if (!this.IfHaveOnePieceBoxListAward(client))
				{
					DestPosID = this.CalculateMoveCellToNextEvent(client, client.ClientData.OnePieceMoveLeft, myOnePieceData);
				}
				this.ModifyOnePieceTreasureData(client, myOnePieceData);
				strcmd = string.Format("{0}:{1}", result, DestPosID);
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessOnePieceTriggerEventCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result;
				string strcmd;
				if (client.ClientData.OnePieceMoveLeft != 0)
				{
					result = 5;
					strcmd = string.Format("{0}:{1}", result, -1);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				OnePieceTreasureData myOnePieceData = new OnePieceTreasureData();
				this.GetOnePieceTreasureData(client, myOnePieceData);
				if (myOnePieceData.EventID == 0)
				{
					result = 6;
					strcmd = string.Format("{0}:{1}", result, -1);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				OnePieceTreasureEventConfig myTreasureEventConfig = null;
				if (!this.TreasureEventConfig.TryGetValue(myOnePieceData.EventID, out myTreasureEventConfig))
				{
					result = 6;
					strcmd = string.Format("{0}:{1}", result, -1);
					client.sendCmd(nID, strcmd, false);
					return true;
				}
				result = (int)this.TriggerEvent(client, myOnePieceData, myTreasureEventConfig);
				if (result == 0 || result == 16)
				{
					myOnePieceData.EventID = 0;
				}
				this.ModifyOnePieceTreasureData(client, myOnePieceData);
				strcmd = string.Format("{0}:{1}", result, (int)myTreasureEventConfig.Type);
				client.sendCmd(nID, strcmd, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool InitConfig()
		{
			string TreasureDice = GameManager.systemParamsList.GetParamValueByName("TreasureDice");
			if (!string.IsNullOrEmpty(TreasureDice))
			{
				string[] Filed = TreasureDice.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < Filed.Length; i++)
				{
					string[] StringPair = Filed[i].Split(new char[]
					{
						','
					});
					if (StringPair.Length == 2)
					{
						this.SystemParamsTreasureDice.Add(Global.SafeConvertToDouble(StringPair[1]));
					}
				}
			}
			string TreasureFreeNum = GameManager.systemParamsList.GetParamValueByName("TreasureFreeNum");
			if (!string.IsNullOrEmpty(TreasureFreeNum))
			{
				string[] Filed = TreasureFreeNum.Split(new char[]
				{
					','
				});
				if (Filed.Length == 2)
				{
					this.SystemParamsTreasureFreeNum = Global.SafeConvertToInt32(Filed[0]);
					this.SystemParamsTreasureMiracleNum = Global.SafeConvertToInt32(Filed[1]);
				}
			}
			string TreasurePrice = GameManager.systemParamsList.GetParamValueByName("TreasurePrice");
			if (!string.IsNullOrEmpty(TreasurePrice))
			{
				this.SystemParamsTreasurePrice = Global.SafeConvertToInt32(TreasurePrice);
			}
			string TreasureSuperPrice = GameManager.systemParamsList.GetParamValueByName("TreasureSuperPrice");
			if (!string.IsNullOrEmpty(TreasureSuperPrice))
			{
				this.SystemParamsTreasureSuperPrice = Global.SafeConvertToInt32(TreasureSuperPrice);
			}
			return this.LoadOnePieceTreasureMapFile() && this.LoadOnePieceTreasureEventFile() && this.LoadOnePieceTreasureBoxFile();
		}

		
		public bool LoadOnePieceTreasureMapFile()
		{
			try
			{
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Treasure/TreasureMap.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						OnePieceTreasureMapConfig myTreasureMap = new OnePieceTreasureMapConfig();
						myTreasureMap.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myTreasureMap.Num = (int)Global.GetSafeAttributeLong(xmlItem, "Num");
						myTreasureMap.Floor = (int)Global.GetSafeAttributeLong(xmlItem, "Floor");
						myTreasureMap.Trigger = (TriggerType)Global.GetSafeAttributeLong(xmlItem, "Trigger");
						myTreasureMap.Score = (int)Global.GetSafeAttributeLong(xmlItem, "Score");
						string RandomEvent = Global.GetSafeAttributeStr(xmlItem, "Event");
						if (string.IsNullOrEmpty(RandomEvent))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取TreasureMap.xml中的Event失败", new object[0]), null, true);
						}
						else
						{
							string[] Filed = RandomEvent.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < Filed.Length; i++)
							{
								string[] RatePair = Filed[i].Split(new char[]
								{
									','
								});
								if (RatePair.Length == 2)
								{
									OnePieceRandomEvent myRandomEvent = new OnePieceRandomEvent();
									myRandomEvent.EventID = Global.SafeConvertToInt32(RatePair[0]);
									myRandomEvent.Rate = Global.SafeConvertToDouble(RatePair[1]);
									myTreasureMap.LisRandomEvent.Add(myRandomEvent);
								}
							}
						}
						this.TreasureMapConfig[myTreasureMap.ID] = myTreasureMap;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "TreasureMap.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public bool LoadOnePieceTreasureEventFile()
		{
			try
			{
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Treasure/TreasureEvent.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						OnePieceTreasureEventConfig myTreasureEvent = new OnePieceTreasureEventConfig();
						myTreasureEvent.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myTreasureEvent.Type = (TreasureEventType)Global.GetSafeAttributeLong(xmlItem, "Type");
						string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "NewGoods");
						if (!string.IsNullOrEmpty(goodsIDs))
						{
							ConfigParser.ParseAwardsItemList(goodsIDs, ref myTreasureEvent.GoodsList, '|', ',');
						}
						string MoneyAward = Global.GetSafeAttributeStr(xmlItem, "NewValue");
						if (!string.IsNullOrEmpty(MoneyAward))
						{
							string[] Filed = MoneyAward.Split(new char[]
							{
								','
							});
							if (Filed.Length == 2)
							{
								myTreasureEvent.NewValue.Type = (MoneyTypes)Global.SafeConvertToInt32(Filed[0]);
								myTreasureEvent.NewValue.Num = Global.SafeConvertToInt32(Filed[1]);
							}
						}
						if (string.IsNullOrEmpty(goodsIDs) && string.IsNullOrEmpty(MoneyAward))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取TreasureEvent.xml奖励配置项1失败", new object[0]), null, true);
						}
						string NeedGoods = Global.GetSafeAttributeStr(xmlItem, "NeedGoods");
						if (!string.IsNullOrEmpty(MoneyAward))
						{
							string[] Filed = MoneyAward.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < Filed.Length; i++)
							{
								string[] GoodsPairFiled = Filed[i].Split(new char[]
								{
									','
								});
								if (GoodsPairFiled.Length == 2)
								{
									OnePieceGoodsPair myGoodsPair = new OnePieceGoodsPair();
									myGoodsPair._NeedGoodsID = Global.SafeConvertToInt32(GoodsPairFiled[0]);
									myGoodsPair._NeedGoodsCount = Global.SafeConvertToInt32(GoodsPairFiled[1]);
									myTreasureEvent.NeedGoods.Add(myGoodsPair);
								}
							}
						}
						string NeedValue = Global.GetSafeAttributeStr(xmlItem, "NeedValue");
						if (!string.IsNullOrEmpty(NeedValue))
						{
							string[] Filed = NeedValue.Split(new char[]
							{
								','
							});
							if (Filed.Length == 2)
							{
								myTreasureEvent.NeedValue.Type = (MoneyTypes)Global.SafeConvertToInt32(Filed[0]);
								myTreasureEvent.NeedValue.Num = Global.SafeConvertToInt32(Filed[1]);
							}
						}
						string Move = Global.GetSafeAttributeStr(xmlItem, "Move");
						if (!string.IsNullOrEmpty(Move))
						{
							string[] Filed = Move.Split(new char[]
							{
								','
							});
							for (int i = 0; i < Filed.Length; i++)
							{
								myTreasureEvent.MoveRange.Add(Global.SafeConvertToInt32(Filed[i]));
							}
						}
						myTreasureEvent.NewDiec = (int)Global.GetSafeAttributeLong(xmlItem, "NewDiec");
						myTreasureEvent.NewSuperDiec = (int)Global.GetSafeAttributeLong(xmlItem, "NewSuperDiec");
						myTreasureEvent.FuBenID = (int)Global.GetSafeAttributeLong(xmlItem, "FuBenID");
						string TreasureBox = Global.GetSafeAttributeStr(xmlItem, "Box");
						if (!string.IsNullOrEmpty(TreasureBox))
						{
							string[] Filed = TreasureBox.Split(new char[]
							{
								'|'
							});
							for (int i = 0; i < Filed.Length; i++)
							{
								string[] BoxPair = Filed[i].Split(new char[]
								{
									','
								});
								if (BoxPair.Length == 2)
								{
									OnePieceTreasureBoxPair myBox = new OnePieceTreasureBoxPair();
									myBox.BoxID = Global.SafeConvertToInt32(BoxPair[0]);
									myBox.OpenNum = Global.SafeConvertToInt32(BoxPair[1]);
									myTreasureEvent.BoxList.Add(myBox);
								}
							}
						}
						this.TreasureEventConfig[myTreasureEvent.ID] = myTreasureEvent;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "TreasureEvent.xml", ex.Message), null, true);
			}
			return true;
		}

		
		public bool LoadOnePieceTreasureBoxFile()
		{
			try
			{
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/Treasure/TreasureBox.xml"));
				if (null == xml)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						List<OnePieceTreasureBoxConfig> RandomTreasureBox = new List<OnePieceTreasureBoxConfig>();
						int BoxID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						IEnumerable<XElement> xmlBoxItems = xmlItem.Elements();
						foreach (XElement xmlBoxItem in xmlBoxItems)
						{
							OnePieceTreasureBoxConfig myTreasureBox = new OnePieceTreasureBoxConfig();
							myTreasureBox.ID = (int)Global.GetSafeAttributeLong(xmlBoxItem, "ID");
							myTreasureBox.Type = (TeasureBoxType)Global.GetSafeAttributeLong(xmlBoxItem, "Type");
							string goodsIDs = Global.GetSafeAttributeStr(xmlBoxItem, "Goods");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取TreasureBox.xml奖励配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] Filed = goodsIDs.Split(new char[]
								{
									','
								});
								if (Filed.Length != 1)
								{
									ConfigParser.ParseAwardsItemList(goodsIDs, ref myTreasureBox.Goods, '|', ',');
								}
								else
								{
									myTreasureBox.Num = Global.SafeConvertToInt32(goodsIDs);
								}
							}
							myTreasureBox.BeginNum = (int)Global.GetSafeAttributeLong(xmlBoxItem, "BeginNum");
							myTreasureBox.EndNum = (int)Global.GetSafeAttributeLong(xmlBoxItem, "EndNum");
							RandomTreasureBox.Add(myTreasureBox);
						}
						this.TreasureBoxConfig[BoxID] = RandomTreasureBox;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "TreasureBox.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		private const string OnePiece_TreasureMapfileName = "Config/Treasure/TreasureMap.xml";

		
		private const string OnePiece_TreasureEventfileName = "Config/Treasure/TreasureEvent.xml";

		
		private const string OnePiece_TreasureBoxfileName = "Config/Treasure/TreasureBox.xml";

		
		private const int OnePiece_FloorHashNum = 1000;

		
		private const int OnePiece_FloorCellNum = 30;

		
		private const int OnePiece_DiceMaxNum = 99;

		
		public List<double> SystemParamsTreasureDice = new List<double>();

		
		public int SystemParamsTreasureFreeNum = 0;

		
		public int SystemParamsTreasureMiracleNum = 0;

		
		public int SystemParamsTreasurePrice = 0;

		
		public int SystemParamsTreasureSuperPrice = 0;

		
		public int OnePiece_FakeRollNum_GM = 0;

		
		private static OnePieceManager instance = new OnePieceManager();

		
		public Dictionary<int, OnePieceTreasureMapConfig> TreasureMapConfig = new Dictionary<int, OnePieceTreasureMapConfig>();

		
		public Dictionary<int, OnePieceTreasureEventConfig> TreasureEventConfig = new Dictionary<int, OnePieceTreasureEventConfig>();

		
		public Dictionary<int, List<OnePieceTreasureBoxConfig>> TreasureBoxConfig = new Dictionary<int, List<OnePieceTreasureBoxConfig>>();
	}
}
