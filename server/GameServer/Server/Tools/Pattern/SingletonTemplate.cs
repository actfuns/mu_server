using System;

namespace Server.Tools.Pattern
{
	
	public class SingletonTemplate<T> where T : class
	{
		
		public static T Instance()
		{
			if (SingletonTemplate<T>.__singletionInstance == null)
			{
				lock (SingletonTemplate<T>.__singletionInstanceLock)
				{
					if (SingletonTemplate<T>.__singletionInstance == null)
					{
						SingletonTemplate<T>.__singletionInstance = (T)((object)Activator.CreateInstance(typeof(T), true));
					}
				}
			}
			return SingletonTemplate<T>.__singletionInstance;
		}

		
		private static T __singletionInstance = default(T);

		
		private static readonly object __singletionInstanceLock = new object();
	}
}
