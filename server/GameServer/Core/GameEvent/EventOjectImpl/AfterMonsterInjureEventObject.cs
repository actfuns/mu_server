using System;
using GameServer.Interface;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class AfterMonsterInjureEventObject : EventObjectEx
	{
		
		public AfterMonsterInjureEventObject(IObject attacker, Monster monster, int sceneType, int injure, int merlininjure) : base(34)
		{
			this.Attacker = attacker;
			this.Monster = monster;
			this.SceneType = sceneType;
			this.Injure = injure;
			this.MerlinInjure = merlininjure;
		}

		
		public int SceneType;

		
		public IObject Attacker;

		
		public Monster Monster;

		
		public int Injure;

		
		public int MerlinInjure;
	}
}
