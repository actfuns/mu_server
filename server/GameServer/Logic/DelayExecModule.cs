using System;
using System.Collections;
using System.Threading;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class DelayExecModule
	{
		
		public void SetDelayExecProc(params DelayExecProcIds[] procIds)
		{
			if (procIds != null && procIds.Length != 0)
			{
				lock (this.mutex)
				{
					foreach (DelayExecProcIds procId in procIds)
					{
						this.DelayExecPorcsBits.Set((int)procId, true);
					}
				}
			}
		}

		
		public void ExecDelayProcs(GameClient client)
		{
			bool recalcProps;
			bool updateOtherProps;
			bool notifyRefreshProps;
			bool processClickGoodsPack;
			lock (this.mutex)
			{
				recalcProps = this.DelayExecPorcsBits.Get(0);
				updateOtherProps = this.DelayExecPorcsBits.Get(1);
				notifyRefreshProps = this.DelayExecPorcsBits.Get(2);
				processClickGoodsPack = this.DelayExecPorcsBits.Get(3);
				this.DelayExecPorcsBits.SetAll(false);
			}
			if (recalcProps)
			{
				this.RecalcProps(client);
			}
			if (updateOtherProps)
			{
				this.UpdateOtherProps(client);
			}
			if (notifyRefreshProps || recalcProps)
			{
				this.NotifyRefreshProps(client);
			}
			if (processClickGoodsPack)
			{
				GameManager.GoodsPackMgr.ProcessClickGoodsPackWhenMovingToOtherGrid(client, 1);
			}
		}

		
		private void RecalcProps(GameClient client)
		{
			Global.RefreshEquipProp(client);
		}

		
		private void UpdateOtherProps(GameClient client)
		{
			Global.UpdateHorseDataProps(client, true);
			Global.UpdateJingMaiListProps(client, true);
		}

		
		private void NotifyRefreshProps(GameClient client)
		{
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
		}

		
		private static void ProcessClickGoodsPack(GameClient client)
		{
			GameManager.GoodsPackMgr.ProcessClickGoodsPackWhenMovingToOtherGrid(client, 1);
		}

		
		public static void DelayClose(GameClient client, string msg, string log, int minutes)
		{
			BanManager.BanRoleName(Global.FormatRoleName(client, client.ClientData.RoleName), minutes, 1);
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
			ThreadPool.QueueUserWorkItem(delegate(object x)
			{
				Thread.Sleep(Global.GetRandomNumber(900, 5500));
				Global.ForceCloseClient(client, msg, true);
			});
		}

		
		private object mutex = new object();

		
		public BitArray DelayExecPorcsBits = new BitArray(4);
	}
}
