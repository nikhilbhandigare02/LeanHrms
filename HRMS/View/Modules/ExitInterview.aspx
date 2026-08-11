<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="ExitInterview.aspx.cs" Inherits="Lean.View.Modules.ExitInterview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../assets/js/commonFunctions.js"></script>
    <script type="text/javascript">
        function isVirtualModeSelected() {
            var ddlInterviewMode = document.getElementById('<%= ddlInterviewMode.ClientID %>');
            if (!ddlInterviewMode || ddlInterviewMode.selectedIndex < 0) {
                return false;
            }
            var selectedText = (ddlInterviewMode.options[ddlInterviewMode.selectedIndex].text || '').trim().toLowerCase();
            // The live "Interview Mode" lookup data has a typo ("Vertual" instead of
            // "Virtual") - match both so this keeps working if the typo gets fixed later.
            return selectedText === 'virtual' || selectedText === 'vertual';
        }

        function toggleLocationField() {
            var divLocation = document.getElementById('<%= divLocation.ClientID %>');
            var txtLocation = document.getElementById('<%= txtLocation.ClientID %>');
            if (!divLocation) {
                return;
            }

            if (isVirtualModeSelected()) {
                divLocation.style.display = 'block';
            } else {
                divLocation.style.display = 'none';
                if (txtLocation) {
                    txtLocation.value = '';
                }
            }
        }

        function validateLocation(source, args) {
            if (!isVirtualModeSelected()) {
                args.IsValid = true;
                return;
            }
            args.IsValid = args.Value !== null && args.Value.trim().length > 0;
        }

        // Stops the date picker itself from offering past dates when scheduling a NEW
        // interview - cvInterviewDate_ServerValidate already rejects a past date on
        // Save, this just prevents picking one in the first place. Skipped for an
        // existing record being edited, which may legitimately already be in the past.
        function initInterviewDateMin() {
            var hdnId = document.getElementById('<%= hdnExitInterviewId.ClientID %>');
            var txtDate = document.getElementById('<%= txtInterviewDate.ClientID %>');
            if (!txtDate) {
                return;
            }

            var isExistingRecord = hdnId && hdnId.value && hdnId.value !== '0';
            if (isExistingRecord) {
                txtDate.removeAttribute('min');
                return;
            }

            var today = new Date();
            var yyyy = today.getFullYear();
            var mm = String(today.getMonth() + 1).padStart(2, '0');
            var dd = String(today.getDate()).padStart(2, '0');
            txtDate.min = yyyy + '-' + mm + '-' + dd;
        }

        // Stops the time picker itself from offering a past time when the selected
        // Interview Date is today - cvInterviewTime_ServerValidate already rejects
        // this combination on Save, this just prevents picking it in the first place.
        // If the date isn't today (or the interview is an existing record), any time
        // is allowed, so any earlier min restriction is cleared.
        function updateInterviewTimeMin() {
            var hdnId = document.getElementById('<%= hdnExitInterviewId.ClientID %>');
            var txtDate = document.getElementById('<%= txtInterviewDate.ClientID %>');
            var txtTime = document.getElementById('<%= txtInterviewTime.ClientID %>');
            if (!txtDate || !txtTime) {
                return;
            }

            var isExistingRecord = hdnId && hdnId.value && hdnId.value !== '0';
            if (isExistingRecord) {
                txtTime.removeAttribute('min');
                return;
            }

            var now = new Date();
            var todayStr = now.getFullYear() + '-' + String(now.getMonth() + 1).padStart(2, '0') + '-' + String(now.getDate()).padStart(2, '0');

            if (txtDate.value === todayStr) {
                txtTime.min = String(now.getHours()).padStart(2, '0') + ':' + String(now.getMinutes()).padStart(2, '0');
            } else {
                txtTime.removeAttribute('min');
            }
        }

        // Call on initial page load AND after every async (UpdatePanel) postback -
        // $(document).ready only fires once, but Edit/Schedule/View are all
        // triggered via async postbacks that don't raise a new DOM ready event.
        function initLocationToggle() {
            toggleLocationField();
            initInterviewDateMin();
            updateInterviewTimeMin();
        }

        $(document).ready(function () {
            initLocationToggle();

            if (window.Sys && window.Sys.Application) {
                Sys.Application.add_load(initLocationToggle);
            }
        });
    </script>
    <style>
        .gridview-pagination a {
            margin-right: 10px;
        }
    </style>
    <style>
        .small-font {
            font-size: 14px;
        }

        .page-container {
            margin-left: 20px;
            margin-right: 20px;
        }

        .pagination-container {
            margin-top: 20px;
            padding: 10px;
        }

        .app-search .position-relative {
            display: flex;
        }

        .app-search input {
            border-radius: 0;
        }

        .app-search .btn {
            border-radius: 0;
        }

        .app-search .input-group-append {
            position: absolute;
            right: 0;
            top: 0;
            bottom: 0;
            display: flex;
            align-items: center;
        }
    </style>
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
        .highlighted {
            background-color: rgba(0, 0, 0, 0.5);
            color: #ffffff;
        }
    </style>
    <style>
        .view-link {
            color: grey;
        }

        .edit-link {
            color: blue;
        }

        .delete-link {
            color: red;
        }
    </style>
    <style>
        .pagination-container {
            display: flex;
            align-items: center;
        }

            .pagination-container button,
            .pagination-container select {
                margin: 0;
            }

        .custom-gridview {
            border-left: none;
            border-right: none;
        }

            .custom-gridview th,
            .custom-gridview td {
                border-left: none;
                border-right: none;
            }
    </style>
    <style>
        .date-field {
            width: 100%;
            cursor: pointer;
            position: relative;
        }

        input[type="date"]::-webkit-calendar-picker-indicator {
            background: transparent;
            bottom: 0;
            color: transparent;
            cursor: pointer;
            height: auto;
            left: 0;
            position: absolute;
            right: 0;
            top: 0;
            width: auto;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-body">
                    <h2 class="card-title mb-4 d-flex justify-content-between align-items-center" style="font-size: 22px;">&nbsp;Exit Interview Management
                        <div class="d-flex justify-content-end align-items-center">
                            <div class="app-search d-none d-lg-block" id="searchdata" runat="server">
                                <div class="position-relative">
                                    <input type="text" class="form-control" id="searchInput" placeholder="Search..." onkeydown="searchOnEnter(event)">
                                    <span class="bx bx-search-alt"></span>
                                </div>
                            </div>
                            <button runat="server" id="btn_addExitInterview" onserverclick="btn_addExitInterview_ServerClick" class="btn btn-primary ms-2 me-2" title="Schedule Exit Interview">
                                <i class="fas fa-calendar-plus"></i>&nbsp;Schedule Interview
                            </button>
                            <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary" Text="Back" Visible="true" OnClick="btnBack_Click" />
                        </div>
                    </h2>

                    <!-- Exit Interview Form Panel -->
                    <asp:UpdatePanel ID="UpdatePanelForm" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:HiddenField ID="hdnExitInterviewId" runat="server" />
                            <asp:Panel ID="pnlExitInterviewForm" runat="server" Visible="false">
                                <div class="card mb-3">
                                    <div class="card-body">
                                        <h5 class="card-title" id="formTitle" runat="server">Schedule Exit Interview</h5>
                                        <hr />
                                <div class="row">
                                    <div class="col-lg-6">
                                        <div class="form-group mb-3">
                                            <label for="ddlEmployee">Employee <span class="text-danger">*</span></label>
                                            <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-control custom-dropdown">
                                                <asp:ListItem Text="Select Employee" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvEmployee" runat="server" ControlToValidate="ddlEmployee" 
                                                InitialValue="" ErrorMessage="Please select employee" CssClass="text-danger" Display="Dynamic" />
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-group mb-3">
                                            <label for="ddlInterviewer">Interviewer <span class="text-danger">*</span></label>
                                            <asp:DropDownList ID="ddlInterviewer" runat="server" CssClass="form-control custom-dropdown">
                                                <asp:ListItem Text="Select Interviewer" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvInterviewer" runat="server" ControlToValidate="ddlInterviewer" 
                                                InitialValue="" ErrorMessage="Please select interviewer" CssClass="text-danger" Display="Dynamic" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6">
                                        <div class="form-group mb-3 date-field">
                                            <label for="txtInterviewDate">Interview Date <span class="text-danger">*</span></label>
                                            <asp:TextBox ID="txtInterviewDate" CssClass="form-control date-field" TextMode="Date" runat="server"
                                                onchange="updateInterviewTimeMin();"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvInterviewDate" runat="server" ControlToValidate="txtInterviewDate" 
                                                ErrorMessage="Please select interview date" CssClass="text-danger" Display="Dynamic" />
                                            <asp:CustomValidator ID="cvInterviewDate" runat="server" ControlToValidate="txtInterviewDate"
                                                ErrorMessage="Interview date must be a future date" CssClass="text-danger" Display="Dynamic"
                                                OnServerValidate="cvInterviewDate_ServerValidate" />
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-group mb-3">
                                            <label for="txtInterviewTime">Interview Time <span class="text-danger">*</span></label>
                                            <asp:TextBox ID="txtInterviewTime" CssClass="form-control" TextMode="Time" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvInterviewTime" runat="server" ControlToValidate="txtInterviewTime"
                                                ErrorMessage="Please select interview time" CssClass="text-danger" Display="Dynamic" />
                                            <asp:CustomValidator ID="cvInterviewTime" runat="server" ControlToValidate="txtInterviewTime"
                                                ErrorMessage="Please select a current or future time for today's interview." CssClass="text-danger" Display="Dynamic"
                                                OnServerValidate="cvInterviewTime_ServerValidate" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-6">
                                        <div class="form-group mb-3">
                                            <label for="ddlInterviewStatus">Interview Status <span class="text-danger">*</span></label>
                                            <asp:DropDownList ID="ddlInterviewStatus" runat="server" CssClass="form-control custom-dropdown">
                                                <asp:ListItem Text="Select Status" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvInterviewStatus" runat="server" ControlToValidate="ddlInterviewStatus" 
                                                InitialValue="" ErrorMessage="Please select interview status" CssClass="text-danger" Display="Dynamic" />
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-group mb-3">
                                            <label for="ddlInterviewMode">Interview Mode <span class="text-danger">*</span></label>
                                            <asp:DropDownList ID="ddlInterviewMode" runat="server" CssClass="form-control custom-dropdown" onchange="toggleLocationField()">
                                                <asp:ListItem Text="Select Mode" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvInterviewMode" runat="server" ControlToValidate="ddlInterviewMode"
                                                InitialValue="" ErrorMessage="Please select interview mode" CssClass="text-danger" Display="Dynamic" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row" id="divLocation" runat="server" style="display:none;">
                                    <div class="col-lg-12">
                                        <div class="form-group mb-3">
                                            <label for="txtLocation">Location <span id="spnLocationRequired" runat="server" class="text-danger">*</span></label>
                                            <asp:TextBox ID="txtLocation" CssClass="form-control" runat="server" placeholder="Enter interview location"></asp:TextBox>
                                            <asp:CustomValidator ID="cvLocation" runat="server" ControlToValidate="txtLocation"
                                                ErrorMessage="Location is required for Offline interviews" CssClass="text-danger" Display="Dynamic"
                                                ClientValidationFunction="validateLocation" OnServerValidate="cvLocation_ServerValidate" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="form-group mb-3">
                                            <label for="txtNotes">Notes / Comments</label>
                                            <asp:TextBox ID="txtNotes" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="3" placeholder="Enter any additional notes"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-12">
                                        <div class="d-flex justify-content-end">
                                            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSave_Click" />
                                            <asp:Button ID="btnCancel" runat="server" CssClass="btn btn-secondary ms-2" Text="Cancel" OnClick="btnCancel_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                    <br />
                    <asp:Panel ID="pnlGridSection" runat="server" Visible="true">
                    <div class="table-responsive">
                        <asp:HiddenField ID="hfPageIndex" runat="server" />
                        <asp:UpdatePanel ID="UpdatePanelGrid" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:GridView runat="server" ID="gvExitInterviews" class="table custom-gridview" AutoGenerateColumns="false"
                                    DataKeyNames="ExitInterviewId" OnRowCommand="gvExitInterviews_RowCommand" EnablePersistedSelection="true"
                                    OnPageIndexChanging="OnPageIndexChanging" PageSize="10" AllowSorting="true" OnSorting="gvExitInterviews_Sorting"
                                    Style="margin: 0 auto;" EmptyDataText="No exit interviews found.">
                                    <Columns>
                                        <asp:TemplateField HeaderText="SR No" ItemStyle-Width="80px">
                                            <ItemTemplate>
                                                <%# GetSerialNumber(Container.DataItemIndex) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="EmployeeName" HeaderText="Employee Name" ItemStyle-Width="150px" />
                                        <asp:BoundField DataField="InterviewerName" HeaderText="Interviewer" ItemStyle-Width="150px" />
                                        <asp:BoundField DataField="InterviewDate" HeaderText="Interview Date" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" ItemStyle-Width="120px" />
                                        <asp:BoundField DataField="InterviewTime" HeaderText="Time" ItemStyle-Width="80px" />
                                        <asp:BoundField DataField="Status" HeaderText="Status" ItemStyle-Width="100px" />
                                        <asp:BoundField DataField="Mode" HeaderText="Mode" ItemStyle-Width="100px" />
                                        <asp:TemplateField HeaderText="Action" ItemStyle-Width="100px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkView" runat="server" CommandName="ViewInterview" title="View Interview"
                                                    CommandArgument='<%# Eval("ExitInterviewId") %>' CausesValidation="false">
                                                    <i class="fa fa-eye"></i>
                                                </asp:LinkButton>
                                                &nbsp;
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditInterview" title="Edit Interview"
                                                    CommandArgument='<%# Eval("ExitInterviewId") %>' CausesValidation="false">
                                                    <i class="fa fa-edit"></i>
                                                </asp:LinkButton>
                                                &nbsp;
                                                <asp:LinkButton ID="lnkDelete" runat="server" CommandName="DeleteInterview" title="Delete Interview"
                                                    CommandArgument='<%# Eval("ExitInterviewId") %>' CausesValidation="false">
                                                    <i class="fa fa-trash"></i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="gridview-pagination" />
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div class="pagination-container" style="font-size: 14px; color: black;">
                            <asp:DropDownList runat="server" ID="ddlPageSelector" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSelector_SelectedIndexChanged"
                                Style="background-color: white; color: black; border: 1px solid #ddd; padding: 5px 10px; margin: 2px; margin-left: auto;">
                            </asp:DropDownList>
                        </div>
                    </div>
                    </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gvExitInterviews" EventName="RowCommand" />
                    </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script>
        function showMessage(status, remark) {
            Swal.fire({
                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }
    </script>
    <script type="text/javascript">
        // Freeze the Save/Update button and show "Submitting..." for the duration of
        // the async postback, so the user can't double-click while waiting on the
        // backend response (SaveExitInterview/UpdateExitInterview can take a few
        // seconds since they also send an email). The button's real text ("Save" vs
        // "Update") and enabled state come back correctly on their own once the
        // server response re-renders the panel, but we restore them defensively in
        // endRequest too, in case the panel isn't the one that ends up refreshed.
        (function () {
            var saveButtonId = '<%= btnSave.ClientID %>';

            function setButtonBusy(btn, busy) {
                if (!btn) {
                    return;
                }
                if (busy) {
                    btn.dataset.originalText = btn.value;
                    btn.disabled = true;
                    btn.value = 'Submitting...';
                } else {
                    btn.disabled = false;
                    if (btn.dataset.originalText) {
                        btn.value = btn.dataset.originalText;
                    }
                }
            }

            Sys.Application.add_load(function () {
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                if (!prm || prm._exitInterviewSaveGuardAttached) {
                    return;
                }
                prm._exitInterviewSaveGuardAttached = true;

                prm.add_beginRequest(function (sender, args) {
                    var postBackElement = args.get_postBackElement();
                    if (postBackElement && postBackElement.id === saveButtonId) {
                        setButtonBusy(document.getElementById(saveButtonId), true);
                    }
                });

                prm.add_endRequest(function () {
                    setButtonBusy(document.getElementById(saveButtonId), false);
                });
            });
        })();
    </script>
    <script type="text/javascript">
        function initializeSearch() {
            $(document).on('input', '#searchInput', function () {
                var searchTerm = $(this).val().toLowerCase();
                filterGrid(searchTerm);
            });
            $(document).on('keydown', '#searchInput', searchOnEnter);
        }

        function filterGrid(searchTerm) {
            $('#<%= gvExitInterviews.ClientID %> tr:has(td)').hide();

            if (searchTerm === '') {
                $('#<%= gvExitInterviews.ClientID %> tr:has(td)').show();
            } else {
                $('#<%= gvExitInterviews.ClientID %> tr:has(td)').filter(function () {
                    var found = false;
                    $(this).find('td').each(function (index) {
                        var cellText = $(this).text().toLowerCase();
                        if (cellText.includes(searchTerm)) {
                            found = true;
                            return false;
                        }
                    });
                    return found;
                }).show();
            }
        }

        function searchOnEnter(event) {
            if (event.key === 'Enter') {
                event.preventDefault();
                var searchTerm = $('#searchInput').val().toLowerCase();
                filterGrid(searchTerm);
            }
        }

        $(document).ready(function () {
            initializeSearch();
        });
    </script>
</asp:Content>
