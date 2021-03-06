﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	
	public class ThemeDaLiBaoActivity : Activity
	{
		
		public override bool GiveAward(GameClient client, int _params)
		{
			bool result2;
			if (null == client)
			{
				result2 = false;
			}
			else
			{
				bool result = true;
				if (null != this.MyAwardItem)
				{
					result = base.GiveAward(client, this.MyAwardItem);
				}
				if (result)
				{
					int occupation = client.ClientData.Occupation;
					AwardItem myOccAward = this.GetOccAward(occupation);
					if (null != myOccAward)
					{
						result = base.GiveAward(client, myOccAward);
					}
				}
				if (client._IconStateMgr.CheckThemeDaLiBao(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				result2 = result;
			}
			return result2;
		}

		
		public AwardItem GetOccAward(int _params)
		{
			AwardItem myOccAward = null;
			if (this.OccAwardItemDict.ContainsKey(_params))
			{
				myOccAward = this.OccAwardItemDict[_params];
			}
			return myOccAward;
		}

		
		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				int occupation = client.ClientData.Occupation;
				AwardItem myOccAward = this.GetOccAward(occupation);
				result = ((this.MyAwardItem.GoodsDataList.Count <= 0 && (myOccAward == null || myOccAward.GoodsDataList.Count <= 0)) || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList));
			}
			return result;
		}

		
		public bool Init()
		{
			try
			{
				string fileName = "Config/ThemeActivityLiBao.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(fileName));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				if (null == xml)
				{
					return false;
				}
				this.ActivityType = 151;
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				this.MyAwardItem = new AwardItem();
				this.MyAwardItem.MinAwardCondionValue = 0;
				this.MyAwardItem.AwardYuanBao = 0;
				XElement xmlItem = xml.Element("ThemeActivityLiBao");
				if (null == xmlItem)
				{
					return false;
				}
				string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
				if (string.IsNullOrEmpty(goodsIDs))
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型主题服礼包活动配置文件中的物品配置项1失败", new object[0]), null, true);
				}
				else
				{
					string[] fields = goodsIDs.Split(new char[]
					{
						'|'
					});
					if (fields.Length <= 0)
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型主题服礼包活动配置文件中的物品配置项1失败", new object[0]), null, true);
					}
					else
					{
						this.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型主题服礼包配置1");
					}
				}
				goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
				if (string.IsNullOrEmpty(goodsIDs))
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("读取大型主题服礼包活动配置文件中的物品配置项2失败", new object[0]), null, true);
				}
				else
				{
					string[] fields = goodsIDs.Split(new char[]
					{
						'|'
					});
					if (fields.Length <= 0)
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("解析大型主题服礼包活动配置文件中的物品配置项2失败", new object[0]), null, true);
					}
					else
					{
						List<GoodsData> GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "大型主题服礼包配置2");
						foreach (GoodsData item in GoodsDataList)
						{
							SystemXmlItem systemGoods = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(item.GoodsID, out systemGoods))
							{
								int toOccupation = Global.GetMainOccupationByGoodsID(item.GoodsID);
								AwardItem myOccAward = this.GetOccAward(toOccupation);
								if (null == myOccAward)
								{
									myOccAward = new AwardItem();
									myOccAward.GoodsDataList.Add(item);
									this.OccAwardItemDict[toOccupation] = myOccAward;
								}
								else
								{
									myOccAward.GoodsDataList.Add(item);
								}
							}
						}
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "Config/ThemeActivityLiBao.xml解析出现异常", ex, true);
				return false;
			}
			return true;
		}

		
		public AwardItem MyAwardItem = new AwardItem();

		
		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
