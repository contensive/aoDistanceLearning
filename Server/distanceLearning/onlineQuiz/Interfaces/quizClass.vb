
Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses

Namespace Contensive.Addons.OnlineQuiz
    '
    ' Sample Vb2005 addon
    '
    Public Class quizClass
        Inherits Contensive.BaseClasses.AddonBaseClass
        '
        Public Const studyPageFormId As Integer = 1
        Public Const onineQuizFormId As Integer = 2
        Public Const scoreCardFormId As Integer = 3
        '
        '====================================================================================================
        '
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                Dim adminHint As String = ""
                Dim userMessage As String = ""
                Dim sqlCriteria As String
                'Dim cs As CPCSBaseClass = CP.CSNew()
                Dim adminUrl As String
                Dim quizSelected As Boolean
                Dim isEditing As Boolean
                Dim rightNow As Date = Now()
                Dim isAuthenticated As Boolean
                Dim isStudyPage As Boolean
                Dim lastPageCompleted As Boolean = False
                Dim srcPageOrder As Integer = 0
                Dim srcPageOrderNext As Integer = 0
                Dim answerName As String = ""
                Dim lastPointThreshold As Integer = -1
                Dim lastItemEdit As String = ""
                Dim ptr As Integer = 0
                Dim warningMsgPoints As Integer = -9999
                Dim itemListIssues As String = ""
                Dim answersCid As Integer = CP.Content.GetID("quiz answers")
                Dim questionsCid As Integer = CP.Content.GetID("quiz questions")
                Dim quizResultMessagesCid As Integer = CP.Content.GetID("quiz result messages")
                Dim srcPageOrderText As String = CP.Doc.GetText(rnSrcPageOrder)
                Dim Action As String = CP.Doc.GetText("Action")
                Dim isScoreCardPage As Boolean = CP.Doc.GetBoolean("scoreCard")
                Dim dstPageOrder As Integer = CP.Doc.GetInteger(rnDstPageOrder)
                Dim userMessages As New List(Of String)
                '
                Dim quiz As DistanceLearning.Models.QuizModel = Nothing
                If True Then
                    Dim quizName As String
                    Dim quizId As Integer = CP.Doc.GetInteger("quizId")
                    If (quizId = 0) Then
                        quizName = CP.Doc.GetText("Quiz Name")
                        If (Not String.IsNullOrEmpty(quizName)) Then
                            quizId = CP.Content.GetRecordID("quizzes", quizName)
                        End If
                    End If
                    quiz = DistanceLearning.Models.QuizModel.create(CP, quizId)
                End If
                If (quiz Is Nothing) Then
                    '
                    '
                    adminHint &= "<p>The quiz you selected cannot be found. Use advanced edit to change the addon options and select a different quiz.</p>"
                    returnHtml = "<p>This quiz is not currently available.</p>"
                Else
                    srcPageOrder = CP.Utils.EncodeInteger(srcPageOrderText)
                    isStudyPage = (srcPageOrderText = "") And Not isScoreCardPage
                    isAuthenticated = CP.User.IsAuthenticated()
                    isEditing = CP.User.IsEditingAnything()
                    adminUrl = CP.Site.GetText("adminUrl")
                    Call CP.User.Track()
                    quizSelected = False
                    sqlCriteria = ""
                    returnHtml = ""
                    '
                    ' verify authentiation
                    '
                    If quiz.requireAuthentication And (Not CP.User.IsAuthenticated) Then
                        '
                        ' require authentication
                        '
                        returnHtml = "" _
                        & cr & "<p>Before beginning, you must log in.</p>" _
                        & CP.Html.Indent(CP.Utils.ExecuteAddon(guidLogin)) _
                        & ""
                    Else
                        Dim srcFormId As Integer = CP.Doc.GetInteger("srcFormId")
                        Dim dstFormId As Integer = CP.Doc.GetInteger("dstFormId")
                        '
                        ' -- get the response (like an application)
                        Dim response As DistanceLearning.Models.QuizResponseModel = Nothing
                        If True Then
                            Dim responseId As Integer = CP.Doc.GetInteger("responseId")
                            response = DistanceLearning.Models.QuizResponseModel.create(CP, responseId, quiz.id, CP.User.Id)
                            If (response Is Nothing) Then
                                If (srcFormId > studyPageFormId) Or (dstFormId > studyPageFormId) Then
                                    genericController.createNewQuizResponse(CP, quiz)
                                End If
                            End If
                        End If
                        If response Is Nothing Then
                            response = New DistanceLearning.Models.QuizResponseModel
                        End If
                        '
                        If (srcFormId > 0) Then
                            '
                            ' -- process the form results
                            Select Case srcFormId
                                Case studyPageFormId
                                    '
                                    ' -- process the landing page form
                                    dstFormId = onineQuizFormId
                                Case onineQuizFormId
                                    '
                                    ' -- process the online quiz
                                    dstFormId = processOnlineQuizForm(CP, quiz, response, userMessages)
                                Case scoreCardFormId
                                    '
                                    ' -- process the score card
                                    dstFormId = processScoreCardForm(CP, quiz, response, userMessages)
                            End Select
                        End If
                        '
                        If (response.dateSubmitted <= Date.MinValue) And (dstFormId = scoreCardFormId) Then
                            dstFormId = studyPageFormId
                        End If
                        If (response.id = 0) Then
                            dstFormId = studyPageFormId
                        End If
                        '
                        Select Case dstFormId
                            Case scoreCardFormId
                                '
                                '
                                returnHtml = getScoreCardform(CP, quiz, response)
                                'returnHtml = getQuiz(CP, quiz.name, quiz.id, adminHint, isEditing, CP.User.Id, response.id, userMessage, isAuthenticated, quiz.allowRetake, dstPageOrder, isStudyPage, (response.dateSubmitted > Date.MinValue))
                            Case onineQuizFormId
                                '
                                '
                                returnHtml = getOnlineQuizForm(CP, quiz, response)
                                'returnHtml = getQuiz(CP, quiz.name, quiz.id, adminHint, isEditing, CP.User.Id, response.id, userMessage, isAuthenticated, quiz.allowRetake, dstPageOrder, isStudyPage, (response.dateSubmitted > Date.MinValue))
                            Case Else
                                '
                                '
                                returnHtml = getScoreCardform(CP, quiz, response)
                                'returnHtml = getQuiz(CP, quiz.name, quiz.id, adminHint, isEditing, CP.User.Id, response.id, userMessage, isAuthenticated, quiz.allowRetake, dstPageOrder, isStudyPage, (response.dateSubmitted > Date.MinValue))
                        End Select
                        adminHint = getAdminHints(CP, quiz, response)
                    End If
                    '
                    ' Add wrapper
                    '
                    returnHtml = "" & vbCrLf & vbTab & "<div class=""onlineQuiz"">" & CP.Html.Indent(returnHtml) & CP.Html.Indent(adminHintWrapper(CP, adminHint)) & vbCrLf & vbTab & "</div>"
                End If
            Catch ex As Exception
                errorReport(CP, ex, "execute")
            End Try
            Return returnHtml
        End Function
        '
        '====================================================================================================
        ''' <summary>
        ''' Score Response
        ''' </summary>
        ''' <param name="cp"></param>
        ''' <param name="responseId"></param>
        Private Sub scoreResponse(ByVal cp As CPBaseClass, ByRef responseId As Integer)
            Try
                '
                Dim responseSubject As String
                Dim responseName As String
                Dim ScoreBox As String
                Dim TotalCorrect As Integer
                Dim TotalQuestions As Integer
                Dim answerId As Integer
                Dim AnswerCorrect As Boolean
                Dim cs As CPCSBaseClass = cp.CSNew()
                Dim cs2 As CPCSBaseClass = cp.CSNew()
                Dim CS3 As CPCSBaseClass = cp.CSNew()
                Dim cs4 As CPCSBaseClass = cp.CSNew()
                Dim Counter As Integer
                Dim q As String
                Dim SelectedAnswerID As Integer
                Dim Correct As Boolean
                Dim NumIncorrect As Integer
                Dim NumCorrect As Integer
                Dim Passed As Boolean
                Dim SubjectID As Integer
                Dim subjects() As subjectsStruc = Nothing
                Dim questionId As Integer
                Dim subjectCnt As Integer
                Dim subjectPtr As Integer
                Dim GradeCaption As String = ""
                Dim SubjectCaptions() As String
                Dim subjectScores() As Integer
                Dim quizName As String
                Dim userId As Integer
                Dim quizId As Integer
                Dim quizTypeId As Integer
                Dim answerPoints As Integer
                Dim quizPoints As Integer = 0
                Dim rightNow As Date = Now
                '
                ' only the responseId is valid, lookup the quiz and userId
                '
                Call cs.Open("quiz responses", "id=" & responseId)
                If cs.OK() Then
                    quizId = cs.GetInteger("quizId")
                    userId = cs.GetInteger("memberId")
                End If
                Call cs.Close()
                '
                Call cs.Open("Quizzes", "id=" & quizId)
                If cs.OK() Then
                    quizName = cs.GetText("name")
                    quizTypeId = cs.GetInteger("typeId")
                    cs2.Open("Quiz Questions", "QuizID=" & CStr(quizId), "QOrder")
                    '
                    ' subjectPtr=0 is the 'no subject' subject
                    '
                    subjectCnt = 1
                    ReDim Preserve subjects(subjectCnt)
                    '
                    Counter = 0
                    NumIncorrect = 0
                    NumCorrect = 0
                    Do While cs2.OK()
                        Counter = Counter + 1
                        SubjectID = cs2.GetInteger("SubjectID")
                        questionId = cs2.GetInteger("ID")
                        SelectedAnswerID = getResponseSelectedAnswerId(cp, responseId, questionId)
                        '
                        ' Set subjectPtr to the Response Array for this question's subject
                        '
                        subjectPtr = 0
                        If subjectCnt > 0 Then
                            Do While (subjectPtr < subjectCnt)
                                If (subjects(subjectPtr).SubjectID = SubjectID) Then
                                    Exit Do
                                End If
                                subjectPtr = subjectPtr + 1
                            Loop
                        End If
                        If subjectPtr = subjectCnt Then
                            ReDim Preserve subjects(subjectPtr)
                            subjects(subjectPtr).SubjectID = SubjectID
                            subjectCnt = subjectCnt + 1
                        End If
                        subjects(subjectPtr).TotalQuestions = subjects(subjectPtr).TotalQuestions + 1
                        '
                        ' determine if they got the question right
                        '
                        q = ""
                        answerId = 0
                        answerPoints = 0
                        Correct = False
                        AnswerCorrect = False
                        Call CS3.Open("Quiz Answers", "QuestionID=" & questionId, "QOrder")
                        If Not CS3.OK() Then
                            '
                            ' Question with no choices is correct
                            '
                            Correct = True
                        Else
                            Do While CS3.OK()
                                '
                                ' cycle through all the answers and build display
                                '
                                answerId = CS3.GetInteger("ID")
                                AnswerCorrect = CS3.GetBoolean("Correct")
                                answerPoints = CS3.GetInteger("points")
                                If answerId = SelectedAnswerID Then
                                    subjects(subjectPtr).points += answerPoints
                                    quizPoints += answerPoints
                                    If AnswerCorrect Then
                                        '
                                        ' selected answer is current answer
                                        '
                                        Correct = True
                                    End If
                                End If
                                '
                                Call CS3.GoNext()
                            Loop
                        End If
                        '
                        ' Add QuestionText to top
                        '
                        If Correct Then
                            NumCorrect = NumCorrect + 1
                            subjects(subjectPtr).CorrectAnswers = subjects(subjectPtr).CorrectAnswers + 1
                        Else
                            Passed = False
                            NumIncorrect = NumIncorrect + 1
                        End If
                        cs2.GoNext()
                    Loop
                    If NumIncorrect = 0 Then
                        Passed = True
                    End If
                    Call cs2.Close()
                End If
                Call cs.Close()
                '
                ' Build ScoreBox
                '
                ScoreBox = ""
                TotalCorrect = 0
                TotalQuestions = 0
                ReDim SubjectCaptions(subjectCnt)
                ReDim subjectScores(subjectCnt)
                '
                ' if more than just 'no subject', Iterate through all the subjects, displaying the score for each
                '
                For subjectPtr = 0 To subjectCnt - 1
                    With subjects(subjectPtr)
                        If .TotalQuestions > 0 Then
                            '
                            ' Determine Score
                            '
                            TotalQuestions = TotalQuestions + .TotalQuestions
                            TotalCorrect = TotalCorrect + .CorrectAnswers
                            .Score = CDbl(.CorrectAnswers) / CDbl(.TotalQuestions)
                            '
                            ' Determine grade and get Caption
                            '
                            Call getSubjectCaptions(cp, .SubjectID, .Score, .SubjectCaption, GradeCaption)
                            '
                            ' Save Chart data
                            '
                            SubjectCaptions(subjectPtr) = .SubjectCaption & "\n" & GradeCaption
                            subjectScores(subjectPtr) = CInt(.Score * 100)
                        End If
                    End With
                Next
                '
                ' save TotalQuestions and TotalCorrect
                '
                Call cs4.Open("quiz responses", "id=" & responseId)
                If cs4.OK() Then
                    Call cs4.SetField("totalQuestions", TotalQuestions.ToString())
                    Call cs4.SetField("TotalCorrect", TotalCorrect.ToString())
                    Call cs4.SetField("totalPoints", quizPoints.ToString())
                End If
                Call cs4.Close()
                '
                ' Save the subject scores
                '
                If subjectCnt > 0 Then
                    For subjectPtr = 0 To subjectCnt - 1
                        With subjects(subjectPtr)
                            Call cs4.Insert("Quiz Response Scores")
                            If cs4.OK() Then
                                responseName = cp.Content.GetRecordName("quiz responses", responseId)
                                responseSubject = cp.Content.GetRecordName("quiz subjects", .SubjectID)
                                '
                                Call cs4.SetField("name", responseName & ", subject:" & responseSubject)
                                Call cs4.SetField("QuizResponseID", responseId.ToString())
                                Call cs4.SetField("QuizSubjectID", .SubjectID.ToString())
                                Call cs4.SetField("Score", .Score.ToString())
                                Call cs4.SetField("points", .points.ToString())
                            End If
                            cs4.Close()
                        End With
                    Next
                End If
            Catch ex As Exception
                errorReport(cp, ex, "scoreResponse")
            End Try
        End Sub
        '
        '====================================================================================================
        '
        Private Function GetQuizTakenCopy(ByVal cp As CPBaseClass) As String
            Dim returnHtml As String = ""
            Try
                returnHtml = cp.Content.GetCopy("Quiz Already Taken", QuizAlreadyTakenDefault)
                If returnHtml = "" Then
                    Call cp.Content.Delete("Copy Content", "name='Quiz Already Taken'")
                    returnHtml = cp.Content.GetCopy("Quiz Already Taken", QuizAlreadyTakenDefault)
                End If
            Catch ex As Exception
                errorReport(cp, ex, "GetQuizTakenCopy")
            End Try
            Return returnHtml
        End Function
        '
        '====================================================================================================
        '
        Private Function GetChart(ByVal cp As CPBaseClass, ByRef subjectCnt As Integer, ByRef SubjectCaptions() As String, ByRef subjectScores() As Integer) As String
            Dim returnHtml As String = ""
            Try
                '
                Dim Ptr As Integer
                '
                returnHtml &= vbCrLf & vbTab & "<script type=""text/javascript"" src=""http://www.google.com/jsapi""></script>"
                returnHtml &= vbCrLf & vbTab & "<script type=""text/javascript"">"
                returnHtml &= vbCrLf & vbTab & vbTab & "google.load(""visualization"", ""1"", {packages:[""columnchart""]});"
                returnHtml &= vbCrLf & vbTab & vbTab & "google.setOnLoadCallback(drawChart);"
                returnHtml &= vbCrLf & vbTab & vbTab & "function drawChart() {"
                returnHtml &= vbCrLf & vbTab & vbTab & vbTab & "var data = new google.visualization.DataTable();"
                returnHtml &= vbCrLf & vbTab & vbTab & vbTab & "data.addColumn('string', 'Subject');"
                returnHtml &= vbCrLf & vbTab & vbTab & vbTab & "data.addColumn('number', 'Score');"
                returnHtml &= vbCrLf & vbTab & vbTab & vbTab & "data.addRows(" & subjectCnt & ");"
                For Ptr = 0 To subjectCnt - 1
                    returnHtml &= vbCrLf & vbTab & vbTab & vbTab & "data.setValue(" & Ptr & ", 0, '" & SubjectCaptions(Ptr) & "');"
                    returnHtml &= vbCrLf & vbTab & vbTab & vbTab & "data.setValue(" & Ptr & ", 1, " & subjectScores(Ptr) & ");"
                Next
                returnHtml &= vbCrLf & vbTab & vbTab & vbTab & "var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));"
                returnHtml &= vbCrLf & vbTab & vbTab & vbTab & "chart.draw(data, {legend:'none',showCategories:false, min:0, max:100, width: 400, height: 240, is3D: true, title: 'Quiz Results'});"
                returnHtml &= vbCrLf & vbTab & vbTab & "}"
                returnHtml &= vbCrLf & vbTab & "</script>"
                returnHtml &= vbCrLf & vbTab & "<div id=""chart_div""></div>"
                '
                GetChart = "" & vbCrLf & vbTab & "<div class=""quizChart"">" & cp.Html.Indent(returnHtml) & vbCrLf & vbTab & "<p>Click on the column to see your score</p>" & vbCrLf & vbTab & "</div>"
            Catch ex As Exception
                errorReport(cp, ex, "GetChart")
            End Try
            Return returnHtml
        End Function
        '
        '====================================================================================================
        '
        Private Sub getSubjectCaptions(ByVal cp As CPBaseClass, ByRef SubjectID As Integer, ByRef ScorePercentile As Double, ByRef Return_SubjectCaption As String, ByRef Return_GradeCaption As String)
            Try
                '
                Dim cs As CPCSBaseClass = cp.CSNew()
                Dim Score As Integer
                '
                Return_GradeCaption = ""
                Return_SubjectCaption = ""
                Call cs.Open("Quiz Subjects", "id=" & SubjectID)
                If cs.OK() Then
                    Score = CInt(Int(100 * ScorePercentile))
                    Return_SubjectCaption = cs.GetText("Name")
                    If Score >= cs.GetInteger("apercentile") Then
                        '
                        ' Got an A
                        '
                        Return_GradeCaption = cs.GetText("acaption")
                    ElseIf Score >= cs.GetInteger("bpercentile") Then
                        '
                        ' Got a B
                        '
                        Return_GradeCaption = cs.GetText("bcaption")
                    ElseIf Score >= cs.GetInteger("cpercentile") Then
                        '
                        ' Got a C
                        '
                        Return_GradeCaption = cs.GetText("ccaption")
                    ElseIf Score >= cs.GetInteger("dpercentile") Then
                        '
                        ' Got a D
                        '
                        Return_GradeCaption = cs.GetText("dcaption")
                    Else
                        '
                        ' Got an F
                        '
                        Return_GradeCaption = cs.GetText("fcaption")
                    End If
                End If
                Call cs.Close()
            Catch ex As Exception
                errorReport(cp, ex, "SetSubjectArgs")
            End Try
        End Sub
        '
        '====================================================================================================
        '
        Private Sub saveResponseDetails(ByVal cp As CPBaseClass, ByRef quiz As DistanceLearning.Models.QuizModel, ByRef response As DistanceLearning.Models.QuizResponseModel)
            Try
                Dim questionList As List(Of DistanceLearning.Models.QuizQuestionModel) = DistanceLearning.Models.QuizQuestionModel.getQuestionsForQuizList(cp, quiz.id)
                For Each question As DistanceLearning.Models.QuizQuestionModel In questionList
                    Dim SelectedAnswerID As Integer = cp.Doc.GetInteger("question" & question.id.ToString() & "answer")
                    If SelectedAnswerID <> 0 Then
                        Dim responseDetails As DistanceLearning.Models.QuizResponseDetailModel = DistanceLearning.Models.QuizResponseDetailModel.create(cp, response.id, question.id)
                        If (responseDetails Is Nothing) Then
                            responseDetails = DistanceLearning.Models.QuizResponseDetailModel.add(cp)
                            responseDetails.name = response.name & ", question: " & question.name
                            responseDetails.responseId = response.id
                            responseDetails.questionId = question.id
                        End If
                        responseDetails.answerId = SelectedAnswerID
                        responseDetails.saveObject(cp)
                    End If
                Next
            Catch ex As Exception
                errorReport(cp, ex, "saveResponseDetails")
            End Try
        End Sub
        '
        '====================================================================================================
        '
        Private Function getResponseSelectedAnswerId(ByVal cp As CPBaseClass, ByRef responseId As Integer, ByRef questionId As Integer) As Integer
            Dim returnShort As Integer = 0
            Try
                Dim cs As CPCSBaseClass = cp.CSNew()
                '
                Call cs.Open("quiz response details", "(responseid=" & responseId & ")and(questionid=" & questionId & ")")
                If cs.OK() Then
                    returnShort = cs.GetInteger("answerId")
                End If
                Call cs.Close()
            Catch ex As Exception
                errorReport(cp, ex, "getResponseAnswerId")
            End Try
            Return returnShort
        End Function
        ''
        ''====================================================================================================
        ''
        'Private Sub verifyQuizResponse(ByVal cp As CPBaseClass, ByRef responseId As Integer, ByVal userId As Integer, ByVal quizId As Integer)
        '    Try
        '        '
        '        Dim cs As CPCSBaseClass = cp.CSNew()
        '        Dim userName As String
        '        Dim quizName As String
        '        Dim attemptNumber As Integer
        '        '
        '        ' verify response record
        '        '
        '        Call cs.Open("quiz responses", "id=" & responseId)
        '        If Not cs.OK() Then
        '            Call cs.Close()
        '            attemptNumber = 1
        '            Call cs.OpenSQL("select count(*) as cnt from quizResponses where (memberid=" & userId & ")and(quizid=" & quizId & ")and(dateSubmitted is not null)")
        '            If cs.OK() Then
        '                attemptNumber = cs.GetInteger("cnt") + 1
        '            End If
        '            Call cs.Close()

        '            userName = cp.Content.GetRecordName("people", userId)
        '            quizName = cp.Content.GetRecordName("quizzes", quizId)
        '            Call cs.Insert("quiz responses")
        '            responseId = cs.GetInteger("id")
        '            Call cs.SetField("name", userName & ", quiz:" & quizName)
        '            Call cs.SetField("memberid", userId.ToString())
        '            Call cs.SetField("quizid", quizId.ToString())
        '            Call cs.SetField("attemptNumber", attemptNumber.ToString())
        '        End If
        '        Call cs.Close()
        '    Catch ex As Exception
        '        errorReport(cp, ex, "verifyQuizResponse")
        '    End Try
        'End Sub
        '
        '====================================================================================================
        '
        Private Function adminHintWrapper(ByVal cp As CPBaseClass, ByVal hint As String) As String
            Dim returnHtml As String = ""
            '
            If cp.User.IsEditingAnything() Or cp.User.IsAdmin() Then
                returnHtml = "" _
                    & "<table border=0 width=""100%"" cellspacing=0 cellpadding=0><tr><td class=""ccHintWrapper"">" _
                    & "<table border=0 width=""100%"" cellspacing=0 cellpadding=0><tr><td class=""ccHintWrapperContent"">" _
                    & "<b>Administrator</b>" _
                    & "<BR>" _
                    & "<BR>" & hint _
                    & "</td></tr></table>" _
                    & "</td></tr></table>" _
                    & ""
            End If
            Return returnHtml
        End Function
        ''
        ''====================================================================================================
        '' pageOrder = getNextpageOrder( cp, quizId, pageOrder )
        ''
        'Private Function getNextPageOrder(ByVal cp As CPBaseClass, ByVal quizId As Integer, ByVal pageOrder As Integer, ByVal isStudyPage As Boolean) As Integer
        '    Dim returnInt As Integer = pageOrder
        '    Try
        '        Dim sqlCriteria As String = "(quizId=" & quizId & ")and(pageOrder>" & pageOrder & ")"
        '        Dim cs As CPCSBaseClass = cp.CSNew()
        '        '
        '        If isStudyPage Then
        '            returnInt = getFirstPageOrder(cp, quizId)
        '        Else
        '            If cs.Open("quiz questions", sqlCriteria, "pageOrder,id", , "pageOrder") Then
        '                returnInt = cs.GetInteger("pageOrder")
        '            End If
        '            Call cs.Close()
        '        End If
        '    Catch ex As Exception
        '        Call errorReport(cp, ex, "getNextpageOrder")
        '    End Try
        '    Return returnInt
        'End Function
        ''
        ''====================================================================================================
        '' pageOrder = getFirstpageOrder( cp, quizId )
        ''
        'Private Function getFirstPageOrder(ByVal cp As CPBaseClass, ByVal quizId As Integer) As Integer
        '    Dim returnInt As Integer = 0
        '    Try
        '        Dim sqlCriteria As String = "(quizId=" & quizId & ")"
        '        Dim cs As CPCSBaseClass = cp.CSNew()
        '        If cs.Open("quiz questions", sqlCriteria, "pageOrder,id", , "pageOrder") Then
        '            returnInt = cs.GetInteger("pageOrder")
        '        End If
        '        Call cs.Close()
        '    Catch ex As Exception
        '        Call errorReport(cp, ex, "getFirstPageOrder")
        '    End Try
        '    Return returnInt
        'End Function
        ''
        ''====================================================================================================
        '' pageOrder = getPreviouspageOrder( cp, quizId, pageOrder )
        ''
        'Private Function getPreviousPageOrder(ByVal cp As CPBaseClass, ByVal quizId As Integer, ByVal pageOrder As Integer, ByRef isStudyPage As Boolean) As Integer
        '    Dim returnInt As Integer = pageOrder
        '    Try
        '        Dim sqlCriteria As String = "(quizId=" & quizId & ")"
        '        Dim cs As CPCSBaseClass = cp.CSNew()
        '        '
        '        If Not isStudyPage Then
        '            sqlCriteria &= "and((pageorder is null)or(pageOrder<" & pageOrder & "))"
        '        End If
        '        If cs.Open("quiz questions", sqlCriteria, "pageOrder desc", , "pageOrder") Then
        '            returnInt = cs.GetInteger("pageOrder")
        '        End If
        '        Call cs.Close()
        '    Catch ex As Exception
        '        Call errorReport(cp, ex, "getPreviousPageOrder")
        '    End Try
        '    Return returnInt
        'End Function
        '
        '====================================================================================================
        '
        Private Function processOnlineQuizForm(cp As CPBaseClass, quiz As DistanceLearning.Models.QuizModel, response As Contensive.Addons.DistanceLearning.Models.QuizResponseModel, ByRef userMessages As List(Of String)) As Integer
            Dim result As Integer = 0
            Try
                Dim button As String = cp.Doc.GetText(rnButton)
                Select Case LCase(button)
                    Case "begin quiz"
                        '
                        ' ? 
                        '
                    Case "resume quiz"
                        '
                        ' ? 
                        '
                    Case "previous"
                        '
                        ' previous - if past start, try studypage
                        '
                        Call saveResponseDetails(cp, quiz, response)
                    Case "submit"
                        '
                        ' submit
                        '
                        Call saveResponseDetails(cp, quiz, response)
                        Call scoreResponse(cp, response.id)
                        response.dateSubmitted = Now
                        response.saveObject(cp)
                    Case "continue"
                        '
                        ' continue
                        '
                        Call saveResponseDetails(cp, quiz, response)
                    Case "save"
                        '
                        ' save the response
                        '
                        Call saveResponseDetails(cp, quiz, response)
                        userMessages.Add("<p>Your quiz has been saved.</p>")
                End Select
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
        '
        '====================================================================================================
        '
        Private Function processScoreCardForm(cp As CPBaseClass, ByVal quiz As DistanceLearning.Models.QuizModel, ByRef response As DistanceLearning.Models.QuizResponseModel, ByRef userMessages As List(Of String)) As Integer
            Dim result As Integer = 0
            Try
                Dim Action As String = cp.Doc.GetText("Action")
                '
                If (response.dateSubmitted > Date.MinValue) Then
                    If LCase(Action) = "retake quiz" Then
                        '
                        ' start a retake - create a response and set dstPageOrder, isStudyPage
                        '
                        Dim previousResponses As List(Of DistanceLearning.Models.QuizResponseModel) = DistanceLearning.Models.QuizResponseModel.GetResponseList(cp, quiz.id, cp.User.Id)
                        response = DistanceLearning.Models.QuizResponseModel.add(cp, quiz.id)
                        response.name = cp.User.Name + ", quiz:" & quiz.name
                        response.QuizID = quiz.id
                        response.MemberID = cp.User.Id
                        response.attemptNumber = previousResponses.Count + 1
                        response.saveObject(cp)
                    End If
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
        '
        '====================================================================================================
        '
        Private Function getOnlineQuizForm(cp As CPBaseClass, quiz As DistanceLearning.Models.QuizModel, response As DistanceLearning.Models.QuizResponseModel) As String
            Dim returnHtml As String = ""
            Try
                Dim buttonList As String = ""
                Dim responseAnswerId As Integer
                Dim answerId As Integer
                Dim htmlRadio As String
                Dim answerCnt As Integer
                Dim questionCnt As Integer
                'Dim cs As CPCSBaseClass = cp.CSNew()
                'Dim cs2 As CPCSBaseClass = cp.CSNew()
                'Dim CS3 As CPCSBaseClass = cp.CSNew()
                'Dim cs4 As CPCSBaseClass = cp.CSNew()
                Dim Counter As Integer
                Dim q As String
                'Dim quizVideo As String = ""
                Dim questionId As Integer
                'Dim sqlCriteria As String
                Dim questionName As String
                Dim quizEditIcon As String = ""
                Dim topCopy As String = ""
                Dim buttonCopy As String = ""
                Dim nextPageOrder As Integer
                Dim qs As String
                Dim formAction As String
                Dim previousPageOrder As Integer
                Dim isLastPage As Boolean
                Dim isFirstPage As Boolean
                'Dim quizIncludeStudyPage As Boolean = False
                'Dim quizStudycopy As String = ""
                Dim answerText As String = ""
                'Dim quizTypeId As Integer
                'Dim quizDateStart As Date
                'Dim quizPageList As String
                Dim quizpages() As String
                Dim pagePtr As Integer
                Dim quizProgress As Double
                Dim jsHead As String
                Dim progressBarHtml As String = ""
                Dim questionSubjectId As Integer
                Dim subjectName As String = ""
                '
                ' not submitted -- mark the response with the lastDisplayedPageOrder 
                '
                response.lastDisplayedPageOrder = 1
                response.lastDisplayedStudyPage = 1
                If (response.dateStarted = Date.MinValue) Then
                    response.dateStarted = Now
                End If
                response.saveObject(cp)
                '
                ' take the quiz for the first time
                '
                Dim quizProgressText As String = ""
                '
                ' progress bar for all but first page
                '
                quizProgress = 0
                quizPageList = cs.GetText("pageList")
                If quizPageList <> "" Then
                    quizpages = quizPageList.Split(","c)
                    For pagePtr = 0 To quizpages.Length - 1
                        If (quizpages(pagePtr) = dstPageOrder.ToString) Then
                            Exit For
                        End If
                    Next
                    If (pagePtr > 0) Then
                        quizProgress = pagePtr / quizpages.Length
                        quizProgressText = CStr(Int(quizProgress * 100))
                        progressBarHtml = "" _
                                        & cr & "<div class=""progressbarCon"">" _
                                        & cr & "<div class=""progressbarTitle"">Your Progress " & quizProgressText & "%</div>" _
                                        & cr & "<div id=""progressbar""></div>" _
                                        & cr & "</div>"
                        jsHead = "$(document).ready(function(){$(""#progressbar"").progressbar({value:" & quizProgressText & "});});"
                        cp.Doc.AddHeadJavascript(jsHead)
                    End If
                End If
                '
                If isEditing Then
                    returnHtml &= "<div class=""quizName"">" & quizEditIcon & "Edit this quiz (" & cs.GetText("name") & ")</div>"
                End If
                '
                If pagePtr = 0 Then
                    If cs.GetBoolean("allowCustomTopCopy") Then
                        topCopy = cs.GetText("CustomTopCopy")
                    Else
                        If (quizVideo <> "") Then
                            topCopy = defaultTopVideoCopy
                        Else
                            topCopy = defaultTopNoVideoCopy
                        End If
                        If Not quizAllowRetake Then
                            topCopy = topCopy & defaultOneTime
                        End If
                    End If
                End If
                '
                If cs.GetBoolean("allowCustomButtonCopy") Then
                    buttonCopy = cs.GetText("CustomButtonCopy")
                ElseIf isAuthenticated Then
                    buttonCopy = defaultButtonWithSaveCopy
                Else
                    buttonCopy = defaultButtonCopy
                End If
                '
                questionCnt = 0
                sqlCriteria = "(QuizID=" & cp.Db.EncodeSQLNumber(quizId) & ")"
                If dstPageOrder <= 0 Then
                    sqlCriteria &= "and((pageOrder=0)or(pageOrder is null))"
                Else
                    sqlCriteria &= "and(pageOrder=" & dstPageOrder.ToString() & ")"
                End If
                cs2.Open("Quiz Questions", sqlCriteria, "QOrder")
                If Not cs2.OK() Then
                    '
                    ' no questions found
                    '
                    adminHint &= "<p>No Quiz Questions can be found for this quiz.</p>"
                Else
                    '
                    Counter = 0
                    '
                    questionName = cs2.GetText("name")
                    Do While cs2.OK()
                        questionId = cs2.GetInteger("ID")
                        questionSubjectId = cs2.GetInteger("subjectId")
                        Counter = Counter + 1
                        q = ""
                        '
                        ' Add Question
                        '
                        quizEditIcon = ""
                        If isEditing Then
                            quizEditIcon = cs2.GetEditLink(False) & "&nbsp;&nbsp;&nbsp;&nbsp;"
                        End If
                        q = q & vbCrLf & vbTab & "<div class=""questionText"">" & quizEditIcon & cs2.GetText("name") & "</div>"
                        '
                        ' Add subject edit
                        '
                        If isEditing And (questionSubjectId <> 0) Then
                            subjectName = cp.Content.GetRecordName("quiz subjects", questionSubjectId)
                            quizEditIcon = cp.Content.GetEditLink("quiz subjects", questionSubjectId.ToString(), False, subjectName, True) & "&nbsp;&nbsp;&nbsp;&nbsp;"
                            q &= cr & "<div class=""questionChoice"">" & quizEditIcon & "&nbsp;Edit the subject for this question, " & subjectName & ".</div>"
                        End If
                        '
                        ' Add Choices
                        '
                        Call CS3.Open("Quiz Answers", "QuestionID=" & CStr(questionId), "QOrder")
                        answerCnt = 0
                        If Not CS3.OK() Then
                            '
                            ' this question has no answers
                            '
                            adminHint &= "<p>Your Quiz Question """ & questionName & """ does not appear to have any answers configured. To add answers, turn on Edit and click the Add icon under the question.</p>"
                        Else

                            responseAnswerId = 0
                            Call cs4.Open("quiz response details", "(responseid=" & responseId & ")and(questionid=" & questionId & ")")
                            If cs4.OK() Then
                                responseAnswerId = cs4.GetInteger("answerId")
                            End If
                            Call cs4.Close()
                            '
                            Do While CS3.OK()
                                '
                                answerText = CS3.GetText("name")
                                'answerText = CS3.GetText("AText")
                                quizEditIcon = ""
                                If isEditing Then
                                    If quizTypeId = quizTypeIdPoints Then
                                        If (CS3.GetInteger("points") = 1) Then
                                            answerText = "[" & CS3.GetInteger("points") & " point ] " & answerText
                                        Else
                                            answerText = "[" & CS3.GetInteger("points") & " points ] " & answerText
                                        End If
                                    ElseIf CS3.GetBoolean("correct") Then
                                        answerText = "[ correct answer ] " & answerText
                                    End If
                                    quizEditIcon = CS3.GetEditLink(False) & "&nbsp;&nbsp;&nbsp;&nbsp;"
                                End If
                                answerId = CS3.GetInteger("ID")
                                htmlRadio = "<input type=""radio"" name=""" & CStr(Counter) & "Answer"" value=""" & answerId & """"
                                If (answerId = responseAnswerId) Then
                                    htmlRadio = htmlRadio & " checked=""checked"">"
                                Else
                                    htmlRadio = htmlRadio & ">"
                                End If
                                q = q & vbCrLf & vbTab & "<div class=""questionChoice"">" & htmlRadio & "" & quizEditIcon & "&nbsp;" & answerText & "</div>"
                                answerCnt = answerCnt + 1
                                Call CS3.GoNext()
                                '
                            Loop
                        End If
                        If isEditing Then
                            quizEditIcon = CS3.GetAddLink("questionId=" & questionId)
                            If answerCnt > 0 Then
                                q = q & vbCrLf & vbTab & "<div class=""questionChoice"">" & quizEditIcon & "&nbsp;Add another answer to this question.</div>"
                            Else
                                q = q & vbCrLf & vbTab & "<div class=""questionChoice"">" & quizEditIcon & "&nbsp;Add an answer to this question.</div>"
                            End If
                        End If
                        '
                        Call CS3.Close()
                        '
                        returnHtml &= vbCrLf & vbTab & "<div class=""question"">" & cp.Html.Indent(q) & vbCrLf & vbTab & "</div>"
                        questionCnt = questionCnt + 1
                        cs2.GoNext()
                    Loop
                End If
                If isEditing Then
                    quizEditIcon = cs2.GetAddLink("quizid=" & quizId & ",pageOrder=" & dstPageOrder)
                    If questionCnt > 0 Then
                        returnHtml &= vbCrLf & vbTab & "<div class=""questionText"">" & quizEditIcon & "&nbsp;Add another question to the quiz.</div>"
                    Else
                        returnHtml &= vbCrLf & vbTab & "<div class=""questionText"">" & quizEditIcon & "&nbsp;Add a question to the quiz.</div>"
                    End If
                End If
                Call cs2.Close()
                '
                ' Add hiddens and button
                '
                buttonList = ""
                nextPageOrder = getNextPageOrder(cp, quizId, dstPageOrder, isStudyPage)
                previousPageOrder = getPreviousPageOrder(cp, quizId, dstPageOrder, isStudyPage)
                isLastPage = (dstPageOrder = nextPageOrder)
                isFirstPage = (dstPageOrder = previousPageOrder)
                If isFirstPage And quizIncludeStudyPage Then
                    '
                    ' go back to study page
                    '
                    buttonList &= vbCrLf & vbTab & "<input type=""submit"" name=""action"" value=""Previous"" onClick=""return verifyAnswers();"">" & ""
                End If
                If Not isFirstPage Then
                    '
                    ' not first page
                    '
                    buttonList &= vbCrLf & vbTab & "<input type=""submit"" name=""action"" value=""Previous"" onClick=""return verifyAnswers();"">" & ""
                End If
                If isAuthenticated Then
                    '
                    ' if authenticated, allow save
                    '
                    buttonList &= vbCrLf & vbTab & "<input type=""submit"" name=""action"" value=""Save"" onClick=""return verifyAnswers();"">"
                End If
                If isLastPage Then
                    '
                    ' this is the last page of the quiz
                    '
                    buttonList &= vbCrLf & vbTab & "<input type=""submit"" name=""action"" value=""Submit"" onClick=""return verifyAnswers();"">"
                Else
                    '
                    ' now last, allow continue
                    '
                    buttonList &= vbCrLf & vbTab & "<input type=""submit"" name=""action"" value=""Continue"" onClick=""return verifyAnswers();"">"
                End If
                returnHtml = "" _
                        & cp.Html.Indent(returnHtml) _
                        & cp.Html.Indent(buttonCopy) _
                        & vbCrLf & vbTab & "<div class=""button"">" _
                        & cp.Html.Indent(buttonList) _
                        & vbCrLf & vbTab & "</div>" _
                        & progressBarHtml
                '
                ' Add form wrapper
                '
                qs = cp.Doc.RefreshQueryString
                qs = cp.Utils.ModifyQueryString(qs, rnDstPageOrder, dstPageOrder.ToString(), True)
                formAction = "?" & qs
                returnHtml = "" _
                            & cp.Html.Indent(userMessages) _
                            & cp.Html.Indent(topCopy) _
                            & vbCrLf & vbTab & "<form method=""post"" name=""quizForm"" action=""" & formAction & """>" _
                            & cp.Html.Indent(returnHtml) _
                            & vbCrLf & vbTab & "<input type=""hidden"" name=""quizID"" value=""" & quizId & """>" _
                            & vbCrLf & vbTab & "<input type=""hidden"" name=""" & rnSrcPageOrder & """ value=""" & dstPageOrder & """>" _
                            & vbCrLf & vbTab & "<input type=""hidden"" name=""qNumbers"" value=""" & CStr(Counter) & """>" _
                            & vbCrLf & vbTab & "<input type=""hidden"" name=""quizName"" value=""" & quizName & """>" _
                            & vbCrLf & vbTab & "<input type=""hidden"" name=""responseId"" value=""" & responseId & """>" _
                            & vbCrLf & vbTab & "</form>"
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return returnHtml
        End Function
        '
        '====================================================================================================
        '
        Private Function getScoreCardform(cp As CPBaseClass, quiz As DistanceLearning.Models.QuizModel, response As DistanceLearning.Models.QuizResponseModel) As String
            Dim returnHtml As String = ""
            Try
                Dim buttonList As String = ""
                Dim Counter As Integer
                Dim quizEditIcon As String = ""
                Dim topCopy As String = ""
                Dim buttonCopy As String = ""
                Dim qs As String
                Dim formAction As String
                Dim answerText As String = ""
                Dim progressBarHtml As String = ""
                Dim subjectName As String = ""
                '
                ' show result page
                If quiz.typeId = quizTypeIdPoints Then
                    '
                    ' point-based quiz, show result message
                    '
                    Dim resultMessage As DistanceLearning.Models.QuizResultMessageModel = DistanceLearning.Models.QuizResultMessageModel.createByPointThreshold(cp, response.totalPoints)
                    If (resultMessage Is Nothing) Then
                        returnHtml = "<p>Thank you. The quiz is complete.</p>"
                    Else
                        returnHtml = resultMessage.copy
                    End If
                Else
                    '
                    ' graded quiz, show scorecard
                    '
                    Call cp.Doc.SetProperty("id", responseId.ToString())
                    returnHtml = cp.Utils.ExecuteAddon(scoreCardAddon)
                End If
                If quiz.allowRetake Then
                    buttonCopy = cr & "<p>You may retake this quiz. To begin, click Retake.</p>"
                    buttonList = cr & "<input type=""submit"" name=""action"" value=""Retake Quiz"">"
                    returnHtml = "" _
                            & cp.Html.Indent(returnHtml) _
                            & cp.Html.Indent(buttonCopy) _
                            & vbCrLf & vbTab & "<div class=""button"">" _
                            & cp.Html.Indent(buttonList) _
                            & vbCrLf & vbTab & "</div>"
                End If
                '
                ' Add form wrapper
                '
                qs = cp.Doc.RefreshQueryString
                qs = cp.Utils.ModifyQueryString(qs, rnDstPageOrder, dstPageOrder.ToString(), True)
                formAction = "?" & qs
                returnHtml = "" _
                    & cp.Html.Indent(userMessages) _
                    & cp.Html.Indent(topCopy) _
                    & vbCrLf & vbTab & "<form method=""post"" name=""quizForm"" action=""" & formAction & """>" _
                    & cp.Html.Indent(returnHtml) _
                    & vbCrLf & vbTab & "<input type=""hidden"" name=""quizID"" value=""" & quizId & """>" _
                    & vbCrLf & vbTab & "<input type=""hidden"" name=""" & rnSrcPageOrder & """ value=""" & dstPageOrder & """>" _
                    & vbCrLf & vbTab & "<input type=""hidden"" name=""qNumbers"" value=""" & CStr(Counter) & """>" _
                    & vbCrLf & vbTab & "<input type=""hidden"" name=""quizName"" value=""" & quizName & """>" _
                    & vbCrLf & vbTab & "<input type=""hidden"" name=""responseId"" value=""" & responseId & """>" _
                    & vbCrLf & vbTab & "</form>"
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return returnHtml
        End Function
        '
        '====================================================================================================
        '
        Private Function getStudyPageForm(cp As CPBaseClass, quiz As DistanceLearning.Models.QuizModel, response As DistanceLearning.Models.QuizResponseModel) As String
            Dim result As String = "getStudyPageForm"
            Try
                '
                ' display the study page
                '
                returnHtml = quizStudycopy
                If quizVideo <> "" Then
                    '
                    returnHtml &= "<script>window.open('" & quizVideo & "', '');</script>"
                    '
                End If
                If (quizDateStart <> DateTime.MinValue) Then
                    buttonCopy = "<p>Click Resume Quiz when you are ready.</p>"
                    buttonList &= vbCrLf & vbTab & "<input type=""submit"" name=""action"" value=""Resume Quiz"">"
                Else
                    buttonCopy = "<p>Click Begin Quiz when you are ready to start.</p>"
                    buttonList &= vbCrLf & vbTab & "<input type=""submit"" name=""action"" value=""Begin Quiz"">"
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
        '
        '====================================================================================================
        '
        Private Function getAdminHints(cp As CPBaseClass, quiz As DistanceLearning.Models.QuizModel, response As DistanceLearning.Models.QuizResponseModel) As String
            Dim result As String = "getAdminHints"
            Try
                '
                Dim lastQuestionId As Integer = 0
                Dim foundCorrect As Boolean
                Dim lastQuestionName As String = ""
                Dim answerList As String
                Dim answersCid As Integer = cp.Content.GetID("quiz answers")
                Dim questionsCid As Integer = cp.Content.GetID("quiz questions")
                Dim quizResultMessagesCid As Integer = cp.Content.GetID("quiz result messages")
                Dim cs As CPCSBaseClass = cp.CSNew()
                Dim itemList As String = ""
                Dim adminUrl As String = cp.Site.GetText("adminUrl")
                '
                If cp.User.IsAdmin Then
                    If (quiz.typeId = quizTypeIdGraded) Then
                        '
                        ' graded quiz, check that all questions have at least one marked correct
                        '
                        Dim Sql As String = "select q.name as questionName,q.id as questionId,a.name as answerName,a.id as answerId,a.correct" _
                            & " from ((quizzes z" _
                            & " left join quizQuestions q on q.quizId=z.id)" _
                            & " left join quizAnswers a on a.questionid=q.id)" _
                            & " where(z.id=" & quiz.id & ")" _
                            & " order by q.id,a.id"
                        Call cs.OpenSQL(Sql)
                        If cs.OK() Then
                            lastQuestionId = 0
                            foundCorrect = True
                            answerList = ""
                            Do While cs.OK()
                                Dim questionName As String = cs.GetText("questionName")
                                Dim questionId As Integer = cs.GetInteger("questionId")
                                Dim answerName As String = cs.GetText("answerName")
                                Dim answerId As Integer = cs.GetInteger("answerId")
                                If (lastQuestionId <> questionId) Then
                                    '
                                    ' new question
                                    '
                                    If Not foundCorrect Then
                                        itemList &= "" _
                                            & cr & cp.Html.li(lastQuestionName) _
                                            & cr & cp.Html.ul(answerList) _
                                            & ""
                                    End If
                                    foundCorrect = False
                                    answerList = ""
                                End If
                                foundCorrect = foundCorrect Or cs.GetBoolean("correct")
                                answerList &= cr & cp.Html.li("<a href=""" & adminUrl & "?af=4&cid=" & answersCid & "&id=" & answerId & """>" & answerName & "</a>")
                                lastQuestionName = questionName
                                lastQuestionId = questionId
                                Call cs.GoNext()
                            Loop
                            If Not foundCorrect Then
                                itemList &= "" _
                                        & cr & cp.Html.li(lastQuestionName) _
                                        & cr & cp.Html.ul(answerList) _
                                        & ""
                            End If
                        End If
                        Call cs.Close()
                        If itemList <> "" Then
                            result &= "" _
                                    & cr & cp.Html.p("WARNING: This quiz is configured as a graded quiz and the following questions have no correct answer.") _
                                    & cr & cp.Html.ul(itemList)
                        End If
                    Else
                        '
                        ' points-based quiz, check that all answers have a point value
                        '
                        Dim Sql As String = "select a.name,a.id" _
                            & " from ((quizzes z" _
                            & " left join quizQuestions q on q.quizId=z.id)" _
                            & " left join quizAnswers a on a.questionid=q.id)" _
                            & " where(z.id=" & quiz.id & ")" _
                            & " and (points is null)"
                        Call cs.OpenSQL(Sql)
                        Do While cs.OK()
                            Dim answerId As Integer = cs.GetInteger("id")
                            If (answerId <> 0) Then
                                Dim answerName As String = cs.GetText("name")
                                If answerName = "" Then
                                    answerName = "Answer #" & answerId
                                End If
                                itemList &= cr & cp.Html.li("<a href=""" & adminUrl & "?af=4&cid=" & answersCid & "&id=" & answerId & """>" & answerName & "</a>")
                            End If
                            Call cs.GoNext()
                        Loop
                        Call cs.Close()
                        If itemList <> "" Then
                            result &= "" _
                                    & cr & cp.Html.p("WARNING: This quiz is configured as a points-based quiz and the following answers have no points assigned. They will count as 0 points.") _
                                    & cr & cp.Html.ul(itemList)
                        End If
                        '
                        ' check result messages
                        '
                        itemList = ""
                        Dim pointThreshold As Integer = 0
                        Dim itemListIssues As String = ""
                        Dim lastPointThreshold As Integer = 0
                        If (Not cs.Open("quiz result messages", "(quizid=" & quiz.id & ")", "pointThreshold")) Then
                            result &= cr & cp.Html.p("WARNING: This quiz has no Quiz Result Messages. The quiz will end with a simple thank you page.")
                        Else
                            Dim ptr As Integer = 0
                            Do While cs.OK
                                Dim itemId As Integer = cs.GetInteger("id")
                                Dim itemName As String = cs.GetText("name")
                                If itemName = "" Then
                                    itemName = "Message #" & itemId
                                End If
                                Dim itemEdit As String = "<a href=""" & adminUrl & "?af=4&cid=" & quizResultMessagesCid & "&id=" & itemId & """>" & itemName & "</a>"
                                Dim integerTest As String = cs.GetText("pointThreshold")
                                If (integerTest = "") Then
                                    itemListIssues &= cr & cp.Html.p("WARNING: Quiz Result Message '" & itemName & "' has no Point Threshold. It will never be shown." & itemEdit)
                                Else
                                    pointThreshold = cp.Utils.EncodeInteger(integerTest)
                                    If (ptr = 0) Then
                                        If pointThreshold > 0 Then
                                            itemListIssues &= cr & cp.Html.p("WARNING: There is no Quiz Result Message for point scores less than " & pointThreshold & ". The quiz will end with a simple thank you page.")
                                        End If
                                        'itemList &= "<li>Total Points under " & pointThreshold & " see Result Message " & itemEdit & "</li>"
                                    Else
                                        If (pointThreshold = lastPointThreshold) And (pointThreshold <> warningMsgPoints) Then
                                            warningMsgPoints = pointThreshold
                                            itemListIssues &= cr & cp.Html.p("WARNING: There are multiple Quiz Result Messages with a Point Threshold [" & pointThreshold & "]. Only the messag with the lowest ID # will be displayed.")
                                        End If
                                        itemList &= "<li>Total Points from " & lastPointThreshold & " to " & (pointThreshold - 1) & " see Result Message " & lastItemEdit & "</li>"
                                    End If
                                    lastPointThreshold = pointThreshold
                                    lastItemEdit = itemEdit
                                End If
                                ptr += 1
                                Call cs.GoNext()
                            Loop
                            itemList &= "<li>Total Points over " & lastPointThreshold & " see Result Message " & lastItemEdit & "</li>"
                        End If
                        link = adminUrl & "?cid=" & quizResultMessagesCid & "&af=4&aa=2&wc=quizid%3D" & quiz.id
                        itemList &= cr & cp.Html.li("<a href=""" & link & """>Add a Quiz Result Message</a>")
                        result &= "" _
                                & cr & cp.Html.p("Result Messages.") _
                                & cr & cp.Html.ul(itemList) _
                                & itemListIssues
                    End If
                    If result = "" Then
                        result &= "<p>Your online quiz appears to be configured correctly.</p>"
                    End If
                    If (quizSelected) Then
                        result &= "<p>Edit this quiz <a href=""" & adminUrl & "?af=4&cid=" & cp.Content.GetID("quizzes") & "&id=" & quiz.id & """>" & cp.Utils.EncodeHTML(quizName) & "</a>.</p>"
                        result &= "<p>To edit questions and answers on this page, turn on edit mode from the tool panel.</p>"
                    End If
                    result &= "<p>Create a <a href=""" & adminUrl & "?af=4&cid=" & cp.Content.GetID("quizzes") & """>new quiz</a>.</p>"
                    result &= "<p>Create a <a href=""" & adminUrl & "?af=4&cid=" & cp.Content.GetID("quiz subjects") & """>new quiz subject</a>.</p>"
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return result
        End Function
        '
        '====================================================================================================
        '
        Private Sub errorReport(ByVal cp As CPBaseClass, ByVal ex As Exception, ByVal method As String)
            Try
                cp.Site.ErrorReport(ex, "Unexpected error in quizClass." & method)
            Catch exLost As Exception
                '
                ' stop anything thrown from cp errorReport
                '
            End Try
        End Sub

    End Class
End Namespace