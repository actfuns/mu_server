using System;

namespace GameDBServer.Logic
{
	
	public class MyWeakReference
	{
		
		public MyWeakReference(object target)
		{
			this._Target = target;
		}

		
		
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

		
		private object _ThreadMutex = new object();

		
		private object _Target = null;
	}
}
