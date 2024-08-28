Imports System.Globalization
Imports System.Text
Imports System.Threading

Public Class Utility
    Private Shared culture As CultureInfo = New CultureInfo("ms-MY")

    Public Shared Function GetValidDecimalInputAmt(input As String) As Decimal
        Dim valid = False
        Dim rawInput As String
        Dim amount As Decimal = 0

        ' Get user's input input type is valid
        While Not valid
            rawInput = GetRawInput(input)
            valid = Decimal.TryParse(rawInput, amount)
            If Not valid Then PrintMessage("Invalid input. Try again.", False)
        End While

        Return amount
    End Function

    Public Shared Function GetValidIntInputAmt(input As String) As Long
        Dim valid = False
        Dim rawInput As String
        Dim amount As Long = 0

        ' Get user's input input type is valid
        While Not valid
            rawInput = GetRawInput(input)
            valid = Long.TryParse(rawInput, amount)
            If Not valid Then PrintMessage("Invalid input. Try again.", False)
        End While

        Return amount
    End Function

    Public Shared Function GetRawInput(message As String) As String
        Console.Write($"Enter {message}: ")
        Return Console.ReadLine()
    End Function

    Public Shared Function GetHiddenConsoleInput() As String
        Dim input As StringBuilder = New StringBuilder()
        While True
            Dim key = Console.ReadKey(True)
            If key.Key Is ConsoleKey.Enter Then Exit While
            If key.Key Is ConsoleKey.Backspace AndAlso input.Length > 0 Then
                input.Remove(input.Length - 1, 1)
            ElseIf key.Key IsNot ConsoleKey.Backspace Then
                input.Append(key.KeyChar)
            End If
        End While
        Return input.ToString()
    End Function


    Public Shared Sub printDotAnimation(Optional timer As Integer = 10)
        For x = 0 To timer - 1
            System.Console.Write(".")
            Thread.Sleep(100)
        Next
        Console.WriteLine()
    End Sub

    Public Shared Function FormatAmount(amt As Decimal) As String
        Return String.Format(culture, "{0:C2}", amt)
    End Function

    Public Shared Sub PrintMessage(msg As String, success As Boolean)
        If success Then
            Console.ForegroundColor = ConsoleColor.Yellow
        Else
            Console.ForegroundColor = ConsoleColor.Red
        End If

        Console.WriteLine(msg)
        Console.ResetColor()
        Console.WriteLine("Press any key to continue")
        Console.ReadKey()
    End Sub






End Class
