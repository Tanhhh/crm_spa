
using Erp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Erp.Domain.Staff.Repositories;
using Erp.Domain.Sale.Repositories;
using Erp.Domain.Account.Repositories;
using Erp.Domain.Entities;
using System.Globalization;
using Erp.BackOffice.Staff.Models;
using Erp.BackOffice.Sale.Models;
using Erp.Domain.Sale.Interfaces;
using Erp.Domain.Sale.Entities;
using Erp.Domain.Staff.Entities;
using Erp.Domain;
using Erp.Domain.Account.Helper;

namespace Erp.BackOffice.Helpers
{
    public class SelectListHelper
    {
        public static SelectList GetSelectList_NhomSP( object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            DM_NHOMSANPHAMRepositories serviceRepository = new DM_NHOMSANPHAMRepositories(new Domain.Sale.ErpSaleDbContext());



            var categories = serviceRepository.GetAllDM_NHOMSANPHAM().ToList();

            List<SelectListItem> options = new List<SelectListItem>
            {

            };
            // Get the top level parents
            var parents = categories.Where(x => x.NHOM_CHA == null || x.NHOM_CHA == 0);
            int tmp = 0;
            if (tmp == 0)
            {
                string menucap1 = "Menu cấp 1";
                options.Add(new SelectListItem()
                {

                    Value = "0",
                    Text = menucap1.ToString()
                });
            }
            tmp++;
            foreach (var parent in parents)
            {

                // Add SelectListItem for the parent
                options.Add(new SelectListItem()
                {
                    Value = parent.NHOMSANPHAM_ID.ToString(),
                    Text = parent.TEN_NHOMSANPHAM
                });
                // Get the child items associated with the parent
                var children = categories.Where(x => x.NHOM_CHA == parent.NHOMSANPHAM_ID);
                // Add SelectListItem for each child
                foreach (var child in children)
                {
                    options.Add(new SelectListItem()
                    {
                        Value = child.NHOMSANPHAM_ID.ToString(),
                        Text = string.Format("--{0}", child.TEN_NHOMSANPHAM)
                    });



                    var children2 = categories.Where(x => x.NHOM_CHA == child.NHOMSANPHAM_ID);
                    // Add SelectListItem for each child
                    foreach (var child2 in children2)
                    {
                        options.Add(new SelectListItem()
                        {
                            Value = child2.NHOMSANPHAM_ID.ToString(),
                            Text = string.Format("----{0}", child2.TEN_NHOMSANPHAM)
                        });
                    }


                }






            }





            var selectList = new SelectList(options, "Value", "Text", sValue);

            return selectList;
        }

        public static List<Category> GetLitCategory(string text)
        {
            CategoryRepository categoryRepository = new CategoryRepository(new Domain.ErpDbContext());
            List<Category> arr = categoryRepository.GetListCategoryByCode(text);

            return arr;
        }

