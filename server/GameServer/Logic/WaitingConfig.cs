using System;

namespace GameServer.Logic
{
	// Token: 0x020003B2 RID: 946
	internal class WaitingConfig
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06001043 RID: 4163 RVA: 0x000FF00C File Offset: 0x000FD20C
		// (set) Token: 0x06001044 RID: 4164 RVA: 0x000FF024 File Offset: 0x000FD224
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

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06001045 RID: 4165 RVA: 0x000FF030 File Offset: 0x000FD230
		// (set) Token: 0x06001046 RID: 4166 RVA: 0x000FF048 File Offset: 0x000FD248
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

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06001047 RID: 4167 RVA: 0x000FF054 File Offset: 0x000FD254
		// (set) Token: 0x06001048 RID: 4168 RVA: 0x000FF06C File Offset: 0x000FD26C
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

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06001049 RID: 4169 RVA: 0x000FF078 File Offset: 0x000FD278
		// (set) Token: 0x0600104A RID: 4170 RVA: 0x000FF090 File Offset: 0x000FD290
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

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600104B RID: 4171 RVA: 0x000FF09C File Offset: 0x000FD29C
		// (set) Token: 0x0600104C RID: 4172 RVA: 0x000FF0B4 File Offset: 0x000FD2B4
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

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600104D RID: 4173 RVA: 0x000FF0C0 File Offset: 0x000FD2C0
		// (set) Token: 0x0600104E RID: 4174 RVA: 0x000FF0D8 File Offset: 0x000FD2D8
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

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600104F RID: 4175 RVA: 0x000FF0E4 File Offset: 0x000FD2E4
		// (set) Token: 0x06001050 RID: 4176 RVA: 0x000FF0FC File Offset: 0x000FD2FC
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

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06001051 RID: 4177 RVA: 0x000FF108 File Offset: 0x000FD308
		// (set) Token: 0x06001052 RID: 4178 RVA: 0x000FF120 File Offset: 0x000FD320
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

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06001053 RID: 4179 RVA: 0x000FF12C File Offset: 0x000FD32C
		// (set) Token: 0x06001054 RID: 4180 RVA: 0x000FF144 File Offset: 0x000FD344
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

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06001055 RID: 4181 RVA: 0x000FF150 File Offset: 0x000FD350
		// (set) Token: 0x06001056 RID: 4182 RVA: 0x000FF168 File Offset: 0x000FD368
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

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06001057 RID: 4183 RVA: 0x000FF174 File Offset: 0x000FD374
		// (set) Token: 0x06001058 RID: 4184 RVA: 0x000FF18C File Offset: 0x000FD38C
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

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06001059 RID: 4185 RVA: 0x000FF198 File Offset: 0x000FD398
		// (set) Token: 0x0600105A RID: 4186 RVA: 0x000FF1B0 File Offset: 0x000FD3B0
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

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600105B RID: 4187 RVA: 0x000FF1BC File Offset: 0x000FD3BC
		// (set) Token: 0x0600105C RID: 4188 RVA: 0x000FF1D4 File Offset: 0x000FD3D4
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

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600105D RID: 4189 RVA: 0x000FF1E0 File Offset: 0x000FD3E0
		// (set) Token: 0x0600105E RID: 4190 RVA: 0x000FF1F8 File Offset: 0x000FD3F8
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

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600105F RID: 4191 RVA: 0x000FF204 File Offset: 0x000FD404
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

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06001060 RID: 4192 RVA: 0x000FF27C File Offset: 0x000FD47C
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

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06001061 RID: 4193 RVA: 0x000FF2F4 File Offset: 0x000FD4F4
		public int LoginAllow_VIPExp
		{
			get
			{
				return this.VipExp;
			}
		}

		// Token: 0x040018E6 RID: 6374
		private int _SeverID = 0;

		// Token: 0x040018E7 RID: 6375
		private int _NormalNeedWaitNumber = 0;

		// Token: 0x040018E8 RID: 6376
		private int _VIPNeedWaitNumber = 0;

		// Token: 0x040018E9 RID: 6377
		private int _NormalMaxNumber = 0;

		// Token: 0x040018EA RID: 6378
		private int _VIPMaxNumber = 0;

		// Token: 0x040018EB RID: 6379
		private int _NormalWaitingMaxNumber = 0;

		// Token: 0x040018EC RID: 6380
		private int _VIPWaitingMaxNumber = 0;

		// Token: 0x040018ED RID: 6381
		private int _NormalEnterMinInt = 0;

		// Token: 0x040018EE RID: 6382
		private int _VIPEnterMinInt = 0;

		// Token: 0x040018EF RID: 6383
		private int _NormalAllowMSecs = 0;

		// Token: 0x040018F0 RID: 6384
		private int _VIPAllowMSecs = 0;

		// Token: 0x040018F1 RID: 6385
		private int _NormalLogoutAllowMSecs = 0;

		// Token: 0x040018F2 RID: 6386
		private int _VIPLogoutAllowMSecs = 0;

		// Token: 0x040018F3 RID: 6387
		private int _VipExp = 0;
	}
}
