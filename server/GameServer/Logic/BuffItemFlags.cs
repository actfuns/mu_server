using System;

namespace GameServer.Logic
{
	
	public struct BuffItemFlags
	{
		
		public const int UpdateByTime = 1;

		
		public const int UpdateByVip = 2;

		
		public const int UpdateByMapCode = 4;

		
		public const int NotifyUpdateProps = 268435456;

		
		public const int CustomMessage = 1073741824;

		
		public bool isUpdateByTime;

		
		public bool isUpdateByVip;

		
		public bool isUpdateByMapCode;
	}
}
