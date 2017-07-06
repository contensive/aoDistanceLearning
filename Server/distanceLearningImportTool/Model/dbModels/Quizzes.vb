Imports Contensive.BaseClasses

Namespace Model.dbModels
    '
    Public Class Quizzes
        Inherits BasicRecord
        '
        '
        '
        Public Shared Function createRecord(ByVal CP As CPBaseClass, value As String) as Integer
            Dim recordId as Integer = 0
            Try
                '
                Dim cs As CPCSBaseClass = CP.CSNew()
                If Not String.IsNullOrEmpty(value) Then
                    If cs.Insert(cnQuizzes) Then
                        '
                        recordId = cs.GetInteger("id")
                        Call cs.SetField("name",value)
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

