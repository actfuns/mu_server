using System;
using GameDBServer.Logic.WanMoTa;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	
	public class ModifyWanMoTaCmdProcessor : ICmdProcessor
	{
		
		private ModifyWanMoTaCmdProcessor()
		{
		}

		
		public static ModifyWanMoTaCmdProcessor getInstance()
		{
			return ModifyWanMoTaCmdProcessor.instance;
		}

		
		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				ModifyWanMotaData modifyData = DataHelper.BytesToObject<ModifyWanMotaData>(cmdParams, 0, count);
				if (null == modifyData)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数传输错误, CMD={0}, CmdData={2}", TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, cmdParams), null, true);
					client.sendCmd(10158, string.Format("{0}:{1}", 0, -1));
				}
				else
				{
					string cmd = modifyData.strParams;
					string[] fields = cmd.Split(new char[]
					{
						':'
					});
					if (fields.Length != 6)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, fields.Length, cmdParams), null, true);
						client.sendCmd(10158, string.Format("{0}:{1}", 0, -1));
					}
					else
					{
						fields[4] = modifyData.strSweepReward;
						int roleID = Convert.ToInt32(fields[0]);
						WanMotaInfo dataWanMota = WanMoTaManager.getInstance().getWanMoTaData(roleID);
						if (null == dataWanMota)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("没有找到相应的万魔塔信息，CMD={0}, RoleID={1}", TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, roleID), null, true);
							client.sendCmd(10158, string.Format("{0}:{1}", 0, -1));
						}
						else
						{
							int ret = WanMoTaManager.getInstance().updateWanMoTaData(roleID, fields, 1);
							if (ret < 0)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("数据库更新万魔塔数据失败，CMD={0}, RoleID={1}", TCPGameServerCmds.CMD_DB_MODIFY_WANMOTA, roleID), null, true);
							}
							else
							{
								bool bPassLayerCountChange = false;
								lock (dataWanMota)
								{
									dataWanMota.lFlushTime = DataHelper.ConvertToInt64(fields[1], dataWanMota.lFlushTime);
									int nPassLayerCount = DataHelper.ConvertToInt32(fields[2], dataWanMota.nPassLayerCount);
									dataWanMota.nSweepLayer = DataHelper.ConvertToInt32(fields[3], dataWanMota.nSweepLayer);
									dataWanMota.strSweepReward = DataHelper.ConvertToStr(fields[4], dataWanMota.strSweepReward);
									dataWanMota.lSweepBeginTime = DataHelper.ConvertToInt64(fields[5], dataWanMota.lSweepBeginTime);
									if (nPassLayerCount != dataWanMota.nPassLayerCount)
									{
										dataWanMota.nPassLayerCount = nPassLayerCount;
										bPassLayerCountChange = true;
									}
								}
								if (bPassLayerCountChange)
								{
									WanMoTaManager.getInstance().ModifyWanMoTaPaihangData(dataWanMota, false);
								}
							}
							string strcmd = string.Format("{0}:{1}", roleID, ret);
							client.sendCmd(10158, strcmd);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
				string strcmd = string.Format("{0}:{1}", 0, -1);
			}
		}

		
		private static ModifyWanMoTaCmdProcessor instance = new ModifyWanMoTaCmdProcessor();
	}
}
