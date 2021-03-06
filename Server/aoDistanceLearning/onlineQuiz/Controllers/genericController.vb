﻿
'Option Strict On
'Option Explicit On

'Imports System
'Imports System.Collections.Generic
'Imports System.Text
'Imports Contensive.BaseClasses

'Namespace Contensive.Addons.DistanceLearning
'    Public Class genericController
'        '
'        Public Shared Function createNewQuizResponse(cp As CPBaseClass, quiz As DistanceLearning.Models.QuizModel) As DistanceLearning.Models.QuizResponseModel
'            Dim response As DistanceLearning.Models.QuizResponseModel = Nothing
'            Try
'                Dim previousResponses As List(Of DistanceLearning.Models.QuizResponseModel) = DistanceLearning.Models.QuizResponseModel.GetResponseList(cp, quiz.id, cp.User.Id)
'                '
'                ' -- all previous responses must be complete
'                If previousResponses.Count > 0 Then
'                    For Each response In previousResponses
'                        If (Not DistanceLearning.Controllers.genericController.isDateEmpty(response.dateSubmitted)) Then
'                            response.dateSubmitted = Now
'                            response.saveObject(cp)
'                        End If
'                    Next
'                End If
'                '
'                ' -- housekeep subject FK
'                cp.Db.ExecuteSQL("update quizquestions set subjectid=null where id in (select q.id from quizquestions q left join quizsubjects s on s.id=q.subjectid where s.id is null)")
'                '
'                ' -- add a new response, and create all the response details (with no answer selected)
'                response = DistanceLearning.Models.QuizResponseModel.add(cp, quiz.id)
'                response.name = cp.User.Name & ", " & Now.ToShortDateString() & ", " & quiz.name
'                response.MemberID = cp.User.Id
'                response.attemptNumber = previousResponses.Count + 1
'                response.saveObject(cp)
'                '
'                Dim quizSubjectList As List(Of DistanceLearning.Models.QuizSubjectModel) = DistanceLearning.Models.QuizSubjectModel.getObjectList(cp, quiz.id)
'                Dim quizQuestionList As List(Of DistanceLearning.Models.QuizQuestionModel) = New List(Of DistanceLearning.Models.QuizQuestionModel)
'                Dim quizQuestionFullList As List(Of DistanceLearning.Models.QuizQuestionModel) = DistanceLearning.Models.QuizQuestionModel.getQuestionsForQuizList(cp, quiz.id)
'                If (quiz.maxNumberQuest = 0) Or (quiz.maxNumberQuest >= quizQuestionFullList.Count) Then
'                    '
'                    ' -- include all questions that have subjects, ordered by subject order
'                    For Each subject As DistanceLearning.Models.QuizSubjectModel In quizSubjectList
'                        For Each quizQuestion As DistanceLearning.Models.QuizQuestionModel In quizQuestionFullList
'                            If (quizQuestion.SubjectID = subject.id) Then
'                                quizQuestionList.Add(quizQuestion)
'                            End If
'                        Next
'                    Next
'                    '
'                    ' -- include all questions with no subjects
'                    For Each quizQuestion As DistanceLearning.Models.QuizQuestionModel In quizQuestionFullList
'                        If (quizQuestion.SubjectID = 0) Then
'                            quizQuestionList.Add(quizQuestion)
'                        End If
'                    Next
'                Else
'                    '
'                    ' -- include a random set of questions
'                    Randomize()
'                    If (quizSubjectList.Count > 0) Then
'                        '
'                        ' -- subjects included, add maxNumberQuest to each subject
'                        For Each subject As DistanceLearning.Models.QuizSubjectModel In quizSubjectList
'                            '
'                            ' -- create list of all questions in the subject
'                            Dim subjectQuestionList As List(Of DistanceLearning.Models.QuizQuestionModel) = New List(Of DistanceLearning.Models.QuizQuestionModel)
'                            For Each question As DistanceLearning.Models.QuizQuestionModel In quizQuestionFullList
'                                If question.SubjectID = subject.id Then
'                                    subjectQuestionList.Add(question)
'                                End If
'                            Next
'                            '
'                            ' -- add random maxNumberQuest to quizquestionList
'                            If (subjectQuestionList.Count > 0) Then
'                                For questionPtr As Integer = 0 To quiz.maxNumberQuest - 1
'                                    If (subjectQuestionList.Count = 0) Then
'                                        '
'                                        ' -- no more questions in this subject
'                                        Exit For
'                                    Else
'                                        Dim indexTest As Integer = CInt(Int(Rnd() * (subjectQuestionList.Count)))
'                                        Dim question As DistanceLearning.Models.QuizQuestionModel = subjectQuestionList(indexTest)
'                                        quizQuestionList.Add(question)
'                                        subjectQuestionList.Remove(question)
'                                    End If
'                                Next
'                            End If
'                        Next
'                    Else
'                        '
'                        ' -- no subjects, add maxNumberQuest questions to quiz
'                        For questionPtr As Integer = 0 To quiz.maxNumberQuest - 1
'                            Dim indexTest As Integer = CInt(Int(Rnd() * (quizQuestionFullList.Count)))
'                            Dim question As DistanceLearning.Models.QuizQuestionModel = quizQuestionFullList(indexTest)
'                            quizQuestionList.Add(question)
'                            quizQuestionFullList.Remove(question)
'                        Next
'                    End If
'                End If
'                '
'                ' -- first create quiz pages that have subjects
'                Dim pageNumber As Integer = 1
'                Dim detailSortOrder As Integer = 0
'                For Each quizSubject As DistanceLearning.Models.QuizSubjectModel In quizSubjectList
'                    Dim subjectQuestionCount As Integer = 0
'                    For Each quizQuestion As DistanceLearning.Models.QuizQuestionModel In quizQuestionList
'                        If quizQuestion.SubjectID = quizSubject.id Then
'                            Dim detail As DistanceLearning.Models.QuizResponseDetailModel = DistanceLearning.Models.QuizResponseDetailModel.add(cp)
'                            detail.questionId = quizQuestion.id
'                            detail.responseId = response.id
'                            detail.pageNumber = pageNumber
'                            detail.SortOrder = quizSubject.name + quizQuestion.SortOrder '  detailSortOrder.ToString.PadLeft(7, "0"c)
'                            detail.saveObject(cp)
'                            If (quiz.questionPresentation = DistanceLearning.Models.QuizModel.questionPresentationEnum.OneQuestionPerPage) Then
'                                pageNumber += 1
'                            End If
'                            subjectQuestionCount += 1
'                            detailSortOrder += 1
'                        End If
'                    Next
'                    If (subjectQuestionCount > 0) And (quiz.questionPresentation = DistanceLearning.Models.QuizModel.questionPresentationEnum.OneSubjectPerPage) Then
'                        pageNumber += 1
'                    End If
'                Next
'                '
'                ' -- then add pages for questions with no subjects
'                For Each quizQuestion As DistanceLearning.Models.QuizQuestionModel In quizQuestionList
'                    Dim addDetail As Boolean = True
'                    If quizQuestion.SubjectID > 0 Then
'                        Dim subjectFound As Boolean = False
'                        For Each quizSubject As DistanceLearning.Models.QuizSubjectModel In quizSubjectList
'                            subjectFound = Equals(quizQuestion.SubjectID, quizSubject.id)
'                            If (subjectFound) Then Exit For
'                        Next
'                        addDetail = Not subjectFound
'                    End If
'                    If addDetail Then
'                        Dim detail As DistanceLearning.Models.QuizResponseDetailModel = DistanceLearning.Models.QuizResponseDetailModel.add(cp)
'                        detail.questionId = quizQuestion.id
'                        detail.responseId = response.id
'                        detail.pageNumber = pageNumber
'                        detail.SortOrder = detailSortOrder.ToString.PadLeft(7, "0"c)
'                        detail.saveObject(cp)
'                        If (quiz.questionPresentation = DistanceLearning.Models.QuizModel.questionPresentationEnum.OneQuestionPerPage) Then
'                            pageNumber += 1
'                        End If
'                        detailSortOrder += 1
'                    End If
'                Next
'            Catch ex As Exception
'                cp.Site.ErrorReport(ex)
'            End Try
'            Return response
'        End Function
'        '
'        Friend Function encodeMinDate(ByVal sourceDate As Date) As Date
'            Dim returnValue As Date = sourceDate
'            If returnValue < #1/1/1900# Then
'                returnValue = Date.MinValue
'            End If
'            Return returnValue
'        End Function
'        '
'        '
'        '
'        Friend Function encodeShortDateString(ByVal sourceDate As Date) As String
'            Dim returnValue As String
'            '
'            If sourceDate < #1/1/1900# Then
'                returnValue = ""
'            Else
'                returnValue = sourceDate.ToShortDateString
'            End If
'            Return returnValue

'        End Function
'        '
'        '
'        '
'        Friend Function encodeBlankCurrency(ByVal source As Double) As String
'            Dim returnValue As String = ""
'            If source <> 0 Then
'                returnValue = FormatCurrency(source, 2)
'            End If
'            Return returnValue
'        End Function
'        '
'        '
'        '
'        Public Function getSortOrderFromInteger(id As Integer) As String
'            Return id.ToString().PadLeft(7, "0"c)
'        End Function
'    End Class
'End Namespace
