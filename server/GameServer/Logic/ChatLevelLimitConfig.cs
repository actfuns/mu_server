using System;

namespace GameServer.Logic
{
	// Token: 0x020003B5 RID: 949
	internal class ChatLevelLimitConfig
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600106B RID: 4203 RVA: 0x000FF418 File Offset: 0x000FD618
		// (set) Token: 0x0600106C RID: 4204 RVA: 0x000FF430 File Offset: 0x000FD630
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				this._ID = value;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600106D RID: 4205 RVA: 0x000FF43C File Offset: 0x000FD63C
		// (set) Token: 0x0600106E RID: 4206 RVA: 0x000FF454 File Offset: 0x000FD654
		public int Day
		{
			get
			{
				return this._Day;
			}
			set
			{
				this._Day = value;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600106F RID: 4207 RVA: 0x000FF460 File Offset: 0x000FD660
		// (set) Token: 0x06001070 RID: 4208 RVA: 0x000FF478 File Offset: 0x000FD678
		public string Limit
		{
			get
			{
				return this._Limit;
			}
			set
			{
				this._Limit = value;
			}
		}

		// Token: 0x040018FE RID: 6398
		private int _ID = 0;

		// Token: 0x040018FF RID: 6399
		private int _Day = 0;

		// Token: 0x04001900 RID: 6400
		private string _Limit = "";
	}
}
