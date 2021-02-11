using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Tasklify.Interfaces;

namespace takehome.tests
{
    
    public class UsersControllerTests
    {
        [Fact]
        public void TestGetAll()
        {
            Mock<TaskRepository> mockTaskRepository = new Mock<TaskRepository>();
        }
    }
}
