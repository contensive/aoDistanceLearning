using System;
using System.Collections.Generic;
using System.Text.Json;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningAuthentication.Interfaces.Remotes {
    // 
    public class ProcessSignUp : AddonBaseClass {
        // 
        // 
        // 
        public override object Execute(CPBaseClass CP) {
            string returnHtml;
            var remoteResponse = new Model.architectureModels.remoteResponseObject();
            try {
                // 
                var SignUp = deserializeSignUpObject(CP, CP.Doc.GetText("jsonData"));
                // 
                Model.dbModels.User.CreateUser(CP, ref SignUp, ref remoteResponse.errors);
                // 
                if (remoteResponse.errors.Count > 0) {
                    remoteResponse.data = "Sign Up Process Errors";
                } else {
                    remoteResponse.data = Model.dbModels.User.getUser(CP, SignUp.id);
                }
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "execute");
                remoteResponse = new Model.architectureModels.remoteResponseObject() { data = new object(), errors = new List<Model.architectureModels.errorClass>() { new Model.architectureModels.errorClass() { number = 1, userMsg = "Internal Error" } } };
                // http error
                // 
                CP.Response.SetStatus("500");
            } finally {
                returnHtml = JsonSerializer.Serialize(remoteResponse);
            }
            return returnHtml;
        }
        // 
        // 
        // 
        private Model.solutionModels.SignUp deserializeSignUpObject(CPBaseClass CP, string value) {
            var SignUpObject = new Model.solutionModels.SignUp();
            //
            try {
                //
                //
                //
                SignUpObject = JsonSerializer.Deserialize<Model.solutionModels.SignUp>(value);

            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "error in deserializeUserObject");
            }
            // 
            return SignUpObject;
        }
        // 
        // 
        // 
        private void errorReport(CPBaseClass cp, Exception ex, string @method) {
            try {
                cp.Site.ErrorReport(ex, "Unexpected error in ProcessSignUp." + @method);
            } catch (Exception) {
                //
                //
                //
            }
        }
    }
    //
}