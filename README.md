esinv
=====
Event souring investigation  
This is a mickey mouse sample just copies content from the following  

https://github.com/gregoryyoung/m-r  
http://beingtheworst.com/  
https://github.com/beingtheworst/btw-samples  
Lokad  
http://msdn.microsoft.com/en-us/library/jj554200.aspx  
http://channel9.msdn.com/Events/aspConf/aspConf/CQRS-with-ASP-NET-MVC-A-Year-On  


Running UTs
===========
Need to run a server see test\esinv.tests\readme.txt


Issues
======
Too many abstractions  
Skip repository and use event store directly ?  
Merchant repository should be based on the event store also  
Should i allow multiple key types ? Guid for order, int for merchant  
How do i get the merchant from the event store unless the command includes the merchant Id  
How\Where do i publish events so they can be subscribed to externally ? Bus will need to cater for internal and external events  
Throwing exceptions on command failures - not recording attempts ? .i.e Commands are expected to succeed when they get to the handler  


Domain constraints
==================
Payments must all be with the same currency  
Payments can exceed the order creation value  
Refunds must all be with the same currency as the initial payment  
