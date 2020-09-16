using System;
using Server.Tools;

namespace Server.Protocol
{
	
	public class CmdPacket
	{
		
		public CmdPacket(int nID, byte[] data, int count)
		{
			this.CmdID = nID;
			this.Data = new byte[count];
			DataHelper.CopyBytes(this.Data, 0, data, 0, count);
		}

		
		public int CmdID;

		
		public byte[] Data;
	}
}
