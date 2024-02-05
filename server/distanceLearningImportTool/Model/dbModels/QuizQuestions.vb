Imports Contensive.BaseClasses

Namespace Model.dbModels
    '
    Public Class QuizQuestions
        Inherits BasicRecord
        '
        '
        '
        Public Shared Function createRecord(ByVal CP As CPBaseClass, value As String, quizId As Integer, subjectId As Integer) as Integer
            Dim recordId as Integer = 0
            Try
                '
                Dim cs As CPCSBaseClass = CP.CSNew()
                If Not String.IsNullOrEmpty(value) Then
                    If cs.Insert(cnQuizQuestions) Then
                        '
                        recordId = cs.GetInteger("id")
                        Call cs.SetField("name",value)
                        Call cs.SetField("copy",value)
                        Call cs.SetField("quizId",quizId)
                        '
                        If subjectId<>0 Then
                            Call cs.SetField("subjectId",subjectId)
                        End If
                        '
                        Call cs.SetField("SortOrder",recordId.ToString("D10"))
                        '
                    End If
                    Call cs.Close()
                End If
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Quizzes.createRecord")
            End Try
            Return recordId
        End Function
        '
        '
        '
    End Class
    '
End Namespace
