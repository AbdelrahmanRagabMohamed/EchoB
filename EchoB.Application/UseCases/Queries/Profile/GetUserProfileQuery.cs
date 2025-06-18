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
    public class GetUserProfileQuery : IRequest<UserDto>
    {
        public string UserId { get; set; }
    }
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserDto>
    {
        private readonly UserManager<EchoBUser> _userManager;
        private readonly ILogger<GetUserProfileQuery> _logger;
        private readonly IMapper _mapper;

        public GetUserProfileQueryHandler(
            UserManager<EchoBUser> userManager,
            ILogger<GetUserProfileQuery> logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
           
            if (request.UserId == null )
            {
                _logger.LogWarning("User ID is empty.");
                return null!;
            }

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("User not found for ID: {UserId}", request.UserId);
                throw new UserNotFoundException(request.UserId);
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}
