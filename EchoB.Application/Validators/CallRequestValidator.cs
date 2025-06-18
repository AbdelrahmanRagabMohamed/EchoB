using EchoB.Application.DTOs;
using EchoB.Application.DTOs.Call;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Application.Validators
{
    public class CallRequestValidator : AbstractValidator<CallRequestDto>
    {
        public CallRequestValidator()
        {
            RuleFor(x => x.ReceiverId).NotEmpty().WithMessage("Receiver ID is required");
        }
    }
}
