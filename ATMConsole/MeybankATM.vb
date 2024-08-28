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



    Public Sub Initialization()
        transaction_amt = 0

        _accountList = New List(Of BankAccount) From {
    New BankAccount() With {
            .FullName = "John",
            .AccountNumber = 333111,
            .CardNumber = 123,
            .PinCode = 111111,
            .Balance = 2000D,
            .isLocked = False
        },
    New BankAccount() With {
            .FullName = "Mike",
            .AccountNumber = 111222,
            .CardNumber = 456,
            .PinCode = 222222,
            .Balance = 1500.3D,
            .isLocked = True
        },
    New BankAccount() With {
            .FullName = "Mary",
            .AccountNumber = 888555,
            .CardNumber = 789,
            .PinCode = 333333,
            .Balance = 2900.12D,
            .isLocked = False
        }
}
    End Sub


    Private Shared Sub LockAccount()
        Console.Clear()
        Utility.PrintMessage("Your account is locked.", True)
        Console.WriteLine("Please go to the nearest branch to unlocked your account.")
        Console.WriteLine("Thank you for using Meybank. ")
        Console.ReadKey()
        Environment.Exit(1)
    End Sub


    Public Sub Execute()
        'Initialization();
        ATMScreen.ShowMenu1()

        While True
            Select Case Utility.GetValidIntInputAmt("your option")
                Case 1
                    CheckCardNoPassword()

                    _listOfTransactions = New List(Of Transaction)()

                    While True
                        ATMScreen.ShowMenu2()

                        Select Case Utility.GetValidIntInputAmt("your option")
                            Case CInt(SecureMenu.CheckBalance)
                                CheckBalance(selectedAccount)
                            Case CInt(SecureMenu.PlaceDeposit)
                                PlaceDeposit(selectedAccount)
                            Case CInt(SecureMenu.MakeWithdrawal)
                                MakeWithdrawal(selectedAccount)
                            Case CInt(SecureMenu.ThirdPartyTransfer)
                                Dim vMThirdPartyTransfer = New VMThirdPartyTransfer()
                                vMThirdPartyTransfer = ATMScreen.ThirdPartyTransferForm()

                                PerformThirdPartyTransfer(selectedAccount, vMThirdPartyTransfer)
                            Case CInt(SecureMenu.ViewTransaction)
                                ViewTransaction(selectedAccount)

                            Case CInt(SecureMenu.Logout)
                                Utility.PrintMessage("You have succesfully logout. Please collect your ATM card..", True)

                                Execute()
                            Case Else
                                Utility.PrintMessage("Invalid Option Entered.", False)
                        End Select
                    End While

                Case 2
                    Console.Write(vbLf & "Thank you for using Meybank. Exiting program now .")
                    Utility.printDotAnimation(15)

                    Environment.Exit(1)
                Case Else
                    Utility.PrintMessage("Invalid Option Entered.", False)
            End Select
        End While
    End Sub


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
        Dim pass = False

        While Not pass
            inputAccount = New BankAccount()

            Console.WriteLine(vbLf & "Note: Actual ATM system will accept user's ATM card to validate")
            Console.Write("and read card number, bank account number and bank account status. " & vbLf & vbLf)
            'Console.Write("Enter ATM Card Number: ");
            'inputAccount.CardNumber = Convert.ToInt32(Console.ReadLine());
            inputAccount.CardNumber = Utility.GetValidIntInputAmt("ATM Card Number")

            Console.Write("Enter 6 Digit PIN: ")
            inputAccount.PinCode = Convert.ToInt32(Utility.GetHiddenConsoleInput())
            ' for brevity, length 6 is not validated and data type.


            System.Console.Write(vbLf & "Checking card number and password.")
            Utility.printDotAnimation()

            For Each account As BankAccount In _accountList
                If inputAccount.CardNumber.Equals(account.CardNumber) Then
                    selectedAccount = account

                    If inputAccount.PinCode.Equals(account.PinCode) Then
                        If selectedAccount.isLocked Then
                            LockAccount()
                        Else
                            pass = True
                        End If
                    Else

                        pass = False
                        tries += 1

                        If tries >= maxTries Then
                            selectedAccount.isLocked = True

                            LockAccount()
                        End If

                    End If
                End If
            Next

            If Not pass Then Utility.PrintMessage("Invalid Card number or PIN.", False)

            Console.Clear()
        End While
    End Sub

    Public Sub CheckBalance(bankAccount As BankAccount) Implements IBalance.CheckBalance
        Throw New NotImplementedException()
    End Sub

    Public Sub PlaceDeposit(bankAccount As BankAccount) Implements IDeposit.PlaceDeposit
        Throw New NotImplementedException()
    End Sub
End Class
