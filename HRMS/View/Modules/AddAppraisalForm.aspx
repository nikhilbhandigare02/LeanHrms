<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AddAppraisalForm.aspx.cs" Inherits="HRMS.View.Modules.AddAppraisalForm" %>

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
                    <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlEmployee_SelectedIndexChanged"></asp:DropDownList>
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
                    <asp:TextBox ID="txtAppraisalCTC" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Auto-calculated from components"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtGrossSalary">Gross Salary</label>
                    <asp:TextBox ID="txtGrossSalary" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Auto-calculated from components"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtNetSalary">Net Salary</label>
                    <asp:TextBox ID="txtNetSalary" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Auto-calculated from components"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtSalaryRevisionDate">Salary Revision Date</label>
                    <asp:TextBox ID="txtSalaryRevisionDate" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtIncrementAmount">Increment Amount</label>
                    <asp:TextBox ID="txtIncrementAmount" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Auto-calculated"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtIncrementPercentage">Increment Percentage</label>
                    <asp:TextBox ID="txtIncrementPercentage" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Auto-calculated"></asp:TextBox>
                </div>

                <div class="field-block">
                    <label for="txtIncrementAmountMonthly">Increment Amount Monthly</label>
                    <asp:TextBox ID="txtIncrementAmountMonthly" runat="server" CssClass="form-control" ReadOnly="true" placeholder="Auto-calculated"></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="sec-card">
            <h3 class="sec-head">Salary Components <span style="font-weight:400; font-size:12px; color:var(--hrms-muted);">(auto-scaled from the employee's current active remuneration by the CTC increment ratio &mdash; adjust before saving if needed)</span></h3>

            <div style="display:flex; gap:24px; flex-wrap:wrap;">
                <div style="flex:1; min-width:320px;">
                    <h4 style="font-size:14px; font-weight:700; margin-bottom:10px; color:#213855;">Earnings</h4>
                    <div id="appraisalEarningsContainer" style="display:grid; grid-template-columns:1fr 1fr; gap:10px;">
                        <asp:Repeater ID="rptAppraisalEarnings" runat="server" OnItemDataBound="rptAppraisalEarnings_ItemDataBound">
                            <ItemTemplate>
                                <div style="display:flex; align-items:center; gap:8px;">
                                    <asp:HiddenField ID="hfComponentId" runat="server" Value='<%# Eval("Id") %>' />
                                    <asp:HiddenField ID="hfComponentName" runat="server" Value='<%# Eval("Text") %>' />
                                    <asp:CheckBox ID="chkComponent" runat="server" onclick="toggleAppraisalComponent(this)" />
                                    <label style="flex:1; font-size:13px; margin:0;"><%# Eval("Text") %></label>
                                    <asp:TextBox ID="txtComponentAmount" runat="server" CssClass="form-control appraisal-amount-input" Style="width:110px; min-height:34px;" onkeypress="return isNumberKey(event, true)" onpaste="return validateNumberPaste(event, true)" oninput="limitDecimalPlaces(this, 2); recalculateAppraisalTotals();" />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                <div style="flex:1; min-width:320px;">
                    <h4 style="font-size:14px; font-weight:700; margin-bottom:10px; color:#213855;">Deductions</h4>
                    <div id="appraisalDeductionsContainer" style="display:grid; grid-template-columns:1fr 1fr; gap:10px;">
                        <asp:Repeater ID="rptAppraisalDeductions" runat="server" OnItemDataBound="rptAppraisalDeductions_ItemDataBound">
                            <ItemTemplate>
                                <div style="display:flex; align-items:center; gap:8px;">
                                    <asp:HiddenField ID="hfComponentId" runat="server" Value='<%# Eval("Id") %>' />
                                    <asp:HiddenField ID="hfComponentName" runat="server" Value='<%# Eval("Text") %>' />
                                    <asp:CheckBox ID="chkComponent" runat="server" onclick="toggleAppraisalComponent(this)" />
                                    <label style="flex:1; font-size:13px; margin:0;"><%# Eval("Text") %></label>
                                    <asp:TextBox ID="txtComponentAmount" runat="server" CssClass="form-control appraisal-amount-input" Style="width:110px; min-height:34px;" onkeypress="return isNumberKey(event, true)" onpaste="return validateNumberPaste(event, true)" oninput="limitDecimalPlaces(this, 2); recalculateAppraisalTotals();" />
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
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
    <asp:HiddenField ID="hdnActiveRemunerationId" runat="server" Value="0" />
    <asp:HiddenField ID="hdnEmployeeCategory" runat="server" Value="" />
    <asp:HiddenField ID="hdnRemunerationStatus" runat="server" Value="" />

    <script>
        // Initialize Flatpickr date pickers
        function initializeDatePickers() {
            flatpickr('#<%= txtEffectiveDate.ClientID %>', {
                dateFormat: 'd-m-Y',
                allowInput: true
            });

            flatpickr('#<%= txtSalaryRevisionDate.ClientID %>', {
                dateFormat: 'd-m-Y',
                allowInput: true
            });
        }

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

        // Initialize on page load. Gross/Net/New CTC only get auto-recomputed
        // in fresh Add mode, where components were just prefilled from the
        // employee's active remuneration. In Edit/View mode these fields must
        // keep showing the actually-saved historical appraisal values instead
        // - recomputing from the (possibly unrelated, since it reflects
        // whatever is CURRENTLY active) remuneration components would silently
        // clobber the correct saved figures for anything but the very latest appraisal.
        document.addEventListener('DOMContentLoaded', function () {
            initializeDatePickers();
            initializeAppraisalComponentStates();

            var isEdit = document.getElementById('<%= hdnIsEdit.ClientID %>').value === '1';
            var isView = document.getElementById('<%= hdnIsView.ClientID %>').value === '1';
            if (!isEdit && !isView) {
                updateGrossNetCtc();
            }
        });

        // The disabled/enabled state of each amount input is purely a client-side
        // concern (server-side Enabled=false would cause ASP.NET to silently
        // discard posted values even after JS re-enables the field). Derive it
        // fresh from each row's checkbox on every load.
        function initializeAppraisalComponentStates() {
            document.querySelectorAll('.appraisal-amount-input').forEach(function (input) {
                var row = input.parentElement;
                var checkbox = row ? row.querySelector('input[type="checkbox"]') : null;
                input.disabled = !(checkbox && checkbox.checked);
            });
        }

        function toggleAppraisalComponent(checkbox) {
            var row = checkbox.parentElement;
            var input = row.querySelector('.appraisal-amount-input');
            if (input) {
                input.disabled = !checkbox.checked;
                if (!checkbox.checked) {
                    input.value = '';
                } else if (!input.value) {
                    var original = parseFloat(input.getAttribute('data-original-amount')) || 0;
                    if (original > 0) {
                        input.value = original.toFixed(2);
                    }
                }
            }
            recalculateAppraisalTotals();
        }

        function sumEnabledAmounts(containerId) {
            var total = 0;
            document.querySelectorAll('#' + containerId + ' .appraisal-amount-input').forEach(function (input) {
                if (!input.disabled) {
                    total += parseFloat(input.value) || 0;
                }
            });
            return total;
        }

        function recalculateAppraisalTotals() {
            var newCtc = updateGrossNetCtc();
            updateIncrementFields(newCtc);
        }

        // Gross/Net/New CTC reflect whatever is currently checked, always.
        function updateGrossNetCtc() {
            var totalEarnings = sumEnabledAmounts('appraisalEarningsContainer');
            var totalDeductions = sumEnabledAmounts('appraisalDeductionsContainer');

            document.getElementById('<%= txtGrossSalary.ClientID %>').value = totalEarnings.toFixed(2);
            document.getElementById('<%= txtNetSalary.ClientID %>').value = (totalEarnings - totalDeductions).toFixed(2);

            var newCtc = totalEarnings * 12;
            document.getElementById('<%= txtAppraisalCTC.ClientID %>').value = newCtc.toFixed(2);
            return newCtc;
        }

        // Increment fields are only meant to reflect an actual, deliberate
        // change the user made - not fire the moment an employee is selected
        // and their existing components get prefilled.
        function updateIncrementFields(newCtc) {
            var oldCtc = parseFloat(document.getElementById('<%= txtCTCOld.ClientID %>').value) || 0;
            var incrementAmount = newCtc - oldCtc;

            document.getElementById('<%= txtIncrementAmount.ClientID %>').value = incrementAmount.toFixed(2);
            document.getElementById('<%= txtIncrementPercentage.ClientID %>').value =
                oldCtc > 0 ? ((incrementAmount / oldCtc) * 100).toFixed(2) : '0.00';
            document.getElementById('<%= txtIncrementAmountMonthly.ClientID %>').value = (incrementAmount / 12).toFixed(2);
        }
    </script>
</asp:Content>

