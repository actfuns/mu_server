using System;
using GameServer.Core.GameEvent;

namespace GameServer.Logic.BangHui.ZhanMengShiJian
{
	// Token: 0x020005C0 RID: 1472
	public class ZhanMengShiJianEventListener : IEventListener
	{
		// Token: 0x06001AB3 RID: 6835 RVA: 0x00198473 File Offset: 0x00196673
		private ZhanMengShiJianEventListener()
		{
		}

		// Token: 0x06001AB4 RID: 6836 RVA: 0x00198480 File Offset: 0x00196680
		public static ZhanMengShiJianEventListener getInstance()
		{
			return ZhanMengShiJianEventListener.instance;
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x00198498 File Offset: 0x00196698
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 0)
			{
				ZhanMengShijianEvent eventObj = (ZhanMengShijianEvent)eventObject;
				ZhanMengShiJianManager.getInstance().addZhanMengShiJian(eventObj.BhId, eventObj.RoleName, eventObj.ShijianType, eventObj.Param1, eventObj.Param2, eventObj.Param3, eventObj.ServerId);
			}
		}

		// Token: 0x04002975 RID: 10613
		private static ZhanMengShiJianEventListener instance = new ZhanMengShiJianEventListener();
	}
}
