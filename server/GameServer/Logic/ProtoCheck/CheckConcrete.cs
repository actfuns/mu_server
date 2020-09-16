using System;
using Server.Data;

namespace GameServer.Logic.ProtoCheck
{
	
	internal static class CheckConcrete
	{
		
		public static bool Checker_SpriteActionData(SpriteActionData data1, SpriteActionData data2)
		{
			return data1.roleID == data2.roleID && data1.mapCode == data2.mapCode && data1.direction == data2.direction && data1.action == data2.action && data1.toX == data2.toX && data1.toY == data2.toY && data1.targetX == data2.targetX && data1.targetY == data2.targetY && data1.yAngle == data2.yAngle && data1.moveToX == data2.moveToX && data1.moveToY == data2.moveToY;
		}

		
		public static bool Checker_SpriteMagicCodeData(SpriteMagicCodeData data1, SpriteMagicCodeData data2)
		{
			return data1.roleID == data2.roleID && data1.mapCode == data2.mapCode && data1.magicCode == data2.magicCode;
		}

		
		public static bool Checker_SpriteMoveData(SpriteMoveData data1, SpriteMoveData data2)
		{
			return data1.roleID == data2.roleID && data1.mapCode == data2.mapCode && data1.action == data2.action && data1.toX == data2.toX && data1.toY == data2.toY && data1.extAction == data2.extAction && data1.fromX == data2.fromX && data1.fromY == data2.fromY && data1.startMoveTicks == data2.startMoveTicks && data1.pathString == data2.pathString;
		}

		
		public static bool Checker_SpritePositionData(SpritePositionData data1, SpritePositionData data2)
		{
			return data1.roleID == data2.roleID && data1.mapCode == data2.mapCode && data1.toX == data2.toX && data1.toY == data2.toY && data1.currentPosTicks == data2.currentPosTicks;
		}

		
		public static bool Checker_SpriteAttackData(SpriteAttackData data1, SpriteAttackData data2)
		{
			return data1.roleID == data2.roleID && data1.roleX == data2.roleX && data1.roleY == data2.roleY && data1.enemy == data2.enemy && data1.enemyX == data2.enemyX && data1.enemyY == data2.enemyY && data1.realEnemyX == data2.realEnemyX && data1.realEnemyY == data2.realEnemyY && data1.magicCode == data2.magicCode;
		}

		
		public static bool Checker_CS_SprUseGoods(CS_SprUseGoods data1, CS_SprUseGoods data2)
		{
			return data1.RoleId == data2.RoleId && data1.DbId == data2.DbId && data1.GoodsId == data2.GoodsId && data1.UseNum == data2.UseNum;
		}

		
		public static bool Checker_CS_QueryFuBen(CS_QueryFuBen data1, CS_QueryFuBen data2)
		{
			return data1.RoleId == data2.RoleId && data1.MapId == data2.MapId && data1.FuBenId == data2.FuBenId;
		}

		
		public static bool Checker_CS_ClickOn(CS_ClickOn data1, CS_ClickOn data2)
		{
			return data1.RoleId == data2.RoleId && data1.MapCode == data2.MapCode && data1.NpcId == data2.NpcId && data1.ExtId == data2.ExtId;
		}

		
		public static bool Checker_SCClientHeart(SCClientHeart data1, SCClientHeart data2)
		{
			return data1.RoleID == data2.RoleID && data1.RandToken == data2.RandToken && data1.Ticks == data2.Ticks;
		}

		
		public static bool Checker_SCFindMonster(SCFindMonster data1, SCFindMonster data2)
		{
			return data1.RoleID == data2.RoleID && data1.X == data2.X && data1.Y == data2.Y && data1.Num == data2.Num;
		}

		
		public static bool Checker_SCMoveEnd(SCMoveEnd data1, SCMoveEnd data2)
		{
			return data1.RoleID == data2.RoleID && data1.Action == data2.Action && data1.MapCode == data2.MapCode && data1.ToMapX == data2.ToMapX && data1.ToMapY == data2.ToMapY && data1.ToDiection == data2.ToDiection && data1.TryRun == data2.TryRun;
		}

		
		public static bool Checker_CSPropAddPoint(CSPropAddPoint data1, CSPropAddPoint data2)
		{
			return data1.RoleID == data2.RoleID && data1.Strength == data2.Strength && data1.Intelligence == data2.Intelligence && data1.Dexterity == data2.Dexterity && data1.Constitution == data2.Constitution;
		}

		
		public static bool Checker_SCMapChange(SCMapChange data1, SCMapChange data2)
		{
			return data1.RoleID == data2.RoleID && data1.TeleportID == data2.TeleportID && data1.NewMapCode == data2.NewMapCode && data1.ToNewMapX == data2.ToNewMapX && data1.ToNewMapY == data2.ToNewMapY && data1.ToNewDiection == data2.ToNewDiection && data1.State == data2.State;
		}
	}
}
