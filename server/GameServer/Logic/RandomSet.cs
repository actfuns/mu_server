using System;

namespace GameServer.Logic
{
	// Token: 0x020004F2 RID: 1266
	public class RandomSet
	{
		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06001782 RID: 6018 RVA: 0x00170064 File Offset: 0x0016E264
		// (set) Token: 0x06001783 RID: 6019 RVA: 0x0017007B File Offset: 0x0016E27B
		public int[] ResultList { get; private set; }

		// Token: 0x06001784 RID: 6020 RVA: 0x00170084 File Offset: 0x0016E284
		public RandomSet(int count)
		{
			this.AllCount = count;
			this.ResultList = new int[count];
			for (int i = 0; i < count; i++)
			{
				this.ResultList[i] = i;
			}
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x001700D8 File Offset: 0x0016E2D8
		public int RandomNext()
		{
			int rand = Global.GetRandomNumber(this.RandomCount, this.AllCount);
			int t = this.ResultList[this.RandomCount];
			this.ResultList[this.RandomCount] = this.ResultList[rand];
			this.ResultList[rand] = t;
			return t;
		}

		// Token: 0x04002191 RID: 8593
		private int RandomCount = 0;

		// Token: 0x04002192 RID: 8594
		private int AllCount = 0;
	}
}
