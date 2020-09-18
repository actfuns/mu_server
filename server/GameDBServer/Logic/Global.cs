using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Logic.Rank;
using GameDBServer.Server;
using MySQLDriverCS;
using ProtoBuf;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class Global
	{
		
		public static string GetXElementNodePath(XElement element)
		{
			string result;
			try
			{
				string path = element.Name.ToString();
				element = element.Parent;
				while (null != element)
				{
					path = element.Name.ToString() + "/" + path;
					element = element.Parent;
				}
				result = path;
			}
			catch (Exception)
			{
				result = "";
			}
			return result;
		}

		
		public static XElement GetXElement(XElement XML, string newroot)
		{
			return XML.DescendantsAndSelf(newroot).Single<XElement>();
		}

		
		public static XElement GetSafeXElement(XElement XML, string newroot)
		{
			XElement result;
			try
			{
				result = XML.DescendantsAndSelf(newroot).Single<XElement>();
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取: {0} 失败, xml节点名: {1}", newroot, Global.GetXElementNodePath(XML)));
			}
			return result;
		}

		
		public static XElement GetXElement(XElement XML, string newroot, string attribute, string value)
		{
			return XML.DescendantsAndSelf(newroot).Single((XElement X) => X.Attribute(attribute).Value == value);
		}

		
		public static XElement GetSafeXElement(XElement XML, string newroot, string attribute, string value)
		{
			XElement result;
			try
			{
				result = XML.DescendantsAndSelf(newroot).Single((XElement X) => X.Attribute(attribute).Value == value);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取: {0}/{1}={2} 失败, xml节点名: {3}", new object[]
				{
					newroot,
					attribute,
					value,
					Global.GetXElementNodePath(XML)
				}));
			}
			return result;
		}

		
		public static XAttribute GetSafeAttribute(XElement XML, string attribute)
		{
			XAttribute result;
			try
			{
				XAttribute attrib = XML.Attribute(attribute);
				if (null == attrib)
				{
					throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
				}
				result = attrib;
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
			}
			return result;
		}

		
		public static string GetSafeAttributeStr(XElement XML, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, attribute);
			return (string)attrib;
		}

		
		public static long GetSafeAttributeLong(XElement XML, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, attribute);
			string str = (string)attrib;
			long result;
			if (str == null || str == "")
			{
				result = -1L;
			}
			else
			{
				try
				{
					result = (long)Convert.ToDouble(str);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		
		public static double GetSafeAttributeDouble(XElement XML, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, attribute);
			string str = (string)attrib;
			double result;
			if (str == null || str == "")
			{
				result = 0.0;
			}
			else
			{
				try
				{
					result = Convert.ToDouble(str);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		
		public static XAttribute GetSafeAttribute(XElement XML, string root, string attribute)
		{
			XAttribute result;
			try
			{
				XAttribute attrib = XML.Element(root).Attribute(attribute);
				if (null == attrib)
				{
					throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
				}
				result = attrib;
			}
			catch (Exception)
			{
				throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
			}
			return result;
		}

		
		public static string GetSafeAttributeStr(XElement XML, string root, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, root, attribute);
			return (string)attrib;
		}

		
		public static long GetSafeAttributeLong(XElement XML, string root, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, root, attribute);
			string str = (string)attrib;
			long result;
			if (str == null || str == "")
			{
				result = -1L;
			}
			else
			{
				try
				{
					result = (long)Convert.ToDouble(str);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		
		public static double GetSafeAttributeDouble(XElement XML, string root, string attribute)
		{
			XAttribute attrib = Global.GetSafeAttribute(XML, root, attribute);
			string str = (string)attrib;
			double result;
			if (str == null || str == "")
			{
				result = -1.0;
			}
			else
			{
				try
				{
					result = Convert.ToDouble(str);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("读取属性: {0}/{1} 失败, xml节点名: {2}", root, attribute, Global.GetXElementNodePath(XML)));
				}
			}
			return result;
		}

		
		public static void DBRoleInfo2RoleDataEx(DBRoleInfo dbRoleInfo, RoleDataEx roleDataEx)
		{
			lock (dbRoleInfo)
			{
				roleDataEx.PTID = dbRoleInfo.PTID;
				roleDataEx.WorldRoleID = dbRoleInfo.WorldRoleID;
				roleDataEx.Channel = dbRoleInfo.Channel;
				roleDataEx.RoleID = dbRoleInfo.RoleID;
				roleDataEx.RoleName = dbRoleInfo.RoleName;
				roleDataEx.RoleSex = dbRoleInfo.RoleSex;
				roleDataEx.Occupation = dbRoleInfo.Occupation;
				roleDataEx.SubOccupation = dbRoleInfo.SubOccupation;
				roleDataEx.OccupationList = dbRoleInfo.OccupationList;
				roleDataEx.Level = dbRoleInfo.Level;
				roleDataEx.Faction = dbRoleInfo.Faction;
				roleDataEx.Money1 = dbRoleInfo.Money1;
				roleDataEx.Money2 = dbRoleInfo.Money2;
				roleDataEx.Experience = dbRoleInfo.Experience;
				roleDataEx.PKMode = dbRoleInfo.PKMode;
				roleDataEx.PKValue = dbRoleInfo.PKValue;
				string[] fileds = dbRoleInfo.Position.Split(new char[]
				{
					':'
				});
				if (fileds.Length == 4)
				{
					roleDataEx.MapCode = Convert.ToInt32(fileds[0]);
					roleDataEx.RoleDirection = Convert.ToInt32(fileds[1]);
					roleDataEx.PosX = Convert.ToInt32(fileds[2]);
					roleDataEx.PosY = Convert.ToInt32(fileds[3]);
				}
				roleDataEx.LifeV = 0;
				roleDataEx.MagicV = 0;
				roleDataEx.OldTasks = dbRoleInfo.OldTasks;
				roleDataEx.TaskDataList = dbRoleInfo.DoingTaskList;
				roleDataEx.RolePic = dbRoleInfo.RolePic;
				roleDataEx.BagNum = dbRoleInfo.BagNum;
				roleDataEx.TaskDataList = dbRoleInfo.DoingTaskList;
				roleDataEx.GoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 0);
				roleDataEx.RebornGoodsStoreList = Global.GetGoodsDataListBySite(dbRoleInfo, 15001);
				roleDataEx.OtherName = dbRoleInfo.OtherName;
				roleDataEx.MainQuickBarKeys = dbRoleInfo.MainQuickBarKeys;
				roleDataEx.OtherQuickBarKeys = dbRoleInfo.OtherQuickBarKeys;
				roleDataEx.LoginNum = dbRoleInfo.LoginNum;
				roleDataEx.LeftFightSeconds = dbRoleInfo.LeftFightSeconds;
				roleDataEx.FriendDataList = dbRoleInfo.FriendDataList;
				roleDataEx.HorsesDataList = dbRoleInfo.HorsesDataList;
				roleDataEx.HorseDbID = dbRoleInfo.HorseDbID;
				roleDataEx.PetsDataList = dbRoleInfo.PetsDataList;
				roleDataEx.PetDbID = dbRoleInfo.PetDbID;
				roleDataEx.InterPower = dbRoleInfo.InterPower;
				roleDataEx.JingMaiDataList = dbRoleInfo.JingMaiDataList;
				roleDataEx.DJPoint = ((dbRoleInfo.RoleDJPointData != null) ? dbRoleInfo.RoleDJPointData.DJPoint : 0);
				roleDataEx.DJTotal = ((dbRoleInfo.RoleDJPointData != null) ? dbRoleInfo.RoleDJPointData.Total : 0);
				roleDataEx.DJWincnt = ((dbRoleInfo.RoleDJPointData != null) ? dbRoleInfo.RoleDJPointData.Wincnt : 0);
				roleDataEx.TotalOnlineSecs = Math.Max(0, dbRoleInfo.TotalOnlineSecs);
				roleDataEx.AntiAddictionSecs = Math.Max(0, dbRoleInfo.AntiAddictionSecs);
				roleDataEx.BiGuanTime = dbRoleInfo.BiGuanTime;
				roleDataEx.YinLiang = dbRoleInfo.YinLiang;
				roleDataEx.SkillDataList = dbRoleInfo.SkillDataList;
				roleDataEx.TotalJingMaiExp = dbRoleInfo.TotalJingMaiExp;
				roleDataEx.JingMaiExpNum = dbRoleInfo.JingMaiExpNum;
				roleDataEx.RegTime = DataHelper.ConvertToTicks(dbRoleInfo.RegTime);
				roleDataEx.LastHorseID = dbRoleInfo.LastHorseID;
				roleDataEx.SaleGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, -1);
				roleDataEx.DefaultSkillID = dbRoleInfo.DefaultSkillID;
				roleDataEx.AutoLifeV = dbRoleInfo.AutoLifeV;
				roleDataEx.AutoMagicV = dbRoleInfo.AutoMagicV;
				roleDataEx.BufferDataList = dbRoleInfo.BufferDataList;
				roleDataEx.MyDailyTaskDataList = dbRoleInfo.MyDailyTaskDataList;
				roleDataEx.MyDailyJingMaiData = dbRoleInfo.MyDailyJingMaiData;
				roleDataEx.NumSkillID = dbRoleInfo.NumSkillID;
				roleDataEx.MyPortableBagData = dbRoleInfo.MyPortableBagData;
				roleDataEx.RebornGirdData = dbRoleInfo.RebornGirdData;
				roleDataEx.MyHuodongData = dbRoleInfo.MyHuodongData;
				roleDataEx.FuBenDataList = dbRoleInfo.FuBenDataList;
				roleDataEx.MainTaskID = dbRoleInfo.MainTaskID;
				roleDataEx.PKPoint = dbRoleInfo.PKPoint;
				roleDataEx.LianZhan = dbRoleInfo.LianZhan;
				roleDataEx.MyRoleDailyData = dbRoleInfo.MyRoleDailyData;
				roleDataEx.KillBoss = dbRoleInfo.KillBoss;
				roleDataEx.MyYaBiaoData = dbRoleInfo.MyYaBiaoData;
				roleDataEx.BattleNameStart = dbRoleInfo.BattleNameStart;
				roleDataEx.BattleNameIndex = dbRoleInfo.BattleNameIndex;
				roleDataEx.CZTaskID = dbRoleInfo.CZTaskID;
				roleDataEx.BattleNum = dbRoleInfo.BattleNum;
				roleDataEx.HeroIndex = dbRoleInfo.HeroIndex;
				roleDataEx.ZoneID = dbRoleInfo.ZoneID;
				roleDataEx.BHName = dbRoleInfo.BHName;
				roleDataEx.BHVerify = dbRoleInfo.BHVerify;
				roleDataEx.BHZhiWu = dbRoleInfo.BHZhiWu;
				roleDataEx.BGDayID1 = dbRoleInfo.BGDayID1;
				roleDataEx.BGMoney = dbRoleInfo.BGMoney;
				roleDataEx.BGDayID2 = dbRoleInfo.BGDayID2;
				roleDataEx.BGGoods = dbRoleInfo.BGGoods;
				roleDataEx.BangGong = dbRoleInfo.BangGong;
				roleDataEx.HuangHou = dbRoleInfo.HuangHou;
				roleDataEx.PaiHangPosDict = PaiHangManager.CalcPaiHangPosDictRoleID(dbRoleInfo.RoleID);
				roleDataEx.JieBiaoDayID = dbRoleInfo.JieBiaoDayID;
				roleDataEx.JieBiaoDayNum = dbRoleInfo.JieBiaoDayNum;
				roleDataEx.VipDailyDataList = dbRoleInfo.VipDailyDataList;
				roleDataEx.YangGongBKDailyJiFen = dbRoleInfo.YangGongBKDailyJiFen;
				roleDataEx.LastMailID = dbRoleInfo.LastMailID;
				roleDataEx.OnceAwardFlag = dbRoleInfo.OnceAwardFlag;
				roleDataEx.Gold = dbRoleInfo.Gold;
				roleDataEx.BanChat = dbRoleInfo.BanChat;
				roleDataEx.BanLogin = dbRoleInfo.BanLogin;
				roleDataEx.IsFlashPlayer = dbRoleInfo.IsFlashPlayer;
				roleDataEx.ChangeLifeCount = dbRoleInfo.ChangeLifeCount;
				roleDataEx.AdmiredCount = dbRoleInfo.AdmiredCount;
				roleDataEx.CombatForce = dbRoleInfo.CombatForce;
				roleDataEx.AutoAssignPropertyPoint = dbRoleInfo.AutoAssignPropertyPoint;
				roleDataEx.GoodsLimitDataList = dbRoleInfo.GoodsLimitDataList;
				roleDataEx.RoleParamsDict = dbRoleInfo.RoleParamsDict;
				roleDataEx.MyPortableBagData.GoodsUsedGridNum = Global.GetGoodsDataCountBySite(dbRoleInfo, -1000);
				roleDataEx.RebornGirdData.GoodsUsedGridNum = Global.GetGoodsDataCountBySite(dbRoleInfo, 15001);
				if (dbRoleInfo.RebornBagNum < 50)
				{
					roleDataEx.RebornBagNum = 50;
					DBManager dbMgr = DBManager.getInstance();
					DBWriter.UpdateRoleRebornBagNum(dbMgr, roleDataEx.RoleID, 50);
				}
				else
				{
					roleDataEx.RebornBagNum = dbRoleInfo.RebornBagNum;
				}
				if (dbRoleInfo.RebornGirdData.ExtGridNum < 60)
				{
					roleDataEx.RebornGirdData.ExtGridNum = 60;
					DBManager dbMgr = DBManager.getInstance();
					DBWriter.UpdateRoleRebornStorageInfo(dbMgr, roleDataEx.RoleID, 60);
				}
				roleDataEx.RebornShowEquip = dbRoleInfo.RebornShowEquip;
				roleDataEx.RebornShowModel = dbRoleInfo.RebornShowModel;
				roleDataEx.ZhanDuiID = dbRoleInfo.ZhanDuiID;
				roleDataEx.ZhanDuiZhiWu = dbRoleInfo.ZhanDuiZhiWu;
				long ticks = DateTime.Now.Ticks / 10000L;
				int restartSecs = GameDBManager.GameConfigMgr.GetGameConfigItemInt("anti-addiction-restart", 18000);
				if ((ticks - dbRoleInfo.LogOffTime) / 1000L >= (long)restartSecs)
				{
					roleDataEx.AntiAddictionSecs = 0;
				}
				roleDataEx.LastOfflineTime = dbRoleInfo.LogOffTime;
				dbRoleInfo.UpdateDBPositionTicks = ticks;
				dbRoleInfo.UpdateDBTimeTicks = ticks;
				dbRoleInfo.UpdateDBInterPowerTimeTicks = ticks;
				roleDataEx.MyWingData = dbRoleInfo.MyWingData;
				roleDataEx.RolePictureJudgeReferInfo = dbRoleInfo.PictureJudgeReferInfo;
				roleDataEx.RoleStarConstellationInfo = dbRoleInfo.StarConstellationInfo;
				roleDataEx.VIPLevel = Global.GetRoleParamsInt32(dbRoleInfo, "VIPExp");
				roleDataEx.ElementhrtsList = Global.GetGoodsDataListBySite(dbRoleInfo, 3000);
				roleDataEx.UsingElementhrtsList = Global.GetGoodsDataListBySite(dbRoleInfo, 3001);
				roleDataEx.PetList = Global.GetGoodsDataListBySite(dbRoleInfo, 4000);
				roleDataEx.DamonGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 5000);
				roleDataEx.Store_Yinliang = dbRoleInfo.store_yinliang;
				roleDataEx.Store_Money = dbRoleInfo.store_money;
				roleDataEx.LingYuDict = dbRoleInfo.LingYuDict;
				roleDataEx.MagicSwordParam = dbRoleInfo.MagicSwordParam;
				roleDataEx.MyMarriageData = dbRoleInfo.MyMarriageData;
				roleDataEx.MyMarryPartyJoinList = dbRoleInfo.MyMarryPartyJoinList;
				roleDataEx.GroupMailRecordList = dbRoleInfo.GroupMailRecordList;
				roleDataEx.MyGuardStatueDetail = dbRoleInfo.MyGuardStatueDetail;
				roleDataEx.MyHolyItemDataDic = dbRoleInfo.MyHolyItemDataDic;
				roleDataEx.MyTalentData = dbRoleInfo.MyTalentData;
				roleDataEx.TianTiData = dbRoleInfo.TianTiData;
				roleDataEx.FashionGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 6000);
				roleDataEx.OrnamentGoodsList = Global.GetGoodsDataListBySite(dbRoleInfo, 9000);
				roleDataEx.OrnamentDataDict = dbRoleInfo.OrnamentDataDict;
				roleDataEx.MerlinData = dbRoleInfo.MerlinData;
				if (null == roleDataEx.FluorescentGemData)
				{
					roleDataEx.FluorescentGemData = new FluorescentGemData();
				}
				roleDataEx.FluorescentGemData.GemBagList = Global.GetGoodsDataListBySite(dbRoleInfo, 7000);
				roleDataEx.FluorescentGemData.GemEquipList = dbRoleInfo.FluorescentGemData.GemEquipList;
				roleDataEx.FluorescentPoint = dbRoleInfo.FluorescentPoint;
				roleDataEx.BuildingDataList = dbRoleInfo.BuildingDataList;
				roleDataEx.SevenDayActDict = dbRoleInfo.SevenDayActDict;
				roleDataEx.SoulStonesInBag = Global.GetGoodsDataListBySite(dbRoleInfo, 8000);
				roleDataEx.SoulStonesInUsing = Global.GetGoodsDataListBySite(dbRoleInfo, 8001);
				roleDataEx.BanTradeToTicks = dbRoleInfo.BanTradeToTicks;
				roleDataEx.SpecActInfoDict = dbRoleInfo.SpecActInfoDict;
				roleDataEx.TarotData = dbRoleInfo.TarotData;
				roleDataEx.JunTuanZhiWu = dbRoleInfo.JunTuanZhiWu;
				roleDataEx.PaiZhuDamonGoodsDataList = Global.GetGoodsDataListBySiteRange(dbRoleInfo, 10000, 10001);
				roleDataEx.ShenJiDict = dbRoleInfo.ShenJiDict;
				roleDataEx.EverydayActInfoDict = dbRoleInfo.EverydayActInfoDict;
				roleDataEx.HuiJiData = dbRoleInfo.HuiJiData;
				roleDataEx.ArmorData = dbRoleInfo.ArmorData;
				roleDataEx.UserID = dbRoleInfo.UserID;
				roleDataEx.FuWenTabList = dbRoleInfo.FuWenTabList;
				roleDataEx.FuWenGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 11000);
				roleDataEx.AlchemyInfo = dbRoleInfo.AlchemyInfo;
				roleDataEx.JingLingYuanSuJueXingData = dbRoleInfo.JingLingYuanSuJueXingData;
				roleDataEx.BianShenData = dbRoleInfo.BianShenData;
				roleDataEx.RebornGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 15000);
				roleDataEx.RebornCount = Global.GetRoleParamsInt32(dbRoleInfo, "10240");
				roleDataEx.RebornLevel = Global.GetRoleParamsInt32(dbRoleInfo, "10241");
				roleDataEx.RebornExperience = Global.GetRoleParamsInt64(dbRoleInfo, "10242");
				roleDataEx.RebornYinJi = RebornStampManager.GetUserRebornInfoFromCached(dbRoleInfo.RoleID);
				roleDataEx.SpecPriorityActInfoDict = dbRoleInfo.SpecPriorityActInfoDict;
				roleDataEx.HolyGoodsDataList = Global.GetGoodsDataListBySite(dbRoleInfo, 16000);
				roleDataEx.RebornEquipHoleData = RebornEquip.GetRebornEquipHoleData(dbRoleInfo);
				roleDataEx.MazingerStoreDataInfo = RebornEquip.GetMazingerStoreData(dbRoleInfo);
			}
		}

		
		public static void DBRoleInfo2RoleData4Selector(DBManager dbMgr, DBRoleInfo dbRoleInfo, RoleData4Selector roleData4Selector)
		{
			lock (dbRoleInfo)
			{
				roleData4Selector.RoleID = dbRoleInfo.RoleID;
				roleData4Selector.RoleName = dbRoleInfo.RoleName;
				roleData4Selector.RoleSex = dbRoleInfo.RoleSex;
				roleData4Selector.Occupation = dbRoleInfo.Occupation;
				roleData4Selector.Level = dbRoleInfo.Level;
				roleData4Selector.Faction = dbRoleInfo.Faction;
				roleData4Selector.MyWingData = dbRoleInfo.MyWingData;
				roleData4Selector.GoodsDataList = Global.GetUsingGoodsDataList(dbRoleInfo);
				roleData4Selector.OtherName = dbRoleInfo.OtherName;
				roleData4Selector.CombatForce = dbRoleInfo.CombatForce;
				roleData4Selector.AdmiredCount = dbRoleInfo.AdmiredCount;
				roleData4Selector.ZoneId = dbRoleInfo.ZoneID;
				roleData4Selector.OccupationList = dbRoleInfo.OccupationList;
				if (!int.TryParse(DBQuery.GetRoleParamByName(dbMgr, dbRoleInfo.RoleID, "FashionWingsID"), out roleData4Selector.FashionWingsID))
				{
					roleData4Selector.FashionWingsID = 0;
				}
				if (!long.TryParse(DBQuery.GetRoleParamByName(dbMgr, dbRoleInfo.RoleID, "SettingBitFlags"), out roleData4Selector.SettingBitFlags))
				{
					roleData4Selector.SettingBitFlags = 0L;
				}
				if (!int.TryParse(DBQuery.GetRoleParamByName(dbMgr, dbRoleInfo.RoleID, "10213"), out roleData4Selector.SubOccupation))
				{
					roleData4Selector.SubOccupation = 0;
				}
				roleData4Selector.HuiJiData = dbRoleInfo.HuiJiData;
			}
		}

		
		public static LineData LineItemToLineData(LineItem lineItem)
		{
			return new LineData
			{
				LineID = lineItem.LineID,
				GameServerIP = lineItem.GameServerIP,
				GameServerPort = lineItem.GameServerPort,
				OnlineCount = lineItem.OnlineCount
			};
		}

		
		public static int SafeConvertToInt32(string str, int fromBase = 10)
		{
			str = str.Trim();
			int result;
			if (string.IsNullOrEmpty(str))
			{
				result = 0;
			}
			else
			{
				try
				{
					return Convert.ToInt32(str, fromBase);
				}
				catch (Exception)
				{
				}
				result = 0;
			}
			return result;
		}

		
		public static long SafeConvertToInt64(string str, int fromBase = 10)
		{
			str = str.Trim();
			long result;
			if (string.IsNullOrEmpty(str))
			{
				result = 0L;
			}
			else
			{
				try
				{
					return Convert.ToInt64(str, fromBase);
				}
				catch (Exception)
				{
				}
				result = 0L;
			}
			return result;
		}

		
		public static DateTime SafeConvertToDateTime(string str, DateTime defValue)
		{
			str = str.Trim();
			DateTime result;
			if (string.IsNullOrEmpty(str))
			{
				result = defValue;
			}
			else
			{
				try
				{
					DateTime dt;
					if (DateTime.TryParse(str, out dt))
					{
						return dt;
					}
				}
				catch (Exception)
				{
				}
				result = defValue;
			}
			return result;
		}

		
		public static double GetOffsetSecond(DateTime date)
		{
			double temp = (date - DateTime.Parse("2011-11-11")).TotalMilliseconds;
			return temp / 1000.0;
		}

		
		public static long GetOffsetMinute(DateTime date)
		{
			return (long)(Global.GetOffsetSecond(date) / 60.0);
		}

		
		public static int GetOffsetDay(DateTime now)
		{
			double temp = (now - DateTime.Parse("2011-11-11")).TotalMilliseconds;
			return (int)(temp / 1000.0 / 60.0 / 60.0 / 24.0);
		}

		
		public static string GetDayStartTime(DateTime now)
		{
			return Global.GetDateTimeString(now.Date);
		}

		
		public static string GetDayEndTime(DateTime now)
		{
			return Global.GetDateTimeString(now.Date.AddDays(1.0));
		}

		
		public static string GetDateTimeString(DateTime now)
		{
			return now.ToString("yyyy-MM-dd HH:mm:ss");
		}

		
		public static DateTime GetRealDate(int day)
		{
			DateTime startDay = DateTime.Parse("2011-11-11");
			return Global.GetAddDaysDataTime(startDay, day, true);
		}

		
		public static string GetSocketRemoteEndPoint(Socket s)
		{
			try
			{
				return string.Format("{0} ", s.RemoteEndPoint);
			}
			catch (Exception)
			{
			}
			return "";
		}

		
		public static string GetDebugHelperInfo(Socket socket)
		{
			string result;
			if (null == socket)
			{
				result = "socket为null, 无法打印错误信息";
			}
			else
			{
				string ret = "";
				try
				{
					ret += string.Format("IP={0} ", Global.GetSocketRemoteEndPoint(socket));
				}
				catch (Exception)
				{
				}
				result = ret;
			}
			return result;
		}

		
		public static List<GoodsData> GetGoodsDataListBySite(DBRoleInfo dbRoleInfo, int site)
		{
			List<GoodsData> goodsDataList = new List<GoodsData>();
			List<GoodsData> result;
			if (null == dbRoleInfo.GoodsDataList)
			{
				result = goodsDataList;
			}
			else
			{
				for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
				{
					if (dbRoleInfo.GoodsDataList[i].Site == site)
					{
						goodsDataList.Add(dbRoleInfo.GoodsDataList[i]);
					}
				}
				result = goodsDataList;
			}
			return result;
		}

		
		public static List<GoodsData> GetGoodsDataListBySiteRange(DBRoleInfo dbRoleInfo, int siteBegin, int siteEnd)
		{
			List<GoodsData> goodsDataList = new List<GoodsData>();
			List<GoodsData> result;
			if (null == dbRoleInfo.GoodsDataList)
			{
				result = goodsDataList;
			}
			else
			{
				for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
				{
					if (dbRoleInfo.GoodsDataList[i].Site >= siteBegin && dbRoleInfo.GoodsDataList[i].Site <= siteEnd)
					{
						goodsDataList.Add(dbRoleInfo.GoodsDataList[i]);
					}
				}
				result = goodsDataList;
			}
			return result;
		}

		
		public static int GetGoodsDataCountBySite(DBRoleInfo dbRoleInfo, int site)
		{
			int result;
			if (null == dbRoleInfo.GoodsDataList)
			{
				result = 0;
			}
			else
			{
				int count = 0;
				for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
				{
					if (dbRoleInfo.GoodsDataList[i].Site == site)
					{
						count++;
					}
				}
				result = count;
			}
			return result;
		}

		
		public static GoodsData GetGoodsDataByDbID(DBRoleInfo dbRoleInfo, int goodsDbID)
		{
			lock (dbRoleInfo)
			{
				if (null != dbRoleInfo.GoodsDataList)
				{
					for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
					{
						if (dbRoleInfo.GoodsDataList[i].Id == goodsDbID)
						{
							return dbRoleInfo.GoodsDataList[i];
						}
					}
				}
			}
			return null;
		}

		
		public static List<GoodsData> GetUsingGoodsDataList(DBRoleInfo dbRoleInfo)
		{
			List<GoodsData> usingGoodsDataList = new List<GoodsData>();
			List<GoodsData> result;
			if (null == dbRoleInfo.GoodsDataList)
			{
				result = usingGoodsDataList;
			}
			else
			{
				for (int i = 0; i < dbRoleInfo.GoodsDataList.Count; i++)
				{
					if (dbRoleInfo.GoodsDataList[i].Using > 0 && dbRoleInfo.GoodsDataList[i].Site != 15000)
					{
						usingGoodsDataList.Add(dbRoleInfo.GoodsDataList[i]);
					}
				}
				result = usingGoodsDataList;
			}
			return result;
		}

		
		public static void UpdateGoodsLimitByID(DBRoleInfo dbRoleInfo, int goodsID, int dayID, int usedNum)
		{
			lock (dbRoleInfo)
			{
				if (dbRoleInfo.GoodsLimitDataList == null)
				{
					dbRoleInfo.GoodsLimitDataList = new List<GoodsLimitData>();
				}
				int findIndex = -1;
				for (int i = 0; i < dbRoleInfo.GoodsLimitDataList.Count; i++)
				{
					if (dbRoleInfo.GoodsLimitDataList[i].GoodsID == goodsID)
					{
						findIndex = i;
						dbRoleInfo.GoodsLimitDataList[i].DayID = dayID;
						dbRoleInfo.GoodsLimitDataList[i].UsedNum = usedNum;
						break;
					}
				}
				if (-1 == findIndex)
				{
					GoodsLimitData goodsLimitData = new GoodsLimitData
					{
						GoodsID = goodsID,
						DayID = dayID,
						UsedNum = usedNum
					};
					dbRoleInfo.GoodsLimitDataList.Add(goodsLimitData);
				}
			}
		}

		
		public static HorseData GetHorseDataByDbID(DBRoleInfo dbRoleInfo, int dbID)
		{
			HorseData result;
			if (null == dbRoleInfo)
			{
				result = null;
			}
			else
			{
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.HorsesDataList)
					{
						for (int i = 0; i < dbRoleInfo.HorsesDataList.Count; i++)
						{
							if (dbRoleInfo.HorsesDataList[i].DbID == dbID)
							{
								return dbRoleInfo.HorsesDataList[i];
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		
		public static PetData GetPetDataByDbID(DBRoleInfo dbRoleInfo, int dbID)
		{
			PetData result;
			if (null == dbRoleInfo)
			{
				result = null;
			}
			else
			{
				lock (dbRoleInfo)
				{
					if (null != dbRoleInfo.PetsDataList)
					{
						for (int i = 0; i < dbRoleInfo.PetsDataList.Count; i++)
						{
							if (dbRoleInfo.PetsDataList[i].DbID == dbID)
							{
								return dbRoleInfo.PetsDataList[i];
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		
		public static DJPointData GetRoleDJPointData(DBRoleInfo dbRoleInfo)
		{
			DJPointData djPointData = null;
			lock (dbRoleInfo)
			{
				djPointData = new DJPointData
				{
					DbID = dbRoleInfo.RoleDJPointData.DbID,
					RoleID = dbRoleInfo.RoleDJPointData.RoleID,
					DJPoint = dbRoleInfo.RoleDJPointData.DJPoint,
					Total = dbRoleInfo.RoleDJPointData.Total,
					Wincnt = dbRoleInfo.RoleDJPointData.Wincnt,
					Yestoday = dbRoleInfo.RoleDJPointData.Yestoday,
					Lastweek = dbRoleInfo.RoleDJPointData.Lastweek,
					Lastmonth = dbRoleInfo.RoleDJPointData.Lastmonth,
					Dayupdown = dbRoleInfo.RoleDJPointData.Dayupdown,
					Weekupdown = dbRoleInfo.RoleDJPointData.Weekupdown,
					Monthupdown = dbRoleInfo.RoleDJPointData.Monthupdown
				};
			}
			return djPointData;
		}

		
		public static int GetRoleOnlineState(DBRoleInfo dbRoleInfo)
		{
			int result;
			if (null == dbRoleInfo)
			{
				result = 0;
			}
			else if (dbRoleInfo.ServerLineID <= 0)
			{
				result = 0;
			}
			else
			{
				result = 1;
			}
			return result;
		}

		
		public static int GetUserOnlineState(DBUserInfo dbUserInfo)
		{
			int result;
			if (null == dbUserInfo)
			{
				result = 0;
			}
			else
			{
				result = UserOnlineManager.GetUserOnlineState(dbUserInfo.UserID);
			}
			return result;
		}

		
		public static DBRoleInfo FindOnlineRoleInfoByUserInfo(DBManager dbMgr, DBUserInfo dbUserInfo)
		{
			DBRoleInfo result;
			if (null == dbUserInfo)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < dbUserInfo.ListRoleIDs.Count; i++)
				{
					int roleID = dbUserInfo.ListRoleIDs[i];
					DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
					if (null != dbRoleInfo)
					{
						if (dbRoleInfo.ServerLineID > 0)
						{
							return dbRoleInfo;
						}
					}
				}
				result = null;
			}
			return result;
		}

		
		public static bool IsGameServerClientOnline(int lineId)
		{
			GameServerClient client = LineManager.GetGameServerClient(lineId);
			return client != null && client.CurrentSocket != null && client == TCPManager.getInstance().getClient(client.CurrentSocket);
		}

		
		public static string FormatRoleName(DBRoleInfo dbRoleInfo)
		{
			return Global.FormatRoleName(dbRoleInfo.ZoneID, dbRoleInfo.RoleName);
		}

		
		public static string FormatRoleName(string zoneID, string roleName)
		{
			return roleName;
		}

		
		public static string FormatRoleName(int zoneID, string roleName)
		{
			return roleName;
		}

		
		public static string FormatBangHuiName(int zoneID, string bhName)
		{
			return bhName;
		}

		
		public static int GetDBRoleInfoByZhiWu(List<BangHuiMgrItemData> bangHuiMgrItemDataList, int zhiWu)
		{
			int result;
			if (bangHuiMgrItemDataList == null || bangHuiMgrItemDataList.Count <= 0)
			{
				result = 0;
			}
			else
			{
				for (int i = 0; i < bangHuiMgrItemDataList.Count; i++)
				{
					if (bangHuiMgrItemDataList[i].BHZhiwu == zhiWu)
					{
						return bangHuiMgrItemDataList[i].RoleID;
					}
				}
				result = 0;
			}
			return result;
		}

		
		public static void WriteRoleInfoLog(DBManager dbMgr, DBRoleInfo dbRoleInfo)
		{
			try
			{
				if (null != dbRoleInfo)
				{
					string userID = dbRoleInfo.UserID;
					DBUserInfo dbUserInfo = dbMgr.GetDBUserInfo(userID);
					if (null != dbUserInfo)
					{
						int userMoney = dbUserInfo.Money;
					}
					if (!string.IsNullOrEmpty(dbRoleInfo.Position))
					{
						string[] fileds = dbRoleInfo.Position.Split(new char[]
						{
							':'
						});
						if (fileds.Length == 4)
						{
							int mapCode = Convert.ToInt32(fileds[0]);
						}
					}
					string lastTime = new DateTime(dbRoleInfo.LastTime * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
					string logOffTime = new DateTime(dbRoleInfo.LogOffTime * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
				}
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("将角色的数据写入二进制日志出错，RoleID={0}", dbRoleInfo.RoleID), null, true);
			}
		}

		
		public static void LoadLangDict()
		{
			XElement xml = null;
			try
			{
				xml = XElement.Load("Language.xml");
			}
			catch (Exception)
			{
				return;
			}
			try
			{
				if (null != xml)
				{
					Dictionary<string, string> langDict = new Dictionary<string, string>();
					IEnumerable<XElement> langItems = xml.Elements();
					foreach (XElement langItem in langItems)
					{
						langDict[Global.GetSafeAttributeStr(langItem, "ChineseText")] = Global.GetSafeAttributeStr(langItem, "OtherLangText");
					}
					Global.LangDict = langDict;
				}
			}
			catch (Exception)
			{
			}
		}

		
		public static string GetLang(string chineseText)
		{
			string result;
			if (null == Global.LangDict)
			{
				result = chineseText;
			}
			else
			{
				string otherLangText = "";
				if (!Global.LangDict.TryGetValue(chineseText, out otherLangText))
				{
					result = chineseText;
				}
				else if (string.IsNullOrEmpty(otherLangText))
				{
					result = chineseText;
				}
				else
				{
					result = otherLangText;
				}
			}
			return result;
		}

		
		public static List<MailData> LoadUserMailItemDataList(DBManager dbMgr, int rid)
		{
			DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref rid);
			if (null != dbRoleInfo)
			{
				dbRoleInfo.LastMailID = 0;
			}
			return DBQuery.GetMailItemDataList(dbMgr, rid);
		}

		
		public static int LoadUserMailItemDataCount(DBManager dbMgr, int rid, int excludeReadState = 0, int limitCount = 1)
		{
			return DBQuery.GetMailItemDataCount(dbMgr, rid, excludeReadState, limitCount);
		}

		
		public static MailData LoadMailItemData(DBManager dbMgr, int rid, int mailID)
		{
			MailData mailData = DBQuery.GetMailItemData(dbMgr, rid, mailID);
			if (null != mailData)
			{
				if (mailData.IsRead != 1)
				{
					DBWriter.UpdateMailHasReadFlag(dbMgr, mailID, rid);
					mailData.IsRead = 1;
					mailData.ReadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				}
			}
			return mailData;
		}

		
		public static bool UpdateHasReadMailFlag(DBManager dbMgr, int rid, int mailID)
		{
			bool ret = true;
			DBWriter.UpdateMailHasReadFlag(dbMgr, mailID, rid);
			return ret;
		}

		
		public static bool UpdateHasFetchMailGoodsStat(DBManager dbMgr, int rid, int mailID)
		{
			bool ret = DBWriter.UpdateMailHasFetchGoodsFlag(dbMgr, mailID, rid);
			DBWriter.UpdateMailHasReadFlag(dbMgr, mailID, rid);
			return ret;
		}

		
		public static bool DeleteMail(DBManager dbMgr, int rid, string mailIDs)
		{
			bool result = false;
			string[] mailidArr = mailIDs.Split(new char[]
			{
				'|'
			});
			foreach (string strID in mailidArr)
			{
				try
				{
					int mailID = int.Parse(strID);
					bool ret = DBWriter.DeleteMailDataItemExcludeGoodsList(dbMgr, mailID, rid);
					if (ret)
					{
						DBWriter.DeleteMailIDInMailTemp(dbMgr, mailID);
						DBWriter.DeleteMailGoodsList(dbMgr, mailID);
						result = ret;
					}
				}
				catch
				{
				}
			}
			return result;
		}

		
		public static int AddMail(DBManager dbMgr, string[] fields, out int addGoodsCount)
		{
			int senderrid = Convert.ToInt32(fields[0]);
			string senderrname = fields[1];
			int receiverrid = Convert.ToInt32(fields[2]);
			string reveiverrname = fields[3];
			string subject = fields[4];
			string content = fields[5];
			int yinliang = Convert.ToInt32(fields[6]);
			int tongqian = Convert.ToInt32(fields[7]);
			int yuanbao = Convert.ToInt32(fields[8]);
			string goodslist = fields[9];
			if (reveiverrname == "")
			{
				string uid = "";
				Global.GetRoleNameAndUserID(dbMgr, receiverrid, out reveiverrname, out uid);
			}
			senderrname = senderrname.Replace('$', ':');
			reveiverrname = reveiverrname.Replace('$', ':');
			subject = subject.Replace('$', ':');
			content = content.Replace('$', ':');
			addGoodsCount = 0;
			DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref senderrid);
			if (null != dbRoleInfo)
			{
				senderrname = Global.FormatRoleName(dbRoleInfo);
			}
			int mailID = DBWriter.AddMailBody(dbMgr, senderrid, senderrname, receiverrid, reveiverrname, subject, content, yinliang, tongqian, yuanbao);
			if (mailID >= 0)
			{
				addGoodsCount = Global.AddMailGoods(dbMgr, mailID, goodslist.Split(new char[]
				{
					'|'
				}));
				DBWriter.UpdateLastScanMailID(dbMgr, receiverrid, mailID);
			}
			return mailID;
		}

		
		private static int AddMailGoods(DBManager dbMgr, int mailid, string[] goodsArr)
		{
			int result;
			if (goodsArr == null || goodsArr.Length <= 0)
			{
				result = 0;
			}
			else
			{
				int addCount = 0;
				for (int i = 0; i < goodsArr.Length; i++)
				{
					string[] goods = goodsArr[i].Split(new char[]
					{
						'_'
					});
					if (16 == goods.Length)
					{
						if (DBWriter.AddMailGoodsDataItem(dbMgr, mailid, int.Parse(goods[0]), int.Parse(goods[1]), int.Parse(goods[2]), goods[3], int.Parse(goods[4]), int.Parse(goods[5]), int.Parse(goods[6]), goods[7], int.Parse(goods[8]), int.Parse(goods[9]), int.Parse(goods[10]), int.Parse(goods[11]), int.Parse(goods[12]), int.Parse(goods[13]), int.Parse(goods[14]), int.Parse(goods[15])))
						{
							addCount++;
						}
					}
				}
				result = addCount;
			}
			return result;
		}

		
		public static int FindDBRoleID(DBManager dbMgr, string roleName)
		{
			int roleID = dbMgr.DBRoleMgr.FindDBRoleID(roleName);
			if (roleID < 0)
			{
				try
				{
					string roleRealName = "";
					int zoneID = -1;
					Global.GetRoleRealNameAndZoneID(roleName, out roleRealName, out zoneID);
					if (zoneID >= 0)
					{
						roleID = DBQuery.GetRoleIDByRoleName(dbMgr, roleRealName, zoneID);
					}
				}
				catch
				{
				}
			}
			return roleID;
		}

		
		public static void GetRoleRealNameAndZoneID(string inRoleName, out string outRoleName, out int zoneID)
		{
			outRoleName = "";
			zoneID = -1;
			if (inRoleName.IndexOf('[') == 0 && inRoleName.IndexOf(']') >= 1)
			{
				string tmpStr = inRoleName.Substring(1);
				Regex r = new Regex("^[\\d]+");
				MatchCollection mc = r.Matches(tmpStr);
				if (mc.Count > 0)
				{
					zoneID = int.Parse(mc[0].Value);
				}
				int pos = inRoleName.IndexOf(']') + 1;
				outRoleName = inRoleName.Substring(pos);
			}
		}

		
		public static int TransMoneyToYuanBao(int money)
		{
			int moneyToYuanBao = GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10);
			return money * moneyToYuanBao;
		}

		
		public static string GetHuoDongKeyString(string fromDate, string toDate)
		{
			return string.Format("{0}_{1}", fromDate, toDate);
		}

		
		public static bool IsInActivityPeriod(string fromDate, string toDate)
		{
			try
			{
				if (DateTime.Now >= DateTime.Parse(fromDate) && DateTime.Now <= DateTime.Parse(toDate))
				{
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		
		public static bool GetUserMaxLevelRole(DBManager dbMgr, string userID, out string maxLevelRoleName, out int maxLevelRoleZoneID)
		{
			maxLevelRoleName = "";
			maxLevelRoleZoneID = 1;
			DBUserInfo userInfo = dbMgr.GetDBUserInfo(userID);
			if (null != userInfo)
			{
				int maxLevel = -1;
				int pos = -1;
				int maxchangelife = -1;
				for (int i = 0; i < userInfo.ListRoleChangeLifeCount.Count; i++)
				{
					if (userInfo.ListRoleChangeLifeCount[i] > maxchangelife)
					{
						maxchangelife = userInfo.ListRoleChangeLifeCount[i];
						maxLevel = userInfo.ListRoleLevels[i];
						pos = i;
					}
					else if (userInfo.ListRoleChangeLifeCount[i] == maxchangelife)
					{
						if (maxLevel < userInfo.ListRoleLevels[i])
						{
							maxLevel = userInfo.ListRoleLevels[i];
							pos = i;
						}
					}
				}
				if (pos >= 0 && pos < userInfo.ListRoleNames.Count)
				{
					maxLevelRoleName = userInfo.ListRoleNames[pos];
				}
				if (pos >= 0 && pos < userInfo.ListRoleZoneIDs.Count)
				{
					maxLevelRoleZoneID = userInfo.ListRoleZoneIDs[pos];
				}
			}
			return true;
		}

		
		public static bool GetRoleNameAndUserID(DBManager dbMgr, int rid, out string maxLevelRoleName, out string userID)
		{
			maxLevelRoleName = "";
			userID = "";
			DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref rid);
			if (null != roleInfo)
			{
				maxLevelRoleName = Global.FormatRoleName(roleInfo);
				userID = roleInfo.UserID;
			}
			return true;
		}

		
		public static List<HuoDongPaiHangData> GetPaiHangItemListByHuoDongLimit(DBManager dbMgr, List<int> minGateValueList, int activityType, string midDate, int maxPaiHang = 10)
		{
			List<HuoDongPaiHangData> listPaiHangReal = DBQuery.GetActivityPaiHangListNearMidTime(dbMgr, activityType, midDate, maxPaiHang);
			List<HuoDongPaiHangData> listPaiHang = new List<HuoDongPaiHangData>();
			int preUserPaiHang = 0;
			for (int i = 0; i < listPaiHangReal.Count; i++)
			{
				HuoDongPaiHangData phData = listPaiHangReal[i];
				phData.PaiHang = -1;
				for (int j = preUserPaiHang; j < minGateValueList.Count; j++)
				{
					if (phData.PaiHangValue >= minGateValueList[j])
					{
						phData.PaiHang = j + 1;
						listPaiHang.Add(phData);
						preUserPaiHang = phData.PaiHang;
						break;
					}
				}
				if (phData.PaiHang < 0 || phData.PaiHang >= minGateValueList.Count)
				{
					break;
				}
			}
			return listPaiHang;
		}

		
		public static List<InputKingPaiHangData> GetInputKingPaiHangListByHuoDongLimit(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int maxPaiHang = 3)
		{
			RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, minGateValueList);
			RankData rankData = GameDBManager.RankCacheMgr.GetRankData(key, minGateValueList, maxPaiHang);
			List<InputKingPaiHangData> result;
			if (null == rankData)
			{
				result = null;
			}
			else
			{
				List<InputKingPaiHangData> listPaiHang = GameDBManager.RankCacheMgr.GetRankDataList(rankData);
				if (null == listPaiHang)
				{
					listPaiHang = new List<InputKingPaiHangData>();
				}
				result = listPaiHang;
			}
			return result;
		}

		
		public static List<InputKingPaiHangData> GetUsedMoneyKingPaiHangListByHuoDongLimit(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int maxPaiHang = 3)
		{
			RankDataKey key = new RankDataKey(RankType.Consume, fromDate, toDate, minGateValueList);
			RankData rankData = GameDBManager.RankCacheMgr.GetRankData(key, minGateValueList, maxPaiHang);
			List<InputKingPaiHangData> result;
			if (null == rankData)
			{
				result = null;
			}
			else
			{
				List<InputKingPaiHangData> listPaiHang = GameDBManager.RankCacheMgr.GetRankDataList(rankData);
				if (null == listPaiHang)
				{
					listPaiHang = new List<InputKingPaiHangData>();
				}
				result = listPaiHang;
			}
			return result;
		}

		
		public static TCPProcessCmdResults ProcessHuoDongForKing(DBManager dbMgr, TCPOutPacketPool pool, int nID, string[] fields, int activityType, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			int roleID = Convert.ToInt32(fields[0]);
			string fromDate = fields[1].Replace('$', ':');
			string toDate = fields[2].Replace('$', ':');
			string[] minGateValueArr = fields[3].Split(new char[]
			{
				'_'
			});
			List<int> minGateValueList = new List<int>();
			foreach (string item in minGateValueArr)
			{
				minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
			}
			DBRoleInfo roleInfo = dbMgr.GetDBRoleInfo(ref roleID);
			TCPProcessCmdResults result;
			if (null == roleInfo)
			{
				string strcmd = string.Format("{0}:{1}:0", -1001, roleID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
				result = TCPProcessCmdResults.RESULT_DATA;
			}
			else
			{
				string paiHangDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (!Global.IsInActivityPeriod(fromDate, toDate))
				{
					paiHangDate = toDate;
				}
				List<HuoDongPaiHangData> listPaiHang = Global.GetPaiHangItemListByHuoDongLimit(dbMgr, minGateValueList, activityType, paiHangDate, 10);
				int paiHang = -1;
				for (int i = 0; i < listPaiHang.Count; i++)
				{
					if (listPaiHang[i] != null && roleInfo.RoleID == listPaiHang[i].RoleID)
					{
						paiHang = listPaiHang[i].PaiHang;
						break;
					}
				}
				if (paiHang <= 0)
				{
					string strcmd = string.Format("{0}:{1}:0", -10007, roleID);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else
				{
					string huoDongKeyStr = Global.GetHuoDongKeyString(fromDate, toDate);
					int hasgettimes = 0;
					string lastgettime = "";
					string strcmd;
					lock (roleInfo)
					{
						DBQuery.GetAwardHistoryForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, activityType, huoDongKeyStr, out hasgettimes, out lastgettime);
						if (hasgettimes > 0)
						{
							strcmd = string.Format("{0}:{1}:0", -10005, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
						int ret = DBWriter.AddHongDongAwardRecordForRole(dbMgr, roleInfo.RoleID, roleInfo.ZoneID, activityType, huoDongKeyStr, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (ret < 0)
						{
							strcmd = string.Format("{0}:{1}:0", -1008, roleID);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					strcmd = string.Format("{0}:{1}:{2}", 1, roleID, paiHang);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
			}
			return result;
		}

		
		public static TCPProcessCmdResults GetHuoDongPaiHangForKing(DBManager dbMgr, TCPOutPacketPool pool, int nID, string[] fields, int activityType, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			int roleID = Convert.ToInt32(fields[0]);
			string fromDate = fields[1].Replace('$', ':');
			string toDate = fields[2].Replace('$', ':');
			string[] minGateValueArr = fields[3].Split(new char[]
			{
				'_'
			});
			List<int> minGateValueList = new List<int>();
			foreach (string item in minGateValueArr)
			{
				minGateValueList.Add(Global.SafeConvertToInt32(item, 10));
			}
			string paiHangDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			if (!Global.IsInActivityPeriod(fromDate, toDate))
			{
				paiHangDate = toDate;
			}
			List<HuoDongPaiHangData> listPaiHang = Global.GetPaiHangItemListByHuoDongLimit(dbMgr, minGateValueList, activityType, paiHangDate, 10);
			tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<HuoDongPaiHangData>>(listPaiHang, pool, nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static void LogAndExitProcess(string error)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			File.AppendAllText("error.log", error + "\r\n");
			Console.WriteLine(error);
			Console.WriteLine("本程序将自动退出");
			Console.ForegroundColor = ConsoleColor.White;
			for (int i = 30; i > 0; i--)
			{
				Console.Write("\b\b" + i.ToString("00"));
				Thread.Sleep(600);
			}
			Process.GetCurrentProcess().Kill();
		}

		
		public static bool InitDBAutoIncrementValues(DBManager dbManger)
		{
			int baseValue = GameDBManager.ZoneID * GameDBManager.DBAutoIncreaseStepValue;
			bool result;
			if (baseValue < 0)
			{
				result = false;
			}
			else
			{
				int dbMaxValue = DBQuery.GetMaxRoleID(dbManger) + 1;
				int ret = DBWriter.ChangeTablesAutoIncrementValue(dbManger, "t_roles", Math.Max(baseValue, dbMaxValue));
				dbMaxValue = DBQuery.GetMaxMailID(dbManger) + 1;
				int ret2 = DBWriter.ChangeTablesAutoIncrementValue(dbManger, "t_mail", Math.Max(baseValue, dbMaxValue));
				dbMaxValue = DBQuery.GetMaxBangHuiID(dbManger) + 1;
				int ret3 = DBWriter.ChangeTablesAutoIncrementValue(dbManger, "t_banghui", Math.Max(baseValue, dbMaxValue));
				if (0 != ret)
				{
					Console.WriteLine("更新数据库表t_roles自增长字段出错");
				}
				if (0 != ret2)
				{
					Console.WriteLine("更新数据库表t_mail自增长字段出错");
				}
				if (0 != ret3)
				{
					Console.WriteLine("更新数据库表t_banghui自增长字段出错");
				}
				result = (ret == 0 && ret2 == 0 && 0 == ret3);
			}
			return result;
		}

		
		public static void UpdateRoleParamByName(DBManager dbMgr, DBRoleInfo dbRoleInfo, string name, string value, RoleParamType roleParamType = null)
		{
			if (roleParamType == null)
			{
				roleParamType = RoleParamNameInfo.GetRoleParamType(name, value);
			}
			bool saved = DBWriter.UpdateRoleParams(dbMgr, dbRoleInfo.RoleID, name, value, roleParamType);
			lock (dbRoleInfo)
			{
				RoleParamsData roleParamsData = null;
				if (!dbRoleInfo.RoleParamsDict.TryGetValue(name, out roleParamsData))
				{
					roleParamsData = new RoleParamsData
					{
						ParamName = name,
						ParamValue = value,
						ParamType = roleParamType
					};
					dbRoleInfo.RoleParamsDict[name] = roleParamsData;
				}
				else
				{
					roleParamsData.ParamValue = value;
				}
				if (saved)
				{
					roleParamsData.UpdateFaildTicks = 0L;
				}
				else
				{
					roleParamsData.UpdateFaildTicks = TimeUtil.NOW();
				}
				if (name == "20017" || name == "10213")
				{
					DBRoleInfo.InitFromRoleParams(dbRoleInfo);
				}
			}
		}

		
		public static long ModifyRoleParamLongByName(DBManager dbMgr, DBRoleInfo dbRoleInfo, string name, long value, RoleParamType roleParamType = null)
		{
			value += Global.GetRoleParamsInt64(dbRoleInfo, name);
			Global.UpdateRoleParamByName(dbMgr, dbRoleInfo, name, value.ToString(), roleParamType);
			return value;
		}

		
		public static string GetRoleParamByName(DBRoleInfo dbRoleInfo, string name)
		{
			lock (dbRoleInfo)
			{
				RoleParamsData roleParamsData = null;
				if (dbRoleInfo.RoleParamsDict.TryGetValue(name, out roleParamsData))
				{
					return roleParamsData.ParamValue;
				}
			}
			return null;
		}

		
		public static int GetRoleParamsInt32(DBRoleInfo dbRoleInfo, string name)
		{
			string valueString = Global.GetRoleParamByName(dbRoleInfo, name);
			int result;
			if (valueString == null || valueString.Length <= 0)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(valueString, 10);
			}
			return result;
		}

		
		public static long GetRoleParamsInt64(DBRoleInfo dbRoleInfo, string name)
		{
			string valueString = Global.GetRoleParamByName(dbRoleInfo, name);
			long result;
			if (valueString == null || valueString.Length <= 0)
			{
				result = 0L;
			}
			else
			{
				result = Global.SafeConvertToInt64(valueString, 10);
			}
			return result;
		}

		
		public static int AddNewQiangGouBuyItem(DBManager dbMgr, int roleID, int goodsID, int goodsNum, int totalPrice, int leftMoney, int qiangGouId, int actStartDay)
		{
			lock (Global.QiangGouMutex)
			{
				int ret = DBWriter.AddNewQiangGouBuyItem(dbMgr, roleID, goodsID, goodsNum, totalPrice, leftMoney, qiangGouId, actStartDay);
				if (ret < 0)
				{
					return -10001;
				}
			}
			return 1;
		}

		
		public static void QueryQiangGouBuyItemInfo(DBManager dbMgr, int roleID, int goodsID, int qiangGouId, int random, int actStartDay, out int roleBuyNum, out int totalBuyNum)
		{
			lock (Global.QiangGouMutex)
			{
				roleBuyNum = DBQuery.QueryQiangGouBuyItemNumByRoleID(dbMgr, roleID, goodsID, qiangGouId, random, actStartDay);
				totalBuyNum = DBQuery.QueryQiangGouBuyItemNum(dbMgr, goodsID, qiangGouId, random, actStartDay);
			}
		}

		
		public static int GetBitValue(int whichOne)
		{
			return (int)Math.Pow(2.0, (double)(whichOne - 1));
		}

		
		public static long GetBitRangeValue(long value, int startBit, int count)
		{
			long result;
			if (count > 63)
			{
				result = 0L;
			}
			else if (startBit < 0)
			{
				result = 0L;
			}
			else if (startBit < 0)
			{
				result = 0L;
			}
			else
			{
				long bv = (long)Math.Pow(2.0, (double)count) - 1L;
				long mask = long.MaxValue;
				if (startBit != 0)
				{
					value >>= 1;
					value &= mask;
					value >>= startBit - 1;
				}
				result = (value & bv);
			}
			return result;
		}

		
		public static DateTime GetAddDaysDataTime(DateTime dateTime, int addDays, bool roundDay = true)
		{
			if (roundDay)
			{
				dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
			}
			return new DateTime(dateTime.Ticks + (long)addDays * 10000L * 1000L * 24L * 60L * 60L);
		}

		
		public static WingData GetWingData(DBRoleInfo dbRoleInfo)
		{
			WingData result;
			if (null == dbRoleInfo)
			{
				result = null;
			}
			else
			{
				lock (dbRoleInfo)
				{
					result = dbRoleInfo.MyWingData;
				}
			}
			return result;
		}

		
		public static TCPProcessCmdResults SaveConsumeLog(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Global.SafeConvertToInt32(fields[0], 10);
				int money = Global.SafeConvertToInt32(fields[1], 10);
				string datestr = DateTime.Now.ToString("yyyy-MM-dd");
				DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbRoleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateCityInfoItem(dbMgr, dbRoleInfo.LastIP, dbRoleInfo.UserID, "usedmoney", money);
				int nRet = DBWriter.SaveConsumeLog(dbMgr, roleID, datestr, money);
				if (0 <= nRet)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
					Global.AddTotalUsedMoney(dbMgr, roleID, money);
					GameDBManager.RankCacheMgr.OnUserDoSomething(roleID, RankType.Consume, money);
				}
				else
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				LogManager.WriteException(e.ToString());
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static string GetLocalAddressIPs()
		{
			string addressIP = "";
			try
			{
				foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
				{
					if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
					{
						if (addressIP == "")
						{
							addressIP = _IPAddress.ToString();
						}
						else
						{
							addressIP = addressIP + "_" + _IPAddress.ToString();
						}
					}
				}
			}
			catch
			{
			}
			return addressIP;
		}

		
		public static string GetInternalIP()
		{
			string localIP = "?";
			try
			{
				IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ip in host.AddressList)
				{
					if (ip.AddressFamily.ToString() == "InterNetwork")
					{
						string[] fields = ip.ToString().Split(new char[]
						{
							'.'
						});
						if (fields[0] == "10" || fields[0] == "192")
						{
							localIP = ip.ToString();
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
			return localIP;
		}

		
		public static int GetTotalUsedMoney(DBManager dbMgr, int roleID)
		{
			DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
			int result2;
			if (null == dbRoleInfo)
			{
				result2 = 0;
			}
			else
			{
				string strCostMoney = DBQuery.GetRoleParamByName(dbMgr, roleID, "TotalCostMoney");
				string[] strFields = strCostMoney.Split(new char[]
				{
					','
				});
				int costMoney;
				if (strFields == null || strFields.Length != 2)
				{
					costMoney = DBQuery.GetUserUsedMoney(dbMgr, roleID, dbRoleInfo.RegTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					string result = string.Format("{0},{1}", 1, costMoney);
					DBWriter.UpdateRoleParams(dbMgr, roleID, "TotalCostMoney", result, null);
				}
				else
				{
					costMoney = Convert.ToInt32(strFields[1]);
				}
				result2 = costMoney;
			}
			return result2;
		}

		
		public static void AddTotalUsedMoney(DBManager dbMgr, int roleID, int addMoney)
		{
			DBRoleInfo dbRoleInfo = dbMgr.GetDBRoleInfo(ref roleID);
			if (null != dbRoleInfo)
			{
				lock (Global.addtotalusedmoney_mutex)
				{
					string strCostMoney = DBQuery.GetRoleParamByName(dbMgr, roleID, "TotalCostMoney");
					string[] strFields = strCostMoney.Split(new char[]
					{
						','
					});
					if (strFields != null && strFields.Length == 2)
					{
						int costMoney = Convert.ToInt32(strFields[1]);
						string result = string.Format("{0},{1}", 1, costMoney + addMoney);
						DBWriter.UpdateRoleParams(dbMgr, roleID, "TotalCostMoney", result, null);
					}
				}
			}
		}

		
		public static List<uint> ParseRoleparamStreamValueToList(string value)
		{
			List<uint> lsValues = new List<uint>();
			List<uint> result;
			if (string.IsNullOrEmpty(value))
			{
				result = lsValues;
			}
			else
			{
				byte[] b = Convert.FromBase64String(value);
				value = Encoding.GetEncoding("latin1").GetString(b);
				int pos = 0;
				for (int usedLenght = 0; usedLenght < value.Length; usedLenght += 4)
				{
					byte[] bytes_4 = Encoding.GetEncoding("latin1").GetBytes(value.Substring(pos, 4));
					lsValues.Add(BitConverter.ToUInt32(bytes_4, 0));
					pos += 4;
				}
				result = lsValues;
			}
			return result;
		}

		
		public static string ParseListToRoleparamStreamValue(List<uint> lsUint)
		{
			string newStringValue = "";
			for (int i = 0; i < lsUint.Count; i++)
			{
				byte[] bytes = BitConverter.GetBytes(lsUint[i]);
				newStringValue += Encoding.GetEncoding("latin1").GetString(bytes);
			}
			byte[] b = Encoding.GetEncoding("latin1").GetBytes(newStringValue);
			return Convert.ToBase64String(b);
		}

		
		public static int[] StringArray2IntArray(string[] sa)
		{
			int[] da = new int[sa.Length];
			for (int i = 0; i < sa.Length; i++)
			{
				string str = sa[i].Trim();
				str = (string.IsNullOrEmpty(str) ? "0" : str);
				da[i] = Convert.ToInt32(str);
			}
			return da;
		}

		
		public static List<GoodsData> ParseGoodsDataList(string strGoodIDs)
		{
			string[] fields = strGoodIDs.Split(new char[]
			{
				'|'
			});
			List<GoodsData> goodsDataList = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] sa = fields[i].Split(new char[]
				{
					','
				});
				if (sa.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("ParseGMailGoodsDataList解析{0}中第{1}个的奖励项时失败, 物品配置项个数错误", strGoodIDs, i), null, true);
				}
				else
				{
					int[] goodsFields = Global.StringArray2IntArray(sa);
					GoodsData gmailData = new GoodsData
					{
						GoodsID = goodsFields[0],
						GCount = goodsFields[1],
						Forge_level = goodsFields[3],
						Binding = goodsFields[2],
						Lucky = goodsFields[5],
						ExcellenceInfo = goodsFields[6],
						AppendPropLev = goodsFields[4]
					};
					goodsDataList.Add(gmailData);
				}
			}
			return goodsDataList;
		}

		
		public static Encoding GetSysEncoding()
		{
			Encoding result;
			if ("utf8" == DBConnections.dbNames.ToLower())
			{
				result = Encoding.UTF8;
			}
			else
			{
				result = Encoding.Default;
			}
			return result;
		}

		
		public static void CheckCodes()
		{
			bool result = true;
			result &= Global.CheckAllProtoBufContract();
			result &= Global.CheckDuplicateEnum(typeof(TCPGameServerCmds));
			result &= Global.CheckDuplicateEnum(typeof(SaleGoodsConsts));
			result &= Global.CheckDuplicateEnum(typeof(ActivityTypes));
			if (!(result & Global.CheckDuplicateFieldValue(typeof(RoleParamName))))
			{
				Console.WriteLine("代码检查发现问题,请尽快反馈给研发部\n\n");
			}
		}

		
		public static bool CheckDuplicateEnum(Type type)
		{
			bool result = true;
			HashSet<int> hashSet = new HashSet<int>();
			Array array = type.GetEnumValues();
			foreach (object v0 in array)
			{
				int v = (int)v0;
				if (!hashSet.Add(v) && !Global.DontCheckEnumNames.Contains(v0.ToString()))
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("枚举类型[{0}]定义常量值[{1}]重复", type.ToString(), v), null, true);
					result = false;
				}
			}
			return result;
		}

		
		public static bool CheckDuplicateFieldValue(Type type)
		{
			bool result = true;
			HashSet<string> hashSet = new HashSet<string>();
			FieldInfo[] fields = type.GetFields();
			foreach (FieldInfo field in fields)
			{
				string v = field.GetValue(null).ToString();
				if (!hashSet.Add(v))
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("类型[{0}]定义常量值[{1}]重复", type.ToString(), v), null, true);
					result = false;
				}
			}
			return result;
		}

		
		public static bool CheckAllProtoBufContract()
		{
			bool result = true;
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				bool hasDefaultConstructor = false;
				Dictionary<int, string> dict = new Dictionary<int, string>();
				object[] attributes = type.GetCustomAttributes(typeof(ProtoContractAttribute), false);
				if (attributes.Length > 0)
				{
					foreach (MemberInfo member in type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
					{
						if (member.MemberType == MemberTypes.Constructor)
						{
							ConstructorInfo ci = member as ConstructorInfo;
							if (null != ci)
							{
								if (ci.GetParameters().Length == 0)
								{
									hasDefaultConstructor = true;
								}
							}
						}
						else if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
						{
							object[] memberAttributes = member.GetCustomAttributes(typeof(ProtoMemberAttribute), false);
							foreach (object obj in memberAttributes)
							{
								ProtoMemberAttribute a = obj as ProtoMemberAttribute;
								if (null != a)
								{
									string name;
									if (dict.TryGetValue(a.Tag, out name) && name != member.Name)
									{
										result = false;
										Console.WriteLine("Protobuf定义{2}的Tag值{0}有重复:{1}", a.Tag, member.Name, type.ToString());
										break;
									}
									dict.Add(a.Tag, member.Name);
								}
							}
						}
					}
					if (!hasDefaultConstructor)
					{
						result = false;
						Console.WriteLine("Protobuf要求{0}结构必须有默认构造函数", type.ToString());
					}
				}
			}
			return result;
		}

		
		public static string GCC(int formatId, params object[] args)
		{
			string format = Global.FormatArray[formatId];
			string data = string.Format(format, args);
			string result;
			if (string.IsNullOrEmpty(data))
			{
				result = "";
			}
			else
			{
				byte[] bytes = Encoding.UTF8.GetBytes(data);
				MD5 md5 = MD5.Create();
				byte[] bytes2 = md5.ComputeHash(bytes);
				StringBuilder sb = new StringBuilder(32);
				foreach (byte b in bytes2)
				{
					sb.Append(b.ToString("X2"));
				}
				result = sb.ToString();
			}
			return result;
		}

		
		public static bool CCC(string cc, int formatId, params object[] args)
		{
			string format = Global.FormatArray[formatId];
			string data = string.Format(format, args);
			string cc2 = Global.GCC(formatId, args);
			return cc == cc2;
		}

		
		public static bool AddCC(string cc)
		{
			bool result;
			lock (Global.CCHashSet1)
			{
				result = Global.CCHashSet1.Add(cc);
			}
			return result;
		}

		
		public static bool ProcessGMMsg(string[] args)
		{
			bool result;
			if (args[0] == "-charge")
			{
				if (args.Length >= 6)
				{
					UserMoneyMgr.GMAddCharge(args[1], args[2], args[3], args[4], args[5].Replace('$', ':'));
					result = true;
				}
				else
				{
					result = false;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public const int MaxFuMoMailNum = 50;

		
		public static object BangHuiMutex = new object();

		
		private static Dictionary<string, string> LangDict = null;

		
		private static Dictionary<string, List<InputKingPaiHangData>> dictCachingInputMoneyKingPaiHangListByHuoDongLimit = new Dictionary<string, List<InputKingPaiHangData>>();

		
		private static Dictionary<string, long> dictInputMoneyKingPaiHangListByHuoDongLimitHour = new Dictionary<string, long>();

		
		private static object CachingInputMoneyKingPaiHangLock = new object();

		
		private static Dictionary<string, List<InputKingPaiHangData>> dictCachingUsedMoneyKingPaiHangListByHuoDongLimit = new Dictionary<string, List<InputKingPaiHangData>>();

		
		private static Dictionary<string, long> dictUsedMoneyKingPaiHangListByHuoDongLimitHour = new Dictionary<string, long>();

		
		private static object CachingUsedMoneyKingPaiHangLock = new object();

		
		public static object QiangGouMutex = new object();

		
		private static object addtotalusedmoney_mutex = new object();

		
		public static string[] DontCheckEnumNames = new string[]
		{
			"Max",
			"Max_Configed"
		};

		
		private static readonly string[] FormatArray = new string[]
		{
			"",
			"j0U8l>.fjoean13fw16d2lf3s13e5.*{0}XX",
			"jOU81>.fjoean13fw16d2lf3s13e5.*{0}YY{1}",
			"jOU8l>.fjofw16d21f3s13e5.*{0}YY{1}.ean13{2}",
			"jOU81>.fjoeanl3fw16d21f.*{0}YY{1}3sl3e5.{2}={3}",
			"jOU81>.fjoeanl3fw16d2.*{0}YY{1}.{2}={3}1f3sl3e5-{4}"
		};

		
		private static HashSet<string> CCHashSet1 = new HashSet<string>();

		
		
		public delegate T SQLDelegate<T>(MySQLDataReader reader);
	}
}
