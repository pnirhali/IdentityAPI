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
            Assert.AreEqual(Convert.ToString(conflitResult.StatusCode), "409");
        }

        [TestMethod()]
        public async Task RegisterCrossVerifingEmailToCreateUser()
        {
            var store = new Mock<IUserStore<AppUser>>();
            var _mockUserMgr = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);

            var appUser = new AppUser { Email = "poonamdhimate@gmail.com" };
            var registrationDTO = new RegistrationDTO { Email = "poonamdhimate@gmail.com", Password = "zaq1ZAQ!" };

            //Set up findByEmail
            _mockUserMgr.Setup(userMgr => userMgr.FindByEmailAsync(null))
                            .Returns(Task.FromResult<AppUser>(null));
            
            //Set up create and assert user enterered email is same as set in AppUser obj
            _mockUserMgr.Setup(userMgr => userMgr.CreateAsync(appUser, It.IsAny<string>()))
                .Returns(Task.FromResult(It.IsAny<IdentityResult>()))
                .Callback(() => Assert.AreEqual(appUser.Email, registrationDTO.Email));

            var authenticateController = new AuthenticateController(_mockUserMgr.Object, null);
            //Called Register method
            var result = await authenticateController.Register(registrationDTO);

            // if result is null or not succedded
            var badResult = result as BadRequestObjectResult;
            Assert.AreEqual(Convert.ToString(badResult.StatusCode), "400");
        }


    }
}