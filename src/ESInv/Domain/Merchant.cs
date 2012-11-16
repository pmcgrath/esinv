using System;


namespace ESInv.Domain
{
	public class Merchant
	{
		public int Id { get; private set; }
		public string Name { get; private set; }


		public Merchant(
			int id,
			string name)
		{
			this.Id = id;
			this.Name = name;
		}
	}
}
