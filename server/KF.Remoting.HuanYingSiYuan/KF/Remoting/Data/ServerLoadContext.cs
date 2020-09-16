using System;
using System.Collections.Generic;
using KF.Contract.Data;

namespace KF.Remoting.Data
{
	
	public class ServerLoadContext
	{
		
		public void CalcServerLoadAvg()
		{
			if (this.AlivedServerCount > 0 && this.AlivedGameFuBenCount > 0)
			{
				this.RealServerLoadAvg = this.AlivedGameFuBenCount / this.AlivedServerCount;
			}
			else
			{
				this.RealServerLoadAvg = 0;
			}
			this.ServerLoadAvg = this.RealServerLoadAvg + 5;
		}

		
		public int AlivedServerCount;

		
		public int AlivedGameFuBenCount;

		
		public int ServerLoadAvg;

		
		public int RealServerLoadAvg;

		
		public int SignUpRoleCount;

		
		public int StartGameRoleCount;

		
		public bool AssginGameFuBenComplete;

		
		public LinkedList<KuaFuServerGameConfig> IdelActiveServerQueue = new LinkedList<KuaFuServerGameConfig>();
	}
}
