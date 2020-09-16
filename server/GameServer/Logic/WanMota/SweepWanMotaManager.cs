using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic.WanMota
{
	
	internal class SweepWanMotaManager
	{
		
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

		
		public static void UpdataSweepInfo(GameClient client, List<SingleLayerRewardData> listRewardData)
		{
			client.sendCmd<List<SingleLayerRewardData>>(617, listRewardData, false);
		}

		
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

		
		public static readonly int nSweepReqMinLayerOrder = 1;

		
		public static readonly int nWanMoTaSweepFuBenOrder = 19999;

		
		public static readonly int nWanMoTaMaxSweepNum = 1;
	}
}
