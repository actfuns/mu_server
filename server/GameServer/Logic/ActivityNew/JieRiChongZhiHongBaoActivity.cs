using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x0200004A RID: 74
	public class JieRiChongZhiHongBaoActivity : Activity
	{
		// Token: 0x060000E4 RID: 228 RVA: 0x0000FEB4 File Offset: 0x0000E0B4
		public static JieRiChongZhiHongBaoActivity getInstance()
		{
			return JieRiChongZhiHongBaoActivity.instance;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000FECC File Offset: 0x0000E0CC
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

		// Token: 0x060000E6 RID: 230 RVA: 0x00010178 File Offset: 0x0000E378
		public string GetKeyStr()
		{
			return this.ActivityKeyStr;
		}

		// Token: 0x04000198 RID: 408
		private const string CfgFile = "Config/JieRiGifts/JieRiChongZhiHongBao.xml";

		// Token: 0x04000199 RID: 409
		private static JieRiChongZhiHongBaoActivity instance = new JieRiChongZhiHongBaoActivity();

		// Token: 0x0400019A RID: 410
		private object Mutex = new object();

		// Token: 0x0400019B RID: 411
		private HashSet<long> recvDict = new HashSet<long>();

		// Token: 0x0400019C RID: 412
		private SortedDictionary<int, JieRiChongZhiHongBaoInfo> HongBaoDict = new SortedDictionary<int, JieRiChongZhiHongBaoInfo>();
	}
}
