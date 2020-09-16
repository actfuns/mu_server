using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class AllThingsCalcItem
	{
		
		public static void InitAllForgeLevelInfo()
		{
			lock (AllThingsCalcItem.QiangHuaFuJiaItemList)
			{
				SystemXmlItems xmlitems = new SystemXmlItems();
				xmlitems.LoadFromXMlFile("Config/QiangHuaFuJia.xml", "", "ID", 0);
				AllThingsCalcItem.QiangHuaFuJiaItemList.Clear();
				foreach (KeyValuePair<int, SystemXmlItem> kv in xmlitems.SystemXmlItemDict)
				{
					SystemXmlItem item = kv.Value;
					QiangHuaFuJiaItem qiangHuaFuJiaItem = new QiangHuaFuJiaItem();
					qiangHuaFuJiaItem.Id = item.GetIntValue("ID", -1);
					qiangHuaFuJiaItem.Level = item.GetIntValue("QiangHuaLevel", -1);
					qiangHuaFuJiaItem.Num = item.GetIntValue("Num", -1);
					qiangHuaFuJiaItem.AddAttackInjurePercent = item.GetDoubleValue("AddAttackInjurePercent");
					qiangHuaFuJiaItem.MaxLifePercent = item.GetDoubleValue("MaxLifePercent");
					AllThingsCalcItem.QiangHuaFuJiaItemList.Add(qiangHuaFuJiaItem);
				}
				AllThingsCalcItem.QiangHuaFuJiaItemList.Sort((QiangHuaFuJiaItem x, QiangHuaFuJiaItem y) => x.Id - y.Id);
				for (int i = 0; i < AllThingsCalcItem.QiangHuaFuJiaItemList.Count; i++)
				{
					AllThingsCalcItem.QiangHuaFuJiaItemList[i].Id = i + 1;
				}
			}
		}

		
		public void ChangeTotalForgeLevel(int level, bool toAdd)
		{
			lock (this.TotalForgeLevelAccDict)
			{
				int num = 0;
				foreach (QiangHuaFuJiaItem item in AllThingsCalcItem.QiangHuaFuJiaItemList)
				{
					if (item.Level <= level)
					{
						if (toAdd)
						{
							if (this.TotalForgeLevelAccDict.TryGetValue(item.Level, out num))
							{
								this.TotalForgeLevelAccDict[item.Level] = num + 1;
							}
							else
							{
								this.TotalForgeLevelAccDict[item.Level] = 1;
							}
						}
						else if (this.TotalForgeLevelAccDict.TryGetValue(item.Level, out num))
						{
							this.TotalForgeLevelAccDict[item.Level] = num - 1;
						}
					}
				}
			}
		}

		
		public int GetTotalForgeLevelValidIndex()
		{
			lock (this.TotalForgeLevelAccDict)
			{
				foreach (QiangHuaFuJiaItem item in AllThingsCalcItem.QiangHuaFuJiaItemList)
				{
					int num;
					if (this.TotalForgeLevelAccDict.TryGetValue(item.Level, out num) && item.Num <= num)
					{
						return item.Id;
					}
				}
			}
			return 0;
		}

		
		public static QiangHuaFuJiaItem GetQiangHuaFuJiaItem(int index)
		{
			if (index >= 0)
			{
				lock (AllThingsCalcItem.QiangHuaFuJiaItemList)
				{
					if (index < AllThingsCalcItem.QiangHuaFuJiaItemList.Count)
					{
						return AllThingsCalcItem.QiangHuaFuJiaItemList[index];
					}
				}
			}
			return null;
		}

		
		public int TotalPurpleQualityNum = 0;

		
		public int TotalGoldQualityNum = 0;

		
		public int TotalForge5LevelNum = 0;

		
		public int TotalForge7LevelNum = 0;

		
		public int TotalForge9LevelNum = 0;

		
		public int TotalForge11LevelNum = 0;

		
		public int TotalForge13LevelNum = 0;

		
		public int TotalForge15LevelNum = 0;

		
		public int TotalJewel4LevelNum = 0;

		
		public int TotalJewel5LevelNum = 0;

		
		public int TotalJewel6LevelNum = 0;

		
		public int TotalJewel7LevelNum = 0;

		
		public int TotalJewel8LevelNum = 0;

		
		public int TotalGreenZhuoYueNum = 0;

		
		public int TotalBlueZhuoYueNum = 0;

		
		public int TotalPurpleZhuoYueNum = 0;

		
		public Dictionary<int, int> TotalForgeLevelAccDict = new Dictionary<int, int>();

		
		public static List<QiangHuaFuJiaItem> QiangHuaFuJiaItemList = new List<QiangHuaFuJiaItem>();
	}
}
