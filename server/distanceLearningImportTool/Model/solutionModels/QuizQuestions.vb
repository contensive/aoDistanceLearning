Imports Contensive.BaseClasses

Namespace Model.solutionModels
    '
    Public Class QuizQuestions
        '
        Public value As String
        Public answerPos As Integer
        Public answerValue As String
        '
        Public answers As New List(Of QuizAnswers)
        '
        Public Shared function LoadQuestion(ByVal CP As CPBaseClass, csvRowObject as csvFileRow) as QuizQuestions
            dim Question as New QuizQuestions
            Try
                '
                ' Add the question
                '
                Question.value = csvRowObject.Question
                Question.answerValue = csvRowObject.Answer

                Select Case csvRowObject.Answer.toupper()
                    Case "A"
                        Question.answerPos = 1
                    Case "B"
                        Question.answerPos = 2
                    Case "C"
                        Question.answerPos = 3
                    Case "D"
                        Question.answerPos = 4
                    Case "E"
                        Question.answerPos = 5
                    Case "F"
                        Question.answerPos = 6
                    Case "G"
                        Question.answerPos = 7
                End select

                '
                ' Add the answers
                '
                for i=1 to csvRowObject.OptionCount
                    Dim answer as New QuizAnswers
                    answer.pos = i
                    If answer.pos = Question.answerPos Then
                        answer.correctAnswer = True
                    End If
                    '
                    Select case i
                        Case 1
                            answer.value = csvRowObject.OptionA
                        Case 2
                            answer.value = csvRowObject.OptionB
                        Case 3
                            answer.value = csvRowObject.OptionC
                        Case 4
                            answer.value = csvRowObject.OptionD
                        Case 5
                            answer.value = csvRowObject.OptionE
                        Case 6
                            answer.value = csvRowObject.OptionF
                        Case 7
                            answer.value = csvRowObject.OptionG
                    End Select
                    '
                    Question.answers.add(answer)
                    '
                Next

                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in QuizQuestions.LoadQuestion")
            End Try
            Return Question
        End function
        '
    End Class
    '
End Namespace
