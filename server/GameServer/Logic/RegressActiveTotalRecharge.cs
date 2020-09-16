using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class RegressActiveTotalRecharge : Activity
	{
		
		public bool Init()
		{
			this.ActivityType = 112;
			this.FromDate = "-1";
			this.ToDate = "-1";
			this.AwardStartDate = "-1";
			this.AwardEndDate = "-1";
			string fileName = Global.GameResPath("Config\\HuiGuiChongZhiGift.xml");
			XElement xml = XElement.Load(fileName);
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
			}
			try
			{
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					RegressActiveTotalRechargeXML Regress = new RegressActiveTotalRechargeXML();
					Regress.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
					Regress.HuoDongLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "HuoDongLevel"));
					Regress.MinYuanBao = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "MinYuanBao"));
					string goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID1");
					if (!string.IsNullOrEmpty(goods))
					{
						string[] fields = goods.Split(new char[]
						{
							'|'
						});
						if (fields.Length > 0)
						{
							Regress.GoodsID1 = GoodsHelper.ParseGoodsDataList(fields, fileName);
						}
					}
					goods = Global.GetSafeAttributeStr(xmlItem, "GoodsID2");
					if (!string.IsNullOrEmpty(goods))
					{
						string[] fields = goods.Split(new char[]
						{
							'|'
						});
						if (fields.Length > 0)
						{
							Regress.GoodsID2 = GoodsHelper.ParseGoodsDataList(fields, fileName);
						}
					}
					this.regressActiveTotalRechargeXML.Add(Regress.ID, Regress);
				}
				if (this.regressActiveTotalRechargeXML == null)
				{
					return false;
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return true;
		}

		
		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					16,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					16,
					RegressActiveOpen.OpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
		}

		
		public bool GiveAwardCheck(GameClient client, int Level, int Money, int RechargeConfID, out List<GoodsData> goodsList)
		{
			goodsList = null;
			RegressActiveTotalRechargeXML Conf;
			bool result;
			if (!this.regressActiveTotalRechargeXML.TryGetValue(RechargeConfID, out Conf))
			{
				result = false;
			}
			else if (Conf.HuoDongLevel != Level)
			{
				result = false;
			}
			else if (Money < Conf.MinYuanBao)
			{
				result = false;
			}
			else
			{
				goodsList = Conf.GoodsID1;
				result = false;
			}
			return result;
		}

		
		public bool GiveAward(GameClient client, List<GoodsData> goodsData)
		{
			bool result;
			if (goodsData == null)
			{
				result = false;
			}
			else
			{
				foreach (GoodsData it in goodsData)
				{
					if (Global.GetGoodsRebornEquip(it.GoodsID) == 1)
					{
						Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, it.GoodsID, it.GCount, it.Quality, it.Props, it.Forge_level, it.Binding, 15000, it.Jewellist, true, 1, "三周年回归活动累计充值奖励", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
					}
					else
					{
						Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, it.GoodsID, it.GCount, it.Quality, it.Props, it.Forge_level, it.Binding, 0, it.Jewellist, true, 1, "三周年回归活动累计充值奖励", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
					}
				}
				result = true;
			}
			return result;
		}

		
		protected const string RegressActiveTotalRechargeXml = "Config\\HuiGuiChongZhiGift.xml";

		
		private Dictionary<int, RegressActiveTotalRechargeXML> regressActiveTotalRechargeXML = new Dictionary<int, RegressActiveTotalRechargeXML>();
	}
}
