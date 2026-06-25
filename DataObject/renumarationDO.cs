﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class renumarationDO
    {
        public int RenumerationId { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; }
        public decimal? MonthlyCTC { get; set; }
        public decimal? YearlyCTC { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string SalaryStructureID { get; set; }
        public string Status { get; set; }
        public DateTime? EffectiveFromDate { get; set; }
        public DateTime? EffectiveToDate { get; set; }
        public string EmployeeCategory { get; set; }
        public decimal? CTCAmount { get; set; }
        public decimal? GrossSalary { get; set; }
        public decimal? MonthlySalary { get; set; }
        public decimal? AnnualSalary { get; set; }

        // Earnings Components
        public bool IsBasicSalaryEnabled { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? HRA { get; set; }
        public bool IsHRAEnabled { get; set; }
        public decimal? ConveyanceAllowance { get; set; }
        public bool IsConveyanceAllowanceEnabled { get; set; }
        public decimal? MedicalAllowance { get; set; }
        public bool IsMedicalAllowanceEnabled { get; set; }
        public decimal? SpecialAllowance { get; set; }
        public bool IsSpecialAllowanceEnabled { get; set; }
        public decimal? EducationAllowance { get; set; }
        public bool IsEducationAllowanceEnabled { get; set; }
        public decimal? TravelAllowance { get; set; }
        public bool IsTravelAllowanceEnabled { get; set; }
        public decimal? UniformAllowance { get; set; }
        public bool IsUniformAllowanceEnabled { get; set; }
        public decimal? TelephoneAllowance { get; set; }
        public bool IsTelephoneAllowanceEnabled { get; set; }
        public decimal? FoodAllowance { get; set; }
        public bool IsFoodAllowanceEnabled { get; set; }
        public decimal? ShiftAllowance { get; set; }
        public bool IsShiftAllowanceEnabled { get; set; }
        public decimal? Incentive { get; set; }
        public bool IsIncentiveEnabled { get; set; }
        public decimal? Bonus { get; set; }
        public bool IsBonusEnabled { get; set; }
        public decimal? OtherAllowance { get; set; }
        public bool IsOtherAllowanceEnabled { get; set; }
        public decimal? OtherEarnings { get; set; }

        // Deductions Components
        public bool IsPFEnabled { get; set; }
        public decimal? PF { get; set; }
        public bool IsESIEnabled { get; set; }
        public decimal? ESI { get; set; }
        public bool IsProfessionalTaxEnabled { get; set; }
        public decimal? ProfessionalTax { get; set; }
        public bool IsTDSEnabled { get; set; }
        public decimal? TDS { get; set; }
        public bool IsLabourWelfareFundEnabled { get; set; }
        public decimal? LabourWelfareFund { get; set; }
        public bool IsLoanDeductionEnabled { get; set; }
        public decimal? LoanDeduction { get; set; }
        public bool IsAdvanceRecoveryEnabled { get; set; }
        public decimal? AdvanceRecovery { get; set; }
        public bool IsOtherDeductionsEnabled { get; set; }
        public decimal? OtherDeductions { get; set; }

        public int? InsertedBy { get; set; }
        public DateTime? InsertedDate { get; set; }
        public string StatusMessage { get; set; }
        public string Remarks { get; set; }
    }
}
