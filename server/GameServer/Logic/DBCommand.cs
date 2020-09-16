using System;

namespace GameServer.Logic
{
	
	public class DBCommand
	{
		
		
		
		public int DBCommandID { get; set; }

		
		
		
		public string DBCommandText { get; set; }

		
		// (add) Token: 0x06001FC8 RID: 8136 RVA: 0x001B7AA0 File Offset: 0x001B5CA0
		// (remove) Token: 0x06001FC9 RID: 8137 RVA: 0x001B7ADC File Offset: 0x001B5CDC
		public event DBCommandEventHandler DBCommandEvent;

		
		public void DoDBCommandEvent(DBCommandEventArgs e)
		{
			if (null != this.DBCommandEvent)
			{
				this.DBCommandEvent(this, e);
			}
		}

		
		public int ServerId;
	}
}
