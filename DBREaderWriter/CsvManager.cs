using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Gravity.DBReaderWriter;


namespace Gravity.DBReaderWriter
{
    public class CsvManager
    {
        private CsvReader csvReader;
        private StreamReader streamReader;
        private List<NodeModel> supplierRecords;

        private string nodePrefix;
        private string spName;
        private string inputFilePath;
        private string outputFilePath;

        private const int NodeType = 2; //Supplier Type

        Regex regexSpecialChars = new Regex("[^a-zA-Z0-9 -]");



        public CsvManager(string nodePrefix, string spName, string inputFilePath, string outputFilePath)
        {
            this.nodePrefix = nodePrefix;
            this.spName = spName;
            this.inputFilePath = inputFilePath;
            this.outputFilePath = outputFilePath;
        }

        public void Read()
        {
            streamReader = new StreamReader(inputFilePath);
            csvReader = new CsvReader(streamReader);
            csvReader.Configuration.IsHeaderCaseSensitive = false;
            csvReader.Configuration.IgnoreHeaderWhiteSpace = true;

            supplierRecords = csvReader.GetRecords<NodeModel>().ToList();
        }

        public void GenerateScript()
        {
            using (StreamWriter writer =
                new StreamWriter(outputFilePath))
            {
                foreach (var supplierDetail in supplierRecords)
                {
                    var code = GenerateNodeCode(supplierDetail.Supplier);
                    if (String.IsNullOrWhiteSpace(code))
                    {
                        Console.WriteLine("Code could not be generated for Supplier " + supplierDetail.Supplier + ". Skipping this supplier.");
                        continue;
                    }

                    string query = "exec " +
                                   spName + " " +
                                   "'" + code + "'," +
                                   "'" + supplierDetail.Supplier.Replace("'", "''") + "'," +
                                   "'" + supplierDetail.Supplier.Replace("'", "''") + "'," +
                                   NodeType + "," +
                                   "'" + supplierDetail.Address.Replace("'", "''") + "'," +
                                   "'" + supplierDetail.Country.Replace("'", "''") + "'," +
                                   "'" + supplierDetail.SupplierCtcEmail + "'," +
                                   "'" + supplierDetail.Phone + "'," +
                                   "'" + supplierDetail.SupplierCtcName.Replace("'", "''") + "';";
                    writer.WriteLine(query);
                }
            }

        }

        private string GenerateNodeCode(string supplier)
        {
            var code = string.Empty;

            var supplierNameParts = supplier.Split(' ');

            if (!supplierNameParts.Any()) return code;

            if (supplierNameParts.Length == 1)
            {
                code = nodePrefix + supplierNameParts[0].ToUpper();
            }
            else if (supplierNameParts.Length == 2)
            {
                code = nodePrefix + supplierNameParts[0].ToUpper()+ supplierNameParts[1].ToUpper();
            }
            else
            {
                code = nodePrefix + supplierNameParts[0].ToUpper() +
                       supplierNameParts[1].ToUpper() +
                       supplierNameParts[2].ToUpper();
            }

            if (code.Length > 45)
            {
                code = code.Substring(0, 44);
            }

            code = regexSpecialChars.Replace(code, "");
            return code;
        }
    }
}
