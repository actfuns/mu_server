using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class ShenQiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static ShenQiManager getInstance()
		{
			return ShenQiManager.instance;
		}

		
		public bool initialize()
		{
			this.LoadArtifactXml();
			this.LoadToughnessXml();
			this.LoadGodXml();
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1816, 1, 1, ShenQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1817, 2, 2, ShenQiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot4))
			{
				result = false;
			}
			else
			{
				switch (nID)
				{
				case 1816:
					result = this.ProcessShenQiInfoCmd(client, nID, bytes, cmdParams);
					break;
				case 1817:
					result = this.ProcessShenQiUpCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		
		public bool ProcessShenQiInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				ShenQiData data = ShenQiManager.GetShenQiData(client);
				client.sendCmd<ShenQiData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessShenQiUpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				int useBaoJi = Convert.ToInt32(cmdParams[1]);
				ShenQiData data = this.LevelUpShenQiData(client, useBaoJi);
				client.sendCmd<ShenQiData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public void LoadArtifactXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(ShenQiConsts.Artifact);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					Dictionary<int, ArtifactItem> artifactDict = new Dictionary<int, ArtifactItem>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							string name = Global.GetDefAttributeStr(xmlItem, "Name", "");
							int[] propArray = new int[]
							{
								Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "LifeV", "0")),
								Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "AddAttack", "0")),
								Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "AddDefense", "0")),
								Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Toughness", "0"))
							};
							string[] qiangHua = Global.GetDefAttributeStr(xmlItem, "QiangHua", "").Split(new char[]
							{
								'|'
							});
							int[] qiangHuaRateArray = new int[3];
							int[][] qiangHuaArray = new int[3][];
							for (int i = 0; i < qiangHua.Length; i++)
							{
								string[] item = qiangHua[i].Split(new char[]
								{
									','
								});
								qiangHuaRateArray[i] = (int)(Convert.ToDouble(item[0]) * 100.0);
								qiangHuaArray[i] = new int[4];
								qiangHuaArray[i][0] = Convert.ToInt32(item[1]);
								qiangHuaArray[i][1] = Convert.ToInt32(item[2]);
								qiangHuaArray[i][2] = Convert.ToInt32(item[3]);
								qiangHuaArray[i][3] = Convert.ToInt32(item[4]);
							}
							int costShenLi = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "CostShenLiJingHua", "0"));
							int costJinBi = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "CostGoldCoin", "0"));
							int costZuanShi = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "CostDiamond", "0"));
							int costGoldGoodsID = 0;
							int costGoldGoodsNum = 0;
							string[] costGoldGoods = Global.GetDefAttributeStr(xmlItem, "CostGoldGoods", "").Split(new char[]
							{
								','
							});
							if (costGoldGoods.Length == 2)
							{
								costGoldGoodsID = Convert.ToInt32(costGoldGoods[0]);
								costGoldGoodsNum = Convert.ToInt32(costGoldGoods[1]);
							}
							artifactDict[id] = new ArtifactItem
							{
								ID = id,
								Name = name,
								PropArray = propArray,
								QiangHuaRate = qiangHuaRateArray,
								QiangHuaArray = qiangHuaArray,
								CostShenLiJingHua = costShenLi,
								CostGoldCoin = costJinBi,
								CostDiamond = costZuanShi,
								CostGoldGoodsID = costGoldGoodsID,
								CostGoldGoodsNum = costGoldGoodsNum
							};
						}
					}
					lock (this.ShenQiRunTimeData.Mutex)
					{
						this.ShenQiRunTimeData.ArtifactXmlDict = artifactDict;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		
		public void LoadToughnessXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(ShenQiConsts.Toughness);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					List<ToughnessItem> toughnessList = new List<ToughnessItem>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int toughness = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Toughness", "0"));
							double deLucky = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DeLucky", "0"));
							double deFatalAttack = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DeFatalAttack", "0"));
							double deDoubleAttack = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DeDoubleAttack", "0"));
							double deSavagePercent = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DeSavagePercent", "0"));
							double deColdPercent = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DeColdPercent", "0"));
							double deRuthlessPercent = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DeRuthlessPercent", "0"));
							double deFrozenPercent = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DeFrozenPercent", "0"));
							double dePalsyPercent = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DePalsyPercent", "0"));
							double deSpeedDownPercent = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DeSpeedDownPercent", "0"));
							double deBlowPercent = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "DeBlowPercent", "0"));
							toughnessList.Add(new ToughnessItem
							{
								Toughness = toughness,
								DeLucky = deLucky,
								DeFatalAttack = deFatalAttack,
								DeDoubleAttack = deDoubleAttack,
								DeSavagePercent = deSavagePercent,
								DeColdPercent = deColdPercent,
								DeRuthlessPercent = deRuthlessPercent,
								DeFrozenPercent = deFrozenPercent,
								DePalsyPercent = dePalsyPercent,
								DeSpeedDownPercent = deSpeedDownPercent,
								DeBlowPercent = deBlowPercent
							});
						}
					}
					if (toughnessList.Count < 1)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("ShenQi :: 韧性表不存在数据。", new object[0]), null, true);
					}
					else
					{
						toughnessList.Sort((ToughnessItem x, ToughnessItem y) => x.Toughness - y.Toughness);
						lock (this.ShenQiRunTimeData.Mutex)
						{
							this.ShenQiRunTimeData.ToughnessXmlList = toughnessList;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		
		public void LoadGodXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(ShenQiConsts.God);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					List<GodItem> godList = new List<GodItem>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							string[] condition = Global.GetDefAttributeStr(xmlItem, "OpenCondition", "").Split(new char[]
							{
								'|'
							});
							List<int> openCondition = new List<int>();
							foreach (string item in condition)
							{
								openCondition.Add(Convert.ToInt32(item));
							}
							string[] props = Global.GetDefAttributeStr(xmlItem, "ActivationProperty", "").Split(new char[]
							{
								'|'
							});
							List<Dictionary<int, double>> activationProperty = new List<Dictionary<int, double>>();
							foreach (string item in props)
							{
								Dictionary<int, double> kvItem = new Dictionary<int, double>();
								string[] KvpFileds = item.Split(new char[]
								{
									','
								});
								if (KvpFileds.Length == 2)
								{
									kvItem[(int)ConfigParser.GetPropIndexByPropName(KvpFileds[0])] = Global.SafeConvertToDouble(KvpFileds[1]);
								}
								activationProperty.Add(kvItem);
							}
							godList.Add(new GodItem
							{
								ID = id,
								OpenCondition = openCondition,
								ActivationProperty = activationProperty
							});
						}
					}
					if (godList.Count < 1)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("ShenQi :: 神像表不存在数据。", new object[0]), null, true);
					}
					else
					{
						lock (this.ShenQiRunTimeData.Mutex)
						{
							this.ShenQiRunTimeData.GodXmlList = godList;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		
		public static ShenQiData GetShenQiData(GameClient client)
		{
			ShenQiData result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.ShenQi, false))
			{
				result = new ShenQiData
				{
					UpResultType = -1
				};
			}
			else
			{
				ShenQiData data = client.ClientData.shenQiData;
				try
				{
					if (null == data)
					{
						data = new ShenQiData();
						List<int> props = Global.GetRoleParamsIntListFromDB(client, "36");
						if (props == null || props.Count < 5)
						{
							props = new List<int>();
							for (int i = 0; i < 5; i++)
							{
								props.Add(0);
							}
							props[0] = 1;
							Global.SaveRoleParamsIntListToDB(client, props, "36", true);
						}
						data.ShenQiID = props[0];
						data.LifeAdd = props[1];
						data.AttackAdd = props[2];
						data.DefenseAdd = props[3];
						data.ToughnessAdd = props[4];
						client.ClientData.shenQiData = data;
					}
					data.ShenLiJingHuaLeft = client.ClientData.ShenLiJingHuaPoints;
					data.UpResultType = -100;
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ShenQi :: 获取角色神器数据错误 ex:{0}", ex.Message), null, true);
				}
				result = data;
			}
			return result;
		}

		
		public ShenQiData LevelUpShenQiData(GameClient client, int useBaoJi)
		{
			ShenQiData result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.ShenQi, false))
			{
				result = new ShenQiData
				{
					UpResultType = -1
				};
			}
			else
			{
				ShenQiData data = client.ClientData.shenQiData;
				try
				{
					if (null == data)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("ShenQi :: 注入失败，获取角色神器数据错误，角色id：{0}", client.ClientData.RoleID), null, true);
						return new ShenQiData
						{
							UpResultType = 0
						};
					}
					ArtifactItem artifactItem = null;
					lock (this.ShenQiRunTimeData.Mutex)
					{
						this.ShenQiRunTimeData.ArtifactXmlDict.TryGetValue(data.ShenQiID, out artifactItem);
					}
					if (null == artifactItem)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("ShenQi :: 注入失败，获取角色神器数据配置项错误，角色id：{0}, ShenQiID：{1}", client.ClientData.RoleID, data.ShenQiID), null, true);
						return new ShenQiData
						{
							UpResultType = 0
						};
					}
					if (client.ClientData.ShenLiJingHuaPoints < artifactItem.CostShenLiJingHua)
					{
						return new ShenQiData
						{
							UpResultType = -2
						};
					}
					int GoldGoodsNum = 0;
					if (artifactItem.CostGoldGoodsID > 0)
					{
						GoldGoodsNum = Global.GetTotalGoodsCountByID(client, artifactItem.CostGoldGoodsID);
					}
					if (client.ClientData.YinLiang < artifactItem.CostGoldCoin && (artifactItem.CostGoldGoodsNum <= 0 || GoldGoodsNum < artifactItem.CostGoldGoodsNum))
					{
						return new ShenQiData
						{
							UpResultType = -4
						};
					}
					if (useBaoJi > 0 && client.ClientData.UserMoney < artifactItem.CostDiamond && !HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, artifactItem.CostDiamond))
					{
						return new ShenQiData
						{
							UpResultType = -3
						};
					}
					GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, -artifactItem.CostShenLiJingHua, "神器注入_精华", true, true);
					if (artifactItem.CostGoldGoodsNum > 0 && GoldGoodsNum >= artifactItem.CostGoldGoodsNum)
					{
						bool oneUseBind = false;
						bool oneUseTimeLimit = false;
						if (Global.UseGoodsBindOrNot(client, artifactItem.CostGoldGoodsID, artifactItem.CostGoldGoodsNum, true, out oneUseBind, out oneUseTimeLimit) < 1)
						{
							return new ShenQiData
							{
								UpResultType = -4
							};
						}
					}
					else if (!GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, artifactItem.CostGoldCoin, "神器注入_金币", false))
					{
						return new ShenQiData
						{
							UpResultType = -4
						};
					}
					if (useBaoJi > 0)
					{
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, artifactItem.CostDiamond, "神器注入_钻石", true, true, false, DaiBiSySType.ShenQiXiTong))
						{
							return new ShenQiData
							{
								UpResultType = -4
							};
						}
					}
					int rate = 0;
					int r = Global.GetRandomNumber(0, 101);
					int[] addProps = null;
					for (int i = 0; i < artifactItem.QiangHuaRate.Length; i++)
					{
						rate += artifactItem.QiangHuaRate[i];
						if (r <= rate)
						{
							addProps = artifactItem.QiangHuaArray[i];
							data.BurstType = i;
							if (useBaoJi > 0 && 0 == i)
							{
								addProps = artifactItem.QiangHuaArray[1];
								data.BurstType = 1;
							}
							break;
						}
					}
					data.LifeAdd += addProps[0];
					data.LifeAdd = ((data.LifeAdd > artifactItem.PropArray[0]) ? artifactItem.PropArray[0] : data.LifeAdd);
					data.AttackAdd += addProps[1];
					data.AttackAdd = ((data.AttackAdd > artifactItem.PropArray[1]) ? artifactItem.PropArray[1] : data.AttackAdd);
					data.DefenseAdd += addProps[2];
					data.DefenseAdd = ((data.DefenseAdd > artifactItem.PropArray[2]) ? artifactItem.PropArray[2] : data.DefenseAdd);
					data.ToughnessAdd += addProps[3];
					data.ToughnessAdd = ((data.ToughnessAdd > artifactItem.PropArray[3]) ? artifactItem.PropArray[3] : data.ToughnessAdd);
					if (data.LifeAdd < artifactItem.PropArray[0] || data.DefenseAdd < artifactItem.PropArray[2] || data.AttackAdd < artifactItem.PropArray[1] || data.ToughnessAdd < artifactItem.PropArray[3])
					{
						data.UpResultType = 1;
					}
					else if (this.ShenQiRunTimeData.ArtifactXmlDict.ContainsKey(data.ShenQiID + 1))
					{
						data.UpResultType = 2;
						data.ShenQiID++;
						data.LifeAdd = 0;
						data.AttackAdd = 0;
						data.DefenseAdd = 0;
						data.ToughnessAdd = 0;
					}
					else
					{
						data.UpResultType = 3;
					}
					data.ShenLiJingHuaLeft = client.ClientData.ShenLiJingHuaPoints;
					List<int> props = new List<int>();
					props.AddRange(new int[]
					{
						data.ShenQiID,
						data.LifeAdd,
						data.AttackAdd,
						data.DefenseAdd,
						data.ToughnessAdd
					});
					Global.SaveRoleParamsIntListToDB(client, props, "36", true);
					client.ClientData.shenQiData = data;
					this.UpdateRoleShenQiProps(client);
					this.UpdateRoleTouhgnessProps(client);
					if (data.UpResultType == 2 || data.UpResultType == 3)
					{
						this.UpdateRoleGodProps(client);
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "神像", "神器升阶", client.ClientData.RoleName, "系统", "增加", 1, client.ClientData.ZoneID, client.strUserID, data.ShenQiID, client.ServerId, null);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "韧性", "神器注入_韧性", client.ClientData.RoleName, "系统", "增加", addProps[3], client.ClientData.ZoneID, client.strUserID, Convert.ToInt32(RoleAlgorithm.GetExtProp(client, 101)), client.ServerId, null);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ShenQi :: 升级角色神器数据错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
					return data;
				}
				result = data;
			}
			return result;
		}

		
		public void UpdateRoleShenQiProps(GameClient client)
		{
			try
			{
				ShenQiData artifact = client.ClientData.shenQiData;
				if (null != artifact)
				{
					int life = artifact.LifeAdd;
					int attack = artifact.AttackAdd;
					int defense = artifact.DefenseAdd;
					int toughness = artifact.ToughnessAdd;
					Dictionary<int, ArtifactItem> artifactDic = null;
					lock (this.ShenQiRunTimeData.Mutex)
					{
						artifactDic = this.ShenQiRunTimeData.ArtifactXmlDict;
					}
					if (null != artifactDic)
					{
						foreach (ArtifactItem item in artifactDic.Values)
						{
							if (item.ID < artifact.ShenQiID)
							{
								life += item.PropArray[0];
								attack += item.PropArray[1];
								defense += item.PropArray[2];
								toughness += item.PropArray[3];
							}
						}
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							26,
							13,
							life
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							26,
							45,
							attack
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							26,
							46,
							defense
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							26,
							101,
							toughness
						});
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenQi :: 更新角色神器加成错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
		}

		
		public void UpdateRoleTouhgnessProps(GameClient client)
		{
			try
			{
				double toughness = RoleAlgorithm.GetExtProp(client, 101);
				if (toughness >= 1.0)
				{
					List<ToughnessItem> toughnessList = null;
					lock (this.ShenQiRunTimeData.Mutex)
					{
						toughnessList = this.ShenQiRunTimeData.ToughnessXmlList;
					}
					if (toughnessList != null && toughnessList.Count >= 1)
					{
						int index;
						for (index = toughnessList.Count - 1; index >= 0; index--)
						{
							if (toughness >= (double)toughnessList[index].Toughness)
							{
								break;
							}
						}
						ToughnessItem toughnessItem = (index >= 0) ? toughnessList[index] : null;
						if (null != toughnessItem)
						{
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								51,
								toughnessItem.DeLucky
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								52,
								toughnessItem.DeFatalAttack
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								53,
								toughnessItem.DeDoubleAttack
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								64,
								toughnessItem.DeSavagePercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								65,
								toughnessItem.DeColdPercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								66,
								toughnessItem.DeRuthlessPercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								97,
								toughnessItem.DeFrozenPercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								98,
								toughnessItem.DePalsyPercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								99,
								toughnessItem.DeSpeedDownPercent
							});
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								27,
								100,
								toughnessItem.DeBlowPercent
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenQi :: 更新角色韧性加成错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
		}

		
		public void UpdateRoleGodProps(GameClient client)
		{
			try
			{
				ShenQiData shenQiData = client.ClientData.shenQiData;
				int shenQiID = shenQiData.ShenQiID;
				if (shenQiID >= 1)
				{
					ArtifactItem artifactItem = null;
					lock (this.ShenQiRunTimeData.Mutex)
					{
						this.ShenQiRunTimeData.ArtifactXmlDict.TryGetValue(shenQiID, out artifactItem);
					}
					if (shenQiData.LifeAdd < artifactItem.PropArray[0] || shenQiData.DefenseAdd < artifactItem.PropArray[2] || shenQiData.AttackAdd < artifactItem.PropArray[1] || shenQiData.ToughnessAdd < artifactItem.PropArray[3])
					{
						shenQiID--;
					}
					List<GodItem> godList = null;
					lock (this.ShenQiRunTimeData.Mutex)
					{
						godList = this.ShenQiRunTimeData.GodXmlList;
					}
					if (godList != null && godList.Count >= 1)
					{
						Dictionary<int, double> addPropsDict = new Dictionary<int, double>();
						foreach (GodItem item in godList)
						{
							bool open = true;
							foreach (int id in item.OpenCondition)
							{
								if (id > shenQiID)
								{
									open = false;
									break;
								}
							}
							if (open)
							{
								foreach (Dictionary<int, double> propDic in item.ActivationProperty)
								{
									foreach (KeyValuePair<int, double> propItem in propDic)
									{
										addPropsDict[propItem.Key] = (addPropsDict.ContainsKey(propItem.Key) ? (addPropsDict[propItem.Key] + propItem.Value) : propItem.Value);
									}
								}
							}
						}
						foreach (KeyValuePair<int, double> item2 in addPropsDict)
						{
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								28,
								item2.Key,
								item2.Value
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ShenQi :: 更新角色神像加成错误 roleid={0} ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
		}

		
		public static void InitRoleShenQiData(GameClient client)
		{
			client.ClientData.ShenLiJingHuaPoints = Global.GetRoleParamsInt32FromDB(client, "10157");
			if (GlobalNew.IsGongNengOpened(client, GongNengIDs.ShenQi, false))
			{
				client.ClientData.shenQiData = ShenQiManager.GetShenQiData(client);
				ShenQiManager.getInstance().UpdateRoleShenQiProps(client);
				ShenQiManager.getInstance().UpdateRoleTouhgnessProps(client);
				ShenQiManager.getInstance().UpdateRoleGodProps(client);
			}
		}

		
		public ShenQiRunData ShenQiRunTimeData = new ShenQiRunData();

		
		private static ShenQiManager instance = new ShenQiManager();
	}
}
