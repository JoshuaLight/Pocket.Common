﻿using System;
using System.Collections.Generic;

namespace Pocket.Flows
{
    internal sealed class DispatchedCollectionFlow<T> : ICollectionFlow<T>
    {
        private readonly ICollectionFlow<T> _inner;

        public DispatchedCollectionFlow(ICollectionFlow<T> inner, Action<Action> dispatcher)
        {
            _inner = inner;

            Added = inner.Added.Dispatched(@by: dispatcher);
            Removed = inner.Removed.Dispatched(@by: dispatcher);
        }

        public IEnumerable<T> Current => _inner.Current;

        public IFlow<T> Added { get; }
        public IFlow<T> Removed { get; }
    }
}
