using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class AssignRights : System.Web.UI.Page
    {
        protected string UserId = null;
        private Dictionary<int, int> SubmenuToParentMenuMap
        {
            get
            {
                if (ViewState["SubmenuToParentMenuMap"] == null)
                {
                    ViewState["SubmenuToParentMenuMap"] = new Dictionary<int, int>();
                }
                return (Dictionary<int, int>)ViewState["SubmenuToParentMenuMap"];
            }
            set
            {
                ViewState["SubmenuToParentMenuMap"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("~/view/authentication/login.aspx", false);
                    return;
                }
                BindRoles();
                BindMenu("Bindmenu","");
            }
        }

        protected void ddlrole_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Clear all checkboxes first
                foreach (ListItem item in cbxMenu.Items)
                {
                    item.Selected = false;
                }
                foreach (ListItem item in cbx_submenu.Items)
                {
                    item.Selected = false;
                }
                lbl_submenu.Visible = false;
                SubmenuToParentMenuMap.Clear();

                int roleId;
                if (int.TryParse(ddlrole.SelectedValue, out roleId) && roleId > 0)
                {
                    assignRightsBL assignrights = new assignRightsBL();
                    List<rightsDO> assignedRights = assignrights.GetAssignedRightsForCheckbox(roleId);

                    if (assignedRights != null && assignedRights.Count > 0)
                    {
                        // Auto-select menus
                        foreach (rightsDO right in assignedRights)
                        {
                            if (right.menuid > 0)
                            {
                                foreach (ListItem item in cbxMenu.Items)
                                {
                                    if (item.Value == right.menuid.ToString())
                                    {
                                        item.Selected = true;
                                        break;
                                    }
                                }
                            }
                        }

                        // Trigger menu selection to load submenus
                        cbxMenu_SelectedIndexChanged(sender, e);

                        // Auto-select submenus
                        foreach (rightsDO right in assignedRights)
                        {
                            if (right.submenuid > 0)
                            {
                                foreach (ListItem item in cbx_submenu.Items)
                                {
                                    if (item.Value == right.submenuid.ToString())
                                    {
                                        item.Selected = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AssignRights", "ddlrole_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        public void BindRoles()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownroles();
                if (list1 != null)
                {
                    ddlrole.DataSource = list1;
                    ddlrole.DataTextField = "Text";
                    ddlrole.DataValueField = "Id";
                }
                else
                {
                    ddlrole.DataSource = null;
                }
                ddlrole.DataBind();
                ddlrole.Items.Insert(0, new ListItem("--Select Role--", "0"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AssignRights", "BindRoles", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

        }
        public void BindMenu(string type, string menuId)
        {
            try
            {
                List<DropDownData> list1 = new List<DropDownData>();
                CommonBL commonbl = new CommonBL();
                list1 = commonbl.dropdownMenu(type, menuId);
                if (list1 != null)
                {
                    cbxMenu.DataSource = list1;
                    cbxMenu.DataTextField = "Text";
                    cbxMenu.DataValueField = "Id";
                }
                else
                {
                    cbxMenu.DataSource = null;
                }
                cbxMenu.DataBind();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "BindMenu", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void cbxMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Get all selected menu IDs
                List<string> selectedMenuIds = new List<string>();
                foreach (ListItem item in cbxMenu.Items)
                {
                    if (item.Selected)
                    {
                        selectedMenuIds.Add(item.Value);
                    }
                }

                // Clear existing submenus and the map
                cbx_submenu.Items.Clear();
                SubmenuToParentMenuMap.Clear();

                if (selectedMenuIds.Count > 0)
                {
                    // Get submenus for each selected menu
                    CommonBL commonbl = new CommonBL();
                    foreach (string menuId in selectedMenuIds)
                    {
                        List<DropDownData> submenus = commonbl.dropdownSubMenu("Bindsubmenu", menuId);
                        if (submenus != null)
                        {
                            foreach (DropDownData submenu in submenus)
                            {
                                // Check if we already added this submenu
                                bool exists = false;
                                foreach (ListItem item in cbx_submenu.Items)
                                {
                                    if (item.Value == submenu.Id.ToString())
                                    {
                                        exists = true;
                                        break;
                                    }
                                }
                                if (!exists)
                                {
                                    cbx_submenu.Items.Add(new ListItem(submenu.Text, submenu.Id.ToString()));
                                    SubmenuToParentMenuMap[submenu.Id] = Convert.ToInt32(menuId);
                                }
                            }
                        }
                    }
                    if (cbx_submenu.Items.Count > 0)
                    {
                        lbl_submenu.Visible = true;
                    }
                    else
                    {
                        lbl_submenu.Visible = false;
                    }
                }
                else
                {
                    lbl_submenu.Visible = false;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "cbxMenu_SelectedIndexChanged", "Exception Message=" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        private void BindSubMenus(string type, string menuId)
        {
            try
            {
                cbx_submenu.Items.Clear();

                CommonBL commonbl = new CommonBL();
                List<DropDownData> list1 = commonbl.dropdownSubMenu(type, menuId);

                if (list1 != null && list1.Count > 0)
                {
                    cbx_submenu.DataSource = list1;
                    cbx_submenu.DataTextField = "Text";
                    cbx_submenu.DataValueField = "Id";
                    cbx_submenu.DataBind();
                    lbl_submenu.Visible = true;
                }
                else
                {
                    lbl_submenu.Visible = false;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("viewAssignRights", "BindSubMenus", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private int GetClientIdFromSession()
        {
            int userId = 0;
            if (Session["UserId"] != null)
            {
                userId = Convert.ToInt32(Session["UserId"]);
            }
            return userId;
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/viewAssignRights.aspx", false);
            return;
        }
        protected void btn_reset_Click(object sender, EventArgs e)
        {
            try
            {
                ddlrole.SelectedIndex = 0;
                // Clear menu checkboxes
                foreach (ListItem item in cbxMenu.Items)
                {
                    item.Selected = false;
                }
                // Clear submenu checkboxes
                foreach (ListItem item in cbx_submenu.Items)
                {
                    item.Selected = false;
                }
                lbl_submenu.Visible = false;
                SubmenuToParentMenuMap.Clear();
                
                // Clear the "Select All" checkboxes using JavaScript
                string script = @"
                    document.getElementById('selectAllMenus').checked = false;
                    var selectAllSubmenus = document.getElementById('selectAllSubmenus');
                    if (selectAllSubmenus) {
                        selectAllSubmenus.checked = false;
                    }
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "ClearSelectAll", script, true);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AssignRights", "btn_reset_Click", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected void btn_submit_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = GetClientIdFromSession();
                assignRightsBL assignrights = new assignRightsBL();
                int roleId = Convert.ToInt32(ddlrole.SelectedValue);

                List<ListItem> selectedMenus = cbxMenu.Items.Cast<ListItem>().Where(i => i.Selected).ToList();
                List<ListItem> selectedSubmenus = cbx_submenu.Items.Cast<ListItem>().Where(i => i.Selected).ToList();

                // Distinct validation message when nothing was picked at all, instead of
                // letting it fall through to the generic assignment-failure message.
                if (selectedMenus.Count == 0 && selectedSubmenus.Count == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "RightsSavedScript",
                        "showRightSavedMessage('Error', 'Please select at least one menu or submenu before assigning rights.');", true);
                    return;
                }

                int successCount = 0;
                List<string> failureReasons = new List<string>();

                // First, insert selected menus without submenus
                foreach (ListItem menuItem in selectedMenus)
                {
                    rightsDO rightsDO = new rightsDO();
                    rightsDO.Insertedby = userId;
                    rightsDO.roleid = roleId;
                    rightsDO.menuid = Convert.ToInt32(menuItem.Value);
                    rightsDO.submenuid = 0;

                    List<rightsDO> rights = assignrights.SaveRights(rightsDO);
                    if (rights != null && rights.Count > 0 && rights[0].Status == "Success")
                    {
                        successCount++;
                    }
                    else
                    {
                        string reason = rights != null && rights.Count > 0 && !string.IsNullOrWhiteSpace(rights[0].Remarks)
                            ? rights[0].Remarks
                            : "Unknown error.";
                        failureReasons.Add("Menu '" + menuItem.Text + "': " + reason);
                    }
                }

                // Then, insert selected submenus
                foreach (ListItem submenuItem in selectedSubmenus)
                {
                    int submenuId = Convert.ToInt32(submenuItem.Value);
                    int menuId = 0;

                    if (SubmenuToParentMenuMap.ContainsKey(submenuId))
                    {
                        menuId = SubmenuToParentMenuMap[submenuId];
                    }

                    rightsDO rightsDO = new rightsDO();
                    rightsDO.Insertedby = userId;
                    rightsDO.roleid = roleId;
                    rightsDO.menuid = menuId;
                    rightsDO.submenuid = submenuId;

                    List<rightsDO> rights = assignrights.SaveRights(rightsDO);
                    if (rights != null && rights.Count > 0 && rights[0].Status == "Success")
                    {
                        successCount++;
                    }
                    else
                    {
                        string reason = rights != null && rights.Count > 0 && !string.IsNullOrWhiteSpace(rights[0].Remarks)
                            ? rights[0].Remarks
                            : "Unknown error.";
                        failureReasons.Add("Submenu '" + submenuItem.Text + "': " + reason);
                    }
                }

                int totalSelected = selectedMenus.Count + selectedSubmenus.Count;
                string status;
                string remark;

                if (failureReasons.Count == 0)
                {
                    status = "Success";
                    remark = "Rights assigned successfully!";
                }
                else if (successCount > 0)
                {
                    // Partial failure - some of the selected rights were saved, but not
                    // all. Report the actual reason(s) instead of the old generic
                    // "Some rights could not be assigned!" with no explanation.
                    status = "Error";
                    remark = successCount + " of " + totalSelected + " rights assigned. " + string.Join(" ", failureReasons);
                }
                else
                {
                    status = "Error";
                    remark = string.Join(" ", failureReasons);
                }

                string safeStatus = HttpUtility.JavaScriptStringEncode(status);
                string safeRemark = HttpUtility.JavaScriptStringEncode(remark);

                ClientScript.RegisterStartupScript(this.GetType(), "RightsSavedScript",
                               "showRightSavedMessage('" + safeStatus + "', '" + safeRemark + "');" +
                               "setTimeout(function(){ window.location.href = 'viewAssignRights.aspx'; }, 5000);", true);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AssignRights", "btn_submit_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
    }
}