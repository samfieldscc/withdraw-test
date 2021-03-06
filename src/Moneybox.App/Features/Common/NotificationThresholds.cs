﻿using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;

namespace Moneybox.App.Features.Common
{
    public static class NotificationThresholds
    {
        public static void SendIfLowFunds(INotificationService notificationService, Account account)
        {
            if(account.Balance < 500m)
            {
                notificationService.NotifyFundsLow(account.User.Email);
            }
        }

        public static void SendIfNearPaidInLimit(INotificationService notificationService, Account account)
        {
            if (Account.PayInLimit - account.PaidIn < 500m)
            {
                notificationService.NotifyApproachingPayInLimit(account.User.Email);
            }
        }
    }
}
