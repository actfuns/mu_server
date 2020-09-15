using System;

namespace Server.Tools
{
	// Token: 0x02000102 RID: 258
	public class SingletonTemplate<T> where T : class
	{
		// Token: 0x0600044F RID: 1103 RVA: 0x00021784 File Offset: 0x0001F984
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

		// Token: 0x0400073A RID: 1850
		private static T __singletionInstance = default(T);

		// Token: 0x0400073B RID: 1851
		private static readonly object __singletionInstanceLock = new object();
	}
}
