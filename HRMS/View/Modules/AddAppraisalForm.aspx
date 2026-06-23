<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AddAppraisalForm.aspx.cs" Inherits="HRMS.View.Modules.AddAppraisalForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css" rel="stylesheet" />
    <style>
        :root {
            --hrms-primary: #2563eb;
            --hrms-border: #dbe4f0;
            --hrms-text: #10213f;
            --hrms-muted: #64748b;
            --hrms-bg: #f6f9fe;
            --hrms-success: #16a34a;
            --hrms-warning: #f97316;
        }

        .appraisal-form-page {
            max-width: 1380px;
            margin: 0 auto;
            padding: 8px 18px 28px;
            color: var(--hrms-text);
        }

        .appraisal-breadcrumb {
            display: flex;
            align-items: center;
            gap: 10px;
            color: #31537f;
            font-size: 12px;
            font-weight: 700;
            margin-bottom: 12px;
        }

        .appraisal-title-row {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 16px;
            margin-bottom: 14px;
        }

        .appraisal-form-title {
            display: block;
            font-size: 22px;
            line-height: 1.2;
            font-weight: 800;
            color: #071733;
            margin-bottom: 4px;
        }

        .appraisal-form-subtitle {
            font-size: 13px;
            color: #425b7c;
            margin: 0;
        }

        .sec-card {
            background: #fff;
            border: 1px solid var(--hrms-border);
            border-radius: 8px;
            box-shadow: 0 4px 14px rgba(15, 23, 42, 0.05);
            padding: 24px;
            margin-bottom: 18px;
        }

        .sec-head {
            font-size: 18px;
            font-weight: 600;
            color: #1f2937;
            margin-bottom: 20px;
            padding-bottom: 12px;
            border-bottom: 1px dashed #e5e7eb;
        }

        .form-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 16px 18px;
        }

        @media (max-width: 992px) {
            .form-grid {
                grid-template-columns: repeat(1, minmax(0, 1fr));
            }
        }

        .field-block label {
            display: block;
            color: #213855;
            font-size: 13px;
            font-weight: 800;
            margin-bottom: 7px;
        }

        .field-block label.required::after {
            content: " *";
            color: #ef4444;
            font-weight: 800;
        }

        .field-block .form-control,
        .field-block select,
        .field-block input {
            width: 100%;
            border: 1px solid #cfdbea;
            border-radius: 5px;
            min-height: 40px;
            color: #10213f;
            font-size: 13px;
            box-shadow: none;
            padding: 8px 12px;
        }

        .field-block .form-control:focus,
        .field-block select:focus,
        .field-block input:focus {
            border-color: var(--hrms-primary);
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.12);
        }

        .appraisal-actions {
            display: flex;
            justify-content: flex-end;
            gap: 12px;
            margin-top: 24px;
            padding-top: 20px;
            border-top: 1px solid #e5e7eb;
        }

        .appraisal-actions .btn {
            min-width: 120px;
            min-height: 42px;
            font-weight: 700;
            font-size: 13px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="appraisal-form-page">
        <div class="appraisal-breadcrumb">
            <a href="AppraisalDetails.aspx" style="color:#31537f; text-decoration:none;">Appraisal Management</a>
            <i class="fas fa-chevron-right"></i>
            <span><%= hdnIsView.Value == "1" ? "View Appraisal" : (hdnIsEdit.Value == "1" ? "Edit Appraisal" : "Add New Appraisal") %></span>
        </div>

        <div class="appraisal-title-row">
            <div>
                <asp:Label runat="server" ID="lblPageTitle" CssClass="appraisal-form-title" Text="Add New Appraisal"></asp:Label>
                <p class="appraisal-form-subtitle">Enter the appraisal details below</p>
            </div>
            <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary" Text="Back to List" OnClick="btnBack_Click"></asp:Button>
        </div>

        <div class="sec-card">
            <h3 class="sec-head">Appraisal Information</h3>

            <div class="form-grid">
                <div class="field-block">
                    <label for="ddlEmployee">Employee Name</label>
                    <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-control"></asp:DropDownList>
                    <asp:TextBox ID="txtEmployeeName" runat="server" CssClass="form-control" ReadOnly="true" Visible="false"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtEffectiveDate">Effective Date</label>
                    <asp:TextBox ID="txtEffectiveDate" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtCTCOld">Old CTC</label>
                    <asp:TextBox ID="txtCTCOld" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtAppraisalCTC">New CTC</label>
                    <asp:TextBox ID="txtAppraisalCTC" runat="server" CssClass="form-control" placeholder="Enter New CTC"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtGrossSalary">Gross Salary</label>
                    <asp:TextBox ID="txtGrossSalary" runat="server" CssClass="form-control" placeholder="Enter Gross Salary"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtNetSalary">Net Salary</label>
                    <asp:TextBox ID="txtNetSalary" runat="server" CssClass="form-control" placeholder="Enter Net Salary"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtSalaryRevisionDate">Salary Revision Date</label>
                    <asp:TextBox ID="txtSalaryRevisionDate" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtIncrementAmount">Increment Amount</label>
                    <asp:TextBox ID="txtIncrementAmount" runat="server" CssClass="form-control" placeholder="Enter Increment Amount" ></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtIncrementPercentage">Increment Percentage</label>
                    <asp:TextBox ID="txtIncrementPercentage" runat="server" CssClass="form-control" placeholder="Enter Increment Percentage"></asp:TextBox>
                </div>
            </div>

            <div class="appraisal-actions">
                <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-secondary" Text="Cancel" OnClick="btnBack_Click"></asp:Button>
                <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save Appraisal" OnClick="btnSave_Click"></asp:Button>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnAppraisalId" runat="server" Value="0" />
    <asp:HiddenField ID="hdnIsEdit" runat="server" Value="0" />
    <asp:HiddenField ID="hdnIsView" runat="server" Value="0" />
    <asp:HiddenField ID="hdnUserId" runat="server" Value="0" />
</asp:Content>

