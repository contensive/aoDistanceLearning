
using System;
using System.Collections.Generic;
using System.Text;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using Contensive.Addons.DistanceLearning.Views;
using Contensive.Addons.DistanceLearning.Controllers;
using System.Globalization;
using Microsoft.VisualBasic;
using System.Linq;

namespace Contensive.Addons.DistanceLearning.Controllers {
    public static class GenericController {
        //
        //====================================================================================================
        /// <summary>
        /// true if argument is numeric
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool isNumeric(string value) {
            return value.All(char.IsNumber);
        }
        // 
        // ====================================================================================================
        /// <summary>
        /// buffer an url to include protocol
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string verifyProtocol(string url) {
            // 
            // -- allow empty
            if ((string.IsNullOrWhiteSpace(url)))
                return string.Empty;
            // 
            // -- allow /myPage
            if ((url.Substring(0, 1) == "/"))
                return url;
            // 
            // -- allow if it includes ://
            if ((!url.IndexOf("://").Equals(-1)))
                return url;
            // 
            // -- add http://
            return "http://" + url;
        }
        //
        // create main form tab container
        public static string getTabWrapper(CPBaseClass cp, string innerBody, string activeTabCaption, QuizModel quiz) {
            //adminFramework.formSimpleClass formOuter = new adminFramework.formSimpleClass();
            //formOuter.isOuterContainer = false;
            // formOuter.title = "Settings";
            //formOuter.title = "this is the title";
            //formOuter.body = innerBody;
            // formOuter.addFormButton("Save", "button");
            // formOuter.addFormButton("Cancel", "button");
            string qs;
            qs = cp.Doc.RefreshQueryString;
            //formOuter.formActionQueryString = qs;
            qs = cp.Utils.ModifyQueryString(qs, "quizId", quiz.id.ToString());
            //
            adminFramework.contentWithTabsClass tabForm = new adminFramework.contentWithTabsClass();
            if (!string.IsNullOrEmpty(quiz.name)) {
                tabForm.title = quiz.name;
            } else {
                tabForm.title = "Quiz " + quiz.id;
            }
            tabForm.isOuterContainer = true;
            tabForm.body = innerBody; // formOuter.getHtml(cp);
            //
            tabForm.addTab();
            tabForm.tabCaption = "Details";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeaturesQuizOverviewDetails);
            tabForm.tabStyleClass = "";

