namespace ATMSimpleSimulationTests
{
    [TestFixture]
    public class CardTests
    {
        Bank _bank;
        Account _account;
        ATM _atm;
        DebitCard _debitCard;
        //readonly string _cardPIN = "0000";

        [SetUp]
        public void Setup()
        {
            _bank = new Bank("TEST");
            _atm = new ATM(1, _bank, new string[] { "TEST" });
            _account = new Account(_bank, new Client("Test client", 23, "123-123-123"), "123123");
            _debitCard = new DebitCard(_account.Name, "2323", CARDTYPES.DEBIT, "2023/12/31", _account);
        }

        [Test]
        [TestCase(CARDTYPES.VISA)]
        [TestCase(CARDTYPES.JCB)]
        [TestCase(CARDTYPES.MASTERCARD)]
        public void Balance_GetCardBalanceCreditCard(CARDTYPES cardType)
        {
            // Arrange
            var credit = 500;
            var creditCard = new CreditCard(
                _account.Name,
                "2323",
                cardType,
                "2023/12/31",
                _account,
                credit,
                "123");

            // Act
            var result = creditCard.Balance();

            // Assert
            Assert.That(result, Is.EqualTo(credit));
        }

        [Test]
        public void Balance_GetCardBalanceDebitCard()
        {
            // Arrange
            _account.Deposit(500);

            // Act
            var result = _debitCard.Balance();

            // Assert
            Assert.That(result, Is.EqualTo(500));
        }

        [Test]
        [TestCase(CARDTYPES.VISA)]
        [TestCase(CARDTYPES.JCB)]
        [TestCase(CARDTYPES.MASTERCARD)]
        public void ChangePIN_FailsToChangeCreditCardPIN_ReturnsFalse(CARDTYPES cardType)
        {
            // Arrange
            var consoleMock = new Mock<IConsoleUserInput>();
            var creditCard = new CreditCard(
                _account.Name,
                "2323",
                cardType,
                "2023/12/31",
                _account,
                500,
                "123");

            // Act
            var result = creditCard.ChangePIN("4321", consoleMock.Object);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        [TestCase(CARDTYPES.VISA)]
        [TestCase(CARDTYPES.JCB)]
        [TestCase(CARDTYPES.MASTERCARD)]
        public void ChangePIN_ChangesCreditCardPIN_ReturnsTrue(CARDTYPES cardType)
        {
            // Arrange
            var consoleMock = new Mock<IConsoleUserInput>();
            consoleMock.Setup(c => c.GetUserInput()).Returns("1234");
            var creditCard = new CreditCard(
                _account.Name,
                "2323",
                cardType,
                "2023/12/31",
                _account,
                500,
                "123");

            // Act
            var result = creditCard.ChangePIN("0000", consoleMock.Object);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ChangePIN_FailsToChangeDebitCardPIN_ReturnsFalse()
        {
            // Arrange
            var consoleMock = new Mock<IConsoleUserInput>();
            //consoleMock.Setup(c => c.GetUserInput()).Returns("");

            // Act
            var result = _debitCard.ChangePIN("4321", consoleMock.Object);

            // Assert
            Assert.That(result, Is.False);
        }


        [Test]
        public void ChangePIN_ChangesDebitCardPIN_ReturnsTrue()
        {
            // Arrange
            var consoleMock = new Mock<IConsoleUserInput>();
            consoleMock.Setup(c => c.GetUserInput()).Returns("1234");

            // Act
            var result = _debitCard.ChangePIN("0000", consoleMock.Object);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase(1f)]
        [TestCase(20f)]
        [TestCase(50f)]
        [TestCase(100f)]
        public void Pay_UserFailsToPaysWithDebit_ReturnsFalse(float amountToPay)
        {
            // Arrange
            var consoleMock = new Mock<IConsoleUserInput>();
            consoleMock.Setup(c => c.GetUserInput()).Returns("1234");

            // Act
            var result = _debitCard.Pay(amountToPay, consoleMock.Object);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(_debitCard.Balance(), Is.EqualTo(0));
            });
        }

