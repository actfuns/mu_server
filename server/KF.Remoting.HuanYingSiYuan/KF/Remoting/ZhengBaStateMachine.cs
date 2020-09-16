using System;

namespace KF.Remoting
{
	
	internal class ZhengBaStateMachine
	{
		
		public ZhengBaStateMachine.StateType GetCurrState()
		{
			return this._CurrState;
		}

		
		public void SetCurrState(ZhengBaStateMachine.StateType state, DateTime now)
		{
			ZhengBaStateMachine.StateHandler oldHandler = this.Handlers[(int)this._CurrState];
			if (oldHandler != null)
			{
				oldHandler.Leave(now);
			}
			this._CurrState = state;
			ZhengBaStateMachine.StateHandler newHandler = this.Handlers[(int)this._CurrState];
			this._CurrStateEnterTicks = now.Ticks;
			if (newHandler != null)
			{
				newHandler.Enter(now);
			}
		}

		
		public long ContinueTicks(DateTime now)
		{
			return now.Ticks - this._CurrStateEnterTicks;
		}

		
		public void Install(ZhengBaStateMachine.StateHandler handler)
		{
			this.Handlers[(int)handler.State] = handler;
		}

		
		public void Tick(DateTime now)
		{
			ZhengBaStateMachine.StateHandler handler = this.Handlers[(int)this._CurrState];
			if (handler != null)
			{
				handler.Update(now);
			}
		}

		
		private ZhengBaStateMachine.StateHandler[] Handlers = new ZhengBaStateMachine.StateHandler[8];

		
		private ZhengBaStateMachine.StateType _CurrState = ZhengBaStateMachine.StateType.None;

		
		private long _CurrStateEnterTicks;

		
		public enum StateType
		{
			
			None,
			
			Idle,
			
			TodayPkStart,
			
			TodayPkEnd,
			
			InitPkLoop,
			
			NotifyEnter,
			
			PkLoopStart,
			
			PkLoopEnd,
			
			Max
		}

		
		public class StateHandler
		{
			
			
			
			public ZhengBaStateMachine.StateType State { get; private set; }

			
			public void Enter(DateTime now)
			{
				if (this.enterAction != null)
				{
					this.enterAction(now);
				}
			}

			
			public void Update(DateTime now)
			{
				if (this.updateAction != null)
				{
					this.updateAction(now);
				}
			}

			
			public void Leave(DateTime now)
			{
				if (this.leaveAction != null)
				{
					this.leaveAction(now);
				}
			}

			
			public StateHandler(ZhengBaStateMachine.StateType state, Action<DateTime> enter, Action<DateTime> updater, Action<DateTime> leaver)
			{
				this.State = state;
				this.enterAction = enter;
				this.updateAction = updater;
				this.leaveAction = leaver;
			}

			
			private Action<DateTime> enterAction = null;

			
			private Action<DateTime> updateAction = null;

			
			private Action<DateTime> leaveAction = null;
		}
	}
}
