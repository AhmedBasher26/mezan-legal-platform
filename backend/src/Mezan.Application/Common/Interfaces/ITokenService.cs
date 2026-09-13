using Mezan.Domain.Entities;

namespace Mezan.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(Lawyer lawyer);
}
