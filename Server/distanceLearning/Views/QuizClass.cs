
using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Contensive.BaseClasses;
using Contensive.Addons.DistanceLearning.Models;
using static Contensive.Addons.DistanceLearning.Constants;
using Contensive.Addons.DistanceLearning.Controllers;

namespace Contensive.Addons.DistanceLearning {
    // 
    public class QuizClass : AddonBaseClass {
        // 
        public const int studyPageFormId = 1;
        public const int onineQuizFormId = 2;
        public const int scoreCardFormId = 3;
        public struct subjectsStruc {
            public int SubjectID;
            public string SubjectCaption;
            public int TotalQuestions;
            public int CorrectAnswers;
            public double Score;
            public int points;
        }
        // 
        // ====================================================================================================
        // 
        public override object Execute(CPBaseClass cp) {
            string returnHtml = "";
            try {
                //
                // -- get settings
                string loadHint = "";
                string requestQuizGuid = cp.Doc.GetText("instanceId");
                QuizModel quiz = QuizModel.create(cp, requestQuizGuid);
                if ((quiz == null)) {
                    //
                    // -- attempt legacy quiz methods
                    int quizId = cp.Doc.GetInteger("quiz");
                    if ((quizId == 0)) {
                        loadHint += "Addon argument quizId (Quiz in features tab) was not found in addon instance. Use advanded edit on this page to set it. ";
                        string quizName = cp.Doc.GetText("Quiz Name");
                        if ((string.IsNullOrEmpty(quizName))) {
                            loadHint += "Legacy addon argument quizName (Quiz Name in features tab) was not found in addon instance. ";
                        } else {
                            quizId = cp.Content.GetRecordID("quizzes", quizName);
                            if (quizId == 0) {
                                loadHint += "Legacy addon argument quizName (Quiz Name in features tab) was found, but no quizzes were found with this name. ";
                            }
                        }
                    }
                    quiz = QuizModel.create(cp, quizId);
                }
                if ((quiz == null) && (!string.IsNullOrEmpty(requestQuizGuid))) {
                    //
                    // -- no quiz but valid guid, create a new quiz
                    quiz = QuizModel.add(cp);
                    quiz.ccguid = requestQuizGuid;
                    quiz.name = "Quiz " + quiz.id.ToString();
                    quiz.saveObject(cp);
                }
                string adminHint = "";
                if ((quiz == null)) {
                    // 
                    // -- quiz could not be selected or created - possibly legacy method without a selection
                    adminHint += "<p>The quiz you selected cannot be found. " + loadHint + "</p>";
                    returnHtml = "<p>This quiz is not currently available.</p>";
                } else if (quiz.requireAuthentication & (!cp.User.IsAuthenticated)) {
                    // 
                    // -- require authentication
                    returnHtml = ""
                        + "<p>Before beginning, you must log in.</p>"
                        + cp.Html.Indent(cp.Addon.Execute(Constants.guidLogin))
                        + "";
                } else {
                    // 
                    // -- get the response (like an application)
                    List<QuizResponseDetailModel> responseDetailsList = new List<QuizResponseDetailModel>();
                    QuizResponseModel response = QuizResponseModel.createLastForThisUser(cp, quiz.id, cp.User.Id);
                    if (response == null) {
                        // 
                        // -- this user has no response for this quiz yet. Save creates the data needed for display
                        response = new QuizResponseModel();
                        saveResponseDetails(cp, ref quiz, ref response);
                    }
                    responseDetailsList = QuizResponseDetailModel.getObjectListForQuizDisplay(cp, response.id);
                    List<string> userMessageList = new List<string>();
                    // 
                    if ((!string.IsNullOrEmpty(cp.Doc.GetText(Constants.rnButton)))) {
                        //
                        // -- button pressed, process input
                        if ((!DistanceLearning.Controllers.genericController.isDateEmpty(response.dateSubmitted)))
                            // 
                            // -- process the score card
                            processScoreCardForm(cp, quiz, ref response, ref userMessageList);
                        else if ((response.lastPageNumber == 0))
                            // 
                            // -- process study page form
                            processStudyForm(cp, quiz, ref response, ref userMessageList);
                        else
                            // 
                            // -- process the online quiz
                            processOnlineQuizForm(cp, quiz, response, responseDetailsList, ref userMessageList);
                    }
                    adminHint = getAdminHints(cp, quiz, response);
                    // 
                    if ((!DistanceLearning.Controllers.genericController.isDateEmpty(response.dateSubmitted)))
                        // 
                        // -- score card
                        returnHtml = getScoreCardform(cp, quiz, response, ref adminHint, ref userMessageList);
                    else if ((response.lastPageNumber == 0))
                        // 
                        // -- study page
                        returnHtml = getStudyPageForm(cp, quiz, response, ref adminHint, ref userMessageList);
                    else
                        // 
                        // -- online quiz
                        returnHtml = getOnlineQuizForm(cp, quiz, response, ref adminHint, ref userMessageList);
                }
                // 
                // Add wrapper
                // 
                returnHtml = "" + "" + "" + "<div class=\"onlineQuiz\">" + cp.Html.Indent(returnHtml) + cp.Html.Indent(adminHintWrapper(cp, adminHint)) + "" + "" + "</div>";
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "execute");
            }
            return returnHtml;
        }
        // 
        // ====================================================================================================
        /// <summary>
        ///         ''' Score Response
        ///         ''' </summary>
        ///         ''' <param name="cp"></param>
        ///         ''' <param name="responseId"></param>
        private void scoreResponse(CPBaseClass cp, ref int responseId) {
            try {
                // 
                string responseSubject;
                string responseName;
                string ScoreBox;
                int TotalCorrect;
                int TotalQuestions;
                int answerId;
                bool AnswerCorrect;
                CPCSBaseClass cs = cp.CSNew();
                CPCSBaseClass cs2 = cp.CSNew();
                CPCSBaseClass CS3 = cp.CSNew();
                CPCSBaseClass cs4 = cp.CSNew();
                int Counter;
                string q;
                int SelectedAnswerID;
                bool Correct;
                int NumIncorrect;
                int NumCorrect;
                bool Passed;
                int SubjectID;
                subjectsStruc[] subjects = null;
                int questionId;
                int subjectCnt = 0;
                int subjectPtr;
                string GradeCaption = "";
                string[] SubjectCaptions;
                int[] subjectScores;
                string quizName;
                int userId;
                int quizId = 0;
                int quizTypeId;
                int answerPoints;
                int quizPoints = 0;
                DateTime rightNow = DateTime.Now;
                // 
                // only the responseId is valid, lookup the quiz and userId
                // 
                cs.Open("quiz responses", "id=" + responseId);
                if (cs.OK()) {
                    quizId = cs.GetInteger("quizId");
                    userId = cs.GetInteger("memberId");
                }
                cs.Close();
                // 
                cs.Open("Quizzes", "id=" + quizId);
                if (cs.OK()) {
                    quizName = cs.GetText("name");
                    quizTypeId = cs.GetInteger("typeId");
                    cs2.Open("Quiz Questions", "QuizID=" + System.Convert.ToString(quizId), "sortOrder");
                    // 
                    // subjectPtr=0 is the 'no subject' subject
                    // 
                    subjectCnt = 1;
                    var oldSubjects = subjects;
                    subjects = new subjectsStruc[subjectCnt + 1];
                    if (oldSubjects != null)
                        Array.Copy(oldSubjects, subjects, Math.Min(subjectCnt + 1, oldSubjects.Length));
                    // 
                    Counter = 0;
                    NumIncorrect = 0;
                    NumCorrect = 0;
                    while (cs2.OK()) {
                        Counter = Counter + 1;
                        SubjectID = cs2.GetInteger("SubjectID");
                        questionId = cs2.GetInteger("ID");
                        SelectedAnswerID = getResponseSelectedAnswerId(cp, ref responseId, ref questionId);
                        // 
                        // Set subjectPtr to the Response Array for this question's subject
                        // 
                        subjectPtr = 0;
                        if (subjectCnt > 0) {
                            while ((subjectPtr < subjectCnt)) {
                                if ((subjects[subjectPtr].SubjectID == SubjectID))
                                    break;
                                subjectPtr = subjectPtr + 1;
                            }
                        }
                        if (subjectPtr == subjectCnt) {
                            var oldSubjectsx = subjects;
                            subjects = new subjectsStruc[subjectPtr + 1];
                            if (oldSubjectsx != null)
                                Array.Copy(oldSubjectsx, subjects, Math.Min(subjectPtr + 1, oldSubjectsx.Length));
                            subjects[subjectPtr].SubjectID = SubjectID;
                            subjectCnt = subjectCnt + 1;
                        }
                        subjects[subjectPtr].TotalQuestions = subjects[subjectPtr].TotalQuestions + 1;
                        // 
                        // determine if they got the question right
                        // 
                        q = "";
                        answerId = 0;
                        answerPoints = 0;
                        Correct = false;
                        AnswerCorrect = false;
                        CS3.Open("Quiz Answers", "QuestionID=" + questionId, "sortOrder");
                        if (!CS3.OK())
                            // 
                            // Question with no choices is correct
                            // 
                            Correct = true;
                        else
                            while (CS3.OK()) {
                                // 
                                // cycle through all the answers and build display
                                // 
                                answerId = CS3.GetInteger("ID");
                                AnswerCorrect = CS3.GetBoolean("Correct");
                                answerPoints = CS3.GetInteger("points");
                                if (answerId == SelectedAnswerID) {
                                    subjects[subjectPtr].points += answerPoints;
                                    quizPoints += answerPoints;
                                    if (AnswerCorrect)
                                        // 
                                        // selected answer is current answer
                                        // 
                                        Correct = true;
                                }
                                // 
                                CS3.GoNext();
                            }
                        // 
                        // Add QuestionText to top
                        // 
                        if (Correct) {
                            NumCorrect = NumCorrect + 1;
                            subjects[subjectPtr].CorrectAnswers = subjects[subjectPtr].CorrectAnswers + 1;
                        } else {
                            Passed = false;
                            NumIncorrect = NumIncorrect + 1;
                        }
                        cs2.GoNext();
                    }
                    if (NumIncorrect == 0)
                        Passed = true;
                    cs2.Close();
                }
                cs.Close();
                // 
                // Build ScoreBox
                // 
                ScoreBox = "";
                TotalCorrect = 0;
                TotalQuestions = 0;
                SubjectCaptions = new string[subjectCnt + 1];
                subjectScores = new int[subjectCnt + 1];
                // 
                // if more than just 'no subject', Iterate through all the subjects, displaying the score for each
                // 
                for (subjectPtr = 0; subjectPtr <= subjectCnt - 1; subjectPtr++) {
                    {
                        var withBlock = subjects[subjectPtr];
                        if (withBlock.TotalQuestions > 0) {
                            // 
                            // Determine Score
                            // 
                            TotalQuestions = TotalQuestions + withBlock.TotalQuestions;
                            TotalCorrect = TotalCorrect + withBlock.CorrectAnswers;
                            withBlock.Score = System.Convert.ToDouble(withBlock.CorrectAnswers) / System.Convert.ToDouble(withBlock.TotalQuestions);
                            // 
                            // Determine grade and get Caption
                            // 
                            getSubjectCaptions(cp, ref withBlock.SubjectID, ref withBlock.Score, ref withBlock.SubjectCaption, ref GradeCaption);
                            // 
                            // Save Chart data
                            // 
                            SubjectCaptions[subjectPtr] = withBlock.SubjectCaption + @"\n" + GradeCaption;
                            subjectScores[subjectPtr] = System.Convert.ToInt32(withBlock.Score * 100);
                        }
                    }
                }
                // 
                // save TotalQuestions and TotalCorrect
                // 
                cs4.Open("quiz responses", "id=" + responseId);
                if (cs4.OK()) {
                    cs4.SetField("totalQuestions", TotalQuestions.ToString());
                    cs4.SetField("TotalCorrect", TotalCorrect.ToString());
                    cs4.SetField("totalPoints", quizPoints.ToString());
                }
                cs4.Close();
                // 
                // Save the subject scores
                // 
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
                                cs4.SetField("QuizResponseID", responseId.ToString());
                                cs4.SetField("QuizSubjectID", withBlock.SubjectID.ToString());
                                cs4.SetField("Score", withBlock.Score.ToString());
                                cs4.SetField("points", withBlock.points.ToString());
                            }
                            cs4.Close();
                        }
                    }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "scoreResponse");
            }
        }
        // 
        // ====================================================================================================
        // 
        private string GetQuizTakenCopy(CPBaseClass cp) {
            string returnHtml = "";
            try {
                returnHtml = cp.Content.GetCopy("Quiz Already Taken", QuizAlreadyTakenDefault);
                if (returnHtml == "") {
                    cp.Content.Delete("Copy Content", "name='Quiz Already Taken'");
                    returnHtml = cp.Content.GetCopy("Quiz Already Taken", QuizAlreadyTakenDefault);
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "GetQuizTakenCopy");
            }
            return returnHtml;
        }
        // 
        // ====================================================================================================
        // 
        private string GetChart(CPBaseClass cp, ref int subjectCnt, ref string[] SubjectCaptions, ref int[] subjectScores) {
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
                returnHtml += "" + "" + "" + "" + "chart.draw(data, {legend:'none',showCategories:false, min:0, max:100, width: 400, height: 240, is3D: true, title: 'Quiz Results'});";
                returnHtml += "" + "" + "" + "}";
                returnHtml += "" + "" + "</script>";
                returnHtml += "" + "" + "<div id=\"chart_div\"></div>";
                // 
                returnHtml = "" + "" + "" + "<div class=\"quizChart\">" + cp.Html.Indent(returnHtml) + "" + "" + "<p>Click on the column to see your score</p>" + "" + "" + "</div>";
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "GetChart");
            }
            return returnHtml;
        }
        // 
        // ====================================================================================================
        // 
        private void getSubjectCaptions(CPBaseClass cp, ref int SubjectID, ref double ScorePercentile, ref string Return_SubjectCaption, ref string Return_GradeCaption) {
            try {
                // 
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
                cp.Site.ErrorReport(ex, "SetSubjectArgs");
            }
        }
        // 
        // ====================================================================================================
        // 
        private void saveResponseDetails(CPBaseClass cp, ref DistanceLearning.Models.QuizModel quiz, ref DistanceLearning.Models.QuizResponseModel response) {
            try {
                List<DistanceLearning.Models.QuizQuestionModel> questionList = DistanceLearning.Models.QuizQuestionModel.getQuestionsForQuizList(cp, quiz.id);
                foreach (DistanceLearning.Models.QuizQuestionModel question in questionList) {
                    int SelectedAnswerID = cp.Doc.GetInteger(getRadioAnswerRequestName(question.id));
                    if (SelectedAnswerID != 0) {
                        DistanceLearning.Models.QuizResponseDetailModel responseDetails = DistanceLearning.Models.QuizResponseDetailModel.create(cp, response.id, question.id);
                        if ((responseDetails == null)) {
                            responseDetails = DistanceLearning.Models.QuizResponseDetailModel.add(cp);
                            responseDetails.name = response.name + ", question: " + question.name;
                            responseDetails.responseId = response.id;
                            responseDetails.questionId = question.id;
                        }
                        responseDetails.answerId = SelectedAnswerID;
                        responseDetails.saveObject(cp);
                    }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "saveResponseDetails");
            }
        }
        // 
        // ====================================================================================================
        // 
        private int getResponseSelectedAnswerId(CPBaseClass cp, ref int responseId, ref int questionId) {
            int returnShort = 0;
            try {
                CPCSBaseClass cs = cp.CSNew();
                // 
                cs.Open("quiz response details", "(responseid=" + responseId + ")and(questionid=" + questionId + ")");
                if (cs.OK())
                    returnShort = cs.GetInteger("answerId");
                cs.Close();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "getResponseAnswerId");
            }
            return returnShort;
        }
        // '
        // '====================================================================================================
        // '
        // Private Sub verifyQuizResponse(ByVal cp As CPBaseClass, ByRef responseId As Integer, ByVal userId As Integer, ByVal quizId As Integer)
        // Try
        // '
        // Dim cs As CPCSBaseClass = cp.CSNew()
        // Dim userName As String
        // Dim quizName As String
        // Dim attemptNumber As Integer
        // '
        // ' verify response record
        // '
        // Call cs.Open("quiz responses", "id=" & responseId)
        // If Not cs.OK() Then
        // Call cs.Close()
        // attemptNumber = 1
        // Call cs.OpenSQL("select count(*) as cnt from quizResponses where (memberid=" & userId & ")and(quizid=" & quizId & ")and(dateSubmitted is not null)")
        // If cs.OK() Then
        // attemptNumber = cs.GetInteger("cnt") + 1
        // End If
        // Call cs.Close()

        // userName = cp.Content.GetRecordName("people", userId)
        // quizName = cp.Content.GetRecordName("quizzes", quizId)
        // Call cs.Insert("quiz responses")
        // responseId = cs.GetInteger("id")
        // Call cs.SetField("name", userName & ", quiz:" & quizName)
        // Call cs.SetField("memberid", userId.ToString())
        // Call cs.SetField("quizid", quizId.ToString())
        // Call cs.SetField("attemptNumber", attemptNumber.ToString())
        // End If
        // Call cs.Close()
        // Catch ex As Exception
        // cp.Site.ErrorReport( ex, "verifyQuizResponse")
        // End Try
        // End Sub
        // 
        // ====================================================================================================
        // 
        private string adminHintWrapper(CPBaseClass cp, string hint) {
            string returnHtml = "";
            // 
            if (cp.User.IsEditingAnything | cp.User.IsAdmin)
                returnHtml = ""
                    + "<table border=0 width=\"100%\" cellspacing=0 cellpadding=0><tr><td class=\"ccHintWrapper\">"
                    + "<table border=0 width=\"100%\" cellspacing=0 cellpadding=0><tr><td class=\"ccHintWrapperContent\">"
                    + "<b>Administrator</b>"
                    + "<BR>"
                    + "<BR>" + hint
                    + "</td></tr></table>"
                    + "</td></tr></table>"
                    + "";
            return returnHtml;
        }
        // '
        // '====================================================================================================
        // ' pageOrder = getNextpageOrder( cp, quizId, pageOrder )
        // '
        // Private Function getNextPageOrder(ByVal cp As CPBaseClass, ByVal quizId As Integer, ByVal pageOrder As Integer, ByVal isStudyPage As Boolean) As Integer
        // Dim returnInt As Integer = pageOrder
        // Try
        // Dim sqlCriteria As String = "(quizId=" & quizId & ")and(pageOrder>" & pageOrder & ")"
        // Dim cs As CPCSBaseClass = cp.CSNew()
        // '
        // If isStudyPage Then
        // returnInt = getFirstPageOrder(cp, quizId)
        // Else
        // If cs.Open("quiz questions", sqlCriteria, "pageOrder,id", , "pageOrder") Then
        // returnInt = cs.GetInteger("pageOrder")
        // End If
        // Call cs.Close()
        // End If
        // Catch ex As Exception
        // Call cp.Site.ErrorReport( ex, "getNextpageOrder")
        // End Try
        // Return returnInt
        // End Function
        // '
        // '====================================================================================================
        // ' pageOrder = getFirstpageOrder( cp, quizId )
        // '
        // Private Function getFirstPageOrder(ByVal cp As CPBaseClass, ByVal quizId As Integer) As Integer
        // Dim returnInt As Integer = 0
        // Try
        // Dim sqlCriteria As String = "(quizId=" & quizId & ")"
        // Dim cs As CPCSBaseClass = cp.CSNew()
        // If cs.Open("quiz questions", sqlCriteria, "pageOrder,id", , "pageOrder") Then
        // returnInt = cs.GetInteger("pageOrder")
        // End If
        // Call cs.Close()
        // Catch ex As Exception
        // Call cp.Site.ErrorReport( ex, "getFirstPageOrder")
        // End Try
        // Return returnInt
        // End Function
        // '
        // '====================================================================================================
        // ' pageOrder = getPreviouspageOrder( cp, quizId, pageOrder )
        // '
        // Private Function getPreviousPageOrder(ByVal cp As CPBaseClass, ByVal quizId As Integer, ByVal pageOrder As Integer, ByRef isStudyPage As Boolean) As Integer
        // Dim returnInt As Integer = pageOrder
        // Try
        // Dim sqlCriteria As String = "(quizId=" & quizId & ")"
        // Dim cs As CPCSBaseClass = cp.CSNew()
        // '
        // If Not isStudyPage Then
        // sqlCriteria &= "and((pageorder is null)or(pageOrder<" & pageOrder & "))"
        // End If
        // If cs.Open("quiz questions", sqlCriteria, "pageOrder desc", , "pageOrder") Then
        // returnInt = cs.GetInteger("pageOrder")
        // End If
        // Call cs.Close()
        // Catch ex As Exception
        // Call cp.Site.ErrorReport( ex, "getPreviousPageOrder")
        // End Try
        // Return returnInt
        // End Function
        // 
        // ====================================================================================================
        // 
        private void processStudyForm(CPBaseClass cp, DistanceLearning.Models.QuizModel quiz, ref Addons.DistanceLearning.Models.QuizResponseModel response, ref List<string> userMessages) {
            try {
                string button = cp.Doc.GetText(rnButton);
                if ((!string.IsNullOrEmpty(button))) {
                    if ((response.id == 0))
                        response = genericController.createNewQuizResponse(cp, quiz);
                    if ((DistanceLearning.Controllers.genericController.isDateEmpty(response.dateStarted)))
                        response.dateStarted = DateTime.Now;
                    response.MemberID = cp.User.Id;
                    response.lastPageNumber = 1;
                    response.saveObject(cp);
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
        }
        // 
        // 
        // 
        private int processOnlineQuizForm(CPBaseClass cp, DistanceLearning.Models.QuizModel quiz, Addons.DistanceLearning.Models.QuizResponseModel response, List<DistanceLearning.Models.QuizResponseDetailModel> responseDetailsList, ref List<string> userMessages) {
            int result = 0;
            try {
                switch (cp.Doc.GetText(rnButton)) {
                    case buttonPrevious: {
                            // 
                            // previous - if past start, try studypage
                            // 
                            saveResponseDetails(cp, ref quiz, ref response);
                            int firstPageNumber = 1;
                            if (quiz.includeStudyPage)
                                firstPageNumber = 0;
                            if ((response.lastPageNumber > firstPageNumber)) {
                                response.lastPageNumber -= 1;
                                response.saveObject(cp);
                            }

                            break;
                        }

                    case buttonSubmit: {
                            // 
                            // submit
                            // 
                            saveResponseDetails(cp, ref quiz, ref response);
                            scoreResponse(cp, ref response.id);
                            response.lastPageNumber = 0;
                            response.dateSubmitted = DateTime.Now;
                            response.saveObject(cp);
                            break;
                        }

                    case buttonContinue: {
                            // 
                            // continue
                            // 
                            saveResponseDetails(cp, ref quiz, ref response);
                            if ((response.lastPageNumber < responseDetailsList[responseDetailsList.Count - 1].pageNumber))
                                response.lastPageNumber += 1;
                            response.saveObject(cp);
                            break;
                        }

                    case buttonSave: {
                            // 
                            // save the response
                            // 
                            saveResponseDetails(cp, ref quiz, ref response);
                            userMessages.Add("<p>Your quiz has been saved.</p>");
                            break;
                        }
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
        // 
        // ====================================================================================================
        // 
        private int processScoreCardForm(CPBaseClass cp, DistanceLearning.Models.QuizModel quiz, ref DistanceLearning.Models.QuizResponseModel response, ref List<string> userMessages) {
            int result = 0;
            try {
                if (cp.Doc.GetText(rnButton) == buttonRetakeQuiz)
                    // 
                    // start a retake - create a response and set dstPageOrder, isStudyPage
                    // 
                    response = genericController.createNewQuizResponse(cp, quiz);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
        // 
        // ====================================================================================================
        // 
        private string getOnlineQuizForm(CPBaseClass cp, DistanceLearning.Models.QuizModel quiz, DistanceLearning.Models.QuizResponseModel response, ref string adminHint, ref List<string> userMessages) {
            string returnHtml = "";
            int hint = 0;
            try {
                // 
                cp.Utils.AppendLog("onlineQuic, getOnlineQuizForm, response.id [" + response.id + "]");
                // 
                string buttonList = "";
                string htmlRadio;
                int answerCnt = 0;
                string q;
                string quizEditIcon = "";
                string topCopy = "";
                string buttonCopy = "";
                string qs;
                string formAction;
                double quizProgress;
                string jsHead;
                string progressBarHtml = "";
                // Dim questionSubjectId As Integer
                // Dim subjectName As String = ""
                string quizProgressText = "";
                if ((DistanceLearning.Controllers.genericController.isDateEmpty(response.dateStarted))) {
                    response.dateStarted = DateTime.Now;
                    response.saveObject(cp);
                }
                hint = 10;
                // 
                // -- progress bar for all but first page
                List<DistanceLearning.Models.QuizResponseDetailModel> responseDetailList = DistanceLearning.Models.QuizResponseDetailModel.getObjectListForQuizDisplay(cp, response.id);
                // 
                cp.Utils.AppendLog("onlineQuic, getOnlineQuizForm, responseDetailList.count [" + responseDetailList.Count + "]");
                // 
                quizProgress = 0;
                if (responseDetailList.Count > 0) {
                    int answeredCount = 0;
                    foreach (DistanceLearning.Models.QuizResponseDetailModel responseDetail in responseDetailList) {
                        if (responseDetail.answerId > 0)
                            answeredCount += 1;
                    }
                    quizProgress = answeredCount / (double)responseDetailList.Count;
                    quizProgressText = System.Convert.ToString(Conversion.Int(quizProgress * 100));
                    progressBarHtml = ""
                        + cr + "<div class=\"progressbarCon\">"
                        + cr + "<div class=\"progressbarTitle\">Your Progress " + quizProgressText + "%</div>"
                        + cr + "<div id=\"progressbar\"></div>"
                        + cr + "</div>";
                    jsHead = "$(document).ready(function(){$(\"#progressbar\").progressbar({value:" + quizProgressText + "});});";
                    cp.Doc.AddHeadJavascript(jsHead);
                }
                hint = 10;
                // 
                if (cp.User.IsEditingAnything) {
                }
                hint = 30;
                // 
                if (quiz.allowCustomButtonCopy)
                    buttonCopy = quiz.customButtonCopy;
                else if (cp.User.IsAuthenticated)
                    buttonCopy = defaultButtonWithSaveCopy;
                else
                    buttonCopy = defaultButtonCopy;
                hint = 40;
                // 
                int questionCnt = 0;
                int lastSubjectId = -1;
                foreach (DistanceLearning.Models.QuizResponseDetailModel responseDetail in responseDetailList) {
                    if (responseDetail.pageNumber == response.lastPageNumber) {
                        DistanceLearning.Models.QuizQuestionModel question = DistanceLearning.Models.QuizQuestionModel.create(cp, responseDetail.questionId);
                        DistanceLearning.Models.QuizSubjectModel subject = DistanceLearning.Models.QuizSubjectModel.create(cp, question.SubjectID);
                        if (subject == null)
                            subject = new DistanceLearning.Models.QuizSubjectModel();
                        answerCnt = 0;
                        answerCnt += 1;
                        q = "";
                        hint = 50;
                        // 
                        // -- add subject header if required
                        if ((subject.id > 0) & (subject.id != lastSubjectId) & (!string.IsNullOrEmpty(subject.name))) {
                            lastSubjectId = subject.id;
                            returnHtml += "" + "" + "<h2 class=\"subject\">" + cp.Html.Indent(subject.name) + "" + "" + "</h2>";
                            // 
                            if (cp.User.IsEditingAnything) {
                            }
                        }
                        hint = 60;
                        // 
                        // 
                        // -- Add Question
                        if (cp.User.IsEditingAnything) {
                        }
                        q = q + "" + "" + "<div class=\"questionText\">" + quizEditIcon + question.copy + "</div>";
                        hint = 70;
                        // 
                        // Add Choices
                        // 
                        List<DistanceLearning.Models.QuizAnswerModel> answerList = DistanceLearning.Models.QuizAnswerModel.getAnswersForQuestionList(cp, question.id);
                        if ((answerList == null))
                            // 
                            // -- this question has no answers
                            adminHint += "<p>Your Quiz Question \"" + question.name + "\" does not appear to have any answers configured. To add answers, turn on Edit and click the Add icon under the question.</p>";
                        else
                            foreach (DistanceLearning.Models.QuizAnswerModel answer in answerList) {
                                string answerCopy = answer.copy;
                                quizEditIcon = "";
                                if (cp.User.IsEditingAnything) {
                                }
                                htmlRadio = "<input type=\"radio\" class=\"questionRaioInput\" name=\"" + getRadioAnswerRequestName(question.id) + "\" value=\"" + answer.id + "\"";
                                if ((answer.id == responseDetail.answerId))
                                    htmlRadio = htmlRadio + " checked=\"checked\">";
                                else
                                    htmlRadio = htmlRadio + ">";
                                q = q + "" + "" + "<div class=\"questionChoice\">" + htmlRadio + "" + quizEditIcon + "<div class=\"quizanswerClass\">" + answerCopy + "</div></div>";
                                answerCnt = answerCnt + 1;
                            }
                        hint = 80;
                        if (cp.User.IsEditingAnything) {
                        }
                        // 
                        returnHtml += "" + "" + "<div class=\"question\">" + cp.Html.Indent(q) + "" + "" + "</div>";
                        questionCnt = questionCnt + 1;
                    }
                }
                if (questionCnt == 0)
                    adminHint += "<p>No Quiz Questions can be found for this quiz.</p>";
                hint = 90;

                if (cp.User.IsEditingAnything) {
                }
                // 
                // -- Add hiddens and button
                bool isFirstPage = (response.lastPageNumber < 2);
                bool isLastPage = true;
                if ((responseDetailList.Count > 0))
                    isLastPage = (response.lastPageNumber >= responseDetailList[responseDetailList.Count - 1].pageNumber);
                buttonList = "";
                if (((!isFirstPage) | quiz.includeStudyPage))
                    // 
                    // -- previous
                    buttonList += "" + "" + cp.Html.Button(rnButton, buttonPrevious, "quizButtonPrevious btn btn-primary", "quizButtonPrevious").Replace(">", " onClick=\"return verifyAnswers();\">");
                if (cp.User.IsAuthenticated)
                    // 
                    // -- authenticated, allow save
                    buttonList += "" + "" + cp.Html.Button(rnButton, buttonSave, "quizButtonSave btn btn-primary", "quizButtonSave").Replace(">", " onClick=\"return verifyAnswers();\">");
                hint = 100;
                // If (response.currentPageNumber > 1) Or (response.currentPageNumber = responseDetailList(responseDetailList.Count - 1).pageNumber) Then
                // End If
                if (!isLastPage)
                    // 
                    // -- continue, not last page
                    buttonList += "" + "" + cp.Html.Button(rnButton, buttonContinue, "quizButtonContinue btn btn-primary", "quizButtonContinue").Replace(">", " onClick=\"return verifyAnswers();\">");
                else
                    // 
                    // -- submit, last page
                    buttonList += "" + "" + cp.Html.Button(rnButton, buttonSubmit, "quizButtonSubmit btn btn-primary", "quizButtonSubmit").Replace(">", " onClick=\"return verifyAnswers();\">");
                returnHtml = ""
                        + cp.Html.Indent(returnHtml)
                        + cp.Html.Indent(buttonCopy)
                        + "" + "" + "<div class=\"button\">"
                        + cp.Html.Indent(buttonList)
                        + "" + "" + "</div>"
                        + progressBarHtml;
                hint = 110;
                // 
                // Add form wrapper
                // 
                qs = cp.Doc.RefreshQueryString;
                formAction = "?" + qs;
                foreach (string msg in userMessages)
                    returnHtml += cp.Html.div(msg);
                hint = 120;
                returnHtml = ""
                    + cp.Html.Indent(topCopy)
                    + "" + "" + "<form method=\"post\" name=\"quizForm\" action=\"" + formAction + "\">"
                    + cp.Html.Indent(returnHtml)
                    + "" + "" + "<input type=\"hidden\" name=\"quizID\" value=\"" + quiz.id + "\">"
                    + "" + "" + "<input type=\"hidden\" name=\"" + rnPageNumber + "\" value=\"" + response.lastPageNumber + "\">"
                    + "" + "" + "<input type=\"hidden\" name=\"qNumbers\" value=\"" + System.Convert.ToString(answerCnt) + "\">"
                    + "" + "" + "<input type=\"hidden\" name=\"quizName\" value=\"" + quiz.name + "\">"
                    + "" + "" + "<input type=\"hidden\" name=\"responseId\" value=\"" + response.id + "\">"
                    + "" + "" + "</form>";
                hint = 999;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "hint [" + hint + "]");
            }
            return returnHtml;
        }
        // 
        // ====================================================================================================
        // 
        private string getScoreCardform(CPBaseClass cp, DistanceLearning.Models.QuizModel quiz, DistanceLearning.Models.QuizResponseModel response, ref string adminHint, ref List<string> userMessages) {
            string returnHtml = "";
            try {
                string buttonList = "";
                int answerCnt = 0;
                string quizEditIcon = "";
                string topCopy = "";
                string buttonCopy = "";
                // Dim qs As String
                string formAction = "?" + cp.Doc.RefreshQueryString;
                string answerText = "";
                string progressBarHtml = "";
                string subjectName = "";
                // 
                // show result page
                if (quiz.typeId == quizTypeIdPoints) {
                    // 
                    // point-based quiz, show result message
                    // 
                    DistanceLearning.Models.QuizResultMessageModel resultMessage = DistanceLearning.Models.QuizResultMessageModel.createByPointThreshold(cp, response.totalPoints);
                    if ((resultMessage == null))
                        returnHtml = "<p>Thank you. The quiz is complete.</p>";
                    else
                        returnHtml = resultMessage.copy;
                } else {
                    // 
                    // graded quiz, show scorecard
                    // 
                    cp.Doc.SetProperty("id", response.id.ToString());
                    returnHtml = cp.Utils.ExecuteAddon(scoreCardAddon);
                }
                if (quiz.allowRetake) {
                    buttonCopy = cr + "<p>You may retake this quiz. To begin, click Retake.</p>";
                    buttonList = cr + cp.Html.Button(rnButton, buttonRetakeQuiz, " btn btn-primary");
                    returnHtml = ""
                            + cp.Html.Indent(returnHtml)
                            + cp.Html.Indent(buttonCopy)
                            + "" + "" + "<div class=\"button\">"
                            + cp.Html.Indent(buttonList)
                            + "" + "" + "</div>";
                }
                // 
                // Add form wrapper
                // 
                foreach (string msg in userMessages)
                    returnHtml += cp.Html.div(msg);
                returnHtml = ""
                    + cp.Html.Indent(topCopy)
                    + "" + "" + "<form method=\"post\" name=\"quizForm\" action=\"" + formAction + "\">"
                    + cp.Html.Indent(returnHtml)
                    + "" + "" + "<input type=\"hidden\" name=\"quizID\" value=\"" + quiz.id + "\">"
                    + "" + "" + "<input type=\"hidden\" name=\"" + rnPageNumber + "\" value=\"" + response.lastPageNumber + "\">"
                    + "" + "" + "<input type=\"hidden\" name=\"qNumbers\" value=\"" + System.Convert.ToString(answerCnt) + "\">"
                    + "" + "" + "<input type=\"hidden\" name=\"quizName\" value=\"" + quiz.name + "\">"
                    + "" + "" + "<input type=\"hidden\" name=\"responseId\" value=\"" + response.id + "\">"
                    + "" + "" + "</form>";
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return returnHtml;
        }
        // 
        // ====================================================================================================
        /// <summary>
        ///         ''' refactor -- use the view class that imports a layout
        ///         ''' </summary>
        ///         ''' <param name="cp"></param>
        ///         ''' <param name="quiz"></param>
        ///         ''' <param name="response"></param>
        ///         ''' <param name="adminHint"></param>
        ///         ''' <param name="userMessages"></param>
        ///         ''' <returns></returns>
        private string getStudyPageForm(CPBaseClass cp, DistanceLearning.Models.QuizModel quiz, DistanceLearning.Models.QuizResponseModel response, ref string adminHint, ref List<string> userMessages) {
            string returnHtml = "getStudyPageForm";
            try {
                CPBlockBaseClass layout = cp.BlockNew();
                // 
                layout.OpenLayout("Quiz Landing Page");
                layout.SetInner("#js-quizTitle", quiz.name);
                layout.SetInner("#js-quizStudyCopy", quiz.studyCopy);
                // todo -- needs to be removed from template
                layout.SetOuter("#js-quizStCustomText", "");

                // 
                if ((string.IsNullOrEmpty(quiz.courseMaterial.filename)))
                    layout.SetInner("#js-quizCourseMaterial", "");
                else
                    layout.SetOuter("#js-quizCourseMaterial", "<br><div id=\"js-quizCourseMaterial\">" + cp.Content.GetCopy("courseMaterial") + " <a href=\"" + cp.Site.FilePath + quiz.courseMaterial.pathFilename + "\"  target=\"_blank\">Click here</a></div><br>");
                // 
                if ((string.IsNullOrEmpty(quiz.videoEmbedCode)))
                    layout.SetOuter("#js-quizVideo", "");
                else
                    layout.SetInner("#js-quizVideo", quiz.videoEmbedCode);
                if ((DistanceLearning.Controllers.genericController.isDateEmpty(response.dateStarted)))
                    // 
                    // -- start Quiz
                    layout.SetOuter("#js-quizStartButton", cp.Html.Form(cp.Html.Button(rnButton, buttonStartQuiz, " btn btn-primary"), "startbuttonform"));
                else
                    // 
                    // -- resume Quiz
                    layout.SetOuter("#js-quizStartButton", cp.Html.Form(cp.Html.Button(rnButton, buttonResumeQuiz, " btn btn-primary"), "startbuttonform"));
                returnHtml = layout.GetHtml();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return returnHtml;
        }
        // 
        // ====================================================================================================
        // 
        private string getAdminHints(CPBaseClass cp, DistanceLearning.Models.QuizModel quiz, DistanceLearning.Models.QuizResponseModel response) {
            string result = "";
            try {
                int lastQuestionId = 0;
                bool foundCorrect;
                string lastQuestionName = "";
                string answerList;
                int answersCid = cp.Content.GetID("quiz answers");
                int questionsCid = cp.Content.GetID("quiz questions");
                int quizResultMessagesCid = cp.Content.GetID("quiz result messages");
                CPCSBaseClass cs = cp.CSNew();
                string itemList = "";
                string adminUrl = cp.Site.GetText("adminUrl");
                int warningMsgPoints = 0;
                string distanceLearningPortalLink;
                // 
                distanceLearningPortalLink = cp.Site.GetText("adminUrl")
                    + "?addonGuid=" + DistanceLearning.Constants.portalAddonGuid
                    + "&setPortalGuid=" + DistanceLearning.Constants.portalDistanceLearning
                    + "&dstFeatureGuid=";
                if (cp.User.IsAdmin) {
                    if ((quiz.typeId == quizTypeIdGraded)) {
                        // 
                        // graded quiz, check that all questions have at least one marked correct
                        // 
                        string Sql = "select q.name as questionName,q.id as questionId,a.name as answerName,a.id as answerId,a.correct"
                            + " from ((quizzes z"
                            + " left join quizQuestions q on q.quizId=z.id)"
                            + " left join quizAnswers a on a.questionid=q.id)"
                            + " where(z.id=" + quiz.id + ")"
                            + " order by q.id,a.id";
                        cs.OpenSQL(Sql);
                        if (cs.OK()) {
                            lastQuestionId = 0;
                            foundCorrect = true;
                            answerList = "";
                            while (cs.OK()) {
                                string questionName = cs.GetText("questionName");
                                int questionId = cs.GetInteger("questionId");
                                string answerName = cs.GetText("answerName");
                                int answerId = cs.GetInteger("answerId");
                                if ((lastQuestionId != questionId)) {
                                    // 
                                    // new question
                                    // 
                                    if (!foundCorrect)
                                        itemList += ""
+ cr + cp.Html.li(lastQuestionName)
+ cr + cp.Html.ul(answerList)
+ "";
                                    foundCorrect = false;
                                    answerList = "";
                                }
                                foundCorrect = foundCorrect | cs.GetBoolean("correct");
                                string link = distanceLearningPortalLink + DistanceLearning.Constants.portalFeatureQuizOverviewQuestionDetails
                                    + "&questionId=" + questionId
                                    + "&quizId=" + quiz.id;
                                answerList += cr + cp.Html.li("<a href=\"" + link + "\">" + answerName + "</a>");
                                lastQuestionName = questionName;
                                lastQuestionId = questionId;
                                cs.GoNext();
                            }
                            if (!foundCorrect)
                                itemList += ""
+ cr + cp.Html.li(lastQuestionName)
+ cr + cp.Html.ul(answerList)
+ "";
                        }
                        cs.Close();
                        if (itemList != "")
                            result += ""
+ cr + cp.Html.p("WARNING: This quiz is configured as a graded quiz and the following questions have no correct answer.")
+ cr + cp.Html.ul(itemList);
                    } else {
                        // 
                        // points-based quiz, check that all answers have a point value
                        // 
                        string Sql = "select q.copy as questionCopy, a.copy as answerCopy, a.name,a.id,q.id as questionId"
                            + " from ((quizzes z"
                            + " left join quizQuestions q on q.quizId=z.id)"
                            + " left join quizAnswers a on a.questionid=q.id)"
                            + " where(z.id=" + quiz.id + ")"
                            + " and (q.points is null)";
                        cs.OpenSQL(Sql);
                        while (cs.OK()) {
                            int answerId = cs.GetInteger("id");
                            if ((answerId != 0)) {
                                string answerName = "Answer: " + cs.GetText("answerCopy") + "<br>&nbsp;from question: " + cs.GetText("questionCopy");
                                if (answerName == "")
                                    answerName = "Answer #" + answerId;
                                string link3 = distanceLearningPortalLink + DistanceLearning.Constants.portalFeatureQuizOverviewQuestionDetails
                                    + "&answerId=" + answerId
                                    + "&questionId=" + cs.GetInteger("questionId")
                                    + "&quizId=" + quiz.id;
                                itemList += cr + cp.Html.li("<a href=\"" + link3 + "\">" + answerName + "</a>");
                            }
                            cs.GoNext();
                        }
                        cs.Close();
                        if (itemList != "")
                            result += ""
+ cr + cp.Html.p("WARNING: This quiz is configured as a points-based quiz and the following answers have no points assigned. They will count as 0 points.")
+ cr + cp.Html.ul(itemList);
                        // 
                        // check result messages
                        // 
                        itemList = "";
                        int pointThreshold = 0;
                        string itemListIssues = "";
                        int lastPointThreshold = 0;
                        string lastItemEdit = "";
                        if ((!cs.Open("quiz result messages", "(quizid=" + quiz.id + ")", "pointThreshold")))
                            result += cr + cp.Html.p("WARNING: This quiz has no Quiz Result Messages. The quiz will end with a simple thank you page.");
                        else {
                            int ptr = 0;
                            while (cs.OK()) {
                                int itemId = cs.GetInteger("id");
                                string itemName = cs.GetText("name");
                                if (itemName == "")
                                    itemName = "Message #" + itemId;
                                string itemEdit = "<a href=\"" + adminUrl + "?af=4&cid=" + quizResultMessagesCid + "&id=" + itemId + "\">" + itemName + "</a>";
                                string integerTest = cs.GetText("pointThreshold");
                                if ((integerTest == ""))
                                    itemListIssues += cr + cp.Html.p("WARNING: Quiz Result Message '" + itemName + "' has no Point Threshold. It will never be shown." + itemEdit);
                                else {
                                    pointThreshold = cp.Utils.EncodeInteger(integerTest);
                                    if ((ptr == 0)) {
                                        if (pointThreshold > 0)
                                            itemListIssues += cr + cp.Html.p("WARNING: There is no Quiz Result Message for point scores less than " + pointThreshold + ". The quiz will end with a simple thank you page.");
                                    } else {
                                        if ((pointThreshold == lastPointThreshold) & (pointThreshold != warningMsgPoints)) {
                                            warningMsgPoints = pointThreshold;
                                            itemListIssues += cr + cp.Html.p("WARNING: There are multiple Quiz Result Messages with a Point Threshold [" + pointThreshold + "]. Only the message with the lowest ID # will be displayed.");
                                        }
                                        itemList += "<li>Total Points from " + lastPointThreshold + " to " + (pointThreshold - 1) + " see Result Message " + lastItemEdit + "</li>";
                                    }
                                    lastPointThreshold = pointThreshold;
                                    lastItemEdit = itemEdit;
                                }
                                ptr += 1;
                                cs.GoNext();
                            }
                            itemList += "<li>Total Points over " + lastPointThreshold + " see Result Message " + lastItemEdit + "</li>";
                        }
                        string link = adminUrl + "?cid=" + quizResultMessagesCid + "&af=4&aa=2&wc=quizid%3D" + quiz.id;
                        itemList += cr + cp.Html.li("<a href=\"" + link + "\">Add a Quiz Result Message</a>");
                        result += ""
                                + cr + cp.Html.p("Result Messages.")
                                + cr + cp.Html.ul(itemList)
                                + itemListIssues;
                    }
                    if (result == "")
                        result += "<p>Your online quiz appears to be configured correctly.</p>";
                    string link2;
                    if ((quiz != null)) {
                        link2 = distanceLearningPortalLink + DistanceLearning.Constants.portalFeaturesQuizOverviewDetails + "&quizId=" + quiz.id;
                        result += "<p>Edit this quiz <a href=\"" + link2 + "\">" + cp.Utils.EncodeHTML(quiz.name) + "</a>.</p>";
                    }
                    link2 = distanceLearningPortalLink + DistanceLearning.Constants.portalFeatureDashboard;
                    result += "<p><a href=\"" + link2 + "\">Distance Learning Manager</a></p>";
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
            }
            return result;
        }
        // 
        private string getRadioAnswerRequestName(int questionId) {
            return "q" + questionId.ToString() + "a";
        }
        // 
        // ====================================================================================================
        // 
        private void errorReport(CPBaseClass cp, Exception ex, string method) {
            try {
                cp.Site.ErrorReport(ex, "Unexpected error in quizClass." + method);
            } catch (Exception exLost) {
            }
        }
    }
}