


Imports System
    Imports System.Collections.Generic
    Imports System.Text
    Imports Contensive.BaseClasses

Namespace Contensive.Addons.OnlineQuiz
    Public Class QuizLandingPageView
        Public Function getView(ByVal CP As CPBaseClass) As String
            Dim returnHtml As String = ""
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                Dim cs2 As CPCSBaseClass = CP.CSNew()
                Dim layout As CPBlockBaseClass = CP.BlockNew
                Dim blockLayout As CPBlockBaseClass = CP.BlockNew
                Dim tmpHtml As String = ""
                Dim sqlCriteria As String
                Dim quizId As Integer
                Dim quizVideo As String = ""
                ' Dim quizTypeId As Integer
                Dim quizSelected As Boolean
                Dim userId As Integer
                Dim quizName As String = ""
                Dim customTopCopy As String = ""
                Dim customButtonCopy As String = ""
                Dim qs As String = ""
                ''

                Call CP.User.Track()
                userId = CP.User.Id
                quizId = 0
                quizSelected = False
                sqlCriteria = ""
                returnHtml = ""
                '
                quizId = CP.Doc.GetInteger("Quiz")
                If cs.Open("quizzes", "id=" & quizId) Then
                    quizVideo = cs.GetText("Video")
                    customTopCopy = cs.GetText("customTopCopy")
                    customButtonCopy = cs.GetText("customButtonCopy")
                    quizName = cs.GetText("name")
                End If
                cs.Close()
                Dim Button As String = CP.Doc.GetText("Start")
                If Button = customButtonCopy Then
                    qs = CP.Doc.RefreshQueryString
                    qs = CP.Utils.ModifyQueryString(qs, "AddonGuid", onlineQuizAddonGuid)
                    qs = CP.Utils.ModifyQueryString(qs, "quizId", quizId)
                    CP.Response.Redirect("?" + qs)
                End If

                layout.OpenLayout("Quiz Landing Page")
                blockLayout.Load(layout.GetOuter("#js-quizland"))
                blockLayout.SetInner("#js-quizTittle", "<h2>" & quizName & "<h2>")
                blockLayout.SetOuter("#js-quizCustomText", "<p>" & customTopCopy & "</p>")
                blockLayout.SetOuter("#js-quizVideo", "<div id=""js-quizVideo""><video width=""320"" height=""240"" controls><source src=""movie.mp4"" type=""video/mp4""></video></div>")
                blockLayout.SetOuter("#js-quizStartButton", CP.Html.Form(CP.Html.Button("Start", customButtonCopy) + CP.Html.Hidden("quizId", quizId), "startbuttonform"))

                'do
                ' read a table
                ' end do

                layout.SetInner(".quizlanding", tmpHtml)

                returnHtml = blockLayout.GetHtml


            Catch ex As Exception
                errorReport(CP, ex, "execute")
            End Try
            Return returnHtml
        End Function
        '
        Private Sub errorReport(ByVal cp As CPBaseClass, ByVal ex As Exception, ByVal method As String)
            Try
                cp.Site.ErrorReport(ex, "Unexpected error in quizClass." & method)
            Catch exLost As Exception
                '
                ' stop anything thrown from cp errorReport
                '
            End Try
        End Sub
    End Class
End Namespace

