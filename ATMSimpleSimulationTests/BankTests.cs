namespace ATMSimpleSimulationTests
{
    [TestFixture]
    public class BankTests
    {
        Bank _bank;
        Account _account;

        [SetUp]
        public void Setup()
        {
            _bank = new Bank("TEST");
            _bank.AddAtm(new ATM(1, _bank));
            _bank.AddAtm(new ATM(2, _bank));
            _bank.AddAtm(new ATM(3, _bank));

            _account = new Account(_bank, new Client("Test client", 23, "123-123-123"), "123123");
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GetATM_FindsATM_ReturnsATM(int atmSerialId)
        {
            // Act
            var result = _bank.GetATM(atmSerialId);

            // Assert
            Assert.That(result, Is.InstanceOf<ATM>());
        }

        [Test]
        public void GetATM_CannotFindATM_ReturnsNULL()
        {
            // Act
            var result = _bank.GetATM(0);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateNewCard_CreatesADebitCardCard_ReturnsCard()
        {
            // Act
            var result = _bank.CreateNewCard(_account, CARDTYPES.DEBIT);

            // Assert
            Assert.That(result, Is.TypeOf<DebitCard>());
            Assert.That(result is Card);
        }

        [Test]
        [TestCase(CARDTYPES.VISA)]
        [TestCase(CARDTYPES.JCB)]
        [TestCase(CARDTYPES.MASTERCARD)]
        public void CreateNewCard_CreatesACreaditCardCard_ReturnsCard(CARDTYPES cardType)
        {
            // Act
            var result = _bank.CreateNewCard(_account, cardType);

            // Assert
            Assert.That(result, Is.TypeOf<CreditCard>());
            Assert.That(result is Card);
        }

        [Test]
        public void CreateNewCard_Creates100UniqueCards()
        {
            // Act
            var cardNumbers = Enumerable.Range(0,100)
                .Select(i => ((CreditCard)_bank.CreateNewCard(_account, CARDTYPES.VISA)).CardNumber)
                .ToList();

            // Assert
            Assert.That(cardNumbers.Distinct().Count(), Is.EqualTo(cardNumbers.Count));
            Assert.That(cardNumbers, Is.Unique);
            Assert.That(cardNumbers.Max(m => m.Length), Is.EqualTo(10));
        }

        [Test]
        public void CreateNewCard_Creates100CardCVV()
        {
            // Act
            var cvvs = Enumerable.Range(0, 100)
                .Select(i => ((CreditCard)_bank.CreateNewCard(_account, CARDTYPES.VISA)).GetCVV())
                .ToList();

            // Assert
            Assert.That(cvvs.Count, Is.EqualTo(100));
            Assert.That(cvvs.Max(m => m.Length), Is.EqualTo(3));
        }

        [Test]
        public void CreateAccount_CreatesNewAccount_ReturnsAccount()
        {
            // Act
            var result = _bank.CreateAccount(new Client("test", 24, "080-133-2233"));

            // Assert
            Assert.That(result, Is.TypeOf<Account>());
            Assert.That(_bank.GetAllAccounts().Count, Is.EqualTo(1));
        }

        [Test]
        public void GenerateAccountDebitCard_CreatesDebitCard_ReturnsAccount()
        {
            // Arrange
            var newAccount = new Account(_bank, new Client("Test client", 23, "123-123-123"), "144555");

            // Act
            var result = _bank.GenerateAccountDebitCard(newAccount);

            // Assert
            Assert.That(newAccount.DebitCard, Is.Not.Null);
            Assert.That(result, Is.TypeOf<DebitCard>());
        }
    }
}