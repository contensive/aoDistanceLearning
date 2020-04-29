using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Views;
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Models.Db;

namespace Contensive.Addons.DistanceLearning
{
   public class quizOverviewQuestionListClass : Contensive.BaseClasses.AddonBaseClass
   {
        public override object Execute(CPBaseClass cp)
        {
            string result = "";
            try
            {
                CPCSBaseClass cs = cp.CSNew();
                QuizModel quiz = DbBaseModel.create<QuizModel>(cp, cp.Doc.GetInteger("QuizId"));
                QuizQuestionModel quizQuestion = QuizQuestionModel.create(cp, cp.Doc.GetInteger("ID"));
                string button = cp.Doc.GetText("button");
            switch (button)
            {
                case "Edit":
                    string qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewQuestionDetails);
                        qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuizId, cp.Doc.GetInteger(Constants.rnQuizId).ToString());
                        qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuestionId, cp.Doc.GetInteger(Constants.rnQuestionId).ToString());
                    cp.Response.Redirect("?" + qs);
                    break;
                case "Delete":
                    QuizQuestionModel.delete(cp, cp.Doc.GetInteger(Constants.rnQuestionId));
                    break;
                case "AddQuestion":
                    qs = cp.Doc.RefreshQueryString;
                    qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewQuestionDetails);
                    qs = cp.Utils.ModifyQueryString(qs, Constants.rnQuizId, cp.Doc.GetInteger(Constants.rnQuizId).ToString());
                        cp.Response.Redirect("?" + qs);
                       // qs = cp.Utils.ModifyQueryString(qs, constants.rnQuestionId, " ");
                        
                    break;
            }

                // string qs;
                // string qsBase;
                //string rqs = "";
                
                if (quiz == null)
                {
                    return "";
                }
                adminFramework.ReportListClass reportList = new adminFramework.ReportListClass(cp);
                reportList.isOuterContainer = false;
                //reportList.title = "Distance Learning";
                reportList.addColumn();
                reportList.columnCaption = "Subject";
                reportList.columnCaptionClass = "afwTextAlignLeft afwWidth200px";
                //
                reportList.addColumn();
                reportList.columnCaption = "Questions    ";
                reportList.columnCaptionClass = "afwTextAlignright";
                reportList.addColumn();
                reportList.columnCaption = "Sort Order";
                reportList.columnCaptionClass = "afwTextAlignright afwWidth100px";
                //
                reportList.addColumn();
                reportList.columnCaption = " ";
                reportList.columnCaptionClass = "afwTextAlignright afwWidth100px";
                //
             QuizQuestionModel quizQuestions = QuizQuestionModel.create(cp, cp.Doc.GetInteger("ID"));
                string addButtonForm="";
                addButtonForm = cp.Html.Button("Button", "AddQuestion", "addQuestionClass btn btn-primary", "js-addQuestionButtonId");
                addButtonForm += cp.Html.Hidden(Constants.rnQuizId, quiz.id.ToString());
                addButtonForm = cp.Html.Form(addButtonForm);
                reportList.htmlAfterTable = addButtonForm;
                //addButtonForm
                //
                // the following is creating a list of questions from the question model
                List<QuizQuestionModel> questionList = QuizQuestionModel.getQuestionsForQuizList(cp, quiz.id);
                //
                // the following is modifying a refreshquery string to add to the query model
                // 
                List<Models.QuizSubjectModel> subjectList = Models.QuizSubjectModel.getObjectList(cp);
                //
                // - add empty subject at end for questions without subjects
                subjectList.Add(new QuizSubjectModel() { });
                List<int> usedSubjectIdList = new List<int>();
                foreach (QuizSubjectModel subject in subjectList)
                {
                    usedSubjectIdList.Add(subject.id);
                    //
                    // -- list all questions in this subject
                    foreach (QuizQuestionModel question in questionList)
                    {
                        //
                        // -- list this question if it is in this subject
                        if (subject.id == question.SubjectID)
                        {
                            addQuestionToList(cp, question, subject, reportList);
                        }
                    }
                }
                foreach (QuizQuestionModel question in questionList)
                {
                    if (!usedSubjectIdList.Contains(question.SubjectID))
                    {
                        addQuestionToList(cp, question, null, reportList);
                    }
                }

                    cp.Doc.AddRefreshQueryString("quizId", quiz.id.ToString());
                //
                result = GenericController.getTabWrapper(cp, reportList.getHtml(cp), "Questions", quiz);

                cp.Doc.AddHeadStyle(reportList.styleSheet);
            }
            catch (Exception ex)
            {
                cp.Site.ErrorReport( ex, "execute");
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
        private void addQuestionToList(CPBaseClass cp, QuizQuestionModel question, QuizSubjectModel subject,  adminFramework.ReportListClass reportList)
        {
            List<QuizResponseModel> responseList = QuizResponseModel.GetResponseList(cp, question.id);
            reportList.addRow();
            reportList.columnCellClass = "afwTextAlignLeft";
            if (subject == null)
            {
                reportList.setCell("");
            }
            else
            {
                reportList.setCell(subject.name);
            }
            //
            reportList.columnCellClass = "afwTextAlignLeft";
            reportList.setCell(question.copy);
            reportList.setCell(question.SortOrder.ToString());
            //
            string miniForm = "";
            miniForm += cp.Html.Button("button", "Edit", "questionEdit btn btn-primary", "js-questionEdit");
            miniForm += cp.Html.Button("button", "Delete", "questionDelete btn btn-primary", "js-questionDelete");
            miniForm += cp.Html.Hidden(Constants.rnQuestionId, question.id.ToString());
            miniForm += cp.Html.Hidden(Constants.rnQuizId, question.quizId.ToString());
            miniForm = cp.Html.Form(miniForm);
            reportList.columnCellClass = "afwTextAlignLeft";
            reportList.setCell(miniForm);
        }
    }
}




