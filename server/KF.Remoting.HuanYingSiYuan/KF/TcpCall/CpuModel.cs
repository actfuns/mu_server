using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KF.TcpCall
{
	// Token: 0x02000084 RID: 132
	public class CpuModel
	{
		// Token: 0x060006E1 RID: 1761 RVA: 0x0005B60C File Offset: 0x0005980C
		public void Start()
		{
			this.cpulist = new List<float>();
			this.Cpu = new PerformanceCounter();
			this.Cpu.CategoryName = "Processor";
			this.Cpu.CounterName = "% Processor Time";
			this.Cpu.InstanceName = "_Total";
			this.MinNum = 1E+08f;
			this.MaxNum = 0f;
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x0005B67C File Offset: 0x0005987C
		public void GetValue()
		{
			float value = this.Cpu.NextValue();
			if (value > 0f && value < 100f)
			{
				if (value > this.MaxNum)
				{
					this.MaxNum = value;
				}
				if (value < this.MinNum)
				{
					this.MinNum = value;
				}
				this.cpulist.Add(value);
			}
		}

		// Token: 0x060006E3 RID: 1763 RVA: 0x0005B6F0 File Offset: 0x000598F0
		public void Print()
		{
			if (this.cpulist.Count < 1)
			{
				Console.WriteLine(string.Format("cpu max={0},min={1}, avg={2}", 0, 0, 0 / this.cpulist.Count));
			}
			else
			{
				float allCpu = 0f;
				foreach (float num in this.cpulist)
				{
					float item = num;
					allCpu += item;
				}
				Console.WriteLine(string.Format("cpu max={0},min={1}, avg={2}(心跳取值，极值不准，平均还可以)", this.MaxNum, this.MinNum, allCpu / (float)this.cpulist.Count));
			}
		}

		// Token: 0x040003B7 RID: 951
		private PerformanceCounter Cpu;

		// Token: 0x040003B8 RID: 952
		private List<float> cpulist = new List<float>();

		// Token: 0x040003B9 RID: 953
		private float MaxNum;

		// Token: 0x040003BA RID: 954
		private float MinNum;
	}
}
