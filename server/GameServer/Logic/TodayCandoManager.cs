using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.WanMota;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020007A3 RID: 1955
	public class TodayCandoManager
	{
		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06003304 RID: 13060 RVA: 0x002D38F0 File Offset: 0x002D1AF0
		public static XElement xmlData
		{
			get
			{
				lock (TodayCandoManager._xmlDataMutex)
				{
					if (TodayCandoManager._xmlData != null)
					{
						return TodayCandoManager._xmlData;
					}
				}
				XElement xml = null;
				try
				{
					string fileName = "Config/JinRiKeZuo.xml";
					xml = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath(fileName));
				}
				catch (Exception e)
				{
					xml = null;
					LogManager.WriteException(e.ToString());
				}
				lock (TodayCandoManager._xmlDataMutex)
				{
					TodayCandoManager._xmlData = xml;
				}
				return TodayCandoManager._xmlData;
			}
		}

		// Token: 0x06003305 RID: 13061 RVA: 0x002D39D4 File Offset: 0x002D1BD4
		private static int GetLeftCountByType(GameClient client, int type, int copyId)
		{
			int leftnum = 0;
			switch (type)
			{
			case 1:
			{
				DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, 8);
				if (null == dailyTaskData)
				{
					return 10;
				}
				int maxnum = Global.GetMaxDailyTaskNum(client, 8, dailyTaskData);
				leftnum = maxnum - dailyTaskData.RecNum;
				goto IL_4AC;
			}
			case 5:
			{
				int nMapID = Global.GetDaimonSquareCopySceneIDForRole(client);
				DaimonSquareDataInfo bcDataTmp = null;
				Data.DaimonSquareDataInfoList.TryGetValue(nMapID, out bcDataTmp);
				int nDate = TimeUtil.NowDateTime().DayOfYear;
				int nCount = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, 2);
				if (nCount < 0)
				{
					nCount = 0;
				}
				int nVipLev = client.ClientData.VipLevel;
				int nNum = 0;
				int[] nArry = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPEnterDaimonSquareCountAddValue", ',');
				if (nVipLev > 0 && nArry != null && nArry[nVipLev] > 0)
				{
					nNum = nArry[nVipLev];
				}
				leftnum = bcDataTmp.MaxEnterNum + nNum - nCount;
				goto IL_4AC;
			}
			case 6:
			{
				int nMapID = Global.GetBloodCastleCopySceneIDForRole(client);
				BloodCastleDataInfo bcDataTmp2 = null;
				if (!Data.BloodCastleDataInfoList.TryGetValue(nMapID, out bcDataTmp2))
				{
					goto IL_4AC;
				}
				int nDate = TimeUtil.NowDateTime().DayOfYear;
				int nType = 1;
				int nCount = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, nDate, nType);
				if (nCount < 0)
				{
					nCount = 0;
				}
				int nVipLev = client.ClientData.VipLevel;
				int nNum = 0;
				int[] nArry = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPEnterBloodCastleCountAddValue", ',');
				if (nVipLev > 0 && nArry != null && nArry[nVipLev] > 0)
				{
					nNum = nArry[nVipLev];
				}
				leftnum = bcDataTmp2.MaxEnterNum + nNum - nCount;
				goto IL_4AC;
			}
			case 7:
			{
				DateTime now = TimeUtil.NowDateTime();
				string nowTime = TimeUtil.NowDateTime().ToString("HH:mm");
				List<string> timePointsList = GameManager.AngelTempleMgr.m_AngelTempleData.BeginTime;
				leftnum = 0;
				for (int i = 0; i < timePointsList.Count; i++)
				{
					DateTime perpareTime = DateTime.Parse(timePointsList[i]).AddMinutes((double)(GameManager.AngelTempleMgr.m_AngelTempleData.PrepareTime / 60));
					if (now <= perpareTime)
					{
						leftnum++;
					}
				}
				goto IL_4AC;
			}
			case 8:
				leftnum = 1;
				if (SweepWanMotaManager.GetSweepCount(client) >= SweepWanMotaManager.nWanMoTaMaxSweepNum)
				{
					leftnum = 0;
				}
				goto IL_4AC;
			case 9:
			{
				BufferData bufferData = Global.GetBufferDataByID(client, 34);
				leftnum = (int)(bufferData.BufferVal - (long)bufferData.BufferSecs);
				goto IL_4AC;
			}
			case 10:
				leftnum = GameManager.BattleMgr.LeftEnterCount();
				goto IL_4AC;
			case 11:
				leftnum = GameManager.ArenaBattleMgr.LeftEnterCount();
				goto IL_4AC;
			case 13:
				leftnum = JingJiChangManager.getInstance().GetLeftEnterCount(client);
				goto IL_4AC;
			case 15:
			{
				DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, 9);
				if (null == dailyTaskData)
				{
					return Global.MaxTaofaTaskNumForMU;
				}
				int maxnum = Global.GetMaxDailyTaskNum(client, 9, dailyTaskData);
				leftnum = maxnum - dailyTaskData.RecNum;
				goto IL_4AC;
			}
			case 16:
			{
				int temp = 0;
				CaiJiLogic.ReqCaiJiLastNum(client, 0, out temp);
				leftnum = temp;
				goto IL_4AC;
			}
			case 19:
				leftnum = HuanYingSiYuanManager.getInstance().GetLeftCount(client);
				goto IL_4AC;
			}
			if (copyId > 0)
			{
				SystemXmlItem systemFuBenItem = null;
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyId, out systemFuBenItem))
				{
					return -1;
				}
				int enternum = systemFuBenItem.GetIntValue("EnterNumber", -1);
				int finishnum = systemFuBenItem.GetIntValue("FinishNumber", -1);
				int total = (enternum < finishnum) ? finishnum : enternum;
				if (type == 4 || type == 3)
				{
					int[] nAddNum = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJinBiFuBenNum", ',');
					if (type == 3)
					{
						nAddNum = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJinYanFuBenNum", ',');
					}
					if (client.ClientData.VipLevel > 0 && client.ClientData.VipLevel <= VIPEumValue.VIPENUMVALUE_MAXLEVEL && nAddNum != null && nAddNum.Length > VIPEumValue.VIPENUMVALUE_MAXLEVEL)
					{
						total += nAddNum[client.ClientData.VipLevel];
					}
				}
				FuBenData tmpfubdata = Global.GetFuBenData(client, copyId);
				if (null == tmpfubdata)
				{
					return total;
				}
				leftnum = total - tmpfubdata.EnterNum;
			}
			IL_4AC:
			return leftnum;
		}

		// Token: 0x06003306 RID: 13062 RVA: 0x002D3E94 File Offset: 0x002D2094
		private static bool TaskHasDone(GameClient client, int taskID)
		{
			return client.ClientData.MainTaskID >= taskID;
		}

		// Token: 0x06003307 RID: 13063 RVA: 0x002D3EB8 File Offset: 0x002D20B8
		private static List<TodayCandoData> GetRoleCandoData(int typeId, GameClient client)
		{
			List<TodayCandoData> candolist = new List<TodayCandoData>();
			List<TodayCandoData> result;
			if (TodayCandoManager.xmlData == null)
			{
				result = null;
			}
			else
			{
				IEnumerable<XElement> xmlItems = TodayCandoManager.xmlData.Elements();
				int iMaxlevel = -1;
				int iMaxchangelife = -1;
				int ifirst = 0;
				int lastSectype = -1;
				Dictionary<int, List<TodayCandoData>> temp = new Dictionary<int, List<TodayCandoData>>();
				foreach (XElement item in xmlItems)
				{
					if (null != item)
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
							condition2 = TodayCandoManager.TaskHasDone(client, taskid);
						}
						if (type == typeId && condition && condition2)
						{
							TodayCandoData data = new TodayCandoData();
							data.ID = (int)Global.GetSafeAttributeLong(item, "ID");
							int secondtype = (int)Global.GetSafeAttributeLong(item, "SecondType");
							if (ifirst == 0)
							{
								lastSectype = secondtype;
							}
							else if (ifirst != 0 && lastSectype != secondtype)
							{
								iMaxchangelife = curmaxchangelife;
								iMaxlevel = curmaxlevel;
							}
							ifirst++;
							if (iMaxchangelife < curmaxchangelife && lastSectype == secondtype)
							{
								if (temp.ContainsKey(secondtype))
								{
									foreach (TodayCandoData tempitem in temp[secondtype])
									{
										candolist.Remove(tempitem);
									}
									temp[secondtype] = new List<TodayCandoData>();
								}
								iMaxchangelife = curmaxchangelife;
								iMaxlevel = curmaxlevel;
							}
							if (iMaxchangelife == curmaxchangelife && iMaxlevel < curmaxlevel && lastSectype == secondtype)
							{
								if (temp.ContainsKey(secondtype))
								{
									foreach (TodayCandoData tempitem in temp[secondtype])
									{
										candolist.Remove(tempitem);
									}
									temp[secondtype] = new List<TodayCandoData>();
								}
								iMaxchangelife = curmaxchangelife;
								iMaxlevel = curmaxlevel;
							}
							int copyId = (int)Global.GetSafeAttributeLong(item, "CodeID");
							int leftcount = TodayCandoManager.GetLeftCountByType(client, secondtype, copyId);
							if (leftcount > 0)
							{
								data.LeftCount = leftcount;
								candolist.Add(data);
								if (temp.ContainsKey(secondtype))
								{
									temp[secondtype].Add(data);
								}
								else
								{
									temp[secondtype] = new List<TodayCandoData>();
									temp[secondtype].Add(data);
								}
								lastSectype = secondtype;
							}
						}
					}
				}
				result = candolist;
			}
			return result;
		}

		// Token: 0x06003308 RID: 13064 RVA: 0x002D4324 File Offset: 0x002D2524
		public static TCPProcessCmdResults ProcessQueryTodayCandoInfo(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (fields.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int typeid = Global.SafeConvertToInt32(fields[1]);
				List<TodayCandoData> datalist = TodayCandoManager.GetRoleCandoData(typeid, client);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<TodayCandoData>>(datalist, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "QueryTodayCandoInfo", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		// Token: 0x04003F03 RID: 16131
		private static object _xmlDataMutex = new object();

		// Token: 0x04003F04 RID: 16132
		private static XElement _xmlData = null;
	}
}
