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
	
	public class HuanYingSiYuanPersistence
	{
		
		private HuanYingSiYuanPersistence()
		{
		}

		
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

		
		public int GetNextGameId()
		{
			return Interlocked.Add(ref this.CurrGameId, 1);
		}

		
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

		
		public static readonly HuanYingSiYuanPersistence Instance = new HuanYingSiYuanPersistence();

		
		public object Mutex = new object();

		
		private int CurrGameId = Global.UninitGameId;

		
		public bool Initialized = false;

		
		public int SignUpWaitSecs1 = 30;

		
		public int SignUpWaitSecs2 = 60;

		
		private Queue<GameFuBenStateDbItem> GameFuBenStateDbItemQueue = new Queue<GameFuBenStateDbItem>();
	}
}
