<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master"
    AutoEventWireup="true"
    CodeBehind="AppraisalDetails.aspx.cs"
    Inherits="HRMS.View.Modules.AppraisalDetails" %>

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

        .appraisal-form {
            display: none;
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
    </style>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="payroll-wrap">

        <!-- Header -->

        <div class="page-header">

            <div>
                <div class="page-title">
                    Appraisal Management
                </div>

                <div class="page-subtitle">
                    Manage employee appraisal records
                </div>
            </div>

            <asp:Button ID="btnAddNew"
                runat="server"
                Text="+ Add New Appraisal"
                CssClass="btn btn-primary"
                OnClick="btnAddNew_Click" />

        </div>

        <!-- Grid -->

        <div class="sec-card">

            <div class="sec-head">
                Appraisal Employee List
            </div>

            <asp:GridView ID="gvAppraisal"
                runat="server"
                CssClass="table appraisal-grid"
                AutoGenerateColumns="False"
                GridLines="None"
                DataKeyNames="appraisal_id"
                EmptyDataText="No appraisal records found">

                <Columns>
                    
                    <asp:BoundField DataField="employee_name"
                        HeaderText="Employee Name" />

                    <asp:BoundField DataField="appraisal_effective_date"
                        HeaderText="Effective Date"
                        DataFormatString="{0:dd-MMM-yyyy}" />

                    <asp:BoundField DataField="appraisal_ctc"
                        HeaderText="CTC" />

                    <asp:BoundField DataField="gross_salary"
                        HeaderText="Gross Salary" />

                    <asp:BoundField DataField="net_salary"
                        HeaderText="Net Salary" />

                    <asp:BoundField DataField="salary_revision_date"
                        HeaderText="Revision Date"
                        DataFormatString="{0:dd-MMM-yyyy}" />

                    <asp:BoundField DataField="increament_percentage"
                        HeaderText="Percentage %" />

                    <asp:TemplateField HeaderText="Action">

                        <ItemTemplate>

                            <asp:LinkButton ID="btnView"
                                runat="server"
                                CssClass="action-btn view-btn"
                                CommandName="View"
                                ToolTip="View"
                                CommandArgument='<%# Eval("appraisal_id") %>'
                                OnClick="btnView_Click"
                                 >

                                <i class='bx bx-show'></i>

                            </asp:LinkButton>

                            <asp:LinkButton ID="btnEdit"
                                runat="server"
                                CssClass="action-btn view-btn"
                                Style="background: #e6f7ff; color: #1890ff;"
                                CommandName="Edit"
                                ToolTip="Edit"
                                CommandArgument='<%# Eval("appraisal_id") %>'
                                OnClick="btnEdit_Click">

                                <i class='bx bx-edit'></i>

                            </asp:LinkButton>

                            <asp:LinkButton ID="btnDelete"
                                runat="server"
                                CssClass="action-btn delete-btn"
                                ToolTip="Delete"
                                OnClientClick="return confirmDelete(this);"
                                CommandArgument='<%# Eval("appraisal_id") %>'
                                OnClick="btnDelete_Click">

                                <i class='bx bx-trash'></i>

                            </asp:LinkButton>

                        </ItemTemplate>

                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

        </div>
    </div>

    <script>

        function toggleAppraisalForm() {

            var form = document.getElementById("divAppraisalForm");

            if (form.style.display === "none" ||
                form.style.display === "") {

                form.style.display = "block";
                window.scrollTo({
                    top: form.offsetTop - 80,
                    behavior: 'smooth'
                });
            }
            else {

                form.style.display = "none";
            }
        }

    </script>

    <script type="text/javascript">
        function confirmDelete(btn) {
            Swal.fire({
                title: 'Are you sure?',
                text: 'Do you want to delete this appraisal?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, Delete',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Store the original href (postback reference)
                    var originalHref = btn.href;
                    // Clear onclick to avoid re-triggering the confirmation
                    btn.onclick = null;
                    // Manually trigger the postback
                    window.location.href = originalHref;
                }
            });
            return false;
        }
    </script>

</asp:Content>
