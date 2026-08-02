using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.viewModels {
    // 
    public class QuizImportCSVHandler : AddonBaseClass {
        // 
        public override object Execute(CPBaseClass CP) {
            string returnHtml = "";
            try {
                // 
                string body = "";
                formBaseClass form;
                string rqs = CP.Doc.GetProperty(common.jsMultiformAjaxVbFrameRqs);
                var rightNow = common.getRightNow(CP);
                int srcFormId = CP.Utils.EncodeInteger(CP.Doc.GetProperty(common.rnSrcFormId));
                int dstFormId = CP.Utils.EncodeInteger(CP.Doc.GetProperty(common.rnDstFormId));
                var formHandler = new QuizImportCSVHandler();
                applicationClass application;
                // 
                // get previously started application
                // 
                application = common.getApplication(CP, false);
                // 
                // if there is no application, only allow form one
                // 
                // cp.Utils.AppendLog("QuizImportCSVHandler.log","application.id: " & application.id)

                // cp.Utils.AppendLog("QuizImportCSVHandler.log","rnSrcFormId: " & CP.Doc.GetProperty(rnSrcFormId))
                // cp.Utils.AppendLog("QuizImportCSVHandler.log","rnDstFormId: " & CP.Doc.GetProperty(rnDstFormId))

                if (application.id == 0) {
                    if (srcFormId != common.formIdFileSelection) {
                        srcFormId = 0;
                    }
                    dstFormId = common.formIdFileSelection;
                }
                // 
                // process forms
                // 
                if (srcFormId != 0) {
                    switch (srcFormId) {
                        case common.formIdFileSelection: {
                                // 
                                form = new QuizFormFileSelection();
                                dstFormId = form.processForm(CP, srcFormId, rqs, rightNow, ref application);
                                break;
                            }
                        case common.formIdEmailNotification: {
                                // 
                                form = new QuizFormEmailNotification();
                                dstFormId = form.processForm(CP, srcFormId, rqs, rightNow, ref application);
                                break;
                            }
                        case common.formIdThankYou: {
                                // 
                                form = new QuizFormThankYou();
                                dstFormId = form.processForm(CP, srcFormId, rqs, rightNow, ref application);
                                break;
                            }
                    }
                }
                // 
                // 
                // 
                switch (dstFormId) {
                    case common.formIdThankYou: {
                            // 
                            // 
                            // 
                            form = new QuizFormThankYou();
                            body = form.getForm(CP, dstFormId, rqs, rightNow, ref application);
                            break;
                        }
                    case common.formIdEmailNotification: {
                            // 
                            // 
                            // 
                            form = new QuizFormEmailNotification();
                            body = form.getForm(CP, dstFormId, rqs, rightNow, ref application);
                            break;
                        }

                    default: {
                            // 
                            // default form
                            // 
                            dstFormId = common.formIdFileSelection;
                            form = new QuizFormFileSelection();
                            body = form.getForm(CP, dstFormId, rqs, rightNow, ref application);
                            break;
                        }
                }
                common.saveApplication(CP, application, rightNow);
                // 
                // assemble body
                // 
                returnHtml = body;
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "error in formHandlerClass.execute");
            }
            return returnHtml;
        }
    }
    // 
}