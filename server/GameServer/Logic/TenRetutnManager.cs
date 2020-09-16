using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class TenRetutnManager : IManager
	{
		
		public static TenRetutnManager getInstance()
		{
			return TenRetutnManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
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

		
		public bool InitConfig()
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.SystemOpen = false;
				string fileName = Global.GameResPath("Config/TenRetutnAward.xml");
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return true;
				}
				try
				{
					this.RuntimeData.SystemOpen = false;
					this.RuntimeData._tenAwardDic.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							TenRetutnAwardsData config = new TenRetutnAwardsData();
							config.ID = Convert.ToInt32(Global.GetSafeAttributeLong(xmlItem, "ID"));
							config.MailUser = GLang.GetLang(112, new object[0]);
							config.MailTitle = Global.GetSafeAttributeStr(xmlItem, "MailTitle");
							config.MailContent = Global.GetSafeAttributeStr(xmlItem, "MailContent");
							ConfigParser.ParseAwardsItemList(Global.GetDefAttributeStr(xmlItem, "GoodsID1", ""), ref config.GoodsID1, '|', ',');
							ConfigParser.ParseAwardsItemList(Global.GetDefAttributeStr(xmlItem, "GoodsID2", ""), ref config.GoodsID2, '|', ',');
							config.UserList = Global.GetSafeAttributeStr(xmlItem, "UserList");
							string beginTime = Global.GetDefAttributeStr(xmlItem, "BeginTime", "2019-12-31");
							string finishTime = Global.GetDefAttributeStr(xmlItem, "FinishTime", "2011-11-11");
							config.BeginTimeStr = beginTime.Replace(':', '$');
							config.FinishTimeStr = finishTime.Replace(':', '$');
							if (DateTime.TryParse(beginTime, out config.BeginTime) && DateTime.TryParse(finishTime, out config.FinishTime) && TimeUtil.NowDateTime() < config.FinishTime)
							{
								config.SystemOpen = true;
								this.RuntimeData._tenAwardDic.Add(config.ID, config);
								fileName = Global.GameResPath("Config/" + config.UserList);
								if (File.Exists(fileName))
								{
									string[] allUserIds = File.ReadAllLines(fileName);
									foreach (string userid in allUserIds)
									{
										if (!string.IsNullOrEmpty(userid))
										{
											config._tenUserIdAwardsDict[userid.ToLower()] = false;
										}
									}
								}
								if (config._tenUserIdAwardsDict.Count == 0)
								{
									config.SystemOpen = false;
								}
							}
							this.RuntimeData.SystemOpen |= config.SystemOpen;
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Warning, "加载Config/TenRetutnAward.xml时文件出现异常!!!", ex, true);
				}
			}
			return true;
		}

		
		public void GiveAwards(GameClient client)
		{
			if (this.RuntimeData.SystemOpen)
			{
				DateTime now = TimeUtil.NowDateTime();
				string userId = client.strUserID.ToLower();
				List<TenRetutnAwardsData> list = new List<TenRetutnAwardsData>();
				lock (this.RuntimeData.Mutex)
				{
					foreach (TenRetutnAwardsData config in this.RuntimeData._tenAwardDic.Values)
					{
						if (config.SystemOpen && now >= config.BeginTime && now <= config.FinishTime)
						{
							bool hasGet;
							if (config._tenUserIdAwardsDict.TryGetValue(userId, out hasGet) && !hasGet)
							{
								list.Add(config);
							}
						}
					}
				}
				foreach (TenRetutnAwardsData data in list)
				{
					string keyStr = string.Format("{0}_{1}_{2}_{3}", new object[]
					{
						data.BeginTimeStr,
						data.FinishTimeStr,
						data.ID,
						client.ClientData.ZoneID
					});
					string[] result = Global.QeuryUserActivityInfo(client, keyStr, 999, "0");
					if (result != null && result.Length != 0)
					{
						int ret = Global.SafeConvertToInt32(result[0]);
						int hasGetTimes = Global.SafeConvertToInt32(result[3]);
						if (hasGetTimes > 0)
						{
							lock (this.RuntimeData.Mutex)
							{
								data._tenUserIdAwardsDict[userId] = true;
							}
						}
						else
						{
							List<AwardsItemData> itemList = new List<AwardsItemData>(data.GoodsID1.Items);
							foreach (AwardsItemData item in data.GoodsID2.Items)
							{
								if (Global.IsCanGiveRewardByOccupation(client, item.GoodsID))
								{
									itemList.Add(item);
								}
							}
							ret = Global.UseMailGivePlayerAward2(client, itemList, data.MailTitle, data.MailContent, 0, 0, 0);
							if (ret >= 0)
							{
								Global.UpdateUserActivityInfo(client, keyStr, 999, 1L, now.ToString("yyyy-MM-dd HH$mm$ss"));
							}
							lock (this.RuntimeData.Mutex)
							{
								data._tenUserIdAwardsDict[userId] = true;
							}
						}
					}
				}
			}
		}

		
		private static TenRetutnManager instance = new TenRetutnManager();

		
		public TenRetutnData RuntimeData = new TenRetutnData();
	}
}
