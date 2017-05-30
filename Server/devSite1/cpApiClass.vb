
Option Explicit On
Option Strict On

Imports System.Web.Configuration

Public Class cpApiClass
    '
    '==============================================================================
    ''' <summary>
    ''' return the contensive page
    ''' </summary>
    ''' <param name="cp"></param>
    ''' <param name="isAdmin"></param>
    ''' <returns></returns>
    Public Shared Function getContensivePage(cp As Contensive.Processor.CPClass, page As Page, isAdmin As Boolean) As String
        Dim result As String = ""
        Try
            Dim TotalBytes As Integer
            Dim isBinaryRequest As Boolean = False
            Dim c As String
            Dim cName As String
            Dim row As String()
            Dim rowPtr As Integer
            '
            ' -- Setup Contensive
            cp.Context.appName = WebConfigurationManager.AppSettings("applicationName")
            cp.Context.domain = page.Request.ServerVariables("SERVER_NAME")
            cp.Context.pathPage = CStr(page.Request.ServerVariables("SCRIPT_NAME"))
            cp.Context.referrer = CStr(page.Request.ServerVariables("HTTP_REFERER"))
            cp.Context.isSecure = CBool(page.Request.ServerVariables("SERVER_PORT_SECURE"))
            cp.Context.remoteIp = CStr(page.Request.ServerVariables("REMOTE_ADDR"))
            cp.Context.browserUserAgent = CStr(page.Request.ServerVariables("HTTP_USER_AGENT"))
            cp.Context.acceptLanguage = CStr(page.Request.ServerVariables("HTTP_ACCEPT_LANGUAGE"))
            cp.Context.accept = CStr(page.Request.ServerVariables("HTTP_ACCEPT"))
            cp.Context.acceptCharSet = CStr(page.Request.ServerVariables("HTTP_ACCEPT_CHARSET"))
            cp.Context.profileUrl = CStr(page.Request.ServerVariables("HTTP_PROFILE"))
            cp.Context.xWapProfile = CStr(page.Request.ServerVariables("HTTP_X_WAP_PROFILE"))
            '
            ' -- Create ServerQueryString
            Dim requestUrl As String = page.Request.Url.ToString()
            Dim queryPos As Integer = requestUrl.IndexOf("?")
            If (queryPos > 0) Then
                cp.Context.queryString = requestUrl.Substring(queryPos + 1)
            End If
            For Each key As String In page.Request.QueryString
                If (Not String.IsNullOrEmpty(key)) Then
                    isBinaryRequest = isBinaryRequest Or (key.ToLower() = "requestbinary")
                End If
            Next
            cp.Context.isBinaryRequest = isBinaryRequest
            '
            ' Create ServerForm
            '
            If isBinaryRequest Then
                '
                ' binary multipart form 
                '
                TotalBytes = page.Request.TotalBytes
                If TotalBytes > 0 Then
                    page.Server.ScriptTimeout = 1800
                    cp.Context.binaryRequest = page.Request.BinaryRead(TotalBytes - 1)
                End If
            Else
                '
                ' non-binary form, create Form String
                '
                c = ""
                For Each key As String In page.Request.Form
                    If (Not String.IsNullOrEmpty(key)) Then
                        c = c & "&" & page.Server.UrlEncode(key) & "=" & page.Server.UrlEncode(page.Request.Form(key))
                    End If
                Next
                If Len(c) > 0 Then
                    c = Mid(c, 2)
                End If
                cp.Context.form = c
            End If
            '
            ' Create ServerCookie string
            '
            c = ""
            For Each key As String In page.Request.Cookies
                If (Not String.IsNullOrEmpty(key)) Then
                    c = c & "&" & key & "=" & page.Server.UrlEncode(page.Request.Cookies.Item(key).Value)
                End If
            Next
            If Len(c) > 0 Then
                c = Mid(c, 2)
            End If
            cp.Context.cookies = c
            '
            ' get doc and process Contensive output
            '
            result = result & cp.getDoc(isAdmin)
            '
            ' setup IIS Response
            '
            page.Response.CacheControl = "no-cache"
            page.Response.Expires = -1
            page.Response.Buffer = True
            '
            ' post processing
            '
            c = cp.Context.responseRedirect
            If c <> "" Then
                '
                ' redirect
                '
                If Not page.Response.IsRequestBeingRedirected Then
                    page.Response.Redirect(c, False)
                    page.Response.End()
                End If
            Else
                '
                ' concatinate writestream data to the end of the body
                '
                result = result & cp.Context.responseBuffer
                '
                ' set content type
                '
                c = cp.Context.responseContentType
                If c <> "" Then
                    page.Response.ContentType = c
                End If
                '
                ' set cookies
                '
                c = cp.Context.responseCookies
                If c <> "" Then
                    row = Split(c, vbCrLf)
                    Do While (rowPtr + 5) <= UBound(row)
                        cName = row(rowPtr + 0)
                        If cName <> "" Then
                            page.Response.Cookies.Add(New HttpCookie(cName, row(rowPtr + 1)))
                            c = row(rowPtr + 2)
                            If c <> "" Then
                                page.Response.Cookies(cName).Expires = cp.Utils.EncodeDate(c)
                            End If
                            c = row(rowPtr + 3)
                            If c <> "" Then
                                page.Response.Cookies(cName).Domain = c
                            End If
                            c = row(rowPtr + 4)
                            If c <> "" Then
                                page.Response.Cookies(cName).Path = c
                            End If
                            c = row(rowPtr + 5)
                            If c <> "" Then
                                page.Response.Cookies(cName).Secure = cp.Utils.EncodeBoolean(c)
                            End If
                        End If
                        rowPtr = rowPtr + 6
                    Loop
                End If
                '
                ' set headers
                '
                c = cp.Context.responseHeaders
                If c <> "" Then
                    row = Split(c, vbCrLf)
                    rowPtr = 0
                    Do While (rowPtr + 1) <= UBound(row)
                        cName = row(rowPtr + 0)
                        If cName <> "" Then
                            Call page.Response.AddHeader(cName, CStr(row(rowPtr + 1)))
                        End If
                        rowPtr = rowPtr + 2
                    Loop
                End If
                '
                ' set http status
                '
                c = cp.Context.responseStatus
                If c <> "" Then
                    page.Response.Status = c
                End If
            End If
        Catch ex As Exception
            cp.Site.ErrorReport(ex)
        End Try
        Return result
    End Function

End Class
