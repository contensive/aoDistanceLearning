
Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Contensive.BaseClasses

Namespace Contensive.Addons.OnlineQuiz
    Public Class genericController
        '
        Public Shared Function createNewQuizResponse(cp As CPBaseClass, quiz As DistanceLearning.Models.QuizModel) As DistanceLearning.Models.QuizResponseModel
            Dim response As DistanceLearning.Models.QuizResponseModel = Nothing
            Try
                response = DistanceLearning.Models.QuizResponseModel.createUncompletedObject(cp, quiz.id, cp.User.Id)
                If (response Is Nothing) Then
                    '
                    ' -- clear FK
                    cp.Db.ExecuteSQL("update quizquestions set subjectid=null where id in (select q.id from quizquestions q left join quizsubjects s on s.id=q.subjectid where s.id is null)")
                    '
                    ' -- add a new response, and create all the response details (with no answer selected)
                    response = DistanceLearning.Models.QuizResponseModel.add(cp, quiz.id)
                    response.name = cp.User.Name & ", " & Now.ToShortDateString() & ", " & quiz.name
                    Dim quizSubjectList As List(Of DistanceLearning.Models.QuizSubjectModel) = DistanceLearning.Models.QuizSubjectModel.getObjectList(cp, quiz.id)
                    Dim quizQuestionList As List(Of DistanceLearning.Models.QuizQuestionModel) = DistanceLearning.Models.QuizQuestionModel.getQuestionsForQuizList(cp, quiz.id)
                    '
                    ' -- pages start with questions with subjects
                    Dim pageNumber As Integer = 1
                    For Each quizSubject As DistanceLearning.Models.QuizSubjectModel In quizSubjectList
                        For Each quizQuestion As DistanceLearning.Models.QuizQuestionModel In quizQuestionList
                            If quizQuestion.SubjectID = quizSubject.id Then
                                Dim detail As DistanceLearning.Models.QuizResponseDetailModel = DistanceLearning.Models.QuizResponseDetailModel.add(cp)
                                detail.questionId = quizQuestion.id
                                detail.responseId = response.id
                                detail.pageNumber = pageNumber
                                detail.SortOrder = quizQuestion.SortOrder
                                detail.saveObject(cp)
                                If (quiz.questionPresentation = 1) Then
                                    pageNumber += 1
                                End If
                            End If
                        Next
                        If (quiz.questionPresentation = 3) Then
                            pageNumber += 1
                        End If
                    Next
                    '
                    ' -- then add questions with no subjects
                    For Each quizQuestion As DistanceLearning.Models.QuizQuestionModel In quizQuestionList
                        Dim addDetail As Boolean = True
                        If quizQuestion.SubjectID > 0 Then
                            Dim subjectFound As Boolean = False
                            For Each quizSubject As DistanceLearning.Models.QuizSubjectModel In quizSubjectList
                                subjectFound = Equals(quizQuestion.SubjectID, quizSubject.id)
                                If (subjectFound) Then Exit For
                            Next
                            addDetail = Not subjectFound
                        End If
                        If addDetail Then
                            Dim detail As DistanceLearning.Models.QuizResponseDetailModel = DistanceLearning.Models.QuizResponseDetailModel.add(cp)
                            detail.questionId = quizQuestion.id
                            detail.responseId = response.id
                            detail.pageNumber = pageNumber
                            detail.saveObject(cp)
                            If (quiz.questionPresentation = 1) Then
                                pageNumber += 1
                            End If
                        End If
                    Next
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return response
        End Function
        '
        Friend Function encodeMinDate(ByVal sourceDate As Date) As Date
            Dim returnValue As Date = sourceDate
            If returnValue < #1/1/1900# Then
                returnValue = Date.MinValue
            End If
            Return returnValue
        End Function
        '
        '
        '
        Friend Function encodeShortDateString(ByVal sourceDate As Date) As String
            Dim returnValue As String
            '
            If sourceDate < #1/1/1900# Then
                returnValue = ""
            Else
                returnValue = sourceDate.ToShortDateString
            End If
            Return returnValue

        End Function
        '
        '
        '
        Friend Function encodeBlankCurrency(ByVal source As Double) As String
            Dim returnValue As String = ""
            If source <> 0 Then
                returnValue = FormatCurrency(source, 2)
            End If
            Return returnValue
        End Function
        '
        '
        '
        Friend Sub appendLog(ByVal cp As CPBaseClass, ByVal logMessage As String)
            Dim nowDate As Date = Date.Now.Date()
            Dim logFilename As String = nowDate.Year & nowDate.Month.ToString("D2") & nowDate.Day.ToString("D2") & ".log"
            Call cp.File.CreateFolder(cp.Site.PhysicalInstallPath & "\logs\" & traceLogPath)
            Call cp.Utils.AppendLog(traceLogPath & "\" & logFilename, logMessage)
        End Sub
        '
        '
        '
        Private Sub localErrorReport(ByVal cp As CPBaseClass, ByVal ex As Exception, ByVal method As String)
            Call cp.Site.ErrorReport(ex, "error in aoAccountBilling.commonModule." & method)
        End Sub
        '
        '
        '
        Public Function getSortOrderFromInteger(id As Integer) As String
            Return id.ToString().PadLeft(7, "0"c)
        End Function
    End Class
End Namespace
