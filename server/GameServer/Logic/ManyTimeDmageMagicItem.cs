using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200051F RID: 1311
	public class ManyTimeDmageMagicItem
	{
		// Token: 0x060018DC RID: 6364 RVA: 0x001848FC File Offset: 0x00182AFC
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

		// Token: 0x060018DD RID: 6365 RVA: 0x00184994 File Offset: 0x00182B94
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

		// Token: 0x060018DE RID: 6366 RVA: 0x00184A00 File Offset: 0x00182C00
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

		// Token: 0x040022DA RID: 8922
		public static ManyTimeDmageItem SingleDamageItem = new ManyTimeDmageItem
		{
			InjuredPercent = 1.0
		};

		// Token: 0x040022DB RID: 8923
		public long execTicks = 0L;

		// Token: 0x040022DC RID: 8924
		public int enemy = -1;

		// Token: 0x040022DD RID: 8925
		public int enemyX = 0;

		// Token: 0x040022DE RID: 8926
		public int enemyY = 0;

		// Token: 0x040022DF RID: 8927
		public int realEnemyX = 0;

		// Token: 0x040022E0 RID: 8928
		public int realEnemyY = 0;

		// Token: 0x040022E1 RID: 8929
		public int magicCode = 0;

		// Token: 0x040022E2 RID: 8930
		public long startTicks;

		// Token: 0x040022E3 RID: 8931
		public int execIndex = -1;

		// Token: 0x040022E4 RID: 8932
		public List<ManyTimeDmageItem> itemList;

		// Token: 0x040022E5 RID: 8933
		public LinkedListNode<ManyTimeDmageMagicItem> linkedListNode;
	}
}
