using System;
using KF.Contract.Interface;

namespace KF.Contract.Data.HuanYingSiYuan
{
	
	[Serializable]
	public class HuanYingSiYuanGameData : IGameData, ICloneable
	{
		
		public object Clone()
		{
			return new HuanYingSiYuanGameData
			{
				ZhanDouLi = this.ZhanDouLi
			};
		}

		
		public static int GetZhanDouLi(IGameData gameData)
		{
			int zhanDouLi = 0;
			if (null != gameData)
			{
				HuanYingSiYuanGameData huanYingSiYuanGameData = gameData as HuanYingSiYuanGameData;
				if (null != huanYingSiYuanGameData)
				{
					zhanDouLi = huanYingSiYuanGameData.ZhanDouLi;
				}
			}
			return zhanDouLi;
		}

		
		public int ZhanDouLi;
	}
}
