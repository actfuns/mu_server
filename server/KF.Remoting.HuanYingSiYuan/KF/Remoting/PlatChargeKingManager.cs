using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.Data;
using Tmsk.Tools;

namespace KF.Remoting
{
	// Token: 0x02000039 RID: 57
	internal class PlatChargeKingManager
	{
		// Token: 0x06000299 RID: 665 RVA: 0x00025F3C File Offset: 0x0002413C
		public void Update()
		{
			try
			{
				lock (this.Mutex)
				{
					if (this.IsNeedDownload())
					{
						bool flag = false;
						List<InputKingPaiHangDataEx> list = new List<InputKingPaiHangDataEx>();
						if (KuaFuServerManager.GetPlatChargeKingUrl != null)
						{
							for (int i = 0; i < KuaFuServerManager.GetPlatChargeKingUrl.Length; i++)
							{
								ClientServerListData clientListData = new ClientServerListData();
								clientListData.lTime = TimeUtil.NOW();
								clientListData.strMD5 = MD5Helper.get_md5_string(ConstData.HTTP_MD5_KEY + clientListData.lTime.ToString());
								byte[] clientBytes = DataHelper2.ObjectToBytes<ClientServerListData>(clientListData);
								byte[] responseData = WebHelper.RequestByPost(KuaFuServerManager.GetPlatChargeKingUrl[i], clientBytes, 2000, 30000);
								if (responseData == null)
								{
									flag = true;
									break;
								}
								InputKingPaiHangDataEx tmpRankEx = DataHelper2.BytesToObject<InputKingPaiHangDataEx>(responseData, 0, responseData.Length);
								if (tmpRankEx == null)
								{
									flag = true;
									break;
								}
								list.Add(tmpRankEx);
							}
							if (flag)
							{
								this.rankEx = new InputKingPaiHangDataEx();
							}
							else
							{
								this.rankEx = this.MergePlatfromInputKingList(list);
							}
						}
					}
					if (this.IsNeedDownloadEveryDay())
					{
						if (KuaFuServerManager.GetPlatChargeKingUrl_EveryDay != null)
						{
							bool flag = false;
							Dictionary<int, List<InputKingPaiHangDataEx>> dict = new Dictionary<int, List<InputKingPaiHangDataEx>>();
							for (int i = 0; i < KuaFuServerManager.GetPlatChargeKingUrl_EveryDay.Length; i++)
							{
								List<InputKingPaiHangDataEx> tempRankExList = new List<InputKingPaiHangDataEx>();
								if (this.MeiRiPCKingFromDate < this.MeiRiPCKingToDate)
								{
									DateTime timeLoop = this.MeiRiPCKingFromDate;
									while (timeLoop < this.MeiRiPCKingToDate && timeLoop < TimeUtil.NowDateTime())
									{
										InputKingPaiHangDataEx tmpRankEx = null;
										byte[] clientBytes = DataHelper2.ObjectToBytes<InputKingPaiHangDataSearch>(new InputKingPaiHangDataSearch
										{
											startDate = timeLoop.ToString("yyyy-MM-dd HH:mm:ss"),
											endDate = timeLoop.AddDays(1.0).AddSeconds(-1.0).ToString("yyyy-MM-dd HH:mm:ss")
										});
										byte[] responseData = WebHelper.RequestByPost(KuaFuServerManager.GetPlatChargeKingUrl_EveryDay[i], clientBytes, 2000, 30000);
										if (responseData != null)
										{
											tmpRankEx = DataHelper2.BytesToObject<InputKingPaiHangDataEx>(responseData, 0, responseData.Length);
										}
										if (null != tmpRankEx)
										{
											tempRankExList.Add(tmpRankEx);
										}
										else
										{
											tempRankExList.Add(new InputKingPaiHangDataEx());
											flag = true;
										}
										if (flag)
										{
											break;
										}
										timeLoop = timeLoop.AddDays(1.0);
									}
									if (flag)
									{
										break;
									}
									if (!dict.ContainsKey(i))
									{
										dict.Add(i, tempRankExList);
									}
								}
							}
							if (flag)
							{
								this.rankExList = new List<InputKingPaiHangDataEx>();
							}
							else
							{
								this.rankExList = this.MergePlatfromInputKingListEveryDay(dict);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "PlatChargeKingManager.Update exception", ex, true);
			}
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0002629C File Offset: 0x0002449C
		public InputKingPaiHangDataEx GetRankEx()
		{
			InputKingPaiHangDataEx result = null;
			lock (this.Mutex)
			{
				this.bHasVisitor = true;
				result = this.rankEx;
			}
			return result;
		}

		// Token: 0x0600029B RID: 667 RVA: 0x000262F8 File Offset: 0x000244F8
		public List<InputKingPaiHangDataEx> GetRankExList(DateTime fromDate, DateTime toDate)
		{
			List<InputKingPaiHangDataEx> result = null;
			List<InputKingPaiHangDataEx> result2;
			if (fromDate >= toDate || fromDate < this.MeiRiPCKingFromDate || toDate < this.MeiRiPCKingToDate)
			{
				result2 = result;
			}
			else
			{
				lock (this.Mutex)
				{
					this.MeiRiPCKingFromDate = fromDate;
					this.MeiRiPCKingToDate = toDate;
					this.bHasVisitorEvery = true;
					result = this.rankExList;
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600029C RID: 668 RVA: 0x00026464 File Offset: 0x00024664
		public InputKingPaiHangDataEx MergePlatfromInputKingList(List<InputKingPaiHangDataEx> EveryPlatfrom)
		{
			InputKingPaiHangDataEx InputRes = new InputKingPaiHangDataEx();
			InputKingPaiHangDataEx result;
			if (EveryPlatfrom == null)
			{
				result = InputRes;
			}
			else
			{
				bool flag = false;
				string RankTime = "";
				string StartTime = "";
				string EndTime = "";
				List<InputKingPaiHangData> list = new List<InputKingPaiHangData>();
				foreach (InputKingPaiHangDataEx it in EveryPlatfrom)
				{
					if (it != null)
					{
						if (it.ListData != null)
						{
							if (!flag && !string.IsNullOrEmpty(it.RankTime) && !string.IsNullOrEmpty(it.StartTime) && !string.IsNullOrEmpty(it.EndTime))
							{
								RankTime = it.RankTime;
								StartTime = it.StartTime;
								EndTime = it.EndTime;
								flag = true;
							}
							foreach (InputKingPaiHangData iter in it.ListData)
							{
								if (iter != null)
								{
									list.Add(iter);
								}
							}
						}
					}
				}
				list.Sort(delegate(InputKingPaiHangData _left, InputKingPaiHangData _right)
				{
					int result2;
					if (_left.PaiHangValue > _right.PaiHangValue)
					{
						result2 = -1;
					}
					else if (_left.PaiHangValue < _right.PaiHangValue)
					{
						result2 = 1;
					}
					else if (!string.IsNullOrEmpty(_left.InputTime))
					{
						int res = _left.InputTime.CompareTo(_right.InputTime);
						if (0 == res)
						{
							if (!string.IsNullOrEmpty(_left.UserID))
							{
								result2 = _left.UserID.CompareTo(_right.UserID);
							}
							else
							{
								result2 = 1;
							}
						}
						else
						{
							result2 = res;
						}
					}
					else if (!string.IsNullOrEmpty(_left.UserID))
					{
						result2 = _left.UserID.CompareTo(_right.UserID);
					}
					else
					{
						result2 = 1;
					}
					return result2;
				});
				InputRes.ListData = list;
				InputRes.RankTime = RankTime;
				InputRes.StartTime = StartTime;
				InputRes.EndTime = EndTime;
				result = InputRes;
			}
			return result;
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00026624 File Offset: 0x00024824
		public List<InputKingPaiHangDataEx> MergePlatfromInputKingListEveryDay(Dictionary<int, List<InputKingPaiHangDataEx>> EveryPlatfrom)
		{
			List<InputKingPaiHangDataEx> res = new List<InputKingPaiHangDataEx>();
			List<InputKingPaiHangDataEx> result;
			if (EveryPlatfrom == null)
			{
				result = res;
			}
			else
			{
				Dictionary<int, List<InputKingPaiHangDataEx>> dict = new Dictionary<int, List<InputKingPaiHangDataEx>>();
				int temp = 0;
				foreach (List<InputKingPaiHangDataEx> iter in EveryPlatfrom.Values)
				{
					foreach (InputKingPaiHangDataEx it in iter)
					{
						temp++;
						if (dict.ContainsKey(temp))
						{
							dict[temp].Add(it);
						}
						else
						{
							dict.Add(temp, new List<InputKingPaiHangDataEx>
							{
								it
							});
						}
					}
					temp = 0;
				}
				foreach (KeyValuePair<int, List<InputKingPaiHangDataEx>> iter2 in dict)
				{
					InputKingPaiHangDataEx OnePlatfrom = this.MergePlatfromInputKingList(iter2.Value);
					if (OnePlatfrom != null)
					{
						res.Add(OnePlatfrom);
					}
					else
					{
						res.Add(new InputKingPaiHangDataEx());
					}
				}
				result = res;
			}
			return result;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x000267AC File Offset: 0x000249AC
		public long QueryHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid)
		{
			long hasgettimes = 0L;
			string lastgettime = "";
			lock (this.Mutex)
			{
				KuaFuCopyDbMgr.Instance.GetAwardHistoryForUser(userid, actType, huoDongKeyStr, out hasgettimes, out lastgettime);
			}
			return hasgettimes;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00026818 File Offset: 0x00024A18
		public int UpdateHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid, int extTag)
		{
			long hasgettimes = 0L;
			string lastgettime = "";
			int ret = 0;
			lock (this.Mutex)
			{
				int histForRole = KuaFuCopyDbMgr.Instance.GetAwardHistoryForUser(userid, actType, huoDongKeyStr, out hasgettimes, out lastgettime);
				hasgettimes |= 1L << extTag - 1;
				lastgettime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (histForRole < 0)
				{
					ret = KuaFuCopyDbMgr.Instance.AddHongDongAwardRecordForUser(userid, actType, huoDongKeyStr, hasgettimes, lastgettime);
				}
				else
				{
					ret = KuaFuCopyDbMgr.Instance.UpdateHongDongAwardRecordForUser(userid, actType, huoDongKeyStr, hasgettimes, lastgettime);
				}
			}
			return ret;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x000268DC File Offset: 0x00024ADC
		private bool IsNeedDownload()
		{
			return this.bHasVisitor;
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00026900 File Offset: 0x00024B00
		private bool IsNeedDownloadEveryDay()
		{
			return this.bHasVisitorEvery;
		}

		// Token: 0x04000168 RID: 360
		private object Mutex = new object();

		// Token: 0x04000169 RID: 361
		private InputKingPaiHangDataEx rankEx = null;

		// Token: 0x0400016A RID: 362
		private bool bHasVisitor = false;

		// Token: 0x0400016B RID: 363
		private DateTime MeiRiPCKingFromDate = DateTime.MinValue;

		// Token: 0x0400016C RID: 364
		private DateTime MeiRiPCKingToDate = DateTime.MinValue;

		// Token: 0x0400016D RID: 365
		private List<InputKingPaiHangDataEx> rankExList = new List<InputKingPaiHangDataEx>();

		// Token: 0x0400016E RID: 366
		private bool bHasVisitorEvery = false;
	}
}
