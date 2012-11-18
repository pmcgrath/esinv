using System;


namespace ESInv.Domain
{
	public class Money
	{
		public readonly string Currency;
		public readonly decimal Amount;


		public Money(
			string currency,
			decimal amount)
		{
			this.Currency = currency.ToUpper();
			this.Amount = amount;
		}


		public static bool operator ==(
			Money value1,
			Money value2)
		{
			if (Object.ReferenceEquals(value1, value2)) { return true; }

			if (((object)value1 == null) || ((object)value2 == null)) { return false; }

			return (value1.Currency == value2.Currency) && (value1.Amount == value2.Amount);
		}


		public static bool operator !=(
			Money value1,
			Money value2)
		{
			return !(value1 == value2);
		}


		public static Money operator +(
			Money value1,
			Money value2)
		{
			EnsureCommonOperatorPreConditionsAreSatisfied(value1, value2);

			return new Money(value1.Currency, value1.Amount + value2.Amount);
		}


		public static Money operator -(
			Money value1,
			Money value2)
		{
			EnsureCommonOperatorPreConditionsAreSatisfied(value1, value2);
			ESInv.DBC.Ensure.That(value1.Amount >= value2.Amount, "value 1 must be >= value 2 amount");

			return new Money(value1.Currency, value1.Amount - value2.Amount);
		}


		public static bool operator >(
			Money value1,
			Money value2)
		{
			EnsureCommonOperatorPreConditionsAreSatisfied(value1, value2);

			return value1.Amount > value2.Amount;
		}


		public static bool operator >=(
			Money value1,
			Money value2)
		{
			EnsureCommonOperatorPreConditionsAreSatisfied(value1, value2);

			return value1.Amount >= value2.Amount;
		}


		public static bool operator <(
			Money value1,
			Money value2)
		{
			EnsureCommonOperatorPreConditionsAreSatisfied(value1, value2);

			return value1.Amount < value2.Amount;
		}


		public static bool operator <=(
			Money value1,
			Money value2)
		{
			EnsureCommonOperatorPreConditionsAreSatisfied(value1, value2);

			return value1.Amount <= value2.Amount;
		}


		public override bool Equals(
			Object other)
		{
			return this == (other as Money);
		}


		public override int GetHashCode()
		{
			return this.Currency.GetHashCode() ^ this.Amount.GetHashCode();
		}


		private static void EnsureCommonOperatorPreConditionsAreSatisfied(
			Money value1,
			Money value2)
		{
			ESInv.DBC.Ensure.That(value1 != null, "value 1 cannot be null");
			ESInv.DBC.Ensure.That(value2 != null, "value 2 cannot be null");
			ESInv.DBC.Ensure.That(value1.Currency == value2.Currency, "Currencies must be the same");
		}
	}
}
