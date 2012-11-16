using System;


namespace ESInv.Domain
{
	public interface ICardNumberResolutionService
	{
		string Resolve(
			ulong number);
	}
}
