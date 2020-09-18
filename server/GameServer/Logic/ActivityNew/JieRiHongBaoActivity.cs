using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.ActivityNew
{
	
	public class JieRiHongBaoActivity : JieRiActivity
	{
		
		public static JieRiHongBaoActivity getInstance()
		{
			return JieRiHongBaoActivity.instance;
		}

		
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

		
		private const string CfgFile = "Config/JieRiGifts/JieRiQuanMinHongBao.xml";

		
		private object Mutex = new object();

		
		private string RedPacketsQuanMinMessage;

		
		private SortedDictionary<int, RedPacketPeopleItem> HongBaoDict = new SortedDictionary<int, RedPacketPeopleItem>();

		
		private HashSet<int> HongBaoIdSended = new HashSet<int>();

		
		private static JieRiHongBaoActivity instance = new JieRiHongBaoActivity();
	}
}
