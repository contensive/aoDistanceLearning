
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
                        If quizQuestion.SubjectID = 0 Then
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
    End Class
End Namespace
