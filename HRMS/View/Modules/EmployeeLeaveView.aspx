<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="EmployeeLeaveView.aspx.cs" Inherits="HRMS.View.Modules.EmployeeLeaveView" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .leave-view-card {
            border: 0;
            border-radius: 12px;
            box-shadow: 0 6px 20px rgba(15, 23, 42, .06);
        }

        .leave-view-section-title {
            font-size: 15px;
            font-weight: 700;
            color: #223153;
            margin-bottom: 16px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .leave-view-field label {
            font-size: 12px;
            font-weight: 700;
            color: #64748B;
            text-transform: uppercase;
            letter-spacing: .03em;
            margin-bottom: 6px;
        }

        .leave-view-field .form-control {
            background-color: #F7FAFF;
            border: 1px solid #DDE6F3;
            border-radius: 8px;
            color: #0B1B45;
            font-weight: 600;
        }

        .leave-status-badge {
            font-size: 13px;
            font-weight: 700;
            padding: 8px 16px;
            border-radius: 20px;
        }

        .leave-pending-note {
            display: flex;
            align-items: center;
            gap: 10px;
            background: #FFF8E6;
            border: 1px solid #FCE7A0;
            color: #7A5B00;
            border-radius: 8px;
            padding: 12px 16px;
            font-weight: 600;
            font-size: 13px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-12">
            <div class="card leave-view-card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <div class="card-title mb-0" style="font-size: 22px;">Employee Leave View</div>
                        <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
                    </div>

                    <div class="row mb-3">
                        <div class="col-12">
                            <div class="card leave-view-card">
                                <div class="card-body">
                                    <div class="leave-view-section-title">
                                        <i class="far fa-calendar-alt"></i> Leave Request Details
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <div class="mb-3 leave-view-field">
                                                <label>Leave</label>
                                                <asp:TextBox ID="txtLookupId" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="mb-3 leave-view-field">
                                                <label>Leave Type</label>
                                                <asp:TextBox ID="txtLeaveTypeId" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="mb-3 leave-view-field">
                                                <label>Leave From Date</label>
                                                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <div class="mb-3 leave-view-field">
                                                <label>Leave To Date</label>
                                                <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </div>
                                        </div>
                                        <div class="col-12">
                                            <div class="mb-3 leave-view-field">
                                                <label>Leave Description</label>
                                                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" ReadOnly="true" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-12">
                            <div class="card leave-view-card">
                                <div class="card-body">
                                    <div class="leave-view-section-title">
                                        <i class="far fa-check-circle"></i> Verification Details
                                    </div>
                                    <div class="row">
                                        <div class="col-12 mb-3">
                                            <label class="d-block" style="font-size: 12px; font-weight: 700; color: #64748B; text-transform: uppercase; letter-spacing: .03em;">Verification Status</label>
                                            <asp:Label ID="lblApprovalStatus" runat="server" CssClass="badge leave-status-badge bg-warning" />
                                        </div>

                                        <asp:Panel ID="pnlApprovedDates" runat="server" CssClass="col-12" Visible="false">
                                            <div class="row">
                                                <div class="col-lg-6">
                                                    <div class="mb-3 leave-view-field">
                                                        <label>Approved From Date</label>
                                                        <asp:TextBox ID="txtApprovedFromDate" runat="server" CssClass="form-control" ReadOnly="true" />
                                                    </div>
                                                </div>
                                                <div class="col-lg-6">
                                                    <div class="mb-3 leave-view-field">
                                                        <label>Approved To Date</label>
                                                        <asp:TextBox ID="txtApprovedToDate" runat="server" CssClass="form-control" ReadOnly="true" />
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:Panel>

                                        <asp:Panel ID="pnlRejectionReason" runat="server" CssClass="col-12" Visible="false">
                                            <div class="mb-3 leave-view-field">
                                                <label>Rejection Reason</label>
                                                <asp:TextBox ID="txtRejectionRemark" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" ReadOnly="true" />
                                            </div>
                                        </asp:Panel>

                                        <asp:Panel ID="pnlPendingNote" runat="server" CssClass="col-12" Visible="false">
                                            <div class="leave-pending-note">
                                                <i class="far fa-clock"></i> This leave request is awaiting approval.
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
