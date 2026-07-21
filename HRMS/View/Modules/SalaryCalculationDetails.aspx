<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="SalaryCalculationDetails.aspx.cs" Inherits="HRMS.View.Modules.SalaryCalculationDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css" rel="stylesheet" />
    <style>
        .salary-details-page {
            --sd-primary: #2563EB;
            --sd-primary-dark: #1D4ED8;
            --sd-heading: #1E293B;
            --sd-muted: #64748B;
            --sd-line: #E8EEF7;
            --sd-panel: #F8FAFC;
            overflow: hidden; /* Prevent page scrolling */
        }

        .salary-details-page .sd-card {
            background: #FFFFFF;
            border: 1px solid var(--sd-line);
            border-radius: 12px;
            box-shadow: 0 10px 30px rgba(15, 23, 42, 0.05);
            padding: 16px 20px; /* Reduce padding */
            margin-bottom: 16px;
        }

        .salary-details-page .sd-title {
            color: var(--sd-heading);
            font-size: 18px; /* Smaller title font */
            font-weight: 700;
            margin-bottom: 16px;
        }

        .salary-details-page .form-group {
            margin-bottom: 12px; /* Reduce spacing between fields */
        }

        .salary-details-page .form-label {
            font-size: 12px; /* Smaller label font */
            font-weight: 600;
            color: var(--sd-heading);
            margin-bottom: 4px;
            display: block;
        }

        .salary-details-page .form-control {
            width: 100%;
            padding: 6px 10px; /* Smaller padding */
            border: 1px solid var(--sd-line);
            border-radius: 6px;
            font-size: 12px; /* Smaller input font */
            color: var(--sd-heading);
            background: #fff;
        }

        .salary-details-page .form-control:disabled,
        .salary-details-page .form-control[readonly] {
            background: var(--sd-panel);
            cursor: not-allowed;
        }

        .salary-details-page .row {
            display: flex;
            flex-wrap: wrap;
            margin-right: -6px;
            margin-left: -6px;
        }

        .salary-details-page .col-md-6 {
            flex: 0 0 50%;
            max-width: 50%;
            padding-right: 6px;
            padding-left: 6px;
            box-sizing: border-box;
        }

        @media (max-width: 768px) {
            .salary-details-page .col-md-6 {
                flex: 0 0 100%;
                max-width: 100%;
            }
        }

        .salary-details-page .btn {
            padding: 6px 16px;
            border-radius: 6px;
            font-size: 12px;
            font-weight: 600;
            border: none;
            cursor: pointer;
            transition: all 0.2s;
        }

        .salary-details-page .btn-primary {
            background: var(--sd-primary);
            color: white;
        }

        .salary-details-page .btn-primary:hover {
            background: var(--sd-primary-dark);
        }

        .salary-details-page .btn-secondary {
            background: var(--sd-line);
            color: var(--sd-heading);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="salary-details-page">
        <div class="sd-card">
            <div class="sd-title">
                <%= (Mode == "view" ? "View" : "Edit") %> Salary Calculation Details
            </div>

            <div class="row">
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Employee Code</label>
                        <asp:TextBox ID="txtEmployeeCode" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Employee Name</label>
                        <asp:TextBox ID="txtEmployeeName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Email</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Verification Status</label>
                        <asp:TextBox ID="txtVerificationStatus" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Monthly Salary</label>
                        <asp:TextBox ID="txtMonthlySalary" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Per Day Salary</label>
                        <asp:TextBox ID="txtPerDaySalary" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Total Working Days</label>
                        <asp:TextBox ID="txtTotalWorkingDays" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Present Days</label>
                        <asp:TextBox ID="txtPresentDays" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Absent Days</label>
                        <asp:TextBox ID="txtAbsentDays" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Leave Deduction Days</label>
                        <asp:TextBox ID="txtLeaveDeductionDays" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtLeaveDeductionDays_TextChanged"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Other Deduction</label>
                        <asp:TextBox ID="txtOtherDeduction" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtOtherDeduction_TextChanged"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Deducted Amount</label>
                        <asp:TextBox ID="txtDeductedAmount" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-6">
                    <div class="form-group">
                        <label class="form-label">Net Salary</label>
                        <asp:TextBox ID="txtNetSalary" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div style="margin-top: 24px;">
                <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary" Text="Back to List" OnClick="btnBack_Click" style="margin-right: 8px;" />
                <asp:Button ID="btnVerifyUpdate" runat="server" CssClass="btn btn-primary" Text="Verify & Update" OnClick="btnVerifyUpdate_Click" Visible="false" />
            </div>
        </div>
    </div>
</asp:Content>
