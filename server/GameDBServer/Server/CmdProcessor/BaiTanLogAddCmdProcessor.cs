using System;
using System.Text;
using GameDBServer.Logic.BaiTan;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	// Token: 0x020001E9 RID: 489
	public class BaiTanLogAddCmdProcessor : ICmdProcessor
	{
		// Token: 0x06000A34 RID: 2612 RVA: 0x00061744 File Offset: 0x0005F944
		private BaiTanLogAddCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(10150, this);
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x00061760 File Offset: 0x0005F960
		public static BaiTanLogAddCmdProcessor getInstance()
		{
			return BaiTanLogAddCmdProcessor.instance;
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x00061778 File Offset: 0x0005F978
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(cmdParams, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd(30767, "0");
				return;
			}
			string[] fields = cmdData.Split(new char[]
			{
				':'
			});
			if (fields.Length != 13)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
				client.sendCmd(30767, "0");
			}
			else
			{
				int roleID = Convert.ToInt32(fields[0]);
				int otherroleid = Convert.ToInt32(fields[1]);
				string otherrname = fields[2];
				int goodsid = Convert.ToInt32(fields[3]);
				int goodsnum = Convert.ToInt32(fields[4]);
				int forgelevel = Convert.ToInt32(fields[5]);
				int totalprice = Convert.ToInt32(fields[6]);
				int leftyuanbao = Convert.ToInt32(fields[7]);
				int YinLiang = Convert.ToInt32(fields[8]);
				int LeftYinLiang = Convert.ToInt32(fields[9]);
				int tax = Convert.ToInt32(fields[10]);
				int excellenceinfo = Convert.ToInt32(fields[11]);
				string washprops = fields[12];
				BaiTanLogItemData data = new BaiTanLogItemData();
				data.rid = roleID;
				data.OtherRoleID = otherroleid;
				data.OtherRName = otherrname;
				data.GoodsID = goodsid;
				data.GoodsNum = goodsnum;
				data.ForgeLevel = forgelevel;
				data.TotalPrice = totalprice;
				data.LeftYuanBao = leftyuanbao;
				data.YinLiang = YinLiang;
				data.LeftYinLiang = LeftYinLiang;
				data.BuyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				data.Tax = tax;
				data.Excellenceinfo = excellenceinfo;
				data.Washprops = washprops;
				BaiTanManager.getInstance().onAddBaiTanLog(data);
				client.sendCmd<string>(10150, string.Format("{0}", 0));
			}
		}

		// Token: 0x04000C4A RID: 3146
		private static BaiTanLogAddCmdProcessor instance = new BaiTanLogAddCmdProcessor();
	}
}
