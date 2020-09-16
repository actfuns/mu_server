using System;
using System.Collections.Generic;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class WaBaoManager
	{
		
		public static TCPOutPacket ProcessRandomWaBao(GameClient client, TCPOutPacketPool pool, int cmd)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = -1
			};
			TCPOutPacket result;
			if (null != client.ClientData.WaBaoGoodsData)
			{
				goodsData.Id = -1000;
				result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
			}
			else
			{
				int waBaoGoodsID = (int)GameManager.systemParamsList.GetParamValueIntByName("WaBaoGoodsID", -1);
				if (Global.GetTotalGoodsCountByID(client, waBaoGoodsID) <= 0)
				{
					result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
				}
				else
				{
					bool usedBinding = false;
					bool usedTimeLimited = false;
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, pool, client, waBaoGoodsID, 1, false, out usedBinding, out usedTimeLimited, false))
					{
						result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
					}
					else
					{
						int randomNum = Global.GetRandomNumber(1, 10001);
						Dictionary<int, SystemXmlItem> systemXmlItemDict = GameManager.systemWaBaoMgr.SystemXmlItemDict;
						List<int> idsList = new List<int>();
						foreach (SystemXmlItem systemWaBaoItem in systemXmlItemDict.Values)
						{
							if (randomNum >= systemWaBaoItem.GetIntValue("StartValues", -1) && randomNum <= systemWaBaoItem.GetIntValue("EndValues", -1))
							{
								idsList.Add(systemWaBaoItem.GetIntValue("ID", -1));
							}
						}
						if (idsList.Count <= 0)
						{
							goodsData.Id = -20;
							result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
						}
						else
						{
							int index = Global.GetRandomNumber(0, idsList.Count);
							int randomID = idsList[index];
							SystemXmlItem waBaoItem = null;
							if (!GameManager.systemWaBaoMgr.SystemXmlItemDict.TryGetValue(randomID, out waBaoItem))
							{
								goodsData.Id = -30;
								result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
							}
							else
							{
								goodsData.Id = randomID;
								goodsData.GoodsID = waBaoItem.GetIntValue("GoodsID", -1);
								goodsData.Using = 0;
								goodsData.Forge_level = waBaoItem.GetIntValue("Level", -1);
								goodsData.Starttime = "1900-01-01 12:00:00";
								goodsData.Endtime = "1900-01-01 12:00:00";
								goodsData.Site = 0;
								goodsData.Quality = waBaoItem.GetIntValue("Quality", -1);
								goodsData.Props = "";
								goodsData.GCount = 1;
								goodsData.Binding = (usedBinding ? 1 : 0);
								goodsData.Jewellist = "";
								goodsData.BagIndex = 0;
								goodsData.AddPropIndex = 0;
								goodsData.BornIndex = 0;
								goodsData.Lucky = 0;
								goodsData.Strong = 0;
								goodsData.ExcellenceInfo = 0;
								goodsData.AppendPropLev = 0;
								goodsData.ChangeLifeLevForEquip = 0;
								client.ClientData.WaBaoGoodsData = goodsData;
								Global.BroadcastWaBaoGoodsHint(client, goodsData);
								result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
							}
						}
					}
				}
			}
			return result;
		}

		
		public static TCPOutPacket ProcessGetWaBaoGoodsData(GameClient client, TCPOutPacketPool pool, int cmd)
		{
			TCPOutPacket result;
			if (null == client.ClientData.WaBaoGoodsData)
			{
				string strcmd = string.Format("{0}:{1}", -1, client.ClientData.RoleID);
				result = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, cmd);
			}
			else if (!Global.CanAddGoods(client, client.ClientData.WaBaoGoodsData.GoodsID, client.ClientData.WaBaoGoodsData.GCount, client.ClientData.WaBaoGoodsData.Binding, client.ClientData.WaBaoGoodsData.Endtime, true, false))
			{
				string strcmd = string.Format("{0}:{1}", -10, client.ClientData.RoleID);
				result = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, cmd);
			}
			else
			{
				int dbRet = Global.AddGoodsDBCommand(pool, client, client.ClientData.WaBaoGoodsData.GoodsID, client.ClientData.WaBaoGoodsData.GCount, client.ClientData.WaBaoGoodsData.Quality, client.ClientData.WaBaoGoodsData.Props, client.ClientData.WaBaoGoodsData.Forge_level, client.ClientData.WaBaoGoodsData.Binding, client.ClientData.WaBaoGoodsData.Site, client.ClientData.WaBaoGoodsData.Jewellist, true, 1, "挖宝获取道具", "1900-01-01 12:00:00", client.ClientData.WaBaoGoodsData.AddPropIndex, client.ClientData.WaBaoGoodsData.BornIndex, client.ClientData.WaBaoGoodsData.Lucky, client.ClientData.WaBaoGoodsData.Strong, 0, 0, 0, null, null, 0, true);
				if (dbRet < 0)
				{
					string strcmd = string.Format("{0}:{1}", -10, client.ClientData.RoleID);
					result = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, cmd);
				}
				else
				{
					client.ClientData.WaBaoGoodsData = null;
					string strcmd = string.Format("{0}:{1}", 0, client.ClientData.RoleID);
					result = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, cmd);
				}
			}
			return result;
		}

		
		public static TCPOutPacket ProcessWaBaoByYaoShi(GameClient client, TCPOutPacketPool pool, int cmd, int idXiangZi, int idYaoShi, bool autoBuy)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = -1
			};
			TCPOutPacket result;
			if ("1" != GameManager.GameConfigMgr.GetGameConfigItemStr("keydigtreasure", "1"))
			{
				goodsData.Id = -20;
				result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
			}
			else
			{
				Dictionary<int, int> dictYaoShi = Global.GetYaoShiDiaoLuoForXiangZhi(idXiangZi);
				if (dictYaoShi == null || dictYaoShi.Count <= 0)
				{
					goodsData.Id = -30;
					result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
				}
				else
				{
					bool bCanOpen = false;
					foreach (int key in dictYaoShi.Keys)
					{
						if (key == idYaoShi)
						{
							bCanOpen = true;
							break;
						}
					}
					if (!bCanOpen)
					{
						goodsData.Id = -50;
						result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
					}
					else
					{
						bool existXiangZi = true;
						bool existYaoShi = true;
						bool needSubXiangZi = true;
						bool needSubYaoShi = true;
						if (!Global.CanAddGoodsNum(client, 1))
						{
							goodsData.Id = -300;
							result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
						}
						else
						{
							Dictionary<int, int> needGoods = new Dictionary<int, int>();
							if (Global.GetTotalGoodsCountByID(client, idXiangZi) <= 0)
							{
								existXiangZi = false;
								needGoods.Add(idXiangZi, 1);
							}
							if (0 != idYaoShi)
							{
								if (Global.GetTotalGoodsCountByID(client, idYaoShi) <= 0)
								{
									existYaoShi = false;
									needGoods.Add(idYaoShi, 1);
								}
							}
							int oldMoney = client.ClientData.UserMoney;
							int subMoney = 0;
							if (needGoods.Count > 0)
							{
								if (autoBuy)
								{
									subMoney = Global.SubUserMoneyForGoods(client, needGoods, "精雕细琢挖宝");
									if (subMoney <= 0)
									{
										goodsData.Id = subMoney;
										return DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
									}
									if (!existXiangZi)
									{
										needSubXiangZi = false;
									}
									if (!existYaoShi)
									{
										needSubYaoShi = false;
									}
								}
								else
								{
									if (!existXiangZi)
									{
										goodsData.Id = -100;
										return DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
									}
									if (!existYaoShi)
									{
										goodsData.Id = -200;
										return DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
									}
								}
							}
							if (0 == idYaoShi)
							{
								needSubYaoShi = false;
							}
							GoodsData retGoodsData = null;
							int ret = GoodsBaoXiang.ProcessFallByYaoShiWaBao(client, dictYaoShi[idYaoShi], needSubYaoShi ? idYaoShi : -1, needSubXiangZi ? idXiangZi : -1, out retGoodsData, (idYaoShi == 0) ? 1 : 0, subMoney);
							if (ret <= 0 || null == retGoodsData)
							{
								goodsData.Id = ret;
								result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
							}
							else
							{
								goodsData = retGoodsData;
								Global.BroadcastYaoShiWaBaoGoodsHint(client, goodsData, idYaoShi, idXiangZi);
								Global.AddDigTreasureWithYaoShiEvent(client, idYaoShi, idXiangZi, needSubYaoShi ? 1 : 0, needSubXiangZi ? 1 : 0, subMoney, oldMoney, client.ClientData.UserMoney, goodsData);
								result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
							}
						}
					}
				}
			}
			return result;
		}
	}
}
