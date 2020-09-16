using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class SingleEquipAddPropMgr
	{
		
		private static List<double[]> GetCachingPropsList(SingleEquipProps singleEquipPropsMgr, int occupation, SystemXmlItem systemGoods)
		{
			int categoriy = systemGoods.GetIntValue("Categoriy", -1);
			int suitID = systemGoods.GetIntValue("SuitID", -1);
			return singleEquipPropsMgr.GetSingleEquipPropsList(occupation, categoriy, suitID);
		}

		
		private static void ApplyNewPropsToEquipProps(double[] equipProps, double[] newProps, bool toAdd)
		{
		}

		
		public static void LoadAllSingleEquipProps()
		{
			SingleEquipAddPropMgr.LoadSingleEquipPropsForge();
			SingleEquipAddPropMgr.LoadSingleEquipPropsJewels();
			SingleEquipAddPropMgr.LoadSingleEquipPropsFuJia();
		}

		
		private static void LoadSingleEquipPropsForge()
		{
			SingleEquipAddPropMgr._SingleEquipPropsForgeMgr.LoadEquipPropItems("Config/SingleEquipAddProp/QiangHua/");
		}

		
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

		
		private static void LoadSingleEquipPropsJewels()
		{
			SingleEquipAddPropMgr._SingleEquipPropsJewelsMgr.LoadEquipPropItems("Config/SingleEquipAddProp/Jewels/");
		}

		
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

		
		private static void LoadSingleEquipPropsFuJia()
		{
			SingleEquipAddPropMgr._SingleEquipPropsFuJiaMgr.LoadEquipPropItems("Config/SingleEquipAddProp/FuJia/");
		}

		
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

		
		private static SingleEquipProps _SingleEquipPropsForgeMgr = new SingleEquipProps();

		
		private static SingleEquipProps _SingleEquipPropsJewelsMgr = new SingleEquipProps();

		
		private static SingleEquipProps _SingleEquipPropsFuJiaMgr = new SingleEquipProps();
	}
}
