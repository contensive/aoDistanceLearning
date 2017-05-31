using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Interfaces;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning
{
   public class quizOverviewQuestionPageClass : Contensive.BaseClasses.AddonBaseClass

        {
            public override object Execute(CPBaseClass cp)
            {
                string result = "";
                try
                {



                    string button = cp.Doc.GetText("button");
                    switch (button)
                    {
                        case "Edit":
                            string qs = cp.Doc.RefreshQueryString;
                       
                            qs = cp.Utils.ModifyQueryString(qs,"dstFeatureGuid", constants.portalQuestionDetailsPageaddon);
                            qs = cp.Utils.ModifyQueryString(qs, constants.rnQuestionId, cp.Doc.GetInteger(constants.rnQuestionId).ToString());
                            cp.Response.Redirect("?" + qs);
                            break;
                        case "Delete":
                           // QuizQuestionModel.delete(cp, question.id);
                            break;
                        case "AddQuestion":
                           // cp.Response.Redirect("?" = qs);
                            break;
                    }



                // string qs;
                string qsBase;
                    string rqs = "";
                    CPCSBaseClass cs = cp.CSNew();
                    QuizModel quiz = QuizModel.create(cp, cp.Doc.GetInteger("QuizId"));
                    QuizQuestionModel quizQuestion = QuizQuestionModel.create(cp, cp.Doc.GetInteger("ID"));
                    if (quiz == null)
                    {
                        return "";
                    }
                    else
                    {
                    //qs = cp.Doc.RefreshQueryString;
                    //qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalQuestionPageaddon, true);
                    //qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                    //qs = cp.Utils.ModifyQueryString(qs,"addonId", "");
                    //qs = cp.Utils.ModifyQueryString(qs,"addonGuid", constants.quizOverViewSelectAddon);
                    //    qsBase = cp.Utils.ModifyQueryString(rqs, constants.rnAddonguid, constants.quizOverViewSettingsAddon, true);
                    adminFramework.reportListClass reportList = new adminFramework.reportListClass(cp);
                   
                        //qs = cp.Utils.ModifyQueryString(qsBase, "QuizId", cs.GetInteger("responseId").ToString(), true);
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
                    // the following is modifying a refresshquery string to add to the query model
                    qsBase = cp.Utils.ModifyQueryString(rqs, constants.rnAddonguid, constants.quizOverViewSelectAddon, true);
                    //
                    // 
                    foreach (QuizQuestionModel question in questionList)
                    {
                        //
                        //this next statement is like a cs.open but opens the object to get there field
                        List<QuizResponseModel> responseList = QuizResponseModel.getObjectList(cp, question.id);
                        
                        reportList.addRow();
                       // qs = cp.Utils.ModifyQueryString(qsBase, "QuizId", cs.GetInteger("responseId").ToString(), true);
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
                        //question.saveObject(cp);
                        cp.Doc.AddRefreshQueryString("quizId", quiz.id.ToString());
                      //  qs = cp.Doc.RefreshQueryString;
                     //   qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid",constants.portalQuestionDetailsPageaddon);
                        //qs = cp.Utils.ModifyQueryString(qs,"RequestBinary", "");

                    }
                    //
                    result = genericController.getTabWrapper(cp, reportList.getHtml(cp), "Questions");

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




