Imports Contensive.BaseClasses

Namespace Interfaces.Remotes
    '
    Public Class ProcessUrl
        Inherits AddonBaseClass
        '
        '
        '
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnMessage As String = ""
            Try
                Dim email As String = CP.Doc.GetText("invite")
                Dim allowLinkLogin As Boolean = CP.Site.GetBoolean("AllowLinkLogin")
                If Not String.IsNullOrEmpty(email) Then
                    If CP.User.IsAuthenticated Then




                        'check if this user's email is the same as the link's
                        If String.Compare(email, CP.User.Email) = 0 Then

                        End If







                    ElseIf Not allowLinkLogin Then
                        returnMessage = "Email Invitations are disabled"
                        Return returnMessage
                    Else

                        Dim emailText As String = ""

                        Dim fromAddr As String = CP.Site.GetText("APPA From Email Address")
                        Dim url As String = CP.Request.

                        emailText = "" + url

                        Dim cs As CPCSBaseClass = CP.CSNew()
                        If (cs.Open("People", "email=" + CP.Db.EncodeSQLText(email))) Then
                            'get their login eid qs



                        Else
                            If cs.Insert("People") Then
                                cs.SetField("name", email)
                                cs.SetField("email", email)
                                cs.Save()
                            End If

                            'get their login qs

                        End If

                        Dim emailError As String = ""
                        CP.Email.send(email, fromAddr, "APPA Quiz Link", emailText, True, True, emailError)

                        'show blocking message/message
                        returnMessage = ""
                        Return returnMessage


                    End If
                End If

            Catch ex As Exception
                CP.Site.ErrorReport(ex)
                returnMessage = "An error occurred"
                Return returnMessage
            End Try
        End Function
    End Class
    '
End Namespace
