using System;

namespace GameServer.Logic
{
	
	internal class WaitingConfig
	{
		
		
		
		public int SeverID
		{
			get
			{
				return this._SeverID;
			}
			set
			{
				this._SeverID = value;
			}
		}

		
		
		
		public int NormalNeedWaitNumber
		{
			get
			{
				return this._NormalNeedWaitNumber;
			}
			set
			{
				this._NormalNeedWaitNumber = value;
			}
		}

		
		
		
		public int VIPNeedWaitNumber
		{
			get
			{
				return this._VIPNeedWaitNumber;
			}
			set
			{
				this._VIPNeedWaitNumber = value;
			}
		}

		
		
		
		public int NormalMaxNumber
		{
			get
			{
				return this._NormalMaxNumber;
			}
			set
			{
				this._NormalMaxNumber = value;
			}
		}

		
		
		
		public int VIPMaxNumber
		{
			get
			{
				return this._VIPMaxNumber;
			}
			set
			{
				this._VIPMaxNumber = value;
			}
		}

		
		
		
		public int NormalWaitingMaxNumber
		{
			get
			{
				return this._NormalWaitingMaxNumber;
			}
			set
			{
				this._NormalWaitingMaxNumber = value;
			}
		}

		
		
		
		public int VIPWaitingMaxNumber
		{
			get
			{
				return this._VIPWaitingMaxNumber;
			}
			set
			{
				this._VIPWaitingMaxNumber = value;
			}
		}

		
		
		
		public int NormalEnterMinInt
		{
			get
			{
				return this._NormalEnterMinInt;
			}
			set
			{
				this._NormalEnterMinInt = value;
			}
		}

		
		
		
		public int VIPEnterMinInt
		{
			get
			{
				return this._VIPEnterMinInt;
			}
			set
			{
				this._VIPEnterMinInt = value;
			}
		}

		
		
		
		public int NormalAllowMSecs
		{
			get
			{
				return this._NormalAllowMSecs;
			}
			set
			{
				this._NormalAllowMSecs = value;
			}
		}

		
		
		
		public int VIPAllowMSecs
		{
			get
			{
				return this._VIPAllowMSecs;
			}
			set
			{
				this._VIPAllowMSecs = value;
			}
		}

		
		
		
		public int NormalLogoutAllowMSecs
		{
			get
			{
				return this._NormalLogoutAllowMSecs;
			}
			set
			{
				this._NormalLogoutAllowMSecs = value;
			}
		}

		
		
		
		public int VIPLogoutAllowMSecs
		{
			get
			{
				return this._VIPLogoutAllowMSecs;
			}
			set
			{
				this._VIPLogoutAllowMSecs = value;
			}
		}

		
		
		
		public int VipExp
		{
			get
			{
				return this._VipExp;
			}
			set
			{
				this._VipExp = value;
			}
		}

		
		
		public string UserWaitConfig
		{
			get
			{
				return string.Format("{0},{1},{2},{3},{4},{5}", new object[]
				{
					this.NormalNeedWaitNumber,
					this.NormalMaxNumber,
					this.NormalWaitingMaxNumber,
					this.NormalEnterMinInt,
					this.NormalAllowMSecs,
					this.NormalLogoutAllowMSecs
				});
			}
		}

		
		
		public string VIPWaitConfig
		{
			get
			{
				return string.Format("{0},{1},{2},{3},{4},{5}", new object[]
				{
					this.VIPNeedWaitNumber,
					this.VIPMaxNumber,
					this.VIPWaitingMaxNumber,
					this.VIPEnterMinInt,
					this.VIPAllowMSecs,
					this.VIPLogoutAllowMSecs
				});
			}
		}

		
		
		public int LoginAllow_VIPExp
		{
			get
			{
				return this.VipExp;
			}
		}

		
		private int _SeverID = 0;

		
		private int _NormalNeedWaitNumber = 0;

		
		private int _VIPNeedWaitNumber = 0;

		
		private int _NormalMaxNumber = 0;

		
		private int _VIPMaxNumber = 0;

		
		private int _NormalWaitingMaxNumber = 0;

		
		private int _VIPWaitingMaxNumber = 0;

		
		private int _NormalEnterMinInt = 0;

		
		private int _VIPEnterMinInt = 0;

		
		private int _NormalAllowMSecs = 0;

		
		private int _VIPAllowMSecs = 0;

		
		private int _NormalLogoutAllowMSecs = 0;

		
		private int _VIPLogoutAllowMSecs = 0;

		
		private int _VipExp = 0;
	}
}
