using System;
using GameServer.Core.Executor;
using Server.Tools.Pattern;

namespace GameServer.Logic.Copy
{
	
	public class UniqueTeamId : SingletonTemplate<UniqueTeamId>
	{
		
		private UniqueTeamId()
		{
		}

		
		public void Init()
		{
			this.CurrSecond = (int)Global.GetOffsetSecond(TimeUtil.NowDateTime());
			this.ThisServerId = (long)GameCoreInterface.getinstance().GetLocalServerId();
		}

		
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
				this.AutoInc = (ushort)((autoInc = this.AutoInc) + 1);
				_validAutoInc = autoInc;
				_validCurSecond = (long)this.CurrSecond;
			}
			return this.ThisServerId << 48 | _validCurSecond << 16 | (long)((ulong)_validAutoInc);
		}

		
		public const long INVALID_TEAM_ID = -1L;

		
		private object Mutex = new object();

		
		private long ThisServerId;

		
		private ushort AutoInc = 0;

		
		private int CurrSecond;
	}
}
