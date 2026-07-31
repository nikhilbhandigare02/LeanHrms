<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="changePassword.aspx.cs" Inherits="HRMS.View.Authentication.changePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link rel="shortcut icon" href="../../assets/images/faviconicon.png" />
    <link href="../../assets/css/app.min.css" rel="stylesheet" type="text/css" />
    <link href="../../assets/css/icons.min.css" rel="stylesheet" type="text/css" />
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script type="text/javascript">
        function showChangePassMessage(status, remark) {
            Swal.fire({

                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }
    </script>
    <style>
        .password-container {
            position: relative;
        }

            .password-container input {
                padding-right: 40px; /* space for the icon */
            }

        .password-toggle-icon {
            position: absolute;
            top: 50%;
            right: 10px;
            transform: translateY(-50%);
            cursor: pointer;
            color: #aaa;
        }

        .password-policy-checklist {
            list-style: none;
            padding: 0;
            margin: 8px 0 0 0;
            font-size: 12px;
        }

            .password-policy-checklist li {
                color: #6c757d;
                margin-bottom: 3px;
            }

                .password-policy-checklist li i {
                    font-size: 8px;
                    margin-right: 6px;
                }

                .password-policy-checklist li.valid {
                    color: #198754;
                }

                    .password-policy-checklist li.valid i:before {
                        content: "\f00c";
                    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="account-pages my-5 pt-sm-5">
            <div class="container">
                <div class="row justify-content-center">
                    <div class="col-md-8 col-lg-6 col-xl-5">
                        <div class="card overflow-hidden">
                            <div class="bg-primary-subtle">
                                <div class="row">
                                    <div class="col-7 mx-auto">
                                        <div class="text-primary p-4">
                                            <h4 class="text-primary">Change Password</h4>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <br />
                            <br />
                            <div class="card-body pt-0">
                                <%--  <div class="auth-logo">
                                        <div class="avatar-md profile-user-wid mb-4">
                                            <span class="avatar-title rounded-circle bg-light">
                                                <img src="~/assets/images/logo-light.svg" alt="" class="rounded-circle" height="34" />
                                            </span>
                                        </div>
                                </div>--%>
                                <div class="p-2">
                                    <div class="alert alert-info" runat="server" id="divAlert" visible="false">
                                        <asp:Label Text="" ID="lblErrorMessager" runat="server" />
                                    </div>
                                    <div>
                                        <asp:TextBox ID="txtusername" runat="server" CssClass="form-control" AutoCompleteType="Disabled" placeholder="User Name"></asp:TextBox>
                                    </div>
                                    <br />
                                    <div class="password-container">
                                        <asp:TextBox ID="txtoldpass" runat="server" CssClass="form-control" AutoCompleteType="Disabled" TextMode="Password" placeholder="Old Password" required=""></asp:TextBox>
                                        <span class="password-toggle-icon" onclick="togglePasswordVisibility('<%= txtoldpass.ClientID %>', this)">
                                            <i class="fa fa-eye-slash"></i>
                                        </span>
                                    </div>
                                    <br />
                                   <div class="password-container">
                                        <asp:TextBox ID="txtnewpass" runat="server" CssClass="form-control" AutoCompleteType="Disabled" TextMode="Password" placeholder="New Password" onkeyup="updatePasswordPolicyChecklist(this.value)"></asp:TextBox>
                                       <span class="password-toggle-icon" onclick="togglePasswordVisibility('<%= txtnewpass.ClientID %>', this)">
    <i class="fa fa-eye-slash"></i>
</span>
                                    </div>
                                    <ul class="password-policy-checklist" id="passwordPolicyChecklist">
                                        <li id="pwRuleLength"><i class="fa fa-circle"></i> At least 8 characters</li>
                                        <li id="pwRuleUpper"><i class="fa fa-circle"></i> At least 1 uppercase letter</li>
                                        <li id="pwRuleLower"><i class="fa fa-circle"></i> At least 1 lowercase letter</li>
                                        <li id="pwRuleNumber"><i class="fa fa-circle"></i> At least 1 number</li>
                                        <li id="pwRuleSymbol"><i class="fa fa-circle"></i> At least 1 special character</li>
                                    </ul>
                                    <br />
                                    <div class="password-container">
                                        <asp:TextBox ID="txtconfirmpass" runat="server" CssClass="form-control" AutoCompleteType="Disabled" TextMode="Password" placeholder="Confirm password"></asp:TextBox>
                                   <span class="password-toggle-icon" onclick="togglePasswordVisibility('<%= txtconfirmpass.ClientID %>', this)">
    <i class="fa fa-eye-slash"></i>
</span>
                                        </div>
                                    <br />
                                    <div class="mt-3 d-grid">
                                        <asp:Button ID="loginButton" runat="server" Text="Change Password" CssClass="btn btn-primary waves-effect waves-light" OnClick="btnchange_Click" OnClientClick="return validateChangePasswordForm();" />
                                        <%--<asp:Button ID="cancelBtn" runat="server" Text="Cancel " CssClass="btn btn-primary waves-effect waves-light" OnClick="cancelBtn_Click" />--%>
                                    </div>
                                    <div class="mt-4 text-center">
                                        <a href="login.aspx">LogIn here <i class="mdi mdi-lock me-1"></i></a>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script type="text/javascript">
        function showChangePassMessage(status, remark) {
            Swal.fire({

                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }
    </script>

    <script>
        function getPasswordPolicyChecks(password) {
            return {
                pwRuleLength: password.length >= 8,
                pwRuleUpper: /[A-Z]/.test(password),
                pwRuleLower: /[a-z]/.test(password),
                pwRuleNumber: /[0-9]/.test(password),
                pwRuleSymbol: /[^A-Za-z0-9]/.test(password)
            };
        }

        function updatePasswordPolicyChecklist(password) {
            var checks = getPasswordPolicyChecks(password);
            var isValid = true;

            Object.keys(checks).forEach(function (id) {
                var item = document.getElementById(id);
                if (item) {
                    item.classList.toggle('valid', checks[id]);
                }
                if (!checks[id]) {
                    isValid = false;
                }
            });

            return isValid;
        }

        function validateChangePasswordForm() {
            var newPasswordInput = document.getElementById('<%= txtnewpass.ClientID %>');
            var confirmPasswordInput = document.getElementById('<%= txtconfirmpass.ClientID %>');

            var isPolicyValid = updatePasswordPolicyChecklist(newPasswordInput.value);
            if (!isPolicyValid) {
                showChangePassMessage('Error', 'Password does not meet the required policy.');
                newPasswordInput.focus();
                return false;
            }

            if (newPasswordInput.value !== confirmPasswordInput.value) {
                showChangePassMessage('Error', 'Passwords do not match.');
                confirmPasswordInput.focus();
                return false;
            }

            return true;
        }

        function togglePasswordVisibility(inputId, iconSpan) {
            var textbox = document.getElementById(inputId);
            var icon = iconSpan.querySelector('i');

            if (textbox.type === "password") {
                textbox.type = "text";
                icon.classList.remove("fa-eye-slash");
                icon.classList.add("fa-eye");
            } else {
                textbox.type = "password";
                icon.classList.remove("fa-eye");
                icon.classList.add("fa-eye-slash");
            }
        }
    </script>
</body>
</html>
