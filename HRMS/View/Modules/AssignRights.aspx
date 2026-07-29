<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AssignRights.aspx.cs" Inherits="HRMS.View.Modules.AssignRights" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
    <style>
        .rights-section {
            border: 1px solid #e2e8f0;
            border-radius: 8px;
            padding: 16px;
            margin-bottom: 20px;
            background-color: #f8fafc;
        }
        
        .checkbox-list-container {
            padding: 8px;
            background-color: white;
            border: 1px solid #e2e8f0;
            border-radius: 4px;
        }
        
        .select-all-container {
            margin-bottom: 12px;
            padding: 10px;
            background-color: #f1f5f9;
            border-radius: 4px;
            font-weight: 500;
        }
        
       .checkbox-list-container label {
    display: inline-block;
    font-weight: normal;
    margin-bottom: 4px;
    vertical-align: middle;
        margin-left: 4px;

}

       .checkbox-list-container table {
    width: 100%;
    border-collapse: separate;
    border-spacing: 50px 12px;
}

.checkbox-list-container td {
    padding-right: 50px;
    vertical-align: top;
    white-space: nowrap;
}

.checkbox-list-container input[type="checkbox"] {
    margin-right: 6px;
}

       .checkbox-list-container input[type="checkbox"] {
    vertical-align: middle;
    margin-right: 6px;
}

        
        .checkbox-list-container label:hover {
            background-color: #f1f5f9;
        }
        
       .form-group > label {
    font-weight: 600;
    color: #334155;
    margin-bottom: 8px;
    display: block;
}
        
        .btn-group {
            display: flex;
            gap: 12px;
            margin-top: 16px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-lg-12">
            <div class="card">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                          <asp:Label runat="server" ID="lbluser" CssClass="card-title mb-4" Style="font-size: 2.0em; font-weight: bold;">Assign Rights</asp:Label>
                        <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary" Text="Back" OnClick="btnBack_Click" />
                    </div>
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="form-group mb-4">
                                <label for="input-roles">User Role</label>
                                <div class="mb-3 position-relative">
                                    <asp:DropDownList ID="ddlrole" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlrole_SelectedIndexChanged">
                                        <asp:ListItem Text="Please select" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfv_ddlrole" runat="server" ControlToValidate="ddlrole" InitialValue="0" ErrorMessage="User Role is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-lg-12">
                        <div class="rights-section">
                            <div class="form-group mb-0">
                                <label>Select Menu</label>
                                <div class="select-all-container">
                                    <input type="checkbox" id="selectAllMenus" onclick="toggleSelectAllMenus(this.checked)" />
                                    <label for="selectAllMenus" style="display: inline; margin-left: 6px; cursor: pointer;">Select All</label>
                                </div>
                                <div class="checkbox-list-container">
                                    <asp:CheckBoxList runat="server" CssClass="" ID="cbxMenu" AutoPostBack="true" OnSelectedIndexChanged="cbxMenu_SelectedIndexChanged" RepeatColumns="3" RepeatLayout="Table">
                                    </asp:CheckBoxList>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-lg-12">
                        <div id="lbl_submenu" runat="server" visible="false" class="rights-section">
                            <div class="form-group mb-0">
                                <label>Submenu</label>
                                <div class="select-all-container">
                                    <input type="checkbox" id="selectAllSubmenus" onclick="toggleSelectAll('<%= cbx_submenu.ClientID %>', this.checked)" />
                                    <label for="selectAllSubmenus" style="display: inline; margin-left: 6px; cursor: pointer;">Select All</label>
                                </div>
                                <div class="checkbox-list-container">
                                    <asp:CheckBoxList ID="cbx_submenu" runat="server" CssClass="" RepeatColumns="3" RepeatLayout="Table">
                                    </asp:CheckBoxList>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <div class="btn-group">
                        <asp:Button ID="btn_submit" runat="server" CssClass="btn btn-success" Text="Submit" CommandArgument="Submit" OnClick="btn_submit_Click" ValidationGroup="SaveValidationGroup" />
                        <asp:Button ID="btn_reset" runat="server" CssClass="btn btn-primary" Text="Reset"  OnClick="btn_reset_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <script>
        function showRightSavedMessage(status, remark) {
            Swal.fire({
                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }
        
        function toggleSelectAll(checkBoxListId, isChecked) {
            var checkBoxList = document.getElementById(checkBoxListId);
            if (checkBoxList) {
                var checkboxes = checkBoxList.getElementsByTagName('input');
                for (var i = 0; i < checkboxes.length; i++) {
                    checkboxes[i].checked = isChecked;
                }
            }
        }

        function toggleSelectAllMenus(isChecked) {
            toggleSelectAll('<%= cbxMenu.ClientID %>', isChecked);
            // Force a postback so cbxMenu_SelectedIndexChanged runs and the
            // submenu list opens (when checked) or hides (when unchecked),
            // same as it does for a manual checkbox click.
            __doPostBack('<%= cbxMenu.UniqueID %>', '');
        }

        // Keeps a "Select All" checkbox in sync with its list's actual state.
        // Every postback re-renders the whole page, so without this the plain
        // HTML "Select All" checkbox always resets to unchecked and a second
        // click would select-all again instead of unselecting.
        function syncSelectAllState(checkBoxListId, selectAllId) {
            var checkBoxList = document.getElementById(checkBoxListId);
            var selectAll = document.getElementById(selectAllId);
            if (checkBoxList && selectAll) {
                var checkboxes = checkBoxList.getElementsByTagName('input');
                var allChecked = checkboxes.length > 0;
                for (var i = 0; i < checkboxes.length; i++) {
                    if (!checkboxes[i].checked) {
                        allChecked = false;
                        break;
                    }
                }
                selectAll.checked = allChecked;
            }
        }
    </script>
    <script type="text/javascript">
        window.onload = function () {
            var dropdownId = '<%= ddlrole.ClientID %>';
            var ddl = document.getElementById(dropdownId);
            if (ddl && ddl.options.length > 0) {
                ddl.options[0].disabled = true;
                ddl.options[0].style.color = 'gray';
            }

            syncSelectAllState('<%= cbxMenu.ClientID %>', 'selectAllMenus');
            syncSelectAllState('<%= cbx_submenu.ClientID %>', 'selectAllSubmenus');
        };
</script>
</asp:Content>

