using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class MazingerStoreManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2
	{
		
		public bool InitConfig()
		{
			Dictionary<int, Dictionary<int, MazingerUpGrade>> MazingerGradeHot = new Dictionary<int, Dictionary<int, MazingerUpGrade>>();
			Dictionary<int, int> MazingerGradeLevelMaxHot = new Dictionary<int, int>();
			string fileName = Global.GameResPath(MazingerStoreConst.MoShenMiBaoJie);
			XElement xml = XElement.Load(fileName);
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
			}
			try
			{
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					MazingerUpGrade rst = new MazingerUpGrade();
					Dictionary<int, int> UseGoods = new Dictionary<int, int>();
					rst.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
					rst.Stage = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "MiBaoStageLevel"));
					rst.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "MiBaoType"));
					rst.LuckyOne = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "LuckyOne"));
					rst.LuckyTwo = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "LuckyTwo"));
					rst.Rate = Convert.ToDouble(Global.GetSafeAttributeStr(xmlItem, "LuckyTwoRate"));
					string[] need = Global.GetSafeAttributeStr(xmlItem, "NeedGoods").Split(new char[]
					{
						'|'
					});
					if (need.Length != 1)
					{
						UseGoods.Add(Convert.ToInt32(need[0]), Convert.ToInt32(need[1]));
					}
					rst.UseGoods = UseGoods;
					if (MazingerGradeLevelMaxHot.ContainsKey(rst.Type))
					{
						if (MazingerGradeLevelMaxHot[rst.Type] < rst.Stage)
						{
							MazingerGradeLevelMaxHot[rst.Type] = rst.Stage;
						}
					}
					else
					{
						MazingerGradeLevelMaxHot.Add(rst.Type, rst.Stage);
					}
					if (MazingerGradeHot.ContainsKey(rst.Type))
					{
						if (MazingerGradeHot[rst.Type].ContainsKey(rst.Stage))
						{
							MazingerGradeHot[rst.Type][rst.Stage] = rst;
						}
						else
						{
							MazingerGradeHot[rst.Type].Add(rst.Stage, rst);
						}
					}
					else
					{
						Dictionary<int, MazingerUpGrade> dict = new Dictionary<int, MazingerUpGrade>();
						dict.Add(rst.Stage, rst);
						MazingerGradeHot.Add(rst.Type, dict);
					}
				}
				this.MazingerGrade = MazingerGradeHot;
				this.MazingerGradeLevelMax = MazingerGradeLevelMaxHot;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			bool result;
			if (this.MazingerGrade == null || this.MazingerGradeLevelMax == null)
			{
				result = false;
			}
			else
			{
				Dictionary<int, Dictionary<int, Dictionary<int, MazingerUpStar>>> MazingerStarHot = new Dictionary<int, Dictionary<int, Dictionary<int, MazingerUpStar>>>();
				Dictionary<int, Dictionary<int, int>> MazingerStarLevelMaxHot = new Dictionary<int, Dictionary<int, int>>();
				fileName = Global.GameResPath(MazingerStoreConst.MoShenMiBaoXing);
				xml = XElement.Load(fileName);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
				}
				try
				{
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						MazingerUpStar rst2 = new MazingerUpStar();
						Dictionary<int, int> UseGoods = new Dictionary<int, int>();
						Dictionary<int, double> Attr = new Dictionary<int, double>();
						rst2.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
						rst2.Stage = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "MiBaoStageLevel"));
						rst2.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "MibaoStarLevel"));
						rst2.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "MiBaoType"));
						rst2.UpExp = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "MibaoStarExp"));
						rst2.Exp = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "GoodsExp"));
						string[] need = Global.GetSafeAttributeStr(xmlItem, "NeedGoods").Split(new char[]
						{
							'|'
						});
						if (need.Length != 1)
						{
							UseGoods.Add(Convert.ToInt32(need[0]), Convert.ToInt32(need[1]));
						}
						rst2.UseGoods = UseGoods;
						string[] attr = Global.GetSafeAttributeStr(xmlItem, "MiBaoAttribute").Split(new char[]
						{
							'|'
						});
						foreach (string it in attr)
						{
							string[] temp = it.Split(new char[]
							{
								','
							});
							if (temp.Length == 2)
							{
								Attr.Add((int)ConfigParser.GetPropIndexByPropName(temp[0]), Convert.ToDouble(temp[1]));
							}
						}
						rst2.Attr = Attr;
						if (MazingerStarLevelMaxHot.ContainsKey(rst2.Type))
						{
							if (MazingerStarLevelMaxHot[rst2.Type].ContainsKey(rst2.Stage))
							{
								if (MazingerStarLevelMaxHot[rst2.Type][rst2.Stage] < rst2.Level)
								{
									MazingerStarLevelMaxHot[rst2.Type][rst2.Stage] = rst2.Level;
								}
							}
							else
							{
								MazingerStarLevelMaxHot[rst2.Type].Add(rst2.Stage, rst2.Level);
							}
						}
						else
						{
							Dictionary<int, int> dict2 = new Dictionary<int, int>();
							dict2.Add(rst2.Stage, rst2.Level);
							MazingerStarLevelMaxHot.Add(rst2.Type, dict2);
						}
						if (MazingerStarHot.ContainsKey(rst2.Type))
						{
							if (MazingerStarHot[rst2.Type].ContainsKey(rst2.Stage))
							{
								if (!MazingerStarHot[rst2.Type][rst2.Stage].ContainsKey(rst2.Level))
								{
									MazingerStarHot[rst2.Type][rst2.Stage].Add(rst2.Level, rst2);
								}
							}
							else
							{
								Dictionary<int, MazingerUpStar> star = new Dictionary<int, MazingerUpStar>();
								star.Add(rst2.Level, rst2);
								MazingerStarHot[rst2.Type].Add(rst2.Stage, star);
							}
						}
						else
						{
							Dictionary<int, Dictionary<int, MazingerUpStar>> dict3 = new Dictionary<int, Dictionary<int, MazingerUpStar>>();
							Dictionary<int, MazingerUpStar> star = new Dictionary<int, MazingerUpStar>();
							star.Add(rst2.Level, rst2);
							dict3.Add(rst2.Stage, star);
							MazingerStarHot.Add(rst2.Type, dict3);
						}
					}
					this.MazingerStar = MazingerStarHot;
					this.MazingerStarLevelMax = MazingerStarLevelMaxHot;
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = (this.MazingerStar != null && MazingerStarLevelMaxHot != null);
			}
			return result;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2097, 2, 2, MazingerStoreManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		
		public static MazingerStoreManager getInstance()
		{
			return MazingerStoreManager.instance;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			if (nID == 2097)
			{
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int ClientType = Convert.ToInt32(cmdParams[0]);
					int ClientOpt = Convert.ToInt32(cmdParams[1]);
					MazingerStore resultData = this.ProcessMazingerStoreUpGrade(client, ClientType, ClientOpt);
					client.sendCmd<MazingerStore>(nID, resultData, false);
				}
				catch (Exception ex)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(ex, "CMD_SPR_MAZINGERSTORE_UPGRADE", false, false);
				}
			}
			return true;
		}

		
		public void UpdateProps(GameClient client)
		{
			double[] _ExtProps = new double[177];
			try
			{
				if (client.ClientData.MazingerStoreDataInfo != null && client.ClientData.MazingerStoreDataInfo.Count != 0)
				{
					foreach (MazingerStoreData it in client.ClientData.MazingerStoreDataInfo.Values)
					{
						if (this.MazingerStar.ContainsKey(it.Type))
						{
							if (this.MazingerStar[it.Type].ContainsKey(it.Stage))
							{
								if (this.MazingerStar[it.Type][it.Stage].ContainsKey(it.StarLevel))
								{
									foreach (KeyValuePair<int, double> iter in this.MazingerStar[it.Type][it.Stage][it.StarLevel].Attr)
									{
										_ExtProps[iter.Key] += iter.Value;
									}
								}
							}
						}
					}
				}
			}
			finally
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.MazingerStore,
					_ExtProps
				});
			}
		}

		
		public List<double> GetSystemParamMibao()
		{
			List<string> param = GameManager.systemParamsList.GetParamValueStringListByName("MibaoBaoji", ',');
			List<double> result;
			try
			{
				List<double> rst = new List<double>();
				foreach (string it in param)
				{
					rst.Add(Convert.ToDouble(it));
				}
				result = rst;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				result = null;
			}
			return result;
		}

		
		public MazingerStoreData CopyMazingerStoreMemData(GameClient client, int key)
		{
			return new MazingerStoreData
			{
				RoleID = client.ClientData.RoleID,
				Type = client.ClientData.MazingerStoreDataInfo[key].Type,
				Stage = client.ClientData.MazingerStoreDataInfo[key].Stage,
				StarLevel = client.ClientData.MazingerStoreDataInfo[key].StarLevel,
				Exp = client.ClientData.MazingerStoreDataInfo[key].Exp
			};
		}

		
		public bool UseXmlGoods(GameClient client, Dictionary<int, Dictionary<int, GoodsData>> TotleGoods)
		{
			foreach (Dictionary<int, GoodsData> goodDict in TotleGoods.Values)
			{
				using (Dictionary<int, GoodsData>.Enumerator enumerator2 = goodDict.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						KeyValuePair<int, GoodsData> iter = enumerator2.Current;
						bool EquipBindUse;
						if (!RebornStone.RebornUseGoodHasBinding(client, iter.Value.GoodsID, iter.Key, 1, out EquipBindUse))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		
		public MazingerStore ProcessMazingerStoreUpGrade(GameClient client, int ClientType, int ClientOpt)
		{
			MazingerStore res = new MazingerStore();
			res.IsBoom = 0;
			bool up = false;
			MazingerStore result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.MazingerStore, false))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("玩家魔神秘宝功能未开启, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
				res.result = 12;
				result = res;
			}
			else
			{
				if (ClientOpt < 0 || ClientOpt > 1)
				{
					res.result = 2;
				}
				else if (ClientOpt == 0)
				{
					if (this.MazingerStar == null || this.MazingerStarLevelMax == null || !this.MazingerStar.ContainsKey(ClientType) || !this.MazingerStarLevelMax.ContainsKey(ClientType))
					{
						res.result = 3;
					}
					else
					{
						if (client.ClientData.MazingerStoreDataInfo == null)
						{
							client.ClientData.MazingerStoreDataInfo = new Dictionary<int, MazingerStoreData>();
						}
						MazingerStoreData newData = null;
						bool flag = false;
						int StarLevelMax = 0;
						if (client.ClientData.MazingerStoreDataInfo.ContainsKey(ClientType))
						{
							StarLevelMax = this.MazingerStarLevelMax[ClientType][client.ClientData.MazingerStoreDataInfo[ClientType].Stage];
							if (client.ClientData.MazingerStoreDataInfo[ClientType].StarLevel >= StarLevelMax)
							{
								res.result = 5;
								goto IL_B64;
							}
							newData = this.CopyMazingerStoreMemData(client, ClientType);
							flag = true;
						}
						else
						{
							newData = new MazingerStoreData();
							newData.RoleID = client.ClientData.RoleID;
							newData.Type = ClientType;
							newData.Exp = 0;
							newData.Stage = 1;
							newData.StarLevel = 0;
							StarLevelMax = this.MazingerStarLevelMax[ClientType][newData.Stage];
						}
						if (newData == null || !this.MazingerStar.ContainsKey(newData.Type) || !this.MazingerStar[newData.Type].ContainsKey(newData.Stage) || !this.MazingerStar[newData.Type][newData.Stage].ContainsKey(newData.StarLevel) || this.MazingerStar[newData.Type][newData.Stage][newData.StarLevel].UseGoods == null || this.MazingerStar[newData.Type][newData.Stage][newData.StarLevel].UseGoods.Count == 0)
						{
							res.result = 6;
						}
						else
						{
							List<double> rate = this.GetSystemParamMibao();
							if (rate == null || rate.Count != 2)
							{
								res.result = 4;
							}
							else
							{
								int AddExp = this.MazingerStar[newData.Type][newData.Stage][newData.StarLevel].Exp;
								double rand = Global.GetRandom();
								if (rand <= rate[0])
								{
									AddExp = Convert.ToInt32((double)AddExp * rate[1]);
									res.IsBoom = 1;
								}
								foreach (KeyValuePair<int, int> iter in this.MazingerStar[newData.Type][newData.Stage][newData.StarLevel].UseGoods)
								{
									bool EquipBindUse;
									if (!RebornStone.RebornUseGoodHasBinding(client, iter.Key, iter.Value, 1, out EquipBindUse))
									{
										res.result = 7;
										break;
									}
								}
								if (res.result != 7)
								{
									int totleExp = newData.Exp + AddExp;
									int currLevel = newData.StarLevel;
									int oldUpExp = this.MazingerStar[newData.Type][newData.Stage][newData.StarLevel].UpExp;
									while (currLevel < StarLevelMax)
									{
										if (totleExp < oldUpExp)
										{
											break;
										}
										currLevel++;
										totleExp -= oldUpExp;
										if (totleExp < 0)
										{
											totleExp = 0;
											break;
										}
										if (this.MazingerStar[newData.Type][newData.Stage].ContainsKey(currLevel))
										{
											oldUpExp = this.MazingerStar[newData.Type][newData.Stage][currLevel].UpExp;
										}
										if (!up)
										{
											up = true;
										}
									}
									newData.StarLevel = currLevel;
									if (currLevel >= StarLevelMax)
									{
										newData.Exp = 0;
									}
									else
									{
										newData.Exp = totleExp;
									}
									if (flag)
									{
										int ret = Global.sendToDB<int, MazingerStoreData>(14126, newData, client.ServerId);
										if (ret < 0)
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("魔神秘宝修改数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
											res.result = 10;
											goto IL_B64;
										}
										GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔神秘宝升星", DateTime.Now.ToString(), newData.Type.ToString(), client.ClientData.RoleName, "系统", ClientType, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
										EventLogManager.AddMazingerStoreEvent(client, client.ClientData.MazingerStoreDataInfo[newData.Type].StarLevel, newData.StarLevel, client.ClientData.MazingerStoreDataInfo[newData.Type].Exp, newData.Exp, "魔神秘宝升星");
										client.ClientData.MazingerStoreDataInfo[newData.Type] = newData;
									}
									else
									{
										int ret = Global.sendToDB<int, MazingerStoreData>(14125, newData, client.ServerId);
										if (ret < 0)
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("魔神秘宝插入数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
											res.result = 10;
											goto IL_B64;
										}
										GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔神秘宝升星", DateTime.Now.ToString(), newData.Type.ToString(), client.ClientData.RoleName, "系统", ClientType, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
										EventLogManager.AddMazingerStoreEvent(client, 0, newData.StarLevel, 0, newData.Exp, "魔神秘宝升星");
										client.ClientData.MazingerStoreDataInfo.Add(newData.Type, newData);
									}
									res.result = 1;
									res.data = client.ClientData.MazingerStoreDataInfo[newData.Type];
								}
							}
						}
					}
				}
				else if (this.MazingerGrade == null || this.MazingerGradeLevelMax == null || !this.MazingerGrade.ContainsKey(ClientType) || !this.MazingerGradeLevelMax.ContainsKey(ClientType))
				{
					res.result = 3;
				}
				else if (client.ClientData.MazingerStoreDataInfo == null || !client.ClientData.MazingerStoreDataInfo.ContainsKey(ClientType))
				{
					res.result = 11;
				}
				else if (client.ClientData.MazingerStoreDataInfo[ClientType].Stage >= this.MazingerGradeLevelMax[ClientType])
				{
					res.result = 5;
				}
				else
				{
					MazingerStoreData newData = this.CopyMazingerStoreMemData(client, ClientType);
					if (newData == null || !this.MazingerGrade.ContainsKey(newData.Type) || !this.MazingerGrade[newData.Type].ContainsKey(newData.Stage) || this.MazingerGrade[newData.Type][newData.Stage].UseGoods == null || this.MazingerGrade[newData.Type][newData.Stage].UseGoods.Count == 0)
					{
						res.result = 6;
					}
					else
					{
						foreach (KeyValuePair<int, int> iter in this.MazingerGrade[newData.Type][newData.Stage].UseGoods)
						{
							bool EquipBindUse;
							if (!RebornStone.RebornUseGoodHasBinding(client, iter.Key, iter.Value, 1, out EquipBindUse))
							{
								res.result = 7;
								break;
							}
						}
						if (res.result != 7)
						{
							newData.Exp++;
							if (this.MazingerGrade[newData.Type][newData.Stage].LuckyOne + newData.Exp >= 110000)
							{
								newData.Stage++;
								newData.StarLevel = 0;
								newData.Exp = 0;
								up = true;
							}
							else if (this.MazingerGrade[newData.Type][newData.Stage].LuckyOne + newData.Exp > this.MazingerGrade[newData.Type][newData.Stage].LuckyTwo)
							{
								if (Global.GetRandom() < this.MazingerGrade[newData.Type][newData.Stage].Rate)
								{
									newData.Stage++;
									newData.StarLevel = 0;
									newData.Exp = 0;
									up = true;
									res.IsBoom = 1;
								}
							}
							int ret = Global.sendToDB<int, MazingerStoreData>(14126, newData, client.ServerId);
							if (ret < 0)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("魔神秘宝修改数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
								res.result = 10;
							}
							else
							{
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔神秘宝升阶", DateTime.Now.ToString(), newData.Type.ToString(), client.ClientData.RoleName, "系统", ClientType, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
								EventLogManager.AddMazingerStoreEvent(client, client.ClientData.MazingerStoreDataInfo[newData.Type].Stage, newData.Stage, client.ClientData.MazingerStoreDataInfo[newData.Type].Exp, newData.Exp, "魔神秘宝升阶");
								client.ClientData.MazingerStoreDataInfo[newData.Type] = newData;
								res.result = 1;
								res.data = client.ClientData.MazingerStoreDataInfo[newData.Type];
							}
						}
					}
				}
				IL_B64:
				if (up && res.result == 1)
				{
					Global.RefreshEquipPropAndNotify(client);
				}
				result = res;
			}
			return result;
		}

		
		public Dictionary<int, Dictionary<int, MazingerUpGrade>> MazingerGrade = new Dictionary<int, Dictionary<int, MazingerUpGrade>>();

		
		public Dictionary<int, int> MazingerGradeLevelMax = new Dictionary<int, int>();

		
		public Dictionary<int, Dictionary<int, Dictionary<int, MazingerUpStar>>> MazingerStar = new Dictionary<int, Dictionary<int, Dictionary<int, MazingerUpStar>>>();

		
		public Dictionary<int, Dictionary<int, int>> MazingerStarLevelMax = new Dictionary<int, Dictionary<int, int>>();

		
		public List<double> MazingerRate = new List<double>();

		
		private static MazingerStoreManager instance = new MazingerStoreManager();
	}
}
