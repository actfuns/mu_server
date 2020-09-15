using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000770 RID: 1904
	public class NPCTasksManager
	{
		// Token: 0x1700039F RID: 927
		// (get) Token: 0x060030F2 RID: 12530 RVA: 0x002B669C File Offset: 0x002B489C
		public Dictionary<int, List<int>> SourceNPCTasksDict
		{
			get
			{
				return this._SourceNPCTasksDict;
			}
		}

		// Token: 0x060030F3 RID: 12531 RVA: 0x002B66B4 File Offset: 0x002B48B4
		private void AddSourceNPCTask(int npcID, int taskID, Dictionary<int, List<int>> sourceNPCTasksDict)
		{
			List<int> taskList = null;
			if (!sourceNPCTasksDict.TryGetValue(npcID, out taskList))
			{
				taskList = new List<int>();
				sourceNPCTasksDict[npcID] = taskList;
			}
			if (-1 == taskList.IndexOf(taskID))
			{
				taskList.Add(taskID);
			}
		}

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x060030F4 RID: 12532 RVA: 0x002B6700 File Offset: 0x002B4900
		public Dictionary<int, List<int>> DestNPCTasksDict
		{
			get
			{
				return this._DestNPCTasksDict;
			}
		}

		// Token: 0x060030F5 RID: 12533 RVA: 0x002B6718 File Offset: 0x002B4918
		private void AddDestNPCTask(int npcID, int taskID, Dictionary<int, List<int>> destNPCTasksDict)
		{
			List<int> taskList = null;
			if (!destNPCTasksDict.TryGetValue(npcID, out taskList))
			{
				taskList = new List<int>();
				destNPCTasksDict[npcID] = taskList;
			}
			if (-1 == taskList.IndexOf(taskID))
			{
				taskList.Add(taskID);
			}
		}

		// Token: 0x060030F6 RID: 12534 RVA: 0x002B6764 File Offset: 0x002B4964
		public void LoadNPCTasks(SystemXmlItems systemTasks)
		{
			Dictionary<int, List<int>> sourceNPCTasksDict = new Dictionary<int, List<int>>();
			Dictionary<int, List<int>> destNPCTasksDict = new Dictionary<int, List<int>>();
			foreach (int key in systemTasks.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemTask = systemTasks.SystemXmlItemDict[key];
				this.AddSourceNPCTask(systemTask.GetIntValue("SourceNPC", -1), systemTask.GetIntValue("ID", -1), sourceNPCTasksDict);
				this.AddDestNPCTask(systemTask.GetIntValue("DestNPC", -1), systemTask.GetIntValue("ID", -1), destNPCTasksDict);
			}
			this._SourceNPCTasksDict = sourceNPCTasksDict;
			this._DestNPCTasksDict = destNPCTasksDict;
		}

		// Token: 0x04003D7C RID: 15740
		private Dictionary<int, List<int>> _SourceNPCTasksDict = null;

		// Token: 0x04003D7D RID: 15741
		private Dictionary<int, List<int>> _DestNPCTasksDict = null;
	}
}
