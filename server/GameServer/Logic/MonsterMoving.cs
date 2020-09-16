using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using HSGameEngine.Tools.AStarEx;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class MonsterMoving
	{
		
		public bool _LinearMove(Monster sprite, Point p, int action)
		{
			long ticks = TimeUtil.NOW();
			sprite.DestPoint = p;
			bool ret = this.AStarMove(sprite, p, action);
			long ticks2 = TimeUtil.NOW();
			if (ticks2 > ticks + 100L)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("_LinearMove 消耗:{0}毫秒, start({1}, {2}), to({3}, {4}), mapID={5}", new object[]
				{
					ticks2 - ticks,
					sprite.Coordinate.X,
					sprite.Coordinate.Y,
					p.X,
					p.Y,
					sprite.MonsterZoneNode.MapCode
				}), null, true);
			}
			return ret;
		}

		
		public bool FindLinearNoObsMaxPoint(GameMap gameMap, Monster sprite, Point p, out Point maxPoint)
		{
			List<ANode> path = new List<ANode>();
			Global.Bresenham(path, (int)(sprite.Coordinate.X / (double)gameMap.MapGridWidth), (int)(sprite.Coordinate.Y / (double)gameMap.MapGridHeight), (int)(p.X / (double)gameMap.MapGridWidth), (int)(p.Y / (double)gameMap.MapGridHeight), gameMap.MyNodeGrid);
			bool result;
			if (path.Count > 1)
			{
				maxPoint = new Point((double)(path[path.Count - 1].x * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(path[path.Count - 1].y * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
				path.Clear();
				result = true;
			}
			else
			{
				maxPoint = new Point(0.0, 0.0);
				result = false;
			}
			return result;
		}

		
		protected double CalcDirection(Point op, Point ep)
		{
			return Global.GetDirectionByTan(ep.X, ep.Y, op.X, op.Y);
		}

		
		private bool AStarMove(Monster sprite, Point p, int action)
		{
			Point srcPoint = sprite.Coordinate;
			Point start = new Point
			{
				X = srcPoint.X / 20.0,
				Y = srcPoint.Y / 20.0
			};
			Point end = new Point
			{
				X = p.X / 20.0,
				Y = p.Y / 20.0
			};
			bool result;
			if (start.X == end.X && start.Y == end.Y)
			{
				result = true;
			}
			else
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[sprite.MonsterZoneNode.MapCode];
				if (start != end)
				{
					List<ANode> path = null;
					gameMap.MyNodeGrid.setStartNode((int)start.X, (int)start.Y);
					gameMap.MyNodeGrid.setEndNode((int)end.X, (int)end.Y);
					try
					{
						path = gameMap.MyAStarFinder.find(gameMap.MyNodeGrid);
					}
					catch (Exception)
					{
						sprite.DestPoint = new Point(-1.0, -1.0);
						LogManager.WriteLog(LogTypes.Error, string.Format("AStar怪物寻路失败, ExtenstionID={0}, Start=({1},{2}), End=({3},{4}), fixedObstruction=({5},{6})", new object[]
						{
							sprite.MonsterInfo.ExtensionID,
							(int)start.X,
							(int)start.Y,
							(int)end.X,
							(int)end.Y,
							gameMap.MyNodeGrid.numCols,
							gameMap.MyNodeGrid.numRows
						}), null, true);
						return false;
					}
					if (path == null || path.Count <= 1)
					{
						Point maxPoint;
						if (this.FindLinearNoObsMaxPoint(gameMap, sprite, p, out maxPoint))
						{
							path = null;
							end = new Point
							{
								X = maxPoint.X / (double)gameMap.MapGridWidth,
								Y = maxPoint.Y / (double)gameMap.MapGridHeight
							};
							p = maxPoint;
							gameMap.MyNodeGrid.setStartNode((int)start.X, (int)start.Y);
							gameMap.MyNodeGrid.setEndNode((int)end.X, (int)end.Y);
							path = gameMap.MyAStarFinder.find(gameMap.MyNodeGrid);
						}
					}
					if (path == null || path.Count <= 1)
					{
						sprite.DestPoint = new Point(-1.0, -1.0);
						sprite.Action = GActions.Stand;
						Global.RemoveStoryboard(sprite.Name);
						return false;
					}
					sprite.Destination = p;
					double UnitCost = (double)Data.RunUnitCost;
					UnitCost /= sprite.MoveSpeed;
					UnitCost = 20.0 / UnitCost * (double)Global.MovingFrameRate;
					UnitCost *= 0.5;
					StoryBoardEx.RemoveStoryBoard(sprite.Name);
					StoryBoardEx sb = new StoryBoardEx(sprite.Name);
					sb.Completed = new StoryBoardEx.CompletedDelegateHandle(this.Move_Completed);
					Point firstPoint = new Point((double)(path[0].x * gameMap.MapGridWidth), (double)(path[0].y * gameMap.MapGridHeight));
					sprite.Direction = this.CalcDirection(sprite.Coordinate, firstPoint);
					sprite.Action = (GActions)action;
					sb.Binding();
					sprite.FirstStoryMove = true;
					sb.Start(sprite, path, UnitCost, 20);
				}
				result = true;
			}
			return result;
		}

		
		private void Move_Completed(object sender, EventArgs e)
		{
			Global.RemoveStoryboard((sender as StoryBoardEx).Name);
		}

		
		public double CalcDirection(Monster sprite, Point p)
		{
			return Global.GetDirectionByTan(p.X, p.Y, sprite.Coordinate.X, sprite.Coordinate.Y);
		}

		
		public void ChangeDirection(Monster sprite, double direction)
		{
			if (sprite.Direction != direction)
			{
				sprite.Direction = direction;
			}
		}

		
		public double ChangeDirection(Monster sprite, Point p)
		{
			double direction = Global.GetDirectionByTan(p.X, p.Y, sprite.Coordinate.X, sprite.Coordinate.Y);
			if (sprite.Direction != direction)
			{
				sprite.Direction = direction;
			}
			return direction;
		}
	}
}
