using System;
using System.Collections.Generic;

namespace HSGameEngine.Tools.AStar
{
	// Token: 0x020008DA RID: 2266
	internal interface IPathFinder
	{
		// Token: 0x1400000D RID: 13
		// (add) Token: 0x0600415B RID: 16731
		// (remove) Token: 0x0600415C RID: 16732
		event PathFinderDebugHandler PathFinderDebug;

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x0600415D RID: 16733
		bool Stopped { get; }

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x0600415E RID: 16734
		// (set) Token: 0x0600415F RID: 16735
		HeuristicFormula Formula { get; set; }

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x06004160 RID: 16736
		// (set) Token: 0x06004161 RID: 16737
		bool Diagonals { get; set; }

		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x06004162 RID: 16738
		// (set) Token: 0x06004163 RID: 16739
		bool HeavyDiagonals { get; set; }

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x06004164 RID: 16740
		// (set) Token: 0x06004165 RID: 16741
		int HeuristicEstimate { get; set; }

		// Token: 0x1700063C RID: 1596
		// (get) Token: 0x06004166 RID: 16742
		// (set) Token: 0x06004167 RID: 16743
		bool PunishChangeDirection { get; set; }

		// Token: 0x1700063D RID: 1597
		// (get) Token: 0x06004168 RID: 16744
		// (set) Token: 0x06004169 RID: 16745
		bool ReopenCloseNodes { get; set; }

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x0600416A RID: 16746
		// (set) Token: 0x0600416B RID: 16747
		bool TieBreaker { get; set; }

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x0600416C RID: 16748
		// (set) Token: 0x0600416D RID: 16749
		int SearchLimit { get; set; }

		// Token: 0x17000640 RID: 1600
		// (get) Token: 0x0600416E RID: 16750
		// (set) Token: 0x0600416F RID: 16751
		double CompletedTime { get; set; }

		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x06004170 RID: 16752
		// (set) Token: 0x06004171 RID: 16753
		bool DebugProgress { get; set; }

		// Token: 0x17000642 RID: 1602
		// (get) Token: 0x06004172 RID: 16754
		// (set) Token: 0x06004173 RID: 16755
		bool DebugFoundPath { get; set; }

		// Token: 0x06004174 RID: 16756
		void FindPathStop();

		// Token: 0x06004175 RID: 16757
		List<PathFinderNode> FindPath(Point2D start, Point2D end);
	}
}
