using ATMSimpleSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ATMSimpleSimulationTests
{
    [TestFixture]
    public class AccountTests
    {
        Bank _bank;
        Account _account;

        [SetUp]
        public void Setup()
        {
            _bank = new Bank("TEST");
            _account = new Account(_bank, new Client("Test client", 23, "123-123-123"), "123123");
        }

        [Test]
        [TestCase(11f)]
        [TestCase(20f)]
        [TestCase(50f)]
        public void Widthdraw_CannotWithdraw_ReturnsFalse(float amountToWidthdraw)
        {
            // Arrange
            _account.Deposit(10f);

            // Act
            var result = _account.Widthdraw(amountToWidthdraw);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase(1f)]
        [TestCase(10f)]
        [TestCase(20f)]
        [TestCase(50f)]
        [TestCase(99f)]
        public void Widthdraw_CanWithdraw_ReturnsTrue(float amountToWidthdraw)
        {
            // Arrange
            var balance = 100f;
            _account.Deposit(balance);

            // Act
            var result = _account.Widthdraw(amountToWidthdraw);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(_account.Balance(), Is.EqualTo(balance - amountToWidthdraw));
            });
        }

        [Test]
        [TestCase("123")]
        [TestCase("43333")]
        [TestCase("144441234")]
        [TestCase("0")]
        [TestCase("456")]
        [TestCase("4das")]
        [TestCase("gfgf")]
        [TestCase("a1s2")]
        [TestCase("aaaa")]
        [TestCase("gfgffdfd")]
        public void ValidatePin_FailsToValidatePIN_ReturnsFalse(string pin)
        {
            // Act
            var result = _account.ValidatePin(pin);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("1234")]
        [TestCase("4321")]
        [TestCase("0000")]
        [TestCase("1111")]
        public void ValidatePin_ValidatesPIN_ReturnsTrue(string pin)
        {
            // Act
            var result = _account.ValidatePin(pin);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("0000", "1234")]
        [TestCase("00000", "1234")]
        public void ChangeDebitCardPing_FailsToChangePIN_ReturnsFalse(string oldPin, string newPin)
        {
            // Arrange
            var consoleMock = new Mock<IConsoleUserInput>();
            consoleMock.SetupSequence(c => c.GetUserInput()).Returns(newPin).Returns(oldPin);

            // Act
            var result = _account.ChangeDebitCardPin(oldPin, consoleMock.Object);
            
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.False);
                Assert.That(_account.PIN, Is.Not.EqualTo(newPin));
            });
        }

        [Test]
        [TestCase("0000", "1234")]
        [TestCase("0000", "4321")]
        [TestCase("0000", "4444")]
        public void ChangeDebitCardPing_SuccedesToChangePIN_ReturnsTrue(string oldPin, string newPin)
        {
            // Arrange
            var consoleMock = new Mock<IConsoleUserInput>();
            consoleMock.Setup(c => c.GetUserInput()).Returns(newPin);

            // Act
            var result = _account.ChangeDebitCardPin(oldPin, consoleMock.Object);

            // Assert
            consoleMock.Verify(c => c.GetUserInput(), Times.AtLeast(2));
            
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(_account.PIN, Is.EqualTo(newPin));
            });
        }

        [Test]
        public void HandlePhoneNumberChange_FailsToChange_ReturnsFalse()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }

        [Test]
        public void HandlePhoneNumberChange_SuccedesToChange_ReturnsTrue()
        {
            // Arrange

            // Act

            // Assert
            Assert.Fail();
        }
    }
}
