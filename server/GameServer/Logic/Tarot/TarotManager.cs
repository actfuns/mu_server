using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.TarotData;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Tarot
{
	// Token: 0x0200042E RID: 1070
	internal class TarotManager : ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x06001393 RID: 5011 RVA: 0x00135980 File Offset: 0x00133B80
		public static TarotManager getInstance()
		{
			return TarotManager.instance;
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x00135998 File Offset: 0x00133B98
		public void Initialize()
		{
			string fileName = Global.GameResPath("Config/Tarot.xml");
			XElement xml = XElement.Load(fileName);
			if (null == xml)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
			}
			IEnumerable<XElement> xmlItems = xml.Elements();
			foreach (XElement xmlItem in xmlItems)
			{
				TarotManager.TarotTemplate data = new TarotManager.TarotTemplate();
				data.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Level"));
				if (data.Level != 0)
				{
					data.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
					data.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
					data.GoodsID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "GoodsID"));
					string[] needGoodsInfo = Global.GetSafeAttributeStr(xmlItem, "NeedGoods").Split(new char[]
					{
						','
					});
					data.NeedGoodID = Convert.ToInt32(needGoodsInfo[0]);
					data.NeedPartCount = Convert.ToInt32(needGoodsInfo[1]);
					if (TarotManager.TarotNeedCardNum.ContainsKey(data.NeedGoodID))
					{
						Dictionary<int, int> tarotNeedCardNum;
						int needGoodID;
						(tarotNeedCardNum = TarotManager.TarotNeedCardNum)[needGoodID = data.NeedGoodID] = tarotNeedCardNum[needGoodID] + data.NeedPartCount;
					}
					else
					{
						TarotManager.TarotNeedCardNum.Add(data.NeedGoodID, data.NeedPartCount);
					}
					if (TarotManager.TarotMaxLevelDict.ContainsKey(data.GoodsID) && TarotManager.TarotMaxLevelDict[data.GoodsID] < data.Level)
					{
						TarotManager.TarotMaxLevelDict[data.GoodsID] = data.Level;
					}
					else
					{
						TarotManager.TarotMaxLevelDict.Add(data.GoodsID, data.Level);
					}
					TarotManager.TarotTemplates.Add(data);
				}
			}
			TarotManager.TarotCardIds = TarotManager.TarotMaxLevelDict.Keys.ToList<int>();
			string[] kingCost = GameManager.systemParamsList.GetParamValueByName("TarotKingCost").Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			TarotManager.KingItemId = Convert.ToInt32(kingCost[0]);
			TarotManager.UseKingItemCount = Convert.ToInt32(kingCost[1]);
			TarotManager.KingBuffTime = (long)(Convert.ToInt32(kingCost[2]) * 60);
			string[] kingValueInfo = GameManager.systemParamsList.GetParamValueByName("TarotKingNum").Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string info in kingValueInfo)
			{
				TarotManager.KingBuffValueList.Add(Convert.ToInt32(info));
			}
			TCPCmdDispatcher.getInstance().registerProcessorEx(1701, 1, 1, TarotManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1702, 2, 2, TarotManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1703, 1, 1, TarotManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1704, 1, 1, TarotManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1705, 3, 3, TarotManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x00135D1C File Offset: 0x00133F1C
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1701:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int goodID = Convert.ToInt32(cmdParams[0]);
					string strret = string.Format("{0}:{1}", Convert.ToInt32(this.ProcessTarotUpCmd(client, goodID)), goodID);
					client.sendCmd(nID, strret, false);
				}
				catch (Exception ex)
				{
					string strret = string.Format("{0}:{1}", -1, cmdParams[0]);
					client.sendCmd(nID, strret, false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_TAROT_UPORINIT", false, false);
				}
				break;
			case 1702:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int goodID = Convert.ToInt32(cmdParams[0]);
					byte pos = Convert.ToByte(cmdParams[1]);
					string strret = string.Format("{0}:{1}", Convert.ToInt32(this.ProcessSetTarotPosCmd(client, goodID, pos)), goodID);
					client.sendCmd(nID, strret, false);
				}
				catch (Exception ex)
				{
					string strret = string.Format("{0}:{1}", -1, cmdParams[0]);
					client.sendCmd(nID, strret, false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_SET_TAROTPOS", false, false);
				}
				break;
			case 1703:
				try
				{
					string strret = string.Empty;
					int restult = Convert.ToInt32(this.ProcessUseKingPrivilegeCmd(client, out strret));
					client.sendCmd(nID, string.Format("{0}:{1}", restult, strret), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_USE_TAROTKINGPRIVILEGE", false, false);
				}
				break;
			case 1704:
				try
				{
					TarotSystemData tarotData = client.ClientData.TarotData;
					client.sendCmd<TarotSystemData>(nID, tarotData, false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_USE_TAROTKINGPRIVILEGE", false, false);
				}
				break;
			case 1705:
				if (cmdParams == null || cmdParams.Length != 3)
				{
					return false;
				}
				try
				{
					int dbid = Convert.ToInt32(cmdParams[0]);
					int goodID = Convert.ToInt32(cmdParams[1]);
					int num = Convert.ToInt32(cmdParams[2]);
					int resNum = 0;
					int restult = Convert.ToInt32(this.ProcessTarotMoneyCmd(client, goodID, num, dbid, out resNum));
					string strret = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						restult,
						goodID,
						resNum,
						dbid
					});
					client.sendCmd(nID, strret, false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_TAROT_MONEY_NUM", false, false);
				}
				break;
			}
			return true;
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x001360B8 File Offset: 0x001342B8
		public TarotManager.ETarotResult ProcessTarotUpCmd(GameClient client, int goodID)
		{
			TarotManager.ETarotResult result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.TarotCard, false))
			{
				result = TarotManager.ETarotResult.NotOpen;
			}
			else
			{
				TarotSystemData tarotData = client.ClientData.TarotData;
				TarotCardData currentTarot = tarotData.TarotCardDatas.Find((TarotCardData x) => x.GoodId == goodID);
				if (currentTarot == null)
				{
					currentTarot = new TarotCardData();
					currentTarot.GoodId = goodID;
					tarotData.TarotCardDatas.Add(currentTarot);
				}
				if (currentTarot.Level >= TarotManager.TarotMaxLevelDict[goodID])
				{
					result = TarotManager.ETarotResult.MaxLevel;
				}
				else
				{
					TarotManager.TarotTemplate nextTemp = TarotManager.TarotTemplates.Find((TarotManager.TarotTemplate x) => x.GoodsID == goodID && x.Level == currentTarot.Level + 1);
					if (nextTemp == null)
					{
						result = TarotManager.ETarotResult.Error;
					}
					else if (currentTarot.TarotMoney < nextTemp.NeedPartCount)
					{
						result = TarotManager.ETarotResult.NeedPart;
					}
					else
					{
						currentTarot.TarotMoney -= nextTemp.NeedPartCount;
						currentTarot.Level++;
						this.UpdataPalyerTarotAttr(client);
						TarotManager.UpdateTarotData2DB(client, currentTarot, null);
						result = TarotManager.ETarotResult.Success;
					}
				}
			}
			return result;
		}

		// Token: 0x06001397 RID: 5015 RVA: 0x00136258 File Offset: 0x00134458
		public TarotManager.ETarotResult ProcessSetTarotPosCmd(GameClient client, int goodID, byte pos)
		{
			TarotManager.ETarotResult result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.TarotCard, false))
			{
				result = TarotManager.ETarotResult.NotOpen;
			}
			else if (pos < 0 || pos > 6)
			{
				result = TarotManager.ETarotResult.Error;
			}
			else
			{
				TarotSystemData tarotData = client.ClientData.TarotData;
				TarotCardData currentTarot = tarotData.TarotCardDatas.Find((TarotCardData x) => x.GoodId == goodID);
				if (currentTarot == null)
				{
					result = TarotManager.ETarotResult.Error;
				}
				else if (currentTarot.Postion == pos)
				{
					result = TarotManager.ETarotResult.Error;
				}
				else
				{
					if (pos > 0)
					{
						if (currentTarot.Postion > 0)
						{
							return TarotManager.ETarotResult.Error;
						}
						TarotCardData targetTarot = tarotData.TarotCardDatas.Find((TarotCardData x) => x.Postion == pos);
						if (targetTarot != null)
						{
							targetTarot.Postion = 0;
						}
					}
					currentTarot.Postion = pos;
					this.UpdataPalyerTarotAttr(client);
					TarotManager.UpdateTarotData2DB(client, currentTarot, null);
					result = TarotManager.ETarotResult.Success;
				}
			}
			return result;
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x001363A4 File Offset: 0x001345A4
		public TarotManager.ETarotResult ProcessUseKingPrivilegeCmd(GameClient client, out string strret)
		{
			strret = string.Empty;
			TarotManager.ETarotResult result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.TarotCard, false))
			{
				result = TarotManager.ETarotResult.NotOpen;
			}
			else
			{
				TarotSystemData tarotData = client.ClientData.TarotData;
				if (tarotData.KingData.StartTime > 0L)
				{
					tarotData.KingData = new TarotKingData();
					this.UpdataPalyerTarotAttr(client);
					TarotManager.UpdateTarotData2DB(client, null, tarotData.KingData);
					result = TarotManager.ETarotResult.Success;
				}
				else
				{
					int kingItemCount = Global.GetTotalGoodsCountByID(client, TarotManager.KingItemId);
					if (kingItemCount < TarotManager.UseKingItemCount)
					{
						result = TarotManager.ETarotResult.ItemNotEnough;
					}
					else
					{
						bool usedBinding = false;
						bool usedTimeLimited = false;
						if (Global.UseGoodsBindOrNot(client, TarotManager.KingItemId, TarotManager.UseKingItemCount, true, out usedBinding, out usedTimeLimited) < 1)
						{
							result = TarotManager.ETarotResult.NeedPart;
						}
						else
						{
							tarotData.KingData.StartTime = TimeUtil.NOW();
							tarotData.KingData.BufferSecs = TarotManager.KingBuffTime;
							TarotManager.TarotCardIds = Global.RandomSortList<int>(TarotManager.TarotCardIds);
							TarotManager.KingBuffValueList = Global.RandomSortList<int>(TarotManager.KingBuffValueList);
							tarotData.KingData.AddtionDict = new Dictionary<int, int>();
							int totalNum = TarotManager.KingBuffValueList[0];
							if (totalNum < 3)
							{
								result = TarotManager.ETarotResult.Error;
							}
							else
							{
								for (int i = 0; i < 3; i++)
								{
									int ranNum;
									if (i < 2)
									{
										ranNum = Global.GetRandomNumber(0, totalNum - 3);
										totalNum -= ranNum;
									}
									else
									{
										ranNum = totalNum - 3;
									}
									int ranGoodId = TarotManager.TarotCardIds[i];
									object obj = strret;
									strret = string.Concat(new object[]
									{
										obj,
										ranGoodId,
										":",
										ranNum + 1,
										":"
									});
									tarotData.KingData.AddtionDict.Add(ranGoodId, ranNum + 1);
								}
								this.UpdataPalyerTarotAttr(client);
								TarotManager.UpdateTarotData2DB(client, null, tarotData.KingData);
								strret = strret.TrimEnd(new char[]
								{
									':'
								});
								result = TarotManager.ETarotResult.Success;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x00136624 File Offset: 0x00134824
		public TarotManager.ETarotResult ProcessTarotMoneyCmd(GameClient client, int goodId, int num, int dbid, out int resNum)
		{
			resNum = 0;
			TarotManager.ETarotResult result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.TarotCard, false))
			{
				result = TarotManager.ETarotResult.NotOpen;
			}
			else
			{
				GoodsData Data = Global.GetGoodsByDbID(client, dbid);
				if (Data == null)
				{
					result = TarotManager.ETarotResult.Error;
				}
				else if (TarotManager.TarotNeedCardNum.ContainsKey(goodId))
				{
					if (num < 0 || num > Data.GCount)
					{
						result = TarotManager.ETarotResult.MoneyNumError;
					}
					else
					{
						TarotManager.TarotTemplate nextTemp = TarotManager.TarotTemplates.Find((TarotManager.TarotTemplate x) => x.NeedGoodID == goodId);
						if (nextTemp == null)
						{
							result = TarotManager.ETarotResult.NotFindGood;
						}
						else
						{
							TarotSystemData tarotData = client.ClientData.TarotData;
							TarotCardData currentTarot = tarotData.TarotCardDatas.Find((TarotCardData x) => x.GoodId == nextTemp.GoodsID);
							if (currentTarot == null)
							{
								currentTarot = new TarotCardData();
								currentTarot.GoodId = nextTemp.GoodsID;
								tarotData.TarotCardDatas.Add(currentTarot);
							}
							int reNum = TarotManager.TarotNeedCardNum[goodId] - this.CountTarotNowToCurrLevel(goodId, currentTarot.Level) - currentTarot.TarotMoney;
							if (reNum == 0)
							{
								result = TarotManager.ETarotResult.HasMaxNum;
							}
							else
							{
								if (num > reNum)
								{
									num = reNum;
								}
								if (GameManager.ClientMgr.NotifyUseGoodsByDbId(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, dbid, num, false, false))
								{
									GoodsData goodsData = Global.GetGoodsByDbID(client, dbid);
									if (null != goodsData)
									{
										resNum = goodsData.GCount;
									}
									currentTarot.TarotMoney += num;
									EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.TarotMoney, LogRecordType.TarotMoney, new object[]
									{
										num,
										currentTarot.TarotMoney,
										dbid,
										"塔罗牌货币增加"
									});
									this.UpdataPalyerTarotAttr(client);
									TarotManager.UpdateTarotData2DB(client, currentTarot, null);
									GameManager.logDBCmdMgr.AddDBLogInfo(dbid, "塔罗牌货币", "塔罗牌", client.ClientData.RoleName, "系统", "修改", 0, client.ClientData.ZoneID, client.strUserID, num, client.ServerId, null);
									result = TarotManager.ETarotResult.Success;
								}
								else
								{
									result = TarotManager.ETarotResult.UseGoodError;
								}
							}
						}
					}
				}
				else
				{
					result = TarotManager.ETarotResult.GoodIdError;
				}
			}
			return result;
		}

		// Token: 0x0600139A RID: 5018 RVA: 0x001368E8 File Offset: 0x00134AE8
		public int CountTarotNowToCurrLevel(int cardId, int level)
		{
			int NeedNum = 0;
			foreach (TarotManager.TarotTemplate i in TarotManager.TarotTemplates)
			{
				if (i.NeedGoodID == cardId && i.Level <= level)
				{
					NeedNum += i.NeedPartCount;
				}
			}
			return NeedNum;
		}

		// Token: 0x0600139B RID: 5019 RVA: 0x0013696C File Offset: 0x00134B6C
		public void RemoveTarotKingData(GameClient client)
		{
			TarotSystemData tarotData = client.ClientData.TarotData;
			if (tarotData.KingData.StartTime != 0L)
			{
				long nowTicks = TimeUtil.NOW();
				if (nowTicks - tarotData.KingData.StartTime >= tarotData.KingData.BufferSecs * 1000L)
				{
					tarotData.KingData = new TarotKingData();
					this.UpdataPalyerTarotAttr(client);
					TarotManager.UpdateTarotData2DB(client, null, tarotData.KingData);
				}
			}
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x001369EC File Offset: 0x00134BEC
		public void UpdataPalyerTarotAttr(GameClient client)
		{
			EquipPropItem itemNew = new EquipPropItem();
			double[] extProps = itemNew.ExtProps;
			foreach (TarotCardData card in client.ClientData.TarotData.TarotCardDatas)
			{
				EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(card.GoodId);
				if (card.Postion > 0)
				{
					for (int i = 0; i < extProps.Length; i++)
					{
						int addLevel = 0;
						if (client.ClientData.TarotData.KingData.AddtionDict.ContainsKey(card.GoodId))
						{
							addLevel = client.ClientData.TarotData.KingData.AddtionDict[card.GoodId];
						}
						extProps[i] += item.ExtProps[i] * (double)(card.Level + addLevel);
					}
				}
			}
			client.ClientData.PropsCacheManager.SetExtProps(new object[]
			{
				PropsSystemTypes.TarotCard,
				0,
				extProps
			});
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x00136B98 File Offset: 0x00134D98
		private static void UpdateTarotData2DB(GameClient client, TarotCardData tarotData, TarotKingData tarotKingBuffData)
		{
			string[] dbFields = null;
			string tarotStrInfo = (tarotData == null) ? "-1" : tarotData.GetDataStrInfo();
			string kingBuffStrInfo = (tarotKingBuffData == null) ? "-1" : tarotKingBuffData.GetDataStrInfo();
			string sCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, tarotStrInfo, kingBuffStrInfo);
			TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 20100, sCmd, out dbFields, client.ServerId);
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x00136C14 File Offset: 0x00134E14
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x04001CDC RID: 7388
		private static List<TarotManager.TarotTemplate> TarotTemplates = new List<TarotManager.TarotTemplate>();

		// Token: 0x04001CDD RID: 7389
		private static Dictionary<int, int> TarotMaxLevelDict = new Dictionary<int, int>();

		// Token: 0x04001CDE RID: 7390
		private static Dictionary<int, int> TarotNeedCardNum = new Dictionary<int, int>();

		// Token: 0x04001CDF RID: 7391
		private static List<int> TarotCardIds = new List<int>();

		// Token: 0x04001CE0 RID: 7392
		private static int KingItemId = 0;

		// Token: 0x04001CE1 RID: 7393
		private static long KingBuffTime = 0L;

		// Token: 0x04001CE2 RID: 7394
		private static int UseKingItemCount = 0;

		// Token: 0x04001CE3 RID: 7395
		private static List<int> KingBuffValueList = new List<int>();

		// Token: 0x04001CE4 RID: 7396
		private static TarotManager instance = new TarotManager();

		// Token: 0x0200042F RID: 1071
		public enum ETarotResult
		{
			// Token: 0x04001CE6 RID: 7398
			Error = -1,
			// Token: 0x04001CE7 RID: 7399
			Success,
			// Token: 0x04001CE8 RID: 7400
			Fail,
			// Token: 0x04001CE9 RID: 7401
			MaxLevel,
			// Token: 0x04001CEA RID: 7402
			NeedPart,
			// Token: 0x04001CEB RID: 7403
			PartSuitIsMax,
			// Token: 0x04001CEC RID: 7404
			NotOpen,
			// Token: 0x04001CED RID: 7405
			PartNumError,
			// Token: 0x04001CEE RID: 7406
			PosError,
			// Token: 0x04001CEF RID: 7407
			ItemNotEnough,
			// Token: 0x04001CF0 RID: 7408
			HasMaxNum,
			// Token: 0x04001CF1 RID: 7409
			MoneyNumError,
			// Token: 0x04001CF2 RID: 7410
			GoodIdError,
			// Token: 0x04001CF3 RID: 7411
			UseGoodError,
			// Token: 0x04001CF4 RID: 7412
			NotFindCard,
			// Token: 0x04001CF5 RID: 7413
			NotFindGood
		}

		// Token: 0x02000430 RID: 1072
		public class TarotTemplate
		{
			// Token: 0x1700004E RID: 78
			// (get) Token: 0x060013A1 RID: 5025 RVA: 0x00136C8C File Offset: 0x00134E8C
			// (set) Token: 0x060013A2 RID: 5026 RVA: 0x00136CA3 File Offset: 0x00134EA3
			public int ID { get; set; }

			// Token: 0x1700004F RID: 79
			// (get) Token: 0x060013A3 RID: 5027 RVA: 0x00136CAC File Offset: 0x00134EAC
			// (set) Token: 0x060013A4 RID: 5028 RVA: 0x00136CC3 File Offset: 0x00134EC3
			public string Name { get; set; }

			// Token: 0x17000050 RID: 80
			// (get) Token: 0x060013A5 RID: 5029 RVA: 0x00136CCC File Offset: 0x00134ECC
			// (set) Token: 0x060013A6 RID: 5030 RVA: 0x00136CE3 File Offset: 0x00134EE3
			public int GoodsID { get; set; }

			// Token: 0x17000051 RID: 81
			// (get) Token: 0x060013A7 RID: 5031 RVA: 0x00136CEC File Offset: 0x00134EEC
			// (set) Token: 0x060013A8 RID: 5032 RVA: 0x00136D03 File Offset: 0x00134F03
			public int Level { get; set; }

			// Token: 0x17000052 RID: 82
			// (get) Token: 0x060013A9 RID: 5033 RVA: 0x00136D0C File Offset: 0x00134F0C
			// (set) Token: 0x060013AA RID: 5034 RVA: 0x00136D23 File Offset: 0x00134F23
			public int NeedGoodID { get; set; }

			// Token: 0x17000053 RID: 83
			// (get) Token: 0x060013AB RID: 5035 RVA: 0x00136D2C File Offset: 0x00134F2C
			// (set) Token: 0x060013AC RID: 5036 RVA: 0x00136D43 File Offset: 0x00134F43
			public int NeedPartCount { get; set; }
		}
	}
}
