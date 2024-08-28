Imports System.Globalization

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






End Class
