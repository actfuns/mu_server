using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Logic;

namespace GameServer.Interface
{
	
	public interface IObject
	{
		
		
		ObjectTypes ObjectType { get; }

		
		int GetObjectID();

		
		
		
		long LastLifeMagicTick { get; set; }

		
		
		
		Point CurrentGrid { get; set; }

		
		
		
		Point CurrentPos { get; set; }

		
		
		int CurrentMapCode { get; }

		
		
		int CurrentCopyMapID { get; }

		
		
		
		Dircetions CurrentDir { get; set; }

		
		
		
		List<int> PassiveEffectList { get; set; }

		
		T GetExtComponent<T>(ExtComponentTypes type) where T : class;
	}
}
