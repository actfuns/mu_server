using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000628 RID: 1576
	public class EnchaseJewelMgr
	{
		// Token: 0x06002041 RID: 8257 RVA: 0x001BD218 File Offset: 0x001BB418
		public static int ProcessEnchaseJewel(GameClient client, int actionType, int equipGoodsDbID, int jewelGoodsIDorDbID, out string jewellist, out int binding)
		{
			jewellist = "";
			binding = 0;
			GoodsData equipGoodsData = Global.GetGoodsByDbID(client, equipGoodsDbID);
			int result;
			if (null == equipGoodsData)
			{
				result = -1;
			}
			else if (equipGoodsData.Site != 0)
			{
				result = -9998;
			}
			else if (equipGoodsData.Using > 0)
			{
				result = -9999;
			}
			else
			{
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(equipGoodsData.GoodsID, out systemGoods))
				{
					result = -2;
				}
				else
				{
					int categoriy = systemGoods.GetIntValue("Categoriy", -1);
					if (categoriy < 0 || categoriy >= 49)
					{
						result = -3;
					}
					else
					{
						GoodsData jewelGoodsData = null;
						int jewelGoodsID;
						if (0 == actionType)
						{
							jewelGoodsData = Global.GetGoodsByDbID(client, jewelGoodsIDorDbID);
							if (jewelGoodsData == null || jewelGoodsData.GCount <= 0)
							{
								return -100;
							}
							jewelGoodsID = jewelGoodsData.GoodsID;
						}
						else
						{
							jewelGoodsID = jewelGoodsIDorDbID;
						}
						if (!Global.CanEnchaseJewel(jewelGoodsID))
						{
							result = -4;
						}
						else if (!Global.CanAddJewelIntoEquip(equipGoodsData.GoodsID, jewelGoodsID))
						{
							result = -5;
						}
						else
						{
							if (0 == actionType)
							{
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, jewelGoodsData, 1, false, false))
								{
									return -101;
								}
								if (!string.IsNullOrEmpty(equipGoodsData.Jewellist))
								{
									string[] jewelFields = equipGoodsData.Jewellist.Split(new char[]
									{
										','
									});
									if (jewelFields.Length >= 6)
									{
										return -110;
									}
								}
								jewellist = equipGoodsData.Jewellist;
								if (jewellist.Length > 0)
								{
									jewellist += ",";
								}
								jewellist += string.Format("{0}", jewelGoodsID);
								binding = equipGoodsData.Binding;
								if (equipGoodsData.Binding != jewelGoodsData.Binding)
								{
									if (jewelGoodsData.Binding > 0)
									{
										binding = 1;
									}
								}
								if (Global.ModGoodsJewelDBCommand(Global._TCPManager.TcpOutPacketPool, client, equipGoodsData, jewellist, binding) < 0)
								{
									return -102;
								}
							}
							else
							{
								if (!Global.CanAddGoods(client, jewelGoodsID, 1, 0, "1900-01-01 12:00:00", true, false))
								{
									return -200;
								}
								jewellist = equipGoodsData.Jewellist;
								if (string.IsNullOrEmpty(jewellist))
								{
									return -201;
								}
								string[] fields = jewellist.Split(new char[]
								{
									','
								});
								List<string> copyList = new List<string>();
								for (int i = 0; i < fields.Length; i++)
								{
									copyList.Add(fields[i].Trim());
								}
								bool findJewel = false;
								for (int i = 0; i < copyList.Count; i++)
								{
									if (copyList[i] == jewelGoodsID.ToString())
									{
										findJewel = true;
										copyList.RemoveAt(i);
										break;
									}
								}
								if (!findJewel)
								{
									return -300;
								}
								jewellist = "";
								for (int i = 0; i < copyList.Count; i++)
								{
									if (jewellist.Length > 0)
									{
										jewellist += ",";
									}
									jewellist += copyList[i];
								}
								if (Global.ModGoodsJewelDBCommand(Global._TCPManager.TcpOutPacketPool, client, equipGoodsData, jewellist, equipGoodsData.Binding) < 0)
								{
									return -202;
								}
								int dbRet = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, jewelGoodsID, 1, 0, "", 0, equipGoodsData.Binding, 0, "", true, 1, "宝石解镶嵌", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
								if (dbRet < 0)
								{
									return -203;
								}
							}
							result = 0;
						}
					}
				}
			}
			return result;
		}
	}
}
