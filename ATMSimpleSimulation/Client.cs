using System.Collections;

namespace ATMSimpleSimulation
{
    public class Client : IEnumerable<Account>
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public List<Account> Accounts { get; set; }
        public List<Card> Cards { get; set; }

        public Client(string name, int age, string phoneNumber)
        {
            Name = name;
            Age = age;
            PhoneNumber = phoneNumber;
            Accounts = new List<Account>();
            Cards = new List<Card>();
        }

        public IEnumerator<Account> GetEnumerator()
        {
            return Accounts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public DebitCard? GetDebitCard(Bank bank) {
            var accountDebitCards = from acc in bank.GetAllAccounts() select acc.DebitCard;
            //var accountDebitCards = bank.GetAllAccounts().Select(acc => acc.DebitCard);
            foreach (var card in Cards)
            {
                if (card is not DebitCard debitCard) continue;
                foreach (var accountDebitCard in accountDebitCards)
                    if (accountDebitCard.CardNumber == debitCard.CardNumber) return debitCard;
            }
            return null;
        }

        public IEnumerable<Card> GetCards(Bank bank)
        {
            foreach (var card in Cards)
                if (card.CardAccount.Bank().Name == bank.Name)
                    yield return card;
        }
    }
}
