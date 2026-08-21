<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="TerminationDetailsView.aspx.cs" Inherits="HRMS.View.Modules.TerminationDetailsView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
            background: linear-gradient(135deg, #1f2937, #374151);
            color: #fff;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            box-shadow: 0 4px 10px rgba(31, 41, 55, 0.25);
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

        /* ===== Profile card ===== */
        .profile-card {
            overflow: hidden;
            border: 1px solid #e9ecef;
        }

        .profile-banner {
            height: 64px;
            background: linear-gradient(135deg, #1f2937, #374151);
        }

        .profile-header-row {
            display: flex;
            align-items: center;
            gap: 18px;
            margin-top: -38px;
            margin-bottom: 24px;
            flex-wrap: wrap;
        }

        .profile-avatar {
            width: 76px;
            height: 76px;
            border-radius: 50%;
            background: #556ee6;
            color: #fff;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 26px;
            font-weight: 700;
            border: 4px solid #ffffff;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
            flex: 0 0 76px;
        }

        .profile-name {
            font-size: 19px;
            font-weight: 700;
            color: #212529;
            margin-top: 30px;
        }

        .profile-code-row {
            margin-top: 4px;
            display: flex;
            align-items: center;
            gap: 8px;
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

        .profile-status-wrap {
            margin-left: auto;
            margin-top: 30px;
        }

        .status-badge {
            display: inline-block;
            padding: 5px 14px;
            font-size: 12px;
            font-weight: 700;
            letter-spacing: 0.3px;
            border-radius: 20px;
        }

            .status-badge.terminated {
                color: #e03131;
                background-color: rgba(224, 49, 49, 0.1);
            }

            .status-badge.showcause {
                color: #f08c00;
                background-color: rgba(240, 140, 0, 0.1);
            }

        /* ===== Info grid ===== */
        .info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 14px;
            margin-bottom: 8px;
        }

        .contents-wrapper {
            display: contents;
        }

        .info-card {
            background-color: #f8f9fa;
            border: 1px solid #eef0f2;
            border-radius: 8px;
            padding: 12px 16px;
        }

            .info-card.full-width {
                grid-column: 1 / -1;
            }

        .info-label {
            font-size: 11.5px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.4px;
            color: #868e96;
            margin-bottom: 4px;
        }

        .info-value {
            font-size: 14px;
            font-weight: 600;
            color: #212529;
        }

            .info-value.highlight {
                color: #b91c1c;
            }

        /* ===== Letter section ===== */
        .letter-section {
            margin-top: 24px;
        }

        .letter-heading {
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.4px;
            color: #1f2937;
            margin-bottom: 8px;
        }

        .letter-box {
            padding: 28px 34px;
            border: 1px solid #e5e7eb;
            border-left: 3px solid #1f2937;
            border-radius: 0 8px 8px 0;
            background-color: #ffffff;
            font-family: Georgia, 'Times New Roman', Times, serif;
            font-size: 14.5px;
            line-height: 1.8;
            color: #2b2f33;
        }

            .letter-box .letter-paragraph {
                margin: 0 0 16px 0;
            }

            .letter-box .letter-paragraph:last-child {
                margin-bottom: 0;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="termination-page-header">
        <div class="icon-badge">
            <i class="fa fa-eye"></i>
        </div>
        <div>
            <h1>Termination Details</h1>
            <p>Read-only record of the saved termination / show cause notice.</p>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-lg rounded-3 profile-card">

                <asp:Panel ID="pnlNotFound" runat="server" Visible="false">
                    <div class="card-body">
                        <div class="alert alert-warning mb-0">
                            No termination or show cause record was found for this employee.
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlDetails" runat="server">

                    <div class="profile-banner"></div>

                    <div class="card-body">

                        <div class="profile-header-row">
                            <div class="profile-avatar"><asp:Literal ID="litInitials" runat="server" /></div>
                            <div>
                                <div class="profile-name"><asp:Literal ID="litEmployeeName" runat="server" /></div>
                                <div class="profile-code-row">
                                    <span class="badge-emp-code"><asp:Literal ID="litEmployeeCode" runat="server" /></span>
                                </div>
                            </div>
                            <div class="profile-status-wrap">
                                <asp:Literal ID="litStatus" runat="server" />
                            </div>
                        </div>

                        <div class="info-grid">
                            <div class="info-card">
                                <div class="info-label">Termination Type</div>
                                <div class="info-value"><asp:Literal ID="litTerminationType" runat="server" /></div>
                            </div>
                            <div class="info-card">
                                <div class="info-label">Termination Date</div>
                                <div class="info-value highlight"><asp:Literal ID="litTerminationDate" runat="server" /></div>
                            </div>

                            <asp:Panel ID="pnlPerformance" runat="server" Visible="false" CssClass="contents-wrapper">
                                <div class="info-card">
                                    <div class="info-label">Performance Rating</div>
                                    <div class="info-value"><asp:Literal ID="litPerformanceRating" runat="server" /></div>
                                </div>
                                <div class="info-card">
                                    <div class="info-label">Notice Period</div>
                                    <div class="info-value"><asp:Literal ID="litNoticePeriod" runat="server" /></div>
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="pnlShowCause" runat="server" Visible="false" CssClass="contents-wrapper">
                                <div class="info-card">
                                    <div class="info-label">Response Deadline</div>
                                    <div class="info-value highlight"><asp:Literal ID="litResponseDeadline" runat="server" /></div>
                                </div>
                            </asp:Panel>

                            <div class="info-card full-width">
                                <div class="info-label">Reason</div>
                                <div class="info-value"><asp:Literal ID="litReason" runat="server" /></div>
                            </div>
                        </div>

                        <div class="letter-section">
                            <div class="letter-heading">Letter / Notice Content</div>
                            <div class="letter-box">
                                <asp:Literal ID="litLetterContent" runat="server" />
                            </div>
                        </div>

                    </div>
                </asp:Panel>

                <div class="card-body pt-0">
                    <a href="~/View/Modules/EmployeeAction.aspx" runat="server" class="btn btn-secondary">
                        <i class="fa fa-arrow-left"></i> Back to Termination List
                    </a>
                </div>

            </div>
        </div>
    </div>

</asp:Content>
