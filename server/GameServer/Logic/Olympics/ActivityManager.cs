using System;
using System.Collections.Generic;
using System.Linq;
using Server.Tools.Pattern;

namespace GameServer.Logic.Olympics
{
	// Token: 0x0200038A RID: 906
	public class ActivityManager : SingletonTemplate<ActivityManager>
	{
		// Token: 0x06000F77 RID: 3959 RVA: 0x000F2D8A File Offset: 0x000F0F8A
		private ActivityManager()
		{
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x000F2DA0 File Offset: 0x000F0FA0
		public List<ActivityData> GetActivityList()
		{
			return this._activityDic.Values.ToList<ActivityData>();
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x000F2DC4 File Offset: 0x000F0FC4
		public void ActivityAdd(ActivityData data)
		{
			if (!this._activityDic.ContainsKey(data.ActivityType))
			{
				this._activityDic.Add(data.ActivityType, data);
				this.ActivityNotifyState(data);
			}
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x000F2E08 File Offset: 0x000F1008
		public void ActivityDel(int activityType)
		{
			if (this._activityDic.ContainsKey(activityType))
			{
				ActivityData oldData = this._activityDic[activityType];
				oldData.ActivityIsOpen = false;
				this._activityDic.Remove(activityType);
				this.ActivityNotifyState(oldData);
			}
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x000F2E54 File Offset: 0x000F1054
		public void UpdateActivityData(ActivityData newData)
		{
			if (this._activityDic.ContainsKey(newData.ActivityType))
			{
				this._activityDic[newData.ActivityType] = newData;
				this.ActivityNotifyState(newData);
			}
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x000F2E94 File Offset: 0x000F1094
		private void ActivityNotifyState(ActivityData data)
		{
			int index = 0;
			GameClient client;
			while ((client = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
			{
				client.sendCmd<ActivityData>(1005, data, false);
			}
		}

		// Token: 0x040017E5 RID: 6117
		private Dictionary<int, ActivityData> _activityDic = new Dictionary<int, ActivityData>();
	}
}
