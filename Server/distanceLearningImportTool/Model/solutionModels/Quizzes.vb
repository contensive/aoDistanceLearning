Imports System.IO
Imports Contensive.BaseClasses

Namespace Model.solutionModels
    '
    Public Class Quizzes
        '
        Public value As String
        Public subjects As New List(Of QuizSubjects)
        '
        '
        '
        Public Shared Function GetQuizzesObject(ByVal CP As CPBaseClass, csv_file_path As String, csvObjectlist as List(Of csvFileRow), ByRef errorList As List(Of Model.architectureModels.errorClass)) As Quizzes
            Dim oQuizzes As New Quizzes
            Try
                dim fileName as String  = Path.GetFileName(csv_file_path)
                dim extension as String = Path.getextension(fileName)
                oQuizzes.value = fileName.replace(extension,"").Trim()
                oQuizzes.subjects = Model.solutionModels.QuizSubjects.LoadCsvObject(Cp, csvObjectlist)

            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Quizzes.GetQuizzesObject")
            End Try
            Return oQuizzes
        End Function
        '
        '
        '
        Public Shared Function CreateDBRecords(ByVal CP As CPBaseClass,Quiz As Quizzes) As Boolean
            Dim result As Boolean = False
            Try

                Dim QuizId As Integer = 0
                Dim QuizSubjectId As Integer = 0
                Dim QuizQuestionId As Integer = 0
                Dim QuizAnswerId As Integer = 0

                '
                ' Create Quiz Record
                '
                QuizId = Model.dbModels.Quizzes.createRecord(CP, Quiz.value)

                For Each Subject In Quiz.subjects
                    '
                    ' Create Quiz Subjects Records
                    '
                    QuizSubjectId = Model.dbModels.QuizSubjects.createRecord(CP, Subject.value, QuizId)
                    '
                    For Each Question In Subject.questions
                        '
                        ' Create Quiz Questions Record
                        '
                        QuizQuestionId = Model.dbModels.QuizQuestions.createRecord(CP, Question.value, QuizId, QuizSubjectId)
                        '
                        For Each Answer In Question.answers
                            '
                            ' Create Quiz Answers Record
                            '
                            QuizAnswerId = Model.dbModels.QuizAnswers.createRecord(CP, Answer.Value, QuizQuestionId, Answer.correctAnswer)
                            '
                        Next
                        '
                    Next
                    '
                Next
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Quizzes.CreateDBRecords")
            End Try
            Return result
        End Function
        '
        '
        '
    End Class
    '
End Namespace
