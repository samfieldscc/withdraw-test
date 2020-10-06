using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m; 

        public Guid Id { get; set; }

        public User User { get; set; }

        //The object should be the only one to update the property. This ensures proper implementation of bussiness/domain logic
        public decimal Balance { get; private set; }


        //The object should be the only one to update the property. This ensures proper implementation of bussiness/domain logic
        public decimal Withdrawn { get; private set; }


        //The object should be the only one to update the property. This ensures proper implementation of bussiness/domain logic
        public decimal PaidIn { get; private set; }

        public Account(decimal balance, decimal withdrawn, decimal paidIn)
        {
            Balance = balance;
            Withdrawn = withdrawn;
            PaidIn = paidIn;
        }
       
        public void TryWithdrawn(decimal amount)
        { 
            if (amount < 0) throw new ArgumentOutOfRangeException("The amount must be positive.");

            this.SetBalance(OperationType.Subtract, amount);

            this.Withdrawn -= amount;
        } 

        public void TryTransfer(decimal amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException("The amount must be positive.");

            PaidIn += amount;

            if (PaidIn > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            //PAID IN LIMIT THRESHOLD
            //We could create a domain event with the help of a mediator to notify about this particular state and in a later stage
            //decide if he wanted to send the notification or not. I decided to do this in the feature layer because of time constraints.

            this.SetBalance(OperationType.Sum, amount);
        } 

        private void SetBalance(OperationType operation, decimal amount)
        {
            switch (operation)
            {
                case OperationType.Sum:
                    Balance += amount;
                    break;
                case OperationType.Subtract:
                    Balance -= amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"The {nameof(operation)} must be a valid operation.");
            }

            if (Balance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make widthdrawn.");
            }

            //LOW FUNDS THRESHOLD
            //We could create a domain event with the help of a mediator to notify about this particular state and in a later stage
            //decide if he wanted to send the notification or not. I decided to do this in the feature layer because of time constraints.
          
        }

        public void SetCurrentPaidIn(decimal amount)
        {
            PaidIn = amount;
        }
    }
}
