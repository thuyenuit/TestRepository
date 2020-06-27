using SMS.API.Infrastructure.Core;
using SMS.Shared.Shares;
using SMS.Model.Models;
using SMS.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SMS.DTO.Base;
using SMS.DTO.Category.Resquest;
using SMS.DTO.ProductCategory.Model;

namespace SMS.API.Api
{
    [Authorize]
    [RoutePrefix("api/product-categories")]
    public class ProductCategoryController : BaseApiController
    {
        private readonly IProductCategoryService productCategoryService;
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;

        public ProductCategoryController(IProductService productService,
            ICategoryService categoryService,
            IProductCategoryService productCategoryService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
            this.productCategoryService = productCategoryService;
        }

        /// <summary>
        /// Search product categories
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("search")]
        public BaseResponse<BasePaginationSet<ProductCategoryViewModel>> Search(string keyword, int page = SystemParam.PAGE, int pageSize = SystemParam.PAGE_SIZE)
        {
            BaseResponse<BasePaginationSet<ProductCategoryViewModel>> response = new BaseResponse<BasePaginationSet<ProductCategoryViewModel>>();
            response.ResponseCode = BaseCode.SUCCESS;
           
            var queryProductCategory = productCategoryService.GetAll().Where(x => x.IsActive);

            if(!string.IsNullOrEmpty(keyword))
            {
                queryProductCategory = queryProductCategory.Where(x => x.Name.ToUpper().Contains(keyword.ToUpper()) 
                                                                || x.Alias.ToUpper().Contains(keyword.ToUpper())
                                                                || x.Category.Name.ToUpper().Contains(keyword.ToUpper())
                                                                || x.Category.Alias.ToUpper().Contains(keyword.ToUpper()));
            }
          
            IQueryable<ProductCategoryViewModel> queryResponse = queryProductCategory.Select(x => new ProductCategoryViewModel
            {
                ProductCategoryID = x.ProductCategoryID,
                Name = x.Name,
                Alias = x.Alias,
                IsHomeFlag = x.IsHomeFlag,
                IsActive = x.IsActive,
                Sequence = x.Sequence,
                CategoryID = x.CategoryID,
                CategoryName = x.Category.Name,
                SequenceCategroy = x.Category.Sequence,
            }).OrderBy(x => x.SequenceCategroy).ThenBy(x => x.CategoryName).ThenBy(x => x.Sequence).ThenBy(x => x.Name);

            int totalRow = queryResponse.Count();

            List<ProductCategoryViewModel> lstResult = queryResponse.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var paginationset = new BasePaginationSet<ProductCategoryViewModel>()
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
        /// Find product category by id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("find")]
        public BaseResponse<ProductCategoryViewModel> Find(HttpRequestMessage request, int id)
        {
            BaseResponse<ProductCategoryViewModel> response = new BaseResponse<ProductCategoryViewModel>()
            {
                ResponseCode = BaseCode.SUCCESS,
                Message = "Found a product category",
                MsgType = BaseCode.SUCCESS_TYPE
            };

            ProductCategoryViewModel model = new ProductCategoryViewModel();
            var entityProductCategory = productCategoryService.GetAll().FirstOrDefault(x => x.ProductCategoryID == id);
            if (entityProductCategory != null)
            {
                model.ProductCategoryID = entityProductCategory.ProductCategoryID;
                model.Name = entityProductCategory.Name;
                model.CategoryID = entityProductCategory.CategoryID;
                model.Alias = entityProductCategory.Alias;
                model.Sequence = entityProductCategory.Sequence;
                model.IsActive = entityProductCategory.IsActive;
                model.IsHomeFlag = entityProductCategory.IsHomeFlag;
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
        /// Create product categroy
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public BaseResponse<BusinessException> Create(AddOrEditCategoryRequest<ProductCategoryViewModel> request)
        {
            BaseResponse<BusinessException> response = new BaseResponse<BusinessException>()
            {
                ResponseCode = BaseCode.SUCCESS,
                Message = "Create product category success",
                MsgType = BaseCode.SUCCESS_TYPE
            };
       
            try
            {
                if (string.IsNullOrEmpty(request.Model.Name))
                {
                    response.ResponseCode = BaseCode.VALIDATE_ERROR;
                    response.Message = "The Name field is required";
                    response.MsgType = BaseCode.ERROR_TYPE;
                    return response;
                }

                if (request.Model.CategoryID <= 0)
                {
                    response.ResponseCode = BaseCode.VALIDATE_ERROR;
                    response.Message = "The Category field is required";
                    response.MsgType = BaseCode.ERROR_TYPE;
                    return response;
                }

                if (productCategoryService.GetAll().Any(x => x.CategoryID == request.Model.CategoryID && x.Name.ToUpper().Equals(request.Model.Name.ToUpper())))
                {
                    response.ResponseCode = BaseCode.VALIDATE_ERROR;
                    response.Message = "Update product category fail. This Name field is already in use";
                    response.MsgType = BaseCode.ERROR_TYPE;
                    return response;
                }

                ProductCategory objInsert = new ProductCategory()
                {
                    Name = request.Model.Name,
                    Alias = request.Model.Alias,
                    Sequence = request.Model.Sequence,
                    IsHomeFlag = request.Model.IsHomeFlag,
                    IsActive = request.Model.IsActive,
                    CategoryID = request.Model.CategoryID,
                    CreateBy = request.UserID,
                    CreateDate = DateTime.Now,
                };
                productCategoryService.Create(objInsert);
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
        /// Update product category
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public BaseResponse<BusinessException> Update(AddOrEditCategoryRequest<ProductCategoryViewModel> request)
        {
            BaseResponse<BusinessException> response = new BaseResponse<BusinessException>()
            {
                ResponseCode = BaseCode.SUCCESS,
                Message = "Update product category success",
                MsgType = BaseCode.SUCCESS_TYPE
            };

            try
            {
                if (string.IsNullOrEmpty(request.Model.Name))
                {
                    response.ResponseCode = BaseCode.VALIDATE_ERROR;
                    response.Message = "The Name field is required";
                    response.MsgType = BaseCode.ERROR_TYPE;
                    return response;
                }

                if (request.Model.CategoryID <= 0)
                {
                    response.ResponseCode = BaseCode.VALIDATE_ERROR;
                    response.Message = "The Category field is required";
                    response.MsgType = BaseCode.ERROR_TYPE;
                    return response;
                }

                if (productCategoryService.GetAll().Any(x => x.ProductCategoryID != request.Model.ProductCategoryID 
                                                        && x.CategoryID == request.Model.CategoryID 
                                                        && request.Model.Name.ToUpper().Contains(x.Name.ToUpper())))
                {
                    response.ResponseCode = BaseCode.VALIDATE_ERROR;
                    response.Message = "Update product category fail. This Name field is already in use";
                    response.MsgType = BaseCode.ERROR_TYPE;
                    return response;
                }

                ProductCategory entity = new ProductCategory()
                {
                    ProductCategoryID = request.Model.ProductCategoryID,
                    CategoryID = request.Model.CategoryID,
                    Name = request.Model.Name,
                    Alias = request.Model.Alias,
                    Sequence = request.Model.Sequence,
                    IsHomeFlag = request.Model.IsHomeFlag,
                    IsActive = request.Model.IsActive,
                    UpdateBy = request.UserID,
                    ModifiedDate = DateTime.Now,
                };
                productCategoryService.Update(entity);
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
        /// Delete product category
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
                Message = "Delete product category success",
                MsgType = BaseCode.SUCCESS_TYPE
            };

            var entityProductCategory = productCategoryService.GetAll().FirstOrDefault(x => x.ProductCategoryID == id);
            if (entityProductCategory != null)
            {
                productCategoryService.Delete(entityProductCategory);
            }
            else
            {
                response.ResponseCode = BaseCode.NOT_FOUND;
                response.Message = "Delete product category fail. This record is not found!";
                response.MsgType = BaseCode.ERROR_TYPE;
            }
            return response;
        }
    }
}
