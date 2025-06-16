using KarginScales.Models;
using System.Collections.Generic;

namespace KarginScales.Service;

public interface IDataService
{
    LoadResult<List<Polymer>> LoadData(string path);
}
