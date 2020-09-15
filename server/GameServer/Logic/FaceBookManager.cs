using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020004DA RID: 1242
	public class FaceBookManager : IManager
	{
		// Token: 0x06001711 RID: 5905 RVA: 0x00169F88 File Offset: 0x00168188
		public static FaceBookManager getInstance()
		{
			return FaceBookManager.instance;
		}

		// Token: 0x06001712 RID: 5906 RVA: 0x00169FA0 File Offset: 0x001681A0
		public bool initialize()
		{
			return FaceBookManager.initFacebook();
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x00169FC0 File Offset: 0x001681C0
		public bool startup()
		{
			return true;
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x00169FD4 File Offset: 0x001681D4
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06001715 RID: 5909 RVA: 0x00169FE8 File Offset: 0x001681E8
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x00169FFC File Offset: 0x001681FC
		public static bool initFacebook()
		{
			string fileName = "Config/FacebookAward.xml";
			XElement xml = ConfigHelper.Load(Global.GameResPath(fileName));
			bool result;
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Error, "加载Config/FacebookAward.xml时出错!!!文件不存在", null, true);
				result = false;
			}
			else
			{
				try
				{
					FaceBookManager._FacebookAwards.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							FacebookAwardData config = new FacebookAwardData();
							config.AwardID = Convert.ToInt32(Global.GetSafeAttributeLong(xmlItem, "ID"));
							config.AwardName = Global.GetSafeAttributeStr(xmlItem, "Name");
							config.DbKey = Global.GetSafeAttributeStr(xmlItem, "DbKey");
							config.DayMaxNum = Convert.ToInt32(Global.GetSafeAttributeLong(xmlItem, "DayMaxNum"));
							config.OnlyNum = Convert.ToInt32(Global.GetSafeAttributeLong(xmlItem, "OnlyNum"));
							config.MailUser = GLang.GetLang(112, new object[0]);
							config.MailTitle = Global.GetSafeAttributeStr(xmlItem, "MailTitle");
							config.MailContent = Global.GetSafeAttributeStr(xmlItem, "MailContent");
							string awards = Global.GetSafeAttributeStr(xmlItem, "AwardGoods");
							if (awards.Length > 0)
							{
								config.AwardGoods = new List<GoodsData>();
								string[] awardsArr = awards.Split(new char[]
								{
									'|'
								});
								foreach (string award in awardsArr)
								{
									string[] oneArr = award.Split(new char[]
									{
										','
									});
									GoodsData d = new GoodsData();
									d.Id = Convert.ToInt32(oneArr[0]);
									d.GCount = Convert.ToInt32(oneArr[1]);
									d.Binding = Convert.ToInt32(oneArr[2]);
									config.AwardGoods.Add(d);
								}
							}
							FaceBookManager._FacebookAwards.Add(config.AwardID, config);
						}
					}
					FaceBookManager.initFacebookDb();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, "加载Config/FacebookAward.xml时文件出现异常!!!", null, true);
					Process.GetCurrentProcess().Kill();
					return false;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x0016A298 File Offset: 0x00168498
		private static void initFacebookDb()
		{
			string dbCmds = "";
			foreach (FacebookAwardData item in FaceBookManager._FacebookAwards.Values)
			{
				if (dbCmds.Length > 0)
				{
					dbCmds += "#";
				}
				string goodsStr = "";
				if (item.AwardGoods != null && item.AwardGoods.Count > 0)
				{
					foreach (GoodsData goods in item.AwardGoods)
					{
						if (goodsStr.Length > 0)
						{
							goodsStr += "|";
						}
						goodsStr += string.Format("{0},{1},{2}", goods.Id, goods.GCount, goods.Binding);
					}
				}
				dbCmds += string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
				{
					item.AwardID,
					item.DbKey,
					item.OnlyNum,
					item.DayMaxNum,
					goodsStr,
					item.MailTitle,
					item.MailContent,
					item.MailUser
				});
			}
			string[] dbFields = null;
			Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 21000, dbCmds, out dbFields, 0);
		}

		// Token: 0x04002101 RID: 8449
		private static FaceBookManager instance = new FaceBookManager();

		// Token: 0x04002102 RID: 8450
		private static Dictionary<int, FacebookAwardData> _FacebookAwards = new Dictionary<int, FacebookAwardData>();
	}
}
