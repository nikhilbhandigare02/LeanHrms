<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="EmployeeAction.aspx.cs" Inherits="HRMS.View.Modules.EmployeeAction" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/css/bootstrap-datepicker.min.css" />
    <link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <%--    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />--%>

    <style>
        .custom-dropdown-container {
            position: relative;
        }

        .custom-dropdown {
            padding-right: 25px;
            -webkit-appearance: none;
            -moz-appearance: none;
            appearance: none;
            background: url('data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"%3E%3Cpath stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" /%3E%3C/svg%3E') no-repeat right center;
            background-size: 16px;
        }
    </style>
    <style>
        .star {
            font-size: 30px;
            color: gray;
            cursor: pointer;
        }

            .star.active {
                color: gold;
            }
    </style>
    <style>
        .nav-pills .nav-link {
            cursor: pointer;
        }
    </style>

    <style>
        .page-list {
            display: flex;
            align-items: center;
            list-style: none;
            margin: 0;
            padding: 0;
            gap: 6px;
        }

            .page-list li {
                display: inline-block;
            }

        .page-btn {
            display: inline-block;
            min-width: 34px;
            padding: 6px 10px;
            text-align: center;
            border: 1px solid #dee2e6;
            border-radius: 6px;
            color: #495057;
            background-color: #fff;
            text-decoration: none;
            font-weight: 500;
            font-size: 13px;
            cursor: pointer;
        }

            .page-btn:hover {
                background-color: #f1f3f5;
                color: #495057;
                text-decoration: none;
            }

            .page-btn.active {
                background-color: #556ee6;
                border-color: #556ee6;
                color: #fff;
            }

            .page-btn[disabled],
            .page-btn.disabled {
                opacity: 0.5;
                pointer-events: none;
                cursor: default;
            }
    </style>

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
            background: linear-gradient(135deg, #e03131, #ff6b6b);
            color: #fff;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            box-shadow: 0 4px 10px rgba(224, 49, 49, 0.25);
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

        .custom-gridview thead th {
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

        .custom-gridview tbody td {
            padding: 12px 10px;
            vertical-align: middle;
            font-size: 13.5px;
            color: #343a40;
            border-bottom: 1px solid #f1f3f5;
        }

        .custom-gridview tbody tr:hover {
            background-color: #f8f9ff;
        }

        .emp-avatar {
            width: 34px;
            height: 34px;
            border-radius: 50%;
            background: #556ee6;
            color: #fff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 13px;
            font-weight: 600;
            margin-right: 10px;
            flex: 0 0 34px;
        }

        .emp-name-cell {
            display: flex;
            align-items: center;
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

        .btn-terminate {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 14px;
            font-size: 12.5px;
            font-weight: 600;
            color: #e03131;
            background-color: rgba(224, 49, 49, 0.08);
            border: 1px solid rgba(224, 49, 49, 0.2);
            border-radius: 20px;
            text-decoration: none;
            transition: background-color 0.15s ease-in-out;
        }

            .btn-terminate:hover {
                background-color: rgba(224, 49, 49, 0.16);
                color: #c92a2a;
                text-decoration: none;
            }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--  <div class="modal fade" id="terminationModal" tabindex="-1"
        aria-labelledby="terminationLabel" aria-hidden="true">

        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">

                <!-- Header -->
                <div class="modal-header">
                    <h5 class="modal-title" id="terminationLabel">Terminate Employee</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>

                <!-- Body -->
                <div class="modal-body">

                    <asp:HiddenField ID="hfUserId" runat="server" />
                    <asp:HiddenField ID="hfEmployeeCode" runat="server" />


                    <!-- Termination Date -->
                    <!-- Termination Date -->
                    <div class="mb-3">
                        <label class="form-label">Termination Date</label>
                        <div class="input-group">
                            <asp:TextBox ID="txtTerminationDate" runat="server"
                                CssClass="form-control"
                                placeholder="Select termination date"
                                autocomplete="off" />
                            <span class="input-group-text">
                                <i class="fas fa-calendar-alt"></i>
                            </span>
                        </div>
                        <span id="spanDateError" class="text-danger"></span>
                    </div>

                    <!-- Termination Reason -->
                    <div class="mb-3">
                        <label class="form-label">Reason for Termination</label>
                        <asp:DropDownList ID="ddlTerminationReason" runat="server"
                            CssClass="form-control">
                        </asp:DropDownList>
                        <span id="spanReasonError" class="text-danger"></span>
                    </div>


                    <!-- Remark -->
                    <div class="mb-3">
                        <label class="form-label">Do you want to further specify the reason</label>
                        <asp:TextBox ID="txtTerminationRemark" runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="3" />
                    </div>

                    <!-- Info Message -->
                    <div class="alert alert-warning">
                        The employee will lose access to the system after the termination date.
                    </div>

                </div>

                <!-- Footer -->
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary"
                        data-bs-dismiss="modal">
                        Cancel
                    </button>

                    <asp:Button ID="btnConfirmTermination" runat="server"
                        CssClass="btn btn-danger"
                        Text="Terminate Employee"
                        OnClick="btnConfirmTermination_Click"
                          OnClientClick="return validateTerminationForm();"/>
                </div>

            </div>
        </div>
    </div>--%>

    <!-- Termination Modal -->
    <div class="modal fade" id="terminationModal" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content shadow">

                <!-- Header -->
                <div class="modal-header bg-danger text-white">
                    <h5 class="modal-title">
                        <i class="fas fa-user-times me-2"></i>Employee Termination
                    </h5>
                    <button type="button" class="btn-close btn-close-white"
                        data-bs-dismiss="modal">
                    </button>
                </div>

                <!-- Body -->
                <div class="modal-body">

                    <!-- Hidden Fields -->
                    <asp:HiddenField ID="hfUserId" runat="server" />
                    <asp:HiddenField ID="hfEmployeeCode" runat="server" />
                    <asp:HiddenField ID="hfEmployeeEmail" runat="server" />
                    <asp:HiddenField ID="hfEmployeeName" runat="server" />
                    <asp:HiddenField ID="hfCompanyId" runat="server" />

                    <asp:HiddenField ID="hfTerminationType" runat="server" Value="Performance" />
                    <asp:HiddenField ID="hfPerformanceRating" runat="server" />



                    <%--  <!-- Tabs -->
                <ul class="nav nav-pills mb-3">
                    <li class="nav-item">
                        <a class="nav-link active"
                           href="#"
                           onclick="showPerformanceBased(); return false;">
                            Performance Based Letter
                        </a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link"
                           href="#"
                           onclick="showShowCause(); return false;">
                            Show Cause Notice
                        </a>
                    </li>
                </ul>--%>
                    <ul class="nav nav-pills mb-3">
                        <li class="nav-item">
                            <a id="tabPerformance"
                                class="nav-link active"
                                href="#"
                                onclick="showPerformanceBased(); return false;">Performance Based Letter
                            </a>
                        </li>
                        <li class="nav-item">
                            <a id="tabShowCause"
                                class="nav-link"
                                href="#"
                                onclick="showShowCause(); return false;">Show Cause Notice
                            </a>
                        </li>
                    </ul>

                    <!-- PERFORMANCE BASED -->
                    <div id="performanceSection">

                        <div class="alert alert-warning">
                            <strong>Performance-Based Termination</strong>
                            <ul class="mb-0 mt-2">
                                <li>Performance reviewed over a defined period</li>
                                <li>KPIs and targets were not achieved</li>
                                <li>Prior warnings / PIP were issued</li>
                                <li>Decision supported by documentation</li>
                            </ul>
                        </div>

                        <!-- ⭐ Star Rating -->
                        <div class="row mb-3">

                            <!-- ⭐ Performance Rating -->
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Performance Rating</label>
                                <div class="star-rating">
                                    <span class="star" onclick="setRating(1)">★</span>
                                    <span class="star" onclick="setRating(2)">★</span>
                                    <span class="star" onclick="setRating(3)">★</span>
                                    <span class="star" onclick="setRating(4)">★</span>
                                    <span class="star" onclick="setRating(5)">★</span>
                                </div>
                            </div>

                            <!-- 📅 Notice Period -->
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Notice Period</label>
                                <asp:DropDownList ID="ddlNoticePeriod"
                                    runat="server"
                                    CssClass="form-select">
                                    <asp:ListItem Text="Immediate" Value="0" />
                                    <asp:ListItem Text="7 Days" Value="7" />
                                    <asp:ListItem Text="15 Days" Value="15" />
                                    <asp:ListItem Text="30 Days" Value="30" />
                                </asp:DropDownList>
                            </div>

                        </div>


                        <!-- Letter Preview -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">Termination Letter Preview</label>
                            <asp:TextBox ID="txtLetterPreview"
                                runat="server"
                                CssClass="form-control"
                                Rows="4"
                                TextMode="MultiLine" />
                        </div>
                    </div>

                    <!-- SHOW CAUSE -->
                    <div id="showCauseSection" style="display: none;">

                        <div class="alert alert-warning">
                            <strong>Show Cause Notice</strong>
                            <ul class="mb-0 mt-2">
                                <li>Show cause notice is issued</li>
                                <li>15 days response deadline</li>
                                <li>Violation or misconduct under review</li>
                            </ul>
                        </div>

                        <!-- 📅 Response Deadline (same date picker UI) -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">Response Deadline</label>
                            <div class="input-group">
                                <asp:TextBox ID="txtResponseDeadline"
                                    runat="server"
                                    CssClass="form-control datepicker"
                                    placeholder="Select response deadline"
                                    autocomplete="off" />
                                <span class="input-group-text">
                                    <i class="fas fa-calendar-alt"></i>
                                </span>
                            </div>
                        </div>

                        <!-- Notice Letter -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">Notice Letter Content</label>
                            <asp:TextBox ID="txtNoticeLetter"
                                runat="server"
                                CssClass="form-control"
                                Rows="4"
                                TextMode="MultiLine" />
                        </div>
                        <!-- Escalate Button -->
                        <div class="d-flex justify-content-end gap-2 mt-3">
                            <asp:Button ID="btnSendShowCause"
                                runat="server"
                                Text="Send Show Cause Notice"
                                CssClass="btn btn-danger"
                                OnClick="btnSendShowCause_Click" />

                            <button type="button"
                                class="btn btn-danger"
                                onclick="escalateToTermination()">
                                Escalate to Termination
                            </button>
                        </div>


                    </div>


                    <%-- <div class="mb-3">
          <label class="form-label fw-bold">Termination Date</label>
          <div class="input-group">
              <asp:TextBox ID="txtTerminationDate"
                           runat="server"
                           CssClass="form-control datepicker"
                           placeholder="Select termination date"
                           autocomplete="off" />
              <span class="input-group-text">
                  <i class="fas fa-calendar-alt"></i>
              </span>
          </div>
          <span id="spanDateError" class="text-danger"></span>
      </div>--%>

                    <div id="terminationDateSection" class="mb-3">
                        <label class="form-label fw-bold">Termination Date</label>
                        <div class="input-group">
                            <asp:TextBox ID="txtTerminationDate"
                                runat="server"
                                CssClass="form-control datepicker"
                                placeholder="Select termination date"
                                autocomplete="off" />
                            <span class="input-group-text">
                                <i class="fas fa-calendar-alt"></i>
                            </span>
                        </div>
                        <span id="spanDateError" class="text-danger"></span>
                    </div>

                    <div class="alert alert-warning">
                        <i class="fas fa-exclamation-triangle me-1"></i>
                        Employee access will be disabled after termination.
                    </div>

                </div>

                <!-- Footer -->
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmTermination"
                        runat="server"
                        Text="Submit"
                        CssClass="btn btn-danger"
                        OnClick="btnConfirmTermination_Click" />
                    <button type="button"
                        class="btn btn-secondary"
                        data-bs-dismiss="modal">
                        Cancel
                    </button>
                </div>

            </div>
        </div>
    </div>





    <div class="termination-page-header">
        <div class="icon-badge">
            <i class="fa fa-user-times"></i>
        </div>
        <div>
            <h1>Termination List</h1>
            <p>Select an employee below to initiate the termination process.</p>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-lg rounded-3">
                <div class="card-body">

                    <!-- GridView -->
                    <div class="row">
                        <div class="col-12">
                            <asp:HiddenField ID="hfPageIndexViewUser" runat="server" />
                            <asp:GridView runat="server" ID="gridview" class="table custom-gridview" AutoGenerateColumns="false"
                                DataKeyNames="UserId,EmployeeCode,user_mail_id,user_fullname,company_id" EnablePersistedSelection="true"
                                OnPageIndexChanging="OnPageIndexChanging" PageSize="10"
                                AllowSorting="true" OnSorting="gridview_Sorting" OnRowCommand="gvEmployees_RowCommand"
                                Style="margin: 0 auto;" EmptyDataText="No records found.">
                                <Columns>
                                    <asp:TemplateField HeaderText="SR No">
                                        <ItemTemplate>
                                            <%# (gridview.PageIndex * gridview.PageSize) + Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Employee Code">
                                        <ItemTemplate>
                                            <span class="badge-emp-code"><%# Eval("EmployeeCode") %></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="UserId" HeaderText="User Id"  Visible="false" />
                                    <asp:BoundField DataField="Username" HeaderText="Username"  />
                                    <asp:TemplateField HeaderText="Employee Name">
                                        <ItemTemplate>
                                            <div class="emp-name-cell">
                                                <span class="emp-avatar"><%# GetInitials(Eval("user_fullname").ToString()) %></span>
                                                <span><%# Eval("user_fullname") %></span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="user_mail_id" HeaderText="Email Id"  />
                                    <asp:BoundField DataField="contact_detail" HeaderText="Contact Number"/>

                                    <%-- <asp:TemplateField HeaderText="Action" ItemStyle-Width="80px">
    <ItemTemplate>
        <asp:LinkButton 
            ID="lnkTerminate" 
            runat="server"
            CssClass="me-1"
            CommandArgument='<%# Eval("UserId") %>'
            OnClientClick='<%# "openTerminationModal(" + Eval("UserId") + "); return false;" %>'
            ToolTip="Terminate Employee">
            <i class="fa fa-user-times text-danger"></i>
        </asp:LinkButton>
    </ItemTemplate>
</asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="80px">
                                        <ItemTemplate>

                                            <%-- <asp:LinkButton
                                                ID="lnkTerminate"
                                                runat="server"
                                                CssClass="me-1"
                                                ToolTip="Terminate Employee"
                                                OnClientClick='<%# "openTerminationModal(" 
        + Eval("UserId") 
        + ", \"" + HttpUtility.JavaScriptStringEncode(Eval("EmployeeCode").ToString()) + "\""
        + ", \"" + HttpUtility.JavaScriptStringEncode(Eval("user_mail_id").ToString()) + "\""
        + ", \"" + HttpUtility.JavaScriptStringEncode(Eval("user_fullname").ToString()) + "\""
        + "); return false;" %>'>
    <i class="fa fa-user-times text-danger"></i>
                                            </asp:LinkButton>--%>
                                            <%-- <asp:LinkButton
                                                ID="lnkTerminate"
                                                runat="server"
                                                CssClass="me-1"
                                                ToolTip="Terminate Employee"
                                                CommandName="SelectEmployee"
                                                CommandArgument='<%# Eval("UserId") %>'
                                                OnClientClick='<%# "openTerminationModal(" 
        + Eval("UserId") 
        + ", \"" + HttpUtility.JavaScriptStringEncode(Eval("EmployeeCode").ToString()) + "\""
        + ", \"" + HttpUtility.JavaScriptStringEncode(Eval("user_mail_id").ToString()) + "\""
        + ", \"" + HttpUtility.JavaScriptStringEncode(Eval("user_fullname").ToString()) + "\""
        + ");" %>'>
    <i class="fa fa-user-times text-danger"></i>
                                            </asp:LinkButton>--%>


                                            <%-- <asp:LinkButton
    ID="lnkTerminate"
    runat="server"
    CssClass="me-1"
    ToolTip="Terminate Employee"
    CommandName="SelectEmployee"
    CommandArgument='<%# Eval("UserId") %>'>
    
    <i class="fa fa-user-times text-danger"></i>

</asp:LinkButton>--%>

                                            <asp:LinkButton
                                                ID="lnkTerminate"
                                                runat="server"
                                                CssClass="btn-terminate"
                                                ToolTip="Terminate Employee"
                                                CommandName="SelectEmployee"
                                                CommandArgument='<%# Container.DataItemIndex %>'>
    <i class="fa fa-user-times"></i> Terminate
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

                                <ul class="page-list">
                                    <li>
                                        <asp:LinkButton ID="lnkPrevPage" runat="server" CssClass="page-btn" OnClick="lnkPrevPage_Click">&laquo; Prev</asp:LinkButton>
                                    </li>
                                    <asp:Repeater ID="rptPageNumbers" runat="server" OnItemCommand="rptPageNumbers_ItemCommand">
                                        <ItemTemplate>
                                            <li>
                                                <asp:LinkButton ID="lnkPageNumber" runat="server" CssClass='<%# (bool)Eval("IsActive") ? "page-btn active" : "page-btn" %>' CommandName="GoToPage" CommandArgument='<%# Eval("PageIndex") %>' Text='<%# Eval("PageNumber") %>'></asp:LinkButton>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <li>
                                        <asp:LinkButton ID="lnkNextPage" runat="server" CssClass="page-btn" OnClick="lnkNextPage_Click">Next &raquo;</asp:LinkButton>
                                    </li>
                                </ul>
                            </asp:Panel>


                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
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
    <script>
        function openTerminationModal(userId, employeeCode, email, name) {
            document.getElementById('<%= hfUserId.ClientID %>').value = userId;
            document.getElementById('<%= hfEmployeeCode.ClientID %>').value = employeeCode;
            document.getElementById('<%= hfEmployeeEmail.ClientID %>').value = email;
            document.getElementById('<%= hfEmployeeName.ClientID %>').value = name;

            var modal = new bootstrap.Modal(
                document.getElementById('terminationModal')
            );
            modal.show();
        }

    </script>
    <script type="text/javascript">
        $(document).ready(function () {

            flatpickr("#<%= txtTerminationDate.ClientID %>", {
                dateFormat: "d-m-Y",
                allowInput: true,
                minDate: "today"
            });

            flatpickr("#<%= txtResponseDeadline.ClientID %>", {
                dateFormat: "d-m-Y",
                allowInput: true,
                minDate: "today"
            });


            $('.input-group-text').on('click', function () {
                $(this).closest('.input-group').find('input').focus();
            });
        });


    </script>

    <script>
        window.onload = function () {
            var terminationDate = document.getElementById('<%= txtTerminationDate.ClientID %>');
<%--        var terminationReason = document.getElementById('<%= ddlTerminationReason.ClientID %>');--%>

            // Remove error when user changes date
            terminationDate.addEventListener('input', function () {
                terminationDate.classList.remove('is-invalid');
                document.getElementById('spanDateError').innerText = '';
            });

            // Remove error when user changes dropdown
            terminationReason.addEventListener('change', function () {
                terminationReason.classList.remove('is-invalid');
                document.getElementById('spanReasonError').innerText = '';
            });
        };

        function validateTerminationForm() {
            var terminationDate = document.getElementById('<%= txtTerminationDate.ClientID %>');
<%--       var terminationReason = document.getElementById('<%= ddlTerminationReason.ClientID %>');--%>
            var isValid = true;

            // Clear previous errors
            terminationDate.classList.remove('is-invalid');
            terminationReason.classList.remove('is-invalid');
            document.getElementById('spanDateError').innerText = '';
            document.getElementById('spanReasonError').innerText = '';

            // Validate date (d-m-Y)
            var dateVal = terminationDate.value.trim();
            if (!dateVal) {
                terminationDate.classList.add('is-invalid');
                document.getElementById('spanDateError').innerText = 'Termination date is required.';
                isValid = false;
            } else {
                var dateRegex = /^(0?[1-9]|[12][0-9]|3[01])-(0?[1-9]|1[0-2])-(\d{4})$/;
                if (!dateRegex.test(dateVal)) {
                    terminationDate.classList.add('is-invalid');
                    document.getElementById('spanDateError').innerText = 'Invalid date format. Use DD-MM-YYYY.';
                    isValid = false;
                }
            }

            // Validate reason
            var reasonVal = terminationReason.value;
            if (reasonVal === "" || reasonVal === "0") {
                terminationReason.classList.add('is-invalid');
                document.getElementById('spanReasonError').innerText = 'Please select a termination reason.';
                isValid = false;
            }

            return isValid;
        }
    </script>

    <%-- <script>
     function setRating(value) {
         document.getElementById("<%= hfPerformanceRating.ClientID %>").value = value;

         const stars = document.querySelectorAll(".star-rating .star");
         stars.forEach((star, index) => {
             star.classList.toggle("selected", index < value);
         });
     }

     function showPerformanceBased() {
         document.getElementById("performanceSection").style.display = "block";
         document.getElementById("showCauseSection").style.display = "none";
         document.getElementById("<%= hfTerminationType.ClientID %>").value = "Performance";
    }

    function showShowCause() {
        document.getElementById("performanceSection").style.display = "none";
        document.getElementById("showCauseSection").style.display = "block";
        document.getElementById("<%= hfTerminationType.ClientID %>").value = "ShowCause";
     }
 </script>--%>



    <%-- <script>
      function showPerformanceBased() {
          document.getElementById("performanceSection").style.display = "block";
          document.getElementById("showCauseSection").style.display = "none";
          document.getElementById("<%= hfTerminationType.ClientID %>").value = "Performance";
      }

      function showShowCause() {
          document.getElementById("performanceSection").style.display = "none";
          document.getElementById("showCauseSection").style.display = "block";
          document.getElementById("<%= hfTerminationType.ClientID %>").value = "ShowCause";
}

function setRating(val) {
    var hf = document.getElementById('<%= hfPerformanceRating.ClientID %>');
          if (hf) hf.value = val;

          document.querySelectorAll(".star").forEach((s, i) => {
              s.classList.toggle("active", i < val);
          });
      }
      window.addEventListener('DOMContentLoaded', (event) => {
          document.getElementById('<%= hfTerminationType.ClientID %>').value = "Performance";
      });

  </script>

    <script>
        function escalateToTermination() {

            // Hide show cause
            document.getElementById("showCauseSection").style.display = "none";

            // Show termination
            document.getElementById("terminationSection").style.display = "block";

            // Copy response deadline → termination date
            var responseDate = document.getElementById('<%= txtResponseDeadline.ClientID %>').value;
        var terminationDate = document.getElementById('<%= txtTerminationDate.ClientID %>');

            if (!responseDate) {
                alert("Please select Response Deadline first.");
                return;
            }

            terminationDate.value = responseDate;
        }
    </script>--%>

    <script>
        function showPerformanceBased() {
            document.getElementById("performanceSection").style.display = "block";
            document.getElementById("showCauseSection").style.display = "none";
            document.getElementById("terminationDateSection").style.display = "block";

            // Tab color change
            document.getElementById("tabPerformance").classList.add("active");
            document.getElementById("tabShowCause").classList.remove("active");

            document.getElementById("<%= hfTerminationType.ClientID %>").value = "Performance";
        }

        function showShowCause() {
            document.getElementById("performanceSection").style.display = "none";
            document.getElementById("showCauseSection").style.display = "block";
            document.getElementById("terminationDateSection").style.display = "none";

            // Tab color change
            document.getElementById("tabPerformance").classList.remove("active");
            document.getElementById("tabShowCause").classList.add("active");

            document.getElementById("<%= hfTerminationType.ClientID %>").value = "ShowCause";
        }

        function escalateToTermination() {

            var responseDate = document.getElementById('<%= txtResponseDeadline.ClientID %>').value;

            // Show termination date ONLY now
            document.getElementById("terminationDateSection").style.display = "block";

            // Copy response deadline → termination date
            var terminationDate = document.getElementById('<%= txtTerminationDate.ClientID %>');
            terminationDate.value = responseDate;

            // Scroll & focus
            terminationDate.scrollIntoView({ behavior: "smooth", block: "center" });
            //    terminationDate.focus();
        }

        function setRating(val) {
            var hf = document.getElementById('<%= hfPerformanceRating.ClientID %>');
            if (hf) hf.value = val;

            document.querySelectorAll(".star").forEach((s, i) => {
                s.classList.toggle("active", i < val);
            });
        }

        window.addEventListener('DOMContentLoaded', function () {
            // Default = Performance
            document.getElementById("terminationDateSection").style.display = "block";
            document.getElementById("<%= hfTerminationType.ClientID %>").value = "Performance";
        });
    </script>

</asp:Content>
