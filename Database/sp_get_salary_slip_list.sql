-- Returns saved salary slips for an employee within a month range (single year).
-- Consumed by ProcessModel.SalarySlipBL.GetSalarySlipList / View\Modules\SalarySlip.aspx.
-- NOTE: adjust the table/column names below if your salary data lives elsewhere.

DELIMITER $$

DROP PROCEDURE IF EXISTS `alpha_hrms`.`sp_get_salary_slip_list`$$

CREATE PROCEDURE `alpha_hrms`.`sp_get_salary_slip_list`
(
    IN p_user_id    INT,
    IN p_year       INT,
    IN p_from_month INT,
    IN p_to_month   INT
)
BEGIN
    -- One row per month: the latest saved record (highest id) for that user/month/year.
    SELECT
        t.user_id,
        t.employeeCode,
        t.employeeName,
        t.month,
        t.year,
        t.designation,
        t.DaysPaid,
        t.BasicSalary,
        t.HouseRentAllowance,
        t.SpecialAllowance,
        t.LeaveTravelAllowance,
        t.ProfessionalTax,
        t.TotalEarnings,
        t.TotalDeductions,
        t.NetPay
    FROM employee_salary_master t
    INNER JOIN (
        SELECT user_id, month, year, MAX(emp_salary_master_id) AS latest_id
        FROM employee_salary_master
        WHERE user_id = p_user_id
          AND year = p_year
          AND month BETWEEN p_from_month AND p_to_month
        GROUP BY user_id, month, year
    ) latest ON latest.latest_id = t.emp_salary_master_id
    ORDER BY t.month;
END$$

DELIMITER ;
