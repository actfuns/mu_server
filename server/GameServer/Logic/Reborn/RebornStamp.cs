using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Reborn
{
	
	internal class RebornStamp
	{
		
		public static RebornStamp getInstance()
		{
			return RebornStamp.instance;
		}

		
		public static bool ParseYinJiConfig()
		{
			string fileName = Global.GameResPath(RebornStampConsts.RebornStampZhu);
			XElement xml = XElement.Load(fileName);
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
			}
			try
			{
				Dictionary<MainAttrType, Dictionary<int, int>> CurrMainYinJiHot = new Dictionary<MainAttrType, Dictionary<int, int>>();
				Dictionary<MainAttrType, List<int>> TypeMapHot = new Dictionary<MainAttrType, List<int>>();
				Dictionary<int, ChongShengYinJiZhu> MainYinJiHot = new Dictionary<int, ChongShengYinJiZhu>();
				Dictionary<MainAttrType, Dictionary<int, int>> MainYinJiLevelUpHot = new Dictionary<MainAttrType, Dictionary<int, int>>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					ChongShengYinJiZhu temp = new ChongShengYinJiZhu();
					List<int> TypeList = new List<int>();
					Dictionary<int, double> AttrList = new Dictionary<int, double>();
					temp.ItemID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
					temp.MainType = (MainAttrType)Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "TypeZhu"));
					temp.NeedLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "NeedLevel"));
					string[] arrType = Global.GetSafeAttributeStr(xmlItem, "TypeFu").Split(new char[]
					{
						','
					});
					for (int i = 0; i < arrType.Length; i++)
					{
						TypeList.Add(Convert.ToInt32(arrType[i]));
					}
					temp.MinorType = TypeList;
					temp.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Level"));
					string[] arrAttr = Global.GetSafeAttributeStr(xmlItem, "ShuXing").Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < arrAttr.Length; i++)
					{
						AttrList.Add((int)ConfigParser.GetPropIndexByPropName(arrAttr[i].Split(new char[]
						{
							','
						})[0]), Convert.ToDouble(arrAttr[i].Split(new char[]
						{
							','
						})[1]));
					}
					temp.AttrList = AttrList;
					MainYinJiHot.Add(temp.ItemID, temp);
					if (CurrMainYinJiHot.ContainsKey(temp.MainType))
					{
						CurrMainYinJiHot[temp.MainType].Add(temp.Level, temp.ItemID);
					}
					else
					{
						Dictionary<int, int> dict = new Dictionary<int, int>();
						dict.Add(temp.Level, temp.ItemID);
						CurrMainYinJiHot.Add(temp.MainType, dict);
					}
					if (MainYinJiLevelUpHot.ContainsKey(temp.MainType))
					{
						MainYinJiLevelUpHot[temp.MainType].Add(temp.NeedLevel, temp.ItemID);
					}
					else
					{
						Dictionary<int, int> dict = new Dictionary<int, int>();
						dict.Add(temp.NeedLevel, temp.ItemID);
						MainYinJiLevelUpHot.Add(temp.MainType, dict);
					}
					if (!TypeMapHot.ContainsKey(temp.MainType))
					{
						TypeMapHot.Add(temp.MainType, TypeList);
					}
				}
				RebornStamp.CurrMainYinJi = CurrMainYinJiHot;
				RebornStamp.TypeMap = TypeMapHot;
				RebornStamp.MainYinJi = MainYinJiHot;
				RebornStamp.MainYinJiLevelUp = MainYinJiLevelUpHot;
				if (RebornStamp.CurrMainYinJi == null || RebornStamp.TypeMap == null || RebornStamp.MainYinJi == null || RebornStamp.MainYinJiLevelUp == null)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			fileName = Global.GameResPath(RebornStampConsts.RebornStampZi);
			xml = XElement.Load(fileName);
			if (null == xml)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName), null, true);
			}
			try
			{
				Dictionary<int, Dictionary<int, int>> MinorYinJiLevelUpHot = new Dictionary<int, Dictionary<int, int>>();
				Dictionary<int, int> MinorLevelLimitHot = new Dictionary<int, int>();
				Dictionary<int, ChongShengYinJiZi> MinorYinJiHot = new Dictionary<int, ChongShengYinJiZi>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					ChongShengYinJiZi temp2 = new ChongShengYinJiZi();
					List<int> TypeList = new List<int>();
					Dictionary<int, double> AttrList = new Dictionary<int, double>();
					temp2.ItemID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "ID"));
					temp2.MinorType = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Type"));
					temp2.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "Level"));
					string[] arrAttr = Global.GetSafeAttributeStr(xmlItem, "ShuXing").Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < arrAttr.Length; i++)
					{
						string[] str = arrAttr[i].Split(new char[]
						{
							','
						});
						if (str.Length == 1)
						{
							AttrList.Add(0, 0.0);
						}
						else
						{
							AttrList.Add((int)ConfigParser.GetPropIndexByPropName(str[0]), Convert.ToDouble(str[1]));
						}
					}
					temp2.AttrList = AttrList;
					MinorYinJiHot.Add(temp2.ItemID, temp2);
					if (MinorLevelLimitHot.ContainsKey(temp2.MinorType))
					{
						if (MinorLevelLimitHot[temp2.MinorType] < temp2.Level)
						{
							MinorLevelLimitHot[temp2.MinorType] = temp2.Level;
						}
					}
					else
					{
						MinorLevelLimitHot.Add(temp2.MinorType, temp2.Level);
					}
					if (MinorYinJiLevelUpHot.ContainsKey(temp2.MinorType))
					{
						MinorYinJiLevelUpHot[temp2.MinorType].Add(temp2.Level, temp2.ItemID);
					}
					else
					{
						Dictionary<int, int> dict = new Dictionary<int, int>();
						dict.Add(temp2.Level, temp2.ItemID);
						MinorYinJiLevelUpHot.Add(temp2.MinorType, dict);
					}
				}
				RebornStamp.MinorYinJiLevelUp = MinorYinJiLevelUpHot;
				RebornStamp.MinorLevelLimit = MinorLevelLimitHot;
				RebornStamp.MinorYinJi = MinorYinJiHot;
				if (RebornStamp.MinorYinJiLevelUp == null || RebornStamp.MinorLevelLimit == null || RebornStamp.MinorYinJi == null)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			List<int> YinJiResetHot = new List<int>();
			int[] num = GameManager.systemParamsList.GetParamValueIntArrayByName("ChongShengYinJiChongZhi", ',');
			foreach (int it in num)
			{
				YinJiResetHot.Add(it);
			}
			RebornStamp.YinJiReset = YinJiResetHot;
			return RebornStamp.YinJiReset != null;
		}

		
		public static bool CheckTypeMatch(RebornStampData curr, int StampID, int StampType, out int Index, out int MainAttr)
		{
			Index = 0;
			MainAttr = 0;
			bool result;
			if (curr == null || curr.StampInfo == null || curr.StampInfo.Count < 0 || curr.StampInfo.Count > 16)
			{
				result = false;
			}
			else if (!RebornStamp.TypeMap.ContainsKey((MainAttrType)curr.StampInfo[0]) || !RebornStamp.TypeMap.ContainsKey((MainAttrType)curr.StampInfo[8]))
			{
				result = false;
			}
			else if (!RebornStamp.MinorYinJi.ContainsKey(StampID) || RebornStamp.MinorYinJi[StampID].MinorType != StampType)
			{
				result = false;
			}
			else
			{
				bool Exist = false;
				int i = 2;
				int j = 10;
				while (i < 8)
				{
					if (StampType == curr.StampInfo[i])
					{
						Exist = true;
						MainAttr = 1;
						Index = i;
						break;
					}
					if (StampType == curr.StampInfo[j])
					{
						Exist = true;
						MainAttr = 2;
						Index = j;
						break;
					}
					i += 2;
					j += 2;
				}
				if (!Exist)
				{
					result = false;
				}
				else
				{
					int temp = 0;
					bool[] flag = new bool[3];
					if (MainAttr == 1 && RebornStamp.TypeMap.ContainsKey((MainAttrType)curr.StampInfo[0]))
					{
						for (i = 2; i < 8; i += 2)
						{
							foreach (int it in RebornStamp.TypeMap[(MainAttrType)curr.StampInfo[0]])
							{
								if (curr.StampInfo[i] == 0)
								{
									flag[temp] = true;
								}
								else if (curr.StampInfo[i] == it)
								{
									flag[temp] = true;
									break;
								}
							}
							temp++;
						}
					}
					if (MainAttr == 2 && RebornStamp.TypeMap.ContainsKey((MainAttrType)curr.StampInfo[8]))
					{
						for (i = 10; i < 16; i += 2)
						{
							foreach (int it in RebornStamp.TypeMap[(MainAttrType)curr.StampInfo[8]])
							{
								if (curr.StampInfo[i] == 0)
								{
									flag[temp] = true;
								}
								else if (curr.StampInfo[i] == it)
								{
									flag[temp] = true;
									break;
								}
							}
							temp++;
						}
					}
					bool[] array = flag;
					for (int k = 0; k < array.Length; k++)
					{
						if (!array[k])
						{
							Exist = false;
						}
					}
					result = Exist;
				}
			}
			return result;
		}

		
		public static int GetCurrMinorLevelItemID(RebornStampData dbInfo, int Index)
		{
			int result;
			if (dbInfo.StampInfo[Index] == 0)
			{
				result = 0;
			}
			else
			{
				result = RebornStamp.MinorYinJiLevelUp[dbInfo.StampInfo[Index]][dbInfo.StampInfo[Index + 1]];
			}
			return result;
		}

		
		public static int GetCurrMainLevelItemID(RebornStampData dbInfo, int Index)
		{
			int result;
			if (dbInfo.StampInfo[Index] == 0)
			{
				result = 0;
			}
			else
			{
				result = RebornStamp.CurrMainYinJi[(MainAttrType)dbInfo.StampInfo[Index]][dbInfo.StampInfo[Index + 1]];
			}
			return result;
		}

		
		public static int GetMainYinJiLevelUpNum(RebornStampData dbInfo, int Index)
		{
			int MainUpNum = 0;
			int result;
			if (Index != 0 && Index != 8)
			{
				result = MainUpNum;
			}
			else
			{
				int CurrID = RebornStamp.GetCurrMainLevelItemID(dbInfo, Index);
				if (Index == 0)
				{
					foreach (KeyValuePair<int, int> it in RebornStamp.MainYinJiLevelUp[(MainAttrType)dbInfo.StampInfo[Index]])
					{
						if (it.Value > CurrID)
						{
							if (dbInfo.StampInfo[3] >= it.Key && dbInfo.StampInfo[5] >= it.Key && dbInfo.StampInfo[7] >= it.Key)
							{
								MainUpNum++;
							}
							if (MainUpNum > 0 && (dbInfo.StampInfo[3] < it.Key || dbInfo.StampInfo[5] < it.Key || dbInfo.StampInfo[7] < it.Key))
							{
								break;
							}
						}
					}
				}
				else if (Index == 8)
				{
					foreach (KeyValuePair<int, int> it in RebornStamp.MainYinJiLevelUp[(MainAttrType)dbInfo.StampInfo[Index]])
					{
						if (it.Value > CurrID)
						{
							if (dbInfo.StampInfo[11] >= it.Key && dbInfo.StampInfo[13] >= it.Key && dbInfo.StampInfo[15] >= it.Key)
							{
								MainUpNum++;
							}
							if (MainUpNum > 0 && (dbInfo.StampInfo[11] < it.Key || dbInfo.StampInfo[13] < it.Key || dbInfo.StampInfo[15] < it.Key))
							{
								break;
							}
						}
					}
				}
				result = MainUpNum;
			}
			return result;
		}

		
		public static bool IsMainLevelUp(RebornStampData data, int MainAttr, out int ZhuID)
		{
			bool result;
			if (MainAttr == 1)
			{
				int ItemId = RebornStamp.CurrMainYinJi[(MainAttrType)data.StampInfo[0]][data.StampInfo[1]];
				ZhuID = ItemId;
				ChongShengYinJiZhu ZhuData;
				if (!RebornStamp.MainYinJi.TryGetValue(ItemId + 1, out ZhuData))
				{
					result = false;
				}
				else
				{
					lock (RebornStamp.MainYinJi)
					{
						for (int i = 2; i < 8; i += 2)
						{
							if (RebornStamp.MainYinJi[ItemId + 1].NeedLevel > data.StampInfo[i + 1])
							{
								return false;
							}
						}
					}
					result = true;
				}
			}
			else if (MainAttr == 2)
			{
				int ItemId = RebornStamp.CurrMainYinJi[(MainAttrType)data.StampInfo[8]][data.StampInfo[9]];
				ZhuID = ItemId;
				ChongShengYinJiZhu ZhuData;
				if (!RebornStamp.MainYinJi.TryGetValue(ItemId + 1, out ZhuData))
				{
					result = false;
				}
				else
				{
					lock (RebornStamp.MainYinJi)
					{
						for (int i = 10; i < 16; i += 2)
						{
							if (RebornStamp.MainYinJi[ItemId + 1].NeedLevel > data.StampInfo[i + 1])
							{
								return false;
							}
						}
					}
					result = true;
				}
			}
			else
			{
				ZhuID = -1;
				result = false;
			}
			return result;
		}

		
		public void RefreshProps(GameClient client)
		{
			double[] _ExtProps = new double[177];
			try
			{
				if (client.ClientData.RebornYinJi != null)
				{
					if (client.ClientData.RebornYinJi.StampInfo != null)
					{
						int i = 0;
						int j = 8;
						while (i < 8 && j < 16)
						{
							bool flag = false;
							if (i == 0)
							{
								int ItemId = RebornStamp.GetCurrMainLevelItemID(client.ClientData.RebornYinJi, 0);
								if (ItemId == 0)
								{
									break;
								}
								foreach (KeyValuePair<int, double> it in RebornStamp.MainYinJi[ItemId].AttrList)
								{
									_ExtProps[it.Key] += it.Value;
								}
								flag = true;
							}
							if (j == 8)
							{
								int ItemId = RebornStamp.GetCurrMainLevelItemID(client.ClientData.RebornYinJi, 8);
								if (ItemId == 0)
								{
									break;
								}
								foreach (KeyValuePair<int, double> it in RebornStamp.MainYinJi[ItemId].AttrList)
								{
									_ExtProps[it.Key] += it.Value;
								}
								flag = true;
							}
							if (!flag)
							{
								int ItemId = RebornStamp.GetCurrMinorLevelItemID(client.ClientData.RebornYinJi, i);
								if (ItemId == 0)
								{
									break;
								}
								foreach (KeyValuePair<int, double> it in RebornStamp.MinorYinJi[ItemId].AttrList)
								{
									_ExtProps[it.Key] += it.Value;
								}
								ItemId = RebornStamp.GetCurrMinorLevelItemID(client.ClientData.RebornYinJi, j);
								if (ItemId == 0)
								{
									break;
								}
								foreach (KeyValuePair<int, double> it in RebornStamp.MinorYinJi[ItemId].AttrList)
								{
									_ExtProps[it.Key] += it.Value;
								}
							}
							i += 2;
							j += 2;
						}
					}
				}
			}
			finally
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.RebornYinJi,
					_ExtProps
				});
			}
		}

		
		private static bool InsertRebornYinJi(GameClient client, string YinJiInfo)
		{
			string sCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				client.ClientData.RoleID,
				YinJiInfo,
				0,
				0
			});
			string[] dbFields;
			TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14115, sCmd, out dbFields, client.ServerId);
			return retcmd != TCPProcessCmdResults.RESULT_FAILED;
		}

		
		private static List<int> YinJiUpdateInfo(int StampType1, int StampType2)
		{
			List<int> list = new List<int>();
			lock (list)
			{
				if (RebornStamp.TypeMap.ContainsKey((MainAttrType)StampType1))
				{
					list.Add(StampType1);
					list.Add(1);
					foreach (int it in RebornStamp.TypeMap[(MainAttrType)StampType1])
					{
						list.Add(it);
						list.Add(0);
					}
				}
				if (RebornStamp.TypeMap.ContainsKey((MainAttrType)StampType2))
				{
					list.Add(StampType2);
					list.Add(1);
					foreach (int it in RebornStamp.TypeMap[(MainAttrType)StampType2])
					{
						list.Add(it);
						list.Add(0);
					}
				}
			}
			return list;
		}

		
		private static string MakeYinJiUpdateInfo(List<int> UpdateInfo)
		{
			string result;
			if (UpdateInfo.Count != 16)
			{
				result = "";
			}
			else
			{
				string updateInfo = "";
				int temp = 0;
				foreach (int it in UpdateInfo)
				{
					temp++;
					updateInfo += it.ToString();
					if (temp == 8)
					{
						updateInfo += "|";
					}
					else if (temp < UpdateInfo.Count)
					{
						updateInfo += "_";
					}
				}
				result = updateInfo;
			}
			return result;
		}

		
		private static string MakeYinJiUpdateInfoByType(int StampType1, int StampType2)
		{
			return RebornStamp.MakeYinJiUpdateInfo(RebornStamp.YinJiUpdateInfo(StampType1, StampType2));
		}

		
		private static bool UpdateRebornYinJiInfo(GameClient client, string UpdateInfo, int UsePoint, int ResetNum)
		{
			string sCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				client.ClientData.RoleID,
				UpdateInfo,
				UsePoint,
				ResetNum
			});
			string[] dbFields;
			TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14116, sCmd, out dbFields, client.ServerId);
			return retcmd != TCPProcessCmdResults.RESULT_FAILED;
		}

		
		private static bool GetRebornYinJiInfo(GameClient client, out RebornStampData data)
		{
			string sCmd = string.Format("{0}", client.ClientData.RoleID);
			data = Global.sendToDB<RebornStampData, string>(14117, sCmd, client.ServerId);
			return data.RoleID == client.ClientData.RoleID && data != null;
		}

		
		public static ResOpcode ProcessRebornYinJiLevelUp(GameClient client, int RoldID, int StampID, int StampType, int UpNum, out int IDZhu, out int OutMainYinJiID1, out int OutUsePoint)
		{
			IDZhu = -1;
			OutMainYinJiID1 = 0;
			OutUsePoint = 0;
			RebornStampData dbInfo;
			ResOpcode result;
			if (!RebornStamp.GetRebornYinJiInfo(client, out dbInfo))
			{
				result = ResOpcode.ChooseGetInfoYinJiNotActive;
			}
			else
			{
				lock (dbInfo)
				{
					int Index = 0;
					int MainAttr = 0;
					if (!RebornStamp.CheckTypeMatch(dbInfo, StampID, StampType, out Index, out MainAttr))
					{
						return ResOpcode.LevelUpYinJiCheckErr;
					}
					if (UpNum <= 0)
					{
						return ResOpcode.LevelUpYinJiUpNumErr;
					}
					lock (client.ClientData.PropPointMutex)
					{
						long AllPoint = (long)Global.GetRoleParamsInt32FromDB(client, "10246");
						long IsUsePoint = AllPoint - (long)dbInfo.UsePoint;
						if (IsUsePoint < (long)UpNum)
						{
							return ResOpcode.LevelUpYinJiPointErr;
						}
					}
					int IsLevelUpNum = RebornStamp.MinorLevelLimit[StampType] - dbInfo.StampInfo[Index + 1];
					if (0 >= IsLevelUpNum)
					{
						return ResOpcode.LevelUpYinJiMaxLv;
					}
					if (IsLevelUpNum < UpNum)
					{
						return ResOpcode.LevelUpYinJiOverUpLvErr;
					}
					int CurrID = RebornStamp.GetCurrMinorLevelItemID(dbInfo, Index);
					OutMainYinJiID1 = CurrID + UpNum;
					ChongShengYinJiZi data;
					if (!RebornStamp.MinorYinJi.TryGetValue(OutMainYinJiID1, out data))
					{
						return ResOpcode.LevelUpYinJiMaxLv;
					}
					dbInfo.StampInfo[Index + 1] = data.Level;
					if (RebornStamp.IsMainLevelUp(dbInfo, MainAttr, out IDZhu))
					{
						if (MainAttr == 1)
						{
							int MID = RebornStamp.GetCurrMainLevelItemID(dbInfo, 0);
							int LevelUp = MID + RebornStamp.GetMainYinJiLevelUpNum(dbInfo, 0);
							dbInfo.StampInfo[1] = RebornStamp.MainYinJi[LevelUp].Level;
							IDZhu = LevelUp;
						}
						if (MainAttr == 2)
						{
							int MID = RebornStamp.GetCurrMainLevelItemID(dbInfo, 8);
							int LevelUp = MID + RebornStamp.GetMainYinJiLevelUpNum(dbInfo, 8);
							dbInfo.StampInfo[9] = RebornStamp.MainYinJi[LevelUp].Level;
							IDZhu = LevelUp;
						}
					}
					string UpdateInfo = RebornStamp.MakeYinJiUpdateInfo(dbInfo.StampInfo);
					OutUsePoint = dbInfo.UsePoint + UpNum;
					if (!RebornStamp.UpdateRebornYinJiInfo(client, UpdateInfo, OutUsePoint, dbInfo.ResetNum))
					{
						return ResOpcode.LevelUpYinJiSaveErr;
					}
				}
				client.ClientData.RebornYinJi = dbInfo;
				Global.RefreshEquipProp(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				result = ResOpcode.Succ;
			}
			return result;
		}

		
		public static ResOpcode ProcessRebornYinJiGetInfo(GameClient client, int RoldID, out RebornStampData dbInfo)
		{
			dbInfo = null;
			ResOpcode result;
			if (!RebornStamp.GetRebornYinJiInfo(client, out dbInfo))
			{
				result = ResOpcode.GetYinJiInfoErr;
			}
			else
			{
				result = ResOpcode.Succ;
			}
			return result;
		}

		
		public static ResOpcode ProcessRebornYinJiReset(GameClient client, int RoldID)
		{
			RebornStampData db;
			ResOpcode result;
			if (!RebornStamp.GetRebornYinJiInfo(client, out db))
			{
				result = ResOpcode.ChooseGetInfoYinJiNotActive;
			}
			else
			{
				lock (db)
				{
					int UseZuanShiNum;
					if (db.ResetNum < RebornStamp.YinJiReset.Count)
					{
						UseZuanShiNum = RebornStamp.YinJiReset[db.ResetNum];
					}
					else
					{
						UseZuanShiNum = RebornStamp.YinJiReset[RebornStamp.YinJiReset.Count - 1];
					}
					db.ResetNum++;
					if (!GameManager.ClientMgr.SubUserMoney(client, UseZuanShiNum, "重生印记洗点", true, true, true, true, DaiBiSySType.None))
					{
						return ResOpcode.ResetYinJiZuanShiErr;
					}
					if (!RebornStamp.UpdateRebornYinJiInfo(client, "", 0, db.ResetNum))
					{
						return ResOpcode.ResetYinJiInfoErr;
					}
				}
				RebornStampData NewData;
				if (RebornStamp.GetRebornYinJiInfo(client, out NewData))
				{
					client.ClientData.RebornYinJi = NewData;
				}
				Global.RefreshEquipProp(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				result = ResOpcode.Succ;
			}
			return result;
		}

		
		public static ResOpcode ProcessRebornYinJiChoose(GameClient client, int RoldID, int StampType1, int StampType2)
		{
			ResOpcode result;
			if (StampType1 < 1 || StampType1 > 6)
			{
				result = ResOpcode.ChooseYinJiTypeErr;
			}
			else if (StampType2 < 1 || StampType2 > 6)
			{
				result = ResOpcode.ChooseYinJiTypeErr;
			}
			else if (StampType1 == StampType2)
			{
				result = ResOpcode.ChooseYinJiTypeErr;
			}
			else
			{
				RebornStampData db;
				if (RebornStamp.GetRebornYinJiInfo(client, out db))
				{
					lock (db)
					{
						if (db.StampInfo != null && db.StampInfo.Count > 0)
						{
							return ResOpcode.ChooseGetInfoYinJiIsActive;
						}
						if (!RebornStamp.UpdateRebornYinJiInfo(client, RebornStamp.MakeYinJiUpdateInfoByType(StampType1, StampType2), db.UsePoint, db.ResetNum))
						{
							return ResOpcode.ChooseYinJiIsActiveErr;
						}
						RebornStampData NewData;
						if (RebornStamp.GetRebornYinJiInfo(client, out NewData))
						{
							client.ClientData.RebornYinJi = NewData;
						}
						Global.RefreshEquipProp(client);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						return ResOpcode.ChooseGetInfoYinJiIsActive;
					}
				}
				if (!RebornStamp.InsertRebornYinJi(client, RebornStamp.MakeYinJiUpdateInfoByType(StampType1, StampType2)))
				{
					result = ResOpcode.ChooseYinJiIsActiveErr;
				}
				else
				{
					RebornStampData NewData;
					if (RebornStamp.GetRebornYinJiInfo(client, out NewData))
					{
						client.ClientData.RebornYinJi = NewData;
					}
					Global.RefreshEquipProp(client);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					result = ResOpcode.ChooseGetInfoYinJiIsActive;
				}
			}
			return result;
		}

		
		public static Dictionary<int, ChongShengYinJiZhu> MainYinJi = new Dictionary<int, ChongShengYinJiZhu>();

		
		public static Dictionary<MainAttrType, List<int>> TypeMap = new Dictionary<MainAttrType, List<int>>();

		
		public static Dictionary<MainAttrType, Dictionary<int, int>> CurrMainYinJi = new Dictionary<MainAttrType, Dictionary<int, int>>();

		
		public static Dictionary<MainAttrType, Dictionary<int, int>> MainYinJiLevelUp = new Dictionary<MainAttrType, Dictionary<int, int>>();

		
		public static Dictionary<int, ChongShengYinJiZi> MinorYinJi = new Dictionary<int, ChongShengYinJiZi>();

		
		public static Dictionary<int, Dictionary<int, int>> MinorYinJiLevelUp = new Dictionary<int, Dictionary<int, int>>();

		
		public static Dictionary<int, int> MinorLevelLimit = new Dictionary<int, int>();

		
		public static List<int> YinJiReset = new List<int>();

		
		private static RebornStamp instance = new RebornStamp();
	}
}
