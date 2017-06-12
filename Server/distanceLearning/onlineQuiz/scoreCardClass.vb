
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses

Namespace Contensive.Addons.OnlineQuiz
    Public Class scoreCardClass
        Inherits AddonBaseClass
        '
        ' - update references to your installed version of cpBase
        ' - Verify project root name space is empty
        ' - Change the namespace to the collection name
        ' - Change this class name to the addon name
        ' - Create a Contensive Addon record with the namespace apCollectionName.ad
        ' - add reference to CPBase.DLL, typically installed in c:\program files\kma\contensive\
        '
        '=====================================================================================
        ' addon api
        '=====================================================================================
        '
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                Dim responseId As Integer
                '
                responseId = CP.Doc.GetInteger("Id")
                returnHtml = getScoreCard(CP, responseId)
            Catch ex As Exception
                errorReport(CP, ex, "execute")
            End Try
            Return returnHtml
        End Function
        '
        '=====================================================================================
        ' common report for this class
        '=====================================================================================
        '
        Private Sub errorReport(ByVal cp As CPBaseClass, ByVal ex As Exception, ByVal method As String)
            Try
                cp.Site.ErrorReport(ex, "Unexpected error in scoreCardClass." & method)
            Catch exLost As Exception
                '
                ' stop anything thrown from cp errorReport
                '
            End Try
        End Sub
        '
        '========================================================================
        '
        '========================================================================
        '
        Private Function getScoreCard(ByVal cp As CPBaseClass, ByVal responseId As Integer) As String
            Dim returnHtml As String = ""
            Dim hint As Integer = 0
            Try
                '
                Dim chart As String = ""
                Dim responseSubject As String
                Dim responseName As String
                Dim scoreCaption As String
                Dim Choice As String
                Dim ScoreBox As String
                'Dim TotalCorrect As Integer
                'Dim TotalQuestions As Integer
                'Dim answerId As Integer
                'Dim AnswerCorrect As Boolean
                'Dim cs As CPCSBaseClass = cp.CSNew()
                'Dim cs2 As CPCSBaseClass = cp.CSNew()
                'Dim CS3 As CPCSBaseClass = cp.CSNew()
                Dim cs4 As CPCSBaseClass = cp.CSNew()
                Dim totalQuestions As Integer
                Dim SummaryBox As String = ""
                Dim questionRow As String
                'Dim SelectedAnswerID As Integer
                Dim Correct As Boolean
                Dim totalQuestionsIncorrect As Integer
                Dim totalQuestionsCorrect As Integer
                Dim Percentage As Double
                Dim Passed As Boolean
                'Dim SubjectID As Integer
                Dim subjects() As subjectsStruc = Nothing
                'Dim questionId As Integer
                Dim subjectCnt As Integer = 0
                Dim subjectPtr As Integer
                Dim GradeCaption As String = ""
                'Dim OverallCaption As String
                Dim SubjectCaptions() As String = Nothing
                Dim subjectScores() As Integer = Nothing
                Dim headerCopy As String
                Dim footerCopy As String
                'Dim quizName As String = ""
                Dim rightNow As Date
                'Dim userId As Integer
                'Dim quizId As Integer
                'Dim answerPoints As Integer
                'Dim resultPoints As Integer = 0
                '
                rightNow = Now()
                '
                ' only the responseId is valid, lookup the quiz and userId
                '
                Dim response As DistanceLearning.Models.QuizResponseModel = DistanceLearning.Models.QuizResponseModel.create(cp, responseId)
                If (response Is Nothing) Then
                    '
                    ' -- response is not valid
                    returnHtml = "<p>The requested response could not be found.</p>"
                Else
                    Dim responseDetailList As List(Of DistanceLearning.Models.QuizResponseDetailModel) = DistanceLearning.Models.QuizResponseDetailModel.getObjectListForQuizDisplay(cp, response.id)
                    Dim quiz As DistanceLearning.Models.QuizModel = DistanceLearning.Models.QuizModel.create(cp, response.QuizID)
                    If (quiz Is Nothing) Then
                        '
                        ' -- quiz not valid
                        returnHtml = "<p>The quiz associated to the response requested is not currently longer available.</p>"
                    Else
                        subjectCnt = 0
                        totalQuestions = 0
                        totalQuestionsIncorrect = 0
                        totalQuestionsCorrect = 0
                        Dim totalPoints As Integer = 0
                        For Each responseDetail As DistanceLearning.Models.QuizResponseDetailModel In responseDetailList
                            '
                            ' -- process all the questions selected in the resposne detail list
                            Dim question As DistanceLearning.Models.QuizQuestionModel = DistanceLearning.Models.QuizQuestionModel.create(cp, responseDetail.questionId)
                            hint = 20
                            totalQuestions = totalQuestions + 1
                            '
                            ' Set subjectPtr to the Response Array for this question's subject
                            subjectPtr = 0
                            If question.SubjectID > 0 Then
                                Do While (subjectPtr < subjectCnt)
                                    If (subjects(subjectPtr).SubjectID = question.SubjectID) Then
                                        Exit Do
                                    End If
                                    subjectPtr = subjectPtr + 1
                                Loop
                                If subjectPtr >= subjectCnt Then
                                    ReDim Preserve subjects(subjectPtr)
                                    subjects(subjectPtr).SubjectID = question.SubjectID
                                    subjectCnt = subjectCnt + 1
                                End If
                                subjects(subjectPtr).TotalQuestions = subjects(subjectPtr).TotalQuestions + 1
                            End If
                            '
                            ' New Question
                            '
                            hint = 30
                            questionRow = ""
                            Dim answerList As List(Of DistanceLearning.Models.QuizAnswerModel) = DistanceLearning.Models.QuizAnswerModel.getAnswersForQuestionList(cp, question.id)
                            If (answerList.Count = 0) Then
                                '
                                ' --Question with no choices is correct
                                Correct = True
                            Else
                                '
                                ' -- Add Choices
                                Correct = False
                                For Each answer As DistanceLearning.Models.QuizAnswerModel In answerList
                                    '
                                    ' -- cycle through all the answers and build display
                                    Choice = ""
                                    If answer.id = responseDetail.answerId Then
                                        '
                                        ' -- user selected this answer
                                        totalPoints += answer.points
                                        If (subjectCnt > subjectPtr) And (question.SubjectID > 0) Then
                                            subjects(subjectPtr).points += answer.points
                                        End If
                                        Choice = Choice & "<input type=""radio"" name=""" & totalQuestions & "Answer"" value=""" & answer.id.ToString() & """ checked disabled>"
                                        Choice = Choice & "&nbsp;" & answer.copy
                                        If answer.Correct Then
                                            '
                                            ' selected answer is current answer
                                            Correct = True
                                        End If
                                        questionRow = questionRow & vbCrLf & vbTab & "<div class=""questionChoice"">" & Choice & "</div>"
                                    Else
                                        '
                                        ' -- user did not select this answer
                                        Choice = Choice & "<input type=""radio"" name=""" & totalQuestions & "Answer"" value=""" & answer.id.ToString() & """ disabled>"
                                        Choice = Choice & "&nbsp;" & answer.copy
                                        If answer.Correct Then
                                            '
                                            ' unselected answer is correct answer, make red
                                            '
                                            questionRow = questionRow & vbCrLf & vbTab & "<div class=""questionChoiceWrong"">" & Choice & "</div>"
                                        Else
                                            '
                                            ' unselected answer is not correct answer
                                            '
                                            questionRow = questionRow & vbCrLf & vbTab & "<div class=""questionChoice"">" & Choice & "</div>"
                                        End If
                                    End If
                                Next
                            End If
                            '
                            ' Add QuestionText to top
                            '
                            hint = 50
                            If Correct Then
                                questionRow = "" _
                                & vbCrLf & vbTab & "<div class=""questionText"">" & question.copy & "</div>" _
                                & questionRow
                                totalQuestionsCorrect = totalQuestionsCorrect + 1
                                'If (subjectCnt > subjectPtr) Then
                                '    If question.SubjectID > 0 Then
                                '        subjects(subjectPtr).CorrectAnswers = subjects(subjectPtr).CorrectAnswers + 1
                                '    End If
                                'End If
                            Else
                                questionRow = "" _
                                & vbCrLf & vbTab & "<div class=""questionTextWrong"">" & question.copy & "</div>" _
                                & questionRow
                                Passed = False
                                totalQuestionsIncorrect = totalQuestionsIncorrect + 1
                            End If
                            '
                            ' Add Explination
                            '
                            questionRow &= vbCrLf & vbTab & "<div class=""instructionText"">" & question.instructions & "</div>"
                            'hint = 50
                            'If Correct Then
                            '    questionRow = "" _
                            '    & vbCrLf & vbTab & "<div class=""instructionText"">" & question.instructions & "</div>" _
                            '    & questionRow
                            '    totalQuestionsCorrect = totalQuestionsCorrect + 1
                            '    If (subjectCnt > subjectPtr) Then
                            '        If question.SubjectID > 0 Then
                            '            subjects(subjectPtr).CorrectAnswers = subjects(subjectPtr).CorrectAnswers + 1
                            '        End If
                            '    End If
                            'Else
                            '    questionRow = "" _
                            '    & vbCrLf & vbTab & "<div class=""instructionText"">" & question.instructions & "</div>" _
                            '    & questionRow
                            '    Passed = False
                            '    totalQuestionsIncorrect = totalQuestionsIncorrect + 1
                            'End If
                            '
                            ' Question container
                            '
                            SummaryBox &= "" _
                            & vbCrLf & vbTab & "<div class=""question"">" _
                            & cp.Html.Indent(questionRow) _
                            & vbCrLf & vbTab & "</div>"
                        Next
                        response.totalCorrect = totalQuestionsCorrect
                        response.totalPoints = totalPoints
                        response.totalQuestions = responseDetailList.Count
                        response.Score = 0
                        If (response.totalQuestions > 0) Then
                            response.Score = (CDbl(response.totalCorrect) * 100) / CDbl(response.totalQuestions)
                        End If
                        response.saveObject(cp)
                        '
                        hint = 60
                        If totalQuestionsIncorrect = 0 Then
                            '
                            Passed = True
                            '
                        Else
                            '
                            SummaryBox = "" _
                            & vbCrLf & vbTab & "<div class=""errorMsg"">Questions with incorrect answers are highlighted in red.</div>" _
                            & SummaryBox
                        End If
                        '
                        ' add hiddens
                        '
                        SummaryBox = "" _
                        & SummaryBox _
                        & vbCrLf & vbTab & "<input type=""hidden"" name=""quizID"" value=""" & CStr(response.QuizID) & """>" _
                        & vbCrLf & vbTab & "<input type=""hidden"" name=""scoreCard"" value=""1"">" _
                        & vbCrLf & vbTab & "<input type=""hidden"" name=""qNumbers"" value=""" & CStr(totalQuestions) & """>"
                        '
                        ' add summary wrapper
                        '
                        SummaryBox = "" _
                        & vbCrLf & vbTab & "<div class=""summary"">" _
                        & cp.Html.Indent(SummaryBox) _
                        & vbCrLf & vbTab & "</div>"
                        '
                        ' Build ScoreBox
                        '
                        hint = 70
                        ScoreBox = ""
                        'TotalCorrect = 0
                        'TotalQuestions = 0
                        If False Then
                            '
                            ' -- no longer score by subject
                            If subjectCnt > 0 Then
                                ReDim SubjectCaptions(subjectCnt - 1)
                                ReDim subjectScores(subjectCnt - 1)
                                '
                                ' if more than just 'no subject', Iterate through all the subjects, displaying the score for each
                                '
                                For subjectPtr = 0 To subjectCnt - 1
                                    With subjects(subjectPtr)
                                        If .TotalQuestions > 0 Then
                                            '
                                            ' Determine Score
                                            '
                                            'TotalQuestions = TotalQuestions + .TotalQuestions
                                            'TotalCorrect = TotalCorrect + .CorrectAnswers
                                            .Score = CDbl(.CorrectAnswers) / CDbl(.TotalQuestions)
                                            '
                                            ' Determine grade and get Caption
                                            '
                                            Call getSubjectGradeCaption(cp, .SubjectID, .Score, .SubjectCaption, GradeCaption)
                                            '
                                            ' Save Chart data
                                            '
                                            SubjectCaptions(subjectPtr) = .SubjectCaption & "\n" & GradeCaption
                                            subjectScores(subjectPtr) = .Score * 100
                                            '
                                            ' Output line
                                            '
                                            ScoreBox = ScoreBox & vbCrLf & vbTab & "<li class=""subjectScore"">Your grade for the " & .SubjectCaption & " section is <b>" & GradeCaption & "</b>, " & CStr(.CorrectAnswers) & " correct out of " & CStr(.TotalQuestions) & " questions is " & CStr(Int(.Score * 100)) & "%.</li>"
                                        End If
                                    End With
                                Next
                            End If
                            hint = 80
                            If ScoreBox <> "" Then
                                ScoreBox = "" _
                                & vbCrLf & vbTab & "<div class=""subjectScoreCaption"">Your score in each subject is as follows:</div>" _
                                & vbCrLf & vbTab & "<ul class=""subjectScoreList"">" _
                                & cp.Html.Indent(ScoreBox) _
                                & vbCrLf & vbTab & "</ul>"
                            End If
                        End If
                        '
                        Percentage = 0
                        If (response.totalQuestions > 0) Then
                            Dim PassingGrade As Boolean = False
                            Percentage = CDbl(response.Score) / CDbl(100)
                            getQuizGradeCaption(cp, quiz, Percentage, GradeCaption, PassingGrade)
                            scoreCaption = ""
                            If PassingGrade Then
                                scoreCaption = "Congratulations, you passed the quiz!"
                            Else
                                scoreCaption = "Your score did not pass the quiz."
                            End If
                            If GradeCaption <> "" Then
                                scoreCaption &= " Your grade for this quiz is <b>" & GradeCaption & "</b>, "
                            Else
                                scoreCaption &= " Your total score for this quiz is " & CStr(Int(Percentage * 100)) & "%"
                            End If
                            scoreCaption &= " (" & CStr(response.totalCorrect) & " correct out of " & CStr(response.totalQuestions)
                            If response.totalQuestions = 1 Then
                                scoreCaption = scoreCaption & " question)"
                            Else
                                scoreCaption = scoreCaption & " questions)"
                            End If
                            ScoreBox = vbCrLf & vbTab & "<div class=""totalScore"">" & scoreCaption & "</div>" & ScoreBox
                        End If
                        ScoreBox = "" _
                            & vbCrLf & vbTab & "<div class=""score"">" _
                            & cp.Html.Indent(ScoreBox) _
                            & vbCrLf & vbTab & "</div>"
                        '
                        ' Save the response
                        '
                        'Call verifyQuizResponse(cp, responseId, response.MemberID, response.QuizID)
                        '
                        If False Then
                            '
                            ' -- no longer score by subject
                            If subjectCnt > 0 Then
                                For subjectPtr = 0 To subjectCnt - 1
                                    With subjects(subjectPtr)
                                        Call cs4.Insert("Quiz Response Scores")
                                        If cs4.OK() Then
                                            responseName = cp.Content.GetRecordName("quiz responses", responseId)
                                            responseSubject = cp.Content.GetRecordName("quiz subjects", .SubjectID)
                                            '
                                            Call cs4.SetField("name", responseName & ", subject:" & responseSubject)
                                            Call cs4.SetField("QuizResponseID", responseId)
                                            Call cs4.SetField("QuizSubjectID", .SubjectID)
                                            Call cs4.SetField("Score", .Score)
                                        End If
                                        cs4.Close()
                                    End With
                                Next
                            End If
                        End If
                        '
                        headerCopy = "<div class=""aodlHeaderContainer"">" & cp.Content.GetCopy(quiz.name & " Summary Header Copy") & "</div>"
                        footerCopy = "<div class=""aodlFooterContainer"">" & cp.Content.GetCopy(quiz.name & " Summary Footer Copy") & "</div>"
                        '
                        ' Build the final form
                        '
                        'hint = 100
                        'chart = ""
                        'If subjectCnt > 1 Then
                        '    chart = GetChart(cp, subjectCnt, SubjectCaptions, subjectScores)
                        'End If
                        returnHtml = headerCopy & ScoreBox & SummaryBox & chart & footerCopy
                    End If
                End If
            Catch ex As Exception
                Call errorReport(cp, ex, "getScoreCard-hint=" & hint.ToString())
            End Try
            Return returnHtml
        End Function
        '
        '
        '
        Private Function GetChart(ByVal cp As CPBaseClass, ByVal subjectCnt As Integer, ByVal SubjectCaptions() As String, ByVal subjectScores() As Integer) As String
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
                returnHtml &= vbCrLf & vbTab & vbTab & vbTab & "chart.draw(data, {legend:'none',showCategories:false, min:0, max:100, width: 400, height: 240, is3D: true, title: 'Quiz Results By Subject'});"
                returnHtml &= vbCrLf & vbTab & vbTab & "}"
                returnHtml &= vbCrLf & vbTab & "</script>"
                returnHtml &= vbCrLf & vbTab & "<div id=""chart_div""></div>"
                '
                returnHtml = "" _
                    & vbCrLf & vbTab & "<div class=""quizChart"">" _
                    & cp.Html.Indent(returnHtml) _
                    & vbCrLf & vbTab & "<p>Click on the column to see your score</p>" _
                    & vbCrLf & vbTab & "</div>"

            Catch ex As Exception
                Call errorReport(cp, ex, "verifyQuizResponse")
            End Try
            Return returnHtml
        End Function
        '
        '
        '
        Private Sub getSubjectGradeCaption(ByVal cp As CPBaseClass, ByVal SubjectID As Integer, ByVal ScorePercentile As Double, ByRef Return_SubjectCaption As String, ByRef Return_GradeCaption As String)
            Try
                Dim cs As CPCSBaseClass = cp.CSNew()
                Dim Score As Integer
                '
                Return_GradeCaption = ""
                Return_SubjectCaption = ""
                Call cs.Open("Quiz Subjects", "id=" & SubjectID)
                If cs.OK() Then
                    Score = CLng(Int(100 * ScorePercentile))
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
                Call errorReport(cp, ex, "getSubjectGradeCaption")
            End Try
        End Sub
        '
        Private Sub getQuizGradeCaption(ByVal cp As CPBaseClass, ByVal quiz As DistanceLearning.Models.QuizModel, ByVal ScorePercentile As Double, ByRef Return_GradeCaption As String, ByRef Return_PassingGrade As Boolean)
            Try
                Return_GradeCaption = ""
                Dim Score As Integer = CLng(Int(100 * ScorePercentile))
                If Score >= quiz.APercentile Then
                    '
                    ' Got an A
                    Return_GradeCaption = quiz.ACaption
                    Return_PassingGrade = quiz.APassingGrade
                ElseIf Score >= quiz.bPercentile Then
                    '
                    ' Got a B
                    '
                    Return_GradeCaption = quiz.BCaption
                    Return_PassingGrade = quiz.BPassingGrade
                ElseIf Score >= quiz.cPercentile Then
                    '
                    ' Got a C
                    '
                    Return_GradeCaption = quiz.CCaption
                    Return_PassingGrade = quiz.CPassingGrade
                ElseIf Score >= quiz.DPercentile Then
                    '
                    ' Got a D
                    '
                    Return_GradeCaption = quiz.DCaption
                    Return_PassingGrade = quiz.DPassingGrade
                Else
                    '
                    ' Got an F
                    '
                    Return_GradeCaption = quiz.FCaption
                    Return_PassingGrade = quiz.FPassingGrade
                End If
            Catch ex As Exception
                Call errorReport(cp, ex, "getQuizGradeCaption")
            End Try
        End Sub
        '
        '
        '
        Private Function getResponseAnswerId(ByVal cp As CPBaseClass, ByVal responseId As Integer, ByVal questionId As Integer) As Integer
            Dim returnInt As Integer = 0
            Try
                Dim cs As CPCSBaseClass = cp.CSNew()
                '
                getResponseAnswerId = 0
                Call cs.Open("quiz response details", "(responseid=" & responseId & ")and(questionid=" & questionId & ")")
                If cs.OK() Then
                    getResponseAnswerId = cs.GetInteger("answerId")
                End If
                Call cs.Close()

            Catch ex As Exception
                Call errorReport(cp, ex, "verifyQuizResponse")
            End Try
        End Function
        ''
        ''
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
        '            Call cs.SetField("memberid", userId)
        '            Call cs.SetField("quizid", quizId)
        '            Call cs.SetField("attemptNumber", attemptNumber)
        '        End If
        '        Call cs.Close()

        '    Catch ex As Exception
        '        Call errorReport(cp, ex, "verifyQuizResponse")
        '    End Try
        'End Sub
    End Class
End Namespace
