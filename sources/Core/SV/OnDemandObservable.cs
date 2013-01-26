
namespace SV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Subjects;

    /// <summary>
    ///     An observable sequence which generates elements if at least one subscriber exists.
    /// </summary>
    /// <typeparam name="TResult">
    ///     The type of the elements in the produced sequence.
    /// </typeparam>
    /// <remarks>
    ///     This is a combination of cold and hot subscription. In iddle state, the sequence doesn't generate elements. When somebody subscribes to a sequence, the sequence 
    ///     calls custom subscribe action (passed via constructor) in order to start receiving elements and pass them to subscribers. All new subscribers will receive "hot" 
    ///     elements, i.e. elements, which are received after subscription. When all subscribers kill their subscripton, the sequece stops generating elements by killing its
    ///     own subscription created by calling custom subscribe methods passed via constructor.
    /// </remarks>
    public class OnDemandObservable<TResult> : ObservableBase<TResult>, IDisposable
    {
        #region Fields

        private readonly List<IObserver<TResult>> subscribers = new List<IObserver<TResult>>();

        private readonly Func<IObserver<TResult>, Action> subscribe;

        private Subject<TResult> results;

        private Action unsubscribe;

        private bool isDisposed;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="OnDemandObservable{TResult}"/> class.
        /// </summary>
        /// <param name="subscribe">
        ///     Implementation of the resulting observable sequence's Subscribe method, returning an <see cref="Action"/> delegate that will be wrapped in an <see cref="IDisposable"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="subscribe"/> is <c>null</c>.
        /// </exception>
        public OnDemandObservable(Func<IObserver<TResult>, Action> subscribe)
        {
            subscribe.EnsureNotNull("subscribe");

            this.subscribe = subscribe;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Cleanups all resorces.
        /// </summary>
        /// <param name="disposing">
        ///     A flag indicating whether the method was called by <see cref="Dispose"/>.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed == false)
            {
                if (disposing)
                {
                    this.results.Dispose();
                }

                this.results = null;
                this.isDisposed = true;
            }
        }

        /// <summary>
        ///     Implement this method with the core subscription logic for the observable sequence.
        /// </summary>
        /// <param name="observer">
        ///     An observer to send notifications to.
        /// </param>
        /// <returns>
        ///     Disposable object representing an observer's subscription to the observable sequence.
        /// </returns>
        protected override IDisposable SubscribeCore(IObserver<TResult> observer)
        {
            lock (this.subscribers)
            {
                this.subscribers.Add(observer);

                if (this.results == null)
                {
                    this.results = new Subject<TResult>();
                    this.unsubscribe = this.subscribe(results);
                }

                this.results.Subscribe(observer);
            }

            return Disposable.Create(() => this.UnscubscribeCore(observer));
        }

        private void UnscubscribeCore(IObserver<TResult> observer)
        {
            lock (this.subscribers)
            {
                this.subscribers.Remove(observer);

                if (this.subscribers.Any() == false)
                {
                    this.unsubscribe();
                    this.unsubscribe = null;

                    this.results.Dispose();
                    this.results = null;
                }
            }
        }

        #endregion
    }
}
