using System;

namespace KF.Contract.Data
{
	
	[Serializable]
	public class AsyncDataItem
	{
		
		public KuaFuEventTypes EventType;

		
		public object[] Args;
	}
}
