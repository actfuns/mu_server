using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	
	internal class FuMoMailManager : IManager
	{
		
		public static FuMoMailManager getInstance()
		{
			return FuMoMailManager.instance;
		}

		
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

		
		public void LoadFuMoInfoFromDB(DBManager dbMgr)
		{
			FuMoMailManager.FuMoMailDatas = DBQuery.GetFuMoMailCached(dbMgr);
			FuMoMailManager.FuMoMailTemps = DBQuery.GetFuMoMailTempCached(dbMgr);
			FuMoMailManager.MaxMailID = DBQuery.GetMailMaxIDFromTable(dbMgr);
			FuMoMailManager.LoadCurrUserFuMoMailList();
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
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

		
		public static int MaxMailID = 0;

		
		public static Dictionary<int, FuMoMailData> FuMoMailDatas = new Dictionary<int, FuMoMailData>();

		
		public static Dictionary<int, Dictionary<int, FuMoMailTemp>> FuMoMailTemps = new Dictionary<int, Dictionary<int, FuMoMailTemp>>();

		
		public static Dictionary<int, List<FuMoMailData>> CurrUserMailDatas = new Dictionary<int, List<FuMoMailData>>();

		
		private static FuMoMailManager instance = new FuMoMailManager();
	}
}
