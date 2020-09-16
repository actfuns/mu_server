using System;

namespace GameServer.Logic
{
	
	internal class ChatLevelLimitConfig
	{
		
		
		
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

		
		private int _ID = 0;

		
		private int _Day = 0;

		
		private string _Limit = "";
	}
}
