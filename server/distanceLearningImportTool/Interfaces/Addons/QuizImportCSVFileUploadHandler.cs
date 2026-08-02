using System;
using System.Collections.Generic;
using System.Text.Json;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Interfaces.Addons {
    // 
    public class QuizImportCSVFileUploadHandler : AddonBaseClass {
        // 
        // 
        // 
        public override object Execute(CPBaseClass CP) {
            string returnHtml;
            var remoteResponse = new Model.architectureModels.remoteResponseObject();
            try {
                // 
                int recordId = CP.Doc.GetInteger("appId");

                // CP.Utils.AppendLog("QuizImportCSVFileUploadHandler.log","recordId: " & recordId)
                // 
                Model.dbModels.QuizImports.SetCSVFile(CP, ref recordId, ref remoteResponse.errors);

                remoteResponse.data = "ok";

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