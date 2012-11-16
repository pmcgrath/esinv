using System;


namespace ESInv.Domain
{
	public interface IRateService
	{
		decimal GetRate(
			string from,
			string to);
	}
}
