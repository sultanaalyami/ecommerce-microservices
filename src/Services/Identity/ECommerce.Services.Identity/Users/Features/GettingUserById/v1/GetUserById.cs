using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Queries;
using ECommerce.Services.Identity.Shared.Extensions;
using ECommerce.Services.Identity.Shared.Models;
using ECommerce.Services.Identity.Users.Dtos;
using ECommerce.Services.Identity.Users.Dtos.v1;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Services.Identity.Users.Features.GettingUserById.v1;

public record GetUserById(Guid Id) : IQuery<UserByIdResponse>;

internal class GetUserByIdValidator : AbstractValidator<GetUserById>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("InternalCommandId is required.");
    }
}

internal class GetUserByIdHandler : IQueryHandler<GetUserById, UserByIdResponse>
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _mapper = mapper;
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
    }

    public async Task<UserByIdResponse> Handle(GetUserById query, CancellationToken cancellationToken)
    {
        Guard.Against.Null(query, nameof(query));

        var identityUser = await _userManager.FindUserWithRoleByIdAsync(query.Id);

        var identityUserDto = _mapper.Map<IdentityUserDto>(identityUser);

        return new UserByIdResponse(identityUserDto);
    }
}
