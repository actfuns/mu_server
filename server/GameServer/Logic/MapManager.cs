using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x02000753 RID: 1875
	public class MapManager
	{
		// Token: 0x17000346 RID: 838
		// (get) Token: 0x06002F30 RID: 12080 RVA: 0x002A4AF4 File Offset: 0x002A2CF4
		public Dictionary<int, GameMap> DictMaps
		{
			get
			{
				return this._DictMaps;
			}
		}

		// Token: 0x06002F31 RID: 12081 RVA: 0x002A4B0C File Offset: 0x002A2D0C
		public GameMap InitAddMap(int mapCode, int mapPicCode, int mapWidth, int mapHeight, int birthPosX, int birthPosY, int birthRadius)
		{
			GameMap gameMap = new GameMap
			{
				MapCode = mapCode,
				MapPicCode = mapPicCode,
				MapWidth = mapWidth,
				MapHeight = mapHeight,
				DefaultBirthPosX = birthPosX,
				DefaultBirthPosY = birthPosY,
				BirthRadius = birthRadius
			};
			gameMap.InitMap();
			lock (this._DictMaps)
			{
				this._DictMaps.Add(mapCode, gameMap);
			}
			return gameMap;
		}

		// Token: 0x06002F32 RID: 12082 RVA: 0x002A4BB4 File Offset: 0x002A2DB4
		public GameMap GetGameMap(int mapCode)
		{
			GameMap gameMap;
			GameMap result;
			if (this._DictMaps.TryGetValue(mapCode, out gameMap) && gameMap != null)
			{
				result = gameMap;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x04003CC8 RID: 15560
		private Dictionary<int, GameMap> _DictMaps = new Dictionary<int, GameMap>(10);
	}
}
