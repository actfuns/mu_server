using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	// Token: 0x0200007F RID: 127
	public class TimeOutEventBlock<T>
	{
		// Token: 0x04000374 RID: 884
		public DateTime EndTime;

		// Token: 0x04000375 RID: 885
		public List<T> ChildList = new List<T>();
	}
}
