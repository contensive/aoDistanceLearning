using System;
using System.Collections.Generic;
using Contensive.BaseClasses;
using Microsoft.VisualBasic;

namespace Contensive.distanceLearningImportTool.Model.dbModels {
    // 
    public class QuizImports : BasicRecord {
        // 
        // 
        // 
        public static bool existCsvFilename(CPBaseClass CP, int recordId) {
            try {
                using (var cs = CP.CSNew()) {
                    if (cs.Open(common.cnQuizCSVImports, "id=" + CP.Db.EncodeSQLNumber(recordId))) {
                        if (!string.IsNullOrEmpty(cs.GetText("csvFilename"))) {
                            return CP.CdnFiles.FileExists(cs.GetFilename("csvFilename").Replace("/", @"\"));
                        }
                    }
                    return false;
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in QuizImports.ExistCsvFilename");
                return false;
            }
        }
        // 
        // 
        // 
        public static string getCsvFilename(CPBaseClass CP, int recordId) {
            try {
                using (var cs = CP.CSNew()) {
                    if (cs.Open(common.cnQuizCSVImports, "id=" + CP.Db.EncodeSQLNumber(recordId))) {
                        return CP.CdnFiles.PhysicalFilePath + cs.GetFilename("csvFilename").Replace("/", @"\");
                    }
                    return "";
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in QuizImports.GetCsvFilename");
                return "";
            }
        }
        // 
        // 
        // 
        public static void setProcessError(CPBaseClass CP, int RecordId, ref List<architectureModels.errorClass> errorList) {
            try {
                using (var cs = CP.CSNew()) {
                    string errorStr = "Process detect the follow errors:" + Environment.NewLine;
                    foreach (var oneError in errorList)
                        errorStr += oneError.userMsg + Environment.NewLine;
                    if (cs.Open(common.cnQuizCSVImports, "id=" + RecordId)) {
                        cs.SetField("importErrors", errorStr);
                    }
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in QuizImports.SetProcessError");
            }
        }
        // 
        // 
        // 
        public static bool setEndProcess(CPBaseClass CP, int RecordId) {
            bool Result = false;
            try {
                // 
                var cs = CP.CSNew();
                // 
                if (cs.Open(common.cnQuizCSVImports, "id=" + RecordId)) {
                    // 
                    cs.SetField("dateimportcompleted", DateTime.Now.ToString());
                    // 
                }
                cs.Close();
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in QuizImports.SetEndProcess");
            }
            return Result;
        }
        // 
        // 
        // 
        public static bool SendNotificationEmail(CPBaseClass CP, int RecordId) {
            bool Result = false;
            try {
                // 
                var cs = CP.CSNew();
                string email = "";
                // 
                if (cs.Open(common.cnQuizCSVImports, "id=" + RecordId)) {
                    // 
                    email = cs.GetText("notificationEmail");
                    // 
                }
                cs.Close();

                CP.Email.sendSystem("Quiz Import Notification Task");

                if (!string.IsNullOrEmpty(email)) {
                    string htmlBody = CP.Content.GetCopy("Quiz Import Process Task Content");
                    string fromEmail = CP.Site.GetText("Quiz Import Process From Email");
                    string subjectEmail = CP.Site.GetText("Quiz Import Process Subject Email");
                    CP.Email.send(email, fromEmail, subjectEmail, htmlBody);
                }

                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in QuizImports.SetEndProcess");
            }
            return Result;
        }
        // 
        // 
        // 
        public static bool SetCSVFile(CPBaseClass CP, ref int RecordId, ref List<architectureModels.errorClass> errorList) {
            bool result = false;
            try {
                var cs = CP.CSNew();

                if (RecordId != 0) {
                    if (cs.Open(common.cnQuizCSVImports, "id=" + RecordId)) {
                        // 
                        cs.SetFormInput("csvFilename", "csvFilename");
                        // 
                    }
                    cs.Close();
                    // 
                } else {
                    // 
                    var oneError = new architectureModels.errorClass();
                    oneError.number = 320;
                    oneError.userMsg = "App Id is not Valid.";
                    errorList.Add(oneError);
                }
                // 
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Model.dbModels.Patient.SetPatientFile");
                var oneError = new architectureModels.errorClass();
                oneError.number = 300;
                oneError.userMsg = "Error in Set CSV File.";
                errorList.Add(oneError);
            }
            return result;
        }
        // 
        // 
        // 
    }
    // 
}