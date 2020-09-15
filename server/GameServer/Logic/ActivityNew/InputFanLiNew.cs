using System;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000029 RID: 41
	public class InputFanLiNew : Activity
	{
		// Token: 0x06000057 RID: 87 RVA: 0x00007348 File Offset: 0x00005548
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/SanZhouNian_ChongZhiFanLi.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/SanZhouNian_ChongZhiFanLi.xml"));
				if (null == xml)
				{
					return false;
				}
				this.ActivityType = 48;
				XElement args = xml.Element("SanZhouNian_ChongZhiFanLi");
				if (null != args)
				{
					this.FromDate = Global.GetSafeAttributeStr(args, "HuoDongKaiQi");
					this.ToDate = Global.GetSafeAttributeStr(args, "HuoDongGuanBi");
					this.AwardStartDate = this.FromDate;
					this.AwardEndDate = this.ToDate;
					this.InputFanLiNewData.ChongZhiJinEList = Global.GetSafeAttributeIntArray(args, "ChongZhiJinE", -1, ',');
					this.InputFanLiNewData.FanZuanShuLiangList = Global.GetSafeAttributeIntArray(args, "FanZuanShuLiang", -1, ',');
					this.InputFanLiNewData.XiaoFeiZuanShiList = Global.GetSafeAttributeIntArray(args, "XiaoFeiZuanShi", -1, ',');
					this.OpenStateVavle = (((int)Global.GetSafeAttributeLong(args, "HuoDongKaiGuan") > 0) ? 1 : 0);
					if (this.InputFanLiNewData.ChongZhiJinEList == null || this.InputFanLiNewData.FanZuanShuLiangList == null || null == this.InputFanLiNewData.XiaoFeiZuanShiList)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常", "Config/SanZhouNian_ChongZhiFanLi.xml"), null, true);
						return false;
					}
					bool lengthCheck = this.InputFanLiNewData.ChongZhiJinEList.Length == this.InputFanLiNewData.FanZuanShuLiangList.Length;
					if (!(lengthCheck & this.InputFanLiNewData.ChongZhiJinEList.Length == this.InputFanLiNewData.XiaoFeiZuanShiList.Length))
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常", "Config/SanZhouNian_ChongZhiFanLi.xml"), null, true);
						return false;
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/SanZhouNian_ChongZhiFanLi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000756C File Offset: 0x0000576C
		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					15,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					15,
					this.OpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, strcmd, false);
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000762C File Offset: 0x0000582C
		public int GetAwardIndex(GameClient client, int chargeMoney, int consumeMoney)
		{
			int awardIndex = 0;
			for (int loop = 0; loop < this.InputFanLiNewData.ChongZhiJinEList.Length; loop++)
			{
				int limiteJinE = Global.TransMoneyToYuanBao(this.InputFanLiNewData.ChongZhiJinEList[loop]);
				if (chargeMoney >= limiteJinE && consumeMoney >= this.InputFanLiNewData.XiaoFeiZuanShiList[loop])
				{
					awardIndex = loop + 1;
				}
			}
			return awardIndex;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00007698 File Offset: 0x00005898
		public override bool CanGiveAward(GameClient client, int index, int totalMoney)
		{
			return this.InAwardTime() && 0 != this.OpenStateVavle && index > 0;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000076DC File Offset: 0x000058DC
		public override bool GiveAward(GameClient client, int index)
		{
			bool result;
			if (index <= 0 || index > this.InputFanLiNewData.FanZuanShuLiangList.Length)
			{
				result = false;
			}
			else
			{
				int AwardYuanBao = this.InputFanLiNewData.FanZuanShuLiangList[index - 1];
				GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, AwardYuanBao, "充值返利新", ActivityTypes.None, "");
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(386, new object[0]), new object[]
				{
					AwardYuanBao
				}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
				GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, AwardYuanBao, "充值返利新"), null, client.ServerId);
				result = true;
			}
			return result;
		}

		// Token: 0x040000E7 RID: 231
		protected const string InputFanLiNewData_fileName = "Config/SanZhouNian_ChongZhiFanLi.xml";

		// Token: 0x040000E8 RID: 232
		protected InputFanLiNewConfig InputFanLiNewData = new InputFanLiNewConfig();

		// Token: 0x040000E9 RID: 233
		protected int OpenStateVavle = 0;
	}
}
