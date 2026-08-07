<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="KnowledgeTransferList.aspx.cs" Inherits="HRMS.View.Modules.KnowledgeTransferList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .custom-dropdown-container {
            position: relative;
        }

        .custom-dropdown {
            padding-right: 25px;
            -webkit-appearance: none;
            -moz-appearance: none;
            appearance: none;
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"%3E%3Cpath stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" /%3E%3C/svg%3E') no-repeat right center;
            background-size: 16px;
        }

        .resignation-page .card {
            border: 0;
            border-radius: 14px;
            box-shadow: 0 10px 28px rgba(31, 45, 61, 0.08);
        }

        .resignation-page .card-title {
            margin-bottom: 0 !important;
            color: #1f2d3d;
            letter-spacing: 0.2px;
        }

        .resignation-page .table.custom-gridview {
            margin-bottom: 0;
            border-collapse: separate;
            border-spacing: 0;
            table-layout: auto;
            width: 100%;
        }

        .resignation-page .table.custom-gridview thead th {
            font-size: 13px;
            font-weight: 700;
            color: #334155;
            background: #f8fafc;
            border-bottom: 1px solid #e2e8f0;
            padding: 12px 10px;
            white-space: nowrap;
        }

        .resignation-page .table.custom-gridview tbody td {
            font-size: 13px;
            color: #334155;
            vertical-align: middle;
            padding: 12px 10px;
            border-bottom: 1px solid #eef2f7;
            word-wrap: break-word;
            overflow-wrap: anywhere;
        }

        .resignation-page .table.custom-gridview td:last-child,
        .resignation-page .table.custom-gridview th:last-child { min-width: 140px; }

        .resignation-page .col-emp-name { min-width: 120px; }
        .resignation-page .col-date { min-width: 95px; }
        .resignation-page .col-last-date { min-width: 95px; }
        .resignation-page .col-authority { min-width: 130px; }
        .resignation-page .col-status { min-width: 90px; }

        .resignation-page .table.custom-gridview tbody tr:hover {
            background: #f8fbff;
        }

        .resignation-page .badge {
            font-size: 11px;
            font-weight: 700;
            padding: 7px 10px;
            border-radius: 999px;
            letter-spacing: 0.3px;
        }

        .resignation-page .action-btn {
            min-width: 84px;
            border-radius: 8px;
            font-size: 12px;
            font-weight: 600;
            margin-right: 0;
            border: 0;
            transition: all 0.2s ease;
            box-shadow: 0 4px 10px rgba(15, 23, 42, 0.14);
            display: inline-flex;
            align-items: center;
            justify-content: center;
            text-decoration: none !important;
        }

        .resignation-page .btn-primary.action-btn {
            background: linear-gradient(135deg, #2563eb, #1d4ed8);
        }

        .resignation-page .action-btn:hover {
            transform: translateY(-1px);
            filter: brightness(1.03);
        }

        .resignation-page .action-btn.icon-only {
            min-width: 34px;
            width: 34px;
            height: 34px;
            padding: 0;
            font-size: 13px;
        }

        .resignation-page .action-wrap {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            flex-wrap: wrap;
            justify-content: center;
            max-width: 100%;
        }

        .resignation-page .table-responsive {
            overflow-x: auto;
            -webkit-overflow-scrolling: touch;
        }

        @media (max-width: 1200px) {
            .resignation-page .table.custom-gridview thead th,
            .resignation-page .table.custom-gridview tbody td {
                font-size: 12px;
                padding: 10px 8px;
            }

            .resignation-page .action-btn {
                min-width: 72px;
                font-size: 11px;
                padding: 6px 8px;
            }
        }

        .resignation-page #searchInput {
            border-radius: 10px;
            border: 1px solid #dbe3ee;
        }

        .resignation-page .info-label {
            font-size: 12px;
            color: #94a3b8;
            text-transform: uppercase;
            letter-spacing: 0.4px;
            margin-bottom: 2px;
        }

        .resignation-page .info-value {
            font-size: 14px;
            color: #1f2d3d;
            font-weight: 600;
            margin-bottom: 14px;
            display: block;
            word-wrap: break-word;
        }

        .resignation-page label.field-label {
            font-weight: 600;
            color: #334155;
            margin-bottom: 6px;
            display: block;
        }

        .resignation-page .btn-accept {
            background: linear-gradient(135deg, #16a34a, #15803d);
            border: 0;
            color: #fff;
            border-radius: 8px;
            font-weight: 600;
            padding: 10px 22px;
        }

        .resignation-page .btn-cancel {
            background: #64748b;
            border: 0;
            color: #fff;
            border-radius: 8px;
            font-weight: 600;
            padding: 10px 22px;
        }
    </style>
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="resignation-page">

    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-lg rounded-3">
                <div class="card-body">

                    <!-- ==================== LIST SECTION ==================== -->
                    <asp:Panel ID="pnlKTList" runat="server" Visible="true">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4"
                            Style="font-size: 2.0em; font-weight: bold;">Knowledge Transfer &amp; Handover</asp:Label>
                        <div class="d-flex justify-content-end align-items-center">
                            <div class="app-search d-none d-lg-block" id="searchdata" runat="server">
                                <div class="position-relative">
                                    <input type="text" class="form-control" id="searchInput" placeholder="Search..." onkeydown="searchOnEnter(event)">
                                    <span class="bx bx-search-alt"></span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- GridView -->
                    <div class="row">
                        <div class="col-12">
                            <asp:HiddenField ID="hfPageIndexViewUser" runat="server" />
                            <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvResignations" class="table custom-gridview" AutoGenerateColumns="false" OnRowCommand="gvUsers_RowCommand"
                                DataKeyNames="EmployeeResignationId" EnablePersistedSelection="true"
                                OnPageIndexChanging="OnPageIndexChanging" PageSize="10"
                                AllowSorting="true" OnSorting="gridview_Sorting"
                                Style="margin: 0 auto;" EmptyDataText="No records found.">
                                <Columns>

                                    <%-- SR NO --%>
                                    <asp:TemplateField HeaderText="SR No" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <%# (gvResignations.PageIndex * gvResignations.PageSize) + Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="EmployeeName" HeaderText="Employee Name"
                                        HeaderStyle-CssClass="col-emp-name" ItemStyle-CssClass="col-emp-name" />

                                    <asp:BoundField DataField="hr_status" HeaderText="Hr Status"
                                        HeaderStyle-CssClass="col-status" ItemStyle-CssClass="col-status" />

                                    <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <div class="action-wrap">
                                            <asp:LinkButton ID="lnkKT" runat="server"
                                                CommandName="OpenKT"
                                                CommandArgument='<%# Eval("EmployeeResignationId") %>'
                                                ToolTip="Knowledge Transfer &amp; Handover"
                                                CssClass="btn btn-primary btn-sm action-btn">
                                                <i class="fas fa-people-arrows"></i>&nbsp;KT
                                            </asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>
                            </div>

                            <!-- Pagination -->
                            <asp:Panel ID="paginationContainer" runat="server"
                                CssClass="pagination-container"
                                Style="text-align: right; font-size: 14px; color: black;"
                                Visible="false">

                                <asp:DropDownList runat="server" ID="ddlPageSelector" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlPageSelector_SelectedIndexChanged"
                                    Style="background-color: white; color: black; border: 1px solid #ddd; padding: 5px 10px; margin: 2px;">
                                </asp:DropDownList>
                            </asp:Panel>

                        </div>
                    </div>
                    </asp:Panel>

                    <!-- ==================== KT FORM SECTION ==================== -->
                    <asp:Panel ID="pnlKTForm" runat="server" Visible="false">
                        <asp:HiddenField ID="hfResignationId" runat="server" />
                        <asp:HiddenField ID="hfKTId" runat="server" />

                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <span class="card-title" style="font-size: 1.6em; font-weight: bold;">Knowledge Transfer &amp; Handover</span>
                        </div>

                        <!-- Employee Information (Read Only) -->
                        <div class="row mb-2">
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Employee ID</div>
                                <asp:Label ID="lblEmployeeId" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Employee Name</div>
                                <asp:Label ID="lblEmployeeName" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Department</div>
                                <asp:Label ID="lblDepartment" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Designation</div>
                                <asp:Label ID="lblDesignation" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Resignation Date</div>
                                <asp:Label ID="lblResignationDate" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Proposed Last Working Date</div>
                                <asp:Label ID="lblProposedLastWorkingDate" runat="server" CssClass="info-value" Text="-" />
                            </div>
                        </div>

                        <hr />

                        <h5 class="mb-3">KT plan</h5>

                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <asp:TextBox ID="txtKTPlan" runat="server"
                                    CssClass="form-control" TextMode="MultiLine" Rows="3"
                                    placeholder="Required" />
                                <asp:RequiredFieldValidator ID="rfvKTPlan" runat="server" ControlToValidate="txtKTPlan"
                                    ErrorMessage="KT Plan is mandatory" CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="KTMainGroup" />
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="field-label">Replacement employee <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtReplacementEmployee" runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="rfvReplacementEmployee" runat="server" ControlToValidate="txtReplacementEmployee"
                                    ErrorMessage="Replacement Employee is mandatory" CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="KTMainGroup" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="field-label">KT status <span class="text-danger">*</span></label>
                                <asp:DropDownList ID="ddlKTStatus" runat="server" CssClass="form-control custom-dropdown">
                                    <asp:ListItem Text="-- Select --" Value="" />
                                    <asp:ListItem Text="Pending" Value="Pending" />
                                    <asp:ListItem Text="In Progress" Value="In Progress" />
                                    <asp:ListItem Text="Completed" Value="Completed" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvKTStatus" runat="server" ControlToValidate="ddlKTStatus"
                                    InitialValue="" ErrorMessage="Please select KT Status" CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="KTMainGroup" />
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="field-label">KT start date</label>
                                <asp:TextBox ID="txtKTStartDate" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="field-label">KT completion date</label>
                                <asp:TextBox ID="txtKTCompletionDate" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                        </div>

                        <h5 class="mb-3 mt-2">Project handover</h5>

                        <div class="table-responsive mb-3">
                            <asp:GridView runat="server" ID="gvProjectHandover" class="table custom-gridview" AutoGenerateColumns="false"
                                OnRowCommand="gvProjectHandover_RowCommand" EmptyDataText="No project handover rows added yet.">
                                <Columns>
                                    <asp:BoundField DataField="ProjectName" HeaderText="Project Name" />
                                    <asp:BoundField DataField="AssignedEmployee" HeaderText="Assigned Employee" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" />
                                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="90px">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkRemoveRow" runat="server" CommandName="RemoveRow"
                                                CommandArgument="<%# Container.DataItemIndex %>" CausesValidation="false"
                                                CssClass="btn btn-sm btn-outline-danger" ToolTip="Remove row">
                                                <i class="fa fa-trash"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>

                        <div class="row align-items-end mb-3">
                            <div class="col-md-4 mb-2">
                                <label class="field-label">Project Name <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtNewProjectName" runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="rfvNewProjectName" runat="server" ControlToValidate="txtNewProjectName"
                                    ErrorMessage="Project Name is mandatory" CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="ProjectRowGroup" />
                            </div>
                            <div class="col-md-4 mb-2">
                                <label class="field-label">Assigned Employee <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtNewAssignedEmployee" runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="rfvNewAssignedEmployee" runat="server" ControlToValidate="txtNewAssignedEmployee"
                                    ErrorMessage="Assigned Employee is mandatory" CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="ProjectRowGroup" />
                            </div>
                            <div class="col-md-3 mb-2">
                                <label class="field-label">Status <span class="text-danger">*</span></label>
                                <asp:DropDownList ID="ddlNewProjectStatus" runat="server" CssClass="form-control custom-dropdown">
                                    <asp:ListItem Text="-- Select --" Value="" />
                                    <asp:ListItem Text="Pending" Value="Pending" />
                                    <asp:ListItem Text="In Progress" Value="In Progress" />
                                    <asp:ListItem Text="Completed" Value="Completed" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvNewProjectStatus" runat="server" ControlToValidate="ddlNewProjectStatus"
                                    InitialValue="" ErrorMessage="Please select Status" CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="ProjectRowGroup" />
                            </div>
                            <div class="col-md-1 mb-2">
                                <asp:Button ID="btnAddProjectRow" runat="server" CssClass="btn btn-secondary w-100"
                                    Text="Add" ValidationGroup="ProjectRowGroup" OnClick="btnAddProjectRow_Click" />
                            </div>
                        </div>

                        <div class="d-flex gap-2 mt-3">
                            <asp:Button ID="btnSaveKT" runat="server"
                                CssClass="btn-accept" Text="Save"
                                ValidationGroup="KTMainGroup"
                                OnClick="btnSaveKT_Click" />
                            <asp:Button ID="btnCancelKT" runat="server"
                                CssClass="btn-cancel" Text="Cancel"
                                CausesValidation="false"
                                OnClick="btnCancelKT_Click" />
                        </div>

                    </asp:Panel>

                </div>
            </div>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script>
        function showUserSavedMessage(status, remark) {
            Swal.fire({
                icon: status.toLowerCase() === "success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }

        function showKTResult(status, message) {
            Swal.fire({
                icon: status.toLowerCase() === "success" ? "success" : "error",
                text: message,
                timer: status.toLowerCase() === "success" ? 2500 : undefined,
                showConfirmButton: status.toLowerCase() !== "success"
            });
        }

    </script>

    <script>
        function initializeSearch() {
            $(document).on('input', '#searchInput', function () {
                var searchTerm = $(this).val().toLowerCase();
                filterGrid(searchTerm);
            });
            $(document).on('keydown', '#searchInput', searchOnEnter);
        }

        function filterGrid(searchTerm) {
            var rows = $('#<%= gvResignations.ClientID %> tbody tr').filter(function () {
                return $(this).find('td').length > 0;
            });

            rows.hide();

            if (searchTerm === '') {
                rows.show();
                return;
            }

            rows.filter(function () {
                var employeeName = $(this).find('td').eq(1).text().toLowerCase();
                var rowText = $(this).text().toLowerCase();
                return employeeName.indexOf(searchTerm) >= 0 || rowText.indexOf(searchTerm) >= 0;
            }).show();
        }

        function searchOnEnter(event) {
            if (event.key === 'Enter') {
                event.preventDefault();
                var searchTerm = $('#searchInput').val().toLowerCase();
                filterGrid(searchTerm);
            }
        }

        $(document).ready(function () {
            initializeSearch();
        });
    </script>
    </div>
</asp:Content>
