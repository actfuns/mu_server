using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.KFBoCai
{
	
	public class KFBoCaiShopManager
	{
		
		public static KFBoCaiShopManager GetInstance()
		{
			return KFBoCaiShopManager.instance;
		}

		
		private KFBoCaiShopManager()
		{
		}

		
		public void InitData()
		{
			try
			{
				lock (this.mutex)
				{
					this.StartTime = TimeUtil.NowDateTime();
					string Periods = TimeUtil.DataTimeToString(this.StartTime, "yyMMdd");
					KFBoCaiDbManager.DelTableData("t_bocai_shop", string.Format("Periods!='{0}'", Periods));
					KFBoCaiDbManager.SelectBoCaiShop(Periods, out this.cacheList);
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
						this.cacheList.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
		}

		
		private bool AddItem(KFBoCaiShopDB Item)
		{
			try
			{
				lock (this.mutex)
				{
					KFBoCaiShopDB data = this.cacheList.Find((KFBoCaiShopDB x) => x.ID == Item.ID && Item.WuPinID == x.WuPinID);
					if (null == data)
					{
						data = Item;
						this.cacheList.Add(Item);
					}
					else
					{
						data.BuyNum += Item.BuyNum;
					}
					KFBoCaiDbManager.ReplaceBoCaiShop(data);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		
		public bool BuyItem(KFBoCaiShopDB Item, int maxNum)
		{
			try
			{
				lock (this.mutex)
				{
					KFBoCaiShopDB data = this.cacheList.Find((KFBoCaiShopDB x) => x.ID == Item.ID && Item.WuPinID == x.WuPinID);
					if (null == data)
					{
						return this.AddItem(Item);
					}
					if (data.BuyNum + Item.BuyNum <= maxNum)
					{
						return this.AddItem(Item);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩商店]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		
		private static KFBoCaiShopManager instance = new KFBoCaiShopManager();

		
		private List<KFBoCaiShopDB> cacheList = new List<KFBoCaiShopDB>();

		
		private DateTime StartTime;

		
		public object mutex = new object();
	}
}
