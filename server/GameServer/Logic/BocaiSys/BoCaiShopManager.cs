using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	// Token: 0x0200007E RID: 126
	public class BoCaiShopManager
	{
		// Token: 0x060001E2 RID: 482 RVA: 0x00020628 File Offset: 0x0001E828
		private BoCaiShopManager()
		{
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00020640 File Offset: 0x0001E840
		public static BoCaiShopManager GetInstance()
		{
			return BoCaiShopManager.instance;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00020658 File Offset: 0x0001E858
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

		// Token: 0x060001E5 RID: 485 RVA: 0x00020744 File Offset: 0x0001E944
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

		// Token: 0x060001E6 RID: 486 RVA: 0x000207FC File Offset: 0x0001E9FC
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

		// Token: 0x060001E7 RID: 487 RVA: 0x00020870 File Offset: 0x0001EA70
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

		// Token: 0x060001E8 RID: 488 RVA: 0x000209E4 File Offset: 0x0001EBE4
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

		// Token: 0x040002DB RID: 731
		private static BoCaiShopManager instance = new BoCaiShopManager();

		// Token: 0x040002DC RID: 732
		private DateTime StartTime;

		// Token: 0x040002DD RID: 733
		private object mutex = new object();

		// Token: 0x040002DE RID: 734
		private List<KFBoCaiShopDB> DBShopList;
	}
}
