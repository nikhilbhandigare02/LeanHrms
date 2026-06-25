<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="Remunerationdetails.aspx.cs" Inherits="HRMS.View.Modules.Remunerationdetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
,    <style>
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

        .remuneration-grid {
            width: 100%;
            border-collapse: collapse;
        }

            .remuneration-grid th {
                background: #f8fafc;
                padding: 12px;
                font-size: 13px;
                font-weight: 600;
                color: #374151;
                border-bottom: 1px solid #e5e7eb;
                text-align: left;
            }

            .remuneration-grid td {
                padding: 12px;
                font-size: 13px;
                border-bottom: 1px solid #edf2f7;
            }

        .action-btn {
            width: 35px;
            height: 35px;
            border-radius: 8px;
            border: none;
            display: inline-flex;
            justify-content: center;
            align-items: center;
            cursor: pointer;
        }

        .view-btn {
            background: #e0f2fe;
            color: #0284c7;
        }

        .delete-btn {
            background: #fee2e2;
            color: #dc2626;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="payroll-wrap">
        <!-- Header -->
        <div class="page-header">
            <div>
                <div class="page-title">
                    Remuneration Management
                </div>
                <div class="page-subtitle">
                    Manage employee remuneration records
                </div>
            </div>
        </div>

        <!-- Filters -->
        <div class="sec-card">
            <div class="sec-head">
                Filters
            </div>
            <div class="row mb-3">
                <div class="col-lg-6 col-md-6">
                    <div class="form-group">
                        <label class="fw-bold mb-1">Select Year</label>
                        <asp:DropDownList ID="ddlYear" runat="server"
                            CssClass="form-control">
                            <asp:ListItem Text="-- Select Year --" Value="" />
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="d-flex gap-2">
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success" OnClick="btnsearch_click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnclear_click" />
            </div>
        </div>

        <!-- Grid -->
        <div class="sec-card">
            <div class="sec-head">
                Remuneration List
            </div>
            <asp:HiddenField ID="hfPageIndexViewUser" runat="server" />
            <asp:GridView runat="server" ID="gridview" CssClass="table remuneration-grid" AutoGenerateColumns="false"
                DataKeyNames="remuneration_id" EnablePersistedSelection="true"
                OnPageIndexChanging="OnPageIndexChanging" PageSize="10"
                AllowSorting="true" OnSorting="gridview_Sorting" OnRowCommand="gridview_RowCommand"
                GridLines="None"
                Style="margin: 0 auto;" EmptyDataText="No records found.">
                <Columns>
                    <asp:TemplateField HeaderText="SR No">
                        <ItemTemplate>
                            <%# (gridview.PageIndex * gridview.PageSize + Container.DataItemIndex + 1) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="emp_code" HeaderText="Employee Code" />
                    <asp:BoundField DataField="user_fullname" HeaderText="Employee Name" />
                    <asp:BoundField DataField="salary_structure_id" HeaderText="Salary Structure" />
                    <asp:BoundField DataField="employee_category" HeaderText="Category" />
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnView"
                                runat="server"
                                CssClass="action-btn view-btn"
                                CommandName="View"
                                ToolTip="View"
                                CommandArgument='<%# Eval("remuneration_id") %>'
                                OnClick="btnView_Click">
                                <i class='bx bx-show'></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnEdit"
                                runat="server"
                                CssClass="action-btn view-btn"
                                Style="background: #e6f7ff; color: #1890ff;"
                                CommandName="Edit"
                                ToolTip="Edit"
                                CommandArgument='<%# Eval("remuneration_id") %>'
                                OnClick="btnEdit_Click">
                                <i class='bx bx-edit'></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnDelete"
                                runat="server"
                                CssClass="action-btn delete-btn"
                                ToolTip="Delete"
                                OnClientClick="return confirmDelete(this);"
                                CommandArgument='<%# Eval("remuneration_id") %>'
                                OnClick="btnDelete_Click">
                                <i class='bx bx-trash'></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <!-- Pagination -->
            <asp:Panel ID="paginationContainer" runat="server"
                CssClass="pagination-container"
                Style="text-align: right; font-size: 14px; color: black;"
                Visible="false">
                <asp:DropDownList runat="server" ID="ddlPageSelector" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlPageSelector_SelectedIndexChanged"
                    Style="background-color: white; color: black; border: 1px solid #ddd; padding: 5px 10px; margin: 2px;">
                </asp:DropDownList>
            </asp:Panel>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script type="text/javascript">
        function confirmDelete(btn) {
            Swal.fire({
                title: 'Are you sure?',
                text: 'Do you want to delete this remuneration?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, Delete',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    var originalHref = btn.href;
                    btn.onclick = null;
                    window.location.href = originalHref;
                }
            });
            return false;
        }
    </script>
</asp:Content>