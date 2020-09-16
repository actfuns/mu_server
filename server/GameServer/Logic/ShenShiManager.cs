using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.JingJiChang;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class ShenShiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static ShenShiManager getInstance()
		{
			return ShenShiManager.instance;
		}

		
		public bool initialize()
		{
			this.LoadFuWenHoleXml();
			this.LoadFuWenXml();
			this.LoadFuWenGodXml();
			this.LoadFuWenRandomXml();
			this.LoadHuoDongFuWenRandomXml();
			this.LoadDefaultXml();
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1870, 1, 1, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1871, 1, 1, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1872, 1, 1, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1873, 1, 1, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1874, 4, 4, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1875, 4, 4, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1876, 3, 3, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1877, 2, 2, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1878, 2, 2, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1879, 2, 2, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1880, 2, 2, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1881, 3, 3, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1883, 1, 1, ShenShiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpen(client, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1870:
					return this.ProcessShenShiMainInfoCmd(client, nID, bytes, cmdParams);
				case 1871:
					return this.ProcessGetFuWenListCmd(client, nID, bytes, cmdParams);
				case 1872:
					return this.ProcessGetFuWenTabListCmd(client, nID, bytes, cmdParams);
				case 1873:
					return this.ProcessGetShenShiListCmd(client, nID, bytes, cmdParams);
				case 1874:
					return this.ProcessModFuWenCmd(client, nID, bytes, cmdParams);
				case 1875:
					return this.ProcessModShenShiCmd(client, nID, bytes, cmdParams);
				case 1876:
					return this.ProcessModSkillCmd(client, nID, bytes, cmdParams);
				case 1877:
					return this.ProcessFuWenChouQuCmd(client, nID, bytes, cmdParams);
				case 1878:
					return this.ProcessFuWenZhiZuoCmd(client, nID, bytes, cmdParams);
				case 1879:
					return this.ProcessFuWenFenJieCmd(client, nID, bytes, cmdParams);
				case 1880:
					return this.ProcessModFuWenTabCmd(client, nID, bytes, cmdParams);
				case 1881:
					return this.ProcessModFuWenTabNameCmd(client, nID, bytes, cmdParams);
				case 1883:
					return this.ProcessFuWenTabBuyCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		
		public bool ProcessShenShiMainInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				ShenShiMainData data = new ShenShiMainData
				{
					FuWenTabId = Global.GetRoleParamsInt32FromDB(client, "10185"),
					NextFreeTime = Global.GetRoleParamsDateTimeFromDB(client, "10186")
				};
				client.sendCmd<ShenShiMainData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 获取主页面信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessGetFuWenTabListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				client.sendCmd<List<FuWenTabData>>(nID, client.ClientData.FuWenTabList, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 获取符文页错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessGetFuWenListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				client.sendCmd<List<GoodsData>>(nID, client.ClientData.FuWenGoodsDataList, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 获取符文背包信息错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessGetShenShiListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				List<int> data = Global.GetRoleParamsIntListFromDB(client, "38");
				client.sendCmd<List<int>>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 获取神识列表错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessModFuWenCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 4))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int tabId = Convert.ToInt32(cmdParams[1]);
				int grid = Convert.ToInt32(cmdParams[2]);
				int fuWenGoodsID = Convert.ToInt32(cmdParams[3]);
				int result = 0;
				string activeShenShi = "";
				if (client.ClientData.FuWenTabList == null || tabId >= client.ClientData.FuWenTabList.Count || tabId < 0 || grid < 0 || grid >= 24)
				{
					result = -1;
				}
				else
				{
					FuWenHoleItem fuWenHole = this.GetFuWenHole(grid + 1);
					if (null == fuWenHole)
					{
						result = -5;
					}
					else if (fuWenHole.OpenLevel > Global.GetUnionLevel(client, false))
					{
						result = -4;
					}
					else
					{
						if (fuWenGoodsID > 0)
						{
							GoodsData newGd = ShenShiManager.GetFuWenGoodsDataByGoodsID(client, fuWenGoodsID);
							if (newGd == null)
							{
								result = -3;
								goto IL_274;
							}
							SystemXmlItem systemGoods = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(newGd.GoodsID, out systemGoods))
							{
								result = -5;
								goto IL_274;
							}
							FuWenItem item = null;
							if (newGd == null || !this.ShenShiRunTimeData.FuWenDict.TryGetValue(newGd.GoodsID, out item))
							{
								result = -5;
								goto IL_274;
							}
							if (!this.CheckIsFuWenByGoodsID(newGd.GoodsID))
							{
								result = -7;
								goto IL_274;
							}
							if (item.Type != this.GetFuWenHole(grid + 1).Type)
							{
								result = -6;
								goto IL_274;
							}
							int equipNum = this.GetTabEquipFuWenNum(client, tabId, newGd.GoodsID);
							if (equipNum >= ShenShiManager.GetFuWenGoodsDataCountByGoodsID(client, newGd.GoodsID))
							{
								result = -8;
								goto IL_274;
							}
						}
						else
						{
							fuWenGoodsID = 0;
						}
						client.ClientData.FuWenTabList[tabId].FuWenEquipList[grid] = fuWenGoodsID;
						this.CheckShenShiProps(client, tabId, false);
						if (Global.GetRoleParamsInt32FromDB(client, "10185") == tabId)
						{
							activeShenShi = this.CheckShenShiList(client, tabId);
							this.UpdateFuWenProps(client);
						}
						if (Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[tabId], client.ServerId) < 0)
						{
							result = -9;
						}
					}
				}
				IL_274:
				if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieRiFuWen))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					result,
					cmdParams[1],
					cmdParams[2],
					cmdParams[3],
					activeShenShi,
					string.Join<int>(",", client.ClientData.FuWenTabList[tabId].ShenShiActiveList)
				});
				client.sendCmd(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 替换符文错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessModShenShiCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 4))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int tabId = Convert.ToInt32(cmdParams[1]);
				int shenShiId = Convert.ToInt32(cmdParams[2]);
				int equip = Convert.ToInt32(cmdParams[3]);
				int result = 0;
				if (client.ClientData.FuWenTabList == null || tabId >= client.ClientData.FuWenTabList.Count || tabId < 0)
				{
					result = -1;
				}
				else if (client.buffManager.IsBuffEnabled(114))
				{
					result = -19;
				}
				else
				{
					if (equip > 0)
					{
						List<int> canActiveList = this.SelectCanActiveList(this.GetActiveShenShiList(client, client.ClientData.FuWenTabList[tabId].FuWenEquipList));
						if (!canActiveList.Contains(shenShiId))
						{
							result = -10;
							goto IL_2C5;
						}
						FuWenGodItem equipGod = null;
						if (!this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(shenShiId, out equipGod))
						{
							result = -5;
							goto IL_2C5;
						}
						foreach (int id in client.ClientData.FuWenTabList[tabId].ShenShiActiveList)
						{
							FuWenGodItem godItem = null;
							if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(id, out godItem))
							{
								if (godItem.Type == equipGod.Type)
								{
									result = -11;
									break;
								}
							}
						}
						if (result != 0)
						{
							goto IL_2C5;
						}
						if (client.ClientData.FuWenTabList[tabId].ShenShiActiveList.Count >= 3)
						{
							result = -12;
							goto IL_2C5;
						}
						client.ClientData.FuWenTabList[tabId].ShenShiActiveList.Add(shenShiId);
					}
					else
					{
						if (!client.ClientData.FuWenTabList[tabId].ShenShiActiveList.Contains(shenShiId))
						{
							result = -13;
							goto IL_2C5;
						}
						client.ClientData.FuWenTabList[tabId].ShenShiActiveList.Remove(shenShiId);
					}
					if (Global.GetRoleParamsInt32FromDB(client, "10185") == tabId)
					{
						FuWenTabData tabData = client.ClientData.FuWenTabList[tabId];
						client.ClientData.ShenShiEquipData.ShenShiActiveList = tabData.ShenShiActiveList;
					}
					if (Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[tabId], client.ServerId) < 0)
					{
						result = -9;
					}
				}
				IL_2C5:
				string data = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					result,
					cmdParams[1],
					cmdParams[2],
					cmdParams[3]
				});
				client.sendCmd(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 更改神识错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessModSkillCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int tabId = Convert.ToInt32(cmdParams[1]);
				int skillId = Convert.ToInt32(cmdParams[2]);
				int result = 0;
				if (0 == skillId)
				{
					result = -22;
				}
				else if (client.ClientData.FuWenTabList == null || tabId >= client.ClientData.FuWenTabList.Count || tabId < 0)
				{
					result = -1;
				}
				else if (client.buffManager.IsBuffEnabled(114))
				{
					result = -19;
				}
				else
				{
					if (skillId > 0)
					{
						if (!SpriteAttack.CanUseMaigc(client, skillId))
						{
							result = -14;
							goto IL_1CA;
						}
						if (!this.FuWenMagicList.Contains(skillId))
						{
							result = -14;
							goto IL_1CA;
						}
						client.ClientData.FuWenTabList[tabId].SkillEquip = skillId;
					}
					else
					{
						if (client.ClientData.FuWenTabList[tabId].SkillEquip <= 0)
						{
							result = -15;
							goto IL_1CA;
						}
						client.ClientData.FuWenTabList[tabId].SkillEquip = 0;
					}
					if (Global.GetRoleParamsInt32FromDB(client, "10185") == tabId)
					{
						FuWenTabData tabData = client.ClientData.FuWenTabList[tabId];
						client.ClientData.ShenShiEquipData.SkillEquip = tabData.SkillEquip;
					}
					if (Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[tabId], client.ServerId) < 0)
					{
						result = -9;
					}
				}
				IL_1CA:
				string data = string.Format("{0}:{1}:{2}", result, cmdParams[1], cmdParams[2]);
				client.sendCmd(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 替换技能错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessFuWenChouQuCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int chouJiangType = Convert.ToInt32(cmdParams[1]);
				DateTime now = TimeUtil.NowDateTime();
				FuWenChouQuResult data = new FuWenChouQuResult
				{
					Result = 0,
					ChouQuCount = chouJiangType
				};
				int num = (chouJiangType == 1) ? 1 : 10;
				if (!ShenShiManager.CanAddGoodsNum(client, num))
				{
					data.Result = -16;
				}
				else
				{
					List<int> chouQuList = new List<int>();
					if (chouJiangType == 1)
					{
						int gdId = 0;
						int random = Global.GetRandomNumber(1, 100001);
						lock (this.ShenShiRunTimeData.Mutex)
						{
							foreach (FuWenRandomItem one in this.GetRandomList(0))
							{
								if (random >= one.BeginNum && random <= one.EndNum)
								{
									if (this.CheckIsFuWenByGoodsID(one.GoodsID))
									{
										gdId = one.GoodsID;
										break;
									}
								}
							}
						}
						if (gdId == 0)
						{
							data.Result = -5;
							goto IL_553;
						}
						DateTime nextFreeTime = Global.GetRoleParamsDateTimeFromDB(client, "10186");
						if (nextFreeTime < now)
						{
							nextFreeTime = now.AddSeconds((double)this.FuWenFreeTime);
							Global.SaveRoleParamsDateTimeToDB(client, "10186", nextFreeTime, true);
						}
						else if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -this.FuWenChouQuCost, "神识符文抽取_钻石(改幸运之星)", false, DaiBiSySType.FuWenChouQu))
						{
							data.Result = -17;
							goto IL_553;
						}
						chouQuList.Add(gdId);
						data.GoodsList = string.Format("{0},{1},{2},{3},{4},{5},{6}", new object[]
						{
							gdId,
							1,
							0,
							0,
							0,
							0,
							0
						});
						data.FreeTime = nextFreeTime;
					}
					else
					{
						int gdId = 0;
						for (int i = 0; i < num - 1; i++)
						{
							int random = Global.GetRandomNumber(1, 100001);
							lock (this.ShenShiRunTimeData.Mutex)
							{
								foreach (FuWenRandomItem one in this.GetRandomList(0))
								{
									if (random >= one.BeginNum && random <= one.EndNum)
									{
										if (this.CheckIsFuWenByGoodsID(one.GoodsID))
										{
											chouQuList.Add(one.GoodsID);
											break;
										}
									}
								}
							}
						}
						int random2 = Global.GetRandomNumber(1, 100001);
						lock (this.ShenShiRunTimeData.Mutex)
						{
							foreach (FuWenRandomItem one in this.GetRandomList(1))
							{
								if (random2 >= one.BeginNum && random2 <= one.EndNum)
								{
									if (this.CheckIsFuWenByGoodsID(one.GoodsID))
									{
										chouQuList.Add(one.GoodsID);
										break;
									}
								}
							}
						}
						if (chouQuList.Count < num)
						{
							data.Result = -5;
							goto IL_553;
						}
						int lastVal = chouQuList[num - 1];
						random2 = Global.GetRandomNumber(0, num);
						chouQuList[num - 1] = chouQuList[random2];
						chouQuList[random2] = lastVal;
						if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -this.FuWenChouQuCost_10, "神识符文抽取10_钻石(改幸运之星)", false, DaiBiSySType.FuWenChouQu))
						{
							data.Result = -17;
							goto IL_553;
						}
						data.GoodsList = string.Join("|", chouQuList.ConvertAll<string>((int _gd) => string.Format("{0},{1},{2},{3},{4},{5},{6}", new object[]
						{
							_gd,
							1,
							0,
							0,
							0,
							0,
							0
						})));
						data.FreeTime = Global.GetRoleParamsDateTimeFromDB(client, "10186");
					}
					foreach (int goodsID in chouQuList)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsID, 1, 0, "", 0, 1, 11000, "", true, 1, "神识符文抽取", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
					}
				}
				IL_553:
				client.sendCmd<FuWenChouQuResult>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 抽取符文错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessFuWenZhiZuoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int goodsID = Convert.ToInt32(cmdParams[1]);
				int result = 0;
				if (!this.CheckIsFuWenByGoodsID(goodsID))
				{
					result = -7;
				}
				else if (!ShenShiManager.CanAddGoodsNum(client, 1))
				{
					result = -16;
				}
				else if (ShenShiManager.GetFuWenGoodsDataCountByGoodsID(client, goodsID) >= 8)
				{
					result = -8;
				}
				else
				{
					FuWenItem fuWen = null;
					if (!this.ShenShiRunTimeData.FuWenDict.TryGetValue(goodsID, out fuWen))
					{
						result = -7;
					}
					else if (fuWen.Level >= 7 && GameManager.systemParamsList.GetParamValueIntByName("FuWenSevenOpen", -1) != 1L)
					{
						result = -5;
					}
					else if (fuWen.PayNum > client.ClientData.FuWenZhiChen)
					{
						result = -18;
					}
					else
					{
						GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(client, -fuWen.PayNum, "制作神识符文消耗", true, true, false);
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsID, 1, 0, "", 0, 1, 11000, "", true, 1, "神识符文制作", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
					}
				}
				string data = string.Format("{0}:{1}", result, string.Format("{0},{1},{2},{3},{4},{5},{6}", new object[]
				{
					goodsID,
					1,
					0,
					0,
					0,
					0,
					0
				}));
				client.sendCmd(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 制作符文错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessFuWenFenJieCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				List<int> fenJieList = Array.ConvertAll<string, int>(cmdParams[1].Split(new char[]
				{
					','
				}), (string x) => Convert.ToInt32(x)).ToList<int>();
				int result = 0;
				int fuWenFenJieVal = 0;
				for (int i = 0; i < fenJieList.Count; i++)
				{
					int goodsId = fenJieList[i++];
					if (fenJieList.Count == i)
					{
						result = -3;
						break;
					}
					int goodsNum = fenJieList[i];
					if (goodsNum > 0)
					{
						List<GoodsData> goodsDataList = this.GetFuWenGoodsDataListByGoodsID(client, goodsId);
						if (goodsDataList == null || goodsDataList.Count < 1)
						{
							result = -3;
							break;
						}
						if (!this.CheckIsFuWenByGoodsID(goodsId))
						{
							result = -7;
							break;
						}
						FuWenItem fuWen = null;
						if (!this.ShenShiRunTimeData.FuWenDict.TryGetValue(goodsId, out fuWen))
						{
							result = -7;
							break;
						}
						for (int j = 0; j < goodsDataList.Count; j++)
						{
							GoodsData goodsData = goodsDataList[j];
							int subNum = (goodsNum > goodsData.GCount) ? goodsData.GCount : goodsNum;
							if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData, subNum, false, false))
							{
								result = -9;
								break;
							}
							goodsNum -= subNum;
							if (goodsNum <= 0 || result != 0)
							{
								break;
							}
						}
						if (goodsNum > 0)
						{
							result = -3;
							break;
						}
						if (result == 0)
						{
							fuWenFenJieVal += fuWen.SendNum * fenJieList[i];
						}
					}
				}
				GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(client, fuWenFenJieVal, "分解符文获得", true, true, false);
				for (int tabId = 0; tabId < client.ClientData.FuWenTabList.Count; tabId++)
				{
					this.UpdateFuWenTabList(client, tabId);
				}
				if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieRiFuWen))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				client.sendCmd(nID, string.Format("{0}", result), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 分解符文错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessModFuWenTabCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int changeTab = Convert.ToInt32(cmdParams[1]);
				int useFuWenTab = Global.GetRoleParamsInt32FromDB(client, "10185");
				int result = 0;
				string activeShenShi = "";
				if (client.ClientData.FuWenTabList == null || changeTab >= client.ClientData.FuWenTabList.Count || changeTab < 0)
				{
					result = -1;
				}
				else if (client.buffManager.IsBuffEnabled(114))
				{
					result = -19;
				}
				else if (useFuWenTab != changeTab)
				{
					Global.SaveRoleParamsInt32ValueToDB(client, "10185", changeTab, true);
					activeShenShi = this.CheckShenShiList(client, changeTab);
					this.CheckShenShiProps(client, changeTab, false);
					this.UpdateFuWenProps(client);
					if (Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[changeTab], client.ServerId) < 0)
					{
						result = -9;
					}
				}
				if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieRiFuWen))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					result,
					cmdParams[1],
					activeShenShi,
					string.Join<int>(",", client.ClientData.FuWenTabList[changeTab].ShenShiActiveList)
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 更改启用符文页错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessModFuWenTabNameCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int tabId = Convert.ToInt32(cmdParams[1]);
				string tabName = cmdParams[2];
				int result = 0;
				if (client.ClientData.FuWenTabList == null || tabId >= client.ClientData.FuWenTabList.Count || tabId < 0)
				{
					result = -1;
				}
				else if (string.IsNullOrEmpty(tabName) || NameServerNamager.CheckInvalidCharacters(tabName, false) <= 0 || tabName.Length < 1 || tabName.Length > 5)
				{
					result = -20;
				}
				else if (client.buffManager.IsBuffEnabled(114))
				{
					result = -19;
				}
				else
				{
					client.ClientData.FuWenTabList[tabId].Name = tabName;
					if (Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[tabId], client.ServerId) < 0)
					{
						result = -9;
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, cmdParams[1], cmdParams[2]), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 更改符文页名称错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public bool ProcessFuWenTabBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int result = 0;
				FuWenTabData data = null;
				int fuWenTabId = client.ClientData.FuWenTabList.Count;
				if (fuWenTabId >= this.InitFuWenTabNum + this.FuWenTabBuyCost.Count)
				{
					result = -21;
				}
				else
				{
					int zuanShiCost = this.FuWenTabBuyCost[fuWenTabId - this.InitFuWenTabNum];
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, zuanShiCost, "神识符文页购买_钻石", true, true, false, DaiBiSySType.None))
					{
						result = -17;
					}
					else
					{
						data = ShenShiManager.AddRoleFuWenTab(client.ClientData.RoleID, fuWenTabId, client.ServerId);
						if (data != null)
						{
							data.ShenShiActiveList = new List<int>();
							client.ClientData.FuWenTabList.Add(data);
						}
						else
						{
							result = -9;
							GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, zuanShiCost, "神识符文页购买回滚_钻石", ActivityTypes.None, "");
						}
					}
				}
				if (null == data)
				{
					data = new FuWenTabData
					{
						TabID = result
					};
				}
				client.sendCmd<FuWenTabData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 处理购买符文页命令。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public void LoadFuWenHoleXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(ShenShiConsts.FuWenHole);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					Dictionary<int, FuWenHoleItem> fuWenHoleDict = new Dictionary<int, FuWenHoleItem>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "HoleID", "0"));
							int[] level = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xmlItem, "OpenLevel", "").Split(new char[]
							{
								'|'
							}), (string x) => Convert.ToInt32(x));
							if (level.Length < 2)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("加载xml配置文件:{0}, 错误。", fileName), null, true);
							}
							else
							{
								fuWenHoleDict[id] = new FuWenHoleItem
								{
									HoleID = id,
									Type = this.ToType(Global.GetDefAttributeStr(xmlItem, "Type", "")),
									OpenLevel = Global.GetUnionLevel(level[0], level[1], false)
								};
							}
						}
					}
					lock (this.ShenShiRunTimeData.Mutex)
					{
						this.ShenShiRunTimeData.FuWenHoleDict = fuWenHoleDict;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		
		public void LoadFuWenXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(ShenShiConsts.FuWen);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					Dictionary<int, FuWenItem> fuWenDict = new Dictionary<int, FuWenItem>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int goodsId = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0"));
							fuWenDict[goodsId] = new FuWenItem
							{
								GoodsId = goodsId,
								Type = this.ToType(Global.GetDefAttributeStr(xmlItem, "Type", "0")),
								Level = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Level", "0")),
								Blue = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Blue", "0")),
								Red = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Red", "0")),
								Green = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Green", "0")),
								PayNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "PayNum", "0")),
								SendNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SendNum", "0"))
							};
						}
					}
					lock (this.ShenShiRunTimeData.Mutex)
					{
						this.ShenShiRunTimeData.FuWenDict = fuWenDict;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		
		public void LoadFuWenGodXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(ShenShiConsts.FuWenGod);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					Dictionary<int, FuWenGodItem> fuWenGodDict = new Dictionary<int, FuWenGodItem>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int type = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Type", "0"));
							int level = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Level", "0"));
							int key = 100 * type + level;
							fuWenGodDict[key] = new FuWenGodItem
							{
								GodId = key,
								Type = type,
								Level = level,
								NeedBlue = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedBlue", "0")),
								NeedRed = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedRed", "0")),
								NeedGreen = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "NeedGreen", "0")),
								MagicItemList = GameManager.SystemMagicActionMgr.ParseActionsInterface(Global.GetDefAttributeStr(xmlItem, "ShenShiScript", ""))
							};
						}
					}
					lock (this.ShenShiRunTimeData.Mutex)
					{
						this.ShenShiRunTimeData.FuWenGodDict = fuWenGodDict;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		
		public void LoadFuWenRandomXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(ShenShiConsts.FuWenRandom);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					List<FuWenRandomItem> fuWenRandomList = new List<FuWenRandomItem>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							fuWenRandomList.Add(new FuWenRandomItem
							{
								ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
								GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0")),
								BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0")),
								EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndNum", "0"))
							});
						}
					}
					fileName = Global.GameResPath(ShenShiConsts.FuWenPayRandom);
					xml = CheckHelper.LoadXml(fileName, true);
					if (null != xml)
					{
						List<FuWenRandomItem> fuWenPayRandomList = new List<FuWenRandomItem>();
						nodes = xml.Elements();
						foreach (XElement xmlItem in nodes)
						{
							if (xmlItem != null)
							{
								fuWenPayRandomList.Add(new FuWenRandomItem
								{
									ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
									GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0")),
									BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0")),
									EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndNum", "0"))
								});
							}
						}
						lock (this.ShenShiRunTimeData.Mutex)
						{
							this.ShenShiRunTimeData.FuWenRandomList = fuWenRandomList;
							this.ShenShiRunTimeData.FuWenPayRandomList = fuWenPayRandomList;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		
		public void ReloadConfig()
		{
			this.LoadHuoDongFuWenRandomXml();
		}

		
		public void LoadHuoDongFuWenRandomXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(ShenShiConsts.HuoDongFuWenRandom);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					List<FuWenRandomItem> fuWenRandomList = new List<FuWenRandomItem>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							fuWenRandomList.Add(new FuWenRandomItem
							{
								ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
								GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0")),
								BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0")),
								EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndNum", "0"))
							});
						}
					}
					fileName = Global.GameResPath(ShenShiConsts.HuoDongFuWenPayRandom);
					xml = CheckHelper.LoadXml(fileName, true);
					if (null != xml)
					{
						List<FuWenRandomItem> fuWenPayRandomList = new List<FuWenRandomItem>();
						nodes = xml.Elements();
						foreach (XElement xmlItem in nodes)
						{
							if (xmlItem != null)
							{
								fuWenPayRandomList.Add(new FuWenRandomItem
								{
									ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0")),
									GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0")),
									BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0")),
									EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EndNum", "0"))
								});
							}
						}
						lock (this.ShenShiRunTimeData.Mutex)
						{
							this.ShenShiRunTimeData.HuoDongFuWenRandomList = fuWenRandomList;
							this.ShenShiRunTimeData.HuoDongFuWenPayRandomList = fuWenPayRandomList;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		
		public void LoadDefaultXml()
		{
			try
			{
				this.FuWenFreeTime = (int)GameManager.systemParamsList.GetParamValueIntByName("FuWenFreeRandom", -1);
				string[] costArr = GameManager.systemParamsList.GetParamValueByName("FuWenPay").Split(new char[]
				{
					','
				});
				this.FuWenChouQuCost = Convert.ToInt32(costArr[0]);
				this.FuWenChouQuCost_10 = Convert.ToInt32(costArr[1]);
				Dictionary<int, int> parentMagicCode = new Dictionary<int, int>();
				foreach (string one in GameManager.systemParamsList.GetParamValueByName("FuWenMagic").Split(new char[]
				{
					','
				}))
				{
					int magicCode = Convert.ToInt32(one);
					SystemXmlItem systemMagic = null;
					if (GameManager.SystemMagicQuickMgr.MagicItemsDict.TryGetValue(magicCode, out systemMagic))
					{
						int nextMagicID = systemMagic.GetIntValue("NextMagicID", -1);
						if (nextMagicID > 0)
						{
							parentMagicCode.Add(nextMagicID, magicCode);
						}
						this.FuWenMagicList.Add(magicCode);
					}
				}
				lock (this.ShenShiRunTimeData.Mutex)
				{
					this.ShenShiRunTimeData.ParentMagicCode = parentMagicCode;
				}
				string[] fuWenArr = GameManager.systemParamsList.GetParamValueByName("FuWenList").Split(new char[]
				{
					','
				});
				if (fuWenArr.Length < 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("没配置符文页数量", new object[0]), null, true);
				}
				else
				{
					this.InitFuWenTabNum = Convert.ToInt32(fuWenArr[0]);
					for (int i = 1; i < fuWenArr.Length; i++)
					{
						this.FuWenTabBuyCost.Add(Convert.ToInt32(fuWenArr[i]));
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", "SystemParams.xml"), ex, true);
			}
		}

		
		public List<FuWenRandomItem> GetRandomList(int type)
		{
			List<FuWenRandomItem> randomList = this.ShenShiRunTimeData.FuWenRandomList;
			if (HuodongCachingMgr.GetJieriFuLiActivity().IsOpened(EJieRiFuLiType.FuWenKuangHuan))
			{
				if (type == 1)
				{
					randomList = this.ShenShiRunTimeData.HuoDongFuWenPayRandomList;
				}
				else
				{
					randomList = this.ShenShiRunTimeData.HuoDongFuWenRandomList;
				}
			}
			else if (type == 1)
			{
				randomList = this.ShenShiRunTimeData.FuWenPayRandomList;
			}
			else
			{
				randomList = this.ShenShiRunTimeData.FuWenRandomList;
			}
			return randomList;
		}

		
		public static FuWenTabData AddRoleFuWenTab(int rid, int tabID, int serverID)
		{
			FuWenTabData newTab = new FuWenTabData
			{
				TabID = tabID,
				Name = string.Format(GLang.GetLang(2621, new object[0]), tabID + 1),
				FuWenEquipList = new List<int>(new int[24]),
				SkillEquip = 0,
				OwnerID = rid
			};
			Global.sendToDB<int, FuWenTabData>(20315, newTab, serverID);
			return newTab;
		}

		
		public FuWenHoleItem GetFuWenHole(int fuWenId)
		{
			FuWenHoleItem ret = null;
			try
			{
				lock (this.ShenShiRunTimeData.Mutex)
				{
					if (null == this.ShenShiRunTimeData.FuWenHoleDict)
					{
						return null;
					}
					this.ShenShiRunTimeData.FuWenHoleDict.TryGetValue(fuWenId, out ret);
					return ret;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShiManager :: 获取符文页数据, 失败。FuWenID:{0}, ex:{1}", fuWenId, ex.Message), null, true);
			}
			return null;
		}

		
		public void InitRoleShenShiData(GameClient client)
		{
			if (null == client.ClientData.FuWenGoodsDataList)
			{
				client.ClientData.FuWenGoodsDataList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 11000), client.ServerId);
			}
			if (this.IsGongNengOpen(client, false))
			{
				if (null == client.ClientData.FuWenTabList)
				{
					client.ClientData.FuWenTabList = new List<FuWenTabData>();
				}
				for (int i = 0; i < client.ClientData.FuWenTabList.Count; i++)
				{
					if (client.ClientData.FuWenTabList[i].ShenShiActiveList == null)
					{
						client.ClientData.FuWenTabList[i].ShenShiActiveList = new List<int>();
					}
				}
				for (int i = client.ClientData.FuWenTabList.Count; i < this.InitFuWenTabNum; i++)
				{
					FuWenTabData newTab = ShenShiManager.AddRoleFuWenTab(client.ClientData.RoleID, i, client.ServerId);
					if (newTab == null)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("初始化角色符文数据, 失败。rid:{0} tabID:{1}", client.ClientData.RoleID, i), null, true);
						break;
					}
					newTab.ShenShiActiveList = new List<int>();
					client.ClientData.FuWenTabList.Add(newTab);
				}
				if (null == client.ClientData.ShenShiEquipData)
				{
					client.ClientData.ShenShiEquipData = new SkillEquipData
					{
						ShenShiActiveList = new List<int>()
					};
				}
				this.UpdateFuWenTabList(client, Global.GetRoleParamsInt32FromDB(client, "10185"));
				client.ClientData.FuWenZhiChen = Global.GetRoleParamsInt32FromDB(client, "10187");
				int tabId = Global.GetRoleParamsInt32FromDB(client, "10185");
				client.sendCmd<List<GoodsData>>(1871, client.ClientData.FuWenGoodsDataList, false);
				client.sendCmd<List<FuWenTabData>>(1872, client.ClientData.FuWenTabList, false);
			}
		}

		
		public int ToType(string type)
		{
			if (type != null)
			{
				if (type == "Blue")
				{
					return 1;
				}
				if (type == "Red")
				{
					return 2;
				}
				if (type == "Green")
				{
					return 3;
				}
			}
			LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 类型配置错误。type:{0}", type), null, true);
			return 0;
		}

		
		public static GoodsData AddFuWenGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
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
				Binding = 1,
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
			if (null == client.ClientData.FuWenGoodsDataList)
			{
				client.ClientData.FuWenGoodsDataList = new List<GoodsData>();
			}
			lock (client.ClientData.FuWenGoodsDataList)
			{
				client.ClientData.FuWenGoodsDataList.Add(gd);
			}
			return gd;
		}

		
		public static int GetIdleSlotOfFuWenBagGoods(GameClient client)
		{
			int idelPos = 0;
			int result;
			if (null == client.ClientData.FuWenGoodsDataList)
			{
				result = idelPos;
			}
			else
			{
				List<int> usedBagIndex = new List<int>();
				for (int i = 0; i < client.ClientData.FuWenGoodsDataList.Count; i++)
				{
					if (usedBagIndex.IndexOf(client.ClientData.FuWenGoodsDataList[i].BagIndex) < 0)
					{
						usedBagIndex.Add(client.ClientData.FuWenGoodsDataList[i].BagIndex);
					}
				}
				for (int j = 0; j < ShenShiManager.GetMaxFuWenCount(); j++)
				{
					if (usedBagIndex.IndexOf(j) < 0)
					{
						idelPos = j;
						break;
					}
				}
				result = idelPos;
			}
			return result;
		}

		
		public static bool CanAddGoodsNum(GameClient client, int num)
		{
			return client != null && num > 0;
		}

		
		public static int GetMaxFuWenCount()
		{
			return int.MaxValue;
		}

		
		public bool IsGongNengOpen(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot6) && GlobalNew.IsGongNengOpened(client, GongNengIDs.ShenShiFuWen, hint);
		}

		
		public static GoodsData GetGoodsByID(GameClient client, int goodsID, int bingding, string startTime, string endTime, ref int startIndex)
		{
			GoodsData result;
			if (null == client)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				lock (client.ClientData.FuWenGoodsDataList)
				{
					if (startIndex >= client.ClientData.FuWenGoodsDataList.Count)
					{
						return null;
					}
					for (int i = startIndex; i < client.ClientData.FuWenGoodsDataList.Count; i++)
					{
						GoodsData goods = client.ClientData.FuWenGoodsDataList[i];
						if (goods.GoodsID == goodsID && goods.Binding == bingding && Global.DateTimeEqual(goods.Endtime, endTime) && Global.DateTimeEqual(goods.Starttime, startTime))
						{
							startIndex = i + 1;
							return goods;
						}
					}
				}
				result = null;
			}
			return result;
		}

		
		public static void UpdateFuWenGoodsData(GameClient client, GoodsData goodsData)
		{
			if (client.ClientData.FuWenGoodsDataList != null && null != goodsData)
			{
				lock (client.ClientData.FuWenGoodsDataList)
				{
					if (goodsData.GCount == 0)
					{
						client.ClientData.FuWenGoodsDataList.Remove(goodsData);
					}
					else
					{
						for (int i = 0; i < client.ClientData.FuWenGoodsDataList.Count; i++)
						{
							if (client.ClientData.FuWenGoodsDataList[i].GoodsID == goodsData.GoodsID)
							{
								client.ClientData.FuWenGoodsDataList[i] = goodsData;
							}
						}
					}
				}
			}
		}

		
		public static GoodsData GetFuWenGoodsDataByGoodsID(GameClient client, int goodsID)
		{
			GoodsData result;
			if (client.ClientData.FuWenGoodsDataList == null || goodsID <= 0)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.FuWenGoodsDataList)
				{
					result = client.ClientData.FuWenGoodsDataList.Find((GoodsData _g) => _g.GoodsID == goodsID);
				}
			}
			return result;
		}

		
		public List<GoodsData> GetFuWenGoodsDataListByGoodsID(GameClient client, int goodsID)
		{
			List<GoodsData> ret = new List<GoodsData>();
			List<GoodsData> result;
			if (client.ClientData.FuWenGoodsDataList == null || goodsID <= 0)
			{
				result = ret;
			}
			else
			{
				lock (client.ClientData.FuWenGoodsDataList)
				{
					foreach (GoodsData good in client.ClientData.FuWenGoodsDataList)
					{
						if (good.GoodsID == goodsID)
						{
							ret.Add(good);
						}
					}
				}
				result = ret;
			}
			return result;
		}

		
		public static int GetFuWenGoodsDataCountByGoodsID(GameClient client, int goodsID)
		{
			int result;
			if (client.ClientData.FuWenGoodsDataList == null || goodsID <= 0)
			{
				result = 0;
			}
			else
			{
				lock (client.ClientData.FuWenGoodsDataList)
				{
					int sum = 0;
					foreach (GoodsData good in client.ClientData.FuWenGoodsDataList)
					{
						if (good.GoodsID == goodsID)
						{
							sum += good.GCount;
						}
					}
					result = sum;
				}
			}
			return result;
		}

		
		public static GoodsData GetFuWenGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.FuWenGoodsDataList)
			{
				result = null;
			}
			else if (id <= 0)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.FuWenGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.FuWenGoodsDataList.Count; i++)
					{
						if (client.ClientData.FuWenGoodsDataList[i].Id == id)
						{
							return client.ClientData.FuWenGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		
		public string CheckShenShiList(GameClient client, int tabId)
		{
			string ret = "";
			try
			{
				if (tabId != Global.GetRoleParamsInt32FromDB(client, "10185"))
				{
					return ":";
				}
				FuWenTabData tabData = client.ClientData.FuWenTabList[tabId];
				List<int> oldShenShiList = Global.GetRoleParamsIntListFromDB(client, "38");
				List<int> addShenShiList = this.GetActiveShenShiList(client, tabData.FuWenEquipList);
				addShenShiList.AddRange(oldShenShiList);
				List<int> newShenShiList = this.SelectCanActiveList(addShenShiList);
				oldShenShiList = newShenShiList.Except(oldShenShiList).ToList<int>();
				if (oldShenShiList.Count > 0)
				{
					int leftLogCount = newShenShiList.Count - 24;
					if (leftLogCount > 0)
					{
						newShenShiList.RemoveRange(newShenShiList.Count - 1, leftLogCount);
					}
					Global.SaveRoleParamsIntListToDB(client, newShenShiList, "38", true);
					ret = string.Join<int>(",", oldShenShiList);
				}
				return ret;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 检查是否有可激活神识错误。rid:{0} tabId:{1} ex:{2}", client.ClientData.RoleID, tabId, ex.Message), null, true);
			}
			return ret;
		}

		
		public List<int> GetActiveShenShiList(GameClient client, List<int> fuWenList)
		{
			List<int> ret = new List<int>();
			List<int> result;
			if (fuWenList == null || fuWenList.Count < 0)
			{
				result = ret;
			}
			else
			{
				try
				{
					int blue = 0;
					int red = 0;
					int green = 0;
					for (int i = 0; i < fuWenList.Count; i++)
					{
						if (fuWenList[i] > 0)
						{
							FuWenItem item = null;
							if (this.ShenShiRunTimeData.FuWenDict.TryGetValue(fuWenList[i], out item))
							{
								if (item.Type == this.GetFuWenHole(i + 1).Type)
								{
									blue += item.Blue;
									red += item.Red;
									green += item.Green;
								}
							}
						}
					}
					ret = this.ShenShiRunTimeData.FuWenGodDict.Values.ToList<FuWenGodItem>().FindAll((FuWenGodItem _g) => _g.NeedBlue <= blue && _g.NeedRed <= red && _g.NeedGreen <= green).ConvertAll<int>((FuWenGodItem _g) => _g.GodId);
					return ret;
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 获取可激活神识列错误。 ex:{0}", ex.Message), null, true);
				}
				result = ret;
			}
			return result;
		}

		
		public List<int> SelectCanActiveList(List<int> shenShiList)
		{
			List<int> ret = new List<int>();
			Dictionary<int, FuWenGodItem> typeShenShiDict = new Dictionary<int, FuWenGodItem>();
			foreach (int shenShiID in shenShiList)
			{
				FuWenGodItem godItem = null;
				if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(shenShiID, out godItem))
				{
					FuWenGodItem tmp = null;
					if (!typeShenShiDict.TryGetValue(godItem.Type, out tmp))
					{
						typeShenShiDict[godItem.Type] = godItem;
					}
					else if (tmp.Level < godItem.Level)
					{
						typeShenShiDict[godItem.Type] = godItem;
					}
				}
			}
			return typeShenShiDict.Values.ToList<FuWenGodItem>().ConvertAll<int>((FuWenGodItem _g) => _g.GodId);
		}

		
		public int GetTabEquipFuWenNum(GameClient client, int tabID, int goodsID)
		{
			int result;
			if (client.ClientData.FuWenTabList == null || tabID >= client.ClientData.FuWenTabList.Count)
			{
				result = 0;
			}
			else
			{
				result = client.ClientData.FuWenTabList[tabID].FuWenEquipList.FindAll((int _g) => _g == goodsID).Count;
			}
			return result;
		}

		
		public bool CheckIsFuWenByGoodsID(int goodsID)
		{
			SystemXmlItem systemGoods = null;
			return GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods) && systemGoods.GetIntValue("Categoriy", -1) == 940;
		}

		
		public void UpdateFuWenTabList(GameClient client, int tabId)
		{
			lock (client.ClientData.FuWenTabList)
			{
				if (tabId < client.ClientData.FuWenTabList.Count)
				{
					Dictionary<int, int> fuWenCountDict = new Dictionary<int, int>();
					Dictionary<int, int> fuWenEquipCountDict = new Dictionary<int, int>();
					bool writeDB = false;
					for (int i = 0; i < client.ClientData.FuWenTabList[tabId].FuWenEquipList.Count; i++)
					{
						int one = client.ClientData.FuWenTabList[tabId].FuWenEquipList[i];
						if (0 != one)
						{
							int fuWenCount = 0;
							int fuWenEquipCount;
							if (fuWenEquipCountDict.ContainsKey(one))
							{
								Dictionary<int, int> dictionary;
								int key;
								fuWenEquipCount = ((dictionary = fuWenEquipCountDict)[key = one] = dictionary[key] + 1);
							}
							else
							{
								fuWenEquipCount = (fuWenEquipCountDict[one] = 1);
							}
							if (!fuWenCountDict.TryGetValue(one, out fuWenCount))
							{
								fuWenCount = (fuWenCountDict[one] = ShenShiManager.GetFuWenGoodsDataCountByGoodsID(client, one));
							}
							if (fuWenCount < fuWenEquipCount)
							{
								client.ClientData.FuWenTabList[tabId].FuWenEquipList[i] = 0;
								writeDB = true;
							}
						}
					}
					this.CheckShenShiProps(client, tabId, true);
					if (tabId == Global.GetRoleParamsInt32FromDB(client, "10185"))
					{
						this.UpdateFuWenProps(client);
					}
					if (writeDB)
					{
						Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[tabId], client.ServerId);
					}
				}
			}
		}

		
		public int GetCurrentTabTotalLevel(GameClient client)
		{
			int totalLev = 0;
			int tabId = Global.GetRoleParamsInt32FromDB(client, "10185");
			int result;
			if (client.ClientData.FuWenTabList == null || client.ClientData.FuWenTabList.Count <= tabId)
			{
				result = totalLev;
			}
			else
			{
				FuWenTabData tabData = client.ClientData.FuWenTabList[tabId];
				foreach (int goodsID in tabData.FuWenEquipList)
				{
					FuWenItem fuWen = null;
					if (this.ShenShiRunTimeData.FuWenDict.TryGetValue(goodsID, out fuWen))
					{
						totalLev += fuWen.Level;
					}
				}
				result = totalLev;
			}
			return result;
		}

		
		public void UpdateFuWenProps(GameClient client)
		{
			int tabId = Global.GetRoleParamsInt32FromDB(client, "10185");
			if (client.ClientData.FuWenTabList != null && client.ClientData.FuWenTabList.Count > tabId)
			{
				FuWenTabData tabData = client.ClientData.FuWenTabList[tabId];
				double[] _ExtProps = new double[177];
				foreach (int goodsID in tabData.FuWenEquipList)
				{
					if (goodsID > 0)
					{
						GoodsData goodsData = ShenShiManager.GetFuWenGoodsDataByGoodsID(client, goodsID);
						if (null != goodsData)
						{
							SystemXmlItem systemGoods = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
							{
								EquipPropItem item = GameManager.EquipPropsMgr.FindEquipPropItem(goodsID);
								if (null != item)
								{
									for (int i = 0; i < 177; i++)
									{
										_ExtProps[i] += Global.GetEquipExtPropsItemVal(client, goodsData, item, i, systemGoods);
									}
								}
							}
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.ShenShiFuWen,
					_ExtProps
				});
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.RecalcProps,
					DelayExecProcIds.NotifyRefreshProps
				});
			}
		}

		
		public void CheckShenShiProps(GameClient client, int tabId, bool writeDB = false)
		{
			try
			{
				if (client.ClientData.FuWenTabList != null && client.ClientData.FuWenTabList.Count > tabId)
				{
					FuWenTabData tabData = client.ClientData.FuWenTabList[tabId];
					List<int> newShenShiList = this.SelectCanActiveList(this.GetActiveShenShiList(client, tabData.FuWenEquipList));
					int i;
					for (i = 0; i < tabData.ShenShiActiveList.Count; i++)
					{
						int shenShiId = newShenShiList.Find((int x) => x / 100 == tabData.ShenShiActiveList[i] / 100);
						if (shenShiId > 0)
						{
							tabData.ShenShiActiveList[i] = shenShiId;
						}
						else
						{
							tabData.ShenShiActiveList.RemoveAt(i--);
						}
					}
					if (tabId == Global.GetRoleParamsInt32FromDB(client, "10185"))
					{
						client.ClientData.ShenShiEquipData.SkillEquip = tabData.SkillEquip;
						client.ClientData.ShenShiEquipData.ShenShiActiveList = tabData.ShenShiActiveList;
					}
					if (writeDB)
					{
						Global.sendToDB<int, FuWenTabData>(20316, tabData, client.ServerId);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 检查神识有效性。rid:{0} tabId:{1} ex:{2}", client.ClientData.RoleID, tabId, ex.Message), null, true);
			}
		}

		
		public FuWenTabData GetRoleFuWenTabData(GameClient client)
		{
			int tabId = Global.GetRoleParamsInt32FromDB(client, "10185");
			FuWenTabData result;
			if (client.ClientData.FuWenTabList == null || client.ClientData.FuWenTabList.Count <= tabId)
			{
				result = null;
			}
			else
			{
				result = client.ClientData.FuWenTabList[tabId];
			}
			return result;
		}

		
		public List<int> GetAttackerShenShiActiveList(object attacker, ref int rid, ref int magicCode)
		{
			if (this.ShenShiRunTimeData.ParentMagicCode.ContainsKey(magicCode))
			{
				magicCode = this.ShenShiRunTimeData.ParentMagicCode[magicCode];
			}
			List<int> shenShiActiveList = new List<int>();
			SkillEquipData shenShiEquipData;
			if (attacker is GameClient)
			{
				shenShiEquipData = (attacker as GameClient).ClientData.ShenShiEquipData;
				rid = (attacker as GameClient).ClientData.RoleID;
			}
			else
			{
				if (!(attacker is Robot))
				{
					return null;
				}
				shenShiEquipData = (attacker as Robot).getRoleDataMini().ShenShiEquipData;
				rid = (attacker as Robot).RoleID;
			}
			List<int> result;
			if (shenShiEquipData == null || shenShiEquipData.SkillEquip != magicCode || shenShiEquipData.ShenShiActiveList == null || shenShiEquipData.ShenShiActiveList.Count < 1)
			{
				result = null;
			}
			else
			{
				result = shenShiEquipData.ShenShiActiveList;
			}
			return result;
		}

		
		public double GetMagicCodeAddPercent(object attacker, object enemy, int magicCode)
		{
			double ret = 0.0;
			int rid = 0;
			try
			{
				GameClient client = (attacker is GameClient) ? (attacker as GameClient) : null;
				Robot robot = (attacker is Robot) ? (attacker as Robot) : null;
				List<int> shenShiActiveList = this.GetAttackerShenShiActiveList(attacker, ref rid, ref magicCode);
				if (shenShiActiveList == null || shenShiActiveList.Count < 1)
				{
					return 0.0;
				}
				foreach (int shenShiID in shenShiActiveList)
				{
					FuWenGodItem godItem = null;
					if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(shenShiID, out godItem))
					{
						foreach (MagicActionItem magicItem in godItem.MagicItemList)
						{
							double _params = 0.0;
							double _params2 = 0.0;
							if (magicItem.MagicActionParams.Length > 0)
							{
								_params = magicItem.MagicActionParams[0];
							}
							if (magicItem.MagicActionParams.Length > 1)
							{
								_params2 = magicItem.MagicActionParams[1];
							}
							switch (magicItem.MagicActionID)
							{
							case MagicActionIDs.MU_XINGHONG:
								if (null != client)
								{
									if (Convert.ToInt32((double)client.ClientData.LifeV * _params) >= client.ClientData.CurrentLifeV)
									{
										ret += _params2;
									}
								}
								else if (null != robot)
								{
									if (robot.MonsterInfo.VLifeMax * _params >= robot.VLife)
									{
										ret += _params2;
									}
								}
								break;
							case MagicActionIDs.MU_JUXIONG:
								if (client != null)
								{
									if (Convert.ToInt32((double)client.ClientData.LifeV * _params) <= client.ClientData.CurrentLifeV)
									{
										ret += _params2;
									}
								}
								else if (robot != null)
								{
									if (robot.MonsterInfo.VLifeMax * _params <= robot.VLife)
									{
										ret += _params2;
									}
								}
								break;
							case MagicActionIDs.MU_SUOMING:
								if (enemy is GameClient)
								{
									if (Convert.ToInt32((double)(enemy as GameClient).ClientData.LifeV * _params) >= (enemy as GameClient).ClientData.CurrentLifeV)
									{
										ret += _params2;
									}
								}
								else if (enemy is Monster)
								{
									if ((enemy as Monster).MonsterInfo.VLifeMax * _params >= (enemy as Monster).VLife)
									{
										ret += _params2;
									}
								}
								break;
							case MagicActionIDs.MU_ZHANZHENG:
								if (enemy is GameClient)
								{
									if (Convert.ToInt32((double)(enemy as GameClient).ClientData.LifeV * _params) <= (enemy as GameClient).ClientData.CurrentLifeV)
									{
										ret += _params2;
									}
								}
								else if (enemy is Monster)
								{
									if ((enemy as Monster).MonsterInfo.VLifeMax * _params <= (enemy as Monster).VLife)
									{
										ret += _params2;
									}
								}
								break;
							case MagicActionIDs.MU_SUILU:
								if (enemy is GameClient)
								{
									if ((enemy as GameClient).buffManager.IsBuffEnabled(117) || (enemy as GameClient).RoleBuffer.GetExtProp(50) > 0.1)
									{
										ret += _params;
									}
								}
								else if (enemy is Monster)
								{
									if ((enemy as Monster).IsMonsterXuanYun())
									{
										ret += _params;
									}
								}
								break;
							case MagicActionIDs.MU_JIELV:
								if (enemy is GameClient)
								{
									if ((enemy as GameClient).RoleBuffer.GetExtProp(47) > 0.1)
									{
										ret += _params;
									}
								}
								else if (enemy is Monster)
								{
									if ((enemy as Monster).IsMonsterDingShen())
									{
										ret += _params;
									}
								}
								break;
							case MagicActionIDs.MU_XUELANG:
								if (enemy is GameClient)
								{
									if ((enemy as GameClient).buffManager.IsBuffEnabled(118))
									{
										ret += _params;
									}
								}
								else if (enemy is Monster)
								{
									if ((enemy as Monster).IsMonsterSpeedDown())
									{
										ret += _params;
									}
								}
								break;
							case MagicActionIDs.MU_XIAOYONG:
								if (null != client)
								{
									if (enemy is GameClient)
									{
										if (Global.GetTwoPointDistance(client.CurrentPos, (enemy as GameClient).CurrentPos) <= _params * 100.0)
										{
											ret += _params2;
										}
									}
									else if (enemy is Monster)
									{
										if (Global.GetTwoPointDistance(client.CurrentPos, (enemy as Monster).CurrentPos) <= _params * 100.0)
										{
											ret += _params2;
										}
									}
								}
								else if (null != robot)
								{
									if (enemy is GameClient)
									{
										if (Global.GetTwoPointDistance(robot.CurrentPos, (enemy as GameClient).CurrentPos) <= _params * 100.0)
										{
											ret += _params2;
										}
									}
									else if (enemy is Monster)
									{
										if (Global.GetTwoPointDistance(robot.CurrentPos, (enemy as Monster).CurrentPos) <= _params * 100.0)
										{
											ret += _params2;
										}
									}
								}
								break;
							case MagicActionIDs.MU_MAOYOU:
								if (null != client)
								{
									if (enemy is GameClient)
									{
										if (Global.GetTwoPointDistance(client.CurrentPos, (enemy as GameClient).CurrentPos) >= _params * 100.0)
										{
											ret += _params2;
										}
									}
									else if (enemy is Monster)
									{
										if (Global.GetTwoPointDistance(client.CurrentPos, (enemy as Monster).CurrentPos) >= _params * 100.0)
										{
											ret += _params2;
										}
									}
								}
								else if (null != robot)
								{
									if (enemy is GameClient)
									{
										if (Global.GetTwoPointDistance(robot.CurrentPos, (enemy as GameClient).CurrentPos) >= _params * 100.0)
										{
											ret += _params2;
										}
									}
									else if (enemy is Monster)
									{
										if (Global.GetTwoPointDistance(robot.CurrentPos, (enemy as Monster).CurrentPos) >= _params * 100.0)
										{
											ret += _params2;
										}
									}
								}
								break;
							}
						}
					}
				}
				if (magicCode == 11004 || magicCode == 11006)
				{
					ret /= 5.0;
				}
				return ret;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 获取神识伤害百分比加成信息错误。rid:{0}, maggicCode:{1}, ex:{2}", rid, magicCode, ex.Message), null, true);
			}
			return ret;
		}

		
		public double GetMagicCodeAddInjure(object attacker, object enemy, int magicCode)
		{
			double ret = 0.0;
			int rid = 0;
			try
			{
				GameClient client = (attacker is GameClient) ? (attacker as GameClient) : null;
				Robot robot = (attacker is Robot) ? (attacker as Robot) : null;
				List<int> shenShiActiveList = this.GetAttackerShenShiActiveList(attacker, ref rid, ref magicCode);
				if (shenShiActiveList == null || shenShiActiveList.Count < 1)
				{
					return 0.0;
				}
				foreach (int shenShiID in shenShiActiveList)
				{
					FuWenGodItem godItem = null;
					if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(shenShiID, out godItem))
					{
						foreach (MagicActionItem magicItem in godItem.MagicItemList)
						{
							double _params = 0.0;
							if (magicItem.MagicActionParams.Length > 0)
							{
								_params = magicItem.MagicActionParams[0];
							}
							MagicActionIDs magicActionID = magicItem.MagicActionID;
							if (magicActionID == MagicActionIDs.MU_LUHUO)
							{
								if (null != client)
								{
									double defense = (RoleAlgorithm.GetExtProp(client, 4) + RoleAlgorithm.GetExtProp(client, 6)) * 0.5;
									ret += defense * _params;
								}
								else if (null != robot)
								{
									double defense = (double)(robot.MonsterInfo.Defense + robot.MonsterInfo.MDefense) * 0.5;
									ret += defense * _params;
								}
							}
						}
					}
				}
				if (magicCode == 11004 || magicCode == 11006)
				{
					ret /= 5.0;
				}
				return ret;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 获取神识伤害加成信息错误。rid:{0}, maggicCode:{1}, ex:{2}", rid, magicCode, ex.Message), null, true);
			}
			return ret;
		}

		
		public double GetMagicCodeSkillCDSubPercent(GameClient client, int magicCode)
		{
			double ret = 0.0;
			try
			{
				if (client.ClientData.CurrentMagicCDSubPercent > 0.0 || this.ShenShiRunTimeData.ParentMagicCode.ContainsKey(magicCode))
				{
					return client.ClientData.CurrentMagicCDSubPercent;
				}
				int tabId = Global.GetRoleParamsInt32FromDB(client, "10185");
				if (client.ClientData.FuWenTabList != null && client.ClientData.FuWenTabList.Count > tabId)
				{
					FuWenTabData tabData = client.ClientData.FuWenTabList[tabId];
					if (tabData.SkillEquip == magicCode && tabData.ShenShiActiveList != null && tabData.ShenShiActiveList.Count >= 0)
					{
						foreach (int shenShiID in tabData.ShenShiActiveList)
						{
							FuWenGodItem godItem = null;
							if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(shenShiID, out godItem))
							{
								foreach (MagicActionItem magicItem in godItem.MagicItemList)
								{
									double _params = 0.0;
									double _params2 = 0.0;
									if (magicItem.MagicActionParams.Length > 0)
									{
										_params = magicItem.MagicActionParams[0];
									}
									if (magicItem.MagicActionParams.Length > 1)
									{
										_params2 = magicItem.MagicActionParams[1];
									}
									MagicActionIDs magicActionID = magicItem.MagicActionID;
									if (magicActionID == MagicActionIDs.MU_MENGJING)
									{
										if (Global.GetRandom() <= _params)
										{
											ret += _params2;
										}
									}
								}
							}
						}
					}
				}
				return (ret > 1.0) ? 1.0 : ret;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 获取神识CD缩减加成信息错误。rid:{0}, maggicCode:{1}, ex:{2}", client.ClientData.RoleID, magicCode, ex.Message), null, true);
			}
			return ret;
		}

		
		public double GetMagicCodeAddPercent2(object attacker, List<object> enemyList, int magicCode)
		{
			double ret = 0.0;
			int rid = 0;
			try
			{
				GameClient gameClient = (attacker is GameClient) ? (attacker as GameClient) : null;
				Robot robot = (attacker is Robot) ? (attacker as Robot) : null;
				List<int> shenShiActiveList = this.GetAttackerShenShiActiveList(attacker, ref rid, ref magicCode);
				if (shenShiActiveList == null || shenShiActiveList.Count < 1)
				{
					return 0.0;
				}
				foreach (int shenShiID in shenShiActiveList)
				{
					FuWenGodItem godItem = null;
					if (this.ShenShiRunTimeData.FuWenGodDict.TryGetValue(shenShiID, out godItem))
					{
						foreach (MagicActionItem magicItem in godItem.MagicItemList)
						{
							double _params = 0.0;
							double _params2 = 0.0;
							if (magicItem.MagicActionParams.Length > 0)
							{
								_params = magicItem.MagicActionParams[0];
							}
							if (magicItem.MagicActionParams.Length > 1)
							{
								_params2 = magicItem.MagicActionParams[1];
							}
							MagicActionIDs magicActionID = magicItem.MagicActionID;
							if (magicActionID == MagicActionIDs.MU_BAIZHAN)
							{
								if ((double)enemyList.Count >= _params)
								{
									ret += _params2;
								}
							}
						}
					}
				}
				if (magicCode == 11004 || magicCode == 11006)
				{
					ret /= 5.0;
				}
				return ret;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenShi :: 获取神识技能伤害百分比加成。rid:{0}, maggicCode:{1}, ex:{2}", rid, magicCode, ex.Message), null, true);
			}
			return ret;
		}

		
		public ShenShiRunData ShenShiRunTimeData = new ShenShiRunData();

		
		public int FuWenFreeTime = 0;

		
		public int FuWenChouQuCost = 0;

		
		public int FuWenChouQuCost_10 = 0;

		
		public List<int> FuWenMagicList = new List<int>();

		
		public int InitFuWenTabNum = 0;

		
		public List<int> FuWenTabBuyCost = new List<int>();

		
		private static ShenShiManager instance = new ShenShiManager();
	}
}
