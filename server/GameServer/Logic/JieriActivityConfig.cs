using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class JieriActivityConfig
	{
		
		public bool InList(int type)
		{
			return this.ConfigDict.ContainsKey(type);
		}

		
		public string GetFileName(int type)
		{
			string result;
			if (this.ConfigDict.ContainsKey(type))
			{
				result = this.ConfigDict[type];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public string GetActivityName(int type)
		{
			string result;
			if (this.ConfigDict.ContainsKey(type))
			{
				result = this.ActivityNameDict[type];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public Dictionary<int, string> ConfigDict = new Dictionary<int, string>();

		
		public Dictionary<int, string> ActivityNameDict = new Dictionary<int, string>();

		
		public List<int> openList = new List<int>();
	}
}
