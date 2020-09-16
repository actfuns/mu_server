using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Interface;

namespace GameServer.Logic
{
	
	public class SpriteMagicHelper
	{
		
		public void AddMagicHelper(MagicActionIDs magicActionID, double[] magicActionParams, int objID)
		{
			MagicHelperItem magicHelperItem = new MagicHelperItem
			{
				MagicActionID = magicActionID,
				MagicActionParams = magicActionParams,
				StartedTicks = TimeUtil.NOW() * 10000L,
				LastTicks = 0L,
				ExecutedNum = 0,
				ObjectID = objID
			};
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict[magicActionID] = magicHelperItem;
			}
		}

		
		public void RemoveMagicHelper(MagicActionIDs magicActionID)
		{
			lock (this._MagicHelperDict)
			{
				if (this._MagicHelperDict.ContainsKey(magicActionID))
				{
					this._MagicHelperDict.Remove(magicActionID);
				}
			}
		}

		
		private bool CanExecuteItem(MagicHelperItem magicHelperItem, int effectSecs, int maxNum)
		{
			long nowTicks = TimeUtil.NOW();
			long ticks = magicHelperItem.StartedTicks + (long)effectSecs * 1000L;
			bool result;
			if (maxNum <= 0)
			{
				if (nowTicks >= ticks)
				{
					lock (this._MagicHelperDict)
					{
						this._MagicHelperDict.Remove(magicHelperItem.MagicActionID);
					}
					result = false;
				}
				else
				{
					result = true;
				}
			}
			else if (magicHelperItem.ExecutedNum >= maxNum)
			{
				lock (this._MagicHelperDict)
				{
					this._MagicHelperDict.Remove(magicHelperItem.MagicActionID);
				}
				result = false;
			}
			else
			{
				long ticksSlot = (long)(effectSecs / maxNum * 1000 * 10000);
				result = (nowTicks - magicHelperItem.LastTicks >= ticksSlot);
			}
			return result;
		}

		
		public double GetAddDrugEffect()
		{
			double percent = 1.0;
			lock (this._MagicHelperDict)
			{
				MagicHelperItem magicHelperItem = null;
				if (this._MagicHelperDict.TryGetValue(MagicActionIDs.FOREVER_ADDDRUGEFFECT, out magicHelperItem))
				{
					percent = magicHelperItem.MagicActionParams[0] / 100.0;
				}
			}
			return percent;
		}

		
		public double GetMoveSlow()
		{
			double percent = 0.0;
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_SLOW, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = percent;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], 0))
			{
				result = percent;
			}
			else
			{
				percent = magicHelperItem.MagicActionParams[0] / 100.0;
				result = percent;
			}
			return result;
		}

		
		public bool GetFreeze()
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_FREEZE, out magicHelperItem);
			}
			return null != magicHelperItem && this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[0], 0);
		}

		
		public double GetInjure2Life()
		{
			double percent = 0.0;
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_INJUE2LIFE, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = percent;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], 0))
			{
				result = percent;
			}
			else
			{
				percent = magicHelperItem.MagicActionParams[0] / 100.0;
				result = percent;
			}
			return result;
		}

		
		public double GetSubInjure()
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_SUBINJUE, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = 0.0;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], 0))
			{
				result = 0.0;
			}
			else
			{
				result = magicHelperItem.MagicActionParams[0];
			}
			return result;
		}

		
		public double GetAddInjure()
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_ADDINJUE, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = 0.0;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], 0))
			{
				result = 0.0;
			}
			else
			{
				result = magicHelperItem.MagicActionParams[0];
			}
			return result;
		}

		
		public double GetSubInjure1()
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_SUBINJUE1, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = 0.0;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], 0))
			{
				result = 0.0;
			}
			else
			{
				result = magicHelperItem.MagicActionParams[0];
			}
			return result;
		}

		
		public double GetAddInjure1()
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_ADDINJUE1, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = 0.0;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], 0))
			{
				result = 0.0;
			}
			else
			{
				result = magicHelperItem.MagicActionParams[0];
			}
			return result;
		}

		
		public double GetInjure2Magic()
		{
			double percent = 0.0;
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_INJUE2MAGIC, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = percent;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], 0))
			{
				result = percent;
			}
			else
			{
				percent = magicHelperItem.MagicActionParams[0] / 100.0;
				result = percent;
			}
			return result;
		}

		
		public double GetNewInjure2Magic()
		{
			double injure2Magic = 0.0;
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.NEW_TIME_INJUE2MAGIC, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = injure2Magic;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], 0))
			{
				result = injure2Magic;
			}
			else
			{
				injure2Magic = magicHelperItem.MagicActionParams[0];
				result = injure2Magic;
			}
			return result;
		}

		
		public double GetNewInjure2Magic3()
		{
			double injure2Magic = 0.0;
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.NEW_TIME_INJUE2MAGIC3, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = injure2Magic;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], 0))
			{
				result = injure2Magic;
			}
			else
			{
				injure2Magic = magicHelperItem.MagicActionParams[0];
				result = injure2Magic;
			}
			return result;
		}

		
		public double GetNewMagicSubInjure()
		{
			double injure2Magic = 0.0;
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.NEW_MAGIC_SUBINJURE, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = injure2Magic;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], 0))
			{
				result = injure2Magic;
			}
			else
			{
				injure2Magic = magicHelperItem.MagicActionParams[0];
				result = injure2Magic;
			}
			return result;
		}

		
		public void ExecuteAttack(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_ATTACK, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], (int)magicHelperItem.MagicActionParams[2]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW() * 10000L;
					int enemy = magicHelperItem.ObjectID;
					if (-1 != enemy)
					{
						GSpriteTypes st = Global.GetSpriteType((uint)enemy);
						if (st == GSpriteTypes.Monster)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster((self as GameClient).ClientData.MapCode, enemy);
							if (null != enemyMonster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyMonster, 0, 0, magicHelperItem.MagicActionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
						}
						else
						{
							GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
							if (null != enemyClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyClient, 0, 0, magicHelperItem.MagicActionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		
		public void ExecuteNewAttack(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.NEW_TIME_ATTACK, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], (int)magicHelperItem.MagicActionParams[2]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW() * 10000L;
					int enemy = magicHelperItem.ObjectID;
					if (-1 != enemy)
					{
						GSpriteTypes st = Global.GetSpriteType((uint)enemy);
						if (st == GSpriteTypes.Monster)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster((self as GameClient).ClientData.MapCode, enemy);
							if (null != enemyMonster)
							{
								int addInjure = (int)(magicHelperItem.MagicActionParams[0] / magicHelperItem.MagicActionParams[2]);
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyMonster, 0, 0, 1.0 / magicHelperItem.MagicActionParams[2], 0, false, addInjure, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
						}
						else
						{
							GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
							if (null != enemyClient)
							{
								int addInjure = (int)(magicHelperItem.MagicActionParams[0] / magicHelperItem.MagicActionParams[2]);
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyClient, 0, 0, 1.0 / magicHelperItem.MagicActionParams[2], 0, false, addInjure, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		
		public void ExecuteNewAttack3(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.NEW_TIME_ATTACK3, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], (int)magicHelperItem.MagicActionParams[2]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW() * 10000L;
					int enemy = magicHelperItem.ObjectID;
					if (-1 != enemy)
					{
						GSpriteTypes st = Global.GetSpriteType((uint)enemy);
						if (st == GSpriteTypes.Monster)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster((self as GameClient).ClientData.MapCode, enemy);
							if (null != enemyMonster)
							{
								int injureValue = (int)(magicHelperItem.MagicActionParams[0] / magicHelperItem.MagicActionParams[2]);
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyMonster, 0, injureValue, 0.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
						}
						else
						{
							GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
							if (null != enemyClient)
							{
								int injureValue = (int)(magicHelperItem.MagicActionParams[0] / magicHelperItem.MagicActionParams[2]);
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyClient, 0, injureValue, 0.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		
		public void ExecuteMAttack(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_MAGIC, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], (int)magicHelperItem.MagicActionParams[2]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW() * 10000L;
					int enemy = magicHelperItem.ObjectID;
					if (-1 != enemy)
					{
						GSpriteTypes st = Global.GetSpriteType((uint)enemy);
						if (st == GSpriteTypes.Monster)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster((self as GameClient).ClientData.MapCode, enemy);
							if (null != enemyMonster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyMonster, 0, 0, magicHelperItem.MagicActionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
						}
						else
						{
							GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
							if (null != enemyClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyClient, 0, 0, magicHelperItem.MagicActionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		
		public void ExecuteNewMAttack(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.NEW_TIME_MAGIC, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], (int)magicHelperItem.MagicActionParams[2]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW() * 10000L;
					int enemy = magicHelperItem.ObjectID;
					if (-1 != enemy)
					{
						GSpriteTypes st = Global.GetSpriteType((uint)enemy);
						if (st == GSpriteTypes.Monster)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster((self as GameClient).ClientData.MapCode, enemy);
							if (null != enemyMonster)
							{
								int addInjure = (int)(magicHelperItem.MagicActionParams[0] / magicHelperItem.MagicActionParams[2]);
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyMonster, 0, 0, 1.0 / magicHelperItem.MagicActionParams[2], 1, false, addInjure, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
						}
						else
						{
							GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
							if (null != enemyClient)
							{
								int addInjure = (int)(magicHelperItem.MagicActionParams[0] / magicHelperItem.MagicActionParams[2]);
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyClient, 0, 0, 1.0 / magicHelperItem.MagicActionParams[2], 1, false, addInjure, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		
		public void ExecuteNewMAttack3(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.NEW_TIME_MAGIC3, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], (int)magicHelperItem.MagicActionParams[2]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW() * 10000L;
					int enemy = magicHelperItem.ObjectID;
					if (-1 != enemy)
					{
						GSpriteTypes st = Global.GetSpriteType((uint)enemy);
						if (st == GSpriteTypes.Monster)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster((self as GameClient).ClientData.MapCode, enemy);
							if (null != enemyMonster)
							{
								int injureValue = (int)(magicHelperItem.MagicActionParams[0] / magicHelperItem.MagicActionParams[2]);
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyMonster, 0, injureValue, 0.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
						}
						else
						{
							GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
							if (null != enemyClient)
							{
								int injureValue = (int)(magicHelperItem.MagicActionParams[0] / magicHelperItem.MagicActionParams[2]);
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyClient, 0, injureValue, 0.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		
		public void ExecuteNewMAttack4(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.NEW_TIME_MAGIC4, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], (int)magicHelperItem.MagicActionParams[2]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW() * 10000L;
					int enemy = magicHelperItem.ObjectID;
					if (-1 != enemy)
					{
						GSpriteTypes st = Global.GetSpriteType((uint)enemy);
						if (st == GSpriteTypes.Monster)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster((self as GameClient).ClientData.MapCode, enemy);
							if (null != enemyMonster)
							{
								int injureValue = (int)(magicHelperItem.MagicActionParams[0] / magicHelperItem.MagicActionParams[2]);
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyMonster, 0, injureValue, 0.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
						}
						else
						{
							GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
							if (null != enemyClient)
							{
								int injureValue = (int)(magicHelperItem.MagicActionParams[0] / magicHelperItem.MagicActionParams[2]);
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyClient, 0, injureValue, 0.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		
		public void ExecuteSubLife(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_SUBLIFE, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], (int)magicHelperItem.MagicActionParams[2]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW() * 10000L;
					int enemy = magicHelperItem.ObjectID;
					if (-1 != enemy)
					{
						GSpriteTypes st = Global.GetSpriteType((uint)enemy);
						if (st == GSpriteTypes.Monster)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster((self as GameClient).ClientData.MapCode, enemy);
							if (null != enemyMonster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyMonster, 0, (int)magicHelperItem.MagicActionParams[0], 0.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
						}
						else
						{
							GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
							if (null != enemyClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyClient, 0, (int)magicHelperItem.MagicActionParams[0], 0.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		
		public void ExecuteSubLife2(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_DS_INJURE, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[0], (int)magicHelperItem.MagicActionParams[1]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW() * 10000L;
					int enemy = magicHelperItem.ObjectID;
					if (-1 != enemy)
					{
						int nOcc = Global.CalcOriginalOccupationID(self as GameClient);
						int attackType = nOcc;
						GSpriteTypes st = Global.GetSpriteType((uint)enemy);
						if (st == GSpriteTypes.Monster)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster((self as GameClient).ClientData.MapCode, enemy);
							if (null != enemyMonster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyMonster, 0, (int)magicHelperItem.MagicActionParams[2], 1.0, attackType, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
								if (enemyMonster.VLife <= 0.0)
								{
									magicHelperItem.ExecutedNum = (int)magicHelperItem.MagicActionParams[1];
								}
							}
						}
						else
						{
							GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
							if (null != enemyClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyClient, 0, (int)magicHelperItem.MagicActionParams[2], 1.0, attackType, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
								if (enemyClient.ClientData.CurrentLifeV <= 0)
								{
									magicHelperItem.ExecutedNum = (int)magicHelperItem.MagicActionParams[1];
								}
							}
						}
					}
				}
			}
		}

		
		public void ExecuteAddLife(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_ADDLIFE, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], (int)magicHelperItem.MagicActionParams[2]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW() * 10000L;
					if (self is GameClient)
					{
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, magicHelperItem.MagicActionParams[0], "道具脚本" + MagicActionIDs.TIME_ADDLIFE.ToString());
					}
				}
			}
		}

		
		public void ExecuteAddMagic1(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_ADDMAGIC1, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[1], (int)magicHelperItem.MagicActionParams[2]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW() * 10000L;
					if (self is GameClient)
					{
						GameManager.ClientMgr.AddSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, magicHelperItem.MagicActionParams[0], "道具脚本" + MagicActionIDs.TIME_ADDMAGIC1.ToString());
					}
				}
			}
		}

		
		public void ExecuteDelayAttack(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_DELAYATTACK, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				long nowTicks = TimeUtil.NOW() * 10000L;
				long ticks = magicHelperItem.StartedTicks + (long)((int)magicHelperItem.MagicActionParams[1] * 1000 * 10000);
				if (nowTicks >= ticks)
				{
					lock (this._MagicHelperDict)
					{
						this._MagicHelperDict.Remove(magicHelperItem.MagicActionID);
					}
					int enemy = magicHelperItem.ObjectID;
					if (-1 != enemy)
					{
						GSpriteTypes st = Global.GetSpriteType((uint)enemy);
						if (st == GSpriteTypes.Monster)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster((self as GameClient).ClientData.MapCode, enemy);
							if (null != enemyMonster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyMonster, 0, 0, magicHelperItem.MagicActionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
						}
						else
						{
							GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
							if (null != enemyClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyClient, 0, 0, magicHelperItem.MagicActionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		
		public void ExecuteDelayMAttack(IObject self)
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.TIME_DELAYMAGIC, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				long nowTicks = TimeUtil.NOW() * 10000L;
				long ticks = magicHelperItem.StartedTicks + (long)((int)magicHelperItem.MagicActionParams[1] * 1000 * 10000);
				if (nowTicks >= ticks)
				{
					lock (this._MagicHelperDict)
					{
						this._MagicHelperDict.Remove(magicHelperItem.MagicActionID);
					}
					int enemy = magicHelperItem.ObjectID;
					if (-1 != enemy)
					{
						GSpriteTypes st = Global.GetSpriteType((uint)enemy);
						if (st == GSpriteTypes.Monster)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster((self as GameClient).ClientData.MapCode, enemy);
							if (null != enemyMonster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyMonster, 0, 0, magicHelperItem.MagicActionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
						}
						else
						{
							GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
							if (null != enemyClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, enemyClient, 0, 0, magicHelperItem.MagicActionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		
		public static void ExecuteAllItems(IObject self)
		{
			(self as GameClient).RoleMagicHelper.ExecuteAttack(self);
			(self as GameClient).RoleMagicHelper.ExecuteNewAttack(self);
			(self as GameClient).RoleMagicHelper.ExecuteNewAttack3(self);
			(self as GameClient).RoleMagicHelper.ExecuteMAttack(self);
			(self as GameClient).RoleMagicHelper.ExecuteNewMAttack(self);
			(self as GameClient).RoleMagicHelper.ExecuteNewMAttack3(self);
			(self as GameClient).RoleMagicHelper.ExecuteNewMAttack4(self);
			(self as GameClient).RoleMagicHelper.ExecuteSubLife(self);
			(self as GameClient).RoleMagicHelper.ExecuteSubLife2(self);
			(self as GameClient).RoleMagicHelper.ExecuteAddLife(self);
			(self as GameClient).RoleMagicHelper.ExecuteAddMagic1(self);
			(self as GameClient).RoleMagicHelper.ExecuteDelayAttack(self);
			(self as GameClient).RoleMagicHelper.ExecuteDelayMAttack(self);
		}

		
		public double MU_GetSubInjure1()
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = 0.0;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[0], 0))
			{
				result = 0.0;
			}
			else
			{
				result = magicHelperItem.MagicActionParams[1];
			}
			return result;
		}

		
		public double MU_GetSubInjure2()
		{
			MagicHelperItem magicHelperItem = null;
			lock (this._MagicHelperDict)
			{
				this._MagicHelperDict.TryGetValue(MagicActionIDs.MU_SUB_DAMAGE_VALUE, out magicHelperItem);
			}
			double result;
			if (null == magicHelperItem)
			{
				result = 0.0;
			}
			else if (!this.CanExecuteItem(magicHelperItem, (int)magicHelperItem.MagicActionParams[0], 0))
			{
				result = 0.0;
			}
			else
			{
				result = magicHelperItem.MagicActionParams[1];
			}
			return result;
		}

		
		private Dictionary<MagicActionIDs, MagicHelperItem> _MagicHelperDict = new Dictionary<MagicActionIDs, MagicHelperItem>();
	}
}
