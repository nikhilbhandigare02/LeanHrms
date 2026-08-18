<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="TerminationList.aspx.cs" Inherits="HRMS.View.Modules.TerminationList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%-- Intentionally empty: Page_Load transfers execution to EmployeeAction.aspx,
         so this page has no UI of its own (see TerminationList.aspx.cs). --%>
</asp:Content>
