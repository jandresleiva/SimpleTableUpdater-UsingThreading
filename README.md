# SimpleTableUpdater-UsingThreading
This little tool was made for personal use, but serves as a minimalistic example of MySQL database connection using Parallel Tasks.

It's simple but efective.

There are a couple of configurations set in the file configurations.json which will be automaticly copied to the resulting directory due a build event.
    connectionString contains the connection string used to connect to the DB.
    qtyRows which will determine how many rows the process will affect.
    It also has a MaxDegreeOfParallelism which determines how many simultaneous tasks will try to run. Increasing its value will end in faster runs, but be aware that it can exceed the maximum connections accepted by the mysql server.

The file [data/DBConnection.cs](data/DBConnection.cs) has a connection string which should be modified to match your mysql user credentials.

And finally the [Updater.cs](Updater) file has the definition of the methods to operate over the DB. (Not static anymore)
    This method constructor takes a string argument with the connectionString. This will allow the use of multiple instances with different DBs in the future.
    UpdateCUILAsync(int from)
    IsolatePhonesAsync(int from)
    ChangeDNIForIdAsync(int from)
    TestReadDBAsync(int from) - This test method is ment to test the DB connection listing IDs of a table.

These methods are defined for my own database, but they have a couple of examples about how to interact with the DB. 
In the future I may also implement a easier interface to interact with the DB.

Updates
    2020/07/12 - Added JSON Configuration file

The file configurations.json defines three variables used to establish the database connection.