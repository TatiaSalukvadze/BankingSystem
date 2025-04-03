using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Contracts.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Contracts.Interfaces.IServices;
using BankingSystem.Domain.Entities;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using BankingSystem.Contracts.DTOs.Auth;
using Newtonsoft.Json.Linq;

namespace BankingSystem.UnitTests
{
    public class AuthServiceTests
    {
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

            _mockTokenService = new Mock<ITokenService>();
            var mockUserStore = new Mock<IUserStore<IdentityUser>>();
            //var mockUserRoleStore = mockUserStore.As<IUserRoleStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                mockUserStore.Object,//new Mock<IUserStore<IdentityUser>>().Object,
                null, // IOptions<IdentityOptions>
                null, // IPasswordHasher<IdentityUser>
                null, // IEnumerable<IUserValidator<IdentityUser>>
                null, // IEnumerable<IPasswordValidator<IdentityUser>>
                null, // ILookupNormalizer
                null, // IdentityErrorDescriber
                null, // IServiceProvider
                null  //
            );
            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
                _mockUserManager.Object,//new Mock<IRoleStore<IdentityRole>>().Object,
                new Mock<IHttpContextAccessor>().Object,
                 new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                 null,//new Mock<IOptions<IdentityOptions>>().Object,
                 null,//new Mock<ILogger<SignInManager<IdentityUser>>>().Object,
                 null,//new Mock<IAuthenticationSchemeProvider>().Object
                 null
            );

            _mockEmailService = new Mock<IEmailService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.RefreshTokenRepository).Returns(refreshTokenRepositoryMock.Object);
            _authService = new AuthService(_mockTokenService.Object, _mockUserManager.Object,
                _mockSignInManager.Object, _mockEmailService.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task RegisterPersonAsync_ShouldRegisterPerson()
        {
            var registerPersonDto = new RegisterPersonDTO
            {
                Name = "Tatia",
                Surname = "Salu",
                IDNumber = "33001059000",
                Birthdate = DateTime.Now.AddYears(-23),
                Email = "t@gmail.com",
                Password = "TatiaSalu1.",
                ClientUrl = ""
            };
            var newUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = registerPersonDto.Email,
                Email = registerPersonDto.Email,
            };
            IdentityUser existingUser = null;
            string token = "token";
            _mockUserManager.Setup(um => um.FindByEmailAsync(registerPersonDto.Email)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerPersonDto.Password))
                .Callback((IdentityUser newIdentityUser, string _) => newIdentityUser.Id = newUser.Id)
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User")).Returns(Task.FromResult(IdentityResult.Success));
            _mockUserManager.Setup(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(token);
            _mockEmailService.Setup(e => e.SendTokenEmailAsync(token, registerPersonDto.Email, registerPersonDto.ClientUrl,
            "Email Confirmation Token")).Returns(Task.CompletedTask);

            var response = await _authService.RegisterPersonAsync(registerPersonDto);

            Assert.True(response.Success);
            Assert.Equal("User was registered successfully!", response.Message);
            Assert.Equal(newUser.Id, response.Data);
            Assert.Equal(200, response.StatusCode);

            _mockUserManager.Verify(um => um.FindByEmailAsync(registerPersonDto.Email), Times.Once());
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerPersonDto.Password), Times.Once());
            _mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"), Times.Once());
            _mockUserManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<IdentityUser>()), Times.Once());
            _mockEmailService.Verify(e => e.SendTokenEmailAsync(token, It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async Task RegisterPersonAsync_ShouldNotRegisterPerson()
        {
            var registerPersonDto = new RegisterPersonDTO
            {
                Name = "Tatia",
                Surname = "Salu",
                IDNumber = "33001059000",
                Birthdate = DateTime.Now.AddYears(-23),
                Email = "t@gmail.com",
                Password = "TatiaSalu1.",
                ClientUrl = ""
            };
            var newUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = registerPersonDto.Email,
                Email = registerPersonDto.Email,
            };
            IdentityUser existingUser = null;
            string token = "token";

            _mockUserManager.Setup(um => um.FindByEmailAsync(registerPersonDto.Email)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerPersonDto.Password))
                .ReturnsAsync(IdentityResult.Failed());



            var response = await _authService.RegisterPersonAsync(registerPersonDto);

            Assert.False(response.Success);
            Assert.Equal("Adding user in identity system failed!", response.Message);
            Assert.Null(response.Data);
            Assert.Equal(400, response.StatusCode);

            _mockUserManager.Verify(um => um.FindByEmailAsync(registerPersonDto.Email), Times.Once());
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerPersonDto.Password), Times.Once());
            _mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"), Times.Never());
            _mockUserManager.Verify(um => um.GenerateEmailConfirmationTokenAsync(It.IsAny<IdentityUser>()), Times.Never());
            _mockEmailService.Verify(e => e.SendTokenEmailAsync(token, It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldResetPassword()
        {
            var resetPasswordDto = new ResetPasswordDTO()
            {
                Password = "TatiaSalu1.",
                ConfirmPassword = "TatiaSalu1.",
                Email = "t@gmail.com",
                Token = "token"
            };
            var existingUser = new IdentityUser() { Email = resetPasswordDto.Email };
            _mockUserManager.Setup(um => um.FindByEmailAsync(resetPasswordDto.Email)).ReturnsAsync(existingUser);
            _mockUserManager.Setup(um => um.ResetPasswordAsync(It.IsAny<IdentityUser>(), resetPasswordDto.Token,
                resetPasswordDto.Password)).ReturnsAsync(IdentityResult.Success);

            var response = await _authService.ResetPasswordAsync(resetPasswordDto);

            Assert.True(response.Success);
            Assert.Equal("Password reset successful!", response.Message);
            Assert.Equal(200, response.StatusCode);

            _mockUserManager.Verify(um => um.FindByEmailAsync(resetPasswordDto.Email), Times.Once());
            _mockUserManager.Verify(um => um.ResetPasswordAsync(It.IsAny<IdentityUser>(), resetPasswordDto.Token,
                resetPasswordDto.Password), Times.Once());

        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldNotResetPasswordOfNonexistentUser()
        {
            var resetPasswordDto = new ResetPasswordDTO()
            {
                Password = "TatiaSalu1.",
                ConfirmPassword = "TatiaSalu1.",
                Email = "t@gmail.com",
                Token = "token"
            };
            IdentityUser existingUser = null;
            _mockUserManager.Setup(um => um.FindByEmailAsync(resetPasswordDto.Email)).ReturnsAsync(existingUser);


            var response = await _authService.ResetPasswordAsync(resetPasswordDto);

            Assert.False(response.Success);
            Assert.Equal("User not found!", response.Message);
            Assert.Equal(404, response.StatusCode);

            _mockUserManager.Verify(um => um.FindByEmailAsync(resetPasswordDto.Email), Times.Once());
            _mockUserManager.Verify(um => um.ResetPasswordAsync(It.IsAny<IdentityUser>(), resetPasswordDto.Token,
                resetPasswordDto.Password), Times.Never());

        }

        [Fact]
        public async Task LogoutAsync_ShouldLogout()
        {
            var logoutDto = new LogoutDTO()
            {
                RefreshToken = "token",
                DeviceId = "device_id_1"
            };
            string email = "t@gmail.com";
            var existingUser = new IdentityUser() { Id = Guid.NewGuid().ToString()};
            var refreshToken = new RefreshToken() { Id = 1, Token = logoutDto.RefreshToken, DeviceId = logoutDto.DeviceId,
            IdentityUserId = existingUser.Id };


            _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetRefreshTokenAsync(logoutDto.RefreshToken))
                .ReturnsAsync(refreshToken);
            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(existingUser);
            _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.DeleteRefreshTokenAsync(refreshToken.Id))
                .ReturnsAsync(true);

            var response = await _authService.LogoutAsync(logoutDto, email);

            Assert.True(response.Success);
            Assert.Equal("Logged out Successfully!", response.Message);
            Assert.Equal(200, response.StatusCode);

            _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once());
            _mockUnitOfWork.Verify(u => u.RefreshTokenRepository, Times.Exactly(2));

        }

        [Fact]
        public async Task LogoutAsync_ShouldNotLogoutWithInvalidToken()
        {
            var logoutDto = new LogoutDTO()
            {
                RefreshToken = "token",
                DeviceId = "device_id_1"
            };
            string email = "t@gmail.com";
            RefreshToken refreshToken = null;

            _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetRefreshTokenAsync(logoutDto.RefreshToken))
                .ReturnsAsync(refreshToken);

            var response = await _authService.LogoutAsync(logoutDto, email);

            Assert.False(response.Success);
            Assert.Equal("Provided refresh token is invalid!", response.Message);
            Assert.Equal(400, response.StatusCode);

            _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Never());
            _mockUnitOfWork.Verify(u => u.RefreshTokenRepository, Times.Once());

        }

        [Fact]
        public async Task RefreshTokensAsync_ShouldNot()
        {
            var refreshTokensDto = new RefreshTokensDTO()
            {
                AccessToken = "accessToken",
                RefreshToken = "refreshToken",
                DeviceId = "device_id_1"
            };

            var refreshToken = new RefreshToken()
            {
                Id = 1,
                Token = refreshTokensDto.RefreshToken,
                DeviceId = refreshTokensDto.DeviceId,
                ExpirationDate = DateTime.UtcNow.AddMinutes(20)
            };
            string newAccessToken = "newAccessToken";
            var newRefreshToken = new RefreshToken() { };

            _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.GetRefreshTokenAsync(refreshTokensDto.RefreshToken))
                .ReturnsAsync(refreshToken);
            _mockTokenService.Setup(t => t.RenewAccessToken(refreshTokensDto.AccessToken)).Returns(newAccessToken);
            _mockTokenService.Setup(t => t.GenerateRefreshToken(refreshToken.IdentityUserId, refreshTokensDto.DeviceId))
                .Returns(newRefreshToken);
            _mockUnitOfWork.Setup(u => u.RefreshTokenRepository.SaveRefreshTokenAsync(newRefreshToken)).ReturnsAsync(false);

            var response = await _authService.RefreshTokensAsync(refreshTokensDto);

            Assert.False(response.Success);
            Assert.Equal("Refresh Token was not saved!", response.Message);
            Assert.Equal(400, response.StatusCode);

            _mockTokenService.Verify(t => t.RenewAccessToken(It.IsAny<string>()), Times.Once());
            _mockTokenService.Verify(t => t.GenerateRefreshToken(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _mockUnitOfWork.Verify(u => u.RefreshTokenRepository, Times.Exactly(2));
        }

    }
}
