using MasjidConnect.Model.Request.Masjid;
using MasjidConnect.Model.Request.User;
using MasjidConnect.Model.Response;
using MasjidConnect.Model.Response.Masjid;
using MasjidConnect.Model.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasjidConnect.Application.Interfaces
{
    public interface IMasjidRepository
    {
        Task<ResponseHeader> RegisterMasjidAsync(MasjidRequest model);
        Task<MasjidRespose?> GetMasjidByIdAsync(int Id);
        Task<MasjidRespose?> GetAllMasjidAsync();
        Task<ResponseHeader> RegisterNamazTimeAsync(NamazTimeRequest model);
        Task<MasjidRespose?> GetNamazTimeByIdAsync(int Id);
    }
}
