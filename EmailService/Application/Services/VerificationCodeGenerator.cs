using System.Text;
using EmailService.Domain.Interfaces;

namespace EmailService.Application.Services;

public class VerificationCodeGenerator : IVerificationCodeGenerator
{
    private const int CodeLength = 6;  
    private static readonly Random Random = new Random();

    public string Generate()
    {
        var code = new StringBuilder();
        
        for (var i = 0; i < CodeLength; i++)
        {
            code.Append(Random.Next(0, 10)); 
        }
        
        return code.ToString();
    }
}