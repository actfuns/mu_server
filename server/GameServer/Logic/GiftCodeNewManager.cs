using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020002D6 RID: 726
	public class GiftCodeNewManager : IManager
	{
		// Token: 0x06000B6F RID: 2927 RVA: 0x000B3EF8 File Offset: 0x000B20F8
		public static GiftCodeNewManager getInstance()
		{
			return GiftCodeNewManager.instance;
		}

		// Token: 0x06000B70 RID: 2928 RVA: 0x000B3F10 File Offset: 0x000B2110
		public bool initialize()
		{
			return this.initGiftCode();
		}

		// Token: 0x06000B71 RID: 2929 RVA: 0x000B3F34 File Offset: 0x000B2134
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000B72 RID: 2930 RVA: 0x000B3F48 File Offset: 0x000B2148
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000B73 RID: 2931 RVA: 0x000B3F5C File Offset: 0x000B215C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x000B3F70 File Offset: 0x000B2170
		public static bool IsFuncOpen()
		{
			return true;
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x000B3F84 File Offset: 0x000B2184
		public bool initGiftCode()
		{
			bool success = true;
			string fileName = "";
			lock (this._lockConfig)
			{
				try
				{
					GiftCodeNewManager._GiftCodeCfgAwards.Clear();
					Dictionary<string, GiftCodeInfo> newDic = new Dictionary<string, GiftCodeInfo>();
					fileName = Global.GameResPath("Config/GiftCodeNew.xml");
					XElement xml = CheckHelper.LoadXml(fileName, true);
					if (null == xml)
					{
						return false;
					}
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (xmlItem != null)
						{
							GiftCodeInfo info = new GiftCodeInfo();
							info.GiftCodeTypeID = Global.GetDefAttributeStr(xmlItem, "TypeID", "");
							info.GiftCodeName = Global.GetDefAttributeStr(xmlItem, "TypeName", "");
							info.Description = Global.GetDefAttributeStr(xmlItem, "Description", "");
							info.ChannelList.Clear();
							string[] channel = Global.GetDefAttributeStr(xmlItem, "Channel", "").Split(new char[]
							{
								'|'
							});
							foreach (string p in channel)
							{
								if (!string.IsNullOrEmpty(p))
								{
									info.ChannelList.Add(p);
								}
							}
							info.PlatformList.Clear();
							string[] platform = Global.GetDefAttributeStr(xmlItem, "Platform", "").Split(new char[]
							{
								'|'
							});
							foreach (string p in platform)
							{
								if (!string.IsNullOrEmpty(p))
								{
									info.PlatformList.Add(Global.SafeConvertToInt32(p));
								}
							}
							string timeBegin = Global.GetDefAttributeStr(xmlItem, "TimeBegin", "");
							string timeEnd = Global.GetDefAttributeStr(xmlItem, "TimeEnd", "");
							if (!string.IsNullOrEmpty(timeBegin))
							{
								info.TimeBegin = DateTime.Parse(timeBegin);
							}
							if (!string.IsNullOrEmpty(timeEnd))
							{
								info.TimeEnd = DateTime.Parse(timeEnd);
							}
							info.ZoneList.Clear();
							string[] zone = Global.GetDefAttributeStr(xmlItem, "Zone", "").Split(new char[]
							{
								'|'
							});
							foreach (string p in zone)
							{
								if (!string.IsNullOrEmpty(p))
								{
									info.ZoneList.Add(Global.SafeConvertToInt32(p));
								}
							}
							info.UserType = (GiftCodeUserType)Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "UserType", "0"));
							info.UseCount = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "UseCount", "0"));
							string goods = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
							if (!string.IsNullOrEmpty(goods))
							{
								string[] fields = goods.Split(new char[]
								{
									'|'
								});
								if (fields.Length > 0)
								{
									info.GoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
								}
							}
							goods = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
							if (!string.IsNullOrEmpty(goods))
							{
								string[] fields = goods.Split(new char[]
								{
									'|'
								});
								if (fields.Length > 0)
								{
									info.ProGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
								}
							}
							newDic.Add(info.GiftCodeTypeID, info);
						}
					}
					GiftCodeNewManager._GiftCodeCfgAwards = newDic;
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("[GiftCodeNew]加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		// Token: 0x06000B76 RID: 2934 RVA: 0x000B43F0 File Offset: 0x000B25F0
		private GiftCodeInfo GetGiftCodeInfo(string giftid)
		{
			GiftCodeInfo result;
			lock (this._lockConfig)
			{
				GiftCodeInfo info = null;
				GiftCodeNewManager._GiftCodeCfgAwards.TryGetValue(giftid, out info);
				result = info;
			}
			return result;
		}

		// Token: 0x06000B77 RID: 2935 RVA: 0x000B444C File Offset: 0x000B264C
		public void ProcessGiftCodeList(string strcmd)
		{
			if (null != strcmd)
			{
				if (!GiftCodeNewManager.IsFuncOpen())
				{
					LogManager.WriteLog(LogTypes.Info, string.Format("[GiftCodeNew]礼包码功能未开放，礼包码信息={0}", strcmd), null, true);
				}
				else
				{
					try
					{
						string[] fields = strcmd.Split(new char[]
						{
							'#'
						});
						if (fields.Length > 0)
						{
							GiftCodeAwardData data = new GiftCodeAwardData();
							for (int i = 0; i < fields.Length; i++)
							{
								string[] GiftData = fields[i].Split(new char[]
								{
									','
								});
								if (GiftData.Length != 4)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("[GiftCodeNew]ProcessGiftCodeList[{0}]参数错误。", fields[i]), null, true);
								}
								else
								{
									data.reset();
									data.UserId = GiftData[0];
									data.RoleID = Convert.ToInt32(GiftData[1]);
									data.GiftId = GiftData[2];
									data.CodeNo = GiftData[3];
									if (data.RoleID <= 0)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("[GiftCodeNew]ProcessGiftCodeList[{0}]角色id错误。", data.RoleID), null, true);
									}
									else
									{
										this.SendAward(null, data);
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						DataHelper.WriteFormatExceptionLog(ex, "[GiftCodeNew]ProcessGiftCodeList error", false, false);
					}
				}
			}
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x000B45B0 File Offset: 0x000B27B0
		public void ProcessGiftCodeCmd(GameClient client, string userId, int roleId, string giftId, string codeNo)
		{
			try
			{
				this.SendAward(client, new GiftCodeAwardData
				{
					UserId = userId,
					RoleID = roleId,
					GiftId = giftId,
					CodeNo = codeNo
				});
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("GiftCodeNew#ProcessGiftCodeCmd#发放领取礼包码失败#rid={0},codeNo={1}", roleId, codeNo), null, true);
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x000B462C File Offset: 0x000B282C
		private void SendAward(GameClient client, GiftCodeAwardData ItemData)
		{
			if (null != ItemData)
			{
				try
				{
					GiftCodeInfo gift = this.GetGiftCodeInfo(ItemData.GiftId);
					if (null == gift)
					{
						this.AddLogEvent(ItemData, -2);
					}
					else if (null != gift.GoodsList)
					{
						int index = 0;
						List<GoodsData> lTmp = new List<GoodsData>();
						foreach (GoodsData item in gift.GoodsList)
						{
							index++;
							lTmp.Add(item);
							if (index % 5 == 0)
							{
								this.SendMailForGiftCode(lTmp, ItemData, gift.GiftCodeName, gift.Description);
								lTmp.Clear();
							}
						}
						if (lTmp.Count > 0)
						{
							this.SendMailForGiftCode(lTmp, ItemData, gift.GiftCodeName, gift.Description);
							lTmp.Clear();
						}
						if (null != client)
						{
							client.ClientData.AddAwardRecord(RoleAwardMsg.LiPinDuiHuan, gift.GoodsList, false);
							GameManager.ClientMgr.NotifyGetAwardMsg(client, RoleAwardMsg.LiPinDuiHuan, "");
						}
					}
				}
				catch (Exception ex)
				{
					this.AddLogEvent(ItemData, -4);
					DataHelper.WriteFormatExceptionLog(ex, "[GiftCodeNew]SendAward error", false, false);
				}
			}
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x000B47C8 File Offset: 0x000B29C8
		private void SendMailForGiftCode(List<GoodsData> GoodList, GiftCodeAwardData ItemData, string subject, string content)
		{
			if (GoodList != null && null != ItemData)
			{
				subject = (string.IsNullOrEmpty(subject) ? GLang.GetLang(121, new object[0]) : subject);
				content = (string.IsNullOrEmpty(content) ? GLang.GetLang(122, new object[0]) : content);
				content = string.Format(content, ItemData.GiftId, ItemData.CodeNo);
				bool bSuccess = Global.UseMailGivePlayerAward3(ItemData.RoleID, GoodList, subject, content, 0, 0, 0);
				if (bSuccess)
				{
					GameClient client = GameManager.ClientMgr.FindClient(ItemData.RoleID);
					if (null != client)
					{
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(123, new object[0]));
					}
					this.AddLogEvent(ItemData, 1);
				}
				else
				{
					this.AddLogEvent(ItemData, -3);
				}
			}
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x000B48B8 File Offset: 0x000B2AB8
		private void AddLogEvent(GiftCodeAwardData ItemData, int result)
		{
			if (null != ItemData)
			{
				EventLogManager.SystemRoleEvents[80].AddImporEvent(new object[]
				{
					GameManager.ServerId,
					ItemData.UserId,
					CacheManager.GetZoneIdByRoleId((long)ItemData.RoleID, GameManager.ServerId),
					ItemData.RoleID,
					ItemData.GiftId,
					ItemData.CodeNo,
					result
				});
			}
		}

		// Token: 0x040012AD RID: 4781
		private static GiftCodeNewManager instance = new GiftCodeNewManager();

		// Token: 0x040012AE RID: 4782
		private object _lockConfig = new object();

		// Token: 0x040012AF RID: 4783
		private static Dictionary<string, GiftCodeInfo> _GiftCodeCfgAwards = new Dictionary<string, GiftCodeInfo>();
	}
}
