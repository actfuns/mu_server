using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Client;
using Server.Tools;
using Tmsk.Contract.Data;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001C1 RID: 449
	public class JieriPlatChargeKing : Activity
	{
		// Token: 0x1700001A RID: 26
		
		
		public List<InputKingPaiHangData> RealRankList
		{
			get
			{
				List<InputKingPaiHangData> realRankList;
				lock (this.Mutex)
				{
					realRankList = this._realRankList;
				}
				return realRankList;
			}
			private set
			{
				lock (this.Mutex)
				{
					this._realRankList = value;
				}
			}
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x0004EB20 File Offset: 0x0004CD20
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(this.CfgFile));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(this.CfgFile));
				if (null == xml)
				{
					return false;
				}
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					this.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					this.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				if (null != args)
				{
					IEnumerable<XElement> xmlItems = args.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							JieriPlatChargeKing.ChargeItem ci = new JieriPlatChargeKing.ChargeItem();
							ci.Rank = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							ci.NeedChargeYB = (int)Global.GetSafeAttributeLong(xmlItem, "MinYuanBao");
							this.chargeItemList.Add(ci);
						}
					}
					this.chargeItemList.Sort(delegate(JieriPlatChargeKing.ChargeItem left, JieriPlatChargeKing.ChargeItem right)
					{
						int result;
						if (left.Rank < right.Rank)
						{
							result = -1;
						}
						else if (left.Rank > right.Rank)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", this.CfgFile, ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x0004ED58 File Offset: 0x0004CF58
		public void Update()
		{
			if (this.InActivityTime() || this.InAwardTime())
			{
				DateTime now = TimeUtil.NowDateTime();
				if (!(now < this.lastUpdateTime.AddSeconds(15.0)))
				{
					this.lastUpdateTime = now;
					InputKingPaiHangDataEx tmpRankEx = KFCopyRpcClient.getInstance().GetPlatChargeKing();
					if (tmpRankEx != null)
					{
						List<InputKingPaiHangData> tmpRankList = tmpRankEx.ListData;
						if (tmpRankEx.StartTime != this.FromDate || tmpRankEx.EndTime != this.ToDate)
						{
						}
						if (tmpRankList != null)
						{
							bool bNeedSort = false;
							int i;
							for (i = 1; i < tmpRankList.Count<InputKingPaiHangData>(); i++)
							{
								if (tmpRankList[i].PaiHangValue > tmpRankList[i - 1].PaiHangValue)
								{
									bNeedSort = true;
									break;
								}
							}
							if (bNeedSort)
							{
								tmpRankList.Sort((InputKingPaiHangData _left, InputKingPaiHangData _right) => _right.PaiHangValue - _left.PaiHangValue);
							}
							tmpRankList.ForEach(delegate(InputKingPaiHangData _item)
							{
								_item.PaiHangValue = Global.TransMoneyToYuanBao(_item.PaiHangValue);
							});
							int procListIdx = 0;
							i = 0;
							while (i < this.chargeItemList.Count && procListIdx < tmpRankList.Count)
							{
								if (tmpRankList[procListIdx].PaiHangValue >= this.chargeItemList[i].NeedChargeYB)
								{
									tmpRankList[procListIdx].PaiHang = this.chargeItemList[i].Rank;
									procListIdx++;
								}
								i++;
							}
							if (procListIdx < tmpRankList.Count)
							{
								tmpRankList.RemoveRange(procListIdx, tmpRankList.Count - procListIdx);
							}
						}
						this.RealRankList = tmpRankList;
					}
				}
			}
			else
			{
				this.RealRankList = null;
			}
		}

		// Token: 0x040009F5 RID: 2549
		private const int updateIntervalSec = 15;

		// Token: 0x040009F6 RID: 2550
		private readonly string CfgFile = "Config/JieRiGifts/PingTaiChongZhiKing.xml";

		// Token: 0x040009F7 RID: 2551
		private List<JieriPlatChargeKing.ChargeItem> chargeItemList = new List<JieriPlatChargeKing.ChargeItem>();

		// Token: 0x040009F8 RID: 2552
		private object Mutex = new object();

		// Token: 0x040009F9 RID: 2553
		private List<InputKingPaiHangData> _realRankList = null;

		// Token: 0x040009FA RID: 2554
		private DateTime lastUpdateTime = TimeUtil.NowDateTime().AddSeconds(-30.0);

		// Token: 0x020001C2 RID: 450
		private class ChargeItem
		{
			// Token: 0x040009FE RID: 2558
			public int Rank;

			// Token: 0x040009FF RID: 2559
			public int NeedChargeYB;
		}
	}
}
