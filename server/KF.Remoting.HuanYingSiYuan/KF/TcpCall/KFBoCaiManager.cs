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
	
	[AutoCSer.Net.TcpStaticServer.Server(Name = "KfCall", IsServer = true, IsAttribute = true, IsClientAwaiter = false, MemberFilters = MemberFilters.Static, IsSegmentation = true, ClientSegmentationCopyPath = "GameServer\\Remoting\\")]
	public class KFBoCaiManager
	{
		
		public static KFBoCaiManager GetInstance()
		{
			return KFBoCaiManager.instance;
		}

		
		private KFBoCaiManager()
		{
		}

		
		~KFBoCaiManager()
		{
			this.BackgroundThread.Abort();
			this.BackgroundUpdate.Abort();
		}

		
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

		
		private static KFBoCaiManager instance = new KFBoCaiManager();

		
		private Thread BackgroundThread;

		
		private Thread BackgroundUpdate;

		
		internal static class TcpStaticServer
		{
			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool _M5(KFBoCaiShopDB Item, int maxNum)
			{
				return KFBoCaiManager.BoCaiBuyItem(Item, maxNum);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool _M6(KFBuyBocaiData data)
			{
				return KFBoCaiManager.BuyBoCai(data);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static KFStageData _M7(int type)
			{
				return KFBoCaiManager.GetKFStageData(type);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static OpenLottery _M8(int type)
			{
				return KFBoCaiManager.GetOpenLottery(type);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static List<OpenLottery> _M9(int type, long DataPeriods, bool getOne)
			{
				return KFBoCaiManager.GetOpenLottery(type, DataPeriods, getOne);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static List<KFBoCaoHistoryData> _M10(int type)
			{
				return KFBoCaiManager.GetWinHistory(type);
			}

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool _M11(int type, string buyValue, int buyNum, long DataPeriods)
			{
				return KFBoCaiManager.IsCanBuy(type, buyValue, buyNum, DataPeriods);
			}
		}
	}
}
