<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master"
    AutoEventWireup="true"
    CodeBehind="AppraisalHistory.aspx.cs"
    Inherits="HRMS.View.Modules.AppraisalHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>

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

        .filter-row {
            display: flex;
            flex-wrap: wrap;
            gap: 16px;
            align-items: flex-end;
            margin-bottom: 18px;
        }

        .filter-row .filter-item {
            min-width: 200px;
        }

        .appraisal-grid {
            width: 100%;
            border-collapse: collapse;
        }

            .appraisal-grid th {
                background: #f8fafc;
                padding: 12px;
                font-size: 13px;
                font-weight: 600;
                color: #374151;
                border-bottom: 1px solid #e5e7eb;
                text-align: left;
            }

            .appraisal-grid td {
                padding: 12px;
                font-size: 13px;
                border-bottom: 1px solid #edf2f7;
            }

        .status-pill {
            display: inline-block;
            padding: 3px 10px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
        }

        .status-pill.active {
            background: #dcfce7;
            color: #16a34a;
        }

        .status-pill.superseded,
        .status-pill.inactive {
            background: #f1f5f9;
            color: #64748b;
        }

        .grid-toolbar {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 15px;
            flex-wrap: wrap;
            gap: 10px;
        }

        .total-records {
            font-size: 13px;
            font-weight: 600;
            color: #374151;
        }

        .pager-wrap {
            margin-top: 15px;
        }

            .pager-wrap table {
                margin: 0 auto;
            }

            .pager-wrap span {
                padding: 6px 12px;
                background: #2563eb;
                color: #fff;
                border-radius: 6px;
                font-size: 13px;
                font-weight: 600;
            }

            .pager-wrap a {
                padding: 6px 12px;
                color: #374151;
                font-size: 13px;
                text-decoration: none;
            }

                .pager-wrap a:hover {
                    color: #2563eb;
                }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="payroll-wrap">

        <div class="page-header">
            <div>
                <div class="page-title">Appraisal History</div>
                <div class="page-subtitle">Full increment trail per employee, filterable by employee and date</div>
            </div>

            <asp:Button ID="btnBack"
                runat="server"
                Text="Back to Appraisal Management"
                CssClass="btn btn-secondary"
                OnClick="btnBack_Click" />
        </div>

        <div class="sec-card">
            <div class="filter-row">
                <div class="filter-item">
                    <label class="form-label">Employee</label>
                    <asp:DropDownList ID="ddlEmployeeFilter" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>

                <div class="filter-item">
                    <label class="form-label">Effective Date From</label>
                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy" autocomplete="off"></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">Effective Date To</label>
                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy" autocomplete="off"></asp:TextBox>
                </div>

                <div class="filter-item">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-outline-secondary" OnClick="btnClear_Click" CausesValidation="false" />
                </div>
            </div>
        </div>

        <div class="sec-card">
            <div class="grid-toolbar">
                <div class="sec-head" style="margin-bottom: 0;">Appraisal History</div>
                <div style="display: flex; align-items: center; gap: 16px;">
                    <span class="total-records">
                        Total Records: <asp:Literal ID="litTotalRecords" runat="server" Text="0"></asp:Literal>
                    </span>
                    <asp:LinkButton ID="btnExport" runat="server" CssClass="btn btn-success" OnClick="btnExport_Click">
                        <i class="fa fa-file-excel-o"></i> Export to Excel
                    </asp:LinkButton>
                </div>
            </div>

            <asp:GridView ID="gvAppraisalHistory"
                runat="server"
                CssClass="table appraisal-grid"
                AutoGenerateColumns="False"
                GridLines="None"
                DataKeyNames="appraisal_id"
                AllowPaging="True"
                PageSize="10"
                OnPageIndexChanging="gvAppraisalHistory_PageIndexChanging"
                EmptyDataText="No appraisal history found for the selected filters.">

                <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" PreviousPageText="Prev" NextPageText="Next" />
                <PagerStyle CssClass="pager-wrap" HorizontalAlign="Center" />

                <Columns>
                    <asp:BoundField DataField="emp_code" HeaderText="Employee Code" />

                    <asp:BoundField DataField="employee_name" HeaderText="Employee Name" />

                    <asp:BoundField DataField="appraisal_effective_date"
                        HeaderText="Effective Date"
                        DataFormatString="{0:dd-MMM-yyyy}" />

                    <asp:BoundField DataField="appraisal_ctc" HeaderText="CTC" DataFormatString="{0:N2}" />

                    <asp:BoundField DataField="gross_salary" HeaderText="Gross Salary" DataFormatString="{0:N2}" />

                    <asp:BoundField DataField="net_salary" HeaderText="Net Salary" DataFormatString="{0:N2}" />

                    <asp:BoundField DataField="salary_revision_date"
                        HeaderText="Revision Date"
                        DataFormatString="{0:dd-MMM-yyyy}" />

                    <asp:BoundField DataField="increament_percentage" HeaderText="Percentage %" DataFormatString="{0:N2}" />

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='status-pill <%# (bool)Eval("is_active") ? "active" : "inactive" %>'>
                                <%# (bool)Eval("is_active") ? "Active" : "Inactive" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="inserted_date_display" HeaderText="Saved On" />
                </Columns>

            </asp:GridView>
        </div>
    </div>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            flatpickr('#<%= txtFromDate.ClientID %>', { dateFormat: 'd-m-Y', allowInput: true });
            flatpickr('#<%= txtToDate.ClientID %>', { dateFormat: 'd-m-Y', allowInput: true });
        });
    </script>
</asp:Content>
