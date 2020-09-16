using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Logic
{
	
	internal class RandomWeight
	{
		
		public static int GetWeightIndex(List<int> WeightList, string info = "")
		{
			List<WeightObject> dList;
			int maxWeight = RandomWeight.GetMaxWeight(WeightList, out dList);
			int randomValue = Global.GetRandomNumber(0, maxWeight);
			if (!string.IsNullOrEmpty(info))
			{
				LogManager.WriteLog(LogTypes.Info, string.Format("[ljl]{0}, 权重={1}, maxWeight={2}", info, randomValue, maxWeight), null, true);
			}
			return dList.Find((WeightObject x) => x.RegionMin <= randomValue && x.RegionMax > randomValue).Index;
		}

		
		private static int GetMaxWeight(List<int> WeightList, out List<WeightObject> dList)
		{
			int index = 0;
			int maxWeights = 0;
			dList = new List<WeightObject>();
			foreach (int weights in WeightList)
			{
				WeightObject obj = new WeightObject();
				obj.RegionMin = maxWeights;
				maxWeights += weights;
				obj.RegionMax = maxWeights;
				obj.Index = index;
				index++;
				dList.Add(obj);
			}
			return maxWeights;
		}
	}
}
