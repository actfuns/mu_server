using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.CheatGuard
{
	// Token: 0x0200024E RID: 590
	internal class SpeedUpTickCheck : SingletonTemplate<SpeedUpTickCheck>
	{
		// Token: 0x06000843 RID: 2115 RVA: 0x0007EF0C File Offset: 0x0007D10C
		private SpeedUpTickCheck()
		{
		}

		// Token: 0x06000844 RID: 2116 RVA: 0x0007EF94 File Offset: 0x0007D194
		private void ForceRemove(int roleId)
		{
			lock (this.Mutex)
			{
				this.checkRoleDict.Remove(roleId);
				this.roleLastLog1Ticks.Remove(roleId);
				this.roleLastLog2Ticks.Remove(roleId);
			}
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x0007F000 File Offset: 0x0007D200
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

		// Token: 0x06000846 RID: 2118 RVA: 0x0007F094 File Offset: 0x0007D294
		public void OnLogin(GameClient client)
		{
			if (client != null)
			{
				this.ForceRemove(client.ClientData.RoleID);
			}
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0007F0C4 File Offset: 0x0007D2C4
		public void OnLogout(GameClient client)
		{
			if (client != null)
			{
				this.ForceRemove(client.ClientData.RoleID);
			}
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x0007F0F4 File Offset: 0x0007D2F4
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

		// Token: 0x04000E27 RID: 3623
		private object Mutex = new object();

		// Token: 0x04000E28 RID: 3624
		private Dictionary<int, SpeedUpTickCheck.CheckRoleItem> checkRoleDict = new Dictionary<int, SpeedUpTickCheck.CheckRoleItem>();

		// Token: 0x04000E29 RID: 3625
		private Dictionary<int, long> roleLastLog1Ticks = new Dictionary<int, long>();

		// Token: 0x04000E2A RID: 3626
		private Dictionary<int, long> roleLastLog2Ticks = new Dictionary<int, long>();

		// Token: 0x04000E2B RID: 3627
		private double totalDiffRate = 0.0;

		// Token: 0x04000E2C RID: 3628
		private double totalDiffCnt = 0.0;

		// Token: 0x04000E2D RID: 3629
		private double currUseDiffRate = 1.0;

		// Token: 0x04000E2E RID: 3630
		private int TotalElapsedTimes = 10;

		// Token: 0x04000E2F RID: 3631
		private double TotalElapsedDiffRate = 0.2;

		// Token: 0x0200024F RID: 591
		private class CheckRoleItem
		{
			// Token: 0x04000E30 RID: 3632
			public string UserId;

			// Token: 0x04000E31 RID: 3633
			public int RoleId;

			// Token: 0x04000E32 RID: 3634
			public string RoleName;

			// Token: 0x04000E33 RID: 3635
			public string IpAndPort;

			// Token: 0x04000E34 RID: 3636
			public long LastReportClientTick;

			// Token: 0x04000E35 RID: 3637
			public uint LastReceiveServerMs;

			// Token: 0x04000E36 RID: 3638
			public int MaybeTroubleTimes;

			// Token: 0x04000E37 RID: 3639
			public List<double> MaybeTroubleDiffRates = new List<double>();

			// Token: 0x04000E38 RID: 3640
			public long CliTotalElapsedTicks;

			// Token: 0x04000E39 RID: 3641
			public long SrvTotalElapsedTicks;

			// Token: 0x04000E3A RID: 3642
			public int TotalElapsedTimes;
		}
	}
}
