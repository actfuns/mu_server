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
	// Token: 0x02000720 RID: 1824
	public class HuodongCachingMgr
	{
		// Token: 0x06002B95 RID: 11157 RVA: 0x00268B54 File Offset: 0x00266D54
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

		// Token: 0x06002B96 RID: 11158 RVA: 0x00268BF4 File Offset: 0x00266DF4
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

		// Token: 0x06002B97 RID: 11159 RVA: 0x00268D20 File Offset: 0x00266F20
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

		// Token: 0x06002B98 RID: 11160 RVA: 0x00268DB4 File Offset: 0x00266FB4
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

		// Token: 0x06002B99 RID: 11161 RVA: 0x00268F54 File Offset: 0x00267154
		public static long GetHuoDongDateTime(string str)
		{
			string strDateTime = HuodongCachingMgr.ParseDateTime(str);
			return Global.SafeConvertToTicks(strDateTime);
		}

		// Token: 0x06002B9A RID: 11162 RVA: 0x00268F74 File Offset: 0x00267174
		public static long GetHuoDongDateTimeForCommonTimeString(string str)
		{
			return Global.SafeConvertToTicks(str);
		}

		// Token: 0x06002B9B RID: 11163 RVA: 0x00268F8C File Offset: 0x0026718C
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

		// Token: 0x06002B9C RID: 11164 RVA: 0x0026901C File Offset: 0x0026721C
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

		// Token: 0x06002B9D RID: 11165 RVA: 0x00269288 File Offset: 0x00267488
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

		// Token: 0x06002B9E RID: 11166 RVA: 0x002694F8 File Offset: 0x002676F8
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

		// Token: 0x06002B9F RID: 11167 RVA: 0x002696AC File Offset: 0x002678AC
		public static int ResetWLoginItem()
		{
			int ret = GameManager.systemWeekLoginGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._WLoginDict)
			{
				HuodongCachingMgr._WLoginDict.Clear();
			}
			return ret;
		}

		// Token: 0x06002BA0 RID: 11168 RVA: 0x00269710 File Offset: 0x00267910
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

		// Token: 0x06002BA1 RID: 11169 RVA: 0x0026990C File Offset: 0x00267B0C
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

		// Token: 0x06002BA2 RID: 11170 RVA: 0x00269A48 File Offset: 0x00267C48
		public static int ResetMOnlineTimeItem()
		{
			int ret = GameManager.systemMOnlineTimeGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._MonthTimeDict)
			{
				HuodongCachingMgr._MonthTimeDict.Clear();
			}
			return ret;
		}

		// Token: 0x06002BA3 RID: 11171 RVA: 0x00269AAC File Offset: 0x00267CAC
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

		// Token: 0x06002BA4 RID: 11172 RVA: 0x00269BA0 File Offset: 0x00267DA0
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

		// Token: 0x06002BA5 RID: 11173 RVA: 0x00269D84 File Offset: 0x00267F84
		public static int ResetNewStepItem()
		{
			int ret = GameManager.systemNewRoleGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._NewStepDict)
			{
				HuodongCachingMgr._NewStepDict.Clear();
			}
			return ret;
		}

		// Token: 0x06002BA6 RID: 11174 RVA: 0x00269DE8 File Offset: 0x00267FE8
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

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06002BA7 RID: 11175 RVA: 0x0026A188 File Offset: 0x00268388
		public static int CombatGiftMaxVal
		{
			get
			{
				return HuodongCachingMgr._CombatAwardlDict.Count;
			}
		}

		// Token: 0x06002BA8 RID: 11176 RVA: 0x0026A1A4 File Offset: 0x002683A4
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

		// Token: 0x06002BA9 RID: 11177 RVA: 0x0026A3C0 File Offset: 0x002685C0
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

		// Token: 0x06002BAA RID: 11178 RVA: 0x0026A434 File Offset: 0x00268634
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

		// Token: 0x06002BAB RID: 11179 RVA: 0x0026A594 File Offset: 0x00268794
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

		// Token: 0x06002BAC RID: 11180 RVA: 0x0026A7B8 File Offset: 0x002689B8
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

		// Token: 0x06002BAD RID: 11181 RVA: 0x0026A978 File Offset: 0x00268B78
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

		// Token: 0x06002BAE RID: 11182 RVA: 0x0026A9DC File Offset: 0x00268BDC
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

		// Token: 0x06002BAF RID: 11183 RVA: 0x0026AAA0 File Offset: 0x00268CA0
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

		// Token: 0x06002BB0 RID: 11184 RVA: 0x0026AC48 File Offset: 0x00268E48
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

		// Token: 0x06002BB1 RID: 11185 RVA: 0x0026AE44 File Offset: 0x00269044
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

		// Token: 0x06002BB2 RID: 11186 RVA: 0x0026AEC8 File Offset: 0x002690C8
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

		// Token: 0x06002BB3 RID: 11187 RVA: 0x0026AF98 File Offset: 0x00269198
		public static int ResetUpLevelItem()
		{
			int ret = GameManager.systemUpLevelGiftMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._UpLevelDict)
			{
				HuodongCachingMgr._UpLevelDict.Clear();
			}
			return ret;
		}

		// Token: 0x06002BB4 RID: 11188 RVA: 0x0026AFFC File Offset: 0x002691FC
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

		// Token: 0x06002BB5 RID: 11189 RVA: 0x0026B4C0 File Offset: 0x002696C0
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

		// Token: 0x06002BB6 RID: 11190 RVA: 0x0026B5D8 File Offset: 0x002697D8
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

		// Token: 0x06002BB7 RID: 11191 RVA: 0x0026B734 File Offset: 0x00269934
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

		// Token: 0x06002BB8 RID: 11192 RVA: 0x0026B8F8 File Offset: 0x00269AF8
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

		// Token: 0x06002BB9 RID: 11193 RVA: 0x0026BC14 File Offset: 0x00269E14
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

		// Token: 0x06002BBA RID: 11194 RVA: 0x0026BC78 File Offset: 0x00269E78
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

		// Token: 0x06002BBB RID: 11195 RVA: 0x0026BF18 File Offset: 0x0026A118
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

		// Token: 0x06002BBC RID: 11196 RVA: 0x0026C248 File Offset: 0x0026A448
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

		// Token: 0x06002BBD RID: 11197 RVA: 0x0026C2AC File Offset: 0x0026A4AC
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

		// Token: 0x06002BBE RID: 11198 RVA: 0x0026C784 File Offset: 0x0026A984
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

		// Token: 0x06002BBF RID: 11199 RVA: 0x0026CADC File Offset: 0x0026ACDC
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

		// Token: 0x06002BC0 RID: 11200 RVA: 0x0026CC84 File Offset: 0x0026AE84
		public static bool JugeInLimitTimeLoginPeriod()
		{
			return HuodongCachingMgr.GetLimitTimeLoginHuoDongID() > 0;
		}

		// Token: 0x06002BC1 RID: 11201 RVA: 0x0026CCA0 File Offset: 0x0026AEA0
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

		// Token: 0x06002BC2 RID: 11202 RVA: 0x0026CD8C File Offset: 0x0026AF8C
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

		// Token: 0x06002BC3 RID: 11203 RVA: 0x0026CF40 File Offset: 0x0026B140
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

		// Token: 0x06002BC4 RID: 11204 RVA: 0x0026CFC4 File Offset: 0x0026B1C4
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

		// Token: 0x06002BC5 RID: 11205 RVA: 0x0026D1D4 File Offset: 0x0026B3D4
		public static int GetEveryDayOnLineItemCount()
		{
			return GameManager.systemEveryDayOnLineAwardMgr.SystemXmlItemDict.Count;
		}

		// Token: 0x06002BC6 RID: 11206 RVA: 0x0026D1F8 File Offset: 0x0026B3F8
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

		// Token: 0x06002BC7 RID: 11207 RVA: 0x0026D360 File Offset: 0x0026B560
		public static int ResetEveryDayOnLineAwardItem()
		{
			int ret = GameManager.systemEveryDayOnLineAwardMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._EveryDayOnLineAwardDict)
			{
				HuodongCachingMgr._EveryDayOnLineAwardDict.Clear();
			}
			return ret;
		}

		// Token: 0x06002BC8 RID: 11208 RVA: 0x0026D3C4 File Offset: 0x0026B5C4
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

		// Token: 0x06002BC9 RID: 11209 RVA: 0x0026D564 File Offset: 0x0026B764
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

		// Token: 0x06002BCA RID: 11210 RVA: 0x0026D6E8 File Offset: 0x0026B8E8
		public static int GetSeriesLoginCount()
		{
			return GameManager.systemSeriesLoginAwardMgr.SystemXmlItemDict.Count;
		}

		// Token: 0x06002BCB RID: 11211 RVA: 0x0026D70C File Offset: 0x0026B90C
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

		// Token: 0x06002BCC RID: 11212 RVA: 0x0026D86C File Offset: 0x0026BA6C
		public static int ResetSeriesLoginItem()
		{
			int ret = GameManager.systemSeriesLoginAwardMgr.ReloadLoadFromXMlFile();
			lock (HuodongCachingMgr._SeriesLoginAward)
			{
				HuodongCachingMgr._SeriesLoginAward.Clear();
			}
			return ret;
		}

		// Token: 0x06002BCD RID: 11213 RVA: 0x0026D8D0 File Offset: 0x0026BAD0
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

		// Token: 0x06002BCE RID: 11214 RVA: 0x0026DA34 File Offset: 0x0026BC34
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

		// Token: 0x06002BCF RID: 11215 RVA: 0x0026DB70 File Offset: 0x0026BD70
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

		// Token: 0x06002BD0 RID: 11216 RVA: 0x0026DE3C File Offset: 0x0026C03C
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

		// Token: 0x06002BD1 RID: 11217 RVA: 0x0026DEF8 File Offset: 0x0026C0F8
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

		// Token: 0x06002BD2 RID: 11218 RVA: 0x0026DF5C File Offset: 0x0026C15C
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

		// Token: 0x06002BD3 RID: 11219 RVA: 0x0026E2D8 File Offset: 0x0026C4D8
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

		// Token: 0x06002BD4 RID: 11220 RVA: 0x0026E33C File Offset: 0x0026C53C
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

		// Token: 0x06002BD5 RID: 11221 RVA: 0x0026E550 File Offset: 0x0026C750
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

		// Token: 0x06002BD6 RID: 11222 RVA: 0x0026E5B4 File Offset: 0x0026C7B4
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

		// Token: 0x06002BD7 RID: 11223 RVA: 0x0026E878 File Offset: 0x0026CA78
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

		// Token: 0x06002BD8 RID: 11224 RVA: 0x0026E8DC File Offset: 0x0026CADC
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

		// Token: 0x06002BD9 RID: 11225 RVA: 0x0026ED04 File Offset: 0x0026CF04
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

		// Token: 0x06002BDA RID: 11226 RVA: 0x0026ED7C File Offset: 0x0026CF7C
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

		// Token: 0x06002BDB RID: 11227 RVA: 0x0026F0FC File Offset: 0x0026D2FC
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

		// Token: 0x06002BDC RID: 11228 RVA: 0x0026F160 File Offset: 0x0026D360
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

		// Token: 0x06002BDD RID: 11229 RVA: 0x0026F590 File Offset: 0x0026D790
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

		// Token: 0x06002BDE RID: 11230 RVA: 0x0026F608 File Offset: 0x0026D808
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

		// Token: 0x06002BDF RID: 11231 RVA: 0x0026F988 File Offset: 0x0026DB88
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

		// Token: 0x06002BE0 RID: 11232 RVA: 0x0026F9EC File Offset: 0x0026DBEC
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

		// Token: 0x06002BE1 RID: 11233 RVA: 0x0026FD6C File Offset: 0x0026DF6C
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

		// Token: 0x06002BE2 RID: 11234 RVA: 0x0026FDD0 File Offset: 0x0026DFD0
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

		// Token: 0x06002BE3 RID: 11235 RVA: 0x00270200 File Offset: 0x0026E400
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

		// Token: 0x06002BE4 RID: 11236 RVA: 0x00270278 File Offset: 0x0026E478
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

		// Token: 0x06002BE5 RID: 11237 RVA: 0x002704B0 File Offset: 0x0026E6B0
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

		// Token: 0x06002BE6 RID: 11238 RVA: 0x00270764 File Offset: 0x0026E964
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

		// Token: 0x06002BE7 RID: 11239 RVA: 0x002708B8 File Offset: 0x0026EAB8
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

		// Token: 0x06002BE8 RID: 11240 RVA: 0x0027091C File Offset: 0x0026EB1C
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

		// Token: 0x06002BE9 RID: 11241 RVA: 0x00270A70 File Offset: 0x0026EC70
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

		// Token: 0x06002BEA RID: 11242 RVA: 0x00270AD4 File Offset: 0x0026ECD4
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

		// Token: 0x06002BEB RID: 11243 RVA: 0x00270F6C File Offset: 0x0026F16C
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

		// Token: 0x06002BEC RID: 11244 RVA: 0x00271028 File Offset: 0x0026F228
		public static int ResetJieriLianXuChargeActivity()
		{
			lock (HuodongCachingMgr._JieriLianXuChargeMutex)
			{
				HuodongCachingMgr._JieriLianXuChargeAct = null;
			}
			return 0;
		}

		// Token: 0x06002BED RID: 11245 RVA: 0x00271078 File Offset: 0x0026F278
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

		// Token: 0x06002BEE RID: 11246 RVA: 0x00271134 File Offset: 0x0026F334
		public static int ResetJieriPCKingActivityEveryDay()
		{
			lock (HuodongCachingMgr._JieriPCKingEveryDayMutex)
			{
				HuodongCachingMgr._JieriPCKingEveryDayAct = null;
			}
			return 0;
		}

		// Token: 0x06002BEF RID: 11247 RVA: 0x00271184 File Offset: 0x0026F384
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

		// Token: 0x06002BF0 RID: 11248 RVA: 0x00271240 File Offset: 0x0026F440
		public static int ResetJieriPlatChargeKingActivity()
		{
			lock (HuodongCachingMgr._JieriPlatChargeKingMutex)
			{
				HuodongCachingMgr._JieriPlatChargeKingAct = null;
			}
			return 0;
		}

		// Token: 0x06002BF1 RID: 11249 RVA: 0x00271290 File Offset: 0x0026F490
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

		// Token: 0x06002BF2 RID: 11250 RVA: 0x00271404 File Offset: 0x0026F604
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

		// Token: 0x06002BF3 RID: 11251 RVA: 0x00271538 File Offset: 0x0026F738
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

		// Token: 0x06002BF4 RID: 11252 RVA: 0x00271730 File Offset: 0x0026F930
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

		// Token: 0x06002BF5 RID: 11253 RVA: 0x002718A4 File Offset: 0x0026FAA4
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

		// Token: 0x06002BF6 RID: 11254 RVA: 0x002719CC File Offset: 0x0026FBCC
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

		// Token: 0x06002BF7 RID: 11255 RVA: 0x00271A54 File Offset: 0x0026FC54
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

		// Token: 0x06002BF8 RID: 11256 RVA: 0x00271A9F File Offset: 0x0026FC9F
		public static void ProcessKaiFuGiftAwardActions()
		{
			HuodongCachingMgr.ProcessGetKaiFuGiftAward();
			HuodongCachingMgr.ProcessAutoAddKaiFuGiftRoleNum();
		}

		// Token: 0x06002BF9 RID: 11257 RVA: 0x00271AB0 File Offset: 0x0026FCB0
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

		// Token: 0x06002BFA RID: 11258 RVA: 0x00271D98 File Offset: 0x0026FF98
		public static void ProcessAutoAddKaiFuGiftRoleNum()
		{
		}

		// Token: 0x06002BFB RID: 11259 RVA: 0x00271DA0 File Offset: 0x0026FFA0
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

		// Token: 0x06002BFC RID: 11260 RVA: 0x00271E58 File Offset: 0x00270058
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

		// Token: 0x06002BFD RID: 11261 RVA: 0x0027209C File Offset: 0x0027029C
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

		// Token: 0x06002BFE RID: 11262 RVA: 0x00272184 File Offset: 0x00270384
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

		// Token: 0x06002BFF RID: 11263 RVA: 0x00272250 File Offset: 0x00270450
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

		// Token: 0x06002C00 RID: 11264 RVA: 0x00272850 File Offset: 0x00270A50
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

		// Token: 0x06002C01 RID: 11265 RVA: 0x00272D0C File Offset: 0x00270F0C
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

		// Token: 0x06002C02 RID: 11266 RVA: 0x00272D38 File Offset: 0x00270F38
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

		// Token: 0x06002C03 RID: 11267 RVA: 0x00272F94 File Offset: 0x00271194
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

		// Token: 0x06002C04 RID: 11268 RVA: 0x0027300C File Offset: 0x0027120C
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

		// Token: 0x06002C05 RID: 11269 RVA: 0x002731EC File Offset: 0x002713EC
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

		// Token: 0x06002C06 RID: 11270 RVA: 0x00273250 File Offset: 0x00271450
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

		// Token: 0x06002C07 RID: 11271 RVA: 0x00273660 File Offset: 0x00271860
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

		// Token: 0x06002C08 RID: 11272 RVA: 0x002736C4 File Offset: 0x002718C4
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

		// Token: 0x06002C09 RID: 11273 RVA: 0x00273B40 File Offset: 0x00271D40
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

		// Token: 0x06002C0A RID: 11274 RVA: 0x00273BA4 File Offset: 0x00271DA4
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

		// Token: 0x06002C0B RID: 11275 RVA: 0x00273E44 File Offset: 0x00272044
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

		// Token: 0x06002C0C RID: 11276 RVA: 0x00273EA8 File Offset: 0x002720A8
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

		// Token: 0x06002C0D RID: 11277 RVA: 0x00273F60 File Offset: 0x00272160
		public static int ResetJieriGiveActivity()
		{
			lock (HuodongCachingMgr._JieriGiveMutex)
			{
				HuodongCachingMgr._JieriGiveActivity = null;
			}
			return 0;
		}

		// Token: 0x06002C0E RID: 11278 RVA: 0x00273FB0 File Offset: 0x002721B0
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

		// Token: 0x06002C0F RID: 11279 RVA: 0x00274068 File Offset: 0x00272268
		public static int ResetJieriRecvActivity()
		{
			lock (HuodongCachingMgr._JieriRecvMutex)
			{
				HuodongCachingMgr._JieriRecvActivity = null;
			}
			return 0;
		}

		// Token: 0x06002C10 RID: 11280 RVA: 0x002740B8 File Offset: 0x002722B8
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

		// Token: 0x06002C11 RID: 11281 RVA: 0x00274178 File Offset: 0x00272378
		public static int ResetJieRiGiveKingActivity()
		{
			lock (HuodongCachingMgr._JieriGiveKingMutex)
			{
				HuodongCachingMgr._JieriGiveKingActivity = null;
			}
			return 0;
		}

		// Token: 0x06002C12 RID: 11282 RVA: 0x002741C8 File Offset: 0x002723C8
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

		// Token: 0x06002C13 RID: 11283 RVA: 0x00274288 File Offset: 0x00272488
		public static int ResetJieriRecvKingActivity()
		{
			lock (HuodongCachingMgr._JieriRecvKingMutex)
			{
				HuodongCachingMgr._JieriRecvKingActivity = null;
			}
			return 0;
		}

		// Token: 0x06002C14 RID: 11284 RVA: 0x002742D8 File Offset: 0x002724D8
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

		// Token: 0x06002C15 RID: 11285 RVA: 0x00274390 File Offset: 0x00272590
		public static int ResetJieriFuLiActivity()
		{
			lock (HuodongCachingMgr._JieriFuLiMutex)
			{
				HuodongCachingMgr._JieriFuLiActivity = null;
			}
			return 0;
		}

		// Token: 0x06002C16 RID: 11286 RVA: 0x002743E0 File Offset: 0x002725E0
		public static int ResetOneDollarChongZhiActivity()
		{
			lock (HuodongCachingMgr._OneDollarChongZhiMutex)
			{
				HuodongCachingMgr._OneDollarChongZhi = null;
			}
			GameManager.ClientMgr.NotifyAllOneDollarChongZhiState();
			return 0;
		}

		// Token: 0x06002C17 RID: 11287 RVA: 0x0027443C File Offset: 0x0027263C
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

		// Token: 0x06002C18 RID: 11288 RVA: 0x002744C4 File Offset: 0x002726C4
		public static int ResetInputFanLiNewActivity()
		{
			lock (HuodongCachingMgr._InputFanLiNewMutex)
			{
				HuodongCachingMgr._InputFanLiNew = null;
			}
			GameManager.ClientMgr.NotifyAllInputFanLiNewState();
			return 0;
		}

		// Token: 0x06002C19 RID: 11289 RVA: 0x00274520 File Offset: 0x00272720
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

		// Token: 0x06002C1A RID: 11290 RVA: 0x002745A8 File Offset: 0x002727A8
		public static int ResetRegressActiveOpen()
		{
			lock (HuodongCachingMgr._RegressActiveOpenMutex)
			{
				HuodongCachingMgr._RegressActiveOpen = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveOpenState();
			return 0;
		}

		// Token: 0x06002C1B RID: 11291 RVA: 0x00274604 File Offset: 0x00272804
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

		// Token: 0x06002C1C RID: 11292 RVA: 0x0027468C File Offset: 0x0027288C
		public static int ResetRegressActiveSignGift()
		{
			lock (HuodongCachingMgr._RegressActiveSignGiftMutex)
			{
				HuodongCachingMgr._RegressActiveSignGift = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveSignGiftState();
			return 0;
		}

		// Token: 0x06002C1D RID: 11293 RVA: 0x002746E8 File Offset: 0x002728E8
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

		// Token: 0x06002C1E RID: 11294 RVA: 0x00274770 File Offset: 0x00272970
		public static int ResetRegressActiveTotalRecharge()
		{
			lock (HuodongCachingMgr._RegressActiveTotalRechargeMutex)
			{
				HuodongCachingMgr._RegressActiveTotalRecharge = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveTotalRechargeState();
			return 0;
		}

		// Token: 0x06002C1F RID: 11295 RVA: 0x002747CC File Offset: 0x002729CC
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

		// Token: 0x06002C20 RID: 11296 RVA: 0x00274854 File Offset: 0x00272A54
		public static int ResetRegressActiveDayBuy()
		{
			lock (HuodongCachingMgr._RegressActiveDayBuyMutex)
			{
				HuodongCachingMgr._RegressActiveDayBuy = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveDayBuyState();
			return 0;
		}

		// Token: 0x06002C21 RID: 11297 RVA: 0x002748B0 File Offset: 0x00272AB0
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

		// Token: 0x06002C22 RID: 11298 RVA: 0x00274938 File Offset: 0x00272B38
		public static int ResetRegressActiveStore()
		{
			lock (HuodongCachingMgr._RegressActiveStoreMutex)
			{
				HuodongCachingMgr._RegressActiveStore = null;
			}
			GameManager.ClientMgr.NotifyAllRegressActiveDayBuyState();
			return 0;
		}

		// Token: 0x06002C23 RID: 11299 RVA: 0x00274994 File Offset: 0x00272B94
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

		// Token: 0x06002C24 RID: 11300 RVA: 0x00274A1C File Offset: 0x00272C1C
		public static int ResetJieRiSuperInputFanLiActivity()
		{
			lock (HuodongCachingMgr._JieriSuperInputMutex)
			{
				HuodongCachingMgr._JieriSuperInput = null;
			}
			HuodongCachingMgr.GetJieRiSuperInputActivity();
			return 0;
		}

		// Token: 0x06002C25 RID: 11301 RVA: 0x00274A74 File Offset: 0x00272C74
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

		// Token: 0x06002C26 RID: 11302 RVA: 0x00274AFC File Offset: 0x00272CFC
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

		// Token: 0x06002C27 RID: 11303 RVA: 0x00274B68 File Offset: 0x00272D68
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

		// Token: 0x06002C28 RID: 11304 RVA: 0x00274C24 File Offset: 0x00272E24
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

		// Token: 0x06002C29 RID: 11305 RVA: 0x00274C8C File Offset: 0x00272E8C
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

		// Token: 0x06002C2A RID: 11306 RVA: 0x00274D48 File Offset: 0x00272F48
		public static int ResetJieriVIPYouHuiAct()
		{
			lock (HuodongCachingMgr._JieriVIPYouHuiActMutex)
			{
				HuodongCachingMgr._JieriVIPYouHuiActivity = null;
			}
			return 0;
		}

		// Token: 0x06002C2B RID: 11307 RVA: 0x00274D98 File Offset: 0x00272F98
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

		// Token: 0x06002C2C RID: 11308 RVA: 0x00274E54 File Offset: 0x00273054
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

		// Token: 0x06002C2D RID: 11309 RVA: 0x00274EC8 File Offset: 0x002730C8
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

		// Token: 0x06002C2E RID: 11310 RVA: 0x00274F84 File Offset: 0x00273184
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

		// Token: 0x06002C2F RID: 11311 RVA: 0x00274FF8 File Offset: 0x002731F8
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

		// Token: 0x06002C30 RID: 11312 RVA: 0x00275080 File Offset: 0x00273280
		public static int ResetThemeDaLiBaoActivity()
		{
			lock (HuodongCachingMgr._ThemeDaLiBaoActivityMutex)
			{
				HuodongCachingMgr._ThemeDaLiBaoActivity = null;
			}
			return 0;
		}

		// Token: 0x06002C31 RID: 11313 RVA: 0x002750D0 File Offset: 0x002732D0
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

		// Token: 0x06002C32 RID: 11314 RVA: 0x00275158 File Offset: 0x00273358
		public static int ResetThemeDuiHuanActivity()
		{
			lock (HuodongCachingMgr._ThemeDuiHuanActivityMutex)
			{
				HuodongCachingMgr._ThemeDuiHuanActivity = null;
			}
			return 0;
		}

		// Token: 0x06002C33 RID: 11315 RVA: 0x002751A8 File Offset: 0x002733A8
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

		// Token: 0x06002C34 RID: 11316 RVA: 0x00275230 File Offset: 0x00273430
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

		// Token: 0x06002C35 RID: 11317 RVA: 0x00275298 File Offset: 0x00273498
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

		// Token: 0x06002C36 RID: 11318 RVA: 0x00275320 File Offset: 0x00273520
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

		// Token: 0x06002C37 RID: 11319 RVA: 0x00275394 File Offset: 0x00273594
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

		// Token: 0x06002C38 RID: 11320 RVA: 0x0027541C File Offset: 0x0027361C
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

		// Token: 0x06002C39 RID: 11321 RVA: 0x00275494 File Offset: 0x00273694
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

		// Token: 0x06002C3A RID: 11322 RVA: 0x00275550 File Offset: 0x00273750
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

		// Token: 0x06002C3B RID: 11323 RVA: 0x002755B4 File Offset: 0x002737B4
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

		// Token: 0x06002C3C RID: 11324 RVA: 0x00275670 File Offset: 0x00273870
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

		// Token: 0x06002C3D RID: 11325 RVA: 0x00275AEC File Offset: 0x00273CEC
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

		// Token: 0x06002C3E RID: 11326 RVA: 0x00275B50 File Offset: 0x00273D50
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

		// Token: 0x06002C3F RID: 11327 RVA: 0x00275FC8 File Offset: 0x002741C8
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

		// Token: 0x06002C40 RID: 11328 RVA: 0x0027602C File Offset: 0x0027422C
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

		// Token: 0x06002C41 RID: 11329 RVA: 0x00276518 File Offset: 0x00274718
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

		// Token: 0x06002C42 RID: 11330 RVA: 0x0027657C File Offset: 0x0027477C
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

		// Token: 0x06002C43 RID: 11331 RVA: 0x002769F4 File Offset: 0x00274BF4
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

		// Token: 0x06002C44 RID: 11332 RVA: 0x00276A58 File Offset: 0x00274C58
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

		// Token: 0x06002C45 RID: 11333 RVA: 0x00276D18 File Offset: 0x00274F18
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

		// Token: 0x06002C46 RID: 11334 RVA: 0x00276D7C File Offset: 0x00274F7C
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

		// Token: 0x06002C47 RID: 11335 RVA: 0x00276EB0 File Offset: 0x002750B0
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

		// Token: 0x06002C48 RID: 11336 RVA: 0x0027727C File Offset: 0x0027547C
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

		// Token: 0x06002C49 RID: 11337 RVA: 0x002772E0 File Offset: 0x002754E0
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

		// Token: 0x06002C4A RID: 11338 RVA: 0x002776E4 File Offset: 0x002758E4
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

		// Token: 0x06002C4B RID: 11339 RVA: 0x00277748 File Offset: 0x00275948
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

		// Token: 0x06002C4C RID: 11340 RVA: 0x00277B2C File Offset: 0x00275D2C
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

		// Token: 0x06002C4D RID: 11341 RVA: 0x00277B90 File Offset: 0x00275D90
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

		// Token: 0x06002C4E RID: 11342 RVA: 0x00277CDC File Offset: 0x00275EDC
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

		// Token: 0x06002C4F RID: 11343 RVA: 0x00277E50 File Offset: 0x00276050
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

		// Token: 0x06002C50 RID: 11344 RVA: 0x00277EB4 File Offset: 0x002760B4
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

		// Token: 0x06002C51 RID: 11345 RVA: 0x00278244 File Offset: 0x00276444
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

		// Token: 0x06002C52 RID: 11346 RVA: 0x002782A8 File Offset: 0x002764A8
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

		// Token: 0x06002C53 RID: 11347 RVA: 0x002785DC File Offset: 0x002767DC
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

		// Token: 0x06002C54 RID: 11348 RVA: 0x00278640 File Offset: 0x00276840
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

		// Token: 0x06002C55 RID: 11349 RVA: 0x002786B8 File Offset: 0x002768B8
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

		// Token: 0x06002C56 RID: 11350 RVA: 0x00278798 File Offset: 0x00276998
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

		// Token: 0x06002C57 RID: 11351 RVA: 0x00278A88 File Offset: 0x00276C88
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

		// Token: 0x06002C58 RID: 11352 RVA: 0x00278AEC File Offset: 0x00276CEC
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

		// Token: 0x06002C59 RID: 11353 RVA: 0x00278E6C File Offset: 0x0027706C
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

		// Token: 0x06002C5A RID: 11354 RVA: 0x00278ED0 File Offset: 0x002770D0
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

		// Token: 0x06002C5B RID: 11355 RVA: 0x002791F0 File Offset: 0x002773F0
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

		// Token: 0x06002C5C RID: 11356 RVA: 0x00279254 File Offset: 0x00277454
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

		// Token: 0x06002C5D RID: 11357 RVA: 0x002792E8 File Offset: 0x002774E8
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

		// Token: 0x06002C5E RID: 11358 RVA: 0x0027945C File Offset: 0x0027765C
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

		// Token: 0x06002C5F RID: 11359 RVA: 0x002796FC File Offset: 0x002778FC
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

		// Token: 0x06002C60 RID: 11360 RVA: 0x00279760 File Offset: 0x00277960
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

		// Token: 0x06002C61 RID: 11361 RVA: 0x00279AAC File Offset: 0x00277CAC
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

		// Token: 0x06002C62 RID: 11362 RVA: 0x00279B10 File Offset: 0x00277D10
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

		// Token: 0x06002C63 RID: 11363 RVA: 0x00279E00 File Offset: 0x00278000
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

		// Token: 0x06002C64 RID: 11364 RVA: 0x00279E64 File Offset: 0x00278064
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

		// Token: 0x06002C65 RID: 11365 RVA: 0x0027A12C File Offset: 0x0027832C
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

		// Token: 0x06002C66 RID: 11366 RVA: 0x0027A190 File Offset: 0x00278390
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

		// Token: 0x06002C67 RID: 11367 RVA: 0x0027A628 File Offset: 0x00278828
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

		// Token: 0x06002C68 RID: 11368 RVA: 0x0027A6A0 File Offset: 0x002788A0
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

		// Token: 0x06002C69 RID: 11369 RVA: 0x0027A950 File Offset: 0x00278B50
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

		// Token: 0x06002C6A RID: 11370 RVA: 0x0027A9B4 File Offset: 0x00278BB4
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

		// Token: 0x06002C6B RID: 11371 RVA: 0x0027AAF8 File Offset: 0x00278CF8
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

		// Token: 0x06002C6C RID: 11372 RVA: 0x0027AB5C File Offset: 0x00278D5C
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

		// Token: 0x04003A63 RID: 14947
		private static long lastJieRiProcessTicks = 0L;

		// Token: 0x04003A64 RID: 14948
		private static int JieRiState = 0;

		// Token: 0x04003A65 RID: 14949
		private static int HefuState = 1;

		// Token: 0x04003A66 RID: 14950
		private static int ThemeState = 0;

		// Token: 0x04003A67 RID: 14951
		private static Dictionary<int, WLoginItem> _WLoginDict = new Dictionary<int, WLoginItem>();

		// Token: 0x04003A68 RID: 14952
		private static Dictionary<int, MOnlineTimeItem> _MonthTimeDict = new Dictionary<int, MOnlineTimeItem>();

		// Token: 0x04003A69 RID: 14953
		private static Dictionary<int, NewStepItem> _NewStepDict = new Dictionary<int, NewStepItem>();

		// Token: 0x04003A6A RID: 14954
		private static Dictionary<int, CombatAwardItem> _CombatAwardlDict = new Dictionary<int, CombatAwardItem>();

		// Token: 0x04003A6B RID: 14955
		private static Dictionary<int, Dictionary<int, UpLevelItem>> _UpLevelDict = new Dictionary<int, Dictionary<int, UpLevelItem>>();

		// Token: 0x04003A6C RID: 14956
		private static object _BigAwardItemMutex = new object();

		// Token: 0x04003A6D RID: 14957
		private static BigAwardItem _BigAwardItem = null;

		// Token: 0x04003A6E RID: 14958
		private static object _SongLiItemMutex = new object();

		// Token: 0x04003A6F RID: 14959
		private static SongLiItem _SongLiItem = null;

		// Token: 0x04003A70 RID: 14960
		private static DateTime _LimitTimeLoginStartTime = new DateTime(1971, 1, 1);

		// Token: 0x04003A71 RID: 14961
		private static DateTime _LimitTimeLoginEndTime = new DateTime(1971, 1, 1);

		// Token: 0x04003A72 RID: 14962
		private static Dictionary<int, LimitTimeLoginItem> _LimitTimeLoginDict = new Dictionary<int, LimitTimeLoginItem>();

		// Token: 0x04003A73 RID: 14963
		private static Dictionary<int, EveryDayOnLineAward> _EveryDayOnLineAwardDict = new Dictionary<int, EveryDayOnLineAward>();

		// Token: 0x04003A74 RID: 14964
		private static Dictionary<int, SeriesLoginAward> _SeriesLoginAward = new Dictionary<int, SeriesLoginAward>();

		// Token: 0x04003A75 RID: 14965
		private static FirstChongZhiGift _FirstChongZhiActivity = null;

		// Token: 0x04003A76 RID: 14966
		private static object _FirstChongZhiActivityMutex = new object();

		// Token: 0x04003A77 RID: 14967
		private static InputFanLiActivity _InputFanLiActivity = null;

		// Token: 0x04003A78 RID: 14968
		private static object _InputFanLiActivityMutex = new object();

		// Token: 0x04003A79 RID: 14969
		private static InputSongActivity _InputSongActivity = null;

		// Token: 0x04003A7A RID: 14970
		private static object _InputSongActivityMutex = new object();

		// Token: 0x04003A7B RID: 14971
		private static KingActivity _InputKingActivity = null;

		// Token: 0x04003A7C RID: 14972
		private static object _InputKingActivityMutex = new object();

		// Token: 0x04003A7D RID: 14973
		private static KingActivity _LevelKingActivity = null;

		// Token: 0x04003A7E RID: 14974
		private static object _LevelKingActivityMutex = new object();

		// Token: 0x04003A7F RID: 14975
		private static KingActivity _EquipKingActivity = null;

		// Token: 0x04003A80 RID: 14976
		private static object _EquipKingActivityMutex = new object();

		// Token: 0x04003A81 RID: 14977
		private static KingActivity _HorseKingActivity = null;

		// Token: 0x04003A82 RID: 14978
		private static object _HorseKingActivityMutex = new object();

		// Token: 0x04003A83 RID: 14979
		private static KingActivity _JingMaiKingActivity = null;

		// Token: 0x04003A84 RID: 14980
		private static object _JingMaiKingActivityMutex = new object();

		// Token: 0x04003A85 RID: 14981
		private static KingActivity _XinXiaofeiKingActivity = null;

		// Token: 0x04003A86 RID: 14982
		private static object _XinXiaofeiKingMutex = new object();

		// Token: 0x04003A87 RID: 14983
		public static Dictionary<int, UpLevelAwardItem> UpLevelAwardItemDict = null;

		// Token: 0x04003A88 RID: 14984
		public static Dictionary<int, KaiFuGiftItem> KaiFuGiftItemDict = null;

		// Token: 0x04003A89 RID: 14985
		private static int LastProcessGetKaiFuGiftAwardDayID = 0;

		// Token: 0x04003A8A RID: 14986
		public static int ProcessKaiFuGiftAwardHour = 12;

		// Token: 0x04003A8B RID: 14987
		private static int LastAutoAddKaiFuGiftRoleNumDayID = TimeUtil.NowDateTime().DayOfYear;

		// Token: 0x04003A8C RID: 14988
		private static JieriActivityConfig _JieriActivityConfig = null;

		// Token: 0x04003A8D RID: 14989
		private static object _JieriActivityConfigMutex = new object();

		// Token: 0x04003A8E RID: 14990
		private static ThemeActivityConfig _ThemeActivityConfig = null;

		// Token: 0x04003A8F RID: 14991
		private static object _ThemeActivityConfigMutex = new object();

		// Token: 0x04003A90 RID: 14992
		private static ThemeDaLiBaoActivity _ThemeDaLiBaoActivity = null;

		// Token: 0x04003A91 RID: 14993
		private static object _ThemeDaLiBaoActivityMutex = new object();

		// Token: 0x04003A92 RID: 14994
		private static ThemeDuiHuanActivity _ThemeDuiHuanActivity = null;

		// Token: 0x04003A93 RID: 14995
		private static object _ThemeDuiHuanActivityMutex = new object();

		// Token: 0x04003A94 RID: 14996
		private static ThemeZhiGouActivity _ThemeZhiGouActivity = null;

		// Token: 0x04003A95 RID: 14997
		private static object _ThemeZhiGouActivityMutex = new object();

		// Token: 0x04003A96 RID: 14998
		private static JieriDaLiBaoActivity _JieriDaLiBaoActivity = null;

		// Token: 0x04003A97 RID: 14999
		private static object _JieriDaLiBaoActivityMutex = new object();

		// Token: 0x04003A98 RID: 15000
		private static JieRiDengLuActivity _JieRiDengLuActivity = null;

		// Token: 0x04003A99 RID: 15001
		private static object _JieriDengLuActivityMutex = new object();

		// Token: 0x04003A9A RID: 15002
		private static JieriVIPActivity _JieriVIPActivity = null;

		// Token: 0x04003A9B RID: 15003
		private static object _JieriVIPActivityMutex = new object();

		// Token: 0x04003A9C RID: 15004
		private static JieriGiveActivity _JieriGiveActivity = null;

		// Token: 0x04003A9D RID: 15005
		private static object _JieriGiveMutex = new object();

		// Token: 0x04003A9E RID: 15006
		private static JieriRecvActivity _JieriRecvActivity = null;

		// Token: 0x04003A9F RID: 15007
		private static object _JieriRecvMutex = new object();

		// Token: 0x04003AA0 RID: 15008
		private static JieRiGiveKingActivity _JieriGiveKingActivity = null;

		// Token: 0x04003AA1 RID: 15009
		private static object _JieriGiveKingMutex = new object();

		// Token: 0x04003AA2 RID: 15010
		private static JieRiRecvKingActivity _JieriRecvKingActivity = null;

		// Token: 0x04003AA3 RID: 15011
		private static object _JieriRecvKingMutex = new object();

		// Token: 0x04003AA4 RID: 15012
		private static JieRiFuLiActivity _JieriFuLiActivity = null;

		// Token: 0x04003AA5 RID: 15013
		private static object _JieriFuLiMutex = new object();

		// Token: 0x04003AA6 RID: 15014
		private static JieriCZSongActivity _JieriCZSongActivity = null;

		// Token: 0x04003AA7 RID: 15015
		private static object _JieriCZSongActivityMutex = new object();

		// Token: 0x04003AA8 RID: 15016
		private static JieRiCZQGActivity _JieRiCZQGActivity = null;

		// Token: 0x04003AA9 RID: 15017
		private static object _JieRiCZQGActivityMutex = new object();

		// Token: 0x04003AAA RID: 15018
		private static OneDollarBuyActivity _OneDollarBuyActivity = null;

		// Token: 0x04003AAB RID: 15019
		private static object _OneDollarBuyActivityMutex = new object();

		// Token: 0x04003AAC RID: 15020
		private static OneDollarChongZhi _OneDollarChongZhi = null;

		// Token: 0x04003AAD RID: 15021
		private static object _OneDollarChongZhiMutex = new object();

		// Token: 0x04003AAE RID: 15022
		private static InputFanLiNew _InputFanLiNew = null;

		// Token: 0x04003AAF RID: 15023
		private static object _InputFanLiNewMutex = new object();

		// Token: 0x04003AB0 RID: 15024
		private static RegressActiveOpen _RegressActiveOpen = null;

		// Token: 0x04003AB1 RID: 15025
		private static object _RegressActiveOpenMutex = new object();

		// Token: 0x04003AB2 RID: 15026
		private static RegressActiveSignGift _RegressActiveSignGift = null;

		// Token: 0x04003AB3 RID: 15027
		private static object _RegressActiveSignGiftMutex = new object();

		// Token: 0x04003AB4 RID: 15028
		private static RegressActiveTotalRecharge _RegressActiveTotalRecharge = null;

		// Token: 0x04003AB5 RID: 15029
		private static object _RegressActiveTotalRechargeMutex = new object();

		// Token: 0x04003AB6 RID: 15030
		private static RegressActiveDayBuy _RegressActiveDayBuy = null;

		// Token: 0x04003AB7 RID: 15031
		private static object _RegressActiveDayBuyMutex = new object();

		// Token: 0x04003AB8 RID: 15032
		private static RegressActiveStore _RegressActiveStore = null;

		// Token: 0x04003AB9 RID: 15033
		private static object _RegressActiveStoreMutex = new object();

		// Token: 0x04003ABA RID: 15034
		private static JieriSuperInputActivity _JieriSuperInput = null;

		// Token: 0x04003ABB RID: 15035
		private static object _JieriSuperInputMutex = new object();

		// Token: 0x04003ABC RID: 15036
		private static JieriVIPYouHuiActivity _JieriVIPYouHuiActivity = null;

		// Token: 0x04003ABD RID: 15037
		private static object _JieriVIPYouHuiActMutex = new object();

		// Token: 0x04003ABE RID: 15038
		private static SpecialActivity _SpecialActivity = null;

		// Token: 0x04003ABF RID: 15039
		private static object _SpecialActivityMutex = new object();

		// Token: 0x04003AC0 RID: 15040
		private static SpecPriorityActivity _SpecPriorityActivity = null;

		// Token: 0x04003AC1 RID: 15041
		private static object _SpecPriorityActivityMutex = new object();

		// Token: 0x04003AC2 RID: 15042
		private static EverydayActivity _EverydayActivity = null;

		// Token: 0x04003AC3 RID: 15043
		private static object _EverydayActivityMutex = new object();

		// Token: 0x04003AC4 RID: 15044
		private static JieriIPointsExchgActivity _JieriIPointsExchgActivity = null;

		// Token: 0x04003AC5 RID: 15045
		private static object _JieriIPointsExchgActivityMutex = new object();

		// Token: 0x04003AC6 RID: 15046
		private static WeedEndInputActivity _WeedEndInputActivity = null;

		// Token: 0x04003AC7 RID: 15047
		private static object _WeedEndInputActivityMutex = new object();

		// Token: 0x04003AC8 RID: 15048
		private static JieRiLeiJiCZActivity _JieRiLeiJiCZActivity = null;

		// Token: 0x04003AC9 RID: 15049
		private static object _JieRiLeiJiCZActivityMutex = new object();

		// Token: 0x04003ACA RID: 15050
		private static JieRiTotalConsumeActivity _JieRiTotalConsumeActivity = null;

		// Token: 0x04003ACB RID: 15051
		private static object _JieRiTotalConsumeActivityMutex = new object();

		// Token: 0x04003ACC RID: 15052
		private static JieRiMeiRiLeiJiActivity _JieRiMeiRiLeiJiActivity = null;

		// Token: 0x04003ACD RID: 15053
		private static object _JieRiMeiRiLeiJiActivityMutex = new object();

		// Token: 0x04003ACE RID: 15054
		private static JieRiMultAwardActivity _JieRiMultAwardActivity = null;

		// Token: 0x04003ACF RID: 15055
		private static object _JieRiMultAwardActivityMutex = new object();

		// Token: 0x04003AD0 RID: 15056
		private static JieRiZiKaLiaBaoActivity _JieRiZiKaLiaBaoActivity = null;

		// Token: 0x04003AD1 RID: 15057
		private static object _JieRiZiKaLiaBaoActivityMutex = new object();

		// Token: 0x04003AD2 RID: 15058
		private static KingActivity _JieRiXiaoFeiKingActivity = null;

		// Token: 0x04003AD3 RID: 15059
		private static object _JieRiXiaoFeiKingActivityMutex = new object();

		// Token: 0x04003AD4 RID: 15060
		private static KingActivity _JieRiCZKingActivity = null;

		// Token: 0x04003AD5 RID: 15061
		private static object _JieRiCZKingActivityMutex = new object();

		// Token: 0x04003AD6 RID: 15062
		private static HuodongCachingMgr.TotalChargeActivity _TotalChargeActivity = null;

		// Token: 0x04003AD7 RID: 15063
		private static object _TotalChargeActivityMutex = new object();

		// Token: 0x04003AD8 RID: 15064
		private static HuodongCachingMgr.TotalConsumeActivity _TotalConsumeActivity = null;

		// Token: 0x04003AD9 RID: 15065
		private static object _TotalConsumeActivityMutex = new object();

		// Token: 0x04003ADA RID: 15066
		private static JieriFanLiActivity[] _JieriWingFanliAct = new JieriFanLiActivity[11];

		// Token: 0x04003ADB RID: 15067
		private static object _JieriWingFanliActMutex = new object();

		// Token: 0x04003ADC RID: 15068
		private static object _JieriLianXuChargeMutex = new object();

		// Token: 0x04003ADD RID: 15069
		private static JieriLianXuChargeActivity _JieriLianXuChargeAct = null;

		// Token: 0x04003ADE RID: 15070
		private static object _JieriPlatChargeKingMutex = new object();

		// Token: 0x04003ADF RID: 15071
		private static JieriPlatChargeKing _JieriPlatChargeKingAct = null;

		// Token: 0x04003AE0 RID: 15072
		private static object _JieriPCKingEveryDayMutex = new object();

		// Token: 0x04003AE1 RID: 15073
		private static JieriPlatChargeKingEveryDay _JieriPCKingEveryDayAct = null;

		// Token: 0x04003AE2 RID: 15074
		private static object _DanBiChongZhiMutex = new object();

		// Token: 0x04003AE3 RID: 15075
		private static DanBiChongZhiActivity _DanBiChongZhiAct = null;

		// Token: 0x04003AE4 RID: 15076
		private static HeFuActivityConfig _HeFuActivityConfig = null;

		// Token: 0x04003AE5 RID: 15077
		private static object _HeFuActivityConfigMutex = new object();

		// Token: 0x04003AE6 RID: 15078
		private static HeFuLoginActivity _HeFuLoginActivity = null;

		// Token: 0x04003AE7 RID: 15079
		private static object _HeFuLoginActivityMutex = new object();

		// Token: 0x04003AE8 RID: 15080
		private static HeFuRechargeActivity _HeFuRechargeActivity = null;

		// Token: 0x04003AE9 RID: 15081
		private static object _HeFuRechargeActivityMutex = new object();

		// Token: 0x04003AEA RID: 15082
		private static HeFuTotalLoginActivity _HeFuTotalLoginActivity = null;

		// Token: 0x04003AEB RID: 15083
		private static object _HeFuTotalLoginActivityMutex = new object();

		// Token: 0x04003AEC RID: 15084
		private static HeFuPKKingActivity _HeFuPKKingActivity = null;

		// Token: 0x04003AED RID: 15085
		private static object _HeFuAwardTimeActivityMutex = new object();

		// Token: 0x04003AEE RID: 15086
		private static HeFuAwardTimesActivity _HeFuAwardTimeActivity = null;

		// Token: 0x04003AEF RID: 15087
		private static HeFuLuoLanActivity _HeFuLuoLanActivity = null;

		// Token: 0x04003AF0 RID: 15088
		private static object _HeFuLuoLanActivityMutex = new object();

		// Token: 0x04003AF1 RID: 15089
		private static object _HeFuPKKingActivityMutex = new object();

		// Token: 0x04003AF2 RID: 15090
		private static HeFuWCKingActivity _HeFuWCKingActivity = null;

		// Token: 0x04003AF3 RID: 15091
		private static object _HeFuWCKingActivityMutex = new object();

		// Token: 0x04003AF4 RID: 15092
		private static XinFanLiActivity _XinFanLiActivity = null;

		// Token: 0x04003AF5 RID: 15093
		private static object _XinFanLiActivityMutex = new object();

		// Token: 0x04003AF6 RID: 15094
		private static object _MeiRiChongZhiHaoLiActivityMutex = new object();

		// Token: 0x04003AF7 RID: 15095
		private static MeiRiChongZhiActivity _MeiRiChongZhiHaoLiActivity = null;

		// Token: 0x04003AF8 RID: 15096
		private static object _ChongJiHaoLiActivityMutex = new object();

		// Token: 0x04003AF9 RID: 15097
		private static ChongJiHaoLiActivity _ChongJiHaoLiActivity = null;

		// Token: 0x04003AFA RID: 15098
		private static object _ShenZhuangJiQingHuiKuiHaoLiActivityMutex = new object();

		// Token: 0x04003AFB RID: 15099
		private static ShenZhuangHuiKuiHaoLiActivity _ShenZhuangJiQingHuiKuiHaoLiActivity = null;

		// Token: 0x04003AFC RID: 15100
		private static object _YueDuZhuanPanActivityMutex = new object();

		// Token: 0x04003AFD RID: 15101
		private static YueDuZhuanPanActivity _YueDuZhuanPanActivity = null;

		// Token: 0x02000721 RID: 1825
		private static class GiftCodeFlags
		{
			// Token: 0x04003AFF RID: 15103
			public const int Local = 1;

			// Token: 0x04003B00 RID: 15104
			public const int Center = 2;
		}

		// Token: 0x02000722 RID: 1826
		public class TotalConsumeActivity : KingActivity
		{
			// Token: 0x06002C70 RID: 11376 RVA: 0x0027B23C File Offset: 0x0027943C
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

		// Token: 0x02000723 RID: 1827
		public class TotalChargeActivity : KingActivity
		{
			// Token: 0x06002C72 RID: 11378 RVA: 0x0027B2D4 File Offset: 0x002794D4
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
