using System;
using System.Collections;
using System.Threading;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020001D4 RID: 468
	public class DelayExecModule
	{
		// Token: 0x060005ED RID: 1517 RVA: 0x00054294 File Offset: 0x00052494
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

		// Token: 0x060005EE RID: 1518 RVA: 0x00054324 File Offset: 0x00052524
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

		// Token: 0x060005EF RID: 1519 RVA: 0x00054404 File Offset: 0x00052604
		private void RecalcProps(GameClient client)
		{
			Global.RefreshEquipProp(client);
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x0005440E File Offset: 0x0005260E
		private void UpdateOtherProps(GameClient client)
		{
			Global.UpdateHorseDataProps(client, true);
			Global.UpdateJingMaiListProps(client, true);
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x00054424 File Offset: 0x00052624
		private void NotifyRefreshProps(GameClient client)
		{
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x00054475 File Offset: 0x00052675
		private static void ProcessClickGoodsPack(GameClient client)
		{
			GameManager.GoodsPackMgr.ProcessClickGoodsPackWhenMovingToOtherGrid(client, 1);
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x000544B8 File Offset: 0x000526B8
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

		// Token: 0x04000A5E RID: 2654
		private object mutex = new object();

		// Token: 0x04000A5F RID: 2655
		public BitArray DelayExecPorcsBits = new BitArray(4);
	}
}
