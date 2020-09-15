using System;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.WanMota
{
	// Token: 0x020007B5 RID: 1973
	public class WanMoTaTopLayerManager : SingletonTemplate<WanMoTaTopLayerManager>
	{
		// Token: 0x060033E8 RID: 13288 RVA: 0x002DF384 File Offset: 0x002DD584
		private WanMoTaTopLayerManager()
		{
		}

		// Token: 0x060033E9 RID: 13289 RVA: 0x002DF3A4 File Offset: 0x002DD5A4
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

		// Token: 0x060033EA RID: 13290 RVA: 0x002DF404 File Offset: 0x002DD604
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

		// Token: 0x04003F6A RID: 16234
		private int iTopLayer = 0;

		// Token: 0x04003F6B RID: 16235
		private object TopLayerMutex = new object();
	}
}
