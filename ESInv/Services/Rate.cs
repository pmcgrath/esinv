using System;


namespace ESInv.Services
{
	public class Rate : ESInv.Domain.IRateService
	{
		public decimal GetRate(
			string from,
			string to)
		{
			if (from == to)						{ return 1M; }
			if (from == "EUR" && to == "USD")	{ return 1.27M; }

			return 0;
		}
	}
}
