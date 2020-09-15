using System;
using System.Text;

namespace GameServer.Logic
{
	// Token: 0x0200070E RID: 1806
	public class HeFuFanLiActivity : KingActivity
	{
		// Token: 0x06002B51 RID: 11089 RVA: 0x00267D5C File Offset: 0x00265F5C
		public override bool GiveAward(GameClient client, int _params)
		{
			return base.GiveAward(client, new AwardItem
			{
				AwardYuanBao = _params
			});
		}

		// Token: 0x06002B52 RID: 11090 RVA: 0x00267D84 File Offset: 0x00265F84
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
