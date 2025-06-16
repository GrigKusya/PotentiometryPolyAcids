using KarginScales.Models;
using System.Collections.Generic;
using ClosedXML.Excel;
using System;
using System.Linq;

namespace KarginScales.Service;

public class ExcelDataService : IDataService
{
    public LoadResult<List<Polymer>> LoadData(string path)
    {
        try
        {
            var result = new List<Polymer>();
            using XLWorkbook wb = new XLWorkbook(path);

            foreach (var ws in wb.Worksheets)
            {
                string polymerName = ws.Name + " кислота";
                var polymer = new Polymer(polymerName, GetListData(ws.RangeUsed()).ToList());
                result.Add(polymer);
            }

            return LoadResult<List<Polymer>>.Success(result);
        }
        catch(Exception ex)
        {
            return LoadResult<List<Polymer>>.Failure($"Ошибка при загрузке данных из Excel: {ex.Message}");
        }
    }

    private IEnumerable<DataPoint> GetListData(IXLRange? range)
    {
        for (int row = 3; row <= range?.RowCount(); ++row)
        {
            DataPoint point = new DataPoint
            {
                // Уберите округление здесь:
                Temperature = range.Cell(row, 4).GetDouble(), // Было Math.Round(..., 1)
                Gamma = range.Cell(row, 1).GetDouble()
            };
            yield return point;
        }
    }
}
