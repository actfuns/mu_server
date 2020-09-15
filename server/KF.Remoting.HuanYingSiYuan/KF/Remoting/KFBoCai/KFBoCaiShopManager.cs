using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.KFBoCai
{
	// Token: 0x02000031 RID: 49
	public class KFBoCaiShopManager
	{
		// Token: 0x06000249 RID: 585 RVA: 0x000223F8 File Offset: 0x000205F8
		public static KFBoCaiShopManager GetInstance()
		{
			return KFBoCaiShopManager.instance;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0002240F File Offset: 0x0002060F
		private KFBoCaiShopManager()
		{
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00022430 File Offset: 0x00020630
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

		// Token: 0x0600024C RID: 588 RVA: 0x000224E8 File Offset: 0x000206E8
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

		// Token: 0x0600024D RID: 589 RVA: 0x000225E8 File Offset: 0x000207E8
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

		// Token: 0x0600024E RID: 590 RVA: 0x00022734 File Offset: 0x00020934
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

		// Token: 0x0400013E RID: 318
		private static KFBoCaiShopManager instance = new KFBoCaiShopManager();

		// Token: 0x0400013F RID: 319
		private List<KFBoCaiShopDB> cacheList = new List<KFBoCaiShopDB>();

		// Token: 0x04000140 RID: 320
		private DateTime StartTime;

		// Token: 0x04000141 RID: 321
		public object mutex = new object();
	}
}
