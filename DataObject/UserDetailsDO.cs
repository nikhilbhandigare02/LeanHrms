using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class UserDetailsDO
    {
        public int UserId { get; set; }
        public int? usernameId { get; set; }
        public int? empcodeId { get; set; }
        public string EmployeeCode { get; set; }
        public string Username { get; set; }

        public string user_fullname { get; set; }

        public string roledescription { get; set; }

        public string user_mail_id { get; set; }

        public string contact_detail { get; set; }

        public string user_type { get; set; }

        public bool Isactive { get; set; }

        public DateTime ActivatedDate { get; set; }

        public string UserStatusflag { get; set; }

        public DateTime DeactivatedDate { get; set; }

        public bool PassResetflag { get; set; }

        public int WrongPassCount { get; set; }

        public int Insertedby { get; set; }

        public DateTime? Inserteddate { get; set; }

        public int? Updatedby { get; set; }

        public DateTime Updateddate { get; set; }

        public bool AllowmultipleRoles { get; set; }

        //public string EmpCode { get; set; }
        //public int designation_id { get; set; }
        public int CompanyId { get; set; }

        public string user_role { get; set; }
        public string password { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int EmailChanged { get; set; }
        public int MobileChanged { get; set; }
        public int SendCredentialsMail { get; set; }
        public string searchbyType { get; set; }
        public Int32 designation_id { get; set; }
        public string designation_name { get; set; }
        public string searchValue { get; set; }

        public DateTime? TerminationDate { get; set; }

        public string notice_status { get; set; }
        public DateTime? ResponseDeadline { get; set; }

        public Int32 company_id { get; set; }
        public string company_name { get; set; }

        public int ESIC_no { get; set; }

        public int PF_no { get; set; }

        public string department { get; set; }

        public string branch { get; set; }

        public string division { get; set; }

        public DateTime date_of_joining { get; set; }

        public int probation_period_months { get; set; }

        public string reporting_manager { get; set; }

        public string employee_type { get; set; }
        public string EmploymentTypeId { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int Age { get; set; }
        public string MaritalStatus { get; set; }
        public string BloodGroup { get; set; }
        public string Nationality { get; set; }
        public string AadhaarNumber { get; set; }
        public string PanNumber { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? PassportExpiryDate { get; set; }
        public string EmployeePhoto { get; set; }
        public string EmployeeStatus { get; set; }
        public int NoticePeriod { get; set; }
        public DateTime? ExitDate { get; set; }
        public string SeparationReason { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public int ExtendedProbationDays { get; set; }
        public int ExtendedProbationMonth { get; set; }
        public string ProbationStatus { get; set; }
        public string ProbationRemarks { get; set; }
        public string ApprovedBy { get; set; }
        public string BranchOffice { get; set; }
        public string Location { get; set; }
        public string EmployeeLevel { get; set; }
        public string FunctionalManagerName { get; set; }
        public string HodName { get; set; }
        public string AttendanceType { get; set; }
        public string WeeklyOff { get; set; }
        public string AttendancePolicy { get; set; }
        public string WorkingHours { get; set; }
        public string PunchingDeviceId { get; set; }
        public string BiometricId { get; set; }
        public string OvertimeEligible { get; set; }
        public string OvertimeRate { get; set; }
        public string WorkLocation { get; set; }
        public string RemoteWorkAllowed { get; set; }
        public string AssetType { get; set; }
        public string AssetNumber { get; set; }
        public string AssetName { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string AssetCondition { get; set; }
        public string AssetStatus { get; set; }
        public string BankName { get; set; }
        public string BankBranchName { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountNumber { get; set; }
        public string ConfirmAccountNumber { get; set; }
        public string IfscCode { get; set; }
        public string AccountType { get; set; }
        public string SalaryAccountFlagText { get; set; }
        public string QualificationLevel { get; set; }
        public string DegreeName { get; set; }
        public string Specialization { get; set; }
        public string University { get; set; }
        public string InstituteName { get; set; }
        public string YearOfPassing { get; set; }
        public string PercentageCgpa { get; set; }
        public string EducationCertificateFile { get; set; }
        public string PermanentHouseNumber { get; set; }
        public string PermanentBuildingName { get; set; }
        public string PermanentStreet { get; set; }
        public string PermanentArea { get; set; }
        public string PermanentLandmark { get; set; }
        public string PermanentCity { get; set; }
        public string PermanentDistrict { get; set; }
        public string PermanentState { get; set; }
        public string PermanentCountry { get; set; }
        public string PermanentPinCode { get; set; }
        public string SameAsPermanent { get; set; }
        public string CurrentHouseNumber { get; set; }
        public string CurrentBuildingName { get; set; }
        public string CurrentStreet { get; set; }
        public string CurrentArea { get; set; }
        public string CurrentLandmark { get; set; }
        public string CurrentCity { get; set; }
        public string CurrentDistrict { get; set; }
        public string CurrentState { get; set; }
        public string CurrentCountry { get; set; }
        public string CurrentPinCode { get; set; }
        public string AlternateMobileNumber { get; set; }
        public string PersonalEmail { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNumber { get; set; }
        public string EmergencyContactRelationship { get; set; }
        public string CertificationName { get; set; }
        public string CertificationAuthority { get; set; }
        public string CertificateNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string CertificationFile { get; set; }
        public string RenewalRequired { get; set; }

    }
    public class UpdateProbationRequest
    {
        public int UserId { get; set; }
        public int ProbationFlag { get; set; }
        public string Remark { get; set; }   // <-- ADD THIS

        public string DateOfExtended { get; set; }   // <-- ADD THIS


    }
    public class userProbationflagResponseDO
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string ResponseMsg { get; set; }
    }
    public class UserDateForProbationPeriodDO
    {
        public int EmpleaveDetailsId { get; set; }
        public DateTime InsertedDate { get; set; }
        public int SixMonthCompleted { get; set; }

        // Response message (SUCCESS / FAILED / INFO)
        public string ResponseMessage { get; set; }
    }
    public class userDateResponseDataDO
    {
        //public int User_Id { get; set; }
        //public string Username { get; set; }
        //public string User_fullname { get; set; }
        //public string User_Email { get; set; }
        //public string Contact_No { get; set; }
        //public string User_Role { get; set; }

        public bool Success { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string ResponseMsg { get; set; }

        public List<UserDateForProbationPeriodDO> UsersprobationperiodDateList { get; set; }
    }

    public class UserEmployeeTypeDO
    {
        public string employee_type { get; set; }
        public string ResponseMessage { get; set; }

    }

    public class UserEmployeeTypeResponseDO
    {
        public bool Success { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string ResponseMsg { get; set; }

        public List<UserEmployeeTypeDO> EmployeeTypeList { get; set; }
    }

    public class EmployeeAssetDetailsDO
    {
        public int AssetAssignmentId { get; set; }
        public int UserId { get; set; }
        public string AssetType { get; set; }
        public string AssetNumber { get; set; }
        public string AssetName { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int AssetConditionId { get; set; }
        public string AssetCondition { get; set; }
        public int AssetStatusId { get; set; }
        public string AssetStatus { get; set; }
        public int InsertedBy { get; set; }
        public DateTime? InsertedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class EmployeeEducationDetailsDO
    {
        public int EducationId { get; set; }
        public int UserId { get; set; }
        public string QualificationLevel { get; set; }
        public string DegreeName { get; set; }
        public string Specialization { get; set; }
        public string University { get; set; }
        public string InstituteName { get; set; }
        public string YearOfPassing { get; set; }
        public string PercentageCgpa { get; set; }
        public string CertificateFile { get; set; }
    }

    public class EmployeeWorkExperienceDetailsDO
    {
        public int ExperienceId { get; set; }
        public int UserId { get; set; }
        public string OrganizationName { get; set; }
        public string Industry { get; set; }
        public string Designation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string TotalExperience { get; set; }
        public string LastDrawnSalary { get; set; }
        public string EmploymentPeriod { get; set; }
    }

}
