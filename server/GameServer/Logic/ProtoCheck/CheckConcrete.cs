using System;
using Server.Data;

namespace GameServer.Logic.ProtoCheck
{
	// Token: 0x020003BA RID: 954
	internal static class CheckConcrete
	{
		// Token: 0x0600107F RID: 4223 RVA: 0x000FF958 File Offset: 0x000FDB58
		public static bool Checker_SpriteActionData(SpriteActionData data1, SpriteActionData data2)
		{
			return data1.roleID == data2.roleID && data1.mapCode == data2.mapCode && data1.direction == data2.direction && data1.action == data2.action && data1.toX == data2.toX && data1.toY == data2.toY && data1.targetX == data2.targetX && data1.targetY == data2.targetY && data1.yAngle == data2.yAngle && data1.moveToX == data2.moveToX && data1.moveToY == data2.moveToY;
		}

		// Token: 0x06001080 RID: 4224 RVA: 0x000FFA1C File Offset: 0x000FDC1C
		public static bool Checker_SpriteMagicCodeData(SpriteMagicCodeData data1, SpriteMagicCodeData data2)
		{
			return data1.roleID == data2.roleID && data1.mapCode == data2.mapCode && data1.magicCode == data2.magicCode;
		}

		// Token: 0x06001081 RID: 4225 RVA: 0x000FFA5C File Offset: 0x000FDC5C
		public static bool Checker_SpriteMoveData(SpriteMoveData data1, SpriteMoveData data2)
		{
			return data1.roleID == data2.roleID && data1.mapCode == data2.mapCode && data1.action == data2.action && data1.toX == data2.toX && data1.toY == data2.toY && data1.extAction == data2.extAction && data1.fromX == data2.fromX && data1.fromY == data2.fromY && data1.startMoveTicks == data2.startMoveTicks && data1.pathString == data2.pathString;
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x000FFB04 File Offset: 0x000FDD04
		public static bool Checker_SpritePositionData(SpritePositionData data1, SpritePositionData data2)
		{
			return data1.roleID == data2.roleID && data1.mapCode == data2.mapCode && data1.toX == data2.toX && data1.toY == data2.toY && data1.currentPosTicks == data2.currentPosTicks;
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x000FFB60 File Offset: 0x000FDD60
		public static bool Checker_SpriteAttackData(SpriteAttackData data1, SpriteAttackData data2)
		{
			return data1.roleID == data2.roleID && data1.roleX == data2.roleX && data1.roleY == data2.roleY && data1.enemy == data2.enemy && data1.enemyX == data2.enemyX && data1.enemyY == data2.enemyY && data1.realEnemyX == data2.realEnemyX && data1.realEnemyY == data2.realEnemyY && data1.magicCode == data2.magicCode;
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x000FFBF4 File Offset: 0x000FDDF4
		public static bool Checker_CS_SprUseGoods(CS_SprUseGoods data1, CS_SprUseGoods data2)
		{
			return data1.RoleId == data2.RoleId && data1.DbId == data2.DbId && data1.GoodsId == data2.GoodsId && data1.UseNum == data2.UseNum;
		}

		// Token: 0x06001085 RID: 4229 RVA: 0x000FFC44 File Offset: 0x000FDE44
		public static bool Checker_CS_QueryFuBen(CS_QueryFuBen data1, CS_QueryFuBen data2)
		{
			return data1.RoleId == data2.RoleId && data1.MapId == data2.MapId && data1.FuBenId == data2.FuBenId;
		}

		// Token: 0x06001086 RID: 4230 RVA: 0x000FFC84 File Offset: 0x000FDE84
		public static bool Checker_CS_ClickOn(CS_ClickOn data1, CS_ClickOn data2)
		{
			return data1.RoleId == data2.RoleId && data1.MapCode == data2.MapCode && data1.NpcId == data2.NpcId && data1.ExtId == data2.ExtId;
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x000FFCD4 File Offset: 0x000FDED4
		public static bool Checker_SCClientHeart(SCClientHeart data1, SCClientHeart data2)
		{
			return data1.RoleID == data2.RoleID && data1.RandToken == data2.RandToken && data1.Ticks == data2.Ticks;
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x000FFD14 File Offset: 0x000FDF14
		public static bool Checker_SCFindMonster(SCFindMonster data1, SCFindMonster data2)
		{
			return data1.RoleID == data2.RoleID && data1.X == data2.X && data1.Y == data2.Y && data1.Num == data2.Num;
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x000FFD64 File Offset: 0x000FDF64
		public static bool Checker_SCMoveEnd(SCMoveEnd data1, SCMoveEnd data2)
		{
			return data1.RoleID == data2.RoleID && data1.Action == data2.Action && data1.MapCode == data2.MapCode && data1.ToMapX == data2.ToMapX && data1.ToMapY == data2.ToMapY && data1.ToDiection == data2.ToDiection && data1.TryRun == data2.TryRun;
		}

		// Token: 0x0600108A RID: 4234 RVA: 0x000FFDDC File Offset: 0x000FDFDC
		public static bool Checker_CSPropAddPoint(CSPropAddPoint data1, CSPropAddPoint data2)
		{
			return data1.RoleID == data2.RoleID && data1.Strength == data2.Strength && data1.Intelligence == data2.Intelligence && data1.Dexterity == data2.Dexterity && data1.Constitution == data2.Constitution;
		}

		// Token: 0x0600108B RID: 4235 RVA: 0x000FFE38 File Offset: 0x000FE038
		public static bool Checker_SCMapChange(SCMapChange data1, SCMapChange data2)
		{
			return data1.RoleID == data2.RoleID && data1.TeleportID == data2.TeleportID && data1.NewMapCode == data2.NewMapCode && data1.ToNewMapX == data2.ToNewMapX && data1.ToNewMapY == data2.ToNewMapY && data1.ToNewDiection == data2.ToNewDiection && data1.State == data2.State;
		}
	}
}
