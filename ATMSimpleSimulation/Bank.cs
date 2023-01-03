using System.Collections;
using System.Text;

namespace ATMSimpleSimulation
{
    public class Bank : IEnumerable<Account>
    {
        private List<Client> _clients;
        private List<Account> _accounts;
        private List<string> _usedCardNumbers;
        private List<ATM> _atms;
        public string Name { get; private set; }

        public Bank(string name)
        {
            Name = name;
            _accounts = new List<Account>();
            _atms = new List<ATM>();
            _clients = new List<Client>();
            _usedCardNumbers = new List<string>();
        }

        public void AddAtm(ATM atm) {
            this._atms.Add(atm);
        }

        public List<Account> GetAllAccounts() {
            return this._accounts;
        }

        public ATM? GetATM(int serialId)
        {
            return this._atms.Find(x => x.SerialId() == serialId);
        }

        private string GenerateNewCardNumber()
        {
            Random r = new Random();
            StringBuilder number = new StringBuilder();
            while (true)
            {
                number.Clear();
                for (int i = 0; i < 10; i++)
                {
                    number.Append(r.Next(0, 10).ToString());
                }
                if (!_usedCardNumbers.Contains(number.ToString()))
                {
                    _usedCardNumbers.Add(number.ToString());
                    break;
                }
            }
            return number.ToString();
        }

        private string GenerateNewCVV() {
            StringBuilder cvv = new StringBuilder();
            for (int i = 0; i < 3; i++) {
                var randomNumber = new Random().Next(0, 10);
                cvv.Append(randomNumber.ToString());
            }
            return cvv.ToString();
        }

        public Card CreateNewCard(Account account, CARDTYPES cardType)
        {
            string newCardNumber = GenerateNewCardNumber();
            if (cardType == CARDTYPES.DEBIT)
                return new DebitCard(account.Name, newCardNumber, cardType, "", account);
            else {
                return new CreditCard(account.Name, newCardNumber, cardType, "", account, 500, GenerateNewCVV());
            }
        }

        public Account CreateAccount(Client client)
        {
            var newAccount = new Account(this, client, GenerateNewCardNumber());
            _accounts.Add(newAccount);
            Console.WriteLine($"Your {Name} Bank Account has been created!");
            return newAccount;
        }

        public DebitCard GenerateAccountDebitCard(Account clientAccount) {
            DebitCard newDebitCard = (DebitCard)CreateNewCard(clientAccount, CARDTYPES.DEBIT);
            clientAccount.AddDebitCard(newDebitCard);
            return newDebitCard;
        }

        public string GenerateDetailStatement(Card card)
        {
            StringBuilder statement = new StringBuilder();

            statement.AppendLine($"{Name} Bank Statement");
            statement.AppendLine($"Client: {card.CardAccount.Name}");
            statement.AppendLine($"Account phone number: {card.CardAccount.PhoneNumber}");
            statement.AppendLine($"Account number: {card.CardAccount.AccountNumber}");
            statement.AppendLine($"Account balance: {card.CardAccount.Balance()}");

            var bank = card.CardAccount.Bank();
            var clientCards = card.CardAccount.AccountClient.GetCards(bank);
            foreach (var clientCard in clientCards)
            {
                if (clientCard.GetType() == typeof(DebitCard))
                {
                    statement.AppendLine("=========================");
                    statement.AppendLine("Debit card Info:");
                } else
                {
                    statement.AppendLine("=========================");
                    statement.AppendLine("Credit card Info:");
                }
                statement.AppendLine($"Type: {card.CardType}");
                statement.AppendLine($"Card number: {card.CardNumber}");
                statement.AppendLine($"Card Balance: {card.Balance()}");
                if (clientCard.GetType() == typeof(CreditCard))
                {
                    var creditCard = (CreditCard)clientCard;
                    statement.AppendLine($"Total credit: {creditCard.TotalCredit}");
                    statement.AppendLine($"Available to widthdraw: {creditCard.GetAvailableWithdrawCredit()}");
                }
            }
            return statement.ToString();
        }

        public IEnumerator<Account> GetEnumerator()
        {
            return this._accounts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
