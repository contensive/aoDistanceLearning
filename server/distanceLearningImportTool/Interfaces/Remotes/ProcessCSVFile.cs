using System;
using System.Collections.Generic;
using System.Text.Json;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Interfaces.Remotes {
    // 
    public class ProcessCSVFile : AddonBaseClass {
        // 
        // 
        // 
        public override object Execute(CPBaseClass CP) {
            string returnHtml;
            var remoteResponse = new Model.architectureModels.remoteResponseObject();
            try {
                // 
                int recordId = CP.Doc.GetInteger("recordId");
                // 
                // CP.Utils.AppendLog("ProcessCSVFile.log","recordId: " & recordId)
                // 
                if (recordId != 0) {
                    // 

                    if (Model.dbModels.QuizImports.existCsvFilename(CP, recordId)) {
                        string filename = Model.dbModels.QuizImports.getCsvFilename(CP, recordId);

                        var csvObjectList = Model.solutionModels.csvFile.GetCsvFileObjectList(CP, filename, ref remoteResponse.errors);
                        // 
                        var QuizObject = Model.solutionModels.Quizzes.GetQuizzesObject(CP, filename, csvObjectList, ref remoteResponse.errors);
                        // 

                        if (remoteResponse.errors.Count > 0) {
                            Model.dbModels.QuizImports.setProcessError(CP, recordId, ref remoteResponse.errors);
                        } else {
                            // 
                            // Process / Create database records
                            // 
                            Model.solutionModels.Quizzes.CreateDBRecords(CP, QuizObject);
                            // 
                            // remoteResponse.data = QuizObject
                        }
                        // 
                        Model.dbModels.QuizImports.setEndProcess(CP, recordId);
                        // 
                        Model.dbModels.QuizImports.SendNotificationEmail(CP, recordId);
                        // 

                    } else {
                        // CP.Utils.AppendLog("ProcessCSVFile.log","ExistCsvFilename: False")
                    }
                    // 
                } else {
                    remoteResponse.data = "ProcessCSVFile Error";
                    // send message error
                    var oneError = new Model.architectureModels.errorClass();
                    oneError.number = 200;
                    oneError.userMsg = "User Is Not Authenticate.";
                    remoteResponse.errors.Add(oneError);
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
        private void errorReport(CPBaseClass cp, Exception ex, string @method) {
            try {
                cp.Site.ErrorReport(ex, "Unexpected error in GetPatients." + @method);
            } catch (Exception) {
                //
                //
                //
            }
        }
        // 
        // 
        // 
    }
    // 
}