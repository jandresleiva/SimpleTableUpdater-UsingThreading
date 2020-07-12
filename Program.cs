using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Update_Clients_CUIL.Data;

namespace Update_Clients_CUIL
{
    class Program
    {
        // This program iterates through a table and updates values. The Updater class has a static member DoUpdateAsync
        // responsible of the transformation.
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            // This configurations.json has been added to the final folder through a build event macro
            string file = File.ReadAllText("configurations.json");
            Console.Write(file);
            Configurations configurations = JsonConvert.DeserializeObject<Configurations>(file);
            // This var holds the max number of rows in the table to update.
            var qtyRows = configurations.qtyRows;

            ParallelOptions po = new ParallelOptions();
            
            // Instantiates the updater object we will use to transform the data, passing the connection string gathered from the json file.
            Updater updater = new Updater(configurations.connectionString);

            // This property determines the maximum number of parallel tasks
            // WARNING: I noted that with a degree over 20 the mysql server gets unresponsive for my specific build. Higher values will usually result in faster results.
            po.MaxDegreeOfParallelism = configurations.maxParallelism;
            Parallel.For(0, (int)qtyRows, po, async (j) => {
                await updater.TestReadDBAsync(j);
            });

        }
    }
}
