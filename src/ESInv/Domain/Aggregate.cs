using System;
using System.Collections.Generic;


namespace ESInv.Domain
{
	public abstract class Aggregate<TState>
	{
		private readonly TState c_state;
		private readonly IList<ESInv.Messaging.IEvent> c_uncommittedChanges;


		protected TState State { get { return this.c_state; } }
		

		public IEnumerable<ESInv.Messaging.IEvent> UncommittedChanges { get { return this.c_uncommittedChanges; } }


		protected Aggregate(
			TState state)
		{
			this.c_state = state;

			this.c_uncommittedChanges = new List<ESInv.Messaging.IEvent>();
		}


		protected void Apply(
			ESInv.Messaging.IEvent @event)
		{
			this.c_uncommittedChanges.Add(@event);
			((dynamic)this.c_state).Mutate(@event);
		}


		public void ClearUncommitedChanges()
		{
			this.c_uncommittedChanges.Clear();
		}
	}
}
