using System;
using System.Collections.Generic;

namespace GameServer.Logic.ExtensionProps
{
	
	public class SpriteExtensionProps
	{
		
		public void AddID(int id)
		{
			lock (this.Mutex)
			{
				this.ExtensionPropIDsList.Add(id);
			}
		}

		
		public void RemoveID(int id)
		{
			lock (this.Mutex)
			{
				for (int i = 0; i < this.ExtensionPropIDsList.Count; i++)
				{
					if (this.ExtensionPropIDsList[i] == id)
					{
						this.ExtensionPropIDsList.RemoveAt(i);
						break;
					}
				}
			}
		}

		
		public List<int> GetIDs()
		{
			List<int> list = null;
			lock (this.Mutex)
			{
				list = this.ExtensionPropIDsList.GetRange(0, this.ExtensionPropIDsList.Count);
			}
			return list;
		}

		
		private object Mutex = new object();

		
		private List<int> ExtensionPropIDsList = new List<int>();
	}
}
