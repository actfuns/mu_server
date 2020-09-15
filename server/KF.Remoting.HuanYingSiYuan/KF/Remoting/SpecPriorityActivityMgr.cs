using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	// Token: 0x02000036 RID: 54
	public class SpecPriorityActivityMgr
	{
		// Token: 0x06000266 RID: 614 RVA: 0x00024760 File Offset: 0x00022960
		public static SpecPriorityActivityMgr Instance()
		{
			return SpecPriorityActivityMgr._instance;
		}

		// Token: 0x06000267 RID: 615 RVA: 0x00024778 File Offset: 0x00022978
		public void InitConfig()
		{
			try
			{
				lock (this.Mutex)
				{
					string fileName = "Config/TeQuanTiaoJian.xml";
					string fullPathFileName = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
					this.SpecPConditionList.Clear();
					XElement xml = ConfigHelper.Load(fullPathFileName);
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						SpecPConditionConfig myData = new SpecPConditionConfig();
						myData.GroupID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ID", 0L);
						string FromDate = ConfigHelper.GetElementAttributeValue(xmlItem, "KaiQiShiJian", "");
						if (!string.IsNullOrEmpty(FromDate))
						{
							myData.FromDate = DateTime.Parse(FromDate);
						}
						else
						{
							myData.FromDate = DateTime.Parse("2008-08-08 08:08:08");
						}
						string ToDate = ConfigHelper.GetElementAttributeValue(xmlItem, "JieShuShiJian", "");
						if (!string.IsNullOrEmpty(ToDate))
						{
							myData.ToDate = DateTime.Parse(ToDate);
						}
						else
						{
							myData.ToDate = DateTime.Parse("2028-08-08 08:08:08");
						}
						myData.ConditionType = (SpecPConditionType)ConfigHelper.GetElementAttributeValueLong(xmlItem, "TiaoJianLeiXing", 0L);
						this.SpecPConditionList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x06000268 RID: 616 RVA: 0x00024950 File Offset: 0x00022B50
		public void LoadDatabase(DateTime now)
		{
			try
			{
				lock (this.Mutex)
				{
					this.LastUpdateDayID = TimeUtil.GetOffsetDay(now);
					this.InitActivityConditionInfo(now, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "SpecPriorityActivityMgr.LoadDatabase failed!", ex, true);
			}
		}

		// Token: 0x06000269 RID: 617 RVA: 0x000249D0 File Offset: 0x00022BD0
		public int SpecPriority_ModifyActivityConditionNum(int key, int add)
		{
			try
			{
				lock (this.Mutex)
				{
					int hastimes = 0;
					if (this.ActConditionInfoDict.TryGetValue(key, out hastimes))
					{
						this.ActConditionInfoDict[key] = hastimes + add;
					}
					else
					{
						this.ActConditionInfoDict[key] = add;
					}
					this.SaveActivityConditionInfo();
					return 0;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return -11000;
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00024A88 File Offset: 0x00022C88
		public SpecPrioritySyncData SpecPriority_GetActivityConditionInfo()
		{
			SpecPrioritySyncData result;
			lock (this.Mutex)
			{
				result = new SpecPrioritySyncData
				{
					ActConditionInfoDict = new Dictionary<int, int>(this.ActConditionInfoDict)
				};
			}
			return result;
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00024B18 File Offset: 0x00022D18
		private void SaveActivityConditionInfo()
		{
			lock (this.Mutex)
			{
				using (Dictionary<int, int>.Enumerator enumerator = this.ActConditionInfoDict.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, int> kvp = enumerator.Current;
						SpecPConditionConfig conConfig = this.SpecPConditionList.Find(delegate(SpecPConditionConfig x)
						{
							int groupID = x.GroupID;
							KeyValuePair<int, int> kvp2 = kvp;
							return groupID == kvp2.Key;
						});
						if (null != conConfig)
						{
							string huoDongKeyStr = string.Format("{0}_{1}_{2}", conConfig.FromDate.ToString("yyyy-MM-dd HH:mm:ss"), conConfig.ToDate.ToString("yyyy-MM-dd HH:mm:ss"), conConfig.GroupID);
							int specPriorityActivityType = this.SpecPriorityActivityType;
							string huoDongKeyStr2 = huoDongKeyStr;
							string userid = "0";
							KeyValuePair<int, int> kvp3 = kvp;
							this.UpdateHuodongAwardUserHist(specPriorityActivityType, huoDongKeyStr2, userid, kvp3.Value);
						}
					}
				}
			}
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00024CA4 File Offset: 0x00022EA4
		private void InitActivityConditionInfo(DateTime now, bool launch = false)
		{
			lock (this.Mutex)
			{
				List<SpecPConditionConfig> specpList = this.CalSpecPConditionListByNow(now);
				List<int> removeList = new List<int>();
				using (Dictionary<int, int>.Enumerator enumerator = this.ActConditionInfoDict.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, int> kvp = enumerator.Current;
						if (!specpList.Exists(delegate(SpecPConditionConfig x)
						{
							int groupID = x.GroupID;
							KeyValuePair<int, int> kvp2 = kvp;
							return groupID == kvp2.Key;
						}))
						{
							List<int> list = removeList;
							KeyValuePair<int, int> kvp3 = kvp;
							list.Add(kvp3.Key);
						}
					}
				}
				foreach (int key in removeList)
				{
					this.ActConditionInfoDict.Remove(key);
				}
				foreach (SpecPConditionConfig item in specpList)
				{
					if (!this.ActConditionInfoDict.ContainsKey(item.GroupID))
					{
						this.ActConditionInfoDict[item.GroupID] = 0;
					}
				}
				if (launch)
				{
					Dictionary<int, int> TempConditionInfoDict = new Dictionary<int, int>(this.ActConditionInfoDict);
					using (Dictionary<int, int>.Enumerator enumerator = TempConditionInfoDict.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<int, int> kvp = enumerator.Current;
							SpecPConditionConfig conConfig = specpList.Find(delegate(SpecPConditionConfig x)
							{
								int groupID = x.GroupID;
								KeyValuePair<int, int> kvp4 = kvp;
								return groupID == kvp4.Key;
							});
							if (null != conConfig)
							{
								string huoDongKeyStr = string.Format("{0}_{1}_{2}", conConfig.FromDate.ToString("yyyy-MM-dd HH:mm:ss"), conConfig.ToDate.ToString("yyyy-MM-dd HH:mm:ss"), conConfig.GroupID);
								long hasgettimes = this.QueryHuodongAwardUserHist(this.SpecPriorityActivityType, huoDongKeyStr, "0");
								Dictionary<int, int> actConditionInfoDict = this.ActConditionInfoDict;
								KeyValuePair<int, int> kvp3 = kvp;
								actConditionInfoDict[kvp3.Key] = (int)hasgettimes;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00024FD0 File Offset: 0x000231D0
		private List<SpecPConditionConfig> CalSpecPConditionListByNow(DateTime now)
		{
			List<SpecPConditionConfig> result;
			lock (this.Mutex)
			{
				result = this.SpecPConditionList.FindAll((SpecPConditionConfig x) => x.FromDate <= now && now <= x.ToDate);
			}
			return result;
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00025048 File Offset: 0x00023248
		public void Update(DateTime now)
		{
			try
			{
				int CurrentDayID = TimeUtil.GetOffsetDay(now);
				if (CurrentDayID != this.LastUpdateDayID)
				{
					this.InitActivityConditionInfo(now, false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "SpecPriorityActivityMgr.Update failed!", ex, true);
			}
		}

		// Token: 0x0600026F RID: 623 RVA: 0x000250A0 File Offset: 0x000232A0
		private long QueryHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid)
		{
			long hasgettimes = 0L;
			string lastgettime = "";
			lock (this.Mutex)
			{
				KuaFuCopyDbMgr.Instance.GetAwardHistoryForUser(userid, actType, huoDongKeyStr, out hasgettimes, out lastgettime);
			}
			return hasgettimes;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0002510C File Offset: 0x0002330C
		private int UpdateHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid, int extTag)
		{
			long hasgettimes = 0L;
			string lastgettime = "";
			int ret = 0;
			lock (this.Mutex)
			{
				int histForUser = KuaFuCopyDbMgr.Instance.GetAwardHistoryForUser(userid, actType, huoDongKeyStr, out hasgettimes, out lastgettime);
				hasgettimes = (long)extTag;
				lastgettime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (histForUser < 0)
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

		// Token: 0x04000155 RID: 341
		private static SpecPriorityActivityMgr _instance = new SpecPriorityActivityMgr();

		// Token: 0x04000156 RID: 342
		public object Mutex = new object();

		// Token: 0x04000157 RID: 343
		public List<SpecPConditionConfig> SpecPConditionList = new List<SpecPConditionConfig>();

		// Token: 0x04000158 RID: 344
		public Dictionary<int, int> ActConditionInfoDict = new Dictionary<int, int>();

		// Token: 0x04000159 RID: 345
		public int SpecPriorityActivityType = 49;

		// Token: 0x0400015A RID: 346
		private int LastUpdateDayID;
	}
}
