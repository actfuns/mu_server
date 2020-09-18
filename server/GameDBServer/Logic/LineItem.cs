using System;
using GameDBServer.Server;

namespace GameDBServer.Logic
{
	
	public class LineItem
	{
		
		
		
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

		
		private int _LineID = 0;

		
		private string _GameServerIP = "";

		
		private int _GameServerPort = 0;

		
		private int _OnlineCount = 0;

		
		private string _MapOnlineNum = "";

		
		private long _OnlineTicks = 0L;

		
		public GameServerClient ServerClient;
	}
}
