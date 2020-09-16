using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class GiftCodeNewManager : IManager
	{
		
		public static GiftCodeNewManager getInstance()
		{
			return GiftCodeNewManager.instance;
		}

		
		public bool initialize()
		{
			return this.initGiftCode();
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

		
		public static bool IsFuncOpen()
		{
			return true;
		}

		
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

		
		private static GiftCodeNewManager instance = new GiftCodeNewManager();

		
		private object _lockConfig = new object();

		
		private static Dictionary<string, GiftCodeInfo> _GiftCodeCfgAwards = new Dictionary<string, GiftCodeInfo>();
	}
}
