<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="Remunerationform.aspx.cs" Inherits="HRMS.View.Modules.Remunerationform" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
                                <%--<label class="form-label">Salary Structure ID</label>--%>
                                <asp:TextBox ID="txtSalaryStructureID" runat="server" CssClass="form-control" ReadOnly="true" Visible="false" ></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    
                    <div class="form-row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Effective From Date <span class="required">*</span></label>
                                <asp:TextBox ID="txtEffectiveFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                        </div>
                     <%--   <div class="col-md-6">
                            <div class="form-group">
                               <label class="form-label" aria-disabled="true" >Effective To Date </label>
                                <asp:TextBox ID="txtEffectiveToDate" runat="server" CssClass="form-control" placeholder="dd-mm-yyyy"  Visible="false" ></asp:TextBox>
                            </div>
                        </div>--%>
               

                   
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Employee Category</label>
                                <asp:DropDownList ID="ddlEmployeeCategory" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                             </div>
                     <div class="form-row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Status</label>
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Gross Salary <span class="required">*</span></label>
                                <asp:TextBox ID="txtGrossSalary" runat="server" ReadOnly="true" CssClass="form-control numeric-input"></asp:TextBox>
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
                                <label class="form-label">CTC <span class="required">*</span></label>
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
                            <div id="remunerationEarningsContainer">
                            <asp:Repeater ID="rptEarnings" runat="server">
                                <HeaderTemplate>
                                    <div class="component-grid">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="component-item">
                                        <asp:HiddenField ID="hfComponentId" runat="server" Value='<%# Eval("Id") %>' />
                                        <asp:HiddenField ID="hfComponentName" runat="server" Value='<%# Eval("Text") %>' />
                                        <asp:CheckBox ID="chkComponent" runat="server" onclick="toggleRemunerationComponent(this)" />
                                        <label><%# Eval("Text") %></label>
                                        <asp:TextBox ID="txtComponentAmount" runat="server" CssClass="form-control numeric-input" onkeypress="return isNumberKey(event, true)" onpaste="return validateNumberPaste(event, true)" oninput="limitDecimalPlaces(this, 2); recalculateRemunerationTotals();"></asp:TextBox>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </div>
                                </FooterTemplate>
                            </asp:Repeater>
                            </div>
                        </div>
                    </div>

                    <!-- Deductions -->
                    <div class="components-column">
                        <div class="sec-card">
                            <div class="sec-head">Deductions Components</div>
                            <div id="remunerationDeductionsContainer">
                            <asp:Repeater ID="rptDeductions" runat="server">
                                <HeaderTemplate>
                                    <div class="component-grid">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="component-item">
                                        <asp:HiddenField ID="hfComponentId" runat="server" Value='<%# Eval("Id") %>' />
                                        <asp:HiddenField ID="hfComponentName" runat="server" Value='<%# Eval("Text") %>' />
                                        <asp:CheckBox ID="chkComponent" runat="server" onclick="toggleRemunerationComponent(this)" />
                                        <label><%# Eval("Text") %></label>
                                        <asp:TextBox ID="txtComponentAmount" runat="server" CssClass="form-control numeric-input" onkeypress="return isNumberKey(event, true)" onpaste="return validateNumberPaste(event, true)" oninput="limitDecimalPlaces(this, 2); recalculateRemunerationTotals();"></asp:TextBox>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </div>
                                </FooterTemplate>
                            </asp:Repeater>
                            </div>
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

        // Allow only numeric keys (digits, backspace, delete, tab, enter, decimal point)
        function isNumberKey(evt, allowDecimal) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                if (allowDecimal && charCode == 46) {
                    var input = evt.target || evt.srcElement;
                    if (input.value.indexOf('.') !== -1) {
                        return false;
                    }
                    return true;
                }
                return false;
            }

            // Check decimal places limit when typing
            if (allowDecimal) {
                var input = evt.target || evt.srcElement;
                var value = input.value;
                var decimalIndex = value.indexOf('.');

                if (decimalIndex !== -1) {
                    var cursorPosition = input.selectionStart;
                    var decimalPlaces = value.length - decimalIndex - 1;

                    // If cursor is after decimal and already 2 decimal places, disallow
                    if (cursorPosition > decimalIndex && decimalPlaces >= 2) {
                        return false;
                    }
                }
            }

            return true;
        }

        // Validate pasted content
        function validateNumberPaste(evt, allowDecimal, decimalLimit) {
            if (!decimalLimit) decimalLimit = 2;
            var clipboardData, pastedData;
            evt.stopPropagation();
            evt.preventDefault();
            clipboardData = evt.clipboardData || window.clipboardData;
            pastedData = clipboardData.getData('Text');

            if (allowDecimal) {
                // Allow numbers with up to specified decimal places
                var regex = new RegExp('^\\d*\\.?\\d{0,' + decimalLimit + '}$');
                if (!regex.test(pastedData)) {
                    return false;
                }
            } else {
                if (!/^\d*$/.test(pastedData)) {
                    return false;
                }
            }

            var input = evt.target || evt.srcElement;
            input.value = pastedData;
            return false;
        }

        // Limit decimal places on input
        function limitDecimalPlaces(input, limit) {
            var value = input.value;
            var decimalIndex = value.indexOf('.');

            if (decimalIndex !== -1) {
                var integerPart = value.substring(0, decimalIndex);
                var decimalPart = value.substring(decimalIndex + 1, decimalIndex + 1 + limit);
                input.value = integerPart + '.' + decimalPart;
            }
        }

        // Component checking/typing is handled entirely client-side (no more
        // AutoPostBack per checkbox/amount field) so editing amounts no
        // longer triggers a full page/UpdatePanel re-render on every change.
        // The actual values are simply read from the DOM by the server at Save.
        document.addEventListener('DOMContentLoaded', function () {
            initializeRemunerationComponentStates();
            recalculateRemunerationTotals();
        });

        function initializeRemunerationComponentStates() {
            document.querySelectorAll('#remunerationEarningsContainer .numeric-input, #remunerationDeductionsContainer .numeric-input').forEach(function (input) {
                var row = input.closest('.component-item');
                var checkbox = row ? row.querySelector('input[type="checkbox"]') : null;
                input.disabled = !(checkbox && checkbox.checked);
            });
        }

        function toggleRemunerationComponent(checkbox) {
            var row = checkbox.closest('.component-item');
            var input = row ? row.querySelector('.numeric-input') : null;
            if (input) {
                input.disabled = !checkbox.checked;
                if (!checkbox.checked) {
                    input.value = '';
                }
            }
            recalculateRemunerationTotals();
        }

        function sumEnabledRemunerationAmounts(containerId, excludeNames) {
            var total = 0;
            document.querySelectorAll('#' + containerId + ' .component-item').forEach(function (row) {
                var input = row.querySelector('.numeric-input');
                if (!input || input.disabled) {
                    return;
                }
                if (excludeNames) {
                    var label = row.querySelector('label');
                    var name = label ? label.textContent.toLowerCase() : '';
                    if (excludeNames.some(function (n) { return name.indexOf(n) !== -1; })) {
                        return;
                    }
                }
                total += parseFloat(input.value) || 0;
            });
            return total;
        }

        function recalculateRemunerationTotals() {
            var totalEarnings = sumEnabledRemunerationAmounts('remunerationEarningsContainer');
            // "Exceed paid leave"-style deductions are excluded from the net
            // monthly figure, matching the server's original CalculateGrossSalary logic.
            var totalDeductions = sumEnabledRemunerationAmounts('remunerationDeductionsContainer', ['exceed', 'paid leave']);

            document.getElementById('<%= txtGrossSalary.ClientID %>').value = totalEarnings.toFixed(2);
            document.getElementById('<%= txtMonthlySalary.ClientID %>').value = (totalEarnings - totalDeductions).toFixed(2);
            document.getElementById('<%= txtAnnualSalary.ClientID %>').value = (totalEarnings * 12).toFixed(2);
        }
    </script>
</asp:Content>