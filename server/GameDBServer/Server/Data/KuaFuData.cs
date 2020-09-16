using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	[Serializable]
	public class KuaFuData<T> where T : new()
	{
		
		public KuaFuData()
		{
		}

		
		public KuaFuData(long age, T v)
		{
			this.Age = age;
			this.V = v;
		}

		
		[ProtoMember(1)]
		public long Age;

		
		[ProtoMember(2)]
		public T V = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
	}
}
