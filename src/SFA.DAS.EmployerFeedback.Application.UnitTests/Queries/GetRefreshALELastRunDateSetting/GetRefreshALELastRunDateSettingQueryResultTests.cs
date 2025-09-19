using SFA.DAS.EmployerFeedback.Application.Queries.GetRefreshALELastRunDateSetting;
using NUnit.Framework;
using System;
using System.Globalization;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetRefreshALELastRunDateSetting
{
    public class GetRefreshALELastRunDateSettingQueryResultTests
    {
        [Test]
        public void CanInstantiate_GetRefreshALELastRunDateSettingQueryResult()
        {
            var nowString = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            var result = new GetRefreshALELastRunDateSettingQueryResult { Value = nowString };
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(nowString));
        }
    }
}
