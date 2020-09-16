using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	public class PropsCacheModule
	{
		
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

		
		private object mutex = new object();

		
		private Dictionary<int, PropsValueFactory> extPropValueDict = new Dictionary<int, PropsValueFactory>();

		
		private Dictionary<int, PropsValueFactory> basePropsValueDict = new Dictionary<int, PropsValueFactory>();
	}
}
