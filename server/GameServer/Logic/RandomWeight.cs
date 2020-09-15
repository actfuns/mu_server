using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020000AB RID: 171
	internal class RandomWeight
	{
		// Token: 0x060002B3 RID: 691 RVA: 0x0002DD20 File Offset: 0x0002BF20
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

		// Token: 0x060002B4 RID: 692 RVA: 0x0002DD98 File Offset: 0x0002BF98
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
