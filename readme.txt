
Greg young
Lokad
http://msdn.microsoft.com/en-us/library/jj554200.aspx
http://channel9.msdn.com/Events/aspConf/aspConf/CQRS-with-ASP-NET-MVC-A-Year-On




Too many abstractions 
Skip repository and use event store directly ?
Merchant repository should be based on the event store also
Should i allow multiple key types ? Guid for order, int for merchant
How do i get the merchant from the event store unless the command includes the merchant Id
How\Where do i publish events so they can be subscribed to externally ? Bus will need to cater for internal and external events
Throwing exceptions on command failures - not recording attempts ? .i.e Commands are expected to succeed when they get to the handler



Payments must all be with the same currency
Payments can exceed the order creation value
Refunds must all be with the same currency as the initial payment


VS
--
Place updates in a readme in devstuff
Right red border
Tabs\Whitespace
Show whitespace
Solution\properties on left hand side always
Track file in solution explorer
Others ? - Read existing devstuff content


