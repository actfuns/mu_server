using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Interface;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class PerformanceTest
	{
		
		public static List<object> GetClientsByMap(int mapCode)
		{
			List<object> objsList = new List<object>();
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			int totalNum = 0;
			int index = 0;
			GameClient client;
			while ((client = GameManager.ClientMgr.GetNextClient(ref index, true)) != null)
			{
				Point clientGrid = new Point((double)(client.ClientData.PosX / gameMap.MapGridWidth), (double)(client.ClientData.PosY / gameMap.MapGridHeight));
				Point monsterGrid = new Point(0.0, 0.0);
				if (Math.Abs(clientGrid.X - monsterGrid.X) <= (double)Global.MaxCache9XGridNum && Math.Abs(clientGrid.Y - monsterGrid.Y) <= (double)Global.MaxCache9YGridNum)
				{
					objsList.Add(client);
				}
				totalNum++;
				if (totalNum > 1000)
				{
					break;
				}
			}
			return objsList;
		}

		
		public static bool NotifyOthersToMovingForMonsterTest(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int roleID, long startMoveTicks, int currentX, int currentY, int action, int toX, int toY, int cmd, double moveCost = 1.0, string pathString = "", List<object> objsList = null)
		{
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, currentX, currentY, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			objsList = PerformanceTest.GetClientsByMap(mapCode);
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
			{
				roleID,
				mapCode,
				action,
				toX,
				toY,
				moveCost,
				0,
				currentX,
				currentY,
				startMoveTicks,
				pathString
			});
			TCPOutPacket tcpOutPacket = null;
			int runNum = Global.GetRandomNumber(1, 10);
			lock (PerformanceTest.testLock1)
			{
				for (int i = 0; i < runNum; i++)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, cmd);
					lock (PerformanceTest.testLock2)
					{
						lock (PerformanceTest.testLock3)
						{
							DataHelper.CopyBytes(PerformanceTest._Buffer, 0, tcpOutPacket.GetPacketBytes(), 0, tcpOutPacket.PacketDataSize);
						}
					}
					Global._TCPManager.TcpOutPacketPool.Push(tcpOutPacket);
				}
			}
			tcpOutPacket = null;
			return true;
		}

		
		public static void InitForTestMode(GameClient client)
		{
			if (GameManager.TestGamePerformanceMode && (GameManager.TestGamePerformanceForAllUser || client.strUserID == null || client.strUserID.StartsWith("mu")))
			{
				if (GameManager.TestGamePerformanceAllPK)
				{
					client.ClientData.PKMode = 1;
				}
				if (client.ClientData.LoginNum <= 0)
				{
					int occu = Global.CalcOriginalOccupationID(client);
					int[] nArray = GameManager.TestRoleEquipsArrays[occu];
					List<GoodsData> usingGoodsList = Global.GetUsingGoodsList(client.ClientData);
					if (usingGoodsList == null || usingGoodsList.Count == 0)
					{
						client.ClientData.Level = 95;
						client.ClientData.ChangeLifeCount = 7;
						for (int i = 0; i < nArray.Length; i++)
						{
							GoodsData goodsData = new GoodsData
							{
								Id = -1,
								GoodsID = nArray[i],
								Using = 0,
								Forge_level = 10,
								Starttime = "1900-01-01 12:00:00",
								Endtime = "1900-01-01 12:00:00",
								Site = 0,
								Quality = 0,
								Props = "",
								GCount = 1,
								Binding = 1,
								Jewellist = "",
								BagIndex = i,
								AddPropIndex = 0,
								BornIndex = 0,
								Lucky = 1,
								Strong = 0,
								ExcellenceInfo = 63,
								AppendPropLev = 40,
								ChangeLifeLevForEquip = 0
							};
							int nDBid = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, GLang.GetLang(515, new object[0]), false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
							string format = "{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}";
							object[] array = new object[9];
							array[0] = client.ClientData.RoleID;
							array[1] = 1;
							array[2] = nDBid;
							array[3] = goodsData.GoodsID;
							array[4] = 1;
							array[5] = 0;
							array[6] = 1;
							array[7] = goodsData.BagIndex;
							string strCmd = string.Format(format, array);
							Global.ModifyGoodsByCmdParams(client, strCmd, "客户端修改", null);
						}
					}
				}
			}
		}

		
		private static object testLock1 = new object();

		
		private static object testLock2 = new object();

		
		private static object testLock3 = new object();

		
		private static byte[] _Buffer = new byte[4096];
	}
}
