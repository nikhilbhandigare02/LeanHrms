<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="NoticePeriodManagement.aspx.cs" Inherits="HRMS.View.Modules.NoticePeriodManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .notice-page .card {
            border: 0;
            border-radius: 14px;
            box-shadow: 0 10px 28px rgba(31, 45, 61, 0.08);
        }

        .notice-page .page-title {
            font-size: 1.6em;
            font-weight: bold;
            color: #1f2d3d;
        }

        .notice-page .info-row {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 12px 0;
            border-bottom: 1px solid #eef2f7;
        }

        .notice-page .info-row:last-child {
            border-bottom: 0;
        }

        .notice-page .info-label {
            font-size: 14px;
            color: #64748b;
        }

        .notice-page .info-value {
            font-size: 14px;
            font-weight: 600;
            color: #1f2d3d;
        }

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="notice-page">
        <asp:HiddenField ID="hfResignationId" runat="server" />

        <div class="row">
            <div class="col-lg-12">
                <div class="card">
                    <div class="card-body">

                        <div class="d-flex justify-content-between align-items-center mb-4">
                            <span class="page-title">Notice period management</span>
                            <asp:Label ID="lblNoticeStatus" runat="server" CssClass="badge bg-success" Text="Active" />
                        </div>

                        <div class="info-row">
                            <span class="info-label">Notice start date</span>
                            <asp:Label ID="lblNoticeStartDate" runat="server" CssClass="info-value" Text="-" />
                        </div>
                        <div class="info-row">
                            <span class="info-label">Notice end date</span>
                            <asp:Label ID="lblNoticeEndDate" runat="server" CssClass="info-value" Text="-" />
                        </div>
                        <div class="info-row">
                            <span class="info-label">Remaining days</span>
                            <asp:Label ID="lblRemainingDays" runat="server" CssClass="info-value" Text="-" />
                        </div>
                        <div class="info-row">
                            <span class="info-label">Last working date</span>
                            <asp:Label ID="lblLastWorkingDate" runat="server" CssClass="info-value" Text="-" />
                        </div>
                        <div class="info-row">
                            <span class="info-label">Attendance status</span>
                            <asp:Label ID="lblAttendanceStatus" runat="server" CssClass="info-value" Text="-" />
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
