<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AccountsDashboard.aspx.cs" Inherits="HRMS.View.Modules.AccountsDashboard" MaintainScrollPositionOnPostBack="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .stat-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 10px 20px rgba(0, 0, 0, 0.1);
            transition: transform 0.3s ease;
        }

            .stat-card:hover {
                transform: translateY(-5px);
            }

            .stat-card h3 {
                font-size: 2rem;
                font-weight: 700;
                margin: 0;
            }

            .stat-card p {
                opacity: 0.9;
                margin: 8px 0 0 0;
            }

        .section-title {
            font-size: 1.25rem;
            font-weight: 700;
            color: #172033;
            margin-bottom: 16px;
        }

        .count-ratio-badge {
            display: inline-flex;
            align-items: center;
            font-size: 13px;
            font-weight: 600;
            color: #475569;
            background: #f1f5f9;
            border-radius: 20px;
            padding: 3px 12px;
            margin-left: 8px;
            vertical-align: middle;
        }

        .table-container {
            background: white;
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
        }

        .stat-card .amount-row {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .stat-card .eye-btn {
            background: none;
            border: none;
            padding: 0;
            cursor: pointer;
            color: rgba(255,255,255,0.85);
            font-size: 1.1rem;
            line-height: 1;
            transition: color 0.2s;
        }

            .stat-card .eye-btn:hover {
                color: #fff;
            }

        .paid-status {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            max-width: 120px;
            height: 31px;
            background: #28a745;
            color: #fff;
            border-radius: .375rem;
            font-size: 14px;
            font-weight: 500;
        }

        /* Reimbursement Documents modal */
        #reimbDocsModal .modal-content {
            border: none;
            border-radius: 14px;
            overflow: hidden;
        }

        #reimbDocsModal .modal-header {
            background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
            border-bottom: none;
            padding: 18px 22px;
        }

        #reimbDocsModal .modal-title {
            color: #fff;
            font-weight: 700;
            font-size: 1.05rem;
        }

        #reimbDocsModal .modal-title small {
            display: block;
            font-weight: 400;
            font-size: .75rem;
            opacity: .9;
            margin-top: 2px;
        }

        #reimbDocsModal .btn-close {
            filter: brightness(0) invert(1);
            opacity: .9;
        }

        #reimbDocsModal .modal-body {
            padding: 18px 20px;
            max-height: 360px;
            overflow-y: auto;
        }

        .reimb-doc-item {
            display: flex;
            align-items: center;
            gap: 12px;
            background: #f8fafc;
            border: 1px solid #e5e7eb;
            border-radius: 10px;
            padding: 10px 12px;
            margin-bottom: 10px;
            transition: background .15s ease, box-shadow .15s ease;
        }

            .reimb-doc-item:last-child {
                margin-bottom: 0;
            }

            .reimb-doc-item:hover {
                background: #fff;
                box-shadow: 0 4px 14px rgba(17,24,39,.08);
            }

        .reimb-doc-icon {
            flex: 0 0 40px;
            width: 40px;
            height: 40px;
            border-radius: 9px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            color: #fff;
        }

        .reimb-doc-icon.pdf {
            background: linear-gradient(135deg, #f5576c 0%, #f093fb 100%);
        }

        .reimb-doc-icon.image {
            background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
        }

        .reimb-doc-icon.doc {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        }

        .reimb-doc-icon.other {
            background: #94a3b8;
        }

        .reimb-doc-info {
            flex: 1 1 auto;
            min-width: 0;
        }

        .reimb-doc-name {
            display: block;
            font-size: 13px;
            font-weight: 600;
            color: #1f2937;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .reimb-doc-meta {
            display: block;
            font-size: 11px;
            color: #94a3b8;
            margin-top: 2px;
        }

        .reimb-doc-view-btn {
            flex: 0 0 auto;
            width: 34px;
            height: 34px;
            border-radius: 8px;
            border: 1px solid #e0f2fe;
            background: #e0f2fe;
            color: #0284c7;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 14px;
            transition: background .15s ease, color .15s ease;
        }

            .reimb-doc-view-btn:hover {
                background: #0284c7;
                color: #fff;
            }

        .reimb-doc-empty {
            text-align: center;
            padding: 30px 10px;
            color: #94a3b8;
        }

            .reimb-doc-empty i {
                font-size: 32px;
                display: block;
                margin-bottom: 10px;
                opacity: .6;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row mb-4">
        <div class="col-12">
            <h4 class="font-size-18 mb-0">Accounts Dashboard</h4>
            <p class="text-muted mt-1">Employee-wise salary and reimbursement details</p>
        </div>
    </div>

    <div class="row mb-4">
        <div class="col-md-4">
            <div class="stat-card">
                <div class="amount-row">
                    <%--                    <h3 id="salaryAmount" data-value="&#8377;1,25,400" data-hidden="true">*****</h3>--%>
                    <h3 id="salaryAmount" runat="server" data-hidden="true">*****
                    </h3>
                    <%--<button type="button" class="eye-btn" onclick="toggleAmount('salaryAmount', this)" title="Show/Hide">
                        <i class="mdi mdi-eye"></i>
                    </button>--%>
                    <button type="button"
                        class="eye-btn"
                        onclick="toggleAmount('<%= salaryAmount.ClientID %>', this)"
                        title="Show/Hide">
                        <i class="mdi mdi-eye"></i>
                    </button>
                </div>
                <p>Total Salary Disbursed</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="stat-card" style="background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);">
                <div class="amount-row">
                    <%--                    <h3 id="reimbAmount" data-value="&#8377;12,500" data-hidden="true">*****</h3>--%>
                    <h3 id="reimbAmount" runat="server" data-hidden="true">*****
                    </h3>
                    <%--   <button type="button" class="eye-btn" onclick="toggleAmount('reimbAmount', this)" title="Show/Hide">
                        <i class="mdi mdi-eye"></i>
                    </button>--%>
                    <button type="button"
                        class="eye-btn"
                        onclick="toggleAmount('<%= reimbAmount.ClientID %>', this)"
                        title="Show/Hide">
                        <i class="mdi mdi-eye"></i>
                    </button>
                </div>
                <p>Total Reimbursements</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="stat-card" style="background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);">
                <h3>
                    <asp:Literal ID="litActiveEmployeeCount" runat="server"></asp:Literal>
                </h3>
                <p>Active Employees</p>
            </div>
        </div>
    </div>

    <div class="row mb-4">
        <div class="col-lg-12">
            <div class="table-container">
                <h5 class="section-title">
                    Employee-wise Salary Details
                    <span class="count-ratio-badge">
                        <asp:Literal ID="litSalaryCountRatio" runat="server"></asp:Literal>
                    </span>
                </h5>

                <div class="row mb-3 align-items-end">

                    <!-- Search By -->
                    <div class="col-auto">

                        <asp:Label ID="lblSearchBy"
                            runat="server"
                            Text="Search By"
                            CssClass="form-label fw-semibold">
                        </asp:Label>

                        <asp:DropDownList ID="ddlSearchBy"
                            runat="server"
                            CssClass="form-select"
                            Width="260px"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlSearchBy_SelectedIndexChanged">

                            <asp:ListItem Text="-- Select Search Type --" Value=""></asp:ListItem>
                            <asp:ListItem Text="Status" Value="Status"></asp:ListItem>
                            <asp:ListItem Text="Employee Code" Value="EmployeeCode"></asp:ListItem>
                            <asp:ListItem Text="Employee Name" Value="EmployeeName"></asp:ListItem>

                        </asp:DropDownList>

                    </div>

                    <!-- Status -->
                    <div class="col-auto" id="divStatus" runat="server" visible="false">

                        <asp:Label runat="server"
                            Text="Status"
                            CssClass="form-label fw-semibold">
                        </asp:Label>

                        <asp:DropDownList ID="ddlStatusSearch"
                            runat="server"
                            CssClass="form-select"
                            Width="220px">

                            <asp:ListItem Text="-- Select Status --" Value=""></asp:ListItem>
                            <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                            <asp:ListItem Text="Paid" Value="Paid"></asp:ListItem>

                        </asp:DropDownList>

                    </div>

                    <div class="col-auto" id="divEmployeeCode" runat="server" visible="false">
                        <asp:Label runat="server"
                            Text="Employee Code"
                            CssClass="form-label fw-semibold">
                        </asp:Label>
                        <asp:DropDownList ID="ddlEmployeeCodeSearch"
                            runat="server"
                            CssClass="form-select"
                            Style="width: 220px;">
                        </asp:DropDownList>
                    </div>

                    <div class="col-auto" id="divEmployeeName" runat="server" visible="false">
                        <asp:Label runat="server"
                            Text="Employee Name"
                            CssClass="form-label fw-semibold">
                        </asp:Label>
                        <asp:DropDownList ID="ddlEmployeeNameSearch"
                            runat="server"
                            CssClass="form-select"
                            Style="width: 250px;">
                        </asp:DropDownList>
                    </div>
                    <div class="col-auto">
                        <asp:Button ID="btnSearch"
                            runat="server"
                            Text="Search"
                            CssClass="btn btn-primary px-4"
                            OnClick="btnSearch_Click" />
                    </div>

                    <!-- Clear -->
                    <div class="col-auto">
                        <asp:Button ID="btnClear"
                            runat="server"
                            Text="Clear"
                            CssClass="btn btn-outline-secondary px-4"
                            OnClick="btnClear_Click" />
                    </div>

                    <!-- Export -->
                    <div class="col-auto ms-auto">
                        <asp:LinkButton ID="btnExportExcel"
                            runat="server"
                            CssClass="btn btn-success"
                            ToolTip="Export to Excel"
                            OnClientClick="downloadSalarySlipData(); return false;">
        <i class="fa fa-file-excel-o"></i> Export
                        </asp:LinkButton>
                    </div>

                </div>
                <div class="table-responsive">
                    <table id="tblSalaryList" class="table table-nowrap mb-0">
                        <thead class="table-light">
                            <tr>
                                <th scope="col">Employee Name</th>
                                <th scope="col">Employee Code</th>
                                <th scope="col">basic salary</th>
                                <%--    <th scope="col">Allowances</th>
                                <th scope="col">Deductions</th>--%>
                                <th scope="col">Net Salary</th>
                                <th scope="col">Status</th>
                            </tr>
                        </thead>
                        <%--   <tbody>
                            <tr>
                                <td>Meera Sharma</td>
                                <td>EMP001</td>
                                <td>&#8377;30,000</td>
                                <td>&#8377;10,000</td>
                                <td>&#8377;3,000</td>
                                <td>&#8377;37,000</td>
                                <td><span class="badge bg-success">Paid</span></td>
                            </tr>
                            <tr>
                                <td>Raj Patel</td>
                                <td>EMP002</td>
                                <td>&#8377;25,000</td>
                                <td>&#8377;8,000</td>
                                <td>&#8377;2,500</td>
                                <td>&#8377;30,500</td>
                                <td><span class="badge bg-primary">Pending</span></td>
                            </tr>
                            <tr>
                                <td>Priya Singh</td>
                                <td>EMP003</td>
                                <td>&#8377;28,000</td>
                                <td>&#8377;9,000</td>
                                <td>&#8377;2,800</td>
                                <td>&#8377;34,200</td>
                                <td><span class="badge bg-success">Paid</span></td>
                            </tr>
                        </tbody>--%>
                        <tbody>
                            <asp:Repeater ID="rptEmployeeSalary" runat="server" OnItemDataBound="rptEmployeeSalary_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("username") %></td>
                                        <td><%# Eval("employeecode") %></td>


                                        <td>&#8377;<%# Convert.ToDecimal(Eval("basic_salary")).ToString("N2") %></td>


                                        <%--  <td>&#8377;<%# Convert.ToDecimal(Eval("special_allowance")).ToString("N2") %></td>



                                        <td>&#8377;<%# Convert.ToDecimal(Eval("total_deductions")).ToString("N2") %></td>--%>



                                        <td>&#8377;<%# Convert.ToDecimal(Eval("net_pay")).ToString("N2") %></td>

                                        <td>
                                            <asp:Panel ID="pnlPending" runat="server" Visible='<%# Eval("status").ToString() != "Paid" %>'>
                                                <asp:DropDownList
                                                    ID="ddlStatus"
                                                    runat="server"
                                                    CssClass="form-select form-select-sm"
                                                    AutoPostBack="true"
                                                    ToolTip='<%# Eval("salary_slip_details_id") %>'
                                                    OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                                    <asp:ListItem Value="Pending">Pending</asp:ListItem>
                                                    <asp:ListItem Value="Paid">Paid</asp:ListItem>
                                                </asp:DropDownList>
                                            </asp:Panel>

                                            <asp:Panel ID="pnlPaid" runat="server"
                                                Visible='<%# Eval("status").ToString() == "Paid" %>'>

                                                <span class="paid-status">
                                                    <%# Eval("status") %>
                                                </span>

                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                <asp:Label ID="lblNoSalaryData"
                    runat="server"
                    CssClass="text-center d-block mt-3"
                    ForeColor="red"
                    Visible="false"
                    Text="No data found for this month.">
            </asp:Label>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="table-container">
                <h5 class="section-title">Employee-wise Reimbursement Details</h5>
                <div class="row mb-3 align-items-end">

                    <!-- Search By -->
                    <div class="col-auto">
                        <asp:Label ID="lblreimbSearchBy"
                            runat="server"
                            Text="Search By"
                            CssClass="form-label fw-semibold">
                        </asp:Label>

                        <asp:DropDownList ID="ddlSearchByReimb"
                            runat="server"
                            CssClass="form-select"
                            Width="260px"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlSearchByReimb_SelectedIndexChanged">

                            <asp:ListItem Text="-- Select Search Type --" Value=""></asp:ListItem>
                            <asp:ListItem Text="Status" Value="Status"></asp:ListItem>
                            <asp:ListItem Text="Employee Code" Value="EmployeeCode"></asp:ListItem>
                            <asp:ListItem Text="Employee Name" Value="EmployeeName"></asp:ListItem>

                        </asp:DropDownList>
                    </div>

                    <!-- Status -->
                    <div class="col-auto" id="divReimbStatus" runat="server" visible="false">

                        <asp:Label runat="server"
                            Text="Status"
                            CssClass="form-label fw-semibold">
                        </asp:Label>

                        <asp:DropDownList ID="ddlReimbStatusSearch"
                            runat="server"
                            CssClass="form-select"
                            Width="220px">

                            <asp:ListItem Text="-- Select Status --" Value=""></asp:ListItem>
                            <asp:ListItem Text="Approved" Value="168"></asp:ListItem>
                            <asp:ListItem Text="Paid" Value="166"></asp:ListItem>

                        </asp:DropDownList>

                    </div>

                    <!-- Employee Code -->
                    <div class="col-auto" id="divReimbEmpCode" runat="server" visible="false">

                        <asp:Label runat="server"
                            Text="Employee Code"
                            CssClass="form-label fw-semibold">
                        </asp:Label>

                        <asp:DropDownList ID="ddlReimbEmployeeCode"
                            runat="server"
                            CssClass="form-select"
                            Width="220px">
                        </asp:DropDownList>

                    </div>

                    <!-- Employee Name -->
                    <div class="col-auto" id="divReimbEmpName" runat="server" visible="false">

                        <asp:Label runat="server"
                            Text="Employee Name"
                            CssClass="form-label fw-semibold">
                        </asp:Label>

                        <asp:DropDownList ID="ddlReimbEmployeeName"
                            runat="server"
                            CssClass="form-select"
                            Width="250px">
                        </asp:DropDownList>

                    </div>

                    <!-- Search -->
                    <div class="col-auto">
                        <asp:Button ID="btnReimbSearch"
                            runat="server"
                            Text="Search"
                            CssClass="btn btn-primary px-4"
                            OnClick="btnReimbSearch_Click" />
                    </div>

                    <!-- Clear -->
                    <div class="col-auto">
                        <asp:Button ID="btnReimbClear"
                            runat="server"
                            Text="Clear"
                            CssClass="btn btn-outline-secondary px-4"
                            OnClick="btnReimbClear_Click" />
                    </div>
                    <div class="col-auto ms-auto">
                        <asp:LinkButton ID="btnreimbexportexcel"
                            runat="server"
                            CssClass="btn btn-success"
                            ToolTip="Export to Excel"
                            OnClientClick="downloadReimbuSlipData(); return false;">
<i class="fa fa-file-excel-o"></i> Export
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="table-responsive">
                    <table class="table table-nowrap mb-0" id="tblReimbursementList">
                        <thead class="table-light">
                            <tr>
                                <th scope="col">Employee Name</th>
                                <th scope="col">Employee Code</th>
                                <th scope="col">Reimbursement Type</th>
                                <th scope="col">Amount</th>
                                <th scope="col">Date</th>
                              
                                <th scope="col">Documents</th>
                                  <th scope="col">Status</th>
                            </tr>
                        </thead>
                        <%--             <tbody>
                            <tr>
                                <td>Meera Sharma</td>
                                <td>EMP001</td>
                                <td>Travel</td>
                                <td>&#8377;3,500</td>
                                <td>10 Jul 2024</td>
                                <td><span class="badge bg-success">Approved</span></td>
                            </tr>
                            <tr>
                                <td>Raj Patel</td>
                                <td>EMP002</td>
                                <td>Meal</td>
                                <td>&#8377;1,500</td>
                                <td>12 Jul 2024</td>
                                <td><span class="badge bg-warning">Pending</span></td>
                            </tr>
                            <tr>
                                <td>Priya Singh</td>
                                <td>EMP003</td>
                                <td>Office Supplies</td>
                                <td>&#8377;2,000</td>
                                <td>08 Jul 2024</td>
                                <td><span class="badge bg-danger">Rejected</span></td>
                            </tr>
                        </tbody>--%>
                        <tbody>
                            <asp:Repeater ID="rptEmployeeReimbursement" runat="server" OnItemDataBound="rptEmployeeReimbursement_ItemDataBound" OnItemCommand="rptEmployeeReimbursement_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("employee_name") %></td>
                                        <td><%# Eval("employee_code") %></td>
                                        <td><%# Eval("claim_type") %></td>
                                        <td>&#8377;<%# Convert.ToDecimal(Eval("claim_amount")).ToString("N2") %></td>
                                        <td><%# Convert.ToDateTime(Eval("claim_date")).ToString("dd MMM yyyy") %></td>

                                        
                                        <td>
                                            <asp:LinkButton runat="server"
                                                CssClass="btn btn-sm btn-outline-primary"
                                                CommandName="ViewDocs"
                                                CommandArgument='<%# Eval("reimbursement_id") %>'
                                                CausesValidation="false">
                                                <i class="far fa-eye"></i> View Doc
                                            </asp:LinkButton>
                                        </td>
                                        <td>
    <asp:Panel ID="pnlreimbPending" runat="server" Visible='<%# Eval("status").ToString() != "Paid" %>'>
        <asp:DropDownList
            ID="ddlreimbStatus"
            runat="server"
            CssClass="form-select form-select-sm"
            AutoPostBack="true"
            ToolTip='<%# Eval("reimbursement_id") %>'
            OnSelectedIndexChanged="ddlreimbStatus_SelectedIndexChanged">

            <asp:ListItem Value="Pending">Approved</asp:ListItem>
            <asp:ListItem Value="Paid">Paid</asp:ListItem>

        </asp:DropDownList>
    </asp:Panel>

    <asp:Panel ID="pnlreimbPaid" runat="server"
        Visible='<%# Eval("status").ToString() == "Paid" %>'>

        <span class="paid-status">
            <%# Eval("status") %>
        </span>

    </asp:Panel>
</td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                <asp:Label ID="lblnoreimbdata"
                    runat="server"
                    CssClass="text-center d-block mt-3"
                    ForeColor="red"
                    Visible="false"
                    Text="No data found for this month.">
                </asp:Label>
            </div>
        </div>
    </div>

    <!-- Reimbursement Documents Modal -->
    <div class="modal fade" id="reimbDocsModal" tabindex="-1" aria-labelledby="reimbDocsModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="reimbDocsModalLabel">
                        Reimbursement Documents
                        <small>
                            <asp:Literal ID="litReimbDocsCount" runat="server"></asp:Literal>
                        </small>
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:Repeater ID="rptReimbDocsModal" runat="server" OnItemCommand="rptReimbDocsModal_ItemCommand">
                        <ItemTemplate>
                            <div class="reimb-doc-item">
                                <div class='<%# "reimb-doc-icon " + GetFileIconClass(Eval("FileExtension")) %>'>
                                    <i class='<%# GetFileIconGlyph(Eval("FileExtension")) %>'></i>
                                </div>
                                <div class="reimb-doc-info">
                                    <span class="reimb-doc-name" title='<%# Eval("FileName") %><%# Eval("FileExtension") %>'><%# Eval("FileName") %><%# Eval("FileExtension") %></span>
                                    <span class="reimb-doc-meta">Uploaded <%# Eval("InsertedDate", "{0:dd MMM yyyy}") %></span>
                                </div>
                                <asp:LinkButton runat="server"
                                    CssClass="reimb-doc-view-btn"
                                    ToolTip="View / Download"
                                    CommandName="DownloadReimbDoc"
                                    CommandArgument='<%# Eval("UserDocDetId") %>'
                                    CausesValidation="false">
                                    <i class="far fa-eye"></i>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div class="reimb-doc-empty" id="reimbDocsEmptyState" runat="server" visible="false">
                        <i class="far fa-folder-open"></i>
                        <asp:Label ID="lblNoReimbDocs" runat="server" Text="No documents uploaded for this reimbursement."></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnOpenReimbDocsModal" runat="server" Value="false" />

    <script>
        function toggleAmount(id, btn) {
            var el = document.getElementById(id);
            console.log("Element:", el);
            console.log("Data Value:", el.getAttribute("data-value"));
            var icon = btn.querySelector('i');
            var isHidden = el.getAttribute('data-hidden') === 'true';
            if (isHidden) {
                el.innerHTML = el.getAttribute('data-value');
                el.setAttribute('data-hidden', 'false');
                icon.classList.remove('mdi-eye');
                icon.classList.add('mdi-eye-off');
            } else {
                el.innerHTML = '*****';
                el.setAttribute('data-hidden', 'true');
                icon.classList.remove('mdi-eye-off');
                icon.classList.add('mdi-eye');
            }
        }
    </script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/select2/4.1.0-rc.0/css/select2.min.css" rel="stylesheet" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/select2/4.1.0-rc.0/js/select2.min.js"></script>

    <script>
        function initEmployeeNameSelect2() {
            $('#<%= ddlEmployeeNameSearch.ClientID %>').select2({
                placeholder: "-- Please Select or search --",
                width: '250px',
                allowClear: true
            });
        }

        function initEmployeeCodeSelect2() {
            $('#<%= ddlEmployeeCodeSearch.ClientID %>').select2({
                placeholder: "-- Please Select or search --",
                width: '220px',
                allowClear: true
            });
        }

        // Reimbursement Employee Name
        function initReimbEmployeeNameSelect2() {
            $('#<%= ddlReimbEmployeeName.ClientID %>').select2({
                placeholder: "-- Please Select or search --",
                width: '250px',
                allowClear: true
            });
        }

        // Reimbursement Employee Code
        function initReimbEmployeeCodeSelect2() {
            $('#<%= ddlReimbEmployeeCode.ClientID %>').select2({
                placeholder: "-- Please Select or search --",
                width: '220px',
                allowClear: true
            });
        }

        $(document).ready(function () {
            initEmployeeNameSelect2();
            initEmployeeCodeSelect2();

            initReimbEmployeeNameSelect2();
            initReimbEmployeeCodeSelect2();
        });

        // For UpdatePanel
        if (typeof Sys !== 'undefined' && Sys.WebForms) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                initEmployeeNameSelect2();
                initEmployeeCodeSelect2();

                initReimbEmployeeNameSelect2();
                initReimbEmployeeCodeSelect2();
       var scroll = sessionStorage.getItem("AccountsDashboardScroll");
 if (scroll !== null) {
     window.scrollTo(0, parseInt(scroll));
 }
            });
        }
    </script>
    
    <script>
        // Save scroll position before every postback
        window.addEventListener("beforeunload", function () {
            sessionStorage.setItem("AccountsDashboardScroll", window.scrollY);
        });

        // Restore after page loads
        window.addEventListener("load", function () {
            var scroll = sessionStorage.getItem("AccountsDashboardScroll");
            if (scroll !== null) {
                window.scrollTo(0, parseInt(scroll));
            }
        });
    </script>
    <script>
        function showReimbDocsModal() {
            var modalEl = document.getElementById('reimbDocsModal');
            if (modalEl && typeof bootstrap !== 'undefined') {
                bootstrap.Modal.getOrCreateInstance(modalEl).show();
            }
        }

        $(document).ready(function () {
            if ($('#<%= hdnOpenReimbDocsModal.ClientID %>').val() === 'true') {
                showReimbDocsModal();
                $('#<%= hdnOpenReimbDocsModal.ClientID %>').val('false');
            }
        });

        if (typeof Sys !== 'undefined' && Sys.WebForms) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                if ($('#<%= hdnOpenReimbDocsModal.ClientID %>').val() === 'true') {
                    showReimbDocsModal();
                    $('#<%= hdnOpenReimbDocsModal.ClientID %>').val('false');
                }
            });
        }
    </script>

    <script>
        function showAccountsSavedMessage(status, remark) {
            Swal.fire({

                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }
    </script>

    <script>
        function cleanCsvValue(value) {
            if (!value) return "";
            value = value.replace(/\r?\n|\r/g, " ").trim();
            return '"' + value.replace(/"/g, '""') + '"';
        }

        function downloadSalarySlipData() {

            var table = document.getElementById("tblSalaryList");

            if (!table) {
                alert("Salary table not found.");
                return;
            }

            var csv = "Employee Name,Employee Code,Basic Salary,Net Salary,Status\n";

            var rows = table.getElementsByTagName("tr");
            var hasRows = false;

            for (var i = 1; i < rows.length; i++) {

                var cells = rows[i].getElementsByTagName("td");

                if (cells.length < 5)
                    continue;

                var empName = cleanCsvValue(cells[0].innerText);
                var empCode = cleanCsvValue(cells[1].innerText);
                var basicSalary = cleanCsvValue(cells[2].innerText.replace(/[^\d.,-]/g, ""));
                var netSalary = cleanCsvValue(cells[3].innerText.replace(/[^\d.,-]/g, ""));

                var status = "";

                var ddl = cells[4].querySelector("select");
                if (ddl)
                    status = ddl.value;
                else
                    status = cells[4].innerText;

                status = cleanCsvValue(status);

                csv += empName + "," +
                    empCode + "," +
                    basicSalary + "," +
                    netSalary + "," +
                    status + "\n";

                hasRows = true;
            }

            if (!hasRows) {
                Swal.fire({
                    icon: "info",
                    title: "No Data",
                    text: "No salary records available."
                });
                return;
            }

            var blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });

            var link = document.createElement("a");
            link.href = URL.createObjectURL(blob);
            link.download = "SalarySlip_List.csv";

            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }

    </script>
    <script>
        function downloadReimbuSlipData() {

            var table = document.getElementById("tblReimbursementList");

            if (!table) {
                alert("Reimbursement table not found.");
                return;
            }

            var csv = "Employee Name,Employee Code,Reimbursement Type,Amount,Date,Status\n";

            var rows = table.getElementsByTagName("tr");
            var hasRows = false;

            for (var i = 1; i < rows.length; i++) {

                var cells = rows[i].getElementsByTagName("td");

                if (cells.length < 6)
                    continue;

                var empName = cleanCsvValue(cells[0].innerText);
                var empCode = cleanCsvValue(cells[1].innerText);
                var claimType = cleanCsvValue(cells[2].innerText);
                var amount = cleanCsvValue(cells[3].innerText.replace(/[^\d.,-]/g, ""));
                var claimDate = cleanCsvValue(cells[4].innerText);

                var status = "";

                var ddl = cells[5].querySelector("select");
                if (ddl)
                    status = ddl.options[ddl.selectedIndex].text;
                else
                    status = cells[5].innerText;

                status = cleanCsvValue(status);

                csv += empName + "," +
                    empCode + "," +
                    claimType + "," +
                    amount + "," +
                    claimDate + "," +
                    status + "\n";

                hasRows = true;
            }

            if (!hasRows) {
                Swal.fire({
                    icon: "info",
                    title: "No Data",
                    text: "No reimbursement records available."
                });
                return;
            }

            var blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });

            var link = document.createElement("a");
            link.href = URL.createObjectURL(blob);
            link.download = "Employee_Reimbursement_List.csv";

            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    </script>
</asp:Content>
