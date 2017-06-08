
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
                response = DistanceLearning.Models.QuizResponseModel.create(cp, quiz.id, cp.User.Id)
                If (response Is Nothing) Then
                    '
                    ' -- add a new response, and create all the response details (with no answer selected)
                    response = DistanceLearning.Models.QuizResponseModel.add(cp, quiz.id)
                    Dim quizQuestionList As List(Of DistanceLearning.Models.QuizQuestionModel) = DistanceLearning.Models.QuizQuestionModel.getQuestionsForQuizList(cp, quiz.id)
                    For Each quizQuestion As DistanceLearning.Models.QuizQuestionModel In quizQuestionList
                        Dim detail As DistanceLearning.Models.QuizResponseDetailModel = DistanceLearning.Models.QuizResponseDetailModel.add(cp)
                        detail.questionId = quizQuestion.id
                        detail.responseId = response.id
                        detail.saveObject(cp)
                    Next
                End If
            Catch ex As Exception
                cp.Site.ErrorReport(ex)
            End Try
            Return response
        End Function
    End Class
End Namespace
