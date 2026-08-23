using FluentAssertions;
using TaskFlow.Application.Tasks;

namespace TaskFlow.Tests.Tasks;

public class PaginationTests
{
    [Fact]
    public void Page_ShouldDefaultToOne()
    {
        var query = new TaskQueryParameters();

        query.Page.Should().Be(1);
    }

    [Fact]
    public void PageSize_ShouldDefaultToTwenty()
    {
        var query = new TaskQueryParameters();

        query.PageSize.Should().Be(20);
    }

    [Fact]
    public void PageSize_ShouldNotExceedOneHundred()
    {
        var query = new TaskQueryParameters
        {
            PageSize = 500
        };

        query.PageSize.Should().Be(100);
    }

    [Fact]
    public void NegativePage_ShouldBecomeOne()
    {
        var query = new TaskQueryParameters
        {
            Page = -5
        };

        query.Page.Should().Be(1);
    }
}
