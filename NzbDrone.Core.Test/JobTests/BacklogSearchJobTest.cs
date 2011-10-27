﻿using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.Model.Notification;
using NzbDrone.Core.Providers;
using NzbDrone.Core.Providers.Jobs;
using NzbDrone.Core.Repository;

namespace NzbDrone.Core.Test.JobTests
{
    [TestFixture]
    public class BacklogSearchJobTest
    {
        [Test]
        public void no_missing_epsiodes_should_not_trigger_any_search()
        {
            //Setup
            var notification = new ProgressNotification("Backlog Search Job Test");

            var episodes = new List<Episode>();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            mocker.GetMock<EpisodeProvider>()
                .Setup(s => s.EpisodesWithoutFiles(true)).Returns(episodes);

            //Act
            mocker.Resolve<BacklogSearchJob>().Start(notification, 0, 0);

            //Assert
            mocker.GetMock<SeasonSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), It.IsAny<int>()),
                                                       Times.Never());

            mocker.GetMock<EpisodeSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), 0),
                                                       Times.Never());
        }

        [Test]
        public void individual_missing_episode()
        {
            //Setup
            var notification = new ProgressNotification("Backlog Search Job Test");

            var episodes = Builder<Episode>.CreateListOfSize(1).Build();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            mocker.GetMock<EpisodeProvider>()
                .Setup(s => s.EpisodesWithoutFiles(true)).Returns(episodes);

            mocker.GetMock<EpisodeSearchJob>()
                .Setup(s => s.Start(notification, It.IsAny<int>(), 0)).Verifiable();

            //Act
            mocker.Resolve<BacklogSearchJob>().Start(notification, 0, 0);

            //Assert
            mocker.GetMock<SeasonSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), It.IsAny<int>()),
                                                       Times.Never());

            mocker.GetMock<EpisodeSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), 0),
                                                       Times.Once());
        }

        [Test]
        public void individual_missing_episodes_only()
        {
            //Setup
            var notification = new ProgressNotification("Backlog Search Job Test");

            var episodes = Builder<Episode>.CreateListOfSize(5).Build();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            mocker.GetMock<EpisodeProvider>()
                .Setup(s => s.EpisodesWithoutFiles(true)).Returns(episodes);

            mocker.GetMock<EpisodeSearchJob>()
                .Setup(s => s.Start(notification, It.IsAny<int>(), 0)).Verifiable();

            //Act
            mocker.Resolve<BacklogSearchJob>().Start(notification, 0, 0);

            //Assert
            mocker.GetMock<SeasonSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), It.IsAny<int>()),
                                                       Times.Never());

            mocker.GetMock<EpisodeSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), 0),
                                                       Times.Exactly(episodes.Count));
        }

        [Test]
        public void series_season_missing_episodes_only_mismatch_count()
        {
            //Setup
            var notification = new ProgressNotification("Backlog Search Job Test");

            var episodes = Builder<Episode>.CreateListOfSize(5)
                .All()
                .With(e => e.SeriesId = 1)
                .With(e => e.SeasonNumber = 1)
                .Build();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            mocker.GetMock<EpisodeProvider>()
                .Setup(s => s.EpisodesWithoutFiles(true)).Returns(episodes);

            mocker.GetMock<EpisodeSearchJob>()
                .Setup(s => s.Start(notification, It.IsAny<int>(), 0)).Verifiable();

            mocker.GetMock<EpisodeProvider>()
                .Setup(s => s.GetEpisodeNumbersBySeason(1, 1)).Returns(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            //Act
            mocker.Resolve<BacklogSearchJob>().Start(notification, 0, 0);

            //Assert
            mocker.GetMock<SeasonSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), It.IsAny<int>()),
                                                       Times.Never());

            mocker.GetMock<EpisodeSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), 0),
                                                       Times.Exactly(episodes.Count));
        }

        [Test]
        public void series_season_missing_episodes_only()
        {
            //Setup
            var notification = new ProgressNotification("Backlog Search Job Test");

            var episodes = Builder<Episode>.CreateListOfSize(5)
                .All()
                .With(e => e.SeriesId = 1)
                .With(e => e.SeasonNumber = 1)
                .Build();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            mocker.GetMock<EpisodeProvider>()
                .Setup(s => s.EpisodesWithoutFiles(true)).Returns(episodes);

            mocker.GetMock<SeasonSearchJob>()
                .Setup(s => s.Start(notification, It.IsAny<int>(), It.IsAny<int>())).Verifiable();

            mocker.GetMock<EpisodeProvider>()
                .Setup(s => s.GetEpisodeNumbersBySeason(1, 1)).Returns(episodes.Select(e => e.EpisodeNumber).ToList());

            //Act
            mocker.Resolve<BacklogSearchJob>().Start(notification, 0, 0);

            //Assert
            mocker.GetMock<SeasonSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), It.IsAny<int>()),
                                                       Times.Once());

            mocker.GetMock<EpisodeSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), 0),
                                                       Times.Never());
        }

        [Test]
        public void multiple_missing_episodes()
        {
            //Setup
            var notification = new ProgressNotification("Backlog Search Job Test");

            var episodes = Builder<Episode>.CreateListOfSize(10)
                .TheFirst(5)
                .With(e => e.SeriesId = 1)
                .With(e => e.SeasonNumber = 1)
                .Build();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            mocker.GetMock<EpisodeProvider>()
                .Setup(s => s.EpisodesWithoutFiles(true)).Returns(episodes);

            mocker.GetMock<SeasonSearchJob>()
                .Setup(s => s.Start(notification, It.IsAny<int>(), It.IsAny<int>())).Verifiable();

            mocker.GetMock<EpisodeSearchJob>()
                .Setup(s => s.Start(notification, It.IsAny<int>(), 0)).Verifiable();

            mocker.GetMock<EpisodeProvider>()
                .Setup(s => s.GetEpisodeNumbersBySeason(1, 1)).Returns(new List<int> { 1, 2, 3, 4, 5 });

            //Act
            mocker.Resolve<BacklogSearchJob>().Start(notification, 0, 0);

            //Assert
            mocker.GetMock<SeasonSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), It.IsAny<int>()),
                                                       Times.Once());

            mocker.GetMock<EpisodeSearchJob>().Verify(c => c.Start(notification, It.IsAny<int>(), 0),
                                                       Times.Exactly(5));
        }
    }
}