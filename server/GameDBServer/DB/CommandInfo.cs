using System;
using MySQLDriverCS;

namespace GameDBServer.DB
{
	
	public class CommandInfo
	{
		
		// (add) Token: 0x0600041F RID: 1055 RVA: 0x00020094 File Offset: 0x0001E294
		// (remove) Token: 0x06000420 RID: 1056 RVA: 0x000200D0 File Offset: 0x0001E2D0
		private event EventHandler _solicitationEvent;

		
		// (add) Token: 0x06000421 RID: 1057 RVA: 0x0002010C File Offset: 0x0001E30C
		// (remove) Token: 0x06000422 RID: 1058 RVA: 0x00020117 File Offset: 0x0001E317
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

		
		public CommandInfo(string sqlText, MySQLParameter[] para)
		{
			this.CommandText = sqlText;
			this.Parameters = para;
		}

		
		public CommandInfo(string sqlText, MySQLParameter[] para, EffentNextType type)
		{
			this.CommandText = sqlText;
			this.Parameters = para;
			this.EffentNextType = type;
		}

		
		public object ShareObject = null;

		
		public object OriginalData = null;

		
		public string CommandText;

		
		public MySQLParameter[] Parameters;

		
		public EffentNextType EffentNextType = EffentNextType.None;
	}
}
