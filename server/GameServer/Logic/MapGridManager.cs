using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Logic
{
	// Token: 0x02000752 RID: 1874
	public class MapGridManager
	{
		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06002F2B RID: 12075 RVA: 0x002A4944 File Offset: 0x002A2B44
		public Dictionary<int, MapGrid> DictGrids
		{
			get
			{
				return this._DictGrids;
			}
		}

		// Token: 0x06002F2C RID: 12076 RVA: 0x002A495C File Offset: 0x002A2B5C
		public void InitAddMapGrid(int mapCode, int mapWidth, int mapHeight, int gridWidth, int gridHeight, GameMap gameMap)
		{
			MapGrid mapGrid = new MapGrid(mapCode, mapWidth, mapHeight, gridWidth, gridHeight, gameMap);
			lock (this._DictGrids)
			{
				this._DictGrids.Add(mapCode, mapGrid);
			}
		}

		// Token: 0x06002F2D RID: 12077 RVA: 0x002A49C0 File Offset: 0x002A2BC0
		public MapGrid GetMapGrid(int mapCode)
		{
			MapGrid result;
			lock (this._DictGrids)
			{
				MapGrid mapGrid;
				if (this._DictGrids.TryGetValue(mapCode, out mapGrid))
				{
					result = mapGrid;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06002F2E RID: 12078 RVA: 0x002A4A28 File Offset: 0x002A2C28
		public string GetAllMapClientCountForConsole()
		{
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<int, MapGrid> kv in this._DictGrids)
			{
				if (null != kv.Value)
				{
					int count = kv.Value.GetGridClientCountForConsole();
					if (count > 0)
					{
						sb.AppendFormat("{0}:{1}\n", kv.Key, count);
					}
				}
			}
			return sb.ToString();
		}

		// Token: 0x04003CC7 RID: 15559
		private Dictionary<int, MapGrid> _DictGrids = new Dictionary<int, MapGrid>(100);
	}
}
