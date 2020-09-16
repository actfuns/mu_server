using System;

namespace HSGameEngine.Tools.AStar
{
	
	
	public delegate void PathFinderDebugHandler(int fromX, int fromY, int x, int y, PathFinderNodeType type, int totalCost, int cost);
}
