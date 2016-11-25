﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Windows;

namespace AnyStatus.Tests.Tests.Commands
{
    [TestClass]
    public class TestCommandTests
    {
        [TestMethod]
        public async void Should_ExecuteScheduledJob()
        {
            var logger = Substitute.For<ILogger>();
            var mediator = Substitute.For<IMediator>();
            var scheduledJob = Substitute.For<IScheduledJob>();
            Func<IScheduledJob> _jobFactory = () => { return scheduledJob; };

            var item = new Item { Name = "Test" };
            var command = new TestCommand(item, canExecute => { /* todo: test can execute */ }, message => { });
            var handler = new TestCommandHandler(_jobFactory);

            handler.Handle(command);

            await scheduledJob.Received(1).ExecuteAsync();

            Assert.AreSame(scheduledJob.Item, command.Item);
        }
    }
}
