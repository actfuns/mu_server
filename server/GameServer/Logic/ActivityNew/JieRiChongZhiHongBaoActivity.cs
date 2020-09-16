using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.ActivityNew
{
	
	public class JieRiChongZhiHongBaoActivity : Activity
	{
		
		public static JieRiChongZhiHongBaoActivity getInstance()
		{
			return JieRiChongZhiHongBaoActivity.instance;
		}

		
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/JieRiGifts/JieRiChongZhiHongBao.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/JieRiGifts/JieRiChongZhiHongBao.xml"));
				if (null == xml)
				{
					return false;
				}
				lock (this.Mutex)
				{
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
					args = xml.Element("GiftList");
					if (null != args)
					{
						IEnumerable<XElement> xmlItems = args.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (null != xmlItem)
							{
								JieRiChongZhiHongBaoInfo item = new JieRiChongZhiHongBaoInfo();
								item.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
								item.RechargeDifference = (int)Global.GetSafeAttributeLong(xmlItem, "RechargeDifference");
								item.PlatformID = Global.GetSafeAttributeStr(xmlItem, "PlatformID");
								item.RedPacketSize = (int)Global.GetSafeAttributeLong(xmlItem, "RedPacketSize");
								item.Interval = Global.GetSafeAttributeIntArray(xmlItem, "Interval", -1, ',');
								item.DurationTime = (int)Global.GetSafeAttributeLong(xmlItem, "DurationTime");
								this.HongBaoDict.Add(item.ID, item);
							}
						}
					}
					base.PredealDateTime();
					this.ActivityKeyStr = string.Format("{0}_{1}", this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiChongZhiHongBao.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		public string GetKeyStr()
		{
			return this.ActivityKeyStr;
		}

		
		private const string CfgFile = "Config/JieRiGifts/JieRiChongZhiHongBao.xml";

		
		private static JieRiChongZhiHongBaoActivity instance = new JieRiChongZhiHongBaoActivity();

		
		private object Mutex = new object();

		
		private HashSet<long> recvDict = new HashSet<long>();

		
		private SortedDictionary<int, JieRiChongZhiHongBaoInfo> HongBaoDict = new SortedDictionary<int, JieRiChongZhiHongBaoInfo>();
	}
}
