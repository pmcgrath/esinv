using NUnit.Framework;
using System;


namespace ESInv.Tests.Domain
{
	[TestFixture]
	public class Money
	{
		[Test]
		public void Ctor_Lower_Case_Currency_Paramter_Results_In_Object_Creation_With_Upper_Case_Currency()
		{
			var _SUT = new ESInv.Domain.Money("eur", 100M);

			Assert.AreEqual("EUR", _SUT.Currency);
		}

	
		[Test]
		public void Equals_Sign_Where_Both_Null_Result_Is_True()
		{
			ESInv.Domain.Money _value1 = null;
			ESInv.Domain.Money _value2 = null;

			Assert.IsTrue(_value1 == _value2);
		}


		[Test]
		public void Equals_Sign_Where_One_Is_Null_Results_Is_False()
		{
			ESInv.Domain.Money _value1 = new ESInv.Domain.Money("EUR", 100M);
			ESInv.Domain.Money _value2 = null;

			Assert.IsFalse(_value1 == _value2);
		}


		[Test]
		public void Equals_Sign_Where_Same_Values_Result_Is_True()
		{
			var _value1 = new ESInv.Domain.Money("EUR", 100M);
			var _value2 = new ESInv.Domain.Money("EUR", 100M);

			Assert.IsTrue(_value1 == _value2);
		}


		[Test]
		public void Equals_Sign_Where_Different_Values_Results_Is_False()
		{
			var _value1 = new ESInv.Domain.Money("EUR", 100M);
			var _value2 = new ESInv.Domain.Money("EUR", 99.99M);

			Assert.IsFalse(_value1 == _value2);
		}


		[Test]
		public void NotEquals_Where_Both_Null_Results_Is_False()
		{
			ESInv.Domain.Money _value1 = null;
			ESInv.Domain.Money _value2 = null;

			Assert.IsFalse(_value1 != _value2);
		}


		[Test]
		public void NotEquals_Where_One_Is_Null_Result_Is_True()
		{
			ESInv.Domain.Money _value1 = new ESInv.Domain.Money("EUR", 100M);
			ESInv.Domain.Money _value2 = null;

			Assert.IsTrue(_value1 != _value2);
		}


		[Test]
		public void NotEquals_Where_Same_Values_Results_Is_False()
		{
			var _value1 = new ESInv.Domain.Money("EUR", 100M);
			var _value2 = new ESInv.Domain.Money("EUR", 100M);

			Assert.IsFalse(_value1 != _value2);
		}


		[Test]
		public void NotEquals_Where_Different_Values_Result_Is_True()
		{
			var _value1 = new ESInv.Domain.Money("EUR", 100M);
			var _value2 = new ESInv.Domain.Money("EUR", 99.99M);

			Assert.IsTrue(_value1 != _value2);
		}


		[Test]
		public void Minus_Where_Second_Is_Smaller_Results_In_Good_Result()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _euro90 = new ESInv.Domain.Money("EUR", 90M);

			var _result = _euro100 - _euro90;

			Assert.AreEqual(new ESInv.Domain.Money("EUR", 10M), _result);
		}


		[Test, ExpectedException]
		public void Minus_Where_Second_Is_Greater_Than_First_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _euro90 = new ESInv.Domain.Money("EUR", 190M);

