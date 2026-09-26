using Asp.Versioning;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AuthUtils;
using Entities.Auth;
using Entities.ODNS.Request;
using Entities.ODNS.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Net.Http.Headers;
using ODNSBusiness;
using ODNSAPI.Downloads;

namespace ODNSAPI.Controllers.ODNSControllers.V2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    //[Route("[controller]/[action]")]
    [ApiVersion(2.0)]
    public class ODNSQueryController : ControllerBase
    {
        private readonly ILogger<ODNSQueryController> _logger;
        private IBusinessOdns _businessOdns;
        private readonly LatestDownloadProvider _downloadProvider;
        public ODNSQueryController(
            ILogger<ODNSQueryController> logger,
            IBusinessOdns businessOdns,
            LatestDownloadProvider downloadProvider)
        {
            _logger = logger;
            _businessOdns = businessOdns;
            _downloadProvider = downloadProvider;
        }
        /// <summary>
        /// Endpoint used to retrieve the DNS entries from the ODNS project
        /// </summary>
        /// <remarks>
        /// All the request body parameters are optional except for pagination.
        /// 
        /// You can filter by providing the filter object and the search condition per property.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///        "pagination": {
        ///          "page": 1,
        ///          "per_page": 10
        ///        },
        ///        "filter": {
        ///          "protocol": "tcp",
        ///          "backend_resolver_country": "USA"
        ///        },
        ///          "sort": {
        ///            "field": "timestamp_request",
        ///            "order": "desc"
        ///          }
        ///     }
        /// 
        /// Note: timestamps are of the following format yyyy-MM-dd hh:mm:ss.ms
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        [EnableRateLimiting("fixed")]
        [HttpPost]
        [ApiKeyAuth]
        public async Task<GetDnsEntriesResponse> GetDnsEntries(GetDnsEntriesRequestV2 request)
        {
            string forwardedForIp = Request.Headers.ContainsKey("X-Forwarded-For")? Request.Headers["X-Forwarded-For"].ToString() : Request.HttpContext.Connection.RemoteIpAddress.ToString();
            
            //List<string> forwardedForIps = new List<string> { "54.93.128.255" , "13.36.21.255", "3.212.50.255" };
            //forwardedForIp = forwardedForIps[new Random().Next(forwardedForIps.Count)];
            
            GetDnsEntriesResponse response = await _businessOdns.GetDnsEntries(request, forwardedForIp);
            return response;
            //return await _businessOdns.GetDnsEntries(request);
        }

        [EnableRateLimiting("fixed")]
        [HttpPost]
        public async Task<StatusCode> RequestApiKey(ApiKeyRecordIn request)
        {
            StatusCode response = await _businessOdns.RequestApiKey(request);
            return response;
        }

        /// <summary>
        /// Download the latest complete TCP or UDP scan.
        /// </summary>
        /// <param name="protocol">tcp or udp</param>
        /// <param name="format">csv, csv.zst, or parquet</param>
        /// <param name="cancellationToken"></param>
        [EnableRateLimiting("fixed")]
        [HttpGet]
        [ApiKeyAuth]
        [Produces("text/csv", "application/zstd", "application/vnd.apache.parquet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> DownloadLatest(
            [FromQuery, Required] string protocol,
            [FromQuery, Required] string format,
            CancellationToken cancellationToken)
        {
            if (!_downloadProvider.IsSupportedProtocol(protocol))
                return BadRequest("protocol must be tcp or udp");
            if (!_downloadProvider.IsSupportedFormat(format))
                return BadRequest("format must be csv, csv.zst, or parquet");

            try
            {
                DownloadFileDescriptor? file = await _downloadProvider.GetLatest(
                    protocol,
                    format,
                    cancellationToken
                );
                if (file == null)
                    return NotFound("No download is available for the requested dataset");

                Response.ContentType = file.ContentType;
                Response.Headers[HeaderNames.ContentDisposition] =
                    $"attachment; filename=\"{file.Name}\"";
                Response.Headers["X-Accel-Redirect"] = _downloadProvider.GetInternalPath(file);
                return new EmptyResult();
            }
            catch (FileNotFoundException)
            {
                return NotFound("No download has been published yet");
            }
            catch (DirectoryNotFoundException)
            {
                return NotFound("No download has been published yet");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "The download manifest is invalid");
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Downloads are temporarily unavailable"
                );
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "The download manifest could not be read");
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Downloads are temporarily unavailable"
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "The download manifest could not be accessed");
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    "Downloads are temporarily unavailable"
                );
            }
        }

        /// <summary>
        /// Endpoint used to retrieve the latest DNS entries from the ODNS project
        /// </summary>
        /// <remarks>
        /// All the request body parameters are optional.
        /// 
        /// You can filter by providing the filter object and the search condition per property.
        /// 
        /// Sample request:
        /// 
        ///     {
        ///        "pagination": {
        ///          "page": 1,
        ///          "per_page": 10
        ///        },
        ///        "filter": {
        ///          "protocol": "tcp",
        ///          "backend_resolver_country": "USA"
        ///        },
        ///          "sort": {
        ///            "field": "timestamp_request",
        ///            "order": "desc"
        ///          }
        ///     }
        /// 
        /// Note: timestamps are of the following format yyyy-MM-dd hh:mm:ss.ms
        /// </remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        //[EnableRateLimiting("fixed")]
        //[HttpPost]
        //public async Task<GetDnsEntriesResponse> GetLatestDnsEntries(GetDnsEntriesRequest request)
        //{
        //    request.latest = true;
        //    GetDnsEntriesResponse response = await _businessOdns.GetDnsEntries(request);
        //    return response;
        //    //return await _businessOdns.GetDnsEntries(request);
        //}

    }
}
