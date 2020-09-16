using System;

namespace GameDBServer.Logic
{
	// Token: 0x020001D2 RID: 466
	public class MyWeakReference
	{
		// Token: 0x060009CF RID: 2511 RVA: 0x0005E7EE File Offset: 0x0005C9EE
		public MyWeakReference(object target)
		{
			this._Target = target;
		}

		// Token: 0x170000D7 RID: 215
		
		public bool IsAlive
		{
			get
			{
				bool result;
				lock (this._ThreadMutex)
				{
					result = (null != this._Target);
				}
				return result;
			}
		}

		// Token: 0x170000D8 RID: 216
		
		
		public object Target
		{
			get
			{
				object target;
				lock (this._ThreadMutex)
				{
					target = this._Target;
				}
				return target;
			}
			set
			{
				lock (this._ThreadMutex)
				{
					this._Target = value;
				}
			}
		}

		// Token: 0x04000BF7 RID: 3063
		private object _ThreadMutex = new object();

		// Token: 0x04000BF8 RID: 3064
		private object _Target = null;
	}
}
