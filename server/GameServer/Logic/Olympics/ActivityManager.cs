using System;
using System.Collections.Generic;
using System.Linq;
using Server.Tools.Pattern;

namespace GameServer.Logic.Olympics
{
	
	public class ActivityManager : SingletonTemplate<ActivityManager>
	{
		
		private ActivityManager()
		{
		}

		
		public List<ActivityData> GetActivityList()
		{
			return this._activityDic.Values.ToList<ActivityData>();
		}

		
		public void ActivityAdd(ActivityData data)
		{
			if (!this._activityDic.ContainsKey(data.ActivityType))
			{
				this._activityDic.Add(data.ActivityType, data);
				this.ActivityNotifyState(data);
			}
		}

		
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

		
		public void UpdateActivityData(ActivityData newData)
		{
			if (this._activityDic.ContainsKey(newData.ActivityType))
			{
				this._activityDic[newData.ActivityType] = newData;
				this.ActivityNotifyState(newData);
			}
		}

		
		private void ActivityNotifyState(ActivityData data)
		{
			int index = 0;
			GameClient client;
			while ((client = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
			{
				client.sendCmd<ActivityData>(1005, data, false);
			}
		}

		
		private Dictionary<int, ActivityData> _activityDic = new Dictionary<int, ActivityData>();
	}
}
