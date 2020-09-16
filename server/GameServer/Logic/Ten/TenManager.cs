using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Ten
{
	
	public class TenManager : IManager
	{
		
		public static TenManager getInstance()
		{
			return TenManager.instance;
		}

		
		public bool initialize()
		{
			return TenManager.initConfig();
		}

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public static bool initConfig()
		{
			string fileName = Global.GameResPath("Config/TenAward.xml");
			XElement xml = CheckHelper.LoadXml(fileName, true);
			bool result;
			if (null == xml)
			{
				result = false;
			}
			else
			{
				try
				{
					TenManager._tenAwardDic.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							TenAwardData config = new TenAwardData();
							config.AwardID = Convert.ToInt32(Global.GetSafeAttributeLong(xmlItem, "ID"));
							config.AwardName = Global.GetSafeAttributeStr(xmlItem, "Name");
							config.DbKey = Global.GetSafeAttributeStr(xmlItem, "DbKey");
							config.DayMaxNum = Convert.ToInt32(Global.GetSafeAttributeLong(xmlItem, "DayMaxNum"));
							config.OnlyNum = Convert.ToInt32(Global.GetSafeAttributeLong(xmlItem, "OnlyNum"));
							config.MailUser = GLang.GetLang(112, new object[0]);
							config.MailTitle = Global.GetSafeAttributeStr(xmlItem, "MailTitle");
							config.MailContent = Global.GetSafeAttributeStr(xmlItem, "MailContent");
							string beginTime = Global.GetDefAttributeStr(xmlItem, "BeginDate", "");
							string endTime = Global.GetDefAttributeStr(xmlItem, "EndDate", "");
							string roleLevel = Global.GetDefAttributeStr(xmlItem, "Level", "0,1");
							if (string.IsNullOrEmpty(beginTime))
							{
								config.BeginTime = DateTime.MinValue;
							}
							else
							{
								config.BeginTime = DateTime.Parse(beginTime);
							}
							if (string.IsNullOrEmpty(endTime))
							{
								config.EndTime = DateTime.MaxValue;
							}
							else
							{
								config.EndTime = DateTime.Parse(endTime);
							}
							string[] arrLevel = roleLevel.Split(new char[]
							{
								','
							});
							config.RoleLevel = int.Parse(arrLevel[0]) * 100 + int.Parse(arrLevel[1]);
							string awards = Global.GetSafeAttributeStr(xmlItem, "AwardGoods");
							if (!string.IsNullOrEmpty(awards))
							{
								string[] awardsArr = awards.Split(new char[]
								{
									'|'
								});
								config.AwardGoods = GoodsHelper.ParseGoodsDataList(awardsArr, fileName);
							}
							TenManager._tenAwardDic.Add(config.AwardID, config);
						}
					}
					TenManager.initDb();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, "加载Config/TenAward.xml时文件出现异常!!!", null, true);
					Process.GetCurrentProcess().Kill();
					return false;
				}
				result = true;
			}
			return result;
		}

		
		public static void initDb()
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
			{
				if (GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("Ten"))
				{
					string dbCmds = "";
					foreach (TenAwardData item in TenManager._tenAwardDic.Values)
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
								goodsStr += string.Format("{0},{1},{2}", goods.GoodsID, goods.GCount, goods.Binding);
							}
						}
						dbCmds += string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
						{
							item.AwardID,
							item.DbKey,
							item.OnlyNum,
							item.DayMaxNum,
							goodsStr,
							item.MailTitle,
							item.MailContent,
							item.MailUser,
							item.BeginTime.ToString("yyyyMMddHHmmss"),
							item.EndTime.ToString("yyyyMMddHHmmss"),
							item.RoleLevel
						});
					}
					string[] dbFields = null;
					Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 13113, dbCmds, out dbFields, 0);
				}
			}
		}

		
		private static TenManager instance = new TenManager();

		
		private static Dictionary<int, TenAwardData> _tenAwardDic = new Dictionary<int, TenAwardData>();
	}
}
