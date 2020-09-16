using System;

namespace HSGameEngine.Tools.AStar
{
	
	public enum HeuristicFormula
	{
		
		Manhattan = 1,
		
		MaxDXDY,
		
		DiagonalShortCut,
		
		Euclidean,
		
		EuclideanNoSQR,
		
		Custom1
	}
}
