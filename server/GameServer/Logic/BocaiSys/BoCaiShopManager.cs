using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	
	public class BoCaiShopManager
	{
		
		private BoCaiShopManager()
		{
		}

		
		public static BoCaiShopManager GetInstance()
		{
			return BoCaiShopManager.instance;
		}

		
		public void Init()
		{
			try
			{
				lock (this.mutex)
				{
					this.StartTime = TimeUtil.NowDateTime();
					BoCaiShopDBData DBData = Global.sendToDB<BoCaiShopDBData, string>(2085, "", 0);
					if (DBData == null || !DBData.Flag)
					{
						LogManager.WriteLog(LogTypes.Error, "[ljl_博彩商店] db get fail", null, true);
					}
					else if (null == DBData.ItemList)
					{
						this.DBShopList = new List<KFBoCaiShopDB>();
					}
					else
					{
						this.DBShopList = DBData.ItemList;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
		}

		
		public void Update()
		{
			try
			{
				if (this.StartTime.Day != TimeUtil.NowDateTime().Day)
				{
					this.StartTime = TimeUtil.NowDateTime();
					lock (this.mutex)
					{
						this.DBShopList.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
		}

		
		public void InsertBocaiShop(KFBoCaiShopDB data)
		{
			try
			{
				if (!Global.Send2DB<KFBoCaiShopDB>(2086, data, 0).Equals(true.ToString()))
				{
					LogManager.WriteLog(LogTypes.Warning, "[ljl_博彩商店] InsertBocaiShop fail", null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
		}

		
		public void GetSelfBuyData(int roleID, ref BoCaiShopInfo msgData)
		{
			try
			{
				msgData.ItemList = new List<SelfBuyInfo>();
				lock (this.mutex)
				{
					foreach (KFBoCaiShopDB item in this.DBShopList)
					{
						if (item.RoleID == roleID)
						{
							SelfBuyInfo d = new SelfBuyInfo();
							d.ID = item.ID;
							d.BuyNum = item.BuyNum;
							d.WuPinID = item.WuPinID;
							msgData.ItemList.Add(d);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
		}

		
		public bool CanBuyItem(KFBoCaiShopDB Item, int maxNum)
		{
			try
			{
				KFBoCaiShopDB temp = null;
				lock (this.mutex)
				{
					temp = this.DBShopList.Find((KFBoCaiShopDB x) => x.ID == Item.ID && x.WuPinID.Equals(Item.WuPinID) && x.RoleID == Item.RoleID);
					if (null == temp)
					{
						temp = Item;
						this.DBShopList.Add(Item);
					}
					else
					{
						if (temp.BuyNum + Item.BuyNum > maxNum)
						{
							return false;
						}
						temp.BuyNum += Item.BuyNum;
					}
				}
				this.InsertBocaiShop(temp);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		
		private static BoCaiShopManager instance = new BoCaiShopManager();

		
		private DateTime StartTime;

		
		private object mutex = new object();

		
		private List<KFBoCaiShopDB> DBShopList;
	}
}
