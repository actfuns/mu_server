using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class NPCTasksManager
	{
		
		
		public Dictionary<int, List<int>> SourceNPCTasksDict
		{
			get
			{
				return this._SourceNPCTasksDict;
			}
		}

		
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

		
		
		public Dictionary<int, List<int>> DestNPCTasksDict
		{
			get
			{
				return this._DestNPCTasksDict;
			}
		}

		
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

		
		private Dictionary<int, List<int>> _SourceNPCTasksDict = null;

		
		private Dictionary<int, List<int>> _DestNPCTasksDict = null;
	}
}
