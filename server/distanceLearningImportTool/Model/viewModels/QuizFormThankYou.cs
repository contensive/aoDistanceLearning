using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.viewModels {
    // 
    public class QuizFormThankYou : formBaseClass {
        // 
        // 
        // 
        internal override int processForm(CPBaseClass cp, int srcFormId, string rqs, DateTime rightnow, ref applicationClass application) {
            int nextFormId = srcFormId;
            try {
                string button;
                var cs = cp.CSNew();
                bool isInputOK = true;
                // 
                // ajax routines return a different name for button
                // 
                button = cp.Doc.GetText("ajaxButton");
                if (string.IsNullOrEmpty(button)) {
                    button = cp.Doc.GetText(common.rnButton);
                }

                // Check if exist a file to upload 
                if (!string.IsNullOrEmpty(cp.Doc.GetText("csvFilename"))) {
                    if (application.id == 0) {
                        // New App
                        // create record and set site property
                        if (cs.Insert(common.cnMultiFormAjaxApplications)) {
                            application.id = cs.GetInteger("id");
                            cp.Visit.SetProperty(common.cnMultiFormAjaxApplications + " ApplicationId", application.id.ToString());

                            cs.SetFormInput("csvFilename", "csvFilename");

                        }
                        cs.Close();
                        // 
                    } else {
                        // Existing App
                        // Open record and set site property
                        if (cs.Open(common.cnMultiFormAjaxApplications, "id=" + application.id)) {
                            cs.SetFormInput("csvFilename", "csvFilename");
                        }
                        cs.Close();
                        // 
                    }
                    isInputOK = true;
                    // 
                } else {
                    // 
                    application.errorMsg = "CSV file is required.";
                    isInputOK = false;
                    // 
                }
                // 
                if (isInputOK) {
                    // 
                    switch (button ?? "") {
                        case common.buttonNext: {
                                nextFormId = common.formIdEmailNotification;
                                break;
                            }
                    }

                } else {
                    // 
                    nextFormId = common.formIdEmailNotification;
                    // 
                }
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
                layout.OpenLayout("{4d840be4-ba54-4328-8262-66c7427827a1}"); // 
                // 
                layout.SetInner("#mfaContent", cp.Content.GetCopy("Import Quiz Tool Thank You"));
                // 
                // wrap it in a form for the javascript to use during submit
                // 
                body = layout.GetHtml();
                body += cp.Html.Hidden(common.rnAppId, application.id.ToString());
                body += cp.Html.Hidden(common.rnSrcFormId, dstFormId.ToString());
                returnHtml = cp.Html.Form(body, "", "", "mfaForm3", rqs);
            } catch (Exception ex) {
                errorReport(ex, cp, "getForm");
            }
            return returnHtml;
        }
        // 
        // 
        // 
        private void errorReport(Exception ex, CPBaseClass cp, string @method) {
            cp.Site.ErrorReport(ex, "error in QuizFormThankYou." + @method);
        }

    }
    // 
}