using System;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public interface ICopySceneManager
	{
		
		bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType);

		
		bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType);

		
		void TimerProc();
	}
}
