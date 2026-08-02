using System;
using System.Collections.Generic;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningAuthentication.Model.solutionModels {
    // 
    public class Login {
        // 
        public string username = "";
        public string password = "";
        // 
        // 
        // 
        public static bool ProcessLogin(CPBaseClass CP, Login Login, ref List<architectureModels.errorClass> errorList) {
            bool result = false;
            string errorStep = "00";
            try {
                // 
                errorStep = "01";
                // 

                if (Login == null) {
                    errorStep = "02";
                    // send message error
                    var oneError = new architectureModels.errorClass();
                    oneError.number = 600;
                    oneError.userMsg = "No values found.";
                    errorList.Add(oneError);

                    // check if username and password is not empty
                } else if (!string.IsNullOrEmpty(Login.username) & !string.IsNullOrEmpty(Login.password)) {
                    // check if valid username ans password combination
                    errorStep = "03";
                    if (CP.User.Login(Login.username, Login.password)) {

                        result = true;
                        // *****************
                    } else {
                        errorStep = "04";
                        // send message error
                        var oneError = new architectureModels.errorClass();
                        oneError.number = 640;
                        oneError.userMsg = "username or password is invalid.";
                        errorList.Add(oneError);
                    }

                } else {
                    errorStep = "05";
                    // send message error
                    var oneError = new architectureModels.errorClass();
                    oneError.number = 620;
                    oneError.userMsg = "One or more fields are empty.";
                    errorList.Add(oneError);
                    // 
                }

            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in ProcessLogin - step: " + errorStep);
            }
            return result;
        }
        // 
        // 
        // 
        public static bool ForgetPassword(CPBaseClass CP, string email) {
            var Result = default(bool);
            try {
                // Call CP.email.sendPassword(email)
                // 
                var ActualUser = dbModels.User.getRecordFromEmail(CP, email);

                if (ActualUser.id != 0) {
                    // Generate username if not exist
                    ActualUser.VerifyActualUsernameIsValid(CP);

                    // Generate new password
                    string NewPassword = ActualUser.GenerateUserNewPassword(CP, ActualUser.id);

                    // Send Email
                    string SystemEmailName = "Distance Learning Authentication - Forgot Password";
                    string Message = CP.Content.GetCopy("Distance Learning Authentication - Forgot Password");

                    Message = Message.Replace("[Username]", ActualUser.username);
                    Message = Message.Replace("[Password]", NewPassword);
                    Message = Message.Replace("[MemberName]", ActualUser.name);
                    Message = Message.Replace("[MemberFirstName]", ActualUser.firstname);
                    Message = Message.Replace("[MemberLastName]", ActualUser.lastname);
                    // 
                    // 
                    // LogEvent(CP, "ForgetPassword.log", "Calling sendSystem: " & SystemEmailName & " from inside of sendUserNameAndPasswordEmail for member id : " & ActualUser.id & " with message : " & Message)
                    // 
                    CP.Email.sendSystem(SystemEmailName, Message, ActualUser.id);
                }
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in ForgetPassword");
            }
            return Result;
        }
        // 
        // 
        // 
    }
    // 
}