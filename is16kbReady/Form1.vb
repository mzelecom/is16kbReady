Imports System.Text.RegularExpressions
Imports System.IO

Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim ofd As New OpenFileDialog()
        ofd.Filter = "APK Files|*.apk"

        If ofd.ShowDialog() = DialogResult.OK Then
            txtFile.Text = ofd.FileName
            run()
        End If

    End Sub

    Sub run()

        rtbOutput.Clear()

        Try
            Dim p As New Process()
            p.StartInfo.FileName = ExtractChecker() '"checker.exe"
            p.StartInfo.Arguments = """" & txtFile.Text & """"
            p.StartInfo.RedirectStandardOutput = True
            p.StartInfo.UseShellExecute = False
            p.StartInfo.CreateNoWindow = True

            p.Start()
            Dim output As String = p.StandardOutput.ReadToEnd()
            p.WaitForExit()

            output = RemoveAnsi(output)
            output = output.Replace("âœ—", "").Replace("âš ï¸", "").Replace("âœ…", "").Replace("âœ", "").Replace("âŒ", "")
            Dim lines() As String = output.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

            For Each line As String In lines

                Dim isGreen = line.Contains("✓") Or
                              line.Contains("SUPPORTED")

                Dim isRed = line.Contains("✗") Or
                            line.Contains("ERROR") Or
                            line.Contains("FAILED") Or
                            line.Contains("UNALIGNED") Or
                            line.Contains("NOT SUPPORTED") Or
                            line.Contains("âœ—") Or
                            line.Contains("âŒ")

                If isGreen AndAlso Not isRed Then
                    AppendColored(line, Color.LimeGreen)
                ElseIf isRed Then
                    AppendColored(line, Color.Red)
                Else
                    AppendColored(line, Color.White)
                End If

            Next

        Catch ex As Exception

        End Try

    End Sub
    Private Function RemoveAnsi(input As String) As String
        Return Regex.Replace(input, "\x1B\[[0-9;]*[A-Za-z]", "")
    End Function

    Private Sub AppendColored(text As String, color As Color)
        rtbOutput.SelectionStart = rtbOutput.TextLength
        rtbOutput.SelectionColor = color
        rtbOutput.AppendText(text & vbCrLf)
        rtbOutput.SelectionColor = color.White
    End Sub

    Private Function ExtractChecker() As String
        Dim tempPath As String = Path.Combine(Path.GetTempPath(), "checker.exe")
        File.WriteAllBytes(tempPath, My.Resources.checker)
        Return tempPath
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
