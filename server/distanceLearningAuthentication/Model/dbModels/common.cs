using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningAuthentication.Model.dbModels {
    // 
    public static class common {
        // 
        public const string cnPeople = "People";
        public const string cnStates = "States";
        public const string cnCountries = "Countries";
        public const string cnMemberRules = "Member Rules";
        // 
        public const string cnPatient = "Patient";
        public const string cnPatientStatus = "Patient Status";
        public const string cnRolePatientStatusRules = "Role Patient Status Rules";
        // 
        public const string cnRoles = "tbl_user_roles";
        public const string cnProducts = "tbl_product";
        public const string cnDoctorCreditCards = "Doctor Credit Cards";
        // 
        public const string cnAccounts = "Accounts";
        public const string cnItems = "Items";
        // 
        // 
        public static string ValidateFilePath(CPBaseClass CP, string FilePath, string DefaultPath = "") {
            string result = "";
            try {
                if (!string.IsNullOrEmpty(FilePath)) {
                    result = CP.Http.CdnFilePathPrefix + FilePath;
                } else {
                    result = DefaultPath;
                }
            } catch (Exception) {

            }
            return result;
        }
        // 
        // 
        // 
    }
    // 
}