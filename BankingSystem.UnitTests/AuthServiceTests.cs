using BankingSystem.Application.Services;
using BankingSystem.Contracts.Interfaces.IExternalServices;
using BankingSystem.Contracts.Interfaces.IRepositories;
using BankingSystem.Contracts.Interfaces;
using Moq;
using Microsoft.AspNetCore.Identity;
using BankingSystem.Contracts.DTOs.OnlineBank;
using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using BankingSystem.Contracts.DTOs.Auth;
using MockQueryable;

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

            var mockUserStore = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                null, null, null, null
            );

            _mockTokenService = new Mock<ITokenService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.RefreshTokenRepository).Returns(refreshTokenRepositoryMock.Object);
            _authService = new AuthService(_mockTokenService.Object, _mockUserManager.Object,
                _mockSignInManager.Object, _mockEmailService.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task LoginPersonAsync_ShouldLoginUser()
        {
            var loginDto = new LoginDTO { Username = "shamtam11@gmail.com", Password = "Paroli123.", DeviceId = "device-id-1" };
            var user = new IdentityUser { UserName = "shamtam11@gmail.com", Email = "shamtam11@gmail.com" };

            _mockUserManager.Setup(x => x.Users).Returns(new List<IdentityUser> { user }.AsQueryable().BuildMock());
            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, true)).ReturnsAsync(SignInResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
            _mockTokenService.Setup(x => x.GenerateAccessToken(user.Email, "User")).Returns("accessToken");
            _mockTokenService.Setup(x => x.GenerateRefreshToken(user.Id, loginDto.DeviceId)).Returns(new RefreshToken { Token = "refreshToken" });
            _mockUnitOfWork.Setup(x => x.RefreshTokenRepository.SaveRefreshTokenAsync(It.IsAny<RefreshToken>())).ReturnsAsync(true);

            var result = await _authService.LoginPersonAsync(loginDto);

            Assert.True(result.Success);
            Assert.Equal("Login successful!", result.Message);
            Assert.NotNull(result.Data);

            _mockUserManager.Verify(x => x.Users, Times.Once);
            _mockSignInManager.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            _mockUserManager.Verify(x => x.GetRolesAsync(It.IsAny<IdentityUser>()), Times.Once);
            _mockTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.RefreshTokenRepository.SaveRefreshTokenAsync(It.IsAny<RefreshToken>()), Times.Once);
        }

        [Fact]
        public async Task LoginPersonAsync_ShouldNotLoginUser()
        {
            var loginDto = new LoginDTO { Username = "tamar143", Password = "Paroli123.", DeviceId = "device-id-1" };
            var user = new IdentityUser { UserName = "shamtam11@gmail.com", Email = "shamtam11@gmail.com" };

            _mockUserManager.Setup(x => x.Users).Returns(new List<IdentityUser> { user }.AsQueryable().BuildMock());

            var result = await _authService.LoginPersonAsync(loginDto);

            Assert.False(result.Success);
            Assert.Equal("Invalid username!", result.Message);
            Assert.Null(result.Data);

            _mockUserManager.Verify(x => x.Users, Times.Once);
            _mockSignInManager.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
            _mockUserManager.Verify(x => x.GetRolesAsync(It.IsAny<IdentityUser>()), Times.Never);
            _mockTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockTokenService.Verify(x => x.GenerateRefreshToken(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.RefreshTokenRepository.SaveRefreshTokenAsync(It.IsAny<RefreshToken>()), Times.Never);
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
        public async Task ConfirmEmailAsync_ShouldConfirmEmail()
        {
            var emailConfirmationDto = new EmailConfirmationDTO { Email = "shtam11@gmail.com", Token = "token" };
            var user = new IdentityUser { Email = "shtam11@gmail.com" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(emailConfirmationDto.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, emailConfirmationDto.Token)).ReturnsAsync(IdentityResult.Success);

            var result = await _authService.ConfirmEmailAsync(emailConfirmationDto);

            Assert.True(result.Success);
            Assert.Equal("Email was successfully confirmed!", result.Message);
            Assert.Equal(200, result.StatusCode);

            _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.ConfirmEmailAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailAsync_ShouldNotConfirmEmail()
        {
            var emailConfirmationDto = new EmailConfirmationDTO { Email = "shtam11@gmail.com", Token = "token" };
            IdentityUser user = null;

            _mockUserManager.Setup(x => x.FindByEmailAsync(emailConfirmationDto.Email)).ReturnsAsync(user);

            var result = await _authService.ConfirmEmailAsync(emailConfirmationDto);

            Assert.False(result.Success);
            Assert.Equal("Email confirmation request is invalid, user was not found!", result.Message);
            Assert.Equal(404, result.StatusCode);

            _mockUserManager.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.ConfirmEmailAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldSendResetPasswordLink()
        {
            var forgotPasswordDTO = new ForgotPasswordDTO { Email = "shamugiatamar22@gmail.com", ClientUrl = "http://something.com" };
            var user = new IdentityUser { Email = "shamugiatamar22@gmail.com" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(forgotPasswordDTO.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>())).ReturnsAsync("resetToken");
            _mockEmailService.Setup(x => x.SendTokenEmailAsync("resetToken", forgotPasswordDTO.Email, forgotPasswordDTO.ClientUrl, "Reset password token")).Returns(Task.CompletedTask);

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDTO);

            Assert.True(result.Success);
            Assert.Equal("Check email to reset password!", result.Message);
            Assert.Equal(200, result.StatusCode);

            _mockUserManager.Verify(x => x.FindByEmailAsync(forgotPasswordDTO.Email), Times.Once);
            _mockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(user), Times.Once);
            _mockEmailService.Verify(x => x.SendTokenEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldNotSendResetPasswordLink()
        {
            var forgotPasswordDTO = new ForgotPasswordDTO { Email = "shamugiatamar22@gmail.com", ClientUrl = "http://something.com" };
            IdentityUser user = null;

            _mockUserManager.Setup(x => x.FindByEmailAsync(forgotPasswordDTO.Email)).ReturnsAsync(user);

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDTO);

            Assert.False(result.Success);
            Assert.Equal("User not found!", result.Message);
            Assert.Equal(404, result.StatusCode);

            _mockUserManager.Verify(x => x.FindByEmailAsync(forgotPasswordDTO.Email), Times.Once);
            _mockUserManager.Verify(x => x.GeneratePasswordResetTokenAsync(It.IsAny<IdentityUser>()), Times.Never);
            _mockEmailService.Verify(x => x.SendTokenEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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
        public async Task RefreshTokensAsync_ShouldReturnTokens()
        {
            var refreshTokensDto = new RefreshTokensDTO { AccessToken = "oldAccessToken", RefreshToken = "validRefreshToken", DeviceId = "device-123" };
            var existingRefreshToken = new RefreshToken { Token = "validRefreshToken", ExpirationDate = DateTime.UtcNow.AddMinutes(10), DeviceId = "device-123", IdentityUserId = "user-123", Id = 1 };

            var newAccessToken = "newAccessToken";
            var newRefreshToken = new RefreshToken { Token = "newRefreshToken" };

            _mockUnitOfWork.Setup(x => x.RefreshTokenRepository.GetRefreshTokenAsync(refreshTokensDto.RefreshToken)).ReturnsAsync(existingRefreshToken);
            _mockTokenService.Setup(x => x.RenewAccessToken(refreshTokensDto.AccessToken)).Returns(newAccessToken);
            _mockTokenService.Setup(x => x.GenerateRefreshToken(existingRefreshToken.IdentityUserId, refreshTokensDto.DeviceId)).Returns(newRefreshToken);
            _mockUnitOfWork.Setup(x => x.RefreshTokenRepository.SaveRefreshTokenAsync(newRefreshToken)).ReturnsAsync(true);
            _mockUnitOfWork.Setup(x => x.RefreshTokenRepository.DeleteRefreshTokenAsync(existingRefreshToken.Id)).ReturnsAsync(true);

            var result = await _authService.RefreshTokensAsync(refreshTokensDto);

            Assert.True(result.Success);
            Assert.Equal("New access token and refresh token retrieved!", result.Message);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);

            _mockUnitOfWork.Verify(x => x.RefreshTokenRepository.GetRefreshTokenAsync(refreshTokensDto.RefreshToken), Times.Once);
            _mockTokenService.Verify(x => x.RenewAccessToken(refreshTokensDto.AccessToken), Times.Once);
            _mockTokenService.Verify(x => x.GenerateRefreshToken(existingRefreshToken.IdentityUserId, refreshTokensDto.DeviceId), Times.Once);
            _mockUnitOfWork.Verify(x => x.RefreshTokenRepository.SaveRefreshTokenAsync(newRefreshToken), Times.Once);
            _mockUnitOfWork.Verify(x => x.RefreshTokenRepository.DeleteRefreshTokenAsync(existingRefreshToken.Id), Times.Once);
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
    }
}
