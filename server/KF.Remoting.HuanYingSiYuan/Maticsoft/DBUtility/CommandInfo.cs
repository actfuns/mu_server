using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Maticsoft.DBUtility
{
	
	public class CommandInfo
	{
		
		// (add) Token: 0x0600030B RID: 779 RVA: 0x0002B1C0 File Offset: 0x000293C0
		// (remove) Token: 0x0600030C RID: 780 RVA: 0x0002B1FC File Offset: 0x000293FC
		private event EventHandler _solicitationEvent;

		
		// (add) Token: 0x0600030D RID: 781 RVA: 0x0002B238 File Offset: 0x00029438
		// (remove) Token: 0x0600030E RID: 782 RVA: 0x0002B243 File Offset: 0x00029443
		public event EventHandler SolicitationEvent
		{
			add
			{
				this._solicitationEvent += value;
			}
			remove
			{
				this._solicitationEvent -= value;
			}
		}

		
		public void OnSolicitationEvent()
		{
			if (this._solicitationEvent != null)
			{
				this._solicitationEvent(this, new EventArgs());
			}
		}

		
		public CommandInfo()
		{
		}

		
		public CommandInfo(string sqlText, SqlParameter[] para)
		{
			this.CommandText = sqlText;
			this.Parameters = para;
		}

		
		public CommandInfo(string sqlText, SqlParameter[] para, EffentNextType type)
		{
			this.CommandText = sqlText;
			this.Parameters = para;
			this.EffentNextType = type;
		}

		
		public object ShareObject = null;

		
		public object OriginalData = null;

		
		public string CommandText;

		
		public DbParameter[] Parameters;

		
		public EffentNextType EffentNextType = EffentNextType.None;
	}
}
