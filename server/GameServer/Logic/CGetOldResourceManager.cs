using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020005E0 RID: 1504
	public class CGetOldResourceManager
	{
		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06001C00 RID: 7168 RVA: 0x001A42A4 File Offset: 0x001A24A4
		public static XElement xmlData
		{
			get
			{
				lock (CGetOldResourceManager._xmlDataMutex)
				{
					if (CGetOldResourceManager._xmlData != null)
					{
						return CGetOldResourceManager._xmlData;
					}
				}
				XElement xml = null;
				try
				{
					string fileName = "Config/ZiYuanZhaoHui.xml";
					xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(fileName));
				}
				catch (Exception e)
				{
					xml = null;
					LogManager.WriteException(e.ToString());
				}
				lock (CGetOldResourceManager._xmlDataMutex)
				{
					CGetOldResourceManager._xmlData = xml;
				}
				return CGetOldResourceManager._xmlData;
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06001C01 RID: 7169 RVA: 0x001A4388 File Offset: 0x001A2588
		public static double[] ExpGold
		{
			get
			{
				double[] exp;
				if (CGetOldResourceManager._Exp != null)
				{
					exp = CGetOldResourceManager._Exp;
				}
				else
				{
					double[] Exp = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiExp", ',');
					lock (CGetOldResourceManager._ExpMutex)
					{
						CGetOldResourceManager._Exp = Exp;
					}
					exp = CGetOldResourceManager._Exp;
				}
				return exp;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06001C02 RID: 7170 RVA: 0x001A4404 File Offset: 0x001A2604
		public static double[] BondGold
		{
			get
			{
				double[] bondGold;
				if (CGetOldResourceManager._BondGold != null)
				{
					bondGold = CGetOldResourceManager._BondGold;
				}
				else
				{
					double[] Exp = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiBandGold", ',');
					lock (CGetOldResourceManager._BondGoldMutex)
					{
						CGetOldResourceManager._BondGold = Exp;
					}
					bondGold = CGetOldResourceManager._BondGold;
				}
				return bondGold;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06001C03 RID: 7171 RVA: 0x001A4480 File Offset: 0x001A2680
		public static double[] MoJing
		{
			get
			{
				double[] moJing;
				if (CGetOldResourceManager._MoJing != null)
				{
					moJing = CGetOldResourceManager._MoJing;
				}
				else
				{
					double[] values = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiMoJing", ',');
					lock (CGetOldResourceManager._MoJingMutex)
					{
						CGetOldResourceManager._MoJing = values;
					}
					moJing = CGetOldResourceManager._MoJing;
				}
				return moJing;
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06001C04 RID: 7172 RVA: 0x001A44FC File Offset: 0x001A26FC
		public static double[] ShengWang
		{
			get
			{
				double[] shengWang;
				if (CGetOldResourceManager._ShengWang != null)
				{
					shengWang = CGetOldResourceManager._ShengWang;
				}
				else
				{
					double[] values = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiShengWang", ',');
					lock (CGetOldResourceManager._ShengWangMutex)
					{
						CGetOldResourceManager._ShengWang = values;
					}
					shengWang = CGetOldResourceManager._ShengWang;
				}
				return shengWang;
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06001C05 RID: 7173 RVA: 0x001A4578 File Offset: 0x001A2778
		public static double[] ChengJiu
		{
			get
			{
				double[] chengJiu;
				if (CGetOldResourceManager._ChengJiu != null)
				{
					chengJiu = CGetOldResourceManager._ChengJiu;
				}
				else
				{
					double[] values = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiChengJiu", ',');
					lock (CGetOldResourceManager._ChengJiuMutex)
					{
						CGetOldResourceManager._ChengJiu = values;
					}
					chengJiu = CGetOldResourceManager._ChengJiu;
				}
				return chengJiu;
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06001C06 RID: 7174 RVA: 0x001A45F4 File Offset: 0x001A27F4
		public static double[] ZhanGong
		{
			get
			{
				double[] zhanGong;
				if (CGetOldResourceManager._ZhanGong != null)
				{
					zhanGong = CGetOldResourceManager._ZhanGong;
				}
				else
				{
					double[] values = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiZhanGong", ',');
					lock (CGetOldResourceManager._ZhanGongMutex)
					{
						CGetOldResourceManager._ZhanGong = values;
					}
					zhanGong = CGetOldResourceManager._ZhanGong;
				}
				return zhanGong;
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06001C07 RID: 7175 RVA: 0x001A4670 File Offset: 0x001A2870
		public static double[] BangZuan
		{
			get
			{
				double[] bangZuan;
				if (CGetOldResourceManager._BangZuan != null)
				{
					bangZuan = CGetOldResourceManager._BangZuan;
				}
				else
				{
					double[] values = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiBindZuan", ',');
					lock (CGetOldResourceManager._BangZuanMutex)
					{
						CGetOldResourceManager._BangZuan = values;
					}
					bangZuan = CGetOldResourceManager._BangZuan;
				}
				return bangZuan;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06001C08 RID: 7176 RVA: 0x001A46EC File Offset: 0x001A28EC
		public static double[] XingHun
		{
			get
			{
				double[] xingHun;
				if (CGetOldResourceManager._XingHun != null)
				{
					xingHun = CGetOldResourceManager._XingHun;
				}
				else
				{
					double[] values = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiXingHun", ',');
					lock (CGetOldResourceManager._XingHunMutex)
					{
						CGetOldResourceManager._XingHun = values;
					}
					xingHun = CGetOldResourceManager._XingHun;
				}
				return xingHun;
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06001C09 RID: 7177 RVA: 0x001A4768 File Offset: 0x001A2968
		public static double[] YuanSuFenMo
		{
			get
			{
				double[] yuanSuFenMo;
				if (CGetOldResourceManager._YuanSuFenMo != null)
				{
					yuanSuFenMo = CGetOldResourceManager._YuanSuFenMo;
				}
				else
				{
					double[] values = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZiYuanZhaoHuiYuanSuFenMo", ',');
					lock (CGetOldResourceManager._YuanSuFenMoMutex)
					{
						CGetOldResourceManager._YuanSuFenMo = values;
					}
					yuanSuFenMo = CGetOldResourceManager._YuanSuFenMo;
				}
				return yuanSuFenMo;
			}
		}

		// Token: 0x06001C0A RID: 7178 RVA: 0x001A47E4 File Offset: 0x001A29E4
		public static double RoleChangelifeRate(int count)
		{
			try
			{
				if (CGetOldResourceManager._changelifeRate == null)
				{
					CGetOldResourceManager._changelifeRate = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuanShengExpXiShu", ',');
				}
				if (CGetOldResourceManager._changelifeRate != null && CGetOldResourceManager._changelifeRate.Length > count)
				{
					return CGetOldResourceManager._changelifeRate[count];
				}
				return 1.0;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "获取经验各项产出根据转生对应的经验系数 error count=" + count, false, false);
			}
			return 1.0;
		}

		// Token: 0x06001C0B RID: 7179 RVA: 0x001A4888 File Offset: 0x001A2A88
		public static List<OldResourceInfo> GetOldResourceInfo(GameClient client)
		{
			List<OldResourceInfo> oldinfo = new List<OldResourceInfo>();
			if (client.ClientData.OldResourceInfoDict != null)
			{
				foreach (OldResourceInfo item in client.ClientData.OldResourceInfoDict.Values)
				{
					if (item != null && item.leftCount > 0)
					{
						oldinfo.Add(item);
					}
				}
			}
			return oldinfo;
		}

		// Token: 0x06001C0C RID: 7180 RVA: 0x001A4928 File Offset: 0x001A2B28
		public static bool HasOldResource(GameClient client)
		{
			return CGetOldResourceManager.GetOldResourceInfo(client).Count > 0;
		}

		// Token: 0x06001C0D RID: 7181 RVA: 0x001A4954 File Offset: 0x001A2B54
		private static bool RoleCando(GameClient client, XElement item)
		{
			int type = (int)Global.GetSafeAttributeLong(item, "Type");
			string[] minilevel = Global.GetSafeAttributeStr(item, "MinLevel").Split(new char[]
			{
				','
			});
			string[] maxlevel = Global.GetSafeAttributeStr(item, "MaxLevel").Split(new char[]
			{
				','
			});
			int curmaxchangelife = Global.SafeConvertToInt32(minilevel[0]);
			int curmaxlevel = Global.SafeConvertToInt32(minilevel[1]);
			string NeedRenWu = Global.GetSafeAttributeStr(item, "NeedRenWu");
			bool condition = false;
			if (Global.SafeConvertToInt32(minilevel[0]) < client.ClientData.ChangeLifeCount && client.ClientData.ChangeLifeCount < Global.SafeConvertToInt32(maxlevel[0]))
			{
				condition = true;
			}
			if (Global.SafeConvertToInt32(minilevel[0]) == client.ClientData.ChangeLifeCount)
			{
				if (Global.SafeConvertToInt32(minilevel[1]) <= client.ClientData.Level)
				{
					condition = true;
				}
			}
			if (Global.SafeConvertToInt32(maxlevel[0]) == client.ClientData.ChangeLifeCount)
			{
				if (Global.SafeConvertToInt32(maxlevel[1]) >= client.ClientData.Level)
				{
					condition = true;
				}
			}
			bool condition2;
			if (string.IsNullOrEmpty(NeedRenWu))
			{
				condition2 = true;
			}
			else
			{
				int taskid = Global.SafeConvertToInt32(NeedRenWu);
				condition2 = CGetOldResourceManager.TaskHasDone(client, taskid);
			}
			return condition && condition2;
		}

		// Token: 0x06001C0E RID: 7182 RVA: 0x001A4AD8 File Offset: 0x001A2CD8
		private static bool IsCanCalcOldResource(GameClient client)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				DateTime dtCreateRoleTime = Global.GetRegTime(client.ClientData);
				DateTime dtCurTime = TimeUtil.NowDateTime();
				if (dtCreateRoleTime.Year == dtCurTime.Year)
				{
					if (dtCreateRoleTime.DayOfYear == dtCurTime.DayOfYear)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06001C0F RID: 7183 RVA: 0x001A4B40 File Offset: 0x001A2D40
		private static bool TaskHasDone(GameClient client, int taskID)
		{
			return client.ClientData.MainTaskID >= taskID;
		}

		// Token: 0x06001C10 RID: 7184 RVA: 0x001A4B64 File Offset: 0x001A2D64
		public static FuBenData GetOldFubenData(GameClient client, int fuBenID)
		{
			FuBenData result;
			if (null == client.ClientData.OldFuBenDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.OldFuBenDataList)
				{
					for (int i = 0; i < client.ClientData.OldFuBenDataList.Count; i++)
					{
						if (client.ClientData.OldFuBenDataList[i].FuBenID == fuBenID)
						{
							return client.ClientData.OldFuBenDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06001C11 RID: 7185 RVA: 0x001A4C28 File Offset: 0x001A2E28
		public static int GetVIPActiveNumByType(GameClient client, int activeId)
		{
			int nVipLev = client.ClientData.VipLevel;
			string keyname;
			switch (activeId)
			{
			case 1:
				keyname = "VIPEnterBloodCastleCountAddValue";
				break;
			case 2:
				keyname = "VIPEnterDaimonSquareCountAddValue";
				break;
			default:
				return 0;
			}
			int[] nArry = GameManager.systemParamsList.GetParamValueIntArrayByName(keyname, ',');
			if (nVipLev > 0 && nArry != null && nArry[nVipLev] > 0)
			{
				int nNum = nArry[nVipLev];
			}
			return 0;
		}

		// Token: 0x06001C12 RID: 7186 RVA: 0x001A4CAC File Offset: 0x001A2EAC
		private static void CalcOldResourceInfo(int leftnum, CGetResData data, OldResourceInfo inInfo, out OldResourceInfo outInfo)
		{
			outInfo = inInfo;
			if (leftnum > 0)
			{
				if (inInfo == null)
				{
					outInfo = new OldResourceInfo();
					outInfo.bandmoney = 0;
					outInfo.chengjiu = 0;
					outInfo.exp = 0;
					outInfo.mojing = 0;
					outInfo.zhangong = 0;
					outInfo.shengwang = 0;
					outInfo.leftCount = 0;
					outInfo.bandDiamond = 0;
					outInfo.xinghun = 0;
					outInfo.yuanSuFenMo = 0;
				}
				outInfo.bandmoney += data.bandmoney * leftnum;
				outInfo.chengjiu += data.chengjiu * leftnum;
				outInfo.exp += data.exp * leftnum;
				outInfo.mojing += data.mojing * leftnum;
				outInfo.zhangong += data.zhangong * leftnum;
				outInfo.shengwang += data.shengwang * leftnum;
				outInfo.bandDiamond += data.bandDiamond * leftnum;
				outInfo.xinghun += data.xinghun * leftnum;
				outInfo.yuanSuFenMo += data.yuanSuFenMo * leftnum;
				outInfo.leftCount += leftnum;
				outInfo.type = data.type;
			}
		}

		// Token: 0x06001C13 RID: 7187 RVA: 0x001A4E14 File Offset: 0x001A3014
		private static void CalcOldResourceInfo(int oldday, int oldnum, int total, CGetResData data, OldResourceInfo inInfo, out OldResourceInfo outInfo)
		{
			outInfo = inInfo;
			int leftnum = 0;
			int yesterdayid = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
			if (oldday >= 0 && oldnum >= 0)
			{
				if (yesterdayid == oldday)
				{
					leftnum = total - oldnum;
				}
				else
				{
					leftnum = total;
				}
				leftnum = ((leftnum > 0) ? leftnum : 0);
			}
			CGetOldResourceManager.CalcOldResourceInfo(leftnum, data, inInfo, out outInfo);
		}

		// Token: 0x06001C14 RID: 7188 RVA: 0x001A4E88 File Offset: 0x001A3088
		private static void GetFubenResourceInfo(GameClient client, int copyId, int total, bool needFinish, CGetResData data, OldResourceInfo inInfo, out OldResourceInfo outInfo)
		{
			outInfo = inInfo;
			if (total >= 1)
			{
				FuBenData fubendata = CGetOldResourceManager.GetOldFubenData(client, copyId);
				int oldday;
				int oldnum;
				if (null != fubendata)
				{
					oldday = fubendata.DayID;
					oldnum = (needFinish ? fubendata.FinishNum : fubendata.EnterNum);
				}
				else
				{
					oldday = 0;
					oldnum = 0;
				}
				CGetOldResourceManager.CalcOldResourceInfo(oldday, oldnum, total, data, inInfo, out outInfo);
			}
		}

		// Token: 0x06001C15 RID: 7189 RVA: 0x001A4EF0 File Offset: 0x001A30F0
		private static void ComputeResourceByType(GameClient client, int type, Dictionary<int, List<CGetResData>> getRestDataDict, out OldResourceInfo outInfo)
		{
			outInfo = null;
			if (getRestDataDict.ContainsKey(type))
			{
				List<CGetResData> datalist = getRestDataDict[type];
				foreach (CGetResData data in datalist)
				{
					if (data.copyId != -1)
					{
						SystemXmlItem systemFuBenItem = null;
						if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(data.copyId, out systemFuBenItem))
						{
							continue;
						}
						bool needFinish = true;
						int total = systemFuBenItem.GetIntValue("FinishNumber", -1);
						if (total < 0)
						{
							needFinish = false;
							total = systemFuBenItem.GetIntValue("EnterNumber", -1);
						}
						OldResourceInfo tempdata = null;
						CGetOldResourceManager.GetFubenResourceInfo(client, data.copyId, total, needFinish, data, tempdata, out tempdata);
						if (tempdata != null)
						{
							int leftnum = tempdata.leftCount;
							CGetOldResourceManager.CalcOldResourceInfo(tempdata.leftCount, data, outInfo, out outInfo);
						}
					}
					if (data.activeId != -1)
					{
						int leftnum = 0;
						switch (data.type)
						{
						case 5:
						{
							int nNum = CGetOldResourceManager.GetVIPActiveNumByType(client, data.activeId);
							int nMapID = Global.GetDaimonSquareCopySceneIDForRole(client);
							DaimonSquareDataInfo bcDataTmp = null;
							Data.DaimonSquareDataInfoList.TryGetValue(nMapID, out bcDataTmp);
							if (null == bcDataTmp)
							{
								bcDataTmp = Data.DaimonSquareDataInfoList.FirstOrDefault<KeyValuePair<int, DaimonSquareDataInfo>>().Value;
								if (bcDataTmp == null)
								{
									break;
								}
							}
							int nDate = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int nCount = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, 2);
							if (nCount < 0)
							{
								nCount = 0;
							}
							leftnum = bcDataTmp.MaxEnterNum + nNum - nCount;
							break;
						}
						case 6:
						{
							int nNum = CGetOldResourceManager.GetVIPActiveNumByType(client, data.activeId);
							int nMapID = Global.GetBloodCastleCopySceneIDForRole(client);
							BloodCastleDataInfo bcDataTmp2 = null;
							Data.BloodCastleDataInfoList.TryGetValue(nMapID, out bcDataTmp2);
							if (null == bcDataTmp2)
							{
								bcDataTmp2 = Data.BloodCastleDataInfoList.FirstOrDefault<KeyValuePair<int, BloodCastleDataInfo>>().Value;
								if (bcDataTmp2 == null)
								{
									break;
								}
							}
							int nDate = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int nCount = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, 1);
							if (nCount < 0)
							{
								nCount = 0;
							}
							leftnum = bcDataTmp2.MaxEnterNum + nNum - nCount;
							break;
						}
						case 7:
						{
							List<string> timePointsList = GameManager.AngelTempleMgr.m_AngelTempleData.BeginTime;
							int nDate = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int nCount = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, 5);
							if (nCount < 0)
							{
								nCount = 0;
							}
							leftnum = timePointsList.Count - nCount;
							break;
						}
						case 9:
						{
							int nDate = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int nCount = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, 6);
							if (nCount < 0)
							{
								nCount = 0;
							}
							leftnum = 1 - nCount;
							break;
						}
						case 10:
						{
							SystemXmlItem systemBattle = null;
							if (!GameManager.SystemBattle.SystemXmlItemDict.TryGetValue(1, out systemBattle))
							{
								return;
							}
							string[] fields = null;
							string timePoints = systemBattle.GetStringValue("TimePoints");
							if (timePoints != null && timePoints != "")
							{
								fields = timePoints.Split(new char[]
								{
									','
								});
							}
							int nDate = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int nCount = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, 3);
							if (nCount < 0)
							{
								nCount = 0;
							}
							if (fields != null)
							{
								leftnum = fields.Length - nCount;
							}
							break;
						}
						case 11:
						{
							SystemXmlItem systemBattle = null;
							if (!GameManager.SystemArenaBattle.SystemXmlItemDict.TryGetValue(1, out systemBattle))
							{
								return;
							}
							List<string> timePointsList = new List<string>();
							string[] fields = null;
							string timePoints = systemBattle.GetStringValue("TimePoints");
							if (timePoints != null && timePoints != "")
							{
								fields = timePoints.Split(new char[]
								{
									','
								});
							}
							int nDate = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
							int nCount = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, 4);
							if (nCount < 0)
							{
								nCount = 0;
							}
							if (fields != null)
							{
								leftnum = fields.Length - nCount;
							}
							break;
						}
						}
						leftnum = ((leftnum > 0) ? leftnum : 0);
						CGetOldResourceManager.CalcOldResourceInfo(leftnum, data, outInfo, out outInfo);
					}
					if (data.type == 13)
					{
						int jingjiFuBenId = (int)GameManager.systemParamsList.GetParamValueIntByName("JingJiFuBenID", -1);
						SystemXmlItem jingjiFuBenItem = null;
						GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(jingjiFuBenId, out jingjiFuBenItem);
						int total = jingjiFuBenItem.GetIntValue("EnterNumber", -1);
						CGetOldResourceManager.GetFubenResourceInfo(client, jingjiFuBenId, total, false, data, outInfo, out outInfo);
					}
					else if (data.type != 8)
					{
						if (data.type == 1)
						{
							int oldday = -1;
							int oldnum = -1;
							if (client.ClientData.YesterdayDailyTaskData != null)
							{
								oldday = DateTime.Parse(client.ClientData.YesterdayDailyTaskData.RecTime).DayOfYear;
								oldnum = client.ClientData.YesterdayDailyTaskData.RecNum;
							}
							CGetOldResourceManager.CalcOldResourceInfo(oldday, oldnum, 10, data, outInfo, out outInfo);
						}
						else if (data.type == 15)
						{
							int oldday = -1;
							int oldnum = -1;
							if (client.ClientData.YesterdayTaofaTaskData != null)
							{
								oldday = DateTime.Parse(client.ClientData.YesterdayTaofaTaskData.RecTime).DayOfYear;
								oldnum = client.ClientData.YesterdayTaofaTaskData.RecNum;
							}
							CGetOldResourceManager.CalcOldResourceInfo(oldday, oldnum, Global.MaxTaofaTaskNumForMU, data, outInfo, out outInfo);
						}
						else if (data.type == 16)
						{
							int oldday = -1;
							int oldnum = -1;
							if (client.ClientData.OldCrystalCollectData != null)
							{
								oldday = client.ClientData.OldCrystalCollectData.OldDay;
								oldnum = client.ClientData.OldCrystalCollectData.OldNum;
							}
							CGetOldResourceManager.CalcOldResourceInfo(oldday, oldnum, CaiJiLogic.DailyNum, data, outInfo, out outInfo);
						}
						else if (data.type == 19)
						{
							int oldday = Global.GetRoleParamsInt32FromDB(client, "HysyYTDSuccessDayId");
							int oldnum = Global.GetRoleParamsInt32FromDB(client, "HysyYTDSuccessCount");
							int leftnum = 3;
							int[] nParams = GameManager.systemParamsList.GetParamValueIntArrayByName("TempleMirageWinNum", ',');
							if (nParams != null && nParams.Length == 2)
							{
								leftnum = nParams[0];
							}
							CGetOldResourceManager.CalcOldResourceInfo(leftnum - oldnum, data, outInfo, out outInfo);
						}
					}
				}
			}
		}

		// Token: 0x06001C16 RID: 7190 RVA: 0x001A56B8 File Offset: 0x001A38B8
		public static void InitRoleOldResourceInfo(GameClient client, bool isFirstLogin)
		{
			if (CGetOldResourceManager.IsCanCalcOldResource(client))
			{
				if (!isFirstLogin)
				{
					client.ClientData.OldResourceInfoDict = CGetOldResourceManager.ReadResourceGetfromDB(client);
				}
				else if (CGetOldResourceManager.xmlData != null)
				{
					Dictionary<int, List<CGetResData>> getRestDataDict = new Dictionary<int, List<CGetResData>>();
					IEnumerable<XElement> xmlItems = CGetOldResourceManager.xmlData.Elements();
					foreach (XElement item in xmlItems)
					{
						if (CGetOldResourceManager.RoleCando(client, item))
						{
							int type = (int)Global.GetSafeAttributeLong(item, "Type");
							int copyId = (int)Global.GetSafeAttributeLong(item, "CodeID");
							int activeId = (int)Global.GetSafeAttributeLong(item, "HuoDongID");
							int exp = (int)Global.GetSafeAttributeLong(item, "ExpAward");
							int bandmoney = (int)Global.GetSafeAttributeLong(item, "BandMoneyAward");
							int shengwang = (int)Global.GetSafeAttributeLong(item, "ShengWangAward");
							int zhangong = (int)Global.GetSafeAttributeLong(item, "ZhanGongAward");
							int mojing = (int)Global.GetSafeAttributeLong(item, "MoJingAward");
							int chengjiu = (int)Global.GetSafeAttributeLong(item, "ChengJiuAward");
							int bandDiamond = (int)Global.GetSafeAttributeLong(item, "BindZuanAward");
							int xinghun = (int)Global.GetSafeAttributeLong(item, "XingHunAward");
							int yuanSuFenMo = (int)Global.GetSafeAttributeLong(item, "YuanSuFenMo");
							CGetResData data = new CGetResData();
							data.type = type;
							data.copyId = copyId;
							data.activeId = activeId;
							data.exp = ((exp > 0) ? exp : 0);
							data.bandmoney = ((bandmoney > 0) ? bandmoney : 0);
							data.shengwang = ((shengwang > 0) ? shengwang : 0);
							data.zhangong = ((zhangong > 0) ? zhangong : 0);
							data.mojing = ((mojing > 0) ? mojing : 0);
							data.chengjiu = ((chengjiu > 0) ? chengjiu : 0);
							data.bandDiamond = ((bandDiamond > 0) ? bandDiamond : 0);
							data.xinghun = ((xinghun > 0) ? xinghun : 0);
							data.yuanSuFenMo = ((yuanSuFenMo > 0) ? yuanSuFenMo : 0);
							if (!getRestDataDict.ContainsKey(type))
							{
								getRestDataDict[type] = new List<CGetResData>();
							}
							getRestDataDict[type].Add(data);
						}
					}
					Dictionary<int, OldResourceInfo> ResourceInfoDict = new Dictionary<int, OldResourceInfo>();
					List<int> dictypes = getRestDataDict.Keys.ToList<int>();
					foreach (int type in dictypes)
					{
						OldResourceInfo info = null;
						int type;
						CGetOldResourceManager.ComputeResourceByType(client, type, getRestDataDict, out info);
						if (info != null)
						{
							ResourceInfoDict[type] = info;
							ResourceInfoDict[type].roleId = client.ClientData.RoleID;
						}
					}
					client.ClientData.OldResourceInfoDict = ResourceInfoDict;
					CGetOldResourceManager.ReplaceDataToDB(client);
				}
			}
		}

		// Token: 0x06001C17 RID: 7191 RVA: 0x001A59D4 File Offset: 0x001A3BD4
		public static int GiveRoleOldResource(GameClient client, int actType, int goldorZuanshi, int getModel)
		{
			int ret = 0;
			OldResourceInfo dataInfo = null;
			OldResourceInfo dataToclient = null;
			int cost = 0;
			double changeliferate = CGetOldResourceManager.RoleChangelifeRate(client.ClientData.ChangeLifeCount);
			if (getModel == 0)
			{
				if (client.ClientData.OldResourceInfoDict == null || !client.ClientData.OldResourceInfoDict.TryGetValue(actType, out dataInfo))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("CGetOldResource:资源获取失败, dict={0}, actType={1}", client.ClientData.OldResourceInfoDict, actType), null, true);
					return -3;
				}
				if (dataInfo.leftCount == 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("CGetOldResource:资源数量获取异常, leftCount={0}", dataInfo.leftCount), null, true);
					return -3;
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.BondGold.Length)
				{
					if (CGetOldResourceManager.BondGold[goldorZuanshi] != 0.0)
					{
						cost = (int)((double)dataInfo.bandmoney / CGetOldResourceManager.BondGold[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ExpGold.Length)
				{
					if (!changeliferate.Equals(0.0) && CGetOldResourceManager.ExpGold[goldorZuanshi] != 0.0)
					{
						cost += (int)((double)dataInfo.exp / (changeliferate * CGetOldResourceManager.ExpGold[goldorZuanshi]));
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ChengJiu.Length)
				{
					if (CGetOldResourceManager.ChengJiu[goldorZuanshi] != 0.0)
					{
						cost += (int)((double)dataInfo.chengjiu / CGetOldResourceManager.ChengJiu[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ShengWang.Length)
				{
					if (CGetOldResourceManager.ShengWang[goldorZuanshi] != 0.0)
					{
						cost += (int)((double)dataInfo.shengwang / CGetOldResourceManager.ShengWang[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.MoJing.Length)
				{
					if (CGetOldResourceManager.MoJing[goldorZuanshi] != 0.0)
					{
						cost += (int)((double)dataInfo.mojing / CGetOldResourceManager.MoJing[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ZhanGong.Length)
				{
					if (CGetOldResourceManager.ZhanGong[goldorZuanshi] != 0.0)
					{
						cost += (int)((double)dataInfo.zhangong / CGetOldResourceManager.ZhanGong[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.BangZuan.Length)
				{
					if (CGetOldResourceManager.BangZuan[goldorZuanshi] != 0.0)
					{
						cost += (int)((double)dataInfo.bandDiamond / CGetOldResourceManager.BangZuan[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.XingHun.Length)
				{
					if (CGetOldResourceManager.XingHun[goldorZuanshi] != 0.0)
					{
						cost += (int)((double)dataInfo.xinghun / CGetOldResourceManager.XingHun[goldorZuanshi]);
					}
				}
				if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.YuanSuFenMo.Length)
				{
					if (CGetOldResourceManager.YuanSuFenMo[goldorZuanshi] != 0.0)
					{
						cost += (int)((double)dataInfo.yuanSuFenMo / CGetOldResourceManager.YuanSuFenMo[goldorZuanshi]);
					}
				}
				dataToclient = dataInfo;
			}
			else
			{
				dataToclient = new OldResourceInfo();
				int count = 0;
				List<int> dicTypes = client.ClientData.OldResourceInfoDict.Keys.ToList<int>();
				foreach (int type in dicTypes)
				{
					if (client.ClientData.OldResourceInfoDict.TryGetValue(type, out dataInfo))
					{
						if (dataInfo.leftCount != 0)
						{
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.BondGold.Length)
							{
								if (CGetOldResourceManager.BondGold[goldorZuanshi] != 0.0)
								{
									cost += (int)((double)dataInfo.bandmoney / CGetOldResourceManager.BondGold[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ExpGold.Length)
							{
								if (!changeliferate.Equals(0.0) && CGetOldResourceManager.ExpGold[goldorZuanshi] != 0.0)
								{
									cost += (int)((double)dataInfo.exp / (changeliferate * CGetOldResourceManager.ExpGold[goldorZuanshi]));
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ChengJiu.Length)
							{
								if (CGetOldResourceManager.ChengJiu[goldorZuanshi] != 0.0)
								{
									cost += (int)((double)dataInfo.chengjiu / CGetOldResourceManager.ChengJiu[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ShengWang.Length)
							{
								if (CGetOldResourceManager.ShengWang[goldorZuanshi] != 0.0)
								{
									cost += (int)((double)dataInfo.shengwang / CGetOldResourceManager.ShengWang[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.MoJing.Length)
							{
								if (CGetOldResourceManager.MoJing[goldorZuanshi] != 0.0)
								{
									cost += (int)((double)dataInfo.mojing / CGetOldResourceManager.MoJing[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.ZhanGong.Length)
							{
								if (CGetOldResourceManager.ZhanGong[goldorZuanshi] != 0.0)
								{
									cost += (int)((double)dataInfo.zhangong / CGetOldResourceManager.ZhanGong[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.BangZuan.Length)
							{
								if (CGetOldResourceManager.BangZuan[goldorZuanshi] != 0.0)
								{
									cost += (int)((double)dataInfo.bandDiamond / CGetOldResourceManager.BangZuan[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.XingHun.Length)
							{
								if (CGetOldResourceManager.XingHun[goldorZuanshi] != 0.0)
								{
									cost += (int)((double)dataInfo.xinghun / CGetOldResourceManager.XingHun[goldorZuanshi]);
								}
							}
							if (goldorZuanshi >= 0 && goldorZuanshi < CGetOldResourceManager.YuanSuFenMo.Length)
							{
								if (CGetOldResourceManager.YuanSuFenMo[goldorZuanshi] != 0.0)
								{
									cost += (int)((double)dataInfo.yuanSuFenMo / CGetOldResourceManager.YuanSuFenMo[goldorZuanshi]);
								}
							}
							dataToclient.bandmoney += dataInfo.bandmoney;
							dataToclient.exp += dataInfo.exp;
							dataToclient.chengjiu += dataInfo.chengjiu;
							dataToclient.shengwang += dataInfo.shengwang;
							dataToclient.mojing += dataInfo.mojing;
							dataToclient.zhangong += dataInfo.zhangong;
							dataToclient.bandDiamond += dataInfo.bandDiamond;
							dataToclient.xinghun += dataInfo.xinghun;
							dataToclient.yuanSuFenMo += dataInfo.yuanSuFenMo;
							count++;
						}
					}
				}
				if (count == 0)
				{
					return -3;
				}
			}
			int result;
			if (cost <= 0)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("CGetOldResource:资源消耗结算异常, leftCount={0}", cost), null, true);
				result = -3;
			}
			else
			{
				switch (goldorZuanshi)
				{
				case 0:
					if (cost > client.ClientData.Money1 + client.ClientData.YinLiang)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("CGetOldResource:消耗物资不足, cost={0},money1={1},yinliang={2}", cost, client.ClientData.Money1, client.ClientData.YinLiang), null, true);
						return -1;
					}
					if (Global.SubBindTongQianAndTongQian(client, cost, "资源找回"))
					{
						if (dataToclient.exp > 0)
						{
							long giveexp = (long)((float)dataToclient.exp * CGetOldResourceManager.GoldRate);
							GameManager.ClientMgr.ProcessRoleExperience(client, giveexp, true, true, false, "none");
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(30, new object[0]), new object[]
							{
								giveexp
							}), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
						}
						if (dataToclient.mojing > 0)
						{
							GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, (int)((float)dataToclient.mojing * CGetOldResourceManager.GoldRate), "资源找回(金币)", false, true, false);
						}
						if (dataToclient.shengwang > 0)
						{
							GameManager.ClientMgr.ModifyShengWangValue(client, (int)((float)dataToclient.shengwang * CGetOldResourceManager.GoldRate), "资源找回(金币)", false, true);
						}
						if (dataToclient.bandmoney > 0)
						{
							int givemoney = (int)((float)dataToclient.bandmoney * CGetOldResourceManager.GoldRate);
							GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, givemoney, "金币资源找回", true);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(31, new object[0]), new object[]
							{
								givemoney
							}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
						}
						if (dataToclient.chengjiu > 0)
						{
							GameManager.ClientMgr.ModifyChengJiuPointsValue(client, (int)((float)dataToclient.chengjiu * CGetOldResourceManager.GoldRate), "资源找回(金币)", false, true);
						}
						if (dataToclient.zhangong > 0)
						{
							int zhangong = (int)((float)dataToclient.zhangong * CGetOldResourceManager.GoldRate);
							GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref zhangong, AddBangGongTypes.None, 0);
						}
						if (dataToclient.bandDiamond > 0)
						{
							GameManager.ClientMgr.AddUserGold(client, (int)((float)dataToclient.bandDiamond * CGetOldResourceManager.GoldRate), "资源找回获得绑钻");
						}
						if (dataToclient.xinghun > 0)
						{
							GameManager.ClientMgr.ModifyStarSoulValue(client, (int)((float)dataToclient.xinghun * CGetOldResourceManager.GoldRate), "资源找回获得星魂", true, true);
						}
						if (dataToclient.yuanSuFenMo > 0)
						{
							GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, (int)((float)dataToclient.yuanSuFenMo * CGetOldResourceManager.GoldRate), "资源找回获得元素粉末", true, false);
						}
					}
					break;
				case 1:
					if (cost > client.ClientData.UserMoney)
					{
						return -2;
					}
					if (GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cost, "资源找回", true, true, false, DaiBiSySType.None))
					{
						if (dataToclient.exp > 0)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, (long)dataToclient.exp, true, true, false, "none");
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(30, new object[0]), new object[]
							{
								dataToclient.exp
							}), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
						}
						if (dataToclient.mojing > 0)
						{
							GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, dataToclient.mojing, "资源找回(钻石)", false, true, false);
						}
						if (dataToclient.shengwang > 0)
						{
							GameManager.ClientMgr.ModifyShengWangValue(client, dataToclient.shengwang, "资源找回(钻石)", false, true);
						}
						if (dataToclient.bandmoney > 0)
						{
							GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, dataToclient.bandmoney, "钻石资源找回", true);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(31, new object[0]), new object[]
							{
								dataToclient.bandmoney
							}), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
						}
						if (dataToclient.chengjiu > 0)
						{
							GameManager.ClientMgr.ModifyChengJiuPointsValue(client, dataToclient.chengjiu, "资源找回(钻石)", false, true);
						}
						if (dataToclient.zhangong > 0)
						{
							GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref dataToclient.zhangong, AddBangGongTypes.None, 0);
						}
						if (dataToclient.bandDiamond > 0)
						{
							GameManager.ClientMgr.AddUserGold(client, dataToclient.bandDiamond, "资源找回获得绑钻");
						}
						if (dataToclient.xinghun > 0)
						{
							GameManager.ClientMgr.ModifyStarSoulValue(client, dataToclient.xinghun, "资源找回获得星魂", true, true);
						}
						if (dataToclient.yuanSuFenMo > 0)
						{
							GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, dataToclient.yuanSuFenMo, "资源找回获得元素粉末", true, false);
						}
					}
					break;
				}
				lock (client)
				{
					if (getModel == 0)
					{
						if (client.ClientData.OldResourceInfoDict != null && client.ClientData.OldResourceInfoDict.ContainsKey(actType))
						{
							client.ClientData.OldResourceInfoDict.Remove(actType);
						}
					}
					else if (client.ClientData.OldResourceInfoDict != null)
					{
						client.ClientData.OldResourceInfoDict.Clear();
					}
				}
				CGetOldResourceManager.ReplaceDataToDB(client);
				result = ret;
			}
			return result;
		}

		// Token: 0x06001C18 RID: 7192 RVA: 0x001A6860 File Offset: 0x001A4A60
		public static void ReplaceDataToDB(GameClient client)
		{
			Dictionary<int, Dictionary<int, OldResourceInfo>> dict = new Dictionary<int, Dictionary<int, OldResourceInfo>>();
			dict[client.ClientData.RoleID] = client.ClientData.OldResourceInfoDict;
			Global.sendToDB<int, byte[]>(10164, DataHelper.ObjectToBytes<Dictionary<int, Dictionary<int, OldResourceInfo>>>(dict), client.ServerId);
		}

		// Token: 0x06001C19 RID: 7193 RVA: 0x001A68A8 File Offset: 0x001A4AA8
		public static Dictionary<int, OldResourceInfo> ReadResourceGetfromDB(GameClient client)
		{
			Dictionary<int, OldResourceInfo> dict = new Dictionary<int, OldResourceInfo>();
			byte[] bytesData = null;
			Dictionary<int, OldResourceInfo> result;
			if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10163, string.Format("{0}", client.ClientData.RoleID), out bytesData, client.ServerId))
			{
				result = dict;
			}
			else if (bytesData == null || bytesData.Length <= 6)
			{
				result = dict;
			}
			else
			{
				int length = BitConverter.ToInt32(bytesData, 0);
				dict = DataHelper.BytesToObject<Dictionary<int, OldResourceInfo>>(bytesData, 6, length - 2);
				result = dict;
			}
			return result;
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x001A6940 File Offset: 0x001A4B40
		public static TCPProcessCmdResults ProcessOldResourceCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				switch (nID)
				{
				case 642:
				{
					if (fields.Length != 1)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					List<OldResourceInfo> infodata = CGetOldResourceManager.GetOldResourceInfo(client);
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<OldResourceInfo>>(infodata, pool, nID);
					break;
				}
				case 643:
				{
					if (fields.Length != 4)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					int actType = Global.SafeConvertToInt32(fields[1]);
					int mOrZ = Global.SafeConvertToInt32(fields[2]);
					int getModel = Global.SafeConvertToInt32(fields[3]);
					int ret = CGetOldResourceManager.GiveRoleOldResource(client, actType, mOrZ, getModel);
					if (ret == 0)
					{
						client._IconStateMgr.CheckZiYuanZhaoHui(client);
						client._IconStateMgr.SendIconStateToClient(client);
					}
					string strcmd = string.Format("{0}:{1}:{2}", ret, actType, getModel);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
					break;
				}
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "QueryOldResource", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x04002A3A RID: 10810
		public static float GoldRate = 0.75f;

		// Token: 0x04002A3B RID: 10811
		private static object _xmlDataMutex = new object();

		// Token: 0x04002A3C RID: 10812
		private static XElement _xmlData = null;

		// Token: 0x04002A3D RID: 10813
		private static double[] _Exp = null;

		// Token: 0x04002A3E RID: 10814
		private static object _ExpMutex = new object();

		// Token: 0x04002A3F RID: 10815
		private static double[] _BondGold = null;

		// Token: 0x04002A40 RID: 10816
		private static object _BondGoldMutex = new object();

		// Token: 0x04002A41 RID: 10817
		private static double[] _MoJing = null;

		// Token: 0x04002A42 RID: 10818
		private static object _MoJingMutex = new object();

		// Token: 0x04002A43 RID: 10819
		private static double[] _ShengWang = null;

		// Token: 0x04002A44 RID: 10820
		private static object _ShengWangMutex = new object();

		// Token: 0x04002A45 RID: 10821
		private static double[] _ChengJiu = null;

		// Token: 0x04002A46 RID: 10822
		private static object _ChengJiuMutex = new object();

		// Token: 0x04002A47 RID: 10823
		private static double[] _ZhanGong = null;

		// Token: 0x04002A48 RID: 10824
		private static object _ZhanGongMutex = new object();

		// Token: 0x04002A49 RID: 10825
		private static double[] _BangZuan = null;

		// Token: 0x04002A4A RID: 10826
		private static object _BangZuanMutex = new object();

		// Token: 0x04002A4B RID: 10827
		private static double[] _XingHun = null;

		// Token: 0x04002A4C RID: 10828
		private static object _XingHunMutex = new object();

		// Token: 0x04002A4D RID: 10829
		private static double[] _YuanSuFenMo = null;

		// Token: 0x04002A4E RID: 10830
		private static object _YuanSuFenMoMutex = new object();

		// Token: 0x04002A4F RID: 10831
		private static double[] _changelifeRate = null;
	}
}
