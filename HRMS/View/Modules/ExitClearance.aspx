<%@ Page Title="Exit Clearance" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="ExitClearance.aspx.cs" Inherits="HRMS.View.Modules.ExitClearance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .clearance-sections-container {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
        }
        .clearance-section {
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 20px;
            background-color: #f8f9fa;
        }
        .clearance-section-header {
            font-size: 18px;
            font-weight: bold;
            margin-bottom: 15px;
            color: #2563eb;
            border-bottom: 2px solid #2563eb;
            padding-bottom: 10px;
        }
        .clearance-item {
            display: flex;
            align-items: center;
            margin-bottom: 12px;
        }
        .clearance-item input[type="checkbox"] {
            margin-right: 10px;
            width: 18px;
            height: 18px;
            cursor: pointer;
        }
        .clearance-item label {
            margin: 0;
            cursor: pointer;
            font-weight: 500;
        }
        .clearance-remarks {
            width: 100%;
            margin-top: 15px;
        }
        .clearance-remarks textarea {
            width: 100%;
            min-height: 80px;
            padding: 10px;
            border: 1px solid #ced4da;
            border-radius: 4px;
            resize: vertical;
        }
        .employee-info {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 25px;
            border-radius: 12px;
            margin-bottom: 25px;
            color: white;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }
        .employee-info-header {
            font-size: 22px;
            font-weight: bold;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 2px solid rgba(255, 255, 255, 0.3);
        }
        .employee-info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 15px;
        }
        .employee-info-item {
            display: flex;
            flex-direction: column;
        }
        .employee-info-label {
            font-weight: 600;
            font-size: 13px;
            color: rgba(255, 255, 255, 0.8);
            margin-bottom: 5px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        .employee-info-value {
            font-size: 16px;
            font-weight: 500;
            color: white;
        }
        .btn-submit {
            background-color: #2563eb;
            color: white;
            padding: 10px 30px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
            font-weight: bold;
        }
        .btn-submit:hover {
            background-color: #1d4ed8;
        }
        .btn-back {
            background-color: #6c757d;
            color: white;
            padding: 10px 30px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
            font-weight: bold;
            margin-right: 10px;
        }
        .btn-back:hover {
            background-color: #5a6268;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-lg rounded-3">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <asp:Label runat="server" ID="lblTitle" CssClass="card-title mb-4"
                            Style="font-size: 2.0em; font-weight: bold;">Exit Clearance</asp:Label>
                    </div>

                    <!-- Employee Selection -->
                    <div class="form-group mb-4">
                        <label for="ddlEmployee" style="font-weight: bold; font-size: 14px;">Select Employee:</label>
                        <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlEmployee_SelectedIndexChanged">
                            <asp:ListItem Text="-- Select Employee --" Value="" />
                        </asp:DropDownList>
                    </div>

                    <!-- Employee Information -->
                    <asp:Panel ID="employeeInfo" runat="server" CssClass="employee-info" Visible="false">
                        <div class="employee-info-header">Employee Details</div>
                        <div class="employee-info-grid">
                            <div class="employee-info-item">
                                <span class="employee-info-label">Employee Name</span>
                                <asp:Label ID="lblEmployeeName" runat="server" CssClass="employee-info-value"></asp:Label>
                            </div>
                            <div class="employee-info-item">
                                <span class="employee-info-label">Employee Code</span>
                                <asp:Label ID="lblEmployeeCode" runat="server" CssClass="employee-info-value"></asp:Label>
                            </div>
                            <div class="employee-info-item">
                                <span class="employee-info-label">Email</span>
                                <asp:Label ID="lblEmployeeEmail" runat="server" CssClass="employee-info-value"></asp:Label>
                            </div>
                            <div class="employee-info-item">
                                <span class="employee-info-label">Resignation Date</span>
                                <asp:Label ID="lblResignationDate" runat="server" CssClass="employee-info-value"></asp:Label>
                            </div>
                            <div class="employee-info-item">
                                <span class="employee-info-label">Last Working Date</span>
                                <asp:Label ID="lblLastWorkingDate" runat="server" CssClass="employee-info-value"></asp:Label>
                            </div>
                            <%--<div class="employee-info-item">
                                <span class="employee-info-label">Resignation ID</span>
                                <asp:Label ID="lblResignationId" runat="server" CssClass="employee-info-value"></asp:Label>
                            </div>--%>
                        </div>
                    </asp:Panel>

                    <!-- Hidden Fields -->
                    <asp:HiddenField ID="hfEmployeeResignationId" runat="server" />
                    <asp:HiddenField ID="hfEmployeeCode" runat="server" />
                    <asp:HiddenField ID="hfUserId" runat="server" />
                    <asp:HiddenField ID="hfExitClearanceId" runat="server" />

                    <!-- Clearance Sections Container -->
                    <asp:Panel ID="pnlClearanceSections" runat="server" CssClass="clearance-sections-container">
                        <!-- Dynamic clearance sections will be loaded here -->
                    </asp:Panel>

                    <!-- Action Buttons -->
                    <asp:Panel ID="actionButtons" runat="server" CssClass="text-center mt-4" Visible="false">
                        <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn-back" OnClick="btnBack_Click" />
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit Clearance" CssClass="btn-submit" OnClick="btnSubmit_Click" />
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
</asp:Content>
