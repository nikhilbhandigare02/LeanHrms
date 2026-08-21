<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="TerminationHistoryList.aspx.cs" Inherits="HRMS.View.Modules.TerminationHistoryList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .termination-page-header {
            display: flex;
            align-items: center;
            gap: 14px;
            margin-bottom: 20px;
        }

        .termination-page-header .icon-badge {
            width: 48px;
            height: 48px;
            flex: 0 0 48px;
            border-radius: 12px;
            background: linear-gradient(135deg, #556ee6, #6f86ef);
            color: #fff;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            box-shadow: 0 4px 10px rgba(85, 110, 230, 0.25);
        }

        .termination-page-header h1 {
            font-size: 24px;
            font-weight: 700;
            color: #212529;
            margin: 0;
        }

        .termination-page-header p {
            margin: 2px 0 0;
            font-size: 13.5px;
            color: #6c757d;
        }

        .history-gridview thead th {
            background-color: #f8f9fa;
            color: #495057;
            font-size: 12.5px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.4px;
            border-bottom: 2px solid #e9ecef;
            padding: 12px 10px;
            vertical-align: middle;
        }

        .history-gridview tbody td {
            padding: 12px 10px;
            vertical-align: middle;
            font-size: 13.5px;
            color: #343a40;
            border-bottom: 1px solid #f1f3f5;
        }

        .history-gridview tbody tr:hover {
            background-color: #f8f9ff;
        }

        .badge-emp-code {
            display: inline-block;
            padding: 4px 10px;
            font-size: 12px;
            font-weight: 600;
            color: #556ee6;
            background-color: rgba(85, 110, 230, 0.1);
            border-radius: 20px;
        }

        .badge-status {
            display: inline-block;
            padding: 4px 10px;
            font-size: 11.5px;
            font-weight: 700;
            border-radius: 20px;
        }

            .badge-status.terminated {
                color: #e03131;
                background-color: rgba(224, 49, 49, 0.1);
            }

            .badge-status.cap {
                color: #b45f00;
                background-color: rgba(240, 140, 0, 0.12);
            }

            .badge-status.showcause {
                color: #f08c00;
                background-color: rgba(240, 140, 0, 0.1);
            }

            .badge-status.removed {
                color: #868e96;
                background-color: rgba(134, 142, 150, 0.1);
            }

            .badge-status.other {
                color: #556ee6;
                background-color: rgba(85, 110, 230, 0.1);
            }

        .btn-view {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 14px;
            font-size: 12.5px;
            font-weight: 600;
            color: #556ee6;
            background-color: rgba(85, 110, 230, 0.08);
            border: 1px solid rgba(85, 110, 230, 0.2);
            border-radius: 20px;
            text-decoration: none;
            white-space: nowrap;
            transition: background-color 0.15s ease-in-out;
        }

            .btn-view:hover {
                background-color: rgba(85, 110, 230, 0.16);
                color: #4c63d2;
                text-decoration: none;
            }

        .history-action-row {
            display: flex;
            align-items: center;
            gap: 8px;
            flex-wrap: nowrap;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="termination-page-header">
        <div class="icon-badge">
            <i class="fa fa-history"></i>
        </div>
        <div>
            <h1>Termination History</h1>
            <p>Full record of every termination action taken - nothing here is ever deleted.</p>
        </div>
        <div class="ms-auto d-flex gap-2">
            <asp:Button ID="btnExport" runat="server" CssClass="btn btn-success"
                Text="Export" OnClick="btnExport_Click" />
            <a href="~/View/Modules/EmployeeAction.aspx" runat="server" class="btn btn-secondary">
                <i class="fa fa-arrow-left"></i> Back to Termination List
            </a>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-lg rounded-3">
                <div class="card-body">

                    <asp:GridView ID="gridHistory" runat="server"
                        CssClass="table history-gridview"
                        AutoGenerateColumns="false"
                        AllowPaging="true"
                        PageSize="15"
                        OnPageIndexChanging="gridHistory_PageIndexChanging"
                        EmptyDataText="No termination history found."
                        Style="margin: 0 auto;">
                        <Columns>
                            <asp:TemplateField HeaderText="SR No">
                                <ItemTemplate>
                                    <%# (gridHistory.PageIndex * gridHistory.PageSize) + Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Employee Code">
                                <ItemTemplate>
                                    <span class="badge-emp-code"><%# Eval("EmployeeCode") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="EmployeeName" HeaderText="Employee Name" />
                            <asp:BoundField DataField="ActionType" HeaderText="Action Type" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <span class="badge-status <%# GetStatusBadgeClass(Eval("Status")) %>"><%# Eval("Status") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Recorded On">
                                <ItemTemplate>
                                    <%# Eval("RecordedDate") != null && Eval("RecordedDate") != DBNull.Value ? Convert.ToDateTime(Eval("RecordedDate")).ToString("dd-MMM-yyyy") : "-" %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <div class="history-action-row">
                                        <asp:HyperLink runat="server"
                                            CssClass="btn-view"
                                            ToolTip="View Termination Details"
                                            NavigateUrl='<%# "~/View/Modules/TerminationDetailsView.aspx?user_id=" + Eval("UserId") %>'>
    <i class="fa fa-eye"></i> View
                                        </asp:HyperLink>

                                        <asp:HyperLink runat="server"
                                            CssClass="btn-view"
                                            ToolTip="Continue managing this employee's termination"
                                            NavigateUrl='<%# "~/View/Modules/EmployeeAction.aspx?manage_user_id=" + Eval("UserId") %>'
                                            Visible='<%# IsManageable(Eval("Status")) %>'>
    <i class="fa fa-cogs"></i> Manage
                                        </asp:HyperLink>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                </div>
            </div>
        </div>
    </div>

</asp:Content>
