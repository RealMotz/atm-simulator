namespace ATMSimpleSimulationTests
{
    [TestFixture]
    public class ATMTests
    {
        Bank _bank;
        Account _account;
        ATM _atm;
        DebitCard _debitCard;
        readonly string _cardPIN = "0000";

        [SetUp]
        public void Setup()
        {
            _bank = new Bank("TEST");
            _atm = new ATM(1, _bank, new string[] { "TEST" });
            _account = new Account(_bank, new Client("Test client", 23, "123-123-123"), "123123");
            _debitCard = new DebitCard(_account.Name, "2323", CARDTYPES.DEBIT, "2023/12/31", _account);
        }

        [Test]
        public void Read_CardIsRecordedCorrectly_ReturnsTrue()
        {
            // Act
            var result = _atm.Read(_debitCard);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(_atm.CardInUSe, Is.Not.Null);
        }

        [Test]
        public void Read_CardIsNotRecordedCorrectly_ReturnsFalse()
        {
            // Arrange
            var atm = new ATM(1, _bank);

            // Act
            var result = atm.Read(_debitCard);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(atm.CardInUSe, Is.Null);
        }

        [Test]
        public void Authenticate_SuccessFulAuthentication_ReturnsTrue()
        {
            // Act
            var result = _atm.Authenticate(_debitCard, _cardPIN);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Authenticate_FailedAuthentication_ReturnsFalse()
        {
            // Arrange
            var wrongPin = "1234";

            // Act
            var result = _atm.Authenticate(_debitCard, wrongPin);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("1", 20f)]
        [TestCase("2", 50f)]
        [TestCase("3", 100f)]
        [TestCase("", 0f)]
        [TestCase("-1", 0f)]
        [TestCase("5", 0f)]
        public void GetQuickWidthdrawOption_RecievesOption_ReturnsFloat(string option, float expectedResult)
        {
            // Act
            var result = _atm.GetQuickWidthdrawOption(option);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("")]
        [TestCase("5")]
        [TestCase("asdad")]
        [TestCase(null)]
        public void Widthdraw_QuickWidthdrawFails_ReturnsFalse(string? quickWidthdrawOption)
        {
            // Arrange
            var consoleMock = new Mock<IConsoleUserInput>();
            consoleMock.SetupSequence(c => c.GetUserInput()).Returns("1").Returns(quickWidthdrawOption);

            // Act
            var result = _atm.Widthdraw(consoleMock.Object);

            // Assert
            consoleMock.Verify(c => c.GetUserInput(), Times.Exactly(2));
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("")]
        [TestCase("asdad")]
        [TestCase(null)]
        [TestCase("0")]
        [TestCase("-1")]
        [TestCase("10")]
        [TestCase("20")]
        public void Widthdraw_StandardWidthdrawFails_ReturnsFalse(string? quickWidthdrawOption)
        {
            // Arrange
            var consoleMock = new Mock<IConsoleUserInput>();
            consoleMock.SetupSequence(c => c.GetUserInput()).Returns("2").Returns(quickWidthdrawOption);

            // Act
            var result = _atm.Widthdraw(consoleMock.Object);

            // Assert
            consoleMock.Verify(c => c.GetUserInput(), Times.Exactly(2));
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("1", "1")]
        [TestCase("1", "2")]
        [TestCase("1", "3")]
        [TestCase("2", "50")]
        [TestCase("2", "200")]
        [TestCase("2", "400")]
        [TestCase("2", "1")]
        public void Widthdraw_UserWidrawsMoney_ReturnsTrue(string withdrawOption, string? widthdrawAmount)
        {
            // Arrange
            var initialMoney = 500f;
            var consoleMock = new Mock<IConsoleUserInput>();
            consoleMock.SetupSequence(c => c.GetUserInput()).Returns(withdrawOption).Returns(widthdrawAmount);
            _atm.Deposit(initialMoney);
            _account.Deposit(initialMoney);
            _atm.Read(_debitCard);

             // Act
             var result = _atm.Widthdraw(consoleMock.Object);

            // Assert
            consoleMock.Verify(c => c.GetUserInput(), Times.Exactly(2));
            Assert.That(initialMoney, Is.Not.EqualTo(_account.Balance()));
            Assert.That(result, Is.True);
        }
    }
}
