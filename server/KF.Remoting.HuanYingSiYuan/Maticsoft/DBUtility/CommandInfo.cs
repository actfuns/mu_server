using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Maticsoft.DBUtility
{
	// Token: 0x02000049 RID: 73
	public class CommandInfo
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600030B RID: 779 RVA: 0x0002B1C0 File Offset: 0x000293C0
		// (remove) Token: 0x0600030C RID: 780 RVA: 0x0002B1FC File Offset: 0x000293FC
		private event EventHandler _solicitationEvent;

		// Token: 0x14000002 RID: 2
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

		// Token: 0x0600030F RID: 783 RVA: 0x0002B250 File Offset: 0x00029450
		public void OnSolicitationEvent()
		{
			if (this._solicitationEvent != null)
			{
				this._solicitationEvent(this, new EventArgs());
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0002B27F File Offset: 0x0002947F
		public CommandInfo()
		{
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0002B29F File Offset: 0x0002949F
		public CommandInfo(string sqlText, SqlParameter[] para)
		{
			this.CommandText = sqlText;
			this.Parameters = para;
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0002B2CD File Offset: 0x000294CD
		public CommandInfo(string sqlText, SqlParameter[] para, EffentNextType type)
		{
			this.CommandText = sqlText;
			this.Parameters = para;
			this.EffentNextType = type;
		}

		// Token: 0x040001CC RID: 460
		public object ShareObject = null;

		// Token: 0x040001CD RID: 461
		public object OriginalData = null;

		// Token: 0x040001CF RID: 463
		public string CommandText;

		// Token: 0x040001D0 RID: 464
		public DbParameter[] Parameters;

		// Token: 0x040001D1 RID: 465
		public EffentNextType EffentNextType = EffentNextType.None;
	}
}
