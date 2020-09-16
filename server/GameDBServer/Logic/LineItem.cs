using System;
using GameDBServer.Server;

namespace GameDBServer.Logic
{
	// Token: 0x020001CE RID: 462
	public class LineItem
	{
		// Token: 0x170000D1 RID: 209
		
		
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
