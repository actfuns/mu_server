using System;
using System.Runtime.CompilerServices;
using System.Threading;
using AutoCSer.Metadata;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using KF.Contract.Data;
using KF.Remoting;
using Server.Tools;

namespace KF.TcpCall
{
	// Token: 0x02000085 RID: 133
	[AutoCSer.Net.TcpStaticServer.Server(Name = "KfCall", IsServer = true, IsAttribute = true, IsClientAwaiter = false, MemberFilters = MemberFilters.Static, IsSegmentation = true, ClientSegmentationCopyPath = "GameServer\\Remoting\\")]
	public class TestS2KFCommunication
	{
		// Token: 0x060006E5 RID: 1765 RVA: 0x0005B818 File Offset: 0x00059A18
		[AutoCSer.Net.TcpStaticServer.Method(ParameterFlags = ParameterFlags.SerializeBox, ServerTask = ServerTaskType.Queue, IsClientAwaiter = false)]
		public static string SendData(int strLen, bool flag)
		{
			if (TestS2KFCommunication.Flag != flag)
			{
				TestS2KFCommunication.Flag = flag;
				if (TestS2KFCommunication.Flag)
				{
					Console.WriteLine("压测开启");
					object len = strLen;
					TestS2KFCommunication.upDateTimer = new Timer(new TimerCallback(TestS2KFCommunication.UpDateTick), len, 1, 86400000);
					TestS2KFCommunication.CpuData.Start();
					TestS2KFCommunication.UpTickCpuTimer = new Timer(new TimerCallback(TestS2KFCommunication.UpTickCpu), null, 1, 500);
				}
				else
				{
					Console.WriteLine("压测关闭");
					TestS2KFCommunication.upDateTimer.Change(-1, -1);
					TestS2KFCommunication.UpTickCpuTimer.Change(-1, -1);
					TestS2KFCommunication.CpuData.Print();
				}
			}
			char[] ChArray = new char[]
			{
				'A',
				'B',
				'C',
				'D',
				'E',
				'F',
				'G',
				'H',
				'J',
				'K',
				'L',
				'M',
				'N',
				'P',
				'Q',
				'R',
				'S',
				'T',
				'W',
				'V',
				'U',
				'X',
				'Y',
				'Z'
			};
			string str = "";
			for (int i = 0; i < strLen; i++)
			{
				str += ChArray[i % ChArray.Length];
			}
			return str;
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x0005B958 File Offset: 0x00059B58
		private static void UpDateTick(object sender)
		{
			try
			{
				char[] ChArray = new char[]
				{
					'A',
					'B',
					'C',
					'D',
					'E',
					'F',
					'G',
					'H',
					'J',
					'K',
					'L',
					'M',
					'N',
					'P',
					'Q',
					'R',
					'S',
					'T',
					'W',
					'V',
					'U',
					'X',
					'Y',
					'Z'
				};
				string str = "";
				for (int i = 0; i < (int)sender; i++)
				{
					str += ChArray[i % ChArray.Length];
				}
				long allnum = 0L;
				long sunum = 0L;
				while (TestS2KFCommunication.Flag)
				{
					allnum += 1L;
					if (allnum > 50000L)
					{
						break;
					}
					try
					{
						ClientAgentManager.Instance().BroadCastMsg(KFCallMsg.New<string>(KuaFuEventTypes.Test, str), 0);
						sunum += 1L;
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
						Console.WriteLine("UpDateTick Exception");
					}
				}
				Console.WriteLine(string.Format("发送allnum={0}， sunum={1}", allnum - 1L, sunum));
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("UpDateTick Exception");
			}
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x0005BA84 File Offset: 0x00059C84
		private static void UpTickCpu(object sender)
		{
			try
			{
				TestS2KFCommunication.CpuData.GetValue();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				Console.WriteLine("UpDateTick Exception");
			}
		}

		// Token: 0x040003BB RID: 955
		private static bool Flag = false;

		// Token: 0x040003BC RID: 956
		private static Timer upDateTimer = null;

		// Token: 0x040003BD RID: 957
		private static Timer UpTickCpuTimer = null;

		// Token: 0x040003BE RID: 958
		private static CpuModel CpuData = new CpuModel();

		// Token: 0x02000086 RID: 134
		internal static class TcpStaticServer
		{
			// Token: 0x060006EA RID: 1770 RVA: 0x0005BAF8 File Offset: 0x00059CF8
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static string _M23(int strLen, bool flag)
			{
				return TestS2KFCommunication.SendData(strLen, flag);
			}
		}
	}
}
