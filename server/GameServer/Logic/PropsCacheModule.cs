using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x020001D3 RID: 467
	public class PropsCacheModule
	{
		// Token: 0x060005E9 RID: 1513 RVA: 0x00053EEC File Offset: 0x000520EC
		public double GetExtPropsValue(int propIndex, Func<double> factoryFunc)
		{
			long nowTicks = TimeUtil.CurrentTicksInexact;
			PropsValueFactory propsValueFactory;
			long age;
			lock (this.mutex)
			{
				if (!this.extPropValueDict.TryGetValue(propIndex, out propsValueFactory))
				{
					propsValueFactory = new PropsValueFactory
					{
						propIndex = propIndex,
						nextCalcTicks = nowTicks,
						factoryFunc = factoryFunc
					};
					this.extPropValueDict[propIndex] = propsValueFactory;
				}
				if (propsValueFactory.nextCalcTicks > nowTicks)
				{
					return propsValueFactory.propValue;
				}
				propsValueFactory.Age += 1L;
				age = propsValueFactory.Age;
			}
			double propValue = propsValueFactory.factoryFunc();
			double result;
			lock (this.mutex)
			{
				if (propsValueFactory.Age <= age)
				{
					propsValueFactory.nextCalcTicks = nowTicks + GameManager.FlagRecalcRolePropsTicks;
					propsValueFactory.propValue = propValue;
				}
				result = propValue;
			}
			return result;
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x00054028 File Offset: 0x00052228
		public double GetBasePropsValue(int propIndex, Func<double> factoryFunc, bool cache = true)
		{
			if (cache)
			{
				long nowTicks = TimeUtil.CurrentTicksInexact;
				PropsValueFactory propsValueFactory;
				long age;
				lock (this.mutex)
				{
					if (!this.basePropsValueDict.TryGetValue(propIndex, out propsValueFactory))
					{
						propsValueFactory = new PropsValueFactory
						{
							propIndex = propIndex,
							nextCalcTicks = nowTicks,
							factoryFunc = factoryFunc
						};
						this.basePropsValueDict[propIndex] = propsValueFactory;
					}
					if (propsValueFactory.nextCalcTicks > nowTicks)
					{
						return propsValueFactory.propValue;
					}
					propsValueFactory.Age += 1L;
					age = propsValueFactory.Age;
				}
				double propValue = propsValueFactory.factoryFunc();
				lock (this.mutex)
				{
					if (propsValueFactory.Age == age)
					{
						propsValueFactory.nextCalcTicks = nowTicks + GameManager.FlagRecalcRolePropsTicks;
						propsValueFactory.propValue = propValue;
					}
					return propValue;
				}
			}
			return factoryFunc();
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x0005417C File Offset: 0x0005237C
		public void ResetAllProps()
		{
			lock (this.mutex)
			{
				foreach (PropsValueFactory props in this.basePropsValueDict.Values)
				{
					props.nextCalcTicks = 0L;
				}
				foreach (PropsValueFactory props in this.extPropValueDict.Values)
				{
					props.nextCalcTicks = 0L;
				}
			}
		}

		// Token: 0x04000A5B RID: 2651
		private object mutex = new object();

		// Token: 0x04000A5C RID: 2652
		private Dictionary<int, PropsValueFactory> extPropValueDict = new Dictionary<int, PropsValueFactory>();

		// Token: 0x04000A5D RID: 2653
		private Dictionary<int, PropsValueFactory> basePropsValueDict = new Dictionary<int, PropsValueFactory>();
	}
}
