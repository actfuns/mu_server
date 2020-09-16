using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using KF.Remoting.Data;
using KF.Remoting.IPStatistics;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	
	public class IPStatisticsService : MarshalByRefObject, IIPStatisticsService
	{
		
		public static IPStatisticsService getInstance()
		{
			return IPStatisticsService._Instance;
		}

		
		public override object InitializeLifetimeService()
		{
			IPStatisticsService._Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		
		private IPStatisticsService()
		{
			IPStatisticsService._Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		
		~IPStatisticsService()
		{
			this.BackgroundThread.Abort();
		}

		
		public void ThreadProc(object state)
		{
			for (;;)
			{
				Thread.Sleep(1000);
				int currMinite = Global.GetOffsetMiniteNow();
				if (currMinite != IPStatisticsService.lastMinite)
				{
					try
					{
						this.IPStatisticsProc();
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
					}
					LogManager.WriteLog(LogTypes.IPStatistics, string.Format("minite change  {0} {1}", IPStatisticsService.lastMinite, currMinite), null, true);
					IPStatisticsService.lastMinite = currMinite;
				}
			}
		}

		
		private void IPStatisticsProc()
		{
			Dictionary<int, List<IPStatisticsData>> lastData = new Dictionary<int, List<IPStatisticsData>>();
			lock (IPStatisticsService.dictCurrData)
			{
				foreach (KeyValuePair<int, List<IPStatisticsData>> item in IPStatisticsService.dictCurrData)
				{
					lastData.Add(item.Key, item.Value);
				}
				IPStatisticsService.dictCurrData.Clear();
			}
			Dictionary<long, HashSet<int>> ip2serveridDict = new Dictionary<long, HashSet<int>>();
			Dictionary<long, IPStatisticsData> totalDataDict = new Dictionary<long, IPStatisticsData>();
			Dictionary<int, List<IPOperaData>> IPOperaDataDict = new Dictionary<int, List<IPOperaData>>();
			foreach (KeyValuePair<int, List<IPStatisticsData>> ipDataList in lastData)
			{
				foreach (IPStatisticsData ipData in ipDataList.Value)
				{
					try
					{
						IPOperaData operaData = this.checkIP(ipData, true);
						if (null != operaData)
						{
							List<IPOperaData> opList;
							if (!IPOperaDataDict.TryGetValue(ipDataList.Key, out opList))
							{
								opList = new List<IPOperaData>();
								IPOperaDataDict[ipDataList.Key] = opList;
							}
							opList.Add(operaData);
						}
						if (ip2serveridDict.ContainsKey(ipData.ipAsInt))
						{
							ip2serveridDict[ipData.ipAsInt].Add(ipDataList.Key);
						}
						else
						{
							HashSet<int> hashServerID = new HashSet<int>();
							hashServerID.Add(ipDataList.Key);
							ip2serveridDict.Add(ipData.ipAsInt, hashServerID);
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(LogTypes.Error, "IPStatisticsProc", ex, false);
					}
					IPStatisticsData tmpIPData = null;
					if (totalDataDict.TryGetValue(ipData.ipAsInt, out tmpIPData))
					{
						tmpIPData += ipData;
					}
					else
					{
						totalDataDict.Add(ipData.ipAsInt, ipData);
					}
				}
			}
			List<IPOperaData> resultList = new List<IPOperaData>();
			foreach (KeyValuePair<long, IPStatisticsData> ipData2 in totalDataDict)
			{
				IPOperaData operaData = this.checkIP(ipData2.Value, false);
				if (null != operaData)
				{
					resultList.Add(operaData);
				}
			}
			this.AddOpList(IPOperaDataDict, resultList, ip2serveridDict);
			lock (IPStatisticsService.dictIPOperaData)
			{
				IPStatisticsService.dictIPOperaData = IPOperaDataDict;
			}
		}

		
		private void AddOpList(Dictionary<int, List<IPOperaData>> IPOperaDataDict, List<IPOperaData> resultList, Dictionary<long, HashSet<int>> ip2serveridDict)
		{
			foreach (IPOperaData result in resultList)
			{
				HashSet<int> hashServerID = null;
				if (ip2serveridDict.TryGetValue(result.ipAsInt, out hashServerID))
				{
					foreach (int serverid in hashServerID)
					{
						List<IPOperaData> operaDataList = null;
						if (!IPOperaDataDict.TryGetValue(serverid, out operaDataList))
						{
							operaDataList = new List<IPOperaData>();
							IPOperaDataDict.Add(serverid, operaDataList);
						}
						operaDataList.Add(result);
					}
				}
			}
		}

		
		private IPOperaData checkIP(IPStatisticsData ipData, bool local)
		{
			IPOperaData operaData = null;
			foreach (StatisticsControl config in IPStatisticsPersistence.Instance._IPControlList)
			{
				if (config.Local == local && !this.checkIP(ipData, config))
				{
					if (!IPStatisticsPersistence.Instance.isCanPassIP(ipData.ipAsInt))
					{
						if (null == operaData)
						{
							operaData = new IPOperaData();
						}
						operaData.ipAsInt = ipData.ipAsInt;
						if (config.OperaType >= 0)
						{
							if (config.OperaParam > operaData.OperaTime[config.OperaType])
							{
								operaData.OperaTime[config.OperaType] = config.OperaParam;
							}
							if (ipData.IPInfoParams[config.ParamType] > operaData.OperaParam[config.OperaType])
							{
								operaData.OperaParam[config.OperaType] = ipData.IPInfoParams[config.ParamType];
							}
						}
						string logmsg = string.Format("cant pass ip={0}:{1} ruleid={2} paramValue={3}", new object[]
						{
							operaData.ipAsInt,
							IpHelper.IntToIp(operaData.ipAsInt),
							config.ID,
							ipData.IPInfoParams[config.ParamType]
						});
						if (config.ComParamType >= 0)
						{
							logmsg += string.Format(" comParamValue={0}", ipData.IPInfoParams[config.ComParamType]);
						}
						LogManager.WriteLog(LogTypes.IPStatistics, logmsg, null, true);
					}
				}
			}
			return operaData;
		}

		
		private bool checkIP(IPStatisticsData IPData, StatisticsControl config)
		{
			bool bPass = true;
			if (config.ParamLimit > 0)
			{
				if (IPData.IPInfoParams[config.ParamType] >= config.ParamLimit)
				{
					bPass = false;
				}
			}
			if (!bPass)
			{
				if (config.ComParamType >= 0)
				{
					double coe = double.MaxValue;
					if (IPData.IPInfoParams[config.ComParamType] > 0)
					{
						coe = (double)IPData.IPInfoParams[config.ParamType] * 1.0 / (double)IPData.IPInfoParams[config.ComParamType];
					}
					bPass = ((config.ComParamLimit > 0.0) ? (coe > config.ComParamLimit) : (coe < -config.ComParamLimit));
				}
			}
			return bPass;
		}

		
		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.ServerId != 0)
				{
					bool bFirstInit = false;
					int ret = ClientAgentManager.Instance().InitializeClient(clientInfo, out bFirstInit);
					if (ret > 0)
					{
					}
					result = ret;
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
					result = -4003;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1},info:{2}", clientInfo.ServerId, clientInfo.ClientId, clientInfo.Token));
				result = -11003;
			}
			return result;
		}

		
		public int RequestMinite()
		{
			return IPStatisticsService.lastMinite;
		}

		
		public int IPStatisticsDataReport(int serverId, int lastMinite, List<IPStatisticsData> list)
		{
			int result;
			if (!ClientAgentManager.Instance().ExistAgent(serverId))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("IPStatisticsDataReport时ServerId错误.ServerId:{0},roleId:{1}", serverId), null, true);
				result = -500;
			}
			else
			{
				lock (IPStatisticsService.dictCurrData)
				{
					IPStatisticsService.dictCurrData.Add(serverId, list);
				}
				LogManager.WriteLog(LogTypes.IPStatistics, string.Format("recv ip report serverid={0} minite={1} count={2}", serverId, lastMinite, list.Count), null, true);
				result = 0;
			}
			return result;
		}

		
		public List<IPOperaData> GetIPStatisticsResult(int serverId)
		{
			List<IPOperaData> result;
			if (!ClientAgentManager.Instance().ExistAgent(serverId))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("GetIPStatisticsResult时ServerId错误.ServerId:{0},roleId:{1}", serverId), null, true);
				result = null;
			}
			else
			{
				List<IPOperaData> list = null;
				lock (IPStatisticsService.dictIPOperaData)
				{
					if (IPStatisticsService.dictIPOperaData.TryGetValue(serverId, out list))
					{
						return list;
					}
				}
				result = list;
			}
			return result;
		}

		
		private static IPStatisticsService _Instance = null;

		
		public readonly GameTypes IPGameType = GameTypes.IPStatistics;

		
		private static int lastMinite = Global.GetOffsetMiniteNow();

		
		public Thread BackgroundThread;

		
		private static Dictionary<int, List<IPStatisticsData>> dictCurrData = new Dictionary<int, List<IPStatisticsData>>();

		
		private static Dictionary<int, List<IPOperaData>> dictIPOperaData = new Dictionary<int, List<IPOperaData>>();
	}
}
