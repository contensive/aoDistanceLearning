using System;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.viewModels {
    // 
    public static class common {
        // 
        public const string divMultiFormAjaxFrameDiv = "multiFormAjaxFrame";
        public const string jsMultiformAjaxVbFrameRqs = "multiformAjaxVbFrameRqs";
        // 
        // 
        // request names 
        // 
        public const string rnAppId = "appId";
        public const string rnSrcFormId = "srcFormId";
        public const string rnDstFormId = "dstFormId";
        public const string rnButton = "button";
        // 
        // 
        // 
        public const string cnMultiFormAjaxApplications = "Quiz CSV Imports";
        // 
        // 
        // 
        public const string buttonOK = "OK";
        public const string buttonSave = "Save";
        public const string buttonCancel = "Cancel";
        public const string buttonNext = "Next";
        public const string buttonPrevious = "Previous";
        public const string buttonContinue = "Continue";
        public const string buttonRestart = "Restart";
        public const string buttonFinish = "Finish";
        // 
        // 
        public const int formIdFileSelection = 1;
        public const int formIdEmailNotification = 2;
        public const int formIdThankYou = 3;
        // 
        // 
        internal static DateTime encodeMinDate(DateTime sourceDate) {
            var returnValue = sourceDate;
            if (returnValue < DateTime.Parse("1900-01-01")) {
                returnValue = DateTime.MinValue;
            }
            return returnValue;
        }
        // 
        // 
        // 
        internal static DateTime getRightNow(CPBaseClass cp) {
            var returnValue = DateTime.Now;
            try {
                string testString = cp.Site.GetText("Sample Manager Test Mode Date", "");
                // 
                // change 'sample' to the name of this collection
                // 
                if (!string.IsNullOrEmpty(testString)) {
                    returnValue = encodeMinDate(cp.Utils.EncodeDate(testString));
                    if (returnValue == DateTime.MinValue) {
                        returnValue = DateTime.Now;
                    }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "Error in getRightNow");
            }
            return returnValue;
        }
        // 
        // 
        // 

        internal static applicationClass getApplication(CPBaseClass cp, bool createRecordIfMissing) {
            var application = new applicationClass();
            try {
                var cs = cp.CSNew();
                // 
                application.Completed = false;
                application.changed = false;
                application.id = cp.Visit.GetInteger(cnMultiFormAjaxApplications + " ApplicationId");
                // 
                if (cs.Open(cnMultiFormAjaxApplications, "id=" + application.id)) {
                    // 
                    application.notificationEmail = cs.GetText("notificationEmail");
                    // 
                } else {
                    // 
                    cs.Close();
                    if (!cs.Open(cnMultiFormAjaxApplications, "(dateCompleted is null)")) {
                        cs.Close();
                        cs.Insert(cnMultiFormAjaxApplications);
                    }
                    // 
                    application.id = cs.GetInteger("id");
                    application.notificationEmail = "";
                    cp.Visit.SetProperty(cnMultiFormAjaxApplications + " ApplicationId", application.id);
                    // 
                }
                cs.Close();
                // 
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "Error in getApplication");
            }
            return application;
        }
        // 
        // 
        // 
        public static void saveApplication(CPBaseClass cp, applicationClass application, DateTime rightNow) {
            try {
                var cs = cp.CSNew();
                if (application.changed) {
                    if (application.id > 0) {
                        cs.Open(cnMultiFormAjaxApplications, "(id=" + application.id + ")");
                    }
                    if (!cs.OK()) {
                        if (cs.Insert(cnMultiFormAjaxApplications)) {
                            // 
                            // create a new record. 
                            // Set application Id in case needed later
                            // Set visit property to save the application Id
                            // 
                            application.id = cs.GetInteger("id");
                            cp.Visit.SetProperty(cnMultiFormAjaxApplications + " ApplicationId", application.id.ToString());
                        }
                    }
                    if (cs.OK()) {
                        cs.SetField("notificationEmail", application.notificationEmail);
                        if (application.Completed) {
                            cs.SetField("datecompleted", rightNow.ToString());
                            cp.Visit.SetProperty(cnMultiFormAjaxApplications + " ApplicationId", "0");
                        }
                    }
                    cs.Close();
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "Error in getApplication");
            }
        }
        // 
        // 
        // 
    }
    // 
    public class applicationClass {
        public int id;
        public bool changed;
        public bool Completed;
        // Public csvFilename As String
        public string notificationEmail;
        public string errorMsg;
    }
    // 
    // base class for the form classes. Having this means in he form handler you can only create one object, not one per form
    // 
    public abstract class formBaseClass {
        internal abstract int processForm(CPBaseClass cp, int srcFormId, string rqs, DateTime rightNow, ref applicationClass application);

        internal abstract string getForm(CPBaseClass cp, int dstFormId, string rqs, DateTime rightNow, ref applicationClass application);
    }

    // 
}