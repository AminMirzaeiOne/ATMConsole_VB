﻿
Imports System.ComponentModel

Public Enum SecureMenu
    ' Value 1 is needed because menu starts with 1 while enum starts with 0 if no value given.

    <Description("Check balance")>
    CheckBalance = 1

    <Description("Place Deposit")>
    PlaceDeposit = 2

    <Description("Make Withdrawal")>
    MakeWithdrawal = 3

    <Description("Third Party Transfer")>
    ThirdPartyTransfer = 4

    <Description("Transaction")>
    ViewTransaction = 5

    <Description("Logout")>
    Logout = 6
End Enum


Public Class ATMScreen
    Friend Shared cur As String = "RM "

    Public Shared Function ThirdPartyTransferForm() As VMThirdPartyTransfer
        Dim vMThirdPartyTransfer = New VMThirdPartyTransfer()

        ''' Console.Write("\nRecipient's account number: ");
        'vMThirdPartyTransfer.RecipientBankAccountNumber = Convert.ToInt64(Console.ReadLine()); // no validation here yet.
        vMThirdPartyTransfer.RecipientBankAccountNumber = Utility.GetValidIntInputAmt("recipient's account number")

        'Console.Write($"\nTransfer amount: {cur}");
        vMThirdPartyTransfer.TransferAmount = Utility.GetValidDecimalInputAmt("amount")

        'Console.Write("\nRecipient's account name: ");
        vMThirdPartyTransfer.RecipientBankAccountName = Utility.GetRawInput("recipient's account name")
        ' no validation here yet.

        Return vMThirdPartyTransfer
    End Function


End Class
