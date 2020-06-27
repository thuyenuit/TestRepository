using SMS.API.Infrastructure.Core;
using SMS.API.Models;
using SMS.DTO.Account.Request;
using SMS.DTO.Account.Response;
using SMS.DTO.Base;
using SMS.Service.IServices;
using SMS.Service.ServiceObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SMS.Shared.Shares;

namespace SMS.API.Api
{
    [Authorize]
    [RoutePrefix("api/account")]
    public class AccountController : BaseApiController
    {
        public AccountController() : base()
        {
        }
     
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public BaseResponse<TokenResponse> Login(LoginRequest model)
        {
            BaseResponse<TokenResponse> response = new BaseResponse<TokenResponse>() {
                ResponseCode = (int)HttpStatusCode.OK,
                Message = "Đăng nhập thành công",
                MsgType = "success"
            };

            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                response.ResponseCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Vui lòng nhập tài khoản và mật khẩu!";
                response.MsgType = "error";
            }
            else
            {
                using (var client = new HttpClient())
                {
                    var postData = new List<KeyValuePair<string, string>>();
                    postData.Add(new KeyValuePair<string, string>("username", model.UserName));
                    postData.Add(new KeyValuePair<string, string>("password", model.Password));
                    postData.Add(new KeyValuePair<string, string>("grant_type", "password"));

                    client.BaseAddress = new Uri(SystemParam.BASE_API);
                    var content = new FormUrlEncodedContent(postData);
                    HttpResponseMessage httpResponse = client.PostAsync("token", content).Result;

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        TokenResponse token = httpResponse.Content.ReadAsAsync<TokenResponse>().Result;
                        response.Data = token;
                    }
                    else
                    {
                        response.ResponseCode = (int)HttpStatusCode.BadRequest;
                        response.Message = "Tài khoản hoặc mật khẩu không đúng!!";
                        response.MsgType = "error";
                    }
                }
            }
            return response;
        }
    }
}