			var _result = _euro100 - _euro90;
		}


		[Test, ExpectedException]
		public void Minus_Where_Second_Is_Different_Currency_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _US100 = new ESInv.Domain.Money("USD", 90M);

			var _result = _euro100 - _US100;
		}


		[Test, ExpectedException]
		public void Minus_Where_Second_Is_Null_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);

			var _result = _euro100 - null;
		}


		[Test]
		public void Plus_Where_Good_Arguments_Results_In_Good_Result()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _euro90 = new ESInv.Domain.Money("EUR", 90M);

			var _result = _euro100 + _euro90;

			Assert.AreEqual(new ESInv.Domain.Money("EUR", 190M), _result);
		}


		[Test, ExpectedException]
		public void Plus_Where_Second_Is_Different_Currency_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _US100 = new ESInv.Domain.Money("USD", 90M);

			var _result = _euro100 + _US100;
		}


		[Test, ExpectedException]
		public void Plus_Where_Second_Is_Null_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);

			var _result = _euro100 + null;
		}


		[Test]
		public void Greater_Than_Where_Good_Arguments_Result_Is_True()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _euro90 = new ESInv.Domain.Money("EUR", 90M);

			Assert.IsTrue(_euro100 > _euro90);
		}


		[Test]
		public void Greater_Than_Where_Good_Arguments_Result_Is_False()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _euro190 = new ESInv.Domain.Money("EUR", 190M);

			Assert.IsFalse(_euro100 > _euro190);
		}


		[Test, ExpectedException]
		public void Greater_Than_Where_Second_Is_Different_Currency_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _US100 = new ESInv.Domain.Money("USD", 90M);

			var _result = _euro100 > _US100;
		}


		[Test, ExpectedException]
		public void Greater_Than_Where_Second_Is_Null_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);

			var _result = _euro100 > null;
		}


		[Test]
		public void Greater_Than_Or_Equal_Where_Good_Arguments_Result_Is_True()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _euro90 = new ESInv.Domain.Money("EUR", 90M);

			Assert.IsTrue(_euro100 >= _euro90);
		}


		[Test]
		public void Greater_Than_Or_Equal_Where_Good_Arguments_Result_Is_False()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _euro190 = new ESInv.Domain.Money("EUR", 190M);

			Assert.IsFalse(_euro100 >= _euro190);
		}


		[Test]
		public void Greater_Than_Or_Equal_Where_Good_Arguments_Both_Are_Same_Value_Result_Is_False()
		{
			var _euro100_1 = new ESInv.Domain.Money("EUR", 100M);
			var _euro100_2 = new ESInv.Domain.Money("EUR", 100M);

			Assert.IsTrue(_euro100_1 >= _euro100_2);
		}


		[Test, ExpectedException]
		public void Greater_Than_Or_Equal_Where_Second_Is_Different_Currency_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _US100 = new ESInv.Domain.Money("USD", 90M);

			var _result = _euro100 >= _US100;
		}


		[Test, ExpectedException]
		public void Greater_Than_Or_Equal_Where_Second_Is_Null_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);

			var _result = _euro100 >= null;
		}


		[Test]
		public void Less_Than_Where_Good_Arguments_Result_Is_True()
		{
			var _euro90 = new ESInv.Domain.Money("EUR", 90M);
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);

			Assert.IsTrue(_euro90 < _euro100);
		}


		[Test]
		public void Less_Than_Where_Good_Arguments_Result_Is_False()
		{
			var _euro190 = new ESInv.Domain.Money("EUR", 190M);
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);

			Assert.IsFalse(_euro190 < _euro100);
		}


		[Test, ExpectedException]
		public void Less_Than_Where_Second_Is_Different_Currency_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _US100 = new ESInv.Domain.Money("USD", 90M);

			var _result = _euro100 < _US100;
		}


		[Test, ExpectedException]
		public void Less_Than_Where_Second_Is_Null_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);

			var _result = _euro100 < null;
		}


		[Test]
		public void Less_Than_Or_Equal_Where_Good_Arguments_Result_Is_True()
		{
			var _euro90 = new ESInv.Domain.Money("EUR", 90M);
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);

			Assert.IsTrue(_euro90 <= _euro100);
		}


		[Test]
		public void Less_Than_Or_Equal_Where_Good_Arguments_Result_Is_False()
		{
			var _euro190 = new ESInv.Domain.Money("EUR", 190M);
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);

			Assert.IsFalse(_euro190 <= _euro100);
		}


		[Test]
		public void Less_Than_Or_Equal_Where_Good_Arguments_Both_Are_Same_Value_Result_Is_False()
		{
			var _euro100_1 = new ESInv.Domain.Money("EUR", 100M);
			var _euro100_2 = new ESInv.Domain.Money("EUR", 100M);

			Assert.IsTrue(_euro100_1 <= _euro100_2);
		}


		[Test, ExpectedException]
		public void Less_Than_Or_Equal_Where_Second_Is_Different_Currency_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);
			var _US100 = new ESInv.Domain.Money("USD", 90M);

			var _result = _euro100 <= _US100;
		}


		[Test, ExpectedException]
		public void Less_Than_Or_Equal_Where_Second_Is_Null_Results_In_An_Exception()
		{
			var _euro100 = new ESInv.Domain.Money("EUR", 100M);

			var _result = _euro100 <= null;
		}
	}
}
