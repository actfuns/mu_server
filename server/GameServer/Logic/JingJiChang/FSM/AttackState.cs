using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;

namespace GameServer.Logic.JingJiChang.FSM
{
	
	internal class AttackState : IFSMState
	{
		
		public AttackState(GameClient player, Robot owner, FinishStateMachine FSM)
		{
			this.owner = owner;
			this.FSM = FSM;
			this.target = player;
			this.owner.LockObject = player.GetObjectID();
			EMagicSwordTowardType eMagicSwordType = GameManager.MagicSwordMgr.GetMagicSwordTypeByWeapon(owner.getRoleDataMini().Occupation, owner.getRoleDataMini().GoodsDataList, null);
			this.fiveComboSkillList = JingJiChangConstants.getJingJiChangeFiveCombatSkillList(owner.getRoleDataMini().Occupation, eMagicSwordType);
		}

		
		public void onBegin()
		{
			this.changeAction(GActions.Stand);
			this.benginCombatTime = TimeUtil.NOW() + 2000L;
		}

		
		public void onEnd()
		{
			this.simulateEndTime = 0L;
			this.castSimulateEndTime = 0L;
			this.skillSpellCDTime = 0L;
			this.benginCombatTime = 0L;
		}

		
		public void onUpdate(long ticks)
		{
			if (ticks >= this.benginCombatTime)
			{
				if (this.owner.VLife <= 0.0)
				{
					this.owner.MyMagicsManyTimeDmageQueue.Clear();
					this.FSM.switchState(AIState.DEAD);
				}
				else if (!this.owner.IsMonsterDongJie())
				{
					SpriteAttack.ExecMagicsManyTimeDmageQueueEx(this.owner);
					if (null == this.target)
					{
						this.FSM.switchState(AIState.RETURN);
					}
					else if (this.target.ClientData.CurrentLifeV <= 0)
					{
						this.FSM.switchState(AIState.RETURN);
					}
					else
					{
						if (this.castSimulateEndTime > 0L)
						{
							if (ticks > this.castSimulateEndTime)
							{
								this.castSimulateEndTime = 0L;
								int _direction = 0;
								if (this.testAttackDistance(out _direction))
								{
									this.owner.Direction = (double)_direction;
									SpriteAttack.ProcessAttackByJingJiRobot(this.owner, this.target, this.skillId, -1, 1.0);
								}
							}
						}
						if (this.simulateEndTime > 0L)
						{
							if (ticks >= this.simulateEndTime)
							{
								this.simulateEndTime = 0L;
								int nNextSkillID = Global.GetNextSkillID(this.skillId);
								if (nNextSkillID <= 0)
								{
									if (this.isCombatCD)
									{
										this.changeAction(GActions.Stand);
										this.skillSpellCDTime = ticks + 500L;
									}
								}
							}
						}
						else if (this.skillSpellCDTime <= 0L || ticks >= this.skillSpellCDTime)
						{
							if (this.skillSpellCDTime > 0L && ticks >= this.skillSpellCDTime)
							{
								this.skillSpellCDTime = 0L;
							}
							else
							{
								if (this.isUseFiveComboSkill)
								{
									this.selectFiveComboSkill();
								}
								else
								{
									bool isFiveCombo;
									this.selectSkill(out isFiveCombo);
									if (this.skillId == -1)
									{
										return;
									}
									if (isFiveCombo)
									{
										this.isCombatCD = false;
										this.isUseFiveComboSkill = true;
										return;
									}
									this.isCombatCD = true;
								}
								int direction = 0;
								if (!this.testAttackDistance(out direction))
								{
									this.moveTo(ticks);
								}
								else
								{
									if (this.owner.Action == GActions.Run)
									{
										this.owner.Direction = (double)((int)Global.GetDirectionByAspect((int)this.target.CurrentPos.X, (int)this.target.CurrentPos.Y, (int)this.owner.CurrentPos.X, (int)this.owner.CurrentPos.Y));
										this.changeAction(GActions.Stand);
									}
									this.attack(direction);
								}
							}
						}
					}
				}
			}
		}

		
		private bool testAttackDistance(out int direction)
		{
			direction = 0;
			SystemXmlItem systemMagic = null;
			bool result;
			if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(this.skillId, out systemMagic))
			{
				result = false;
			}
			else
			{
				List<MagicActionItem> magicScanTypeItemList = null;
				if (!GameManager.SystemMagicScanTypeMgr.MagicActionsDict.TryGetValue(this.skillId, out magicScanTypeItemList) || null == magicScanTypeItemList)
				{
				}
				List<MagicActionItem> magicActionItemList = null;
				if (!GameManager.SystemMagicActionMgr.MagicActionsDict.TryGetValue(this.skillId, out magicActionItemList) || null == magicActionItemList)
				{
					result = false;
				}
				else
				{
					MagicActionItem magicScanTypeItem = null;
					if (magicScanTypeItemList != null && magicScanTypeItemList.Count > 0)
					{
						magicScanTypeItem = magicScanTypeItemList[0];
					}
					int attackDistance = systemMagic.GetIntValue("AttackDistance", -1);
					if (systemMagic.GetIntValue("MagicType", -1) == 1 || systemMagic.GetIntValue("MagicType", -1) == 3)
					{
						int targetType = systemMagic.GetIntValue("TargetType", -1);
						if (1 == targetType)
						{
							return true;
						}
						if (this.skillId == 11004 && 4 == targetType)
						{
							return true;
						}
						if (2 == targetType)
						{
							return false;
						}
					}
					else
					{
						int attackDirection = 0;
						Point targetPos;
						if (1 == systemMagic.GetIntValue("TargetPos", -1))
						{
							targetPos = this.owner.CurrentPos;
						}
						else if (2 == systemMagic.GetIntValue("TargetPos", -1))
						{
							targetPos = this.target.CurrentPos;
							attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), this.owner.CurrentPos.X, this.owner.CurrentPos.Y);
						}
						else
						{
							targetPos = this.target.CurrentPos;
							attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), this.owner.CurrentPos.X, this.owner.CurrentPos.Y);
						}
						List<object> enemiesObjList = new List<object>();
						direction = attackDirection;
						if (magicScanTypeItem != null)
						{
							if (magicScanTypeItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
							{
								GameManager.ClientMgr.LookupRolesInSquare(this.owner.CurrentMapCode, this.owner.CopyMapID, (int)this.owner.CurrentPos.X, (int)this.owner.CurrentPos.Y, (int)targetPos.X, (int)targetPos.Y, (int)magicScanTypeItem.MagicActionParams[0], (int)magicScanTypeItem.MagicActionParams[1], enemiesObjList);
							}
							else if (magicScanTypeItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
							{
								GameManager.ClientMgr.LookupEnemiesInCircleByAngle(attackDirection, this.owner.CurrentMapCode, this.owner.CopyMapID, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), enemiesObjList, magicScanTypeItem.MagicActionParams[0], true);
							}
							else if (magicScanTypeItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
							{
								GameManager.ClientMgr.LookupEnemiesInCircle(this.owner.CurrentMapCode, this.owner.CopyMapID, (int)this.owner.CurrentPos.X, (int)this.owner.CurrentPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), enemiesObjList);
							}
						}
						else
						{
							GameManager.ClientMgr.LookupEnemiesInCircle(this.owner.CurrentMapCode, this.owner.CopyMapID, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), enemiesObjList);
						}
						if (enemiesObjList.Count <= 0)
						{
							return false;
						}
						for (int i = 0; i < enemiesObjList.Count; i++)
						{
							if ((enemiesObjList[i] as GameClient).ClientData.RoleID == this.target.GetObjectID())
							{
								return true;
							}
						}
					}
					result = false;
				}
			}
			return result;
		}

		
		private void selectFiveComboSkill()
		{
			if (!this.isSelectFiveComboSkill)
			{
				this.skillId = this.fiveComboSkillList[this.fiveComboSkillIndex];
				if (this.fiveComboSkillIndex >= this.fiveComboSkillList.Length - 1)
				{
					this.fiveComboSkillIndex = 0;
					this.isUseFiveComboSkill = false;
				}
				else
				{
					this.fiveComboSkillIndex++;
					this.isSelectFiveComboSkill = true;
				}
			}
		}

		
		private void selectSkill(out bool isFiveCombo)
		{
			this.skillId = -1;
			isFiveCombo = false;
			if (null != this.owner.MonsterInfo.SkillIDs)
			{
				int nNextSkillID = Global.GetNextSkillID(this.prevSkillID);
				if (nNextSkillID > 0)
				{
					this.skillId = nNextSkillID;
					this.prevSkillID = nNextSkillID;
				}
				else
				{
					int index = Global.GetRandomNumber(0, this.owner.MonsterInfo.SkillIDs.Length);
					for (int i = index; i < this.owner.MonsterInfo.SkillIDs.Length; i++)
					{
						if (this.owner.MyMagicCoolDownMgr.SkillCoolDown(this.owner.MonsterInfo.SkillIDs[i]))
						{
							if (this.SkillNeedMagicVOk(this.owner.MonsterInfo.SkillIDs[i]))
							{
								this.skillId = this.owner.MonsterInfo.SkillIDs[i];
								break;
							}
						}
					}
					if (this.skillId == -1)
					{
						for (int i = index - 1; i >= 0; i--)
						{
							if (this.owner.MyMagicCoolDownMgr.SkillCoolDown(this.owner.MonsterInfo.SkillIDs[i]))
							{
								if (this.SkillNeedMagicVOk(this.owner.MonsterInfo.SkillIDs[i]))
								{
									this.skillId = this.owner.MonsterInfo.SkillIDs[i];
								}
							}
						}
					}
					if (!this.isTryHighPrioritySkill)
					{
						EMagicSwordTowardType eMagicSwordType = GameManager.MagicSwordMgr.GetMagicSwordTypeByWeapon(this.owner.getRoleDataMini().Occupation, this.owner.getRoleDataMini().GoodsDataList, null);
						int nFirstSkill = JingJiChangConstants.GetJingJiChangeHighPrioritySkill(this.owner.getRoleDataMini().Occupation, eMagicSwordType);
						if (nFirstSkill != -1)
						{
							if (this.owner.MyMagicCoolDownMgr.SkillCoolDown(nFirstSkill) && this.SkillNeedMagicVOk(nFirstSkill))
							{
								this.skillId = nFirstSkill;
							}
						}
						this.isTryHighPrioritySkill = true;
					}
					this.prevSkillID = this.skillId;
					for (int i = 0; i < this.fiveComboSkillList.Length; i++)
					{
						if (this.fiveComboSkillList[i] == this.skillId)
						{
							isFiveCombo = true;
							break;
						}
					}
				}
			}
		}

		
		private bool SkillNeedMagicVOk(int skillID)
		{
			int usedMagicV = Global.GetNeedMagicV(this.owner, skillID, 1);
			if (usedMagicV > 0)
			{
				int nMax = (int)this.owner.MonsterInfo.VManaMax;
				int nNeed = nMax * (usedMagicV / 100);
				nNeed = Global.GMax(0, nNeed);
				if (this.owner.VMana - (double)nNeed < 0.0)
				{
					return false;
				}
			}
			return true;
		}

		
		private void moveTo(long ticks)
		{
			if (ticks >= this.moveEndTime)
			{
				Point ownerGrid = this.owner.CurrentGrid;
				int nCurrX = (int)ownerGrid.X;
				int nCurrY = (int)ownerGrid.Y;
				Point targetGrid = this.target.CurrentGrid;
				int nTargetCurrX = (int)targetGrid.X;
				int nTargetCurrY = (int)targetGrid.Y;
				int nDir = (int)this.owner.Direction;
				if (nCurrX != nTargetCurrX || nCurrY != nTargetCurrY)
				{
					int nX = nTargetCurrX;
					int nY = nTargetCurrY;
					if (nX > nCurrX)
					{
						nDir = 2;
						if (nY > nCurrY)
						{
							nDir = 1;
						}
						else if (nY < nCurrY)
						{
							nDir = 3;
						}
					}
					else if (nX < nCurrX)
					{
						nDir = 6;
						if (nY > nCurrY)
						{
							nDir = 7;
						}
						else if (nY < nCurrY)
						{
							nDir = 5;
						}
					}
					else if (nY > nCurrY)
					{
						nDir = 0;
					}
					else if (nY < nCurrY)
					{
						nDir = 4;
					}
					this.owner.Direction = (double)nDir;
					int nOldX = nCurrX;
					int nOldY = nCurrY;
					ChuanQiUtils.RunTo1(this.owner, (Dircetions)nDir);
					ownerGrid = this.owner.CurrentGrid;
					nCurrX = (int)ownerGrid.X;
					nCurrY = (int)ownerGrid.Y;
					for (int i = 0; i < 7; i++)
					{
						if (nOldX != nCurrX || nOldY != nCurrY)
						{
							break;
						}
						if (Global.GetRandomNumber(0, 3) > 0)
						{
							nDir++;
						}
						else if (nDir > 0)
						{
							nDir--;
						}
						else
						{
							nDir = 7;
						}
						if (nDir > 7)
						{
							nDir = 0;
						}
						ChuanQiUtils.RunTo1(this.owner, (Dircetions)nDir);
						ownerGrid = this.owner.CurrentGrid;
						nCurrX = (int)ownerGrid.X;
						nCurrY = (int)ownerGrid.Y;
					}
				}
				this.moveEndTime = ticks + 600L;
			}
		}

		
		private void attack(int direction)
		{
			if (!this.owner.IsMoving)
			{
				if (null != this.target)
				{
					double newDirection = (double)((int)Global.GetDirectionByAspect((int)this.target.CurrentPos.X, (int)this.target.CurrentPos.Y, (int)this.owner.CurrentPos.X, (int)this.owner.CurrentPos.Y));
					if (newDirection != this.owner.SafeDirection)
					{
						this.owner.Direction = (double)((int)newDirection);
					}
					if (this.owner.EnemyTarget != this.target.CurrentPos)
					{
						this.owner.EnemyTarget = this.target.CurrentPos;
					}
					this.owner.CurrentMagic = this.skillId;
					if (this.skillId > 0)
					{
						if (GameManager.SystemMagicsMgr.SystemXmlItemDict[this.skillId].GetStringValue("SkillAction") == "" || GameManager.SystemMagicsMgr.SystemXmlItemDict[this.skillId].GetStringValue("SkillAction").Equals(""))
						{
							this.changeAction(GActions.Attack);
						}
						else
						{
							GameManager.ClientMgr.NotifyOthersMagicCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.owner, this.owner.RoleID, this.owner.MonsterZoneNode.MapCode, this.skillId, 116);
							this.changeAction(GActions.Magic);
						}
					}
					else
					{
						this.changeAction(GActions.Attack);
					}
					this.isSelectFiveComboSkill = false;
					this.simulate();
				}
			}
		}

		
		private void changeAction(GActions action)
		{
			if (this.owner.VLife > 0.0)
			{
				double newDirection = (double)((int)Global.GetDirectionByAspect((int)this.target.CurrentPos.X, (int)this.target.CurrentPos.Y, (int)this.owner.CurrentPos.X, (int)this.owner.CurrentPos.Y));
				List<object> listObjs = Global.GetAll9Clients(this.owner);
				GameManager.ClientMgr.NotifyOthersDoAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.owner, this.owner.MonsterZoneNode.MapCode, this.owner.CopyMapID, this.owner.RoleID, (int)newDirection, (int)action, (int)this.owner.SafeCoordinate.X, (int)this.owner.SafeCoordinate.Y, (int)this.target.CurrentPos.X, (int)this.target.CurrentPos.Y, 114, listObjs);
				Global.RemoveStoryboard(this.owner.Name);
				this.monsterMoving.ChangeDirection(this.owner, newDirection);
				this.owner.Action = action;
			}
		}

		
		private void simulate()
		{
			int frameCount;
			int castFrameCount;
			if (this.skillId == -1)
			{
				frameCount = 3;
				castFrameCount = 3;
			}
			else
			{
				frameCount = 5;
				castFrameCount = 5;
				for (int i = 0; i < JingJiChangConstants.SkillFrameCounts.Length; i++)
				{
					if (this.skillId == JingJiChangConstants.SkillFrameCounts[i][0])
					{
						frameCount = JingJiChangConstants.SkillFrameCounts[i][1];
						castFrameCount = JingJiChangConstants.SkillFrameCounts[i][2];
						break;
					}
				}
			}
			this.simulateEndTime = TimeUtil.NOW() + (long)(frameCount * 100);
			this.castSimulateEndTime = TimeUtil.NOW() + (long)(castFrameCount * 100);
		}

		
		public static readonly AIState state = AIState.ATTACK;

		
		private Robot owner = null;

		
		private FinishStateMachine FSM = null;

		
		private long moveEndTime = 0L;

		
		private MonsterMoving monsterMoving = new MonsterMoving();

		
		private GameClient target = null;

		
		private int skillId = -1;

		
		private int prevSkillID = -1;

		
		private long simulateEndTime = 0L;

		
		private long castSimulateEndTime = 0L;

		
		private long skillSpellCDTime = 0L;

		
		private long benginCombatTime = 0L;

		
		private int[] fiveComboSkillList;

		
		private bool isTryHighPrioritySkill = false;

		
		private bool isCombatCD = true;

		
		private bool isUseFiveComboSkill = false;

		
		private int fiveComboSkillIndex = 0;

		
		private bool isSelectFiveComboSkill = false;
	}
}
