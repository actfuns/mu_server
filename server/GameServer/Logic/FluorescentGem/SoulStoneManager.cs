using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.FluorescentGem
{
	
	public class SoulStoneManager : SingletonTemplate<SoulStoneManager>, IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		private SoulStoneManager()
		{
			this.JingHuaCategorys.Add(910);
			this.EquipCategorys.Add(911);
			this.EquipCategorys.Add(912);
			this.EquipCategorys.Add(913);
			this.EquipCategorys.Add(914);
			this.EquipCategorys.Add(915);
			this.EquipCategorys.Add(916);
			this.EquipCategorys.Add(917);
			this.EquipCategorys.Add(918);
			this.EquipCategorys.Add(919);
			this.EquipCategorys.Add(920);
			this.EquipCategorys.Add(921);
			this.EquipCategorys.Add(922);
			this.EquipCategorys.Add(923);
			this.EquipCategorys.Add(924);
			this.EquipCategorys.Add(925);
			this.EquipCategorys.Add(926);
			this.EquipCategorys.Add(927);
			this.EquipCategorys.Add(928);
		}

		
		public bool initialize()
		{
			bool result;
			if (!this.LoadRandType() || !this.LoadRandInfo() || !this.LoadExp() || !this.LoadStoneType() || !this.LoadStoneGroup())
			{
				LogManager.WriteLog(LogTypes.Error, "SoulStoneManager.initialize failed!", null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1321, 3, 3, SingletonTemplate<SoulStoneManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1322, 4, 4, SingletonTemplate<SoulStoneManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1323, 3, 3, SingletonTemplate<SoulStoneManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1324, 1, 1, SingletonTemplate<SoulStoneManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1320, 1, 1, SingletonTemplate<SoulStoneManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot9))
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1320:
					result = this.ProcessSoulStoneQueryGet(client, nID, bytes, cmdParams);
					break;
				case 1321:
					result = this.ProcessSoulStoneGet(client, nID, bytes, cmdParams);
					break;
				case 1322:
					result = this.ProcessSoulStoneLevelUp(client, nID, bytes, cmdParams);
					break;
				case 1323:
					result = this.ProcessSoulStoneModEquip(client, nID, bytes, cmdParams);
					break;
				case 1324:
					result = this.ProcessSoulStoneResetBag(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		
		public void LoadJingHuaExpConfig()
		{
			Dictionary<int, int> tmpJingHuaExp = new Dictionary<int, int>();
			Dictionary<int, int> tmpLvlLimit = new Dictionary<int, int>();
			List<string> expList = GameManager.systemParamsList.GetParamValueStringListByName("HunShiExp", '|');
			if (expList != null)
			{
				foreach (string s in expList)
				{
					string[] fields = s.Split(new char[]
					{
						','
					});
					if (fields.Length == 2)
					{
						tmpJingHuaExp[Convert.ToInt32(fields[0])] = Convert.ToInt32(fields[1]);
					}
				}
			}
			List<string> lvlList = GameManager.systemParamsList.GetParamValueStringListByName("HunShiOpen", '|');
			if (lvlList != null)
			{
				for (int i = 0; i < lvlList.Count; i++)
				{
					string[] fields = lvlList[i].Split(new char[]
					{
						','
					});
					if (fields.Length == 2)
					{
						int cl = Convert.ToInt32(fields[0]);
						int lvl = Convert.ToInt32(fields[1]);
						tmpLvlLimit[i + 1] = Global.GetUnionLevel(cl, lvl, false);
					}
				}
			}
			this.jinghuaExpDict = tmpJingHuaExp;
			this.equipLvlLimitDict = tmpLvlLimit;
		}

		
		private bool LoadRandType()
		{
			try
			{
				this.defaultRandId = int.MaxValue;
				XElement randTypeXml = XElement.Load(Global.GameResPath("Config/Gem/HunShiType.xml"));
				foreach (XElement typeXml in randTypeXml.Elements())
				{
					SoulStoneRandConfig randCfg = new SoulStoneRandConfig();
					randCfg.RandId = (int)Global.GetSafeAttributeLong(typeXml, "ID");
					randCfg.NeedLangHunFenMo = (int)Global.GetSafeAttributeLong(typeXml, "NeedLangHunFenMo");
					randCfg.SuccessRate = Global.GetSafeAttributeDouble(typeXml, "SuccessRate");
					long[] tmpLongArr = Global.GetSafeAttributeLongArray(typeXml, "SuccessTo", -1);
					Debug.Assert(tmpLongArr != null);
					for (int i = 0; i < tmpLongArr.Length; i++)
					{
						randCfg.SuccessTo.Add((int)tmpLongArr[i]);
					}
					tmpLongArr = Global.GetSafeAttributeLongArray(typeXml, "FailTo", -1);
					Debug.Assert(tmpLongArr != null);
					for (int i = 0; i < tmpLongArr.Length; i++)
					{
						randCfg.FailTo.Add((int)tmpLongArr[i]);
					}
					string[] tmpStringArr = Global.GetSafeAttributeStr(typeXml, "AddedGoodsNeed").Split(new char[]
					{
						'|'
					});
					Debug.Assert(tmpStringArr != null && tmpStringArr.Length == 5);
					for (int i = 0; i < 5; i++)
					{
						randCfg.AddedNeedDict[i + ESoulStoneExtCostType.MoJing] = Convert.ToInt32(tmpStringArr[i]);
					}
					randCfg.AddedRate = Global.GetSafeAttributeDouble(typeXml, "AddedGoodsOdds");
					randCfg.AddedGoods = Global.ParseGoodsFromStr_7(Global.GetSafeAttributeStr(typeXml, "AddedGoods").Split(new char[]
					{
						','
					}), 0);
					tmpStringArr = Global.GetSafeAttributeStr(typeXml, "ReduceNeed").Split(new char[]
					{
						'|'
					});
					Debug.Assert(tmpStringArr != null && tmpStringArr.Length == 5);
					for (int i = 0; i < 5; i++)
					{
						randCfg.ReduceNeedDict[i + ESoulStoneExtCostType.MoJing] = Convert.ToInt32(tmpStringArr[i]);
					}
					randCfg.ReduceRate = Global.GetSafeAttributeDouble(typeXml, "ReduceOdds");
					randCfg.ReduceValue = (int)Global.GetSafeAttributeLong(typeXml, "ReduceNum");
					tmpStringArr = Global.GetSafeAttributeStr(typeXml, "AdvanceSuccessNeed").Split(new char[]
					{
						'|'
					});
					Debug.Assert(tmpStringArr != null && tmpStringArr.Length == 5);
					for (int i = 0; i < 5; i++)
					{
						randCfg.UpSucRateNeedDict[i + ESoulStoneExtCostType.MoJing] = Convert.ToInt32(tmpStringArr[i]);
					}
					randCfg.UpSucRateTo = Global.GetSafeAttributeDouble(typeXml, "AdvanceSuccessRate");
					tmpStringArr = Global.GetSafeAttributeStr(typeXml, "HoldTypeNeed").Split(new char[]
					{
						'|'
					});
					Debug.Assert(tmpStringArr != null && tmpStringArr.Length == 5);
					for (int i = 0; i < 5; i++)
					{
						randCfg.FailHoldNeedDict[i + ESoulStoneExtCostType.MoJing] = Convert.ToInt32(tmpStringArr[i]);
					}
					tmpLongArr = Global.GetSafeAttributeLongArray(typeXml, "HoldTypeFailTo", -1);
					Debug.Assert(tmpLongArr != null);
					for (int i = 0; i < tmpLongArr.Length; i++)
					{
						randCfg.FailToIfHold.Add((int)tmpLongArr[i]);
					}
					this.randDict.Add(randCfg.RandId, randCfg);
					this.defaultRandId = Math.Min(this.defaultRandId, randCfg.RandId);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "load config file Config/Gem/HunShiType.xml failed", ex, true);
				return false;
			}
			return true;
		}

		
		private bool LoadRandInfo()
		{
			try
			{
				XElement randInfoXml = XElement.Load(Global.GameResPath("Config/Gem/HunShi.xml"));
				foreach (XElement randXml in randInfoXml.Elements())
				{
					SoulStoneRandConfig randCfg = null;
					int randId = (int)Global.GetSafeAttributeLong(randXml, "TypeID");
					if (!this.randDict.TryGetValue(randId, out randCfg))
					{
						throw new Exception("can't find typeid=" + randId + ", please check Config/Gem/HunShiType.xml");
					}
					randCfg.RandMinNumber = int.MaxValue;
					randCfg.RandMaxNumber = int.MinValue;
					foreach (XElement xml in randXml.Elements())
					{
						SoulStoneRandInfo info = new SoulStoneRandInfo();
						info.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
						info.Goods = Global.ParseGoodsFromStr_7(Global.GetSafeAttributeStr(xml, "Goods").Split(new char[]
						{
							','
						}), 0);
						info.RandBegin = (int)Global.GetSafeAttributeLong(xml, "BeginNum");
						info.RandEnd = (int)Global.GetSafeAttributeLong(xml, "EndNum");
						randCfg.RandMinNumber = Math.Min(randCfg.RandMinNumber, info.RandBegin);
						randCfg.RandMaxNumber = Math.Max(randCfg.RandMaxNumber, info.RandEnd);
						randCfg.RandStoneList.Add(info);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "load config file Config/Gem/HunShiType.xml failed", ex, true);
				return false;
			}
			return true;
		}

		
		private bool LoadExp()
		{
			try
			{
				XElement xml = XElement.Load(Global.GameResPath("Config/Gem/HunShiExp.xml"));
				foreach (XElement suitXml in xml.Elements())
				{
					SoulStoneExpConfig expCfg = new SoulStoneExpConfig();
					expCfg.Suit = (int)Global.GetSafeAttributeLong(suitXml, "SuitID");
					expCfg.MinLevel = int.MaxValue;
					expCfg.MaxLevel = int.MinValue;
					Dictionary<int, int> tmpLvlExp = new Dictionary<int, int>();
					foreach (XElement lvlXml in suitXml.Elements())
					{
						int lvl = (int)Global.GetSafeAttributeLong(lvlXml, "ID");
						int exp = (int)Global.GetSafeAttributeLong(lvlXml, "Exp");
						expCfg.MinLevel = Math.Min(expCfg.MinLevel, lvl);
						expCfg.MaxLevel = Math.Max(expCfg.MaxLevel, lvl);
						tmpLvlExp.Add(lvl, exp);
					}
					int prevTotalExp = 0;
					int curLvlExp = 0;
					for (int lvl = expCfg.MinLevel; lvl <= expCfg.MaxLevel; lvl++)
					{
						if (!tmpLvlExp.TryGetValue(lvl, out curLvlExp))
						{
							curLvlExp = 0;
						}
						expCfg.Lvl2Exp.Add(lvl, curLvlExp + prevTotalExp);
						prevTotalExp += curLvlExp;
					}
					this.suitExpDict.Add(expCfg.Suit, expCfg);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "load config file Config/Gem/HunShiExp.xml failed", ex, true);
				return false;
			}
			return true;
		}

		
		private bool LoadStoneType()
		{
			try
			{
				XElement xml = XElement.Load(Global.GameResPath("Config/Gem/HunShiGoodsType.xml"));
				foreach (XElement typeXml in xml.Elements())
				{
					int type = (int)Global.GetSafeAttributeLong(typeXml, "ID");
					long[] stones = Global.GetSafeAttributeLongArray(typeXml, "Goods", -1);
					for (int i = 0; i < stones.Length; i++)
					{
						this.stone2TypeDict.Add((int)stones[i], type);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "load config file Config/Gem/HunShiGoodsType.xml failed", ex, true);
				return false;
			}
			return true;
		}

		
		private bool LoadStoneGroup()
		{
			try
			{
				XElement xml = XElement.Load(Global.GameResPath("Config/Gem/HunShiGroup.xml"));
				foreach (XElement groupXml in xml.Elements())
				{
					SoulStoneGroupConfig groupCfg = new SoulStoneGroupConfig();
					groupCfg.Group = (int)Global.GetSafeAttributeLong(groupXml, "ID");
					long[] stones = Global.GetSafeAttributeLongArray(groupXml, "HunShiGoodsType", -1);
					for (int i = 0; i < stones.Length; i++)
					{
						groupCfg.NeedTypeList.Add((int)stones[i]);
					}
					string[] strProps = Global.GetSafeAttributeStr(groupXml, "GroupProperty").Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < strProps.Length; i++)
					{
						string[] oneProp = strProps[i].Split(new char[]
						{
							','
						});
						ExtPropIndexes propIndex;
						if (Enum.TryParse<ExtPropIndexes>(oneProp[0], out propIndex))
						{
							double val = Convert.ToDouble(oneProp[1]);
							groupCfg.AttrValue.Add((int)propIndex, val);
						}
						else
						{
							LogManager.WriteLog(LogTypes.Error, string.Concat(new string[]
							{
								"can't parse ",
								groupCfg.Group.ToString(),
								" ",
								oneProp[0],
								" as ExtPropIndexes"
							}), null, true);
						}
					}
					this.groupDict.Add(groupCfg.Group, groupCfg);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, "load config file Config/Gem/HunShiGroup.xml failed", ex, true);
				return false;
			}
			return true;
		}

		
		private List<SoulStoneExtFuncItem> GetExtFuncItems()
		{
			JieRiFuLiActivity act = HuodongCachingMgr.GetJieriFuLiActivity();
			List<SoulStoneExtFuncItem> result;
			if (act == null)
			{
				result = null;
			}
			else
			{
				object arg = null;
				if (!act.IsOpened(EJieRiFuLiType.SoulStoneExtFunc, out arg))
				{
					result = null;
				}
				else if (arg == null)
				{
					result = null;
				}
				else
				{
					List<Tuple<int, int>> tList = arg as List<Tuple<int, int>>;
					if (tList == null || tList.Count <= 0)
					{
						result = null;
					}
					else
					{
						List<SoulStoneExtFuncItem> resultList = new List<SoulStoneExtFuncItem>();
						for (int i = 0; i < tList.Count; i++)
						{
							resultList.Add(new SoulStoneExtFuncItem
							{
								FuncType = tList[i].Item1,
								CostType = tList[i].Item2
							});
						}
						result = resultList;
					}
				}
			}
			return result;
		}

		
		private bool IsGongNengOpened(GameClient client)
		{
			return client != null && GlobalNew.IsGongNengOpened(client, GongNengIDs.SoulStone, false);
		}

		
		private int GenerateBagIndex(int cycle, int grid)
		{
			return cycle * 100 + grid;
		}

		
		private void ParseCycleAndGrid(int bagIndex, out int cycle, out int grid)
		{
			cycle = bagIndex / 100;
			grid = bagIndex % 100;
		}

		
		public void CheckOpen(GameClient client)
		{
			if (client != null)
			{
				if (!client.ClientData.IsSoulStoneOpened)
				{
					if (this.IsGongNengOpened(client))
					{
						client.ClientData.IsSoulStoneOpened = true;
						string szRandId = Global.GetRoleParamByName(client, "SoulStoneRandId");
						int iRand = 0;
						if (string.IsNullOrEmpty(szRandId) || !int.TryParse(szRandId, out iRand) || !this.randDict.ContainsKey(iRand))
						{
							Global.SaveRoleParamsInt32ValueToDB(client, "SoulStoneRandId", this.defaultRandId, true);
						}
						this.ResetSoulStoneBag(client);
						this.UpdateProps(client);
					}
				}
			}
		}

		
		public bool CanAddGoodsNum(GameClient client, int num)
		{
			return client != null && num > 0 && num + client.ClientData.SoulStoneInBag.Count <= 100;
		}

		
		public int GetIdleSlotOfBag(GameClient client)
		{
			byte[] flagArray = new byte[100];
			for (int i = 0; i < client.ClientData.SoulStoneInBag.Count; i++)
			{
				int bagIndex = client.ClientData.SoulStoneInBag[i].BagIndex;
				if (bagIndex >= 0 && bagIndex < 100)
				{
					flagArray[bagIndex] = 1;
				}
			}
			for (int i = 0; i < 100; i++)
			{
				if (flagArray[i] == 0)
				{
					return i;
				}
			}
			return -1;
		}

		
		private void AddSoulStoneGoods(GameClient client, GoodsData gd, int site)
		{
			if (client != null && gd != null)
			{
				gd.Site = site;
				if (site == 8000)
				{
					client.ClientData.SoulStoneInBag.Add(gd);
				}
				else if (site == 8001)
				{
					client.ClientData.SoulStoneInUsing.Add(gd);
				}
			}
		}

		
		public GoodsData AddSoulStoneGoods(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
		{
			GoodsData gd = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = startTime,
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = binding,
				Jewellist = jewelList,
				BagIndex = bagIndex,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife,
				WashProps = washProps
			};
			this.AddSoulStoneGoods(client, gd, gd.Site);
			return gd;
		}

		
		public void RemoveSoulStoneGoods(GameClient client, GoodsData goodsData, int site)
		{
			if (goodsData != null && client != null)
			{
				if (goodsData.Site == 8000)
				{
					client.ClientData.SoulStoneInBag.Remove(goodsData);
				}
				else if (goodsData.Site == 8001)
				{
					client.ClientData.SoulStoneInUsing.Remove(goodsData);
				}
			}
		}

		
		private GoodsData GetSoulStoneByDbId(GameClient client, int site, int dbid)
		{
			GoodsData result;
			if (site == 8000)
			{
				result = client.ClientData.SoulStoneInBag.Find((GoodsData _g) => _g.Id == dbid);
			}
			else if (site == 8001)
			{
				result = client.ClientData.SoulStoneInUsing.Find((GoodsData _g) => _g.Id == dbid);
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		private void UpdateProps(GameClient client)
		{
			if (client != null)
			{
				EquipPropItem totalProp = new EquipPropItem();
				List<int>[] eachCycleStones = new List<int>[4];
				for (int i = 1; i <= 3; i++)
				{
					eachCycleStones[i] = new List<int>();
				}
				foreach (GoodsData gd in client.ClientData.SoulStoneInUsing)
				{
					int cycle2 = 0;
					int grid = 0;
					this.ParseCycleAndGrid(gd.BagIndex, out cycle2, out grid);
					if (cycle2 >= 1 && cycle2 <= 3 && this.stone2TypeDict.ContainsKey(gd.GoodsID))
					{
						eachCycleStones[cycle2].Add(this.stone2TypeDict[gd.GoodsID]);
					}
					int lvl = (gd.ElementhrtsProps != null) ? gd.ElementhrtsProps[0] : 1;
					EquipPropItem baseProp = GameManager.EquipPropsMgr.FindEquipPropItem(gd.GoodsID);
					int i = 0;
					while (baseProp != null && i < baseProp.ExtProps.Length)
					{
						totalProp.ExtProps[i] += baseProp.ExtProps[i] * (double)lvl;
						i++;
					}
				}
				foreach (KeyValuePair<int, SoulStoneGroupConfig> kvp in this.groupDict)
				{
					SoulStoneGroupConfig group = kvp.Value;
					if (group.NeedTypeList != null && group.NeedTypeList.Count > 0)
					{
						int cycle;
						for (cycle = 1; cycle <= 3; cycle++)
						{
							if (group.NeedTypeList.All((int _t) => eachCycleStones[cycle].Contains(_t)))
							{
								foreach (KeyValuePair<int, double> attrKvp in group.AttrValue)
								{
									if (attrKvp.Key >= 0 && attrKvp.Key < totalProp.ExtProps.Length)
									{
										totalProp.ExtProps[attrKvp.Key] += attrKvp.Value;
									}
								}
							}
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.SoulStone,
					totalProp.ExtProps
				});
			}
		}

		
		private int ExtCostTypeHadValue(GameClient client, ESoulStoneExtCostType costType)
		{
			int val = 0;
			if (costType == ESoulStoneExtCostType.MoJing)
			{
				val = GameManager.ClientMgr.GetTianDiJingYuanValue(client);
			}
			else if (costType == ESoulStoneExtCostType.XingHun)
			{
				val = client.ClientData.StarSoul;
			}
			else if (costType == ESoulStoneExtCostType.ChengJiu)
			{
				val = GameManager.ClientMgr.GetChengJiuPointsValue(client);
			}
			else if (costType == ESoulStoneExtCostType.ShengWang)
			{
				val = GameManager.ClientMgr.GetShengWangValue(client);
			}
			else if (costType == ESoulStoneExtCostType.ZuanShi)
			{
				val = client.ClientData.UserMoney;
			}
			return val;
		}

		
		private bool DoExtCostType(GameClient client, ESoulStoneExtCostType costType, int val)
		{
			bool result;
			if (val <= 0)
			{
				result = true;
			}
			else
			{
				if (costType == ESoulStoneExtCostType.MoJing)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, -val, "聚魂额外消耗", true, true, false);
				}
				else if (costType == ESoulStoneExtCostType.XingHun)
				{
					GameManager.ClientMgr.ModifyStarSoulValue(client, -val, "聚魂额外消耗", true, true);
				}
				else if (costType == ESoulStoneExtCostType.ChengJiu)
				{
					GameManager.ClientMgr.ModifyChengJiuPointsValue(client, -val, "聚魂额外消耗", true, true);
				}
				else if (costType == ESoulStoneExtCostType.ShengWang)
				{
					GameManager.ClientMgr.ModifyShengWangValue(client, -val, "聚魂额外消耗", true, true);
				}
				else if (costType == ESoulStoneExtCostType.ZuanShi)
				{
					GameManager.ClientMgr.SubUserMoney(client, val, "聚魂额外消耗", true, true, true, true, DaiBiSySType.None);
				}
				result = true;
			}
			return result;
		}

		
		private bool ProcessSoulStoneQueryGet(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				client.sendCmd<SoulStoneQueryGetData>(nID, new SoulStoneQueryGetData
				{
					CurrRandId = Global.GetRoleParamsInt32FromDB(client, "SoulStoneRandId"),
					ExtFuncList = this.GetExtFuncItems()
				}, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return true;
		}

		
		private bool ProcessSoulStoneResetBag(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client))
				{
					return true;
				}
				int roleid = Convert.ToInt32(cmdParams[0]);
				this.ResetSoulStoneBag(client);
				client.sendCmd<List<GoodsData>>(nID, client.ClientData.SoulStoneInBag, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return true;
		}

		
		private void ResetSoulStoneBag(GameClient client)
		{
			if (client != null)
			{
				client.ClientData.SoulStoneInBag.Sort(delegate(GoodsData _left, GoodsData _right)
				{
					int l_suit = Global.GetEquipGoodsSuitID(_left.GoodsID);
					int r_suit = Global.GetEquipGoodsSuitID(_right.GoodsID);
					int result;
					if (l_suit > r_suit)
					{
						result = -1;
					}
					else if (l_suit < r_suit)
					{
						result = 1;
					}
					else
					{
						int lvlDiff = 0;
						if (_left.ElementhrtsProps != null && _right.ElementhrtsProps != null)
						{
							lvlDiff = _left.ElementhrtsProps[0] - _right.ElementhrtsProps[0];
						}
						if (lvlDiff > 0)
						{
							result = -1;
						}
						else if (lvlDiff < 0)
						{
							result = 1;
						}
						else
						{
							result = _left.GoodsID - _right.GoodsID;
						}
					}
					return result;
				});
				for (int i = 0; i < client.ClientData.SoulStoneInBag.Count; i++)
				{
					client.ClientData.SoulStoneInBag[i].BagIndex = i;
				}
			}
		}

		
		private bool ProcessSoulStoneModEquip(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleid = Convert.ToInt32(cmdParams[0]);
				int bagIndex = Convert.ToInt32(cmdParams[1]);
				int dbid = Convert.ToInt32(cmdParams[2]);
				ESoulStoneErrorCode ec = this.handleModEquip(client, bagIndex, dbid);
				string rsp = string.Format("{0}:{1}:{2}", (int)ec, bagIndex, dbid);
				client.sendCmd(nID, rsp, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return true;
		}

		
		private ESoulStoneErrorCode handleModEquip(GameClient client, int bagIndex, int newDbId)
		{
			ESoulStoneErrorCode result;
			if (!this.IsGongNengOpened(client))
			{
				result = ESoulStoneErrorCode.NotOpen;
			}
			else
			{
				int cycle = 0;
				int grid = 0;
				this.ParseCycleAndGrid(bagIndex, out cycle, out grid);
				if (cycle < 1 || cycle > 3 || grid < 1 || grid > 6)
				{
					result = ESoulStoneErrorCode.VisitParamsError;
				}
				else
				{
					GoodsData newGd = null;
					if (newDbId != -1)
					{
						newGd = client.ClientData.SoulStoneInBag.Find((GoodsData _g) => _g.Id == newDbId);
						if (newGd == null)
						{
							return ESoulStoneErrorCode.VisitParamsError;
						}
						Dictionary<int, int> tmpLvlLimitDict = this.equipLvlLimitDict;
						if (tmpLvlLimitDict == null)
						{
							return ESoulStoneErrorCode.ConfigError;
						}
						if (!tmpLvlLimitDict.ContainsKey(cycle) || tmpLvlLimitDict[cycle] > Global.GetUnionLevel(client, false))
						{
							return ESoulStoneErrorCode.CanNotEquip;
						}
						SystemXmlItem systemGoods = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(newGd.GoodsID, out systemGoods))
						{
							return ESoulStoneErrorCode.ConfigError;
						}
						int categoriy = systemGoods.GetIntValue("Categoriy", -1);
						if (!this.EquipCategorys.Contains(categoriy))
						{
							return ESoulStoneErrorCode.CanNotEquip;
						}
						GoodsData checkGd = client.ClientData.SoulStoneInUsing.Find(delegate(GoodsData _g)
						{
							int _cycle = 0;
							int _grid = 0;
							this.ParseCycleAndGrid(_g.BagIndex, out _cycle, out _grid);
							return _cycle == cycle && _grid != grid && _g.GoodsID == newGd.GoodsID;
						});
						if (checkGd != null)
						{
							return ESoulStoneErrorCode.CanNotEquip;
						}
					}
					GoodsData oldGd = client.ClientData.SoulStoneInUsing.Find((GoodsData _g) => _g.BagIndex == bagIndex);
					if (oldGd != null)
					{
						if (!this.CanAddGoodsNum(client, 1))
						{
							return ESoulStoneErrorCode.BagNoSpace;
						}
						int newBagIndex = this.GetIdleSlotOfBag(client);
						if (newBagIndex < 0)
						{
							return ESoulStoneErrorCode.BagNoSpace;
						}
						int newSite = 8000;
						string[] dbFields = null;
						string strCmd = Global.FormatUpdateDBGoodsStr(new object[]
						{
							client.ClientData.RoleID,
							oldGd.Id,
							"*",
							"*",
							"*",
							"*",
							newSite,
							"*",
							"*",
							oldGd.GCount,
							"*",
							newBagIndex,
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*"
						});
						TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strCmd, out dbFields, client.ServerId);
						if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED || dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
						{
							return ESoulStoneErrorCode.DbFailed;
						}
						this.RemoveSoulStoneGoods(client, oldGd, oldGd.Site);
						oldGd.BagIndex = newBagIndex;
						oldGd.Site = newSite;
						this.AddSoulStoneGoods(client, oldGd, oldGd.Site);
						GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 1, oldGd.Id, oldGd.Using, oldGd.Site, oldGd.GCount, oldGd.BagIndex, 1);
					}
					if (newGd != null)
					{
						int newBagIndex = bagIndex;
						int newSite = 8001;
						string[] dbFields = null;
						string strCmd = Global.FormatUpdateDBGoodsStr(new object[]
						{
							client.ClientData.RoleID,
							newGd.Id,
							"*",
							"*",
							"*",
							"*",
							newSite,
							"*",
							"*",
							newGd.GCount,
							"*",
							newBagIndex,
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*"
						});
						TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strCmd, out dbFields, client.ServerId);
						if (dbRequestResult != TCPProcessCmdResults.RESULT_FAILED && dbFields.Length > 0 && Convert.ToInt32(dbFields[1]) >= 0)
						{
							this.RemoveSoulStoneGoods(client, newGd, newGd.Site);
							newGd.BagIndex = newBagIndex;
							newGd.Site = newSite;
							this.AddSoulStoneGoods(client, newGd, newSite);
							GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 4, newGd.Id, newGd.Using, newGd.Site, newGd.GCount, newGd.BagIndex, 1);
						}
						else if (oldGd == null)
						{
							return ESoulStoneErrorCode.DbFailed;
						}
					}
					this.UpdateProps(client);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					result = ESoulStoneErrorCode.Success;
				}
			}
			return result;
		}

		
		private bool ProcessSoulStoneLevelUp(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleid = Convert.ToInt32(cmdParams[0]);
				int target = Convert.ToInt32(cmdParams[1]);
				int site = Convert.ToInt32(cmdParams[2]);
				string[] szSourceList = cmdParams[3].Split(new char[]
				{
					','
				});
				List<int> srcList = new List<int>();
				for (int i = 0; i < szSourceList.Length; i++)
				{
					if (!string.IsNullOrEmpty(szSourceList[i]))
					{
						srcList.Add(Convert.ToInt32(szSourceList[i]));
					}
				}
				srcList = srcList.Distinct<int>().ToList<int>();
				int currLvl;
				int currExp;
				ESoulStoneErrorCode ec = this.handleSoulStoneLevelUp(client, target, site, srcList, out currLvl, out currExp);
				string rsp = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					(int)ec,
					target,
					site,
					currLvl,
					currExp
				});
				client.sendCmd(nID, rsp, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return true;
		}

		
		private ESoulStoneErrorCode handleSoulStoneLevelUp(GameClient client, int target, int site, List<int> srcList, out int currLvl, out int currExp)
		{
			currLvl = 0;
			currExp = 0;
			ESoulStoneErrorCode result;
			if (!this.IsGongNengOpened(client))
			{
				result = ESoulStoneErrorCode.NotOpen;
			}
			else if (srcList == null || srcList.Count <= 0)
			{
				result = ESoulStoneErrorCode.VisitParamsError;
			}
			else if (srcList.IndexOf(target) >= 0)
			{
				result = ESoulStoneErrorCode.VisitParamsError;
			}
			else
			{
				GoodsData targetGd = this.GetSoulStoneByDbId(client, site, target);
				if (targetGd == null)
				{
					result = ESoulStoneErrorCode.VisitParamsError;
				}
				else
				{
					SystemXmlItem targetGoodsXml = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(targetGd.GoodsID, out targetGoodsXml))
					{
						result = ESoulStoneErrorCode.ConfigError;
					}
					else if (!this.EquipCategorys.Contains(targetGoodsXml.GetIntValue("Categoriy", -1)))
					{
						result = ESoulStoneErrorCode.VisitParamsError;
					}
					else
					{
						int targetSuit = targetGoodsXml.GetIntValue("SuitID", -1);
						SoulStoneExpConfig targetExpCfg = null;
						if (!this.suitExpDict.TryGetValue(targetSuit, out targetExpCfg))
						{
							result = ESoulStoneErrorCode.ConfigError;
						}
						else if (targetGd.ElementhrtsProps == null)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("roleid={0}, dbid={1}的魂石等级和经验为null", client.ClientData.RoleID, target), null, true);
							result = ESoulStoneErrorCode.UnknownFailed;
						}
						else if (targetGd.ElementhrtsProps[0] >= targetExpCfg.MaxLevel)
						{
							result = ESoulStoneErrorCode.LevelIsFull;
						}
						else
						{
							int addExp = 0;
							foreach (int srcId in srcList)
							{
								GoodsData srcGd = this.GetSoulStoneByDbId(client, 8000, srcId);
								if (srcGd == null)
								{
									return ESoulStoneErrorCode.VisitParamsError;
								}
								SystemXmlItem systemMaterial = null;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(srcGd.GoodsID, out systemMaterial))
								{
									return ESoulStoneErrorCode.ConfigError;
								}
								if (910 == systemMaterial.GetIntValue("Categoriy", -1))
								{
									Dictionary<int, int> tmpJingHuaExp = this.jinghuaExpDict;
									if (tmpJingHuaExp == null || !tmpJingHuaExp.ContainsKey(srcGd.GoodsID))
									{
										return ESoulStoneErrorCode.ConfigError;
									}
									addExp += tmpJingHuaExp[srcGd.GoodsID] * srcGd.GCount;
								}
								else
								{
									int suitid = systemMaterial.GetIntValue("SuitID", -1);
									SoulStoneExpConfig expCfg = null;
									if (!this.suitExpDict.TryGetValue(suitid, out expCfg))
									{
										return ESoulStoneErrorCode.ConfigError;
									}
									if (srcGd.ElementhrtsProps == null)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("roleid={0}, dbid={1}的魂石等级和经验为null", client.ClientData.RoleID, srcGd.Id), null, true);
										return ESoulStoneErrorCode.UnknownFailed;
									}
									int hadExp;
									if (!expCfg.Lvl2Exp.TryGetValue(srcGd.ElementhrtsProps[0], out hadExp))
									{
										return ESoulStoneErrorCode.ConfigError;
									}
									addExp += hadExp * srcGd.GCount + srcGd.ElementhrtsProps[1] * srcGd.GCount;
								}
							}
							foreach (int srcId in srcList)
							{
								GoodsData srcGd = this.GetSoulStoneByDbId(client, 8000, srcId);
								if (srcGd != null)
								{
									if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, srcGd, srcGd.GCount, false, false))
									{
									}
								}
							}
							List<int> elementhrtsProps;
							(elementhrtsProps = targetGd.ElementhrtsProps)[1] = elementhrtsProps[1] + addExp;
							while (targetGd.ElementhrtsProps[0] < targetExpCfg.MaxLevel)
							{
								int currLvlExp = 0;
								int nextLvlExp = 0;
								if (!targetExpCfg.Lvl2Exp.TryGetValue(targetGd.ElementhrtsProps[0], out currLvlExp) || !targetExpCfg.Lvl2Exp.TryGetValue(targetGd.ElementhrtsProps[0] + 1, out nextLvlExp))
								{
									break;
								}
								int needExp = nextLvlExp - currLvlExp;
								if (targetGd.ElementhrtsProps[1] < needExp)
								{
									break;
								}
								(elementhrtsProps = targetGd.ElementhrtsProps)[0] = elementhrtsProps[0] + 1;
								(elementhrtsProps = targetGd.ElementhrtsProps)[1] = elementhrtsProps[1] - needExp;
							}
							UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
							{
								RoleID = client.ClientData.RoleID,
								DbID = target,
								WashProps = null
							};
							updateGoodsArgs.ElementhrtsProps = new List<int>();
							updateGoodsArgs.ElementhrtsProps.Add(targetGd.ElementhrtsProps[0]);
							updateGoodsArgs.ElementhrtsProps.Add(targetGd.ElementhrtsProps[1]);
							Global.UpdateGoodsProp(client, targetGd, updateGoodsArgs, true);
							if (targetGd.Site == 8001)
							{
								this.UpdateProps(client);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							}
							currLvl = targetGd.ElementhrtsProps[0];
							currExp = targetGd.ElementhrtsProps[1];
							result = ESoulStoneErrorCode.Success;
						}
					}
				}
			}
			return result;
		}

		
		private bool ProcessSoulStoneGet(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleid = Convert.ToInt32(cmdParams[0]);
				int reqTimes = Convert.ToInt32(cmdParams[1]);
				string[] szExtFuncs = cmdParams[2].Split(new char[]
				{
					','
				});
				List<int> selectExtFuncs = null;
				if (szExtFuncs.Length > 0)
				{
					selectExtFuncs = new List<int>();
					for (int i = 0; i < szExtFuncs.Length; i++)
					{
						if (!string.IsNullOrEmpty(szExtFuncs[i]))
						{
							selectExtFuncs.Add(Convert.ToInt32(szExtFuncs[i]));
						}
					}
					selectExtFuncs = selectExtFuncs.Distinct<int>().ToList<int>();
				}
				List<SoulStoneExtFuncItem> openedExtFuncs = this.GetExtFuncItems();
				SoulStoneGetData data = new SoulStoneGetData();
				data.RequestTimes = reqTimes;
				data.RealDoTimes = 0;
				if (reqTimes != 1 && reqTimes != 10)
				{
					data.Error = 2;
				}
				else
				{
					data.Stones = new List<int>();
					data.ExtGoods = new List<int>();
					for (int times = 1; times <= reqTimes; times++)
					{
						List<int> goodsIdList = null;
						List<int> extGoodsList = null;
						ESoulStoneErrorCode ec = this.handleSoulStoneGetOne(client, selectExtFuncs, openedExtFuncs, out goodsIdList, out extGoodsList, times);
						if (ec != ESoulStoneErrorCode.Success)
						{
							if (data.RealDoTimes == 0)
							{
								data.Error = (int)ec;
							}
							break;
						}
						data.Error = 0;
						data.RealDoTimes++;
						if (goodsIdList != null)
						{
							data.Stones.AddRange(goodsIdList);
						}
						if (extGoodsList != null)
						{
							data.ExtGoods.AddRange(extGoodsList);
						}
					}
				}
				data.NewRandId = Global.GetRoleParamsInt32FromDB(client, "SoulStoneRandId");
				client.sendCmd<SoulStoneGetData>(nID, data, false);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return true;
		}

		
		private ESoulStoneErrorCode handleSoulStoneGetOne(GameClient client, List<int> selectExtFuncs, List<SoulStoneExtFuncItem> openedExtFuncs, out List<int> goodsIdList, out List<int> extGoodsList, int currTimes)
		{
			goodsIdList = new List<int>();
			extGoodsList = new List<int>();
			ESoulStoneErrorCode result;
			if (!this.IsGongNengOpened(client))
			{
				result = ESoulStoneErrorCode.NotOpen;
			}
			else if (selectExtFuncs != null && selectExtFuncs.Count > 0 && !selectExtFuncs.All((int _type) => openedExtFuncs != null && openedExtFuncs.Exists((SoulStoneExtFuncItem _item) => _item.FuncType == _type)))
			{
				result = ESoulStoneErrorCode.SelectExtFuncNotOpen;
			}
			else
			{
				int oldRandId = Global.GetRoleParamsInt32FromDB(client, "SoulStoneRandId");
				SoulStoneRandConfig randCfg = null;
				if (!this.randDict.TryGetValue(oldRandId, out randCfg))
				{
					result = ESoulStoneErrorCode.ConfigError;
				}
				else
				{
					SoulStoneExtFuncItem addedItem = null;
					SoulStoneExtFuncItem reduceItem = null;
					SoulStoneExtFuncItem upSucessItem = null;
					SoulStoneExtFuncItem holdTypeItem = null;
					if (selectExtFuncs != null && openedExtFuncs != null && selectExtFuncs.Contains(1))
					{
						addedItem = openedExtFuncs.Find((SoulStoneExtFuncItem _item) => _item.FuncType == 1);
					}
					if (selectExtFuncs != null && openedExtFuncs != null && selectExtFuncs.Contains(2))
					{
						reduceItem = openedExtFuncs.Find((SoulStoneExtFuncItem _item) => _item.FuncType == 2);
					}
					if (selectExtFuncs != null && openedExtFuncs != null && selectExtFuncs.Contains(3))
					{
						upSucessItem = openedExtFuncs.Find((SoulStoneExtFuncItem _item) => _item.FuncType == 3);
					}
					if (selectExtFuncs != null && openedExtFuncs != null && selectExtFuncs.Contains(4))
					{
						holdTypeItem = openedExtFuncs.Find((SoulStoneExtFuncItem _item) => _item.FuncType == 4);
					}
					Dictionary<ESoulStoneExtCostType, int> extCostDict = new Dictionary<ESoulStoneExtCostType, int>
					{
						{
							ESoulStoneExtCostType.MoJing,
							0
						},
						{
							ESoulStoneExtCostType.XingHun,
							0
						},
						{
							ESoulStoneExtCostType.ChengJiu,
							0
						},
						{
							ESoulStoneExtCostType.ShengWang,
							0
						},
						{
							ESoulStoneExtCostType.ZuanShi,
							0
						}
					};
					bool bAddedItem = false;
					bool bUpSuccessRate = false;
					bool bHoldType = false;
					int costLangHun = randCfg.NeedLangHunFenMo;
					if (addedItem != null)
					{
						ESoulStoneExtCostType costType = (ESoulStoneExtCostType)addedItem.CostType;
						int costValue;
						if (randCfg.AddedNeedDict.TryGetValue(costType, out costValue) && costValue > 0)
						{
							int hadVal = this.ExtCostTypeHadValue(client, costType);
							if (hadVal < costValue + extCostDict[costType])
							{
								return ESoulStoneErrorCode.ExtCostNotEnough;
							}
							Dictionary<ESoulStoneExtCostType, int> dictionary;
							ESoulStoneExtCostType key;
							(dictionary = extCostDict)[key = costType] = dictionary[key] + costValue;
							if ((double)Global.GetRandomNumber(1, 100) * 1.0 / 100.0 <= randCfg.AddedRate)
							{
								bAddedItem = true;
							}
						}
					}
					if (reduceItem != null)
					{
						ESoulStoneExtCostType costType = (ESoulStoneExtCostType)reduceItem.CostType;
						int costValue;
						if (randCfg.ReduceNeedDict.TryGetValue(costType, out costValue) && costValue > 0)
						{
							int hadVal = this.ExtCostTypeHadValue(client, costType);
							if (hadVal < costValue + extCostDict[costType])
							{
								return ESoulStoneErrorCode.ExtCostNotEnough;
							}
							Dictionary<ESoulStoneExtCostType, int> dictionary;
							ESoulStoneExtCostType key;
							(dictionary = extCostDict)[key = costType] = dictionary[key] + costValue;
							if ((double)Global.GetRandomNumber(1, 100) * 1.0 / 100.0 <= randCfg.ReduceRate)
							{
								costLangHun -= randCfg.ReduceValue;
							}
						}
					}
					if (upSucessItem != null)
					{
						ESoulStoneExtCostType costType = (ESoulStoneExtCostType)upSucessItem.CostType;
						int costValue;
						if (randCfg.UpSucRateNeedDict.TryGetValue(costType, out costValue) && costValue > 0)
						{
							int hadVal = this.ExtCostTypeHadValue(client, costType);
							if (hadVal < costValue + extCostDict[costType])
							{
								return ESoulStoneErrorCode.ExtCostNotEnough;
							}
							Dictionary<ESoulStoneExtCostType, int> dictionary;
							ESoulStoneExtCostType key;
							(dictionary = extCostDict)[key = costType] = dictionary[key] + costValue;
							bUpSuccessRate = true;
						}
					}
					if (holdTypeItem != null)
					{
						ESoulStoneExtCostType costType = (ESoulStoneExtCostType)holdTypeItem.CostType;
						int costValue;
						if (randCfg.UpSucRateNeedDict.TryGetValue(costType, out costValue) && costValue > 0)
						{
							int hadVal = this.ExtCostTypeHadValue(client, costType);
							if (hadVal < costValue + extCostDict[costType])
							{
								return ESoulStoneErrorCode.ExtCostNotEnough;
							}
							Dictionary<ESoulStoneExtCostType, int> dictionary;
							ESoulStoneExtCostType key;
							(dictionary = extCostDict)[key = costType] = dictionary[key] + costValue;
							bHoldType = true;
						}
					}
					costLangHun = Math.Max(0, costLangHun);
					if (costLangHun > 0 && costLangHun > Global.GetRoleParamsInt32FromDB(client, "LangHunFenMo"))
					{
						result = ESoulStoneErrorCode.LangHunFenMoNotEnough;
					}
					else if (!this.CanAddGoodsNum(client, 1 + (bAddedItem ? 1 : 0)))
					{
						result = ESoulStoneErrorCode.BagNoSpace;
					}
					else
					{
						foreach (KeyValuePair<ESoulStoneExtCostType, int> kvp in extCostDict)
						{
							this.DoExtCostType(client, kvp.Key, kvp.Value);
						}
						GameManager.ClientMgr.ModifyLangHunFenMoValue(client, -costLangHun, "聚魂", true, true);
						int magic = Global.GetRandomNumber(randCfg.RandMinNumber, randCfg.RandMaxNumber);
						foreach (SoulStoneRandInfo ri in randCfg.RandStoneList)
						{
							if (ri.RandBegin <= magic && magic <= ri.RandEnd)
							{
								GoodsData gd = Global.CopyGoodsData(ri.Goods);
								List<int> elementhrtsProps = new List<int>();
								elementhrtsProps.Add(1);
								elementhrtsProps.Add(0);
								gd.Site = 8000;
								gd.ElementhrtsProps = elementhrtsProps;
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, gd.GoodsID, gd.GCount, 0, "", gd.Forge_level, gd.Binding, gd.Site, "", false, 1, "聚魂", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, elementhrtsProps, 0, true);
								goodsIdList.Add(gd.GoodsID);
								break;
							}
						}
						if (bAddedItem)
						{
							GoodsData addedGd = Global.CopyGoodsData(randCfg.AddedGoods);
							List<int> elementhrtsProps = new List<int>();
							elementhrtsProps.Add(1);
							elementhrtsProps.Add(0);
							addedGd.Site = 8000;
							addedGd.ElementhrtsProps = elementhrtsProps;
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, addedGd.GoodsID, addedGd.GCount, 0, "", addedGd.Forge_level, addedGd.Binding, addedGd.Site, "", false, 1, "聚魂", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, elementhrtsProps, 0, true);
							extGoodsList.Add(addedGd.GoodsID);
						}
						double upSuccessRate = bUpSuccessRate ? randCfg.UpSucRateTo : randCfg.SuccessRate;
						double randSuccessRate = (double)Global.GetRandomNumber(1, 101) * 1.0 / 100.0;
						int newRandId;
						if (randSuccessRate <= upSuccessRate)
						{
							newRandId = randCfg.SuccessTo[Global.GetRandomNumber(0, randCfg.SuccessTo.Count)];
						}
						else if (bHoldType)
						{
							newRandId = randCfg.FailToIfHold[Global.GetRandomNumber(0, randCfg.FailToIfHold.Count)];
						}
						else
						{
							newRandId = randCfg.FailTo[Global.GetRandomNumber(0, randCfg.FailTo.Count)];
						}
						Global.SaveRoleParamsInt32ValueToDB(client, "SoulStoneRandId", newRandId, true);
						if (this.bOpenStoneGetLog)
						{
							StringBuilder sb = new StringBuilder();
							sb.AppendFormat("rolename={0} 第{1}次聚魂, 再随机成功配置几率={2}, 产生几率={3}, 随机组变化{4}--->{5},", new object[]
							{
								client.ClientData.RoleName,
								currTimes,
								upSuccessRate,
								randSuccessRate,
								oldRandId,
								newRandId
							});
							sb.Append("消耗[");
							sb.Append("狼魂粉末:" + costLangHun + ",");
							sb.Append("魔晶:" + extCostDict[ESoulStoneExtCostType.MoJing] + ",");
							sb.Append("星魂:" + extCostDict[ESoulStoneExtCostType.XingHun] + ",");
							sb.Append("成就:" + extCostDict[ESoulStoneExtCostType.ChengJiu] + ",");
							sb.Append("声望:" + extCostDict[ESoulStoneExtCostType.ShengWang] + ",");
							sb.Append("钻石:" + extCostDict[ESoulStoneExtCostType.ZuanShi] + "]");
							sb.AppendLine();
							LogManager.WriteLog(LogTypes.Error, sb.ToString(), null, true);
						}
						result = ESoulStoneErrorCode.Success;
					}
				}
			}
			return result;
		}

		
		public void GM_Test(GameClient client, string[] args)
		{
			if (client != null)
			{
				if (args.Length >= 2)
				{
					if (args[1] == "addlanghun")
					{
						if (args.Length >= 3)
						{
							int val = Convert.ToInt32(args[2]);
							GameManager.ClientMgr.ModifyLangHunFenMoValue(client, val, "GM", true, true);
						}
					}
					else if (args[1] == "juhun")
					{
						if (args.Length >= 3)
						{
							int times = Convert.ToInt32(args[2]);
							List<int> selectExtFuncs = new List<int>();
							if (args.Length >= 4)
							{
								string[] fields = args[3].Split(new char[]
								{
									','
								});
								for (int i = 0; i < fields.Length; i++)
								{
									selectExtFuncs.Add(Convert.ToInt32(fields[i]));
								}
							}
							List<SoulStoneExtFuncItem> openedExtFuncs = this.GetExtFuncItems();
							for (int currTime = 1; currTime <= times; currTime++)
							{
							}
						}
					}
					else if (args[1] == "modequip")
					{
						if (args.Length >= 4)
						{
							int slot = Convert.ToInt32(args[2]);
							int newDbId = Convert.ToInt32(args[3]);
							this.handleModEquip(client, slot, newDbId);
						}
					}
					else if (args[1] == "resetbag")
					{
						this.ResetSoulStoneBag(client);
					}
					else if (args[1] == "lvlup")
					{
						if (args.Length >= 5)
						{
							int target = Convert.ToInt32(args[2]);
							int site = Convert.ToInt32(args[3]);
							List<int> srcList = new List<int>();
							string[] fields = args[4].Split(new char[]
							{
								','
							});
							for (int i = 0; i < fields.Length; i++)
							{
								srcList.Add(Convert.ToInt32(fields[i]));
							}
							int currLvl;
							int currExp;
							this.handleSoulStoneLevelUp(client, target, site, srcList, out currLvl, out currExp);
						}
					}
				}
			}
		}

		
		private int defaultRandId;

		
		private Dictionary<int, SoulStoneRandConfig> randDict = new Dictionary<int, SoulStoneRandConfig>();

		
		private Dictionary<int, SoulStoneExpConfig> suitExpDict = new Dictionary<int, SoulStoneExpConfig>();

		
		private Dictionary<int, int> stone2TypeDict = new Dictionary<int, int>();

		
		private Dictionary<int, SoulStoneGroupConfig> groupDict = new Dictionary<int, SoulStoneGroupConfig>();

		
		private HashSet<int> EquipCategorys = new HashSet<int>();

		
		private HashSet<int> JingHuaCategorys = new HashSet<int>();

		
		private Dictionary<int, int> jinghuaExpDict = null;

		
		private Dictionary<int, int> equipLvlLimitDict = null;

		
		private bool bOpenStoneGetLog = false;
	}
}
