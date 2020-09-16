using System;

namespace Server.Tools
{
	
	public enum LogTypes
	{
		
		Ignore = -1,
		
		Info,
		
		Warning,
		
		Error,
		
		SQL,
		
		Exception,
		
		Trace,
		
		DataCheck = 80,
		
		TotalUserMoney = 100,
		
		Fatal = 1000
	}
}
