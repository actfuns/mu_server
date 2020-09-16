using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace GameServer.Server.CmdProcesser
{
	
	public class CmdRegisterTriggerManager : IManager
	{
		
		private CmdRegisterTriggerManager()
		{
		}

		
		public static CmdRegisterTriggerManager getInstance()
		{
			return CmdRegisterTriggerManager.instance;
		}

		
		private void TriggerProcessor(ICmdProcessor icp)
		{
		}

		
		public bool initialize()
		{
			this.TriggerProcessor(ZhanMengBuildUpLevelCmdProcessor.getInstance());
			this.TriggerProcessor(ZhanMengBuildGetBufferCmdProcessor.getInstance());
			this.TriggerProcessor(WingOffOnCmdProcessor.getInstance());
			this.TriggerProcessor(WingUpgradeCmdProcessor.getInstance());
			this.TriggerProcessor(WingUpStarCmdProcessor.getInstance());
			this.TriggerProcessor(GetWingInfoCmdProcessor.getInstance());
			this.TriggerProcessor(GetSweepWanMoTaAwardCmdProcessor.getInstance());
			this.TriggerProcessor(GetWanMoTaDetailCmdProcessor.getInstance());
			this.TriggerProcessor(SweepWanMoTaCmdProcessor.getInstance());
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
			return true;
		}

		
		private static CmdRegisterTriggerManager instance = new CmdRegisterTriggerManager();

		
		private List<ICmdProcessor> CmdProcessorList = new List<ICmdProcessor>();
	}
}
