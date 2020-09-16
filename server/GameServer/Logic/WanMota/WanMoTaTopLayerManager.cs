using System;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.WanMota
{
	
	public class WanMoTaTopLayerManager : SingletonTemplate<WanMoTaTopLayerManager>
	{
		
		private WanMoTaTopLayerManager()
		{
		}

		
		public void CheckNeedUpdate(int layer)
		{
			lock (this.TopLayerMutex)
			{
				if (this.iTopLayer < layer)
				{
					this.iTopLayer = layer;
				}
			}
		}

		
		public void OnClientPass(GameClient client, int layer)
		{
			if (layer >= 30 && layer % 10 == 0)
			{
				string broadCastMsg = StringUtil.substitute(GLang.GetLang(572, new object[0]), new object[]
				{
					Global.FormatRoleName(client, client.ClientData.RoleName),
					layer
				});
				Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.HintMsg, broadCastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
			}
			if (layer >= 30)
			{
				bool bTop = false;
				lock (this.TopLayerMutex)
				{
					if (this.iTopLayer < layer)
					{
						bTop = true;
						this.iTopLayer = layer;
					}
				}
				if (bTop)
				{
					string broadCastMsg = StringUtil.substitute(GLang.GetLang(573, new object[0]), new object[]
					{
						Global.FormatRoleName(client, client.ClientData.RoleName)
					});
					Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.HintMsg, broadCastMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
				}
			}
		}

		
		private int iTopLayer = 0;

		
		private object TopLayerMutex = new object();
	}
}
