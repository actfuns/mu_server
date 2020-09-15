using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.Goods
{
	// Token: 0x020004EB RID: 1259
	public class GoodsReplaceManager : SingletonTemplate<GoodsReplaceManager>
	{
		// Token: 0x06001770 RID: 6000 RVA: 0x0016F794 File Offset: 0x0016D994
		private GoodsReplaceManager()
		{
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x0016F7B8 File Offset: 0x0016D9B8
		public bool NeedCheckSuit(int categoriy)
		{
			return (categoriy >= 0 && categoriy <= 6) || (categoriy >= 11 && categoriy <= 21);
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x0016F840 File Offset: 0x0016DA40
		public void Init()
		{
			this.replaceJudgerDict.Clear();
			this.replaceJudgerDict["WingSuit".ToLower()] = new CondJudger_WingSuit();
			this.replaceJudgerDict["QiangHuaLevel".ToLower()] = new CondJudger_EquipForgeLvl();
			this.replaceJudgerDict["ZhuiJiaLevel".ToLower()] = new CondJudger_EquipAppendLvl();
			this.replaceJudgerDict["EquipSuit".ToLower()] = new CondJudger_EquipSuit();
			this.replaceJudgerDict["JuHun".ToLower()] = new CondJudger_JuHun();
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ReplaceGoods.xml"));
			XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ReplaceGoods.xml"));
			if (xml == null)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!!文件不存在", "Config/ReplaceGoods.xml"), null, true);
			}
			else
			{
				try
				{
					this.replaceDict.Clear();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						if (null != xmlItem)
						{
							GoodsReplaceManager.ReplaceRecord record = new GoodsReplaceManager.ReplaceRecord();
							record.seq = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
							record.condIdx = Global.GetSafeAttributeStr(xmlItem, "ToType").ToLower();
							record.condArg = Global.GetSafeAttributeStr(xmlItem, "ToTypeProperty");
							record.oldGoods = (int)Global.GetSafeAttributeLong(xmlItem, "OldGoods");
							record.newGoods = (int)Global.GetSafeAttributeLong(xmlItem, "NewGoods");
							List<GoodsReplaceManager.ReplaceRecord> recordList = null;
							if (!this.replaceDict.TryGetValue(record.oldGoods, out recordList))
							{
								recordList = new List<GoodsReplaceManager.ReplaceRecord>();
								this.replaceDict[record.oldGoods] = recordList;
							}
							recordList.Add(record);
						}
					}
					foreach (KeyValuePair<int, List<GoodsReplaceManager.ReplaceRecord>> kvp in this.replaceDict)
					{
						kvp.Value.Sort(delegate(GoodsReplaceManager.ReplaceRecord left, GoodsReplaceManager.ReplaceRecord right)
						{
							int result;
							if (left.seq > right.seq)
							{
								result = 1;
							}
							else if (left.seq == right.seq)
							{
								result = 0;
							}
							else
							{
								result = -1;
							}
							return result;
						});
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("加载{0}时出错!!! {1}", "Config/ReplaceGoods.xml", ex.Message), null, true);
				}
			}
		}

		// Token: 0x06001773 RID: 6003 RVA: 0x0016FB14 File Offset: 0x0016DD14
		public GoodsReplaceResult GetReplaceResult(GameClient client, int OriginGoods)
		{
			GoodsReplaceResult result2;
			if (client == null)
			{
				result2 = null;
			}
			else
			{
				GoodsReplaceResult result = new GoodsReplaceResult();
				result.OriginBindGoods.IsBind = true;
				result.OriginBindGoods.GoodsID = OriginGoods;
				result.OriginBindGoods.GoodsCnt = Global.GetTotalBindGoodsCountByID(client, OriginGoods);
				result.OriginUnBindGoods.IsBind = false;
				result.OriginUnBindGoods.GoodsID = OriginGoods;
				result.OriginUnBindGoods.GoodsCnt = Global.GetTotalNotBindGoodsCountByID(client, OriginGoods);
				List<GoodsReplaceManager.ReplaceRecord> records = null;
				if (this.replaceDict.TryGetValue(OriginGoods, out records))
				{
					foreach (GoodsReplaceManager.ReplaceRecord record in records)
					{
						ICondJudger judger = null;
						if (this.replaceJudgerDict.TryGetValue(record.condIdx, out judger))
						{
							string strPlaceHolder = string.Empty;
							if (judger.Judge(client, record.condArg, out strPlaceHolder))
							{
								int replaceGoodsID = record.newGoods;
								int bindCnt = Global.GetTotalBindGoodsCountByID(client, replaceGoodsID);
								int unBindCnt = Global.GetTotalNotBindGoodsCountByID(client, replaceGoodsID);
								if (bindCnt > 0)
								{
									GoodsReplaceResult.ReplaceItem item = new GoodsReplaceResult.ReplaceItem();
									item.IsBind = true;
									item.GoodsID = replaceGoodsID;
									item.GoodsCnt = bindCnt;
									result.TotalBindCnt += bindCnt;
									result.BindList.Add(item);
								}
								if (unBindCnt > 0)
								{
									GoodsReplaceResult.ReplaceItem item = new GoodsReplaceResult.ReplaceItem();
									item.IsBind = false;
									item.GoodsID = replaceGoodsID;
									item.GoodsCnt = unBindCnt;
									result.TotalUnBindCnt += unBindCnt;
									result.UnBindList.Add(item);
								}
							}
						}
					}
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x04002151 RID: 8529
		private const string ReplaceCfgFile = "Config/ReplaceGoods.xml";

		// Token: 0x04002152 RID: 8530
		private Dictionary<int, List<GoodsReplaceManager.ReplaceRecord>> replaceDict = new Dictionary<int, List<GoodsReplaceManager.ReplaceRecord>>();

		// Token: 0x04002153 RID: 8531
		private Dictionary<string, ICondJudger> replaceJudgerDict = new Dictionary<string, ICondJudger>();

		// Token: 0x020004EC RID: 1260
		private class ReplaceRecord
		{
			// Token: 0x04002155 RID: 8533
			public int seq;

			// Token: 0x04002156 RID: 8534
			public string condIdx;

			// Token: 0x04002157 RID: 8535
			public string condArg;

			// Token: 0x04002158 RID: 8536
			public int oldGoods;

			// Token: 0x04002159 RID: 8537
			public int newGoods;
		}
	}
}
