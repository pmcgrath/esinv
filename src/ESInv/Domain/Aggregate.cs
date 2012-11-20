using System;
using System.Collections.Generic;


namespace ESInv.Domain
{
	public abstract class Aggregate<TState> : ESInv.Domain.IAggregate
		where TState : IAggregateState
	{
		private readonly TState c_state;
		private readonly IList<ESInv.Messaging.IEvent> c_uncommittedChanges;


		protected TState State { get { return this.c_state; } }


		public Guid Id { get { return this.c_state.Id; } }
		public int Version { get { return this.c_state.Version; } }
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
			((dynamic)this.c_state).Mutate(@event);

			this.c_uncommittedChanges.Add(@event);
		}


		public void ClearUncommitedChanges()
		{
			this.c_uncommittedChanges.Clear();
		}
	}
}
