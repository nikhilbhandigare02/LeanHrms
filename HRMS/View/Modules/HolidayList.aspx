<%@ Page Title="Holiday List" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="HolidayList.aspx.cs" Inherits="HRMS.View.Modules.HolidayList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --holiday-primary: #2563EB;
            --holiday-primary-dark: #1D4ED8;
            --holiday-primary-soft: #EFF6FF;
            --holiday-border: #DBE4F0;
            --holiday-text: #10213F;
            --holiday-muted: #64748B;
            --holiday-bg: #F6F9FE;
            --holiday-danger: #EF4444;
            --holiday-success: #16A34A;
        }

        body {
            background: var(--holiday-bg) !important;
        }

        .holiday-page {
            color: var(--holiday-text);
            max-width: 1380px;
            margin: 0 auto;
            padding: 8px 18px 28px;
        }

        .holiday-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 16px;
            margin-bottom: 22px;
        }

        .holiday-title-wrap {
            display: flex;
            align-items: center;
            gap: 14px;
        }

        .holiday-icon {
            width: 48px;
            height: 48px;
            border-radius: 12px;
            background: var(--holiday-primary-soft);
            color: var(--holiday-primary);
            display: inline-flex;
            align-items: center;
            justify-content: center;
            box-shadow: inset 0 0 0 1px #DBEAFE;
        }

        .holiday-icon i {
            font-size: 24px;
        }

        .holiday-title {
            margin: 0;
            font-size: 26px;
            font-weight: 800;
            color: #071733;
            letter-spacing: 0;
        }

        .holiday-subtitle {
            margin: 4px 0 0;
            color: #425B7C;
            font-size: 13px;
            font-weight: 600;
        }

        .holiday-actions {
            display: flex;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
            justify-content: flex-end;
        }

        .holiday-shell {
            background: #fff;
            border: 1px solid var(--holiday-border);
            border-radius: 8px;
            box-shadow: 0 12px 30px rgba(15, 23, 42, 0.05);
            padding: 22px;
        }

        .holiday-toolbar {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 16px;
            margin-bottom: 22px;
        }

        .holiday-year-group {
            display: flex;
            align-items: center;
            gap: 16px;
            flex-wrap: wrap;
        }

        .holiday-year-label {
            color: #24324A;
            font-size: 13px;
            font-weight: 800;
            white-space: nowrap;
        }

        .holiday-year-group .holiday-control {
            min-width: 230px;
        }

        .holiday-reset-wrap {
            margin-left: auto;
            display: inline-flex;
            gap: 12px;
            align-items: center;
            flex-wrap: wrap;
        }

        .holiday-upload-wrap {
            position: relative;
            display: inline-flex;
            align-items: center;
        }

        .holiday-upload-wrap i {
            position: absolute;
            left: 18px;
            z-index: 1;
            color: #fff;
            font-size: 18px;
            pointer-events: none;
        }

        .holiday-upload-wrap .holiday-btn {
            padding-left: 46px;
        }

        .holiday-control {
            height: 44px;
            border: 1px solid #D6DEE9;
            border-radius: 8px;
            background: #fff;
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 0 14px;
            color: var(--holiday-muted);
        }

        .holiday-control i {
            font-size: 18px;
            color: var(--holiday-primary);
        }

        .holiday-control input {
            border: 0;
            outline: 0;
            width: 100%;
            color: var(--holiday-text);
            background: transparent;
            font-size: 14px;
        }

        .holiday-control select {
            border: 0;
            outline: 0;
            width: 100%;
            color: var(--holiday-text);
            background: transparent;
            font-size: 14px;
            appearance: auto;
        }

        .holiday-year-picker {
            position: relative;
            width: 100%;
        }

        .holiday-year-picker select {
            display: none;
        }

        .holiday-year-trigger {
            align-items: center;
            background: transparent;
            border: 0;
            color: var(--holiday-text);
            display: flex;
            font-size: 14px;
            height: 100%;
            justify-content: space-between;
            outline: 0;
            padding: 0;
            text-align: left;
            width: 100%;
        }

        .holiday-year-trigger:after {
            border-color: #475569 transparent transparent transparent;
            border-style: solid;
            border-width: 5px 4px 0 4px;
            content: "";
            margin-left: 10px;
        }

        .holiday-year-menu {
            background: #FFFFFF;
            border: 1px solid #CBD5E1;
            border-radius: 8px;
            box-shadow: 0 16px 34px rgba(15, 23, 42, .14);
            display: none;
            left: 0;
            list-style: none;
            margin: 6px 0 0;
            max-height: 220px;
            overflow-y: auto;
            padding: 5px;
            position: absolute;
            top: 100%;
            width: 100%;
            z-index: 9999;
        }

        .holiday-year-picker.open .holiday-year-menu {
            display: block;
        }

        .holiday-year-option {
            border-radius: 6px;
            color: #0F172A;
            cursor: pointer;
            font-size: 14px;
            padding: 9px 12px;
        }

        .holiday-year-option:hover,
        .holiday-year-option.selected {
            background: var(--holiday-primary-soft);
            color: var(--holiday-primary);
            font-weight: 800;
        }

        .holiday-btn {
            min-height: 44px;
            border-radius: 8px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            font-weight: 800;
            padding: 0 18px;
            border: 1px solid var(--holiday-border);
            white-space: nowrap;
            transition: all .15s ease;
        }

        .holiday-btn-primary {
            background: var(--holiday-primary);
            border-color: var(--holiday-primary);
            color: #fff;
            box-shadow: 0 12px 22px rgba(37, 99, 235, 0.18);
        }

        .holiday-btn-primary:hover,
        .holiday-btn-primary:focus {
            background: var(--holiday-primary-dark);
            border-color: var(--holiday-primary-dark);
            color: #fff;
        }

        .holiday-btn-light {
            background: #fff;
            color: var(--holiday-primary);
        }

        .holiday-btn-light:hover,
        .holiday-btn-light:focus {
            color: var(--holiday-primary-dark);
            border-color: #BFDBFE;
            background: var(--holiday-primary-soft);
        }

        .holiday-upload {
            position: relative;
            overflow: hidden;
        }

        .holiday-file {
            width: 1px;
            height: 1px;
            opacity: 0;
            overflow: hidden;
            position: absolute;
            pointer-events: none;
        }

        .holiday-choose-file {
            color: var(--holiday-primary);
        }

        .holiday-message {
            display: block;
            margin-bottom: 14px;
            font-weight: 500;
        }

        .holiday-table-wrap {
            border: 1px solid var(--holiday-border);
            border-radius: 8px;
            overflow: auto;
        }

        .holiday-table {
            margin: 0;
            min-width: 820px;
        }

        .holiday-table th {
            background: #F8FAFF;
            color: #10213F;
            font-weight: 800;
            padding: 16px 18px !important;
            border-color: #E6EDF6 !important;
            font-size: 13px;
        }

        .holiday-table th:first-child,
        .holiday-table td:first-child {
            width: 105px;
            text-align: center;
        }

        .holiday-table th:last-child,
        .holiday-table td:last-child {
            text-align: center;
        }

        .holiday-table td {
            padding: 15px 18px !important;
            border-color: #EEF3F9 !important;
            vertical-align: middle !important;
            color: #1D3151;
            font-size: 14px;
            height: 58px;
        }

        .holiday-th-icon {
            display: inline-flex;
            align-items: center;
            gap: 10px;
        }

        .holiday-th-icon i {
            color: var(--holiday-primary);
            font-size: 16px;
        }

        .holiday-table tr:hover td {
            background: #F8FAFF;
        }

        .holiday-date {
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .holiday-date i {
            color: var(--holiday-primary);
            font-size: 19px;
        }

        .holiday-day-pill {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-width: 76px;
            min-height: 30px;
            padding: 4px 12px;
            border-radius: 999px;
            color: #0F766E;
            background: #ECFEFF;
            border: 1px solid #CFFAFE;
            font-weight: 800;
            font-size: 12px;
        }

        .holiday-day-pill.day-monday {
            color: #1769d1;
            background: #eaf3ff;
            border-color: #cfe4ff;
        }

        .holiday-day-pill.day-tuesday {
            color: #4d46d6;
            background: #eeeeff;
            border-color: #d9d6ff;
        }

        .holiday-day-pill.day-wednesday {
            color: #0f766e;
            background: #e6fffb;
            border-color: #b8f1ea;
        }

        .holiday-day-pill.day-thursday {
            color: #db2777;
            background: #fff0f6;
            border-color: #ffd6e7;
        }

        .holiday-day-pill.day-friday {
            color: #f97316;
            background: #fff7ed;
            border-color: #fed7aa;
        }

        .holiday-day-pill.day-saturday {
            color: #0f9f4f;
            background: #e6f7ec;
            border-color: #d1f0dc;
        }

        .holiday-day-pill.day-sunday {
            color: #b42318;
            background: #fff1f0;
            border-color: #ffd6d2;
        }

        .holiday-row-actions {
            display: flex;
            gap: 10px;
            align-items: center;
            flex-wrap: wrap;
            justify-content: center;
        }

        .holiday-download-small {
            min-height: 44px;
            padding: 0 18px;
            font-size: 13px;
            box-shadow: none;
        }

        .holiday-download-small i {
            font-size: 18px;
        }

        .holiday-action-edit,
        .holiday-action-delete,
        .holiday-action-save,
        .holiday-action-cancel {
            border-radius: 8px;
            min-height: 34px;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 0 12px;
            font-weight: 800;
            border: 1px solid transparent;
            text-decoration: none !important;
            font-size: 12px;
        }

        .holiday-action-edit {
            color: var(--holiday-primary) !important;
            background: var(--holiday-primary-soft);
            border-color: #DBEAFE;
        }

        .holiday-action-delete {
            color: var(--holiday-danger) !important;
            background: #FEF2F2;
            border-color: #FECACA;
        }

        .holiday-action-save {
            color: var(--holiday-success) !important;
            background: #F0FDF4;
            border-color: #BBF7D0;
        }

        .holiday-action-cancel {
            color: #64748B !important;
            background: #F8FAFC;
            border-color: #E2E8F0;
        }

        .holiday-edit-input {
            min-width: 140px;
            border-radius: 8px;
            border-color: #D6DEE9;
        }

        .holiday-empty {
            padding: 28px;
            text-align: center;
            color: var(--holiday-muted);
        }

        @media (max-width: 991px) {
            .holiday-header {
                align-items: flex-start;
                flex-direction: column;
            }

            .holiday-actions {
                width: 100%;
                justify-content: flex-start;
            }

            .holiday-toolbar {
                align-items: stretch;
                flex-direction: column;
            }

            .holiday-reset-wrap {
                margin-left: 0;
            }

            .holiday-year-group .holiday-control {
                min-width: 100%;
            }

            .holiday-shell {
                padding: 16px;
            }

            .holiday-page {
                padding-left: 10px;
                padding-right: 10px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="holiday-page">
        <div class="holiday-header">
            <div class="holiday-title-wrap">
                <div class="holiday-icon">
                    <i class="bx bx-calendar-event"></i>
                </div>
                <div>
                    <h1 class="holiday-title">Holiday List</h1>
                    <p class="holiday-subtitle">Manage company holidays</p>
                </div>
            </div>
            <div class="holiday-actions">
                <asp:FileUpload ID="fileUpload" runat="server" CssClass="holiday-file" />
                <label class="holiday-btn holiday-btn-light holiday-choose-file" for="<%= fileUpload.ClientID %>">
                    <i class="bx bx-upload"></i>
                    <span>Choose File</span>
                </label>
                <span class="holiday-upload-wrap">
                    <i class="bx bx-upload"></i>
                    <asp:Button ID="btnUpload" runat="server" Text="Upload Excel" CssClass="holiday-btn holiday-btn-primary" OnClick="btnUpload_Click" />
                </span>
            </div>
        </div>

        <div class="holiday-shell">
            <asp:Label ID="lblMessage" runat="server" CssClass="holiday-message text-success"></asp:Label>
            <asp:Label ID="lblError" runat="server" CssClass="holiday-message text-danger"></asp:Label>

            <div class="holiday-toolbar">
                <div class="holiday-year-group">
                    <span class="holiday-year-label">Select Year</span>
                    <label class="holiday-control" for="holidaySearchYear">
                        <i class="bx bx-calendar"></i>
                        <span class="holiday-year-picker" id="holidayYearPicker">
                            <select id="holidaySearchYear" onchange="filterHolidayRows();">
                            </select>
                        </span>
                    </label>
                </div>
                <div class="holiday-reset-wrap">
                    <button type="button" class="holiday-btn holiday-btn-light" onclick="resetHolidayFilters();">
                        <i class="bx bx-reset"></i>
                        <span>Reset</span>
                    </button>
                    <button type="button" class="holiday-btn holiday-btn-light holiday-download-small" onclick="downloadHolidayData();">
                        <i class="bx bx-download"></i>
                        <span>Download Excel</span>
                    </button>
                </div>
            </div>

            <div class="holiday-table-wrap">
                <asp:HiddenField ID="hdnHolidayScrollY" runat="server" ClientIDMode="Static" />
                <asp:GridView ID="gvHolidayList" runat="server" AutoGenerateColumns="False" CssClass="table holiday-table"
                        DataKeyNames="holiday_id" OnRowEditing="gvHolidayList_RowEditing" OnRowCancelingEdit="gvHolidayList_RowCancelingEdit"
                        OnRowUpdating="gvHolidayList_RowUpdating" OnRowCommand="gvHolidayList_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="Sr No.">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span class="holiday-th-icon"><i class="bx bx-calendar"></i>Date</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <span class="holiday-date" data-date="<%# Convert.ToDateTime(Eval("holiday_date")).ToString("yyyy-MM-dd") %>">
                                        <%# Convert.ToDateTime(Eval("holiday_date")).ToString("dd-MM-yyyy") %>
                                    </span>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtDate" runat="server" CssClass="form-control holiday-edit-input" Text='<%# Convert.ToDateTime(Eval("holiday_date")).ToString("yyyy-MM-dd") %>' TextMode="Date"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span class="holiday-th-icon"><i class="bx bx-calendar"></i>Day</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <span class="holiday-day-pill"><%# Eval("holiday_day") %></span>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtDay" runat="server" CssClass="form-control holiday-edit-input" Text='<%# Eval("holiday_day") %>'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span class="holiday-th-icon"><i class="bx bx-party"></i>Holiday</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <%# Eval("holiday_name") %>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtHoliday" runat="server" CssClass="form-control holiday-edit-input" Text='<%# Eval("holiday_name") %>'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <span class="holiday-th-icon"><i class="bx bx-cog"></i>Action</span>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="holiday-row-actions">
                                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CssClass="holiday-action-edit" OnClientClick="saveHolidayScrollPosition();"><i class="bx bx-edit"></i> Edit</asp:LinkButton>
                                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="SoftDelete" CommandArgument='<%# Eval("holiday_id") %>' CssClass="holiday-action-delete" OnClientClick="saveHolidayScrollPosition(); return confirm('Are you sure you want to delete this holiday?');"><i class="bx bx-trash"></i> Delete</asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <div class="holiday-row-actions">
                                        <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" CssClass="holiday-action-save" OnClientClick="saveHolidayScrollPosition();"><i class="bx bx-check"></i> Update</asp:LinkButton>
                                        <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel" CssClass="holiday-action-cancel" OnClientClick="saveHolidayScrollPosition();"><i class="bx bx-x"></i> Cancel</asp:LinkButton>
                                    </div>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="holiday-empty">No active holiday records found.</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            populateHolidayYears();
            applyDayPillColors();
            restoreHolidayScrollPosition();
        });

        function saveHolidayScrollPosition() {
            var scrollField = document.getElementById('hdnHolidayScrollY');
            if (scrollField) {
                scrollField.value = window.pageYOffset || document.documentElement.scrollTop || 0;
            }
        }

        function restoreHolidayScrollPosition() {
            var scrollField = document.getElementById('hdnHolidayScrollY');
            if (!scrollField || !scrollField.value) {
                return;
            }

            var y = parseInt(scrollField.value, 10);
            if (!isNaN(y) && y > 0) {
                window.setTimeout(function () {
                    window.scrollTo(0, y);
                    scrollField.value = '';
                }, 0);
            }
        }

        function populateHolidayYears() {
            var yearSelect = document.getElementById('holidaySearchYear');
            var table = document.getElementById('<%= gvHolidayList.ClientID %>');

            if (!yearSelect) return;

            yearSelect.innerHTML = '';

            var years = [];
            if (table) {
                var rows = table.getElementsByTagName('tr');
                for (var i = 1; i < rows.length; i++) {
                    var rowDate = getHolidayRowDate(rows[i]);
                    var yearValue = rowDate.length >= 4 ? rowDate.substring(0, 4) : '';
                    if (yearValue && years.indexOf(yearValue) === -1) {
                        years.push(yearValue);
                    }
                }
            }

            years.sort();

            if (years.length === 0) {
                var option = document.createElement('option');
                option.value = '';
                option.text = 'No Years';
                yearSelect.appendChild(option);
                refreshHolidayYearPicker();
                return;
            }

            for (var j = 0; j < years.length; j++) {
                var yearOption = document.createElement('option');
                yearOption.value = years[j];
                yearOption.text = years[j];
                yearSelect.appendChild(yearOption);
            }

            yearSelect.value = years[years.length - 1];
            refreshHolidayYearPicker();
            filterHolidayRows();
        }

        function refreshHolidayYearPicker() {
            var picker = document.getElementById('holidayYearPicker');
            var select = document.getElementById('holidaySearchYear');
            if (!picker || !select) return;

            var trigger = picker.querySelector('.holiday-year-trigger');
            var menu = picker.querySelector('.holiday-year-menu');

            if (!trigger) {
                trigger = document.createElement('button');
                trigger.type = 'button';
                trigger.className = 'holiday-year-trigger';
                trigger.addEventListener('click', function (event) {
                    event.preventDefault();
                    event.stopPropagation();
                    picker.classList.toggle('open');
                });
                picker.appendChild(trigger);
            }

            if (!menu) {
                menu = document.createElement('ul');
                menu.className = 'holiday-year-menu';
                picker.appendChild(menu);
            }

            menu.innerHTML = '';
            Array.prototype.slice.call(select.options).forEach(function (option) {
                var item = document.createElement('li');
                item.className = 'holiday-year-option';
                item.textContent = option.text;
                item.setAttribute('data-value', option.value);
                if (option.value === select.value) {
                    item.classList.add('selected');
                }
                item.addEventListener('click', function (event) {
                    event.stopPropagation();
                    select.value = option.value;
                    picker.classList.remove('open');
                    refreshHolidayYearPicker();
                    filterHolidayRows();
                });
                menu.appendChild(item);
            });

            var selected = select.options[select.selectedIndex];
            trigger.textContent = selected ? selected.text : 'Select Year';
        }

        document.addEventListener('click', function () {
            var picker = document.getElementById('holidayYearPicker');
            if (picker) {
                picker.classList.remove('open');
            }
        });

        function getHolidayRowDate(row) {
            if (!row) return '';

            var dateNode = row.querySelector('[data-date]');
            if (dateNode) {
                return dateNode.getAttribute('data-date') || '';
            }

            var dateInput = row.querySelector('input[type="date"]');
            if (dateInput) {
                return dateInput.value || '';
            }

            return '';
        }

        function filterHolidayRows() {
            var searchYear = document.getElementById('holidaySearchYear').value || '';
            var table = document.getElementById('<%= gvHolidayList.ClientID %>');

            if (!table) return;

            var rows = table.getElementsByTagName('tr');
            for (var i = 1; i < rows.length; i++) {
                var row = rows[i];
                var rowDate = getHolidayRowDate(row);
                var rowYear = rowDate.length >= 4 ? rowDate.substring(0, 4) : '';
                var isEditRow = row.querySelector('input[type="date"], .holiday-edit-input') !== null;
                var yearMatches = isEditRow || !searchYear || rowYear === searchYear;

                row.style.display = yearMatches ? '' : 'none';
            }

        }

        function resetHolidayFilters() {
            populateHolidayYears();
            filterHolidayRows();
        }

        function applyDayPillColors() {
            var pills = document.querySelectorAll('.holiday-day-pill');
            for (var i = 0; i < pills.length; i++) {
                var day = (pills[i].innerText || pills[i].textContent || '').toLowerCase().trim();
                if (day) {
                    pills[i].classList.add('day-' + day);
                }
            }
        }

        function downloadHolidayData() {
            var table = document.getElementById('<%= gvHolidayList.ClientID %>');
            var csv = 'Sr No,Date,Day,Holiday\n';
            var hasRows = false;

            if (table) {
                var rows = table.getElementsByTagName('tr');
                for (var i = 1; i < rows.length; i++) {
                    var row = rows[i];
                    if (row.style.display === 'none') continue;

                    var cells = row.getElementsByTagName('td');
                    if (cells.length < 4) continue;

                    var srNo = cleanCsvValue(cells[0].innerText || cells[0].textContent);
                    var dateText = cleanCsvValue(cells[1].innerText || cells[1].textContent);
                    var day = cleanCsvValue(cells[2].innerText || cells[2].textContent);
                    var holiday = cleanCsvValue(cells[3].innerText || cells[3].textContent);
                    csv += srNo + ',' + dateText + ',' + day + ',' + holiday + '\n';
                    hasRows = true;
                }
            }

            if (!hasRows) {
                Swal.fire({ icon: 'info', title: 'No Data', text: 'No holiday records available to download.' });
                return;
            }

            var blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
            var link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.download = 'Holiday_List.csv';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }

        function cleanCsvValue(value) {
            value = (value || '').replace(/\s+/g, ' ').trim();
            value = value.replace(/"/g, '""');
            return '"' + value + '"';
        }
    </script>
</asp:Content>
