using System;
using System.Collections.Generic;
using GameServer.Logic.Ornament;
using GameServer.Logic.Reborn;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class UsingEquipManager
	{
		
		public bool CanUsingEquip(GameClient client, GoodsData goodsData, int toBagIndex, bool hintClient = false)
		{
			bool result;
			if (null == goodsData)
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemGoods = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
				{
					result = false;
				}
				else if (!Global.IsCanEquipOrUseByOccupation(client, goodsData.GoodsID))
				{
					result = false;
				}
				else
				{
					int nRet = this.EquipFirstPropCondition(client, systemGoods);
					if (nRet == -1)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(556, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = false;
					}
					else if (nRet == -2)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(557, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = false;
					}
					else if (nRet == -3)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(558, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = false;
					}
					else if (nRet == -4)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(559, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = false;
					}
					else
					{
						int usingEquipResult = this._CanUsingEquip(client, goodsData, toBagIndex, systemGoods);
						if (usingEquipResult < 0)
						{
							if (hintClient)
							{
								string goodsName = systemGoods.GetStringValue("Title");
								if (-3 == usingEquipResult)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(560, new object[0]), new object[]
									{
										goodsName
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-2 == usingEquipResult)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(561, new object[0]), new object[]
									{
										goodsName
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-1 == usingEquipResult)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(562, new object[0]), new object[]
									{
										goodsName
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-5 == usingEquipResult)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(563, new object[0]), new object[]
									{
										goodsName
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-4 == usingEquipResult)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(564, new object[0]), new object[]
									{
										goodsName
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-44 == usingEquipResult)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(565, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-444 == usingEquipResult)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(566, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-55 == usingEquipResult)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(567, new object[0]), new object[]
									{
										goodsName
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		
		public int _CanUsingChongWu(int nCategoriy)
		{
			List<GoodsData> listSpecial = null;
			if (9 == nCategoriy)
			{
				if (this.EquipDict.TryGetValue(9, out listSpecial))
				{
					if (listSpecial != null && listSpecial.Count > 0)
					{
						return -4;
					}
				}
				if (!this.EquipDict.TryGetValue(10, out listSpecial))
				{
					return 0;
				}
			}
			if (10 == nCategoriy)
			{
				if (this.EquipDict.TryGetValue(10, out listSpecial))
				{
					if (listSpecial != null && listSpecial.Count > 0)
					{
						return -4;
					}
				}
				if (!this.EquipDict.TryGetValue(9, out listSpecial))
				{
					return 0;
				}
			}
			int result;
			if (listSpecial != null && listSpecial.Count > 0)
			{
				result = -2;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		public int _CanUsingEquip(GameClient client, GoodsData goodsData, int toBagIndex, SystemXmlItem systemGoods = null)
		{
			if (null == systemGoods)
			{
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
				{
					return -1;
				}
			}
			int categoriy = systemGoods.GetIntValue("Categoriy", -1);
			if (!RebornEquip.IsRebornEquip(goodsData.GoodsID))
			{
				if ((categoriy < 0 || categoriy >= 49) && categoriy != 340)
				{
					return -2;
				}
			}
			else
			{
				if (categoriy < 30 || categoriy > 38)
				{
					return -2;
				}
				int Suit = systemGoods.GetIntValue("ToReborn", -1);
				int Level = systemGoods.GetIntValue("ToRebornLevel", -1);
				if (client.ClientData.RebornCount < Suit && client.ClientData.RebornLevel < Level)
				{
					return -4;
				}
				if (goodsData.GCount <= 0)
				{
					return -5;
				}
			}
			int nHandType = systemGoods.GetIntValue("HandType", -1);
			if (categoriy < 22 && categoriy >= 11)
			{
				int nActionType = systemGoods.GetIntValue("ActionType", -1);
				int nRet = WeaponAdornManager.VerifyWeaponCanEquip(Global.CalcOriginalOccupationID(client), nHandType, nActionType, this.EquipDict);
				if (nRet < 0)
				{
					return nRet;
				}
			}
			if (categoriy <= 38 && categoriy >= 37)
			{
				int nRet = RebornEquip.VerifyWeaponCanEquip(client.UsingEquipMgr.EquipDict);
				if (nRet < 0)
				{
					return nRet;
				}
			}
			bool is2Dot2Disable = GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot2);
			List<GoodsData> list = null;
			int result;
			if (!this.EquipDict.TryGetValue(categoriy, out list))
			{
				if (categoriy == 23 && !is2Dot2Disable)
				{
					result = OrnamentManager.getInstance()._CanUsingOrnament(client, toBagIndex, list);
				}
				else if (categoriy == 9 || categoriy == 10)
				{
					result = this._CanUsingChongWu(categoriy);
				}
				else if (GoodsUtil.CanEquip(categoriy, goodsData.Site))
				{
					result = 0;
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				int nCount = list.Count;
				if (categoriy < 22 && categoriy >= 11)
				{
					if (nHandType == 2 || GameManager.MagicSwordMgr.IsMagicSword(client))
					{
						if (nCount >= 2)
						{
							return -3;
						}
						return 0;
					}
				}
				else if (categoriy == 6)
				{
					if (nCount >= 2)
					{
						return -3;
					}
					return 0;
				}
				else if (categoriy == 36)
				{
					if (nCount >= 2)
					{
						return -3;
					}
					return 0;
				}
				else if (categoriy == 9 || categoriy == 10)
				{
					int nRet = this._CanUsingChongWu(categoriy);
					if (nRet < 0)
					{
						return nRet;
					}
				}
				else if (categoriy == 23 && !is2Dot2Disable)
				{
					return OrnamentManager.getInstance()._CanUsingOrnament(client, toBagIndex, list);
				}
				result = ((list.Count < 1) ? 0 : -3);
			}
			return result;
		}

		
		public int EquipFirstPropCondition(GameClient client, SystemXmlItem systemGoods = null)
		{
			int nNeedStrength = systemGoods.GetIntValue("Strength", -1);
			int nNeedIntelligence = systemGoods.GetIntValue("Intelligence", -1);
			int nNeedDexterity = systemGoods.GetIntValue("Dexterity", -1);
			int nNeedConstitution = systemGoods.GetIntValue("Constitution", -1);
			int result;
			if (nNeedStrength > 0 && (double)nNeedStrength > RoleAlgorithm.GetStrength(client, true))
			{
				result = -1;
			}
			else if (nNeedIntelligence > 0 && (double)nNeedIntelligence > RoleAlgorithm.GetIntelligence(client, true))
			{
				result = -2;
			}
			else if (nNeedDexterity > 0 && (double)nNeedDexterity > RoleAlgorithm.GetDexterity(client, true))
			{
				result = -3;
			}
			else if (nNeedConstitution > 0 && (double)nNeedConstitution > RoleAlgorithm.GetConstitution(client, true))
			{
				result = -4;
			}
			else
			{
				result = 1;
			}
			return result;
		}

		
		public void RefreshEquip(GoodsData goodsData)
		{
			if (null != goodsData)
			{
				SystemXmlItem systemGoods = null;
				if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
				{
					int categoriy = systemGoods.GetIntValue("Categoriy", -1);
					if ((categoriy >= 0 && categoriy < 49) || categoriy == 340)
					{
						List<GoodsData> list = null;
						if (!this.EquipDict.TryGetValue(categoriy, out list))
						{
							list = new List<GoodsData>();
							this.EquipDict[categoriy] = list;
						}
						if (goodsData.Using <= 0)
						{
							list.Remove(goodsData);
							if (categoriy == 5 || (categoriy >= 11 && categoriy <= 21) || (categoriy >= 37 && categoriy <= 38))
							{
								lock (this.WeaponStrongList)
								{
									this.WeaponStrongList.Remove(goodsData);
								}
								lock (this.WeaponEquipList)
								{
									this.WeaponEquipList.Remove(goodsData);
								}
							}
							else
							{
								lock (this.EquipList)
								{
									this.EquipList.Remove(goodsData);
								}
							}
						}
						else
						{
							if (list.IndexOf(goodsData) < 0)
							{
								list.Add(goodsData);
							}
							if (categoriy == 5 || (categoriy >= 11 && categoriy <= 21) || (categoriy >= 37 && categoriy <= 38))
							{
								lock (this.WeaponStrongList)
								{
									if (this.WeaponStrongList.IndexOf(goodsData) < 0)
									{
										this.WeaponStrongList.Add(goodsData);
									}
								}
								if ((categoriy >= 11 && categoriy <= 21) || (categoriy >= 37 && categoriy <= 38))
								{
									lock (this.WeaponEquipList)
									{
										if (this.WeaponEquipList.IndexOf(goodsData) < 0)
										{
											this.WeaponEquipList.Add(goodsData);
										}
									}
								}
							}
							else
							{
								lock (this.EquipList)
								{
									if (this.EquipList.IndexOf(goodsData) < 0)
									{
										this.EquipList.Add(goodsData);
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void RefreshEquips(GameClient client)
		{
			if (client.ClientData.GoodsDataList != null && client.ClientData.GoodsDataList.Count > 0)
			{
				lock (client.ClientData.GoodsDataList)
				{
					List<GoodsData> toCorrectGoodsDataList = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.GoodsDataList.Count; i++)
					{
						if (client.ClientData.GoodsDataList[i].Using > 0)
						{
							if (this._CanUsingEquip(client, client.ClientData.GoodsDataList[i], client.ClientData.GoodsDataList[i].BagIndex, null) < 0)
							{
								toCorrectGoodsDataList.Add(client.ClientData.GoodsDataList[i]);
							}
							else
							{
								this.RefreshEquip(client.ClientData.GoodsDataList[i]);
							}
						}
					}
					for (int i = 0; i < toCorrectGoodsDataList.Count; i++)
					{
						GoodsData goodsData = toCorrectGoodsDataList[i];
						goodsData.Using = 0;
						Global.ResetBagGoodsData(client, goodsData);
					}
				}
			}
			if (client.ClientData.RebornGoodsDataList != null && client.ClientData.RebornGoodsDataList.Count > 0)
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					List<GoodsData> toCorrectGoodsDataList = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using > 0)
						{
							if (this._CanUsingEquip(client, client.ClientData.RebornGoodsDataList[i], client.ClientData.RebornGoodsDataList[i].BagIndex, null) < 0)
							{
								toCorrectGoodsDataList.Add(client.ClientData.RebornGoodsDataList[i]);
							}
							else
							{
								this.RefreshEquip(client.ClientData.RebornGoodsDataList[i]);
							}
						}
					}
					for (int i = 0; i < toCorrectGoodsDataList.Count; i++)
					{
						GoodsData goodsData = toCorrectGoodsDataList[i];
						goodsData.Using = 0;
						Global.ResetBagGoodsData(client, goodsData);
					}
				}
			}
			if (client.ClientData.FashionGoodsDataList != null && client.ClientData.FashionGoodsDataList.Count > 0)
			{
				lock (client.ClientData.FashionGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.FashionGoodsDataList.Count; i++)
					{
						if (client.ClientData.FashionGoodsDataList[i].Using > 0)
						{
							if (this._CanUsingEquip(client, client.ClientData.FashionGoodsDataList[i], client.ClientData.FashionGoodsDataList[i].BagIndex, null) >= 0)
							{
								this.RefreshEquip(client.ClientData.FashionGoodsDataList[i]);
							}
						}
					}
				}
			}
			if (client.ClientData.DamonGoodsDataList != null && client.ClientData.DamonGoodsDataList.Count > 0)
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					List<GoodsData> toCorrectGoodsDataList = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						if (client.ClientData.DamonGoodsDataList[i].Using > 0)
						{
							if (this._CanUsingEquip(client, client.ClientData.DamonGoodsDataList[i], client.ClientData.DamonGoodsDataList[i].BagIndex, null) < 0)
							{
								toCorrectGoodsDataList.Add(client.ClientData.DamonGoodsDataList[i]);
							}
							else
							{
								this.RefreshEquip(client.ClientData.DamonGoodsDataList[i]);
							}
						}
					}
					for (int i = 0; i < toCorrectGoodsDataList.Count; i++)
					{
						GoodsData goodsData = toCorrectGoodsDataList[i];
						goodsData.Using = 0;
						Global.ResetBagGoodsData(client, goodsData);
					}
				}
			}
			if (client.ClientData.OrnamentGoodsDataList != null && client.ClientData.OrnamentGoodsDataList.Count > 0)
			{
				lock (client.ClientData.OrnamentGoodsDataList)
				{
					List<GoodsData> toCorrectGoodsDataList = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.OrnamentGoodsDataList.Count; i++)
					{
						if (client.ClientData.OrnamentGoodsDataList[i].Using > 0)
						{
							if (this._CanUsingEquip(client, client.ClientData.OrnamentGoodsDataList[i], client.ClientData.OrnamentGoodsDataList[i].BagIndex, null) < 0)
							{
								toCorrectGoodsDataList.Add(client.ClientData.OrnamentGoodsDataList[i]);
							}
							else
							{
								this.RefreshEquip(client.ClientData.OrnamentGoodsDataList[i]);
							}
						}
					}
					for (int i = 0; i < toCorrectGoodsDataList.Count; i++)
					{
						GoodsData goodsData = toCorrectGoodsDataList[i];
						goodsData.Using = 0;
						Global.ResetBagGoodsData(client, goodsData);
					}
				}
			}
		}

		
		public void AttackSomebody(GameClient client)
		{
			if (this.WeaponStrongList.Count > 0)
			{
				GoodsData goodsData = null;
				lock (this.WeaponStrongList)
				{
					goodsData = this.WeaponStrongList[Global.GetRandomNumber(0, this.WeaponStrongList.Count)];
				}
				GameManager.ClientMgr.AddEquipStrong(client, goodsData, 1);
			}
		}

		
		public void InjuredSomebody(GameClient client)
		{
			if (this.EquipList.Count > 0)
			{
				GoodsData goodsData = null;
				lock (this.EquipList)
				{
					goodsData = this.EquipList[Global.GetRandomNumber(0, this.EquipList.Count)];
				}
				GameManager.ClientMgr.AddEquipStrong(client, goodsData, 1);
			}
		}

		
		public void GMAddEquipStrong(GameClient client, int val)
		{
			if (this.EquipList.Count > 0)
			{
				List<GoodsData> goodsDataList = new List<GoodsData>();
				lock (this.EquipList)
				{
					goodsDataList.AddRange(this.EquipList);
				}
				lock (this.WeaponStrongList)
				{
					goodsDataList.AddRange(this.WeaponStrongList);
				}
				foreach (GoodsData goodsData in goodsDataList)
				{
					GameManager.ClientMgr.AddEquipStrong(client, goodsData, val * 500);
				}
			}
		}

		
		public GoodsData GetGoodsDataByCategoriy(GameClient client, int categoriy)
		{
			List<GoodsData> list = null;
			GoodsData result;
			if (!this.EquipDict.TryGetValue(categoriy, out list))
			{
				result = null;
			}
			else if (list == null || list.Count <= 0)
			{
				result = null;
			}
			else
			{
				result = list[0];
			}
			return result;
		}

		
		public List<GoodsData> GetGoodsByCategoriyList(List<int> categoriyList)
		{
			List<GoodsData> result;
			if (categoriyList == null || categoriyList.Count == 0)
			{
				result = null;
			}
			else
			{
				List<GoodsData> resultList = new List<GoodsData>();
				lock (this.EquipDict)
				{
					foreach (KeyValuePair<int, List<GoodsData>> kvp in this.EquipDict)
					{
						int categoriy = kvp.Key;
						if (categoriyList.Contains(categoriy) && kvp.Value != null)
						{
							resultList.AddRange(kvp.Value);
						}
					}
				}
				result = resultList;
			}
			return result;
		}

		
		public List<GoodsData> GetGoodsByIDRange(List<Tuple<int, int>> idRange)
		{
			List<GoodsData> result;
			if (idRange == null || idRange.Count == 0)
			{
				result = null;
			}
			else
			{
				List<GoodsData> resultList = new List<GoodsData>();
				lock (this.WeaponStrongList)
				{
					this.WeaponStrongList.ForEach(delegate(GoodsData data)
					{
						if (idRange.Exists((Tuple<int, int> range) => range.Item1 <= data.GoodsID && range.Item2 >= data.GoodsID))
						{
							resultList.Add(data);
						}
					});
				}
				lock (this.EquipList)
				{
					this.EquipList.ForEach(delegate(GoodsData data)
					{
						if (idRange.Exists((Tuple<int, int> range) => range.Item1 <= data.GoodsID && range.Item2 >= data.GoodsID))
						{
							resultList.Add(data);
						}
					});
				}
				result = resultList;
			}
			return result;
		}

		
		public List<GoodsData> GetWeaponStrongList()
		{
			return this.WeaponStrongList;
		}

		
		public List<GoodsData> GetWeaponEquipList()
		{
			return this.WeaponEquipList;
		}

		
		public int GetUsingEquipAllAppendPropLeva()
		{
			int nAllAppendPropLeva = 0;
			foreach (GoodsData goodsdata in this.WeaponStrongList)
			{
				if (goodsdata != null && goodsdata.Using > 0)
				{
					nAllAppendPropLeva += goodsdata.AppendPropLev;
				}
			}
			foreach (GoodsData goodsdata in this.EquipList)
			{
				if (goodsdata != null && goodsdata.Using > 0)
				{
					int nCategories = Global.GetGoodsCatetoriy(goodsdata.GoodsID);
					if (nCategories != 9 && nCategories != 10 && !GoodsUtil.GetGoodsTypeInfo(nCategories).FashionGoods && nCategories != 8)
					{
						nAllAppendPropLeva += goodsdata.AppendPropLev;
					}
				}
			}
			return nAllAppendPropLeva;
		}

		
		public int GetUsingEquipAllForge()
		{
			int nForgeLevel = 0;
			foreach (GoodsData goodsdata in this.WeaponStrongList)
			{
				if (goodsdata != null && goodsdata.Using > 0)
				{
					nForgeLevel += goodsdata.Forge_level;
				}
			}
			foreach (GoodsData goodsdata in this.EquipList)
			{
				if (goodsdata != null && goodsdata.Using > 0)
				{
					int nCategories = Global.GetGoodsCatetoriy(goodsdata.GoodsID);
					if (nCategories != 9 && nCategories != 10 && !GoodsUtil.GetGoodsTypeInfo(nCategories).FashionGoods && nCategories != 8)
					{
						nForgeLevel += goodsdata.Forge_level;
					}
				}
			}
			return nForgeLevel;
		}

		
		public List<int> GetUsingEquipForge()
		{
			List<int> result = new List<int>();
			lock (this.WeaponStrongList)
			{
				foreach (GoodsData goodsdata in this.WeaponStrongList)
				{
					if (goodsdata != null && goodsdata.Using > 0)
					{
						result.Add(goodsdata.Forge_level);
					}
				}
			}
			lock (this.EquipList)
			{
				foreach (GoodsData goodsdata in this.EquipList)
				{
					if (goodsdata != null && goodsdata.Using > 0)
					{
						int nCategories = Global.GetGoodsCatetoriy(goodsdata.GoodsID);
						if (nCategories != 9 && nCategories != 10 && !GoodsUtil.GetGoodsTypeInfo(nCategories).FashionGoods && nCategories != 8)
						{
							result.Add(goodsdata.Forge_level);
						}
					}
				}
			}
			return result;
		}

		
		public List<int> GetUsingEquipAppend()
		{
			List<int> result = new List<int>();
			lock (this.WeaponStrongList)
			{
				foreach (GoodsData goodsdata in this.WeaponStrongList)
				{
					if (goodsdata != null && goodsdata.Using > 0)
					{
						result.Add(goodsdata.AppendPropLev);
					}
				}
			}
			lock (this.EquipList)
			{
				foreach (GoodsData goodsdata in this.EquipList)
				{
					if (goodsdata != null && goodsdata.Using > 0)
					{
						int nCategories = Global.GetGoodsCatetoriy(goodsdata.GoodsID);
						if (nCategories != 9 && nCategories != 10 && !GoodsUtil.GetGoodsTypeInfo(nCategories).FashionGoods && nCategories != 8)
						{
							result.Add(goodsdata.AppendPropLev);
						}
					}
				}
			}
			return result;
		}

		
		public List<int> GetUsingEquipExcellencePropNum()
		{
			List<int> result = new List<int>();
			lock (this.WeaponStrongList)
			{
				foreach (GoodsData goodsdata in this.WeaponStrongList)
				{
					if (goodsdata != null && goodsdata.Using > 0)
					{
						result.Add(Global.GetEquipExcellencePropNum(goodsdata));
					}
				}
			}
			lock (this.EquipList)
			{
				foreach (GoodsData goodsdata in this.EquipList)
				{
					if (goodsdata != null && goodsdata.Using > 0)
					{
						int nCategories = Global.GetGoodsCatetoriy(goodsdata.GoodsID);
						if (nCategories != 9 && nCategories != 10)
						{
							result.Add(Global.GetEquipExcellencePropNum(goodsdata));
						}
					}
				}
			}
			return result;
		}

		
		public List<int> GetUsingEquipSuit()
		{
			List<int> result = new List<int>();
			lock (this.WeaponStrongList)
			{
				foreach (GoodsData goodsdata in this.WeaponStrongList)
				{
					if (goodsdata != null && goodsdata.Using > 0)
					{
						result.Add(Global.GetEquipGoodsSuitID(goodsdata.GoodsID));
					}
				}
			}
			lock (this.EquipList)
			{
				foreach (GoodsData goodsdata in this.EquipList)
				{
					if (goodsdata != null && goodsdata.Using > 0)
					{
						int nCategories = Global.GetGoodsCatetoriy(goodsdata.GoodsID);
						if (nCategories != 9 && nCategories != 10)
						{
							result.Add(Global.GetEquipGoodsSuitID(goodsdata.GoodsID));
						}
					}
				}
			}
			return result;
		}

		
		public void RightEquipIndex(ref int index)
		{
			List<GoodsData> listGood = null;
			GoodsData Good = null;
			int Count = 0;
			lock (this.EquipDict)
			{
				int i = 11;
				while (i < 22)
				{
					if (this.EquipDict.TryGetValue(i, out listGood))
					{
						if (listGood.Count != 0)
						{
							Count += listGood.Count;
							if (Count >= 2)
							{
								return;
							}
							if (Count == 1 && listGood.Count > 0)
							{
								Good = listGood[0];
							}
						}
					}
					IL_8E:
					i++;
					continue;
					goto IL_8E;
				}
			}
			if (Count == 1 && Good != null)
			{
				if (Good.BagIndex == index && Good.BagIndex == 1)
				{
					index = 0;
				}
				else if (Good.BagIndex == index && Good.BagIndex == 0)
				{
					index = 1;
				}
			}
		}

		
		public void RightAnelIndex(ref int index, int Categoriy)
		{
			if (Categoriy == 6)
			{
				List<GoodsData> listGood = null;
				GoodsData Good = null;
				int Count = 0;
				lock (this.EquipDict)
				{
					if (this.EquipDict.TryGetValue(Categoriy, out listGood))
					{
						Count += listGood.Count;
						if (Count >= 2)
						{
							return;
						}
						if (Count == 1 && listGood.Count > 0)
						{
							Good = listGood[0];
						}
					}
				}
				if (Count == 1 && Good != null)
				{
					if (Good.BagIndex == index && Good.BagIndex == 1)
					{
						index = 0;
					}
					else if (Good.BagIndex == index && Good.BagIndex == 0)
					{
						index = 1;
					}
				}
			}
		}

		
		public void RebornRightAnelIndex(ref int index, int Categoriy)
		{
			List<GoodsData> listGood = null;
			GoodsData Good = null;
			int Count = 0;
			lock (this.EquipDict)
			{
				if (this.EquipDict.TryGetValue(Categoriy, out listGood))
				{
					Count += listGood.Count;
					if (Count >= 2)
					{
						return;
					}
					if (Count == 1 && listGood.Count > 0)
					{
						Good = listGood[0];
					}
				}
			}
			if (Count == 1 && Good != null)
			{
				if (Good.BagIndex == index && Good.BagIndex == 1)
				{
					index = 0;
				}
				else if (Good.BagIndex == index && Good.BagIndex == 0)
				{
					index = 1;
				}
			}
		}

		
		public int GetUsingEquipArchangelWeaponSuit()
		{
			int nMaxSuitID = 0;
			foreach (GoodsData goodsdata in this.WeaponStrongList)
			{
				if (goodsdata != null && goodsdata.Using > 0)
				{
					if (Data.IsDaTianShiGoods(goodsdata.GoodsID))
					{
						SystemXmlItem systemGoods = null;
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsdata.GoodsID, out systemGoods))
						{
							int nSuitID = systemGoods.GetIntValue("SuitID", -1);
							if (nMaxSuitID < nSuitID)
							{
								nMaxSuitID = nSuitID;
							}
						}
					}
				}
			}
			return nMaxSuitID;
		}

		
		private Dictionary<int, List<GoodsData>> EquipDict = new Dictionary<int, List<GoodsData>>();

		
		private GoodsData WeaponEquip = null;

		
		private List<GoodsData> WeaponStrongList = new List<GoodsData>();

		
		private List<GoodsData> WeaponEquipList = new List<GoodsData>();

		
		private List<GoodsData> EquipList = new List<GoodsData>();
	}
}
