namespace ATMSimpleSimulation
{
    public class TransactionEventArgs : EventArgs
    {
        public string? TransactionType { get; set; }
        public float TransactionAmount { get; set; }
    }

    public class Account
    {
        private int MAX_FAIL_ATTEMPTS = 3;
        private Bank _bank;
        private float _balance;

        public Client AccountClient { get; private set; }
        public string AccountNumber { get; private set; }
        public string Name { get; private set; }
        public string PhoneNumber { get; private set; }
        public string PIN { get; private set; }
        public DebitCard? DebitCard { get; private set; }
        public List<CreditCard> CreditCards = new List<CreditCard>();
        public List<string> Transactions = new List<string>();
        public event EventHandler<TransactionEventArgs> TransactionCompletedHandler;

        public Account(Bank bank, Client client, string accountNumber) {
            this.Name = client.Name;
            this.PhoneNumber = client.PhoneNumber;
            this.PIN = "0000";
            this.DebitCard = null;
            this.CreditCards = new List<CreditCard>();
            this.AccountNumber = accountNumber;
            this._balance = 0;
            this._bank = bank;
            this.AccountClient = client;
        }

        protected virtual void OnTransactionCompleted(TransactionEventArgs e) {
            EventHandler<TransactionEventArgs> handler = TransactionCompletedHandler;
            handler?.Invoke(this, e);
        }

        public Bank Bank()
        {
            return this._bank;
        }

        private bool CanWidthdraw(float amount)
        {
            if (_balance < amount) {
                Console.WriteLine($"Cannot Withdraw: Insufficient funds...");
                return false;
            }
            return true;
        }

        public void AddDebitCard(DebitCard card)
        {
            this.DebitCard = card;
        }

        public bool Widthdraw(float amount)
        {
            if (!CanWidthdraw(amount)) return false;
            _balance -= amount;
            Console.WriteLine($"New Balance: {_balance}");
            TransactionEventArgs args = new TransactionEventArgs();
            args.TransactionType = "Widthdraw from account";
            args.TransactionAmount = amount;
            OnTransactionCompleted(args);
            return true;
        }

        public void Deposit(float amount)
        {
            this._balance += amount;
            TransactionEventArgs args = new TransactionEventArgs();
            args.TransactionType = "Widthdraw from account";
            args.TransactionAmount = amount;
            OnTransactionCompleted(args);
        }

        public float Balance() {
            return this._balance;
        }

        public void BalanceReport() {
            Console.WriteLine($"Current Account Balance is: {this._balance}");
        }

        public bool ValidatePin(string pin)
        {
            if (pin.Length != 4) return false;
            var allDigits = pin.Select(i => char.GetNumericValue(i)).ToList();
            if(allDigits.Contains(-1)) return false;
            return true;
        }

        private string RequestForPin(string oldPin, IConsoleUserInput console)
        {
            string? newPin = "";
            int maxTries = 0;
            while (newPin == null || !ValidatePin(newPin) || newPin == oldPin)
            {
                newPin = console.GetUserInput();
                maxTries++;
                if (maxTries == MAX_FAIL_ATTEMPTS)
                    return "";
            }
            return newPin;
        }

        public string HandlePinChange(string oldPin, IConsoleUserInput console)
        {
            Console.Write("Type new PIN: ");
            string? newPin = RequestForPin(oldPin, console);
            if (newPin == "") return "";

            Console.Write("Confirm new PIN: ");
            string? confirmationPIN = RequestForPin(oldPin, console);
            if (confirmationPIN == "") return "";

            if (newPin != confirmationPIN) return "";
            return newPin;
        }

        public bool ChangeDebitCardPin(string oldPin, IConsoleUserInput console)
        {
            var newPin = HandlePinChange(oldPin, console);
            if (newPin == "") return false;

            PIN = newPin;
            Console.WriteLine("Your PIN has been changed!!");
            return true;
        }

        public bool Pay(float amount, IConsoleUserInput console)
        {
            var pin = console.GetUserInput();
            if (string.IsNullOrEmpty(pin)) return false;
            if (!ValidatePin(pin))
            {
                Console.WriteLine("Incorrect PIN ...");
                return false;
            }

            if (_balance < amount)
            {
                Console.WriteLine("Insufficient funds...");
                return false;
            }

            _balance-= amount;
            Console.WriteLine($"Payment for ${amount} was succesful!!");
            return true;
        }

        public bool HandlePhoneNumberChange(IConsoleUserInput console)
        {
            Console.Write("Type new phone number: ");
            string? newPhoneNumber = console.GetUserInput();
            if (string.IsNullOrEmpty(newPhoneNumber))
            {
                Console.WriteLine("Invalid phone number...");
                return false;
            }

            Console.Write("Confirm new phone number: ");
            string? confirmatioNewPhoneNumber = console.GetUserInput();
            if (string.IsNullOrEmpty(confirmatioNewPhoneNumber))
            {
                Console.WriteLine("Invalid phone number...");
                return false;
            }

            if (newPhoneNumber != confirmatioNewPhoneNumber)
            {
                Console.WriteLine("Phone numbers don't match...");
                return false;
            }
            this.PhoneNumber = newPhoneNumber;
            Console.WriteLine("Phone number changed succesfully!!");
            return true;
        }

        // [TODO] Add test
        public IEnumerable<string> GetLastestTransactions(int numberOfTransactions)
        {
            for (int i = Transactions.Count - 1; i >= 0; i--)
            {
                if (Transactions.Count - i <= numberOfTransactions)
                    yield return Transactions[i];
                else
                    break;
            }
        }
    }
}
