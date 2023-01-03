using ATMSimpleSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMSimpleSimulationTests
{
    [TestFixture]
    public class ClientTests
    {
        Bank _bank;
        Client _client;

        public void SetupBankWith3Accounts()
        {
            var client1 = new Client("test_client_1", 22, "123");
            var client2 = new Client("test_client_2", 30, "4124");
            var client3 = new Client("test_client_3", 24, "12314555");

            var account1 = _bank.CreateAccount(client1);
            var account2 = _bank.CreateAccount(client2);
            var account3 = _bank.CreateAccount(client3);

            client1.Cards.Add(_bank.GenerateAccountDebitCard(account1));
            client1.Accounts.Add(account1);
            client2.Cards.Add(_bank.GenerateAccountDebitCard(account2));
            client2.Accounts.Add(account2);
            client3.Cards.Add(_bank.GenerateAccountDebitCard(account3));
            client3.Accounts.Add(account3);
        }

        [SetUp]
        public void Setup()
        {
            _client = new Client("Test", 30, "080-4655-3445");
            _bank = new Bank("TEST");
        }

        [Test]
        public void GetDebitCard_CannotFindCard_ReturnsNull()
        {
            // Arrange
            var bank = new Bank("TEST2");
            var account = bank.CreateAccount(_client);
            var debitCard = bank.GenerateAccountDebitCard(account);

            _client.Accounts.Add(account);
            _client.Cards.Add(debitCard);

            SetupBankWith3Accounts();

            // Act`
            var result = _client.GetDebitCard(_bank);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetDebitCard_FindsCard_ReturnsCard()
        {
            // Arrange
            var account = _bank.CreateAccount(_client);
            var debitCard = _bank.GenerateAccountDebitCard(account);

            _client.Accounts.Add(account);
            _client.Cards.Add(debitCard);

            // Act
            var result = _client.GetDebitCard(_bank);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.TypeOf<DebitCard>());
                Assert.That(result, Is.EqualTo(debitCard));
            });
        }

        [Test]
        public void GetCards_FindsCards_ReturnsCards()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [Test]
        // Doest this method return null??
        public void GetCards_NothingFound_ReturnsCards()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }
    }
}
