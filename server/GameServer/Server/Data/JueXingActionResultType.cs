using System;

namespace Server.Data
{
	
	public static class JueXingActionResultType
	{
		
		public const int Success = 0;

		
		public const int HaveActive = -1;

		
		public const int ErrConfig = -2;

		
		public const int NeedPart = -3;

		
		public const int ErrDB = -4;

		
		public const int ErrParent = -5;

		
		public const int ErrSuitType = -6;

		
		public const int ErrNotActivite = -7;

		
		public const int ErrLevelLimit = -8;

		
		public const int ErrChenLimit = -9;

		
		public const int ErrGoodsLimit = -10;
	}
}
