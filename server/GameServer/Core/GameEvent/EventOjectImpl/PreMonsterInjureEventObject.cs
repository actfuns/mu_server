using System;
using GameServer.Interface;
using GameServer.Logic;
using Tmsk.Contract;

namespace GameServer.Core.GameEvent.EventOjectImpl
{
	
	public class PreMonsterInjureEventObject : EventObjectEx
	{
		
		public PreMonsterInjureEventObject(IObject attacker, Monster monster, int sceneType) : base(33)
		{
			this.Attacker = attacker;
			this.Monster = monster;
			this.SceneType = sceneType;
		}

		
		public int SceneType;

		
		public IObject Attacker;

		
		public Monster Monster;

		
		public int Injure;
	}
}
