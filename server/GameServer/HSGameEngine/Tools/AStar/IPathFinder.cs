using System;
using System.Collections.Generic;

namespace HSGameEngine.Tools.AStar
{
	
	internal interface IPathFinder
	{
		
		// (add) Token: 0x0600415B RID: 16731
		// (remove) Token: 0x0600415C RID: 16732
		event PathFinderDebugHandler PathFinderDebug;

		
		
		bool Stopped { get; }

		
		
		
		HeuristicFormula Formula { get; set; }

		
		
		
		bool Diagonals { get; set; }

		
		
		
		bool HeavyDiagonals { get; set; }

		
		
		
		int HeuristicEstimate { get; set; }

		
		
		
		bool PunishChangeDirection { get; set; }

		
		
		
		bool ReopenCloseNodes { get; set; }

		
		
		
		bool TieBreaker { get; set; }

		
		
		
		int SearchLimit { get; set; }

		
		
		
		double CompletedTime { get; set; }

		
		
		
		bool DebugProgress { get; set; }

		
		
		
		bool DebugFoundPath { get; set; }

		
		void FindPathStop();

		
		List<PathFinderNode> FindPath(Point2D start, Point2D end);
	}
}
