using System.Transactions;

namespace ATMSimpleSimulation
{
    public enum CARDTYPES
    {
        DEBIT,
        VISA,
        MASTERCARD,
        JCB
    }
    public abstract class Card
    {
        public string CardHolderName { get; private set; }
        public string CardNumber { get; private set; }
        public CARDTYPES CardType { get; private set; }
        public Account CardAccount { get; private set; }
        public string ExpiracyDate { get; private set; }

        public Card(string holderName, string cardNumber, CARDTYPES cardType, string expiracyDate, Account account)
        {
            CardNumber = cardNumber;
            CardType = cardType;
            CardAccount = account;
            ExpiracyDate = expiracyDate;
            CardHolderName = holderName;

            CardAccount.TransactionCompletedHandler += new EventHandler<TransactionEventArgs>(RegisterTransaction);
        }


        public virtual float Balance()
        {
            return CardAccount.Balance();
        }
        public virtual bool Widthdraw(float amount)
        {
            return this.CardAccount.Widthdraw(amount);
        }
        public abstract bool ChangePIN(string oldPin, IConsoleUserInput console);
        public abstract bool Pay(float amount, IConsoleUserInput console);

        // [TODO] Add test
        // [TODO] Might be a good idea to move this from here
        private void RegisterTransaction(object? sender, TransactionEventArgs args)
        {
            if (string.IsNullOrEmpty(args.TransactionType)) return;
            var sign = args.TransactionType.Contains("widthdraw") ? "-" : "+";
            CardAccount.Transactions.Add($"[{args.TransactionType}] {sign}${args.TransactionAmount}");
        }
    }

    public class DebitCard : Card
    {
        public DebitCard(string holderName, string cardNumber, CARDTYPES cardType, string expiracyDate, Account account) : base(holderName, cardNumber, cardType, expiracyDate, account) {}

        public override bool ChangePIN(string oldPin, IConsoleUserInput console)
        {
            return this.CardAccount.ChangeDebitCardPin(oldPin, console);
        }

        public override bool Pay(float amount, IConsoleUserInput console)
        {
            return this.CardAccount.Pay(amount, console);
        }
    }

    public class CreditCard : Card
    {
        private string PIN;
        private string _cvv;
        public float TotalCredit;
        private float _availableCredit;
        private float _availableWidthdrawCredit;

        public CreditCard(string holderName, string cardNumber, CARDTYPES cardType, string expiracyDate, Account account, float credit, string cvv) : base(holderName, cardNumber, cardType, expiracyDate, account)
        {
            _availableCredit = credit;
            TotalCredit = credit;
            _cvv = cvv;
            _availableWidthdrawCredit = 50;
            PIN = "0000";
        }

        public string GetCVV()
        {
            return this._cvv;
        }

        public void SetCVV(string cvv)
        {
            this._cvv = cvv;
        }

        public string Pin()
        {
            return PIN;
        }

        public float GetAvailableWithdrawCredit()
        {
            return this._availableWidthdrawCredit;
        }

        public override float Balance()
        {
            return _availableCredit;
        }

        private bool ValidateCVV(string cvv, IConsoleUserInput console)
        {
            return cvv == this._cvv;
        }

        public override bool Pay(float amount, IConsoleUserInput console)
        {
            var cvv = console.GetUserInput();
            if (string.IsNullOrEmpty(cvv)) return false;
            if (!ValidateCVV(cvv, console))
            {
                Console.WriteLine("Invalid CVV...");
                return false;
            }
            if (_availableCredit < amount)
            {
                Console.WriteLine("Insufficient credit...");
                return false;
            }
            _availableCredit -= amount;
            Console.WriteLine($"Payment for ${amount} was succesful!!");
            return true;
        }

        public override bool Widthdraw(float amount)
        {
            if (_availableWidthdrawCredit < amount) return false;
            this._availableWidthdrawCredit-= amount;
            return true;
        }

        public override bool ChangePIN(string oldPin, IConsoleUserInput console)
        {
            var newPin = this.CardAccount.HandlePinChange(oldPin, console);
            if (newPin == "")
            {
                Console.WriteLine("Incorrect PIN. Try Again Later...");
                return false;
            }

            this.PIN = newPin;
            return true;
        }
    }
}
