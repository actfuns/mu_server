using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic.WanMota
{
	// Token: 0x020007B2 RID: 1970
	internal class SweepWanMotaManager
	{
		// Token: 0x060033CE RID: 13262 RVA: 0x002DE0E0 File Offset: 0x002DC2E0
		public static int GetSweepCount(GameClient client)
		{
			FuBenData fuBenData = Global.GetFuBenData(client, SweepWanMotaManager.nWanMoTaSweepFuBenOrder);
			int result;
			if (null == fuBenData)
			{
				result = 0;
			}
			else
			{
				result = fuBenData.EnterNum;
			}
			return result;
		}

		// Token: 0x060033CF RID: 13263 RVA: 0x002DE114 File Offset: 0x002DC314
		public static void SweepBegin(GameClient client)
		{
			if (client.ClientData.WanMoTaProp.nPassLayerCount >= SweepWanMotaManager.nSweepReqMinLayerOrder)
			{
				if (null == client.ClientData.WanMoTaSweeping)
				{
					client.ClientData.WanMoTaSweeping = new SweepWanmota(client);
				}
				client.ClientData.WanMoTaSweeping.nSweepingOrder = 1;
				client.ClientData.WanMoTaSweeping.nSweepingMaxOrder = client.ClientData.WanMoTaProp.nPassLayerCount;
				client.ClientData.WanMoTaProp.lFlushTime = TimeUtil.NOW();
				client.ClientData.WanMoTaSweeping.BeginSweeping();
				if (-1 != WanMoTaDBCommandManager.SweepBeginDBCommand(client, 1))
				{
					Global.UpdateFuBenData(client, SweepWanMotaManager.nWanMoTaSweepFuBenOrder, 1, 1);
				}
			}
		}

		// Token: 0x060033D0 RID: 13264 RVA: 0x002DE1E4 File Offset: 0x002DC3E4
		public static void SweepContinue(GameClient client)
		{
			if (client.ClientData.WanMoTaProp.nPassLayerCount >= SweepWanMotaManager.nSweepReqMinLayerOrder)
			{
				if (null == client.ClientData.WanMoTaSweeping)
				{
					client.ClientData.WanMoTaSweeping = new SweepWanmota(client);
				}
				client.ClientData.WanMoTaSweeping.nSweepingOrder = client.ClientData.WanMoTaProp.nSweepLayer;
				client.ClientData.WanMoTaSweeping.nSweepingMaxOrder = client.ClientData.WanMoTaProp.nPassLayerCount;
				client.ClientData.WanMoTaSweeping.BeginSweeping();
			}
		}

		// Token: 0x060033D1 RID: 13265 RVA: 0x002DE28D File Offset: 0x002DC48D
		public static void UpdataSweepInfo(GameClient client, List<SingleLayerRewardData> listRewardData)
		{
			client.sendCmd<List<SingleLayerRewardData>>(617, listRewardData, false);
		}

		// Token: 0x060033D2 RID: 13266 RVA: 0x002DE2A0 File Offset: 0x002DC4A0
		public static List<SingleLayerRewardData> SummarySweepRewardInfo(GameClient client)
		{
			List<SingleLayerRewardData> listRewardData = null;
			List<SingleLayerRewardData> result;
			if (client.ClientData.LayerRewardData == null || client.ClientData.LayerRewardData.WanMoTaLayerRewardList.Count < 1)
			{
				result = listRewardData;
			}
			else
			{
				int nExp = 0;
				int nMoney = 0;
				int nXinHun = 0;
				List<GoodsData> rewardList = new List<GoodsData>();
				lock (client.ClientData.LayerRewardData)
				{
					for (int i = 0; i < client.ClientData.LayerRewardData.WanMoTaLayerRewardList.Count; i++)
					{
						nExp += client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].nExp;
						nMoney += client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].nMoney;
						nXinHun += client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].nXinHun;
						if (null != client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].sweepAwardGoodsList)
						{
							for (int j = 0; j < client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].sweepAwardGoodsList.Count; j++)
							{
								SweepWanMotaManager.CombineGoodList(rewardList, client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].sweepAwardGoodsList[j]);
							}
						}
					}
					SingleLayerRewardData layerReward = WanMotaCopySceneManager.AddSingleSweepReward(client, rewardList, 0, nExp, nMoney, nXinHun, out listRewardData);
				}
				result = listRewardData;
			}
			return result;
		}

		// Token: 0x060033D3 RID: 13267 RVA: 0x002DE478 File Offset: 0x002DC678
		public static void CombineGoodList(List<GoodsData> goodList, GoodsData goodData)
		{
			int gridNum = Global.GetGoodsGridNumByID(goodData.GoodsID);
			if (gridNum > 1)
			{
				for (int i = 0; i < goodList.Count; i++)
				{
					if (goodList[i].GoodsID == goodData.GoodsID)
					{
						if (goodList[i].GCount + goodData.GCount <= gridNum)
						{
							goodList[i].GCount += goodData.GCount;
							return;
						}
					}
				}
			}
			goodList.Add(goodData);
		}

		// Token: 0x04003F65 RID: 16229
		public static readonly int nSweepReqMinLayerOrder = 1;

		// Token: 0x04003F66 RID: 16230
		public static readonly int nWanMoTaSweepFuBenOrder = 19999;

		// Token: 0x04003F67 RID: 16231
		public static readonly int nWanMoTaMaxSweepNum = 1;
	}
}
