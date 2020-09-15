using System;
using Server.Data;

namespace GameServer.Logic.Goods
{
	// Token: 0x0200046C RID: 1132
	internal class CondJudger_NoBuff : ICondJudger
	{
		// Token: 0x060014BD RID: 5309 RVA: 0x00145518 File Offset: 0x00143718
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = true;
			int bufferId = 0;
			long bufferValue = 0L;
			string[] args = arg.Split(new char[]
			{
				'|'
			});
			bool result;
			if (args.Length != 3 || !int.TryParse(args[0], out bufferId) || !long.TryParse(args[1], out bufferValue) || string.IsNullOrEmpty(args[2]))
			{
				result = true;
			}
			else
			{
				string msg = args[2];
				BufferData bufferData = Global.GetBufferDataByID(client, bufferId);
				if (bufferData != null && !Global.IsBufferDataOver(bufferData, 0L))
				{
					if (bufferData.BufferVal == bufferValue)
					{
						bOK = false;
					}
				}
				if (!bOK && !string.IsNullOrEmpty(msg))
				{
					failedMsg = string.Format(msg, new object[0]);
				}
				result = bOK;
			}
			return result;
		}
	}
}
