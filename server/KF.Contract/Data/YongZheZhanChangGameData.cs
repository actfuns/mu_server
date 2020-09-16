using System;
using KF.Contract.Interface;

namespace KF.Contract.Data
{
	
	[Serializable]
	public class YongZheZhanChangGameData : IGameData, ICloneable
	{
		
		public object Clone()
		{
			return new YongZheZhanChangGameData
			{
				ZhanDouLi = this.ZhanDouLi
			};
		}

		
		public int ZhanDouLi;

		
		public string RoleName;
	}
}
