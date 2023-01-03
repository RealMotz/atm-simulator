
using ATMSimpleSimulation;

var bank = new Bank("ABC");
var allowedBanks = new string[] { "ABC" };
bank.AddAtm(new ATM(1, bank, allowedBanks));
bank.AddAtm(new ATM(2, bank));
bank.AddAtm(new ATM(3, bank));
bank.AddAtm(new ATM(4, bank));
bank.AddAtm(new ATM(5, bank));
var atm = bank.GetATM(serialId: 1);
if (atm == null)
{
    Console.WriteLine("ATM not found...");
    return;
}
atm.Deposit(1000);

var carlos = new Client("Carlos", 30, "080-4655-3445");

var newAccount = bank.CreateAccount(carlos);
newAccount.Deposit(400f);
var debitCard = bank.GenerateAccountDebitCard(newAccount);

carlos.Accounts.Add(newAccount);
carlos.Cards.Add(debitCard);

var atmSimulationClient = new ATMSimulator();
atmSimulationClient.SimulateATM(atm, debitCard);