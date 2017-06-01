using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning
{
   public class quizOverviewQuestionsClass : Contensive.BaseClasses.AddonBaseClass
   {
        public override object Execute(CPBaseClass cp)
        {
            string result = "";
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger("QuizId"));
                QuizQuestionModel quizQuestion = QuizQuestionModel.create(cp, cp.Doc.GetInteger("ID"));
                string button = cp.Doc.GetText("button");
            switch (button)
            {
                case "Edit":
                    string qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureQuizOverviewQuestionDetails);
                        qs = cp.Utils.ModifyQueryString(qs, constants.rnQuizId, cp.Doc.GetInteger(constants.rnQuizId).ToString());
                        qs = cp.Utils.ModifyQueryString(qs, constants.rnQuestionId, cp.Doc.GetInteger(constants.rnQuestionId).ToString());
                    cp.Response.Redirect("?" + qs);
                    break;
                case "Delete":
                    QuizQuestionModel.delete(cp, cp.Doc.GetInteger(constants.rnQuestionId));
                    break;
                case "AddQuestion":
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureQuizOverviewQuestionDetails);
                    qs = cp.Utils.ModifyQueryString(qs, constants.rnQuestionId, cp.Doc.GetInteger(constants.rnQuestionId).ToString());
                    cp.Response.Redirect("?" + qs);
                    break;
            }

                // string qs;
                // string qsBase;
                //string rqs = "";
                
                if (quiz == null)
                {
                    return "";
                }
                else
                {
                adminFramework.reportListClass reportList = new adminFramework.reportListClass(cp);                                           
                reportList.isOuterContainer = false;
                //reportList.title = "Distance Learning";
                reportList.addColumn();
                reportList.columnCaption = "Subject";
                reportList.columnCaptionClass = "afwTextAlignLeft afwWidth200px";
                //
                reportList.addColumn();
                reportList.columnCaption = "Questions    " + cp.Html.Button("Button", "AddQuestion", "addQuestionClass", "js-addQuestionButtonId");
                reportList.columnCaptionClass = "afwTextAlignright afwWidth50px";               
                //
                // the following is creating a list of questions from the question model
                List<QuizQuestionModel> questionList = QuizQuestionModel.getQuestionsForQuizList(cp,quiz.id);
                //
                // the following is modifying a refreshquery string to add to the query model
                //qsBase = cp.Utils.ModifyQueryString(rqs, constants.rnAddonguid, constants.quizOverViewSelectAddon, true);
                //
                // 
                foreach (QuizQuestionModel question in questionList)
                {                       
                    //
                    //this next statement is like a cs.open but opens the object to get there field
                    List<QuizResponseModel> responseList = QuizResponseModel.getObjectList(cp, question.id);                        
                    reportList.addRow();                      
                    // the following is how to look up a field in another model
                    Models.QuizSubjectModel subject = QuizSubjectModel.create(cp, question.SubjectID);
                    reportList.setCell(subject.name);
                    reportList.columnCellClass = "afwTextAlignCenter";
                    string miniForm;
                    miniForm = question.QText;
                    miniForm += cp.Html.Button("button", "Edit", "questionEdit", "js-questionEdit");
                    miniForm += cp.Html.Button("button", "Delete", "questionDelete", "js-questionDelete");
                    miniForm += cp.Html.Hidden(constants.rnQuestionId, question.id.ToString());
                    miniForm = cp.Html.Form(miniForm);
                    reportList.setCell(miniForm);
                    reportList.columnCellClass = "afwTextAlignLeft";
                    cp.Doc.AddRefreshQueryString("quizId", quiz.id.ToString());

                }
 
                //
                result = genericController.getTabWrapper(cp, reportList.getHtml(cp), "Questions", quiz.id);

                    cp.Doc.AddHeadStyle(reportList.styleSheet);
                  
            }
            }
            catch (Exception ex)
            {
                errorReport(cp, ex, "execute");
            }
            return result;


        }
        //
        // ===============================================================================
        // handle errors for this class
        // ===============================================================================
        //
        private void errorReport(CPBaseClass cp, Exception ex, string method)
        {
            cp.Site.ErrorReport(ex, "error in addonTemplateCs2005.blankClass.getForm");
        }
    }
}




