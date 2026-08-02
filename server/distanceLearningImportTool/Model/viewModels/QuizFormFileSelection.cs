using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.viewModels {
    // 
    public class QuizFormFileSelection : formBaseClass {
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
                    // If application.id = 0 Then
                    // ' New App
                    // ' create record and set site property
                    // 'If cs.Insert(cnMultiFormAjaxApplications) Then
                    // '    application.id = cs.GetInteger("id")
                    // '    Call cp.Visit.SetProperty(cnMultiFormAjaxApplications & " ApplicationId", application.id.ToString())
                    // '    '
                    // '    'Call cs.SetFormInput("csvFilename", "csvFilename")
                    // 'End If
                    // 'Call cs.Close()
                    // '
                    // Else 
                    // ' Existing App
                    // ' Open record and set site property
                    // 'If cs.Open(cnMultiFormAjaxApplications, "id=" & application.id ) Then
                    // '    Call cs.SetFormInput("csvFilename", "csvFilename")
                    // 'End If
                    // 'Call cs.Close()
                    // '
                    // End If
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
                        case common.buttonNext:
                        case common.buttonContinue: {
                                nextFormId = common.formIdEmailNotification;
                                break;
                            }
                    }

                } else {
                    // 
                    nextFormId = common.formIdFileSelection;
                    // 
                }
            } catch (Exception ex) {
                errorReport(ex, cp, "processForm");
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
                string body;
                // 
                layout.OpenLayout("{6116daa8-a3c8-4c69-9643-7783ef724e37}"); // Quiz CSV Imports File Selection Layout
                // 
                layout.SetInner("#mfaContent", cp.Content.GetCopy("Import Quiz Tool File Selection"));
                if (!string.IsNullOrEmpty(application.errorMsg)) {
                    layout.SetInner("#mfaErrorWrapper", application.errorMsg);
                }

                // 
                // wrap it in a form for the javascript to use during submit
                // 
                body = layout.GetHtml();
                body += cp.Html.Hidden(common.rnAppId, application.id.ToString());
                body += cp.Html.Hidden(common.rnSrcFormId, dstFormId.ToString());
                returnHtml = cp.Html.Form(body, "", "", "mfaForm1", rqs);
            } catch (Exception ex) {
                errorReport(ex, cp, "getForm");
            }
            return returnHtml;
        }
        // 
        // 
        // 
        private void errorReport(Exception ex, CPBaseClass cp, string @method) {
            cp.Site.ErrorReport(ex, "error in aoManagerTemplate.multiFormAjaxSample." + @method);
        }
    }
    // 
}