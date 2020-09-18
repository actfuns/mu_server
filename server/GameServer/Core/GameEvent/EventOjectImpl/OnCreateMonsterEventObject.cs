using System;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class OnCreateMonsterEventObject : EventObjectEx
	{
		
		public OnCreateMonsterEventObject(Monster monster) : base(30)
		{
			this.Monster = monster;
		}

		
		public Monster Monster;
	}
}
