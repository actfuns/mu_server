using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using AutoCSer.Metadata;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using KF.Remoting.KFBoCai;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.TcpCall
{
	// Token: 0x0200002E RID: 46
	[AutoCSer.Net.TcpStaticServer.Server(Name = "KfCall", IsServer = true, IsAttribute = true, IsClientAwaiter = false, MemberFilters = MemberFilters.Static, IsSegmentation = true, ClientSegmentationCopyPath = "GameServer\\Remoting\\")]
	public class KFBoCaiManager
	{
		// Token: 0x0600021F RID: 543 RVA: 0x00020310 File Offset: 0x0001E510
		public static KFBoCaiManager GetInstance()
		{
			return KFBoCaiManager.instance;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x00020327 File Offset: 0x0001E527
		private KFBoCaiManager()
		{
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00020334 File Offset: 0x0001E534
		~KFBoCaiManager()
		{
			this.BackgroundThread.Abort();
			this.BackgroundUpdate.Abort();
		}

		// Token: 0x06000222 RID: 546 RVA: 0x00020378 File Offset: 0x0001E578
		public void StartUp()
		{
			try
			{
				KFBoCaiCaiDaXiao.GetInstance().InitData();
				KFBoCaiCaiShuzi.GetInstance().InitData();
				this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
				this.BackgroundThread.IsBackground = true;
				this.BackgroundThread.Start();
				this.BackgroundUpdate = new Thread(new ParameterizedThreadStart(this.ThreadUpdate));
				this.BackgroundUpdate.IsBackground = true;
				this.BackgroundUpdate.Start();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		// Token: 0x06000223 RID: 547 RVA: 0x00020424 File Offset: 0x0001E624
		public void ThreadProc(object state)
		{
			try
			{
				for (;;)
				{
					KFBoCaiCaiDaXiao.GetInstance().Thread();
					KFBoCaiCaiShuzi.GetInstance().Thread();
					Thread.Sleep(1000);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00020480 File Offset: 0x0001E680
		public void ThreadUpdate(object state)
		{
			try
			{
				for (;;)
				{
					KFBoCaiCaiDaXiao.GetInstance().UpData(false);
					KFBoCaiCaiShuzi.GetInstance().UpData(false);
					Thread.Sleep(50);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x06000225 RID: 549 RVA: 0x000204DC File Offset: 0x0001E6DC
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static bool IsCanBuy(int type, string buyValue, int buyNum, long DataPeriods)
		{
			try
			{
				if (1 == type)
				{
					return KFBoCaiCaiDaXiao.GetInstance().IsCanBuy(buyValue, buyNum, DataPeriods);
				}
				if (2 == type)
				{
					return KFBoCaiCaiShuzi.GetInstance().IsCanBuy(buyValue, buyNum, DataPeriods);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00020550 File Offset: 0x0001E750
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static bool BuyBoCai(KFBuyBocaiData data)
		{
			try
			{
				if (1 == data.BocaiType)
				{
					return KFBoCaiCaiDaXiao.GetInstance().BuyBoCai(data);
				}
				if (2 == data.BocaiType)
				{
					return KFBoCaiCaiShuzi.GetInstance().BuyBoCai(data);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x000205C8 File Offset: 0x0001E7C8
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static OpenLottery GetOpenLottery(int type)
		{
			try
			{
				if (1 == type)
				{
					return KFBoCaiCaiDaXiao.GetInstance().GetOpenLottery();
				}
				if (2 == type)
				{
					return KFBoCaiCaiShuzi.GetInstance().GetOpenLottery();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return null;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00020634 File Offset: 0x0001E834
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static List<OpenLottery> GetOpenLottery(int type, long DataPeriods, bool getOne)
		{
			List<OpenLottery> dList = null;
			try
			{
				if (getOne)
				{
					KFBoCaiDbManager.SelectOpenLottery(type, string.Format(" AND `DataPeriods`={0}", DataPeriods), out dList);
				}
				else if (0 == type)
				{
					if (1 == (int)DataPeriods)
					{
						return KFBoCaiCaiDaXiao.GetInstance().GetOpenHistory();
					}
					if (2 == (int)DataPeriods)
					{
						return KFBoCaiCaiShuzi.GetInstance().GetOpenHistory();
					}
				}
				else
				{
					KFBoCaiDbManager.SelectOpenLottery(type, string.Format(" AND `DataPeriods`>{0}", DataPeriods), out dList);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return dList;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x000206F4 File Offset: 0x0001E8F4
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static KFStageData GetKFStageData(int type)
		{
			try
			{
				if (1 == type)
				{
					return KFBoCaiCaiDaXiao.GetInstance().GetKFStageData();
				}
				if (2 == type)
				{
					return KFBoCaiCaiShuzi.GetInstance().GetKFStageData();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return null;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00020760 File Offset: 0x0001E960
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static List<KFBoCaoHistoryData> GetWinHistory(int type)
		{
			try
			{
				if (1 == type)
				{
					return KFBoCaiCaiDaXiao.GetInstance().GetWinHistory();
				}
				if (2 == type)
				{
					return KFBoCaiCaiShuzi.GetInstance().GetWinHistory();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return null;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x000207CC File Offset: 0x0001E9CC
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static bool BoCaiBuyItem(KFBoCaiShopDB Item, int maxNum)
		{
			try
			{
				return KFBoCaiShopManager.GetInstance().BuyItem(Item, maxNum);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		// Token: 0x04000134 RID: 308
		private static KFBoCaiManager instance = new KFBoCaiManager();

		// Token: 0x04000135 RID: 309
		private Thread BackgroundThread;

		// Token: 0x04000136 RID: 310
		private Thread BackgroundUpdate;

		// Token: 0x0200002F RID: 47
		internal static class TcpStaticServer
		{
			// Token: 0x0600022D RID: 557 RVA: 0x00020820 File Offset: 0x0001EA20
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool _M5(KFBoCaiShopDB Item, int maxNum)
			{
				return KFBoCaiManager.BoCaiBuyItem(Item, maxNum);
			}

			// Token: 0x0600022E RID: 558 RVA: 0x0002083C File Offset: 0x0001EA3C
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool _M6(KFBuyBocaiData data)
			{
				return KFBoCaiManager.BuyBoCai(data);
			}

			// Token: 0x0600022F RID: 559 RVA: 0x00020854 File Offset: 0x0001EA54
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static KFStageData _M7(int type)
			{
				return KFBoCaiManager.GetKFStageData(type);
			}

			// Token: 0x06000230 RID: 560 RVA: 0x0002086C File Offset: 0x0001EA6C
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static OpenLottery _M8(int type)
			{
				return KFBoCaiManager.GetOpenLottery(type);
			}

			// Token: 0x06000231 RID: 561 RVA: 0x00020884 File Offset: 0x0001EA84
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static List<OpenLottery> _M9(int type, long DataPeriods, bool getOne)
			{
				return KFBoCaiManager.GetOpenLottery(type, DataPeriods, getOne);
			}

			// Token: 0x06000232 RID: 562 RVA: 0x000208A0 File Offset: 0x0001EAA0
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static List<KFBoCaoHistoryData> _M10(int type)
			{
				return KFBoCaiManager.GetWinHistory(type);
			}

			// Token: 0x06000233 RID: 563 RVA: 0x000208B8 File Offset: 0x0001EAB8
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool _M11(int type, string buyValue, int buyNum, long DataPeriods)
			{
				return KFBoCaiManager.IsCanBuy(type, buyValue, buyNum, DataPeriods);
			}
		}
	}
}
