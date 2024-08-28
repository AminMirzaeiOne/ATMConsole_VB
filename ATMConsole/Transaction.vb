Public Enum TransactionType

    Deposit
    Withdrawal
    ThirdPartyTransfer
End Enum


Public Class Transaction
    Public Property TransactionId As Integer

    Public Property BankAccountNoFrom As Long


    Public Property BankAccountNoTo As Long

    Public Property TransactionType As TransactionType
    Public Property TransactionAmount As Decimal


    Public Property TransactionDate As Date
End Class
