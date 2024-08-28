Public Class MeybankATM
    Implements ILogin, IBalance, IDeposit, IWithdrawal, IThirdPartyTransfer, ITransaction

    Private Shared tries As Integer
    Private Const maxTries As Integer = 3
    Private Const minimum_kept_amt As Decimal = 20

    'todo: A transaction class with transaction amount can replace these two variable.
    Private Shared transaction_amt As Decimal

    Private Shared _accountList As List(Of BankAccount)
    Private Shared _listOfTransactions As List(Of Transaction)
    Private Shared selectedAccount As BankAccount
    Private Shared inputAccount As BankAccount

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
