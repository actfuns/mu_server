using System;

namespace GameServer.Logic.Algorithm
{
	// Token: 0x020001D8 RID: 472
	public class DataCheckModule
	{
		// Token: 0x020001D9 RID: 473
		public class CheckLinerValue
		{
			// Token: 0x060005FD RID: 1533 RVA: 0x000549F6 File Offset: 0x00052BF6
			public CheckLinerValue(int _maxSize)
			{
				this.maxSize = _maxSize;
				this.maxPos = this.maxSize - 1;
				this.valueArray = new int[this.maxSize];
			}

			// Token: 0x060005FE RID: 1534 RVA: 0x00054A32 File Offset: 0x00052C32
			public void Clear()
			{
				Array.Clear(this.valueArray, 0, this.valueArray.Length);
			}

			// Token: 0x060005FF RID: 1535 RVA: 0x00054A4C File Offset: 0x00052C4C
			public bool Push(int v, int num, int limit)
			{
				bool result;
				if (num > this.maxSize)
				{
					result = false;
				}
				else
				{
					lock (this.mutex)
					{
						if (num <= this.dataCount)
						{
							int sum = 0;
							int pos = this.dataPos;
							for (int i = 1; i < num; i++)
							{
								pos = ((pos == 0) ? this.maxPos : (pos - 1));
								sum += this.valueArray[pos];
							}
							sum += v;
							if (sum > limit)
							{
								return false;
							}
						}
						this.valueArray[this.dataPos] = v;
						this.dataPos = ((this.dataPos >= this.maxPos) ? 0 : (this.dataPos + 1));
						if (this.dataCount < this.maxSize)
						{
							this.dataCount++;
						}
					}
					result = true;
				}
				return result;
			}

			// Token: 0x04000A70 RID: 2672
			public int[] valueArray;

			// Token: 0x04000A71 RID: 2673
			private int maxSize;

			// Token: 0x04000A72 RID: 2674
			private int maxPos;

			// Token: 0x04000A73 RID: 2675
			private int dataPos;

			// Token: 0x04000A74 RID: 2676
			private int dataCount;

			// Token: 0x04000A75 RID: 2677
			private object mutex = new object();
		}
	}
}
