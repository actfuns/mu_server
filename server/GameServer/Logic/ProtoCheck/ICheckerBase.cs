using System;

namespace GameServer.Logic.ProtoCheck
{
	
	internal interface ICheckerBase
	{
		
		bool Check(object obj1, object obj2);
	}
}
