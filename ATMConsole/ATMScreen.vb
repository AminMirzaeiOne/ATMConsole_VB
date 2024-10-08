﻿
Imports System.ComponentModel

Public Enum SecureMenu
    ' Value 1 is needed because menu starts with 1 while enum starts with 0 if no value given.

    <System.ComponentModel.Description("Check balance")>
    CheckBalance = 1

    <System.ComponentModel.Description("Place Deposit")>
    PlaceDeposit = 2

    <System.ComponentModel.Description("Make Withdrawal")>
    MakeWithdrawal = 3

    <System.ComponentModel.Description("Third Party Transfer")>
    ThirdPartyTransfer = 4

    <System.ComponentModel.Description("Transaction")>
    ViewTransaction = 5

    <System.ComponentModel.Description("Logout")>
    Logout = 6
End Enum


Public Class ATMScreen
    Friend Shared cur As String = "RM "

    Public Shared Function ThirdPartyTransferForm() As VMThirdPartyTransfer
        Dim vMThirdPartyTransfer = New VMThirdPartyTransfer()

        'vMThirdPartyTransfer.RecipientBankAccountNumber = Convert.ToInt64(Console.ReadLine()); // no validation here yet.
        vMThirdPartyTransfer.RecipientBankAccountNumber = Utility.GetValidIntInputAmt("recipient's account number")

        'Console.Write($"\nTransfer amount: {cur}");
        vMThirdPartyTransfer.TransferAmount = Utility.GetValidDecimalInputAmt("amount")

        'Console.Write("\nRecipient's account name: ");
        vMThirdPartyTransfer.RecipientBankAccountName = Utility.GetRawInput("recipient's account name")
        ' no validation here yet.

        Return vMThirdPartyTransfer
    End Function

    Public Shared Sub ShowMenu1()
        Console.Clear()
        Console.WriteLine(" ------------------------")
        Console.WriteLine("| Meybank ATM Main Menu  |")
        Console.WriteLine("|                        |")
        Console.WriteLine("| 1. Insert ATM card     |")
        Console.WriteLine("| 2. Exit                |")
        Console.WriteLine("|                        |")
        Console.WriteLine(" ------------------------")
    End Sub

    Public Shared Sub ShowMenu2()
        Console.Clear()
        Console.WriteLine(" ---------------------------")
        Console.WriteLine("| Meybank ATM Secure Menu    |")
        Console.WriteLine("|                            |")
        Console.WriteLine("| 1. Balance Enquiry         |")
        Console.WriteLine("| 2. Cash Deposit            |")
        Console.WriteLine("| 3. Withdrawal              |")
        Console.WriteLine("| 4. Third Party Transfer    |")
        Console.WriteLine("| 5. Transactions            |")
        Console.WriteLine("| 6. Logout                  |")
        Console.WriteLine("|                            |")
        Console.WriteLine(" ---------------------------")
    End Sub



End Class
