In order for the UTs to pass we obviously need an instance of an EventStore node running - specifically a local single node running on the default port
Can run as a non admin if you do not need the http server
If you do need the http server, which you will for testing and troubleshooting run the following as an admin
	lib\eventstore\EventStore.SingleNode.exe

To access the web app using default configuration - can get this from the standard out from the above process
	http://127.0.0.1:2113/   
