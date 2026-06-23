<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="ResignationList.aspx.cs" Inherits="HRMS.View.Modules.ResignationList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/css/bootstrap-datepicker.min.css" />
    <link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
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
        .resignation-page .col-email { min-width: 220px; }
        .resignation-page .col-date { min-width: 95px; }
        .resignation-page .col-notice { min-width: 70px; }
        .resignation-page .col-last-date { min-width: 95px; }
        .resignation-page .col-reason { min-width: 150px; }
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

        .resignation-page .btn-success.action-btn {
            background: linear-gradient(135deg, #16a34a, #15803d);
        }

        .resignation-page .btn-danger.action-btn {
            background: linear-gradient(135deg, #ef4444, #dc2626);
        }

        .resignation-page .action-btn:hover {
            transform: translateY(-1px);
            filter: brightness(1.03);
        }

        .resignation-page .action-btn.disabled,
        .resignation-page .action-btn[disabled] {
            opacity: 1;
            background-color: #94a3b8 !important;
            color: #fff !important;
            cursor: not-allowed;
            pointer-events: none;
            box-shadow: none;
            transform: none !important;
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

        .resignation-page .modal-footer .btn {
            min-width: 96px;
            border-radius: 10px;
            font-weight: 600;
            border: 0;
            box-shadow: 0 5px 12px rgba(15, 23, 42, 0.14);
        }

        .resignation-page .modal-footer .btn-success {
            background: linear-gradient(135deg, #2563eb, #1d4ed8);
        }

        .resignation-page .modal-footer .btn-secondary {
            background: #64748b;
        }
    </style>
    <script src="../../assets/libs/jquery/jquery.min.js"></script>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="resignation-page">
    <div class="modal fade" id="resignationModal" tabindex="-1"
        aria-labelledby="resignationLabel" aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">

                <div class="modal-header">
                    <h5 class="modal-title" id="resignationLabel">HR Resignation Action</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>

                <div class="modal-body">

                    <asp:HiddenField ID="hfResignationId" runat="server" />
                    <asp:HiddenField ID="hfHrAction" runat="server" />

                    <!-- Action -->
                    <!-- Action -->
                    <div class="mb-3" id="divAction">
                        <label>Action</label>
                        <asp:DropDownList ID="ddlHrAction" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Select Action" Value="" />
                            <asp:ListItem Text="Accept" Value="Accepted" />
                            <asp:ListItem Text="Reject" Value="Rejected" />
                        </asp:DropDownList>
                    </div>

                    <div class="mb-3" id="divRemark">
                        <label>HR Remark</label>
                        <asp:TextBox ID="txtHrRemark" runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine" Rows="3" />
                    </div>

                    <div class="mb-3" id="divExtendedDays">
                        <label>Extended Notice Days</label>
                        <asp:TextBox ID="txtExtendedDays" runat="server"
                            CssClass="form-control" />
                    </div>

                    <div class="mb-3" id="divLastWorkingDate">
                        <label>Last Working Date</label>
                        <%--<asp:TextBox ID="txtLastWorkingDate" runat="server"
                            CssClass="form-control" TextMode="Date" />--%>
                        <div class="input-group">
                            <asp:TextBox runat="server" ID="txtLastWorkingDate"
                                CssClass="form-control"
                                autocomplete="off"
                                placeholder="Select resignation date" />
                            <span class="input-group-text">
                                <i class="fas fa-calendar-alt"></i>
                            </span>
                        </div>
                    </div>


                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary"
                        data-bs-dismiss="modal">
                        Cancel
                    </button>

                    <asp:Button ID="btnSubmitResignationAction" runat="server"
                        CssClass="btn btn-success"
                        Text="Submit"
                        OnClick="btnSubmitResignationAction_Click" />
                </div>

            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-lg rounded-3">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4"
                            Style="font-size: 2.0em; font-weight: bold;">Resignation List</asp:Label>
                        <div class="d-flex justify-content-end align-items-center">
                            <div class="app-search d-none d-lg-block" id="searchdata" runat="server">
                                <div class="position-relative">
                                    <input type="text" class="form-control" id="searchInput" placeholder="Search..." onkeydown="searchOnEnter(event)">
                                    <span class="bx bx-search-alt"></span>
                                </div>
                            </div>
                            <%--                            <button runat="server" id="btn_advanceserach" onserverclick="AdvSearchFunction" class="btn btn-outline-dark ms-2 light-border" title="Advance Search">
        <i class="bx bx-search-alt"></i>&nbsp;Advance Search
     
    </button>--%>
    &nbsp;
     
    <%--                            <button runat="server" id="btn_adduser" onserverclick="Button3_ServerClick" class="btn btn-primary ms-2" title="Add User">
        <i class="fas fa-user-plus"></i>&nbsp;Add User
     
    </button>--%>
                            <%--                            <asp:Button ID="btnBack1" runat="server" CssClass="btn btn-secondary" Text="Back" Visible="false" OnClick="btnBack1_click" />--%>
                        </div>
                    </div>

                    <!-- Search Row -->
                    <%--    <div class="row mb-3 align-items-end">
                        <!-- Dropdown -->
                        <div class="col-md-4">
                            <div class="form-group">
                                <label for="ddlcompany" class="fw-bold">Select Company</label>
                                <asp:DropDownList ID="ddlcompany" runat="server" CssClass="form-control custom-dropdown" TabIndex="6">
                                    <asp:ListItem Text="-- Select Company --" Value="" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <!-- Buttons: Search + Clear -->
                        <div class="col-md-8 d-flex align-items-end">
                            <div class="d-flex gap-2">
                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success" OnClick="btnsearch_click" />
                                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnclear_click" />
                            </div>
                        </div>
                    </div>--%>

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
                                    <asp:BoundField DataField="EmployeeEmail" HeaderText="Email"
                                        HeaderStyle-CssClass="col-email" ItemStyle-CssClass="col-email" />

                                    <asp:BoundField DataField="resignation_date" HeaderText="Resignation Date"
                                        DataFormatString="{0:yyyy-MM-dd}"
                                        HeaderStyle-CssClass="col-date" ItemStyle-CssClass="col-date" />

                                    <asp:BoundField DataField="notice_period_days" HeaderText="Notice (Days)"
                                        HeaderStyle-CssClass="col-notice" ItemStyle-CssClass="text-center col-notice" />

                                    <asp:BoundField DataField="last_working_date_display" HeaderText="Last Working Date"
                                        DataFormatString="{0:yyyy-MM-dd}"
                                        HeaderStyle-CssClass="col-last-date" ItemStyle-CssClass="col-last-date" />

                                    <asp:BoundField DataField="reason" HeaderText="Reason"
                                        HeaderStyle-CssClass="col-reason" ItemStyle-CssClass="col-reason" />

                                    <asp:TemplateField HeaderText="Authority Status" HeaderStyle-CssClass="col-authority" ItemStyle-CssClass="text-center col-authority">
                                        <ItemTemplate>
                                            <span class='badge <%# Convert.ToString(Eval("authority_status")).Trim().ToLower().Contains("manager") ? "bg-warning text-dark" : "bg-primary" %>'>
                                                <%# Convert.ToString(Eval("authority_status")) %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="hr_status" HeaderText="Hr Status"
                                        HeaderStyle-CssClass="col-status" ItemStyle-CssClass="col-status" />

                                    <asp:TemplateField HeaderText="Action" ItemStyle-CssClass="text-center">
                                        <ItemTemplate>
                                            <div class="action-wrap">
                                            <asp:LinkButton ID="lnkAccept" runat="server"
                                                CommandName="Accept"
                                                CommandArgument='<%# Eval("EmployeeResignationId") %>'
                                                Enabled='<%# !Convert.ToString(Eval("authority_status")).Trim().ToLower().Contains("manager")
                                                    && !Convert.ToString(Eval("hr_status")).Trim().ToLower().Contains("accept")
                                                    && !Convert.ToString(Eval("hr_status")).Trim().ToLower().Contains("reject") %>'
                                                CssClass='<%# (!Convert.ToString(Eval("authority_status")).Trim().ToLower().Contains("manager")
                                                    && !Convert.ToString(Eval("hr_status")).Trim().ToLower().Contains("accept")
                                                    && !Convert.ToString(Eval("hr_status")).Trim().ToLower().Contains("reject")) ? "btn btn-success btn-sm action-btn" : "btn btn-secondary btn-sm action-btn disabled" %>'>
                    Accept
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="lnkReject" runat="server"
                                                CommandName="Reject"
                                                CommandArgument='<%# Eval("EmployeeResignationId") %>'
                                                Enabled='<%# !Convert.ToString(Eval("authority_status")).Trim().ToLower().Contains("manager")
                                                    && !Convert.ToString(Eval("hr_status")).Trim().ToLower().Contains("accept")
                                                    && !Convert.ToString(Eval("hr_status")).Trim().ToLower().Contains("reject") %>'
                                                CssClass='<%# (!Convert.ToString(Eval("authority_status")).Trim().ToLower().Contains("manager")
                                                    && !Convert.ToString(Eval("hr_status")).Trim().ToLower().Contains("accept")
                                                    && !Convert.ToString(Eval("hr_status")).Trim().ToLower().Contains("reject")) ? "btn btn-danger btn-sm action-btn" : "btn btn-secondary btn-sm action-btn disabled" %>'>
                    Reject
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

    </script>
    <script type="text/javascript">
       <%-- function openResignationModal(resignationId) {
            document.getElementById('<%= hfResignationId.ClientID %>').value = resignationId;
            var modal = new bootstrap.Modal(document.getElementById('resignationModal'));
            modal.show();
        }--%>

        function openResignationModal(action) {

            document.getElementById('<%= txtExtendedDays.ClientID %>').value = "";
            document.getElementById('<%= txtLastWorkingDate.ClientID %>').value = "";
            document.getElementById('<%= txtHrRemark.ClientID %>').value = "";

            var ddl = document.getElementById('<%= ddlHrAction.ClientID %>');
            ddl.value = action;
            ddl.disabled = true;


            // store actual value for postback
            document.getElementById('<%= hfHrAction.ClientID %>').value = action;
            // Hide all sections first
            document.getElementById('divRemark').style.display = 'none';
            document.getElementById('divExtendedDays').style.display = 'none';
            document.getElementById('divLastWorkingDate').style.display = 'none';

            // Action-based UI
            if (action === 'Accepted') {
                document.getElementById('divLastWorkingDate').style.display = 'block';
                document.getElementById('divExtendedDays').style.display = 'block'; // optional, HR can set extra notice

            }

            if (action === 'Rejected') {
                document.getElementById('divRemark').style.display = 'block';
            }

            if (action === 'EarlyRelease') {
                document.getElementById('divLastWorkingDate').style.display = 'block';
            }

            if (action === 'ExtendedNotice') {
                document.getElementById('divExtendedDays').style.display = 'block';
            }

            var modal = new bootstrap.Modal(document.getElementById('resignationModal'));
            modal.show();
        }


        function closeResignationModal() {
            var modalEl = document.getElementById('resignationModal');
            var modal = bootstrap.Modal.getInstance(modalEl);
            modal.hide();
            resetResignationFields();

        }

        function resetResignationFields() {
            document.getElementById('<%= hfResignationId.ClientID %>').value = "";
            document.getElementById('<%= hfHrAction.ClientID %>').value = "";
            document.getElementById('<%= ddlHrAction.ClientID %>').value = "";
            document.getElementById('<%= txtHrRemark.ClientID %>').value = "";
            document.getElementById('<%= txtExtendedDays.ClientID %>').value = "";
            document.getElementById('<%= txtLastWorkingDate.ClientID %>').value = "";

            // Hide optional sections
            document.getElementById('divRemark').style.display = 'none';
            document.getElementById('divExtendedDays').style.display = 'none';
            document.getElementById('divLastWorkingDate').style.display = 'none';

            // Enable dropdown again
            document.getElementById('<%= ddlHrAction.ClientID %>').disabled = false;
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

    <script type="text/javascript">
        $(document).ready(function () {

            flatpickr("#<%= txtLastWorkingDate.ClientID %>", {
                   dateFormat: "d-m-Y",
                allowInput: true,
                minDate: "today"   // ✅ disables all past dates
               });



               // Click calendar icon to open datepicker
               $('.input-group-text').on('click', function () {
                   $(this).closest('.input-group').find('input').focus();
               });
           });


    </script>
    </div>
</asp:Content>
