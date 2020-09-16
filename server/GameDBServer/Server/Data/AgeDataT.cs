using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	[Serializable]
	public class AgeDataT<T>
	{
		
		public AgeDataT()
		{
		}

		
		public AgeDataT(long age, T v)
		{
			this.Age = age;
			this.V = v;
		}

		
		[ProtoMember(1)]
		public long Age;

		
		[ProtoMember(2)]
		public T V;
	}
}
