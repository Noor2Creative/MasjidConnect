using Azure;
using MasjidConnect.Application;
using MasjidConnect.Application.Interfaces;
using MasjidConnect.Model.Request.Masjid;
using MasjidConnect.Model.Response;
using MasjidConnect.Model.Response.Masjid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MasjidConnectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasjidController : BaseApiController
    {
        private readonly IMasjidRepository _masjidRepository;
        public MasjidController(IMasjidRepository masjidRepository, IConfiguration config) : base(config)
        {
            _masjidRepository = masjidRepository;
        }

        [Authorize]
        [HttpPost("register_masjid")]
        public async Task<IActionResult> RegisterMasjidAsync([FromForm] MasjidRequest model, [FromHeader(Name ="Authorization")] string authorization)
        {
            ResponseHeader response = new ResponseHeader();
            try
            {
                
                // Call base method to validate token
                var (isValid, responseHeader, claims) = ValidateTokeAndGetClaims(authorization, "sub", "role");

                if (!isValid)
                {
                    return Unauthorized(responseHeader); // send response header with error
                }

                #region File upload
                if (model.MasjidImage != null && model.MasjidImage.Any())
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\Masjid");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var sb = new StringBuilder();
                    sb.Append("<Document>");

                    foreach (var file in model.MasjidImage)
                    {
                        var ext = Path.GetExtension(file.FileName).ToLower();
                        if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp")
                        {
                            response.Status = Enums.StatusType.Fail.GetDescription();
                            response.Code = Enums.ErrorType.InvalidFileFormat.GetDescription().Split(':')[0];
                            response.Message = $"{ext} {Enums.ErrorType.InvalidFileFormat.GetDescription().Split(':')[1]}";
                            return BadRequest(response);
                        }

                        var fileName = $"MasjidConnect_{DateTime.Now.ToString("ddMMyyHHmmssfff")}{ext}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // You can save file URL to DB here
                        var fileUrl = $"wwwroot/Images/Masjid";
                        

                        sb.Append($"<Images><Image>{fileName}</Image><ImagePath>{fileUrl}</ImagePath></Images>");
                    }
                    sb.Append("</Document>");
                    model.ImagesXml = sb.ToString();
                }
                #endregion

                string userId = claims.ContainsKey("sub") ? claims["sub"] : null;
                string role = claims.ContainsKey("role") ? claims["role"] : null;
                
                model.EnteredBy = Convert.ToInt32(userId);

                response = await _masjidRepository.RegisterMasjidAsync(model);
                if (response.Status.ToUpper() != "OK")
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = Enums.StatusType.Fail.GetDescription();
                response.Code = ex.Message;
                response.Message = ex.StackTrace;
                return StatusCode(500, response);
            }            
        }


        
        [HttpPost("GetAllMasjid")]
        public async Task<IActionResult> GetAllMasjidAsync()
        {
            MasjidRespose masjidRespose = new MasjidRespose
            {
                ResponseHeader = new ResponseHeader(),
                MasjidList = new List<MasjidDto>()
            };
            try
            {
                masjidRespose = await _masjidRepository.GetAllMasjidAsync();
                if (masjidRespose.ResponseHeader.Status.ToUpper() == "OK")
                    return Ok(masjidRespose);

                return BadRequest(masjidRespose);
            }
            catch (Exception ex)
            {
                masjidRespose.ResponseHeader.Status = Enums.StatusType.Fail.GetDescription();
                masjidRespose.ResponseHeader.Code = ex.Message;
                masjidRespose.ResponseHeader.Message = ex.StackTrace;
                return StatusCode(500, masjidRespose);
            }
        }

        [HttpPost("GetMasjidById")]
        public async Task<IActionResult> GetMasjidByIdAsync(int Id)
        {
            MasjidRespose masjidRespose = new MasjidRespose
            {
                ResponseHeader = new ResponseHeader(),
                MasjidDto = new MasjidDto()
            };
            try
            {
                masjidRespose = await _masjidRepository.GetMasjidByIdAsync(Id);
                if (masjidRespose.ResponseHeader.Status.ToUpper() == "OK")
                    return Ok(masjidRespose);

                return BadRequest(masjidRespose);
            }
            catch (Exception ex)
            {
                masjidRespose.ResponseHeader.Status = Enums.StatusType.Fail.GetDescription();
                masjidRespose.ResponseHeader.Code = ex.Message;
                masjidRespose.ResponseHeader.Message = ex.StackTrace;
                return StatusCode(500, masjidRespose);
            }
        }

        [Authorize]
        [HttpPost("RegisterNamazTime")]
        public async Task<IActionResult> RegisterNamazTimeAsync(NamazTimeRequest model)
        {
            ResponseHeader response = new ResponseHeader();
            try
            {
                response = await _masjidRepository.RegisterNamazTimeAsync(model);
                if (response.Status.ToUpper() != "OK")
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = Enums.StatusType.Fail.GetDescription();
                response.Code = ex.Message;
                response.Message = ex.StackTrace;
                return StatusCode(500, response);
            }
        }



        [HttpPost("test_api")]
        public async Task<IActionResult> Test_api()
        {
            return Ok("Hello world!");
        }
    }
}
