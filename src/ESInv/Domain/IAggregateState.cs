using System;


namespace ESInv.Domain
{
	public interface IAggregateState
	{
		Guid Id { get; }
		int Version { get; }
	}
}
