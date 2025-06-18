using AutoMapper;
using EchoB.Application.DTOs;
using EchoB.Application.DTOs.User;
using EchoB.Domain.Entities;
using EchoB.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.UseCases.Queries.Profile
{
    public class GetMyProfileQuery : IRequest<UserDto>
    {
    }
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, UserDto>
    {
        private readonly UserManager<EchoBUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetMyProfileQueryHandler> _logger;
        private readonly IMapper _mapper;


        public GetMyProfileQueryHandler(
            UserManager<EchoBUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetMyProfileQueryHandler> logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext?.User);
            if (userId == null)
               throw new UnauthorizedAccessException();
            

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException();

            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("User not found for ID: {UserId}", userId);
                throw new UserNotFoundException(userId);
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}
