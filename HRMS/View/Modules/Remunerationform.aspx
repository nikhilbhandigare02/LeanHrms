<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="Remunerationform.aspx.cs" Inherits="HRMS.View.Modules.Remunerationform" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <link href="https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css" rel="stylesheet" />
    <style>
        .payroll-wrap {
            max-width: 1400px;
            margin: 0 auto;
        }
        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }
        .page-title {
            font-size: 28px;
            font-weight: 700;
            color: #111827;
        }
        .page-subtitle {
            color: #6b7280;
            font-size: 14px;
        }
        .sec-card {
            background: #fff;
            border: 1px solid #e5e7eb;
            border-radius: 12px;
            box-shadow: 0 4px 14px rgba(17,24,39,.05);
            padding: 18px;
            margin-bottom: 18px;
        }
        .sec-head {
            font-size: 18px;
            font-weight: 600;
            color: #1f2937;
            margin-bottom: 15px;
        }
        .form-label {
            font-size: 13px;
            font-weight: 600;
            color: #374151;
            margin-bottom: 5px;
        }
        .form-control {
            border-radius: 8px;
            min-height: 40px;
        }
        .required {
            color: red;
        }
        .components-section {
            display: flex;
            gap: 20px;
            flex-wrap: wrap;
        }
        .components-column {
            flex: 1;
            min-width: 400px;
        }
        .component-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 12px;
        }
        .component-item {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .component-item input[type="checkbox"] {
            width: 18px;
            height: 18px;
        }
        .component-item label {
            flex: 1;
            margin: 0;
            font-size: 13px;
        }
        .component-item .form-control {
            width: 80px;
            flex-shrink: 0;
        }
        .form-row {
            display: flex;
            flex-wrap: wrap;
            margin-right: -10px;
            margin-left: -10px;
        }
        .form-row .col-md-6 {
            padding-right: 10px;
            padding-left: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="payroll-wrap">
                <!-- Header -->
                <div class="page-header">
                    <div>
                        <div class="page-title">
                            <%= (Mode == "view" ? "View Remuneration" : Mode == "edit" ? "Edit Remuneration" : "Add New Remuneration") %>
                        </div>
                        <div class="page-subtitle">
                            <%= (Mode == "view" ? "View remuneration details" : Mode == "edit" ? "Edit remuneration details" : "Add new remuneration record") %>
                        </div>
                    </div>
                    <asp:Button ID="btnBack"
                        runat="server"
                        Text=" Back to List"
                        CssClass="btn btn-secondary"
                        OnClick="btnBack_Click" />
                </div>

                <!-- Basic Details Section -->
                <div class="sec-card">
                    <div class="sec-head">Salary Structure Details</div>
                    
                    <div class="form-row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Employee Name <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Salary Structure ID</label>
                                <asp:TextBox ID="txtSalaryStructureID" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    
                    <div class="form-row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Effective From Date <span class="required">*</span></label>
                                <asp:TextBox ID="txtEffectiveFromDate" runat="server" CssClass="form-control" placeholder="dd-mm-yyyy"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Effective To Date</label>
                                <asp:TextBox ID="txtEffectiveToDate" runat="server" CssClass="form-control" placeholder="dd-mm-yyyy"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Employee Category</label>
                                <asp:DropDownList ID="ddlEmployeeCategory" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Status</label>
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">CTC Amount <span class="required">*</span></label>
                                <asp:TextBox ID="txtCTCAmount" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtCTCAmount_TextChanged"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Gross Salary <span class="required">*</span></label>
                                <asp:TextBox ID="txtGrossSalary" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Monthly Salary <span class="required">*</span></label>
                                <asp:TextBox ID="txtMonthlySalary" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Annual Salary <span class="required">*</span></label>
                                <asp:TextBox ID="txtAnnualSalary" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Components Section -->
                <div class="components-section">
                    <!-- Earnings -->
                    <div class="components-column">
                        <div class="sec-card">
                            <div class="sec-head">Earnings Components</div>
                            <asp:Repeater ID="rptEarnings" runat="server">
                                <HeaderTemplate>
                                    <div class="component-grid">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="component-item">
                                        <asp:HiddenField ID="hfComponentId" runat="server" Value='<%# Eval("Id") %>' />
                                        <asp:HiddenField ID="hfComponentName" runat="server" Value='<%# Eval("Text") %>' />
                                        <asp:CheckBox ID="chkComponent" runat="server" AutoPostBack="true" OnCheckedChanged="chkComponent_CheckedChanged" />
                                        <label><%# Eval("Text") %></label>
                                        <asp:TextBox ID="txtComponentAmount" runat="server" CssClass="form-control" Enabled="false" AutoPostBack="true" OnTextChanged="CalculateGrossSalary"></asp:TextBox>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </div>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <!-- Deductions -->
                    <div class="components-column">
                        <div class="sec-card">
                            <div class="sec-head">Deductions Components</div>
                            <asp:Repeater ID="rptDeductions" runat="server">
                                <HeaderTemplate>
                                    <div class="component-grid">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="component-item">
                                        <asp:HiddenField ID="hfComponentId" runat="server" Value='<%# Eval("Id") %>' />
                                        <asp:HiddenField ID="hfComponentName" runat="server" Value='<%# Eval("Text") %>' />
                                        <asp:CheckBox ID="chkComponent" runat="server" AutoPostBack="true" OnCheckedChanged="chkComponent_CheckedChanged" />
                                        <label><%# Eval("Text") %></label>
                                        <asp:TextBox ID="txtComponentAmount" runat="server" CssClass="form-control" Enabled="false" AutoPostBack="true" OnTextChanged="CalculateGrossSalary"></asp:TextBox>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </div>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>

                <div style="margin-top: 20px; text-align: center;">
                    <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <script type="text/javascript">
        // Initialize date pickers
        function initializeDatePickers() {
            flatpickr("#<%= txtEffectiveFromDate.ClientID %>", {
                dateFormat: "d-m-Y",
                allowInput: true
            });
            
            flatpickr("#<%= txtEffectiveToDate.ClientID %>", {
                dateFormat: "d-m-Y",
                allowInput: true
            });
        }
        
        // Initialize on page load
        $(document).ready(function () {
            initializeDatePickers();
        });
        
        // Re-initialize after UpdatePanel partial postback
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            initializeDatePickers();
        });
    </script>
</asp:Content>
