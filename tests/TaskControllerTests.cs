using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasklify.Contracts;
using Tasklify.Controllers;
using Tasklify.Interfaces;
using Xunit;

namespace takehome.tests
{
    public class TaskControllerTests
    {
        [Fact]
        public async void TestGetAll()
        {
            Mock<TaskRepository> mockTaskRepository = new Mock<TaskRepository>();
            Mock<IUsersDAL> mockUsersDAL = new Mock<IUsersDAL>();

            mockTaskRepository.Setup(x => x.GetAllAsync()).Returns(TestDataHelper.GetTestGetAllTask());

            TasksController controller = new TasksController(mockUsersDAL.Object, mockTaskRepository.Object);

            var getAPIResult = (ActionResult) await controller.GetAll();

            var getApiResultObj = getAPIResult as ObjectResult;
            Assert.NotNull(getApiResultObj);

            var contracts = (getApiResultObj.Value as IList<TasklifyTask>);

            Assert.Equal("I AM A SUMMARY", contracts[1].Summary);
            Assert.Equal("jk lol rofl 1337.", contracts[1].Description);
            Assert.Equal(0, contracts[1].Assignee);
            Assert.Equal(2, contracts[1].Id);
        }

        [Fact]
        public async void TestGetById()
        {
            Mock<TaskRepository> mockTaskRepository = new Mock<TaskRepository>();
            Mock<IUsersDAL> mockUsersDAL = new Mock<IUsersDAL>();

            mockTaskRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(TestDataHelper.GetTestGetById());

            TasksController controller = new TasksController(mockUsersDAL.Object, mockTaskRepository.Object);

            var getAPIResult = (ActionResult)await controller.GetById(1);

            var getApiResultObj = getAPIResult as ObjectResult;
            Assert.NotNull(getApiResultObj);

            var contract = (getApiResultObj.Value as TasklifyTask);

            Assert.Equal("I AM A SUMMARY", contract.Summary);
            Assert.Equal("jk lol rofl 1337.", contract.Description);
            Assert.Equal(0, contract.Assignee);
            Assert.Equal(1, contract.Id);
        }

        [Fact]
        public async void TestGetByEmptyId()
        {
            Mock<TaskRepository> mockTaskRepository = new Mock<TaskRepository>();
            Mock<IUsersDAL> mockUsersDAL = new Mock<IUsersDAL>();

            mockTaskRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult<TasklifyTask>(null));

            TasksController controller = new TasksController(mockUsersDAL.Object, mockTaskRepository.Object);

            var getAPIResult = (ActionResult)await controller.GetById(1);

            var getApiResultObj = getAPIResult as NotFoundObjectResult;
            Assert.Null(getApiResultObj);
        }

        [Fact]
        public async void TestPut()
        {
            Mock<TaskRepository> mockTaskRepository = new Mock<TaskRepository>();
            Mock<IUsersDAL> mockUsersDAL = new Mock<IUsersDAL>();

            mockTaskRepository.Setup(x => x.UpdateByIdAsync(It.IsAny<int>(),It.IsAny<TasklifyTask>())).Returns(TestDataHelper.GetTestTaskForEditAfterUpdate());

            TasksController controller = new TasksController(mockUsersDAL.Object, mockTaskRepository.Object);

            var getAPIResult = (ActionResult)await controller.UpdateById(1, TestDataHelper.GetTestTaskForEdit());

            var getApiResultObj = getAPIResult as OkObjectResult;
            Assert.NotNull(getApiResultObj);

            var contract = getApiResultObj.Value as TasklifyTask;

            Assert.Equal("SUMMARY Updated", contract.Summary);
            Assert.Equal("Description Updated", contract.Description);
            Assert.Equal(1, contract.Assignee);
            Assert.Equal(1, contract.Id);
        }
        
        [Fact]
        public async void TestPutNotFound()
        {
            Mock<TaskRepository> mockTaskRepository = new Mock<TaskRepository>();
            Mock<IUsersDAL> mockUsersDAL = new Mock<IUsersDAL>();

            mockTaskRepository.Setup(x => x.UpdateByIdAsync(It.IsAny<int>(),It.IsAny<TasklifyTask>())).Returns(Task.FromException<TasklifyTask>(new Exception(TestDataHelper.PutExceptionMessage())));

            TasksController controller = new TasksController(mockUsersDAL.Object, mockTaskRepository.Object);

            var getAPIResult = (ActionResult)await controller.UpdateById(5, TestDataHelper.GetTestTaskForEdit());

            var getApiResultObj = getAPIResult as ObjectResult;
            Assert.NotNull(getApiResultObj);

            var exceptionMessage = getApiResultObj.Value;
            var statusCode = getApiResultObj.StatusCode;

            Assert.Equal(503, statusCode);
            Assert.Equal("Error while updating task with \r\nid:5. \r\nException:Error while updating task with id:5", exceptionMessage);
        }

