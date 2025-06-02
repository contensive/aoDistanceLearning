
using Contensive.Addons.DistanceLearning.Controllers;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;

namespace Contensive.Addons.DistanceLearning {
    public class QuizOverViewSettingClass : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            string result = "";
            string innerBody = "";
            string qs = "";
            int setportalId = cp.Doc.GetInteger("setPortalId");

            try {
                QuizModel quiz;
                string quizName = cp.Doc.GetText("quizName");
                if (quizName != String.Empty) {
                    quiz = DbBaseModel.addDefault<QuizModel>(cp);
                    quiz.name = quizName;
                    quiz.save(cp);
                } else {
                    int quizId = cp.Doc.GetInteger("QuizId");
                    quiz = DbBaseModel.create<QuizModel>(cp, quizId);
                }
                if (quiz == null) {
                    return "";
                }

                string button = cp.Doc.GetText("button");
                switch (button) {
                    case "Save":
                        quiz.name = cp.Doc.GetText("name");
                        quiz.allowRetake = cp.Doc.GetBoolean("allowRetake");
                        quiz.questionPresentation = cp.Doc.GetInteger("questionPresentation");
                        quiz.maxNumberQuest = cp.Doc.GetInteger("maxNumberQuest");
                        quiz.customButtonCopy = cp.Doc.GetText(nameof(quiz.customButtonCopy));
                        string subjectNameEditList = cp.Doc.GetText(Constants.rnSubjectNameEditList);
                        if (true) {
                            string subjectIdEditList = cp.Doc.GetText(Constants.rnSubjectIdEditList);
                            List<string> subjectNameList = new List<string>();
                            if (!string.IsNullOrEmpty(subjectNameEditList)) {
                                subjectNameList.AddRange(subjectNameEditList.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
                            }
                            List<string> subjectIdList = new List<string>();
                            if (!string.IsNullOrEmpty(subjectIdEditList)) {
                                subjectIdList.AddRange(subjectIdEditList.Split(new string[] { "," }, StringSplitOptions.None));
                            }
                            for (int ptr = 0; ptr < subjectIdList.Count; ptr++) {
                                int subjectId = 0;
                                if (int.TryParse(subjectIdList[ptr], out subjectId)) {
                                    if (ptr >= subjectNameList.Count) {
                                        // -- past the end of the list of names, delete this id
                                        DbBaseModel.delete<QuizSubjectModel>(cp, subjectId);
                                    } else {
                                        QuizSubjectModel subject = DbBaseModel.create<QuizSubjectModel>(cp, subjectId);
                                        if (subject != null) {
                                            if (string.IsNullOrEmpty(subjectNameList[ptr].Trim())) {
                                                // -- name is a blank line, delete the subject
                                                DbBaseModel.delete<QuizSubjectModel>(cp, subjectId);
                                            } else {
                                                // -- update the subject name 
                                                if (subject.name != subjectNameList[ptr]) {
                                                    subject.name = subjectNameList[ptr];
                                                    subject.save(cp);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (subjectNameList.Count > subjectIdList.Count) {
                                // -- they added more to the names, insert them as new subject
                                for (int ptr = subjectIdList.Count; ptr < subjectNameList.Count; ptr++) {
                                    Models.QuizSubjectModel subject = DbBaseModel.addDefault<QuizSubjectModel>(cp);
                                    subject.name = subjectNameList[ptr];
                                    subject.quizId = quiz.id;
                                    subject.save(cp);
                                }
                            }
                        }
                        quiz.save(cp);
                        break;
                    case "Cancel":
                        qs = cp.Doc.RefreshQueryString;
                        qs = cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails, true);
                        qs = cp.Utils.ModifyQueryString(qs, "QuizId", quiz.id.ToString(), true);
                        cp.Response.Redirect("?" + qs);
                        break;
                }
                //
                BaseClasses.LayoutBuilder.LayoutBuilderNameValueBaseClass layout = cp.AdminUI.CreateLayoutBuilderNameValue();
                layout.isOuterContainer = false;
                layout.addFormHidden("QuizId", quiz.id.ToString());
                layout.body = innerBody;
                layout.addFormButton("Save", "button");
                layout.addFormButton("Cancel", "button");
                //
                layout.title = "Settings";
                layout.isOuterContainer = false;
                layout.addRow();
                layout.rowName = "Quiz Name";
                layout.rowValue = cp.Html.InputText("name", quiz.name);
                layout.rowHelp = "You can choose to Change the name of the quiz.";
                layout.addRow();
                layout.rowName = "Question Presentation";
                layout.rowValue = cp.Html.SelectList("questionPresentation", quiz.questionPresentation.ToString(), "All questions on one page, One subject per page, One question per page.", "Select Type of Presentation");
                layout.rowHelp = "You can choose to display one question per page, all questions on one page , or all subject questions per page.";
                layout.addRow();
                layout.rowName = "Allow users to retake";
                layout.rowValue = cp.Html.CheckBox("allowRetake", quiz.allowRetake);
                layout.rowHelp = "If this box is checked users can choose to retake the quiz.";
                layout.addRow();
                layout.rowName = "Max questions to display";
                layout.rowValue = cp.Html.InputText("maxNumberQuest", quiz.maxNumberQuest.ToString());
                layout.rowHelp = "This is the max number of questions that will display per quiz or per subject(if subjects are used ). The system will randomly select"
                    + " questions from the available pool up to the max number entered. If user is allowed to retake it will display a random selection from the pool"
                    + " so the user doesnt get the same quiz twice";
                //
                // -- build subjects list with subject id list to handle edits
                layout.addRow();
                layout.rowName = "Include Subjects";
                List<Models.QuizSubjectModel> subjectList = DbBaseModel.createList<QuizSubjectModel>(cp, "(quizId=" + quiz.id + ")");
                string subjectNameTextList = "";
                string subjectIdCommaList = "";
                string nameDelimiter = "";
                string idDelimiter = "";
                foreach (Models.QuizSubjectModel subject in subjectList) {
                    subjectNameTextList += nameDelimiter + subject.name;
                    nameDelimiter = Environment.NewLine;
                    subjectIdCommaList += idDelimiter + subject.id;
                    idDelimiter = ",";
                }
                layout.rowValue = cp.Html.InputTextExpandable(Constants.rnSubjectNameEditList, subjectNameTextList, 5) + cp.Html.Hidden(Constants.rnSubjectIdEditList, subjectIdCommaList);
                layout.rowHelp = "If you wish to organize your questions by subject, enter the subject section in the text box one subject per line. If this quiz has no sections leave blank</p>";
                //
                layout.addRow();
                layout.rowName = "Button Instructions";
                layout.rowValue = cp.Html.InputTextExpandable("customButtonCopy", quiz.customButtonCopy, 5);
                layout.rowHelp = "If included, this copy is added at the bottom of the page on each oneline quiz page.";
                //
                result = GenericController.getTabWrapper(cp, layout.getHtml(), "Settings", quiz);
                cp.Doc.AddHeadStyle(layout.styleSheet);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "execute");
            }
            return result;
        }
        //
        // ===============================================================================
        // handle errors for this class
        // ===============================================================================
        //
        private void errorReport(CPBaseClass cp, Exception ex, string method) {
            cp.Site.ErrorReport(ex, "error in addonTemplateCs2005.blankClass.getForm");
        }
    }

}