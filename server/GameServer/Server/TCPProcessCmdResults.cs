using System;

namespace GameServer.Server
{
	
	public enum TCPProcessCmdResults
	{
		
		RESULT_OK,
		
		RESULT_FAILED,
		
		RESULT_DATA,
		
		RESULT_UNREGISTERED,
		
		RESUTL_CONTINUE,
		
		RESULT_DATA_CLOSE
	}
}
