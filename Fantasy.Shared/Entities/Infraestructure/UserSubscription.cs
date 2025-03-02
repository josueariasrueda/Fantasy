using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Shared.Entities.Infraestructure;

public class UserSubscription
{
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    public Guid SubscriptionId { get; set; }
    public Subscription Subscription { get; set; } = null!;
}