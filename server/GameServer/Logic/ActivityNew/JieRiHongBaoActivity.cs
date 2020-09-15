using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x0200002E RID: 46
	public class JieRiHongBaoActivity : JieRiActivity
	{
		// Token: 0x06000064 RID: 100 RVA: 0x000078C4 File Offset: 0x00005AC4
		public static JieRiHongBaoActivity getInstance()
		{
			return JieRiHongBaoActivity.instance;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000078DC File Offset: 0x00005ADC
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/JieRiGifts/JieRiQuanMinHongBao.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/JieRiGifts/JieRiQuanMinHongBao.xml"));
				if (null == xml)
				{
					return false;
				}
				lock (this.Mutex)
				{
					this.RedPacketsQuanMinMessage = GameManager.systemParamsList.GetParamValueByName("RedPacketsQuanMinMessage");
					this.HongBaoDict.Clear();
					XElement args = xml.Element("Activities");
					if (null != args)
					{
						this.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
						this.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
						this.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
						this.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
						this.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
					}
					base.PredealDateTime();
					args = xml.Element("GiftList");
					if (null != args)
					{
						IEnumerable<XElement> xmlItems = args.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (null != xmlItem)
							{
								RedPacketPeopleItem item = new RedPacketPeopleItem();
								item.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
								item.Day = (int)Global.GetSafeAttributeLong(xmlItem, "Day");
								item.Time = TimeSpan.Parse(Global.GetSafeAttributeStr(xmlItem, "Time"));
								item.RedPacketSize = (int)Global.GetSafeAttributeLong(xmlItem, "RedPacketSize");
								item.Interval = Global.GetSafeAttributeIntArray(xmlItem, "Interval", -1, ',');
								item.DurationTime = (int)Global.GetSafeAttributeLong(xmlItem, "DurationTime");
								item.SendTime = this.StartTime.AddDays((double)(item.Day - 1)).Add(item.Time);
								this.HongBaoDict.Add(item.ID, item);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiQuanMinHongBao.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00007BA0 File Offset: 0x00005DA0
		public HongBaoListQueryData QueryHongBaoList()
		{
			try
			{
				HongBaoListQueryData queryData = new HongBaoListQueryData
				{
					KeyStr = this.ActivityKeyStr
				};
				return Global.sendToDB<HongBaoListQueryData, HongBaoListQueryData>(1437, queryData, 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00007BFC File Offset: 0x00005DFC
		public List<HongBaoSendData> SendHongBaoProc(DateTime now, Dictionary<int, HongBaoSendData> dict)
		{
			List<HongBaoSendData> result;
			if (GameManager.IsKuaFuServer)
			{
				result = null;
			}
			else
			{
				List<HongBaoSendData> list = new List<HongBaoSendData>();
				lock (this.Mutex)
				{
					foreach (RedPacketPeopleItem item in this.HongBaoDict.Values)
					{
						if (now > item.SendTime && !this.HongBaoIdSended.Contains(item.ID))
						{
							DateTime endTime = item.SendTime.AddSeconds((double)item.DurationTime);
							if (!(now >= endTime))
							{
								foreach (HongBaoSendData hongbao in dict.Values)
								{
									if (hongbao.senderID == item.ID)
									{
										this.HongBaoIdSended.Add(item.ID);
									}
								}
								if (!this.HongBaoIdSended.Contains(item.ID))
								{
									HongBaoSendData sendData = new HongBaoSendData();
									sendData.senderID = item.ID;
									sendData.sender = this.ActivityKeyStr;
									sendData.sendTime = item.SendTime;
									sendData.type = 102;
									sendData.endTime = item.SendTime.AddSeconds((double)item.DurationTime);
									sendData.message = this.RedPacketsQuanMinMessage;
									sendData.sumDiamondNum = item.RedPacketSize;
									sendData.leftZuanShi = item.RedPacketSize;
									int hongbaoId = Global.sendToDB<int, HongBaoSendData>(1435, sendData, GameManager.ServerId);
									if (hongbaoId > 0)
									{
										sendData.hongBaoID = hongbaoId;
										this.HongBaoIdSended.Add(item.ID);
										list.Add(sendData);
									}
								}
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00007E80 File Offset: 0x00006080
		public int OpenHongBao(int id)
		{
			lock (this.Mutex)
			{
				RedPacketPeopleItem item;
				if (this.HongBaoDict.TryGetValue(id, out item))
				{
					if (item.Interval.Length == 2)
					{
						return Global.GetRandomNumber(item.Interval[0], item.Interval[1]);
					}
				}
			}
			return 0;
		}

		// Token: 0x040000F5 RID: 245
		private const string CfgFile = "Config/JieRiGifts/JieRiQuanMinHongBao.xml";

		// Token: 0x040000F6 RID: 246
		private object Mutex = new object();

		// Token: 0x040000F7 RID: 247
		private string RedPacketsQuanMinMessage;

		// Token: 0x040000F8 RID: 248
		private SortedDictionary<int, RedPacketPeopleItem> HongBaoDict = new SortedDictionary<int, RedPacketPeopleItem>();

		// Token: 0x040000F9 RID: 249
		private HashSet<int> HongBaoIdSended = new HashSet<int>();

		// Token: 0x040000FA RID: 250
		private static JieRiHongBaoActivity instance = new JieRiHongBaoActivity();
	}
}
