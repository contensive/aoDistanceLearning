using System;
using System.Collections.Generic;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningAuthentication.Model.dbModels {
    // 
    [Serializable]
    public class User : BasicRecord {
        // 
        public string firstname;
        public string lastname;
        public string email;
        public string username;
        public int visitId;
        // 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CP"></param>
        /// <returns></returns>
        public bool VerifyActualUsernameIsValid(CPBaseClass CP) {
            bool result = false;
            try {
                string usernameBase = "";
                if (string.IsNullOrEmpty(username)) {
                    usernameBase = (firstname + lastname).Replace(" ", "").ToLower();
                    if (verifyUsernameAlreadyExist(CP, usernameBase)){
                        for (int i = 1; i <= 100; i++) {
                            if ( !verifyUsernameAlreadyExist(CP, usernameBase + i)) {
                                username = usernameBase + i;
                                // set username value
                                setFieldWithValue(CP, id, "username", username);
                                break;
                            }
                        }
                    } else {
                        // set username value
                        username = usernameBase;
                        setFieldWithValue(CP, id, "username", username);
                        result = true;
                    }
                } else {
                    result = true;
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Member.VerifyActualUsername");
            }
            return result;
        }
        // 
        public string GenerateUserNewPassword(CPBaseClass CP, int userId) {
            string newPassword = "";
            try {
                var cs = CP.CSNew();
                newPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
                // 
                setFieldWithValue(CP, userId, "password", newPassword);
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Member.GenerateUserNewPassword");
            }
            return newPassword;
        }
        // 
        // *****************
        // Shared Functions
        // *****************
        // 
        public static object setFieldWithValue(CPBaseClass CP, int userId, string fieldName, string value) {
            bool result = false;
            try {
                var cs = CP.CSNew();
                if (cs.Open(common.cnPeople, "id=" + CP.Db.EncodeSQLNumber(userId))) {
                    cs.SetField(fieldName, value);
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Member.verifyUsernameAlreadyExist");
            }
            return result;
        }
        // 
        public static bool verifyUsernameAlreadyExist(CPBaseClass CP, string username) {
            bool result = false;
            try {
                var cs = CP.CSNew();
                result = cs.Open(common.cnPeople, "username=" + CP.Db.EncodeSQLText(username));
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Member.verifyUsernameAlreadyExist");
            }
            return result;
        }
        // 
        public static bool CreateUser(CPBaseClass CP, ref solutionModels.SignUp ActualUser, ref List<architectureModels.errorClass> errorList) {
            var Result = default(bool);
            string errorStep = "00";
            // 
            try {
                // 
                var cs = CP.CSNew();
                // Dim emailExist As Boolean = False
                bool usernameExist = false;
                //
                //
                errorStep = "01";
                // check if email exist
                // emailExist = cs.Open(cnPeople, "email = " & CP.Db.EncodeSQLText(ActualUser.email))
                // Call cs.Close()

                errorStep = "02";
                // check if username exist
                usernameExist = cs.Open(common.cnPeople, "username = " + CP.Db.EncodeSQLText(ActualUser.username));
                cs.Close();

                errorStep = "03";
                // 
                if (!string.IsNullOrEmpty(ActualUser.username) & !string.IsNullOrEmpty(ActualUser.password)) {

                    if (!usernameExist) {
                        // 
                        if (cs.Insert(common.cnPeople)) {
                            // 
                            ActualUser.id = cs.GetInteger("id");
                            // 
                            errorStep = "04";
                            cs.SetField("Name", ActualUser.firstname + " " + ActualUser.lastname);
                            cs.SetField("FirstName", ActualUser.firstname);
                            cs.SetField("LastName", ActualUser.lastname);
                            cs.SetField("Username", ActualUser.username);
                            cs.SetField("Email", ActualUser.username);

                            errorStep = "07";
                            cs.SetField("password", ActualUser.password);
                            // 
                            cs.Save();
                            // 
                            // Login the user
                            // 
                            // CP.User.LoginByID(ActualUser.id)
                            // 
                        }
                        cs.Close();
                        // 
                    } else {
                        // 
                        var oneError = new architectureModels.errorClass();
                        oneError.number = 720;
                        oneError.userMsg = ActualUser.username + " already exists in the system. Please login or use a different email to continue.";
                        errorList.Add(oneError);
                        // 
                    }
                    // 
                } else {
                    // 
                    var oneError = new architectureModels.errorClass();
                    oneError.number = 700;
                    oneError.userMsg = "email and/or password can't be empty.";
                    errorList.Add(oneError);
                    // 
                }
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in User.CreateUser - errorStep:" + errorStep);
            }

            // 
            return Result;
        }
        // 
        public static User getRecordFromEmail(CPBaseClass CP, string email) {
            var oMember = new User();
            try {
                // 
                var cs = CP.CSNew();
                if (cs.Open(common.cnPeople, "email = " + CP.Db.EncodeSQLText(email))) {
                    oMember = getUser(CP, cs.GetInteger("id"));
                }
                cs.Close();
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Member.getRecordFromEmail");
            }
            return oMember;
        }
        // 
        public static User getUser(CPBaseClass CP, int UserId) {
            var oUser = new User();
            // 
            try {
                var cs = CP.CSNew();

                if (cs.Open(common.cnPeople, "id=" + CP.Db.EncodeSQLNumber(UserId))) {
                    // 
                    oUser.id = cs.GetInteger("id");
                    // 
                    oUser.name = cs.GetText("Name");
                    oUser.firstname = cs.GetText("FirstName");
                    oUser.lastname = cs.GetText("LastName");
                    oUser.email = cs.GetText("Email");
                    oUser.username = cs.GetText("username");
                    // 
                    oUser.visitId = CP.Visit.Id;
                    // 
                }
                cs.Close();
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in User.getUser");
            }
            // 
            return oUser;
        }
        // 
        // 
        // 
        public static bool ActualUserIsInGroup(CPBaseClass CP, int groupId) {
            bool result = false;
            try {
                // 
                var cs = CP.CSNew();
                if (cs.Open(common.cnMemberRules, "MemberID=" + CP.Db.EncodeSQLNumber(CP.User.Id) + " and GroupID=" + CP.Db.EncodeSQLNumber(groupId))) {
                    result = true;
                }
                cs.Close();
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in User.ActualUserIsInGroup");
            }
            return result;
        }
        // 
        // 
        // 
        public static bool AddUserToGroup(CPBaseClass CP, int groupId) {
            bool result = false;
            try {

                var csGroup = CP.CSNew();
                bool UserExistInGroupId = false;
                if (groupId != 0) {
                    UserExistInGroupId = csGroup.Open("Member Rules", "MemberID=" + CP.User.Id + " And GroupID=" + groupId);
                    csGroup.Close();
                    if (!UserExistInGroupId) {
                        if (csGroup.Insert("Member Rules")) {
                            csGroup.SetField("MemberID", CP.User.Id);
                            csGroup.SetField("GroupID", groupId);
                            csGroup.SetField("active", "1");
                            result = true;
                        }
                        csGroup.Close();
                    }
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in User.AddUserToGroup");
            }
            return result;
        }
        // 
        // 
        // 
        public static int getAccountId(CPBaseClass CP, int userId) {
            int accountId = 0;
            try {
                // 
                var cs = CP.CSNew();
                var csAccount = CP.CSNew();
                if (userId != 0) {
                    if (cs.Open(common.cnPeople, "id=" + CP.Db.EncodeSQLNumber(userId))) {
                        accountId = cs.GetInteger("accountId");

                        // 
                        // Check if really exit the account
                        // 
                        if (!csAccount.Open(common.cnAccounts, "id=" + CP.Db.EncodeSQLNumber(accountId))) {
                            accountId = 0;
                        }
                        csAccount.Close();
                        // 
                    }
                    cs.Close();
                }

            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in User.getAccountId");
            }
            return accountId;
        }
        // 
        // 
        // 
    }
    // 
}