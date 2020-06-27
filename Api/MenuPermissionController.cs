using SMS.API.Infrastructure.Core;
using SMS.DTO.Base;
using SMS.DTO.Menu.Model;
using SMS.DTO.Menu.Request;
using SMS.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SMS.API.Api
{
    [Authorize]
    [RoutePrefix("api/menupermission")]
    public class MenuPermissionController : BaseApiController
    {
        private readonly ISubMenuService subMenuService;
        private readonly IMenuService menuService;
        private readonly IMenuPermissionService menuPermissionService;

        public MenuPermissionController(
            ISubMenuService subMenuService,
            IMenuService menuService,
            IMenuPermissionService menuPermissionService)
        {
            this.menuService = menuService;
            this.subMenuService = subMenuService;
            this.menuPermissionService = menuPermissionService;
        }

        [HttpPost]
        [Route("getmenu")]
        public BaseResponse<List<MenuModel>> Search(MGetMenuRequest request)
        {
            BaseResponse<List<MenuModel>> response = new BaseResponse<List<MenuModel>>();
            response.ResponseCode = BaseCode.SUCCESS;

           /* var resultMenu = (from mp in menuPermissionService.GetAll()
                              join me in menuService.GetAll() on mp.MenuID equals me.MenuID
                              join sm in subMenuService.GetAll().Where(x => x.IsDisplay) on mp.SubMenuID equals sm.SubMenuID
                              where mp.UserID == request.UserID
                              && me.IsDisplay
                              select new
                              {
                                  MenuID = mp.MenuID,
                                  MenuName = me.MenuName,
                                  Url = me.Url,
                                  Icon = me.Icon,
                                  OrderMenu = me.OrderNumber,
                                  SubMenuID = mp.SubMenuID,
                                  SubMenuName = sm.SubMenuName,
                                  SubUrl = sm.Url,
                                  SubIcon = sm.Icon,
                                  OrderSubMenu = sm.OrderNumber
                              }).OrderBy(x => x.OrderMenu).ThenBy(x => x.OrderSubMenu).ToList();

            var lstGroupByMenuId = resultMenu.GroupBy(x => new { x.MenuID, x.SubMenuID }).Select(x => new { x.Key.MenuID, x.Key.SubMenuID }).ToList();

            List<MenuModel> lstMenu = new List<MenuModel>();       
            List<int> lstMenuID = resultMenu.Select(x => x.MenuID).GroupBy(x => x).Select(x => x.Key).ToList();     
            foreach (var item in lstMenuID)
            {
                var temp = resultMenu.Where(x => x.MenuID == item).FirstOrDefault();
                MenuModel objMenu = new MenuModel();
                objMenu.MenuID = item;
                objMenu.MenuName = temp.MenuName;
                objMenu.OrderNumber = temp.OrderMenu;
                objMenu.Icon = temp.Icon;
                objMenu.Url = temp.Url;

                var lstTemp = lstGroupByMenuId.Where(x => x.MenuID == item && x.SubMenuID > 0).ToList();
                List<SubMenuModel> lstSubMenu = new List<SubMenuModel>() ;
                foreach (var item2 in lstTemp)
                {
                    var subTemp = resultMenu.Where(x => x.MenuID == item && x.SubMenuID == item2.SubMenuID).FirstOrDefault();
                    if (subTemp != null)
                    {
                        lstSubMenu.Add(new SubMenuModel() {
                            SubMenuID = subTemp.SubMenuID,
                            SubMenuName = subTemp.SubMenuName,
                            Url = subTemp.SubUrl,
                            Icon = subTemp.SubIcon,
                            OrderNumber = subTemp.OrderSubMenu,
                            MenuID = subTemp.MenuID
                        });
                    }
                }
                objMenu.SubMenus = lstSubMenu;
                lstMenu.Add(objMenu);
            }*/
            response.Data = null;
            return response;
        }
    }
}
