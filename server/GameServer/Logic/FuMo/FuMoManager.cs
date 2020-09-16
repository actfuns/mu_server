using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.FuMo
{
	
	internal class FuMoManager : ICmdProcessorEx, ICmdProcessor
	{
		
		public static FuMoManager getInstance()
		{
			return FuMoManager.instance;
		}

		
		public static bool startup()
		{
			return true;
		}

		
		public static bool showdown()
		{
			return true;
		}

		
		public static bool destroy()
		{
			return true;
		}

		
		public void Initialize()
		{
			string fileName = Global.GameResPath("Config/EquipEnchantmentExtra.xml");
			XElement xml = XElement.Load(fileName);
			if (null == xml)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
			}
			try
			{
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					FuMoExtraTemplate data = new FuMoExtraTemplate();
					data.Condition = new List<int>();
					data.Parameter = new Dictionary<double, double>();
					data.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
					if (FuMoManager.MaxExtrasID < data.ID)
					{
						FuMoManager.MaxExtrasID = data.ID;
					}
					data.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
					string[] parem = Global.GetSafeAttributeStr(xmlItem, "Condition").Split(new char[]
					{
						','
					});
					foreach (string it in parem)
					{
						data.Condition.Add(Convert.ToInt32(it));
					}
					data.Type = Global.GetSafeAttributeStr(xmlItem, "Type");
					string[] parem2 = Global.GetSafeAttributeStr(xmlItem, "Parameter").Split(new char[]
					{
						'|'
					});
					foreach (string it in parem2)
					{
						string[] parem3 = it.Split(new char[]
						{
							','
						});
						data.Parameter.Add(Convert.ToDouble(parem3[0]), Convert.ToDouble(parem3[1]));
					}
					if (!FuMoManager.IDExtrasTypeMap.ContainsKey(data.ID))
					{
						FuMoManager.IDExtrasTypeMap.Add(data.ID, (int)ConfigParser.GetPropIndexByPropName(data.Type));
					}
					FuMoManager.FuMoExtras.Add(data.ID, data);
					FuMoManager.FuMoExtras[data.ID].Parameter = (from p in FuMoManager.FuMoExtras[data.ID].Parameter
					orderby p.Value
					select p).ToDictionary((KeyValuePair<double, double> p) => p.Key, (KeyValuePair<double, double> p) => p.Value);
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。", fileName));
			}
			try
			{
				fileName = Global.GameResPath("Config/EquipEnchantmentRandom.xml");
				XElement xml2 = XElement.Load(fileName);
				if (null == xml2)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
				}
				IEnumerable<XElement> nodes = xml2.Elements();
				foreach (XElement node in nodes)
				{
					FuMoRandomTemplate data2 = new FuMoRandomTemplate();
					List<int> weight = new List<int>();
					data2.Parameter = new Dictionary<double, double>();
					data2.ID = Convert.ToInt32(Global.GetSafeAttributeStr(node, "ID"));
					if (FuMoManager.MaxRandomID < data2.ID)
					{
						FuMoManager.MaxRandomID = data2.ID;
					}
					data2.Name = Global.GetSafeAttributeStr(node, "Name");
					data2.Type = Global.GetSafeAttributeStr(node, "Type");
					string[] parem4 = Global.GetSafeAttributeStr(node, "Parameter").Split(new char[]
					{
						'|'
					});
					foreach (string it in parem4)
					{
						string[] parem2 = it.Split(new char[]
						{
							','
						});
						data2.Parameter.Add(Convert.ToDouble(parem2[0]), Convert.ToDouble(parem2[1]));
					}
					data2.BeginNum = Convert.ToInt32(Global.GetSafeAttributeStr(node, "BeginNum"));
					data2.EndNum = Convert.ToInt32(Global.GetSafeAttributeStr(node, "EndNum"));
					FuMoManager.FuMoRandoms.Add(data2.ID, data2);
					FuMoManager.FuMoRandoms[data2.ID].Parameter = (from p in FuMoManager.FuMoRandoms[data2.ID].Parameter
					orderby p.Value
					select p).ToDictionary((KeyValuePair<double, double> p) => p.Key, (KeyValuePair<double, double> p) => p.Value);
					weight.Add(data2.BeginNum);
					weight.Add(data2.EndNum);
					if (!FuMoManager.IDMap.ContainsKey(data2.ID))
					{
						FuMoManager.IDMap.Add(data2.ID, weight);
					}
					if (!FuMoManager.IDRandomsTypeMap.ContainsKey(data2.ID))
					{
						FuMoManager.IDRandomsTypeMap.Add(data2.ID, (int)ConfigParser.GetPropIndexByPropName(data2.Type));
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。{1}", fileName, ex.ToString()));
			}
			int[] num = GameManager.systemParamsList.GetParamValueIntArrayByName("EnchantmentGiveLimit", ',');
			FuMoManager.FuMoParems.GiveNum = num[0];
			FuMoManager.FuMoParems.AcceptNum = num[1];
			FuMoManager.FuMoParems.FuMoMoneyAddNum = (int)GameManager.systemParamsList.GetParamValueIntByName("EnchantmentGiveNum", -1);
			FuMoManager.FuMoParems.FuMoMoneySubNum = -(int)GameManager.systemParamsList.GetParamValueIntByName("EnchantmentCost", -1);
			FuMoManager.FuMoParems.DailyActiveCond = (int)GameManager.systemParamsList.GetParamValueIntByName("FuMoHuoyue", -1);
			int[] use = GameManager.systemParamsList.GetParamValueIntArrayByName("EnchantmentInheritCost", ',');
			FuMoManager.FuMoParems.FuMoJinBi = use[0];
			FuMoManager.FuMoParems.FuMoZuanShi = use[1];
			double[] prob = GameManager.systemParamsList.GetParamValueDoubleArrayByName("EnchantmentRandomNum", ',');
			FuMoManager.FuMoParems.FuMoProb1 = prob[0];
			FuMoManager.FuMoParems.FuMoProb2 = prob[1];
			TCPCmdDispatcher.getInstance().registerProcessorEx(2021, 2, 2, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2022, 2, 2, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2023, 1, 1, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2024, 4, 4, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2025, 2, 2, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2026, 2, 2, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2027, 1, 1, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2028, 1, 1, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2029, 2, 2, FuMoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
		}

		
		public static bool TryFuMoTrueAttrValue(List<int> inAttr, out List<int> outAttr)
		{
			bool flag = true;
			outAttr = null;
			if (inAttr.Count > 6)
			{
				flag = false;
			}
			for (int i = 0; i < inAttr.Count - 1; i += 2)
			{
				if (inAttr[i] == 0 || inAttr[i + 1] == 0)
				{
					flag = false;
				}
				if (i < 4 && !FuMoManager.IDRandomsTypeMap.ContainsValue(inAttr[i]))
				{
					flag = false;
				}
				if (i == 4 && !FuMoManager.IDExtrasTypeMap.ContainsValue(inAttr[i]))
				{
					flag = false;
				}
				if (!flag)
				{
					outAttr = new List<int>();
					outAttr.Add(14);
					outAttr.Add(0);
					outAttr.Add(94);
					outAttr.Add(0);
					return false;
				}
			}
			return true;
		}

		
		public static bool IsSameIDFromRandom(int rand1, int rand2, out int resid)
		{
			int tempid = -1;
			int tempid2 = -2;
			resid = -1;
			foreach (KeyValuePair<int, FuMoRandomTemplate> id in FuMoManager.FuMoRandoms)
			{
				if (rand1 < id.Value.EndNum && rand1 > id.Value.BeginNum)
				{
					tempid = id.Key;
				}
				if (rand2 < id.Value.EndNum && rand2 > id.Value.BeginNum)
				{
					tempid2 = id.Key;
				}
			}
			bool result;
			if (tempid == tempid2)
			{
				resid = tempid;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public static bool FilterAttrWeight(int rAttr1, int rAttr2, out List<int> list)
		{
			list = new List<int>();
			int AttrID = 0;
			int AttrID2 = 0;
			foreach (KeyValuePair<int, List<int>> it in FuMoManager.IDMap)
			{
				if (rAttr1 >= it.Value[0] && rAttr1 <= it.Value[1])
				{
					AttrID = it.Key;
				}
				if (rAttr2 >= it.Value[0] && rAttr2 <= it.Value[1])
				{
					AttrID2 = it.Key;
				}
				if (AttrID == AttrID2 && AttrID != 1 && AttrID != 10)
				{
					int rand = Global.GetRandomNumber(0, 2);
					if (rand == 0)
					{
						AttrID2 = Global.GetRandomNumber(1, AttrID);
					}
					else
					{
						AttrID2 = Global.GetRandomNumber(AttrID, FuMoManager.MaxRandomID);
					}
				}
				else if (AttrID == AttrID2 && AttrID == 1)
				{
					AttrID2 = Global.GetRandomNumber(AttrID + 1, FuMoManager.MaxRandomID);
				}
				else if (AttrID == AttrID2 && AttrID == FuMoManager.MaxRandomID)
				{
					AttrID2 = Global.GetRandomNumber(1, FuMoManager.MaxRandomID);
				}
				if (AttrID != 0 && AttrID2 != 0)
				{
					list.Add(AttrID);
					list.Add(AttrID2);
					list.Sort();
					return true;
				}
			}
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 2021:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int mailid = Convert.ToInt32(cmdParams[0]);
					int GetType = Convert.ToInt32(cmdParams[1]);
					int ResGetNum = 0;
					int result = Convert.ToInt32(FuMoManager.ProcessGetFuMoMoneyCmd(client, mailid, GetType, out ResGetNum));
					client.sendCmd(nID, string.Format("{0}:{1}", result, ResGetNum), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1".ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_UPDATA_FUMOMONEY_ACCEPTNUM", false, false);
				}
				break;
			case 2022:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int recrid = Convert.ToInt32(cmdParams[0]);
					string recname = cmdParams[1];
					int result = Convert.ToInt32(FuMoManager.ProcessFoMoMoneyGiveCmd(client, recrid, recname));
					client.sendCmd(nID, string.Format("{0}", result), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1".ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_GIVE_FUMOMONEY", false, false);
				}
				break;
			case 2023:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int roldid = Convert.ToInt32(cmdParams[0]);
					Dictionary<int, List<FuMoMailData>> maildata;
					int restult = Convert.ToInt32(FuMoManager.ProcessGetFoMoMoneyMailListCmd(client, out maildata));
					client.sendCmd<Dictionary<int, List<FuMoMailData>>>(nID, maildata, false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1".ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_GET_FUMOMAIL_LIST", false, false);
				}
				break;
			case 2024:
				if (cmdParams == null || cmdParams.Length != 4)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					int leftGoodsDbID = Convert.ToInt32(cmdParams[1]);
					int rightGoodsDbID = Convert.ToInt32(cmdParams[2]);
					int nSubMoneyType = Convert.ToInt32(cmdParams[3]);
					int result = Convert.ToInt32(FuMoManager.ProcessSpriteEquipAppendFuMoAttrInhertCmd(client, roleID, leftGoodsDbID, rightGoodsDbID, nSubMoneyType));
					client.sendCmd(nID, string.Format("{0}", result), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1".ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_FUMOATTR_APPEND", false, false);
				}
				break;
			case 2025:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					int dbID = Convert.ToInt32(cmdParams[1]);
					FuMoCachedTemplate cached = null;
					int result = Convert.ToInt32(FuMoManager.ProcessEquipFuMoAttrAppendCmd(client, roleID, dbID, out cached));
					client.sendCmd<FuMoCachedTemplate>(nID, cached, false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1".ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_FUMOATTR", false, false);
				}
				break;
			case 2026:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					int DbID = Convert.ToInt32(cmdParams[1]);
					int result = Convert.ToInt32(FuMoManager.ProcessSaveFuMoAttrCmd(client, roleID, DbID));
					client.sendCmd(nID, string.Format("{0}", result), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1".ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_SAVE_FUMOATTR", false, false);
				}
				break;
			case 2027:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					string UserList = null;
					int result = Convert.ToInt32(FuMoManager.ProcessGiveTodayUserListCmd(client, roleID, out UserList));
					client.sendCmd(nID, string.Format("{0}:{1}", result, UserList), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1".ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_TODAY_USER_GIVE_LIST", false, false);
				}
				break;
			case 2028:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					FuMoCachedTemplate temp = null;
					int result = Convert.ToInt32(FuMoManager.ProcessNotSaveFuMoAttrCmd(client, roleID, out temp));
					client.sendCmd<FuMoCachedTemplate>(nID, temp, false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1".ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_NOT_SAVE_FUMOATTR", false, false);
				}
				break;
			case 2029:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int roleID = Convert.ToInt32(cmdParams[0]);
					int mailID = Convert.ToInt32(cmdParams[1]);
					int result = Convert.ToInt32(FuMoManager.UpdataIsReadCmd(client, roleID, mailID));
					client.sendCmd(nID, string.Format("{0}", result), false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1".ToString(), false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_UPDATA_FUMOMAIL_ISREAD", false, false);
				}
				break;
			}
			return true;
		}

		
		public static bool GetRandomValue(List<int> inList, out List<int> outList)
		{
			int count = 0;
			outList = new List<int>();
			lock (outList)
			{
				foreach (int inId in inList)
				{
					count++;
					if (count == 3)
					{
						FuMoExtraTemplate Extra = null;
						if (!FuMoManager.FuMoExtras.TryGetValue(inId, out Extra))
						{
							return false;
						}
						double random = Global.GetRandom();
						double tempExtras = 0.0;
						int ExtrasValue = 0;
						foreach (KeyValuePair<double, double> it in Extra.Parameter)
						{
							if (random < it.Value + tempExtras)
							{
								ExtrasValue = (int)(it.Key * 1000.0);
								break;
							}
							tempExtras += it.Value;
						}
						outList.Add((int)ConfigParser.GetPropIndexByPropName(Extra.Type));
						outList.Add(ExtrasValue);
						break;
					}
					else
					{
						FuMoRandomTemplate Rattr = null;
						if (!FuMoManager.FuMoRandoms.TryGetValue(inId, out Rattr))
						{
							return false;
						}
						double random = Global.GetRandom();
						double tempRandoms = 0.0;
						int RandomsValue = 0;
						foreach (KeyValuePair<double, double> it in Rattr.Parameter)
						{
							if (random < it.Value + tempRandoms)
							{
								RandomsValue = (int)(it.Key * 1000.0);
								break;
							}
							tempRandoms += it.Value;
						}
						outList.Add((int)ConfigParser.GetPropIndexByPropName(Rattr.Type));
						outList.Add(RandomsValue);
					}
				}
			}
			return true;
		}

		
		public static bool GetRandomAttrArray(out List<int> list)
		{
			list = null;
			List<int> inList = null;
			lock (FuMoManager.Mutex)
			{
				if (FuMoManager.FilterAttrWeight(Global.GetRandomNumber(0, 100000), Global.GetRandomNumber(0, 100000), out inList))
				{
					foreach (KeyValuePair<int, FuMoExtraTemplate> extr in FuMoManager.FuMoExtras)
					{
						if (extr.Value.Condition[0] == inList[0] && extr.Value.Condition[1] == inList[1])
						{
							inList.Add(extr.Key);
							break;
						}
					}
				}
			}
			return FuMoManager.GetRandomValue(inList, out list);
		}

		
		private static TCPProcessCmdResults UpdateMailFuMoData2DB(GameClient client, int recrid, int num, string content, string recname)
		{
			string[] dbFields = null;
			string sCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.RoleName,
				recrid,
				num,
				content,
				client.ClientData.Occupation
			});
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14102, sCmd, out dbFields, client.ServerId);
		}

		
		private static TCPProcessCmdResults UpdateMailFuMoGiveNumData2DB(GameClient client, string recrid_list, int nDate, int accept, int give)
		{
			string[] dbFields = null;
			string sCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				recrid_list,
				nDate,
				accept,
				give
			});
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14104, sCmd, out dbFields, client.ServerId);
		}

		
		private static TCPProcessCmdResults UpdateFuMoMailMap2DB(GameClient client, int rid, int give, int nDate, string recid_list)
		{
			string[] dbFields = null;
			string sCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				rid,
				nDate,
				give,
				recid_list
			});
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14108, sCmd, out dbFields, client.ServerId);
		}

		
		private static TCPProcessCmdResults UpdateFuMoAcceptNum(GameClient client, int nDate, int num)
		{
			string[] dbFields = null;
			string sCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, nDate, num);
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14101, sCmd, out dbFields, client.ServerId);
		}

		
		private static TCPProcessCmdResults DeleteFuMoMail(GameClient client, int mailid)
		{
			string[] dbFields = null;
			string sCmd = string.Format("{0}:{1}", client.ClientData.RoleID, mailid);
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14109, sCmd, out dbFields, client.ServerId);
		}

		
		private static TCPProcessCmdResults DeleteFuMoMailList(GameClient client, string mailid_list)
		{
			string[] dbFields = null;
			string sCmd = string.Format("{0}:{1}", client.ClientData.RoleID, mailid_list);
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14110, sCmd, out dbFields, client.ServerId);
		}

		
		private static TCPProcessCmdResults UpdateMailState(GameClient client, int mailid)
		{
			string[] dbFields = null;
			string sCmd = string.Format("{0}:{1}", client.ClientData.RoleID, mailid);
			return Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14111, sCmd, out dbFields, client.ServerId);
		}

		
		public static FuMoMailOptEnum GiveFuMoMoneyToDB(GameClient client, int recrid, string recname)
		{
			FuMoMailOptEnum result;
			if (Global.FindFriendData(client, recrid) == null)
			{
				result = FuMoMailOptEnum.FuMo_NotFriend;
			}
			else
			{
				bool flag = false;
				GameClient otherClient = GameManager.ClientMgr.FindClient(recrid);
				if (otherClient == null)
				{
					otherClient = new GameClient();
					otherClient.ClientData = Global.GetSafeClientDataFromLocalOrDB(recrid);
				}
				else
				{
					flag = true;
				}
				if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.FuMo, false))
				{
					result = FuMoMailOptEnum.FuMo_GongNengWeiKaiQi;
				}
				else if (!GlobalNew.IsGongNengOpened(otherClient, GongNengIDs.FuMo, false))
				{
					result = FuMoMailOptEnum.FuMo_OtherGongNengWeiKaiQi;
				}
				else
				{
					int daily = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveValue);
					if (daily < FuMoManager.FuMoParems.DailyActiveCond)
					{
						result = FuMoMailOptEnum.FuMo_GiveFuMoMoneyActive;
					}
					else
					{
						string strdb = string.Format("{0}:{1}", client.ClientData.RoleID, TimeUtil.GetOffsetDayNow());
						Dictionary<int, FuMoMailTemp> list = Global.sendToDB<Dictionary<int, FuMoMailTemp>, string>(14107, strdb, 0);
						FuMoMailTemp OutTemp = null;
						lock (list)
						{
							if (list.TryGetValue(client.ClientData.RoleID, out OutTemp))
							{
								if (OutTemp.Give >= FuMoManager.FuMoParems.GiveNum)
								{
									return FuMoMailOptEnum.FuMo_GiveFuMoMoneyMax;
								}
								string[] RecridList = OutTemp.ReceiverRID.Split(new char[]
								{
									'_'
								});
								foreach (string id in RecridList)
								{
									if (id != "" && Convert.ToInt32(id) == recrid)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("附魔币赠送当日已经总送过了, RoleID={0}", client.ClientData.RoleID), null, true);
										return FuMoMailOptEnum.FuMo_GiveFuMoMoneyRepeat;
									}
								}
								string UpReceiverList = string.Format("{0}_{1}", recrid, OutTemp.ReceiverRID);
								int CurrDay = TimeUtil.GetOffsetDayNow();
								OutTemp.Give++;
								if (TCPProcessCmdResults.RESULT_DATA != FuMoManager.UpdateFuMoMailMap2DB(client, OutTemp.SenderRID, OutTemp.Give, CurrDay, UpReceiverList))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("更新映射表出错, RoleID={0}", client.ClientData.RoleID), null, true);
									return FuMoMailOptEnum.FuMo_RunFuMoDBError;
								}
							}
							else if (TCPProcessCmdResults.RESULT_DATA != FuMoManager.UpdateMailFuMoGiveNumData2DB(client, recrid.ToString(), TimeUtil.GetOffsetDayNow(), 0, 1))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("修改赠送次数出错, RoleID={0}", client.ClientData.RoleID), null, true);
								return FuMoMailOptEnum.FuMo_RunFuMoDBError;
							}
						}
						if (TCPProcessCmdResults.RESULT_DATA != FuMoManager.UpdateMailFuMoData2DB(client, recrid, FuMoManager.FuMoParems.FuMoMoneyAddNum, GLang.GetLang(6003, new object[0]), recname))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("插入新的赠送附魔灵石邮件出错, RoleID={0}", client.ClientData.RoleID), null, true);
							result = FuMoMailOptEnum.FuMo_RunFuMoDBError;
						}
						else
						{
							if (flag)
							{
								otherClient._IconStateMgr.CheckFuMoMailIcon(otherClient);
							}
							EventLogManager.AddRoleEvent(client, OpTypes.FuMo, OpTags.FuMoMail, LogRecordType.FuMoMail, new object[]
							{
								client.ClientData.RoleID,
								recrid,
								FuMoManager.FuMoParems.FuMoMoneyAddNum,
								"赠送给对方附魔灵石"
							});
							result = FuMoMailOptEnum.FuMo_AcceptSucc;
						}
					}
				}
			}
			return result;
		}

		
		private static FuMoMailOptEnum ProcessFoMoMoneyGiveCmd(GameClient client, int recrid, string recname)
		{
			return FuMoManager.GiveFuMoMoneyToDB(client, recrid, recname);
		}

		
		private static FuMoMailOptEnum ProcessGetFoMoMoneyMailListCmd(GameClient client, out Dictionary<int, List<FuMoMailData>> maildata)
		{
			maildata = null;
			maildata = Global.sendToDB<Dictionary<int, List<FuMoMailData>>, string>(14106, client.ClientData.RoleID.ToString(), 0);
			client._IconStateMgr.CheckFuMoMailIcon(client);
			return FuMoMailOptEnum.FuMo_AcceptSucc;
		}

		
		private static FuMoMailOptEnum ProcessGetFuMoMoneyCmd(GameClient client, int mailid, int GetType, out int ResGetNum)
		{
			ResGetNum = 0;
			string strdb = string.Format("{0}:{1}", client.ClientData.RoleID, TimeUtil.GetOffsetDayNow());
			string[] fields = null;
			TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14105, strdb, out fields, client.ServerId);
			FuMoMailOptEnum result;
			if (retcmd == TCPProcessCmdResults.RESULT_FAILED)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("获取接受次数出错, RoleID={0}", client.ClientData.RoleID), null, true);
				result = FuMoMailOptEnum.FuMo_GetAcceptError;
			}
			else
			{
				int num = Convert.ToInt32(fields[1]);
				int resid = Convert.ToInt32(fields[0]);
				if (resid != client.ClientData.RoleID)
				{
					result = FuMoMailOptEnum.FuMo_Fail;
				}
				else if (num >= FuMoManager.FuMoParems.AcceptNum)
				{
					result = FuMoMailOptEnum.FuMo_AcceptMaxTime;
				}
				else
				{
					Dictionary<int, List<FuMoMailData>> MailData = Global.sendToDB<Dictionary<int, List<FuMoMailData>>, string>(14106, client.ClientData.RoleID.ToString(), 0);
					if (MailData == null)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("查找玩家的附魔灵石邮件列表为空, RoleID={0}", client.ClientData.RoleID), null, true);
						result = FuMoMailOptEnum.FuMo_GetMailListError;
					}
					else
					{
						lock (MailData)
						{
							List<FuMoMailData> FMMailDate = new List<FuMoMailData>();
							if (!MailData.TryGetValue(client.ClientData.RoleID, out FMMailDate))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("查找玩家的附魔灵石邮件列表为空, RoleID={0}", client.ClientData.RoleID), null, true);
								return FuMoMailOptEnum.FuMo_GetMailListError;
							}
							if (GetType == 0)
							{
								if (FMMailDate == null)
								{
									return FuMoMailOptEnum.FuMo_Fail;
								}
								bool flag = true;
								foreach (FuMoMailData it in FMMailDate)
								{
									if (it.MaillID == mailid)
									{
										int nDate = TimeUtil.GetOffsetDayNow();
										if (num == -1)
										{
											if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.UpdateMailFuMoGiveNumData2DB(client, "", nDate, 1, 0))
											{
												LogManager.WriteLog(LogTypes.Error, string.Format("领取插入映射数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
												return FuMoMailOptEnum.FuMo_DBError;
											}
										}
										else if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.UpdateFuMoAcceptNum(client, nDate, num + 1))
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("领取更新映射数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
											return FuMoMailOptEnum.FuMo_DBError;
										}
										if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.DeleteFuMoMail(client, it.MaillID))
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("领取删除邮件数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
											return FuMoMailOptEnum.FuMo_DBError;
										}
										if (!GameManager.ClientMgr.ModifyFuMoLingShiValue(client, FuMoManager.FuMoParems.FuMoMoneyAddNum, "附魔邮件领取", true, true, false))
										{
											return FuMoMailOptEnum.FuMo_MoneyError;
										}
										flag = false;
										ResGetNum = 1;
										if (null != client)
										{
											client._IconStateMgr.CheckFuMoMailIcon(client);
										}
										EventLogManager.AddRoleEvent(client, OpTypes.FuMo, OpTags.FuMoMail, LogRecordType.FuMoMail, new object[]
										{
											client.ClientData.RoleID,
											client.ClientData.RoleID,
											FuMoManager.FuMoParems.FuMoMoneySubNum,
											"手动领取附魔灵石邮件"
										});
										return FuMoMailOptEnum.FuMo_AcceptSucc;
									}
								}
								if (flag)
								{
									LogManager.WriteLog(LogTypes.Info, string.Format("没有找到当前玩家的附魔灵石邮件id, RoleID={0} MailID={1}", client.ClientData.RoleID, mailid), null, true);
									return FuMoMailOptEnum.FuMo_NotFriend;
								}
							}
							else
							{
								if (GetType != 1)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("领取附魔灵石操作类型出错, RoleID={0} Type={1}", client.ClientData.RoleID), null, true);
									return FuMoMailOptEnum.FuMo_NotFriend;
								}
								if (FMMailDate == null)
								{
									return FuMoMailOptEnum.FuMo_Fail;
								}
								bool flag = false;
								int accept;
								if (num == -1)
								{
									flag = true;
									accept = FuMoManager.FuMoParems.AcceptNum;
								}
								else
								{
									accept = FuMoManager.FuMoParems.AcceptNum - num;
								}
								int temp = 0;
								int addValue = 0;
								string removeMailIdList = null;
								foreach (FuMoMailData it in FMMailDate)
								{
									if (temp >= accept)
									{
										break;
									}
									addValue += FuMoManager.FuMoParems.FuMoMoneyAddNum;
									removeMailIdList = string.Format("{0}_{1}", it.MaillID, removeMailIdList);
									temp++;
								}
								int nDate = TimeUtil.GetOffsetDayNow();
								if (flag)
								{
									if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.UpdateMailFuMoGiveNumData2DB(client, "", nDate, temp, 0))
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("批量领取插入映射数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
										return FuMoMailOptEnum.FuMo_DBError;
									}
								}
								else if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.UpdateFuMoAcceptNum(client, nDate, num + temp))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("批量领取更新映射数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
									return FuMoMailOptEnum.FuMo_DBError;
								}
								ResGetNum = temp;
								if (TCPProcessCmdResults.RESULT_FAILED == FuMoManager.DeleteFuMoMailList(client, removeMailIdList))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("批量领取删除邮件数据出错, RoleID={0}", client.ClientData.RoleID), null, true);
									return FuMoMailOptEnum.FuMo_DBError;
								}
								if (!GameManager.ClientMgr.ModifyFuMoLingShiValue(client, addValue, "附魔邮件领取", true, true, false))
								{
									return FuMoMailOptEnum.FuMo_MoneyError;
								}
								if (null != client)
								{
									client._IconStateMgr.CheckFuMoMailIcon(client);
								}
								EventLogManager.AddRoleEvent(client, OpTypes.FuMo, OpTags.FuMoMail, LogRecordType.FuMoMail, new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleID,
									addValue,
									"一键领取附魔灵石邮件"
								});
								return FuMoMailOptEnum.FuMo_AcceptSucc;
							}
						}
						result = FuMoMailOptEnum.FuMo_Fail;
					}
				}
			}
			return result;
		}

		
		private static FuMoMailOptEnum ProcessEquipFuMoAttrAppendCmd(GameClient client, int roleID, int DbID, out FuMoCachedTemplate FMCached)
		{
			FMCached = new FuMoCachedTemplate
			{
				Result = -1,
				RoleID = roleID,
				DbID = DbID,
				AttrTypeValue = null
			};
			FuMoMailOptEnum result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.FuMo, false))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("附魔玩家功能未开启, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
				FMCached.Result = 19;
				result = FuMoMailOptEnum.FuMo_GongNengWeiKaiQi;
			}
			else if (Global.GetRoleParamsInt32FromDB(client, "10217") < Math.Abs(FuMoManager.FuMoParems.FuMoMoneySubNum))
			{
				FMCached.Result = 15;
				result = FuMoMailOptEnum.FuMo_MoneyError;
			}
			else
			{
				GoodsData equipData = Global.GetGoodsByDbID(client, DbID);
				if (equipData == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("没有找到附魔物品,RoleID={0} ", client.ClientData.RoleID), null, true);
					FMCached.Result = 11;
					result = FuMoMailOptEnum.FuMo_GetGoodInfo;
				}
				else if (!GoodsUtil.IsEquip(equipData.GoodsID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("不属于可附魔类型装备,RoleID={0}, Goodid = {1}", client.ClientData.RoleID, equipData.GoodsID), null, true);
					FMCached.Result = 17;
					result = FuMoMailOptEnum.FuMo_NotFuMoType;
				}
				else
				{
					List<int> list = null;
					if (!FuMoManager.GetRandomAttrArray(out list))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("获取随机属性出错,RoleID={0}, Goodid = {1}", client.ClientData.RoleID, equipData.GoodsID), null, true);
						FMCached.Result = 20;
						result = FuMoMailOptEnum.FuMo_GetRandomAttrError;
					}
					else
					{
						List<int> outList = null;
						if (!FuMoManager.TryFuMoTrueAttrValue(list, out outList))
						{
							list = outList;
						}
						lock (FuMoManager.FuMoCached)
						{
							if (FuMoManager.FuMoCached.ContainsKey(client.ClientData.RoleID))
							{
								if (!FuMoManager.FuMoCached.Remove(client.ClientData.RoleID))
								{
									FMCached.Result = -1;
									return FuMoMailOptEnum.FuMo_Fail;
								}
							}
						}
						lock (list)
						{
							if (null == equipData.ElementhrtsProps)
							{
								UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
								{
									RoleID = client.ClientData.RoleID,
									DbID = DbID
								};
								updateGoodsArgs.ElementhrtsProps = list;
								Global.UpdateGoodsProp(client, equipData, updateGoodsArgs, true);
								if (equipData.Using > 0)
								{
									Global.RefreshEquipPropAndNotify(client);
								}
							}
							FMCached.Result = 1;
							FMCached.RoleID = client.ClientData.RoleID;
							FMCached.DbID = DbID;
							FMCached.AttrTypeValue = list;
							FuMoManager.FuMoCached.Add(client.ClientData.RoleID, FMCached);
						}
						if (!GameManager.ClientMgr.ModifyFuMoLingShiValue(client, FuMoManager.FuMoParems.FuMoMoneySubNum, "附魔消耗", true, true, false))
						{
							result = FuMoMailOptEnum.FuMo_MoneyError;
						}
						else
						{
							GameManager.logDBCmdMgr.AddDBLogInfo(DbID, "装备附魔", "装备附魔操作", client.ClientData.RoleName, "系统", "添加附魔属性", 0, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, equipData);
							result = FuMoMailOptEnum.FuMo_AcceptSucc;
						}
					}
				}
			}
			return result;
		}

		
		private static FuMoMailOptEnum ProcessSaveFuMoAttrCmd(GameClient client, int roleID, int DbID)
		{
			GoodsData equipData = Global.GetGoodsByDbID(client, DbID);
			FuMoMailOptEnum result;
			if (equipData == null)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("没有找到附魔物品,RoleID={0} DbID={1} ", client.ClientData.RoleID, DbID), null, true);
				result = FuMoMailOptEnum.FuMo_GetGoodInfo;
			}
			else
			{
				FuMoCachedTemplate temp = null;
				if (!FuMoManager.FuMoCached.TryGetValue(roleID, out temp))
				{
					result = FuMoMailOptEnum.FuMo_Fail;
				}
				else
				{
					lock (temp)
					{
						if (temp.RoleID != roleID && temp.DbID != DbID)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("玩家对应装备附魔保存出错 id RoleID={0} 缓存DbID {1} 参数DbID{2}", client.ClientData.RoleID, temp.DbID, DbID), null, true);
							return FuMoMailOptEnum.FuMo_SaveError;
						}
						if (temp.Result != 1)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("缓存的属性是错误的 id RoleID={0} 缓存DbID {1} 参数DbID{2}", client.ClientData.RoleID, temp.DbID, DbID), null, true);
							return FuMoMailOptEnum.FuMo_SaveError;
						}
						UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
						{
							RoleID = client.ClientData.RoleID,
							DbID = DbID
						};
						updateGoodsArgs.ElementhrtsProps = temp.AttrTypeValue;
						Global.UpdateGoodsProp(client, equipData, updateGoodsArgs, true);
						if (!FuMoManager.FuMoCached.Remove(client.ClientData.RoleID))
						{
							return FuMoMailOptEnum.FuMo_Fail;
						}
						if (equipData.Using > 0)
						{
							Global.RefreshEquipPropAndNotify(client);
						}
					}
					result = FuMoMailOptEnum.FuMo_AcceptSucc;
				}
			}
			return result;
		}

		
		private static FuMoMailOptEnum ProcessNotSaveFuMoAttrCmd(GameClient client, int roleID, out FuMoCachedTemplate temp)
		{
			temp = null;
			FuMoCachedTemplate data = null;
			FuMoMailOptEnum result;
			if (FuMoManager.FuMoCached.TryGetValue(roleID, out data))
			{
				GoodsData equipData = Global.GetGoodsByDbID(client, data.DbID);
				if (equipData == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("没有找到附魔物品,RoleID={0} DbID={1} ", client.ClientData.RoleID, data.DbID), null, true);
					result = FuMoMailOptEnum.FuMo_GetGoodInfo;
				}
				else
				{
					temp = data;
					result = FuMoMailOptEnum.FuMo_AcceptSucc;
				}
			}
			else
			{
				result = FuMoMailOptEnum.FuMo_RoleInfoError;
			}
			return result;
		}

		
		private static FuMoMailOptEnum ProcessSpriteEquipAppendFuMoAttrInhertCmd(GameClient client, int roleID, int leftGoodsDbID, int rightGoodsDbID, int nSubMoneyType)
		{
			GoodsData leftGoodsData = Global.GetGoodsByDbID(client, leftGoodsDbID);
			FuMoMailOptEnum result;
			if (null == leftGoodsData)
			{
				LogManager.WriteLog(LogTypes.Info, string.Format("获取物品信息出错, RoleID={0}", client.ClientData.RoleID), null, true);
				result = FuMoMailOptEnum.FuMo_GetGoodInfo;
			}
			else if (!GoodsUtil.IsEquip(leftGoodsData.GoodsID))
			{
				result = FuMoMailOptEnum.FuMo_NotFuMoType;
			}
			else
			{
				GoodsData rightGoodsData = Global.GetGoodsByDbID(client, rightGoodsDbID);
				if (null == rightGoodsData)
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("获取物品信息出错, RoleID={0}", client.ClientData.RoleID), null, true);
					result = FuMoMailOptEnum.FuMo_GetGoodInfo;
				}
				else if (!GoodsUtil.IsEquip(rightGoodsData.GoodsID))
				{
					result = FuMoMailOptEnum.FuMo_NotFuMoType;
				}
				else
				{
					int OccupationLeft = Global.GetMainOccupationByGoodsID(leftGoodsData.GoodsID);
					int OccupationRight = Global.GetMainOccupationByGoodsID(rightGoodsData.GoodsID);
					if (OccupationLeft != OccupationRight)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("装备职业不同, RoleID={0}", client.ClientData.RoleID), null, true);
						result = FuMoMailOptEnum.FuMo_EquipJobDiff;
					}
					else
					{
						int categoryLeft = Global.GetGoodsCatetoriy(leftGoodsData.GoodsID);
						int categoryRight = Global.GetGoodsCatetoriy(rightGoodsData.GoodsID);
						int ret = GoodsUtil.CanUpgradeInhert(categoryLeft, categoryRight, 14);
						if (ret < 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("装备左右类型不同, RoleID={0}", client.ClientData.RoleID), null, true);
							result = FuMoMailOptEnum.FuMo_LiftRightEquipDiff;
						}
						else if (leftGoodsData.Site != 0 || rightGoodsData.Site != 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("装备不在背包拒绝操作, RoleID={0}", client.ClientData.RoleID), null, true);
							result = FuMoMailOptEnum.FuMo_EquipNotInGoods;
						}
						else if (nSubMoneyType < 1 || nSubMoneyType > 2)
						{
							LogManager.WriteLog(LogTypes.Info, string.Format("参数使用货币类型出错, RoleID={0}", client.ClientData.RoleID), null, true);
							result = FuMoMailOptEnum.FuMo_Fail;
						}
						else
						{
							if (nSubMoneyType == 1)
							{
								if (!Global.SubBindTongQianAndTongQian(client, FuMoManager.FuMoParems.FuMoJinBi, "装备附魔传承"))
								{
									return FuMoMailOptEnum.FuMo_JinBiLacking;
								}
							}
							else if (nSubMoneyType == 2)
							{
								if (!GameManager.ClientMgr.SubUserMoney(client, FuMoManager.FuMoParems.FuMoZuanShi, "装备附魔传承", true, true, true, true, DaiBiSySType.None))
								{
									return FuMoMailOptEnum.FuMo_ZuanShiLacking;
								}
							}
							if (leftGoodsData.ElementhrtsProps.Count <= 0)
							{
								LogManager.WriteLog(LogTypes.Info, string.Format("左边没有附魔属性, RoleID={0}", client.ClientData.RoleID), null, true);
								result = FuMoMailOptEnum.FuMo_FuMoAttrError;
							}
							else
							{
								int nBinding = 0;
								if (rightGoodsData.Binding == 1 || leftGoodsData.Binding == 1)
								{
									nBinding = 1;
								}
								List<int> temp = new List<int>();
								lock (temp)
								{
									temp.AddRange(leftGoodsData.ElementhrtsProps);
									if (temp.Count < 4)
									{
										return FuMoMailOptEnum.FuMo_FuMoAttrError;
									}
									UpdateGoodsArgs updateGoodsArgsLeft = new UpdateGoodsArgs
									{
										RoleID = client.ClientData.RoleID,
										DbID = leftGoodsDbID
									};
									updateGoodsArgsLeft.ElementhrtsProps = null;
									if (Global.UpdateGoodsProp(client, leftGoodsData, updateGoodsArgsLeft, true) < 0)
									{
										return FuMoMailOptEnum.FuMo_FuMoAttrError;
									}
									UpdateGoodsArgs updateGoodsArgsRight = new UpdateGoodsArgs
									{
										RoleID = client.ClientData.RoleID,
										DbID = rightGoodsDbID
									};
									updateGoodsArgsRight.Binding = nBinding;
									updateGoodsArgsRight.ElementhrtsProps = new List<int>(temp);
									if (Global.UpdateGoodsProp(client, rightGoodsData, updateGoodsArgsRight, true) < 0)
									{
										return FuMoMailOptEnum.FuMo_FuMoAttrError;
									}
								}
								Global.ModRoleGoodsEvent(client, leftGoodsData, 0, "装备附魔传承_提供方", false);
								Global.ModRoleGoodsEvent(client, rightGoodsData, 0, "装备附魔传承_接受方", false);
								EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, leftGoodsData.GoodsID, (long)leftGoodsData.Id, 0, leftGoodsData.GCount, "装备附魔传承_提供方");
								EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, rightGoodsData.GoodsID, (long)rightGoodsData.Id, 0, rightGoodsData.GCount, "装备附魔传承_接受方");
								ProcessTask.ProcessAddTaskVal(client, TaskTypes.EquipChuanCheng, -1, 1, new object[0]);
								Global.BroadcastAppendChuanChengOk(client, leftGoodsData, rightGoodsData);
								if (leftGoodsData.Using > 0 || rightGoodsData.Using > 0)
								{
									Global.RefreshEquipPropAndNotify(client);
								}
								result = FuMoMailOptEnum.FuMo_AcceptSucc;
							}
						}
					}
				}
			}
			return result;
		}

		
		private static FuMoMailOptEnum ProcessGiveTodayUserListCmd(GameClient client, int roleID, out string UserList)
		{
			UserList = null;
			string strdb = string.Format("{0}:{1}", client.ClientData.RoleID, TimeUtil.GetOffsetDayNow());
			Dictionary<int, FuMoMailTemp> list = Global.sendToDB<Dictionary<int, FuMoMailTemp>, string>(14107, strdb, 0);
			FuMoMailTemp OutTemp = null;
			lock (list)
			{
				if (list.TryGetValue(client.ClientData.RoleID, out OutTemp))
				{
					UserList = OutTemp.ReceiverRID;
				}
			}
			return FuMoMailOptEnum.FuMo_AcceptSucc;
		}

		
		private static FuMoMailOptEnum UpdataIsReadCmd(GameClient client, int roleID, int mailID)
		{
			string strdb = string.Format("{0}:{1}", roleID, mailID);
			string[] fields = null;
			TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14111, strdb, out fields, client.ServerId);
			FuMoMailOptEnum result;
			if (retcmd == TCPProcessCmdResults.RESULT_FAILED)
			{
				result = FuMoMailOptEnum.FuMo_Fail;
			}
			else
			{
				result = FuMoMailOptEnum.FuMo_AcceptSucc;
			}
			return result;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public static Dictionary<int, FuMoExtraTemplate> FuMoExtras = new Dictionary<int, FuMoExtraTemplate>();

		
		public static Dictionary<int, FuMoRandomTemplate> FuMoRandoms = new Dictionary<int, FuMoRandomTemplate>();

		
		public static FuMoParemLimit FuMoParems = new FuMoParemLimit();

		
		public static Dictionary<int, List<int>> IDMap = new Dictionary<int, List<int>>();

		
		public static Dictionary<int, FuMoCachedTemplate> FuMoCached = new Dictionary<int, FuMoCachedTemplate>();

		
		public static Dictionary<int, int> IDRandomsTypeMap = new Dictionary<int, int>();

		
		public static Dictionary<int, int> IDExtrasTypeMap = new Dictionary<int, int>();

		
		public static int MaxRandomID = 0;

		
		public static int MaxExtrasID = 0;

		
		public static object Mutex = new object();

		
		private static FuMoManager instance = new FuMoManager();
	}
}
