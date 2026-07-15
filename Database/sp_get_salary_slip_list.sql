-- Returns saved salary slips for an employee within a month range.
-- Source: salary_slip_details (keyed by employeecode; user_id/year are not reliably populated).
-- Returns one active row per month (latest by id). year is matched leniently because many
-- rows carry year = 0 / NULL.
-- Consumed by ProcessModel.SalarySlipBL.GetSalarySlipList / View\Modules\SalarySlip.aspx.

DELIMITER $$

DROP PROCEDURE IF EXISTS `alpha_hrms`.`sp_get_salary_slip_list`$$

CREATE PROCEDURE `alpha_hrms`.`sp_get_salary_slip_list`
(
    IN p_employee_code INT,
    IN p_year          INT,
    IN p_from_month    INT,
    IN p_to_month      INT
)
BEGIN
    SELECT
        t.salary_slip_details_id,
        t.employeecode,
        t.username,
        t.month,
        t.year,
        t.designation_name,
        t.department,
        t.days_paid,
        t.basic_salary,
        t.house_rent_allowance,
        t.special_allowance,
        t.leave_travel_allowance,
        t.Bonus,
        t.Incentive,
        t.Others,
        t.professional_tax,
        t.total_earnings,
        t.total_deductions,
        t.net_pay
    FROM salary_slip_details t
    INNER JOIN (
        SELECT employeecode, month, MAX(salary_slip_details_id) AS latest_id
        FROM salary_slip_details
        WHERE employeecode = p_employee_code
          AND CAST(is_active AS UNSIGNED) = 1
          AND CAST(month AS UNSIGNED) BETWEEN p_from_month AND p_to_month
          AND (year = p_year OR year = 0 OR year IS NULL)
        GROUP BY employeecode, month
    ) latest ON latest.latest_id = t.salary_slip_details_id
    ORDER BY CAST(t.month AS UNSIGNED);
END$$

DELIMITER ;
