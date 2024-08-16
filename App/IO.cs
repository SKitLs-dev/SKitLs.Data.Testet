using Newtonsoft.Json;
using OfficeOpenXml;
using SKitLs.Data.Core.IO.Excel;
using SKitLs.Data.IO;
using SKitLs.Data.IO.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester.App
{
    public static class IO
    {
        public static void Run()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var path = "test/io_test.json";

            try
            {
                //var writer = new PersonExcelWriter(path, "Persons")
                //{
                //    HandleInnerExceptions = false,
                //    CreateNewList = true,
                //};
                var writer = new JsonWriter<Person, int>(path)
                {
                    CreateNewFile = true,
                };

                var persons = new List<Person>
                {
                    new Person(1, "John Doe", 30, "john.doe@example.com"),
                    new Person(2, "Jane Smith", 25, "jane.smith@example.com")
                };

                if (!writer.WriteDataList(persons))
                {
                    // Raised if HandleInnerExceptions = true
                    Console.WriteLine("Error on writing data.");
                }
            }
            catch (Exception e)
            {
                // Raised if HandleInnerExceptions = false / default
                Console.WriteLine(e.Message);
                return;
            }

            try
            {
                //var reader = new PersonExcelReader(path, "Persons");
                var reader = new JsonReader<Person>(path);
                var persons = reader.ReadData<Person>();
                Console.WriteLine(string.Join(',', persons));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class Person : ModelDso<int>
    {
        public int Id { get; set; }
        public override int GetId() => Id;
        public override void SetId(int id) => Id = id;

        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }

        public Person(int id, string name, int age, string email)
        {
            Id = id;
            Name = name;
            Age = age;
            Email = email;
        }

        public override string ToString() => $"{Name} {Age} {Email}";
    }

    public class PersonExcelWriter : ExcelWriterBase<Person>
    {
        public PersonExcelWriter(string dataPath, string worksheetName, int startRow = 1, int startColumn = 1)
            : base(dataPath, worksheetName, startRow, startColumn)
        { }

        // Implementing the Convert method
        public override ExcelPartRow Convert(Person data)
        {
            var row = new ExcelPartRow(data.Id, StartColumn, StartColumn + 2);
            row.Add(data.Name);
            row.Add(data.Age.ToString());
            row.Add(data.Email);
            return row;
        }
    }

    public class PersonExcelReader : ExcelReaderBase<Person>
    {
        public PersonExcelReader(string dataPath, string worksheetName, int startRow = 1, int startColumn = 1, int endColumn = 3, int emptyRowsBreakHit = 3)
            : base(dataPath, worksheetName, startRow, startColumn, endColumn, emptyRowsBreakHit)
        { }

        // Implementing the Convert method
        public override Person Convert(ExcelPartRow row)
        {
            string name = row[0];
            int age = int.Parse(row[1]);
            string email = row[2];
            return new Person(row.RowIndex, name, age, email);
        }
    }
}