<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AddReimbursementForm.aspx.cs" Inherits="HRMS.View.Modules.AddReimbursementForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css" rel="stylesheet" />
    <!-- Flatpickr CSS and JS for date pickers -->
    <link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
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

        .reimbursement-form-page {
            max-width: 1380px;
            margin: 0 auto;
            padding: 8px 18px 28px;
            color: var(--hrms-text);
        }

        .reimbursement-breadcrumb {
            display: flex;
            align-items: center;
            gap: 10px;
            color: #31537f;
            font-size: 12px;
            font-weight: 700;
            margin-bottom: 12px;
        }

        .reimbursement-title-row {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 16px;
            margin-bottom: 14px;
        }

        .reimbursement-form-title {
            display: block;
            font-size: 22px;
            line-height: 1.2;
            font-weight: 800;
            color: #071733;
            margin-bottom: 4px;
        }

        .reimbursement-form-subtitle {
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

        .field-block .form-control {
            width: 100%;
            padding: 10px 14px;
            border: 1px solid var(--hrms-border);
            border-radius: 6px;
            font-size: 13px;
            color: var(--hrms-text);
            background: #fff;
            transition: all 0.2s;
        }

        .field-block .form-control:focus {
            outline: none;
            border-color: var(--hrms-primary);
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
        }

        .field-block .form-control[readonly] {
            background: #f8fafc;
            color: #64748b;
            cursor: not-allowed;
        }

        .action-buttons {
            display: flex;
            gap: 12px;
            margin-top: 24px;
            flex-wrap: wrap;
        }

        .btn {
            padding: 10px 24px;
            border-radius: 6px;
            font-size: 13px;
            font-weight: 700;
            border: none;
            cursor: pointer;
            transition: all 0.2s;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .btn-primary {
            background: var(--hrms-primary);
            color: #fff;
        }

        .btn-primary:hover {
            background: #1d4ed8;
        }

        .btn-success {
            background: var(--hrms-success);
            color: #fff;
        }

        .btn-success:hover {
            background: #15803d;
        }

        .btn-danger {
            background: #dc2626;
            color: #fff;
        }

        .btn-danger:hover {
            background: #b91c1c;
        }

        .btn-secondary {
            background: #64748b;
            color: #fff;
        }

        .btn-secondary:hover {
            background: #475569;
        }

        .status-section {
            margin-top: 20px;
            padding: 16px;
            background: #f8fafc;
            border-radius: 6px;
            border: 1px solid #e2e8f0;
        }

        .status-section h4 {
            font-size: 14px;
            font-weight: 600;
            color: #1f2937;
            margin: 0 0 12px 0;
        }

        .status-buttons {
            display: flex;
            gap: 12px;
        }

        .hidden {
            display: none;
        }

        .hidden-field {
            display: none;
        }

        .reimbursement-documents-table {
            width: 100%;
            border-collapse: collapse;
        }

            .reimbursement-documents-table th {
                background: #f8fafc;
                padding: 10px 12px;
                font-size: 13px;
                font-weight: 600;
                color: #374151;
                border-bottom: 1px solid var(--hrms-border);
                text-align: left;
            }

            .reimbursement-documents-table td {
                padding: 10px 12px;
                font-size: 13px;
                border-bottom: 1px solid #edf2f7;
            }

        .btn-link-action {
            color: var(--hrms-primary);
            text-decoration: none;
            font-weight: 600;
            font-size: 13px;
        }

            .btn-link-action:hover {
                text-decoration: underline;
            }

        .hrms-muted-text {
            color: var(--hrms-muted);
            font-size: 13px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="reimbursement-form-page">
        <!-- Breadcrumb -->
        <div class="reimbursement-breadcrumb">
            <i class='bx bx-home-alt'></i>
            <span>Home</span>
            <i class='bx bx-chevron-right'></i>
            <span>Reimbursement</span>
            <i class='bx bx-chevron-right'></i>
            <span id="breadcrumbCurrent">Reimbursement Form</span>
        </div>

        <!-- Title Row -->
        <div class="reimbursement-title-row">
            <div>
                <asp:Label ID="lblPageTitle" runat="server" CssClass="reimbursement-form-title" Text="Add New Reimbursement"></asp:Label>
                <p class="reimbursement-form-subtitle">Fill in the reimbursement details below</p>
            </div>
            <asp:Button ID="btnBack" runat="server" Text="Back to List" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
        </div>

        <!-- Hidden Fields -->
        <asp:HiddenField ID="hdnReimbursementId" runat="server" Value="0" />
        <asp:HiddenField ID="hdnIsEdit" runat="server" Value="0" />
        <asp:HiddenField ID="hdnIsView" runat="server" Value="0" />

        <!-- Reimbursement Details Card -->
        <div class="sec-card">
            <div class="sec-head">Reimbursement Details</div>
            
            <div class="form-grid">
                <div class="field-block" id="fieldReimbursementNumber">
                    <label>Reimbursement Number</label>
                    <asp:TextBox ID="txtReimbursementNumber" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>

                <div class="field-block" id="fieldClaimType">
                    <label>Claim Type</label>
                    <asp:TextBox ID="txtClaimType" runat="server" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="field-block" id="fieldClaimDate">
                    <label>Claim Date</label>
                    <asp:TextBox ID="txtClaimDate" runat="server" CssClass="form-control datepicker"></asp:TextBox>
                </div>

                <div class="field-block" id="fieldPaymentMonth" aria-disabled="true">
                    <%--<label>Payment Month</label>--%>
                    <asp:TextBox ID="txtPaymentMonth" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                </div>

                <div class="field-block" id="fieldClaimAmount">
                    <label>Claim Amount</label>
                    <asp:TextBox ID="txtClaimAmount" runat="server" CssClass="form-control"></asp:TextBox>
                </div>

                <div class="field-block" id="fieldDocument" style="grid-column: span 2;">
                    <%--<label>Document</label>--%>
                    <div style="display: flex; gap: 10px;">
                        <asp:TextBox ID="txtDocument" runat="server" CssClass="form-control" style="flex: 1;" Visible="false"></asp:TextBox>
                        <asp:HiddenField ID="hdnDocumentData" runat="server" />
                        <asp:Button ID="btnDownloadDocument" runat="server" Text="Download" CssClass="btn btn-primary" OnClick="btnDownloadDocument_Click" visible="false"/>
                    </div>
                </div>

                <div class="field-block" id="fieldReimbursementDocuments" style="grid-column: span 2;">
                    <label>Uploaded Documents</label>
                    <asp:Repeater ID="rptReimbursementDocuments" runat="server" OnItemCommand="rptReimbursementDocuments_ItemCommand">
                        <HeaderTemplate>
                            <table class="reimbursement-documents-table">
                                <thead><tr><th>File</th><th>Uploaded On</th><th>Action</th></tr></thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("FileName") %><%# Eval("FileExtension") %></td>
                                <td><%# Eval("InsertedDate", "{0:dd-MMM-yyyy}") %></td>
                                <td>
                                    <asp:LinkButton runat="server" CommandName="DownloadReimbursementDocument" CommandArgument='<%# Eval("UserDocDetId") %>' CssClass="btn-link-action" CausesValidation="false">
                                        <i class="far fa-eye"></i> View / Download
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblNoReimbursementDocuments" runat="server" Text="No documents uploaded." CssClass="hrms-muted-text" Visible="false" />
                </div>
            </div>

            <!-- Approval/Review Section -->
            <div id="approvalSection" class="status-section" runat="server">
                <div class="sec-head" style="margin-top: 20px;">Approval & Review</div>
                <div class="form-grid">
                    <div class="field-block" id="fieldMonth">
                        <label>Month</label>
                        <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control" style="background-color: #fff;"></asp:DropDownList>
                    </div>

                    <div class="field-block" id="fieldStatus">
                        <label>Status</label>
                        <asp:TextBox ID="txtStatus" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control hidden-field"></asp:DropDownList>
                    </div>

                    <div class="field-block" id="fieldRemarks" style="grid-column: span 2;">
                        <label>Remarks</label>
                        <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Add any comments or notes here..."></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- Action Buttons -->
            <div class="action-buttons">
                <asp:Button ID="btnSave" runat="server" Text="Save Reimbursement" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" />
            </div>
        </div>
    </div>

    <script>
        // Initialize date picker
        document.addEventListener('DOMContentLoaded', function() {
            flatpickr('.datepicker', {
                dateFormat: 'd-M-Y',
                allowInput: true
            });

            setFormMode();
        });

        function setFormMode() {
            var isView = document.getElementById('<%= hdnIsView.ClientID %>').value;
            var isEdit = document.getElementById('<%= hdnIsEdit.ClientID %>').value;

            if (isView === '1') {
                // View mode: hide approval section, make all fields read-only
                document.getElementById('<%= approvalSection.ClientID %>').classList.add('hidden-field');
                
                var inputs = document.querySelectorAll('.form-control');
                inputs.forEach(function(input) {
                    input.setAttribute('readonly', 'true');
                });
                
                document.getElementById('<%= btnSave.ClientID %>').style.display = 'none';
            } 
            else if (isEdit === '1') {
                // Edit mode: make only remarks and status dropdown editable
                var editableInputs = ['<%= txtRemarks.ClientID %>', '<%= ddlStatus.ClientID %>'];
                
                var inputs = document.querySelectorAll('.form-control');
                inputs.forEach(function(input) {
                    if (!editableInputs.includes(input.id)) {
                        input.setAttribute('readonly', 'true');
                    }
                });

                // Show dropdown instead of textbox for status
                document.getElementById('<%= txtStatus.ClientID %>').style.display = 'none';
                document.getElementById('<%= ddlStatus.ClientID %>').classList.remove('hidden-field');
                
                // Set dropdown value based on current status
                var currentStatus = document.getElementById('<%= txtStatus.ClientID %>').value;
                document.getElementById('<%= ddlStatus.ClientID %>').value = currentStatus;
            }
        }
    </script>
</asp:Content>
