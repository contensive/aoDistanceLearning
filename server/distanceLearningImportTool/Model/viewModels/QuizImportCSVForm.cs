using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.viewModels {
    // 
    public class QuizImportCSVForm : AddonBaseClass {
        // 
        public override object Execute(CPBaseClass CP) {
            string returnHtml = "";
            // 
            try {
                // 
                // 
                // 
                string body = "";
                string rqs = CP.Doc.RefreshQueryString;
                var formHandler = new QuizImportCSVHandler();
                // 
                // 
                // 
                CP.Doc.SetProperty("multiformAjaxVbFrameRqs", rqs);
                body = (string)formHandler.Execute(CP);
                // 
                // 
                // 
                CP.Doc.AddHeadJavascript("var " + common.jsMultiformAjaxVbFrameRqs + "='" + rqs + "';");
                returnHtml = CP.Html.div(body, "", "", common.divMultiFormAjaxFrameDiv);
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "error in QuizImportCSVForm.execute");
            }
            return returnHtml;
        }
    }
    // 
}