using System;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x0200008C RID: 140
	public interface ICopySceneManager
	{
		// Token: 0x0600020F RID: 527
		bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType);

		// Token: 0x06000210 RID: 528
		bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType);

		// Token: 0x06000211 RID: 529
		void TimerProc();
	}
}
