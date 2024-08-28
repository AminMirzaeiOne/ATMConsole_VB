Public Interface ITransaction
    Sub InsertTransaction(bankAccount As BankAccount, transaction As Transaction)

    Sub ViewTransaction(bankAccount As BankAccount)
End Interface
