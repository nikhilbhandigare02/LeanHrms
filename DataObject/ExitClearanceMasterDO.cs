using System;

namespace DataObject
{
    public class ExitClearanceMasterDO
    {
        public int ClearanceMasterId { get; set; }
        public string DepartmentName { get; set; }
        public string ClearanceItem  { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class ExitClearanceEmployeeDetailsDO
    {
        public int EmployeeResignationId { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeEmail { get; set; }
        public DateTime ResignationDate { get; set; }
        public DateTime LastWorkingDate { get; set; }
    }

    public class ExitClearanceEmployeeDO
    {
        public int EmployeeResignationId { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
    }

    public class ExitClearanceResponseDO
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public int ExitClearanceId { get; set; }
    }

    public class ExitClearanceDataDO
    {
        public int ExitClearanceId { get; set; }
        public int EmployeeResignationId { get; set; }
        public string EmpCode { get; set; }

        // IT Department
        public int LaptopReturned { get; set; }
        public int DesktopReturned { get; set; }
        public int MobilePhoneReturned { get; set; }
        public int EmailDisabled { get; set; }
        public int VpnDisabled { get; set; }
        public int HrmsAccessRemoved { get; set; }
        public string ItRemarks { get; set; }

        // Administration Department
        public int IdCardReturned { get; set; }
        public int AccessCardReturned { get; set; }
        public int OfficeKeysReturned { get; set; }
        public int ParkingPassReturned { get; set; }
        public string AdministrationRemarks { get; set; }

        // Finance Department
        public int LoanRecoveryCompleted { get; set; }
        public int SalaryAdvanceRecovered { get; set; }
        public int ExpenseClaimsProcessed { get; set; }
        public string FinanceRemarks { get; set; }

        // Security Department
        public int BiometricDisabled { get; set; }
        public int BuildingAccessRevoked { get; set; }
        public string SecurityRemarks { get; set; }

        public int InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int IsActive { get; set; }
    }
}
