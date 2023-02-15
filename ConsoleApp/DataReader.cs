using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    public class DataReader
    {
        IEnumerable<ImportedObject> ImportedObjects;

        public void ImportAndPrintData(string fileToImport)
        {
            ImportedObjects = new List<ImportedObject>() { 
                new ImportedObject() };

            var streamReader = new StreamReader(fileToImport);

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                var values = line.Split(';');
                // check if line is correct, must include 7 columns
                if (values.Count() == 7)
                {
                    if (ClearImportedData(values[0]) == null)
                    {
                        Console.WriteLine("Error");
                    }

                    var importedObject = new ImportedObject
                    {
                        Type = ClearImportedData(values[0]).ToUpper(),
                        Name = ClearImportedData(values[1]),
                        Schema = ClearImportedData(values[2]),
                        ParentName = ClearImportedData(values[3]),
                        ParentType = ClearImportedData(values[4]).ToUpper(),
                        DataType = ClearImportedData(values[5]).ToUpper(),
                        IsNullable = ClearImportedData(values[6])
                    };
                    ((List<ImportedObject>)ImportedObjects).Add(importedObject);
                }
            }

            // clear imported data - remove nullable rows and header
            ImportedObjects = ImportedObjects.Where(o => o.Type != null 
                && o.Type != "TYPE").ToList();

            // assign number of children
            for (int i = 0; i < ImportedObjects.Count(); i++)
            {
                var importedObject = ImportedObjects.ToArray()[i];
                foreach (var impObj in ImportedObjects)
                {
                    if (impObj.ParentType == importedObject.Type)
                    {
                        if (impObj.ParentName == importedObject.Name)
                        {
                            importedObject.NumberOfChildren = 1 
                                + importedObject.NumberOfChildren;
                        }
                    }
                }
            }

            foreach (var database in ImportedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' (" +
                        $"{database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in ImportedObjects)
                    {                        
                        if (table.ParentType == database.Type)
                        {
                            if (table.ParentName == database.Name)
                            {
                                Console.WriteLine($"\tTable '{table.Schema}." +
                                    $"{table.Name}' ({table.NumberOfChildren} " +
                                    $"columns)");

                                // print all table's columns
                                foreach (var column in ImportedObjects)
                                {
                                    if (column.ParentType == table.Type)
                                    {
                                        if (column.ParentName == table.Name)
                                        {
                                            Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clear imported string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ClearImportedData(string value)
        {
            value = value.Trim().Length != 0 ? value.
                Trim(new Char[] { ' ', '\t' }) : "";

            //value = value.Trim().Length != 0 ? value.Trim().Replace(" ", "").
            //    Replace(Environment.NewLine, "") : "";

            return value;
        }
    }

    class ImportedObject : ImportedObjectBaseClass
    {
        public string Schema { get; set; }

        public string ParentName { get; set; }
        
        public string ParentType { get; set; }

        public string DataType { get; set; }
        public string IsNullable { get; set; }

        public double NumberOfChildren { get; set; }
    }

    class ImportedObjectBaseClass
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
