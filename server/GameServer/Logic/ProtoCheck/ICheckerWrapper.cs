using System;

namespace GameServer.Logic.ProtoCheck
{
	
	internal class ICheckerWrapper<T> : ICheckerBase where T : class
	{
		
		public ICheckerWrapper(ICheckerWrapper<T>.CheckerCallback cb)
		{
			this._cb = cb;
		}

		
		public bool Check(object obj1, object obj2)
		{
			T data = obj1 as T;
			T data2 = obj2 as T;
			return this._cb(data, data2);
		}

		
		private ICheckerWrapper<T>.CheckerCallback _cb = null;

		
		
		public delegate bool CheckerCallback(T data1, T data2);
	}
}
