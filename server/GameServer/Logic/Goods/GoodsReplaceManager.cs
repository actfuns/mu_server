using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.Goods
{
	
	public class GoodsReplaceManager : SingletonTemplate<GoodsReplaceManager>
	{
		
		private GoodsReplaceManager()
		{
		}

		
		public bool NeedCheckSuit(int categoriy)
		{
			return (categoriy >= 0 && categoriy <= 6) || (categoriy >= 11 && categoriy <= 21);
		}

		
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

		
		private const string ReplaceCfgFile = "Config/ReplaceGoods.xml";

		
		private Dictionary<int, List<GoodsReplaceManager.ReplaceRecord>> replaceDict = new Dictionary<int, List<GoodsReplaceManager.ReplaceRecord>>();

		
		private Dictionary<string, ICondJudger> replaceJudgerDict = new Dictionary<string, ICondJudger>();

		
		private class ReplaceRecord
		{
			
			public int seq;

			
			public string condIdx;

			
			public string condArg;

			
			public int oldGoods;

			
			public int newGoods;
		}
	}
}
