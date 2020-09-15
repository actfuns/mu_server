using System;
using System.Collections.Generic;
using System.Reflection;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.GoldAuction
{
	// Token: 0x02000096 RID: 150
	public class CopyData
	{
		// Token: 0x0600026C RID: 620 RVA: 0x0002A2D8 File Offset: 0x000284D8
		public static void Copy<T>(T sData, ref T rData)
		{
			try
			{
				foreach (FieldInfo info in rData.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					info.SetValue(rData, info.GetValue(sData));
				}
			}
			catch (Exception ex)
			{
				rData = default(T);
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0002A370 File Offset: 0x00028570
		public static void CopyGoldAuctionItem(GoldAuctionItem sData, ref GoldAuctionItem rData)
		{
			try
			{
				rData = new GoldAuctionItem();
				CopyData.Copy<GoldAuctionItem>(sData, ref rData);
				rData.RoleList = new List<AuctionRoleData>();
				foreach (AuctionRoleData item in sData.RoleList)
				{
					AuctionRoleData temp = new AuctionRoleData();
					CopyData.Copy<AuctionRoleData>(item, ref temp);
					rData.RoleList.Add(temp);
				}
			}
			catch (Exception ex)
			{
				rData = null;
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0002A434 File Offset: 0x00028634
		public static void CopyAuctionItem2DB(GoldAuctionItem sData, out GoldAuctionDBItem rData)
		{
			try
			{
				rData = new GoldAuctionDBItem();
				CopyData.Copy<GoldAuctionDBItem>(sData, ref rData);
				rData.BuyerData = new AuctionRoleData();
				CopyData.Copy<AuctionRoleData>(sData.BuyerData, ref rData.BuyerData);
				rData.RoleList = new List<AuctionRoleData>();
				foreach (AuctionRoleData item in sData.RoleList)
				{
					AuctionRoleData temp = new AuctionRoleData();
					CopyData.Copy<AuctionRoleData>(item, ref temp);
					rData.RoleList.Add(temp);
				}
			}
			catch (Exception ex)
			{
				rData = null;
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0002A518 File Offset: 0x00028718
		public static void CopyAuctionDB2Item(GoldAuctionDBItem sData, out GoldAuctionItem rData)
		{
			try
			{
				rData = new GoldAuctionItem();
				rData.ProductionTime = sData.ProductionTime;
				rData.AuctionTime = sData.AuctionTime;
				rData.AuctionSource = sData.AuctionSource;
				rData.StrGoods = sData.StrGoods;
				rData.BuyerData = new AuctionRoleData();
				CopyData.Copy<AuctionRoleData>(sData.BuyerData, ref rData.BuyerData);
				foreach (AuctionRoleData item in sData.RoleList)
				{
					AuctionRoleData temp = new AuctionRoleData();
					CopyData.Copy<AuctionRoleData>(item, ref temp);
					rData.RoleList.Add(temp);
				}
				rData.BossLife = sData.BossLife;
				rData.KillBossRoleID = sData.KillBossRoleID;
				rData.UpDBWay = sData.UpDBWay;
				rData.AuctionType = sData.AuctionType;
				rData.OldAuctionType = sData.OldAuctionType;
				rData.Lock = false;
			}
			catch (Exception ex)
			{
				rData = null;
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0002A688 File Offset: 0x00028888
		public static bool Copy2AuctionItemS2C(GoldAuctionItem sData, out AuctionItemS2C rData, string Seach, int Color)
		{
			rData = new AuctionItemS2C();
			try
			{
				rData.Goods = GlobalNew.ParseGoodsData(sData.StrGoods);
				int goodsID = rData.Goods.GoodsID;
				if (!string.IsNullOrEmpty(Seach))
				{
					List<int> goodsIDs = Global.StringToIntList(Seach, '#');
					if (goodsIDs.Find((int x) => x == goodsID) < 1)
					{
						return false;
					}
				}
				int color = Global.GetEquipColor(rData.Goods);
				if (Color > 0 && (1 << color - 1 & Color) == 0)
				{
					return false;
				}
				rData.BuyRoleId = sData.BuyerData.m_RoleID;
				rData.Price = sData.BuyerData.Value;
				rData.AuctionItemKey = string.Format("{0}|{1}", sData.ProductionTime.Replace(':', ','), sData.AuctionSource);
				AuctionConfig cfg = GoldAuctionConfigModel.GetAuctionConfig(sData.AuctionSource);
				if (null != cfg)
				{
					rData.MaxPrice = (long)cfg.MaxPrice;
					rData.UnitPrice = (long)cfg.UnitPrice;
				}
				rData.LastTime = TimeUtil.GetDiffTimeSeconds(DateTime.Parse(sData.AuctionTime).AddHours((double)sData.LifeTime), TimeUtil.NowDateTime(), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0002A844 File Offset: 0x00028A44
		public static bool Copy2AuctionItemS2C(AuctionItemS2C sData, out AuctionItemS2C rData, GoldAuctionItem AuctionItem)
		{
			rData = new AuctionItemS2C();
			try
			{
				CopyData.Copy<AuctionItemS2C>(sData, ref rData);
				rData.Goods = new GoodsData();
				if (null != AuctionItem)
				{
					rData.LastTime = TimeUtil.GetDiffTimeSeconds(DateTime.Parse(AuctionItem.AuctionTime).AddHours((double)AuctionItem.LifeTime), TimeUtil.NowDateTime(), false);
				}
				CopyData.Copy<GoodsData>(sData.Goods, ref rData.Goods);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}
	}
}
