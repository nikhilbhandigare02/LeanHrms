<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="KnowledgeTransfer.aspx.cs" Inherits="HRMS.View.Modules.KnowledgeTransfer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .kt-page .card {
            border: 0;
            border-radius: 14px;
            box-shadow: 0 10px 28px rgba(31, 45, 61, 0.08);
        }

        .kt-page .page-title {
            font-size: 1.6em;
            font-weight: bold;
            color: #1f2d3d;
        }

        .kt-page .info-label {
            font-size: 12px;
            color: #94a3b8;
            text-transform: uppercase;
            letter-spacing: 0.4px;
            margin-bottom: 2px;
        }

        .kt-page .info-value {
            font-size: 14px;
            color: #1f2d3d;
            font-weight: 600;
            margin-bottom: 14px;
            display: block;
            word-wrap: break-word;
        }

        .kt-page label.field-label {
            font-weight: 600;
            color: #334155;
            margin-bottom: 6px;
            display: block;
        }

        .kt-page .btn-accept {
            background: linear-gradient(135deg, #16a34a, #15803d);
            border: 0;
            color: #fff;
            border-radius: 8px;
            font-weight: 600;
            padding: 10px 22px;
        }

        .kt-page .btn-cancel {
            background: #64748b;
            border: 0;
            color: #fff;
            border-radius: 8px;
            font-weight: 600;
            padding: 10px 22px;
        }

        .kt-page .table.custom-gridview {
            margin-bottom: 0;
            border-collapse: separate;
            border-spacing: 0;
        }

        .kt-page .table.custom-gridview thead th {
            font-size: 13px;
            font-weight: 700;
            color: #334155;
            background: #f8fafc;
            border-bottom: 1px solid #e2e8f0;
            padding: 10px;
        }

        .kt-page .table.custom-gridview tbody td {
            font-size: 13px;
            color: #334155;
            vertical-align: middle;
            padding: 10px;
            border-bottom: 1px solid #eef2f7;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="kt-page">
        <asp:HiddenField ID="hfResignationId" runat="server" />
        <asp:HiddenField ID="hfKTId" runat="server" />

        <div class="row">
            <div class="col-lg-12">
                <div class="card">
                    <div class="card-body">

                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <span class="page-title">Knowledge Transfer &amp; Handover</span>
                        </div>

                        <!-- Employee Information (Read Only) -->
                        <div class="row mb-2">
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Employee ID</div>
                                <asp:Label ID="lblEmployeeId" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Employee Name</div>
                                <asp:Label ID="lblEmployeeName" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Department</div>
                                <asp:Label ID="lblDepartment" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Designation</div>
                                <asp:Label ID="lblDesignation" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Resignation Date</div>
                                <asp:Label ID="lblResignationDate" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Proposed Last Working Date</div>
                                <asp:Label ID="lblProposedLastWorkingDate" runat="server" CssClass="info-value" Text="-" />
                            </div>
                        </div>

                        <hr />

                        <h5 class="mb-3">KT plan</h5>

                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <asp:TextBox ID="txtKTPlan" runat="server"
                                    CssClass="form-control" TextMode="MultiLine" Rows="3"
                                    placeholder="Required" />
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="field-label">Replacement employee (optional)</label>
                                <asp:TextBox ID="txtReplacementEmployee" runat="server" CssClass="form-control" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="field-label">KT status</label>
                                <asp:DropDownList ID="ddlKTStatus" runat="server" CssClass="form-control custom-dropdown">
                                    <asp:ListItem Text="Pending" Value="Pending" />
                                    <asp:ListItem Text="In Progress" Value="In Progress" />
                                    <asp:ListItem Text="Completed" Value="Completed" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="field-label">KT start date</label>
                                <asp:TextBox ID="txtKTStartDate" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="field-label">KT completion date</label>
                                <asp:TextBox ID="txtKTCompletionDate" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                        </div>

                        <h5 class="mb-3 mt-2">Project handover</h5>

                        <div class="table-responsive mb-3">
                            <asp:GridView runat="server" ID="gvProjectHandover" class="table custom-gridview" AutoGenerateColumns="false"
                                OnRowCommand="gvProjectHandover_RowCommand" EmptyDataText="No project handover rows added yet.">
                                <Columns>
                                    <asp:BoundField DataField="ProjectName" HeaderText="Project Name" />
                                    <asp:BoundField DataField="AssignedEmployee" HeaderText="Assigned Employee" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" />
                                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="90px">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkRemoveRow" runat="server" CommandName="RemoveRow"
                                                CommandArgument="<%# Container.DataItemIndex %>" CausesValidation="false"
                                                CssClass="btn btn-sm btn-outline-danger" ToolTip="Remove row">
                                                <i class="fa fa-trash"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>

                        <div class="row align-items-end mb-3">
                            <div class="col-md-4 mb-2">
                                <label class="field-label">Project Name</label>
                                <asp:TextBox ID="txtNewProjectName" runat="server" CssClass="form-control" />
                            </div>
                            <div class="col-md-4 mb-2">
                                <label class="field-label">Assigned Employee</label>
                                <asp:TextBox ID="txtNewAssignedEmployee" runat="server" CssClass="form-control" />
                            </div>
                            <div class="col-md-3 mb-2">
                                <label class="field-label">Status</label>
                                <asp:DropDownList ID="ddlNewProjectStatus" runat="server" CssClass="form-control custom-dropdown">
                                    <asp:ListItem Text="Pending" Value="Pending" />
                                    <asp:ListItem Text="In Progress" Value="In Progress" />
                                    <asp:ListItem Text="Completed" Value="Completed" />
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-1 mb-2">
                                <asp:Button ID="btnAddProjectRow" runat="server" CssClass="btn btn-secondary w-100"
                                    Text="Add" CausesValidation="false" OnClick="btnAddProjectRow_Click" />
                            </div>
                        </div>

                        <div class="d-flex gap-2 mt-3">
                            <asp:Button ID="btnSaveKT" runat="server"
                                CssClass="btn-accept" Text="Save"
                                OnClientClick="return validateKTForm();"
                                OnClick="btnSaveKT_Click" />
                            <asp:Button ID="btnCancel" runat="server"
                                CssClass="btn-cancel" Text="Cancel"
                                CausesValidation="false"
                                OnClick="btnCancel_Click" />
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script>
        function showKTResult(status, message, redirectUrl) {
            var isSuccess = status.toLowerCase() === "success";
            Swal.fire({
                icon: isSuccess ? "success" : "error",
                text: message,
                timer: isSuccess ? 2500 : undefined,
                showConfirmButton: !isSuccess
            }).then(function () {
                if (redirectUrl) {
                    window.location.href = redirectUrl;
                }
            });
        }

        function validateKTForm() {
            var ktPlan = document.getElementById('<%= txtKTPlan.ClientID %>').value.trim();
            if (!ktPlan) {
                alert('KT Plan is mandatory.');
                return false;
            }
            return true;
        }
    </script>
</asp:Content>
