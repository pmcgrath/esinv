using System;


namespace CQRSInv
{
	class Program
	{
		static void Main(
			string[] args)
		{
			new ESInv.Tests.OrderState().When_Order_Created_Should_Result_In_An_Order_With_Order_Attributes();
		}
	}
}
