DELIMITER $$

DROP PROCEDURE IF EXISTS `lean_mngt`.`SP_SaveEmployeeOnboarding`$$

CREATE DEFINER=`scadmin`@`%` PROCEDURE `lean_mngt`.`SP_SaveEmployeeOnboarding`
(
    IN p_user_id INT,
    IN p_emp_code VARCHAR(50),
    IN p_username VARCHAR(100),
    IN p_fullname VARCHAR(150),
    IN p_email VARCHAR(150),
    IN p_contact VARCHAR(20),

    IN p_gender VARCHAR(50),
    IN p_dob DATE,
    IN p_marital_status VARCHAR(50),
    IN p_blood_group VARCHAR(50),
    IN p_nationality VARCHAR(100),

    IN p_employment_type VARCHAR(50),
    IN p_employee_category VARCHAR(50),
    IN p_joining_date DATE,
    IN p_probation_months INT,
    IN p_employee_status VARCHAR(50),
    IN p_notice_period INT,
    IN p_exit_date DATE,
    IN p_separation_reason VARCHAR(100),

    IN p_company VARCHAR(100),
    IN p_department VARCHAR(100),
    IN p_branch_office VARCHAR(100),
    IN p_location VARCHAR(100),
    IN p_reporting_manager_id INT,
    IN p_functional_manager_id INT,
    IN p_hod_id INT,
    IN p_designation VARCHAR(100),
    IN p_employee_level VARCHAR(50),

    IN p_salary_structure VARCHAR(100),
    IN p_basic_salary DECIMAL(18,2),
    IN p_gross_salary DECIMAL(18,2),
    IN p_ctc DECIMAL(18,2),
    IN p_salary_effective_date DATE,
    IN p_payment_mode VARCHAR(50),
    IN p_uan_number VARCHAR(50),
    IN p_pf_number VARCHAR(50),
    IN p_esic_number VARCHAR(50),
    IN p_professional_tax_number VARCHAR(50),
    IN p_labour_welfare_fund_number VARCHAR(50),
    IN p_tax_regime VARCHAR(50),
    IN p_tds_applicable VARCHAR(10),

    IN p_asset_type VARCHAR(100),
    IN p_asset_number VARCHAR(100),
    IN p_asset_name VARCHAR(150),
    IN p_assigned_date DATE,
    IN p_return_date DATE,
    IN p_asset_condition VARCHAR(50),
    IN p_asset_status VARCHAR(50),

    IN p_inserted_by INT
)
BEGIN
    DECLARE v_user_id INT;

    IF p_user_id IS NULL OR p_user_id = 0 THEN

        INSERT INTO lean_mngt.userm
        (
            emp_code, username, user_fullname, user_mail_id, contact_detail,
            gender, DOB, marital_status, blood_group, nationality,
            employment_type, employee_type, date_of_joining,
            probation_period_months, employee_status, notice_period,
            exit_date, separation_reason,
            is_active, inserted_by, inserted_date
        )
        VALUES
        (
            p_emp_code, p_username, p_fullname, p_email, p_contact,
            p_gender, p_dob, p_marital_status, p_blood_group, p_nationality,
            p_employment_type, p_employee_category, p_joining_date,
            p_probation_months, p_employee_status, p_notice_period,
            p_exit_date, p_separation_reason,
            1, p_inserted_by, NOW()
        );

        SET v_user_id = LAST_INSERT_ID();

    ELSE
        SET v_user_id = p_user_id;
    END IF;

    INSERT INTO lean_mngt.employee_organization
    (
        employee_id, company, department, branch_office, location,
        reporting_manager_id, functional_manager_id, hod_id,
        designation, employee_level, created_date
    )
    VALUES
    (
        v_user_id, p_company, p_department, p_branch_office, p_location,
        p_reporting_manager_id, p_functional_manager_id, p_hod_id,
        p_designation, p_employee_level, NOW()
    );

    INSERT INTO lean_mngt.employee_salary_information
    (
        user_id, salary_structure, basic_salary, gross_salary, ctc,
        salary_effective_date, payment_mode, uan_number, pf_number,
        esic_number, professional_tax_number, labour_welfare_fund_number,
        tax_regime, tds_applicable,
        inserted_by, inserted_date, is_active
    )
    VALUES
    (
        v_user_id, p_salary_structure, p_basic_salary, p_gross_salary, p_ctc,
        p_salary_effective_date, p_payment_mode, p_uan_number, p_pf_number,
        p_esic_number, p_professional_tax_number, p_labour_welfare_fund_number,
        p_tax_regime, p_tds_applicable,
        p_inserted_by, NOW(), 1
    );

    IF p_asset_type IS NOT NULL AND p_asset_type <> '' THEN
        INSERT INTO lean_mngt.employee_assets
        (
            user_id, asset_type, asset_number, asset_name,
            assigned_date, return_date, asset_condition, asset_status,
            inserted_by, inserted_date, is_active
        )
        VALUES
        (
            v_user_id, p_asset_type, p_asset_number, p_asset_name,
            p_assigned_date, p_return_date, p_asset_condition, p_asset_status,
            p_inserted_by, NOW(), 1
        );
    END IF;

    SELECT
        'Success' AS Status,
        'Employee onboarding saved successfully' AS Message,
        v_user_id AS UserId;

END$$

DELIMITER ;
