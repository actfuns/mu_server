using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.CheatGuard
{
	
	internal class SpeedUpTickCheck : SingletonTemplate<SpeedUpTickCheck>
	{
		
		private SpeedUpTickCheck()
		{
		}

		
		private void ForceRemove(int roleId)
		{
			lock (this.Mutex)
			{
				this.checkRoleDict.Remove(roleId);
				this.roleLastLog1Ticks.Remove(roleId);
				this.roleLastLog2Ticks.Remove(roleId);
			}
		}

		
		public void LoadConfig()
		{
			try
			{
				string[] arr = GameManager.systemParamsList.GetParamValueByName("SpeedUpTickCheck").Split(new char[]
				{
					','
				});
				this.TotalElapsedTimes = Convert.ToInt32(arr[0]);
				this.TotalElapsedDiffRate = Convert.ToDouble(arr[1]);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, ex.Message.ToString(), null, true);
				this.TotalElapsedTimes = 10;
				this.TotalElapsedDiffRate = 0.2;
			}
		}

		
		public void OnLogin(GameClient client)
		{
			if (client != null)
			{
				this.ForceRemove(client.ClientData.RoleID);
			}
		}

		
		public void OnLogout(GameClient client)
		{
			if (client != null)
			{
				this.ForceRemove(client.ClientData.RoleID);
			}
		}

		
		public void OnClientHeart(GameClient client, long reportRealClientTick)
		{
			if (client != null)
			{
				if (!client.ClientSocket.session.IsGM)
				{
					lock (this.Mutex)
					{
						SpeedUpTickCheck.CheckRoleItem roleItem = null;
						if (!this.checkRoleDict.TryGetValue(client.ClientData.RoleID, out roleItem))
						{
							roleItem = new SpeedUpTickCheck.CheckRoleItem();
							roleItem.RoleId = client.ClientData.RoleID;
							roleItem.RoleName = client.ClientData.RoleName;
							roleItem.UserId = client.strUserID;
							roleItem.IpAndPort = RobotTaskValidator.getInstance().GetIp(client);
							roleItem.LastReportClientTick = reportRealClientTick;
							roleItem.LastReceiveServerMs = TimeUtil.timeGetTime();
							roleItem.MaybeTroubleTimes = 0;
							roleItem.MaybeTroubleDiffRates.Clear();
							roleItem.CliTotalElapsedTicks = 0L;
							roleItem.SrvTotalElapsedTicks = 0L;
							roleItem.TotalElapsedTimes = 0;
							this.checkRoleDict[client.ClientData.RoleID] = roleItem;
						}
						else
						{
							long _LastReportClientTick = roleItem.LastReportClientTick;
							uint _LastReceiveServerMs = roleItem.LastReceiveServerMs;
							roleItem.LastReportClientTick = reportRealClientTick;
							roleItem.LastReceiveServerMs = TimeUtil.timeGetTime();
							uint serverDiffMs = roleItem.LastReceiveServerMs - _LastReceiveServerMs;
							long serverDiffTick = (long)((ulong)serverDiffMs * 10000UL);
							long clientDiffTick = roleItem.LastReportClientTick - _LastReportClientTick;
							if (serverDiffTick > 0L)
							{
								double pipeTickDiffRate = (double)Math.Abs(serverDiffTick - clientDiffTick) * 1.0 / (double)serverDiffTick;
								if (pipeTickDiffRate > this.currUseDiffRate)
								{
									roleItem.MaybeTroubleTimes++;
									roleItem.MaybeTroubleDiffRates.Add(pipeTickDiffRate);
									if (roleItem.MaybeTroubleTimes >= 5)
									{
										long lastLog1Tick = 0L;
										if (!this.roleLastLog1Ticks.TryGetValue(roleItem.RoleId, out lastLog1Tick) || TimeUtil.NowDateTime().Ticks - lastLog1Tick >= 7200000000L)
										{
											this.roleLastLog1Ticks[roleItem.RoleId] = TimeUtil.NowDateTime().Ticks;
											LogManager.WriteLog(LogTypes.Fatal, string.Format("Check1 uid={0},rid={1},rname={2},ip={3} 疑似使用加速, 心跳时间差比例={4}", new object[]
											{
												roleItem.UserId,
												roleItem.RoleId,
												roleItem.RoleName,
												roleItem.IpAndPort,
												string.Join<double>(",", roleItem.MaybeTroubleDiffRates)
											}), null, false);
										}
										roleItem.MaybeTroubleTimes = 0;
										roleItem.MaybeTroubleDiffRates.Clear();
									}
								}
								else if (Global.GetRandom() > 0.6)
								{
									this.totalDiffRate += pipeTickDiffRate;
									this.totalDiffCnt += 1.0;
									if (this.totalDiffCnt >= 100.0)
									{
										double oldDiffRate = this.currUseDiffRate;
										this.currUseDiffRate = this.totalDiffRate / this.totalDiffCnt;
										this.totalDiffCnt = 0.0;
										LogManager.WriteLog(LogTypes.Error, string.Format("加速时间允许时间差范围变更 {0} ---> {1}", oldDiffRate, this.currUseDiffRate), null, true);
									}
								}
								roleItem.CliTotalElapsedTicks += clientDiffTick;
								roleItem.SrvTotalElapsedTicks += serverDiffTick;
								roleItem.TotalElapsedTimes++;
								if (roleItem.TotalElapsedTimes >= this.TotalElapsedTimes)
								{
									double _rate = (double)Math.Abs(roleItem.SrvTotalElapsedTicks - roleItem.CliTotalElapsedTicks) * 1.0 / (double)roleItem.SrvTotalElapsedTicks;
									if (_rate > this.TotalElapsedDiffRate)
									{
										long lastLog2Tick = 0L;
										if (!this.roleLastLog2Ticks.TryGetValue(roleItem.RoleId, out lastLog2Tick) || TimeUtil.NowDateTime().Ticks - lastLog2Tick >= 7200000000L)
										{
											this.roleLastLog2Ticks[roleItem.RoleId] = TimeUtil.NowDateTime().Ticks;
											LogManager.WriteLog(LogTypes.Fatal, string.Format("Check2 uid={0},rid={1},rname={2},ip={3} 疑似使用加速, CliTotalElapsedTicks={4}, SrvTotalElapsedTicks={5}, diffRate={6}", new object[]
											{
												roleItem.UserId,
												roleItem.RoleId,
												roleItem.RoleName,
												roleItem.IpAndPort,
												roleItem.CliTotalElapsedTicks,
												roleItem.SrvTotalElapsedTicks,
												_rate
											}), null, false);
										}
									}
									roleItem.CliTotalElapsedTicks = 0L;
									roleItem.SrvTotalElapsedTicks = 0L;
									roleItem.TotalElapsedTimes = 0;
								}
							}
						}
					}
				}
			}
		}

		
		private object Mutex = new object();

		
		private Dictionary<int, SpeedUpTickCheck.CheckRoleItem> checkRoleDict = new Dictionary<int, SpeedUpTickCheck.CheckRoleItem>();

		
		private Dictionary<int, long> roleLastLog1Ticks = new Dictionary<int, long>();

		
		private Dictionary<int, long> roleLastLog2Ticks = new Dictionary<int, long>();

		
		private double totalDiffRate = 0.0;

		
		private double totalDiffCnt = 0.0;

		
		private double currUseDiffRate = 1.0;

		
		private int TotalElapsedTimes = 10;

		
		private double TotalElapsedDiffRate = 0.2;

		
		private class CheckRoleItem
		{
			
			public string UserId;

			
			public int RoleId;

			
			public string RoleName;

			
			public string IpAndPort;

			
			public long LastReportClientTick;

			
			public uint LastReceiveServerMs;

			
			public int MaybeTroubleTimes;

			
			public List<double> MaybeTroubleDiffRates = new List<double>();

			
			public long CliTotalElapsedTicks;

			
			public long SrvTotalElapsedTicks;

			
			public int TotalElapsedTimes;
		}
	}
}
