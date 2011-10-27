﻿// ReSharper disable RedundantUsingDirective

using System;
using System.IO;
using System.Linq;
using AutoMoq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.Model;
using NzbDrone.Core.Providers;
using NzbDrone.Core.Providers.Core;
using NzbDrone.Core.Repository;
using NzbDrone.Core.Repository.Quality;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Test.Common;

namespace NzbDrone.Core.Test.ProviderTests
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class SabProviderTest : TestBase
    {
        [Test]
        public void AddByUrlSuccess()
        {
            //Setup
            const string sabHost = "192.168.5.55";
            const int sabPort = 2222;
            const string apikey = "5c770e3197e4fe763423ee7c392c25d1";
            const string username = "admin";
            const string password = "pass";
            const SabnzbdPriorityType priority = SabnzbdPriorityType.Normal;
            const string category = "tv";


            var mocker = new AutoMoqer();

            var fakeConfig = mocker.GetMock<ConfigProvider>();
            fakeConfig.SetupGet(c => c.SabHost)
                .Returns(sabHost);
            fakeConfig.SetupGet(c => c.SabPort)
                .Returns(sabPort);
            fakeConfig.SetupGet(c => c.SabApiKey)
                .Returns(apikey);
            fakeConfig.SetupGet(c => c.SabUsername)
                .Returns(username);
            fakeConfig.SetupGet(c => c.SabPassword)
                .Returns(password);
            fakeConfig.SetupGet(c => c.SabTvPriority)
                .Returns(priority);
            fakeConfig.SetupGet(c => c.SabTvCategory)
                .Returns(category);

            mocker.GetMock<HttpProvider>(MockBehavior.Strict)
                .Setup(
                    s =>
                    s.DownloadString(
                        "http://192.168.5.55:2222/api?mode=addurl&name=http://www.nzbclub.com/nzb_download.aspx?mid=1950232&priority=0&pp=3&cat=tv&nzbname=This+is+an+Nzb&apikey=5c770e3197e4fe763423ee7c392c25d1&ma_username=admin&ma_password=pass"))
                .Returns("ok");

            //Act
            bool result = mocker.Resolve<SabProvider>().AddByUrl(
                "http://www.nzbclub.com/nzb_download.aspx?mid=1950232", "This is an Nzb");

            //Assert
            result.Should().BeTrue();
        }


        [Test]
        public void AddByUrlNewzbin()
        {
            //Setup
            const string sabHost = "192.168.5.55";
            const int sabPort = 2222;
            const string apikey = "5c770e3197e4fe763423ee7c392c25d1";
            const string username = "admin";
            const string password = "pass";
            const SabnzbdPriorityType priority = SabnzbdPriorityType.Normal;
            const string category = "tv";


            var mocker = new AutoMoqer();

            var fakeConfig = mocker.GetMock<ConfigProvider>();
            fakeConfig.SetupGet(c => c.SabHost)
                .Returns(sabHost);
            fakeConfig.SetupGet(c => c.SabPort)
                .Returns(sabPort);
            fakeConfig.SetupGet(c => c.SabApiKey)
                .Returns(apikey);
            fakeConfig.SetupGet(c => c.SabUsername)
                .Returns(username);
            fakeConfig.SetupGet(c => c.SabPassword)
                .Returns(password);
            fakeConfig.SetupGet(c => c.SabTvPriority)
                .Returns(priority);
            fakeConfig.SetupGet(c => c.SabTvCategory)
                .Returns(category);

            mocker.GetMock<HttpProvider>(MockBehavior.Strict)
                .Setup(
                    s =>
                    s.DownloadString(
                        "http://192.168.5.55:2222/api?mode=addid&name=6107863&priority=0&pp=3&cat=tv&nzbname=This+is+an+Nzb&apikey=5c770e3197e4fe763423ee7c392c25d1&ma_username=admin&ma_password=pass"))
                .Returns("ok");

            //Act
            bool result = mocker.Resolve<SabProvider>().AddByUrl(
                "http://www.newzbin.com/browse/post/6107863/nzb", "This is an Nzb");

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void AddByUrlError()
        {
            //Setup
            string sabHost = "192.168.5.55";
            int sabPort = 2222;
            string apikey = "5c770e3197e4fe763423ee7c392c25d1";
            string username = "admin";
            string password = "pass";
            var priority = SabnzbdPriorityType.Normal;
            string category = "tv";

            var mocker = new AutoMoqer();

            var fakeConfig = mocker.GetMock<ConfigProvider>();
            fakeConfig.SetupGet(c => c.SabHost)
                .Returns(sabHost);
            fakeConfig.SetupGet(c => c.SabPort)
                .Returns(sabPort);
            fakeConfig.SetupGet(c => c.SabApiKey)
                .Returns(apikey);
            fakeConfig.SetupGet(c => c.SabUsername)
                .Returns(username);
            fakeConfig.SetupGet(c => c.SabPassword)
                .Returns(password);
            fakeConfig.SetupGet(c => c.SabTvPriority)
                .Returns(priority);
            fakeConfig.SetupGet(c => c.SabTvCategory)
                .Returns(category);
            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadString(It.IsAny<String>()))
                .Returns("error");

            //Act
            var sabProvider = mocker.Resolve<SabProvider>();
            var result = sabProvider.AddByUrl("http://www.nzbclub.com/nzb_download.aspx?mid=1950232", "This is an nzb");

            //Assert
            Assert.IsFalse(result);
            ExceptionVerification.ExcpectedWarns(1);
        }

        [Test]
        public void IsInQueue_True()
        {
            //Setup
            string sabHost = "192.168.5.55";
            int sabPort = 2222;
            string apikey = "5c770e3197e4fe763423ee7c392c25d1";
            string username = "admin";
            string password = "pass";

            var mocker = new AutoMoqer();

            var fakeConfig = mocker.GetMock<ConfigProvider>();
            fakeConfig.SetupGet(c => c.SabHost)
              .Returns(sabHost);
            fakeConfig.SetupGet(c => c.SabPort)
                .Returns(sabPort);
            fakeConfig.SetupGet(c => c.SabApiKey)
                .Returns(apikey);
            fakeConfig.SetupGet(c => c.SabUsername)
                .Returns(username);
            fakeConfig.SetupGet(c => c.SabPassword)
                .Returns(password);

            mocker.GetMock<HttpProvider>(MockBehavior.Strict)
                .Setup(s => s.DownloadString("http://192.168.5.55:2222/api?mode=queue&output=xml&apikey=5c770e3197e4fe763423ee7c392c25d1&ma_username=admin&ma_password=pass"))
                .Returns(File.ReadAllText(@".\Files\Queue.xml"));

            //Act
            bool result = mocker.Resolve<SabProvider>().IsInQueue("Ubuntu Test");

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsInQueue_False_Empty()
        {
            //Setup
            string sabHost = "192.168.5.55";
            int sabPort = 2222;
            string apikey = "5c770e3197e4fe763423ee7c392c25d1";
            string username = "admin";
            string password = "pass";

            var mocker = new AutoMoqer();

            var fakeConfig = mocker.GetMock<ConfigProvider>();
            fakeConfig.SetupGet(c => c.SabHost)
                .Returns(sabHost);
            fakeConfig.SetupGet(c => c.SabPort)
                .Returns(sabPort);
            fakeConfig.SetupGet(c => c.SabApiKey)
                .Returns(apikey);
            fakeConfig.SetupGet(c => c.SabUsername)
                .Returns(username);
            fakeConfig.SetupGet(c => c.SabPassword)
                .Returns(password);

            mocker.GetMock<HttpProvider>(MockBehavior.Strict)
                .Setup(s => s.DownloadString("http://192.168.5.55:2222/api?mode=queue&output=xml&apikey=5c770e3197e4fe763423ee7c392c25d1&ma_username=admin&ma_password=pass"))
                .Returns(File.ReadAllText(@".\Files\QueueEmpty.xml"));

            //Act
            bool result = mocker.Resolve<SabProvider>().IsInQueue(String.Empty);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), ExpectedMessage = "API Key Incorrect")]
        public void IsInQueue_False_Error()
        {
            //Setup
            string sabHost = "192.168.5.55";
            int sabPort = 2222;
            string apikey = "5c770e3197e4fe763423ee7c392c25d1";
            string username = "admin";
            string password = "pass";

            var mocker = new AutoMoqer();

            var fakeConfig = mocker.GetMock<ConfigProvider>();
            fakeConfig.SetupGet(c => c.SabHost)
                .Returns(sabHost);
            fakeConfig.SetupGet(c => c.SabPort)
                .Returns(sabPort);
            fakeConfig.SetupGet(c => c.SabApiKey)
                .Returns(apikey);
            fakeConfig.SetupGet(c => c.SabUsername)
                .Returns(username);
            fakeConfig.SetupGet(c => c.SabPassword)
                .Returns(password);

            mocker.GetMock<HttpProvider>(MockBehavior.Strict)
                .Setup(s => s.DownloadString("http://192.168.5.55:2222/api?mode=queue&output=xml&apikey=5c770e3197e4fe763423ee7c392c25d1&ma_username=admin&ma_password=pass"))
                .Returns(File.ReadAllText(@".\Files\QueueError.xml"));


            //Act
            mocker.Resolve<SabProvider>().IsInQueue(String.Empty);
        }

        [Test]
        [TestCase(1, new[] { 2 }, "My Episode Title", QualityTypes.DVD, false, "My Series Name - 1x2 - My Episode Title [DVD]")]
        [TestCase(1, new[] { 2 }, "My Episode Title", QualityTypes.DVD, true, "My Series Name - 1x2 - My Episode Title [DVD] [Proper]")]
        [TestCase(1, new[] { 2 }, "", QualityTypes.DVD, true, "My Series Name - 1x2 -  [DVD] [Proper]")]
        [TestCase(1, new[] { 2, 4 }, "My Episode Title", QualityTypes.HDTV, false, "My Series Name - 1x2-1x4 - My Episode Title [HDTV]")]
        [TestCase(1, new[] { 2, 4 }, "My Episode Title", QualityTypes.HDTV, true, "My Series Name - 1x2-1x4 - My Episode Title [HDTV] [Proper]")]
        [TestCase(1, new[] { 2, 4 }, "", QualityTypes.HDTV, true, "My Series Name - 1x2-1x4 -  [HDTV] [Proper]")]
        public void sab_title(int seasons, int[] episodes, string title, QualityTypes quality, bool proper, string expected)
        {
            var mocker = new AutoMoqer();

            var series = Builder<Series>.CreateNew()
                .With(c => c.Path = @"d:\tv shows\My Series Name")
                .Build();

            var parsResult = new EpisodeParseResult()
            {
                AirDate = DateTime.Now,
                EpisodeNumbers = episodes.ToList(),
                Quality = new Quality(quality, proper),
                SeasonNumber = seasons,
                Series = series,
                EpisodeTitle =  title
            };

            //Act
            var actual = mocker.Resolve<SabProvider>().GetSabTitle(parsResult);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(true, "My Series Name - Season 1 [Bluray720p] [Proper]")]
        [TestCase(false, "My Series Name - Season 1 [Bluray720p]")]
        public void sab_season_title(bool proper, string expected)
        {
            var mocker = new AutoMoqer();

            var series = Builder<Series>.CreateNew()
                .With(c => c.Path = @"d:\tv shows\My Series Name")
                .Build();

            var parsResult = new EpisodeParseResult()
            {
                AirDate = DateTime.Now,
                Quality = new Quality(QualityTypes.Bluray720p, proper),
                SeasonNumber = 1,
                Series = series,
                EpisodeTitle = "My Episode Title",
                FullSeason = true
            };

            //Act
            var actual = mocker.Resolve<SabProvider>().GetSabTitle(parsResult);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Explicit]
        public void AddNewzbingByUrlSuccess()
        {
            //Setup
            const string sabHost = "192.168.1.50";
            const int sabPort = 8080;
            const string apikey = "f37dc33baec2e5566f5aec666287870d";
            const string username = "root";
            const string password = "*************";
            const SabnzbdPriorityType priority = SabnzbdPriorityType.Normal;
            const string category = "tv";


            var mocker = new AutoMoqer();

            var fakeConfig = mocker.GetMock<ConfigProvider>();
            fakeConfig.SetupGet(c => c.SabHost)
                .Returns(sabHost);
            fakeConfig.SetupGet(c => c.SabPort)
                .Returns(sabPort);
            fakeConfig.SetupGet(c => c.SabApiKey)
                .Returns(apikey);
            fakeConfig.SetupGet(c => c.SabUsername)
                .Returns(username);
            fakeConfig.SetupGet(c => c.SabPassword)
                .Returns(password);
            fakeConfig.SetupGet(c => c.SabTvPriority)
                .Returns(priority);
            fakeConfig.SetupGet(c => c.SabTvCategory)
                .Returns(category);


            mocker.SetConstant(new HttpProvider());

            //Act
            bool result = mocker.Resolve<SabProvider>().AddByUrl(
                "http://www.newzbin.com/browse/post/6107863/nzb", "Added by unit tests.");

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void Get_Categories_Success_Passed_Values()
        {
            //Setup
            const string host = "192.168.5.55";
            const int port = 2222;
            const string apikey = "5c770e3197e4fe763423ee7c392c25d1";
            const string username = "admin";
            const string password = "pass";

            var mocker = new AutoMoqer();

            mocker.GetMock<HttpProvider>(MockBehavior.Strict)
                .Setup(s => s.DownloadString("http://192.168.5.55:2222/api?mode=get_cats&output=json&apikey=5c770e3197e4fe763423ee7c392c25d1&ma_username=admin&ma_password=pass"))
                .Returns(File.ReadAllText(@".\Files\Categories_json.txt"));

            //Act
            var result = mocker.Resolve<SabProvider>().GetCategories(host, port, apikey, username, password);

            //Assert
            result.Should().NotBeNull();
            result.categories.Should().HaveCount(c => c > 0);
            result.categories.Should().NotContain("*");
        }

        [Test]
        public void Get_Categories_Success_Config_Values()
        {
            //Setup
            const string host = "192.168.5.55";
            const int port = 2222;
            const string apikey = "5c770e3197e4fe763423ee7c392c25d1";
            const string username = "admin";
            const string password = "pass";

            var mocker = new AutoMoqer();

            var fakeConfig = mocker.GetMock<ConfigProvider>();
            fakeConfig.SetupGet(c => c.SabHost)
                .Returns(host);
            fakeConfig.SetupGet(c => c.SabPort)
                .Returns(port);
            fakeConfig.SetupGet(c => c.SabApiKey)
                .Returns(apikey);
            fakeConfig.SetupGet(c => c.SabUsername)
                .Returns(username);
            fakeConfig.SetupGet(c => c.SabPassword)
                .Returns(password);

            mocker.GetMock<HttpProvider>(MockBehavior.Strict)
                .Setup(s => s.DownloadString("http://192.168.5.55:2222/api?mode=get_cats&output=json&apikey=5c770e3197e4fe763423ee7c392c25d1&ma_username=admin&ma_password=pass"))
                .Returns(File.ReadAllText(@".\Files\Categories_json.txt"));

            //Act
            var result = mocker.Resolve<SabProvider>().GetCategories();

            //Assert
            result.Should().NotBeNull();
            result.categories.Should().HaveCount(c => c > 0);
            result.categories.Should().NotContain("*");
        }
    }
}