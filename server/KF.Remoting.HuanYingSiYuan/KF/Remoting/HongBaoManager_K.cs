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
	
	public class HongBaoManager_K
	{
		
		public static HongBaoManager_K getInstance()
		{
			return HongBaoManager_K._Instance;
		}

		
		
		
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

		
		private const double SaveServerStateProcInterval = 30.0;

		
		private const long DelaySendTicks = 15000L;

		
		private const string CfgFile = "Config/JieRiGifts/JieRiChongZhiHongBao.xml";

		
		private static HongBaoManager_K _Instance = new HongBaoManager_K();

		
		private object Mutex = new object();

		
		public string FromDate = "";

		
		public string ToDate = "";

		
		public string AwardStartDate = "";

		
		public string AwardEndDate = "";

		
		public int ActivityType = -1;

		
		protected int CodeForParamsValidate = 0;

		
		public string ActivityKeyStr;

		
		public DateTime StartTime;

		
		public DateTime EndTime;

		
		public readonly GameTypes GameType = GameTypes.JunTuan;

		
		private DateTime CheckTime20;

		
		private bool Initialiazed = false;

		
		private DateTime SaveServerStateProcTime;

		
		private int LastUpdateRankHour = -1;

		
		public JunTuanPersistence Persistence = JunTuanPersistence.Instance;

		
		private long HuoDongStartTicks;

		
		private int NextSendID;

		
		private long LeftCharge;

		
		private long TotalCharge;

		
		private SortedList<int, JieRiChongZhiHongBaoInfo> ConfigDict = new SortedList<int, JieRiChongZhiHongBaoInfo>();

		
		private Dictionary<int, SystemHongBaoData> HongBaoDataDict = new Dictionary<int, SystemHongBaoData>();

		
		private Dictionary<long, int> HongBaoRecvDict = new Dictionary<long, int>();
	}
}
