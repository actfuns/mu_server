using System;

namespace GameServer.Logic.KuaFuIPStatistics
{
	
	public class UserOperaData
	{
		
		public string UserID;

		
		public long IPAsInt;

		
		public int[] OperaTime = new int[4];

		
		public int[] OperaCount = new int[4];

		
		public int[] AllCount = new int[6];

		
		public long createTicks = 0L;
	}
}
