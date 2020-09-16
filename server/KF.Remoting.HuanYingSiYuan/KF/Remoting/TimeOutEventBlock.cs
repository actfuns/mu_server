using System;
using System.Collections.Generic;

namespace KF.Remoting
{
	
	public class TimeOutEventBlock<T>
	{
		
		public DateTime EndTime;

		
		public List<T> ChildList = new List<T>();
	}
}
