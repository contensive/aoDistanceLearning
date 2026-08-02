using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.viewModels {
    // 
    public class QuizFormEmailNotification : formBaseClass {
        // 
        // 
        // 
        internal override int processForm(CPBaseClass cp, int srcFormId, string rqs, DateTime rightnow, ref applicationClass application) {
            int nextFormId = srcFormId;
            try {
                string button;
                var cs = cp.CSNew();
                // 
                // ajax routines return a different name for button
                // 
                button = cp.Doc.GetText("ajaxButton");
                if (string.IsNullOrEmpty(button)) {
                    button = cp.Doc.GetText(common.rnButton);
                }

                // Check if exist a file to upload 
                if (!string.IsNullOrEmpty(cp.Doc.GetText("notificationEmail"))) {
                    // 
                    application.notificationEmail = cp.Doc.GetText("notificationEmail");
                    // 
                }
                // 
                switch (button ?? "") {
                    case common.buttonNext:
                    case common.buttonFinish: {
                            application.changed = true;
                            application.Completed = true;

                            // ************
                            // call the addon to process the csv upload

                            cp.Doc.SetProperty("recordId", application.id);
                            cp.Addon.ExecuteAsProcess("{53588d6f-9915-4712-a9f8-f69624a5a0ec}");

                            // ************

                            nextFormId = common.formIdThankYou;
                            break;
                        }
                }
                // 
            } catch (Exception ex) {
                errorReport(ex, cp, "QuizFormFileSelection.processForm");
            }
            return nextFormId;
        }
        // 
        // 
        // 
        internal override string getForm(CPBaseClass cp, int dstFormId, string rqs, DateTime rightNow, ref applicationClass application) {
            string returnHtml = "";
            try {
                var layout = cp.BlockNew();
                var cs = cp.CSNew();
                string body;
                // 
                layout.OpenLayout("{92a6d8a6-b7f6-49d5-b59f-54ced0de0e70}");
                // 

                layout.SetInner("#mfaContent", cp.Content.GetCopy("Import Quiz Tool email Notification"));
                if (!string.IsNullOrEmpty(application.errorMsg)) {
                    layout.SetInner("#mfaErrorWrapper", application.errorMsg);
                }

                // 
                // wrap it in a form for the javascript to use during submit
                // 
                body = layout.GetHtml();
                body += cp.Html5.Hidden(common.rnAppId, application.id);
                body += cp.Html.Hidden(common.rnSrcFormId, dstFormId.ToString());
                returnHtml = cp.Html.Form(body, "", "", "mfaForm2", rqs);
            } catch (Exception ex) {
                errorReport(ex, cp, "getForm");
            }
            return returnHtml;
        }
        // 
        // 
        // 
        private void errorReport(Exception ex, CPBaseClass cp, string @method) {
            cp.Site.ErrorReport(ex, "error in QuizFormEmailNotification." + @method);
        }
    }
    // 
}