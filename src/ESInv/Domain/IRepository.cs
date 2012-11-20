using System;


namespace ESInv.Domain
{
	public interface IRepository<TAggregate>
		where TAggregate : IAggregate
	{
		TAggregate GetById(
			Guid id);


		void Save(
			TAggregate aggregate);
	}
}
