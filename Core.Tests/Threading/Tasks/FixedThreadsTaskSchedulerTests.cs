﻿using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarinaStudio.Threading.Tasks
{
	/// <summary>
	/// Tests of <see cref="FixedThreadsTaskScheduler"/>.
	/// </summary>
	[TestFixture]
	class FixedThreadsTaskSchedulerTests
	{
		// Fields.
		readonly Random random = new Random();


		/// <summary>
		/// Test for disposing task scheduler.
		/// </summary>
		[Test]
		public void DisposingTest()
		{
			// prepare
			using var taskScheduler = new FixedThreadsTaskScheduler(2);
			var taskFactory = new TaskFactory(taskScheduler);

			// run tasks
			this.RunTasksAndWait(taskScheduler, taskFactory, 100);

			// dispose
			taskScheduler.Dispose();
			Thread.Sleep(1000);
			Assert.AreEqual(0, taskScheduler.ExecutionThreadCount, "No active execution thread allowed after disposing");

			// try run task
			try
			{
				taskFactory.StartNew(() => { });
				throw new AssertionException("Should not allow starting new task after disposing.");
			}
			catch(Exception ex)
			{
				if (ex is AssertionException)
					throw;
			}
		}


		// Run given number of tasks and wait for completion.
		void RunTasksAndWait(FixedThreadsTaskScheduler taskScheduler, TaskFactory taskFactory, int numberOfTasks)
		{
			var exception = (Exception?)null;
			var syncLock = new object();
			var numberOfRunTasks = 0;
			lock (syncLock)
			{
				for (var i = 0; i < numberOfTasks; ++i)
				{
					taskFactory.StartNew(() =>
					{
						try
						{
							Assert.IsTrue(taskScheduler.IsExecutionThread, "Task is not run on correct execution thread.");
							Thread.Sleep(this.random.Next(10, 50));
						}
						catch (Exception ex)
						{
							exception = ex;
						}
						finally
						{
							lock (syncLock)
							{
								++numberOfRunTasks;
								if (numberOfRunTasks == numberOfTasks || exception != null)
									Monitor.Pulse(syncLock);
							}
						}
					});
				}
				Assert.IsTrue(Monitor.Wait(syncLock, numberOfTasks * 50 + 10000), "Cannot complete waiting for task running.");
			}
			if (exception != null)
				throw new AssertionException("Error occurred while running task.", exception);
		}


		/// <summary>
		/// Test for awaiting tasks.
		/// </summary>
		[Test]
		public async Task TaskAwaitingTest()
		{
			// prepare
			using var taskScheduler = new FixedThreadsTaskScheduler(1);
			var taskFactory = new TaskFactory<int>(taskScheduler);

			// run task and wait
			var taskRun = false;
			var result = await taskFactory.StartNew(() =>
			{
				Thread.Sleep(1000);
				taskRun = true;
				return 1234;
			});

			// check result
			Assert.IsTrue(taskRun, "Task not be run correctly.");
			Assert.AreEqual(1234, result, "Result generated by task is incorrect.");
		}


		/// <summary>
		/// Test for task execution.
		/// </summary>
		[Test]
		public void TaskExecutionTest()
		{
			this.TaskExecutionTest(1);
			this.TaskExecutionTest(32);
		}


		// Test for task execution.
		void TaskExecutionTest(int maxConcurrencyLevel)
		{
			// prepare
			using var taskScheduler = new FixedThreadsTaskScheduler(maxConcurrencyLevel);
			var taskFactory = new TaskFactory(taskScheduler);

			// run single task
			this.RunTasksAndWait(taskScheduler, taskFactory, 1);
			Assert.AreEqual(1, taskScheduler.ExecutionThreadCount, "Number of execution threads should be 1.");

			// run single task again
			this.RunTasksAndWait(taskScheduler, taskFactory, 1);
			Assert.AreEqual(1, taskScheduler.ExecutionThreadCount, "Number of execution threads should be 1.");

			// run lots of tasks
			this.RunTasksAndWait(taskScheduler, taskFactory, 1234);
			Assert.GreaterOrEqual(taskScheduler.MaximumConcurrencyLevel, taskScheduler.ExecutionThreadCount, "Too many execution thread.");
		}
	}
}
