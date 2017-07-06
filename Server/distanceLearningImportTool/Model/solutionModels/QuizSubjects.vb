Imports Contensive.BaseClasses

Namespace Model.solutionModels
    '
    Public Class QuizSubjects
        '
        Public value As String
        Public questions As New List(Of QuizQuestions)
        '
        '
        '
        Public Shared Function LoadCsvObject(ByVal CP As CPBaseClass, CSVRowListObject as List(Of csvFileRow)) As List(Of QuizSubjects)
            Dim oQuizSubjectsList As New List(Of QuizSubjects)
            Try
                '
                Dim isNewSubject as Boolean = False
                For Each csvRowObject In CSVRowListObject
                    '
                    isNewSubject = False
                    Dim Subject = oQuizSubjectsList.FirstOrDefault(Function(myObject) myObject.value = csvRowObject.Subject)
                    If (Subject is Nothing) Then
                        ' Subject not exit
                        Subject = New QuizSubjects
                        Subject.value = csvRowObject.Subject
                        isNewSubject = True
                    End If
                    '
                    Subject.questions.add(Model.solutionModels.QuizQuestions.LoadQuestion(CP, csvRowObject))
                    '
                    If isNewSubject Then
                        oQuizSubjectsList.Add(Subject)
                    End If
                    '
                Next
                '
            Catch ex As Exception

            End Try
            Return oQuizSubjectsList
        End Function
        '
        '
        '
    End Class
    '
End Namespace
