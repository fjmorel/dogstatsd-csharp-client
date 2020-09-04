namespace StatsdClient.Worker
{
    internal interface IAsynchronousWorkerHandler<T>
    {
        /// <summary>
        /// Called when a new value is ready to be handled by the worker.
        /// </summary>
        void OnNewValue(ref T v);

        /// <summary>
        /// Called when the worker is waiting for new value to handle.
        /// </summary>
        /// <returns>Return true to make the worker in a sleep state, false otherwise.</returns>
        bool OnIdle();

        /// <summary>
        /// Called when AsynchronousWorker is shutdown.
        /// </summary>
        void OnShutdown();
    }
}