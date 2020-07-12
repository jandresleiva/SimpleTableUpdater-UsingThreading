# SimpleTableUpdater-UsingThreading
This little tool was made for personal use, but serves as a minimalistic example of MySQL database connection using Parallel Tasks.

It's simple but efective.

## Configurations
There are a couple of configurations set in the file [configurations.json](configurations.json) which will be automaticly copied to the resulting directory due a build event.

* **connectionString** contains the connection string used to connect to the DB.
* **qtyRows** which will determine how many rows the process will affect.
* **maxParallelism** which determines how many simultaneous tasks will try to run. Increasing its value will end in faster runs, but be aware that it can exceed the maximum connections accepted by the mysql server.

## Connection
The file [data/DBConnection.cs](data/DBConnection.cs) has the connection logic. An DBConnection object must be instantiated with the propper connectionString in order to connect to the DB. It's usually done in the Updater.cs file:

```var dbCon = new DBConnection(connectionString);```

## Operations
The [Updater.cs](Updater) file has the definition of the methods to operate over the DB. (It's not static anymore)
An updater object must be instantiated with the connectionString as an argument. (This can be read from the configurations or defined by a fixed string)

```Updater updater = new Updater(configurations.connectionString);```

**Note:** You need to remember that this will execute the method in parallel processes, so be aware of locking the table or writing in the same table as you read. It's not imposible, but it may endup with unexpected behaviours since the order of execution is not linear.

### Methods
* This method constructor takes a string argument with the connectionString. This will allow the use of multiple instances with different DBs in the future.
* UpdateCUILAsync(int from)
* IsolatePhonesAsync(int from)
* ChangeDNIForIdAsync(int from)
* TestReadDBAsync(int from) - This test method is ment to test the DB connection listing IDs of a table.

### Example method
A new method can be created but must follow some basic guidelines
* It must receive an int, for the Parallel execution to define which line of the table it will read.
* It must return a Task.
* You would probably need a new DBConnection to operate (remember to close it).
* You will need to ReadAsync and operate through the results.

```
        public async Task TestReadDBAsync(int from)
        {
            var dbCon = new DBConnection(connectionString);
            if (dbCon.IsConnect())
            {
                var query = "SELECT * FROM digi_new.clientes LIMIT " + from + ", 1";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                var resultsReader = await cmd.ExecuteReaderAsync();
                while (await resultsReader.ReadAsync())
                {
                    Console.WriteLine("ID: " + resultsReader.GetString(0));
                }
                dbCon.Close();
            }
        }
```

**Note:** These methods are defined for my own database, but they have a couple of examples about how to interact with the DB. 
In the future I may also implement a easier interface to interact with the DB.

## Updates
    2020/07/12 - Added JSON Configuration file

The file configurations.json defines three variables used to establish the database connection.
