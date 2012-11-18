using System;


namespace ESInv.Domain
{
	public interface IRepository<TAggregate>
	{
		TAggregate GetById(
			Guid id);


		void Save(
			TAggregate aggregate);
	}
}
