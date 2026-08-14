<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AddFullandFinal.aspx.cs" Inherits="HRMS.View.Modules.AddFullandFinal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <style>
        .fnf {
            --fnf-primary: #2563EB;
            --fnf-primary-dark: #1D4ED8;
            --fnf-heading: #1E293B;
            --fnf-muted: #64748B;
            --fnf-line: #E8EEF7;
            --fnf-panel: #F8FAFC;
        }

        .fnf .fnf-card {
            background: #FFFFFF;
            border: 1px solid var(--fnf-line);
            border-radius: 12px;
            box-shadow: 0 10px 30px rgba(15, 23, 42, 0.05);
            padding: 28px 30px;
        }

        .fnf .fnf-title {
            color: var(--fnf-heading);
            font-size: 22px;
            font-weight: 800;
            margin-bottom: 22px;
        }

        .fnf .fnf-form {
            background: var(--fnf-panel);
            border: 1px solid var(--fnf-line);
            border-radius: 10px;
            padding: 20px;
        }

        .fnf .fnf-label {
            color: var(--fnf-heading);
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .fnf .fnf-form .form-control,
        .fnf .fnf-form .form-select {
            border: 1px solid #D9E2EF;
            border-radius: 8px;
        }

        .fnf .fnf-hint {
            color: var(--fnf-muted);
            font-size: 12.5px;
            margin-top: 6px;
        }

        .fnf .fnf-attachments {
            background: #FFFFFF;
            border: 1px dashed #C9D6EA;
            border-radius: 8px;
            padding: 12px 14px;
            font-size: 13px;
            color: var(--fnf-heading);
        }

        .fnf .fnf-attachments ul {
            margin: 6px 0 0 18px;
            padding: 0;
        }

        .fnf .fnf-btn {
            background: var(--fnf-primary);
            border: none;
            border-radius: 8px;
            color: #FFFFFF;
            font-weight: 600;
            height: 42px;
            min-width: 160px;
            transition: background-color .18s ease, box-shadow .18s ease, transform .18s ease;
        }

        .fnf .fnf-btn:hover {
            background: var(--fnf-primary-dark);
            box-shadow: 0 10px 20px rgba(37, 99, 235, 0.22);
            transform: translateY(-1px);
            color: #FFFFFF;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="fnf">
        <div class="row">
            <div class="col-12">
                <div class="fnf-card">
                    <div class="fnf-title">Full and Final Settlement</div>

                    <div class="fnf-form">
                        <div class="row g-3">
                            <div class="col-md-6">
                                <div class="fnf-label">Employee</div>
                                <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlEmployee_SelectedIndexChanged">
                                </asp:DropDownList>
                                <div class="fnf-hint">Only employees whose exit has been HR-accepted are listed here.</div>
                            </div>
                        </div>

                        <asp:Panel ID="pnlMail" runat="server" Visible="false">
                            <div class="row g-3 mt-3">
                                <div class="col-md-12">
                                    <div class="fnf-attachments">
                                        <strong>Attachments</strong>
                                        <asp:Label ID="lblAttachments" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="row g-3 mt-0">
                                <div class="col-md-6">
                                    <div class="fnf-label">To</div>
                                    <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" ReadOnly="true" />
                                </div>
                                <div class="col-md-6">
                                    <div class="fnf-label">CC <span class="text-muted fw-normal">(optional)</span></div>
                                    <asp:TextBox ID="txtCc" runat="server" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="row g-3 mt-0">
                                <div class="col-md-12">
                                    <div class="fnf-label">Subject</div>
                                    <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="row g-3 mt-0">
                                <div class="col-md-12">
                                    <div class="fnf-label">Body</div>
                                    <asp:TextBox ID="txtBody" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="8" />
                                    <div class="fnf-hint">Pre-filled automatically - feel free to edit before sending.</div>
                                </div>
                            </div>
                            <div class="row g-3 mt-2">
                                <div class="col-md-12">
                                    <asp:Button ID="btnSendMail" runat="server" CssClass="fnf-btn" Text="Send Mail" OnClientClick="return validateMailForm();" OnClick="btnSendMail_Click" />
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        function showFullAndFinalResult(status, remark) {
            Swal.fire({
                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }

        function isValidEmailList(value) {
            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            var parts = value.split(/[;,]/);
            for (var i = 0; i < parts.length; i++) {
                var trimmed = parts[i].trim();
                if (trimmed === '') continue;
                if (!emailRegex.test(trimmed)) return false;
            }
            return true;
        }

        function validateMailForm() {
            var to = document.getElementById('<%= txtTo.ClientID %>').value.trim();
            var cc = document.getElementById('<%= txtCc.ClientID %>').value.trim();
            var subject = document.getElementById('<%= txtSubject.ClientID %>').value.trim();
            var body = document.getElementById('<%= txtBody.ClientID %>').value.trim();

            if (!to) {
                Swal.fire({ icon: 'error', text: 'This employee has no email on file.', timer: 3000, showConfirmButton: false });
                return false;
            }
            if (!isValidEmailList(to)) {
                Swal.fire({ icon: 'error', text: 'The employee\'s email on file looks invalid.', timer: 3000, showConfirmButton: false });
                return false;
            }
            if (cc && !isValidEmailList(cc)) {
                Swal.fire({ icon: 'error', text: 'Please enter valid email address(es) in CC.', timer: 3000, showConfirmButton: false });
                return false;
            }
            if (!subject) {
                Swal.fire({ icon: 'error', text: 'Please enter a subject.', timer: 3000, showConfirmButton: false });
                return false;
            }
            if (!body) {
                Swal.fire({ icon: 'error', text: 'Please enter the email body.', timer: 3000, showConfirmButton: false });
                return false;
            }
            return true;
        }
    </script>
</asp:Content>
