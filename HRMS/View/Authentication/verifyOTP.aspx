<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="verifyOTP.aspx.cs" Inherits="HRMS.View.Authentication.verifyOTP" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Verify OTP</title>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="../../assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link rel="shortcut icon" href="../../assets/images/faviconicon.png" />
    <link href="../../assets/css/app.min.css" rel="stylesheet" type="text/css" />
    <link href="../../assets/css/icons.min.css" rel="stylesheet" type="text/css" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <style>
        body {
            min-height: 100vh;
            background: #f4f6fb;
            font-family: "Segoe UI", Arial, sans-serif;
        }

        .otp-card {
            border: 0;
            border-radius: 4px;
            box-shadow: 0 16px 40px rgba(15, 23, 42, 0.06);
        }

        .otp-logo {
            padding-top: 20px;
        }

        .otp-subtitle {
            color: #111827;
            font-size: 14px;
            margin: 18px 0 0;
        }

        .otp-icon {
            width: 54px;
            height: 54px;
            border-radius: 50%;
            background: #eef4ff;
            color: #2563eb;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 24px;
            margin-bottom: 12px;
        }

        .otp-heading {
            color: #1f2937;
            font-size: 22px;
            font-weight: 800;
            margin-bottom: 6px;
        }

        .otp-help {
            color: #64748b;
            font-size: 14px;
            margin-bottom: 24px;
        }

        .otp-label {
            font-weight: 700;
            font-size: 15px;
            color: #2f3a4a;
            margin-bottom: 9px;
        }

        .otp-input {
            height: 42px;
            border: 1px solid #cbd5e1;
            border-radius: 4px;
            font-size: 16px;
            font-weight: 700;
            letter-spacing: 10px;
            text-align: center;
            color: #111827;
            background: #fff;
        }

        .otp-input:focus {
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, .12);
        }

        .otp-timer {
            color: #64748b;
            font-size: 13px;
            text-align: center;
            margin: 14px 0 18px;
        }

        .otp-timer span {
            color: #2563eb;
            font-weight: 800;
        }

        .otp-button {
            height: 42px;
            border-radius: 4px;
            background: #596be8;
            border-color: #596be8;
            font-size: 16px;
            font-weight: 800;
            color: #fff;
        }

        .otp-button:hover {
            background: #4659d4;
            border-color: #4659d4;
        }

        .otp-resend {
            text-align: center;
            font-size: 14px;
            color: #64748b;
            margin-top: 18px;
        }

        .otp-resend a {
            color: #2563eb;
            font-weight: 700;
            text-decoration: none;
        }

        .otp-resend a:hover {
            text-decoration: underline;
        }
    </style>
    <script type="text/javascript">
        function showDataSavedMessage(status, remark) {
            Swal.fire({
                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 3000,
                showConfirmButton: false
            });
        }

        document.addEventListener("DOMContentLoaded", function () {
            var otp = document.getElementById("<%= txt_verifyotp.ClientID %>");
            if (otp) {
                otp.setAttribute("maxlength", "6");
                otp.setAttribute("inputmode", "numeric");
                otp.setAttribute("autocomplete", "one-time-code");
                otp.addEventListener("input", function () {
                    this.value = this.value.replace(/\D/g, "").slice(0, 6);
                });
            }

            var remaining = 165;
            var timer = document.getElementById("otpTimer");
            function renderTimer() {
                if (!timer) return;
                var min = Math.floor(remaining / 60);
                var sec = remaining % 60;
                timer.textContent = String(min).padStart(2, "0") + ":" + String(sec).padStart(2, "0");
                if (remaining > 0) remaining -= 1;
            }
            renderTimer();
            window.setInterval(renderTimer, 1000);
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="account-pages my-5 pt-sm-5">
            <div class="container">
                <div class="row justify-content-center">
                    <div class="col-md-8 col-lg-6 col-xl-5">
                        <div class="card otp-card overflow-hidden">
                            <div class="row justify-content-center align-items-center">
                                <div class="col-12 text-center">
                                    <div class="otp-logo">
                                        <img src="../../assets/images/alphonsol_logo.png" alt="Alphonsol" height="30" />
                                    </div>
                                    <h6 class="otp-subtitle">Secure access to your HRMS account</h6>
                                </div>
                            </div>

                            <div class="card-body pt-4">
                                <div class="p-2">
                                    <div class="alert alert-info" runat="server" id="divAlert" visible="false">
                                        <asp:Label Text="" ID="lblErrorMessager" runat="server" />
                                    </div>

                                    <div id="email" runat="server" visible="false">
                                        <asp:TextBox ID="emailtxt" runat="server" CssClass="form-control" AutoCompleteType="Disabled"></asp:TextBox>
                                    </div>

                                    <div id="div_otp" runat="server" visible="false">
                                        <div class="text-center">
                                            <span class="otp-icon"><i class="mdi mdi-email-check-outline"></i></span>
                                            <div class="otp-heading">Verify OTP</div>
                                            <div class="otp-help">We have sent a 6-digit OTP to your registered email</div>
                                        </div>

                                        <div class="form-group">
                                            <label class="otp-label" for="<%= txt_verifyotp.ClientID %>">Enter Your OTP</label>
                                            <asp:TextBox ID="txt_verifyotp" runat="server" CssClass="form-control otp-input" AutoCompleteType="Disabled" placeholder="------" required=""></asp:TextBox>
                                        </div>

                                        <div class="otp-timer">
                                            OTP will expire in <span id="otpTimer">02:45</span>
                                        </div>

                                        <div class="mt-3 d-grid">
                                            <asp:Button ID="btn_verifyOtp" runat="server" Text="VERIFY OTP" CssClass="btn otp-button waves-effect waves-light" OnClick="btn_verifyOtp_Click" />
                                        </div>

                                        <div class="otp-resend">
                                            Didn't receive the OTP?
                                            <a href="<%= Request.RawUrl %>">Resend OTP</a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
