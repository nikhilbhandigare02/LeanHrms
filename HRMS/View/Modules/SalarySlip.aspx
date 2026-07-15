<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="SalarySlip.aspx.cs" Inherits="HRMS.View.Modules.SalarySlip" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <style>
        .salary-slip-page {
            --ss-primary: #2563EB;
            --ss-primary-dark: #1D4ED8;
            --ss-heading: #1E293B;
            --ss-muted: #64748B;
            --ss-line: #E8EEF7;
            --ss-panel: #F8FAFC;
        }

        .salary-slip-page .ss-card {
            background: #FFFFFF;
            border: 1px solid var(--ss-line);
            border-radius: 12px;
            box-shadow: 0 10px 30px rgba(15, 23, 42, 0.05);
            padding: 28px 30px;
        }

        .salary-slip-page .ss-title {
            color: var(--ss-heading);
            font-size: 22px;
            font-weight: 800;
            margin-bottom: 22px;
        }

        .salary-slip-page .ss-filter {
            background: var(--ss-panel);
            border: 1px solid var(--ss-line);
            border-radius: 10px;
            padding: 20px;
        }

        .salary-slip-page .ss-filter-label {
            color: var(--ss-heading);
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .salary-slip-page .ss-filter .form-select {
            border: 1px solid #D9E2EF;
            border-radius: 8px;
            height: 42px;
        }

        .salary-slip-page .ss-search-btn {
            background: var(--ss-primary);
            border: none;
            border-radius: 8px;
            color: #FFFFFF;
            font-weight: 600;
            height: 42px;
            min-width: 120px;
            transition: background-color .18s ease, box-shadow .18s ease, transform .18s ease;
        }

        .salary-slip-page .ss-search-btn:hover {
            background: var(--ss-primary-dark);
            box-shadow: 0 10px 20px rgba(37, 99, 235, 0.22);
            transform: translateY(-1px);
        }

        /* Grid */
        .salary-slip-page .ss-grid-wrap { margin-top: 24px; overflow-x: auto; }

        .salary-slip-page .ss-grid {
            border-collapse: collapse;
            width: 100%;
        }

        .salary-slip-page .ss-grid th {
            background: #F1F5FB;
            border-bottom: 1px solid var(--ss-line);
            color: #334155;
            font-size: 13px;
            font-weight: 700;
            padding: 12px 14px;
            text-align: left;
        }

        .salary-slip-page .ss-grid td {
            border-bottom: 1px solid #EEF2F7;
            color: #334155;
            font-size: 14px;
            padding: 11px 14px;
        }

        .salary-slip-page .ss-grid tr:hover td { background: #F8FBFF; }

        .salary-slip-page .ss-grid .num { text-align: right; }

        .salary-slip-page .ss-dl {
            background: var(--ss-primary);
            border-radius: 6px;
            color: #FFFFFF !important;
            display: inline-block;
            font-size: 13px;
            font-weight: 600;
            padding: 6px 14px;
            text-decoration: none;
        }

        .salary-slip-page .ss-dl:hover { background: var(--ss-primary-dark); }

        /* Empty state */
        .salary-slip-page .ss-empty { padding: 56px 20px; text-align: center; }
        .salary-slip-page .ss-empty h4 { color: #475569; font-weight: 700; margin-bottom: 8px; }
        .salary-slip-page .ss-empty p { color: var(--ss-muted); margin: 0; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="salary-slip-page">
        <div class="row">
            <div class="col-12">
                <div class="ss-card">
                    <div class="ss-title">Salary Slip Details</div>

                    <!-- Filter -->
                    <div class="ss-filter">
                        <div class="row g-3 align-items-end">
                            <div class="col-md-2">
                                <div class="ss-filter-label">Year</div>
                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-2">
                                <div class="ss-filter-label">From Month</div>
                                <asp:DropDownList ID="ddlFromMonth" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-2">
                                <div class="ss-filter-label">To Month</div>
                                <asp:DropDownList ID="ddlToMonth" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <div class="ss-filter-label">Employee Name</div>
                                <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-select"></asp:DropDownList>
                            </div>
                            <div class="col-md-2">
                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="ss-search-btn w-100" OnClick="btnSearch_Click" />
                            </div>
                        </div>
                    </div>

                    <!-- Results grid -->
                    <asp:Panel ID="pnlResults" runat="server" Visible="false">
                        <div class="ss-grid-wrap">
                            <asp:GridView ID="gvSlips" runat="server" AutoGenerateColumns="false" CssClass="ss-grid"
                                GridLines="None" Width="100%" OnRowCommand="gvSlips_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Month">
                                        <ItemTemplate><%# GetMonthLabel(Eval("Month")) %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Year" HeaderText="Year" />
                                    <asp:TemplateField HeaderText="Total Earnings" ItemStyle-CssClass="num" HeaderStyle-CssClass="num">
                                        <ItemTemplate><%# FormatMoney(Eval("TotalEarnings")) %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total Deductions" ItemStyle-CssClass="num" HeaderStyle-CssClass="num">
                                        <ItemTemplate><%# FormatMoney(Eval("TotalDeductions")) %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Net Pay" ItemStyle-CssClass="num" HeaderStyle-CssClass="num">
                                        <ItemTemplate><%# FormatMoney(Eval("NetPay")) %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDownload" runat="server" CssClass="ss-dl"
                                                CommandName="DownloadSlip"
                                                CommandArgument='<%# string.Format("{0}|{1}|{2}", Eval("employeecode"), Eval("Year"), Eval("Month")) %>'>
                                                <i class="fa fa-download"></i>&nbsp;Download
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </asp:Panel>

                    <!-- Empty state -->
                    <asp:Panel ID="pnlNoData" runat="server" Visible="true">
                        <div class="ss-empty">
                            <h4>No Salary Slip Found</h4>
                            <p>Please select Year, From Month, To Month, and Employee to view salary slip details.</p>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>

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
</asp:Content>
