Imports System.Security.Principal

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

    Private Shared Function PreviewBankNotesCount(amount As Decimal) As Boolean
        Dim hundredNotesCount As Integer = CInt(amount) / 100
        Dim fiftyNotesCount As Integer = (CInt(amount) Mod 100) / 50
        Dim tenNotesCount As Integer = (CInt(amount) Mod 50) / 10

        Console.WriteLine(vbLf & "Summary")
        Console.WriteLine("-------")
        Console.WriteLine($"{ATMScreen.cur} 100 x {hundredNotesCount} = {100 * hundredNotesCount}")
        Console.WriteLine($"{ATMScreen.cur} 50 x {fiftyNotesCount} = {50 * fiftyNotesCount}")
        Console.WriteLine($"{ATMScreen.cur} 10 x {tenNotesCount} = {10 * tenNotesCount}")
        Console.Write($"Total amount: {Utility.FormatAmount(amount)}

")

        'Console.Write("\n\nPress 1 to confirm or 0 to cancel: ");
        Dim opt As String = Utility.GetValidIntInputAmt("1 to confirm or 0 to cancel").ToString()

        Return If(opt.Equals("1"), True, False)
    End Function





    Public Sub InsertTransaction(bankAccount As BankAccount, transaction As Transaction) Implements ITransaction.InsertTransaction
        _listOfTransactions.Add(transaction)
    End Sub

    Public Sub ViewTransaction(bankAccount As BankAccount) Implements ITransaction.ViewTransaction
        If _listOfTransactions.Count <= 0 Then
            Utility.PrintMessage($"There is no transaction yet.", True)
        Else
            Dim table = New ConsoleTable("Type", "From", "To", "Amount " & ATMScreen.cur.ToString(), "Transaction Date")

            For Each tran In _listOfTransactions
                table.AddRow(tran.TransactionType, tran.BankAccountNoFrom, tran.BankAccountNoTo, tran.TransactionAmount, tran.TransactionDate)
            Next
            table.Options.EnableCount = False
            table.Write()
            Utility.PrintMessage($"You have performed {_listOfTransactions.Count} transactions.", True)
        End If
    End Sub

    Public Sub PerformThirdPartyTransfer(bankAccount As BankAccount, vmThirdPartyTransfer As VMThirdPartyTransfer) Implements IThirdPartyTransfer.PerformThirdPartyTransfer
        If vmThirdPartyTransfer.TransferAmount <= 0 Then
            Utility.PrintMessage("Amount needs to be more than zero. Try again.", False)
        ElseIf vmThirdPartyTransfer.TransferAmount > bankAccount.Balance Then
            ' Check giver's account balance - Start
            Utility.PrintMessage($"Withdrawal failed. You do not have enough fund to withdraw {Utility.FormatAmount(transaction_amt)}", False)
        ElseIf bankAccount.Balance - vmThirdPartyTransfer.TransferAmount < 20 Then
            ' Check giver's account balance - End
            Utility.PrintMessage($"Withdrawal failed. Your account needs to have minimum {Utility.FormatAmount(minimum_kept_amt)}", False)
        Else
            ' Check if receiver's bank account number is valid.
            Dim selectedBankAccountReceiver = (From b In _accountList Where b.AccountNumber Is vmThirdPartyTransfer.RecipientBankAccountNumber Select b).FirstOrDefault()

            If selectedBankAccountReceiver Is Nothing Then
                Utility.PrintMessage($"Third party transfer failed. Receiver bank account number is invalid.", False)
            ElseIf selectedBankAccountReceiver.FullName IsNot vmThirdPartyTransfer.RecipientBankAccountName Then
                Utility.PrintMessage($"Third party transfer failed. Recipient's account name does not match.", False)
            Else
                ' Bind transaction_amt to Transaction object
                ' Add transaction record - Start
                Dim transaction As Transaction = New Transaction() With {
    .BankAccountNoFrom = bankAccount.AccountNumber,
    .BankAccountNoTo = vmThirdPartyTransfer.RecipientBankAccountNumber,
    .TransactionType = TransactionType.ThirdPartyTransfer,
    .TransactionAmount = vmThirdPartyTransfer.TransferAmount,
    .TransactionDate = Date.Now
}
                _listOfTransactions.Add(transaction)
                Utility.PrintMessage($"You have successfully transferred out {Utility.FormatAmount(vmThirdPartyTransfer.TransferAmount)} to {vmThirdPartyTransfer.RecipientBankAccountName}", True)
                ' Add transaction record - End

                ' Update balance amount (Giver)
                bankAccount.Balance = bankAccount.Balance - vmThirdPartyTransfer.TransferAmount

                ' Update balance amount (Receiver)
                selectedBankAccountReceiver.Balance = selectedBankAccountReceiver.Balance + vmThirdPartyTransfer.TransferAmount
            End If
        End If
    End Sub

    Public Sub MakeWithdrawal(bankAccount As BankAccount) Implements IWithdrawal.MakeWithdrawal
        Console.WriteLine(vbLf & "Note: For GUI or actual ATM system, user can ")
        Console.Write("choose some default withdrawal amount or custom amount. " & vbLf & vbLf)

        ' Console.Write("Enter amount: " + ATMScreen.cur);
        ' transaction_amt = ATMScreen.ValidateInputAmount(Console.ReadLine());

        transaction_amt = Utility.GetValidDecimalInputAmt("amount")

        If transaction_amt <= 0 Then
            Utility.PrintMessage("Amount needs to be more than zero. Try again.", False)
        ElseIf transaction_amt > account.Balance Then
            Utility.PrintMessage($"Withdrawal failed. You do not have enough fund to withdraw {Utility.FormatAmount(transaction_amt)}", False)
        ElseIf account.Balance - transaction_amt < minimum_kept_amt Then
            Utility.PrintMessage($"Withdrawal failed. Your account needs to have minimum {Utility.FormatAmount(minimum_kept_amt)}", False)
        ElseIf transaction_amt Mod 10 <> 0 Then
            Utility.PrintMessage($"Key in the deposit amount only with multiply of 10. Try again.", False)
        Else
            ' Bind transaction_amt to Transaction object
            ' Add transaction record - Start
            Dim transaction = New Transaction() With {
        .BankAccountNoFrom = account.AccountNumber,
        .BankAccountNoTo = account.AccountNumber,
        .TransactionType = TransactionType.Withdrawal,
        .TransactionAmount = transaction_amt,
        .TransactionDate = Date.Now
    }
            InsertTransaction(account, transaction)
            ' Add transaction record - End

            ' Another method to update account balance.
            account.Balance = account.Balance - transaction_amt

            Utility.PrintMessage($"Please collect your money. You have successfully withdraw {Utility.FormatAmount(transaction_amt)}", True)
        End If
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
        Utility.PrintMessage($"Your bank account balance amount is: {Utility.FormatAmount(bankAccount.Balance)}", True)
    End Sub

    Public Sub PlaceDeposit(bankAccount As BankAccount) Implements IDeposit.PlaceDeposit
        Console.WriteLine(vbLf & "Note: Actual ATM system will just let you ")
        Console.Write("place bank notes into ATM machine. " & vbLf & vbLf)
        'Console.Write("Enter amount: " + ATMScreen.cur);
        transaction_amt = Utility.GetValidDecimalInputAmt("amount")

        System.Console.Write(vbLf & "Check and counting bank notes.")
        Utility.printDotAnimation()

        If transaction_amt <= 0 Then
            Utility.PrintMessage("Amount needs to be more than zero. Try again.", False)
        ElseIf transaction_amt Mod 10 <> 0 Then
            Utility.PrintMessage($"Key in the deposit amount only with multiply of 10. Try again.", False)
        ElseIf Not PreviewBankNotesCount(transaction_amt) Then
            Utility.PrintMessage($"You have cancelled your action.", False)
        Else
            ' Bind transaction_amt to Transaction object
            ' Add transaction record - Start
            Dim transaction = New Transaction() With {
        .BankAccountNoFrom = account.AccountNumber,
        .BankAccountNoTo = account.AccountNumber,
        .TransactionType = TransactionType.Deposit,
        .TransactionAmount = transaction_amt,
.TransactionDate = Date.Now
}
            InsertTransaction(account, transaction)
            ' Add transaction record - End

            ' Another method to update account balance.
            account.Balance = account.Balance + transaction_amt

            Utility.PrintMessage($"You have successfully deposited {Utility.FormatAmount(transaction_amt)}", True)
        End If
    End Sub
End Class
