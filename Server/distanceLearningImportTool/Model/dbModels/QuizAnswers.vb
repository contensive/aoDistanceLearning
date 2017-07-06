Imports Contensive.BaseClasses

Namespace Model.dbModels
    '
    Public Class QuizAnswers
        Inherits BasicRecord
        '
        '
        '
        Public Shared Function createRecord(ByVal CP As CPBaseClass, value As String, questionId As Integer, correct As Boolean) as Integer
            Dim recordId as Integer = 0
            Try
                '
                Dim cs As CPCSBaseClass = CP.CSNew()
                If Not String.IsNullOrEmpty(value) Then
                    If cs.Insert(cnQuizAnswers) Then
                        '
                        recordId = cs.GetInteger("id")
                        Call cs.SetField("name",value)
                        Call cs.SetField("copy",value)
                        Call cs.SetField("questionId",questionId)
                        Call cs.SetField("correct",correct)
                        '
                    End If
                    Call cs.Close()
                End If
                '
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in Quizzes.createRecord")
            End Try
            Return recordId
        End Function
        '
        '
        '
    End Class
    '
End Namespace
