using System;
using System.Collections.Specialized;
using System.Web;
using NUnit.Framework;
using PerpetualEngine.Storage;

namespace Xamarin.Piwik.Tests
{
    [TestFixture()]
    public class TestActionBuffer
    {
        SimpleStorage storage = SimpleStorage.EditGroup(Guid.NewGuid().ToString());

        [Test()]
        public void TestOutbox()
        {
            var actions = new ActionBuffer(CreateParameters(), storage);
            actions.Add(CreateParameters());
            Assert.That(actions.Count, Is.EqualTo(1));

            var outbox = actions.CreateOutbox();
            Assert.That(outbox, Does.Contain("requests"));
            Assert.That(actions.Count, Is.EqualTo(1));

            actions.Add(CreateParameters());
            Assert.That(actions.Count, Is.EqualTo(2));

            actions.ClearOutbox();
            Assert.That(actions.Count, Is.EqualTo(1));

            outbox = actions.CreateOutbox();
            actions.ClearOutbox();
            Assert.That(actions.Count, Is.EqualTo(0));
        }

        NameValueCollection CreateParameters()
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["cdt"] = (DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString();
            return parameters;
        }

    }
}
