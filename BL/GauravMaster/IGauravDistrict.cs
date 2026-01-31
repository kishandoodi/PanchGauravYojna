using MO.Common;
using MO.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.GauravMaster
{
    public interface IGauravDistrict
    {
        Task<result> Save(GauravDistrictMO model);
        Task<List<GauravDistrictMO>> GetList(int FyId);
        Task<result> Delete(string guid, int FyId);
        Task<result> ToggleActive(string guid, int FyId);
    }
}
