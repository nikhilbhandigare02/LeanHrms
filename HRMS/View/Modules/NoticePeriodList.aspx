<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="NoticePeriodList.aspx.cs" Inherits="HRMS.View.Modules.NoticePeriodList" %>

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

        .page-list {
            display: flex;
            align-items: center;
            justify-content: flex-end;
            list-style: none;
            margin: 0;
            padding: 0;
            gap: 6px;
        }

            .page-list li {
                display: inline-block;
            }

        .page-btn {
            display: inline-block;
            min-width: 34px;
            padding: 6px 10px;
            text-align: center;
            border: 1px solid #dee2e6;
            border-radius: 6px;
            color: #495057;
            background-color: #fff;
            text-decoration: none;
            font-weight: 500;
            font-size: 13px;
            cursor: pointer;
        }

            .page-btn:hover {
                background-color: #f1f3f5;
                color: #495057;
                text-decoration: none;
            }

            .page-btn.active {
                background-color: #556ee6;
                border-color: #556ee6;
                color: #fff;
            }

            .page-btn[disabled],
            .page-btn.disabled {
                opacity: 0.5;
                pointer-events: none;
                cursor: default;
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
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4"
                            Style="font-size: 2.0em; font-weight: bold;">Notice Period List</asp:Label>
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

                                    <asp:BoundField DataField="resignation_date" HeaderText="Resignation Date"
                                        DataFormatString="{0:yyyy-MM-dd}"
                                        HeaderStyle-CssClass="col-date" ItemStyle-CssClass="col-date" />

                                    <asp:BoundField DataField="last_working_date_display" HeaderText="Last Working Date"
                                        DataFormatString="{0:yyyy-MM-dd}"
                                        HeaderStyle-CssClass="col-last-date" ItemStyle-CssClass="col-last-date" />

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
                                            <asp:LinkButton ID="lnkView" runat="server"
                                                CommandName="ViewNotice"
                                                CommandArgument='<%# Eval("EmployeeResignationId") %>'
                                                ToolTip="View"
                                                CssClass="btn btn-primary btn-sm action-btn icon-only">
                                                <i class="fas fa-eye"></i>
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

                                <ul class="page-list">
                                    <li>
                                        <asp:LinkButton ID="lnkPrevPage" runat="server" CssClass="page-btn" OnClick="lnkPrevPage_Click">&laquo; Prev</asp:LinkButton>
                                    </li>
                                    <asp:Repeater ID="rptPageNumbers" runat="server" OnItemCommand="rptPageNumbers_ItemCommand">
                                        <ItemTemplate>
                                            <li>
                                                <asp:LinkButton ID="lnkPageNumber" runat="server" CssClass='<%# (bool)Eval("IsActive") ? "page-btn active" : "page-btn" %>' CommandName="GoToPage" CommandArgument='<%# Eval("PageIndex") %>' Text='<%# Eval("PageNumber") %>'></asp:LinkButton>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <li>
                                        <asp:LinkButton ID="lnkNextPage" runat="server" CssClass="page-btn" OnClick="lnkNextPage_Click">Next &raquo;</asp:LinkButton>
                                    </li>
                                </ul>
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
