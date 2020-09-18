using System;
using GameServer.Core.GameEvent;

namespace GameServer.Logic.BangHui.ZhanMengShiJian
{
	
	public class ZhanMengShiJianEventListener : IEventListener
	{
		
		private ZhanMengShiJianEventListener()
		{
		}

		
		public static ZhanMengShiJianEventListener getInstance()
		{
			return ZhanMengShiJianEventListener.instance;
		}

		
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				ZhanMengShijianEvent eventObj = (ZhanMengShijianEvent)eventObject;
				ZhanMengShiJianManager.getInstance().addZhanMengShiJian(eventObj.BhId, eventObj.RoleName, eventObj.ShijianType, eventObj.Param1, eventObj.Param2, eventObj.Param3, eventObj.ServerId);
			}
		}

		
		private static ZhanMengShiJianEventListener instance = new ZhanMengShiJianEventListener();
	}
}
