using System;
using System.Text;

namespace GameServer.Logic
{
	
	public class HeFuFanLiActivity : KingActivity
	{
		
		public override bool GiveAward(GameClient client, int _params)
		{
			return base.GiveAward(client, new AwardItem
			{
				AwardYuanBao = _params
			});
		}

		
		public override string GetAwardMinConditionValues()
		{
			StringBuilder strBuilder = new StringBuilder();
			int maxPaiHang = this.AwardDict.Count;
			for (int paiHang = 1; paiHang <= maxPaiHang; paiHang++)
			{
				if (this.AwardDict.ContainsKey(paiHang))
				{
					if (strBuilder.Length > 0)
					{
						strBuilder.Append("_");
					}
					strBuilder.Append(string.Format("{0},{1}", this.AwardDict[paiHang].MinAwardCondionValue, this.AwardDict[paiHang].MinAwardCondionValue2));
				}
			}
			return strBuilder.ToString();
		}
	}
}
