using MySql.Data.MySqlClient;
using System;
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
            // This var holds the max number of rows in the table to update.
            var qtyRows = 71179;
            //var qtyRows = 100;

            ParallelOptions po = new ParallelOptions();
            // This property determines the maximum number of parallel tasks
            // WARNING: I noted that with a degree over 20 the mysql server gets unresponsive for my specific build. Higher values will usually result in faster results.
            po.MaxDegreeOfParallelism = 10;
            Parallel.For(0, (int)qtyRows, po, async (j) => {
                await Updater.ChangeDNIForIdAsync(j, 1);
            });

        }
    }
}
