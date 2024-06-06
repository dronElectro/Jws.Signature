﻿using Jws.Signature.Jws;
using Jws.Signature.Signing;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Jws.Signature.Tests;

public class MakeJwsIntegrationTests
{
    private readonly ITestOutputHelper _outputHelper;

    public MakeJwsIntegrationTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    private ISignDataService GetSignDataService()
    {
        var privateKey =
            "-----BEGIN RSA PRIVATE KEY-----\n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\n-----END RSA PRIVATE KEY-----";

        var signDataRsaService = new SignDataRsaService(privateKey);
        return signDataRsaService;
    }

    private IVerifySignService GetVerifySignService()
    {
        var publicKey =
            "-----BEGIN RSA PUBLIC KEY-----\nMIICCgKCAgEAvL/hvAGJH9sRNGbR2lqk2Y70UMM4hfeA+SC5dHxOADeKYbwKANpDxbjYsRYGVVJaAz+cqVCEYQF59LtQKy6+Ux0qeavE42zlE1ewRZnQnT00ZS5585p3fW24pizyCC2Q/ddt0qGa0FPMrGzqo/8OGbIbScwYgI/UAALzb4fbSGcVNVxw0MtUXhncrML16fS7j9SXOYwNjoI2MmzossVsP8LGOQUF9irxEbjAFaLwciWSV/aCs39J65SdhDyv53eARRYzXtdsAXQ1hvzot2QVl4ScSENJWFyEIfWOIj2MIw4GjmRO4ufQfhM54mMGruGPBhy6UOPEyyvRzWdvAlVV4VHh6lyZhzXNMgw/sCAFwex9kf/X3HDehiC+AXHM9TvWljm6ngiZardtO68BuFssuRbotNxyb8K/+D2zU7iF5P4lwapZHiFMrCTSuHXvEhvXBE2vY+h+InrQdiuEt4JEptwvrfACdW6X6UmhwsKdQh5EHD4wE4EL6BL+VL8ddp8q2rs0d5rQyK3NlYPfOkQAXmc/rl71NKgebL8SE4XdtbIn9Bn006/a22DzEh7e0vdwJof1DD2m0Apz/Ue3sJjrz0XlktNw+kTq09NyN1sz/T6edHCAnK7gkPzKYM10SarMasFqwGVfSSehhNzsuJPdsogpDMsY2zP6l59/e7A5KkUCAwEAAQ==\n-----END RSA PUBLIC KEY-----";

        var verifySignService = new VerifySignService(publicKey);
        return verifySignService;
    }

    [Fact]
    public void MakeJwsTest()
    {
        var testEntity = new TestEntity { Id = 2, Name = "test entity 22", Description = "second test" };

        var makeJwsService = new MakeJwsService(GetSignDataService());

        var jws = makeJwsService.MakeJws(testEntity, "RS256");

        _outputHelper.WriteLine(jws);

        var parseJwsService = new ParseJwsService(GetVerifySignService());

        var parsedEntity = parseJwsService.ParseJws<TestEntity>(jws);

        Assert.Equal(testEntity.Id, parsedEntity.Id);
        Assert.Equal(testEntity.Name, parsedEntity.Name);
        Assert.Equal(testEntity.Description, parsedEntity.Description);

        _outputHelper.WriteLine(parsedEntity.ToString());
    }
}