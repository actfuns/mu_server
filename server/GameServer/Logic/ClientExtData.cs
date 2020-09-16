using System;

namespace GameServer.Logic
{
	
	public class ClientExtData
	{
		
		public int MapCode;

		
		public int FromX;

		
		public int FromY;

		
		public int ToX;

		
		public int ToY;

		
		public int MaxDistance2;

		
		public long ClientCmdTicks;

		
		public double MoveSpeed;

		
		public bool RunStoryboard;

		
		public long CanMoveTicks;

		
		public long StartMoveTicks;

		
		public long EndMoveTicks;

		
		public long ReservedTicks;

		
		public long StopMoveTicks;

		
		public long ServerClientTimeDiffTicks;

		
		public long CalcNum;

		
		public long MinTimeDiff = 2147483647L;

		
		public bool KeepAlive = true;

		
		public long ServerLoginTickCount;

		
		public long ClientLoginClientTicks;
	}
}
