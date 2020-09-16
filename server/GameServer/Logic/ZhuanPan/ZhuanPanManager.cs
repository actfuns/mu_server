using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.Reborn;
using GameServer.Server;
using GameServer.Tools;
using Server.Tools;

namespace GameServer.Logic.ZhuanPan
{
	
	public class ZhuanPanManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static ZhuanPanManager getInstance()
		{
			return ZhuanPanManager.instance;
		}

		
		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1810, 1, 1, ZhuanPanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1811, 2, 2, ZhuanPanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1813, 1, 1, ZhuanPanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		
		public bool LoadConfig()
		{
			try
			{
				if (!this.LoadSystemParams())
				{
					return false;
				}
				if (!this.LoadZhuanPan())
				{
					return false;
				}
				if (!this.LoadZhuanPanAward())
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("转盘系统读取配置表出错", new object[0]), null, true);
			}
			return true;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1810:
					return this.ProcessZhuanPanInfoCmd(client, nID, bytes, cmdParams);
				case 1811:
					return this.ProcessZhuanPanChouJiangCmd(client, nID, bytes, cmdParams);
				case 1813:
					return this.ProcessZhuanPanLingJiangCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		
		public bool ProcessZhuanPanInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				List<ZhuanPanItem> zhuanPanList = new List<ZhuanPanItem>();
				DateTime lastChouJiang = Global.GetRoleParamsDateTimeFromDB(client, "10155");
				DateTime nextChouJiang = DateTime.MaxValue;
				int chouJiangFuLiCont = Global.GetRoleParamsInt32FromDB(client, "10156");
				if (lastChouJiang < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime)
				{
					lastChouJiang = ZhuanPanManager.ZhuanPanRunTimeData.BeginTime;
					Global.SaveRoleParamsDateTimeToDB(client, "10155", lastChouJiang, true);
				}
				int goodsIndex = Global.GetRoleParamsInt32FromDB(client, "10162") - 1;
				ZhuanPanMainData zhuanPanData = null;
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					zhuanPanList = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanItemXmlList;
					int addHours = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanFree;
					int[] zhuanPanXiaoHaoArr = new int[ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray.Count * 2];
					for (int i = 0; i < ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray.Count; i++)
					{
						zhuanPanXiaoHaoArr[i * 2] = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray[i][0];
						zhuanPanXiaoHaoArr[i * 2 + 1] = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray[i][1];
					}
					if (addHours > 0)
					{
						nextChouJiang = lastChouJiang.AddHours((double)addHours);
					}
					if (chouJiangFuLiCont < 1 || chouJiangFuLiCont > ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi)
					{
						chouJiangFuLiCont = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi;
						Global.SaveRoleParamsInt32ValueToDB(client, "10156", chouJiangFuLiCont, true);
					}
					DateTime chouJiangTime = Global.GetRoleParamsDateTimeFromDB(client, "10165");
					ZhuanPanItem goodsAward;
					if (chouJiangTime < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime)
					{
						goodsAward = null;
						chouJiangFuLiCont = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi;
						Global.SaveRoleParamsInt32ValueToDB(client, "10156", chouJiangFuLiCont, true);
					}
					else
					{
						goodsAward = ((goodsIndex < 0 || goodsIndex >= zhuanPanList.Count) ? null : zhuanPanList[goodsIndex]);
					}
					ZhuanPanItem goodsRealAward = null;
					if (goodsAward != null)
					{
						int binding = Global.GetRoleParamsInt32FromDB(client, "10166");
						string[] goods = goodsAward.GoodsID.Split(new char[]
						{
							','
						});
						goods[2] = binding.ToString();
						goodsRealAward = new ZhuanPanItem
						{
							ID = goodsAward.ID,
							GoodsID = string.Join(",", goods),
							AwardLevel = goodsAward.AwardLevel,
							GongGao = goodsAward.GongGao,
							AwardLabel = goodsAward.AwardLabel
						};
					}
					zhuanPanData = new ZhuanPanMainData
					{
						ZhuanPanAwardItemList = zhuanPanList,
						FreeTime = nextChouJiang,
						LeftFuLiCount = chouJiangFuLiCont,
						ZhuanPanCostArray = zhuanPanXiaoHaoArr,
						GoodsAward = goodsRealAward,
						GongGaoList = ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList
					};
				}
				client.sendCmd<ZhuanPanMainData>(nID, zhuanPanData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessZhuanPanChouJiangCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int awardType = Convert.ToInt32(cmdParams[1]);
				DateTime now = TimeUtil.NowDateTime();
				ZhuanPanChouJiangData data = new ZhuanPanChouJiangData();
				List<ZhuanPanItem> zhuanPanList = new List<ZhuanPanItem>();
				int awardID = 0;
				int binding = 1;
				data.AwardType = awardType;
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					int addHours = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanFree;
					int moneyTypeIndex = awardType - 1;
					if (moneyTypeIndex < 0 || moneyTypeIndex >= ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray.Count)
					{
						data.Result = -200;
						client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
						return true;
					}
					int moneyType = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray[moneyTypeIndex][0];
					int subMoney = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray[moneyTypeIndex][1];
					if (moneyType <= 0 || subMoney <= 0)
					{
						data.Result = -200;
						client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
						return true;
					}
					int fuLiCount = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi;
					Dictionary<int, Dictionary<int, ZhuanPanAwardItem>> zhuanPanAwardDict = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanAwardXmlDict;
					zhuanPanList = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanItemXmlList;
					if (now < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime || now > ZhuanPanManager.ZhuanPanRunTimeData.EndTime)
					{
						data.Result = -100;
						client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
						return true;
					}
					DateTime chouJiangTime = Global.GetRoleParamsDateTimeFromDB(client, "10165");
					if (chouJiangTime < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime)
					{
						chouJiangTime = ZhuanPanManager.ZhuanPanRunTimeData.BeginTime;
						Global.SaveRoleParamsDateTimeToDB(client, "10165", chouJiangTime, true);
						Global.SaveRoleParamsInt32ValueToDB(client, "10162", 0, true);
					}
					DateTime nextChouJiang = DateTime.MaxValue;
					if (addHours > 0)
					{
						nextChouJiang = Global.GetRoleParamsDateTimeFromDB(client, "10155").AddHours((double)addHours);
					}
					if (!Global.CanAddGoodsNum(client, 1) || !RebornEquip.CanAddGoodsNum(client, 1))
					{
						data.Result = -4;
						client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
						return true;
					}
					awardID = Global.GetRoleParamsInt32FromDB(client, "10162");
					if (awardID > 0)
					{
						data.Result = -202;
						client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
						return true;
					}
					Dictionary<int, ZhuanPanAwardItem> zhuanPanAwardItemDict = null;
					if (!zhuanPanAwardDict.TryGetValue(awardType, out zhuanPanAwardItemDict))
					{
						data.Result = -101;
						client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
						return true;
					}
					int roleFuliCout = Global.GetRoleParamsInt32FromDB(client, "10156");
					data.LeftFuLiCount = roleFuliCout;
					data.FreeTime = nextChouJiang;
					bool free = false;
					if (awardType == 3)
					{
						if (!zhuanPanAwardDict.ContainsKey(4))
						{
							data.Result = -101;
							client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
							return true;
						}
						if (now > nextChouJiang)
						{
							data.FreeTime = now.AddHours((double)addHours);
							free = true;
						}
					}
					if (!free)
					{
						if (!MoneyUtil.CheckHasMoney(client, moneyType, subMoney))
						{
							data.Result = -awardType;
							client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
							return true;
						}
						string strCostList = "";
						if (!MoneyUtil.CostMoney(client, moneyType, subMoney, ref strCostList, "转盘抽奖", true))
						{
							data.Result = -awardType;
							client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
							return true;
						}
					}
					if (awardType == 3)
					{
						if (now > nextChouJiang)
						{
							Global.SaveRoleParamsDateTimeToDB(client, "10155", now, true);
							data.FreeTime = now.AddHours((double)addHours);
						}
						else
						{
							binding = 0;
							roleFuliCout--;
							if (roleFuliCout < 1)
							{
								if (!zhuanPanAwardDict.TryGetValue(4, out zhuanPanAwardItemDict))
								{
									data.Result = -101;
									client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
									return true;
								}
								roleFuliCout = fuLiCount;
								awardType = 4;
							}
							if (roleFuliCout > ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi)
							{
								roleFuliCout = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi;
							}
							Global.SaveRoleParamsInt32ValueToDB(client, "10156", roleFuliCout, true);
							data.LeftFuLiCount = roleFuliCout;
						}
					}
					int random = Global.GetRandomNumber(1, 100000);
					foreach (KeyValuePair<int, ZhuanPanAwardItem> item in zhuanPanAwardItemDict)
					{
						if (random >= item.Value.StartValue && random <= item.Value.EndValue)
						{
							awardID = item.Key;
						}
					}
					if (zhuanPanList.Count < awardID || awardID <= 0)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("转盘抽奖随机出的awardID={0}找不到对应的奖励配置", awardID), null, true);
						data.Result = -201;
						client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
						return true;
					}
					data.Result = 1;
					ZhuanPanItem award = zhuanPanList[awardID - 1];
					SystemXmlItem systemGoods = null;
					int goodID = Convert.ToInt32(award.GoodsID.Split(new char[]
					{
						','
					})[0]);
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodID, out systemGoods))
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("转盘抽奖随机出的goodID={0}道具表中不存在", goodID), null, true);
						string strinfo = string.Format("系统中不存在{0}", goodID);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						data.Result = -201;
						client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
						return true;
					}
					string goodName = systemGoods.GetStringValue("Title");
					if (awardType == 3 && binding > 0)
					{
						awardType = 4;
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, goodName, "转盘抽奖_类型：" + awardType, client.ClientData.RoleName, "系统", "修改", -1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
					string[] goods = award.GoodsID.Split(new char[]
					{
						','
					});
					goods[2] = binding.ToString();
					data.GoodsItem = new ZhuanPanItem
					{
						ID = award.ID,
						GoodsID = string.Join(",", goods),
						AwardLevel = award.AwardLevel,
						GongGao = award.GongGao,
						AwardLabel = award.AwardLevel
					};
				}
				Global.SaveRoleParamsInt32ValueToDB(client, "10162", awardID, true);
				Global.SaveRoleParamsDateTimeToDB(client, "10165", now, true);
				Global.SaveRoleParamsInt32ValueToDB(client, "10166", binding, true);
				data.AwardType = awardType;
				client.sendCmd<ZhuanPanChouJiangData>(nID, data, false);
				client._IconStateMgr.CheckFreeZhuanPanChouState(client);
				client._IconStateMgr.SendIconStateToClient(client);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessZhuanPanLingJiangCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				ZhuanPanItem goodsItem = null;
				int goodIndex = Global.GetRoleParamsInt32FromDB(client, "10162") - 1;
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					DateTime chouJiangTime = Global.GetRoleParamsDateTimeFromDB(client, "10165");
					if (chouJiangTime < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime)
					{
						client.sendCmd(nID, "-100", false);
						return true;
					}
					if (goodIndex < 0 || goodIndex > ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanItemXmlList.Count)
					{
						client.sendCmd(nID, "-101", false);
						return true;
					}
					goodsItem = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanItemXmlList[goodIndex];
				}
				if (!Global.CanAddGoodsNum(client, 1))
				{
					client.sendCmd(nID, "-4", false);
					return true;
				}
				string[] goods = goodsItem.GoodsID.Split(new char[]
				{
					','
				});
				int goodsID = Convert.ToInt32(goods[0]);
				int gcount = Convert.ToInt32(goods[1]);
				int binding = Global.GetRoleParamsInt32FromDB(client, "10166");
				int level = Convert.ToInt32(goods[3]);
				int appendprop = Convert.ToInt32(goods[4]);
				int lucky = Convert.ToInt32(goods[5]);
				int excellenceinfo = Convert.ToInt32(goods[6]);
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
				{
					string strinfo = string.Format("系统中不存在{0}", goodsID);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					client.sendCmd(nID, "-201", false);
					return true;
				}
				int site = 0;
				int categoriy = systemGoods.GetIntValue("Categoriy", -1);
				if (categoriy >= 800 && categoriy < 816)
				{
					site = 3000;
				}
				else if (categoriy == 901)
				{
					site = 7000;
				}
				else if (categoriy >= 910 && categoriy <= 928)
				{
					site = 8000;
				}
				else if (categoriy == 940)
				{
					site = 11000;
				}
				else if (categoriy >= 980 && categoriy <= 981)
				{
					site = 16000;
				}
				Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsID, gcount, 0, "", level, binding, site, "", true, 1, "转盘抽奖", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceinfo, appendprop, 0, null, null, 0, true);
				if (goodsItem.GongGao == 1)
				{
					ZhuanPanGongGaoData gongGaoData = new ZhuanPanGongGaoData
					{
						ZoneId = client.ClientData.ZoneID,
						Rid = client.ClientData.RoleID,
						RoleName = client.ClientData.RoleName,
						GoodsId = goodsItem.GoodsID,
						GoodsIndex = goodIndex + 1
					};
					int index = 0;
					GameClient gc;
					while ((gc = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
					{
						gc.sendCmd<ZhuanPanGongGaoData>(1812, gongGaoData, false);
					}
					lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
					{
						if (null == ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList)
						{
							ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList = new List<ZhuanPanGongGaoData>();
						}
						while (ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList.Count >= 20)
						{
							ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList.RemoveAt(0);
						}
						ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList.Add(gongGaoData);
					}
				}
				Global.SaveRoleParamsInt32ValueToDB(client, "10162", 0, true);
				client.sendCmd(nID, "1", false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool LoadSystemParams()
		{
			bool result;
			try
			{
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					string strXiaoHao = GameManager.systemParamsList.GetParamValueByName("ZhuanPanCost");
					ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray = ConfigParser.ParserIntArrayList(strXiaoHao, true, '|', ',');
					int freeTime = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("ZhuanPanFree"));
					ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanFree = freeTime;
					ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("ZhuanPanZuanShiFuLi"));
					string time = GameManager.systemParamsList.GetParamValueByName("ZhuanPanTime");
					if (time == "" || null == time)
					{
						result = false;
					}
					else
					{
						string[] timeArr = time.Split(new char[]
						{
							','
						});
						if (!DateTime.TryParse(timeArr[0], out ZhuanPanManager.ZhuanPanRunTimeData.BeginTime) || !DateTime.TryParse(timeArr[1], out ZhuanPanManager.ZhuanPanRunTimeData.EndTime))
						{
							result = false;
						}
						else
						{
							ZhuanPanManager.ZhuanPanRunTimeData.EndTime.AddDays(1.0);
							DateTime now = TimeUtil.NowDateTime();
							ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanOpen = (now < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime || now > ZhuanPanManager.ZhuanPanRunTimeData.EndTime);
							result = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("转盘系统读取配置表出错，出错文件 SystemParams.xml ex:" + ex.Message, new object[0]), null, true);
				result = false;
			}
			return result;
		}

		
		public bool LoadZhuanPan()
		{
			bool result;
			try
			{
				string fileName = Global.GameResPath("Config\\ZhuanPan.xml");
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					result = false;
				}
				else
				{
					List<ZhuanPanItem> zhuanPanItemList = new List<ZhuanPanItem>();
					IEnumerable<XElement> nodes = xml.Elements();
					if (null == nodes)
					{
						result = false;
					}
					else
					{
						foreach (XElement xmlItem in nodes)
						{
							if (xmlItem != null)
							{
								ZhuanPanItem zhuanPanItem = new ZhuanPanItem
								{
									ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
									GoodsID = Global.GetDefAttributeStr(xmlItem, "GoodsID", ""),
									AwardLevel = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "AwardLevel", "0")),
									GongGao = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GongGao", "0")),
									AwardLabel = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "AwardLabel", "0"))
								};
								zhuanPanItemList.Add(zhuanPanItem);
							}
						}
						lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
						{
							ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanItemXmlList = zhuanPanItemList;
						}
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("转盘系统读取配置表出错，出错文件 ZhuanPan.xml ex:" + ex.Message, new object[0]), null, true);
				result = false;
			}
			return result;
		}

		
		public bool LoadZhuanPanAward()
		{
			bool result;
			try
			{
				string fileName = Global.GameResPath("Config\\ZhuanPanAward.xml");
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					result = false;
				}
				else
				{
					Dictionary<int, Dictionary<int, ZhuanPanAwardItem>> zhuanPanAwardDict = new Dictionary<int, Dictionary<int, ZhuanPanAwardItem>>();
					IEnumerable<XElement> typeNodes = xml.Elements("AwardLevel");
					if (null == typeNodes)
					{
						result = false;
					}
					else
					{
						foreach (XElement xmlNodesItem in typeNodes)
						{
							int typeID = Convert.ToInt32(Global.GetDefAttributeStr(xmlNodesItem, "TypeID", "0"));
							IEnumerable<XElement> nodes = xmlNodesItem.Elements("Award");
							if (null == nodes)
							{
								return false;
							}
							Dictionary<int, ZhuanPanAwardItem> awardItemDict = new Dictionary<int, ZhuanPanAwardItem>();
							foreach (XElement xmlItem in nodes)
							{
								int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
								ZhuanPanAwardItem awardItem = new ZhuanPanAwardItem
								{
									StartValue = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "StarValue", "0")),
									EndValue = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndValue", "0"))
								};
								awardItemDict.Add(id, awardItem);
							}
							zhuanPanAwardDict.Add(typeID, awardItemDict);
						}
						lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
						{
							ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanAwardXmlDict = zhuanPanAwardDict;
						}
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("转盘系统读取配置表出错，出错文件 ZhuanPanAward.xml ex:" + ex.Message, new object[0]), null, true);
				result = false;
			}
			return result;
		}

		
		public void ZhuanPanTimer_Work()
		{
			DateTime now = TimeUtil.NowDateTime();
			bool zhuanPanOpen = false;
			DateTime beginTime = DateTime.MaxValue;
			DateTime endTime = DateTime.MinValue;
			lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
			{
				zhuanPanOpen = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanOpen;
				beginTime = ZhuanPanManager.ZhuanPanRunTimeData.BeginTime;
				endTime = ZhuanPanManager.ZhuanPanRunTimeData.EndTime;
			}
			if (zhuanPanOpen && (now > endTime || now < beginTime))
			{
				GameManager.ClientMgr.NotifyAllActivityState(10, 0, beginTime.ToString("yyyyMMddHHmmss"), endTime.ToString("yyyyMMddHHmmss"), 0);
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanOpen = false;
					ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList.Clear();
				}
			}
			else if (!zhuanPanOpen && now > beginTime && now < endTime)
			{
				GameManager.ClientMgr.NotifyAllActivityState(10, 1, beginTime.ToString("yyyyMMddHHmmss"), endTime.ToString("yyyyMMddHHmmss"), 0);
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanOpen = true;
				}
			}
		}

		
		public void NotifyActivityState(GameClient client)
		{
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				bool zhuanPanOpen = false;
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					zhuanPanOpen = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanOpen;
				}
				if (zhuanPanOpen)
				{
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						10,
						1,
						"",
						0,
						0
					});
					client.sendCmd(770, strcmd, false);
				}
			}
		}

		
		public DateTime GetBeginTime()
		{
			return ZhuanPanManager.ZhuanPanRunTimeData.BeginTime;
		}

		
		private static ZhuanPanData ZhuanPanRunTimeData = new ZhuanPanData();

		
		private static ZhuanPanManager instance = new ZhuanPanManager();
	}
}
