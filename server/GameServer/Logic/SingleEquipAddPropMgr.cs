using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000797 RID: 1943
	public class SingleEquipAddPropMgr
	{
		// Token: 0x060032A9 RID: 12969 RVA: 0x002CF4FC File Offset: 0x002CD6FC
		private static List<double[]> GetCachingPropsList(SingleEquipProps singleEquipPropsMgr, int occupation, SystemXmlItem systemGoods)
		{
			int categoriy = systemGoods.GetIntValue("Categoriy", -1);
			int suitID = systemGoods.GetIntValue("SuitID", -1);
			return singleEquipPropsMgr.GetSingleEquipPropsList(occupation, categoriy, suitID);
		}

		// Token: 0x060032AA RID: 12970 RVA: 0x002CF533 File Offset: 0x002CD733
		private static void ApplyNewPropsToEquipProps(double[] equipProps, double[] newProps, bool toAdd)
		{
		}

		// Token: 0x060032AB RID: 12971 RVA: 0x002CF536 File Offset: 0x002CD736
		public static void LoadAllSingleEquipProps()
		{
			SingleEquipAddPropMgr.LoadSingleEquipPropsForge();
			SingleEquipAddPropMgr.LoadSingleEquipPropsJewels();
			SingleEquipAddPropMgr.LoadSingleEquipPropsFuJia();
		}

		// Token: 0x060032AC RID: 12972 RVA: 0x002CF54B File Offset: 0x002CD74B
		private static void LoadSingleEquipPropsForge()
		{
			SingleEquipAddPropMgr._SingleEquipPropsForgeMgr.LoadEquipPropItems("Config/SingleEquipAddProp/QiangHua/");
		}

		// Token: 0x060032AD RID: 12973 RVA: 0x002CF560 File Offset: 0x002CD760
		public static void ProcessSingleEquipPropsForge(double[] equipProps, int occupation, GoodsData goodsData, SystemXmlItem systemGoods, bool toAdd)
		{
			List<double[]> propsList = SingleEquipAddPropMgr.GetCachingPropsList(SingleEquipAddPropMgr._SingleEquipPropsForgeMgr, occupation, systemGoods);
			if (propsList != null && propsList.Count > 0)
			{
				int propsIndex;
				if (goodsData.Forge_level >= 10)
				{
					propsIndex = 3;
				}
				else if (goodsData.Forge_level >= 9)
				{
					propsIndex = 2;
				}
				else if (goodsData.Forge_level >= 7)
				{
					propsIndex = 1;
				}
				else
				{
					if (goodsData.Forge_level < 5)
					{
						return;
					}
					propsIndex = 0;
				}
				if (propsIndex < propsList.Count)
				{
					for (int i = 0; i <= propsIndex; i++)
					{
						double[] newProps = propsList[i];
						if (newProps == null || newProps.Length != 10)
						{
							break;
						}
						SingleEquipAddPropMgr.ApplyNewPropsToEquipProps(equipProps, newProps, toAdd);
					}
				}
			}
		}

		// Token: 0x060032AE RID: 12974 RVA: 0x002CF642 File Offset: 0x002CD842
		private static void LoadSingleEquipPropsJewels()
		{
			SingleEquipAddPropMgr._SingleEquipPropsJewelsMgr.LoadEquipPropItems("Config/SingleEquipAddProp/Jewels/");
		}

		// Token: 0x060032AF RID: 12975 RVA: 0x002CF658 File Offset: 0x002CD858
		public static void ProcessSingleEquipPropsJewels(double[] equipProps, int occupation, AllThingsCalcItem singleEquipJewels, SystemXmlItem systemGoods, bool toAdd)
		{
			List<double[]> propsList = SingleEquipAddPropMgr.GetCachingPropsList(SingleEquipAddPropMgr._SingleEquipPropsJewelsMgr, occupation, systemGoods);
			if (propsList != null && propsList.Count > 0)
			{
				int propsIndex;
				if (singleEquipJewels.TotalJewel8LevelNum >= 6)
				{
					propsIndex = 2;
				}
				else if (singleEquipJewels.TotalJewel6LevelNum + singleEquipJewels.TotalJewel7LevelNum + singleEquipJewels.TotalJewel8LevelNum >= 6)
				{
					propsIndex = 1;
				}
				else
				{
					if (singleEquipJewels.TotalJewel4LevelNum + singleEquipJewels.TotalJewel5LevelNum + singleEquipJewels.TotalJewel6LevelNum + singleEquipJewels.TotalJewel7LevelNum + singleEquipJewels.TotalJewel8LevelNum < 6)
					{
						return;
					}
					propsIndex = 0;
				}
				if (propsIndex < propsList.Count)
				{
					for (int i = 0; i <= propsIndex; i++)
					{
						double[] newProps = propsList[i];
						if (newProps == null || newProps.Length != 10)
						{
							break;
						}
						SingleEquipAddPropMgr.ApplyNewPropsToEquipProps(equipProps, newProps, toAdd);
					}
				}
			}
		}

		// Token: 0x060032B0 RID: 12976 RVA: 0x002CF74D File Offset: 0x002CD94D
		private static void LoadSingleEquipPropsFuJia()
		{
			SingleEquipAddPropMgr._SingleEquipPropsFuJiaMgr.LoadEquipPropItems("Config/SingleEquipAddProp/FuJia/");
		}

		// Token: 0x060032B1 RID: 12977 RVA: 0x002CF760 File Offset: 0x002CD960
		public static void ProcessSingleEquipPropsFuJia(double[] equipProps, int occupation, GoodsData goodsData, SystemXmlItem systemGoods, bool toAdd)
		{
			List<double[]> propsList = SingleEquipAddPropMgr.GetCachingPropsList(SingleEquipAddPropMgr._SingleEquipPropsFuJiaMgr, occupation, systemGoods);
			if (propsList != null && propsList.Count > 0)
			{
				int propsIndex;
				if (goodsData.Quality >= 4)
				{
					propsIndex = 3;
				}
				else if (goodsData.Quality >= 3)
				{
					propsIndex = 2;
				}
				else if (goodsData.Quality >= 2)
				{
					propsIndex = 1;
				}
				else
				{
					if (goodsData.Quality < 1)
					{
						return;
					}
					propsIndex = 0;
				}
				if (propsIndex < propsList.Count)
				{
					double[] newProps = propsList[propsIndex];
					if (newProps != null && newProps.Length == 10)
					{
						int addPropIndex = goodsData.AddPropIndex;
						addPropIndex = Global.GMax(addPropIndex, 0);
						addPropIndex = Global.GMin(addPropIndex, 10);
						double[] calcProps = new double[newProps.Length];
						for (int i = 0; i < newProps.Length; i++)
						{
							double origExtProp = newProps[i];
							double newProp = origExtProp * (double)(1 + addPropIndex);
							calcProps[i] = newProp;
						}
						SingleEquipAddPropMgr.ApplyNewPropsToEquipProps(equipProps, calcProps, toAdd);
					}
				}
			}
		}

		// Token: 0x04003EBA RID: 16058
		private static SingleEquipProps _SingleEquipPropsForgeMgr = new SingleEquipProps();

		// Token: 0x04003EBB RID: 16059
		private static SingleEquipProps _SingleEquipPropsJewelsMgr = new SingleEquipProps();

		// Token: 0x04003EBC RID: 16060
		private static SingleEquipProps _SingleEquipPropsFuJiaMgr = new SingleEquipProps();
	}
}
