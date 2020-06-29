# SimpleTableUpdater-UsingThreading
This little tool was made for personal use, but serves as a minimalistic example of MySQL database connection using Parallel Tasks.

It's simple but efective.
The base file "Program.cs" has a couple of vars such as qtyRows which will determine how many rows the process will affect.
It also has a MaxDegreeOfParallelism which determines how many simultaneous tasks will try to run. Increasing its value will end in faster runs, but be aware that it can exceed the maximum connections accepted by the mysql server.

The file data/DBConnectio.cs has a connection string which should be modified to match your mysql user credentials.

And finally the Updater.cs file has the definition of the static method DoUpdateAsync(int from, int qty) which iterates through the table, and between those boundaries, to apply the transformation defined in it's body.

It's been considered for later updates to create a configuration file to hold all the variables and connection string. 
It's been considered adding some standard transformations for later use. 
