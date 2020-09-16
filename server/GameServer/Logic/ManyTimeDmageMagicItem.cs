using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class ManyTimeDmageMagicItem
	{
		
		public bool Start(long nowTicks, int magicCode, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY)
		{
			bool result;
			if (this.execIndex == -1)
			{
				this.execIndex = 0;
				this.startTicks = nowTicks;
				this.magicCode = magicCode;
				this.enemy = enemy;
				this.enemyX = enemyX;
				this.enemyY = enemyY;
				this.realEnemyX = realEnemyX;
				this.realEnemyY = realEnemyY;
				if (null != this.itemList)
				{
					this.execTicks = this.startTicks + this.itemList[this.execIndex].InjuredSeconds;
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public ManyTimeDmageItem Get()
		{
			ManyTimeDmageItem result;
			lock (this)
			{
				if (null == this.itemList)
				{
					result = ManyTimeDmageMagicItem.SingleDamageItem;
				}
				else
				{
					result = this.itemList[this.execIndex];
				}
			}
			return result;
		}

		
		public bool Next()
		{
			lock (this)
			{
				if (null == this.itemList)
				{
					this.execIndex = -1;
				}
				else
				{
					if (this.execIndex < this.itemList.Count - 1)
					{
						this.execIndex++;
						this.execTicks = this.startTicks + this.itemList[this.execIndex].InjuredSeconds;
						return true;
					}
					this.execTicks = 0L;
					this.execIndex = -1;
				}
			}
			return false;
		}

		
		public static ManyTimeDmageItem SingleDamageItem = new ManyTimeDmageItem
		{
			InjuredPercent = 1.0
		};

		
		public long execTicks = 0L;

		
		public int enemy = -1;

		
		public int enemyX = 0;

		
		public int enemyY = 0;

		
		public int realEnemyX = 0;

		
		public int realEnemyY = 0;

		
		public int magicCode = 0;

		
		public long startTicks;

		
		public int execIndex = -1;

		
		public List<ManyTimeDmageItem> itemList;

		
		public LinkedListNode<ManyTimeDmageMagicItem> linkedListNode;
	}
}
