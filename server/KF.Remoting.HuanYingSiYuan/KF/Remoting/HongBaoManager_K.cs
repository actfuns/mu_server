using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	// Token: 0x02000027 RID: 39
	public class HongBaoManager_K
	{
		// Token: 0x060001B9 RID: 441 RVA: 0x000188D4 File Offset: 0x00016AD4
		public static HongBaoManager_K getInstance()
		{
			return HongBaoManager_K._Instance;
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060001BA RID: 442 RVA: 0x000188EC File Offset: 0x00016AEC
		// (set) Token: 0x060001BB RID: 443 RVA: 0x00018909 File Offset: 0x00016B09
		private KuaFuCmdData JunTuanBaseDataListCmdData
		{
			get
			{
				return this.Persistence.JunTuanBaseDataListCmdData;
			}
			set
			{
				this.Persistence.JunTuanBaseDataListCmdData = value;
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00018918 File Offset: 0x00016B18
		public void ThreadProc(object state)
		{
			if (this.Initialiazed)
			{
				try
				{
					DateTime now = TimeUtil.NowDateTime();
					long nowTicks = TimeUtil.NOW();
					bool check20 = false;
					if (now > this.CheckTime20)
					{
						this.CheckTime20 = now.AddSeconds(20.0);
						check20 = true;
					}
					this.SendHongBaoProc(now);
					this.CheckHongBaoState(nowTicks, check20);
					if (check20)
					{
						this.Persistence.UpdateHongBaoHuoDongData(this.HuoDongStartTicks, this.NextSendID, this.LeftCharge, this.TotalCharge);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		// Token: 0x060001BD RID: 445 RVA: 0x000189DC File Offset: 0x00016BDC
		public bool LoadConfig()
		{
			try
			{
				this.Initialiazed = false;
				XElement xml = ConfigHelper.Load(KuaFuServerManager.GetResourcePath("Config/JieRiGifts/JieRiChongZhiHongBao.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
				if (null == xml)
				{
					return false;
				}
				lock (this.Mutex)
				{
					this.ConfigDict.Clear();
					XElement args = xml.Element("Activities");
					if (null != args)
					{
						this.FromDate = ConfigHelper.GetElementAttributeValue(args, "FromDate", "");
						this.ToDate = ConfigHelper.GetElementAttributeValue(args, "ToDate", "");
						this.ActivityType = (int)ConfigHelper.GetElementAttributeValueLong(args, "ActivityType", 0L);
						this.AwardStartDate = ConfigHelper.GetElementAttributeValue(args, "AwardStartDate", "");
						this.AwardEndDate = ConfigHelper.GetElementAttributeValue(args, "AwardEndDate", "");
						this.StartTime = DateTime.Parse(this.FromDate);
						this.EndTime = DateTime.Parse(this.ToDate);
						this.ActivityKeyStr = string.Format("{0}_{1}", this.FromDate, this.ToDate).Replace(':', '$');
					}
					else
					{
						this.ActivityKeyStr = "";
						this.StartTime = (this.EndTime = DateTime.MinValue);
					}
					this.HuoDongStartTicks = this.StartTime.Ticks / 10000000L * 1000L;
					args = xml.Element("GiftList");
					if (null != args)
					{
						IEnumerable<XElement> xmlItems = args.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (null != xmlItem)
							{
								JieRiChongZhiHongBaoInfo item = new JieRiChongZhiHongBaoInfo();
								item.ID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ID", 0L);
								item.RechargeDifference = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "RechargeDifference", 0L);
								item.PlatformID = ConfigHelper.GetElementAttributeValue(xmlItem, "PlatformID", "");
								item.RedPacketSize = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "RedPacketSize", 0L);
								item.Interval = ConfigHelper.String2IntArray(ConfigHelper.GetElementAttributeValue(xmlItem, "Interval", ""), ',');
								item.DurationTime = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "DurationTime", 0L);
								if (string.Compare(item.PlatformID, KuaFuServerManager.platformType.ToString(), true) == 0)
								{
									this.ConfigDict.Add(item.ID, item);
								}
							}
						}
					}
				}
				this.Initialiazed = this.InitData();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiChongZhiHongBao.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00018D3C File Offset: 0x00016F3C
		public bool InitData()
		{
			lock (this.Mutex)
			{
				long huoDongStartTicks = 0L;
				int nextSendId = 0;
				long leftCharge = 0L;
				long totalCharge = 0L;
				if (this.Persistence.GetHongBaoHuoDongData(ref huoDongStartTicks, ref nextSendId, ref leftCharge, ref totalCharge))
				{
					if (huoDongStartTicks == this.HuoDongStartTicks)
					{
						this.HuoDongStartTicks = huoDongStartTicks;
						this.NextSendID = nextSendId;
						this.LeftCharge = leftCharge;
						this.TotalCharge = totalCharge;
						this.Persistence.LoadHongBaoDataList(this.ActivityKeyStr, this.HongBaoDataDict, this.HongBaoRecvDict);
						foreach (KeyValuePair<int, SystemHongBaoData> kv in this.HongBaoDataDict)
						{
							if (this.NextSendID < kv.Value.ID)
							{
								this.NextSendID = kv.Value.ID;
							}
						}
					}
					else
					{
						this.NextSendID = 0;
						this.LeftCharge = 0L;
						this.TotalCharge = 0L;
						this.HongBaoDataDict.Clear();
						this.HongBaoRecvDict.Clear();
					}
				}
			}
			return true;
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00018ED0 File Offset: 0x000170D0
		public List<SystemHongBaoData> SendHongBaoProc(DateTime now)
		{
			List<SystemHongBaoData> list = new List<SystemHongBaoData>();
			lock (this.Mutex)
			{
				JieRiChongZhiHongBaoInfo item = null;
				JieRiChongZhiHongBaoInfo next = null;
				for (int i = 0; i < this.ConfigDict.Values.Count; i++)
				{
					item = this.ConfigDict.Values[i];
					if (i < this.ConfigDict.Values.Count - 1)
					{
						next = this.ConfigDict.Values[i + 1];
					}
					if (item.ID >= this.NextSendID)
					{
						break;
					}
				}
				if (this.LeftCharge >= (long)item.RechargeDifference)
				{
					this.NextSendID = next.ID;
					this.LeftCharge -= (long)item.RechargeDifference;
					SystemHongBaoData sendData = new SystemHongBaoData();
					sendData.ID = item.ID;
					sendData.LeftZuanShi = item.RedPacketSize;
					sendData.StartTime = TimeUtil.NOW() + 15000L;
					sendData.DurationTime = item.DurationTime * 1000;
					DateTime startTime = new DateTime(sendData.StartTime * 10000L);
					DateTime endTime = new DateTime((sendData.StartTime + (long)item.DurationTime * 1000L) * 10000L);
					int hongbaoId = (int)this.Persistence.CreateHongBao(this.ActivityKeyStr, sendData.ID, startTime, endTime, sendData.LeftZuanShi, 0);
					if (hongbaoId > 0)
					{
						sendData.HongBaoId = hongbaoId;
						this.HongBaoDataDict[hongbaoId] = sendData;
					}
				}
			}
			return list;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x000190C4 File Offset: 0x000172C4
		public void CheckHongBaoState(long nowTicks, bool writedb)
		{
			lock (this.Mutex)
			{
				List<SystemHongBaoData> removeList = new List<SystemHongBaoData>();
				foreach (SystemHongBaoData hongbao in this.HongBaoDataDict.Values)
				{
					if (hongbao.LeftZuanShi <= 0)
					{
						hongbao.State = 3;
						removeList.Add(hongbao);
					}
					else if (nowTicks > hongbao.StartTime + (long)hongbao.DurationTime)
					{
						hongbao.State = 2;
						removeList.Add(hongbao);
					}
					else if (writedb)
					{
						this.Persistence.UpdateHongBao(hongbao.HongBaoId, hongbao.LeftZuanShi, hongbao.State);
					}
				}
				foreach (SystemHongBaoData hongbao in removeList)
				{
					this.HongBaoDataDict.Remove(hongbao.HongBaoId);
					this.Persistence.UpdateHongBao(hongbao.HongBaoId, hongbao.LeftZuanShi, hongbao.State);
				}
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00019270 File Offset: 0x00017470
		public AsyncDataItem GetHongBaoDataList(long dataAge)
		{
			try
			{
				if (this.TotalCharge > 0L)
				{
					DateTime now = TimeUtil.NowDateTime();
					long nowTicks = TimeUtil.NOW();
					if (now >= this.StartTime && now < this.EndTime)
					{
						lock (this.Mutex)
						{
							byte[] bytes = DataHelper2.ObjectToBytes<Dictionary<int, SystemHongBaoData>>(this.HongBaoDataDict);
							KuaFuCmdData cmdData = new KuaFuCmdData
							{
								Age = nowTicks,
								Bytes0 = bytes
							};
							return new AsyncDataItem(KuaFuEventTypes.ChongZhiHongBaoList, new object[]
							{
								cmdData
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
			return null;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00019364 File Offset: 0x00017564
		public int OpenHongBao(int hongBaoId, int rid, int zoneid, string userid, string rname)
		{
			try
			{
				lock (this.Mutex)
				{
					SystemHongBaoData hongBaoData;
					if (!this.HongBaoDataDict.TryGetValue(hongBaoId, out hongBaoData))
					{
						return -20;
					}
					if (hongBaoData.LeftZuanShi <= 0)
					{
						return -42;
					}
					long kvp = ((long)hongBaoId << 36) + (long)rid;
					if (this.HongBaoRecvDict.ContainsKey(kvp))
					{
						return -200;
					}
					JieRiChongZhiHongBaoInfo info;
					if (!this.ConfigDict.TryGetValue(hongBaoData.ID, out info))
					{
						return -42;
					}
					int maxv = Math.Min(hongBaoData.LeftZuanShi, info.Interval[1]);
					int minv = Math.Min(hongBaoData.LeftZuanShi, info.Interval[0]);
					int zuanshi = Global.GetRandomNumber(minv, maxv);
					hongBaoData.LeftZuanShi -= zuanshi;
					this.HongBaoRecvDict.Add(kvp, zuanshi);
					this.Persistence.WriteHongBaoRecv(this.ActivityKeyStr, hongBaoData.HongBaoId, rid, zoneid, userid, rname, zuanshi);
					return zuanshi;
				}
			}
			catch (Exception ex)
			{
			}
			return -11000;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x000194E0 File Offset: 0x000176E0
		public void AddServerTotalCharge(string keyStr, long addCharge)
		{
			try
			{
				if (!(keyStr != this.ActivityKeyStr))
				{
					DateTime now = TimeUtil.NowDateTime();
					if (!(now < this.StartTime) && !(now > this.EndTime))
					{
						lock (this.Mutex)
						{
							this.LeftCharge += addCharge;
							this.TotalCharge += addCharge;
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x040000F5 RID: 245
		private const double SaveServerStateProcInterval = 30.0;

		// Token: 0x040000F6 RID: 246
		private const long DelaySendTicks = 15000L;

		// Token: 0x040000F7 RID: 247
		private const string CfgFile = "Config/JieRiGifts/JieRiChongZhiHongBao.xml";

		// Token: 0x040000F8 RID: 248
		private static HongBaoManager_K _Instance = new HongBaoManager_K();

		// Token: 0x040000F9 RID: 249
		private object Mutex = new object();

		// Token: 0x040000FA RID: 250
		public string FromDate = "";

		// Token: 0x040000FB RID: 251
		public string ToDate = "";

		// Token: 0x040000FC RID: 252
		public string AwardStartDate = "";

		// Token: 0x040000FD RID: 253
		public string AwardEndDate = "";

		// Token: 0x040000FE RID: 254
		public int ActivityType = -1;

		// Token: 0x040000FF RID: 255
		protected int CodeForParamsValidate = 0;

		// Token: 0x04000100 RID: 256
		public string ActivityKeyStr;

		// Token: 0x04000101 RID: 257
		public DateTime StartTime;

		// Token: 0x04000102 RID: 258
		public DateTime EndTime;

		// Token: 0x04000103 RID: 259
		public readonly GameTypes GameType = GameTypes.JunTuan;

		// Token: 0x04000104 RID: 260
		private DateTime CheckTime20;

		// Token: 0x04000105 RID: 261
		private bool Initialiazed = false;

		// Token: 0x04000106 RID: 262
		private DateTime SaveServerStateProcTime;

		// Token: 0x04000107 RID: 263
		private int LastUpdateRankHour = -1;

		// Token: 0x04000108 RID: 264
		public JunTuanPersistence Persistence = JunTuanPersistence.Instance;

		// Token: 0x04000109 RID: 265
		private long HuoDongStartTicks;

		// Token: 0x0400010A RID: 266
		private int NextSendID;

		// Token: 0x0400010B RID: 267
		private long LeftCharge;

		// Token: 0x0400010C RID: 268
		private long TotalCharge;

		// Token: 0x0400010D RID: 269
		private SortedList<int, JieRiChongZhiHongBaoInfo> ConfigDict = new SortedList<int, JieRiChongZhiHongBaoInfo>();

		// Token: 0x0400010E RID: 270
		private Dictionary<int, SystemHongBaoData> HongBaoDataDict = new Dictionary<int, SystemHongBaoData>();

		// Token: 0x0400010F RID: 271
		private Dictionary<long, int> HongBaoRecvDict = new Dictionary<long, int>();
	}
}
