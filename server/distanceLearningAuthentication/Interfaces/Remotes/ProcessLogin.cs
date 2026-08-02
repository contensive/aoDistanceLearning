using System;
using System.Collections.Generic;
using System.Text.Json;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningAuthentication.Interfaces.Remotes {
    // 
    public class ProcessLogin : AddonBaseClass {
        // 
        // 
        // 
        public override object Execute(CPBaseClass CP) {
            string returnHtml;
            var remoteResponse = new Model.architectureModels.remoteResponseObject();
            try {
                // 
                Model.solutionModels.Login.ProcessLogin(CP, deserializeLoginObject(CP, CP.Doc.GetText("jsonData")), ref remoteResponse.errors);
                if (remoteResponse.errors.Count > 0) {
                    remoteResponse.data = "Process Login Result Error";
                    CP.User.Logout();
                } else {
                    remoteResponse.data = Model.dbModels.User.getUser(CP, CP.User.Id);
                }


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
        private Model.solutionModels.Login deserializeLoginObject(CPBaseClass CP, string value) {
            var SignUpObject = new Model.solutionModels.Login();
            //
            try {
                //
                //
                //
                SignUpObject = JsonSerializer.Deserialize<Model.solutionModels.Login>(value);

            } catch (Exception ex) {
                try {
                    CP.Site.ErrorReport(ex, "error in deserializeLoginObject");
                } catch (Exception) {
                }
            }
            // 
            return SignUpObject;
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