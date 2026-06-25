using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class AppraisalDetails : System.Web.UI.Page
    {
        public class ComponentSaveResponse
        {
            public bool success { get; set; }
            public string message { get; set; }
            public int id { get; set; }
            public string text { get; set; }
            public string componentType { get; set; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static ComponentSaveResponse SaveNewSalaryComponent(string componentName, string componentType)
        {
            try
            {
                string safeName = Convert.ToString(componentName ?? string.Empty).Trim();
                string safeType = Convert.ToString(componentType ?? string.Empty).Trim().ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(safeName))
                {
                    return new ComponentSaveResponse { success = false, message = "Component name is required." };
                }
                if (safeType != "EARNING" && safeType != "DEDUCTION")
                {
                    return new ComponentSaveResponse { success = false, message = "Invalid component type." };
                }

                int insertedBy = 0;
                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                {
                    int.TryParse(Convert.ToString(HttpContext.Current.Session["userId"] ?? "0"), out insertedBy);
                }

                SalarySlipBL bl = new SalarySlipBL();
                UserDetailsDO result = bl.InsertSalaryComponent(safeName, safeType, insertedBy);
                bool ok = result != null && string.Equals(Convert.ToString(result.Status), "Success", StringComparison.OrdinalIgnoreCase);

                return new ComponentSaveResponse
                {
                    success = ok,
                    message = result != null ? Convert.ToString(result.Remarks) : "Unable to save component.",
                    id = result != null ? result.UserId : 0,
                    text = safeName,
                    componentType = safeType
                };
            }
            catch (Exception ex)
            {
                return new ComponentSaveResponse { success = false, message = ex.Message };
            }
        }

        protected string UserId = null;
        private static bool IsSaveSuccessful(string status, string remark)
        {
            string normalizedStatus = (status ?? string.Empty).Trim();
            string normalizedRemark = (remark ?? string.Empty).Trim();

            return string.Equals(normalizedStatus, "Success", StringComparison.OrdinalIgnoreCase)
                || normalizedRemark.IndexOf("created", StringComparison.OrdinalIgnoreCase) >= 0
                || normalizedRemark.IndexOf("saved", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            int userId = 0;
            //BindComponents();
            if (!IsPostBack)
            {

                if (Session["userId"] == null)
                {
                    Response.Redirect("~/view/authentication/login.aspx", false);
                    return;
                }

                if (Request.QueryString["user_id"] != null)
                {
                    userId = Convert.ToInt32(Request.QueryString["user_id"]);
                }
                else
                {
                    userId = 0;
                }
                BindAppraisalGrid();
                //BindYears();
                //BindMonths();
                //BindSavedComponentsForEdit();


            }
        }
        //protected override void OnPreRender(EventArgs e)
        //{
        //    if (ddl_componentModal.Items.Count <= 1)
        //    {
        //        BindComponents();
        //    }
        //    base.OnPreRender(e);
        //}
        //public void BindComponents()
        //{
        //    List<DropDownData> list1 = new List<DropDownData>();
        //    CommonBL commonbl = new CommonBL();
        //    try
        //    {
        //        list1 = commonbl.dropdownComponent_Forremuneration();
        //        if (list1 != null && list1.Count > 0)
        //        {
        //            ddl_component.DataSource = list1;
        //            ddl_component.DataTextField = "Text";
        //            ddl_component.DataValueField = "Id";

        //            ddl_componentModal.DataSource = list1;
        //            ddl_componentModal.DataTextField = "Text";
        //            ddl_componentModal.DataValueField = "Id";
        //        }
        //        else
        //        {
        //            ddl_component.DataSource = null;
        //            ddl_componentModal.DataSource = null;
        //        }
        //        ddl_component.DataBind();
        //        ddl_component.Items.Insert(0, new ListItem("-- Please Select --", ""));

        //        ddl_componentModal.DataBind();
        //        ddl_componentModal.Items.Insert(0, new ListItem("-- Please Select --", ""));

        //        var options = (list1 ?? new List<DropDownData>())
        //            .Select(comp => new
        //            {
        //                Id = comp.Id.ToString(),
        //                Text = comp.Text,
        //                ComponentType = (comp.ComponentType ?? string.Empty).Trim().ToUpper()
        //            })
        //            .ToList();

        //        hfComponentOptions.Value = new JavaScriptSerializer().Serialize(options);
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("Remunerationform", "BindComponents", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
        //        hfComponentOptions.Value = "[]";
        //    }
        //}

        //public void BindMonths()
        //{
        //    ddlMonth.Items.Clear();
        //    ddlMonth.Items.Add(new ListItem("-Select Month-", ""));

        //    for (int i = 1; i <= 12; i++)
        //    {
        //        ddlMonth.Items.Add(new ListItem(
        //            CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
        //            i.ToString()
        //        ));
        //    }
        //     //}

        //     public void BindYears()
        //     {
        //         ddlYear.Items.Clear();
        //         ddlYear.Items.Add(new ListItem("-Select Year-", ""));

        //         int currentYear = DateTime.Now.Year;

        //         for (int year = currentYear - 5; year <= currentYear + 5; year++)
        //         {
        //             ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
        //         }
        //     }


        //     protected void ddl_employeename_SelectedIndexChanged(object sender, EventArgs e)
        //     {
        //         if (!string.IsNullOrEmpty(ddl_username.SelectedValue))
        //         {
        //             int userId = Convert.ToInt32(ddl_username.SelectedValue);
        //             BindEmployeeCodeAndDesignation(userId);
        //             BindEmployeeSalary(userId);
        //         }
        //         else
        //         {
        //             ddl_username.Text = "";
        //             ddldesign.Items.Clear();
        //             ddldesign.Items.Insert(0, new ListItem("-- Select --", ""));
        //         }
        //     }

        //     protected void ddl_employeeCode_SelectedIndexChanged(object sender, EventArgs e)
        //     {
        //         if (!string.IsNullOrEmpty(ddl_empcode.SelectedValue))
        //         {
        //             int userId = Convert.ToInt32(ddl_empcode.SelectedValue);
        //             ListItem selectedUser = ddl_username.Items.FindByValue(ddl_empcode.SelectedValue);
        //             if (selectedUser != null)
        //             {
        //                 ddl_username.SelectedValue = ddl_empcode.SelectedValue;
        //             }
        //             BindEmployeeCodeAndDesignation(userId);
        //             BindEmployeeSalary(userId);
        //         }
        //     }
        //     private void BindEmployeeSalary(int userId)
        //     {
        //         SalarySlipBL bl = new SalarySlipBL();
        //         SalarySlipDO activeSlip = bl.GetLatestSavedSlipByUserId(userId);
        //         if (activeSlip != null)
        //         {
        //             decimal basic = activeSlip.BasicSalary;
        //             decimal hra = activeSlip.HouseRentAllowance;
        //             decimal special = activeSlip.SpecialAllowance;
        //             decimal professionalTax = activeSlip.ProfessionalTax;

        //             txtBasicSalary.Text = basic.ToString("0.00");
        //             txtHRA.Text = hra.ToString("0.00");
        //             txtSpecial.Text = special.ToString("0.00");
        //             txtProfessionalTax.Text = professionalTax.ToString("0.00");
        //             txtTotalEarnings.Text = activeSlip.TotalEarnings.ToString("0.00");
        //             txtTotalDeductions.Text = activeSlip.TotalDeductions.ToString("0.00");
        //             txtNetPay.Text = activeSlip.NetPay.ToString("0.00");

        //             ViewState["FullBasic"] = basic;
        //             ViewState["FullHRA"] = hra;
        //             ViewState["FullSpecial"] = special;
        //             ViewState["FullPT"] = professionalTax;

        //             int activeMonth = 0;
        //             int activeYear = activeSlip.Year;
        //             int.TryParse(Convert.ToString(activeSlip.Month), out activeMonth);
        //             BindActiveComponents(userId, activeMonth, activeYear);
        //             return;
        //         }

        //         // Hard stop: do not fallback to old master rows. This prevents stale 64000 values.
        //         ClearSalaryFields();
        //         ScriptManager.RegisterStartupScript(
        //             this,
        //             this.GetType(),
        //             "ActiveSlipMissing",
        //             "showsalarySavedMessage('error', 'Active salary slip not found for selected user. Please save remuneration again.');",
        //             true);
        //     }

        //     private void BindActiveComponents(int userId, int month, int year)
        //     {
        //         if (month <= 0 || year <= 0)
        //         {
        //             hfDynamicComponents.Value = string.Empty;
        //             return;
        //         }

        //         SalarySlipBL bl = new SalarySlipBL();
        //         var components = bl.GetRemunerationComponents(userId, month, year);
        //         if (components == null || components.Count == 0)
        //         {
        //             hfDynamicComponents.Value = string.Empty;
        //             return;
        //         }

        //         hfDynamicComponents.Value = new JavaScriptSerializer().Serialize(components);
        //         string js = @"
        //             (function(){
        //                 var earningBody = document.getElementById('earningsBody');
        //                 var deductionBody = document.getElementById('deductionsBody');
        //                 if (earningBody) {
        //                     Array.from(earningBody.querySelectorAll('tr.dynamic-component')).forEach(function(r){ r.remove(); });
        //                 }
        //                 if (deductionBody) {
        //                     Array.from(deductionBody.querySelectorAll('tr.dynamic-component')).forEach(function(r){ r.remove(); });
        //                 }
        //                 var data = " + hfDynamicComponents.Value + @";
        //                 data.forEach(function(c){
        //                     if (typeof createDynamicRow === 'function') {
        //                         createDynamicRow(c.ComponentName, parseFloat(c.Amount || 0), !!c.IsDeduction);
        //                     }
        //                 });
        //                 if (typeof calculateSalary === 'function') {
        //                     calculateSalary();
        //                 }
        //             })();";
        //         ScriptManager.RegisterStartupScript(this, this.GetType(), "LoadActiveComponents", js, true);
        //     }
        //     private void ClearSalaryFields()
        //     {
        //         txtBasicSalary.Text = "0.00";
        //         txtHRA.Text = "0.00";
        //         txtSpecial.Text = "0.00";
        //         txtProfessionalTax.Text = "0.00";

        //         txtTotalEarnings.Text = "0.00";
        //         txtTotalDeductions.Text = "0.00";
        //         txtNetPay.Text = "0.00";
        //     }



        //     protected void btnshowApprisal_Click(object sender, EventArgs e)
        //     {
        //         if (string.IsNullOrEmpty(ddl_username.SelectedValue))
        //         {
        //             ScriptManager.RegisterStartupScript(
        //                 this,
        //                 this.GetType(),
        //                 "UserValidation",
        //                 "showsalarySavedMessage('error', 'Please select a Employee Name');",
        //                 true
        //             );
        //             return;
        //         }
        //         int userId = Convert.ToInt32(ddl_username.SelectedValue);
        //         SalarySlipBL bl = new SalarySlipBL();

        //         DateTime? lastDate = bl.GetLastAppraisalDate(userId);

        //         if (lastDate.HasValue)
        //         {
        //             txtLastAppraisalDate.Text = lastDate.Value.ToString("dd-MM-yyyy");
        //         }

        //         pnlAppraisal.Visible = true;
        //         btnshowApprisal.Visible = false;
        //     }

        //     private void BindSavedComponentsForEdit()
        //     {
        //         try
        //         {
        //             string mode = Convert.ToString(Request.QueryString["mode"]);
        //             if (!string.Equals(mode, "edit", StringComparison.OrdinalIgnoreCase))
        //             {
        //                 return;
        //             }

        //             int userId;
        //             int month;
        //             int year;
        //             if (!int.TryParse(Convert.ToString(Request.QueryString["user_id"]), out userId)
        //                 || !int.TryParse(Convert.ToString(Request.QueryString["month"]), out month)
        //                 || !int.TryParse(Convert.ToString(Request.QueryString["year"]), out year))
        //             {
        //                 return;
        //             }

        //             SalarySlipBL bl = new SalarySlipBL();
        //             var components = bl.GetRemunerationComponents(userId, month, year);
        //             if (components == null || components.Count == 0)
        //             {
        //                 return;
        //             }

        //             hfDynamicComponents.Value = new JavaScriptSerializer().Serialize(components);

        //             string js = @"
        //                 (function(){
        //                     var data = " + hfDynamicComponents.Value + @";
        //                     data.forEach(function(c){
        //                         if (typeof createDynamicRow === 'function') {
        //                             createDynamicRow(c.ComponentName, parseFloat(c.Amount || 0), !!c.IsDeduction);
        //                         }
        //                     });
        //                     if (typeof calculateSalary === 'function') {
        //                         calculateSalary();
        //                     }
        //                 })();";

        //             ScriptManager.RegisterStartupScript(this, this.GetType(), "LoadSavedComponents", js, true);
        //         }
        //         catch (Exception ex)
        //         {
        //             CommonBL errorlog = new CommonBL();
        //             errorlog.fnStoreErrorLog("Remunerationform", "BindSavedComponentsForEdit", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
        //         }
        //     }


        //     private void BindEmployeeCodeAndDesignation(int userId)
        //     {
        //         UserDetailsBL userBL = new UserDetailsBL();
        //         var userData = userBL.ViewAllUsers().Where(x => x.UserId == userId).ToList();
        //         if (userData == null || userData.Count == 0)
        //         {
        //             userData = userBL.GetUserDetails(userId);
        //         }
        //         if (userData != null && userData.Count > 0)
        //         {
        //             var emp = userData.FirstOrDefault();
        //             ddl_empcode.Items.Clear();
        //             ddl_empcode.Items.Add(new ListItem(emp.EmployeeCode, userId.ToString()));
        //             ddl_empcode.SelectedValue = userId.ToString();

        //             ddldesign.Items.Clear();
        //             string designation = string.IsNullOrWhiteSpace(emp.designation_name) ? "-- Not Found --" : emp.designation_name;
        //             ddldesign.Items.Add(new ListItem(designation, designation));
        //             ddldesign.SelectedValue = designation;

        //             HttpContext.Current.Session["EmployeeCode"] = emp.EmployeeCode;
        //             HttpContext.Current.Session["CompanyId"] = emp.company_id;
        //         }
        //         else
        //         {
        //             ddl_empcode.Items.Clear();
        //             ddl_empcode.Items.Insert(0, new ListItem("-- Not Found --", ""));
        //             ddldesign.Items.Clear();
        //             ddldesign.Items.Insert(0, new ListItem("-- Not Found --", ""));
        //         }
        //     }


        //     protected void btnShowAdditional_Click(object sender, EventArgs e)
        //     {
        //         pnlAdditionalComponents.Visible = true;
        //     }

        //     protected void btnCloseAdditional_Click(object sender, EventArgs e)
        //     {
        //         pnlAdditionalComponents.Visible = false;
        //     }

        //     protected void btnCloseAppraisal_Click(object sender, EventArgs e)
        //     {
        //         pnlAppraisal.Visible = false;
        //         btnshowApprisal.Visible = true;
        //     }
        //     protected void btnAddSalaryComponent_Click(object sender, EventArgs e)
        //     {
        //         pnlAdditionalComponents.Visible = true; 

        //         List<SalaryComponent> list =
        //             ViewState["Components"] as List<SalaryComponent>
        //             ?? new List<SalaryComponent>();

        //         if (string.IsNullOrEmpty(ddl_component.SelectedValue))
        //             return;

        //         string componentName = ddl_component.SelectedItem.Text;

        //         if (list.Any(x => x.ComponentName == componentName))
        //             return;

        //         bool isDeduction =
        //             componentName == "Provident Fund"
        //             || componentName == "Other Deduction";

        //         list.Add(new SalaryComponent
        //         {
        //             ComponentName = componentName,
        //             Amount = 0,
        //             IsDeduction = isDeduction
        //         });

        //         ViewState["Components"] = list;

        //         BindComponentRepeater();
        //         ScriptManager.RegisterStartupScript(
        //    this,
        //    this.GetType(),
        //    "recalcSalary",
        //    "calculateSalary();",
        //    true
        //);
        //     }

        //     private void BindComponentRepeater()
        //     {
        //         var list = ViewState["Components"] as List<SalaryComponent>
        //                    ?? new List<SalaryComponent>();

        //         rptComponents.DataSource = list;
        //         rptComponents.DataBind();

        //     }

        //     protected void RemoveComponent(object sender, CommandEventArgs e)
        //     {
        //         pnlAdditionalComponents.Visible = true; 

        //         var list = ViewState["Components"] as List<SalaryComponent>;
        //         if (list == null) return;

        //         string componentName = e.CommandArgument.ToString();

        //         list.RemoveAll(x => x.ComponentName == componentName);

        //         ViewState["Components"] = list;

        //         BindComponentRepeater();
        //         ddl_component.ClearSelection();
        //         ddl_component.SelectedIndex = 0;
        //         ScriptManager.RegisterStartupScript(
        //    this,
        //    this.GetType(),
        //    "recalcSalary",
        //    "calculateSalary();",
        //    true
        //);
        //     }

        private void BindAppraisalGrid()
        {
            AppraisalBL bl = new AppraisalBL();

            List<AppraisalDetailsDO> lst = bl.GetAppraisalDetailsList();

            gvAppraisal.DataSource = lst;
            gvAppraisal.DataBind();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            // Clear session variables to ensure add mode
            Session["Appraisal_ID"] = null;
            Session["Appraisal_Mode"] = null;
            Response.Redirect("AddAppraisalForm.aspx");
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            int appraisalId = Convert.ToInt32(btn.CommandArgument);

            Session["Appraisal_ID"] = appraisalId;
            Session["Appraisal_Mode"] = "View";

            Response.Redirect("AddAppraisalForm.aspx");
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            int appraisalId = Convert.ToInt32(btn.CommandArgument);

            Session["Appraisal_ID"] = appraisalId;
            Session["Appraisal_Mode"] = "Edit";

            Response.Redirect("AddAppraisalForm.aspx");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                AppraisalBL bl = new AppraisalBL();

                LinkButton btn = (LinkButton)sender;
                int appraisalId = Convert.ToInt32(btn.CommandArgument);
                ResponseDO result = bl.DeleteAppraisalDetails(appraisalId);

                if (result.Status == 1)
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "msg",
                        $"Swal.fire('Success','{HttpUtility.JavaScriptStringEncode(result.message)}','success')",
                        true);
                    BindAppraisalGrid();
                }
                else
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "msg",
                        $"Swal.fire('Warning','{HttpUtility.JavaScriptStringEncode(result.message)}','warning');",
                        true);
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "error",
                    $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(ex.Message)}','error');",
                    true);
            }
        }
    }
}
