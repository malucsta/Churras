﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Bbqs.Errors;
using Domain.Bbqs.Events;
using Domain.Common;
using Domain.People;
using Domain.People.Events;
using FluentResults;

namespace Domain.Bbqs
{
    public class Bbq : AggregateRoot
    {
        public string Reason { get; set; } = string.Empty;
        public BbqStatus Status { get; set; }
        public DateTime Date { get; set; }
        public bool IsTrincasPaying { get; set; }
        public int NumberOfConfirmations { get; set; } = 0;
        public Dictionary<string, ShoppingList> ShoppingList { private get; set; } 
            = new Dictionary<string, ShoppingList>();

        public Result When(ThereIsSomeoneElseInTheMood @event)
        {
            Id = @event.Id.ToString();
            Date = @event.Date;
            Reason = @event.Reason;
            Status = BbqStatus.New;

            return Result.Ok();
        }

        public Result When(BbqStatusUpdated @event)
        {
            if (@event.GonnaHappen)
                Status = BbqStatus.PendingConfirmations;
            else
                Status = BbqStatus.ItsNotGonnaHappen;

            if (@event.TrincaWillPay)
                IsTrincasPaying = true;

            return Result.Ok();
        }

        public Result When(InviteWasAccepted @event)
        {
            NumberOfConfirmations++;
            
            if (NumberOfConfirmations >= 7)
                Status = BbqStatus.Confirmed; 

            var vegetables = @event.IsVeg ? 0.60m : 0.30m;
            var meat = @event.IsVeg ? 0m : 0.30m;
            ShoppingList.Add(@event.PersonId, new ShoppingList { MeatInKilogram = meat, VegetablesInKilogram = vegetables });

            return Result.Ok();
        }

        public Result When(InviteWasDeclined @event)
        {
            var list = ShoppingList.FirstOrDefault(x => x.Key == @event.PersonId);
           
            if (list.Value is null) 
                return Result.Fail(new InviteNotFoundError(@event.PersonId));

            NumberOfConfirmations--;
             
            if (NumberOfConfirmations < 7)
                Status = BbqStatus.PendingConfirmations;

            ShoppingList.Remove(@event.PersonId);

            return Result.Ok();
        }

        public object GetShoppingList()
        {
            return new
            {
                Id,
                TotalMeatInKilogram = ShoppingList.Sum(x => x.Value.MeatInKilogram),
                TotalVegetablesInKilogram = ShoppingList.Sum(x => x.Value.VegetablesInKilogram)
            };
        }

        public object TakeSnapshot()
        {
            return new
            {
                Id,
                Date,
                IsTrincasPaying,
                Status = Status.ToString()
            };
        }
    }
}