﻿using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features.Common;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        //Since these properties are only used for reading, it's safe to declare them as readonly
        //I refactored the private variable to use a _ prefix. This kind of modifications is usually decided by the team,
        //the most important factor is to be consistent.
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this._accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            this._notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            var from = this._accountRepository.GetAccountById(fromAccountId);

            //These kinds of validations could be wrapped using FluentValidation, but for simplicity because its only one validation I decided to do it in here.
            //That being said, although it's only one validation we can already see code duplication in the other feature, which could go against clean code practices,
            //so in the end I would actually implement a common validation setup with fluent validation to avoid that, or another strategy to avoid code validation duplications.
            if (from == null) 
                throw new InvalidOperationException($" The user with account id {fromAccountId} does not exist.");
           
            from.TryWithdrawn(amount);

            this._accountRepository.Update(from);

            //Notifications are only sent after we can confirm that the transactions happened successfully.
            //As explain in the account class, the preferred option, in my opinion, is to have a domain event dispatch this action using a mediator from inside the domain object.
            //Whatever strategy used the key point is to avoid adding any dependencies inside the domain object unless otherwise advised by the team.
            NotificationThresholds.SendIfLowFunds(_notificationService, from);
        }
    }
}
