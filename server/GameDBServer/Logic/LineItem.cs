using System;
using GameDBServer.Server;

namespace GameDBServer.Logic
{
	// Token: 0x020001CE RID: 462
	public class LineItem
	{
		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060009AE RID: 2478 RVA: 0x0005D360 File Offset: 0x0005B560
		// (set) Token: 0x060009AF RID: 2479 RVA: 0x0005D3A8 File Offset: 0x0005B5A8
		public int LineID
		{
			get
			{
				int lineID;
				lock (this)
				{
					lineID = this._LineID;
				}
				return lineID;
			}
			set
			{
				lock (this)
				{
					this._LineID = value;
				}
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060009B0 RID: 2480 RVA: 0x0005D3F0 File Offset: 0x0005B5F0
		// (set) Token: 0x060009B1 RID: 2481 RVA: 0x0005D438 File Offset: 0x0005B638
		public string GameServerIP
		{
			get
			{
				string gameServerIP;
				lock (this)
				{
					gameServerIP = this._GameServerIP;
				}
				return gameServerIP;
			}
			set
			{
				lock (this)
				{
					this._GameServerIP = value;
				}
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060009B2 RID: 2482 RVA: 0x0005D480 File Offset: 0x0005B680
		// (set) Token: 0x060009B3 RID: 2483 RVA: 0x0005D4C8 File Offset: 0x0005B6C8
		public int GameServerPort
		{
			get
			{
				int gameServerPort;
				lock (this)
				{
					gameServerPort = this._GameServerPort;
				}
				return gameServerPort;
			}
			set
			{
				lock (this)
				{
					this._GameServerPort = value;
				}
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060009B4 RID: 2484 RVA: 0x0005D510 File Offset: 0x0005B710
		// (set) Token: 0x060009B5 RID: 2485 RVA: 0x0005D558 File Offset: 0x0005B758
		public int OnlineCount
		{
			get
			{
				int onlineCount;
				lock (this)
				{
					onlineCount = this._OnlineCount;
				}
				return onlineCount;
			}
			set
			{
				lock (this)
				{
					this._OnlineCount = value;
				}
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060009B6 RID: 2486 RVA: 0x0005D5A0 File Offset: 0x0005B7A0
		// (set) Token: 0x060009B7 RID: 2487 RVA: 0x0005D5E8 File Offset: 0x0005B7E8
		public string MapOnlineNum
		{
			get
			{
				string mapOnlineNum;
				lock (this)
				{
					mapOnlineNum = this._MapOnlineNum;
				}
				return mapOnlineNum;
			}
			set
			{
				lock (this)
				{
					this._MapOnlineNum = value;
				}
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060009B8 RID: 2488 RVA: 0x0005D630 File Offset: 0x0005B830
		// (set) Token: 0x060009B9 RID: 2489 RVA: 0x0005D678 File Offset: 0x0005B878
		public long OnlineTicks
		{
			get
			{
				long onlineTicks;
				lock (this)
				{
					onlineTicks = this._OnlineTicks;
				}
				return onlineTicks;
			}
			set
			{
				lock (this)
				{
					this._OnlineTicks = value;
				}
			}
		}

		// Token: 0x04000BE5 RID: 3045
		private int _LineID = 0;

		// Token: 0x04000BE6 RID: 3046
		private string _GameServerIP = "";

		// Token: 0x04000BE7 RID: 3047
		private int _GameServerPort = 0;

		// Token: 0x04000BE8 RID: 3048
		private int _OnlineCount = 0;

		// Token: 0x04000BE9 RID: 3049
		private string _MapOnlineNum = "";

		// Token: 0x04000BEA RID: 3050
		private long _OnlineTicks = 0L;

		// Token: 0x04000BEB RID: 3051
		public GameServerClient ServerClient;
	}
}
