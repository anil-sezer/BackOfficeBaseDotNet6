﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BackOfficeBase.Application.Authorization.Permissions;
using BackOfficeBase.Domain.AppConsts.Authorization;
using BackOfficeBase.Domain.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace BackOfficeBase.Tests.Application.Authorization
{
    public class PermissionAppServiceTests : AppServiceTestBase
    {
        private readonly IPermissionAppService _permissionAppService;
        private static readonly string TestPermissionClaimForUser = "TestPermissionClaimForUser";
        private static readonly string TestPermissionClaimForRole = "TestPermissionClaimForRoe";
        private readonly User _testUser = GetTestUser();
        private readonly Role _testRole = GetTestRole();

        public PermissionAppServiceTests()
        {
            AddUserToRole(_testUser, _testRole);

            var mockUserClaimStore = SetupMockUserClaimStore(_testUser);
            var mockRoleClaimStore = SetupMockRoleClaimStore(_testRole);

            var userManager = new UserManager<User>(mockUserClaimStore.Object, null, null, null, null, null, null, null, null);
            var roleManager = new RoleManager<Role>(mockRoleClaimStore.Object, null, null, null, null);
            _permissionAppService = new PermissionAppService(userManager, roleManager);
        }

        [Fact]
        public async Task Should_Permission_Granted_To_User()
        {
            var isPermissionGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync(_testUser.UserName, TestPermissionClaimForUser);

            Assert.True(isPermissionGranted);
        }

        [Fact]
        public async Task Should_Permission_Granted_To_User_Role()
        {
            var isPermissionGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync(_testUser.UserName, TestPermissionClaimForRole);

            Assert.True(isPermissionGranted);
        }

        [Fact]
        public async Task Should_Not_Permission_Granted_To_User()
        {
            var isPermissionNotGranted =
                await _permissionAppService.IsUserGrantedToPermissionAsync(_testUser.UserName, "NotGrantedPermissionClaim");

            Assert.False(isPermissionNotGranted);
        }

        private static Mock<IRoleClaimStore<Role>> SetupMockRoleClaimStore(Role testRole)
        {
            var mockRoleClaimStore = new Mock<IRoleClaimStore<Role>>();
            mockRoleClaimStore.Setup(x => x.GetClaimsAsync(testRole, CancellationToken.None)).ReturnsAsync(
                new List<Claim>
                {
                    new Claim(CustomClaimTypes.Permission, TestPermissionClaimForRole)
                });

            return mockRoleClaimStore;
        }

        private static Mock<IUserClaimStore<User>> SetupMockUserClaimStore(User testUser)
        {
            var mockUserClaimStore = new Mock<IUserClaimStore<User>>();
            mockUserClaimStore.Setup(x => x.FindByNameAsync(testUser.UserName, CancellationToken.None)).ReturnsAsync(testUser);
            mockUserClaimStore.Setup(x => x.GetClaimsAsync(testUser, CancellationToken.None)).ReturnsAsync(
                new List<Claim>
                {
                    new Claim(CustomClaimTypes.Permission, TestPermissionClaimForUser)
                });

            return mockUserClaimStore;
        }
    }
}
