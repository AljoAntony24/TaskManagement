using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskManagement;
using TaskManagement.Controllers;
using TaskManagement.Data;
using TaskManagement.Model;
using TaskManagement.Services;

namespace TestTaskManagement.tests
{
    public class TaskServicesTest
    {
        [Fact]
        public void IsTaskCompleted_TaskIdEven_ReturnsTrue()
        {
            var service = new TaskServices();
            bool result = service.IsTaskCompleted(2);
            Assert.True(result);
        }
        [Fact]
        public void IsTaskCompleted_TaskIdOdd_ReturnsFalse()
        {
            var service = new TaskServices();

            bool result = service.IsTaskCompleted(3);

            Assert.False(result);
        }


        //private readonly Mock<DataCon> _dbContextMock;
        //private readonly Mock<JwtHandler> _jwtHandlerMock;
        //private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        //private readonly TaskManagement.Controllers.TaskController _controller;

        
        //public TaskController()
        //{
        //    var options = new DbContextOptionsBuilder<DataCon>()
        //            .UseInMemoryDatabase(databaseName: "TaskManagement_Db")
        //            .Options;

        //    _dbContextMock = new Mock<DataCon>(options);
        //    _jwtHandlerMock = new Mock<JwtHandler>();
        //    _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        //    _controller = new TaskManagement.Controllers.TaskController(_dbContextMock.Object, _jwtHandlerMock.Object, _httpContextAccessorMock.Object);
        //}

        //[Fact]
        //public async Task AddTask_ValidTaskName_ReturnsSuccessMessage()
        //{
        //    // Arrange
        //    var taskName = "Sample Task";
        //    var taskDetails = "Details here";

        //    var claims = new List<Claim>
        //    {
        //        new Claim("User_ID", "1"),
        //        new Claim("Role_Type", "Admin")
        //    };
        //    var identity = new ClaimsIdentity(claims, "TestAuth");
        //    var user = new ClaimsPrincipal(identity);

        //    var context = new DefaultHttpContext
        //    {
        //        User = user
        //    };

        //    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        //    // Act
        //    var result = await _controller.AddTask(taskName, taskDetails);

        //    // Assert
        //    var resObject = JsonConvert.DeserializeObject<dynamic>(result);
        //    Assert.Equal(200, (int)resObject.Status);
        //    Assert.Equal("Task Details Added Succsessfully", (string)resObject.Message);
        //}

        //[Fact]
        //public async Task AddTask_NullTaskName_ReturnsNotFound()
        //{
        //    // Act
        //    var result = await _controller.AddTask(null, "details");

        //    // Assert
        //    var resObject = JsonConvert.DeserializeObject<dynamic>(result);
        //    Assert.Equal(404, (int)resObject.Status);
        //    Assert.Equal("Enter The TaskName", (string)resObject.Message);
        //}
    }
}
