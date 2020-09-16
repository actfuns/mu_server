using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	internal class LingYuManager
	{
		
		public static void LoadConfig()
		{
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(LingYuManager.LingYuTypeFile));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(LingYuManager.LingYuTypeFile));
			if (xml == null)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件不存在", LingYuManager.LingYuTypeFile), null, true);
			}
			else
			{
				try
				{
					LingYuManager.LingYuTypeDict.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							LingYuType lyType = new LingYuType();
							lyType.Type = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "TypeID", "0"));
							lyType.Name = Global.GetDefAttributeStr(xmlItem, "Name", "no-name");
							lyType.LifeScale = Global.GetSafeAttributeDouble(xmlItem, "LifeScale");
							lyType.AttackScale = Global.GetSafeAttributeDouble(xmlItem, "AttackScale");
							lyType.DefenseScale = Global.GetSafeAttributeDouble(xmlItem, "DefenseScale");
							lyType.MAttackScale = Global.GetSafeAttributeDouble(xmlItem, "MAttackScale");
							lyType.MDefenseScale = Global.GetSafeAttributeDouble(xmlItem, "MDefenseScale");
							lyType.HitScale = Global.GetSafeAttributeDouble(xmlItem, "HitScale");
							LingYuManager.LingYuTypeDict[lyType.Type] = lyType;
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时异常{1}", LingYuManager.LingYuTypeFile, ex), null, true);
				}
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(LingYuManager.LingYuLevelUpFile));
			xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(LingYuManager.LingYuLevelUpFile));
			if (xml == null)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件不存在", LingYuManager.LingYuLevelUpFile), null, true);
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
							int TypeID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "TypeID", "0"));
							LingYuType lyType = null;
							if (!LingYuManager.LingYuTypeDict.TryGetValue(TypeID, out lyType))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("加载翎羽升级文件{0}时，未找到类型为{1}的翎羽配置", LingYuManager.LingYuLevelUpFile, TypeID), null, true);
							}
							else
							{
								IEnumerable<XElement> xmlItemLevels = xmlItem.Elements();
								foreach (XElement xmlItemLevel in xmlItemLevels)
								{
									LingYuLevel lyLevel = new LingYuLevel();
									lyLevel.Level = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "Level", "0"));
									lyLevel.MinAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MinAttackV", "0"));
									lyLevel.MaxAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MaxAttackV", "0"));
									lyLevel.MinMAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MinMAttackV", "0"));
									lyLevel.MaxMAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MaxMAttackV", "0"));
									lyLevel.MinDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MinDefenseV", "0"));
									lyLevel.MaxDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MaxDefenseV", "0"));
									lyLevel.MinMDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MinMDefenseV", "0"));
									lyLevel.MaxMDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MaxMDefenseV", "0"));
									lyLevel.HitV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "HitV", "0"));
									lyLevel.LifeV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "LifeV", "0"));
									lyLevel.JinBiCost = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "JinBiCost", "0"));
									string costGoods = Global.GetDefAttributeStr(xmlItemLevel, "GoodsCost", "0");
									string[] costGoodsField = costGoods.Split(new char[]
									{
										','
									});
									if (costGoodsField.Length != 2)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("翎羽Type{0},级别{1}, 消耗物品配置错误", TypeID, lyLevel.Level), null, true);
									}
									else
									{
										lyLevel.GoodsCost = Convert.ToInt32(costGoodsField[0]);
										lyLevel.GoodsCostCnt = Convert.ToInt32(costGoodsField[1]);
										lyType.LevelDict[lyLevel.Level] = lyLevel;
										LingYuManager.LingYuLevelLimit = Global.GMax(LingYuManager.LingYuLevelLimit, lyLevel.Level);
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时异常{1}", LingYuManager.LingYuLevelUpFile, ex), null, true);
				}
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(LingYuManager.LingYuSuitUpFile));
			xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(LingYuManager.LingYuSuitUpFile));
			if (xml == null)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件不存在", LingYuManager.LingYuSuitUpFile), null, true);
			}
			else
			{
				try
				{
					lock (LingYuManager.LingYuTypeDict)
					{
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (null != xmlItem)
							{
								int TypeID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "TypeID", "0"));
								LingYuType lyType = null;
								if (!LingYuManager.LingYuTypeDict.TryGetValue(TypeID, out lyType))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("加载翎羽进阶文件{0}时，未找到类型为{1}的翎羽配置", LingYuManager.LingYuSuitUpFile, TypeID), null, true);
								}
								else
								{
									IEnumerable<XElement> xmlItemLevels = xmlItem.Elements();
									foreach (XElement xmlItemLevel in xmlItemLevels)
									{
										LingYuSuit lySuit = new LingYuSuit();
										lySuit.Suit = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "SuitID", "0"));
										lySuit.MinAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MinAttackV", "0"));
										lySuit.MaxAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MaxAttackV", "0"));
										lySuit.MinMAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MinMAttackV", "0"));
										lySuit.MaxMAttackV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MaxMAttackV", "0"));
										lySuit.MinDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MinDefenseV", "0"));
										lySuit.MaxDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MaxDefenseV", "0"));
										lySuit.MinMDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MinMDefenseV", "0"));
										lySuit.MaxMDefenseV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "MaxMDefenseV", "0"));
										lySuit.HitV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "HitV", "0"));
										lySuit.LifeV = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "LifeV", "0"));
										lySuit.JinBiCost = Convert.ToInt32(Global.GetDefAttributeStr(xmlItemLevel, "JinBiCost", "0"));
										string costGoods = Global.GetDefAttributeStr(xmlItemLevel, "GoodsCost", "0");
										lySuit.GoodsCost = ConfigHelper.ParserIntArrayList(costGoods, true, '|', ',');
										lyType.SuitDict[lySuit.Suit] = lySuit;
										LingYuManager.LingYuSuitLimit = Global.GMax(LingYuManager.LingYuSuitLimit, lySuit.Suit);
									}
								}
							}
						}
						LingYuManager.LingYuSuitLimit = Global.GMin(LingYuManager.LingYuSuitLimit, (int)GameManager.systemParamsList.GetParamValueIntByName("LingYuMax", 0));
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时异常{1}", LingYuManager.LingYuSuitUpFile, ex), null, true);
				}
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(LingYuManager.LingYuCollectFile));
			xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(LingYuManager.LingYuCollectFile));
			if (xml == null)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件不存在", LingYuManager.LingYuCollectFile), null, true);
			}
			else
			{
				try
				{
					LingYuManager.LingYuCollectList.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							LingYuCollect lyCollect = new LingYuCollect();
							lyCollect.Num = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Num", "0"));
							lyCollect.NeedSuit = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedSuit", "0"));
							lyCollect.Luck = Global.GetSafeAttributeDouble(xmlItem, "Luck");
							lyCollect.DeLuck = Global.GetSafeAttributeDouble(xmlItem, "DeLuck");
							LingYuManager.LingYuCollectList.Add(lyCollect);
						}
					}
					LingYuManager.LingYuCollectList.Sort(delegate(LingYuCollect left, LingYuCollect right)
					{
						int result;
						if (left.NeedSuit > right.NeedSuit)
						{
							result = 1;
						}
						else if (left.NeedSuit == right.NeedSuit)
						{
							if (left.Num > right.Num)
							{
								result = 1;
							}
							else if (left.Num == right.Num)
							{
								result = 0;
							}
							else
							{
								result = -1;
							}
						}
						else
						{
							result = -1;
						}
						return result;
					});
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时异常{1}", LingYuManager.LingYuCollectFile, ex), null, true);
				}
			}
		}

		
		public static string Error2Str(LingYuError lyError)
		{
			string result;
			if (lyError == LingYuError.Success)
			{
				result = GLang.GetLang(407, new object[0]);
			}
			else if (lyError == LingYuError.NotOpen)
			{
				result = GLang.GetLang(408, new object[0]);
			}
			else if (lyError == LingYuError.LevelFull)
			{
				result = GLang.GetLang(409, new object[0]);
			}
			else if (lyError == LingYuError.NeedLevelUp)
			{
				result = GLang.GetLang(410, new object[0]);
			}
			else if (lyError == LingYuError.NeedSuitUp)
			{
				result = GLang.GetLang(411, new object[0]);
			}
			else if (lyError == LingYuError.SuitFull)
			{
				result = GLang.GetLang(412, new object[0]);
			}
			else if (lyError == LingYuError.LevelUpMaterialNotEnough)
			{
				result = GLang.GetLang(413, new object[0]);
			}
			else if (lyError == LingYuError.LevelUpJinBiNotEnough)
			{
				result = GLang.GetLang(414, new object[0]);
			}
			else if (lyError == LingYuError.SuitUpMaterialNotEnough)
			{
				result = GLang.GetLang(415, new object[0]);
			}
			else if (lyError == LingYuError.SuitUpJinBiNotEnough)
			{
				result = GLang.GetLang(416, new object[0]);
			}
			else if (lyError == LingYuError.ErrorConfig)
			{
				result = GLang.GetLang(417, new object[0]);
			}
			else if (lyError == LingYuError.ErrorParams)
			{
				result = GLang.GetLang(418, new object[0]);
			}
			else if (lyError == LingYuError.ZuanShiNotEnough)
			{
				result = GLang.GetLang(419, new object[0]);
			}
			else if (lyError == LingYuError.DBSERVERERROR)
			{
				result = GLang.GetLang(420, new object[0]);
			}
			else
			{
				result = "unknown";
			}
			return result;
		}

		
		public static void UpdateLingYuProps(GameClient client)
		{
			if (null != client.ClientData.MyWingData)
			{
				if (client.ClientData.MyWingData.WingID > 0)
				{
					double MinAttackV = 0.0;
					double MaxAttackV = 0.0;
					double MinMAttackV = 0.0;
					double MaxMAttackV = 0.0;
					double MinDefenseV = 0.0;
					double MaxDefenseV = 0.0;
					double MinMDefenseV = 0.0;
					double MaxMDefenseV = 0.0;
					double HitV = 0.0;
					double LifeV = 0.0;
					int[] suitCnt = new int[LingYuManager.LingYuSuitLimit + 1];
					if (client.ClientData.MyWingData.Using == 1)
					{
						lock (client.ClientData.LingYuDict)
						{
							foreach (KeyValuePair<int, LingYuData> kv in client.ClientData.LingYuDict)
							{
								int type = kv.Value.Type;
								int level = kv.Value.Level;
								int suit = kv.Value.Suit;
								for (int i = 0; i <= suit; i++)
								{
									suitCnt[i]++;
								}
								LingYuType lyType = null;
								if (LingYuManager.LingYuTypeDict.TryGetValue(type, out lyType))
								{
									LingYuLevel lyLevel = null;
									lyType.LevelDict.TryGetValue(level, out lyLevel);
									LingYuSuit lySuit = null;
									lyType.SuitDict.TryGetValue(suit, out lySuit);
									if (lyLevel != null)
									{
										MinAttackV += (double)lyLevel.MinAttackV;
										MaxAttackV += (double)lyLevel.MaxAttackV;
										MinMAttackV += (double)lyLevel.MinMAttackV;
										MaxMAttackV += (double)lyLevel.MaxMAttackV;
										MinDefenseV += (double)lyLevel.MinDefenseV;
										MaxDefenseV += (double)lyLevel.MaxDefenseV;
										MinMDefenseV += (double)lyLevel.MinMDefenseV;
										MaxMDefenseV += (double)lyLevel.MaxMDefenseV;
										HitV += (double)lyLevel.HitV;
										LifeV += (double)lyLevel.LifeV;
									}
									if (lySuit != null)
									{
										MinAttackV += (double)lySuit.MinAttackV;
										MaxAttackV += (double)lySuit.MaxAttackV;
										MinMAttackV += (double)lySuit.MinMAttackV;
										MaxMAttackV += (double)lySuit.MaxMAttackV;
										MinDefenseV += (double)lySuit.MinDefenseV;
										MaxDefenseV += (double)lySuit.MaxDefenseV;
										MinMDefenseV += (double)lySuit.MinMDefenseV;
										MaxMDefenseV += (double)lySuit.MaxMDefenseV;
										HitV += (double)lySuit.HitV;
										LifeV += (double)lySuit.LifeV;
									}
								}
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						7,
						MinAttackV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						8,
						MaxAttackV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						9,
						MinMAttackV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						10,
						MaxMAttackV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						3,
						MinDefenseV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						4,
						MaxDefenseV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						5,
						MinMDefenseV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						6,
						MaxMDefenseV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						18,
						HitV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						13,
						LifeV
					});
					double lucky = 0.0;
					double deLucky = 0.0;
					if (client.ClientData.MyWingData.Using == 1)
					{
						for (int i = LingYuManager.LingYuCollectList.Count<LingYuCollect>() - 1; i >= 0; i--)
						{
							LingYuCollect lyCollect = LingYuManager.LingYuCollectList[i];
							if (suitCnt[lyCollect.NeedSuit] >= lyCollect.Num)
							{
								lucky = lyCollect.Luck;
								deLucky = lyCollect.DeLuck;
								break;
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						17,
						lucky
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						5,
						51,
						deLucky
					});
				}
			}
		}

		
		public static List<LingYuData> GetLingYuList(GameClient client)
		{
			List<LingYuData> dataList = new List<LingYuData>();
			Dictionary<int, LingYuType>.KeyCollection keys = LingYuManager.LingYuTypeDict.Keys;
			foreach (int type in keys)
			{
				LingYuData lyData = null;
				lock (client.ClientData.LingYuDict)
				{
					if (!client.ClientData.LingYuDict.TryGetValue(type, out lyData))
					{
						lyData = new LingYuData();
						lyData.Type = type;
						lyData.Level = 1;
						lyData.Suit = 0;
					}
				}
				dataList.Add(lyData);
			}
			return dataList;
		}

		
		public static TCPProcessCmdResults ProcessGetLingYuList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				List<LingYuData> dataList = LingYuManager.GetLingYuList(client);
				byte[] bytesData = DataHelper.ObjectToBytes<List<LingYuData>>(dataList);
				GameManager.ClientMgr.SendToClient(client, bytesData, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetLingYuList", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static LingYuError AdvanceLingYuLevel(GameClient client, int roleID, int type, int useZuanshiIfNoMaterial)
		{
			LingYuError result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.WingLingYu, false))
			{
				result = LingYuError.NotOpen;
			}
			else
			{
				LingYuType lyType = null;
				if (!LingYuManager.LingYuTypeDict.TryGetValue(type, out lyType))
				{
					result = LingYuError.ErrorParams;
				}
				else
				{
					LingYuData lyData = null;
					lock (client.ClientData.LingYuDict)
					{
						if (!client.ClientData.LingYuDict.TryGetValue(type, out lyData))
						{
							lyData = new LingYuData();
							lyData.Type = type;
							lyData.Level = 1;
							lyData.Suit = 0;
						}
					}
					if (lyData.Level == LingYuManager.LingYuLevelLimit)
					{
						result = LingYuError.LevelFull;
					}
					else if (lyData.Level > 0 && lyData.Level % 10 == 0 && lyData.Level / 10 != lyData.Suit)
					{
						result = LingYuError.NeedSuitUp;
					}
					else
					{
						LingYuLevel nextLevel = null;
						if (!lyType.LevelDict.TryGetValue(lyData.Level + 1, out nextLevel))
						{
							result = LingYuError.ErrorConfig;
						}
						else if (Global.GetTotalBindTongQianAndTongQianVal(client) < nextLevel.JinBiCost)
						{
							result = LingYuError.LevelUpJinBiNotEnough;
						}
						else
						{
							int haveGoodsCnt = Global.GetTotalGoodsCountByID(client, nextLevel.GoodsCost);
							if (haveGoodsCnt < nextLevel.GoodsCostCnt && useZuanshiIfNoMaterial == 0)
							{
								result = LingYuError.LevelUpMaterialNotEnough;
							}
							else
							{
								string strCostList = "";
								int oldLevel = lyData.Level;
								int oldYinLiang = client.ClientData.YinLiang;
								int oldMoney = client.ClientData.Money1;
								int oldUserMoney = client.ClientData.UserMoney;
								int oldUserGlod = client.ClientData.Gold;
								int goodsCostCnt = nextLevel.GoodsCostCnt;
								int zuanshiCost = 0;
								if (haveGoodsCnt < nextLevel.GoodsCostCnt)
								{
									goodsCostCnt = 0;
									int goodsPrice = 0;
									if (!Data.LingYuMaterialZuanshiDict.TryGetValue(nextLevel.GoodsCost, out goodsPrice))
									{
										return LingYuError.ErrorConfig;
									}
									zuanshiCost = nextLevel.GoodsCostCnt * goodsPrice;
									if (client.ClientData.UserMoney < zuanshiCost && !HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, zuanshiCost))
									{
										return LingYuError.ZuanShiNotEnough;
									}
								}
								if (!Global.SubBindTongQianAndTongQian(client, nextLevel.JinBiCost, "翎羽升级消耗"))
								{
									result = LingYuError.DBSERVERERROR;
								}
								else
								{
									strCostList = EventLogManager.NewResPropString(ResLogType.SubJinbi, new object[]
									{
										-nextLevel.JinBiCost,
										oldYinLiang,
										client.ClientData.YinLiang,
										oldMoney,
										client.ClientData.Money1
									});
									if (goodsCostCnt > 0)
									{
										bool bUsedBinding = false;
										bool bUsedTimeLimited = false;
										if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nextLevel.GoodsCost, goodsCostCnt, false, out bUsedBinding, out bUsedTimeLimited, false))
										{
											return LingYuError.DBSERVERERROR;
										}
										GoodsData goodsData = new GoodsData
										{
											GoodsID = nextLevel.GoodsCost,
											GCount = goodsCostCnt
										};
										strCostList += EventLogManager.AddGoodsDataPropString(goodsData);
									}
									if (zuanshiCost > 0)
									{
										if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, zuanshiCost, "翎羽升级", true, true, false, DaiBiSySType.LingYuShengXing))
										{
											return LingYuError.DBSERVERERROR;
										}
										strCostList += EventLogManager.AddResPropString(ResLogType.FristBindZuanShi, new object[]
										{
											-zuanshiCost,
											oldUserGlod,
											client.ClientData.Gold,
											oldUserMoney,
											client.ClientData.UserMoney
										});
									}
									int iRet = LingYuManager.UpdateLingYu2DB(roleID, type, lyData.Level + 1, lyData.Suit, client.ServerId);
									if (iRet < 0)
									{
										result = LingYuError.DBSERVERERROR;
									}
									else
									{
										lyData.Level++;
										lock (client.ClientData.LingYuDict)
										{
											client.ClientData.LingYuDict[type] = lyData;
										}
										LingYuManager.UpdateLingYuProps(client);
										GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
										GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
										EventLogManager.AddLingYuLevelEvent(client, useZuanshiIfNoMaterial, oldLevel, lyData.Suit, lyData.Level, strCostList);
										if (client._IconStateMgr.CheckReborn(client))
										{
											client._IconStateMgr.SendIconStateToClient(client);
										}
										result = LingYuError.Success;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public static TCPProcessCmdResults ProcessAdvanceLingYuLevel(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int type = Convert.ToInt32(fields[1]);
				int useZuanshiIfNoMaterial = Convert.ToInt32(fields[2]);
				LingYuError lyError = LingYuManager.AdvanceLingYuLevel(client, roleID, type, useZuanshiIfNoMaterial);
				LingYuData lyData = null;
				lock (client.ClientData.LingYuDict)
				{
					if (!client.ClientData.LingYuDict.TryGetValue(type, out lyData))
					{
						lyData = new LingYuData();
						lyData.Type = type;
						lyData.Level = 1;
						lyData.Suit = 0;
					}
				}
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					roleID,
					(int)lyError,
					lyData.Type,
					lyData.Level
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessAdvanceLingYuLevel", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static LingYuError AdvanceLingYuSuit(GameClient client, int roleID, int type, int useZuanshiIfNoMaterial)
		{
			LingYuError result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.WingLingYu, false))
			{
				result = LingYuError.NotOpen;
			}
			else
			{
				LingYuType lyType = null;
				if (!LingYuManager.LingYuTypeDict.TryGetValue(type, out lyType))
				{
					result = LingYuError.ErrorParams;
				}
				else
				{
					LingYuData lyData = null;
					lock (client.ClientData.LingYuDict)
					{
						if (!client.ClientData.LingYuDict.TryGetValue(type, out lyData))
						{
							lyData = new LingYuData();
							lyData.Type = type;
							lyData.Level = 1;
							lyData.Suit = 0;
						}
					}
					if (lyData.Suit == LingYuManager.LingYuSuitLimit)
					{
						result = LingYuError.SuitFull;
					}
					else if (lyData.Level == 0 || lyData.Level / 10 == lyData.Suit)
					{
						result = LingYuError.NeedLevelUp;
					}
					else
					{
						LingYuSuit nextSuit = null;
						if (!lyType.SuitDict.TryGetValue(lyData.Suit + 1, out nextSuit))
						{
							result = LingYuError.ErrorConfig;
						}
						else if (Global.GetTotalBindTongQianAndTongQianVal(client) < nextSuit.JinBiCost)
						{
							result = LingYuError.SuitUpJinBiNotEnough;
						}
						else
						{
							bool useZuanShi = false;
							int zuanshiCost = 0;
							for (int i = 0; i < nextSuit.GoodsCost.Count; i++)
							{
								int goodsId = nextSuit.GoodsCost[i][0];
								int costCount = nextSuit.GoodsCost[i][1];
								int haveGoodsCnt = Global.GetTotalGoodsCountByID(client, goodsId);
								if (haveGoodsCnt < costCount)
								{
									if (useZuanshiIfNoMaterial == 0)
									{
										return LingYuError.SuitUpMaterialNotEnough;
									}
									useZuanShi = true;
								}
								int goodsPrice;
								if (!Data.LingYuMaterialZuanshiDict.TryGetValue(goodsId, out goodsPrice))
								{
									return LingYuError.ErrorConfig;
								}
								zuanshiCost += costCount * goodsPrice;
							}
							string strCostList = "";
							int oldLevel = lyData.Level;
							int oldSuit = lyData.Suit;
							int oldYinLiang = client.ClientData.YinLiang;
							int oldMoney = client.ClientData.Money1;
							int oldUserMoney = client.ClientData.UserMoney;
							int oldUserGlod = client.ClientData.Gold;
							if (useZuanShi)
							{
								if (client.ClientData.UserMoney < zuanshiCost && !HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, zuanshiCost))
								{
									return LingYuError.ZuanShiNotEnough;
								}
							}
							if (!Global.SubBindTongQianAndTongQian(client, nextSuit.JinBiCost, "翎羽升阶消耗"))
							{
								result = LingYuError.DBSERVERERROR;
							}
							else
							{
								strCostList = EventLogManager.NewResPropString(ResLogType.SubJinbi, new object[]
								{
									-nextSuit.JinBiCost,
									oldYinLiang,
									client.ClientData.YinLiang,
									oldMoney,
									client.ClientData.Money1
								});
								if (!useZuanShi)
								{
									bool bUsedBinding = false;
									bool bUsedTimeLimited = false;
									for (int i = 0; i < nextSuit.GoodsCost.Count; i++)
									{
										int goodsId = nextSuit.GoodsCost[i][0];
										int costCount = nextSuit.GoodsCost[i][1];
										if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsId, costCount, false, out bUsedBinding, out bUsedTimeLimited, false))
										{
											GameManager.logDBCmdMgr.AddDBLogInfo(0, "升级失败", "翎羽升级", Global.GetMapName(client.ClientData.MapCode), "系统", "记录", 0, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
											return LingYuError.DBSERVERERROR;
										}
										GoodsData goodsData = new GoodsData
										{
											GoodsID = goodsId,
											GCount = costCount
										};
										strCostList += EventLogManager.AddGoodsDataPropString(goodsData);
									}
								}
								else
								{
									if (zuanshiCost <= 0)
									{
										return LingYuError.ErrorConfig;
									}
									if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, zuanshiCost, "翎羽升级", true, true, false, DaiBiSySType.LingYuShengJie))
									{
										return LingYuError.DBSERVERERROR;
									}
									strCostList += EventLogManager.AddResPropString(ResLogType.FristBindZuanShi, new object[]
									{
										-zuanshiCost,
										oldUserGlod,
										client.ClientData.Gold,
										oldUserMoney,
										client.ClientData.UserMoney
									});
								}
								int iRet = LingYuManager.UpdateLingYu2DB(roleID, type, lyData.Level, lyData.Suit + 1, client.ServerId);
								if (iRet < 0)
								{
									result = LingYuError.DBSERVERERROR;
								}
								else
								{
									lyData.Suit++;
									lock (client.ClientData.LingYuDict)
									{
										client.ClientData.LingYuDict[type] = lyData;
									}
									if (LingYuManager.SuitOfNotifyList.Contains(lyData.Suit))
									{
										string broadcastMsg = StringUtil.substitute(GLang.GetLang(421, new object[0]), new object[]
										{
											Global.FormatRoleName(client, client.ClientData.RoleName),
											lyType.Name,
											lyData.Suit
										});
										Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.HintMsg, broadcastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
									}
									LingYuManager.UpdateLingYuProps(client);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
									EventLogManager.AddLingYuSuitEvent(client, useZuanshiIfNoMaterial, oldSuit, lyData.Suit, oldLevel, lyData.Level, strCostList);
									if (client._IconStateMgr.CheckReborn(client))
									{
										client._IconStateMgr.SendIconStateToClient(client);
									}
									result = LingYuError.Success;
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public static TCPProcessCmdResults ProcessAdvanceLingYuSuit(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int type = Convert.ToInt32(fields[1]);
				int useZuanshiIfNoMaterial = Convert.ToInt32(fields[2]);
				LingYuError lyError = LingYuManager.AdvanceLingYuSuit(client, roleID, type, useZuanshiIfNoMaterial);
				LingYuData lyData = null;
				lock (client.ClientData.LingYuDict)
				{
					if (!client.ClientData.LingYuDict.TryGetValue(type, out lyData))
					{
						lyData = new LingYuData();
						lyData.Type = type;
						lyData.Level = 1;
						lyData.Suit = 0;
					}
				}
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					roleID,
					(int)lyError,
					lyData.Type,
					lyData.Suit
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessAdvanceLingYuSuit", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private static int UpdateLingYu2DB(int roleID, int type, int level, int suit, int serverId)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				roleID,
				type,
				level,
				suit
			});
			string[] fields = Global.ExecuteDBCmd(10176, strcmd, serverId);
			int result;
			if (fields == null || fields.Length != 2)
			{
				result = -1;
			}
			else
			{
				result = Convert.ToInt32(fields[1]);
			}
			return result;
		}

		
		public static void InitAsOpened(GameClient client)
		{
			Dictionary<int, LingYuType>.KeyCollection keys = LingYuManager.LingYuTypeDict.Keys;
			foreach (int type in keys)
			{
				lock (client.ClientData.LingYuDict)
				{
					if (!client.ClientData.LingYuDict.ContainsKey(type))
					{
						LingYuData data = new LingYuData
						{
							Type = type,
							Level = 1,
							Suit = 0
						};
						LingYuManager.UpdateLingYu2DB(client.ClientData.RoleID, type, 1, 0, client.ServerId);
						client.ClientData.LingYuDict[type] = data;
					}
				}
			}
			LingYuManager.UpdateLingYuProps(client);
		}

		
		public static int GetTotalLevel(GameClient client)
		{
			int totalLev = 0;
			lock (client.ClientData.LingYuDict)
			{
				foreach (LingYuData value in client.ClientData.LingYuDict.Values)
				{
					totalLev += value.Suit * 10 + value.Level;
				}
			}
			return totalLev;
		}

		
		public static bool IfLingYuPerfect(GameClient client)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.WingLingYu, false))
			{
				result = false;
			}
			else
			{
				Dictionary<int, LingYuType>.KeyCollection keys = LingYuManager.LingYuTypeDict.Keys;
				if (keys.Count != client.ClientData.LingYuDict.Count)
				{
					result = false;
				}
				else
				{
					foreach (LingYuData value in client.ClientData.LingYuDict.Values)
					{
						if (value.Suit != LingYuManager.LingYuSuitLimit || value.Level != LingYuManager.LingYuLevelLimit)
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		
		public static void SetLingYuMax_GM(GameClient client)
		{
			LingYuManager.InitAsOpened(client);
			lock (client.ClientData.LingYuDict)
			{
				foreach (LingYuData value in client.ClientData.LingYuDict.Values)
				{
					LingYuType lyType = null;
					if (LingYuManager.LingYuTypeDict.TryGetValue(value.Type, out lyType))
					{
						value.Suit = LingYuManager.LingYuSuitLimit;
						value.Level = LingYuManager.LingYuLevelLimit;
						LingYuManager.UpdateLingYu2DB(client.ClientData.RoleID, value.Type, value.Level, value.Suit, client.ServerId);
						string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.RoleID,
							0,
							value.Type,
							value.Suit
						});
						client.sendCmd(802, strcmd, false);
						strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.RoleID,
							0,
							value.Type,
							value.Level
						});
						client.sendCmd(801, strcmd, false);
					}
				}
			}
			LingYuManager.UpdateLingYuProps(client);
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
			if (client._IconStateMgr.CheckReborn(client))
			{
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		
		private const int DEFAULT_LINGYU_LEVEL = 1;

		
		private static string LingYuTypeFile = "Config/LingyuType.xml";

		
		private static string LingYuLevelUpFile = "Config/LingYuLevelUp.xml";

		
		private static string LingYuSuitUpFile = "Config/LingYuSuitUp.xml";

		
		private static string LingYuCollectFile = "Config/LingYucollect.xml";

		
		private static int LingYuLevelLimit = 0;

		
		private static int LingYuSuitLimit = 0;

		
		private static Dictionary<int, LingYuType> LingYuTypeDict = new Dictionary<int, LingYuType>();

		
		private static List<LingYuCollect> LingYuCollectList = new List<LingYuCollect>();

		
		private static int[] SuitOfNotifyList = new int[]
		{
			3,
			6,
			9
		};
	}
}
