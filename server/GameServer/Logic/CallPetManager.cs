using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	
	internal class CallPetManager
	{
		
		public static void LoadCallPetType()
		{
			try
			{
				lock (CallPetManager._CallPetMutex)
				{
					CallPetManager.CallPetTypeDict.Clear();
					string fileName = "Config/CallPetType.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
					XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
					if (null == xml)
					{
						LogManager.WriteLog(LogTypes.Fatal, "加载Config/CallPetType.xml时出错!!!文件不存在", null, true);
					}
					else
					{
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							CallPetType CfgData = new CallPetType();
							CfgData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							CfgData.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xmlItem, "MinZhuanSheng");
							CfgData.MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
							CfgData.MaxZhuanSheng = (int)Global.GetSafeAttributeLong(xmlItem, "MaxZhuanSheng");
							CfgData.MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
							CallPetManager.CallPetTypeDict[CfgData.ID] = CfgData;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "加载Config/CallPetType.xml时文件出错", ex, true);
			}
		}

		
		public static void LoadCallPetConfig()
		{
			try
			{
				lock (CallPetManager._CallPetMutex)
				{
					CallPetManager.CallPetConfigList.Clear();
					CallPetManager.FreeCallPetConfigList.Clear();
					CallPetManager.HuoDongCallPetConfigList.Clear();
					CallPetManager.TeQuanCallPetConfigList.Clear();
					string fileName = "Config/CallPet.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
					XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
					if (null == xml)
					{
						LogManager.WriteLog(LogTypes.Fatal, "加载Config/CallPet.xml时出错!!!文件不存在", null, true);
					}
					else
					{
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							CallPetConfig CfgData = new CallPetConfig();
							CfgData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							CfgData.GoodsID = (int)Global.GetSafeAttributeLong(xmlItem, "GoodsID");
							CfgData.Num = (int)Global.GetSafeAttributeLong(xmlItem, "Num");
							CfgData.QiangHuaFallID = (int)Global.GetSafeAttributeLong(xmlItem, "QiangHuaFallID");
							CfgData.ZhuiJiaFallID = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuiJiaFallID");
							CfgData.LckyProbability = (int)Global.GetSafeAttributeLong(xmlItem, "LckyProbability");
							CfgData.ZhuoYueFallID = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuoYueFallID");
							CfgData.MinMoney = (int)Global.GetSafeAttributeLong(xmlItem, "MinMoney");
							CfgData.MaxMoney = (int)Global.GetSafeAttributeLong(xmlItem, "MaxMoney");
							CfgData.MinBindYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "MinBindYuanBao");
							CfgData.MaxBindYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "MaxBindYuanBao");
							CfgData.MinExp = (int)Global.GetSafeAttributeLong(xmlItem, "MinExp");
							CfgData.MaxExp = (int)Global.GetSafeAttributeLong(xmlItem, "MaxExp");
							CfgData.StartValues = (int)Global.GetSafeAttributeLong(xmlItem, "StartValues");
							CfgData.EndValues = (int)Global.GetSafeAttributeLong(xmlItem, "EndValues");
							CallPetManager.CallPetConfigList.Add(CfgData);
						}
						fileName = "Config/FreeCallPet.xml";
						GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
						xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
						if (null == xml)
						{
							LogManager.WriteLog(LogTypes.Fatal, "加载Config/FreeCallPet.xml时出错!!!文件不存在", null, true);
						}
						else
						{
							xmlItems = xml.Elements();
							foreach (XElement xmlItem in xmlItems)
							{
								CallPetConfig CfgData = new CallPetConfig();
								CfgData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
								CfgData.GoodsID = (int)Global.GetSafeAttributeLong(xmlItem, "GoodsID");
								CfgData.Num = (int)Global.GetSafeAttributeLong(xmlItem, "Num");
								CfgData.QiangHuaFallID = (int)Global.GetSafeAttributeLong(xmlItem, "QiangHuaFallID");
								CfgData.ZhuiJiaFallID = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuiJiaFallID");
								CfgData.LckyProbability = (int)Global.GetSafeAttributeLong(xmlItem, "LckyProbability");
								CfgData.ZhuoYueFallID = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuoYueFallID");
								CfgData.MinMoney = (int)Global.GetSafeAttributeLong(xmlItem, "MinMoney");
								CfgData.MaxMoney = (int)Global.GetSafeAttributeLong(xmlItem, "MaxMoney");
								CfgData.MinBindYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "MinBindYuanBao");
								CfgData.MaxBindYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "MaxBindYuanBao");
								CfgData.MinExp = (int)Global.GetSafeAttributeLong(xmlItem, "MinExp");
								CfgData.MaxExp = (int)Global.GetSafeAttributeLong(xmlItem, "MaxExp");
								CfgData.StartValues = (int)Global.GetSafeAttributeLong(xmlItem, "StartValues");
								CfgData.EndValues = (int)Global.GetSafeAttributeLong(xmlItem, "EndValues");
								CallPetManager.FreeCallPetConfigList.Add(CfgData);
							}
							fileName = "Config/HuoDongCallPet.xml";
							GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
							xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
							if (null == xml)
							{
								LogManager.WriteLog(LogTypes.Fatal, "加载Config/HuoDongCallPet.xml时出错!!!文件不存在", null, true);
							}
							else
							{
								xmlItems = xml.Elements();
								foreach (XElement xmlItem in xmlItems)
								{
									CallPetConfig CfgData = new CallPetConfig();
									CfgData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
									CfgData.GoodsID = (int)Global.GetSafeAttributeLong(xmlItem, "GoodsID");
									CfgData.Num = (int)Global.GetSafeAttributeLong(xmlItem, "Num");
									CfgData.QiangHuaFallID = (int)Global.GetSafeAttributeLong(xmlItem, "QiangHuaFallID");
									CfgData.ZhuiJiaFallID = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuiJiaFallID");
									CfgData.LckyProbability = (int)Global.GetSafeAttributeLong(xmlItem, "LckyProbability");
									CfgData.ZhuoYueFallID = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuoYueFallID");
									CfgData.MinMoney = (int)Global.GetSafeAttributeLong(xmlItem, "MinMoney");
									CfgData.MaxMoney = (int)Global.GetSafeAttributeLong(xmlItem, "MaxMoney");
									CfgData.MinBindYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "MinBindYuanBao");
									CfgData.MaxBindYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "MaxBindYuanBao");
									CfgData.MinExp = (int)Global.GetSafeAttributeLong(xmlItem, "MinExp");
									CfgData.MaxExp = (int)Global.GetSafeAttributeLong(xmlItem, "MaxExp");
									CfgData.StartValues = (int)Global.GetSafeAttributeLong(xmlItem, "StartValues");
									CfgData.EndValues = (int)Global.GetSafeAttributeLong(xmlItem, "EndValues");
									CallPetManager.HuoDongCallPetConfigList.Add(CfgData);
								}
								fileName = "Config/TeQuanCallPet.xml";
								GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
								xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
								if (null == xml)
								{
									LogManager.WriteLog(LogTypes.Fatal, "加载Config/TeQuanBuHuo.xml时出错!!!文件不存在", null, true);
								}
								else
								{
									xmlItems = xml.Elements();
									foreach (XElement xmlItem in xmlItems)
									{
										CallPetConfig CfgData = new CallPetConfig();
										CfgData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
										CfgData.GoodsID = (int)Global.GetSafeAttributeLong(xmlItem, "GoodsID");
										CfgData.Num = (int)Global.GetSafeAttributeLong(xmlItem, "Num");
										CfgData.QiangHuaFallID = (int)Global.GetSafeAttributeLong(xmlItem, "QiangHuaFallID");
										CfgData.ZhuiJiaFallID = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuiJiaFallID");
										CfgData.LckyProbability = (int)Global.GetSafeAttributeLong(xmlItem, "LckyProbability");
										CfgData.ZhuoYueFallID = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuoYueFallID");
										CfgData.MinMoney = (int)Global.GetSafeAttributeLong(xmlItem, "MinMoney");
										CfgData.MaxMoney = (int)Global.GetSafeAttributeLong(xmlItem, "MaxMoney");
										CfgData.MinBindYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "MinBindYuanBao");
										CfgData.MaxBindYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "MaxBindYuanBao");
										CfgData.MinExp = (int)Global.GetSafeAttributeLong(xmlItem, "MinExp");
										CfgData.MaxExp = (int)Global.GetSafeAttributeLong(xmlItem, "MaxExp");
										CfgData.StartValues = (int)Global.GetSafeAttributeLong(xmlItem, "StartValues");
										CfgData.EndValues = (int)Global.GetSafeAttributeLong(xmlItem, "EndValues");
										CallPetManager.TeQuanCallPetConfigList.Add(CfgData);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "加载Config/CallPet.xml或FreeCallPet.xml时文件出错", ex, true);
			}
		}

		
		public static void LoadCallPetSystem()
		{
			lock (CallPetManager._CallPetMutex)
			{
				CallPetManager.CallPetPriceDict.Clear();
				string[] strPrice = GameManager.systemParamsList.GetParamValueByName("CallPet").Split(new char[]
				{
					','
				});
				if (strPrice == null || strPrice.Length != 2)
				{
					SysConOut.WriteLine("        加载SystemParams.xml时出错!!!CallPet不存在");
				}
				else
				{
					CallPetManager.CallPetPriceDict[1] = Convert.ToInt32(strPrice[0]);
					CallPetManager.CallPetPriceDict[10] = Convert.ToInt32(strPrice[1]);
					double nHour = GameManager.systemParamsList.GetParamValueDoubleByName("FreeCallPet", 0.0);
					if (nHour <= 0.0)
					{
						SysConOut.WriteLine("        加载SystemParams.xml时出错!!!FreeCallPet不存在");
					}
					else
					{
						CallPetManager.CallPetFreeHour = nHour;
						double nTemp = GameManager.systemParamsList.GetParamValueDoubleByName("ConsumeCallPetJiFen", 0.0);
						if (nTemp < 0.0)
						{
							SysConOut.WriteLine("        加载SystemParams.xml时出错!!!ConsumeCallPetJiFen小于0");
						}
						else
						{
							CallPetManager.ConsumeCallPetJiFen = nTemp;
							nTemp = GameManager.systemParamsList.GetParamValueDoubleByName("ZhaoHuan", 0.0);
							if (nTemp < 0.0)
							{
							}
							CallPetManager.CallPetGoodsID = (int)nTemp;
						}
					}
				}
			}
		}

		
		public static CallPetType GetCallPetType(int type = 1)
		{
			CallPetType config = null;
			lock (CallPetManager._CallPetMutex)
			{
				if (CallPetManager.CallPetTypeDict.ContainsKey(type))
				{
					config = CallPetManager.CallPetTypeDict[type];
				}
			}
			return config;
		}

		
		public static List<CallPetConfig> GetCallPetConfigList(bool freeCall)
		{
			List<CallPetConfig> result;
			lock (CallPetManager._CallPetMutex)
			{
				if (freeCall)
				{
					result = CallPetManager.FreeCallPetConfigList;
				}
				else
				{
					SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
					if (spAct != null && spAct.IsChouJiangOpen(SpecPActivityChouJiangType.TeQuanBuHuo))
					{
						result = CallPetManager.TeQuanCallPetConfigList;
					}
					else
					{
						JieRiFuLiActivity act = HuodongCachingMgr.GetJieriFuLiActivity();
						object o_placeholder = null;
						if (act != null && act.IsOpened(EJieRiFuLiType.CallPetReplace, out o_placeholder))
						{
							result = CallPetManager.HuoDongCallPetConfigList;
						}
						else
						{
							result = CallPetManager.CallPetConfigList;
						}
					}
				}
			}
			return result;
		}

		
		public static int GetCallPetPrice(int times)
		{
			int price = -1;
			lock (CallPetManager._CallPetMutex)
			{
				if (CallPetManager.CallPetPriceDict.ContainsKey(times))
				{
					price = CallPetManager.CallPetPriceDict[times];
				}
			}
			return price;
		}

		
		public static GoodsData GetPetByDbID(GameClient client, int id)
		{
			if (null != client.ClientData.PetList)
			{
				for (int i = 0; i < client.ClientData.PetList.Count; i++)
				{
					if (client.ClientData.PetList[i].Id == id)
					{
						return client.ClientData.PetList[i];
					}
				}
			}
			return null;
		}

		
		public static void AddPetData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 4000)
			{
				if (null == client.ClientData.PetList)
				{
					client.ClientData.PetList = new List<GoodsData>();
				}
				lock (client.ClientData.PetList)
				{
					client.ClientData.PetList.Add(goodsData);
				}
			}
		}

		
		public static GoodsData AddPetData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, int idelBagIndex, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
		{
			GoodsData gd = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = "1900-01-01 12:00:00",
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = binding,
				Jewellist = jewelList,
				BagIndex = idelBagIndex,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife
			};
			CallPetManager.AddPetData(client, gd);
			return gd;
		}

		
		public static void RemovePetGoodsData(GameClient client, GoodsData goodsData)
		{
			lock (client.ClientData.PetList)
			{
				if (null != client.ClientData.PetList)
				{
					client.ClientData.PetList.Remove(goodsData);
				}
			}
		}

		
		public static int GetIdleSlotOfBag(GameClient client)
		{
			int idelPos = -1;
			int result;
			if (null == client.ClientData.PetList)
			{
				result = 0;
			}
			else
			{
				List<int> usedBagIndex = new List<int>();
				for (int i = 0; i < client.ClientData.PetList.Count; i++)
				{
					if (usedBagIndex.IndexOf(client.ClientData.PetList[i].BagIndex) < 0)
					{
						usedBagIndex.Add(client.ClientData.PetList[i].BagIndex);
					}
				}
				for (int j = 0; j < CallPetManager.GetMaxPetCount(); j++)
				{
					if (usedBagIndex.IndexOf(j) < 0)
					{
						return j;
					}
				}
				result = idelPos;
			}
			return result;
		}

		
		public static int GetPetListCount(GameClient client)
		{
			int result;
			if (null == client.ClientData.PetList)
			{
				result = 0;
			}
			else
			{
				result = client.ClientData.PetList.Count;
			}
			return result;
		}

		
		public static int GetMaxPetCount()
		{
			return CallPetManager.MaxPetGridNum;
		}

		
		public static long getFreeSec(GameClient client)
		{
			double currSec = Global.GetOffsetSecond(TimeUtil.NowDateTime());
			double lastSec = Convert.ToDouble(Global.GetRoleParamByName(client, "CallPetFreeTime"));
			double nIntSec = CallPetManager.CallPetFreeHour * 60.0 * 60.0;
			return (long)Global.GMax(0.0, lastSec + nIntSec - currSec);
		}

		
		public static void ResetPetBagAllGoods(GameClient client)
		{
			if (null != client.ClientData.PetList)
			{
				lock (client.ClientData.PetList)
				{
					Dictionary<string, GoodsData> oldGoodsDict = new Dictionary<string, GoodsData>();
					List<GoodsData> toRemovedGoodsDataList = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.PetList.Count; i++)
					{
						if (client.ClientData.PetList[i].Using <= 0)
						{
							client.ClientData.PetList[i].BagIndex = 1;
							int gridNum = Global.GetGoodsGridNumByID(client.ClientData.PetList[i].GoodsID);
							if (gridNum > 1)
							{
								GoodsData oldGoodsData = null;
								string key = string.Format("{0}_{1}_{2}", client.ClientData.PetList[i].GoodsID, client.ClientData.PetList[i].Binding, Global.DateTimeTicks(client.ClientData.PetList[i].Endtime));
								if (oldGoodsDict.TryGetValue(key, out oldGoodsData))
								{
									int toAddNum = Global.GMin(gridNum - oldGoodsData.GCount, client.ClientData.PetList[i].GCount);
									oldGoodsData.GCount += toAddNum;
									client.ClientData.PetList[i].GCount -= toAddNum;
									client.ClientData.PetList[i].BagIndex = 1;
									oldGoodsData.BagIndex = 1;
									if (!Global.ResetBagGoodsData(client, client.ClientData.PetList[i]))
									{
										break;
									}
									if (oldGoodsData.GCount >= gridNum)
									{
										if (client.ClientData.PetList[i].GCount > 0)
										{
											oldGoodsDict[key] = client.ClientData.PetList[i];
										}
										else
										{
											oldGoodsDict.Remove(key);
											toRemovedGoodsDataList.Add(client.ClientData.PetList[i]);
										}
									}
									else if (client.ClientData.PetList[i].GCount <= 0)
									{
										toRemovedGoodsDataList.Add(client.ClientData.PetList[i]);
									}
								}
								else
								{
									oldGoodsDict[key] = client.ClientData.PetList[i];
								}
							}
						}
					}
					for (int i = 0; i < toRemovedGoodsDataList.Count; i++)
					{
						client.ClientData.PetList.Remove(toRemovedGoodsDataList[i]);
					}
					client.ClientData.PetList.Sort((GoodsData x, GoodsData y) => y.GoodsID - x.GoodsID);
					int index = 0;
					for (int i = 0; i < client.ClientData.PetList.Count; i++)
					{
						if (client.ClientData.PetList[i].Using <= 0)
						{
							bool flag2 = 0 == 0;
							client.ClientData.PetList[i].BagIndex = index++;
							if (!Global.ResetBagGoodsData(client, client.ClientData.PetList[i]))
							{
								break;
							}
						}
					}
				}
			}
			TCPOutPacket tcpOutPacket = null;
			if (null != client.ClientData.PetList)
			{
				lock (client.ClientData.PetList)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.PetList, Global._TCPManager.TcpOutPacketPool, 754);
				}
			}
			else
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.PetList, Global._TCPManager.TcpOutPacketPool, 754);
			}
			Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
		}

		
		public static CallSpriteResult CallPet(GameClient client, int times, out string strGetGoods)
		{
			strGetGoods = "";
			CallSpriteResult result;
			if (times != 1 && times != 10)
			{
				result = CallSpriteResult.ErrorParams;
			}
			else
			{
				CallPetType TypeData = CallPetManager.GetCallPetType(1);
				if (null == TypeData)
				{
					result = CallSpriteResult.ErrorConfig;
				}
				else if (client.ClientData.Level < TypeData.MinLevel)
				{
					result = CallSpriteResult.ErrorLevel;
				}
				else if (client.ClientData.Level > TypeData.MaxLevel)
				{
					result = CallSpriteResult.ErrorLevel;
				}
				else if (client.ClientData.ChangeLifeCount < TypeData.MinZhuanSheng)
				{
					result = CallSpriteResult.ErrorLevel;
				}
				else if (client.ClientData.ChangeLifeCount > TypeData.MaxZhuanSheng)
				{
					result = CallSpriteResult.ErrorLevel;
				}
				else
				{
					bool bFreeCall = false;
					bool bUseGoods = false;
					int bind = 0;
					if (1 == times)
					{
						if (CallPetManager.getFreeSec(client) <= 0L)
						{
							bFreeCall = true;
							bind = 1;
						}
					}
					if (!bFreeCall && CallPetManager.CallPetGoodsID > 0)
					{
						if (1 == times)
						{
							if (null != Global.GetGoodsByID(client, CallPetManager.CallPetGoodsID))
							{
								bUseGoods = true;
								bind = 1;
							}
						}
					}
					int nNeedLuckStar = CallPetManager.GetCallPetPrice(times);
					if (nNeedLuckStar < 0)
					{
						result = CallSpriteResult.ErrorConfig;
					}
					else
					{
						if (!bFreeCall && !bUseGoods)
						{
							if (Global.IsRoleHasEnoughMoney(client, nNeedLuckStar, 163) < 0 && !HuanLeDaiBiManager.GetInstance().HuanledaibiReplaceEnough(client, nNeedLuckStar, DaiBiSySType.JingLingLieQu))
							{
								return CallSpriteResult.ZuanShiNotEnough;
							}
						}
						if (CallPetManager.GetMaxPetCount() - CallPetManager.GetPetListCount(client) < times)
						{
							result = CallSpriteResult.SpriteBagIsFull;
						}
						else
						{
							if (!bFreeCall)
							{
								if (bUseGoods)
								{
									bool usedBinding = false;
									bool usedTimeLimited = false;
									if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, CallPetManager.CallPetGoodsID, 1, false, out usedBinding, out usedTimeLimited, false))
									{
										bUseGoods = false;
									}
								}
							}
							if (!bFreeCall && !bUseGoods)
							{
								if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -nNeedLuckStar, "精灵召唤", false, DaiBiSySType.JingLingLieQu))
								{
									return CallSpriteResult.ZuanShiNotEnough;
								}
								bind = 0;
							}
							for (int i = 0; i < times; i++)
							{
								CallPetConfig CfgData = null;
								List<CallPetConfig> CfgList = CallPetManager.GetCallPetConfigList(bFreeCall || bUseGoods);
								if (CfgList == null || CfgList.Count <= 0)
								{
									return CallSpriteResult.ErrorConfig;
								}
								int random = Global.GetRandomNumber(1, 100001);
								foreach (CallPetConfig item in CfgList)
								{
									if (random >= item.StartValues && random <= item.EndValues)
									{
										CfgData = item;
										break;
									}
								}
								LogManager.WriteLog(LogTypes.Info, string.Format("获取精灵随机数: random = {0}, GoodsID = {1}", random, CfgData.GoodsID), null, true);
								if (null != CfgData)
								{
									int nExcellenceProp = 0;
									if (CfgData.ZhuoYueFallID != -1)
									{
										nExcellenceProp = GameManager.GoodsPackMgr.GetExcellencePropertysID(CfgData.GoodsID, CfgData.ZhuoYueFallID);
									}
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, CfgData.GoodsID, CfgData.Num, 0, "", 0, bind, 4000, "", false, 1, "精灵召唤", "1900-01-01 12:00:00", 0, 0, 0, 0, nExcellenceProp, 0, 0, null, null, 0, true);
									strGetGoods += string.Format("{0},{1},{2},{3},{4},{5},{6}|", new object[]
									{
										CfgData.GoodsID,
										CfgData.Num,
										bind,
										0,
										0,
										0,
										nExcellenceProp
									});
								}
							}
							if (bFreeCall)
							{
								Global.UpdateRoleParamByName(client, "CallPetFreeTime", Global.GetOffsetSecond(TimeUtil.NowDateTime()).ToString(), true);
								if (client._IconStateMgr.CheckPetIcon(client))
								{
									client._IconStateMgr.SendIconStateToClient(client);
								}
							}
							else if (!bUseGoods)
							{
								int nPetJiFen = (int)((double)nNeedLuckStar * CallPetManager.ConsumeCallPetJiFen);
								GameManager.ClientMgr.ModifyPetJiFenValue(client, nPetJiFen, "精灵召唤", false, true);
							}
							result = CallSpriteResult.Success;
						}
					}
				}
			}
			return result;
		}

		
		public static CallSpriteResult MovePet(GameClient client, int dbid)
		{
			GoodsData goodsData = CallPetManager.GetPetByDbID(client, dbid);
			CallSpriteResult result;
			if (null == goodsData)
			{
				result = CallSpriteResult.GoodsNotExist;
			}
			else if (!Global.CanAddGoods(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, "1900-01-01 12:00:00", true, false))
			{
				result = CallSpriteResult.BagIsFull;
			}
			else
			{
				string[] dbFields = null;
				string strCmd = Global.FormatUpdateDBGoodsStr(new object[]
				{
					client.ClientData.RoleID,
					dbid,
					"*",
					"*",
					"*",
					"*",
					0,
					"*",
					"*",
					1,
					"*",
					0,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*"
				});
				TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strCmd, out dbFields, client.ServerId);
				if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
				{
					result = CallSpriteResult.DBSERVERERROR;
				}
				else if (dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
				{
					result = CallSpriteResult.DBSERVERERROR;
				}
				else
				{
					CallPetManager.RemovePetGoodsData(client, goodsData);
					goodsData.Site = 0;
					Global.AddGoodsData(client, goodsData);
					result = CallSpriteResult.Success;
				}
			}
			return result;
		}

		
		public static TCPProcessCmdResults ProcessGetPetList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				byte[] bytesData = DataHelper.ObjectToBytes<List<GoodsData>>(client.ClientData.PetList);
				GameManager.ClientMgr.SendToClient(client, bytesData, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetPetList", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static TCPProcessCmdResults ProcessGetPetUIInfo(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				string strcmd = string.Format("{0}:{1}", roleID, CallPetManager.getFreeSec(client));
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetPetUIInfo", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static TCPProcessCmdResults ProcessCallPetCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int times = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				string strGetGoods = "";
				CallSpriteResult result = CallPetManager.CallPet(client, times, out strGetGoods);
				string strcmd;
				if (result != CallSpriteResult.Success)
				{
					strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						(int)result,
						roleID,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					times,
					strGetGoods,
					CallPetManager.getFreeSec(client)
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessCallPetCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public static TCPProcessCmdResults ProcessMovePetCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_OK;
		}

		
		public static TCPProcessCmdResults ProcessResetPetBagCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				CallPetManager.ResetPetBagAllGoods(client);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessResetPetBagCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		private static object _CallPetMutex = new object();

		
		private static Dictionary<int, CallPetType> CallPetTypeDict = new Dictionary<int, CallPetType>();

		
		private static List<CallPetConfig> CallPetConfigList = new List<CallPetConfig>();

		
		private static List<CallPetConfig> FreeCallPetConfigList = new List<CallPetConfig>();

		
		private static List<CallPetConfig> HuoDongCallPetConfigList = new List<CallPetConfig>();

		
		private static List<CallPetConfig> TeQuanCallPetConfigList = new List<CallPetConfig>();

		
		private static double CallPetFreeHour = 60.0;

		
		private static Dictionary<int, int> CallPetPriceDict = new Dictionary<int, int>();

		
		private static double ConsumeCallPetJiFen = 0.1;

		
		private static int CallPetGoodsID = 0;

		
		public static int MaxPetGridNum = 240;
	}
}
