using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using Server.Data;

namespace GameServer.Logic
{
	
	public class MergeNewGoods
	{
		
		private static CacheMergeItem GetCacheMergeItem(int mergeItemID)
		{
			CacheMergeItem cacheMergeItem = null;
			lock (MergeNewGoods.MergeItemsDict)
			{
				if (MergeNewGoods.MergeItemsDict.TryGetValue(mergeItemID, out cacheMergeItem))
				{
					return cacheMergeItem;
				}
			}
			SystemXmlItem systemMergeItem = null;
			CacheMergeItem result;
			if (!GameManager.systemGoodsMergeItems.SystemXmlItemDict.TryGetValue(mergeItemID, out systemMergeItem))
			{
				result = null;
			}
			else
			{
				List<int> origGoodsIDList = new List<int>();
				List<int> origGoodsNumList = new List<int>();
				string origGoodsIDs = systemMergeItem.GetStringValue("OrigGoodsIDs").Trim();
				if (!string.IsNullOrEmpty(origGoodsIDs))
				{
					string[] fields = origGoodsIDs.Split(new char[]
					{
						'|'
					});
					if (null != fields)
					{
						for (int i = 0; i < fields.Length; i++)
						{
							string[] fields2 = fields[i].Trim().Split(new char[]
							{
								','
							});
							if (fields2.Length == 2)
							{
								origGoodsIDList.Add(Convert.ToInt32(fields2[0]));
								origGoodsNumList.Add(Convert.ToInt32(fields2[1]));
							}
						}
					}
				}
				Dictionary<string, int> dictDestroyGoodsIDs = new Dictionary<string, int>();
				string destroyGoodsIDs = systemMergeItem.GetStringValue("destroy").Trim();
				if (!string.IsNullOrEmpty(destroyGoodsIDs))
				{
					string[] fields = destroyGoodsIDs.Split(new char[]
					{
						'|'
					});
					if (null != fields)
					{
						for (int i = 0; i < fields.Length; i++)
						{
							string[] fields2 = fields[i].Trim().Split(new char[]
							{
								','
							});
							if (fields2.Length == 2)
							{
								dictDestroyGoodsIDs[fields2[0]] = Convert.ToInt32(fields2[1]);
							}
						}
					}
				}
				List<int> tmpList = new List<int>();
				string newGoodID = systemMergeItem.GetStringValue("NewGoodsID");
				if (!string.IsNullOrEmpty(newGoodID))
				{
					string[] fields = newGoodID.Split(new char[]
					{
						'|'
					});
					if (null != fields)
					{
						for (int i = 0; i < fields.Length; i++)
						{
							int nID = Convert.ToInt32(fields[i]);
							if (nID > 0)
							{
								tmpList.Add(nID);
							}
						}
					}
				}
				cacheMergeItem = new CacheMergeItem
				{
					MergeType = systemMergeItem.GetIntValue("MergeType", -1),
					NewGoodsID = tmpList,
					OrigGoodsIDList = origGoodsIDList,
					OrigGoodsNumList = origGoodsNumList,
					DianJuan = systemMergeItem.GetIntValue("DianJuan", -1),
					Money = systemMergeItem.GetIntValue("Money", -1),
					JingYuan = systemMergeItem.GetIntValue("JingYuan", -1),
					SuccessRate = Global.GMin(systemMergeItem.GetIntValue("SuccessRate", -1), 100),
					DestroyGoodsIDs = dictDestroyGoodsIDs,
					PubStartTime = systemMergeItem.GetStringValue("PubStartTime"),
					PubEndTime = systemMergeItem.GetStringValue("PubEndTime")
				};
				lock (MergeNewGoods.MergeItemsDict)
				{
					MergeNewGoods.MergeItemsDict[mergeItemID] = cacheMergeItem;
				}
				result = cacheMergeItem;
			}
			return result;
		}

		
		public static int ReloadCacheMergeItems()
		{
			int ret = GameManager.systemGoodsMergeItems.ReloadLoadFromXMlFile();
			lock (MergeNewGoods.MergeItemsDict)
			{
				MergeNewGoods.MergeItemsDict.Clear();
			}
			return ret;
		}

		
		private static int CanMergeNewGoods(GameClient client, CacheMergeItem cacheMergeItem, int nMergeTargetItemID, bool bLeftGrid = false)
		{
			if (!string.IsNullOrEmpty(cacheMergeItem.PubStartTime) && !string.IsNullOrEmpty(cacheMergeItem.PubEndTime))
			{
				long startTime = Global.SafeConvertToTicks(cacheMergeItem.PubStartTime);
				long endTime = Global.SafeConvertToTicks(cacheMergeItem.PubEndTime);
				long nowTicks = TimeUtil.NOW();
				if (nowTicks < startTime || nowTicks > endTime)
				{
					return -50;
				}
			}
			int result;
			if (!Global.CanAddGoods(client, nMergeTargetItemID, 1, 0, "1900-01-01 12:00:00", true, bLeftGrid))
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < cacheMergeItem.OrigGoodsIDList.Count; i++)
				{
					int goodsNum = Global.GetTotalBindGoodsCountByID(client, cacheMergeItem.OrigGoodsIDList[i]);
					int goodsNum2 = Global.GetTotalNotBindGoodsCountByID(client, cacheMergeItem.OrigGoodsIDList[i]);
					if (goodsNum + goodsNum2 < cacheMergeItem.OrigGoodsNumList[i])
					{
						return -2;
					}
				}
				if (cacheMergeItem.DianJuan > 0)
				{
					if (client.ClientData.UserMoney < cacheMergeItem.DianJuan)
					{
						return -3;
					}
				}
				if (cacheMergeItem.Money > 0)
				{
					if (Global.GetTotalBindTongQianAndTongQianVal(client) < cacheMergeItem.Money)
					{
						return -4;
					}
				}
				if (cacheMergeItem.JingYuan > 0)
				{
					if (GameManager.ClientMgr.GetTianDiJingYuanValue(client) < cacheMergeItem.JingYuan)
					{
						return -7;
					}
				}
				result = 0;
			}
			return result;
		}

		
		private static bool JugeSucess(int mergeItemID, CacheMergeItem cacheMergeItem, int addSuccessPercent)
		{
			int randNum = Global.GetRandomNumber(0, 101);
			double awardmuti = 0.0;
			if (50 == mergeItemID)
			{
				JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null != activity)
				{
					JieRiMultConfig config = activity.GetConfig(13);
					if (null != config)
					{
						awardmuti += config.GetMult();
					}
				}
				SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != spAct)
				{
					awardmuti += spAct.GetMult(SpecPActivityBuffType.SPABT_MergeFruit);
				}
			}
			awardmuti = Math.Max(1.0, awardmuti);
			int successRate = (int)((double)cacheMergeItem.SuccessRate * awardmuti);
			return randNum <= successRate + addSuccessPercent;
		}

		
		private static int GetUsingGoodsNum(bool sucesss, CacheMergeItem cacheMergeItem, int goodsID, int goodsNum)
		{
			int result;
			if (sucesss)
			{
				result = goodsNum;
			}
			else if (!cacheMergeItem.DestroyGoodsIDs.ContainsKey(goodsID.ToString()))
			{
				result = goodsNum;
			}
			else
			{
				cacheMergeItem.DestroyGoodsIDs.TryGetValue(goodsID.ToString(), out goodsNum);
				result = goodsNum;
			}
			return result;
		}

		
		private static int ProcessMergeNewGoods(GameClient client, int mergeItemID, CacheMergeItem cacheMergeItem, int luckyGoodsID, int nUseBindItemFirst)
		{
			int newGoodsBinding = 0;
			int addSuccessPercent = 0;
			bool bLeftGrid = false;
			int nNewGoodsID = cacheMergeItem.NewGoodsID[0];
			if (cacheMergeItem.NewGoodsID.Count > 1)
			{
				if (!Global.CanAddGoodsNum(client, 1))
				{
					return -1;
				}
				nNewGoodsID = cacheMergeItem.NewGoodsID[Global.GetRandomNumber(0, cacheMergeItem.NewGoodsID.Count)];
				bLeftGrid = true;
			}
			int ret = MergeNewGoods.CanMergeNewGoods(client, cacheMergeItem, nNewGoodsID, bLeftGrid);
			int result;
			if (ret < 0)
			{
				result = ret;
			}
			else
			{
				if (luckyGoodsID > 0)
				{
					int luckyPercent = Global.GetLuckyValue(luckyGoodsID);
					if (luckyPercent > 0)
					{
						bool usedBinding = false;
						bool usedTimeLimited = false;
						if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, luckyGoodsID, 1, false, out usedBinding, out usedTimeLimited, false))
						{
							if (newGoodsBinding <= 0)
							{
								newGoodsBinding = (usedBinding ? 1 : 0);
							}
							addSuccessPercent = luckyPercent;
						}
					}
				}
				bool success = MergeNewGoods.JugeSucess(mergeItemID, cacheMergeItem, addSuccessPercent);
				for (int i = 0; i < cacheMergeItem.OrigGoodsIDList.Count; i++)
				{
					int usingGoodsNum = MergeNewGoods.GetUsingGoodsNum(success, cacheMergeItem, cacheMergeItem.OrigGoodsIDList[i], cacheMergeItem.OrigGoodsNumList[i]);
					int nBindGoodNum = Global.GetTotalBindGoodsCountByID(client, cacheMergeItem.OrigGoodsIDList[i]);
					int nNotBindGoodNum = Global.GetTotalNotBindGoodsCountByID(client, cacheMergeItem.OrigGoodsIDList[i]);
					if (usingGoodsNum > nBindGoodNum + nNotBindGoodNum)
					{
						return -10;
					}
					bool usedBinding = false;
					bool usedTimeLimited = false;
					if (nUseBindItemFirst > 0 && nBindGoodNum > 0)
					{
						int nSum;
						int nSubNum;
						if (usingGoodsNum > nBindGoodNum)
						{
							nSum = nBindGoodNum;
							nSubNum = usingGoodsNum - nBindGoodNum;
						}
						else
						{
							nSum = usingGoodsNum;
							nSubNum = 0;
						}
						if (nSum > 0)
						{
							if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.OrigGoodsIDList[i], nSum, false, out usedBinding, out usedTimeLimited, true))
							{
								return -10;
							}
						}
						if (nSubNum > 0)
						{
							if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.OrigGoodsIDList[i], nSubNum, false, out usedBinding, out usedTimeLimited, true))
							{
								return -10;
							}
						}
						newGoodsBinding = 1;
					}
					else
					{
						int nSum;
						int nSubNum;
						if (usingGoodsNum > nNotBindGoodNum)
						{
							nSum = nNotBindGoodNum;
							nSubNum = usingGoodsNum - nNotBindGoodNum;
						}
						else
						{
							nSum = usingGoodsNum;
							nSubNum = 0;
						}
						if (nSum > 0)
						{
							if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.OrigGoodsIDList[i], nSum, false, out usedBinding, out usedTimeLimited, true))
							{
								return -10;
							}
						}
						if (nSubNum > 0)
						{
							if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.OrigGoodsIDList[i], nSubNum, false, out usedBinding, out usedTimeLimited, true))
							{
								return -10;
							}
							newGoodsBinding = 1;
						}
					}
				}
				if (cacheMergeItem.DianJuan > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.DianJuan, "合成新物品", true, true, false, DaiBiSySType.None))
					{
						return -11;
					}
				}
				if (cacheMergeItem.Money > 0)
				{
					if (!Global.SubBindTongQianAndTongQian(client, cacheMergeItem.Money, "材料合成"))
					{
						return -12;
					}
				}
				if (cacheMergeItem.JingYuan > 0)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, -cacheMergeItem.JingYuan, "材料合成", true, true, false);
				}
				if (!success)
				{
					result = -1000;
				}
				else
				{
					int dbRet = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nNewGoodsID, 1, 0, "", 0, newGoodsBinding, 0, "", true, 1, "材料合成新物品", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
					if (dbRet < 0)
					{
						result = -20;
					}
					else
					{
						if (90 == Global.GetGoodsCatetoriy(nNewGoodsID))
						{
							if (Global.GetJewelLevel(nNewGoodsID) >= 6)
							{
								Global.BroadcastMergeJewelOk(client, nNewGoodsID);
							}
						}
						if (120 == Global.GetGoodsCatetoriy(nNewGoodsID))
						{
						}
						ChengJiuManager.OnFirstHeCheng(client);
						ChengJiuManager.OnRoleGoodsHeCheng(client, nNewGoodsID);
						SevenDayGoalEventObject evObj = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.HeChengTimes);
						evObj.Arg1 = nNewGoodsID;
						GlobalEventSource.getInstance().fireEvent(evObj);
						ProcessTask.ProcessAddTaskVal(client, TaskTypes.Merge_GuoShi, cacheMergeItem.MergeType, 1, new object[0]);
						result = 0;
					}
				}
			}
			return result;
		}

		
		public static int Process(GameClient client, int mergeItemID, int luckyGoodsID, int WingDBID, int CrystalDBID, int nUseBindItemFirst)
		{
			CacheMergeItem cacheMergeItem = MergeNewGoods.GetCacheMergeItem(mergeItemID);
			int result;
			if (null == cacheMergeItem)
			{
				result = -1000;
			}
			else
			{
				if (mergeItemID >= 4 && mergeItemID <= 6)
				{
					int ret = MergeNewGoods.CanMergeNewGoods(client, cacheMergeItem, cacheMergeItem.NewGoodsID[0], false);
					if (ret < 0)
					{
						return ret;
					}
					ret = MergeNewGoods.ProcessWingMerge(client, mergeItemID, luckyGoodsID, WingDBID, CrystalDBID, cacheMergeItem);
					if (ret < 0)
					{
						return ret;
					}
					ChengJiuManager.OnFirstHeCheng(client);
					ChengJiuManager.OnRoleGoodsHeCheng(client, cacheMergeItem.NewGoodsID[0]);
				}
				else
				{
					int ret = MergeNewGoods.ProcessMergeNewGoods(client, mergeItemID, cacheMergeItem, luckyGoodsID, nUseBindItemFirst);
					if (ret < 0)
					{
						return ret;
					}
				}
				result = 0;
			}
			return result;
		}

		
		public static int ProcessWingMerge(GameClient client, int mergeItemID, int luckyGoodsID, int WingDBID, int CrystalDBID, CacheMergeItem cacheMergeItem)
		{
			GoodsData goodData = null;
			if (mergeItemID == 5 || mergeItemID == 6)
			{
				if (WingDBID < 0)
				{
					return -304;
				}
				goodData = Global.GetGoodsByDbID(client, WingDBID);
				if (goodData == null)
				{
					return -305;
				}
			}
			bool usedBinding = false;
			bool usedTimeLimited = false;
			if (cacheMergeItem.OrigGoodsIDList != null)
			{
				for (int i = 0; i < cacheMergeItem.OrigGoodsIDList.Count; i++)
				{
					GoodsData goodsData = Global.GetGoodsByID(client, cacheMergeItem.OrigGoodsIDList[i]);
					if (null == goodsData)
					{
						return -301;
					}
					if (goodsData.GCount < cacheMergeItem.OrigGoodsNumList[i])
					{
						return -301;
					}
				}
				for (int i = 0; i < cacheMergeItem.OrigGoodsIDList.Count; i++)
				{
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cacheMergeItem.OrigGoodsIDList[i], cacheMergeItem.OrigGoodsNumList[i], false, out usedBinding, out usedTimeLimited, true))
					{
						return -301;
					}
				}
			}
			int newGoodsBinding = 0;
			List<int> nNeedCrystalID = MergeNewGoods.GetCrystalIDForWingMerge(client, mergeItemID);
			int result;
			if (nNeedCrystalID == null)
			{
				result = -302;
			}
			else
			{
				int nGoodsID = -1;
				bool usedBinding2 = false;
				bool usedTimeLimited2 = false;
				if (CrystalDBID > 0)
				{
					GoodsData goodsinfo = Global.GetGoodsByDbID(client, CrystalDBID);
					if (goodsinfo != null)
					{
						nGoodsID = goodsinfo.GoodsID;
						if (nNeedCrystalID.Count > 0 && !nNeedCrystalID.Contains(nGoodsID))
						{
							return -302;
						}
						if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsinfo.GoodsID, 1, false, out usedBinding2, out usedTimeLimited2, false))
						{
							if (newGoodsBinding <= 0)
							{
								newGoodsBinding = (usedBinding2 ? 1 : 0);
							}
						}
					}
				}
				if (!MergeNewGoods.RollWingMergeSuccess(client, cacheMergeItem, luckyGoodsID))
				{
					result = -300;
				}
				else
				{
					int nWingGoods = MergeNewGoods.GetFianlWingGoodsID(client, mergeItemID, nGoodsID, nNeedCrystalID);
					int ExcellenceProperty = MergeNewGoods.RollWingGoodsExcellenceProperty(mergeItemID);
					int nForge = 0;
					if (goodData != null)
					{
						nForge = goodData.Forge_level;
					}
					int dbRet = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, nWingGoods, 1, 0, "", nForge, newGoodsBinding, 0, "", true, 1, "材料合成新物品--翅膀合成", "1900-01-01 12:00:00", 0, 0, 0, 0, ExcellenceProperty, 0, 0, null, null, 0, true);
					if (dbRet < 0)
					{
						result = -20;
					}
					else
					{
						if (goodData != null)
						{
							bool usedBinding3 = false;
							bool usedTimeLimited3 = false;
							GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodData.GoodsID, 1, false, out usedBinding3, out usedTimeLimited3, false);
						}
						result = 0;
					}
				}
			}
			return result;
		}

		
		public static List<int> GetWingIDForWingMerge(GameClient client, int mergeItemID)
		{
			List<int> lRet = null;
			string StrWingID = GameManager.systemParamsList.GetParamValueByName("HeChengChiBang");
			string[] arrID = StrWingID.Split(new char[]
			{
				'|'
			});
			List<int> result;
			if (arrID.Length < 0 || arrID.Length > 3)
			{
				result = null;
			}
			else
			{
				List<List<int>> WingIDList = new List<List<int>>();
				for (int i = 0; i < arrID.Length; i++)
				{
					string[] sData = arrID[i].Split(new char[]
					{
						','
					});
					if (sData.Length != 3)
					{
						return null;
					}
					List<int> id = new List<int>();
					int nValue = -1;
					if (!int.TryParse(sData[0], out nValue))
					{
						return null;
					}
					id.Add(nValue);
					int nValue2 = -1;
					if (!int.TryParse(sData[1], out nValue2))
					{
						return null;
					}
					id.Add(nValue2);
					int nValue3 = -1;
					if (!int.TryParse(sData[2], out nValue3))
					{
						return null;
					}
					id.Add(nValue3);
					WingIDList.Add(id);
				}
				if (mergeItemID == 5)
				{
					result = WingIDList[0];
				}
				else if (mergeItemID == 6)
				{
					result = WingIDList[1];
				}
				else
				{
					result = lRet;
				}
			}
			return result;
		}

		
		public static int RollWingGoodsExcellenceProperty(int MergeID)
		{
			int nRet = 0;
			double[] ExcellenceArr = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WingMergeExcellencePropertyRandomID", ',');
			int result;
			if (ExcellenceArr == null || ExcellenceArr.Length != 3)
			{
				result = nRet;
			}
			else
			{
				int nIndex = -1;
				if (MergeID == 4)
				{
					nIndex = 0;
				}
				else if (MergeID == 5)
				{
					nIndex = 1;
				}
				else if (MergeID == 6)
				{
					nIndex = 2;
				}
				if (nIndex == -1)
				{
					result = nRet;
				}
				else
				{
					int nIndex2 = (int)ExcellenceArr[nIndex];
					if (nIndex2 == -1)
					{
						result = nRet;
					}
					else
					{
						ExcellencePropertyGroupItem excellencePropertyGroupItem = GameManager.GoodsPackMgr.GetExcellencePropertyGroupItem(nIndex2);
						if (excellencePropertyGroupItem == null || excellencePropertyGroupItem.ExcellencePropertyItems == null || excellencePropertyGroupItem.Max == null || excellencePropertyGroupItem.Max.Length <= 0)
						{
							result = nRet;
						}
						else
						{
							int nNum = 0;
							int rndPercent = Global.GetRandomNumber(1, 100001);
							int i;
							for (i = 0; i < excellencePropertyGroupItem.ExcellencePropertyItems.Length; i++)
							{
								if (rndPercent > excellencePropertyGroupItem.ExcellencePropertyItems[i].BasePercent && rndPercent <= excellencePropertyGroupItem.ExcellencePropertyItems[i].BasePercent + excellencePropertyGroupItem.ExcellencePropertyItems[i].SelfPercent)
								{
									nNum = excellencePropertyGroupItem.ExcellencePropertyItems[i].Num;
									break;
								}
							}
							List<int> idList = new List<int>();
							if (nNum > 0 && nNum <= excellencePropertyGroupItem.Max.Length)
							{
								int nCount = 0;
								do
								{
									int nProp = Global.GetRandomNumber(0, excellencePropertyGroupItem.Max.Length);
									if (idList.IndexOf(nProp) < 0)
									{
										idList.Add(nProp);
										nCount++;
									}
								}
								while (nCount != nNum);
							}
							i = 0;
							while (i < idList.Count && i < excellencePropertyGroupItem.Max.Length)
							{
								nRet |= Global.GetBitValue(excellencePropertyGroupItem.Max[idList[i]]);
								i++;
							}
							result = nRet;
						}
					}
				}
			}
			return result;
		}

		
		public static bool RollWingMergeSuccess(GameClient client, CacheMergeItem cacheMergeItem, int luckyGoodsID)
		{
			int newGoodsBinding = 0;
			int addSuccessPercent = 0;
			if (luckyGoodsID > 0)
			{
				int luckyPercent = Global.GetLuckyValue(luckyGoodsID);
				if (luckyPercent > 0)
				{
					bool usedBinding = false;
					bool usedTimeLimited = false;
					if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, luckyGoodsID, 1, false, out usedBinding, out usedTimeLimited, false))
					{
						if (newGoodsBinding <= 0)
						{
							int num = usedBinding ? 1 : 0;
						}
						addSuccessPercent = luckyPercent;
					}
				}
			}
			int randNum = Global.GetRandomNumber(0, 101);
			return randNum <= cacheMergeItem.SuccessRate + addSuccessPercent;
		}

		
		public static List<int> GetCrystalIDForWingMerge(GameClient client, int mergeItemID)
		{
			string nStrCrystal = GameManager.systemParamsList.GetParamValueByName("ZhiYeHeChengJingShi");
			string[] arr = nStrCrystal.Split(new char[]
			{
				'|'
			});
			List<int> result;
			if (arr.Length < 0 || arr.Length > 3)
			{
				result = null;
			}
			else
			{
				List<List<int>> CrystalIDList = new List<List<int>>();
				for (int i = 0; i < arr.Length; i++)
				{
					string[] sData = arr[i].Split(new char[]
					{
						','
					});
					if (sData.Length != 3)
					{
						return null;
					}
					List<int> id = new List<int>();
					int nValue = -1;
					if (!int.TryParse(sData[0], out nValue))
					{
						return null;
					}
					id.Add(nValue);
					int nValue2 = -1;
					if (!int.TryParse(sData[1], out nValue2))
					{
						return null;
					}
					id.Add(nValue2);
					int nValue3 = -1;
					if (!int.TryParse(sData[2], out nValue3))
					{
						return null;
					}
					id.Add(nValue3);
					CrystalIDList.Add(id);
				}
				if (mergeItemID == 4)
				{
					result = CrystalIDList[0];
				}
				else if (mergeItemID == 5)
				{
					result = CrystalIDList[1];
				}
				else if (mergeItemID == 6)
				{
					result = CrystalIDList[2];
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		
		public static List<int> GetWingMergeCreateGoodsID(GameClient client, int mergeItemID)
		{
			string StrWing = GameManager.systemParamsList.GetParamValueByName("WingMergeCreatedID");
			string[] arr = StrWing.Split(new char[]
			{
				'|'
			});
			List<int> result;
			if (arr.Length < 0 || arr.Length > 3)
			{
				result = null;
			}
			else
			{
				List<List<int>> WingIDList = new List<List<int>>();
				for (int i = 0; i < arr.Length; i++)
				{
					string[] sData = arr[i].Split(new char[]
					{
						','
					});
					if (sData.Length != 3)
					{
						return null;
					}
					List<int> id = new List<int>();
					int nValue = -1;
					if (!int.TryParse(sData[0], out nValue))
					{
						return null;
					}
					id.Add(nValue);
					int nValue2 = -1;
					if (!int.TryParse(sData[1], out nValue2))
					{
						return null;
					}
					id.Add(nValue2);
					int nValue3 = -1;
					if (!int.TryParse(sData[2], out nValue3))
					{
						return null;
					}
					id.Add(nValue3);
					WingIDList.Add(id);
				}
				if (mergeItemID == 4)
				{
					result = WingIDList[0];
				}
				else if (mergeItemID == 5)
				{
					result = WingIDList[1];
				}
				else if (mergeItemID == 6)
				{
					result = WingIDList[2];
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		
		public static int GetFianlWingGoodsID(GameClient client, int mergeItemID, int nGoodsID, List<int> nNeedCrystalID)
		{
			List<int> nGoods = MergeNewGoods.GetWingMergeCreateGoodsID(client, mergeItemID);
			int result;
			if (nGoods == null)
			{
				result = -303;
			}
			else
			{
				int nIndex = -1;
				if (nGoodsID != -1)
				{
					for (int i = 0; i < nNeedCrystalID.Count; i++)
					{
						if (nNeedCrystalID[i] == nGoodsID)
						{
							nIndex = i;
							break;
						}
					}
					if (nIndex == -1)
					{
						return -303;
					}
				}
				else
				{
					nIndex = Global.GetRandomNumber(0, 3);
				}
				if (nIndex < 0 || nIndex > 3)
				{
					nIndex = 0;
				}
				int nWingGoods = nGoods[nIndex];
				result = nWingGoods;
			}
			return result;
		}

		
		private static Dictionary<int, CacheMergeItem> MergeItemsDict = new Dictionary<int, CacheMergeItem>();
	}
}