        [Test]
        [TestCase(1f)]
        [TestCase(20f)]
        [TestCase(50f)]
        [TestCase(100f)]
        public void Pay_UserPaysWithDebit_ReturnsTrue(float amountToPay)
        {
            // Arrange
            var consoleMock = new Mock<IConsoleUserInput>();
            consoleMock.Setup(c => c.GetUserInput()).Returns("0000");

            var balance = 101f;
            _account.Deposit(balance);


            // Act
            var result = _debitCard.Pay(amountToPay, consoleMock.Object);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(_debitCard.Balance(), Is.EqualTo(balance - amountToPay));
            });
        }

        [Test]
        [TestCase(1f)]
        [TestCase(20f)]
        [TestCase(50f)]
        [TestCase(100f)]
        public void Pay_UserFailsToPaysWithCredit_ReturnsFalse(float amountToPay)
        {
            // Arrange
            var availableCredit = 0;
            var creditCard = new CreditCard(
                _account.Name,
                "2323",
                CARDTYPES.VISA,
                "2023/12/31",
                _account,
                availableCredit,
                "123");

            var consoleMock = new Mock<IConsoleUserInput>();
            consoleMock.Setup(c => c.GetUserInput()).Returns("000");

            // Act
            var result = creditCard.Pay(amountToPay, consoleMock.Object);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(creditCard.Balance(), Is.EqualTo(availableCredit));
            });
        }

        [Test]
        [TestCase(1f)]
        [TestCase(20f)]
        [TestCase(50f)]
        [TestCase(100f)]
        public void Pay_UserPaysWithCredit_ReturnTrue(float amountToPay)
        {
            // Arrange
            var availableCredit = 101f;
            var creditCard = new CreditCard(
                _account.Name,
                "2323",
                CARDTYPES.VISA,
                "2023/12/31",
                _account,
                availableCredit,
                "123");

            var consoleMock = new Mock<IConsoleUserInput>();
            consoleMock.Setup(c => c.GetUserInput()).Returns("123");

            // Act
            var result = creditCard.Pay(amountToPay, consoleMock.Object);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(creditCard.Balance(), Is.EqualTo(availableCredit - amountToPay));
            });
        }

        [Test]
        [TestCase(11f)]
        [TestCase(20f)]
        [TestCase(50f)]
        [TestCase(100f)]
        public void Widthdraw_UserFailsToWidrawWithDebit_ReturnFalse(float withdrawAmount)
        {
            // Arrange
            var balance = 10f;
            _account.Deposit(balance);

            // Act
            var result = _debitCard.Widthdraw(withdrawAmount);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(_debitCard.Balance(), Is.EqualTo(balance));
            });
        }

        [Test]
        [TestCase(5f)]
        [TestCase(10f)]
        [TestCase(20f)]
        [TestCase(50f)]
        [TestCase(100f)]
        public void Widthdraw_UserWidrawsWithDebit_ReturnTrue(float withdrawAmount)
        {
            // Arrange
            var balance = 100f;
            _account.Deposit(balance);

            // Act
            var result = _debitCard.Widthdraw(withdrawAmount);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(_debitCard.Balance(), Is.EqualTo(balance - withdrawAmount));
            });
        }

        [Test]
        public void Widthdraw_UserFailsToWidrawWithCredit_ReturnFalse()
        {
            // Arrange
            var creditCard = new CreditCard(
                _account.Name,
                "2323",
                CARDTYPES.VISA,
                "2023/12/31",
                _account,
                200f,
                "123");
            var withdrawCredit = creditCard.GetAvailableWithdrawCredit();

            // Act
            var result = creditCard.Widthdraw(100f);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(creditCard.GetAvailableWithdrawCredit(), Is.EqualTo(withdrawCredit));
            });
        }

        [Test]
        [TestCase(CARDTYPES.VISA)]
        [TestCase(CARDTYPES.JCB)]
        [TestCase(CARDTYPES.MASTERCARD)]
        public void Widthdraw_UserWidrawsWithCredit_ReturnTrue(CARDTYPES cardType)
        {
            // Arrange
            var amountToWithdraw = 20f;
            var creditCard = new CreditCard(
                _account.Name,
                "2323",
                cardType,
                "2023/12/31",
                _account,
                200f,
                "123");
            var withdrawCredit = creditCard.GetAvailableWithdrawCredit();

            // Act
            var result = creditCard.Widthdraw(amountToWithdraw);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(creditCard.GetAvailableWithdrawCredit(), Is.EqualTo(withdrawCredit - amountToWithdraw));
            });
        }
    }
}
