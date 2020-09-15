using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020004D1 RID: 1233
	public static class ClientCmdCheck
	{
		// Token: 0x060016C7 RID: 5831 RVA: 0x0016343C File Offset: 0x0016163C
		public static long GetClientTicks(GameClient client, long ccTicks = 0L)
		{
			long clientTicks = (long)client.ClientSocket.ClientCmdSecs * 1000L + TimeUtil.Before1970Ticks;
			long result;
			if (Math.Abs(ccTicks - clientTicks) >= 1000L)
			{
				result = TimeUtil.NowTickCount64() - client.ClientData.ClientExtData.ServerClientTimeDiffTicks;
			}
			else
			{
				result = ccTicks;
			}
			return result;
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x00163498 File Offset: 0x00161698
		public static bool ClientCheck(GameClient client)
		{
			ClientExtData clientExtData = client.ClientData.ClientExtData;
			if (Data.CheckTimeBoost && clientExtData.KeepAlive)
			{
				long nowTickCount = TimeUtil.NowTickCount64();
				long clientTicks = (long)client.ClientSocket.ClientCmdSecs * 1000L + TimeUtil.Before1970Ticks;
				long diff = nowTickCount - clientTicks;
				lock (clientExtData)
				{
					if (diff < clientExtData.MinTimeDiff)
					{
						if (clientExtData.ClientLoginClientTicks == 0L)
						{
							clientExtData.ServerLoginTickCount = nowTickCount;
							clientExtData.ClientLoginClientTicks = clientTicks;
							clientExtData.MinTimeDiff = diff - 18000L;
						}
						else if (nowTickCount < clientExtData.ServerLoginTickCount + 20000L)
						{
							clientExtData.MinTimeDiff = diff - 18000L;
						}
						else if (diff < clientExtData.MinTimeDiff)
						{
							clientExtData.KeepAlive = false;
							LogManager.WriteLog(LogTypes.Check, string.Format("客户端时钟偏差过大,可能是加速或调时钟#rid={0},login={1},MinTimeDiff={2},diff={3}", new object[]
							{
								client.ClientData.RoleID,
								clientExtData.ServerLoginTickCount,
								clientExtData.MinTimeDiff,
								diff
							}), null, true);
						}
					}
					else
					{
						lock (clientExtData)
						{
							clientExtData.ServerClientTimeDiffTicks += (diff - clientExtData.ServerClientTimeDiffTicks) / (clientExtData.CalcNum += 1L);
						}
					}
				}
			}
			return clientExtData.KeepAlive;
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x001636A4 File Offset: 0x001618A4
		public static void RecordClientPosition(GameClient client)
		{
			ClientExtData clientExtData = client.ClientData.ClientExtData;
			lock (clientExtData)
			{
				clientExtData.EndMoveTicks = 0L;
				clientExtData.RunStoryboard = false;
				clientExtData.FromX = client.ClientData.PosX;
				clientExtData.FromY = client.ClientData.PosY;
				clientExtData.ToX = client.ClientData.PosX;
				clientExtData.ToY = client.ClientData.PosY;
				clientExtData.MapCode = client.ClientData.MapCode;
				clientExtData.ReservedTicks = 0L;
			}
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x0016375C File Offset: 0x0016195C
		public static void StopClientStoryboard(GameClient client)
		{
			ClientExtData clientExtData = client.ClientData.ClientExtData;
			lock (clientExtData)
			{
				clientExtData.RunStoryboard = false;
			}
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x001637B0 File Offset: 0x001619B0
		public static void ResetClientPosition(GameClient client, int posX, int posY)
		{
			GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, posX, posY, client.ClientData.RoleDirection, 159, 0);
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x001637F4 File Offset: 0x001619F4
		public static void ClientAction(GameClient client, long nowTicks, long reserveTicks)
		{
			ClientExtData clientExtData = client.ClientData.ClientExtData;
			lock (clientExtData)
			{
				if (client.ClientData.MapCode == clientExtData.MapCode)
				{
					clientExtData.CanMoveTicks = ClientCmdCheck.GetClientTicks(client, 0L) + reserveTicks;
				}
			}
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x0016386C File Offset: 0x00161A6C
		public static void ClientStopMove(GameClient client, int x, int y, long startMoveTicks = 0L)
		{
			if (Data.CheckPositionCheat)
			{
				startMoveTicks = ClientCmdCheck.GetClientTicks(client, startMoveTicks);
				bool resetPos = ClientCmdCheck.MoveTo(client, x, y, startMoveTicks, true);
				if (resetPos)
				{
					ClientCmdCheck.ResetClientPosition(client, client.ClientData.PosX, client.ClientData.PosY);
				}
			}
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x001638C4 File Offset: 0x00161AC4
		public static void MoveSpeedChange(GameClient client, double newMoveSpeed)
		{
			if (Data.CheckPositionCheatSpeed)
			{
				long startMoveTicks = ClientCmdCheck.GetClientTicks(client, 0L);
				ClientExtData clientExtData = client.ClientData.ClientExtData;
				lock (clientExtData)
				{
					if (client.ClientData.MapCode == clientExtData.MapCode)
					{
						if (!clientExtData.RunStoryboard)
						{
							if (clientExtData.EndMoveTicks > clientExtData.StartMoveTicks)
							{
								long elapseTicks = startMoveTicks - clientExtData.StartMoveTicks;
								double factor = (double)elapseTicks / (double)(clientExtData.EndMoveTicks - clientExtData.StartMoveTicks);
								if (factor < ClientCmdCheck.MinDistanceFactor)
								{
									int dx = (int)((double)(clientExtData.ToX - clientExtData.FromX) * (1.0 - factor));
									int dy = (int)((double)(clientExtData.ToY - clientExtData.FromY) * (1.0 - factor));
									if (Math.Abs(dx) + Math.Abs(dy) > 50)
									{
										clientExtData.FromX = (client.ClientData.PosX = clientExtData.ToX - dx);
										clientExtData.FromY = (client.ClientData.PosY = clientExtData.ToY - dy);
										dx = clientExtData.ToX - clientExtData.FromX;
										dy = clientExtData.ToY - clientExtData.FromY;
										clientExtData.MaxDistance2 = dx * dx + dy * dy;
										clientExtData.StartMoveTicks = startMoveTicks;
										clientExtData.ReservedTicks = 0L;
										if (newMoveSpeed >= 0.05)
										{
											clientExtData.MoveSpeed = newMoveSpeed;
											clientExtData.EndMoveTicks = startMoveTicks + (long)(Math.Pow((double)clientExtData.MaxDistance2, 0.5) / (ClientCmdCheck.MoveSpeedPerMS * clientExtData.MoveSpeed));
										}
										else
										{
											clientExtData.StartMoveTicks = startMoveTicks;
											clientExtData.FromX = client.ClientData.PosX;
											clientExtData.FromY = client.ClientData.PosY;
										}
									}
									else
									{
										clientExtData.StartMoveTicks = startMoveTicks;
										clientExtData.ToX = client.ClientData.PosX;
										clientExtData.ToY = client.ClientData.PosY;
									}
								}
								else
								{
									clientExtData.StartMoveTicks = startMoveTicks;
									clientExtData.ToX = client.ClientData.PosX;
									clientExtData.ToY = client.ClientData.PosY;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x00163B64 File Offset: 0x00161D64
		public static bool ClientPosition(GameClient client, int x, int y, long startMoveTicks = 0L)
		{
			bool result;
			if (!Data.CheckPositionCheat)
			{
				result = true;
			}
			else
			{
				bool resetPos = false;
				ClientExtData clientExtData = client.ClientData.ClientExtData;
				lock (clientExtData)
				{
					if (client.ClientData.MapCode != clientExtData.MapCode)
					{
						return false;
					}
					int dx = x - clientExtData.ToX;
					int dy = y - clientExtData.ToY;
					if (dx != 0 || dy != 0)
					{
						if (!clientExtData.RunStoryboard)
						{
							if (clientExtData.StartMoveTicks < clientExtData.EndMoveTicks)
							{
								if (startMoveTicks >= clientExtData.EndMoveTicks)
								{
									clientExtData.StartMoveTicks = startMoveTicks;
									client.ClientData.PosX = clientExtData.ToX;
									client.ClientData.PosY = clientExtData.ToY;
								}
							}
							else if (Math.Abs(dx) + Math.Abs(dy) >= 500)
							{
								LogManager.WriteLog(LogTypes.Check, string.Format("ClientPosition位置不匹配#rid={0}", client.ClientData.RoleID), null, true);
								client.ClientData.PosX = clientExtData.ToX;
								client.ClientData.PosY = clientExtData.ToY;
								resetPos = true;
								clientExtData.StartMoveTicks = startMoveTicks;
							}
						}
					}
				}
				if (resetPos)
				{
					ClientCmdCheck.ResetClientPosition(client, client.ClientData.PosX, client.ClientData.PosY);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x00163D30 File Offset: 0x00161F30
		private static bool MoveTo(GameClient client, int x, int y, long startMoveTicks, bool stop)
		{
			bool resetPos = false;
			ClientExtData clientExtData = client.ClientData.ClientExtData;
			lock (clientExtData)
			{
				if (client.ClientData.MapCode != clientExtData.MapCode)
				{
					LogManager.WriteLog(LogTypes.Check, string.Format("MoveTo地图不匹配#rid={2},current={0},last={1}", client.ClientData.MapCode, clientExtData.MapCode, client.ClientData.RoleID), null, true);
					return false;
				}
				if (startMoveTicks < clientExtData.CanMoveTicks)
				{
					LogManager.WriteLog(LogTypes.Check, string.Format("MoveTo未到可移动时间rid={1},#time={0}", clientExtData.CanMoveTicks - startMoveTicks, client.ClientData.RoleID), null, true);
					return false;
				}
				int dx2 = clientExtData.ToX - clientExtData.FromX;
				int dy2 = clientExtData.ToY - clientExtData.FromY;
				if (dx2 != 0 || dy2 != 0)
				{
					int dx3 = x - clientExtData.FromX;
					int dy3 = y - clientExtData.FromY;
					long distance2 = (long)(dx3 * dx3 + dy3 * dy3);
					if (distance2 > 0L)
					{
						long elapseTicks;
						if (startMoveTicks < clientExtData.StopMoveTicks)
						{
							elapseTicks = startMoveTicks - clientExtData.StartMoveTicks - clientExtData.ReservedTicks;
						}
						else
						{
							elapseTicks = clientExtData.StopMoveTicks - clientExtData.StartMoveTicks - clientExtData.ReservedTicks;
						}
						if (!clientExtData.RunStoryboard && client.InSafeRegion)
						{
							clientExtData.ReservedTicks += (long)(Math.Pow((double)distance2, 0.5) / (ClientCmdCheck.MoveSpeedPerMS * clientExtData.MoveSpeed * 0.8) - (double)elapseTicks);
						}
						else
						{
							clientExtData.ReservedTicks += (long)(Math.Pow((double)distance2, 0.5) / (ClientCmdCheck.MoveSpeedPerMS * clientExtData.MoveSpeed) - (double)elapseTicks);
						}
						if (clientExtData.ReservedTicks > ClientCmdCheck.MaxReserveMs)
						{
							resetPos = true;
							LogManager.WriteLog(LogTypes.Check, string.Format("MoveTo时间校验超限#rid={1},ticks={0}", clientExtData.ReservedTicks, client.ClientData.RoleID), null, true);
						}
						else if (clientExtData.ReservedTicks < -100L)
						{
							clientExtData.ReservedTicks = -100L;
						}
					}
				}
				else
				{
					int dx3 = x - clientExtData.FromX;
					int dy3 = y - clientExtData.FromY;
					long distance2 = (long)(dx3 * dx3 + dy3 * dy3);
					if (distance2 > 5000L)
					{
						x = clientExtData.FromX;
						y = clientExtData.FromY;
						resetPos = true;
					}
				}
				if (resetPos)
				{
					clientExtData.ToX = x;
					clientExtData.ToY = y;
					clientExtData.MaxDistance2 = (x - clientExtData.FromX) * (x - clientExtData.FromX) + (y - clientExtData.FromY) * (y - clientExtData.FromY);
				}
				else if (!clientExtData.RunStoryboard)
				{
					client.ClientData.PosX = x;
					client.ClientData.PosY = y;
				}
				if (stop)
				{
					clientExtData.StopMoveTicks = startMoveTicks;
				}
				else
				{
					clientExtData.StopMoveTicks = long.MaxValue;
				}
			}
			return resetPos;
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x001640D0 File Offset: 0x001622D0
		public static void SpritePreMove(GameClient client, int fromX, int fromY, int toX, int toY, long startMoveTicks)
		{
			if (!Data.IgnoreClientPos)
			{
				client.ClientData.PosX = fromX;
				client.ClientData.PosY = fromY;
				client.ClientData.ReportPosTicks = startMoveTicks;
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];
				mapGrid.MoveObject(-1, -1, fromX, fromY, client);
			}
			client.ClientData.DestPoint = new Point((double)toX, (double)toY);
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x00164150 File Offset: 0x00162350
		public static bool ValidateClientMoveStartTicks(GameClient client, long startMoveTicks = 0L)
		{
			double sStartMoveCoe = GameManager.systemParamsList.GetParamValueDoubleByName("CHEAT_STARTMOVE", 0.0);
			bool result;
			if (sStartMoveCoe <= 0.0)
			{
				result = true;
			}
			else
			{
				int nCheckStartMoveCount = (int)GameManager.systemParamsList.GetParamValueIntByName("CHECK_STARTMOVE_COUNT", 0);
				if (nCheckStartMoveCount <= 0)
				{
					result = true;
				}
				else
				{
					long currTicks = (long)((ulong)TimeUtil.timeGetTime());
					if (currTicks - client.CheckCheatData.LastStartMoveServerTicks < 1000L)
					{
						result = true;
					}
					else if (client.CheckCheatData.LastStartMoveTicks <= 0L || client.CheckCheatData.LastStartMoveServerTicks <= 0L)
					{
						client.CheckCheatData.LastStartMoveTicks = startMoveTicks;
						client.CheckCheatData.LastStartMoveServerTicks = currTicks;
						result = true;
					}
					else
					{
						long moveSubTicks = startMoveTicks - client.CheckCheatData.LastStartMoveTicks;
						long moveSubServerTicks = currTicks - client.CheckCheatData.LastStartMoveServerTicks;
						if (moveSubTicks > 0L && moveSubServerTicks > 0L)
						{
							if ((double)moveSubTicks > (double)moveSubServerTicks + Math.Abs((double)moveSubServerTicks * sStartMoveCoe))
							{
								client.CheckCheatData.LastMoveStartMoveTicksCheatNum += 1L;
							}
							else
							{
								client.CheckCheatData.LastMoveStartMoveTicksCheatNum = 0L;
							}
						}
						if (client.CheckCheatData.LastMoveStartMoveTicksCheatNum >= (long)nCheckStartMoveCount)
						{
							int fromX = client.ClientData.PosX;
							int fromY = client.ClientData.PosY;
							GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, fromX, fromY, client.ClientData.RoleDirection, 159, 0);
							client.ClientData.InstantMoveTick = TimeUtil.NOW() + 1000L;
							LogManager.WriteLog(LogTypes.Error, string.Format("通过STARTMOVE指令判断客户端启用的本地进程加速: {0}, {1}, {2} {3}, 断开连接", new object[]
							{
								Global.GetSocketRemoteEndPoint(client.ClientSocket, false),
								client.ClientData.RoleID,
								moveSubTicks,
								moveSubServerTicks
							}), null, true);
							result = false;
						}
						else
						{
							client.CheckCheatData.LastStartMoveTicks = startMoveTicks;
							client.CheckCheatData.LastStartMoveServerTicks = currTicks;
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x001643A4 File Offset: 0x001625A4
		public static bool SpriteMoveCmd(GameClient client, int fromX, int fromY, int toX, int toY, long startMoveTicks, double moveSpeed, List<Point> path, out bool stepMove)
		{
			stepMove = false;
			bool result;
			if (!Data.CheckPositionCheat)
			{
				ClientCmdCheck.SpritePreMove(client, fromX, fromY, toX, toY, startMoveTicks);
				result = true;
			}
			else
			{
				ClientExtData clientExtData = client.ClientData.ClientExtData;
				bool resetPos;
				lock (clientExtData)
				{
					if (client.ClientData.MapCode != clientExtData.MapCode)
					{
						return false;
					}
					resetPos = ClientCmdCheck.MoveTo(client, fromX, fromY, startMoveTicks, false);
					if (!resetPos)
					{
						ClientCmdCheck.SpritePreMove(client, fromX, fromY, toX, toY, startMoveTicks);
						clientExtData.RunStoryboard = false;
						if (moveSpeed < 0.05)
						{
							return false;
						}
						if (path.Count < 2)
						{
							LogManager.WriteLog(LogTypes.Check, string.Format("SpriteMoveCmd路径点不足两个#rid={0}", client.ClientData.RoleID), null, true);
							return false;
						}
						if (path.Count == 2)
						{
							if (Math.Abs(path[0].X - path[1].X) > 1.0 || Math.Abs(path[0].Y - path[1].Y) > 1.0)
							{
								LogManager.WriteLog(LogTypes.Check, string.Format("SpriteMoveCmd,2点路径非法#rid={0}", client.ClientData.RoleID), null, true);
								return false;
							}
							toX = (int)path[1].X * 100 + 50;
							toY = (int)path[1].Y * 100 + 50;
						}
						else if (path.Count == 3)
						{
							if (path[0].X == path[1].X && path[0].Y == path[1].Y)
							{
								if (Math.Abs(path[0].X - path[2].X) + Math.Abs(path[0].Y - path[2].Y) > 2.0)
								{
									clientExtData.RunStoryboard = true;
								}
							}
							else
							{
								if (path[0].X + path[2].X != path[1].X + path[1].X || path[0].Y + path[2].Y != path[1].Y + path[1].Y)
								{
									LogManager.WriteLog(LogTypes.Check, string.Format("SpriteMoveCmd,3点路径非法#rid={0}", client.ClientData.RoleID), null, true);
									return false;
								}
								if (Math.Abs(path[0].X - path[2].X) > 2.0 || Math.Abs(path[0].Y - path[2].Y) > 2.0)
								{
									LogManager.WriteLog(LogTypes.Check, string.Format("SpriteMoveCmd,3点距离非法#rid={0}", client.ClientData.RoleID), null, true);
									return false;
								}
							}
							toX = (int)path[2].X * 100 + 50;
							toY = (int)path[2].Y * 100 + 50;
						}
						else
						{
							clientExtData.RunStoryboard = true;
						}
						clientExtData.FromX = fromX;
						clientExtData.FromY = fromY;
						clientExtData.ToX = toX;
						clientExtData.ToY = toY;
						clientExtData.StartMoveTicks = startMoveTicks;
						clientExtData.MaxDistance2 = (toX - fromX) * (toX - fromX) + (toY - fromY) * (toY - fromY);
						clientExtData.MoveSpeed = moveSpeed;
						clientExtData.EndMoveTicks = startMoveTicks + (long)(Math.Pow((double)clientExtData.MaxDistance2, 0.5) / (ClientCmdCheck.MoveSpeedPerMS * clientExtData.MoveSpeed));
						stepMove = !clientExtData.RunStoryboard;
					}
				}
				if (resetPos)
				{
					ClientCmdCheck.ResetClientPosition(client, clientExtData.FromX, clientExtData.FromY);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x00164914 File Offset: 0x00162B14
		public static string GetLifeLogString(int mapCode, int current, int max, int add)
		{
			string result;
			try
			{
				if (current >= max)
				{
					result = null;
				}
				else
				{
					lock (ClientCmdCheck.MapCodes)
					{
						if ((long)(add * 100) < (long)max * ClientCmdCheck.MinLogAddLifePercent)
						{
							return null;
						}
						if (!ClientCmdCheck.MapCodes.Contains(mapCode))
						{
							return null;
						}
					}
					StackTrace stackTrace = new StackTrace(1, true);
					result = string.Format("mapCode={0},life={1},max={2},add={3}\r\n{4}", new object[]
					{
						mapCode,
						current,
						max,
						add,
						stackTrace.ToString()
					});
				}
			}
			catch (Exception ex)
			{
				result = ex.ToString();
			}
			return result;
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x00164A10 File Offset: 0x00162C10
		public static void WriteZhanLiLogs(GameClient client)
		{
			try
			{
				if (client.ClientData.ChangeLifeCount >= 4)
				{
					bool writeZhanLiLogs = false;
					long nowTicks = TimeUtil.NOW();
					long nowZhanLi = (long)client.ClientData.CombatForce;
					int offsetDay = TimeUtil.GetOffsetDayNow();
					ExtData extData = ExtDataManager.GetClientExtData(client);
					lock (extData.ZhanLiLogged)
					{
						if (extData.OffsetDay != offsetDay)
						{
							extData.LastZhanLi = 0L;
							extData.ZhanLiWriteten = 0L;
							extData.OffsetDay = offsetDay;
							extData.ZhanLiLogNextWriteTicks = nowTicks + (long)Global.GetRandomNumber(20000, 180000);
							extData.ZhanLiLogged.Clear();
						}
						else if (extData.ZhanLiWriteten < nowZhanLi && extData.ZhanLiLogNextWriteTicks < nowTicks)
						{
							extData.ZhanLiLogNextWriteTicks = nowTicks + 5000L;
							if (extData.LastZhanLi != nowZhanLi)
							{
								extData.LastZhanLi = nowZhanLi;
							}
							else if (!extData.ZhanLiLogged.Contains((long)client.ClientData.CombatForce))
							{
								writeZhanLiLogs = true;
								extData.ZhanLiWriteten = nowZhanLi;
								extData.ZhanLiLogged.Add((long)client.ClientData.CombatForce);
							}
						}
					}
					if (writeZhanLiLogs)
					{
						StringBuilder sb = new StringBuilder();
						Global.PrintSomeProps(client, ref sb);
						LogManager.WriteLog(LogTypes.Alert, sb.ToString(), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x00164BEC File Offset: 0x00162DEC
		public static void WriteLifeLogs(GameClient client)
		{
			try
			{
				ClientCmdCheck.WriteZhanLiLogs(client);
				List<string> list = null;
				lock (client.ClientData)
				{
					if (client.ClientData.AddLifeAlertList.Count == 0)
					{
						return;
					}
					list = new List<string>();
					while (client.ClientData.AddLifeAlertList.Count > 0)
					{
						list.Add(client.ClientData.AddLifeAlertList.Dequeue());
					}
				}
				foreach (string str in list)
				{
					LogManager.WriteLog(LogTypes.Trace, string.Format("#AlertLog#AddLifeAlert#rid={0},rname={1},userid={2},{3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						client.strUserID,
						str
					}), null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x040020AA RID: 8362
		public static long MaxCheckTicks = 3000L;

		// Token: 0x040020AB RID: 8363
		public static double MoveSpeedPerMS = 0.5;

		// Token: 0x040020AC RID: 8364
		public static long MaxReserveMs = 200L;

		// Token: 0x040020AD RID: 8365
		public static double MaxDistanceFactor = 1.05;

		// Token: 0x040020AE RID: 8366
		public static double MinDistanceFactor = 0.95;

		// Token: 0x040020AF RID: 8367
		public static long MinLogAddLifeV = 1000L;

		// Token: 0x040020B0 RID: 8368
		public static long MinLogAddLifePercent = 15L;

		// Token: 0x040020B1 RID: 8369
		public static HashSet<int> MapCodes = new HashSet<int>();
	}
}
