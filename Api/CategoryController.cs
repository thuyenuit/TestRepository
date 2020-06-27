using SMS.API.Infrastructure.Core;
using SMS.Shared.Shares;
using SMS.DTO.Category.Model;
using SMS.Model.Models;
using SMS.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using SMS.DTO.Base;
using SMS.DTO.Category.Resquest;

namespace SMS.API.Api
{
    [Authorize]
    [RoutePrefix("api/categories")]
    public class CategoryController : BaseApiController
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;

        public CategoryController(IProductService productService,
            ICategoryService categoryService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
        }

        /// <summary>
        /// Search categories
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("search")]
        public BaseResponse<BasePaginationSet<CategoryModel>> Search(string keyword, int page = SystemParam.PAGE, int pageSize = SystemParam.PAGE_SIZE)
        {
            BaseResponse<BasePaginationSet<CategoryModel>> response = new BaseResponse<BasePaginationSet<CategoryModel>>();
            response.ResponseCode = BaseCode.SUCCESS;
            response.MsgType = BaseCode.SUCCESS_TYPE;

            var queryCategory = categoryService.GetAll().Where(x => x.IsActive);
            if (!string.IsNullOrEmpty(keyword))
            {
                queryCategory = queryCategory.Where(x => x.Alias.ToUpper().Contains(keyword.ToUpper()) || x.Name.ToUpper().Contains(keyword.ToUpper()));
            }
                       
            IQueryable<CategoryModel> queryResponse = queryCategory.Select(x => new CategoryModel
            {
                CategoryName = x.Name,
                CategoryID = x.CategoryID,
                Alias = x.Alias,
                IsHomeFlag = x.IsHomeFlag,
                IsActive = x.IsActive,
                MetaDescription = x.MetaDescription,
                MetaKeyword = x.MetaKeyword,
                Sequence = x.Sequence,
            }).OrderBy(x => x.Sequence).ThenBy(x => x.Alias);

            int totalRow = queryResponse.Count();

            List<CategoryModel> lstResult = queryResponse.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var paginationset = new BasePaginationSet<CategoryModel>()
            {
                Items = lstResult,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalRow,
                TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize),
            };
            response.Data = paginationset;
            return response;
        }


        /// <summary>
        /// Get all categories
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-all")]
        public BaseResponse<List<CategoryModel>> GetAll()
        {
            BaseResponse<List<CategoryModel>> response = new BaseResponse<List<CategoryModel>>();
            response.ResponseCode = BaseCode.SUCCESS;
            response.MsgType = BaseCode.SUCCESS_TYPE;

            List<CategoryModel> lstResult = categoryService.GetAll().Where(x => x.IsActive).Select(x => new CategoryModel
            {
                CategoryName = x.Name,
                CategoryID = x.CategoryID,
                Alias = x.Alias,
                IsHomeFlag = x.IsHomeFlag,
                IsActive = x.IsActive,
                MetaDescription = x.MetaDescription,
                MetaKeyword = x.MetaKeyword,
                Sequence = x.Sequence,
            }).OrderBy(x => x.Sequence).ThenBy(x => x.Alias).ToList();
            response.Data = lstResult;

            return response;
        }

        /// <summary>
        /// Find category by id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("find")]
        public BaseResponse<CategoryModel> Find(HttpRequestMessage request, int id)
        {
            BaseResponse<CategoryModel> response = new BaseResponse<CategoryModel>()         
            {
                ResponseCode = BaseCode.SUCCESS,
                Message = "Found a category",
                MsgType = BaseCode.SUCCESS_TYPE
            };

            CategoryModel model = new CategoryModel();

            var objCategory = categoryService.GetAll().FirstOrDefault(x => x.CategoryID == id);
            if (objCategory != null)
            {
                model.CategoryID = objCategory.CategoryID;
                model.CategoryName = objCategory.Name;
                model.Alias = objCategory.Alias;
                model.Sequence = objCategory.Sequence;
                model.IsActive = objCategory.IsActive;
                model.IsHomeFlag = objCategory.IsHomeFlag;
                model.MetaDescription = objCategory.MetaDescription;
                model.MetaKeyword = objCategory.MetaKeyword;
            }
            else
            {
                response.ResponseCode = BaseCode.NOT_FOUND;
                response.Message = "Not found data";
                response.MsgType = BaseCode.ERROR_TYPE;
                return response;
            }
            response.Data = model;
            return response;

        }

        /// <summary>
        /// Create categroy
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public BaseResponse<BusinessException> Create(AddOrEditCategoryRequest<CategoryModel> request)
        {
            BaseResponse<BusinessException> response = new BaseResponse<BusinessException>()
            {
                ResponseCode = BaseCode.SUCCESS,
                Message = "Thêm mới thành công",
                MsgType = BaseCode.SUCCESS_TYPE
            };

            try
            {
                Category objInsert = new Category()
                {
                    Name = request.Model.CategoryName,
                    Alias = request.Model.Alias,
                    CreateBy = request.UserID,
                    CreateDate = DateTime.Now,
                    IsHomeFlag = request.Model.IsHomeFlag,
                    IsActive = request.Model.IsActive,
                    MetaDescription = request.Model.MetaDescription,
                    MetaKeyword = request.Model.MetaKeyword,
                    Sequence = request.Model.Sequence
                };
                categoryService.Create(objInsert);
                return response;
            }
            catch (BusinessException ex)
            {
                response.ResponseCode = BaseCode.CRUD_ERROR;
                response.Message = ex.Message;
                response.Data = ex;
                response.MsgType = BaseCode.ERROR_TYPE;
                return response;
            }
        }

        
        /// <summary>
        /// Update category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public BaseResponse<BusinessException> Update(AddOrEditCategoryRequest<CategoryModel> request)
        {
            BaseResponse<BusinessException> response = new BaseResponse<BusinessException>()
            {
                ResponseCode = BaseCode.SUCCESS,
                Message = "Cập nhật thành công",
                MsgType = BaseCode.SUCCESS_TYPE
            };

            try
            {
                if (string.IsNullOrEmpty(request.Model.CategoryName))
                {
                    response.ResponseCode = BaseCode.VALIDATE_ERROR;
                    response.Message = "Vui lòng nhập tên sản phẩm";
                    response.MsgType = BaseCode.ERROR_TYPE;
                    return response;
                }

                if (categoryService.GetAll().Any(x => x.CategoryID != request.Model.CategoryID && request.Model.CategoryName.ToUpper().Contains(x.Name.ToUpper())))
                {
                    response.ResponseCode = BaseCode.VALIDATE_ERROR;
                    response.Message = "Cập nhật thất bại. Tên thể loại đã tồn tại";
                    response.MsgType = BaseCode.ERROR_TYPE;
                    return response;
                }

                Category objCategory = new Category()
                {
                    CategoryID = request.Model.CategoryID,
                    Name = request.Model.CategoryName,
                    Alias = request.Model.Alias,
                    MetaDescription = request.Model.MetaDescription,
                    MetaKeyword = request.Model.MetaKeyword,
                    Sequence = request.Model.Sequence,
                    IsHomeFlag = request.Model.IsHomeFlag,
                    IsActive = request.Model.IsActive,
                    UpdateBy = request.UserID,
                    ModifiedDate = DateTime.Now,
                };
                categoryService.Update(objCategory);
                return response;
            }
            catch (BusinessException ex)
            {
                response.ResponseCode = BaseCode.CRUD_ERROR;
                response.Message = ex.Message;
                response.Data = ex;
                response.MsgType = BaseCode.ERROR_TYPE;
                return response;
            }
        }

        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        public BaseResponse<BusinessException> Delete(HttpRequestMessage request, int id)
        {
            BaseResponse<BusinessException> response = new BaseResponse<BusinessException>()
            {
                ResponseCode = BaseCode.SUCCESS,
                Message = "Xóa thành công",
                MsgType = BaseCode.SUCCESS_TYPE
            };

            var objCategory = categoryService.GetAll().FirstOrDefault(x => x.CategoryID == id);
            if (objCategory != null)
            {
                categoryService.Delete(objCategory);
            }
            else
            {
                response.ResponseCode = BaseCode.NOT_FOUND;
                response.Message = "Xóa thất bại. Không tìm thấy bản ghi";
                response.MsgType = BaseCode.ERROR_TYPE;
            }
            return response;
        }

        ///// <summary>
        ///// Xóa thể loại
        ///// </summary>
        ///// <param name="request"> request</param>
        ///// <param name="strJsonId">json id</param>
        //[HttpPut]
        //[Route("delete")]
        //public HttpResponseMessage Delete(HttpRequestMessage request, string strJsonId)
        //{
        //    return CreateHttpResponse(request, () =>
        //    {
        //        if (string.IsNullOrEmpty(strJsonId))
        //        {
        //            return request.CreateResponse(HttpStatusCode.BadRequest, "Xóa thất bại. Vui lòng kiểm tra lại");
        //        }



        //        return request.CreateResponse(HttpStatusCode.OK, "Xóa thành công");
        //    });
        //}
    }
}
