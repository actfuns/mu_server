using System;
using GameServer.Logic.Goods;
using GameServer.Server;
using Server.Data;

namespace GameServer.Logic.Damon
{
	// Token: 0x020004CC RID: 1228
	public class SaleDamons
	{
		// Token: 0x060016BD RID: 5821 RVA: 0x00162720 File Offset: 0x00160920
		public static TCPProcessCmdResults SaleDamonsProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			int nTotalMoHe = 0;
			string[] idsList = strGoodsID.Split(new char[]
			{
				','
			});
			int i = 0;
			while (i < idsList.Length)
			{
				int goodsDbID = Global.SafeConvertToInt32(idsList[i]);
				GoodsData goodsData = Global.GetGoodsByDbID(client, goodsDbID);
				if (goodsData != null && goodsData.Site == 0 && goodsData.Using <= 0)
				{
					int category = Global.GetGoodsCatetoriy(goodsData.GoodsID);
					if (category >= 9 && category <= 10)
					{
						SystemXmlItem xmlItem = null;
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out xmlItem) && null != xmlItem)
						{
							string modGoodsCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								4,
								goodsData.Id,
								goodsData.GoodsID,
								0,
								goodsData.Site,
								goodsData.GCount,
								goodsData.BagIndex,
								""
							});
							if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, modGoodsCmd, "客户端修改", null))
							{
								int nMoHePrice = xmlItem.GetIntValue("ZhanHunPrice", -1);
								if (nMoHePrice > 0)
								{
									nTotalMoHe += nMoHePrice;
								}
								for (int j = 0; j < goodsData.Forge_level; j++)
								{
									SystemXmlItem xmlItems = null;
									GameManager.SystemDamonUpgrade.SystemXmlItemDict.TryGetValue(j + 2, out xmlItems);
									if (null != xmlItems)
									{
										int nReqMoHe = xmlItems.GetIntValue("NeedEXP", -1);
										if (nReqMoHe > 0)
										{
											nTotalMoHe += nReqMoHe;
										}
									}
								}
								nTotalMoHe += (int)PetSkillManager.DelGoodsReturnLingJing(goodsData);
							}
						}
					}
				}
				IL_20D:
				i++;
				continue;
				goto IL_20D;
			}
			if (nTotalMoHe > 0)
			{
				GameManager.ClientMgr.ModifyMUMoHeValue(client, nTotalMoHe, "一键出售或者回收", true, true, false);
			}
			return TCPProcessCmdResults.RESULT_OK;
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x00162978 File Offset: 0x00160B78
		public static TCPProcessCmdResults SaleStoreDamonsProcess(GameClient client, int nRoleID, string strGoodsID)
		{
			int nTotalMoHe = 0;
			string[] idsList = strGoodsID.Split(new char[]
			{
				','
			});
			int i = 0;
			while (i < idsList.Length)
			{
				int goodsDbID = Global.SafeConvertToInt32(idsList[i]);
				GoodsData goodsData = CallPetManager.GetPetByDbID(client, goodsDbID);
				if (goodsData != null && goodsData.Site == 4000 && goodsData.Using <= 0)
				{
					int category = Global.GetGoodsCatetoriy(goodsData.GoodsID);
					if (category >= 9 && category <= 10)
					{
						SystemXmlItem xmlItem = null;
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out xmlItem) && null != xmlItem)
						{
							string modGoodsCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								4,
								goodsData.Id,
								goodsData.GoodsID,
								0,
								goodsData.Site,
								goodsData.GCount,
								goodsData.BagIndex,
								""
							});
							if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, modGoodsCmd, "客户端修改", null))
							{
								int nMoHePrice = xmlItem.GetIntValue("ZhanHunPrice", -1);
								if (nMoHePrice > 0)
								{
									nTotalMoHe += nMoHePrice;
								}
								for (int j = 0; j < goodsData.Forge_level; j++)
								{
									SystemXmlItem xmlItems = null;
									GameManager.SystemDamonUpgrade.SystemXmlItemDict.TryGetValue(j + 2, out xmlItems);
									if (null != xmlItems)
									{
										int nReqMoHe = xmlItems.GetIntValue("NeedEXP", -1);
										if (nReqMoHe > 0)
										{
											nTotalMoHe += nReqMoHe;
										}
									}
								}
							}
						}
					}
				}
				IL_207:
				i++;
				continue;
				goto IL_207;
			}
			if (nTotalMoHe > 0)
			{
				GameManager.ClientMgr.ModifyMUMoHeValue(client, nTotalMoHe, "一键出售或者回收", true, true, false);
			}
			return TCPProcessCmdResults.RESULT_OK;
		}
	}
}
