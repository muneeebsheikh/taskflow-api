using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Tests.Helpers;

public static class TestDbContextFactory
{
    public static TaskFlowDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TaskFlowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TaskFlowDbContext(options);
    }
}
