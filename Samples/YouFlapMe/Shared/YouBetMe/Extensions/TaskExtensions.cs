using System;
using System.Threading.Tasks;
using System.Threading;

namespace Youbetme.Extensions
{
	public static class TaskExtensions
	{
		public static Task LogOnFaulted(this Task task)
		{
			return task.ContinueWith(antecedentTask =>
			{
				foreach (var ex in antecedentTask.Exception.InnerExceptions)
					System.Diagnostics.Debug.WriteLine(ex.ToString());
										
			}, TaskContinuationOptions.OnlyOnFaulted);
		}

		public static Task OnFaulted(this Task task, Action action)
		{
			return task.ContinueWith(antecedentTask => action(), 
				TaskContinuationOptions.OnlyOnFaulted)
					.LogOnFaulted();
		}

		public static Task OnFaultedCurrentThread(this Task task, Action action)
		{
			return task.ContinueWith(antecedentTask => action(), 
				CancellationToken.None, 
				TaskContinuationOptions.OnlyOnFaulted, 
				TaskScheduler.FromCurrentSynchronizationContext())
					.LogOnFaulted();
		}

		public static Task OnSuccess(this Task task, Action action)
		{
			return task.ContinueWith(antecedentTask => action(), 
				TaskContinuationOptions.OnlyOnRanToCompletion)
					.LogOnFaulted();
		}

		public static Task OnSuccessCurrentThread(this Task task, Action action)
		{
			return task.ContinueWith(antecedentTask => action(),
				CancellationToken.None, 
				TaskContinuationOptions.OnlyOnRanToCompletion, 
				TaskScheduler.FromCurrentSynchronizationContext())
					.LogOnFaulted();
		}

		public static Task OnSuccessCurrentThread(this Task<bool> task, Action<Task<bool>> action)
		{
			return task.ContinueWith(action, 
				CancellationToken.None, 
				TaskContinuationOptions.OnlyOnRanToCompletion, 
				TaskScheduler.FromCurrentSynchronizationContext())
					.LogOnFaulted();
		}
	}
}
