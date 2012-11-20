using System;
using System.Linq;


namespace ESInv.Data
{
	public class Repository<TAggregate> : ESInv.Domain.IRepository<TAggregate>
		where TAggregate : ESInv.Domain.IAggregate
	{
		private readonly ESInv.Data.EventStore.IStore c_eventStore = new ESInv.Data.EventStore.Store();


		public TAggregate GetById(
			Guid id)
		{
			var _eventStream = string.Format("{0}/{1}", typeof(TAggregate).Name, id);
			
			var _events = this.c_eventStore.GetAll(_eventStream);
			
			return (TAggregate)Activator.CreateInstance(typeof(TAggregate), _events);
		}


		public void Save(
			TAggregate aggregate)
		{
			var _eventStream = string.Format("{0}/{1}", typeof(TAggregate).Name, aggregate.Id);
			
			var _expectedVersion = aggregate.Version - aggregate.UncommittedChanges.Count();

			this.c_eventStore.Save(_eventStream, _expectedVersion, aggregate.UncommittedChanges);

			aggregate.ClearUncommitedChanges();
		}
	}
}
