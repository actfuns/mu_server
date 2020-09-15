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

namespace GameServer.Logic.MUWings
{
	// Token: 0x02000535 RID: 1333
	public class ZhuLingZhuHunManager
	{
		// Token: 0x0600195B RID: 6491 RVA: 0x0018B7FE File Offset: 0x001899FE
		private ZhuLingZhuHunManager()
		{
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x0018B80C File Offset: 0x00189A0C
		public static void LoadConfig()
		{
			string fileName = "Config/ZhuLingType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
			if (xml == null)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件不存在", fileName), null, true);
			}
			else
			{
				XElement xml2 = xml.Element("Types");
				if (xml2 == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件不存在", fileName), null, true);
				}
				else
				{
					IEnumerable<XElement> xmlItems = xml2.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						int id = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
						string strGoods = Global.GetSafeAttributeStr(xmlItem, "GoodsID");
						int bindJinBi = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "CostBandJinBi"));
						string[] goods = strGoods.Split(new char[]
						{
							','
						});
						if (goods.Length != 2)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!! ID={1} 消耗物品配置错误", fileName, id), null, true);
						}
						else
						{
							int goodsID = Convert.ToInt32(goods[0]);
							int goodsNum = Convert.ToInt32(goods[1]);
							if (id == 1)
							{
								ZhuLingZhuHunManager.ZhuLingCostGoodsID = goodsID;
								ZhuLingZhuHunManager.ZhuLingCostGoodsNum = goodsNum;
								ZhuLingZhuHunManager.ZhuLingCostJinBi = bindJinBi;
							}
							else if (id == 2)
							{
								ZhuLingZhuHunManager.ZhuHunCostGoodsID = goodsID;
								ZhuLingZhuHunManager.ZhuHunCostGoodsNum = goodsNum;
								ZhuLingZhuHunManager.ZhuHunCostJinBi = bindJinBi;
							}
						}
					}
				}
			}
			fileName = "Config/MaxWinZhuLing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
			if (xml == null)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件不存在", fileName), null, true);
			}
			else
			{
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					ZhuLingZhuHunLimit i = new ZhuLingZhuHunLimit();
					i.SuitID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "SuitID"));
					i.ZhuLingLimit = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "PlainZhuLing"));
					i.ZhuHunLimit = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "SeniorZhuLing"));
					ZhuLingZhuHunManager.Limit.Add(i);
				}
			}
			fileName = "Config/WinZhuLing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
			if (xml == null)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件不存在", fileName), null, true);
			}
			else
			{
				for (int j = 0; j < 6; j++)
				{
					ZhuLingZhuHunManager.Effect.Add(new ZhuLingZhuHunEffect());
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					int type = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "TypeID"));
					int occupation = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Occupation"));
					if (occupation < 0 || occupation >= ZhuLingZhuHunManager.Effect.Count<ZhuLingZhuHunEffect>())
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!! 职业配置有问题", fileName), null, true);
					}
					else
					{
						ZhuLingZhuHunManager.Effect[occupation].Occupation = occupation;
						if (type == 1)
						{
							ZhuLingZhuHunManager.Effect[occupation].MaxAttackV = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "MaxAttackV"));
							ZhuLingZhuHunManager.Effect[occupation].MaxMAttackV = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "MaxMAttackV"));
							ZhuLingZhuHunManager.Effect[occupation].MaxDefenseV = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "MaxDefenseV"));
							ZhuLingZhuHunManager.Effect[occupation].MaxMDefenseV = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "MaxMDefenseV"));
							ZhuLingZhuHunManager.Effect[occupation].LifeV = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "LifeV"));
							ZhuLingZhuHunManager.Effect[occupation].HitV = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "HitV"));
							ZhuLingZhuHunManager.Effect[occupation].DodgeV = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "DodgeV"));
						}
						else if (type == 2)
						{
							ZhuLingZhuHunManager.Effect[occupation].AllAttribute = Global.GetSafeAttributeDouble(xmlItem, "AllAttribute");
						}
					}
				}
			}
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x0018BD50 File Offset: 0x00189F50
		public static double GetZhuLingPct(GameClient client)
		{
			double pct = 0.0;
			double result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.WingZhuLing, false))
			{
				result = pct;
			}
			else
			{
				ZhuLingZhuHunLimit i = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				if (i == null)
				{
					result = pct;
				}
				else
				{
					pct = (double)client.ClientData.MyWingData.ZhuLingNum / (double)i.ZhuLingLimit;
					result = pct;
				}
			}
			return result;
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x0018BDC0 File Offset: 0x00189FC0
		public static bool IfZhuLingPerfect(GameClient client)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.WingZhuLing, false))
			{
				result = false;
			}
			else
			{
				ZhuLingZhuHunLimit i = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				result = (i != null && client.ClientData.MyWingData.ZhuLingNum >= i.ZhuLingLimit);
			}
			return result;
		}

		// Token: 0x0600195F RID: 6495 RVA: 0x0018BE2C File Offset: 0x0018A02C
		public static void SetZhuLingMax_GM(GameClient client)
		{
			ZhuLingZhuHunLimit i = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
			if (i != null)
			{
				client.ClientData.MyWingData.ZhuLingNum = i.ZhuLingLimit;
				MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.JinJieFailedNum, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum);
				ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				if (client._IconStateMgr.CheckReborn(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		// Token: 0x06001960 RID: 6496 RVA: 0x0018BF58 File Offset: 0x0018A158
		public static ZhuLingZhuHunError ReqZhuLing(GameClient client)
		{
			int oldLevel = client.ClientData.MyWingData.ZhuLingNum;
			int oldYinLiang = client.ClientData.YinLiang;
			int oldMoney = client.ClientData.Money1;
			ZhuLingZhuHunError result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.WingZhuLing, false))
			{
				result = ZhuLingZhuHunError.ZhuLingNotOpen;
			}
			else
			{
				ZhuLingZhuHunLimit i = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				if (i == null)
				{
					result = ZhuLingZhuHunError.ErrorConfig;
				}
				else if (client.ClientData.MyWingData.ZhuLingNum >= i.ZhuLingLimit)
				{
					result = ZhuLingZhuHunError.ZhuLingFull;
				}
				else if (Global.GetTotalGoodsCountByID(client, ZhuLingZhuHunManager.ZhuLingCostGoodsID) < ZhuLingZhuHunManager.ZhuLingCostGoodsNum)
				{
					result = ZhuLingZhuHunError.ZhuLingMaterialNotEnough;
				}
				else if (Global.GetTotalBindTongQianAndTongQianVal(client) < ZhuLingZhuHunManager.ZhuLingCostJinBi)
				{
					result = ZhuLingZhuHunError.ZhuLingJinBiNotEnough;
				}
				else if (!Global.SubBindTongQianAndTongQian(client, ZhuLingZhuHunManager.ZhuLingCostJinBi, "注灵消耗金币"))
				{
					result = ZhuLingZhuHunError.DBSERVERERROR;
				}
				else
				{
					string strCostList = EventLogManager.NewResPropString(ResLogType.SubJinbi, new object[]
					{
						-ZhuLingZhuHunManager.ZhuLingCostJinBi,
						oldYinLiang,
						client.ClientData.YinLiang,
						oldMoney,
						client.ClientData.Money1
					});
					bool bUsedBinding = true;
					bool bUsedTimeLimited = false;
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ZhuLingZhuHunManager.ZhuLingCostGoodsID, ZhuLingZhuHunManager.ZhuLingCostGoodsNum, false, out bUsedBinding, out bUsedTimeLimited, false))
					{
						result = ZhuLingZhuHunError.DBSERVERERROR;
					}
					else
					{
						GoodsData goodsDataCost = new GoodsData
						{
							GoodsID = ZhuLingZhuHunManager.ZhuLingCostGoodsID,
							GCount = ZhuLingZhuHunManager.ZhuLingCostGoodsNum
						};
						strCostList += EventLogManager.AddGoodsDataPropString(goodsDataCost);
						int iRet = MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.JinJieFailedNum, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, client.ClientData.MyWingData.ZhuLingNum + 1, client.ClientData.MyWingData.ZhuHunNum);
						if (iRet < 0)
						{
							result = ZhuLingZhuHunError.DBSERVERERROR;
						}
						else
						{
							client.ClientData.MyWingData.ZhuLingNum++;
							ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							EventLogManager.AddWingZhuLingEvent(client, oldLevel, client.ClientData.MyWingData.ZhuLingNum, strCostList);
							if (client._IconStateMgr.CheckReborn(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							result = ZhuLingZhuHunError.Success;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06001961 RID: 6497 RVA: 0x0018C274 File Offset: 0x0018A474
		public static TCPProcessCmdResults ProcessReqZhuLing(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				ZhuLingZhuHunError e = ZhuLingZhuHunManager.ReqZhuLing(client);
				string strcmd = string.Format("{0}:{1}:{2}", roleID, (int)e, client.ClientData.MyWingData.ZhuLingNum);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessReqZhuLing", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06001962 RID: 6498 RVA: 0x0018C3FC File Offset: 0x0018A5FC
		public static double GetZhuHunPct(GameClient client)
		{
			double pct = 0.0;
			double result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.WingZhuHun, false))
			{
				result = pct;
			}
			else
			{
				ZhuLingZhuHunLimit i = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				if (i == null)
				{
					result = pct;
				}
				else
				{
					pct = (double)client.ClientData.MyWingData.ZhuHunNum / (double)i.ZhuHunLimit;
					result = pct;
				}
			}
			return result;
		}

		// Token: 0x06001963 RID: 6499 RVA: 0x0018C46C File Offset: 0x0018A66C
		public static bool IfZhuHunPerfect(GameClient client)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.WingZhuHun, false))
			{
				result = false;
			}
			else
			{
				ZhuLingZhuHunLimit i = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				result = (i != null && client.ClientData.MyWingData.ZhuHunNum >= i.ZhuHunLimit);
			}
			return result;
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x0018C4D8 File Offset: 0x0018A6D8
		public static void SetZhuHunMax_GM(GameClient client)
		{
			ZhuLingZhuHunLimit i = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
			if (i != null)
			{
				client.ClientData.MyWingData.ZhuHunNum = i.ZhuHunLimit;
				MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.JinJieFailedNum, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum);
				ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				if (client._IconStateMgr.CheckReborn(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		// Token: 0x06001965 RID: 6501 RVA: 0x0018C604 File Offset: 0x0018A804
		public static ZhuLingZhuHunError ReqZhuHun(GameClient client)
		{
			int oldLevel = client.ClientData.MyWingData.ZhuHunNum;
			int oldYinLiang = client.ClientData.YinLiang;
			int oldMoney = client.ClientData.Money1;
			ZhuLingZhuHunError result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.WingZhuHun, false))
			{
				result = ZhuLingZhuHunError.ZhuHunNotOpen;
			}
			else
			{
				ZhuLingZhuHunLimit i = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				if (i == null)
				{
					result = ZhuLingZhuHunError.ErrorConfig;
				}
				else if (client.ClientData.MyWingData.ZhuHunNum >= i.ZhuHunLimit)
				{
					result = ZhuLingZhuHunError.ZhuHunFull;
				}
				else if (Global.GetTotalGoodsCountByID(client, ZhuLingZhuHunManager.ZhuHunCostGoodsID) < ZhuLingZhuHunManager.ZhuHunCostGoodsNum)
				{
					result = ZhuLingZhuHunError.ZhuHunMaterialNotEnough;
				}
				else if (Global.GetTotalBindTongQianAndTongQianVal(client) < ZhuLingZhuHunManager.ZhuHunCostJinBi)
				{
					result = ZhuLingZhuHunError.ZhuHunJinBiNotEnough;
				}
				else if (!Global.SubBindTongQianAndTongQian(client, ZhuLingZhuHunManager.ZhuHunCostJinBi, "注魂消耗"))
				{
					result = ZhuLingZhuHunError.DBSERVERERROR;
				}
				else
				{
					string strCostList = EventLogManager.NewResPropString(ResLogType.SubJinbi, new object[]
					{
						-ZhuLingZhuHunManager.ZhuHunCostJinBi,
						oldYinLiang,
						client.ClientData.YinLiang,
						oldMoney,
						client.ClientData.Money1
					});
					bool bUsedBinding = true;
					bool bUsedTimeLimited = false;
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ZhuLingZhuHunManager.ZhuHunCostGoodsID, ZhuLingZhuHunManager.ZhuHunCostGoodsNum, false, out bUsedBinding, out bUsedTimeLimited, false))
					{
						result = ZhuLingZhuHunError.DBSERVERERROR;
					}
					else
					{
						GoodsData goodsDataCost = new GoodsData
						{
							GoodsID = ZhuLingZhuHunManager.ZhuHunCostGoodsID,
							GCount = ZhuLingZhuHunManager.ZhuHunCostGoodsNum
						};
						strCostList += EventLogManager.AddGoodsDataPropString(goodsDataCost);
						int iRet = MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.JinJieFailedNum, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum + 1);
						if (iRet < 0)
						{
							result = ZhuLingZhuHunError.DBSERVERERROR;
						}
						else
						{
							client.ClientData.MyWingData.ZhuHunNum++;
							ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							EventLogManager.AddWingZhuHunEvent(client, oldLevel, client.ClientData.MyWingData.ZhuHunNum, strCostList);
							if (client._IconStateMgr.CheckReborn(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							result = ZhuLingZhuHunError.Success;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06001966 RID: 6502 RVA: 0x0018C920 File Offset: 0x0018AB20
		public static TCPProcessCmdResults ProcessReqZhuHun(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				ZhuLingZhuHunError e = ZhuLingZhuHunManager.ReqZhuHun(client);
				string strcmd = string.Format("{0}:{1}:{2}", roleID, (int)e, client.ClientData.MyWingData.ZhuHunNum);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessReqZhuHun", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x0018CAA8 File Offset: 0x0018ACA8
		public static void UpdateZhuLingZhuHunProps(GameClient client)
		{
			if (null != client.ClientData.MyWingData)
			{
				if (client.ClientData.MyWingData.WingID > 0)
				{
					ZhuLingZhuHunEffect e = ZhuLingZhuHunManager.GetEffect(Global.CalcOriginalOccupationID(client));
					if (e != null)
					{
						double MaxAttackV = 0.0;
						double MinAttackV = 0.0;
						double MaxMAttackV = 0.0;
						double MinMAttackV = 0.0;
						double MaxDefenseV = 0.0;
						double MinDefenseV = 0.0;
						double MaxMDefenseV = 0.0;
						double MinMDefenseV = 0.0;
						double LifeV = 0.0;
						double HitV = 0.0;
						double DodgeV = 0.0;
						if (client.ClientData.MyWingData.Using == 1)
						{
							MaxAttackV = (double)(e.MaxAttackV * client.ClientData.MyWingData.ZhuLingNum);
							MaxMAttackV = (double)(e.MaxMAttackV * client.ClientData.MyWingData.ZhuLingNum);
							MaxDefenseV = (double)(e.MaxDefenseV * client.ClientData.MyWingData.ZhuLingNum);
							MaxMDefenseV = (double)(e.MaxMDefenseV * client.ClientData.MyWingData.ZhuLingNum);
							LifeV = (double)(e.LifeV * client.ClientData.MyWingData.ZhuLingNum);
							HitV = (double)(e.HitV * client.ClientData.MyWingData.ZhuLingNum);
							DodgeV = (double)(e.DodgeV * client.ClientData.MyWingData.ZhuLingNum);
							double AllAttribute = e.AllAttribute;
							SystemXmlItem baseXmlNodeSuit = WingPropsCacheManager.GetWingPropsCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID);
							SystemXmlItem baseXmlNodeStar = WingStarCacheManager.GetWingStarCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel);
							if (baseXmlNodeSuit == null)
							{
								baseXmlNodeSuit = new SystemXmlItem();
							}
							if (baseXmlNodeStar == null)
							{
								baseXmlNodeStar = new SystemXmlItem();
							}
							MaxAttackV += (baseXmlNodeSuit.GetDoubleValue("MaxAttackV") + baseXmlNodeStar.GetDoubleValue("MaxAttackV")) * (AllAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							MinAttackV += (baseXmlNodeSuit.GetDoubleValue("MinAttackV") + baseXmlNodeStar.GetDoubleValue("MinAttackV")) * (AllAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							MaxMAttackV += (baseXmlNodeSuit.GetDoubleValue("MaxMAttackV") + baseXmlNodeStar.GetDoubleValue("MaxMAttackV")) * (AllAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							MinMAttackV += (baseXmlNodeSuit.GetDoubleValue("MinMAttackV") + baseXmlNodeStar.GetDoubleValue("MinMAttackV")) * (AllAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							MaxDefenseV += (baseXmlNodeSuit.GetDoubleValue("MaxDefenseV") + baseXmlNodeStar.GetDoubleValue("MaxDefenseV")) * (AllAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							MinDefenseV += (baseXmlNodeSuit.GetDoubleValue("MinDefenseV") + baseXmlNodeStar.GetDoubleValue("MinDefenseV")) * (AllAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							MaxMDefenseV += (baseXmlNodeSuit.GetDoubleValue("MaxMDefenseV") + baseXmlNodeStar.GetDoubleValue("MaxMDefenseV")) * (AllAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							MinMDefenseV += (baseXmlNodeSuit.GetDoubleValue("MinMDefenseV") + baseXmlNodeStar.GetDoubleValue("MinMDefenseV")) * (AllAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							LifeV += (baseXmlNodeSuit.GetDoubleValue("MaxLifeV") + baseXmlNodeStar.GetDoubleValue("MaxLifeV")) * (AllAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							HitV += (baseXmlNodeSuit.GetDoubleValue("HitV") + baseXmlNodeStar.GetDoubleValue("HitV")) * (AllAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							DodgeV += (baseXmlNodeSuit.GetDoubleValue("Dodge") + baseXmlNodeStar.GetDoubleValue("Dodge")) * (AllAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
						}
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							8,
							MaxAttackV
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							7,
							MinAttackV
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							10,
							MaxMAttackV
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							9,
							MinMAttackV
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							4,
							MaxDefenseV
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							3,
							MinDefenseV
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							6,
							MaxMDefenseV
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							5,
							MinMDefenseV
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							18,
							HitV
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							13,
							LifeV
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							19,
							DodgeV
						});
					}
				}
			}
		}

		// Token: 0x06001968 RID: 6504 RVA: 0x0018D18C File Offset: 0x0018B38C
		private static ZhuLingZhuHunEffect GetEffect(int Occupation)
		{
			foreach (ZhuLingZhuHunEffect e in ZhuLingZhuHunManager.Effect)
			{
				if (e.Occupation == Occupation)
				{
					return e;
				}
			}
			return null;
		}

		// Token: 0x06001969 RID: 6505 RVA: 0x0018D1FC File Offset: 0x0018B3FC
		private static ZhuLingZhuHunLimit GetLimit(int suit)
		{
			foreach (ZhuLingZhuHunLimit i in ZhuLingZhuHunManager.Limit)
			{
				if (i.SuitID == suit)
				{
					return i;
				}
			}
			return null;
		}

		// Token: 0x04002388 RID: 9096
		private static int ZhuLingCostGoodsID = 0;

		// Token: 0x04002389 RID: 9097
		private static int ZhuLingCostGoodsNum = 0;

		// Token: 0x0400238A RID: 9098
		private static int ZhuLingCostJinBi = 0;

		// Token: 0x0400238B RID: 9099
		private static int ZhuHunCostGoodsID = 0;

		// Token: 0x0400238C RID: 9100
		private static int ZhuHunCostGoodsNum = 0;

		// Token: 0x0400238D RID: 9101
		private static int ZhuHunCostJinBi = 0;

		// Token: 0x0400238E RID: 9102
		private static List<ZhuLingZhuHunLimit> Limit = new List<ZhuLingZhuHunLimit>();

		// Token: 0x0400238F RID: 9103
		private static List<ZhuLingZhuHunEffect> Effect = new List<ZhuLingZhuHunEffect>();
	}
}
