using System;


namespace ESInv.Services
{
	public class CardNumberResolution : ESInv.Domain.ICardNumberResolutionService
	{
		public string Resolve(
			ulong number)
		{
			return "USD";
		}
	}
}
