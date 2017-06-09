
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
                Dim chart As String
                Dim responseSubject As String
                Dim responseName As String
                Dim scoreCaption As String
                Dim Choice As String
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
                Dim SummaryBox As String = ""
                Dim q As String
                Dim SelectedAnswerID As Integer
                Dim Correct As Boolean
                Dim NumIncorrect As Integer
                Dim NumCorrect As Integer
                Dim Percentage As Double
                Dim Passed As Boolean
                Dim SubjectID As Integer
                Dim subjects() As subjectsStruc = Nothing
                Dim questionId As Integer
                Dim subjectCnt As Integer = 0
                Dim subjectPtr As Integer
                Dim GradeCaption As String = ""
                'Dim OverallCaption As String
                Dim SubjectCaptions() As String = Nothing
                Dim subjectScores() As Integer = Nothing
                Dim headerCopy As String
                Dim footerCopy As String
                Dim quizName As String = ""
                Dim rightNow As Date
                Dim userId As Integer
                Dim quizId As Integer
                Dim answerPoints As Integer
                'Dim resultPoints As Integer = 0
                '
                rightNow = Now()
                '
                ' only the responseId is valid, lookup the quiz and userId
                '
                Call cs.Open("quiz responses", "id=" & responseId)
                If cs.OK() Then
                    quizId = cs.GetInteger("quizId")
                    userId = cs.GetInteger("memberId")
                    TotalQuestions = cs.GetInteger("TotalQuestions")
                    TotalCorrect = cs.GetInteger("TotalCorrect")
                End If
                Call cs.Close()
                '
                'Call verifyQuizResponse(responseId, userId, quizId)
                '
                hint = 10
                Call cs.Open("Quizzes", "id=" & quizId)
                '
                If cs.OK() Then
                    '
                    quizName = cs.GetText("name")
                    Call cs2.Open("Quiz Questions", "QuizID=" & CStr(quizId), "QOrder")
                    '
                    ' subjectPtr=0 is the 'no subject' subject
                    '
                    subjectCnt = 0
                    'ReDim Preserve subjectTally(subjectCnt)
                    '
                    Counter = 0
                    NumIncorrect = 0
                    NumCorrect = 0
                    Do While cs2.OK()
                        hint = 20
                        Counter = Counter + 1
                        SubjectID = cs2.GetInteger("SubjectID")
                        questionId = cs2.GetInteger("ID")
                        SelectedAnswerID = getResponseAnswerId(cp, responseId, questionId)
                        '
                        ' Set subjectPtr to the Response Array for this question's subject
                        '
                        subjectPtr = 0
                        If SubjectID > 0 Then
                            Do While (subjectPtr < subjectCnt)
                                If (subjects(subjectPtr).SubjectID = SubjectID) Then
                                    Exit Do
                                End If
                                subjectPtr = subjectPtr + 1
                            Loop
                            If subjectPtr >= subjectCnt Then
                                ReDim Preserve subjects(subjectPtr)
                                subjects(subjectPtr).SubjectID = SubjectID
                                subjectCnt = subjectCnt + 1
                            End If
                            subjects(subjectPtr).TotalQuestions = subjects(subjectPtr).TotalQuestions + 1
                        End If
                        '
                        '
                        ' New Question
                        '
                        hint = 30
                        q = ""
                        Call CS3.Open("Quiz Answers", "QuestionID=" & questionId, "sortOrder")
                        If Not CS3.OK() Then
                            '
                            ' Question with no choices is correct
                            '
                            Correct = True
                        Else
                            '
                            ' Add Choices
                            '
                            Correct = False
                            Do While CS3.OK()
                                '
                                ' cycle through all the answers and build display
                                '
                                answerId = CS3.GetInteger("ID")
                                AnswerCorrect = CS3.GetBoolean("Correct")
                                answerPoints = CS3.GetInteger("points")
                                Choice = ""
                                '
                                hint = 40
                                If answerId = SelectedAnswerID Then
                                    If (subjectCnt > subjectPtr) And (SubjectID > 0) Then
                                        subjects(subjectPtr).points += answerPoints
                                    End If
                                    Choice = Choice & "<input type=""radio"" name=""" & Counter & "Answer"" value=""" & CStr(CS3.GetInteger("ID")) & """ checked disabled>"
                                    Choice = Choice & "&nbsp;" & CS3.GetText("name")
                                    'Choice = Choice & "&nbsp;" & CS3.GetText("AText")
                                    If AnswerCorrect Then
                                        '
                                        ' selected answer is current answer
                                        '
                                        Correct = True
                                    End If
                                    q = q & vbCrLf & vbTab & "<div class=""questionChoice"">" & Choice & "</div>"
                                Else
                                    Choice = Choice & "<input type=""radio"" name=""" & Counter & "Answer"" value=""" & CStr(CS3.GetInteger("ID")) & """ disabled>"
                                    Choice = Choice & "&nbsp;" & CS3.GetText("name")
                                    'Choice = Choice & "&nbsp;" & CS3.GetText("AText")
                                    If AnswerCorrect Then
                                        '
                                        ' unselected answer is correct answer, make red
                                        '
                                        q = q & vbCrLf & vbTab & "<div class=""questionChoiceWrong"">" & Choice & "</div>"
                                    Else
                                        '
                                        ' unselected answer is not correct answer
                                        '
                                        q = q & vbCrLf & vbTab & "<div class=""questionChoice"">" & Choice & "</div>"
                                    End If
                                End If
                                '
                                Call CS3.GoNext()
                            Loop
                        End If
                        '
                        ' Add QuestionText to top
                        '
                        hint = 50
                        If Correct Then
                            q = "" _
                                & vbCrLf & vbTab & "<div class=""questionText"">" & cs2.GetText("QText") & "</div>" _
                                & q
                            NumCorrect = NumCorrect + 1
                            If (subjectCnt > subjectPtr) Then
                                If SubjectID > 0 Then
                                    subjects(subjectPtr).CorrectAnswers = subjects(subjectPtr).CorrectAnswers + 1
                                End If
                            End If
                        Else
                            q = "" _
                                & vbCrLf & vbTab & "<div class=""questionTextWrong"">" & cs2.GetText("QText") & "</div>" _
                                & q
                            Passed = False
                            NumIncorrect = NumIncorrect + 1
                        End If
                        '
                        ' Add Explination
                        '
                        hint = 50
                        If Correct Then
                            q = "" _
                                & vbCrLf & vbTab & "<div class=""ExpinationText"">" & cs2.GetText("instructions") & "</div>" _
                                & q
                            NumCorrect = NumCorrect + 1
                            If (subjectCnt > subjectPtr) Then
                                If SubjectID > 0 Then
                                    subjects(subjectPtr).CorrectAnswers = subjects(subjectPtr).CorrectAnswers + 1
                                End If
                            End If
                        Else
                            q = "" _
                                & vbCrLf & vbTab & "<div class=""ExpinationText"">" & cs2.GetText("instructions") & "</div>" _
                                & q
                            Passed = False
                            NumIncorrect = NumIncorrect + 1
                        End If
                        '
                        ' Question container
                        '
                        SummaryBox &= "" _
                            & vbCrLf & vbTab & "<div class=""question"">" _
                            & cp.Html.Indent(q) _
                            & vbCrLf & vbTab & "</div>"
                        Call cs2.GoNext()
                    Loop
                    '
                    hint = 60
                    If NumIncorrect = 0 Then
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
                        & vbCrLf & vbTab & "<input type=""hidden"" name=""quizID"" value=""" & CStr(quizId) & """>" _
                        & vbCrLf & vbTab & "<input type=""hidden"" name=""scoreCard"" value=""1"">" _
                        & vbCrLf & vbTab & "<input type=""hidden"" name=""qNumbers"" value=""" & CStr(Counter) & """>"
                    '
                    ' add summary wrapper
                    '
                    SummaryBox = "" _
                        & vbCrLf & vbTab & "<div class=""summary"">" _
                        & cp.Html.Indent(SummaryBox) _
                        & vbCrLf & vbTab & "</div>"
                    '
                    cs2.Close()
                    '
                End If
                '
                ' Build ScoreBox
                '
                hint = 70
                ScoreBox = ""
                'TotalCorrect = 0
                'TotalQuestions = 0
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
                                Call SetSubjectArgs(cp, .SubjectID, .Score, .SubjectCaption, GradeCaption)
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
                '
                Percentage = 0
                If TotalQuestions > 0 Then
                    Percentage = CDbl(TotalCorrect) / CDbl(TotalQuestions)
                    Call SetSubjectArgs(cp, 0, Percentage, "", GradeCaption)
                    scoreCaption = ""
                    If GradeCaption <> "" Then
                        scoreCaption = "Your grade for this quiz is <b>" & GradeCaption & "</b>, "
                    Else
                        scoreCaption = "Your total score for this quiz is " & CStr(Int(Percentage * 100)) & "%"
                    End If
                    scoreCaption = scoreCaption & " (" & CStr(TotalCorrect) & " correct out of " & CStr(TotalQuestions)
                    If TotalQuestions = 1 Then
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
                hint = 90
                Call verifyQuizResponse(cp, responseId, userId, quizId)
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
                                Call cs4.SetField("QuizResponseID", responseId)
                                Call cs4.SetField("QuizSubjectID", .SubjectID)
                                Call cs4.SetField("Score", .Score)
                            End If
                            cs4.Close()
                        End With
                    Next
                End If
                '
                headerCopy = "<div class=""aodlHeaderContainer"">" & cp.Content.GetCopy(quizName & " Summary Header Copy") & "</div>"
                footerCopy = "<div class=""aodlFooterContainer"">" & cp.Content.GetCopy(quizName & " Summary Footer Copy") & "</div>"
                '
                ' Build the final form
                '
                hint = 100
                chart = ""
                If subjectCnt > 1 Then
                    chart = GetChart(cp, subjectCnt, SubjectCaptions, subjectScores)
                End If
                returnHtml = headerCopy & ScoreBox & SummaryBox & chart & footerCopy
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
        Private Sub SetSubjectArgs(ByVal cp As CPBaseClass, ByVal SubjectID As Integer, ByVal ScorePercentile As Double, ByRef Return_SubjectCaption As String, ByRef Return_GradeCaption As String)
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
                Call errorReport(cp, ex, "verifyQuizResponse")
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
        '
        '
        '
        Private Sub verifyQuizResponse(ByVal cp As CPBaseClass, ByRef responseId As Integer, ByVal userId As Integer, ByVal quizId As Integer)
            Try
                '
                Dim cs As CPCSBaseClass = cp.CSNew()
                Dim userName As String
                Dim quizName As String
                Dim attemptNumber As Integer
                '
                ' verify response record
                '
                Call cs.Open("quiz responses", "id=" & responseId)
                If Not cs.OK() Then
                    Call cs.Close()
                    attemptNumber = 1
                    Call cs.OpenSQL("select count(*) as cnt from quizResponses where (memberid=" & userId & ")and(quizid=" & quizId & ")and(dateSubmitted is not null)")
                    If cs.OK() Then
                        attemptNumber = cs.GetInteger("cnt") + 1
                    End If
                    Call cs.Close()

                    userName = cp.Content.GetRecordName("people", userId)
                    quizName = cp.Content.GetRecordName("quizzes", quizId)
                    Call cs.Insert("quiz responses")
                    responseId = cs.GetInteger("id")
                    Call cs.SetField("name", userName & ", quiz:" & quizName)
                    Call cs.SetField("memberid", userId)
                    Call cs.SetField("quizid", quizId)
                    Call cs.SetField("attemptNumber", attemptNumber)
                End If
                Call cs.Close()

            Catch ex As Exception
                Call errorReport(cp, ex, "verifyQuizResponse")
            End Try
        End Sub
    End Class
End Namespace
