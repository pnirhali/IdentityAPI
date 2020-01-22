using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Microsoft.AspNetCore.Identity;
using IdentityAPI.Data;
using System.Threading.Tasks;
using IdentityAPI.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAPI.Controllers.Tests
{
    [TestClass()]
    public class AuthenticateControllerTests
    {
       
        public AuthenticateControllerTests()
        {

        }
        [TestMethod()]
        public async Task RegisterForExistingUserReturnsConflict()
        {
            var store = new Mock<IUserStore<AppUser>>();
            var _mockUserMgr = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);

            _mockUserMgr.Setup(userMgr => userMgr.FindByEmailAsync(It.IsAny<string>()))
                        .Returns(Task.FromResult(new AppUser()));

            var authenticateController = new AuthenticateController(_mockUserMgr.Object, null);
            var result = await authenticateController.Register(new RegistrationDTO());
            var conflitResult = result as ConflictObjectResult;
            Assert.AreEqual(conflitResult.StatusCode, "409");
        }
    }
}