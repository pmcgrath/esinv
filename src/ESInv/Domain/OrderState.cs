using System;
using System.Collections.Generic;
using System.Linq;


namespace ESInv.Domain
{
	public class OrderState
	{
		private IList<OrderEntry> c_entries;
		

		public Guid Id { get; private set; }
		public int MerchantId { get; private set; }
		public Money SaleValue { get; private set; }
		public IEnumerable<PaymentOffer> Offers { get; private set; }
		public DateTimeOffset CreationTimestamp { get; private set; }


		public IEnumerable<OrderEntry> Entries { get { return this.c_entries; } }
		public bool PaymentsHaveBeenMade { get { return this.DetermineIfPaymentsHaveBeenMade(); } }
		public string ExistingPaymentsCurrency { get { return this.DetermineExistingPaymentsCurrency(); } }
		public Money NetPayments { get { return this.DetermineNetPayments(); } }


		public OrderState(
			IEnumerable<ESInv.Messaging.IEvent> events)
		{
			foreach (var @event in events) { this.Mutate(@event); }
		}

	
		public void Mutate(
			ESInv.Messaging.IEvent @event)
		{
			((dynamic)this).When((dynamic)@event);
		}


		public bool IsAnOfferCurrency(
			string currency)
		{
			return this.Offers.Any(offer => offer.PaymentValue.Currency == currency);
		}


		private void When(
			ESInv.Messages.OrderCreated @event)
		{
			ESInv.DBC.Ensure.That(this.Id == Guid.Empty, "Order has already been created");

			this.Id = @event.Id;
			this.MerchantId = @event.MerchantId;
			this.SaleValue = @event.SaleValue.FromMessage();
			this.Offers = @event.Offers.Select(offer => offer.FromMessage());
			this.CreationTimestamp = @event.Timestamp;

			this.c_entries = new List<OrderEntry>();
		}


		private void When(
			ESInv.Messages.OrderPaymentMade @event)
		{
			ESInv.DBC.Ensure.That(this.Id != Guid.Empty, "Order has NOT already been created");

			this.c_entries.Add(
				new OrderEntry(
					OrderEntryType.Debit,
					@event.Value.FromMessage(),
					DateTimeOffset.Now));
		}


		private void When(
			ESInv.Messages.OrderRefundMade @event)
		{
			ESInv.DBC.Ensure.That(this.Id != Guid.Empty, "Order has NOT already been created");

			this.c_entries.Add(
				new OrderEntry(
					OrderEntryType.Credit,
					@event.Value.FromMessage(),
					DateTimeOffset.Now));
		}


		private bool DetermineIfPaymentsHaveBeenMade()
		{
			return this.c_entries.Any(entry => entry.Type == OrderEntryType.Debit);
		}


		private string DetermineExistingPaymentsCurrency()
		{
			if (!this.c_entries.Any(entry => entry.Type == OrderEntryType.Debit)) { return null; }

			return this.c_entries.First(entry => entry.Type == OrderEntryType.Debit).Value.Currency;
		}


		private Money DetermineNetPayments()
		{
			var _accumulatorSeed = new Money(this.Entries.First().Value.Currency, 0M);

			return this.c_entries
				.Aggregate(
					_accumulatorSeed,
					(accumulatedValue, entry) => 
						{
							if (entry.Type == OrderEntryType.Debit) { return accumulatedValue + entry.Value; }
							return accumulatedValue - entry.Value;
						});
		}
	}
}
