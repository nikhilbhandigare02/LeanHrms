<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="SendDocuments.aspx.cs" Inherits="HRMS.View.Modules.SendDocuments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <style>
        .send-doc {
            --sd-primary: #2563EB;
            --sd-primary-dark: #1D4ED8;
            --sd-heading: #1E293B;
            --sd-muted: #64748B;
            --sd-line: #E8EEF7;
            --sd-panel: #F8FAFC;
        }

        .send-doc .sd-card {
            background: #FFFFFF;
            border: 1px solid var(--sd-line);
            border-radius: 12px;
            box-shadow: 0 10px 30px rgba(15, 23, 42, 0.05);
            padding: 28px 30px;
        }

        .send-doc .sd-title {
            color: var(--sd-heading);
            font-size: 22px;
            font-weight: 800;
            margin-bottom: 22px;
        }

        .send-doc .sd-form {
            background: var(--sd-panel);
            border: 1px solid var(--sd-line);
            border-radius: 10px;
            padding: 20px;
        }

        .send-doc .sd-label {
            color: var(--sd-heading);
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .send-doc .sd-form .form-control,
        .send-doc .sd-form .form-select {
            border: 1px solid #D9E2EF;
            border-radius: 8px;
        }

        .send-doc .sd-hint {
            color: var(--sd-muted);
            font-size: 12.5px;
            margin-top: 12px;
        }

        .send-doc .sd-btn {
            background: var(--sd-primary);
            border: none;
            border-radius: 8px;
            color: #FFFFFF;
            font-weight: 600;
            height: 42px;
            transition: background-color .18s ease, box-shadow .18s ease, transform .18s ease;
        }

        .send-doc .sd-btn:hover {
            background: var(--sd-primary-dark);
            box-shadow: 0 10px 20px rgba(37, 99, 235, 0.22);
            transform: translateY(-1px);
            color: #FFFFFF;
        }

        #mailModal textarea.form-control {
            resize: vertical;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="send-doc">
        <div class="row">
            <div class="col-12">
                <div class="sd-card">
                    <div class="sd-title">Send Documents</div>

                    <div class="sd-form">
                        <div class="row g-3">
                            <div class="col-md-4">
                                <div class="sd-label">Document Category</div>
                                <asp:DropDownList ID="ddlDocumentCategory" runat="server" CssClass="form-select" onchange="toggleCategoryMode();">
                                    <asp:ListItem Value="">-- Select Category --</asp:ListItem>
                                    <asp:ListItem Value="Offer Letter">Offer Letter</asp:ListItem>
                                    <asp:ListItem Value="Appointment Letter">Appointment Letter</asp:ListItem>
                                    <asp:ListItem Value="Confirmation Letter (Probation)">Confirmation Letter (Probation)</asp:ListItem>
                                    <asp:ListItem Value="Confirmation Letter (Internship)">Confirmation Letter (Internship)</asp:ListItem>
                                    <asp:ListItem Value="Others">Others</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="row g-3 mt-0" id="divGeneratedFields" runat="server">
                            <div class="col-md-4">
                                <div class="sd-label">Employee / Candidate Name</div>
                                <asp:TextBox ID="txtCandidateName" runat="server" CssClass="form-control" placeholder="Type the full name" />
                            </div>
                            <div class="col-md-4">
                                <div class="sd-label">Designation</div>
                                <asp:TextBox ID="txtDesignation" runat="server" CssClass="form-control" placeholder="e.g. Software Engineer" />
                            </div>
                            <div class="col-md-4">
                                <div class="sd-label">Effective / Confirmation Date</div>
                                <asp:TextBox ID="txtEffectiveDate" runat="server" CssClass="form-control" TextMode="Date" />
                            </div>
                            <div class="col-md-8">
                                <div class="sd-label">Additional Details <span class="text-muted fw-normal">(optional - e.g. CTC, stipend, probation duration)</span></div>
                                <asp:TextBox ID="txtAdditionalDetails" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="row g-3 mt-0" id="divUploadField" runat="server" style="display:none;">
                            <div class="col-md-8">
                                <div class="sd-label">Attach Document</div>
                                <asp:FileUpload ID="fuDocument" runat="server" CssClass="form-control" accept=".pdf,.doc,.docx" />
                            </div>
                        </div>
                        <div class="row g-3 mt-0">
                            <div class="col-md-12">
                                <button type="button" id="btnOpenMailModal" class="sd-btn" style="min-width:160px;" onclick="return openMailModal();">Send</button>
                            </div>
                        </div>
                        <div class="sd-hint" id="hintGenerated" runat="server">Type the name, category, designation and effective date - the document is generated automatically and attached to the email as a PDF. No manual file upload needed.</div>
                        <div class="sd-hint" id="hintUpload" runat="server" style="display:none;">Select "Others" and attach the document (PDF, DOC, DOCX, max 10 MB) you want to send.</div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Mail Compose Modal -->
    <div class="modal fade" id="mailModal" tabindex="-1" role="dialog" aria-labelledby="mailModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="mailModalLabel">Compose Email</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="sd-label" for="<%= txtTo.ClientID %>">To</label>
                        <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" placeholder="recipient@example.com, another@example.com" />
                    </div>
                    <div class="row">
                        <div class="col-md-6 mb-3">
                            <label class="sd-label" for="<%= txtCc.ClientID %>">CC</label>
                            <asp:TextBox ID="txtCc" runat="server" CssClass="form-control" placeholder="Optional" />
                        </div>
                        <div class="col-md-6 mb-3">
                            <label class="sd-label" for="<%= txtBcc.ClientID %>">BCC</label>
                            <asp:TextBox ID="txtBcc" runat="server" CssClass="form-control" placeholder="Optional" />
                        </div>
                    </div>
                    <div class="mb-3">
                        <label class="sd-label" for="<%= txtSubject.ClientID %>">Subject</label>
                        <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" />
                    </div>
                    <div class="mb-3">
                        <label class="sd-label" for="<%= txtBody.ClientID %>">Body</label>
                        <asp:TextBox ID="txtBody" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="8" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    <asp:Button ID="btnSendMail" runat="server" CssClass="btn btn-primary" Text="Send" OnClientClick="return validateMailForm();" OnClick="btnSendMail_Click" />
                </div>
            </div>
        </div>
    </div>

    <script>
        function showSendDocumentResult(status, remark) {
            Swal.fire({
                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }

        function isUploadCategory(category) {
            return category === 'Others';
        }

        function toggleCategoryMode() {
            var category = document.getElementById('<%= ddlDocumentCategory.ClientID %>').value;
            var generatedFields = document.getElementById('<%= divGeneratedFields.ClientID %>');
            var uploadField = document.getElementById('<%= divUploadField.ClientID %>');
            var hintGenerated = document.getElementById('<%= hintGenerated.ClientID %>');
            var hintUpload = document.getElementById('<%= hintUpload.ClientID %>');

            if (isUploadCategory(category)) {
                generatedFields.style.display = 'none';
                uploadField.style.display = '';
                hintGenerated.style.display = 'none';
                hintUpload.style.display = '';
            } else {
                generatedFields.style.display = '';
                uploadField.style.display = 'none';
                hintGenerated.style.display = '';
                hintUpload.style.display = 'none';
            }
        }

        function openMailModal() {
            var category = document.getElementById('<%= ddlDocumentCategory.ClientID %>').value;

            if (!category) {
                Swal.fire({ icon: 'error', text: 'Please select a document category.', timer: 3000, showConfirmButton: false });
                return false;
            }

            if (isUploadCategory(category)) {
                var fileInput = document.getElementById('<%= fuDocument.ClientID %>');
                if (!fileInput || !fileInput.value) {
                    Swal.fire({ icon: 'error', text: 'Please attach a document.', timer: 3000, showConfirmButton: false });
                    return false;
                }
            } else {
                var candidateNameField = document.getElementById('<%= txtCandidateName.ClientID %>');
                if (!candidateNameField || !candidateNameField.value.trim()) {
                    Swal.fire({ icon: 'error', text: 'Please type the employee/candidate name.', timer: 3000, showConfirmButton: false });
                    return false;
                }

                var effectiveDateField = document.getElementById('<%= txtEffectiveDate.ClientID %>');
                if (!effectiveDateField || !effectiveDateField.value) {
                    Swal.fire({ icon: 'error', text: 'Please select the effective/confirmation date.', timer: 3000, showConfirmButton: false });
                    return false;
                }
            }

            var subjectField = document.getElementById('<%= txtSubject.ClientID %>');
            if (subjectField && !subjectField.value) {
                subjectField.value = 'Your ' + category + ' from Alphonsol Pvt. Ltd.';
            }

            var modal = bootstrap.Modal.getOrCreateInstance(document.getElementById('mailModal'));
            modal.show();
            return false;
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
            var bcc = document.getElementById('<%= txtBcc.ClientID %>').value.trim();
            var subject = document.getElementById('<%= txtSubject.ClientID %>').value.trim();
            var body = document.getElementById('<%= txtBody.ClientID %>').value.trim();

            // Returning false here keeps the modal open (no postback happens at all),
            // instead of letting a full-page postback close it out from under the user.
            if (!to) {
                Swal.fire({ icon: 'error', text: 'Please enter at least one recipient email in To.', timer: 3000, showConfirmButton: false });
                return false;
            }
            if (!isValidEmailList(to)) {
                Swal.fire({ icon: 'error', text: 'Please enter valid email address(es) in To.', timer: 3000, showConfirmButton: false });
                return false;
            }
            if (cc && !isValidEmailList(cc)) {
                Swal.fire({ icon: 'error', text: 'Please enter valid email address(es) in CC.', timer: 3000, showConfirmButton: false });
                return false;
            }
            if (bcc && !isValidEmailList(bcc)) {
                Swal.fire({ icon: 'error', text: 'Please enter valid email address(es) in BCC.', timer: 3000, showConfirmButton: false });
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

        document.addEventListener('DOMContentLoaded', function () {
            toggleCategoryMode();
        });
    </script>
</asp:Content>
