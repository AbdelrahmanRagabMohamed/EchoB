using EchoB.Application.DTOs;
using EchoB.Domain.Entities;
using PhoneNumbers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EchoB.Application.DTOs.Auth;

namespace EchoB.Application.UseCases.Queries.Contact
{
    public class MatchContactsQuery : IRequest<List<RegisteredContactDTO>>
    {
        public List<string> PhoneNumbers { get; set; } = new();
    }

    public class MatchContactsQueryHandler : IRequestHandler<MatchContactsQuery, List<RegisteredContactDTO>>
    {
        private readonly UserManager<EchoBUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MatchContactsQueryHandler> _logger;
        private readonly PhoneNumberUtil _phoneUtil = PhoneNumberUtil.GetInstance();

        public MatchContactsQueryHandler(
            UserManager<EchoBUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MatchContactsQueryHandler> logger)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<List<RegisteredContactDTO>> Handle(MatchContactsQuery request, CancellationToken cancellationToken)
        {
            if (request.PhoneNumbers == null || !request.PhoneNumbers.Any())
                return new List<RegisteredContactDTO>();

            var formattedNumbers = NormalizePhoneNumbers(request.PhoneNumbers);

            var currentUserId = Guid.Parse(_userManager.GetUserId(_httpContextAccessor.HttpContext?.User));

            var matchedUsers = await _userManager.Users
                .Where(u => formattedNumbers.Contains(u.PhoneNumber) && u.Id != currentUserId)
                .Select(u => new RegisteredContactDTO
                {
                    Id = u.Id,
                    PhoneNumber = u.PhoneNumber,
                    FullName = u.FullName,
                    ProfileImageUrl = u.ProfilePictureUrl
                })
                .ToListAsync(cancellationToken);

            return matchedUsers;
        }

        private List<string> NormalizePhoneNumbers(List<string> phoneNumbers, string defaultRegion = "EG")
        {
            var normalizedNumbers = new List<string>();

            foreach (var rawNumber in phoneNumbers)
            {
                try
                {
                    var parsedNumber = _phoneUtil.Parse(rawNumber, defaultRegion);

                    if (_phoneUtil.IsValidNumber(parsedNumber))
                    {
                        var formattedNumber = _phoneUtil.Format(parsedNumber, PhoneNumberFormat.E164);
                        normalizedNumbers.Add(formattedNumber);
                    }
                }
                catch (NumberParseException ex)
                {
                    _logger.LogWarning("Failed to parse phone number '{Phone}': {Error}", rawNumber, ex.Message);
                    continue;
                }
            }

            return normalizedNumbers;
        }
    }
}
