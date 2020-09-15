using System;
using GameServer.Core.GameEvent;
using GameServer.Server;
using GameServer.Server.CmdProcesser;

namespace GameServer.Logic.BangHui.ZhanMengShiJian
{
	// Token: 0x020005BF RID: 1471
	public class ZhanMengShiJianManager : IManager
	{
		// Token: 0x06001AAB RID: 6827 RVA: 0x00198357 File Offset: 0x00196557
		private ZhanMengShiJianManager()
		{
		}

		// Token: 0x06001AAC RID: 6828 RVA: 0x00198364 File Offset: 0x00196564
		public static ZhanMengShiJianManager getInstance()
		{
			return ZhanMengShiJianManager.instance;
		}

		// Token: 0x06001AAD RID: 6829 RVA: 0x0019837C File Offset: 0x0019657C
		public bool initialize()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(566, 2, ZhanMengShiJianDetailCmdProcessor.getInstance());
			GlobalEventSource.getInstance().registerListener(0, ZhanMengShiJianEventListener.getInstance());
			return true;
		}

		// Token: 0x06001AAE RID: 6830 RVA: 0x001983B8 File Offset: 0x001965B8
		public bool startup()
		{
			return true;
		}

		// Token: 0x06001AAF RID: 6831 RVA: 0x001983CC File Offset: 0x001965CC
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06001AB0 RID: 6832 RVA: 0x001983E0 File Offset: 0x001965E0
		public bool destroy()
		{
			GlobalEventSource.getInstance().removeListener(0, ZhanMengShiJianEventListener.getInstance());
			return true;
		}

		// Token: 0x06001AB1 RID: 6833 RVA: 0x00198404 File Offset: 0x00196604
		public void addZhanMengShiJian(int BhId, string RoleName, int ShijianType, int Param1, int Param2, int Param3, int serverId)
		{
			string cmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				BhId,
				RoleName,
				ShijianType,
				Param1,
				Param2,
				Param3
			});
			Global.sendToDB<string, string>(10138, cmd, serverId);
		}

		// Token: 0x04002974 RID: 10612
		private static ZhanMengShiJianManager instance = new ZhanMengShiJianManager();
	}
}
