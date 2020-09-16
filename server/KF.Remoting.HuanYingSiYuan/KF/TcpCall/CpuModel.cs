using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KF.TcpCall
{
	
	public class CpuModel
	{
		
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

		
		private PerformanceCounter Cpu;

		
		private List<float> cpulist = new List<float>();

		
		private float MaxNum;

		
		private float MinNum;
	}
}
