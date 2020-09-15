using System;
using MySQLDriverCS;

namespace GameDBServer.DB
{
	// Token: 0x020000F1 RID: 241
	public class CommandInfo
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600041F RID: 1055 RVA: 0x00020094 File Offset: 0x0001E294
		// (remove) Token: 0x06000420 RID: 1056 RVA: 0x000200D0 File Offset: 0x0001E2D0
		private event EventHandler _solicitationEvent;

		// Token: 0x14000002 RID: 2
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

		// Token: 0x06000423 RID: 1059 RVA: 0x00020124 File Offset: 0x0001E324
		public void OnSolicitationEvent()
		{
			if (this._solicitationEvent != null)
			{
				this._solicitationEvent(this, new EventArgs());
			}
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x00020153 File Offset: 0x0001E353
		public CommandInfo()
		{
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x00020173 File Offset: 0x0001E373
		public CommandInfo(string sqlText, MySQLParameter[] para)
		{
			this.CommandText = sqlText;
			this.Parameters = para;
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x000201A1 File Offset: 0x0001E3A1
		public CommandInfo(string sqlText, MySQLParameter[] para, EffentNextType type)
		{
			this.CommandText = sqlText;
			this.Parameters = para;
			this.EffentNextType = type;
		}

		// Token: 0x040006D8 RID: 1752
		public object ShareObject = null;

		// Token: 0x040006D9 RID: 1753
		public object OriginalData = null;

		// Token: 0x040006DB RID: 1755
		public string CommandText;

		// Token: 0x040006DC RID: 1756
		public MySQLParameter[] Parameters;

		// Token: 0x040006DD RID: 1757
		public EffentNextType EffentNextType = EffentNextType.None;
	}
}
