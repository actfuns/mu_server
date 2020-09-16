using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	
	public class GoodsCoolDownMgr
	{
		
		public bool GoodsCoolDown(int goodsID)
		{
			CoolDownItem coolDownItem = null;
			bool result;
			if (!this.GoodsCoolDownDict.TryGetValue(goodsID, out coolDownItem))
			{
				result = true;
			}
			else
			{
				long ticks = TimeUtil.NOW();
				result = (ticks > coolDownItem.StartTicks + coolDownItem.CDTicks);
			}
			return result;
		}

		
		public void AddGoodsCoolDown(GameClient client, int goodsID)
		{
			SystemXmlItem systemGoods = null;
			if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
			{
				int cdTime = systemGoods.GetIntValue("CDTime", -1);
				if (cdTime > 0)
				{
					int pubCDTime = systemGoods.GetIntValue("PubCDTime", -1);
					int cdGroup = systemGoods.GetIntValue("ShareGroupID", -1);
					long nowTicks = TimeUtil.NOW();
					Global.AddCoolDownItem(this.GoodsCoolDownDict, goodsID, nowTicks, (long)(cdTime * 1000));
					if (cdGroup > 0)
					{
						if (null != client.ClientData.GoodsDataList)
						{
							for (int i = 0; i < client.ClientData.GoodsDataList.Count; i++)
							{
								GoodsData goodsData = client.ClientData.GoodsDataList[i];
								if (null != goodsData)
								{
									if (goodsData.Using <= 0)
									{
										SystemXmlItem systemGoods2 = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods2))
										{
											if (null != systemGoods2)
											{
												if (cdGroup == systemGoods2.GetIntValue("ShareGroupID", -1))
												{
													Global.AddCoolDownItem(this.GoodsCoolDownDict, goodsData.GoodsID, nowTicks, (long)(pubCDTime * 1000));
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		private Dictionary<int, CoolDownItem> GoodsCoolDownDict = new Dictionary<int, CoolDownItem>();
	}
}
