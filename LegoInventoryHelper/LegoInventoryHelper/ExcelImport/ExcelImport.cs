using ClosedXML.Excel;
using LegoInventoryHelper.Models;

namespace LegoInventoryHelper.ExcelImport
{
    public class ExcelImport
    {
        public static List<CreateInventoryItem> ExtractSetsFromExcel(string filename)
        {
            var workBook = new XLWorkbook(filename);
            var sheet = workBook.Worksheet("Tabelle1");
            var firstRowUsed = sheet.FirstRowUsed();
            var articelRow = firstRowUsed.RowUsed();
            articelRow = articelRow.RowBelow();
            var excelSets = new List<CreateInventoryItem>();

            while (!articelRow.Cell(1).IsEmpty())
            {
                var quantity = int.Parse(articelRow.Cell(4).GetString());
                for (int i = 0; i < quantity; i++)
                {
                    excelSets.Add(new CreateInventoryItem(articelRow.Cell(1).GetString(), double.Parse(articelRow.Cell(5).GetString())));
                }
                articelRow = articelRow.RowBelow();
            }
            return excelSets;
        }
    }
}
