<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master"
    AutoEventWireup="true"
    CodeBehind="AttendanceDetails.aspx.cs"
    Inherits="HRMS.View.Modules.AttendanceDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css" rel="stylesheet" />

    <style>
        .payroll-wrap {
            max-width: 1400px;
            margin: 0 auto;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            flex-wrap: wrap;
            gap: 15px;
        }

        .page-title {
            font-size: 28px;
            font-weight: 700;
            color: #111827;
        }

        .page-subtitle {
            color: #6b7280;
            font-size: 14px;
        }

        .sec-card {
            background: #fff;
            border: 1px solid #e5e7eb;
            border-radius: 12px;
            box-shadow: 0 4px 14px rgba(17,24,39,.05);
            padding: 18px;
            margin-bottom: 18px;
        }

        .sec-head {
            font-size: 18px;
            font-weight: 600;
            color: #1f2937;
            margin-bottom: 15px;
        }

        .attendance-filter {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
        }

        .attendance-filter select {
            min-width: 260px;
            border-radius: 8px;
            border: 1px solid #DDE6F3;
            height: 42px;
            padding: 0 12px;
        }

        .attendance-filter-date {
            display: flex;
            flex-direction: column;
            gap: 2px;
        }

            .attendance-filter-date label {
                font-size: 11px;
                font-weight: 600;
                color: #6b7280;
            }

            .attendance-filter-date input {
                border-radius: 8px;
                border: 1px solid #DDE6F3;
                height: 42px;
                padding: 0 12px;
            }

        .attendance-grid {
            width: 100%;
            border-collapse: collapse;
        }

            .attendance-grid th {
                background: #f8fafc;
                padding: 12px;
                font-size: 13px;
                font-weight: 600;
                color: #374151;
                border-bottom: 1px solid #e5e7eb;
                text-align: left;
            }

            .attendance-grid td {
                padding: 12px;
                font-size: 13px;
                border-bottom: 1px solid #edf2f7;
            }

        .attendance-hours-low {
            color: #dc2626;
            font-weight: 700;
        }

        .attendance-hours-ok {
            color: #15803d;
            font-weight: 600;
        }

        .attendance-pager {
            padding: 14px 0 4px;
            text-align: center;
        }

            .attendance-pager table {
                margin: 0 auto;
            }

            .attendance-pager a,
            .attendance-pager span {
                display: inline-block;
                min-width: 30px;
                padding: 6px 10px;
                margin: 0 3px;
                border-radius: 6px;
                font-size: 13px;
                font-weight: 600;
                text-decoration: none;
            }

            .attendance-pager a {
                color: #374151;
                border: 1px solid #e5e7eb;
            }

                .attendance-pager a:hover {
                    background: #f8fafc;
                    border-color: #cbd5e1;
                }

            .attendance-pager span {
                color: #fff;
                background: #2563eb;
            }

        @media (max-width: 992px) {
            .page-header {
                flex-direction: column;
                align-items: stretch;
            }
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="payroll-wrap">

        <!-- Header -->

        <div class="page-header">

            <div>
                <div class="page-title">
                    Attendance Details
                </div>

                <div class="page-subtitle">
                    Employee check-in / check-out log
                </div>
            </div>

            <div class="attendance-filter">
                <asp:DropDownList ID="ddlEmployeeFilter" runat="server" CssClass="form-control"></asp:DropDownList>
                <div class="attendance-filter-date">
                    <label>From</label>
                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="attendance-filter-date">
                    <label>To</label>
                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" CausesValidation="false" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" CausesValidation="false" />
                <asp:Button ID="btnExport" runat="server" Text="Export to Excel" CssClass="btn btn-success" OnClick="btnExport_Click" CausesValidation="false" />
            </div>

        </div>

        <!-- Grid -->

        <div class="sec-card">

            <div class="sec-head">
                Attendance List
            </div>

            <asp:GridView ID="gvAttendance"
                runat="server"
                CssClass="table attendance-grid"
                AutoGenerateColumns="False"
                GridLines="None"
                EmptyDataText="No attendance records found"
                OnRowDataBound="gvAttendance_RowDataBound"
                AllowPaging="True"
                PageSize="15"
                OnPageIndexChanging="gvAttendance_PageIndexChanging"
                PagerSettings-Mode="NumericFirstLast"
                PagerSettings-Position="Bottom"
                PagerStyle-CssClass="attendance-pager">

                <Columns>

                    <asp:BoundField HeaderText="Employee Name" DataField="EmployeeName" />

                    <asp:BoundField HeaderText="Employee Code" DataField="EmployeeCode" />

                    <asp:BoundField HeaderText="Date" DataField="LoginDate" DataFormatString="{0:dd-MMM-yyyy}" />

                    <asp:TemplateField HeaderText="Check In">
                        <ItemTemplate>
                            <asp:Label ID="lblCheckIn" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Check Out">
                        <ItemTemplate>
                            <asp:Label ID="lblCheckOut" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Hrs">
                        <ItemTemplate>
                            <asp:Label ID="lblHours" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

        </div>

    </div>

</asp:Content>
