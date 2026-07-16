
<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="SalaryCalculation.aspx.cs" Inherits="HRMS.View.Modules.SalaryCalculation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .salary-calc-page {
            --sc-primary: #2563EB;
            --sc-primary-dark: #1D4ED8;
            --sc-heading: #1E293B;
            --sc-muted: #64748B;
            --sc-line: #E8EEF7;
            --sc-panel: #F8FAFC;
        }

        .salary-calc-page .sc-card {
            background: #FFFFFF;
            border: 1px solid var(--sc-line);f
            border-radius: 12px;
            box-shadow: 0 10px 30px rgba(15, 23, 42, 0.05);
            padding: 28px 30px;
        }

        .salary-calc-page .sc-title {
            color: var(--sc-heading);
            font-size: 22px;
            font-weight: 800;
            margin-bottom: 22px;
        }

        .salary-calc-page .sc-grid-wrap {
            margin-top: 24px;
            overflow-x: auto;
        }

        .salary-calc-page .sc-grid {
            border-collapse: collapse;
            width: 100%;
        }

        .salary-calc-page .sc-grid th {
            background: #F1F5FB;
            border-bottom: 1px solid var(--sc-line);
            color: #334155;
            font-size: 13px;
            font-weight: 700;
            padding: 12px 14px;
            text-align: left;
        }

        .salary-calc-page .sc-grid td {
            border-bottom: 1px solid #EEF2F7;
            color: #334155;
            font-size: 14px;
            padding: 11px 14px;
        }

        .salary-calc-page .sc-grid tr:hover td {
            background: #F8FBFF;
        }

        .salary-calc-page .ss-grid .num {
            text-align: right;
        }

        .salary-calc-page .action-btn {
            width: 35px;
            height: 35px;
            border-radius: 8px;
            border: none;
            display: inline-flex;
            justify-content: center;
            align-items: center;
            cursor: pointer;
            margin: 0 2px;
        }

        .salary-calc-page .view-btn {
            background: #e0f2fe;
            color: #0284c7;
        }

        .salary-calc-page .edit-btn {
            background: #e6f7ff;
            color: #1890ff;
        }

        .salary-calc-page .status-badge {
            display: inline-flex;
            align-items: center;
            padding: 4px 12px;
            border-radius: 9999px;
            font-size: 12px;
            font-weight: 600;
        }

        .salary-calc-page .status-verified {
            background: #dcfce7;
            color: #166534;
        }

        .salary-calc-page .status-pending {
            background: #fef3c7;
            color: #92400e;
        }

        .salary-calc-page .sc-filters {
            display: flex;
            gap: 16px;
            margin-bottom: 20px;
            flex-wrap: wrap;
            align-items: flex-end;
        }

        .salary-calc-page .filter-group {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .salary-calc-page .filter-group label {
            font-size: 13px;
            font-weight: 600;
            color: #475569;
        }

        .salary-calc-page .filter-input,
        .salary-calc-page .filter-select {
            padding: 8px 12px;
            border: 1px solid #e2e8f0;
            border-radius: 8px;
            font-size: 14px;
            min-width: 200px;
        }

        .salary-calc-page .filter-btn {
            padding: 8px 20px;
            background: #2563EB;
            color: white;
            border: none;
            border-radius: 8px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
        }

        .salary-calc-page .filter-btn:hover {
            background: #1D4ED8;
        }
    </style>
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var txtSearch = document.getElementById('<%= txtSearch.ClientID %>');
            var btnFilter = document.getElementById('<%= btnFilter.ClientID %>');
            var searchTimeout;

            if (txtSearch && btnFilter) {
                txtSearch.addEventListener('input', function () {
                    clearTimeout(searchTimeout);
                    // Wait 300ms after typing stops before triggering filter
                    searchTimeout = setTimeout(function () {
                        btnFilter.click();
                    }, 300);
                });
            }
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="salary-calc-page">
        <div class="row">
            <div class="col-14">
                <div class="sc-card">
                    <div class="sc-title">Salary Calculation Details</div>

                    <div class="sc-filters">
                        <div class="filter-group">
                            <label>Search</label>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="filter-input" placeholder="Search by name or employee code" />
                        </div>
                        <div class="filter-group">
                            <label>Status</label>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="filter-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                <asp:ListItem Value="">All</asp:ListItem>
                                <asp:ListItem Value="Pending">Pending</asp:ListItem>
                                <asp:ListItem Value="Verified">Verified</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="filter-btn" OnClick="btnFilter_Click" Style="display: none;" />
                        <asp:Button ID="btnExport" runat="server" Text="Export to Excel" CssClass="filter-btn" OnClick="btnExport_Click" Style="background: #10b981;" />
                    </div>

                    <div class="sc-grid-wrap">
                        <asp:GridView ID="gvSalaryCalculations" runat="server" AutoGenerateColumns="false" CssClass="sc-grid"
                            GridLines="None" Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="SR No">
                                    <ItemTemplate>
                                        <%# Container.DataItemIndex + 1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="employee_code" HeaderText="Employee Code" />
                                <asp:BoundField DataField="user_fullname" HeaderText="Name" />
                                <asp:TemplateField HeaderText="Monthly Salary" ItemStyle-CssClass="num" HeaderStyle-CssClass="num">
                                    <ItemTemplate><%# FormatMoney(Eval("monthly_salary")) %></ItemTemplate>
                                </asp:TemplateField>
                                <%--<asp:TemplateField HeaderText="Total Working Days" ItemStyle-CssClass="num" HeaderStyle-CssClass="num">
                                    <ItemTemplate><%# Eval("total_working_days") %></ItemTemplate>
                                </asp:TemplateField>--%>
                                <%--<asp:TemplateField HeaderText="Present Days" ItemStyle-CssClass="num" HeaderStyle-CssClass="num">
                                    <ItemTemplate><%# Eval("present_days") %></ItemTemplate>
                                </asp:TemplateField>--%>
                                <%--<asp:TemplateField HeaderText="Absent Days" ItemStyle-CssClass="num" HeaderStyle-CssClass="num">
                                    <ItemTemplate><%# Eval("absent_days") %></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Leave Days" ItemStyle-CssClass="num" HeaderStyle-CssClass="num">
                                    <ItemTemplate><%# Eval("leave_deduction_days") %></ItemTemplate>
                                </asp:TemplateField>--%>
                                <asp:TemplateField HeaderText="Deducted Amount" ItemStyle-CssClass="num" HeaderStyle-CssClass="num">
                                    <ItemTemplate><%# FormatMoney(Eval("deducted_amount")) %></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="NET Salary" ItemStyle-CssClass="num" HeaderStyle-CssClass="num">
            <ItemTemplate><%# FormatMoney(Eval("deducted_monthly_salary")) %></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Verification Status">
            <ItemTemplate>
                <span class='status-badge <%# Eval("verification_status").ToString().ToLower() == "verified" ? "status-verified" : "status-pending" %>'>
                    <%# Eval("verification_status") %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnView"
                                                runat="server"
                                                CssClass="action-btn view-btn"
                                                ToolTip="View"
                                                CommandArgument='<%# Eval("user_id") %>'
                                                OnClick="btnView_Click">
                                                <i class='bx bx-show'></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="btnEdit"
                                                runat="server"
                                                CssClass="action-btn edit-btn"
                                                ToolTip="Edit"
                                                CommandArgument='<%# Eval("user_id") %>'
                                                OnClick="btnEdit_Click"
                                                Visible='<%# Eval("verification_status").ToString() == "Pending" %>'>
                                                <i class='bx bx-edit'></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