        [Fact]
        public async void TestPost()
        {
            Mock<TaskRepository> mockTaskRepository = new Mock<TaskRepository>();
            Mock<IUsersDAL> mockUsersDAL = new Mock<IUsersDAL>();

            mockTaskRepository.Setup(x => x.AddAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(TestDataHelper.GetTestTaskForAddSuccess());
            mockUsersDAL.Setup(x => x.GetUsersAsync()).Returns(TestDataHelper.GetTestGetAllUser());

            TasksController controller = new TasksController(mockUsersDAL.Object, mockTaskRepository.Object);

            var getAPIResult = (ActionResult)await controller.Add(TestDataHelper.GetTestTaskForAdd());

            var getApiResultObj = getAPIResult as OkObjectResult;
            Assert.NotNull(getApiResultObj);

            var contract = getApiResultObj.Value as TasklifyTask;

            Assert.Equal("SUMMARY Add", contract.Summary);
            Assert.Equal("Description Add", contract.Description);
            Assert.Equal(1, contract.Assignee);
            Assert.Equal(1, contract.Id);
        }

        [Fact]
        public async void TestPostEmptySummary()
        {
            Mock<TaskRepository> mockTaskRepository = new Mock<TaskRepository>();
            Mock<IUsersDAL> mockUsersDAL = new Mock<IUsersDAL>();

            mockTaskRepository.Setup(x => x.AddAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromException<TasklifyTask>(new Exception("")));
            mockUsersDAL.Setup(x => x.GetUsersAsync()).Returns(TestDataHelper.GetTestGetAllUser());

            TasksController controller = new TasksController(mockUsersDAL.Object, mockTaskRepository.Object);

            var getAPIResult = (ActionResult)await controller.Add(TestDataHelper.GetTestTaskForAddNoSummary());

            var getApiResultObj = getAPIResult as ObjectResult;
            Assert.NotNull(getApiResultObj);

            var exceptionMessage = getApiResultObj.Value;
            var statusCode = getApiResultObj.StatusCode;

            Assert.Equal(400, statusCode);
            Assert.Equal("Error while creating new task with \r\nSummary: \r\nDescription . \r\nException:Task Summary cannot be empty.", exceptionMessage);            
        }

        [Fact]
        public async void TestPostLongSummaryDescription()
        {
            Mock<TaskRepository> mockTaskRepository = new Mock<TaskRepository>();
            Mock<IUsersDAL> mockUsersDAL = new Mock<IUsersDAL>();

            mockTaskRepository.Setup(x => x.AddAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromException<TasklifyTask>(new Exception("")));
            mockUsersDAL.Setup(x => x.GetUsersAsync()).Returns(TestDataHelper.GetTestGetAllUser());

            TasksController controller = new TasksController(mockUsersDAL.Object, mockTaskRepository.Object);

            var getAPIResult = (ActionResult)await controller.Add(TestDataHelper.GetTestTaskForAddLongSummaryAndDescription());

            var getApiResultObj = getAPIResult as ObjectResult;
            Assert.NotNull(getApiResultObj);

            var exceptionMessage = getApiResultObj.Value;
            var statusCode = getApiResultObj.StatusCode;

            Assert.Equal(400, statusCode);
            Assert.Equal("Error while creating new task with \r\nSummary:SUMMARY Add SUMMARY Add SUMMARY Add SUMMARY AddSUMMARY AddSUMMARY Add SUMMARY AddSUMMARY Add SUMMARY Add SUMMARY Add SUMMARY Add SUMMARY Add \r\nDescription Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add Description Add. \r\nException:Task Summary should be 100 characters or less. Task Description should be 500 characters or less.", exceptionMessage);
        }

        [Fact]
        public async void TestPostInValidAssihnee ()
        {
            Mock<TaskRepository> mockTaskRepository = new Mock<TaskRepository>();
            Mock<IUsersDAL> mockUsersDAL = new Mock<IUsersDAL>();

            mockTaskRepository.Setup(x => x.AddAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromException<TasklifyTask>(new Exception("")));
            mockUsersDAL.Setup(x => x.GetUsersAsync()).Returns(TestDataHelper.GetTestGetAllUser());

            TasksController controller = new TasksController(mockUsersDAL.Object, mockTaskRepository.Object);

            var getAPIResult = (ActionResult)await controller.Add(TestDataHelper.GetTestTaskForAddInvalidAssignee());

            var getApiResultObj = getAPIResult as ObjectResult;
            Assert.NotNull(getApiResultObj);

            var exceptionMessage = getApiResultObj.Value;
            var statusCode = getApiResultObj.StatusCode;

            Assert.Equal(400, statusCode);
            Assert.Equal("Error while creating new task with \r\nSummary:SUMMARY Add  \r\nDescription Description Add . \r\nException: Task Assignee is not valid User.", exceptionMessage);
        }

        [Fact]
        public async void TestDelete()
        {
            Mock<TaskRepository> mockTaskRepository = new Mock<TaskRepository>();
            Mock<IUsersDAL> mockUsersDAL = new Mock<IUsersDAL>();

            mockTaskRepository.Setup(x => x.RemoveByIdAsync(It.IsAny<int>())).Returns(Task.FromResult<bool>(true));
            //mockUsersDAL.Setup(x => x.GetUsersAsync()).Returns(TestDataHelper.GetTestGetAllUser());

            TasksController controller = new TasksController(mockUsersDAL.Object, mockTaskRepository.Object);

            var getAPIResult = (ActionResult)await controller.DeleteById(1);

            var getApiResultObj = getAPIResult as NoContentResult;
            Assert.NotNull(getApiResultObj);

            var statusCode = getApiResultObj.StatusCode;

            Assert.Equal(204, statusCode);
        }
    }
}
