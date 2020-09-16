using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class MapManager
	{
		
		
		public Dictionary<int, GameMap> DictMaps
		{
			get
			{
				return this._DictMaps;
			}
		}

		
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

		
		private Dictionary<int, GameMap> _DictMaps = new Dictionary<int, GameMap>(10);
	}
}
