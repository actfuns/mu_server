using System;
using System.Collections.Generic;
using GameServer.Server;
using Server.Data;
using Server.Protocol;

namespace GameServer.Logic.Damon
{
	// Token: 0x020004CD RID: 1229
	public class UpgradeDamon
	{
		// Token: 0x060016C0 RID: 5824 RVA: 0x00162BD4 File Offset: 0x00160DD4
		public static void LoadUpgradeAttr()
		{
			string strAttrList = GameManager.systemParamsList.GetParamValueByName("PetQiangHuaProps");
			string[] arrAttr = strAttrList.Split(new char[]
			{
				'|'
			});
			if (arrAttr != null)
			{
				for (int i = 0; i < arrAttr.Length; i++)
				{
					string[] arrSingleAttr = arrAttr[i].Split(new char[]
					{
						','
					});
					if (arrSingleAttr != null || arrSingleAttr.Length != 2)
					{
						UpgradeDamon.UpgradeAttrDict[int.Parse(arrSingleAttr[0])] = double.Parse(arrSingleAttr[1]);
					}
				}
			}
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x00162C74 File Offset: 0x00160E74
		public static double GetPetQiangPer(int nPropIndex)
		{
			double PetQiang = 0.0;
			UpgradeDamon.UpgradeAttrDict.TryGetValue(nPropIndex, out PetQiang);
			return PetQiang;
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x00162CA0 File Offset: 0x00160EA0
		public static TCPProcessCmdResults UpgradeDamonProcess(TCPOutPacketPool pool, GameClient client, GoodsData goodsData, out TCPOutPacket tcpOutPacket, int nID, TCPClientPool tcpClientPool, TCPManager tcpMgr)
		{
			tcpOutPacket = null;
			SystemXmlItem xmlItem = null;
			TCPProcessCmdResults result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out xmlItem) || null == xmlItem)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					-13,
					client.ClientData.RoleID,
					goodsData.Id,
					0,
					0
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				result = TCPProcessCmdResults.RESULT_DATA;
			}
			else
			{
				int MaxUpgradeLevel = xmlItem.GetIntValue("SuitID", -1) * 10 + 9;
				if (goodsData.Forge_level >= MaxUpgradeLevel)
				{
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-4,
						client.ClientData.RoleID,
						goodsData.Id,
						goodsData.Forge_level,
						goodsData.Binding
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else if (goodsData.Site != 5000)
				{
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-9,
						client.ClientData.RoleID,
						goodsData.Id,
						goodsData.Forge_level,
						goodsData.Binding
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else
				{
					SystemXmlItem xmlItems = null;
					GameManager.SystemDamonUpgrade.SystemXmlItemDict.TryGetValue(goodsData.Forge_level + 2, out xmlItems);
					if (null == xmlItems)
					{
						string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-6,
							client.ClientData.RoleID,
							goodsData.Id,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
						result = TCPProcessCmdResults.RESULT_DATA;
					}
					else
					{
						int nReqMoHe = xmlItems.GetIntValue("NeedEXP", -1);
						long lHaveMoHe = (long)GameManager.ClientMgr.GetMUMoHeValue(client);
						if (lHaveMoHe < (long)nReqMoHe)
						{
							string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								-11,
								client.ClientData.RoleID,
								goodsData.Id,
								goodsData.Forge_level,
								goodsData.Binding
							});
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							result = TCPProcessCmdResults.RESULT_DATA;
						}
						else
						{
							GameManager.ClientMgr.ModifyMUMoHeValue(client, -nReqMoHe, "精灵升级", true, true, false);
							int nBingProp = 1;
							string[] dbFields = null;
							string strDbCmd = Global.FormatUpdateDBGoodsStr(new object[]
							{
								client.ClientData.RoleID,
								goodsData.Id,
								"*",
								goodsData.Forge_level + 1,
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
								"*",
								nBingProp,
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*"
							});
							TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(tcpClientPool, pool, 10006, strDbCmd, out dbFields, client.ServerId);
							if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
							{
								string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
								{
									-10,
									client.ClientData.RoleID,
									goodsData.Id,
									0,
									0
								});
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								result = TCPProcessCmdResults.RESULT_DATA;
							}
							else if (dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
							{
								string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
								{
									-10,
									client.ClientData.RoleID,
									goodsData.Id,
									0,
									0
								});
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								result = TCPProcessCmdResults.RESULT_DATA;
							}
							else
							{
								int oldUsing = goodsData.Using;
								if (goodsData.Using > 0)
								{
									goodsData.Using = 0;
									Global.RefreshEquipProp(client, goodsData);
								}
								goodsData.Forge_level++;
								goodsData.Binding = nBingProp;
								JingLingQiYuanManager.getInstance().RefreshProps(client, true);
								if (oldUsing != goodsData.Using)
								{
									goodsData.Using = oldUsing;
									if (Global.RefreshEquipProp(client, goodsData))
									{
										GameManager.ClientMgr.NotifyUpdateEquipProps(tcpMgr.MySocketListener, pool, client);
										GameManager.ClientMgr.NotifyOthersLifeChanged(tcpMgr.MySocketListener, pool, client, true, false, 7);
									}
								}
								Global.ModRoleGoodsEvent(client, goodsData, 0, "强化", false);
								EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "强化");
								string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
								{
									1,
									client.ClientData.RoleID,
									goodsData.Id,
									goodsData.Forge_level,
									nBingProp
								});
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
								result = TCPProcessCmdResults.RESULT_DATA;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04002072 RID: 8306
		private static Dictionary<int, double> UpgradeAttrDict = new Dictionary<int, double>();
	}
}
