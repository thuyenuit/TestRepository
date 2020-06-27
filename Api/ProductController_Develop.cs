using SMS.API.ViewModels;
using SMS.Shared.Shares;
using SMS.Service.IServices;
using SMS.Service.ServiceObject;
using SMS.API.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SMS.API.Api
{
    /// <summary>
    /// Product API
    /// </summary>
    [Authorize]
    [RoutePrefix("api/product")]
    public class ProductController : BaseApiController
    {
        private readonly IProductService productService;
        public ProductController(IProductService productService) : base()
        {
            this.productService = productService;
        }

        /// <summary>
        /// Tìm kiếm sản phẩm
        /// </summary>
        /// <param name="request"> request</param>
        /// <param name="id"> id sản phẩm</param>
        /// <returns></returns>
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Search(HttpRequestMessage request, string keyword, int categoryId, int statusId, int page = SystemParam.PAGE, int pageSize = SystemParam.PAGE_SIZE)
        {
            return CreateHttpResponse(request, () =>
            {
                IDictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("KeyWord", keyword);
                dic.Add("CategoryId", categoryId);
                dic.Add("Status", statusId);
                IQueryable<ProductSO> queryProduct = productService.Search(dic);

                IQueryable<ProductViewModel> lstResponse = queryProduct.Select(x => new ProductViewModel
                {
                    ProductName =x.ProductName
                }).OrderByDescending(x => x.ProductName);

                int totalRow = lstResponse.Count();
                IEnumerable<ProductViewModel> lstResult = lstResponse.Skip((page) * pageSize).Take(pageSize);

                var paginationset = new PaginationSet<ProductViewModel>()
                {
                    Items = lstResult,
                    Page = page,
                    TotalCount = totalRow,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize),
                };
                return request.CreateResponse(HttpStatusCode.OK, paginationset);
            });
        }

        /// <summary>
        /// Get sản phẩm bởi id
        /// </summary>
        /// <param name="request"> request</param>
        /// <param name="id"> id sản phẩm</param>
        /// <returns></returns>
        [HttpGet]
        [Route("firstOrDefault")]
        public HttpResponseMessage FirstOrDefault(HttpRequestMessage request, long id)
        {
            return CreateHttpResponse(request, () =>
            {
                var objProduct = productService.GetAll().FirstOrDefault(x => x.ProductID == id);
                return request.CreateResponse(HttpStatusCode.OK, objProduct);
            });
        }

        /// <summary>
        /// Thêm mới một sản phẩm
        /// </summary>
        /// <param name="request"> request</param>
        /// <param name="model">obj model sản phẩm</param>
        [HttpPost]
        [Route("Create")]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductSO model)
        {
            return CreateHttpResponse(request, () =>
            {
                if (string.IsNullOrEmpty(model.ProductName))
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Vui lòng nhập tên sản phẩm");
                }

                if (string.IsNullOrEmpty(model.ProductName))
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Vui lòng nhập mã sản phẩm");
                }

                if (productService.GetAll().Any(x => x.Name.ToUpper().Equals(model.CategoryName.ToUpper())
                                                                             || x.Code.ToUpper().Equals(model.ProductCode.ToUpper())))
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Tên hoặc mã sản phẩm đã tồn tại. Vui lòng kiểm tra lại");
                }

                productService.Create(model);

                return request.CreateResponse(HttpStatusCode.OK, "Thêm mới thành công");
            });
        }

        /// <summary>
        /// Cập nhật một sản phẩm
        /// </summary>
        /// <param name="request"> request</param>
        /// <param name="model">obj model sản phẩm</param>
        [HttpPost]
        [Route("Update")]
        public HttpResponseMessage Update(HttpRequestMessage request, ProductSO model)
        {
            return CreateHttpResponse(request, () =>
            {
                if (string.IsNullOrEmpty(model.ProductName))
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Vui lòng nhập tên sản phẩm");
                }

                if (productService.GetAll().Any(x => x.ProductID != model.ProductID 
                                                && (model.ProductName.ToUpper().Contains(x.Name.ToUpper()) 
                                                    || model.ProductCode.ToUpper().Contains(x.Code.ToUpper())) ))
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Cập nhật thất bại. Tên hoặc mã sản phẩm đã tồn tại");
                }

                var objProduct = productService.GetAll().FirstOrDefault(x => x.ProductID == model.ProductID);
                if (objProduct == null)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Cập nhật thất bại. Sản phẩm không tồn tại");
                }
                else
                {
                    productService.Update(model);
                }


                return request.CreateResponse(HttpStatusCode.OK, "Cập nhật thành công");
            });
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        /// <param name="request"> request</param>
        /// <param name="strJsonId">json id</param>
        [HttpPut]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, string strJsonId)
        {
            return CreateHttpResponse(request, () =>
            {
                if (string.IsNullOrEmpty(strJsonId))
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Xóa thất bại. Vui lòng kiểm tra lại");
                }



                return request.CreateResponse(HttpStatusCode.OK, "Xóa thành công");
            });
        }
		
    }
}