        public static SelectList GetSelectList_Product(string Type, object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            ProductOrServiceRepository serviceRepository = new ProductOrServiceRepository(new Domain.Sale.ErpSaleDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = serviceRepository.GetAllProductByType(Type).OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code + " - " + i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList SelectListMonth(string text)
        {
            var selectListItems = new List<SelectListItem>();
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            for (int i = 1; i <= 12; i++)
            {
                string value = string.Format("{0}/{1}", i.ToString("00"), year);
                var item = new SelectListItem { Value = value, Text = string.Format(text, value) };
                selectListItems.Add(item);
            }

            if (month <= 3)
            {
                for (int i = 12; i >= 10; i--)
                {
                    string value = string.Format("{0}/{1}", i.ToString("00"), year - 1);
                    var item = new SelectListItem { Value = value, Text = string.Format(text, value) };
                    selectListItems.Insert(0, item);
                }
            }

            if (month >= 10)
            {
                for (int i = 1; i <= 3; i++)
                {
                    string value = string.Format("{0}/{1}", i.ToString("00"), year + 1);
                    var item = new SelectListItem { Value = value, Text = string.Format(text, value) };
                    selectListItems.Insert(selectListItems.Count, item);
                }
            }

            var selectList = new SelectList(selectListItems, "Value", "Text", string.Format("{0}/{1}", month.ToString("00"), year));

            return selectList;
        }

        public static List<SelectListItem> SelectListItemMonth(string text)
        {
            var selectListItems = new List<SelectListItem>();
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            for (int i = 1; i <= 12; i++)
            {
                string value = string.Format("{0}/{1}", i.ToString("00"), year);
                var item = new SelectListItem { Value = value, Text = string.Format("{0} {1}", text, value) };
                selectListItems.Add(item);
            }

            if (month <= 3)
            {
                for (int i = 12; i >= 10; i--)
                {
                    string value = string.Format("{0}/{1}", i.ToString("00"), year - 1);
                    var item = new SelectListItem { Value = value, Text = string.Format("{0} {1}", text, value) };
                    selectListItems.Insert(0, item);
                }
            }

            if (month >= 10)
            {
                for (int i = 1; i <= 3; i++)
                {
                    string value = string.Format("{0}/{1}", i.ToString("00"), year + 1);
                    var item = new SelectListItem { Value = value, Text = string.Format("{0} {1}", text, value) };
                    selectListItems.Insert(selectListItems.Count, item);
                }
            }

            return selectListItems;
        }

        public static SelectList GetSelectList_Category(string sCode, object SelectedValue, string NullOrNameEmpty)
        {
            return GetSelectList_Category(sCode, SelectedValue, null, NullOrNameEmpty);
        }

        public static SelectList GetSelectList_CategoryHT(string sCode, object SelectedValue, string NullOrNameEmpty)
        {
            return GetSelectList_CategoryHT(sCode, SelectedValue, "Name", NullOrNameEmpty,null);
        }

        public static SelectList GetSelectList_CategoryHT2(string sCode, object SelectedValue, string NullOrNameEmpty, string NotHavePlan)
        {
            return GetSelectList_CategoryHT(sCode, SelectedValue, "Name", NullOrNameEmpty, NotHavePlan);
        }

        public static SelectList GetSelectList_StatusStaffEquipment()
        {
            var selectListItems = new List<SelectListItem>();
            for (int i = 0; i < 2; i++)
            {
                SelectListItem item = new SelectListItem();
                if (i == 0)
                {

                    item.Text = "Đang sử dụng";

                    item.Value = "true";
                }
                else
                {

                    item.Text = "Chưa sử dụng";

                    item.Value = "false";
                }


                selectListItems.Add(item);
            }
            var selectList = new SelectList(selectListItems, "Value", "Text", null);

            return selectList;
        }
        public static SelectList GetSelectList_Category(string sCode, object SelectedValue, string sValueField, string NullOrNameEmpty)
        {
            CategoryRepository categoryRepository = new CategoryRepository(new Domain.ErpDbContext());
            var selectListItems = new List<SelectListItem>();

            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }

            try
            {
                var q = categoryRepository.GetCategoryByCode(sCode);

                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    if (sValueField != null && sValueField == "Name")
                        item.Value = i.Name;
                    else if (sValueField != null && sValueField == "Value")
                        item.Value = i.Value;
                    else
                        item.Value = i.Value;

                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", SelectedValue);

            return selectList;
        }

        public static SelectList GetSelectList_CategoryHT(string sCode, object SelectedValue, string sValueField, string NullOrNameEmpty, string NotHavePlan)
        {
            CategoryRepository categoryRepository = new CategoryRepository(new Domain.ErpDbContext());
            var selectListItems = new List<SelectListItem>();

            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            if (NotHavePlan != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NotHavePlan;
                itemEmpty.Value = NotHavePlan;
                selectListItems.Add(itemEmpty);
            }

            try
            {
                var q = categoryRepository.GetCategoryByCode(sCode);

                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    if (sValueField != null && sValueField == "Name")
                        item.Value = i.Name;
                    else if (sValueField != null && sValueField == "Value")
                        item.Value = i.Value;
                    else
                        item.Value = i.Value;

                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", SelectedValue);

            return selectList;
        }
        public static SelectList GetSelectList_Number(int nStart, int nEnd, object sValue)
        {
            var selectListItems = new List<SelectListItem>();

            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = App_GlobalResources.Wording.Empty;
            itemEmpty.Value = null;

            selectListItems.Add(itemEmpty);

            try
            {
                for (int i = nStart; i <= nEnd; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();

                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_Gender(object sValue, string NullOrNameEmpty = null)
        {
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;

                selectListItems.Add(itemEmpty);
            }
            try
            {
                SelectListItem item = new SelectListItem();
                item.Text = "Nam";
                item.Value = "false";

                selectListItems.Add(item);

                SelectListItem item2 = new SelectListItem();
                item2.Text = "Nữ";
                item2.Value = "true";

                selectListItems.Add(item2);
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_Location(string sParentId, object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            LocationRepository locationRepository = new LocationRepository(new Domain.ErpDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }

            try
            {
                var q = locationRepository.GetList(sParentId);

                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = Erp.BackOffice.Helpers.Common.Capitalize(/*i.Type.ToLower() + " " +*/ i.Name.ToLower());
                    item.Value = i.Id;

                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        #region SelectList for PageMenu
        public static SelectList GetSelectList_Page(string AreaName)
        {
            var selectListItems = new List<SelectListItem>();
            PageRepository pageRepository = new PageRepository(new Domain.ErpDbContext());
            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = App_GlobalResources.Wording.Empty;
            itemEmpty.Value = null;
            selectListItems.Add(itemEmpty);

            try
            {
                var q = pageRepository.GetPages()
                    .Where(item => item.AreaName == AreaName).ToList();

                var controllerList = q.GroupBy(
                                        p => p.ControllerName,
                                        (key, g) => new
                                        {
                                            ControllerName = key,
                                            ActionList = g.ToList()
                                        }
                                        ).OrderBy(item => item.ControllerName);
                int index1 = 1;
                foreach (var controller in controllerList)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = index1 + ". " + controller.ControllerName;
                    item.Value = controller.ControllerName;
                    selectListItems.Add(item);

                    int index2 = 1;
                    foreach (var action in controller.ActionList)
                    {
                        SelectListItem itemAction = new SelectListItem();
                        itemAction.Text = HttpContext.Current.Server.HtmlDecode(@"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;") + index1 + "." + index2 + controller.ControllerName + "/" + action.ActionName;
                        itemAction.Value = action.Id.ToString();
                        selectListItems.Add(itemAction);

                        index2++;
                    }

                    index1++;
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", null);

            return selectList;
        }

        public static SelectList GetSelectList_PageMenu(string LanguageId)
        {
            var selectListItems = new List<SelectListItem>();
            PageMenuRepository pageMenuRepository = new PageMenuRepository(new Domain.ErpDbContext());
            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = App_GlobalResources.Wording.Empty;
            itemEmpty.Value = null;
            selectListItems.Add(itemEmpty);

            try
            {
                var q = pageMenuRepository.GetPageMenus(LanguageId).ToList();

                List<vwPageMenu> pageMenuResults = new List<vwPageMenu>();
                BuildListMuiltiLevel(q, pageMenuResults, null);

                foreach (var i in pageMenuResults)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", null);

            return selectList;
        }

        public static void BuildListMuiltiLevel(List<vwPageMenu> pageMenus, List<vwPageMenu> pageMenuResults, int? parentId, string level = "")
        {
            int index = 1;
            var parentMenus = pageMenus.Where(x => x.ParentId == parentId).OrderBy(x => x.OrderNo).ToList();
            foreach (vwPageMenu item in parentMenus)
            {
                string prefix = string.IsNullOrEmpty(level) ? index.ToString(CultureInfo.InvariantCulture) : level + "." + index.ToString(CultureInfo.InvariantCulture);
                int n = prefix.Split('.').Count();
                string tab = "";
                for (int i = 1; i < n; i++)
                {
                    tab = tab + @"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                }
                item.Name = HttpContext.Current.Server.HtmlDecode(tab) + prefix + " " + item.Name;
                pageMenuResults.Add(item);
                BuildListMuiltiLevel(pageMenus, pageMenuResults, item.Id, prefix);
                index++;
            }
        }

        public static SelectList GetSelectListStaffNotFamily(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            StaffsRepository staffRepository = new StaffsRepository(new Domain.Staff.ErpStaffDbContext());

            StaffFamilyRepository staffFamilyRepository = new StaffFamilyRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            var staff = Erp.BackOffice.Helpers.Common.GetStaffByCurrentUser();

            try
            {
                List<Erp.Domain.Staff.Entities.vwStaffs> q = new List<Domain.Staff.Entities.vwStaffs>();
                q = staffRepository.GetvwAllStaffs().OrderBy(item => item.Name).ToList();
                //if (staff.Sale_BranchId == null)
                //{
                //    q = staffRepository.GetvwAllStaffs().OrderBy(item => item.Name).ToList();
                //}
                //else
                //{
                //    q = staffRepository.GetvwAllStaffs().Where(x => x.Sale_BranchId == staff.Sale_BranchId).OrderBy(x => x.Name).ToList();

                //}

                List<int> staff_family = staffFamilyRepository.GetAllStaffFamily().Select(n => n.StaffId.Value).Distinct().ToList();
                if (sValue != null)
                {
                    staff_family.Remove(int.Parse(sValue.ToString()));
                }
                if (staff_family != null && staff_family.Count() > 0)
                {
                    q = q.Where(n => !staff_family.Contains(n.Id)).ToList();
                }
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code + " - " + i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        #endregion

        public static SelectList GetSelectList_Module(object sValue)
        {
            var selectListItems = new List<SelectListItem>();
            ModuleRepository ModuleRepository = new ModuleRepository(new Domain.ErpDbContext());
            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = App_GlobalResources.Wording.Empty;
            itemEmpty.Value = null;
            selectListItems.Add(itemEmpty);
            try
            {
                var q = ModuleRepository.GetAllModule().OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Name.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_MetadataFields(string ModuleName, object sValue)
        {
            var selectListItems = new List<SelectListItem>();
            MetadataFieldRepository MetadataFieldRepository = new MetadataFieldRepository(new Domain.ErpDbContext());
            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = App_GlobalResources.Wording.Empty;
            itemEmpty.Value = null;
            selectListItems.Add(itemEmpty);
            try
            {
                var q = MetadataFieldRepository.GetAllMetadataField()
                    .Where(item => item.ModuleName == ModuleName)
                    .OrderBy(item => item.Id).ToList();
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Name.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_Branch_byID(int? sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {

            }
            try
            {


                BranchRepository branchRepository = new BranchRepository(new Domain.Staff.ErpStaffDbContext());
                var list = branchRepository.GetAllBranch().Where(x => x.Id == sValue).ToList();

                var q = list.Select(item => new { item.Id, item.Name })
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }).OrderBy(item => item.Text).ToList();

                selectListItems.AddRange(q);

            }
            catch (Exception ex)
            {

            }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_Branch(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                BranchRepository branchRepository = new BranchRepository(new Domain.Staff.ErpStaffDbContext());
                var list = branchRepository.GetAllBranch().ToList();
                //if (!Filters.SecurityFilter.IsAdmin())
                if (Helpers.Common.CurrentUser.BranchId > 0)
                {
                    list = list.Where(x => x.Id == Helpers.Common.CurrentUser.BranchId).ToList();
                }
                else
                {
                    list = list.Where(x => x.ParentId == null).ToList();
                }
                var q = list.Select(item => new { item.Id, item.Name })
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }).OrderBy(item => item.Text).ToList();

                selectListItems.AddRange(q);

            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_BranchAll(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                BranchRepository branchRepository = new BranchRepository(new Domain.Staff.ErpStaffDbContext());

                var q = branchRepository.GetAllBranch().Where(x => x.ParentId == null)
                    .Select(item => new { item.Id, item.Name })
                    .ToList()
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }).OrderBy(item => item.Text).ToList();

                selectListItems.AddRange(q);

            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_BranchDepartment(object sValue, int? BranchId, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            BranchDepartmentRepository branchDepartmentRepository = new BranchDepartmentRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                if (BranchId != null && BranchId.Value > 0)
                {
                    var q = branchDepartmentRepository.GetAllvwBranchDepartment().Where(x => x.Sale_BranchId == BranchId).OrderBy(item => item.Staff_DepartmentId);
                    foreach (var i in q)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = i.Staff_DepartmentId;
                        item.Value = i.Id.ToString();
                        selectListItems.Add(item);
                    }
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_SymbolTimekeeping(object sValue, string NullOrNameEmpty, bool? TypeDayOff)
        {
            var selectListItems = new List<SelectListItem>();
            SymbolTimekeepingRepository symboltimekeepingRepository = new SymbolTimekeepingRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                IQueryable<Domain.Staff.Entities.SymbolTimekeeping> q;
                if (TypeDayOff == true)
                {
                    q = symboltimekeepingRepository.GetAllSymbolTimekeeping().Where(x => x.DayOff == true).OrderBy(item => item.Name);
                }
                else
                {
                    q = symboltimekeepingRepository.GetAllSymbolTimekeeping().OrderBy(item => item.Name);
                }
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code + " - " + i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_Shifts(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            ShiftsRepository shiftsRepository = new ShiftsRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = shiftsRepository.GetAllShifts().OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name + "(" + i.StartTime + " - " + i.EndTime + ")";
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_Salaytable(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            SalaryTableRepository shiftsRepository = new SalaryTableRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = shiftsRepository.GetAllSalaryTable().Where(item => item.Status != App_GlobalResources.Wording.SalaryTableStatus_InProcess).OrderByDescending(item => item.TargetYear).ThenBy(n => n.TargetMonth);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = string.Format("{0} {1}/{2}", i.Name, i.TargetMonth, i.TargetYear);
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_TaxIncomePerson(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            TaxIncomePersonRepository taxIncomePersonRepository = new TaxIncomePersonRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = taxIncomePersonRepository.GetAllTaxIncomePerson();
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = string.Format("{0}", i.Code);
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_User(object sValue)
        {
            var selectListItems = new List<SelectListItem>();
            UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = App_GlobalResources.Wording.Empty;
            itemEmpty.Value = null;
            selectListItems.Add(itemEmpty);
            try
            {
                var q = userRepository.GetAllUsers().OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.FullName;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        //Cong
        public static SelectList GetSelectList_User_kd(object sValue, string BranchId)
        {
            int branchId = 0;
            if (!String.IsNullOrEmpty(BranchId))
            {
                branchId = Convert.ToInt32(BranchId);
            }
           

            HttpCookie cookie = HttpContext.Current.Request.Cookies["BRANCH_ID_SPA_CookieName"];
            string strBrandID = "0";
            if (cookie != null)
            {
                strBrandID = cookie.Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }
            //get  CurrentUser.branchId

            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);

            branchId = intBrandID.Value;
            var selectListItems = new List<SelectListItem>();
            UserType_kdRepository userRepository = new UserType_kdRepository(new Domain.ErpDbContext());
            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = "Nhóm quản lý";
            itemEmpty.Value = null;
            selectListItems.Add(itemEmpty);
            try
            {
                var q = userRepository.GetAll().Where(x => x.BranchId == branchId).OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        //end cong
        public static SelectList GetSelectList_Staff(object sValue, string NullOrNameEmpty, string PositionCode = null)
        {
            var selectListItems = new List<SelectListItem>();
            StaffsRepository staffRepository = new StaffsRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            var staff = Erp.BackOffice.Helpers.Common.CurrentUser;
            try
            {

                var q = staffRepository.GetvwAllStaffs().OrderBy(item => item.Name).ToList();
                //if (!string.IsNullOrEmpty(staff.DrugStore))
                //{
                //    q = q.Where(x => ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + x.DrugStore + ",") == true).ToList();
                //}
                if (!string.IsNullOrEmpty(PositionCode))
                {
                    q = q.Where(x => x.PositionCode == PositionCode).ToList();
                }
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code + " - " + i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_GetDotBCBHXH(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            DotBCBHXHRepository dotBCBHXHRepository = new DotBCBHXHRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            var staff = Erp.BackOffice.Helpers.Common.GetStaffByCurrentUser();

            try
            {
                //if (staff.Sale_BranchId == null)
                //{
                List<Erp.Domain.Staff.Entities.DotBCBHXH> q = dotBCBHXHRepository.GetAllDotBCBHXH().ToList();

                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
                //}
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_Staff_2(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            StaffsRepository staffRepository = new StaffsRepository(new Domain.Staff.ErpStaffDbContext());

            StaffSocialInsuranceRepository staffSocialInsuranceRepository = new StaffSocialInsuranceRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            var staff = Erp.BackOffice.Helpers.Common.GetStaffByCurrentUser();

            try
            {
                List<Erp.Domain.Staff.Entities.vwStaffs> q = new List<Domain.Staff.Entities.vwStaffs>();
                DateTime date = DateTime.Now;
                q = staffRepository.GetvwAllStaffs().OrderBy(item => item.Name).ToList();
                //if (staff.Sale_BranchId == null)
                //{
                //    q = staffRepository.GetvwAllStaffs().OrderBy(item => item.Name).ToList();
                //}
                //else
                //{
                //    q = staffRepository.GetvwAllStaffs().Where(x => x.Sale_BranchId == staff.Sale_BranchId).OrderBy(x => x.Name).ToList();

                //}

                List<int> staff_used = staffSocialInsuranceRepository.GetAll().Where(n => n.SocietyEndDate >= date || n.MedicalEndDate >= date).Select(n => n.StaffId.Value).ToList();
                if (sValue != null)
                {
                    staff_used.Remove(int.Parse(sValue.ToString()));
                }
                if (staff_used != null && staff_used.Count() > 0)
                {
                    q = q.Where(n => !staff_used.Contains(n.Id)).ToList();
                }
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code + " - " + i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList(string TableName, string DisplayField, string ValueField, object sValue, string emptyLabel = null)
        {
            var selectListItems = new List<SelectListItem>();
            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = emptyLabel == null ? App_GlobalResources.Wording.Empty : emptyLabel;
            itemEmpty.Value = null;
            selectListItems.Add(itemEmpty);
            try
            {
                var q = Domain.Helper.SqlHelper.QuerySQL<SelectListItem>(string.Format("select {0} as [Value], {1} as Text from {2} order by {3}", DisplayField, ValueField, TableName, DisplayField));
                selectListItems.AddRange(q);
                //foreach (var i in q)
                //{
                //    SelectListItem item = new SelectListItem();
                //    item.Text = i.UserName;
                //    item.Value = i.Id.ToString();
                //    selectListItems.Add(item);
                //}
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_Contact(object sValue)
        {
            var selectListItems = new List<SelectListItem>();
            ContactRepository ContactRepository = new ContactRepository(new Domain.Account.ErpAccountDbContext());
            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = App_GlobalResources.Wording.Empty;
            itemEmpty.Value = null;
            selectListItems.Add(itemEmpty);
            try
            {
                var q = ContactRepository.GetAllContact().OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.LastName + " " + i.FirstName;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_DocumentType(object sValue)
        {
            var selectListItems = new List<SelectListItem>();
            DocumentTypeRepository documentTypeRepository = new DocumentTypeRepository(new Domain.Staff.ErpStaffDbContext());
            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = App_GlobalResources.Wording.Empty;
            itemEmpty.Value = null;
            selectListItems.Add(itemEmpty);
            try
            {
                var q = documentTypeRepository.GetAllDocumentType().OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_ContractType(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            LabourContractTypeRepository labourContractTypeRepository = new LabourContractTypeRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = labourContractTypeRepository.GetAllLabourContractType().OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_Contract(object sValue)
        {
            var selectListItems = new List<SelectListItem>();
            ContractRepository contractRepository = new ContractRepository(new Domain.Account.ErpAccountDbContext());
            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = App_GlobalResources.Wording.Empty;
            itemEmpty.Value = null;
            selectListItems.Add(itemEmpty);
            try
            {
                var q = contractRepository.GetAllContract().OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_LabourContract(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            LabourContractRepository labourcontractRepository = new LabourContractRepository(new Domain.Staff.ErpStaffDbContext());

            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = labourcontractRepository.GetAllLabourContract().OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code + " - " + i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_BonusDiscipline(object sValue, int? StaffId, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            BonusDisciplineRepository bonusDisciplineRepository = new BonusDisciplineRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = bonusDisciplineRepository.GetAllvwBonusDiscipline().Where(x => x.StaffId == StaffId).OrderByDescending(item => item.CreatedDate);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_CategoryValueTextName(string sCode, object SelectedValue, string sValueField, string NullOrNameEmpty)
        {
            CategoryRepository categoryRepository = new CategoryRepository(new Domain.ErpDbContext());
            var selectListItems = new List<SelectListItem>();

            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }

            try
            {
                var q = categoryRepository.GetCategoryByCode(sCode);

                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name + " - " + i.Value;
                    if (sValueField != null && sValueField == "Name")
                        item.Value = i.Name;
                    else if (sValueField != null && sValueField == "Value")
                        item.Value = i.Value;
                    else
                        item.Value = i.Value;

                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", SelectedValue);

            return selectList;
        }

        public static SelectList GetSelectList_UserbyCreateModuel(object sValue, string ActionName, string ModuelName, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
            UserTypePageRepository userTypePageRepository = new UserTypePageRepository(new Domain.ErpDbContext());
            PageRepository pageRepository = new PageRepository(new Domain.ErpDbContext());

            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }

            try
            {
                var page = pageRepository.GetPageByAcctionController(ActionName, ModuelName);
                var utype = userTypePageRepository.GetAllItem().Where(x => x.PageId == page.Id);
                var model = utype.Select(x => new Areas.Administration.Models.UserTypePageViewModel
                {
                    PageId = x.PageId,
                    UserTypeId = x.UserTypeId
                }).ToList();
                foreach (var i in model)
                {
                    var UserList = userRepository.GetUsers().Where(x => x.UserTypeId == i.UserTypeId)
               .Select(x => new SelectListItem
               {
                   Value = x.Id.ToString(),
                   Text = x.FullName

               }).ToList();
                    selectListItems = selectListItems.Union(UserList).ToList();
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_InternalNotifications(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            InternalNotificationsRepository internalNotificationsRepository = new InternalNotificationsRepository(new Domain.Staff.ErpStaffDbContext());

            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = internalNotificationsRepository.GetAllInternalNotifications().OrderByDescending(item => item.ModifiedDate);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Titles;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_TemplatePrint(object sValue, string ModelName, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            TemplatePrintRepository templatePrintRepository = new TemplatePrintRepository(new Domain.Sale.ErpSaleDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = templatePrintRepository.GetAllTemplatePrint().Where(x => x.Code.Contains(ModelName)).OrderBy(item => item.CreatedDate);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Title;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_WarehouseLocationItem(object sValue, int? ProductId)
        {
            var selectListItems = new List<SelectListItem>();
            WarehouseLocationItemRepository ContactRepository = new WarehouseLocationItemRepository(new Domain.Sale.ErpSaleDbContext());
            SelectListItem itemEmpty = new SelectListItem();
            itemEmpty.Text = App_GlobalResources.Wording.Empty;
            itemEmpty.Value = null;
            selectListItems.Add(itemEmpty);
            try
            {
                var q = ContactRepository.GetAllLocationItem().Where(x => x.ProductId == ProductId && x.IsOut != true).OrderBy(item => item.ExpiryDate).ToList();
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = "Tầng: " + i.Floor + "Kệ: " + i.Shelf + " - Ngày sản xuất:" + i.ExpiryDate.Value.ToString() + " - Lô sản xuất:" + i.LoCode;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_FullUserName(object sValue, string NullOrNameEmpty)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["BRANCH_ID_SPA_CookieName"];
            string strBrandID = "0";
            if (cookie != null)
            {
                strBrandID = cookie.Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }
            //get  CurrentUser.branchId

            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);

            var selectListItems = new List<SelectListItem>();
            UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = userRepository.GetAllUsers()
                    .Where(x => x.UserName != "admin"
                     && x.BranchId == intBrandID
                        )
                    .Select(item => new { item.Id, item.FullName })
                    .OrderBy(item => item.Id).ToList();
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.FullName;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_FullUserNameKD(object sValue, string NullOrNameEmpty)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["BRANCH_ID_SPA_CookieName"];
            string strBrandID = "0";
            if (cookie != null)
            {
                strBrandID = cookie.Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }
            //get  CurrentUser.branchId

            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);

            var selectListItems = new List<SelectListItem>();
            UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                if (intBrandID > 0)
                {
                    var q = userRepository.GetAllUsers()
                        .Where(x => x.UserName != "admin"
                         && x.BranchId == intBrandID && x.UserTypeId == 2026 && x.Status == UserStatus.Active
                            )
                        .Select(item => new { item.Id, item.FullName })
                        .OrderBy(item => item.Id).ToList();
                    foreach (var i in q)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = i.FullName;
                        item.Value = i.Id.ToString();
                        selectListItems.Add(item);
                    }
                }
                else
                {
                    var q = userRepository.GetAllUsers()
                       .Where(x => x.UserName != "admin"
                        && x.UserTypeId == 2026 && x.Status == UserStatus.Active
                           )
                       .Select(item => new { item.Id, item.FullName })
                       .OrderBy(item => item.Id).ToList();
                    foreach (var i in q)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = i.FullName;
                        item.Value = i.Id.ToString();
                        selectListItems.Add(item);
                    }
                }
                
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_Customer(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            CustomerRepository customerRepository = new CustomerRepository(new Domain.Account.ErpAccountDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = customerRepository.GetAllCustomer().Where(x => x.isLock == null || x.isLock == false).OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.CompanyName;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_Customer_byBrand(int? BranchId, object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            CustomerRepository customerRepository = new CustomerRepository(new Domain.Account.ErpAccountDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = customerRepository.GetAllCustomer().Where(x => x.BranchId == BranchId && (x.isLock == null || x.isLock == false)).OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.CompanyName;
                    item.Value = i.Code;
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_Setting(object sValue, string Code, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            SettingRepository settingRepository = new SettingRepository(new Domain.ErpDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = settingRepository.GetAll().Where(x => x.Code == Code).OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Note;
                    item.Value = i.Key.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_Warehouse(object sValue, string NullOrNameEmpty)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["BRANCH_ID_SPA_CookieName"];
            string strBrandID = "0";
            if (cookie != null)
            {
                strBrandID = cookie.Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }
            //get  CurrentUser.branchId

            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);

            var selectListItems = new List<SelectListItem>();
            WarehouseRepository warehouseRepository = new WarehouseRepository(new Domain.Sale.ErpSaleDbContext());
            UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = warehouseRepository.GetvwAllWarehouse().ToList();
                var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);

                if (intBrandID > 0)
                {
                    q = q.Where(x => x.BranchId == intBrandID).ToList();
                }
                if (!Filters.SecurityFilter.IsAdmin() && !Filters.SecurityFilter.IsKeToan())
                {
                    q = q.Where(x => ("," + user.WarehouseId + ",").Contains("," + x.Id + ",") == true).ToList();
                }
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code + " - " + i.Name + " (" + i.BranchName + ")";
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_Supplier(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            SupplierRepository supplierRepository = new SupplierRepository(new Domain.Sale.ErpSaleDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = supplierRepository.GetAllSupplier().OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.CompanyName;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_ServiceReminder(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            ServiceReminderRepository serviceReminderRepository = new ServiceReminderRepository(new Domain.Sale.ErpSaleDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = serviceReminderRepository.GetAllServiceReminder().OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_ComboName(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            ProductOrServiceRepository serviceRepository = new ProductOrServiceRepository(new Domain.Sale.ErpSaleDbContext());
            UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {

                var q = serviceRepository.GetAllvwService().Where(x => x.IsCombo == true);

                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code + " - " + i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_User(object sValue, string NullOrNameEmpty)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["BRANCH_ID_SPA_CookieName"];
            string strBrandID = "0";
            if (cookie != null)
            {
                strBrandID = cookie.Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }
            //get  CurrentUser.branchId

            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);

            var selectListItems = new List<SelectListItem>();
            UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }


            try
            {
                var q = userRepository.GetAllUsers().Where(x => x.Status == UserStatus.Active);
                if (intBrandID > 0)
                {
                    q = q.Where(x => x.BranchId == intBrandID);
                }
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    //item.Text = i.UserCode + " - " + i.UserName;
                    item.Text = i.FullName;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_UserPosition(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }


            try
            {
                var q = userRepository.GetAllUsersPosititon().Where(x => x.Status == UserStatus.Active).ToList();
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.FullName;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }


        public static SelectList GetSelectList_Service(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            ProductOrServiceRepository serviceRepository = new ProductOrServiceRepository(new Domain.Sale.ErpSaleDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = serviceRepository.GetAllvwService().Where(x => x.NotUse != true).OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code + " - " + i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_KPICatalog(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            KPICatalogRepository KPICatalogRepository = new KPICatalogRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = KPICatalogRepository.GetAllKPICatalog().ToList();
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_UserbyType(string UserTypeName, object sValue, string NullOrNameEmpty)
        {
            UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = App_GlobalResources.Wording.Empty;
                itemEmpty.Value = null;

                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = userRepository.GetAllvwUsers().OrderBy(item => item.Id).ToList();
                if (!string.IsNullOrEmpty(UserTypeName))
                {
                    q = q.Where(x => x.UserTypeName.Trim().ToLower() == UserTypeName.Trim()).ToList();
                }
                //if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin())
                //{
                //    q = q.Where(x => ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + x.DrugStore + ",") == true).ToList();
                //}

                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.FullName;
                    item.Value = i.Id.ToString();

                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_Staff_FingerPrint(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            FPMachineRepository fPMachineRepository = new FPMachineRepository(new Domain.Staff.ErpStaffDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = fPMachineRepository.GetAllFingerPrint().GroupBy(x => x.UserId)
                    .Select(item => new { item.Key, item.FirstOrDefault().Name })
                    .OrderBy(item => item.Name).ToList();
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Key.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_BranchAllNew(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                BranchRepository branchRepository = new BranchRepository(new Domain.Staff.ErpStaffDbContext());

                var q = branchRepository.GetAllBranch().Where(x => x.ParentId == null || x.ParentId <= 0)
                    .Select(item => new { item.Id, item.Name })
                    .ToList()
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }).OrderBy(item => item.Text).ToList();

                selectListItems.AddRange(q);

            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_DepartmentbyBranch(int? BranchId, object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                BranchRepository branchRepository = new BranchRepository(new Domain.Staff.ErpStaffDbContext());

                var q = branchRepository.GetAllBranch().Where(x => x.ParentId == BranchId)
                    .Select(item => new { item.Id, item.Name })
                    .ToList()
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }).OrderBy(item => item.Text).ToList();

                selectListItems.AddRange(q);

            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_DepartmentAllNew(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                BranchRepository branchRepository = new BranchRepository(new Domain.Staff.ErpStaffDbContext());
                var list = branchRepository.GetAllBranch().Where(x => x.ParentId != null || x.ParentId.Value > 0 && (x.IsDeleted == null || x.IsDeleted == false)).ToList();
                //nếu k phải là admin thì lấy danh sách Chi nhánh theo phân quyền trong user
                //if (!Filters.SecurityFilter.IsAdmin() && !Filters.SecurityFilter.IsKeToan())
                //{
                //    list = list.Where(x => ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + x.Id + ",") == true).ToList();
                //}
                var q = list.Select(item => new { item.Id, item.Name })

                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }).OrderBy(item => item.Text).ToList();

                selectListItems.AddRange(q);

            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_DepartmentByList(string ArrayList, object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            List<string> ListArrayID = new List<string>();
            if (!string.IsNullOrEmpty(ArrayList))
                ListArrayID = ArrayList.Split(',').ToList();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                BranchRepository branchRepository = new BranchRepository(new Domain.Staff.ErpStaffDbContext());
                var list = branchRepository.GetAllBranch().Where(x => x.ParentId == null && (x.IsDeleted == null || x.IsDeleted == false)).ToList();
                list = list.Where(id1 => ListArrayID.Any(id2 => id2.ToString() == id1.Id.ToString())).ToList();
                var q = list.Select(item => new { item.Id, item.Name })
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }).OrderBy(item => item.Text).ToList();

                selectListItems.AddRange(q);

            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_Position(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {


                PositionRepository positionRepository = new PositionRepository(new Domain.Staff.ErpStaffDbContext());
                var list = positionRepository.GetAllPosition().ToList();

                var q = list.Select(item => new { item.Id, item.Name })
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }).OrderBy(item => item.Text).ToList();

                selectListItems.AddRange(q);

            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }


        public static SelectList GetSelectList_Staff(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                StaffsRepository branchRepository = new StaffsRepository(new Domain.Staff.ErpStaffDbContext());
                var list = branchRepository.GetAllStaffs().ToList();
                var q = list.Select(item => new { item.Id, item.Name })
                    .Select(item => new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }).OrderBy(item => item.Text).ToList();

                selectListItems.AddRange(q);

            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static List<BranchViewModel> GetSelectAllDepartment()
        {
            var list = new List<BranchViewModel>();

            try
            {
                BranchRepository branchRepository = new BranchRepository(new Domain.Staff.ErpStaffDbContext());

                list = branchRepository.GetAllBranch().Where(x => x.ParentId != null || x.ParentId > 0 && (x.IsDeleted == null || x.IsDeleted == false))
                    .Select(item => new BranchViewModel
                    {
                        Name = item.Name,
                        Id = item.Id,
                        ParentId = item.ParentId,
                        CityId = item.CityId,
                        DistrictId = item.DistrictId,
                        WardId = item.WardId,
                        Phone = item.Phone,
                        Email = item.Email,
                        Code = item.Code
                    }).ToList();
            }
            catch { }


            return list;
        }

        public static SelectList GetSelectList_WarehousebyDrugStore(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            WarehouseRepository warehouseRepository = new WarehouseRepository(new Domain.Sale.ErpSaleDbContext());
            UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
                var q = warehouseRepository.GetvwAllWarehouse().ToList();
                //if (!Filters.SecurityFilter.IsAdmin() && !Filters.SecurityFilter.IsKeToan())
                //{
                //    q = q.Where(x => ("," + user.DrugStore + ",").Contains("," + x.BranchId + ",") == true).ToList();
                //}
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Code + " - " + i.Name + " (" + i.BranchName + ")";
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static List<PositionViewModel> GetSelectAllPosition()
        {
            var list = new List<PositionViewModel>();

            try
            {
                PositionRepository positionRepository = new PositionRepository(new Domain.Staff.ErpStaffDbContext());

                list = positionRepository.GetAllPosition()
                    .Select(item => new PositionViewModel
                    {
                        Name = item.Name,
                        Id = item.Id,
                        CommissionPercent = item.CommissionPercent,
                        MinimumRevenue = item.MinimumRevenue,
                        Code = item.Code
                    }).ToList();
            }
            catch { }


            return list;
        }

        public static SelectList GetSelectList_UserType(object sValue, string NullOrNameEmpty)
        {
            UserTypeRepository userTypeRepository = new UserTypeRepository(new Domain.ErpDbContext());
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = App_GlobalResources.Wording.Empty;
                itemEmpty.Value = null;

                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = userTypeRepository.GetAll().OrderBy(item => item.Id).ToList();
                //if (Erp.BackOffice.Filters.SecurityFilter.IsAdminDrugStore())
                //{
                //    q = q.Where(x => (x.Code == "ADS" || x.Code == "SDS")).ToList();
                //}
                //if (Erp.BackOffice.Filters.SecurityFilter.IsStaffDrugStore())
                //{
                //    q = q.Where(x => x.Code == "SDS").ToList();
                //}

                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Id.ToString();

                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_Rating(object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            LoyaltyPointRepository serviceRepository = new LoyaltyPointRepository(new Domain.Sale.ErpSaleDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = serviceRepository.GetAllLoyaltyPoint().OrderBy(item => item.Id).ToList();
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static List<vwUsers> GetSelectvwAllUser()
        {
            var list = new List<vwUsers>();

            try
            {
                // lấy cookie chi nhánh
                HttpCookie cookie = HttpContext.Current.Request.Cookies["BRANCH_ID_SPA_CookieName"];
                string strBrandID = "0";
                if (cookie != null)
                {
                    strBrandID = cookie.Value;
                    if (strBrandID == "")
                    {
                        strBrandID = "0";
                    }
                }
                //get  CurrentUser.branchId

                if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
                {
                    strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
                }

                int? intBrandID = int.Parse(strBrandID);

                UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
                list = userRepository.GetAllvwUsers().ToList();
                list = list.Where(x => x.BranchId == intBrandID || x.BranchId < 2000).ToList();
                list = list.Where(x => x.Status == UserStatus.Active).ToList();
            }
            catch { }


            return list;
        }

        public static List<UserType> GetSelectUserType()
        {
            var list = new List<UserType>();

            try
            {
                UserTypeRepository userTypeRepository = new UserTypeRepository(new Domain.ErpDbContext());
                list = userTypeRepository.GetAll().ToList();
                //if (Erp.BackOffice.Filters.SecurityFilter.IsAdminDrugStore())
                //{
                //    list = list.Where(x => (x.Code == "ADS" || x.Code == "SDS")).ToList();
                //}
                //if (Erp.BackOffice.Filters.SecurityFilter.IsStaffDrugStore())
                //{
                //    list = list.Where(x => x.Code == "SDS").ToList();
                //}
            }
            catch { }


            return list;
        }

        //public static SelectList GetSelectList_CityByUser(string sParentId, object sValue, string NullOrNameEmpty)
        //{
        //    var selectListItems = new List<SelectListItem>();
        //    LocationRepository locationRepository = new LocationRepository(new Domain.ErpDbContext());
        //    BranchRepository branchRepository = new BranchRepository(new Domain.Staff.ErpStaffDbContext());
        //    UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
        //    if (NullOrNameEmpty != null)
        //    {
        //        SelectListItem itemEmpty = new SelectListItem();
        //        itemEmpty.Text = NullOrNameEmpty;
        //        itemEmpty.Value = null;
        //        selectListItems.Add(itemEmpty);
        //    }

        //    try
        //    {
        //        var user = userRepository.GetByvwUserName(Erp.BackOffice.Helpers.Common.CurrentUser.UserName);
        //        var _branch = branchRepository.GetAllBranch().ToList();
        //        var q = locationRepository.GetList(sParentId);
        //        if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin() && !Erp.BackOffice.Filters.SecurityFilter.IsKeToan() && !Erp.BackOffice.Filters.SecurityFilter.IsQLKhoTong())
        //        {
        //            // q = q.Where(x => ("," + user.DrugStore + ",").Contains("," + x.BranchId + ",") == true);
        //            List<string> branchList = new List<string>();
        //            if (!string.IsNullOrEmpty(user.DrugStore))
        //                branchList = user.DrugStore.Split(',').ToList();
        //            var branchL = _branch.Where(id1 => branchList.Any(id2 => Convert.ToInt32(id2.ToString()) == id1.Id)).ToList();
        //            if (branchL.Count() > 0)
        //            {
        //                q = q.Where(id1 => branchL.Any(id2 => !string.IsNullOrEmpty(id2.CityId)&&id2.CityId == id1.Id)).ToList();
        //            }
        //        }

        //        foreach (var i in q)
        //        {
        //            SelectListItem item = new SelectListItem();
        //            item.Text = Erp.BackOffice.Helpers.Common.Capitalize(i.Type.ToLower() + " " + i.Name.ToLower());
        //            item.Value = i.Id;
        //            selectListItems.Add(item);
        //        }
        //    }
        //    catch { }

        //    var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

        //    return selectList;
        //}

        public static List<vwUsers> GetSelectTrinhDuocVien()
        {
            var list = new List<vwUsers>();

            try
            {
                UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
                list = userRepository.GetAllvwUsers().Where(x => x.UserTypeCode == "TDV").ToList();

                //if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin() && !Erp.BackOffice.Filters.SecurityFilter.IsKeToan())
                //{
                //    list = list.Where(x => ("," + Helpers.Common.CurrentUser.DrugStore + ",").Contains("," + x.DrugStore + ",") == true).ToList();
                //}
            }
            catch { }


            return list;
        }

        public static List<BranchViewModel> GetSelectAllDepartmentbyUser()
        {
            var list = new List<BranchViewModel>();

            try
            {
                BranchRepository branchRepository = new BranchRepository(new Domain.Staff.ErpStaffDbContext());
                UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
                var user = userRepository.GetByvwUserName(Helpers.Common.CurrentUser.UserName);
                list = branchRepository.GetAllBranch().Where(x => x.ParentId != null || x.ParentId > 0 && (x.IsDeleted == null || x.IsDeleted == false))
                    .Select(item => new BranchViewModel
                    {
                        Name = item.Name,
                        Id = item.Id,
                        ParentId = item.ParentId,
                        CityId = item.CityId,
                        DistrictId = item.DistrictId,
                        WardId = item.WardId,
                        Phone = item.Phone,
                        Email = item.Email,
                        Code = item.Code
                    }).ToList();
                if (!Erp.BackOffice.Filters.SecurityFilter.IsAdmin() && !Erp.BackOffice.Filters.SecurityFilter.IsKeToan())
                {
                    list = list
                        //.Where(x => 
                        //("," + user.DrugStore + ",").Contains("," + x.Id + ",") == true
                        //)
                        .ToList();
                }
            }
            catch { }


            return list;
        }
        public static SelectList GetSelectList_CustomerbyUserManager(int? StaffId, object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            CustomerRepository customerRepository = new CustomerRepository(new Domain.Account.ErpAccountDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = customerRepository.GetAllCustomer().OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.CompanyName;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_Room(object sValue, string NullOrNameEmpty)
        {
            RoomRepository roomRepository = new RoomRepository(new Domain.Staff.ErpStaffDbContext());
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;

                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = roomRepository.GetAllRoom().Where(x => x.BranchId == Erp.BackOffice.Helpers.Common.CurrentUser.BranchId).OrderBy(item => item.Floor);

                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name + " (" + i.Floor + ")";
                    item.Value = i.Id.ToString();

                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_bed(object sValue, string NullOrNameEmpty)
        {
            // lấy cookie chi nhánh
            HttpCookie cookie = HttpContext.Current.Request.Cookies["BRANCH_ID_SPA_CookieName"];
            string strBrandID = "0";
            if (cookie != null)
            {
                strBrandID = cookie.Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }
            //get  CurrentUser.branchId

            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);

            RoomRepository roomRepository = new RoomRepository(new Domain.Staff.ErpStaffDbContext());
            BedRepository bedRepository = new BedRepository(new Domain.Staff.ErpStaffDbContext());
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;

                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = roomRepository.GetAllRoom().Where(x => x.BranchId == intBrandID).OrderBy(item => item.Floor).ToList();

                foreach (var i in q)
                {
                    var a = bedRepository.GetAllBedbyIdRoom(i.Id);
                    foreach (var j in a)
                    {
                        if (sValue == null) //create
                        {
                            //SelectListItem item = new SelectListItem();
                            //item.Text = j.Name_Bed + " (" + i.Floor + ")" + " (" + i.Name + ")";
                            //item.Value = j.Bed_ID.ToString();
                            //selectListItems.Add(item);
                            if (j.Trang_Thai == false)
                            {
                                SelectListItem item = new SelectListItem();
                                item.Text = j.Name_Bed + " (" + i.Floor + ")" + " (" + i.Name + ")";
                                item.Value = j.Bed_ID.ToString();
                                selectListItems.Add(item);
                            }
                        }
                        else //edit
                        {
                            if (j.Trang_Thai == false || sValue.ToString() == j.Bed_ID.ToString())
                            {
                                SelectListItem item = new SelectListItem();
                                item.Text = j.Name_Bed + " (" + i.Floor + ")" + " (" + i.Name + ")";
                                item.Value = j.Bed_ID.ToString();
                                selectListItems.Add(item);
                            }
                        }
                    }
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }
        public static List<Bed> GetListBedbyIdRoom(int Id)
        {
            BedRepository roomRepository = new BedRepository(new Domain.Staff.ErpStaffDbContext());
            List<Bed> listbed = roomRepository.GetAllBedbyIdRoom(Id);
            return listbed;
        }


        public static List<ProductViewModel> GetAllProductAndService()
        {
            try
            {
                ProductOrServiceRepository productRepository = new ProductOrServiceRepository(new Domain.Sale.ErpSaleDbContext());
                var list = productRepository.GetAllvwProductAndService().Select(item => new ProductViewModel
                {
                    Name = item.Name,
                    Id = item.Id,
                    Image_Name = item.Image_Name,
                    CategoryCode = item.CategoryCode,
                    Code = item.Code,
                    ProductGroup = item.ProductGroup,
                    ProductLinkId = item.ProductLinkId,
                    ProductLinkCode = item.ProductLinkCode,
                    ProductLinkName = item.ProductLinkName,
                    Type = item.Type,
                    Unit = item.Unit
                }).ToList();
                

                return list;
            }
            catch
            {
                return null;
            }


        }

        public static List<UserOnlineViewModel> GetListUserOnline()
        {
            var list = new List<UserOnlineViewModel>();

            try
            {

                //lấy cookie chi nhánh
                HttpCookie cookie = HttpContext.Current.Request.Cookies["BRANCH_ID_SPA_CookieName"];
                string strBrandID = "0";
                if (cookie != null)
                {
                    strBrandID = cookie.Value;
                    if (strBrandID == "")
                    {
                        strBrandID = "0";
                    }
                }
                //get  CurrentUser.branchId

                if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
                {
                    strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
                }

                int? intBrandID = int.Parse(strBrandID);

                UserRepository userRepository = new UserRepository(new Domain.ErpDbContext());
                CheckInOutRepository checkInOutRepository = new CheckInOutRepository(new Domain.Staff.ErpStaffDbContext());
                StaffMadeRepository staffMadeRepository = new StaffMadeRepository(new Domain.Sale.ErpSaleDbContext());
                //
                var data = SqlHelper.QuerySP<vwUsers>("TOIUUCSKH_AllvwUsers").ToList();
				var branchId = intBrandID; //Erp.BackOffice.Helpers.Common.CurrentUser.BranchId;
                var time_now = DateTime.Now.ToShortDateString();
                data = data.Where(x => x.BranchId == branchId).ToList();
                var check_in_out = SqlHelper.QuerySP<vwCheckInOut>("TOIUUCSKH_CheckInOut", new { @pBranchId =branchId}).ToList();
                check_in_out = check_in_out.Where(x => x.TimeDate.Value.ToShortDateString() == time_now).ToList();
                //
                list = data.Select(x => new UserOnlineViewModel
                {
                    Id = x.Id,
                    ProfileImage = x.ProfileImage,
                    FullName = x.FullName,
                    UserCode = x.UserCode,
                    UserTypeId = x.UserTypeId,
                    UserTypeName = x.UserTypeName,
                    Status = "no_shift"
                }).ToList();

                //đếm số lượt mà nhân viên đã làm trong ngày
                var staffMade = staffMadeRepository.GetvwAllStaffMade().AsEnumerable().Where(x => x.WorkDay != null && x.BranchId == branchId && x.WorkDay.Value.ToShortDateString() == time_now).ToList();
				//gắn số lượt nhân viên đã làm vào danh sách user đang online
				foreach (var item in list)
                {
                    if (staffMade.Any(x => x.UserId == item.Id))
                    {
                        item.Count_Scheduling = staffMade.Where(x => x.UserId == item.Id).Count();
                    }
                    var _inOut = check_in_out.Where(x => x.UserId == item.Id).OrderBy(x => x.TimeType).ThenByDescending(x => x.TimeStr).ToList();
                    if (_inOut.Any())
                    {
                        if (_inOut.Any(x => x.TimeType == "In"))
                        {
                            item.InTime = _inOut.FirstOrDefault(x => x.TimeType == "In").TimeStr;
                            item.Status = "inprocess";
                        }
                        if (_inOut.Any(x => x.TimeType == "Out"))
                        {
                            item.OutTime = _inOut.FirstOrDefault(x => x.TimeType == "Out").TimeStr;
                            item.Status = "no_shift";
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return list;
        }
        public static SelectList GetSelectList_FullLoyaltyName(object sValue, string NullOrNameEmpty)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["BRANCH_ID_SPA_CookieName"];
            string strBrandID = "0";
            if (cookie != null)
            {
                strBrandID = cookie.Value;
                if (strBrandID == "")
                {
                    strBrandID = "0";
                }
            }
            //get  CurrentUser.branchId

            if ((Helpers.Common.CurrentUser.BranchId != null) && (Helpers.Common.CurrentUser.BranchId > 0))
            {
                strBrandID = Helpers.Common.CurrentUser.BranchId.ToString();
            }

            int? intBrandID = int.Parse(strBrandID);

            var selectListItems = new List<LoyaltyPointViewModel>();
            LoyaltyPointRepository loyaltyPointRepository = new LoyaltyPointRepository(new Domain.Sale.ErpSaleDbContext());
            if (NullOrNameEmpty != null)
            {
                LoyaltyPointViewModel itemEmpty = new LoyaltyPointViewModel();
                itemEmpty.Name = NullOrNameEmpty;
                itemEmpty.Id = 0;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = loyaltyPointRepository.GetAllLoyaltyPoint()
                    .Select(item => new { item.Id, item.Name })
                    .OrderBy(item => item.Id).ToList();
                foreach (var i in q)
                {
                    LoyaltyPointViewModel item = new LoyaltyPointViewModel();
                    item.Id = i.Id;
                    item.Name = i.Name;
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Id", "Name", sValue);

            return selectList;
        }
        public static SelectList GetSelectList_AllNguonKhach(object sValue, string NullOrNameEmpty)
        {
            

            Domain.Repositories.CategoryRepository categoryRepository = new CategoryRepository(new ErpDbContext());

            var selectListItems = new List<SelectListItem>();

            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = App_GlobalResources.Wording.Empty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }

            try
            {
                var q = categoryRepository.GetCategoryByCode("NguonKhach");

                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Name;
                    item.Value = i.Value;
                    selectListItems.Add(item);
                }
            }
            catch (Exception ex)
            {

            }
            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        public static SelectList GetSelectList_Customer_UseMobile(int? BranchId, object sValue, string NullOrNameEmpty)
        {
            var selectListItems = new List<SelectListItem>();
            CustomerRepository customerRepository = new CustomerRepository(new Domain.Account.ErpAccountDbContext());
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            try
            {
                var q = customerRepository.GetAllCustomer().Where(x => !String.IsNullOrEmpty(x.PlayId) &&  x.BranchId == BranchId && (x.isLock == null || x.isLock == false)).OrderBy(item => item.Id);
                foreach (var i in q)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.CompanyName + "-" + i.Code;
                    item.Value = i.Id.ToString();
                    selectListItems.Add(item);
                }
            }
            catch { }

            var selectList = new SelectList(selectListItems, "Value", "Text", sValue);

            return selectList;
        }

        //Lấy list typetemplate cho Crm_Template
        public static SelectList GetSelectList_CrmTemplate(string NullOrNameEmpty = "Chọn loại tin nhắn")
        {
            var selectListItems = new List<SelectListItem>();
            if (NullOrNameEmpty != null)
            {
                SelectListItem itemEmpty = new SelectListItem();
                itemEmpty.Text = NullOrNameEmpty;
                itemEmpty.Value = null;
                selectListItems.Add(itemEmpty);
            }
            selectListItems.Add(new SelectListItem { Value = "1", Text = "SMS" });
            selectListItems.Add(new SelectListItem { Value = "2", Text = "Zalo" });
            selectListItems.Add(new SelectListItem { Value = "3", Text = "Email" });


            var selectList = new SelectList(selectListItems, "Value", "Text");

            return selectList;
        }
        public static SelectList GetTypeLead_CrmTemplate()
        {
            var selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem { Value = "false", Text = "Contact" });
            selectListItems.Add(new SelectListItem { Value = "true", Text = "Lead" });
            var selectList = new SelectList(selectListItems, "Value", "Text");

            return selectList;
        }
    }
}