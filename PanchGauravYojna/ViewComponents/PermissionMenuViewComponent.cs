using BL.ManageMaster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MO.Common;
using MO.MenuMaster;
using PanchGauravYojna.Helpers;

namespace PanchGauravYojna.ViewComponents
{
    public class PermissionMenuViewComponent : ViewComponent
    {
        private readonly IMenu _iMenu;
        
        public PermissionMenuViewComponent(IMenu iMenu)
        {
            _iMenu = iMenu;            
        }
        public async Task<IViewComponentResult> InvokeAsync(string groupId)
        {
            //// --- Claims Expired Check ---
            //if (!CommonHelper.IsUserValid(User, out int groupId, out int userId))
            //{
            //    await HttpContext.SignOutAsync("Cookies");
            //    return RedirectToAction("Login", "Account");
            //}

            var menuList = await _iMenu.GetMenuPermissiondataList(new CommonFilter { option1 = groupId });

            var treeView = BuildTree(menuList, null); // Convert to hierarchical structure
            return View(treeView);
        }

        private List<MenuItem> BuildTree(List<MenuItem> menuItems, int? parentId)
        {
            return menuItems.Where(m => m.ParentId == parentId && m.isChecked == true)
                .Select(m => new MenuItem
                {
                    Id = m.Id,
                    ParentId = m.ParentId,
                    Name = m.Name,
                    OrderNumber = m.OrderNumber,
                    Children = BuildTree(menuItems, m.Id),
                    isChecked = m.isChecked,
                    ControllerName = m.ControllerName,
                    ActionName = m.ActionName,
                    IconClass = m.IconClass,
                }).ToList();
        }
    }
}
