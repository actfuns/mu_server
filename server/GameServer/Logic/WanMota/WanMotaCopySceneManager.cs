using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.WanMota
{
	
	public class WanMotaCopySceneManager
	{
		
		
		public static int nWanMoTaFirstFuBenOrder
		{
			get
			{
				return WanMotaCopySceneManager._firstFuBenOrder_Impl;
			}
		}

		
		
		public static int nWanMoTaLastFuBenOrder
		{
			get
			{
				return WanMotaCopySceneManager._lastFuBenOrderImpl;
			}
		}

		
		public static bool IsWanMoTaMapCode(int mapCode)
		{
			return mapCode >= WanMotaCopySceneManager.nWanMoTaFirstFuBenOrder && mapCode <= WanMotaCopySceneManager.nWanMoTaLastFuBenOrder;
		}

		
		public static int WanmotaIsSweeping(GameClient client)
		{
			int result;
			if (client.ClientData.WanMoTaProp != null && client.ClientData.WanMoTaSweeping != null && client.ClientData.WanMoTaProp.nSweepLayer >= 0 && null != client.ClientData.WanMoTaSweeping.WanMoTaSweepingTimer)
			{
				result = 0;
			}
			else
			{
				result = 1;
			}
			return result;
		}

		
		public static WanMotaInfo GetWanMoTaDetail(GameClient client, bool bIsLogin)
		{
			WanMotaInfo dataWanMoTa = null;
			if (1 == WanMotaCopySceneManager.WanmotaIsSweeping(client))
			{
				dataWanMoTa = Global.sendToDB<WanMotaInfo, byte[]>(10159, DataHelper.ObjectToBytes<int>(client.ClientData.RoleID), client.ServerId);
				client.ClientData.WanMoTaProp = dataWanMoTa;
			}
			else
			{
				dataWanMoTa = client.ClientData.WanMoTaProp;
			}
			if (null != dataWanMoTa)
			{
				if (bIsLogin)
				{
					if (null != client.ClientData.WanMoTaProp)
					{
						byte[] bytes = Convert.FromBase64String(client.ClientData.WanMoTaProp.strSweepReward);
						client.ClientData.LayerRewardData = DataHelper.BytesToObject<LayerRewardData>(bytes, 0, bytes.Length);
					}
				}
				else if (null != client.ClientData.LayerRewardData)
				{
					lock (client.ClientData.LayerRewardData)
					{
						if (client.ClientData.LayerRewardData.WanMoTaLayerRewardList.Count > 0)
						{
							int nBeginIndex = 0;
							if (client.ClientData.LayerRewardData.WanMoTaLayerRewardList.Count > 10)
							{
								nBeginIndex = client.ClientData.LayerRewardData.WanMoTaLayerRewardList.Count - 10;
							}
							List<SingleLayerRewardData> listRewardData = new List<SingleLayerRewardData>();
							for (int i = nBeginIndex; i < client.ClientData.LayerRewardData.WanMoTaLayerRewardList.Count; i++)
							{
								listRewardData.Add(client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i]);
							}
							SweepWanMotaManager.UpdataSweepInfo(client, listRewardData);
						}
					}
				}
			}
			return dataWanMoTa;
		}

		
		public static void GetBossReward(GameClient client, int nFubenID, List<GoodsData> goodNormal, List<int> GoodsIDList)
		{
			SystemXmlItem systemFuBenItem = null;
			if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(nFubenID, out systemFuBenItem))
			{
				int nBossGoodsPackID = systemFuBenItem.GetIntValue("BossGoodsList", -1);
				if (nBossGoodsPackID > 0)
				{
					int maxFallCountByID = GameManager.GoodsPackMgr.GetFallGoodsMaxCount(nBossGoodsPackID);
					if (maxFallCountByID <= 0)
					{
						maxFallCountByID = GoodsPackManager.MaxFallCount;
					}
					List<GoodsData> goodsDataList = GameManager.GoodsPackMgr.GetGoodsDataList(client, nBossGoodsPackID, maxFallCountByID, 0, 1.0);
					if (goodsDataList != null && goodsDataList.Count > 0)
					{
						for (int i = 0; i < goodsDataList.Count; i++)
						{
							goodNormal.Add(goodsDataList[i]);
							GoodsIDList.Add(goodsDataList[i].GoodsID);
						}
					}
				}
			}
		}

		
		public static void AddRewardToClient(GameClient client, List<GoodsData> goodNormal, int nExp, int nMoney, int nXinHun, string strTitle)
		{
			if (null != goodNormal)
			{
				if (!Global.CanAddGoodsNum(client, goodNormal.Count))
				{
					Global.UseMailGivePlayerAward2(client, goodNormal, Global.GetLang(strTitle), Global.GetLang(strTitle), 0, 0, 0);
				}
				else
				{
					foreach (GoodsData item in goodNormal)
					{
						GoodsData goodsData = new GoodsData(item);
						goodsData.Id = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, strTitle, true, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
					}
				}
			}
			GameManager.ClientMgr.ModifyStarSoulValue(client, nXinHun, strTitle, true, true);
			GameManager.ClientMgr.ProcessRoleExperience(client, (long)nExp, true, true, false, "none");
			GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nMoney, "万魔塔", false);
		}

		
		public static void GetFubenItemReward(GameClient client, FuBenMapItem fuBenMapItem, bool bFirstPass, List<GoodsData> goodNormal, List<int> GoodsIDList)
		{
			if (bFirstPass)
			{
				if (null != fuBenMapItem.FirstGoodsDataList)
				{
					for (int i = 0; i < fuBenMapItem.FirstGoodsDataList.Count; i++)
					{
						goodNormal.Add(fuBenMapItem.FirstGoodsDataList[i]);
						GoodsIDList.Add(fuBenMapItem.FirstGoodsDataList[i].GoodsID);
					}
				}
			}
			else
			{
				if (null != fuBenMapItem.GoodsDataList)
				{
					for (int i = 0; i < fuBenMapItem.GoodsDataList.Count; i++)
					{
						goodNormal.Add(new GoodsData(fuBenMapItem.GoodsDataList[i]));
						GoodsIDList.Add(fuBenMapItem.GoodsDataList[i].GoodsID);
					}
				}
				WanMotaCopySceneManager.GetBossReward(client, fuBenMapItem.FuBenID, goodNormal, GoodsIDList);
			}
		}

		
		public static FuBenTongGuanData GiveCopyMapGiftNoScore(GameClient client, FuBenMapItem fuBenMapItem, bool bFirstPass)
		{
			FuBenTongGuanData result;
			if (null == fuBenMapItem)
			{
				result = null;
			}
			else
			{
				List<GoodsData> goodNormal = new List<GoodsData>();
				List<int> goodsID = new List<int>();
				WanMotaCopySceneManager.GetFubenItemReward(client, fuBenMapItem, bFirstPass, goodNormal, goodsID);
				if (bFirstPass)
				{
					WanMotaCopySceneManager.GetFubenItemReward(client, fuBenMapItem, false, goodNormal, goodsID);
				}
				FuBenTongGuanData fuBenTongGuanData = new FuBenTongGuanData();
				fuBenTongGuanData.FuBenID = fuBenMapItem.FuBenID;
				fuBenTongGuanData.TotalScore = 0;
				fuBenTongGuanData.KillNum = 0;
				fuBenTongGuanData.KillScore = 0;
				fuBenTongGuanData.MaxKillScore = 0;
				fuBenTongGuanData.UsedSecs = 0;
				fuBenTongGuanData.TimeScore = 0;
				fuBenTongGuanData.MaxTimeScore = 0;
				fuBenTongGuanData.DieCount = 0;
				fuBenTongGuanData.DieScore = 0;
				fuBenTongGuanData.MaxDieScore = 0;
				fuBenTongGuanData.GoodsIDList = goodsID;
				string strTitle;
				if (bFirstPass)
				{
					strTitle = string.Format(GLang.GetLang(570, new object[0]), client.ClientData.WanMoTaNextLayerOrder + 1);
				}
				else
				{
					strTitle = string.Format(GLang.GetLang(571, new object[0]), client.ClientData.WanMoTaNextLayerOrder + 1);
				}
				if (bFirstPass)
				{
					fuBenTongGuanData.AwardExp = fuBenMapItem.nFirstExp + fuBenMapItem.Experience;
					fuBenTongGuanData.AwardJinBi = fuBenMapItem.nFirstGold + fuBenMapItem.Money1;
					fuBenTongGuanData.AwardXingHun = fuBenMapItem.nFirstXingHunAward + fuBenMapItem.nXingHunAward;
				}
				else
				{
					fuBenTongGuanData.AwardExp = fuBenMapItem.Experience;
					fuBenTongGuanData.AwardJinBi = fuBenMapItem.Money1;
					fuBenTongGuanData.AwardXingHun = fuBenMapItem.nXingHunAward;
				}
				WanMotaCopySceneManager.AddRewardToClient(client, goodNormal, fuBenTongGuanData.AwardExp, fuBenTongGuanData.AwardJinBi, fuBenTongGuanData.AwardXingHun, strTitle);
				int nWanMoTaNextLayerOrder = GameManager.ClientMgr.GetWanMoTaPassLayerValue(client) + 1;
				GameManager.ClientMgr.SaveWanMoTaPassLayerValue(client, nWanMoTaNextLayerOrder, false);
				client.ClientData.WanMoTaNextLayerOrder = nWanMoTaNextLayerOrder;
				SingletonTemplate<WanMoTaTopLayerManager>.Instance().OnClientPass(client, nWanMoTaNextLayerOrder);
				GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.WanMoTaCurrLayerOrder, 0);
				WanMoTaDBCommandManager.LayerChangeDBCommand(client, nWanMoTaNextLayerOrder);
				ProcessTask.ProcessRoleTaskVal(client, TaskTypes.WanMoTa, -1);
				result = fuBenTongGuanData;
			}
			return result;
		}

		
		public static SingleLayerRewardData AddSingleSweepReward(GameClient client, List<GoodsData> goodNormal, int nParamLayerOrder, int nParamExp, int nParamMoney, int nParamXinHun, out List<SingleLayerRewardData> listRewardData)
		{
			SingleLayerRewardData layerReward = new SingleLayerRewardData
			{
				nLayerOrder = nParamLayerOrder,
				nExp = nParamExp,
				nMoney = nParamMoney,
				nXinHun = nParamXinHun,
				sweepAwardGoodsList = goodNormal
			};
			listRewardData = new List<SingleLayerRewardData>();
			listRewardData.Add(layerReward);
			return layerReward;
		}

		
		public static void GetWanmotaSweepReward(GameClient client, int nFubenID)
		{
			FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(nFubenID, nFubenID);
			if (null != fuBenMapItem)
			{
				List<GoodsData> goodNormal = new List<GoodsData>();
				List<int> goodsID = new List<int>();
				WanMotaCopySceneManager.GetFubenItemReward(client, fuBenMapItem, false, goodNormal, goodsID);
				if (null == client.ClientData.LayerRewardData)
				{
					client.ClientData.LayerRewardData = new LayerRewardData();
				}
				if (WanMotaCopySceneManager.IsWanMoTaMapCode(nFubenID))
				{
					List<SingleLayerRewardData> listRewardData = null;
					SingleLayerRewardData layerReward = WanMotaCopySceneManager.AddSingleSweepReward(client, goodNormal, nFubenID - WanMotaCopySceneManager.nWanMoTaFirstFuBenOrder + 1, fuBenMapItem.Experience, fuBenMapItem.Money1, fuBenMapItem.nXingHunAward, out listRewardData);
					SweepWanMotaManager.UpdataSweepInfo(client, listRewardData);
					lock (client.ClientData.LayerRewardData)
					{
						client.ClientData.LayerRewardData.WanMoTaLayerRewardList.Add(layerReward);
					}
				}
			}
		}

		
		public static void SendMsgToClientForWanMoTaCopyMapAward(GameClient client, CopyMap copyMap, bool anyAlive)
		{
			if (copyMap != null)
			{
				int fuBenSeqID = FuBenManager.FindFuBenSeqIDByRoleID(client.ClientData.RoleID);
				FuBenTongGuanData fubenTongGuanData = null;
				bool bFirstPassWanMoTa = false;
				if (fuBenSeqID > 0)
				{
					FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
					if (null != fuBenInfoItem)
					{
						fuBenInfoItem.EndTicks = TimeUtil.NOW();
						int addFuBenNum = 1;
						if (fuBenInfoItem.nDayOfYear != TimeUtil.NowDateTime().DayOfYear)
						{
							addFuBenNum = 0;
						}
						int fuBenID = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
						if (fuBenID > 0)
						{
							if (WanMotaCopySceneManager.IsWanMoTaMapCode(client.ClientData.MapCode))
							{
								if (!Global.FuBenPassed(client, fuBenID))
								{
									bFirstPassWanMoTa = true;
								}
							}
							int usedSecs = (int)((fuBenInfoItem.EndTicks - fuBenInfoItem.StartTicks) / 1000L);
							Global.UpdateFuBenDataForQuickPassTimer(client, fuBenID, usedSecs, addFuBenNum);
							FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(fuBenID, client.ClientData.MapCode);
							fubenTongGuanData = WanMotaCopySceneManager.GiveCopyMapGiftNoScore(client, fuBenMapItem, true);
							GameManager.DBCmdMgr.AddDBCmd(10053, string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								client.ClientData.RoleID,
								Global.FormatRoleName(client, client.ClientData.RoleName),
								fuBenID,
								usedSecs
							}), null, client.ServerId);
						}
					}
				}
				GameManager.ClientMgr.NotifyAllFuBenBeginInfo(copyMap, client.ClientData.RoleID, !anyAlive);
				if (fubenTongGuanData != null && bFirstPassWanMoTa)
				{
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FuBenTongGuanData>(fubenTongGuanData, Global._TCPManager.TcpOutPacketPool, 521);
					if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		
		private static int _firstFuBenOrder_Impl = 20000;

		
		private static int _lastFuBenOrderImpl = 20149;
	}
}
