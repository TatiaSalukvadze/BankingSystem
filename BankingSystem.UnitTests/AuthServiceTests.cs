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

namespace BankingSystem.UnitTests
{
    public class AuthServiceTests
    {
        private readonly Mock<ITokenService> _tokenService;
        private readonly Mock<UserManager<IdentityUser>> _userManager;
        private readonly Mock<SignInManager<IdentityUser>> _signInManager;
        private readonly Mock<IEmailService> _emailService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

            _tokenService = new Mock<ITokenService>();
            _userManager = new Mock<UserManager<IdentityUser>>();
            _signInManager = new Mock<SignInManager<IdentityUser>>();
            _emailService = new Mock<IEmailService>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _mockUnitOfWork.Setup(u => u.RefreshTokenRepository).Returns(refreshTokenRepositoryMock.Object);
            _authService = new AuthService(_tokenService.Object, _userManager.Object, _signInManager.Object,
                _emailService.Object, _mockUnitOfWork.Object);
        }

    }
}
