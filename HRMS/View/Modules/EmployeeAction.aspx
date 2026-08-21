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

        .term-action-cell {
            display: flex;
            flex-direction: column;
            align-items: flex-start;
            gap: 6px;
            min-width: 160px;
        }

        .term-action-row {
            display: flex;
            align-items: center;
            gap: 6px;
            flex-wrap: nowrap;
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
            white-space: nowrap;
            transition: background-color 0.15s ease-in-out;
        }

            .btn-terminate:hover {
                background-color: rgba(224, 49, 49, 0.16);
                color: #c92a2a;
                text-decoration: none;
            }

        .btn-terminated {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 14px;
            font-size: 12.5px;
            font-weight: 600;
            color: #868e96;
            background-color: rgba(134, 142, 150, 0.1);
            border: 1px solid rgba(134, 142, 150, 0.25);
            border-radius: 20px;
            white-space: nowrap;
            cursor: not-allowed;
        }

        .btn-cap-badge {
            display: inline-flex;
            align-items: center;
            padding: 4px 10px;
            font-size: 11px;
            font-weight: 700;
            letter-spacing: 0.2px;
            color: #b45f00;
            background-color: rgba(240, 140, 0, 0.12);
            border: 1px solid rgba(240, 140, 0, 0.25);
            border-radius: 20px;
            white-space: nowrap;
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

        .required-asterisk {
            color: #e03131;
            margin-left: 2px;
        }

        .field-error {
            display: block;
            margin-top: 4px;
            font-size: 12.5px;
            color: #e03131;
        }

        .is-invalid-field {
            border-color: #e03131 !important;
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
                    <asp:HiddenField ID="hfCapStage" runat="server" Value="1" />



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
                        <li class="nav-item">
                            <a id="tabDirectTerminate"
                                class="nav-link"
                                href="#"
                                onclick="showDirectTerminate(); return false;">Direct Terminate
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
                                <label class="form-label fw-bold">Performance Rating<span class="required-asterisk">*</span></label>
                                <div class="star-rating">
                                    <span class="star" onclick="setRating(1)">★</span>
                                    <span class="star" onclick="setRating(2)">★</span>
                                    <span class="star" onclick="setRating(3)">★</span>
                                    <span class="star" onclick="setRating(4)">★</span>
                                    <span class="star" onclick="setRating(5)">★</span>
                                </div>
                                <span id="spanPerformanceRatingError" class="field-error"></span>
                            </div>

                            <!-- 📅 Notice Period -->
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Notice Period (Days)<span class="required-asterisk">*</span></label>
                                <asp:TextBox ID="txtNoticePeriod"
                                    runat="server"
                                    CssClass="form-control"
                                    Text="0"
                                    placeholder="Enter notice period in days (0 = Immediate)"
                                    onkeypress="return allowDigitsOnly(event);"
                                    onpaste="return blockNonDigitPaste(event);"
                                    oninput="sanitizeDigitsOnly(this); calculateTerminationDate();"
                                    onchange="calculateTerminationDate();" />
                                <span id="spanNoticePeriodError" class="field-error"></span>
                            </div>

                        </div>


                        <!-- Letter Preview -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">Termination Letter Preview<span class="required-asterisk">*</span></label>
                            <asp:TextBox ID="txtLetterPreview"
                                runat="server"
                                CssClass="form-control"
                                Rows="4"
                                TextMode="MultiLine" />
                            <span id="spanLetterPreviewError" class="field-error"></span>
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

                        <!-- Notice Days -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">Notice Days<span class="required-asterisk">*</span></label>
                            <asp:TextBox ID="txtShowCauseNoticeDays"
                                runat="server"
                                CssClass="form-control"
                                Text="15"
                                placeholder="Enter notice days"
                                onkeypress="return allowDigitsOnly(event);"
                                onpaste="return blockNonDigitPaste(event);"
                                oninput="sanitizeDigitsOnly(this); calculateResponseDeadline();"
                                onchange="calculateResponseDeadline();" />
                            <span id="spanShowCauseNoticeDaysError" class="field-error"></span>
                        </div>

                        <!-- 📅 Response Deadline (same date picker UI) -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">Response Deadline<span class="required-asterisk">*</span></label>
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
                            <span id="spanResponseDeadlineError" class="field-error"></span>
                        </div>

                        <!-- Notice Letter -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">Notice Letter Content<span class="required-asterisk">*</span></label>
                            <asp:TextBox ID="txtNoticeLetter"
                                runat="server"
                                CssClass="form-control"
                                Rows="4"
                                TextMode="MultiLine" />
                            <span id="spanNoticeLetterError" class="field-error"></span>
                        </div>
                        <!-- Escalate Button -->
                        <div id="showCauseButtonsRow" class="d-flex justify-content-end gap-2 mt-3">
                            <asp:Button ID="btnSendShowCause"
                                runat="server"
                                Text="Send Show Cause Notice"
                                CssClass="btn btn-danger"
                                OnClientClick="return validateShowCauseForm();"
                                OnClick="btnSendShowCause_Click" />

                            <asp:Button ID="btnRemoveTermination"
                                runat="server"
                                Text="Remove Termination"
                                CssClass="btn btn-danger"
                                Visible="false"
                                OnClientClick="return confirmRemoveTermination();"
                                OnClick="btnRemoveTermination_Click" />

                            <asp:Button ID="btnEscalateShowCause"
                                runat="server"
                                Text="Escalate to Termination"
                                CssClass="btn btn-danger"
                                OnClientClick="return confirmEscalateToTermination();"
                                OnClick="btnConfirmTermination_Click" Visible="true" Enabled="false" />
                        </div>


                    </div>

                    <!-- DIRECT TERMINATE -->
                    <div id="directTerminateSection" style="display: none;">

                        <div class="alert alert-warning">
                            <strong>Direct Termination</strong>
                            <ul class="mb-0 mt-2">
                                <li>Used when no prior performance review or show cause process applies</li>
                                <li>Reason and supporting remarks must be documented</li>
                            </ul>
                        </div>

                        <!-- Termination Reason -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">Termination Reason<span class="required-asterisk">*</span></label>
                            <asp:TextBox ID="txtDirectTerminationReason"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Enter reason for termination"
                                onkeyup="generateDirectTerminationLetter();"
                                onchange="generateDirectTerminationLetter();" />
                            <span id="spanDirectReasonError" class="field-error"></span>
                        </div>

                        <!-- Termination Letter / Remarks -->
                        <div class="mb-3">
                            <label class="form-label fw-bold">Termination Letter / Remarks (if applicable)</label>
                            <asp:TextBox ID="txtDirectTerminationRemarks"
                                runat="server"
                                CssClass="form-control"
                                Rows="4"
                                TextMode="MultiLine" />
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
                        <label class="form-label fw-bold">Termination Date<span class="required-asterisk">*</span></label>
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

                </div>

                <!-- Footer: Direct Terminate (Submit / Cancel) -->
                <div id="terminationModalFooter" class="modal-footer">
                    <asp:Button ID="btnConfirmTermination"
                        runat="server"
                        Text="Submit"
                        CssClass="btn btn-danger"
                        OnClientClick="return validateDirectTerminateForm();"
                        OnClick="btnConfirmTermination_Click" />
                    <button type="button"
                        class="btn btn-secondary"
                        data-bs-dismiss="modal">
                        Cancel
                    </button>
                </div>

                <!-- Footer: Performance Based Letter (Send Termination Notice / Escalate to Termination) -->
                <div id="performanceModalFooter" class="modal-footer" style="display: none;">
                    <asp:Button ID="btnSendTerminationNotice"
                        runat="server"
                        Text="Send Termination Notice"
                        CssClass="btn btn-danger"
                        OnClientClick="return validatePerformanceForm();"
                        OnClick="btnConfirmTermination_Click" />
                    <asp:Button ID="btnRemovePerformanceCap"
                        runat="server"
                        Text="Remove Termination"
                        CssClass="btn btn-danger"
                        Visible="false"
                        OnClientClick="return confirmRemovePerformanceCap();"
                        OnClick="btnRemovePerformanceCap_Click" />
                    <asp:Button ID="btnEscalateToTerminationPerf"
                        runat="server"
                        Text="Escalate to Termination"
                        CssClass="btn btn-danger"
                        OnClientClick="return validatePerformanceForm();"
                        OnClick="btnConfirmTermination_Click"
                        Visible="true"
                        Enabled="false" />
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
        <div class="ms-auto">
            <a href="~/View/Modules/TerminationHistoryList.aspx" runat="server" class="btn btn-secondary">
                <i class="fa fa-history"></i> History
            </a>
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
                                    <%--<asp:BoundField DataField="Username" HeaderText="Username"  />--%>
                                    <asp:TemplateField HeaderText="Employee Name">
                                        <ItemTemplate>
                                            <div class="emp-name-cell">
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
                                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="220px">
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

                                            <div class="term-action-cell">

                                                <div class="term-action-row">
                                                    <asp:LinkButton
                                                        ID="lnkTerminate"
                                                        runat="server"
                                                        CssClass="btn-terminate"
                                                        ToolTip="Terminate Employee"
                                                        CommandName="SelectEmployee"
                                                        CommandArgument='<%# Container.DataItemIndex %>'
                                                        Visible='<%# ShouldShowActionButton(Eval("notice_status")) %>'>
    <i class="fa fa-user-times"></i> <%# GetTerminateActionLabel(Eval("notice_status")) %>
                                                    </asp:LinkButton>

                                                    <span class="btn-terminated"
                                                        style='<%# (Eval("notice_status") != null && Eval("notice_status").ToString() == "Terminated") ? "" : "display:none;" %>'>
    <i class="fa fa-user-times"></i> Terminated
                                                    </span>
                                                </div>

                                                <div class="term-action-row">
                                                    <span class="btn-cap-badge"
                                                        style='<%# string.IsNullOrEmpty(GetCapBadgeText(Eval("notice_status"))) ? "display:none;" : "" %>'>
    <%# GetCapBadgeText(Eval("notice_status")) %>
                                                    </span>

                                                    <asp:HyperLink
                                                        ID="lnkView"
                                                        runat="server"
                                                        CssClass="btn-view"
                                                        ToolTip="View Termination Details"
                                                        NavigateUrl='<%# "~/View/Modules/TerminationDetailsView.aspx?user_id=" + Eval("UserId") %>'
                                                        Visible='<%# Eval("notice_status") != null && Eval("notice_status").ToString() != "" && Eval("notice_status").ToString() != "None" %>'>
    <i class="fa fa-eye"></i> View
                                                    </asp:HyperLink>
                                                </div>

                                            </div>

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

        function confirmRemoveTermination() {
            Swal.fire({
                icon: 'warning',
                title: 'Remove Termination',
                text: 'Are you sure you want to remove the termination?',
                showCancelButton: true,
                confirmButtonText: 'Yes, remove it',
                cancelButtonText: 'Cancel',
                confirmButtonColor: '#dc3545'
            }).then(function (result) {
                if (result.isConfirmed) {
                    __doPostBack('<%= btnRemoveTermination.UniqueID %>', '');
                }
            });

            // Always block the button's own synchronous postback - the popup
            // above decides asynchronously whether to trigger it manually.
            return false;
        }

        function confirmRemovePerformanceCap() {
            Swal.fire({
                icon: 'warning',
                title: 'Remove Termination',
                text: 'Are you sure you want to remove this CAP termination?',
                showCancelButton: true,
                confirmButtonText: 'Yes, remove it',
                cancelButtonText: 'Cancel',
                confirmButtonColor: '#dc3545'
            }).then(function (result) {
                if (result.isConfirmed) {
                    __doPostBack('<%= btnRemovePerformanceCap.UniqueID %>', '');
                }
            });

            return false;
        }

        function confirmEscalateToTermination() {
            // This reuses the Direct Terminate save under the hood, so it
            // must pass that same validation - checked here (Show Cause tab)
            // before the confirm popup even appears, not after.
            if (!validateDirectTerminateForm()) {
                Swal.fire({
                    icon: 'error',
                    title: 'Missing information',
                    text: 'Please switch to the Direct Terminate tab and fill in the required fields (Termination Reason, Termination Date) before escalating.'
                });
                return false;
            }

            Swal.fire({
                icon: 'warning',
                title: 'Escalate to Termination',
                text: 'This will terminate the employee. Are you sure you want to proceed?',
                showCancelButton: true,
                confirmButtonText: 'Yes, terminate',
                cancelButtonText: 'Cancel',
                confirmButtonColor: '#dc3545'
            }).then(function (result) {
                if (result.isConfirmed) {
                    // Reuse the exact same save as the Direct Terminate tab's
                    // Submit button - switch to that type first so the save
                    // actually terminates instead of resaving a pending
                    // Show Cause Notice. The Direct Terminate Reason field
                    // must already be filled in for validation to pass.
                    document.getElementById('<%= hfTerminationType.ClientID %>').value = "DirectTerminate";
                    __doPostBack('<%= btnEscalateShowCause.UniqueID %>', '');
                }
            });

            return false;
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
                minDate: "today",
                onChange: function () {
                    var type = document.getElementById('<%= hfTerminationType.ClientID %>').value;
                    if (type === "DirectTerminate") {
                        generateDirectTerminationLetter();
                    } else {
                        generatePerformanceLetter();
                    }
                }
            });

            flatpickr("#<%= txtResponseDeadline.ClientID %>", {
                dateFormat: "d-m-Y",
                allowInput: true,
                minDate: "today",
                onChange: function () {
                    generateShowCauseLetter();
                }
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
        // ===== Termination Date: auto-calculate from Notice Period, but stay editable =====
        // Termination Date = Base Date (today) + Notice Period Days. Recalculated only
        // when Notice Period changes - manual typing/date-picker edits are otherwise
        // left alone.

        function pad2ForDate(n) {
            return n < 10 ? "0" + n : "" + n;
        }

        function formatDateDMY(d) {
            return pad2ForDate(d.getDate()) + "-" + pad2ForDate(d.getMonth() + 1) + "-" + d.getFullYear();
        }

        function regenerateLetterForCurrentType() {
            var typeEl = document.getElementById('<%= hfTerminationType.ClientID %>');
            var type = typeEl ? typeEl.value : "Performance";
            if (type === "DirectTerminate") {
                generateDirectTerminationLetter();
            } else {
                generatePerformanceLetter();
            }
        }

        function setTerminationDateValue(dateObj) {
            var picker = document.getElementById('<%= txtTerminationDate.ClientID %>');
            if (!picker) return;

            if (picker._flatpickr) {
                picker._flatpickr.setDate(dateObj, true);
            } else {
                picker.value = formatDateDMY(dateObj);
            }

            regenerateLetterForCurrentType();
        }

        function calculateTerminationDate() {
            var noticeInput = document.getElementById('<%= txtNoticePeriod.ClientID %>');
            var noticeDays = noticeInput ? parseInt(noticeInput.value, 10) : 0;
            if (isNaN(noticeDays) || noticeDays < 0) noticeDays = 0;

            // Base Date = today. Immediate / 0 Days => Termination Date = Base Date.
            var terminationDate = new Date();
            terminationDate.setDate(terminationDate.getDate() + noticeDays);

            setTerminationDateValue(terminationDate);
        }

        // Response Deadline = today + Notice Days, same pattern as the
        // Performance tab's Termination Date auto-calc. Still fully editable
        // afterward - only recalculated when Notice Days changes.
        function calculateResponseDeadline() {
            var noticeInput = document.getElementById('<%= txtShowCauseNoticeDays.ClientID %>');
            var noticeDays = noticeInput ? parseInt(noticeInput.value, 10) : 0;
            if (isNaN(noticeDays) || noticeDays < 0) noticeDays = 0;

            var deadline = new Date();
            deadline.setDate(deadline.getDate() + noticeDays);

            var picker = document.getElementById('<%= txtResponseDeadline.ClientID %>');
            if (picker) {
                if (picker._flatpickr) {
                    picker._flatpickr.setDate(deadline, true);
                } else {
                    picker.value = formatDateDMY(deadline);
                }
            }

            generateShowCauseLetter();
        }

    </script>

    <script>
        // ===== Numeric-only enforcement for day-count fields =====
        // Blocks non-digit keystrokes outright (no letters, symbols, spaces,
        // decimal point, minus sign), and strips anything that still slips
        // through via paste/autofill/browser number spinners.
        function allowDigitsOnly(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            // Allow navigation/control keys (backspace, tab, arrows, etc. come
            // through as keydown normally, but guard here too since keypress
            // is what's wired up).
            if (charCode < 48 || charCode > 57) {
                evt.preventDefault();
                return false;
            }
            return true;
        }

        function blockNonDigitPaste(evt) {
            var pasted = (evt.clipboardData || window.clipboardData).getData('text');
            if (!/^\d+$/.test(pasted)) {
                evt.preventDefault();
                return false;
            }
            return true;
        }

        function sanitizeDigitsOnly(input) {
            var cleaned = input.value.replace(/[^0-9]/g, '');
            if (cleaned !== input.value) input.value = cleaned;
        }

        // ===== Per-field inline validation =====
        // Each Submit button validates only the fields relevant to its own
        // tab, clears previous errors first, writes messages directly under
        // the offending fields (not a popup), and returns false to block the
        // postback whenever anything fails.

        function showFieldError(spanId, inputEl, message) {
            var span = document.getElementById(spanId);
            if (span) span.textContent = message;
            if (inputEl) inputEl.classList.add('is-invalid-field');
        }

        function clearFieldError(spanId, inputEl) {
            var span = document.getElementById(spanId);
            if (span) span.textContent = '';
            if (inputEl) inputEl.classList.remove('is-invalid-field');
        }

        function isPositiveWholeNumber(text) {
            return /^\d+$/.test((text || '').trim());
        }

        function validatePerformanceForm() {
            var isValid = true;

            var ratingEl = document.getElementById('<%= hfPerformanceRating.ClientID %>');
            clearFieldError('spanPerformanceRatingError', null);
            if (!ratingEl || !ratingEl.value) {
                showFieldError('spanPerformanceRatingError', null, 'Performance Rating is required.');
                isValid = false;
            }

            var noticeEl = document.getElementById('<%= txtNoticePeriod.ClientID %>');
            clearFieldError('spanNoticePeriodError', noticeEl);
            if (!noticeEl || noticeEl.value.trim() === '') {
                showFieldError('spanNoticePeriodError', noticeEl, 'Notice Period (Days) is required.');
                isValid = false;
            } else if (!isPositiveWholeNumber(noticeEl.value)) {
                showFieldError('spanNoticePeriodError', noticeEl, 'Notice Period (Days) must be a whole number (0-9 only).');
                isValid = false;
            }

            var letterEl = document.getElementById('<%= txtLetterPreview.ClientID %>');
            clearFieldError('spanLetterPreviewError', letterEl);
            if (!letterEl || letterEl.value.trim() === '') {
                showFieldError('spanLetterPreviewError', letterEl, 'Termination Letter Preview is required.');
                isValid = false;
            }

            var dateEl = document.getElementById('<%= txtTerminationDate.ClientID %>');
            clearFieldError('spanDateError', dateEl);
            if (!dateEl || dateEl.value.trim() === '') {
                showFieldError('spanDateError', dateEl, 'Termination Date is required.');
                isValid = false;
            }

            return isValid;
        }

        function validateShowCauseForm() {
            var isValid = true;

            var noticeDaysEl = document.getElementById('<%= txtShowCauseNoticeDays.ClientID %>');
            clearFieldError('spanShowCauseNoticeDaysError', noticeDaysEl);
            if (!noticeDaysEl || noticeDaysEl.value.trim() === '') {
                showFieldError('spanShowCauseNoticeDaysError', noticeDaysEl, 'Notice Days is required.');
                isValid = false;
            } else if (!isPositiveWholeNumber(noticeDaysEl.value)) {
                showFieldError('spanShowCauseNoticeDaysError', noticeDaysEl, 'Notice Days must be a whole number (0-9 only).');
                isValid = false;
            }

            var deadlineEl = document.getElementById('<%= txtResponseDeadline.ClientID %>');
            clearFieldError('spanResponseDeadlineError', deadlineEl);
            if (!deadlineEl || deadlineEl.value.trim() === '') {
                showFieldError('spanResponseDeadlineError', deadlineEl, 'Response Deadline is required.');
                isValid = false;
            }

            var noticeLetterEl = document.getElementById('<%= txtNoticeLetter.ClientID %>');
            clearFieldError('spanNoticeLetterError', noticeLetterEl);
            if (!noticeLetterEl || noticeLetterEl.value.trim() === '') {
                showFieldError('spanNoticeLetterError', noticeLetterEl, 'Notice Letter Content is required.');
                isValid = false;
            }

            return isValid;
        }

        function validateDirectTerminateForm() {
            var isValid = true;

            var reasonEl = document.getElementById('<%= txtDirectTerminationReason.ClientID %>');
            clearFieldError('spanDirectReasonError', reasonEl);
            if (!reasonEl || reasonEl.value.trim() === '') {
                showFieldError('spanDirectReasonError', reasonEl, 'Termination Reason is required.');
                isValid = false;
            }

            var dateEl = document.getElementById('<%= txtTerminationDate.ClientID %>');
            clearFieldError('spanDateError', dateEl);
            if (!dateEl || dateEl.value.trim() === '') {
                showFieldError('spanDateError', dateEl, 'Termination Date is required.');
                isValid = false;
            }

            return isValid;
        }
    </script>

    <script>
        // ===== Automatic letter generation =====
        // Builds the letter/notice text from the employee + termination data
        // already on the page, so HR never has to type it manually. Output stays
        // in a normal editable textarea, so HR can still tweak it before submit.

        function getEmployeeNameForLetter() {
            var el = document.getElementById('<%= hfEmployeeName.ClientID %>');
            return (el && el.value) ? el.value : "the employee";
        }

        function getEmployeeCodeForLetter() {
            var el = document.getElementById('<%= hfEmployeeCode.ClientID %>');
            return (el && el.value) ? el.value : "N/A";
        }

        function getTerminationDateForLetter() {
            var el = document.getElementById('<%= txtTerminationDate.ClientID %>');
            return (el && el.value) ? el.value : "[Termination Date]";
        }

        function generatePerformanceLetter() {
            var name = getEmployeeNameForLetter();
            var code = getEmployeeCodeForLetter();
            var rating = document.getElementById('<%= hfPerformanceRating.ClientID %>').value || "Not rated";
            var noticeInput = document.getElementById('<%= txtNoticePeriod.ClientID %>');
            var noticeDaysVal = noticeInput ? parseInt(noticeInput.value, 10) : 0;
            if (isNaN(noticeDaysVal) || noticeDaysVal < 0) noticeDaysVal = 0;
            var noticeText = noticeDaysVal === 0 ? "Immediate" : (noticeDaysVal + " Days");
            var termDate = getTerminationDateForLetter();

            var stageEl = document.getElementById('<%= hfCapStage.ClientID %>');
            var stage = stageEl ? stageEl.value : "1";

            var letter;
            if (stage === "1" || stage === "2") {
                var capLabel = stage === "1" ? "CAP 1 (Corrective Action Plan - Round 1)" : "CAP 2 (Corrective Action Plan - Round 2, Final Warning)";
                letter =
                    "Dear " + name + " (Employee Code: " + code + "),\n\n" +
                    "This letter is issued under " + capLabel + " following a review of your performance.\n\n" +
                    "Performance Rating: " + rating + " / 5\n" +
                    "Notice Period (if this is not resolved): " + noticeText + "\n" +
                    "Target Date for Improvement: " + termDate + "\n\n" +
                    "This has been raised after due consideration of your performance record over the review period, including KPIs/targets not achieved, supported by documentation on file. " +
                    (stage === "2"
                        ? "This is the final corrective action stage - failure to show sustained improvement will result in termination of employment.\n\n"
                        : "You are expected to show sustained improvement during this period. Failure to do so may result in further corrective action, up to and including termination of employment.\n\n") +
                    "Please reach out to HR if you have any questions regarding this notice.\n\n" +
                    "Regards,\nHR Team";
            } else {
                letter =
                    "Dear " + name + " (Employee Code: " + code + "),\n\n" +
                    "This letter is to formally inform you that, following two Corrective Action Plan (CAP) reviews of your performance, your employment with the company is being terminated.\n\n" +
                    "Performance Rating: " + rating + " / 5\n" +
                    "Notice Period: " + noticeText + "\n" +
                    "Effective Termination Date: " + termDate + "\n\n" +
                    "This decision has been taken after due consideration of your performance record over the review period, including KPIs/targets not achieved across both CAP rounds and prior warnings / Performance Improvement Plan (PIP) discussions, all of which are supported by documentation on file.\n\n" +
                    "Please ensure a proper handover of all company assets, documents and pending work before your last working day. HR will contact you separately regarding your full and final settlement.\n\n" +
                    "We wish you well in your future endeavors.\n\n" +
                    "Regards,\nHR Team";
            }

            var box = document.getElementById('<%= txtLetterPreview.ClientID %>');
            if (box) box.value = letter;
        }

        function generateShowCauseLetter() {
            var name = getEmployeeNameForLetter();
            var code = getEmployeeCodeForLetter();
            var deadlineEl = document.getElementById('<%= txtResponseDeadline.ClientID %>');
            var deadline = (deadlineEl && deadlineEl.value) ? deadlineEl.value : "[Response Deadline]";

            var letter =
                "Dear " + name + " (Employee Code: " + code + "),\n\n" +
                "This Show Cause Notice is issued to you in respect of a violation/misconduct that is currently under review.\n\n" +
                "You are hereby directed to submit a written explanation clarifying your position on this matter.\n\n" +
                "Response Deadline: " + deadline + "\n\n" +
                "Please submit your written explanation to HR on or before the above date. Failure to respond within the given timeframe may result in further disciplinary action, up to and including termination of employment.\n\n" +
                "Regards,\nHR Team";

            var box = document.getElementById('<%= txtNoticeLetter.ClientID %>');
            if (box) box.value = letter;
        }

        function generateDirectTerminationLetter() {
            var name = getEmployeeNameForLetter();
            var code = getEmployeeCodeForLetter();
            var reasonEl = document.getElementById('<%= txtDirectTerminationReason.ClientID %>');
            var reason = (reasonEl && reasonEl.value.trim()) ? reasonEl.value.trim() : "[Termination Reason]";
            var termDate = getTerminationDateForLetter();

            var letter =
                "Dear " + name + " (Employee Code: " + code + "),\n\n" +
                "This letter is to formally inform you that your employment with the company is being terminated with immediate effect.\n\n" +
                "Reason for Termination: " + reason + "\n" +
                "Effective Termination Date: " + termDate + "\n\n" +
                "As this is a direct termination, your access to all company systems, premises and assets will be disabled/revoked with effect from the above date. Please hand over all company property and pending work prior to your last working day, and coordinate with HR for your full and final settlement.\n\n" +
                "Regards,\nHR Team";

            var box = document.getElementById('<%= txtDirectTerminationRemarks.ClientID %>');
            if (box) box.value = letter;
        }

        function showPerformanceBased() {
            document.getElementById("performanceSection").style.display = "block";
            document.getElementById("showCauseSection").style.display = "none";
            document.getElementById("directTerminateSection").style.display = "none";
            document.getElementById("terminationDateSection").style.display = "block";
            // Performance tab has its own action buttons (Send Termination
            // Notice / Escalate to Termination) - hide the shared footer.
            document.getElementById("terminationModalFooter").style.display = "none";
            document.getElementById("performanceModalFooter").style.display = "flex";

            // Tab color change
            document.getElementById("tabPerformance").classList.add("active");
            document.getElementById("tabShowCause").classList.remove("active");
            document.getElementById("tabDirectTerminate").classList.remove("active");

            document.getElementById("<%= hfTerminationType.ClientID %>").value = "Performance";

            generatePerformanceLetter();
        }

        function showShowCause() {
            document.getElementById("performanceSection").style.display = "none";
            document.getElementById("showCauseSection").style.display = "block";
            document.getElementById("directTerminateSection").style.display = "none";
            document.getElementById("terminationDateSection").style.display = "none";
            // Show Cause has its own action buttons (Send Show Cause Notice /
            // Escalate to Termination) - hide the other footers.
            document.getElementById("terminationModalFooter").style.display = "none";
            document.getElementById("performanceModalFooter").style.display = "none";

            // Tab color change
            document.getElementById("tabPerformance").classList.remove("active");
            document.getElementById("tabShowCause").classList.add("active");
            document.getElementById("tabDirectTerminate").classList.remove("active");

            document.getElementById("<%= hfTerminationType.ClientID %>").value = "ShowCause";

            generateShowCauseLetter();
        }

        window.addEventListener('DOMContentLoaded', function () {
            calculateResponseDeadline();
        });

        function showDirectTerminate() {
            document.getElementById("performanceSection").style.display = "none";
            document.getElementById("showCauseSection").style.display = "none";
            document.getElementById("directTerminateSection").style.display = "block";
            document.getElementById("terminationDateSection").style.display = "block";
            document.getElementById("terminationModalFooter").style.display = "flex";
            document.getElementById("performanceModalFooter").style.display = "none";

            // Tab color change
            document.getElementById("tabPerformance").classList.remove("active");
            document.getElementById("tabShowCause").classList.remove("active");
            document.getElementById("tabDirectTerminate").classList.add("active");

            document.getElementById("<%= hfTerminationType.ClientID %>").value = "DirectTerminate";

            generateDirectTerminationLetter();
        }

        function setRating(val) {
            var hf = document.getElementById('<%= hfPerformanceRating.ClientID %>');
            if (hf) hf.value = val;

            document.querySelectorAll(".star").forEach((s, i) => {
                s.classList.toggle("active", i < val);
            });

            generatePerformanceLetter();
        }

        window.addEventListener('DOMContentLoaded', function () {
            // Default = Performance
            document.getElementById("terminationDateSection").style.display = "block";
            document.getElementById("terminationModalFooter").style.display = "none";
            document.getElementById("performanceModalFooter").style.display = "flex";
            document.getElementById("<%= hfTerminationType.ClientID %>").value = "Performance";
            calculateTerminationDate();
        });
    </script>

</asp:Content>
