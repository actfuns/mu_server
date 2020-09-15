using System;

namespace GameServer.Logic
{
	// Token: 0x020003B3 RID: 947
	internal class TradeLevelLimitConfig
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06001063 RID: 4195 RVA: 0x000FF384 File Offset: 0x000FD584
		// (set) Token: 0x06001064 RID: 4196 RVA: 0x000FF39C File Offset: 0x000FD59C
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

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06001065 RID: 4197 RVA: 0x000FF3A8 File Offset: 0x000FD5A8
		// (set) Token: 0x06001066 RID: 4198 RVA: 0x000FF3C0 File Offset: 0x000FD5C0
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06001067 RID: 4199 RVA: 0x000FF3CC File Offset: 0x000FD5CC
		// (set) Token: 0x06001068 RID: 4200 RVA: 0x000FF3E4 File Offset: 0x000FD5E4
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

		// Token: 0x040018F4 RID: 6388
		private int _ID = 0;

		// Token: 0x040018F5 RID: 6389
		private int _Day = 0;

		// Token: 0x040018F6 RID: 6390
		private string _Limit = "";
	}
}
