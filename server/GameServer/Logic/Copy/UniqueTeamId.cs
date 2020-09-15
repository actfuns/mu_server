using System;
using GameServer.Core.Executor;
using Server.Tools.Pattern;

namespace GameServer.Logic.Copy
{
	// Token: 0x02000292 RID: 658
	public class UniqueTeamId : SingletonTemplate<UniqueTeamId>
	{
		// Token: 0x060009CC RID: 2508 RVA: 0x0009C306 File Offset: 0x0009A506
		private UniqueTeamId()
		{
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x0009C323 File Offset: 0x0009A523
		public void Init()
		{
			this.CurrSecond = (int)Global.GetOffsetSecond(TimeUtil.NowDateTime());
			this.ThisServerId = (long)GameCoreInterface.getinstance().GetLocalServerId();
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x0009C348 File Offset: 0x0009A548
		public long Create()
		{
			ushort _validAutoInc;
			long _validCurSecond;
			lock (this.Mutex)
			{
				if (this.AutoInc >= 65535)
				{
					this.CurrSecond++;
					this.AutoInc = 0;
				}
				ushort autoInc;
				this.AutoInc = (autoInc = this.AutoInc) + 1;
				_validAutoInc = autoInc;
				_validCurSecond = (long)this.CurrSecond;
			}
			return this.ThisServerId << 48 | _validCurSecond << 16 | (long)((ulong)_validAutoInc);
		}

		// Token: 0x0400104C RID: 4172
		public const long INVALID_TEAM_ID = -1L;

		// Token: 0x0400104D RID: 4173
		private object Mutex = new object();

		// Token: 0x0400104E RID: 4174
		private long ThisServerId;

		// Token: 0x0400104F RID: 4175
		private ushort AutoInc = 0;

		// Token: 0x04001050 RID: 4176
		private int CurrSecond;
	}
}
