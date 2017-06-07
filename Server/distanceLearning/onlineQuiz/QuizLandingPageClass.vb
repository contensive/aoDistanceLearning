


Imports System
    Imports System.Collections.Generic
    Imports System.Text
    Imports Contensive.BaseClasses

Namespace Contensive.Addons.OnlineQuiz
    '
    ' Sample Vb2005 addon
    '
    Public Class QuizLandingPageClass
        Inherits Contensive.BaseClasses.AddonBaseClass
        '
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = ""
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                Dim cs2 As CPCSBaseClass = CP.CSNew()
                Dim layout As CPBlockBaseClass = CP.BlockNew
                Dim blockLayout As CPBlockBaseClass = CP.BlockNew
                Dim tmpHtml As String = ""
                ''
                Dim sql As String = ""

                ' Dim certificationId As Integer = CP.Doc.GetInteger("certificationId")
                blockLayout.OpenLayout("Quiz Landing Page")
                '

                blockLayout.Load(layout.GetOuter("#js-qland"))
                blockLayout.SetInner("#js-startestButton", CP.Html.Form("startbuttonform") & CP.Html.Button("Start", "Start The Test"))

                tmpHtml = tmpHtml.Replace("#js-startestButton", CP.Html.Form("startbuttonform") & CP.Html.Button("Start", "Start The Test"))

                '
                tmpHtml &= blockLayout.GetHtml





                Call cs.Close()
                'do
                ' read a table
                ' end do

                layout.SetInner(".quizlanding", tmpHtml)

                returnHtml = layout.GetHtml


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

