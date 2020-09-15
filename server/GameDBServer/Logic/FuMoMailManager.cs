using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	// Token: 0x0200012E RID: 302
	internal class FuMoMailManager : IManager
	{
		// Token: 0x06000500 RID: 1280 RVA: 0x000296F0 File Offset: 0x000278F0
		public static FuMoMailManager getInstance()
		{
			return FuMoMailManager.instance;
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x00029708 File Offset: 0x00027908
		public static string GetTodayGiveList(int rid, int time)
		{
			Dictionary<int, FuMoMailTemp> value = null;
			if (FuMoMailManager.FuMoMailTemps.TryGetValue(time, out value))
			{
				FuMoMailTemp fumoval = null;
				if (value.TryGetValue(rid, out fumoval))
				{
					return fumoval.ReceiverRID;
				}
			}
			return null;
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x00029774 File Offset: 0x00027974
		public static void LoadCurrUserFuMoMailList()
		{
			foreach (KeyValuePair<int, FuMoMailData> it in FuMoMailManager.FuMoMailDatas)
			{
				if (FuMoMailManager.CurrUserMailDatas.ContainsKey(it.Value.ReceiverRID))
				{
					FuMoMailManager.CurrUserMailDatas[it.Value.ReceiverRID].Add(it.Value);
				}
				else
				{
					List<FuMoMailData> datas = new List<FuMoMailData>();
					datas.Add(it.Value);
					FuMoMailManager.CurrUserMailDatas.Add(it.Value.ReceiverRID, datas);
				}
				FuMoMailManager.CurrUserMailDatas[it.Value.ReceiverRID].Sort((FuMoMailData left, FuMoMailData right) => left.SendTime.CompareTo(right.SendTime));
			}
			FuMoMailManager.FuMoMailDatas.Clear();
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x00029888 File Offset: 0x00027A88
		public Dictionary<int, List<FuMoMailData>> GetFuMoMailItemDataListFromCached(int rid)
		{
			Dictionary<int, List<FuMoMailData>> dict = new Dictionary<int, List<FuMoMailData>>();
			List<FuMoMailData> temp = null;
			if (FuMoMailManager.CurrUserMailDatas.TryGetValue(rid, out temp))
			{
				dict.Add(rid, temp);
			}
			return dict;
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x000298C4 File Offset: 0x00027AC4
		public Dictionary<int, FuMoMailTemp> GetFuMoTempDataListFromCached(int nDate, int rid)
		{
			Dictionary<int, FuMoMailTemp> data = null;
			FuMoMailTemp temp = null;
			if (FuMoMailManager.FuMoMailTemps.TryGetValue(nDate, out data))
			{
				if (data.TryGetValue(rid, out temp))
				{
					return data;
				}
			}
			return null;
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x00029908 File Offset: 0x00027B08
		public int GetFuMoTempDataAcceptFromCached(int nDate, int rid)
		{
			Dictionary<int, FuMoMailTemp> data = null;
			FuMoMailTemp temp = null;
			if (FuMoMailManager.FuMoMailTemps.TryGetValue(nDate, out data))
			{
				if (data.TryGetValue(rid, out temp))
				{
					return temp.Accept;
				}
			}
			return -1;
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x00029950 File Offset: 0x00027B50
		public bool UpdataReadStateCached(int rid, int mailid, string today)
		{
			List<FuMoMailData> temp = null;
			if (FuMoMailManager.CurrUserMailDatas.TryGetValue(rid, out temp))
			{
				foreach (FuMoMailData it in temp)
				{
					if (it.MaillID == mailid)
					{
						it.IsRead = 1;
						it.ReadTime = today;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x000299E4 File Offset: 0x00027BE4
		public bool UpdataRemoveMailListCached(string[] mailidList, int rid)
		{
			List<FuMoMailData> data = null;
			bool result;
			if (FuMoMailManager.CurrUserMailDatas.TryGetValue(rid, out data))
			{
				List<FuMoMailData> temp = new List<FuMoMailData>();
				temp.AddRange(data);
				foreach (string it in mailidList)
				{
					if (!(it == ""))
					{
						foreach (FuMoMailData itdata in data)
						{
							if (Convert.ToInt32(it) == itdata.MaillID && !temp.Remove(itdata))
							{
								return false;
							}
						}
					}
				}
				FuMoMailManager.CurrUserMailDatas[rid] = temp;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x00029AE0 File Offset: 0x00027CE0
		public bool UpdataRemoveMailListCached(int mailid, int rid)
		{
			List<FuMoMailData> data = null;
			if (FuMoMailManager.CurrUserMailDatas.TryGetValue(rid, out data))
			{
				foreach (FuMoMailData itdata in data)
				{
					if (itdata.MaillID == mailid)
					{
						if (FuMoMailManager.CurrUserMailDatas[rid].Remove(itdata))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x00029B80 File Offset: 0x00027D80
		public bool UpdateGiveAndListCached(int roleid, int give, int nDate, string recid_list)
		{
			Dictionary<int, FuMoMailTemp> data = null;
			if (FuMoMailManager.FuMoMailTemps.TryGetValue(nDate, out data))
			{
				FuMoMailTemp temp = null;
				if (data.TryGetValue(roleid, out temp))
				{
					FuMoMailManager.FuMoMailTemps[nDate][roleid].Give = give;
					FuMoMailManager.FuMoMailTemps[nDate][roleid].ReceiverRID = recid_list;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x00029BF4 File Offset: 0x00027DF4
		public bool UpdateAcceptCached(int roleid, int accept, int nDate)
		{
			Dictionary<int, FuMoMailTemp> data = null;
			if (FuMoMailManager.FuMoMailTemps.TryGetValue(nDate, out data))
			{
				FuMoMailTemp temp = null;
				if (data.TryGetValue(roleid, out temp))
				{
					FuMoMailManager.FuMoMailTemps[nDate][roleid].Accept = accept;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x00029C50 File Offset: 0x00027E50
		public bool InsertAcceptMapCached(int sendid, string recrid_list, int nDate, int accept, int give)
		{
			Dictionary<int, FuMoMailTemp> addData = new Dictionary<int, FuMoMailTemp>();
			Dictionary<int, FuMoMailTemp> data = null;
			FuMoMailTemp addTemp = new FuMoMailTemp();
			addTemp.SenderRID = sendid;
			addTemp.TodayID = nDate;
			addTemp.Give = give;
			addTemp.ReceiverRID = recrid_list;
			addTemp.Accept = accept;
			bool result;
			if (!FuMoMailManager.FuMoMailTemps.TryGetValue(nDate, out data))
			{
				addData.Add(sendid, addTemp);
				FuMoMailManager.FuMoMailTemps.Add(nDate, addData);
				result = true;
			}
			else if (data.ContainsKey(sendid))
			{
				result = false;
			}
			else
			{
				FuMoMailManager.FuMoMailTemps[nDate].Add(sendid, addTemp);
				result = true;
			}
			return result;
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x00029D14 File Offset: 0x00027F14
		public bool InsertFuMoMailCached(DBManager dbMgr, int sendid, string sendname, int sendjob, int recid, int num, string content, string today)
		{
			FuMoMailData data = new FuMoMailData
			{
				MaillID = FuMoMailManager.MaxMailID + 1,
				SenderRID = sendid,
				SenderRName = sendname,
				SenderJob = sendjob,
				SendTime = today,
				ReceiverRID = recid,
				IsRead = 0,
				ReadTime = "2000-11-11 11:11:11",
				FuMoMoney = num,
				Content = content
			};
			bool result;
			if (data == null)
			{
				result = false;
			}
			else
			{
				FuMoMailManager.MaxMailID = DBQuery.GetMailMaxIDFromTable(dbMgr);
				List<FuMoMailData> datas = null;
				if (FuMoMailManager.CurrUserMailDatas.TryGetValue(recid, out datas))
				{
					FuMoMailManager.CurrUserMailDatas[recid].Add(data);
					FuMoMailManager.CurrUserMailDatas[recid].Sort((FuMoMailData left, FuMoMailData right) => left.SendTime.CompareTo(right.SendTime));
					result = true;
				}
				else
				{
					datas = new List<FuMoMailData>();
					datas.Add(data);
					FuMoMailManager.CurrUserMailDatas.Add(recid, datas);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x00029E24 File Offset: 0x00028024
		public int MaxLimitContorl(int rid)
		{
			List<FuMoMailData> data;
			if (FuMoMailManager.CurrUserMailDatas.TryGetValue(rid, out data))
			{
				if (data.Count > 50)
				{
					return data.Count - 50;
				}
			}
			return 0;
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x00029E6C File Offset: 0x0002806C
		public string MakeDelListSQL(string[] mailidList)
		{
			string parem = null;
			int num = 0;
			foreach (string it in mailidList)
			{
				string temp = string.Format(" maillid={0} ", it);
				if (it == "")
				{
					break;
				}
				num++;
				if (num == 1)
				{
					parem += temp;
				}
				else
				{
					parem = parem + " OR " + temp;
				}
			}
			return parem;
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x00029EF8 File Offset: 0x000280F8
		public int DelFuMoMailFromLimitContorl(DBManager dbMgr, int roleid, int num)
		{
			int result;
			if (num > 0)
			{
				List<FuMoMailData> dataList = null;
				if (FuMoMailManager.CurrUserMailDatas.TryGetValue(roleid, out dataList))
				{
					int temp = 0;
					string removeMailIdList = null;
					List<FuMoMailData> tempDataList = new List<FuMoMailData>(dataList);
					foreach (FuMoMailData itdata in dataList)
					{
						temp++;
						if (temp > num)
						{
							break;
						}
						if (!tempDataList.Remove(itdata))
						{
							return -1;
						}
						removeMailIdList = string.Format("{0}_{1}", itdata.MaillID, removeMailIdList);
					}
					FuMoMailManager.CurrUserMailDatas[roleid] = tempDataList;
					if (removeMailIdList == null)
					{
						return 0;
					}
					string parem = this.MakeDelListSQL(removeMailIdList.Split(new char[]
					{
						'_'
					}));
					if (DBWriter.DeleteMailFuMoByMailIDList(dbMgr, roleid, parem))
					{
						return 1;
					}
				}
				result = -1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x0002A030 File Offset: 0x00028230
		public void LoadFuMoInfoFromDB(DBManager dbMgr)
		{
			FuMoMailManager.FuMoMailDatas = DBQuery.GetFuMoMailCached(dbMgr);
			FuMoMailManager.FuMoMailTemps = DBQuery.GetFuMoMailTempCached(dbMgr);
			FuMoMailManager.MaxMailID = DBQuery.GetMailMaxIDFromTable(dbMgr);
			FuMoMailManager.LoadCurrUserFuMoMailList();
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x0002A05C File Offset: 0x0002825C
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x0002A070 File Offset: 0x00028270
		public bool startup()
		{
			return true;
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x0002A084 File Offset: 0x00028284
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x0002A098 File Offset: 0x00028298
		public bool destroy()
		{
			return true;
		}

		// Token: 0x040007C7 RID: 1991
		public static int MaxMailID = 0;

		// Token: 0x040007C8 RID: 1992
		public static Dictionary<int, FuMoMailData> FuMoMailDatas = new Dictionary<int, FuMoMailData>();

		// Token: 0x040007C9 RID: 1993
		public static Dictionary<int, Dictionary<int, FuMoMailTemp>> FuMoMailTemps = new Dictionary<int, Dictionary<int, FuMoMailTemp>>();

		// Token: 0x040007CA RID: 1994
		public static Dictionary<int, List<FuMoMailData>> CurrUserMailDatas = new Dictionary<int, List<FuMoMailData>>();

		// Token: 0x040007CB RID: 1995
		private static FuMoMailManager instance = new FuMoMailManager();
	}
}
