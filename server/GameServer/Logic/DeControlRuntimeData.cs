using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class DeControlRuntimeData
	{
		
		public object Mutex = new object();

		
		public bool IsGongNengOpend;

		
		public List<DeControlItem>[] DeControlItemListArray = new List<DeControlItem>[177];
	}
}
