namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DataReader
    {
        private List<ImportedObject> DatabaseObject;

        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            DatabaseObject = new List<ImportedObject>(); // Initialize as an empty list

            var importedLines = new List<string>();

            /*
             Resource handling: The StreamReader instance is not being disposed of properly. It should be enclosed in a using statement to ensure that it is disposed of correctly.
             */
            using (var streamReader = new StreamReader(fileToImport))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    importedLines.Add(line);
                }
            }

            for (int i = 0; i < importedLines.Count; i++) // Change condition to use less than, since the last index is Count - 1
            {
                //Some Lines are empty on the file so we handle exceptions to continue on the next one
                try
                {
                    var importedLine = importedLines[i];
                    var values = importedLine.Split(';');
                    var importedObject = new ImportedObject();
                    importedObject.Type = values[0];
                    importedObject.Name = values[1];
                    importedObject.Schema = values[2];
                    importedObject.ParentName = values[3];
                    importedObject.ParentType = values[4];
                    importedObject.DataType = values[5];
                    importedObject.IsNullable = values[6] == "1" ? true : false;
                    DatabaseObject.Add(importedObject); // Add the importedObject directly to the list, no need to cast to List<ImportedObject>
                }
                catch
                {
                    continue;
                }
            }

            // clear and correct imported data
            //String processing: The string processing code that clears and corrects imported data can be simplified by using string.Replace() with multiple arguments instead of chaining multiple calls to string.Replace().For example, importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper(); can be simplified to importedObject.Type = importedObject.Type.Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
            foreach (var importedObject in DatabaseObject)
            {
                importedObject.Type = importedObject.Type.Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                importedObject.Name = importedObject.Name.Replace(" ", "").Replace(Environment.NewLine, "").Trim();
                importedObject.Schema = importedObject.Schema.Replace(" ", "").Replace(Environment.NewLine, "").Trim();
                importedObject.ParentName = importedObject.ParentName.Replace(" ", "").Replace(Environment.NewLine, "").Trim();
                importedObject.ParentType = importedObject.ParentType.Replace(" ", "").Replace(Environment.NewLine, "").Trim();
            }

            // Assign number of children using LINQ
            DatabaseObject.ForEach(importedObject => importedObject.NumberOfChildren = DatabaseObject.Count(impObj => impObj.ParentType == importedObject.Type && impObj.ParentName == importedObject.Name));

            // Print all databases, tables and columns using LINQ
            var databases = DatabaseObject.Where(database => database.Type == "DATABASE");

            foreach (var database in databases)
            {
                var tables = DatabaseObject.Where(table => table.ParentType.ToUpper() == database.Type && table.ParentName == database.Name);

                Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                foreach (var table in tables)
                {
                    var columns = DatabaseObject.Where(column => column.ParentType.ToUpper() == table.Type && column.ParentName == table.Name);

                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                    foreach (var column in columns)
                    {
                        Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable ? "accepts nulls" : "with no nulls")}");
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
