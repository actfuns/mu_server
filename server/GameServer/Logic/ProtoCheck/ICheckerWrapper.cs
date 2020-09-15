using System;

namespace GameServer.Logic.ProtoCheck
{
	// Token: 0x020003B7 RID: 951
	internal class ICheckerWrapper<T> : ICheckerBase where T : class
	{
		// Token: 0x06001073 RID: 4211 RVA: 0x000FF4A4 File Offset: 0x000FD6A4
		public ICheckerWrapper(ICheckerWrapper<T>.CheckerCallback cb)
		{
			this._cb = cb;
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x000FF4C0 File Offset: 0x000FD6C0
		public bool Check(object obj1, object obj2)
		{
			T data = obj1 as T;
			T data2 = obj2 as T;
			return this._cb(data, data2);
		}

		// Token: 0x04001901 RID: 6401
		private ICheckerWrapper<T>.CheckerCallback _cb = null;

		// Token: 0x020003B8 RID: 952
		// (Invoke) Token: 0x06001076 RID: 4214
		public delegate bool CheckerCallback(T data1, T data2);
	}
}
