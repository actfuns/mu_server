using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.GoldAuction;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020005B8 RID: 1464
	public class AngelTempleManager
	{
		// Token: 0x06001A85 RID: 6789 RVA: 0x00195C70 File Offset: 0x00193E70
		public void InitAngelTemple()
		{
			Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.AngelTemple);
			this.AngelTempleMonsterUpgradePercent = Global.SafeConvertToDouble(GameManager.GameConfigMgr.GetGameConifgItem("AngelTempleMonsterUpgradeNumber"));
			this.AngelTempleMinHurt = GameManager.systemParamsList.GetParamValueIntByName("AngelTempleMinHurt", -1);
			double[] AngelTempleBossUpgradeParams = GameManager.systemParamsList.GetParamValueDoubleArrayByName("AngelTempleBossUpgrade", ',');
			if (AngelTempleBossUpgradeParams != null && AngelTempleBossUpgradeParams.Length == 4)
			{
				this.AngelTempleBossUpgradeTime = (int)AngelTempleBossUpgradeParams[0];
				this.AngelTempleBossUpgradeParam1 = AngelTempleBossUpgradeParams[1];
				this.AngelTempleBossUpgradeParam2 = AngelTempleBossUpgradeParams[2];
				this.AngelTempleBossUpgradeParam3 = AngelTempleBossUpgradeParams[3];
			}
			this.m_sKillBossRoleName = GameManager.GameConfigMgr.GetGameConifgItem("AngelTempleRole");
			for (int i = 0; i < 5; i++)
			{
				AngelTemplePointInfo tmp = new AngelTemplePointInfo();
				tmp.m_RoleID = 0;
				tmp.m_DamagePoint = 0L;
				tmp.m_GetAwardFlag = 0;
				tmp.m_RoleName = "";
				this.m_PointInfoArray[i] = tmp;
			}
			this.m_BossHP = 10000L;
			SystemXmlItem ItemAngelTempleData = null;
			GameManager.systemAngelTempleData.SystemXmlItemDict.TryGetValue(1, out ItemAngelTempleData);
			if (ItemAngelTempleData == null)
			{
				throw new Exception("AngelTemple Scene ERROR");
			}
			this.m_AngelTempleData.MapCode = ItemAngelTempleData.GetIntValue("MapCode", -1);
			this.m_AngelTempleData.MinChangeLifeNum = ItemAngelTempleData.GetIntValue("MinZhuangSheng", -1);
			this.m_AngelTempleData.MinLevel = ItemAngelTempleData.GetIntValue("MinLevel", -1);
			List<string> strTimeList = new List<string>();
			string timePoints = ItemAngelTempleData.GetStringValue("TimePoints");
			if (timePoints != null && timePoints != "")
			{
				string[] sField = timePoints.Split(new char[]
				{
					','
				});
				for (int i = 0; i < sField.Length; i++)
				{
					strTimeList.Add(sField[i].Trim());
				}
			}
			this.m_AngelTempleData.BeginTime = strTimeList;
			this.m_AngelTempleData.PrepareTime = Global.GMax(ItemAngelTempleData.GetIntValue("PrepareSecs", -1), ItemAngelTempleData.GetIntValue("WaitingEnterSecs", -1));
			this.m_AngelTempleData.DurationTime = ItemAngelTempleData.GetIntValue("FightingSecs", -1);
			this.m_AngelTempleData.LeaveTime = ItemAngelTempleData.GetIntValue("ClearRolesSecs", -1);
			this.m_AngelTempleData.MinPlayerNum = ItemAngelTempleData.GetIntValue("MinRequestNum", -1);
			this.m_AngelTempleData.MaxPlayerNum = ItemAngelTempleData.GetIntValue("MaxEnterNum", -1);
			this.m_AngelTempleData.BossID = ItemAngelTempleData.GetIntValue("BossID", -1);
			this.m_AngelTempleData.BossPosX = ItemAngelTempleData.GetIntValue("BossPosX", -1);
			this.m_AngelTempleData.BossPosY = ItemAngelTempleData.GetIntValue("BossPosY", -1);
		}

		// Token: 0x06001A86 RID: 6790 RVA: 0x00195F34 File Offset: 0x00194134
		public void GMSetHuoDongStartNow()
		{
			this.InitAngelTemple();
			this.m_AngelTempleData.BeginTime = new List<string>
			{
				TimeUtil.NowDateTime().ToString("HH:mm")
			};
			lock (this.m_AngelTempleScene)
			{
				this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_NULL;
			}
		}

		// Token: 0x06001A87 RID: 6791 RVA: 0x00195FBC File Offset: 0x001941BC
		public void OnLoadDynamicMonsters(Monster monster)
		{
			if (monster.MonsterInfo.ExtensionID == this.m_AngelTempleData.BossID)
			{
				this.LastMinDamage = 0L;
				this.m_AngelTempleBoss = monster;
				if (0.0 == this.BossBaseHP)
				{
					this.BossBaseHP = monster.MonsterInfo.VLifeMax;
				}
				if (this.AngelTempleMonsterUpgradePercent <= 0.0)
				{
					this.AngelTempleMonsterUpgradePercent = 1.0;
				}
				this.AngelTempleMonsterUpgradePercent = Global.Clamp(this.AngelTempleMonsterUpgradePercent, 0.001, 1000.0);
				monster.MonsterInfo.VLifeMax = this.BossBaseHP * this.AngelTempleMonsterUpgradePercent;
				monster.VLife = monster.MonsterInfo.VLifeMax;
				this.m_BossHP = (long)monster.MonsterInfo.VLifeMax;
			}
		}

		// Token: 0x06001A88 RID: 6792 RVA: 0x001960AC File Offset: 0x001942AC
		public void SetTotalPointInfo(string sName, long nPoint)
		{
			this.m_sTotalDamageName = sName;
			this.m_nTotalDamageValue = nPoint;
		}

		// Token: 0x06001A89 RID: 6793 RVA: 0x001960C0 File Offset: 0x001942C0
		public void SendTimeInfoToAll(long ticks)
		{
			int nRemainSecs;
			int nStatus;
			lock (this.m_AngelTempleScene)
			{
				nRemainSecs = (int)((this.m_AngelTempleScene.m_lStatusEndTime - ticks) / 1000L);
				nStatus = (int)this.m_AngelTempleScene.m_eStatus;
			}
			GameManager.ClientMgr.NotifyAngelTempleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.m_AngelTempleData.MapCode, 570, null, nStatus, nRemainSecs, 0, 0, 0, 0.0);
		}

		// Token: 0x06001A8A RID: 6794 RVA: 0x00196168 File Offset: 0x00194368
		public void OnEnterScene(GameClient client)
		{
			this.SetLeaveFlag(client, false);
			this.SendTimeInfoToClient(client);
			this.NotifyInfoToClient(client);
			if (null != this.m_AngelTempleBoss)
			{
				this.NotifyInfoToAllClient(this.m_AngelTempleBoss.VLife);
			}
		}

		// Token: 0x06001A8B RID: 6795 RVA: 0x001961B0 File Offset: 0x001943B0
		public void SendTimeInfoToClient(GameClient client)
		{
			long ticks = TimeUtil.NOW();
			int nRemainSecs;
			int nStatus;
			lock (this.m_AngelTempleScene)
			{
				nRemainSecs = (int)((this.m_AngelTempleScene.m_lStatusEndTime - ticks) / 1000L);
				nStatus = (int)this.m_AngelTempleScene.m_eStatus;
			}
			string strcmd = string.Format("{0}:{1}", nStatus, nRemainSecs);
			client.sendCmd(570, strcmd, false);
		}

		// Token: 0x06001A8C RID: 6796 RVA: 0x00196248 File Offset: 0x00194448
		public bool ChangeToNextStatus(out AngelTempleStatus newStatus)
		{
			bool changed = false;
			long ticks = TimeUtil.NOW();
			lock (this.m_AngelTempleScene)
			{
				if (this.m_AngelTempleScene.m_eStatus == AngelTempleStatus.FIGHT_STATUS_NULL)
				{
					if (this.CanEnterAngelTempleOnTime())
					{
						this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_PREPARE;
						this.m_AngelTempleScene.m_lPrepareTime = ticks;
						this.m_AngelTempleScene.m_lStatusEndTime = ticks + (long)(this.m_AngelTempleData.PrepareTime * 1000);
						changed = true;
					}
				}
				else if (this.m_AngelTempleScene.m_eStatus == AngelTempleStatus.FIGHT_STATUS_PREPARE)
				{
					if (ticks >= this.m_AngelTempleScene.m_lStatusEndTime)
					{
						this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_BEGIN;
						this.m_AngelTempleScene.m_lBeginTime = ticks;
						this.m_AngelTempleScene.m_lStatusEndTime = ticks + (long)(this.m_AngelTempleData.DurationTime * 1000);
						changed = true;
					}
				}
				else if (this.m_AngelTempleScene.m_eStatus == AngelTempleStatus.FIGHT_STATUS_BEGIN)
				{
					if (ticks >= this.m_AngelTempleScene.m_lStatusEndTime || this.m_AngelTempleScene.m_bEndFlag != 0)
					{
						this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_END;
						this.m_AngelTempleScene.m_lEndTime = ticks;
						this.m_AngelTempleScene.m_lStatusEndTime = ticks + (long)(this.m_AngelTempleData.LeaveTime * 1000);
						changed = true;
					}
				}
				else if (this.m_AngelTempleScene.m_eStatus == AngelTempleStatus.FIGHT_STATUS_END)
				{
					if (ticks >= this.m_AngelTempleScene.m_lStatusEndTime)
					{
						this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_NULL;
						changed = true;
					}
				}
				newStatus = this.m_AngelTempleScene.m_eStatus;
			}
			return changed;
		}

		// Token: 0x06001A8D RID: 6797 RVA: 0x00196448 File Offset: 0x00194648
		public void HeartBeatAngelTempleScene()
		{
			long ticks = TimeUtil.NOW();
			AngelTempleStatus newStatus;
			if (this.ChangeToNextStatus(out newStatus))
			{
				switch (newStatus)
				{
				case AngelTempleStatus.FIGHT_STATUS_NULL:
				{
					List<object> objsList = GameManager.ClientMgr.GetMapClients(this.m_AngelTempleData.MapCode);
					if (objsList != null)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							GameClient c = objsList[i] as GameClient;
							if (c != null)
							{
								if (c.ClientData.MapCode == this.m_AngelTempleData.MapCode)
								{
									int toMapCode = GameManager.MainMapCode;
									int toPosX = -1;
									int toPosY = -1;
									if (MapTypes.Normal == Global.GetMapType(c.ClientData.LastMapCode))
									{
										if (GameManager.BattleMgr.BattleMapCode != c.ClientData.LastMapCode || GameManager.ArenaBattleMgr.BattleMapCode != c.ClientData.LastMapCode)
										{
											toMapCode = c.ClientData.LastMapCode;
											toPosX = c.ClientData.LastPosX;
											toPosY = c.ClientData.LastPosY;
										}
									}
									GameMap gameMap = null;
									if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
									{
										c.ClientData.bIsInAngelTempleMap = false;
										GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, c, toMapCode, toPosX, toPosY, -1, 0);
									}
								}
							}
						}
					}
					this.CleanUpAngelTempleScene();
					if (ticks >= this.m_AngelTempleScene.m_lEndTime + (long)(this.m_AngelTempleData.LeaveTime * 20000))
					{
						this.m_AngelTempleScene.m_eStatus = AngelTempleStatus.FIGHT_STATUS_NULL;
					}
					break;
				}
				case AngelTempleStatus.FIGHT_STATUS_PREPARE:
					Global.AddFlushIconStateForAll(1007, true);
					break;
				case AngelTempleStatus.FIGHT_STATUS_BEGIN:
				{
					lock (this.m_AngelTempleScene)
					{
						this.bBossKilled = false;
						this.m_AngelTempleScene.m_bEndFlag = 0;
					}
					this.SendTimeInfoToAll(ticks);
					int monsterID = this.m_AngelTempleData.BossID;
					GameMap gameMap = null;
					if (!GameManager.MapMgr.DictMaps.TryGetValue(this.m_AngelTempleData.MapCode, out gameMap))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("天使神殿报错 地图配置 ID = {0}", this.m_AngelTempleData.MapCode), null, true);
						return;
					}
					int gridX = gameMap.CorrectWidthPointToGridPoint(this.m_AngelTempleData.BossPosX) / gameMap.MapGridWidth;
					int gridY = gameMap.CorrectHeightPointToGridPoint(this.m_AngelTempleData.BossPosY) / gameMap.MapGridHeight;
					this.AngelTempleMonsterUpgradePercent = Global.SafeConvertToDouble(GameManager.GameConfigMgr.GetGameConifgItem("AngelTempleMonsterUpgradeNumber"));
					GameManager.MonsterZoneMgr.AddDynamicMonsters(this.m_AngelTempleData.MapCode, monsterID, -1, 1, gridX, gridY, 1, 0, SceneUIClasses.Normal, null, null);
					break;
				}
				case AngelTempleStatus.FIGHT_STATUS_END:
					Global.AddFlushIconStateForAll(1007, false);
					this.SendTimeInfoToAll(ticks);
					if (!this.bBossKilled && this.m_AngelTempleBoss != null)
					{
						MonsterData md = this.m_AngelTempleBoss.GetMonsterData();
						double damage = 0.0;
						if (md.MaxLifeV != md.LifeV)
						{
							damage = Global.Clamp(md.MaxLifeV - md.LifeV, md.MaxLifeV / 10.0, md.MaxLifeV);
							this.AngelTempleMonsterUpgradePercent *= damage * 0.8 / md.MaxLifeV;
							Global.UpdateDBGameConfigg("AngelTempleMonsterUpgradeNumber", this.AngelTempleMonsterUpgradePercent.ToString("0.00"));
						}
						GameManager.MonsterMgr.AddDelayDeadMonster(this.m_AngelTempleBoss);
						GameManager.ClientMgr.NotifyAngelTempleMsgBossDisappear(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.m_AngelTempleData.MapCode);
						LogManager.WriteLog(LogTypes.SQL, string.Format("天使神殿Boss未死亡,血量减少百分比{0:P} ,Boss生命值比例成长为{1}", damage / md.MaxLifeV, this.AngelTempleMonsterUpgradePercent), null, true);
						this.m_AngelTempleBoss = null;
					}
					this.GiveAwardAngelTempleScene(this.bBossKilled);
					break;
				}
			}
			if (newStatus == AngelTempleStatus.FIGHT_STATUS_BEGIN)
			{
			}
		}

		// Token: 0x06001A8E RID: 6798 RVA: 0x001968E8 File Offset: 0x00194AE8
		public void NotifyInfoToAllClient(double nBossHP)
		{
			lock (this.m_PointDamageInfoMutex)
			{
				GameManager.ClientMgr.NotifyAngelTempleMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.m_AngelTempleData.MapCode, 572, this.m_PointInfoArray, 0, 0, 0, 0, 0, nBossHP);
			}
		}

		// Token: 0x06001A8F RID: 6799 RVA: 0x0019696C File Offset: 0x00194B6C
		public void NotifyInfoToClient(GameClient client)
		{
			string strName = Global.FormatRoleName(client, client.ClientData.RoleName);
			double dValue = Math.Round((double)client.ClientData.AngelTempleCurrentPoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
			string strcmd = string.Format("{0}:{1}", strName, dValue);
			GameManager.ClientMgr.SendToClient(client, strcmd, 573);
		}

		// Token: 0x06001A90 RID: 6800 RVA: 0x001969D4 File Offset: 0x00194BD4
		public void ProcessAttackBossInAngelTempleScene(GameClient client, Monster monster, int nDamage)
		{
			if (nDamage > 0)
			{
				AngelTemplePointInfo tmpInfo;
				lock (this.m_PointDamageInfoMutex)
				{
					if (!this.m_RoleDamageAngelValue.TryGetValue(client.ClientData.RoleID, out tmpInfo))
					{
						tmpInfo = new AngelTemplePointInfo();
						tmpInfo.m_RoleID = client.ClientData.RoleID;
						tmpInfo.m_DamagePoint = (long)nDamage;
						tmpInfo.m_GetAwardFlag = 0;
						tmpInfo.m_RoleName = Global.FormatRoleName(client, client.ClientData.RoleName);
						this.m_RoleDamageAngelValue.Add(client.ClientData.RoleID, tmpInfo);
					}
					else
					{
						tmpInfo.m_DamagePoint += (long)nDamage;
					}
					this.AddRoleAuctionData(client, nDamage);
					if (tmpInfo.m_DamagePoint > this.LastMinDamage)
					{
						if (tmpInfo.Ranking < 0)
						{
							this.m_PointInfoArray[5] = tmpInfo;
							tmpInfo.Ranking = 1;
							Array.Sort<AngelTemplePointInfo>(this.m_PointInfoArray, tmpInfo);
						}
						else
						{
							Array.Sort<AngelTemplePointInfo>(this.m_PointInfoArray, 0, 5, tmpInfo);
						}
						if (null != this.m_PointInfoArray[5])
						{
							this.m_PointInfoArray[5].Ranking = -1;
						}
						this.LastMinDamage = ((this.m_PointInfoArray[4] != null) ? this.m_PointInfoArray[4].m_DamagePoint : 0L);
					}
				}
				client.ClientData.AngelTempleCurrentPoint = tmpInfo.m_DamagePoint;
				if (client.ClientData.AngelTempleCurrentPoint > client.ClientData.AngelTempleTopPoint)
				{
					client.ClientData.AngelTempleTopPoint = client.ClientData.AngelTempleCurrentPoint;
				}
				if (tmpInfo.m_DamagePoint > this.m_nTotalDamageValue)
				{
					string strName = Global.FormatRoleName(client, client.ClientData.RoleName);
					this.SetTotalPointInfo(strName, tmpInfo.m_DamagePoint);
				}
				long lTicks = TimeUtil.NOW();
				int percent = (int)(100.0 * monster.VLife / (double)this.m_BossHP);
				if (lTicks >= client.ClientData.m_NotifyInfoTickForSingle + this.m_NotifyInfoDelayTick)
				{
					client.ClientData.m_NotifyInfoTickForSingle = lTicks;
					this.NotifyInfoToClient(client);
				}
				if (lTicks >= this.m_NotifyInfoTickForSingle + this.m_NotifyInfoDelayTick || percent != this.m_LastNotifyBossHPPercent)
				{
					this.m_LastNotifyBossHPPercent = percent;
					this.m_NotifyInfoTickForSingle = lTicks;
					this.NotifyInfoToAllClient(monster.VLife);
				}
			}
		}

		// Token: 0x06001A91 RID: 6801 RVA: 0x00196C84 File Offset: 0x00194E84
		public void GiveAwardAngelTempleScene(bool bBossKilled)
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(this.m_AngelTempleData.MapCode);
			if (null != objsList)
			{
				int roleCount = 0;
				List<AngelTemplePointInfo> pointList = new List<AngelTemplePointInfo>();
				lock (this.m_PointDamageInfoMutex)
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						if (objsList[i] is GameClient)
						{
							GameClient client = objsList[i] as GameClient;
							AngelTemplePointInfo tmpInfo;
							if (!this.m_RoleDamageAngelValue.TryGetValue(client.ClientData.RoleID, out tmpInfo))
							{
								this.SendAngelTempleAwardMsg(client, -1, 0, 0, GLang.GetLang(6, new object[0]), "", bBossKilled);
							}
							else if (!tmpInfo.LeaveScene)
							{
								if (Interlocked.CompareExchange(ref tmpInfo.m_GetAwardFlag, 1, 0) == 0)
								{
									if (tmpInfo.m_DamagePoint < this.AngelTempleMinHurt)
									{
										this.SendAngelTempleAwardMsg(client, -1, 0, 0, GLang.GetLang(6, new object[0]), "", bBossKilled);
									}
									else
									{
										roleCount++;
										pointList.Add(tmpInfo);
									}
								}
							}
						}
					}
				}
				pointList.Sort(new Comparison<AngelTemplePointInfo>(AngelTemplePointInfo.Compare_static));
				if (bBossKilled)
				{
					foreach (KeyValuePair<int, SystemXmlItem> kv in GameManager.AngelTempleAward.SystemXmlItemDict)
					{
						if (null != kv.Value)
						{
							int id = kv.Value.GetIntValue("ID", -1);
							int minPaiMing = kv.Value.GetIntValue("MinPaiMing", -1);
							int maxPaiMing = kv.Value.GetIntValue("MaxPaiMing", -1);
							int shengWang = kv.Value.GetIntValue("ShengWang", -1);
							int gold = kv.Value.GetIntValue("Gold", -1);
							string goodsStr = kv.Value.GetStringValue("Goods");
							minPaiMing = Global.GMax(0, minPaiMing - 1);
							maxPaiMing = Global.GMin(10000, maxPaiMing - 1);
							int i = minPaiMing;
							while (i <= maxPaiMing && i < roleCount)
							{
								pointList[i].m_AwardPaiMing = i + 1;
								pointList[i].m_AwardShengWang += shengWang;
								pointList[i].m_AwardGold += gold;
								pointList[i].GoodsList.AddNoRepeat(goodsStr);
								i++;
							}
						}
					}
					int[] luckPaiMings = new int[roleCount];
					for (int i = 0; i < roleCount; i++)
					{
						luckPaiMings[i] = i;
					}
					int luckAwardsCount = 0;
					foreach (KeyValuePair<int, SystemXmlItem> kv in GameManager.AngelTempleLuckyAward.SystemXmlItemDict)
					{
						if (null != kv.Value)
						{
							int awardID = kv.Value.GetIntValue("ID", -1);
							int awardNum = kv.Value.GetIntValue("Number", -1);
							string luckAwardsName = Global.GetLang(kv.Value.GetStringValue("Name"));
							string luckAwardGoods = kv.Value.GetStringValue("Goods");
							int count = 0;
							while (count < awardNum && luckAwardsCount < roleCount)
							{
								int rand = Global.GetRandomNumber(luckAwardsCount, roleCount);
								int t = luckPaiMings[luckAwardsCount];
								luckPaiMings[luckAwardsCount] = luckPaiMings[rand];
								luckPaiMings[rand] = t;
								int index = luckPaiMings[luckAwardsCount];
								pointList[index].m_LuckPaiMingName = luckAwardsName;
								pointList[index].GoodsList.AddNoRepeat(luckAwardGoods);
								count++;
								luckAwardsCount++;
							}
						}
					}
				}
				else
				{
					SystemXmlItem xmlItem = null;
					foreach (KeyValuePair<int, SystemXmlItem> kv in GameManager.AngelTempleAward.SystemXmlItemDict)
					{
						if (null != kv.Value)
						{
							xmlItem = kv.Value;
						}
					}
					if (null != xmlItem)
					{
						int id = xmlItem.GetIntValue("ID", -1);
						int shengWang = xmlItem.GetIntValue("ShengWang", -1);
						int gold = xmlItem.GetIntValue("Gold", -1);
						string goodsStr = xmlItem.GetStringValue("Goods");
						for (int i = 0; i < roleCount; i++)
						{
							pointList[i].m_AwardPaiMing = -1;
							pointList[i].m_LuckPaiMingName = GLang.GetLang(6, new object[0]);
							pointList[i].m_AwardShengWang = shengWang;
							pointList[i].m_AwardGold = gold;
							pointList[i].GoodsList.AddNoRepeat(goodsStr);
						}
					}
				}
				double awardmuti = 0.0;
				JieRiMultAwardActivity activity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null != activity)
				{
					JieRiMultConfig config = activity.GetConfig(1);
					if (null != config)
					{
						awardmuti += config.GetMult();
					}
				}
				SpecPriorityActivity spAct = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != spAct)
				{
					awardmuti += spAct.GetMult(SpecPActivityBuffType.SPABT_AngelTemple);
				}
				awardmuti = Math.Max(1.0, awardmuti);
				if (awardmuti > 1.0)
				{
					foreach (AngelTemplePointInfo dp in pointList)
					{
						dp.m_AwardGold = (int)((double)dp.m_AwardGold * awardmuti);
						dp.m_AwardShengWang = (int)((double)dp.m_AwardShengWang * awardmuti);
						foreach (AwardsItemData item in dp.GoodsList.Items)
						{
							item.GoodsNum = (int)((double)item.GoodsNum * awardmuti);
						}
					}
				}
				foreach (AngelTemplePointInfo dp in pointList)
				{
					GameClient gc = GameManager.ClientMgr.FindClient(dp.m_RoleID);
					if (null != gc)
					{
						ProcessTask.ProcessAddTaskVal(gc, TaskTypes.AngelTemple, -1, 1, new object[0]);
						if (dp.m_AwardGold > 0)
						{
							GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gc, dp.m_AwardGold, "天使神殿奖励", false);
						}
						if (dp.m_AwardShengWang > 0)
						{
							GameManager.ClientMgr.ModifyShengWangValue(gc, dp.m_AwardShengWang, "天使神殿", true, true);
						}
						foreach (AwardsItemData item in dp.GoodsList.Items)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gc, item.GoodsID, item.GoodsNum, 0, "", item.Level, item.Binding, 0, "", true, 1, "天使神殿奖励物品", "1900-01-01 12:00:00", 0, 0, item.IsHaveLuckyProp, 0, item.ExcellencePorpValue, item.AppendLev, 0, null, null, 0, true);
						}
						this.SendAngelTempleAwardMsg(gc, dp.m_AwardPaiMing, dp.m_AwardGold, dp.m_AwardShengWang, dp.m_LuckPaiMingName, dp.GoodsList.ToString(), bBossKilled);
					}
				}
			}
		}

		// Token: 0x06001A92 RID: 6802 RVA: 0x0019758C File Offset: 0x0019578C
		private void SendAngelTempleAwardMsg(GameClient client, int paiMing, int awardGold, int awardShengWang, string luckPaiMingName, string goodsString, bool success)
		{
			string strcmd;
			if (client.CodeRevision >= 2)
			{
				strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					paiMing,
					awardGold,
					awardShengWang,
					luckPaiMingName,
					goodsString,
					success ? 1 : 0
				});
			}
			else
			{
				strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					paiMing,
					awardGold,
					awardShengWang,
					luckPaiMingName,
					goodsString
				});
			}
			GameManager.ClientMgr.SendToClient(client, strcmd, 571);
		}

		// Token: 0x06001A93 RID: 6803 RVA: 0x00197644 File Offset: 0x00195844
		private void SetLeaveFlag(GameClient client, bool leaveFlag)
		{
			AngelTemplePointInfo tmpInfo = null;
			lock (this.m_PointDamageInfoMutex)
			{
				if (this.m_RoleDamageAngelValue.TryGetValue(client.ClientData.RoleID, out tmpInfo))
				{
					tmpInfo.LeaveScene = leaveFlag;
				}
			}
		}

		// Token: 0x06001A94 RID: 6804 RVA: 0x001976B4 File Offset: 0x001958B4
		public void LeaveAngelTempleScene(GameClient client, bool logout = false)
		{
			this.SetLeaveFlag(client, true);
			if (client.ClientData.MapCode == this.m_AngelTempleData.MapCode || client.ClientData.bIsInAngelTempleMap)
			{
				Interlocked.Decrement(ref this.m_AngelTempleScene.m_nPlarerCount);
				client.ClientData.bIsInAngelTempleMap = false;
				if (logout)
				{
					client.ClientData.MapCode = client.ClientData.LastMapCode;
					client.ClientData.PosX = client.ClientData.LastPosX;
					client.ClientData.PosY = client.ClientData.LastPosY;
				}
			}
		}

		// Token: 0x06001A95 RID: 6805 RVA: 0x00197768 File Offset: 0x00195968
		public bool CanEnterAngelTempleOnTime()
		{
			lock (this.m_AngelTempleScene)
			{
				if (this.m_AngelTempleScene.m_eStatus >= AngelTempleStatus.FIGHT_STATUS_PREPARE && this.m_AngelTempleScene.m_eStatus < AngelTempleStatus.FIGHT_STATUS_END)
				{
					return true;
				}
			}
			DateTime now = TimeUtil.NowDateTime();
			string nowTime = now.ToString("HH:mm");
			List<string> timePointsList = this.m_AngelTempleData.BeginTime;
			bool result;
			if (null == timePointsList)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < timePointsList.Count; i++)
				{
					DateTime staticTime = DateTime.Parse(timePointsList[i]);
					DateTime perpareTime = staticTime.AddMinutes((double)(this.m_AngelTempleData.PrepareTime / 60));
					if (timePointsList[i] == nowTime || (now > staticTime && now <= perpareTime))
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06001A96 RID: 6806 RVA: 0x00197894 File Offset: 0x00195A94
		public bool AddBuffer(GameClient client, BufferItemTypes buffID, double[] newParams, bool notifyPropsChanged)
		{
			if (buffID == BufferItemTypes.MU_ANGELTEMPLEBUFF1)
			{
				Global.RemoveBufferData(client, 86);
			}
			else if (buffID == BufferItemTypes.MU_ANGELTEMPLEBUFF2)
			{
				Global.RemoveBufferData(client, 85);
			}
			int nIndex = 0;
			int nOldBufferGoodsIndexID = -1;
			BufferData bufferData = Global.GetBufferDataByID(client, (int)buffID);
			if (bufferData != null && !Global.IsBufferDataOver(bufferData, 0L))
			{
				nOldBufferGoodsIndexID = (int)bufferData.BufferVal;
			}
			bool result;
			if (nOldBufferGoodsIndexID == nIndex)
			{
				result = false;
			}
			else
			{
				double[] actionParams = new double[2];
				Global.UpdateBufferData(client, buffID, newParams, 1, notifyPropsChanged);
				if (notifyPropsChanged)
				{
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06001A97 RID: 6807 RVA: 0x00197968 File Offset: 0x00195B68
		public void KillAngelBoss(GameClient client, Monster monster)
		{
			if (this.m_AngelTempleData.BossID == monster.MonsterInfo.ExtensionID)
			{
				lock (this.m_AngelTempleScene)
				{
					this.bBossKilled = true;
					this.m_AngelTempleScene.m_bEndFlag = 1;
				}
				string sName = Global.FormatRoleName(client, client.ClientData.RoleName);
				this.m_sKillBossRoleName = sName;
				Global.UpdateDBGameConfigg("AngelTempleRole", this.m_sKillBossRoleName);
				this.NotifyInfoToClient(client);
				this.NotifyInfoToAllClient(monster.VLife);
				this.m_AngelTempleScene.m_nKillBossRole = client.ClientData.RoleID;
				this.m_sKillBossRoleID = client.ClientData.RoleID;
				double usedTime = (double)((TimeUtil.NOW() - this.m_AngelTempleScene.m_lBeginTime) / 1000L);
				double usedTime2 = Global.Clamp(usedTime, (double)(this.m_AngelTempleData.DurationTime / 10), (double)this.m_AngelTempleData.DurationTime);
				this.AngelTempleMonsterUpgradePercent *= (double)this.m_AngelTempleData.DurationTime * 0.8 / usedTime2;
				Global.UpdateDBGameConfigg("AngelTempleMonsterUpgradeNumber", this.AngelTempleMonsterUpgradePercent.ToString("0.00"));
				LogManager.WriteLog(LogTypes.SQL, string.Format("天使神殿Boss被击杀,用时{0}秒 ,Boss生命值比例成长为{1}", usedTime, this.AngelTempleMonsterUpgradePercent), null, true);
				GlodAuctionProcessCmdEx.getInstance().KillBossAddAuction(this.m_AngelTempleScene.m_nKillBossRole, this.m_BossHP, this.m_RoleAuctionData.Values.ToList<AuctionRoleData>(), AuctionEnum.AngelTemple);
				this.m_AngelTempleBoss = null;
			}
		}

		// Token: 0x06001A98 RID: 6808 RVA: 0x00197B2C File Offset: 0x00195D2C
		private void AddRoleAuctionData(GameClient client, int nDamage)
		{
			try
			{
				AuctionRoleData tempRole;
				if (!this.m_RoleAuctionData.TryGetValue(client.ClientData.RoleID, out tempRole))
				{
					tempRole = new AuctionRoleData();
					tempRole.m_RoleID = client.ClientData.RoleID;
					tempRole.m_RoleName = Global.FormatRoleName(client, client.ClientData.RoleName);
					tempRole.ZoneID = client.ClientData.ZoneID;
					tempRole.strUserID = client.strUserID;
					tempRole.ServerId = client.ServerId;
					tempRole.Value = (long)nDamage;
					this.m_RoleAuctionData.Add(tempRole.m_RoleID, tempRole);
				}
				else
				{
					tempRole.Value += (long)nDamage;
				}
			}
			catch
			{
			}
		}

		// Token: 0x06001A99 RID: 6809 RVA: 0x00197BF8 File Offset: 0x00195DF8
		public void CleanUpAngelTempleScene()
		{
			this.m_RoleAuctionData.Clear();
			this.m_AngelTempleScene.CleanAll();
			lock (this.m_PointDamageInfoMutex)
			{
				this.m_RoleDamageAngelValue.Clear();
				for (int i = 0; i < this.m_PointInfoArray.Length; i++)
				{
					if (null != this.m_PointInfoArray[i])
					{
						this.m_PointInfoArray[i] = new AngelTemplePointInfo();
					}
				}
			}
		}

		// Token: 0x06001A9A RID: 6810 RVA: 0x00197C98 File Offset: 0x00195E98
		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				if (!string.IsNullOrEmpty(this.m_sTotalDamageName) && this.m_sTotalDamageName == oldName)
				{
					this.m_sTotalDamageName = newName;
				}
			}
		}

		// Token: 0x0400292A RID: 10538
		private const int PaiHangArrayLength = 6;

		// Token: 0x0400292B RID: 10539
		public AngelTempleSceneInfo m_AngelTempleScene = new AngelTempleSceneInfo();

		// Token: 0x0400292C RID: 10540
		public AngelTempleData m_AngelTempleData = new AngelTempleData();

		// Token: 0x0400292D RID: 10541
		public Dictionary<int, AngelTemplePointInfo> m_RoleDamageAngelValue = new Dictionary<int, AngelTemplePointInfo>();

		// Token: 0x0400292E RID: 10542
		public Dictionary<int, AuctionRoleData> m_RoleAuctionData = new Dictionary<int, AuctionRoleData>();

		// Token: 0x0400292F RID: 10543
		public object m_PointDamageInfoMutex = new object();

		// Token: 0x04002930 RID: 10544
		private long LastMinDamage;

		// Token: 0x04002931 RID: 10545
		public AngelTemplePointInfo[] m_PointInfoArray = new AngelTemplePointInfo[6];

		// Token: 0x04002932 RID: 10546
		public Monster m_AngelTempleBoss = null;

		// Token: 0x04002933 RID: 10547
		public bool bBossKilled = false;

		// Token: 0x04002934 RID: 10548
		public long m_BossHP = 0L;

		// Token: 0x04002935 RID: 10549
		public long m_nTotalDamageValue = -1L;

		// Token: 0x04002936 RID: 10550
		public string m_sTotalDamageName = "";

		// Token: 0x04002937 RID: 10551
		public int m_sKillBossRoleID = 0;

		// Token: 0x04002938 RID: 10552
		public string m_sKillBossRoleName = "";

		// Token: 0x04002939 RID: 10553
		public long m_NotifyInfoTickForAll = 0L;

		// Token: 0x0400293A RID: 10554
		public long m_NotifyInfoTickForSingle = 0L;

		// Token: 0x0400293B RID: 10555
		public int m_LastNotifyBossHPPercent = -1;

		// Token: 0x0400293C RID: 10556
		public long m_NotifyInfoDelayTick = 3000L;

		// Token: 0x0400293D RID: 10557
		public long AngelTempleMinHurt = 0L;

		// Token: 0x0400293E RID: 10558
		private int AngelTempleBossUpgradeTime = 0;

		// Token: 0x0400293F RID: 10559
		private double AngelTempleBossUpgradeParam1 = 0.0;

		// Token: 0x04002940 RID: 10560
		private double AngelTempleBossUpgradeParam2 = 0.0;

		// Token: 0x04002941 RID: 10561
		private double AngelTempleBossUpgradeParam3 = 0.0;

		// Token: 0x04002942 RID: 10562
		private double AngelTempleMonsterUpgradePercent = 0.0;

		// Token: 0x04002943 RID: 10563
		private double BossBaseHP = 0.0;
	}
}
