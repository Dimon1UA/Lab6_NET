using System;
using Xunit;

public class QueryHelperTests
{
    [Fact]
    public void GetPaidDeliveries_ShouldReturnCorrectResults()
    {
        // Arrange
        var deliveries = new List<Delivery>
        {
            new Delivery { IsPaid = true },
            new Delivery { IsPaid = false },
            new Delivery { IsPaid = true }
        };
        var queryHelper = new QueryHelper(deliveries);

        // Act
        var paidDeliveries = queryHelper.GetPaidDeliveries();

        // Assert
        Assert.Equal(2, paidDeliveries.Count());
    }
}
