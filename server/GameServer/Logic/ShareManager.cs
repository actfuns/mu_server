using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	
	internal class ShareManager
	{
		
		
		public static List<GoodsData> ShareGoodslist
		{
			get
			{
				List<GoodsData> shareGoodslist;
				if (ShareManager._ShareGoodslist != null && ShareManager._ShareGoodslist.Count > 0)
				{
					shareGoodslist = ShareManager._ShareGoodslist;
				}
				else
				{
					string info = GameManager.systemParamsList.GetParamValueByName("ShareAward");
					lock (ShareManager._ShareGoodsMutex)
					{
						ShareManager._ShareGoodslist = ShareManager.ParseGoodsDataList(info.Split(new char[]
						{
							'|'
						}));
					}
					shareGoodslist = ShareManager._ShareGoodslist;
				}
				return shareGoodslist;
			}
		}

		
		private static List<GoodsData> ParseGoodsDataList(string[] fields)
		{
			List<GoodsData> goodsDataList = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] sa = fields[i].Split(new char[]
				{
					','
				});
				if (sa.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Warning, string.Format("解析分享奖励道具失败, 物品配置项个数错误", new object[0]), null, true);
				}
				else
				{
					int[] goodsFields = Global.StringArray2IntArray(sa);
					GoodsData goodsData = Global.GetNewGoodsData(goodsFields[0], goodsFields[1], 0, goodsFields[3], goodsFields[2], 0, goodsFields[5], 0, goodsFields[6], goodsFields[4], 0);
					goodsDataList.Add(goodsData);
				}
			}
			return goodsDataList;
		}

		
		public static bool CanGetShareAward(GameClient client)
		{
			string oldstr = Global.GetRoleParamByName(client, "DailyShare");
			bool result;
			if (oldstr == null)
			{
				result = false;
			}
			else
			{
				string[] fields = oldstr.Split(new char[]
				{
					','
				});
				string olddayid = fields[0];
				result = (olddayid == TimeUtil.NowDateTime().DayOfYear.ToString() && fields[1] == "0");
			}
			return result;
		}

		
		public static bool HasDoneShare(GameClient client)
		{
			string oldstr = Global.GetRoleParamByName(client, "DailyShare");
			bool result;
			if (oldstr == null)
			{
				result = false;
			}
			else
			{
				string[] fields = oldstr.Split(new char[]
				{
					','
				});
				string olddayid = fields[0];
				result = (olddayid == TimeUtil.NowDateTime().DayOfYear.ToString());
			}
			return result;
		}

		
		public static TCPProcessCmdResults ProcessShareCMD(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int resoult = 0;
				int extdata = 0;
				switch (nID)
				{
				case 663:
					ShareManager.UpdateRoleShareState(client);
					if (ShareManager.HasDoneShare(client))
					{
						if (ShareManager.CanGetShareAward(client))
						{
							extdata = 1;
						}
						else
						{
							extdata = 2;
						}
					}
					else
					{
						extdata = 0;
					}
					break;
				case 664:
					resoult = ShareManager.GiveRoleShareAward(client);
					if (resoult == 0 || resoult == -2)
					{
						extdata = 2;
					}
					else if (resoult == -1)
					{
						extdata = 0;
					}
					else
					{
						extdata = 1;
					}
					break;
				case 665:
					if (ShareManager.HasDoneShare(client))
					{
						if (ShareManager.CanGetShareAward(client))
						{
							extdata = 1;
						}
						else
						{
							extdata = 2;
						}
					}
					else
					{
						extdata = 0;
					}
					break;
				}
				string strcmd = string.Format("{0}:{1}", resoult, extdata);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ProcessShareCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		public static int UpdateRoleShareState(GameClient client)
		{
			int ret = 0;
			string data = Global.GetRoleParamByName(client, "DailyShare");
			if (data == null)
			{
				data = "-1,0";
			}
			string[] fields = data.Split(new char[]
			{
				','
			});
			int result;
			if (fields[0] == TimeUtil.NowDateTime().DayOfYear.ToString())
			{
				result = 1;
			}
			else
			{
				Global.SaveRoleParamsStringToDB(client, "DailyShare", string.Format("{0},{1}", TimeUtil.NowDateTime().DayOfYear, 0), true);
				result = ret;
			}
			return result;
		}

		
		public static int GiveRoleShareAward(GameClient client)
		{
			int ret = 0;
			if (ShareManager.CanGetShareAward(client))
			{
				if (Global.CanAddGoodsDataList(client, ShareManager.ShareGoodslist))
				{
					List<GoodsData> goodlist = ShareManager.ShareGoodslist;
					foreach (GoodsData item in goodlist)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, item.GoodsID, item.GCount, item.Quality, "", item.Forge_level, item.Binding, 0, "", true, 1, "分享", "1900-01-01 12:00:00", item.AddPropIndex, item.BornIndex, item.Lucky, item.Strong, item.ExcellenceInfo, item.AppendPropLev, item.ChangeLifeLevForEquip, item.WashProps, null, 0, true);
					}
					Global.SaveRoleParamsStringToDB(client, "DailyShare", string.Format("{0},{1}", TimeUtil.NowDateTime().DayOfYear, 1), true);
					client.ClientData.AddAwardRecord(RoleAwardMsg.FenXiang, goodlist, false);
					GameManager.ClientMgr.NotifyGetAwardMsg(client, RoleAwardMsg.FenXiang, "");
				}
				else
				{
					ret = -3;
				}
			}
			else if (!ShareManager.HasDoneShare(client))
			{
				ret = -1;
			}
			else
			{
				ret = -2;
			}
			return ret;
		}

		
		private static List<GoodsData> _ShareGoodslist = null;

		
		private static object _ShareGoodsMutex = new object();
	}
}
