using Azure.Core;
using MasjidConnect.Application;
using MasjidConnect.Application.Interfaces;
using MasjidConnect.Infranstructure.DbHelpers;
using MasjidConnect.Model.Request.Masjid;
using MasjidConnect.Model.Response;
using MasjidConnect.Model.Response.Masjid;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;
using System.Reflection;


namespace MasjidConnect.Infranstructure.Repositories
{
    public class MasjidRepository : IMasjidRepository
    {
        private readonly IDbExecuter _dbExecuter;
        public MasjidRepository(IDbExecuter dbExecuter) 
        {
            _dbExecuter = dbExecuter;
        }

        public async Task<ResponseHeader> RegisterMasjidAsync(MasjidRequest model)
        {
            ResponseHeader responseHeader = new ResponseHeader();
            try
            {                
                //validate model

                var parameters = new[]
                {
                    new SqlParameter("@MasjidId", model.Id),
                    new SqlParameter("@MasjidName", model.Name),
                    new SqlParameter("@OtherName", model.OtherName?? (object)DBNull.Value),
                    new SqlParameter("@Address", model.Address),
                    new SqlParameter("@City", model.City ?? (object)DBNull.Value),
                    new SqlParameter("@District", model.District),
                    new SqlParameter("@State", model.State),
                    //new SqlParameter("@CityId", model.City),
                    new SqlParameter("@EnteredBy", model.EnteredBy),
                    new SqlParameter("@xml_MasjidImages", model.ImagesXml),
                };

                DataTable result = await _dbExecuter.ExecuteProcedureDtAsync(ProcedureManager.InsertOrUpdateMasjid, parameters);
                if (result != null && result.Rows.Count > 0)
                {
                    string flag = result.Rows[0]["flag"].ToString().ToLower();
                    if (flag == "ok")
                        responseHeader.Status = Enums.StatusType.Success.GetDescription();
                    else if (flag == "exist")
                        responseHeader.Status = Enums.StatusType.Exist.GetDescription();
                    else
                        responseHeader.Status = Enums.StatusType.Fail.GetDescription();
                    responseHeader.Message = result.Rows[0]["msg"].ToString();
                }
                else
                {
                    responseHeader.Status = Enums.StatusType.Fail.GetDescription();
                    responseHeader.Message = "";
                }
                return responseHeader;
            }
            catch (Exception ex)
            {
                responseHeader.Status = Enums.StatusType.Fail.GetDescription();
                responseHeader.Code = ex.Message;
                responseHeader.Message = ex.StackTrace.ToString();
                return responseHeader;
            }
        }
        public async Task<MasjidRespose?> GetMasjidByIdAsync(int Id)
        {
            MasjidRespose masjidRespose = new MasjidRespose
            {
                ResponseHeader = new ResponseHeader(),
                MasjidList = new List<MasjidDto>()
            };
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Id", Id) };

                using (SqlDataReader dr = await _dbExecuter.ExecuteProcedureDrAsync(ProcedureManager.GetMasjid, parameters))
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            MasjidDto masjidDto = new MasjidDto
                            {
                                Id = Convert.ToInt32(dr["RecId"].ToString()),
                                Name = dr["MasjidName"].ToString(),
                                Address = dr["Address"].ToString(),
                                OtherName = dr["OtherName"].ToString(),
                                State = dr["State"].ToString(),
                                District = dr["District"].ToString(),
                                City = dr["City"].ToString(),
                                ContactPerson = dr["ContactPerson"].ToString(),
                                ContactNo = dr["ContactNo"].ToString()
                            };
                            masjidRespose.MasjidDto = masjidDto;
                        }
                        masjidRespose.ResponseHeader.Status = Enums.StatusType.Success.GetDescription();
                        masjidRespose.ResponseHeader.Code = "";
                        masjidRespose.ResponseHeader.Message = "";
                    }
                    else
                    {
                        masjidRespose.ResponseHeader.Status = Enums.StatusType.Fail.GetDescription();
                        masjidRespose.ResponseHeader.Code = Enums.ErrorType.NoRecord.ToString();
                        masjidRespose.ResponseHeader.Message = Enums.ErrorType.NoRecord.GetDescription();
                    }
                }
                ;
                return masjidRespose;
            }
            catch (Exception ex)
            {

                masjidRespose.ResponseHeader.Status = Enums.StatusType.Fail.GetDescription();
                masjidRespose.ResponseHeader.Code = ex.Message;
                masjidRespose.ResponseHeader.Message = ex.StackTrace;
                return masjidRespose;
            }
        }
        public async Task<MasjidRespose?> GetAllMasjidAsync()
        {
            MasjidRespose masjidRespose = new MasjidRespose
            {
                ResponseHeader = new ResponseHeader(),
                MasjidList = new List<MasjidDto>()
            };
            try
            {
                using (SqlDataReader dr = await _dbExecuter.ExecuteProcedureDrAsync(ProcedureManager.SpGetAllMasjid, null))
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            MasjidDto masjidDto = new MasjidDto
                            {
                                Id = Convert.ToInt32(dr["RecId"].ToString()),
                                Name = dr["MasjidName"].ToString(),
                                Address = dr["Address"].ToString(),
                                OtherName = dr["OtherName"].ToString(),
                                State = dr["State"].ToString(),
                                District = dr["District"].ToString(),
                                City = dr["City"].ToString(),
                                ContactPerson = dr["ContactPerson"].ToString(),
                                ContactNo = dr["ContactNo"].ToString()
                            };
                            masjidRespose.MasjidList.Add(masjidDto);
                        }
                        masjidRespose.ResponseHeader.Status = Enums.StatusType.Success.GetDescription();
                        masjidRespose.ResponseHeader.Code = "";
                        masjidRespose.ResponseHeader.Message = "";
                    }
                    else
                    {
                        masjidRespose.ResponseHeader.Status = Enums.StatusType.Fail.GetDescription();
                        masjidRespose.ResponseHeader.Code = Enums.ErrorType.NoRecord.ToString();
                        masjidRespose.ResponseHeader.Message = Enums.ErrorType.NoRecord.GetDescription();
                    }
                }
                ;
                return masjidRespose;
            }
            catch (Exception ex)
            {

                masjidRespose.ResponseHeader.Status = Enums.StatusType.Fail.GetDescription();
                masjidRespose.ResponseHeader.Code = ex.Message;
                masjidRespose.ResponseHeader.Message = ex.StackTrace;
                return masjidRespose;
            }
        }

        public async Task<ResponseHeader?> RegisterNamazTimeAsync(NamazTimeRequest model)
        {
            ResponseHeader responseHeader = new ResponseHeader();
            try
            {
                //validate model

                //var parameters = new[]
                //{
                //    new SqlParameter("@MasjidId", model.Id),
                //    new SqlParameter("@MasjidName", model.Name),
                //    new SqlParameter("@OtherName", model.OtherName?? (object)DBNull.Value),
                //    new SqlParameter("@Address", model.Address),
                //    new SqlParameter("@City", model.Email ?? (object)DBNull.Value),
                //    new SqlParameter("@District", model.District),
                //    new SqlParameter("@State", model.State),
                //    new SqlParameter("@City", model.City),
                //    new SqlParameter("@EnteredBy", model.EnteredBy),
                //    new SqlParameter("@xml_MasjidImages", model.ImagesXml),
                //};

                //DataTable result = await _dbExecuter.ExecuteProcedureDtAsync(ProcedureManager.InsertOrUpdateMasjid, parameters);
                //if (result != null && result.Rows.Count > 0 && result.Rows[0]["flag"].ToString().ToLower() == "ok")
                //{
                //    responseHeader.Status = "Success";
                //    responseHeader.Message = result.Rows[0]["msg"].ToString();
                //}
                //else
                //{
                //    responseHeader.Status = "Fail";
                //    responseHeader.Message = result.Rows[0]["msg"].ToString();
                //}
                return responseHeader;
            }
            catch (Exception ex)
            {
                responseHeader.Status = "Fail";
                responseHeader.Code = ex.Message;
                responseHeader.Message = ex.StackTrace.ToString();
                return responseHeader;
            }
        }

        public async Task<MasjidRespose?> GetNamazTimeByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
