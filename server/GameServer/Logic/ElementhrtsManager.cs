using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020004D3 RID: 1235
	internal class ElementhrtsManager
	{
		// Token: 0x060016D8 RID: 5848 RVA: 0x00164DE0 File Offset: 0x00162FE0
		public static ElementhrtsManager.RefineType GetRefineType(int Grade)
		{
			ElementhrtsManager.RefineType config = null;
			lock (ElementhrtsManager.RefineTypeDict)
			{
				if (ElementhrtsManager.RefineTypeDict.ContainsKey(Grade))
				{
					config = ElementhrtsManager.RefineTypeDict[Grade];
				}
			}
			return config;
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x00164E50 File Offset: 0x00163050
		public static void LoadRefineType()
		{
			string fileName = "Config/RefineType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, "加载Config/RefineType.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					lock (ElementhrtsManager.RefineTypeDict)
					{
						ElementhrtsManager.RefineTypeDict.Clear();
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (null != xmlItem)
							{
								ElementhrtsManager.RefineType config = new ElementhrtsManager.RefineType();
								config.Grade = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
								config.MinZhuanSheng = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MinZhuanSheng", "0"));
								config.MinLevel = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MinLevel", "0"));
								config.MaxZhuanSheng = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MaxZhuanSheng", "0"));
								config.MaxLevel = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MaxLevel", "0"));
								config.RefineCost = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "RefineCost", "0"));
								config.ZuanShiCost = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ZuanShiCost", "0"));
								config.SuccessRate = Global.GetSafeAttributeDouble(xmlItem, "SuccessRate");
								config.RefineLevel = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "RefineLevel", "0"));
								ElementhrtsManager.RefineTypeDict[config.Grade] = config;
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, "加载Config/RefineType.xml时文件出错", ex, true);
				}
			}
		}

		// Token: 0x060016DA RID: 5850 RVA: 0x001650A8 File Offset: 0x001632A8
		public static List<ElementhrtsManager.ElementHrtsBase> GetElementHrtsBase(int Grade)
		{
			List<ElementhrtsManager.ElementHrtsBase> cfgList = null;
			lock (ElementhrtsManager.ElementHrtsBaseDict)
			{
				if (ElementhrtsManager.ElementHrtsBaseDict.ContainsKey(Grade))
				{
					cfgList = ElementhrtsManager.ElementHrtsBaseDict[Grade];
				}
			}
			return cfgList;
		}

		// Token: 0x060016DB RID: 5851 RVA: 0x00165118 File Offset: 0x00163318
		public static void LoadElementHrtsBase()
		{
			string fileName = "Config/Refine.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, "加载Config/RefineType.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					lock (ElementhrtsManager.ElementHrtsBaseDict)
					{
						ElementhrtsManager.ElementHrtsBaseDict.Clear();
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (null != xmlItem)
							{
								int Grade = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "TypeID", "0"));
								List<ElementhrtsManager.ElementHrtsBase> baseList = new List<ElementhrtsManager.ElementHrtsBase>();
								IEnumerable<XElement> args = xmlItem.Elements();
								foreach (XElement arg in args)
								{
									baseList.Add(new ElementhrtsManager.ElementHrtsBase
									{
										ID = Convert.ToInt32(Global.GetDefAttributeStr(arg, "ID", "0")),
										GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(arg, "GoodsID", "0")),
										StartValues = Convert.ToInt32(Global.GetDefAttributeStr(arg, "StartValues", "0")),
										EndValues = Convert.ToInt32(Global.GetDefAttributeStr(arg, "EndValues", "0"))
									});
								}
								ElementhrtsManager.ElementHrtsBaseDict[Grade] = baseList;
							}
						}
					}
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Fatal, "加载Config/RefineType.xml时出现异常!!!", null, true);
				}
			}
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x00165374 File Offset: 0x00163574
		public static ElementhrtsManager.ElementHrtsLevelInfo GetElementHrtsLevelInfo(int grade, int level)
		{
			string key = grade.ToString() + "|" + level.ToString();
			ElementhrtsManager.ElementHrtsLevelInfo config = null;
			lock (ElementhrtsManager.ElementHrtsLevelDict)
			{
				if (ElementhrtsManager.ElementHrtsLevelDict.ContainsKey(key))
				{
					config = ElementhrtsManager.ElementHrtsLevelDict[key];
				}
			}
			return config;
		}

		// Token: 0x060016DD RID: 5853 RVA: 0x001653FC File Offset: 0x001635FC
		public static void LoadElementHrtsLevelInfo()
		{
			string fileName = "Config/ElementsHeart.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, "加载Config/ElementsHeart.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					lock (ElementhrtsManager.ElementHrtsLevelDict)
					{
						ElementhrtsManager.ElementHrtsLevelDict.Clear();
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (null != xmlItem)
							{
								int ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
								int Level = 0;
								int TotalExp = 0;
								IEnumerable<XElement> args = xmlItem.Elements();
								foreach (XElement arg in args)
								{
									ElementhrtsManager.ElementHrtsLevelInfo config = new ElementhrtsManager.ElementHrtsLevelInfo();
									config.Level = Convert.ToInt32(Global.GetDefAttributeStr(arg, "ID", "0"));
									if (Level + 1 != config.Level)
									{
										LogManager.WriteLog(LogTypes.Fatal, string.Format("加载Config/ElementsHeart.xml时出错!!!，{0}, {1}", ID, Level), null, true);
										return;
									}
									Level = config.Level;
									config.NeedExp = Convert.ToInt32(Global.GetDefAttributeStr(arg, "NeedExp", "0"));
									TotalExp += config.NeedExp;
									config.TotalExp = TotalExp;
									string key = ID.ToString() + "|" + Level.ToString();
									ElementhrtsManager.ElementHrtsLevelDict[key] = config;
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, "加载Config/ElementsHeart.xml时出现异常!!!", ex, true);
				}
			}
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x0016568C File Offset: 0x0016388C
		public static int GetSpecialElementHrtsExp(int GoodsID)
		{
			int Exp = 0;
			lock (ElementhrtsManager.SpecialExpDict)
			{
				if (ElementhrtsManager.SpecialExpDict.ContainsKey(GoodsID))
				{
					Exp = ElementhrtsManager.SpecialExpDict[GoodsID];
				}
			}
			return Exp;
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x001656FC File Offset: 0x001638FC
		public static void LoadSpecialElementHrtsExp()
		{
			lock (ElementhrtsManager.SpecialExpDict)
			{
				ElementhrtsManager.SpecialExpDict.Clear();
				string strParam = GameManager.systemParamsList.GetParamValueByName("SpecialElementsHeart");
				if (null == strParam)
				{
					SysConOut.WriteLine("SpecialElementsHeart 不存在，加载失败");
				}
				else
				{
					string[] fields = strParam.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < fields.Length; i++)
					{
						string[] str = fields[i].Split(new char[]
						{
							','
						});
						if (2 != str.Length)
						{
							SysConOut.WriteLine("加载SpecialElementsHeart时出现异常!!!");
						}
						int GoodsID = Convert.ToInt32(str[0]);
						int Exp = Convert.ToInt32(str[1]);
						ElementhrtsManager.SpecialExpDict[GoodsID] = Exp;
					}
				}
			}
		}

		// Token: 0x060016E0 RID: 5856 RVA: 0x00165804 File Offset: 0x00163A04
		public static bool IsElementHrt(int categoriy)
		{
			return categoriy >= 800 && categoriy < 816;
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x0016582C File Offset: 0x00163A2C
		public static GoodsData GetElementhrtsByDbID(GameClient client, int Site, int id)
		{
			List<GoodsData> goodsList = null;
			if (3000 == Site)
			{
				goodsList = client.ClientData.ElementhrtsList;
			}
			else if (3001 == Site)
			{
				goodsList = client.ClientData.UsingElementhrtsList;
			}
			GoodsData result;
			if (null == goodsList)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < goodsList.Count; i++)
				{
					if (goodsList[i].Id == id)
					{
						return goodsList[i];
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x001658C8 File Offset: 0x00163AC8
		public static GoodsData GetElementhrtsByDbID(GameClient client, int id)
		{
			if (null != client.ClientData.ElementhrtsList)
			{
				for (int i = 0; i < client.ClientData.ElementhrtsList.Count; i++)
				{
					if (client.ClientData.ElementhrtsList[i].Id == id)
					{
						return client.ClientData.ElementhrtsList[i];
					}
				}
			}
			if (null != client.ClientData.UsingElementhrtsList)
			{
				for (int i = 0; i < client.ClientData.UsingElementhrtsList.Count; i++)
				{
					if (client.ClientData.UsingElementhrtsList[i].Id == id)
					{
						return client.ClientData.UsingElementhrtsList[i];
					}
				}
			}
			return null;
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x001659B4 File Offset: 0x00163BB4
		public static void AddElementhrtsData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 3000)
			{
				if (null == client.ClientData.ElementhrtsList)
				{
					client.ClientData.ElementhrtsList = new List<GoodsData>();
				}
				lock (client.ClientData.ElementhrtsList)
				{
					client.ClientData.ElementhrtsList.Add(goodsData);
				}
			}
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x00165A4C File Offset: 0x00163C4C
		public static void AddUsingElementhrtsData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 3001)
			{
				if (null == client.ClientData.UsingElementhrtsList)
				{
					client.ClientData.UsingElementhrtsList = new List<GoodsData>();
				}
				lock (client.ClientData.UsingElementhrtsList)
				{
					client.ClientData.UsingElementhrtsList.Add(goodsData);
				}
			}
		}

		// Token: 0x060016E5 RID: 5861 RVA: 0x00165AE4 File Offset: 0x00163CE4
		public static GoodsData AddElementhrtsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, int idelBagIndex, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
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
			if (3000 == gd.Site)
			{
				ElementhrtsManager.AddElementhrtsData(client, gd);
			}
			if (3001 == gd.Site)
			{
				ElementhrtsManager.AddUsingElementhrtsData(client, gd);
			}
			return gd;
		}

		// Token: 0x060016E6 RID: 5862 RVA: 0x00165BDC File Offset: 0x00163DDC
		public static void RemoveElementhrtsData(GameClient client, GoodsData goodsData)
		{
			if (3000 == goodsData.Site)
			{
				lock (client.ClientData.ElementhrtsList)
				{
					if (null != client.ClientData.ElementhrtsList)
					{
						client.ClientData.ElementhrtsList.Remove(goodsData);
					}
				}
			}
			if (3001 == goodsData.Site)
			{
				lock (client.ClientData.UsingElementhrtsList)
				{
					if (null != client.ClientData.UsingElementhrtsList)
					{
						client.ClientData.UsingElementhrtsList.Remove(goodsData);
					}
				}
			}
		}

		// Token: 0x060016E7 RID: 5863 RVA: 0x00165CD8 File Offset: 0x00163ED8
		public static int GetIdleSlotOfBag(GameClient client)
		{
			int idelPos = -1;
			int result;
			if (null == client.ClientData.ElementhrtsList)
			{
				result = 0;
			}
			else
			{
				List<int> usedBagIndex = new List<int>();
				for (int i = 0; i < client.ClientData.ElementhrtsList.Count; i++)
				{
					if (usedBagIndex.IndexOf(client.ClientData.ElementhrtsList[i].BagIndex) < 0)
					{
						usedBagIndex.Add(client.ClientData.ElementhrtsList[i].BagIndex);
					}
				}
				for (int j = 0; j < ElementhrtsManager.GetMaxElementhrtsCount(); j++)
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

		// Token: 0x060016E8 RID: 5864 RVA: 0x00165DB4 File Offset: 0x00163FB4
		public static bool CanAddGoodsNum(GameClient client, int num)
		{
			return client != null && num > 0 && num + client.ClientData.ElementhrtsList.Count <= ElementhrtsManager.GetMaxElementhrtsCount();
		}

		// Token: 0x060016E9 RID: 5865 RVA: 0x00165E00 File Offset: 0x00164000
		public static int GetIdleSlotOfUsing(GameClient client)
		{
			int idelPos = -1;
			int result;
			if (null == client.ClientData.UsingElementhrtsList)
			{
				result = 0;
			}
			else
			{
				List<int> usedBagIndex = new List<int>();
				for (int i = 0; i < client.ClientData.UsingElementhrtsList.Count; i++)
				{
					if (usedBagIndex.IndexOf(client.ClientData.UsingElementhrtsList[i].BagIndex) < 0)
					{
						usedBagIndex.Add(client.ClientData.UsingElementhrtsList[i].BagIndex);
					}
				}
				for (int j = 0; j < ElementhrtsManager.GetMaxUsingElementhrtsCount(client); j++)
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

		// Token: 0x060016EA RID: 5866 RVA: 0x00165EDC File Offset: 0x001640DC
		public static int GetElementhrtsListCount(GameClient client)
		{
			int result;
			if (null == client.ClientData.ElementhrtsList)
			{
				result = 0;
			}
			else
			{
				result = client.ClientData.ElementhrtsList.Count;
			}
			return result;
		}

		// Token: 0x060016EB RID: 5867 RVA: 0x00165F18 File Offset: 0x00164118
		public static int GetUsingElementhrtsListCount(GameClient client)
		{
			int result;
			if (null == client.ClientData.UsingElementhrtsList)
			{
				result = 0;
			}
			else
			{
				result = client.ClientData.UsingElementhrtsList.Count;
			}
			return result;
		}

		// Token: 0x060016EC RID: 5868 RVA: 0x00165F54 File Offset: 0x00164154
		public static int GetMaxElementhrtsCount()
		{
			return ElementhrtsManager.MaxElementhrtsGridNum;
		}

		// Token: 0x060016ED RID: 5869 RVA: 0x00165F6C File Offset: 0x0016416C
		public static int GetMaxUsingElementhrtsCount(GameClient client)
		{
			int slotCount = Global.GetRoleParamsInt32FromDB(client, "10160");
			return Math.Max(ElementhrtsManager.MaxUsingElementhrtsGridNum, slotCount);
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x0016601C File Offset: 0x0016421C
		public static void SortElementhrtsList(GameClient client)
		{
			if (null != client.ClientData.ElementhrtsList)
			{
				client.ClientData.ElementhrtsList.Sort(delegate(GoodsData x, GoodsData y)
				{
					int result;
					if (Global.GetEquipGoodsSuitID(y.GoodsID) - Global.GetEquipGoodsSuitID(x.GoodsID) == 0)
					{
						if (x.GoodsID - y.GoodsID == 0)
						{
							result = x.Id - y.Id;
						}
						else
						{
							result = x.GoodsID - y.GoodsID;
						}
					}
					else
					{
						result = Global.GetEquipGoodsSuitID(y.GoodsID) - Global.GetEquipGoodsSuitID(x.GoodsID);
					}
					return result;
				});
				bool bModify = false;
				int bagindex = 0;
				foreach (GoodsData item in client.ClientData.ElementhrtsList)
				{
					if (item.BagIndex != bagindex)
					{
						item.BagIndex = bagindex;
						bool flag = 0 == 0;
						bModify = true;
					}
					bagindex++;
				}
				if (!bModify)
				{
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(null, Global._TCPManager.TcpOutPacketPool, 725);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
				else
				{
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.ElementhrtsList, Global._TCPManager.TcpOutPacketPool, 725);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
			}
		}

		// Token: 0x060016EF RID: 5871 RVA: 0x00166160 File Offset: 0x00164360
		private static void RequestElementHrtList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, GameClient client, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string strDBcmd = StringUtil.substitute("{0}:{1}", new object[]
			{
				client.ClientData.RoleID,
				3000
			});
			byte[] bytesCmd = new UTF8Encoding().GetBytes(strDBcmd);
			TCPProcessCmdResults result = Global.TransferRequestToDBServer(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, 204, bytesCmd, bytesCmd.Length, out tcpOutPacket, client.ServerId);
			if (TCPProcessCmdResults.RESULT_FAILED != result && null != tcpOutPacket)
			{
				List<GoodsData> goodsDataList = DataHelper.BytesToObject<List<GoodsData>>(tcpOutPacket.GetPacketBytes(), 6, tcpOutPacket.PacketDataSize - 6);
				client.ClientData.ElementhrtsList = goodsDataList;
				Global.PushBackTcpOutPacket(tcpOutPacket);
			}
			if (null == client.ClientData.ElementhrtsList)
			{
				client.ClientData.ElementhrtsList = new List<GoodsData>();
			}
		}

		// Token: 0x060016F0 RID: 5872 RVA: 0x0016624C File Offset: 0x0016444C
		public static TCPProcessCmdResults RequestElementExtend(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (2 != fields.Length)
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
				int result = -3;
				int bagindex = Convert.ToInt32(fields[1]);
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot4))
				{
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int slotCount = ElementhrtsManager.GetMaxUsingElementhrtsCount(client);
				if (bagindex != slotCount + 1)
				{
					result = -2;
				}
				else
				{
					List<string> list = GameManager.systemParamsList.GetParamValueStringListByName("ElementsHeartSlots", '|');
					if (list != null && list.Count > 0)
					{
						foreach (string str in list)
						{
							List<int> args = Global.StringToIntList(str, ',');
							if (args != null && args.Count == 2)
							{
								if (bagindex == args[0])
								{
									int needMoney = args[1];
									if (needMoney > 0)
									{
										if (client.ClientData.UserMoney >= needMoney)
										{
											if (GameManager.ClientMgr.SubUserMoney(client, needMoney, "扩展元素之心槽位", true, true, true, true, DaiBiSySType.None))
											{
												EventLogManager.AddRoleEvent(client, OpTypes.ElementhrtsSlotExtend, OpTags.Trace, LogRecordType.IntValue, new object[]
												{
													bagindex,
													"扩展元素之心槽位"
												});
												result = 0;
												Global.SaveRoleParamsInt32ValueToDB(client, "10160", bagindex, true);
											}
										}
										else
										{
											result = -10;
										}
									}
									else
									{
										result = -3;
									}
								}
							}
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetElementHrtList", false, false);
			}
			return TCPProcessCmdResults.RESULT_OK;
		}

		// Token: 0x060016F1 RID: 5873 RVA: 0x00166570 File Offset: 0x00164770
		public static TCPProcessCmdResults ProcessGetElementHrtList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (2 != fields.Length)
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
				int site = Convert.ToInt32(fields[1]);
				if (site == 3000)
				{
					byte[] bytesData = DataHelper.ObjectToBytes<List<GoodsData>>(client.ClientData.ElementhrtsList);
					GameManager.ClientMgr.SendToClient(client, bytesData, nID);
				}
				else if (site == 3001)
				{
					byte[] bytesData = DataHelper.ObjectToBytes<List<GoodsData>>(client.ClientData.UsingElementhrtsList);
					GameManager.ClientMgr.SendToClient(client, bytesData, nID);
				}
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetElementHrtList", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060016F2 RID: 5874 RVA: 0x00166748 File Offset: 0x00164948
		public static TCPProcessCmdResults ProcessGetElementHrtsInfo(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int currGrade = Global.GetRoleParamsInt32FromDB(client, "ElementGrade");
				if (currGrade <= 0)
				{
					currGrade = 1;
				}
				int currPowder = Global.GetRoleParamsInt32FromDB(client, "ElementPowder");
				int slotCount = ElementhrtsManager.GetMaxUsingElementhrtsCount(client);
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					roleID,
					currPowder,
					currGrade,
					slotCount
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetElementHrtsInfo", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x00166928 File Offset: 0x00164B28
		public static TCPProcessCmdResults ProcessUseElementHrt(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (3 != fields.Length)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int dbid = Convert.ToInt32(fields[1]);
				int state = Convert.ToInt32(fields[2]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				bool bEquip = state > 0;
				GoodsData goodsData = null;
				int newsite = 0;
				int newbagindex = 0;
				string strCmd;
				if (bEquip)
				{
					goodsData = ElementhrtsManager.GetElementhrtsByDbID(client, 3000, dbid);
					if (null == goodsData)
					{
						strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							1,
							roleID,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (3001 == goodsData.Site)
					{
						strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							3,
							roleID,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (ElementhrtsManager.GetUsingElementhrtsListCount(client) >= ElementhrtsManager.GetMaxUsingElementhrtsCount(client))
					{
						strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							5,
							roleID,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					ElementhrtsError result = ElementhrtsManager.CheckCanEquipElementHrt(client, goodsData.GoodsID);
					if (ElementhrtsError.Success != result)
					{
						strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							(int)result,
							roleID,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int bagindex = ElementhrtsManager.GetIdleSlotOfUsing(client);
					if (bagindex < 0)
					{
						strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							5,
							roleID,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					newsite = 3001;
					newbagindex = bagindex;
				}
				else
				{
					goodsData = ElementhrtsManager.GetElementhrtsByDbID(client, 3001, dbid);
					if (null == goodsData)
					{
						strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							1,
							roleID,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (3001 != goodsData.Site)
					{
						strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							2,
							roleID,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (ElementhrtsManager.GetElementhrtsListCount(client) >= ElementhrtsManager.GetMaxElementhrtsCount())
					{
						strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							4,
							roleID,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int bagindex = ElementhrtsManager.GetIdleSlotOfBag(client);
					if (bagindex < 0)
					{
						strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							4,
							roleID,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					newsite = 3000;
					newbagindex = bagindex;
				}
				string[] dbFields = null;
				strCmd = Global.FormatUpdateDBGoodsStr(new object[]
				{
					roleID,
					dbid,
					"*",
					"*",
					"*",
					"*",
					newsite,
					"*",
					"*",
					1,
					"*",
					newbagindex,
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
				TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(tcpClientPool, pool, 10006, strCmd, out dbFields, client.ServerId);
				if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
				{
					strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						16,
						dbid,
						goodsData.Site,
						goodsData.BagIndex,
						0
					});
					GameManager.ClientMgr.SendToClient(client, strCmd, nID);
					return TCPProcessCmdResults.RESULT_OK;
				}
				if (dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
				{
					strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						16,
						dbid,
						goodsData.Site,
						goodsData.BagIndex,
						0
					});
					GameManager.ClientMgr.SendToClient(client, strCmd, nID);
					return TCPProcessCmdResults.RESULT_OK;
				}
				GoodsData DamonData = null;
				if (null != client.ClientData.DamonGoodsDataList)
				{
					lock (client.ClientData.DamonGoodsDataList)
					{
						for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
						{
							GoodsData gd = client.ClientData.DamonGoodsDataList[i];
							if (gd.Using > 0)
							{
								DamonData = gd;
								break;
							}
						}
					}
				}
				ElementhrtsManager.RemoveElementhrtsData(client, goodsData);
				goodsData.Site = newsite;
				goodsData.BagIndex = newbagindex;
				if (bEquip)
				{
					ElementhrtsManager.AddUsingElementhrtsData(client, goodsData);
				}
				else
				{
					ElementhrtsManager.AddElementhrtsData(client, goodsData);
				}
				if (null != DamonData)
				{
					if (Global.RefreshEquipProp(client, goodsData))
					{
						GameManager.ClientMgr.NotifyUpdateEquipProps(tcpMgr.MySocketListener, pool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(tcpMgr.MySocketListener, pool, client, true, false, 7);
					}
				}
				GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, pool, client, 3, goodsData.Id, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, 1);
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					roleID,
					dbid,
					state,
					newbagindex
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessUseElementHrt", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060016F4 RID: 5876 RVA: 0x00167310 File Offset: 0x00165510
		public static ElementhrtsError CheckCanEquipElementHrt(GameClient client, int GoodsID)
		{
			SystemXmlItem systemGoods = null;
			ElementhrtsError result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(GoodsID, out systemGoods))
			{
				result = ElementhrtsError.ErrorConfig;
			}
			else
			{
				int categoriy = systemGoods.GetIntValue("Categoriy", -1);
				if (!ElementhrtsManager.IsElementHrt(categoriy))
				{
					result = ElementhrtsError.CantEquip;
				}
				else if (categoriy == 810)
				{
					result = ElementhrtsError.CantEquip;
				}
				else if (null == client.ClientData.UsingElementhrtsList)
				{
					result = ElementhrtsError.Success;
				}
				else
				{
					foreach (GoodsData item in client.ClientData.UsingElementhrtsList)
					{
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemGoods))
						{
							if (categoriy == systemGoods.GetIntValue("Categoriy", -1))
							{
								return ElementhrtsError.SameCategoriy;
							}
						}
					}
					result = ElementhrtsError.Success;
				}
			}
			return result;
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x00167430 File Offset: 0x00165630
		public static TCPProcessCmdResults ProcessGetSomeElementHrts(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (3 != fields.Length)
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
				int times = Convert.ToInt32(fields[1]);
				bool bUseMoney = Convert.ToInt32(fields[2]) > 0;
				string strCmd;
				if (1 != times && 10 != times)
				{
					strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						6,
						roleID,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (ElementhrtsManager.GetElementhrtsListCount(client) + times > ElementhrtsManager.GetMaxElementhrtsCount())
				{
					strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						7,
						roleID,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (times > 1 && bUseMoney)
				{
					strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						9,
						roleID,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string strResult = "";
				int nCount = 0;
				for (int i = 0; i < times; i++)
				{
					int GoodsID = 0;
					int EhtLevel = 0;
					ElementhrtsError result = ElementhrtsManager.GetSomeElementHrts(client, bUseMoney, out GoodsID, out EhtLevel);
					if (ElementhrtsError.Success != result)
					{
						strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							(int)result,
							roleID,
							0,
							0,
							strResult
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					strResult += GoodsID;
					strResult += ",";
					strResult += EhtLevel;
					strResult += "|";
					nCount++;
				}
				LogManager.WriteLog(LogTypes.Info, string.Format("玩家抽取获取元素之心 times = {0}, count = {1}", times, nCount), null, true);
				int currGrade = Global.GetRoleParamsInt32FromDB(client, "ElementGrade");
				if (currGrade <= 0)
				{
					currGrade = 1;
				}
				int currPowder = Global.GetRoleParamsInt32FromDB(client, "ElementPowder");
				strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					roleID,
					currPowder,
					currGrade,
					strResult
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessGetSomeElementHrts", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060016F6 RID: 5878 RVA: 0x00167894 File Offset: 0x00165A94
		public static ElementhrtsError GetSomeElementHrts(GameClient client, bool bUseMoney, out int GoodsID, out int EhtLevel)
		{
			GoodsID = 0;
			EhtLevel = 0;
			try
			{
				int currGrade = Global.GetRoleParamsInt32FromDB(client, "ElementGrade");
				if (currGrade <= 0)
				{
					currGrade = 1;
				}
				int currPowder = Global.GetRoleParamsInt32FromDB(client, "ElementPowder");
				if (bUseMoney)
				{
					currGrade = ElementhrtsManager.ZhuanShiGrade;
				}
				ElementhrtsManager.RefineType refinecfg = ElementhrtsManager.GetRefineType(currGrade);
				if (null == refinecfg)
				{
					return ElementhrtsError.ErrorConfig;
				}
				if (client.ClientData.Level < refinecfg.MinLevel)
				{
					return ElementhrtsError.ErrorLevel;
				}
				if (client.ClientData.Level > refinecfg.MaxLevel)
				{
					return ElementhrtsError.ErrorLevel;
				}
				if (client.ClientData.ChangeLifeCount < refinecfg.MinZhuanSheng)
				{
					return ElementhrtsError.ErrorLevel;
				}
				if (client.ClientData.ChangeLifeCount > refinecfg.MaxZhuanSheng)
				{
					return ElementhrtsError.ErrorLevel;
				}
				if (ElementhrtsManager.GetElementhrtsListCount(client) >= ElementhrtsManager.GetMaxElementhrtsCount())
				{
					return ElementhrtsError.BagNotEnough;
				}
				if (refinecfg.RefineCost > 0)
				{
					if (currPowder < refinecfg.RefineCost)
					{
						return ElementhrtsError.PowderNotEnough;
					}
				}
				if (refinecfg.ZuanShiCost > 0)
				{
					if (client.ClientData.UserMoney < refinecfg.ZuanShiCost)
					{
						return ElementhrtsError.MoneyNotEnough;
					}
				}
				if (refinecfg.ZuanShiCost > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, refinecfg.ZuanShiCost, "获取元素之心", true, true, false, DaiBiSySType.None))
					{
						return ElementhrtsError.MoneyNotEnough;
					}
				}
				if (refinecfg.RefineCost > 0)
				{
					GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, -refinecfg.RefineCost, "获取元素", true, false);
				}
				List<ElementhrtsManager.ElementHrtsBase> baseList = ElementhrtsManager.GetElementHrtsBase(currGrade);
				if (baseList == null || baseList.Count <= 0)
				{
					return ElementhrtsError.ErrorConfig;
				}
				int random = Global.GetRandomNumber(1, 100001);
				foreach (ElementhrtsManager.ElementHrtsBase item in baseList)
				{
					if (random >= item.StartValues && random <= item.EndValues)
					{
						GoodsID = item.GoodsID;
						break;
					}
				}
				LogManager.WriteLog(LogTypes.Info, string.Format("获取元素之心随机数: grade = {0}, random = {1}, GoodsID = {2}", currGrade, random, GoodsID), null, true);
				if (0 == GoodsID)
				{
					GoodsID = baseList[0].GoodsID;
					LogManager.WriteLog(LogTypes.Error, string.Format("获取元素之心获得配置异常: grade = {0}, random = {1}", currGrade, random), null, true);
				}
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(GoodsID, out systemGoods))
				{
					return ElementhrtsError.ErrorConfig;
				}
				if (null == systemGoods)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("GetSomeElementHrts: (null == systemGoods) GoodsID={0}", GoodsID), null, true);
					return ElementhrtsError.ErrorConfig;
				}
				string props = systemGoods.GetStringValue("EquipProps");
				int suitid = systemGoods.GetIntValue("SuitID", -1);
				int level = 1;
				int categoriy = systemGoods.GetIntValue("Categoriy", -1);
				List<int> elementhrtsProps = new List<int>();
				elementhrtsProps.Add(level);
				elementhrtsProps.Add(0);
				Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, GoodsID, 1, 0, "", 0, 1, 3000, "", false, 1, "获取元素之心", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, elementhrtsProps, 0, true);
				EhtLevel = level;
				int randIndex = Global.GetRandomNumber(0, 100);
				if ((double)randIndex <= refinecfg.SuccessRate * 100.0)
				{
					Global.SaveRoleParamsInt32ValueToDB(client, "ElementGrade", refinecfg.RefineLevel, true);
				}
				else
				{
					Global.SaveRoleParamsInt32ValueToDB(client, "ElementGrade", 1, true);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "GetSomeElementHrts", false, false);
			}
			return ElementhrtsError.Success;
		}

		// Token: 0x060016F7 RID: 5879 RVA: 0x00167D24 File Offset: 0x00165F24
		public static TCPProcessCmdResults ProcessPowerElementHrt(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length < 3)
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
				int srcid = Convert.ToInt32(fields[1]);
				int srcsite = Convert.ToInt32(fields[2]);
				List<int> materialList = new List<int>();
				string[] strMaterials = fields[3].Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < strMaterials.Length; i++)
				{
					if (strMaterials[i] != "")
					{
						materialList.Add(Convert.ToInt32(strMaterials[i]));
					}
				}
				if (materialList.Count <= 0)
				{
					string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						9,
						roleID,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (materialList.IndexOf(srcid) >= 0)
				{
					string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						9,
						roleID,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				GoodsData goodsData = ElementhrtsManager.GetElementhrtsByDbID(client, srcsite, srcid);
				if (null == goodsData)
				{
					string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						9,
						roleID,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
				{
					string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						15,
						roleID,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int categoriy = systemGoods.GetIntValue("Categoriy", -1);
				if (!ElementhrtsManager.IsElementHrt(categoriy))
				{
					string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						9,
						roleID,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (categoriy == 810)
				{
					string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						13,
						roleID,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int addExp = 0;
				foreach (int item in materialList)
				{
					GoodsData material = ElementhrtsManager.GetElementhrtsByDbID(client, item);
					if (null == material)
					{
						string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							15,
							roleID,
							0,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int exp = 0;
					SystemXmlItem systemMaterial = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(material.GoodsID, out systemMaterial))
					{
						string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							15,
							roleID,
							0,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int suitid = systemMaterial.GetIntValue("SuitID", -1);
					if (systemMaterial.GetIntValue("Categoriy", -1) == 810)
					{
						exp = ElementhrtsManager.GetSpecialElementHrtsExp(material.GoodsID);
					}
					else if (material.ElementhrtsProps != null && material.ElementhrtsProps.Count >= 2)
					{
						ElementhrtsManager.ElementHrtsLevelInfo materialInfo = ElementhrtsManager.GetElementHrtsLevelInfo(systemMaterial.GetIntValue("SuitID", -1), material.ElementhrtsProps[0]);
						if (null != materialInfo)
						{
							exp = materialInfo.TotalExp + material.ElementhrtsProps[1];
						}
					}
					addExp += exp;
				}
				foreach (int item in materialList)
				{
					GoodsData material = ElementhrtsManager.GetElementhrtsByDbID(client, item);
					if (null == material)
					{
						string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							15,
							roleID,
							0,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, tcpClientPool, pool, client, material, 1, false, false))
					{
						string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							15,
							roleID,
							0,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int SuitID = systemGoods.GetIntValue("SuitID", -1);
				int currLevel = 1;
				int currExp = 0;
				if (goodsData.ElementhrtsProps != null && goodsData.ElementhrtsProps.Count >= 2)
				{
					currLevel = goodsData.ElementhrtsProps[0];
					currExp = goodsData.ElementhrtsProps[1];
				}
				while (addExp > 0)
				{
					ElementhrtsManager.ElementHrtsLevelInfo info = ElementhrtsManager.GetElementHrtsLevelInfo(SuitID, currLevel + 1);
					if (null == info)
					{
						break;
					}
					int NeedExp = Global.GMax(0, info.NeedExp - currExp);
					if (NeedExp < 0)
					{
						break;
					}
					if (NeedExp > addExp)
					{
						currExp += addExp;
						addExp = 0;
					}
					else
					{
						currLevel++;
						currExp = 0;
						addExp -= NeedExp;
					}
				}
				UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
				{
					RoleID = client.ClientData.RoleID,
					DbID = srcid,
					WashProps = null
				};
				updateGoodsArgs.ElementhrtsProps = new List<int>();
				updateGoodsArgs.ElementhrtsProps.Add(currLevel);
				updateGoodsArgs.ElementhrtsProps.Add(currExp);
				GoodsData DamonData = null;
				if (null != client.ClientData.DamonGoodsDataList)
				{
					lock (client.ClientData.DamonGoodsDataList)
					{
						for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
						{
							GoodsData gd = client.ClientData.DamonGoodsDataList[i];
							if (gd.Using > 0)
							{
								DamonData = gd;
								break;
							}
						}
					}
				}
				bool bEquip = false;
				int oldsuit = goodsData.Site;
				if (null != DamonData)
				{
					if (3001 == goodsData.Site)
					{
						goodsData.Site = 3000;
						bEquip = Global.RefreshEquipProp(client, goodsData);
						goodsData.Site = oldsuit;
					}
				}
				Global.UpdateGoodsProp(client, goodsData, updateGoodsArgs, true);
				if (null != DamonData)
				{
					if (bEquip && Global.RefreshEquipProp(client, goodsData))
					{
						GameManager.ClientMgr.NotifyUpdateEquipProps(tcpMgr.MySocketListener, pool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(tcpMgr.MySocketListener, pool, client, true, false, 7);
					}
				}
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					0,
					roleID,
					srcid,
					srcsite,
					currLevel,
					currExp
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessPowerElementHrt", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x060016F8 RID: 5880 RVA: 0x00168884 File Offset: 0x00166A84
		public static TCPProcessCmdResults ProcessResetElementHrtBag(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length < 1)
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
				ElementhrtsManager.SortElementhrtsList(client);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessPowerElementHrt", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x040020C4 RID: 8388
		public static int MaxElementhrtsGridNum = 100;

		// Token: 0x040020C5 RID: 8389
		public static int MaxUsingElementhrtsGridNum = 8;

		// Token: 0x040020C6 RID: 8390
		public static int ZhuanShiGrade = 6;

		// Token: 0x040020C7 RID: 8391
		private static Dictionary<int, ElementhrtsManager.RefineType> RefineTypeDict = new Dictionary<int, ElementhrtsManager.RefineType>();

		// Token: 0x040020C8 RID: 8392
		private static Dictionary<int, List<ElementhrtsManager.ElementHrtsBase>> ElementHrtsBaseDict = new Dictionary<int, List<ElementhrtsManager.ElementHrtsBase>>();

		// Token: 0x040020C9 RID: 8393
		private static Dictionary<string, ElementhrtsManager.ElementHrtsLevelInfo> ElementHrtsLevelDict = new Dictionary<string, ElementhrtsManager.ElementHrtsLevelInfo>();

		// Token: 0x040020CA RID: 8394
		private static Dictionary<int, int> SpecialExpDict = new Dictionary<int, int>();

		// Token: 0x020004D4 RID: 1236
		public class RefineType
		{
			// Token: 0x040020CC RID: 8396
			public int Grade;

			// Token: 0x040020CD RID: 8397
			public int MinZhuanSheng;

			// Token: 0x040020CE RID: 8398
			public int MinLevel;

			// Token: 0x040020CF RID: 8399
			public int MaxZhuanSheng;

			// Token: 0x040020D0 RID: 8400
			public int MaxLevel;

			// Token: 0x040020D1 RID: 8401
			public int RefineCost;

			// Token: 0x040020D2 RID: 8402
			public int ZuanShiCost;

			// Token: 0x040020D3 RID: 8403
			public double SuccessRate;

			// Token: 0x040020D4 RID: 8404
			public int RefineLevel;
		}

		// Token: 0x020004D5 RID: 1237
		public class ElementHrtsBase
		{
			// Token: 0x040020D5 RID: 8405
			public int ID;

			// Token: 0x040020D6 RID: 8406
			public int GoodsID;

			// Token: 0x040020D7 RID: 8407
			public int StartValues;

			// Token: 0x040020D8 RID: 8408
			public int EndValues;
		}

		// Token: 0x020004D6 RID: 1238
		public class ElementHrtsLevelInfo
		{
			// Token: 0x040020D9 RID: 8409
			public int Level;

			// Token: 0x040020DA RID: 8410
			public int NeedExp;

			// Token: 0x040020DB RID: 8411
			public int TotalExp;
		}
	}
}
