using System;

namespace GameServer.Logic
{
	// Token: 0x0200077A RID: 1914
	public class ProcessAreaScripts
	{
		// Token: 0x06003118 RID: 12568 RVA: 0x002B796C File Offset: 0x002B5B6C
		public static void ProcessScripts(GameClient client, string LuaScriptFileName, string functionName, int areaLuaID)
		{
			LuaScriptFileName = LuaScriptFileName.ToLower();
			functionName = functionName.ToLower();
			if ("anquanqu.lua" == LuaScriptFileName)
			{
				if ("enterarea" == functionName)
				{
					GameManager.LuaMgr.BroadcastMapRegionEvent(client, areaLuaID, 0, 1);
				}
				else if ("leavearea" == functionName)
				{
					GameManager.LuaMgr.BroadcastMapRegionEvent(client, areaLuaID, 0, 0);
				}
			}
			else if ("jinqu.lua" == LuaScriptFileName)
			{
				if ("enterarea" == functionName)
				{
					GameManager.LuaMgr.BroadcastMapRegionEvent(client, areaLuaID, 2, 1);
				}
				else if ("leavearea" == functionName)
				{
					GameManager.LuaMgr.BroadcastMapRegionEvent(client, areaLuaID, 2, 0);
				}
			}
			else if ("jiaofu.lua" == LuaScriptFileName)
			{
				if ("enterarea" == functionName)
				{
					GameManager.LuaMgr.BroadcastMapRegionEvent(client, areaLuaID, 1, 1);
				}
				else if ("leavearea" == functionName)
				{
					GameManager.LuaMgr.BroadcastMapRegionEvent(client, areaLuaID, 1, 0);
				}
			}
			else if ("rmtempcunmin.lua" == LuaScriptFileName)
			{
				if (!("enterarea" == functionName))
				{
					if ("leavearea" == functionName)
					{
						GameManager.LuaMgr.RemoveNPCForClient(client, 17);
					}
				}
			}
			else if ("caijiyaocao.lua" == LuaScriptFileName)
			{
				if ("enterarea" == functionName)
				{
					GameManager.LuaMgr.Error(client, "风云突变，天降大雨", 0);
					GameManager.LuaMgr.SendGameEffect(client, "xiayu1.swf", 0, 1, "xiayu2.mp3");
				}
				else if ("leavearea" == functionName)
				{
					GameManager.LuaMgr.SendGameEffect(client, "", 0, 0, "");
				}
			}
			else if ("gouhuo.lua" == LuaScriptFileName)
			{
				if ("enterarea" == functionName)
				{
					GameManager.LuaMgr.NotifySelfDeco(client, 60000, 1, -1, 6112, 2520, 0, -1, -1, 0, 0);
				}
				else if ("leavearea" == functionName)
				{
					GameManager.LuaMgr.NotifySelfDeco(client, 60000, -1, -1, 0, 0, 0, -1, -1, 0, 0);
				}
			}
			else if ("task.lua" == LuaScriptFileName)
			{
				if ("enterarea" == functionName)
				{
					GameManager.LuaMgr.NotifySelfDeco(client, 60000, 1, -1, 6112, 2520, 0, -1, -1, 0, 0);
				}
			}
		}
	}
}
