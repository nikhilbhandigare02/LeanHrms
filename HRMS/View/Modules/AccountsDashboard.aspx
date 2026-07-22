<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AccountsDashboard.aspx.cs" Inherits="HRMS.View.Modules.AccountsDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .stat-card {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 10px 20px rgba(0, 0, 0, 0.1);
            transition: transform 0.3s ease;
        }

            .stat-card:hover {
                transform: translateY(-5px);
            }

            .stat-card h3 {
                font-size: 2rem;
                font-weight: 700;
                margin: 0;
            }

            .stat-card p {
                opacity: 0.9;
                margin: 8px 0 0 0;
            }

        .section-title {
            font-size: 1.25rem;
            font-weight: 700;
            color: #172033;
            margin-bottom: 16px;
        }

        .table-container {
            background: white;
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
        }

        .stat-card .amount-row {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .stat-card .eye-btn {
            background: none;
            border: none;
            padding: 0;
            cursor: pointer;
            color: rgba(255,255,255,0.85);
            font-size: 1.1rem;
            line-height: 1;
            transition: color 0.2s;
        }

            .stat-card .eye-btn:hover {
                color: #fff;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row mb-4">
        <div class="col-12">
            <h4 class="font-size-18 mb-0">Accounts Dashboard</h4>
            <p class="text-muted mt-1">Employee-wise salary and reimbursement details</p>
        </div>
    </div>

    <div class="row mb-4">
        <div class="col-md-4">
            <div class="stat-card">
                <div class="amount-row">
                    <%--                    <h3 id="salaryAmount" data-value="&#8377;1,25,400" data-hidden="true">*****</h3>--%>
                    <h3 id="salaryAmount" runat="server" data-hidden="true">*****
                    </h3>
                    <%--<button type="button" class="eye-btn" onclick="toggleAmount('salaryAmount', this)" title="Show/Hide">
                        <i class="mdi mdi-eye"></i>
                    </button>--%>
                    <button type="button"
                        class="eye-btn"
                        onclick="toggleAmount('<%= salaryAmount.ClientID %>', this)"
                        title="Show/Hide">
                        <i class="mdi mdi-eye"></i>
                    </button>
                </div>
                <p>Total Salary Disbursed</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="stat-card" style="background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);">
                <div class="amount-row">
                    <%--                    <h3 id="reimbAmount" data-value="&#8377;12,500" data-hidden="true">*****</h3>--%>
                    <h3 id="reimbAmount" runat="server" data-hidden="true">*****
                    </h3>
                    <%--   <button type="button" class="eye-btn" onclick="toggleAmount('reimbAmount', this)" title="Show/Hide">
                        <i class="mdi mdi-eye"></i>
                    </button>--%>
                    <button type="button"
                        class="eye-btn"
                        onclick="toggleAmount('<%= reimbAmount.ClientID %>', this)"
                        title="Show/Hide">
                        <i class="mdi mdi-eye"></i>
                    </button>
                </div>
                <p>Total Reimbursements</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="stat-card" style="background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);">
                <h3>
                    <asp:Literal ID="litActiveEmployeeCount" runat="server"></asp:Literal>
                </h3>
                <p>Active Employees</p>
            </div>
        </div>
    </div>

    <div class="row mb-4">
        <div class="col-lg-12">
            <div class="table-container">
                <h5 class="section-title">Employee-wise Salary Details</h5>
                <div class="table-responsive">
                    <table class="table table-nowrap mb-0">
                        <thead class="table-light">
                            <tr>
                                <th scope="col">Employee Name</th>
                                <th scope="col">Employee Code</th>
                                <th scope="col">Basic Salary</th>
                                <th scope="col">Allowances</th>
                                <th scope="col">Deductions</th>
                                <th scope="col">Net Salary</th>
                                <th scope="col">Status</th>
                            </tr>
                        </thead>
                        <%--   <tbody>
                            <tr>
                                <td>Meera Sharma</td>
                                <td>EMP001</td>
                                <td>&#8377;30,000</td>
                                <td>&#8377;10,000</td>
                                <td>&#8377;3,000</td>
                                <td>&#8377;37,000</td>
                                <td><span class="badge bg-success">Paid</span></td>
                            </tr>
                            <tr>
                                <td>Raj Patel</td>
                                <td>EMP002</td>
                                <td>&#8377;25,000</td>
                                <td>&#8377;8,000</td>
                                <td>&#8377;2,500</td>
                                <td>&#8377;30,500</td>
                                <td><span class="badge bg-primary">Pending</span></td>
                            </tr>
                            <tr>
                                <td>Priya Singh</td>
                                <td>EMP003</td>
                                <td>&#8377;28,000</td>
                                <td>&#8377;9,000</td>
                                <td>&#8377;2,800</td>
                                <td>&#8377;34,200</td>
                                <td><span class="badge bg-success">Paid</span></td>
                            </tr>
                        </tbody>--%>
                        <tbody>
                            <asp:Repeater ID="rptEmployeeSalary" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("username") %></td>
                                        <td><%# Eval("employeecode") %></td>


                                        <td>&#8377;<%# Convert.ToDecimal(Eval("basic_salary")).ToString("N2") %></td>


                                        <td>&#8377;<%# Convert.ToDecimal(Eval("special_allowance")).ToString("N2") %></td>



                                        <td>&#8377;<%# Convert.ToDecimal(Eval("total_deductions")).ToString("N2") %></td>



                                        <td>&#8377;<%# Convert.ToDecimal(Eval("net_pay")).ToString("N2") %></td>

                                        <td>
                                            <span class='badge <%# Eval("status").ToString().ToLower() == "paid" ? "bg-success" : "bg-primary" %>'>
                                                <%# Eval("status") %>
                                            </span>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                <asp:Label ID="lblNoSalaryData"
                    runat="server"
                    CssClass="text-center d-block mt-3"
                    ForeColor="red"
                    Visible="false"
                    Text="No data found for this month.">
            </asp:Label>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="table-container">
                <h5 class="section-title">Employee-wise Reimbursement Details</h5>
                <div class="table-responsive">
                    <table class="table table-nowrap mb-0">
                        <thead class="table-light">
                            <tr>
                                <th scope="col">Employee Name</th>
                                <th scope="col">Employee Code</th>
                                <th scope="col">Reimbursement Type</th>
                                <th scope="col">Amount</th>
                                <th scope="col">Date</th>
                                <th scope="col">Status</th>
                            </tr>
                        </thead>
                        <%--             <tbody>
                            <tr>
                                <td>Meera Sharma</td>
                                <td>EMP001</td>
                                <td>Travel</td>
                                <td>&#8377;3,500</td>
                                <td>10 Jul 2024</td>
                                <td><span class="badge bg-success">Approved</span></td>
                            </tr>
                            <tr>
                                <td>Raj Patel</td>
                                <td>EMP002</td>
                                <td>Meal</td>
                                <td>&#8377;1,500</td>
                                <td>12 Jul 2024</td>
                                <td><span class="badge bg-warning">Pending</span></td>
                            </tr>
                            <tr>
                                <td>Priya Singh</td>
                                <td>EMP003</td>
                                <td>Office Supplies</td>
                                <td>&#8377;2,000</td>
                                <td>08 Jul 2024</td>
                                <td><span class="badge bg-danger">Rejected</span></td>
                            </tr>
                        </tbody>--%>
                        <tbody>
                            <asp:Repeater ID="rptEmployeeReimbursement" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("employee_name") %></td>
                                        <td><%# Eval("employee_code") %></td>
                                        <td><%# Eval("claim_type") %></td>
                                        <td>&#8377;<%# Convert.ToDecimal(Eval("claim_amount")).ToString("N2") %></td>
                                        <td><%# Convert.ToDateTime(Eval("claim_date")).ToString("dd MMM yyyy") %></td>

                                        <td>
                                            <span class="badge <%# GetStatusClass(Eval("status").ToString()) %>">
                                                <%# Eval("status") %>
                                            </span>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                <asp:Label ID="lblnoreimbdata"
                    runat="server"
                    CssClass="text-center d-block mt-3"
                    ForeColor="red"
                    Visible="false"
                    Text="No data found for this month.">
                </asp:Label>
            </div>
        </div>
    </div>

    <script>
        function toggleAmount(id, btn) {
            var el = document.getElementById(id);
            console.log("Element:", el);
            console.log("Data Value:", el.getAttribute("data-value"));
            var icon = btn.querySelector('i');
            var isHidden = el.getAttribute('data-hidden') === 'true';
            if (isHidden) {
                el.innerHTML = el.getAttribute('data-value');
                el.setAttribute('data-hidden', 'false');
                icon.classList.remove('mdi-eye');
                icon.classList.add('mdi-eye-off');
            } else {
                el.innerHTML = '*****';
                el.setAttribute('data-hidden', 'true');
                icon.classList.remove('mdi-eye-off');
                icon.classList.add('mdi-eye');
            }
        }
    </script>
</asp:Content>
