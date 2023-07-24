using System.Collections.Generic;
using System.Threading.Tasks;
using CAVerifierServer.CustomException;
using CAVerifierServer.Options;
using CAVerifierServer.VerifyCodeSender;
using Microsoft.Extensions.Options;
using Moq;
using Volo.Abp.Sms;

namespace CAVerifierServer.MsgSender;

public partial class VerifierCodeSmsSenderTests
{
    private IOptions<SmsServiceOptions> GetSmsServiceOptions()
    {
        var smsServiceDic = new Dictionary<string, SmsServiceOption>();
        smsServiceDic.Add("AWS", new SmsServiceOption
        {
            SupportingCountriesRatio = new Dictionary<string, int>
            {
                { "CN", 1 }
            },
        });
        smsServiceDic.Add("Telesign", new SmsServiceOption
        {
            SupportingCountriesRatio = new Dictionary<string, int>
            {
                { "CN", 2 }
            },
        });
        smsServiceDic.Add("Twilio", new SmsServiceOption
        {
            SupportingCountriesRatio = new Dictionary<string, int>
            {
                { "CN", 3 }
            },
        });
        smsServiceDic.Add("MockSmsServiceSender", new SmsServiceOption
        {
            SupportingCountriesRatio = new Dictionary<string, int>
            {
                { "CN", 4 }
            },
        });
        return new OptionsWrapper<SmsServiceOptions>(
            new SmsServiceOptions
            {
                SmsServiceInfos = smsServiceDic
            });
    }

    private ISMSServiceSender GetMockSmsServiceSender()
    {
        var mockSmsSender = new Mock<ISMSServiceSender>();
        mockSmsSender.Setup(o => o.ServiceName).Returns("MockSmsServiceSender");
        mockSmsSender.Setup(o => o.SendAsync(It.IsAny<SmsMessage>()))
            .Returns(Task.CompletedTask);
        return mockSmsSender.Object;
    }


    private IOptionsSnapshot<MobileCountryRegularCategoryOptions> GetMockMobileCountryRegularCategoryOptions()
    {
        var mockMobileCountryRegularCategoryOptions = new Mock<IOptionsSnapshot<MobileCountryRegularCategoryOptions>>();
        var list = new List<MobileInfo>
        {
            new MobileInfo
            {
                CountryCode = "+86",
                Country = "CN",
                MobileRegular = @"^(\\+?0?86\\-?)?1[3456789]\\d{9}$"
            }
        };
        mockMobileCountryRegularCategoryOptions.Setup(o => o.Value).Returns(
            new MobileCountryRegularCategoryOptions
            {
                MobileInfos = list
            });
        return mockMobileCountryRegularCategoryOptions.Object;
    }
}