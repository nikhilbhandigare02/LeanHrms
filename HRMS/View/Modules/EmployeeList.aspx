<%@ Page Title="Employee List" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="EmployeeList.aspx.cs" Inherits="HRMS.View.Modules.EmployeeList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script src="../../assets/js/commonFunctions.js"></script>
    <style>
        .gridview-pagination a {
            margin-right: 10px;
        }
    </style>

    <style>
        .drawer {
            display: none;
            position: fixed;
            right: 0;
            top: 0;
            height: 100%;
            width: 350px;
            background-color: #fff;
            box-shadow: -1px 0 10px rgba(0, 0, 0, 0.1);
            overflow: hidden;
            padding: 20px;
        }

            .drawer.show {
                display: block;
            }

        .drawer-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }

        div.scroll {
            overflow-x: auto;
            white-space: nowrap;
            max-width: 19%;
            height: auto
        }

        .close-button {
            cursor: pointer;
        }

       /* .small-font {
           font-size: 14px;
        }*/

        .page-container {
            margin-left: 20px;
            margin-right: 20px;
        }

        .pagination-container {
            margin-top: 20px;
            padding: 10px;
        }

        .app-search .position-relative {
            display: flex;
        }

        .app-search input {
            border-radius: 0;
        }

        .app-search .btn {
            border-radius: 0;
        }

        .app-search .input-group-append {
            position: absolute;
            right: 0;
            top: 0;
            bottom: 0;
            display: flex;
            align-items: center;
        }

        .drawer-content {
            display: flex;
            flex-direction: column;
            height: 100%;
        }

        .drawer-footer {
            margin-top: auto;
            display: flex;
            justify-content: flex-end;
            align-items: center;
        }
    </style>
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
    </style>
    <style>
        .highlighted {
            background-color: rgba(0, 0, 0, 0.5);
            color: #ffffff;
        }
    </style>
    <style>
        .view-link {
            color: grey;
        }

        .edit-link {
            color: blue;
        }

        .comment-link {
            color: orange;
        }

        .phone-link {
            color: #19b512;
        }

        .delete-link {
            color: red;
        }
    </style>
    <style>
        .pagination-container {
            display: flex;
            align-items: center;
        }

            .pagination-container button,
            .pagination-container select {
                margin: 0;
            }

        .custom-gridview {
            border-left: none;
            border-right: none;
        }

            .custom-gridview th,
            .custom-gridview td {
                border-left: none;
                border-right: none;
            }
    </style>
    <style>
        #overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.3);
            backdrop-filter: blur(3px);
            z-index: 999;
            animation: fadeInOverlay 0.3s ease-in-out;
        }

        #popup {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: #fff;
            border: 1px solid #ccc;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
            z-index: 1000;
            padding: 20px;
            max-width: 400px;
            height: 200px;
            animation: fadeInPopup 0.3s ease-in-out;
        }

        #phonePopup {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: #fff;
            border: 1px solid #ccc;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
            z-index: 1000;
            padding: 20px;
            max-width: 400px;
            height: 200px;
            animation: fadeInPopup 0.3s ease-in-out;
        }


        #DeletePopUp {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: #fff;
            border: 1px solid #ccc;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
            z-index: 1000;
            padding: 20px;
            max-width: 400px;
            height: 200px;
            animation: fadeInPopup 0.3s ease-in-out;
        }

        #DeleteMessage {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: #fff;
            border: 1px solid #ccc;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
            z-index: 1000;
            padding: 20px;
            max-width: 400px;
            height: 200px;
            animation: fadeInPopup 0.3s ease-in-out;
        }

        @keyframes fadeInOverlay {
            from {
                opacity: 0;
            }

            to {
                opacity: 1;
            }
        }

        @keyframes fadeInPopup {
            from {
                opacity: 0;
                transform: translate(-50%, -50%) scale(0.8);
            }

            to {
                opacity: 1;
                transform: translate(-50%, -50%) scale(1);
            }
        }
    </style>
    <style>
        #phonePopupPhoneNumber,
        #phonePopupMobileNumber,
        #phonePopupLabels {
            font-size: 16px;
        }

        .light-border {
            border-color: lightgrey;
        }

        :root {
            --employee-primary: #2563EB;
            --employee-text: #0B1B45;
            --employee-muted: #64748B;
            --employee-border: #DDE6F3;
            --employee-soft: #F5F8FD;
            --employee-success: #0F9F6E;
            --employee-warning: #F97316;
        }

        .employee-list-page {
            background: #F7FAFF;
            border-radius: 0;
            margin: -10px -6px 0;
            min-height: calc(100vh - 92px);
            padding: 28px 30px 34px;
        }

        .employee-list-header {
            align-items: flex-start;
            display: flex;
            gap: 24px;
            justify-content: space-between;
            margin-bottom: 24px;
        }

        .employee-list-title {
            color: var(--employee-text);
            font-size: 28px;
            font-weight: 900;
            letter-spacing: 0;
            margin: 0 0 6px;
        }

        .employee-list-subtitle {
            color: #536486;
            font-size: 14px;
            font-weight: 600;
            margin: 0 0 14px;
        }

        .employee-breadcrumb {
            align-items: center;
            color: #7182A4;
            display: flex;
            flex-wrap: wrap;
            font-size: 13px;
            font-weight: 700;
            gap: 10px;
        }

        .employee-breadcrumb span:last-child {
            color: var(--employee-primary);
        }

        .employee-list-toolbar {
            align-items: center;
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            justify-content: flex-end;
            min-width: 520px;
        }

        .employee-search {
            position: relative;
        }

        .employee-search input {
            background: #FFFFFF;
            border: 1px solid var(--employee-border);
            border-radius: 8px;
            box-shadow: 0 8px 24px rgba(15, 23, 42, .04);
            color: var(--employee-text);
            font-size: 13px;
            font-weight: 600;
            height: 48px;
            min-width: 390px;
            padding: 0 16px 0 44px;
        }

        .employee-search .bx {
            color: #1D4ED8;
            font-size: 20px;
            left: 16px;
            position: absolute;
            top: 14px;
        }

        .employee-toolbar-btn {
            align-items: center;
            border-radius: 8px !important;
            display: inline-flex;
            font-size: 13px;
            font-weight: 800;
            gap: 9px;
            height: 48px;
            justify-content: center;
            min-width: 150px;
            padding: 0 18px;
        }

        .employee-btn-outline {
            background: #FFFFFF;
            border: 1px solid var(--employee-border);
            color: #10213F;
        }

        .employee-btn-primary {
            background: linear-gradient(180deg, #3367F6 0%, #1D4FE6 100%);
            border: 1px solid #1D4FE6;
            box-shadow: 0 12px 24px rgba(37, 99, 235, .18);
            color: #FFFFFF;
        }

        .employee-stats-grid {
            display: grid;
            gap: 16px;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            margin-bottom: 24px;
            position: relative;
        }

        .employee-stat-card {
            align-items: center;
            background: #FFFFFF;
            border: 1px solid var(--employee-border);
            border-radius: 12px;
            box-shadow: 0 14px 32px rgba(15, 23, 42, .045);
            color: inherit;
            cursor: pointer;
            display: flex;
            gap: 18px;
            min-height: 134px;
            padding: 22px;
            text-decoration: none;
            transition: border-color .2s ease, box-shadow .2s ease, transform .2s ease;
            width: 100%;
        }

        .employee-stat-card:hover,
        .employee-stat-card:focus {
            border-color: #2563EB;
            box-shadow: 0 18px 40px rgba(37, 99, 235, .12);
            outline: none;
            text-decoration: none;
            transform: translateY(-2px);
        }

        .employee-stat-card.is-active {
            border-color: #2563EB;
            box-shadow: 0 18px 40px rgba(37, 99, 235, .16);
        }

        .employee-stat-trigger {
            height: 0;
            opacity: 0;
            overflow: hidden;
            position: absolute;
            pointer-events: none;
            width: 0;
        }

        .employee-stat-icon {
            align-items: center;
            border-radius: 50%;
            display: inline-flex;
            flex: 0 0 78px;
            font-size: 32px;
            height: 78px;
            justify-content: center;
            width: 78px;
        }

        .employee-stat-icon.blue { background: #E7EDFF; color: #2563EB; }
        .employee-stat-icon.green { background: #DDF8EA; color: #10B981; }
        .employee-stat-icon.orange { background: #FFE9CC; color: #F97316; }
        .employee-stat-icon.purple { background: #F0E4FF; color: #8B5CF6; }

        .employee-stat-label {
            color: #223153;
            font-size: 13px;
            font-weight: 800;
            margin-bottom: 8px;
        }

        .employee-stat-value {
            color: #0B1B45;
            font-size: 30px;
            font-weight: 900;
            line-height: 1;
            margin-bottom: 8px;
        }

        .employee-stat-caption {
            color: #66799D;
            font-size: 12px;
            font-weight: 700;
        }

        .employee-list-card {
            background: #FFFFFF;
            border: 1px solid var(--employee-border);
            border-radius: 12px;
            box-shadow: 0 18px 42px rgba(15, 23, 42, .05);
            overflow: hidden;
        }

        .employee-list-card .table-responsive {
            margin: 0;
        }

        .employee-table {
            color: var(--employee-text);
            margin: 0 !important;
            width: 100%;
        }

        .employee-table th {
            background: #F7FAFE;
            border-bottom: 1px solid var(--employee-border) !important;
            color: #253A63;
            font-size: 12px;
            font-weight: 900;
            height: 54px;
            padding: 14px 18px !important;
            white-space: nowrap;
        }

        .employee-table td {
            border-bottom: 1px solid #E8EEF7 !important;
            color: #0F2453;
            font-size: 13px;
            font-weight: 400;
            padding: 16px 18px !important;
            vertical-align: middle;
        }

        .employee-table tr:hover td {
            background: #F8FBFF;
        }

        .employee-action-stack {
            align-items: center;
            display: flex;
            gap: 10px;
            justify-content: center;
        }

        .employee-action-btn {
            align-items: center;
            background: #FFFFFF;
            border: 1px solid #D8E3F3;
            border-radius: 8px;
            color: #2563EB;
            display: inline-flex;
            height: 34px;
            justify-content: center;
            transition: background .18s ease, border-color .18s ease, transform .18s ease;
            width: 34px;
        }

        .employee-action-btn:hover {
            background: #EFF6FF;
            border-color: #AFC7FF;
            color: #1D4ED8;
            transform: translateY(-1px);
        }

        .employee-action-btn.danger {
            border-color: #FFD2D7;
            color: #EF4444;
        }

        .employee-action-btn.danger:hover {
            background: #FFF1F2;
            border-color: #FDA4AF;
            color: #DC2626;
        }

        .employee-pagination {
            align-items: center;
            display: flex;
            justify-content: flex-end;
            padding: 14px 18px;
        }

        .employee-pagination select {
            border: 1px solid var(--employee-border) !important;
            border-radius: 8px;
            color: #536486 !important;
            font-size: 13px;
            font-weight: 700;
            min-width: 100px;
            padding: 8px 12px !important;
        }

        .employee-advanced-card {
            background: #FFFFFF;
            border: 1px solid var(--employee-border);
            border-radius: 12px;
            box-shadow: 0 14px 32px rgba(15, 23, 42, .045);
            margin-bottom: 18px;
            padding: 18px;
        }

        .employee-advanced-card label {
            color: #24324A;
            font-size: 12px;
            font-weight: 800;
        }

        @media (max-width: 1199px) {
            .employee-list-header {
                display: block;
            }

            .employee-list-toolbar {
                justify-content: flex-start;
                margin-top: 18px;
                min-width: 0;
            }

            .employee-search input {
                min-width: 280px;
            }

            .employee-stats-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 767px) {
            .employee-list-page {
                padding: 18px 14px 26px;
            }

            .employee-stats-grid {
                grid-template-columns: 1fr;
            }

            .employee-search,
            .employee-search input,
            .employee-toolbar-btn {
                width: 100%;
            }
        }

        .user-list-page {
            background: transparent;
            margin: 20px;
            min-height: auto;
            padding: 0;
        }

        .user-list-header {
            align-items: center;
            display: flex;
            flex-wrap: wrap;
            gap: 14px;
            justify-content: space-between;
            margin-bottom: 18px;
        }

        .user-list-page .employee-list-title {
            color: #222;
            font-size: 22px;
            font-weight: 700;
            margin: 0;
        }

        .user-list-toolbar {
            align-items: center;
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            justify-content: flex-end;
        }

        .user-list-page .employee-search input {
            border: 1px solid #ced4da;
            border-radius: 4px;
            box-shadow: none;
            height: 38px;
            min-width: 280px;
            padding-left: 38px;
        }

        .user-list-page .employee-search .bx {
            font-size: 17px;
            left: 13px;
            top: 10px;
        }

        .user-list-card {
            background: transparent;
            border: 0;
            border-radius: 0;
            box-shadow: none;
            overflow: visible;
        }

        .user-list-page .table-responsive {
            background: #FFFFFF;
            border: 1px solid #dee2e6;
            border-radius: 4px;
            margin: 0;
            overflow-x: auto;
        }

        .user-list-page .custom-gridview {
            margin-bottom: 0 !important;
        }

        .user-list-page .custom-gridview th {
            background: #f8f9fa;
            color: #212529;
            font-size: 13px;
            font-weight: 600;
            padding: 10px 12px !important;
            white-space: nowrap;
        }

        .user-list-page .custom-gridview td {
            color: #212529;
            font-size: 13px;
            font-weight: 400;
            padding: 10px 12px !important;
            vertical-align: middle;
        }

        .user-list-page .employee-action-stack {
            justify-content: flex-start;
        }

        .user-list-page .employee-action-btn {
            border-radius: 4px;
            height: 30px;
            width: 30px;
        }

        .user-list-page .employee-pagination {
            justify-content: flex-start;
            padding: 12px 0;
        }

        .user-list-page .employee-advanced-card {
            border-radius: 4px;
            box-shadow: none;
        }

        @media (max-width: 767px) {
            .user-list-page {
                margin: 12px;
            }

            .user-list-toolbar,
            .user-list-page .employee-search,
            .user-list-page .employee-search input,
            .user-list-toolbar .btn {
                width: 100%;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Bootstrap Modal -->
    <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="deleteModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="deleteModalLabel">Confirm Deletion</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true" onclick="closeDeleteModal()">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    Are you sure you want to delete this User?
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" onclick="closeDeleteModal()" data-dismiss="modal">Cancel</button>
                    <asp:Button ID="confirmDeleteButton" runat="server" type="button" class="btn btn-danger" OnClick="confirmDeleteButton_Click" Text="Delete" />
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnClickedUserId" runat="server" />
    <div id="pageShell" runat="server" class="employee-list-page">
        <div id="pageHeader" runat="server" class="employee-list-header">
            <div>
                <h1 class="employee-list-title"><asp:Literal ID="litPageTitle" runat="server" Text="Employee List" /></h1>
            </div>

            <div id="pageToolbar" runat="server" class="employee-list-toolbar">
                <div class="employee-search" id="searchdata" runat="server">
                    <input type="text" id="searchInput" placeholder="Search by employee code, username, email..." onkeydown="searchOnEnter(event)">
                    <span class="bx bx-search-alt"></span>
                </div>
                <button runat="server" id="btn_advanceserach" onserverclick="AdvSearchFunction" class="employee-toolbar-btn employee-btn-outline" title="Advance Search">
                    <i class="bx bx-filter-alt"></i> Advance Search
                </button>
                <button runat="server" id="btn_adduser" onserverclick="Button3_ServerClick" class="employee-toolbar-btn employee-btn-primary" title="Add Employee">
                    <i class="fas fa-plus"></i> Add Employee
                </button>
                <asp:Button ID="btnBack1" runat="server" CssClass="employee-toolbar-btn employee-btn-outline" Text="Back" Visible="false" OnClick="btnBack1_click" />
            </div>
        </div>

        <div id="employeeStatsGrid" runat="server" class="employee-stats-grid">
            <asp:LinkButton ID="btnFilterTotalEmployees" runat="server" CssClass="employee-stat-trigger" CommandArgument="All" OnClick="EmployeeStatCard_Click" CausesValidation="false" />
            <asp:LinkButton ID="btnFilterActiveEmployees" runat="server" CssClass="employee-stat-trigger" CommandArgument="Active" OnClick="EmployeeStatCard_Click" CausesValidation="false" />
            <asp:LinkButton ID="btnFilterProbationEmployees" runat="server" CssClass="employee-stat-trigger" CommandArgument="Probation" OnClick="EmployeeStatCard_Click" CausesValidation="false" />
            <asp:LinkButton ID="btnFilterNewJoiners" runat="server" CssClass="employee-stat-trigger" CommandArgument="NewJoiners" OnClick="EmployeeStatCard_Click" CausesValidation="false" />

            <div id="cardTotalEmployees" runat="server" class="employee-stat-card">
                <span class="employee-stat-icon blue"><i class="fas fa-users"></i></span>
                <div>
                    <div class="employee-stat-label">Total Employees</div>
                    <div class="employee-stat-value"><asp:Literal ID="litTotalEmployees" runat="server" Text="0" /></div>
                    <div class="employee-stat-caption">All Employees</div>
                </div>
            </div>
            <div id="cardActiveEmployees" runat="server" class="employee-stat-card">
                <span class="employee-stat-icon green"><i class="fas fa-user-check"></i></span>
                <div>
                    <div class="employee-stat-label">Active Employees</div>
                    <div class="employee-stat-value"><asp:Literal ID="litActiveEmployees" runat="server" Text="0" /></div>
                    <div class="employee-stat-caption">Currently Active</div>
                </div>
            </div>
            <div id="cardProbationEmployees" runat="server" class="employee-stat-card">
                <span class="employee-stat-icon orange"><i class="fas fa-hourglass-half"></i></span>
                <div>
                    <div class="employee-stat-label">On Probation</div>
                    <div class="employee-stat-value"><asp:Literal ID="litProbationEmployees" runat="server" Text="0" /></div>
                    <div class="employee-stat-caption">Under Probation</div>
                </div>
            </div>
            <div id="cardNewJoiners" runat="server" class="employee-stat-card">
                <span class="employee-stat-icon purple"><i class="fas fa-user-plus"></i></span>
                <div>
                    <div class="employee-stat-label">New Joiners</div>
                    <div class="employee-stat-value"><asp:Literal ID="litNewJoiners" runat="server" Text="0" /></div>
                    <div class="employee-stat-caption">This Month</div>
                </div>
            </div>
        </div>

        <div class="overlay" onclick="closeFilterDrawer()"></div>
        <div id="advancedSearchFields" class="employee-advanced-card row" runat="server" visible="false">
            <div class="col-lg-4">
                <div class="mb-3">
                    <label for="input-ddl_employeeCode">Employee Code</label>
                    <asp:DropDownList ID="ddl_employeeCode" CssClass="form-control custom-dropdown" runat="server"></asp:DropDownList>
                </div>
            </div>
            <div class="col-lg-4">
                <div class="mb-3">
                    <label for="input-ddl_username">Username</label>
                    <asp:DropDownList ID="ddl_username" CssClass="form-control" runat="server"></asp:DropDownList>
                </div>
            </div>
            <div class="col-lg-4">
                <div class="mb-3">
                    <label for="input-contact-number">Contact Number</label>
                    <asp:TextBox ID="txt_contact" runat="server" CssClass="form-control" TabIndex="4"
                        placeholder="Enter Contact Number" onblur="checkNoLeadingSpace(this)" oninput="formatPhoneNumber(this)"></asp:TextBox>
                    <asp:CustomValidator ID="cvPhoneNumber" runat="server" ControlToValidate="txt_contact"
                        ClientValidationFunction="validatePhoneNumber"
                        ErrorMessage="Invalid phone number format. Example: +91 XXXXXXXXXX"
                        ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                    <asp:RegularExpressionValidator ID="revPhoneNumber" runat="server" ControlToValidate="txt_contact"
                        ValidationExpression="^\+91 \d{10}$" ErrorMessage="Invalid phone number format. Example: +91 9876523451"
                        ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                </div>
            </div>
            <div class="col-12">
                <asp:Button runat="server" ID="Button1" Text="Search" CssClass="employee-toolbar-btn employee-btn-primary" OnClick="AdvSearchButton_Click" />
                <asp:Button runat="server" ID="Button7" Text="Clear" CssClass="employee-toolbar-btn employee-btn-outline" OnClick="AdvClearButton_Click" />
                <asp:Button runat="server" ID="Button8" Text="Back" CssClass="employee-toolbar-btn employee-btn-outline" OnClientClick="highlightBackButton();" OnClick="AdvBackButton_Click" />
            </div>
        </div>

        <div id="listCard" runat="server" class="employee-list-card">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="table-responsive">
                        <asp:HiddenField ID="hfPageIndexViewUser" runat="server" />
                        <asp:GridView runat="server" ID="gridview" CssClass="table employee-table custom-gridview" AutoGenerateColumns="false" DataKeyNames="UserId" OnRowCommand="gvUsers_RowCommand" EnablePersistedSelection="true" OnPageIndexChanging="OnPageIndexChanging" PageSize="10"
                            AllowSorting="true" OnSorting="gridview_Sorting" Style="margin: 0 auto;" EmptyDataText="No records found.">


                                    <Columns>
                                        <asp:TemplateField HeaderText="SR No" ItemStyle-Width="80px">
                                            <ItemTemplate>
                                                <%# (Convert.ToInt32(hfPageIndexViewUser.Value) * gridview.PageSize) + Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>



                                        <%--<asp:TemplateField DataField="EmployeeCode" HeaderText="Employee Code" HeaderStyle-CssClass="header-cell" ItemStyle-Width="80px">                                    
                                </asp:TemplateField>--%>
                                        <asp:BoundField DataField="EmployeeCode" HeaderText="Employee Code" SortExpression="EmployeeCode" ItemStyle-Width="130px" Visible="true" />
                                        <asp:BoundField DataField="UserId" HeaderText="User Id" ItemStyle-Width="130px" Visible="false" />
                                        <asp:BoundField DataField="Username" HeaderText="Username" SortExpression="Username" ItemStyle-Width="130px" />
                                        <asp:BoundField DataField="user_fullname" HeaderText="Employee Name" SortExpression="user_fullname" ItemStyle-Width="170px" />
                                        <asp:BoundField DataField="user_mail_id" HeaderText="Email Id" SortExpression="user_mail_id" ItemStyle-Width="190px" />
                                        <asp:BoundField DataField="contact_detail" HeaderText="Contact Number" SortExpression="contact_detail" ItemStyle-Width="150px" />
                                        <asp:BoundField DataField="designation_name" HeaderText="Designation" SortExpression="designation_name" ItemStyle-Width="150px" />
                                        <asp:TemplateField HeaderText="Action" ItemStyle-Width="100px">
                                            <ItemTemplate>
                                                <div class="employee-action-stack">
                                                    <asp:LinkButton ID="lnkView" runat="server" CssClass="employee-action-btn" CommandName="viewUser" CommandArgument='<%# Eval("UserId") + "|" + Eval("EmployeeCode") %>' title="Update Employee">
                                                        <i class="far fa-edit"></i>
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="lnkDelete" CommandName="deleteUser" CssClass="employee-action-btn danger" runat="server" CommandArgument='<%# Eval("UserId") + "|" + Eval("EmployeeCode") %>' title="Delete Employee">
                                                        <i class="far fa-trash-alt"></i>
                                                    </asp:LinkButton>
                                                </div>
                                            </ItemTemplate>

                                        </asp:TemplateField>
                                        <%-- <asp:BoundField DataField="roledescription" HeaderText="Role" />--%>
                                    </Columns>
                                    <PagerStyle CssClass="gridview-pagination" />

                                </asp:GridView>

                    </div>
                    <div class="employee-pagination">
                        <asp:DropDownList runat="server" ID="ddlPageSelector" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSelector_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gridview" EventName="RowCommand" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
    <!-- end col -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script>
        function showUserSavedMessage(status, remark) {
            Swal.fire({

                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }
    </script>

    <script type="text/javascript">
        window.onload = function () {
            var dropdownIds = [
            '<%= ddl_employeeCode.ClientID %>',
            '<%= ddl_username.ClientID %>'
            ];

            dropdownIds.forEach(function (id) {
                var ddl = document.getElementById(id);
                if (ddl && ddl.options.length > 0) {
                    ddl.options[0].disabled = true;
                    ddl.options[0].style.color = 'gray';
                }
            });
        };
    </script>

    <%--new code--%>
    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script type="text/javascript">
        function openDeleteModal() {
            // Show the modal
            $('#deleteModal').modal('show');
            return false;
        }
        function closeDeleteModal() {
            // Show the modal
            $('#deleteModal').modal('hide');
            return false;
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
      <%= gridview.ClientID %> 
            $('#<%= gridview.ClientID %> tr:has(td)').hide();

            if (searchTerm === '') {
                $('#<%= gridview.ClientID %> tr:has(td)').show();
              } else {
                  var genderColumnIndex = 3;

                  $('#<%= gridview.ClientID %> tr:has(td)').filter(function () {
                    var found = false;
                    $(this).find('td').each(function (index) {
                        var cellText = $(this).text().toLowerCase();

                        if (index === genderColumnIndex && cellText.startsWith(searchTerm)) {
                            found = true;
                            return false;
                        }

                        if (index !== genderColumnIndex && cellText.includes(searchTerm)) {
                            found = true;
                            return false;
                        }
                    });
                    return found;
                }).show();
            }
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

    <script>
        function validatePhoneNumber(sender, args) {
            var phoneNumberPattern = /^\+91 \d{10}$/;
            args.IsValid = phoneNumberPattern.test(args.Value);
        }

        function formatPhoneNumber(element) {
            var input = element.value.replace(/[^\d]/g, ''); // Remove all non-numeric characters
            if (input.startsWith('91')) {
                input = input.substring(2); // Remove leading '91' if already present
            }
            if (input.length > 10) {
                input = input.substring(0, 10); // Limit to 10 digits
            }
            if (input.length > 0) {
                input = '+91 ' + input;
            }
            element.value = input;
        }
    </script>

</asp:Content>

