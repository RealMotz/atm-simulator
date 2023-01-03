namespace ATMSimpleSimulation
{
    public class ATM
    {
        private float _availableMoney;
        private int _serialId;
        private bool _authenticated;
        private HashSet<string> _banksAllowed;
        private readonly float[] _quickDrawOptions = new float[] { 20f, 50f, 100f };
        public Card? CardInUSe { get; set; }

        public ATM(int serialId, Bank bank, string[]? allowedBanks =null)
        {
            _serialId = serialId;
            _availableMoney = 0;
            _authenticated = false;
            _banksAllowed = new HashSet<string>();
            CardInUSe = null;
            if (allowedBanks != null)
                foreach (string allowedBank in allowedBanks) _banksAllowed.Add(allowedBank);
            //_quickDrawOptions = new int[3] { 20, 50, 100 };
        }

        private void ShowBankWithdrawPolicy(Bank bank)
        {
            Console.WriteLine($"{bank.Name} might apply additional charges...");
        }

        public void Deposit(float money)
        {
            this._availableMoney+= money;
        }

        public void AddAllowedBank(string bankName) {
            this._banksAllowed.Add(bankName);
        }

        public bool Authenticate(Card card, string pin)
        {
            if (_authenticated) return true;
            if (card.CardAccount.PIN == pin)
            {
                this._authenticated = true;
                return true;
            }

            Console.WriteLine("Invalid PIN...");
            return false;
        }

        public bool Read(Card card) {
            var bankName = card.CardAccount.Bank().Name;
            if (!_banksAllowed.Contains(bankName)) {
                Console.WriteLine($"This ATM does not support {bankName} Bank Cards...");
                return false;
            }

            CardInUSe = card;
            ShowBankWithdrawPolicy(card.CardAccount.Bank());
            return true;
        }

        public void CancelTransaction()
        {
            // [TODO] These two could be unified
            _authenticated = false;
            CardInUSe = null;
        }

        public bool IsAutheticated() {
            return this._authenticated;
        }

        public int SerialId()
        {
            return _serialId;
        }

        public void ShowOptions()
        {
            if (CardInUSe == null) return;
            Console.WriteLine($"Welcome {CardInUSe.CardHolderName}!!");
            Console.WriteLine("Available options: ");
            Console.WriteLine("[1] Withdraw");
            Console.WriteLine("[2] Cash Available");
            Console.WriteLine("[3] Change PIN");
            Console.WriteLine("[4] Change Mobile Number");
            Console.WriteLine("[5] Recent Transaccions");
            Console.WriteLine("[6] Bank Statement");
        }

        public float GetQuickWidthdrawOption(string option)
        {
            return option switch
            {
                "1" => 20f,
                "2" => 50f,
                "3" => 100f,
                _ => 0,
            };
        }

        public bool Widthdraw(IConsoleUserInput console)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("Available options: ");
            Console.WriteLine("[1] Quick Withdraw");
            Console.WriteLine("[2] Standard Withdraw");
            Console.Write("Selection: ");
            var userOption = console.GetUserInput();
            float moneyToWidthdraw;
            if (userOption == null) return false;

            if (userOption.Trim() == "1")
            {
                Console.WriteLine("Available options: ");
                for(int i = 1; i <= _quickDrawOptions.Length; i++)
                    Console.WriteLine($"[{i}] ${_quickDrawOptions[i-1]}");

                Console.Write("Selection: ");
                userOption = console.GetUserInput();

                // [TODO] Validate options. Throw exceptions when needed.
                var allOptions = Enumerable.Range(1, _quickDrawOptions.Count()).Select(i => i.ToString()).ToList();
                if (userOption == null || !allOptions.Contains(userOption)) {
                    Console.WriteLine("Wrong option. Returning to main menu...");
                    return false;
                }

                moneyToWidthdraw = GetQuickWidthdrawOption(userOption);
            }
            else if (userOption.Trim() == "2")
            {
                Console.Write("Money to widthdraw: ($) ");
                userOption = console.GetUserInput();
                if (!float.TryParse(userOption, out moneyToWidthdraw)) return false;
                if(moneyToWidthdraw <= 0) return false;
            }
            else {
                // [TODO] Test this case. Maybe?
                Widthdraw(console);
                return true;
            }

            if (this._availableMoney < moneyToWidthdraw)
            {
                Console.WriteLine("ATM widthdraw error... try again later...");
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine("");
                Console.WriteLine("");
                return false;
            }

            CardInUSe?.Widthdraw(moneyToWidthdraw);
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("");
            Console.WriteLine("");
            return true;
        }

        public void CheckBalance()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("------------");
            Console.WriteLine("Card Balance");
            Console.WriteLine("------------");
            Console.WriteLine($"HolderName: {CardInUSe?.CardHolderName}");
            Console.WriteLine($"Account: {CardInUSe?.CardNumber}");
            Console.WriteLine($"Account Balance: {CardInUSe?.Balance()}");

            if (CardInUSe?.GetType() != typeof(CreditCard)) {
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine("");
                Console.WriteLine("");
                return;
            }
            CreditCard creditCard = (CreditCard)CardInUSe;
            Console.WriteLine("------------");
            Console.WriteLine($"Credit Balance: {creditCard.Balance()}");
            Console.WriteLine($"Total Credit: {creditCard.TotalCredit}");
            Console.WriteLine($"Available Widthdraw Credit: {creditCard.GetAvailableWithdrawCredit()}");
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        private void ChangeUserPin(IConsoleUserInput console)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.Write("Type current PIN: ");
            string? typedPIN  = "";
            while (typedPIN == null || (typedPIN.Length < 4))
                typedPIN = console.GetUserInput();

            string oldPin;
            if (CardInUSe.GetType() == typeof(DebitCard))
                oldPin = CardInUSe.CardAccount.PIN;
            else {
                oldPin = ((CreditCard)CardInUSe).Pin();
            }

            if (typedPIN != oldPin)
            {
                Console.WriteLine("Incorrect PIN. Try Again Later...");
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine("");
                Console.WriteLine("");
                return;
            }

            CardInUSe?.ChangePIN(typedPIN, console);
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        private void ChangeMobilePhoneNumber(IConsoleUserInput console)
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.Write("Type current phone number: ");
            var phoneNumber = Console.ReadLine();
            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber != CardInUSe.CardAccount.PhoneNumber)
            {
                Console.WriteLine("Invalid phone number...");
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine("");
                Console.WriteLine("");
                return;
            }
            CardInUSe.CardAccount.HandlePhoneNumberChange(console);
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        private void GenerateLatestTransactionsReport()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("Previous 5 transactions: ");
            var last5Transactions = CardInUSe.CardAccount.GetLastestTransactions(5).ToList();
            foreach (var transaction in last5Transactions)
            {
                Console.WriteLine(transaction);
            }
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        private void GenerateBankStatement()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            var bank = CardInUSe.CardAccount.Bank();
            Console.WriteLine(bank.GenerateDetailStatement(CardInUSe));
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        public void SelectOption(string option, IConsoleUserInput console)
        {
            if (CardInUSe == null) return;
            switch (option) {
                case "1":
                    // Widthdraw
                    Widthdraw(console);
                    break;
                case "2":
                    // Cash available
                    CheckBalance();
                    break;
                case "3":
                    // Change PIN
                    ChangeUserPin(console);
                    break;
                case "4":
                    // Change mobile number
                    ChangeMobilePhoneNumber(console);
                    break;
                case "5":
                    // Recent transactions
                    GenerateLatestTransactionsReport();
                    break;
                case "6":
                    // Bank statement
                    GenerateBankStatement();
                    break;
                default:
                    break;
            }
        }
    }
}
