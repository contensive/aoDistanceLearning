
using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Contensive.BaseClasses;
using static Contensive.Addons.DistanceLearning.Constants;
using Contensive.Models.Db;
using Contensive.Addons.DistanceLearning.Models;

namespace Contensive.Addons.DistanceLearning {
    namespace Views {
        public class ScoreCardClass : AddonBaseClass {
            //
            // =====================================================================================
            // 
            public override object Execute(CPBaseClass cp) {
                string returnHtml = "";
                try {
                    int responseId;
                    // 
                    responseId = cp.Doc.GetInteger("Id");
                    returnHtml = getScoreCard(cp, responseId);
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex, "execute");
                }
                return returnHtml;
            }
            // 
            // ========================================================================
            // 
            // ========================================================================
            // 
            private string getScoreCard(CPBaseClass cp, int responseId) {
                string returnHtml = "";
                int hint = 0;
                try {
                    // 
                    string chart = "";
                    string responseSubject;
                    string responseName;
                    string scoreCaption;
                    string Choice;
                    string ScoreBox;
                    // Dim TotalCorrect As Integer
                    // Dim TotalQuestions As Integer
                    // Dim answerId As Integer
                    // Dim AnswerCorrect As Boolean
                    // Dim cs As CPCSBaseClass = cp.CSNew()
                    // Dim cs2 As CPCSBaseClass = cp.CSNew()
                    // Dim CS3 As CPCSBaseClass = cp.CSNew()
                    CPCSBaseClass cs4 = cp.CSNew();
                    int totalQuestions;
                    string SummaryBox = "";
                    string questionRow;
                    // Dim SelectedAnswerID As Integer
                    bool Correct;
                    int totalQuestionsIncorrect;
                    int totalQuestionsCorrect;
                    double Percentage;
                    bool Passed;
                    // Dim SubjectID As Integer
                    subjectsStruc[] subjects = null;
                    // Dim questionId As Integer
                    int subjectCnt = 0;
                    int subjectPtr;
                    string GradeCaption = "";
                    // Dim OverallCaption As String
                    string[] SubjectCaptions = null;
                    int[] subjectScores = null;
                    string headerCopy;
                    string footerCopy;
                    // Dim quizName As String = ""
                    DateTime rightNow;
                    // Dim userId As Integer
                    // Dim quizId As Integer
                    // Dim answerPoints As Integer
                    // Dim resultPoints As Integer = 0
                    // 
                    rightNow = DateTime.Now;
                    // 
                    // only the responseId is valid, lookup the quiz and userId
                    // 
                    QuizResponseModel response = DbBaseModel.create<QuizResponseModel>(cp, responseId);
                    if ((response == null))
                        // 
                        // -- response is not valid
                        returnHtml = "<p>The requested response could not be found.</p>";
                    else {
                        List<DistanceLearning.Models.QuizResponseDetailModel> responseDetailList = DistanceLearning.Models.QuizResponseDetailModel.getObjectListForQuizDisplay(cp, response.id);
                        DistanceLearning.Models.QuizModel quiz = DbBaseModel.create<QuizModel>(cp, response.quizID);
                        if ((quiz == null))
                            // 
                            // -- quiz not valid
                            returnHtml = "<p>The quiz associated to the response requested is not currently longer available.</p>";
                        else {
                            subjectCnt = 0;
                            totalQuestions = 0;
                            totalQuestionsIncorrect = 0;
                            totalQuestionsCorrect = 0;
                            int totalPoints = 0;
                            foreach (DistanceLearning.Models.QuizResponseDetailModel responseDetail in responseDetailList) {
                                // 
                                // -- process all the questions selected in the resposne detail list
                                QuizQuestionModel question = DbBaseModel.create<QuizQuestionModel>(cp, responseDetail.questionId);
                                hint = 20;
                                totalQuestions = totalQuestions + 1;
                                // 
                                // Set subjectPtr to the Response Array for this question's subject
                                subjectPtr = 0;
                                if (question.subjectID > 0) {
                                    while ((subjectPtr < subjectCnt)) {
                                        if ((subjects[subjectPtr].SubjectID == question.subjectID))
                                            break;
                                        subjectPtr = subjectPtr + 1;
                                    }
                                    if (subjectPtr >= subjectCnt) {
                                        var oldSubjects = subjects;
                                        subjects = new subjectsStruc[subjectPtr + 1];
                                        if (oldSubjects != null)
                                            Array.Copy(oldSubjects, subjects, Math.Min(subjectPtr + 1, oldSubjects.Length));
                                        subjects[subjectPtr].SubjectID = question.subjectID;
                                        subjectCnt = subjectCnt + 1;
                                    }
                                    subjects[subjectPtr].TotalQuestions = subjects[subjectPtr].TotalQuestions + 1;
                                }
                                // 
                                // New Question
                                // 
                                hint = 30;
                                questionRow = "";
                                List<DistanceLearning.Models.QuizAnswerModel> answerList = DistanceLearning.Models.QuizAnswerModel.getAnswersForQuestionList(cp, question.id);
                                if ((answerList.Count == 0))
                                    // 
                                    // --Question with no choices is correct
                                    Correct = true;
                                else {
                                    // 
                                    // -- Add Choices
                                    Correct = false;
                                    foreach (DistanceLearning.Models.QuizAnswerModel answer in answerList) {
                                        // 
                                        // -- cycle through all the answers and build display
                                        Choice = "";
                                        if (answer.id == responseDetail.answerId) {
                                            // 
                                            // -- user selected this answer
                                            totalPoints += answer.points;
                                            if ((subjectCnt > subjectPtr) & (question.subjectID > 0))
                                                subjects[subjectPtr].points += answer.points;
                                            Choice = Choice + "<input type=\"radio\" class=\"questionRaioInput\" name=\"" + totalQuestions + "Answer\" value=\"" + answer.id.ToString() + "\" checked disabled>";
                                            Choice = Choice + "<div class=\"quizanswerClass\">" + answer.copy + "</div>";
                                            if (answer.correct)
                                                // 
                                                // selected answer is current answer
                                                Correct = true;
                                            questionRow = questionRow + "" + "" + "<div class=\"questionChoice\">" + Choice + "</div>";
                                        } else {
                                            // 
                                            // -- user did not select this answer
                                            Choice = Choice + "<input type=\"radio\" class=\"questionRaioInput\" name=\"" + totalQuestions + "Answer\" value=\"" + answer.id.ToString() + "\" disabled>";
                                            Choice = Choice + "<div class=\"quizanswerClass\">" + answer.copy + "</div>";
                                            if (answer.correct)
                                                // 
                                                // unselected answer is correct answer, make red
                                                // 
                                                questionRow = questionRow + "" + "" + "<div class=\"questionChoiceWrong\">" + Choice + "</div>";
                                            else
                                                // 
                                                // unselected answer is not correct answer
                                                // 
                                                questionRow = questionRow + "" + "" + "<div class=\"questionChoice\">" + Choice + "</div>";
                                        }
                                    }
                                }
                                // 
                                // Add QuestionText to top
                                // 
                                hint = 50;
                                if (Correct) {
                                    questionRow = ""
                                        + "" + "" + "<div class=\"questionText\">" + question.copy + "</div>"
                                        + questionRow;
                                    totalQuestionsCorrect = totalQuestionsCorrect + 1;
                                } else {
                                    questionRow = ""
                              + "" + "" + "<div class=\"questionTextWrong\">" + question.copy + "</div>"
                              + questionRow;
                                    Passed = false;
                                    totalQuestionsIncorrect = totalQuestionsIncorrect + 1;
                                }
                                // 
                                // Add Explination
                                // 
                                questionRow += "" + "" + "<div class=\"instructionText\">" + question.instructions + "</div>";
                                // hint = 50
                                // If Correct Then
                                // questionRow = "" _
                                // & vbCrLf & vbTab & "<div class=""instructionText"">" & question.instructions & "</div>" _
                                // & questionRow
                                // totalQuestionsCorrect = totalQuestionsCorrect + 1
                                // If (subjectCnt > subjectPtr) Then
                                // If question.SubjectID > 0 Then
                                // subjects(subjectPtr).CorrectAnswers = subjects(subjectPtr).CorrectAnswers + 1
                                // End If
                                // End If
                                // Else
                                // questionRow = "" _
                                // & vbCrLf & vbTab & "<div class=""instructionText"">" & question.instructions & "</div>" _
                                // & questionRow
                                // Passed = False
                                // totalQuestionsIncorrect = totalQuestionsIncorrect + 1
                                // End If
                                // 
                                // Question container
                                // 
                                SummaryBox += ""
                                + "" + "" + "<div class=\"question\">"
                                + cp.Html.Indent(questionRow)
                                + "" + "" + "</div>";
                            }
                            response.totalCorrect = totalQuestionsCorrect;
                            response.totalPoints = totalPoints;
                            response.totalQuestions = responseDetailList.Count;
                            response.score = 0;
                            if ((response.totalQuestions > 0))
                                response.score = (System.Convert.ToDouble(response.totalCorrect) * 100) / System.Convert.ToDouble(response.totalQuestions);
                            response.save(cp);
                            // 
                            hint = 60;
                            if (totalQuestionsIncorrect == 0)
                                // 
                                Passed = true;
                            else
                                // 
                                SummaryBox = ""
                                + "" + "" + "<div class=\"errorMsg\">Questions with incorrect answers are highlighted in red.</div>"
                                + SummaryBox;
                            // 
                            // add hiddens
                            // 
                            SummaryBox = ""
                            + SummaryBox
                            + "" + "" + "<input type=\"hidden\" name=\"quizID\" value=\"" + System.Convert.ToString(response.quizID) + "\">"
                            + "" + "" + "<input type=\"hidden\" name=\"scoreCard\" value=\"1\">"
                            + "" + "" + "<input type=\"hidden\" name=\"qNumbers\" value=\"" + System.Convert.ToString(totalQuestions) + "\">";
                            // 
                            // add summary wrapper
                            // 
                            SummaryBox = ""
                            + "" + "" + "<div class=\"summary\">"
                            + cp.Html.Indent(SummaryBox)
                            + "" + "" + "</div>";
                            // 
                            // Build ScoreBox
                            // 
                            hint = 70;
                            ScoreBox = "";
                            // TotalCorrect = 0
                            // TotalQuestions = 0
                            //if (false) {
                            //    // 
                            //    // -- no longer score by subject
                            //    if (subjectCnt > 0) {
                            //        SubjectCaptions = new string[subjectCnt - 1 + 1];
                            //        subjectScores = new int[subjectCnt - 1 + 1];
                            //        // 
                            //        // if more than just 'no subject', Iterate through all the subjects, displaying the score for each
                            //        // 
                            //        for (subjectPtr = 0; subjectPtr <= subjectCnt - 1; subjectPtr++) {
                            //            {
                            //                var withBlock = subjects[subjectPtr];
                            //                if (withBlock.TotalQuestions > 0) {
                            //                    // 
                            //                    // Determine Score
                            //                    // 
                            //                    // TotalQuestions = TotalQuestions + .TotalQuestions
                            //                    // TotalCorrect = TotalCorrect + .CorrectAnswers
                            //                    withBlock.Score = System.Convert.ToDouble(withBlock.CorrectAnswers) / System.Convert.ToDouble(withBlock.TotalQuestions);
                            //                    // 
                            //                    // Determine grade and get Caption
                            //                    // 
                            //                    getSubjectGradeCaption(cp, withBlock.SubjectID, withBlock.Score, ref withBlock.SubjectCaption, ref GradeCaption);
                            //                    // 
                            //                    // Save Chart data
                            //                    // 
                            //                    SubjectCaptions[subjectPtr] = withBlock.SubjectCaption + @"\n" + GradeCaption;
                            //                    subjectScores[subjectPtr] = withBlock.Score * 100;
                            //                    // 
                            //                    // Output line
                            //                    // 
                            //                    ScoreBox = ScoreBox + "" + "" + "<li class=\"subjectScore\">Your grade for the " + withBlock.SubjectCaption + " section is <b>" + GradeCaption + "</b>, " + System.Convert.ToString(withBlock.CorrectAnswers) + " correct out of " + System.Convert.ToString(withBlock.TotalQuestions) + " questions is " + System.Convert.ToString(Int(withBlock.Score * 100)) + "%.</li>";
                            //                }
                            //            }
                            //        }
                            //    }
                            //    hint = 80;
                            //    if (ScoreBox != "")
                            //        ScoreBox = ""
                            //            + "" + "" + "<div class=\"subjectScoreCaption\">Your score in each subject is as follows:</div>"
                            //            + "" + "" + "<ul class=\"subjectScoreList\">"
                            //            + cp.Html.Indent(ScoreBox)
                            //            + "" + "" + "</ul>";
                            //}
                            // 
                            Percentage = 0;
                            if ((response.totalQuestions > 0)) {
                                bool PassingGrade = false;
                                Percentage = System.Convert.ToDouble(response.score) / System.Convert.ToDouble(100);
                                getQuizGradeCaption(cp, quiz, Percentage, ref GradeCaption, ref PassingGrade);
                                scoreCaption = "";
                                if (PassingGrade)
                                    scoreCaption = "Congratulations, you passed the quiz!";
                                else
                                    scoreCaption = "Your score did not pass the quiz.";
                                if (GradeCaption != "")
                                    scoreCaption += " Your grade for this quiz is <b>" + GradeCaption + "</b>, ";
                                else
                                    scoreCaption += " Your total score for this quiz is " + System.Convert.ToString(Conversion.Int(Percentage * 100)) + "%";
                                scoreCaption += " (" + System.Convert.ToString(response.totalCorrect) + " correct out of " + System.Convert.ToString(response.totalQuestions);
                                if (response.totalQuestions == 1)
                                    scoreCaption = scoreCaption + " question)";
                                else
                                    scoreCaption = scoreCaption + " questions)";
                                ScoreBox = "" + "" + "<div class=\"totalScore\">" + scoreCaption + "</div>" + ScoreBox;
                            }
                            ScoreBox = ""
                                + "" + "" + "<div class=\"score\">"
                                + cp.Html.Indent(ScoreBox)
                                + "" + "" + "</div>";
                            // 
                            // Save the response
                            // 
                            // Call verifyQuizResponse(cp, responseId, response.MemberID, response.QuizID)
                            // 
                            if (false) {
                                // 
                                // -- no longer score by subject
                                if (subjectCnt > 0) {
                                    for (subjectPtr = 0; subjectPtr <= subjectCnt - 1; subjectPtr++) {
                                        {
                                            var withBlock = subjects[subjectPtr];
                                            cs4.Insert("Quiz Response Scores");
                                            if (cs4.OK()) {
                                                responseName = cp.Content.GetRecordName("quiz responses", responseId);
                                                responseSubject = cp.Content.GetRecordName("quiz subjects", withBlock.SubjectID);
                                                // 
                                                cs4.SetField("name", responseName + ", subject:" + responseSubject);
                                                cs4.SetField("QuizResponseID", responseId);
                                                cs4.SetField("QuizSubjectID", withBlock.SubjectID);
                                                cs4.SetField("Score", withBlock.Score);
                                            }
                                            cs4.Close();
                                        }
                                    }
                                }
                            }
                            // 
                            headerCopy = "<div class=\"aodlHeaderContainer\">" + cp.Content.GetCopy(quiz.name + " Summary Header Copy") + "</div>";
                            footerCopy = "<div class=\"aodlFooterContainer\">" + cp.Content.GetCopy(quiz.name + " Summary Footer Copy") + "</div>";
                            // 
                            // Build the final form
                            // 
                            // hint = 100
                            // chart = ""
                            // If subjectCnt > 1 Then
                            // chart = GetChart(cp, subjectCnt, SubjectCaptions, subjectScores)
                            // End If
                            returnHtml = headerCopy + ScoreBox + SummaryBox + chart + footerCopy;
                        }
                    }
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex, "getScoreCard-hint=" + hint.ToString());
                }
                return returnHtml;
            }
            // 
            // 
            // 
            private string GetChart(CPBaseClass cp, int subjectCnt, string[] SubjectCaptions, int[] subjectScores) {
                string returnHtml = "";
                try {
                    // 
                    int Ptr;
                    // 
                    returnHtml += "" + "" + "<script type=\"text/javascript\" src=\"http://www.google.com/jsapi\"></script>";
                    returnHtml += "" + "" + "<script type=\"text/javascript\">";
                    returnHtml += "" + "" + "" + "google.load(\"visualization\", \"1\", {packages:[\"columnchart\"]});";
                    returnHtml += "" + "" + "" + "google.setOnLoadCallback(drawChart);";
                    returnHtml += "" + "" + "" + "function drawChart() {";
                    returnHtml += "" + "" + "" + "" + "var data = new google.visualization.DataTable();";
                    returnHtml += "" + "" + "" + "" + "data.addColumn('string', 'Subject');";
                    returnHtml += "" + "" + "" + "" + "data.addColumn('number', 'Score');";
                    returnHtml += "" + "" + "" + "" + "data.addRows(" + subjectCnt + ");";
                    for (Ptr = 0; Ptr <= subjectCnt - 1; Ptr++) {
                        returnHtml += "" + "" + "" + "" + "data.setValue(" + Ptr + ", 0, '" + SubjectCaptions[Ptr] + "');";
                        returnHtml += "" + "" + "" + "" + "data.setValue(" + Ptr + ", 1, " + subjectScores[Ptr] + ");";
                    }
                    returnHtml += "" + "" + "" + "" + "var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));";
                    returnHtml += "" + "" + "" + "" + "chart.draw(data, {legend:'none',showCategories:false, min:0, max:100, width: 400, height: 240, is3D: true, title: 'Quiz Results By Subject'});";
                    returnHtml += "" + "" + "" + "}";
                    returnHtml += "" + "" + "</script>";
                    returnHtml += "" + "" + "<div id=\"chart_div\"></div>";
                    // 
                    returnHtml = ""
                        + "" + "" + "<div class=\"quizChart\">"
                        + cp.Html.Indent(returnHtml)
                        + "" + "" + "<p>Click on the column to see your score</p>"
                        + "" + "" + "</div>";
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex, "verifyQuizResponse");
                }
                return returnHtml;
            }
            // 
            // 
            // 
            private void getSubjectGradeCaption(CPBaseClass cp, int SubjectID, double ScorePercentile, ref string Return_SubjectCaption, ref string Return_GradeCaption) {
                try {
                    CPCSBaseClass cs = cp.CSNew();
                    int Score;
                    // 
                    Return_GradeCaption = "";
                    Return_SubjectCaption = "";
                    cs.Open("Quiz Subjects", "id=" + SubjectID);
                    if (cs.OK()) {
                        Score = System.Convert.ToInt32(Conversion.Int(100 * ScorePercentile));
                        Return_SubjectCaption = cs.GetText("Name");
                        if (Score >= cs.GetInteger("apercentile"))
                            // 
                            // Got an A
                            // 
                            Return_GradeCaption = cs.GetText("acaption");
                        else if (Score >= cs.GetInteger("bpercentile"))
                            // 
                            // Got a B
                            // 
                            Return_GradeCaption = cs.GetText("bcaption");
                        else if (Score >= cs.GetInteger("cpercentile"))
                            // 
                            // Got a C
                            // 
                            Return_GradeCaption = cs.GetText("ccaption");
                        else if (Score >= cs.GetInteger("dpercentile"))
                            // 
                            // Got a D
                            // 
                            Return_GradeCaption = cs.GetText("dcaption");
                        else
                            // 
                            // Got an F
                            // 
                            Return_GradeCaption = cs.GetText("fcaption");
                    }
                    cs.Close();
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex, "getSubjectGradeCaption");
                }
            }
            // 
            private void getQuizGradeCaption(CPBaseClass cp, DistanceLearning.Models.QuizModel quiz, double ScorePercentile, ref string Return_GradeCaption, ref bool Return_PassingGrade) {
                try {
                    Return_GradeCaption = "";
                    int Score = System.Convert.ToInt32(Conversion.Int(100 * ScorePercentile));
                    if (Score >= quiz.APercentile) {
                        // 
                        // Got an A
                        Return_GradeCaption = quiz.ACaption;
                        Return_PassingGrade = quiz.APassingGrade;
                    } else if (Score >= quiz.BPercentile) {
                        // 
                        // Got a B
                        // 
                        Return_GradeCaption = quiz.BCaption;
                        Return_PassingGrade = quiz.BPassingGrade;
                    } else if (Score >= quiz.CPercentile) {
                        // 
                        // Got a C
                        // 
                        Return_GradeCaption = quiz.CCaption;
                        Return_PassingGrade = quiz.CPassingGrade;
                    } else if (Score >= quiz.DPercentile) {
                        // 
                        // Got a D
                        // 
                        Return_GradeCaption = quiz.DCaption;
                        Return_PassingGrade = quiz.DPassingGrade;
                    } else {
                        // 
                        // Got an F
                        // 
                        Return_GradeCaption = quiz.FCaption;
                        Return_PassingGrade = quiz.FPassingGrade;
                    }
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex, "getQuizGradeCaption");
                }
            }
            // 
            // 
            // 
            private int getResponseAnswerId(CPBaseClass cp, int responseId, int questionId) {
                try {
                    using (CPCSBaseClass cs = cp.CSNew()) {
                        cs.Open("quiz response details", "(responseid=" + responseId + ")and(questionid=" + questionId + ")");
                        if (cs.OK()) {
                            return cs.GetInteger("answerId");
                        }
                    }
                    return 0;
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex, "verifyQuizResponse");
                    return 0;
                }
            }
        }
    }
}