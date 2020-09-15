using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020005B7 RID: 1463
	public class AllThingsCalcItem
	{
		// Token: 0x06001A7E RID: 6782 RVA: 0x001957B8 File Offset: 0x001939B8
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

		// Token: 0x06001A7F RID: 6783 RVA: 0x00195960 File Offset: 0x00193B60
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

		// Token: 0x06001A80 RID: 6784 RVA: 0x00195A94 File Offset: 0x00193C94
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

		// Token: 0x06001A81 RID: 6785 RVA: 0x00195B5C File Offset: 0x00193D5C
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

		// Token: 0x04002917 RID: 10519
		public int TotalPurpleQualityNum = 0;

		// Token: 0x04002918 RID: 10520
		public int TotalGoldQualityNum = 0;

		// Token: 0x04002919 RID: 10521
		public int TotalForge5LevelNum = 0;

		// Token: 0x0400291A RID: 10522
		public int TotalForge7LevelNum = 0;

		// Token: 0x0400291B RID: 10523
		public int TotalForge9LevelNum = 0;

		// Token: 0x0400291C RID: 10524
		public int TotalForge11LevelNum = 0;

		// Token: 0x0400291D RID: 10525
		public int TotalForge13LevelNum = 0;

		// Token: 0x0400291E RID: 10526
		public int TotalForge15LevelNum = 0;

		// Token: 0x0400291F RID: 10527
		public int TotalJewel4LevelNum = 0;

		// Token: 0x04002920 RID: 10528
		public int TotalJewel5LevelNum = 0;

		// Token: 0x04002921 RID: 10529
		public int TotalJewel6LevelNum = 0;

		// Token: 0x04002922 RID: 10530
		public int TotalJewel7LevelNum = 0;

		// Token: 0x04002923 RID: 10531
		public int TotalJewel8LevelNum = 0;

		// Token: 0x04002924 RID: 10532
		public int TotalGreenZhuoYueNum = 0;

		// Token: 0x04002925 RID: 10533
		public int TotalBlueZhuoYueNum = 0;

		// Token: 0x04002926 RID: 10534
		public int TotalPurpleZhuoYueNum = 0;

		// Token: 0x04002927 RID: 10535
		public Dictionary<int, int> TotalForgeLevelAccDict = new Dictionary<int, int>();

		// Token: 0x04002928 RID: 10536
		public static List<QiangHuaFuJiaItem> QiangHuaFuJiaItemList = new List<QiangHuaFuJiaItem>();
	}
}
