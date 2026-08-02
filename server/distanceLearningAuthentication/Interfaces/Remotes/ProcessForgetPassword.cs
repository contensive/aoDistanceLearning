using System;
using System.Collections.Generic;
using System.Text.Json;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningAuthentication.Interfaces.Remotes {
    // 
    public class ProcessForgetPassword : AddonBaseClass {
        // 
        // 
        // 
        public override object Execute(CPBaseClass CP) {
            string returnHtml;
            var remoteResponse = new Model.architectureModels.remoteResponseObject();
            try {
                // 
                // 
                string email = CP.Doc.GetText("email");
                if (!string.IsNullOrEmpty(email)) {
                    // 
                    Model.solutionModels.Login.ForgetPassword(CP, email);
                    // 
                    remoteResponse.data = "Process Forget Password Ok";
                } else {
                    remoteResponse.data = "Process Forget Password Error";
                    // send message error
                    var oneError = new Model.architectureModels.errorClass();
                    oneError.number = 240;
                    oneError.userMsg = "Email Information is required.";
                    remoteResponse.errors.Add(oneError);
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
        private void errorReport(CPBaseClass cp, Exception ex, string @method) {
            try {
                cp.Site.ErrorReport(ex, "Unexpected error in Login." + @method);
            } catch (Exception) {
                //
                //
                //
            }
        }
    }
    //
}