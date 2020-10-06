using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features.Common;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = this.accountRepository.GetAccountById(fromAccountId);
            var to = this.accountRepository.GetAccountById(toAccountId);

            //These kinds of validations could be wrapped using FluentValidation, but for simplicity because its only one validation I decided to do it in here.
            //That being said, although it's only one validation we can already see code duplication in the other feature, which could go against clean code practices,
            //so in the end I would actually implement a common validation setup with fluent validation to avoid that, or another strategy to avoid code validation duplications.
            if (from == null || to == null) throw new InvalidOperationException($" The user with account id {fromAccountId} does not exist.");

            //If at any point any of these operations fails an exception is thrown and the changes are safely discarded.
            //But usually this would be wrapped within a unit of work.
            from.TryWithdrawn(amount);
            to.TryTransfer(amount);  
             
            this.accountRepository.Update(from);
            this.accountRepository.Update(to);

            //Notifications are only sent after we can confirm that the transactions happened successfully.
            //As explain in the account class, the prefered option, in my opinion, is to have a domain event dispatch this action using a mediator from inside the domain object.
            //Whatever strategy used the key point is to avoid adding any dependencies inside the domain object unless otherwise advised by the team.
            NotificationThresholds.SendIfLowFunds(notificationService, from);
            NotificationThresholds.SendIfLowFunds(notificationService, to);
            NotificationThresholds.SendIfNearPaidInLimit(notificationService, to);
        }
    }
}
