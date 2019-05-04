using EdityMcEditface.BuildTasks;
using EdityMcEditface.HtmlRenderer.Compiler;
using EdityMcEditface.HtmlRenderer.SiteBuilder;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Threax.AspNetCore.Tests;
using Xunit;

namespace EdityMcEditface.HtmlRenderer.Tests
{
    public class CreateIISWebConfigTests : IDisposable
    {
        private Mockup mockup = new Mockup();
        private RetainContentsStream outputStream = new RetainContentsStream();

        public CreateIISWebConfigTests()
        {
            mockup.Add<BuildEventArgs>(s => new BuildEventArgs(s.Get<IBuildStatusTracker>(), s.Get<ISiteBuilder>(), s.Get<BuilderUserInfo>()));
        }

        public void Dispose()
        {
            outputStream.Dispose();
        }

        [Fact]
        public void Direct()
        {
            MockDirectSiteBuilder();
            var taskDefinition = new BuildTaskDefinition();
            var fileName = $"{System.Reflection.MethodBase.GetCurrentMethod().Name}-web.config";
            TestConfig(taskDefinition, fileName);
        }

        [Fact]
        public void DirectWithHttpsRedirect()
        {
            MockDirectSiteBuilder();
            var taskDefinition = new BuildTaskDefinition()
            {
                Settings = new Dictionary<string, object>()
                {
                    { "redirectToHttps", true }
                }
            };
            var fileName = $"{System.Reflection.MethodBase.GetCurrentMethod().Name}-web.config";
            TestConfig(taskDefinition, fileName);
        }

        [Fact]
        public void DirectWithoutCache()
        {
            MockDirectSiteBuilder();
            var taskDefinition = new BuildTaskDefinition()
            {
                Settings = new Dictionary<string, object>()
                {
                    { "cacheControlMaxAge", "0.00:00:00" }
                }
            };
            var fileName = $"{System.Reflection.MethodBase.GetCurrentMethod().Name}-web.config";
            TestConfig(taskDefinition, fileName);
        }

        [Fact]
        public void DirectNonDefaultCache()
        {
            MockDirectSiteBuilder();
            var taskDefinition = new BuildTaskDefinition()
            {
                Settings = new Dictionary<string, object>()
                {
                    { "cacheControlMaxAge", "1.23:45:56" }
                }
            };
            var fileName = $"{System.Reflection.MethodBase.GetCurrentMethod().Name}-web.config";
            TestConfig(taskDefinition, fileName);
        }

        [Fact]
        public void RoundRobin()
        {
            MockRoundRobinSiteBuilder();
            var taskDefinition = new BuildTaskDefinition();
            var fileName = $"{System.Reflection.MethodBase.GetCurrentMethod().Name}-web.config";
            TestConfig(taskDefinition, fileName);
        }

        [Fact]
        public void RoundRobinWithHttpsRedirect()
        {
            MockRoundRobinSiteBuilder();
            var taskDefinition = new BuildTaskDefinition()
            {
                Settings = new Dictionary<string, object>()
                {
                    { "redirectToHttps", true }
                }
            };
            var fileName = $"{System.Reflection.MethodBase.GetCurrentMethod().Name}-web.config";
            TestConfig(taskDefinition, fileName);
        }

        private void TestConfig(BuildTaskDefinition taskDefinition, string fileName)
        {
            var args = mockup.Get<BuildEventArgs>();
            var createConfig = new CreateIISWebConfig(taskDefinition);
            createConfig.Execute(args);
            var configText = outputStream.Contents;
            FileUtils.WriteTestFile(typeof(CreateIISWebConfigTests), fileName, configText);
            var expected = FileUtils.ReadTestFile(typeof(CreateIISWebConfigTests), fileName);
            Assert.Equal(expected, configText);
        }

        private void MockDirectSiteBuilder()
        {
            mockup.Add<ISiteBuilder>(s =>
            {
                var mock = new Mock<ISiteBuilder>();
                mock.Setup(i => i.Project)
                    .Returns(() => new EdityProject());
                mock.Setup(i => i.OpenOutputParentWriteStream(It.IsAny<String>()))
                    .Returns(() => outputStream);
                return mock.Object;
            });
        }

        private void MockRoundRobinSiteBuilder()
        {
            mockup.Add<ISiteBuilder>(s =>
            {
                var mock = new Mock<ISiteBuilder>();
                mock.Setup(i => i.Project)
                    .Returns(() => new EdityProject());
                mock.Setup(i => i.OpenOutputParentWriteStream(It.IsAny<String>()))
                    .Returns(() => outputStream);
                mock.Setup(i => i.DeploymentSubFolder)
                    .Returns(() => "DeploymentSubFolder");
                return mock.Object;
            });
        }
    }
}
