using System;

namespace GameServer.Logic.OnePiece
{
	
	public enum OnePieceTreasureErrorCode
	{
		
		OnePiece_Success,
		
		OnePiece_ErrorZuanShiNotEnough,
		
		OnePiece_ErrorBagNotEnough,
		
		OnePiece_ErrorParams,
		
		OnePiece_DBFailed,
		
		OnePiece_ErrorMoving,
		
		OnePiece_ErrorNotHaveEvent,
		
		OnePiece_ErrorNeedGoodsID,
		
		OnePiece_ErrorNeedGoodsCount,
		
		OnePiece_ErrorGoodsNotEnough,
		
		OnePiece_ErrorNeedMoneyNotEnough,
		
		OnePiece_ErrorMoveRange,
		
		OnePiece_ErrorMoveNumNotEnough,
		
		OnePiece_ResetPos,
		
		OnePiece_ErrorRollNumMax,
		
		OnePiece_ErrorRollNumNotEnough,
		
		OnePiece_ErrorCheckMail
	}
}
