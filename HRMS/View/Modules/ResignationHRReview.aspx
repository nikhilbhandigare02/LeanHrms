<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="ResignationHRReview.aspx.cs" Inherits="HRMS.View.Modules.ResignationHRReview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <style>
        .hr-review-page .card {
            border: 0;
            border-radius: 14px;
            box-shadow: 0 10px 28px rgba(31, 45, 61, 0.08);
        }

        .hr-review-page .page-title {
            font-size: 1.6em;
            font-weight: bold;
            color: #1f2d3d;
        }

        .hr-review-page .hr-steps {
            display: flex;
            flex-wrap: wrap;
            justify-content: space-around;
            gap: 10px;
            margin: 18px 0 28px 0;
        }

        .hr-review-page .hr-step {
            flex: 1 1 120px;
            text-align: center;
            padding: 10px 8px;
            border-radius: 8px;
            font-size: 13px;
            color: #64748b;
        }

        .hr-review-page .hr-step.active {
            border: 1px solid #1f2d3d;
            color: #1f2d3d;
            font-weight: 600;
        }

        .hr-review-page .info-label {
            font-size: 12px;
            color: #94a3b8;
            text-transform: uppercase;
            letter-spacing: 0.4px;
            margin-bottom: 2px;
        }

        .hr-review-page .info-value {
            font-size: 14px;
            color: #1f2d3d;
            font-weight: 600;
            margin-bottom: 14px;
            display: block;
            word-wrap: break-word;
        }

        .hr-review-page label.field-label {
            font-weight: 600;
            color: #334155;
            margin-bottom: 6px;
            display: block;
        }

        .hr-review-page .hint-text {
            font-size: 13px;
            color: #64748b;
            margin: 18px 0;
        }

        .hr-review-page .btn-accept {
            background: linear-gradient(135deg, #16a34a, #15803d);
            border: 0;
            color: #fff;
            border-radius: 8px;
            font-weight: 600;
            padding: 10px 22px;
        }

        .hr-review-page .btn-cancel {
            background: #64748b;
            border: 0;
            color: #fff;
            border-radius: 8px;
            font-weight: 600;
            padding: 10px 22px;
        }
    </style>
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="hr-review-page">
        <asp:HiddenField ID="hfResignationId" runat="server" />
        <asp:HiddenField ID="hfHRReviewId" runat="server" />

        <div class="row">
            <div class="col-lg-12">
                <div class="card">
                    <div class="card-body">

                       <%-- <div class="hr-steps">
                            <div class="hr-step active">HR review</div>
                            <div class="hr-step">Notice</div>
                            <div class="hr-step">Assets</div>
                            <div class="hr-step">Interview</div>
                        </div>--%>

                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <span class="page-title">HR Review &amp; Acceptance</span>
                            <div class="d-flex align-items-center gap-2">
                                <asp:Label ID="lblReviewStatus" runat="server" CssClass="badge bg-warning text-dark" Text="HR review pending" />
                                <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary" Text="Back"
                                    CausesValidation="false" OnClick="btnBack_Click" />
                            </div>
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
                                <div class="info-label">Reporting Manager</div>
                                <asp:Label ID="lblReportingManager" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Date of Joining</div>
                                <asp:Label ID="lblDateOfJoining" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Resignation Date</div>
                                <asp:Label ID="lblResignationDate" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-3 col-sm-6">
                                <div class="info-label">Proposed Last Working Date</div>
                                <asp:Label ID="lblProposedLastWorkingDate" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-12">
                                <div class="info-label">Reason</div>
                                <asp:Label ID="lblReason" runat="server" CssClass="info-value" Text="-" />
                            </div>
                            <div class="col-md-12" id="divManagerRemark" runat="server" visible="false">
                                <div class="info-label">Remarks</div>
                                <asp:Label ID="lblManagerRemark" runat="server" CssClass="info-value" Text="-" />
                            </div>
                        </div>

                        <hr />

                        <h5 class="mb-3">HR review &amp; acceptance</h5>

                        <!-- HR Review Section -->
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="field-label">Notice Period Required</label>
                                <asp:DropDownList ID="ddlNoticePeriodRequired" runat="server" CssClass="form-control custom-dropdown">
                                    <asp:ListItem Text="-- Select --" Value="" />
                                    <asp:ListItem Text="Yes" Value="Yes" />
                                    <asp:ListItem Text="No" Value="No" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvNoticePeriodRequired" runat="server" ControlToValidate="ddlNoticePeriodRequired"
                                    InitialValue="" ErrorMessage="Please select Notice Period Required" CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="HRReviewGroup" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="field-label">Notice Days</label>
                                <asp:TextBox ID="txtNoticeDays" runat="server" CssClass="form-control" TextMode="Number"
                                    onchange="recalculateRevisedLastWorkingDate();" oninput="recalculateRevisedLastWorkingDate();" />
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="field-label">Notice Start Date <span class="text-danger">*</span></label>
                                <asp:TextBox ID="txtNoticeStartDate" runat="server" CssClass="form-control" TextMode="Date"
                                    onchange="recalculateRevisedLastWorkingDate();" />
                                <asp:RequiredFieldValidator ID="rfvNoticeStartDate" runat="server" ControlToValidate="txtNoticeStartDate"
                                    ErrorMessage="Please select Notice Start Date" CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="HRReviewGroup" />
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="field-label">Revised Last Working Date</label>
                                <asp:TextBox ID="txtRevisedLastWorkingDate" runat="server"
                                    CssClass="form-control" autocomplete="off" ReadOnly="true"
                                    placeholder="Auto-calculated from Resignation Date + Notice Days" />
                                <asp:RequiredFieldValidator ID="rfvRevisedLastWorkingDate" runat="server" ControlToValidate="txtRevisedLastWorkingDate"
                                    ErrorMessage="Please select Revised Last Working Date" CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="HRReviewGroup" />
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="field-label">Buyout Applicable</label>
                                <asp:DropDownList ID="ddlBuyoutApplicable" runat="server" CssClass="form-control custom-dropdown">
                                    <asp:ListItem Text="-- Select --" Value="" />
                                    <asp:ListItem Text="Yes" Value="Yes" />
                                    <asp:ListItem Text="No" Value="No" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvBuyoutApplicable" runat="server" ControlToValidate="ddlBuyoutApplicable"
                                    InitialValue="" ErrorMessage="Please select Buyout Applicable" CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="HRReviewGroup" />
                            </div>

                            <div class="col-md-12 mb-3">
                                <label class="field-label">HR Remarks</label>
                                <asp:TextBox ID="txtHRRemarks" runat="server"
                                    CssClass="form-control" TextMode="MultiLine" Rows="3"
                                    placeholder="Required" />
                                <asp:RequiredFieldValidator ID="rfvHRRemarks" runat="server" ControlToValidate="txtHRRemarks"
                                    ErrorMessage="HR Remarks are mandatory" CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="HRReviewGroup" />
                            </div>
                        </div>

                        <div class="hint-text">
                            On acceptance: generates resignation acceptance letter and notifies employee and manager.
                        </div>

                        <div class="d-flex gap-2">
                            <asp:Button ID="btnAcceptResignation" runat="server"
                                CssClass="btn-accept" Text="Accept Resignation"
                                ValidationGroup="HRReviewGroup"
                                OnClientClick="return freezeAcceptButtonIfValid();"
                                OnClick="btnAcceptResignation_Click" />
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
        function showUserSavedMessage(status, remark) {
            Swal.fire({
                icon: status.toLowerCase() === "success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }

        function showHRReviewResult(status, message, redirectUrl) {
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

        function recalculateRevisedLastWorkingDate() {
            var noticeStartDateText = document.getElementById('<%= txtNoticeStartDate.ClientID %>').value;
            var noticeDaysText = document.getElementById('<%= txtNoticeDays.ClientID %>').value;
            var revisedField = document.getElementById('<%= txtRevisedLastWorkingDate.ClientID %>');

            if (!revisedField) {
                return;
            }

            var noticeDays = parseInt(noticeDaysText, 10);

            if (!noticeStartDateText || noticeDaysText.trim() === '' || isNaN(noticeDays) || noticeDays < 0) {
                // No Notice Start Date selected, or Notice Days is empty/invalid -
                // fall back to the Proposed Last Working Date instead of calculating.
                var proposedLabel = document.getElementById('<%= lblProposedLastWorkingDate.ClientID %>');
                revisedField.value = proposedLabel ? proposedLabel.textContent.trim() : '';
                return;
            }

            // Revised Last Working Date = Notice Start Date + Notice Days.
            // noticeStartDateText is "yyyy-MM-dd" (native date input format) - parse the
            // parts explicitly rather than `new Date(text)`, which JS treats as UTC and
            // can shift a day in negative-UTC-offset time zones.
            var parts = noticeStartDateText.split('-');
            var noticeStartDate = new Date(parseInt(parts[0], 10), parseInt(parts[1], 10) - 1, parseInt(parts[2], 10));
            noticeStartDate.setDate(noticeStartDate.getDate() + noticeDays);

            var dd = String(noticeStartDate.getDate()).padStart(2, '0');
            var mm = String(noticeStartDate.getMonth() + 1).padStart(2, '0');
            var yyyy = noticeStartDate.getFullYear();

            revisedField.value = dd + '-' + mm + '-' + yyyy;
        }

        // Freeze the Accept/Update button and show "Updating..." for the duration of
        // the postback, so the user can't double-click while waiting on the backend
        // response. This is a full (non-AJAX) postback, so the button is disabled via
        // setTimeout - deferring it to the next tick lets the browser's own form
        // submission (already triggered by this same click) go through first with the
        // button's value included, rather than risk the disabled button being dropped
        // from the submitted form data. Field validation is handled by the
        // RequiredFieldValidators (ValidationGroup "HRReviewGroup") - Page_ClientValidate
        // is called explicitly here so the button is only frozen when they all pass.
        function freezeAcceptButtonIfValid() {
            if (typeof (Page_ClientValidate) === 'function' && !Page_ClientValidate('HRReviewGroup')) {
                return false;
            }

            setTimeout(function () {
                var btn = document.getElementById('<%= btnAcceptResignation.ClientID %>');
                if (btn) {
                    btn.disabled = true;
                    btn.value = 'Updating...';
                }
            }, 0);

            return true;
        }

    </script>
</asp:Content>
