using System;

namespace HRMS.View.Modules
{
    // TerminationList is kept only as a navigation/menu entry point. It has no
    // UI or logic of its own: Page_Load transfers execution to EmployeeAction.aspx
    // so that page's UI, layout, controls, data binding, search/filter,
    // actions, and validations are reused exactly as-is, without duplicating
    // any of its implementation. Server.Transfer (preserveForm: true) keeps the
    // browser's address bar on TerminationList.aspx while EmployeeAction.aspx's
    // own page lifecycle - including its own postbacks/events - runs normally,
    // as if it had been requested directly.
    public partial class TerminationList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Server.Transfer("~/View/Modules/EmployeeAction.aspx", true);
        }
    }
}
