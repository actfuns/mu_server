using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class HuodongCachingMgr
	{
		
		public static List<GoodsData> ParseGoodsDataList(string[] fields, string fileName)
		{
			List<GoodsData> goodsDataList = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] sa = fields[i].Split(new char[]
				{
					','
				});
				if (sa.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("解析{0}文件中的奖励项时失败, 物品配置项个数错误", fileName), null, true);
				}
				else
				{
					int[] goodsFields = Global.StringArray2IntArray(sa);
					GoodsData goodsData = Global.GetNewGoodsData(goodsFields[0], goodsFields[1], 0, goodsFields[3], goodsFields[2], 0, goodsFields[5], 0, goodsFields[6], goodsFields[4], 0);
					goodsDataList.Add(goodsData);
				}
			}
			return goodsDataList;
		}

		
		public static List<AwardEffectTimeItem.TimeDetail> ParseGoodsTimeList(string[] fields, string fileName)
		{
			List<AwardEffectTimeItem.TimeDetail> result2;
			if (fields == null)
			{
				result2 = null;
			}
			else
			{
				List<AwardEffectTimeItem.TimeDetail> result = new List<AwardEffectTimeItem.TimeDetail>();
				foreach (string str in fields)
				{
					AwardEffectTimeItem.TimeDetail detail = new AwardEffectTimeItem.TimeDetail();
					string[] szTime = str.Split(new char[]
					{
						','
					});
					int type = Convert.ToInt32(szTime[0]);
					bool bReadOK = false;
					if (type == 1)
					{
						if (szTime.Length == 2)
						{
							detail.Type = AwardEffectTimeItem.EffectTimeType.ETT_LastMinutesFromNow;
							detail.LastMinutes = Convert.ToInt32(szTime[1]);
							bReadOK = true;
						}
					}
					else if (type == 2)
					{
						if (szTime.Length == 3)
						{
							detail.Type = AwardEffectTimeItem.EffectTimeType.ETT_AbsoluteLastTime;
							detail.AbsoluteStartTime = szTime[1];
							detail.AbsoluteEndTime = szTime[2];
							bReadOK = true;
						}
					}
					if (!bReadOK)
					{
						detail.Type = AwardEffectTimeItem.EffectTimeType.ETT_AbsoluteLastTime;
						detail.AbsoluteStartTime = "1900-01-01 12:00:00";
						detail.AbsoluteEndTime = "1900-01-01 12:00:00";
					}
					result.Add(detail);
				}
				result2 = result;
			}
			return result2;
		}

		
		public static List<GoodsData> ParseGoodsDataList2(string[] fields, string fileName)
		{
			List<GoodsData> goodsDataList = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] sa = fields[i].Split(new char[]
				{
					','
				});
				if (sa.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("解析{0}文件中的奖励项时失败, 物品配置项个数错误", fileName), null, true);
				}
				else
				{
					int[] goodsFields = Global.StringArray2IntArray(sa);
					GoodsData goodsData = Global.GetNewGoodsData(goodsFields[0], goodsFields[1], 0, 0, 0, 0, 0, 0, 0, 0, 0);
					goodsDataList.Add(goodsData);
				}
			}
			return goodsDataList;
		}

		
		private static string ParseDateTime(string str)
		{
			int yue = str.IndexOf('月');
			string result;
			if (-1 == yue)
			{
				result = "";
			}
			else
			{
				int ri = str.IndexOf('日');
				if (-1 == ri)
				{
					result = "";
				}
				else
				{
					int shi = str.IndexOf('时');
					if (-1 == shi)
					{
						result = "";
					}
					else
					{
						int fen = str.IndexOf('分');
						if (-1 == fen)
						{
							result = "";
						}
						else
						{
							string yueStr = str.Substring(0, yue);
							if (string.IsNullOrEmpty(yueStr))
							{
								result = "";
							}
							else
							{
								string riStr = str.Substring(yue + 1, ri - yue - 1);
								if (string.IsNullOrEmpty(riStr))
								{
									result = "";
								}
								else
								{
									string shiStr = str.Substring(ri + 1, shi - ri - 1);
									if (string.IsNullOrEmpty(shiStr))
									{
										result = "";
									}
									else
									{
										string fenStr = str.Substring(shi + 1, fen - shi - 1);
										if (string.IsNullOrEmpty(fenStr))
										{
											result = "";
										}
										else
										{
											int year = TimeUtil.NowDateTime().Year;
											result = string.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", new object[]
											{
												year,
												yueStr,
												riStr,
												shiStr,
												fenStr,
												0
											});
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public static long GetHuoDongDateTime(string str)
		{
			string strDateTime = HuodongCachingMgr.ParseDateTime(str);
			return Global.SafeConvertToTicks(strDateTime);
		}

		
		public static long GetHuoDongDateTimeForCommonTimeString(string str)
		{
			return Global.SafeConvertToTicks(str);
		}

		
		private static int GetBitValue(int whichOne)
		{
			int bitVal = 0;
			if (1 == whichOne)
			{
				bitVal = 1;
			}
			else if (2 == whichOne)
			{
				bitVal = 2;
			}
			else if (3 == whichOne)
			{
				bitVal = 4;
			}
			else if (4 == whichOne)
			{
				bitVal = 8;
			}
			else if (5 == whichOne)
			{
				bitVal = 16;
			}
			else if (6 == whichOne)
			{
				bitVal = 32;
			}
			else if (7 == whichOne)
			{
				bitVal = 64;
			}
			return bitVal;
		}

		
		public static bool GiveAward(GameClient client, AwardItem myAwardItem, string goodsFromWere)
		{
			bool result;
			if (client == null || null == myAwardItem)
			{
				result = false;
			}
			else
			{
				if (myAwardItem.GoodsDataList != null)
				{
					for (int i = 0; i < myAwardItem.GoodsDataList.Count; i++)
					{
						int nGoodsID = myAwardItem.GoodsDataList[i].GoodsID;
						if (Global.IsCanGiveRewardByOccupation(client, nGoodsID))
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, myAwardItem.GoodsDataList[i].GoodsID, myAwardItem.GoodsDataList[i].GCount, myAwardItem.GoodsDataList[i].Quality, "", myAwardItem.GoodsDataList[i].Forge_level, myAwardItem.GoodsDataList[i].Binding, 0, "", true, 1, goodsFromWere, "1900-01-01 12:00:00", myAwardItem.GoodsDataList[i].AddPropIndex, myAwardItem.GoodsDataList[i].BornIndex, myAwardItem.GoodsDataList[i].Lucky, myAwardItem.GoodsDataList[i].Strong, myAwardItem.GoodsDataList[i].ExcellenceInfo, myAwardItem.GoodsDataList[i].AppendPropLev, myAwardItem.GoodsDataList[i].ChangeLifeLevForEquip, null, null, 0, true);
						}
					}
					client.ClientData.AddAwardRecord(RoleAwardMsg.CombatGift, myAwardItem.GoodsDataList, false);
				}
				if (myAwardItem.AwardYuanBao > 0)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myAwardItem.AwardYuanBao, goodsFromWere, ActivityTypes.None, "");
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(386, new object[0]), new object[]
					{
						myAwardItem.AwardYuanBao
					}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
					GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, myAwardItem.AwardYuanBao, goodsFromWere), null, client.ServerId);
				}
				result = true;
			}
			return result;
		}

		
		protected static bool GiveEffectiveTimeAward(GameClient client, AwardItem myAwardItem, string goodsFromWhere)
		{
			bool result;
			if (client == null || null == myAwardItem)
			{
				result = false;
			}
			else
			{
				if (myAwardItem.GoodsDataList != null)
				{
					for (int i = 0; i < myAwardItem.GoodsDataList.Count; i++)
					{
						int nGoodsID = myAwardItem.GoodsDataList[i].GoodsID;
						if (Global.IsCanGiveRewardByOccupation(client, nGoodsID))
						{
							Global.AddEffectiveTimeGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, myAwardItem.GoodsDataList[i].GoodsID, myAwardItem.GoodsDataList[i].GCount, myAwardItem.GoodsDataList[i].Quality, "", myAwardItem.GoodsDataList[i].Forge_level, myAwardItem.GoodsDataList[i].Binding, 0, "", false, 1, goodsFromWhere, myAwardItem.GoodsDataList[i].Starttime, myAwardItem.GoodsDataList[i].Endtime, myAwardItem.GoodsDataList[i].AddPropIndex, myAwardItem.GoodsDataList[i].BornIndex, myAwardItem.GoodsDataList[i].Lucky, myAwardItem.GoodsDataList[i].Strong, myAwardItem.GoodsDataList[i].ExcellenceInfo, myAwardItem.GoodsDataList[i].AppendPropLev, myAwardItem.GoodsDataList[i].ChangeLifeLevForEquip, null, null);
						}
					}
				}
				if (myAwardItem.AwardYuanBao > 0)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myAwardItem.AwardYuanBao, goodsFromWhere, ActivityTypes.None, "");
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(386, new object[0]), new object[]
					{
						myAwardItem.AwardYuanBao
					}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
					GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, myAwardItem.AwardYuanBao, goodsFromWhere), null, client.ServerId);
				}
				result = true;
			}
			return result;
		}

		
		private static WLoginItem GetWLoginItem(int whichOne)
		{
			WLoginItem wLoginItem = null;
			lock (HuodongCachingMgr._WLoginDict)
			{
				if (HuodongCachingMgr._WLoginDict.TryGetValue(whichOne, out wLoginItem))
				{
					return wLoginItem;
				}
			}
			SystemXmlItem systemWeekLoginItem = null;
			WLoginItem result;
			if (!GameManager.systemWeekLoginGiftMgr.SystemXmlItemDict.TryGetValue(whichOne, out systemWeekLoginItem))
			{
				LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位周连续登录配置项失败, WhichOne={0}", whichOne), null, true);
				result = null;
			}
			else
			{
				int timeOl = systemWeekLoginItem.GetIntValue("TimeOl", -1);
				wLoginItem = new WLoginItem
				{
					TimeOl = timeOl,
					GoodsDataList = null
				};
				lock (HuodongCachingMgr._WLoginDict)
				{
					HuodongCachingMgr._WLoginDict[whichOne] = wLoginItem;
				}
				string goodsIDs = systemWeekLoginItem.GetStringValue("GoodsIDs");
				if (string.IsNullOrEmpty(goodsIDs))
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位周连续登录配置项中的物品奖励失败, WhichOne={0}", whichOne), null, true);
					result = wLoginItem;
				}
				else
				{
					string[] fields = goodsIDs.Split(new char[]
					{
						'|'
					});
					if (fields.Length <= 0)
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位周连续登录配置项中的物品奖励失败, WhichOne={0}", whichOne), null, true);
						result = wLoginItem;
					}
					else
					{
						List<GoodsData> goodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "周连续登录配置");
						wLoginItem.GoodsDataList = goodsDataList;
						result = wLoginItem;
					}
				}
			}
			return result;
		}

		
		public static int ResetWLoginItem()
		{
			int ret = GameManager.systemWeekLoginGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._WLoginDict)
			{
				HuodongCachingMgr._WLoginDict.Clear();
			}
			return ret;
		}

		
		public static int ProcessGetWLoginGift(GameClient client, int whichOne)
		{
			WLoginItem wLoginItem = HuodongCachingMgr.GetWLoginItem(whichOne);
			int result;
			if (null == wLoginItem)
			{
				result = -1;
			}
			else if (wLoginItem.GoodsDataList == null || wLoginItem.GoodsDataList.Count <= 0)
			{
				result = -5;
			}
			else if (client.ClientData.MyHuodongData.LoginNum < wLoginItem.TimeOl)
			{
				result = -10;
			}
			else
			{
				int bitVal = HuodongCachingMgr.GetBitValue(whichOne);
				if ((client.ClientData.MyHuodongData.LoginGiftState & bitVal) == bitVal)
				{
					result = -100;
				}
				else if (!Global.CanAddGoodsDataList(client, wLoginItem.GoodsDataList))
				{
					result = -200;
				}
				else
				{
					for (int i = 0; i < wLoginItem.GoodsDataList.Count; i++)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, wLoginItem.GoodsDataList[i].GoodsID, wLoginItem.GoodsDataList[i].GCount, wLoginItem.GoodsDataList[i].Quality, "", wLoginItem.GoodsDataList[i].Forge_level, wLoginItem.GoodsDataList[i].Binding, 0, "", true, 1, "周连续登录奖励", "1900-01-01 12:00:00", wLoginItem.GoodsDataList[i].AddPropIndex, wLoginItem.GoodsDataList[i].BornIndex, wLoginItem.GoodsDataList[i].Lucky, wLoginItem.GoodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
					}
					client.ClientData.MyHuodongData.LoginGiftState = (client.ClientData.MyHuodongData.LoginGiftState | bitVal);
					Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyHuodongData(client);
					result = 0;
				}
			}
			return result;
		}

		
		private static MOnlineTimeItem GetMOnlineTimeItem(int whichOne)
		{
			MOnlineTimeItem mOnlineTimeItem = null;
			lock (HuodongCachingMgr._MonthTimeDict)
			{
				if (HuodongCachingMgr._MonthTimeDict.TryGetValue(whichOne, out mOnlineTimeItem))
				{
					return mOnlineTimeItem;
				}
			}
			SystemXmlItem systemMOnlineTimeItem = null;
			MOnlineTimeItem result;
			if (!GameManager.systemMOnlineTimeGiftMgr.SystemXmlItemDict.TryGetValue(whichOne, out systemMOnlineTimeItem))
			{
				LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位月在线时长配置项失败, WhichOne={0}", whichOne), null, true);
				result = null;
			}
			else
			{
				int timeOl = Global.GMax(systemMOnlineTimeItem.GetIntValue("TimeOl", -1), 0) * 3600;
				int bindYuanBao = Global.GMax(systemMOnlineTimeItem.GetIntValue("BindYuanBao", -1), 0);
				mOnlineTimeItem = new MOnlineTimeItem
				{
					TimeOl = timeOl,
					BindYuanBao = bindYuanBao
				};
				lock (HuodongCachingMgr._MonthTimeDict)
				{
					HuodongCachingMgr._MonthTimeDict[whichOne] = mOnlineTimeItem;
				}
				result = mOnlineTimeItem;
			}
			return result;
		}

		
		public static int ResetMOnlineTimeItem()
		{
			int ret = GameManager.systemMOnlineTimeGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._MonthTimeDict)
			{
				HuodongCachingMgr._MonthTimeDict.Clear();
			}
			return ret;
		}

		
		public static int ProcessGetMOnlineTimeGift(GameClient client, int whichOne)
		{
			MOnlineTimeItem mOnlineTimeItem = HuodongCachingMgr.GetMOnlineTimeItem(whichOne);
			int result;
			if (null == mOnlineTimeItem)
			{
				result = -1;
			}
			else if (client.ClientData.MyHuodongData.CurMTime < mOnlineTimeItem.TimeOl)
			{
				result = -10;
			}
			else
			{
				int bitVal = HuodongCachingMgr.GetBitValue(whichOne);
				if ((client.ClientData.MyHuodongData.OnlineGiftState & bitVal) == bitVal)
				{
					result = -100;
				}
				else
				{
					GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, mOnlineTimeItem.BindYuanBao, "月在线时长礼物");
					client.ClientData.MyHuodongData.OnlineGiftState = (client.ClientData.MyHuodongData.OnlineGiftState | bitVal);
					Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyHuodongData(client);
					result = 0;
				}
			}
			return result;
		}

		
		private static NewStepItem GetNewStepItem(int step)
		{
			NewStepItem newStepItem = null;
			lock (HuodongCachingMgr._NewStepDict)
			{
				if (HuodongCachingMgr._NewStepDict.TryGetValue(step, out newStepItem))
				{
					return newStepItem;
				}
			}
			SystemXmlItem systemNewRoleItem = null;
			NewStepItem result;
			if (!GameManager.systemNewRoleGiftMgr.SystemXmlItemDict.TryGetValue(step, out systemNewRoleItem))
			{
				LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位见面有礼配置项失败, Step={0}", step), null, true);
				result = null;
			}
			else
			{
				int timeSecs = Global.GMax(systemNewRoleItem.GetIntValue("TimeSecs", -1), 0) * 60;
				newStepItem = new NewStepItem
				{
					TimeSecs = timeSecs,
					GoodsDataList = null
				};
				lock (HuodongCachingMgr._NewStepDict)
				{
					HuodongCachingMgr._NewStepDict[step] = newStepItem;
				}
				string goodsIDs = systemNewRoleItem.GetStringValue("GoodsIDs");
				if (string.IsNullOrEmpty(goodsIDs))
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位见面有礼配置项失败, Step={0}", step), null, true);
					result = newStepItem;
				}
				else
				{
					string[] fields = goodsIDs.Split(new char[]
					{
						'|'
					});
					if (fields.Length <= 0)
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位见面有礼配置项失败, Step={0}", step), null, true);
						result = newStepItem;
					}
					else
					{
						List<GoodsData> goodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "见面有礼配置项");
						newStepItem.GoodsDataList = goodsDataList;
						newStepItem.BindMoney = systemNewRoleItem.GetIntValue("BindMoney", -1);
						newStepItem.BindYuanBao = systemNewRoleItem.GetIntValue("BindYuanBao", -1);
						result = newStepItem;
					}
				}
			}
			return result;
		}

		
		public static int ResetNewStepItem()
		{
			int ret = GameManager.systemNewRoleGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._NewStepDict)
			{
				HuodongCachingMgr._NewStepDict.Clear();
			}
			return ret;
		}

		
		public static int ProcessGetNewStepGift(GameClient client, int step)
		{
			NewStepItem newStepItem = HuodongCachingMgr.GetNewStepItem(step + 1);
			int result;
			if (null == newStepItem)
			{
				result = -1;
			}
			else if (newStepItem.GoodsDataList == null || newStepItem.GoodsDataList.Count <= 0)
			{
				result = -5;
			}
			else if (client.ClientData.MyHuodongData.NewStep != step)
			{
				result = -10;
			}
			else
			{
				long nowTicks = TimeUtil.NOW();
				if (nowTicks - client.ClientData.MyHuodongData.StepTime < (long)(newStepItem.TimeSecs * 1000))
				{
					int subSecs = newStepItem.TimeSecs - (int)((nowTicks - client.ClientData.MyHuodongData.StepTime) / 1000L);
					result = -(10000 + subSecs);
				}
				else if (!Global.CanAddGoodsDataList(client, newStepItem.GoodsDataList))
				{
					result = -200;
				}
				else
				{
					for (int i = 0; i < newStepItem.GoodsDataList.Count; i++)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, newStepItem.GoodsDataList[i].GoodsID, newStepItem.GoodsDataList[i].GCount, newStepItem.GoodsDataList[i].Quality, "", newStepItem.GoodsDataList[i].Forge_level, newStepItem.GoodsDataList[i].Binding, 0, "", true, 1, "新手见面奖品", "1900-01-01 12:00:00", newStepItem.GoodsDataList[i].AddPropIndex, newStepItem.GoodsDataList[i].BornIndex, newStepItem.GoodsDataList[i].Lucky, newStepItem.GoodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
					}
					int tongQian = newStepItem.BindMoney;
					if (tongQian > 0)
					{
						GameManager.ClientMgr.NotifyAddJinBiMsg(client, tongQian);
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, tongQian, "新手见面礼物", false);
						GameManager.SystemServerEvents.AddEvent(string.Format("从新手见面奖品领取金币, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.RoleName,
							client.ClientData.Money1,
							tongQian
						}), EventLevels.Record);
					}
					int bindYuanBao = newStepItem.BindYuanBao;
					if (bindYuanBao > 0)
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bindYuanBao, "新手见面礼物");
						GameManager.SystemServerEvents.AddEvent(string.Format("从新手见面奖品领取绑定元宝, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.RoleName,
							client.ClientData.UserMoney,
							bindYuanBao
						}), EventLevels.Record);
					}
					client.ClientData.MyHuodongData.NewStep++;
					client.ClientData.MyHuodongData.StepTime = nowTicks;
					Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyHuodongData(client);
					result = 0;
				}
			}
			return result;
		}

		
		
		public static int CombatGiftMaxVal
		{
			get
			{
				return HuodongCachingMgr._CombatAwardlDict.Count;
			}
		}

		
		private static void InitCombatAwardDict()
		{
			lock (HuodongCachingMgr._CombatAwardlDict)
			{
				if (HuodongCachingMgr._CombatAwardlDict.Count == 0)
				{
					foreach (KeyValuePair<int, SystemXmlItem> kv in GameManager.systemCombatAwardMgr.SystemXmlItemDict)
					{
						SystemXmlItem systemItem = kv.Value;
						CombatAwardItem combatAwardItem = new CombatAwardItem
						{
							ID = systemItem.GetIntValue("ID", -1),
							ComBatValue = systemItem.GetIntValue("ComatEffectiveness", -1)
						};
						string goods1IDs = systemItem.GetStringValue("GoodsOne");
						if (!string.IsNullOrEmpty(goods1IDs))
						{
							string[] fields = goods1IDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length > 0)
							{
								combatAwardItem.GeneralAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, string.Format("战斗力奖励配置项 GoodsOne", new object[0]));
							}
						}
						string goods2IDs = systemItem.GetStringValue("GoodsTwo");
						if (!string.IsNullOrEmpty(goods2IDs))
						{
							string[] fields = goods2IDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length > 0)
							{
								combatAwardItem.OccAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, string.Format("战斗力奖励配置项 GoodsTwo", new object[0]));
							}
						}
						string timeGoods = systemItem.GetStringValue("GoodsThr");
						string timeList = systemItem.GetStringValue("EffectiveTime");
						combatAwardItem.EffectTimeAwardItem.Init(timeGoods, timeList, string.Format("战斗力奖励配置项 GoodsThr 和 EffectiveTime", new object[0]));
						HuodongCachingMgr._CombatAwardlDict[combatAwardItem.ID] = combatAwardItem;
					}
				}
			}
		}

		
		public static int GetNextCombatGiftNeedVal(GameClient client)
		{
			HuodongCachingMgr.InitCombatAwardDict();
			long combatFlag = Global.GetRoleParamsInt64FromDB(client, "10154");
			for (int i = 0; i < HuodongCachingMgr.CombatGiftMaxVal; i++)
			{
				if (Global.GetLongSomeBit(combatFlag, i * 2) == 0L)
				{
					CombatAwardItem combatAwardItem = HuodongCachingMgr.GetCombatAwardItem(client, i + 1);
					if (combatAwardItem != null)
					{
						return combatAwardItem.ComBatValue;
					}
				}
			}
			return -1;
		}

		
		public static int GiveCombatGift(GameClient client, CombatAwardItem combatAwardItem)
		{
			int result;
			if (combatAwardItem == null)
			{
				result = -101;
			}
			else
			{
				long combatFlag = Global.GetRoleParamsInt64FromDB(client, "10154");
				if (Global.GetLongSomeBit(combatFlag, (combatAwardItem.ID - 1) * 2) == 0L)
				{
					result = -101;
				}
				else if (1L == Global.GetLongSomeBit(combatFlag, (combatAwardItem.ID - 1) * 2 + 1))
				{
					result = -103;
				}
				else
				{
					int totalCnt = combatAwardItem.TotalAwardCnt(client);
					if (totalCnt > 0 && Global.CanAddGoodsNum(client, totalCnt))
					{
						if (!HuodongCachingMgr.GiveAward(client, combatAwardItem.GeneralAwardItem, "战力礼包") || !HuodongCachingMgr.GiveAward(client, combatAwardItem.OccAwardItem, "战力礼包") || !HuodongCachingMgr.GiveEffectiveTimeAward(client, combatAwardItem.EffectTimeAwardItem.ToAwardItem(), "战力礼包"))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("发送战力礼包奖励的时候，发送失败，但是已经设置为领取成功, roleid={0}, rolename={1}, awardid={3}", client.ClientData.RoleID, client.ClientData.RoleName, combatAwardItem.ID), null, true);
						}
						GameManager.ClientMgr.NotifyGetAwardMsg(client, RoleAwardMsg.CombatGift, "");
						combatFlag = Global.SetLongSomeBit((combatAwardItem.ID - 1) * 2 + 1, combatFlag, true);
						Global.SaveRoleParamsInt64ValueToDB(client, "10154", combatFlag, true);
						result = 1;
					}
					else
					{
						result = -20;
					}
				}
			}
			return result;
		}

		
		public static TCPProcessCmdResults ProcessQueryCombatGiftFlagList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 1)
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
				long combatFlalg = Global.GetRoleParamsInt64FromDB(client, "10154");
				int maxCount = HuodongCachingMgr.CombatGiftMaxVal;
				string strCmd = null;
				for (int i = 0; i < maxCount; i++)
				{
					int sendVal;
					if (Global.GetLongSomeBit(combatFlalg, i * 2) == 0L)
					{
						sendVal = 0;
					}
					else if (Global.GetLongSomeBit(combatFlalg, i * 2 + 1) == 0L)
					{
						sendVal = 1;
					}
					else
					{
						sendVal = 2;
					}
					strCmd = strCmd + sendVal + "_";
				}
				if (strCmd != null)
				{
					strCmd = strCmd.Substring(0, strCmd.Length - 1);
				}
				else
				{
					strCmd = "";
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				LogManager.WriteException(e.ToString());
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public static TCPProcessCmdResults ProcessGetCombatGiftAward(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int id = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int ret = -101;
				CombatAwardItem combatAwardItem = HuodongCachingMgr.GetCombatAwardItem(client, id);
				if (null != combatAwardItem)
				{
					ret = HuodongCachingMgr.GiveCombatGift(client, combatAwardItem);
					client._IconStateMgr.CheckCombatGift(client);
				}
				cmdData = string.Format("{0}:{1}", ret, id);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				LogManager.WriteException(e.ToString());
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public static CombatAwardItem GetCombatAwardItem(GameClient client, int awardIndex)
		{
			HuodongCachingMgr.InitCombatAwardDict();
			CombatAwardItem combatAwardItem = null;
			lock (HuodongCachingMgr._CombatAwardlDict)
			{
				HuodongCachingMgr._CombatAwardlDict.TryGetValue(awardIndex, out combatAwardItem);
			}
			return combatAwardItem;
		}

		
		public static int ResetCombatAwardItem()
		{
			int ret = GameManager.systemCombatAwardMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._CombatAwardlDict)
			{
				HuodongCachingMgr._CombatAwardlDict.Clear();
			}
			HuodongCachingMgr.InitCombatAwardDict();
			return ret;
		}

		
		public static int ProcessCombatGift(GameClient client, bool give = false)
		{
			int result;
			if (client.ClientData.NextCombatForceGiftVal <= 0)
			{
				result = -1;
			}
			else if (client.ClientData.CombatForce < client.ClientData.NextCombatForceGiftVal)
			{
				result = -1;
			}
			else
			{
				int nextCombatAwardIndex = 0;
				lock (HuodongCachingMgr._CombatAwardlDict)
				{
					IEnumerable<KeyValuePair<int, CombatAwardItem>> query = from items in HuodongCachingMgr._CombatAwardlDict
					where items.Value.ComBatValue <= client.ClientData.CombatForce
					select items;
					if (query.Any<KeyValuePair<int, CombatAwardItem>>())
					{
						nextCombatAwardIndex = query.Max((KeyValuePair<int, CombatAwardItem> _b) => _b.Value.ID);
					}
				}
				long combatFlag = Global.GetRoleParamsInt64FromDB(client, "10154");
				for (int i = 0; i < nextCombatAwardIndex; i++)
				{
					if (Global.GetLongSomeBit(combatFlag, i * 2) != 1L)
					{
						combatFlag = Global.SetLongSomeBit(i * 2, combatFlag, true);
					}
				}
				Global.SaveRoleParamsInt64ValueToDB(client, "10154", combatFlag, true);
				client._IconStateMgr.CheckCombatGift(client);
				client.ClientData.NextCombatForceGiftVal = HuodongCachingMgr.GetNextCombatGiftNeedVal(client);
				result = 0;
			}
			return result;
		}

		
		private static void InitUpLevelDict()
		{
			lock (HuodongCachingMgr._UpLevelDict)
			{
				if (HuodongCachingMgr._UpLevelDict.Count == 0)
				{
					foreach (KeyValuePair<int, SystemXmlItem> kv in GameManager.systemUpLevelGiftMgr.SystemXmlItemDict)
					{
						SystemXmlItem systemItem = kv.Value;
						UpLevelItem newStepItem = new UpLevelItem
						{
							ID = systemItem.GetIntValue("ID", -1),
							ToLevel = Global.GetUnionLevel(systemItem.GetIntValue("ToZhuanSheng", -1), systemItem.GetIntValue("ToLevel", -1), false),
							GoodsDataList = null,
							BindMoney = systemItem.GetIntValue("BindMoney", -1),
							MoJing = systemItem.GetIntValue("MoJing", -1),
							Occupation = systemItem.GetIntValue("Occupation", -1)
						};
						Dictionary<int, UpLevelItem> dict;
						if (!HuodongCachingMgr._UpLevelDict.TryGetValue(newStepItem.Occupation, out dict))
						{
							dict = new Dictionary<int, UpLevelItem>();
							HuodongCachingMgr._UpLevelDict.Add(newStepItem.Occupation, dict);
						}
						dict.Add(newStepItem.ToLevel, newStepItem);
						string goodsIDs = systemItem.GetStringValue("GoodsIDs");
						if (!string.IsNullOrEmpty(goodsIDs))
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length > 0)
							{
								List<GoodsData> goodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "升级有礼配置项");
								newStepItem.GoodsDataList = goodsDataList;
							}
						}
					}
				}
			}
		}

		
		private static UpLevelItem GetUpLevelItem(int occu, int unionlevel)
		{
			HuodongCachingMgr.InitUpLevelDict();
			UpLevelItem result;
			lock (HuodongCachingMgr._UpLevelDict)
			{
				UpLevelItem newStepItem = null;
				Dictionary<int, UpLevelItem> dict;
				if (HuodongCachingMgr._UpLevelDict.TryGetValue(occu, out dict))
				{
					if (dict.TryGetValue(unionlevel, out newStepItem))
					{
						return newStepItem;
					}
				}
				result = null;
			}
			return result;
		}

		
		private static UpLevelItem GetUpLevelItemByID(int occu, int id)
		{
			HuodongCachingMgr.InitUpLevelDict();
			lock (HuodongCachingMgr._UpLevelDict)
			{
				Dictionary<int, UpLevelItem> dict;
				if (HuodongCachingMgr._UpLevelDict.TryGetValue(occu, out dict))
				{
					foreach (KeyValuePair<int, UpLevelItem> vk in dict)
					{
						if (vk.Value.ID == id)
						{
							return vk.Value;
						}
					}
				}
			}
			return null;
		}

		
		public static int ResetUpLevelItem()
		{
			int ret = GameManager.systemUpLevelGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._UpLevelDict)
			{
				HuodongCachingMgr._UpLevelDict.Clear();
			}
			return ret;
		}

		
		public static int GiveUpLevelGift(GameClient client, UpLevelItem newStepItem)
		{
			int unionLevel = Global.GetUnionLevel(client, false);
			int result;
			if (newStepItem.ToLevel > unionLevel)
			{
				result = -101;
			}
			else
			{
				List<int> flagList = Global.GetRoleParamsIntListFromDB(client, "UpLevelGiftFlags");
				for (int occLoop = 0; occLoop < 6; occLoop++)
				{
					UpLevelItem stepItem = HuodongCachingMgr.GetUpLevelItem(occLoop, newStepItem.ToLevel);
					if (stepItem != null && 1 == Global.GetBitValue(flagList, stepItem.ID * 2 + 1))
					{
						return -103;
					}
				}
				if (newStepItem.GoodsDataList != null && newStepItem.GoodsDataList.Count > 0)
				{
					if (!Global.CanAddGoodsDataList(client, newStepItem.GoodsDataList))
					{
						return -20;
					}
					for (int i = 0; i < newStepItem.GoodsDataList.Count; i++)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, newStepItem.GoodsDataList[i].GoodsID, newStepItem.GoodsDataList[i].GCount, newStepItem.GoodsDataList[i].Quality, "", newStepItem.GoodsDataList[i].Forge_level, newStepItem.GoodsDataList[i].Binding, 0, "", true, 1, "升级有礼奖品", "1900-01-01 12:00:00", newStepItem.GoodsDataList[i].AddPropIndex, newStepItem.GoodsDataList[i].BornIndex, newStepItem.GoodsDataList[i].Lucky, newStepItem.GoodsDataList[i].Strong, newStepItem.GoodsDataList[i].ExcellenceInfo, newStepItem.GoodsDataList[i].AppendPropLev, 0, null, null, 0, true);
					}
					client.ClientData.AddAwardRecord(RoleAwardMsg.DengJiLiBao, newStepItem.GoodsDataList, false);
				}
				for (int occLoop = 0; occLoop < 6; occLoop++)
				{
					UpLevelItem stepItem = HuodongCachingMgr.GetUpLevelItem(occLoop, newStepItem.ToLevel);
					if (null != stepItem)
					{
						Global.SetBitValue(ref flagList, stepItem.ID * 2 + 1, 1);
					}
				}
				Global.SaveRoleParamsIntListToDB(client, flagList, "UpLevelGiftFlags", true);
				int tongQian = newStepItem.BindMoney;
				if (tongQian > 0)
				{
					GameManager.ClientMgr.NotifyAddJinBiMsg(client, tongQian);
					GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, tongQian, "升级有礼", false);
					GameManager.SystemServerEvents.AddEvent(string.Format("从升级有礼领取金币, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						client.ClientData.Money1,
						tongQian
					}), EventLevels.Record);
					client.ClientData.AddAwardRecord(RoleAwardMsg.DengJiLiBao, MoneyTypes.TongQian, tongQian);
				}
				int bindYuanBao = newStepItem.BindYuanBao;
				if (bindYuanBao > 0)
				{
					GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bindYuanBao, "升级有礼");
					GameManager.SystemServerEvents.AddEvent(string.Format("从升级有礼领取绑定元宝, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						client.ClientData.UserMoney,
						bindYuanBao
					}), EventLevels.Record);
					client.ClientData.AddAwardRecord(RoleAwardMsg.DengJiLiBao, MoneyTypes.BindYuanBao, bindYuanBao);
				}
				int awardMoJing = newStepItem.MoJing;
				if (awardMoJing > 0)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, awardMoJing, "升级有礼", false, true, false);
					GameManager.SystemServerEvents.AddEvent(string.Format("从升级有礼领取魔晶, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						GameManager.ClientMgr.GetTianDiJingYuanValue(client),
						awardMoJing
					}), EventLevels.Record);
					client.ClientData.AddAwardRecord(RoleAwardMsg.DengJiLiBao, MoneyTypes.JingYuanZhi, awardMoJing);
				}
				GameManager.ClientMgr.NotifyGetAwardMsg(client, RoleAwardMsg.DengJiLiBao, "");
				GameManager.ClientMgr.NotifyGetLevelUpGiftData(client, unionLevel);
				result = 1;
			}
			return result;
		}

		
		public static int ProcessGetUpLevelGift(GameClient client, bool give = false)
		{
			int result;
			if (client.ClientData.MapCode == 6090)
			{
				result = -1;
			}
			else
			{
				int unionLevel = Global.GetUnionLevel(client, false);
				UpLevelItem newStepItem = HuodongCachingMgr.GetUpLevelItem(Global.CalcOriginalOccupationID(client), unionLevel);
				if (null == newStepItem)
				{
					result = -1;
				}
				else if (newStepItem.Occupation != Global.CalcOriginalOccupationID(client))
				{
					result = -1;
				}
				else
				{
					List<int> flagList = Global.GetRoleParamsIntListFromDB(client, "UpLevelGiftFlags");
					if (Global.GetBitValue(flagList, newStepItem.ID * 2) == 0)
					{
						Global.SetBitValue(ref flagList, newStepItem.ID * 2, 1);
						Global.SaveRoleParamsIntListToDB(client, flagList, "UpLevelGiftFlags", true);
					}
					if (give && 0 == Global.GetBitValue(flagList, newStepItem.ID * 2 + 1))
					{
						result = HuodongCachingMgr.GiveUpLevelGift(client, newStepItem);
					}
					else
					{
						client._IconStateMgr.CheckFuUpLevelGift(client);
						client._IconStateMgr.CheckSpecialActivity(client);
						client._IconStateMgr.CheckEverydayActivity(client);
						result = 0;
					}
				}
			}
			return result;
		}

		
		public static TCPProcessCmdResults ProcessQueryUpLevelGiftFlagList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 1)
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
				List<int> flagList = Global.GetRoleParamsIntListFromDB(client, "UpLevelGiftFlags");
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<int>>(flagList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				LogManager.WriteException(e.ToString());
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public static TCPProcessCmdResults ProcessGetUpLevelGiftAward(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				int id = Convert.ToInt32(fields[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int ret = -101;
				UpLevelItem upLevelItem = HuodongCachingMgr.GetUpLevelItemByID(Global.CalcOriginalOccupationID(client), id);
				if (null != upLevelItem)
				{
					ret = HuodongCachingMgr.GiveUpLevelGift(client, upLevelItem);
					client._IconStateMgr.CheckFuUpLevelGift(client);
				}
				cmdData = string.Format("{0}:{1}", ret, id);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				LogManager.WriteException(e.ToString());
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		private static BigAwardItem GetBigAwardItem()
		{
			lock (HuodongCachingMgr._BigAwardItemMutex)
			{
				if (HuodongCachingMgr._BigAwardItem != null)
				{
					return HuodongCachingMgr._BigAwardItem;
				}
			}
			try
			{
				string fileName = "Config/Gifts/BigGift.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				BigAwardItem bigAwardItem = new BigAwardItem();
				XElement args = xml.Element("GiftTime");
				if (null != args)
				{
					string fromDate = Global.GetSafeAttributeStr(args, "FromDate");
					string toDate = Global.GetSafeAttributeStr(args, "ToDate");
					if (fromDate.Trim().CompareTo("-1") == 0 && 0 == toDate.Trim().CompareTo("-1"))
					{
						fromDate = "2012-06-06 16:16:16";
						toDate = "2032-06-06 16:16:16";
					}
					bigAwardItem.StartTicks = HuodongCachingMgr.GetHuoDongDateTimeForCommonTimeString(fromDate);
					bigAwardItem.EndTicks = HuodongCachingMgr.GetHuoDongDateTimeForCommonTimeString(toDate);
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							int id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							bigAwardItem.NeedJiFenDict[id] = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "NeedJiFen"));
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大奖活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取大奖活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									bigAwardItem.GoodsDataListDict[id] = HuodongCachingMgr.ParseGoodsDataList(fields, "大奖活动配置");
								}
							}
						}
					}
				}
				lock (HuodongCachingMgr._BigAwardItemMutex)
				{
					HuodongCachingMgr._BigAwardItem = bigAwardItem;
				}
				return bigAwardItem;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/BigGift.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetBigAwardItem()
		{
			string fileName = "Config/Gifts/BigGift.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._BigAwardItemMutex)
			{
				HuodongCachingMgr._BigAwardItem = null;
			}
			return 0;
		}

		
		public static int ProcessGetBigAwardGift(GameClient client, int bigAwardID, int whichOne)
		{
			BigAwardItem bigAwardItem = HuodongCachingMgr.GetBigAwardItem();
			int result;
			if (null == bigAwardItem)
			{
				result = -1;
			}
			else if (bigAwardID != GameManager.GameConfigMgr.GetGameConfigItemInt("big_award_id", 0) || GameManager.GameConfigMgr.GetGameConfigItemInt("big_award_id", 0) <= 0)
			{
				result = -5;
			}
			else
			{
				long ticks = TimeUtil.NOW();
				if (ticks < bigAwardItem.StartTicks || ticks >= bigAwardItem.EndTicks)
				{
					result = -10;
				}
				else
				{
					int subGiftJiFen = 0;
					if (!bigAwardItem.NeedJiFenDict.TryGetValue(whichOne, out subGiftJiFen))
					{
						result = -30;
					}
					else
					{
						List<GoodsData> goodsDataList = null;
						if (!bigAwardItem.GoodsDataListDict.TryGetValue(whichOne, out goodsDataList))
						{
							result = -50;
						}
						else if (!Global.CanAddGoodsDataList(client, goodsDataList))
						{
							result = -300;
						}
						else
						{
							int retJiFen = 0;
							if (subGiftJiFen > 0)
							{
								string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, subGiftJiFen);
								string[] fields = Global.ExecuteDBCmd(10046, strcmd, client.ServerId);
								if (fields == null || fields.Length < 2)
								{
									return -200;
								}
								retJiFen = Convert.ToInt32(fields[1]);
								if (retJiFen < 0)
								{
									return retJiFen * 1000;
								}
							}
							for (int i = 0; i < goodsDataList.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, "", goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "充值有礼", "1900-01-01 12:00:00", goodsDataList[i].AddPropIndex, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, goodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
							}
							Global.BroadcastJiFenDaLiHint(client);
							result = retJiFen;
						}
					}
				}
			}
			return result;
		}

		
		private static SongLiItem GetSongLiItem()
		{
			lock (HuodongCachingMgr._SongLiItemMutex)
			{
				if (HuodongCachingMgr._SongLiItem != null)
				{
					return HuodongCachingMgr._SongLiItem;
				}
			}
			try
			{
				string sectionKey = string.Empty;
				string fileName = Global.GetGiftExchangeFileName(out sectionKey);
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				SongLiItem songLiItem = new SongLiItem();
				xml = xml.Elements().First((XElement _xml) => _xml.Attribute("TypeID").Value.ToString().ToLower() == sectionKey);
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					string fromDate = Global.GetSafeAttributeStr(args, "FromDate");
					string toDate = Global.GetSafeAttributeStr(args, "ToDate");
					if (fromDate.Trim().CompareTo("-1") == 0 && 0 == toDate.Trim().CompareTo("-1"))
					{
						fromDate = "2012-06-06 16:16:16";
						toDate = "2032-06-06 16:16:16";
					}
					songLiItem.StartTicks = HuodongCachingMgr.GetHuoDongDateTimeForCommonTimeString(fromDate);
					songLiItem.EndTicks = HuodongCachingMgr.GetHuoDongDateTimeForCommonTimeString(toDate);
					songLiItem.IsNeedCode = (int)Global.GetSafeAttributeLong(args, "IsNeedCode");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> arglist = args.Elements();
					if (null != arglist)
					{
						for (int i = 0; i < arglist.Count<XElement>(); i++)
						{
							XElement xmlItem = arglist.ElementAt(i);
							if (null != xmlItem)
							{
								int pingTaiID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
								string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
								if (string.IsNullOrEmpty(goodsIDs))
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取送礼活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									string[] fields = goodsIDs.Split(new char[]
									{
										'|'
									});
									if (fields.Length <= 0)
									{
										LogManager.WriteLog(LogTypes.Warning, string.Format("读取送礼活动配置文件中的物品配置项失败", new object[0]), null, true);
									}
									else
									{
										List<GoodsData> goodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "送礼活动配置");
										songLiItem.SongGoodsDataDict[pingTaiID] = goodsDataList;
									}
								}
							}
						}
					}
				}
				lock (HuodongCachingMgr._SongLiItemMutex)
				{
					HuodongCachingMgr._SongLiItem = songLiItem;
				}
				return songLiItem;
			}
			catch (Exception ex)
			{
				LogManager.WriteException("处理送礼活动配置时发生异常" + ex.ToString());
			}
			return null;
		}

		
		public static int ResetSongLiItem()
		{
			string fileName = Global.GetGiftExchangeFileName();
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._SongLiItemMutex)
			{
				HuodongCachingMgr._SongLiItem = null;
			}
			return 0;
		}

		
		public static int ProcessGetSongLiGift(GameClient client, int songLiID, string liPinMa)
		{
			string[] args = liPinMa.Split(new char[]
			{
				'$'
			});
			string ptid = "1";
			string channel = "APPS";
			string appid = "1";
			string codeno = args[0];
			if (args.Length >= 4)
			{
				codeno = args[0];
				ptid = args[1];
				channel = args[2];
				appid = args[3];
			}
			int zoneid = client.ClientData.ZoneID;
			string userId = client.strUserID;
			int roleId = client.ClientData.RoleID;
			string giftId = "";
			int result2;
			if (codeno.Length < 20)
			{
				if (GameManager.PlatConfigMgr.GetGameConfigItemStr("lipinma_v2", "0") == "1")
				{
					if (TimeUtil.NOW() * 10000L - client.ClientData.GetLiPinMaTicks < 30000000L)
					{
						GameManager.ClientMgr.NotifyHintMsg(client, GLang.UseGiftCodeMsg(-11000));
						result2 = 0;
					}
					else
					{
						client.ClientData.GetLiPinMaTicks = TimeUtil.NOW() * 10000L;
						int result = HuanYingSiYuanClient.getInstance().UseGiftCode(ptid, userId, roleId.ToString(), channel, codeno, appid, zoneid, ref giftId);
						GameManager.ClientMgr.NotifyHintMsg(client, GLang.UseGiftCodeMsg(result));
						if (result < 0)
						{
							result2 = 0;
						}
						else
						{
							GiftCodeNewManager.getInstance().ProcessGiftCodeCmd(client, userId, roleId, giftId, codeno);
							result2 = 1;
						}
					}
				}
				else
				{
					result2 = -1020;
				}
			}
			else if (GameManager.PlatConfigMgr.GetGameConfigItemStr("lipinma_v1", "0").StartsWith("-"))
			{
				result2 = -1020;
			}
			else
			{
				SongLiItem songLiItem = HuodongCachingMgr.GetSongLiItem();
				if (null == songLiItem)
				{
					result2 = -1;
				}
				else if (songLiID != GameManager.GameConfigMgr.GetGameConfigItemInt("songli_id", 0) || GameManager.GameConfigMgr.GetGameConfigItemInt("songli_id", 0) <= 0)
				{
					result2 = -5;
				}
				else
				{
					long ticks = TimeUtil.NOW();
					if (ticks < songLiItem.StartTicks || ticks >= songLiItem.EndTicks)
					{
						result2 = -10;
					}
					else if (null == songLiItem.SongGoodsDataDict)
					{
						result2 = -50;
					}
					else
					{
						string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, songLiID, liPinMa);
						string[] fields = Global.ExecuteDBCmd(10061, strcmd, client.ServerId);
						if (fields == null || fields.Length < 2)
						{
							result2 = -200;
						}
						else
						{
							int retCode = Convert.ToInt32(fields[1]);
							if (retCode < 0)
							{
								result2 = retCode;
							}
							else
							{
								int goodsDataListID = retCode;
								List<GoodsData> goodsDataList = null;
								if (!songLiItem.SongGoodsDataDict.TryGetValue(goodsDataListID, out goodsDataList) || null == goodsDataList)
								{
									result2 = -50;
								}
								else if (!Global.CanAddGoodsDataList(client, goodsDataList))
								{
									result2 = -400;
								}
								else if (string.IsNullOrEmpty(liPinMa))
								{
									result2 = -100;
								}
								else
								{
									strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, songLiID, liPinMa);
									fields = Global.ExecuteDBCmd(10047, strcmd, client.ServerId);
									if (fields == null || fields.Length < 2)
									{
										result2 = -200;
									}
									else
									{
										retCode = Convert.ToInt32(fields[1]);
										if (retCode < 0)
										{
											result2 = retCode;
										}
										else
										{
											client.ClientData.MyHuodongData.SongLiID = songLiID;
											for (int i = 0; i < goodsDataList.Count; i++)
											{
												Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataList[i].GoodsID, goodsDataList[i].GCount, goodsDataList[i].Quality, "", goodsDataList[i].Forge_level, goodsDataList[i].Binding, 0, "", true, 1, "系统送礼", "1900-01-01 12:00:00", goodsDataList[i].AddPropIndex, goodsDataList[i].BornIndex, goodsDataList[i].Lucky, goodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
											}
											result2 = 0;
										}
									}
								}
							}
						}
					}
				}
			}
			return result2;
		}

		
		private static string praseKalendsGiftCode(string liPinMa, int used = 0)
		{
			string result2;
			try
			{
				string url = GameManager.GameConfigMgr.GetGameConfigItemStr("kl_giftcode_u_r_l", "");
				if (string.IsNullOrEmpty(url))
				{
					result2 = null;
				}
				else
				{
					url = "http://" + url;
					string strMD5Key = GameManager.GameConfigMgr.GetGameConfigItemStr("kl_giftcode_md5key", "tmsk_mu_06");
					if (string.IsNullOrEmpty(strMD5Key))
					{
						result2 = null;
					}
					else
					{
						int timeout = GameManager.GameConfigMgr.GetGameConfigItemInt("kl_giftcode_timeout", 200);
						long lTime = (long)DataHelper.UnixSecondsNow();
						string strMD5 = MD5Helper.get_md5_string(string.Concat(new object[]
						{
							liPinMa,
							lTime,
							used,
							strMD5Key
						}));
						Dictionary<string, string> resultDict = new Dictionary<string, string>();
						resultDict["giftid"] = liPinMa;
						resultDict["time"] = lTime.ToString();
						resultDict["used"] = used.ToString();
						resultDict["sign"] = strMD5;
						string strBody = Global.GetJson(resultDict);
						string strResult = Global.doPost(url, strBody, timeout);
						if (string.IsNullOrEmpty(strResult))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("kl_giftcode text null ", new object[0]), null, true);
							result2 = null;
						}
						else
						{
							int result = 0;
							if (int.TryParse(strResult, out result))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("kl_giftcode return error : {0}", strResult), null, true);
								result2 = null;
							}
							else
							{
								Hashtable rspTable = (Hashtable)MUJson.jsonDecode(strResult);
								if (null == rspTable)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("kl_giftcode rspTable null : {0}", strResult), null, true);
									result2 = null;
								}
								else
								{
									string strGiftID = rspTable["giftid"].ToString();
									if (string.IsNullOrEmpty(strGiftID))
									{
										result2 = null;
									}
									else
									{
										string strTime = rspTable["time"].ToString();
										if (string.IsNullOrEmpty(strTime))
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("kl_giftcode time null : {0}", strResult), null, true);
											result2 = null;
										}
										else
										{
											long.TryParse(strTime, out lTime);
											string strSign = rspTable["sign"].ToString();
											if (string.IsNullOrEmpty(strSign))
											{
												LogManager.WriteLog(LogTypes.Error, string.Format("kl_giftcode sign null : {0}", strResult), null, true);
												result2 = null;
											}
											else
											{
												strSign = strSign.ToLower();
												string strMD5Param = strGiftID + lTime + strMD5Key;
												string sign = MD5Helper.get_md5_string(strMD5Param);
												sign = sign.ToLower();
												if (sign != strSign)
												{
													LogManager.WriteLog(LogTypes.Error, string.Format("kl_giftcode MD5 error : {0}", strResult), null, true);
													result2 = null;
												}
												else
												{
													if ("-1" != strGiftID)
													{
														if (strGiftID.Length < 5)
														{
															LogManager.WriteLog(LogTypes.Error, string.Format("kl_giftcode GiftCode Length error : {0}", strResult), null, true);
															return null;
														}
													}
													result2 = strGiftID;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "praseKalendsGiftCode", false, false);
				result2 = null;
			}
			return result2;
		}

		
		private static void InitLimitTimeLoginTimes()
		{
			lock (HuodongCachingMgr._LimitTimeLoginDict)
			{
				if (HuodongCachingMgr._LimitTimeLoginStartTime.Year != 1971 || HuodongCachingMgr._LimitTimeLoginEndTime.Year != 1971)
				{
					return;
				}
			}
			try
			{
				string fileName = "Config/Gifts/HuoDongLoginNumGift.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null != xml)
				{
					XElement args = xml.Element("Activities");
					if (null != args)
					{
						string fromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
						string toDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
						lock (HuodongCachingMgr._LimitTimeLoginDict)
						{
							try
							{
								HuodongCachingMgr._LimitTimeLoginStartTime = DateTime.Parse(fromDate);
							}
							catch (Exception)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位限时累计登录配置的开始时间错误, fromDate={0}", fromDate), null, true);
							}
							try
							{
								HuodongCachingMgr._LimitTimeLoginEndTime = DateTime.Parse(toDate);
							}
							catch (Exception)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位限时累计登录配置的结束时间错误, toDate={0}", toDate), null, true);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/HuoDongLoginNumGift.xml解析出现异常", ex, true);
			}
		}

		
		public static bool JugeInLimitTimeLoginPeriod()
		{
			return HuodongCachingMgr.GetLimitTimeLoginHuoDongID() > 0;
		}

		
		public static int GetLimitTimeLoginHuoDongID()
		{
			HuodongCachingMgr.InitLimitTimeLoginTimes();
			DateTime now = TimeUtil.NowDateTime();
			int result;
			lock (HuodongCachingMgr._LimitTimeLoginDict)
			{
				if (HuodongCachingMgr._LimitTimeLoginStartTime.Year == 1971 || HuodongCachingMgr._LimitTimeLoginEndTime.Year == 1971)
				{
					result = -1;
				}
				else if (HuodongCachingMgr._LimitTimeLoginStartTime.Ticks >= HuodongCachingMgr._LimitTimeLoginEndTime.Ticks)
				{
					result = -1;
				}
				else if (now.Ticks >= HuodongCachingMgr._LimitTimeLoginStartTime.Ticks && now.Ticks < HuodongCachingMgr._LimitTimeLoginEndTime.Ticks)
				{
					result = HuodongCachingMgr._LimitTimeLoginStartTime.DayOfYear;
				}
				else
				{
					result = -1;
				}
			}
			return result;
		}

		
		private static LimitTimeLoginItem GetLimitTimeLoginItem(int whichOne)
		{
			LimitTimeLoginItem limitTimeLoginItem = null;
			lock (HuodongCachingMgr._LimitTimeLoginDict)
			{
				if (HuodongCachingMgr._LimitTimeLoginDict.TryGetValue(whichOne, out limitTimeLoginItem))
				{
					return limitTimeLoginItem;
				}
			}
			SystemXmlItem systemLimitTimeLoginItem = null;
			LimitTimeLoginItem result;
			if (!GameManager.SystemDengLuDali.SystemXmlItemDict.TryGetValue(whichOne, out systemLimitTimeLoginItem))
			{
				LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位限时累计登录配置项失败, WhichOne={0}", whichOne), null, true);
				result = null;
			}
			else
			{
				int timeOl = systemLimitTimeLoginItem.GetIntValue("TimeOl", -1);
				limitTimeLoginItem = new LimitTimeLoginItem
				{
					TimeOl = timeOl,
					GoodsDataList = null
				};
				lock (HuodongCachingMgr._LimitTimeLoginDict)
				{
					HuodongCachingMgr._LimitTimeLoginDict[whichOne] = limitTimeLoginItem;
				}
				string goodsIDs = systemLimitTimeLoginItem.GetStringValue("GoodsIDs");
				if (string.IsNullOrEmpty(goodsIDs))
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位限时累计登录配置项中的物品奖励失败, WhichOne={0}", whichOne), null, true);
					result = limitTimeLoginItem;
				}
				else
				{
					string[] fields = goodsIDs.Split(new char[]
					{
						'|'
					});
					if (fields.Length <= 0)
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位限时累计登录配置项中的物品奖励失败, WhichOne={0}", whichOne), null, true);
						result = limitTimeLoginItem;
					}
					else
					{
						List<GoodsData> goodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "限时累计登录配置");
						limitTimeLoginItem.GoodsDataList = goodsDataList;
						result = limitTimeLoginItem;
					}
				}
			}
			return result;
		}

		
		public static int ResetLimitTimeLoginItem()
		{
			int ret = GameManager.SystemDengLuDali.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._LimitTimeLoginDict)
			{
				HuodongCachingMgr._LimitTimeLoginStartTime = new DateTime(1971, 1, 1);
				HuodongCachingMgr._LimitTimeLoginEndTime = new DateTime(1971, 1, 1);
				HuodongCachingMgr._LimitTimeLoginDict.Clear();
			}
			return ret;
		}

		
		public static int ProcessGetLimitTimeLoginGift(GameClient client, int whichOne)
		{
			int result;
			if (!HuodongCachingMgr.JugeInLimitTimeLoginPeriod())
			{
				result = -10000;
			}
			else
			{
				LimitTimeLoginItem limitTimeLoginItem = HuodongCachingMgr.GetLimitTimeLoginItem(whichOne);
				if (null == limitTimeLoginItem)
				{
					result = -1;
				}
				else if (limitTimeLoginItem.GoodsDataList == null || limitTimeLoginItem.GoodsDataList.Count <= 0)
				{
					result = -5;
				}
				else if (client.ClientData.MyHuodongData.LimitTimeLoginNum < limitTimeLoginItem.TimeOl)
				{
					result = -10;
				}
				else
				{
					int bitVal = HuodongCachingMgr.GetBitValue(whichOne);
					if ((client.ClientData.MyHuodongData.LimitTimeGiftState & bitVal) == bitVal)
					{
						result = -100;
					}
					else if (!Global.CanAddGoodsDataList(client, limitTimeLoginItem.GoodsDataList))
					{
						result = -200;
					}
					else
					{
						for (int i = 0; i < limitTimeLoginItem.GoodsDataList.Count; i++)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, limitTimeLoginItem.GoodsDataList[i].GoodsID, limitTimeLoginItem.GoodsDataList[i].GCount, limitTimeLoginItem.GoodsDataList[i].Quality, "", limitTimeLoginItem.GoodsDataList[i].Forge_level, limitTimeLoginItem.GoodsDataList[i].Binding, 0, "", true, 1, "限时累计登录奖励", "1900-01-01 12:00:00", limitTimeLoginItem.GoodsDataList[i].AddPropIndex, limitTimeLoginItem.GoodsDataList[i].BornIndex, limitTimeLoginItem.GoodsDataList[i].Lucky, limitTimeLoginItem.GoodsDataList[i].Strong, 0, 0, 0, null, null, 0, true);
						}
						client.ClientData.MyHuodongData.LimitTimeGiftState = (client.ClientData.MyHuodongData.LimitTimeGiftState | bitVal);
						Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyHuodongData(client);
						result = 0;
					}
				}
			}
			return result;
		}

		
		public static int GetEveryDayOnLineItemCount()
		{
			return GameManager.systemEveryDayOnLineAwardMgr.SystemXmlItemDict.Count;
		}

		
		public static EveryDayOnLineAward GetEveryDayOnLineItem(int step)
		{
			EveryDayOnLineAward EveryDayOnLineAwardItem = null;
			lock (HuodongCachingMgr._EveryDayOnLineAwardDict)
			{
				if (HuodongCachingMgr._EveryDayOnLineAwardDict.TryGetValue(step, out EveryDayOnLineAwardItem))
				{
					return EveryDayOnLineAwardItem;
				}
			}
			SystemXmlItem systemEveryDayOnLineAwardItem = null;
			EveryDayOnLineAward result;
			if (!GameManager.systemEveryDayOnLineAwardMgr.SystemXmlItemDict.TryGetValue(step, out systemEveryDayOnLineAwardItem))
			{
				LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位每日在线奖励配置项失败, Step={0}", step), null, true);
				result = null;
			}
			else
			{
				int timeSecs = Global.GMax(systemEveryDayOnLineAwardItem.GetIntValue("TimeSecs", -1), 0) * 60;
				EveryDayOnLineAwardItem = new EveryDayOnLineAward
				{
					TimeSecs = timeSecs,
					FallPacketID = -1
				};
				lock (HuodongCachingMgr._EveryDayOnLineAwardDict)
				{
					HuodongCachingMgr._EveryDayOnLineAwardDict[step] = EveryDayOnLineAwardItem;
				}
				int FallIDs = systemEveryDayOnLineAwardItem.GetIntValue("FallID", -1);
				if (FallIDs == -1)
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位每日在线奖励配置项失败, Step={0}", step), null, true);
					result = EveryDayOnLineAwardItem;
				}
				else
				{
					EveryDayOnLineAwardItem.FallPacketID = FallIDs;
					result = EveryDayOnLineAwardItem;
				}
			}
			return result;
		}

		
		public static int ResetEveryDayOnLineAwardItem()
		{
			int ret = GameManager.systemEveryDayOnLineAwardMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._EveryDayOnLineAwardDict)
			{
				HuodongCachingMgr._EveryDayOnLineAwardDict.Clear();
			}
			return ret;
		}

		
		public static int ProcessGetEveryDayOnLineAwardGift(GameClient client, List<GoodsData> goodsDataList, int nType = 0)
		{
			int nDate = TimeUtil.NowDateTime().DayOfYear;
			if (client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID != nDate)
			{
				client.ClientData.MyHuodongData.EveryDayOnLineAwardStep = 0;
				client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID = nDate;
			}
			int nTotal = HuodongCachingMgr.GetEveryDayOnLineItemCount();
			int result;
			if (nTotal == client.ClientData.MyHuodongData.EveryDayOnLineAwardStep)
			{
				result = -1;
			}
			else
			{
				bool bIsSuc = false;
				int nRet = 1;
				int nIndex = nTotal - client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;
				int i = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep + 1;
				while (i <= nTotal)
				{
					EveryDayOnLineAward EveryDayOnLineAwardItem = HuodongCachingMgr.GetEveryDayOnLineItem(i);
					if (null == EveryDayOnLineAwardItem)
					{
						return -2;
					}
					if (client.ClientData.DayOnlineSecond < EveryDayOnLineAwardItem.TimeSecs)
					{
						if (!bIsSuc)
						{
							return -3;
						}
						return 1;
					}
					else
					{
						nRet = GoodsBaoXiang.ProcessActivityAward(client, EveryDayOnLineAwardItem.FallPacketID, 1, 1, "每日在线奖励物品", goodsDataList);
						if (nRet != 1)
						{
							return nRet;
						}
						bIsSuc = true;
						client.ClientData.MyHuodongData.EveryDayOnLineAwardStep++;
						i++;
					}
				}
				client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID = nDate;
				Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyHuodongData(client);
				result = nRet;
			}
			return result;
		}

		
		public static int ProcessGetEveryDayOnLineAwardGift2(GameClient client, List<GoodsData> goodsDataList, out int nRet)
		{
			int nDate = TimeUtil.NowDateTime().DayOfYear;
			if (client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID != nDate)
			{
				client.ClientData.MyHuodongData.EveryDayOnLineAwardStep = 0;
				client.ClientData.MyHuodongData.GetEveryDayOnLineAwardDayID = nDate;
			}
			int nSetp = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;
			int nTotal = HuodongCachingMgr.GetEveryDayOnLineItemCount();
			int result;
			if (nTotal == client.ClientData.MyHuodongData.EveryDayOnLineAwardStep)
			{
				nRet = -1;
				result = nSetp;
			}
			else
			{
				int nIndex = nTotal - client.ClientData.MyHuodongData.EveryDayOnLineAwardStep;
				for (int i = client.ClientData.MyHuodongData.EveryDayOnLineAwardStep + 1; i <= nTotal; i++)
				{
					EveryDayOnLineAward EveryDayOnLineAwardItem = HuodongCachingMgr.GetEveryDayOnLineItem(i);
					if (null == EveryDayOnLineAwardItem)
					{
						nRet = -2;
						return nSetp;
					}
					if (client.ClientData.DayOnlineSecond < EveryDayOnLineAwardItem.TimeSecs)
					{
						if (nSetp == client.ClientData.MyHuodongData.EveryDayOnLineAwardStep)
						{
							nRet = -3;
						}
						else
						{
							nRet = 1;
						}
						return nSetp;
					}
					nRet = GoodsBaoXiang.ProcessActivityAward(client, EveryDayOnLineAwardItem.FallPacketID, 1, 1, "每日在线奖励物品", goodsDataList);
					if (nRet != 1)
					{
						return nSetp;
					}
					nSetp++;
				}
				nRet = 1;
				result = nSetp;
			}
			return result;
		}

		
		public static int GetSeriesLoginCount()
		{
			return GameManager.systemSeriesLoginAwardMgr.SystemXmlItemDict.Count;
		}

		
		private static SeriesLoginAward GetSeriesLoginAward(int whichOne)
		{
			SeriesLoginAward SeriesLoginItem = null;
			lock (HuodongCachingMgr._SeriesLoginAward)
			{
				if (HuodongCachingMgr._SeriesLoginAward.TryGetValue(whichOne, out SeriesLoginItem))
				{
					return SeriesLoginItem;
				}
			}
			SystemXmlItem systemSeriesLoginItem = null;
			SeriesLoginAward result;
			if (!GameManager.systemSeriesLoginAwardMgr.SystemXmlItemDict.TryGetValue(whichOne, out systemSeriesLoginItem))
			{
				LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位连续登录奖励配置项失败, WhichOne={0}", whichOne), null, true);
				result = null;
			}
			else
			{
				int LoginTime = systemSeriesLoginItem.GetIntValue("LoginTime", -1);
				SeriesLoginItem = new SeriesLoginAward
				{
					NeedSeriesLoginNum = LoginTime,
					FallPacketID = -1
				};
				lock (HuodongCachingMgr._SeriesLoginAward)
				{
					HuodongCachingMgr._SeriesLoginAward[whichOne] = SeriesLoginItem;
				}
				int FallIDs = systemSeriesLoginItem.GetIntValue("FallID", -1);
				if (FallIDs == -1)
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("根据奖励类型定位连续登陆奖励配置项失败, Step={0}", whichOne), null, true);
					result = SeriesLoginItem;
				}
				else
				{
					SeriesLoginItem.FallPacketID = FallIDs;
					result = SeriesLoginItem;
				}
			}
			return result;
		}

		
		public static int ResetSeriesLoginItem()
		{
			int ret = GameManager.systemSeriesLoginAwardMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._SeriesLoginAward)
			{
				HuodongCachingMgr._SeriesLoginAward.Clear();
			}
			return ret;
		}

		
		public static int ProcessGetSeriesLoginGift(GameClient client, List<GoodsData> goodsDataList, int nIndex = 0)
		{
			int nDay = TimeUtil.NowDateTime().DayOfYear;
			int result;
			if (client.ClientData.MyHuodongData.SeriesLoginAwardDayID == nDay && client.ClientData.MyHuodongData.SeriesLoginGetAwardStep == client.ClientData.SeriesLoginNum)
			{
				result = -2;
			}
			else
			{
				int nRet = -1;
				int nTotal = HuodongCachingMgr.GetSeriesLoginCount();
				for (int i = client.ClientData.MyHuodongData.SeriesLoginGetAwardStep + 1; i <= nTotal; i++)
				{
					SeriesLoginAward SeriesLoginItem = HuodongCachingMgr.GetSeriesLoginAward(i);
					if (null == SeriesLoginItem)
					{
						return -1;
					}
					if (SeriesLoginItem.FallPacketID == -1)
					{
						return -1;
					}
					if (client.ClientData.SeriesLoginNum < SeriesLoginItem.NeedSeriesLoginNum)
					{
						break;
					}
					nRet = GoodsBaoXiang.ProcessActivityAward(client, SeriesLoginItem.FallPacketID, 1, 1, "连续登陆奖励物品", goodsDataList);
					if (nRet != 1)
					{
						break;
					}
					client.ClientData.MyHuodongData.SeriesLoginGetAwardStep++;
				}
				client.ClientData.MyHuodongData.SeriesLoginAwardDayID = nDay;
				Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyHuodongData(client);
				result = nRet;
			}
			return result;
		}

		
		public static int ProcessGetSeriesLoginGift2(GameClient client, List<GoodsData> goodsDataList)
		{
			int nStep = client.ClientData.MyHuodongData.SeriesLoginGetAwardStep;
			int nDay = TimeUtil.NowDateTime().DayOfYear;
			int result;
			if (client.ClientData.MyHuodongData.SeriesLoginAwardDayID == nDay && client.ClientData.MyHuodongData.SeriesLoginGetAwardStep == client.ClientData.SeriesLoginNum)
			{
				result = nStep;
			}
			else
			{
				int nTotal = HuodongCachingMgr.GetSeriesLoginCount();
				for (int i = client.ClientData.MyHuodongData.SeriesLoginGetAwardStep + 1; i <= nTotal; i++)
				{
					SeriesLoginAward SeriesLoginItem = HuodongCachingMgr.GetSeriesLoginAward(i);
					if (null == SeriesLoginItem)
					{
						return nStep;
					}
					if (SeriesLoginItem.FallPacketID == -1)
					{
						return nStep;
					}
					if (client.ClientData.SeriesLoginNum < SeriesLoginItem.NeedSeriesLoginNum)
					{
						return nStep;
					}
					int nRet = GoodsBaoXiang.ProcessActivityAward(client, SeriesLoginItem.FallPacketID, 1, 1, "连续登陆奖励物品", goodsDataList);
					if (nRet != 1)
					{
						return nStep;
					}
					nStep++;
				}
				result = nStep;
			}
			return result;
		}

		
		public static bool LoadActivitiesConfig()
		{
			string strError = "";
			Activity instance = HuodongCachingMgr.GetFirstChongZhiActivity();
			if (instance == null || instance.GetParamsValidateCode() < 0)
			{
				strError = "HuodongCachingMgr.GetFirstChongZhiActivity()配置项出错";
			}
			else
			{
				instance = HuodongCachingMgr.GetInputFanLiActivity();
				if (instance == null || instance.GetParamsValidateCode() < 0)
				{
					strError = "充值返利活动配置项出错";
				}
				else
				{
					instance = HuodongCachingMgr.GetWeekEndInputActivity();
					if (instance == null || instance.GetParamsValidateCode() < 0)
					{
						strError = "周末充值活动配置项出错";
					}
					else
					{
						instance = HuodongCachingMgr.GetInputSongActivity();
						if (instance == null || instance.GetParamsValidateCode() < 0)
						{
							strError = "充值送礼活动配置项出错";
						}
						else
						{
							instance = HuodongCachingMgr.GetInputKingActivity();
							if (instance == null || instance.GetParamsValidateCode() < 0)
							{
								strError = "充值王活动配置项出错";
							}
							else
							{
								instance = HuodongCachingMgr.GetLevelKingActivity();
								if (instance == null || instance.GetParamsValidateCode() < 0)
								{
									strError = "冲级王活动配置项出错";
								}
								else
								{
									instance = HuodongCachingMgr.GetEquipKingActivity();
									if (instance == null || instance.GetParamsValidateCode() < 0)
									{
										strError = "装备王活动配置项出错";
									}
									else
									{
										instance = HuodongCachingMgr.GetHorseKingActivity();
										if (instance == null || instance.GetParamsValidateCode() < 0)
										{
											strError = "坐骑王活动配置项出错";
										}
										else
										{
											instance = HuodongCachingMgr.GetJingMaiKingActivity();
											if (instance == null || instance.GetParamsValidateCode() < 0)
											{
												strError = "经脉王活动配置项出错";
											}
											else
											{
												instance = HuodongCachingMgr.GetSpecialActivity();
												if (instance == null || instance.GetParamsValidateCode() < 0)
												{
													strError = "专享活动配置项出错";
												}
												else
												{
													instance = HuodongCachingMgr.GetEverydayActivity();
													if (instance == null || instance.GetParamsValidateCode() < 0)
													{
														strError = "每日活动配置项出错";
													}
													else
													{
														instance = HuodongCachingMgr.GetSpecPriorityActivity();
														if (instance == null || instance.GetParamsValidateCode() < 0)
														{
															strError = "特权活动配置项出错";
														}
														else
														{
															instance = HuodongCachingMgr.GetOneDollarBuyActivity();
															if (instance == null || instance.GetParamsValidateCode() < 0)
															{
																strError = "1元直购活动配置项出错";
															}
															else
															{
																instance = HuodongCachingMgr.GetJieRiSuperInputActivity();
																if (instance == null || instance.GetParamsValidateCode() < 0)
																{
																	strError = "节日超级充值返利活动配置项出错";
																}
																else
																{
																	instance = HuodongCachingMgr.GetOneDollarChongZhiActivity();
																	if (instance == null || instance.GetParamsValidateCode() < 0)
																	{
																		strError = "1元充值活动配置项出错";
																	}
																	else
																	{
																		instance = HuodongCachingMgr.GetInputFanLiNewActivity();
																		if (instance == null || instance.GetParamsValidateCode() < 0)
																		{
																			strError = "3周年充值返利活动配置项出错";
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bool result;
			if (!string.IsNullOrEmpty(strError))
			{
				LogManager.WriteLog(LogTypes.Fatal, strError, null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		
		public static DanBiChongZhiActivity GetDanBiChongZhiActivity()
		{
			lock (HuodongCachingMgr._DanBiChongZhiMutex)
			{
				if (HuodongCachingMgr._DanBiChongZhiAct != null)
				{
					return HuodongCachingMgr._DanBiChongZhiAct;
				}
			}
			DanBiChongZhiActivity act = new DanBiChongZhiActivity();
			if (act.init())
			{
				lock (HuodongCachingMgr._DanBiChongZhiMutex)
				{
					HuodongCachingMgr._DanBiChongZhiAct = act;
					return HuodongCachingMgr._DanBiChongZhiAct;
				}
			}
			return null;
		}

		
		public static int ResetDanBiChongZhiActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiDanBiChongZhi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._DanBiChongZhiMutex)
			{
				HuodongCachingMgr._DanBiChongZhiAct = null;
			}
			return 0;
		}

		
		public static FirstChongZhiGift GetFirstChongZhiActivity()
		{
			lock (HuodongCachingMgr._FirstChongZhiActivityMutex)
			{
				if (HuodongCachingMgr._FirstChongZhiActivity != null)
				{
					return HuodongCachingMgr._FirstChongZhiActivity;
				}
			}
			try
			{
				string fileName = "Config/Gifts/FirstCharge.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				FirstChongZhiGift activity = new FirstChongZhiGift();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							AwardItem myAwardItem2 = new AwardItem();
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取首充活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取首充活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "首充活动配置");
								}
							}
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取首充活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取首充活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "首充活动配置");
								}
							}
							activity.AwardDict = myAwardItem;
							activity.AwardDict2 = myAwardItem2;
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._FirstChongZhiActivityMutex)
				{
					HuodongCachingMgr._FirstChongZhiActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/FirstCharge.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetFirstChongZhiGift()
		{
			string fileName = "Config/Gifts/FirstCharge.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._FirstChongZhiActivityMutex)
			{
				HuodongCachingMgr._FirstChongZhiActivity = null;
			}
			return 0;
		}

		
		public static InputFanLiActivity GetInputFanLiActivity()
		{
			lock (HuodongCachingMgr._InputFanLiActivityMutex)
			{
				if (HuodongCachingMgr._InputFanLiActivity != null)
				{
					return HuodongCachingMgr._InputFanLiActivity;
				}
			}
			try
			{
				string fileName = "Config/Gifts/FanLi.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				InputFanLiActivity activity = new InputFanLiActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.ToDate = Global.GetHuoDongTimeByKaiFu(7, 23, 59, 59);
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(8, 0, 0, 0);
					activity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(8, 23, 59, 59);
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					XElement xmlItem = args.Element("Award");
					if (null != xmlItem)
					{
						activity.FanLiPersent = Global.GetSafeAttributeDouble(xmlItem, "FanLi");
						if (activity.FanLiPersent < 0.0)
						{
							activity.FanLiPersent = 0.0;
						}
					}
				}
				lock (HuodongCachingMgr._InputFanLiActivityMutex)
				{
					HuodongCachingMgr._InputFanLiActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/FanLi.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetInputFanLiActivity()
		{
			string fileName = "Config/Gifts/FanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._InputFanLiActivityMutex)
			{
				HuodongCachingMgr._InputFanLiActivity = null;
			}
			return 0;
		}

		
		public static InputSongActivity GetInputSongActivity()
		{
			lock (HuodongCachingMgr._InputSongActivityMutex)
			{
				if (HuodongCachingMgr._InputSongActivity != null)
				{
					return HuodongCachingMgr._InputSongActivity;
				}
			}
			try
			{
				string fileName = "Config/Gifts/ChongZhiSong.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				InputSongActivity activity = new InputSongActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
				}
				activity.MyAwardItem = new AwardItem();
				args = xml.Element("GiftList");
				if (null != args)
				{
					XElement xmlItem = args.Element("Award");
					if (null != xmlItem)
					{
						activity.MyAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
						activity.MyAwardItem.AwardYuanBao = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "YuanBao"));
						string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取充值加送活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取充值加送活动配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								activity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "充值加送活动配置");
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._InputSongActivityMutex)
				{
					HuodongCachingMgr._InputSongActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/ChongZhiSong.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetInputSongActivity()
		{
			string fileName = "Config/Gifts/ChongZhiSong.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._InputSongActivityMutex)
			{
				HuodongCachingMgr._InputSongActivity = null;
			}
			return 0;
		}

		
		public static KingActivity GetInputKingActivity()
		{
			lock (HuodongCachingMgr._InputKingActivityMutex)
			{
				if (HuodongCachingMgr._InputKingActivity != null)
				{
					return HuodongCachingMgr._InputKingActivity;
				}
			}
			try
			{
				string fileName = "Config/XinFuGifts/MuChongZhi.xml";
				if (Global.isDoubleXinFu(34))
				{
					fileName = "Config/XinFuGifts/MuDoubleChongZhi.xml";
				}
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				KingActivity activity = new KingActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 0, 0, 0);
					activity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(8, 23, 59, 59);
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							AwardItem myAwardItem2 = new AwardItem();
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取充值王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取充值王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "充值王活动配置");
								}
							}
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取充值王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取充值王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "充值王活动配置");
								}
							}
							string rankings = Global.GetSafeAttributeStr(xmlItem, "ID");
							string[] paiHangs = rankings.Split(new char[]
							{
								'-'
							});
							if (paiHangs.Length > 0)
							{
								int min = Global.SafeConvertToInt32(paiHangs[0]);
								int max = Global.SafeConvertToInt32(paiHangs[paiHangs.Length - 1]);
								for (int paiHang = min; paiHang <= max; paiHang++)
								{
									activity.AwardDict.Add(paiHang, myAwardItem);
									activity.AwardDict2.Add(paiHang, myAwardItem2);
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._InputKingActivityMutex)
				{
					HuodongCachingMgr._InputKingActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/XinFuGifts/MuChongZhi.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetInputKingActivity()
		{
			string fileName = "Config/Gifts/MuChongZhi.xml";
			if (Global.isDoubleXinFu(34))
			{
				fileName = "Config/XinFuGifts/MuDoubleChongZhi.xml";
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._InputKingActivityMutex)
			{
				HuodongCachingMgr._InputKingActivity = null;
			}
			return 0;
		}

		
		public static KingActivity GetLevelKingActivity()
		{
			lock (HuodongCachingMgr._LevelKingActivityMutex)
			{
				if (HuodongCachingMgr._LevelKingActivity != null)
				{
					return HuodongCachingMgr._LevelKingActivity;
				}
			}
			try
			{
				string fileName = "Config/Gifts/LevelKing.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				KingActivity activity = new KingActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.ToDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					activity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(10, 23, 59, 59);
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel"));
							myAwardItem.AwardYuanBao = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "YuanBao"));
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取冲级王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取冲级王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "冲级王活动配置");
								}
							}
							string rankings = Global.GetSafeAttributeStr(xmlItem, "Ranking");
							string[] paiHangs = rankings.Split(new char[]
							{
								'-'
							});
							if (paiHangs.Length > 0)
							{
								int min = Global.SafeConvertToInt32(paiHangs[0]);
								int max = Global.SafeConvertToInt32(paiHangs[paiHangs.Length - 1]);
								for (int paiHang = min; paiHang <= max; paiHang++)
								{
									activity.AwardDict.Add(paiHang, myAwardItem);
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._LevelKingActivityMutex)
				{
					HuodongCachingMgr._LevelKingActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/LevelKing.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetLevelKingActivity()
		{
			string fileName = "Config/Gifts/LevelKing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._LevelKingActivityMutex)
			{
				HuodongCachingMgr._LevelKingActivity = null;
			}
			return 0;
		}

		
		public static KingActivity GetEquipKingActivity()
		{
			lock (HuodongCachingMgr._EquipKingActivityMutex)
			{
				if (HuodongCachingMgr._EquipKingActivity != null)
				{
					return HuodongCachingMgr._EquipKingActivity;
				}
			}
			try
			{
				string fileName = "Config/XinFuGifts/MuBoss.xml";
				if (Global.isDoubleXinFu(36))
				{
					fileName = "Config/XinFuGifts/MuDoubleBoss.xml";
				}
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				KingActivity activity = new KingActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 0, 0, 0);
					activity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(8, 23, 59, 59);
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							AwardItem myAwardItem2 = new AwardItem();
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinBoss"));
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取Boss王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取Boss王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "Boss王活动配置");
								}
							}
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取Boss王活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取Boss王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "Boss王活动配置");
								}
							}
							string rankings = Global.GetSafeAttributeStr(xmlItem, "ID");
							string[] paiHangs = rankings.Split(new char[]
							{
								'-'
							});
							if (paiHangs.Length > 0)
							{
								int min = Global.SafeConvertToInt32(paiHangs[0]);
								int max = Global.SafeConvertToInt32(paiHangs[paiHangs.Length - 1]);
								for (int paiHang = min; paiHang <= max; paiHang++)
								{
									activity.AwardDict.Add(paiHang, myAwardItem);
									activity.AwardDict2.Add(paiHang, myAwardItem2);
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._EquipKingActivityMutex)
				{
					HuodongCachingMgr._EquipKingActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/XinFuGifts/MuBoss.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetEquipKingActivity()
		{
			string fileName = "Config/Gifts/MuBoss.xml";
			if (Global.isDoubleXinFu(36))
			{
				fileName = "Config/XinFuGifts/MuDoubleBoss.xml";
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._EquipKingActivityMutex)
			{
				HuodongCachingMgr._EquipKingActivity = null;
			}
			return 0;
		}

		
		public static KingActivity GetHorseKingActivity()
		{
			lock (HuodongCachingMgr._HorseKingActivityMutex)
			{
				if (HuodongCachingMgr._HorseKingActivity != null)
				{
					return HuodongCachingMgr._HorseKingActivity;
				}
			}
			try
			{
				string fileName = "Config/Gifts/WuXueKing.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				KingActivity activity = new KingActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.ToDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					activity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(10, 23, 59, 59);
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinWuXue"));
							myAwardItem.AwardYuanBao = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "YuanBao"));
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取武学王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取武学王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "武学王活动配置");
								}
							}
							string rankings = Global.GetSafeAttributeStr(xmlItem, "Ranking");
							string[] paiHangs = rankings.Split(new char[]
							{
								'-'
							});
							if (paiHangs.Length > 0)
							{
								int min = Global.SafeConvertToInt32(paiHangs[0]);
								int max = Global.SafeConvertToInt32(paiHangs[paiHangs.Length - 1]);
								for (int paiHang = min; paiHang <= max; paiHang++)
								{
									activity.AwardDict.Add(paiHang, myAwardItem);
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._HorseKingActivityMutex)
				{
					HuodongCachingMgr._HorseKingActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/WuXueKing.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetHorseKingActivity()
		{
			string fileName = "Config/Gifts/WuXueKing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._HorseKingActivityMutex)
			{
				HuodongCachingMgr._HorseKingActivity = null;
			}
			return 0;
		}

		
		public static KingActivity GetJingMaiKingActivity()
		{
			lock (HuodongCachingMgr._JingMaiKingActivityMutex)
			{
				if (HuodongCachingMgr._JingMaiKingActivity != null)
				{
					return HuodongCachingMgr._JingMaiKingActivity;
				}
			}
			try
			{
				string fileName = "Config/Gifts/JingMaiKing.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				KingActivity activity = new KingActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.ToDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 7, 10, 0);
					activity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(10, 23, 59, 59);
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinJingMai"));
							myAwardItem.AwardYuanBao = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "YuanBao"));
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取经脉王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取经脉王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "经脉王活动配置");
								}
							}
							string rankings = Global.GetSafeAttributeStr(xmlItem, "Ranking");
							string[] paiHangs = rankings.Split(new char[]
							{
								'-'
							});
							if (paiHangs.Length > 0)
							{
								int min = Global.SafeConvertToInt32(paiHangs[0]);
								int max = Global.SafeConvertToInt32(paiHangs[paiHangs.Length - 1]);
								for (int paiHang = min; paiHang <= max; paiHang++)
								{
									activity.AwardDict.Add(paiHang, myAwardItem);
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._JingMaiKingActivityMutex)
				{
					HuodongCachingMgr._JingMaiKingActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/JingMaiKing.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetJingMaiKingActivity()
		{
			string fileName = "Config/Gifts/JingMaiKing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._JingMaiKingActivityMutex)
			{
				HuodongCachingMgr._JingMaiKingActivity = null;
			}
			return 0;
		}

		
		public static KingActivity GetXinXiaoFeiKingActivity()
		{
			lock (HuodongCachingMgr._XinXiaofeiKingMutex)
			{
				if (HuodongCachingMgr._XinXiaofeiKingActivity != null)
				{
					return HuodongCachingMgr._XinXiaofeiKingActivity;
				}
			}
			try
			{
				string fileName = "Config/XinFuGifts/MuXiaoFei.xml";
				if (Global.isDoubleXinFu(35))
				{
					fileName = "Config/XinFuGifts/MuDoubleXiaoFei.xml";
				}
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				KingActivity activity = new KingActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(7, 0, 0, 0);
					activity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(8, 23, 59, 59);
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							AwardItem myAwardItem2 = new AwardItem();
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							myAwardItem.AwardYuanBao = 0;
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取新服消费达人活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取新服消费达人活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "新服消费达人活动配置");
								}
							}
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("新服消费达人活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("新服消费达人活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "新服消费达人活动配置");
								}
							}
							string rankings = Global.GetSafeAttributeStr(xmlItem, "ID");
							string[] paiHangs = rankings.Split(new char[]
							{
								'-'
							});
							if (paiHangs.Length > 0)
							{
								int min = Global.SafeConvertToInt32(paiHangs[0]);
								int max = Global.SafeConvertToInt32(paiHangs[paiHangs.Length - 1]);
								for (int paiHang = min; paiHang <= max; paiHang++)
								{
									activity.AwardDict.Add(paiHang, myAwardItem);
									activity.AwardDict2.Add(paiHang, myAwardItem2);
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._XinXiaofeiKingMutex)
				{
					HuodongCachingMgr._XinXiaofeiKingActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/XinFuGifts/MuXiaoFei.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetXinXiaoFeiKingActivity()
		{
			string fileName = "Config/JieRiGifts/MuXiaoFei.xml";
			if (Global.isDoubleXinFu(35))
			{
				fileName = "Config/XinFuGifts/MuDoubleXiaoFei.xml";
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._XinXiaofeiKingMutex)
			{
				HuodongCachingMgr._XinXiaofeiKingActivity = null;
			}
			return 0;
		}

		
		public static void ReadAwardConfig(XElement args, out Dictionary<int, AwardItem> AwardDict, out Dictionary<int, AwardItem> AwardDict2)
		{
			AwardDict = new Dictionary<int, AwardItem>();
			AwardDict2 = new Dictionary<int, AwardItem>();
			if (null != args)
			{
				IEnumerable<XElement> xmlItems = args.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						AwardItem myAwardItem = new AwardItem();
						AwardItem myAwardItem2 = new AwardItem();
						XAttribute hasAttr = xmlItem.Attribute("MinYuanBao");
						if (hasAttr != null)
						{
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
						}
						myAwardItem.AwardYuanBao = 0;
						string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取新服消费达人活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取新服消费达人活动配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "新服消费达人活动配置");
							}
						}
						goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("新服消费达人活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("新服消费达人活动配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								myAwardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "新服消费达人活动配置");
							}
						}
						string indexstr = Global.GetSafeAttributeStr(xmlItem, "ID");
						int index = Global.SafeConvertToInt32(indexstr);
						AwardDict.Add(index, myAwardItem);
						AwardDict2.Add(index, myAwardItem2);
					}
				}
			}
		}

		
		public static void ReadAwardConfig(XElement args, out Dictionary<int, AwardItem> AwardDict, out Dictionary<int, AwardItem> AwardDict2, out Dictionary<int, AwardEffectTimeItem> AwardDict3)
		{
			AwardDict = new Dictionary<int, AwardItem>();
			AwardDict2 = new Dictionary<int, AwardItem>();
			AwardDict3 = new Dictionary<int, AwardEffectTimeItem>();
			if (null != args)
			{
				IEnumerable<XElement> xmlItems = args.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						AwardItem myAwardItem = new AwardItem();
						AwardItem myAwardItem2 = new AwardItem();
						AwardEffectTimeItem myAwardItem3 = new AwardEffectTimeItem();
						XAttribute hasAttr = xmlItem.Attribute("MinYuanBao");
						if (hasAttr != null)
						{
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
						}
						myAwardItem.AwardYuanBao = 0;
						string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("节日活动返利配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("节日活动返利配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "节日活动返利配置");
							}
						}
						goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("节日活动返利配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("节日活动返利配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								myAwardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "节日活动返利配置");
							}
						}
						goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsThr");
						if (!string.IsNullOrEmpty(goodsIDs))
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length > 0)
							{
								myAwardItem3.Init(goodsIDs, Global.GetSafeAttributeStr(xmlItem, "EffectiveTime"), "节日返利");
							}
						}
						string indexstr = Global.GetSafeAttributeStr(xmlItem, "ID");
						int index = Global.SafeConvertToInt32(indexstr);
						AwardDict.Add(index, myAwardItem);
						AwardDict2.Add(index, myAwardItem2);
						AwardDict3.Add(index, myAwardItem3);
					}
				}
			}
		}

		
		public static HuodongCachingMgr.TotalChargeActivity GetTotalChargeActivity()
		{
			lock (HuodongCachingMgr._TotalChargeActivityMutex)
			{
				if (HuodongCachingMgr._TotalChargeActivity != null)
				{
					return HuodongCachingMgr._TotalChargeActivity;
				}
			}
			try
			{
				string fileName = "Config/Gifts/LeiJiChongZhi.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				HuodongCachingMgr.TotalChargeActivity activity = new HuodongCachingMgr.TotalChargeActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.ActivityType = 38;
				}
				args = xml.Element("GiftList");
				HuodongCachingMgr.ReadAwardConfig(args, out activity.AwardDict, out activity.AwardDict2);
				activity.PredealDateTime();
				lock (HuodongCachingMgr._TotalChargeActivityMutex)
				{
					HuodongCachingMgr._TotalChargeActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/LeiJiChongZhi.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetTotalChargeActivity()
		{
			string fileName = "Config/Gifts/LeiJiChongZhi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._TotalChargeActivityMutex)
			{
				HuodongCachingMgr._TotalChargeActivity = null;
			}
			return 0;
		}

		
		public static HuodongCachingMgr.TotalConsumeActivity GetTotalConsumeActivity()
		{
			lock (HuodongCachingMgr._TotalConsumeActivityMutex)
			{
				if (HuodongCachingMgr._TotalConsumeActivity != null)
				{
					return HuodongCachingMgr._TotalConsumeActivity;
				}
			}
			try
			{
				string fileName = "Config/Gifts/LeiJiXiaoFei.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				HuodongCachingMgr.TotalConsumeActivity activity = new HuodongCachingMgr.TotalConsumeActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.ActivityType = 39;
				}
				args = xml.Element("GiftList");
				HuodongCachingMgr.ReadAwardConfig(args, out activity.AwardDict, out activity.AwardDict2);
				activity.PredealDateTime();
				lock (HuodongCachingMgr._TotalConsumeActivityMutex)
				{
					HuodongCachingMgr._TotalConsumeActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/LeiJiXiaoFei.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetTotalConsumeActivity()
		{
			string fileName = "Config/Gifts/LeiJiXiaoFei.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._TotalConsumeActivityMutex)
			{
				HuodongCachingMgr._TotalConsumeActivity = null;
			}
			return 0;
		}

		
		public static JieriFanLiActivity GetJieriFanLiActivity(ActivityTypes nActType)
		{
			int nArrayIdx = 0;
			string attrstr = "";
			switch (nActType)
			{
			case ActivityTypes.JieriWing:
				nArrayIdx = 0;
				attrstr = "WingLevel";
				break;
			case ActivityTypes.JieriAddon:
				nArrayIdx = 1;
				attrstr = "ZhuiJiaLevel";
				break;
			case ActivityTypes.JieriStrengthen:
				nArrayIdx = 2;
				attrstr = "QiangHuaLevel";
				break;
			case ActivityTypes.JieriAchievement:
				nArrayIdx = 3;
				attrstr = "ChengJiuLevel";
				break;
			case ActivityTypes.JieriMilitaryRank:
				nArrayIdx = 4;
				attrstr = "JunXianLevel";
				break;
			case ActivityTypes.JieriVIPFanli:
				nArrayIdx = 5;
				attrstr = "VIPLevel";
				break;
			case ActivityTypes.JieriAmulet:
				nArrayIdx = 6;
				attrstr = "HuShenFuLevel";
				break;
			case ActivityTypes.JieriArchangel:
				nArrayIdx = 7;
				attrstr = "DaTianShiLevel";
				break;
			case ActivityTypes.JieriLianXuCharge:
				break;
			case ActivityTypes.JieriMarriage:
				nArrayIdx = 8;
				attrstr = "GoodWillSuit";
				break;
			default:
				switch (nActType)
				{
				case ActivityTypes.JieRiHuiJi:
					nArrayIdx = 9;
					attrstr = "EmblemLevel";
					break;
				case ActivityTypes.JieRiFuWen:
					nArrayIdx = 10;
					attrstr = "FuWenLevel";
					break;
				}
				break;
			}
			lock (HuodongCachingMgr._JieriWingFanliActMutex)
			{
				if (null != HuodongCachingMgr._JieriWingFanliAct[nArrayIdx])
				{
					return HuodongCachingMgr._JieriWingFanliAct[nArrayIdx];
				}
			}
			string fileName = "";
			try
			{
				fileName = "Config/JieRiGifts/";
				switch (nActType)
				{
				case ActivityTypes.JieriWing:
					fileName += "WingFanLi.xml";
					break;
				case ActivityTypes.JieriAddon:
					fileName += "ZhuiJiaFanLi.xml";
					break;
				case ActivityTypes.JieriStrengthen:
					fileName += "QiangHuaFanLi.xml";
					break;
				case ActivityTypes.JieriAchievement:
					fileName += "ChengJiuFanLi.xml";
					break;
				case ActivityTypes.JieriMilitaryRank:
					fileName += "JunXianFanLi.xml";
					break;
				case ActivityTypes.JieriVIPFanli:
					fileName += "VIPFanLi.xml";
					break;
				case ActivityTypes.JieriAmulet:
					fileName += "HuShenFuFanLi.xml";
					break;
				case ActivityTypes.JieriArchangel:
					fileName += "DaTianShiFanLi.xml";
					break;
				case ActivityTypes.JieriLianXuCharge:
					break;
				case ActivityTypes.JieriMarriage:
					fileName += "HunYinFanLi.xml";
					break;
				default:
					switch (nActType)
					{
					case ActivityTypes.JieRiHuiJi:
						fileName += "JieRiHuiJiFanLi.xml";
						break;
					case ActivityTypes.JieRiFuWen:
						fileName += "JieRiFuWenFanLi.xml";
						break;
					}
					break;
				}
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				JieriFanLiActivity activity = new JieriFanLiActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.ActivityType = (int)nActType;
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				HuodongCachingMgr.ReadAwardConfig(args, out activity.AwardDict, out activity.AwardDict2, out activity.AwardDict3);
				activity.PredealDateTime();
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							string indexstr = Global.GetSafeAttributeStr(xmlItem, "ID");
							int index = Global.SafeConvertToInt32(indexstr);
							indexstr = Global.GetSafeAttributeStr(xmlItem, attrstr);
							string[] attrarray = indexstr.Split(new char[]
							{
								','
							});
							activity.AwardDict[index].MinAwardCondionValue = Convert.ToInt32(attrarray[0]);
							if (attrarray.Length > 1)
							{
								activity.AwardDict[index].MinAwardCondionValue2 = Convert.ToInt32(attrarray[1]);
							}
						}
					}
				}
				lock (HuodongCachingMgr._JieriWingFanliActMutex)
				{
					HuodongCachingMgr._JieriWingFanliAct[nArrayIdx] = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常", fileName), ex, true);
			}
			return null;
		}

		
		public static JieriLianXuChargeActivity GetJieriLianXuChargeActivity()
		{
			lock (HuodongCachingMgr._JieriLianXuChargeMutex)
			{
				if (HuodongCachingMgr._JieriLianXuChargeAct != null)
				{
					return HuodongCachingMgr._JieriLianXuChargeAct;
				}
			}
			JieriLianXuChargeActivity act = new JieriLianXuChargeActivity();
			if (act.Init())
			{
				lock (HuodongCachingMgr._JieriLianXuChargeMutex)
				{
					HuodongCachingMgr._JieriLianXuChargeAct = act;
					return HuodongCachingMgr._JieriLianXuChargeAct;
				}
			}
			return null;
		}

		
		public static int ResetJieriLianXuChargeActivity()
		{
			lock (HuodongCachingMgr._JieriLianXuChargeMutex)
			{
				HuodongCachingMgr._JieriLianXuChargeAct = null;
			}
			return 0;
		}

		
		public static JieriPlatChargeKingEveryDay GetJieriPCKingEveryDayActivity()
		{
			lock (HuodongCachingMgr._JieriPCKingEveryDayMutex)
			{
				if (HuodongCachingMgr._JieriPCKingEveryDayAct != null)
				{
					return HuodongCachingMgr._JieriPCKingEveryDayAct;
				}
			}
			JieriPlatChargeKingEveryDay act = new JieriPlatChargeKingEveryDay();
			if (act.Init())
			{
				lock (HuodongCachingMgr._JieriPCKingEveryDayMutex)
				{
					HuodongCachingMgr._JieriPCKingEveryDayAct = act;
					return HuodongCachingMgr._JieriPCKingEveryDayAct;
				}
			}
			return null;
		}

		
		public static int ResetJieriPCKingActivityEveryDay()
		{
			lock (HuodongCachingMgr._JieriPCKingEveryDayMutex)
			{
				HuodongCachingMgr._JieriPCKingEveryDayAct = null;
			}
			return 0;
		}

		
		public static JieriPlatChargeKing GetJieriPlatChargeKingActivity()
		{
			lock (HuodongCachingMgr._JieriPlatChargeKingMutex)
			{
				if (HuodongCachingMgr._JieriPlatChargeKingAct != null)
				{
					return HuodongCachingMgr._JieriPlatChargeKingAct;
				}
			}
			JieriPlatChargeKing act = new JieriPlatChargeKing();
			if (act.Init())
			{
				lock (HuodongCachingMgr._JieriPlatChargeKingMutex)
				{
					HuodongCachingMgr._JieriPlatChargeKingAct = act;
					return HuodongCachingMgr._JieriPlatChargeKingAct;
				}
			}
			return null;
		}

		
		public static int ResetJieriPlatChargeKingActivity()
		{
			lock (HuodongCachingMgr._JieriPlatChargeKingMutex)
			{
				HuodongCachingMgr._JieriPlatChargeKingAct = null;
			}
			return 0;
		}

		
		private static void InitUpLevelAwardItemDict()
		{
			if (null == HuodongCachingMgr.UpLevelAwardItemDict)
			{
				try
				{
					string fileName = "Config/Gifts/UpLevelAward.xml";
					XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
					if (null != xml)
					{
						Dictionary<int, UpLevelAwardItem> upLevelAwardItemDict = new Dictionary<int, UpLevelAwardItem>();
						IEnumerable<XElement> args = xml.Elements("Level");
						if (null != args)
						{
							foreach (XElement xmlItem in args)
							{
								UpLevelAwardItem upLevelAwardItem = new UpLevelAwardItem
								{
									ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID"),
									MinDay = (int)Global.GetSafeAttributeLong(xmlItem, "MinDay"),
									MaxDay = (int)Global.GetSafeAttributeLong(xmlItem, "MaxDay"),
									AwardYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "AwardYuanBao")
								};
								upLevelAwardItemDict[upLevelAwardItem.ID] = upLevelAwardItem;
							}
						}
						HuodongCachingMgr.UpLevelAwardItemDict = upLevelAwardItemDict;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/UpLevelAward.xml解析出现异常", ex, true);
				}
			}
		}

		
		public static void ProcessUpLevelAward4_60Level_100Level(GameClient client, int oldLevel, int newLevel)
		{
			HuodongCachingMgr.InitUpLevelAwardItemDict();
			Dictionary<int, UpLevelAwardItem> upLevelAwardItemDict = HuodongCachingMgr.UpLevelAwardItemDict;
			if (null != upLevelAwardItemDict)
			{
				int elapsedDays = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), Global.GetRegTime(client.ClientData), false);
				elapsedDays++;
				int bitValue = 0;
				if (oldLevel < 60 && newLevel >= 60)
				{
					for (int i = 0; i < upLevelAwardItemDict.Values.Count; i++)
					{
						UpLevelAwardItem upLevelAwardItem = upLevelAwardItemDict.Values.ElementAt(i);
						if (elapsedDays >= upLevelAwardItem.MinDay && elapsedDays <= upLevelAwardItem.MaxDay)
						{
							bitValue = (int)Math.Pow(2.0, (double)i);
						}
					}
				}
				else if (oldLevel < 100 && newLevel >= 100)
				{
					if (elapsedDays >= 1 && elapsedDays <= 100)
					{
						bitValue = 16;
					}
				}
				if (bitValue > 0)
				{
					int nID = GameManager.ClientMgr.GetTo60or100ID(client);
					if ((nID & bitValue) != bitValue)
					{
						nID |= bitValue;
						GameManager.ClientMgr.ModifyTo60or100ID(client, nID, true, true);
					}
				}
			}
		}

		
		public static void ProcessGetUpLevelAward4_60Level_100Level(GameClient client, int awardID)
		{
			HuodongCachingMgr.InitUpLevelAwardItemDict();
			Dictionary<int, UpLevelAwardItem> upLevelAwardItemDict = HuodongCachingMgr.UpLevelAwardItemDict;
			if (null != upLevelAwardItemDict)
			{
				UpLevelAwardItem upLevelAwardItem = null;
				if (upLevelAwardItemDict.TryGetValue(awardID, out upLevelAwardItem))
				{
					if (null != upLevelAwardItem)
					{
						int disableTo60level = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-to60level", 0);
						if (disableTo60level <= 0)
						{
							int bitValue = 32;
							int nID = GameManager.ClientMgr.GetTo60or100ID(client);
							if ((nID & bitValue) == bitValue)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(388, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
							else
							{
								nID |= bitValue;
								GameManager.ClientMgr.ModifyTo60or100ID(client, nID, true, true);
								bool ret = false;
								if (null != client)
								{
									ret = GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, upLevelAwardItem.AwardYuanBao, "达到60级绑定元宝奖励");
								}
								if (!ret)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("处理达到60级绑定元宝奖励时，为角色名称={0}, 添加绑定元宝{1} 失败", client.ClientData.RoleName, upLevelAwardItem.AwardYuanBao), null, true);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(389, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, string.Format(GLang.GetLang(391, new object[0]), upLevelAwardItem.AwardYuanBao), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
									Global.BroadcastClientTo60(client, upLevelAwardItem.MinDay, upLevelAwardItem.MaxDay, upLevelAwardItem.AwardYuanBao);
								}
							}
						}
					}
				}
			}
		}

		
		private static void InitKaiFuGiftItemDict()
		{
			if (null == HuodongCachingMgr.KaiFuGiftItemDict)
			{
				try
				{
					string fileName = "Config/Gifts/KaiFuGift.xml";
					XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
					if (null != xml)
					{
						Dictionary<int, KaiFuGiftItem> kaiFuGiftItemDict = new Dictionary<int, KaiFuGiftItem>();
						IEnumerable<XElement> args = xml.Elements("KaiFu");
						if (null != args)
						{
							foreach (XElement xmlItem in args)
							{
								KaiFuGiftItem kaiFuGiftItem = new KaiFuGiftItem
								{
									Day = (int)Global.GetSafeAttributeLong(xmlItem, "Day"),
									MinTime = (int)Global.GetSafeAttributeLong(xmlItem, "MinTime"),
									MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel"),
									AwardYuanBao = (int)Global.GetSafeAttributeLong(xmlItem, "AwardYuanBao")
								};
								kaiFuGiftItemDict[kaiFuGiftItem.Day] = kaiFuGiftItem;
							}
						}
						HuodongCachingMgr.KaiFuGiftItemDict = kaiFuGiftItemDict;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/KaiFuGift.xml解析出现异常", ex, true);
				}
			}
		}

		
		public static void ProcessKaiFuGiftAward(GameClient client)
		{
			int level = client.ClientData.Level;
			if (level >= 40)
			{
				int hours = client.ClientData.TotalOnlineSecs / 3600;
				if (hours >= 2)
				{
					int elapsedDays = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), Global.GetKaiFuTime(), true);
					elapsedDays++;
					Dictionary<int, KaiFuGiftItem> kaiFuGiftItemDict = HuodongCachingMgr.KaiFuGiftItemDict;
					if (null != kaiFuGiftItemDict)
					{
						KaiFuGiftItem kaiFuGiftItem = null;
						if (kaiFuGiftItemDict.TryGetValue(elapsedDays, out kaiFuGiftItem))
						{
							if (level >= kaiFuGiftItem.MinLevel)
							{
								if (hours >= kaiFuGiftItem.MinTime)
								{
									int dayID = GameManager.ClientMgr.GetKaiFuOnlineDayID(client);
									if (elapsedDays != dayID)
									{
										GameManager.ClientMgr.ModifyKaiFuOnlineDayID(client, elapsedDays, true, true);
										int kaiFuOnlineDayBit = Global.GetRoleParamsInt32FromDB(client, "KaiFuOnlineDayBit");
										kaiFuOnlineDayBit |= (int)Math.Pow(2.0, (double)(elapsedDays - 1));
										Global.SaveRoleParamsInt32ValueToDB(client, "KaiFuOnlineDayBit", kaiFuOnlineDayBit, true);
									}
								}
							}
						}
					}
				}
			}
		}

		
		public static void ProcessDayOnlineSecs(GameClient client, int preLoginDay)
		{
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (dayID != preLoginDay)
			{
				int elapsedDays = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), Global.GetKaiFuTime(), true);
				elapsedDays++;
				if (elapsedDays > 1)
				{
					if (elapsedDays < 9)
					{
						int onlineSecs = client.ClientData.TotalOnlineSecs;
						Global.SaveRoleParamsInt32ValueToDB(client, string.Format("{0}{1}", "KaiFuOnlineDayTimes_", elapsedDays - 1), onlineSecs, true);
					}
				}
			}
		}

		
		public static bool GetCurrentDayKaiFuOnlineSecs(GameClient client, out int totalOnlineSecs, out int dayID)
		{
			totalOnlineSecs = 0;
			dayID = 0;
			int elapsedDays = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), Global.GetKaiFuTime(), true);
			elapsedDays++;
			bool result;
			if (elapsedDays >= 8)
			{
				result = false;
			}
			else
			{
				totalOnlineSecs = client.ClientData.TotalOnlineSecs;
				dayID = elapsedDays;
				result = true;
			}
			return result;
		}

		
		public static void ProcessKaiFuGiftAwardActions()
		{
			HuodongCachingMgr.ProcessGetKaiFuGiftAward();
			HuodongCachingMgr.ProcessAutoAddKaiFuGiftRoleNum();
		}

		
		public static void ProcessGetKaiFuGiftAward()
		{
			int elapsedDays = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), Global.GetKaiFuTime(), true);
			elapsedDays++;
			if (elapsedDays > 1)
			{
				if (elapsedDays < 9)
				{
					int dayID = TimeUtil.NowDateTime().DayOfYear;
					if (dayID != HuodongCachingMgr.LastProcessGetKaiFuGiftAwardDayID)
					{
						if (HuodongCachingMgr.ProcessKaiFuGiftAwardHour == TimeUtil.NowDateTime().Hour)
						{
							HuodongCachingMgr.LastProcessGetKaiFuGiftAwardDayID = dayID;
							int disableKaifuaward = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-kaifuaward", 0);
							if (disableKaifuaward <= 0)
							{
								string[] dbFields = Global.ExecuteDBCmd(10111, string.Format("{0}", elapsedDays - 1), 0);
								if (dbFields != null && dbFields.Length >= 4)
								{
									int roleID = Global.SafeConvertToInt32(dbFields[0]);
									if (roleID > 0)
									{
										int zoneID = Global.SafeConvertToInt32(dbFields[1]);
										string roleName = dbFields[2];
										int totalRoleNum = Global.SafeConvertToInt32(dbFields[3]);
										Dictionary<int, KaiFuGiftItem> kaiFuGiftItemDict = HuodongCachingMgr.KaiFuGiftItemDict;
										if (null != kaiFuGiftItemDict)
										{
											KaiFuGiftItem kaiFuGiftItem = null;
											if (kaiFuGiftItemDict.TryGetValue(elapsedDays - 1, out kaiFuGiftItem))
											{
												GameClient client = GameManager.ClientMgr.FindClient(roleID);
												bool ret;
												if (null != client)
												{
													ret = GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, kaiFuGiftItem.AwardYuanBao, "开服在线奖励", ActivityTypes.None, "");
												}
												else
												{
													ret = GameManager.ClientMgr.AddUserMoneyOffLine(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, roleID, kaiFuGiftItem.AwardYuanBao, "开服在线奖励", zoneID, Global.QueryUserMoneyFromDB(roleID, roleName, 0));
												}
												if (!ret)
												{
													LogManager.WriteLog(LogTypes.Error, string.Format("处理开服在线奖励活动时，为角色名称={0}, 添加元宝{1} 失败", roleName, kaiFuGiftItem.AwardYuanBao), null, true);
												}
												else
												{
													GameManager.DBCmdMgr.AddDBCmd(10112, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
													{
														roleID,
														Math.Max(1, elapsedDays - 1),
														kaiFuGiftItem.AwardYuanBao,
														totalRoleNum,
														zoneID
													}), null, 0);
													GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", roleID, kaiFuGiftItem.AwardYuanBao, GLang.GetLang(392, new object[0])), null, 0);
													Global.BroadcastClientKaiFuOnlineRandomAward(zoneID, roleName, kaiFuGiftItem.AwardYuanBao);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		public static void ProcessAutoAddKaiFuGiftRoleNum()
		{
		}

		
		public static void FixKaiFuOnlineAwardDataList(List<KaiFuOnlineAwardData> kaiFuOnlineAwardDataList, int dayID, int serverId)
		{
			if (null != kaiFuOnlineAwardDataList)
			{
				bool founded = false;
				for (int i = 0; i < kaiFuOnlineAwardDataList.Count; i++)
				{
					if (kaiFuOnlineAwardDataList[i].DayID == dayID)
					{
						founded = true;
					}
				}
				if (!founded)
				{
					string[] dbFields = Global.ExecuteDBCmd(10111, string.Format("{0}", dayID), serverId);
					if (dbFields != null && dbFields.Length >= 4)
					{
						int totalRoleNum = Global.SafeConvertToInt32(dbFields[3]);
						kaiFuOnlineAwardDataList.Add(new KaiFuOnlineAwardData
						{
							DayID = dayID,
							TotalRoleNum = totalRoleNum
						});
					}
				}
			}
		}

		
		public static void OnJieriRoleLogin(GameClient client, int preLoginDay, bool isLogin = false)
		{
			int currDayID = Global.GetOffsetDayNow();
			int roleLoginDayID = Math.Max(0, Global.GetRoleParamsInt32FromDB(client, "10152"));
			if (roleLoginDayID > currDayID)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("玩家退后登陆了！！rid={0}, rname={1}", client.ClientData.RoleID, client.ClientData.RoleName), null, true);
			}
			else
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "10152", currDayID, true);
				OneDollarBuyActivity odbAct = HuodongCachingMgr.GetOneDollarBuyActivity();
				if (null != odbAct)
				{
					odbAct.OnRoleLogin(client);
				}
				OneDollarChongZhi odczAct = HuodongCachingMgr.GetOneDollarChongZhiActivity();
				if (null != odczAct)
				{
					odczAct.OnRoleLogin(client);
				}
				InputFanLiNew iflAct = HuodongCachingMgr.GetInputFanLiNewActivity();
				if (null != iflAct)
				{
					iflAct.OnRoleLogin(client);
				}
				RegressActiveOpen raoAct = HuodongCachingMgr.GetRegressActiveOpen();
				if (null != raoAct)
				{
					raoAct.OnRoleLogin(client);
				}
				SpecialActivity specAct = HuodongCachingMgr.GetSpecialActivity();
				if (null != specAct)
				{
					specAct.OnRoleLogin(client, isLogin);
				}
				EverydayActivity everyAct = HuodongCachingMgr.GetEverydayActivity();
				if (null != everyAct)
				{
					everyAct.OnRoleLogin(client);
				}
				SpecPriorityActivity specPAct = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != specAct)
				{
					specPAct.OnRoleLogin(client, isLogin);
				}
				WeedEndInputActivity act = HuodongCachingMgr.GetWeekEndInputActivity();
				if (null != act)
				{
					act.OnRoleLogin(client, isLogin);
				}
				JieriSuperInputActivity jrsiAct = HuodongCachingMgr.GetJieRiSuperInputActivity();
				if (null != jrsiAct)
				{
					jrsiAct.OnRoleLogin(client);
				}
				int jieriLoginDayID = Math.Max(0, Global.GetRoleParamsInt32FromDB(client, "JieriLoginDayID"));
				JieRiDengLuActivity instance = HuodongCachingMgr.GetJieRiDengLuActivity();
				if (instance != null && instance.InActivityTime() && jieriLoginDayID < currDayID)
				{
					DateTime startDay = DateTime.Parse(instance.FromDate);
					DateTime endDay = DateTime.Parse(instance.ToDate);
					int startDayID = Global.GetOffsetDay(startDay);
					int endDayID = Global.GetOffsetDay(endDay);
					int jieriLoginNum = Math.Max(0, Global.GetRoleParamsInt32FromDB(client, "JieriLoginNum"));
					if (jieriLoginDayID >= startDayID && jieriLoginDayID <= endDayID)
					{
						jieriLoginNum++;
						if (jieriLoginNum > currDayID - startDayID + 1)
						{
							jieriLoginNum = currDayID - startDayID + 1;
						}
					}
					else
					{
						jieriLoginNum = 1;
					}
					Global.SaveRoleParamsInt32ValueToDB(client, "JieriLoginNum", jieriLoginNum, true);
					Global.SaveRoleParamsInt32ValueToDB(client, "JieriLoginDayID", currDayID, true);
				}
			}
		}

		
		public static int GetZiKaTodayLeftMergeNum(GameClient client, int index)
		{
			JieRiZiKaLiaBaoActivity instance = HuodongCachingMgr.GetJieRiZiKaLiaBaoActivity();
			int result;
			if (null == instance)
			{
				result = 0;
			}
			else
			{
				JieRiZiKa config = instance.GetAward(index);
				if (null == config)
				{
					result = 0;
				}
				else
				{
					int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
					int lastday = 0;
					int count = 0;
					string strFlag = "JRExcharge" + index;
					string JieRiExchargeFlag = Global.GetRoleParamByName(client, strFlag);
					if (null != JieRiExchargeFlag)
					{
						string[] fields = JieRiExchargeFlag.Split(new char[]
						{
							','
						});
						if (2 == fields.Length)
						{
							lastday = Convert.ToInt32(fields[0]);
							count = Convert.ToInt32(fields[1]);
						}
					}
					if (currday == lastday)
					{
						result = config.DayMaxTimes - count;
					}
					else
					{
						result = config.DayMaxTimes;
					}
				}
			}
			return result;
		}

		
		public static int ModifyZiKaTodayLeftMergeNum(GameClient client, int index, int addNum = 1)
		{
			int currday = Global.GetOffsetDay(TimeUtil.NowDateTime());
			string strFlag = "JRExcharge" + index;
			string JieRiExchargeFlag = Global.GetRoleParamByName(client, strFlag);
			int lastday = 0;
			int count = 0;
			if (null != JieRiExchargeFlag)
			{
				string[] fields = JieRiExchargeFlag.Split(new char[]
				{
					','
				});
				if (2 != fields.Length)
				{
					return 0;
				}
				lastday = Convert.ToInt32(fields[0]);
				count = Convert.ToInt32(fields[1]);
			}
			if (currday == lastday)
			{
				count += addNum;
			}
			else
			{
				lastday = currday;
				count = addNum;
			}
			string result = string.Format("{0},{1}", lastday, count);
			Global.SaveRoleParamsStringToDB(client, strFlag, result, true);
			return count;
		}

		
		public static string MergeZiKa(GameClient client, int index)
		{
			string strcmd = string.Format("{0}:{1}:{2}", 0, client.ClientData.RoleID, 14);
			string result;
			if (HuodongCachingMgr.GetZiKaTodayLeftMergeNum(client, index) <= 0)
			{
				strcmd = string.Format("{0}:{1}:{2}", -20000, client.ClientData.RoleID, 14);
				result = strcmd;
			}
			else
			{
				JieRiZiKaLiaBaoActivity instance = HuodongCachingMgr.GetJieRiZiKaLiaBaoActivity();
				if (null == instance)
				{
					strcmd = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 14);
					result = strcmd;
				}
				else
				{
					JieRiZiKa config = instance.GetAward(index);
					if (null == config)
					{
						strcmd = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 14);
						result = strcmd;
					}
					else if (null == config.MyAwardItem)
					{
						strcmd = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 14);
						result = strcmd;
					}
					else if (null == config.MyAwardItem.GoodsDataList)
					{
						strcmd = string.Format("{0}:{1}:{2}", -20001, client.ClientData.RoleID, 14);
						result = strcmd;
					}
					else
					{
						if (null != config.NeedGoodsList)
						{
							for (int i = 0; i < config.NeedGoodsList.Count; i++)
							{
								if (Global.GetTotalGoodsNotUsingCountByID(client, config.NeedGoodsList[i].GoodsID) < config.NeedGoodsList[i].GCount)
								{
									return string.Format("{0}:{1}:{2}", -20003, client.ClientData.RoleID, 14);
								}
							}
						}
						if (config.NeedMoJing > 0)
						{
							if (GameManager.ClientMgr.GetTianDiJingYuanValue(client) < config.NeedMoJing)
							{
								return string.Format("{0}:{1}:{2}", -20004, client.ClientData.RoleID, 14);
							}
						}
						if (config.NeedQiFuJiFen > 0)
						{
							if (Global.GetRoleParamsInt32FromDB(client, "ZJDJiFen") < config.NeedQiFuJiFen)
							{
								return string.Format("{0}:{1}:{2}", -20005, client.ClientData.RoleID, 14);
							}
						}
						if (config.NeedPetJiFen > 0)
						{
						}
						string castResList = "";
						if (config.NeedMoJing > 0)
						{
							int oldMoJing = GameManager.ClientMgr.GetTianDiJingYuanValue(client);
							GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, -config.NeedMoJing, "字卡系统兑换物品", false, true, false);
							castResList += EventLogManager.AddResPropString(ResLogType.RongLianZhi, new object[]
							{
								-config.NeedMoJing,
								oldMoJing,
								GameManager.ClientMgr.GetTianDiJingYuanValue(client)
							});
						}
						if (config.NeedQiFuJiFen > 0)
						{
							int qiFuJiFen = Global.GetRoleParamsInt32FromDB(client, "ZJDJiFen");
							Global.AddZaJinDanJiFen(client, -config.NeedQiFuJiFen, "字卡系统兑换物品", false);
							castResList += EventLogManager.AddResPropString(ResLogType.QiFuJiFen, new object[]
							{
								-config.NeedQiFuJiFen,
								qiFuJiFen,
								Global.GetRoleParamsInt32FromDB(client, "ZJDJiFen")
							});
						}
						if (null != config.NeedGoodsList)
						{
							for (int i = 0; i < config.NeedGoodsList.Count; i++)
							{
								bool usedBinding = false;
								bool usedTimeLimited = false;
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, config.NeedGoodsList[i].GoodsID, config.NeedGoodsList[i].GCount, false, out usedBinding, out usedTimeLimited, true))
								{
									return string.Format("{0}:{1}:{2}", -20004, client.ClientData.RoleID, 14);
								}
								castResList += EventLogManager.AddGoodsDataPropString(config.NeedGoodsList[i]);
							}
						}
						if (!instance.GiveAward(client, index))
						{
							strcmd = string.Format("{0}:{1}:{2}", -20005, client.ClientData.RoleID, 14);
							result = strcmd;
						}
						else
						{
							if (castResList.Length > 0)
							{
								castResList = castResList.Remove(0, 1);
							}
							string strResList = EventLogManager.MakeGoodsDataPropString(config.MyAwardItem.GoodsDataList);
							EventLogManager.AddPurchaseEvent(client, 1, index, castResList, strResList);
							int leftNum = Math.Max(0, config.DayMaxTimes - HuodongCachingMgr.ModifyZiKaTodayLeftMergeNum(client, index, 1));
							strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								1,
								client.ClientData.RoleID,
								14,
								leftNum,
								index
							});
							result = strcmd;
						}
					}
				}
			}
			return result;
		}

		
		public static bool LoadJieriActivitiesConfig()
		{
			JieriActivityConfig config = HuodongCachingMgr.GetJieriActivityConfig();
			bool result;
			if (null == config)
			{
				result = true;
			}
			else
			{
				string strError = "";
				Activity instance = HuodongCachingMgr.GetJieriDaLiBaoActivity();
				if (instance == null || instance.GetParamsValidateCode() < 0)
				{
					strError = "节日大礼包活动配置项出错";
				}
				else
				{
					instance = HuodongCachingMgr.GetJieriIPointsExchgActivity();
					if (instance == null || instance.GetParamsValidateCode() < 0)
					{
						strError = "节日充值点兑换活动配置项出错";
					}
					else
					{
						instance = HuodongCachingMgr.GetJieRiDengLuActivity();
						if (instance == null || instance.GetParamsValidateCode() < 0)
						{
							strError = "节日登录豪礼活动配置项出错";
						}
						else
						{
							instance = HuodongCachingMgr.GetJieriCZSongActivity();
							if (instance == null || instance.GetParamsValidateCode() < 0)
							{
								strError = "节日累计充值活动配置项出错";
							}
							else
							{
								instance = HuodongCachingMgr.GetJieRiZiKaLiaBaoActivity();
								if (instance == null || instance.GetParamsValidateCode() < 0)
								{
									strError = "节日字卡换礼盒活动配置项出错";
								}
								else
								{
									instance = HuodongCachingMgr.GetJieriXiaoFeiKingActivity();
									if (instance == null || instance.GetParamsValidateCode() < 0)
									{
										strError = "节日消费王活动配置项出错";
									}
									else
									{
										instance = HuodongCachingMgr.GetJieRiLeiJiCZActivity();
										if (null == instance)
										{
											strError = "节日累计充值活动配置项出错";
										}
										else
										{
											instance = HuodongCachingMgr.GetJieRiTotalConsumeActivity();
											if (null == instance)
											{
												strError = "节日累计消费活动配置项出错";
											}
											else
											{
												instance = HuodongCachingMgr.GetJieRiMultAwardActivity();
												if (null == instance)
												{
													strError = "节日累计消费活动配置项出错";
												}
												else
												{
													instance = HuodongCachingMgr.GetJieRiCZKingActivity();
													if (null == instance)
													{
														strError = "节日累计消费活动配置项出错";
													}
													else if (HuodongCachingMgr.GetJieriGiveActivity() == null)
													{
														strError = "节日赠送活动配置项出错";
													}
													else if (HuodongCachingMgr.GetJieriGiveKingActivity() == null)
													{
														strError = "节日赠送王配置项出错";
													}
													else if (HuodongCachingMgr.GetJieriRecvKingActivity() == null)
													{
														strError = "节日收取王配置项出错";
													}
													else
													{
														instance = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriWing);
														if (null == instance)
														{
															strError = "节日翅膀返利活动配置项出错";
														}
														else
														{
															instance = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriAddon);
															if (null == instance)
															{
																strError = "节日节日追加返利活动配置项出错";
															}
															else
															{
																instance = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriStrengthen);
																if (null == instance)
																{
																	strError = "节日强化返利活动配置项出错";
																}
																else
																{
																	instance = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriAchievement);
																	if (null == instance)
																	{
																		strError = "节日成就返利活动配置项出错";
																	}
																	else
																	{
																		instance = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriMilitaryRank);
																		if (null == instance)
																		{
																			strError = "节日军衔返利活动配置项出错";
																		}
																		else
																		{
																			instance = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriVIPFanli);
																			if (null == instance)
																			{
																				strError = "节日VIP返利活动配置项出错";
																			}
																			else
																			{
																				instance = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriAmulet);
																				if (null == instance)
																				{
																					strError = "节日护身符返利活动配置项出错";
																				}
																				else
																				{
																					instance = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriArchangel);
																					if (null == instance)
																					{
																						strError = "节日大天使返利活动配置项出错";
																					}
																					else
																					{
																						instance = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieriMarriage);
																						if (null == instance)
																						{
																							strError = "节日婚姻返利活动配置项出错";
																						}
																						else
																						{
																							instance = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieRiHuiJi);
																							if (null == instance)
																							{
																								strError = "节日徽记返利活动配置项出错";
																							}
																							else
																							{
																								instance = HuodongCachingMgr.GetJieriFanLiActivity(ActivityTypes.JieRiFuWen);
																								if (null == instance)
																								{
																									strError = "节日符文返利活动配置项出错";
																								}
																								else
																								{
																									instance = HuodongCachingMgr.GetJieriLianXuChargeActivity();
																									if (null == instance)
																									{
																										strError = "节日连续充值活动配置项出错";
																									}
																									else
																									{
																										instance = HuodongCachingMgr.GetJieriRecvActivity();
																										if (null == instance)
																										{
																											strError = "节日收礼活动配置项出错";
																										}
																										else
																										{
																											instance = HuodongCachingMgr.GetJieriPlatChargeKingActivity();
																											if (null == instance)
																											{
																												strError = "节日平台充值王活动配置出错";
																											}
																											else
																											{
																												instance = HuodongCachingMgr.GetJieriPCKingEveryDayActivity();
																												if (null == instance)
																												{
																													strError = "节日每日平台充值王活动配置出错";
																												}
																												else
																												{
																													instance = HuodongCachingMgr.GetJieriFuLiActivity();
																													if (null == instance)
																													{
																														strError = "节日福利活动配置出错";
																													}
																													else
																													{
																														instance = HuodongCachingMgr.GetJieRiCZQGActivity();
																														if (null == instance)
																														{
																															strError = "节日充值抢购配置出错";
																														}
																														else
																														{
																															instance = HuodongCachingMgr.GetOneDollarBuyActivity();
																															if (null == instance)
																															{
																																strError = "1元直购配置出错";
																															}
																															else
																															{
																																instance = HuodongCachingMgr.GetJieriVIPYouHuiAct();
																																if (null == instance)
																																{
																																	strError = "节日VIP优惠配置出错";
																																}
																																else
																																{
																																	instance = HuodongCachingMgr.GetDanBiChongZhiActivity();
																																	if (instance == null || instance.GetParamsValidateCode() < 0)
																																	{
																																		strError = "单笔充值活动配置项出错";
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(strError))
				{
					LogManager.WriteLog(LogTypes.Fatal, strError, null, true);
					result = true;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		
		public static int GetThemeActivityState()
		{
			ThemeActivityConfig config = HuodongCachingMgr.GetThemeActivityConfig();
			int result;
			if (null != config)
			{
				result = config.ActivityOpenVavle;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public static ThemeActivityConfig GetThemeActivityConfig()
		{
			lock (HuodongCachingMgr._ThemeActivityConfigMutex)
			{
				if (HuodongCachingMgr._ThemeActivityConfig != null)
				{
					return HuodongCachingMgr._ThemeActivityConfig;
				}
			}
			try
			{
				string fileName = "Config/ThemeActivityType.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				ThemeActivityConfig config = new ThemeActivityConfig();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
                    int activityid = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Type"));
					int endData = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "EndData"));
					string filename = Global.GetSafeAttributeStr(xmlItem, "PeiZhi");
					config.ConfigDict[activityid] = filename;
					config.EndDataDict[activityid] = endData;
					config.openList.Add(activityid);
					filename = Global.GetSafeAttributeStr(xmlItem, "Name");
					config.ActivityNameDict[activityid] = filename;
				}
				fileName = "Config/ThemeActivityOpen.xml";
				xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null != xml)
				{
					XElement xmlItem = xml.Element("ThemeActivityOpen");
					if (null != xmlItem)
					{
						config.ActivityOpenVavle = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Open"));
					}
				}
				lock (HuodongCachingMgr._ThemeActivityConfigMutex)
				{
					HuodongCachingMgr._ThemeActivityConfig = config;
				}
				return config;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/ThemeActivityType.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetThemeActivityConfig()
		{
			string fileName = "Config/ThemeActivityType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			fileName = "Config/ThemeActivityOpen.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._ThemeActivityConfigMutex)
			{
				HuodongCachingMgr._ThemeActivityConfig = null;
			}
			return 0;
		}

		
		public static JieriActivityConfig GetJieriActivityConfig()
		{
			lock (HuodongCachingMgr._JieriActivityConfigMutex)
			{
				if (HuodongCachingMgr._JieriActivityConfig != null)
				{
					return HuodongCachingMgr._JieriActivityConfig;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/MuJieRiType.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				JieriActivityConfig config = new JieriActivityConfig();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					int activityid = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Type"));
					string filename = Global.GetSafeAttributeStr(xmlItem, "PeiZhi");
					config.ConfigDict[activityid] = filename;
					config.openList.Add(activityid);
					filename = Global.GetSafeAttributeStr(xmlItem, "Name");
					config.ActivityNameDict[activityid] = filename;
				}
				lock (HuodongCachingMgr._JieriActivityConfigMutex)
				{
					HuodongCachingMgr._JieriActivityConfig = config;
				}
				return config;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/JieRiGifts/MuJieRiType.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetJieriActivityConfig()
		{
			string fileName = "Config/JieRiGifts/MuJieRiType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieriActivityConfigMutex)
			{
				HuodongCachingMgr._JieriActivityConfig = null;
			}
			return 0;
		}

		
		public static JieriDaLiBaoActivity GetJieriDaLiBaoActivity()
		{
			lock (HuodongCachingMgr._JieriDaLiBaoActivityMutex)
			{
				if (HuodongCachingMgr._JieriDaLiBaoActivity != null)
				{
					return HuodongCachingMgr._JieriDaLiBaoActivity;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/JieRiLiBao.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				JieriDaLiBaoActivity activity = new JieriDaLiBaoActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				activity.MyAwardItem = new AwardItem();
				args = xml.Element("GiftList");
				if (null != args)
				{
					XElement xmlItem = args.Element("Award");
					if (null != xmlItem)
					{
						activity.MyAwardItem.MinAwardCondionValue = 0;
						activity.MyAwardItem.AwardYuanBao = 0;
						string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日礼包活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日礼包活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								activity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日礼包配置1");
							}
						}
						goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日礼包活动配置文件中的物品配置项2失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日礼包活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								List<GoodsData> GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日礼包配置2");
								foreach (GoodsData item in GoodsDataList)
								{
									SystemXmlItem systemGoods = null;
									if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemGoods))
									{
										int toOccupation = Global.GetMainOccupationByGoodsID(item.GoodsID);
										AwardItem myOccAward = activity.GetOccAward(toOccupation);
										if (null == myOccAward)
										{
											myOccAward = new AwardItem();
											myOccAward.GoodsDataList.Add(item);
											activity.OccAwardItemDict[toOccupation] = myOccAward;
										}
										else
										{
											myOccAward.GoodsDataList.Add(item);
										}
									}
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._JieriDaLiBaoActivityMutex)
				{
					HuodongCachingMgr._JieriDaLiBaoActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/JieRiGifts/JieRiLiBao.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetJieriDaLiBaoActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiLiBao.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieriDaLiBaoActivityMutex)
			{
				HuodongCachingMgr._JieriDaLiBaoActivity = null;
			}
			return 0;
		}

		
		public static JieRiDengLuActivity GetJieRiDengLuActivity()
		{
			lock (HuodongCachingMgr._JieriDengLuActivityMutex)
			{
				if (HuodongCachingMgr._JieRiDengLuActivity != null)
				{
					return HuodongCachingMgr._JieRiDengLuActivity;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/JieRiDengLu.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				JieRiDengLuActivity activity = new JieRiDengLuActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							myAwardItem.MinAwardCondionValue = 0;
							myAwardItem.AwardYuanBao = 0;
							int day = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "TimeOl"));
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取节日登录有礼活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析节日登录有礼活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "节日登录有礼配置");
								}
							}
							activity.AwardItemDict[day] = myAwardItem;
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取节日登录有礼活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析节日登录有礼活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									List<GoodsData> GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "节日登录有礼配置2");
									foreach (GoodsData item in GoodsDataList)
									{
										SystemXmlItem systemGoods = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemGoods))
										{
											int toOccupation = Global.GetMainOccupationByGoodsID(item.GoodsID);
											int key = day * 100 + toOccupation;
											AwardItem myOccAward = activity.GetOccAward(key);
											if (null == myOccAward)
											{
												myOccAward = new AwardItem();
												myOccAward.GoodsDataList.Add(item);
												activity.OccAwardItemDict[key] = myOccAward;
											}
											else
											{
												myOccAward.GoodsDataList.Add(item);
											}
										}
									}
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._JieriDengLuActivityMutex)
				{
					HuodongCachingMgr._JieRiDengLuActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/JieRiGifts/JieRiDengLu.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetJieRiDengLuActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiDengLu.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieriDengLuActivityMutex)
			{
				HuodongCachingMgr._JieRiDengLuActivity = null;
			}
			return 0;
		}

		
		public static JieriVIPActivity GetJieriVIPActivity()
		{
			lock (HuodongCachingMgr._JieriVIPActivityMutex)
			{
				if (HuodongCachingMgr._JieriVIPActivity != null)
				{
					return HuodongCachingMgr._JieriVIPActivity;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/JieRiVip.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				JieriVIPActivity activity = new JieriVIPActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				activity.MyAwardItem = new AwardItem();
				args = xml.Element("GiftList");
				if (null != args)
				{
					XElement xmlItem = args.Element("Award");
					if (null != xmlItem)
					{
						activity.MyAwardItem.MinAwardCondionValue = 0;
						activity.MyAwardItem.AwardYuanBao = 0;
						string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日VIP活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日VIP活动配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								activity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日VIP配置");
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._JieriVIPActivityMutex)
				{
					HuodongCachingMgr._JieriVIPActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/JieRiGifts/JieRiVip.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetJieriVIPActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiVip.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieriVIPActivityMutex)
			{
				HuodongCachingMgr._JieriVIPActivity = null;
			}
			return 0;
		}

		
		public static JieriGiveActivity GetJieriGiveActivity()
		{
			lock (HuodongCachingMgr._JieriGiveMutex)
			{
				if (HuodongCachingMgr._JieriGiveActivity != null)
				{
					return HuodongCachingMgr._JieriGiveActivity;
				}
			}
			JieriGiveActivity act = new JieriGiveActivity();
			if (act.Init())
			{
				lock (HuodongCachingMgr._JieriGiveMutex)
				{
					HuodongCachingMgr._JieriGiveActivity = act;
					return act;
				}
			}
			return null;
		}

		
		public static int ResetJieriGiveActivity()
		{
			lock (HuodongCachingMgr._JieriGiveMutex)
			{
				HuodongCachingMgr._JieriGiveActivity = null;
			}
			return 0;
		}

		
		public static JieriRecvActivity GetJieriRecvActivity()
		{
			lock (HuodongCachingMgr._JieriRecvMutex)
			{
				if (HuodongCachingMgr._JieriRecvActivity != null)
				{
					return HuodongCachingMgr._JieriRecvActivity;
				}
			}
			JieriRecvActivity act = new JieriRecvActivity();
			if (act.Init())
			{
				lock (HuodongCachingMgr._JieriRecvMutex)
				{
					HuodongCachingMgr._JieriRecvActivity = act;
					return act;
				}
			}
			return null;
		}

		
		public static int ResetJieriRecvActivity()
		{
			lock (HuodongCachingMgr._JieriRecvMutex)
			{
				HuodongCachingMgr._JieriRecvActivity = null;
			}
			return 0;
		}

		
		public static JieRiGiveKingActivity GetJieriGiveKingActivity()
		{
			lock (HuodongCachingMgr._JieriGiveKingMutex)
			{
				if (HuodongCachingMgr._JieriGiveKingActivity != null)
				{
					return HuodongCachingMgr._JieriGiveKingActivity;
				}
			}
			JieRiGiveKingActivity act = new JieRiGiveKingActivity();
			if (act.Init())
			{
				act.LoadRankFromDB();
				lock (HuodongCachingMgr._JieriGiveKingMutex)
				{
					HuodongCachingMgr._JieriGiveKingActivity = act;
					return act;
				}
			}
			return null;
		}

		
		public static int ResetJieRiGiveKingActivity()
		{
			lock (HuodongCachingMgr._JieriGiveKingMutex)
			{
				HuodongCachingMgr._JieriGiveKingActivity = null;
			}
			return 0;
		}

		
		public static JieRiRecvKingActivity GetJieriRecvKingActivity()
		{
			lock (HuodongCachingMgr._JieriRecvKingMutex)
			{
				if (HuodongCachingMgr._JieriRecvKingActivity != null)
				{
					return HuodongCachingMgr._JieriRecvKingActivity;
				}
			}
			JieRiRecvKingActivity act = new JieRiRecvKingActivity();
			if (act.Init())
			{
				act.LoadRankFromDB();
				lock (HuodongCachingMgr._JieriRecvKingMutex)
				{
					HuodongCachingMgr._JieriRecvKingActivity = act;
					return act;
				}
			}
			return null;
		}

		
		public static int ResetJieriRecvKingActivity()
		{
			lock (HuodongCachingMgr._JieriRecvKingMutex)
			{
				HuodongCachingMgr._JieriRecvKingActivity = null;
			}
			return 0;
		}

		
		public static JieRiFuLiActivity GetJieriFuLiActivity()
		{
			lock (HuodongCachingMgr._JieriFuLiMutex)
			{
				if (HuodongCachingMgr._JieriFuLiActivity != null)
				{
					return HuodongCachingMgr._JieriFuLiActivity;
				}
			}
			JieRiFuLiActivity act = new JieRiFuLiActivity();
			if (act.Init())
			{
				lock (HuodongCachingMgr._JieriFuLiMutex)
				{
					HuodongCachingMgr._JieriFuLiActivity = act;
					return act;
				}
			}
			return null;
		}

		
		public static int ResetJieriFuLiActivity()
		{
			lock (HuodongCachingMgr._JieriFuLiMutex)
			{
				HuodongCachingMgr._JieriFuLiActivity = null;
			}
			return 0;
		}

		
		public static int ResetOneDollarChongZhiActivity()
		{
			lock (HuodongCachingMgr._OneDollarChongZhiMutex)
			{
				HuodongCachingMgr._OneDollarChongZhi = null;
			}
			GameManager.ClientMgr.NotifyAllOneDollarChongZhiState();
			return 0;
		}

		
		public static OneDollarChongZhi GetOneDollarChongZhiActivity()
		{
			lock (HuodongCachingMgr._OneDollarChongZhiMutex)
			{
				if (HuodongCachingMgr._OneDollarChongZhi != null)
				{
					return HuodongCachingMgr._OneDollarChongZhi;
				}
				OneDollarChongZhi act = new OneDollarChongZhi();
				if (act.Init())
				{
					HuodongCachingMgr._OneDollarChongZhi = act;
					return HuodongCachingMgr._OneDollarChongZhi;
				}
			}
			return null;
		}

		
		public static int ResetInputFanLiNewActivity()
		{
			lock (HuodongCachingMgr._InputFanLiNewMutex)
			{
				HuodongCachingMgr._InputFanLiNew = null;
			}
			GameManager.ClientMgr.NotifyAllInputFanLiNewState();
			return 0;
		}

		
		public static InputFanLiNew GetInputFanLiNewActivity()
		{
			lock (HuodongCachingMgr._InputFanLiNewMutex)
			{
				if (HuodongCachingMgr._InputFanLiNew != null)
				{
					return HuodongCachingMgr._InputFanLiNew;
				}
				InputFanLiNew act = new InputFanLiNew();
				if (act.Init())
				{
					HuodongCachingMgr._InputFanLiNew = act;
					return HuodongCachingMgr._InputFanLiNew;
				}
			}
			return null;
		}

		
		public static int ResetRegressActiveOpen()
		{
			lock (HuodongCachingMgr._RegressActiveOpenMutex)
			{
				HuodongCachingMgr._RegressActiveOpen = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveOpenState();
			return 0;
		}

		
		public static RegressActiveOpen GetRegressActiveOpen()
		{
			lock (HuodongCachingMgr._RegressActiveOpenMutex)
			{
				if (HuodongCachingMgr._RegressActiveOpen != null)
				{
					return HuodongCachingMgr._RegressActiveOpen;
				}
				RegressActiveOpen act = new RegressActiveOpen();
				if (act.Init())
				{
					HuodongCachingMgr._RegressActiveOpen = act;
					return HuodongCachingMgr._RegressActiveOpen;
				}
			}
			return null;
		}

		
		public static int ResetRegressActiveSignGift()
		{
			lock (HuodongCachingMgr._RegressActiveSignGiftMutex)
			{
				HuodongCachingMgr._RegressActiveSignGift = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveSignGiftState();
			return 0;
		}

		
		public static RegressActiveSignGift GetRegressActiveSignGift()
		{
			lock (HuodongCachingMgr._RegressActiveSignGiftMutex)
			{
				if (HuodongCachingMgr._RegressActiveSignGift != null)
				{
					return HuodongCachingMgr._RegressActiveSignGift;
				}
				RegressActiveSignGift act = new RegressActiveSignGift();
				if (act.Init())
				{
					HuodongCachingMgr._RegressActiveSignGift = act;
					return HuodongCachingMgr._RegressActiveSignGift;
				}
			}
			return null;
		}

		
		public static int ResetRegressActiveTotalRecharge()
		{
			lock (HuodongCachingMgr._RegressActiveTotalRechargeMutex)
			{
				HuodongCachingMgr._RegressActiveTotalRecharge = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveTotalRechargeState();
			return 0;
		}

		
		public static RegressActiveTotalRecharge GetRegressActiveTotalRecharge()
		{
			lock (HuodongCachingMgr._RegressActiveTotalRechargeMutex)
			{
				if (HuodongCachingMgr._RegressActiveTotalRecharge != null)
				{
					return HuodongCachingMgr._RegressActiveTotalRecharge;
				}
				RegressActiveTotalRecharge act = new RegressActiveTotalRecharge();
				if (act.Init())
				{
					HuodongCachingMgr._RegressActiveTotalRecharge = act;
					return HuodongCachingMgr._RegressActiveTotalRecharge;
				}
			}
			return null;
		}

		
		public static int ResetRegressActiveDayBuy()
		{
			lock (HuodongCachingMgr._RegressActiveDayBuyMutex)
			{
				HuodongCachingMgr._RegressActiveDayBuy = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveDayBuyState();
			return 0;
		}

		
		public static RegressActiveDayBuy GetRegressActiveDayBuy()
		{
			lock (HuodongCachingMgr._RegressActiveDayBuyMutex)
			{
				if (HuodongCachingMgr._RegressActiveDayBuy != null)
				{
					return HuodongCachingMgr._RegressActiveDayBuy;
				}
				RegressActiveDayBuy act = new RegressActiveDayBuy();
				if (act.Init())
				{
					HuodongCachingMgr._RegressActiveDayBuy = act;
					return HuodongCachingMgr._RegressActiveDayBuy;
				}
			}
			return null;
		}

		
		public static int ResetRegressActiveStore()
		{
			lock (HuodongCachingMgr._RegressActiveStoreMutex)
			{
				HuodongCachingMgr._RegressActiveStore = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveDayBuyState();
			return 0;
		}

		
		public static RegressActiveStore GetRegressActiveStore()
		{
			lock (HuodongCachingMgr._RegressActiveStoreMutex)
			{
				if (HuodongCachingMgr._RegressActiveStore != null)
				{
					return HuodongCachingMgr._RegressActiveStore;
				}
				RegressActiveStore act = new RegressActiveStore();
				if (act.Init())
				{
					HuodongCachingMgr._RegressActiveStore = act;
					return HuodongCachingMgr._RegressActiveStore;
				}
			}
			return null;
		}

		
		public static int ResetJieRiSuperInputFanLiActivity()
		{
			lock (HuodongCachingMgr._JieriSuperInputMutex)
			{
				HuodongCachingMgr._JieriSuperInput = null;
			}
			HuodongCachingMgr.GetJieRiSuperInputActivity();
			return 0;
		}

		
		public static JieriSuperInputActivity GetJieRiSuperInputActivity()
		{
			lock (HuodongCachingMgr._JieriSuperInputMutex)
			{
				if (HuodongCachingMgr._JieriSuperInput != null)
				{
					return HuodongCachingMgr._JieriSuperInput;
				}
				JieriSuperInputActivity act = new JieriSuperInputActivity();
				if (act.Init())
				{
					HuodongCachingMgr._JieriSuperInput = act;
					return HuodongCachingMgr._JieriSuperInput;
				}
			}
			return null;
		}

		
		public static int ResetOneDollarBuyActivity()
		{
			lock (HuodongCachingMgr._OneDollarBuyActivityMutex)
			{
				if (null != HuodongCachingMgr._OneDollarBuyActivity)
				{
					HuodongCachingMgr._OneDollarBuyActivity.Dispose();
				}
				HuodongCachingMgr._OneDollarBuyActivity = null;
			}
			HuodongCachingMgr.GetOneDollarBuyActivity();
			return 0;
		}

		
		public static OneDollarBuyActivity GetOneDollarBuyActivity()
		{
			lock (HuodongCachingMgr._OneDollarBuyActivityMutex)
			{
				if (HuodongCachingMgr._OneDollarBuyActivity != null)
				{
					return HuodongCachingMgr._OneDollarBuyActivity;
				}
			}
			OneDollarBuyActivity act = new OneDollarBuyActivity();
			if (act.Init())
			{
				lock (HuodongCachingMgr._OneDollarBuyActivityMutex)
				{
					HuodongCachingMgr._OneDollarBuyActivity = act;
					return HuodongCachingMgr._OneDollarBuyActivity;
				}
			}
			return null;
		}

		
		public static int ResetJieRiCZQGActivity()
		{
			lock (HuodongCachingMgr._JieRiCZQGActivityMutex)
			{
				if (null != HuodongCachingMgr._JieRiCZQGActivity)
				{
					HuodongCachingMgr._JieRiCZQGActivity.Dispose();
				}
				HuodongCachingMgr._JieRiCZQGActivity = null;
			}
			return 0;
		}

		
		public static JieRiCZQGActivity GetJieRiCZQGActivity()
		{
			lock (HuodongCachingMgr._JieRiCZQGActivityMutex)
			{
				if (HuodongCachingMgr._JieRiCZQGActivity != null)
				{
					return HuodongCachingMgr._JieRiCZQGActivity;
				}
			}
			JieRiCZQGActivity act = new JieRiCZQGActivity();
			if (act.Init())
			{
				lock (HuodongCachingMgr._JieRiCZQGActivityMutex)
				{
					HuodongCachingMgr._JieRiCZQGActivity = act;
					return HuodongCachingMgr._JieRiCZQGActivity;
				}
			}
			return null;
		}

		
		public static int ResetJieriVIPYouHuiAct()
		{
			lock (HuodongCachingMgr._JieriVIPYouHuiActMutex)
			{
				HuodongCachingMgr._JieriVIPYouHuiActivity = null;
			}
			return 0;
		}

		
		public static JieriVIPYouHuiActivity GetJieriVIPYouHuiAct()
		{
			lock (HuodongCachingMgr._JieriVIPYouHuiActMutex)
			{
				if (HuodongCachingMgr._JieriVIPYouHuiActivity != null)
				{
					return HuodongCachingMgr._JieriVIPYouHuiActivity;
				}
			}
			JieriVIPYouHuiActivity act = new JieriVIPYouHuiActivity();
			if (act.Init())
			{
				lock (HuodongCachingMgr._JieriVIPYouHuiActMutex)
				{
					HuodongCachingMgr._JieriVIPYouHuiActivity = act;
					return HuodongCachingMgr._JieriVIPYouHuiActivity;
				}
			}
			return null;
		}

		
		public static int ResetSpecialActivity()
		{
			lock (HuodongCachingMgr._SpecialActivityMutex)
			{
				if (null != HuodongCachingMgr._SpecialActivity)
				{
					HuodongCachingMgr._SpecialActivity.Dispose();
				}
				HuodongCachingMgr._SpecialActivity = null;
			}
			GameManager.ClientMgr.ReGenerateSpecActGroup();
			return 0;
		}

		
		public static SpecialActivity GetSpecialActivity()
		{
			lock (HuodongCachingMgr._SpecialActivityMutex)
			{
				if (HuodongCachingMgr._SpecialActivity != null)
				{
					return HuodongCachingMgr._SpecialActivity;
				}
			}
			SpecialActivity act = new SpecialActivity();
			if (act.Init())
			{
				lock (HuodongCachingMgr._SpecialActivityMutex)
				{
					HuodongCachingMgr._SpecialActivity = act;
					return HuodongCachingMgr._SpecialActivity;
				}
			}
			return null;
		}

		
		public static int ResetSpecPriorityActivity()
		{
			lock (HuodongCachingMgr._SpecPriorityActivityMutex)
			{
				if (null != HuodongCachingMgr._SpecPriorityActivity)
				{
					HuodongCachingMgr._SpecPriorityActivity.Dispose();
				}
				HuodongCachingMgr._SpecPriorityActivity = null;
			}
			GameManager.ClientMgr.ReGenerateSpecPriorityActGroup();
			return 0;
		}

		
		public static SpecPriorityActivity GetSpecPriorityActivity()
		{
			lock (HuodongCachingMgr._SpecPriorityActivityMutex)
			{
				if (HuodongCachingMgr._SpecPriorityActivity != null)
				{
					return HuodongCachingMgr._SpecPriorityActivity;
				}
				SpecPriorityActivity act = new SpecPriorityActivity();
				if (act.Init())
				{
					HuodongCachingMgr._SpecPriorityActivity = act;
					return HuodongCachingMgr._SpecPriorityActivity;
				}
			}
			return null;
		}

		
		public static int ResetThemeDaLiBaoActivity()
		{
			lock (HuodongCachingMgr._ThemeDaLiBaoActivityMutex)
			{
				HuodongCachingMgr._ThemeDaLiBaoActivity = null;
			}
			return 0;
		}

		
		public static ThemeDaLiBaoActivity GetThemeDaLiBaoActivity()
		{
			lock (HuodongCachingMgr._ThemeDaLiBaoActivityMutex)
			{
				if (HuodongCachingMgr._ThemeDaLiBaoActivity != null)
				{
					return HuodongCachingMgr._ThemeDaLiBaoActivity;
				}
				ThemeDaLiBaoActivity act = new ThemeDaLiBaoActivity();
				if (act.Init())
				{
					HuodongCachingMgr._ThemeDaLiBaoActivity = act;
					return HuodongCachingMgr._ThemeDaLiBaoActivity;
				}
			}
			return null;
		}

		
		public static int ResetThemeDuiHuanActivity()
		{
			lock (HuodongCachingMgr._ThemeDuiHuanActivityMutex)
			{
				HuodongCachingMgr._ThemeDuiHuanActivity = null;
			}
			return 0;
		}

		
		public static ThemeDuiHuanActivity GetThemeDuiHuanActivity()
		{
			lock (HuodongCachingMgr._ThemeDuiHuanActivityMutex)
			{
				if (HuodongCachingMgr._ThemeDuiHuanActivity != null)
				{
					return HuodongCachingMgr._ThemeDuiHuanActivity;
				}
				ThemeDuiHuanActivity act = new ThemeDuiHuanActivity();
				if (act.Init())
				{
					HuodongCachingMgr._ThemeDuiHuanActivity = act;
					return HuodongCachingMgr._ThemeDuiHuanActivity;
				}
			}
			return null;
		}

		
		public static int ResetThemeZhiGouActivity()
		{
			lock (HuodongCachingMgr._ThemeZhiGouActivityMutex)
			{
				if (null != HuodongCachingMgr._ThemeZhiGouActivity)
				{
					HuodongCachingMgr._ThemeZhiGouActivity.Dispose();
				}
				HuodongCachingMgr._ThemeZhiGouActivity = null;
			}
			return 0;
		}

		
		public static ThemeZhiGouActivity GetThemeZhiGouActivity()
		{
			lock (HuodongCachingMgr._ThemeZhiGouActivityMutex)
			{
				if (HuodongCachingMgr._ThemeZhiGouActivity != null)
				{
					return HuodongCachingMgr._ThemeZhiGouActivity;
				}
				ThemeZhiGouActivity act = new ThemeZhiGouActivity();
				if (act.Init())
				{
					HuodongCachingMgr._ThemeZhiGouActivity = act;
					return HuodongCachingMgr._ThemeZhiGouActivity;
				}
			}
			return null;
		}

		
		public static int ResetEverydayActivity()
		{
			lock (HuodongCachingMgr._EverydayActivityMutex)
			{
				if (null != HuodongCachingMgr._EverydayActivity)
				{
					HuodongCachingMgr._EverydayActivity.Dispose();
				}
				HuodongCachingMgr._EverydayActivity = null;
			}
			GameManager.ClientMgr.ReGenerateEverydayActGroup();
			return 0;
		}

		
		public static EverydayActivity GetEverydayActivity()
		{
			lock (HuodongCachingMgr._EverydayActivityMutex)
			{
				if (HuodongCachingMgr._EverydayActivity != null)
				{
					return HuodongCachingMgr._EverydayActivity;
				}
				EverydayActivity act = new EverydayActivity();
				if (act.Init())
				{
					HuodongCachingMgr._EverydayActivity = act;
					return HuodongCachingMgr._EverydayActivity;
				}
			}
			return null;
		}

		
		public static int ResetWeedEndInputActivity()
		{
			string fileName = "Config/Gifts/ZhouMoChongZhiType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			fileName = "Config/Gifts/ZhouMoChongZhi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._WeedEndInputActivityMutex)
			{
				HuodongCachingMgr._WeedEndInputActivity = null;
			}
			return 0;
		}

		
		public static WeedEndInputActivity GetWeekEndInputActivity()
		{
			lock (HuodongCachingMgr._WeedEndInputActivityMutex)
			{
				if (HuodongCachingMgr._WeedEndInputActivity != null)
				{
					return HuodongCachingMgr._WeedEndInputActivity;
				}
			}
			WeedEndInputActivity act = new WeedEndInputActivity();
			if (act.Init())
			{
				lock (HuodongCachingMgr._WeedEndInputActivityMutex)
				{
					HuodongCachingMgr._WeedEndInputActivity = act;
					return HuodongCachingMgr._WeedEndInputActivity;
				}
			}
			return null;
		}

		
		public static int ResetJieriIPointsExchangeActivity()
		{
			string fileName = "Config/JieRiGifts/ChongZhiDuiHuan.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieriIPointsExchgActivityMutex)
			{
				HuodongCachingMgr._JieriIPointsExchgActivity = null;
			}
			return 0;
		}

		
		public static JieriIPointsExchgActivity GetJieriIPointsExchgActivity()
		{
			lock (HuodongCachingMgr._JieriIPointsExchgActivityMutex)
			{
				if (HuodongCachingMgr._JieriIPointsExchgActivity != null)
				{
					return HuodongCachingMgr._JieriIPointsExchgActivity;
				}
			}
			JieriIPointsExchgActivity act = new JieriIPointsExchgActivity();
			if (act.Init())
			{
				lock (HuodongCachingMgr._JieriIPointsExchgActivityMutex)
				{
					HuodongCachingMgr._JieriIPointsExchgActivity = act;
					return HuodongCachingMgr._JieriIPointsExchgActivity;
				}
			}
			return null;
		}

		
		public static JieriCZSongActivity GetJieriCZSongActivity()
		{
			lock (HuodongCachingMgr._JieriCZSongActivityMutex)
			{
				if (HuodongCachingMgr._JieriCZSongActivity != null)
				{
					return HuodongCachingMgr._JieriCZSongActivity;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/JieRiDayChongZhi.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				JieriCZSongActivity activity = new JieriCZSongActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							int id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							myAwardItem.AwardYuanBao = 0;
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日充值送活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日充值送活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日充值送配置1");
								}
							}
							activity.AwardItemDict[id] = myAwardItem;
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日充值送活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日充值送活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									List<GoodsData> GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日充值送配置2");
									foreach (GoodsData item in GoodsDataList)
									{
										SystemXmlItem systemGoods = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemGoods))
										{
											int key = id;
											AwardItem myOccAward = activity.GetOccAward(key);
											if (null == myOccAward)
											{
												myOccAward = new AwardItem();
												myOccAward.GoodsDataList.Add(item);
												myOccAward.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
												activity.OccAwardItemDict[key] = myOccAward;
											}
											else
											{
												myOccAward.GoodsDataList.Add(item);
											}
										}
									}
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._JieriCZSongActivityMutex)
				{
					HuodongCachingMgr._JieriCZSongActivity = activity;
				}
				return activity;
			}
			catch (Exception)
			{
			}
			return null;
		}

		
		public static int ResetJieriCZSongActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiDayChongZhi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieriCZSongActivityMutex)
			{
				HuodongCachingMgr._JieriCZSongActivity = null;
			}
			return 0;
		}

		
		public static JieRiLeiJiCZActivity GetJieRiLeiJiCZActivity()
		{
			lock (HuodongCachingMgr._JieRiLeiJiCZActivityMutex)
			{
				if (HuodongCachingMgr._JieRiLeiJiCZActivity != null)
				{
					return HuodongCachingMgr._JieRiLeiJiCZActivity;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/JieRiLeiJi.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				JieRiLeiJiCZActivity activity = new JieRiLeiJiCZActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							int id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							myAwardItem.AwardYuanBao = 0;
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取节日累计充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析节日累计充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "节日累计充值配置1");
								}
							}
							activity.AwardItemDict[id] = myAwardItem;
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (!string.IsNullOrEmpty(goodsIDs))
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析节日累计充值活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									List<GoodsData> GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "节日累计充值配置2");
									foreach (GoodsData item in GoodsDataList)
									{
										SystemXmlItem systemGoods = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemGoods))
										{
											int key = id;
											AwardItem myOccAward = activity.GetOccAward(key);
											if (null == myOccAward)
											{
												myOccAward = new AwardItem();
												myOccAward.GoodsDataList.Add(item);
												myOccAward.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
												activity.OccAwardItemDict[key] = myOccAward;
											}
											else
											{
												myOccAward.GoodsDataList.Add(item);
											}
										}
									}
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiLeiJiCZActivityMutex)
				{
					HuodongCachingMgr._JieRiLeiJiCZActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/JieRiGifts/JieRiLeiJi.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetJieRiLeiJiCZActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiLeiJi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieRiLeiJiCZActivityMutex)
			{
				HuodongCachingMgr._JieRiLeiJiCZActivity = null;
			}
			return 0;
		}

		
		public static JieRiMeiRiLeiJiActivity GetJieriMeiRiLeiJiActivity()
		{
			lock (HuodongCachingMgr._JieRiMeiRiLeiJiActivityMutex)
			{
				if (HuodongCachingMgr._JieRiMeiRiLeiJiActivity != null)
				{
					return HuodongCachingMgr._JieRiMeiRiLeiJiActivity;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/JieRiMeiRiLeiJi.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				JieRiMeiRiLeiJiActivity activity = new JieRiMeiRiLeiJiActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							int day = (int)Global.GetSafeAttributeLong(xmlItem, "Day");
							int id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							myAwardItem.AwardYuanBao = 0;
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日充值送活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日充值送活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日充值送配置1");
								}
							}
							if (!activity.DayAwardItemDict.ContainsKey(day))
							{
								activity.DayAwardItemDict[day] = new List<AwardItem>();
							}
							activity.DayAwardItemDict[day].Add(myAwardItem);
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日充值送活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日充值送活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									List<GoodsData> GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日充值送配置2");
									foreach (GoodsData item in GoodsDataList)
									{
										SystemXmlItem systemGoods = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemGoods))
										{
											int key = id;
											AwardItem myOccAward = activity.GetOccAward(key);
											if (null == myOccAward)
											{
												myOccAward = new AwardItem();
												myOccAward.GoodsDataList.Add(item);
												myOccAward.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
												if (!activity.DayOccAwardItemDict.ContainsKey(day))
												{
													activity.DayOccAwardItemDict[day] = new Dictionary<int, AwardItem>();
												}
												activity.DayOccAwardItemDict[day][key] = myOccAward;
											}
											else
											{
												myOccAward.GoodsDataList.Add(item);
											}
										}
									}
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiMeiRiLeiJiActivityMutex)
				{
					HuodongCachingMgr._JieRiMeiRiLeiJiActivity = activity;
				}
				return activity;
			}
			catch (Exception)
			{
			}
			return null;
		}

		
		public static int ResetJieRiMeiRiLeiJiActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiMeiRiLeiJi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieRiMeiRiLeiJiActivityMutex)
			{
				HuodongCachingMgr._JieRiMeiRiLeiJiActivity = null;
			}
			return 0;
		}

		
		public static JieRiTotalConsumeActivity GetJieRiTotalConsumeActivity()
		{
			lock (HuodongCachingMgr._JieRiTotalConsumeActivityMutex)
			{
				if (HuodongCachingMgr._JieRiTotalConsumeActivity != null)
				{
					return HuodongCachingMgr._JieRiTotalConsumeActivity;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/JieRiLeiJiXiaoFei.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				JieRiTotalConsumeActivity activity = new JieRiTotalConsumeActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							int id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							myAwardItem.AwardYuanBao = 0;
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取节日累计消费活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析节日累计消费活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "节日累计消费配置1");
								}
							}
							activity.AwardItemDict[id] = myAwardItem;
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (!string.IsNullOrEmpty(goodsIDs))
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析节日累计消费活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									List<GoodsData> GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "节日累计消费配置2");
									foreach (GoodsData item in GoodsDataList)
									{
										SystemXmlItem systemGoods = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemGoods))
										{
											int key = id;
											AwardItem myOccAward = activity.GetOccAward(key);
											if (null == myOccAward)
											{
												myOccAward = new AwardItem();
												myOccAward.GoodsDataList.Add(item);
												myOccAward.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
												activity.OccAwardItemDict[key] = myOccAward;
											}
											else
											{
												myOccAward.GoodsDataList.Add(item);
											}
										}
									}
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiTotalConsumeActivityMutex)
				{
					HuodongCachingMgr._JieRiTotalConsumeActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/JieRiGifts/JieRiLeiJiXiaoFei.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetJieRiTotalConsumeActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiLeiJiXiaoFei.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieRiTotalConsumeActivityMutex)
			{
				HuodongCachingMgr._JieRiTotalConsumeActivity = null;
			}
			return 0;
		}

		
		public static JieRiMultAwardActivity GetJieRiMultAwardActivity()
		{
			lock (HuodongCachingMgr._JieRiMultAwardActivityMutex)
			{
				if (HuodongCachingMgr._JieRiMultAwardActivity != null)
				{
					return HuodongCachingMgr._JieRiMultAwardActivity;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/JieRiDuoBei.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				JieRiMultAwardActivity activity = new JieRiMultAwardActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							JieRiMultConfig config = new JieRiMultConfig();
							config.index = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							config.type = (int)Global.GetSafeAttributeLong(xmlItem, "TypeID");
							config.Multiplying = Global.GetSafeAttributeDouble(xmlItem, "Multiplying");
							config.Effective = (int)Global.GetSafeAttributeLong(xmlItem, "Effective");
							config.StartDate = Global.GetSafeAttributeStr(xmlItem, "AwardStartDate");
							config.EndDate = Global.GetSafeAttributeStr(xmlItem, "AwardEndDate");
							activity.activityDict[config.type] = config;
						}
					}
				}
				lock (HuodongCachingMgr._JieRiMultAwardActivityMutex)
				{
					HuodongCachingMgr._JieRiMultAwardActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/JieRiGifts/JieRiDuoBei.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetJieRiMultAwardActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiDuoBei.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieRiMultAwardActivityMutex)
			{
				HuodongCachingMgr._JieRiMultAwardActivity = null;
			}
			return 0;
		}

		
		public static int ResetJieRiFanLiAwardActivity()
		{
			string fileName = "Config/JieRiGifts/WingFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			fileName = "Config/JieRiGifts/ZhuiJiaFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			fileName = "Config/JieRiGifts/QiangHuaFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			fileName = "Config/JieRiGifts/ChengJiuFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			fileName = "Config/JieRiGifts/JunXianFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			fileName = "Config/JieRiGifts/VIPFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			fileName = "Config/JieRiGifts/HuShenFuFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			fileName = "Config/JieRiGifts/DaTianShiFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			fileName = "Config/JieRiGifts/HunYinFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			fileName = "Config/JieRiGifts/JieRiHuiJiFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			fileName = "Config/JieRiGifts/JieRiFuWenFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieriWingFanliActMutex)
			{
				for (int i = 0; i < HuodongCachingMgr._JieriWingFanliAct.Length; i++)
				{
					HuodongCachingMgr._JieriWingFanliAct[i] = null;
				}
			}
			return 0;
		}

		
		public static JieRiZiKaLiaBaoActivity GetJieRiZiKaLiaBaoActivity()
		{
			lock (HuodongCachingMgr._JieRiZiKaLiaBaoActivityMutex)
			{
				if (HuodongCachingMgr._JieRiZiKaLiaBaoActivity != null)
				{
					return HuodongCachingMgr._JieRiZiKaLiaBaoActivity;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/JieRiBaoXiang.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				JieRiZiKaLiaBaoActivity activity = new JieRiZiKaLiaBaoActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							JieRiZiKa config = new JieRiZiKa();
							config.id = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							config.type = (int)Global.GetSafeAttributeLong(xmlItem, "Type");
							config.NeedMoJing = (int)Global.GetSafeAttributeLong(xmlItem, "MoJing");
							config.NeedQiFuJiFen = (int)Global.GetSafeAttributeLong(xmlItem, "JiFen");
							config.DayMaxTimes = (int)Global.GetSafeAttributeLong(xmlItem, "DayMaxTimes");
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "DuiHuanGoodsIDs");
							if (!string.IsNullOrEmpty(goodsIDs))
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日字卡换礼盒活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									config.NeedGoodsList = HuodongCachingMgr.ParseGoodsDataList2(fields, "大型节日字卡换礼盒配置1");
								}
							}
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "NewGoodsID");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日字卡换礼盒活动配置文件中的合成物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日字卡换礼盒活动配置文件中的合成物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									config.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日字卡换礼盒合成配置2");
								}
							}
							activity.JieRiZiKaDict[config.id] = config;
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiZiKaLiaBaoActivityMutex)
				{
					HuodongCachingMgr._JieRiZiKaLiaBaoActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/JieRiGifts/JieRiBaoXiang.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetJieRiZiKaLiaBaoActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiBaoXiang.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieRiZiKaLiaBaoActivityMutex)
			{
				HuodongCachingMgr._JieRiZiKaLiaBaoActivity = null;
			}
			return 0;
		}

		
		public static KingActivity GetJieriXiaoFeiKingActivity()
		{
			lock (HuodongCachingMgr._JieRiXiaoFeiKingActivityMutex)
			{
				if (HuodongCachingMgr._JieRiXiaoFeiKingActivity != null)
				{
					return HuodongCachingMgr._JieRiXiaoFeiKingActivity;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/JieRiXiaoFeiKing.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				KingActivity activity = new KingActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							AwardItem myAwardItem2 = new AwardItem();
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							myAwardItem.AwardYuanBao = 0;
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日消费王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日消费王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日消费王活动配置");
								}
							}
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (!string.IsNullOrEmpty(goodsIDs))
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日消费王活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日消费王活动配置");
								}
							}
							string rankings = Global.GetSafeAttributeStr(xmlItem, "ID");
							string[] paiHangs = rankings.Split(new char[]
							{
								'-'
							});
							if (paiHangs.Length > 0)
							{
								int min = Global.SafeConvertToInt32(paiHangs[0]);
								int max = Global.SafeConvertToInt32(paiHangs[paiHangs.Length - 1]);
								for (int paiHang = min; paiHang <= max; paiHang++)
								{
									activity.AwardDict.Add(paiHang, myAwardItem);
									activity.AwardDict2.Add(paiHang, myAwardItem2);
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiXiaoFeiKingActivityMutex)
				{
					HuodongCachingMgr._JieRiXiaoFeiKingActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/JieRiGifts/JieRiXiaoFeiKing.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetJieRiXiaoFeiKingActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiXiaoFeiKing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieRiXiaoFeiKingActivityMutex)
			{
				HuodongCachingMgr._JieRiXiaoFeiKingActivity = null;
			}
			return 0;
		}

		
		public static KingActivity GetJieRiCZKingActivity()
		{
			lock (HuodongCachingMgr._JieRiCZKingActivityMutex)
			{
				if (HuodongCachingMgr._JieRiCZKingActivity != null)
				{
					return HuodongCachingMgr._JieRiCZKingActivity;
				}
			}
			try
			{
				string fileName = "Config/JieRiGifts/JieRiChongZhiKing.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				KingActivity activity = new KingActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					activity.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							int rank = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							myAwardItem.AwardYuanBao = 0;
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日充值王活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日充值王活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日充值王活动配置1");
								}
							}
							activity.AwardDict.Add(rank, myAwardItem);
							AwardItem myOccAwardItem = new AwardItem();
							myOccAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							myOccAwardItem.AwardYuanBao = 0;
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型节日充值王活动配置文件中的物品配置项2失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型节日充值王活动配置文件中的物品配置项2失败", new object[0]), null, true);
								}
								else
								{
									myOccAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型节日充值王活动配置2");
								}
							}
							activity.AwardDict2.Add(rank, myOccAwardItem);
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._JieRiCZKingActivityMutex)
				{
					HuodongCachingMgr._JieRiCZKingActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/JieRiGifts/JieRiChongZhiKing.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetJieRiCZKingActivity()
		{
			string fileName = "Config/JieRiGifts/JieRiChongZhiKing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._JieRiCZKingActivityMutex)
			{
				HuodongCachingMgr._JieRiCZKingActivity = null;
			}
			return 0;
		}

		
		public static bool LoadHeFuActivitiesConfig()
		{
			string strError = "";
			Activity instance = HuodongCachingMgr.GetHeFuLoginActivity();
			if (instance == null || instance.GetParamsValidateCode() < 0)
			{
				strError = "合服大礼包活动配置项出错";
			}
			else
			{
				instance = HuodongCachingMgr.GetHeFuTotalLoginActivity();
				if (instance == null || instance.GetParamsValidateCode() < 0)
				{
					strError = "合服累计登陆活动配置项出错";
				}
				else
				{
					instance = HuodongCachingMgr.GetHeFuPKKingActivity();
					if (instance == null || instance.GetParamsValidateCode() < 0)
					{
						strError = "合服PK王活动配置项出错";
					}
					else
					{
						instance = HuodongCachingMgr.GetHeFuWCKingActivity();
						if (instance == null || instance.GetParamsValidateCode() < 0)
						{
							strError = "合服王城霸主活动配置项出错";
						}
						else
						{
							instance = HuodongCachingMgr.GetHeFuRechargeActivity();
							if (instance == null || instance.GetParamsValidateCode() < 0)
							{
								strError = "合服充值返利活动配置项出错";
							}
							else
							{
								instance = HuodongCachingMgr.GetXinFanLiActivity();
								if (instance == null || instance.GetParamsValidateCode() < 0)
								{
									strError = "新的新区返利活动配置项出错";
								}
								else
								{
									instance = HuodongCachingMgr.GetHeFuLuoLanActivity();
									if (null == instance)
									{
										strError = "合服罗兰城主活动配置项出错";
									}
								}
							}
						}
					}
				}
			}
			bool result;
			if (!string.IsNullOrEmpty(strError))
			{
				LogManager.WriteLog(LogTypes.Fatal, strError, null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		
		public static HeFuActivityConfig GetHeFuActivityConfing()
		{
			lock (HuodongCachingMgr._HeFuActivityConfigMutex)
			{
				if (HuodongCachingMgr._HeFuActivityConfig != null)
				{
					return HuodongCachingMgr._HeFuActivityConfig;
				}
			}
			try
			{
				string fileName = "Config/HeFuGifts/HeFuType.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				HeFuActivityConfig config = new HeFuActivityConfig();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					int activityid = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
					config.openList.Add(activityid);
				}
				lock (HuodongCachingMgr._HeFuActivityConfigMutex)
				{
					HuodongCachingMgr._HeFuActivityConfig = config;
				}
				return config;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/HeFuGifts/HeFuType.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetHeFuActivityConfig()
		{
			string fileName = "Config/HeFuGifts/HeFuType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._HeFuActivityConfigMutex)
			{
				HuodongCachingMgr._HeFuActivityConfig = null;
			}
			return 0;
		}

		
		public static HeFuLoginActivity GetHeFuLoginActivity()
		{
			lock (HuodongCachingMgr._HeFuLoginActivityMutex)
			{
				if (HuodongCachingMgr._HeFuLoginActivity != null)
				{
					return HuodongCachingMgr._HeFuLoginActivity;
				}
			}
			try
			{
				string fileName = "Config/HeFuGifts/HeFuLiBao.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				HeFuLoginActivity activity = new HeFuLoginActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
				}
				args = xml.Element("Time");
				int ActivityTime = 7;
				int AwardTime = 7;
				if (null != args)
				{
					ActivityTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Activity", ActivityTime.ToString()));
					AwardTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Award", AwardTime.ToString()));
				}
				activity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				activity.ToDate = Global.GetHuoDongTimeByHeFu(ActivityTime - 1, 23, 59, 59);
				activity.AwardStartDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				activity.AwardEndDate = Global.GetHuoDongTimeByHeFu(AwardTime - 1, 23, 59, 59);
				args = xml.Element("GiftList");
				if (null != args)
				{
					XElement xmlItem = args.Element("Award");
					if (null != xmlItem)
					{
						AwardItem NormalAward = new AwardItem();
						string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取Config/HeFuGifts/HeFuLiBao.xml的普通奖励失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析Config/HeFuGifts/HeFuLiBao.xml的普通奖励失败", new object[0]), null, true);
							}
							else
							{
								NormalAward.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型合服礼包配置");
							}
						}
						activity.AwardDict[1] = NormalAward;
						AwardItem VIPAward = new AwardItem();
						string VIPGoodsIDs = Global.GetSafeAttributeStr(xmlItem, "VIPGoodsIDs");
						if (string.IsNullOrEmpty(VIPGoodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取Config/HeFuGifts/HeFuLiBao.xml的VIP奖励失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = VIPGoodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析Config/HeFuGifts/HeFuLiBao.xml的VIP奖励失败", new object[0]), null, true);
							}
							else
							{
								VIPAward.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型合服礼包配置");
							}
						}
						activity.AwardDict[2] = VIPAward;
					}
				}
				lock (HuodongCachingMgr._HeFuLoginActivityMutex)
				{
					HuodongCachingMgr._HeFuLoginActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/HeFuGifts/HeFuLiBao.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetHeFuLoginActivity()
		{
			string fileName = "Config/HeFuGifts/HeFuLiBao.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._HeFuLoginActivityMutex)
			{
				HuodongCachingMgr._HeFuLoginActivity = null;
			}
			return 0;
		}

		
		public static HeFuTotalLoginActivity GetHeFuTotalLoginActivity()
		{
			lock (HuodongCachingMgr._HeFuTotalLoginActivityMutex)
			{
				if (HuodongCachingMgr._HeFuTotalLoginActivity != null)
				{
					return HuodongCachingMgr._HeFuTotalLoginActivity;
				}
			}
			try
			{
				string fileName = "Config/HeFuGifts/HeFuDengLu.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				HeFuTotalLoginActivity activity = new HeFuTotalLoginActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
				}
				args = xml.Element("Time");
				int ActivityTime = 7;
				int AwardTime = 7;
				if (null != args)
				{
					ActivityTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Activity", ActivityTime.ToString()));
					AwardTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Award", AwardTime.ToString()));
				}
				activity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				activity.ToDate = Global.GetHuoDongTimeByHeFu(ActivityTime - 1, 23, 59, 59);
				activity.AwardStartDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				activity.AwardEndDate = Global.GetHuoDongTimeByHeFu(AwardTime - 1, 23, 59, 59);
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						int day = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "TimeOl"));
						AwardItem myAwardItem = new AwardItem();
						string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取合服累计登陆配置文件中的GoodsIDs失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析合服累计登陆配置文件中的GoodsIDs失败", new object[0]), null, true);
							}
							else
							{
								myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "合服累计登陆配置");
							}
						}
						activity.AwardDict[day] = myAwardItem;
					}
				}
				lock (HuodongCachingMgr._HeFuTotalLoginActivityMutex)
				{
					HuodongCachingMgr._HeFuTotalLoginActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/HeFuGifts/HeFuDengLu.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetHeFuTotalLoginActivity()
		{
			string fileName = "Config/HeFuGifts/HeFuDengLu.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._HeFuTotalLoginActivityMutex)
			{
				HuodongCachingMgr._HeFuTotalLoginActivity = null;
			}
			return 0;
		}

		
		public static int GetHeFuPKKingRoleID()
		{
			HeFuPKKingActivity activity = HuodongCachingMgr.GetHeFuPKKingActivity();
			int hefuPKKingRoleID = GameManager.GameConfigMgr.GetGameConfigItemInt("hefupkking", 0);
			int hefuPKKingNum = GameManager.GameConfigMgr.GetGameConfigItemInt("hefupkkingnum", 0);
			int result;
			if (activity != null && !activity.InActivityTime() && !activity.InAwardTime())
			{
				result = 0;
			}
			else if (hefuPKKingRoleID > 0 && hefuPKKingNum >= activity.winerCount)
			{
				result = hefuPKKingRoleID;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public static void UpdateHeFuPKKingRoleID(int roleID)
		{
			HeFuPKKingActivity activity = HuodongCachingMgr.GetHeFuPKKingActivity();
			if (activity == null || activity.InActivityTime())
			{
				int hefuPKKingRoleID = GameManager.GameConfigMgr.GetGameConfigItemInt("hefupkking", 0);
				int hefuPKKingDayID = GameManager.GameConfigMgr.GetGameConfigItemInt("hefupkkingdayid", 0);
				int hefuPKKingNum = GameManager.GameConfigMgr.GetGameConfigItemInt("hefupkkingnum", 0);
				if (0 >= HuodongCachingMgr.GetHeFuPKKingRoleID())
				{
					int CurrDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
					if (roleID != hefuPKKingRoleID || CurrDay != hefuPKKingDayID + 1)
					{
						hefuPKKingRoleID = roleID;
						hefuPKKingDayID = CurrDay;
						hefuPKKingNum = 1;
					}
					else
					{
						hefuPKKingRoleID = roleID;
						hefuPKKingDayID = CurrDay;
						hefuPKKingNum++;
					}
					Global.UpdateDBGameConfigg("hefupkking", hefuPKKingRoleID.ToString());
					Global.UpdateDBGameConfigg("hefupkkingdayid", hefuPKKingDayID.ToString());
					Global.UpdateDBGameConfigg("hefupkkingnum", hefuPKKingNum.ToString());
				}
			}
		}

		
		public static HeFuPKKingActivity GetHeFuPKKingActivity()
		{
			lock (HuodongCachingMgr._HeFuPKKingActivityMutex)
			{
				if (HuodongCachingMgr._HeFuPKKingActivity != null)
				{
					return HuodongCachingMgr._HeFuPKKingActivity;
				}
			}
			try
			{
				string fileName = "Config/HeFuGifts/PKJiangLi.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				HeFuPKKingActivity activity = new HeFuPKKingActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.winerCount = Convert.ToInt32(Global.GetDefAttributeStr(args, "WinerCount", "3"));
				}
				args = xml.Element("Time");
				int ActivityTime = 5;
				int AwardTime = 7;
				if (null != args)
				{
					ActivityTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Activity", ActivityTime.ToString()));
					AwardTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Award", AwardTime.ToString()));
				}
				activity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				activity.ToDate = Global.GetHuoDongTimeByHeFu(ActivityTime - 1, 23, 59, 59);
				activity.AwardStartDate = Global.GetHuoDongTimeByHeFu(activity.winerCount, 0, 0, 0);
				activity.AwardEndDate = Global.GetHuoDongTimeByHeFu(AwardTime - 1, 23, 59, 59);
				args = xml.Element("GiftList");
				if (null != args)
				{
					XElement xmlItem = args.Element("Award");
					if (null != xmlItem)
					{
						string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDOne");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取合服战场之神配置GoodsIDOne失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析合服战场之神配置GoodsIDOne失败", new object[0]), null, true);
							}
							else
							{
								activity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "合服战场之神配置");
							}
						}
					}
				}
				lock (HuodongCachingMgr._HeFuPKKingActivityMutex)
				{
					HuodongCachingMgr._HeFuPKKingActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/HeFuGifts/PKJiangLi.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetHeFuPKKingActivity()
		{
			string fileName = "Config/HeFuGifts/PKJiangLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._HeFuPKKingActivityMutex)
			{
				HuodongCachingMgr._HeFuPKKingActivity = null;
			}
			return 0;
		}

		
		public static HeFuLuoLanActivity GetHeFuLuoLanActivity()
		{
			lock (HuodongCachingMgr._HeFuLuoLanActivityMutex)
			{
				if (HuodongCachingMgr._HeFuLuoLanActivity != null)
				{
					return HuodongCachingMgr._HeFuLuoLanActivity;
				}
			}
			try
			{
				string fileName = "Config/HeFuGifts/HeFuLuoLan.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				HeFuLuoLanActivity activity = new HeFuLuoLanActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
				}
				args = xml.Element("Time");
				int ActivityTime = 7;
				int AwardTime = 7;
				if (null != args)
				{
					ActivityTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Activity", ActivityTime.ToString()));
					AwardTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Award", AwardTime.ToString()));
				}
				activity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				activity.ToDate = Global.GetHuoDongTimeByHeFu(ActivityTime - 1, 23, 59, 59);
				activity.AwardStartDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				activity.AwardEndDate = Global.GetHuoDongTimeByHeFu(AwardTime - 1, 23, 59, 59);
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							HeFuLuoLanAward hefuAward = new HeFuLuoLanAward();
							int ID = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "ID"));
							hefuAward.winNum = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "WinNum"));
							hefuAward.status = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "Status"));
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取合服罗兰城主配置GoodsOne失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("解析合服罗兰城主配置GoodsOne失败", new object[0]), null, true);
								}
								else
								{
									hefuAward.awardData.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "合服罗兰城主配置");
								}
							}
							activity.HeFuLuoLanAwardDict[ID] = hefuAward;
						}
					}
				}
				lock (HuodongCachingMgr._HeFuLuoLanActivityMutex)
				{
					HuodongCachingMgr._HeFuLuoLanActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/HeFuGifts/HeFuLuoLan.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetHeFuLuoLanActivity()
		{
			string fileName = "Config/HeFuGifts/HeFuLuoLan.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._HeFuLuoLanActivityMutex)
			{
				HuodongCachingMgr._HeFuLuoLanActivity = null;
			}
			return 0;
		}

		
		public static HeFuAwardTimesActivity GetHeFuAwardTimesActivity()
		{
			lock (HuodongCachingMgr._HeFuAwardTimeActivityMutex)
			{
				if (HuodongCachingMgr._HeFuAwardTimeActivity != null)
				{
					return HuodongCachingMgr._HeFuAwardTimeActivity;
				}
			}
			try
			{
				string fileName = "Config/HeFuGifts/HeFuZhangChang.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				HeFuAwardTimesActivity activity = new HeFuAwardTimesActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
				}
				args = xml.Element("Time");
				int ActivityTime = 7;
				int AwardTime = 7;
				if (null != args)
				{
					ActivityTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Activity", ActivityTime.ToString()));
					AwardTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Award", AwardTime.ToString()));
				}
				activity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				activity.ToDate = Global.GetHuoDongTimeByHeFu(ActivityTime - 1, 23, 59, 59);
				activity.AwardStartDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				activity.AwardEndDate = Global.GetHuoDongTimeByHeFu(AwardTime - 1, 23, 59, 59);
				args = xml.Element("GiftList");
				if (null != args)
				{
					XElement xmlItem = args.Element("Award");
					if (null != xmlItem)
					{
						string ActivitiesIDs = Global.GetSafeAttributeStr(xmlItem, "ActivitiesIDs");
						if (string.IsNullOrEmpty(ActivitiesIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取合服为战而生配置ActivitiesIDs失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = ActivitiesIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("解析合服战场之神配置GoodsIDOne失败", new object[0]), null, true);
							}
							else
							{
								for (int i = 0; i < fields.Length; i++)
								{
									activity.activityList.Add(Convert.ToInt32(fields[i]));
								}
							}
						}
						activity.activityTimes = (float)Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "Override", "2"));
						activity.specialTimeID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SpecialTimeID", "0"));
					}
				}
				lock (HuodongCachingMgr._HeFuAwardTimeActivityMutex)
				{
					HuodongCachingMgr._HeFuAwardTimeActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/HeFuGifts/HeFuZhangChang.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetHeFuAwardTimeActivity()
		{
			string fileName = "Config/HeFuGifts/HeFuZhangChang.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._HeFuAwardTimeActivityMutex)
			{
				HuodongCachingMgr._HeFuAwardTimeActivity = null;
			}
			return 0;
		}

		
		public static int GetHeFuWCKingBHID()
		{
			HeFuWCKingActivity activity = HuodongCachingMgr.GetHeFuWCKingActivity();
			DateTime startAward = DateTime.Parse(activity.AwardStartDate);
			DateTime endAward = DateTime.Parse(activity.AwardEndDate);
			int result;
			if (TimeUtil.NowDateTime() >= startAward && TimeUtil.NowDateTime() <= endAward)
			{
				int hefuWCKingBHID = GameManager.GameConfigMgr.GetGameConfigItemInt("hefuwcking", 0);
				int hefuWCKingNum = GameManager.GameConfigMgr.GetGameConfigItemInt("hefuwckingnum", 0);
				if (hefuWCKingNum >= 3)
				{
					result = hefuWCKingBHID;
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public static void UpdateHeFuWCKingBHID(int bhid)
		{
			HeFuWCKingActivity activity = HuodongCachingMgr.GetHeFuWCKingActivity();
			DateTime startAward = DateTime.Parse(activity.FromDate);
			DateTime endAward = DateTime.Parse(activity.ToDate);
			if (TimeUtil.NowDateTime() >= startAward && TimeUtil.NowDateTime() <= endAward)
			{
				int hefuWCKingBHID = GameManager.GameConfigMgr.GetGameConfigItemInt("hefuwcking", 0);
				int hefuWCKingDayID = GameManager.GameConfigMgr.GetGameConfigItemInt("hefuwckingdayid", 0);
				int hefuWCKingNum = GameManager.GameConfigMgr.GetGameConfigItemInt("hefuwckingnum", 0);
				if (hefuWCKingNum < 3)
				{
					int dayID = TimeUtil.NowDateTime().DayOfYear;
					if (dayID != hefuWCKingDayID)
					{
						if (hefuWCKingBHID != bhid)
						{
							hefuWCKingBHID = bhid;
							hefuWCKingDayID = dayID;
							hefuWCKingNum = 1;
						}
						else
						{
							if (hefuWCKingDayID == dayID - 1 || (dayID == 1 && hefuWCKingDayID >= 365))
							{
								hefuWCKingNum++;
							}
							else
							{
								hefuWCKingNum = 1;
							}
							hefuWCKingBHID = bhid;
							hefuWCKingDayID = dayID;
						}
						GameManager.GameConfigMgr.UpdateGameConfigItem("hefuwcking", hefuWCKingBHID.ToString(), false);
						GameManager.GameConfigMgr.UpdateGameConfigItem("hefuwckingdayid", hefuWCKingDayID.ToString(), false);
						GameManager.GameConfigMgr.UpdateGameConfigItem("hefuwckingnum", hefuWCKingNum.ToString(), false);
					}
				}
			}
			else
			{
				GameManager.GameConfigMgr.UpdateGameConfigItem("hefuwckingnum", "0", false);
			}
		}

		
		public static HeFuWCKingActivity GetHeFuWCKingActivity()
		{
			lock (HuodongCachingMgr._HeFuWCKingActivityMutex)
			{
				if (HuodongCachingMgr._HeFuWCKingActivity != null)
				{
					return HuodongCachingMgr._HeFuWCKingActivity;
				}
			}
			try
			{
				string fileName = "Config/HeFuGifts/WangChengJiangLi.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				HeFuWCKingActivity activity = new HeFuWCKingActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
					activity.ToDate = Global.GetHuoDongTimeByHeFu(4, 23, 59, 59);
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
					activity.AwardEndDate = Global.GetHuoDongTimeByHeFu(5, 23, 59, 59);
				}
				activity.MyAwardItem = new AwardItem();
				args = xml.Element("GiftList");
				if (null != args)
				{
					XElement xmlItem = args.Element("Award");
					if (null != xmlItem)
					{
						activity.MyAwardItem.MinAwardCondionValue = 0;
						activity.MyAwardItem.AwardYuanBao = 0;
						string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
						if (string.IsNullOrEmpty(goodsIDs))
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型合服王城争霸活动配置文件中的物品配置项1失败", new object[0]), null, true);
						}
						else
						{
							string[] fields = goodsIDs.Split(new char[]
							{
								'|'
							});
							if (fields.Length <= 0)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型合服王城争霸活动配置文件中的物品配置项失败", new object[0]), null, true);
							}
							else
							{
								activity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型合服王城争霸配置");
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._HeFuWCKingActivityMutex)
				{
					HuodongCachingMgr._HeFuWCKingActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/HeFuGifts/WangChengJiangLi.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetHeFuWCKingActivity()
		{
			string fileName = "Config/HeFuGifts/WangChengJiangLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._HeFuWCKingActivityMutex)
			{
				HuodongCachingMgr._HeFuWCKingActivity = null;
			}
			return 0;
		}

		
		public static HeFuRechargeActivity GetHeFuRechargeActivity()
		{
			lock (HuodongCachingMgr._HeFuRechargeActivityMutex)
			{
				if (HuodongCachingMgr._HeFuRechargeActivity != null)
				{
					return HuodongCachingMgr._HeFuRechargeActivity;
				}
			}
			try
			{
				string fileName = "Config/HeFuGifts/HeFuFanLi.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				HeFuRechargeActivity activity = new HeFuRechargeActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
				}
				args = xml.Element("Time");
				int ActivityTime = 7;
				int AwardTime = 7;
				if (null != args)
				{
					ActivityTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Activity", ActivityTime.ToString()));
					AwardTime = Convert.ToInt32(Global.GetDefAttributeStr(args, "Award", AwardTime.ToString()));
				}
				activity.FromDate = Global.GetHuoDongTimeByHeFu(0, 0, 0, 0);
				activity.ToDate = Global.GetHuoDongTimeByHeFu(ActivityTime - 1, 23, 59, 59);
				activity.AwardStartDate = Global.GetHuoDongTimeByHeFu(1, 0, 0, 0);
				activity.AwardEndDate = Global.GetHuoDongTimeByHeFu(AwardTime, 23, 59, 59);
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							int rank = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "Level"));
							HeFuRechargeData data = new HeFuRechargeData();
							string strFanli = Global.GetDefAttributeStr(xmlItem, "FanLi", "0.0");
							data.Coe = (float)Convert.ToDouble(strFanli);
							data.LowLimit = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							activity.ConfigDict[rank] = data;
							HeFuRechargeActivity heFuRechargeActivity = activity;
							heFuRechargeActivity.strcoe += rank;
							HeFuRechargeActivity heFuRechargeActivity2 = activity;
							heFuRechargeActivity2.strcoe += ",";
							HeFuRechargeActivity heFuRechargeActivity3 = activity;
							heFuRechargeActivity3.strcoe += data.Coe;
							HeFuRechargeActivity heFuRechargeActivity4 = activity;
							heFuRechargeActivity4.strcoe += "|";
						}
					}
				}
				lock (HuodongCachingMgr._HeFuRechargeActivityMutex)
				{
					HuodongCachingMgr._HeFuRechargeActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/HeFuGifts/HeFuFanLi.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetHeFuRechargeActivity()
		{
			string fileName = "Config/HeFuGifts/HeFuFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._HeFuRechargeActivityMutex)
			{
				HuodongCachingMgr._HeFuRechargeActivity = null;
			}
			return 0;
		}

		
		public static XinFanLiActivity GetXinFanLiActivity()
		{
			lock (HuodongCachingMgr._XinFanLiActivityMutex)
			{
				if (HuodongCachingMgr._XinFanLiActivity != null)
				{
					return HuodongCachingMgr._XinFanLiActivity;
				}
			}
			try
			{
				string fileName = "Config/XinFuGifts/MuFanLi.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				XinFanLiActivity activity = new XinFanLiActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(1, 0, 0, 0);
					activity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(7, 23, 59, 59);
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)(Global.GetSafeAttributeDouble(xmlItem, "FanLi") * 100.0));
							myAwardItem.AwardYuanBao = 0;
							myAwardItem.GoodsDataList = new List<GoodsData>();
							string rans = Global.GetSafeAttributeStr(xmlItem, "ID");
							string[] paiHangs = rans.Split(new char[]
							{
								'-'
							});
							if (paiHangs.Length > 0)
							{
								int min = Global.SafeConvertToInt32(paiHangs[0]);
								int max = Global.SafeConvertToInt32(paiHangs[paiHangs.Length - 1]);
								for (int paiHang = min; paiHang <= max; paiHang++)
								{
									activity.AwardDict.Add(paiHang, myAwardItem);
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._XinFanLiActivityMutex)
				{
					HuodongCachingMgr._XinFanLiActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/XinFuGifts/MuFanLi.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetXinFanLiActivity()
		{
			string fileName = "Config/Gifts/XinFanLi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._XinFanLiActivityMutex)
			{
				HuodongCachingMgr._XinFanLiActivity = null;
			}
			return 0;
		}

		
		public static MeiRiChongZhiActivity GetMeiRiChongZhiActivity()
		{
			lock (HuodongCachingMgr._MeiRiChongZhiHaoLiActivityMutex)
			{
				if (HuodongCachingMgr._MeiRiChongZhiHaoLiActivity != null)
				{
					return HuodongCachingMgr._MeiRiChongZhiHaoLiActivity;
				}
			}
			try
			{
				string fileName = "Config/Gifts/DayChongZhi.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				MeiRiChongZhiActivity activity = new MeiRiChongZhiActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							AwardItem myAwardItem = new AwardItem();
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao"));
							myAwardItem.GoodsDataList = new List<GoodsData>();
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取每日充值豪礼活动配置文件中的物品配置1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取每日充值豪礼活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "每日充值豪礼活动");
								}
							}
							int nID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							activity.AwardDict.Add(nID, myAwardItem);
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._MeiRiChongZhiHaoLiActivityMutex)
				{
					HuodongCachingMgr._MeiRiChongZhiHaoLiActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/Gifts/DayChongZhi.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetMeiRiChongZhiActivity()
		{
			string fileName = "Config/Gifts/DayChongZhi.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._MeiRiChongZhiHaoLiActivityMutex)
			{
				HuodongCachingMgr._MeiRiChongZhiHaoLiActivity = null;
			}
			return 0;
		}

		
		public static KingActivity GetChongJiHaoLiActivity()
		{
			lock (HuodongCachingMgr._ChongJiHaoLiActivityMutex)
			{
				if (HuodongCachingMgr._ChongJiHaoLiActivity != null)
				{
					return HuodongCachingMgr._ChongJiHaoLiActivity;
				}
			}
			try
			{
				string fileName = "Config/XinFuGifts/MuLevel.xml";
				if (Global.isDoubleXinFu(33))
				{
					fileName = "Config/XinFuGifts/MuDoubleLevel.xml";
				}
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				ChongJiHaoLiActivity activity = new ChongJiHaoLiActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.ToDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					activity.AwardStartDate = Global.GetHuoDongTimeByKaiFu(0, 0, 0, 0);
					activity.AwardEndDate = Global.GetHuoDongTimeByKaiFu(6, 23, 59, 59);
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							int nID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							AwardItem myAwardItem = new AwardItem();
							AwardItem myAwardItem2 = new AwardItem();
							myAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinZhuanSheng"));
							myAwardItem.MinAwardCondionValue2 = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "Roles"));
							myAwardItem.MinAwardCondionValue3 = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel"));
							myAwardItem2.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinZhuanSheng"));
							myAwardItem2.MinAwardCondionValue2 = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "Roles"));
							myAwardItem2.MinAwardCondionValue3 = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel"));
							myAwardItem.GoodsDataList = new List<GoodsData>();
							myAwardItem2.GoodsDataList = new List<GoodsData>();
							int rolelimit = (int)Global.GetSafeAttributeLong(xmlItem, "Roles");
							if (rolelimit == -1)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取MuLevel.xml失败 字段：Roles", new object[0]), null, true);
							}
							else
							{
								activity.RoleLimit.Add(nID, rolelimit);
							}
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取MuLevel.xml失败 GoodsOne", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取MuLevel.xml 失败 奖励列表1配置错误", new object[0]), null, true);
								}
								else
								{
									myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "读取MuLevel.xml 奖励列表1");
								}
							}
							activity.AwardDict.Add(nID, myAwardItem);
							goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取MuLevel.xml失败 GoodsTwo", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取MuLevel.xml 失败 奖励列表2 配置错误", new object[0]), null, true);
								}
								else
								{
									myAwardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "读取MuLevel.xml 奖励列表2");
								}
							}
							activity.AwardDict2.Add(nID, myAwardItem2);
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._ChongJiHaoLiActivityMutex)
				{
					HuodongCachingMgr._ChongJiHaoLiActivity = activity;
				}
				return activity;
			}
			catch (Exception e)
			{
				LogManager.WriteException(e.ToString());
			}
			return null;
		}

		
		public static int ResetChongJiHaoLiActivity()
		{
			string fileName = "Config/XinFuGifts/MuLevel.xml";
			if (Global.isDoubleXinFu(33))
			{
				fileName = "Config/XinFuGifts/MuDoubleLevel.xml";
			}
			GeneralCachingXmlMgr.RemoveCachingXml(Global.IsolateResPath(fileName));
			lock (HuodongCachingMgr._ChongJiHaoLiActivityMutex)
			{
				HuodongCachingMgr._ChongJiHaoLiActivity = null;
			}
			return 0;
		}

		
		public static ShenZhuangHuiKuiHaoLiActivity GetShenZhuangJiQiHuiKuiHaoLiActivity()
		{
			lock (HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivityMutex)
			{
				if (HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivity != null)
				{
					return HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivity;
				}
			}
			try
			{
				string fileName = "Config/RiChangGifts/ShenZhuangAward.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				ShenZhuangHuiKuiHaoLiActivity activity = new ShenZhuangHuiKuiHaoLiActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							activity.MyAwardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xmlItem, "Roles"));
							activity.MyAwardItem.GoodsDataList = new List<GoodsData>();
							string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsIDs");
							if (string.IsNullOrEmpty(goodsIDs))
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("读取神装激情回馈豪礼配置文件1失败", new object[0]), null, true);
							}
							else
							{
								string[] fields = goodsIDs.Split(new char[]
								{
									'|'
								});
								if (fields.Length <= 0)
								{
									LogManager.WriteLog(LogTypes.Warning, string.Format("读取神装激情回馈配置文件失败", new object[0]), null, true);
								}
								else
								{
									activity.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "神装激情回馈");
								}
							}
						}
					}
				}
				activity.PredealDateTime();
				lock (HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivityMutex)
				{
					HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/RiChangGifts/ShenZhuangAward.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetShenZhuangJiQiHuiKuiHaoLiActivity()
		{
			string fileName = "Config/RiChangGifts/ShenZhuangAward.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivityMutex)
			{
				HuodongCachingMgr._ShenZhuangJiQingHuiKuiHaoLiActivity = null;
			}
			return 0;
		}

		
		public static YueDuZhuanPanActivity GetYueDuZhuanPanActivity()
		{
			lock (HuodongCachingMgr._YueDuZhuanPanActivityMutex)
			{
				if (HuodongCachingMgr._YueDuZhuanPanActivity != null)
				{
					return HuodongCachingMgr._YueDuZhuanPanActivity;
				}
			}
			try
			{
				string fileName = "Config/RiChangGifts/NewDig2.xml";
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return null;
				}
				YueDuZhuanPanActivity activity = new YueDuZhuanPanActivity();
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					activity.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					activity.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
				}
				lock (HuodongCachingMgr._YueDuZhuanPanActivityMutex)
				{
					HuodongCachingMgr._YueDuZhuanPanActivity = activity;
				}
				return activity;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/RiChangGifts/NewDig2.xml解析出现异常", ex, true);
			}
			return null;
		}

		
		public static int ResetYueDuZhuanPanActivity()
		{
			string fileName = "Config/RiChangGifts/NewDig2.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
			lock (HuodongCachingMgr._YueDuZhuanPanActivityMutex)
			{
				HuodongCachingMgr._YueDuZhuanPanActivity = null;
			}
			return 0;
		}

		
		public static void CheckJieRiActivityState(long ticks)
		{
			if (ticks - HuodongCachingMgr.lastJieRiProcessTicks >= 10000L)
			{
				HuodongCachingMgr.lastJieRiProcessTicks = ticks;
				DateTime ActTime = Global.GetJieriStartDay();
				DateTime EndTime = Global.GetJieriStartDay().AddDays((double)Global.GetJieriDaysNum());
				if (TimeUtil.NowDateTime() >= ActTime && TimeUtil.NowDateTime() < EndTime)
				{
					if (HuodongCachingMgr.JieRiState == 0)
					{
						HuodongCachingMgr.JieRiState = 1;
						GameManager.ClientMgr.NotifyAllActivityState(1, HuodongCachingMgr.JieRiState, "", "", 0);
					}
				}
				if (TimeUtil.NowDateTime() >= EndTime)
				{
					if (HuodongCachingMgr.JieRiState == 1)
					{
						HuodongCachingMgr.JieRiState = 0;
						GameManager.ClientMgr.NotifyAllActivityState(1, HuodongCachingMgr.JieRiState, "", "", 0);
					}
				}
				EndTime = Global.GetHefuStartDay().AddDays(8.0);
				if (TimeUtil.NowDateTime() >= EndTime)
				{
					if (HuodongCachingMgr.HefuState == 1)
					{
						HuodongCachingMgr.HefuState = 0;
						GameManager.ClientMgr.NotifyAllActivityState(2, HuodongCachingMgr.HefuState, "", "", 0);
					}
				}
				ThemeActivityConfig config = HuodongCachingMgr.GetThemeActivityConfig();
				if (null != config)
				{
					if (HuodongCachingMgr.ThemeState != config.ActivityOpenVavle)
					{
						HuodongCachingMgr.ThemeState = config.ActivityOpenVavle;
						GameManager.ClientMgr.NotifyAllActivityState(14, config.ActivityOpenVavle, "", "", 0);
					}
				}
				else if (HuodongCachingMgr.ThemeState == 1)
				{
					HuodongCachingMgr.ThemeState = 0;
					GameManager.ClientMgr.NotifyAllActivityState(14, 0, "", "", 0);
				}
			}
		}

		
		private static long lastJieRiProcessTicks = 0L;

		
		private static int JieRiState = 0;

		
		private static int HefuState = 1;

		
		private static int ThemeState = 0;

		
		private static Dictionary<int, WLoginItem> _WLoginDict = new Dictionary<int, WLoginItem>();

		
		private static Dictionary<int, MOnlineTimeItem> _MonthTimeDict = new Dictionary<int, MOnlineTimeItem>();

		
		private static Dictionary<int, NewStepItem> _NewStepDict = new Dictionary<int, NewStepItem>();

		
		private static Dictionary<int, CombatAwardItem> _CombatAwardlDict = new Dictionary<int, CombatAwardItem>();

		
		private static Dictionary<int, Dictionary<int, UpLevelItem>> _UpLevelDict = new Dictionary<int, Dictionary<int, UpLevelItem>>();

		
		private static object _BigAwardItemMutex = new object();

		
		private static BigAwardItem _BigAwardItem = null;

		
		private static object _SongLiItemMutex = new object();

		
		private static SongLiItem _SongLiItem = null;

		
		private static DateTime _LimitTimeLoginStartTime = new DateTime(1971, 1, 1);

		
		private static DateTime _LimitTimeLoginEndTime = new DateTime(1971, 1, 1);

		
		private static Dictionary<int, LimitTimeLoginItem> _LimitTimeLoginDict = new Dictionary<int, LimitTimeLoginItem>();

		
		private static Dictionary<int, EveryDayOnLineAward> _EveryDayOnLineAwardDict = new Dictionary<int, EveryDayOnLineAward>();

		
		private static Dictionary<int, SeriesLoginAward> _SeriesLoginAward = new Dictionary<int, SeriesLoginAward>();

		
		private static FirstChongZhiGift _FirstChongZhiActivity = null;

		
		private static object _FirstChongZhiActivityMutex = new object();

		
		private static InputFanLiActivity _InputFanLiActivity = null;

		
		private static object _InputFanLiActivityMutex = new object();

		
		private static InputSongActivity _InputSongActivity = null;

		
		private static object _InputSongActivityMutex = new object();

		
		private static KingActivity _InputKingActivity = null;

		
		private static object _InputKingActivityMutex = new object();

		
		private static KingActivity _LevelKingActivity = null;

		
		private static object _LevelKingActivityMutex = new object();

		
		private static KingActivity _EquipKingActivity = null;

		
		private static object _EquipKingActivityMutex = new object();

		
		private static KingActivity _HorseKingActivity = null;

		
		private static object _HorseKingActivityMutex = new object();

		
		private static KingActivity _JingMaiKingActivity = null;

		
		private static object _JingMaiKingActivityMutex = new object();

		
		private static KingActivity _XinXiaofeiKingActivity = null;

		
		private static object _XinXiaofeiKingMutex = new object();

		
		public static Dictionary<int, UpLevelAwardItem> UpLevelAwardItemDict = null;

		
		public static Dictionary<int, KaiFuGiftItem> KaiFuGiftItemDict = null;

		
		private static int LastProcessGetKaiFuGiftAwardDayID = 0;

		
		public static int ProcessKaiFuGiftAwardHour = 12;

		
		private static int LastAutoAddKaiFuGiftRoleNumDayID = TimeUtil.NowDateTime().DayOfYear;

		
		private static JieriActivityConfig _JieriActivityConfig = null;

		
		private static object _JieriActivityConfigMutex = new object();

		
		private static ThemeActivityConfig _ThemeActivityConfig = null;

		
		private static object _ThemeActivityConfigMutex = new object();

		
		private static ThemeDaLiBaoActivity _ThemeDaLiBaoActivity = null;

		
		private static object _ThemeDaLiBaoActivityMutex = new object();

		
		private static ThemeDuiHuanActivity _ThemeDuiHuanActivity = null;

		
		private static object _ThemeDuiHuanActivityMutex = new object();

		
		private static ThemeZhiGouActivity _ThemeZhiGouActivity = null;

		
		private static object _ThemeZhiGouActivityMutex = new object();

		
		private static JieriDaLiBaoActivity _JieriDaLiBaoActivity = null;

		
		private static object _JieriDaLiBaoActivityMutex = new object();

		
		private static JieRiDengLuActivity _JieRiDengLuActivity = null;

		
		private static object _JieriDengLuActivityMutex = new object();

		
		private static JieriVIPActivity _JieriVIPActivity = null;

		
		private static object _JieriVIPActivityMutex = new object();

		
		private static JieriGiveActivity _JieriGiveActivity = null;

		
		private static object _JieriGiveMutex = new object();

		
		private static JieriRecvActivity _JieriRecvActivity = null;

		
		private static object _JieriRecvMutex = new object();

		
		private static JieRiGiveKingActivity _JieriGiveKingActivity = null;

		
		private static object _JieriGiveKingMutex = new object();

		
		private static JieRiRecvKingActivity _JieriRecvKingActivity = null;

		
		private static object _JieriRecvKingMutex = new object();

		
		private static JieRiFuLiActivity _JieriFuLiActivity = null;

		
		private static object _JieriFuLiMutex = new object();

		
		private static JieriCZSongActivity _JieriCZSongActivity = null;

		
		private static object _JieriCZSongActivityMutex = new object();

		
		private static JieRiCZQGActivity _JieRiCZQGActivity = null;

		
		private static object _JieRiCZQGActivityMutex = new object();

		
		private static OneDollarBuyActivity _OneDollarBuyActivity = null;

		
		private static object _OneDollarBuyActivityMutex = new object();

		
		private static OneDollarChongZhi _OneDollarChongZhi = null;

		
		private static object _OneDollarChongZhiMutex = new object();

		
		private static InputFanLiNew _InputFanLiNew = null;

		
		private static object _InputFanLiNewMutex = new object();

		
		private static RegressActiveOpen _RegressActiveOpen = null;

		
		private static object _RegressActiveOpenMutex = new object();

		
		private static RegressActiveSignGift _RegressActiveSignGift = null;

		
		private static object _RegressActiveSignGiftMutex = new object();

		
		private static RegressActiveTotalRecharge _RegressActiveTotalRecharge = null;

		
		private static object _RegressActiveTotalRechargeMutex = new object();

		
		private static RegressActiveDayBuy _RegressActiveDayBuy = null;

		
		private static object _RegressActiveDayBuyMutex = new object();

		
		private static RegressActiveStore _RegressActiveStore = null;

		
		private static object _RegressActiveStoreMutex = new object();

		
		private static JieriSuperInputActivity _JieriSuperInput = null;

		
		private static object _JieriSuperInputMutex = new object();

		
		private static JieriVIPYouHuiActivity _JieriVIPYouHuiActivity = null;

		
		private static object _JieriVIPYouHuiActMutex = new object();

		
		private static SpecialActivity _SpecialActivity = null;

		
		private static object _SpecialActivityMutex = new object();

		
		private static SpecPriorityActivity _SpecPriorityActivity = null;

		
		private static object _SpecPriorityActivityMutex = new object();

		
		private static EverydayActivity _EverydayActivity = null;

		
		private static object _EverydayActivityMutex = new object();

		
		private static JieriIPointsExchgActivity _JieriIPointsExchgActivity = null;

		
		private static object _JieriIPointsExchgActivityMutex = new object();

		
		private static WeedEndInputActivity _WeedEndInputActivity = null;

		
		private static object _WeedEndInputActivityMutex = new object();

		
		private static JieRiLeiJiCZActivity _JieRiLeiJiCZActivity = null;

		
		private static object _JieRiLeiJiCZActivityMutex = new object();

		
		private static JieRiTotalConsumeActivity _JieRiTotalConsumeActivity = null;

		
		private static object _JieRiTotalConsumeActivityMutex = new object();

		
		private static JieRiMeiRiLeiJiActivity _JieRiMeiRiLeiJiActivity = null;

		
		private static object _JieRiMeiRiLeiJiActivityMutex = new object();

		
		private static JieRiMultAwardActivity _JieRiMultAwardActivity = null;

		
		private static object _JieRiMultAwardActivityMutex = new object();

		
		private static JieRiZiKaLiaBaoActivity _JieRiZiKaLiaBaoActivity = null;

		
		private static object _JieRiZiKaLiaBaoActivityMutex = new object();

		
		private static KingActivity _JieRiXiaoFeiKingActivity = null;

		
		private static object _JieRiXiaoFeiKingActivityMutex = new object();

		
		private static KingActivity _JieRiCZKingActivity = null;

		
		private static object _JieRiCZKingActivityMutex = new object();

		
		private static HuodongCachingMgr.TotalChargeActivity _TotalChargeActivity = null;

		
		private static object _TotalChargeActivityMutex = new object();

		
		private static HuodongCachingMgr.TotalConsumeActivity _TotalConsumeActivity = null;

		
		private static object _TotalConsumeActivityMutex = new object();

		
		private static JieriFanLiActivity[] _JieriWingFanliAct = new JieriFanLiActivity[11];

		
		private static object _JieriWingFanliActMutex = new object();

		
		private static object _JieriLianXuChargeMutex = new object();

		
		private static JieriLianXuChargeActivity _JieriLianXuChargeAct = null;

		
		private static object _JieriPlatChargeKingMutex = new object();

		
		private static JieriPlatChargeKing _JieriPlatChargeKingAct = null;

		
		private static object _JieriPCKingEveryDayMutex = new object();

		
		private static JieriPlatChargeKingEveryDay _JieriPCKingEveryDayAct = null;

		
		private static object _DanBiChongZhiMutex = new object();

		
		private static DanBiChongZhiActivity _DanBiChongZhiAct = null;

		
		private static HeFuActivityConfig _HeFuActivityConfig = null;

		
		private static object _HeFuActivityConfigMutex = new object();

		
		private static HeFuLoginActivity _HeFuLoginActivity = null;

		
		private static object _HeFuLoginActivityMutex = new object();

		
		private static HeFuRechargeActivity _HeFuRechargeActivity = null;

		
		private static object _HeFuRechargeActivityMutex = new object();

		
		private static HeFuTotalLoginActivity _HeFuTotalLoginActivity = null;

		
		private static object _HeFuTotalLoginActivityMutex = new object();

		
		private static HeFuPKKingActivity _HeFuPKKingActivity = null;

		
		private static object _HeFuAwardTimeActivityMutex = new object();

		
		private static HeFuAwardTimesActivity _HeFuAwardTimeActivity = null;

		
		private static HeFuLuoLanActivity _HeFuLuoLanActivity = null;

		
		private static object _HeFuLuoLanActivityMutex = new object();

		
		private static object _HeFuPKKingActivityMutex = new object();

		
		private static HeFuWCKingActivity _HeFuWCKingActivity = null;

		
		private static object _HeFuWCKingActivityMutex = new object();

		
		private static XinFanLiActivity _XinFanLiActivity = null;

		
		private static object _XinFanLiActivityMutex = new object();

		
		private static object _MeiRiChongZhiHaoLiActivityMutex = new object();

		
		private static MeiRiChongZhiActivity _MeiRiChongZhiHaoLiActivity = null;

		
		private static object _ChongJiHaoLiActivityMutex = new object();

		
		private static ChongJiHaoLiActivity _ChongJiHaoLiActivity = null;

		
		private static object _ShenZhuangJiQingHuiKuiHaoLiActivityMutex = new object();

		
		private static ShenZhuangHuiKuiHaoLiActivity _ShenZhuangJiQingHuiKuiHaoLiActivity = null;

		
		private static object _YueDuZhuanPanActivityMutex = new object();

		
		private static YueDuZhuanPanActivity _YueDuZhuanPanActivity = null;

		
		private static class GiftCodeFlags
		{
			
			public const int Local = 1;

			
			public const int Center = 2;
		}

		
		public class TotalConsumeActivity : KingActivity
		{
			
			public override bool CanGiveAward(GameClient client, int index, int totalMoney)
			{
				bool hasGet = false;
				try
				{
					if (this.AwardDict != null && this.AwardDict.ContainsKey(index))
					{
						hasGet = (this.AwardDict[index].MinAwardCondionValue <= totalMoney);
					}
				}
				catch (Exception e)
				{
					LogManager.WriteException(e.ToString());
				}
				return hasGet;
			}
		}

		
		public class TotalChargeActivity : KingActivity
		{
			
			public override bool CanGiveAward(GameClient client, int index, int totalMoney)
			{
				bool hasGet = false;
				try
				{
					if (this.AwardDict != null && this.AwardDict.ContainsKey(index))
					{
						hasGet = (this.AwardDict[index].MinAwardCondionValue <= totalMoney);
					}
				}
				catch (Exception e)
				{
					LogManager.WriteException(e.ToString());
				}
				return hasGet;
			}
		}
	}
}
