using System;

namespace GameServer.Logic.Goods
{
	
	public class CondJudger_FruitSuitNotMax : ICondJudger
	{
		
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool bOK = false;
			int Strength = Global.GetRoleParamsInt32FromDB(client, "PropStrengthChangeless");
			int Intelligence = Global.GetRoleParamsInt32FromDB(client, "PropIntelligenceChangeless");
			int Dexterity = Global.GetRoleParamsInt32FromDB(client, "PropDexterityChangeless");
			int Constitution = Global.GetRoleParamsInt32FromDB(client, "PropConstitutionChangeless");
			if (Strength < UseFruitVerify.GetFruitAddPropLimit(client, "Strength") || Intelligence < UseFruitVerify.GetFruitAddPropLimit(client, "Intelligence") || Dexterity < UseFruitVerify.GetFruitAddPropLimit(client, "Dexterity") || Constitution < UseFruitVerify.GetFruitAddPropLimit(client, "Constitution"))
			{
				bOK = true;
			}
			if (!bOK)
			{
				failedMsg = GLang.GetLang(8015, new object[0]);
			}
			return bOK;
		}
	}
}
