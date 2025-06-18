using AutoMapper;
using EchoB.Application.DTOs;
using EchoB.Application.DTOs.User;
using EchoB.Application.Interfaces;
using EchoB.Application.UseCases.Queries.Profile;
using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using EchoB.Domain.Interfaces;
using EchoB.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Commands.Account
{
    public class UpdateUserProfileCommand : IRequest<UserDto>
    {
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserDto>
    {
        private readonly IMapper _mapper;
        private readonly UserManager<EchoBUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateUserProfileCommandHandler> _logger;
        public UpdateUserProfileCommandHandler( IMapper mapper,
            UserManager<EchoBUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateUserProfileCommandHandler> logger)
        {
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<UserDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext?.User);
            if (userId == null)
                throw new UnauthorizedAccessException();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new UserNotFoundException(userId);

            FullName? fullName = null;
            Bio? bio = null;
            ProfilePictureUrl? profilePictureUrl = null;

            if (!string.IsNullOrWhiteSpace(request.FullName))
                fullName = new FullName(request.FullName);

            if (request.Bio != null)
                bio = new Bio(request.Bio);

            if (!string.IsNullOrWhiteSpace(request.ProfilePictureUrl))
                profilePictureUrl = new ProfilePictureUrl(request.ProfilePictureUrl);

            user.UpdateProfile(fullName, bio, profilePictureUrl, request.DateOfBirth);

            await _userManager.UpdateAsync(user);
            

            return _mapper.Map<UserDto>(user);
        }
    }
}

