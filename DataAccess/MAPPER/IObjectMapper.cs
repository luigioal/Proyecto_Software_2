using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MAPPER
{
    public interface IObjectMapper
    {
        BaseClass MapObject(Dictionary<string, object> objectRow);
        List<BaseClass> MapObjectList(List<Dictionary<string, object>> objectList);
    }
}
