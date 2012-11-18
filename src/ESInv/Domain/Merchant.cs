using System;


namespace ESInv.Domain
{
	public class Merchant
	{
		public readonly int Id;
		public readonly string Name;


		public Merchant(
			int id,
			string name)
		{
			this.Id = id;
			this.Name = name;
		}
	}
}
