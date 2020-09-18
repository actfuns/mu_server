using System;
using GameServer.Core.GameEvent;

namespace GameServer.Logic.BossAI
{
	
	public class BossAIManager : IManager
	{
		
		private BossAIManager()
		{
		}

		
		public static BossAIManager getInstance()
		{
			return BossAIManager.instance;
		}

		
		public bool initialize()
		{
			GlobalEventSource.getInstance().registerListener(16, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(11, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(17, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(19, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(18, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(20, BossAIEventListener.getInstance());
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
			GlobalEventSource.getInstance().removeListener(16, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(11, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(17, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(19, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(18, BossAIEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(20, BossAIEventListener.getInstance());
			return true;
		}

		
		private static BossAIManager instance = new BossAIManager();
	}
}
