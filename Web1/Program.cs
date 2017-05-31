using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Web1
{
	internal static class Program
	{
		public static bool IsServiceFabricAvailable
		{
			get
			{
				try
				{
					FabricRuntime.GetNodeContext();
					return true;
				}
				catch (FabricException sfEx) when (sfEx.HResult == -2147017661 || sfEx.HResult == -2147017536 || sfEx.InnerException?.HResult == -2147017536)
				{
					return false;
				}
			}
		}

		/// <summary>
		/// This is the entry point of the service host process.
		/// </summary>
		private static void Main()
		{
			if (!IsServiceFabricAvailable)
			{
				int iteration = 1;
				while (true)
				{
					Console.WriteLine($"Iteration: {iteration}");
					Debug.WriteLine($"Iteration: {iteration}");
					iteration++;
					Thread.Sleep(TimeSpan.FromSeconds(2));
				}
			}

			try
			{
				// The ServiceManifest.XML file defines one or more service type names.
				// Registering a service maps a service type name to a .NET type.
				// When Service Fabric creates an instance of this service type,
				// an instance of the class is created in this host process.

				ServiceRuntime.RegisterServiceAsync("Web1Type",
					context => new Web1(context)).GetAwaiter().GetResult();

				ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(Web1).Name);

				// Prevents this host process from terminating so services keeps running.
				Thread.Sleep(Timeout.Infinite);
			}
			catch (Exception e)
			{
				ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
				throw;
			}
		}
	}
}
