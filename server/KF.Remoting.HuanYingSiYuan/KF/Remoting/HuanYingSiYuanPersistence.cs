using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Maticsoft.DBUtility;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	// Token: 0x02000020 RID: 32
	public class HuanYingSiYuanPersistence
	{
		// Token: 0x060000F2 RID: 242 RVA: 0x0000C6A0 File Offset: 0x0000A8A0
		private HuanYingSiYuanPersistence()
		{
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000C6F0 File Offset: 0x0000A8F0
		public void InitConfig()
		{
			try
			{
				XElement xmlFile = ConfigHelper.Load("config.xml");
				Consts.HuanYingSiYuanRoleCountTotal = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "HuanYingSiYuanRoleCountTotal", "value", 16L);
				Consts.HuanYingSiYuanRoleCountPerSide = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "HuanYingSiYuanRoleCountPerSide", "value", 8L);
				this.SignUpWaitSecs1 = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "SignUpWaitSecs1", "value", 30L);
				this.SignUpWaitSecs2 = (int)ConfigHelper.GetElementAttributeValueLong(xmlFile, "add", "key", "SignUpWaitSecs2", "value", 60L);
				if (this.CurrGameId == Global.UninitGameId)
				{
					this.CurrGameId = (int)((long)DbHelperMySQL.GetSingle("SELECT IFNULL(MAX(id),0) FROM t_hysy_0;"));
				}
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000C7F0 File Offset: 0x0000A9F0
		public void SaveCostTime(int ms)
		{
			try
			{
				if (ms > KuaFuServerManager.WritePerformanceLogMs)
				{
					LogManager.WriteLog(LogTypes.Warning, "HuanYingSiYuan 执行时间(ms):" + ms, null, true);
				}
			}
			catch
			{
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000C844 File Offset: 0x0000AA44
		private void ExecuteSqlNoQuery(string sqlCmd)
		{
			try
			{
				DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000C880 File Offset: 0x0000AA80
		public int GetNextGameId()
		{
			return Interlocked.Add(ref this.CurrGameId, 1);
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000C8A0 File Offset: 0x0000AAA0
		public void LogCreateHysyFuben(int gameId, int kfSrvId, int fubenSeqId, int roleNum)
		{
			string sql = string.Format("INSERT INTO t_hysy_0(`id`,`serverid`,`fubensid`,`createtime`,`rolenum`) VALUES({0},{1},{2},'{3}',{4});", new object[]
			{
				gameId,
				kfSrvId,
				fubenSeqId,
				TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"),
				roleNum
			});
			this.ExecuteSqlNoQuery(sql);
		}

		// Token: 0x040000BB RID: 187
		public static readonly HuanYingSiYuanPersistence Instance = new HuanYingSiYuanPersistence();

		// Token: 0x040000BC RID: 188
		public object Mutex = new object();

		// Token: 0x040000BD RID: 189
		private int CurrGameId = Global.UninitGameId;

		// Token: 0x040000BE RID: 190
		public bool Initialized = false;

		// Token: 0x040000BF RID: 191
		public int SignUpWaitSecs1 = 30;

		// Token: 0x040000C0 RID: 192
		public int SignUpWaitSecs2 = 60;

		// Token: 0x040000C1 RID: 193
		private Queue<GameFuBenStateDbItem> GameFuBenStateDbItemQueue = new Queue<GameFuBenStateDbItem>();
	}
}
