Imports Contensive.BaseClasses

Namespace Model.solutionModels
    '
    Public Class Login
        '
        Public username As String = ""
        Public password As String = ""
        ' 
        '
        '
        Public Shared Function ProcessLogin(ByVal CP As CPBaseClass, Login As Login, ByRef errorList As List(Of architectureModels.errorClass)) As Boolean
            Dim result As Boolean = False
            Dim errorStep As String = "00"
            Try
                '
                errorStep = "01"
                '

                If IsNothing(Login) Then
                    errorStep = "02"
                    ' send message error
                    Dim oneError As New architectureModels.errorClass
                    oneError.number = 600
                    oneError.userMsg = "No values found."
                    errorList.Add(oneError)

                Else 
                    ' check if username and password is not empty
                    If Not String.IsNullOrEmpty(Login.username) And
                       Not String.IsNullOrEmpty(Login.password) Then
                        ' check if valid username ans password combination
                        errorStep = "03"
                        If CP.User.Login(Login.username, Login.password) Then

                            result = True
                            ' *****************
                        Else
                            errorStep = "04"
                            ' send message error
                            Dim oneError As New architectureModels.errorClass
                            oneError.number = 640
                            oneError.userMsg = "Error in authentication using provided parameters."
                            errorList.Add(oneError)
                        End If

                    Else
                        errorStep = "05"
                        ' send message error
                        Dim oneError As New architectureModels.errorClass
                        oneError.number = 620
                        oneError.userMsg = "One or more fields are empty."
                        errorList.Add(oneError)
                    End If
                    '
                End If

            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in ProcessLogin - step: " & errorStep)
            End Try
            Return result
        End Function
        '
        '
        '
        Public Shared Function ForgetPassword(ByVal CP As CPBaseClass, email As String) As Boolean
            Dim Result As Boolean
            Try
                Call CP.email.sendPassword(email)
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in ForgetPassword")
            End Try
            Return Result
        End Function
        '
        '
        '
    End Class
    '
End Namespace

