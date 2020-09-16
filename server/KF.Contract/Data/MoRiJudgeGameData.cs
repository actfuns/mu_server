using System;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	
	[Serializable]
	public class MoRiJudgeGameData : IGameData, ICloneable
	{
		
		public object Clone()
		{
			return new TianTiGameData
			{
				ZhanDouLi = this.ZhanDouLi
			};
		}

		
		public int ZhanDouLi;
	}
}
