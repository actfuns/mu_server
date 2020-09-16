using System;
using System.Collections.Generic;
using System.Reflection;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.GoldAuction
{
	
	public class CopyData
	{
		
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
