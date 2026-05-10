using ApiBook.Application.DTOs;

namespace ApiBook.Application.Contracts;

public interface IRiskScoreEngine
{
    SecurityAnalysisResult Evaluate(SecurityRequestSnapshot request);
}
