using System;

namespace Server.Tools.Pattern
{
	// Token: 0x020001B1 RID: 433
	public class SingletonTemplate<T> where T : class
	{
		// Token: 0x06000536 RID: 1334 RVA: 0x0004990C File Offset: 0x00047B0C
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

		// Token: 0x040009B6 RID: 2486
		private static T __singletionInstance = default(T);

		// Token: 0x040009B7 RID: 2487
		private static readonly object __singletionInstanceLock = new object();
	}
}
