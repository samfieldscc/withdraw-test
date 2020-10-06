using Xunit; 
using System;

namespace Moneybox.App.UnitTests
{ 
    public class AccountOperationShould
    { 
        [Fact]
        public void ThrowAnExceptionForNegativeAmmountInWithdrawn()
        {
            var account = new Account(1000m, 0m, 0m);

            Assert.Throws<ArgumentOutOfRangeException>(() => { account.TryWithdrawn(-500m); });
        }

        [Fact]
        public void ThrowAnExceptionForInsuficientFundsInWithdrawn()
        {
            var account = new Account(1000m, 0m, 0m);

            Assert.Throws<InvalidOperationException>(() => { account.TryWithdrawn(account.Balance + 1m); });             
        }

        [Fact]
        public void ThrowAnExceptionForNegativeAmmountInTransfer()
        {
            var account = new Account(1000m, 0m, 0m);

            Assert.Throws<ArgumentOutOfRangeException>(() => { account.TryTransfer(-500m); });
        } 

        [Fact]
        public void ExecuteWidthdrawCorrectly()
        {
            var account = new Account(1000m, 0m, 0m);

            var withdrawnAmount = 500m;
            var previousBalance = account.Balance;
            var previousWithdrawn = account.Withdrawn;

            account.TryWithdrawn(withdrawnAmount); 
             
            Assert.Equal(previousBalance - withdrawnAmount, account.Balance);
            Assert.Equal(previousWithdrawn - withdrawnAmount, account.Withdrawn);
            Assert.Equal(0m, account.PaidIn);
        }

        [Fact]
        public void ExecuteTransferCorrectly()
        { 
            var account = new Account(1000m, 0m, 0m);

            var previousBalance = account.Balance;
            var previousPaidIn = account.PaidIn;

            account.TryTransfer(400m); 

            Assert.Equal(previousBalance + 400m, account.Balance);
            Assert.Equal(0m, account.Withdrawn);
            Assert.Equal(previousPaidIn + 400m, account.PaidIn);
        }

        [Fact]
        public void ThrowAnExceptionForPaidInLimitInTransfer()
        { 
            var account = new Account(5000m, 0m, Account.PayInLimit);
             
            Assert.Throws<InvalidOperationException>(() => { account.TryTransfer(2000m); });
        }
    }
}
