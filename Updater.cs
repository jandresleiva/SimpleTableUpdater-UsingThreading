using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks.Sources;
using Update_Clients_CUIL.Data;

namespace Update_Clients_CUIL
{
    class Updater
    {
        /// <summary>
        /// DoUpdateAsync is a static method responsible of the table transformation. It iterates through the table between the given boundaries 
        /// row by row applying any logic defined in its body. 
        /// </summary>
        /// <param name="from">An int representing a starting row number.</param>
        /// <param name="qty">An int representing the number of simultaneous rows (usually 1) to read.</param>
        /// <returns></returns>
        public static async Task UpdateCUILAsync(int from, int qty)
        {

            var dbCon = new DBConnection();
            if (dbCon.IsConnect())
            {
                var query = "SELECT * FROM digi_new.clientes LIMIT "+from+", "+qty;
                var cmd = new MySqlCommand(query, dbCon.Connection);
                var resultsReader = await cmd.ExecuteReaderAsync();
                while (await resultsReader.ReadAsync())
                {
                    var cuil = "";
                    var cliID = resultsReader.GetString(0);
                    var dni = resultsReader.GetString(5);

                    // This var holds a flag to show if the row was indeed modified.
                    var flagDone = 0;

                    // In this case, the column 68 (CUIL) is nullable, so I need to verify it's content before operationg.
                    if (!resultsReader.IsDBNull(68))
                    {
                        // If it is not null, I take the two comma separated values (20,6) and then make a new var with the composed string
                        var cuilDigits = resultsReader.GetString(68).Split(",");
                        cuil = cuilDigits[0] + dni + cuilDigits[1];
                        var dbCon2 = new DBConnection();
                        if (dbCon2.IsConnect())
                        {
                            // Runs the update for the given entry and then rise the flag for logging.
                            var query2 = "UPDATE digi_new.clientes SET cui = " + cuil + " WHERE id = " + cliID;
                            var cmd2 = new MySqlCommand(query2, dbCon2.Connection);
                            await cmd2.ExecuteReaderAsync();
                            flagDone = 1;
                            dbCon2.Close();
                        }
                        else
                        {
                            throw new Exception("Couldn't connect to database");
                        }
                    }

                    // The logging is now being made to the console, but could be a text file.
                    Console.WriteLine(cliID + ": --- " + dni + " ---");
                    if (flagDone == 1)
                        Console.WriteLine(cuil);
                        Console.WriteLine("---- DONE ----");
                }
                dbCon.Close();
            }
            else
            {
                throw new Exception("Couldn't connect to database");
            }
        }


        public static async Task IsolatePhonesAsync(int from, int qty)
        {

            var dbCon = new DBConnection();
            if (dbCon.IsConnect())
            {
                var query = "SELECT * FROM digi_new.clientes LIMIT " + from + ", " + qty;
                var cmd = new MySqlCommand(query, dbCon.Connection);
                var resultsReader = await cmd.ExecuteReaderAsync();
                while (await resultsReader.ReadAsync())
                {
                    var cliID = resultsReader.GetString(0);

                    var telefonos = new List<string>();

                    if (!resultsReader.IsDBNull(9) && !resultsReader.IsDBNull(10))
                    {
                        var ca_telefono = new string(resultsReader.GetString(9).Where(c => char.IsDigit(c)).ToArray());
                        var telefono = new string(resultsReader.GetString(10).Where(c => char.IsDigit(c)).ToArray());
                        var tel_description = "telefono";
                        if (ca_telefono != "" && telefono != "")
                            telefonos.Add("(" + cliID + ", " + ca_telefono + ", " + telefono + ", '" + tel_description + "')");
                    }
                    /*
                    for (int i = 1; i <= 6; i=i+2)
                    {
                        if (!resultsReader.IsDBNull(9 + i) && !resultsReader.IsDBNull(10 + i))
                        {
                            var ca_telefono = new string(resultsReader.GetString(9+i).Where(c => char.IsDigit(c)).ToArray());
                            var telefono = new string(resultsReader.GetString(10+i).Where(c => char.IsDigit(c)).ToArray());
                            var tel_description = "telefono"+(i-1);
                            if (telefono != "")
                                telefonos.Add("(" + cliID + ", " + ca_telefono + ", " + telefono + ", '" + tel_description + "')");
                        }
                    }
                    */
                    if (!resultsReader.IsDBNull(33) && !resultsReader.IsDBNull(34))
                    {
                        var ca_celular = new string(resultsReader.GetString(33).Where(c => char.IsDigit(c)).ToArray());
                        var celular = new string(resultsReader.GetString(34).Where(c => char.IsDigit(c)).ToArray());
                        var cel_description = "celular";
                        if(ca_celular != "" && celular != "")
                            telefonos.Add("(" + cliID + ", " + ca_celular + ", " + celular + ", '" + cel_description + "')");
                    }

                    // This var holds a flag to show if the row was indeed modified.
                    var flagDone = 0;

                    if (telefonos.Count > 0) {
                        var dbCon2 = new DBConnection();
                        if (dbCon2.IsConnect())
                        {
                            // Runs the update for the given entry and then rise the flag for logging.
                            var valuesToInsert = string.Join(", ", telefonos);
                            var query2 = "INSERT digi_new.phones (client, area_code, number, description) VALUES " + valuesToInsert;
                            var cmd2 = new MySqlCommand(query2, dbCon2.Connection);
                            await cmd2.ExecuteReaderAsync();
                            flagDone = 1;
                            dbCon2.Close();
                        }
                        else
                        {
                            throw new Exception("Couldn't connect to database");
                        }
                    }

                    // The logging is now being made to the console, but could be a text file.
                    Console.WriteLine(cliID);
                    if (flagDone == 1)
                        Console.WriteLine("---- DONE ----");
                    else
                        Console.WriteLine("---- FAIL ----");
                }
                dbCon.Close();
            }
            else
            {
                throw new Exception("Couldn't connect to database");
            }
        }

        public static async Task ChangeDNIForIdAsync(int from, int qty)
        {

            var dbCon = new DBConnection();
            if (dbCon.IsConnect())
            {
                var query = "SELECT clientes_info.id as info_id, clientes.id as client_id FROM digi_new.clientes_info LEFT JOIN digi_new.clientes ON digi_new.clientes.dni = digi_new.clientes_info.cliente LIMIT " + from + ", " + qty;
                var cmd = new MySqlCommand(query, dbCon.Connection);
                var resultsReader = await cmd.ExecuteReaderAsync();
                while (await resultsReader.ReadAsync())
                {
                    var infoID = resultsReader.GetString(0);
                    var cliID = resultsReader.GetString(1);

                    var dbCon2 = new DBConnection();
                    if (dbCon2.IsConnect())
                    {
                        // Runs the update for the given entry and then rise the flag for logging.
                        var query2 = "UPDATE digi_new.clientes_info SET cliente = " + cliID + " WHERE clientes_info.id = "+infoID;
                        var cmd2 = new MySqlCommand(query2, dbCon2.Connection);
                        await cmd2.ExecuteReaderAsync();
                        dbCon2.Close();
                    }
                    else
                    {
                        throw new Exception("Couldn't connect to database");
                    }
                    // The logging is now being made to the console, but could be a text file.
                    Console.WriteLine(cliID);
                }
                dbCon.Close();
            }
            else
            {
                throw new Exception("Couldn't connect to database");
            }
        }
    }
}
