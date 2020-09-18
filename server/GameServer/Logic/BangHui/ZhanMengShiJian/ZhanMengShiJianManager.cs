using System;
using GameServer.Core.GameEvent;
using GameServer.Server;
using GameServer.Server.CmdProcesser;

namespace GameServer.Logic.BangHui.ZhanMengShiJian
{
	
	public class ZhanMengShiJianManager : IManager
	{
		
		private ZhanMengShiJianManager()
		{
		}

		
		public static ZhanMengShiJianManager getInstance()
		{
			return ZhanMengShiJianManager.instance;
		}

		
		public bool initialize()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(566, 2, ZhanMengShiJianDetailCmdProcessor.getInstance());
			GlobalEventSource.getInstance().registerListener(0, ZhanMengShiJianEventListener.getInstance());
			return true;
		}

		
		public bool startup()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			GlobalEventSource.getInstance().removeListener(0, ZhanMengShiJianEventListener.getInstance());
			return true;
		}

		
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

		
		private static ZhanMengShiJianManager instance = new ZhanMengShiJianManager();
	}
}