            //
            tabForm.addTab();
            tabForm.tabCaption = "Settings";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewSetting);
            tabForm.tabStyleClass = "";
            //
            tabForm.addTab();
            tabForm.tabCaption = "Study";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewStudyPage);
            tabForm.tabStyleClass = "";
            //
            tabForm.addTab();
            tabForm.tabCaption = "Questions";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewQuestionList);
            tabForm.tabStyleClass = "";
            //
            tabForm.addTab();
            tabForm.tabCaption = "Scoring";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewScoring);
            tabForm.tabStyleClass = "";
            //
            tabForm.addTab();
            tabForm.tabCaption = "Results";
            tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", Constants.portalFeatureQuizOverviewResults);
            tabForm.tabStyleClass = "";
            //
            //tabForm.addTab();
            //tabForm.tabCaption = "Results Details";
            //tabForm.tabLink = "?" + cp.Utils.ModifyQueryString(qs, "dstFeatureGuid", constants.portalFeatureQuizOverviewResultsDetails);
            //tabForm.tabStyleClass = "";

            tabForm.setActiveTab(activeTabCaption);
            return tabForm.getHtml(cp);
        }
        //
        //=========================================================================
        //  create user error if requestName field is not in doc properties
        //=========================================================================
        //
        public static void checkRequiredFieldText(CPBaseClass cp, string requestName, string fieldCaption) {
            try {
                if (cp.Doc.GetProperty(requestName, "") == "") {
                    cp.UserError.Add("The field " + fieldCaption + " is required.");
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "Unexpected Error in checkRequiredFieldText");
            }
        }
        //
        //=========================================================================
        //  get the field value, from cs if ok, else from stream
        //=========================================================================
        //
        public static string getFormField(CPBaseClass cp, CPCSBaseClass cs, string fieldName, string requestName) {
            string returnValue = "";
            try {
                if (cp.Doc.IsProperty(requestName)) {
                    returnValue = cp.Doc.GetText(requestName, "");
                } else if (cs.OK()) {
                    returnValue = cs.GetText(fieldName);
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "Unexpected Error in getFormField");
            }
            return returnValue;
        }
        //
        //=========================================================================
        //  getFormField, variation
        //=========================================================================
        //
        public static string getFormField(CPBaseClass cp, CPCSBaseClass cs, string fieldName) {
            return getFormField(cp, cs, fieldName, fieldName);
        }
        //
        //=========================================================================
        //  if valid date, return the short date, else return blank string 
        //=========================================================================
        //
        public static DateTime encodeMinDate(DateTime src) {
            return src;
        }
        //
        public static string getShortDateString(DateTime srcDate) {
            string returnString = "";
            DateTime workingDate = encodeMinDate(srcDate);
            if (!isDateEmpty(srcDate)) {
                returnString = workingDate.ToShortDateString();
            }
            return returnString;
        }
        //
        public static bool isDateEmpty(DateTime srcDate) {
            return (srcDate < new DateTime(1900, 1, 1));
        }
        public static string getSortOrderFromInteger(int id) {
            return id.ToString().PadLeft(7, '0');
        }
        public static string getDateForHtmlInput(DateTime source) {
            if (isDateEmpty(source)) {
                return "";
            } else {
                return source.Year + "-" + source.Month.ToString().PadLeft(2, '0') + "-" + source.Day.ToString().PadLeft(2, '0');
            }
        }

        // ====================================================================================================
        /// <summary>
        ///         ''' convert string into a style "height: {styleHeight};", if value is numeric it adds "px"
        ///         ''' </summary>
        ///         ''' <param name="styleheight"></param>
        ///         ''' <returns></returns>
        public static string encodeStyleHeight(string styleheight) {
            return string.IsNullOrWhiteSpace(styleheight) ? string.Empty : "overflow:hidden;height:" + styleheight + (Information.IsNumeric(styleheight) ? "px" : string.Empty + ";");
        }
        // 
        // ====================================================================================================
        /// <summary>
        ///         ''' convert string into a style "background-image: url(backgroundImage)
        ///         ''' </summary>
        ///         ''' <param name="backgroundImage"></param>
        ///         ''' <returns></returns>
        public static string encodeStyleBackgroundImage(CPBaseClass cp, string backgroundImage) {
            return string.IsNullOrWhiteSpace(backgroundImage) ? string.Empty : "background-image: url(" + cp.Site.FilePath + backgroundImage + ");";
        }

        // 
        public static DistanceLearning.Models.QuizResponseModel createNewQuizResponse(CPBaseClass cp, DistanceLearning.Models.QuizModel quiz) {
            DistanceLearning.Models.QuizResponseModel response = null/* TODO Change to default(_) if this is not a reference type */;
            try {
                List<DistanceLearning.Models.QuizResponseModel> previousResponses = DistanceLearning.Models.QuizResponseModel.GetResponseList(cp, quiz.id, cp.User.Id);
                // 
                // -- all previous responses must be complete
                if (previousResponses.Count > 0) {
                    foreach (var responsex in previousResponses) {
                        if ((!DistanceLearning.Controllers.GenericController.isDateEmpty(responsex.dateSubmitted))) {
                            responsex.dateSubmitted = DateTime.Now;
                            responsex.save(cp);
                        }
                    }
                }
                // 
                // -- housekeep subject FK
                cp.Db.ExecuteSQL("update quizquestions set subjectid=null where id in (select q.id from quizquestions q left join quizsubjects s on s.id=q.subjectid where s.id is null)");
                // 
                // -- add a new response, and create all the response details (with no answer selected)
                response = DistanceLearning.Models.QuizResponseModel.addDefault(p, quiz.id);
                response.name = cp.User.Name + ", " + DateTime.Now.ToShortDateString() + ", " + quiz.name;
                response.MemberID = cp.User.Id;
                response.attemptNumber = previousResponses.Count + 1;
                response.save(cp);
                // 
                List<DistanceLearning.Models.QuizSubjectModel> quizSubjectList = DistanceLearning.Models.QuizSubjectModel.getObjectList(cp, quiz.id);
                List<DistanceLearning.Models.QuizQuestionModel> quizQuestionList = new List<DistanceLearning.Models.QuizQuestionModel>();
                List<DistanceLearning.Models.QuizQuestionModel> quizQuestionFullList = DistanceLearning.Models.QuizQuestionModel.getQuestionsForQuizList(cp, quiz.id);
                if ((quiz.maxNumberQuest == 0) | (quiz.maxNumberQuest >= quizQuestionFullList.Count)) {
                    // 
                    // -- include all questions that have subjects, ordered by subject order
                    foreach (DistanceLearning.Models.QuizSubjectModel subject in quizSubjectList) {
                        foreach (DistanceLearning.Models.QuizQuestionModel quizQuestion in quizQuestionFullList) {
                            if ((quizQuestion.SubjectID == subject.id))
                                quizQuestionList.Add(quizQuestion);
                        }
                    }
                    // 
                    // -- include all questions with no subjects
                    foreach (DistanceLearning.Models.QuizQuestionModel quizQuestion in quizQuestionFullList) {
                        if ((quizQuestion.SubjectID == 0))
                            quizQuestionList.Add(quizQuestion);
                    }
                } else {
                    // 
                    // -- include a random set of questions
                    Random random = new Random();
                    if ((quizSubjectList.Count > 0)) {
                        // 
                        // -- subjects included, add maxNumberQuest to each subject
                        foreach (DistanceLearning.Models.QuizSubjectModel subject in quizSubjectList) {
                            // 
                            // -- create list of all questions in the subject
                            List<DistanceLearning.Models.QuizQuestionModel> subjectQuestionList = new List<DistanceLearning.Models.QuizQuestionModel>();
                            foreach (DistanceLearning.Models.QuizQuestionModel question in quizQuestionFullList) {
                                if (question.SubjectID == subject.id)
                                    subjectQuestionList.Add(question);
                            }
                            // 
                            // -- add random maxNumberQuest to quizquestionList
                            if ((subjectQuestionList.Count > 0)) {
                                for (int questionPtr = 0; questionPtr <= quiz.maxNumberQuest - 1; questionPtr++) {
                                    if ((subjectQuestionList.Count == 0))
                                        // 
                                        // -- no more questions in this subject
                                        break;
                                    else {

                                        int indexTest = random.Next(0, subjectQuestionList.Count);
                                        DistanceLearning.Models.QuizQuestionModel question = subjectQuestionList[indexTest];
                                        quizQuestionList.Add(question);
                                        subjectQuestionList.Remove(question);
                                    }
                                }
                            }
                        }
                    } else
                        // 
                        // -- no subjects, add maxNumberQuest questions to quiz
                        for (int questionPtr = 0; questionPtr <= quiz.maxNumberQuest - 1; questionPtr++) {
                            int indexTest = random.Next(0, quizQuestionFullList.Count);
                            DistanceLearning.Models.QuizQuestionModel question = quizQuestionFullList[indexTest];
                            quizQuestionList.Add(question);
                            quizQuestionFullList.Remove(question);
                        }
                }
                // 
                // -- first create quiz pages that have subjects
                int pageNumber = 1;
                int detailSortOrder = 0;
                foreach (DistanceLearning.Models.QuizSubjectModel quizSubject in quizSubjectList) {
                    int subjectQuestionCount = 0;
                    foreach (DistanceLearning.Models.QuizQuestionModel quizQuestion in quizQuestionList) {
                        if (quizQuestion.SubjectID == quizSubject.id) {
                            DistanceLearning.Models.QuizResponseDetailModel detail = DistanceLearning.Models.QuizResponseDetailModel.addDefault(p);
                            detail.questionId = quizQuestion.id;
                            detail.responseId = response.id;
                            detail.pageNumber = pageNumber;
                            detail.SortOrder = quizSubject.name + quizQuestion.SortOrder; // detailSortOrder.ToString.PadLeft(7, "0"c)
                            detail.save(cp);
                            if ((quiz.questionPresentation == (int)DistanceLearning.Models.QuizModel.questionPresentationEnum.OneQuestionPerPage))
                                pageNumber += 1;
                            subjectQuestionCount += 1;
                            detailSortOrder += 1;
                        }
                    }
                    if ((subjectQuestionCount > 0) & (quiz.questionPresentation == (int)DistanceLearning.Models.QuizModel.questionPresentationEnum.OneSubjectPerPage))
                        pageNumber += 1;
                }
                // 
                // -- then add pages for questions with no subjects
                foreach (DistanceLearning.Models.QuizQuestionModel quizQuestion in quizQuestionList) {
                    bool addDetail = true;
                    if (quizQuestion.SubjectID > 0) {
                        bool subjectFound = false;
                        foreach (DistanceLearning.Models.QuizSubjectModel quizSubject in quizSubjectList) {
                            subjectFound = Equals(quizQuestion.SubjectID, quizSubject.id);
                            if ((subjectFound))
                                break;
                        }
                        addDetail = !subjectFound;
                    }
                    if (addDetail) {
                        DistanceLearning.Models.QuizResponseDetailModel detail = DistanceLearning.Models.QuizResponseDetailModel.addDefault(p);
                        detail.questionId = quizQuestion.id;
                        detail.responseId = response.id;
                        detail.pageNumber = pageNumber;
                        detail.SortOrder = detailSortOrder.ToString().PadLeft(7, '0');
                        detail.save(cp);
                        if ((quiz.questionPresentation == (int)DistanceLearning.Models.QuizModel.questionPresentationEnum.OneQuestionPerPage))
                            pageNumber += 1;
                        detailSortOrder += 1;
                    }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return response;
        }
        // 
        //        internal static DateTime encodeMinDate(DateTime sourceDate) {
        //            DateTime returnValue = sourceDate;
        //            ;/* Cannot convert MultiLineIfBlockSyntax, System.ArgumentOutOfRangeException: Exception of type 'System.ArgumentOutOfRangeException' was thrown.
        //Parameter name: value
        //Actual value was 1/1/1900 12:00:00 AM.
        //   at ICSharpCode.CodeConverter.CSharp.CommonConversions.GetLiteralExpression(Object value, String valueText)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitLiteralExpression(LiteralExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitLiteralExpression(LiteralExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitBinaryExpression(BinaryExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitBinaryExpression(BinaryExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

        //Input: 
        //            If returnValue < #1/1/1900# Then
        //                returnValue = Date.MinValue
        //            End If

        // */
        //            return returnValue;
        //        }
        // 
        // 
        // 
        //        internal static string encodeShortDateString(DateTime sourceDate) {
        //            string returnValue;
        //            ;/* Cannot convert MultiLineIfBlockSyntax, System.ArgumentOutOfRangeException: Exception of type 'System.ArgumentOutOfRangeException' was thrown.
        //Parameter name: value
        //Actual value was 1/1/1900 12:00:00 AM.
        //   at ICSharpCode.CodeConverter.CSharp.CommonConversions.GetLiteralExpression(Object value, String valueText)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitLiteralExpression(LiteralExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitLiteralExpression(LiteralExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.LiteralExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.NodesVisitor.VisitBinaryExpression(BinaryExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingNodesVisitor.DefaultVisit(SyntaxNode node)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.VisitBinaryExpression(BinaryExpressionSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.BinaryExpressionSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at ICSharpCode.CodeConverter.CSharp.VisualBasicConverter.MethodBodyVisitor.VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
        //   at Microsoft.CodeAnalysis.VisualBasic.Syntax.MultiLineIfBlockSyntax.Accept[TResult](VisualBasicSyntaxVisitor`1 visitor)
        //   at Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor`1.Visit(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.ConvertWithTrivia(SyntaxNode node)
        //   at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisit(SyntaxNode node)

        //Input: 
        //            '
        //            If sourceDate < #1/1/1900# Then
        //                returnValue = ""
        //            Else
        //                returnValue = sourceDate.ToShortDateString
        //            End If

        // */
        //            return returnValue;
        //        }
        // 
        // 
        // 
        internal static string encodeBlankCurrency(double source) {
            string returnValue = "";
            if (source != 0)
                returnValue = source.ToString("C", CultureInfo.CurrentCulture);
            return returnValue;
        }

        public static string addEditWrapper(CPBaseClass cp, string innerHtml, int recordId, string recordName, string contentName) {
            if (!cp.User.IsEditingAnything) {
                return innerHtml;
            }
            string header = cp.Content.GetEditLink(contentName, recordId.ToString(), false, recordName, true);
            string content = cp.Html.div(innerHtml, "", "");
            string miniWrapperCSS = "style=\"white-space: nowrap; text-align: left; box-sizing: border-box; color: white !important; z-index: 1; text-decoration: none !important; display: inline-block !important; list-style-type: none !important; margin: 0 auto auto 0 !important; padding: 2px 10px !important; font-size: 1rem !important; font-weight: 400 !important; line-height: 1.5 !important;\"";
            return cp.Html.div(header + content, "", "ccEditWrapper");
        }
    }
}

