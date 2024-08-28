Public Class MeybankATM
    Implements ILogin, IBalance, IDeposit, IWithdrawal, IThirdPartyTransfer, ITransaction

    Public Sub InsertTransaction(bankAccount As BankAccount, transaction As Transaction) Implements ITransaction.InsertTransaction
        Throw New NotImplementedException()
    End Sub

    Public Sub ViewTransaction(bankAccount As BankAccount) Implements ITransaction.ViewTransaction
        Throw New NotImplementedException()
    End Sub

    Public Sub PerformThirdPartyTransfer(bankAccount As BankAccount, vmThirdPartyTransfer As VMThirdPartyTransfer) Implements IThirdPartyTransfer.PerformThirdPartyTransfer
        Throw New NotImplementedException()
    End Sub

    Public Sub MakeWithdrawal(bankAccount As BankAccount) Implements IWithdrawal.MakeWithdrawal
        Throw New NotImplementedException()
    End Sub

    Public Sub CheckCardNoPassword() Implements ILogin.CheckCardNoPassword
        Throw New NotImplementedException()
    End Sub

    Public Sub CheckBalance(bankAccount As BankAccount) Implements IBalance.CheckBalance
        Throw New NotImplementedException()
    End Sub

    Public Sub PlaceDeposit(bankAccount As BankAccount) Implements IDeposit.PlaceDeposit
        Throw New NotImplementedException()
    End Sub
End Class
