<%@ Page Title="Approval Configuration" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="LeaveConfiguration.aspx.cs" Inherits="HRMS.View.Modules.LeaveConfiguration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .cfg-wrap { max-width: 1100px; margin: 0 auto; }
        .cfg-title { margin: 0; font-size: 28px; font-weight: 700; color: #1e293b; }
        .cfg-card { margin-top: 12px; background: #fff; border: 1px solid #e2e8f0; border-radius: 12px; box-shadow: 0 6px 18px rgba(15,23,42,0.06); padding: 14px; }
        .cfg-table th { background: #f8fafc; font-size: 12px; color: #334155; font-weight: 700; }
        .cfg-table td { vertical-align: middle; }
        .cfg-input { min-height: 36px; border: 1px solid #cbd5e1; border-radius: 8px; font-size: 13px; }
        .cfg-btn { border-radius: 8px; min-width: 80px; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="cfg-wrap">
        <h2 class="cfg-title">Approval Configuration</h2>

        <div class="cfg-card">
            <div class="table-responsive">
                <asp:GridView ID="gvInlineConfig" runat="server" CssClass="table table-bordered align-middle cfg-table"
                    AutoGenerateColumns="false"
                    OnRowDataBound="gvInlineConfig_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Type">
                            <ItemTemplate>
                                <asp:HiddenField ID="hfDayId" runat="server" />
                                <asp:HiddenField ID="hfTypeId" runat="server" />
                                <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control cfg-input" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Approval Option">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlApprovalOption" runat="server" CssClass="form-control cfg-input"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlApprovalOption_SelectedIndexChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Approval Days">
                            <ItemTemplate>
                                <asp:TextBox ID="txtApprovalDays" runat="server" CssClass="form-control cfg-input" TextMode="Number" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:Button ID="btnAction" runat="server" Text="Save" OnClick="btnAction_Click" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <div class="d-flex justify-content-between align-items-center">
                <asp:Label ID="lblMsg" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>
